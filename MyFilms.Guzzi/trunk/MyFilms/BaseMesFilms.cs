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

        //private static MyFilmsData MFdata; // Separate DB File to store "NonANT Data"

/*
        private static Dictionary<string, string> dataPath;
*/
        private static DataRow[] movies;

        private static DataRow[] persons;

        #region ctor
        static BaseMesFilms()
        {
        }
        #endregion

        #region méthodes statique sprivées
        private static void initData()
        {
            data = new AntMovieCatalog();
            LogMyFilms.Debug("BaseMesFilms - Try reading catalogfile '" + MyFilms.conf.StrFileXml + "'");
            try
            {
              data.ReadXml(MyFilms.conf.StrFileXml);
            }
            catch (Exception e)
            {
              LogMyFilms.Error(": Error reading xml database after " + data.Movie.Count.ToString() + " records; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
              throw e;
            }
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
          //lock (data)
          //{
            LogMyFilms.Debug("ReadDataMovies() - Starting ...");
            if (data == null) 
              initData();
            else 
              LogMyFilms.Debug("ReadDataMovies() - Data already cached in memory !");
            LogMyFilms.Debug("StrDfltSelect      : '" + StrDfltSelect + "'");
            LogMyFilms.Debug("StrSelect          : '" + StrSelect + "'");
            LogMyFilms.Debug("StrSort            : '" + StrSort + "'");
            LogMyFilms.Debug("StrSortSens        : '" + StrSortSens + "'");
            LogMyFilms.Debug("RESULTSELECT       : '" + StrDfltSelect + StrSelect, StrSort + " " + StrSortSens + "'");
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
              LogMyFilms.Debug("- BaseMesFilmsPersons:  StrSelect          : '" + StrSelect + "'");
              LogMyFilms.Debug("- BaseMesFilmsPersons:  StrSort            : '" + StrSort + "'");
              LogMyFilms.Debug("- BaseMesFilmsPersons:  StrSortSens        : '" + StrSortSens + "'");
              LogMyFilms.Debug(
                "MF: - BaseMesFilmsPersons:  RESULTSELECT       : '" + StrSelect, StrSort + " " + StrSortSens + "'");
            }
            return persons;
        }

        public static void LoadFilm(string StrFileXml)
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
              LogMyFilms.Error("MF: : Error reading xml database after " + data.Movie.Count.ToString() + " records; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
              throw new Exception("Error reading xml database after " + data.Movie.Count.ToString() + " records; error : " + e.Message.ToString());
            }
        }
        public static void UnloadMesFilms()
        {
            if (data != null)
            {
              data.Dispose();

            }
        }
        public static void SaveMesFilms()
        {
          //lock (data)
          //{
            if (data != null)
            {
              try
              {
                System.Xml.XmlTextWriter MyXmlTextWriter = new System.Xml.XmlTextWriter(MyFilms.conf.StrFileXml, System.Text.Encoding.Default);
                MyXmlTextWriter.Formatting = System.Xml.Formatting.Indented; // Added by Guzzi to get properly formatted output XML
                MyXmlTextWriter.WriteStartDocument();
                data.WriteXml(MyXmlTextWriter, XmlWriteMode.IgnoreSchema);
                MyXmlTextWriter.Close();
              }
              catch
              {
                MediaPortal.Dialogs.GUIDialogOK dlgOk = (MediaPortal.Dialogs.GUIDialogOK)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading("Error");//my videos
                dlgOk.SetLine(1, "Error during updating the XML database !");
                dlgOk.SetLine(2, "Maybe Directory full or no write access.");
                dlgOk.DoModal(MyFilms.ID_MyFilmsDetail);
              }
            }
          //}
        }

        public static void CancelMesFilms()
        {
            if (data != null)
            {
              data.Clear();
            }
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
              bool TraktEnabled = XmlConfig.ReadXmlConfig("MyFilms", config, "AllowTraktSync", false);
              string StrTitle1 = XmlConfig.ReadXmlConfig("MyFilms", config, "AntTitle1", string.Empty);
              string StrDfltSelect = XmlConfig.ReadXmlConfig("MyFilms", config, "StrDfltSelect", string.Empty);
              string GlobalUnwatchedOnlyValue = XmlConfig.ReadXmlConfig("MyFilms", config, "GlobalUnwatchedOnlyValue", "false");
              string WatchedField = XmlConfig.ReadXmlConfig("MyFilms", config, "WatchedField", "Checked");
              string Storage = XmlConfig.ReadXmlConfig("MyFilms", config, "AntStorage", string.Empty);
              LogMyFilms.Debug("BaseMesFilms - GetMovies: catalogfile '" + Catalog + "', enabled for Trakt: '" + TraktEnabled + "'");
              if (System.IO.File.Exists(Catalog) && TraktEnabled)
              {
                try
                {
                  dataExport.ReadXml(Catalog);
                }
                catch (Exception e)
                {
                  LogMyFilms.Error(": Error reading xml database after " + data.Movie.Count.ToString() + " records; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
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
                      if (!string.IsNullOrEmpty(sr["Number"].ToString()))
                        movie.ID = Int32.Parse(sr["Number"].ToString());
                      else movie.ID = 0;
                      movie.Year = Int32.Parse(sr["Year"].ToString());
                      movie.Title = sr["OriginalTitle"].ToString();
                      if (GlobalUnwatchedOnlyValue != null && WatchedField.Length > 0)
                        if (sr[WatchedField].ToString().ToLower() != GlobalUnwatchedOnlyValue.ToLower())
                          movie.Watched = true;
                      //if (MyFilms.conf.StrSuppress && MyFilms.conf.StrSuppressField.Length > 0)
                      //  if ((sr[MyFilms.conf.StrSuppressField].ToString() == MyFilms.conf.StrSuppressValue.ToString()) && (MyFilms.conf.StrSupPlayer))
                      //    movie.Watched = true;
                      movie.Rating = (float)Double.Parse(sr["Rating"].ToString());
                      string mediapath = string.Empty;
                      if (!string.IsNullOrEmpty(Storage) && Storage != "(none)")
                      {
                        mediapath = sr[Storage].ToString();
                        if (mediapath.Contains(";")) // take the first source file
                          mediapath = mediapath.Substring(0, mediapath.IndexOf(";"));
                      }
                      movie.File = mediapath;
                      if (!string.IsNullOrEmpty(sr["IMDB_Id"].ToString()))
                        movie.IMDBNumber = sr["IMDB_Id"].ToString();
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
            switch (Column)
            {
                case "Number":
                    return GUILocalizeStrings.Get(10798650);
                case "Checked":
                    return GUILocalizeStrings.Get(10798651);
                case "MediaLabel":
                    return GUILocalizeStrings.Get(10798652);
                case "MediaType":
                    return GUILocalizeStrings.Get(10798653);
                case "Source":
                    return GUILocalizeStrings.Get(10798654);
                case "Date":
                    return GUILocalizeStrings.Get(10798655);
                case "Borrower":
                    return GUILocalizeStrings.Get(10798656);
                case "Rating":
                    return GUILocalizeStrings.Get(10798657);
                case "OriginalTitle":
                    return GUILocalizeStrings.Get(10798658);
                case "TranslatedTitle":
                    return GUILocalizeStrings.Get(10798659);
                case "FormattedTitle":
                    return GUILocalizeStrings.Get(10798660);
                case "Director":
                    return GUILocalizeStrings.Get(10798661);
                case "Producer":
                    return GUILocalizeStrings.Get(10798662);
                case "Country":
                    return GUILocalizeStrings.Get(10798663);
                case "Category":
                    return GUILocalizeStrings.Get(10798664);
                case "Year":
                    return GUILocalizeStrings.Get(10798665);
                case "Length":
                    return GUILocalizeStrings.Get(10798666);
                case "Actors":
                    return GUILocalizeStrings.Get(10798667);
                case "URL":
                    return GUILocalizeStrings.Get(10798668);
                case "Description":
                    return GUILocalizeStrings.Get(10798669);
                case "Comments":
                    return GUILocalizeStrings.Get(10798670);
                case "VideoFormat":
                    return GUILocalizeStrings.Get(10798671);
                case "VideoBitrate":
                    return GUILocalizeStrings.Get(10798672);
                case "AudioFormat":
                    return GUILocalizeStrings.Get(10798673);
                case "AudioBitrate":
                    return GUILocalizeStrings.Get(10798674);
                case "Resolution":
                    return GUILocalizeStrings.Get(10798675);
                case "Framerate":
                    return GUILocalizeStrings.Get(10798676);
                case "Languages":
                    return GUILocalizeStrings.Get(10798677);
                case "Subtitles":
                    return GUILocalizeStrings.Get(10798678);
                case "DateAdded":
                    return GUILocalizeStrings.Get(10798679);
                case "Size":
                    return GUILocalizeStrings.Get(10798680);
                case "Disks":
                    return GUILocalizeStrings.Get(10798681);
                case "Picture":
                    return GUILocalizeStrings.Get(10798682);
                case "Certification":
                    return GUILocalizeStrings.Get(10798683);
                case "Writer":
                    return GUILocalizeStrings.Get(10798684);
                case "Watched":
                    return GUILocalizeStrings.Get(10798685);
                case "WatchedDate":
                    return GUILocalizeStrings.Get(10798686);
                case "IMDB_Id":
                    return GUILocalizeStrings.Get(10798687);
                case "TMDB_Id":
                    return GUILocalizeStrings.Get(10798688);
                case "SourceTrailer":
                    return GUILocalizeStrings.Get(10798940);
                case "TagLine":
                    return GUILocalizeStrings.Get(10798941);
                case "Tags":
                    return GUILocalizeStrings.Get(10798942);
                case "Aspectratio":
                    return GUILocalizeStrings.Get(10798943);
                case "RatingUser":
                    return GUILocalizeStrings.Get(10798944);
                case "Fanart":
                    return GUILocalizeStrings.Get(10798945);
                case "Studio":
                    return GUILocalizeStrings.Get(10798946);
                case "IMDB_Rank":
                    return GUILocalizeStrings.Get(10798947);
                case "IsOnline":
                    return GUILocalizeStrings.Get(10798948);
                case "Edition":
                    return GUILocalizeStrings.Get(10798949);
                case "IsOnlineTrailer":
                    return GUILocalizeStrings.Get(10798950);
                default:
                    return string.Empty;
            }
        }
        #endregion
    }
  public class MFMovie
  {
    private int _mID = -1;
    private string _mStrTitle = string.Empty;
    private string _mStrFile = string.Empty;
    private string _mStrPath = string.Empty;
    private string _mStrIMDBNumber = string.Empty;
    private string _mStrTMDBNumber = string.Empty;
    private int _mIYear = 1900;
    private float _mFRating;
    private bool _mIWatched;
    private string _mDateAdded = string.Empty;
    private string _mPicture = string.Empty;
    private string _mFanart = string.Empty;

    public MFMovie() { }

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

    public void Reset()
    {
      _mStrTitle = string.Empty;
      _mStrIMDBNumber = string.Empty;
      _mStrTMDBNumber = string.Empty;
      _mIYear = 1900;
      _mFRating = 0.0f;
      _mIWatched = false;
      _mDateAdded = string.Empty;
      _mPicture = string.Empty;
      _mFanart = string.Empty;
    }
  }
}
