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
  using System.Collections.Generic;
  using System.ComponentModel; // for TRAKT
  using System.Threading; // For TRAKT Timer ...
  using System.Data;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Text.RegularExpressions;
  using System.Windows.Forms;

  using grabber;

  using Grabber;

  using MediaPortal.Configuration;
  using MediaPortal.Dialogs;
  using MediaPortal.GUI.Library;
  using MediaPortal.GUI.Video;
  using MediaPortal.Player;
  using MediaPortal.Playlists;
  using MediaPortal.Profile;
  using MediaPortal.Util;
  using MediaPortal.Video.Database;

  using MyFilmsPlugin.MyFilms;

  using MyFilmsPlugin.MyFilms.Utils;

  using SQLite.NET;

  using Trakt.Movie;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;
  using VideoThumbCreator = MyFilmsPlugin.MyFilms.Utils.VideoThumbCreator;
  using Trakt;

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

        public SQLiteClient m_db;
        public class IMDBActorMovie
        {
            public string MovieTitle;
            public string Role;
            public int Year;
        };
        static PlayListPlayer playlistPlayer;
        static VirtualDirectory m_directory = new VirtualDirectory();
        BackgroundWorker bgPicture = new System.ComponentModel.BackgroundWorker();

        static System.Windows.Forms.OpenFileDialog openFileDialog1 = new OpenFileDialog();
        static string _virtualStartDirectory = String.Empty;
        static VirtualDirectory virtualDirectory = new VirtualDirectory();
        static bool m_askBeforePlayingDVDImage = false;
        public static ArrayList result;
        public static string wsearchfile;
        public static int wGetID;
        //public static bool isTrailer = false;
        //public static bool trailerPlayed = false;
        public static bool trailerPlayed = false;

        // private System.Threading.Timer m_TraktTimer = null;
        // private TimerCallback m_timerDelegate = null;
        // BackgroundWorker TraktScrobbleUpdater = new BackgroundWorker();
        // private bool TraktMarkedFirstAsWatched = false;



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
            get { return MyFilms.ID_MyFilmsDetail; }
            set { base.GetID = value; }
        }

        public override string GetModuleName()
        {
          return GUILocalizeStrings.Get(MyFilms.ID_MyFilmsDetail); // return localized string for Module ID
        }

        public override bool Init()
        {
          LogMyFilms.Debug("MyFilmsDetail.Init() started/ended.");
          // trakt scrobble background thread
          //TraktScrobbleUpdater.WorkerSupportsCancellation = true;
          //TraktScrobbleUpdater.DoWork += new DoWorkEventHandler(TraktScrobble_DoWork);
  
          return Load(GUIGraphicsContext.Skin + @"\MyFilmsDetail.xml");
        }

        protected override void OnPageLoad()
        {
            Log.Debug("MyFilms.OnPageLoad() started.");
            base.OnPageLoad(); // let animations run!
            setGUIProperty("menu.overview", GUILocalizeStrings.Get(10798751));
            setGUIProperty("menu.description", GUILocalizeStrings.Get(10798752));
            setGUIProperty("menu.comments", GUILocalizeStrings.Get(10798753));
            setGUIProperty("menu.actors", GUILocalizeStrings.Get(10798754));
            setGUIProperty("menu.techinfos", GUILocalizeStrings.Get(10798755));

            BtnFirst.Label = GUILocalizeStrings.Get(1079872);
            BtnLast.Label = GUILocalizeStrings.Get(1079873);
            //GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnFirst, GUILocalizeStrings.Get(1079872));
            //GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLast, GUILocalizeStrings.Get(1079873));
            Log.Debug("MyFilms.OnPageLoad() finished.");
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
                GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilms);
                return;
            }

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU)
            {
                LogMyFilms.Debug("MyFilmsDetail : ACTION_CONTEXT_MENU detected ! ");
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
            LogMyFilms.Debug("MFD: OnMessage - MessageType: '" + messageType.Message.ToString() + "'");
            int dControl = messageType.TargetControlId;
            int iControl = messageType.SenderControlId;
            switch (messageType.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
                    //---------------------------------------------------------------------------------------
                    // Windows Init
                    //---------------------------------------------------------------------------------------
                    LogMyFilms.Debug("MFD: Message - WINDOWS_INIT - Starting");
                    bool result = base.OnMessage(messageType);
                    if (ImgDetFilm != null)
                        if (ImgDetFilm.IsVisible)
                            ImgDetFilm.Refresh();
                        else if (ImgDetFilm2!= null)
                          if (ImgDetFilm2.IsVisible)
                            ImgDetFilm2.Refresh();

                    //base.OnMessage(messageType); // Guzzi: Removing does not work properly...
                    wGetID = GetID;
                    GUIControl.ShowControl(GetID, 35);
                    // ToDo: Should be unhidden, if ActorThumbs are implemented
                    GUIControl.HideControl(GetID, (int)Controls.CTRL_ActorMultiThumb);
                    setProcessAnimationStatus(false, m_SearchAnimation);

                    // trakt scrobble background thread
                    //TraktScrobbleUpdater.WorkerSupportsCancellation = true;
                    //TraktScrobbleUpdater.DoWork += new DoWorkEventHandler(TraktScrobble_DoWork);

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
                    LogMyFilms.Debug("MFD: Message - WINDOWS_INIT - Finished");
                    return result;

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
                        GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilms);
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
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay); // Added to return to main view after menu
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlayTrailer)
                    // Search Trailer File to play
                    {
                      if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)")
                      {
                        trailerPlayed = true;
                        Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                      }
                        else
                          Change_Menu("trailermenu");
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_ViewFanart)
                    // On Button goto MyFilmsThumbs // Changed to alo launch player due to Ember Media Manager discontinued...
                    {
                        //GUIWindowManager.ActivateWindow(ID_MyFilmsThumbs);
                        Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnMovieThumbs)
                    {
                        GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilmsThumbs);
                        return true;
                    }

                    if (iControl == (int)Controls.CTRL_BtnActors)
                    {
                        GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilmsActors);
                        return true;
                    }

                    if (iControl == (int)Controls.CTRL_BtnActorThumbs)
                    {
                      // Show Actor Details Screen
                      //GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_HOME);
                      GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilmsActors);
                      // Hier Aktivitäten wie z.b. ListControl für Actors?
                      GUIWindowManager.ShowPreviousWindow();
                      //Update_XML_Items(); //To be changed, when DetailScreen is done!!!
                      return true;
                    }
                base.OnMessage(messageType); 
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

            string title = string.Empty; // variable for searchtitle creation
            string mediapath = string.Empty; // variable for searchpath creation (for nfo/xml/xbmc reader)
            switch (choiceView)
            {
                case "mainmenu":
                
                    if (dlgmenu == null) return;
                    dlgmenu.Reset();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798701)); // update menu

                    dlgmenu.Add(GUILocalizeStrings.Get(10798704));//trailer menu "Trailer ..."
                    choiceViewMenu.Add("trailermenu");

                    if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
                    {
                      if (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrWatchedField].ToString().ToLower() != MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower()) // show only the required option
                        dlgmenu.Add(GUILocalizeStrings.Get(1079895)); // set unwatched
                      else 
                        dlgmenu.Add(GUILocalizeStrings.Get(1079894)); // set watched
                      choiceViewMenu.Add("togglewatchedstatus");
                    }
                    
                    dlgmenu.Add(GUILocalizeStrings.Get(931));//rating
                    choiceViewMenu.Add("rating");

                    if (MyFilms.conf.StrSuppress)
                    {
                        dlgmenu.Add(GUILocalizeStrings.Get(432));
                        choiceViewMenu.Add("delete");
                    }

                    dlgmenu.Add(GUILocalizeStrings.Get(10798702)); // Updates ...
                    choiceViewMenu.Add("updatesmenu");

                    dlgmenu.Add(GUILocalizeStrings.Get(10798703)); // Fanart & Cover ...
                    choiceViewMenu.Add("fanartcovermenu");

                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1)
                    {
                      GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay); // Added to return to main view after menu // Removed as it's causing an exception
                      return;
                    }
                    Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                    break;
              
                case "playtrailer":
                    // first check, if trailer files are available, offer options
                    //if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
                    if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim()))
                    {
                      trailerPlayed = true;
                      Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                    }
                    else
                    {
                      // Can add autosearch&register logic here before try starting trailers

                      GUIDialogYesNo dlgYesNotrailersearch = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                      dlgYesNotrailersearch.SetHeading(GUILocalizeStrings.Get(10798704));//trailer
                      dlgYesNotrailersearch.SetLine(1, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrSTitle].ToString());//video title
                      dlgYesNotrailersearch.SetLine(2, GUILocalizeStrings.Get(10798737));//no video found locally
                      dlgYesNotrailersearch.SetLine(3, GUILocalizeStrings.Get(10798739)); // Search local trailers  and update DB ?
                      dlgYesNotrailersearch.DoModal(GetID);
                      //dlgYesNotrailersearch.DoModal(GUIWindowManager.ActiveWindow);
                      if (dlgYesNotrailersearch.IsConfirmed)
                      {
                        setProcessAnimationStatus(true, m_SearchAnimation);
                        //LogMyFilms.Debug("MF: (SearchTrailerLocal) SelectedItemInfo from (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(): '" + (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() + "'"));
                        LogMyFilms.Debug("MF: (Auto search trailer after selecting PLAY) title: '" + (MyFilms.r[MyFilms.conf.StrIndex].ToString() + "'"));
                        MyFilmsDetail.SearchTrailerLocal((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true);
                        afficher_detail(true);
                        setProcessAnimationStatus(false, m_SearchAnimation);
                        trailerPlayed = true;
                        Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                      }
                    }
                break;

                case "playtraileronlinevideos":
                case "playtraileronlinevideosappleitunes":
                case "playtraileronlinevideosimdbtrailer":
                      string site = string.Empty;
                      string titleextension = string.Empty;
                      switch (choiceView)
                      {
                        case "playtraileronlinevideos":
                          site = "YouTube";
                          titleextension = " " + MyFilms.r[MyFilms.conf.StrIndex]["Year"] + " trailer";
                          break;
                        case "playtraileronlinevideosappleitunes":
                          site = "iTunes Movie Trailers";
                          break;
                        case "playtraileronlinevideosimdbtrailer":
                          site = "IMDb Movie Trailers";
                          break;
                        default:
                          return;
                      }
              
                      // Load OnlineVideo Plugin with Searchparameters for YouTube and movie to Search ... OV reference for parameters: site:<sitename>|category:<categoryname>|search:<searchstring>|VKonfail:<true,false>|return:<Locked,Root>
                      //if (PluginManager.IsPluginNameEnabled2("OnlineVideos"))
                      if (MyFilms.OnlineVideosRightPlugin && MyFilms.OnlineVideosRightVersion)
                      {
                        title = string.Empty;
                        if (!string.IsNullOrEmpty(MyFilms.conf.ItemSearchGrabber) && !string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.ItemSearchGrabber].ToString()))
                          title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.ItemSearchGrabber].ToString(); // Configured GrabberTitle
                        else if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString())) // Mastertitle
                          title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
                        else if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString())) // Secondary title
                          title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString();
                        else if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString())) // Name from source (media)
                        {
                          title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
                          if (title.Contains(";")) title = title.Substring(0, title.IndexOf(";"));
                          if (title.Contains("\\")) title = title.Substring(title.LastIndexOf("\\") + 1);
                          if (title.Contains(".")) title = title.Substring(0, title.LastIndexOf("."));
                        }
                        if (title.IndexOf(MyFilms.conf.TitleDelim) > 0)
                          title = title.Substring(title.IndexOf(MyFilms.conf.TitleDelim) + 1);

                        string OVstartparams = "site:" + site + "|category:|search:" + title + titleextension + "|return:Locked";
                        //GUIPropertyManager.SetProperty("Onlinevideos.startparams", OVstartparams);
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", site);
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", title + titleextension);
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "Locked");

                        LogMyFilms.Debug("MF: Starting OnlineVideos with '" + OVstartparams.ToString() + "'");
                        // should this be set here to make original movie doesn't get set to watched??
                        // trailerPlayed = true;
                        GUIWindowManager.ActivateWindow(MyFilms.ID_OnlineVideos, false); // 4755 is ID for OnlineVideos
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
                        GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");
                    }
                    else
                    {
                        ShowMessageDialog("MyFilms", "OnlineVideo plugin not installed or wrong version", "Minimum Version required: 0.28");
                    }
                    break;

                case "togglewatchedstatus":
                    if (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrWatchedField].ToString().ToLower() != MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower())
                    {
                      MyFilmsDetail.Watched_Toggle((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, false);
                    }
                    else
                    {
                      MyFilmsDetail.Watched_Toggle((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true);
                    }
                    afficher_detail(true);
                    break;

              case "rating":
                    MyFilmsDialogSetRating dlgRating = (MyFilmsDialogSetRating)GUIWindowManager.GetWindow(MyFilms.ID_MyFilmsDialogRating);
                    //NumberFormatInfo nfi = new NumberFormatInfo();
                    //nfi.NumberDecimalSeparator = ",";
                    //nfi.NumberGroupSeparator = "";
                    //decimal wrating = 0;
                    if (MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Length > 0)
                    {
                      //wrating = Decimal.Parse(MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Replace(".", ","), nfi);
                      //wrating = Convert.ToDecimal(MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Replace(".", ","), nfi);
                      //CultureInfo ci = new CultureInfo("en-us");
                      //dlgRating.Rating = (decimal)MyFilms.r[MyFilms.conf.StrIndex]["Rating"];
                      //try { wrating = Convert.ToDecimal(MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Replace(".", ",")); }

                      //if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "." && MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Contains("."))
                      //{
                      //  try
                      //  {
                      //    wrating = Decimal.Parse(MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Replace(".", ","), nfi);
                      //    //wrating = Convert.ToDecimal(MyFilms.r[MyFilms.conf.StrIndex]["Rating"], CultureInfo.CurrentCulture);
                      //    LogMyFilms.Debug("MF: Rating dialog using cultureinfo: '" + CultureInfo.CurrentCulture.ToString() + "'");
                      //  }
                      //  catch { }
                      //}
                      //else
                      //{
                      //  wrating = Decimal.Parse(MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString(), nfi);
                      //}
                      
                      //try { wrating = Convert.ToDecimal(MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Replace(".", ",")); }
                      //catch
                     // {
                      //LogMyFilms.Debug("MF: Rating dialog using cultureinfo (decimalseparator): '" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString() + "'");
                      //wrating = (decimal)MyFilms.r[MyFilms.conf.StrIndex]["Rating"]; 
                      //wrating = Decimal.Parse(MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Replace(".", ","), nfi);
                      //try
                        //{
                          //wrating = Decimal.Parse(MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Replace(".", ","), nfi);
                          //wrating = Convert.ToDecimal(MyFilms.r[MyFilms.conf.StrIndex]["Rating"], CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        //}

                        //catch { }
                      //}
                      //dlgRating.Rating = wrating;
                      dlgRating.Rating = (decimal)MyFilms.r[MyFilms.conf.StrIndex]["Rating"];
                      if (dlgRating.Rating > 10) 
                        dlgRating.Rating = 10;
                    }
                    else
                        dlgRating.Rating = 0;

                    dlgRating.SetTitle(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString());
                    dlgRating.DoModal(GetID);
                    if (dlgmenu.SelectedLabel != 2) // If not returning from "ok"
                      Change_Menu("mainmenu");
                    //wrating = dlgRating.Rating;
                    //MyFilms.r[MyFilms.conf.StrIndex]["Rating"] = dlgRating.Rating.ToString("0.0", nfi);
                    //MyFilms.r[MyFilms.conf.StrIndex]["Rating"] = dlgRating.Rating.ToString("0,0", nfiback).Replace(",", ".");
                    //MyFilms.r[MyFilms.conf.StrIndex]["Rating"] = wrating.ToString().Replace(",", ".");
                    MyFilms.r[MyFilms.conf.StrIndex]["Rating"] = dlgRating.Rating;
                    Update_XML_database();
                    afficher_detail(true);
                    break;

              case "updatesmenu":
                    if (dlgmenu == null) return;
                    dlgmenu.Reset();
                    choiceViewMenu.Clear();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798702)); // Updates ...

                    dlgmenu.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
                    choiceViewMenu.Add("grabber");

                    if (MyFilms.conf.StrUpdList[0].Length > 0)
                    {
                      dlgmenu.Add(GUILocalizeStrings.Get(10798642));  // Update by Property (choosen within the UPdate List Property
                      choiceViewMenu.Add("updproperty");
                    }

                    //if (MyFilms.conf.StrStorage.Length != 0 && MyFilms.conf.StrStorage != "(none)" && (MyFilms.conf.WindowsFileDialog))
                    if (MyFilms.conf.StrStorage.Length != 0 && MyFilms.conf.StrStorage != "(none)")
                    {
                      dlgmenu.Add(GUILocalizeStrings.Get(10798636));//filename
                      choiceViewMenu.Add("fileselect");
                    }

                    //No more needed because of updproperties !!! - so discussion about removal?
                    if (StrUpdItem1 != "(none)")
                    {
                      if (StrUpdText1.Length > 0)
                        dlgmenu.Add(StrUpdText1);        //Specific Item1 label to update
                      else
                        dlgmenu.Add(StrUpdItem1);        //Specific Item1 to update
                      choiceViewMenu.Add("item1");
                    }
                    if (StrUpdItem2 != "(none)")
                    {
                      if (StrUpdText2.Length > 0)
                        dlgmenu.Add(StrUpdText2);        //Specific Item2 label to update
                      else
                        dlgmenu.Add(StrUpdItem2);        //Specific Item2 to update
                      choiceViewMenu.Add("item2");
                    }

                    if (ExtendedStartmode("Details context: nfo-reader-update"))
                    {
                      dlgmenu.Add(GUILocalizeStrings.Get(10798730));             //Update Moviedetails from nfo-file - also download actor thumbs, Fanart, etc. if available
                      choiceViewMenu.Add("nfo-reader-update");
                    }

                    //dlgmenu.Add(GUILocalizeStrings.Get(10798721));             //Update Moviedetails from ant.info file
                    //choiceViewMenu.Add("ant-nfo-reader");

                    //dlgmenu.Add(GUILocalizeStrings.Get(10798722));             //Save Moviedetails to ant.info file
                    //choiceViewMenu.Add("ant-nfo-writer");

                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1)
                    {
                      Change_Menu("mainmenu");
                      return;
                    }
                    Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                    break;

              case "fanartcovermenu":
                    if (dlgmenu == null) return;
                    dlgmenu.Reset();
                    choiceViewMenu.Clear();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798703)); // Fanart & Cover ...

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

                    if (ExtendedStartmode("Details context: Thumb creator (and fanart creator?)"))
                    {
                      dlgmenu.Add(GUILocalizeStrings.Get(10798728));
                      //Create Thumb from movie - if no cover available, e.g. with documentaries
                      choiceViewMenu.Add("cover-thumbnailer");
                    }

                    dlgmenu.Add(GUILocalizeStrings.Get(10798761)); // Load Covers (TMDB)
                    choiceViewMenu.Add("tmdbposter");

                    //if (ExtendedStartmode("Details context: Change Local COver)"))
                    //{
                    dlgmenu.Add(GUILocalizeStrings.Get(10798762)); // Change Cover
                    choiceViewMenu.Add("changecover");
                    //}

                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1)
                    {
                      Change_Menu("mainmenu");
                      return;
                    }
                    Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                    break;

              case "trailermenu":
                    if (dlgmenu == null) return;
                    dlgmenu.Reset();
                    choiceViewMenu.Clear();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798704)); // Trailer ...

                    if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
                    {
                      string trailercount = "";
                      if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim())) 
                        trailercount = "0";
                      else
                      {
                      string[] split1 = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim().Split(new Char[] { ';' });
                      trailercount = split1.Count().ToString();
                      }
                      if (trailercount != "0")
                      {
                        dlgmenu.Add(GUILocalizeStrings.Get(10798710) + " (" + trailercount + ")");//play trailer (<number trailers present>)
                        choiceViewMenu.Add("playtrailer");
                      }
                    }
                    dlgmenu.Add(GUILocalizeStrings.Get(10798711));//search youtube trailer with onlinevideos
                    choiceViewMenu.Add("playtraileronlinevideos");

                    dlgmenu.Add(GUILocalizeStrings.Get(10798712));//search apple itunes trailer with onlinevideos
                    choiceViewMenu.Add("playtraileronlinevideosappleitunes");

                    dlgmenu.Add(GUILocalizeStrings.Get(10798716)); //search IMDB trailer with onlinevideos
                    choiceViewMenu.Add("playtraileronlinevideosimdbtrailer");


                    if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
                    {
                      dlgmenu.Add(GUILocalizeStrings.Get(10798723));             //Search local Trailer and Update DB (local)
                      choiceViewMenu.Add("trailer-register");

                      dlgmenu.Add(GUILocalizeStrings.Get(10798725));             //delete Trailer entries from DB record
                      choiceViewMenu.Add("trailer-delete");

                      if (ExtendedStartmode("Details context: Trailer Download"))
                      {
                        dlgmenu.Add(GUILocalizeStrings.Get(10798724)); //load IMDB Trailer, store locally and update DB
                        choiceViewMenu.Add("trailer-imdb");
                      }
                    }

                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1)
                    {
                      Change_Menu("mainmenu");
                      return;
                    }
                    Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                    break;

                case "fileselect":
                    string wfile = string.Empty;
                    string wdirectory = string.Empty;
                    if (System.IO.File.Exists(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString())) // Check if Sourcefile exists
                    {
                      wfile = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
                      wdirectory = System.IO.Path.GetDirectoryName(wfile);
                    }
                    if (MyFilms.conf.WindowsFileDialog)
                    {
                      openFileDialog1.Title = "Select media file";
                      openFileDialog1.RestoreDirectory = true;
                      openFileDialog1.InitialDirectory = wdirectory;
                      if (openFileDialog1.ShowDialog() == DialogResult.OK)
                          wfile = openFileDialog1.FileName;
                    }
                    else
                    {
                      keyboard.Reset();
                      keyboard.Text = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
                      keyboard.DoModal(GetID);
                      if (keyboard.IsConfirmed)
                      {
                        wfile = keyboard.Text.ToString();
                      }
                      else 
                        wfile = string.Empty;
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
                            // Update_XML_database();
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
                        dlgmenu.Add(" " + BaseMesFilms.Translate_Column(wupd.Trim()));
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
                    title = string.Empty;
                    mediapath = string.Empty;
                    if (!string.IsNullOrEmpty(MyFilms.conf.ItemSearchGrabber) && !string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.ItemSearchGrabber].ToString()))
                    {
                      title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.ItemSearchGrabber].ToString(); // Configured GrabberTitle
                      LogMyFilms.Debug("MF: selecting (grabb_Internet_Informations) with '" + MyFilms.conf.ItemSearchGrabber + "' = '" + title.ToString() + "'");
                    }
                    else if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString())) // Mastertitle
                    {
                      title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
                      LogMyFilms.Debug("MF: selecting (grabb_Internet_Informations) with (master)title = '" + title.ToString() + "'");
                    }
                    else if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString())) // Secondary title
                    {
                      title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString();
                      LogMyFilms.Debug("MF: selecting (grabb_Internet_Informations) with (secondary)title = '" + title.ToString() + "'");
                    }
                    else if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString())) // Name from source (media)
                    {
                      title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
                      if (title.Contains(";")) title = title.Substring(0, title.IndexOf(";"));
                      if (title.Contains("\\")) title = title.Substring(title.LastIndexOf("\\") + 1);
                      if (title.Contains(".")) title = title.Substring(0, title.LastIndexOf("."));
                      LogMyFilms.Debug("MF: selecting (grabb_Internet_Informations) with (media source)name = '" + title.ToString() + "'");
                    }
                    if (title.IndexOf(MyFilms.conf.TitleDelim) > 0)
                        title = title.Substring(title.IndexOf(MyFilms.conf.TitleDelim) + 1);
                    mediapath = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
                    if (mediapath.Contains(";")) // take the forst source file
                    {
                      mediapath = mediapath.Substring(0, mediapath.IndexOf(";"));
                    }

                    grabb_Internet_Informations(title, GetID, wChooseScript, MyFilms.conf.StrGrabber_cnf, mediapath);
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
                    //ToDo: Add Dialog to let user choose, if cover should replace existing one (remove skip existing logic for that!)
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
                        dlgOk.SetLine(2, result.Count.ToString() + " " + GUILocalizeStrings.Get(10798705)); // trailers found ! 
                        dlgOk.DoModal(GetID);
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
                    string personartworkpath = string.Empty;
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
                        if (MyFilms.conf.StrPersons && !string.IsNullOrEmpty(MyFilms.conf.StrPathArtist))
                        {
                          LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download PersonArtwork 'enabled' - destination: '" + personartworkpath + "'");
                          personartworkpath = MyFilms.conf.StrPathArtist;
                        }
                        Download_Backdrops_Fanart(wtitle, wttitle, wdirector.ToString(), wyear.ToString(), true, GetID, wtitle, personartworkpath);
                    }
                    afficher_detail(true);
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    break;
                case "deletefanart":
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079874));//Delete fanart
                    dlgYesNo.SetLine(1, "");
                    dlgYesNo.SetLine(2, GUILocalizeStrings.Get(433));//confirm suppression
                    dlgYesNo.DoModal(GetID);
                    if (dlgYesNo.IsConfirmed)
                        Remove_Backdrops_Fanart(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(), false);
                    afficher_detail(true);
                    break;

                case "changecover":
                    ChangeLocalCover((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true);
                    afficher_detail(true);
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    break;

                case "tmdbposter":
                    setProcessAnimationStatus(true, m_SearchAnimation);
                    //Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
                    wtitle = string.Empty;
                    if (MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] != null && MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString().Length > 0)
                      wtitle = MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString();
                    if (wtitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
                      wtitle = wtitle.Substring(wtitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
                    wttitle = string.Empty;
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
                      LogMyFilms.Debug("MyFilmsDetails (posters - menu select) Download Posters: originaltitle: '" + wtitle + "' - translatedtitle: '" + wttitle + "' - director: '" + wdirector + "' - year: '" + wyear.ToString() + "'");
                      Download_TMDB_Posters(wtitle, wttitle, wdirector, wyear.ToString(), true, GetID, wtitle);
                    }
                    afficher_detail(true);
                    setProcessAnimationStatus(false, m_SearchAnimation);
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
        //  Set an entry from the database to watched/unwatched
        //-------------------------------------------------------------------------------------------        
        public static void Watched_Toggle(DataRow[] r1, int Index, bool watched)
        {
          if (watched)
          {
            MyFilms.r[Index][MyFilms.conf.StrWatchedField] = "true";
            LogMyFilms.Info("MF: Database movie set 'watched' by setting '" + MyFilms.conf.StrWatchedField.ToString() + "' to '" + "true" + "' for movie: " + MyFilms.r[Index][MyFilms.conf.StrTitle1]);
          }
          else
          {
            MyFilms.r[Index][MyFilms.conf.StrWatchedField] = MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower();
            LogMyFilms.Info("MF: Database movie set 'watched' by setting '" + MyFilms.conf.StrWatchedField.ToString() + "' to '" + MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower() + "' for movie: " + MyFilms.r[Index][MyFilms.conf.StrTitle1]);
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
        public static void grabb_Internet_Informations(string FullMovieName, int GetID, bool choosescript, string wscript, string FullMoviePath)
        {
            LogMyFilms.Debug("MF: launching (grabb_Internet_Informations) with title = '" + FullMovieName + "', choosescript = '" + choosescript + "', grabberfile = '" + wscript + "'");
            if (choosescript)
            {
              if (System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\scripts\myfilms"))
                {
                    // Grabber Directory filled, search for XML scripts files
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(10798706)); // "Choose internet grabber script"
                    if (dlg == null) return;
                    ArrayList scriptfile = new ArrayList();
                    if (MyFilms.conf.StrGrabber_cnf.Length > 0)
                    {
                        scriptfile.Add(MyFilms.conf.StrGrabber_cnf);
                        dlg.Add(MyFilms.conf.StrGrabber_cnf.Substring(MyFilms.conf.StrGrabber_cnf.LastIndexOf("\\") + 1) + " (default)");
                        dlg.SelectedLabel = 0;
                    }
                    DirectoryInfo dirsInf = new DirectoryInfo(Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\scripts\myfilms");
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
                    dlgOk.SetLine(1, string.Format(GUILocalizeStrings.Get(1079876), Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\scripts\myfilms"));
                    dlgOk.SetLine(2, GUILocalizeStrings.Get(1079877));
                    dlgOk.DoModal(GetID);
                    LogMyFilms.Info("My Films : The Directory grabber config files doesn't exists. Verify your Configuration !");
                    return;                    
                }
            }
            string MovieName = FullMovieName;
            string MovieHierarchy = string.Empty;
            string MoviePath = FullMoviePath;
            if (MyFilms.conf.TitleDelim.Length > 0)
            {
                MovieName = FullMovieName.Substring(FullMovieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
                MovieHierarchy = FullMovieName.Substring(0, FullMovieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
            }
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            Grabber.Grabber_URLClass.IMDBUrl wurl;

            ArrayList listUrl = Grab.ReturnURL(MovieName, wscript, 1, !MyFilms.conf.StrGrabber_Always, MoviePath);
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
                    dlg.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
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
                            grabb_Internet_Informations(keyboard.Text.ToString(), GetID, false, wscript, MoviePath);
                        break;
                    }
                    if (dlg.SelectedLabel > 0)
                    {
                        wurl = (Grabber.Grabber_URLClass.IMDBUrl)listUrl[dlg.SelectedLabel - 1];
                        grabb_Internet_Details_Informations(wurl.URL, MovieHierarchy, wscript, GetID, true, false, "");
                    }
                    break;
            }
        }
        //-------------------------------------------------------------------------------------------
        //  Grab Internet Movie Details Informations and update the XML database and refresh screen
        //-------------------------------------------------------------------------------------------        
        public static void grabb_Internet_Details_Informations(string url, string moviehead, string wscript, int GetID, bool interactive, bool nfo, string nfofile)
        {
//0  - "OriginalTitle", 
//1  - "TranslatedTitle", 
//2  - "Picture", 
//3  - "Description", 
//4  - "Rating", 
//5  - "Actors", 
//6  - "Director", 
//7  - "Producer", 
//8  - "Year", 
//9  - "Country", 
//10 - "Category", 
//11 - "URL"
//12 - "ImageURL"
//13 - "MultipurposeURLlink"
//14 - comment
//15 - language
//16 - tagline
//17 - certification
            
            LogMyFilms.Debug("MF: launching (grabb_Internet_Details_Informations) with url = '" + url.ToString() + "', moviehead = '" + moviehead + "', wscript = '" + wscript + "', GetID = '" + GetID.ToString() + "', interactive = '" + interactive.ToString() + "'"); 
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            string[] Result = new string[20];
            string title = string.Empty;
            string ttitle = string.Empty;
            string wtitle = string.Empty;
            int year = 0;
            string director = string.Empty;
            XmlConfig XmlConfig = new XmlConfig();
            // Those settings were used in the past from AMCupdater settings - now they exist in MF config as primary source!
            //string Img_Path = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Image_Download_Filename_Prefix", "");
            //string Img_Path_Type = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Store_Image_With_Relative_Path", "false");
            bool onlymissing = false;

            if (nfo)
                Result = Grab.GetNfoDetail(nfofile, MyFilms.conf.StrPathImg + MyFilms.conf.StrPicturePrefix, MyFilms.conf.StrPathArtist, "");
            else
            {
              string downLoadPath;
              if (interactive)
              {
                //downLoadPath = Config.GetDirectoryInfo(Config.Dir.Config) + @"\Thumbs\MyFilms";
                downLoadPath = Path.GetTempPath();
              }
              else
                downLoadPath = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix;
              Result = Grab.GetDetail(url, downLoadPath, wscript);
              LogMyFilms.Info("MF: Grabber - downloadpath = '" + downLoadPath + "'");
            }
            LogMyFilms.Info("MF: Grab Internet/nfo Information done for title/ttitle: " + MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] + "/" + MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString());

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

                string[] PropertyList = new string[] { "OriginalTitle", "TranslatedTitle", "Picture", "Description", "Rating", "Actors", "Director", "Producer", "Year", "Country", "Category", "URL", "ImageURL", "MultipurposeURLlink", "Comments", "Languages", "Tagline", "Certification" };
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

                        if (wProperty != "ImageURL" && wProperty != "MultipurposeURLlink" && wProperty != "Tagline" && wProperty != "Certification")
                        {
                          dlgmenu.Add(BaseMesFilms.Translate_Column(wProperty) + ": '" + strOldValue + "' -> '" + strNewValue + "'");
                          choiceViewMenu.Add(wProperty);
                          LogMyFilms.Debug("MF: GrabberUpdate - Add to menu (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                        }
                        else
                        {
                          LogMyFilms.Debug("MF: GrabberUpdate - Not added to menu (unsupported) - (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                        }
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
              LogMyFilms.Debug("MF: GrabInternetDetails - interactive choice: '" + strChoice + "'");
                if (strChoice == "missing")
                    onlymissing = true;
            }

                
            switch (strChoice)
            {
                case "OriginalTitle":
                if (!string.IsNullOrEmpty(Result[0]))
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
                    if (!string.IsNullOrEmpty(Result[1]))
                    {
                        ttitle = Result[1];
                        if (string.IsNullOrEmpty(ttitle) && MyFilms.conf.StrTitle1 == "TranslatedTitle" && !string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString())) // Added to fill ttitle with otitle in case ttitle is empty and mastertitle = ttitle and mastertitle is empty
                          ttitle = Result[0].ToString();
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
                    if (!string.IsNullOrEmpty(Result[2]))
                    {
                        string tmpPicture = Result[2];
                        string tmpPicturename = ""; // picturename only
                        string oldPicture = MyFilmsDetail.getGUIProperty("picture");
                        string newPicture = ""; // full path to new picture
                        string newPictureCatalogname = ""; // entry to be stored in catalog
                        if (string.IsNullOrEmpty(oldPicture))
                          oldPicture = "";

                        // set defaults...
                        tmpPicturename = Result[2].Substring(Result[2].LastIndexOf("\\") + 1);
                        newPictureCatalogname = tmpPicturename; 
                        
                        if (MyFilms.conf.StrPicturePrefix.Length > 0)
                          newPicture = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + tmpPicturename;
                        else
                          newPicture = MyFilms.conf.StrPathImg + "\\" + tmpPicturename;
                      
                        if (MyFilms.conf.PictureHandling == "Relative Path" || string.IsNullOrEmpty(MyFilms.conf.PictureHandling))
                        {
                          newPictureCatalogname = MyFilms.conf.StrPicturePrefix + tmpPicturename;
                        }
                        if (MyFilms.conf.PictureHandling == "Full Path")
                        {
                          newPictureCatalogname = newPicture;
                        }

                        LogMyFilms.Debug("Cover Image path : '" + MyFilms.conf.StrPathImg + "'");
                        LogMyFilms.Debug("Picturehandling  : '" + MyFilms.conf.PictureHandling + "'");
                        LogMyFilms.Debug("PicturePrefix    : '" + MyFilms.conf.StrPicturePrefix + "'");
                        LogMyFilms.Debug("Old  Cover Image : '" + oldPicture + "'");
                        LogMyFilms.Debug("Temp Cover Image : '" + tmpPicture + "'");
                        LogMyFilms.Debug("New  Cover Image : '" + newPicture + "'");
                        LogMyFilms.Debug("New Catalog Entry: '" + newPictureCatalogname + "'");

                        setGUIProperty("picture", tmpPicture);
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
                        if (!System.IO.Directory.Exists(newPicture.Substring(0, newPicture.LastIndexOf("\\"))))
                        {
                          try
                          {
                            System.IO.Directory.CreateDirectory(newPicture.Substring(0, newPicture.LastIndexOf("\\")));
                          }
                          catch (Exception ex)
                          {
                            LogMyFilms.Debug("Could not create directory '" + newPicture.Substring(0, newPicture.LastIndexOf("\\")) + "' - Exception: " + ex.ToString());
                          }
                        }
                        try
                        {
                          File.Copy(tmpPicture, newPicture, true);
                        }
                        catch (Exception ex)
                        {
                          LogMyFilms.Debug("Error copy file: '" + tmpPicture + "' - Exception: " + ex.ToString());
                        }
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString()) || !onlymissing)
                            MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
                        try
                        {
                          File.Delete(tmpPicture);
                        }
                        catch (Exception ex)
                        {
                          LogMyFilms.Debug("Error deleting tmp file: '" + tmpPicture + "' - Exception: " + ex.ToString());
                        }
                    }
                    break;
                case "Description":
                    if (!string.IsNullOrEmpty(Result[3]))
                        MyFilms.r[MyFilms.conf.StrIndex]["Description"] = Result[3].ToString();
                    break;
                case "Rating":
                    if (!string.IsNullOrEmpty(Result[4]))
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
                    if (!string.IsNullOrEmpty(Result[5]))
                        MyFilms.r[MyFilms.conf.StrIndex]["Actors"] = Result[5].ToString();
                    break;
                case "Director":
                    if (!string.IsNullOrEmpty(Result[6]))
                    {
                        director = Result[6].ToString();
                        MyFilms.r[MyFilms.conf.StrIndex]["Director"] = Result[6].ToString();
                    }
                    break;
                case "Producer":
                    if (!string.IsNullOrEmpty(Result[7]))
                        MyFilms.r[MyFilms.conf.StrIndex]["Producer"] = Result[7].ToString();
                    break;
                case "Year":
                    if (!string.IsNullOrEmpty(Result[8]))
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
                    if (!string.IsNullOrEmpty(Result[9]))
                        MyFilms.r[MyFilms.conf.StrIndex]["Country"] = Result[9].ToString();
                    break;
                case "Category":
                    if (!string.IsNullOrEmpty(Result[10]))
                        MyFilms.r[MyFilms.conf.StrIndex]["Category"] = Result[10].ToString();
                    break;
                case "URL":
                    if (!string.IsNullOrEmpty(Result[11]))
                      if (MyFilms.conf.StrStorage != "URL")
                        MyFilms.r[MyFilms.conf.StrIndex]["URL"] = Result[11].ToString();
                    break;
                case "Comments":
                    if (!string.IsNullOrEmpty(Result[14]))
                      MyFilms.r[MyFilms.conf.StrIndex]["Comments"] = Result[14].ToString(); 
                    break;
                case "Languages":
                    if (!string.IsNullOrEmpty(Result[15]))
                      MyFilms.r[MyFilms.conf.StrIndex]["Languages"] = Result[15].ToString();
                    break;
                case "all":
                case "missing":
                    if (!string.IsNullOrEmpty(Result[0]))
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
                    if (!string.IsNullOrEmpty(Result[1]))
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
                    if (!string.IsNullOrEmpty(Result[2]))
                    {
                      string tmpPicture = Result[2];
                      string tmpPicturename = ""; // picturename only
                      string newPicture = ""; // full path to new picture
                      string newPictureCatalogname = ""; // entry to be stored in catalog

                      // set defaults...
                      tmpPicturename = Result[2].Substring(Result[2].LastIndexOf("\\") + 1);
                      newPictureCatalogname = tmpPicturename;

                      if (MyFilms.conf.StrPicturePrefix.Length > 0)
                        newPicture = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + tmpPicturename;
                      else
                        newPicture = MyFilms.conf.StrPathImg + "\\" + tmpPicturename;


                      if (MyFilms.conf.PictureHandling == "Relative Path" || string.IsNullOrEmpty(MyFilms.conf.PictureHandling))
                      {
                        newPictureCatalogname = MyFilms.conf.StrPicturePrefix + tmpPicturename;
                      }
                      if (MyFilms.conf.PictureHandling == "Full Path")
                      {
                        newPictureCatalogname = newPicture;
                      }

                      if (newPicture != tmpPicture)
                      {
                        if (!System.IO.Directory.Exists(newPicture.Substring(0, newPicture.LastIndexOf("\\"))))
                        {
                          try
                          {
                            System.IO.Directory.CreateDirectory(newPicture.Substring(0, newPicture.LastIndexOf("\\")));
                          }
                          catch (Exception ex)
                          {
                            LogMyFilms.Debug(
                              "Could not create directory '" + newPicture.Substring(0, newPicture.LastIndexOf("\\")) +
                              "' - Exception: " + ex.ToString());
                          }
                        }
                        try
                        {
                          File.Copy(tmpPicture, newPicture, true);
                        }
                        catch (Exception ex)
                        {
                          LogMyFilms.Debug("Error copy file: '" + tmpPicture + "' - Exception: " + ex.ToString());
                        }
                      }
                      if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString()) || !onlymissing)
                          MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
                      if (newPicture != tmpPicture)
                      {
                        try
                        {
                          File.Delete(tmpPicture);
                        }
                        catch (Exception ex)
                        {
                          LogMyFilms.Debug("Error deleting tmp file: '" + tmpPicture + "' - Exception: " + ex.ToString());
                        }
                      }
                    }

                    if (!string.IsNullOrEmpty(Result[3]))
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Description"].ToString()) || !onlymissing)
                            MyFilms.r[MyFilms.conf.StrIndex]["Description"] = Result[3].ToString();
                    if (!string.IsNullOrEmpty(Result[4]))
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
                    if (!string.IsNullOrEmpty(Result[5]))
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Actors"].ToString()) || !onlymissing)
                            MyFilms.r[MyFilms.conf.StrIndex]["Actors"] = Result[5].ToString();
                    if (!string.IsNullOrEmpty(Result[6]))
                    {
                        director = Result[6].ToString();
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Director"].ToString()) || !onlymissing) 
                            MyFilms.r[MyFilms.conf.StrIndex]["Director"] = Result[6].ToString();
                    }
                    if (!string.IsNullOrEmpty(Result[7]))
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Producer"].ToString()) || !onlymissing) 
                            MyFilms.r[MyFilms.conf.StrIndex]["Producer"] = Result[7].ToString();
                    if (!string.IsNullOrEmpty(Result[8]))
                    {
                        try
                        {
                            year = Convert.ToInt16(Result[8].ToString());
                        }
                        catch { }
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString()) || !onlymissing) 
                            MyFilms.r[MyFilms.conf.StrIndex]["Year"] = Result[8].ToString();
                    }
                    if (!string.IsNullOrEmpty(Result[9]))
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Country"].ToString()) || !onlymissing) 
                            MyFilms.r[MyFilms.conf.StrIndex]["Country"] = Result[9].ToString();
                    if (!string.IsNullOrEmpty(Result[10]))
                        if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Category"].ToString()) || !onlymissing) 
                            MyFilms.r[MyFilms.conf.StrIndex]["Category"] = Result[10].ToString();
                    if (!string.IsNullOrEmpty(Result[11]))
                        if (MyFilms.conf.StrStorage != "URL")
                            if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["URL"].ToString()) || !onlymissing) 
                                MyFilms.r[MyFilms.conf.StrIndex]["URL"] = Result[11].ToString();
                    if (!string.IsNullOrEmpty(Result[14]))
                      if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Comments"].ToString()) || !onlymissing)
                        MyFilms.r[MyFilms.conf.StrIndex]["Comments"] = Result[14].ToString();
                    if (!string.IsNullOrEmpty(Result[15]))
                      if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Languages"].ToString()) || !onlymissing)
                        MyFilms.r[MyFilms.conf.StrIndex]["Languages"] = Result[15].ToString();
                    break;

                default:
                    break;
            }
            Update_XML_database();
            LogMyFilms.Info("MF: Database Updated for title/ttitle: " + title + "/" + ttitle);
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

            string fileName = MyFilms.r[MyFilms.conf.StrIndex]["Source"].ToString();
            if (string.IsNullOrEmpty(fileName))
            {
              LogMyFilms.Debug("MF: (CreateThumbFromMovie): Error - Moviefilesource is empty !");
              return;
            }
            if (fileName.Contains("VIDEO_TS\\VIDEO_TS.IFO")) // Do not try to create thumbnails for DVDs
            {
              LogMyFilms.Debug("MF: (CreateThumbFromMovie): Moviesource is DVD - return without creating coverfile...");
              return;
            }
            if (fileName.Contains(";"))
              fileName = fileName.Substring(0, fileName.IndexOf(";")).Trim();
            if (!System.IO.File.Exists(fileName))
            {
              LogMyFilms.Debug("MF: (CreateThumbFromMovie): Error - Moviefilesource: '" + fileName + "' does not exist !");
              return;
            }
            string tempImage = System.IO.Path.GetTempPath() + "MovieThumb_" + MyFilms.r[MyFilms.conf.StrIndex]["Number"].ToString() + ".jpg";
            if (System.IO.File.Exists(tempImage)) System.IO.File.Delete(tempImage);
            //if (System.IO.File.Exists(tempImage + "L")) System.IO.File.Delete(tempImage + "L");
            string strThumb = MyFilms.conf.StrPathImg + "\\MovieThumb_" + MyFilms.r[MyFilms.conf.StrIndex]["Number"].ToString() + ".jpg";
            LogMyFilms.Debug("MF: (CreateThumbFromMovie): Moviefilesource: '" + fileName + "', Covernamedestination: '" + strThumb + "', TempImage: '" + tempImage + "'");

            //if (Img_Path_Type.ToLower() == "true")
            //    MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = strThumb;

            //if (File.Exists(strThumb))
            //{
            //  LogMyFilms.Debug("MF: (CreateThumbFromMovie): Coverimagefile already exists - return without creating coverfile...");
            //  return ;
            //}

            //MediaPortal.Services.IVideoThumbBlacklist blacklist = MediaPortal.Services.GlobalServiceProvider.Get<MediaPortal.Services.IVideoThumbBlacklist>();
            //if (blacklist != null && blacklist.Contains(fileName))
            //{
            //  LogMyFilms.Debug("Skipped creating thumbnail for {0}, it has been blacklisted because last attempt failed", fileName);
            //    return ;
            //}

            string ar = GetAspectRatio(fileName);
            int columns = 4;
            int rows = 2;
            if (string.IsNullOrEmpty(ar))
              LogMyFilms.Debug("MF: Failed getting aspectratio of movie");
            else
            {
              LogMyFilms.Debug("MF: GetAspectratio: ar = '" + ar.ToString() + "'");
            }
            //"fullscreen" : "widescreen"
            if (ar == "fullscreen")
            {
              columns = 2;
              rows = 4;
            }
            if (ar == "widescreen")
            {
              columns = 2;
              rows = 5;
            }
            LogMyFilms.Debug("MF: GetAspectratio: ar = '" + ar + "', columns = " + columns.ToString() + ", Rows = " + rows + ".");

            //System.Drawing.Image thumb = null;))))
            try
            {
                // CreateVideoThumb(string aVideoPath, string aThumbPath, bool aCacheThumb, bool aOmitCredits);
              bool success = VideoThumbCreator.CreateVideoThumb(fileName, tempImage, true, false, columns, rows, false, "Cover");
                if (!success)
                {
                  LogMyFilms.Debug("MF: (CreateThumbFromMovie): 'CreateVideoThumb' was NOT successful!");
                  return;
                }
                else
                {
                  LogMyFilms.Debug("MF: (CreateThumbFromMovie): 'CreateVideoThumb' was successful!");
                  //return;
                }
            }
            catch (System.Runtime.InteropServices.COMException comex)
            {
                if (comex.ErrorCode == unchecked((int)0x8004B200))
                {
                  LogMyFilms.Warn("Could not create thumbnail for {0} [Unknown error 0x8004B200]", fileName);
                }
                else
                {
                  LogMyFilms.Error("Could not create thumbnail for {0}", fileName);
                    LogMyFilms.Error(comex);
                }
            }
            catch (Exception ex)
            {
              LogMyFilms.Error("Could not create thumbnail for {0}", fileName);
                LogMyFilms.Error(ex);
            }
            finally
            {
                //if (!File.Exists(strThumb) && blacklist != null)
                //{
                //  blacklist.Add(fileName);
                //}
            }

            string cleanedtitle = GetSearchString(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString());
            LogMyFilms.Debug("MF: Clean title '" + MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() + "' to '" + cleanedtitle + "'");

            try
            {
              MyFilmsPlugin.MyFilms.Utils.ImageFast.CreateImage(
                tempImage.Substring(0, tempImage.LastIndexOf(".")) + "title.jpg", cleanedtitle, tempImage.Substring(0, tempImage.LastIndexOf(".")) + "L.jpg");
            }
            catch (Exception)
            {
              
              throw;
            }

            string newPicture = tempImage.Substring(0, tempImage.LastIndexOf(".") ) + "L.jpg";
            string oldPicture = MyFilmsDetail.getGUIProperty("picture");
            if (oldPicture.Length == 0 || oldPicture == null)
              oldPicture = newPicture;
            LogMyFilms.Debug("Picture Grabber options: Old temp Cover Image: '" + oldPicture.ToString() + "'");
            LogMyFilms.Debug("Picture Grabber options: New temp Cover Image: '" + newPicture.ToString() + "'");
            setGUIProperty("picture", newPicture);
            GUIWindowManager.Process(); // To Update GUI display ...

            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079870)); // choice
            dlgYesNo.SetLine(1, "");
            dlgYesNo.SetLine(2, GUILocalizeStrings.Get(10798733)); // Replace cover with new one
            dlgYesNo.SetLine(3, "");
            dlgYesNo.DoModal(GUIWindowManager.ActiveWindow); 
            //dlgYesNo.DoModal(GetID);
            if (!(dlgYesNo.IsConfirmed))
            {
                setGUIProperty("picture", oldPicture);
                GUIWindowManager.Process();
                return;
            }
            try
            {
              string newFinalPicture = strThumb;
              File.Copy(newPicture, newFinalPicture, true);
              File.Delete(newPicture);
              //MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newFinalPicture; // will be done below ...
              //setGUIProperty("picture", newFinalPicture);
              //GUIWindowManager.Process();
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug("Error copy file: '" + newPicture + "' - Exception: " + ex.ToString());
            }
            MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = strThumb;
            Update_XML_database();
            LogMyFilms.Info("MF: (Update_XML_database()) - Database Updated for created PictureThumb: " + strThumb);
        }

          
        private static string GetAspectRatio(string FileName)
        {
          try
          {
            MediaInfoWrapper minfo = new MediaInfoWrapper(FileName);

            LogMyFilms.Debug("MF: Mediainfo width: " + minfo.Width.ToString());
            LogMyFilms.Debug("MF: Mediainfo height: " + minfo.Height.ToString());
            //ToDo: Calculate aspect ratio here...

            return minfo.AspectRatio;
          }
          catch (Exception ex)
          {
            Log.Error("MF: GetAspectRatio: Error getting aspectratio via mediainfo.dll - Exception: " + ex.ToString());
          }
          return "";
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
            LogMyFilms.Debug("MyFilmsDetails (grabb_Nfo_Details) splits media directory name: '" + movieName.ToString() + "'");
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
        //  Dowload TMDBinfos (Poster(s), Movieinfos) on theMovieDB.org
        //-------------------------------------------------------------------------------------------        
        public static void Download_TMDB_Posters(string wtitle, string wttitle, string director, string year, bool choose, int wGetID, string savetitle)
        {

          //string tmpPicture = "";
          string tmpPicturename = ""; // picturename only
          string oldPicture = MyFilmsDetail.getGUIProperty("picture");
          string newPicture = ""; // full path to new picture
          string newPictureCatalogname = ""; // entry to be stored in catalog
          if (string.IsNullOrEmpty(oldPicture))
            oldPicture = "";

          if (MyFilms.conf.StrPicturePrefix.Length > 0)
            newPicture = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + tmpPicturename;
          else
            newPicture = MyFilms.conf.StrPathImg + "\\" + tmpPicturename;

          if (!System.IO.Directory.Exists(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix))
            try
            {
              System.IO.Directory.CreateDirectory(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix);
            }
            catch (Exception)
            {
            }
          
          Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
          string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
          int wyear = 0;
          try { wyear = Convert.ToInt32(year); }
          catch { }
          System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetTMDBinfos(wtitle, wttitle, wyear, director, MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix, true, choose, MyFilms.conf.StrTitle1, language);
          LogMyFilms.Debug("MF: (TMDB-Infos) - listemovies: '" + wtitle + "', '" + wttitle + "', '" + wyear + "', '" + director + "', '" + MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + "', 'true', '" + choose.ToString() + "', '" + MyFilms.conf.StrTitle1 + "', '" + language + "'");
          int listCount = listemovies.Count;
          LogMyFilms.Debug("MF: (TMDB-Infos) - listemovies: Result Listcount: '" + listCount + "'");

          if (choose)
            listCount = 2;
          switch (listCount)
          {
            case 0:
              break;
            case 1:
              LogMyFilms.Info("MF: Posters " + listemovies[0].Name.Substring(listemovies[0].Name.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);
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
              dlg.SetHeading(GUILocalizeStrings.Get(1079857)); // Load TMDB data (online)
              dlg.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
              foreach (DBMovieInfo t in listemovies)
              {
                dlg.Add(t.Name + "  (" + t.Year + ") - Posters: " + t.Posters.Count + " - Id" + t.Identifier);
                LogMyFilms.Debug("MF: TMDB listemovies: " + t.Name + "  (" + t.Year + ") - Posters: " + t.Posters.Count + " - TMDB-Id: " + t.Identifier);
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
                wotitle_sub_tableau = MyFilms.SubWordGrabbing(wtitle.ToString(), MinChars, Filter); // Min 3 Chars, Filter true (no der die das)
                wttitle_sub_tableau = MyFilms.SubWordGrabbing(wttitle.ToString(), MinChars, Filter); // Min 3 Chars, Filter true (no der die das)
                //First Show Dialog to choose Otitle, Ttitle or substrings - or Keyboard to manually enter searchstring!!!
                GUIDialogMenu dlgs = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlgs == null) return;
                dlgs.Reset();
                dlgs.SetHeading(GUILocalizeStrings.Get(1079859)); // choose search expression
                dlgs.Add("  *****  " + GUILocalizeStrings.Get(1079858) + "  *****  "); //manual selection with keyboard
                //dlgs.Add(wtitle); //Otitle
                dlgs.Add(savetitle); //Otitle = savetitle
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
                //dlgs.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
                if (!(dlgs.SelectedLabel > -1))
                {
                  dlgs.SelectedLabel = -1;
                  dlgs.DoModal(wGetID);
                }
                if (dlgs.SelectedLabel == 0)
                {
                  VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                  if (null == keyboard) return;
                  keyboard.Reset();
                  keyboard.Text = wtitle;
                  keyboard.DoModal(wGetID);
                  if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                  {
                    Download_TMDB_Posters(keyboard.Text, wttitle, string.Empty, string.Empty, true, wGetID, savetitle);
                  }
                  break;
                }
                if (dlgs.SelectedLabel > 0 && dlgs.SelectedLabel < 3) // if one of otitle or ttitle selected, keep year and director
                {
                  Download_TMDB_Posters(dlgs.SelectedLabelText, wttitle, year, director, true, wGetID, savetitle);
                  //Download_TMDB_Posters(string wtitle, string wttitle, string director, string year, bool choose,int wGetID, string savetitle)
                  break;
                }
                if (dlgs.SelectedLabel > 2) // For subitems, search without year and director !
                {
                  Download_TMDB_Posters(dlgs.SelectedLabelText, wttitle, string.Empty, string.Empty, true, wGetID, savetitle);
                  //Download_TMDB_Posters(string wtitle, string wttitle, string director, string year, bool choose,int wGetID, string savetitle)
                  break;
                }

              }
              if (dlg.SelectedLabel > 0)
              {
                bool first = true;
                string filename = string.Empty;
                string filename1 = string.Empty;
                if (MyFilms.conf.StrTitle1 == "OriginalTitle")
                  wttitle = savetitle; // Was wttitle = wtitle;
                foreach (string poster in listemovies[dlg.SelectedLabel - 1].Posters)
                {
                  filename1 = Grabber.GrabUtil.DownloadCovers(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix, poster, wttitle, true, first, out filename);
                  LogMyFilms.Info("MF: Poster " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);
                  if (first)
                    newPicture = filename1;
                  if (filename == string.Empty)
                    filename = filename1;
                  if (!(filename == "already" && filename1 == "already"))
                    filename = "added";
                  first = false;
                }
                foreach (string person in listemovies[dlg.SelectedLabel - 1].Actors)
                {
                  // ToDo: Load Actorinfodetails (API Call to be written in grabber) and download Person Artwork
                }
                listemovies[0].Name = filename;

                tmpPicturename = newPicture.Substring(newPicture.LastIndexOf("\\") + 1);
                if (MyFilms.conf.PictureHandling == "Relative Path" || string.IsNullOrEmpty(MyFilms.conf.PictureHandling))
                {
                  newPictureCatalogname = MyFilms.conf.StrPicturePrefix.Substring(0,MyFilms.conf.StrPicturePrefix.LastIndexOf("\\") + 1) + tmpPicturename;
                }
                if (MyFilms.conf.PictureHandling == "Full Path")
                {
                  newPictureCatalogname = newPicture;
                }
                if (!choose)
                {
                  MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
                  Update_XML_database();
                  LogMyFilms.Info("MF: (Update_XML_database()) - Database Updated for created Coverthumb: " + tmpPicturename);
                }
                else
                {
                  //TMDB_Details_Select();
                  setGUIProperty("picture", newPicture);
                  GUIWindowManager.Process(); // To Update GUI display ...

                  GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                  dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079870)); // choice
                  dlgYesNo.SetLine(1, "");
                  dlgYesNo.SetLine(2, GUILocalizeStrings.Get(10798733)); // Replace cover with new one
                  dlgYesNo.SetLine(3, "");
                  dlgYesNo.DoModal(wGetID);
                  if (!(dlgYesNo.IsConfirmed))
                  {
                    setGUIProperty("picture", oldPicture);
                    GUIWindowManager.Process();
                    return;
                  }
                  MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
                  Update_XML_database();
                  LogMyFilms.Info("MF: (Update_XML_database()) - Database Updated with '" + newPictureCatalogname + "' for created Coverthumb '" + tmpPicturename + "'");
                }
              }
              break;
          }
        }

        //private void CycleSeriesPoster(DBSeries series, bool next)
        //{
        //  if (series.PosterList.Count <= 1) return;

        //  int nCurrent = series.PosterList.IndexOf(series.Poster);

        //  if (next)
        //  {
        //    nCurrent++;
        //    if (nCurrent >= series.PosterList.Count)
        //      nCurrent = 0;
        //  }
        //  else
        //  {
        //    nCurrent--;
        //    if (nCurrent < 0)
        //      nCurrent = series.PosterList.Count - 1;
        //  }

        //  series.Poster = series.PosterList[nCurrent];
        //  series.Commit();

        //  // No need to re-load the facade for non-graphical layouts
        //  if (m_Facade.CurrentLayout == GUIFacadeControl.Layout.List)
        //    seriesposter.Filename = ImageAllocator.GetSeriesPosterAsFilename(series);
        //  else
        //    LoadFacade();
        //}


        //-------------------------------------------------------------------------------------------
        //  Change local Cover Image
        //-------------------------------------------------------------------------------------------        
        public static void ChangeLocalCover(DataRow[] r1, int Index, bool interactive)
        {
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
          //string movieName = "";
          string[] files = null;
          Int64 wsize = 0; // Temporary Filesize detection
          string startPattern = "";
          string currentPicture = MyFilmsDetail.getGUIProperty("picture");
          if (string.IsNullOrEmpty(currentPicture)) 
            return;
          string currentPictureName = currentPicture.Substring(currentPicture.LastIndexOf("\\") + 1);
          string currentStorePath = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix;
          string newPicture = ""; // full path to new picture
          string newPictureCatalogname = ""; // entry to be stored in catalog


          if (!currentPicture.StartsWith(currentStorePath)) 
            return;
          if (currentStorePath.EndsWith("\\"))
            startPattern = "";
          else 
            startPattern = currentPicture.Substring(currentPicture.LastIndexOf("\\") + 1);
          string searchPattern = currentPicture.Substring(currentStorePath.Length);

          int patternLength2 = 999;
          int patternLength = searchPattern.LastIndexOf(".");
          if (searchPattern.Contains("["))
            patternLength2 = searchPattern.LastIndexOf("[") - 1;
          if (patternLength2 < patternLength) 
            patternLength = patternLength2;
          searchPattern = searchPattern.Substring(0, patternLength);
          
          LogMyFilms.Debug("MFD: (ChangeLocalCover) - startPattern = '" + startPattern + "', searchPattern = '" + searchPattern + "', currentStorePath = '" + currentStorePath + "'");
          
          directoryname = currentPicture.Substring(0, currentPicture.LastIndexOf("\\"));

          int nCurrent = -1;
          if (!string.IsNullOrEmpty(directoryname))
          {
            files = Directory.GetFiles(directoryname, @"*" + searchPattern + @"*.jpg", SearchOption.TopDirectoryOnly);
            foreach (string filefound in files)
            {
                wsize = new System.IO.FileInfo(filefound).Length;
                result.Add(filefound);
                resultsize.Add(wsize);
                filesfound[filesfoundcounter] = filefound;
                filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                if (filefound.ToLower() == currentPicture.ToLower()) 
                  nCurrent = filesfoundcounter;
                filesfoundcounter = filesfoundcounter + 1;
            }
          }

          foreach (string n in filesfound)
          {
            if (!string.IsNullOrEmpty(n))
              LogMyFilms.Debug("MF: (Sorted coverfiles) ******* : '" + n + "'");
          }

          Array.Sort(filesfoundsize);
          for (int i = 0; i < result.Count; i++)
          {
            if (!string.IsNullOrEmpty(filesfound[i]))
              LogMyFilms.Debug("MF: (Sorted coverfiles) ******* : Number: '" + i + "' - Size: '" + filesfoundsize[i] + "' - Name: '" + filesfound[i] + "'");
          }

          if (result.Count > 0)
          {
            if (nCurrent == -1) 
              return;
            nCurrent++;
            if (nCurrent >= result.Count) 
              nCurrent = 0;
            newPicture = filesfound[nCurrent];

            string tmpPicturename = newPicture.Substring(newPicture.LastIndexOf("\\") + 1);
            if (MyFilms.conf.PictureHandling == "Relative Path" || string.IsNullOrEmpty(MyFilms.conf.PictureHandling))
            {
              newPictureCatalogname =
                MyFilms.conf.StrPicturePrefix.Substring(0, MyFilms.conf.StrPicturePrefix.LastIndexOf("\\") + 1) +
                tmpPicturename;
            }
            if (MyFilms.conf.PictureHandling == "Full Path")
            {
              newPictureCatalogname = newPicture;
            }
            if (!interactive)
            {
              // add confirmation dialog here....
            }
            else
            {
              MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
              LogMyFilms.Debug("MFD: (ChangeLocalCover) : NewCatalogName: '" + newPictureCatalogname + "'");
              Update_XML_database();
            }
          }
          else
            LogMyFilms.Debug("MyFilmsDetails (LocalCoverChange) - NO COVERS FOUND !!!!");
        }
        

      //-------------------------------------------------------------------------------------------
        //  Grab Internet Movie Details Informations and update the XML database and refresh screen
        //-------------------------------------------------------------------------------------------        
        public static void TMDB_Details_Select(string url, string moviehead, string wscript, int GetID, bool interactive, bool nfo, string nfofile)
        {
          //0  - "OriginalTitle", 
          //1  - "TranslatedTitle", 
          //2  - "Picture", 
          //3  - "Description", 
          //4  - "Rating", 
          //5  - "Actors", 
          //6  - "Director", 
          //7  - "Producer", 
          //8  - "Year", 
          //9  - "Country", 
          //10 - "Category", 
          //11 - "URL"
          //12 - "ImageURL"
          //13 - "MultipurposeURLlink"
          //14 - comment
          //15 - language
          //16 - tagline
          //17 - certification

          LogMyFilms.Debug("MF: launching (grabb_Internet_Details_Informations) with url = '" + url.ToString() + "', moviehead = '" + moviehead + "', wscript = '" + wscript + "', GetID = '" + GetID.ToString() + "', interactive = '" + interactive.ToString() + "'");
          Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
          string[] Result = new string[20];
          string title = string.Empty;
          string ttitle = string.Empty;
          string wtitle = string.Empty;
          int year = 0;
          string director = string.Empty;
          XmlConfig XmlConfig = new XmlConfig();
          // Those settings were used in the past from AMCupdater settings - now they exist in MF config as primary source!
          //string Img_Path = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Image_Download_Filename_Prefix", "");
          //string Img_Path_Type = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Store_Image_With_Relative_Path", "false");
          bool onlymissing = false;

          if (nfo)
            Result = Grab.GetNfoDetail(nfofile, MyFilms.conf.StrPathImg + MyFilms.conf.StrPicturePrefix, MyFilms.conf.StrPathArtist, "");
          else
          {
            string downLoadPath;
            if (interactive)
            {
              //downLoadPath = Config.GetDirectoryInfo(Config.Dir.Config) + @"\Thumbs\MyFilms";
              downLoadPath = Path.GetTempPath();
            }
            else
              downLoadPath = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix;
            Result = Grab.GetDetail(url, downLoadPath, wscript);
            LogMyFilms.Info("MF: Grabber - downloadpath = '" + downLoadPath + "'");
          }
          LogMyFilms.Info("MF: Grab Internet/nfo Information done for title/ttitle: " + MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] + "/" + MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString());

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

            string[] PropertyList = new string[] { "OriginalTitle", "TranslatedTitle", "Picture", "Description", "Rating", "Actors", "Director", "Producer", "Year", "Country", "Category", "URL", "ImageURL", "MultipurposeURLlink", "Comments", "Languages", "Tagline", "Certification" };
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

                if (wProperty != "ImageURL" && wProperty != "MultipurposeURLlink" && wProperty != "Tagline" && wProperty != "Certification")
                {
                  dlgmenu.Add(BaseMesFilms.Translate_Column(wProperty) + ": '" + strOldValue + "' -> '" + strNewValue + "'");
                  choiceViewMenu.Add(wProperty);
                  LogMyFilms.Debug("MF: GrabberUpdate - Add to menu (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                }
                else
                {
                  LogMyFilms.Debug("MF: GrabberUpdate - Not added to menu (unsupported) - (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                }
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
            LogMyFilms.Debug("MF: GrabInternetDetails - interactive choice: '" + strChoice + "'");
            if (strChoice == "missing")
              onlymissing = true;
          }


          switch (strChoice)
          {
            case "OriginalTitle":
              if (!string.IsNullOrEmpty(Result[0]))
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
              if (!string.IsNullOrEmpty(Result[1]))
              {
                ttitle = Result[1];
                if (string.IsNullOrEmpty(ttitle) && MyFilms.conf.StrTitle1 == "TranslatedTitle" && !string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString())) // Added to fill ttitle with otitle in case ttitle is empty and mastertitle = ttitle and mastertitle is empty
                  ttitle = Result[0].ToString();
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
              if (!string.IsNullOrEmpty(Result[2]))
              {
                string tmpPicture = Result[2];
                string tmpPicturename = ""; // picturename only
                string oldPicture = MyFilmsDetail.getGUIProperty("picture");
                string newPicture = ""; // full path to new picture
                string newPictureCatalogname = ""; // entry to be stored in catalog
                if (string.IsNullOrEmpty(oldPicture))
                  oldPicture = "";

                // set defaults...
                tmpPicturename = Result[2].Substring(Result[2].LastIndexOf("\\") + 1);
                newPictureCatalogname = tmpPicturename;

                if (MyFilms.conf.StrPicturePrefix.Length > 0)
                  newPicture = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + tmpPicturename;
                else
                  newPicture = MyFilms.conf.StrPathImg + "\\" + tmpPicturename;

                if (MyFilms.conf.PictureHandling == "Relative Path" || string.IsNullOrEmpty(MyFilms.conf.PictureHandling))
                {
                  newPictureCatalogname = MyFilms.conf.StrPicturePrefix + tmpPicturename;
                }
                if (MyFilms.conf.PictureHandling == "Full Path")
                {
                  newPictureCatalogname = newPicture;
                }

                LogMyFilms.Debug("Cover Image path : '" + MyFilms.conf.StrPathImg + "'");
                LogMyFilms.Debug("Picturehandling  : '" + MyFilms.conf.PictureHandling + "'");
                LogMyFilms.Debug("PicturePrefix    : '" + MyFilms.conf.StrPicturePrefix + "'");
                LogMyFilms.Debug("Old  Cover Image : '" + oldPicture + "'");
                LogMyFilms.Debug("Temp Cover Image : '" + tmpPicture + "'");
                LogMyFilms.Debug("New  Cover Image : '" + newPicture + "'");
                LogMyFilms.Debug("New Catalog Entry: '" + newPictureCatalogname + "'");

                setGUIProperty("picture", tmpPicture);
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
                if (!System.IO.Directory.Exists(newPicture.Substring(0, newPicture.LastIndexOf("\\"))))
                {
                  try
                  {
                    System.IO.Directory.CreateDirectory(newPicture.Substring(0, newPicture.LastIndexOf("\\")));
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.Debug("Could not create directory '" + newPicture.Substring(0, newPicture.LastIndexOf("\\")) + "' - Exception: " + ex.ToString());
                  }
                }
                try
                {
                  File.Copy(tmpPicture, newPicture, true);
                }
                catch (Exception ex)
                {
                  LogMyFilms.Debug("Error copy file: '" + tmpPicture + "' - Exception: " + ex.ToString());
                }
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
                try
                {
                  File.Delete(tmpPicture);
                }
                catch (Exception ex)
                {
                  LogMyFilms.Debug("Error deleting tmp file: '" + tmpPicture + "' - Exception: " + ex.ToString());
                }
              }
              break;
            case "Description":
              if (!string.IsNullOrEmpty(Result[3]))
                MyFilms.r[MyFilms.conf.StrIndex]["Description"] = Result[3].ToString();
              break;
            case "Rating":
              if (!string.IsNullOrEmpty(Result[4]))
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
              if (!string.IsNullOrEmpty(Result[5]))
                MyFilms.r[MyFilms.conf.StrIndex]["Actors"] = Result[5].ToString();
              break;
            case "Director":
              if (!string.IsNullOrEmpty(Result[6]))
              {
                director = Result[6].ToString();
                MyFilms.r[MyFilms.conf.StrIndex]["Director"] = Result[6].ToString();
              }
              break;
            case "Producer":
              if (!string.IsNullOrEmpty(Result[7]))
                MyFilms.r[MyFilms.conf.StrIndex]["Producer"] = Result[7].ToString();
              break;
            case "Year":
              if (!string.IsNullOrEmpty(Result[8]))
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
              if (!string.IsNullOrEmpty(Result[9]))
                MyFilms.r[MyFilms.conf.StrIndex]["Country"] = Result[9].ToString();
              break;
            case "Category":
              if (!string.IsNullOrEmpty(Result[10]))
                MyFilms.r[MyFilms.conf.StrIndex]["Category"] = Result[10].ToString();
              break;
            case "URL":
              if (!string.IsNullOrEmpty(Result[11]))
                if (MyFilms.conf.StrStorage != "URL")
                  MyFilms.r[MyFilms.conf.StrIndex]["URL"] = Result[11].ToString();
              break;
            case "Comments":
              if (!string.IsNullOrEmpty(Result[14]))
                MyFilms.r[MyFilms.conf.StrIndex]["Comments"] = Result[14].ToString();
              break;
            case "Languages":
              if (!string.IsNullOrEmpty(Result[15]))
                MyFilms.r[MyFilms.conf.StrIndex]["Languages"] = Result[15].ToString();
              break;
            case "all":
            case "missing":
              if (!string.IsNullOrEmpty(Result[0]))
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
              if (!string.IsNullOrEmpty(Result[1]))
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
              if (!string.IsNullOrEmpty(Result[2]))
              {
                string tmpPicture = Result[2];
                string tmpPicturename = ""; // picturename only
                string newPicture = ""; // full path to new picture
                string newPictureCatalogname = ""; // entry to be stored in catalog

                // set defaults...
                tmpPicturename = Result[2].Substring(Result[2].LastIndexOf("\\") + 1);
                newPictureCatalogname = tmpPicturename;

                if (MyFilms.conf.StrPicturePrefix.Length > 0)
                  newPicture = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + tmpPicturename;
                else
                  newPicture = MyFilms.conf.StrPathImg + "\\" + tmpPicturename;


                if (MyFilms.conf.PictureHandling == "Relative Path" || string.IsNullOrEmpty(MyFilms.conf.PictureHandling))
                {
                  newPictureCatalogname = MyFilms.conf.StrPicturePrefix + tmpPicturename;
                }
                if (MyFilms.conf.PictureHandling == "Full Path")
                {
                  newPictureCatalogname = newPicture;
                }

                if (newPicture != tmpPicture)
                {
                  if (!System.IO.Directory.Exists(newPicture.Substring(0, newPicture.LastIndexOf("\\"))))
                  {
                    try
                    {
                      System.IO.Directory.CreateDirectory(newPicture.Substring(0, newPicture.LastIndexOf("\\")));
                    }
                    catch (Exception ex)
                    {
                      LogMyFilms.Debug(
                        "Could not create directory '" + newPicture.Substring(0, newPicture.LastIndexOf("\\")) +
                        "' - Exception: " + ex.ToString());
                    }
                  }
                  try
                  {
                    File.Copy(tmpPicture, newPicture, true);
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.Debug("Error copy file: '" + tmpPicture + "' - Exception: " + ex.ToString());
                  }
                }
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
                if (newPicture != tmpPicture)
                {
                  try
                  {
                    File.Delete(tmpPicture);
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.Debug("Error deleting tmp file: '" + tmpPicture + "' - Exception: " + ex.ToString());
                  }
                }
              }

              if (!string.IsNullOrEmpty(Result[3]))
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Description"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Description"] = Result[3].ToString();
              if (!string.IsNullOrEmpty(Result[4]))
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
              if (!string.IsNullOrEmpty(Result[5]))
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Actors"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Actors"] = Result[5].ToString();
              if (!string.IsNullOrEmpty(Result[6]))
              {
                director = Result[6].ToString();
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Director"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Director"] = Result[6].ToString();
              }
              if (!string.IsNullOrEmpty(Result[7]))
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Producer"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Producer"] = Result[7].ToString();
              if (!string.IsNullOrEmpty(Result[8]))
              {
                try
                {
                  year = Convert.ToInt16(Result[8].ToString());
                }
                catch { }
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Year"] = Result[8].ToString();
              }
              if (!string.IsNullOrEmpty(Result[9]))
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Country"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Country"] = Result[9].ToString();
              if (!string.IsNullOrEmpty(Result[10]))
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Category"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Category"] = Result[10].ToString();
              if (!string.IsNullOrEmpty(Result[11]))
                if (MyFilms.conf.StrStorage != "URL")
                  if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["URL"].ToString()) || !onlymissing)
                    MyFilms.r[MyFilms.conf.StrIndex]["URL"] = Result[11].ToString();
              if (!string.IsNullOrEmpty(Result[14]))
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Comments"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Comments"] = Result[14].ToString();
              if (!string.IsNullOrEmpty(Result[15]))
                if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Languages"].ToString()) || !onlymissing)
                  MyFilms.r[MyFilms.conf.StrIndex]["Languages"] = Result[15].ToString();
              break;

            default:
              break;
          }
          Update_XML_database();
          LogMyFilms.Info("MF: Database Updated for title/ttitle: " + title + "/" + ttitle);
          if (title.Length > 0 && MyFilms.conf.StrFanart) // Get Fanart - ToDo Guzzi: Use local Fanart, if chosen ?
          {
            System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, (int)year, director, MyFilms.conf.StrPathFanart, true, false, MyFilms.conf.StrTitle1.ToString());
            //System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, (int)year, director, MyFilms.conf.StrPathFanart, true, false);
          }
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
        public static void Download_Backdrops_Fanart(string wtitle, string wttitle, string director, string year, bool choose,int wGetID, string savetitle, string personartworkpath)
        {
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            int wyear = 0;
            try {  wyear = Convert.ToInt32(year);}
            catch { }
            System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, director, MyFilms.conf.StrPathFanart, true, choose, MyFilms.conf.StrTitle1, personartworkpath);
            //System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, director, MyFilms.conf.StrPathFanart, true, choose);
            LogMyFilms.Debug("MF: (DownloadBackdrops) - listemovies: '" + wtitle + "', '" + wttitle + "', '" + wyear + "', '" + director + "', '" + MyFilms.conf.StrPathFanart + "', 'true', '" + choose.ToString() + "', '" + MyFilms.conf.StrTitle1 + "'");
            int listCount = listemovies.Count;
            LogMyFilms.Debug("MF: (DownloadBackdrops) - listemovies: Result Listcount: '" + listCount.ToString() + "'");

            if (choose)
                listCount = 2;
            switch (listCount)
            {
                case 0:
                    break;
                case 1:
                    LogMyFilms.Info("MF: Fanart " + listemovies[0].Name.Substring(listemovies[0].Name.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);
                    if (listemovies[0].Persons.Count > 0)
                      LogMyFilms.Info("MF: PersonArtwork: " + listemovies[0].Persons.Count.ToString() + " Personartwork checked for " + wttitle);
                    {
                      foreach (DBPersonInfo person in listemovies[0].Persons)
                      {
                        LogMyFilms.Info("MF: PersonArtwork: " + person.Images.Count.ToString() + " images downloaded for " + person.Name);
                      }
                    }
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
                    dlg.SetHeading(GUILocalizeStrings.Get(1079862)); // Load fanart (online)
                    dlg.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
                    foreach (DBMovieInfo t in listemovies)
                    {
                        dlg.Add(t.Name + "  (" + t.Year + ") - Fanarts: " + t.Backdrops.Count + " - Persons: " + t.Persons.Count.ToString());
                        LogMyFilms.Debug("MF: TMDB listemovies: " + t.Name + "  (" + t.Year + ") - Fanarts: " + t.Backdrops.Count + " - TMDB-Id: " + t.Identifier + " - Persons: " + t.Persons.Count.ToString());
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
                        dlgs.SetHeading(GUILocalizeStrings.Get(1079859)); // choose search expression
                        dlgs.Add("  *****  " + GUILocalizeStrings.Get(1079858) + "  *****  "); //manual selection with keyboard
                        //dlgs.Add(wtitle); //Otitle
                        dlgs.Add(savetitle); //Otitle = savetitle
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
                        //dlgs.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
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
                                Download_Backdrops_Fanart(keyboard.Text, wttitle, string.Empty, string.Empty, true, wGetID, savetitle, personartworkpath);
                            }
                            break;
                        }
                        if (dlgs.SelectedLabel > 0 && dlgs.SelectedLabel < 3) // if one of otitle or ttitle selected, keep year and director
                        {
                          Download_Backdrops_Fanart(dlgs.SelectedLabelText, wttitle, year, director, true, wGetID, savetitle, personartworkpath);
                            //Download_Backdrops_Fanart(string wtitle, string wttitle, string director, string year, bool choose,int wGetID, string savetitle)
                            break;
                        }
                        if (dlgs.SelectedLabel > 2) // For subitems, search without year and director !
                        {
                          Download_Backdrops_Fanart(dlgs.SelectedLabelText, wttitle, string.Empty, string.Empty, true, wGetID, savetitle, personartworkpath);
                          //Download_Backdrops_Fanart(string wtitle, string wttitle, string director, string year, bool choose,int wGetID, string savetitle)
                          break;
                        }

                    }
                    if (dlg.SelectedLabel > 0)
                    {
                        // Load Fanart  
                        bool first = true;
                        string filename = string.Empty;
                        string filename1 = string.Empty;
                        if (MyFilms.conf.StrTitle1 == "OriginalTitle")
                          wttitle = savetitle; // Was wttitle = wtitle;
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
                        // Download PersonArtwork
                        string filenameperson = string.Empty;
                        string filename1person = string.Empty;
                        string filename2person = string.Empty;
                        LogMyFilms.Info("MF: Person Artwork - " + listemovies[0].Persons.Count + " persons found - now loading artwork");
                        if (!string.IsNullOrEmpty(personartworkpath) && listemovies[0].Persons != null && listemovies[0].Persons.Count > 0)
                        {
                          List<grabber.DBPersonInfo> listepersons = listemovies[0].Persons;
                          foreach (grabber.DBPersonInfo person in listepersons)
                          {
                            bool firstpersonimage = true;
                            grabber.DBPersonInfo persondetails = new DBPersonInfo();
                            grabber.TheMoviedb TheMoviedb = new grabber.TheMoviedb();
                            persondetails = TheMoviedb.getPersonsById(person.Id, string.Empty);
                            LogMyFilms.Info(
                              "MF: Person Artwork - " + persondetails.Images.Count + " Images found for '" +
                              persondetails.Name + "'");
                            if (persondetails.Images.Count > 0)
                            {
                              foreach (var image in persondetails.Images)
                              {
                                filename1person = Grabber.GrabUtil.DownloadPersonArtwork(personartworkpath, image, persondetails.Name, true, firstpersonimage, out filenameperson);
                                LogMyFilms.Info("MF: Person Artwork " + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + " downloaded for '" + persondetails.Name + "' in movie '" + wttitle + "', path='" + filename1person + "'");
                                if (filenameperson == string.Empty) filenameperson = filename1person;
                                if (!(filenameperson == "already" && filename1person == "already")) filenameperson = "added";
                                firstpersonimage = false;
                              }
                            }
                            else
                            {
                              // Get further IMDB images
                              Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
                              Grabber.MyFilmsIMDB.IMDBUrl wurl;
                              _imdb.FindActor(persondetails.Name);
                              //Grabber.MyFilmsIMDBActor imdbActor = new Grabber.MyFilmsIMDBActor();
                              IMDBActor imdbActor = new IMDBActor();

                              if (_imdb.Count > 0)
                              {
                                string url = string.Empty;
                                wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
                                if (wurl.URL.Length != 0)
                                {
                                  url = wurl.URL;
                                  //url = wurl.URL + "videogallery"; // Assign proper Webpage for Actorinfos
                                  //url = MyFilms.ImdbBaseUrl + url.Substring(url.IndexOf("name"));
                                  Grabber.Grabber_URLClass fetchactor = new Grabber_URLClass();
                                  fetchactor.GetActorDetails(url, persondetails.Name, false, out imdbActor);
                                  filename1person = GrabUtil.DownloadPersonArtwork(personartworkpath, imdbActor.ThumbnailUrl, persondetails.Name, true, firstpersonimage, out filenameperson);
                                  LogMyFilms.Info("MF: Person Artwork " + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + " downloaded for '" + persondetails.Name + "' in movie '" + wttitle + "', path='" + filename1person + "'");
                                  // Add actor to datbbase to get infos in person facades later...
                                  int actorId = VideoDatabase.AddActor(imdbActor.Name);
                                  if (actorId > 0)
                                  {
                                    VideoDatabase.SetActorInfo(actorId, imdbActor);
                                    //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);

                                    if (imdbActor.ThumbnailUrl != string.Empty)
                                    {
                                      string largeCoverArt = Utils.GetLargeCoverArtName(
                                        Thumbs.MovieActors, imdbActor.Name);
                                      string coverArt = Utils.GetCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                                      Utils.FileDelete(largeCoverArt);
                                      Utils.FileDelete(coverArt);
                                      //DownloadCoverArt(Thumbs.MovieActors, imdbActor.ThumbnailUrl, imdbActor.Name);

                                    }
                                  }
                                }
                              }
                              else
                              {
                                int actorId = VideoDatabase.AddActor(imdbActor.Name);
                                imdbActor.Name = persondetails.Name;
                                //IMDBActor.IMDBActorMovie imdbActorMovie = new IMDBActor.IMDBActorMovie();
                                //imdbActorMovie.MovieTitle = _movieDetails.Title;
                                //imdbActorMovie.Year = _movieDetails.Year;
                                //imdbActorMovie.Role = role;
                                //imdbActor.Add(imdbActorMovie);
                                VideoDatabase.SetActorInfo(actorId, imdbActor);
                                //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);
                              }
                              firstpersonimage = false;

                              // Try to get actor images from IMDB
                              // Get further Actors from IMDB
                              //IMDBMovie MPmovie = new IMDBMovie();
                              //MPmovie.Title = listemovies[0].Name;
                              //MPmovie.IMDBNumber = listemovies[0].ImdbID;
                              //FetchActors(MPmovie, personartworkpath);
                              //Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
                              //Grabber.Grabber_URLClass.IMDBUrl wurl;
                              //_imdb.FindActor(person.Name);
                              //if (_imdb.Count > 0)
                              //{
                              //  wurl = (Grabber_URLClass.IMDBUrl)_imdb[0]; // Assume first match is the best !
                              //  if (wurl.IMDBURL.Length != 0)
                              //  {
                              //    IMDBActor imdbactor = new IMDBActor();
                              //    if (Grab.GetActorDetails(wurl, false, out imdbactor))
                              //    {
                              //      //Download Thumb
                              //    }
                              //    string url = imdbactor.ThumbnailUrl;
                              //  }
                              //}
                            }
                          }
                        }
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
                if (rep) // is group view 
                {
                  if ((group == "country" || group == "year" || group == "category") && MyFilms.conf.StrFanartDefaultViews) // Default views and fanart for group view enabled?
                    {
                        if (!System.IO.Directory.Exists(MyFilms.conf.StrPathFanart + "\\_Group"))
                            System.IO.Directory.CreateDirectory(MyFilms.conf.StrPathFanart + "\\_Group");
                        if (!System.IO.Directory.Exists(MyFilms.conf.StrPathFanart + "\\_Group\\" + group))
                            System.IO.Directory.CreateDirectory(MyFilms.conf.StrPathFanart + "\\_Group\\" + group);
                        safeName = MyFilms.conf.StrPathFanart + "\\_Group\\" + group + "\\{" + wtitle2 + "}";
                    }
                    else
                      if (MyFilms.conf.StrFanartDfltImageAll && (wfanart[0] == "" || wfanart[0] == " "))
                      {
                        wfanart[0] = MyFilms.conf.DefaultFanartImage;
                        wfanart[1] = "file";
                        return wfanart;
                      }
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
                // Added to support fanart for external catalogs
                switch (MyFilms.conf.StrFileType)
                {
                  case "5": // XMM
                    if (!string.IsNullOrEmpty(MyFilms.conf.StrPathFanart)) //Search matching files in XMM fanart directory
                    {
                      string searchname = HTMLParser.removeHtml(wtitle2).Replace(" ", "-"); // replaces special character "á" and other special chars !
                      searchname = searchname.Replace(" ", "-");
                      searchname = searchname.Replace(".", "-");
                      searchname = searchname.Replace("'", "-");
                      searchname = "*" + Regex.Replace(searchname, "[\n\r\t]", "-") + "_fanart*.jpg";
                      string[] files = Directory.GetFiles(MyFilms.conf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
                      LogMyFilms.Debug("MFD: Search_Fanart - XMM - search for '" + searchname + "'");
                      if (files.Count() > 0)
                      {
                        wfanart[0] = files[0];
                        wfanart[1] = "file";
                        return wfanart;
                      }
                    }
                    break;
                  case "4": // EAX MC 2.5.0
                  case "9": // EAX 3.x
                    if (!string.IsNullOrEmpty(MyFilms.conf.StrPathFanart)) //Search matching files in XMM fanart directory
                    {
                      string searchname = HTMLParser.removeHtml(wtitle2).Replace(" ", "."); // replaces special character "á" and other special chars !
                      //searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
                      searchname = searchname  + ".*.jpg";
                      string[] files = Directory.GetFiles(MyFilms.conf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
                      if (files.Count() > 0)
                      {
                        wfanart[0] = files[0];
                        wfanart[1] = "file";
                        return wfanart;
                      }
                    }
                    break;

                  case "10": // PVD artist thumbs: e.g. Natalie Portman_1.jpg , then Natalie Portman_2.jpg 
                    if (!string.IsNullOrEmpty(MyFilms.conf.StrPathFanart)) //Search matching files in XMM fanart directory
                    {
                      string searchname = HTMLParser.removeHtml(wtitle2); // replaces special character "á" and other special chars !
                      //searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
                      searchname = searchname + "*.jpg";
                      string[] files = Directory.GetFiles(MyFilms.conf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
                      if (files.Count() > 0)
                      {
                        wfanart[0] = files[0];
                        wfanart[1] = "file";
                        return wfanart;
                      }
                    }
                    break;

                  case "11": // MovingPicturesXML V1.2
                    if (!string.IsNullOrEmpty(MyFilms.conf.StrPathFanart)) //Search matching files in MoPi fanart directory
                    {
                      string searchname = HTMLParser.removeHtml(wtitle2); // replaces special character "á" and other special chars !
                      //searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
                      searchname = "{" + searchname + "}" + "*.jpg";
                      string[] files = Directory.GetFiles(MyFilms.conf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
                      if (files.Count() > 0)
                      {
                        wfanart[0] = files[0];
                        wfanart[1] = "file";
                        return wfanart;
                      }
                    }
                    break;

                  default:
                    break;
                }
                if ((MyFilms.conf.StrFanartDflt) && !(rep) && System.IO.File.Exists(filecover))
                {
                    wfanart[0] = filecover.ToString();
                    wfanart[1] = "file";
                    //Added Guzzi - Fix that no fanart was returned ...
                    return wfanart;
                }
                if (MyFilms.conf.StrFanartDfltImage && (wfanart[0] == "" || wfanart[0] == " "))
                {
                  wfanart[0] = MyFilms.conf.DefaultFanartImage;
                  wfanart[1] = "file";
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
                    setGUIProperty("currentfanart", wfanart[0].ToString()); 
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
            clearGUIProperty("user.item4.label");
            clearGUIProperty("user.item4.field");
            clearGUIProperty("user.item4.value");
            clearGUIProperty("user.item5.label");
            clearGUIProperty("user.item5.field");
            clearGUIProperty("user.item5.value");
            clearGUIProperty("user.source.value");
            clearGUIProperty("user.sourcetrailer.value");
            clearGUIProperty("user.sourcetrailer.count");
            clearGUIProperty("user.watched.value");
        }

        //-------------------------------------------------------------------------------------------
        //  Load detailed db fields : export fields to skin as '#myfilms.<ant db column name> 
        //-------------------------------------------------------------------------------------------
        public static void Load_Detailed_DB(int ItemId, bool wrep)
        {
            LogMyFilms.Debug("MFD: Load_Detailed_DB - ItemId: '" + ItemId.ToString() + "', Details (wrep): '" + wrep.ToString() + "'");
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
                    if (MyFilms.conf.Stritem4.ToLower() == (dc.ColumnName.ToLower()))
                      if (wrep)
                      {
                        setGUIProperty("user.item4.label", MyFilms.conf.Strlabel4);
                        if (MyFilms.conf.Stritem4.ToLower() == "date")
                          setGUIProperty("user.item4.field", "w" + MyFilms.conf.Stritem4.ToLower());
                        else
                          setGUIProperty("user.item4.field", MyFilms.conf.Stritem4.ToLower());
                        setGUIProperty("user.item4.value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                      }
                      else
                      {
                        clearGUIProperty("user.item4.label");
                        clearGUIProperty("user.item4.field");
                        clearGUIProperty("user.item4.value");
                      }
                    if (MyFilms.conf.Stritem5.ToLower() == (dc.ColumnName.ToLower()))
                      if (wrep)
                      {
                        setGUIProperty("user.item5.label", MyFilms.conf.Strlabel5);
                        if (MyFilms.conf.Stritem5.ToLower() == "date")
                          setGUIProperty("user.item5.field", "w" + MyFilms.conf.Stritem5.ToLower());
                        else
                          setGUIProperty("user.item5.field", MyFilms.conf.Stritem5.ToLower());
                        setGUIProperty("user.item5.value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                      }
                      else
                      {
                        clearGUIProperty("user.item5.label");
                        clearGUIProperty("user.item5.field");
                        clearGUIProperty("user.item5.value");
                      }
                    if (wrep && (MyFilms.conf.StrStorage.ToLower() == (dc.ColumnName.ToLower())))
                    {
                        setGUIProperty("user.source.value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                    }

                    if (wrep && (MyFilms.conf.StrStorageTrailer.ToLower() == (dc.ColumnName.ToLower())))
                    {
                      setGUIProperty("user.sourcetrailer.value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                      // add number of trailers : #myfilms.user.sourcetrailer.count
                      if (!string.IsNullOrEmpty(MyFilms.r[ItemId][dc.ColumnName].ToString().Trim()))
                      {
                        string[] split1 = MyFilms.r[ItemId][dc.ColumnName].ToString().Trim().Split(new Char[] { ';' });
                        setGUIProperty("user.sourcetrailer.count", split1.Count().ToString());
                      }
                      else
                        setGUIProperty("user.sourcetrailer.count", "");
                    }

                    if (wrep && (MyFilms.conf.StrWatchedField.ToLower() == (dc.ColumnName.ToLower())))
                    {
                      if (MyFilms.r[ItemId][dc.ColumnName].ToString().ToLower() != MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower())
                        setGUIProperty("user.watched.value", "true");
                      else
                        clearGUIProperty("user.watched.value"); // set to empty, if movie is unwatched
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
                        case "fanart":
                        case "imdb_id":
                        case "tmdb_id":
                        case "datewatched":
                        case "watched":
                            break;

                        case "isonline":
                            if (wrep && MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                            {
                              if (MyFilms.InitialIsOnlineScan)
                                MyFilmsDetail.setGUIProperty("user.source.isonline", MyFilms.r[ItemId][dc.ColumnName].ToString());
                              else
                                MyFilmsDetail.clearGUIProperty("user.source.isonline");
                            }
                            break;
                        case "isonlinetrailer":
                            if (wrep && MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                            {
                              if (MyFilms.InitialIsOnlineScan)
                                MyFilmsDetail.setGUIProperty("user.sourcetrailer.isonline", MyFilms.r[ItemId][dc.ColumnName].ToString());
                              else
                                MyFilmsDetail.clearGUIProperty("user.sourcetrailer.isonline");
                            }
                            break;
                        case "resolution":
                            decimal aspectratio = 0; 
                            string ar = "";
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

            //clearGUIProperty("db.description.value");
            LogMyFilms.Debug("MF: Load_Detailed_PersonInfo for '" + artistname + "'");
            ArrayList actorList = new ArrayList();
            MyFilmsDetail.GetActorByName(artistname, actorList);
            if (actorList.Count < 1 || actorList.Count > 5) // Do not proceed, of none or too many results !
            {
                return;
            }
            int actorID;
            string actorname = "";
            // Define splitter for string
            char[] splitter = { '|' };
            // Iterate through list
            int i = 0;
            foreach (string act in actorList)
            {
              string[] strActor = act.Split(splitter);
              actorID = Convert.ToInt32(strActor[0]);
              actorname = strActor[1];
              if ((actorID.ToString().Length > 0) && i == 0)
              {
                i = 1;
                LogMyFilms.Debug("MF: load details for actor: '" + actorID.ToString() + "'");
                try
                {
                  IMDBActor actor = VideoDatabase.GetActorInfo(actorID);
                  if (actor.Biography.Length > 0) setGUIProperty("db.description.value", actor.Biography);
                  if (actor.Name.Length > 0) setGUIProperty("user.mastertitle.value", actor.Name);
                  if (actor.Name.Length > 0) setGUIProperty("user.secondarytitle.value", actor.Name);
                  if (actor.PlaceOfBirth.Length > 0) setGUIProperty("db.category.value", actor.PlaceOfBirth);
                  if (actor.DateOfBirth.Length > 0) setGUIProperty("db.year.value", actor.DateOfBirth);
                }
                catch (Exception ex)
                {
                  LogMyFilms.Debug("MF: Exception while loading person details: " + ex.ToString());
                }
              }
            }
            //foreach (string act in actorList)
            //{
            //  string[] strActor = act.Split(splitter);
            //  actorID = Convert.ToInt32(strActor[0]);
            //  actorname = strActor[1];
            //  if (actorID.ToString().Length > 0)
            //  {
            //    LogMyFilms.Debug("MF: load details for actor: '" + actorID.ToString() + "'");
            //    try
            //    {
            //      IMDBActor actor = VideoDatabase.GetActorInfo(actorID);
            //      if (actor.Biography.Length > 0) setGUIProperty("db.description.value", actor.Biography);
            //      if (actor.Name.Length > 0) setGUIProperty("user.mastertitle.value", actor.Name);
            //      if (actor.Name.Length > 0) setGUIProperty("user.secondarytitle.value", actor.Name);
            //      if (actor.PlaceOfBirth.Length > 0) setGUIProperty("db.category.value", actor.PlaceOfBirth);
            //      if (actor.DateOfBirth.Length > 0) setGUIProperty("db.year.value", actor.DateOfBirth);
            //    }
            //    catch (Exception ex)
            //    {
            //      LogMyFilms.Debug("MF: Exception while loading person details: " + ex.ToString());
            //    }
            //  }
            //}
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

            if (MyFilms.conf.StrCheckWOLenable)
            {
              WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();
              int wTimeout = MyFilms.conf.StrWOLtimeout;
              bool isActive;
              string UNCpath = MyFilms.r[select_item][MyFilms.conf.StrStorage].ToString();
              string NasServerName;
              string NasMACAddress;

              if (UNCpath.StartsWith("\\\\"))
                UNCpath = (UNCpath.Substring(2, UNCpath.Substring(2).IndexOf("\\") + 0)).ToLower();
              if ((UNCpath.Equals(MyFilms.conf.StrNasName1, StringComparison.InvariantCultureIgnoreCase)) || (UNCpath.Equals(MyFilms.conf.StrNasName2, StringComparison.InvariantCultureIgnoreCase)) || (UNCpath.Equals(MyFilms.conf.StrNasName3, StringComparison.InvariantCultureIgnoreCase)))
              {
                  if (WakeOnLanManager.Ping(UNCpath, wTimeout))
                      isActive = true;
                  else
                      isActive = false;

                  if (!isActive) // Only if NAS server is not yet already rzunning !
                  {
                    if (MyFilms.conf.StrCheckWOLuserdialog)
                    {
                      GUIDialogYesNo dlgOknas = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                      dlgOknas.SetHeading(GUILocalizeStrings.Get(107986)); //my films
                      dlgOknas.SetLine(1, "Film    : '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle.ToString()] + "'"); //video title
                      dlgOknas.SetLine(2, "Server  : '" + UNCpath + "'");
                      dlgOknas.SetLine(3, "Status  : '" + GUILocalizeStrings.Get(10798742)); // srv name + " - (offline) - start ?"
                      dlgOknas.DoModal(GetID);
                      if (!(dlgOknas.IsConfirmed)) return;
                    }

                    // Search the NAS where movie is located:
                      if ((UNCpath.Equals(MyFilms.conf.StrNasName1, StringComparison.InvariantCultureIgnoreCase)) && (MyFilms.conf.StrNasMAC1.ToString().Length > 1))
                      {
                        NasServerName = MyFilms.conf.StrNasName1;
                        NasMACAddress = MyFilms.conf.StrNasMAC1;
                      }
                      else if ((UNCpath.Equals(MyFilms.conf.StrNasName2, StringComparison.InvariantCultureIgnoreCase)) && (MyFilms.conf.StrNasMAC2.ToString().Length > 1))
                      {
                        NasServerName = MyFilms.conf.StrNasName2;
                        NasMACAddress = MyFilms.conf.StrNasMAC2;
                      }
                      else if ((UNCpath.Equals(MyFilms.conf.StrNasName3, StringComparison.InvariantCultureIgnoreCase)) && (MyFilms.conf.StrNasMAC3.ToString().Length > 1))
                      {
                        NasServerName = MyFilms.conf.StrNasName3;
                        NasMACAddress = MyFilms.conf.StrNasMAC3;
                      }
                      else
                        {
                          NasServerName = String.Empty;
                          NasMACAddress = String.Empty;
                        }

                      // Start NAS Server

                      bool SuccessFulStart = wakeOnLanManager.WakeupSystem(wakeOnLanManager.GetHwAddrBytes(NasMACAddress), NasServerName, wTimeout);

                      if (MyFilms.conf.StrCheckWOLuserdialog)
                      {
                        GUIDialogOK dlgOk =
                          (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));
                        dlgOk.SetLine(1, "");
                        if (SuccessFulStart)
                          dlgOk.SetLine(2, "'" + NasServerName + "' " + GUILocalizeStrings.Get(10798743)); //successfully started 
                        else 
                          dlgOk.SetLine(2, "'" + NasServerName + "' " + GUILocalizeStrings.Get(10798744)); // could not be started 
                        dlgOk.DoModal(GetID);
                      }
                  }
              }
              else
              {
                  GUIDialogOK dlgOknas = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                  dlgOknas.SetHeading(GUILocalizeStrings.Get(107986)); //my films
                  //dlgOknas.SetLine(1, "Movie   : '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle.ToString()].ToString() + "'"); //video title
                  dlgOknas.SetLine(2, "Server '" + UNCpath + "' " + GUILocalizeStrings.Get(10798746)); //is not configured for WakeOnLan ! 
                  dlgOknas.SetLine(3, GUILocalizeStrings.Get(10798747)); // Automatic NAS start not possible ... 
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
                // Check, if the content returned is a BR playlist to supress internal player and dialogs
                bool isBRcontent = false;
                if (newItems[0].ToString().ToLower().Contains("bdmv")) 
                  isBRcontent = true;

                if (!isBRcontent)
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
                GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                dlgmenu.Reset();
                dlgmenu.SetHeading(GUILocalizeStrings.Get(10798704)); // Trailer ...
                dlgmenu.Add(GUILocalizeStrings.Get(10798740)); // play all trailers 

                foreach (object t in newItems)
                {
                  string movieFileName = (string)t;
                  Int64 wsize = 0;
                  if (System.IO.File.Exists(movieFileName))
                    wsize = new System.IO.FileInfo(movieFileName).Length;
                  else wsize = 0;
                  string wsizeformatted = string.Format("{0} MB", wsize / 1048576);
                  if (movieFileName.Contains("\\"))
                    dlgmenu.Add(movieFileName.Substring(movieFileName.LastIndexOf("\\") + 1) + " (" + wsizeformatted + ")"); // add moviename to menu
                  else
                    dlgmenu.Add(movieFileName + " (" + wsizeformatted + ")"); // add moviename to menu
                }

                dlgmenu.DoModal(GetID);
                if (dlgmenu.SelectedLabel == -1) return;
                //if (dlgmenu.SelectedLabel > 0) // 0 = Play all trailers - so >0 -> a specific choice has been taken ...
                  //IMovieIndex = dlgmenu.SelectedId; // is "1" - as "0" is used for play all movies ...
                IMovieIndex = dlgmenu.SelectedLabel;

                //GUIDialogFileStacking dlg = (GUIDialogFileStacking)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_FILESTACKING);
                //if (null != dlg)
                //{
                //    dlg.SetNumberOfFiles(newItems.Count);
                //    dlg.DoModal(GUIWindowManager.ActiveWindow);
                //    int selectedFileIndex = dlg.SelectedFile;
                //    if (selectedFileIndex < 1) return;
                //    IMovieIndex = selectedFileIndex++;
                //}
            }
            if (newItems.Count > 0)
            {
                playlistPlayer.Reset();
                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_VIDEO_TEMP;
                PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP);
                playlist.Clear();

                if (IMovieIndex > 0)
                {
                  string movieFileName = (string)newItems[IMovieIndex - 1]; // as in menu there is "all-option" as first index 0 ...
                  PlayListItem newitem = new PlayListItem();
                  newitem.FileName = movieFileName;
                  LogMyFilms.Info("MF: Play specific movie trailer: '" + movieFileName + "'");
                  newitem.Type = PlayListItem.PlayListItemType.Video;
                  playlist.Add(newitem);
                }
                else // if play all trailers is chosen add all available trailers to playlist
                {
                  foreach (object t in newItems)
                  {
                    string movieFileName = (string)t;
                    PlayListItem newitem = new PlayListItem();
                    newitem.FileName = movieFileName;
                    LogMyFilms.Info("MF: Add trailer to playlist: '" + movieFileName + "'");
                    newitem.Type = PlayListItem.PlayListItemType.Video;
                    playlist.Add(newitem);
                  }
                }

                // ask for start movie Index
                
                // play movie...
                //PlayMovieFromPlayListTrailer(NoResumeMovie, IMovieIndex - 1);
                PlayMovieFromPlayListTrailer(NoResumeMovie, 0);
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
                dlgmenu.Add(GUILocalizeStrings.Get(10798712)); //search apple itunes trailer with onlinevideos 10798716
                choiceViewMenu.Add("playtraileronlinevideosappleitunes");
                dlgmenu.Add(GUILocalizeStrings.Get(10798716)); //search IMDB trailer with onlinevideos
                choiceViewMenu.Add("playtraileronlinevideosimdbtrailer");
                dlgmenu.Add(GUILocalizeStrings.Get(712) + " ..."); //Return ...
                choiceViewMenu.Add("return");

                dlgmenu.DoModal(GetID);
                if (dlgmenu.SelectedLabel == -1) return;

                string site = string.Empty;
                string titleextension = string.Empty;
                switch (choiceViewMenu[dlgmenu.SelectedLabel].ToLower())
                {
                  case "playtraileronlinevideos":
                    site = "YouTube";
                    titleextension = " " + MyFilms.r[MyFilms.conf.StrIndex]["Year"] + " trailer";
                    break;
                  case "playtraileronlinevideosappleitunes":
                    site = "iTunes Movie Trailers";
                    break;
                  case "playtraileronlinevideosimdbtrailer":
                    site = "IMDb Movie Trailers";
                    break;
                  default:
                    return;
                }
                // Load OnlineVideo Plugin with Searchparameters for YouTube and movie to Search ... OV reference for parameters: site:<sitename>|category:<categoryname>|search:<searchstring>|VKonfail:<true,false>|return:<Locked,Root>
                //if (PluginManager.IsPluginNameEnabled2("OnlineVideos"))
                if (MyFilms.OnlineVideosRightPlugin && MyFilms.OnlineVideosRightVersion)
                {
                  string title = string.Empty;
                  if (!string.IsNullOrEmpty(MyFilms.conf.ItemSearchGrabber) && !string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.ItemSearchGrabber].ToString()))
                    title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.ItemSearchGrabber].ToString(); // Configured GrabberTitle
                  else if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString())) // Mastertitle
                    title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
                  else if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString())) // Secondary title
                    title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString();
                  else if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString())) // Name from source (media)
                  {
                    title = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
                    if (title.Contains(";")) title = title.Substring(0, title.IndexOf(";"));
                    if (title.Contains("\\")) title = title.Substring(title.LastIndexOf("\\") + 1);
                    if (title.Contains(".")) title = title.Substring(0, title.LastIndexOf("."));
                  }
                  if (title.IndexOf(MyFilms.conf.TitleDelim) > 0)
                    title = title.Substring(title.IndexOf(MyFilms.conf.TitleDelim) + 1);

                  string OVstartparams = "site:" + site + "|category:|search:" + title + titleextension + "|return:Locked";
                  //GUIPropertyManager.SetProperty("Onlinevideos.startparams", OVstartparams);
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", site);
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", title + titleextension);
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "Locked");
                  LogMyFilms.Debug("MF: Starting OnlineVideos with '" + OVstartparams.ToString() + "'");
                  // should this be set here to make original movie doesn't get set to watched??
                  // trailerPlayed = true;
                  GUIWindowManager.ActivateWindow(MyFilms.ID_OnlineVideos, false); // 4755 is ID for OnlineVideos
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");
                }
                else
                {
                  //ShowMessageDialog("MyFilms", "OnlineVideo plugin not installed or wrong version", "Minimum Version required: 0.28");
                  GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                  if (dlgOK != null)
                  {
                    dlgOK.SetHeading("MyFilms");
                    dlgOK.SetLine(1, "OnlineVideo plugin not installed or wrong version");
                    dlgOK.SetLine(2, "Minimum Version required: 0.28");
                    dlgOK.DoModal(GetID);
                    return;
                  }
                }
              }
            }
        }

        public static void Launch_Movie_Trailer_Streams(int select_item, int GetID, GUIAnimation m_SearchAnimation)
        //-------------------------------------------------------------------------------------------
        // Play Movie Trailer Links (streaming) !!!
        //-------------------------------------------------------------------------------------------
        {  // ToDo: Add methods for Streaming - check OV for howto's...
          LogMyFilms.Debug("MF: (Play Movie Trailer Streams) select_item = '" + select_item + "' - GetID = '" + GetID + "' - m_SearchAnimation = '" + m_SearchAnimation + "'");
          if ((MyFilms.conf.CmdPar.Length > 0) && (MyFilms.conf.CmdPar != "(none)"))
            RunProgram(MyFilms.conf.CmdExe, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.CmdPar].ToString());
          if (g_Player.Playing)
            g_Player.Stop();
          // search all files
          ArrayList newItems = new ArrayList();
          bool NoResumeMovie = true; //Modded by Guzzi for NonResuming Trailers 
          int IMovieIndex = 0;
          LogMyFilms.Debug("MyFilmsDetails (Launch_Movie_Trailer): new do Moviesearch with '" + select_item + "' (Selected_Item");
          if (newItems.Count > 0)
          {
            playlistPlayer.Reset();
            playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_VIDEO_TEMP;
            PlayList playlist = playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP);
            playlist.Clear();

            if (IMovieIndex > 0)
            {
              string movieFileName = (string)newItems[IMovieIndex - 1]; // as in menu there is "all-option" as first index 0 ...
              PlayListItem newitem = new PlayListItem();
              newitem.FileName = movieFileName;
              LogMyFilms.Info("MF: Play specific movie trailer: '" + movieFileName + "'");
              newitem.Type = PlayListItem.PlayListItemType.Video;
              playlist.Add(newitem);
            }
            else // if play all trailers is chosen add all available trailers to playlist
            {
              foreach (object t in newItems)
              {
                string movieFileName = (string)t;
                PlayListItem newitem = new PlayListItem();
                newitem.FileName = movieFileName;
                LogMyFilms.Info("MF: Add trailer to playlist: '" + movieFileName + "'");
                newitem.Type = PlayListItem.PlayListItemType.Video;
                playlist.Add(newitem);
              }
            }
            PlayMovieFromPlayListTrailer(NoResumeMovie, 0);
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
            LogMyFilms.Debug("MFD: OnPlayBackStarted was initiated");

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

                //GUIVideoFiles.Reset(); // reset pincode
          
                //ArrayList files = new ArrayList();
                //VideoDatabase.GetFiles(movie.ID, ref files);
                
                //if (files.Count > 1)
                //{
                //  GUIVideoFiles._stackedMovieFiles = files;
                //  GUIVideoFiles._isStacked = true;
                //  GUIVideoFiles.MovieDuration(files);
                //}
                //else
                //{
                //  GUIVideoFiles._isStacked = false;
                //}
                //GUIVideoFiles.PlayMovie(movie.ID);

            }
          // Might require delay to wait until OSD is first (auto)updated from MyVideo database - we might want to override this after ...  
          //GUIPropertyManager.SetProperty("#Play.Current.Thumb", clear ? " " : osdImage);
          // Check, if property for OSD should be set (came from myvideo DB in the past) -> #Play.Current.Thumb
        }
        private void OnPlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
          LogMyFilms.Debug("MFD: OnPlayBackEnded was initiated");
          UpdateOnPlayEnd(type, 0, filename, true, false);
        }
        private void OnPlayBackStopped(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename)
        {
          LogMyFilms.Debug("MFD: OnPlayBackStopped was initiated");
          UpdateOnPlayEnd(type, timeMovieStopped, filename, false, true);
        }
        private void UpdateOnPlayEnd(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename, bool ended, bool stopped)
        {
            LogMyFilms.Debug("MFD: UpdateOnPlayEnd was initiated - trailerPlayed = '" + trailerPlayed + "'");

            if (MyFilms.conf.StrPlayedIndex == -1)
                return;
            if (type != g_Player.MediaType.Video || filename.EndsWith("&txe=.wmv"))
              return;
            if (handleTrailer())
              return;

            //if (isTrailer)
            //{
            //  LogMyFilms.Debug("MFD: Skipping UpdateOnEnd - reason: isTrailer");
            //  isTrailer = false;
            //  return;
            //}
            try
            {
                DataRow[] r1 = BaseMesFilms.LectureDonnées(MyFilms.conf.StrPlayedDfltSelect, MyFilms.conf.StrPlayedSelect, MyFilms.conf.StrPlayedSort, MyFilms.conf.StrPlayedSens);
                // Handle all movie files from idMovie
                ArrayList movies = new ArrayList();
                int playTimePercentage = 0; // Set watched flag after 80% of total played time
                double TotalRuntimeMovie = 0;

                int iidMovie = VideoDatabase.GetMovieId(filename);
                if (iidMovie >= 0)
                {
                    VideoDatabase.GetFiles(iidMovie, ref movies);
                    //HashSet<string> watchedMovies = new HashSet<string>();
                  
                    //// Stacked movies duration -- taken from Deda, MyVideos
                    //if (_isStacked && _totalMovieDuration != 0)
                    //{
                    //  int duration = 0;

                    //  for (int i = 0; i < _stackedMovieFiles.Count; i++)
                    //  {
                    //    int fileID = VideoDatabase.GetFileId((string)_stackedMovieFiles[i]);

                    //    if (g_Player.CurrentFile != (string)_stackedMovieFiles[i])
                    //    {
                    //      //(int)Math.Ceiling((timeMovieStopped / g_Player.Player.Duration) * 100);
                    //      duration += VideoDatabase.GetMovieDuration(fileID);
                    //      continue;
                    //    }
                    //    playTimePercentage = (100 * (duration + timeMovieStopped) / _totalMovieDuration);
                    //    break;
                    //  }
                    //}
                    //else
                    //{
                    //  if (g_Player.Player.Duration >= 1)
                    //    playTimePercentage = (int)Math.Ceiling((timeMovieStopped / g_Player.Player.Duration) * 100);
                    //}

                    //try
                    //{
                    //  //if (file != string.Empty)
                    //  //{
                    //  //  // Set new data
                    //  //  MediaInfoWrapper x = new MediaInfoWrapper(file);
                    //  //  x.VideoDuration
                    //  //  //GUIPropertyManager.SetProperty("#VideoCodec", Util.Utils.MakeFileName(x.VideoCodec));
                    //  //  //GUIPropertyManager.SetProperty("#VideoResolution", x.VideoResolution);
                    //  //  //GUIPropertyManager.SetProperty("#AudioCodec", Util.Utils.MakeFileName(x.AudioCodec));
                    //  //  //GUIPropertyManager.SetProperty("#AudioChannels", x.AudioChannelsFriendly);
                    //  //  //GUIPropertyManager.SetProperty("#HasSubtitles", x.HasSubtitles.ToString());
                    //  //  //GUIPropertyManager.SetProperty("#AspectRatio", x.AspectRatio);
                    //  //}
                    //}
                    //catch (Exception)
                    //{
                      
                    //  throw;
                    //}
                    //if (movies.Count > 0)
                    //{
                    //  foreach (IMDBMovie movie in movies) // Get Total movie length
                    //  {
                    //    LogMyFilms.Debug("MFD: Partial Movie Runtime = '" + movie.RunTime.ToString() + "'");
                    //    if (movie.RunTime > 0)
                    //      TotalRuntimeMovie += movie.RunTime;
                    //  }
                    //  LogMyFilms.Debug("MFD: TotalRuntimeMovie = '" + TotalRuntimeMovie.ToString() + "'");
                    //}

                    if (g_Player.Player.Duration >= 1)
                    {
                      LogMyFilms.Debug("MFD: TotalRuntimeMovie = '" + TotalRuntimeMovie.ToString() + "', g_player.Player.Duration = '" + g_Player.Player.Duration.ToString() + "'");
                      string runtimeFromDb = r1[MyFilms.conf.StrPlayedIndex]["Length"].ToString();
                      try
                      {
                        TotalRuntimeMovie = 60 * double.Parse(runtimeFromDb);
                      }
                      catch (Exception)
                      {
                        TotalRuntimeMovie = 0;
                      }
                      LogMyFilms.Debug("MFD: TotalRuntimeMovie = '" + TotalRuntimeMovie.ToString() + "', Runtime from DB = '" + runtimeFromDb + "'");

                      if (TotalRuntimeMovie > g_Player.Player.Duration)
                        playTimePercentage = (int)Math.Ceiling((timeMovieStopped / TotalRuntimeMovie) * 100);
                      else
                        playTimePercentage = (int)Math.Ceiling((timeMovieStopped / g_Player.Player.Duration) * 100);
                      LogMyFilms.Debug("MFD: Calculated playtimepercentage: '" + playTimePercentage + "' - g_player.Duration: '" + g_Player.Duration.ToString() + "' - playlistPlayer.g_Player.Duration: '" + playlistPlayer.g_Player.Duration.ToString() + "'");
                    }

                    if (movies.Count <= 0)
                          return;
                    for (int i = 0; i < movies.Count; i++)
                    {
                      string strFilePath = (string)movies[i];
                      int idFile = VideoDatabase.GetFileId(strFilePath);
                      byte[] resumeData = null;
                      if (idFile < 0) 
                        break;
                      if (g_Player.IsDVDMenu)
                        {
                          VideoDatabase.SetMovieStopTimeAndResumeData(idFile, 0, null);
                          //watchedMovies.Add(strFilePath);
                        }

                      else if ((filename.Trim().ToLower().Equals(strFilePath.Trim().ToLower())) && (timeMovieStopped > 0))
                        {
                        g_Player.Player.GetResumeState(out resumeData);
                        LogMyFilms.Info("GUIVideoFiles: {0} idFile={1} timeMovieStopped={2} resumeData={3}", "MyFilms", idFile, timeMovieStopped, resumeData);
                        VideoDatabase.SetMovieStopTimeAndResumeData(idFile, timeMovieStopped, resumeData);
                        LogMyFilms.Debug("GUIVideoFiles: {0} store resume time", "MyFilms");

                        //Set file "watched" only if 80% or higher played time (share view)
                        if (playTimePercentage >= 80)
                          {
                            //watchedMovies.Add(strFilePath);
                          }
                        }
                      else
                      {
                        VideoDatabase.DeleteMovieStopTime(idFile);
                      }
                      if (ended)
                        {
                            // Set resumedata to zero
                            VideoDatabase.GetMovieStopTimeAndResumeData(idFile, out resumeData);
                            VideoDatabase.SetMovieStopTimeAndResumeData(idFile, 0, resumeData);
                            LogMyFilms.Info("MF: GUIVideoFiles: OnPlayBackEnded idFile={0} resumeData={1}", idFile, resumeData);
                        }
                      else
                        {
                            // ToDo: Activate and modify code to set watched earlier than "ended" ...
                            // Watched status for videos not in the movie database (sample code from MyVideos)
                            //int playTimePercentage = 0; // Set watched flag after 80% of total played time
                            //if (g_Player.Player.Duration >= 1)
                            //  playTimePercentage = (int)Math.Ceiling((timeMovieStopped / g_Player.Player.Duration) * 100);

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
                    if (ended || (stopped && MyFilms.conf.CheckWatchedPlayerStopped))
                    {
                      // Update db view watched status for played movie
                      IMDBMovie details = new IMDBMovie();
                      VideoDatabase.GetMovieInfo(filename, ref details);
                      //VideoDatabase.GetMovieInfoById(iidMovie, ref details);
                      if (!details.IsEmpty && (playTimePercentage >= 80 || g_Player.IsDVDMenu)) //Flag movie "watched" status only if 80% or higher played time (database view)
                      {
                        details.Watched = 1;
                        //details.Watched++;
                        VideoDatabase.SetWatched(details);
                        //VideoDatabase.SetMovieInfoById(details.ID, ref details);
                      }

                    }
                }
                
                if (trailerPlayed)
                {
                  LogMyFilms.Debug("MFD: Skipping UpdateOnEnd - reason: trailerPlayed = true");
                  trailerPlayed = false;
                  return;
                }
                
                if (MyFilms.conf.CheckWatched)
                {
                  r1[MyFilms.conf.StrPlayedIndex][MyFilms.conf.StrWatchedField] = "True";
                  LogMyFilms.Debug("MFD: Movie set to watched - reason: ended = " + ended + ", stopped = " + stopped + ", playTimePercentage = '" + playTimePercentage + "'" + ", 'update on movie start'");
                }
                if (ended)
                {
                  if (MyFilms.conf.StrSupPlayer)
                    Suppress_Entry(r1, MyFilms.conf.StrPlayedIndex);
                }
                if (ended && MyFilms.conf.CheckWatchedPlayerStopped)
                {
                  r1[MyFilms.conf.StrPlayedIndex][MyFilms.conf.StrWatchedField] = "True";
                  LogMyFilms.Debug("MFD: Movie set to watched - reason: ended = " + ended + ", stopped = " + stopped + ", playTimePercentage = '" + playTimePercentage + "'" + ", 'update on movie end'");
                }
                if (stopped && MyFilms.conf.CheckWatchedPlayerStopped && playTimePercentage >= 80)
                {
                  r1[MyFilms.conf.StrPlayedIndex][MyFilms.conf.StrWatchedField] = "True";
                  LogMyFilms.Debug("MFD: Movie set to watched - reason: ended = " + ended + ", stopped = " + stopped + ", playTimePercentage = '" + playTimePercentage + "'" + ", 'update on movie end'");
                }

                if ((MyFilms.conf.CheckWatched) || (MyFilms.conf.CheckWatchedPlayerStopped) || (MyFilms.conf.StrSupPlayer))
                {
                  //Tried, but didn't Help...
                  //if (ImgDetFilm != null)
                  //if (ImgDetFilm.IsVisible)
                  //  ImgDetFilm.Refresh();
                  //else if (ImgDetFilm2 != null)
                  //  ImgDetFilm2.Refresh();

                  Update_XML_database();
                  afficher_detail(true);
                  //GUIWindowManager.Process(); // Enabling creates lock in handler !!!
                }
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
            //#region Trakt
            //// submit watched state to trakt API
            //PlayListItem item = GetCurrentItem();

            //if (item != null)
            //{
            //      TraktScrobbleUpdater.RunWorkerAsync(item.Episode);
            //}
            //#endregion
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

                        if (System.IO.File.Exists(wfile3 + @"\BDMV\index.bdmv")) // check for bluray in iso ...
                        {
                          if (delete)
                          {
                            delete_movie(wfile3 + @"\BDMV\index.bdmv", ref newItems);
                            Search_parts(wfile3, select_item, delete, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
                            continue;
                          }
                          else
                          {
                            IdMovie = -1;
                            add_update_movie(wfile3 + @"\BDMV\index.bdmv", select_item, ref NoResumeMovie, ref newItems, ref IdMovie, ref IMovieIndex, ref timeMovieStopped);
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
            trailerPlayed = true;
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
            catch (Exception ex)
            {
              LogMyFilms.Error("MFD: setProcessAnimationStatus - Exception: " + ex.StackTrace.ToString());
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
            LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - mastertitle      : '" + MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString() + "'");
            //Title2 = Translated Movietitle
            LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - secondary title  : '" + MyFilms.r[Index][MyFilms.conf.StrTitle2].ToString() + "'");
            //Cleaned Title
            LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - Cleaned Title    : '" + MediaPortal.Util.Utils.FilterFileName(MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString().ToLower()) + "'");            
            //Index of facadeview?
            LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) - Index            : '" + Index + "'");
            //Full Path to Film
            LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) - FullMediasource  : '" + (string)MyFilms.r[Index][MyFilms.conf.StrStorage].ToString().Trim() + "'");

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
            string[] files = null;
            Int64 wsize = 0; // Temporary Filesize detection
            // split searchpath information delimited by semicolumn (multiple searchpathes from config)
            string[] Trailerdirectories = MyFilms.conf.StrDirStorTrailer.ToString().Split(new Char[] { ';' });
            LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) Extended Search '" + ExtendedSearch + "' for movie '" + file + "' in search directories: '" + MyFilms.conf.StrDirStorTrailer + "'");

            //Retrieve original directory of mediafiles
            //directoryname
            movieName = (string)MyFilms.r[Index][MyFilms.conf.StrStorage].ToString().Trim();
            movieName = movieName.Substring(movieName.LastIndexOf(";") + 1);
            LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) splits media directory name: '" + movieName + "'"); 
            try    
            { directoryname = System.IO.Path.GetDirectoryName(movieName); }
            catch
            { directoryname = ""; }
            LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) get media directory name: '" + directoryname + "'");
            if (!System.IO.Directory.Exists(directoryname))
            {
              directoryname = "";
              LogMyFilms.Debug("MyFilmsDetails (SearchtrailerLocal) directory of movie '" + movieName + "' doesn't exist anymore - check your DB");
            }

            //Search Files in Mediadirectory (used befor: SearchFiles("trailer", directoryname, true, true);)
            if (!string.IsNullOrEmpty(directoryname))
            {
              files = Directory.GetFiles(directoryname, "*.*", SearchOption.AllDirectories);
              foreach (string filefound in files)
              {
                if (((filefound.ToLower().Contains("trailer")) || (filefound.ToLower().Contains("trl"))) && (MediaPortal.Util.Utils.IsVideo(filefound)))
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
            }
            
            //Search Filenames with "title" in Trailer Searchpath
            string[] directories;
            if (ExtendedSearch && !string.IsNullOrEmpty(MyFilms.conf.StrDirStorTrailer))
            {
                LogMyFilms.Debug("MF: SearchTrailerLocal - starting ExtendedSearch in Searchdirectory: '" + MyFilms.conf.StrDirStorTrailer.ToString() + "'");
                foreach (string storage in Trailerdirectories)
                {
                  LogMyFilms.Debug("MF: (TrailersearchLocal) - TrailerSearchDirectory: '" + storage + "', search title1: '" + titlename.ToLower() + "', search title2: '" + titlename2.ToLower() + "'");
                  // First search rootdirectory
                  files = Directory.GetFiles(storage, "*.*", SearchOption.TopDirectoryOnly);
                  foreach (string filefound in files)
                  {
                    LogMyFilms.Debug("MF: (TrailersearchLocal) - Files found in root dir to check matching: '" + filefound + "'");
                    if ((!string.IsNullOrEmpty(titlename) && filefound.ToLower().Contains(titlename.ToLower())) || (!string.IsNullOrEmpty(titlename2) && filefound.ToLower().Contains(titlename2.ToLower())) && (MediaPortal.Util.Utils.IsVideo(filefound)))
                    {
                      wsize = new System.IO.FileInfo(filefound).Length;
                      result.Add(filefound);
                      resultsize.Add(wsize);
                      filesfound[filesfoundcounter] = filefound;
                      filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                      filesfoundcounter = filesfoundcounter + 1;
                      LogMyFilms.Debug("MF: (TrailersearchLocal) - Matching Singlefiles found in TrailerRootDIR: Size '" + wsize + "' - Name '" + filefound + "'");
                    }
                  }
                  
                  // Now search subdirectories
                    directories = Directory.GetDirectories(storage, "*.*", SearchOption.AllDirectories);
                    foreach (string directoryfound in directories)
                    {
                        LogMyFilms.Debug("MF: (TrailersearchLocal) - Directory found to check matching: '" + directoryfound + "'");
                        if ((!string.IsNullOrEmpty(titlename) && directoryfound.ToString().ToLower().Contains(titlename.ToLower())) || (!string.IsNullOrEmpty(titlename2) && directoryfound.ToString().ToLower().Contains(titlename2.ToLower())))
                        {
                            LogMyFilms.Debug("MF: (TrailersearchLocal) - Matching Directory found : '" + directoryfound + "'");
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
                                LogMyFilms.Debug("MF: (TrailersearchLocal) - Files found in sub dir to check matching: '" + filefound + "'");
                                if (((!string.IsNullOrEmpty(titlename) && filefound.ToLower().Contains(titlename.ToLower())) || (!string.IsNullOrEmpty(titlename2) && filefound.ToLower().Contains(titlename2.ToLower()))) && (MediaPortal.Util.Utils.IsVideo(filefound)))
                                {
                                    wsize = new System.IO.FileInfo(filefound).Length;
                                    result.Add(filefound);
                                    resultsize.Add(wsize);
                                    filesfound[filesfoundcounter] = filefound;
                                    filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                                    filesfoundcounter = filesfoundcounter + 1;
                                    LogMyFilms.Debug("MF: (TrailersearchLocal) - Matching Singlefiles found in TrailerDIR: Size '" + wsize + "' - Name '" + filefound + "'");
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
            {
              if (!string.IsNullOrEmpty(n))
                LogMyFilms.Debug("MF: (Sorted Trailerfiles) ******* : '" + n + "'");
            }  

            Array.Sort(filesfoundsize);
            for (int i = 0; i < result.Count; i++)
            {
              if (!string.IsNullOrEmpty(filesfound[i]))
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
                          if (!trailersourcepath.Contains(result[i].ToString()))
                          {
                            trailersourcepath = trailersourcepath + ";" + result[i];
                            LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - Added Trailer to Trailersource: '" + result[i] + "'");
                          }
                          else
                          {
                            LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - NOT added Trailer to Trailersource (DUPE): '" + result[i] + "'");
                          }
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
                LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - Database Updated !!!!");
            }
        }
        

        //-------------------------------------------------------------------------------------------
        //  Delete Trailer Entries From DB
        //-------------------------------------------------------------------------------------------        
        public static void DeleteTrailerFromDB(DataRow[] r1, int Index)
        {
          MyFilms.r[Index][MyFilms.conf.StrStorageTrailer] = string.Empty;
            LogMyFilms.Debug("MyFilmsDetails (DeleteTrailerFromDB) - Trailer entries Deleted for Current Movie !");
            Update_XML_database();
            LogMyFilms.Debug("MyFilmsDetails (DeleteTrailerFromDB) - Database Updated !");
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

        /// <summary>
        /// Update Trakt status of episode being watched on Timer Interval
        /// </summary>
        private void TraktUpdater(Object stateInfo)
        {
          PlayListItem item = (PlayListItem)stateInfo;

          // duration in minutes
          double duration = item.Duration / 60000;
          double progress = 0.0;

          // get current progress of player (in seconds) to work out percent complete
          if (duration > 0.0)
            progress = ((g_Player.CurrentPosition / 60.0) / duration) * 100.0;

          TraktMovieScrobble scrobbleData = null;

          IMDBMovie movie = new IMDBMovie(); // ToDo: To be replaced by true current data !! (Guzzi)

          // Set basic properties of scrobbledata
          scrobbleData = TraktHandler.CreateScrobbleData(movie);

          if (scrobbleData == null) return;

          // set duration/progress in scrobble data
          scrobbleData.Duration = Convert.ToInt32(duration).ToString();
          scrobbleData.Progress = Convert.ToInt32(progress).ToString();

          // set watching status on trakt
          TraktResponse response = TraktAPI.ScrobbleMovieState(scrobbleData, TraktScrobbleStates.watching);
          if (response == null) return;
          TraktHandler.CheckTraktErrorAndNotify(response, true);
        }


        /// <summary>
        /// Update trakt status on playback finish
        /// </summary>
        private void TraktScrobble_DoWork(object sender, DoWorkEventArgs e)
        {
          IMDBMovie movie = (IMDBMovie)e.Argument;

          double duration = movie.RunTime / 60000;

          // get scrobble data to send to api
          TraktMovieScrobble scrobbleData = TraktHandler.CreateScrobbleData(movie);
          if (scrobbleData == null) return;

          // set duration/progress in scrobble data
          scrobbleData.Duration = Convert.ToInt32(duration).ToString();
          scrobbleData.Progress = "100";

          TraktResponse response = TraktAPI.ScrobbleMovieState(scrobbleData, TraktScrobbleStates.scrobble);
          if (response == null) return;
          TraktHandler.CheckTraktErrorAndNotify(response, true);

          if (response.Status == "success")
          {
            // set trakt flags so we dont waste time syncing later
            //episode[DBOnlineEpisode.cTraktLibrary] = 1;
            //episode[DBOnlineEpisode.cTraktSeen] = 1;
          }
        }

      private bool IsInternetConnectionAvailable ()
      {
        // Check Internet connection
        if (!Win32API.IsConnectedToInternet())
        {
          GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_OK);
          dlgOk.SetHeading(257);
          dlgOk.SetLine(1, GUILocalizeStrings.Get(703));
          dlgOk.DoModal(GUIWindowManager.ActiveWindow);
          return false;
        }
        else
          return true;
      }

      /// <summary>
      /// make a cleaned title out of the filename/original title
      /// </summary>
      private static string GetSearchString(string strMovie)
      {
        string strURL = strMovie;
        //strURL = strURL.ToLower();
        strURL = strURL.Replace(".", " ");
        strURL = strURL.Trim();

        RemoveAllAfter(ref strURL, "divx");
        RemoveAllAfter(ref strURL, "xvid");
        RemoveAllAfter(ref strURL, "dvd");
        RemoveAllAfter(ref strURL, " dvdrip");
        RemoveAllAfter(ref strURL, "svcd");
        RemoveAllAfter(ref strURL, "mvcd");
        RemoveAllAfter(ref strURL, "vcd");
        RemoveAllAfter(ref strURL, "cd");
        RemoveAllAfter(ref strURL, "ac3");
        RemoveAllAfter(ref strURL, "ogg");
        RemoveAllAfter(ref strURL, "ogm");
        RemoveAllAfter(ref strURL, "internal");
        RemoveAllAfter(ref strURL, "fragment");
        RemoveAllAfter(ref strURL, "proper");
        RemoveAllAfter(ref strURL, "limited");
        RemoveAllAfter(ref strURL, "rerip");
        RemoveAllAfter(ref strURL, "bluray");
        RemoveAllAfter(ref strURL, "brrip");
        RemoveAllAfter(ref strURL, "hddvd");
        RemoveAllAfter(ref strURL, "x264");
        RemoveAllAfter(ref strURL, "mbluray");
        RemoveAllAfter(ref strURL, "1080p");
        RemoveAllAfter(ref strURL, "720p");
        RemoveAllAfter(ref strURL, "480p");
        RemoveAllAfter(ref strURL, "r5");

        RemoveAllAfter(ref strURL, "+divx");
        RemoveAllAfter(ref strURL, "+xvid");
        RemoveAllAfter(ref strURL, "+dvd");
        RemoveAllAfter(ref strURL, "+dvdrip");
        RemoveAllAfter(ref strURL, "+svcd");
        RemoveAllAfter(ref strURL, "+mvcd");
        RemoveAllAfter(ref strURL, "+vcd");
        RemoveAllAfter(ref strURL, "+cd");
        RemoveAllAfter(ref strURL, "+ac3");
        RemoveAllAfter(ref strURL, "+ogg");
        RemoveAllAfter(ref strURL, "+ogm");
        RemoveAllAfter(ref strURL, "+internal");
        RemoveAllAfter(ref strURL, "+fragment");
        RemoveAllAfter(ref strURL, "+proper");
        RemoveAllAfter(ref strURL, "+limited");
        RemoveAllAfter(ref strURL, "+rerip");
        RemoveAllAfter(ref strURL, "+bluray");
        RemoveAllAfter(ref strURL, "+brrip");
        RemoveAllAfter(ref strURL, "+hddvd");
        RemoveAllAfter(ref strURL, "+x264");
        RemoveAllAfter(ref strURL, "+mbluray");
        RemoveAllAfter(ref strURL, "+1080p");
        RemoveAllAfter(ref strURL, "+720p");
        RemoveAllAfter(ref strURL, "+480p");
        RemoveAllAfter(ref strURL, "+r5");
        return strURL;
      }
      /// <summary>
      /// cuts end of sting after strWord
      /// </summary>
      private static void RemoveAllAfter(ref string strLine, string strWord)
      {
        int iPos = strLine.IndexOf(strWord);
        if (iPos > 0)
        {
          strLine = strLine.Substring(0, iPos);
        }
      }

      private bool handleTrailer()
      {
        if (trailerPlayed)
        {

          // Set custom trailer played back to false
          trailerPlayed = false;

          // If a trailer was just played, we need to play the selected movie
          //playMovie(queuedMedia.AttachedMovies[0], queuedMedia.Part);
          return true;
        }

        return false;
      }

      public static int MovieDuration(ArrayList files)
      {
        int _totalMovieDuration = 0;

        foreach (string file in files)
        {
          int fileID = VideoDatabase.GetFileId(file);
          int tempDuration = VideoDatabase.GetMovieDuration(fileID);

          if (tempDuration > 0)
          {
            _totalMovieDuration += tempDuration;
          }
          else
          {
            MediaInfoWrapper mInfo = new MediaInfoWrapper(file);

            if (fileID > -1)
              VideoDatabase.SetMovieDuration(fileID, mInfo.VideoDuration / 1000);
            _totalMovieDuration += mInfo.VideoDuration / 1000;
          }
        }
        return _totalMovieDuration;
      }
    



    }

}
        #endregion
