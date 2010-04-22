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
using System.Collections.Generic;
using System.Text;
using MediaPortal.Configuration;
using System.IO;
using MediaPortal.Util;
using System.ComponentModel;



namespace MesFilms
{
    public class Configuration
    {
        System.ComponentModel.BackgroundWorker bgPictureList = new System.ComponentModel.BackgroundWorker();
        public Configuration(string CurrentConfig, bool create_temp)
        {
            //-----------------------------------------------------------------------------------------------
            //   Load Config Parameters in MyFilms.xml file (section CurrentConfig)
            //-----------------------------------------------------------------------------------------------

            XmlConfig XmlConfig = new XmlConfig();
            XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Current_Config", CurrentConfig);
            using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
            {
                xmlwriter.SetValue("MyFilms", "Current_Config", CurrentConfig);
            }
            using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
            {
                StrStorage = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntStorage", "");
                StrDirStor = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "PathStorage", "");
                SearchFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SearchFileName", "False");
                ItemSearchFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ItemSearchFileName", "");
                SearchSubDirs = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SearchSubDirs", "No");
                CheckWatched = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CheckWatched", false);
                StrIdentItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntIdentItem", "");
                StrTitle1 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntTitle1", "");
                StrTitle2 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntTitle2", "");
                StrSTitle = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntSTitle", "");
                if (StrSTitle == "")
                    StrSTitle = StrTitle1;
                StrViewDfltItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ViewDfltItem", "");
                StrViewDfltText = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ViewDfltText", "");
                for (int i = 1; i < 6; i++)
                {
                    StrViewItem[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewItem{0}", i.ToString()), "");
                    StrViewText[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewText{0}", i.ToString()), "");
                    //if (StrViewText[i - 1].ToLower() == wViewDfltItem.ToLower())
                    //    StrViewDfltItem = StrViewItem[i - 1];
                    StrViewValue[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewValue{0}", i.ToString()), "");
                }
                for (int i = 1; i < 3; i++)
                {
                    StrSearchText[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntSearchText{0}", i), "");
                    StrSearchItem[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntSearchItem{0}", i), "");
                    StrTSort[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntTSort{0}", i), "");
                    StrSort[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntSort{0}", i), "");
                }
                StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalog", "");
                StrFileType = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CatalogType", "0");
                StrPathImg = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntPicture", "");
                StrPathFanart = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "FanartPicture", "");
                StrPathViews = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ViewsPicture", "");
                StrLayOut = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "LayOut", 0);
                StrLayOut = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "WLayOut", StrLayOut);
                Strlabel1 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntLabel1", "");
                Strlabel2 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntLabel2", "");
                Stritem1 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntItem1", "");
                Stritem2 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntItem2", "");
                Stritem3 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntItem3", "");
                StrIdentLabel = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntIdentLabel", "");
                StrMCCovers = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "MCCovers", "Thumbnails");
                StrLogos = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Logos", false);
                StrSuppress = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Suppress", false);
                StrSupPlayer = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SuppressPlayed", false);
                StrSuppressType = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SuppressType", "");
                StrSuppressField = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SuppressField", "");
                StrSuppressValue = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SuppressValue", "");
                switch (StrFileType)
                {
                    case "0":
                        break;
                    case "1":
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool SortTitle = false;
                            bool OnlyFile = false;
                            SortTitle = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SortTitle", false);
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            string TagField = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "DVDPTagField", "");
                            CatalogConverter cv = new CatalogConverter(TagField);
                            StrFileXml = cv.ConvertProfiler(StrFileXml, StrPathImg, SortTitle, TagField,OnlyFile);
                        }
                        else
                            StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", "");
                        break;
                    case "2":
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool SortTitle = false;
                            bool OnlyFile = false;
                            SortTitle = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SortTitle", false);
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            MovieCollector mc = new MovieCollector();
                            StrFileXml = mc.ConvertMovieCollector(StrFileXml, StrPathImg, SortTitle, OnlyFile,StrMCCovers,TitleDelim);
                        }
                        else
                            StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", "");
                        break;
                    case "3":
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool SortTitle = false;
                            bool OnlyFile = false;
                            SortTitle = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SortTitle", false);
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            MyMovies mm = new MyMovies();
                            StrFileXml = mm.ConvertMyMovies(StrFileXml, StrPathImg, SortTitle, OnlyFile);
                        }
                        else
                            StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", "");
                        break;
                    case "4":
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool SortTitle = false;
                            bool OnlyFile = false;
                            SortTitle = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SortTitle", false);
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            EaxMovieCatalog emc = new EaxMovieCatalog();
                            StrFileXml = emc.ConvertEaxMovieCatalog(StrFileXml, StrPathImg, SortTitle, OnlyFile, TitleDelim);
                        }
                        else
                            StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", "");
                        break;
                    case "5":
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool SortTitle = false;
                            bool OnlyFile = false;
                            SortTitle = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SortTitle", false);
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            XMM xmm = new XMM();
                            StrFileXml = xmm.ConvertXMM(StrFileXml, StrPathImg, SortTitle, OnlyFile);
                        }
                        else
                            StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", "");
                        break;
                    case "6":
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool SortTitle = false;
                            bool OnlyFile = false;
                            SortTitle = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SortTitle", false);
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            XbmcNfo nfo = new XbmcNfo();
                            StrFileXml = nfo.ConvertXbmcNfo(StrFileXml, StrPathImg,StrStorage, SortTitle, OnlyFile, TitleDelim);
                        }
                        else
                            StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", "");
                        break;
                }
                StrSelect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSelect", "");
                StrActors = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrActors", "");
                StrTitleSelect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrTitleSelect", "");
                StrFilmSelect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrFilmSelect", "");
                StrDfltSelect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrDfltSelect", "");
                StrSorta = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSort", "");
                CurrentSortMethod = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CurrentSortMethod", "");
                StrSortSens = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSortSens", "");
                string wDfltSort = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSort", "");
                string wDfltSortSens = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSortSens", "");
                string wDfltSortMethod = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltSortMethod", "");
                StrTxtSelect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Selection", "");
                try { StrIndex = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "IndexItem", -1); }
                catch { StrIndex = -1; };
                StrTIndex = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "TitleItem", "");
                Boolselect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Boolselect", false);
                Boolreturn = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Boolreturn", false);
                Boolview = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Boolview", false);
                WStrSort = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "WStrSort", "");
                Wselectedlabel = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "WSelectedLabel", "");
                Wstar = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Wstar", "");
                LastID = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "LastID", -1);
                TitleDelim = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "TitleDelim", ".");
                DefaultCover = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "DefaultCover", "");
                StrGrabber = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Grabber", false);
                StrGrabber_cnf = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Grabber_cnf", "");
                StrGrabber_Dir = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Grabber_Dir", "");
                StrGrabber_Always = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Grabber_Always", false);
                StrGrabber_ChooseScript = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Grabber_ChooseScript", false);
                StrAMCUpd = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AMCUpd", false);
                StrAMCUpd_exe = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AMCUpd_exe", "");
                StrAMCUpd_cnf = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AMCUpd_cnf", "");
                StrFanart = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Fanart", false);
                StrFanartDflt = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "FanartDflt", false);
                StrViews = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Views", false);
                StrViewsDflt = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ViewsDflt", false);
                int j = 0;
                for (int i = 1; i <= 5; i++)
                {
                    if (XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ListSeparator" + i, "").Length > 0)
                    {
                        listSeparator[j] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ListSeparator" + i, "");
                        j++;
                    }
                }
                j = 0;
                for (int i = 1; i <= 5; i++)
                {
                    if (XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "RoleSeparator" + i, "").Length > 0)
                    {
                        RoleSeparator[j] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "RoleSeparator" + i, "");
                        j++;
                    }
                }

                CmdExe = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CmdExe", "");
                if (CmdExe == "(none)")
                    CmdExe = "";
                CmdPar = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CmdPar", "");
                if (CmdPar == "(none)")
                    CmdPar = "";
                OnlyTitleList = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyTitleList", false);
                WindowsFileDialog = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "WindowsFileDialog", false);
                AlwaysDefaultView = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AlwaysDefaultView", false);
                if (AlwaysDefaultView)
                {
                    strIndex = -1;
                    LastID = -1;
                    Wstar = "";
                    Boolreturn = false;
                    Boolselect = true;
                    Wselectedlabel = StrViewDfltText;
                    StrLayOut = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "LayOut", StrLayOut);
                    if (wDfltSort != "(none)" && wDfltSort.Length > 0)
                    {
                        StrSorta = wDfltSort;
                        StrSortSens = wDfltSortSens;
                        CurrentSortMethod = wDfltSortMethod;
                    }
                }
            }
            if (StrSelect == "")
                StrSelect = StrTitle1.ToString() + " not like ''";
            if (StrSort[0].Length == 0)
                StrSort[0] = "(none)";
            if (StrSort[1].Length == 0)
                StrSort[1] = "(none)";
            if (StrSorta == "")
                StrSorta = StrSTitle;
            if (StrSortSens == "")
                StrSortSens = " ASC";
            if (StrFanart)
                if (!(StrPathFanart.Length > 0 && System.IO.Directory.Exists(StrPathFanart)))
                {
                    MediaPortal.GUI.Library.Log.Info("MyFilms : Fanart Path '" + StrPathFanart + "', doesn't exist. Fanart desabled ! ");
                    StrFanart = false;
                }
        }

