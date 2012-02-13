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

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Globalization;

  using MediaPortal.Configuration;

  using MyFilmsPlugin.DataBase;
  using MyFilmsPlugin.MyFilms;

  using MyFilmsPlugin.MyFilms.CatalogConverter;
  using MyFilmsPlugin.MyFilms.Utils;
  using MyFilmsPlugin.MyFilmsGUI;

  public class Configuration
    {
        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

        public enum CatalogType
        {
          AntMovieCatalog3 = 0,
          DVDProfiler = 1,
          MovieCollector = 2,
          MyMovies = 3,
          EaxMovieCatalog2 = 4,
          EaxMovieCatalog3 = 5,
          PersonalVideoDatabase = 6,
          eXtremeMovieManager  = 7,
          XBMC = 8,
          MovingPicturesXML = 9,
          AntMovieCatalog4Xtended = 10,
          XBMCnfoReader = 11
        }

        private bool IsExternalCatalog(CatalogType type)
        {
          return (StrFileType != CatalogType.AntMovieCatalog3 && StrFileType != CatalogType.AntMovieCatalog4Xtended);
        }

        public Configuration(string CurrentConfig, bool setcurrentconfig, bool create_temp, LoadParameterInfo loadParams)
        {
            //-----------------------------------------------------------------------------------------------
            //   Load Config Parameters in MyFilms.xml file (section CurrentConfig)
            //-----------------------------------------------------------------------------------------------

            //LogMyFilms.Debug("MFC: Configuration loading started for '" + CurrentConfig + "'"); 
            //if (setcurrentconfig)
            //{
            //  XmlConfig xmlConfigSave = new XmlConfig();
            //  xmlConfigSave.WriteXmlConfig("MyFilms", "MyFilms", "Current_Config", CurrentConfig);
            //  // the xmlwriter caused late update on the file when leaving MP, thus overwriting MyFilms.xml and moving changes to MyFilms.bak !!! -> We write directly only!
            //  //using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
            //  //{
            //  //  xmlwriter.SetValue("MyFilms", "Current_Config", CurrentConfig);
            //  //  xmlwriter.Dispose();
            //  //}
            //  //using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
            //}
            using (XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true)) // true = cached !
            {
                if (setcurrentconfig)
                {
                  XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Current_Config", CurrentConfig);
                  // XmlSettings.SaveCache(); // ToDo: Debug, if it is required here !
                }
                #region read xml data
                AlwaysShowConfigMenu = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Menu_Config", false);
                StrStorage = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntStorage", string.Empty);
                StrDirStor = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "PathStorage", string.Empty);
                StrStorageTrailer = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntStorageTrailer", string.Empty);
                StrDirStorTrailer = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "PathStorageTrailer", string.Empty);
                StrDirStorActorThumbs = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "PathStorageActorThumbs", string.Empty);
                SearchFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SearchFileName", "False");
                SearchFileTrailer = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SearchFileNameTrailer", "False");
                ItemSearchFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ItemSearchFileName", string.Empty);
                ItemSearchGrabber = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ItemSearchGrabberName", string.Empty);
                ItemSearchGrabberScriptsFilter = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ItemSearchGrabberScriptsFilter", string.Empty);
                GrabberOverrideLanguage = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "GrabberOverrideLanguage", string.Empty);
                GrabberOverridePersonLimit = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "GrabberOverridePersonLimit", string.Empty);
                GrabberOverrideTitleLimit = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "GrabberOverrideTitleLimit", string.Empty);
                GrabberOverrideGetRoles = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "GrabberOverrideGetRoles", string.Empty);
                PictureHandling = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "PictureHandling", string.Empty);
                ItemSearchFileTrailer = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ItemSearchFileNameTrailer", string.Empty);
                SearchSubDirs = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SearchSubDirs", "No");
                SearchOnlyExactMatches = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SearchOnlyExactMatches", "No");
                SearchSubDirsTrailer = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SearchSubDirsTrailer", "No");
                CheckWatched = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CheckWatched", false);
                CheckWatchedPlayerStopped = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CheckWatchedPlayerStopped", false);
                StrIdentItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntIdentItem", string.Empty);
                StrTitle1 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntTitle1", string.Empty);
                StrTitle2 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntTitle2", string.Empty);
                StrSTitle = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntSTitle", string.Empty);
                if (StrSTitle == string.Empty) StrSTitle = StrTitle1;

                StrViewDfltItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ViewDfltItem", string.Empty);
                if (loadParams != null && !string.IsNullOrEmpty(loadParams.View)) StrViewDfltItem = loadParams.View;

                StrViewDfltText = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ViewDfltText", string.Empty);
                if (loadParams != null && !string.IsNullOrEmpty(loadParams.ViewValue)) StrViewDfltText = loadParams.ViewValue;

                //for (int i = 1; i < 6; i++)
                //{
                //  StrViewItem[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewItem{0}", i), string.Empty);
                //  StrViewText[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewText{0}", i), string.Empty);
                //  //if (StrViewText[i - 1].ToLower() == wViewDfltItem.ToLower()) StrViewDfltItem = StrViewItem[i - 1];
                //  StrViewValue[i - 1] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewValue{0}", i), string.Empty);
                //}


                int iCustomViews = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntViewTotalCount", 5); // set to 5 as default to keep compatibility
                CustomViews.View.Clear();
                int index = 1;
                for (int i = 1; i < iCustomViews + 1; i++) // while (true) //
                {
                  if (string.IsNullOrEmpty(XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntViewText" + index, string.Empty)))
                    break;
                  MFview.ViewRow view = CustomViews.View.NewViewRow();
                  view.Label = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewText{0}", index), string.Empty);
                  view.ViewEnabled = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewEnabled{0}", index), true);
                  view.ImagePath = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewImagePath{0}", index), string.Empty);
                  view.DBfield = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewItem{0}", index), string.Empty);
                  view.Value = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewValue{0}", index), string.Empty);
                  view.Filter = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewFilter{0}", index), string.Empty);
                  view.Index = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewIndex{0}", index), 0);
                  view.SortFieldViewType = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewSortFieldViewType{0}", index), "Name");
                  view.SortDirectionView = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewSortDirectionView{0}", index), " ASC");
                  if (view.SortDirectionView.Contains("ASC")) view.SortDirectionView = " ASC"; else view.SortDirectionView = " DESC";
                  view.LayoutView = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, string.Format("AntViewLayoutView{0}", index), "0");
                  LogMyFilms.Debug("Adding view - #: '" + index + "', DBitem: '" + view.DBfield + "', View Label: '" + view.Label + "'");
                  CustomViews.View.AddViewRow(view);
                  index++;
                }
              
                StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalog", string.Empty);
                StrFileXmlTemp = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", string.Empty);
                StrFileType = (CatalogType)int.Parse(XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CatalogType", "0"));
                StrPathImg = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntPicture", string.Empty);
                StrArtistDflt = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ArtistDflt", false);
                StrPathFanart = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "FanartPicture", string.Empty);
                StrPathViews = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ViewsPicture", string.Empty);
                StrPathArtist = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ArtistPicturePath", string.Empty);

                StrLayOut = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "LayOut", 0);
                WStrLayOut = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "WLayOut", StrLayOut);
                StrLayOutInHierarchies = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "LayOutInHierarchies", StrLayOut);

                if (loadParams != null && !string.IsNullOrEmpty(loadParams.Layout))
                {
                  int iLayout = 0;
                  if (int.TryParse(loadParams.Layout, out iLayout)) 
                    if (iLayout >= 0 && iLayout <= 4)
                    {
                      StrLayOut = iLayout;
                      WStrLayOut = iLayout;
                      StrLayOutInHierarchies = iLayout;
                    }
                }

                Strlabel1 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntLabel1", string.Empty);
                Strlabel2 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntLabel2", string.Empty);
                Strlabel3 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntLabel3", string.Empty);
                Strlabel4 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntLabel4", string.Empty);
                Strlabel5 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntLabel5", string.Empty);
                Stritem1 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntItem1", string.Empty);
                Stritem2 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntItem2", string.Empty);
                Stritem3 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntItem3", string.Empty);
                Stritem4 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntItem4", string.Empty);
                Stritem5 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntItem5", string.Empty);
                StrIdentLabel = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntIdentLabel", string.Empty);
                StrLogos = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Logos", false);
                StrSuppress = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Suppress", false);
                StrSuppressManual = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SuppressManual", false);
                StrSupPlayer = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SuppressPlayed", false);
                StrSuppressType = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SuppressType", string.Empty);
                StrWatchedField = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "WatchedField", "Checked"); // Defaults to "Checked", if no value set, as it's most used in ANT like that
                StrSuppressField = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SuppressField", string.Empty);
                StrSuppressValue = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SuppressValue", string.Empty);

                StrEnhancedWatchedStatusHandling = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "EnhancedWatchedStatusHandling", false);
                StrUserProfileName = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "UserProfileName", "Global");

                StrECoptionStoreTaglineInDescription = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ECoptionStoreTaglineInDescription", false);
                // Common EC options
                bool addTagline = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ECoptionAddTagline", false);
                bool addTags = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ECoptionAddTags", false);
                bool addCertification = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ECoptionAddCertification", false);
                bool addWriter = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ECoptionAddWriter", false);
                string DestinationTagline = "";
                string DestinationTags = "";
                string DestinationCertification = "";
                string DestinationWriter = "";

                if (IsExternalCatalog(StrFileType))
                {
                  if (addTagline) DestinationTagline = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ECoptionAddDestinationTagline", "");
                  else DestinationTagline = "";
                  if (addTags) DestinationTags = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ECoptionAddDestinationTags", "");
                  else DestinationTags = "";
                  if (addCertification) DestinationCertification = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ECoptionAddDestinationCertification", "");
                  else DestinationCertification = "";
                  if (addWriter) DestinationWriter = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ECoptionAddDestinationWriter", "");
                  else DestinationWriter = "";
                }
                // LogMyFilms.Debug("MFC: switch (StrFileType) '" + StrFileType.ToString() + "'");

                ReadOnly = IsExternalCatalog(StrFileType);

                #region Catalog Specific Settings ...
                switch (StrFileType)
                {
                    case CatalogType.AntMovieCatalog3:
                    case CatalogType.AntMovieCatalog4Xtended:
                      break;
                    case CatalogType.DVDProfiler:
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool OnlyFile = false;
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            string TagField = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "DVDPTagField", string.Empty);
                            DvdProfiler cv = new DvdProfiler(TagField);
                            StrFileXml = cv.ConvertProfiler(StrFileXml, StrPathImg, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, TagField, OnlyFile);
                        }
                        else
                          StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", string.Empty);
                        break;
                    case CatalogType.MovieCollector:
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool OnlyFile = false;
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            MovieCollector mc = new MovieCollector();
                            StrFileXml = mc.ConvertMovieCollector(StrFileXml, StrPathImg, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, OnlyFile, TitleDelim);
                        }
                        else
                          StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", string.Empty);
                        break;
                    case CatalogType.MyMovies:
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool OnlyFile = false;
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            MyMovies mm = new MyMovies();
                            StrFileXml = mm.ConvertMyMovies(StrFileXml, StrPathImg, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, OnlyFile);
                        }
                        else
                          StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", string.Empty);
                        break;
                    case CatalogType.EaxMovieCatalog2:
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool OnlyFile = false;
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            EaxMovieCatalog emc = new EaxMovieCatalog();
                            StrFileXml = emc.ConvertEaxMovieCatalog(StrFileXml, StrPathImg, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, OnlyFile, TitleDelim);
                        }
                        else
                          StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", string.Empty);
                        break;
                    case CatalogType.EaxMovieCatalog3:
                        if (create_temp)
                        {
                          string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                          string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                          if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                          {
                            StrFileXml = destFile;
                            break;
                          }
                          bool OnlyFile = false;
                          OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                          EaxMovieCatalog3 emc3 = new EaxMovieCatalog3();
                          StrFileXml = emc3.ConvertEaxMovieCatalog3(StrFileXml, StrPathImg, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, OnlyFile, TitleDelim);
                        }
                        else
                          StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", string.Empty);
                        break;
                    case CatalogType.PersonalVideoDatabase:
                        if (create_temp)
                        {
                          string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                          string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                          if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                          {
                            StrFileXml = destFile;
                            break;
                          }
                          bool OnlyFile = false;
                          OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                          PersonalVideoDatabase pvd = new PersonalVideoDatabase();
                          StrFileXml = pvd.ConvertPersonalVideoDatabase(StrFileXml, StrPathImg, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, OnlyFile, TitleDelim, StrECoptionStoreTaglineInDescription);
                        }
                        else
                          StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", string.Empty);
                        break;
                    case CatalogType.eXtremeMovieManager:
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool OnlyFile = false;
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            XMM xmm = new XMM();
                            StrFileXml = xmm.ConvertXMM(StrFileXml, StrPathImg, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, OnlyFile);
                        }
                        else
                          StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", string.Empty);
                        break;
                    case CatalogType.XBMC: // XBMC fulldb export (all movies in one DB)
                        if (create_temp)
                        {
                            string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                            string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                            {
                                StrFileXml = destFile;
                                break;
                            }
                            bool OnlyFile = false;
                            OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                            XbmcNfo nfo = new XbmcNfo();
                            StrFileXml = nfo.ConvertXbmcNfo(StrFileXml, StrPathImg, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, StrStorage, OnlyFile, TitleDelim);
                        }
                        else
                          StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", string.Empty);
                        break;
                    case CatalogType.MovingPicturesXML:
                        if (create_temp)
                        {
                          string WStrPath = System.IO.Path.GetDirectoryName(StrFileXml);
                          string destFile = WStrPath + "\\" + StrFileXml.Substring(StrFileXml.LastIndexOf(@"\") + 1, StrFileXml.Length - StrFileXml.LastIndexOf(@"\") - 5) + "_tmp.xml";
                          if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(StrFileXml))))
                          {
                            StrFileXml = destFile;
                            break;
                          }
                          bool OnlyFile = false;
                          OnlyFile = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyFile", false);
                          MovingPicturesXML mopi = new MovingPicturesXML();
                          StrFileXml = mopi.ConvertMovingPicturesXML(StrFileXml, StrPathImg, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, OnlyFile);
                        }
                        else
                          StrFileXml = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntCatalogTemp", string.Empty);
                        break;
                    case CatalogType.XBMCnfoReader:
                        break;
                }
                #endregion

                StrSelect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSelect", string.Empty);
                StrPersons = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrPersons", string.Empty);
                StrTitleSelect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrTitleSelect", string.Empty);
                StrFilmSelect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrFilmSelect", string.Empty);
                StrDfltSelect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrDfltSelect", string.Empty);
                StrViewSelect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrViewSelect", string.Empty);

                StrSorta = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSort", string.Empty);
                CurrentSortMethod = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CurrentSortMethod", string.Empty);
                StrSortSens = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSortSens", string.Empty);
                StrSortaInHierarchies = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSortInHierarchies", string.Empty);
                CurrentSortMethodInHierarchies = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CurrentSortMethodInHierarchies", string.Empty);
                StrSortSensInHierarchies = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSortSensInHierarchies", string.Empty);
                string wDfltSort = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSort", string.Empty);
                string wDfltSortSens = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSortSens", string.Empty);
                string wDfltSortMethod = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltSortMethod", string.Empty);

                string wDfltSortInHierarchies = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSortInHierarchies", string.Empty); // InHierarchies
                string wDfltSortSensInHierarchies = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSortSensInHierarchies", string.Empty);
                string wDfltSortMethodInHierarchies = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltSortMethodInHierarchies", string.Empty);

                StartView.InitDefaults();
                CurrentView.InitDefaults();

                // ViewManager.CurrentSettings.FilmsSortItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSort", string.Empty);
                CurrentView.FilmsSortItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSort", string.Empty);
                CurrentView.FilmsSortItemFriendlyName = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CurrentSortMethod", string.Empty);
                CurrentView.FilmsSortDirection = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSortSens", string.Empty);

                CurrentView.HierarchySortItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSortInHierarchies", string.Empty);
                CurrentView.HierarchySortItemFriendlyName = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CurrentSortMethodInHierarchies", string.Empty);
                CurrentView.HierarchySortDirection = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "StrSortSensInHierarchies", string.Empty);

                StartView.FilmsSortItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSort", string.Empty);
                StartView.FilmsSortItemFriendlyName = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltSortMethod", string.Empty);
                StartView.FilmsSortDirection = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSortSens", string.Empty);

                StartView.HierarchySortItem = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSortInHierarchies", string.Empty); // InHierarchies
                StartView.HierarchySortItemFriendlyName = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltStrSortSensInHierarchies", string.Empty);
                StartView.HierarchySortDirection = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntDfltSortMethodInHierarchies", string.Empty);

                string startview = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CurrentView", string.Empty);
                if (!string.IsNullOrEmpty(startview)) StartView.LoadFromString(startview);

                ViewContext = (MyFilms.ViewContext)Enum.Parse(typeof(MyFilms.ViewContext), XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ViewContext", Enum.GetName(typeof(MyFilms.ViewContext), MyFilms.ViewContext.None)));
                StrTxtView = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "View", string.Empty);
                StrTxtSelect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Selection", string.Empty);
                try { StrIndex = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "IndexItem", -1); }
                catch { StrIndex = -1; }
                StrTIndex = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "TitleItem", string.Empty);
                Boolselect = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Boolselect", false);
                Boolreturn = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Boolreturn", false);
                Boolindexed = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Boolindexed", false);
                Boolindexedreturn = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Boolindexedreturn", false);
                IndexedChars = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "IndexedChars", 0);
                BoolReverseNames =  XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ReversePersonNames", false);
                WStrSort = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "WStrSort", string.Empty);
                Wselectedlabel = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "WSelectedLabel", string.Empty);
                Wstar = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Wstar", string.Empty);
                LastID = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "LastID", -1);
                TitleDelim = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "TitleDelim", ".");
                DefaultCover = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "DefaultCover", string.Empty);
                DefaultCoverArtist = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "DefaultCoverArtist", string.Empty);
                DefaultCoverViews = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "DefaultCoverViews", string.Empty);
                DefaultFanartImage = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "DefaultFanartImage", string.Empty);
                StrAntFilterMinRating = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AntFilterMinRating", "5");
                StrGrabber_cnf = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Grabber_cnf", string.Empty);
                StrPicturePrefix = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "PicturePrefix", string.Empty);
                StrGrabber_Always = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Grabber_Always", false);
                StrGrabber_ChooseScript = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Grabber_ChooseScript", false);
                StrAMCUpd = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AMCUpd", false);
                StrAMCUpd_cnf = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AMCUpd_cnf", string.Empty);
                StrFanart = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Fanart", false);
                StrFanartDefaultViews = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "FanartDefaultViews", false);
                StrFanartDefaultViewsUseRandom = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "FanartDefaultViewsUseRandom", false);
                StrFanartDflt = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "FanartDflt", false);
                StrFanartDfltImage = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "FanartDfltImage", false);
                StrFanartDfltImageAll = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "FanartDfltImageAll", false);
                UseThumbsForViews = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Views", false);
                UseThumbsForPersons = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "Persons", false);
                StrViewsDflt = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ViewsDflt", false);
                StrViewsDfltAll = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ViewsDfltAll", false);
                StrCheckWOLenable = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "WOL-Enable", false);
                StrWOLtimeout = Convert.ToInt16(XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "WOL-Timeout", "15"));
                StrCheckWOLuserdialog = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "WOL-Userdialog", false);
                StrNasName1 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "NAS-Name-1", string.Empty);
                StrNasMAC1 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "NAS-MAC-1", string.Empty);
                StrNasName2 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "NAS-Name-2", string.Empty);
                StrNasMAC2 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "NAS-MAC-2", string.Empty);
                StrNasName3 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "NAS-Name-3", string.Empty);
                StrNasMAC3 = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "NAS-MAC-3", string.Empty);

                //// read states vars for each possible view - do we also need states for "userdefined views"?  
                //DataBase.AntMovieCatalog ds = new DataBase.AntMovieCatalog();
                //MyFilms.conf.CustomView.Clear();
                //foreach (DataColumn dc in ds.Movie.Columns)
                //{
                //  View tView = new View();
                //  string viewstateName = "ViewState-" + dc.ColumnName;
                //  string viewstate = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, viewstateName, string.Empty);
                //  if (!string.IsNullOrEmpty(viewstate))
                //  {
                //    tView.LoadFromString(viewstate);
                //    MyFilms.conf.CustomView.Add(tView);
                //  }
                //  else
                //  {
                //    tView.ViewDBItem = dc.ColumnName;
                //    CustomView.Add(tView);
                //  }
                //}
              
                StrSearchHistory = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "SearchHistory", string.Empty);
                MyFilms.SearchHistory.Clear();
                foreach (string s in StrSearchHistory.Split('|'))
                {
                  if (!string.IsNullOrEmpty(s.Trim())) 
                    MyFilms.SearchHistory.Add(s);
                }

                int j = 0;
                for (int i = 1; i <= 5; i++)
                {
                  if (XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ListSeparator" + i, string.Empty).Length > 0)
                    {
                      listSeparator[j] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ListSeparator" + i, string.Empty);
                        j++;
                    }
                }
                j = 0;
                for (int i = 1; i <= 5; i++)
                {
                  if (XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "RoleSeparator" + i, string.Empty).Length > 0)
                    {
                      RoleSeparator[j] = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "RoleSeparator" + i, string.Empty);
                        j++;
                    }
                }

                CmdExe = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CmdExe", string.Empty);
                if (CmdExe == "(none)") CmdExe = "";
                CmdPar = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CmdPar", string.Empty);
                if (CmdPar == "(none)") CmdPar = "";
                OnlyTitleList = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "OnlyTitleList", false);
                GlobalAvailableOnly = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "GlobalAvailableOnly", false);
                GlobalUnwatchedOnly = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "GlobalUnwatchedOnly", false);
                GlobalUnwatchedOnlyValue = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "GlobalUnwatchedOnlyValue", "false");

                ScanMediaOnStart = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "CheckMediaOnStart", false);
                BoolShowEmptyValuesInViews = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "ShowEmpty", false);
                AllowTraktSync = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AllowTraktSync", false);
                AllowRecentlyAddedAPI = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AllowRecentAddedAPI", false);

                AlwaysDefaultView = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "AlwaysDefaultView", false);
                if ((AlwaysDefaultView) || (MyFilms.InitialStart) || (loadParams != null && (!string.IsNullOrEmpty(loadParams.View) || !string.IsNullOrEmpty(loadParams.MovieID))))
                {
                  viewContext = MyFilms.ViewContext.StartView;
                  strIndex = -1;
                  LastID = -1;
                  Wstar = "";
                  Boolreturn = false;
                  Boolselect = true;
                  Boolindexed = false;
                  Boolindexedreturn = false;

                  Wselectedlabel = StrViewDfltText;
                  if (loadParams == null || string.IsNullOrEmpty(loadParams.Layout))
                  {
                    StrLayOut = XmlConfig.ReadXmlConfig("MyFilms", CurrentConfig, "LayOut", StrLayOut);
                    StrLayOutInHierarchies = StrLayOut;
                    this.WStrLayOut = StrLayOut;
                  }
                  if (Helper.FieldIsSet(wDfltSort))
                  {
                    StrSorta = wDfltSort;
                    StrSortSens = wDfltSortSens;
                    CurrentSortMethod = wDfltSortMethod;
                  }
                  if (Helper.FieldIsSet(wDfltSortInHierarchies)) // hierarchies sort settings
                  {
                    StrSortaInHierarchies = wDfltSortInHierarchies;
                    StrSortSensInHierarchies = wDfltSortSensInHierarchies;
                    CurrentSortMethodInHierarchies = wDfltSortMethodInHierarchies;
                  }
                }
              XmlConfig.Dispose();
              #endregion
            } // End reading config

            #region check and correct settings
            if (string.IsNullOrEmpty(CurrentSortMethod)) CurrentSortMethod = GUILocalizeStrings.Get(103);
            if (string.IsNullOrEmpty(CurrentSortMethodInHierarchies)) CurrentSortMethodInHierarchies = CurrentSortMethod;
          
            // if (string.IsNullOrEmpty(StrSelect)) StrSelect = StrTitle1 + " not like ''";

            if (string.IsNullOrEmpty(StrSorta)) StrSorta = StrSTitle;
            if (string.IsNullOrEmpty(StrSortaInHierarchies)) StrSortaInHierarchies = StrSTitle;

            if (string.IsNullOrEmpty(StrSortSens) || StrSortSens == "ASC") StrSortSens = " ASC";
            if (StrSortSens == "DESC") StrSortSens = " DESC";

            if (string.IsNullOrEmpty(StrSortSensInHierarchies) || StrSortSensInHierarchies == "ASC") StrSortSensInHierarchies = " ASC";
            if (StrSortSensInHierarchies == "DESC") StrSortSensInHierarchies = " DESC";

            if (string.IsNullOrEmpty(WStrSortSens) || WStrSortSens == "ASC") WStrSortSens = " ASC";
            if (WStrSortSens == "DESC") WStrSortSens = " DESC";

            if (string.IsNullOrEmpty(WStrSortSensCount) || WStrSortSensCount == "DESC") WStrSortSensCount = " DESC";
            if (WStrSortSensCount == "ASC") WStrSortSensCount = " ASC";

            if (StrFanart)
                if (!(StrPathFanart.Length > 0 && System.IO.Directory.Exists(StrPathFanart)))
                {
                  LogMyFilms.Info("MyFilms : Fanart Path '" + StrPathFanart + "', doesn't exist. Fanart disabled ! ");
                  StrFanart = false;
                }
            if (StrArtist)
                if (!(StrPathArtist.Length > 0 && System.IO.Directory.Exists(StrPathArtist)))
                {
                    LogMyFilms.Info("MyFilms : Artist Path '" + StrPathArtist + "', doesn't exist. Artist Pictures disabled ! ");
                    StrArtist = false;
                }
            #endregion
          // LogMyFilms.Debug("MFC: Configuration loading ended for '" + CurrentConfig + "'");
        }

