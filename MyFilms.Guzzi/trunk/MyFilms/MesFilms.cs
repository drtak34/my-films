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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;
using System.Data;

using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using MediaPortal.Configuration;
using MediaPortal.Util;
using MediaPortal.Video.Database; // Guzzi Added for IMDB Fetcher

using NewStringLib;
using Cornerstone.MP;
using MesFilms.MyFilms;
using MesFilms.WakeOnLan;
using System.Threading;
using System.Linq;


namespace MesFilms
{
    /// <summary>
    /// Summary description for GUIMesFilms.
    /// </summary>
    // [PluginIcons("MesFilms.MyFilms.Resources.clapperboard-128x128.png", "MesFilms.MyFilms.Resources.clapperboard-128x128-faded.png")]
    [PluginIcons("MesFilms.MyFilms.Resources.film-reel-128x128.png", "MesFilms.MyFilms.Resources.film-reel-128x128-faded.png")]
    public class MesFilms : GUIWindow, ISetupForm
    {
        //private BaseMesFilms films;
        #region Descriptif zones Ecran

        public const int ID_MesFilms = 7986;
        public const int ID_MesFilmsDetail = 7987;
        public const int ID_MesFilmsActors = 7989;
        public const int ID_MesFilmsThumbs = 7990;
        public const int ID_MesFilmsActorsInfo = 7991;
        enum Controls : int
        {
            CTRL_BtnSrtBy = 2,
            CTRL_BtnViewAs = 3,
            CTRL_BtnSearchT = 4,
            CTRL_BtnOptions = 5,
            CTRL_BtnLayout = 6,
            //CTRL_BtnChangeDB = 7, Not used, done in options instead!
            //CTRL_TxtSelect = 12,
            CTRL_Fanart = 11,
            CTRL_Fanart2 = 21,
            CTRL_LoadingImage = 22,
            CTRL_Image = 1020,
            CTRL_Image2 = 1021,
            CTRL_List = 1026,
            CTRL_Label1 = 1030,
            CTRL_Item1 = 1031,
            CTRL_Label2 = 1032,
            CTRL_Item2 = 1033,
            CTRL_Item3 = 1034,
            CTRL_logos_id2001 = 2001,
            CTRL_logos_id2002 = 2002,
            CTRL_MovieInfoIsAvailable = 3001,
            CTRL_MovieIsAvailable = 3002,
            CTRL_TrailerIsAvailable = 3003,
        }
        //[SkinControlAttribute((int)Controls.CTRL_TxtSelect)]
        //protected GUIFadeLabel TxtSelect = null;
        [SkinControlAttribute((int)Controls.CTRL_BtnSrtBy)]
        protected GUISortButtonControl BtnSrtBy = null;

        [SkinControlAttribute((int)Controls.CTRL_List)]
        protected GUIFacadeControl facadeView = null;

        [SkinControlAttribute((int)Controls.CTRL_Label1)]
        protected GUILabelControl TxtLabel1 = null;

        [SkinControlAttribute((int)Controls.CTRL_Item1)]
        protected GUIFadeLabel TxtItem1 = null;

        [SkinControlAttribute((int)Controls.CTRL_Label2)]
        protected GUILabelControl TxtLabel2 = null;

        [SkinControlAttribute((int)Controls.CTRL_Item2)]
        protected GUIFadeLabel TxtItem2 = null;

        [SkinControlAttribute((int)Controls.CTRL_Item3)]
        protected GUIFadeLabel TxtItem3 = null;

        [SkinControlAttribute((int)Controls.CTRL_Image)]
        protected GUIImage ImgLstFilm = null;

        [SkinControlAttribute((int)Controls.CTRL_Image2)]
        protected GUIImage ImgLstFilm2 = null;

        [SkinControlAttribute((int)Controls.CTRL_logos_id2001)]
        protected GUIImage ImgID2001 = null;

        [SkinControlAttribute((int)Controls.CTRL_logos_id2002)]
        protected GUIImage ImgID2002 = null;

        [SkinControlAttribute((int)Controls.CTRL_Fanart)]
        protected GUIImage ImgFanart = null;

        [SkinControlAttribute((int)Controls.CTRL_Fanart2)]
        protected GUIImage ImgFanart2 = null;

        [SkinControlAttribute((int)Controls.CTRL_LoadingImage)]
        protected GUIImage loadingImage = null;

        // ControlIDs to let the skin react to certain states and properties (e.g. trailer available)

        [SkinControlAttribute((int)Controls.CTRL_MovieInfoIsAvailable)]
        protected GUIImage MovieInfoIsAvailable = null;

        [SkinControlAttribute((int)Controls.CTRL_MovieIsAvailable)]
        protected GUIImage MovieIsAvailable = null;

        [SkinControlAttribute((int)Controls.CTRL_TrailerIsAvailable)]
        protected GUIImage TrailerIsAvailable = null;

        [SkinControlAttribute(3004)]
        protected GUIAnimation m_SearchAnimation = null;

        public int Layout = 0;
        public static int Prev_ItemID = -1;
        //Added to jump back to correct Menu (Either Basichome or MyHome - or others...)
        public static int Prev_MenuID = -1;
        public bool Context_Menu = false;
        public static Configuration conf;
        public static Logos confLogos;
        //private string currentConfig;
        private string strPluginName = "GuzziThek";
        public static DataRow[] r; // will hold current recordset to traverse

        //Imageswapperdefinitions for fanart and cover
        //public ImageSwapper backdrop;
        private ImageSwapper backdrop;
        private AsyncImageResource cover = null;

        // Guzzi: Added from TV-Series for Fanarttoggling
        //private System.Threading.Timer m_scanTimer = null;
        private System.Threading.Timer m_FanartTimer = null;
        private bool m_bFanartTimerDisabled = false;

        //Guzzi Addons for Global nonpermanent Trailer and MinRating Filters
        public bool GlobalFilterTrailersOnly = false;
        public bool GlobalFilterMinRating = false;
        public string GlobalFilterString = "";
        public bool MovieScrobbling = false;
        public int actorID = 0;
        public static string CurrentMovie;
        //public static string CurrentFanartDir;
        public enum optimizeOption { optimizeDisabled };
        //public enum optimizeDisabled;
        public static bool InitialStart = false; //Added to implement InitialViewSetup, ToDo: Add Logic
        #endregion


        #region events
  
        public delegate void FilmsStoppedHandler(int stoptime, string filename);
        public delegate void FilmsEndedHandler(string filename);   
        System.ComponentModel.BackgroundWorker bgUpdateDB = new System.ComponentModel.BackgroundWorker();
        System.ComponentModel.BackgroundWorker bgUpdateFanart = new System.ComponentModel.BackgroundWorker();
        System.ComponentModel.BackgroundWorker bgLoadMovieList = new System.ComponentModel.BackgroundWorker();

        #endregion
        public MesFilms()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #region ISetupForm Members

        // Returns the name of the plugin which is shown in the plugin menu
        public string PluginName()
        {
            return "My Films";
        }

        // Returns the description of the plugin is shown in the plugin menu
        public string Description()
        {
            return "My Films Ant Movie Catalog - Guzzi Edition";
        }

        // Returns the author of the plugin which is shown in the plugin menu
        public string Author()
        {
            return "Zebons (Mod by Guzzi)";
        }

        // show the setup dialog
        public void ShowPlugin()
        {
            System.Windows.Forms.Form setup = new MesFilmsSetup();
            setup.ShowDialog();
        }

        // Indicates whether plugin can be enabled/disabled
        public bool CanEnable()
        {
            return true;
        }

        // get ID of windowplugin belonging to this setup
        public int GetWindowId()
        {
            return 7986;
        }

        // Indicates if plugin is enabled by default;
        public bool DefaultEnabled()
        {
            return true;
        }

        // indicates if a plugin has its own setup screen
        public bool HasSetup()
        {
            return true;
        }
        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
        {
            string wPluginName = strPluginName;
            using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
            {
                wPluginName = xmlreader.GetValueAsString("MyFilms", "PluginName", "My Films");
            }

            strButtonText = wPluginName;
            strButtonImage = String.Empty;
            strButtonImageFocus = String.Empty;
            strPictureImage = String.Format("hover_{0}.png", "Films");
            string strBtnFile = String.Format(@"{0}\media\{1}", GUIGraphicsContext.Skin, strPictureImage);
            if (!System.IO.File.Exists(strBtnFile))
                strPictureImage = "";
            return true;
        }
        public override int GetID
        {
            get {return ID_MesFilms;}
        }

        public override bool Init()
        //This Method is only loaded ONCE when starting Mediaportal !!!
        {
            Log.Debug("MyFilms.Init() started.");
            // create Backdrop image swapper
            backdrop = new ImageSwapper();
            backdrop.ImageResource.Delay = 250;
            backdrop.PropertyOne = "#myfilms.fanart";
            //backdrop.PropertyTwo = "#myfilms.fanart2";
            //backdrop.LoadingImage = loadingImage;

            // create Cover image swapper
            cover = new AsyncImageResource();
            cover.Property = "#myfilms.coverimage";
            cover.Delay = 250;

            // (re)link our backdrop image controls to the backdrop image swapper
            //backdrop.GUIImageOne = ImgFanart;
            //backdrop.GUIImageTwo = ImgFanart2;
            //backdrop.LoadingImage = loadingImage;

            // Ceate Variable for OneTimeView Setup
            InitialStart = true;

            Log.Debug("MyFilms.Init() completed.");

            return Load(GUIGraphicsContext.Skin + @"\MesFilms.xml");
        }

        protected override void OnPageLoad()
            //This is loaded each time, the plugin is entered - can be used to reset certain settings etc.
        {
            Log.Debug("MyFilms.OnPageLoad() started.");
            
            // (re)link our backdrop image controls to the backdrop image swapper
            backdrop.GUIImageOne = ImgFanart;
            backdrop.GUIImageTwo = ImgFanart2;
            //backdrop.LoadingImage = loadingImage;  --> Do NOT activate - otherwise coverimage flickers and goes away !!!!

            // Setup Random Fanart Timer
            m_FanartTimer = new System.Threading.Timer(new TimerCallback(FanartTimerEvent), null, Timeout.Infinite, Timeout.Infinite);
            m_bFanartTimerDisabled = true;
            // Enable Random Fanart Timer
            // Enable to use threaded timer
            //m_FanartTimer.Change(0,10000);
            m_bFanartTimerDisabled = false;

            Log.Debug("MyFilms.OnPageLoad() completed.");

        }

        protected override void OnPageDestroy(int new_windowId)
        {
            Log.Debug("MyFilms.OnPageDestroy() completed.");

            // Disable Random Fanart Timer
            m_FanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_bFanartTimerDisabled = true;

            base.OnPageDestroy(new_windowId);

            Log.Debug("MyFilms.OnPageDestroy() completed.");
        }


        #endregion

        #region Action
        
        //---------------------------------------------------------------------------------------
        //   Handle Keyboard Actions
        //---------------------------------------------------------------------------------------

        public override void OnAction(MediaPortal.GUI.Library.Action actionType)
        {
            Log.Debug("MyFilms : OnAction " + actionType.wID.ToString());
            //Log.Debug("MyFilms : PreviousWindowId: '" + PreviousWindowId.ToString() + "' - Prev_MenuID: '" + Prev_MenuID.ToString() + "'");
            //Log.Debug("MyFilms : Select: '" + conf.Boolselect + "' - View: '" + conf.Boolview + "' - Return: '" + conf.Boolreturn + "'");
            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PARENT_DIR)
                if (GetPrevFilmList()) return;

            if ((actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) && (conf.Boolselect || conf.Boolview))
            {
                Change_LayOut(MesFilms.conf.StrLayOut);
                if (GetPrevFilmList()) return;
            }
            if ((actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) && (conf.Boolreturn))
            {
                conf.Boolreturn = false;
                if (conf.WStrSort.ToString().ToUpper() == "ACTORS")
                    if (GetPrevFilmList())
                        return;
                    else
                        base.OnAction(actionType);
                Change_view(conf.WStrSort.ToLower());
                return;
            }
            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU)
            {
                if (GetPrevFilmList())
                    return;
                else
                {
                    //GUIWindowManager.ShowPreviousWindow();
                    //Fix to not only always return to MyHome, e.g. when coming from Basichome...
                    //GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_HOME);
                    //if (Prev_MenuID != -1)
                    Log.Debug("MyFilms (GuzziFix for Previous Window - Prev_MenuID: '" + Prev_MenuID.ToString() + "'");
                    GUIWindowManager.ActivateWindow(Prev_MenuID); 
                    return;
                }
            }
			
