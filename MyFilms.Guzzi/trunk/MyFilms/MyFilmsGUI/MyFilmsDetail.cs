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

//using Cornerstone.MP;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  using System;
  using System.Collections;
  using System.Data;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Text.RegularExpressions;
  using System.Windows.Forms;

  using grabber;

  using MediaPortal.Configuration;
  using MediaPortal.Dialogs;
  using MediaPortal.GUI.Library;
  using MediaPortal.Player;
  using MediaPortal.Playlists;
  using MediaPortal.Profile;
  using MediaPortal.Util;
  using MediaPortal.Video.Database;

  using MyFilmsPlugin.MyFilms;

  using MyFilmsPlugin.MyFilms.Utils;

  using SQLite.NET;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;
  using VideoThumbCreator = MyFilmsPlugin.MyFilms.Utils.VideoThumbCreator;

  /// <summary>
    /// Summary description for GUIMesFilms.
    /// </summary>
    public class MyFilmsDetail : GUIWindow
    {
        #region Descriptif zones Ecran

        enum Controls : int
        {
            //CTRL_TxtSelect = 12,
            CTRL_BtnPlay = 10000,
            CTRL_BtnPlay1Description = 10001,
            CTRL_BtnPlay2Comment = 10002,
            CTRL_BtnPlay3Persons = 10003,
            CTRL_BtnPlay4TecDetails = 10004,
            CTRL_BtnPlay5 = 10005,
            CTRL_ViewFanart = 10099,
            CTRL_BtnReturn = 102,
            CTRL_BtnNext = 103,
            CTRL_BtnPrior = 104,
            CTRL_BtnLast = 105,
            CTRL_BtnFirst = 106,
            CTRL_BtnMaj = 107,
            CTRL_BtnActors = 108,
            CTRL_BtnPlayTrailer = 109,
            CTRL_BtnActorThumbs = 110,
            CTRL_BtnMovieThumbs = 111,
            CTRL_BtnMovieInfos = 112,
            CTRL_Fanart = 1000,
            CTRL_FanartDir = 1001,
            //CTRL_MovieThumbs = 1002,
            //CTRL_MovieThumbsDir = 1002,
            CTRL_logos_id2001 = 2001,
            CTRL_logos_id2002 = 2002,
            CTRL_logos_id2003 = 2003,
            CTRL_logos_id2012 = 2012,
            CTRL_Title = 2025,
            CTRL_OTitle = 2026,
            CTRL_ImgDD = 2072,
            CTRL_ActorMultiThumb = 3333,
        }
        [SkinControlAttribute((int)Controls.CTRL_BtnMaj)]
        protected GUIButtonControl BtnMaj;
        [SkinControlAttribute((int)Controls.CTRL_BtnFirst)]
        protected GUIButtonControl BtnFirst;
        [SkinControlAttribute((int)Controls.CTRL_BtnLast)]
        protected GUIButtonControl BtnLast;
        [SkinControlAttribute(2024)]
        protected GUIImage ImgDetFilm;
        [SkinControlAttribute(2023)]
        protected GUIImage ImgDetFilm2;
        [SkinControlAttribute((int)Controls.CTRL_logos_id2001)]
        protected GUIImage ImgID2001;
        [SkinControlAttribute((int)Controls.CTRL_logos_id2002)]
        protected GUIImage ImgID2002;
        [SkinControlAttribute((int)Controls.CTRL_logos_id2003)]
        protected GUIImage ImgID2003;
        [SkinControlAttribute((int)Controls.CTRL_logos_id2012)]
        protected GUIImage ImgID2012;
        [SkinControlAttribute((int)Controls.CTRL_ImgDD)] // Indicates if it's a file existant for movie on HD
        protected GUIImage ImgDD;
        [SkinControlAttribute(2080)]
        protected GUIAnimation m_SearchAnimation;
        [SkinControlAttribute((int)Controls.CTRL_Fanart)]
        protected GUIImage ImgFanart;
        [SkinControlAttribute((int)Controls.CTRL_FanartDir)]
        protected GUIMultiImage ImgFanartDir;
        //[SkinControlAttribute((int)Controls.CTRL_MovieThumbs)]
        //protected GUIImage ImgMovieThumbs = null;
        //[SkinControlAttribute((int)Controls.CTRL_MovieThumbsDir)]
        //protected GUIMultiImage ImgMovieThumbsDir = null;

        [SkinControlAttribute((int)Controls.CTRL_ActorMultiThumb)]
        protected GUIMultiImage ActorMultiThumb = null;

        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
        static string wzone = null;
        int StrMax = 0;

        public const int ID_MyFilms = 7986;
        public const int ID_MyFilmsDetail = 7987;
        public const int ID_MyFilmsDialogRating = 7988;
        public const int ID_MyFilmsActors = 7989;
        public const int ID_MyFilmsThumbs = 7990;
        public const int ID_MyFilmsActorsInfo = 7991;

        public SQLiteClient m_db;
        public class IMDBActorMovie
        {
            public string MovieTitle;
            public string Role;
            public int Year;
        };
        static PlayListPlayer playlistPlayer;
        static VirtualDirectory m_directory = new VirtualDirectory();
        System.ComponentModel.BackgroundWorker bgPicture = new System.ComponentModel.BackgroundWorker();

        static System.Windows.Forms.OpenFileDialog openFileDialog1 = new OpenFileDialog();
        static string _virtualStartDirectory = String.Empty;
        static VirtualDirectory virtualDirectory = new VirtualDirectory();
        static bool m_askBeforePlayingDVDImage = false;
        public static ArrayList result;
        public static string wsearchfile;
        public static int wGetID;
        static MyFilmsDetail()
        {
            playlistPlayer = PlayListPlayer.SingletonPlayer;
        }

        #endregion

        public MyFilmsDetail()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public override int GetID
        {
            get { return ID_MyFilmsDetail; }
            set { base.GetID = value; }
        }
        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\MyFilmsDetail.xml");
        }

        protected override void OnPageLoad()
        {
            setGUIProperty("menu.overview", GUILocalizeStrings.Get(10798751));
            setGUIProperty("menu.description", GUILocalizeStrings.Get(10798752));
            setGUIProperty("menu.comments", GUILocalizeStrings.Get(10798753));
            setGUIProperty("menu.actors", GUILocalizeStrings.Get(10798754));
            setGUIProperty("menu.techinfos", GUILocalizeStrings.Get(10798755));

            BtnFirst.Label = GUILocalizeStrings.Get(1079872);
            BtnLast.Label = GUILocalizeStrings.Get(1079873);
            //GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnFirst, GUILocalizeStrings.Get(1079872));
            //GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLast, GUILocalizeStrings.Get(1079873));

            base.OnPageLoad();
        }

        #region Action
        //---------------------------------------------------------------------------------------
        //   Handle Keyboard Actions
        //---------------------------------------------------------------------------------------
        public override void OnAction(MediaPortal.GUI.Library.Action actionType)
        {
            LogMyFilms.Debug("MyFilmsDetail: OnAction " + actionType.wID.ToString());
            if ((actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) || (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PARENT_DIR))
            {
                MyFilms.conf.LastID = MyFilms.ID_MyFilms;
                GUIWindowManager.ActivateWindow(ID_MyFilms);
                return;
            }

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU)
            {
                LogMyFilms.Debug("MyFilmsDetail : ACTION_CONTEXT_MENU erkannt !!! ");
                // context menu for options  like PlayTrailers or Updates
                //MyFilms.Context_Menu_Movie();
                if (BtnMaj.Focus)
                {
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay);
                    Update_XML_Items(); // Call Update Menu
                }
                else
                {
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay);
                    Update_XML_Items(); // Call Update Menu
                }
                return;
            }


            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PAGE_UP)
            {
                if (MyFilms.conf.StrIndex == 0)
                    return;
                MyFilms.conf.StrIndex = MyFilms.conf.StrIndex - 1;
                //GUITextureManager.CleanupThumbs();
                afficher_detail(true);
                return;
            }

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PAGE_DOWN)
            {
                if (MyFilms.conf.StrIndex == StrMax - 1)
                    return;
                MyFilms.conf.StrIndex = MyFilms.conf.StrIndex + 1;
                //GUITextureManager.CleanupThumbs();
                afficher_detail(true);
                return;
            }

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREV_ITEM)
            {
                if (MyFilms.conf.StrIndex == 0)
                    return;
                MyFilms.conf.StrIndex = MyFilms.conf.StrIndex - 1;
                //GUITextureManager.CleanupThumbs();
                afficher_detail(true);
                return;
            }

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_ITEM)
            {
                if (MyFilms.conf.StrIndex == StrMax - 1)
                    return;
                MyFilms.conf.StrIndex = MyFilms.conf.StrIndex + 1;
                //GUITextureManager.CleanupThumbs();
                afficher_detail(true);
                return;
            }

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REWIND)
            {
                if (MyFilms.conf.StrIndex == 0)
                    return;
                MyFilms.conf.StrIndex = MyFilms.conf.StrIndex - 1;
                //GUITextureManager.CleanupThumbs();
                afficher_detail(true);
                return;
            }

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_FORWARD)
            {
                if (MyFilms.conf.StrIndex == StrMax - 1)
                    return;
                MyFilms.conf.StrIndex = MyFilms.conf.StrIndex + 1;
                //GUITextureManager.CleanupThumbs();
                afficher_detail(true);
                return;
            }

            base.OnAction(actionType);
            return;
        }
        //---------------------------------------------------------------------------------------
        //   Handle posted Messages
        //---------------------------------------------------------------------------------------
        public override bool OnMessage(GUIMessage messageType)
        {

            int dControl = messageType.TargetControlId;
            int iControl = messageType.SenderControlId;
            switch (messageType.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
                    //---------------------------------------------------------------------------------------
                    // Windows Init
                    //---------------------------------------------------------------------------------------
                    if (ImgDetFilm != null)
                        if (ImgDetFilm.IsVisible)
                            ImgDetFilm.Refresh();
                        else if (ImgDetFilm2!= null)
                            ImgDetFilm2.Refresh();
                    base.OnMessage(messageType);

                    wGetID = GetID;
                    GUIControl.ShowControl(GetID, 35);
                    // ToDo: Should be unhidden, if ActorThumbs are implemented
                    GUIControl.HideControl(GetID, (int)Controls.CTRL_ActorMultiThumb);
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    g_Player.PlayBackStarted += new g_Player.StartedHandler(OnPlayBackStarted);
                    g_Player.PlayBackEnded += new g_Player.EndedHandler(OnPlayBackEnded);
                    g_Player.PlayBackStopped += new g_Player.StoppedHandler(OnPlayBackStopped);
                    m_directory.SetExtensions(MediaPortal.Util.Utils.VideoExtensions);
                    if (MyFilms.conf.StrTxtSelect.Length == 0)
                        clearGUIProperty("select");
                        //GUIControl.HideControl(GetID, (int)Controls.CTRL_TxtSelect);
                    else
                    {
                        setGUIProperty("select", MyFilms.conf.StrTxtSelect.Replace(MyFilms.conf.TitleDelim, @"\"));
                        //GUIControl.ShowControl(GetID, (int)Controls.CTRL_TxtSelect);
                    }
                    afficher_init(MyFilms.conf.StrIndex); //Populate DataSet & Convert ItemId passed in initially to Index within DataSet
                    int TitlePos = (MyFilms.conf.StrTitleSelect.Length > 0) ? MyFilms.conf.StrTitleSelect.Length + 1 : 0; //only display rest of title after selected part common to group

                    setProcessAnimationStatus(false, m_SearchAnimation);
                    afficher_detail(true);
                    MyFilms.conf.LastID = MyFilms.ID_MyFilmsDetail;
                    return true;                

                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT: //called when exiting plugin either by prev menu or pressing home button
                    if (global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig != "")
                        global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.SaveConfiguration(global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, MyFilms.conf.StrIndex, MyFilms.conf.StrTIndex);
                    using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
                    {
                        string currentmoduleid = "7986";
                        bool currentmodulefullscreen = (GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_TVFULLSCREEN || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_MUSIC || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_TELETEXT);
                        string currentmodulefullscreenstate = GUIPropertyManager.GetProperty("#currentmodulefullscreenstate");
                        // if MP was closed/hibernated by the use of remote control, we have to retrieve the fullscreen state in an alternative manner.
                        if (!currentmodulefullscreen && currentmodulefullscreenstate == "True")
                            currentmodulefullscreen = true;
                        xmlreader.SetValue("general", "lastactivemodule", currentmoduleid);
                        xmlreader.SetValueAsBool("general", "lastactivemodulefullscreen", currentmodulefullscreen);
                        LogMyFilms.Debug("MF: SaveLastActiveModule - module {0}", currentmoduleid);
                        LogMyFilms.Debug("MF: SaveLastActiveModule - fullscreen {0}", currentmodulefullscreen);
                    }
                    return true;

                case GUIMessage.MessageType.GUI_MSG_CD_REMOVED:
                    //---------------------------------------------------------------------------------------
                    // Stop playing after eject Cd
                    //---------------------------------------------------------------------------------------
                    if (g_Player.Playing && g_Player.IsDVD)
                        g_Player.Stop();
                    return true;

                case GUIMessage.MessageType.GUI_MSG_SETFOCUS:
                    //---------------------------------------------------------------------------------------
                    // Set Focus
                    //---------------------------------------------------------------------------------------
                    base.OnMessage(messageType);
                    return true;

                case GUIMessage.MessageType.GUI_MSG_CLICKED:
                    //---------------------------------------------------------------------------------------
                    // Mouse/Keyboard Clicked
                    //---------------------------------------------------------------------------------------
                    if (iControl == (int)Controls.CTRL_BtnReturn)
                    // Return Previous Menu
                    {
                        MyFilms.conf.LastID = MyFilms.ID_MyFilms;
                        GUITextureManager.CleanupThumbs();
                        GUIWindowManager.ActivateWindow(ID_MyFilms);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay)
                    // Search File to play
                    {
                        Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay1Description)
                    // Search File to play
                    {
                        Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay2Comment)
                    // Search File to play
                    {
                        Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay3Persons)
                    // Search File to play
                    {
                        Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay4TecDetails)
                    // Search File to play
                    {
                        Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay5)
                    // Search File to play
                    {
                        Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnNext)
                    // Display Next Film (If last do nothing)
                    {
                        if (MyFilms.conf.StrIndex == StrMax - 1)
                            return true;
                        MyFilms.conf.StrIndex = MyFilms.conf.StrIndex + 1;
                        GUITextureManager.CleanupThumbs();
                        afficher_detail(true);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPrior)
                    // Display Prior Film (If first do nothing)
                    {
                        if (MyFilms.conf.StrIndex == 0)
                            return true;
                        MyFilms.conf.StrIndex = MyFilms.conf.StrIndex - 1;
                        GUITextureManager.CleanupThumbs();
                        afficher_detail(true);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnLast)
                    // Display Next Film (If last do nothing)
                    {
                        if (MyFilms.conf.StrIndex == StrMax - 1)
                            return true;
                        MyFilms.conf.StrIndex = StrMax - 1;
                        GUITextureManager.CleanupThumbs();
                        afficher_detail(true);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnFirst)
                    // Display Next Film (If First do nothing)
                    {
                        if (MyFilms.conf.StrIndex == 0)
                            return true;
                        MyFilms.conf.StrIndex = 0;
                        GUITextureManager.CleanupThumbs();
                        afficher_detail(true);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnMaj)
                    {    // Update items
                        Update_XML_Items();
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlayTrailer)
                    // Search Trailer File to play
                    {
                        Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_ViewFanart)
                    // On Button goto MyFilmsThumbs
                    {
                        GUIWindowManager.ActivateWindow(ID_MyFilmsThumbs);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnMovieThumbs)
                    {
                        GUIWindowManager.ActivateWindow(ID_MyFilmsThumbs);
                        return true;
                    }

                    if (iControl == (int)Controls.CTRL_BtnActors)
                    {
                        GUIWindowManager.ActivateWindow(ID_MyFilmsActors);
                        return true;
                    }

                    if (iControl == (int)Controls.CTRL_BtnActorThumbs)
                    
                        // Show Actor Details Screen
                        //GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_HOME);
                        GUIWindowManager.ActivateWindow(ID_MyFilmsActors);
                        // Hier Aktivitäten wie z.b. ListControl für Actors?
                        GUIWindowManager.ShowPreviousWindow();
                        //Update_XML_Items(); //To be changed, when DetailScreen is done!!!
                        return true;

            }
            base.OnMessage(messageType);
            return true;
        }
        
        //--------------------------------------------------------------------------------------------
        //  Update specifics Infos - creates Dialogue to choose actions...
        //  choice: sets dialogmode depending from where it's called to display differently
        //  possible modes: "localupdates", "internetupdates", etc.
        //--------------------------------------------------------------------------------------------
        private void Update_XML_Items()
        {
            Change_Menu("mainmenu");
        }


        //--------------------------------------------------------------------------------------------
        //   Change Menu (and process corresponding actions)
        //--------------------------------------------------------------------------------------------
        private void Change_Menu(string choiceView)
        {
            
            string StrUpdItem1 = null;
            string StrUpdText1 = null;
            string StrUpdDflT1 = null;
            string StrUpdItem2 = null;
            string StrUpdText2 = null;
            string StrUpdDflT2 = null;
            int ItemID;

            // Read Info from MyFilms Configfile
            XmlConfig XmlConfig = new XmlConfig();
            StrUpdItem1 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdItem1", "");
            StrUpdText1 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdText1", "");
            StrUpdDflT1 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdDflT1", "");
            StrUpdItem2 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdItem2", "");
            StrUpdText2 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdText2", "");
            StrUpdDflT2 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdDflT2", "");

            AntMovieCatalog ds = new AntMovieCatalog();
            ItemID = (int)MyFilms.r[MyFilms.conf.StrIndex]["Number"]; //set unique id num (ant allows it to be non-unique but that is a bad idea)
            //May wish to completely re-load the dataset before updating any fields if used in multi-user system, but would req concurrency locks etc so...

            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);

            GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            System.Collections.Generic.List<string> choiceViewMenu = new System.Collections.Generic.List<string>();

            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            if (null == keyboard) return;
            keyboard.Reset();

            switch (choiceView)
            {
                case "mainmenu":
                
                    if (dlgmenu == null) return;
                    dlgmenu.Reset();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798701)); // update menu

                    dlgmenu.Add(GUILocalizeStrings.Get(10798704));//trailer menu "Trailer ..."
                    choiceViewMenu.Add("trailermenu");
                
                    dlgmenu.Add(GUILocalizeStrings.Get(931));//rating
                    choiceViewMenu.Add("rating");

                    if (MyFilms.conf.StrSuppress)
                    {
                        dlgmenu.Add(GUILocalizeStrings.Get(432));
                        choiceViewMenu.Add("delete");
                    }

                    dlgmenu.Add(GUILocalizeStrings.Get(10798702)); // local updates ...
                    choiceViewMenu.Add("localupdates");

                    dlgmenu.Add(GUILocalizeStrings.Get(10798703)); // online updates ...
                    choiceViewMenu.Add("onlineupdates");

                    //dlgmenu.Add("Trailer ...");
                    //choiceViewMenu.Add("trailermenu");

                    //dlgmenu.Add("Fanart ...");
                    //choiceViewMenu.Add("fanartmenu");

                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1)
                    {
                        return;
                    }
                    Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                    return;
                    //break;
              
                case "playtrailer":
                    Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                    return;
                    //break;

                case "playtraileronlinevideos":
                    // Load OnlineVideo Plugin with Searchparameters for YouTube and movie to Search ...
                    // OV reference for parameters: site:<sitename>|category:<categoryname>|search:<searchstring>|VKonfail:<true,false>|return:<Locked,Root>
                    // Check for Plugin and correct version - Version information for an assembly consists of the following four values: Major Version, Minor Version, Build Number, Revision
                    var hasRightPlugin = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "OnlineVideos");
                    var hasRightVersion = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "OnlineVideos" && plugin.GetType().Assembly.GetName().Version.Minor > 27);
                    //if (PluginManager.IsPluginNameEnabled2("OnlineVideos"))
                    if (hasRightPlugin && hasRightVersion)
                    {
                        string OVstartparams = string.Empty;
                        string OVtitle = string.Empty;
                        if (MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] != null && MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                            OVtitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                        if (OVtitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
                            OVtitle = OVtitle.Substring(OVtitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
                        OVstartparams = "site:Youtube|category:|search:" + OVtitle + " " + (MyFilms.r[MyFilms.conf.StrIndex]["Year"] + " trailer|return:Locked");
                        //GUIPropertyManager.SetProperty("Onlinevideos.startparams", OVstartparams);
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "YouTube");
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", OVtitle.ToString());
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "Locked");

                        LogMyFilms.Debug("MF: Starting OnlineVideos with '" + OVstartparams.ToString() + "'");
                        GUIWindowManager.ActivateWindow(4755, false); // 4755 is ID for OnlineVideos
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");
                    }
                    else
                    {
                        ShowMessageDialog("MyFilms", "OnlineVideo plugin not installed or wrong version", "Minimum Version resuired: 0.28");
                    }
                    return;
                //break;

                case "playtraileronlinevideosappleitunes":
                    var hasRightPlugin2 = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "OnlineVideos");
                    var hasRightVersion2 = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "OnlineVideos" && plugin.GetType().Assembly.GetName().Version.Minor > 27);
                    //if (PluginManager.IsPluginNameEnabled2("OnlineVideos"))
                    if (hasRightPlugin2 && hasRightVersion2)
                    {
                      string OVstartparams = string.Empty;
                      string OVtitle = string.Empty;
                      if (MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] != null && MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                        OVtitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                      if (OVtitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
                        OVtitle = OVtitle.Substring(OVtitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
                      OVstartparams = "site:iTunes Movie Trailers|category:|search:" + OVtitle + " " + (MyFilms.r[MyFilms.conf.StrIndex]["Year"] + " trailer|return:Locked");
                      //GUIPropertyManager.SetProperty("Onlinevideos.startparams", OVstartparams);
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "iTunes Movie Trailers");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", OVtitle.ToString());
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "Locked");

                      LogMyFilms.Debug("MF: Starting OnlineVideos with '" + OVstartparams.ToString() + "'");
                      GUIWindowManager.ActivateWindow(4755, false); // 4755 is ID for OnlineVideos
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");
                    }
                    else
                    {
                      ShowMessageDialog("MyFilms", "OnlineVideo plugin not installed or wrong version", "Minimum Version resuired: 0.28");
                    }
                    return;
                //break;

                case "rating":
                    MyFilmsDialogSetRating dlgRating = (MyFilmsDialogSetRating)GUIWindowManager.GetWindow(7988);
                    if (MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Length > 0)
                        dlgRating.Rating = (decimal)MyFilms.r[MyFilms.conf.StrIndex]["Rating"];
                    else
                        dlgRating.Rating = 0;
                    dlgRating.SetTitle(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString());
                    dlgRating.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1 || dlgmenu.SelectedLabel == 1)
                      Change_Menu("mainmenu");
                    MyFilms.r[MyFilms.conf.StrIndex]["Rating"] = dlgRating.Rating.ToString();
                    Update_XML_database();
                    afficher_detail(true);
                    break;

                case "trailermenu":
                    if (dlgmenu == null) return;
                    dlgmenu.Reset();
                    choiceViewMenu.Clear();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798704)); // Trailer ...

                    dlgmenu.Add(GUILocalizeStrings.Get(10798710));//play trailer
                    choiceViewMenu.Add("playtrailer");

                    dlgmenu.Add(GUILocalizeStrings.Get(10798711));//search youtube trailer with onlinevideos
                    choiceViewMenu.Add("playtraileronlinevideos");

                    dlgmenu.Add(GUILocalizeStrings.Get(10798712));//search apple itunes trailer with onlinevideos
                    choiceViewMenu.Add("playtraileronlinevideosappleitunes");

                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1)
                      Change_Menu("mainmenu");
                    Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                    break;

                case "localupdates":
                    if (dlgmenu == null) return;
                    dlgmenu.Reset();
                    choiceViewMenu.Clear();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798702)); // Local Updates ...

                    if (MyFilms.conf.StrUpdList[0].Length > 0)
                    {
                        dlgmenu.Add(GUILocalizeStrings.Get(10798642));  // Update by Property (choosen within the UPdate List Property
                        choiceViewMenu.Add("updproperty");
                    }

                    if (MyFilms.conf.StrStorage.Length != 0 && MyFilms.conf.StrStorage != "(none)" && (MyFilms.conf.WindowsFileDialog))
                    {
                        dlgmenu.Add(GUILocalizeStrings.Get(863));//file
                        choiceViewMenu.Add("updatedb");
                    }

                    // No more needed because of updproperties !!!
                    //if (!(StrUpdItem1 == "(none)"))
                    //    {
                    //    if (StrUpdText1.Length > 0)
                    //        dlgmenu.Add(StrUpdText1);        //Specific Item1 label to update
                    //    else
                    //        dlgmenu.Add(StrUpdItem1);        //Specific Item1 to update
                    //    choiceViewMenu.Add("item1");
                    //    }
                    //if (!(StrUpdItem2 == "(none)"))
                    //{
                    //    if (StrUpdText2.Length > 0)
                    //        dlgmenu.Add(StrUpdText2);        //Specific Item2 label to update
                    //    else
                    //        dlgmenu.Add(StrUpdItem2);        //Specific Item2 to update
                    //    choiceViewMenu.Add("item2");
                    //}

                    if(ExtendedStartmode("Details context: nfo-reader-update"))
                    {
                      dlgmenu.Add(GUILocalizeStrings.Get(10798730));             //Update Moviedetails from nfo-file - also download actor thumbs, Fanart, etc. if available
                      choiceViewMenu.Add("nfo-reader-update");
                    }

                    //dlgmenu.Add(GUILocalizeStrings.Get(10798721));             //Update Moviedetails from ant.info file
                    //choiceViewMenu.Add("ant-nfo-reader");

                    //dlgmenu.Add(GUILocalizeStrings.Get(10798722));             //Save Moviedetails to ant.info file
                    //choiceViewMenu.Add("ant-nfo-writer");

                    if (MyFilms.conf.StrStorageTrailer.Length > 0)
                    {
                      dlgmenu.Add(GUILocalizeStrings.Get(10798723));             //Search local Trailer and Update DB (local)
                      choiceViewMenu.Add("trailer-register");

                      dlgmenu.Add(GUILocalizeStrings.Get(10798725));             //delete Trailer entries from DB record
                      choiceViewMenu.Add("trailer-delete");
                    }

                    if (ExtendedStartmode("Details context: Thumb creator (and fanart creator?)"))
                    {
                      dlgmenu.Add(GUILocalizeStrings.Get(10798728));
                        //Create Thumb from movie - if no cover available, e.g. with documentaries
                      choiceViewMenu.Add("cover-thumbnailer");
                    }

                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1)
                        Change_Menu("mainmenu");

                    Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                    break;

                case "onlineupdates":
                    if (dlgmenu == null) return;
                    dlgmenu.Reset();
                    choiceViewMenu.Clear();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798703)); // Online Updates ...

                    if (MyFilms.conf.StrGrabber)
                    {
                        dlgmenu.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
                        choiceViewMenu.Add("grabber");
                    }

                    if (ExtendedStartmode("Details context: Trailer Download"))
                    {
                      dlgmenu.Add(GUILocalizeStrings.Get(10798724)); //load IMDB Trailer, store locally and update DB
                      choiceViewMenu.Add("trailer-imdb");

                      dlgmenu.Add(GUILocalizeStrings.Get(10798725));             //delete Trailer entries from DB record
                      choiceViewMenu.Add("trailer-delete");
                    }

                    if (MyFilms.conf.StrFanart)            // Download Fanart
                    {
                        dlgmenu.Add(GUILocalizeStrings.Get(1079862));
                        choiceViewMenu.Add("fanart");
                    }
                    if (MyFilms.conf.StrFanart)            // Remove Fanart
                    {
                        dlgmenu.Add(GUILocalizeStrings.Get(1079874));
                        choiceViewMenu.Add("deletefanart");
                    }

                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1)
                        Change_Menu("mainmenu");

                    Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                    break;

                case "file":
                    string wfile = string.Empty;
                    if (MyFilms.conf.WindowsFileDialog)
                    {
                        openFileDialog1.RestoreDirectory = true;
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                            wfile = openFileDialog1.FileName;
                    }
                    if (wfile != string.Empty)
                    {
                        MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage] = wfile;
                        Update_XML_database();
                        afficher_detail(true);
                    }
                    break;

                case "item1":
                    if (StrUpdDflT1.Length > 0)
                        keyboard.Text = StrUpdDflT1;
                    else
                        keyboard.Text = MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem1].ToString();
                    keyboard.DoModal(GetID);
                    if (keyboard.IsConfirmed)
                    {
                        switch (ds.Movie.Columns[StrUpdItem1].DataType.Name)
                        {
                            case "Decimal":
                                try { MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem1] = Convert.ToDecimal(keyboard.Text); }
                                catch
                                {
                                    dlgOK.SetHeading(GUILocalizeStrings.Get(924)); // menu
                                    dlgOK.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlgOK.DoModal(GetID);
                                    return;
                                }
                                break;
                            case "Int32":
                                try { MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem1] = Convert.ToInt32(keyboard.Text); }
                                catch
                                {
                                    dlgOK.SetHeading(GUILocalizeStrings.Get(924)); // menu
                                    dlgOK.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlgOK.DoModal(GetID);
                                    return;
                                }
                                break;
                            default:
                                MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem1] = keyboard.Text.ToString();
                                break;
                        }

                        Update_XML_database();
                        afficher_detail(true);
                    }
                    break;

                case "item2":
                    if (StrUpdDflT1.Length > 0)
                        keyboard.Text = StrUpdDflT2;
                    else
                        keyboard.Text = MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem2].ToString();
                    keyboard.DoModal(GetID);
                    if (keyboard.IsConfirmed)
                    {
                        switch (ds.Movie.Columns[StrUpdItem2].DataType.Name)
                        {
                            case "Decimal":
                                try { MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem2] = Convert.ToDecimal(keyboard.Text); }
                                catch
                                {
                                    dlgOK.SetHeading(GUILocalizeStrings.Get(924)); // menu
                                    dlgOK.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlgOK.DoModal(GetID);
                                    return;
                                }
                                break;
                            case "Int32":
                                try { MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem2] = Convert.ToInt32(keyboard.Text); }
                                catch
                                {
                                    dlgOK.SetHeading(GUILocalizeStrings.Get(924)); // menu
                                    dlgOK.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlgOK.DoModal(GetID);
                                    return;
                                }
                                break;
                            default:
                                MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem2] = keyboard.Text.ToString();
                                break;
                        } Update_XML_database();
                        afficher_detail(true);
                    }
                    break;

                case "delete":
                    if (MyFilms.conf.StrSuppress)
                    {
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                        dlgYesNo.SetLine(1, GUILocalizeStrings.Get(433));//confirm suppression
                        dlgYesNo.DoModal(GetID);
                        if (dlgYesNo.IsConfirmed)
                        {
                            MyFilmsDetail.Suppress_Entry((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex);
                            //                           Update_XML_database();
                            MyFilms.r = BaseMesFilms.LectureDonnées(MyFilms.conf.StrDfltSelect, MyFilms.conf.StrFilmSelect, MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens);
                            afficher_detail(true);

                        }
                    }
                    break;
                case "updproperty":
                    System.Collections.Generic.List<string> choiceUpd = new System.Collections.Generic.List<string>();
                    ArrayList w_tableau = new ArrayList();
                    if (dlgmenu == null) return;
                    dlgmenu.Reset();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798643)); // menu
                    foreach (string wupd in MyFilms.conf.StrUpdList)
                    {
                        dlgmenu.Add(GUILocalizeStrings.Get(184) + " '" + BaseMesFilms.Translate_Column(wupd.Trim()) + "'");
                        choiceUpd.Add(wupd.Trim());
                    }
                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1)
                        Change_Menu("mainmenu"); // go back to main contextmenu
                    string wproperty = choiceUpd[dlgmenu.SelectedLabel];
                    dlgmenu.Reset();
                    keyboard.Reset();
                    keyboard.Text = MyFilms.r[MyFilms.conf.StrIndex][wproperty].ToString();
                    keyboard.DoModal(GetID);
                    if (keyboard.IsConfirmed)
                    {
                        switch (ds.Movie.Columns[wproperty].DataType.Name)
                        {
                            case "Decimal":
                                try { MyFilms.r[MyFilms.conf.StrIndex][wproperty] = Convert.ToDecimal(keyboard.Text); }
                                catch
                                {
                                    dlgOK.SetHeading(GUILocalizeStrings.Get(10798642)); // menu
                                    dlgOK.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlgOK.DoModal(GetID);
                                    return;
                                }
                                break;
                            case "Int32":
                                try { MyFilms.r[MyFilms.conf.StrIndex][wproperty] = Convert.ToInt32(keyboard.Text); }
                                catch
                                {
                                    dlgOK.SetHeading(GUILocalizeStrings.Get(10798642)); // menu
                                    dlgOK.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlgOK.DoModal(GetID);
                                    return;
                                }
                                break;
                            default:
                                MyFilms.r[MyFilms.conf.StrIndex][wproperty] = keyboard.Text.ToString();
                                break;
                        }
                        Update_XML_database();
                        afficher_detail(true);
                    }
                    break;

                case "grabber":
                    bool wChooseScript = MyFilms.conf.StrGrabber_ChooseScript;
                    if (!System.IO.File.Exists(MyFilms.conf.StrGrabber_cnf) && !MyFilms.conf.StrGrabber_ChooseScript)
                    {
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                        dlgYesNo.SetLine(1, string.Format(GUILocalizeStrings.Get(1079875), MyFilms.conf.StrGrabber_cnf));//File doesn't exist. Do you want to choose it ?
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079878));//confirm suppression
                        dlgYesNo.DoModal(GetID);
                        if (dlgYesNo.IsConfirmed)
                            wChooseScript = true;
                        else
                        {
                            LogMyFilms.Info("My Films : The grabber config file doesn't exists. No grab Information done !");
                            break;
                        }
                    }
                    setProcessAnimationStatus(true, m_SearchAnimation);
                    string title = string.Empty;
                    if (MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] != null && MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                        title = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                    if (title.IndexOf(MyFilms.conf.TitleDelim) > 0)
                        title = title.Substring(title.IndexOf(MyFilms.conf.TitleDelim) + 1);
                    grabb_Internet_Informations(title, GetID, wChooseScript, MyFilms.conf.StrGrabber_cnf);
                    afficher_detail(true);
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    break;

                case "nfo-reader-update":
                    {
                        Grab_Nfo_Details((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, GetID, false);
                        break;
                    }

                case "ant-nfo-reader":
                    ShowMessageDialog("Info", "", "Action not yet implemented");
                    break;

                case "ant-nfo-writer":
                    ShowMessageDialog("Info", "", "Action not yet implemented");
                    break;

                case "trailer-imdb":
                    ShowMessageDialog("Info", "", "Action not yet implemented");
                    break;

                case "cover-thumbnailer":
                    CreateThumbFromMovie();
                    ShowMessageDialog(GUILocalizeStrings.Get(107986), "", "Cover created from movie");
                    break;

                case "trailer-register":
                    {
                        setProcessAnimationStatus(true, m_SearchAnimation);
                        //Zuerst Pfad lesen, dann Dateien suchen, in liste packen, Auswahlmenü präsenteiren und zum Schluß Update des Records
                        // Suchen nach Files mit folgendem Kriterium:
                        // 1.) ... die den Filmnamen im Filenamen haben und im Trailerverzeichnis gefunden werden (wahrscheinlich HD, daher an 1. Stelle setzen)
                        // 2.) Im Verzeichnis des Films suchen nach Filmdateien die das Wort "Trailer" im Namen haben (Endung beliebig: avi, mov, flv, etc.)
                        // 3.) Im (Trailer)-Suchpfad nach Verzeichnissen, die nach dem Filmnamen benannt sind - dann alle Files darin registrien

                        //LogMyFilms.Debug("MF: (SearchTrailerLocal) SelectedItemInfo from (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(): '" + (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() + "'"));
                        LogMyFilms.Debug("MF: (SearchTrailerLocal) SelectedItemInfo from (MyFilms.r[MyFilms.conf.StrIndex]: '" + (MyFilms.r[MyFilms.conf.StrIndex].ToString() + "'"));
                        LogMyFilms.Debug("MF: (SearchTrailerLocal) Parameter 1 - '(DataRow[])MyFilms.r': '" + (DataRow[])MyFilms.r);
                        LogMyFilms.Debug("MF: (SearchTrailerLocal) Parameter 2 - '(int)MyFilms.conf.StrIndex': '" + (int)MyFilms.conf.StrIndex);
                        MyFilmsDetail.SearchTrailerLocal((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true);
                        afficher_detail(true);
                        //GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        setProcessAnimationStatus(false, m_SearchAnimation);
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                        dlgOk.SetLine(1, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrSTitle.ToString()].ToString());//video title
                        dlgOk.SetLine(2, result.Count.ToString() + " Trailers gefunden !");
                        dlgOk.DoModal(GetID);

                        //if (dlg == null) return;
                        //dlg.Reset();
                        //dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                        //dlg.Add(GUILocalizeStrings.Get(10798723));             //Search local Trailer and Update DB (local)
                        //upd_choice[ichoice] = "trailer";
                        //ichoice++;

                        //dlg.DoModal(GetID);
                        //if (dlg.SelectedLabel == -1) return;
                        break;
                    }

                case "trailer-delete":
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                    dlgYesNo.SetLine(1, GUILocalizeStrings.Get(433));//confirm suppression
                    dlgYesNo.DoModal(GetID);
                    if (dlgYesNo.IsConfirmed)
                    {
                        MyFilmsDetail.DeleteTrailerFromDB((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex);
                        //MyFilms.r = BaseMesFilms.LectureDonnées(MyFilms.conf.StrDfltSelect, MyFilms.conf.StrFilmSelect, MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens);
                        afficher_detail(true);
                    }
                    break;

                case "fanart":
                    setProcessAnimationStatus(true, m_SearchAnimation);
                    Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
                    string wtitle = string.Empty;
                    if (MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] != null && MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString().Length > 0)
                        wtitle = MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString();
                    if (wtitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
                        wtitle = wtitle.Substring(wtitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
                    string wttitle = string.Empty;
                    if (MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] != null && MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                        wttitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                    if (wttitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
                        wttitle = wttitle.Substring(wttitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
                    if (MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString().Length > 0 && MyFilms.conf.StrFanart)
                    {
                        int wyear = 0;
                        try { wyear = System.Convert.ToInt16(MyFilms.r[MyFilms.conf.StrIndex]["Year"]); }
                        catch { }
                        string wdirector = string.Empty;
                        try { wdirector = (string)MyFilms.r[MyFilms.conf.StrIndex]["Director"]; }
                        catch { }
                        LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download Fanart: originaltitle: '" + wtitle + "' - translatedtitle: '" + wttitle + "' - director: '" + wdirector + "' - year: '" + wyear.ToString() + "'");
                        Download_Backdrops_Fanart(wtitle, wttitle, wdirector.ToString(), wyear.ToString(), true, GetID);

                    }
                    afficher_detail(true);
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    break;
                case "deletefanart":
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                    dlgYesNo.SetLine(1, GUILocalizeStrings.Get(433));//confirm suppression
                    dlgYesNo.DoModal(GetID);
                    if (dlgYesNo.IsConfirmed)
                        Remove_Backdrops_Fanart(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(), false);
                    afficher_detail(true);
                    break;

                default: // Main Contextmenu
                    ShowMessageDialog("Info", "", "Action not yet implemented");
                    return;
            }
        }
        //-------------------------------------------------------------------------------------------
        //  Suppress an entry from the database
        //-------------------------------------------------------------------------------------------        
        public static void Suppress_Entry(DataRow[] r1, int Index)
        {

            if ((MyFilms.conf.StrSuppressType == "2") || (MyFilms.conf.StrSuppressType == "4"))
            {
                ArrayList newItems = new ArrayList();
                bool NoResumeMovie = true;
                int IMovieIndex = 0;

                Search_All_Files(Index, true, ref NoResumeMovie, ref newItems, ref IMovieIndex, false);
                foreach (object t in newItems)
                {
                    // for each entry test if it's a file, a directory or a dvd copy
                    // no action for files on amovible media or image files
                    if (System.IO.File.Exists(t.ToString()))
                        try
                        {
                            System.IO.File.Delete(t.ToString());
                            LogMyFilms.Info("MF: file deleted : " + t.ToString());
                        }
                        catch
                        {
                            LogMyFilms.Info("MF: unable to delete file : " + t.ToString());
                        }
                }
            }
            if ((MyFilms.conf.StrSuppressType == "1") || (MyFilms.conf.StrSuppressType == "2"))
            {
                string wdelTitle = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
                MyFilms.r[Index].Delete();
                
                LogMyFilms.Info("MF: Database movie deleted : " + wdelTitle);
            }
            else
            {
                MyFilms.r[Index][MyFilms.conf.StrSuppressField] = MyFilms.conf.StrSuppressValue.ToString();
                LogMyFilms.Info("MF: Database movie updated for deletion : " + MyFilms.r[Index][MyFilms.conf.StrTitle1]);
            }
            Update_XML_database();
        }
        //-------------------------------------------------------------------------------------------
        //  Update the XML database and refresh screen
        //-------------------------------------------------------------------------------------------        
        public static void Update_XML_database()
        {
            BaseMesFilms.SaveMesFilms();
            LogMyFilms.Info("MF: Movie Database updated");
        }

        //-------------------------------------------------------------------------------------------
        //  Grab URL Internet Movie Informations and update the XML database and refresh screen
        //-------------------------------------------------------------------------------------------        
        public static void grabb_Internet_Informations(string FullMovieName, int GetID, bool choosescript, string wscript)
        {
            if (choosescript)
            {
                if (MyFilms.conf.StrGrabber_Dir.Length > 0 && System.IO.Directory.Exists(MyFilms.conf.StrGrabber_Dir))
                {
                    // Grabber Directory filled, search for XML scripts files
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                    if (dlg == null) return;
                    ArrayList scriptfile = new ArrayList();
                    if (MyFilms.conf.StrGrabber_cnf.Length > 0)
                    {
                        scriptfile.Add(MyFilms.conf.StrGrabber_cnf);
                        dlg.Add(MyFilms.conf.StrGrabber_cnf.Substring(MyFilms.conf.StrGrabber_cnf.LastIndexOf("\\") + 1));
                        dlg.SelectedLabel = 0;
                    }
                    DirectoryInfo dirsInf = new DirectoryInfo(MyFilms.conf.StrGrabber_Dir);
                    FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();
                    foreach (FileSystemInfo sfi in sfiles)
                    {
                        if ((sfi.Extension.ToLower() == ".xml") && (sfi.FullName != MyFilms.conf.StrGrabber_cnf))
                        {
                            dlg.Add(sfi.Name);
                            scriptfile.Add(sfi.FullName);
                        }
                    }
                    if (scriptfile.Count > 1)
                    {
                        dlg.DoModal(GetID);
                        if (dlg.SelectedLabel > 0)
                            wscript = scriptfile[dlg.SelectedLabel].ToString();
                    }
                }
                else
                {
                    GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    dlgOk.SetHeading(GUILocalizeStrings.Get(645)); // menu
                    dlgOk.SetLine(1, string.Format(GUILocalizeStrings.Get(1079876), MyFilms.conf.StrGrabber_Dir));
                    dlgOk.SetLine(2, GUILocalizeStrings.Get(1079877));
                    dlgOk.DoModal(GetID);
                    LogMyFilms.Info("My Films : The Directory grabber config files doesn't exists. Verify your Configuration !");
                    return;                    
                }
            }
            string MovieName = FullMovieName;
            string MovieHierarchy = string.Empty;
            if (MyFilms.conf.TitleDelim.Length > 0)
            {
                MovieName = FullMovieName.Substring(FullMovieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
                MovieHierarchy = FullMovieName.Substring(0, FullMovieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
            }
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            Grabber.Grabber_URLClass.IMDBUrl wurl;

            ArrayList listUrl = Grab.ReturnURL(MovieName, wscript, 1, !MyFilms.conf.StrGrabber_Always);
            int listCount = listUrl.Count;
            if (!MyFilms.conf.StrGrabber_Always)
                listCount = 2;
            switch (listCount)
            {
                case 1:
                    wurl = (Grabber.Grabber_URLClass.IMDBUrl)listUrl[0];
                    grabb_Internet_Details_Informations(wurl.URL, MovieHierarchy, wscript, GetID, false, false, "");
                    break;
                case 0:
                    break;
                default:
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                    dlg.Add("  *****  " + GUILocalizeStrings.Get(200036) + "  *****  "); //choice for changing movie filename
                    for (int i = 0; i < listUrl.Count; i++)
                    {
                        wurl = (Grabber.Grabber_URLClass.IMDBUrl)listUrl[i];
                        if (wurl.Title.Contains(MyFilms.r[MyFilms.conf.StrIndex]["Director"].ToString()) && wurl.Title.Contains(MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString()) && (!MyFilms.conf.StrGrabber_Always))
                        {
                            if (dlg.SelectedLabel == -1)
                                dlg.SelectedLabel = i + 1;
                            else
                                dlg.SelectedLabel = -2;
                        }
                        dlg.Add(wurl.Title.ToString());
                    }
                    if (!(dlg.SelectedLabel > -1))
                    {
                        dlg.SelectedLabel = -1;
                        dlg.DoModal(GetID);
                    }
                    if (dlg.SelectedLabel == 0)
                    {
                        VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                        if (null == keyboard) return;
                        keyboard.Reset();
                        keyboard.Text = MovieName;
                        keyboard.DoModal(GetID);
                        if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                            grabb_Internet_Informations(keyboard.Text.ToString(), GetID, false, wscript);
                        break;
                    }
                    if (dlg.SelectedLabel > 0)
                    {
                        wurl = (Grabber.Grabber_URLClass.IMDBUrl)listUrl[dlg.SelectedLabel - 1];
                        grabb_Internet_Details_Informations(wurl.URL, MovieHierarchy,wscript, GetID, true, false, "");
                    }
                    break;
            }
        }
        //-------------------------------------------------------------------------------------------
        //  Grab Internet Movie Details Informations and update the XML database and refresh screen
        //-------------------------------------------------------------------------------------------        
        public static void grabb_Internet_Details_Informations(string url, string moviehead, string wscript, int GetID, bool interactive, bool nfo, string nfofile)
        {
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            string[] Result = new string[20];
            string title = string.Empty;
            string ttitle = string.Empty;
            string wtitle = string.Empty;
            int year = 0;
            string director = string.Empty;
            XmlConfig XmlConfig = new XmlConfig();
            string Img_Path = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Image_Download_Filename_Prefix", "");
            string Img_Path_Type = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Store_Image_With_Relative_Path", "false");
            bool onlymissing = false;

            if (nfo)
                Result = Grab.GetNfoDetail(nfofile, MyFilms.conf.StrPathImg + Img_Path, MyFilms.conf.StrPathArtist, "");
            else
            {
              string downLoadPath;
              if (interactive) 
                downLoadPath = Config.GetDirectoryInfo(Config.Dir.Config) + @"\Thumbs\MyFilms";
              else
                downLoadPath = MyFilms.conf.StrPathImg;
              Result = Grab.GetDetail(url, downLoadPath + Img_Path, wscript);
            }
            LogMyFilms.Info("MF: Grab Internet/nfo Information done for : " + ttitle);

            // string Title_Group = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Folder_Name_Is_Group_Name", "false");
            // string Title_Group_Apply = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Group_Name_Applies_To", "");

            string strChoice = "all"; // defaults to "all", if no other choice
            if (interactive) // Dialog only in interactive mode
            {

                System.Collections.Generic.List<string> choiceViewMenu = new System.Collections.Generic.List<string>();
                GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                dlgmenu.Reset();
                dlgmenu.SetHeading(GUILocalizeStrings.Get(10798732)); // Choose property to update
                dlgmenu.Add(GUILocalizeStrings.Get(10798734));
                choiceViewMenu.Add("all");
                dlgmenu.Add(GUILocalizeStrings.Get(10798735));
                choiceViewMenu.Add("missing");

                string[] PropertyList = new string[] { "OriginalTitle", "TranslatedTitle", "Picture", "Description", "Rating", "Actors", "Director", "Producer", "Year", "Country", "Category", "URL" };
                string strOldValue = "";
                string strNewValue = "";

                int i = 0;
                foreach (string wProperty in PropertyList)
                {
                    try
                    {
                        strOldValue = MyFilms.r[MyFilms.conf.StrIndex][wProperty].ToString();
                        if (strOldValue == null)
                            strOldValue = "";
                        strNewValue = Result[i].ToString();
                        if (i == 2)
                            strNewValue = Result[12];
                        if (strOldValue == null)
                            strOldValue = "";

                        dlgmenu.Add(BaseMesFilms.Translate_Column(wProperty) + ": '" + strOldValue + "' -> '" + strNewValue + "'");
                        choiceViewMenu.Add(wProperty);
                    }
                    catch
                    {
                        LogMyFilms.Debug("MF: GrabberUpdate - Error adding Property '" + wProperty + "' to Selectionmenu");
                    }
                    i = i + 1;
                }

                dlgmenu.DoModal(GetID);
                if (dlgmenu.SelectedLabel == -1)
                {
                    return;
                }
                strChoice = choiceViewMenu[dlgmenu.SelectedLabel];
                if (strChoice == "missing")
                    onlymissing = true;
            }

                
            switch (strChoice)
            {
                case "OriginalTitle":
                    if (Result[0] != string.Empty && Result[0] != null)
                    {
                        title = Result[0].ToString();
                        wtitle = MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString();
                        if (wtitle.Contains(MyFilms.conf.TitleDelim))
                            wtitle = wtitle.Substring(wtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1);
                        if (wtitle != title)
                            Remove_Backdrops_Fanart(wtitle, true);
                        if (MyFilms.conf.StrTitle1 == "OriginalTitle")
                            MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] = moviehead + title;
                        else
                            MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] = title;
                    }
                    break;
                case "TranslatedTitle":
                    if (Result[1] != string.Empty && Result[1] != null)
                    {
                        ttitle = Result[1].ToString();
                        wtitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                        if (wtitle.Contains(MyFilms.conf.TitleDelim))
                            wtitle = wtitle.Substring(wtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1);
                        if (wtitle != ttitle)
                            Remove_Backdrops_Fanart(wtitle, true);
                        if (MyFilms.conf.StrTitle1 == "TranslatedTitle")
                            MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] = moviehead + ttitle;
                        else
                            MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] = ttitle;
                    }
                    break;
                case "Picture":
                    if (Result[2] != string.Empty && Result[2] != null)
                    {
                      string oldPicture = GUIPropertyManager.GetProperty("picture");
                      string newPicture = Result[2];
                      LogMyFilms.Debug("Picture Grabber options: Old temp Cover Image: '" + oldPicture.ToString() + "'");
                      LogMyFilms.Debug("Picture Grabber options: New temp Cover Image: '" + newPicture.ToString() + "'");

                      if (Img_Path_Type.ToLower() == "true")
                            Result[2] = Result[2].Substring(Result[2].LastIndexOf("\\") + 1).ToString();
                        if (Img_Path.Length > 0)
                            if (Img_Path.EndsWith("\\"))
                                Result[2] = Img_Path + Result[2];
                            else
                                Result[2] = Img_Path + "\\" + Result[2];


                        setGUIProperty("picture", newPicture);
                        GUIWindowManager.Process(); // To Update GUI display ...

                        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079870)); // choice
                        dlgYesNo.SetLine(1, "");
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(10798733)); // Replace cover with new one
                        dlgYesNo.SetLine(3, "");
                        dlgYesNo.DoModal(GetID);
                        if (!(dlgYesNo.IsConfirmed))
                        {
                            setGUIProperty("picture", oldPicture);
                            GUIWindowManager.Process();
                            return;
                        }
                        if (newPicture != null && newPicture.Length > 0 && oldPicture.Length > 0)
                        {
                          File.Copy(newPicture, oldPicture, true);
                          setGUIProperty("picture", oldPicture);
                          GUIWindowManager.Process();
                          return;
                        }
                        else
                        {
                          try
                          {
                            string newFinalPicture = MyFilms.conf.StrPathImg + "\\" + Result[2];
                            File.Copy(newPicture, newFinalPicture, true);
                            MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newFinalPicture;
                            setGUIProperty("picture", newFinalPicture);
                            GUIWindowManager.Process();
                            return;
                          }
                          catch (Exception ex)
                          {
                            LogMyFilms.Debug("Error copy file: '" + newPicture + "' - Exception: " + ex.ToString());
                          }
                        }
                    }
                    break;
                case "Description":
                    if (Result[3] != string.Empty && Result[3] != null)
                        MyFilms.r[MyFilms.conf.StrIndex]["Description"] = Result[3].ToString();
                    break;
                case "Rating":
                    if (Result[4] != string.Empty && Result[4] != null)
                    {
                        if (Result[4].ToString().Length > 0)
                        {
                            NumberFormatInfo provider = new NumberFormatInfo();
                            provider.NumberDecimalSeparator = ".";
                            provider.NumberDecimalDigits = 1;
                            decimal wnote = Convert.ToDecimal(Result[4], provider);
                            MyFilms.r[MyFilms.conf.StrIndex]["Rating"] = string.Format("{0:F1}", wnote);
                        }
                    }
                    break;
                case "Actors":
                    if (Result[5] != string.Empty && Result[5] != null)
                        MyFilms.r[MyFilms.conf.StrIndex]["Actors"] = Result[5].ToString();
                    break;
                case "Director":
                    if (Result[6] != string.Empty && Result[6] != null)
                    {
                        director = Result[6].ToString();
                        MyFilms.r[MyFilms.conf.StrIndex]["Director"] = Result[6].ToString();
                    }
                    break;
                case "Producer":
                    if (Result[7] != string.Empty && Result[7] != null)
                        MyFilms.r[MyFilms.conf.StrIndex]["Producer"] = Result[7].ToString();
                    break;
                case "Year":
                    if (Result[8] != string.Empty && Result[8] != null)
                    {
                        try
                        {
                            year = Convert.ToInt16(Result[8].ToString());
                        }
                        catch { }
                        MyFilms.r[MyFilms.conf.StrIndex]["Year"] = Result[8].ToString();
                    }
                    break;
                case "Country":
                    if (Result[9] != string.Empty && Result[9] != null)
                        MyFilms.r[MyFilms.conf.StrIndex]["Country"] = Result[9].ToString();
                    break;
                case "Category":
                    if (Result[10] != string.Empty && Result[10] != null)
                        MyFilms.r[MyFilms.conf.StrIndex]["Category"] = Result[10].ToString();
                    break;
                case "URL":
                    if (Result[11] != string.Empty && Result[11] != null)
                        if (MyFilms.conf.StrStorage != "URL")
                            MyFilms.r[MyFilms.conf.StrIndex]["URL"] = Result[11].ToString();
                    break;
                case "all":
                case "missing":
                    if (Result[0] != string.Empty && Result[0] != null)
                    {
                        title = Result[0].ToString();
                        wtitle = MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString();
                        if (wtitle.Contains(MyFilms.conf.TitleDelim))
                            wtitle = wtitle.Substring(wtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1);
                        if (wtitle != title)
                            Remove_Backdrops_Fanart(wtitle, true);
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString()) || !onlymissing)
                        {

                            if (MyFilms.conf.StrTitle1 == "OriginalTitle")
                                MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] = moviehead + title;
                            else
                                MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] = title;
                        }
                    }
                    if (Result[1] != string.Empty && Result[1] != null)
                    {
                        ttitle = Result[1].ToString();
                        wtitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                        if (wtitle.Contains(MyFilms.conf.TitleDelim))
                            wtitle = wtitle.Substring(wtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1);
                        if (wtitle != ttitle)
                            Remove_Backdrops_Fanart(wtitle, true);
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString()) || !onlymissing)
                        {
                            if (MyFilms.conf.StrTitle1 == "TranslatedTitle")
                                MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] = moviehead + ttitle;
                            else
                                MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] = ttitle;
                        }
                    }
                    if (Result[2] != string.Empty && Result[2] != null)
                    {
                        if (Img_Path_Type.ToLower() == "true")
                            Result[2] = Result[2].Substring(Result[2].LastIndexOf("\\") + 1).ToString();
                        if (Img_Path.Length > 0)
                            if (Img_Path.EndsWith("\\"))
                                Result[2] = Img_Path + Result[2];
                            else
                                Result[2] = Img_Path + "\\" + Result[2];
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString()) || !onlymissing)
                            MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = Result[2].ToString();
                    }

                    if (Result[3] != string.Empty && Result[3] != null)
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Description"].ToString()) || !onlymissing)
                            MyFilms.r[MyFilms.conf.StrIndex]["Description"] = Result[3].ToString();
                    if (Result[4] != string.Empty && Result[4] != null)
                    {
                        if (Result[4].ToString().Length > 0)
                        {
                            NumberFormatInfo provider = new NumberFormatInfo();
                            provider.NumberDecimalSeparator = ".";
                            provider.NumberDecimalDigits = 1;
                            decimal wnote = Convert.ToDecimal(Result[4], provider);
                            if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString()) || !onlymissing)
                                MyFilms.r[MyFilms.conf.StrIndex]["Rating"] = string.Format("{0:F1}", wnote);
                        }
                    }
                    if (Result[5] != string.Empty && Result[5] != null)
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Actors"].ToString()) || !onlymissing)
                            MyFilms.r[MyFilms.conf.StrIndex]["Actors"] = Result[5].ToString();
                    if (Result[6] != string.Empty && Result[6] != null)
                    {
                        director = Result[6].ToString();
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Director"].ToString()) || !onlymissing) 
                            MyFilms.r[MyFilms.conf.StrIndex]["Director"] = Result[6].ToString();
                    }
                    if (Result[7] != string.Empty && Result[7] != null)
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Producer"].ToString()) || !onlymissing) 
                            MyFilms.r[MyFilms.conf.StrIndex]["Producer"] = Result[7].ToString();
                    if (Result[8] != string.Empty && Result[8] != null)
                    {
                        try
                        {
                            year = Convert.ToInt16(Result[8].ToString());
                        }
                        catch { }
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString()) || !onlymissing) 
                            MyFilms.r[MyFilms.conf.StrIndex]["Year"] = Result[8].ToString();
                    }
                    if (Result[9] != string.Empty && Result[9] != null)
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Country"].ToString()) || !onlymissing) 
                            MyFilms.r[MyFilms.conf.StrIndex]["Country"] = Result[9].ToString();
                    if (Result[10] != string.Empty && Result[10] != null)
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Category"].ToString()) || !onlymissing) 
                            MyFilms.r[MyFilms.conf.StrIndex]["Category"] = Result[10].ToString();
                    if (Result[11] != string.Empty && Result[11] != null)
                        if (MyFilms.conf.StrStorage != "URL")
                            if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["URL"].ToString()) || !onlymissing) 
                                MyFilms.r[MyFilms.conf.StrIndex]["URL"] = Result[11].ToString();
                    break;

                default:
                    break;
            }
            Update_XML_database();
            LogMyFilms.Info("MF: Database Updated for : " + ttitle);
            if (title.Length > 0 && MyFilms.conf.StrFanart) // Get Fanart - ToDo Guzzi: Use local Fanart, if chosen ?
            {
                System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, (int)year, director, MyFilms.conf.StrPathFanart, true, false, MyFilms.conf.StrTitle1.ToString());
                //System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, (int)year, director, MyFilms.conf.StrPathFanart, true, false);
            }
        }

        //-------------------------------------------------------------------------------------------
        //  Create Thumb via MTN (MovieThumbNailer) from movie itself
        //-------------------------------------------------------------------------------------------        
        public static void CreateThumbFromMovie()
        {
            //XmlConfig XmlConfig = new XmlConfig();
            //string Img_Path = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Image_Download_Filename_Prefix", "");
            //string Img_Path_Type = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Store_Image_With_Relative_Path", "false");

            string path = MyFilms.r[MyFilms.conf.StrIndex]["Source"].ToString();
            string strThumb = MyFilms.conf.StrPathImg + "\\" + MyFilms.r[MyFilms.conf.StrIndex]["Number"].ToString() + ".jpg";


            //if (Img_Path_Type.ToLower() == "true")
            //    MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = strThumb;

            MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = strThumb;
            
            
            if (File.Exists(strThumb))
            {
                return ;
            }

            // Do not try to create thumbnails for DVDs
            if (path.Contains("VIDEO_TS\\VIDEO_TS.IFO"))
            {
                return ;
            }

            MediaPortal.Services.IVideoThumbBlacklist blacklist = MediaPortal.Services.GlobalServiceProvider.Get<MediaPortal.Services.IVideoThumbBlacklist>();
            if (blacklist != null && blacklist.Contains(path))
            {
                LogMyFilms.Debug("Skipped creating thumbnail for {0}, it has been blacklisted because last attempt failed", path);
                return ;
            }


            System.Drawing.Image thumb = null;
            try
            {
                // CreateVideoThumb(string aVideoPath, string aThumbPath, bool aCacheThumb, bool aOmitCredits);
                bool success = VideoThumbCreator.CreateVideoThumb(path, strThumb, true, false);
                if (!success)
                    return;
                else
                    return;
            }
            catch (System.Runtime.InteropServices.COMException comex)
            {
                if (comex.ErrorCode == unchecked((int)0x8004B200))
                {
                    LogMyFilms.Warn("Could not create thumbnail for {0} [Unknown error 0x8004B200]", path);
                }
                else
                {
                    LogMyFilms.Error("Could not create thumbnail for {0}", path);
                    LogMyFilms.Error(comex);
                }
            }
            catch (Exception ex)
            {
                LogMyFilms.Error("Could not create thumbnail for {0}", path);
                LogMyFilms.Error(ex);
            }
            finally
            {
                if (thumb != null)
                    thumb.Dispose();
                if (!File.Exists(strThumb) && blacklist != null)
                {
                    blacklist.Add(path);
                }
            }

            Update_XML_database();
            LogMyFilms.Info("MF: Database Updated for created PictureThumb: " + strThumb);
        }

        //-------------------------------------------------------------------------------------------
        // Grab XBMC (movie.nfo) kompatible Movie Details Informations and update the XML database and refresh screen
        // Last Parameter is set to overwrite all existing data - when set to false it only updates missing infos (important for batch import)
        // bool overwrite: true owerwrites all existing infos - false updates only empty values
        //-------------------------------------------------------------------------------------------        
        public static void Grab_Nfo_Details(DataRow[] r1, int Index, int GetID, bool overwrite)
        {

            string FullMovieName = string.Empty;
            if (MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] != null && MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                FullMovieName = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
            if (FullMovieName.IndexOf(MyFilms.conf.TitleDelim) > 0)
                FullMovieName = FullMovieName.Substring(FullMovieName.IndexOf(MyFilms.conf.TitleDelim) + 1);

            string MovieName = FullMovieName;
            string MovieHierarchy = string.Empty;
            if (MyFilms.conf.TitleDelim.Length > 0)
            {
                MovieName = FullMovieName.Substring(FullMovieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
                MovieHierarchy = FullMovieName.Substring(0, FullMovieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
            }

            //string titlename = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
            //string titlename2 = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString();
            string directoryname = "";
            string movieName = "";
            string nfofile = "";

            //Retrieve original directory of mediafiles
            //directoryname
            movieName = (string)MyFilms.r[Index][MyFilms.conf.StrStorage].ToString().Trim();
            movieName = movieName.Substring(movieName.LastIndexOf(";") + 1);
            LogMyFilms.Debug("MyFilmsDetails (grabb_Nfo_Details) Splittet Mediadirectoryname: '" + movieName.ToString() + "'");
            try
            {
                directoryname = System.IO.Path.GetDirectoryName(movieName); 
            }
            catch
            {
                directoryname = ""; 
            }
            nfofile = directoryname + "\\movie.nfo";
            LogMyFilms.Debug("MyFilmsDetails (grabb_Nfo_Details) nfo-Filename: '" + nfofile.ToString() + "'");

            if (!System.IO.File.Exists(nfofile))
            {
                LogMyFilms.Debug("MyFilmsDetails (grabb_Nfo_Details) File cannot be opened, nfo-Filename: '" + nfofile.ToString() + "'");
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                dlgOk.SetLine(1, " ");
                dlgOk.SetLine(2, "Cannot find file 'movie.nfo' in movie directory !");
                dlgOk.SetLine(3, "'" + nfofile.ToString() + "'");
                dlgOk.DoModal(GUIWindowManager.ActiveWindow);
                return;
            }
            grabb_Internet_Details_Informations("", "", "", GetID, true, true, nfofile);
        }


        //-------------------------------------------------------------------------------------------
        //  Remove Old backdrops Fanart already downloaded (case in title change)
        //-------------------------------------------------------------------------------------------        
        public static void Remove_Backdrops_Fanart(string wtitle,bool suppressDir)
        {
            if (wtitle.Length > 0)
            {
                if (wtitle.Contains(MyFilms.conf.TitleDelim))
                    wtitle = wtitle.Substring(wtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
                wtitle = Grabber.GrabUtil.CreateFilename(wtitle.ToLower()).Replace(' ', '.');
                if (suppressDir)
                {
                    try { System.IO.Directory.Delete(MyFilms.conf.StrPathFanart + "\\{" + wtitle + "}"); }
                    catch { }
                }
                else
                {
                    DirectoryInfo dirsInf = new DirectoryInfo(MyFilms.conf.StrPathFanart + "\\{" + wtitle + "}");
                    FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();
                    foreach (FileSystemInfo sfi in sfiles)
                    {
                        System.IO.File.Delete(sfi.FullName);
                    }   
                }
            }
        }
        //-------------------------------------------------------------------------------------------
        //  Dowload backdrops on theMovieDB.org
        //-------------------------------------------------------------------------------------------        
        public static void Download_Backdrops_Fanart(string wtitle, string wttitle, string director, string year, bool choose,int wGetID)
        {
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            int wyear = 0;
            try {  wyear = Convert.ToInt32(year);}
            catch { }
            System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, director, MyFilms.conf.StrPathFanart, true, choose, MyFilms.conf.StrTitle1);
            //System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, director, MyFilms.conf.StrPathFanart, true, choose);
            LogMyFilms.Debug("MF: (DownloadBackdrops) - listemovies: '" + wtitle + "', '" + wttitle + "', '" + wyear + "', '" + director + "', '" + MyFilms.conf.StrPathFanart + "', 'true', '" + choose.ToString() + "', '" + MyFilms.conf.StrTitle1 + "'");
            int listCount = listemovies.Count;
            LogMyFilms.Debug("MF: (DownloadBackdrops) - listemovies: Result Listcount: '" + listCount.ToString() + "'");
            
            //if ((listCount == 0) && (choose))
            //{
            //    //MyFilmsDetail.ShowMessageDialog("", " No results found", "");
            //    GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            //    if (dlgOK != null)
            //    {
            //        dlgOK.SetHeading("");
            //        dlgOK.SetLine(1, "No results found !");
            //        dlgOK.SetLine(2, "");
            //        dlgOK.DoModal(wGetID);
            //    }
            //}

            if (choose)
                listCount = 2;
            switch (listCount)
            {
                case 0:
                    break;
                case 1:
                    LogMyFilms.Info("MF: Fanart " + listemovies[0].Name.Substring(listemovies[0].Name.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);
                    break;
                default:

                    ArrayList wotitle_tableau = new ArrayList();
                    ArrayList wttitle_tableau = new ArrayList();
                    ArrayList wotitle_sub_tableau = new ArrayList();
                    ArrayList wttitle_sub_tableau = new ArrayList();
                    int MinChars = 2;
                    bool Filter = true;

                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                    dlg.Add("  *****  " + GUILocalizeStrings.Get(200036) + "  *****  "); //choice for changing movie filename
                    foreach (DBMovieInfo t in listemovies)
                    {
                        dlg.Add(t.Name + "  (" + t.Year + ") ");
                    }
                    if (!(dlg.SelectedLabel > -1))
                    {
                        dlg.SelectedLabel = -1;
                        dlg.DoModal(wGetID);
                    }
                    if (dlg.SelectedLabel == 0)
                    {
                        //Get SubTitles and Subwords from otitle and ttitle
                        wotitle_tableau = MyFilms.SubTitleGrabbing(wtitle.ToString());
                        wttitle_tableau = MyFilms.SubTitleGrabbing(wttitle.ToString());
                        wotitle_sub_tableau = MyFilms.SubWordGrabbing(wtitle.ToString(),MinChars,Filter); // Min 3 Chars, Filter true (no der die das)
                        wttitle_sub_tableau = MyFilms.SubWordGrabbing(wttitle.ToString(),MinChars,Filter); // Min 3 Chars, Filter true (no der die das)
                        //First Show Dialog to choose Otitle, Ttitle or substrings - or Keyboard to manually enter searchstring!!!
                        GUIDialogMenu dlgs = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        if (dlgs == null) return;
                        dlgs.Reset();
                        dlgs.SetHeading(GUILocalizeStrings.Get(924) + " - Wähle Suchbegriff"); // menu
                        dlgs.Add("  *****  " + "Manuelle Eingabe über Tastatur" + "  *****  ");
                        dlgs.Add(wtitle); //Otitle
                        dlgs.Add(wttitle); //Ttitle
                        foreach (object t in wotitle_tableau)
                        {
                            if (t.ToString().Length > 1) dlgs.Add(t.ToString());
                        }
                        foreach (object t in wttitle_tableau)
                        {
                            if (t.ToString().Length > 1) dlgs.Add(t.ToString());
                        }
                        foreach (object t in wotitle_sub_tableau)
                        {
                            if (t.ToString().Length > 1) dlgs.Add(t.ToString());
                        }
                        foreach (object t in wttitle_sub_tableau)
                        {
                            if (t.ToString().Length > 1) dlgs.Add(t.ToString());
                        }
                        //Now all titles and Substrings listed in dialog !
                        //dlgs.Add("  *****  " + GUILocalizeStrings.Get(200036) + "  *****  "); //choice for changing movie filename
                        if (!(dlgs.SelectedLabel > -1))
                        {
                            dlgs.SelectedLabel = -1;
                            dlgs.DoModal(wGetID);
                        }
                        //LogMyFilms.Debug("MF: (SingleFanartGrabber) - Info about Selected DIalog Searchstring: DialofSelectedLabelText: '" + dlgs.SelectedLabelText.ToString() + "'");
                        //LogMyFilms.Debug("MF: (SingleFanartGrabber) - Info about Selected DIalog Searchstring: DialofSelectedLabel: '" + dlgs.SelectedLabel.ToString() + "'");
                        if (dlgs.SelectedLabel == 0)
                        {
                            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                            if (null == keyboard) return;
                            keyboard.Reset();
                            keyboard.Text = wtitle;
                            keyboard.DoModal(wGetID);
                            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                            {
                                Remove_Backdrops_Fanart(wttitle, true);
                                Remove_Backdrops_Fanart(wtitle, true);
                                Download_Backdrops_Fanart(keyboard.Text, wttitle, string.Empty, string.Empty, true, wGetID);
                            }
                            break;
                        }
                        if (dlgs.SelectedLabel > 0)
                        {
                            Download_Backdrops_Fanart(dlgs.SelectedLabelText, wttitle, string.Empty, string.Empty, true, wGetID);
                            //Download_Backdrops_Fanart(string wtitle, string wttitle, string director, string year, bool choose,int wGetID)
                            break;
                        }
                    }
                    if (dlg.SelectedLabel > 0)
                    {
                        bool first = true;
                        string filename = string.Empty;
                        string filename1 = string.Empty;
                        if (MyFilms.conf.StrTitle1 == "OriginalTitle")
                            wttitle = wtitle;
                        foreach (string backdrop in listemovies[dlg.SelectedLabel - 1].Backdrops)
                        {
                            filename1 = Grabber.GrabUtil.DownloadBacdropArt(MyFilms.conf.StrPathFanart, backdrop, wttitle, true, first, out filename);
                            LogMyFilms.Info("MF: Fanart " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);

                            if (filename == string.Empty)
                                filename = filename1;
                            if (!(filename == "already" && filename1 == "already"))
                                filename = "added";
                            first = false;
                        }
                        listemovies[0].Name = filename;
                    }
                    break;
            }
        }
        //-------------------------------------------------------------------------------------------
        //  Search Fanart Thumbs 
        //                          wtitle2 = Translated Title if any or Original Title
        //                          main = true for main screen and false for detailed
        //                          searched = dir for Directory searched (detail screen and control multiImage)or file 
        //                          rep = true if the selected Item is a grouped Item (country, genre, year...) on main screen
        //                          filecover = name of the file cover for using 'Default Cover for missing Fanart'
        //      value returned string[2]
        //                          [0] = file or directory found (if not " ")
        //                          [1] = file or dir
        //-------------------------------------------------------------------------------------------

        public static string[] Search_Fanart(string wtitle2, bool main, string searched, bool rep, string filecover, string group)
        //                     Search_Fanart(wlabel, true, "file", false, facadeView.SelectedListItem.ThumbnailImage.ToString(), string.Empty);
        {
            //LogMyFilms.Debug("MF: (SearchFanart) - Vars: wtitle2 = '" + wtitle2 + "'");
            //LogMyFilms.Debug("MF: (SearchFanart) - Vars: main (true for mainscreen, false for Detail) = '" + main + "'");
            //LogMyFilms.Debug("MF: (SearchFanart) - Vars: searched (dir or file) = '" + searched + "'");
            //LogMyFilms.Debug("MF: (SearchFanart) - Vars: rep (true for grouped view) = '" + rep + "'");
            //LogMyFilms.Debug("MF: (SearchFanart) - Vars: filecover = '" + filecover + "'");
            //LogMyFilms.Debug("MF: (SearchFanart) - Vars: group = '" + group + "'");
            //LogMyFilms.Debug("MF: (SearchFanart) - Config: MyFilms.conf.StrFanart = '" + MyFilms.conf.StrFanart + "'");
            string[] wfanart = new string[2];
            wfanart[0] = " ";
            wfanart[1] = " ";
            if (MyFilms.conf.StrFanart)
            {
                if (wtitle2.Contains(MyFilms.conf.TitleDelim))
                  wtitle2 = wtitle2.Substring(wtitle2.LastIndexOf(MyFilms.conf.TitleDelim) + 1); // Removed "trim", as there is no matching in details, if spacees are removed! old: wtitle2 = wtitle2.Substring(wtitle2.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
                wtitle2 = Grabber.GrabUtil.CreateFilename(wtitle2.ToLower()).Replace(' ', '.');
                //LogMyFilms.Debug("MF: (SearchFanart) - wtitle2-cleaned = '" + wtitle2 + "'");
                //LogMyFilms.Debug("MF: (SearchFanart) - MyFilms.conf.StrFanart = '" + MyFilms.conf.StrFanart + "'");

                if (!MyFilms.conf.StrFanart)
                    return wfanart;

                string safeName = string.Empty;
                if (rep)
                {
                    if (group == "country" || group == "year" || group == "category")
                    {
                        if (!System.IO.Directory.Exists(MyFilms.conf.StrPathFanart + "\\_Group"))
                            System.IO.Directory.CreateDirectory(MyFilms.conf.StrPathFanart + "\\_Group");
                        if (!System.IO.Directory.Exists(MyFilms.conf.StrPathFanart + "\\_Group\\" + group))
                            System.IO.Directory.CreateDirectory(MyFilms.conf.StrPathFanart + "\\_Group\\" + group);
                        safeName = MyFilms.conf.StrPathFanart + "\\_Group\\" + group + "\\{" + wtitle2 + "}";
                    }
                    else
                        return wfanart;
                }
                else 
             
                    if ((MyFilms.conf.StrPathFanart.ToString() + wtitle2 + "\\{" + wtitle2 + "}.jpg").Length > 259) // Added to avoid crash with very long filenames - better is if user configures titledelimiters properly !
                     {
                         return wfanart;
                     }
                    else
                    {
                        safeName = MyFilms.conf.StrPathFanart + "\\{" + wtitle2 + "}";
                    }
                //LogMyFilms.Debug("MF: (SearchFanart) - safename = '" + safeName + "'");

                
                try //Added to avoid crash with very long filenames - better is if user configures titledelimiters properly !
                    {
                        FileInfo wfile = new FileInfo(safeName + "\\{" + wtitle2 + "}.jpg");
                    }
                catch (Exception e)
                    {
                        LogMyFilms.Debug("MF: Title too long to create fanart path/filename from it - not loading fanart! - Exception: " + e.ToString());
                        return wfanart;
                    }
                
                //LogMyFilms.Debug("MF: (SearchFanart) - safename(file) = '" + wfile + "'");
                LogMyFilms.Debug("MF: (SearchFanart) - safename(file&ext) = '" + (safeName + "\\{" + wtitle2 + "}.jpg") + "'");
                if (((main) || (searched == "file")) && (System.IO.File.Exists(safeName + "\\{" + wtitle2 + "}.jpg")))
                {
                    wfanart[0] = safeName + "\\{" + wtitle2 + "}.jpg";
                    wfanart[1] = "file";
                    return wfanart;
                }
                if (System.IO.Directory.Exists(safeName))
                {
                    if ((main) || (searched == "file"))
                    {
                        if (System.IO.Directory.GetFiles(safeName).Length > 0)
                        {
                            wfanart[0] = System.IO.Directory.GetFiles(safeName)[0].ToString();
                            wfanart[1] = "file";
                            return wfanart;
                        }
                    }
                    else
                    {
                        if (System.IO.Directory.GetFiles(safeName).Length > 0)
                        {
                            wfanart[0] = safeName;
                            wfanart[1] = "dir";
                            return wfanart;
                        }
                    }
                }
                else
                {
                    try { System.IO.Directory.CreateDirectory(safeName); }
                    catch { }
                }
                if ((MyFilms.conf.StrFanartDflt) && !(rep))
                {
                    wfanart[0] = filecover.ToString();
                    wfanart[1] = "file";
                    //Added Guzzi - Fix that no fanart was returned ...
                    return wfanart;
                }
            }
            
            LogMyFilms.Debug("MF: (SearchFanart) - Fanart not configured: wfanart[0,1]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
            return wfanart;
        }
        //-------------------------------------------------------------------------------------------
        //  Control search Numeric : only numerics
        //-------------------------------------------------------------------------------------------        
        private string control_searchNum(string stext)
        {
            Regex maRegexp = new Regex("^[0-9]{1,2}[\\.,]?[0-9]?$");
            if (!maRegexp.IsMatch(stext))
            {
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading("Error");//rating Integer from 0 to 10
                dlgOk.SetLine(1, "Syntax : nn.n");
                dlgOk.DoModal(GetID);
                return null;
            }
            try
            {
                double wrating = Convert.ToDouble(stext);
                NumberFormatInfo nfi = new CultureInfo("en-US", true).NumberFormat;
                nfi.NumberDecimalDigits = 1;
                if ((wrating > 0) && (wrating < 11))
                    return wrating.ToString("N", nfi);
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        // selects records and sets StrIndex based on ItemId (leaves unchanged if ItemId=-1). 
        private void afficher_init(int ItemId)
        {
            StrMax = MyFilms.r.Length;
        }

        private void afficher_detail(bool searchPicture)
        {
            //-----------------------------------------------------------------------------------------------------------------------
            //    Load Detailed Info
            //-----------------------------------------------------------------------------------------------------------------------
            if (MyFilms.conf.StrIndex > MyFilms.r.Length - 1)
                MyFilms.conf.StrIndex = MyFilms.r.Length - 1;
            if (MyFilms.conf.StrIndex == -1)
            {
                MediaPortal.GUI.Library.Action actionType = new MediaPortal.GUI.Library.Action();
                actionType.wID = MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU;
                base.OnAction(actionType);
                return;
            }
            if (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString().Length > 0)
            {
                int TitlePos = (MyFilms.conf.StrTitleSelect.Length > 0) ? MyFilms.conf.StrTitleSelect.Length + 1 : 0; //only display rest of title after selected part common to group
                MyFilms.conf.StrTIndex = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString().Substring(TitlePos);
                GUIControl.ShowControl(GetID, (int)Controls.CTRL_Title);
            }
            else
                GUIControl.HideControl(GetID, (int)Controls.CTRL_Title);
            if (MyFilms.conf.StrTitle2 != "(none)")
            {
                if ((MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() == MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString()) || (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString().Length == 0))
                    GUIControl.HideControl(GetID, (int)Controls.CTRL_OTitle);
                else
                    GUIControl.ShowControl(GetID, (int)Controls.CTRL_OTitle);
            }
            else
                GUIControl.HideControl(GetID, (int)Controls.CTRL_OTitle);

            string file = "false";
            if (MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().Length > 0)
            {
                if ((MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().IndexOf(":\\") == -1) && (MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().Substring(0, 2) != "\\\\"))
                    file = MyFilms.conf.StrPathImg + "\\" + MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString();
                else
                    file = MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString();
            }
            else
                file = string.Empty;
            if (!System.IO.File.Exists(file))
                file = MyFilms.conf.DefaultCover;
            
            //Should not Disable because of SpeedThumbs - Not working here .....
            setGUIProperty("picture", file);
            // ToDo: Add for ImageSwapper Coverart (coverImage)
            //cover.Filename = file;

            // Logos:
            this.Load_Logos(MyFilms.r[MyFilms.conf.StrIndex]);
          
            //ImageSwapper backdrop = new ImageSwapper();
            string[] wfanart = new string[2];
            string wtitle = MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString();
      			//Added by Guzzi to fix Fanartproblem when Mastertitle is set to OriginalTitle
            if (MyFilms.conf.StrTitle1 != "OriginalTitle")
            {

                if (MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] != null && MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                    wtitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
            }
            if (ImgFanartDir != null)
            {
                wfanart = Search_Fanart(wtitle, false, "dir", false, file, string.Empty);
                LogMyFilms.Debug("MF: (afficher_detail): Backdrops-File (dir): wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
            }
            else
            {
                wfanart = Search_Fanart(wtitle, false, "file", false, file, string.Empty);
                LogMyFilms.Debug("MF: (afficher_detail): Backdrops-File (file): wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
            }

            if (wfanart[0] == " ")
            {
                if (ImgFanartDir != null)
                    ImgFanartDir.PreAllocResources();
                GUIControl.HideControl(GetID, 35);
            }
            else
            {
                GUIControl.ShowControl(GetID, 35);
                if (wfanart[1] == "dir")
                {
                    ImgFanartDir.TexturePath = wfanart[0];
                    ImgFanartDir.PreAllocResources();
                    ImgFanartDir.AllocResources();
                    GUIControl.HideControl(GetID, (int)Controls.CTRL_Fanart);
                    GUIControl.ShowControl(GetID, (int)Controls.CTRL_FanartDir);
                }
                else
                {
                    if (ImgFanartDir != null)
                    {
                        ImgFanartDir.PreAllocResources();
                        GUIControl.HideControl(GetID, (int)Controls.CTRL_FanartDir);
                    }
                    ImgFanart.SetFileName(wfanart[0]);
                    setGUIProperty("fanart", wfanart[0].ToString()); 
                    GUIControl.ShowControl(GetID, (int)Controls.CTRL_Fanart);
                }
            }
            if (MyFilms.conf.StrStorage.Length != 0 && MyFilms.conf.StrStorage != "(none)")
            {
                if (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().Length > 0)
                {
                    int at = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().IndexOf(";", 0, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().Length);
                    if (at == -1)
                        file = SearchMovie(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().Substring(0, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().Length).Trim().ToString(), MyFilms.conf.StrDirStor);
                    else
                        file = SearchMovie(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString().Substring(0, at).Trim().ToString(), MyFilms.conf.StrDirStor);
                }
                else
                    file = "false";
                if (file != "false" && (file.Length > 0))
                    GUIControl.ShowControl(GetID, (int)Controls.CTRL_ImgDD);
                else
                    GUIControl.HideControl(GetID, (int)Controls.CTRL_ImgDD);
            }
            else
                GUIControl.HideControl(GetID, (int)Controls.CTRL_ImgDD);

            Load_Detailed_DB(MyFilms.conf.StrIndex, true);
            if (MyFilms.conf.StrIndex == StrMax - 1)
            {
                GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnNext);
                GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnLast);
            }
            else
            {
                GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnNext);
                GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnLast);
            }
            if (MyFilms.conf.StrIndex == 0)
            {
                GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnPrior);
                GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnFirst);
            }
            else
            {
                GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnPrior);
                GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnFirst);
            }
        }


        private void Load_Logos(DataRow row)
        {
          //if ((ImgID2001 != null) && (ImgID2002 != null) && (MyFilms.conf.StrLogos))
          //{
          //if ((ImgID2001.XPosition == ImgID2002.XPosition) && (ImgID2001.YPosition == ImgID2002.YPosition))
          //{
          //    try
          //    {
          //        string wlogos = Logos.Build_Logos(MyFilms.r[MyFilms.conf.StrIndex], "ID2003", ImgID2001.Height + ImgID2002.Height, ImgID2001.Width + ImgID2002.Width, ImgID2001.XPosition, ImgID2001.YPosition, 1, GetID);
          //        setGUIProperty("logos_id2001", wlogos);
          //        GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2001);
          //        GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2002);
          //        GUIControl.HideControl(GetID, (int)Controls.CTRL_Format);
          //        GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
          //    }
          //    catch (Exception e)
          //    {
          //        LogMyFilms.Error("MF: " + e.Message);
          //    }
          //}
          //else
          //    {
          //        try
          //        {
          //            setGUIProperty("logos_id2001", Logos.Build_Logos(MyFilms.r[MyFilms.conf.StrIndex], "ID2001", ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, 1, GetID));
          //            //GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2001);
          //            //GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
          //            GUIControl.HideControl(GetID, (int)Controls.CTRL_Format);
          //        }
          //        catch (Exception e)
          //        {
          //            LogMyFilms.Error("MF: " + e.Message);
          //        }
          //        try
          //        {
          //            setGUIProperty("logos_id2002", Logos.Build_Logos(MyFilms.r[MyFilms.conf.StrIndex], "ID2002", ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, 1, GetID));
          //            //GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2002);
          //            //GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2002);
          //        }
          //        catch (Exception e)
          //        {
          //            LogMyFilms.Error("MF: " + e.Message);
          //        }
          //    }
          //}
          //else
          //{
          if ((ImgID2001 != null) && (MyFilms.conf.StrLogos))
          {
            try
            {
              setGUIProperty("logos_id2001", Logos.Build_Logos(row, "ID2001", ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, GetID));
              //GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2001);
              //GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
              //GUIControl.HideControl(GetID, (int)Controls.CTRL_Format);
            }
            catch (Exception e)
            {
              LogMyFilms.Error("MF: " + e.Message);
            }
          }
          else
          {
            clearGUIProperty("logos_id2001");
            //GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2001);
            //GUIControl.ShowControl(GetID, (int)Controls.CTRL_Format);
          }
          if ((ImgID2002 != null) && (MyFilms.conf.StrLogos))
          {
            try
            {
              setGUIProperty("logos_id2002", Logos.Build_Logos(row, "ID2002", ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, GetID));
              //GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2002);
              //GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2002);
            }
            catch (Exception e)
            {
              LogMyFilms.Error("MF: " + e.Message);
            }
          }
          else
            clearGUIProperty("logos_id2002");
          //GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2002);
          //}

          if ((ImgID2003 != null) && (MyFilms.conf.StrLogos))
          {
            try
            {
              setGUIProperty("logos_id2003", Logos.Build_Logos(row, "ID2003", ImgID2003.Height, ImgID2003.Width, ImgID2003.XPosition, ImgID2003.YPosition, GetID));
            }
            catch (Exception e)
            {
              LogMyFilms.Error("MF: " + e.Message);
            }
          }
          else
            clearGUIProperty("logos_id2003");

          if ((ImgID2012 != null) && (MyFilms.conf.StrLogos))
          {
            try
            {
              setGUIProperty("logos_id2012", Logos.Build_Logos(row, "ID2012", ImgID2012.Height, ImgID2012.Width, ImgID2012.XPosition, ImgID2012.YPosition, GetID));
            }
            catch (Exception e)
            {
              LogMyFilms.Error("MF: " + e.Message);
            }
          }
          else
            clearGUIProperty("logos_id2012");
        }


        //-------------------------------------------------------------------------------------------
        //  initialize exported fields to skin as '#myfilms.<ant db column name> 
        //-------------------------------------------------------------------------------------------
        public static void Init_Detailed_DB()
        {
            AntMovieCatalog ds = new AntMovieCatalog();
            foreach (DataColumn dc in ds.Movie.Columns)
            {
                //clearGUIProperty("db." + dc.ColumnName.ToLower() + ".label"); // Don't Clear Labels - they're only set once when plugin start !
                clearGUIProperty("db." + dc.ColumnName.ToLower() + ".value");
            }

            //Clear userdefined properties
            clearGUIProperty("db.calc.format.value");
            clearGUIProperty("db.calc.aspectratio.value");
            clearGUIProperty("db.calc.imageformat.value");

            clearGUIProperty("user.mastertitle.label");
            clearGUIProperty("user.mastertitle.value");
            clearGUIProperty("user.secondarytitle.label");
            clearGUIProperty("user.secondarytitle.value");
            clearGUIProperty("user.item1.label");
            clearGUIProperty("user.item1.field");
            clearGUIProperty("user.item1.value");
            clearGUIProperty("user.item2.label");
            clearGUIProperty("user.item2.field");
            clearGUIProperty("user.item2.value");
            clearGUIProperty("user.item3.label");
            clearGUIProperty("user.item3.field");
            clearGUIProperty("user.item3.value");
            clearGUIProperty("user.source.value");
            clearGUIProperty("user.sourcetrailer.value");
        }

        //-------------------------------------------------------------------------------------------
        //  Load detailed db fields : export fields to skin as '#myfilms.<ant db column name> 
        //-------------------------------------------------------------------------------------------
        public static void Load_Detailed_DB(int ItemId, bool wrep)
        {
     
            string wstrformat = "";
            AntMovieCatalog ds = new AntMovieCatalog();
            
            foreach (DataColumn dc in ds.Movie.Columns)
            {
                string wstring = "";
                //LogMyFilms.Debug("MF: PropertyManager: Set Properties for DB Column '" + dc.ColumnName + "' - '" + BaseMesFilms.Translate_Column(dc.ColumnName) + "'");

                if (MyFilms.r[ItemId][dc.ColumnName] != null)
                {
                    //Added by Guzzi to set userdefined properties
                    if (MyFilms.conf.Stritem1.ToLower() == (dc.ColumnName.ToLower()))
                        if (wrep)
                            {
                                setGUIProperty("user.item1.label", MyFilms.conf.Strlabel1);
                                if (MyFilms.conf.Stritem1.ToLower() == "date")
                                    setGUIProperty("user.item1.field", "w" + MyFilms.conf.Stritem1.ToLower());
                                else
                                    setGUIProperty("user.item1.field", MyFilms.conf.Stritem1.ToLower());
                                setGUIProperty("user.item1.value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                            }
                            else
                            {
                                clearGUIProperty("user.item1.label");
                                clearGUIProperty("user.item1.field");
                                clearGUIProperty("user.item1.value");
                            }
                    if (MyFilms.conf.Stritem2.ToLower() == (dc.ColumnName.ToLower()))
                        if (wrep)
                            {
                                setGUIProperty("user.item2.label", MyFilms.conf.Strlabel2);
                                if (MyFilms.conf.Stritem2.ToLower() == "date")
                                    setGUIProperty("user.item2.field", "w" + MyFilms.conf.Stritem2.ToLower());
                                else
                                    setGUIProperty("user.item2.field", MyFilms.conf.Stritem2.ToLower());
                                setGUIProperty("user.item2.value",MyFilms.r[ItemId][dc.ColumnName].ToString());
                            }
                            else
                            {
                                clearGUIProperty("user.item2.label");
                                clearGUIProperty("user.item2.field");
                                clearGUIProperty("user.item2.value");
                            }
                    if (MyFilms.conf.Stritem3.ToLower() == (dc.ColumnName.ToLower()))
                        if (wrep)
                            {
                                setGUIProperty("user.item3.label", MyFilms.conf.Strlabel3); // not currently used in myfilms
                                if (MyFilms.conf.Stritem3.ToLower() == "date")
                                    setGUIProperty("user.item3.field", "w" + MyFilms.conf.Stritem3.ToLower());
                                else
                                    setGUIProperty("user.item3.field", MyFilms.conf.Stritem3.ToLower());
                                setGUIProperty("user.item3.value",MyFilms.r[ItemId][dc.ColumnName].ToString());
                            }
                            else
                            {
                                clearGUIProperty("user.item3.label");
                                clearGUIProperty("user.item3.field");
                                clearGUIProperty("user.item3.value");
                            }

                    if (wrep && (MyFilms.conf.StrStorage.ToLower() == (dc.ColumnName.ToLower())))
                    {
                        setGUIProperty("user.source.value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                    }

                    if (wrep && (MyFilms.conf.StrStorageTrailer.ToLower() == (dc.ColumnName.ToLower())))
                    {
                        setGUIProperty("user.sourcetrailer.value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                    }

                    switch (dc.ColumnName.ToLower())
                    {
                        case "translatedtitle":
                        case "originaltitle":
                        case "formattedtitle":
                            if (wrep)
                                if (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                                    if (MyFilms.r[ItemId][dc.ColumnName].ToString().Contains(MyFilms.conf.TitleDelim))
                                        wstring = MyFilms.r[ItemId][dc.ColumnName].ToString().Substring(MyFilms.r[ItemId][dc.ColumnName].ToString().LastIndexOf(MyFilms.conf.TitleDelim) + 1);
                                    else
                                        wstring = MyFilms.r[ItemId][dc.ColumnName].ToString();
                            setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);

                            if ((MyFilms.conf.StrTitle1.ToLower() == (dc.ColumnName.ToLower())))
                                if (wrep)
                                    setGUIProperty("user.mastertitle.value", wstring);
                                    else
                                    clearGUIProperty("user.mastertitle.value");
                            if ((MyFilms.conf.StrTitle2.ToLower() == (dc.ColumnName.ToLower())))
                                if (wrep)
                                    setGUIProperty("user.secondarytitle.value", wstring);
                                    else
                                    clearGUIProperty("user.secondarytitle.value");
                            break;

                        case "length":
                        case "length_num":
                            if (wrep)
                                if (MyFilms.r[ItemId]["Length"].ToString().Length > 0)
                                    wstring = MyFilms.r[ItemId]["Length"].ToString();
                            setGUIProperty("db.length.value", wstring);
                            break;
                        case "actors":
                            if (wrep)
                                if (MyFilms.r[ItemId]["Actors"].ToString().Length > 0)
                                {
                                    wstring = MyFilms.r[ItemId]["Actors"].ToString().Replace('|', '\n');
                                    wstring = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wstring.ToString()));
                                }
                            setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                            break;
                        case "description":
                        case "comments":
                            if (wrep)
                                if (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                                {
                                    wstring = System.Web.HttpUtility.HtmlEncode(MyFilms.r[ItemId][dc.ColumnName].ToString().Replace('’', '\''));
                                    wstring = wstring.ToString().Replace('|', '\n');
                                    wstring = wstring.ToString().Replace('…', '.');
                                    wstring = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wstring.ToString()));
                                }
                            setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                            break;
                        case "date":
                            if (wrep)
                                if (MyFilms.r[ItemId]["Date"].ToString().Length > 0)
                                    wstring = MyFilms.r[ItemId][dc.ColumnName].ToString();
                            setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                            break;
                        case "videoformat":
                            if (wrep)
                                if (MyFilms.r[ItemId]["VideoFormat"].ToString().Length > 0)
                                    wstring = MyFilms.r[ItemId][dc.ColumnName].ToString();
                            setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                            wstrformat = "V:" + MyFilms.r[ItemId]["VideoFormat"].ToString();
                            break;
                        case "audioformat":
                            if (wrep)
                            {
                                if (MyFilms.r[ItemId]["AudioFormat"].ToString().Length > 0)
                                {
                                    wstring = MyFilms.r[ItemId][dc.ColumnName].ToString();
                                    if (wstrformat.Length > 1)
                                        wstrformat = wstrformat + ",A:" + MyFilms.r[ItemId]["AudioFormat"].ToString();
                                    else
                                        wstrformat = "A:" + MyFilms.r[ItemId]["AudioFormat"].ToString();
                                }
                                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                                setGUIProperty("db.calc.format" + ".value", wstrformat);
                            }
                            break;
                        case "rating":
                            wstring = "0";
                            if ((wrep) && (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                                wstring = MyFilms.r[ItemId][dc.ColumnName].ToString();
                            try
                            {
                                MyFilms.conf.W_rating = (decimal)MyFilms.r[ItemId][dc.ColumnName];
                            }
                            catch
                            {
                                MyFilms.conf.W_rating = 0;
                            }
                            setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                            break;
                        case "contents_id":
                        case "dateadded":
                        case "picture":
                            break;
                        case "resolution":
                            decimal aspectratio = 0; 
                            string ar = " ";
                            if ((wrep) && (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                                try
                                    {
                                        wstring = MyFilms.r[ItemId][dc.ColumnName].ToString();
                                        //LogMyFilms.Debug("MF: Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                                        setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                                        //LogMyFilms.Debug("MF: (Load_Detailed_DB): Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                                        decimal w_hsize;
                                        decimal w_vsize;
                                        string[] arSplit;
                                        string[] Sep = new string[] { "x" };
                                        arSplit = MyFilms.r[ItemId][dc.ColumnName].ToString().Split(Sep, StringSplitOptions.RemoveEmptyEntries); // remove entries empty // StringSplitOptions.None);//will add "" entries also
                                        w_hsize = (decimal)Convert.ToInt32(arSplit[0]);
                                        w_vsize = (decimal)Convert.ToInt32(arSplit[1]);
                                        // To Check if/why exception eccurs
                                        //LogMyFilms.Debug("MF: hsize - wsize: '" + w_hsize + " - " + w_vsize + "'");
                                        aspectratio = (w_hsize / w_vsize);
                                        aspectratio = Math.Round(aspectratio, 2);
                                        //Formats:
                                        //1,33 -> 4:3
                                        //1,78 -> 16:9 / widescreen
                                        //1,85 -> widescreen
                                        //2,35+ -> cinemascope
                                        if (aspectratio < (decimal)(1.4)) ar = "4:3";
                                            else if (aspectratio < (decimal)(1.9)) ar = "16:9";
                                            else if (aspectratio >= (decimal)(1.9)) ar = "cinemascope";
                                        wstring = aspectratio.ToString();
                                    }
                                catch
                                    {
                                        LogMyFilms.Info("MF: Error calculating aspectratio !");
                                    }
                            setGUIProperty("db.calc.aspectratio.value", wstring);
                            setGUIProperty("db.calc.imageformat.value", ar);
                            break;

                        case "checked":
                            if ((wrep) && (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                            {
                              setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                              if (MyFilms.conf.GlobalUnwatchedOnlyValue == "false" && MyFilms.r[ItemId][dc.ColumnName].ToString() == "true") 
                                setGUIProperty("watched", "true");
                              else 
                                setGUIProperty("watched", "false");
                            }
                            else
                              {
                                clearGUIProperty("db." + dc.ColumnName.ToLower() + ".value");
                                clearGUIProperty("watched");
                              }
                              
                            break;
                      
                        default:
                            if ((wrep) && (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                            else
                                clearGUIProperty("db." + dc.ColumnName.ToLower() + ".value");
                            break;

                    }
                }
            }
        }

        //-------------------------------------------------------------------------------------------
        //  Load detailed infos for persons: export fields to skin as '#myfilms.<ant db column name> 
        //-------------------------------------------------------------------------------------------
        public static void Load_Detailed_PersonInfo(string artistname, bool wrep)
        {
            //Todo: Add new properties for publishing actorinfos (MiniBio, Bio, Birthdate, Image, Multiimage, etc.

            clearGUIProperty("db.description.value");
            ArrayList actorList = new ArrayList();
            MyFilmsDetail.GetActorByName(artistname, actorList);
            if (actorList.Count == 0)
            {
                return;
            }
            int actorID;
            string actorname = "";
            // Define splitter for string
            char[] splitter = { '|' };
            // Iterate through list
            foreach (string act in actorList)
            {
                string[] strActor = act.Split(splitter);
                actorID = Convert.ToInt32(strActor[0]);
                actorname = strActor[1];
                if (actorID.ToString().Length > 0)
                {
                    IMDBActor actor = VideoDatabase.GetActorInfo(actorID);
                    if (actor.Biography.Length > 0) setGUIProperty("db.description.value", actor.Biography);
                }
            }
        }

        #endregion
        #region  Lecture du film demandé

        public static void Launch_Movie(int select_item, int GetID, GUIAnimation m_SearchAnimation)
        //-------------------------------------------------------------------------------------------
        // Play Movie
        //-------------------------------------------------------------------------------------------
        {
           
            // Guzzi: Added WOL to start remote host before playing the files
            // Wake up the TV server, if required
            // HandleWakeUpNas();
            LogMyFilms.Info("MF: Launched HandleWakeUpNas() to start movie'" + MyFilms.r[select_item][MyFilms.conf.StrSTitle.ToString()] + "'");

            if (MyFilms.conf.StrCheckWOLuserdialog)
            {
                int wTimeout = MyFilms.conf.StrWOLtimeout;
                bool isActive;
                string UNCpath = MyFilms.r[select_item][MyFilms.conf.StrStorage].ToString();
                WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();

                if (UNCpath.StartsWith("\\\\"))
                UNCpath = (UNCpath.Substring(2, UNCpath.Substring(2).IndexOf("\\") + 0)).ToLower();
                if ((UNCpath.Equals(MyFilms.conf.StrNasName1, StringComparison.InvariantCultureIgnoreCase)) || (UNCpath.Equals(MyFilms.conf.StrNasName2, StringComparison.InvariantCultureIgnoreCase)) || (UNCpath.Equals(MyFilms.conf.StrNasName3, StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (WakeOnLanManager.Ping(UNCpath, wTimeout))
                        isActive = true;
                    else
                        isActive = false;

                    if ((!isActive) || (true)) // Todo: DIsable "Always Show dialog"
                    {
                        GUIDialogYesNo dlgOknas =
                            (GUIDialogYesNo) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                        dlgOknas.SetHeading(GUILocalizeStrings.Get(107986)); //my films
                        dlgOknas.SetLine(1, "Movie   : '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle.ToString()] + "'"); //video title
                        dlgOknas.SetLine(2, "Storage: '" + UNCpath + "' - Status: '" + isActive.ToString() + "'");
                        //Filename/Storagepath
                        dlgOknas.SetLine(3, "Wollen Sie den NAS Server starten ?");
                        //dlgOknas.SetLine(3, "'" + MyFilms.conf.StrNasMAC1 + "', '" + MyFilms.conf.StrWOLtimeout.ToString() + "'");
                        dlgOknas.DoModal(GetID);
                        if (!(dlgOknas.IsConfirmed))
                            return;

                        // Start NAS Server
                        GUIDialogOK dlgOk =
                            (GUIDialogOK) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));
                        dlgOk.SetLine(1, "");

                        if ((UNCpath.Equals(MyFilms.conf.StrNasName1.ToLower())) && (MyFilms.conf.StrNasMAC1.ToString().Length > 1))
                        {
                            if (wakeOnLanManager.WakeupSystem(
                                wakeOnLanManager.GetHwAddrBytes(MyFilms.conf.StrNasMAC1.ToString()), MyFilms.conf.StrNasName1,
                                wTimeout))
                            {
                                dlgOk.SetLine(2, MyFilms.conf.StrNasName1 + " erfolgreich gestartet!");
                            }
                            else
                                dlgOk.SetLine(2, MyFilms.conf.StrNasName1 + " konnte nicht gestartet werden (Timeout)!");
                        }

                        if ((UNCpath.Equals(MyFilms.conf.StrNasName2.ToLower())) && (MyFilms.conf.StrNasMAC2.ToString().Length > 1))
                        {
                            if (wakeOnLanManager.WakeupSystem(wakeOnLanManager.GetHwAddrBytes(MyFilms.conf.StrNasMAC2), MyFilms.conf.StrNasName2, wTimeout))
                            {
                                dlgOk.SetLine(2, MyFilms.conf.StrNasName2 + " erfolgreich gestartet!");
                            }
                            else
                                dlgOk.SetLine(2, MyFilms.conf.StrNasName2 + " konnte nicht gestartet werden (Timeout)!");
                        }

                        if ((UNCpath.Equals(MyFilms.conf.StrNasName3.ToLower())) && (MyFilms.conf.StrNasMAC3.ToString().Length > 0))
                        {
                            if (wakeOnLanManager.WakeupSystem(
                                wakeOnLanManager.GetHwAddrBytes(MyFilms.conf.StrNasMAC3), MyFilms.conf.StrNasName3,
                                wTimeout))
                            {
                                dlgOk.SetLine(2, MyFilms.conf.StrNasName3 + " erfolgreich gestartet!");
                            }
                            else
                                dlgOk.SetLine(2, MyFilms.conf.StrNasName3 + " konnte nicht gestartet werden (Timeout)!");
                        }

                        dlgOk.DoModal(GetID);
                    }
                }
                else
                {
                    GUIDialogOK dlgOknas = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    dlgOknas.SetHeading(GUILocalizeStrings.Get(107986)); //my films
                    //dlgOknas.SetLine(1, "Movie   : '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle.ToString()].ToString() + "'"); //video title
                    dlgOknas.SetLine(2, "Storage: '" + UNCpath + "' ist offline nicht für WOL konfiguriert!"); //Filename/Storagepath
                    dlgOknas.SetLine(3, "Automatischer NAS Server Start nicht möglich...");
                    dlgOknas.DoModal(GetID);
                    return;
                }
            }

            // Run externaly Program before Playing if defined in setup
            LogMyFilms.Debug("MF: (Play Movie) select_item = '" + select_item + "' - GetID = '" + GetID + "' - m_SearchAnimation = '" + m_SearchAnimation + "'");
            setProcessAnimationStatus(true, m_SearchAnimation);
            if ((MyFilms.conf.CmdPar.Length > 0) && (MyFilms.conf.CmdPar != "(none)"))
                RunProgram(MyFilms.conf.CmdExe, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.CmdPar].ToString());
            if (g_Player.Playing)
                g_Player.Stop();

            // search all files
            ArrayList newItems = new ArrayList();
            bool NoResumeMovie = true;
            int IMovieIndex = 0;

            //Guzzi: Added BoolType for Trailerlaunch
			Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex, false);
            //Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex);
            if (newItems.Count > 20)
            // Maximum 20 entries (limitation for MP dialogFileStacking)
            {
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                dlgOk.SetLine(1, MyFilms.r[select_item][MyFilms.conf.StrSTitle.ToString()].ToString());//video title
                dlgOk.SetLine(2, "maximum 20 entries for the playlist");
                dlgOk.DoModal(GetID);
                LogMyFilms.Info("MF: Too many entries found for movie '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle.ToString()] + "', number of entries found = " + newItems.Count.ToString());
                return;
            }
            setProcessAnimationStatus(false, m_SearchAnimation);
            if (newItems.Count > 1)
            {
                if (NoResumeMovie)
                {
                    GUIDialogFileStacking dlg = (GUIDialogFileStacking)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_FILESTACKING);
                    if (null != dlg)
                    {
                        dlg.SetNumberOfFiles(newItems.Count);
                        dlg.DoModal(GUIWindowManager.ActiveWindow);
                        int selectedFileIndex = dlg.SelectedFile;
                        if (selectedFileIndex < 1) return;
                        IMovieIndex = selectedFileIndex++;
                    }
                }
            }
            if (newItems.Count > 0)
            {
                playlistPlayer.Reset();
                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_VIDEO_TEMP;
                PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP);
                playlist.Clear();

                foreach (object t in newItems)
                {
                    string movieFileName = (string)t;
                    PlayListItem newitem = new PlayListItem();
                    newitem.FileName = movieFileName;
                    newitem.Type = PlayListItem.PlayListItemType.Video;
                    playlist.Add(newitem);
                }
                // ask for start movie Index

                // play movie...
                PlayMovieFromPlayList(NoResumeMovie, IMovieIndex - 1);
            }
            else
            {
                //if (first)
                //// ask for mounting file first time
                //{
                GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                dlgYesNo.SetLine(1, GUILocalizeStrings.Get(219));//no disc
                if (MyFilms.conf.StrIdentItem != null && MyFilms.conf.StrIdentItem != "(none)" && MyFilms.conf.StrIdentItem != String.Empty)
                    if (MyFilms.conf.StrIdentLabel.Length > 0)
                        dlgYesNo.SetLine(2, MyFilms.conf.StrIdentLabel + " = " + MyFilms.r[select_item][MyFilms.conf.StrIdentItem]);//Label Identification for Media
                    else
                        dlgYesNo.SetLine(2, "'" + MyFilms.conf.StrIdentItem + "' = " + MyFilms.r[select_item][MyFilms.conf.StrIdentItem]);//ANT Item Identification for Media
                else
                    dlgYesNo.SetLine(2, "' disc n° = " + MyFilms.r[select_item]["Number"]);//ANT Number for Identification Media 
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)
                    Launch_Movie(select_item, GetID, m_SearchAnimation);
                //}
                else
                {
                    GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                    dlgOk.SetLine(1, GUILocalizeStrings.Get(1036));//no video found
                    dlgOk.SetLine(2, MyFilms.r[select_item][MyFilms.conf.StrSTitle].ToString());
                    dlgOk.DoModal(GetID);
                    LogMyFilms.Info("MF: File not found for movie '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle]);
                    return;
                }
            }
        }

        public static void Launch_Movie_Trailer(int select_item, int GetID, GUIAnimation m_SearchAnimation)
        //-------------------------------------------------------------------------------------------
        // Play Movie Trailers !!!
        //-------------------------------------------------------------------------------------------
        {
            // Run externaly Program before Playing if defined in setup
            setProcessAnimationStatus(true, m_SearchAnimation);
            LogMyFilms.Debug("MF: (Play Movie Trailer) select_item = '" + select_item + "' - GetID = '" + GetID + "' - m_SearchAnimation = '" + m_SearchAnimation + "'");
            if ((MyFilms.conf.CmdPar.Length > 0) && (MyFilms.conf.CmdPar != "(none)"))
                RunProgram(MyFilms.conf.CmdExe, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.CmdPar].ToString());
            if (g_Player.Playing)
                g_Player.Stop();
            // search all files
            ArrayList newItems = new ArrayList();
            bool NoResumeMovie = true; //Modded by Guzzi for NonResuming Trailers 
            int IMovieIndex = 0;

            LogMyFilms.Debug("MyFilmsDetails (Launch_Movie_Trailer): new do Moviesearch with '" + select_item + "' (Selected_Item"); 
            //Change back, if method in original properly adapted with bool Trailer
            Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex, true);
            LogMyFilms.Debug("MyFilmsDetails (Launch_Movie_Trailer): newItems.Count: '" + newItems.Count + "'"); 
            if (newItems.Count > 20)
            // Maximum 20 entries (limitation for MP dialogFileStacking)
            {
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                dlgOk.SetLine(1, MyFilms.r[select_item][MyFilms.conf.StrSTitle].ToString());//video title
                dlgOk.SetLine(2, "maximum 20 entries for the playlist");
                dlgOk.DoModal(GetID);
                LogMyFilms.Info("MF: Too many entries found for movie '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle] + "', number of entries found = " + newItems.Count.ToString());
                return;
            }
            setProcessAnimationStatus(false, m_SearchAnimation);
            if (newItems.Count > 1)
            {
                if ((NoResumeMovie == true)) //Modded by Guzzi to always get Selectiondialog for Trailer Choice
                {
                    GUIDialogFileStacking dlg = (GUIDialogFileStacking)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_FILESTACKING);
                    if (null != dlg)
                    {
                        dlg.SetNumberOfFiles(newItems.Count);
                        dlg.DoModal(GUIWindowManager.ActiveWindow);
                        int selectedFileIndex = dlg.SelectedFile;
                        if (selectedFileIndex < 1) return;
                        IMovieIndex = selectedFileIndex++;
                    }
                }
            }
            if (newItems.Count > 0)
            {
                playlistPlayer.Reset();
                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_VIDEO_TEMP;
                PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP);
                playlist.Clear();

                foreach (object t in newItems)
                {
                    string movieFileName = (string)t;
                    PlayListItem newitem = new PlayListItem();
                    newitem.FileName = movieFileName;
                    newitem.Type = PlayListItem.PlayListItemType.Video;
                    playlist.Add(newitem);
                }
                // ask for start movie Index
                
                // play movie...
                PlayMovieFromPlayListTrailer(NoResumeMovie, IMovieIndex - 1);
            }
            else
            {
              LogMyFilms.Info("MF: File not found for movie '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle] + "'");
              GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
              dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986) + " " + MyFilms.r[select_item][MyFilms.conf.StrSTitle].ToString());//my films & Titel
              dlgYesNo.SetLine(1, GUILocalizeStrings.Get(10798737));//no video found locally
              dlgYesNo.SetLine(2, GUILocalizeStrings.Get(10798738)); // Try Youtube?
              dlgYesNo.DoModal(GetID);
              //dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
              if (dlgYesNo.IsConfirmed)
              {


                GUIDialogMenu dlgmenu =
                  (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                System.Collections.Generic.List<string> choiceViewMenu = new System.Collections.Generic.List<string>();
                dlgmenu.Reset();
                choiceViewMenu.Clear();
                dlgmenu.SetHeading(GUILocalizeStrings.Get(10798704)); // Trailer ...

                dlgmenu.Add(GUILocalizeStrings.Get(10798711)); //search youtube trailer with onlinevideos
                choiceViewMenu.Add("playtraileronlinevideos");

                dlgmenu.Add(GUILocalizeStrings.Get(10798712)); //search apple itunes trailer with onlinevideos
                choiceViewMenu.Add("playtraileronlinevideosappleitunes");

                dlgmenu.Add(GUILocalizeStrings.Get(712) + " ..."); //Return ...
                choiceViewMenu.Add("return");

                dlgmenu.DoModal(GetID);
                if (dlgmenu.SelectedLabel == -1) return;
                switch (choiceViewMenu[dlgmenu.SelectedLabel].ToLower())
                {
                  case "playtraileronlinevideos":
                    // Load OnlineVideo Plugin with Searchparameters for YouTube and movie to Search ...
                    // OV reference for parameters: site:<sitename>|category:<categoryname>|search:<searchstring>|VKonfail:<true,false>|return:<Locked,Root>
                    // Check for Plugin and correct version - Version information for an assembly consists of the following four values: Major Version, Minor Version, Build Number, Revision
                    var hasRightPlugin =
                      PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "OnlineVideos");
                    var hasRightVersion =
                      PluginManager.SetupForms.Cast<ISetupForm>().Any(
                        plugin =>
                        plugin.PluginName() == "OnlineVideos" && plugin.GetType().Assembly.GetName().Version.Minor > 27);
                    //if (PluginManager.IsPluginNameEnabled2("OnlineVideos"))
                    if (hasRightPlugin && hasRightVersion)
                    {
                      string OVstartparams = string.Empty;
                      string OVtitle = string.Empty;
                      if (MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] != null &&
                          MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0) OVtitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                      if (OVtitle.IndexOf(MyFilms.conf.TitleDelim) > 0) OVtitle = OVtitle.Substring(OVtitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
                      OVstartparams = "site:Youtube|category:|search:" + OVtitle + " " +
                                      (MyFilms.r[MyFilms.conf.StrIndex]["Year"] + " trailer|return:Locked");
                      //GUIPropertyManager.SetProperty("Onlinevideos.startparams", OVstartparams);
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "YouTube");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", OVtitle.ToString());
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "Locked");

                      LogMyFilms.Debug("MF: Starting OnlineVideos with '" + OVstartparams.ToString() + "'");
                      GUIWindowManager.ActivateWindow(4755, false); // 4755 is ID for OnlineVideos
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");
                    }
                    else
                    {
                      //MyFilmsDetail.ShowMessageDialog("MyFilms", "OnlineVideo plugin not installed or wrong version", "Minimum Version resuired: 0.28");
                    }
                    break;

                  case "playtraileronlinevideosappleitunes":
                    var hasRightPlugin2 =
                      PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "OnlineVideos");
                    var hasRightVersion2 =
                      PluginManager.SetupForms.Cast<ISetupForm>().Any(
                        plugin =>
                        plugin.PluginName() == "OnlineVideos" && plugin.GetType().Assembly.GetName().Version.Minor > 27);
                    //if (PluginManager.IsPluginNameEnabled2("OnlineVideos"))
                    if (hasRightPlugin2 && hasRightVersion2)
                    {
                      string OVstartparams = string.Empty;
                      string OVtitle = string.Empty;
                      if (MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] != null &&
                          MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0) OVtitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                      if (OVtitle.IndexOf(MyFilms.conf.TitleDelim) > 0) OVtitle = OVtitle.Substring(OVtitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
                      OVstartparams = "site:iTunes Movie Trailers|category:|search:" + OVtitle + " " +
                                      (MyFilms.r[MyFilms.conf.StrIndex]["Year"] + " trailer|return:Locked");
                      //GUIPropertyManager.SetProperty("Onlinevideos.startparams", OVstartparams);
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "iTunes Movie Trailers");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", OVtitle.ToString());
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "Locked");

                      LogMyFilms.Debug("MF: Starting OnlineVideos with '" + OVstartparams.ToString() + "'");
                      GUIWindowManager.ActivateWindow(4755, false); // 4755 is ID for OnlineVideos
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
                      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");
                    }
                    else
                    {
                      //ShowMessageDialog("MyFilms", "OnlineVideo plugin not installed or wrong version", "Minimum Version resuired: 0.28");
                    }
                    break;

                  default:
                    return;

                }
              }

            }
        }

        protected static void Search_parts(string fileName, int select_item, bool delete, ref bool NoResumeMovie, ref ArrayList newItems, ref int IdMovie, ref int IMovieIndex, ref int timeMovieStopped)
        {
            // if filename already in arraylist, return
            // search other parts belonging to the same movie
            DirectoryInfo dirsInf = new DirectoryInfo(fileName.Substring(0, fileName.LastIndexOf("\\")));
            FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();
            foreach (FileSystemInfo sfi in sfiles)
            {
                string wfile = dirsInf.FullName + "\\" + sfi.Name;
                if (newItems.Contains(wfile))
                    continue;
                if ((MediaPortal.Util.Utils.ShouldStack(fileName, wfile)) && (MediaPortal.Util.Utils.IsVideo(wfile)))
                {
                    if (delete)
                    {
                        delete_movie(wfile, ref newItems);
                        continue;
                    }
                    else
                    {
                        IdMovie = -1;
                        add_update_movie(wfile, select_item, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                        continue;
                    }
                }
            }
        }



        protected static void delete_movie(string fileName, ref ArrayList newItems)
        {
            VideoDatabase.DeleteMovie(fileName); // Remove file from videodatabase, if any.
            newItems.Add(fileName);
        }
        protected static void add_update_movie(string fileName, int select_item, ref bool NoResumeMovie, ref ArrayList newItems, ref int IdMovie, ref int IMovieIndex, ref int timeMovieStopped)
        {
            // if filename already in arraylist, return
            if (newItems.Contains(fileName))
                return;
            // search infos in the Video Database
            IMDBMovie movieDetails = new IMDBMovie();
            VideoDatabase.GetMovieInfo(fileName, ref movieDetails);
            if (IdMovie == -1)
                IdMovie = VideoDatabase.GetMovieId(fileName);
            int IdFile = VideoDatabase.GetFileId(fileName);
            byte[] resumeData = null;
            if ((IdMovie >= 0) && (IdFile >= 0))
            {
                //  movie database find=> ask for resume movie if any 
                timeMovieStopped = VideoDatabase.GetMovieStopTimeAndResumeData(IdFile, out resumeData);
                if (timeMovieStopped > 0)
                {
                    string title = System.IO.Path.GetFileName(fileName);
                    if (movieDetails.Title != String.Empty) title = movieDetails.Title;

                    if (NoResumeMovie)
                    {
                        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                        if (null == dlgYesNo) return;
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(900)); //resume movie?
                        dlgYesNo.SetLine(1, title);
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(936) + MediaPortal.Util.Utils.SecondsToHMSString(timeMovieStopped));
                        dlgYesNo.SetLine(3, fileName);
                        dlgYesNo.SetDefaultToYes(true);
                        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                        if (!dlgYesNo.IsConfirmed)
                        {
                            VideoDatabase.DeleteMovieStopTime(IdFile);
                            timeMovieStopped = 0;
                        }
                        else
                        {
                            IMovieIndex = newItems.Count + 1;
                            NoResumeMovie = false;
                        }
                    }
                }
            }
            // update the MP Video Database for OSD view during playing // ToDo: Check proper fit for changes in Mediaportal 1.2 by Deda !!!
            update_database(fileName, select_item, -1);
            newItems.Add(fileName);
        }
        private void OnPlayBackStarted(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            if (type != g_Player.MediaType.Video) return;
            // store informations for action at endplayback if any
            MyFilms.conf.StrPlayedIndex = MyFilms.conf.StrIndex;
            MyFilms.conf.StrPlayedDfltSelect = MyFilms.conf.StrDfltSelect;
            MyFilms.conf.StrPlayedSelect = MyFilms.conf.StrFilmSelect;
            MyFilms.conf.StrPlayedSort = MyFilms.conf.StrSorta;
            MyFilms.conf.StrPlayedSens = MyFilms.conf.StrSortSens;
            int idFile = VideoDatabase.GetFileId(filename);
            if (idFile != -1)
            {
                int movieDuration = (int)g_Player.Duration;
                VideoDatabase.SetMovieDuration(idFile, movieDuration);
            }
        }
        private void OnPlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            UpdateOnPlayEnd(type, 0, filename, true);
        }
        private void OnPlayBackStopped(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename)
        {
            UpdateOnPlayEnd(type, timeMovieStopped, filename, false);
        }
        private void UpdateOnPlayEnd(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename, bool ended)
        {

            if (MyFilms.conf.StrPlayedIndex == -1)
                return;
            if (type != g_Player.MediaType.Video || filename.EndsWith("&txe=.wmv"))
                return;
            try
            {
                DataRow[] r1 = BaseMesFilms.LectureDonnées(MyFilms.conf.StrPlayedDfltSelect, MyFilms.conf.StrPlayedSelect, MyFilms.conf.StrPlayedSort, MyFilms.conf.StrPlayedSens);
                // Handle all movie files from idMovie
                ArrayList movies = new ArrayList();
                int iidMovie = VideoDatabase.GetMovieId(filename);
                if (iidMovie >= 0)
                {
                    VideoDatabase.GetFiles(iidMovie, ref movies);
                    if (movies.Count <= 0)
                        return;
                    foreach (object t in movies)
                    {
                        string strFilePath = (string)t;
                        int idFile = VideoDatabase.GetFileId(strFilePath);
                        byte[] resumeData = null;
                        if (idFile < 0)
                            break;
                        if (ended)
                        {
                            // Set resumedata to zero
                            VideoDatabase.GetMovieStopTimeAndResumeData(idFile, out resumeData);
                            VideoDatabase.SetMovieStopTimeAndResumeData(idFile, 0, resumeData);
                            LogMyFilms.Info("MF: GUIVideoFiles: OnPlayBackEnded idFile={0} resumeData={1}", idFile, resumeData);
                        }
                        else
                        {
                            if ((filename == strFilePath) && (timeMovieStopped > 0))
                            {
                                g_Player.Player.GetResumeState(out resumeData);
                                LogMyFilms.Info("MF: GUIVideoFiles: OnPlayBackStopped idFile={0} timeMovieStopped={1} resumeData={2}", idFile, timeMovieStopped, resumeData);
                                VideoDatabase.SetMovieStopTimeAndResumeData(idFile, timeMovieStopped, resumeData);
                                LogMyFilms.Debug("MF: GUIVideoFiles: OnPlayBackStopped store resume time");
                            }
                            else
                                VideoDatabase.DeleteMovieStopTime(idFile);
                        }
                    }
                    if (ended)
                    {
                        IMDBMovie details = new IMDBMovie();
                        VideoDatabase.GetMovieInfoById(iidMovie, ref details);
                        details.Watched++;
                        VideoDatabase.SetWatched(details);
                    }
                }
                if (MyFilms.conf.CheckWatched)
                    r1[MyFilms.conf.StrPlayedIndex]["Checked"] = "True";
                if (ended)
                {
                    if (MyFilms.conf.StrSupPlayer)
                        Suppress_Entry(r1, MyFilms.conf.StrPlayedIndex);
                }
                if ((MyFilms.conf.CheckWatched) || (MyFilms.conf.StrSupPlayer))
                    Update_XML_database();
                MyFilms.conf.StrPlayedIndex = -1;
                MyFilms.conf.StrPlayedDfltSelect = string.Empty;
                MyFilms.conf.StrPlayedSelect = string.Empty;
                MyFilms.conf.StrPlayedSort = string.Empty;
                MyFilms.conf.StrPlayedSens = string.Empty;
            }
            catch
            {
                LogMyFilms.Info("MF: Error during PlayBackEnded ");
            }
        }

        private static string LoadPlaylist(string filename)
        {
            PlayList playlist = new PlayList();
            IPlayListIO loader = PlayListFactory.CreateIO(filename);

            if (!loader.Load(playlist, filename))
            {
                LogMyFilms.Info("MF: Playlist not found for movie : '" + filename.ToString() + "'");
                return "";
            }
            filename = "";
            foreach (PlayListItem playListItem in playlist)
            {
                if (filename.Length == 0)
                    filename = playListItem.FileName.ToString();
                else
                    filename = filename + ";" + playListItem.FileName.ToString();
            }
            return filename;
        }

        private static int update_database(string filename, int select_item, int idMovie)
        {
            IMDBMovie movieDetails = new IMDBMovie();
            if (idMovie <= 0)
            {
                if (MyFilms.r[select_item]["Subtitles"].ToString().Length > 0)
                    idMovie = VideoDatabase.AddMovie(filename, true);
                else
                    idMovie = VideoDatabase.AddMovie(filename, false);
                string wdescription = System.Web.HttpUtility.HtmlEncode(MyFilms.r[select_item]["Description"].ToString().Replace('’', '\''));
                wdescription = wdescription.Replace('|', '\n');
                movieDetails.PlotOutline = movieDetails.Plot = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wdescription));

                movieDetails.Title = MyFilms.r[select_item][MyFilms.conf.StrTitle1].ToString();
                try
                { movieDetails.RunTime = Int32.Parse(MyFilms.r[select_item]["Length"].ToString()); }
                catch
                { movieDetails.RunTime = 0; }
                try
                { movieDetails.Rating = (float)Double.Parse(MyFilms.r[select_item]["Rating"].ToString()); }
                catch
                { movieDetails.Rating = 0; }
                try
                { movieDetails.Year = Int32.Parse(MyFilms.r[select_item]["Year"].ToString()); }
                catch
                { movieDetails.Year = 0; }


                // Modified to match changes by Deda in MyVideos (New Thumbformat)

                string strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, movieDetails.Title);
                string LargeThumb = MediaPortal.Util.Utils.GetLargeCoverArtName(Thumbs.MovieTitle, movieDetails.Title);
                //string strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, movieDetails.Title) + "{" + idMovie.ToString() + "}";;
                //string LargeThumb = MediaPortal.Util.Utils.GetLargeCoverArtName(Thumbs.MovieTitle, movieDetails.Title) + "{" + idMovie.ToString() + "}";;
                //LogMyFilms.Debug("MF: (ThumbCreation): strThumb: '" + strThumb.ToString() + "'");
                //LogMyFilms.Debug("MF: (ThumbCreation): LargeThumb: '" + LargeThumb.ToString() + "'");

                try
                {
                    string wImage;
                    if ((MyFilms.r[select_item]["Picture"].ToString().IndexOf(":\\") == -1) && (MyFilms.r[select_item]["Picture"].ToString().Substring(0, 2) != "\\\\"))
                        wImage = MyFilms.conf.StrPathImg + "\\" + MyFilms.r[select_item]["Picture"];
                    else
                        wImage = MyFilms.r[select_item]["Picture"].ToString();
                    if (!System.IO.File.Exists(wImage))
                        wImage = Config.Dir.Skin + "\\media\\Films_No_Cover.png";
                    if (System.IO.File.Exists(wImage))
                    {
                        if (!System.IO.File.Exists(strThumb))
                            Picture.CreateThumbnail(wImage, strThumb, (int)Thumbs.ThumbResolution, (int)Thumbs.ThumbResolution, 0, Thumbs.SpeedThumbsSmall);
                        if (!System.IO.File.Exists(LargeThumb))
                            Picture.CreateThumbnail(wImage, LargeThumb, (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0, Thumbs.SpeedThumbsLarge);
                        // CreateThumbnail Version 0.2.3 + SVN
                        //if (Picture.CreateThumbnail(wImage, strThumb, (int)Thumbs.ThumbResolution, (int)Thumbs.ThumbResolution, 0, Thumbs.SpeedThumbsSmall))
                        //Picture.CreateThumbnail(wImage, LargeThumb, (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0, Thumbs.SpeedThumbsLarge);
                        //// CreateThumbnail Version 0.2.3 Stable
                        //if (Picture.CreateThumbnail(wImage, strThumb, (int)Thumbs.ThumbResolution, (int)Thumbs.ThumbResolution, 0))
                        //    Picture.CreateThumbnail(wImage, LargeThumb, (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0);

                    }
                }
                catch
                {
                }
                movieDetails.Director = MyFilms.r[select_item]["Director"].ToString();
                wzone = null;
                Analyze(MyFilms.r[select_item]["Category"].ToString(), idMovie, "Genre");
                movieDetails.Genre = wzone;
                wzone = null;
                Analyze(MyFilms.r[select_item]["Actors"].ToString(), idMovie, "Actor");
                movieDetails.Cast = wzone;
                VideoDatabase.SetMovieInfoById(idMovie, ref movieDetails);
            }
            else
            {
                int pathId, widMovie;
                widMovie = VideoDatabase.GetFile(filename, out pathId, out widMovie, true);
                if (widMovie != idMovie)
                {
                    string strPath, strFileName;

                    MediaPortal.Database.DatabaseUtility.Split(filename, out strPath, out strFileName);
                    MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(ref strPath);
                    MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(ref strFileName);
                    pathId = VideoDatabase.GetPath(strPath);
                    if (pathId == -1)
                        pathId = VideoDatabase.AddPath(strPath);
                    else
                        pathId = VideoDatabase.GetPath(strPath);
                    VideoDatabase.AddFile(idMovie, pathId, strFileName);

                }
            }
            return idMovie;
        }

        private static void Analyze(string champselect, int iidmovie, string field)
        {

            ArrayList w_tableau = new ArrayList();
            w_tableau = MyFilms.Search_String(champselect);
            foreach (object t in w_tableau)
            {
                update_fieldDB(t.ToString(), iidmovie, field);
            }
        }

        private static void Search_All_Files(int select_item, bool delete, ref bool NoResumeMovie, ref ArrayList newItems, ref int IMovieIndex, bool Trailer)
        {
            string fileName = string.Empty;
            string[] split1;
            string[] split2;
            string[] split3;
            string strDir = MyFilms.conf.StrDirStor;
            int IdMovie = -1;
            int timeMovieStopped = 0;
            if (Trailer)
            {
              if (MyFilms.conf.StrDirStorTrailer.Length > 0)
                strDir = MyFilms.conf.StrDirStorTrailer;
              else
              {
                strDir = "";
              }
            }
              
            else strDir = MyFilms.conf.StrDirStor;

            LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - StrDirStor: " + MyFilms.conf.StrDirStor);
            LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - StrDirStortrailer: " + MyFilms.conf.StrDirStorTrailer);
            LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - Modus 'Trailer' = '" + Trailer + "' - strDir: '" + strDir + "'");
            // retrieve filename information stored in the DB
            if (Trailer)
              if (MyFilms.conf.StrDirStorTrailer.Length > 0)
                LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - try filename MyFilms.r[select_item][MyFilms.conf.StrStorageTrailer]: '" + MyFilms.r[select_item][MyFilms.conf.StrStorageTrailer].ToString().Trim() + "' - ConfStorageTrailer: '" + MyFilms.conf.StrStorageTrailer + "'");
              else 
                LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - Trailersearchpath not set in config!");
            else
              LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - try filename MyFilms.r[select_item][MyFilms.conf.StrStorage]: '" + MyFilms.r[select_item][MyFilms.conf.StrStorage].ToString().Trim() + "' - ConfStorage: '" + MyFilms.conf.StrStorage + "'");
            if (Trailer)
            {
                try
                { fileName = MyFilms.r[select_item][MyFilms.conf.StrStorageTrailer].ToString().Trim(); }
                catch
                { fileName = string.Empty; }
            }
            else
              if (MyFilms.conf.StrStorage != null && MyFilms.conf.StrStorage != "(none)" && MyFilms.conf.StrStorage != string.Empty)
                {
                    try
                    { fileName = MyFilms.r[select_item][MyFilms.conf.StrStorage].ToString().Trim(); }
                    catch
                    { fileName = string.Empty; }
                }
            if (fileName.Length == 0 && !Trailer)
            {
                // search filename by Title movie
                if ((MyFilms.conf.SearchFile == "True") || (MyFilms.conf.SearchFile == "yes"))
                {
                    string movieName = MyFilms.r[select_item][MyFilms.conf.ItemSearchFile].ToString();
                    movieName = movieName.Substring(movieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
                    if (MyFilms.conf.ItemSearchFile.Length > 0)
                        fileName = Search_FileName(movieName, MyFilms.conf.StrDirStor).Trim();
                    if ((fileName.Length > 0) && (!(MyFilms.conf.StrStorage.Length == 0) && MyFilms.conf.StrStorage != "(none)"))
                    {
                        MyFilms.r[select_item][MyFilms.conf.StrStorage] = fileName;
                        Update_XML_database();
                    }
                }
            }
            bool wisomounted = false;
            string wisofile = string.Empty;
            // split filename information delimited by semicolumn (multifile detection)
            split1 = fileName.Split(new Char[] { ';' });
            ArrayList movies = new ArrayList();
            IMDBMovie movieDetails = new IMDBMovie();
            Regex DVDRegexp = new Regex("video_ts");
            foreach (string wfile in split1)
            {
                if (wfile.IndexOf("/") == -1)
                    fileName = wfile.Trim();
                else
                    fileName = wfile.Substring(0, wfile.IndexOf("/")).Trim();
                if (fileName.Length > 0)
                {
                    // first verify if file exists and if it's a video file
                    if ((System.IO.File.Exists(fileName)) && (MediaPortal.Util.Utils.IsVideo(fileName)) && (!VirtualDirectory.IsImageFile(System.IO.Path.GetExtension(fileName))))
                    {
                        if (delete)
                        {
                            delete_movie(fileName, ref newItems);
                            Search_parts(fileName, select_item, delete, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                            continue;
                        }
                        else
                        {
                            IdMovie = -1;
                            add_update_movie(fileName, select_item, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                            Search_parts(fileName, select_item, delete, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                            continue;
                        }
                    }

                    if (!System.IO.File.Exists(fileName) && !System.IO.Directory.Exists(fileName))
                    {
                        fileName = SearchMovie(fileName, strDir);
                        if (fileName == "false")
                            break;
                    }
                    // for image disk file filenames can be designed after iso filename delimited by '/'    
                    split2 = wfile.ToString().Split(new Char[] { '/' });
                    int wfin = split2.Length;
                    // detect if first part is an iso file
                    if (VirtualDirectory.IsImageFile(System.IO.Path.GetExtension(split2[0].Trim())))
                    {
                        string wtomount = split2[0].Trim();
                        if (!System.IO.File.Exists(wtomount))
                            wtomount = SearchMovie(wtomount, strDir);
                        if (wtomount == "false")
                        {
                            // image file not found
                            break;
                        }
                        // only one iso file can be mounted
                        if ((!wisomounted) && ((wisofile == wtomount) || (wisofile == string.Empty)))
                        {
                            if (MountImageFile(wGetID, wtomount))
                            {
                                wisomounted = true;
                                wisofile = wtomount;
                                string wdir = DaemonTools.GetVirtualDrive();
                                if (wdir.LastIndexOf("\\") != wdir.Length - 1)
                                    wdir = wdir.Trim() + "\\";
                                fileName = string.Empty;
                                for (int wi = 1; wi == split2.Length - 1; wi++)
                                {
                                    if (fileName.Length > 0)
                                        fileName = fileName + ";" + wdir + split2[wi].Trim();
                                    else
                                        fileName = wdir + split2[wi].Trim();
                                }
                                if (fileName == string.Empty)
                                    fileName = wdir;
                            }
                        }
                    }
                    if (PlayListFactory.IsPlayList(fileName))
                        fileName = LoadPlaylist(wfile.Trim());
                    // complete path to file (exist '\\' or '[Drive letter]:') ?
                    if (fileName.Substring(0, 2) != "\\\\" && fileName.Substring(1, 1) != ":")
                    {
                        // search file into StrDir (no complete path)
                        // split StrDir information delimited by semicolumn (mulpath to search)
                        fileName = SearchMovie(fileName, strDir);
                        if (fileName == "false")
                        {
                            // file not found
                            break;
                        }
                    }
                    split3 = fileName.Split(new Char[] { ';' });
                    foreach (string wfile2 in split3)
                    {
                        string wfile3 = string.Empty;
                        if (!System.IO.File.Exists(wfile2.Trim()) && !System.IO.Directory.Exists(wfile2.Trim()))
                        {
                            wfile3 = SearchMovie(wfile2.Trim(), strDir);
                            if (wfile3 == "false")
                                break;
                        }
                        else
                            wfile3 = wfile2.Trim();
                        if ((System.IO.File.Exists(wfile3)) && (MediaPortal.Util.Utils.IsVideo(wfile3)))
                        {
                            if (delete)
                            {
                                delete_movie(wfile3, ref newItems);
                                Search_parts(wfile3, select_item, delete, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                                continue;
                            }
                            else
                            {
                                IdMovie = -1;
                                add_update_movie(wfile3, select_item, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                                Search_parts(wfile3, select_item, delete, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                                continue;
                            }
                        }
                        if (System.IO.File.Exists(wfile3 + @"\VIDEO_TS\VIDEO_TS.IFO"))
                        {
                            if (delete)
                            {
                                delete_movie(wfile3 + @"\VIDEO_TS\VIDEO_TS.IFO", ref newItems);
                                Search_parts(wfile3, select_item, delete, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                                continue;
                            }
                            else
                            {
                                IdMovie = -1;
                                add_update_movie(wfile3 + @"\VIDEO_TS\VIDEO_TS.IFO", select_item, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                                Search_parts(fileName, select_item, delete, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                                continue;
                            }
                        }

                        if (System.IO.Directory.Exists(wfile3))
                        {
                            // it's a Directory so all files included are added to playlist
                            DirectoryInfo dirsInf = new DirectoryInfo(wfile3);
                            //On retourne une liste d'informations sur les fichiers contenus dans le répertoire
                            FileSystemInfo[] filesInf = dirsInf.GetFileSystemInfos();
                            foreach (FileSystemInfo fi in filesInf)
                            {
                                if (MediaPortal.Util.Utils.IsVideo(fi.FullName))
                                {
                                    if (delete)
                                    {
                                        delete_movie(fi.FullName, ref newItems);
                                        Search_parts(fi.FullName, select_item, delete, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                                        continue;
                                    }
                                    else
                                    {
                                        IdMovie = -1;
                                        add_update_movie(fi.FullName, select_item, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                                        Search_parts(fi.FullName, select_item, delete, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private static string Search_FileName(string filename, string storage)
        {
            result = new ArrayList();
            string file = filename;

            SearchFiles(file, storage, false, false);

            //si des resultats existent on les affichent
            if (result.Count != 0)
            {
                if (result.Count == 1)
                    return result[0].ToString();
                string wfile = null;
                result.Sort();
                ArrayList wresult = new ArrayList();
                foreach (String s in result)
                {
                    if (!MediaPortal.Util.Utils.ShouldStack(s, wfile) && s.ToLower() != wfile.ToLower())
                    {
                        wresult.Add(s);
                        wfile = s;
                    }
                }
                if (wresult.Count == 1)
                    return wresult[0].ToString();
                else
                {
                    // Many files found; ask for the good file
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg == null) return string.Empty;
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                    foreach (string Moviefile in wresult)
                        dlg.Add(Moviefile);
                    dlg.DoModal(wGetID);

                    if (dlg.SelectedLabel == -1)
                        return "";
                    return wresult[dlg.SelectedLabel].ToString();
                }
            }
            else
            // No Movie File Found
            {
                LogMyFilms.Info("MF: File not found for movie '" + filename.ToString() + "'; string search '" + wsearchfile.ToString() + "'");
                return string.Empty;
            }
        }
        /// <summary>
        /// Permet de rechercher un fichier dans un dossier et ces sous dossier
        /// </summary>
        /// <param name="fileName">le nom du fichier recherché</param>
        /// <param name="path">le chemin du répertoire dans lequel chercher</param>
        /// <param name="recur">spécifie s'il s'agit d'une relance recursive</param>
        public static void SearchFiles(string fileName, string searchrep, bool recur, bool Trailer)
        {
            System.Text.RegularExpressions.Regex oRegex;
            oRegex = new System.Text.RegularExpressions.Regex(";");
            string file = fileName;
            if (!recur)
            {
                file = MediaPortal.Util.Utils.FilterFileName(fileName.ToLower());
                string[] drives = Environment.GetLogicalDrives();
                foreach (string drive in drives)
                {
                    if (MediaPortal.Util.Utils.getDriveType(drive) == 5) //cd or dvd drive
                    {
                        string driverLetter = String.Format(@"{0}:\", drive.Substring(0, 1));
                        if (searchrep.Length > 0)
                            searchrep = searchrep + ";" + driverLetter;
                        else
                            searchrep = driverLetter;
                    }
                }
                file = Replace_String(file);
                wsearchfile = file;
            }
            string[] SearchDir = oRegex.Split(searchrep);
            oRegex = new System.Text.RegularExpressions.Regex(file);
            string wpath;
            foreach (string path in SearchDir)
            {
                if (path.LastIndexOf(@"\") != path.Length - 1)
                    wpath = path + "\\";
                else
                    wpath = path;
                if (System.IO.Directory.Exists(wpath))
                {
                    DirectoryInfo dirsInf = new DirectoryInfo(wpath);
                    //On retourne une liste d'informations sur les fichiers contenus dans le répertoire
                    FileSystemInfo[] filesInf = dirsInf.GetFileSystemInfos();

                    //Si le nom d'un fichier correspond avec le nom du fichier recherché 
                    //on place son path dans la variable globale result.
                    foreach (FileSystemInfo fi in filesInf)
                    {
                        System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(fi.Name.ToLower());
                        foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
                        {
                            if (MediaPortal.Util.Utils.IsVideo(fi.Name) || VirtualDirectory.IsImageFile(System.IO.Path.GetExtension(fi.Name)))
                                result.Add(wpath + fi.Name);
                            else
                            {
                                if (fi.Attributes.ToString() == "Directory")
                                    if (System.IO.File.Exists(wpath.Trim() + fi.Name.Trim() + "\\VIDEO_TS\\VIDEO_TS.IFO"))
                                        result.Add(wpath.Trim() + fi.Name.Trim() + "\\VIDEO_TS\\VIDEO_TS.IFO");
                                    else
                                        if (MediaPortal.Util.Utils.IsVideo(fi.Name))
                                            result.Add(wpath.Trim() + fi.Name.Trim());
                            }
                        }
                    }
                    //Si le parametre SearchSubDirs vaut true on réalise une 
                    //recherche récursive sur tous les sous-répertoires
                    if (MyFilms.conf.SearchSubDirs == "no") continue;
                    foreach (DirectoryInfo dir in dirsInf.GetDirectories())
                    {
                        //On rappelle la méthode SearchFiles pour tous les sous-répertoires  
                        SearchFiles(file, dir.FullName, true, false);
                    }
                }
            }
        }
        private static void update_fieldDB(string champselect, int iidmovie, string field)
        {
            IMDBMovie movieDetails = new IMDBMovie();
            IMDBActor actor = new IMDBActor();

            if (field == "Genre")
            {
                int iiGenre = VideoDatabase.AddGenre(champselect.ToString());
                VideoDatabase.AddGenreToMovie(iidmovie, iiGenre);
                if (wzone == null)
                    wzone = champselect;
                else
                    wzone = wzone + "/" + champselect;
            }
            else
            {
                VideoDatabase.GetMovieInfoById(iidmovie, ref movieDetails);
                IMDBActor.IMDBActorMovie actorMovie = new IMDBActor.IMDBActorMovie();
                actorMovie.MovieTitle = movieDetails.Title;
                actorMovie.Year = movieDetails.Year;
                int iiActor = VideoDatabase.AddActor(champselect);
                VideoDatabase.AddActorToMovie(iidmovie, iiActor);
                VideoDatabase.AddActorInfoMovie(iiActor, actorMovie);
                if (wzone == null)
                    wzone = champselect;
                else
                    wzone = wzone + "\n" + champselect;
            }
        }
        static public string SearchMovie(string filename, string StrDirStor)
        {
            //check if divx is on Disk
            if (System.IO.File.Exists(filename) || System.IO.Directory.Exists(filename))
            {
                return filename;
            }
            else
            {
                string searchrep = StrDirStor;
                DriveInfo[] allDrives = DriveInfo.GetDrives();

                foreach (DriveInfo d in allDrives)
                {
                    if ((d.DriveType.ToString() == "CDRom") && d.IsReady)
                    {
                        if (searchrep.Length > 0)
                            searchrep = searchrep + ";" + d.Name;
                        else
                            searchrep = d.Name;
                    }
                }
                string file = filename.Substring(filename.LastIndexOf(@"\") + 1);
                System.Text.RegularExpressions.Regex oRegex;
                oRegex = new System.Text.RegularExpressions.Regex(";");
                string[] SearchDir = oRegex.Split(searchrep);
                string wpath;
                foreach (string path in SearchDir)
                {
                    if (!(path.LastIndexOf(@"\") == path.Length - 1))
                        wpath = path + "\\";
                    else
                        wpath = path;
                    if (System.IO.File.Exists(wpath + file) || System.IO.Directory.Exists(wpath + file))
                        return (wpath + file);
                    if ((MyFilms.conf.SearchSubDirs == "no") || (!System.IO.Directory.Exists(wpath))) continue;
                    foreach (string sFolderSub in Directory.GetDirectories(wpath, "*", SearchOption.AllDirectories))
                    {
                        if ((System.IO.File.Exists(sFolderSub + "\\" + file)) || (System.IO.Directory.Exists(sFolderSub + "\\" + file)))
                        {
                            return (sFolderSub + "\\" + file);
                        }
                    }
                }
                return "false";
            }
        }


        static public bool CheckMovie(int idMovie)
        {
            IMDBMovie movieDetails = new IMDBMovie();
            VideoDatabase.GetMovieInfoById(idMovie, ref movieDetails);

            if (!MediaPortal.Util.Utils.IsDVD(movieDetails.Path)) return true;
            string cdlabel = String.Empty;
            cdlabel = MediaPortal.Util.Utils.GetDriveSerial(movieDetails.Path);
            if (cdlabel.Equals(movieDetails.CDLabel)) return true;

            GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            if (dlg == null) return true;
            while (true)
            {
                dlg.SetHeading(GUILocalizeStrings.Get(428));
                dlg.SetLine(1, GUILocalizeStrings.Get(429));
                dlg.SetLine(2, movieDetails.DVDLabel);
                dlg.SetLine(3, movieDetails.Title);
                dlg.DoModal(GUIWindowManager.ActiveWindow);
                if (dlg.IsConfirmed)
                {
                    if (movieDetails.CDLabel.StartsWith("nolabel"))
                    {
                        ArrayList movies = new ArrayList();
                        VideoDatabase.GetFiles(idMovie, ref movies);
                        if (System.IO.File.Exists(/*movieDetails.Path+movieDetails.File*/(string)movies[0]))
                        {
                            cdlabel = MediaPortal.Util.Utils.GetDriveSerial(movieDetails.Path);
                            VideoDatabase.UpdateCDLabel(movieDetails, cdlabel);
                            movieDetails.CDLabel = cdlabel;
                            return true;
                        }
                    }
                    else
                    {
                        cdlabel = MediaPortal.Util.Utils.GetDriveSerial(movieDetails.Path);
                        if (cdlabel.Equals(movieDetails.CDLabel)) return true;
                    }
                }
                else break;
            }
            return false;
        }

        static public void PlayMovieFromPlayList(bool NoResumeMovie)
        {
            PlayMovieFromPlayList(NoResumeMovie, -1);
        }

        static public void PlayMovieFromPlayList(bool NoResumeMovie, int iMovieIndex)
        {
            string filename;
            if (iMovieIndex == -1)
                filename = playlistPlayer.GetNext();
            else
                filename = playlistPlayer.Get(iMovieIndex);
            IMDBMovie movieDetails = new IMDBMovie();
            VideoDatabase.GetMovieInfo(filename, ref movieDetails);
            int idFile = VideoDatabase.GetFileId(filename);
            int idMovie = VideoDatabase.GetMovieId(filename);
            int timeMovieStopped = 0;
            byte[] resumeData = null;
            if ((idMovie >= 0) && (idFile >= 0))
            {
                timeMovieStopped = VideoDatabase.GetMovieStopTimeAndResumeData(idFile, out resumeData);
                if (timeMovieStopped > 0)
                {
                    string title = System.IO.Path.GetFileName(filename);
                    VideoDatabase.GetMovieInfoById(idMovie, ref movieDetails);
                    if (movieDetails.Title != String.Empty) title = movieDetails.Title;

                    if (NoResumeMovie)
                    {
                        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                        if (null == dlgYesNo) return;
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(900)); //resume movie?
                        dlgYesNo.SetLine(1, title);
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(936) + MediaPortal.Util.Utils.SecondsToHMSString(timeMovieStopped));
                        dlgYesNo.SetDefaultToYes(true);
                        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                        if (!dlgYesNo.IsConfirmed)
                            timeMovieStopped = 0;
                    }
                }
            }
            if (iMovieIndex == -1)
                playlistPlayer.PlayNext();
            else
                playlistPlayer.Play(iMovieIndex);
            if (g_Player.Playing && timeMovieStopped > 0)
            {
                if (g_Player.IsDVD)
                    g_Player.Player.SetResumeState(resumeData);
                else
                {
                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SEEK_POSITION, 0, 0, 0, 0, 0, null);
                    msg.Param1 = (int)timeMovieStopped;
                    GUIGraphicsContext.SendMessage(msg);
                }
            }
        }

        static public void PlayMovieFromPlayListTrailer(bool NoResumeMovie, int iMovieIndex)
        {
            string filename;
            if (iMovieIndex == -1)
                filename = playlistPlayer.GetNext();
            else
                filename = playlistPlayer.Get(iMovieIndex);
            IMDBMovie movieDetails = new IMDBMovie();
            VideoDatabase.GetMovieInfo(filename, ref movieDetails);
            int idFile = VideoDatabase.GetFileId(filename);
            int idMovie = VideoDatabase.GetMovieId(filename);
            int timeMovieStopped = 0;
            byte[] resumeData = null;
            if ((idMovie >= 0) && (idFile >= 0))
            {
                timeMovieStopped = VideoDatabase.GetMovieStopTimeAndResumeData(idFile, out resumeData);
                //Todo: Avoid Resume of Trailers
                if ((timeMovieStopped > 0) && (NoResumeMovie)) //Modded by Guzzi to avoid resuming for Trailers
                {
                    string title = System.IO.Path.GetFileName(filename);
                    VideoDatabase.GetMovieInfoById(idMovie, ref movieDetails);
                    if (movieDetails.Title != String.Empty) title = movieDetails.Title;

                    if (NoResumeMovie)
                    {
                        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                        if (null == dlgYesNo) return;
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(900)); //resume movie?
                        dlgYesNo.SetLine(1, title);
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(936) + MediaPortal.Util.Utils.SecondsToHMSString(timeMovieStopped));
                        dlgYesNo.SetDefaultToYes(true);
                        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                        if (!dlgYesNo.IsConfirmed)
                            timeMovieStopped = 0;
                    }
                }
            }
            if (iMovieIndex == -1)
                playlistPlayer.PlayNext();
            else
                playlistPlayer.Play(iMovieIndex);
            if (g_Player.Playing && timeMovieStopped > 0)
            {
                if (g_Player.IsDVD)
                    g_Player.Player.SetResumeState(resumeData);
                else
                {
                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SEEK_POSITION, 0, 0, 0, 0, 0, null);
                    msg.Param1 = (int)timeMovieStopped;
                    GUIGraphicsContext.SendMessage(msg);
                }
            }
        }
        
        
        static public bool MountImageFile(int WindowID, string file)
        {
            XmlConfig XmlConfig = new XmlConfig();    
            m_askBeforePlayingDVDImage = XmlConfig.ReadXmlConfig("MyFilms", "daemon", "askbeforeplaying", false);
//            }
            if (!DaemonTools.IsMounted(file))
            {
                if (m_askBeforePlayingDVDImage)
                {
                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                    if (dlgYesNo != null)
                    {
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(713));
                        dlgYesNo.SetLine(1, GUILocalizeStrings.Get(531));
                        dlgYesNo.DoModal(WindowID);
                        if (!dlgYesNo.IsConfirmed) return false;
                    }
                }
                string vdrive = string.Empty;
                DaemonTools.Mount(file, out vdrive);
                if (vdrive == string.Empty && file != String.Empty) return false; // protected share, with wrong pincode
            }

            return DaemonTools.IsMounted(file);
        }
        public static void setProcessAnimationStatus(bool enable, GUIAnimation m_SearchAnimation)
        {
            try
            {
                if (m_SearchAnimation != null)
                {
                    if (enable)
                        m_SearchAnimation.AllocResources();
                    else
                        m_SearchAnimation.Dispose();
                }
                m_SearchAnimation.Visible = enable;

            }
            catch (Exception)
            {
            }
        }


        static public string Replace_String(string file)
        {
            System.Text.RegularExpressions.Regex oRegex;
            file = file.Replace("-", " ");
            file = file.Replace("!", " ");
            file = file.Replace("#", " ");
            file = file.Replace(";", " ");
            file = file.Replace(":", " ");
            file = file.Replace("'", " ");
            file = file.Replace("=", " ");
            file = file.Replace("&", " ");
            file = file.Replace("(", " ");
            file = file.Replace(")", " ");
            file = file.Replace("@", " ");
            file = file.Replace("%", " ");
            file = file.Replace("$", " ");
            file = file.Replace("_", " ");
            file = file.Replace(".", " ");
            file = file.Replace(",", " ");
            file = file.Trim();
            oRegex = new System.Text.RegularExpressions.Regex(" +");

            file = oRegex.Replace(file, ":");
            file = file.Replace(":", ".*");
            file = file.Replace("é", "[eé]");
            file = file.Replace("è", "[eè]");
            file = file.Replace("ê", "[eê]");
            file = file.Replace("ë", "[eë]");
            file = file.Replace("ô", "[oô]");
            file = file.Replace("ö", "[oö]");
            file = file.Replace("à", "[aà]");
            file = file.Replace("â", "[aâ]");
            file = file.Replace("ä", "[aä]");
            file = file.Replace("ù", "[uù]");
            file = file.Replace("û", "[uû]");
            file = file.Replace("ü", "[uü]");
            file = file.Replace("î", "[iî]");
            file = file.Replace("ï", "[iï]");
            file = file.Replace("ç", "[cç]");
            return file;
        }

        static public void RunProgram(string exeName, string argsLine)
        {
            if (exeName.Length > 0)
            {
                // Use ProcessStartInfo class
                // ProcessStartInfo psI = new ProcessStartInfo(exeName, argsLine);
                //ProcessStartInfo startInfo = new ProcessStartInfo();
                LogMyFilms.Debug("Launching process with filename = '" + exeName + "' and argument = '" + argsLine + "'");
                Process newProcess = new Process();

                try
                {
                    newProcess.StartInfo.FileName = exeName;
                    newProcess.StartInfo.Arguments = argsLine;
                    newProcess.StartInfo.UseShellExecute = true;
                    newProcess.StartInfo.CreateNoWindow = true;
                    newProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //newProcess.StartInfo.WorkingDirectory = "C:\\Programme\\Team MediaPortal\\MediaPortal";
                    if (OSInfo.OSInfo.VistaOrLater())
                    {
                        newProcess.StartInfo.Verb = "runas";
                    }
                    newProcess.Start();
                    while (!newProcess.HasExited)
                    {
                    }
                }

                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        static public void RunAMCupdater(string exeName, string argsLine)
        {
            if (exeName.Length > 0)
            {
                using (Process p = new Process())
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = exeName;
                    psi.UseShellExecute = true;
                    psi.WindowStyle = ProcessWindowStyle.Minimized;
                    psi.Arguments = argsLine;
                    psi.ErrorDialog = false;
                    if (OSInfo.OSInfo.VistaOrLater())
                    {
                        psi.Verb = "runas";
                    }

                    p.StartInfo = psi;
                    LogMyFilms.Debug("MF: RunAMCupdater - Starting external command: {0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);
                    try
                    {
                        p.Start();
                        p.WaitForExit();
                    }
                    catch (Exception e)
                    {
                        LogMyFilms.Debug(e.ToString());
                    }
                    LogMyFilms.Debug("MF: RunAMCupdater - External command finished");
                }
            }
        }


        //-------------------------------------------------------------------------------------------
        //  Search All Trailerfiles locally
        //-------------------------------------------------------------------------------------------        
        public static void SearchTrailerLocal(DataRow[] r1, int Index, bool ExtendedSearch)
        {
            //Searchdirectory:
            LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) - StrDirStortrailer: " + MyFilms.conf.StrDirStorTrailer);
            //Title1 = Movietitle
            LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1] : '" + MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString() + "'");
            //Title2 = Translated Movietitle
            LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2] : '" + MyFilms.r[Index][MyFilms.conf.StrTitle2].ToString() + "'");
            //Cleaned Title
            LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - Cleaned Title                                               : '" + MediaPortal.Util.Utils.FilterFileName(MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString().ToLower()) + "'");            
            //Index of facadeview?
            LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) - Index: '" + Index + "'");
            //Full Path to Film
            LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) - FullMediasource: '" + (string)MyFilms.r[Index][MyFilms.conf.StrStorage].ToString().Trim() + "'");

            result = new ArrayList();
            ArrayList resultsize = new ArrayList();
            string[] filesfound = new string[100];
            Int64[] filesfoundsize = new Int64[100];
            int filesfoundcounter = 0;
            string file = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
            string titlename = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
            string titlename2 = MyFilms.r[Index][MyFilms.conf.StrTitle2].ToString();
            //string file = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
            //string titlename = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
            //string titlename2 = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString();
            string directoryname = "";
            string movieName = "";
            Int64 wsize = 0; // Temporary Filesize detection
            // split searchpath information delimited by semicolumn (multiple searchpathes from config)
            string[] Trailerdirectories = MyFilms.conf.StrDirStorTrailer.ToString().Split(new Char[] { ';' });
            LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) Search for '" + file + "' in '" + MyFilms.conf.StrDirStorTrailer + "'");

            //Retrieve original directory of mediafiles
            //directoryname
            movieName = (string)MyFilms.r[Index][MyFilms.conf.StrStorage].ToString().Trim();
            movieName = movieName.Substring(movieName.LastIndexOf(";") + 1);
            LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) Splittet Mediadirectoryname: '" + movieName + "'"); 
            try    
            { directoryname = System.IO.Path.GetDirectoryName(movieName); }
            catch
            { directoryname = ""; }
            LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) Get Mediadirectoryname: '" + directoryname + "'");


            //Search Files in Mediadirectory (used befor: SearchFiles("trailer", directoryname, true, true);)
            string[] files = Directory.GetFiles(directoryname, "*.*", SearchOption.AllDirectories);
            foreach (string filefound in files)
                {
                    if (((filefound.ToLower().Contains("trailer")) || (filefound.ToLower().Contains("trlr")))&& (MediaPortal.Util.Utils.IsVideo(filefound)))
                    {
                        wsize = new System.IO.FileInfo(filefound).Length;
                        result.Add(filefound);
                        resultsize.Add(wsize);
                        filesfound[filesfoundcounter] = filefound;
                        filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                        filesfoundcounter = filesfoundcounter + 1;
                        LogMyFilms.Debug("MF: (TrailersearchLocal) - FilesFound in MediaDir: Size '" + wsize + "' - Name '" + filefound + "'");
                    }
                }
            
            //Search Filenames with "title" in Trailer Searchpath
            string[] directories;
            if (ExtendedSearch)
            {
                foreach (string storage in Trailerdirectories)
                {
                    LogMyFilms.Debug("MF: (TrailersearchLocal) - TrailerSearchDirectoriy: '" + storage + "'");
                    directories = Directory.GetDirectories(storage, "*.*", SearchOption.AllDirectories);
                    foreach (string directoryfound in directories)
                    {
                        if ((directoryfound.ToString().ToLower().Contains(titlename.ToLower())) || (directoryfound.ToString().ToLower().Contains(titlename2.ToLower())))
                        {
                            LogMyFilms.Debug("MF: (TrailersearchLocal) - Directory found: '" + directoryfound + "'");
                            files = Directory.GetFiles(directoryfound, "*.*", SearchOption.AllDirectories);
                            foreach (string filefound in files)
                            {
                                if (MediaPortal.Util.Utils.IsVideo(filefound))
                                {
                                    wsize = new System.IO.FileInfo(filefound).Length;
                                    result.Add(filefound);
                                    resultsize.Add(wsize);
                                    filesfound[filesfoundcounter] = filefound;
                                    filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                                    filesfoundcounter = filesfoundcounter + 1;
                                    LogMyFilms.Debug("MF: (TrailersearchLocal) - Files added matching Directory: Size '" + wsize + "' - Name '" + filefound + "'");
                                }
                            }

                        }
                        else
                        {
                            files = Directory.GetFiles(directoryfound, "*.*", SearchOption.AllDirectories);
                            foreach (string filefound in files)
                            {
                                if (((filefound.ToLower().Contains(titlename.ToLower())) || (filefound.ToLower().Contains(titlename2.ToLower()))) && (MediaPortal.Util.Utils.IsVideo(filefound)))
                                {
                                    wsize = new System.IO.FileInfo(filefound).Length;
                                    result.Add(filefound);
                                    resultsize.Add(wsize);
                                    filesfound[filesfoundcounter] = filefound;
                                    filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                                    filesfoundcounter = filesfoundcounter + 1;
                                    LogMyFilms.Debug("MF: (TrailersearchLocal) - Singlefiles found in TrailerDIR: Size '" + wsize + "' - Name '" + filefound + "'");
                                }
                            }
                        }
                    }
                }
            }
            
            var sort = from fn in filesfound
                       orderby new FileInfo(fn).Length descending
                       select fn;
            foreach (string n in filesfound)
                LogMyFilms.Debug("MF: (Sorted Trailerfiles) ******* : '" + n + "'");

            Array.Sort(filesfoundsize);
            for (int i = 0; i < result.Count; i++)
            {
                LogMyFilms.Debug("MF: (Sorted Trailerfiles) ******* : Number: '" + i + "' - Size: '" + filesfoundsize[i] + "' - Name: '" + filesfound[i] + "'");
            }

            string trailersourcepath = "";
            
            if (result.Count != 0)
                {
                    //result.Sort();
                trailersourcepath = result[0].ToString();
                    //ArrayList wresult = new ArrayList();
                    //foreach (String s in result)
                if (result.Count > 1)
                    {for (int i = 1; i < result.Count; i++)
                        {
                            trailersourcepath = trailersourcepath + ";" + result[i];
                            LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - Added Trailer to Trailersouce: '" + result[i] + "'");
                        }
                    }
                LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - Total Files found: " + result.Count);
                LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - TrailerSourcePath: '" + trailersourcepath + "'");
                }
            else
                LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - NO TRAILERS FOUND !!!!");

            if ((trailersourcepath.Length > 0) && (!(MyFilms.conf.StrStorageTrailer.Length == 0) && !(MyFilms.conf.StrStorageTrailer == "(none)")))
            {
                LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - Old Trailersourcepath: '" + MyFilms.r[Index][MyFilms.conf.StrStorageTrailer] + "'");
                MyFilms.r[Index][MyFilms.conf.StrStorageTrailer] = trailersourcepath;
                LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - New Trailersourcepath    : '" + MyFilms.r[Index][MyFilms.conf.StrStorageTrailer] + "'");
                Update_XML_database();
                LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - Database Updatewd !!!!");
            }
        }
        

        //-------------------------------------------------------------------------------------------
        //  Delete Trailer Entries From DB
        //-------------------------------------------------------------------------------------------        
        public static void DeleteTrailerFromDB(DataRow[] r1, int Index)
        {
          MyFilms.r[Index][MyFilms.conf.StrStorageTrailer] = string.Empty;
            LogMyFilms.Debug("MyFilmsDetails (DeleteTrailerFromDB) - Trailer entries Deleted for Current Movie !!!");
            Update_XML_database();
            LogMyFilms.Debug("MyFilmsDetails (DeleteTrailerFromDB) - Database Updatewd !!!!");
        }
        
        
        public static void SearchTrailerLocaltemp(int select_item, int GetID, GUIAnimation m_SearchAnimation)
        {
            setProcessAnimationStatus(true, m_SearchAnimation);
            // search all (Trailer)files 
            ArrayList newItems = new ArrayList();
            bool NoResumeMovie = true;
            int IMovieIndex = 0;

            //(int)MyFilms.conf.StrIndex)
            //(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString()
            LogMyFilms.Debug("SearchTrailerLocal - SelectedItemInfo from (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(): '" + (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1] + "'"));
            LogMyFilms.Debug("SearchTrailerLocal - SelectedItemInfo from (MyFilms.r[MyFilms.conf.StrIndex].ToString(): '" + (MyFilms.r[MyFilms.conf.StrIndex] + "'"));

            Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex, true);
            //Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex, false);
            if (newItems.Count > 20)
            // Maximum 20 entries (limitation for MP dialogFileStacking)
            {
                    GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                    dlgOk.SetLine(1, MyFilms.r[select_item][MyFilms.conf.StrSTitle].ToString());//video title
                    dlgOk.SetLine(2, "maximum 20 entries for the playlist");
                    dlgOk.DoModal(GetID);
                    LogMyFilms.Info("MF: Too many entries found for movie '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle] + "', number of entries found = " + newItems.Count);
                return;
            }
            setProcessAnimationStatus(false, m_SearchAnimation);
            if (newItems.Count > 1)
            {
                if (NoResumeMovie)
                {
                            GUIDialogFileStacking dlg = (GUIDialogFileStacking)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_FILESTACKING);
                    if (null != dlg)
                    {
                                    dlg.SetNumberOfFiles(newItems.Count);
                                    dlg.DoModal(GUIWindowManager.ActiveWindow);
                                    int selectedFileIndex = dlg.SelectedFile;
                                    if (selectedFileIndex < 1) return;
                                    IMovieIndex = selectedFileIndex++;
                    }
                }
            }
            if (newItems.Count > 0)
            {
                playlistPlayer.Reset();
                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_VIDEO_TEMP;
                PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP);
                playlist.Clear();

                foreach (object t in newItems)
                {
                    string movieFileName = (string)t;
                    PlayListItem newitem = new PlayListItem();
                    newitem.FileName = movieFileName;
                    newitem.Type = PlayListItem.PlayListItemType.Video;
                    playlist.Add(newitem);
                }
                // ask for start movie Index

                // play movie...
                PlayMovieFromPlayList(NoResumeMovie, IMovieIndex - 1);
            }

        }


        private static void HandleWakeUpNas()
        {
            string hostName;
            bool isWakeOnLanEnabled;
            bool isAutoMacAddressEnabled;
            int intTimeOut;
            String macAddress;
            byte[] hwAddress;

            //Get settings from MediaPortal.xml
            using (Settings xmlreader = new MPSettings())
            //using (Settings xmlreader = new MediaPortal.Profile.MPSettings())
            {
                hostName = xmlreader.GetValueAsString("nas", "hostname", "");
                isWakeOnLanEnabled = xmlreader.GetValueAsBool("nas", "isWakeOnLanEnabled", false);
                isAutoMacAddressEnabled = xmlreader.GetValueAsBool("nas", "isAutoMacAddressEnabled", false);
                intTimeOut = xmlreader.GetValueAsInt("nas", "WOLTimeOut", 10);
            }

            //isWakeOnlanEnabled
            if (isWakeOnLanEnabled)
            {
                //Check for multi-seat installation
                //if (!IsSingleSeat())
                if (true)
                    {
                    WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();

                    //isAutoMacAddressEnabled
                    if (isAutoMacAddressEnabled)
                    {
                        IPAddress ipAddress = null;

                        //Check if we already have a valid IP address stored in RemoteControl.HostName,
                        //otherwise try to resolve the IP address
                        if (!IPAddress.TryParse(hostName, out ipAddress))
                        {
                            //Get IP address of the NAS server
                            try
                            {
                                IPAddress[] ips = Dns.GetHostAddresses(hostName);

                                LogMyFilms.Debug("MF: (HandleWakeUpNas) : WOL - GetHostAddresses({0}) returns:", hostName);

                                foreach (IPAddress ip in ips)
                                {
                                    LogMyFilms.Debug("    {0}", ip);
                                }

                                //Use first valid IP address
                                ipAddress = ips[0];
                            }
                            catch (Exception ex)
                            {
                                LogMyFilms.Error("MF: (HandleWakeUpNas) : WOL - Failed GetHostAddress - {0}", ex.Message);
                            }
                        }

                        //Check for valid IP address
                        if (ipAddress != null)
                        {
                            //Update the MAC address if possible
                            hwAddress = wakeOnLanManager.GetHardwareAddress(ipAddress);

                            if (wakeOnLanManager.IsValidEthernetAddress(hwAddress))
                            {
                                LogMyFilms.Debug("MF: (HandleWakeUpNas) : WOL - Valid auto MAC address: {0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}"
                                          , hwAddress[0], hwAddress[1], hwAddress[2], hwAddress[3], hwAddress[4], hwAddress[5]);

                                //Store MAC address
                                macAddress = BitConverter.ToString(hwAddress).Replace("-", ":");

                                LogMyFilms.Debug("MF: (HandleWakeUpNas) : WOL - Store MAC address: {0}", macAddress);

                                using (
                                  MediaPortal.Profile.Settings xmlwriter =
                                    new MediaPortal.Profile.MPSettings())
                                {
                                    xmlwriter.SetValue("nas", "macAddress", macAddress);
                                }
                            }
                        }
                    }

                    //Use stored MAC address
                    using (Settings xmlreader = new MPSettings())
                    {
                        macAddress = xmlreader.GetValueAsString("nas", "macAddress", null);
                    }

                    LogMyFilms.Debug("MF: (HandleWakeUpNas) : WOL - Use stored MAC address: {0}", macAddress);

                    try
                    {
                        hwAddress = wakeOnLanManager.GetHwAddrBytes(macAddress);

                        //Finally, start up the TV server
                        LogMyFilms.Info("MF: (HandleWakeUpNas) : WOL - Start the NAS server");

                        if (wakeOnLanManager.WakeupSystem(hwAddress, hostName, intTimeOut))
                        {
                            LogMyFilms.Info("MF: (HandleWakeUpNas) : WOL - The NAS server started successfully!");
                        }
                        else
                        {
                            LogMyFilms.Error("MF: (HandleWakeUpNas) : WOL - Failed to start the NAS server");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMyFilms.Error("MF: (HandleWakeUpNas) : WOL - Failed to start the NAS server - {0}", ex.Message);
                    }
                }
            }
        }

        private void ShowMessageDialog(string headline, string line1, string line2)
        {
            GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            if (dlgOK != null)
            {
                dlgOK.SetHeading(headline);
                dlgOK.SetLine(1, line1);
                dlgOK.SetLine(2, line2);
                dlgOK.DoModal(GetID);
                return;
            }
        }

        //string getGUIProperty(guiProperty name)
        //{
        //    return getGUIProperty(name.ToString());
        //}

        public static string getGUIProperty(string name)
        {
            return GUIPropertyManager.GetProperty("#myfilms." + name);
        }

        //void setGUIProperty(guiProperty name, string value)
        //{
        //    setGUIProperty(name.ToString(), value);
        //}

        public static void setGUIProperty(string name, string value)
        {
            string property = "#myfilms." + name;
            LogMyFilms.Debug("MF: setGuiProperty [{0}]: '{1}'", property, value);
            GUIPropertyManager.SetProperty(property, value);
        }

        //void clearGUIProperty(guiProperty name)
        //{
        //    clearGUIProperty(name.ToString());
        //}

        public static void clearGUIProperty(string name)
        {
            setGUIProperty(name, string.Empty);
        }

        public static void GetActorByName(string strActorName, ArrayList actors)
        {
            strActorName = MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(strActorName);
            SQLiteClient m_db = new SQLiteClient(Config.GetFile(Config.Dir.Database, @"VideoDatabaseV5.db3"));

            if (m_db == null)
            {
                return;
            }
            try
            {
                actors.Clear();
                SQLiteResultSet results = m_db.Execute("select * from Actors where strActor like '%" + strActorName + "%'");
                if (results.Rows.Count == 0)
                {
                    return;
                }
                for (int iRow = 0; iRow < results.Rows.Count; iRow++)
                {
                    actors.Add(MediaPortal.Database.DatabaseUtility.Get(results, iRow, "idActor") + "|" +
                               MediaPortal.Database.DatabaseUtility.Get(results, iRow, "strActor"));
                }
            }
            catch (Exception ex)
            {

                LogMyFilms.Error("videodatabase exception err:{0} stack:{1}", ex.Message, ex.StackTrace);
            }
        }

        public static bool ExtendedStartmode(string disabledfeature)
        {
          if (global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.PluginMode != "normal") 
            return true;
          else
            LogMyFilms.Debug("MF: Disabled feature due to startmode 'normal': '" + disabledfeature + "'");
            return false;
        }

    }

}
        #endregion
