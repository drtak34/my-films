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
  using System.Globalization;
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

        // private static Dictionary<string, ReaderWriterLockSlim> _lockDict = new Dictionary<string, ReaderWriterLockSlim>();
        public static ReaderWriterLockSlim _dataLock = new ReaderWriterLockSlim(); // private static ReaderWriterLockSlim _dataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        // private static readonly object _locker = new object();

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
        private static DataRow[] movies;                // selected movies with filters
        // private static DataTable tableMoviesExtended;   // all extended Movie DataTable as join with customfields ...
        private static Stopwatch watch = new Stopwatch();

        #region ctor
        static BaseMesFilms()
        {
          // _lockDict = GetLockerList();
        }
        #endregion

        #region private static methods ...
        private static Dictionary<string, ReaderWriterLockSlim> GetLockerList()
        {
          Dictionary<string, ReaderWriterLockSlim> lockDictionary = new Dictionary<string, ReaderWriterLockSlim>();
          using (XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
          {
            int MesFilms_nb_config = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
            for (int i = 0; i < MesFilms_nb_config; i++) lockDictionary.Add(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty), new ReaderWriterLockSlim());
          }
          return lockDictionary;
        }

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

        public class DataColumnComparer : IEqualityComparer<DataColumn>
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
              from movieRow in data.Movie.AsEnumerable()
              join customFieldsRow in data.CustomFields.AsEnumerable()
              on movieRow.Field<int>("Movie_Id") equals customFieldsRow.Field<int>("Movie_Id")
              //into leftjoined
              // where movieRow.Field<string>("Studio") == ""
              // where movieRow.Field<decimal?>("Rating") > 5
              // orderby movieRow.OriginalTitle ascending

              // from tmp in leftjoined.DefaultIfEmpty()
              select new
              {
                // movieRow.Movie_Id,
                Number = movieRow.Field<int>("Number"),
                OriginalTitle = movieRow.Field<string>("OriginalTitle"),
                TranslatedTitle = movieRow.Field<string>("TranslatedTitle"),
                FormattedTitle = movieRow.Field<string>("FormattedTitle"),
                IndexedTitle = movieRow.Field<string>("IndexedTitle"),
                Checked = movieRow.Field<string>("Checked"),
                MediaLabel = movieRow.Field<string>("MediaLabel"),
                MediaType = movieRow.Field<string>("MediaType"),
                Source = movieRow.Field<string>("Source"),
                Date = movieRow.Field<string>("Date"),
                Borrower = movieRow.Field<string>("Borrower"),
                // Rating = movieRow.Field<Decimal>("Rating"),
                Description = movieRow.Field<string>("Description"),
                Comments = movieRow.Field<string>("Comments"),
                Director = movieRow.Field<string>("Director"),
                Producer = movieRow.Field<string>("Producer"),
                Country = movieRow.Field<string>("Country"),
                Category = movieRow.Field<string>("Category"),
                Year = movieRow.Field<string>("Year"),
                Length = movieRow.Field<string>("Length"),
                Actors = movieRow.Field<string>("Actors"),
                URL = movieRow.Field<string>("URL"),
                VideoFormat = movieRow.Field<string>("VideoFormat"),
                VideoBitrate = movieRow.Field<string>("VideoBitrate"),
                AudioFormat = movieRow.Field<string>("AudioFormat"),
                AudioBitrate = movieRow.Field<string>("AudioBitrate"),
                Resolution = movieRow.Field<string>("Resolution"),
                Framerate = movieRow.Field<string>("Framerate"),
                Languages = movieRow.Field<string>("Languages"),
                Subtitles = movieRow.Field<string>("Subtitles"),
                Size = movieRow.Field<string>("Size"),
                // DateAdded = movieRow.Field<DateTime>("DateAdded"),
                RecentlyAdded = movieRow.Field<string>("RecentlyAdded"),
                AgeAdded = movieRow.Field<int>("AgeAdded"),
                Disks = movieRow.Field<string>("Disks"),
                Picture = movieRow.Field<string>("Picture"),
                Persons = movieRow.Field<string>("Persons"),

                // Extended Fields
                IsOnline = movieRow.Field<string>("IsOnline"),
                IsOnlineTrailer = movieRow.Field<string>("IsOnlineTrailer"),
                //CustomField1 = movieRow.Field<string>("CustomField1"),
                //CustomField2 = movieRow.Field<string>("CustomField2"),
                //CustomField3 = movieRow.Field<string>("CustomField3"),
                //RatingUser = movieRow.Field<string>("RatingUser"),
                //Edition = movieRow.Field<string>("Edition"),
                //Fanart = movieRow.Field<string>("Fanart"),
                //Certification = movieRow.Field<string>("Certification"),
                //Writer = movieRow.Field<string>("Writer"),
                //Watched = movieRow.Field<string>("Watched"),
                //Favorite = movieRow.Field<string>("Favorite"),
                //IMDB_Id = movieRow.Field<string>("IMDB_Id"),
                //TMDB_Id = movieRow.Field<string>("TMDB_Id"),
                //SourceTrailer = movieRow.Field<string>("SourceTrailer"),
                //TagLine = movieRow.Field<string>("TagLine"),
                //Tags = movieRow.Field<string>("Tags"),
                //Studio = movieRow.Field<string>("Studio"),
                //IMDB_Rank = movieRow.Field<string>("IMDB_Rank"),
                //Aspectratio = movieRow.Field<string>("Aspectratio"),
                //CategoryTrakt = movieRow.Field<string>("CategoryTrakt"),
                //LastPosition = movieRow.Field<string>("LastPosition"),
                //AudioChannelCount = movieRow.Field<string>("AudioChannelCount"),

                // CustomFields
                IsOnline_Cust = customFieldsRow.Field<string>("IsOnline"),
                IsOnlineTrailer_Cust = customFieldsRow.Field<string>("IsOnlineTrailer"),
                CustomField1 = customFieldsRow.Field<string>("CustomField1"),
                CustomField2 = customFieldsRow.Field<string>("CustomField2"),
                CustomField3 = customFieldsRow.Field<string>("CustomField3"),
                Edition = customFieldsRow.Field<string>("Edition"),
                Studio = customFieldsRow.Field<string>("Studio"),
                Fanart = customFieldsRow.Field<string>("Fanart"),
                Certification = customFieldsRow.Field<string>("Certification"),
                Writer = customFieldsRow.Field<string>("Writer"),
                TagLine = customFieldsRow.Field<string>("TagLine"),
                Tags = customFieldsRow.Field<string>("Tags"),
                Aspectratio = customFieldsRow.Field<string>("Aspectratio"),
                CategoryTrakt = customFieldsRow.Field<string>("CategoryTrakt"),
                Watched = customFieldsRow.Field<string>("Watched"),
                Favorite = customFieldsRow.Field<string>("Favorite"),
                // RatingUser = customFieldsRow.Field<Decimal>("RatingUser"),
                IMDB_Id = customFieldsRow.Field<string>("IMDB_Id"),
                TMDB_Id = customFieldsRow.Field<string>("TMDB_Id"),
                IMDB_Rank = customFieldsRow.Field<string>("IMDB_Rank"),
                SourceTrailer = customFieldsRow.Field<string>("SourceTrailer"),
                LastPosition = customFieldsRow.Field<string>("LastPosition"),
                AudioChannelCount = customFieldsRow.Field<string>("AudioChannelCount")
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

        private static void CopyExtendedFieldsToCustomFields(bool cleanfileonexit)
        {
          Stopwatch saveDataWatch = new Stopwatch();
          saveDataWatch.Reset(); saveDataWatch.Start();
          IEnumerable<DataColumn> commonColumns = data.Movie.Columns.OfType<DataColumn>().Intersect(data.CustomFields.Columns.OfType<DataColumn>(), new DataColumnComparer());
          foreach (AntMovieCatalog.MovieRow movieRow in data.Movie)
          {
            movieRow.BeginEdit();
            AntMovieCatalog.CustomFieldsRow customFields = movieRow.GetCustomFieldsRows()[0];
            foreach (DataColumn dc in commonColumns)
            {
              object temp;
              // if (dc.ColumnName != "Movie_Id" && DBNull.Value != (temp = customFields[dc.ColumnName]))  movieRow[dc.ColumnName] = temp; // diabled the copy from customfields to MyFilms rows - this is only when saving and we do not modify customfields in plugin !
              if (dc.ColumnName != "Movie_Id" && DBNull.Value != (temp = movieRow[dc.ColumnName]))
              {
                customFields[dc.ColumnName] = temp;
                if (cleanfileonexit) movieRow[dc.ColumnName] = System.Convert.DBNull;
              }
            }
          }
          data.Movie.AcceptChanges();
          saveDataWatch.Stop();
          LogMyFilms.Debug("CopyExtendedFieldsToCustomFields() - Copy CustomFields from MovieRow's done ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");
        }

        private static void CreateMissingCustomFieldsEntries()
        {
          Stopwatch loadDataWatch = new Stopwatch(); loadDataWatch.Reset(); loadDataWatch.Start();
          foreach (AntMovieCatalog.MovieRow movieRow in data.Movie)
          {
            AntMovieCatalog.CustomFieldsRow[] customFieldsRows = movieRow.GetCustomFieldsRows();
            if (customFieldsRows.Length == 0) // create CustomFields Element, if not existing ...
            {
              AntMovieCatalog.CustomFieldsRow customFields = data.CustomFields.NewCustomFieldsRow();
              customFields.SetParentRow(movieRow);
              data.CustomFields.AddCustomFieldsRow(customFields);
              // LogMyFilms.Debug("LoadMyFilmsFromDisk() - created new CustomFieldsRow for movie ID '" + movieRow.Number + "', Title = '" + movieRow.OriginalTitle + "'");
            }
          }
          loadDataWatch.Stop();
          LogMyFilms.Debug("CreateMissingCustomFieldsEntries() - done ... (" + (loadDataWatch.ElapsedMilliseconds) + " ms)");
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
          LogMyFilms.Debug("CreateOrUpdateCustomsFieldsProperties() - add Properties done ... (" + (saveDataWatch.ElapsedMilliseconds) + " ms)");

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
          CustomFieldList.Add(new string[] { "RatingUser", "RatingUser", "ftReal" }); // Decimal in MyFilms ...
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

                CopyExtendedFieldsToCustomFields(false);

                using (FileStream fsTmp = System.IO.File.Create(catalogfile.Replace(".xml", ".tmp"), 1000, FileOptions.DeleteOnClose)) // make sure, only one process is writing to file !
                {
                  using (FileStream fs = new FileStream(catalogfile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) // lock the file for any other use, as we do write to it now !
                  {
                    LogMyFilms.Debug("SaveMyFilmsToDisk()- opening '" + catalogfile + "' as FileStream with FileMode.OpenOrCreate, FileAccess.Write, FileShare.None");
                    fs.SetLength(0); // do not append, owerwrite !
                    using (XmlTextWriter MyXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.Default))
                    {
                      LogMyFilms.Debug("SaveMyFilmsToDisk()- writing '" + catalogfile + "' as MyXmlTextWriter in FileStream");
                      MyXmlTextWriter.Formatting = System.Xml.Formatting.Indented;
                      MyXmlTextWriter.WriteStartDocument();
                      data.WriteXml(MyXmlTextWriter, XmlWriteMode.IgnoreSchema); MyXmlTextWriter.Flush();
                      MyXmlTextWriter.Close();
                    }
                    //xmlDoc.Save(fs);
                    fs.Close(); // write buffer and release lock on file (either Flush, Dispose or Close is required)
                    LogMyFilms.Debug("SaveMyFilmsToDisk()- closing '" + catalogfile + "' FileStream and releasing file lock");
                    success = true;
                  }
                }
                Thread.Sleep(50);
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
          bool success = false;
          LogMyFilms.Debug("LoadMyFilmsFromDisk() - Current Readlocks: '" + _dataLock.CurrentReadCount + "'");
          // if (_dataLock.CurrentReadCount > 0) return false;// might be opened by API as well, so count can be 2+

          _dataLock.EnterWriteLock();
          try
          {
            #region load catalog from file into dataset
            watch.Reset(); watch.Start();
            int _numberOfTries = 20;
            int _timeIntervalBetweenTries = 500;
            var tries = 0;
            while (true)
            {
              try
              {
                using (FileStream fsTmp = System.IO.File.Create(catalogfile.Replace(".xml", ".tmp"), 1000, FileOptions.DeleteOnClose)) // make sure, no process is writing to file !
                {
                  using (FileStream fs = new FileStream(catalogfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                  {
                    data = new AntMovieCatalog();
                    LogMyFilms.Debug("LoadMyFilmsFromDisk() - opening '" + catalogfile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.ReadWrite");
                    foreach (DataTable dataTable in data.Tables) dataTable.BeginLoadData(); // dataTable.Rows.Clear();
                    //// synchronize dataset with hierarchical XMLdoc
                    //xmlDoc = new XmlDataDocument(data);
                    //xmlDoc.Load(fs);
                    data.ReadXml(fs);
                    foreach (DataTable dataTable in data.Tables) dataTable.EndLoadData();
                    fs.Close();
                    LogMyFilms.Debug("LoadMyFilmsFromDisk() - closing  '" + catalogfile + "' FileStream");
                  }
                }
                success = true;
                break;
              }
              catch (Exception e) // catch (IOException e)
              {
                // if (!Helper.IsFileLocked(e)) throw;
                if (++tries > _numberOfTries)
                {
                  // throw new MyCustomException("The file is locked too long: " + e.Message, e);
                  LogMyFilms.Error("LoadMyFilmsFromDisk() - File locked too long ! Returning error !");
                  return false;
                }
                else
                {
                  LogMyFilms.Debug("LoadMyFilmsFromDisk() - File is locked on try '" + tries + "' of '" + _numberOfTries + "' - waiting another '" + _timeIntervalBetweenTries + "' ms. - reason: " + e.Message);
                  Thread.Sleep(_timeIntervalBetweenTries);
                }
              }
            }

            //foreach (DataTable dataTable in data.Tables)
            //{
            //  LogMyFilms.Debug("initData() - loaded table '" + dataTable + "'");
            //  foreach (var childrelation in dataTable.ChildRelations) LogMyFilms.Debug("initData() - childrelation: '" + childrelation + "'");
            //}
            watch.Stop();
            LogMyFilms.Debug("LoadMyFilmsFromDisk() - Finished  (" + (watch.ElapsedMilliseconds) + " ms)");
            #endregion

            CreateOrUpdateCustomsFieldsProperties();

            CreateMissingCustomFieldsEntries();

            #region calculate artificial columns like AgeAdded, IndexedTitle, Persons, etc. and CustomFields Copy ...
            DateTime now = DateTime.Now;
            watch.Reset(); watch.Start();
            IEnumerable<DataColumn> commonColumns = data.Movie.Columns.OfType<DataColumn>().Intersect(data.CustomFields.Columns.OfType<DataColumn>(), new DataColumnComparer());
            //data.Movie.BeginLoadData();
            //data.EnforceConstraints = false; // primary key uniqueness, foreign key referential integrity and nulls in columns with AllowDBNull = false etc...
            foreach (AntMovieCatalog.MovieRow movieRow in data.Movie)
            {
              movieRow.BeginEdit();
              //// Convert(Date,'System.DateTime')
              DateTime added;
              //int iAge = 9999; // set default to 9999 for those, where we do not have date(added) in DB ...
              //// CultureInfo ci = CultureInfo.CurrentCulture;
              //if (!movieRow.IsDateNull() && DateTime.TryParse(movieRow.Date, out added)) // CultureInfo.InvariantCulture ??? // else movieRow.DateAdded = DateTime.MinValue; ???
              //{
              //  // movieRow.DateAdded = Convert.ToDateTime(movieRow.Date); // is same as: movieRow.DateAdded = DateTime.Parse(movieRow.Date, CultureInfo.CurrentCulture);
              //  movieRow.DateAdded = added;
              //  iAge = (int)now.Subtract(added).TotalDays; // iAge = (!movieRow.IsDateAddedNull()) ? ((int)now.Subtract(movieRow.DateAdded).TotalDays) : 9999;
              //}
              //movieRow.AgeAdded = iAge; // sets integer value
              //movieRow.RecentlyAdded = MyFilms.GetDayRange(iAge);
              if (!movieRow.IsDateNull() && DateTime.TryParse(movieRow.Date, out added)) // CultureInfo.InvariantCulture ??? // else movieRow.DateAdded = DateTime.MinValue; ???
                movieRow.DateAdded = Convert.ToDateTime(movieRow.Date); // is same as: movieRow.DateAdded = DateTime.Parse(movieRow.Date, CultureInfo.CurrentCulture);
              else
                movieRow.DateAdded = DateTime.MinValue;
              // movieRow.DateAdded = Convert.ToDateTime(movieRow.Date); // is same as: movieRow.DateAdded = DateTime.Parse(movieRow.Date, CultureInfo.CurrentCulture);
              movieRow.AgeAdded = (int)now.Subtract(movieRow.DateAdded).TotalDays; // sets integer value
              movieRow.RecentlyAdded = MyFilms.GetDayRange(movieRow.AgeAdded);
              string index = movieRow[MyFilms.conf.StrTitle1].ToString();
              movieRow.IndexedTitle = (index.Length > 0) ? index.Substring(0, 1).ToUpper() : "";
              movieRow.Persons = (movieRow.Actors ?? " ") + ", " + (movieRow.Producer ?? " ") + ", " + (movieRow.Director ?? " ") + ", " + (movieRow.Writer ?? " "); // Persons: ISNULL(Actors,' ') + ', ' + ISNULL(Producer, ' ') + ', ' + ISNULL(Director, ' ') + ', ' + ISNULL(Writer, ' ')
              // if (!movieRow.IsLengthNull()) movieRow.Length_Num = Convert.ToInt32(movieRow.Length);

              // Copy CustomFields data ....
              AntMovieCatalog.CustomFieldsRow customFields = movieRow.GetCustomFieldsRows()[0]; // Relations["Movie_CustomFields"]
              foreach (DataColumn dc in commonColumns)
              {
                object temp;
                if (dc.ColumnName != "Movie_Id" && DBNull.Value != (temp = customFields[dc.ColumnName])) movieRow[dc.ColumnName] = temp;
              }
            }
            //data.EnforceConstraints = true;
            //data.Movie.EndLoadData();
            LogMyFilms.Debug("LoadMyFilmsFromDisk() - Calc PreAcceptChanges ... (" + (watch.ElapsedMilliseconds) + " ms)");
            data.Movie.AcceptChanges();
            watch.Stop();
            LogMyFilms.Debug("LoadMyFilmsFromDisk() - Calc & CustomField Copy Finished ... (" + (watch.ElapsedMilliseconds) + " ms)");
            #endregion

            #region Other join table tests
            //watch.Reset(); watch.Start();
            //var commonColumnsX = data.Movie.Columns.OfType<DataColumn>().Intersect(data.CustomFields.Columns.OfType<DataColumn>(), new DataColumnComparer());
            // var onlyCustomFieldColumns = data.CustomFields.Columns.OfType<DataColumn>().Except(data.Movie.Columns.OfType<DataColumn>(), new DataColumnComparer());
            //tableMoviesExtended = new DataTable();
            //tableMoviesExtended.Columns.AddRange(
            //    data.Movie.Columns.OfType<DataColumn>()
            //    .Union(data.CustomFields.Columns.OfType<DataColumn>(), new DataColumnComparer())
            //    .Select(c => new DataColumn(c.Caption, c.DataType, c.Expression, c.ColumnMapping))
            //    .ToArray());

            //var rowData2 = data.Movie.AsEnumerable().Join(
            //    data.CustomFields.AsEnumerable(),
            //    row => commonColumnsX.Select(col => row[col.Caption]).ToArray(),
            //    row => commonColumnsX.Select(col => row[col.Caption]).ToArray(),
            //    (row1, row2) =>
            //    {
            //      var row = tableMoviesExtended.NewRow();
            //      row.ItemArray = tableMoviesExtended.Columns.OfType<DataColumn>().Select(col => row1.Table.Columns.Contains(col.Caption) ? row1[col.Caption] : row2[col.Caption]).ToArray();
            //      return row;
            //    },
            //    new ObjectArrayComparer());

            //foreach (var row in rowData2)
            //  tableMoviesExtended.Rows.Add(row);
            //watch.Stop();
            //foreach (DataColumn dataColumn in tableMoviesExtended.Columns)
            //  LogMyFilms.Debug("LoadMyFilmsFromDisk() - Table Join version 2 containing Column '" + dataColumn.ToString() + "'");

            //LogMyFilms.Debug("LoadMyFilmsFromDisk() - Copy joined data to new datatable version 2 done ... (" + (watch.ElapsedMilliseconds) + " ms)");
            #endregion

            #region create new enhanced movie rowcollection by joining movie and customfields rows
            //watch.Reset(); watch.Start();
            //tableMoviesExtended = SQLOps.Join(data.Movie, data.CustomFields, data.Movie.Movie_IdColumn, data.CustomFields.Movie_IdColumn); // Medium - 2 secs
            //int rows = tableMoviesExtended.Rows.Count;
            //watch.Stop();
            //foreach (DataColumn dataColumn in tableMoviesExtended.Columns) LogMyFilms.Debug("LoadMyFilmsFromDisk() - SQLOps.Join() - containing Column '" + dataColumn + "'");
            //LogMyFilms.Debug("LoadMyFilmsFromDisk() - GetEnhancedMovies() - creating '" + rows + "' rows by SQLOps.Join movie and customfields done ... (" + (watch.ElapsedMilliseconds) + " ms)");

            //watch.Reset(); watch.Start();
            //tableMoviesExtended = GetEnhancedMovies(); // SLOW !!!! > 11 secs for loading movies ...
            //int irows = tableMoviesExtended.Rows.Count;
            //watch.Stop();
            //foreach (DataColumn dataColumn in tableMoviesExtended.Columns) LogMyFilms.Debug("LoadMyFilmsFromDisk() - GetEnhancedMovies() v1 - containing Column '" + dataColumn + "'");
            //LogMyFilms.Debug("LoadMyFilmsFromDisk() - GetEnhancedMovies() - creating '" + irows + "' rows by joining movie and customfields done ... (" + (watch.ElapsedMilliseconds) + " ms)");
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
            //// create extended movie table ...
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
            #endregion
          
          }
          catch (Exception e)
          {
            success = false;
            LogMyFilms.Error("LoadMyFilmsFromDisk() : Error reading xml database after " + data.Movie.Count + " records; error : " + e.Message + ", " + e.StackTrace);
            if (data.Movie.Count > 0) LogMyFilms.Error("LoadMyFilmsFromDisk() : Last Record: '" + data.Movie[data.Movie.Count - 1].Number + "', title: '" + data.Movie[data.Movie.Count - 1].OriginalTitle + "'");
            string strOtitle = (data.Movie.Count > 0) ? data.Movie[data.Movie.Count - 1].OriginalTitle : "n/a";
            throw new Exception("Error reading xml database after " + data.Movie.Count + " records; movie: '" + strOtitle + "'; error : " + e.Message);
            //throw e;
          }
          finally
          {
            _dataLock.ExitWriteLock();
          }

          return success;
        }

    
        /// <summary>
        /// Summary description for SQLOps.
        /// </summary>
        public class SQLOps
        {
          public SQLOps()
          {
          }

          //FJC = First Join Column
          //SJC = Second Join Column

        public static DataTable Join(DataTable First, DataTable Second, DataColumn[] FJC, DataColumn[] SJC)
        {
          //Create Empty Table
          DataTable table = new DataTable("Join");

          // Use a DataSet to leverage DataRelation
          using (DataSet ds = new DataSet())
          {
            //Add Copy of Tables
            ds.Tables.AddRange(new DataTable[] { First.Copy(), Second.Copy() });

            //Identify Joining Columns from First
            DataColumn[] parentcolumns = new DataColumn[FJC.Length];

            for (int i = 0; i < parentcolumns.Length; i++)
            {
              parentcolumns[i] = ds.Tables[0].Columns[FJC[i].ColumnName];
            }

            //Identify Joining Columns from Second
            DataColumn[] childcolumns = new DataColumn[SJC.Length];

            for (int i = 0; i < childcolumns.Length; i++)
            {
              childcolumns[i] = ds.Tables[1].Columns[SJC[i].ColumnName];
            }

            //Create DataRelation
            DataRelation r = new DataRelation(string.Empty, parentcolumns, childcolumns, false);
            ds.Relations.Add(r);

            //Create Columns for JOIN table
            for (int i = 0; i < First.Columns.Count; i++)
            {
              table.Columns.Add(First.Columns[i].ColumnName, First.Columns[i].DataType);
            }

            for (int i = 0; i < Second.Columns.Count; i++)
            {
              //Beware Duplicates
              if (!table.Columns.Contains(Second.Columns[i].ColumnName))
                table.Columns.Add(Second.Columns[i].ColumnName, Second.Columns[i].DataType);
              else
                table.Columns.Add(Second.Columns[i].ColumnName + "_Second", Second.Columns[i].DataType);
            }


            //Loop through First table
            table.BeginLoadData();

            foreach (DataRow firstrow in ds.Tables[0].Rows)
            {
              //Get "joined" rows
              DataRow[] childrows = firstrow.GetChildRows(r);
              if (childrows != null && childrows.Length > 0)
              {
                object[] parentarray = firstrow.ItemArray;
                foreach (DataRow secondrow in childrows)
                {
                  object[] secondarray = secondrow.ItemArray;
                  object[] joinarray = new object[parentarray.Length + secondarray.Length];
                  Array.Copy(parentarray, 0, joinarray, 0, parentarray.Length);
                  Array.Copy(secondarray, 0, joinarray, parentarray.Length, secondarray.Length);
                  table.LoadDataRow(joinarray, true);
                }
              }
              else
              {
                object[] parentarray = firstrow.ItemArray;
                object[] joinarray = new object[parentarray.Length];
                Array.Copy(parentarray, 0, joinarray, 0, parentarray.Length);
                table.LoadDataRow(joinarray, true);
              }
            }
            table.EndLoadData();
          }

          return table;
        }

        public static DataTable Join(DataTable First, DataTable Second, DataColumn FJC, DataColumn SJC)
        {
          return SQLOps.Join(First, Second, new DataColumn[] { FJC }, new DataColumn[] { SJC });
        }

        public static DataTable Join(DataTable First, DataTable Second, string FJC, string SJC)
        {
          return SQLOps.Join(First, Second, new DataColumn[] { First.Columns[FJC] }, new DataColumn[] { First.Columns[SJC] });
        } 
  }

        private static ArrayList GetMoviesGlobal(string Expression, string Sort, bool traktOnly)
        {
          ArrayList moviesGlobal = new ArrayList();
          moviesGlobal.Clear();
          int moviecount = 0; // total count for added movies

          // AntMovieCatalog dataExport = new AntMovieCatalog();
          using (AntMovieCatalog dataExport = new AntMovieCatalog())
          {
            using (XmlSettings xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
            {
              int mesFilmsNbConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
              ArrayList configs = new ArrayList();
              for (int i = 0; i < mesFilmsNbConfig; i++) configs.Add(xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));

              LogMyFilms.Debug("GetMoviesGlobal() - Configs found: '" + configs.Count + "' - now checking, if they are enabled ...");
              foreach (string config in configs)
              {
                dataExport.Clear();
                string StrFileXml = xmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
                string StrFileType = xmlConfig.ReadXmlConfig("MyFilms", config, "CatalogType", "0");
                bool AllowTraktSync = xmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
                bool AllowRecentlyAddedAPI = xmlConfig.ReadXmlConfig("MyFilms", config, "AllowRecentAddedAPI", false);
                string StrUserProfileName = xmlConfig.ReadXmlConfig("MyFilms", config, "UserProfileName", "Global");

                string catalogname = Enum.GetName(typeof(MyFilmsGUI.Configuration.CatalogType), Int32.Parse(StrFileType));

                if (AllowTraktSync || AllowRecentlyAddedAPI)
                  LogMyFilms.Debug("Trakt|LMH = '" + AllowTraktSync + "|" + AllowRecentlyAddedAPI + "', Config = '" + config + "', CatalogType = '" + StrFileType + "|" + catalogname + "', DBFile = '" + StrFileXml + "'");
                else
                  LogMyFilms.Debug("Trakt|LMH = '" + AllowTraktSync + "|" + AllowRecentlyAddedAPI + "', Config = '" + config + "', CatalogType = '" + StrFileType + "|" + catalogname + "'");
                if (File.Exists(StrFileXml) && (AllowTraktSync || (!traktOnly && AllowRecentlyAddedAPI)))
                {
                  MyFilmsGUI.Configuration tmpconf = new MyFilmsGUI.Configuration(config, false, true, null);
                  //if (!_lockDict.ContainsKey(tmpconf.StrFileXml))_lockDict.Add(tmpconf.StrFileXml, new ReaderWriterLockSlim());
                  //_lockDict["string"].EnterWriteLock();
                  _dataLock.EnterReadLock();
                  try
                  {
                    // dataExport.ReadXml(tmpconf.StrFileXml);
                    const int numberOfTries = 8;
                    const int timeIntervalBetweenTries = 250;
                    var tries = 0;
                    while (true)
                    {
                      try
                      {
                        using (FileStream fs = new FileStream(tmpconf.StrFileXml, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                          foreach (DataTable dataTable in dataExport.Tables) dataTable.BeginLoadData(); // dataTable.Rows.Clear();
                          dataExport.ReadXml(fs);
                          foreach (DataTable dataTable in dataExport.Tables) dataTable.EndLoadData();
                          fs.Close();
                        }
                        break;
                      }
                      catch (IOException e)
                      {
                        if (!Helper.IsFileLocked(e)) throw;
                        if (++tries > numberOfTries)
                        {
                          // throw new MyCustomException("The file is locked too long: " + e.Message, e);
                          LogMyFilms.Error("GetMoviesGlobal() - File locked too long ! Returning error !");
                          break;
                        }
                        else
                        {
                          LogMyFilms.Debug("LoadMyFilmsFromDisk() - File '" + tmpconf.StrFileXml + "' is locked on try '" + tries + "' of '" + numberOfTries + "' - waiting another '" + timeIntervalBetweenTries + "' ms.");
                          Thread.Sleep(timeIntervalBetweenTries);
                        }
                      }
                    }
                  }
                  catch (Exception e)
                  {
                    LogMyFilms.Error("GetMoviesGlobal() - Error reading xml database after " + dataExport.Movie.Count.ToString() + " records; error : " + e.Message + ", " + e.StackTrace);
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
                        MFMovie movie = new MFMovie { Config = config, Username = StrUserProfileName }; // MF config context
                        GetMovieDetails(sr, tmpconf, (!traktOnly && tmpconf.AllowRecentlyAddedAPI), ref movie);
                        moviesGlobal.Add(movie);
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
          }
          return moviesGlobal;
        }

        private static void GetMovieDetails(DataRow row, MyFilmsGUI.Configuration tmpconf, bool getDataRowDetailsForArtwork, ref MFMovie movie)
        {
          //-----------------------------------------------------------------------------------------------------------------------
          //    Load Movie Details Info
          //-----------------------------------------------------------------------------------------------------------------------

          movie.ReadOnly = tmpconf.ReadOnly; // is true for readonly catalog types

          #region Number
          if (!string.IsNullOrEmpty(row["Number"].ToString()))
            movie.ID = Int32.Parse(row["Number"].ToString());
          else movie.ID = 0;
          #endregion

          #region year
          int year = 1900;
          Int32.TryParse(row["Year"].ToString(), out year);
          movie.Year = year;
          #endregion

          #region Category
          movie.Category = row["Category"].ToString();
          #endregion

          #region length
          int length = 0;
          Int32.TryParse(row["Length"].ToString(), out length);
          movie.Length = length;
          #endregion

          #region OriginalTitle & TranslatedTitle & FormattedTitle
          movie.Title = row["OriginalTitle"].ToString();
          if (row[tmpconf.StrTitle1].ToString().Length > 0)
          {
            int TitlePos = (row[tmpconf.StrTitle1].ToString().LastIndexOf(tmpconf.TitleDelim) > 0) ? row[tmpconf.StrTitle1].ToString().LastIndexOf(tmpconf.TitleDelim) + 1 : 0; //only display rest of title after selected part common to group
            movie.Title = row[tmpconf.StrTitle1].ToString().Substring(TitlePos);
            movie.GroupName = (TitlePos > 1) ? row[tmpconf.StrTitle1].ToString().Substring(TitlePos - 1) : "";
          }
          if (Helper.FieldIsSet(tmpconf.StrTitle2))
          {
            int TitlePos = (row[tmpconf.StrTitle2].ToString().LastIndexOf(tmpconf.TitleDelim) > 0) ? row[tmpconf.StrTitle2].ToString().LastIndexOf(tmpconf.TitleDelim) + 1 : 0;
            movie.TranslatedTitle = row[tmpconf.StrTitle2].ToString().Substring(TitlePos);
          }
          if (getDataRowDetailsForArtwork)
          {
            movie.FormattedTitle = row["FormattedTitle"].ToString() ?? "";
          }
          #endregion

          #region Edition
          movie.Edition = row["Edition"].ToString();
          #endregion

          #region watched status
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
          #endregion

          #region site rating
          float rating = 0;
          if (!(float.TryParse(row["Rating"].ToString(), out rating))) rating = 0;
          movie.Rating = rating;
          #endregion

          #region user rating
          rating = 0;
          if (tmpconf.StrEnhancedWatchedStatusHandling) // get usercontext ratings, if enabled
          {
            string tmprating = GetUserRating(row[tmpconf.StrWatchedField].ToString(), tmpconf.StrUserProfileName);
            if (tmprating != "-1")
            {
              if (!(float.TryParse(tmprating, out rating))) 
                rating = 0;
            }
          }
          else
          {
            if (!(float.TryParse(row["RatingUser"].ToString(), out rating))) rating = 0;
          }
          movie.RatingUser = rating; // movie.Rating = (float)Double.Parse(sr["Rating"].ToString());
          #endregion

          #region mediapath
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
          #endregion

          #region imdb
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
          #endregion

          #region tmdb
          movie.TMDBNumber = row["TMDB_Id"].ToString();
          #endregion

          #region dateadded
          movie.DateAdded = row["Date"].ToString();
          DateTime wdate = new DateTime(1900, 01, 01);
          try
          {
            wdate = Convert.ToDateTime(row["Date"]);
            movie.DateTime = wdate;
          }
          catch { }
          #endregion

          #region Trakt Categories
          movie.CategoryTrakt.Clear();
          if (!string.IsNullOrEmpty(row["CategoryTrakt"].ToString()))
            movie.CategoryTrakt = row["CategoryTrakt"].ToString().Split(new Char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Distinct().ToList(); //row["CategoryTrakt"].ToString();
          #endregion

          #region Cover
          if (getDataRowDetailsForArtwork)
          {
            movie.Picture = row["Picture"].ToString();
          }
          #endregion
        }

        private static void GetMovieArtworkDetails(ref MFMovie movie) // DataRow row, 
        {
          //-----------------------------------------------------------------------------------------------------------------------
          //    Load Artwork Info
          //-----------------------------------------------------------------------------------------------------------------------
          MyFilmsGUI.Configuration tempconf = new MyFilmsGUI.Configuration(movie.Config, false, true, null);

          try
          {
            string pictureFile;
            if (movie.Picture.Length > 0)
            {
              if ((movie.Picture.IndexOf(":\\") == -1) && (movie.Picture.Substring(0, 2) != "\\\\"))
                pictureFile = tempconf.StrPathImg + "\\" + movie.Picture;
              else
                pictureFile = movie.Picture;
            }
            else
              pictureFile = string.Empty;
            if (string.IsNullOrEmpty(pictureFile) || !File.Exists(pictureFile))
              pictureFile = tempconf.DefaultCover;
            movie.Picture = pictureFile;
            
            string fanartTitle = "";
            if (tempconf.StrTitle1 == "FormattedTitle") // special setting for formatted title - we don't want to use it, as it is usually too long and causes problems with path length
            {
              if (!string.IsNullOrEmpty(movie.TranslatedTitle))
                fanartTitle = movie.TranslatedTitle;
              else if (!string.IsNullOrEmpty(movie.Title))
                fanartTitle = movie.Title;
              else
                fanartTitle = movie.FormattedTitle;
            }
            else
            {
              if (tempconf.StrTitle1 == "TranslatedTitle" && movie.TranslatedTitle.Length > 0)
                fanartTitle = movie.TranslatedTitle;
              else if (!string.IsNullOrEmpty(movie.Title))
                fanartTitle = movie.Title;
              else if (!string.IsNullOrEmpty(movie.FormattedTitle))
                fanartTitle = movie.FormattedTitle;
              else fanartTitle = "";
            }
            movie.Fanart = MyFilmsDetail.Search_Fanart(fanartTitle, false, "file", false, pictureFile, string.Empty, tempconf)[0];
          }
          catch (Exception ex)
          {
            LogMyFilms.DebugException("GetMovieArtworkDetails() - Error: " + ex.Message, ex);
          }
        }

        private static string GetUserRating(string EnhancedWatchedValue, string userprofilename)
        {
          if (EnhancedWatchedValue.Contains(userprofilename))
          {
            string[] split = EnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
              if (s.Contains(":"))
              {
                string tempuser = MyFilmsDetail.EnhancedWatchedValue(s, "username");
                //string tempcount = MyFilmsDetail.EnhancedWatchedValue(s, "count");
                string temprating = MyFilmsDetail.EnhancedWatchedValue(s, "rating");
                //string tempdatewatched = MyFilmsDetail.EnhancedWatchedValue(s, "datewatched");
                if (tempuser == userprofilename)
                {
                  return temprating;
                }
              }
            }
          }
          return "-1";
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
          return ReadDataMovies(StrDfltSelect, StrSelect, StrSort, StrSortSens, false, false);
        }
        public static DataRow[] ReadDataMovies(string StrDfltSelect, string StrSelect, string StrSort, string StrSortSens, bool all, bool doextrasort)
        {
          // LogMyFilms.Debug("ReadDataMovies() - StrDfltSelect            = '" + StrDfltSelect + "'");
          // LogMyFilms.Debug("ReadDataMovies() - StrSelect                = '" + StrSelect + "'");
          // LogMyFilms.Debug("ReadDataMovies() - StrSort/StrSortSens, all = '" + StrSort + "/" + StrSortSens + "', '" + all + "', cached = '" + (data != null) + "'");
          // LogMyFilms.Debug("ReadDataMovies() - Expression               = '" + StrDfltSelect + StrSelect + "|" + StrSort + " " + StrSortSens + "', all = '" + all + "', extrasort = '" + doextrasort + "', cached = '" + (data != null) + "'");
          LogMyFilms.Debug("ReadDataMovies() - Started with Expression = '" + StrDfltSelect + StrSelect + "|" + StrSort + " " + StrSortSens + "', all = '" + all + "', extrasort = '" + doextrasort + "', cached = '" + (data != null) + "'");
          var watchReadMovies = new Stopwatch(); watchReadMovies.Reset(); watchReadMovies.Start();

          if (StrSelect.Length == 0) StrSelect = MyFilms.conf.StrTitle1 + " not like ''";

          if (data == null) initData();

          _dataLock.EnterReadLock();
          try
          {
            // DB field replacements for sorting - currently only used for "Date" - in the future might be used for "DateFile" and "DateWatched" too ...
            switch (StrSort)
            {
              case "Date":
                StrSort = "DateAdded";
                LogMyFilms.Debug("ReadDataMovies() - Sort field replacement: Date -> DateAdded");
                break;
            }
            movies = data.Movie.Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens);
            if (movies.Length == 0 && all)
            {
              StrSelect = MyFilms.conf.StrTitle1 + " not like ''";
              LogMyFilms.Debug("ReadDataMovies() - Switching to full list ...");
              movies = data.Movie.Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens);
            }
            #region Additional sorting ...
            if (doextrasort)
            {
              Stopwatch watchReadMoviesSort = new Stopwatch(); watchReadMoviesSort.Reset(); watchReadMoviesSort.Start();
              MyFilms.FieldType fieldType = MyFilms.GetFieldType(StrSort);
              Type columnType = MyFilms.GetColumnType(StrSort);
              string strColumnType = (columnType == null) ? "<invalid>" : columnType.ToString();

              if (!string.IsNullOrEmpty(StrSort) && columnType == typeof(string)) // don't apply special sorting on "native" types - only on string types !
              {
                LogMyFilms.Debug("ReadDataMovies() - sorting fieldtype = '" + fieldType + "', vartype = '" + strColumnType + "', sortfield = '" + StrSortSens + "', sortascending = '" + StrSort + "'");
                watch.Reset(); watch.Start();
                switch (fieldType)
                {
                  case MyFilms.FieldType.AlphaNumeric:
                    if (StrSortSens == " ASC")
                    {
                      IComparer myComparer = new MyFilms.AlphanumComparatorFast();
                      Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(a[StrSort], b[StrSort]));
                    }
                    else
                    {
                      IComparer myComparer = new MyFilms.myReverserAlphanumComparatorFast();
                      Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(a[StrSort], b[StrSort]));
                      //r.Reverse();
                    }
                    break;
                  #region Date and Decimal types are never used, as we do additional sorting for alphanumeric (string) values only !
                  case MyFilms.FieldType.Date:
                    if (StrSortSens == " ASC")
                    {
                      IComparer myComparer = new MyFilms.myDateComparer();
                      Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(a[StrSort], b[StrSort]));
                    }
                    else
                    {
                      IComparer myComparer = new MyFilms.myDateReverseComparer();
                      Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(a[StrSort], b[StrSort]));
                      //IComparer myComparer = new myDateComparer();
                      //Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(b[StrSort], a[StrSort]));
                    }
                    break;
                  case MyFilms.FieldType.Decimal:
                    if (StrSortSens == " ASC")
                    {
                      IComparer myComparer = new MyFilms.myRatingComparer();
                      Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(a[StrSort], b[StrSort]));
                    }
                    else
                    {
                      IComparer myComparer = new MyFilms.myRatingComparer();
                      Array.Sort<DataRow>(movies, (a, b) => myComparer.Compare(b[StrSort], a[StrSort]));
                      //r.Reverse();
                    }
                    break;
                  #endregion
                }
                LogMyFilms.Debug("ReadDataMovies() - additional sorting finished (" + (watchReadMoviesSort.ElapsedMilliseconds) + " ms)");
              }
              else
              {
                LogMyFilms.Debug("ReadDataMovies() - additional sorting skipped - sorting fieldtype = '" + fieldType + "', vartype = '" + strColumnType + "', sortfield = '" + StrSortSens + "', sortascending = '" + StrSort + "'");
              }
            }
            #endregion
          }
          finally
          {
            _dataLock.ExitReadLock();
          }
          watchReadMovies.Stop();

          LogMyFilms.Debug("ReadDataMovies() - Finished ...  returning '" + movies.Length + "' movies (" + (watchReadMovies.ElapsedMilliseconds) + " ms)");
          return movies;
        }

        public static void LoadMyFilms(string StrFileXml)
        {
          if (!File.Exists(StrFileXml))
          {
            LogMyFilms.Error(string.Format("LoadMyFilms() - the DB file {0} does not exist !", StrFileXml));
            throw new Exception(string.Format("The DB file {0} does not exist !", StrFileXml));
          }
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

            return success;
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

              string Catalog = XmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
              //bool TraktEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
              //bool RecentAddedAPIEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowRecentAddedAPI", false);

              if (System.IO.File.Exists(Catalog))
              {
                int iCustomViews = XmlConfig.ReadXmlConfig("MyFilms", config, "AntViewTotalCount", -1);
                if (iCustomViews == -1) // Customviews not yet present ... ToDo: Remove in later Version - only "migration code"
                {
                  //viewList.Add(new KeyValuePair<string, string>("all", GUILocalizeStrings.Get(342)));
                  viewList.Add(new KeyValuePair<string, string>("Year", GUILocalizeStrings.Get(345)));
                  viewList.Add(new KeyValuePair<string, string>("Category", GUILocalizeStrings.Get(10798664)));
                  viewList.Add(new KeyValuePair<string, string>("Country", GUILocalizeStrings.Get(200026)));
                  viewList.Add(new KeyValuePair<string, string>("Actors", GUILocalizeStrings.Get(10798667)));
                }
                int index = 1;
                while (true) // for (int i = 1; i < iCustomViews + 1; i++)
                {
                  string viewName = "", viewDisplayName = "";
                  viewName = XmlConfig.ReadXmlConfig("MyFilms", config, string.Format("AntViewItem{0}", index), string.Empty);
                  viewDisplayName = XmlConfig.ReadXmlConfig("MyFilms", config, string.Format("AntViewText{0}", index), string.Empty);
                  if (string.IsNullOrEmpty(viewName)) break; // stop loading, if no View name is given

                  if (string.IsNullOrEmpty(viewDisplayName))
                    viewDisplayName = viewName;
                  viewList.Add(new KeyValuePair<string, string>(viewName, viewDisplayName));
                  index++;
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
        /// returns 'null' if no values can be evaluated - user should still be able to manually set a value for startparam
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
            //bool TraktEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
            //bool RecentAddedAPIEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowRecentAddedAPI", false);
            string StrDfltSelect = XmlConfig.ReadXmlConfig("MyFilms", config, "StrDfltSelect", string.Empty);
            string StrSelect = XmlConfig.ReadXmlConfig("MyFilms", config, "StrSelect", string.Empty);
            string StrTitle1 = XmlConfig.ReadXmlConfig("MyFilms", config, "AntTitle1", string.Empty);


            if (string.IsNullOrEmpty(StrSelect))
              StrSelect = StrTitle1 + " not like ''";

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
              DataRow[] results = dataExport.Tables["Movie"].Select(StrDfltSelect + StrSelect, view + " ASC");
              if (results.Length == 0) return null;

              foreach (DataRow enr in results)
              {
                try
                {
                  bool isdate = (view == "Date" || view == "DateAdded");
                  champselect = (isdate) ? string.Format("{0:yyyy/MM/dd}", enr["DateAdded"]) : enr[view].ToString().Trim();
                    
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
                if (string.Compare(champselect, wchampselect, StringComparison.OrdinalIgnoreCase) == 0) // Are the strings equal? Then add count!
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
        /// <param name="unwatchedOnly">only get unwatched episodes (only used with recent added type)</param>
        public static List<MFMovie> GetMostRecent(MostRecentType type, int days, int limit, bool unwatchedOnly)
        {
          string enumtype = Enum.GetName(typeof(MostRecentType), type);
          LogMyFilms.Debug("GetMostRecent() - Called with type = '" + enumtype + "', days = '" + days + "', limit = '" + limit + "', unwatchedonly = '" + unwatchedOnly + "'");

          // Create Time Span to lookup most recents
          DateTime dateCompare = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
          dateCompare = dateCompare.Subtract(new TimeSpan(days, 0, 0, 0, 0));
          //string date = dateCompare.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");

          // get all movies
          // ArrayList allmovies = GetMoviesGlobal("", "", false);
          List<MFMovie> movielist = (from MFMovie movie in GetMoviesGlobal("", "", false) select movie).ToList();
          
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

          // remove films without date and sort descending by dateadded
          movielist = movielist.Where(m => (m.DateAdded ?? string.Empty) != string.Empty).OrderByDescending(x => x.DateTime).ToList();
          LogMyFilms.Debug("GetMostRecent() - result of nonlimited movies: '" + movielist.Count + "'");

          movielist = movielist.Distinct(new DistinctItemComparer()).ToList();
          LogMyFilms.Debug("GetMostRecent() - result of nonlimited movies without dupes: '" + movielist.Count + "'");

          // now apply the result count limit
          movielist = movielist.Take(limit).ToList();

          // get artwork
          List<MFMovie> movielistwithartwork = new List<MFMovie>();
          foreach (MFMovie movie in movielist)
          {
            MFMovie tmpmovie = new MFMovie();
            tmpmovie = movie;
            GetMovieArtworkDetails(ref tmpmovie);
            movielistwithartwork.Add(tmpmovie);
          }
          
          if (movielistwithartwork.Count < 11) 
            foreach (MFMovie movie in movielistwithartwork)
            {
              LogMyFilms.Debug("GetMostRecent() - Returning (limited): config = '" + movie.Config + "', title = '" + movie.Title + "', translatedtitle = '" + movie.TranslatedTitle + "', watched = '" + movie.Watched + "', added = '" + movie.DateAdded + "', datetime = '" + movie.DateTime.ToShortDateString() + "', length = '" + movie.Length.ToString() + "', Category = '" + movie.Category + "', cover = '" + movie.Picture + "', fanart = '" + movie.Fanart + "'");
            }
          LogMyFilms.Debug("GetMostRecent() - Returning '" + movielistwithartwork.Count + "' movies.");
          return movielistwithartwork;
        }

        class DistinctItemComparer : IEqualityComparer<MFMovie>
        {
          public bool Equals(MFMovie x, MFMovie y)
          {
            return x.Title == y.Title && 
                x.File == y.File;
          }
          public int GetHashCode(MFMovie obj)
          {
            return obj.Title.GetHashCode() ^
                obj.File.GetHashCode();
          }
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
    private string _mStrFormattedTitle = string.Empty;
    private string _mStrGroupName = string.Empty;
    private List<string> _mStrCategoryTrakt = new List<string>();
    private string _mStrEdition = string.Empty;
    private string _mStrFile = string.Empty;
    private string _mStrTrailer = string.Empty;
    private string _mStrPath = string.Empty;
    private string _mStrIMDBNumber = string.Empty;
    private string _mStrTMDBNumber = string.Empty;
    private int _mIYear = 1900;
    private string _mStrCategory = string.Empty;
    private int _mILength = 0;
    private float _mFRating;
    private float _mFRatingUser;
    private bool _mIWatched;
    private int _mIWatchedCount = -1;
    private DateTime _mDateTime = System.DateTime.Today;
    private string _mDateAdded = string.Empty;
    private string _mPicture = string.Empty;
    private string _mFanart = string.Empty;
    private string _mConfig = string.Empty;
    private string _mUsername = string.Empty;
    private bool _mReadOnly = false;
    //private bool _mAllowTrakt = false;
    //private bool _mAllowLatestMediaAPI = false;
    // private DataRow _MovieRow = null;
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

    public string FormattedTitle
    {
      get { return _mStrFormattedTitle; }
      set { _mStrFormattedTitle = value; }
    }

    public string GroupName
    {
      get { return _mStrGroupName; }
      set { _mStrGroupName = value; }
    }

    public string Edition
    {
      get { return _mStrEdition; }
      set { _mStrEdition = value; }
    }

    public string File
    {
      get { return _mStrFile; }
      set { _mStrFile = value; }
    }

    public string Trailer
    {
      get { return _mStrTrailer; }
      set { _mStrTrailer = value; }
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
    /// entries for watchlist, recommendations and user lists.
    /// </summary>
    public List<string> CategoryTrakt
    {
      get { return _mStrCategoryTrakt; }
      set { _mStrCategoryTrakt = value; }
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

    public float RatingUser
    {
      get { return _mFRatingUser; }
      set { _mFRatingUser = value; }
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

    //public bool AllowTrakt
    //{
    //  get { return _mAllowTrakt; }
    //  set { _mAllowTrakt = value; }
    //}

    //public bool AllowLatestMediaAPI
    //{
    //  get { return _mAllowLatestMediaAPI; }
    //  set { _mAllowLatestMediaAPI = value; }
    //}
    //public DataRow MovieRow
    //{
    //  get { return _MovieRow; }
    //  set { _MovieRow = value; }
    //}
    #endregion

    public void Reset()
    {
      _mStrTitle = string.Empty;
      _mStrTranslatedTitle = string.Empty;
      _mStrFormattedTitle = string.Empty;
      _mStrGroupName = string.Empty;
      _mStrCategoryTrakt.Clear();
      _mStrEdition = string.Empty;
      _mStrIMDBNumber = string.Empty;
      _mStrTMDBNumber = string.Empty;
      _mIYear = 1900;
      _mStrCategory = string.Empty;
      _mILength = 0;
      _mFRating = 0.0f;
      _mFRatingUser = 0.0f;
      _mIWatched = false;
      _mIWatchedCount = -1;
      _mDateTime = DateTime.Today;
      _mDateAdded = string.Empty;
      _mStrFile = string.Empty;
      _mStrTrailer = string.Empty;
      _mStrPath = string.Empty;
      _mPicture = string.Empty;
      _mFanart = string.Empty;
      _mConfig = string.Empty;
      _mUsername = string.Empty;
      _mReadOnly = false;
      //_mAllowTrakt = false;
      //_mAllowLatestMediaAPI = false;
      // _MovieRow = null;
    }

    public void AddCategoryTrakt(string toAdd)
    {
      _mStrCategoryTrakt.Add(toAdd);
    }

    public void RemoveCategoryTrakt(string toRemove)
    {
      _mStrCategoryTrakt.Remove(toRemove);
    }

    public void Commit()
    {
      DoCommit(); 
      //new System.Threading.Thread(delegate()
      //{
      //  try
      //  {
      //    DoCommit();
      //  }
      //  catch (Exception)
      //  {
          
      //  }
      //  finally
      //  {
          
      //  }
      //  GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
      //  {
      //    // this will be executed after background thread finished
      //    return 0;
      //  }, 0, 0, null);
      //}) { Name = "API-Commit", IsBackground = true }.Start();
      //return;
    }

    private void DoCommit()
    {
      // AntMovieCatalog dataImport = new AntMovieCatalog();
      using (AntMovieCatalog dataImport = new AntMovieCatalog())
      {
        using (XmlSettings XmlConfig = new XmlSettings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MyFilms.xml")))
        {
          #region variables
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
          IEnumerable<DataColumn> commonColumns = dataImport.Movie.Columns.OfType<DataColumn>().Intersect(dataImport.CustomFields.Columns.OfType<DataColumn>(), new BaseMesFilms.DataColumnComparer());
          #endregion

          #region sanity checks
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
          LogMyFilms.Debug("Commit() : Update requested for Number = '" + _mID + "', Movie = '" + _mStrTitle + "' (" + _mIYear + "), IMDB = '" + _mStrIMDBNumber + "', Watched = '" + _mIWatched + "', Rating = '" + _mFRating + "', RatingUser = '" + _mFRatingUser + "', CategoryTrakt = '" + GetStringValue(_mStrCategoryTrakt) + "'");

          if (!System.IO.File.Exists(Catalog)) return;

          if (!TraktEnabled)
          {
            LogMyFilms.Debug("Trakt not enabled for this config - Update rejected ! - Movie = '" + _mStrTitle + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");
            return;
          }
          #endregion

          #region save changes via memory update to disk
          int maxretries = 10; // max retries 10 * 1000 = 10 seconds
          int i = 0;
          bool success = false; // result of update operation

          while (!success && i < maxretries)
          {
            // first check, if there is a global manual lock
            if (!MyFilmsDetail.GlobalLockIsActive(Catalog))
            {
              MyFilmsDetail.SetGlobalLock(true, Catalog);
              if (BaseMesFilms._dataLock.TryEnterWriteLock(10000))
              {
                try
                {
                  #region read xml file from disk to memory
                  dataImport.Clear();
                  using (FileStream fs = new FileStream(Catalog, FileMode.Open, FileAccess.Read, FileShare.Read))
                  {
                    //LogMyFilms.Debug("Commit() - opening '" + Catalog + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");
                    foreach (DataTable dataTable in dataImport.Tables) dataTable.BeginLoadData();
                    dataImport.ReadXml(fs);
                    foreach (DataTable dataTable in dataImport.Tables) dataTable.EndLoadData();
                    fs.Close();
                    //LogMyFilms.Debug("Commit()- closing  '" + Catalog + "' FileStream");
                  }

                  DataRow[] results = dataImport.Movie.Select(StrDfltSelect + "Number" + " = " + "'" + _mID + "'", "OriginalTitle" + " " + "ASC"); // if (results.Length != 1) continue;
                  //AntMovieCatalog.MovieRow[] results = dataImport.Movie.Where(m => m.Number == _mID).ToList();
                  #endregion

                  if (results.Length != 1) LogMyFilms.Warn("Commit() - Warning - Results found: '" + results.Length + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");

                  #region update all movie records in memory
                  foreach (AntMovieCatalog.MovieRow sr in results)
                  {
                    // bool updateRequired = false;

                    #region Copy CustomFields data ....
                    AntMovieCatalog.CustomFieldsRow customFields = null;
                    if (sr.GetCustomFieldsRows().Length > 0)
                    {
                      customFields = sr.GetCustomFieldsRows()[0]; // Relations["Movie_CustomFields"]

                      foreach (DataColumn dc in commonColumns)
                      {
                        object temp;
                        if (dc.ColumnName != "Movie_Id" && DBNull.Value != (temp = customFields[dc.ColumnName])) sr[dc.ColumnName] = temp;
                      }
                    }
                    else // create CustomFields Element, if not existing ...
                    {
                      customFields = dataImport.CustomFields.NewCustomFieldsRow();
                      customFields.SetParentRow(sr);
                      dataImport.CustomFields.AddCustomFieldsRow(customFields);
                    }
                    #endregion

                    #region CategoryTrakt
                    if (sr.IsCategoryTraktNull() || sr.CategoryTrakt != GetStringValue(_mStrCategoryTrakt))
                    {
                      // updateRequired = true;
                      LogMyFilms.Debug("Commit() - Updating Field 'CategoryTrakt' from '" + ((!sr.IsCategoryTraktNull()) ? sr.CategoryTrakt : "") + "' to '" + GetStringValue(_mStrCategoryTrakt) + "'");
                      sr.CategoryTrakt = GetStringValue(_mStrCategoryTrakt);
                    }
                    #endregion

                    #region site rating
                    if (_mFRating > 0) sr.Rating = (decimal)_mFRating;
                    #endregion

                    #region watched status
                    string oldWatchedString = sr[WatchedField].ToString();
                    if (!EnhancedWatchedStatusHandling)
                    {
                      // watched
                      if (_mIWatched)
                        sr[WatchedField] = "true";
                      else
                        sr[WatchedField] = GlobalUnwatchedOnlyValue;
                      // user rating
                      if (_mFRatingUser > 0) sr.RatingUser = (decimal)_mFRatingUser;
                    }
                    else
                    {
                      string oldEnhancedWatchedValue = sr[WatchedField].ToString();
                      string newEnhancedWatchedValue = "";

                      // watched
                      if (!string.IsNullOrEmpty(_mUsername))
                        newEnhancedWatchedValue = NewEnhancedWatchValue(oldEnhancedWatchedValue, _mUsername, _mIWatched, _mIWatchedCount, _mFRatingUser);
                      else
                        newEnhancedWatchedValue = NewEnhancedWatchValue(oldEnhancedWatchedValue, UserProfileName, _mIWatched, _mIWatchedCount, _mFRatingUser);
                      sr[WatchedField] = newEnhancedWatchedValue;
                      // "commmon" user rating
                      if (_mFRatingUser > 0) sr.RatingUser = (decimal)_mFRatingUser;
                    }
                    if (sr[WatchedField].ToString().ToLower() != oldWatchedString.ToLower())
                      LogMyFilms.Debug("Commit() - Updating Field '" + WatchedField + "' from '" + oldWatchedString + "' to '" + sr[WatchedField] + "', WatchedCount = '" + _mIWatchedCount + "', (user)Rating = '" + _mFRatingUser + "', (site)Rating = '" + _mFRating + "'");
                    #endregion

                    #region imdb number
                    string oldIMDB = (sr.IsIMDB_IdNull()) ? "" : sr.IMDB_Id;
                    if (!string.IsNullOrEmpty(_mStrIMDBNumber))
                      sr.IMDB_Id = _mStrIMDBNumber;
                    if (sr.IMDB_Id != oldIMDB)
                      LogMyFilms.Debug("Commit() - Updating 'IMDB_Id' from '" + oldIMDB + "' to '" + sr.IMDB_Id + "'");
                    #endregion

                    #region copy data to customfields ...
                    foreach (DataColumn dc in commonColumns)
                    {
                      object temp;
                      if (dc.ColumnName != "Movie_Id" && DBNull.Value != (temp = sr[dc.ColumnName]))
                      {
                        customFields[dc.ColumnName] = temp;
                      }
                    }
                    #endregion
                  }
                  #endregion

                  #region (re)enable filesystem watcher to notify myfilms on update and disable trakt message handler
                  MyFilms.SendTraktUpdateMessage = false;
                  try
                  {
                    if (MyFilms.FSwatcher.Path.Length > 0) MyFilms.FSwatcher.EnableRaisingEvents = true; // re enable watcher - as myfilms should auto update dataset for current config, if update is done from trakt 
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.DebugException("Commit()- FSwatcher - problem enabling Raisingevents - Message: '" + ex.Message + "'", ex);
                  }
                  #endregion

                  #region write xml file
                  using (FileStream fsTmp = System.IO.File.Create(Catalog.Replace(".xml", ".tmp"), 1000, FileOptions.DeleteOnClose)) // make sure, only one process is writing to file !
                  {
                    using (FileStream fs = new FileStream(Catalog, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) // lock the file for any other use, as we do write to it now !
                    {
                      // LogMyFilms.Debug("Commit() - opening '" + Catalog + "' as FileStream with FileMode.OpenOrCreate, FileAccess.Write, FileShare.None");
                      fs.SetLength(0); // do not append, owerwrite !

                      using (XmlTextWriter MyXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.Default))
                      {
                        LogMyFilms.Debug("Commit() - writing '" + Catalog + "' as MyXmlTextWriter in FileStream");
                        MyXmlTextWriter.Formatting = System.Xml.Formatting.Indented;
                        MyXmlTextWriter.WriteStartDocument();
                        dataImport.WriteXml(MyXmlTextWriter, XmlWriteMode.IgnoreSchema);
                        MyXmlTextWriter.Flush();
                        MyXmlTextWriter.Close();
                      }
                      fs.Close(); // write buffer and release lock on file (either Flush, Dispose or Close is required)
                      // LogMyFilms.Debug("Commit() - closing '" + Catalog + "' FileStream and releasing file lock");
                      success = true;
                    }
                  }
                  Thread.Sleep(50); // give time to release file handle
                  #endregion
                }
                catch (Exception ex)
                {
                  LogMyFilms.Debug("Commit() - failed saving data to disk - Catalog = '" + Catalog + "' - reason: " + ex.Message);
                  success = false;
                }
                finally
                {
                  dataImport.Clear();
                  MyFilmsDetail.SetGlobalLock(false, Catalog);
                  BaseMesFilms._dataLock.ExitWriteLock();
                }
              }
              else
              {
                LogMyFilms.Debug("Commit() - timeout when waiting for slim writelock - could not write data !");
              }
            }
            if (success)
            {
              continue;
            }
            i += 1;
            LogMyFilms.Info("Commit() - Movie Database locked on try '" + i + " of " + maxretries + "' to write, waiting for next retry");
            Thread.Sleep(1000);
          }
          if (!success) LogMyFilms.Error("Commit() - Error writing Movie Database after '" + maxretries + "' retries !");
          #endregion
          MyFilmsDetail.SetGlobalLock(false, Catalog);
          Thread.Sleep(2000); // wait 2 seconds to give other clients a chance to write their data ...
        }
        dataImport.Clear(); // if (dataImport != null) dataImport.Dispose();
      }
    }
    private string NewEnhancedWatchValue(string EnhancedWatchedValue, string UserProfileName, bool watched, int count, float rating)
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
              sNew = tempuser + ":" + count + ":" + rating + ":" + tempdatewatched;
            }
            if (tempuser == UserProfileName) // Update Count Value for selected user
            {
              sNew = tempuser + ":" + count + ":" + rating + ":" + tempdatewatched;
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
          newEnhancedWatchedValue = "Global:" + count + ":" + rating + ":|" + UserProfileName + ":" + count + ":" + rating + ":";
        else
          newEnhancedWatchedValue = EnhancedWatchedValue + "|" + UserProfileName + ":" + count + ":" + rating + ":";
      }
      return newEnhancedWatchedValue;
    }

    private string Remove(string input, string toRemove)
    {
      string output = "";
      string[] split = input.Split(new Char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
      List<string> itemList = split.Distinct().ToList();
      if (itemList.Contains(toRemove)) itemList.Remove(toRemove);
      foreach (string s in itemList)
      {
        if (output.Length > 0) output += ", ";
        output += s;
      }
      return output;
    }

    private string Add(string input, string toAdd)
    {
      string output = "";
      string[] split = input.Split(new Char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
      List<string> itemList = split.Distinct().ToList();
      if (!itemList.Contains(toAdd)) itemList.Add(toAdd);
      foreach (string s in itemList)
      {
        if (output.Length > 0) output += ", ";
        output += s;
      }
      return output;
    }

    private string GetStringValue(List<string> input)
    {
      string output = "";
      List<string> itemList = input.Select(x => x.Trim()).Where(x => x.Length > 0).Distinct().ToList();
      itemList.Sort();
      foreach (string s in itemList)
      {
        if (output.Length > 0) output += ", ";
        output += s;
      }
      return output;
    }

  }

  public static class DBNullableExtensions
  {
    public static object ToDBValue<T>(this Nullable<T> value) where T : struct
    {
      return value.HasValue ? (object)value.Value : DBNull.Value;
    }
  }
}