// static values
        private static string currentConfig = string.Empty;
        public static string CurrentConfig
        {
            get { return currentConfig; }
            set { currentConfig = value; }
        }
        private static int nbConfig = int.MinValue;
        public static int NbConfig
        {
            get { return nbConfig; }
            set { nbConfig = value; }
        }
        private static int currentMovie = int.MinValue;
        public static int CurrentMovie
        {
            get { return currentMovie; }
            set { currentMovie = value; }
        }

        // bool variables
        private bool boolselect = false;
        public bool Boolselect
        {
            get { return boolselect; }
            set { boolselect = value; }
        }
        private bool boolreturn = false;
        public bool Boolreturn
        {
            get { return boolreturn; }
            set { boolreturn = value; }
        }
        private bool boolview = false;
        public bool Boolview
        {
            get { return boolview; }
            set { boolview = value; }
        }
        private bool boolstorage = false;
        public bool Boolstorage
        {
            get { return boolstorage; }
            set { boolstorage = value; }
        }
        private bool alwaysDefaultView = false;
        public bool AlwaysDefaultView
        {
            get { return alwaysDefaultView; }
            set { alwaysDefaultView = value; }
        }
        private bool onlyTitleList = false;
        public bool OnlyTitleList
        {
            get { return onlyTitleList; }
            set { onlyTitleList = value; }
        }
        private bool windowsFileDialog = false;
        public bool WindowsFileDialog
        {
            get { return windowsFileDialog; }
            set { windowsFileDialog = value; }
        }
        private bool strLogos = false;
        public bool StrLogos
        {
            get { return strLogos; }
            set { strLogos = value; }
        }
        private bool strSuppress = false;
        public bool StrSuppress
        {
            get { return strSuppress; }
            set { strSuppress = value; }
        }
        private bool strSupPlayer = false;
        public bool StrSupPlayer
        {
            get { return strSupPlayer; }
            set { strSupPlayer = value; }
        }
        // string variables
        private string strSelect = string.Empty;
        public string StrSelect
        {
            get { return strSelect; }
            set { strSelect = value; }
        }
        private string strDirStor = string.Empty;
        public string StrDirStor
        {
            get { return strDirStor; }
            set { strDirStor = value; }
        }

        private string strIdentLabel = string.Empty;
        public string StrIdentLabel
        {
            get { return strIdentLabel; }
            set { strIdentLabel = value; }
        }
        private string strlabel1 = string.Empty;
        public string Strlabel1
        {
            get { return strlabel1; }
            set { strlabel1 = value; }
        }
        private string strlabel2 = string.Empty;
        public string Strlabel2
        {
            get { return strlabel2; }
            set { strlabel2 = value; }
        }
        private string stritem1 = string.Empty;
        public string Stritem1
        {
            get { return stritem1; }
            set { stritem1 = value; }
        }
        private string stritem2 = string.Empty;
        public string Stritem2
        {
            get { return stritem2; }
            set { stritem2 = value; }
        }
        private string stritem3 = string.Empty;
        public string Stritem3
        {
            get { return stritem3; }
            set { stritem3 = value; }
        }
        private string[] listSeparator = {"","","","",""};
        public string[] ListSeparator
        {
            get { return listSeparator; }
            set { listSeparator = value; }
        }
        private string[] roleSeparator = { "", "", "", "", "" };
        public string[] RoleSeparator
        {
            get { return roleSeparator; }
            set { roleSeparator = value; }
        }

        private string strDfltSelect = string.Empty;
        public string StrDfltSelect
        {
            get { return strDfltSelect; }
            set { strDfltSelect = value; }
        }
        private string strTitle1 = string.Empty;
        public string StrTitle1
        {
            get { return strTitle1; }
            set { strTitle1 = value; }
        }
        private string strTitle2 = string.Empty;
        public string StrTitle2
        {
            get { return strTitle2; }
            set { strTitle2 = value; }
        }
        private string strSTitle = string.Empty;
        public string StrSTitle
        {
            get { return strSTitle; }
            set { strSTitle = value; }
        }
        private string strTitleSelect = string.Empty;
        public string StrTitleSelect
        {
            get { return strTitleSelect; }
            set { strTitleSelect = value; }
        }
        private string strFilmSelect = string.Empty;
        public string StrFilmSelect
        {
            get { return strFilmSelect; }
            set { strFilmSelect = value; }
        }
        private string strSorta = string.Empty;
        public string StrSorta
        {
            get { return strSorta; }
            set { strSorta = value; }
        }
        private string strActors = string.Empty;
        public string StrActors
        {
            get { return strActors; }
            set { strActors = value; }
        }
        private string strTxtSelect = string.Empty;
        public string StrTxtSelect
        {
            get { return strTxtSelect; }
            set { strTxtSelect = value; }
        }
        private string strStorage = string.Empty;
        public string StrStorage
        {
            get { return strStorage; }
            set { strStorage = value; }
        }
        private string searchFile = "False";
        public string SearchFile
        {
            get { return searchFile; }
            set { searchFile = value; }
        }
        private string itemSearchFile = string.Empty;
        public string ItemSearchFile
        {
            get { return itemSearchFile; }
            set { itemSearchFile = value; }
        }
        private string searchSubDirs = "False";
        public string SearchSubDirs
        {
            get { return searchSubDirs; }
            set { searchSubDirs = value; }
        }
        private bool checkWatched = false;
        public bool CheckWatched
        {
            get { return checkWatched; }
            set { checkWatched = value; }
        }
       
        private string strIdentItem = string.Empty;
        public string StrIdentItem
        {
            get { return strIdentItem; }
            set { strIdentItem = value; }
        }
        private string[] strSort = {string.Empty,string.Empty};
        public string[] StrSort
        {
            get { return strSort; }
            set { strSort = value; }
        }
        private string[] strTSort = { string.Empty, string.Empty};
        public string[] StrTSort
        {
            get { return strTSort; }
            set { strTSort = value; }
        }
        private string[] strViewItem = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        public string[] StrViewItem
        {
            get { return strViewItem; }
            set { strViewItem = value; }
        }
        private string[] strViewText = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        public string[] StrViewText
        {
            get { return strViewText; }
            set { strViewText = value; }
        }
        private string[] strViewValue = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        public string[] StrViewValue
        {
            get { return strViewValue; }
            set { strViewValue = value; }
        }
        private string strViewDfltItem = string.Empty;
        public string StrViewDfltItem
        {
            get { return strViewDfltItem; }
            set { strViewDfltItem = value; }
        }
        private string strViewDfltText = string.Empty;
        public string StrViewDfltText
        {
            get { return strViewDfltText; }
            set { strViewDfltText = value; }
        }
        private string[] strSearchItem = {string.Empty,string.Empty};
        public string[] StrSearchItem
        {
            get { return strSearchItem; }
            set { strSearchItem = value; }
        }
        private string[] strSearchText= {string.Empty,string.Empty};
        public string[] StrSearchText
        {
            get { return strSearchText; }
            set { strSearchText = value; }
        }
        private string strFileXml = string.Empty;
        public string StrFileXml
        {
            get { return strFileXml; }
            set { strFileXml = value; }
        }
        private string strFileType = string.Empty;
        public string StrFileType
        {
            get { return strFileType; }
            set { strFileType = value; }
        }
        private string strMCCovers = string.Empty;
        public string StrMCCovers
        {
            get { return strMCCovers; }
            set { strMCCovers = value; }
        }
        private string strPathImg = string.Empty;
        public string StrPathImg
        {
            get { return strPathImg; }
            set { strPathImg = value; }
        }
        private string strPathFanart = string.Empty;
        public string StrPathFanart
        {
            get { return strPathFanart; }
            set { strPathFanart = value; }
        }
        private string strPathViews = string.Empty;
        public string StrPathViews
        {
            get { return strPathViews; }
            set { strPathViews = value; }
        }
        private string strSortSens = string.Empty;
        public string StrSortSens
        {
            get { return strSortSens; }
            set { strSortSens = value; }
        }
        private string wStrSort = string.Empty;
        public string WStrSort
        {
            get { return wStrSort; }
            set { wStrSort = value; }
        }
        private string wStrSelect = string.Empty;
        public string WStrSelect
        {
            get { return wStrSelect; }
            set { wStrSelect = value; }
        }
        private string wStrSortSens = string.Empty;
        public string WStrSortSens
        {
            get { return wStrSortSens; }
            set { wStrSortSens = value; }
        }
        private string wselectedlabel = string.Empty;
        public string Wselectedlabel
        {
            get { return wselectedlabel; }
            set { wselectedlabel = value; }
        }
        private string currentSortMethod = string.Empty;
        public string CurrentSortMethod
        {
            get { return currentSortMethod; }
            set { currentSortMethod = value; }
        }
        private string strTIndex = string.Empty;
        public string StrTIndex
        {
            get { return strTIndex; }
            set { strTIndex = value; }
        }
        private string fileImage = string.Empty;
        public string FileImage
        {
            get { return fileImage; }
            set { fileImage = value; }
        }
        private string strPluginName = string.Empty;
        public string StrPluginName
        {
            get { return strPluginName; }
            set { strPluginName = value; }
        }
        private string titleDelim = string.Empty;
        public string TitleDelim
        {
            get { return titleDelim; }
            set { titleDelim = value; }
        }
        private string wstar = string.Empty;
        public string Wstar
        {
            get { return wstar; }
            set { wstar = value; }
        }
        private string defaultcover = string.Empty;
        public string DefaultCover
        {
            get { return defaultcover; }
            set { defaultcover = value; }
        }
        private string cmdExe = string.Empty;
        public string CmdExe
        {
            get { return cmdExe; }
            set { cmdExe = value; }
        }
        private string cmdPar = string.Empty;
        public string CmdPar
        {
            get { return cmdPar; }
            set { cmdPar = value; }
        }
        private int strLayOut = int.MinValue;
        public int StrLayOut
        {
            get { return strLayOut; }
            set { strLayOut = value; }
        }
        private int lastID = int.MinValue;
        public int LastID
        {
            get { return lastID; }
            set { lastID = value; }
        }
        private decimal w_rating = decimal.Zero;
        public decimal W_rating
        {
            get { return w_rating; }
            set { w_rating = value; }
        }
        private int strIndex = int.MinValue;
        public int StrIndex
        {
            get { return strIndex; }
            set { strIndex = value; }
        }
        private int strPlayedIndex = int.MinValue;
        public int StrPlayedIndex
        {
            get { return strPlayedIndex; }
            set { strPlayedIndex = value; }
        }
        private bool strFanart = false;
        public bool StrFanart
        {
            get { return strFanart; }
            set { strFanart = value; }
        }
        private bool strFanartDflt = false;
        public bool StrFanartDflt
        {
            get { return strFanartDflt; }
            set { strFanartDflt = value; }
        }
        private bool strViews = false;
        public bool StrViews
        {
            get { return strViews; }
            set { strViews = value; }
        }
        private bool strViewsDflt = false;
        public bool StrViewsDflt
        {
            get { return strViewsDflt; }
            set { strViewsDflt = value; }
        }
        private bool strGrabber = false;
        public bool StrGrabber
        {
            get { return strGrabber; }
            set { strGrabber = value; }
        }
        private string strGrabber_cnf = string.Empty;
        public string StrGrabber_cnf
        {
            get { return strGrabber_cnf; }
            set { strGrabber_cnf = value; }
        }
        private string strGrabber_Dir = string.Empty;
        public string StrGrabber_Dir
        {
            get { return strGrabber_Dir; }
            set { strGrabber_Dir = value; }
        }
        private bool strGrabber_Always = false;
        public bool StrGrabber_Always
        {
            get { return strGrabber_Always; }
            set { strGrabber_Always = value; }
        }
        private bool strGrabber_ChooseScript = false;
        public bool StrGrabber_ChooseScript
        {
            get { return strGrabber_ChooseScript; }
            set { strGrabber_ChooseScript = value; }
        }
        private bool strAMCUpd = false;
        public bool StrAMCUpd
        {
            get { return strAMCUpd; }
            set { strAMCUpd = value; }
        }
        private string strAMCUpd_exe = string.Empty;
        public string StrAMCUpd_exe
        {
            get { return strAMCUpd_exe; }
            set { strAMCUpd_exe = value; }
        }
        private string strAMCUpd_cnf = string.Empty;
        public string StrAMCUpd_cnf
        {
            get { return strAMCUpd_cnf; }
            set { strAMCUpd_cnf = value; }
        }
        private string strSuppressPlayed = string.Empty;
        public string StrSuppressPlayed
        {
            get { return strSuppressPlayed; }
            set { strSuppressPlayed = value; }
        }
        private string strSuppressType = string.Empty;
        public string StrSuppressType
        {
            get { return strSuppressType; }
            set { strSuppressType = value; }
        }
        private string strSuppressField = string.Empty;
        public string StrSuppressField
        {
            get { return strSuppressField; }
            set { strSuppressField = value; }
        }
        private string strSuppressValue = string.Empty;
        public string StrSuppressValue
        {
            get { return strSuppressValue; }
            set { strSuppressValue = value; }
        }
        private string strPlayedDfltSelect = string.Empty;
        public string StrPlayedDfltSelect
        {
            get { return strPlayedDfltSelect; }
            set { strPlayedDfltSelect = value; }
        }
        private string strPlayedSelect = string.Empty;
        public string StrPlayedSelect
        {
            get { return strPlayedSelect; }
            set { strPlayedSelect = value; }
        }
        private string strPlayedSort = string.Empty;
        public string StrPlayedSort
        {
            get { return strPlayedSort; }
            set { strPlayedSort = value; }
        }
        private string strPlayedSens = string.Empty;
        public string StrPlayedSens
        {
            get { return strPlayedSens; }
            set { strPlayedSens = value; }
        }
        List<string[]> movieList = new List<string[]>();
        public List<string[]> MovieList
        {
            get { return movieList; }
            set { movieList = value; }
        }


        public static void SaveConfiguration(string currentConfig, int selectedItem, string selectedItemLabel)
        {
            XmlConfig XmlConfig = new XmlConfig();
            XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Current_Config", currentConfig);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSelect", MesFilms.conf.StrSelect.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrActors", MesFilms.conf.StrActors.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrTitleSelect", MesFilms.conf.StrTitleSelect.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrFilmSelect", MesFilms.conf.StrFilmSelect.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrDfltSelect", MesFilms.conf.StrDfltSelect.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSort", MesFilms.conf.StrSorta.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "CurrentSortMethod", MesFilms.conf.CurrentSortMethod.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSortSens", MesFilms.conf.StrSortSens.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Selection", MesFilms.conf.StrTxtSelect);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "IndexItem", (selectedItem > -1) ? ((MesFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItem.ToString()) : "-1"); //may need to check if there is no item selected and so save -1
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "TitleItem", (selectedItem > -1) ? ((MesFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItemLabel.ToString()) : ""); //may need to check if there is no item selected and so save ""
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Boolselect", MesFilms.conf.Boolselect);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Boolreturn", MesFilms.conf.Boolreturn);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Boolview", MesFilms.conf.Boolview);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "WSelectedLabel", MesFilms.conf.Wselectedlabel);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "WStrSort", MesFilms.conf.WStrSort);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Wstar", MesFilms.conf.Wstar);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "WLayOut", MesFilms.conf.StrLayOut);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "LastID", MesFilms.conf.LastID);
                switch (MesFilms.conf.StrFileType)
                {
                    case "0":
                        break;
                    case "1":
                    XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", MesFilms.conf.StrFileXml.ToString());
                        break;
                }
        }
        //--------------------------------------------------------------------------------------------
        //  Control Acces to asked configuration
        //--------------------------------------------------------------------------------------------
        public static string Control_Access_Config(string configname, int GetID)
        {
            if (configname.Length == 0)
                return "";
            string Dwp;
//            using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
//            {
            XmlConfig XmlConfig = new XmlConfig();    
            Dwp = XmlConfig.ReadXmlConfig("MyFilms", configname, "Dwp", "");
//            }

            if (Dwp.Length == 0)
                return configname;
            MediaPortal.Dialogs.VirtualKeyboard keyboard = (MediaPortal.Dialogs.VirtualKeyboard)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            if (null == keyboard) return "";
            keyboard.Reset();
            keyboard.Text = "";
            keyboard.Password = true;
            keyboard.DoModal(GetID);
            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
            {
                Crypto crypto = new Crypto();
                if (crypto.Decrypter(Dwp) == keyboard.Text)
                    return configname;
            }
            return "";
        }
        //--------------------------------------------------------------------------------------------
        //  Choice Configuration
        //--------------------------------------------------------------------------------------------
        public static string Choice_Config(int GetID)
        {
            MediaPortal.Dialogs.GUIDialogMenu dlg = (MediaPortal.Dialogs.GUIDialogMenu)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null)
            {
                
                MesFilms.conf.StrFileXml = "";
                return "";
            }
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
//            using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
//            {
            XmlConfig XmlConfig = new XmlConfig();
            int MesFilms_nb_config = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
            for (int i = 0; i < (int)MesFilms_nb_config; i++)
                dlg.Add(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, ""));