			// Original Code from ZebonsMerge
			// if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU)
            // {
            //   if (facadeView.Focus)
            //        GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnSearchT);
            //    else
            //        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
            //    return;
            // }
			// End Merge Code
			
            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU)
                if (facadeView.SelectedListItemIndex > -1)
                    {
                        if (!(facadeView.Focus)) GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
                        return;
                    }

            if (actionType.m_key != null)
            {
                if ((actionType.m_key.KeyChar == 112) && (facadeView.Focus) && !(facadeView.SelectedListItem.IsFolder) && ((MesFilms.conf.StrSuppress) || (MesFilms.conf.StrGrabber)))
                {
                    MesFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
                }
                if ((actionType.m_key.KeyChar == 120) && (Context_Menu))
                {
                    Context_Menu = false;
                    return;
                }
                if ((actionType.m_key.KeyChar == 120) && (facadeView.Focus) && !(facadeView.SelectedListItem.IsFolder) && ((MesFilms.conf.StrSuppress) || (MesFilms.conf.StrGrabber)))
                {
                    // context menu for update or suppress entry
                    Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
                    return;
                }
            }

            if (actionType.wID.ToString().Substring(0, 6) == "REMOTE")
                return;
            base.OnAction(actionType);
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
                    base.OnMessage(messageType);
                    //Hier mu� irgendwie gemerkt werden, da� eine R�ckkehr vom TrailerIsAvailable erfolgt - CheckAccess WIndowsID des Conterxts via LOGs
                    if ((PreviousWindowId != ID_MesFilmsDetail) && !MovieScrobbling && (PreviousWindowId != ID_MesFilmsActors))
                    {
                        Prev_MenuID = PreviousWindowId; 
                        InitMainScreen();
                        GUIPropertyManager.SetProperty("#myfilms.select", " ");
                        Configuration.Current_Config();
                        Load_Config(Configuration.CurrentConfig, true);
                        // this is too early ... -> InitialStart = false; // Guzzi: Set to false after first initialization to be able to return to noninitialized View - Make sure to set true if changing DB config
                    }
                    if (Configuration.CurrentConfig.Length == 0)
                        GUIWindowManager.ShowPreviousWindow();

                    // Originally Deactivated by Zebons    
                    // ********************************
                    // ToDo: Crash on Details to be fixed (make it threadsafe !!!!!!!)
                    if (!bgLoadMovieList.IsBusy)
                    {
                        bgLoadMovieList.DoWork += new DoWorkEventHandler(bgLoadMovieList_DoWork);
                        bgLoadMovieList.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgLoadMovieList_RunWorkerCompleted);
                        AsynLoadMovieList();
                    }
                    // ********************************
                    // Originally Deactivated by Zebons    
                    
                    
                    GUIControl.ShowControl(GetID, 34);
                    GUIWaitCursor.Hide();

                    if (TxtItem1 != null)
                        TxtItem1.Label = " ";
                    if (TxtLabel1 != null)
                        TxtLabel1.Label = " ";
                    if (TxtItem2 != null)
                        TxtItem2.Label = " ";
                    if (TxtLabel2 != null)
                        TxtLabel2.Label = " ";
                    if (TxtItem3 != null)
                        TxtItem3.Label = " ";
                    if (!(MesFilms.conf.Stritem1 == null) && !(MesFilms.conf.Stritem1 == "(none)") && !(MesFilms.conf.Stritem1 == ""))
                    {
                        if (TxtLabel1 != null)
                            TxtLabel1.Label = MesFilms.conf.Strlabel1;
                        if (TxtItem1 != null)
                            if (MesFilms.conf.Stritem1.ToLower() == "date")
                                TxtItem1.Label = "#myfilms.w" + MesFilms.conf.Stritem1.ToLower();
                            else
                                TxtItem1.Label = "#myfilms." + MesFilms.conf.Stritem1.ToLower();
                    }
                    if (!(MesFilms.conf.Stritem2 == null) && !(MesFilms.conf.Stritem2 == "(none)") && !(MesFilms.conf.Stritem2 == ""))
                    {
                        if (TxtLabel2 != null)
                            TxtLabel2.Label = MesFilms.conf.Strlabel2;
                        if (TxtItem2 != null)
                            if (MesFilms.conf.Stritem2.ToLower() == "date")
                                TxtItem2.Label = "#myfilms.w" + MesFilms.conf.Stritem2.ToLower();
                            else
                                TxtItem2.Label = "#myfilms." + MesFilms.conf.Stritem2.ToLower();
                    }
                    if (!(MesFilms.conf.Stritem3 == null) && !(MesFilms.conf.Stritem3 == "(none)") && !(MesFilms.conf.Stritem3 == ""))
                    {
                        if (TxtItem3 != null)
                            if (MesFilms.conf.Stritem3.ToLower() == "date")
                                TxtItem3.Label = "#myfilms.w" + MesFilms.conf.Stritem3.ToLower();
                            else
                                TxtItem3.Label = "#myfilms." + MesFilms.conf.Stritem3.ToLower();
                    }
                    if (((conf.AlwaysDefaultView) || (InitialStart)) && (PreviousWindowId != ID_MesFilmsDetail) && !MovieScrobbling && (PreviousWindowId != ID_MesFilmsActors))
                        Fin_Charge_Init(true,false);
                    else
                        Fin_Charge_Init(false, false);
                    return true;

                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT: //called when exiting plugin either by prev menu or pressing home button
                    Log.Debug("MyFilms : GUI_MSG_WINDOW_DEINIT erkannt !!! "); 
                    GUITextureManager.CleanupThumbs();

                    if (Configuration.CurrentConfig != "")
                    {
                        if (facadeView == null || facadeView.SelectedListItemIndex == -1)
                            Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, "");
                        else
                            Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
                    }
                    //ImgFanart.SetFileName(string.Empty);
                    //ImgFanart2.SetFileName(string.Empty);
                    facadeView.Resources.Clear();
                    facadeView.Clear();
                    //backdrop.PropertyOne = " ";
                    // added from MoPic
                    backdrop.Filename = string.Empty;
                    cover.Filename = string.Empty;

                    //Disable FanartTimer - already done on pagedestroy ...
                    //m_FanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    //m_bFanartTimerDisabled = true;

                    return true; // fall through to call base class?

                case GUIMessage.MessageType.GUI_MSG_CLICKED:
                    //---------------------------------------------------------------------------------------
                    // Mouse/Keyboard Clicked
                    //---------------------------------------------------------------------------------------
                    Log.Debug("MyFilms : GUI_MSG_CLICKED erkannt !!! "); 
                    if ((iControl == (int)Controls.CTRL_BtnSrtBy) && (conf.Boolselect))
                        // No change sort method and no searchs during select
                        return true;
                    if ((iControl == (int)Controls.CTRL_BtnSearchT) && (conf.Boolselect))
                        conf.Boolselect = false;
                    if (iControl == (int)Controls.CTRL_BtnSearchT)
                    // Search dialog search
                    {
                        GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        if (dlg == null) return true;
                        dlg.Reset();
                        dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                        System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
                        //Add Menuentries here

                        //Guzzi: RandomMovie Search added
                        dlg.Add(GUILocalizeStrings.Get(10798621));//Search global movies by randomsearch (singlesearch, areasearch)
                        choiceSearch.Add("randomsearch");

                        if (MesFilms.conf.StrSearchList[0].Length > 0)
                        {
                            dlg.Add(GUILocalizeStrings.Get(10798615));//Search global movies by property
                            choiceSearch.Add("globalproperty");
                        }
                        
                        dlg.Add(GUILocalizeStrings.Get(10798645));//Search global movies by areas
                        choiceSearch.Add("globalareas");

                        dlg.Add(GUILocalizeStrings.Get(137) + " " + GUILocalizeStrings.Get(342));//Title
                        choiceSearch.Add("title");

                        dlg.Add(GUILocalizeStrings.Get(137) + " " + GUILocalizeStrings.Get(344));//Actors
                        choiceSearch.Add("actors");
                        for (int i = 0; i < 2; i++)
                        {
                            if (MesFilms.conf.StrSearchItem[i] != "(none)" && MesFilms.conf.StrSearchItem[i].Length > 0)
                            {
                                if (MesFilms.conf.StrSearchText[i].Length == 0)
                                    dlg.Add(GUILocalizeStrings.Get(137) + " " + MesFilms.conf.StrSearchItem[i]);//Specific search with no text
                                else
                                    dlg.Add(GUILocalizeStrings.Get(137) + " " + MesFilms.conf.StrSearchText[i]);//Specific search  text
                                choiceSearch.Add(string.Format("search{0}", i.ToString()));
                            }
                        }
                        if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
                        {
                            dlg.Add(GUILocalizeStrings.Get(1079866));//Search related movies by persons
                            choiceSearch.Add("analogyperson");
                        }
                        if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder && MesFilms.conf.StrSearchList[0].Length > 0)
                        {
                            dlg.Add(GUILocalizeStrings.Get(10798614));//Search related movies by property
                            choiceSearch.Add("analogyproperty");
                        }
						
                        dlg.DoModal(GetID);

                        if (dlg.SelectedLabel == -1)
                            return true;
                        if (choiceSearch[dlg.SelectedLabel] == "analogyperson")
                        {
                            SearchRelatedMoviesbyPersons((int)facadeView.SelectedListItem.ItemId);
                            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                            dlg.DeInit();
                            return base.OnMessage(messageType);
                        }

                        if (choiceSearch[dlg.SelectedLabel] == "analogyproperty")
                        {
                            SearchRelatedMoviesbyProperties((int)facadeView.SelectedListItem.ItemId);
							// This version from ZebonsMerge - Change ClassName to SearchRelatedMoviesbyPropertiesZebons
							// SearchRelatedMoviesbyProperties((int)facadeView.SelectedListItem.ItemId, MesFilms.conf.StrSearchList);

                            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                            dlg.DeInit();
                            return base.OnMessage(messageType);
                        }
                        if (choiceSearch[dlg.SelectedLabel] == "randomsearch")
                        {
                            SearchMoviesbyRandomWithTrailer();
                            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                            dlg.DeInit();
                            return base.OnMessage(messageType);
                        }
                        if (choiceSearch[dlg.SelectedLabel] == "globalareas")
                        {
                            SearchMoviesbyAreas();
                            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                            dlg.DeInit();
                            return base.OnMessage(messageType);
                        }
                        if (choiceSearch[dlg.SelectedLabel] == "globalproperty")
                        {
                            SearchMoviesbyProperties();
							// This version from ZebonsMerge - Change ClassName to SearchMoviesbyPropertiesZebons
							// SearchMoviesbyProperties(MesFilms.conf.StrSearchList);

                            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                            dlg.DeInit();
                            return base.OnMessage(messageType);
                        }
                        VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                        if (null == keyboard) return true;
                        keyboard.Reset();
                        keyboard.Text = "";
                        keyboard.DoModal(GetID);
                        if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                        {
                            switch (choiceSearch[dlg.SelectedLabel])
                            {
                                case "title":
                                    if (control_searchText(keyboard.Text))
                                    {
                                        conf.StrSelect = conf.StrTitle1.ToString() + " like '*" + keyboard.Text + "*'";
                                        conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(369) + " [*" + keyboard.Text + @"*]";
                                        conf.StrTitleSelect = "";
                                        GetFilmList();
                                    }
                                    else
                                        return false;
                                    break;
                                case "actors":
                                    if (control_searchText(keyboard.Text))
                                    {
                                        conf.WStrSort = "ACTORS";
                                        conf.Wselectedlabel = "";
                                        conf.WStrSortSens = " ASC";
                                        BtnSrtBy.IsAscending = true;
                                        conf.StrActors = keyboard.Text;
                                        getSelectFromDivx("Actors like '*" + keyboard.Text + "*'", conf.WStrSort, conf.WStrSortSens, keyboard.Text, true, "");
                                    }
                                    else
                                        return false;
                                    break;
                                case "search0":
                                case "search1":
                                    int i = 0;
                                    if (choiceSearch[dlg.SelectedLabel] == "search1")
                                        i = 1;
                                    AntMovieCatalog ds = new AntMovieCatalog();
                                    if (control_searchText(keyboard.Text))
                                    {
                                        if (ds.Movie.Columns[conf.StrSearchItem[i].ToString()].DataType.Name == "string")
                                          conf.StrSelect = conf.StrSearchItem[i].ToString() + " like '*" + keyboard.Text + "*'";
                                        else
                                            conf.StrSelect = conf.StrSearchItem[i].ToString() + " = '" + keyboard.Text + "'";
                                        conf.StrTxtSelect = "Selection " + conf.StrSearchText[i] + " [*" + keyboard.Text + @"*]";
                                        conf.StrTitleSelect = "";
                                        GetFilmList();
                                    }
                                    else
                                        return false;
                                    break;
                            }
                        }
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        dlg.DeInit();
                    }
                    if (iControl == (int)Controls.CTRL_BtnSrtBy)
                    // Choice of Sort Method
                    {
                        GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        if (dlg == null) return true;
                        dlg.Reset();
                        dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                        System.Collections.Generic.List<string> choiceSort = new System.Collections.Generic.List<string>();
                        dlg.Add(GUILocalizeStrings.Get(103));//Title
                        dlg.Add(GUILocalizeStrings.Get(366));//Year
                        dlg.Add(GUILocalizeStrings.Get(104));//Date
                        dlg.Add(GUILocalizeStrings.Get(367));//Rating
                        choiceSort.Add("title");
                        choiceSort.Add("year");
                        choiceSort.Add("date");
                        choiceSort.Add("rating");
                        for (int i = 0; i < 2; i++)
                        {
                            if (conf.StrSort[i] != "(none)" && conf.StrSort[i].Length > 0)
                            {
                                dlg.Add(conf.StrTSort[i]);//Specific sort i
                                choiceSort.Add(string.Format("sort{0}",i.ToString()));
                            }
                        }
                        dlg.DoModal(GetID);

                        if (dlg.SelectedLabel == -1)
                            return true;
                        conf.StrIndex = 0;
                        switch (choiceSort[dlg.SelectedLabel])
                        {
                            case "title":
                                conf.CurrentSortMethod = GUILocalizeStrings.Get(103);
                                conf.StrSorta = conf.StrSTitle;
                                conf.StrSortSens = " ASC";
                                break;
                            case "year":
                                conf.CurrentSortMethod = GUILocalizeStrings.Get(366);
                                conf.StrSorta = "YEAR";
                                conf.StrSortSens = " DESC";
                                break;
                            case "date":
                                conf.CurrentSortMethod = GUILocalizeStrings.Get(621);
                                conf.StrSorta = "DateAdded";
                                conf.StrSortSens = " DESC";
                                break;
                            case "rating":
                                conf.CurrentSortMethod = GUILocalizeStrings.Get(367);
                                conf.StrSorta = "RATING";
                                conf.StrSortSens = " DESC";
                                break;
                            case "sort0":
                            case "sort1":
                                int i = 0;
                                if (choiceSort[dlg.SelectedLabel] == "sort1")
                                    i = 1;
                                conf.CurrentSortMethod = conf.StrTSort[i];
                                conf.StrSorta = conf.StrSort[i];
                                conf.StrSortSens = " ASC";
                                break;
                        }
                        dlg.DeInit();
                        BtnSrtBy.Label = conf.CurrentSortMethod;
                        if (!conf.Boolselect)
                            GetFilmList();
                        else
                            getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.StrSorta, conf.StrSortSens, "*", true, "");
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        return true;
                    }

                    if (iControl == (int)Controls.CTRL_BtnViewAs)
                    // Change Selected View
                    {
                        Selection_type_Video();
                        return base.OnMessage(messageType);
                    }
                    if (iControl == (int)Controls.CTRL_BtnOptions)
                    // Change Selected Option
                    {
                        Selection_type_Option();
                        return base.OnMessage(messageType);
                    } 
                    if ((iControl == (int)Controls.CTRL_BtnLayout) && !conf.Boolselect)
                    // Change Layout View
                    {
                        GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        if (dlg == null) return true;
                        dlg.Reset();
                        dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                        dlg.Add(GUILocalizeStrings.Get(101));//List
                        dlg.Add(GUILocalizeStrings.Get(100));//Icons
                        dlg.Add(GUILocalizeStrings.Get(417));//Large Icons
                        dlg.Add(GUILocalizeStrings.Get(733));//Filmstrip
                        dlg.Add(GUILocalizeStrings.Get(791));//Coverflow
                        dlg.DoModal(GetID);

                        if (dlg.SelectedLabel == -1)
                            return true;
                        conf.StrIndex = 0;
                        int wselectindex = facadeView.SelectedListItemIndex;
                        Change_LayOut(dlg.SelectedLabel);
                        MesFilms.conf.StrLayOut = dlg.SelectedLabel;
                        dlg.DeInit();
                        GetFilmList();
                        GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, (int)wselectindex);
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        return base.OnMessage(messageType);
                    }

                    if (iControl == (int)Controls.CTRL_List)
                    {
                        if (facadeView.SelectedListItemIndex > -1)
                        {
                            if (!facadeView.SelectedListItem.IsFolder && !conf.Boolselect)
                            // New Window for detailed selected item information
                            {
                                conf.StrIndex = facadeView.SelectedListItem.ItemId;
                                conf.StrTIndex = facadeView.SelectedListItem.Label;
                                GUITextureManager.CleanupThumbs();

                                //Start: Added for MovieThumbs
                                CurrentMovie = "";
                                try
                                { CurrentMovie = (string)MesFilms.r[facadeView.SelectedListItem.ItemId][MesFilms.conf.StrStorage].ToString().Trim(); }
                                catch
                                { CurrentMovie = ""; }
                                Log.Debug("MyFilms - PrepareThumbView: CurrentMovie = '" + CurrentMovie + "'");
                                //End: Added for MovieThumbs

                                //Start: Added for Timed Imagerswapper in Main View
                                //CurrentFanartDir = "";
                                //try
                                //{ CurrentFanartDir = (string)MesFilms.r[facadeView.SelectedListItem.ItemId][MesFilms.conf.StrStorage].ToString().Trim(); }
                                //catch
                                //{ CurrentFanartDir = ""; }
                                //Log.Debug("MyFilms - Set CurrentFanartDir: = '" + CurrentFanartDir + "'");

                                GUIWindowManager.ActivateWindow(ID_MesFilmsDetail);
                            }
                            else
                            // View List as selected
                            {
                                conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                                Change_LayOut(MesFilms.conf.StrLayOut);
                                if (facadeView.SelectedListItem.IsFolder)
                                    conf.Boolreturn = false;
                                else
                                    conf.Boolreturn = true;
                                do
                                {
                                    if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                                    conf.StrTitleSelect += conf.Wselectedlabel;
                                } while (GetFilmList() == false); //keep calling while single folders found
                            }
                        }
                    }
                    return base.OnMessage(messageType);
            }
            return base.OnMessage(messageType);
        }
        #endregion


        /// <summary>Jumps to prev folder in FilmList  by modifying Selects and calling GetFilmList</summary>
        /// <returns>If returns false means cannot jump back any further, so caller must exit plugin to main menu.</returns>
        bool GetPrevFilmList()
        {
            Prev_ItemID = -1;
            string SelItem;
            if (conf.StrTitleSelect == "")
            {
                if (NewString.Left(conf.StrTxtSelect, 9) == "Selection" || (conf.StrTxtSelect == "" && conf.Boolselect) || conf.Boolview) //original code block refactored
                {//jump back to main full list
                    conf.Boolselect = false;
                    conf.Boolview = false;
                    conf.Boolreturn = false;
                    conf.StrSelect = conf.StrTxtSelect = conf.StrFilmSelect = "";
                    conf.StrIndex = 0;
                    GetFilmList();
                    return true;
                }

                if (conf.StrTxtSelect == "")
                {
                    return false;
                }
                else
                {   // Jump back to prev view_display (categorised by year, genre etc)
                    if (conf.WStrSort == "ACTORS")
                    {
                        conf.StrSelect = "Actors like '*" + conf.StrActors + "*'";
                        conf.StrTxtSelect = "Selection";
                        getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.StrActors, true, "");
                    }
                    else
                    {
                        SelItem = NewString.StripChars(@"[]", conf.StrTxtSelect);
                        if (conf.WStrSort == "DateAdded")
                            getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", "Date", " DESC", "*", true, SelItem);
                        else
                            getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, SelItem);
                        conf.StrSelect = "";
                    }
                }
            }
            else
            {
                SelItem = NewString.NPosRight(conf.TitleDelim, conf.StrTitleSelect, -1, false, false); // get last substring
                if (NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) > 0)
                    conf.StrTitleSelect = NewString.NPosLeft(conf.TitleDelim, conf.StrTitleSelect, -1, false, false); //jump back a delim
                else
                    conf.StrTitleSelect = "";
                if (GetFilmList(SelItem) == false) // if single folder then call this func to jump back again
                    return GetPrevFilmList();
            }
            return true;
        }

        /// <summary>Sets StrFilmSelect up based on StrSelect, StrTitleSelect etc... </summary>
        static void SetFilmSelect()
        {
            string s = "";
            Prev_ItemID = -1;
            if (conf.Boolselect)
            {
                string sLabel = conf.Wselectedlabel;
                if ((conf.WStrSort == "Date") || (conf.WStrSort == "DateAdded"))
                    conf.StrSelect = "Date" + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(sLabel).ToShortDateString()) + "*'";
                else
                {
                    if (sLabel == "")
                        conf.StrSelect = conf.WStrSort + " is NULL";
                    else
                        conf.StrSelect = conf.WStrSort + " like '*" + sLabel.Replace("'", "''") + "*'";
                }
                conf.StrTxtSelect = "[" + sLabel + "]";
                conf.StrTitleSelect = "";
                conf.Boolselect = false;
            }
            else
            {
                conf.StrTxtSelect = NewString.NPosLeft(@"\", conf.StrTxtSelect, 1, false, false); //strip old path if any
                if (conf.StrTitleSelect != "") conf.StrTxtSelect += @"\" + conf.StrTitleSelect; // append new path if any
            }

            if (conf.StrSelect != "")
                s = conf.StrSelect + " And ";
            if (conf.StrTitleSelect != "") //' in blake's seven causes fuckup
                s = s + String.Format("{0} like '{1}{2}*'", conf.StrTitle1.ToString(), conf.StrTitleSelect.Replace("'", "''"), conf.TitleDelim);
            else
                s = s + conf.StrTitle1.ToString() + " not like ''";
            conf.StrFilmSelect = s;
            Log.Debug("MyFilms (SetFilmSelect) - StrFilmSelect: '" + s + "'");
        }


        bool GetFilmList() { return GetFilmList(-1); }
        /// <summary>
        /// Display List of titles that match based on current StrSelect & StrTitleSelect settings
        /// Titles are treated as though they were folder paths, using the delimeter (set in config) as if it were a filepath slash
        /// Once a title is the only item in that virtual subfolder, further path seperators are ignored and treated as regular chars
        /// </summary>
        /// <param name="gSelItem">If a string then a folder match is looked for. If an int the item with this id is looked for. If found the item is then made the currently selected item</param>
        /// <returns>If returns false means is currently displaying a single folder</returns>
        bool GetFilmList<T>(T gSelItem)
        {
            // Disabled because replaced by SpeedLoader
            //ImgLstFilm.SetFileName("#myfilms.picture");
            //ImgLstFilm2.SetFileName("#myfilms.picture");
            SetFilmSelect();
            //Added GlobalFilterList for Trailer & Rating Filters !!!
			// Added ,false from ZebonsMerge
            r = BaseMesFilms.LectureDonn�es(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens,false);
			//r = BaseMesFilms.LectureDonn�es(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, false);
            Log.Debug("MyFilms (GetFilmList) - GlobalFilterString: '" + GlobalFilterString + "'");
            Log.Debug("MyFilms (GetFilmList) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
            Log.Debug("MyFilms (GetFilmList) - conf.StrFilmSelect: '" + conf.StrFilmSelect + "'");
            Log.Debug("MyFilms (GetFilmList) - conf.StrSorta:      '" + conf.StrSorta + "'");
            Log.Debug("MyFilms (GetFilmList) - conf.StrSortSens:   '" + conf.StrSortSens + "'");
            //if (r.Length == 0)
            //{
            //    //GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            //    //dlgOk.ClearAll();
            //    //dlgOk.SetLine(1, GUILocalizeStrings.Get(10798620));
            //    //dlgOk.SetLine(2, GUILocalizeStrings.Get(10798621));
            //    //dlgOk.DoModal(GetID);
            //    //DisplayAllMovies();
            //    GUIControl.HideControl(GetID, 34);
            //    InitMainScreen();
            //}

            int iCnt = 0;
            int DelimCnt = 0, DelimCnt2;
            GUIListItem item = new GUIListItem();
            string sTitle;
            string sFullTitle;
            string sPrevTitle = "";
            string SelItem = gSelItem.ToString();
            int iSelItem = -2;
            if (typeof(T) == typeof(int)) iSelItem = Int32.Parse(SelItem);

            //setlabels
//            TxtSelect.Label = (conf.StrTxtSelect == "") ? " " : conf.StrTxtSelect.Replace(conf.TitleDelim, @"\"); // always show as though folder path using \ regardless what sep is used
            GUIPropertyManager.SetProperty("#myfilms.select", (conf.StrTxtSelect == "") ? " " : conf.StrTxtSelect.Replace(conf.TitleDelim, @"\"));// always show as though folder path using \ regardless what sep is used
            BtnSrtBy.IsAscending = (conf.StrSortSens == " ASC");
            BtnSrtBy.Label = conf.CurrentSortMethod;

            if (conf.StrTitleSelect != "") DelimCnt = NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) + 1; //get num .'s in title
            facadeView.Clear();
            //----------------------------------------------------------------------------------------
            // Load the DataSet.
            int number = -1;
            int wfacadewiew = 0;
            ArrayList w_tableau = new ArrayList();


            //// Guzzi: Testloop for Debugging ...
            //Log.Debug("MyFilms (GuzziPerfCount) - Start");
            //int i = 0;
            //int ii = 0;
            //string strThumbT;
            //foreach (DataRow sr in r)
            //{
            //    sTitle = sr[conf.StrTitle1].ToString();
            //    DelimCnt2 = NewString.PosCount(conf.TitleDelim, sTitle, false);
            //    if (DelimCnt <= DelimCnt2)
            //    {
            //        sTitle = NewString.NPosMid(conf.TitleDelim, sTitle, DelimCnt, DelimCnt + 1, false, false); //get current substring (folder) within path
            //    }
            //    conf.FileImage = conf.StrPathImg + "\\" + sr["Picture"].ToString();
            //    ii = ii + 1;
            //    strThumbT = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, sTitle);
            //    if (!System.IO.File.Exists(conf.FileImage))
            //    {
            //        i = i + 1;
            //    }
            //}
            //Log.Debug("MyFilms (GuzziPerfCount) - End (" + ii.ToString() + " Records with " + i.ToString() + " Lookups)");

            foreach (DataRow sr in r)
            {
                number++;
                if (conf.Boolreturn)//in case of selection by view verify if value correspond excatly to the searched string
                {
                    w_tableau = Search_String(sr[conf.WStrSort].ToString());
                    foreach (object t in w_tableau)
                    {
                        if ((conf.WStrSort == "Date") || (conf.WStrSort == "DateAdded"))
                        {
                            if (string.Format("{0:dd/MM/yyyy}", DateTime.Parse(t.ToString()).ToShortDateString()) == string.Format("{0:dd/MM/yyyy}", DateTime.Parse(conf.Wselectedlabel).ToShortDateString()))
                                goto suite;
                        }
                        else
                        {
                            if (t.ToString().ToLower().Contains(conf.Wselectedlabel.Trim().ToLower()))
                                goto suite;
                        }
                    }
                    goto fin;
                }
            suite:

                sFullTitle = sTitle = sr[conf.StrTitle1].ToString();
                //Log.Debug("MyFilms (GetFilmList) - BuildDisplaylist - FullTitle: '" + sFullTitle + "'");

                DelimCnt2 = NewString.PosCount(conf.TitleDelim, sTitle, false);
                if (DelimCnt <= DelimCnt2)
                {
                    sTitle = NewString.NPosMid(conf.TitleDelim, sTitle, DelimCnt, DelimCnt + 1, false, false); //get current substring (folder) within path
                    sFullTitle = NewString.NPosRight(conf.TitleDelim, sFullTitle, DelimCnt, false, false); //current rest of path (if only 1 entry in subfolders will present entry ignoring folders)
                }

                if ((iCnt > 0) && (DelimCnt < DelimCnt2) && (sTitle == sPrevTitle)) // don't stack items already at lowest folder level
                {
                    iCnt++;
                    item.Label2 = "(" + iCnt.ToString() + ")  " + NewString.PosRight(")  ", item.Label2);// prepend (items in folder count)
                    if (iCnt == 2)
                    {
                        item.Label = sTitle; //reset to current single folder as > 1 entries
                        item.IsFolder = true;

                        item.ThumbnailImage = conf.FileImage;
                        item.IconImage = conf.FileImage;
                    }
                }
                else
                {
                    iCnt = 1;
                    item = new GUIListItem();
                    item.Label = sFullTitle; // Set = full subfolders path initially
                    if (!MesFilms.conf.OnlyTitleList)
                    {
                        switch (conf.StrSorta)
                        {
                            case "TranslatedTitle":
                            case "OriginalTitle":
                            case "FormattedTitle":
                                item.Label2 = sr["Year"].ToString();
                                break;
                            case "YEAR":
                                item.Label2 = sr["Year"].ToString();
                                break;
                            case "DateAdded":
                                try
                                {
                                    item.Label2 = sr["Date"].ToString();
                                }
                                catch
                                {
                                }
                                break;
                            case "RATING":
                                item.Label2 = sr["Rating"].ToString();
                                break;
                            default:
                                if (conf.StrSorta == conf.StrSTitle)
                                    item.Label2 = sr["Year"].ToString();
                                else
                                    item.Label2 = sr[conf.StrSorta].ToString();
                                break;
                        }
                    }
                    if (sr["Checked"].ToString().ToLower() == "true")
                        item.IsPlayed = true;
                    if (MesFilms.conf.StrSuppress && MesFilms.conf.StrSuppressField.Length > 0)
                        if ((sr[MesFilms.conf.StrSuppressField].ToString() == MesFilms.conf.StrSuppressValue.ToString()) && (MesFilms.conf.StrSupPlayer))
                            item.IsPlayed = true;
                    if (sr["Picture"].ToString().Length > 0)
                    {
                        if ((sr["Picture"].ToString().IndexOf(":\\") == -1) && (sr["Picture"].ToString().Substring(0, 2) != "\\\\"))
                            conf.FileImage = conf.StrPathImg + "\\" + sr["Picture"].ToString();
                        else
                            conf.FileImage = sr["Picture"].ToString();
                    }
                    else
                        conf.FileImage = "";
                    string strThumb = string.Empty;
                    if (!System.IO.File.Exists(conf.FileImage))
                    {
                        
                        string strlabel = item.Label;
                        MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(ref strlabel);
                        conf.FileImage = conf.DefaultCover;
                        if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others"))
                            System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others");
                        strThumb = Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others\\" + strlabel;
                        conf.FileImage = strThumb + ".png"; 
                        if (!System.IO.File.Exists(strThumb + ".png"))
                        {
                            try
                            {
                                if (conf.StrPathViews.Length > 0)
                                    if (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\")
                                        Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                    else
                                        Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                if (!System.IO.File.Exists(strThumb + ".png"))
                                    if (MesFilms.conf.StrViewsDflt && System.IO.File.Exists(MesFilms.conf.DefaultCover))
                                        ImageFast.CreateImage(strThumb + ".png", item.Label);
                                conf.FileImage = strThumb + ".png"; 
                            }
                            catch
                            {
                                conf.FileImage = string.Empty;
                            }
                        }
                    }
                    item.ThumbnailImage = conf.FileImage;
                    strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, sTitle);
                    //Guzzi: Added for Debugging DeDa Changes of MyVideos...
                    //Log.Debug("MyFilms (GetFilmList) : strThumb: '" + strThumb + "'");
                    if ((!System.IO.File.Exists(strThumb)) && (conf.FileImage != conf.DefaultCover))
                        Picture.CreateThumbnail(conf.FileImage, strThumb, 100, 150, 0, Thumbs.SpeedThumbsSmall);

                    item.IconImage = strThumb;
                    item.ItemId = number;
                    item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                    facadeView.Add(item);

                    if (iSelItem == -2) //set selected item = passed in string?
                    {
                        if (sTitle == SelItem)
                        {
                            wfacadewiew = facadeView.Count - 1; //test if this item is one to select
                        }
                    }
                }
                if (iSelItem >= 0) //-1 = ignore, >=0 = itemID to locate (test every item in case item is from within a folder)
                {
                    if (!(conf.StrTIndex.Length > 0))
                    {
                        if (number == iSelItem)
                        {
                            wfacadewiew = facadeView.Count - 1; //test if this item is one to select
                        }
                    }
                    else
                    {
                        if ((number == iSelItem) && (sFullTitle == conf.StrTIndex))
                        {
                         wfacadewiew = facadeView.Count - 1; //test if this item is one to select
                        }
                    }
                }

                sPrevTitle = sTitle;
            fin: ;
            }
            if (facadeView.Count == 0)
            {
                ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798639));
                GUIWaitCursor.Show();
                DisplayAllMovies();
                GUIControl.HideControl(GetID, 34);
                InitMainScreen();
                // Old line:
                GUIControl.ShowControl(GetID, 34);
            }
            else
            {
                //ImgFanart.SetVisibleCondition(1, true);  ->> This fucked up the fanart swapper !!!!!
                if (!backdrop.Active) backdrop.Active = true;
                GUIControl.HideControl(GetID, 34);
            }
            GUIPropertyManager.SetProperty("#myfilms.nbobjects", facadeView.Count.ToString() + " " + GUILocalizeStrings.Get(127));
            GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, (int)wfacadewiew);
            if (facadeView.Count == 1 && item.IsFolder)
            {
                conf.Boolreturn = false;
                conf.Wselectedlabel = item.Label;
            }
            return !(facadeView.Count == 1 && item.IsFolder); //ret false if single folder found
        }


        //----------------------------------------------------------------------------------------
        //    Display rating (Hide/show Star Images)
        //    altered to add half stars
        //   0-0.4=(0)=0s  | 0.5-1.4=(1)=0.5s | 1.5-2.4=(2)=1s   | 2.5-3.4=(3)=1.5s 
        // 3.5-4.4=(4)=2s  | 4.5-5.4=(5)=2.5s | 5.5-6.4=(6)=3s   | 6.5-7.4=(7)=3.5s
        // 7.5-8.4=(8)=4s  | 8.5-9.4=(9)=4.5s | 9.5-10=(10)=5s
        //----------------------------------------------------------------------------------------
        private void affichage_rating(decimal rating)
        {
            int r, i;
            r = Decimal.ToInt32(Decimal.Round(rating, MidpointRounding.AwayFromZero)); // by setting rating here can easily modify for diff effect
            if (r > 10) r = 10;
            for (i = 0; i < 10; i++)
            {
                if (r > i)
                    GUIControl.ShowControl(GetID, 24 + i);
                else
                    GUIControl.HideControl(GetID, 24 + i);
            }
        }

        //----------------------------------------------------------------------------------------
        //    Display Detailed Info (Image, Description, Year, Category)
        //----------------------------------------------------------------------------------------
        private void affichage_Lstdetail(int ItemId, bool wrep, string wlabel)//wrep = false display only image
        //public void affichage_Lstdetail(int ItemId, bool wrep, string wlabel)//wrep = false display only image
        {
            //if (facadeView.SelectedListItem.ItemId == Prev_ItemID)
            //    return;
            Log.Debug("MyFilms (affichage_Lstdetail): ItemId = " + ItemId.ToString() + ", wrep = " + wrep.ToString() + ", wlabel = " + wlabel);
            if (ItemId == -1)
            {
                // reinit some fields
                return;
            }
            if ((facadeView.SelectedListItem.IsFolder) && (MesFilms.conf.Boolselect))
            {
                Log.Debug("MyFilms (affichage_Lstdetail): Item is Folder and BoolSelect is true");
                string[] wfanart = MesFilmsDetail.Search_Fanart(wlabel, true, "file", true, facadeView.SelectedListItem.ThumbnailImage.ToString(), facadeView.SelectedListItem.Path);
                if (wfanart[0] == " ")
                {
                    backdrop.Active = false;
                    GUIControl.HideControl(GetID, 35);
                    Log.Debug("MyFilm (affichage_Lstdetail): INACTIVE backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                }
                else
                {
                    backdrop.Active = true;
                    GUIControl.ShowControl(GetID, 35);
                    Log.Debug("MyFilm (affichage_Lstdetail): ACTIVE backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                }
                Log.Debug("MyFilm (affichage_Lstdetail): backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                backdrop.Filename = wfanart[0];

                //try    
                //    { CurrentFanartDir  = System.IO.Path.GetDirectoryName(wfanart[0]); }
                //catch
                //    { CurrentFanartDir = ""; }
                //Log.Debug("MyFilmsDetails (SearchtrailerLocal) Set CurrentFanartDir to : '" + CurrentFanartDir.ToString() + "'");

                cover.Filename = facadeView.SelectedListItem.ThumbnailImage.ToString();
                if (!backdrop.Active)
                    backdrop.Active = true;
                GUIControl.HideControl(GetID, 34);
                Prev_ItemID = facadeView.SelectedListItem.ItemId;
                // Disabled because replaced by SpeedLoader
                //GUIPropertyManager.SetProperty("#myfilms.picture", facadeView.SelectedListItem.ThumbnailImage.ToString()); 
                affichage_rating(0);
                GUIPropertyManager.SetProperty("#myfilms.logos_id2001", " ");
                GUIPropertyManager.SetProperty("#myfilms.logos_id2002", " ");
                //               return;
            }
            else
            {
                Log.Debug("MyFilms (affichage_Lstdetail): Item is Movie itself!");
                
                //ImageSwapper backdrop = new ImageSwapper();
                string[] wfanart = new string[2];
                wfanart = MesFilmsDetail.Search_Fanart(wlabel, true, "file", false, facadeView.SelectedListItem.ThumbnailImage.ToString(), string.Empty);
                Log.Debug("MyFilm (affichage_Lstdetail): Backdrops-File: wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                //if (wfanart[0] == " ")
                //{
                //    wfanart = MesFilmsDetail.Search_Fanart(wlabel, true, "dir", true, facadeView.SelectedListItem.ThumbnailImage.ToString(), facadeView.SelectedListItem.Path);
                // }
                if (wfanart[0] == " ")
                    {
                        if (backdrop.Active)
                            backdrop.Active = false;
                        if (!cover.Active)
                            cover.Active = true;
                        GUIControl.HideControl(GetID, 35);
                        Log.Debug("MyFilm (affichage_Lstdetail): Fanart-Status: '" + backdrop.Active + "', ' - ControlID35: '" + GUIControl.IsVisibleProperty.ToString()  + "'");    
                    }
                else
                    {
                        if (!backdrop.Active)
                            backdrop.Active = true;
                        if (!cover.Active)
                            cover.Active = true;
                        GUIControl.ShowControl(GetID, 35);
                        Log.Debug("MyFilm (affichage_Lstdetail): Fanart-Status: '" + backdrop.Active + "', ' - ControlID35: '" + GUIControl.IsVisibleProperty.ToString() + "'");
                    }
                Log.Debug("MyFilm (affichage_Lstdetail): Backdrops-File: backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                backdrop.Filename = wfanart[0];
                if (facadeView.SelectedListItem.IsFolder)
                    Prev_ItemID = facadeView.SelectedListItem.ItemId;
                Prev_ItemID = facadeView.SelectedListItem.ItemId;
                conf.FileImage = facadeView.SelectedListItem.ThumbnailImage.ToString();
                // Disabled because of SpeedThumbs
                //GUIPropertyManager.SetProperty("#myfilms.picture", MesFilms.conf.FileImage.ToString());
                cover.Filename = MesFilms.conf.FileImage.ToString();
                // Added to immediately change Fanart - activate to enable timer and reset it !
                //m_FanartTimer.Change(0, 10000); // 10000 = 10 sek.

                XmlConfig XmlConfig = new XmlConfig();
                string logo_type = string.Empty;
                string wlogos = string.Empty;
                Log.Debug("MyFilms : using Logos " + conf.StrLogos.ToString());
                if ((ImgID2001 != null) && (ImgID2002 != null) && (conf.StrLogos))
                {
                    if ((ImgID2001.XPosition == ImgID2002.XPosition) && (ImgID2001.YPosition == ImgID2002.YPosition))
                    {
                        logo_type = "ID2003";
                        try
                        {
                            wlogos = Logos.Build_Logos(r[ItemId], logo_type, Math.Max(ImgID2001.Height, ImgID2002.Height), Math.Max(ImgID2001.Width, ImgID2002.Width), ImgID2001.XPosition, ImgID2001.YPosition, 1, GetID);
                        }
                        catch
                        {
                        }
                        GUIControl.ClearControl(GetID, (int)Controls.CTRL_logos_id2001);
                        Log.Debug("MyFilms : Logo thumb assigned : " + wlogos);
                        if (wlogos.Length == 0)
                            wlogos = " ";
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2001", wlogos);
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2002", " ");
                        //ImgID2001.DoUpdate();
                        GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
                    }
                }
                else
                {
                    if ((ImgID2001 != null) && (conf.StrLogos))
                    {
                        logo_type = "ID2001";
                        try
                        {
                            wlogos = Logos.Build_Logos(r[ItemId], logo_type, ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, 1, GetID);
                        }
                        catch
                        {
                        }
                        if (wlogos.Length == 0)
                            wlogos = " ";
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2001", wlogos);
                        //ImgID2001.DoUpdate();
                        GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
                    }
                    if ((ImgID2002 != null) && (conf.StrLogos))
                    {
                        logo_type = "ID2002";
                        try
                        {
                            wlogos = Logos.Build_Logos(r[ItemId], logo_type, ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, 1, GetID);
                        }
                        catch
                        {
                        }
                        if (wlogos.Length == 0)
                            wlogos = " ";
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2002", wlogos);
                        GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2002);
                    }
                    if (wlogos.Length == 0)
                    {
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2001", " ");
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2002", " ");
                    }
                }
            }

            MesFilmsDetail.Load_Detailed_DB(ItemId, wrep);
            affichage_rating(conf.W_rating);

        }

        //-------------------------------------------------------------------------------------------
        //  Control search Text : no specials characters only alphanumerics
        //-------------------------------------------------------------------------------------------        
        private static bool control_searchText(string stext)
        {
            Regex maRegexp = new Regex("^[^*]+$");
            bool regOK = maRegexp.IsMatch(stext);
            return regOK;
        }
        //-------------------------------------------------------------------------------------------
        //  Proc for Sort Button
        //-------------------------------------------------------------------------------------------        
        void SortChanged(object sender, SortEventArgs e)
        {
            if (e.Order.ToString().Substring(0, 3).ToLower() == conf.StrSortSens.Substring(1,3).ToLower())
                return;
            if (BtnSrtBy.IsAscending)
                conf.StrSortSens = " ASC";
            else
                conf.StrSortSens = " DESC";
            if (!conf.Boolselect)
                GetFilmList();
            else
                getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.StrSortSens, conf.Wstar, true, "");
            return;
        }

        private void item_OnItemSelected(GUIListItem item, GUIControl parent)
        {
            GUIFilmstripControl filmstrip = parent as GUIFilmstripControl;
            //if (filmstrip == null) return;
            //filmstrip.InfoImageFileName = item.ThumbnailImage;
            if (filmstrip != null)
                filmstrip.InfoImageFileName = item.ThumbnailImage;
            if (!(conf.Boolselect || (facadeView.SelectedListItemIndex > -1 && facadeView.SelectedListItem.IsFolder))) //xxxx
            {
                if (facadeView.SelectedListItemIndex > -1)
                    affichage_Lstdetail(facadeView.SelectedListItem.ItemId, true, facadeView.SelectedListItem.Label);
            }
            else
            {
                if (facadeView.SelectedListItemIndex > -1 && !conf.Boolselect)
                    affichage_Lstdetail(facadeView.SelectedListItem.ItemId, false, facadeView.SelectedListItem.Label);
                else
                {
                    affichage_Lstdetail(facadeView.SelectedListItem.ItemId, false, facadeView.SelectedListItem.Label);
                    GUIControl.ShowControl(GetID, 34);
                    affichage_rating(0);
                }
            } 
            //affichage_Lstdetail(item.ItemId, true, item.Label);
        }

        #region Acc�s Donn�es

        //--------------------------------------------------------------------------------------------
        //  Select View for Video
        //--------------------------------------------------------------------------------------------
        private void Selection_type_Video()
        {
            GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg1 == null) return;
            dlg1.Reset();
            dlg1.SetHeading(GUILocalizeStrings.Get(924)); // menu
            dlg1.Add(GUILocalizeStrings.Get(342));//videos
            dlg1.Add(GUILocalizeStrings.Get(345));//year
            dlg1.Add(GUILocalizeStrings.Get(135));//genre
            dlg1.Add(GUILocalizeStrings.Get(200026));//pays
            System.Collections.Generic.List<string> choiceView = new System.Collections.Generic.List<string>();
            choiceView.Add("all");
            choiceView.Add("year");
            choiceView.Add("category");
            choiceView.Add("country");
            if (!(conf.StrStorage.Length == 0) && !(conf.StrStorage == "(none)"))
            {
                dlg1.Add(GUILocalizeStrings.Get(154) + " " + GUILocalizeStrings.Get(1951));//storage
                choiceView.Add("storage");
            }

            for (int i = 0; i < 5; i++)
            {
                if (!(conf.StrViewItem[i] == null) && !(conf.StrViewItem[i] == "(none)") && (conf.StrViewItem[i].Length > 0))
                {
                    choiceView.Add(string.Format("view{0}",i.ToString()));
                    if ((conf.StrViewText[i] == null) || (conf.StrViewText[i].Length == 0))
                        dlg1.Add(conf.StrViewItem[i]);   // specific user View1
                    else
                        dlg1.Add(conf.StrViewText[i]);   // specific Text for View1
                }
            }
            dlg1.DoModal(GetID);

            if (dlg1.SelectedLabel == -1)
            {
                return;
            }
            Change_view(choiceView[dlg1.SelectedLabel].ToLower());
            return;
        }
        
        //--------------------------------------------------------------------------------------------
        //  Select Option
        //--------------------------------------------------------------------------------------------
        private void Selection_type_Option()
        {
            GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg1 == null) return;
            System.Collections.Generic.List<string> choiceView = new System.Collections.Generic.List<string>();
            dlg1.Reset();
            dlg1.SetHeading(GUILocalizeStrings.Get(924)); // menu
            if (Configuration.NbConfig > 1)
            {
                dlg1.Add(GUILocalizeStrings.Get(6022));   // Change Config 
                //dlg1.Add(GUILocalizeStrings.Get(6029) + " " + GUILocalizeStrings.Get(6022));   // Change Config 
                choiceView.Add("config");
            }

            if (MesFilms.conf.StrCheckWOLenable) // Show current NAS server status
            {
                dlg1.Add(GUILocalizeStrings.Get(10798727));   // Show NAS Server Status
                choiceView.Add("nasstatus");
            }

            // Add Submenu for Global Settings 
            dlg1.Add(string.Format(GUILocalizeStrings.Get(10798689)));
            choiceView.Add("globaloptions");

            // Add Submenu for Global Updates
            dlg1.Add(string.Format(GUILocalizeStrings.Get(10798690)));
            choiceView.Add("globalupdates");

            dlg1.DoModal(GetID);

            if (dlg1.SelectedLabel == -1)
            {
                return;
            }
            Change_view(choiceView[dlg1.SelectedLabel].ToLower());
            return;
        }

        public static ArrayList Search_String(string champselect)
        {
            Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
            System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
            foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
            {
                Regex oRegexReplace = new Regex("[,;]");
                champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value,""));
             }
            ArrayList wtab = new ArrayList();

            int wi = 0;
            string[] Sep = conf.ListSeparator;
            string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
            string wzone = string.Empty;
            for (wi = 0; wi < arSplit.Length; wi++)
            {
                if (arSplit[wi].Length > 0)
                {
                    wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim());
                    for (int i = 0; i <= 4; i++)
                    {
                        if (conf.RoleSeparator[i].Length > 0)
                        {
                            if (wzone.Trim().IndexOf(conf.RoleSeparator[i]) == wzone.Trim().Length - 1)
                            {
                                wzone = string.Empty;
                                break;
                            }
                            if (wzone.Trim().IndexOf(conf.RoleSeparator[i]) > 1 && wzone.Trim().IndexOf(conf.RoleSeparator[i]) < wzone.Trim().Length)
                            {
                                wzone = wzone.Substring(0, wzone.IndexOf(conf.RoleSeparator[i])).Trim();
                            }
                        }
                    }
                    if (wzone.Length > 0)
                        wtab.Add(wzone.Trim());
                    wzone = string.Empty;
                }
            }
            return wtab;
        }

        public static ArrayList SubItemGrabbing(string champselect)
        {
            Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
            System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
            foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
            {
                Regex oRegexReplace = new Regex("[,;]");
                champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
            }
            ArrayList wtab = new ArrayList();

            int wi = 0;
            string[] Sep = conf.ListSeparator;
            string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
            string wzone = string.Empty;
            for (wi = 0; wi < arSplit.Length; wi++)
            {
                if (arSplit[wi].Length > 0)
                {
                    wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim());
                    for (int i = 0; i <= 4; i++)
                    {
                        if (conf.RoleSeparator[i].Length > 0)
                        {
                            if (wzone.Trim().IndexOf(conf.RoleSeparator[i]) == wzone.Trim().Length - 1)
                            {
                                wzone = string.Empty;
                                break;
                            }
                            if (wzone.Trim().IndexOf(conf.RoleSeparator[i]) > 1 && wzone.Trim().IndexOf(conf.RoleSeparator[i]) < wzone.Trim().Length)
                            {
                                wzone = wzone.Substring(0, wzone.IndexOf(conf.RoleSeparator[i])).Trim();
                            }
                        }
                    }
                    if (wzone.Length > 0)
                        wtab.Add(wzone.Trim());
                    wzone = string.Empty;
                }
            }
            return wtab;
        }

        public static ArrayList SubTitleGrabbing(string champselect)
        {
            Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
            System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
            foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
            {
                Regex oRegexReplace = new Regex("[,;]");
                champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
            }
            ArrayList wtab = new ArrayList();

            int wi = 0;
            string[] Sep = {" - ", ":"}; //Only Dash as Separator for Movietitles !!!
            //string[] CleanerList = new string[] { "Der ", "der ", "Die ", "die ", "Das ", "das", "des", " so", "sich", " a ", " A ", "The ", "the ","- "," -"," AT "};
            string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
            string wzone = string.Empty;
            for (wi = 0; wi < arSplit.Length; wi++)
            {
                if (arSplit[wi].Length > 0)
                {
                    wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim());
                    for (int i = 0; i <= 4; i++)
                    {
                        if (conf.RoleSeparator[i].Length > 0)
                        {
                            if (wzone.Trim().IndexOf(conf.RoleSeparator[i]) == wzone.Trim().Length - 1)
                            {
                                wzone = string.Empty;
                                break;
                            }
                            if (wzone.Trim().IndexOf(conf.RoleSeparator[i]) > 1 && wzone.Trim().IndexOf(conf.RoleSeparator[i]) < wzone.Trim().Length)
                            {
                                wzone = wzone.Substring(0, wzone.IndexOf(conf.RoleSeparator[i])).Trim();
                            }
                        }
                    }
                    if (wzone.Length > 0)
                        wtab.Add(wzone.Trim());
                    wzone = string.Empty;
                }
            }
            return wtab;
        }

        public static ArrayList SubWordGrabbing(string champselect, int minchars, bool filter)
        {
            Log.Debug("MyFilms (SubWordGrabbing): InputString: '" + champselect + "'"); 
            Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
            System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
            foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
            {
                Regex oRegexReplace = new Regex("[,;]");
                champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
                Log.Debug("MyFilms (SubWordGrabbing): RegExReplace: '" + champselect + "'");
            }

            string[] CleanerList = new string[] { "Der ", "der ", "Die ", "die ", "Das ", "das", "des", " so", "sich", " a ", " A ", "The ", "the ","- "," -"," AT ", "in "};
            int i = 0;
            for (i = 0; i <  13; i++)
            {
                if ((CleanerList[i].Length > 0) && (filter = true))
                {
                    champselect = champselect.Replace(CleanerList[i], " ");
                    //Log.Debug("MyFilms (SubWordGrabbing): CleanerListItem: '" + CleanerList[i] + "'");
                }
            }
            
            ArrayList wtab = new ArrayList();

            int wi = 0;
            //string[] Sep = conf.ListSeparator;
            string[] Sep = new string[]
                {" ",",",";","|","/","(",")",".",@"\",":"};
            string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
            string wzone = string.Empty;
            for (wi = 0; wi < arSplit.Length; wi++)
            {
                if (arSplit[wi].Length > 0)
                {
                    wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim());
                    Log.Debug("MyFilms (SubWordGrabbing): wzone: '" + wzone + "'"); 
                    if (wzone.Length >= minchars)//Only words with minimum 4 letters!
                    {
                        wtab.Add(wzone.Trim());
                        Log.Debug("MyFilms (SubWordGrabbing): AddWordToList: '" + wzone.Trim() + "'");
                    }
                    wzone = string.Empty;
                }
            }
            return wtab;
        }


        /// <summary>Selects records for display grouping them as required</summary>
        /// <param name="WstrSelect">Select this kind of records</param>
        /// <param name="WStrSort">Sort based on this</param>
        /// <param name="WStrSortSens">Asc/Desc. Ascending or descending sort order</param>
        /// <param name="NewWstar">Entries must contain this string to be included</param>
        /// <param name="ClearIndex">Reset Selected Item Index</param>
        /// <param name="SelItem">Select entry matching this string if not empty</param>
        /// 
        //Example for Actors list:
            //conf.WStrSort = "ACTORS";
            //conf.Wselectedlabel = "";
            //conf.WStrSortSens = " ASC";
            //BtnSrtBy.IsAscending = true;
            //conf.StrActors = keyboard.Text;
            //getSelectFromDivx("Actors like '*" + keyboard.Text + "*'", conf.WStrSort, conf.WStrSortSens, keyboard.Text, true, "");

        public void getSelectFromDivx(string WstrSelect, string WStrSort, string WStrSortSens, string NewWstar, bool ClearIndex, string SelItem)
        {
            GUIListItem item = new GUIListItem();
            Prev_ItemID = -1;
            string champselect = "";
            string wchampselect = "";
            ArrayList w_tableau = new ArrayList();
            int Wnb_enr = 0;

            conf.Wstar = NewWstar;
            BtnSrtBy.Label = GUILocalizeStrings.Get(103);
            conf.Boolselect = true;
            conf.Wselectedlabel = "";
            if (ClearIndex) 
                conf.StrIndex = 0;
            Change_LayOut(0); 
            facadeView.Clear();
            int wi = 0;
            Log.Debug("MyFilms (GetSelectFromDivx) - GlobalFilterString: '" + GlobalFilterString + "'");
            Log.Debug("MyFilms (GetSelectFromDivx) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
            Log.Debug("MyFilms (GetSelectFromDivx) - WstrSelect        : '" + WstrSelect + "'");
            Log.Debug("MyFilms (GetSelectFromDivx) - WStrSort          : '" + WStrSort + "'");
            Log.Debug("MyFilms (GetSelectFromDivx) - WStrSortSens      : '" + WStrSortSens + "'");
            Log.Debug("MyFilms (GetSelectFromDivx) - NewWstar          : '" + NewWstar + "'");

            Log.Debug("MyFilms (GetSelectFromDivx) - Setup Array Started (LectureDonn�es)");
            foreach (DataRow enr in BaseMesFilms.LectureDonn�es(GlobalFilterString + " " + conf.StrDfltSelect, WstrSelect, WStrSort, WStrSortSens))
                {
                if ((WStrSort == "Date") || (WStrSort == "DateAdded"))
                    champselect = string.Format("{0:yyyy/MM/dd}", enr["DateAdded"]);
                else
                    champselect = enr[WStrSort].ToString().Trim();
                ArrayList wtab = Search_String(champselect);
                //string wAddList = "";
                //if ((NewWstar.Length < 3) && ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director"))))
                if ((NewWstar.Length < 0) && ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director"))))
                    for (wi = 0; wi < wtab.Count; wi++)
                        {
                        //wAddList = wtab[wi].ToString().Trim().Substring(1,1) + "\\" + wtab[wi].ToString().Trim().Substring(2);
                        if (wtab[wi].ToString().Trim().Length > 1)
                            //w_tableau.Add(wtab[wi].ToString().Trim().Substring(0, 1) + @"\" + wtab[wi].ToString().Trim().Substring(1));
                            w_tableau.Add(wtab[wi].ToString().Trim().Substring(0,1) + @"\" + wtab[wi].ToString().Trim());
                        else
                            w_tableau.Add(wtab[wi].ToString().Trim());
                        }
                else
                    for (wi = 0; wi < wtab.Count; wi++)
                    {
                        w_tableau.Add(wtab[wi].ToString().Trim());
                    }
                }

            Log.Debug("MyFilms (GetSelectFromDivx) - Setup Array Finished (LectureDonn�es)");
            if (WStrSortSens == " ASC")
                w_tableau.Sort(0, w_tableau.Count, null);
            else
            {
                IComparer myComparer = new myReverserClass();
                w_tableau.Sort(0, w_tableau.Count, myComparer);
            }

            Log.Debug("MyFilms (GetSelectFromDivx) - Sorting Finished");
            item = new GUIListItem();

            Log.Debug("MyFilms (GetSelectFromDivx) - Facadesetup Started");

            if (MesFilms.conf.StrViews) // Check if Thumbs directories exist or create them
            {
                if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others")) System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others");
                if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Artist")) System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Artist");
            }

            string strThumbDirectory;
            if ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director")))
                strThumbDirectory = Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Artist\\";
            else
                strThumbDirectory = Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others\\";

            for (wi = 0; wi != w_tableau.Count; wi++)
            {
                champselect = w_tableau[wi].ToString();
                if (string.Compare(champselect.ToString(), wchampselect.ToString(), true) == 0)
                    Wnb_enr++;
                else
                {
                    if (conf.Wstar == "*" || champselect.ToUpper().Contains(conf.Wstar.ToUpper()))
                    {
                        if ((Wnb_enr > 0) && (wchampselect.Length > 0))
                        {
                            item = new GUIListItem();
                            item.Label = wchampselect.ToString();
                            item.Label2 =  Wnb_enr.ToString();
                            if ((MesFilms.conf.StrViews) && ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) 
                                || (WStrSort.ToLower().Contains("director")) || (WStrSort.ToLower().Contains("category") || WStrSort.ToLower().Contains("year") || WStrSort.ToLower().Contains("country"))))
                            {

                                string strThumb;
                                string strThumbLarge = "";
                                string strThumbSource = "";
                                if ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director")))
                                {
                                    strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieActors, item.Label);
                                    strThumbLarge = strThumbDirectory + item.Label + ".png";
                                }
                                else
                                {
                                    strThumb = strThumbDirectory + item.Label + ".png";
                                    strThumbLarge = strThumbDirectory + item.Label + ".png";
                                }

                                if (!System.IO.File.Exists(strThumb))
                                {
                                    if ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director")))
                                    {
                                        if (conf.StrPathArtist.Length > 0)
                                            if (conf.StrPathArtist.Substring(conf.StrPathArtist.Length - 1) == "\\")
                                            {
                                                if (System.IO.File.Exists(conf.StrPathArtist + item.Label + "\\folder.jpg"))
                                                    strThumbSource = conf.StrPathArtist + item.Label + "\\folder.jpg";
                                                else
                                                    if (System.IO.File.Exists(conf.StrPathArtist + item.Label + "\\folder.png"))
                                                        strThumbSource = conf.StrPathArtist + item.Label + "\\folder.png";
                                            }
                                            else
                                            {
                                                if (System.IO.File.Exists(conf.StrPathArtist + "\\" + item.Label + "\\folder.jpg"))
                                                    strThumbSource = conf.StrPathArtist + "\\" + item.Label + "\\folder.jpg";
                                                else
                                                    if (System.IO.File.Exists(conf.StrPathArtist + "\\" + item.Label + "\\folder.png"))
                                                        strThumbSource = conf.StrPathArtist + "\\" + item.Label + "\\folder.png";
                                            }
                                        if ((!System.IO.File.Exists(strThumb)) && (strThumb != conf.DefaultCoverArtist) && (strThumbSource != ""))
                                        {
                                            Picture.CreateThumbnail(strThumbSource, strThumb, 100, 150, 0, Thumbs.SpeedThumbsSmall);
                                            Picture.CreateThumbnail(strThumbSource,strThumbDirectory + item.Label + ".png",400,600,0,Thumbs.SpeedThumbsLarge);
                                            strThumbLarge = strThumbDirectory + item.Label + ".png";
                                        }

                                        if (!System.IO.File.Exists(strThumbLarge))
                                        {
                                            // ImageFast.CreateImage(strThumb, item.Label);
                                            strThumb = conf.DefaultCoverArtist;
                                            strThumbLarge = conf.DefaultCoverArtist;
                                        }

                                        //if (!System.IO.File.Exists(strThumb + ".png"))
                                        //    if (System.IO.File.Exists(MesFilms.conf.DefaultCoverArtist))
                                        //        ImageFast.CreateImage(strThumb + ".png", item.Label);
                                    }
                                    else if ((WStrSort.ToLower().Contains("country")) || (WStrSort.ToLower().Contains("category")) || (WStrSort.ToLower().Contains("year")))
                                    {
                                        if (conf.StrPathViews.Length > 0)
                                            if (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\")
                                            {
                                                if (System.IO.File.Exists(conf.StrPathViews + item.Label + ".jpg"))
                                                    Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".jpg", strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                                else
                                                    if (System.IO.File.Exists(conf.StrPathViews + item.Label + ".png"))
                                                        Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".png", strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                            }
                                            else
                                            {
                                                if (System.IO.File.Exists(conf.StrPathViews + "\\" + item.Label + ".jpg"))
                                                    Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".jpg", strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                                else
                                                    if (System.IO.File.Exists(conf.StrPathViews + "\\" + item.Label + ".png"))
                                                        Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".png", strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                            }
                                        //if (!System.IO.File.Exists(strThumb + ".png"))
                                        //    if (MesFilms.conf.StrViewsDflt && System.IO.File.Exists(MesFilms.conf.DefaultCover))
                                        //        ImageFast.CreateImage(strThumb + ".png", item.Label);
                                    }

                                }
                                item.ThumbnailImage = strThumbLarge;
                                item.IconImage = strThumb;
                            }
                            string[] wfanart;
                            if (WStrSort.ToLower() == "category" || WStrSort.ToLower() == "year" || WStrSort.ToLower() == "country")
                                wfanart = MesFilmsDetail.Search_Fanart(item.Label, true, "file", true, item.ThumbnailImage, WStrSort.ToLower());
                            item.IsFolder = true;
                            item.Path = WStrSort.ToLower();
                            item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                            facadeView.Add(item);
                            if (SelItem != "" && item.Label == SelItem) conf.StrIndex = facadeView.Count - 1; //test if this item is one to select
                        }
                        Wnb_enr = 1;
                        wchampselect = champselect.ToString();
                    }
                }
            }
            Log.Debug("MyFilms (GetSelectFromDivx) - Facadesetup Finished");

            Log.Debug("MyFilms (GetSelectFromDivx) - Facadesetup Groups Started");
            if ((Wnb_enr > 0) && (wchampselect.Length > 0))
            {
                item = new GUIListItem();
                item.Label = wchampselect.ToString();
                item.Label2 = Wnb_enr.ToString();
                //if (MesFilms.conf.StrViews)
                if ((MesFilms.conf.StrViews) && ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director")) 
                    || (WStrSort.ToLower().Contains("category") || WStrSort.ToLower().Contains("year") || WStrSort.ToLower().Contains("country"))))
                    {
                    string strThumb;
                    string strThumbLarge = "";
                    string strThumbSource = "";
                    if ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director")))
                    {
                        strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieActors, item.Label);
                        strThumbLarge = Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Artist\\" + item.Label + ".png";
                    }
                    else
                        strThumb = Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others\\" + item.Label + ".png";
                    
                    if ((!System.IO.File.Exists(strThumb)) || (!System.IO.File.Exists(strThumbLarge)))
                        {
                            if ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director")))
                            {
                                if (conf.StrPathArtist.Length > 0)
                                    if (conf.StrPathArtist.Substring(conf.StrPathArtist.Length - 1) == "\\")
                                    {
                                        if (System.IO.File.Exists(conf.StrPathArtist + item.Label + "\\folder.jpg"))
                                            strThumbSource = conf.StrPathArtist + item.Label + "\\folder.jpg";
                                        else
                                            if (System.IO.File.Exists(conf.StrPathArtist + item.Label + "\\folder.png"))
                                                strThumbSource = conf.StrPathArtist + item.Label + "\\folder.png";
                                    }
                                    else
                                    {
                                        if (System.IO.File.Exists(conf.StrPathArtist + "\\" + item.Label + "\\folder.jpg"))
                                            strThumbSource = conf.StrPathArtist + "\\" + item.Label + "\\folder.jpg";
                                        else
                                            if (System.IO.File.Exists(conf.StrPathArtist + "\\" + item.Label + "\\folder.png"))
                                                strThumbSource = conf.StrPathArtist + "\\" + item.Label + "\\folder.png";
                                    }
                                if ((!System.IO.File.Exists(strThumb)) && (strThumb != conf.DefaultCoverArtist) && (strThumbSource != ""))
                                {
                                    Picture.CreateThumbnail(strThumbSource, strThumb, 100, 150, 0, Thumbs.SpeedThumbsSmall);
                                    Picture.CreateThumbnail(strThumbSource,strThumbDirectory + item.Label + ".png",400,600,0,Thumbs.SpeedThumbsLarge);
                                    strThumbLarge = strThumbDirectory + item.Label + ".png";
                                }

                                if (!System.IO.File.Exists(strThumbLarge))
                                {
                                    // ImageFast.CreateImage(strThumb, item.Label);
                                    strThumb = conf.DefaultCoverArtist;
                                    strThumbLarge = conf.DefaultCoverArtist;
                                }
                            }
                            else if ((WStrSort.ToLower().Contains("country")) || (WStrSort.ToLower().Contains("category")) || (WStrSort.ToLower().Contains("year")))
                            {
                                if (conf.StrPathViews.Length > 0)
                                    if (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\")
                                    {
                                        if (System.IO.File.Exists(conf.StrPathViews + item.Label + ".jpg"))
                                            Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".jpg", strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                        else
                                            if (System.IO.File.Exists(conf.StrPathViews + item.Label + ".png"))
                                                Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".png", strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                    }
                                    else
                                    {
                                        if (System.IO.File.Exists(conf.StrPathViews + "\\" + item.Label + ".jpg"))
                                            Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".jpg", strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                        else
                                            if (System.IO.File.Exists(conf.StrPathViews + "\\" + item.Label + ".png"))
                                                Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".png", strThumb, 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                    }
                                if (!System.IO.File.Exists(strThumb))
                                    ImageFast.CreateImage(strThumb, item.Label);
                                strThumbLarge = strThumb;
                            }
                        }
                    item.ThumbnailImage = strThumbLarge;
                    item.IconImage = strThumb;
                    }  
                item.IsFolder = true;
                //item.ItemId = number; // Only used in GetFilmList
                item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                facadeView.Add(item);
                if (SelItem != "" && item.Label == SelItem) conf.StrIndex = facadeView.Count - 1; //test if this item is one to select
                Wnb_enr = 0;
            }
            Log.Debug("MyFilms (GetSelectFromDivx) - Facadesetup Groups Finished");

            item.FreeMemory();
            conf.StrTxtSelect = "Selection";
            if (conf.Wstar != "*") conf.StrTxtSelect += " " + GUILocalizeStrings.Get(344) + " [*" + conf.Wstar + "*]";
            GUIPropertyManager.SetProperty("#myfilms.select", conf.StrTxtSelect); 
