#region GNU license
// MyFilms - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

namespace MyFilmsPlugin.MyFilms
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.DataSetExtensions;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Xml;

  using MediaPortal.Configuration;
  using MediaPortal.GUI.Library;

  using MyFilmsPlugin.MyFilms.MyFilmsGUI;
  using MyFilmsPlugin.MyFilms.Utils;
  using MyFilmsPlugin.DataBase;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;

  public class BaseMesFilms
    {
        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
        private static AntMovieCatalog data; // Ant compatible File - with temp extended fields and person infos
        //private static XmlDataDocument xmlDoc; // XML Doc file for chierarchical access like XPath

        public static ReaderWriterLockSlim _dataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        public class MFConfig
        {
          public string Name
          {
            get { return name; }
            set { name = value; }
          } private string name;

          public List<KeyValuePair<string, string>> ViewList
          {
            get { return viewList; }
            set { viewList = value; }
          }
          private List<KeyValuePair<string, string>> viewList;
        }

        public enum MostRecentType
        {
          Watched,
          Added
        }
    
        //private static Dictionary<string, string> dataPath;
        private static DataRow[] movies;          // selected movies with filters
        //private static DataTable tableMoviesExtended;  // all movies as join with customfields
        private static Stopwatch watch = new Stopwatch();

        #region ctor
        static BaseMesFilms() {}
        #endregion

        #region private static methods ...
        private static void initData()
        {
          Stopwatch initDataWatch = new Stopwatch();
          initDataWatch.Reset(); initDataWatch.Start();
          bool success = LoadMyFilmsFromDisk(MyFilms.conf.StrFileXml);
          initDataWatch.Stop();
          LogMyFilms.Debug("initData() - End reading catalogfile '" + MyFilms.conf.StrFileXml + "' (success = '" + success + "') (" + (initDataWatch.ElapsedMilliseconds) + " ms)");
        }

        private static DataTable DataTableJoiner(DataTable dt1, DataTable dt2)
        {
          using (DataTable targetTable = dt1.Clone())
          {
            var dt2Query = dt2.Columns.OfType<DataColumn>().Select(dc =>
                new DataColumn(dc.ColumnName, dc.DataType, dc.Expression, dc.ColumnMapping));
            var dt2FilterQuery = from dc in dt2Query.AsEnumerable()
                                 where targetTable.Columns.Contains(dc.ColumnName) == false
                                 select dc;
            targetTable.Columns.AddRange(dt2FilterQuery.ToArray());
            var rowData = from row1 in dt1.AsEnumerable()
                          join row2 in dt2.AsEnumerable()
                          on row1.Field<int>("Movie_ID") equals row2.Field<int>("Movie_ID")
                          select row1.ItemArray.Concat(row2.ItemArray.Where(r2 => row1.ItemArray.Contains(r2) == false)).ToArray();
            foreach (object[] values in rowData) targetTable.Rows.Add(values);
            return targetTable;
          }
        }

        private static DataTable DataTableJoiner2(DataTable dt1, DataTable dt2)
        {
          var commonColumns = dt1.Columns.OfType<DataColumn>().Intersect(dt2.Columns.OfType<DataColumn>(), new DataColumnComparer());

          var result = new DataTable();
          result.Columns.AddRange(
              dt1.Columns.OfType<DataColumn>()
              .Union(dt2.Columns.OfType<DataColumn>(), new DataColumnComparer())
              .Select(c => new DataColumn(c.Caption, c.DataType, c.Expression, c.ColumnMapping))
              .ToArray());

          var rowData = dt1.AsEnumerable().Join(
              dt2.AsEnumerable(),
              row => commonColumns.Select(col => row[col.Caption]).ToArray(),
              row => commonColumns.Select(col => row[col.Caption]).ToArray(),
              (row1, row2) =>
              {
                var row = result.NewRow();
                row.ItemArray = result.Columns.OfType<DataColumn>().Select(col => row1.Table.Columns.Contains(col.Caption) ? row1[col.Caption] : row2[col.Caption]).ToArray();
                return row;
              },
              new ObjectArrayComparer());

          foreach (var row in rowData)
            result.Rows.Add(row);

          return result;
        }

        private class DataColumnComparer : IEqualityComparer<DataColumn>
        {

          #region IEqualityComparer<DataColumn> Members

          public bool Equals(DataColumn x, DataColumn y)
          {
            return x.Caption == y.Caption;
          }

          public int GetHashCode(DataColumn obj)
          {
            return obj.Caption.GetHashCode();
          }

          #endregion
        }

        private class ObjectArrayComparer : IEqualityComparer<object[]>
        {
          #region IEqualityComparer<object[]> Members

          public bool Equals(object[] x, object[] y)
          {
            for (var i = 0; i < x.Length; i++)
            {
              if (!object.Equals(x[i], y[i]))
                return false;
            }

            return true;
          }

          public int GetHashCode(object[] obj)
          {
            return obj.Sum(item => item.GetHashCode());
          }

          #endregion
        }

        private static DataTable GetEnhancedMovies()
        {
          var movies_enhanced =
              from o in data.Movie
              //where   o.Year == "1998"
              orderby o.OriginalTitle ascending
              select new
              {
                o.Movie_Id,
                o.Number,
                o.Checked,
                o.MediaLabel,
                o.MediaType,
                o.Source,
                o.Date,
                o.Borrower,
                o.Rating,
                o.OriginalTitle,
                o.TranslatedTitle,
                o.FormattedTitle,
                o.IndexedTitle,
                o.Director,
                o.Producer,
                o.Country,
                o.Category,
                o.Year,
                o.Length,
                o.Actors,
                o.URL,
                o.Description,
                o.Comments,
                o.VideoFormat,
                o.VideoBitrate,
                o.AudioFormat,
                o.AudioBitrate,
                o.Resolution,
                o.Framerate,
                o.Languages,
                o.Subtitles,
                o.Size,
                o.DateAdded,
                o.RecentlyAdded,
                o.AgeAdded,
                o.Disks,
                o.Picture,
                o.Length_Num,
                o.Persons,
                //o.CustomField1,
                //o.CustomField2,
                //o.CustomField3,
                //o.RatingUser,
                //o.Edition,
                //o.Fanart,
                //o.Certification,
                //o.Writer,
                //o.Watched,
                //o.Favorite,
                //o.IMDB_Id,
                //o.TMDB_Id,
                //o.SourceTrailer,
                //o.TagLine,
                //o.Tags,
                //o.Studio,
                //o.IMDB_Rank,
                o.IsOnline,
                o.IsOnlineTrailer,
                //o.Aspectratio,
                //o.CategoryTrakt,
                //o.LastPosition,
                //o.AudioChannelCount,
                o.GetCustomFieldsRows()[0].CustomField1,
                o.GetCustomFieldsRows()[0].CustomField2,
                o.GetCustomFieldsRows()[0].CustomField3,
                o.GetCustomFieldsRows()[0].Edition,
                o.GetCustomFieldsRows()[0].Studio,
                o.GetCustomFieldsRows()[0].Fanart,
                o.GetCustomFieldsRows()[0].Certification,
                o.GetCustomFieldsRows()[0].Writer,
                o.GetCustomFieldsRows()[0].TagLine,
                o.GetCustomFieldsRows()[0].Tags,
                o.GetCustomFieldsRows()[0].Aspectratio,
                o.GetCustomFieldsRows()[0].CategoryTrakt,
                o.GetCustomFieldsRows()[0].Watched,
                o.GetCustomFieldsRows()[0].Favorite,
                o.GetCustomFieldsRows()[0].RatingUser,
                o.GetCustomFieldsRows()[0].IMDB_Id,
                o.GetCustomFieldsRows()[0].TMDB_Id,
                o.GetCustomFieldsRows()[0].IMDB_Rank,
                o.GetCustomFieldsRows()[0].SourceTrailer,
                //o.GetCustomFieldsRows()[0].IsOnline,
                //o.GetCustomFieldsRows()[0].IsOnlineTrailer,
                o.GetCustomFieldsRows()[0].LastPosition,
                o.GetCustomFieldsRows()[0].AudioChannelCount
              };
          return LINQToDataTable(movies_enhanced);
        }

        private DataTable JoinDataTables(DataTable LeftTable, DataTable RightTable, String LeftPrimaryColumn, String RightPrimaryColumn)
        {
          //first create the datatable columns 
          DataSet mydataSet = new DataSet();
          mydataSet.Tables.Add("  ");
          DataTable myDataTable = mydataSet.Tables[0];

          //add left table columns 
          DataColumn[] dcLeftTableColumns = new DataColumn[LeftTable.Columns.Count];
          LeftTable.Columns.CopyTo(dcLeftTableColumns, 0);

          foreach (DataColumn LeftTableColumn in dcLeftTableColumns)
          {
            if (!myDataTable.Columns.Contains(LeftTableColumn.ToString()))
              myDataTable.Columns.Add(LeftTableColumn.ToString());
          }

          //now add right table columns 
          DataColumn[] dcRightTableColumns = new DataColumn[RightTable.Columns.Count];
          RightTable.Columns.CopyTo(dcRightTableColumns, 0);

          foreach (DataColumn RightTableColumn in dcRightTableColumns)
          {
            if (!myDataTable.Columns.Contains(RightTableColumn.ToString()))
            {
              if (RightTableColumn.ToString() != RightPrimaryColumn)
                myDataTable.Columns.Add(RightTableColumn.ToString());
            }
          }

          //add left-table data to mytable 
          foreach (DataRow LeftTableDataRows in LeftTable.Rows)
          {
            myDataTable.ImportRow(LeftTableDataRows);
          }

          ArrayList var = new ArrayList(); //this variable holds the id's which have joined 

          ArrayList LeftTableIDs = new ArrayList();
          LeftTableIDs = this.DataSetToArrayList(0, LeftTable);

          //import righttable which having not equal Id's with lefttable 
          foreach (DataRow rightTableDataRows in RightTable.Rows)
          {
            if (LeftTableIDs.Contains(rightTableDataRows[0]))
            {
              string wherecondition = "[" + myDataTable.Columns[0].ColumnName + "]='" + rightTableDataRows[0] + "'";
              DataRow[] dr = myDataTable.Select(wherecondition);
              int iIndex = myDataTable.Rows.IndexOf(dr[0]);

              foreach (DataColumn dc in RightTable.Columns)
              {
                if (dc.Ordinal != 0)
                  myDataTable.Rows[iIndex][dc.ColumnName.Trim()] = rightTableDataRows[dc.ColumnName.Trim()].ToString();
              }
            }
            else
            {
              int count = myDataTable.Rows.Count;
              DataRow row = myDataTable.NewRow();
              row[0] = rightTableDataRows[0].ToString();
              myDataTable.Rows.Add(row);
              foreach (DataColumn dc in RightTable.Columns)
              {
                if (dc.Ordinal != 0)
                  myDataTable.Rows[count][dc.ColumnName.Trim()] = rightTableDataRows[dc.ColumnName.Trim()].ToString();
              }
            }
          }
          return myDataTable;
        }

        private ArrayList DataSetToArrayList(int ColumnIndex, DataTable dataTable)
        {
          ArrayList output = new ArrayList();

          foreach (DataRow row in dataTable.Rows)
            output.Add(row[ColumnIndex]);

          return output;
        } 

        private static DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
        {
             DataTable dtReturn = new DataTable();

             // column names 
             PropertyInfo[] oProps = null;

             if (varlist == null) return dtReturn;

             foreach (T rec in varlist)
             {
                  // Use reflection to get property names, to create table, Only first time, others will follow 
                  if (oProps == null)
                  {
                       oProps = ((Type)rec.GetType()).GetProperties();
                       foreach (PropertyInfo pi in oProps)
                       {
                            Type colType = pi.PropertyType;

                            if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()      
                            ==typeof(Nullable<>)))
                             {
                                 colType = colType.GetGenericArguments()[0];
                             }

                            dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                       }
                  }

                  DataRow dr = dtReturn.NewRow();

                  foreach (PropertyInfo pi in oProps)
                  {
                       dr[pi.Name] = pi.GetValue(rec, null) ?? DBNull.Value;
                  }

                  dtReturn.Rows.Add(dr);
             }
             return dtReturn;
        }

        private static DataTable ObjectArrayToDataTable(object[] data)
        {
          DataTable dt = new DataTable();
          if (data.Length == 0) return dt; // if data is empty, return an empty table

          Type t = data[0].GetType();
          System.Reflection.PropertyInfo[] piList = t.GetProperties();

          foreach (System.Reflection.PropertyInfo p in piList)
          {
            dt.Columns.Add(new DataColumn(p.Name, p.PropertyType));
          }

          object[] row = new object[piList.Length];

          foreach (object obj in data)
          {
            int i = 0;
            foreach (System.Reflection.PropertyInfo pi in piList)
            {
              row[i++] = pi.GetValue(obj, null);
            }
            dt.Rows.Add(row);
          }
          return dt;
        }

        private static string GetNameForMyFilmsDatafile(string catalogfullpath)
        {
          string path = System.IO.Path.GetDirectoryName(catalogfullpath);
          string filename = System.IO.Path.GetFileNameWithoutExtension(catalogfullpath);
          string extension = System.IO.Path.GetExtension(catalogfullpath);
          string datafile = path + @"\" + filename + "_MFdata." + extension;
          return datafile;
        }

        private static void CreateEmptyDataFile(string datafile)
        {
          XmlTextWriter destXml = new XmlTextWriter(datafile, System.Text.Encoding.Default);
          destXml.Formatting = Formatting.Indented;
          destXml.WriteStartDocument();
          destXml.WriteStartElement("MyFilmsData");
          destXml.WriteStartElement("Persons");
          destXml.WriteElementString("Properties", string.Empty);
          destXml.WriteStartElement("Contents");
          destXml.WriteStartElement("History");
          destXml.WriteElementString("Properties", string.Empty);
          destXml.WriteStartElement("Contents");
          destXml.Close();
        }

        private static void CopyMovieExtendedDataToCustomFields(bool cleanfileonexit)
        {
          Stopwatch saveDataWatch = new Stopwatch();
          saveDataWatch.Reset(); saveDataWatch.Start();
          var commonColumns = data.Movie.Columns.OfType<DataColumn>().Intersect(data.CustomFields.Columns.OfType<DataColumn>(), new DataColumnComparer());
          foreach (AntMovieCatalog.MovieRow movieRow in data.Movie)
          {
            AntMovieCatalog.CustomFieldsRow customFields = movieRow.GetCustomFieldsRows()[0];
            foreach (DataColumn dc in commonColumns)
            {
              if (movieRow[dc.ColumnName] != DBNull.Value && dc.ColumnName != "Movie_Id")
              {
                customFields[dc.ColumnName] = movieRow[dc.ColumnName];
                if (cleanfileonexit) movieRow[dc.ColumnName] = DBNull.Value;
              }
            }
          }
          saveDataWatch.Stop();
          LogMyFilms.Debug("CopyMovieExtendedDataToCustomFields() - Copy CustomFields from MovieRow's done ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");
        }

        private static void CreateOrUpdateCustomsFieldsProperties()
        {
          Stopwatch saveDataWatch = new Stopwatch(); saveDataWatch.Reset(); saveDataWatch.Start();

          // add Properties, if missing //<Properties Owner="SampleOwner" Mail="myfilms@gmail.com" Site="http://www.team-mediaportal.com" Description="Sample description" />
          AntMovieCatalog.PropertiesRow[] propCollection = data.Catalog[0].GetPropertiesRows();
          if (propCollection.Length == 0)
          {
            AntMovieCatalog.PropertiesRow prop;
            prop = data.Properties.NewPropertiesRow();
            prop.SetParentRow(data.Catalog[0]);
            prop.Owner = "MyFilms";
            prop.Description = "AMC Movie Database";
            prop.Site = "http://www.team-mediaportal.com";
            data.Properties.AddPropertiesRow(prop);
          }
          saveDataWatch.Stop();
          LogMyFilms.Debug("CreateOrUpdateCustomsFieldsProperties() - Add Properties done ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");

          // add CustomFields Definitions, if missing
          saveDataWatch.Reset(); saveDataWatch.Start();
          AntMovieCatalog.CustomFieldsPropertiesRow[] cfpCollection = data.Catalog[0].GetCustomFieldsPropertiesRows();
          if (cfpCollection.Length == 0)
          {
            AntMovieCatalog.CustomFieldsPropertiesRow cfp;
            cfp = data.CustomFieldsProperties.NewCustomFieldsPropertiesRow();
            cfp.SetParentRow(data.Catalog[0]);
            data.CustomFieldsProperties.AddCustomFieldsPropertiesRow(cfp);
          }
          ArrayList cFields = new ArrayList();
          AntMovieCatalog.CustomFieldRow[] cfrCollection = data.CustomFieldsProperties[0].GetCustomFieldRows();

          foreach (AntMovieCatalog.CustomFieldRow fieldRow in cfrCollection) 
            cFields.Add(fieldRow.Tag);

          List<string[]> CustomFieldList = new List<string[]>(); // Tag, Name, Type - // ftString, ftInteger, ftReal, ftBoolean, ftDate, ftList, ftText, ftUrl
          // <CustomField Tag="Edition" Name="Edition" Type="ftString" GUIProperties="rx6:ry51:rw526:rh25:aw1:ah0:lw94" />
          //CustomFieldList.Add(new string[] { "IndexedTitle", "IndexedTitle", "ftString" });
          CustomFieldList.Add(new string[] { "Edition", "Edition", "ftString" });
          CustomFieldList.Add(new string[] { "Studio", "Studio", "ftString" });
          CustomFieldList.Add(new string[] { "Fanart", "Fanart", "ftString" });
          CustomFieldList.Add(new string[] { "Certification", "Certification", "ftString" });
          CustomFieldList.Add(new string[] { "Writer", "Writer", "ftString" });
          CustomFieldList.Add(new string[] { "TagLine", "TagLine", "ftString" });
          CustomFieldList.Add(new string[] { "Tags", "Tags", "ftString" });
          CustomFieldList.Add(new string[] { "Aspectratio", "Aspectratio", "ftString" });
          CustomFieldList.Add(new string[] { "CategoryTrakt", "CategoryTrakt", "ftString" });
          CustomFieldList.Add(new string[] { "Watched", "Watched", "ftString" });
          //CustomFieldList.Add(new string[] { "RecentlyAdded", "RecentlyAdded", "ftString" });
          //CustomFieldList.Add(new string[] { "AgeAdded", "AgeAdded" ,"ftString"});
          CustomFieldList.Add(new string[] { "Favorite", "Favorite", "ftString" });
          CustomFieldList.Add(new string[] { "RatingUser", "RatingUser", "ftString" });
          CustomFieldList.Add(new string[] { "IMDB_Id", "IMDB_Id", "ftString" });
          CustomFieldList.Add(new string[] { "TMDB_Id", "TMDB_Id", "ftString" });
          CustomFieldList.Add(new string[] { "IMDB_Rank", "IMDB_Rank", "ftString" });
          CustomFieldList.Add(new string[] { "SourceTrailer", "SourceTrailer", "ftString" });
          CustomFieldList.Add(new string[] { "IsOnline", "IsOnline", "ftString" });
          CustomFieldList.Add(new string[] { "IsOnlineTrailer", "IsOnlineTrailer", "ftString" });
          CustomFieldList.Add(new string[] { "LastPosition", "LastPosition", "ftString" });
          CustomFieldList.Add(new string[] { "AudioChannelCount", "AudioChannelCount", "ftString" });
          CustomFieldList.Add(new string[] { "CustomField1", "CustomField1", "ftString" });
          CustomFieldList.Add(new string[] { "CustomField2", "CustomField2", "ftString" });
          CustomFieldList.Add(new string[] { "CustomField3", "CustomField3", "ftString" });

          foreach (string[] stringse in CustomFieldList)
          {
            if (!cFields.Contains(stringse[0]))
            {
              AntMovieCatalog.CustomFieldRow cfr;
              cfr = data.CustomField.NewCustomFieldRow();
              cfr.SetParentRow(data.CustomFieldsProperties[0]);
              cfr.Tag = stringse[0];
              cfr.Name = (!string.IsNullOrEmpty(Translate_Column(stringse[0]))) ? Translate_Column(stringse[0]) : stringse[1];
              cfr.Type = stringse[2];
              data.CustomField.AddCustomFieldRow(cfr);
            }
          }
          saveDataWatch.Stop();
          LogMyFilms.Debug("CreateOrUpdateCustomsFieldsProperties() - Adding CustomFields Definitions done ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");
        }

        private static bool SaveMyFilmsToDisk(string catalogfile)
        {
          watch.Reset(); watch.Start();
          bool success = false; // result of write operation
          int maxretries = 5; // max retries 10 * 1000 = 10 seconds
          int i = 0;

          while (!success && i < maxretries)
          {
            if (!MyFilmsDetail.GlobalLockIsActive(catalogfile)) // first check, if there is a global manual lock
            {
              try
              {
                MyFilmsDetail.SetGlobalLock(true, catalogfile);

                using (FileStream fs = new FileStream(catalogfile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) // lock the file for any other use, as we do write to it now !
                {
                  LogMyFilms.Debug("SaveMyFilmsToDisk()- opening '" + catalogfile + "' as FileStream with FileMode.OpenOrCreate, FileAccess.Write, FileShare.None");
                  fs.SetLength(0); // do not append, owerwrite !
                  using (XmlTextWriter MyXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.Default))
                  {
                    LogMyFilms.Debug("SaveMyFilmsToDisk()- writing '" + catalogfile + "' as MyXmlTextWriter in FileStream");
                    MyXmlTextWriter.Formatting = System.Xml.Formatting.Indented;
                    MyXmlTextWriter.WriteStartDocument();
                    data.WriteXml(MyXmlTextWriter, XmlWriteMode.IgnoreSchema);
                    MyXmlTextWriter.Flush();
                    MyXmlTextWriter.Close();
                  }
                  //xmlDoc.Save(fs);
                  fs.Close(); // write buffer and release lock on file (either Flush, Dispose or Close is required)
                  LogMyFilms.Debug("SaveMyFilmsToDisk()- closing '" + catalogfile + "' FileStream and releasing file lock");
                  success = true;
                }
              }
              catch (Exception ex)
              {
                LogMyFilms.DebugException("SaveMyFilmsToDisk()- error saving '" + catalogfile + "' as FileStream with FileMode.OpenOrCreate, FileAccess.Write, FileShare.None", ex);
                // LogMyFilms.Debug("Commit()- exception while trying to save data in '" + catalogfile + "' - exception: " + saveexeption.Message + ", stacktrace: " + saveexeption.StackTrace);
                success = false;
              }
              finally
              {
                MyFilmsDetail.SetGlobalLock(false, catalogfile);
              }
            }
            else
            {
              i += 1;
              LogMyFilms.Info("SaveMyFilmsToDisk() - Movie Database locked on try '" + i + " of " + maxretries + "' to write, waiting for next retry");
              Thread.Sleep(1000);
            }
          }
          watch.Stop();
          LogMyFilms.Debug("SaveMyFilmsToDisk() - Finished Saving ... (" + (watch.ElapsedMilliseconds) + " ms)");
          return success;
        }

        private static bool LoadMyFilmsFromDisk(string catalogfile)
        {
          #region load catalog from file into dataset
          bool success = false;
          LogMyFilms.Debug("LoadMyFilmsFromDisk()- Current Readlocks: '" + _dataLock.CurrentReadCount + "'");
          //if (_dataLock.CurrentReadCount > 0) return false;// might be opened by API as well, so count can be 2+

          watch.Reset(); watch.Start();
          _dataLock.EnterReadLock();
          data = new AntMovieCatalog();
          try
          {
            using (FileStream fs = new FileStream(catalogfile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              LogMyFilms.Debug("LoadMyFilmsFromDisk()- opening '" + catalogfile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");
              foreach (DataTable dataTable in data.Tables) dataTable.BeginLoadData();
              //// synchronize dataset with hierarchical XMLdoc
              //xmlDoc = new XmlDataDocument(data);
              //xmlDoc.Load(fs);
              data.ReadXml(fs);
              foreach (DataTable dataTable in data.Tables) dataTable.EndLoadData();
              fs.Close();
              LogMyFilms.Debug("LoadMyFilmsFromDisk()- closing  '" + catalogfile + "' FileStream");
            }
            success = true;
            //foreach (DataTable dataTable in data.Tables)
            //{
            //  LogMyFilms.Debug("initData() - loaded table '" + dataTable + "'");
            //  foreach (var childrelation in dataTable.ChildRelations) LogMyFilms.Debug("initData() - childrelation: '" + childrelation + "'");
            //}
          }
          catch (Exception e)
          {
            success = false;
            LogMyFilms.Error("LoadMyFilmsFromDisk() : Error reading xml database after " + data.Movie.Count + " records; error : " + e.Message + ", " + e.StackTrace);
            LogMyFilms.Error("LoadMyFilmsFromDisk() : Last Record: '" + data.Movie[data.Movie.Count - 1].Number + "', title: '" + data.Movie[data.Movie.Count - 1].OriginalTitle + "'");
            throw new Exception("Error reading xml database after " + data.Movie.Count + " records; movie: '" + data.Movie[data.Movie.Count - 1].OriginalTitle + "'; error : " + e.Message);
            //LogMyFilms.DebugException("LoadMyFilmsFromDisk()- error reading '" + catalogfile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read", ex);
            // LogMyFilms.Debug("Commit()- exception while trying to save data in '" + catalogfile + "' - exception: " + saveexeption.Message + ", stacktrace: " + saveexeption.StackTrace);
            //throw e;
          }
          finally
          {
            _dataLock.ExitReadLock();
          }
          watch.Stop();
          LogMyFilms.Debug("LoadMyFilmsFromDisk()- Finished  (" + (watch.ElapsedMilliseconds) + " ms)");
          #endregion

          #region calculate artificial columns like AgeAdded etc.
          // Calculate AgeAdded Fields ...
          watch.Reset(); watch.Start();
          DateTime now = DateTime.Now;
          foreach (AntMovieCatalog.MovieRow movieRow in data.Movie)
          {
            if (movieRow.IsAgeAddedNull()) movieRow.AgeAdded = (!movieRow.IsDateAddedNull()) ? ((int)now.Subtract(movieRow.DateAdded).TotalDays).ToString() : "9999";
          }
          watch.Stop();
          LogMyFilms.Debug("LoadMyFilmsFromDisk() - Calc AgeAdded Finished ... (" + (watch.ElapsedMilliseconds) + " ms)");
          #endregion

          CreateOrUpdateCustomsFieldsProperties();

          #region copy customfields to movie table
          // Get Data from CustomFields ...
          watch.Reset(); watch.Start();
          var commonColumns = data.Movie.Columns.OfType<DataColumn>().Intersect(data.CustomFields.Columns.OfType<DataColumn>(), new DataColumnComparer());
          //foreach (DataColumn commonColumn in commonColumns) LogMyFilms.Debug("LoadMyFilmsFromDisk() - Intersect Column: '" + commonColumn.ColumnName + "'");
          foreach (AntMovieCatalog.MovieRow movieRow in data.Movie)
          {
            //LogMyFilms.Debug("LoadMyFilmsFromDisk() - processing movie ID '" + movieRow.Number + "', otitle = '" + movieRow.OriginalTitle + "'");
            AntMovieCatalog.CustomFieldsRow[] cfCollection = movieRow.GetCustomFieldsRows();
            if (cfCollection.Length == 0) // create CustomFields Element, if not existing ...
            {
              AntMovieCatalog.CustomFieldsRow customFields = data.CustomFields.NewCustomFieldsRow();
              customFields.SetParentRow(movieRow);
              data.CustomFields.AddCustomFieldsRow(customFields); // LogMyFilms.Debug("LoadMyFilmsFromDisk() - created new CustomFieldsRow for movie ID '" + movieRow.Number + "', Title = '" + movieRow.OriginalTitle + "'");
            }
            else // copy data to Movie Row if CustomFields Element exists ...
            {
              AntMovieCatalog.CustomFieldsRow customFields = cfCollection[0];
              foreach (DataColumn dc in commonColumns)
              {
                if (customFields[dc.ColumnName] != DBNull.Value && dc.ColumnName != "Movie_Id") movieRow[dc.ColumnName] = customFields[dc.ColumnName];
              }
            }
          }
          watch.Stop();
          LogMyFilms.Debug("LoadMyFilmsFromDisk() - Copy CustomFields to MovieRow's done ... (" + (watch.ElapsedMilliseconds) + " ms)");
          #endregion

          #region create new enhanced movie rowcollection by joining movie and customfields rows
          //watch.Reset(); watch.Start();
          //tableMoviesExtended = GetEnhancedMovies();
          //int rows = tableMoviesExtended.Rows.Count;
          //watch.Stop();
          //LogMyFilms.Debug("LoadMyFilmsFromDisk() - GetEnhancedMovies() - creating '" + rows + "' rows by joining movie and customfields done ... (" + (watch.ElapsedMilliseconds) + " ms)");
          #endregion

          #region create new joined movieextended datatable
          //// create extended movie table ...
          //watch.Reset(); watch.Start();
          //using (tableMoviesExtended = data.Movie.Clone()) //using (DataTable targetTable = data.Tables["MovieEnhanced"])
          //{
          //  var dt2Query = data.CustomFields.Columns.OfType<DataColumn>().Select(dc => new DataColumn(dc.ColumnName, dc.DataType, dc.Expression, dc.ColumnMapping));
          //  var dt2FilterQuery = from dc in dt2Query.AsEnumerable()
          //                       where tableMoviesExtended.Columns.Contains(dc.ColumnName) == false
          //                       select dc;
          //  tableMoviesExtended.Columns.AddRange(dt2FilterQuery.ToArray());
          //  // populate the rows ...
          //  foreach (AntMovieCatalog.MovieRow movieRow in data.Movie)
          //  {
          //    AntMovieCatalog.MovieRow row = movieRow;
          //    AntMovieCatalog.CustomFieldsRow customFieldsRow = movieRow.GetCustomFieldsRows()[0];
          //    var values = movieRow.ItemArray.Concat(customFieldsRow.ItemArray.Where(r2 => row.ItemArray.Contains(r2) == false));
          //    tableMoviesExtended.Rows.Add((object)values);
          //  }
            
          //  //var rowData = from row1 in data.Movie.AsEnumerable()
          //  //              join row2 in data.CustomFields.AsEnumerable()
          //  //              on row1.Movie_Id equals row2.Movie_Id
          //  //              //into mc
          //  //              //from subcustomfields in mc.DefaultIfEmpty()
          //  //              //where row2.Movie_Id == 0
          //  //              select row1.ItemArray.Concat(row2.ItemArray.Where(r2 => row1.ItemArray.Contains(r2) == false)).ToArray();
          //  //foreach (object[] values in rowData)
          //  //{
          //  //  tableMoviesExtended.Rows.Add(values);
          //  //}
          //  //tableMoviesExtended.TableName = "MovieEnhanced";
          //  //data.Tables.Add(tableMoviesExtended);
          //}
          //foreach (DataColumn column in tableMoviesExtended.Columns)
          //{
          //  LogMyFilms.Debug("LoadMyFilmsFromDisk() - 'MovieEnhanced' Columnname = '" + column.ColumnName + "'");
          //}
          //watch.Stop();
          //LogMyFilms.Debug("LoadMyFilmsFromDisk() - JoinTables with '" + tableMoviesExtended.Rows.Count + "' rows done ... (" + (watch.ElapsedMilliseconds) + " ms)");

          //// Copy CustomDataFields to MyFilms Datafields for internal usage
          //initDataWatch.Reset(); initDataWatch.Start();
          //foreach (AntMovieCatalog.CustomFieldRow customFieldRow in data.CustomField) // add Customfields Rows to Movie table
          //{
          //  DataColumn customfieldcolumn = new DataColumn();
          //  customfieldcolumn.ColumnName = customFieldRow.Tag;
          //  switch (customFieldRow.Type) // ftString, ftInteger, ftReal, ftBoolean, ftDate, ftList, ftText, ftUrl
          //  {
          //    case "ftString":
          //      customfieldcolumn.DataType = typeof(string);
          //      break;
          //    case "ftInteger":
          //      customfieldcolumn.DataType = typeof(int);
          //      break;
          //    case "ftBoolean":
          //      customfieldcolumn.DataType = typeof(bool);
          //      break;
          //    case "ftDate":
          //      customfieldcolumn.DataType = typeof(DateTime);
          //      break;
          //    default:
          //      customfieldcolumn.DataType = typeof(string);
          //      break;
          //  }
          //  customfieldcolumn.DefaultValue = customFieldRow.DefaultValue;
          //  customfieldcolumn.Caption = customFieldRow.Name;

          //  if (!data.Tables["MovieEnhanced"].Columns.Contains(customfieldcolumn.ColumnName))
          //    data.Tables["MovieEnhanced"].Columns.Add(customfieldcolumn);
          //  LogMyFilms.Debug("initData() - DynamicCustomField: '" + customFieldRow.Name + "'");
          //}

          // Copy CustomFields to Movie, if present ...
          //foreach (DataColumn column in data.CustomFields.Columns)
          //{
          //  // add CustomColumns, if not already present
          //  if (!data.Movie.Columns.Contains(column.ColumnName))
          //    data.Movie.Columns.Add(column);
          //}

          //initDataWatch.Reset(); initDataWatch.Start();
          //try
          //{
          //  foreach (AntMovieCatalog.MovieRow movieRow in movies)
          //  {
          //    //DataRow customFieldsRow = movieRow.GetCustomFieldsRows()[0]; // get first one, as it's 1:1 relation ...              
          //    foreach (DataColumn column in data.CustomFields.Columns)
          //    {
          //      if (column.ColumnMapping != MappingType.Hidden)
          //      {
          //        string value = movieRow.GetCustomFieldsRows()[0][column.ColumnName].ToString();
          //        movieRow[column.ColumnName] = movieRow.GetCustomFieldsRows()[0][column.ColumnName];
          //        //movieRow[column.ColumnName] = movieRow.GetCustomFieldsRows()[0][column.ColumnName];
          //        LogMyFilms.Debug("initData() - movieRow '" + column.ColumnName + "' populated with value '" + value + "'");
          //      }

          //    }
          //  }
          //}
          //catch (Exception ex)
          //{
          //  LogMyFilms.Error("initData() - Error !" + ex.Message + ", tackTrace: " + ex.StackTrace);
          //}
          //initDataWatch.Stop();
          //LogMyFilms.Debug("initData() - CopyColumns done ... (" + (initDataWatch.ElapsedMilliseconds) + " ms)");
          #endregion

          #region testing joins and views - experimental ...

          //var query1 = from movie in data.Movie
          //             from customfields in data.CustomFields
          //             where movie.Movie_Id == customfields.Movie_Id
          //             where !movie.IsCheckedNull() && movie.TranslatedTitle != ""
          //             select movie;

          //var query2 = from movie in data.Movie
          //             from customfieds in data.CustomFields
          //             where customfieds.IsWatchedNull()
          //             select movie;

          //var finalQuery = query1.Union(query2);

          //DataTable bindingTable = finalQuery.CopyToDataTable();


          //var query =     from movie in data.Movie.AsEnumerable()
          //                join customfields in data.CustomFields.AsEnumerable()
          //                on movie.Field<int>("Movie_Id") equals
          //                    customfields.Field<int>("Movie_Id")
          //                where movie.TranslatedTitle != "" //&& movie.Field<DateTime>("Date").Month == 8
          //                select new
          //                {
          //                  movie,
          //                  customfields,
          //                  MovieAddedDate =
          //                      movie.Field<DateTime>("Date"),
          //                  CustomField1 = 
          //                      customfields.CustomField1
          //                };

          //query.Select(el =>
          //                {
          //                  DataRow row = data.Movie.NewRow();
          //                  row["CustomField1"] = el.CustomField1;
          //                  return row;
          //                }
          //            ).CopyToDataTable(data.Tables["MoviesExtended"], LoadOption.PreserveChanges);

          //var queryext = from movie in data.Movie
          //               where movie.DateAdded.DayOfYear == DateTime.Now.DayOfYear
          //               where movie.IsTranslatedTitleNull() == false
          //              select new
          //              {
          //                movie,
          //                CustomFields = movie.ContentsRow.GetMovieRows(),
          //                movie.TranslatedTitle
          //              };

          //DataTable dt = LINQToDataTable(queryext);

          //foreach (var result in queryext)
          //{
          //  bindingTable.Rows.Add(new object[] { result, result.TranslatedTitle });
          //}


          //// create extended movie table ...
          //watchReadMovies.Reset(); watchReadMovies.Start();
          //DataTable targetTable = data.Movie.Clone();
          //var dt2Columns = data.CustomFields.Columns.OfType<DataColumn>().Select(dc =>
          //    new DataColumn(dc.ColumnName, dc.DataType, dc.Expression, dc.ColumnMapping));
          //targetTable.Columns.AddRange(dt2Columns.ToArray());
          //var rowData =
          //    from row1 in data.Movie.AsEnumerable()
          //    join row2 in data.CustomFields.AsEnumerable()
          //        on row1.Field<int>("Movie_ID") equals row2.Field<int>("Movie_ID")
          //    select row1.ItemArray.Concat(row2.ItemArray).ToArray();
          //foreach (object[] values in rowData)
          //  targetTable.Rows.Add(values);
          //DataRow[] moviesEnhanced = targetTable.Select(MyFilms.conf.StrTitle1 + " not like ''");
          //watchReadMovies.Stop();
          //LogMyFilms.Debug("ReadDataMovies() - JoinTables (" + (watchReadMovies.ElapsedMilliseconds) + " ms)");

          //// create extended movie table ...
          //watchReadMovies.Reset(); watchReadMovies.Start();
          //DataTable targetTable = DataTableJoiner(data.Movie, data.CustomFields);
          //DataRow[] moviesEnhanced = targetTable.Select(MyFilms.conf.StrTitle1 + " not like ''");
          //watchReadMovies.Stop();
          //LogMyFilms.Debug("ReadDataMovies() - JoinTables (" + (watchReadMovies.ElapsedMilliseconds) + " ms) - Records: '" + moviesEnhanced.Length + "'");
          //return moviesEnhanced;


          ////Create new joined table with customFields ...
          //watchReadMovies.Reset(); watchReadMovies.Start();
          //DataTable dt = new DataTable();
          //dt = data.Movie.Clone();
          //DataRow[] drResults = data.CustomFields.Select();
          //foreach (DataRow dr in drResults)
          //{
          //  object[] row = dr.ItemArray;
          //  if (row.ToString() != "Movie_ID")
          //    dt.Rows.Add(row);
          //}
          //DataRow[] new_movies = dt.Select(MyFilms.conf.StrTitle1 + " not like ''");
          //watchReadMovies.Stop();
          //LogMyFilms.Debug("ReadDataMovies() - JoinTables Variant 1 (" + (watchReadMovies.ElapsedMilliseconds) + " ms)");

          //watchReadMovies.Reset(); watchReadMovies.Start();
          //DataTable targetTable = data.Movie.Clone();
          //var dt2Columns = data.CustomFields.Columns.OfType<DataColumn>().Select(dc =>
          //    new DataColumn(dc.ColumnName, dc.DataType, dc.Expression, dc.ColumnMapping));
          //targetTable.Columns.AddRange(dt2Columns.ToArray());
          //var rowData =
          //    from row1 in data.Movie.AsEnumerable()
          //    join row2 in data.CustomFields.AsEnumerable()
          //        on row1.Field<int>("Movie_ID") equals row2.Field<int>("Movie_ID")
          //    select row1.ItemArray.Concat(row2.ItemArray).ToArray();
          //foreach (object[] values in rowData)
          //  targetTable.Rows.Add(values);
          //DataRow[] newmovies = targetTable.Select(MyFilms.conf.StrTitle1 + " not like ''");
          //watchReadMovies.Stop();
          //LogMyFilms.Debug("ReadDataMovies() - JoinTables Variant 2 (" + (watchReadMovies.ElapsedMilliseconds) + " ms)");


          //var movies_enhanced =
          //    from o in data.Movie
          //    where   o.Year == "1998"
          //    orderby o.DateAdded descending
          //    select new 
          //    { 
          //      o.Movie_Id, 
          //      o.OriginalTitle,
          //      CustomField_1 = o.GetCustomFieldsRows()[0].CustomField1
          //      // ...
          //    };

          //foreach (AntMovieCatalog.MovieRow movieRow in movies)
          //  DataRow customFieldsRow = movieRow.GetCustomFieldsRows()[0]; // get first one, is it's 1:1 relation ...

          //SELECT Productos.idProducto, Productos.Nombre, Precios.Precio, Tiendas.idTienda, Zonas.Zona,Productos.idZona FROM
          //Productos INNER JOIN Precios ON Productos.idProducto = Precios.idProducto

          //var moviesquery = from movie in data.Movie
          //              join extendedfields in data.CustomFields on movie.Movie_Id equals extendedfields.Movie_Id into gj
          //              from subpet in gj.DefaultIfEmpty()
          //              select new { movie.OriginalTitle, PetName = (subpet == null ? String.Empty : subpet.CustomField1) };

          //var q = from c in data.Movie
          //  join o in data.CustomFields on c.Movie_Id equals o.Movie_Id into g
          //  select new {movie = c.OriginalTitle, Extendedfields = g};

          //var moviesextended = from ant in data.Movie
          //                     join custom in data.CustomFields on ant.Movie_Id equals custom.Movie_Id into g
          //                     select new
          //                     {
          //                       ant,
          //                       movie = ant.OriginalTitle,
          //                       Extendedfields = g
          //                     };

          //var queryProducer =
          //    from movie in data.Movie
          //    group movie by movie.Producer into groupProducer
          //    orderby groupProducer.Key
          //    select groupProducer;

          //IGrouping<System.Reflection.MemberTypes, System.Reflection.MemberInfo> group =
          //                typeof(String).GetMembers().
          //                GroupBy(member => member.MemberType).
          //                First();



          //DataView dataView = (from movie in
          //                       data.Movie.AsEnumerable()
          //                     where movie.Field<int?>("YearEstablished") < 1960
          //                     orderby movie.Field<string>("Country")
          //                     select movie).AsDataView();

          //DataRowView[] drv = dataView.FindRows("Canada");

          //var query8 = from movie in data.Movie.AsEnumerable()
          //             join customfields in data.CustomField.AsEnumerable()
          //             on movie.Field<string>("Country") equals
          //             customfields.Field<string>("Name")
          //             select new
          //             {
          //               ParkName = movie.Field<string>("Name"),
          //               Country = customfields.Field<string>("Name"),
          //               Continent = customfields.
          //               GetParentRow(data.CustomField.ParentRelations[0]
          //               )
          //               .Field<string>("Name")
          //             };

          //var query = from t1 in data.Movie.AsEnumerable()
          //            join t2 in data.CustomField.AsEnumerable()
          //              // <int> == <int?> für Null Vergleich
          //            on t1.Field<int>("id") equals t2.Field<int?>("idTabelle1")
          //            select new
          //            {
          //              t1id = t1.Field<int>("id"),
          //              t2id = t2.Field<int>("id"),
          //              t1daten = t1.Field<string>("Daten"),
          //              t2daten = t2.Field<string>("Daten"),
          //            };
          //foreach (var row in query)
          //{
          //  Console.WriteLine("{0}\t{1}\t{2:d}\t{3}",
          //  row.t1id, row.t2id, row.t1daten, row.t2daten);
          //}

          //var abfrage = from emp in data.Movie
          //              join con in data.CustomField on emp.Movie_Id
          //              equals con.CustomField_Id into ec
          //              from subEmp in ec.DefaultIfEmpty()
          //              select new
          //              {
          //                emp.Movie_Id,
          //                FirstName =
          //                  (subEmp == null ? String.Empty : subEmp.Name)
          //              };

          //var orders = data.Tables["SalesOrderHeader"].AsEnumerable();
          //var details = data.Tables["SalesOrderDetail"].AsEnumerable();

          //var query0 =
          //    from order in orders
          //    join detail in details
          //    on order.Field<int>("SalesOrderID")
          //    equals detail.Field<int>("SalesOrderID") into ords
          //    select new
          //    {
          //      CustomerID =
          //          order.Field<int>("SalesOrderID"),
          //      ords = ords.Count()
          //    };

          //foreach (var order in query)
          //{
          //  // Console.WriteLine("CustomerID: {0}  Orders Count: {1}", order.CustomerID, order.ords);
          //}



          //DataTable dtMovies = data.Movie;
          //DataTable dtExtendedFields = data.Tables["ExtendedField"];
          //var querynew =
          //    from movie in dtMovies.AsEnumerable()
          //    join extendedfields in dtExtendedFields.AsEnumerable()
          //    on movie.Field<Int32>("MovieID") equals
          //    extendedfields.Field<Int32>("MovieID")
          //    select new
          //    {
          //      ContactID = movie.Field<Int32>("ContactID"),
          //      SalesOrderID = extendedfields.Field<Int32>("SalesOrderID"),
          //      FirstName = movie.Field<string>("FirstName"),
          //      Lastname = movie.Field<string>("Lastname"),
          //      TotalDue = extendedfields.Field<decimal>("TotalDue")
          //    };


          //foreach (var contact_order in querynew)
          //{
          //  Console.WriteLine("ContactID: {0} "
          //                  + "SalesOrderID: {1} "
          //                  + "FirstName: {2} "
          //                  + "Lastname: {3} "
          //                  + "TotalDue: {4}",
          //      contact_order.ContactID,
          //      contact_order.SalesOrderID,
          //      contact_order.FirstName,
          //      contact_order.Lastname,
          //      contact_order.TotalDue);
          //}
          #endregion

          return success;
        }

        private static ArrayList GetMoviesGlobal(string Expression, string Sort, bool traktOnly)
        {
          ArrayList movies = new ArrayList();
          movies.Clear();
          AntMovieCatalog dataExport = new AntMovieCatalog();
          int moviecount = 0; // total count for added movies

          using (XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
          {
            int MesFilms_nb_config = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
            ArrayList configs = new ArrayList();
            for (int i = 0; i < MesFilms_nb_config; i++)
              configs.Add(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));

            foreach (string config in configs)
            {
              string StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
              string StrFileType = XmlConfig.ReadXmlConfig("MyFilms", config, "CatalogType", "0");
              bool AllowTraktSync = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
              bool AllowRecentlyAddedAPI = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowRecentAddedAPI", false);

              if (AllowTraktSync || AllowRecentlyAddedAPI)
                LogMyFilms.Debug("GetMovies() - Trakt = '" + AllowTraktSync + "', RecentMedia = '" + AllowRecentlyAddedAPI + "', CatalogType = '" + StrFileType + "', Config = '" + config + "', Catalogfile = '" + StrFileXml + "'");
              else
                LogMyFilms.Debug("GetMovies() - Trakt = '" + AllowTraktSync + "', RecentMedia = '" + AllowRecentlyAddedAPI + "', CatalogType = '" + StrFileType + "', Config = '" + config + "'");

              if (File.Exists(StrFileXml) && (AllowTraktSync || (!traktOnly && AllowRecentlyAddedAPI)))
              {
                MyFilmsGUI.Configuration tmpconf = new MyFilmsGUI.Configuration(config, false, true, null);
                _dataLock.EnterReadLock();
                try
                {
                  dataExport.ReadXml(tmpconf.StrFileXml);
                }
                catch (Exception e)
                {
                  LogMyFilms.Error(": Error reading xml database after " + dataExport.Movie.Count.ToString() + " records; error : " + e.Message + ", " + e.StackTrace);
                  // throw e; // we should NOT throw the exception, otherwilse MP crashes due to unhandled exception
                }
                finally
                {
                  _dataLock.ExitReadLock();
                }

                try
                {
                  string sqlExpression = (!string.IsNullOrEmpty(Expression)) ? Expression : tmpconf.StrDfltSelect + tmpconf.StrTitle1 + " not like ''";
                  string sqlSort = (!string.IsNullOrEmpty(Sort)) ? Sort : "OriginalTitle" + " " + "ASC";
                  DataRow[] results = dataExport.Tables["Movie"].Select(sqlExpression, sqlSort);
                  // if (results.Length == 0) continue;

                  foreach (DataRow sr in results)
                  {
                    try
                    {
                      MFMovie movie = new MFMovie();
                      movie.Config = config; // MF config context
                      GetMovieDetails(sr, tmpconf, ref movie);
                      if (!traktOnly && tmpconf.AllowRecentlyAddedAPI)
                      {
                        movie.MFconfig = tmpconf;
                        movie.MovieRow = sr;
                        // GetMovieArtworkDetails(sr, tmpconf, ref movie); // moved to LM API as it's a lot of I/O ...
                      }
                      movies.Add(movie);
                      moviecount += 1;
                    }
                    catch (Exception mex)
                    {
                      LogMyFilms.Error("MyFilms videodatabase: add movie exception - err:{0} stack:{1}", mex.Message, mex.StackTrace);
                    }
                  }
                }
                catch (Exception ex)
                {
                  LogMyFilms.Error("MyFilms videodatabase exception err:{0} stack:{1}", ex.Message, ex.StackTrace);
                }
              }
            }
          }
          return movies;
        }

        private static void GetMovieDetails(DataRow row, MyFilmsGUI.Configuration tmpconf, ref MFMovie movie)
        {
          //-----------------------------------------------------------------------------------------------------------------------
          //    Load Movie Details Info
          //-----------------------------------------------------------------------------------------------------------------------

          movie.ReadOnly = tmpconf.ReadOnly; // is true for readonly ctalog types
          if (!string.IsNullOrEmpty(row["Number"].ToString()))
            movie.ID = Int32.Parse(row["Number"].ToString());
          else movie.ID = 0;

          int year = 1900;
          Int32.TryParse(row["Year"].ToString(), out year);
          movie.Year = year;
          movie.Category = row["Category"].ToString();

          int length = 0;
          Int32.TryParse(row["Length"].ToString(), out length);
          movie.Length = length;

          movie.Title = row["OriginalTitle"].ToString();
          if (row[tmpconf.StrTitle1].ToString().Length > 0)
          {
            int TitlePos = (tmpconf.StrTitleSelect.Length > 0) ? tmpconf.StrTitleSelect.Length + 1 : 0; //only display rest of title after selected part common to group
            movie.Title = row[tmpconf.StrTitle1].ToString().Substring(TitlePos);
          }
          if (Helper.FieldIsSet(tmpconf.StrTitle2))
            movie.TranslatedTitle = row[tmpconf.StrTitle2].ToString();

          bool played = false;
          if (tmpconf.StrEnhancedWatchedStatusHandling)
          {
            if (MyFilms.EnhancedWatched(row[tmpconf.StrWatchedField].ToString(), tmpconf.StrUserProfileName) == true)
              played = true;
          }
          else
          {
            if (tmpconf.GlobalUnwatchedOnlyValue != null && tmpconf.StrWatchedField.Length > 0)
              if (row[tmpconf.StrWatchedField].ToString().ToLower() != tmpconf.GlobalUnwatchedOnlyValue.ToLower()) // changed to take setup config into consideration
                played = true;
          }
          movie.Watched = played;
          movie.WatchedCount = -1; // check against it, if value returns...

          float rating = 0;
          bool success = float.TryParse(row["Rating"].ToString(), out rating);
          if (!success) rating = 0;
          movie.Rating = rating; // movie.Rating = (float)Double.Parse(sr["Rating"].ToString());

          string mediapath = string.Empty;
          if (Helper.FieldIsSet(tmpconf.StrStorage))
          {
            mediapath = row[tmpconf.StrStorage].ToString();
            if (mediapath.Contains(";")) // take the first source file
              mediapath = mediapath.Substring(0, mediapath.IndexOf(";"));
            movie.File = mediapath;
          }
          if (string.IsNullOrEmpty(mediapath)) // e.g. offline media files
            movie.File = movie.Title + " {offline} [" + movie.ID + "]";

          string path = "";
          if (!string.IsNullOrEmpty(mediapath))
          {
            try
            {
              path = System.IO.Path.GetDirectoryName(mediapath);
            }
            catch (Exception)
            {
              movie.Path = "{search}";
            }
            movie.Path = path;
          }
          else
          {
            movie.Path = "{offline}";
          }

          string IMDB = "";
          if (!string.IsNullOrEmpty(row["IMDB_Id"].ToString()))
            IMDB = row["IMDB_Id"].ToString();
          else if (!string.IsNullOrEmpty(row["URL"].ToString()))
          {
            string URLstring = row["URL"].ToString();
            Regex CutText = new Regex("" + @"tt\d{7}" + "");
            Match m = CutText.Match(URLstring);
            if (m.Success)
              IMDB = m.Value;
          }
          movie.IMDBNumber = IMDB;

          if (!string.IsNullOrEmpty(row["TMDB_Id"].ToString()))
            movie.TMDBNumber = row["TMDB_Id"].ToString();

          movie.DateAdded = row["Date"].ToString();
          DateTime wdate = new DateTime(1900, 01, 01);
          try
          {
            wdate = Convert.ToDateTime(row["Date"]);
            movie.DateTime = wdate;
          }
          catch { }
        }

        private static void GetMovieArtworkDetails(DataRow row, MyFilmsGUI.Configuration tempconf, ref MFMovie movie)
        {
          //-----------------------------------------------------------------------------------------------------------------------
          //    Load Artwork Info
          //-----------------------------------------------------------------------------------------------------------------------
          string file;
          if (row["Picture"].ToString().Length > 0)
          {
            if ((row["Picture"].ToString().IndexOf(":\\") == -1) && (row["Picture"].ToString().Substring(0, 2) != "\\\\"))
              file = tempconf.StrPathImg + "\\" + row["Picture"].ToString();
            else
              file = row["Picture"].ToString();
          }
          else
            file = string.Empty;
          if (!File.Exists(file) || string.IsNullOrEmpty(file))
            file = tempconf.DefaultCover;
          movie.Picture = file;
          movie.Fanart = MyFilmsDetail.Search_Fanart(MyFilmsDetail.GetSearchTitles(row, "", tempconf).FanartTitle, false, "file", false, file, string.Empty, tempconf)[0];
        }

        private static ArrayList Base_Search_String(string champselect, string[] listSeparator, string[] roleSeparator)
        {
          Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
          System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
          foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
          {
            Regex oRegexReplace = new Regex("[,;]");
            champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, string.Empty));
          }
          ArrayList wtab = new ArrayList();

          int wi = 0;
          string[] Sep = listSeparator;
          string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
          string wzone = string.Empty;
          int wzoneIndexPosition = 0;
          for (wi = 0; wi < arSplit.Length; wi++)
          {
            if (arSplit[wi].Length > 0)
            {
              // wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim()); // Replaced for performancereasons - HTML cleanup was not necessary !
              wzone = arSplit[wi].Replace("  ", " ").Trim();
              for (int i = 0; i <= 4; i++)
              {
                if (roleSeparator[i].Length > 0)
                {
                  wzoneIndexPosition = wzone.IndexOf(roleSeparator[i]);
                  if (wzoneIndexPosition == wzone.Length - 1)
                  {
                    wzone = string.Empty;
                    break;
                  }
                  if (wzoneIndexPosition > 1 && wzoneIndexPosition < wzone.Length)
                  {
                    wzone = wzone.Substring(0, wzoneIndexPosition).Trim();
                  }
                }
              }
              if (wzone.Length > 0)
                wtab.Add(wzone);
            }
          }
          return wtab;
        }

        #endregion

        #region accesseurs
        public static DataRow[] FilmsSelected
        {
          get { return movies; }
        }
        #endregion

        #region Public static methods ...

        public static DataRow[] ReadDataMovies(string StrDfltSelect, string StrSelect, string StrSort, string StrSortSens)
        {
            return ReadDataMovies(StrDfltSelect, StrSelect, StrSort, StrSortSens, false);
        }
        public static DataRow[] ReadDataMovies(string StrDfltSelect, string StrSelect, string StrSort, string StrSortSens, bool all)
        {
          LogMyFilms.Debug("ReadDataMovies() - Starting ... (StrDfltSelect = '" + StrDfltSelect + "', StrSelect = '" + StrSelect + "', StrSort = '" + StrSort + "', StrSortSens = '" + StrSortSens + "', RESULTING DS SELECT = '" + StrDfltSelect + StrSelect + ", " + StrSort + " " + StrSortSens + "')");
          Stopwatch watchReadMovies = new Stopwatch(); watchReadMovies.Reset(); watchReadMovies.Start();
          bool iscached = true;
          if (StrSelect.Length == 0) StrSelect = MyFilms.conf.StrTitle1 + " not like ''";

          if (data == null)
          {
            iscached = false;
            initData();
          }
          movies = data.Movie.Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens); //movies = data.Tables["MovieEnhanced"].Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens);
          if (movies.Length == 0 && all)
          {
            StrSelect = MyFilms.conf.StrTitle1 + " not like ''";
            LogMyFilms.Debug("ReadDataMovies() - Switching to full list ...");
            movies = data.Movie.Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens);
          }
          watchReadMovies.Stop();
          LogMyFilms.Debug("ReadDataMovies() - Finished ... (cached = '" + iscached + "') (" + (watchReadMovies.ElapsedMilliseconds) + " ms)");
          return movies;
        }

        public static void LoadMyFilms(string StrFileXml)
        {
          if (!File.Exists(StrFileXml)) throw new Exception(string.Format("The file {0} does not exist !.", StrFileXml));
          Stopwatch watchReadMovies = new Stopwatch(); watchReadMovies.Reset(); watchReadMovies.Start();
          bool success = LoadMyFilmsFromDisk(StrFileXml);
          watchReadMovies.Stop();
          LogMyFilms.Debug("LoadMyFilms() - Finished ... (success = '" + success + "') (" + (watchReadMovies.ElapsedMilliseconds) + " ms)");
        }

        public static void UnloadMyFilms()
        {
          if (data != null) data.Dispose();
        }

        public static bool SaveMyFilms(string catalogfile, int timeout)
        {
          bool success = false;

          if (data == null) return false;
          if (timeout == 0) timeout = 10000; // default is 10 secs
          CopyMovieExtendedDataToCustomFields(false);
          LogMyFilms.Debug("TryEnterWriteLock(" + timeout + ") - CurrentReadCount = '" + _dataLock.CurrentReadCount + "', RecursiveReadCount = '" + _dataLock.RecursiveReadCount + "', RecursiveUpgradeCount = '" + _dataLock.RecursiveUpgradeCount + "', RecursiveWriteCount = '" + _dataLock.RecursiveWriteCount + "'"); 
          if (_dataLock.TryEnterWriteLock(timeout))
          {
            LogMyFilms.Debug("TryEnterWriteLock successful! - CurrentReadCount = '" + _dataLock.CurrentReadCount + "', RecursiveReadCount = '" + _dataLock.RecursiveReadCount + "', RecursiveUpgradeCount = '" + _dataLock.RecursiveUpgradeCount + "', RecursiveWriteCount = '" + _dataLock.RecursiveWriteCount + "'");
            try
            {
              success = SaveMyFilmsToDisk(catalogfile);
            }
            catch (Exception ex)
            {
              success = false;
              LogMyFilms.DebugException("SaveMyFilms() - error saving data - exception: '" + ex.Message + "'", ex);
              // throw;
            }
            finally
            {
              _dataLock.ExitWriteLock();
            }

            if (success)
              return true; // write successful!
            else
              return false; // write unsuccessful!
          }
          else
          {
            LogMyFilms.Info("SaveMyFilms() - Movie Database could not get slim writelock for '" + timeout + "' ms - returning 'false'");
            return false;
          }
        }

        public static void CancelMyFilms()
        {
          LogMyFilms.Debug("CancelMyFilms() - disposing data ...");
          if (data != null)
            data.Clear();
        }

        public static void GetMovies(ref ArrayList movies)
        {
          movies = GetMoviesGlobal("", "", true);
          LogMyFilms.Debug("GetMovies() - movies matched: '" + movies.Count + "'");
        }

        #region API for Basic Home Editors
        /// <summary>
        /// returns an array of KeyValue ViewLists and name for each config
        /// .Name = config name
        /// .ViewList = List of KeyValues with view name and pretty name
        /// </summary>        
        public static ArrayList GetConfigViewLists()
        {
          ArrayList configViewLists = new ArrayList();
          List<KeyValuePair<string, string>> viewList = null;

          using (XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
          {
            int MesFilms_nb_config = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
            ArrayList configs = new ArrayList();
            for (int i = 0; i < MesFilms_nb_config; i++) configs.Add(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));

            foreach (string config in configs)
            {
              viewList = new List<KeyValuePair<string, string>>();
              viewList.Clear();
              MFConfig configViewList = new MFConfig();
              string[] StrViewItem = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty }, StrViewText = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty }, StrViewValue = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

              string Catalog = XmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
              bool TraktEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
              bool RecentAddedAPIEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowRecentAddedAPI", false);
              for (int i = 1; i < 6; i++)
              {
                StrViewItem[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", config, string.Format("AntViewItem{0}", i), string.Empty);
                StrViewText[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", config, string.Format("AntViewText{0}", i), string.Empty);
                StrViewValue[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", config, string.Format("AntViewValue{0}", i), string.Empty);
              }

              if (System.IO.File.Exists(Catalog))
              {
                //viewList.Add(new KeyValuePair<string, string>("all", GUILocalizeStrings.Get(342)));
                viewList.Add(new KeyValuePair<string, string>("Year", GUILocalizeStrings.Get(345)));
                viewList.Add(new KeyValuePair<string, string>("Category", GUILocalizeStrings.Get(10798664)));
                viewList.Add(new KeyValuePair<string, string>("Country", GUILocalizeStrings.Get(200026)));
                viewList.Add(new KeyValuePair<string, string>("Actors", GUILocalizeStrings.Get(10798667)));
                for (int i = 0; i < 5; i++) // userdefined views
                {
                  if (!string.IsNullOrEmpty(StrViewItem[i]) && StrViewItem[i] != "(none)")
                  {
                    string viewName = "", viewDisplayName = "";
                    // viewName = (string.Format("view{0}", i));
                    viewName = StrViewItem[i];
                    if (!string.IsNullOrEmpty(StrViewText[i]))
                      viewDisplayName = StrViewText[i];
                    else
                      viewDisplayName = StrViewItem[i];
                    viewList.Add(new KeyValuePair<string, string>(viewName, viewDisplayName));
                  }
                }
              }
              configViewList.Name = config;
              configViewList.ViewList = viewList;
              configViewLists.Add(configViewList);
            }
          }
          return configViewLists;
        }

        /// <summary>
        /// returns a string List of available values for a given config & view
        /// use GetConfigViewLists() to get valid values for config and view
        /// returns <null> if no values can be evaluated - user should still be able to manually set a value for startparam
        /// </summary>        
        public static List<string> GetViewListValues(string config, string view)
        {
          List<string> values = new List<string>();
          ArrayList movies = new ArrayList();
          movies.Clear();
          AntMovieCatalog dataExport = new AntMovieCatalog();
          if (string.IsNullOrEmpty(config) || string.IsNullOrEmpty(view)) 
            return null;

          using (XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
          {
            string Catalog = XmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
            string[] listSeparator = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            string[] roleSeparator = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            bool TraktEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
            bool RecentAddedAPIEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowRecentAddedAPI", false);
            string StrDfltSelect = XmlConfig.ReadXmlConfig("MyFilms", config, "StrDfltSelect", string.Empty);
            int j = 0;
            for (int i = 1; i <= 5; i++)
            {
              if (XmlConfig.ReadXmlConfig("MyFilms", config, "ListSeparator" + i, string.Empty).Length > 0)
              {
                listSeparator[j] = XmlConfig.ReadXmlConfig("MyFilms", config, "ListSeparator" + i, string.Empty);
                j++;
              }
            }
            j = 0;
            for (int i = 1; i <= 5; i++)
            {
              if (XmlConfig.ReadXmlConfig("MyFilms", config, "RoleSeparator" + i, string.Empty).Length > 0)
              {
                roleSeparator[j] = XmlConfig.ReadXmlConfig("MyFilms", config, "RoleSeparator" + i, string.Empty);
                j++;
              }
            }

            if (System.IO.File.Exists(Catalog))
            {
              string champselect = "";
              string wchampselect = "";
              ArrayList w_tableau = new ArrayList();
              int Wnb_enr = 0;

              _dataLock.EnterReadLock();
              try
              {
                dataExport.ReadXml(Catalog);
              }
              catch (Exception e)
              {
                LogMyFilms.Error(": Error reading xml database after " + dataExport.Movie.Count.ToString() + " records; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
                throw e;
              }
              finally
              {
                _dataLock.ExitReadLock();
              }
              DataRow[] results = dataExport.Tables["Movie"].Select(StrDfltSelect, view.ToUpper() + " ASC");
              if (results.Length == 0) return null;

              foreach (DataRow enr in results)
              {
                try
                {
                  bool isdate = false;
                  if (view == "Date" || view == "DateAdded")
                    isdate = true;

                  if (isdate)
                    champselect = string.Format("{0:yyyy/MM/dd}", enr["DateAdded"]);
                  else
                    champselect = enr[view].ToString().Trim();
                  ArrayList wtab = Base_Search_String(champselect, listSeparator, roleSeparator);
                  for (int wi = 0; wi < wtab.Count; wi++)
                  {
                    w_tableau.Add(wtab[wi].ToString().Trim());
                  }
                }
                catch (Exception)
                {
                }
              }
              w_tableau.Sort(0, w_tableau.Count, null);

              for (int wi = 0; wi != w_tableau.Count; wi++)
              {
                champselect = w_tableau[wi].ToString();
                if (string.Compare(champselect, wchampselect, true) == 0) // Are the strings equal? Then add count!
                {
                  Wnb_enr++; // count items of distinct property
                }
                else
                {
                  if ((Wnb_enr > 0) && (wchampselect.Length > 0))
                  {
                    values.Add(wchampselect);
                  }
                  Wnb_enr = 1;
                  wchampselect = champselect;
                }
              }

              if ((Wnb_enr > 0) && (wchampselect.Length > 0))
              {
                values.Add(wchampselect);
                Wnb_enr = 0;
              }
            }
          }
          return values;
        }
        #endregion

        #region API for Most Recent movies

        /// <summary>
        /// returns the 3 most recent movies based on conditions
        /// </summary>        
        public static List<MFMovie> GetMostRecent(MostRecentType type)
        {
          return GetMostRecent(type, 30, 3);
        }

        /// <summary>
        /// returns the most recent movies based on conditions
        /// </summary>
        /// <param name="type">most recent type</param>
        /// <param name="days">number of days to look back in database</param>
        /// <param name="limit">number of results to return</param>        
        public static List<MFMovie> GetMostRecent(MostRecentType type, int days, int limit)
        {
          return GetMostRecent(type, days, limit, false);
        }

        /// <summary>
        /// returns the most recent movies based on conditions
        /// </summary>
        /// <param name="type">most recent type</param>
        /// <param name="days">number of days to look back in database</param>
        /// <param name="limit">number of results to return</param>
        /// <param name="unwatched">only get unwatched episodes (only used with recent added type)</param>
        public static List<MFMovie> GetMostRecent(MostRecentType type, int days, int limit, bool unwatchedOnly)
        {
          string enumtype = Enum.GetName(typeof(MostRecentType), type);
          LogMyFilms.Debug("GetMostRecent() - Called with type = '" + enumtype + "', days = '" + days + "', limit = '" + limit + "', unwatchedonly = '" + unwatchedOnly + "'");
          List<MFMovie> movielist = new List<MFMovie>();
          ArrayList allmovies = new ArrayList();

          // Create Time Span to lookup most recents
          DateTime dateCompare = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
          dateCompare = dateCompare.Subtract(new TimeSpan(days, 0, 0, 0, 0));
          string date = dateCompare.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");

          // get all movies
          allmovies = GetMoviesGlobal("", "", false);
          movielist = (from MFMovie movie in allmovies select movie).ToList();
          
          switch (type)
          {
            case MostRecentType.Added:
              // string sqlExpression = "Date" + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(sLabel).ToShortDateString()) + "*'";
              
              // only within the specified timeframe:
              if (unwatchedOnly)
                movielist = movielist.Where(m => m.Watched == false).ToList();
              movielist = movielist.Where(m => m.DateTime > dateCompare).ToList();
              break;

            case MostRecentType.Watched:
              // get the movies that we have watched
              movielist = movielist.Where(m => m.Watched == true).ToList();
              break;
          }

          // sort descending by dateadded
          movielist = movielist.OrderByDescending(x => x.DateTime).ToList();
          //movielist.Sort((x, y) => -x.DateTime.CompareTo(y.DateTime)); // minus is for descending order... saves movielist.Reverse();
          LogMyFilms.Debug("GetMostRecent() - retrieving (nonlimited) movies: '" + movielist.Count + "'");

          // now apply the result count limit
          movielist = movielist.Take(limit).ToList();

          // get artwork
          List<MFMovie> movielistwithartwork = new List<MFMovie>();
          foreach (MFMovie movie in movielist)
          {
            MFMovie tmpmovie = new MFMovie();
            tmpmovie = movie;
            GetMovieArtworkDetails(movie.MovieRow, movie.MFconfig, ref tmpmovie);
            movielistwithartwork.Add(tmpmovie);
          }
          // foreach (MFMovie movie in movielistwithartwork) LogMyFilms.Debug("GetMostRecent() - Returning (limited): config = '" + movie.Config + "', title = '" + movie.Title + "', watched = '" + movie.Watched + "', added = '" + movie.DateAdded + "', datetime = '" + movie.DateTime.ToShortDateString() + "', length = '" + movie.Length.ToString() + "', Category = '" + movie.Category + "', cover = '" + movie.Picture + "', fanart = '" + movie.Fanart + "'");
          LogMyFilms.Debug("GetMostRecent() - Returning '" + movielistwithartwork.Count + "' movies.");
          return movielistwithartwork;
        }
        #endregion

        public static string Translate_Column(string Column)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(Column)) return string.Empty;

            switch (Column)
            {
              case "Number":
              case "number":
                return GUILocalizeStrings.Get(10798650);
              case "Checked":
              case "checked":
                return GUILocalizeStrings.Get(10798651);
              case "MediaLabel":
              case "medialabel":
                return GUILocalizeStrings.Get(10798652);
              case "MediaType":
              case "mediatype":
                return GUILocalizeStrings.Get(10798653);
              case "Source":
              case "source":
                return GUILocalizeStrings.Get(10798654);
              case "Date":
              case "date":
                return GUILocalizeStrings.Get(10798655);
              case "Borrower":
              case "borrower":
                return GUILocalizeStrings.Get(10798656);
              case "Rating":
              case "rating":
                return GUILocalizeStrings.Get(10798657);
              case "OriginalTitle":
              case "originaltitle":
                return GUILocalizeStrings.Get(10798658);
              case "TranslatedTitle":
              case "translatedtitle":
                return GUILocalizeStrings.Get(10798659);
              case "FormattedTitle":
              case "formattedtitle":
                return GUILocalizeStrings.Get(10798660);
              case "Director":
              case "director":
                return GUILocalizeStrings.Get(10798661);
              case "Producer":
              case "producer":
                return GUILocalizeStrings.Get(10798662);
              case "Country":
              case "country":
                return GUILocalizeStrings.Get(10798663);
              case "Category":
              case "category":
                return GUILocalizeStrings.Get(10798664);
              case "Year":
              case "year":
                return GUILocalizeStrings.Get(10798665);
              case "Length":
              case "length":
                return GUILocalizeStrings.Get(10798666);
              case "Actors":
              case "actors":
                return GUILocalizeStrings.Get(10798667);
              case "URL":
              case "url":
                return GUILocalizeStrings.Get(10798668);
              case "Description":
              case "description":
                return GUILocalizeStrings.Get(10798669);
              case "Comments":
              case "comments":
                return GUILocalizeStrings.Get(10798670);
              case "VideoFormat":
              case "videoformat":
                return GUILocalizeStrings.Get(10798671);
              case "VideoBitrate":
              case "videobitrate":
                return GUILocalizeStrings.Get(10798672);
              case "AudioFormat":
              case "audioformat":
                return GUILocalizeStrings.Get(10798673);
              case "AudioBitrate":
              case "audiobitrate":
                return GUILocalizeStrings.Get(10798674);
              case "Resolution":
              case "resolution":
                return GUILocalizeStrings.Get(10798675);
              case "Framerate":
              case "framerate":
                return GUILocalizeStrings.Get(10798676);
              case "Languages":
              case "languages":
                return GUILocalizeStrings.Get(10798677);
              case "Subtitles":
              case "subtitles":
                return GUILocalizeStrings.Get(10798678);
              case "DateAdded":
              case "dateadded":
                return GUILocalizeStrings.Get(10798679);
              case "Size":
              case "size":
                return GUILocalizeStrings.Get(10798680);
              case "Disks":
              case "disks":
                return GUILocalizeStrings.Get(10798681);
              case "Picture":
              case "picture":
                return GUILocalizeStrings.Get(10798682);
              case "Certification":
              case "certification":
                return GUILocalizeStrings.Get(10798683);
              case "Writer":
              case "writer":
                return GUILocalizeStrings.Get(10798684);
              case "Watched":
              case "watched":
                return GUILocalizeStrings.Get(10798685);
              case "WatchedDate":
              case "watcheddate":
                return GUILocalizeStrings.Get(10798686);
              case "IMDB_Id":
              case "imdb_id":
                return GUILocalizeStrings.Get(10798687);
              case "TMDB_Id":
              case "tmdb_id":
                return GUILocalizeStrings.Get(10798688);
              case "SourceTrailer":
              case "sourcetrailer":
                return GUILocalizeStrings.Get(10798940);
              case "TagLine":
              case "tagline":
                return GUILocalizeStrings.Get(10798941);
              case "Tags":
              case "tags":
                return GUILocalizeStrings.Get(10798942);
              case "Aspectratio":
              case "aspectratio":
                return GUILocalizeStrings.Get(10798943);
              case "RatingUser":
              case "ratinguser":
                return GUILocalizeStrings.Get(10798944);
              case "Fanart":
              case "fanart":
                return GUILocalizeStrings.Get(10798945);
              case "Studio":
              case "studio":
                return GUILocalizeStrings.Get(10798946);
              case "IMDB_Rank":
              case "imdb_rank":
                return GUILocalizeStrings.Get(10798947);
              case "IsOnline":
              case "isonline":
                return GUILocalizeStrings.Get(10798948);
              case "Edition":
              case "edition":
                return GUILocalizeStrings.Get(10798949);
              case "IsOnlineTrailer":
              case "isonlinetrailer":
                return GUILocalizeStrings.Get(10798950);
              case "Persons":
              case "persons":
                return GUILocalizeStrings.Get(10798951);
              case "CategoryTrakt":
              case "categorytrakt":
                return GUILocalizeStrings.Get(10798952);
              case "AudioChannelCount":
              case "audiochannelcount":
                return GUILocalizeStrings.Get(10798953);
              case "RecentlyAdded":
              case "recentlyadded":
                return GUILocalizeStrings.Get(10798954);
              case "IndexedTitle":
              case "indexedtitle":
                return GUILocalizeStrings.Get(10798955);
              case "AgeAdded":
              case "ageadded":
                return GUILocalizeStrings.Get(10798956);
              default:
                {
                  string translation = string.Empty;
                  if (data != null)
                  {
                    foreach (AntMovieCatalog.CustomFieldRow customFieldRow in data.CustomField)
                    {
                      if (customFieldRow.Tag.ToLower() == Column.ToLower()) translation = (!customFieldRow.IsNameNull()) ? customFieldRow.Name : customFieldRow.Tag;
                    }
                  }
                  return translation;
                }
            }
        }
        #endregion
    }

  /// <summary>
  /// Movie object
  /// </summary>  
  public class MFMovie
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

    #region private vars
    private int _mID = -1;
    private string _mStrTitle = string.Empty;
    private string _mStrTranslatedTitle = string.Empty;
    private string _mStrFile = string.Empty;
    private string _mStrPath = string.Empty;
    private string _mStrIMDBNumber = string.Empty;
    private string _mStrTMDBNumber = string.Empty;
    private int _mIYear = 1900;
    private string _mStrCategory = string.Empty;
    private int _mILength = 0;
    private float _mFRating;
    private bool _mIWatched;
    private int _mIWatchedCount = -1;
    private DateTime _mDateTime = System.DateTime.Today;
    private string _mDateAdded = string.Empty;
    private string _mPicture = string.Empty;
    private string _mFanart = string.Empty;
    private string _mConfig = string.Empty;
    private string _mUsername = string.Empty;
    private bool _mReadOnly = false;
    private bool _mAllowTrakt = false;
    private bool _mAllowLatestMediaAPI = false;
    private MyFilmsGUI.Configuration _MFconfig = null;
    private DataRow _MovieRow = null;
    #endregion

    public MFMovie() { }

    #region public vars
    public int ID
    {
      get { return _mID; }
      set { _mID = value; }
    }

    public bool IsEmpty
    {
      get
      {
        if ((_mStrTitle != string.Empty) && (_mStrTitle != Strings.Unknown))
        {
          return false;
        }
        return true;
      }
    }

    public bool Watched
    {
      get { return _mIWatched; }
      set { _mIWatched = value; }
    }

    public int WatchedCount
    {
      get { return _mIWatchedCount; }
      set { _mIWatchedCount = value; }
    }

    public string Title
    {
      get { return _mStrTitle; }
      set { _mStrTitle = value; }
    }

    public string TranslatedTitle
    {
      get { return _mStrTranslatedTitle; }
      set { _mStrTranslatedTitle = value; }
    }

    public string File
    {
      get { return _mStrFile; }
      set { _mStrFile = value; }
    }

    public string Path
    {
      get { return _mStrPath; }
      set { _mStrPath = value; }
    }

    public string IMDBNumber
    {
      get { return _mStrIMDBNumber; }
      set { _mStrIMDBNumber = value; }
    }

    public string TMDBNumber
    {
      get { return _mStrTMDBNumber; }
      set { _mStrTMDBNumber = value; }
    }

    public int Year
    {
      get { return _mIYear; }
      set { _mIYear = value; }
    }

    public string Category
    {
      get { return _mStrCategory; }
      set { _mStrCategory = value; }
    }

    /// <summary>
    /// Runtime in minutes.
    /// </summary>
    public int Length
    {
      get { return _mILength; }
      set { _mILength = value; }
    }

    public float Rating
    {
      get { return _mFRating; }
      set { _mFRating = value; }
    }

    public DateTime DateTime
    {
      get { return _mDateTime; }
      set { _mDateTime = value; }
    }

    public string DateAdded
    {
      get { return _mDateAdded; }
      set { _mDateAdded = value; }
    }

    public string Picture
    {
      get { return _mPicture; }
      set { _mPicture = value; }
    }

    public string Fanart
    {
      get { return _mFanart; }
      set { _mFanart = value; }
    }

    public string Config
    {
      get { return _mConfig; }
      set { _mConfig = value; }
    }

    public string Username
    {
      get { return _mUsername; }
      set { _mUsername = value; }
    }

    public bool ReadOnly
    {
      get { return _mReadOnly; }
      set { _mReadOnly = value; }
    }

    public bool AllowTrakt
    {
      get { return _mAllowTrakt; }
      set { _mAllowTrakt = value; }
    }

    public bool AllowLatestMediaAPI
    {
      get { return _mAllowLatestMediaAPI; }
      set { _mAllowLatestMediaAPI = value; }
    }
    public MyFilmsGUI.Configuration MFconfig
    {
      get { return _MFconfig; }
      set { _MFconfig = value; }
    }
    public DataRow MovieRow
    {
      get { return _MovieRow; }
      set { _MovieRow = value; }
    }
    #endregion

    public void Reset()
    {
      _mStrTitle = string.Empty;
      _mStrTranslatedTitle = string.Empty;
      _mStrIMDBNumber = string.Empty;
      _mStrTMDBNumber = string.Empty;
      _mIYear = 1900;
      _mStrCategory = string.Empty;
      _mILength = 0;
      _mFRating = 0.0f;
      _mIWatched = false;
      _mIWatchedCount = -1;
      _mDateTime = System.DateTime.Today;
      _mDateAdded = string.Empty;
      _mPicture = string.Empty;
      _mFanart = string.Empty;
      _mConfig = string.Empty;
      _mUsername = string.Empty;
      _mReadOnly = false;
      _mAllowTrakt = false;
      _mAllowLatestMediaAPI = false;
      _MFconfig = null;
      _MovieRow = null;
    }

    public void Commit()
    {
      AntMovieCatalog dataImport = new AntMovieCatalog();

      using (XmlSettings XmlConfig = new XmlSettings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MyFilms.xml")))
      {
          string config = _mConfig; // MyFilmsGUI.Configuration tmpconf = new MyFilmsGUI.Configuration(config, true, null);
          string Catalog = XmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
          string CatalogTmp = XmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalogTemp", string.Empty);
          string FileType = XmlConfig.ReadXmlConfig("MyFilms", config, "CatalogType", "0");
          bool TraktEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
          string StrDfltSelect = XmlConfig.ReadXmlConfig("MyFilms", config, "StrDfltSelect", string.Empty);
          bool EnhancedWatchedStatusHandling = XmlConfig.ReadXmlConfig("MyFilms", config, "EnhancedWatchedStatusHandling", false);
          string GlobalUnwatchedOnlyValue = XmlConfig.ReadXmlConfig("MyFilms", config, "GlobalUnwatchedOnlyValue", "false");
          string WatchedField = XmlConfig.ReadXmlConfig("MyFilms", config, "WatchedField", "Checked");
          string UserProfileName = XmlConfig.ReadXmlConfig("MyFilms", config, "UserProfileName", "");

          if (FileType != "0" && FileType != "10")
          {
            if (!string.IsNullOrEmpty(CatalogTmp))
              Catalog = CatalogTmp;
            else if (!string.IsNullOrEmpty(Catalog) && Catalog.Contains("\\"))
            {
              string Path = System.IO.Path.GetDirectoryName(Catalog);
              Catalog = Path + "\\" + Catalog.Substring(Catalog.LastIndexOf(@"\") + 1, Catalog.Length - Catalog.LastIndexOf(@"\") - 5) + "_tmp.xml";
            }
            else
            {
              LogMyFilms.Debug("Catalog Type is readonly (EC) - tmp-Catalog not found - Update rejected ! - Movie = '" + _mStrTitle + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");
              return;
            }
          }
          LogMyFilms.Debug("Commit() : TraktSync = '" + TraktEnabled + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");
          LogMyFilms.Debug("Commit() : Update requested for Movie = '" + _mStrTitle + "' (" + _mIYear + "), IMDB = '" + _mStrIMDBNumber + "', Watched = '" + _mIWatched + "'");

          if (System.IO.File.Exists(Catalog))
          {
            if (!TraktEnabled)
            {
              LogMyFilms.Debug("Trakt not enabled for this config - Update rejected ! - Movie = '" + _mStrTitle + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");
              return;
            }
            BaseMesFilms._dataLock.EnterReadLock();
            try
            {
              using (FileStream fs = new FileStream(Catalog, FileMode.Open, FileAccess.Read, FileShare.Read))
              {
                //LogMyFilms.Debug("Commit() - opening '" + Catalog + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");

                foreach (DataTable dataTable in dataImport.Tables)
                  dataTable.BeginLoadData();
                dataImport.ReadXml(fs);
                foreach (DataTable dataTable in dataImport.Tables)
                  dataTable.EndLoadData(); 
                fs.Close();
                //LogMyFilms.Debug("Commit()- closing  '" + Catalog + "' FileStream");
              }

              DataRow[] results = dataImport.Tables["Movie"].Select(StrDfltSelect + "Number" + " = " + "'" + _mID + "'", "OriginalTitle" + " " + "ASC"); // if (results.Length != 1) continue;
              if (results.Length != 1)
                LogMyFilms.Debug("Commit() : Warning - Results found: '" + results.Length + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");

              foreach (DataRow sr in results)
              {
                try
                {
                  // watched status
                  string oldWatchedString = sr[WatchedField].ToString();
                  if (!EnhancedWatchedStatusHandling)
                  {
                    if (_mIWatched)
                      sr[WatchedField] = "true";
                    else
                      sr[WatchedField] = GlobalUnwatchedOnlyValue;
                  }
                  else
                  {
                    string EnhancedWatchedValue = sr[WatchedField].ToString();
                    string newEnhancedWatchedValue = NewEnhancedWatchValue(EnhancedWatchedValue, UserProfileName, _mIWatched, _mIWatchedCount);
                    if (!string.IsNullOrEmpty(_mUsername))
                      newEnhancedWatchedValue = NewEnhancedWatchValue(newEnhancedWatchedValue, _mUsername, _mIWatched, _mIWatchedCount);
                    sr[WatchedField] = newEnhancedWatchedValue;
                  }
                  if (sr[WatchedField].ToString().ToLower() != oldWatchedString.ToLower())
                    LogMyFilms.Debug("Commit() : Updating Field '" + WatchedField + "' from '" + oldWatchedString + "' to '" + sr[WatchedField].ToString() + "', WatchedCount = '" + _mIWatchedCount + "'");

                  // imdb number
                  string oldIMDB = sr["IMDB_Id"].ToString();
                  if (!string.IsNullOrEmpty(_mStrIMDBNumber))
                    sr["IMDB_Id"] = _mStrIMDBNumber;
                  if (sr["IMDB_Id"].ToString() != oldIMDB)
                    LogMyFilms.Debug("Commit() : Updating 'IMDB_Id' from '" + oldIMDB + "' to '" + sr["IMDB_Id"].ToString() + "'");
                }
                catch (Exception ex)
                {
                  LogMyFilms.DebugException("MyFilms videodatabase exception err: " + ex.Message + ", stack: " + ex.StackTrace, ex);
                }
              }
            }
            catch (Exception e)
            {
              LogMyFilms.Error(": Error reading xml database after " + dataImport.Movie.Count.ToString() + " records; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
            }
            finally
            {
              BaseMesFilms._dataLock.ExitReadLock();
            }

            // Now saving changes back to disk
            int maxretries = 10; // max retries 10 * 1000 = 10 seconds
            int i = 0;
            bool success = false; // result of update operation

            while (!success && i < maxretries)
            {
              // first check, if there is a global manual lock
              if (!MyFilmsDetail.GlobalLockIsActive(Catalog))
              {
                MyFilmsDetail.SetGlobalLock(true, Catalog);
                try
                {
                  MyFilms.FSwatcher.EnableRaisingEvents = true; // re enable watcher - as myfilms should auto update dataset for current config, if update is done from trakt
                }
                catch (Exception ex)
                {
                  LogMyFilms.Debug("Commit()- FSwatcher - problem enabling Raisingevents - Message:  '" + ex.Message);
                }
                if (BaseMesFilms._dataLock.TryEnterWriteLock(10000))
                {
                  try
                  {
                    using (FileStream fs = new FileStream(Catalog, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) // lock the file for any other use, as we do write to it now !
                    {
                      LogMyFilms.Debug("Commit()- opening '" + Catalog + "' as FileStream with FileMode.OpenOrCreate, FileAccess.Write, FileShare.None");
                      fs.SetLength(0); // do not append, owerwrite !

                      using (XmlTextWriter MyXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.Default))
                      {
                        LogMyFilms.Debug("Commit()- writing '" + Catalog + "' as MyXmlTextWriter in FileStream");
                        MyXmlTextWriter.Formatting = System.Xml.Formatting.Indented; // Added by Guzzi to get properly formatted output XML
                        MyXmlTextWriter.WriteStartDocument();
                        dataImport.WriteXml(MyXmlTextWriter, XmlWriteMode.IgnoreSchema);
                        MyXmlTextWriter.Flush();
                        MyXmlTextWriter.Close();
                      }
                      fs.Close(); // write buffer and release lock on file (either Flush, Dispose or Close is required)
                      LogMyFilms.Debug("Commit()- closing '" + Catalog + "' FileStream and releasing file lock");
                      success = true;
                    }
                  }
                  catch (Exception saveexeption)
                  {
                    LogMyFilms.Debug("Commit()- exception while trying to save data in '" + Catalog + "' - exception: " + saveexeption.Message + ", stacktrace: " + saveexeption.StackTrace);
                    success = false;
                  }
                  finally
                  {
                    BaseMesFilms._dataLock.ExitWriteLock();
                  }
                }
                else
                {
                  LogMyFilms.Debug("Commit()- timeout when waiting for slim writelock - could not write data!");
                }
                MyFilmsDetail.SetGlobalLock(false, Catalog);
              }
              else
              {
                i += 1;
                LogMyFilms.Info("Movie Database locked on try '" + i + " of " + maxretries + "' to write, waiting for next retry");
                Thread.Sleep(1000);
              }
            }
          }
        } 
      dataImport.Reset();
      if (dataImport != null)
        dataImport.Dispose();
    }

    private string NewEnhancedWatchValue(string EnhancedWatchedValue, string UserProfileName, bool watched, int count)
    {
      string newEnhancedWatchedValue = "";
      if (_mIWatchedCount > -1) 
        count = _mIWatchedCount;
      if (!watched)
        count = 0;

      if (EnhancedWatchedValue.Contains(UserProfileName))
      {
        string[] split = EnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in split)
        {
          if (s.Contains(":"))
          {
            string sNew = s;
            string tempuser = MyFilmsDetail.EnhancedWatchedValue(s, "username");
            string tempcount = MyFilmsDetail.EnhancedWatchedValue(s, "count");
            string temprating = MyFilmsDetail.EnhancedWatchedValue(s, "rating");
            string tempdatewatched = MyFilmsDetail.EnhancedWatchedValue(s, "datewatched");
            

            if (tempuser == "Global" && int.Parse(tempcount) < count) // Update Count Value for Global count, if it is lower than user count
            {
              sNew = tempuser + ":" + count + ":" + temprating + ":" + tempdatewatched;
            }
            if (tempuser == UserProfileName) // Update Count Value for selected user
            {
              sNew = tempuser + ":" + count + ":" + temprating + ":" + tempdatewatched;
            }
            if (string.IsNullOrEmpty(newEnhancedWatchedValue))
              newEnhancedWatchedValue = sNew;
            else
              newEnhancedWatchedValue += "|" + sNew;
          }
        }
      }
      else
      {
        if (string.IsNullOrEmpty(EnhancedWatchedValue) || !EnhancedWatchedValue.Contains(":"))
          newEnhancedWatchedValue = "Global:" + count + ":-1:|" + UserProfileName + ":" + count + ":" + "-1:";
        else
          newEnhancedWatchedValue = EnhancedWatchedValue + "|" + UserProfileName + ":" + count + ":" + "-1:";
      }
      return newEnhancedWatchedValue;
    }

  }
}
