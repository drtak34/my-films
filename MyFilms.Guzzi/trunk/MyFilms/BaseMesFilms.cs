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
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using MediaPortal.ServiceImplementations;


namespace MesFilms
{
    public class BaseMesFilms
    {
        private static AntMovieCatalog data;
        //Auskommentiert, da nie benutzt !!!
        //private static Dictionary<string, string> dataPath;
        private static DataRow[] movies;

        #region ctor
        static BaseMesFilms()
        {
        }
        #endregion

        #region méthodes statique sprivées
        private static void initData()
        {
            data = new AntMovieCatalog();
            try
            {
                data.ReadXml(MesFilms.conf.StrFileXml);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region accesseurs
        public static DataRow[] FilmsSelected
        {
            get { return movies; }
        }

        #endregion

        #region méthodes statique publiques
        public static DataRow[] LectureDonnées(string StrDfltSelect, string StrSelect, string StrSort, string StrSortSens)
        {
            //Changed accordibg latest Zebons SVN
            //return LectureDonnées(StrDfltSelect, StrSelect, StrSort, StrSortSens, true);
            return LectureDonnées(StrDfltSelect, StrSelect, StrSort, StrSortSens, false);
        }
        public static DataRow[] LectureDonnées(string StrDfltSelect, string StrSelect, string StrSort, string StrSortSens, bool all)
        {
            if (data == null)
                initData();
            if (StrSelect.Length == 0)
                StrSelect = MesFilms.conf.StrTitle1.ToString() + " not like ''";
            movies = data.Tables["Movie"].Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens);
            if (movies.Length == 0 && all)
            {
                StrSelect = MesFilms.conf.StrTitle1.ToString() + " not like ''";
                movies = data.Tables["Movie"].Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens);
                //Guzzi
                Log.Debug("MyFilms - BaseMesFilms:  StrDfltSelect      : '" + StrDfltSelect + "'");
                Log.Debug("MyFilms - BaseMesFilms:  StrSelect          : '" + StrSelect + "'");
                Log.Debug("MyFilms - BaseMesFilms:  StrSort            : '" + StrSort + "'");
                Log.Debug("MyFilms - BaseMesFilms:  StrSortSens        : '" + StrSortSens + "'");
                Log.Debug("MyFilms - BaseMesFilms:  RESULTSELECT       : '" + StrDfltSelect + StrSelect, StrSort + " " + StrSortSens + "'");
            }
            return movies;
        }
 
        public static void LoadFilm(string StrFileXml)
        {
            if (!System.IO.File.Exists(StrFileXml))
            {
                throw new Exception(string.Format("Le fichier {0} n'existe pas.", StrFileXml));
            }
            data = new AntMovieCatalog();
            try
            {
                data.ReadXml(StrFileXml);
            }
            catch (Exception e)
            {
                MediaPortal.GUI.Library.Log.Error("MyFilms : Error reading xml database after " + data.Movie.Count.ToString() + " records; error : " + e.Message.ToString());
  //              throw new Exception("Error reading xml database after " + data.Movie.Count.ToString() + " records; error : " + e.Message.ToString());
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
                    System.Xml.XmlTextWriter MyXmlTextWriter = new System.Xml.XmlTextWriter
                              (MesFilms.conf.StrFileXml, System.Text.Encoding.Default);
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
                    dlgOk.DoModal(MesFilms.ID_MesFilmsDetail);
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

        //public static void Lirefilm(string nomfilm)
        //{
        //    if (dataPath.ContainsKey(""))
        //    {

        //    }
        //}
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
                default:
                    return string.Empty;
            }
        }
        #endregion
    }
}