//            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
            {
                MesFilms.conf.StrFileXml = "";
                return "";
            }
            if (dlg.SelectedLabelText.Length > 0)
                return dlg.SelectedLabelText;
            return "";
        }
        //--------------------------------------------------------------------------------------------
        //  Return Current Configuration
        //--------------------------------------------------------------------------------------------
        public static void Current_Config()
        {
            CurrentConfig = null;
//            using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
//            {
            XmlConfig XmlConfig = new XmlConfig();
            NbConfig = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", 0);
            if (NbConfig == 0)
            {
                MediaPortal.Dialogs.GUIDialogOK dlgOk = (MediaPortal.Dialogs.GUIDialogOK)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                dlgOk.SetLine(1, "No Configuration defined");
                dlgOk.SetLine(2, "Please enter setup first");
                dlgOk.DoModal(MesFilms.ID_MesFilms);
                MediaPortal.GUI.Library.GUIWindowManager.ShowPreviousWindow();
            }
            bool boolchoice = true;
            if (CurrentConfig == null)
                CurrentConfig = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Current_Config", "");
            if (!(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Menu_Config", false)))
                CurrentConfig = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", "");
            else
                CurrentConfig = "";
            if (CurrentConfig == "")
            {
                boolchoice = false;
                CurrentConfig = Configuration.Choice_Config(MesFilms.ID_MesFilms); // "" => user esc's dialog on plugin startup so exit plugin unchanged
            }
            CurrentConfig = Configuration.Control_Access_Config(CurrentConfig, MesFilms.ID_MesFilms);
            if ((CurrentConfig == "") && (NbConfig > 1) && (boolchoice)) //error password ? so if many config => choice config menu
                CurrentConfig = Configuration.Choice_Config(MesFilms.ID_MesFilms);
//            }
        }
    }

}
