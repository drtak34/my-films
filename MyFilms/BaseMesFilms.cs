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
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
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
        private static MyFilmsData mfdata;

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
        private static DataRow[] movies;
        private static DataRow[] persons;
        private static Stopwatch watch = new Stopwatch();


        #region ctor
        static BaseMesFilms() {}
        #endregion

        #region méthodes statique sprivées
        private static void initData()
        {
          watch.Reset();watch.Start();
          _dataLock.EnterReadLock();
          data = new AntMovieCatalog();
          string catalogfile = MyFilms.conf.StrFileXml;
          LogMyFilms.Debug("initData() - Start reading catalogfile '" + catalogfile + "'");
          try
            {
              using (FileStream fs = new FileStream(catalogfile, FileMode.Open, FileAccess.Read, FileShare.Read))
              {
                LogMyFilms.Debug("initData()- opening '" + catalogfile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");
                foreach (DataTable dataTable in data.Tables) dataTable.BeginLoadData();
                data.ReadXml(fs);
                foreach (DataTable dataTable in data.Tables) dataTable.EndLoadData();
                fs.Close();
                LogMyFilms.Debug("initData()- closing  '" + catalogfile + "' FileStream");
              }
            }
          catch (Exception e)
            {
              LogMyFilms.Error("initData() : Error reading xml database after " + data.Movie.Count + " records; error : " + e.Message + ", " + e.StackTrace);
              throw e;
            }
          finally
            {
              _dataLock.ExitReadLock();
            }
          watch.Stop();
          LogMyFilms.Debug("initData() - End reading catalogfile '" + catalogfile + "' (" + (watch.ElapsedMilliseconds) + " ms)");
          LogMyFilms.Debug("initData() - CalcDays Started ...");
          watch.Reset(); watch.Start();
          foreach (AntMovieCatalog.MovieRow movieRow in data.Movie)
          {
            if (movieRow.IsAgeAddedNull())
            {
              if (!movieRow.IsDateAddedNull())
              {
                movieRow.AgeAdded_Num = (int)DateTime.Now.Subtract(movieRow.DateAdded).TotalDays;
                movieRow.AgeAdded = ((int)DateTime.Now.Subtract(movieRow.DateAdded).TotalDays).ToString();
              }

              else
              {
                movieRow.AgeAdded_Num = 9999;
                movieRow.AgeAdded = "9999";
              }
            }
          }
          watch.Stop();
          LogMyFilms.Debug("initData() - CalcDays Finished ... (" + (watch.ElapsedMilliseconds) + " ms)");
        }

        private static void initDataMyFilms()
        {
          string datafile = GetNameForMyFilmsDatafile(MyFilms.conf.StrFileXml);

          mfdata = new MyFilmsData();
          LogMyFilms.Debug("MyFilmsData - Try reading datafile '" + datafile + "'");
          try
          {
            if (!System.IO.File.Exists(datafile)) 
              CreateEmptyDataFile(datafile);

            using (FileStream fs = new FileStream(datafile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              LogMyFilms.Debug("initData()- opening '" + datafile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");

              foreach (DataTable dataTable in mfdata.Tables)
                dataTable.BeginLoadData();
              mfdata.ReadXml(fs);
              foreach (DataTable dataTable in mfdata.Tables)
                dataTable.EndLoadData();

              foreach (DataTable dataTable in data.Tables)
                dataTable.BeginLoadData();
              data.ReadXml(fs);
              foreach (DataTable dataTable in data.Tables)
                dataTable.EndLoadData(); 

              // ... change data here, if intended ...
              // fs.SetLength(0);
              // data.WriteXml(fs);
              fs.Close();
              LogMyFilms.Debug("initData()- closing  '" + datafile + "' FileStream");
            }
          }
          catch (Exception e)
          {
            LogMyFilms.Error(": Error reading myfilms data xml '" + datafile + "'; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
            // throw e; // do NOT throw the exception to upper level
          }
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
        #endregion

        #region accesseurs
        public static DataRow[] FilmsSelected
        {
          get { return movies; }
        }
        public static DataRow[] PersonsSelected
        {
          get { return persons; }
        }
        #endregion

        #region méthodes statique publiques

        public static DataRow[] ReadDataMovies(string StrDfltSelect, string StrSelect, string StrSort, string StrSortSens)
        {
            return ReadDataMovies(StrDfltSelect, StrSelect, StrSort, StrSortSens, false);
        }
        public static DataRow[] ReadDataMovies(string StrDfltSelect, string StrSelect, string StrSort, string StrSortSens, bool all)
        {
          LogMyFilms.Debug("ReadDataMovies() - Starting ... (StrDfltSelect = '" + StrDfltSelect + "', StrSelect = '" + StrSelect + "', StrSort = '" + StrSort + "', StrSortSens = '" + StrSortSens + "', RESULTING DS SELECT = '" + StrDfltSelect + StrSelect + ", " + StrSort + " " + StrSortSens + "')");
          watch.Reset();watch.Start();
          if (data == null) 
            initData();
          else 
            LogMyFilms.Debug("ReadDataMovies() - Data already cached in memory !");
          if (StrSelect.Length == 0) StrSelect = MyFilms.conf.StrTitle1 + " not like ''";

          movies = data.Movie.Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens);
          if (movies.Length == 0 && all)
          {
            StrSelect = MyFilms.conf.StrTitle1 + " not like ''";
            LogMyFilms.Debug("ReadDataMovies() - Switching to full list ...");
            movies = data.Movie.Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens);
          }
          watch.Stop();
          LogMyFilms.Debug("ReadDataMovies() - Finished ... (" + (watch.ElapsedMilliseconds) + " ms)");

          return movies;

          #region testing join and view

          //var moviesquery = from movie in data.Movie
          //              join extendedfields in data.CustomFields on movie.Movie_Id equals extendedfields.Movie_Id into gj
          //              from subpet in gj.DefaultIfEmpty()
          //              select new { movie.OriginalTitle, PetName = (subpet == null ? String.Empty : subpet.CustomField1) };
          
          //var q = from c in data.Movie
          //  join o in data.CustomFields on c.Movie_Id equals o.Movie_Id into g
          //  select new {movie = c.OriginalTitle, Extendedfields = g};
 

          var queryProducer =
              from movie in data.Movie
              group movie by movie.Producer into groupProducer
              orderby groupProducer.Key
              select groupProducer;

          IGrouping<System.Reflection.MemberTypes, System.Reflection.MemberInfo> group =
                          typeof(String).GetMembers().
                          GroupBy(member => member.MemberType).
                          First();



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

          var query = from t1 in data.Movie.AsEnumerable()
                      join t2 in data.CustomField.AsEnumerable()
                        // <int> == <int?> für Null Vergleich
                      on t1.Field<int>("id") equals t2.Field<int?>("idTabelle1")
                      select new
                      {
                        t1id = t1.Field<int>("id"),
                        t2id = t2.Field<int>("id"),
                        t1daten = t1.Field<string>("Daten"),
                        t2daten = t2.Field<string>("Daten"),
                      };
          foreach (var row in query)
          {
            Console.WriteLine("{0}\t{1}\t{2:d}\t{3}",
            row.t1id, row.t2id, row.t1daten, row.t2daten);
          }

          var abfrage = from emp in data.Movie
                        join con in data.CustomField on emp.Movie_Id
                        equals con.CustomField_Id into ec
                        from subEmp in ec.DefaultIfEmpty()
                        select new
                        {
                          emp.Movie_Id,
                          FirstName =
                            (subEmp == null ? String.Empty : subEmp.Name)
                        };

          var orders = data.Tables["SalesOrderHeader"].AsEnumerable();
          var details = data.Tables["SalesOrderDetail"].AsEnumerable();

          var query0 =
              from order in orders
              join detail in details
              on order.Field<int>("SalesOrderID")
              equals detail.Field<int>("SalesOrderID") into ords
              select new
              {
                CustomerID =
                    order.Field<int>("SalesOrderID"),
                ords = ords.Count()
              };

          foreach (var order in query)
          {
            // Console.WriteLine("CustomerID: {0}  Orders Count: {1}", order.CustomerID, order.ords);
          }



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
        }

        public static DataRow[] ReadDataPersons(string StrSelect, string StrSort, string StrSortSens)
        {
          if (data == null) initData();
          if (StrSelect.Length == 0) StrSelect = "Name" + " not like ''";
          persons = data.Person.Select(StrSelect, StrSort + " " + StrSortSens);
          return persons;
        }

        public static void LoadMyFilms(string StrFileXml)
        {
          if (!System.IO.File.Exists(StrFileXml)) throw new Exception(string.Format("The file {0} does not exist !.", StrFileXml));
          // return, if readlock already present
          LogMyFilms.Debug("LoadMyFilms()- Current Readlocks: '" + _dataLock.CurrentReadCount + "'");
          //if (_dataLock.CurrentReadCount > 0) // might be opened by API as well, so count can be 2+
          //  return;
          watch.Reset();watch.Start();
          _dataLock.EnterReadLock();
          data = new AntMovieCatalog();
          string catalogfile = StrFileXml;
          try
          {
            //success = LoadMyFilmsFromDisk(StrFileXml);
            using (FileStream fs = new FileStream(catalogfile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              LogMyFilms.Debug("LoadMyFilms()- opening '" + catalogfile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");
              foreach (DataTable dataTable in data.Tables) dataTable.BeginLoadData();
              data.ReadXml(fs);
              foreach (DataTable dataTable in data.Tables) dataTable.EndLoadData(); 
              fs.Close();
              LogMyFilms.Debug("LoadMyFilms()- closing  '" + catalogfile + "' FileStream");
            }
          }
          catch (Exception e)
          {
            LogMyFilms.Error("LoadMyFilms() : Error reading xml database after " + data.Movie.Count + " records; error : " + e.Message + ", " + e.StackTrace);
            LogMyFilms.Error("LoadMyFilms() : Last Record: '" + data.Movie[data.Movie.Count - 1].Number + "', title: '" + data.Movie[data.Movie.Count - 1].OriginalTitle + "'");
            throw new Exception("Error reading xml database after " + data.Movie.Count + " records; movie: '" + data.Movie[data.Movie.Count - 1].OriginalTitle + "'; error : " + e.Message);
          }
          finally
          {
            _dataLock.ExitReadLock();
          }
          watch.Stop();
          LogMyFilms.Debug("LoadMyFilms()- Finished  (" + (watch.ElapsedMilliseconds) + " ms)");
          LogMyFilms.Debug("LoadMyFilms() - CalcDays Started ...");
          watch.Reset(); watch.Start();
          foreach (AntMovieCatalog.MovieRow movieRow in data.Movie)
          {
            if (movieRow.IsAgeAddedNull())
            {
              if (!movieRow.IsDateAddedNull())
              {
                movieRow.AgeAdded_Num = (int)DateTime.Now.Subtract(movieRow.DateAdded).TotalDays;
                movieRow.AgeAdded = ((int)DateTime.Now.Subtract(movieRow.DateAdded).TotalDays).ToString();
              }
              else
              {
                movieRow.AgeAdded_Num = 9999;
                movieRow.AgeAdded = "9999";
              }
            }
          }
          watch.Stop();
          LogMyFilms.Debug("LoadMyFilms() - CalcDays Finished ... (" + (watch.ElapsedMilliseconds) + " ms)");
        }

        public static void LoadMyFilmsData(string datafile)
        {
          if (!System.IO.File.Exists(datafile))
          {
            LogMyFilms.Debug("The file {0} does not exist !.", datafile);
            // throw new Exception(string.Format("The file {0} does not exist !.", datafile)); // do NOT throw an exception to upper level
          }
          mfdata = new MyFilmsData();
          try
          {
            using (FileStream fs = new FileStream(datafile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              LogMyFilms.Debug("LoadMyFilmsData()- opening '" + datafile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");

              foreach (DataTable dataTable in mfdata.Tables)
                dataTable.BeginLoadData();

              mfdata.ReadXml(fs);

              foreach (DataTable dataTable in mfdata.Tables)
                dataTable.EndLoadData();              

              fs.Close();
              LogMyFilms.Debug("LoadMyFilmsData()- closing  '" + datafile + "' FileStream");
            }
          }
          catch (Exception e)
          {
            LogMyFilms.Error("LoadMyFilmsData() - Error reading myfilms data xml database '" + datafile + "'; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
          }
        }

        public static void UnloadMyFilms()
        {
          if (data != null)
            data.Dispose();

          if (mfdata != null)
            mfdata.Dispose();
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

            if (success)
            {
              //try
              //{
              //  SaveMyFilmsData(); // try to save extended data too
              //}
              //catch (Exception)
              //{
              //  LogMyFilms.Info("SaveMyFilms() - Saving MyFilmsData unsuccessful !");
              //}
              return true; // write successful!
            }
            else
            {
              //MediaPortal.Dialogs.GUIDialogOK dlgOk = (MediaPortal.Dialogs.GUIDialogOK)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_OK);
              //dlgOk.SetHeading("Error");//my videos
              //dlgOk.SetLine(1, "Error during updating the XML database '" + catalogfile + "' !");
              //dlgOk.SetLine(2, "Maybe Directory full or no write access.");
              //dlgOk.DoModal(MyFilms.ID_MyFilmsDetail);
              return false;
            }
          }
          else
          {
            LogMyFilms.Info("SaveMyFilms() - Movie Database could not get slim writelock for '" + timeout + "' ms - returning 'false'");
            return false;
          }
        }

        public static bool SaveMyFilmsData()
        {
          if (mfdata != null)
          {
            string datafile = GetNameForMyFilmsDatafile(MyFilms.conf.StrFileXml);
            try
            {
              using (FileStream fs = new FileStream(datafile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) // lock the fole for any other use, as we do write to it now !
              {
                LogMyFilms.Debug("SaveMyFilms()- opening '" + datafile + "' as FileStream with FileMode.Open, FileAccess.Write, FileShare.None");
                // data.ReadXml(fs); data alreadry in memory, only to be saved

                fs.SetLength(0); // do not append, owerwrite !

                using (XmlTextWriter MyXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.Default))
                {
                  LogMyFilms.Debug("SaveMyFilms()- writing '" + datafile + "' as MyXmlTextWriter in FileStream");
                  MyXmlTextWriter.Formatting = System.Xml.Formatting.Indented;
                  MyXmlTextWriter.WriteStartDocument();
                  mfdata.WriteXml(MyXmlTextWriter, XmlWriteMode.IgnoreSchema);
                  MyXmlTextWriter.Flush();
                  MyXmlTextWriter.Close();
                }
                // mfdata.WriteXml(fs);
                fs.Flush();
                fs.Close();
                LogMyFilms.Debug("SaveMyFilms()- closing '" + datafile + "' FileStream and releasing file lock");
              }

              //System.Xml.XmlTextWriter MyXmlTextWriter = new System.Xml.XmlTextWriter(datafile, System.Text.Encoding.Default);
              //MyXmlTextWriter.Formatting = System.Xml.Formatting.Indented; // Added by Guzzi to get properly formatted output XML
              //MyXmlTextWriter.WriteStartDocument();
              //mfdata.WriteXml(MyXmlTextWriter, XmlWriteMode.IgnoreSchema);
              //MyXmlTextWriter.Close();
            }
            catch (Exception ex)
            {
              LogMyFilms.DebugException("SaveMyFilms()- error writing '" + datafile + "', exception: " + ex.Message, ex);
              return false;
            }
            // data.WriteXmlSchema(@"c:\myfilms.xsd"); // this writes XML schema infos to disk
          }
          return true; // write successful!
        }

        public static bool SaveMyFilmsToDisk(string catalogfile)
        {
          watch.Reset();watch.Start();
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

        private bool LoadMyFilmsFromDisk(string catalogfile)
        {
          bool success = false; // result of write operation
          int maxretries = 10; // max retries 10 * 1000 = 10 seconds
          int i = 0;

          while (!success && i < maxretries)
          {
              try
              {
                using (FileStream fs = new FileStream(catalogfile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                  LogMyFilms.Debug("LoadMyFilms()- opening '" + catalogfile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");

                  foreach (DataTable dataTable in data.Tables)
                    dataTable.BeginLoadData();

                  data.ReadXml(fs);

                  foreach (DataTable dataTable in data.Tables)
                    dataTable.EndLoadData();

                  fs.Close();
                  LogMyFilms.Debug("LoadMyFilms()- closing  '" + catalogfile + "' FileStream");
                }
                success = true;
              }
              catch (Exception ex)
              {
                LogMyFilms.DebugException("LoadMyFilmsFromDisk()- error reading '" + catalogfile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read", ex);
                // LogMyFilms.Debug("Commit()- exception while trying to save data in '" + catalogfile + "' - exception: " + saveexeption.Message + ", stacktrace: " + saveexeption.StackTrace);
                success = false;
                throw ex;
              }
          }

          return success;
        }

        public static void CancelMyFilms()
        {
          LogMyFilms.Debug("CancelMyFilms() - disposing data ...");
          if (data != null)
            data.Clear();
          if (mfdata != null)
            mfdata.Clear();
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
          LogMyFilms.Debug("GetMostRecent() - Returning (nonlimited) movies: '" + movielist.Count + "'");

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
          foreach (MFMovie movie in movielistwithartwork)
          {
            LogMyFilms.Debug("GetMostRecent() - Returning (limited): config = '" + movie.Config + "', title = '" + movie.Title + "', watched = '" + movie.Watched + "', added = '" + movie.DateAdded + "', datetime = '" + movie.DateTime.ToShortDateString() + "', length = '" + movie.Length.ToString() + "', Category = '" + movie.Category + "', cover = '" + movie.Picture + "', fanart = '" + movie.Fanart + "'");
          }
          return movielistwithartwork;
        }
        #endregion

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
              MyFilmsGUI.Configuration tmpconf = new MyFilmsGUI.Configuration(config, true, null);
              if (tmpconf.AllowTraktSync || tmpconf.AllowRecentlyAddedAPI)
                LogMyFilms.Debug("GetMovies() - Trakt = '" + tmpconf.AllowTraktSync + "', RecentMedia = '" + tmpconf.AllowRecentlyAddedAPI + "', CatalogType = '" + tmpconf.StrFileType + "', Config = '" + config + "', Catalogfile = '" + tmpconf.StrFileXml + "'");
              else
                LogMyFilms.Debug("GetMovies() - Trakt = '" + tmpconf.AllowTraktSync + "', RecentMedia = '" + tmpconf.AllowRecentlyAddedAPI + "', CatalogType = '" + tmpconf.StrFileType + "', Config = '" + config + "'");

              if (File.Exists(tmpconf.StrFileXml) && (tmpconf.AllowTraktSync || (!traktOnly && tmpconf.AllowRecentlyAddedAPI)))
              {
                _dataLock.EnterReadLock();
                try
                {
                  dataExport.ReadXml(tmpconf.StrFileXml);
                }
                catch (Exception e)
                {
                  LogMyFilms.Error(": Error reading xml database after " + dataExport.Movie.Count.ToString() + " records; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
                  // throw e; // we should NOT throw the exception, otherwilse MP crashes due to unhandled exception
                }
                finally
                {
                  _dataLock.ExitReadLock();
                }

                try
                {
                  string sqlExpression = "";
                  string sqlSort = "";

                  if (!string.IsNullOrEmpty(Expression)) sqlExpression = Expression;
                  else sqlExpression = tmpconf.StrDfltSelect + tmpconf.StrTitle1 + " not like ''";

                  if (!string.IsNullOrEmpty(Sort)) sqlSort = Sort;
                  else sqlSort = "OriginalTitle" + " " + "ASC";

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
                        // GetMovieArtworkDetails(sr, tmpconf, ref movie); // movied to LM API as it's a lot of I/O ...
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
          movie.Length = (int)row["Length_Num"];

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

          if (!string.IsNullOrEmpty(row["URL"].ToString()) && string.IsNullOrEmpty(IMDB))
          {
            string CleanString = row["URL"].ToString();
            Regex CutText = new Regex("" + @"tt\d{7}" + "");
            Match m = CutText.Match(CleanString);
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
              wzone = string.Empty;
            }
          }
          return wtab;
        }

        public static string Translate_Column(string Column)
        {
            //string s = Column;
            //// Check for empty string.
            //if (string.IsNullOrEmpty(s))
            //{
            //    return string.Empty;
            //}
            //// Return char and concat substring.
            //Column = char.ToUpper(s[0]) + s.Substring(1);

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
              default:
                return string.Empty;
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
          string config = _mConfig;
          MyFilmsGUI.Configuration tmpconf = new MyFilmsGUI.Configuration(config, true, null);

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
            //if (StrFileType != "0" && StrFileType != "10")
            //{
            //  LogMyFilms.Debug("Catalog Type is readonly (EC) - Update rejected ! - Movie = '" + _mStrTitle + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");
            //  return;
            //}
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

            if (tempuser == "Global" && int.Parse(tempcount) < count) // Update Count Value for Global count, if it is lower than user count
            {
              sNew = tempuser + ":" + count.ToString() + ":" + temprating;
            }
            if (tempuser == UserProfileName) // Update Count Value for selected user
            {
              sNew = tempuser + ":" + count.ToString() + ":" + temprating;
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
          newEnhancedWatchedValue = "Global:" + count + ":-1|" + UserProfileName + ":" + count + ":" + "-1";
        else
          newEnhancedWatchedValue = EnhancedWatchedValue + "|" + UserProfileName + ":" + count.ToString() + ":" + "-1";
      }
      return newEnhancedWatchedValue;
    }

  }
}
