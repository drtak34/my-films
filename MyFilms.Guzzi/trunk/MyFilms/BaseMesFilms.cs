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
  using System.Data;
  using System.IO;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Xml;

  using MediaPortal.Configuration;
  using MediaPortal.GUI.Library;
  using MediaPortal.Video.Database;

  using MyFilmsPlugin;

  using MyFilmsPlugin.MyFilms.MyFilmsGUI;
  using MyFilmsPlugin.MyFilms.Utils;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;

  public class BaseMesFilms
    {
        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

        private static AntMovieCatalog data; // Ant compatible File - with temp extended fields and person infos
        private static MyFilmsData myfilmsdata;

/*
        private static Dictionary<string, string> dataPath;
*/
        private static DataRow[] movies;
        private static DataRow[] persons;
        private static DataRow[] history;

        #region ctor
        static BaseMesFilms() {}
        #endregion

        #region méthodes statique sprivées
        private static void initData()
        {
            data = new AntMovieCatalog();
            string catalogfile = MyFilms.conf.StrFileXml;
            LogMyFilms.Debug("initData() - Try reading catalogfile '" + catalogfile + "'");
            try
            {
              using (FileStream fs = new FileStream(catalogfile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                  LogMyFilms.Debug("initData()- opening '" + catalogfile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");
                  data.ReadXml(fs);
                  fs.Close();
                  LogMyFilms.Debug("initData()- closing  '" + catalogfile + "' FileStream");
                }
              
              // data.ReadXml(MyFilms.conf.StrFileXml);
            }
            catch (Exception e)
            {
              LogMyFilms.Error("initData() : Error reading xml database after " + data.Movie.Count.ToString() + " records; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
              throw e;
            }
        }
        private static void initDataMyFilms()
        {
          string datafile = GetNameForMyFilmsDatafile(MyFilms.conf.StrFileXml);

          myfilmsdata = new MyFilmsData();
          LogMyFilms.Debug("MyFilmsData - Try reading datafile '" + datafile + "'");
          try
          {
            if (!System.IO.File.Exists(datafile)) 
              CreateEmptyDataFile(datafile);

            using (FileStream fs = new FileStream(datafile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              LogMyFilms.Debug("initData()- opening '" + datafile + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");
              myfilmsdata.ReadXml(fs);
              data.ReadXml(fs);
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
        public static DataRow[] HistorySelected
        {
          get { return history; }
        }
        #endregion

        #region méthodes statique publiques
        public static DataRow[] ReadDataMovies(string StrDfltSelect, string StrSelect, string StrSort, string StrSortSens)
        {
            return ReadDataMovies(StrDfltSelect, StrSelect, StrSort, StrSortSens, false);
        }
        public static DataRow[] ReadDataMovies(string StrDfltSelect, string StrSelect, string StrSort, string StrSortSens, bool all)
        {
          //lock (data)
          //{
            LogMyFilms.Debug("ReadDataMovies() - Starting ...");
            if (data == null)
            {
              initData();
              LogMyFilms.Debug("StrDfltSelect      : '" + StrDfltSelect + "'");
              LogMyFilms.Debug("StrSelect          : '" + StrSelect + "'");
              LogMyFilms.Debug("StrSort            : '" + StrSort + "'");
              LogMyFilms.Debug("StrSortSens        : '" + StrSortSens + "'");
              LogMyFilms.Debug("RESULTSELECT       : '" + StrDfltSelect + StrSelect, StrSort + " " + StrSortSens + "'");
            }
            else 
              LogMyFilms.Debug("ReadDataMovies() - Data already cached in memory !");
            if (StrSelect.Length == 0)
            {
              StrSelect = MyFilms.conf.StrTitle1.ToString() + " not like ''";
            }
            movies = data.Tables["Movie"].Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens);
            if (movies.Length == 0 && all)
            {
              StrSelect = MyFilms.conf.StrTitle1.ToString() + " not like ''";
              LogMyFilms.Debug("ReadDataMovies() - Switching to full list ...");
              movies = data.Tables["Movie"].Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens);
            }
            LogMyFilms.Debug("ReadDataMovies() - Finished ...");
            return movies;
          //}
        }

        public static DataRow[] ReadDataPersons(string StrSelect, string StrSort, string StrSortSens, bool all)
        {
          if (data == null) initData();
          if (StrSelect.Length == 0) StrSelect = "Name" + " not like ''";
          persons = data.Tables["Person"].Select(StrSelect, StrSort + " " + StrSortSens);
          if (persons.Length == 0 && all)
          {
            StrSelect = "Name" + " not like ''";
            persons = data.Tables["Person"].Select(StrSelect, StrSort + " " + StrSortSens);
            //Guzzi
            LogMyFilms.Debug("Persons:  StrSelect          : '" + StrSelect + "'");
            LogMyFilms.Debug("Persons:  StrSort            : '" + StrSort + "'");
            LogMyFilms.Debug("Persons:  StrSortSens        : '" + StrSortSens + "'");
            LogMyFilms.Debug("Persons:  RESULTSELECT       : '" + StrSelect, StrSort + " " + StrSortSens + "'");
          }
          return persons;
        }

        public static DataRow[] ReadDataHistory(string StrSelect, string StrSort, string StrSortSens, bool all)
        {
          if (myfilmsdata == null) initDataMyFilms();
          if (StrSelect.Length == 0) StrSelect = "Name" + " not like ''";
          history = myfilmsdata.Tables["History"].Select(StrSelect, StrSort + " " + StrSortSens);
          if (history.Length == 0 && all)
          {
            StrSelect = "Name" + " not like ''";
            history = data.Tables["History"].Select(StrSelect, StrSort + " " + StrSortSens);
          }
          return history;
        }

        public static void LoadMesFilms(string StrFileXml)
        {
          if (!System.IO.File.Exists(StrFileXml))
          {
            throw new Exception(string.Format("The file {0} does not exist !.", StrFileXml));
          }
          data = new AntMovieCatalog();
          try
          {
            data.ReadXml(StrFileXml);
          }
          catch (Exception e)
          {
            LogMyFilms.Error("Error reading xml database after " + data.Movie.Count.ToString() + " records; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
            throw new Exception("Error reading xml database after " + data.Movie.Count.ToString() + " records; error : " + e.Message.ToString());
          }
        }

        public static void LoadMyFilmsData(string datafile)
        {
          if (!System.IO.File.Exists(datafile))
          {
            LogMyFilms.Debug("The file {0} does not exist !.", datafile);
            // throw new Exception(string.Format("The file {0} does not exist !.", datafile)); // do NOT throw an exception to upper level
          }
          myfilmsdata = new MyFilmsData();
          try
          {
            myfilmsdata.ReadXml(datafile);
          }
          catch (Exception e)
          {
            LogMyFilms.Error("Error reading myfilms data xml database '" + datafile + "'; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
          }
        }

        public static void UnloadMesFilms()
        {
          if (data != null)
            data.Dispose();

          if (myfilmsdata != null)
            myfilmsdata.Dispose();
        }

        public static bool SaveMesFilms()
        {
          //lock (data)
          //{
            if (data != null)
            {
              string catalogfile = MyFilms.conf.StrFileXml;

              //bool _FileUse = false;
              //while (!_FileUse)
              //{
              //  try
              //  {
              //    // write file here
              //    Thread.Sleep(1000);
              //    _FileUse = true;
              //  }
              //  catch (Exception ex)
              //  {
              //    Thread.Sleep(1000);
              //    _FileUse = false;
              //  }
              //}


              try
              {
                using (FileStream fs = new FileStream(catalogfile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) // lock the file for any other use, as we do write to it now !
                {
                  LogMyFilms.Debug("SaveMesFilms()- opening '" + catalogfile + "' as FileStream with FileMode.OpenOrCreate, FileAccess.Write, FileShare.None");
                  // data.ReadXml(fs); data alreadry in memory, only to be saved
                  
                  fs.SetLength(0); // do not append, owerwrite !

                  using (XmlTextWriter MyXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.Default))
                  {
                    LogMyFilms.Debug("SaveMesFilms()- writing '" + catalogfile + "' as MyXmlTextWriter in FileStream"); 
                    MyXmlTextWriter.Formatting = System.Xml.Formatting.Indented; // Added by Guzzi to get properly formatted output XML
                    MyXmlTextWriter.WriteStartDocument();
                    data.WriteXml(MyXmlTextWriter, XmlWriteMode.IgnoreSchema);
                    MyXmlTextWriter.Flush();
                    MyXmlTextWriter.Close();

                  }
                  // data.WriteXml(fs);
                  // fs.Flush();
                  fs.Close(); // write buffer and release lock on file (either Flush, Dispose or Close is required)
                  LogMyFilms.Debug("SaveMesFilms()- closing '" + catalogfile + "' FileStream and releasing file lock");
                }

                //System.Xml.XmlTextWriter MyXmlTextWriter = new System.Xml.XmlTextWriter(MyFilms.conf.StrFileXml, System.Text.Encoding.Default);
                //MyXmlTextWriter.Formatting = System.Xml.Formatting.Indented; // Added by Guzzi to get properly formatted output XML
                //MyXmlTextWriter.WriteStartDocument();
                //data.WriteXml(MyXmlTextWriter, XmlWriteMode.IgnoreSchema);
                ////MyXmlTextWriter.Flush();
                //MyXmlTextWriter.Close();
              }
              catch (Exception ex)
              {
                LogMyFilms.DebugException("SaveMesFilms()- error saving '" + catalogfile + "' as FileStream with FileMode.OpenOrCreate, FileAccess.Write, FileShare.None", ex);
                MediaPortal.Dialogs.GUIDialogOK dlgOk = (MediaPortal.Dialogs.GUIDialogOK)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading("Error");//my videos
                dlgOk.SetLine(1, "Error during updating the XML database '" + catalogfile + "' !");
                dlgOk.SetLine(2, "Maybe Directory full or no write access.");
                dlgOk.DoModal(MyFilms.ID_MyFilmsDetail);
                return false;
              }
              // data.WriteXmlSchema(@"c:\myfilms.xsd"); // this writes XML schema infos to disk
            }
          //}

            if (myfilmsdata != null)
            {
              string datafile = GetNameForMyFilmsDatafile(MyFilms.conf.StrFileXml);
              try
              {
                using (FileStream fs = new FileStream(datafile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) // lock the fole for any other use, as we do write to it now !
                {
                  LogMyFilms.Debug("SaveMesFilms()- opening '" + datafile + "' as FileStream with FileMode.Open, FileAccess.Write, FileShare.None");
                  // data.ReadXml(fs); data alreadry in memory, only to be saved

                  fs.SetLength(0); // do not append, owerwrite !

                  using (XmlTextWriter MyXmlTextWriter = new XmlTextWriter(fs, System.Text.Encoding.Default))
                  {
                    LogMyFilms.Debug("SaveMesFilms()- writing '" + datafile + "' as MyXmlTextWriter in FileStream");
                    MyXmlTextWriter.Formatting = System.Xml.Formatting.Indented;
                    MyXmlTextWriter.WriteStartDocument();
                    myfilmsdata.WriteXml(MyXmlTextWriter, XmlWriteMode.IgnoreSchema);
                    MyXmlTextWriter.Flush();
                    MyXmlTextWriter.Close();
                  }
                  // myfilmsdata.WriteXml(fs);
                  fs.Flush();
                  fs.Close();
                  LogMyFilms.Debug("SaveMesFilms()- closing '" + datafile + "' FileStream and releasing file lock");
                }
                
                //System.Xml.XmlTextWriter MyXmlTextWriter = new System.Xml.XmlTextWriter(datafile, System.Text.Encoding.Default);
                //MyXmlTextWriter.Formatting = System.Xml.Formatting.Indented; // Added by Guzzi to get properly formatted output XML
                //MyXmlTextWriter.WriteStartDocument();
                //myfilmsdata.WriteXml(MyXmlTextWriter, XmlWriteMode.IgnoreSchema);
                //MyXmlTextWriter.Close();
              }
              catch (Exception ex)
              {
                LogMyFilms.DebugException("SaveMesFilms()- error writing '" + datafile + "', exception: " + ex.Message, ex);
              }
              // data.WriteXmlSchema(@"c:\myfilms.xsd"); // this writes XML schema infos to disk
            }
          return true; // write successful!
        }

        public static void CancelMesFilms()
        {
          if (data != null)
            data.Clear();
          if (myfilmsdata != null)
            myfilmsdata.Clear();
        }

        public static void GetMovies(ref ArrayList movies)
        {
          movies.Clear(); 
          AntMovieCatalog dataExport = new AntMovieCatalog();

          using (XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
          {
            int MesFilms_nb_config = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
            ArrayList configs = new ArrayList();
            for (int i = 0; i < MesFilms_nb_config; i++)
              configs.Add(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));

            foreach (string config in configs)
            {
              string Catalog = XmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
              // string StrFileType = XmlConfig.ReadXmlConfig("MyFilms", config, "CatalogType", "0");
              bool TraktEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
              string StrTitle1 = XmlConfig.ReadXmlConfig("MyFilms", config, "AntTitle1", string.Empty);
              string StrDfltSelect = XmlConfig.ReadXmlConfig("MyFilms", config, "StrDfltSelect", string.Empty);
              bool EnhancedWatchedStatusHandling = XmlConfig.ReadXmlConfig("MyFilms", config, "EnhancedWatchedStatusHandling", false);
              string GlobalUnwatchedOnlyValue = XmlConfig.ReadXmlConfig("MyFilms", config, "GlobalUnwatchedOnlyValue", "false");
              string WatchedField = XmlConfig.ReadXmlConfig("MyFilms", config, "WatchedField", "Checked");
              string UserProfileName = XmlConfig.ReadXmlConfig("MyFilms", config, "UserProfileName", "");

              string Storage = XmlConfig.ReadXmlConfig("MyFilms", config, "AntStorage", string.Empty);
              if (TraktEnabled)
                LogMyFilms.Debug("Trakt:GetMovies() - Sync = '" + TraktEnabled + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");
              else
                LogMyFilms.Debug("Trakt:GetMovies() - Sync = '" + TraktEnabled + "', Config = '" + config + "'");
              if (System.IO.File.Exists(Catalog) && TraktEnabled)
              {
                try
                {
                  dataExport.ReadXml(Catalog);
                }
                catch (Exception e)
                {
                  LogMyFilms.Error(": Error reading xml database after " + dataExport.Movie.Count.ToString() + " records; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
                  throw e;
                }

                try
                {
                  DataRow[] results = dataExport.Tables["Movie"].Select(StrDfltSelect + StrTitle1 + " not like ''", "OriginalTitle" + " " + "ASC");
                  // if (results.Length == 0) continue;

                  foreach (DataRow sr in results)
                  {
                    try
                    {
                      MFMovie movie = new MFMovie();
                      movie.Config = config; // MF config context
                      if (!string.IsNullOrEmpty(sr["Number"].ToString()))
                        movie.ID = Int32.Parse(sr["Number"].ToString());
                      else movie.ID = 0;
                      movie.Year = Int32.Parse(sr["Year"].ToString());
                      movie.Title = sr["OriginalTitle"].ToString();

                      bool played = false;
                      if (EnhancedWatchedStatusHandling)
                      {
                        if (MyFilms.EnhancedWatched(sr[WatchedField].ToString(), UserProfileName) == true)
                          played = true;
                      }
                      else
                      {
                        if (GlobalUnwatchedOnlyValue != null && WatchedField.Length > 0)
                          if (sr[WatchedField].ToString().ToLower() != GlobalUnwatchedOnlyValue.ToLower()) // changed to take setup config into consideration
                            played = true;
                      }
                      movie.Watched = played;
                      movie.WatchedCount = -1; // check against it, if value returns...
                      
                      float rating = 0;
                      bool success = float.TryParse(sr["Rating"].ToString(), out rating);
                      if (!success) rating = 0;
                      movie.Rating = rating;
                      // movie.Rating = (float)Double.Parse(sr["Rating"].ToString());
                      
                      string mediapath = string.Empty;
                      if (!string.IsNullOrEmpty(Storage) && Storage != "(none)")
                      {
                        mediapath = sr[Storage].ToString();
                        if (mediapath.Contains(";")) // take the first source file
                          mediapath = mediapath.Substring(0, mediapath.IndexOf(";"));
                      }
                      movie.File = mediapath;

                      string IMDB = "";


                      if (!string.IsNullOrEmpty(sr["IMDB_Id"].ToString()))
                        IMDB = sr["IMDB_Id"].ToString();

                      if (!string.IsNullOrEmpty(sr["URL"].ToString()) && string.IsNullOrEmpty(IMDB))
                      {
                        string CleanString = sr["URL"].ToString();
                        Regex CutText = new Regex("" + @"tt\d{7}" + "");
                        Match m = CutText.Match(CleanString);
                        if (m.Success)
                          IMDB = m.Value;
                      }
                      movie.IMDBNumber = IMDB;

                      if (!string.IsNullOrEmpty(sr["TMDB_Id"].ToString())) 
                        movie.TMDBNumber = sr["TMDB_Id"].ToString();
                      movie.DateAdded = sr["Date"].ToString();
                      movies.Add(movie);
                    }
                    catch (Exception mex)
                    {
                      Log.Error("MyFilms videodatabase: add movie exception - err:{0} stack:{1}", mex.Message, mex.StackTrace);
                      throw;
                    }
                  }
                }
                catch (Exception ex)
                {
                  LogMyFilms.Error("MyFilms videodatabase exception err:{0} stack:{1}", ex.Message, ex.StackTrace);
                  Log.Error("MyFilms videodatabase exception err:{0} stack:{1}", ex.Message, ex.StackTrace);
                }
              }
            } 
          }
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

              default:
                return string.Empty;
            }
        }
        #endregion
    }

  public class MFMovie
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

    private int _mID = -1;
    private string _mStrTitle = string.Empty;
    private string _mStrFile = string.Empty;
    private string _mStrPath = string.Empty;
    private string _mStrIMDBNumber = string.Empty;
    private string _mStrTMDBNumber = string.Empty;
    private int _mIYear = 1900;
    private float _mFRating;
    private bool _mIWatched;
    private int _mIWatchedCount = -1;
    private string _mDateAdded = string.Empty;
    private string _mPicture = string.Empty;
    private string _mFanart = string.Empty;
    private string _mConfig = string.Empty;
    private string _mUsername = string.Empty;

    public MFMovie() { }

    #region Variables
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

    public float Rating
    {
      get { return _mFRating; }
      set { _mFRating = value; }
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

    public void Reset()
    {
      _mStrTitle = string.Empty;
      _mStrIMDBNumber = string.Empty;
      _mStrTMDBNumber = string.Empty;
      _mIYear = 1900;
      _mFRating = 0.0f;
      _mIWatched = false;
      _mIWatchedCount = -1;
      _mDateAdded = string.Empty;
      _mPicture = string.Empty;
      _mFanart = string.Empty;
      _mConfig = string.Empty;
      _mUsername = string.Empty;
    }
    #endregion

    public void Commit()
    {
      AntMovieCatalog dataImport = new AntMovieCatalog();

      using (XmlSettings XmlConfig = new XmlSettings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MyFilms.xml")))
      {
          string config = _mConfig;
          //Configuration confimport = new Configuration(config, true);
          //Configuration.Current_Config();
          //MyFilms.Load_Config(Configuration.config, true);
          string Catalog = XmlConfig.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty);
          string StrFileType = XmlConfig.ReadXmlConfig("MyFilms", config, "CatalogType", "0");
          bool TraktEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
          string StrTitle1 = XmlConfig.ReadXmlConfig("MyFilms", config, "AntTitle1", string.Empty);
          string StrDfltSelect = XmlConfig.ReadXmlConfig("MyFilms", config, "StrDfltSelect", string.Empty);
          bool EnhancedWatchedStatusHandling = XmlConfig.ReadXmlConfig("MyFilms", config, "EnhancedWatchedStatusHandling", false);
          string GlobalUnwatchedOnlyValue = XmlConfig.ReadXmlConfig("MyFilms", config, "GlobalUnwatchedOnlyValue", "false");
          string WatchedField = XmlConfig.ReadXmlConfig("MyFilms", config, "WatchedField", "Checked");
          string UserProfileName = XmlConfig.ReadXmlConfig("MyFilms", config, "UserProfileName", "");

          LogMyFilms.Debug("Commit() : TraktSync = '" + TraktEnabled + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");
          LogMyFilms.Debug("Commit() : Update requested for Movie = '" + _mStrTitle + "' (" + _mIYear + "), IMDB = '" + _mStrIMDBNumber + "', Watched = '" + _mIWatched + "'");

          if (System.IO.File.Exists(Catalog))
          {
            if (!TraktEnabled)
            {
              LogMyFilms.Debug("Trakt not enabled for this config - Update rejected ! - Movie = '" + _mStrTitle + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");
              return;
            }
            if (StrFileType != "0")
            {
              LogMyFilms.Debug("Catalog Type is readonly (EC) - Update rejected ! - Movie = '" + _mStrTitle + "', Config = '" + config + "', Catalogfile = '" + Catalog + "'");
              return;
            }
            try
            {
              using (FileStream fs = new FileStream(Catalog, FileMode.Open, FileAccess.Read, FileShare.Read))
              {
                //LogMyFilms.Debug("Commit()- opening '" + Catalog + "' as FileStream with FileMode.Open, FileAccess.Read, FileShare.Read");
                dataImport.ReadXml(fs);
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
                  if (sr[WatchedField].ToString() != oldWatchedString)
                    LogMyFilms.Debug("Commit() : Updating 'Watched' from '" + oldWatchedString + "' to '" + sr[WatchedField].ToString() + "', WatchedCount = '" + _mIWatchedCount + "'");

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
                  throw;
                }
              }
            }
            catch (Exception e)
            {
              LogMyFilms.Error(": Error reading xml database after " + dataImport.Movie.Count.ToString() + " records; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
            }

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
