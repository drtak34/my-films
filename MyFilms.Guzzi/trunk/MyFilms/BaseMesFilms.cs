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


namespace MesFilms
{
    public class BaseMesFilms
    {
        private static AntMovieCatalog data;
        private static Dictionary<string, string> dataPath;
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
            return LectureDonnées(StrDfltSelect, StrSelect, StrSort, StrSortSens, true);
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

        public static void Lirefilm(string nomfilm)
        {
            if (dataPath.ContainsKey(""))
            {

            }
        }

        #endregion
    }
}