#region Getter/Setter - static values

        private static string currentConfig = string.Empty;
        public static string CurrentConfig
        {
          get { return currentConfig; }
          set { currentConfig = value; }
        }
        private static MFView viewManager = new MFView();
        public static MFView ViewManager
        {
          get { return viewManager; }
          set { viewManager = value; }
        }
        private static View startView = new View();
        public View StartView
        {
          get { return startView; }
          set { startView = value; }
        }
        private static View currentView = new View();
        public View CurrentView
        {
          get { return currentView; }
          set { currentView = value; }
        }
        private static string pluginMode = string.Empty;
        public static string PluginMode
        {
          get { return pluginMode; }
          set { pluginMode = value; }
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
        private bool alwaysShowConfigMenu = false;
        public bool AlwaysShowConfigMenu
        {
          get { return alwaysShowConfigMenu; }
          set { alwaysShowConfigMenu = value; }
        }

        private MyFilms.ViewContext viewContext = MyFilms.ViewContext.None;
        public MyFilms.ViewContext ViewContext
        {
            get { return viewContext; }
            set { viewContext = value; }
        }
        private int menuSelectedID = -1;
        public int MenuSelectedID
        {
          get { return menuSelectedID; }
          set { menuSelectedID = value; }
        }
        private bool boolMenuShowAll = false;
        public bool BoolMenuShowAll
        {
          get { return boolMenuShowAll; }
          set { boolMenuShowAll = value; }
        }
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
        private bool boolindexed = false;
        public bool Boolindexed
        {
          get { return boolindexed; }
          set { boolindexed = value; }
        }
        private int indexedChars = 0;
        public int IndexedChars
        {
          get { return indexedChars; }
          set { indexedChars = value; }
        }
        private bool boolindexedreturn = false;
        public bool Boolindexedreturn
        {
          get { return boolindexedreturn; }
          set { boolindexedreturn = value; }
        }
        private bool boolCollection; // state variable to indicate, if the current browse position is a movie collection (used for sort by actions)
        public bool BoolCollection
        {
          get { return boolCollection; }
          set { boolCollection = value; }
        }
        private bool alwaysDefaultView = false;
        public bool AlwaysDefaultView
        {
            get { return alwaysDefaultView; }
            set { alwaysDefaultView = value; }
        }
        private bool globalAvailableOnly = false;
        public bool GlobalAvailableOnly
        {
          get { return globalAvailableOnly; }
          set { globalAvailableOnly = value; }
        }
        private bool globalUnwatchedOnly = false;
        public bool GlobalUnwatchedOnly
        {
          get { return globalUnwatchedOnly; }
          set { globalUnwatchedOnly = value; }
        }
        private string globalUnwatchedOnlyValue;
        public string GlobalUnwatchedOnlyValue
        {
          get { return globalUnwatchedOnlyValue; }
          set { globalUnwatchedOnlyValue = value; }
        }
        private bool scanMediaOnStart = false;
        public bool ScanMediaOnStart
        {
          get { return scanMediaOnStart; }
          set { scanMediaOnStart = value; }
        }
        private bool allowTraktSync = false;
        public bool AllowTraktSync
        {
          get { return allowTraktSync; }
          set { allowTraktSync = value; }
        }
        private bool allowRecentlyAddedAPI = false;
        public bool AllowRecentlyAddedAPI
        {
          get { return allowRecentlyAddedAPI; }
          set { allowRecentlyAddedAPI = value; }
        }
        private bool onlyTitleList = false;
        public bool OnlyTitleList
        {
            get { return onlyTitleList; }
            set { onlyTitleList = value; }
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

        private bool strSuppressManual = false;
        public bool StrSuppressManual
        {
          get { return strSuppressManual; }
          set { strSuppressManual = value; }
        }

        private bool strEnhancedWatchedStatusHandling = false;
        public bool StrEnhancedWatchedStatusHandling
        {
            get { return strEnhancedWatchedStatusHandling; }
            set { strEnhancedWatchedStatusHandling = value; }
        }

        private string strUserProfileName = string.Empty;
        public string StrUserProfileName
        {
            get { return strUserProfileName; }
            set { strUserProfileName = value; }
        }
    
        private bool strECoptionStoreTaglineInDescription = false;
        public bool StrECoptionStoreTaglineInDescription
        {
            get { return strECoptionStoreTaglineInDescription; }
            set { strECoptionStoreTaglineInDescription = value; }
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

        private string strDirStorTrailer = string.Empty;
        public string StrDirStorTrailer
        {
            get { return strDirStorTrailer; }
            set { strDirStorTrailer = value; }
        }

        private string strDirStorActorThumbs = string.Empty;
        public string StrDirStorActorThumbs
        {
            get { return strDirStorActorThumbs; }
            set { strDirStorActorThumbs = value; }
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
        private string strlabel3 = string.Empty;
        public string Strlabel3
        {
            get { return strlabel3; }
            set { strlabel3 = value; }
        }
        private string strlabel4 = string.Empty;
        public string Strlabel4
        {
          get { return strlabel4; }
          set { strlabel4 = value; }
        }
        private string strlabel5 = string.Empty;
        public string Strlabel5
        {
          get { return strlabel5; }
          set { strlabel5 = value; }
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
        private string stritem4 = string.Empty;
        public string Stritem4
        {
          get { return stritem4; }
          set { stritem4 = value; }
        }
        private string stritem5 = string.Empty;
        public string Stritem5
        {
          get { return stritem5; }
          set { stritem5 = value; }
        }
        private string[] listSeparator = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        public string[] ListSeparator
        {
            get { return listSeparator; }
            set { listSeparator = value; }
        }
        private string[] roleSeparator = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
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
        private string strViewSelect = string.Empty;
        public string StrViewSelect
        {
          get { return strViewSelect; }
          set { strViewSelect = value; }
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
        private string strSortaInHierarchies = string.Empty; //InHierarchies
        public string StrSortaInHierarchies
        {
          get { return strSortaInHierarchies; }
          set { strSortaInHierarchies = value; }
        }
        private bool boolSortCountinViews = false;
        public bool BoolSortCountinViews
        {
          get { return boolSortCountinViews; }
          set { boolSortCountinViews = value; }
        }
        private bool boolShowEmptyValuesInViews = false;
        public bool BoolShowEmptyValuesInViews
        {
          get { return boolShowEmptyValuesInViews; }
          set { boolShowEmptyValuesInViews = value; }
        }
        private bool boolDontSplitValuesInViews = false;
        public bool BoolDontSplitValuesInViews
        {
          get { return boolDontSplitValuesInViews; }
          set { boolDontSplitValuesInViews = value; }
        }
        private bool boolReverseNames = false;
        public bool BoolReverseNames
        {
          get { return boolReverseNames; }
          set { boolReverseNames = value; }
        }
        private string strPersons = string.Empty;
        public string StrPersons
        {
            get { return this.strPersons; }
            set { this.strPersons = value; }
        }
        private string strTxtView = string.Empty;
        public string StrTxtView
        {
          get { return strTxtView; }
          set { strTxtView = value; }
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
        private string strStorageTrailer = string.Empty;
        public string StrStorageTrailer
        {
            get { return strStorageTrailer; }
            set { strStorageTrailer = value; }
        }
        private string searchFile = "False";
        public string SearchFile
        {
            get { return searchFile; }
            set { searchFile = value; }
        }
        private string searchFileTrailer = "False";
        public string SearchFileTrailer
        {
            get { return searchFileTrailer; }
            set { searchFileTrailer = value; }
        }
        private string itemSearchFile = string.Empty;
        public string ItemSearchFile
        {
          get { return itemSearchFile; }
          set { itemSearchFile = value; }
        }
    
        private string itemSearchGrabber = string.Empty;
        public string ItemSearchGrabber
        {
          get { return itemSearchGrabber; }
          set { itemSearchGrabber = value; }
        }
        private string itemSearchGrabberScriptsFilter = string.Empty;
        public string ItemSearchGrabberScriptsFilter
        {
          get { return itemSearchGrabberScriptsFilter; }
          set { itemSearchGrabberScriptsFilter = value; }
        }
        private string grabberOverrideLanguage = string.Empty;
        public string GrabberOverrideLanguage
        {
          get { return grabberOverrideLanguage; }
          set { grabberOverrideLanguage = value; }
        }
        private string grabberOverridePersonLimit = string.Empty;
        public string GrabberOverridePersonLimit
        {
          get { return grabberOverridePersonLimit; }
          set { grabberOverridePersonLimit = value; }
        }
        private string grabberOverrideGetRoles = string.Empty;
        public string GrabberOverrideGetRoles
        {
          get { return grabberOverrideGetRoles; }
          set { grabberOverrideGetRoles = value; }
        }
        private string grabberOverrideTitleLimit = string.Empty;
        public string GrabberOverrideTitleLimit
        {
          get { return grabberOverrideTitleLimit; }
          set { grabberOverrideTitleLimit = value; }
        }
        private string pictureHandling = string.Empty;
        public string PictureHandling
        {
          get { return pictureHandling; }
          set { pictureHandling = value; }
        }
        private string itemSearchFileTrailer = string.Empty;
        public string ItemSearchFileTrailer
        {
            get { return itemSearchFileTrailer; }
            set { itemSearchFileTrailer = value; }
        }
        private string searchSubDirs = "False";
        public string SearchSubDirs
        {
            get { return searchSubDirs; }
            set { searchSubDirs = value; }
        }
        private string searchOnlyExactMatches = "False";
        public string SearchOnlyExactMatches
        {
            get { return searchOnlyExactMatches; }
            set { searchOnlyExactMatches = value; }
        }
        private string searchSubDirsTrailer = "False";
        public string SearchSubDirsTrailer
        {
            get { return searchSubDirsTrailer; }
            set { searchSubDirsTrailer = value; }
        }
        private bool checkWatched = false;
        public bool CheckWatched
        {
          get { return checkWatched; }
          set { checkWatched = value; }
        }

        private bool checkWatchedPlayerStopped = false;
        public bool CheckWatchedPlayerStopped
        {
          get { return checkWatchedPlayerStopped; }
          set { checkWatchedPlayerStopped = value; }
        }

        private string strIdentItem = string.Empty;
        public string StrIdentItem
        {
            get { return strIdentItem; }
            set { strIdentItem = value; }
        }
        private MFview customViews = new MFview();
        public MFview CustomViews
        {
          get { return customViews; }
          set { customViews = value; }
        }
        //private string[] strViewItem = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        //public string[] StrViewItem
        //{
        //  get { return strViewItem; }
        //  set { strViewItem = value; }
        //}
        //private string[] strViewText = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        //public string[] StrViewText
        //{
        //    get { return strViewText; }
        //    set { strViewText = value; }
        //}
        //private string[] strViewValue = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        //public string[] StrViewValue
        //{
        //  get { return strViewValue; }
        //  set { strViewValue = value; }
        //}
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
        private string strFileXml = string.Empty;
        public string StrFileXml
        {
          get { return strFileXml; }
          set { strFileXml = value; }
        }
        private string strFileXmlTemp = string.Empty;
        public string StrFileXmlTemp
        {
          get { return strFileXmlTemp; }
          set { strFileXmlTemp = value; }
        }
        private CatalogType strFileType;
        public CatalogType StrFileType
        {
          get { return strFileType; }
          set { strFileType = value; }
        }
        private bool readOnly = true;
        public bool ReadOnly
        {
          get { return readOnly; }
          set { readOnly = value; }
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
        private string strPathTrailer = string.Empty;
        public string StrPathTrailer
        {
            get { return strPathTrailer; }
            set { strPathTrailer = value; }
        }
        private string strPathViews = string.Empty;
        public string StrPathViews
        {
            get { return strPathViews; }
            set { strPathViews = value; }
        }
        private string strPathArtist = string.Empty;
        public string StrPathArtist
        {
            get { return strPathArtist; }
            set { strPathArtist = value; }
        }
        private string strSortSens = string.Empty;
        public string StrSortSens
        {
          get { return strSortSens; }
          set { strSortSens = value; }
        }
        private string strSortSensInHierarchies = string.Empty; //In Hierarchies
        public string StrSortSensInHierarchies
        {
          get { return strSortSensInHierarchies; }
          set { strSortSensInHierarchies = value; }
        }
        private string wStrSortSensCount = string.Empty; //In Views for number count sort direction
        public string WStrSortSensCount
        {
          get { return this.wStrSortSensCount; }
          set { this.wStrSortSensCount = value; }
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
        private string wStrSortSens = string.Empty; //In Views
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
        private string currentSortMethodInHierarchies = string.Empty; //InHierarchies
        public string CurrentSortMethodInHierarchies
        {
          get { return currentSortMethodInHierarchies; }
          set { currentSortMethodInHierarchies = value; }
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
        private string defaultcoverartist = string.Empty;
        public string DefaultCoverArtist
        {
            get { return defaultcoverartist; }
            set { defaultcoverartist = value; }
        }

        private string defaultcoverviews = string.Empty;
        public string DefaultCoverViews
        {
            get { return defaultcoverviews; }
            set { defaultcoverviews = value; }
        }

        private string defaultfanartimage = string.Empty;
        public string DefaultFanartImage
        {
            get { return defaultfanartimage; }
            set { defaultfanartimage = value; }
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
        private int wStrLayOut = int.MinValue;
        public int WStrLayOut
        {
            get { return this.wStrLayOut; }
            set { this.wStrLayOut = value; }
        }
        private int strLayOutInHierarchies = int.MinValue;
        public int StrLayOutInHierarchies
        {
            get { return strLayOutInHierarchies; }
            set { strLayOutInHierarchies = value; }
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
        private MFMovie strPlayedMovie = null;
        public MFMovie StrPlayedMovie
        {
          get { return strPlayedMovie; }
          set { strPlayedMovie = value; }
        }
        private bool strFanart = false;
        public bool StrFanart
        {
          get { return strFanart; }
          set { strFanart = value; }
        }
        private bool strFanartDefaultViews = false;
        public bool StrFanartDefaultViews
        {
          get { return strFanartDefaultViews; }
          set { strFanartDefaultViews = value; }
        }
        private bool strFanartDefaultViewsUseRandom = false;
        public bool StrFanartDefaultViewsUseRandom
        {
          get { return strFanartDefaultViewsUseRandom; }
          set { strFanartDefaultViewsUseRandom = value; }
        }
        private bool strArtist = false;
        public bool StrArtist
        {
            get { return strArtist; }
            set { strArtist = value; }
        }
        private bool strFanartDflt = false;
        public bool StrFanartDflt
        {
          get { return strFanartDflt; }
          set { strFanartDflt = value; }
        }
        private bool strFanartDfltImage = false;
        public bool StrFanartDfltImage
        {
          get { return strFanartDfltImage; }
          set { strFanartDfltImage = value; }
        }
        private bool strFanartDfltImageAll = false;
        public bool StrFanartDfltImageAll
        {
          get { return strFanartDfltImageAll; }
          set { strFanartDfltImageAll = value; }
        }
        private bool strArtistDflt = false;
        public bool StrArtistDflt
        {
            get { return strArtistDflt; }
            set { strArtistDflt = value; }
        }
        private bool useThumbsForViews = false;
        public bool UseThumbsForViews
        {
          get { return this.useThumbsForViews; }
          set { this.useThumbsForViews = value; }
        }
        private bool useThumbsForPersons = false;
        public bool UseThumbsForPersons
        {
          get { return this.useThumbsForPersons; }
          set { this.useThumbsForPersons = value; }
        }
        private bool strViewsDflt = false;
        public bool StrViewsDflt
        {
          get { return strViewsDflt; }
          set { strViewsDflt = value; }
        }

        private bool strViewsDfltAll = false;
        public bool StrViewsDfltAll
        {
          get { return strViewsDfltAll; }
          set { strViewsDfltAll = value; }
        }

        private bool strCheckWOLenable = false;
        public bool StrCheckWOLenable
        {
            get { return strCheckWOLenable; }
            set { strCheckWOLenable = value; }
        }

        private int strWOLtimeout = 15;
        public int StrWOLtimeout
        {
            get { return strWOLtimeout; }
            set { strWOLtimeout = value; }
        }

        private bool strCheckWOLuserdialog = false;
        public bool StrCheckWOLuserdialog
        {
            get { return strCheckWOLuserdialog; }
            set { strCheckWOLuserdialog = value; }
        }

        private string strNasName1 = string.Empty;
        public string StrNasName1
        {
            get { return strNasName1; }
            set { strNasName1 = value; }
        }

        private string strNasMAC1 = string.Empty;
        public string StrNasMAC1
        {
            get { return strNasMAC1; }
            set { strNasMAC1 = value; }
        }

        private string strNasName2 = string.Empty;
        public string StrNasName2
        {
            get { return strNasName2; }
            set { strNasName2 = value; }
        }

        private string strNasMAC2 = string.Empty;
        public string StrNasMAC2
        {
            get { return strNasMAC2; }
            set { strNasMAC2 = value; }
        }

        private string strNasName3 = string.Empty;
        public string StrNasName3
        {
            get { return strNasName3; }
            set { strNasName3 = value; }
        }

        private string strNasMAC3 = string.Empty;
        public string StrNasMAC3
        {
            get { return strNasMAC3; }
            set { strNasMAC3 = value; }
        }

        private string strSearchHistory = string.Empty;
        public string StrSearchHistory
        {
          get { return strSearchHistory; }
          set { strSearchHistory = value; }
        }

        private string strGrabber_cnf = string.Empty;
        public string StrGrabber_cnf
        {
            get { return strGrabber_cnf; }
            set { strGrabber_cnf = value; }
        }
        private string strPicturePrefix = string.Empty;
        public string StrPicturePrefix
        {
            get { return strPicturePrefix; }
            set { strPicturePrefix = value; }
        }
        private string strAntFilterMinRating = string.Empty;
        public string StrAntFilterMinRating
        {
            get { return strAntFilterMinRating; }
            set { strAntFilterMinRating = value; }
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
        private string strWatchedField = string.Empty;
        public string StrWatchedField
        {
          get { return strWatchedField; }
          set { strWatchedField = value; }
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
        List<string[]> trailerList = new List<string[]>();
        public List<string[]> TrailerList
        {
          get { return trailerList; }
          set { trailerList = value; }
        }
        private bool _MyFilmsPlaybackActive = false;
        public bool MyFilmsPlaybackActive
        {
          get { return _MyFilmsPlaybackActive; }
          set { _MyFilmsPlaybackActive = value; }
        }

        #region GetSet for Network and Powermode
        private bool bResumeFromStandby = false;
        public bool IsResumeFromStandby
        {
          get { return bResumeFromStandby; }
          set { bResumeFromStandby = value; }
        }

        private bool bIsNetworkAvailable = true;
        public bool IsNetworkAvailable
        {
          get { return bIsNetworkAvailable; }
          set { bIsNetworkAvailable = value; }
        }

        #endregion


        #endregion

        public static void SaveConfiguration(string currentConfig, int selectedItem, string selectedItemLabel)
        {
          LogMyFilms.Debug("MFC: Configuration saving started for '" + currentConfig + "'");
          using (XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
          {
            #region save xml data
            //XmlConfig XmlConfig = new XmlConfig();
            XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Current_Config", currentConfig);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSelect", MyFilms.conf.StrSelect);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrPersons", MyFilms.conf.StrPersons);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrTitleSelect", MyFilms.conf.StrTitleSelect);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrFilmSelect", MyFilms.conf.StrFilmSelect);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrDfltSelect", MyFilms.conf.StrDfltSelect);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrViewSelect", MyFilms.conf.StrViewSelect); // Custom View filter, is set

            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSort", MyFilms.conf.StrSorta);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "CurrentSortMethod", MyFilms.conf.CurrentSortMethod);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSortSens", MyFilms.conf.StrSortSens);
            
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSortInHierarchies", MyFilms.conf.StrSortaInHierarchies); //InHierarchies
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "CurrentSortMethodInHierarchies", MyFilms.conf.CurrentSortMethodInHierarchies);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "StrSortSensInHierarchies", MyFilms.conf.StrSortSensInHierarchies);

            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "ViewContext", Enum.GetName(typeof(MyFilms.ViewContext), MyFilms.conf.ViewContext));
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "View", MyFilms.conf.StrTxtView);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Selection", MyFilms.conf.StrTxtSelect);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "IndexItem", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItem.ToString()) : "-1"); //may need to check if there is no item selected and so save -1
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "TitleItem", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItemLabel) : string.Empty); //may need to check if there is no item selected and so save ""

            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Boolselect", MyFilms.conf.Boolselect);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Boolreturn", MyFilms.conf.Boolreturn);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Boolindexed", MyFilms.conf.Boolindexed);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Boolindexedreturn", MyFilms.conf.Boolindexedreturn);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "IndexedChars", MyFilms.conf.IndexedChars);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "ReversePersonNames", MyFilms.conf.BoolReverseNames);


            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "WSelectedLabel", MyFilms.conf.Wselectedlabel);
            
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "WStrSort", MyFilms.conf.WStrSort);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "Wstar", MyFilms.conf.Wstar);

            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "LayOut", MyFilms.conf.StrLayOut);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "LayOutInHierarchies", MyFilms.conf.StrLayOutInHierarchies);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "WLayOut", MyFilms.conf.WStrLayOut);
            
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "LastID", MyFilms.conf.LastID);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "SearchHistory", MyFilms.conf.StrSearchHistory);
            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "UserProfileName", MyFilms.conf.StrUserProfileName);

            //foreach (View viewstate in MyFilms.conf.CustomView)
            //{
            //  string viewstateName = "ViewState-" + viewstate.ViewDBItem;
            //  XmlConfig.WriteXmlConfig("MyFilms", currentConfig, viewstateName, MyFilms.conf.CurrentView.SaveToString());
            //}

            XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "CurrentView", MyFilms.conf.CurrentView.SaveToString());

            switch (MyFilms.conf.StrFileType)
            {
              case CatalogType.DVDProfiler:
                XmlConfig.WriteXmlConfig("MyFilms", currentConfig, "AntCatalogTemp", MyFilms.conf.StrFileXml);
                break;
            }
            #endregion
            // XmlConfig.Dispose();
            // XmlSettings.SaveCache();
          }
          LogMyFilms.Debug("MFC: Configuration saving ended for '" + currentConfig + "'");
        }
        //--------------------------------------------------------------------------------------------
        //  Control Acces to asked configuration
        //--------------------------------------------------------------------------------------------
        public static string Control_Access_Config(string configname, int GetID)
        {
            if (configname.Length == 0)
                return "";

            using (XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
            {
              string Dwp = XmlConfig.ReadXmlConfig("MyFilms", configname, "Dwp", string.Empty);
              if (Dwp.Length == 0) return configname;
              MediaPortal.Dialogs.VirtualKeyboard keyboard = (MediaPortal.Dialogs.VirtualKeyboard)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
              if (null == keyboard) return string.Empty;
              keyboard.Reset();
              keyboard.Text = string.Empty;
              keyboard.Password = true;
              keyboard.DoModal(GetID);
              if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
              {
                Crypto crypto = new Crypto();
                if (crypto.Decrypter(Dwp) == keyboard.Text) return configname;
              }
              return string.Empty;
            }
        }
        //--------------------------------------------------------------------------------------------
        //  Choice Configuration
        //--------------------------------------------------------------------------------------------
        public static string Choice_Config(int GetID)
        {
            MediaPortal.Dialogs.GUIDialogMenu dlg = (MediaPortal.Dialogs.GUIDialogMenu)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null)
            {

              MyFilms.conf.StrFileXml = string.Empty;
              return string.Empty;
            }
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(6022)); // Choose MyFilms DB Config
            using (XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
            {
              //XmlConfig XmlConfig = new XmlConfig();
              int MesFilms_nb_config = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
              for (int i = 0; i < (int)MesFilms_nb_config; i++) dlg.Add(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));

              dlg.DoModal(GetID);
              if (dlg.SelectedLabel == -1)
              {
                try
                {
                  MyFilms.conf.StrFileXml = string.Empty;
                }
                catch (Exception ex)
                {
                  LogMyFilms.Debug(
                    "MF: Error resetting config, as not yet loaded into memory - exception: " + ex.ToString());
                }
                return string.Empty;
              }
              if (dlg.SelectedLabelText.Length > 0)
              {
                string Catalog = XmlConfig.ReadXmlConfig("MyFilms", dlg.SelectedLabelText, "AntCatalog", string.Empty);
                if (!System.IO.File.Exists(Catalog))
                {
                  MediaPortal.Dialogs.GUIDialogOK dlgOk =
                    (MediaPortal.Dialogs.GUIDialogOK)
                    MediaPortal.GUI.Library.GUIWindowManager.GetWindow(
                      (int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_OK);
                  dlgOk.SetHeading(10798624);
                  dlgOk.SetLine(1, "Cannot load Configuration:");
                  dlgOk.SetLine(2, "'" + dlg.SelectedLabelText + "'");
                  dlgOk.SetLine(3, "Verify your settings !");
                  dlgOk.DoModal(MediaPortal.GUI.Library.GUIWindowManager.ActiveWindow);
                  return string.Empty;
                }
                else
                {
                  return dlg.SelectedLabelText;
                }
              }
            }
          return string.Empty;
        }
        //--------------------------------------------------------------------------------------------
        //  Return Current Configuration
        //--------------------------------------------------------------------------------------------
        public static bool Current_Config() // returns true, if a default config is set (requires full reload on entering plugin)
        {
            CurrentConfig = null;
            bool defaultconfig = false;

            using (XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
            {
            NbConfig = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", 0);
            PluginMode = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "PluginMode", "normal"); // Reads Plugin start mode and sets to normal if not present
            LogMyFilms.Info("MyFilms ********** OperationsMode (PluginMode): '" + PluginMode + "' **********");
            if (NbConfig == 0)
            {
                MediaPortal.Dialogs.GUIDialogOK dlgOk = (MediaPortal.Dialogs.GUIDialogOK)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                dlgOk.SetLine(1, "No Configuration defined");
                dlgOk.SetLine(2, "Please enter setup first");
                dlgOk.DoModal(MyFilms.ID_MyFilms);
                //MediaPortal.GUI.Library.GUIWindowManager.ShowPreviousWindow(); // doesn't work in this context - why?
              return false;
            }

            bool boolchoice = true;
            if (CurrentConfig == null)
              if (XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Current_Config", string.Empty).Length > 0)
                  CurrentConfig = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Current_Config", string.Empty);
                else
                  CurrentConfig = string.Empty;

            if (!(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Menu_Config", false)) && (XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", string.Empty).Length > 0))
            {
              CurrentConfig = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", string.Empty);
              defaultconfig = true;
            }
            if (CurrentConfig == string.Empty || (XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Menu_Config", true)))
            {
                boolchoice = false;
                CurrentConfig = Configuration.Choice_Config(MyFilms.ID_MyFilms); // "" => user esc's dialog on plugin startup so exit plugin unchanged
            }
            CurrentConfig = Configuration.Control_Access_Config(CurrentConfig, MyFilms.ID_MyFilms);
            if ((CurrentConfig == "") && (NbConfig > 1) && (boolchoice)) //error password ? so if many config => choice config menu
                CurrentConfig = Configuration.Choice_Config(MyFilms.ID_MyFilms);
            }
            return defaultconfig;
        }
    }

}