//            TxtSelect.Label = conf.StrTxtSelect;
            conf.StrSelect = WstrSelect;
            conf.StrFilmSelect = "";

            if ((conf.StrIndex > facadeView.Count - 1) || (conf.StrIndex < 0)) //check index within bounds, will be unless xml file heavily edited
                conf.StrIndex = 0;
            if (facadeView.Count == 0)
            {
                ShowMessageDialog("", GUILocalizeStrings.Get(10798620), GUILocalizeStrings.Get(10798621));
                GUIWaitCursor.Show();
                DisplayAllMovies();
                GetFilmList();
                GUIControl.ShowControl(GetID, 34);
            }
            else
            {
                //ImgFanart.SetVisibleCondition(1, true);  ->> This fucked up the fanart swapper !!!!!
                if (!backdrop.Active) 
                    backdrop.Active = true;
                GUIControl.ShowControl(GetID, 34);
                GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2001);
                GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2002);
                
                //Make a difference between movies and persons -> Load_Detailed_DB or Load_Detailed_PersonInfo
                if ((WStrSort.ToLower().Contains("actors")) || (WStrSort.ToLower().Contains("producer")) || (WStrSort.ToLower().Contains("director")))
                    MesFilmsDetail.Load_Detailed_PersonInfo(0, false);
                else
                    MesFilmsDetail.Load_Detailed_DB(0, false);
                
                // Disabled because replaced by SpeedLoader
                //ImgLstFilm.SetFileName("#myfilms.picture");
                //ImgLstFilm2.SetFileName("#myfilms.picture");
                affichage_rating(0);
            }
            GUIPropertyManager.SetProperty("#myfilms.nbobjects", facadeView.Count.ToString() + " " + GUILocalizeStrings.Get(127));
            MesFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
            GUIWaitCursor.Hide();
            GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, (int)conf.StrIndex);

        }

        //----------------------------------------------------------------------------------------------
        //  Reverse Sort
        //----------------------------------------------------------------------------------------------
        public class myReverserClass : IComparer
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare(Object x, Object y)
            {
                return ((new CaseInsensitiveComparer()).Compare(y, x));
            }
        }

        private static void Load_Config(string CurrentConfig, bool create_temp)
        {
            conf = new Configuration(CurrentConfig, create_temp);
            if ((conf.Boolreturn) && (conf.Wselectedlabel == ""))
            {
                conf.Boolselect = true;
                conf.Boolreturn = false;
            }
            if (conf.StrLogos)
                confLogos = new Logos();
        }

         //--------------------------------------------------------------------------------------------
        //  Initial Windows load. If LoadDfltSlct = true => load default select if any
        //                           LoadDfltSlct = false => return from  MesFilmsDetail
        //--------------------------------------------------------------------------------------------
        private void Fin_Charge_Init(bool LoadDfltSlct, bool reload)
        {
            if (LoadDfltSlct)
            {
                conf.Boolselect = false;
                //Reset GLobal Filters !
                GlobalFilterMinRating = false;
                GlobalFilterTrailersOnly = false;
                GlobalFilterString = ""; // reset global filterstring
                MovieScrobbling = false; // reset scrobbler filter setting
            }
            if (((PreviousWindowId != ID_MesFilmsDetail) && (PreviousWindowId != ID_MesFilmsActors)) || (reload))
            {
                //chargement des films
                BaseMesFilms.LoadFilm(conf.StrFileXml);
                 r = BaseMesFilms.LectureDonn�es(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens);
            }
            //Layout = conf.StrLayOut;
            
            if (string.IsNullOrEmpty(conf.CurrentSortMethod))
                conf.CurrentSortMethod = GUILocalizeStrings.Get(103);
            else
                BtnSrtBy.Label = conf.CurrentSortMethod.ToString();
            string BtnSearchT = GUILocalizeStrings.Get(137);
            GUIButtonControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnSearchT, BtnSearchT);
            BtnSrtBy.SortChanged += new SortEventHandler(SortChanged);
            InitialStart = false; // Guzzi: Set to false after first initialization to be able to return to noninitialized View - Make sure to set true if changing DB config

            if (conf.Boolselect)
            {
                getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.StrSortSens, conf.Wstar, false, ""); // preserve index from last time
            }
            else
            {
                Change_LayOut(MesFilms.conf.StrLayOut);
                if (!(LoadDfltSlct))
                    GetFilmList(conf.StrIndex);
                else
                {
                    if ((conf.StrViewDfltItem.Length == 0) || (conf.StrViewDfltItem == "(none)"))
                    {
                        conf.StrSelect = conf.StrTitle1.ToString() + " not like ''";
//                        TxtSelect.Label = conf.StrTxtSelect = "";
                        conf.StrTxtSelect = "";
                        GUIPropertyManager.SetProperty("#myfilms.select", " ");
                        conf.Boolselect = false;
                        if (conf.StrSortSens == " ASC")
                            BtnSrtBy.IsAscending = true;
                        else
                            BtnSrtBy.IsAscending = false;
                        GetFilmList(conf.StrIndex);
                    }
                    else
                    {
                        if (conf.StrViewDfltText.Length == 0)
                        {
                            if (conf.StrViewDfltItem.ToLower() == "year" || conf.StrViewDfltItem.ToLower() == "category" || conf.StrViewDfltItem.ToLower() == "country" || conf.StrViewDfltItem.ToLower() == "storage")
                                Change_view(conf.StrViewDfltItem.ToLower());
                            else
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    if (conf.StrViewDfltItem == conf.StrViewText[i])
                                        Change_view(string.Format("View{0}", i.ToString()).ToLower());
                                }
                            }
                        }
                        else
                        // View List as selected
                        {
                            string wStrViewDfltItem = conf.StrViewDfltItem.ToLower();
                            for (int i = 0; i < 5; i++)
                            {
                                if (conf.StrViewDfltItem == conf.StrViewText[i])
                                {
                                    wStrViewDfltItem = conf.StrViewItem[i];
                                    break;
                                }
                            } 
                            conf.Boolselect = true;
                            conf.Boolreturn = true;
                            conf.Boolview = true;
                            conf.WStrSort = wStrViewDfltItem;
                            if (wStrViewDfltItem == "DateAdded")
                                conf.StrSelect = "Date" + " like '" + DateTime.Parse(conf.StrViewDfltText).ToShortDateString() + "'";
                            else
                                conf.StrSelect = wStrViewDfltItem + " like '*" + conf.StrViewDfltText + "*'";
//                            TxtSelect.Label = conf.StrTxtSelect = "[" + conf.StrViewDfltText + "]";
                            conf.StrTxtSelect = "[" + conf.StrViewDfltText + "]";
                            GUIPropertyManager.SetProperty("#myfilms.select", conf.StrTxtSelect);
                            BtnSrtBy.Label = conf.CurrentSortMethod;
                            if (conf.StrSortSens == " ASC")
                                BtnSrtBy.IsAscending = true;
                            else
                                BtnSrtBy.IsAscending = false;
                            GetFilmList(conf.StrIndex);
                        }
                    }
                }
            }
            MesFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation); 
            GUIWaitCursor.Hide();
            if (conf.LastID == ID_MesFilmsDetail)
            {
                GUIWindowManager.ActivateWindow(ID_MesFilmsDetail); // if last window in use was detailed one display that one again
            }
            if (conf.LastID == ID_MesFilmsActors)
            {
                GUIWindowManager.ActivateWindow(ID_MesFilmsActors); // if last window in use was actor one display that one again
            }
        }
        //--------------------------------------------------------------------------------------------
        //   Change LayOut 
        //--------------------------------------------------------------------------------------------
        private void Change_LayOut(int wLayOut)
        {
            switch (wLayOut)
            {
                case 1:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(100));
                    facadeView.View = GUIFacadeControl.ViewMode.SmallIcons;
                    break;
                case 2:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(417));
                    facadeView.View = GUIFacadeControl.ViewMode.LargeIcons;
                    break;
                case 3:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(733));
                    facadeView.View = GUIFacadeControl.ViewMode.Filmstrip;
                    break;

                case 4:
                    try
                    {
                        GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(791));
                        //facadeView.View = GUIFacadeControl.ViewMode.Filmstrip;
                        // To be changed when Coverflow is available in CORE Files ....
                        facadeView.View = GUIFacadeControl.ViewMode.CoverFlow;
                        break;
                    }
                    catch 
                    {
                        GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(733));
                        facadeView.View = GUIFacadeControl.ViewMode.Filmstrip;
                        break;
                    };

                default:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(101));
                    facadeView.View = GUIFacadeControl.ViewMode.List;
                    break;
            }
        }
        //--------------------------------------------------------------------------------------------
        //   Change View Response  (and process corresponding filter list)
        //--------------------------------------------------------------------------------------------
        private void Change_view(string choiceView)
        {
            conf.Boolview = false;
            conf.Boolstorage = false;
            XmlConfig XmlConfig = new XmlConfig();
            switch (choiceView)
            {
                case "all":
                    //  Change View All Films
                    conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
                    conf.WStrSort = conf.StrSTitle;
                    conf.Boolselect = false;
                    conf.Boolreturn = false;
                    Log.Debug("MyFilms (Guzzi): Change_View filter - " + "StrSelect: " + conf.StrSelect + " | WStrSort: " + conf.WStrSort);
                    GetFilmList();
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
                case "year":
                    //  Change View by Year
                    conf.WStrSort = "YEAR";
                    conf.WStrSortSens = " DESC";
                    BtnSrtBy.IsAscending = false;
                    getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, "");
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
                case "category":
                //  Change View by  "Category":
                    conf.WStrSort = "CATEGORY";
                    conf.WStrSortSens = " ASC";
                    BtnSrtBy.IsAscending = true;
                    getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, "");
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
 //                   facadeView.Focus = true;
                    
                    break;
                case "country":
                //  Change View by "Country":
                    conf.WStrSort = "COUNTRY";
                    conf.WStrSortSens = " ASC";
                    BtnSrtBy.IsAscending = true;
                    getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, "");
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List); 
                    break;
                case "storage":
                //  Change View by "Storage":
                    conf.StrSelect = "((" + conf.StrTitle1.ToString() + " not like '') and (" + conf.StrStorage.ToString() + " not like ''))";
                    conf.StrTxtSelect = GUILocalizeStrings.Get(154) + " " + GUILocalizeStrings.Get(1951);
