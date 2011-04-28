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
  using System.Data;

  using MyFilmsPlugin;

  using MyFilmsPlugin.MyFilms.MyFilmsGUI;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;

  public class BaseMesFilms
    {
        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
    
        private static AntMovieCatalog data;
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
            LogMyFilms.Debug("MF: BaseMesFilms - Try reading catalogfile '" + MyFilms.conf.StrFileXml + "'");
            try
            {
                data.ReadXml(MyFilms.conf.StrFileXml);
            }
            catch (Exception e)
            {
              LogMyFilms.Error("MF: : Error reading xml database after " + data.Movie.Count.ToString() + " records; error : " + e.Message.ToString() + ", " + e.StackTrace.ToString());
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
        }

        public static DataRow[] ReadDataPersons(string StrSelect, string StrSort, string StrSortSens, bool all)
        {
          if (data == null)
            initData();
          if (StrSelect.Length == 0)
            StrSelect = "Name" + " not like ''";
          persons = data.Tables["Person"].Select(StrSelect, StrSort + " " + StrSortSens);
          if (persons.Length == 0 && all)
          {
            StrSelect = "Name" + " not like ''";
            persons = data.Tables["Person"].Select(StrSelect, StrSort + " " + StrSortSens);
            //Guzzi
            LogMyFilms.Debug("MF: - BaseMesFilmsPersons:  StrSelect          : '" + StrSelect + "'");
            LogMyFilms.Debug("MF: - BaseMesFilmsPersons:  StrSort            : '" + StrSort + "'");
            LogMyFilms.Debug("MF: - BaseMesFilmsPersons:  StrSortSens        : '" + StrSortSens + "'");
            LogMyFilms.Debug("MF: - BaseMesFilmsPersons:  RESULTSELECT       : '" + StrSelect, StrSort + " " + StrSortSens + "'");
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
            if (data != null)
            {
                try
                {
                    System.Xml.XmlTextWriter MyXmlTextWriter = new System.Xml.XmlTextWriter (MyFilms.conf.StrFileXml, System.Text.Encoding.Default);
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
        }

        public static void CancelMesFilms()
        {
            if (data != null)
            {
                data.Clear();
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
}