//                    TxtSelect.Label = conf.StrTxtSelect;
                    GUIPropertyManager.SetProperty("#myfilms.select", conf.StrTxtSelect);
                    conf.Boolselect = false;
                    conf.Boolreturn = false;
                    conf.Boolview = true;
                    conf.WStrSort = conf.StrSTitle;
                    BtnSrtBy.Label = conf.CurrentSortMethod;
                    if (conf.StrSortSens == " ASC")
                        BtnSrtBy.IsAscending = true;
                    else
                        BtnSrtBy.IsAscending = false;
                    GetFilmList();
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
                case "view0":
                case "view1":
                case "view2":
                case "view3":
                case "view4":
                    // specific user View
                    int i = 0;
                    if (choiceView == "view1")
                        i = 1;
                    if (choiceView == "view2")
                        i = 2;
                    if (choiceView == "view3")
                        i = 3;
                    if (choiceView == "view4")
                        i = 4;

                    GUIWaitCursor.Show();
                    GUIWindowManager.Process(); //Added by hint of Damien to update GUI first ...
                    conf.WStrSort = conf.StrViewItem[i];   
                    conf.WStrSortSens = " ASC";
                    BtnSrtBy.IsAscending = true;
                    if (conf.StrViewValue[i].Length > 0)
                    {
                        conf.Boolview = true;
                        conf.StrTxtSelect = "Selection";
                        conf.Boolselect = true;
                        conf.Wstar = "*";
                        if (conf.Wstar != "*")
                            conf.StrTxtSelect += " " + GUILocalizeStrings.Get(344) + " [*" + conf.Wstar + "*]";
//                        TxtSelect.Label = conf.StrTxtSelect;
                        GUIPropertyManager.SetProperty("#myfilms.select", conf.StrTxtSelect);
                        if (conf.WStrSort == "DateAdded")
                            conf.StrSelect = "Date";
                        else
                            conf.StrSelect = conf.WStrSort;

                        conf.StrFilmSelect = "";
                        conf.Wselectedlabel = conf.StrViewValue[i];
                        conf.Boolreturn = true;
                        do
                        {
                            if (conf.StrTitleSelect != "")
                                conf.StrTitleSelect += conf.TitleDelim;
                            conf.StrTitleSelect += conf.StrViewValue[i];
                        } while (GetFilmList() == false); //keep calling while single folders found
                    }
                    else
                    {
                        if (conf.WStrSort == "DateAdded")
                            getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", "Date", " DESC", "*", true, "");
                        else
                            getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, "");
                    }
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;

                case "config": //Choose Database
                    string newConfig = Configuration.Choice_Config(GetID);
                    newConfig = Configuration.Control_Access_Config(newConfig, GetID);
                    if (newConfig != "") // if user escapes dialog or bad value leave system unchanged
                    {
                        //Change "Config":
                        if (facadeView.SelectedListItem != null)
                            Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
                        else
                            Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, string.Empty);
                        Configuration.CurrentConfig = newConfig;
                        InitialStart = true; //Set to true to make sure initial View is initialized for new DB view
                        Load_Config(newConfig, true);
                        if (InitialStart)
                            Fin_Charge_Init(true, true); //Guzzi: need to always load default view on initial start, even if always default view is disabled ...
                        else
                            Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
                        InitialStart = false; // Guzzi: Set InitialStart to false after initialization done
                    }

                    break;

                case "nasstatus": //Check and show status of NAS Servers
                    
                    //First check status of configured NAS-Servers
                    WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();
                    int intTimeOut = conf.StrWOLtimeout; //Timeout f�r WOL

                    //GUIControl.ShowControl(GetID, 34);
                    GUIWaitCursor.Show();
                    GUIWindowManager.Process(); //Added by hint of Damien to update GUI first ...

                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    //if (dlg == null) return;
                    //dlg.Reset();
                    System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
                    dlg.SetHeading(GUILocalizeStrings.Get(10798727)); // menu
                    dlg.Add("Zur�ck");
                    choiceSearch.Add("BACK");

                    if (MesFilms.conf.StrNasName1.Length > 0)
                    {
                        if (WakeOnLanManager.Ping(MesFilms.conf.StrNasName1, intTimeOut))
                            dlg.Add("'" + MesFilms.conf.StrNasName1 + "' - (aktiv)");
                        else
                            dlg.Add("'" + MesFilms.conf.StrNasName1 + "' - (offline) - starten?");
                        choiceSearch.Add("NAS1");
                    }

                    if (MesFilms.conf.StrNasName2.Length > 0)
                    {
                        if (WakeOnLanManager.Ping(MesFilms.conf.StrNasName2, intTimeOut))
                            dlg.Add("'" + MesFilms.conf.StrNasName2 + "' - (aktiv)");
                        else
                            dlg.Add("'" + MesFilms.conf.StrNasName2 + "' - (offline) - starten?");
                        choiceSearch.Add("NAS2");
                    }

                    if (MesFilms.conf.StrNasName3.Length > 0)
                    {
                        if (WakeOnLanManager.Ping(MesFilms.conf.StrNasName3, intTimeOut))
                            dlg.Add("'" + MesFilms.conf.StrNasName3 + "' - (aktiv)");
                        else
                            dlg.Add("'" + MesFilms.conf.StrNasName3 + "' - (offline) - starten?");
                        choiceSearch.Add("NAS3");
                    }

                    //GUIControl.ShowControl(GetID, 34);
                    GUIWaitCursor.Hide();
                   
                    dlg.DoModal(GetID);
                    if (dlg.SelectedLabel == -1)
                        return;

                    if (choiceSearch[dlg.SelectedLabel] == "BACK")
                    return;

                    if (choiceSearch[dlg.SelectedLabel] == "NAS1")
                    {
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));
                        dlgOk.SetLine(1, "");

                        if (MesFilms.conf.StrNasMAC1.Length > 0)
                        {
                            if (wakeOnLanManager.WakeupSystem(wakeOnLanManager.GetHwAddrBytes(MesFilms.conf.StrNasMAC1), MesFilms.conf.StrNasName1, intTimeOut))
                            {
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName1 + " erfolgreich gestartet!");
                            }
                            else
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName1 + " konnte nicht gestartet werden!");
                        }
                        else
                        {
                            dlgOk.SetLine(1, "Servername: '" + MesFilms.conf.StrNasName1 + "', MAC: '" + MesFilms.conf.StrNasMAC1 + "'");
                            dlgOk.SetLine(2, "Start nicht m�glich, Konfiguration?");
                        }
                        dlgOk.DoModal(GetID);
                            break;
                    }

                    if (choiceSearch[dlg.SelectedLabel] == "NAS2")
                    {
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));
                        dlgOk.SetLine(1, "");

                        if (MesFilms.conf.StrNasMAC2.Length > 0)
                        {
                            if (wakeOnLanManager.WakeupSystem(wakeOnLanManager.GetHwAddrBytes(MesFilms.conf.StrNasMAC2), MesFilms.conf.StrNasName1, intTimeOut))
                            {
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName2 + " erfolgreich gestartet!");
                            }
                            else
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName2 + " konnte nicht gestartet werden!");
                        }
                        else
                        {
                            dlgOk.SetLine(1, "Servername: '" + MesFilms.conf.StrNasName2 + "', MAC: '" + MesFilms.conf.StrNasMAC2 + "'");
                            dlgOk.SetLine(2, "Start nicht m�glich, Konfiguration?");
                        }
                        dlgOk.DoModal(GetID);
                        break;
                    }

                    if (choiceSearch[dlg.SelectedLabel] == "NAS3")
                    {
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));
                        dlgOk.SetLine(1, "");

                        if (MesFilms.conf.StrNasMAC3.Length > 0)
                        {
                            if (wakeOnLanManager.WakeupSystem(wakeOnLanManager.GetHwAddrBytes(MesFilms.conf.StrNasMAC3), MesFilms.conf.StrNasName1, intTimeOut))
                            {
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName3 + " erfolgreich gestartet!");
                            }
                            else
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName3 + " konnte nicht gestartet werden!");
                        }
                        else
                        {
                            dlgOk.SetLine(1, "Servername: '" + MesFilms.conf.StrNasName3 + "', MAC: '" + MesFilms.conf.StrNasMAC3 + "'");
                            dlgOk.SetLine(2, "Start nicht m�glich, Konfiguration?");
                        }
                        dlgOk.DoModal(GetID);
                        break;
                    }
                    return;

                case "globaloptions":

                    GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg1 == null) return;
                    dlg1.Reset();
                    dlg1.SetHeading(GUILocalizeStrings.Get(924)); // menu
                    System.Collections.Generic.List<string> choiceViewGlobalOptions = new System.Collections.Generic.List<string>();

                    // Change global MovieFilter (Only Movies with Trailer)
                    if (GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798628)));
                    if (!GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798629)));
                    choiceViewGlobalOptions.Add("filterdbtrailer");

                    // Change global MovieFilter (Only Movies with highRating)
                    if (GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798628)));
                    if (!GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798629)));
                    choiceViewGlobalOptions.Add("filterdbrating");

                    // Change Value for global MovieFilter (Only Movies with highRating)
                    dlg1.Add(string.Format(GUILocalizeStrings.Get(10798693), MesFilms.conf.StrAntFilterMinRating.ToString()));
                    choiceViewGlobalOptions.Add("filterdbsetrating");

                    if (MesFilms.conf.StrGrabber)
                    {
                        // From ZebopnsMerge
                        //dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), MesFilms.conf.StrGrabber_ChooseScript.ToString(), (!MesFilms.conf.StrGrabber_ChooseScript).ToString()));   // Choose grabber script for that session
                        if (MesFilms.conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798628)));   // Choose grabber script for that session (status on)
                        if (!MesFilms.conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798629)));   // Choose grabber script for that session (status off)
                        choiceViewGlobalOptions.Add("choosescript");

                        //dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), MesFilms.conf.StrGrabber_Always.ToString(), (!MesFilms.conf.StrGrabber_Always).ToString()));   // Change grabber find trying best match option 
                        if (MesFilms.conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798628)));   // Change grabber find trying best match option (status on)
                        if (!MesFilms.conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798629)));   // Change grabber find trying best match option (status off)
                        choiceViewGlobalOptions.Add("findbestmatch");
                    }

                    //dlg1.Add(string.Format(GUILocalizeStrings.Get(1079865), MesFilms.conf.WindowsFileDialog.ToString(), (!MesFilms.conf.WindowsFileDialog).ToString()));  // Using Windows File Dialog File for that session
                    if (MesFilms.conf.WindowsFileDialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798628)));   // Using Windows File Dialog File for that session (status on)
                    if (!MesFilms.conf.WindowsFileDialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798629)));   // Using Windows File Dialog File for that session (status off)
                    choiceViewGlobalOptions.Add("windowsfiledialog");

                    if (MesFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798628)));
                    if (!MesFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798629)));
                    choiceViewGlobalOptions.Add("alwaysdefaultview");

                    dlg1.DoModal(GetID);

                    if (dlg1.SelectedLabel == -1)
                    {
                        return;
                    }
                    Change_view(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
                    return;

                case "globalupdates":

                    GUIDialogMenu dlg2 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg2 == null) return;
                    dlg2.Reset();
                    dlg2.SetHeading(GUILocalizeStrings.Get(924)); // menu
                    System.Collections.Generic.List<string> choiceViewGlobalUpdates = new System.Collections.Generic.List<string>();

                    if (MesFilms.conf.StrAMCUpd)
                    {
                        dlg2.Add(GUILocalizeStrings.Get(1079861));   // Change Config 
                        choiceViewGlobalUpdates.Add("updatedb");
                    }
                    if (MesFilms.conf.StrFanart)
                    {
                        dlg2.Add(GUILocalizeStrings.Get(4514));   // Download all Fanart
                        choiceViewGlobalUpdates.Add("downfanart");
                    }

                    dlg2.Add(GUILocalizeStrings.Get(10798688));   // Search and register all trailers for all movies in DB
                    choiceViewGlobalUpdates.Add("personinfos-all");

                    dlg2.Add(GUILocalizeStrings.Get(10798694));   // Search and register all trailers for all movies in DB
                    choiceViewGlobalUpdates.Add("trailer-all");

                    dlg2.DoModal(GetID);

                    if (dlg2.SelectedLabel == -1)
                    {
                        return;
                    }
                    Change_view(choiceViewGlobalUpdates[dlg2.SelectedLabel].ToLower());
                    return;

                case "filterdbtrailer":
                    // GlobalFilterTrailersOnly
                    GlobalFilterTrailersOnly = !GlobalFilterTrailersOnly;
                    Log.Info("MyFilms : Global filter for Trailers Only is now set to '" + GlobalFilterTrailersOnly.ToString() + "'");
                    if (GlobalFilterTrailersOnly) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798628));
                    if (!GlobalFilterTrailersOnly) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798629));
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    GlobalFilterString = "";
                    if (GlobalFilterMinRating) GlobalFilterString = GlobalFilterString + "Rating > " + MesFilms.conf.StrAntFilterMinRating.ToString() + " AND ";
                    if (GlobalFilterTrailersOnly) GlobalFilterString = GlobalFilterString + "Borrower not like '' AND ";
                    //if ((GlobalFilterMinRating) && (GlobalFilterTrailersOnly)) GlobalFilterString = GlobalFilterString + " AND ";
                    Log.Info("MyFilms (SetGlobalFilterString Trailers) - 'GlobalFilterString' = '" + GlobalFilterString + "'");
                    GUIWaitCursor.Show();
                    GUIWindowManager.Process(); //Added by hint of Damien to update GUI first ...
                    Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    GUIWaitCursor.Hide();
                    break;

                case "filterdbrating":
                    // GlobalFilterMinRating
                    GlobalFilterMinRating = !GlobalFilterMinRating;
                    Log.Info("MyFilms : Global filter for MinimumRating is now set to '" + GlobalFilterMinRating.ToString() + "'");
                    GUIDialogOK dlgOkFilterDBrating = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    dlgOkFilterDBrating.SetHeading(GUILocalizeStrings.Get(10798624));
                    dlgOkFilterDBrating.SetLine(1, "");
                    if (GlobalFilterMinRating) dlgOkFilterDBrating.SetLine(2, GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798628));
                    if (!GlobalFilterMinRating) dlgOkFilterDBrating.SetLine(2, GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798629));
                    dlgOkFilterDBrating.DoModal(GetID);
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    GlobalFilterString = "";
                    if (GlobalFilterMinRating) GlobalFilterString = GlobalFilterString + "Rating > " + MesFilms.conf.StrAntFilterMinRating.ToString() + " AND ";
                    if (GlobalFilterTrailersOnly) GlobalFilterString = GlobalFilterString + "Borrower not like '' AND ";
                    Log.Info("MyFilms (SetGlobalFilterString MinRating) - 'GlobalFilterString' = '" + GlobalFilterString + "'");
                    GUIWaitCursor.Show();
                    Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    GUIWaitCursor.Hide();
                    break;

                case "filterdbsetrating":
                    // Set global value for minimum Rating to restrict movielist
                    Log.Info("MyFilms (FilterDbSetRating) - 'AntFilterMinRating' current setting = '" + MesFilms.conf.StrAntFilterMinRating.ToString() + "'");
                    MesFilmsDialogSetRating dlgRating = (MesFilmsDialogSetRating)GUIWindowManager.GetWindow(7988);
                    if (MesFilms.conf.StrAntFilterMinRating.ToString().Length > 0)
                        dlgRating.Rating = Convert.ToDecimal(MesFilms.conf.StrAntFilterMinRating.Replace(".", ","));
                    else
                        dlgRating.Rating = 0;
                    dlgRating.SetTitle(GUILocalizeStrings.Get(1079881));
                    dlgRating.DoModal(GetID);
                    MesFilms.conf.StrAntFilterMinRating = dlgRating.Rating.ToString().Replace("," , ".");
                    XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntFilterMinRating", MesFilms.conf.StrAntFilterMinRating);
                    Log.Info("MyFilms (FilterDbSetRating) - 'AntFilterMinRating' changed to '" + MesFilms.conf.StrAntFilterMinRating.ToString() + "'");
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    GlobalFilterString = "";
                    if (GlobalFilterMinRating) GlobalFilterString = GlobalFilterString + "Rating > " + MesFilms.conf.StrAntFilterMinRating.ToString() + " AND ";
                    if (GlobalFilterTrailersOnly) GlobalFilterString = GlobalFilterString + "Borrower not like '' AND ";
                    Log.Info("MyFilms (SetGlobalFilterString) - 'GlobalFilterString' = '" + GlobalFilterString + "'");
                    GUIWaitCursor.Show();
                    if (GlobalFilterMinRating)
                    {
                        Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    }
                    GUIWaitCursor.Hide();
                    break;

                case "updatedb":
                    // Launch AMCUpdater in batch mode
                    if (bgUpdateDB.IsBusy)
                    {
                        ShowMessageDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(875), GUILocalizeStrings.Get(330)); //action already launched
                        break;
                    }
                    bgUpdateDB.DoWork += new DoWorkEventHandler(bgUpdateDB_DoWork);
                    bgUpdateDB.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateDB_RunWorkerCompleted);
                    AsynUpdateDatabase();
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;

                case "downfanart":
                    // Launch Fanart download in batch mode
                    if (bgUpdateFanart.IsBusy)
                    {
                        ShowMessageDialog(GUILocalizeStrings.Get(1079862), GUILocalizeStrings.Get(921), GUILocalizeStrings.Get(330)); //action already launched
                        break;
                    }
                    bgUpdateFanart.DoWork += new DoWorkEventHandler(bgUpdateFanart_DoWork);
                    bgUpdateFanart.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateFanart_RunWorkerCompleted);
                    AsynUpdateFanart();
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;

                case "personinfos-all":
                    // Search and update all personinfos from IMDB
                    // Todo: Call Singlepersonupdate - maybe this function should also be available for single movies ? (less traffic)
                    // ToDo: - first implement singlepersonupdate ...
                    break;
                
                case "trailer-all":
                    // Search and register all trailers for all movies in DB
                    AntMovieCatalog ds = new AntMovieCatalog();
                    ArrayList w_index = new ArrayList();
                    int w_index_count = 0;
                    string t_number_id = "";
                    DataRow[] wr = BaseMesFilms.LectureDonn�es(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                    //Now build a list of valid movies in w_index with Number registered
                    foreach (DataRow wsr in wr)
                    {
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            //Log.Debug("MyFilms (GlobalSearchTrailerLocal) - dc.ColumnName '" + dc.ColumnName.ToString() + "'");
                            if (dc.ColumnName.ToString() == "Number")
                            {
                                t_number_id = wsr[dc.ColumnName].ToString();
                                //Log.Debug("MyFilms (GlobalSearchTrailerLocal) - Movienumber stored as '" + t_number_id + "'");
                            }
                        }
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            if (dc.ColumnName.ToString().ToLower() == "translatedtitle")
                                {
                                    w_index.Add(t_number_id);
                                    Log.Debug("MyFilms (GlobalSearchTrailerLocal) - Add MovieIDs to indexlist: dc: '" + dc.ToString() + "' and Number(ID): '" + t_number_id + "'");
                                    w_index_count = w_index_count + 1;
                                }
                        }
                    }
                    Log.Debug("MyFilms (GlobalSearchTrailerLocal) - Number of Records found: " + w_index_count);

                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                    dlgYesNo.SetHeading("Warnung !");
                    dlgYesNo.SetLine(1, "Sehr lange Laufzeit!");
                    dlgYesNo.SetLine(2, "Wollen Sie wirklich die Trailersuche f�r " + w_index_count + " Filme starten?");
                    dlgYesNo.DoModal(GetID);
                    if (!(dlgYesNo.IsConfirmed))
                        break;
                    GUIWaitCursor.Show();
                    for (i = 0; i < w_index_count; i++)
                    {
                        Log.Debug("MyFilms (GlobalSearchTrailerLocal) - Number: '" + i.ToString() + "' - Index to search: '" + w_index[i] + "'");
                        //MesFilmsDetail.SearchTrailerLocal((DataRow[])MesFilms.r, Convert.ToInt32(w_index[i]));
                        MesFilmsDetail.SearchTrailerLocal((DataRow[])MesFilms.r, Convert.ToInt32(i), false);
                    }
                    Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    GUIWaitCursor.Hide();
                    ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798695)); //Traiersearch finished!
                    break;
                
                case "choosescript":
                    MesFilms.conf.StrGrabber_ChooseScript = !MesFilms.conf.StrGrabber_ChooseScript;
                    XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_ChooseScript", MesFilms.conf.StrGrabber_ChooseScript);
                    Log.Info("MyFilms : Grabber Option 'use always that script' changed to " + MesFilms.conf.StrGrabber_ChooseScript.ToString());
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
                case "findbestmatch":
                    MesFilms.conf.StrGrabber_Always = !MesFilms.conf.StrGrabber_Always;
                    XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_Always", MesFilms.conf.StrGrabber_Always);
                    Log.Info("MyFilms : Grabber Option 'try to find best match...' changed to " + MesFilms.conf.StrGrabber_Always.ToString());
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
                case "windowsfiledialog":
                    MesFilms.conf.WindowsFileDialog = !MesFilms.conf.WindowsFileDialog;
                    XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "WindowsFileDialog", MesFilms.conf.WindowsFileDialog);
                    Log.Info("MyFilms : Update Option 'use Windows File Dialog...' changed to " + MesFilms.conf.WindowsFileDialog.ToString());
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
                case "alwaysdefaultview":
                    MesFilms.conf.AlwaysDefaultView = !MesFilms.conf.AlwaysDefaultView;
                    XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AlwaysDefaultView", MesFilms.conf.AlwaysDefaultView);
                    Log.Info("MyFilms : Update Option 'use always default view...' changed to " + MesFilms.conf.AlwaysDefaultView.ToString());
                    GUIWaitCursor.Show();
                    GUIWindowManager.Process(); //Added by hint of Damien to update GUI first ...

                    if (MesFilms.conf.AlwaysDefaultView) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798628));
                    if (!MesFilms.conf.AlwaysDefaultView) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798629));
                    if (MesFilms.conf.AlwaysDefaultView)
                        Fin_Charge_Init(true, true); //DefaultSelect, reload
                    else
                        Fin_Charge_Init(true, true); //NotDefaultSelect, Only reload
                    GUIWaitCursor.Hide();
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
            }
        }
        //--------------------------------------------------------------------------------------------
        //   Display Context Menu for Movie 
        //--------------------------------------------------------------------------------------------
        // Changed from private to PUBLIC - GUZZI - Original ZebonsMerge was private
		// private void Context_Menu_Movie(int selecteditem)

        public void Context_Menu_Movie(int selecteditem)
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return;
            Context_Menu = true;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
            string[] upd_choice = new string[20];
            int ichoice = 0;

            // Moviecontext
            if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
            {
                dlg.Add(GUILocalizeStrings.Get(1079866));//Search related movies by persons
                upd_choice[ichoice] = "analogyperson";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(10798614));//Search related movies by property
                upd_choice[ichoice] = "analogyproperty";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079887));
                upd_choice[ichoice] = "movieimdbtrailer";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079888));
                upd_choice[ichoice] = "movieimdbbilder";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079889));
                upd_choice[ichoice] = "movieimdbinternet";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079879));//Search Infos to related persons (load persons in facadeview) - only available in filmlist
                upd_choice[ichoice] = "moviepersonlist";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079883)); // update personinfos for all envolved persons of a selected movie from IMDB
                upd_choice[ichoice] = "updatepersonmovie";
                ichoice++;
            }

            // Artistcontext
            if (facadeView.SelectedListItemIndex > -1 && facadeView.SelectedListItem.IsFolder && (conf.WStrSort.ToLower().Contains("actor") || conf.WStrSort.ToLower().Contains("director") || conf.WStrSort.ToLower().Contains("producer")))
            {
                dlg.Add(GUILocalizeStrings.Get(1079884));//Show Infos of person (load persons detailscreen - MesFilmsActor) - only available in personlist
                upd_choice[ichoice] = "artistdetail";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079890));//Show IMDB clips http://www.imdb.com/name/nm0000288/videogallery
                upd_choice[ichoice] = "artistimdbclips";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079891));//Show IMDB pictures http://www.imdb.com/name/nm0000288/mediaindex
                upd_choice[ichoice] = "artistimdbbilder";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079886));//Show IMDB internetinfos http://www.imdb.com/name/nm0000288/
                upd_choice[ichoice] = "artistimdbinternet";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079885));//Show IMDB filmlist in facadeview and add availabilityinformations to it
                upd_choice[ichoice] = "artistimdbfilmlist";
                ichoice++;

                dlg.Add(GUILocalizeStrings.Get(1079882)); // update personinfo from IMDB and create actorthumbs - optional: load mediathek for person backdrops etc.
                upd_choice[ichoice] = "updateperson";
                ichoice++;
            }

            if (MesFilms.conf.StrSuppress && facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
            {
                dlg.Add(GUILocalizeStrings.Get(432));
                upd_choice[ichoice] = "suppress";
                ichoice++;
            }
            if (MesFilms.conf.StrGrabber && facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
            {
                dlg.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
                upd_choice[ichoice] = "grabber";
                ichoice++;
            }
            if (MesFilms.conf.StrFanart && facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
            {
                dlg.Add(GUILocalizeStrings.Get(1079862));
                upd_choice[ichoice] = "fanart";
                ichoice++;
                dlg.Add(GUILocalizeStrings.Get(1079874));
                upd_choice[ichoice] = "deletefanart";
                ichoice++;
            }
            
            dlg.DoModal(GetID);

            if (dlg.SelectedLabel == -1)
                return;
            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            switch (upd_choice[dlg.SelectedLabel])
            {

                case "analogyperson":
                    {
                        SearchRelatedMoviesbyPersons((int)facadeView.SelectedListItem.ItemId);
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        dlg.DeInit();
                        break;
                    }

                case "analogyproperty":
                    {
                        SearchRelatedMoviesbyProperties((int)facadeView.SelectedListItem.ItemId);
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        dlg.DeInit();
                        break;
                    }

                case "movieimdbtrailer":
                    // ToDo: load onlinemovies from IMDB (http://www.imdb.com/title/tt0438488/videogallery)
                    // Share loadingclass with actorclips and use parameter "mode"
                    {
                        break;
                    }

                case "movieimdbbilder":
                    // ToDo: load onlinepictures from IMDB (http://www.imdb.com/title/tt0438488/mediaindex)
                    // Share loadingclass with actorpictures and use parameter "mode"
                    {
                        break;
                    }

                case "movieimdbinternet":
                    // ToDo: Launch IMDB-Internetpage via movie-tt-URL in Webbrowserplugin - check InfoPlugin how to implement ...
                    {
                        break;
                    }

                case "moviepersonlist":
                    {
                        //To be modified to call new class with personlist by type and call MesFilmsActors with facade
                        //Temporarily disabled - will be required to create Person List later ....
                        //SearchRelatedMoviesbyPersons((int)facadeView.SelectedListItem.ItemId);


                        if (!facadeView.SelectedListItem.IsFolder && !conf.Boolselect)
                        // New Window for detailed selected item information
                        {
                            conf.StrIndex = facadeView.SelectedListItem.ItemId;
                            conf.StrTIndex = facadeView.SelectedListItem.Label;
                            GUITextureManager.CleanupThumbs();


                            GUIWindowManager.ActivateWindow(ID_MesFilmsActors);
                        }

                        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        //dlg.DeInit();

                        if (0 == 1)
                        {
                            Actors.MesFilmsActors infoDlg =
                                (Actors.MesFilmsActors)GUIWindowManager.GetWindow(ID_MesFilmsActors);
                            if (infoDlg == null)
                            {
                                return;
                            }

                            infoDlg.DoModal(GetID);
                            dlg.DeInit();
                        }
                        break;
                    }

                case "artistdetail":
                    // ToDo: Launch MesFilmsActorinfo - load new facade for LOCAL (!) IMDB-mediaindex
                    // ToDo: Optional add switch to seitch between filmclips (IMDB-videogallery) and photos (IMDB-mediaindex) - launch Onlinevideos to play clips?
                    {
                        break;
                    }

                case "artistimdbclips":
                    {
                        break;
                    }

                case "artistimdbbilder":
                    {
                        break;
                    }

                case "artistimdbinternet":
                    // ToDo: Launch IMDB-Internetpage via actor-URL in Webbrowserplugin - check InfoPlugin how to implement ...
                    {
                        break;
                    }

                case "artistimdbfilmlist":
                    // ToDo: Launch IMDB-Internetpage via actor-URL in Webbrowserplugin - check InfoPlugin how to implement ...
                    // Share loadingclass with actorclips and use parameter "mode"
                    {
                        break;
                    }

                case "updateperson":
                    {
                        //Todo: add calls to update the personinfos from IMDB - use database and grabberclasses from MePo / Deda
                    }
                    break;

                case "updatepersonmovie":
                    // ToDo: Update personinfo for all involve artists (takes longer!)
                    {
                        break;
                    }

                case "suppress":
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                    dlgYesNo.SetLine(1, GUILocalizeStrings.Get(433));//confirm suppression
                    dlgYesNo.DoModal(GetID);
                    if (dlgYesNo.IsConfirmed)
                    {
                        MesFilmsDetail.Suppress_Entry((DataRow[])MesFilms.r, (int)facadeView.SelectedListItem.ItemId);
                        Fin_Charge_Init(true, true);
                    }
                    break;
                case "grabber":
                    string title = string.Empty;
                    if (MesFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"] != null && MesFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"].ToString().Length > 0)
                        title = MesFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"].ToString();
                    if (title.IndexOf(MesFilms.conf.TitleDelim) > 0)
                        title = title.Substring(title.IndexOf(MesFilms.conf.TitleDelim) + 1);
                    MesFilmsDetail.grabb_Internet_Informations(title, GetID, MesFilms.conf.StrGrabber_ChooseScript, MesFilms.conf.StrGrabber_cnf);
                    break;

                case "fanart":
                    Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
                    string wtitle = string.Empty;
                    if (MesFilms.r[facadeView.SelectedListItem.ItemId]["OriginalTitle"] != null && MesFilms.r[facadeView.SelectedListItem.ItemId]["OriginalTitle"].ToString().Length > 0)
                        wtitle = MesFilms.r[facadeView.SelectedListItem.ItemId]["OriginalTitle"].ToString();
                    if (wtitle.IndexOf(MesFilms.conf.TitleDelim) > 0)
                        wtitle = wtitle.Substring(wtitle.IndexOf(MesFilms.conf.TitleDelim) + 1);
                    string wttitle = string.Empty;
                    if (MesFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"] != null && MesFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"].ToString().Length > 0)
                        wttitle = MesFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"].ToString();
                    if (wttitle.IndexOf(MesFilms.conf.TitleDelim) > 0)
                        wttitle = wttitle.Substring(wttitle.IndexOf(MesFilms.conf.TitleDelim) + 1);
                    if (wtitle.Length > 0 && MesFilms.conf.StrFanart)
                    {
                        MesFilmsDetail.Download_Backdrops_Fanart(wtitle, wttitle, MesFilms.r[facadeView.SelectedListItem.ItemId]["Director"].ToString(), MesFilms.r[facadeView.SelectedListItem.ItemId]["Year"].ToString(), true, GetID);
                    }
                    //if (wttitle != null && wttitle.Length > 0)
                    //    wtitle = wttitle;
                    string[] wfanart = MesFilmsDetail.Search_Fanart(facadeView.SelectedListItem.Label, true, "file", false, facadeView.SelectedListItem.ThumbnailImage.ToString(), string.Empty);
                    if (wfanart[0] == " ")
                    {
                        backdrop.Active = false;
                        GUIControl.HideControl(GetID, 35);
                    }
                    else
                    {
                        backdrop.Active = true;
                        GUIControl.ShowControl(GetID, 35);
                    }
                    Log.Debug("MesFilm (Backdrops-NewfromContext): backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '"+ wfanart[1] + "'");
                    backdrop.Filename = wfanart[0];
                    break;
                case "deletefanart":
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my videos
                    dlgYesNo.SetLine(1, GUILocalizeStrings.Get(433));//confirm suppression
                    dlgYesNo.DoModal(GetID);
                    if (dlgYesNo.IsConfirmed)
                        MesFilmsDetail.Remove_Backdrops_Fanart(MesFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"].ToString(), false);
                    break;
            }
        }

        //*****************************************************************************************
        //*  search related movies by persons                                                     *
        //*****************************************************************************************
        private void SearchRelatedMoviesbyPersons(int Index)
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(1079867)); // menu
            ArrayList w_tableau = new ArrayList();
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            if (MesFilms.r[Index]["Producer"].ToString().Length > 0)
            {
                w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MesFilms.r[Index]["Producer"].ToString())));
                foreach (object t in w_tableau)
                {
                    dlg.Add(GUILocalizeStrings.Get(10798612) + " : " + t);
                    choiceSearch.Add(t.ToString());
                }
            }
            if (MesFilms.r[Index]["Director"].ToString().Length > 0)
            {
                w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MesFilms.r[Index]["Director"].ToString())));
                foreach (object t in w_tableau)
                {
                    dlg.Add(GUILocalizeStrings.Get(1079869) + " : " + t);
                    choiceSearch.Add(t.ToString());
                }
            }
            if (MesFilms.r[Index]["Actors"].ToString().Length > 0)
            {
                w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MesFilms.r[Index]["Actors"].ToString())));
                foreach (object t in w_tableau)
                {
                    dlg.Add(GUILocalizeStrings.Get(1079868) + " : " + t);
                    choiceSearch.Add(t.ToString());
                }
            }
            if (choiceSearch.Count == 0)
            {
                if (dlg1 == null) return;
                dlg1.SetHeading(GUILocalizeStrings.Get(1079867));
                dlg1.SetLine(1, GUILocalizeStrings.Get(10798641));
                dlg1.DoModal(GUIWindowManager.ActiveWindow);
                return;
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            string wperson = choiceSearch[dlg.SelectedLabel];
            dlg.Reset();
            choiceSearch.Clear();
            dlg.SetHeading(GUILocalizeStrings.Get(10798611) + wperson); // function selection (actor, director, producer)

            //First add general option to show MP Actor Infos
            if (wperson.Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798731));
                //dlg.Add("Person Infos");
                choiceSearch.Add("PersonInfo");
            }

            DataRow[] wr = BaseMesFilms.LectureDonn�es(MesFilms.conf.StrDfltSelect, "Producer like '*" + wperson + "*'", MesFilms.conf.StrSorta, MesFilms.conf.StrSortSens, false);
            if (wr.Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(10798612) + "  (" + wr.Length.ToString() + ")");
                choiceSearch.Add("Producer");
            }
            wr = BaseMesFilms.LectureDonn�es(MesFilms.conf.StrDfltSelect, "Director like '*" + wperson + "*'", MesFilms.conf.StrSorta, MesFilms.conf.StrSortSens, false);
            if (wr.Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(1079869) + "  (" + wr.Length.ToString() + ")");
                choiceSearch.Add("Director");
            }
            wr = BaseMesFilms.LectureDonn�es(MesFilms.conf.StrDfltSelect, "Actors like '*" + wperson + "*'", MesFilms.conf.StrSorta, MesFilms.conf.StrSortSens, false);
            if (wr.Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(1079868) + "  (" + wr.Length.ToString() + ")");
                choiceSearch.Add("Actors");
            }
            if (choiceSearch.Count == 0)
            {
                if (dlg1 == null) return;
                dlg1.SetHeading(GUILocalizeStrings.Get(1079867));
                dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
                dlg1.DoModal(GUIWindowManager.ActiveWindow);
                return;
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            if (choiceSearch[dlg.SelectedLabel] == "PersonInfo")
            {
                ArrayList actorList = new ArrayList();
                // Search with searchName parameter which contain wanted actor name, result(s) is in array
                // which conatin id and name separated with char "|"
                MediaPortal.Video.Database.VideoDatabase.GetActorByName(wperson, actorList);

                // Check result
                if (actorList.Count == 0)
                {
                    Log.Debug("MyFilms (Person Info): No ActorIDs found for '" + wperson + "'");
                    GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK); 
                    dlgOk.SetHeading("Info");
                    dlgOk.SetLine(1, "");
                    dlgOk.SetLine(2, "Keine Personen Infos vorhanden !");
                    dlgOk.DoModal(GetID);
                    return;
                }
                Log.Debug("MyFilms (Person Info): " + actorList.Count.ToString() + " ActorID(s) found for '" + wperson + "'");
                //int actorID;
                actorID = 0;
                string actorname = "";
                // Define splitter for string
                char[] splitter = { '|' };
                // Iterate through list
                foreach (string act in actorList)
                {
                    // Split id from actor name (two substrings, [0] is id and [1] is name)
                    string[] strActor = act.Split(splitter);
                    // From here we have all what we want, now we can populate datatable, gridview, listview....)
                    // actorID originally is integer in the databse (it can be string in results but if we want get details from
                    // IMDBActor  GetActorInfo(int idActor) we need integer)
                    actorID = Convert.ToInt32(strActor[0]);
                    actorname = strActor[1];
                    Log.Debug("MyFilms (Person Info): ActorID: '" + actorID + "' with ActorName: '" + actorname + "' found found for '" + wperson + "'");
                }
                
                MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorID);
                //MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(1);
                //if (actor != null)

                OnVideoArtistInfoGuzzi(actor);
                //OnVideoArtistInfoGuzzi(wperson);
                return;
            }
            conf.StrSelect = choiceSearch[dlg.SelectedLabel].ToString() + " like '*" + wperson + "*'";
            switch (choiceSearch[dlg.SelectedLabel])
            {
                case "Actors":
                    conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(1079868) + " [*" + wperson + @"*]";
                    break;
                case "Director":
                    conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(1079869) + " [*" + wperson + @"*]";
                    break;
                default:
                    conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(10798612) + " [*" + wperson + @"*]";
                    break;
            }
            conf.StrTitleSelect = "";
            GetFilmList();
        }

        private void OnVideoArtistInfoGuzzi(MediaPortal.Video.Database.IMDBActor actor)
        {
            ActorDialog.MesFilmsActorInfo infoDlg =
                (ActorDialog.MesFilmsActorInfo)GUIWindowManager.GetWindow(ID_MesFilmsActorsInfo);
            if (infoDlg == null)
            {
                return;
                }
            if (actor == null)
            {
                return;
            }
            infoDlg.Actor = actor;
            infoDlg.DoModal(GetID);
        }

        //*****************************************************************************************
        //*  search related movies by properties  (From ZebonsMerge, renamed to ...Zebons)		  *
        //*****************************************************************************************
        private void SearchRelatedMoviesbyPropertiesZebons(int Index, IEnumerable<string> wSearchList)
        {
            // first select the property to be searching on
           
            AntMovieCatalog ds = new AntMovieCatalog();
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            ArrayList w_tableau = new ArrayList();
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
            foreach (string wSearch in wSearchList)
            {
                dlg.Add(GUILocalizeStrings.Get(10798617) + "'" + BaseMesFilms.Translate_Column(wSearch) + "'");
                choiceSearch.Add(wSearch);
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            string wproperty = choiceSearch[dlg.SelectedLabel];
            dlg.Reset();
            if (choiceSearch.Count == 0)
            {
                if (dlg1 == null) return;
                dlg1.SetHeading(GUILocalizeStrings.Get(10798613));
                dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
                dlg1.SetLine(2, BaseMesFilms.Translate_Column(wproperty));
                dlg1.DoModal(GUIWindowManager.ActiveWindow);
                return;
            }
            choiceSearch.Clear();
            if (ds.Movie.Columns[wproperty].DataType.Name == "string")
                w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MesFilms.r[Index][wproperty].ToString())));
            else
                w_tableau.Add(MesFilms.r[Index][wproperty].ToString());
            foreach (object t in w_tableau)
            {
                dlg.Add(wproperty + " : " + t);
                choiceSearch.Add(t.ToString());
            }
            if (choiceSearch.Count == 0)
            {
                if (dlg1 == null) return;
                dlg1.SetHeading(GUILocalizeStrings.Get(10798613));
                dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
                dlg1.SetLine(2, BaseMesFilms.Translate_Column(wproperty));
                dlg1.DoModal(GUIWindowManager.ActiveWindow);
                return;
            }
            dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // property selection
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            if (ds.Movie.Columns[wproperty].DataType.Name == "string")
            {
                conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel].ToString() + "*'";
                conf.StrTxtSelect = "Selection " + BaseMesFilms.Translate_Column(wproperty) + " [*" + choiceSearch[dlg.SelectedLabel].ToString() + @"*]";
            }
            else
            {
                conf.StrSelect = wproperty + " = '" + choiceSearch[dlg.SelectedLabel].ToString() + "'";
                conf.StrTxtSelect = "Selection " + BaseMesFilms.Translate_Column(wproperty) + " [" + choiceSearch[dlg.SelectedLabel].ToString() + @"]";
            }
            
            conf.StrTitleSelect = "";
            GetFilmList();
        }


        //*****************************************************************************************
        //*  Global search movies by properties (ZebonsMerge - Renamed to ....Zebons)             *
        //*****************************************************************************************
        private void SearchMoviesbyPropertiesZebons(IEnumerable<string> wSearchList)
        {
            // first select the property to be searching on
 
            AntMovieCatalog ds = new AntMovieCatalog();
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            ArrayList w_tableau = new ArrayList();
            ArrayList w_tablabel = new ArrayList();
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798615)); // menu
            dlg.Add(GUILocalizeStrings.Get(10798616)); // search on all fields
            choiceSearch.Add("all");
            foreach (string wSearch in wSearchList)
            {
                dlg.Add(GUILocalizeStrings.Get(10798617) + "'" + BaseMesFilms.Translate_Column(wSearch) + "'");
                choiceSearch.Add(wSearch);
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            string wproperty = choiceSearch[dlg.SelectedLabel];
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            if (null == keyboard) return;
            keyboard.Reset();
            keyboard.Text = "";
            keyboard.DoModal(GetID);
            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
            {
                switch (choiceSearch[dlg.SelectedLabel])
                {
                    case "all":
                        ArrayList w_count = new ArrayList();
                        if (dlg == null) return;
                        dlg.Reset();
                        dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
                        DataRow[] wr = BaseMesFilms.LectureDonn�es(conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                        foreach (DataRow wsr in wr)
                        {
                            foreach (string wsearch in wSearchList)
                            {
                                if (System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wsr[wsearch].ToString().ToLower())).Contains(keyboard.Text.ToLower()))
                                    // column contains text serached on : added to w_tableau + w_count
                                    if (w_tableau.Contains(wsearch))
                                    // search position in w_tableau for adding +1 to w_count
                                    {
                                        for (int i = 0; i < w_tableau.Count; i++)
                                        {
                                            if (w_tableau[i].ToString() == wsearch)
                                            {
                                                w_count[i] = (int)w_count[i] + 1;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    // add to w_tableau and move 1 to w_count
                                    {
                                        w_tableau.Add(wsearch.ToString());
                                        w_count.Add(1);
                                    }
                            }
                            //foreach (DataColumn dc in ds.Movie.Columns)
                            //{
                            //    if (System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wsr[dc.ColumnName].ToString())).Contains(keyboard.Text))
                            //        // column contains text serached on : added to w_tableau + w_count
                            //        if (w_tableau.Contains(dc.ColumnName))
                            //        // search position in w_tableau for adding +1 to w_count
                            //        {
                            //            for (int i = 0; i < w_tableau.Count; i++)
                            //            {
                            //                if (w_tableau[i].ToString() == dc.ColumnName)
                            //                {
                            //                    w_count[i] = (int)w_count[i] + 1;
                            //                    break;
                            //                }
                            //            }
                            //        }
                            //        else
                            //        // add to w_tableau and move 1 to w_count
                            //        {
                            //            w_tableau.Add(dc.ColumnName.ToString());
                            //            w_count.Add(1);
                            //        }
                            //}
                        }
                        if (w_tableau.Count == 0)
                        {
                            if (dlg1 == null) return;
                            dlg1.SetHeading(GUILocalizeStrings.Get(10798613));
                            dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
                            dlg1.SetLine(2, keyboard.Text);
                            dlg1.DoModal(GUIWindowManager.ActiveWindow);
                            return;
                        }
                        dlg.Reset();
                        dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618),keyboard.Text)); // menu
                        choiceSearch.Clear();
                        for (int i = 0; i < w_tableau.Count; i++)
                        {
                            dlg.Add(string.Format(GUILocalizeStrings.Get(10798619), w_count[i], BaseMesFilms.Translate_Column(w_tableau[i].ToString())));
                            choiceSearch.Add(w_tableau[i].ToString());
                        }
                        dlg.DoModal(GetID);
                        if (dlg.SelectedLabel == -1)
                            return;
                        wproperty = choiceSearch[dlg.SelectedLabel];
                        if (control_searchText(keyboard.Text))
                        {
                            if (ds.Movie.Columns[wproperty].DataType.Name == "string")
                            {
                                conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel].ToString() + "*'";
                                conf.StrTxtSelect = "Selection " + BaseMesFilms.Translate_Column(wproperty) + " [*" + choiceSearch[dlg.SelectedLabel].ToString() + @"*]";
                            }
                            else
                            {
                                conf.StrSelect = wproperty + " = '" + keyboard.Text + "'";
                                conf.StrTxtSelect = "Selection " + BaseMesFilms.Translate_Column(wproperty) + " [" + keyboard.Text + @"]";
                            }
                        //}
                        //if (control_searchText(keyboard.Text))
                        //{
                        //    if (wproperty == "Rating")
                        //        conf.StrSelect = wproperty + " = " + keyboard.Text;
                        //    else
                        //        if (wproperty == "Number")
                        //            conf.StrSelect = wproperty + " = " + keyboard.Text;
                        //        else
                        //            conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
                        //    conf.StrTxtSelect = "Selection " + BaseMesFilms.Translate_Column(wproperty) + " [*" + keyboard.Text + @"*]";
                            conf.StrTitleSelect = "";
                            //                         getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                            GetFilmList();
                        }
                        break;
                    default:
                        if (control_searchText(keyboard.Text))
                        {
                            switch (wproperty)
                            {
                                case "Rating":
                                    conf.StrSelect = wproperty + " = " + keyboard.Text;
                                    break;
                                case "Number":
                                    conf.StrSelect = wproperty + " = " + keyboard.Text;
                                    break;
                                default:
                                    conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
                                    break;
                            }
                        conf.StrTxtSelect = "Selection " + dlg.SelectedLabelText + " [*" + keyboard.Text + @"*]";
                            conf.StrTitleSelect = "";
   //                         getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                            GetFilmList();
                        }
                        break;
                }
            }
        }

       
        //*****************************************************************************************
        //*  search related movies by properties  (Guzzi Version)                                 *
        //*****************************************************************************************
        private void SearchRelatedMoviesbyProperties(int Index)
        {
            // first select the property to be searching on
            AntMovieCatalog ds = new AntMovieCatalog();
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            ArrayList w_tableau = new ArrayList();
            ArrayList wsub_tableau = new ArrayList();
            int MinChars = 2;
            bool Filter = true;
            if (dlg == null) return;

            conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
            conf.WStrSort = conf.StrSTitle;
            conf.Boolselect = false;
            conf.Boolreturn = false;

            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
            //<entry name="SearchList">TranslatedTitle|OriginalTitle|Description|Comments|Actors|Director|Producer|Year|Date|Category|Country|Rating|Checked|MediaLabel|MediaType|URL|Borrower|Length|VideoFormat|VideoBitrate|AudioFormat|AudioBitrate|Resolution|Framerate|Size|Disks|Languages|Subtitles|Number</entry>
            //<entry name="UpdateList">TranslatedTitle|OriginalTitle|Category|Year|Date|Country|Rating|Checked|MediaLabel|MediaType|Actors|Director|Producer</entry>
            //<entry name="AllItems..">TranslatedTitle|OriginalTitle|FormattedTitle|Description|Comments|Actors|Director|Producer|Rating|Country|Category|Year|Checked|MediaLabel|MediaType|Source|Date|Borrower|Length|URL|VideoFormat|VideoBitrate|AudioFormat|AudioBitrate|Resolution|Framerate|Languages|Subtitles|DateAdded|Size|Disks|Picture|Contents_Id|Number</entry>
            //Sorted lists - manually adding items to have them in right order
            string[] PropertyList = new string[] { "TranslatedTitle", "OriginalTitle", "Description", "Comments", "Actors", "Director", "Producer", "Year", "Date", "Category", "Country", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked", "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate", "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "Source", "Borrower" };
            string[] PropertyListLabel = new string[] { "10798659", "10798658", "10798669", "10798670", "10798667", "10798661", "10798662", "10798665", "10798655", "10798664", "10798663", "10798657", "10798677", "10798678", "10798660", "10798651", "10798652", "10798653", "10798666", "10798671", "10798672", "10798673", "10798674", "10798675", "10798676", "10798680", "10798681", "10798650", "10798668", "10798654", "10798656" };
            for (int ii = 0; ii < 31; ii++)
            {
                dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))));
                choiceSearch.Add(PropertyList[ii].ToString());
            }
            foreach (DataColumn dc in ds.Movie.Columns)
            {
                switch (dc.ColumnName)
                {
                    //case "Description":
                    //case "Comments":
                    //case "OriginalTitle":
                    //case "TranslatedTitle":
                    case "Picture":
                    case "FormattedTitle":
                    case "Contents_Id":
                    case "Length_Num":
                    case "DateAdded":
                    case "Number":
                    case "Size":
                    case "AudioBitrate":
                    case "AudioFormat":
                    case "VideoBitrate":
                    case "Borrower":
                    case "Checked":
                        break;
                    default:
                        //  Can be temorarily disabled for testing  
                        //dlg.Add(GUILocalizeStrings.Get(10798617) + dc.ColumnName);
                        //choiceSearch.Add(dc.ColumnName);
                        break;
                }
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            string wproperty = choiceSearch[dlg.SelectedLabel];
            dlg.Reset();
            choiceSearch.Clear();
            Log.Debug("MyFilms (RelatedPropertySearch): Searchstring in Property: '" + MesFilms.r[Index][wproperty].ToString() + "'");
            //PersonTitle Grabbing (Double Words)
            if (!(wproperty.ToString().ToLower() == "description") && !((wproperty.ToString().ToLower() == "comments")) && !((wproperty.ToString().ToLower() == "rating")))
            {
                w_tableau = Search_String(MesFilms.r[Index][wproperty].ToString());
                foreach (object t in w_tableau)
                {
                    for (int ii = 0; ii < 30; ii++)
                    {
                        if (wproperty.ToLower().Equals(PropertyList[ii].ToString().ToLower()))
                        {
                            dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + ": " + t);
                            //dlg.Add(wproperty + " : " + w_tableau[wi]);
                            choiceSearch.Add(t.ToString());
                            Log.Debug("MyFilms (RelatedPropertySearch): Searchstring Result Add: '" + t.ToString() + "'");
                            break;
                        }
                    }
                }
            }
            //SubWordGrabbing for more Details, if necessary
            if (!(wproperty.ToString().ToLower() == "description") && !((wproperty.ToString().ToLower() == "comments")))
                MinChars = 2;
                else
                MinChars = 5;
            if (MesFilms.r[Index][wproperty].ToString().Length > 0) //To avoid exception in subgrabbing
                wsub_tableau = SubWordGrabbing(MesFilms.r[Index][wproperty].ToString(),MinChars,Filter);
            if ((wproperty.ToString().ToLower() == "rating"))
            {
                dlg.Add(GUILocalizeStrings.Get(10798657) + ": = " + MesFilms.r[Index][wproperty].ToString().Replace(",","."));
                choiceSearch.Add("RatingExact");
                dlg.Add(GUILocalizeStrings.Get(10798657) + ": > " + MesFilms.r[Index][wproperty].ToString().Replace(",","."));
                choiceSearch.Add("RatingBetter");
            }
            else
            {
                Log.Debug("MyFilms (RelatedPropertySearch): Length: '" + MesFilms.r[Index][wproperty].ToString().Length.ToString() + "'");
                if (MesFilms.r[Index][wproperty].ToString().Length > 0)
                {
                    foreach (object t in wsub_tableau)
                    {
                        if (w_tableau.Contains(t)) // Only Add SubWordItems if not already present in SearchStrin Table
                        {
                            Log.Debug("MyFilms (RelatedPropertySearch): Searchstring Result already Present: '" + t.ToString() + "'");
                            break;
                        }
                        else
                        {
                            for (int ii = 0; ii < 30; ii++)
                            {
                                if (wproperty.ToLower().Equals(PropertyList[ii].ToString().ToLower()))
                                {
                                    dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + " (" + GUILocalizeStrings.Get(10798627) + "): '" + t + "'");
                                    //dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + ": '" + wsub_tableau[wi] + "'");
                                    choiceSearch.Add(t.ToString());
                                    Log.Debug("MyFilms (RelatedPropertySearch): Searchstring Result Add: '" + t.ToString() + "'");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if ((choiceSearch.Count == 0) && (1 == 2)) // Temporarily Disabled
            {
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));//InfoPanel
                dlgOk.SetLine(1, GUILocalizeStrings.Get(10798625));
                dlgOk.DoModal(GetID);
                if (dlg.SelectedLabel == -1)
                    return;
                //break;
            }
            
            //if (choiceSearch.Count > 1)
            Log.Debug("MyFilms (Related Search by properties - ChoiceSearch.Count: " + choiceSearch.Count.ToString());
            if (choiceSearch.Count > 0)
            {
                dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // property selection
                dlg.DoModal(GetID);
                if (dlg.SelectedLabel == -1)
                    return;
            }
            else
            dlg.SelectedLabel = 0;
            Log.Debug("MyFilms (Related Search by properties - Selected wproperty: " + wproperty + "'");
            string w_rating = "0";
            
            if (choiceSearch.Count == 0) //Use Special "is NULL" handling if property is empty ...
                conf.StrSelect = wproperty + " is NULL";
            else
                w_rating = MesFilms.r[Index][wproperty].ToString().Replace(",", "."); 
                if ((wproperty == "Rating") && (choiceSearch[dlg.SelectedLabel].ToString() == "RatingExact"))
                    conf.StrSelect = wproperty.ToString() + " = " + w_rating;
                else
                    if ((wproperty == "Rating") && (choiceSearch[dlg.SelectedLabel].ToString() == "RatingBetter"))
                        conf.StrSelect = wproperty + " > " + w_rating;
                        else
                            if (wproperty == "Number")
                                conf.StrSelect = wproperty + " = " + choiceSearch[dlg.SelectedLabel].ToString();
                                    else
                                        conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel].ToString() + "*'";
            if (choiceSearch.Count == 0) 
                conf.StrTxtSelect = "Selection " + wproperty + "(none)";
            else
                conf.StrTxtSelect = "Selection " + wproperty + " [*" + choiceSearch[dlg.SelectedLabel].ToString() + @"*]";
            conf.StrTitleSelect = "";
            GetFilmList();
        }

        //******************************************************************************************************
        //*  Global search movies by RANDOM (Random Search with Options, e.g. Trailer, Rating) - Guzzi Version *
        //******************************************************************************************************
        private void SearchMoviesbyRandomWithTrailer()
        {
            // first select the area where to make random search on - "all", "category", "year", "country"
            AntMovieCatalog ds = new AntMovieCatalog();
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            ArrayList w_tableau = new ArrayList();
            ArrayList wsub_tableau = new ArrayList();
            bool GetItems = false;
            bool GetSubItems = true;
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798621)); // menu for random search
            dlg.Add(GUILocalizeStrings.Get(10798622)); // random search on all
            choiceSearch.Add("randomall");
            dlg.Add(GUILocalizeStrings.Get(10798664)); // random search on Genre
            choiceSearch.Add("Category");
            dlg.Add(GUILocalizeStrings.Get(10798665)); // random search on year
            choiceSearch.Add("Year");
            dlg.Add(GUILocalizeStrings.Get(10798663)); // random search on country
            choiceSearch.Add("Country");
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            string wproperty = choiceSearch[dlg.SelectedLabel];
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            //            if (null == keyboard) return;
            keyboard.Reset();
            keyboard.Text = "";
            //            keyboard.DoModal(GetID);
            //            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
            switch (choiceSearch[dlg.SelectedLabel])
            {
                //case "randomall":
                //    break;

                default:
                    ArrayList w_count = new ArrayList();
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
                    //Modified to checked for GlobalFilterString
                    DataRow[] wr = BaseMesFilms.LectureDonn�es(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                    //DataColumn[] wc = BaseMesFilms.LectureDonn�es(conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                    w_tableau.Add(string.Format(GUILocalizeStrings.Get(10798623))); //Add Defaultgroup for invalid or empty properties
                    w_count.Add(0);
                    foreach (DataRow wsr in wr)
                    {
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            if (dc.ToString().Contains(wproperty))  // Only count property chosen
                            {
                                if (wsr[dc.ColumnName].ToString().Length == 0) //Empty property special handling
                                {
                                    w_count[0] = (int)w_count[0] + 1;
                                    break;
                                }
                                else
                                {
                                    //Log.Debug("MyFilms (Guzzi) AddDistinctClasses: " + "Property: " + dc.ToString() + " and Value: '" + wsr[dc.ColumnName].ToString() + "'");
                                    // column Name contains propertyname : added to w_tableau + w_count
                                    if (GetSubItems)
                                    {
                                        Log.Debug("MyFilms SubItemGrabber: Input: " + wsr[dc.ColumnName].ToString());
                                        wsub_tableau = SubItemGrabbing(wsr[dc.ColumnName].ToString()); //Grab SubItems
                                        foreach (object t in wsub_tableau)
                                        {
                                            Log.Debug("MyFilms SubItemGrabber: Output: " + t.ToString());
                                            {
                                                if (w_tableau.Contains(t.ToString())) // search position in w_tableau for adding +1 to w_count
                                                {
                                                    //if (!w_index.Contains(
                                                    for (int i = 0; i < w_tableau.Count; i++)
                                                    {
                                                        if (w_tableau[i].ToString() == t.ToString())
                                                        {
                                                            w_count[i] = (int)w_count[i] + 1;
                                                            //Log.Debug("MyFilms SubItemGrabber: add Counter for '" + wsub_tableau[wi].ToString() + "'");
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                    // add to w_tableau and move 1 to w_count
                                                {
                                                    Log.Debug("MyFilms SubItemGrabber: add new Entry for '" + wsr[dc.ColumnName].ToString() + "'");
                                                    w_tableau.Add(t.ToString());
                                                    w_count.Add(1);
                                                }
                                            }
                                        }

                                    }
                                    if (GetItems)
                                    {
                                        if (w_tableau.Contains(wsr[dc.ColumnName])) // search position in w_tableau for adding +1 to w_count
                                        {
                                            for (int i = 0; i < w_tableau.Count; i++)
                                            {
                                                if (w_tableau[i].ToString() == wsr[dc.ColumnName].ToString())
                                                {
                                                    w_count[i] = (int)w_count[i] + 1;
                                                    //Log.Debug("MyFilms (Guzzi) Clas already present, adding Counter for Property: " + dc.ToString() + "Value: '" + wsr[dc.ColumnName].ToString() + "'");
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        // add to w_tableau and move 1 to w_count
                                        {
                                            //Log.Debug("MyFilms (Guzzi) AddDistinctClasses with Property: '" + dc.ToString() + "' and Value '" + wsr[dc.ColumnName].ToString() + "'");
                                            w_tableau.Add(wsr[dc.ColumnName].ToString());
                                            w_count.Add(1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (w_tableau.Count == 0)
                    {
                        Log.Debug("MyFilms PropertyClassCount is 0");
                        break;
                    }


                    string wproperty2="";

                    if (!(wproperty == "randomall"))
                        {
                            dlg.Reset();
                            dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), wproperty)); // menu
                            choiceSearch.Clear();
                            //w_tableau = Array.Sort(w_tableau);
                            for (int i = 0; i < w_tableau.Count; i++)
                            {
                                dlg.Add(string.Format(GUILocalizeStrings.Get(10798626), w_count[i], w_tableau[i]));
                                choiceSearch.Add(w_tableau[i].ToString());
                            }
                            dlg.DoModal(GetID);
                            if (dlg.SelectedLabel == -1)
                                return;
                            string t_wproperty2 = choiceSearch[dlg.SelectedLabel];
                            wproperty2 = t_wproperty2;
                        }
                    else
                        wproperty2 = "*";

                    Log.Debug("MyFilms (RandomMovies) - Chosen Subcategory: '" + wproperty2 + "' selecting in '" + wproperty + "'");
                    switch (wproperty)
                    {
                        case "Rating":
                            conf.StrSelect = wproperty + " = " + wproperty2;
                            break;
                        case "Number":
                            conf.StrSelect = wproperty + " = " + wproperty2;
                            break;
                        default:
                            if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623))) // Check, if emptypropertystring is set
                                conf.StrSelect = wproperty + " like ''";
                            else
                                conf.StrSelect = wproperty + " like '*" + wproperty2 + "*'";
                            break;
                    }
                    Log.Debug("MyFilms (RandomMovies) - resulting conf.StrSelect: '" + conf.StrSelect + "'");
                    conf.StrTxtSelect = "Selection " + wproperty + " [*" + wproperty2 + @"*]";
                    conf.StrTitleSelect = "";

                    // Temporarily Enabled for Testing
                    // getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                    //GetFilmList();

                    // we have: wproperty = selected category (randomall for all) and wproperty2 = value to search after

                    //if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623)))
                    //    wproperty2 = "";                                                    // Should not be done, because it is already handled in searchroutine
                    ArrayList w_index = new ArrayList();
                    int w_index_count = 0;
                    string t_number_id = "";
                    //Now build a list of valid movies in w_index with Number registered
                    foreach (DataRow wsr in wr)
                    {
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            //Log.Debug("dc.ColumnName '" + dc.ColumnName.ToString() + "'");
                            if (dc.ColumnName.ToString() == "Number")
                            {
                                t_number_id = wsr[dc.ColumnName].ToString();
                                //Log.Debug("Movienumber stored as '" + t_number_id + "'");
                            }
                        }
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            if ((wproperty == "randomall") && (dc.ColumnName.ToString().ToLower() == "translatedtitle"))
                                {
                                    w_index.Add(t_number_id);
                                    Log.Debug("MyFilms (RamdomSearch - randomall!!!) - Add MovieIDs to indexlist: dc: '" + dc.ToString() + "' and Number(ID): '" + t_number_id + "'");
                                    w_index_count = w_index_count + 1;
                                }
                            else
                                if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623))) // Check, if emptypropertystring is set
                                {
                                    if ((dc.ColumnName.ToString() == wproperty) && (wsr[dc.ColumnName].ToString().Length == 0)) // column Name contains propertyname : add movie number (for later selection) to w_index
                                    {
                                        w_index.Add(t_number_id);
                                        Log.Debug("MyFilms (RamdomSearch - (none)!!!) Add MovieIDs to indexlist: dc: '" + dc.ToString() + "' and Number(ID): '" + t_number_id + "'");
                                        w_index_count = w_index_count + 1;
                                    }
                                }
                                else
                                    {
                                        //Log.Debug("MyFilms (searchmatches) - dc '" + dc.ToString() + "' - dc.ColumnName '" + dc.ColumnName.ToString() + "' - wproperty '" + wproperty + "' and Number(ID): '" + t_number_id + "'");
                                        if (dc.ColumnName.ToString() == wproperty.ToString()) 
                                        {
                                            //Log.Debug("MyFilms - (searfhmatches with subitems) property2: '" + wproperty2 + "' - DB-Content: '" + wsr[dc.ColumnName].ToString() + "'"); 
                                            if (wsr[dc.ColumnName].ToString().Contains(wproperty2)) // column Name contains propertyname : add movie number (for later selection) to w_index
                                                {
                                                    w_index.Add(t_number_id);
                                                    Log.Debug("MyFilms (RamdomSearch - Standard) Counter '" + w_index_count.ToString() + "' Added as '" + w_index[w_index_count] + "'");
                                                    w_index_count = w_index_count + 1;
                                                }
                                        }
                                    }
                        }
                    }
                    // we now have a list with movies matching the choice and their index/number value -> now do loop for selection
                    Log.Debug("MyFilms (ResultBuildIndex) Found " + w_index.Count + " Records matching '" + wproperty2 + "' in '" + wproperty + "'");
                    for (int i = 0; i < w_index.Count; i++)
                        Log.Debug("MyFilms (ResultList) - Index: '" + i + "' - Number: '" + w_index[i].ToString() + "'");
                    if (w_index.Count == 0)
                    {
                        ShowMessageDialog(GUILocalizeStrings.Get(10798621), "Suchergebnis: 0", "Keine Filme in der Auswahl vorhanden"); // menu for random search
                        return;
                    }

                    //Choose Random Movie from Resultlist
                    System.Random rnd = new System.Random();
                    Int32 RandomNumber = rnd.Next(w_index.Count + 1);
                    Log.Debug("MyFilms RandomNumber: '" + RandomNumber + "'");
                    Log.Debug("MyFilms RandomTitle: '" + RandomNumber + "'");

                    //Set Filmlist to random Movie:
                    conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
                    conf.WStrSort = conf.StrSTitle;
                    conf.Boolselect = false;
                    conf.Boolreturn = false;
                    
                    conf.StrSelect = "number = " + Convert.ToInt32(w_index[RandomNumber]);
                    conf.StrTxtSelect = "Selection number [" + Convert.ToInt32(w_index[RandomNumber]).ToString() + "]";
                    conf.StrTitleSelect = "";
                    //getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                    Log.Debug("MyFilms (Guzzi): Change_View filter - " + "StrSelect: " + conf.StrSelect + " | WStrSort: " + conf.WStrSort);
                    GetFilmList(); // Added to update view ????

                    //Set Context to first and only title in facadeview
                    facadeView.SelectedListItemIndex = 0; //(Auf ersten und einzigen Film setzen, der dem Suchergebnis entsprechen sollte)
                    if (!facadeView.SelectedListItem.IsFolder && !conf.Boolselect)
                        // New Window for detailed selected item information
                        {
                            conf.StrIndex = facadeView.SelectedListItem.ItemId;
                            conf.StrTIndex = facadeView.SelectedListItem.Label;
                            GUITextureManager.CleanupThumbs();
                            //GUIWindowManager.ActivateWindow(ID_MesFilmsDetail);
                        }
                        else
                        // View List as selected
                        {
                            conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                            Change_LayOut(MesFilms.conf.StrLayOut);
                            if (facadeView.SelectedListItem.IsFolder)
                                conf.Boolreturn = false;
                            else
                                conf.Boolreturn = true;
                            do
                            {
                                if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                                conf.StrTitleSelect += conf.Wselectedlabel;
                            } while (GetFilmList() == false); //keep calling while single folders found
                        }
                    
                    //Before showing menu, first play the trailer
                    //conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                    //Change_LayOut(MesFilms.conf.StrLayOut);
                    //conf.Boolreturn = true;
                    //if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                    //conf.StrTitleSelect += conf.Wselectedlabel;
                    //while (GetFilmList() == false) ; //keep calling while single folders found
                    //MesFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                    
                    while (!(dlg.SelectedLabel == -1))
                    {
                        dlg.Reset();
                        choiceSearch.Clear();
                        //dlg.SetHeading(string.Format(MesFilms.r[Convert.ToInt32(w_index[RandomNumber])]["Originaltitle"].ToString())); // menu
                        dlg.SetHeading(string.Format("W�hlen Sie eine Aktion aus")); // menu
                        //if (TrailerIsAvailable) // Sp�ter den Eintrag nur anzeigen, wenn auch ein Trailer verf�gbar ist (siehe Klassen option)
                        dlg.Add(string.Format("Hauptfilm spielen"));
                        choiceSearch.Add("PlayMovie");
                        dlg.Add(string.Format("Trailer spielen"));
                        choiceSearch.Add("PlayMovieTrailer");
                        dlg.Add(string.Format("Zeige Filmdetails")); //(goes to MesFilmsDetails ID7987 with selected record to show DetailScreen)
                        choiceSearch.Add("ShowMovieDetails");
                        dlg.Add(string.Format("Zeige alle Filme des gew�hlten Bereiches"));
                        choiceSearch.Add("ShowMovieList");
                        dlg.Add(string.Format("Neue Zufallssuche in gleicher Kategorie"));
                        choiceSearch.Add("RepeatSearch");
                        dlg.Add(string.Format("Neue globale Zufallssuche"));
                        choiceSearch.Add("NewSearch");
                        dlg.Add(string.Format("Zur�ck zum Hauptmen�", "Back"));
                        choiceSearch.Add("Back");
                        dlg.DoModal(GetID);
                        if ((dlg.SelectedLabel == -1) || (dlg.SelectedLabel == 6))
                            return;

                        switch (choiceSearch[dlg.SelectedLabel])
                        {
                            case "PlayMovie":
                                MesFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
                                //MesFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                                return;
                            case "PlayMovieTrailer":
                                //Hier mu� irgendwie sichergestellt werden, da� nach R�ckkehr keine Neuinitialisierung erfolgt (analog return von Details 7988
                                MovieScrobbling = true; //Set True to avoid reload menu after Return ...    
                                //MesFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                                //conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                                //Change_LayOut(MesFilms.conf.StrLayOut);
                                //conf.Boolreturn = true;
                                //if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                                //conf.StrTitleSelect += conf.Wselectedlabel;
                                //while (GetFilmList() == false) ; //keep calling while single folders found
                                
                                MesFilmsDetail.Launch_Movie_Trailer(facadeView.SelectedListItem.ItemId, 7990, null); //7990 To Return to this Dialog
                                // MesFilmsDetail.Launch_Movie_Trailer(1, GetID, m_SearchAnimation);
                                //MesFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);    
                                //GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                                GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                                dlgYesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
                                dlgYesNo.SetLine(1, MesFilms.r[Convert.ToInt32(w_index[RandomNumber])]["Originaltitle"].ToString());
                                dlgYesNo.SetLine(2, "Current ID = '" + w_index[RandomNumber] + "'");
                                dlgYesNo.DoModal(GetID);
                                if (dlgYesNo.IsConfirmed)
                                    MesFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, m_SearchAnimation);
                                    //MesFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                                break;
                            case "ShowMovieDetails":
                                // New Window for detailed selected item information
                                conf.StrIndex = facadeView.SelectedListItem.ItemId; //Guzzi: Mu� hier erst der facadeview geladen werden?
                                conf.StrTIndex = facadeView.SelectedListItem.Label;
                                GUITextureManager.CleanupThumbs();
                                GUIWindowManager.ActivateWindow(ID_MesFilmsDetail);
                                return;

                            case "ShowMovieList":
                                //GetFilmList(); //Is this necessary????
                                conf.StrIndex = facadeView.SelectedListItem.ItemId; //Guzzi: Mu� hier erst der facadeview geladen werden?
                                conf.StrTIndex = facadeView.SelectedListItem.Label;
                                GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                                dlg.DeInit();
                                return;

                            case "RepeatSearch":
                                RandomNumber = rnd.Next(w_index.Count + 1);
                                Log.Debug("MyFilms RandomNumber: '" + RandomNumber + "'");
                                //MesFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);


                                GUIDialogYesNo dlg1YesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                                dlg1YesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
                                dlg1YesNo.SetLine(1, GUILocalizeStrings.Get(219));
                                dlg1YesNo.SetLine(2, "Zuf�llige Film ID = '" + w_index[RandomNumber] + "'");
                                dlg1YesNo.DoModal(GetID);
                                if (dlg1YesNo.IsConfirmed)
                                    //Launch_Movie(select_item, GetID, m_SearchAnimation);
                                    MesFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
                                    //MesFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                                break;
                            case "NewSearch":
                                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                                dlgOk.SetLine(1, "");
                                dlgOk.SetLine(2, "Not yet implemented - be patient ....");
                                SearchMoviesbyRandomWithTrailer();
                                return;

                            case "Back":
                                return;

                            default:
                                {
                                    break;
                                }
                        }
                    }   
                    break;
            }
            Log.Debug("MyFilms (SearchRandomWithTrailer-Info): Here should happen the handling of menucontext....");
        }

        private void SearchMoviesbyAreas()
        {
            // first select the area where to make random search on - "all", "category", "year", "country"
            AntMovieCatalog ds = new AntMovieCatalog();
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            ArrayList w_tableau = new ArrayList();
            ArrayList wsub_tableau = new ArrayList();
            bool GetItems = false;
            bool GetSubItems = true;
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798645)); // menu for random search
            dlg.Add(GUILocalizeStrings.Get(10798622)); // random search on all
            choiceSearch.Add("randomall");
            dlg.Add(GUILocalizeStrings.Get(10798664)); // random search on Genre
            choiceSearch.Add("Category");
            dlg.Add(GUILocalizeStrings.Get(10798665)); // random search on year
            choiceSearch.Add("Year");
            dlg.Add(GUILocalizeStrings.Get(10798663)); // random search on country
            choiceSearch.Add("Country");
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            string wproperty = choiceSearch[dlg.SelectedLabel];
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            //            if (null == keyboard) return;
            keyboard.Reset();
            keyboard.Text = "";
            //            keyboard.DoModal(GetID);
            //            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
            switch (choiceSearch[dlg.SelectedLabel])
            {
                case "randomall":
                    conf.StrSelect = "";
                    conf.StrTxtSelect = "Selection [*]";
                    conf.StrTitleSelect = "";


                    // Temporarily Enabled for Testing
                    //getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                    GetFilmList();
                    break;

                default:
                    ArrayList w_count = new ArrayList();
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
                    //Modified to checked for GlobalFilterString
                    DataRow[] wr = BaseMesFilms.LectureDonn�es(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                    //DataColumn[] wc = BaseMesFilms.LectureDonn�es(conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                    w_tableau.Add(string.Format(GUILocalizeStrings.Get(10798623))); //Add Defaultgroup for invalid or empty properties
                    w_count.Add(0);
                    foreach (DataRow wsr in wr)
                    {
                        foreach (DataColumn dc in ds.Movie.Columns)
                        {
                            if (dc.ToString().Contains(wproperty))  // Only count property chosen
                            {
                                if (wsr[dc.ColumnName].ToString().Length == 0) //Empty property special handling
                                {
                                    w_count[0] = (int)w_count[0] + 1;
                                    break;
                                }
                                else
                                {
                                    //Log.Debug("MyFilms (Guzzi) AddDistinctClasses: " + "Property: " + dc.ToString() + " and Value: '" + wsr[dc.ColumnName].ToString() + "'");
                                    // column Name contains propertyname : added to w_tableau + w_count
                                    if (GetSubItems)
                                    {
                                        Log.Debug("MyFilms SubItemGrabber: Input: " + wsr[dc.ColumnName].ToString());
                                        wsub_tableau = SubItemGrabbing(wsr[dc.ColumnName].ToString()); //Grab SubItems
                                        foreach (object t in wsub_tableau)
                                        {
                                            Log.Debug("MyFilms SubItemGrabber: Output: " + t.ToString());
                                            {
                                                if (w_tableau.Contains(t.ToString())) // search position in w_tableau for adding +1 to w_count
                                                {
                                                    //if (!w_index.Contains(
                                                    for (int i = 0; i < w_tableau.Count; i++)
                                                    {
                                                        if (w_tableau[i].ToString() == t.ToString())
                                                        {
                                                            w_count[i] = (int)w_count[i] + 1;
                                                            //Log.Debug("MyFilms SubItemGrabber: add Counter for '" + wsub_tableau[wi].ToString() + "'");
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                    // add to w_tableau and move 1 to w_count
                                                {
                                                    Log.Debug("MyFilms SubItemGrabber: add new Entry for '" + wsr[dc.ColumnName].ToString() + "'");
                                                    w_tableau.Add(t.ToString());
                                                    w_count.Add(1);
                                                }
                                            }
                                        }

                                    }
                                    if (GetItems)
                                    {
                                        if (w_tableau.Contains(wsr[dc.ColumnName])) // search position in w_tableau for adding +1 to w_count
                                        {
                                            for (int i = 0; i < w_tableau.Count; i++)
                                            {
                                                if (w_tableau[i].ToString() == wsr[dc.ColumnName].ToString())
                                                {
                                                    w_count[i] = (int)w_count[i] + 1;
                                                    //Log.Debug("MyFilms (Guzzi) Clas already present, adding Counter for Property: " + dc.ToString() + "Value: '" + wsr[dc.ColumnName].ToString() + "'");
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        // add to w_tableau and move 1 to w_count
                                        {
                                            //Log.Debug("MyFilms (Guzzi) AddDistinctClasses with Property: '" + dc.ToString() + "' and Value '" + wsr[dc.ColumnName].ToString() + "'");
                                            w_tableau.Add(wsr[dc.ColumnName].ToString());
                                            w_count.Add(1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (w_tableau.Count == 0)
                    {
                        Log.Debug("MyFilms PropertyClassCount is 0");
                        break;
                    }


                    string wproperty2 = "";

                    if (!(wproperty == "randomall"))
                    {
                        dlg.Reset();
                        dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), wproperty)); // menu
                        choiceSearch.Clear();
                        //w_tableau = Array.Sort(w_tableau);
                        for (int i = 0; i < w_tableau.Count; i++)
                        {
                            dlg.Add(string.Format(GUILocalizeStrings.Get(10798626), w_count[i], w_tableau[i]));
                            choiceSearch.Add(w_tableau[i].ToString());
                        }
                        dlg.DoModal(GetID);
                        if (dlg.SelectedLabel == -1)
                            return;
                        string t_wproperty2 = choiceSearch[dlg.SelectedLabel];
                        wproperty2 = t_wproperty2;
                    }
                    else
                        wproperty2 = "*";

                    Log.Debug("MyFilms (RandomMovies) - Chosen Subcategory: '" + wproperty2 + "' selecting in '" + wproperty + "'");
                    switch (wproperty)
                    {
                        case "Rating":
                            conf.StrSelect = wproperty + " = " + wproperty2;
                            break;
                        case "Number":
                            conf.StrSelect = wproperty + " = " + wproperty2;
                            break;
                        default:
                            if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623))) // Check, if emptypropertystring is set
                                conf.StrSelect = wproperty + " is NULL";
                            else
                                conf.StrSelect = wproperty + " like '*" + wproperty2 + "*'";
                            break;
                    }
                    Log.Debug("MyFilms (RandomMovies) - resulting conf.StrSelect: '" + conf.StrSelect + "'");
                    conf.StrTxtSelect = "Selection " + wproperty + " [*" + wproperty2 + @"*]";
                    conf.StrTitleSelect = "";


                    // Temporarily Enabled for Testing
                    //getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                    GetFilmList();
                    break;
            }
            // Select RandomMovie from Filmlist and process 
            // MesFilmsDetail.Launch_Movie_Trailer(facadeView.SelectedListItem.ItemId, GetID, null);


        }

        //*****************************************************************************************
        //*  Global search movies by properties     (Guzzi Version)                               *
        //*****************************************************************************************
        private void SearchMoviesbyProperties()
        {
            // first select the property to be searching on
            AntMovieCatalog ds = new AntMovieCatalog();
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            ArrayList w_tableau = new ArrayList();
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798615)); // menu
            dlg.Add(GUILocalizeStrings.Get(10798616)); // search on all fields
            choiceSearch.Add("all");
            //Sorted lists - manually adding items to have them in right order
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(10798659));
            choiceSearch.Add("TranslatedTitle");
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(10798658));
            choiceSearch.Add("OriginalTitle");
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(10798669));
            choiceSearch.Add("Description");
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(10798670));
            choiceSearch.Add("Comments");
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(1079868));
            choiceSearch.Add("Actors");
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(10798661));
            choiceSearch.Add("Director");
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(10798662));
            choiceSearch.Add("Producer");
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(10798657));
            choiceSearch.Add("Rating");
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(10798665));
            choiceSearch.Add("Year");
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(10798679));
            choiceSearch.Add("Date");
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(10798664));
            choiceSearch.Add("Category");
            dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(10798663));
            choiceSearch.Add("Country");

            foreach (DataColumn dc in ds.Movie.Columns)
            {
                switch (dc.ColumnName)
                {
                    //<entry name="SearchList">TranslatedTitle|OriginalTitle|Description|Comments|Actors|Director|Producer|Year|Date|Category|Country|Rating|Checked|MediaLabel|MediaType|URL|Borrower|Length|VideoFormat|VideoBitrate|AudioFormat|AudioBitrate|Resolution|Framerate|Size|Disks|Languages|Subtitles|Number</entry>
                    //<entry name="AllItems..">TranslatedTitle|OriginalTitle|FormattedTitle|Description|Comments|Actors|Director|Producer|Rating|Country|Category|Year|Checked|MediaLabel|MediaType|Source|Date|Borrower|Length|URL|VideoFormat|VideoBitrate|AudioFormat|AudioBitrate|Resolution|Framerate|Languages|Subtitles|DateAdded|Size|Disks|Picture|Contents_Id|Number</entry>
                    //<entry name="UpdateList">TranslatedTitle|OriginalTitle|Category|Year|Date|Country|Rating|Checked|MediaLabel|MediaType|Actors|Director|Producer</entry>
                    case "Contents_Id":
                    case "Length_Num":
                    case "DateAdded":
                    case "Picture":
                    case "Size":
                    case "AudioBitrate":
                    case "AudioFormat":
                    case "VideoBitrate":
                    case "VideoFormat":
                    case "Checked":
                    case "Disks":
                    case "Borrower":
                        break; //Do not search in those properties !
                    
                    default:
                        //Guzzi:Disabled
                        //dlg.Add(GUILocalizeStrings.Get(10798617) + dc.ColumnName);
                        //choiceSearch.Add(dc.ColumnName);
                        break;
                }
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            string wproperty = choiceSearch[dlg.SelectedLabel];
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            if (null == keyboard) return;
            keyboard.Reset();
            keyboard.Text = "";
            keyboard.DoModal(GetID);
            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
            {
            ArrayList w_count = new ArrayList(); 
                switch (choiceSearch[dlg.SelectedLabel])
                {
                    case "all":
                        if (dlg == null) return;
                        dlg.Reset();
                        dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
                        // Added GlobalFilterString to take Global Filters effective
                        DataRow[] wr = BaseMesFilms.LectureDonn�es(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                        Log.Debug("MyFilms (GlobalSearchAll) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
                        Log.Debug("MyFilms (GlobalSearchAll) - conf.StrTitle1    : [" + conf.StrTitle1.ToString() + " like '*']");
                        Log.Debug("MyFilms (GlobalSearchAll) - conf.StrSorta     : '" + conf.StrSorta + "'");
                        Log.Debug("MyFilms (GlobalSearchAll) - conf.StrSortSens  : '" + conf.StrSortSens + "'");
                        Log.Debug("MyFilms (GlobalSearchAll) - searchStringKBD   : '" + keyboard.Text + "'"); 
                        foreach (DataRow wsr in wr)
                        {
                            foreach (DataColumn dc in ds.Movie.Columns)
                            {
                                if (wsr[dc.ColumnName].ToString().ToLower().Contains(keyboard.Text.ToLower()))
                                    // column contains text searched on : added to w_tableau + w_count
                                    if (w_tableau.Contains(dc.ColumnName.ToLower()))
                                    // search position in w_tableau for adding +1 to w_count
                                    {
                                        for (int i = 0; i < w_tableau.Count; i++)
                                        {
                                            if (w_tableau[i].ToString() == dc.ColumnName.ToLower())
                                            {
                                                w_count[i] = (int)w_count[i] + 1;
                                                //Log.Debug("MyFilms (GlobalSearchAll) - AddCount for: '" + i.ToString() + "' - '" + dc.ColumnName.ToString() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    // add to w_tableau and move 1 to w_count
                                    {
                                        w_tableau.Add(dc.ColumnName.ToString().ToLower());
                                        w_count.Add(1);
                                        //Log.Debug("MyFilms (GlobalSearchAll) - AddProperty for: '" + dc.ColumnName.ToString().ToLower() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                                    }
                            }
                        }
                        Log.Debug("MyFilms (GlobalSearchAll) - Result of Search in all properties (w_tableau.Count): '" + w_tableau.Count.ToString() + "'"); 
                        if (w_tableau.Count == 0) // NodeLabelEditEventArgs Results found
                        {
                            GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                            dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));//InfoPanel
                            dlgOk.SetLine(1, GUILocalizeStrings.Get(10798625));
                            dlgOk.DoModal(GetID);
                            if (dlg.SelectedLabel == -1)
                                return;
                            break;
                        }
                        dlg.Reset();
                        dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), keyboard.Text)); // menu & SearchString
                        choiceSearch.Clear();
                        string[] PropertyList = new string[] { "TranslatedTitle", "OriginalTitle", "Description", "Comments", "Actors", "Director", "Producer", "Year", "Date", "Category", "Country", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked", "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate", "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "Borrower" };
                        string[] PropertyListLabel = new string[] { "10798659", "10798658", "10798669", "10798670", "10798667", "10798661", "10798662", "10798665", "10798655", "10798664", "10798663", "10798657", "10798677", "10798678", "10798660", "10798651", "10798652", "10798653", "10798666", "10798671", "10798672", "10798673", "10798674", "10798675", "10798676", "10798680", "10798681", "10798650", "10798668", "10798656" };
                        for (int ii = 0; ii < 30; ii++)
                        {
                            //Log.Debug("MyFilms (GlobalSearchAll) - OutputSort: Property is '" + PropertyList[ii] + "' - '" + GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + "' (" + PropertyListLabel[ii] + ")");
                            for (int i = 0; i < w_tableau.Count; i++)
                            {
                                //Log.Debug("MyFilms (GlobalSearchAll) - OutputSort: w_tableau is '" + w_tableau[i] + "'"); 
                                if (w_tableau[i].ToString().ToLower().Equals(PropertyList[ii].ToString().ToLower()))
                                {
                                    dlg.Add(string.Format(GUILocalizeStrings.Get(10798619), w_count[i], GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii])))));
                                    choiceSearch.Add(w_tableau[i].ToString());
                                }
                            }
                        }
                        dlg.DoModal(GetID);
                        if (dlg.SelectedLabel == -1)
                            return;
                        wproperty = choiceSearch[dlg.SelectedLabel];
                        if (control_searchText(keyboard.Text))
                        {

                            //Log.Debug("MyFilms (GlobalSearchAll) - ChosenProperty: wproperty is '" + wproperty + "'"); 
                            switch (wproperty)
                            {
                                case "rating":
                                    conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                                    break;
                                case "number":
                                    conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                                    break;
                                default:
                                    conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
                                    break;
                            }
                            conf.StrTxtSelect = "Selection " + wproperty + " [*" + keyboard.Text + @"*]";
                            conf.StrTitleSelect = "";
                            // getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                            GetFilmList();
                        }
                        break;
                    default:
                        Log.Debug("MyFilms (GlobalSearchAll) - ChosenProperty: wproperty is '" + wproperty + "'");
                        Log.Debug("MyFilms (GlobalSearchAll) - ChosenProperty: SearchTest is '" + keyboard.Text + "'"); 
                        if (control_searchText(keyboard.Text))
                        {
                            // Added GloablaFilterString to make filters effective
                            DataRow[] wdr = BaseMesFilms.LectureDonn�es(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                            Log.Debug("MyFilms (GlobalSearchAll) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
                            Log.Debug("MyFilms (GlobalSearchAll) - conf.StrTitle1    : [" + conf.StrTitle1.ToString() + " like '*']");
                            Log.Debug("MyFilms (GlobalSearchAll) - conf.StrSorta     : '" + conf.StrSorta + "'");
                            Log.Debug("MyFilms (GlobalSearchAll) - conf.StrSortSens  : '" + conf.StrSortSens + "'");
                            Log.Debug("MyFilms (GlobalSearchAll) - searchStringKBD   : '" + keyboard.Text + "'");
                            foreach (DataRow wsr in wdr)
                            {
                                foreach (DataColumn dc in ds.Movie.Columns)
                                {
                                    if (dc.ColumnName.ToLower() == wproperty.ToLower())
                                    {
                                        if (wsr[dc.ColumnName].ToString().ToLower().Contains(keyboard.Text.ToLower()))
                                            // column contains text searched on : added to w_tableau + w_count
                                            if (w_tableau.Contains(dc.ColumnName.ToLower()))
                                            // search position in w_tableau for adding +1 to w_count
                                            {
                                                for (int i = 0; i < w_tableau.Count; i++)
                                                {
                                                    if (w_tableau[i].ToString() == dc.ColumnName.ToLower())
                                                    {
                                                        w_count[i] = (int)w_count[i] + 1;
                                                        //Log.Debug("MyFilms (GlobalSearchAll) - AddCount for: '" + i.ToString() + "' - '" + dc.ColumnName.ToString() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            // add to w_tableau and move 1 to w_count
                                            {
                                                w_tableau.Add(dc.ColumnName.ToString().ToLower());
                                                w_count.Add(1);
                                                //Log.Debug("MyFilms (GlobalSearchAll) - AddProperty for: '" + dc.ColumnName.ToString().ToLower() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                                            }
                                    }
                                }
                            }
                            Log.Debug("MyFilms (GlobalSearchAll) - Result of Search in all properties (w_tableau.Count): '" + w_tableau.Count.ToString() + "'");
                            if (w_tableau.Count == 0) // NodeLabelEditEventArgs Results found
                            {
                                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                                dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));//InfoPanel
                                dlgOk.SetLine(1, GUILocalizeStrings.Get(10798625));
                                dlgOk.DoModal(GetID);
                                if (dlg.SelectedLabel == -1)
                                    return;
                                break;
                            }

                            switch (wproperty)
                            {
                                case "Rating":
                                    conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                                    break;
                                case "Number":
                                    conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                                    break;
                                default:
                                    conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
                                    break;
                            }
                            conf.StrTxtSelect = "Selection " + wproperty + " [*" + keyboard.Text + @"*]";
                            conf.StrTitleSelect = "";
                            //                         getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
                            GetFilmList();
                        }
                        break;
                }
            }
        }

        //*****************************************************************************************
        //*  No Movie found to display. Display all movies
        //*****************************************************************************************
        private void DisplayAllMovies()
        {
            MesFilms.conf.StrFilmSelect = MesFilms.conf.StrTitle1.ToString() + " not like ''";
            conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
            conf.WStrSort = conf.StrSTitle;
            conf.Boolselect = false;
            conf.Boolreturn = false;
            r = BaseMesFilms.LectureDonn�es(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, true);
        }

        //*****************************************************************************************
        //*  Initialize Fields on Main screen                                                     *
        //*****************************************************************************************
        private void InitMainScreen()
        {
            Log.Debug("MyFilms (InitMainScreen) - Initialize all properties !!!"); 
            MovieScrobbling = false; //Reset MovieScrobbling
            MesFilmsDetail.Init_Detailed_DB();

            // create Backdrop image swapper
            backdrop = new ImageSwapper();
            backdrop.ImageResource.Delay = 250;
            backdrop.Active = true;
            //if (MesFilms.conf.StrFanart)
            //    backdrop.Active = true;
            //else
            //    backdrop.Active = false;
            //backdrop.PropertyOne = "#myfilms.fanart";
            //backdrop.PropertyTwo = "#myfilms.fanart2";
            //backdrop.LoadingImage = loadingImage; --> Do NOT activate - otherwise coverimage flickers and goes away !!!!

            // create Cover image swapper
            //cover = new AsyncImageResource();
            //cover.Property = "#myfilms.coverimage";
            //cover.Delay = 250;
            cover.Filename = "";

            // (re)link our backdrop image controls to the backdrop image swapper
            backdrop.GUIImageOne = ImgFanart;
            backdrop.GUIImageTwo = ImgFanart2;
            //backdrop.LoadingImage = loadingImage;  --> Do NOT activate - otherwise coverimage flickers and goes away !!!!

            //ImgFanart.SetVisibleCondition(1, false); //Added by ZebonsMerge ->> This fucked up the fanart swapper !!!!!
            //ImgFanart2.SetFileName(string.Empty); //Added by ZebonsMerge
            //GUIPropertyManager.SetProperty("#myfilms.Fanart", " ");
            //GUIPropertyManager.SetProperty("#myfilms.Fanart2", " ");
            GUIPropertyManager.SetProperty("#myfilms.logos_id2001", " ");
            GUIPropertyManager.SetProperty("#myfilms.logos_id2002", " ");
            GUIPropertyManager.SetProperty("#myfilms.nbobjects", " ");
            GUIPropertyManager.SetProperty("#myfilms.label1", " ");
            GUIPropertyManager.SetProperty("#myfilms.label2", " ");
            GUIPropertyManager.SetProperty("#myfilms.item1", " ");
            GUIPropertyManager.SetProperty("#myfilms.item2", " ");
            GUIPropertyManager.SetProperty("#myfilms.item3", " ");
            //GUIPropertyManager.SetProperty("#myfilms.select", " "); // still above in init routine present, here would be doubled !
            GUIPropertyManager.SetProperty("#myfilms.rating", "0");
            affichage_rating(0);
            GUIWaitCursor.Hide();
            GUIControl.HideControl(GetID, 34);
        }

        //*****************************************************************************************
        //*  Ask for Title search and grab information on the NET base on the grab configuration  *
        //*****************************************************************************************
        private void GetTitleGrab()
        {
        }

        //*****************************************************************************************
        //*  Update Database in batch mode                                                        *
        //*****************************************************************************************
        public void AsynUpdateDatabase()
        {
            if (!bgUpdateDB.IsBusy)
            {
                bgUpdateDB.RunWorkerAsync(MesFilms.conf.StrTIndex);
                Log.Info("MyFilms : launching AMCUpdater in batch mode");

            }
        }

        static void bgUpdateDB_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            MesFilmsDetail.RunProgram(MesFilms.conf.StrAMCUpd_exe, MesFilms.conf.StrAMCUpd_cnf);
        }

        void bgUpdateDB_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Log.Info("MyFilms : Update database with AMCUpdater finished");
            if (GetID == 7986)
            {
                Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
                Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
                Load_Config(Configuration.CurrentConfig, true);
            }
        }

        //*****************************************************************************************
        //*  Download Backdrop Fanart in Batch mode                                               *
        //*****************************************************************************************
        public void AsynUpdateFanart()
        {
            if (!bgUpdateFanart.IsBusy)
            {
                bgUpdateFanart.RunWorkerAsync(MesFilms.r);
                Log.Info("MyFilms : Downloading backdrop fanart in batch mode");

            }
        }

        static void bgUpdateFanart_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            string wtitle = string.Empty;
            foreach (DataRow t in MesFilms.r)
            {
                if (t["OriginalTitle"] != null && t["OriginalTitle"].ToString().Length > 0)
                    wtitle = t["OriginalTitle"].ToString();
                if (wtitle.IndexOf(MesFilms.conf.TitleDelim) > 0)
                    wtitle = wtitle.Substring(wtitle.IndexOf(MesFilms.conf.TitleDelim) + 1);
                string wttitle = string.Empty;
                if (t["TranslatedTitle"] != null && t["TranslatedTitle"].ToString().Length > 0)
                    wttitle = t["TranslatedTitle"].ToString();
                if (wttitle.IndexOf(MesFilms.conf.TitleDelim) > 0)
                    wttitle = wttitle.Substring(wttitle.IndexOf(MesFilms.conf.TitleDelim) + 1);
                if (t["OriginalTitle"].ToString().Length > 0)

                {
                    int wyear = 0;
                    try { wyear = System.Convert.ToInt16(t["Year"]); }
                    catch { }
                    string wdirector = string.Empty;
                    try { wdirector = (string)t["Director"]; }
                    catch { }
                    System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, wdirector, MesFilms.conf.StrPathFanart, true, false, MesFilms.conf.StrTitle1.ToString());
                    //System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, wdirector, MesFilms.conf.StrPathFanart, true, false);
                }
            }
        }

        static void bgUpdateFanart_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Log.Info("MyFilms : Backdrop Fanart download finished");
        }

        //*****************************************************************************************
        //*  Load List movie file in batch mode                                                   *
        //*****************************************************************************************
        public void AsynLoadMovieList()
        {
            if (!bgLoadMovieList.IsBusy)
            {
                bgLoadMovieList.RunWorkerAsync();
                Log.Info("MyFilms : Loading Movie List in batch mode");
            }
        }

        static void bgLoadMovieList_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string searchrep = MesFilms.conf.StrDirStor;
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
            System.Text.RegularExpressions.Regex oRegex= new System.Text.RegularExpressions.Regex(";");
            string[] SearchDir = oRegex.Split(searchrep);
            foreach (string path in SearchDir)
            {
                MesFilms.conf.MovieList.Add(System.IO.Directory.GetFiles(path));
                if ((MesFilms.conf.SearchSubDirs == "no") || (!System.IO.Directory.Exists(path))) continue;
                foreach (string sFolderSub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
                {
                    MesFilms.conf.MovieList.Add(System.IO.Directory.GetFiles(sFolderSub));
                }
            }
        }

        static void bgLoadMovieList_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Log.Info("MyFilms : Loading Movie List in batch mode finished");
        }

        private void FanartTimerEvent(object state)
        {
            //Log.Debug("MyFilms (FanartTimerEvent): FanartTimerEvent triggered !!!");
            //Log.Debug("MyFilms (FanartTimerEvent): Current Setting of Backdrop: '" + backdrop.Filename.ToString() + "'");
            // ToDo: Load new Fanart here !!!
            //if ((CurrentMovie.Length > 0) && (backdrop.Filename.Length > 0) && (backdrop.Active == true))
            if (backdrop.Filename != null)
                {
                Log.Debug("MyFilms (FanartTimerEvent): loadFanart triggered for '" + facadeView.SelectedListItem.Label + "' !");
                //Log.Debug("MyFilms (FanartTimerEvent): loadFanart triggered for '" + CurrentMovie.ToString() + "' !");
                //Log.Debug("MyFilms (FanartTimerEvent): loadFanart CurrentFanartDir '" + CurrentFanartDir.ToString() + "' !");
                //Disabled, because it's still not working ...
                loadFanart();
            }
            else
            {
                Log.Debug("MyFilms (FanartTimerEvent): loadFanart NOT triggered !");
            }
            m_bFanartTimerDisabled = false;
        }

        bool fanartSet = false;
        //Fanart currSeriesFanart = null;

        private bool loadFanart()
        //private bool loadFanart(DBTable item)
        {
            if (backdrop == null)
            {
                // Fanart not supported by skin, exit now
                fanartSet = false;
                return false;
            }

            string fanart = "";
            string fanartdir = "";
            string wtitle2 = MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"].ToString();

            // Get FanartDirectory
            if (!(MesFilms.conf.StrTitle1 == "OriginalTitle"))
            {
                if (MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"] != null && MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                    wtitle2 = MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString();
            }

            if (wtitle2.Contains(MesFilms.conf.TitleDelim))
                wtitle2 = wtitle2.Substring(wtitle2.LastIndexOf(MesFilms.conf.TitleDelim) + 1).Trim();
            Log.Debug("MyFilms (FindFanart) - wtitle name = '" + wtitle2 + "'");
            //wtitle2 = Grabber.GrabUtil.CreateFilename(wtitle2.ToLower()).Replace(' ', '.');

            fanartdir = MesFilms.conf.StrPathFanart.ToString() + "\\{" + wtitle2 + "}";
            Log.Debug("MyFilms (FindFanart) - fanartdir = '" + fanartdir.ToString() + "'");
            
            
            
            try
            {
                //Log.Debug("MyFilms (loadFanart): Load Fanart by Timer for activemovie: '" + "'");
                //Fanart fanart = currSeriesFanart;
                //DBSeries series = item as DBSeries;

                // Get a Fanart for selected movie
                
                
                //FindFanart(); // first modify the method before calling it here ....
                fanart = "test";

                if (fanart == null)
                {
                    // This shouldn't happen
                    Log.Debug("MyFilms (loadFanart): Fanart is unavailable, disabling");
                    DisableFanart();
                    return false;
                }

                // Activate Backdrop in Image Swapper                
                if (!backdrop.Active) backdrop.Active = true;

                // Assign Fanart filename to Image Loader
                // Will display fanart in backdrop or reset to default background                
                // backdrop.Filename = "test";
                Log.Debug("MyFilms (loadFanart): Loaded fanart is: '" + backdrop.Filename.ToString() + "'");
                return fanartSet = true;
            }
            catch (Exception ex)
            {
                Log.Debug("MyFilms (loadFanart): Failed to load Fanart: " + ex.Message);
                return fanartSet = false;
            }
        }
        
        private bool FindFanart()
        {
            Log.Debug("MyFilms (FindFanart): Started FanartSearch");
            if (MesFilms.conf.StrFanart)
            {};
            string[] wfanart = new string[2];
            wfanart[0] = " ";
            wfanart[1] = " ";
            //Search Fanarts
            string wtitle2 = MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"].ToString();
            Log.Debug("MyFilms (FindFanart) - wtitle old = '" + wtitle2 + "'");
            //Added by Guzzi to fix Fanartproblem when Mastertitle is set to OriginalTitle
            if (!(MesFilms.conf.StrTitle1 == "OriginalTitle"))
                {
                    if (MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"] != null && MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                        wtitle2 = MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                }

            if (wtitle2.Contains(MesFilms.conf.TitleDelim))
                wtitle2 = wtitle2.Substring(wtitle2.LastIndexOf(MesFilms.conf.TitleDelim) + 1).Trim();
            Log.Debug("MyFilms (FindFanart) - wtitle name = '" + wtitle2 + "'");
            wtitle2 = Grabber.GrabUtil.CreateFilename(wtitle2.ToLower()).Replace(' ', '.');
            Log.Debug("MyFilms (FindFanart) - wtitle filename = '" + wtitle2 + "'");
            Log.Debug("MyFilms (FindFanart) - MesFilms.conf.StrFanart = '" + MesFilms.conf.StrFanart + "'");

            string safeName = string.Empty;
            safeName = MesFilms.conf.StrPathFanart + "\\{" + wtitle2 + "}";
            //Log.Debug("MyFilms (FindFanart) - Directory (safename) = '" + safeName.ToString() + "'");
            FileInfo wfile = new FileInfo(safeName + "\\{" + wtitle2 + "}.jpg");
            //Log.Debug("MyFilms (FindFanart) - FullPath (safename file&ext) = '" + wfile.ToString() + "'");

            // Single File
            //wfanart[0] = safeName + "\\{" + wtitle2 + "}.jpg";
            //wfanart[1] = "file";

            if (System.IO.Directory.Exists(safeName))
            {
                //file
                if (System.IO.Directory.GetFiles(safeName).Length > 0)
                    {
                        wfanart[0] = System.IO.Directory.GetFiles(safeName)[0].ToString();
                        wfanart[1] = "file";
                    }
                //dir    
                if (System.IO.Directory.GetFiles(safeName).Length > 0)
                    {
                        wfanart[0] = safeName;
                        wfanart[1] = "dir";
                    }
            }
            else
            {
                try { System.IO.Directory.CreateDirectory(safeName); }
                catch { }
            }

            ArrayList result = new ArrayList();
            ArrayList resultsize = new ArrayList();
            string[] filesfound = new string[100];
            Int64[] filesfoundsize = new Int64[100];
            int filesfoundcounter = 0;
            Int64 wsize = 0; // Temporary Filesize detection
            //Search Files in Mediadirectory (used befor: SearchFiles("trailer", directoryname, true, true);)
            string[] files = Directory.GetFiles(MesFilms.conf.StrPathFanart + "\\{" + wtitle2 + "}", "*.*", SearchOption.AllDirectories);
            foreach (string filefound in files)
            {
                Log.Debug("MyFilms (FanartSearchLocal) - Processing FilesFound: '" + filefound.ToString() + "'");
                if (Utils.IsPicture(filefound))
                {
                    wsize = new System.IO.FileInfo(filefound).Length;
                    result.Add(filefound);
                    resultsize.Add(wsize);
                    filesfound[filesfoundcounter] = filefound;
                    filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                    filesfoundcounter = filesfoundcounter + 1;
                    Log.Debug("MyFilms (FanartSearchLocal) - FilesFound in FanartDirectory: Size-Name '" + wsize.ToString() + "' - Name '" + filefound + "'");
                }
            }

            Log.Debug("MesFilm (FindFanart): Results for wfanart[1,2]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
            if (wfanart[0] == " ")
            {
                //No Fanart available ...
                return false;
            }
            else
            {
                //Choose random fanart and return it ....
                return false;
            }
        }

        private void DisableFanart()
        {
            // Disable Random Fanart Timer
            m_FanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
            m_bFanartTimerDisabled = true;

            // Disable Fanart                
            if (backdrop.Active) backdrop.Active = false;
            backdrop.Filename = "";
            Log.Debug("MyFilms (DisableFanart): Fanart disabled !");
        }

        //-------------------------------------------------------------------------------------------
        //  Search All Fanart for a given movie
        //-------------------------------------------------------------------------------------------        
        public static void SearchFanart(DataRow[] r1, int Index, bool ExtendedSearch)
        {
            //Searchdirectory:
            Log.Debug("MyFilmsDetails (SearchtrailerLocal) - StrDirStortrailer: " + MesFilms.conf.StrDirStorTrailer);
            //Title1 = Movietitle
            Log.Debug("MyFilmsDetails (SearchTrailerLocal) - MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1] : '" + MesFilms.r[Index][MesFilms.conf.StrTitle1].ToString() + "'");
            //Title2 = Translated Movietitle
            Log.Debug("MyFilmsDetails (SearchTrailerLocal) - MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle2] : '" + MesFilms.r[Index][MesFilms.conf.StrTitle2].ToString() + "'");
            //Cleaned Title
            Log.Debug("MyFilmsDetails (SearchTrailerLocal) - Cleaned Title                                               : '" + MediaPortal.Util.Utils.FilterFileName(MesFilms.r[Index][MesFilms.conf.StrTitle1].ToString().ToLower()) + "'");
            //Index of facadeview?
            Log.Debug("MyFilmsDetails (SearchtrailerLocal) - Index: '" + Index + "'");
            //Full Path to Film
            Log.Debug("MyFilmsDetails (SearchtrailerLocal) - FullMediasource: '" + (string)MesFilms.r[Index][MesFilms.conf.StrStorage].ToString().Trim() + "'");

            ArrayList result = new ArrayList();
            ArrayList resultsize = new ArrayList();
            string[] filesfound = new string[100];
            Int64[] filesfoundsize = new Int64[100];
            int filesfoundcounter = 0;
            string file = MesFilms.r[Index][MesFilms.conf.StrTitle1].ToString();
            string titlename = MesFilms.r[Index][MesFilms.conf.StrTitle1].ToString();
            string titlename2 = MesFilms.r[Index][MesFilms.conf.StrTitle2].ToString();
            string directoryname = "";
            string movieName = "";
            Int64 wsize = 0; // Temporary Filesize detection
            // split searchpath information delimited by semicolumn (multiple searchpathes from config)
            string[] Trailerdirectories = MesFilms.conf.StrDirStorTrailer.ToString().Split(new Char[] { ';' });
            Log.Debug("MyFilmsDetails (SearchtrailerLocal) Search for '" + file + "' in '" + MesFilms.conf.StrDirStorTrailer.ToString() + "'");

            //Retrieve original directory of mediafiles
            //directoryname
            movieName = (string)MesFilms.r[Index][MesFilms.conf.StrStorage].ToString().Trim();
            movieName = movieName.Substring(movieName.LastIndexOf(";") + 1);
            Log.Debug("MyFilmsDetails (SearchtrailerLocal) Splittet Mediadirectoryname: '" + movieName.ToString() + "'");
            try
            { directoryname = System.IO.Path.GetDirectoryName(movieName); }
            catch
            { directoryname = ""; }
            Log.Debug("MyFilmsDetails (SearchtrailerLocal) Get Mediadirectoryname: '" + directoryname.ToString() + "'");


            //Search Files in Mediadirectory (used befor: SearchFiles("trailer", directoryname, true, true);)
            string[] files = Directory.GetFiles(directoryname, "*.*", SearchOption.AllDirectories);
            foreach (string filefound in files)
            {
                if (((filefound.ToString().ToLower().Contains("trailer")) || (filefound.ToString().ToLower().Contains("trlr"))) && (Utils.IsVideo(filefound)))
                {
                    wsize = new System.IO.FileInfo(filefound).Length;
                    result.Add(filefound);
                    resultsize.Add(wsize);
                    filesfound[filesfoundcounter] = filefound;
                    filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                    filesfoundcounter = filesfoundcounter + 1;
                    Log.Debug("MyFilms (TrailersearchLocal) - FilesFound in MediaDir: Size '" + wsize.ToString() + "' - Name '" + filefound + "'");
                }
            }

            //Search Filenames with "title" in Trailer Searchpath
            string[] directories;
            if (ExtendedSearch)
            {
                foreach (string storage in Trailerdirectories)
                {
                    Log.Debug("MyFilms (TrailersearchLocal) - TrailerSearchDirectoriy: '" + storage + "'");
                    directories = Directory.GetDirectories(storage, "*.*", SearchOption.AllDirectories);
                    foreach (string directoryfound in directories)
                    {
                        if ((directoryfound.ToString().ToLower().Contains(titlename.ToLower())) || (directoryfound.ToString().ToLower().Contains(titlename2.ToLower())))
                        {
                            Log.Debug("MyFilms (TrailersearchLocal) - Directory found: '" + directoryfound + "'");
                            files = Directory.GetFiles(directoryfound, "*.*", SearchOption.AllDirectories);
                            foreach (string filefound in files)
                            {
                                if (Utils.IsVideo(filefound))
                                {
                                    wsize = new System.IO.FileInfo(filefound).Length;
                                    result.Add(filefound);
                                    resultsize.Add(wsize);
                                    filesfound[filesfoundcounter] = filefound;
                                    filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                                    filesfoundcounter = filesfoundcounter + 1;
                                    Log.Debug("MyFilms (TrailersearchLocal) - Files added matching Directory: Size '" + wsize.ToString() + "' - Name '" + filefound + "'");
                                }
                            }

                        }
                        else
                        {
                            files = Directory.GetFiles(directoryfound, "*.*", SearchOption.AllDirectories);
                            foreach (string filefound in files)
                            {
                                if (((filefound.ToString().ToLower().Contains(titlename.ToLower())) || (filefound.ToString().ToLower().Contains(titlename2.ToLower()))) && (Utils.IsVideo(filefound)))
                                {
                                    wsize = new System.IO.FileInfo(filefound).Length;
                                    result.Add(filefound);
                                    resultsize.Add(wsize);
                                    filesfound[filesfoundcounter] = filefound;
                                    filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                                    filesfoundcounter = filesfoundcounter + 1;
                                    Log.Debug("MyFilms (TrailersearchLocal) - Singlefiles found in TrailerDIR: Size '" + wsize.ToString() + "' - Name '" + filefound + "'");
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
                Log.Debug("MyFilms (Sorted Trailerfiles) ******* : '" + n + "'");

            Array.Sort(filesfoundsize);
            for (int i = 0; i < result.Count; i++)
            {
                Log.Debug("MyFilms (Sorted Trailerfiles) ******* : Number: '" + i + "' - Size: '" + filesfoundsize[i] + "' - Name: '" + filesfound[i] + "'");
            }

            string trailersourcepath = "";

            if (result.Count != 0)
            {
                //result.Sort();
                trailersourcepath = result[0].ToString();
                //ArrayList wresult = new ArrayList();
                //foreach (String s in result)
                if (result.Count > 1)
                {
                    for (int i = 1; i < result.Count; i++)
                    {
                        trailersourcepath = trailersourcepath + ";" + result[i].ToString();
                        Log.Debug("MyFilmsDetails (SearchTrailerLocal) - Added Trailer to Trailersouce: '" + result[i].ToString() + "'");
                    }
                }
                Log.Debug("MyFilmsDetails (SearchTrailerLocal) - Total Files found: " + result.Count);
                Log.Debug("MyFilmsDetails (SearchTrailerLocal) - TrailerSourcePath: '" + trailersourcepath + "'");
            }
            else
                Log.Debug("MyFilmsDetails (SearchTrailerLocal) - NO TRAILERS FOUND !!!!");

            if ((trailersourcepath.Length > 0) && (!(MesFilms.conf.StrStorageTrailer.Length == 0) && !(MesFilms.conf.StrStorageTrailer == "(none)")))
            {
                Log.Debug("MyFilmsDetails (SearchTrailerLocal) - Old Trailersourcepath: '" + MesFilms.r[Index][MesFilms.conf.StrStorageTrailer] + "'");
                MesFilms.r[Index][MesFilms.conf.StrStorageTrailer] = trailersourcepath;
                Log.Debug("MyFilmsDetails (SearchTrailerLocal) - New Trailersourcepath    : '" + MesFilms.r[Index][MesFilms.conf.StrStorageTrailer] + "'");
                //Update_XML_database();
                Log.Debug("MyFilmsDetails (SearchTrailerLocal) - Database Updatewd !!!!");
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
          }
       }
 
#endregion
    }
}