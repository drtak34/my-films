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
  using System.Collections;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Web.UI.WebControls;

  using Grabber;

  using MediaPortal.Configuration;
  using MediaPortal.Dialogs;
  using MediaPortal.ExtensionMethods;
  using MediaPortal.GUI.Library;
  using MediaPortal.Playlists;
  using MediaPortal.Services;
  using MediaPortal.Util;
  using MediaPortal.Video.Database;

  using MyFilmsPlugin.MyFilms.Configuration;
  using MyFilmsPlugin.MyFilms.Utils;
  using MyFilmsPlugin.MyFilms.Utils.Cornerstone.MP;

  using NLog;
  using NLog.Config;
  using NLog.Targets;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;
  using ImageFast = MyFilmsPlugin.MyFilms.Utils.ImageFast;

  using Action = MediaPortal.GUI.Library.Action;
  using AntMovieCatalog = MyFilmsPlugin.MyFilms.AntMovieCatalog;
  using Layout = MediaPortal.GUI.Library.GUIFacadeControl.Layout;


  /// <summary>
    /// Summary description for GUIMyFilms.
    /// </summary>
    //[PluginIcons("MesFilms.MyFilms.Resources.clapperboard-128x128.png", "MesFilms.MyFilms.Resources.clapperboard-128x128-faded.png")]
    //[PluginIcons("MesFilms.MyFilms.Resources.logo_mesfilms.png", "MesFilms.MyFilms.Resources.logo_mesfilms-faded.png")]
  [PluginIcons("MyFilmsPlugin.MyFilms.Resources.film-reel-128x128.png", "MyFilmsPlugin.MyFilms.Resources.film-reel-128x128-faded.png")]
  public class MyFilms : GUIWindow, ISetupForm
  {
    #region Constructor
    public MyFilms()
    {
      // create Backdrop image swapper
      backdrop = new ImageSwapper();
      backdrop.ImageResource.Delay = 250;
      backdrop.PropertyOne = "#myfilms.fanart";
      backdrop.PropertyTwo = "#myfilms.fanart2";

      // create Film Cover image swapper
      filmcover = new AsyncImageResource();
      filmcover.Property = "#myfilms.coverimage";
      filmcover.Delay = 125;

      // create Group Cover image swapper
      groupcover = new AsyncImageResource();
      groupcover.Property = "#myfilms.groupcoverimage";
      groupcover.Delay = 125;

      // create Group Cover image swapper
      personcover = new AsyncImageResource();
      personcover.Property = "#myfilms.personcoverimage";
      personcover.Delay = 125;
    }
    #endregion

    #region ISetupForm Members

    // Returns the name of the plugin which is shown in the plugin menu
    public string PluginName()
    {
      return "MyFilms";
    }

    // Returns the description of the plugin is shown in the plugin menu
    public string Description()
    {
      return "MyFilms Ant Movie Catalog - Guzzi Mod";
    }

    // Returns the author of the plugin which is shown in the plugin menu
    public string Author()
    {
      return "Zebons (Mod by Guzzi)";
    }

    // show the setup dialog
    public void ShowPlugin()
    {
      System.Windows.Forms.Form setup = new MyFilmsSetup();
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

    /// <summary>
    /// If the plugin should have its own button on the main menu of Media Portal then it
    /// should return true to this method, otherwise if it should not be on home
    /// it should return false
    /// </summary>
    /// <param name="strButtonText">text the button should have</param>
    /// <param name="strButtonImage">image for the button, or empty for default</param>
    /// <param name="strButtonImageFocus">image for the button, or empty for default</param>
    /// <param name="strPictureImage">subpicture for the button or empty for none</param>
    /// <returns>true  : plugin needs its own button on home
    ///          false : plugin does not need its own button on home</returns>
    public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
    {
      string wPluginName = strPluginName;
      using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        wPluginName = xmlreader.GetValueAsString("MyFilms", "PluginName", "MyFilms");
      }

      strButtonText = wPluginName;
      strButtonImage = String.Empty;
      strButtonImageFocus = String.Empty;
      strPictureImage = String.Format("hover_{0}.png", "MyFilms");
      string strBtnFile = String.Format(@"{0}\media\{1}", GUIGraphicsContext.Skin, strPictureImage);
      if (!System.IO.File.Exists(strBtnFile))
        strPictureImage = string.Empty;
      return true;
    }
    #endregion


    /*
       * Log declarations
       */
    private static Logger LogMyFilms = LogManager.GetCurrentClassLogger();  //log
    private const string LogFileName = "MyFilms.log";  //log's filename
    private const string OldLogFileName = "MyFilms.old.log";  //log's old filename

    //private BaseMesFilms films;
    #region Descriptif zones Ecran

    public const int ID_MyFilms = 7986;
    public const int ID_MyFilmsDetail = 7987;
    public const int ID_MyFilmsDialogRating = 7988;
    public const int ID_MyFilmsActors = 7989;
    public const int ID_MyFilmsThumbs = 7990;
    public const int ID_MyFilmsActorsInfo = 7991;
    public const int ID_MyFilmsArtworkSelection = 7992;

    public const int ID_BrowseTheWeb = 54537689;
    public const int ID_OnlineVideos = 4755;
    public const int ID_SubCentral = 84623;

    public const int cacheThumbWith = 400;
    public const int cacheThumbHeight = 600;

    public const string ImdbBaseUrl = "http://www.imdb.com/";

    enum Controls : int
    {
      CTRL_BtnLayout = 2, //6
      CTRL_BtnSrtBy = 3, //2
      CTRL_BtnSearchT = 4,
      CTRL_BtnViewAs = 5, //3
      CTRL_BtnOptions = 6, //5
      CTRL_GlobalOverlayFilter = 7,
      CTRL_ToggleGlobalUnwatchedStatus = 8,
      //CTRL_TxtSelect = 12,
      CTRL_Fanart = 11,
      CTRL_Fanart2 = 21,
      CTRL_LoadingImage = 22,
      CTRL_DummyFacadeFilm = 36,
      CTRL_DummyFacadeGroup = 37,
      CTRL_DummyFacadePerson = 38,
      CTRL_ImageFilm = 1020,
      CTRL_ImageGroup = 1021,
      CTRL_ImagePerson = 1022,
      CTRL_List = 50, // Changed from 1026 to 50 due to meeting MePo Standards
      CTRL_logos_id2001 = 2001,
      CTRL_logos_id2002 = 2002,
      CTRL_logos_id2003 = 2003,
      CTRL_logos_id2012 = 2012,
      CTRL_GuiWaitCursor = 2080,
    }

    #region Skin Variables
    //[SkinControlAttribute((int)Controls.CTRL_TxtSelect)]
    //protected GUIFadeLabel TxtSelect;

    [SkinControlAttribute((int)Controls.CTRL_BtnSrtBy)]
    protected GUISortButtonControl BtnSrtBy;

    [SkinControlAttribute((int)Controls.CTRL_GlobalOverlayFilter)]
    protected GUIButtonControl BtnGlobalOverlayFilter;

    //[SkinControlAttribute((int)Controls.CTRL_ToggleGlobalUnwatchedStatus)]
    //protected GUIButtonControl BtnToggleGlobalWatched;

    [SkinControlAttribute((int)Controls.CTRL_List)]
    protected GUIFacadeControl facadeView;

    [SkinControlAttribute((int)Controls.CTRL_ImageFilm)]
    protected GUIImage ImgLstFilm;

    [SkinControlAttribute((int)Controls.CTRL_ImageGroup)]
    protected GUIImage ImgLstGroup;

    [SkinControlAttribute((int)Controls.CTRL_ImagePerson)]
    protected GUIImage ImgLstPerson;

    [SkinControlAttribute((int)Controls.CTRL_logos_id2001)]
    protected GUIImage ImgID2001;

    [SkinControlAttribute((int)Controls.CTRL_logos_id2002)]
    protected GUIImage ImgID2002;

    [SkinControlAttribute((int)Controls.CTRL_logos_id2003)]
    protected GUIImage ImgID2003;

    [SkinControlAttribute((int)Controls.CTRL_logos_id2012)]
    protected GUIImage ImgID2012;

    [SkinControlAttribute((int)Controls.CTRL_Fanart)]
    protected GUIImage ImgFanart;

    [SkinControlAttribute((int)Controls.CTRL_Fanart2)]
    protected GUIImage ImgFanart2;

    [SkinControlAttribute((int)Controls.CTRL_LoadingImage)]
    protected GUIImage loadingImage;

    [SkinControlAttribute((int)Controls.CTRL_GuiWaitCursor)]
    protected GUIAnimation m_SearchAnimation;

    [SkinControlAttribute((int)Controls.CTRL_DummyFacadeFilm)]
    protected GUILabelControl dummyFacadeFilm = null;
    
    [SkinControlAttribute((int)Controls.CTRL_DummyFacadeGroup)]
    protected GUILabelControl dummyFacadeGroup = null;
    
    [SkinControlAttribute((int)Controls.CTRL_DummyFacadePerson)]
    protected GUILabelControl dummyFacadePerson = null;

    #endregion

    #region Private/Public Properties

    // From OV/TVS
    private bool StopDownload { get; set; }

    private bool doUpdateMainViewByFinishEvent = false;
    
    //private Layout CurrentLayout { get; set; }

    // public static ReaderWriterLockSlim _rw = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    public int Layout = 0;
    public static int Prev_ItemID = -1;

    public static GUIListItem itemToPublish = null;

    public static string Prev_Label = string.Empty;
    //Added to jump back to correct Menu (Either Basichome or MyHome - or others...)
    public bool Context_Menu = false;
    public static Configuration conf;
    public static Logos confLogos;
    //private string currentConfig;
    private string strPluginName = "MyFilms";
    public static DataRow[] r; // will hold current recordset to traverse
    public static MFMovie currentMovie = new MFMovie(); // will hold current recordset to traverse

    //Imageswapperdefinitions for fanart and cover
    private ImageSwapper backdrop;
    private AsyncImageResource filmcover = null;
    private AsyncImageResource groupcover = null;
    private AsyncImageResource personcover = null;

    // Guzzi: Added from TV-Series for Fanarttoggling

    private System.Threading.Timer _fanartTimer = null;

    // Guzzi: Added to proper handle listlevels
    private static Listlevel currentListLevel = Listlevel.Movie;

    //Guzzi Addons for Global nonpermanent Trailer and MinRating Filters
    public bool GlobalFilterTrailersOnly = false;
    public bool GlobalFilterMinRating = false;
    public bool GlobalFilterIsOnlineOnly = false;
    //public string GlobalFilterString = string.Empty;
    public string GlobalFilterStringTrailersOnly = string.Empty;
    public string GlobalFilterStringUnwatched = string.Empty;
    public string GlobalFilterStringMinRating = string.Empty;
    public string GlobalFilterStringIsOnline = string.Empty;
    //public string GlobalUnwatchedFilterString = string.Empty;
    public bool MovieScrobbling = false;

    public static bool trailerscrobbleactive = false;
    public int actorID = 0;

    public enum optimizeOption { optimizeDisabled };

    public static bool InitialStart = false; //Added to implement InitialViewSetup
    public static bool InitialIsOnlineScan = false; //Added to implement switch if facade should display media availability

    // current Random Fanart
    public static List<string> currentFanartList = new List<string>();
    
    // current Trailer List for Scrobbling
    private static List<MFMovie> currentTrailerMoviesList = new List<MFMovie>();
    private static MFMovie currentTrailerPlayingItem = null;

    //PlayList currentPlaylist = null;
    //PlayListItem currentPlayingItem = null;

    private bool NetworkAvailabilityChanged_Subscribed = false;
    private bool PowerModeChanged_Subscribed = false;
    private bool PlayerEvents_Subscribed = false;

    private double lastPublished = 0;
    private Timer publishTimer;

    private const int RandomFanartDelay = 15000;

    // string list for search history
    public static List<string> SearchHistory = new List<string>();
    LoadParameterInfo loadParamInfo;
    #endregion

    #region Enums
    //public enum Layout
    //{
    //  List = 0,
    //  SmallIcons = 1,
    //  LargeIcons = 2,
    //  Filmstrip = 3,
    //  Coverflow = 4
    //}

    public enum ExternalPluginWindows : int
    {
      BrowseTheWeb = 54537689,
      OnlineVideos = 4755,
      SubCentral = 84623
    }

    enum eContextItems
    {
      toggleWatched,
      cycleMoviePoster,
      downloadSubtitle,
      actionMarkAllWatched,
      actionMarkAllUnwatched,
      actionHide,
      actionDelete,
      actionUpdate,
      actionLocalScan,
      actionFullRefresh,
      actionPlayRandom,
      actionLockViews,
      optionsOnlyShowLocal,
      optionsShowHiddenItems,
      optionsFastViewSwitch,
      optionsFanartRandom,
      optionsSeriesFanart,
      optionsUseOnlineFavourites,
      actionRecheckMI,
      showFanartChooser,
      addToPlaylist,
      showActorsGUI
    }

    enum eContextMenus
    {
      download = 100,
      action,
      options,
      rate,
      switchView,
      switchLayout,
      addToView,
      removeFromView
    }

    public enum DeleteMenuItems
    {
      disk,
      database,
      diskdatabase,
      subtitles,
      cancel
    }

    enum Listlevel
    {
      Movie,
      Group,
      Person,
      None
    }

    enum guiProperty
    {
      statusmessage,
      Title,
      Subtitle,
      Description,
      CurrentView,
      MovieCount,
      GroupCount,
      WatchedCount,
      UnWatchedCount,
      LastOnlineUpdate
    }

    #endregion

    #endregion

    #region handler and backgroundworker

    public delegate void FilmsStoppedHandler(int stoptime, string filename);
    public delegate void FilmsEndedHandler(string filename);

    public delegate void EventHandler(object sender, EventArgs e);

    System.ComponentModel.BackgroundWorker bgUpdateDB = new System.ComponentModel.BackgroundWorker();
    System.ComponentModel.BackgroundWorker bgUpdateFanart = new System.ComponentModel.BackgroundWorker();
    System.ComponentModel.BackgroundWorker bgUpdateActors = new System.ComponentModel.BackgroundWorker();
    System.ComponentModel.BackgroundWorker bgUpdateTrailer = new System.ComponentModel.BackgroundWorker();
    System.ComponentModel.BackgroundWorker bgLoadMovieList = new System.ComponentModel.BackgroundWorker();
    System.ComponentModel.BackgroundWorker bgIsOnlineCheck = new System.ComponentModel.BackgroundWorker();
    // System.ComponentModel.BackgroundWorker bgOnPageLoad = null;

    public static FileSystemWatcher FSwatcher = new FileSystemWatcher();

    public delegate void DelegateUpdateProgress(string message);
    public static void UpdateMyProgressbar(string message)
    {
      MyFilmsDetail.setGUIProperty("statusmessage", message);
    }

    //public static event WatchedEventDelegate WatchedItem;
    //public delegate void WatchedEventDelegate(MFMovie movie, bool watched, int count);

    //public static event RatingEventDelegate RateItem;
    //public delegate void RatingEventDelegate(MFMovie movie, string rating);

    //public event TrailerScrobbledEventDelegate TrailerScrobbled;
    //public delegate void TrailerScrobbledEventDelegate(MFMovie movie, bool watched, int count);
    //TrailerScrobbled += (OnPlayBackEnded);
    //private void OnPlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename)
    //{
    //  LogMyFilms.Debug("OnPlayBackEnded was initiated, but has no relevant event data for MyFilms - filename: '" + filename + "'");
    //  if (MyFilms.trailerscrobbleactive == true)
    //  {
    //    MyFilms.trailerscrobbleactive = false;
    //    MyFilms.PlayRandomTrailer(true);
    //  }
    //  return;
    //}


    #endregion

    #region Base Overrides

    public override int GetID
    {
      get { return ID_MyFilms; }
      set { base.GetID = value; }
    }

    public override string GetModuleName()
    {
      return GUILocalizeStrings.Get(ID_MyFilms); // return localized string for Module ID
    }

    public override bool Init() //This Method is only loaded ONCE when starting Mediaportal !
    {
      bool result = Load(GUIGraphicsContext.Skin + @"\MyFilms.xml"); 
      InitLogger(); // Initialize Logger 
      Log.Info("MyFilms.Init() started. See MyFilms.log for further Details.");
      LogMyFilms.Debug("MyFilms.Init() started.");
      LogMyFilms.Info("MyFilms     Version: 'V" + MyFilmsSettings.Version.ToString() +  "', BuildDate: '" + MyFilmsSettings.BuildDate.ToString() + "'");
      LogMyFilms.Info("MediaPortal Version: 'V" + MyFilmsSettings.MPVersion.ToString() + "', BuildDate: '" + MyFilmsSettings.MPBuildDate.ToString() + "'");

      // Fanart Timer
      _fanartTimer = new System.Threading.Timer(new TimerCallback(FanartTimerEvent), null, Timeout.Infinite, Timeout.Infinite);

      // Set Variable for FirstTimeView Setup
      InitialStart = true;

      //Add localized labels for DB Columns
      InitGUIPropertyLabels();

      // check if running version of mediaportal support loading with parameter           
      //if (typeof(GUIWindow).GetField("_loadParameter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null)
      //{
      //  LoadWithParameterSupported = true;
      //}

      // Init Eventhandler for Background Updates
      MyFilmsDetail.DetailsUpdated += new MyFilmsDetail.DetailsUpdatedEventDelegate(OnDetailsUpdated);
      
      // Register Messagehandler for CD-Inserted-Messages
      //GUIWindowManager.Receivers += new SendMessageHandler(GUIWindowManager_OnNewMessage);

      // Register Eventhandler for AMCupdater Background progress reporting
      //AMCupdaterStartEventHandler();
      
      // Initialize Backgroundworker
      InitializeBackgroundWorker();

      LogMyFilms.Debug("MyFilms.Init() completed. Loading main skin file.");
      return result;
    }

    public override void DeInit()
    {
      LogMyFilms.Debug("MyFilms.DeInit() started/ended. Calling base.DeInit()...");
      base.DeInit();
      // Add Other Classes here, if necessary
    }

    //protected override string SerializeName
    //{
    //  get { return "myfilms"; }
    //}

    //protected override void LoadSettings()
    //{
    //  LogMyFilms.Debug("MyFilms.LoadSettings() started.");
    //  base.LoadSettings();
    //  // add plugin specific routines here
    //}

    //protected override void SaveSettings()
    //{
    //  LogMyFilms.Debug("MyFilms.SaveSettings() started.");
    //  base.SaveSettings();
    //  // add plugin specific routines here
    //}

    protected override void OnPageLoad() //This is loaded each time, the plugin is entered - can be used to reset certain settings etc.
    {
      LogMyFilms.Debug("MyFilms.OnPageLoad() started.");
      Log.Debug("MyFilms.OnPageLoad() started. See MyFilms.log for further Details.");

      if (InitialStart)
      {
        // Initial steps
      }

      // Register PowerEventMode and Networkavailability Changed Handler
      InitBSHandler();

      // attach to global action event, to handle remote keys during playback - e.g. trailer previews
      GUIWindowManager.OnNewAction += new OnActionHandler(this.GUIWindowManager_OnNewAction);

      // Support for StartParameters - ToDo: Add start view options (implementation)
      if (PreviousWindowId != ID_MyFilms && PreviousWindowId != ID_MyFilmsDetail)
      {
        // reset the LoadParameterInfo
        loadParamInfo = null;

        string loadParam = null;
        // check if running version of mediaportal supports loading with parameter and handle _loadParameter
        System.Reflection.FieldInfo fi = typeof(GUIWindow).GetField("_loadParameter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fi != null)
        {
          loadParam = (string)fi.GetValue(this);
        }

        // check for LoadParameters by GUIproperties if nothing was set by the _loadParameter
        if (string.IsNullOrEmpty(loadParam)) loadParam = LoadParameterInfo.FromGuiProperties();

        if (!string.IsNullOrEmpty(loadParam))
        {
          loadParamInfo = new LoadParameterInfo(loadParam);

          // set all state variables to reflect the state we were called with
          if (!string.IsNullOrEmpty(loadParamInfo.Config))
          {
            // set config accordingly
          }
          if (!string.IsNullOrEmpty(loadParamInfo.Search))
          {
            //lastSearchQuery = loadParamInfo.Search;
            //Display_SearchResults(loadParamInfo.Search);
            return;
          }
        }
      }

      //MyFilmsDetail.clearGUIProperty("picture");
      BtnGlobalOverlayFilter.Label = GUILocalizeStrings.Get(10798714); // Global Filters ...

      // (re)link backdrop image controls to the backdrop image swapper
      backdrop.GUIImageOne = ImgFanart;
      backdrop.GUIImageTwo = ImgFanart2;
      backdrop.LoadingImage = loadingImage;  // --> Not used - could be used to show other image while loading destination thumb
      if (!filmcover.Active) filmcover.Active = true;
      if (!groupcover.Active) groupcover.Active = true;
      if (!personcover.Active) personcover.Active = true;

      OnPageLoad_DoPageLoad(); // run former WindowInit synchronous
      LogMyFilms.Debug("MyFilms.OnPageLoad() completed.");
      base.OnPageLoad(); // let animations run
      //new System.Threading.Thread(delegate()
      //{
      //  this.OnPageLoad_DoPageLoad();
      //  GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
      //  {
      //    // enter here what to load after background thread has finished !
      //    return 0;
      //  }, 0, 0, null);
      //}) { Name = "MyFilmsOnPageLoadWorker", IsBackground = true }.Start();
    }

    private void OnPageLoad_DoPageLoad()
    {
      bool defaultconfig = false;
      GUIWaitCursor.Init(); GUIWaitCursor.Show();
      if ((PreviousWindowId != ID_MyFilmsDetail) && !MovieScrobbling && (PreviousWindowId != ID_MyFilmsActors) && (PreviousWindowId != ID_OnlineVideos) && (PreviousWindowId != ID_BrowseTheWeb))
      {
        // Prev_MenuID = PreviousWindowId;
        if (InitialStart) InitMainScreen(false); // don't log to MyFilms.log Property clear
        //InitGlobalFilters(false);

        if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Config) && PreviousWindowId != ID_MyFilmsDetail) // config given in load params
        {
          LogMyFilms.Debug("OnPageLoad() - LoadParams - try override loading config: '" + loadParamInfo.Config + "'");
          string newConfig = Configuration.Control_Access_Config(loadParamInfo.Config, GetID);
          if (newConfig != string.Empty) // if user escapes dialog or bad value leave system unchanged
          {
            InitMainScreen(false); // reset all properties and values
            InitGlobalFilters(false); // reset global filters, when loading new config !
            //Change "Config":
            if (!string.IsNullOrEmpty(Configuration.CurrentConfig)) // if there is an active config, save it !
            {
              if (facadeView.SelectedListItem != null)
                Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
              else
                Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, string.Empty);
            }
            Configuration.CurrentConfig = newConfig;
            InitialIsOnlineScan = false; // set false, so facade does not display false media status !!!
            InitialStart = true; //Set to true to make sure initial View is initialized for new DB view
            Load_Config(newConfig, true, loadParamInfo);
            Fin_Charge_Init(true, true);
          }
        }
        else
        {
          defaultconfig = Configuration.Current_Config();
          Load_Config(Configuration.CurrentConfig, true, loadParamInfo);
        }

        InitFSwatcher(); // load DB watcher for multiseat

        if (MyFilms.conf.StrFanart) Fanartstatus(true);
        else Fanartstatus(false);
      }
      if ((Configuration.CurrentConfig == null) || (Configuration.CurrentConfig.Length == 0))
      {
        GUIWaitCursor.Hide();
        GUIWindowManager.ShowPreviousWindow();
        return;
      }

      bool launchMediaScanner = InitialStart;

      if (((conf.AlwaysDefaultView) || (InitialStart)) && (PreviousWindowId != ID_MyFilmsDetail) && !MovieScrobbling && (PreviousWindowId != ID_MyFilmsActors) && (PreviousWindowId != ID_OnlineVideos) && (PreviousWindowId != ID_BrowseTheWeb))
      {
        LogMyFilms.Debug("OnPageLoad() - load facade with DefaultView -> Fin_Charge_Init(true, false)");
        Fin_Charge_Init(true, defaultconfig); // defaultconfig is true, if a default config is set in MF setup (not default view!)
      }
      else
      {
        LogMyFilms.Debug("OnPageLoad() - load facade with last settings -> Fin_Charge_Init(false, false)");
        Fin_Charge_Init(false, defaultconfig);
      }

      // Launch Background availability scanner, if configured in setup
      if (MyFilms.conf.ScanMediaOnStart && launchMediaScanner)
      {
        LogMyFilms.Debug("Launching Availabilityscanner - Initialstart = '" + launchMediaScanner + "'");
        this.AsynIsOnlineCheck();
      }
      //GUIControl.ShowControl(GetID, 34);
      GUIWaitCursor.Hide();

      if (PreviousWindowId != ID_MyFilmsDetail && loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.MovieID) && loadParamInfo.Config == Configuration.CurrentConfig) // movieID given in load params -> jump to details screen !
      {
        LogMyFilms.Debug("OnPageLoad() - LoadParams - try override loading movieid: '" + loadParamInfo.MovieID + "', play: '" + loadParamInfo.Play + "'");
        // facade index is set in filmlist loading - only launching details necessary !

        if (!string.IsNullOrEmpty(loadParamInfo.MovieID)) // if load params for movieid exist, set current index to the movie detected
        {
          int index = -1;
          foreach (DataRow sr in r)
          {
            index += 1;
            if (sr["number"].ToString() == loadParamInfo.MovieID)
            {
              // bool success = int.TryParse(loadParamInfo.MovieID, out index);
              conf.StrIndex = index;
              conf.StrTIndex = sr[conf.StrTitle1].ToString();
            }
          }
          if (index == -1)
            GUIWindowManager.ShowPreviousWindow();
          if (loadParamInfo.Play == "true")
            MyFilmsDetail.Launch_Movie(conf.StrIndex, GetID, null);
          else
            GUIWindowManager.ActivateWindow((int)ID_MyFilmsDetail, true);
        }
      }
      else if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Search)) // search expression given in load params -> do global search !
      {
        LogMyFilms.Debug("OnPageLoad() - LoadParams - try override loading search: '" + loadParamInfo.Search + "' -> NOT YET IMPLEMENTED !");
      }

      if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
      {
        // Originally Deactivated by Zebons    
        // ********************************
        // ToDo: Crash on Details to be fixed (make it threadsafe !!!!!!!)
        //if (!bgLoadMovieList.IsBusy)
        //{
        //  LogMyFilms.Debug("Launching AsynLoadMovieList");
        //  AsynLoadMovieList();
        //}
        // ********************************
        // Originally Deactivated by Zebons    
      }
    }

    protected override void OnPageDestroy(int new_windowId)
    {
      LogMyFilms.Debug("MyFilms.OnPageDestroy(" + new_windowId.ToString() + ") started.");

      // stop any background tasks
      StopDownload = true;
      GUIConnector.Instance.StopBackgroundTask();

      loadParamInfo.SafeDispose();
      //// Reset to force republishing details on reentering
      //Prev_ItemID = -1;
      //Prev_Label = string.Empty;
      //if (!bgOnPageLoad.CancellationPending) // cancel pageload worker thread - otherwise null ref exception when trying to populate facade ...
      //  bgOnPageLoad.CancelAsync();
      //Thread.Sleep(5); // sleep 5 milliseconds
      // Set Facadevisibilities false ...
      //SetDummyControlsForFacade(Listlevel.None);

      //Disable Random Fanart Timer
      _fanartTimer.Change(Timeout.Infinite, Timeout.Infinite);

      //MyFilmsDetail.clearGUIProperty("Fanart");
      //MyFilmsDetail.clearGUIProperty("Fanart2");

      //LogMyFilms.Debug("GUIMessage: GUI_MSG_WINDOW_DEINIT - Start");

      if (Configuration.CurrentConfig != "")
      {
        if (facadeView == null || facadeView.SelectedListItemIndex == -1)
          Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, "");
        else
          Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
      }

      //ImgFanart.SetFileName(string.Empty);
      //ImgFanart2.SetFileName(string.Empty);

      //facadeView.Clear();
      //backdrop.PropertyOne = " ";
      // added from MoPic
      //backdrop.Filename = string.Empty;
      //MyFilmsDetail.clearGUIProperty("currentfanart");
      //filmcover.Filename = string.Empty;

      //LogMyFilms.Debug("GUIMessage: GUI_MSG_WINDOW_DEINIT - End");

      GUITextureManager.CleanupThumbs();
      LogMyFilms.Debug("MyFilms.OnPageDestroy(" + new_windowId.ToString() + ") completed.");
      Log.Debug("MyFilms.OnPageDestroy() completed. See MyFilms.log for further Details.");
      base.OnPageDestroy(new_windowId);
    }

    protected override void OnShowContextMenu()
    {
      LogMyFilms.Debug("OnShowContextMenu() started");
      if (facadeView.SelectedListItemIndex > -1)
      {
        if (!(facadeView.Focus)) 
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
        Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
        return;
      }
      base.OnShowContextMenu();
    }

    #endregion

    #region Main Context Menu (inactive)
    //protected override void OnShowContextMenu()
    //{
    //  try
    //  {
    //    GUIListItem currentitem = this.facadeView.SelectedListItem;
    //    if (currentitem == null) return;

    //    IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
    //    if (dlg == null) return;

    //    bool emptyList = currentitem.Label == "no items";
    //    if (!emptyList)
    //    {
    //      switch (this.currentListLevel)
    //      {
    //        case Listlevel.Series:
    //          {
    //            selectedSeries = (DBSeries)currentitem.TVTag;
    //          }
    //          break;
    //      }
    //    }
    //    bool bExitMenu = false;
    //    do
    //    {
    //      dlg.Reset();
    //      GUIListItem pItem = null;

    //      if (!emptyList)
    //      {
    //        switch (this.currentListLevel)
    //        {
    //          case Listlevel.Movie:
    //            dlg.SetHeading("Movie" + "MovieName");
    //            break;

    //          case Listlevel.Group:
    //            dlg.SetHeading("Group" + "Groupname");
    //            break;

    //          case Listlevel.Person:
    //            dlg.SetHeading("Translation.Person" + ": " + "PersonName");
    //            break;
    //          default:
    //            // group
    //            dlg.SetHeading("Menu");
    //            break;
    //        }

    //        #region Top Level Menu Items - Context Sensitive
    //        if (this.currentListLevel == Listlevel.Movie)
    //        {
    //          pItem = new GUIListItem("Translation.Toggle_watched_flag");
    //          dlg.Add(pItem);
    //          pItem.ItemId = (int)eContextItems.toggleWatched;

    //          pItem = new GUIListItem("Translation.RateEpisode" + " ...");
    //          dlg.Add(pItem);
    //          pItem.ItemId = (int)eContextMenus.rate;
    //        }
    //        else if (this.currentListLevel != Listlevel.Group)
    //        {
    //          pItem = new GUIListItem("Translation.Mark_all_as_watched");
    //          dlg.Add(pItem);
    //          pItem.ItemId = (int)eContextItems.actionMarkAllWatched;

    //          pItem = new GUIListItem("Translation.Mark_all_as_unwatched");
    //          dlg.Add(pItem);
    //          pItem.ItemId = (int)eContextItems.actionMarkAllUnwatched;
    //        }

    //        if (this.currentListLevel != Listlevel.Group)
    //        {
    //          if (MyFilms.conf.StrFanart) // only if skins supports it
    //          {
    //            pItem = new GUIListItem("Translation.FanArt" + " ...");
    //            dlg.Add(pItem);
    //            pItem.ItemId = (int)eContextItems.showFanartChooser;
    //          }

    //          if (File.Exists(GUIGraphicsContext.Skin + @"\TVSeries.Actors.xml"))
    //          {
    //            pItem = new GUIListItem("Translation.Actors" + " ...");
    //            dlg.Add(pItem);
    //            pItem.ItemId = (int)eContextItems.showActorsGUI;
    //          }
    //        }

    //        if (this.currentListLevel == Listlevel.Movie)
    //        {
    //          if (true) //(selectedSeries.PosterList.Count > 1)
    //          {
    //            pItem = new GUIListItem("Translation.CycleSeriesPoster");
    //            dlg.Add(pItem);
    //            pItem.ItemId = (int)eContextItems.cycleMoviePoster;
    //          }
    //        }

    //        #endregion
    //      }
    //      else
    //        dlg.SetHeading("m_CurrLView.Name");

    //      #region Top Level Menu Items - Non-Context Sensitive
    //      pItem = new GUIListItem(Translation.ChangeView + " ...");
    //      dlg.Add(pItem);
    //      pItem.ItemId = (int)eContextMenus.switchView;

    //      if (SkinSettings.GetLayoutCount(this.currentListLevel.ToString()) > 1)
    //      {
    //        pItem = new GUIListItem(Translation.ChangeLayout + " ...");
    //        dlg.Add(pItem);
    //        pItem.ItemId = (int)eContextMenus.switchLayout;
    //      }

    //      if (currentListLevel != Listlevel.Group)
    //      {
    //        pItem = new GUIListItem(Translation.Actions + " ...");
    //        dlg.Add(pItem);
    //        pItem.ItemId = (int)eContextMenus.action;
    //      }

    //      pItem = new GUIListItem(Translation.Options + " ...");
    //      dlg.Add(pItem);
    //      pItem.ItemId = (int)eContextMenus.options;
    //      #endregion

    //      #region Download menu - keep at the bottom for fast access (menu + up => there)
    //      if (!emptyList && subtitleDownloadEnabled && this.currentListLevel == Listlevel.Episode)
    //      {
    //        pItem = new GUIListItem(Translation.Download + " ...");
    //        dlg.Add(pItem);
    //        pItem.ItemId = (int)eContextMenus.download;
    //      }
    //      #endregion

    //      dlg.DoModal(GUIWindowManager.ActiveWindow);
    //      #region Selected Menu Item Actions (Sub-Menus)
    //      switch (dlg.SelectedId)
    //      {
    //        case (int)eContextMenus.download:
    //          {
    //            dlg.Reset();
    //            dlg.SetHeading(Translation.Download);

    //            if (subtitleDownloadEnabled)
    //            {
    //              pItem = new GUIListItem(Translation.Retrieve_Subtitle);
    //              dlg.Add(pItem);
    //              pItem.ItemId = (int)eContextItems.downloadSubtitle;
    //            }

    //            dlg.DoModal(GUIWindowManager.ActiveWindow);
    //            if (dlg.SelectedId != -1)
    //              bExitMenu = true;
    //          }
    //          break;

    //        case (int)eContextMenus.action:
    //          {
    //            dlg.Reset();
    //            dlg.SetHeading(Translation.Actions);
    //            if (currentListLevel != Listlevel.Group)
    //            {
    //              if (DBOption.GetOptions(DBOption.cShowDeleteMenu))
    //              {
    //                pItem = new GUIListItem(Translation.Delete + " ...");
    //                dlg.Add(pItem);
    //                pItem.ItemId = (int)eContextItems.actionDelete;
    //              }

    //              if (!m_parserUpdaterWorking)
    //              {
    //                pItem = new GUIListItem(Translation.Update);
    //                dlg.Add(pItem);
    //                pItem.ItemId = (int)eContextItems.actionUpdate;
    //              }

    //              // add hidden menu
    //              // check if item is already hidden
    //              pItem = new GUIListItem();
    //              switch (currentListLevel)
    //              {
    //                case Listlevel.Series:
    //                  pItem.Label = selectedSeries[DBSeries.cHidden] ? Translation.UnHide : Translation.Hide;
    //                  break;
    //                case Listlevel.Season:
    //                  pItem.Label = selectedSeason[DBSeries.cHidden] ? Translation.UnHide : Translation.Hide;
    //                  break;
    //                case Listlevel.Episode:
    //                  pItem.Label = selectedEpisode[DBSeries.cHidden] ? Translation.UnHide : Translation.Hide;
    //                  break;
    //              }
    //              dlg.Add(pItem);
    //              pItem.ItemId = (int)eContextItems.actionHide;

    //              pItem = new GUIListItem(Translation.updateMI);
    //              dlg.Add(pItem);
    //              pItem.ItemId = (int)eContextItems.actionRecheckMI;
    //            }

    //            // Online to Local Episode Matching order
    //            if (this.currentListLevel != Listlevel.Group)
    //            {
    //              // get current online episode to local episode matching order
    //              string currMatchOrder = selectedSeries[DBOnlineSeries.cChosenEpisodeOrder].ToString();
    //              if (string.IsNullOrEmpty(currMatchOrder)) currMatchOrder = "Aired";

    //              pItem = new GUIListItem(Translation.ChangeOnlineMatchOrder);
    //              dlg.Add(pItem);
    //              pItem.ItemId = (int)eContextItems.actionChangeOnlineEpisodeMatchOrder;
    //            }

    //            // Episode Sort By
    //            if (this.currentListLevel == Listlevel.Episode || this.currentListLevel == Listlevel.Season)
    //            {
    //              // get current episode sort order (DVD or Aired)
    //              string currSortBy = selectedSeries[DBOnlineSeries.cEpisodeSortOrder].ToString();
    //              if (string.IsNullOrEmpty(currSortBy)) currSortBy = "Aired";

    //              pItem = new GUIListItem(string.Format("{0}: {1}", Translation.SortBy, Translation.Get(currSortBy + "Order")));
    //              dlg.Add(pItem);
    //              pItem.ItemId = (int)eContextItems.actionEpisodeSortBy;
    //            }

    //            pItem = new GUIListItem(Translation.Force_Local_Scan + (m_parserUpdaterWorking ? Translation.In_Progress_with_Barracks : ""));
    //            dlg.Add(pItem);
    //            pItem.ItemId = (int)eContextItems.actionLocalScan;

    //            pItem = new GUIListItem(Translation.Force_Online_Refresh + (m_parserUpdaterWorking ? Translation.In_Progress_with_Barracks : ""));
    //            dlg.Add(pItem);
    //            pItem.ItemId = (int)eContextItems.actionFullRefresh;

    //            pItem = new GUIListItem(Translation.Play_Random_Episode);
    //            dlg.Add(pItem);
    //            pItem.ItemId = (int)eContextItems.actionPlayRandom;

    //            if (!String.IsNullOrEmpty(DBOption.GetOptions(DBOption.cParentalControlPinCode)))
    //            {
    //              pItem = new GUIListItem(Translation.ParentalControlLocked);
    //              dlg.Add(pItem);
    //              pItem.ItemId = (int)eContextItems.actionLockViews;
    //            }

    //            dlg.DoModal(GUIWindowManager.ActiveWindow);
    //            if (dlg.SelectedId != -1)
    //              bExitMenu = true;
    //          }
    //          break;

    //        case (int)eContextMenus.options:
    //          {
    //            dlg.Reset();
    //            ShowOptionsMenu();
    //            return;
    //          }

    //        case (int)eContextMenus.switchView:
    //          {
    //            dlg.Reset();
    //            if (showViewSwitchDialog())
    //              return;
    //          }
    //          break;

    //        case (int)eContextMenus.switchLayout:
    //          {
    //            dlg.Reset();
    //            ShowLayoutMenu();
    //            return;
    //          }

    //        case (int)eContextMenus.addToView:
    //          dlg.Reset();
    //          ShowViewTagsMenu(true, selectedSeries);
    //          return;

    //        case (int)eContextMenus.removeFromView:
    //          dlg.Reset();
    //          ShowViewTagsMenu(false, selectedSeries);
    //          return;

    //        case (int)eContextMenus.rate:
    //          {
    //            switch (currentListLevel)
    //            {
    //              case Listlevel.Episode:
    //                showRatingsDialog(m_SelectedEpisode, false);
    //                break;
    //              case Listlevel.Series:
    //              case Listlevel.Season:
    //                showRatingsDialog(m_SelectedSeries, false);
    //                break;
    //            }
    //            LoadFacade();
    //            if (dlg.SelectedId != -1)
    //              bExitMenu = true;
    //            return;
    //          }

    //        default:
    //          bExitMenu = true;
    //          break;
    //      }
    //      #endregion
    //    }
    //    while (!bExitMenu);

    //    if (dlg.SelectedId == -1) return;

    //    #region Selected Menu Item Actions
    //    List<DBMovie> episodeList = new List<DBMovie>();
    //    SQLCondition conditions = null;

    //    switch (dlg.SelectedId)
    //    {
    //      #region Watched/Unwatched
    //      case (int)eContextItems.toggleWatched:
    //        // toggle watched
    //        if (selectedEpisode != null)
    //        {
    //          bool watched = selectedEpisode[DBOnlineEpisode.cWatched];
    //          if (selectedEpisode[DBMovie.cFilename].ToString().Length > 0)
    //          {
    //            conditions = new SQLCondition();
    //            conditions.Add(new DBMovie(), DBMovie.cFilename, selectedEpisode[DBMovie.cFilename], SQLConditionType.Equal);
    //            List<DBMovie> episodes = DBMovie.Get(conditions, false);
    //            foreach (DBMovie episode in episodes)
    //            {
    //              episode[DBOnlineEpisode.cWatched] = !watched;
    //              episode[DBOnlineEpisode.cTraktSeen] = watched ? 2 : 0;
    //              episode.Commit();
    //            }

    //            FollwitConnector.Watch(episodes, !watched);
    //          }
    //          else
    //          {
    //            selectedEpisode[DBOnlineEpisode.cWatched] = !watched;
    //            selectedEpisode[DBOnlineEpisode.cTraktSeen] = watched ? 2 : 0;
    //            selectedEpisode.Commit();

    //            FollwitConnector.Watch(selectedEpisode, !watched, false);
    //          }
    //          // Update Episode Counts
    //          DBSeason.UpdateEpisodeCounts(m_SelectedSeries, m_SelectedSeason);

    //          // Update Trakt
    //          m_TraktSyncTimer.Change(10000, Timeout.Infinite);

    //          LoadFacade();
    //        }
    //        break;

    //      case (int)eContextItems.actionMarkAllWatched:
    //        // Mark all watched that are visible on the facade and
    //        // do not air in the future...its misleading marking watched on episodes
    //        // you cant see. People could import a new episode and have it marked as watched accidently

    //        if (selectedSeries != null)
    //        {
    //          conditions = new SQLCondition();
    //          conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
    //          conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cFirstAired, DateTime.Now.ToString("yyyy-MM-dd"), SQLConditionType.LessEqualThan);
    //        }

    //        if (selectedSeason != null)
    //        {
    //          conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, selectedSeason[DBSeason.cIndex], SQLConditionType.Equal);
    //        }

    //        episodeList = DBMovie.Get(conditions, true);

    //        // reset traktSeen flag for later synchronization 
    //        // and set watched state
    //        foreach (DBMovie episode in episodeList)
    //        {
    //          episode[DBOnlineEpisode.cWatched] = 1;
    //          episode[DBOnlineEpisode.cTraktSeen] = 0;
    //          episode.Commit();
    //        }

    //        FollwitConnector.Watch(episodeList, true);

    //        // Updated Episode Counts
    //        if (this.currentListLevel == Listlevel.Series && selectedSeries != null)
    //        {
    //          DBSeries.UpdateEpisodeCounts(selectedSeries);
    //        }
    //        else if (this.currentListLevel == Listlevel.Season && selectedSeason != null)
    //        {
    //          DBSeason.UpdateEpisodeCounts(selectedSeries, selectedSeason);
    //        }

    //        cache.dump();

    //        // sync to trakt
    //        m_TraktSyncTimer.Change(10000, Timeout.Infinite);

    //        // refresh facade
    //        LoadFacade();
    //        break;

    //      case (int)eContextItems.actionMarkAllUnwatched:
    //        // Mark all unwatched that are visible on the facade

    //        if (selectedSeries != null)
    //        {
    //          conditions = new SQLCondition();
    //          conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
    //        }

    //        if (selectedSeason != null)
    //        {
    //          conditions.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeasonIndex, selectedSeason[DBSeason.cIndex], SQLConditionType.Equal);
    //        }

    //        episodeList = DBMovie.Get(conditions, true);

    //        // set traktSeen flag and watched state
    //        // when traktSeen = 2, the seen flag will be removed from trakt
    //        foreach (DBMovie episode in episodeList)
    //        {
    //          episode[DBOnlineEpisode.cWatched] = 0;
    //          episode[DBOnlineEpisode.cTraktSeen] = 2;
    //          episode.Commit();
    //        }

    //        FollwitConnector.Watch(episodeList, false);

    //        // Updated Episode Counts
    //        if (this.currentListLevel == Listlevel.Series && selectedSeries != null)
    //        {
    //          DBSeries.UpdateEpisodeCounts(selectedSeries);
    //        }
    //        else if (this.currentListLevel == Listlevel.Season && selectedSeason != null)
    //        {
    //          DBSeason.UpdateEpisodeCounts(selectedSeries, selectedSeason);
    //        }

    //        cache.dump();

    //        // sync to trakt
    //        m_TraktSyncTimer.Change(10000, Timeout.Infinite);

    //        // refresh facade
    //        LoadFacade();
    //        break;
    //      #endregion

    //      #region Playlist
    //      case (int)eContextItems.addToPlaylist:
    //        AddItemToPlayList();
    //        break;
    //      #endregion

    //      #region Cycle Artwork
    //      case (int)eContextItems.cycleSeriesBanner:
    //        CycleSeriesBanner(selectedSeries, true);
    //        break;

    //      case (int)eContextItems.cycleSeriesPoster:
    //        CycleSeriesPoster(selectedSeries, true);
    //        break;

    //      case (int)eContextItems.cycleSeasonPoster:
    //        CycleSeasonPoster(selectedSeason, true);
    //        break;
    //      #endregion

    //      #region Fanart Chooser
    //      case (int)eContextItems.showFanartChooser:
    //        ShowFanartChooser(m_SelectedSeries[DBOnlineSeries.cID]);
    //        break;
    //      #endregion

    //      #region Actors GUI
    //      case (int)eContextItems.showActorsGUI:
    //        GUIActors.SeriesId = m_SelectedSeries[DBOnlineSeries.cID];
    //        GUIWindowManager.ActivateWindow(9816);
    //        break;
    //      #endregion

    //      #region Force Online Series Query
    //      case (int)eContextItems.forceSeriesQuery:
    //        {
    //          // clear the series
    //          SQLCondition condition = new SQLCondition();
    //          condition.Add(new DBMovie(), DBMovie.cSeriesID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
    //          DBMovie.Clear(condition);
    //          condition = new SQLCondition();
    //          condition.Add(new DBOnlineEpisode(), DBOnlineEpisode.cSeriesID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
    //          DBOnlineEpisode.Clear(condition);

    //          condition = new SQLCondition();
    //          condition.Add(new DBSeason(), DBSeason.cSeriesID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
    //          DBSeason.Clear(condition);

    //          condition = new SQLCondition();
    //          condition.Add(new DBSeries(), DBSeries.cID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
    //          DBSeries.Clear(condition);

    //          condition = new SQLCondition();
    //          condition.Add(new DBOnlineSeries(), DBOnlineSeries.cID, selectedSeries[DBSeries.cID], SQLConditionType.Equal);
    //          DBOnlineSeries.Clear(condition);

    //          // look for it again
    //          m_parserUpdaterQueue.Add(new CParsingParameters(ParsingAction.NoExactMatch, null, true, false));
    //          // Start Import if delayed
    //          m_scanTimer.Change(1000, 1000);
    //        }
    //        break;
    //      #endregion

    //      #region Downloaders
    //      case (int)eContextItems.downloadSubtitle:
    //        {
    //          if (selectedEpisode != null)
    //          {
    //            DBMovie episode = (DBMovie)currentitem.TVTag;
    //            ShowSubtitleMenu(episode);
    //          }
    //        }
    //        break;
    //      #endregion

    //      #region Favourites
    //      /*case (int)eContextItems.actionToggleFavorite: {
    //      // Toggle Favourites
    //      m_SelectedSeries.toggleFavourite();

    //      // If we are in favourite view we need to reload to remove the series
    //      LoadFacade();
    //      break;
    //    }*/
    //      #endregion

    //      #region Actions
    //      #region Hide
    //      case (int)eContextItems.actionHide:
    //        switch (this.currentListLevel)
    //        {
    //          case Listlevel.Series:
    //            selectedSeries.HideSeries(!selectedSeries[DBSeries.cHidden]);
    //            break;

    //          case Listlevel.Season:
    //            selectedSeason.HideSeason(!selectedSeason[DBSeason.cHidden]);
    //            DBSeries.UpdateEpisodeCounts(m_SelectedSeries);
    //            break;

    //          case Listlevel.Episode:
    //            selectedEpisode.HideEpisode(!selectedEpisode[DBOnlineEpisode.cHidden]);
    //            DBSeason.UpdateEpisodeCounts(m_SelectedSeries, m_SelectedSeason);
    //            break;
    //        }
    //        LoadFacade();
    //        break;
    //      #endregion

    //      #region Delete
    //      case (int)eContextItems.actionDelete:
    //        {
    //          dlg.Reset();
    //          ShowDeleteMenu(selectedSeries, selectedSeason, selectedEpisode);
    //        }
    //        break;
    //      #endregion

    //      #region Update Series/Episode Information
    //      case (int)eContextItems.actionUpdate:
    //        {
    //          dlg.Reset();
    //          UpdateEpisodes(selectedSeries, m_SelectedSeason, m_SelectedEpisode);
    //        }
    //        break;
    //      #endregion

    //      #region MediaInfo
    //      case (int)eContextItems.actionRecheckMI:
    //        switch (currentListLevel)
    //        {
    //          case Listlevel.Episode:
    //            m_SelectedEpisode.ReadMediaInfo();
    //            // reload here so logos update
    //            LoadFacade();
    //            break;
    //          case Listlevel.Season:
    //            foreach (DBMovie ep in DBMovie.Get(m_SelectedSeason[DBSeason.cSeriesID], m_SelectedSeason[DBSeason.cIndex], false))
    //              ep.ReadMediaInfo();
    //            break;
    //          case Listlevel.Series:
    //            foreach (DBMovie ep in DBMovie.Get((int)m_SelectedSeries[DBSeries.cID], false))
    //              ep.ReadMediaInfo();
    //            break;
    //        }
    //        break;
    //      #endregion

    //      #region Import
    //      case (int)eContextItems.actionLocalScan:
    //        // queue scan
    //        lock (m_parserUpdaterQueue)
    //        {
    //          m_parserUpdaterQueue.Add(new CParsingParameters(true, false));
    //        }
    //        // Start Import if delayed
    //        m_scanTimer.Change(1000, 1000);
    //        break;

    //      case (int)eContextItems.actionFullRefresh:
    //        // queue scan
    //        lock (m_parserUpdaterQueue)
    //        {
    //          m_parserUpdaterQueue.Add(new CParsingParameters(false, true));
    //        }
    //        // Start Import if delayed
    //        m_scanTimer.Change(1000, 1000);
    //        break;
    //      #endregion

    //      #region Play
    //      case (int)eContextItems.actionPlayRandom:
    //        playRandomEp();
    //        break;
    //      #endregion

    //      #region Episode Sort By
    //      case (int)eContextItems.actionEpisodeSortBy:
    //        ShowEpisodeSortByMenu(selectedSeries, false);
    //        break;
    //      #endregion

    //      #region Local to Online Episode Match Order
    //      case (int)eContextItems.actionChangeOnlineEpisodeMatchOrder:
    //        ShowEpisodeSortByMenu(selectedSeries, true);
    //        break;
    //      #endregion

    //      #region Lock Views
    //      case (int)eContextItems.actionLockViews:
    //        logicalView.IsLocked = true;
    //        break;
    //      #endregion
    //      #endregion
    //    }
    //    #endregion
    //  }
    //  catch (Exception ex)
    //  {
    //    LogMyFilms.Error("The 'OnShowContextMenu' function has generated an error: " + ex.Message + ", StackTrace : " + ex.StackTrace);
    //  }

    //}
    #endregion

    #region Action

    //---------------------------------------------------------------------------------------
    //   Handle Keyboard Actions
    //---------------------------------------------------------------------------------------

    public override void OnAction(Action action)
    {
      LogMyFilms.Debug("OnAction " + action.wID);
      switch (action.wID)
      {
        case Action.ActionType.ACTION_MOVE_LEFT:
        case Action.ActionType.ACTION_MOVE_RIGHT:
          base.OnAction(action);
          return;

        case Action.ActionType.ACTION_PARENT_DIR:
          if (GetPrevFilmList()) return;
          break;
        case Action.ActionType.ACTION_PREVIOUS_MENU:
          if (!facadeView.Focus)
          {
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List); // set focus to facade, if e.g. menu buttons had focus (after global options etc.)
            return;
          }
          if (conf.Boolselect || conf.Boolview)
          {
            Change_LayOut(MyFilms.conf.StrLayOut);
            if (GetPrevFilmList()) return;
          }
          if (conf.Boolreturn)
          {
            conf.Boolreturn = false;
            if (conf.WStrSort.ToString() == "ACTORS") // Removed "ToUpper"
              if (GetPrevFilmList())
                return;
              else
                base.OnAction(action);
            Change_view(conf.WStrSort.ToLower());
            this.SetDummyControlsForFacade(currentListLevel);
            return;
          }
          if (GetPrevFilmList())
            return;
          else
          {
            GUIWindowManager.ShowPreviousWindow();
            return;
          }
        case Action.ActionType.ACTION_KEY_PRESSED:
          base.OnAction(action);
          break;
        case Action.ActionType.ACTION_PLAY:
        case Action.ActionType.ACTION_MUSIC_PLAY:
          // Play groups as playlist (ToDo)
          base.OnAction(action);
          break;
        case Action.ActionType.ACTION_PREV_PICTURE:
        case Action.ActionType.ACTION_NEXT_PICTURE:
          // Cycle Artwork
          base.OnAction(action);
          break;
        case Action.ActionType.ACTION_CONTEXT_MENU:
          base.OnAction(action);
          break;
        default:
          if (action.m_key != null)
          {
            if ((action.m_key.KeyChar == 112) && facadeView.Focus && !facadeView.SelectedListItem.IsFolder) // 112 = "p", 120 = "x"
            {
              MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
            }
            if ((action.m_key.KeyChar == 120) && Context_Menu)
            {
              Context_Menu = false;
              return;
            }
            if (action.m_key.KeyChar == 120 && facadeView.Focus && !facadeView.SelectedListItem.IsFolder)
            {
              // context menu for update or suppress entry
              Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
              return;
            }
          }

          if (action.wID.ToString().Substring(0, 6) == "REMOTE")
            return;
          base.OnAction(action);
          break;
      }
    }

    private void GUIWindowManager_OnNewAction(Action action)
    {
      //if (currentPlaylist != null && g_Player.HasVideo && g_Player.Player.GetType().Assembly == typeof(GUIOnlineVideos).Assembly)
      //{
      //  if (action.wID == Action.ActionType.ACTION_NEXT_ITEM)
      //  {
      //    int currentPlaylistIndex = currentPlayingItem != null ? currentPlaylist.IndexOf(currentPlayingItem) : 0;
      //    // move to next
      //    if (currentPlaylist.Count > currentPlaylistIndex + 1)
      //    {
      //      currentPlaylistIndex++;
      //      Play_Step1(currentPlaylist[currentPlaylistIndex], GUIWindowManager.ActiveWindow == GUIOnlineVideoFullscreen.WINDOW_FULLSCREEN_ONLINEVIDEO);
      //    }
      //  }
      //  else if (action.wID == Action.ActionType.ACTION_PREV_ITEM)
      //  {
      //    int currentPlaylistIndex = currentPlayingItem != null ? currentPlaylist.IndexOf(currentPlayingItem) : 0;
      //    // move to previous
      //    if (currentPlaylistIndex - 1 >= 0)
      //    {
      //      currentPlaylistIndex--;
      //      Play_Step1(currentPlaylist[currentPlaylistIndex], GUIWindowManager.ActiveWindow == GUIOnlineVideoFullscreen.WINDOW_FULLSCREEN_ONLINEVIDEO);
      //    }
      //  }
      //}
    }


    ////---------------------------------------------------------------------------------------
    ////   Handle Clicked Events
    ////---------------------------------------------------------------------------------------
    //protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
    //{
    //  LogMyFilms.Debug("OnClicked() started");
    //  //if (control == this.viewMenuButton)
    //  //{
    //  //  showViewSwitchDialog();
    //  //  viewMenuButton.Focus = false;
    //  //  return;
    //  //}

    //  //if (actionType != MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?
    //  base.OnClicked(controlId, control, actionType);
    //}

    //---------------------------------------------------------------------------------------
    //   Handle posted Messages
    //---------------------------------------------------------------------------------------
    public override bool OnMessage(GUIMessage messageType)
    {
      int dControl = messageType.TargetControlId;
      int iControl = messageType.SenderControlId;
      LogMyFilms.Debug("GUIMessage: " + messageType.Message.ToString() + ", Param1: " + messageType.Param1.ToString() + ", Sender: " + iControl.ToString() + ", Target: " + dControl.ToString() + "");
      switch (messageType.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
          return base.OnMessage(messageType);

        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          return base.OnMessage(messageType);

        case GUIMessage.MessageType.GUI_MSG_CLICKED:
          //---------------------------------------------------------------------------------------
          // Mouse/Keyboard Clicked
          //---------------------------------------------------------------------------------------
          LogMyFilms.Debug("GUI_MSG_CLICKED recognized !");
          if ((iControl == (int)Controls.CTRL_BtnSrtBy) && (conf.Boolselect))
            // No change sort method and no searchs during select
            return true;
          if ((iControl == (int)Controls.CTRL_BtnSearchT) && (conf.Boolselect))
            conf.Boolselect = false;
          if (iControl == (int)Controls.CTRL_BtnSearchT)
            // Search dialog search
            Change_Search_Options();
          if (iControl == (int)Controls.CTRL_BtnSrtBy)
          // Choice of Sort Method
          {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return true;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(1079902)); // Sort by ...
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
                dlg.Add(GUILocalizeStrings.Get(1079893) + " " + conf.StrTSort[i]);//Specific sort i
                choiceSort.Add(string.Format("sort{0}", i.ToString()));
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
                conf.CurrentSortMethod = GUILocalizeStrings.Get(1079893) + " " + conf.StrTSort[i];
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
            Change_Selection_type_Video();
            return base.OnMessage(messageType);
          }
          if (iControl == (int)Controls.CTRL_BtnOptions)
          // Change Selected Option
          {
            Change_Option();
            // GUIControl.FocusControl(GetID, (int)Controls.CTRL_List); // Added to return to facade // Reremoved for Doug !
            return base.OnMessage(messageType);
          }
          if (iControl == (int)Controls.CTRL_GlobalOverlayFilter)
          // Change Selected Option - call global filters directly
          {
            Change_Global_Filters();
            // GUIControl.FocusControl(GetID, (int)Controls.CTRL_List); // Added to return to facade // Reremoved for Doug !
            return base.OnMessage(messageType);
          }
          if (iControl == (int)Controls.CTRL_ToggleGlobalUnwatchedStatus)
          // Toggle global unwatched filter directly
          {
            MyFilms.conf.GlobalUnwatchedOnly = !MyFilms.conf.GlobalUnwatchedOnly;
            if (conf.GlobalUnwatchedOnly)
            {
              //GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
              if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
                GlobalFilterStringUnwatched = conf.StrWatchedField + " like '*" + conf.StrUserProfileName + ":0*" + "' AND ";
              else
                GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
              MyFilmsDetail.setGUIProperty("globalfilter.unwatched", "true");
            }
            else
            {
              GlobalFilterStringUnwatched = String.Empty;
              MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
            }
            LogMyFilms.Info("Global filter for Unwatched Only is now set to '" + GlobalFilterStringUnwatched + "'");
            this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
            // use this code later to set the label of the button
            //if (MyFilms.conf.GlobalUnwatchedOnly)
            //  BtnToggleGlobalWatched.Label = string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798628));
            //else
            //  BtnToggleGlobalWatched.Label = string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798629));

            // GUIControl.FocusControl(GetID, (int)Controls.CTRL_List); // Added to return to facade // Reremoved for Doug !
            return base.OnMessage(messageType);
          }
          //if ((iControl == (int)Controls.CTRL_BtnLayout) && !conf.Boolselect) // conf.Boolelect is true, if it's a movie facade - so false if it's grouped views ...
          if ((iControl == (int)Controls.CTRL_BtnLayout)) // removed restriction to not change layouts for grouped views ...
          // Change Layout View
          {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return true;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(1079901)); // View (Layout) ...
            dlg.Add(GUILocalizeStrings.Get(101));//List
            if (!conf.UseListViewForGoups || !conf.Boolselect)
            {
              dlg.Add(GUILocalizeStrings.Get(100));//Icons
              dlg.Add(GUILocalizeStrings.Get(417));//Large Icons
              dlg.Add(GUILocalizeStrings.Get(733));//Filmstrip
#if MP11
#else
              dlg.Add(GUILocalizeStrings.Get(791));//Coverflow
#endif
            }
            dlg.DoModal(GetID);

            if (dlg.SelectedLabel == -1)
              return true;
            conf.StrIndex = 0;
            int wselectindex = facadeView.SelectedListItemIndex;
            Change_LayOut(dlg.SelectedLabel);
            MyFilms.conf.StrLayOut = dlg.SelectedLabel;
            dlg.DeInit();
            // GetFilmList(); // commented, as it otherwise does reset the view to filmsview, when in person or genre views ...
            if (!conf.Boolselect) // depending if it's a grouped view ...
              GetFilmList();
            else
              getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.StrSortSens, conf.Wstar, true, "");
            GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, (int)wselectindex);
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
            return base.OnMessage(messageType);
          }

          if (iControl == (int)Controls.CTRL_List && messageType.Param1 != 7)// we only handle "SELECT_ITEM" here - some other events raised onClicked too for some reason?
            return true; 
            //return base.OnMessage(messageType);

          if (iControl == (int)Controls.CTRL_List)
          {
            // if (facadeView.SelectedListItemIndex > -1 && !bgOnPageLoad.IsBusy) // do not allow going to details when loading thread still active !!!
            if (facadeView.SelectedListItemIndex > -1)
              {
              if (!facadeView.SelectedListItem.IsFolder && !conf.Boolselect)
              // New Window for detailed selected item information
              {
                conf.StrIndex = facadeView.SelectedListItem.ItemId;
                conf.StrTIndex = facadeView.SelectedListItem.Label;

                this.SetDummyControlsForFacade(currentListLevel);
                
                // GUITextureManager.CleanupThumbs(); // Guzzi: Removed to keep animations! - do cleanup later in OnPageDestroy!
                GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
              }
              else // View List as selected
              {
                conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                Change_LayOut(MyFilms.conf.StrLayOut);
                if (facadeView.SelectedListItem.IsFolder)
                  conf.Boolreturn = false;
                else
                  conf.Boolreturn = true;
                do
                {
                  if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                  conf.StrTitleSelect += conf.Wselectedlabel;
                } while (GetFilmList() == false); //keep calling while single folders found
                this.SetDummyControlsForFacade(currentListLevel);
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
      Prev_Label = string.Empty;
      string SelItem;
      if (conf.StrTitleSelect == "")
      {
        if (conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(1079870)) || (conf.StrTxtSelect == "" && conf.Boolselect) || conf.Boolview) //original code block refactored // 1079870 = "Selection"
        {
          conf.Boolselect = false;
          conf.Boolview = false;
          conf.Boolreturn = false;
          conf.StrSelect = conf.StrTxtSelect = conf.StrFilmSelect = "";
          conf.StrIndex = 0;
          GetFilmList();
          SetLabelView("all"); // if back on "root", show view as "movies"
          SetLabelSelect("root");
          currentListLevel = Listlevel.Movie;
          SetDummyControlsForFacade(currentListLevel);
          return true; //jump back to main full list  
        }

        if (conf.StrTxtSelect == "" || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798622)) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798632))) //"All" or "Global Filter"
        {
          currentListLevel = Listlevel.Movie;
          return false; //this was already "root" view - so jumping back should leave the plugin !
        }
        else
        {   // Jump back to prev view_display (categorised by year, genre etc) // Removed ACTORS special handling
          if (conf.WStrSort == "ACTORS")
          {
            conf.StrSelect = "Actors like '*" + conf.StrActors + "*'";
            SelItem = NewString.StripChars(@"[]", conf.StrTxtSelect); // Moved one up to first set SelItem to the actor and thus get back to correct facade position
            conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
            getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.StrActors, true, SelItem);
            currentListLevel = Listlevel.Person;
          }
          //else if (conf.WStrSort == "PRODUCER")
          //  {
          //    conf.StrSelect = "Producer like '*" + conf.StrActors + "*'";
          //    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
          //    getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.StrActors, true, "");
          //  }
          //else if (conf.WStrSort == "DIRECTOR")
          //  {
          //    conf.StrSelect = "Director like '*" + conf.StrActors + "*'";
          //    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
          //    getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.StrActors, true, "");
          //  }
          else
          {
            SelItem = NewString.StripChars(@"[]", conf.StrTxtSelect);
            if (conf.WStrSort == "DateAdded")
              getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", "Date", " DESC", "*", true, SelItem);
            else
              getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, SelItem);
            conf.StrSelect = "";
            switch (conf.WStrSort.ToLower())
            {
              case "producer":
              case "director":
              case "writer":
              case "actor":
              case "borrower":
                currentListLevel = Listlevel.Person;
                break;
              case "originaltitle":
              case "translatedtitle":
              case "formattedtitle":
                currentListLevel = Listlevel.Movie;
                break;
              default:
                currentListLevel = Listlevel.Group;
                break;
            }
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
        currentListLevel = Listlevel.Movie;
      }
      SetDummyControlsForFacade(currentListLevel);
      return true;
    }

    /// <summary>Sets StrFilmSelect up based on StrSelect, StrTitleSelect etc... </summary>
    static void SetFilmSelect()
    {
      string s = "";
      Prev_ItemID = -1; 
      Prev_Label = string.Empty;
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
      LogMyFilms.Debug("(SetFilmSelect) - StrFilmSelect: '" + s + "'");
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
      LogMyFilms.Debug("GetFilmList started: BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, false)");
      SetFilmSelect();
      string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
      r = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, false);
      LogMyFilms.Debug("(GetFilmList) - GlobalFilterString:          '" + GlobalFilterString + "'");
      LogMyFilms.Debug("(GetFilmList) - conf.StrDfltSelect:          '" + conf.StrDfltSelect + "'");
      LogMyFilms.Debug("(GetFilmList) - conf.StrFilmSelect:          '" + conf.StrFilmSelect + "'");
      LogMyFilms.Debug("(GetFilmList) - conf.StrSorta:               '" + conf.StrSorta + "'");
      LogMyFilms.Debug("(GetFilmList) - conf.StrSortSens:            '" + conf.StrSortSens + "'");
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
      bool tmpwatched = false;
      List<GUIListItem> facadeDownloadItems = new List<GUIListItem>();

      if (typeof(T) == typeof(int)) iSelItem = Int32.Parse(SelItem);

      // setlabels
      // TxtSelect.Label = (conf.StrTxtSelect == "") ? " " : conf.StrTxtSelect.Replace(conf.TitleDelim, @"\"); // always show as though folder path using \ regardless what sep is used
      MyFilmsDetail.setGUIProperty("select", (conf.StrTxtSelect == "") ? " " : conf.StrTxtSelect.Replace(conf.TitleDelim, @"\"));// always show as though folder path using \ regardless what sep is used

      BtnSrtBy.IsAscending = (conf.StrSortSens == " ASC");
      BtnSrtBy.Label = conf.CurrentSortMethod;

      if (conf.StrTitleSelect != "") DelimCnt = NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) + 1; //get num .'s in title
      facadeView.Clear();
      //----------------------------------------------------------------------------------------
      // Load the DataSet.
      int number = -1;
      int wfacadewiew = 0;
      ArrayList w_tableau = new ArrayList();
      bool isdate = false;
      if (conf.WStrSort == "Date" || conf.WStrSort == "DateAdded") isdate = true;
      // Check and create Group thumb folder ...
      if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups"))
        System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups");

      foreach (DataRow sr in r)
      {
        if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.MovieID)) // if load params for movieid exist, set current index to the movie detected
        {
          if (sr["number"].ToString() == loadParamInfo.MovieID)
          {
            int index = 0;
            bool success = int.TryParse(loadParamInfo.MovieID, out index);
            conf.StrIndex = index;
            conf.StrTIndex = sr[conf.StrTitle1].ToString();
          }
        }

        tmpwatched = false; 
        number++;
        if (conf.Boolreturn)//in case of selection by view verify if value correspond excatly to the searched string
        {
          w_tableau = Search_String(sr[conf.WStrSort].ToString());
          foreach (object t in w_tableau)
          {
            if (isdate)
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
        //LogMyFilms.Debug("(GetFilmList) - BuildDisplaylist - FullTitle: '" + sFullTitle + "'");

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
            item.TVTag = "group";

            item.ThumbnailImage = conf.FileImage;
            item.IconImage = conf.FileImage;
          }
        }
        else
        {
          iCnt = 1;
          item = new GUIListItem();
          item.Label = sFullTitle; // Set = full subfolders path initially
          item.TVTag = "film";
          if (!MyFilms.conf.OnlyTitleList)
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
                try {item.Label2 = sr["Date"].ToString();}
                catch {}
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

          if (conf.StrEnhancedWatchedStatusHandling)
          {
            if (!sr[conf.StrWatchedField].ToString().StartsWith("Global:"))
            {
              if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
                if (!string.IsNullOrEmpty(conf.GlobalUnwatchedOnlyValue) && sr[conf.StrWatchedField].ToString().ToLower() != conf.GlobalUnwatchedOnlyValue.ToLower() && sr[conf.StrWatchedField].ToString().Length > 0) // changed to take setup config into consideration
                  tmpwatched = true;
              if (tmpwatched) sr[conf.StrWatchedField] = "Global:1:-1";
              else sr[conf.StrWatchedField] = "Global:0:-1";
            }
            if (!sr[conf.StrWatchedField].ToString().Contains(conf.StrUserProfileName + ":"))
            {
                if (tmpwatched) sr[conf.StrWatchedField] = sr[conf.StrWatchedField].ToString() + "|" + conf.StrUserProfileName + ":1:-1";
                else sr[conf.StrWatchedField] = sr[conf.StrWatchedField].ToString() + "|" + conf.StrUserProfileName + ":0:-1";
            }
            if (EnhancedWatched(sr[conf.StrWatchedField].ToString(), conf.StrUserProfileName) == true)
              item.IsPlayed = true;
          }
          else
          {
            if (sr[conf.StrWatchedField].ToString().StartsWith("Global:"))
            {
              string s = sr[conf.StrWatchedField].ToString();
              string count = s.Substring(s.IndexOf(":") + 1, 1);
              if (count == "0")
                sr[conf.StrWatchedField] = conf.GlobalUnwatchedOnlyValue;
              else 
                sr[conf.StrWatchedField] = "true";
            }
            if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
              if (sr[conf.StrWatchedField].ToString().ToLower() != conf.GlobalUnwatchedOnlyValue.ToLower()) // changed to take setup config into consideration
                item.IsPlayed = true;
          }
          if (MyFilms.conf.StrSuppress && MyFilms.conf.StrSuppressField.Length > 0)
            if ((sr[MyFilms.conf.StrSuppressField].ToString() == MyFilms.conf.StrSuppressValue.ToString()) && (MyFilms.conf.StrSupPlayer))
              item.IsPlayed = true;

          if (sr["Picture"].ToString().Length > 0)
          {
            if ((sr["Picture"].ToString().IndexOf(":\\") == -1) && (sr["Picture"].ToString().Substring(0, 2) != "\\\\"))
              conf.FileImage = conf.StrPathImg + "\\" + sr["Picture"].ToString();
            else
              conf.FileImage = sr["Picture"].ToString();
          }
          else
          {
            conf.FileImage = string.Empty;
          }
          item.DVDLabel = sTitle; // used by background thread
          item.ThumbnailImage = conf.FileImage;
          item.IconImageBig = conf.FileImage;
          item.IconImage = conf.FileImage;

          item.ItemId = number;
          // set availability status
          if (InitialIsOnlineScan) // only display media status, if onlinescan was done
          {
            if (string.IsNullOrEmpty(sr["IsOnline"].ToString()))
              item.IsRemote = false;
            else
              if (bool.Parse(sr["IsOnline"].ToString())) // if its online, set IsRemote to false !
              {
                item.IsRemote = false;
              }
              else
                item.IsRemote = true;
          }
          facadeDownloadItems.Add(item);
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
        item = new GUIListItem();
        item.Label = GUILocalizeStrings.Get(10798639);
        item.IsRemote = true;
        facadeView.Add(item);
        item.FreeMemory();
        ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798639));
        //GUIWaitCursor.Show();
        //DisplayAllMovies();
        //InitMainScreen(false);
        GUIControl.ShowControl(GetID, 34); // hides certain GUI elements
        //SetLabelSelect("root");
        //SetLabelView("all");
      }
      else
      {
        //ImgFanart.SetVisibleCondition(1, true);  ->> This fucked up the fanart swapper !!!!!
        Fanartstatus(true);
        GUIControl.HideControl(GetID, 34);
      }
      MyFilmsDetail.setGUIProperty("nbobjects.value", facadeView.Count.ToString());
      GUIPropertyManager.SetProperty("#itemcount", facadeView.Count.ToString());
      GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, (int)wfacadewiew);
      if (facadeView.Count == 1 && item.IsFolder)
      {
        conf.Boolreturn = false;
        conf.Wselectedlabel = item.Label;
      }
      currentListLevel = Listlevel.Movie;
      LogMyFilms.Debug("GetFilmList finished!");
      GetImagesFilmList(facadeDownloadItems);
      return !(facadeView.Count == 1 && item.IsFolder); //ret false if single folder found
    }

    private void GetImagesFilmList(List<GUIListItem> itemsWithThumbs)
    {
      StopDownload = false;

      // split the downloads in X+ groups and do multithreaded downloading
      int groupSize = (int)Math.Max(1, Math.Floor((double)itemsWithThumbs.Count / 2)); // Guzzi: Set group to x to only allow x thread(s)
      int groups = (int)Math.Ceiling((double)itemsWithThumbs.Count / groupSize);

      for (int i = 0; i < groups; i++)
      {
        List<GUIListItem> groupList = new List<GUIListItem>();
        for (int j = groupSize * i; j < groupSize * i + (groupSize * (i + 1) > itemsWithThumbs.Count ? itemsWithThumbs.Count - groupSize * i : groupSize); j++)
        {
          groupList.Add(itemsWithThumbs[j]);
        }

        new System.Threading.Thread(delegate(object o)
        {
          List<GUIListItem> items = (List<GUIListItem>)o;
          foreach (GUIListItem item in items)
          {
            // stop download if we have exited window
            if (StopDownload) break;

            string strThumb = string.Empty;
            //if (!File.Exists(item.ThumbnailImage)) // No Coverart in DB - so handle it !
            if (item.ThumbnailImage == "" || !File.Exists(item.ThumbnailImage)) // No Coverart in DB - so handle it !
              {
              //string strlabel = item.Label;
              //MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(ref strlabel);
              //strThumb = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups\" + strlabel;
              //if (System.IO.File.Exists(strThumb + ".png"))
              //{
              //  conf.FileImage = strThumb + ".png"; 
              //}
              //else
              {
                //conf.FileImage = string.Empty;
                //try
                //{
                //    if (conf.StrPathViews.Length > 0)
                //        if (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\")
                //            //Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".png", strThumb + ".png", cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
                //            createCacheThumb(conf.StrPathViews + item.Label + ".png", strThumb + ".png");
                //        else
                //            //Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".png", strThumb + ".png", cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
                //            createCacheThumb(conf.StrPathViews + "\\" + item.Label + ".png", strThumb + ".png");
                //    // Disabled "pseudo covers with label name"
                //    //if (!System.IO.File.Exists(strThumb + ".png"))
                //    //    if (MyFilms.conf.StrViewsDflt && System.IO.File.Exists(MyFilms.conf.DefaultCoverViews))
                //    //        ImageFast.CreateImage(strThumb + ".png", item.Label);
                //    if (System.IO.File.Exists(strThumb + ".png"))                                
                //    conf.FileImage = strThumb + ".png"; 
                //}
                //catch
                //{
                //    conf.FileImage = string.Empty;
                //}
                //if (string.IsNullOrEmpty(conf.FileImage) && conf.DefaultCover.Length > 0)
                //if (conf.DefaultCover.Length > 0)
                //  conf.FileImage = conf.DefaultCover;
              }
            }
            strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, item.DVDLabel); // item.DVDLabel is sTitle
            if (!System.IO.File.Exists(strThumb) && item.ThumbnailImage != conf.DefaultCover && !string.IsNullOrEmpty(item.ThumbnailImage))
            {
              Picture.CreateThumbnail(item.ThumbnailImage, strThumb, 100, 150, 0, Thumbs.SpeedThumbsSmall);
              LogMyFilms.Debug("GetFimList: Background thread creating thumbimage for sTitle: '" + item.DVDLabel.ToString() + "'");
            }
            if (System.IO.File.Exists(strThumb))
            {
              item.IconImage = strThumb;
            }
            else
            {
              if (conf.DefaultCover.Length > 0)
              {
                item.IconImage = conf.DefaultCover;
                item.IconImageBig = conf.DefaultCover;
                item.ThumbnailImage = conf.DefaultCover;
              }
            }
          // Thread.Sleep(10);
          }
        })
        {
          IsBackground = true, Priority = ThreadPriority.BelowNormal,
          Name = "MyFilms FilmList Image Detector" + i.ToString()
        }.Start(groupList);
      }
    }


    bool GetMoviesInGroup(ref ArrayList fanartItems)
    {
      LogMyFilms.Debug("GetRandomFanartForGroups started: BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, false)");

      string s = "";
      string StrSelect = string.Empty;
      if (conf.Boolselect)
      {
        string sLabel = facadeView.SelectedListItem.Label;
        if ((conf.WStrSort == "Date") || (conf.WStrSort == "DateAdded"))
          StrSelect = "Date" + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(sLabel).ToShortDateString()) + "*'";
        else
        {
          if (sLabel == "")
            StrSelect = conf.WStrSort + " is NULL";
          else
            StrSelect = conf.WStrSort + " like '*" + sLabel.Replace("'", "''") + "*'";
        }
      }

      if (StrSelect != "")
        s = StrSelect + " And ";
      if (conf.StrTitleSelect != "") //' in blake's seven causes fuckup
        s = s + String.Format("{0} like '{1}{2}*'", conf.StrTitle1.ToString(), conf.StrTitleSelect.Replace("'", "''"), conf.TitleDelim);
      else
        s = s + conf.StrTitle1.ToString() + " not like ''";
      string StrFilmSelect = s;

      ArrayList RandomFanartItems = new ArrayList();

      string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
      r = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, StrFilmSelect, conf.StrSorta, conf.StrSortSens, false);
      LogMyFilms.Debug("(GetRandomFanartForGroups) - GlobalFilterString:          '" + GlobalFilterString + "'");
      LogMyFilms.Debug("(GetRandomFanartForGroups) - conf.StrDfltSelect:          '" + conf.StrDfltSelect + "'");
      LogMyFilms.Debug("(GetRandomFanartForGroups) - conf.StrFilmSelect:          '" + StrFilmSelect + "'");
      LogMyFilms.Debug("(GetRandomFanartForGroups) - conf.StrSorta:               '" + conf.StrSorta + "'");
      LogMyFilms.Debug("(GetRandomFanartForGroups) - conf.StrSortSens:            '" + conf.StrSortSens + "'");
      LogMyFilms.Debug("(GetRandomFanartForGroups) - Count:                       '" + r.Length + "'");
      int iCnt = 0;
      int DelimCnt = 0, DelimCnt2;
      GUIListItem item = new GUIListItem();
      string sTitle;
      string sFullTitle;
      string sPrevTitle = "";
      //string SelItem = gSelItem.ToString();
      //int iSelItem = -2;

      // if (typeof(T) == typeof(int)) iSelItem = Int32.Parse(SelItem);

      if (conf.StrTitleSelect != "") DelimCnt = NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) + 1; //get num .'s in title
      //----------------------------------------------------------------------------------------
      // Load the DataSet.
      int number = -1;
      ArrayList w_tableau = new ArrayList();
      bool isdate = false;
      if (conf.WStrSort == "Date" || conf.WStrSort == "DateAdded") isdate = true;

      foreach (DataRow sr in r)
      {
        number++;
        if (conf.Boolreturn)//in case of selection by view verify if value correspond excatly to the searched string
        {
          w_tableau = Search_String(sr[conf.WStrSort].ToString());
          foreach (object t in w_tableau)
          {
            if (isdate)
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
          }
        }
        else
        {
          iCnt = 1;
          item = new GUIListItem();
          item.Label = sFullTitle; // Set = full subfolders path initially
          item.ItemId = number;
          if (!item.IsFolder)
            RandomFanartItems.Add(item);
        }
      fin: ;
      }
      fanartItems = RandomFanartItems;
      return !(RandomFanartItems.Count == 1 && item.IsFolder); //ret false if single folder found
    }

    private bool GetRandomFanartForGroups(int limit)
    {
      ArrayList fanartItems = new ArrayList();
      int i = 0;
      GetMoviesInGroup(ref fanartItems);
      currentFanartList.Clear(); // clear list from former content
      foreach (GUIListItem randomFanartItem in fanartItems)
      {
        string[] wfanart = new string[2];

        wfanart = MyFilmsDetail.Search_Fanart(randomFanartItem.Label, true, "file", false, string.Empty, string.Empty);
        if (wfanart[0] != " " && wfanart[0] != MyFilms.conf.DefaultFanartImage && !currentFanartList.Contains(wfanart[0]))
        {
          currentFanartList.Add(wfanart[0]);
          LogMyFilms.Debug("GetRandomFanartForGroups() - added fanart #" + currentFanartList.Count + " : '" + wfanart[0] + "'");
          i += 1;
          if (i >= limit && limit != 0)
            return true;
        }
      }
      if (currentFanartList.Count > 0)
        return true;
      else
        return false;
    }

    private bool GetRandomFanartForFilms(int limit)
    {
      int i = 0;
      string fanartDirectory = string.Empty;
      currentFanartList.Clear(); // clear list from former content
      string[] wfanart = new string[2];

      string fanartTitle, personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
      fanartTitle = MyFilmsDetail.GetFanartTitle(r[facadeView.SelectedListItem.ItemId], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
//       fanartTitle = facadeView.SelectedListItem.Label;
      wfanart = MyFilmsDetail.Search_Fanart(fanartTitle, false, "dir", false, "false", string.Empty);

      if (wfanart[1] == "dir")
        fanartDirectory = wfanart[0];

      if (!string.IsNullOrEmpty(fanartDirectory) && System.IO.Directory.Exists(fanartDirectory))
      {
        string searchname = "*.jpg";
        string[] files = Directory.GetFiles(fanartDirectory, searchname, SearchOption.TopDirectoryOnly);
        if (files.Length > 0)
          foreach (string file in files)
          {
            if (!currentFanartList.Contains(file))
            {
              currentFanartList.Add(file);
              LogMyFilms.Debug("GetRandomFanartForFilms() - added fanart #" + currentFanartList.Count + " : '" + file + "'");
              i += 1;
              if (i >= limit && limit != 0)
                return true;
            }
          }
      }
      if (currentFanartList.Count > 0)
        return true;
      else
        return false;
    }


    private string GetNewRandomFanart(bool reset, bool movie)
    {
      string newFanart = " ";
      bool success = false;
      int errorcount = 0;

      if (reset)
      {
        currentFanartList.Clear();
        if (movie) 
          GetRandomFanartForFilms(25);
        else
          GetRandomFanartForGroups(25); // Limit items for performance reasons...
      }
      if (currentFanartList.Count == 0)
        newFanart = " ";
      else if (currentFanartList.Count == 1) 
        newFanart = currentFanartList[0];
      else if (currentFanartList.Count > 1)
      {
        while (!success && errorcount < 10)
        {
          try
          {
            //Choose Random Fanart from Resultlist
            System.Random rnd = new System.Random();
            Int32 randomFanartIndex = rnd.Next(currentFanartList.Count);
            newFanart = currentFanartList[randomFanartIndex];
            if (newFanart != backdrop.Filename)
            {
              success = true;
              LogMyFilms.Debug("GetNewRandomFanart() - Available: '" + currentFanartList.Count + "', selected ID: '" + randomFanartIndex + "', selected Path: '" + newFanart + "'");
            }
            else
            {
              errorcount += 1;
              Thread.Sleep(25);
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.DebugException("GetNewRandomFanart() - error, invalid index !", ex);
            errorcount += 1;
            success = false;
          }
        }
      }
      return newFanart;
    }

    public static bool EnhancedWatched(string strEnhancedWatchedValue, string strUserProfileName)
    {
      if (strEnhancedWatchedValue.Contains(strUserProfileName + ":0"))
      {
        return false;
      }
      
      if (!strEnhancedWatchedValue.Contains(strUserProfileName + ":"))
      {
        return false;
      }
      return true; // count > 0 -> return true
    }

    //----------------------------------------------------------------------------------------
    //    Display rating (Hide/show Star Images)
    //    altered to add half stars
    //   0-0.4=(0)=0s  | 0.5-1.4=(1)=0.5s | 1.5-2.4=(2)=1s   | 2.5-3.4=(3)=1.5s 
    // 3.5-4.4=(4)=2s  | 4.5-5.4=(5)=2.5s | 5.5-6.4=(6)=3s   | 6.5-7.4=(7)=3.5s
    // 7.5-8.4=(8)=4s  | 8.5-9.4=(9)=4.5s | 9.5-10=(10)=5s
    //----------------------------------------------------------------------------------------
    private void Load_Rating(decimal rating)
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
    //private void Load_Lstdetail(GUIListItem currentItem, int ItemId, string wlabel)//wrep = false display only image, all properties cleared
    private void Load_Lstdetail(GUIListItem currentItem, bool forceLoading)
    {
      LogMyFilms.Debug("Load_Lstdetail - Start: ItemId = " + currentItem.ItemId + ", label = " + currentItem.Label + ", TVtag = " + currentItem.TVTag);
      if ((currentItem.ItemId == Prev_ItemID && currentItem.Label == Prev_Label) && !forceLoading)
      {
        LogMyFilms.Debug("(Load_Lstdetail): ItemId == Prev_ItemID (" + Prev_ItemID + ") -> return");
        return;
      }
      if (currentItem.ItemId == -1)
      {
        // reinit some fields //filmcover.Filename = ""; //backdrop.Filename = ""; //MyFilmsDetail.Init_Detailed_DB(false);
        LogMyFilms.Debug("(Load_Lstdetail): ItemId == -1 -> return");
        return;
      }

      Prev_ItemID = currentItem.ItemId;
      Prev_Label = currentItem.Label;

      switch (currentListLevel) // ToDo: Add and change code on conditions
      {
        case Listlevel.Movie:
          break;
        case Listlevel.Group:
          break;
        case Listlevel.Person:
          break;
        case Listlevel.None:
          break;
        default:
          break;
      }

      if ((currentItem.IsFolder) && (MyFilms.conf.Boolselect))
      {
        LogMyFilms.Debug("(Load_Lstdetail): Item is Folder and BoolSelect is true");
        string[] wfanart = MyFilmsDetail.Search_Fanart(currentItem.Label, true, "file", true, currentItem.ThumbnailImage.ToString(), currentItem.Path);
        if (conf.StrFanartDefaultViewsUseRandom)
        {
          string GroupFanart = GetNewRandomFanart(true, false); // resets and populates fanart list and selects a random one
          if (GroupFanart != " ")
          {
            wfanart[0] = GroupFanart;
          }
        }
        if (wfanart[0] == " ")
        {
          Fanartstatus(false);
          GUIControl.HideControl(GetID, 35);
        }
        else
        {
          Fanartstatus(true);
          GUIControl.ShowControl(GetID, 35);
        }
        backdrop.Filename = wfanart[0];
        MyFilmsDetail.setGUIProperty("currentfanart", wfanart[0]);
        LogMyFilms.Debug("(Load_Lstdetail): Backdrop status: '" + backdrop.Active.ToString() + "', backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");

        conf.FileImage = currentItem.ThumbnailImage;
        MyFilmsDetail.setGUIProperty("picture", MyFilms.conf.FileImage, true);
        //if (this.facadeView.CurrentLayout != GUIFacadeControl.Layout.CoverFlow)
        //{
        //  if (!filmcover.Active) filmcover.Active = true;
        //  if (!groupcover.Active) groupcover.Active = true;
        //  if (!personcover.Active) personcover.Active = true;
        //  groupcover.Filename = conf.FileImage;
        //}
        //else
        //{
        //  if (filmcover.Active) filmcover.Active = false;
        //  if (groupcover.Active) groupcover.Active = false;
        //  if (personcover.Active) personcover.Active = false;
        //  LogMyFilms.Debug("(Load_Lstdetail): Cover deactivated due to Layout.CoverFlow");
        //}
        groupcover.Filename = conf.FileImage;
        filmcover.Filename = conf.FileImage; // Added for backwardcompatibility - might be removed in later releases, when skins are changed

        //GUIControl.ShowControl(GetID, 34);
        //Load_Rating(0); // old method - nor more used
        Load_Logos(MyFilms.r[currentItem.ItemId], false); // reset logos

      }
      else
      {
        LogMyFilms.Debug("(Load_Lstdetail): Item is Movie itself!");
        if (conf.StrFanartDefaultViewsUseRandom)
          currentFanartList.Clear();

        string[] wfanart = new string[2];

        string fanartTitle, personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
        fanartTitle = MyFilmsDetail.GetFanartTitle(r[facadeView.SelectedListItem.ItemId], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
        // fanartTitle = currentItem.Label;
        //if (MyFilms.conf.StrTitle1 == "FormattedTitle") // added to get fanart displayed when mastertitle is set to formattedtitle
        //{
        //  fanartTitle = MyFilms.r[facadeView.SelectedListItem.ItemId]["OriginalTitle"].ToString();
        //  //Added by Guzzi to fix Fanartproblem when Mastertitle is set to OriginalTitle
        //  if (MyFilms.conf.StrTitle1 != "OriginalTitle")
        //  {

        //    if (MyFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"] != null && MyFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"].ToString().Length > 0)
        //      fanartTitle = MyFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"].ToString();
        //  }
        //}
        LogMyFilms.Debug("(Load_Lstdetail): FanartTitle from facadeview = '" + fanartTitle + "'");
        
        wfanart = MyFilmsDetail.Search_Fanart(fanartTitle, true, "file", false, currentItem.ThumbnailImage, string.Empty);
        if (conf.StrFanartDefaultViewsUseRandom)
        {
          string FilmFanart = GetNewRandomFanart(true, true); // resets and populates fanart list and selects a random one
          if (FilmFanart != " ")
          {
            wfanart[0] = FilmFanart;
          }
        }

        LogMyFilms.Debug("(Load_Lstdetail): Backdrops-File: wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
        if (wfanart[0] == " ")
        {
          Fanartstatus(false);
          GUIControl.HideControl(GetID, 35);
        }
        else
        {
          Fanartstatus(true);
          GUIControl.ShowControl(GetID, 35);
        }
        backdrop.Filename = wfanart[0];
        MyFilmsDetail.setGUIProperty("currentfanart", wfanart[0]);
        LogMyFilms.Debug("(Load_Lstdetail): Fanart-Status: '" + backdrop.Active + "', Backdrops-File: backdrop.Filename = wfanart[X]: '" + wfanart[0] + "', '" + wfanart[1] + "'");

        conf.FileImage = currentItem.ThumbnailImage;
        MyFilmsDetail.setGUIProperty("picture", MyFilms.conf.FileImage, true);
        //if (this.facadeView.CurrentLayout != GUIFacadeControl.Layout.CoverFlow)
        //{
        //  if (!filmcover.Active) filmcover.Active = true;
        //  if (!groupcover.Active) groupcover.Active = true;
        //  if (!personcover.Active) personcover.Active = true;
        //  filmcover.Filename = conf.FileImage;
        //}
        //else
        //{
        //  if (filmcover.Active) filmcover.Active = false;
        //  if (groupcover.Active) groupcover.Active = false;
        //  if (personcover.Active) personcover.Active = false;
        //  LogMyFilms.Debug("(Load_Lstdetail): Cover deactivated due to Layout.CoverFlow");
        //}
        filmcover.Filename = conf.FileImage;
        //}

        Load_Logos(MyFilms.r[currentItem.ItemId], true); // set logos
      }

      LogMyFilms.Debug("switch (currentItem.TVTag.ToString() with TVTag = '" + currentItem.TVTag.ToString() + "', currentItem.ItemId = '" + currentItem.ItemId.ToString() + "'"); 
      switch (currentItem.TVTag.ToString()) //Make a difference between movies and persons -> Load_Detailed_DB or Load_Detailed_PersonInfo
      {
        case "person":
          MyFilmsDetail.clearGUIProperty("user.source.isonline");
          MyFilmsDetail.clearGUIProperty("user.sourcetrailer.isonline");

          MyFilmsDetail.Load_Detailed_DB(currentItem.ItemId, false); 
          MyFilmsDetail.Load_Detailed_PersonInfo(currentItem.Label, true); 
          break;
        case "group":
          MyFilmsDetail.clearGUIProperty("user.source.isonline");
          MyFilmsDetail.clearGUIProperty("user.sourcetrailer.isonline");

          MyFilmsDetail.Load_Detailed_DB(currentItem.ItemId, false);
          break;
        case "film":
          MyFilmsDetail.Load_Detailed_DB(currentItem.ItemId, true);
          break;
        default:
          MyFilmsDetail.Load_Detailed_DB(currentItem.ItemId, false);
          //GUIControl.ShowControl(GetID, 34);
          break;
      }
      // Load_Rating(conf.W_rating); // old method - no more used
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
      if (e.Order.ToString().Substring(0, 3).ToLower() == conf.StrSortSens.Substring(1, 3).ToLower())
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
      // LogMyFilms.Debug("Call item_OnItemSelected()with options - item: '" + item.ItemId + "', SelectedListItemIndex: '" + facadeView.SelectedListItemIndex.ToString() + "', Label: '" + facadeView.SelectedListItem.Label + "', TVtag: '" + item.TVTag.ToString() + "'");

      GUIFilmstripControl filmstrip = parent as GUIFilmstripControl;
      if (filmstrip != null)
        filmstrip.InfoImageFileName = item.ThumbnailImage;

      //if (facadeView.SelectedListItem.ItemId == Prev_ItemID && facadeView.SelectedListItem.Label == Prev_Label)
      //{
      //  LogMyFilms.Debug("(item_OnItemSelected): ItemId == Prev_ItemID (" + Prev_ItemID + ") && label == Prev_Label (" + Prev_Label + ") -> return without action !");
      //  return;
      //}

      MovieDetailsPublisher(item, true); // try: load MovieDetailsPublisher always ...

      //if (!(conf.Boolselect || (facadeView.SelectedListItemIndex > -1 && facadeView.SelectedListItem.IsFolder))) //xxxx
      //{
      //  if (facadeView.SelectedListItemIndex > -1)
      //    MovieDetailsPublisher(item, true); //Load_Lstdetail(facadeView.SelectedListItem.ItemId, true, facadeView.SelectedListItem.Label);
      //}
      //else
      //{
      //  if (facadeView.SelectedListItemIndex > -1 && !conf.Boolselect)
      //  {
      //    MovieDetailsPublisher(item, false); //Load_Lstdetail(facadeView.SelectedListItem.ItemId, false, facadeView.SelectedListItem.Label);
      //  }
      //  else
      //  {
      //    MovieDetailsPublisher(item, false); //Load_Lstdetail(facadeView.SelectedListItem.ItemId, false, facadeView.SelectedListItem.Label);
      //    //GUIControl.ShowControl(GetID, 34);
      //  }
      //}
      // Load_Lstdetail(item.ItemId, true, item.Label);
    }

    delegate void MovieDetailsPublisherWorker(GUIListItem item, bool wrep);
    
    //private void MovieDetailsPublisher(int ItemId, string wlabel)
    private void MovieDetailsPublisher(GUIListItem item, bool wrep)
    {
      LogMyFilms.Debug("Call MovieDetailsPublisher() with options - ItemId    : '" + item.ItemId + "', label: '" + item.Label + "'");
      double tickCount = System.Windows.Media.Animation.AnimationTimer.TickCount;
      // Update instance of delayed item with current position
      itemToPublish = item;
      // Publish instantly when previous request has passed the required delay
      if (125 < (int)(tickCount - lastPublished)) // wait 125 ms to load details...
      {
        lastPublished = tickCount;
        MovieDetailsPublisherWorker publisher = new MovieDetailsPublisherWorker(Load_Lstdetail);
        publisher.BeginInvoke(itemToPublish, false, null, null);

        // Load_Lstdetail(itemToPublish, false);
        // Load_Lstdetail(ItemId, wlabel); 
        return;
      }
      else // Publish on timer using delay specified
      {
        lastPublished = tickCount;
        if (publishTimer == null)
          publishTimer = new Timer(delegate { Load_Lstdetail(itemToPublish, false); }, null, 150, Timeout.Infinite);
        else
          publishTimer.Change(150, Timeout.Infinite);
      }
    }

    #region Accès Données

    //--------------------------------------------------------------------------------------------
    //  Select View for Video
    //--------------------------------------------------------------------------------------------
    private void Change_Selection_type_Video()
    {
      GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg1 == null) return;
      dlg1.Reset();
      dlg1.SetHeading(GUILocalizeStrings.Get(1079903)); // Change View (films) ...
      dlg1.Add(GUILocalizeStrings.Get(342));//videos
      dlg1.Add(GUILocalizeStrings.Get(345));//year
      dlg1.Add(GUILocalizeStrings.Get(10798664));//genre -> category
      dlg1.Add(GUILocalizeStrings.Get(200026));//countries
      dlg1.Add(GUILocalizeStrings.Get(10798667));//actors 
      //dlg1.Add(GUILocalizeStrings.Get(200027));//Watched
      System.Collections.Generic.List<string> choiceView = new System.Collections.Generic.List<string>();
      choiceView.Add("all");
      choiceView.Add("year");
      choiceView.Add("category");
      choiceView.Add("country");
      choiceView.Add("actors");

      // Commented, as we have replaced this feature with global overlay filter for "media available"
      //if (conf.StrStorage.Length != 0 && conf.StrStorage != "(none)")
      //{
      //    dlg1.Add(GUILocalizeStrings.Get(154) + " " + GUILocalizeStrings.Get(1951));//storage
      //    choiceView.Add("storage");
      //}

      for (int i = 0; i < 5; i++)
      {
        if (conf.StrViewItem[i] != null && conf.StrViewItem[i] != "(none)" && (conf.StrViewItem[i].Length > 0))
        {
          choiceView.Add(string.Format("view{0}", i));
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
      SetDummyControlsForFacade(currentListLevel);
      return;
    }

    //--------------------------------------------------------------------------------------------
    //  Select main Options
    //--------------------------------------------------------------------------------------------
    private void Change_Option()
    {
      GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg1 == null) return;
      System.Collections.Generic.List<string> choiceView = new System.Collections.Generic.List<string>();
      dlg1.Reset();
      dlg1.SetHeading(GUILocalizeStrings.Get(10798701)); // Options ...
      if (Configuration.NbConfig > 1)
      {
        dlg1.Add(GUILocalizeStrings.Get(6022));   // Change Config 
        //dlg1.Add(GUILocalizeStrings.Get(6029) + " " + GUILocalizeStrings.Get(6022));   // Change Config 
        choiceView.Add("config");
      }

      if (MyFilms.conf.StrCheckWOLenable) // Show current NAS server status
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

      // Add Submenu for useritemx mapping
      dlg1.Add(string.Format(GUILocalizeStrings.Get(10798771)));
      choiceView.Add("globalmappings");

      // Add Submenu for Wiki Online Help
      if (MyFilmsDetail.ExtendedStartmode("Contextmenu for Wiki Onlinehelp")) // check if specialmode is configured for disabled features
      {
        dlg1.Add(string.Format(GUILocalizeStrings.Get(10798699)));
        choiceView.Add("globalwikihelp");
      }

      dlg1.Add(string.Format(GUILocalizeStrings.Get(10798700))); // About ...
      choiceView.Add("about");

      dlg1.DoModal(GetID);

      if (dlg1.SelectedLabel == -1)
      {
        return;
      }
      Change_view(choiceView[dlg1.SelectedLabel].ToLower());
      return;
    }

    //--------------------------------------------------------------------------------------------
    //  Select main global filters
    //--------------------------------------------------------------------------------------------
    private void Change_Global_Filters()
    {
      Context_Menu = true; // make sure, it's set, as otherwise we fall back to first menu level ...
      GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg1 == null) return;
      dlg1.Reset();
      dlg1.SetHeading(GUILocalizeStrings.Get(10798714)); // Global Filters ...
      System.Collections.Generic.List<string> choiceViewGlobalOptions = new System.Collections.Generic.List<string>();

      // Change global Unwatchedfilteroption
      // if ((MesFilms.conf.CheckWatched) || (MesFilms.conf.StrSupPlayer))// Make it conditoional, so only displayed, if options enabled in setup !
      if (MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798628)));
      if (!MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798629)));
      choiceViewGlobalOptions.Add("globalunwatchedfilter");


      // Change global MovieFilter (Only Movies with media files reachable 
      if (InitialIsOnlineScan) // (requires at least initial scan!)
      {
        if (GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798628)));
        if (!GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798629)));
        choiceViewGlobalOptions.Add("filterdbisonline");
      }

      // Change global MovieFilter (Only Movies with Trailer)
      if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
      {
        if (GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798628)));
        if (!GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798629)));
        choiceViewGlobalOptions.Add("filterdbtrailer");
      }

      // Change global MovieFilter (Only Movies with highRating)
      if (GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798628)));
      if (!GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798629)));
      choiceViewGlobalOptions.Add("filterdbrating");

      // Change Value for global MovieFilter (Only Movies with highRating)
      dlg1.Add(string.Format(GUILocalizeStrings.Get(10798693), MyFilms.conf.StrAntFilterMinRating.ToString()));
      choiceViewGlobalOptions.Add("filterdbsetrating");

      dlg1.DoModal(GetID);
      if (dlg1.SelectedLabel == -1)
        return;
      LogMyFilms.Debug("Call change_global_filters menu with option: '" + choiceViewGlobalOptions[dlg1.SelectedLabel].ToString() + "'");
      Change_view(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
      return;
    }

    private void Change_Search_Options()
    {
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return; // true;

      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(137) + " ..."); // Search ...
      System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
      //Add Menuentries here

      if (MyFilms.conf.StrSearchList[0].Length > 0)
      {
        dlg.Add(GUILocalizeStrings.Get(10798615)); //Search global movies by property
        choiceSearch.Add("globalproperty");
      }

      if (MyFilmsDetail.ExtendedStartmode("Global Random Movie Search"))
      {
        //Guzzi: RandomMovie Search added
        dlg.Add(GUILocalizeStrings.Get(10798621)); //Search global movies by randomsearch (singlesearch, areasearch)
        choiceSearch.Add("randomsearch");
      }

      if (MyFilmsDetail.ExtendedStartmode("Global Search Movies by Areas"))
      {
        dlg.Add(GUILocalizeStrings.Get(10798645)); //Search global movies by areas
        choiceSearch.Add("globalareas");
      }

      dlg.Add(GUILocalizeStrings.Get(137) + " " + GUILocalizeStrings.Get(369)); //Title
      choiceSearch.Add("title");

      dlg.Add(GUILocalizeStrings.Get(137) + " " + GUILocalizeStrings.Get(344)); //Actors
      choiceSearch.Add("actors");
      for (int i = 0; i < 2; i++)
      {
        if (MyFilms.conf.StrSearchItem[i] != "(none)" && MyFilms.conf.StrSearchItem[i].Length > 0)
        {
          if (MyFilms.conf.StrSearchText[i].Length == 0) dlg.Add(GUILocalizeStrings.Get(137) + " " + MyFilms.conf.StrSearchItem[i]); //Specific search with no text
          else dlg.Add(GUILocalizeStrings.Get(137) + " " + MyFilms.conf.StrSearchText[i]); //Specific search  text
          choiceSearch.Add(string.Format("search{0}", i.ToString()));
        }
      }
      if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
      {
        dlg.Add(GUILocalizeStrings.Get(1079866)); //Search related movies by persons
        choiceSearch.Add("analogyperson");
      }
      if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder &&
          MyFilms.conf.StrSearchList[0].Length > 0)
      {
        dlg.Add(GUILocalizeStrings.Get(10798614)); //Search related movies by property
        choiceSearch.Add("analogyproperty");
      }

      dlg.DoModal(GetID);

      if (dlg.SelectedLabel == -1) 
        return; // true;
      if (choiceSearch[dlg.SelectedLabel] == "analogyperson")
      {
        SearchRelatedMoviesbyPersons((int)facadeView.SelectedListItem.ItemId, false);
        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
        dlg.DeInit();
        return;
        //return base.OnMessage(messageType);
      }

      if (choiceSearch[dlg.SelectedLabel] == "analogyproperty")
      {
        SearchRelatedMoviesbyProperties((int)facadeView.SelectedListItem.ItemId, false);
        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
        dlg.DeInit();
        return;
        //return base.OnMessage(messageType);
      }
      if (choiceSearch[dlg.SelectedLabel] == "randomsearch")
      {
        SearchMoviesbyRandomWithTrailer(false);
        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
        dlg.DeInit();
        return;
        //return base.OnMessage(messageType);
      }
      if (choiceSearch[dlg.SelectedLabel] == "globalareas")
      {
        SearchMoviesbyAreas(false);
        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
        dlg.DeInit();
        return;
        //return base.OnMessage(messageType);
      }
      if (choiceSearch[dlg.SelectedLabel] == "globalproperty")
      {
        SearchMoviesbyProperties(MyFilms.conf.StrSearchList, false);
        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
        dlg.DeInit();
        return;
        //return base.OnMessage(messageType);
      }
      VirtualKeyboard keyboard =
        (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      //if (null == keyboard) return true;
      if (null == keyboard) 
        return;
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
              currentListLevel = Listlevel.Movie;
              conf.StrSelect = conf.StrTitle1.ToString() + " like '*" + keyboard.Text + "*'";
              //conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(369) + " [*" + keyboard.Text + @"*]";
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(369) + " [*" + keyboard.Text + @"*]";
              conf.StrTitleSelect = "";
              GetFilmList();
            }
            else return; // false;
            break;
          case "actors":
            if (control_searchText(keyboard.Text))
            {
              currentListLevel = Listlevel.Person;
              conf.WStrSort = "ACTORS";
              conf.Wselectedlabel = "";
              conf.WStrSortSens = " ASC";
              BtnSrtBy.IsAscending = true;
              conf.StrActors = keyboard.Text;
              getSelectFromDivx(
                "Actors like '*" + keyboard.Text + "*'", conf.WStrSort, conf.WStrSortSens, keyboard.Text, true, "");
            }
            else return; // false;
            break;
          case "search0":
          case "search1":
            int i = 0;
            if (choiceSearch[dlg.SelectedLabel] == "search1") i = 1;
            AntMovieCatalog ds = new AntMovieCatalog();
            if (control_searchText(keyboard.Text))
            {
              if (ds.Movie.Columns[conf.StrSearchItem[i].ToString()].DataType.Name == "string") conf.StrSelect = conf.StrSearchItem[i].ToString() + " like '*" + keyboard.Text + "*'";
              else conf.StrSelect = conf.StrSearchItem[i].ToString() + " = '" + keyboard.Text + "'";
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + conf.StrSearchText[i] + " [*" + keyboard.Text +
                                  @"*]";
              conf.StrTitleSelect = "";
              GetFilmList();
            }
            else return; // false;
            break;
        }
      }
      else
      {
        // GUIControl.FocusControl(GetID, (int)Controls.CTRL_List); // commented, as we do not want to return to facade
        dlg.DeInit();
        this.Change_Search_Options(); // recall search menu
      }
    }

    public static ArrayList Search_String(string champselect)
    {
      Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
      System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
      foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
      {
        Regex oRegexReplace = new Regex("[,;]");
        champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, string.Empty));
      }
      ArrayList wtab = new ArrayList();

      int wi = 0;
      string[] Sep = conf.ListSeparator;
      string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
      string wzone = string.Empty;
      int wzoneIndexPosition = 0;
      for (wi = 0; wi < arSplit.Length; wi++)
      {
        if (arSplit[wi].Length > 0)
        {
          // wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim()); // Replaced for performancereasons - HTML cleanup was not necessary !
          wzone = arSplit[wi].Replace("  ", " ").Trim();
          for (int i = 0; i <= 4; i++)
          {
            if (conf.RoleSeparator[i].Length > 0)
            {
              wzoneIndexPosition = wzone.IndexOf(conf.RoleSeparator[i]);
              if (wzoneIndexPosition == wzone.Length - 1)
              {
                wzone = string.Empty;
                break;
              }
              if (wzoneIndexPosition > 1 && wzoneIndexPosition < wzone.Length)
              {
                wzone = wzone.Substring(0, wzoneIndexPosition).Trim();
              }
            }
          }
          if (wzone.Length > 0)
            wtab.Add(wzone);
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
      string[] Sep = { " - ", ":" }; //Only Dash as Separator for Movietitles !!!
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
      LogMyFilms.Debug("(SubWordGrabbing): InputString: '" + champselect + "'");
      Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
      System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(champselect);
      foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
      {
        Regex oRegexReplace = new Regex("[,;]");
        champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
        LogMyFilms.Debug("(SubWordGrabbing): RegExReplace: '" + champselect + "'");
      }

      string[] CleanerList = new string[] { "Der ", "der ", "Die ", "die ", "Das ", "das", "des", " so", "sich", " a ", " A ", "The ", "the ", "- ", " -", " AT ", "in " };
      int i = 0;
      for (i = 0; i < 13; i++)
      {
        if ((CleanerList[i].Length > 0) && (filter = true))
        {
          champselect = champselect.Replace(CleanerList[i], " ");
          //LogMyFilms.Debug("(SubWordGrabbing): CleanerListItem: '" + CleanerList[i] + "'");
        }
      }

      ArrayList wtab = new ArrayList();

      int wi = 0;
      //string[] Sep = conf.ListSeparator;
      string[] Sep = new string[] { " ", ",", ";", "|", "/", "(", ")", ".", @"\", ":" };
      string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
      string wzone = string.Empty;
      for (wi = 0; wi < arSplit.Length; wi++)
      {
        if (arSplit[wi].Length > 0)
        {
          wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim());
          LogMyFilms.Debug("(SubWordGrabbing): wzone: '" + wzone + "'");
          if (wzone.Length >= minchars)//Only words with minimum 4 letters!
          {
            wtab.Add(wzone.Trim());
            LogMyFilms.Debug("(SubWordGrabbing): AddWordToList: '" + wzone.Trim() + "'");
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
      //GUIListItem item = new GUIListItem();
      Prev_ItemID = -1;
      Prev_Label = string.Empty;
      string champselect = "";
      string wchampselect = "";
      ArrayList w_tableau = new ArrayList();
      List<GUIListItem> facadeDownloadItems = new List<GUIListItem>();
      int Wnb_enr = 0;

      conf.Wstar = NewWstar;
      BtnSrtBy.Label = GUILocalizeStrings.Get(103);
      conf.Boolselect = true;
      conf.Wselectedlabel = "";
      if (ClearIndex)
        conf.StrIndex = 0;
      if (conf.UseListViewForGoups)
        Change_LayOut(0);
      else
        Change_LayOut(MyFilms.conf.StrLayOut);
      facadeView.Clear();
      //GUIControl.ClearControl(GetID, facadeView.GetID); // taken from OV
      int wi = 0;

      string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
      LogMyFilms.Debug("(GetSelectFromDivx) - GlobalFilterString          : '" + GlobalFilterString + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - conf.StrDfltSelect          : '" + conf.StrDfltSelect + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - WstrSelect                  : '" + WstrSelect + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - WStrSort                    : '" + WStrSort + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - WStrSortSens                : '" + WStrSortSens + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - NewWstar                    : '" + NewWstar + "'");

      bool isperson = false;
      if (WStrSort.ToLower().Contains("actors") || WStrSort.ToLower().Contains("producer") || WStrSort.ToLower().Contains("director") || WStrSort.ToLower().Contains("borrower") || WStrSort.ToLower().Contains("writer"))
        isperson = true;
      bool isdate = false;
      if (WStrSort == "Date" || WStrSort == "DateAdded")
        isdate = true;

      // Collect List of all attributes in w_tableau
      LogMyFilms.Debug("(GetSelectFromDivx) - Read movie DB Group Names");
      foreach (DataRow enr in BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, WstrSelect, WStrSort, WStrSortSens))
      {
        if (isdate)
          champselect = string.Format("{0:yyyy/MM/dd}", enr["DateAdded"]);
        else
          champselect = enr[WStrSort].ToString().Trim();
        ArrayList wtab = Search_String(champselect);
        for (wi = 0; wi < wtab.Count; wi++)
        {
          w_tableau.Add(wtab[wi].ToString().Trim());
        }
      }

      LogMyFilms.Debug("(GetSelectFromDivx) - Sort Group Names");
      if (WStrSortSens == " ASC")
        w_tableau.Sort(0, w_tableau.Count, null);
      else
      {
        IComparer myComparer = new myReverserClass();
        w_tableau.Sort(0, w_tableau.Count, myComparer);
      }
      LogMyFilms.Debug("(GetSelectFromDivx) - Sorting Finished");

      if (MyFilms.conf.StrViews || MyFilms.conf.StrPersons) // Check if Thumbs directories exist or create them
      {
        if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups"))
          System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups");
        if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Persons"))
          System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Persons");
      }

      // setting up thumbs directory configuration
      string strThumbDirectory;
      // string[] strActiveFacadeImages; // image pathes for Icon and Thumb -> moved usage to background thread
      if (WStrSort.ToLower().Contains("actors") || WStrSort.ToLower().Contains("producer") || WStrSort.ToLower().Contains("director") || WStrSort.ToLower().Contains("borrower") || WStrSort.ToLower().Contains("writer"))
        strThumbDirectory = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Persons\";
      else
        strThumbDirectory = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups\" + WStrSort.ToLower() + @"\";
      bool getThumbs = false;
      if (MyFilms.conf.StrViews && (MyFilms.conf.StrViewsDfltAll || (WStrSort.ToLower().Contains("category") || WStrSort.ToLower().Contains("year") || WStrSort.ToLower().Contains("country"))))
        getThumbs = true;
      if (MyFilms.conf.StrPersons && isperson)
        getThumbs = true;
      bool createFanartDir = false;
      if (WStrSort.ToLower() == "category" || WStrSort.ToLower() == "year" || WStrSort.ToLower() == "country")
        createFanartDir = true;
      if (!System.IO.Directory.Exists(strThumbDirectory)) // Check groupview thumbs cache directories and create them
        try { System.IO.Directory.CreateDirectory(strThumbDirectory); }
        catch (Exception) { }
      if (!System.IO.Directory.Exists(conf.StrPathViews + @"\" + WStrSort.ToLower())) // Check groupview thumbs (sub)directories and create them
        try { System.IO.Directory.CreateDirectory(conf.StrPathViews + @"\" + WStrSort.ToLower()); }
        catch (Exception) { }

    if (isperson) // Launch Backgroundworker to (off)-load actors artwork and create cache thumbs
      {AsynUpdateActors(w_tableau);}

      LogMyFilms.Debug("(GetSelectFromDivx) - Facadesetup Groups Started");
      //GUIListItem item = null;
      for (wi = 0; wi != w_tableau.Count; wi++)
      {
        champselect = w_tableau[wi].ToString();
        if (string.Compare(champselect, wchampselect, true) == 0) // Are the strings equal? Then add count!
        {
          Wnb_enr++; // count items of distinct property
        }
        else
        {
          if (conf.Wstar == "*" || champselect.ToUpper().Contains(conf.Wstar.ToUpper()))
          {
            if ((Wnb_enr > 0) && (wchampselect.Length > 0))
            {
              GUIListItem item = new GUIListItem(); 
              //item.ItemId = number;
              item.Label = wchampselect;
              item.Label2 = Wnb_enr.ToString();
              //item.Label3 = WStrSort.ToLower();
              //item.DVDLabel = WStrSort.ToLower();
              //MediaPortal.Util.Utils.SetDefaultIcons(item);
              item.Path = WStrSort.ToLower();

              if (isperson) 
                item.TVTag = "person";
              else 
                item.TVTag = "group";
              item.IsFolder = true;
              facadeDownloadItems.Add(item); // offload artwork loading in background thread - first collect items in list!
              item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
              facadeView.Add(item);
              if (SelItem != "" && item.Label == SelItem)
                conf.StrIndex = facadeView.Count - 1; //test if this item is one to select
            }
            Wnb_enr = 1;
            wchampselect = champselect;
          }
        }
      }

      if ((Wnb_enr > 0) && (wchampselect.Length > 0))
      {
        GUIListItem item = new GUIListItem();
        //item.ItemId = number;
        item.Label = wchampselect;
        item.Label2 = Wnb_enr.ToString();
        //item.Label3 = WStrSort.ToLower();
        //item.DVDLabel = WStrSort.ToLower();
        //MediaPortal.Util.Utils.SetDefaultIcons(item);
        item.Path = WStrSort.ToLower();
        //item.ItemId = number; // Only used in GetFilmList

        if (isperson)
          item.TVTag = "person";
        else
          item.TVTag = "group";
        item.IsFolder = true;
        facadeDownloadItems.Add(item); // offload artwork loading in background thread - first collect items in list!
        item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        facadeView.Add(item);
        if (SelItem != "" && item.Label == SelItem)
          conf.StrIndex = facadeView.Count - 1; //test if this item is one to select
        Wnb_enr = 0;
      }
      LogMyFilms.Debug("(GetSelectFromDivx) - Facadesetup Groups Finished");

      //item.FreeMemory();
      conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
      if (conf.Wstar != "*") conf.StrTxtSelect += " " + GUILocalizeStrings.Get(1079896) + " [*" + conf.Wstar + "*]"; // add to "Selection": Persons with Filter
      MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
      //TxtSelect.Label = conf.StrTxtSelect;
      conf.StrSelect = WstrSelect;
      conf.StrFilmSelect = string.Empty;

      if ((conf.StrIndex > facadeView.Count - 1) || (conf.StrIndex < 0)) //check index within bounds, will be unless xml file heavily edited
        conf.StrIndex = 0;

      // Call this async after facade is loaded - WIP
      //if (getThumbs) this.LoadFacadeImages(WStrSort, strThumbDirectory, isperson);
      if (getThumbs)
      {
        // load first image syncronously, as asyncloading might cause flicker or even let it disappear
        string[] strActiveFacadeImages = SetViewThumbs(WStrSort, facadeView[conf.StrIndex].Label, strThumbDirectory, isperson);
        string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";
        facadeView[conf.StrIndex].ThumbnailImage = strActiveFacadeImages[0].ToString();
        facadeView[conf.StrIndex].IconImage = strActiveFacadeImages[1].ToString();
        facadeView[conf.StrIndex].IconImageBig = strActiveFacadeImages[0].ToString();

        // load the rest of images asynchronously!
        this.GetImages(facadeDownloadItems, WStrSort, strThumbDirectory, isperson, getThumbs, createFanartDir);
      }
      
      if (facadeView.Count == 0)
      {
        ShowMessageDialog(GUILocalizeStrings.Get(10798624), GUILocalizeStrings.Get(10798637), GUILocalizeStrings.Get(10798638)); //"no movies matching the view" - " show filmlist"
        DisplayAllMovies();
        GetFilmList();
        GUIControl.ShowControl(GetID, 34);
        SetLabelSelect("root");
        SetLabelView("all");
      }
      else
      {
        Fanartstatus(true);
        GUIControl.ShowControl(GetID, 34);

        if (isperson) //Make a difference between movies and persons -> Load_Detailed_DB or Load_Detailed_PersonInfo
          MyFilmsDetail.Load_Detailed_PersonInfo(facadeView.SelectedListItem.Label, false);
        // else
        //   MyFilmsDetail.Load_Detailed_DB(0, false);

        // Disabled because replaced by SpeedLoader
        // ImgLstFilm.SetFileName("#myfilms.picture");
        // ImgLstGroup.SetFileName("#myfilms.picture");
        // this.Load_Rating(0); // old method - nor more used
      }
      MyFilmsDetail.setGUIProperty("nbobjects.value", facadeView.Count.ToString());
      GUIPropertyManager.SetProperty("#itemcount", facadeView.Count.ToString());

      //MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
      GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, (int)conf.StrIndex);
    }

    private void GetImages(List<GUIListItem> itemsWithThumbs, string WStrSort, string strThumbDirectory, bool isperson, bool getThumbs, bool createFanartDir)
    {
      StopDownload = false;

      // split the downloads in X+ groups and do multithreaded downloading
      int groupSize = (int)Math.Max(1, Math.Floor((double)itemsWithThumbs.Count / 2)); // Guzzi: Set group to x to only allow x thread(s)
      int groups = (int)Math.Ceiling((double)itemsWithThumbs.Count / groupSize);

      for (int i = 0; i < groups; i++)
      {
        List<GUIListItem> groupList = new List<GUIListItem>();
        for (int j = groupSize * i; j < groupSize * i + (groupSize * (i + 1) > itemsWithThumbs.Count ? itemsWithThumbs.Count - groupSize * i : groupSize); j++)
        {
          groupList.Add(itemsWithThumbs[j]);
        }

        new System.Threading.Thread(delegate(object o)
        {
          List<GUIListItem> items = (List<GUIListItem>)o;
          foreach (GUIListItem item in items)
          {
            // stop download if we have exited window
            if (StopDownload) break;

            if (getThumbs)
            {
              string[] strActiveFacadeImages = SetViewThumbs(WStrSort, item.Label, strThumbDirectory, isperson);
              string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";

              //if (GUITextureManager.LoadFromMemory(ImageFast.FastFromFile(strActiveFacadeImages[0]), texture, 0, 0, 0) > 0)
              //{
              //  item.ThumbnailImage = texture;
              //  item.IconImage = texture;
              //  item.IconImageBig = texture;
              //}
              
              item.ThumbnailImage = strActiveFacadeImages[0].ToString();
              item.IconImage = strActiveFacadeImages[1].ToString();
              item.IconImageBig = strActiveFacadeImages[0].ToString();

              // if selected force an update of thumbnail
              //GUIListItem selectedItem = GUIControl.GetSelectedListItem(ID_MyFilms, 50);
              //if (selectedItem == this)
              //{
              //  GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, ItemId, 0, null));
              //}
            }
            if (createFanartDir)
            {
              string[] wfanart;
              wfanart = MyFilmsDetail.Search_Fanart(item.Label, true, "file", true, item.ThumbnailImage, WStrSort.ToLower());
            }

            // ToDo: Add downloader to SetViewThumbs - or here ...

            //string remoteThumb = item.ImageRemotePath;
            //if (string.IsNullOrEmpty(remoteThumb)) continue;

            //string localThumb = item.Image;
            //if (string.IsNullOrEmpty(localThumb)) continue;

            //if (Helper.DownloadFile(remoteThumb, localThumb))
            //{
            //  // notify that thumbnail image has been downloaded
            //  item.ThumbnailImage = localThumb;
            //  item.NotifyPropertyChanged("ThumbnailImage");
            //}
          }
        })
        {
          IsBackground = true,
          Name = "MyFilms Image Detector and Downloader" + i.ToString()
        }.Start(groupList);
      }
    }

    private void LoadFacadeImages(string WStrSort, string strThumbDirectory, bool isperson)
    {
      for(int i = 0; i < facadeView.Count; i++)
      {
        GUIListItem item = facadeView[i];
        string[] strActiveFacadeImages = SetViewThumbs(WStrSort, item.Label, strThumbDirectory, isperson);
        item.ThumbnailImage = strActiveFacadeImages[0].ToString();
        item.IconImage = strActiveFacadeImages[1].ToString();
        item.IconImageBig = strActiveFacadeImages[0].ToString();
      }
    }

    private string[] SetViewThumbs(string WStrSort, string itemlabel, string strThumbDirectory, bool isPerson)
    {
      string[] thumbimages = new string[2];
      thumbimages[0] = string.Empty; // ThumbnailImage
      thumbimages[1] = string.Empty; //IconImage
      string strThumb = strThumbDirectory + itemlabel + ".png";
      //string strThumbLarge = string.Empty;
      string strThumbSource = string.Empty;

      if (isPerson)
      {
        //strThumbSource = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieActors, itemlabel); // check for actors images in MyVideos...
        //LogMyFilms.Debug("Artist thumbs - GetCoverName(Thumbs, MovieActors) - strThumb = '" + strThumb + "'");
        //if (!System.IO.File.Exists(strThumb))
        //  strThumb = strThumbDirectory + itemlabel + ".png";
        //else
        //  return strThumb;

        if (System.IO.File.Exists(strThumb)) // If there is missing thumbs in cache folder ...
        {
          thumbimages[0] = strThumb;
          thumbimages[1] = strThumbDirectory + itemlabel + "_s.png";
          return thumbimages;
        }
        if (conf.StrPathArtist.Length > 0)
        {
          string strPathArtist = String.Empty;
          if (conf.StrPathArtist.Substring(conf.StrPathArtist.Length - 1) == "\\") strPathArtist = conf.StrPathArtist;
          else strPathArtist = conf.StrPathArtist + "\\";
          if (System.IO.File.Exists(strPathArtist + itemlabel + "\\folder.jpg")) strThumbSource = strPathArtist + itemlabel + "\\folder.jpg";
          else if (System.IO.File.Exists(strPathArtist + itemlabel + "\\folder.png")) strThumbSource = strPathArtist + itemlabel + "\\folder.png";
          else if (System.IO.File.Exists(strPathArtist + itemlabel + "L" + ".jpg")) strThumbSource = strPathArtist + itemlabel + "L" + ".jpg";
          else if (System.IO.File.Exists(strPathArtist + itemlabel + ".jpg")) strThumbSource = strPathArtist + itemlabel + ".jpg";
          else if (System.IO.File.Exists(strPathArtist + itemlabel + ".png")) strThumbSource = strPathArtist + itemlabel + ".png";
          else
            switch (conf.StrFileType) // perform catalog specific searches ...
            {
              case "0": // ANT Movie Catalog
                break;
              case "10":// Starter Settings ANT extnded DB
                break;
              case "1": // DVD Profiler
                break;
              case "2": // Movie Collector V7.1.4
                break;
              case "3": // MyMovies
                break;
              case "4": // EAX MC 2.5.0
              case "5": // EAX 3.x
                if (System.IO.File.Exists(strPathArtist + itemlabel.Replace(" ", ".") + ".jpg")) strThumbSource = strPathArtist + itemlabel.Replace(" ", ".") + ".jpg";
                break;
              case "6": // PVD artist thumbs: e.g. Natalie Portman_1.jpg , then Natalie Portman_2.jpg 
                if (!string.IsNullOrEmpty(conf.StrPathArtist)) //Search matching files in PVD picture directory
                {
                  string searchname = HTMLParser.removeHtml(itemlabel); // replaces special character "á" and other special chars !
                  searchname = searchname + "*.jpg";
                  string[] files = Directory.GetFiles(conf.StrPathArtist, searchname, SearchOption.TopDirectoryOnly);
                  if (files.Count() > 0)
                    strThumbSource = files[0];
                }
                break;
              case "7": // XMM artist thumbs: e.g. Alex-Revan_101640.jpg
                if (!string.IsNullOrEmpty(conf.StrPathArtist)) //Search matching files in XMM picture directory
                {
                  string searchname = HTMLParser.removeHtml(itemlabel); // replaces special character "á" and other special chars !
                  searchname = searchname.Replace(" ", "-");
                  searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
                  string[] files = Directory.GetFiles(conf.StrPathArtist, searchname, SearchOption.TopDirectoryOnly);
                  if (files.Count() > 0)
                    strThumbSource = files[0];
                }
                break;
              case "8": // XBMC fulldb export (all movies in one DB)
                break;
              case "9": // MovingPicturesXML V1.2
                break;
              case "11": // XBMC Nfo (separate nfo files, to scan dirs - MovingPictures or XBMC)
                break;
              default:
                break;
            }
          if (strThumbSource != string.Empty)
          {
            //Picture.CreateThumbnail(strThumbSource, strThumbDirectory + itemlabel + "_s.png", 100, 150, 0, Thumbs.SpeedThumbsSmall);
            createCacheThumb(strThumbSource, strThumbDirectory + itemlabel + "_s.png", 100, 150, "small");
            //Picture.CreateThumbnail(strThumbSource, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
            createCacheThumb(strThumbSource, strThumb, cacheThumbWith, cacheThumbHeight, "large");
            //thumbimages[0] = strThumbSource;
            //thumbimages[1] = strThumbDirectory + itemlabel + "_s.png";
            //return thumbimages;

            if (System.IO.File.Exists(strThumb)) // (re)check if thumbs exist...
            {
              thumbimages[0] = strThumb;
              thumbimages[1] = strThumbDirectory + itemlabel + "_s.png";
              return thumbimages;
            }
          }
        }

        //if ((!System.IO.File.Exists(strThumbLarge)) && (strThumbLarge != conf.DefaultCoverArtist) && (strThumbSource != string.Empty))
        //{
        //  Picture.CreateThumbnail(strThumbSource, strThumbDirectory + itemlabel + "L.png", cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
        //  strThumbLarge = strThumbDirectory + itemlabel + "L.png";
        //}

        // if (!System.IO.File.Exists(strThumb) && conf.StrArtistDflt && conf.DefaultCoverArtist.Length > 0)
        if (conf.StrArtistDflt && conf.DefaultCoverArtist.Length > 0)
        {
          //ImageFast.CreateImage(strThumb, item.Label); // this is to create a pseudo cover with name of label added to it
          //Picture.CreateThumbnail(conf.DefaultCoverArtist, strThumbDirectory + itemlabel + "_s.png", 100, 150, 0, Thumbs.SpeedThumbsSmall);
          //Picture.CreateThumbnail(conf.DefaultCoverArtist, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
          thumbimages[0] = conf.DefaultCoverArtist;
          thumbimages[1] = conf.DefaultCoverArtist;
          return thumbimages;
        }
        else
        {
          thumbimages[0] = "";
          thumbimages[1] = "";
          return thumbimages;
        }

      }
      else if (MyFilms.conf.StrViewsDfltAll || (WStrSort.ToLower().Contains("country") || WStrSort.ToLower().Contains("category") || WStrSort.ToLower().Contains("year")))
      {
        if (System.IO.File.Exists(strThumb)) // If there is thumbs in cache folder ...
        {
          thumbimages[0] = strThumb;
          thumbimages[1] = strThumb;
          return thumbimages;
        }

        if (conf.StrPathViews.Length > 0)
        {
          string strPathViews = String.Empty;
          if (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\")
            strPathViews = conf.StrPathViews;
          else
            strPathViews = conf.StrPathViews + "\\";
          strPathViews = strPathViews + WStrSort.ToLower() + "\\"; // added view subfolder to searchpath
          if (System.IO.File.Exists(strPathViews + itemlabel + ".jpg"))
            createCacheThumb(strPathViews + itemlabel + ".jpg", strThumb, cacheThumbWith, cacheThumbHeight, "large");
          else if (System.IO.File.Exists(strPathViews + itemlabel + ".png"))
            createCacheThumb(strPathViews + itemlabel + ".png", strThumb, cacheThumbWith, cacheThumbHeight, "large");
          if (System.IO.File.Exists(strThumb))
          {
            thumbimages[0] = strThumb;
            thumbimages[1] = strThumb;
            return thumbimages;
          }
          // Check, if default group cover is present
          if (MyFilms.conf.StrViewsDflt)
          {
            if (System.IO.File.Exists(strPathViews + "Default.jpg"))
            {
              thumbimages[0] = strPathViews + "Default.jpg";
              thumbimages[1] = strPathViews + "Default.jpg";
              return thumbimages;
            }
          }
        }

        // Use Default Cover if no specific Cover found:
        //  if (MyFilms.conf.StrViewsDflt && System.IO.File.Exists(MyFilms.conf.DefaultCover))
        if (MyFilms.conf.StrViewsDflt && (MyFilms.conf.DefaultCoverViews.Length > 0))
        {
          //ImageFast.CreateImage(strThumb, item.Label); // Disabled "old" method to use Defaultcover with embedded text of selected item ...
          //Picture.CreateThumbnail(strThumbLarge, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
          thumbimages[0] = conf.DefaultCoverViews;
          thumbimages[1] = conf.DefaultCoverViews;
          return thumbimages;
        }
      }
      return thumbimages;
    }

    private static void createCacheThumb(string ThumbSource, string ThumbTarget, int ThumbWidth, int ThumbHeight, string SpeedThumbSize)
    {
      System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(ThumbSource);
      LogMyFilms.Debug("(SetViewThumb) - Image Width x Height = '" + bmp.Width + "x" + bmp.Height + "' (" + ThumbSource + ")");
      if (bmp.Width < ThumbWidth && bmp.Height < ThumbHeight)
      {
        if (!SaveThumbnailFile(ThumbSource, ThumbTarget)) // if copy unsuccessful, try to create speedthumb
        {
          if (SpeedThumbSize == "small")
            Picture.CreateThumbnail(ThumbSource, ThumbTarget, ThumbWidth, ThumbHeight, 0, Thumbs.SpeedThumbsSmall);
          else
            Picture.CreateThumbnail(ThumbSource, ThumbTarget, ThumbWidth, ThumbHeight, 0, Thumbs.SpeedThumbsLarge);
        }
      }
      else
        if (SpeedThumbSize == "small")
          Picture.CreateThumbnail(ThumbSource, ThumbTarget, ThumbWidth, ThumbHeight, 0, Thumbs.SpeedThumbsSmall);
        else
          Picture.CreateThumbnail(ThumbSource, ThumbTarget, ThumbWidth, ThumbHeight, 0, Thumbs.SpeedThumbsLarge);
      if (bmp != null)
      {
        bmp.SafeDispose();
      }
    }

    private static bool SaveThumbnailFile(string ThumbSource, string ThumbTarget)
    {
      try
      {
        File.Copy(ThumbSource, ThumbTarget, true);

        //FileStream fs = new FileStream(ThumbSource, FileMode.Open, FileAccess.Read, FileShare.Read);	
        //BinaryReader reader = new BinaryReader(fs);
        //byte[] bytes = new byte[fs.Length];
        //int read;
        //reader.Read(bytes, 0, bytes.Length)	
        //reader.Close(); 
        //fs.Close();


        //using (FileStream fs = new FileStream(ThumbTarget, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        //{
        //  using (FileStream fs = new FileStream(ThumbTarget, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        //  {
        //    bmp.Save(fs, Thumbs.ThumbCodecInfo, Thumbs.ThumbEncoderParams);
        //  }
        //  fs.Flush();
        //}



        File.SetAttributes(ThumbTarget, File.GetAttributes(ThumbTarget) | FileAttributes.Hidden);
        // even if run in background thread wait a little so the main process does not starve on IO
        if (MediaPortal.Player.g_Player.Playing)
          Thread.Sleep(100);
        else
          Thread.Sleep(1);
        return true;
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("(SaveThumbnailFile) - Error saving new thumbnail {0} - {1}", ThumbTarget, ex.Message);
        return false;
      }
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

    private static void Load_Config(string CurrentConfig, bool create_temp, LoadParameterInfo loadParams)
    {
      
      conf = new Configuration(CurrentConfig, create_temp, loadParams);
      if ((conf.Boolreturn) && (conf.Wselectedlabel == string.Empty))
      {
        conf.Boolselect = true;
        conf.Boolreturn = false;
      }
      if (conf.StrLogos)
        confLogos = new Logos();
      MyFilmsDetail.setGUIProperty("config.currentconfig", CurrentConfig);

      if (conf.StrDfltSelect.Length > 0)
      {
        string userfilter = conf.StrDfltSelect.Substring(0, conf.StrDfltSelect.LastIndexOf("AND")).Trim();
        MyFilmsDetail.setGUIProperty("config.configfilter", userfilter);
        LogMyFilms.Debug("userfilter from setup: StrDfltSelect = '" + userfilter + "'");
      }
      else
        MyFilmsDetail.clearGUIProperty("config.configfilter");
    }

    private void Refreshfacade()
    {
      Thread LoadThread = new Thread(new ThreadStart(Worker_Refreshfacade));
      LoadThread.IsBackground = true;
      LoadThread.Priority = ThreadPriority.AboveNormal;
      LoadThread.Name = "MyFilms Fin_Charge_Init (false-true)";
      LoadThread.Start();
      //LoadThread.Join(); // wait until background thread finished ...
    }

    private void Worker_Refreshfacade()
    {
      GUIWaitCursor.Init();
      GUIWaitCursor.Show();
      Fin_Charge_Init(false, true);
      GUIWaitCursor.Hide();
    }

    private void Loadfacade()
    {
      Thread LoadThread = new Thread(new ThreadStart(Worker_Refreshfacade));
      LoadThread.IsBackground = true;
      LoadThread.Priority = ThreadPriority.AboveNormal;
      LoadThread.Name = "MyFilms Fin_Charge_Init (true-true)";
      LoadThread.Start();
      //LoadThread.Join(); // wait until background thread finished ...
    }

    private void Worker_Loadfacade()
    {
      GUIWaitCursor.Init();
      GUIWaitCursor.Show();
      Fin_Charge_Init(true, true);
      GUIWaitCursor.Hide();
    }

    //--------------------------------------------------------------------------------------------
    //  Initial Windows load. If LoadDfltSlct = true => load default select if any
    //                           LoadDfltSlct = false => return from  MyFilmsDetail
    //--------------------------------------------------------------------------------------------
    private void Fin_Charge_Init(bool LoadDfltSlct, bool reload)
    {
      LogMyFilms.Debug("Fin_Charge_Init() called with LoadDfltSlct = '" + LoadDfltSlct + "', reload = '" + reload + "'");
      //if (publishTimer != null)
      //  publishTimer.SafeDispose();
      if (LoadDfltSlct)
      {
        conf.Boolselect = false; // Groupviews = false
        ResetGlobalFilters();
        MovieScrobbling = false; // reset scrobbler filter setting
      }
      if (((PreviousWindowId != ID_MyFilmsDetail) && (PreviousWindowId != ID_MyFilmsActors)) || (reload))
      {
        if (reload) //chargement des films
        {
          BaseMesFilms.LoadMyFilms(conf.StrFileXml); // Will be automatically loaded, if not yet done - save time on reentering MyFilms GUI !!!
          MyFilmsDetail.SetGlobalLock(false, MyFilms.conf.StrFileXml); // release global lock, if there is any, after initializing (this is cleanup for older leftovers)
        }
        r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens);
      }

      //Layout = conf.StrLayOut;

      //if (MyFilms.conf.GlobalUnwatchedOnly)
      //  BtnToggleGlobalWatched.Label = string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798628));
      //else
      //  BtnToggleGlobalWatched.Label = string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798629));
      if (string.IsNullOrEmpty(conf.CurrentSortMethod))
        conf.CurrentSortMethod = GUILocalizeStrings.Get(103);
      else
        BtnSrtBy.Label = conf.CurrentSortMethod;
      string BtnSearchT = GUILocalizeStrings.Get(137);

      GUIButtonControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnSearchT, BtnSearchT);
      BtnSrtBy.SortChanged += new SortEventHandler(SortChanged);
      InitialStart = false; // Guzzi: Set to false after first initialization to be able to return to noninitialized View - Make sure to set true if changing DB config

      if (conf.Boolselect) // Groupviews ?
      {
        // Hack to get persons in ASC order after returning from external plugins ...
        if (conf.WStrSort.ToUpper() == "ACTORS" || conf.WStrSort.ToUpper() == "PRODUCER" || conf.WStrSort.ToUpper() == "DIRECTOR" || conf.WStrSort.ToUpper() == "WRITER")
        {
          currentListLevel = Listlevel.Person;
          getSelectFromDivx(conf.StrSelect, conf.WStrSort, " ASC", conf.Wstar, false, ""); // preserve index from last time
        }
        else
        {
          currentListLevel = Listlevel.Group;
          getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.StrSortSens, conf.Wstar, false, ""); // preserve index from last time
        }
        LogMyFilms.Debug("(Fin_Charge_Init) - Boolselect = true -> StrTxtSelect = '" + MyFilms.conf.StrTxtSelect + "', StrTxtView = '" + MyFilms.conf.StrTxtView + "'");
        SetLabelView(MyFilms.conf.StrTxtView); // Reload view name from configfile...
      }
      else
      {
        Change_LayOut(MyFilms.conf.StrLayOut);
        currentListLevel = Listlevel.Movie; // set as default, if no group view
        if (!(LoadDfltSlct))                       // Defaultview not selected
        {
          GetFilmList(conf.StrIndex);
          SetLabelView(MyFilms.conf.StrTxtView); // Reload view name from configfile...
        }
        else                                        // Defaultview selected
        {
          if (string.IsNullOrEmpty(conf.StrViewDfltItem) || conf.StrViewDfltItem == "(none)" || conf.StrViewDfltItem == GUILocalizeStrings.Get(342)) // no Defaultitem defined for defaultview or "films" -> normal movielist
          {
            conf.StrSelect = conf.StrTitle1 + " not like ''"; // was: TxtSelect.Label = conf.StrTxtSelect = "";
            conf.Boolselect = false;
            if (conf.StrSortSens == " ASC")
              BtnSrtBy.IsAscending = true;
            else
              BtnSrtBy.IsAscending = false;
            GetFilmList(conf.StrIndex);
            SetLabelView("all");
            SetLabelSelect("root");
          }
          else
          {
            if (string.IsNullOrEmpty(conf.StrViewDfltText)) // no filteritem defined for the defaultview
            {
              if (conf.StrViewDfltItem.ToLower() == "year" || conf.StrViewDfltItem.ToLower() == "category" || conf.StrViewDfltItem.ToLower() == "country" || conf.StrViewDfltItem.ToLower() == "storage" || conf.StrViewDfltItem.ToLower() == "actors")
                Change_view(conf.StrViewDfltItem.ToLower());
              else
              {
                for (int i = 0; i < 5; i++)
                {
                  if (conf.StrViewDfltItem == conf.StrViewText[i])
                    Change_view(string.Format("View{0}", i).ToLower());
                }
              }
              // SetLabelView and SetLabelSelect will be set by "Change_view"
            }
            else
            // View List as selected - filteritem IS defined for the defaultview
            {
              string wStrViewDfltItem = conf.StrViewDfltItem.ToLower();
              for (int i = 0; i < 5; i++)
              {
                if (conf.StrViewDfltItem == conf.StrViewText[i])
                {
                  wStrViewDfltItem = conf.StrViewItem[i];
                  SetLabelView("view" + i.ToString());
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
              SetLabelSelect(conf.StrTxtSelect);

              if (wStrViewDfltItem.Length > 0)
                SetLabelView(wStrViewDfltItem); // replaces st with localized set - old: MyFilmsDetail.setGUIProperty("view", conf.StrViewDfltItem); // set default view config to #myfilms.view
              MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
              BtnSrtBy.Label = conf.CurrentSortMethod;
              // BtnSrtBy.Label = conf.WStrSort; // ToDo: Check, why initial value not set properly
              if (conf.StrSortSens == " ASC")
                BtnSrtBy.IsAscending = true;
              else
                BtnSrtBy.IsAscending = false;
              GetFilmList(conf.StrIndex);
            }
          }
        }
      }
      LogMyFilms.Debug("Fin_Charge_Init: StrSelect = '" + conf.StrSelect + "', StrTxtSelect = '" + conf.StrTxtSelect + "'");
      if (string.IsNullOrEmpty(conf.StrTxtSelect) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798622)) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798632))) // empty or starts with "all" or "filtered" ... 
        SetLabelSelect("root");
      else
        SetLabelSelect(conf.StrTxtSelect);
      SetDummyControlsForFacade(currentListLevel);
      if (conf.LastID == ID_MyFilmsDetail)
        GUIWindowManager.ActivateWindow(ID_MyFilmsDetail); // if last window in use was detailed one display that one again
      else if (conf.LastID == ID_MyFilmsActors)
        GUIWindowManager.ActivateWindow(ID_MyFilmsActors); // if last window in use was actor one display that one again
      else GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
    }

    private void ResetGlobalFilters()
    {
      //Reset Global Filters !
      GlobalFilterTrailersOnly = false;
      GlobalFilterStringTrailersOnly = String.Empty;
      MyFilmsDetail.clearGUIProperty("globalfilter.trailersonly");

      GlobalFilterMinRating = false;
      GlobalFilterStringMinRating = String.Empty;
      MyFilmsDetail.clearGUIProperty("globalfilter.minrating");
      MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue");

      if (conf.GlobalUnwatchedOnly) // Reset GlobalUnwatchedFilter to the setup default (can be changed via GUI menu)
      {
        // GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
        if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
          GlobalFilterStringUnwatched = conf.StrWatchedField + " like '*" + conf.StrUserProfileName + ":0*" + "' AND ";
        else
          GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
        MyFilmsDetail.setGUIProperty("globalfilter.unwatched", "true");
      }
      else
      {
        GlobalFilterStringUnwatched = String.Empty;
        MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
      }

      if (conf.GlobalAvailableOnly)
        GlobalFilterIsOnlineOnly = true;
      else
        GlobalFilterIsOnlineOnly = false;

      if (GlobalFilterIsOnlineOnly)
      {
        GlobalFilterStringIsOnline = "IsOnline like 'true' AND ";
        MyFilmsDetail.setGUIProperty("globalfilter.isonline", "true");
      }
      else
      {
        GlobalFilterStringIsOnline = String.Empty;
        MyFilmsDetail.clearGUIProperty("globalfilter.isonline");
      }
    }

    //--------------------------------------------------------------------------------------------
    //   Change LayOut 
    //--------------------------------------------------------------------------------------------
    private void Change_LayOut(int wLayOut)
    {

#if MP11
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
                default:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(101));
                    facadeView.View = GUIFacadeControl.ViewMode.List;
                    break;
            }
#else
      switch (wLayOut)
      {
        case 1:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(100));
          facadeView.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
          break;
        case 2:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(417));
          facadeView.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
          break;
        case 3:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(733));
          facadeView.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
          break;
        case 4:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(791));
          facadeView.CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
          //if (facadeView.CurrentLayout == GUIFacadeControl.Layout.CoverFlow)
          //  facadeView.CoverFlowLayout.SelectCard(facadeView.SelectedListItemIndex); // added to reduce broken initial animation
          break;

        default:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(101));
          facadeView.CurrentLayout = GUIFacadeControl.Layout.List;
          break;
      }
#endif
    }
    //--------------------------------------------------------------------------------------------
    //   Change View Response  (and process corresponding filter list)
    //--------------------------------------------------------------------------------------------
    private void Change_view(string choiceView)
    {
      LogMyFilms.Debug("Change_View called with '" + choiceView + "'");
      XmlConfig XmlConfig = new XmlConfig();
      conf.Boolview = false;
      conf.Boolstorage = false;
      switch (choiceView)
      {
        case "all":
          //  Change View All Films
          currentListLevel = Listlevel.Movie;
          conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
          conf.WStrSort = conf.StrSTitle;
          conf.Boolselect = false;
          conf.Boolreturn = false;
          LogMyFilms.Debug("Change_View filter - " + "StrSelect: " + conf.StrSelect + " | WStrSort: " + conf.WStrSort);
          GetFilmList();
          SetLabelView("all");
          this.SetLabelSelect("root");
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          break;
        case "year":
          //  Change View by Year
          currentListLevel = Listlevel.Group;
          conf.WStrSort = "YEAR";
          conf.WStrSortSens = " DESC";
          BtnSrtBy.IsAscending = false;
          getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, "");
          SetLabelView("year");
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          break;
        case "category":
          //  Change View by  "Category":
          currentListLevel = Listlevel.Group;
          conf.WStrSort = "CATEGORY";
          conf.WStrSortSens = " ASC";
          BtnSrtBy.IsAscending = true;
          getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, "");
          SetLabelView("category");
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          break;
        case "country":
          //  Change View by "Country":
          currentListLevel = Listlevel.Group;
          conf.WStrSort = "COUNTRY";
          conf.WStrSortSens = " ASC";
          BtnSrtBy.IsAscending = true;
          getSelectFromDivx(conf.StrTitle1.ToString() + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, "");
          SetLabelView("country");
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          break;

        case "actors":
          //  Change View by "Actors":
          currentListLevel = Listlevel.Person;
          conf.WStrSort = "ACTORS";
          conf.WStrSortSens = " ASC";
          BtnSrtBy.IsAscending = true;
          getSelectFromDivx(conf.StrTitle1 + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, string.Empty);
          SetLabelView("actors");
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          break;

        case "storage":
          //  Change View by "Storage":
          currentListLevel = Listlevel.Movie;
          conf.StrSelect = "((" + conf.StrTitle1.ToString() + " not like '') and (" + conf.StrStorage.ToString() + " not like ''))";
          conf.StrTxtSelect = GUILocalizeStrings.Get(10798736);
          //                    TxtSelect.Label = conf.StrTxtSelect;
          MyFilmsDetail.clearGUIProperty("select");
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
          SetLabelView("storage");
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

          if (conf.StrViewItem[i] != "")

            switch (conf.StrViewItem[i].ToLower())
            {
              case "producer":
              case "director":
              case "writer":
              case "actor":
              case "borrower":
                currentListLevel = Listlevel.Person;
                break;
              case "originaltitle":
              case "translatedtitle":
              case "formattedtitle":
                currentListLevel = Listlevel.Movie;
                break;
              default:
                currentListLevel = Listlevel.Group;
                break;
            }
          conf.WStrSort = conf.StrViewItem[i];
          SetLabelView(choiceView);
          conf.WStrSortSens = " ASC";
          BtnSrtBy.IsAscending = true;
          if (conf.StrViewValue[i].Length > 0)
          {
            conf.Boolview = true;
            conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
            conf.Boolselect = true;
            conf.Wstar = "*";
            if (conf.Wstar != "*")
              conf.StrTxtSelect += " " + GUILocalizeStrings.Get(344) + " [*" + conf.Wstar + "*]";
            //                        TxtSelect.Label = conf.StrTxtSelect;
            MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
            if (conf.WStrSort == "DateAdded")
              conf.StrSelect = "Date";
            else
              conf.StrSelect = conf.WStrSort;

            conf.StrFilmSelect = string.Empty;
            conf.Wselectedlabel = conf.StrViewValue[i];
            conf.Boolreturn = true;
            do
            {
              if (conf.StrTitleSelect != string.Empty)
                conf.StrTitleSelect += conf.TitleDelim;
              conf.StrTitleSelect += conf.StrViewValue[i];
            } while (GetFilmList() == false); //keep calling while single folders found
          }
          else
          {
            if (conf.WStrSort == "DateAdded")
              getSelectFromDivx(conf.StrTitle1 + " not like ''", "Date", " DESC", "*", true, string.Empty);
            else
              getSelectFromDivx(conf.StrTitle1 + " not like ''", conf.WStrSort, conf.WStrSortSens, "*", true, string.Empty);
          }

          //if ((conf.StrViewText[i] == null) || (conf.StrViewText[i].Length == 0))
          //{
          //    MyFilmsDetail.setGUIProperty("view", conf.StrViewItem[i]);   // specific user View1
          //    GUIPropertyManager.SetProperty("#currentmodule", conf.StrViewItem[i]);
          //}
          //else
          //{
          //    MyFilmsDetail.setGUIProperty("view", conf.StrViewText[i]);   // specific Text for View1
          //    GUIPropertyManager.SetProperty("#currentmodule", conf.StrViewText[i]);
          //}
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          break;

        case "config": //Choose Database
          GUIControl.ShowControl(GetID, 34); // hide elements in skin
          string newConfig = Configuration.Choice_Config(GetID);
          newConfig = Configuration.Control_Access_Config(newConfig, GetID);
          if (newConfig != string.Empty) // if user escapes dialog or bad value leave system unchanged
          {
            InitMainScreen(false); // reset all properties and values
            InitGlobalFilters(false); // reset global filters, when loading new config !
            //Change "Config":
            if (facadeView.SelectedListItem != null)
              Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
            else
              Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, string.Empty);
            Configuration.CurrentConfig = newConfig;
            InitialIsOnlineScan = false; // set false, so facade does not display false media status !!!
            InitialStart = true; //Set to true to make sure initial View is initialized for new DB view
            //GUIWaitCursor.Init();
            //GUIWaitCursor.Show();
            //GUIWindowManager.Process();
            //MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
            Load_Config(newConfig, true, null);
            if (InitialStart)
              Fin_Charge_Init(true, true); //Guzzi: need to always load default view on initial start, even if always default view is disabled ...
              //Loadfacade(); // load facade threaded...
            else
            {
              Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
              //if (conf.AlwaysDefaultView)
              //  Loadfacade(); // load threaded Fin_Charge_Init(true, true)
              //else
              //  Refreshfacade(); // load threaded Fin_Charge_Init(false, true)
            }

            // launch DB watcher for multiseat
            InitFSwatcher();

            // Launch Background availability scanner, if configured in setup
            if (MyFilms.conf.ScanMediaOnStart && InitialStart)
            {
              LogMyFilms.Debug("Launching Availabilityscanner - Initialstart = '" + InitialStart.ToString() + "'");
              this.AsynIsOnlineCheck();
            }
            InitialStart = false; // Guzzi: Set InitialStart to false after initialization done

            if (MyFilms.conf.StrFanart)
              Fanartstatus(true);
            else
              Fanartstatus(false);
          }
          else
            GUIControl.HideControl(GetID, 34); // show elements in skin
          //MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
          //GUIWaitCursor.Hide();
          break;

        case "nasstatus": //Check and show status of NAS Servers

          //First check status of configured NAS-Servers
          WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();
          int intTimeOut = conf.StrWOLtimeout; //Timeout für WOL

          //GUIWindowManager.Process(); //Added by hint of Damien to update GUI first ...

          GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          //if (dlg == null) return;
          //dlg.Reset();
          System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
          dlg.SetHeading(GUILocalizeStrings.Get(10798727)); // show nas server status
          dlg.Add(GUILocalizeStrings.Get(712)); //Return
          choiceSearch.Add("BACK");

          if (MyFilms.conf.StrNasName1.Length > 0)
          {
            if (WakeOnLanManager.Ping(MyFilms.conf.StrNasName1, intTimeOut))
              dlg.Add("'" + MyFilms.conf.StrNasName1 + "' - " + GUILocalizeStrings.Get(10798741)); // srv name + " - (active)" 
            else
              dlg.Add("'" + MyFilms.conf.StrNasName1 + "' - " + GUILocalizeStrings.Get(10798742)); // srv name + " - (offline) - start ?"
            choiceSearch.Add("NAS1");
          }

          if (MyFilms.conf.StrNasName2.Length > 0)
          {
            if (WakeOnLanManager.Ping(MyFilms.conf.StrNasName2, intTimeOut))
              dlg.Add("'" + MyFilms.conf.StrNasName2 + "' - " + GUILocalizeStrings.Get(10798741)); // srv name + " - (active)" 
            else
              dlg.Add("'" + MyFilms.conf.StrNasName2 + "' - " + GUILocalizeStrings.Get(10798742)); // srv name + " - (offline) - start ?"
            choiceSearch.Add("NAS2");
          }

          if (MyFilms.conf.StrNasName3.Length > 0)
          {
            if (WakeOnLanManager.Ping(MyFilms.conf.StrNasName3, intTimeOut))
              dlg.Add("'" + MyFilms.conf.StrNasName3 + "' - " + GUILocalizeStrings.Get(10798741)); // srv name + " - (active)" 
            else
              dlg.Add("'" + MyFilms.conf.StrNasName3 + "' - " + GUILocalizeStrings.Get(10798742)); // srv name + " - (offline) - start ?"
            choiceSearch.Add("NAS3");
          }

          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1)
            return;

          switch (choiceSearch[dlg.SelectedLabel])
          {
            case "BACK":
              return;

            default:

              string NasServerName;
              string NasMACAddress;
              if (choiceSearch[dlg.SelectedLabel] == "NAS1")
              {
                NasServerName = MyFilms.conf.StrNasName1;
                NasMACAddress = MyFilms.conf.StrNasMAC1;
              }
              else if (choiceSearch[dlg.SelectedLabel] == "NAS2")
              {
                NasServerName = MyFilms.conf.StrNasName2;
                NasMACAddress = MyFilms.conf.StrNasMAC2;
              }
              else if (choiceSearch[dlg.SelectedLabel] == "NAS3")
              {
                NasServerName = MyFilms.conf.StrNasName3;
                NasMACAddress = MyFilms.conf.StrNasMAC3;
              }
              else
              {
                NasServerName = String.Empty;
                NasMACAddress = String.Empty;
              }

              GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
              dlgOk.SetHeading(GUILocalizeStrings.Get(10798624)); // MyFilms System Information
              dlgOk.SetLine(1, string.Empty);

              if (NasMACAddress.Length > 0)
              {
                if (wakeOnLanManager.WakeupSystem(wakeOnLanManager.GetHwAddrBytes(NasMACAddress), NasServerName, intTimeOut))
                {
                  dlgOk.SetLine(2, "'" + NasServerName + "' " + GUILocalizeStrings.Get(10798743)); //successfully started 
                }
                else dlgOk.SetLine(2, "'" + NasServerName + "' " + GUILocalizeStrings.Get(10798744)); // could not be started 
              }
              else
              {
                dlgOk.SetLine(1, "Servername: '" + NasServerName + "', MAC: '" + NasMACAddress + "'");
                dlgOk.SetLine(2, GUILocalizeStrings.Get(10798745)); // start not possible - check config !
              }
              dlgOk.DoModal(GetID);
              break;
          }
          return;

        case "globaloptions":
          LogMyFilms.Debug("Building (sub)menu globaloptions");

          GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg1 == null) return;
          dlg1.Reset();
          dlg1.SetHeading(GUILocalizeStrings.Get(10798689)); // Global Options ...
          System.Collections.Generic.List<string> choiceViewGlobalOptions = new System.Collections.Generic.List<string>();

          // Change global Unwatchedfilteroption
          // if ((MesFilms.conf.CheckWatched) || (MesFilms.conf.StrSupPlayer))// Make it conditoional, so only displayed, if options enabled in setup !
          if (MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798628)));
          if (!MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("globalunwatchedfilter");

          // Change global MovieFilter (Only Movies with media files reachable (requires at least initial scan!)
          if (InitialIsOnlineScan)
          {
            if (GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798628)));
            if (!GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798629)));
            choiceViewGlobalOptions.Add("filterdbisonline");
          }

          // Change global MovieFilter (Only Movies with Trailer)
          if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
          {
            if (GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798628)));
            if (!GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798629)));
            choiceViewGlobalOptions.Add("filterdbtrailer");
          }

          // Change global MovieFilter (Only Movies with highRating)
          if (GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798628)));
          if (!GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("filterdbrating");

          // Change Value for global MovieFilter (Only Movies with highRating)
          dlg1.Add(string.Format(GUILocalizeStrings.Get(10798693), MyFilms.conf.StrAntFilterMinRating.ToString()));
          choiceViewGlobalOptions.Add("filterdbsetrating");

          // Choose UserProfileName
          //if (MyFilmsDetail.ExtendedStartmode("Global Settings - Enhanced Watched Status Handling - User Profile Name"))
          //{
            if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
            {
              dlg1.Add(string.Format(GUILocalizeStrings.Get(1079840), conf.StrUserProfileName));
              choiceViewGlobalOptions.Add("userprofilename");
            }
          //}

          // From ZebonsMerge
          //dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), MesFilms.conf.StrGrabber_ChooseScript.ToString(), (!MesFilms.conf.StrGrabber_ChooseScript).ToString()));   // Choose grabber script for that session
          if (MyFilms.conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798628)));   // Choose grabber script for that session (status on)
          if (!MyFilms.conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798629)));   // Choose grabber script for that session (status off)
          choiceViewGlobalOptions.Add("choosescript");

          //dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), MesFilms.conf.StrGrabber_Always.ToString(), (!MesFilms.conf.StrGrabber_Always).ToString()));   // Change grabber find trying best match option 
          if (MyFilms.conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798628)));   // Change grabber find trying best match option (status on)
          if (!MyFilms.conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798629)));   // Change grabber find trying best match option (status off)
          choiceViewGlobalOptions.Add("findbestmatch");

          //dlg1.Add(string.Format(GUILocalizeStrings.Get(1079865), MesFilms.conf.WindowsFileDialog.ToString(), (!MesFilms.conf.WindowsFileDialog).ToString()));  // Using Windows File Dialog File for that session
          if (MyFilms.conf.WindowsFileDialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798628)));   // Using Windows File Dialog File for that session (status on)
          if (!MyFilms.conf.WindowsFileDialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798629)));   // Using Windows File Dialog File for that session (status off)
          choiceViewGlobalOptions.Add("windowsfiledialog");

          if (MyFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798628)));
          if (!MyFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("alwaysdefaultview");

          if (MyFilms.conf.UseListViewForGoups) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079897), GUILocalizeStrings.Get(10798628)));
          if (!MyFilms.conf.UseListViewForGoups) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079897), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("alwayslistforgroups");

          if (MyFilms.conf.StrCheckWOLenable)
          {
            if (MyFilms.conf.StrCheckWOLuserdialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079898), GUILocalizeStrings.Get(10798628)));
            if (!MyFilms.conf.StrCheckWOLuserdialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079898), GUILocalizeStrings.Get(10798629)));
            choiceViewGlobalOptions.Add("woluserdialog");
          }

          dlg1.DoModal(GetID);
          if (dlg1.SelectedLabel == -1)
          {
            return;
          }

          LogMyFilms.Debug("Call global menu with option: '" + choiceViewGlobalOptions[dlg1.SelectedLabel].ToString() + "'");

          Change_view(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
          return;

        case "globalupdates":

          GUIDialogMenu dlg2 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg2 == null) return;
          dlg2.Reset();
          dlg2.SetHeading(GUILocalizeStrings.Get(10798690)); // Global Updates ...
          System.Collections.Generic.List<string> choiceViewGlobalUpdates = new System.Collections.Generic.List<string>();

          dlg2.Add(GUILocalizeStrings.Get(1079850));   // Update Online status - to check availability if media files
          choiceViewGlobalUpdates.Add("isonlinecheck");

          if (MyFilms.conf.StrAMCUpd)
          {
            if (bgUpdateDB.IsBusy && bgUpdateDB.WorkerSupportsCancellation)
            {
              // stop background worker
              dlg2.Add(GUILocalizeStrings.Get(1079855));   // Stop Database Updater (active)
              choiceViewGlobalUpdates.Add("cancelupdatedb");
            }
            else
            {
              dlg2.Add(GUILocalizeStrings.Get(1079861));   // Update Database with external AMCupdater
              choiceViewGlobalUpdates.Add("updatedb");
            }
          }

          if (MyFilms.conf.StrFanart)
          {
            dlg2.Add(GUILocalizeStrings.Get(4514));   // Download all Fanart
            choiceViewGlobalUpdates.Add("downfanart");
          }

          if (MyFilmsDetail.ExtendedStartmode("Global Update all PersonInfos")) // check if specialmode is configured for disabled features
          {
            dlg2.Add(GUILocalizeStrings.Get(10798715)); // Load Person infos - all persons
            // Search all personinfos
            choiceViewGlobalUpdates.Add("personinfos-all");
          }

          if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
          {
            dlg2.Add(GUILocalizeStrings.Get(10798694));
            // Search and register all trailers for all movies in DB
            choiceViewGlobalUpdates.Add("trailer-all");
          }

          dlg2.Add(GUILocalizeStrings.Get(10798717)); // incomplete movie data
          // Search records with missing movie data, e.g. after import when internet data loading failed
          choiceViewGlobalUpdates.Add("incomplete-movie-data");

          dlg2.DoModal(GetID);

          if (dlg2.SelectedLabel == -1)
          {
            return;
          }
          Change_view(choiceViewGlobalUpdates[dlg2.SelectedLabel].ToLower());
          return;

        case "globalmappings": // map useritems from GUI
          GUIDialogMenu dlg3 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg3 == null) return;
          dlg3.Reset();
          dlg3.SetHeading(GUILocalizeStrings.Get(10798771)); // Display options ...
          System.Collections.Generic.List<string> choiceGlobalMappings = new System.Collections.Generic.List<string>();
          
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 1 (" + MyFilms.conf.Stritem1 + "-" + MyFilms.conf.Strlabel1 + ")");
          choiceGlobalMappings.Add("useritem1");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 2 (" + MyFilms.conf.Stritem2 + "-" + MyFilms.conf.Strlabel2 + ")");
          choiceGlobalMappings.Add("useritem2");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 3 (" + MyFilms.conf.Stritem3 + "-" + MyFilms.conf.Strlabel3 + ")");
          choiceGlobalMappings.Add("useritem3");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 4 (" + MyFilms.conf.Stritem4 + "-" + MyFilms.conf.Strlabel4 + ")");
          choiceGlobalMappings.Add("useritem4");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 5 (" + MyFilms.conf.Stritem5 + "-" + MyFilms.conf.Strlabel5 + ")");
          choiceGlobalMappings.Add("useritem5");
          
          // master-, secondary-  and sorttitle
          //dlg3.Add(GUILocalizeStrings.Get(10798790) + " (" + MyFilms.conf.StrTitle1 + "-" + BaseMesFilms.Translate_Column(MyFilms.conf.StrTitle1) + ")"); // mastertitle
          dlg3.Add(GUILocalizeStrings.Get(10798790) + " (" + MyFilms.conf.StrTitle1 + "-" + BaseMesFilms.Translate_Column(MyFilms.conf.StrTitle1) + ")"); // mastertitle
          choiceGlobalMappings.Add("mastertitle");
          dlg3.Add(GUILocalizeStrings.Get(10798791) + " (" + MyFilms.conf.StrTitle2 + "-" + BaseMesFilms.Translate_Column(MyFilms.conf.StrTitle2) + ")"); // secondary title
          choiceGlobalMappings.Add("secondarytitle");
          dlg3.Add(GUILocalizeStrings.Get(10798792) + " (" + MyFilms.conf.StrSTitle + "-" + BaseMesFilms.Translate_Column(MyFilms.conf.StrSTitle) + ")"); // sort title
          choiceGlobalMappings.Add("sorttitle");

          dlg3.DoModal(GetID);
          if (dlg3.SelectedLabel == -1)
          { return; }
          int selection = dlg3.SelectedLabel;
          string strUserItemSelection = choiceGlobalMappings[dlg3.SelectedLabel];
          dlg3.Reset();
          choiceGlobalMappings.Clear();

          dlg3.SetHeading(GUILocalizeStrings.Get(10798772)); // Choose field ...
          switch (strUserItemSelection)
          {
            case "mastertitle":
            case "sorttitle":
              break;
            case "secondarytitle":
              dlg3.Add("<" + GUILocalizeStrings.Get(10798774) + ">"); // empty
              choiceGlobalMappings.Add("(none)");
              break;
            default:
              dlg3.Add("<" + GUILocalizeStrings.Get(10798774) + ">"); // empty
              choiceGlobalMappings.Add("");
              break;
          }
          AntMovieCatalog amc = new AntMovieCatalog();
          foreach (DataColumn dc in amc.Movie.Columns)
          {
            if (dc.ColumnName != null && !string.IsNullOrEmpty(BaseMesFilms.Translate_Column(dc.ColumnName)))
            {
              if (dc.ColumnName != "Picture" && dc.ColumnName != "Fanart" && dc.ColumnName != "Contents_Id" && dc.ColumnName != "DateWatched"
                && dc.ColumnName != "SourceTrailer" && dc.ColumnName != "IsOnline" && dc.ColumnName != "IsOnlineTrailer")
              {
                if (selection > 4) // title fields
                {
                  if (dc.ColumnName == "OriginalTitle" || dc.ColumnName == "TranslatedTitle" || dc.ColumnName == "FormattedTitle")
                  {
                    dlg3.Add(dc.ColumnName + "-" + BaseMesFilms.Translate_Column(dc.ColumnName));
                    choiceGlobalMappings.Add(dc.ColumnName);
                  }
                }
                else // display item fields
                {
                  if (MyFilms.conf.StrFileType != "0" || (dc.ColumnName != "DateAdded" && dc.ColumnName != "IMDB_Id" && dc.ColumnName != "TMDB_Id" && dc.ColumnName != "Watched"
                    && dc.ColumnName != "Certification" && dc.ColumnName != "Writer" && dc.ColumnName != "TagLine" && dc.ColumnName != "Tags"
                    && dc.ColumnName != "RatingUser" && dc.ColumnName != "Studio" && dc.ColumnName != "IMDB_Rank" && dc.ColumnName != "Edition" && dc.ColumnName != "Aspectratio"))
                  {
                    dlg3.Add(BaseMesFilms.Translate_Column(dc.ColumnName));
                    choiceGlobalMappings.Add(dc.ColumnName);
                  }
                }
              }
            }
          }

          dlg3.DoModal(GetID);
          if (dlg3.SelectedLabel == -1)
            return;
          string wproperty = choiceGlobalMappings[dlg3.SelectedLabel];
          dlg3.Reset();
          choiceGlobalMappings.Clear();
          LogMyFilms.Debug("Display Options - new field: '" + wproperty + "', new Label: '" + BaseMesFilms.Translate_Column(wproperty) + "'.");
          switch (strUserItemSelection)
          {
            case "useritem1":
              MyFilms.conf.Stritem1 = wproperty;
              MyFilms.conf.Strlabel1 = BaseMesFilms.Translate_Column(wproperty);
              LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.Stritem1 + "', Label: '" + conf.Strlabel1 + "'.");
              break;
            case "useritem2":
              MyFilms.conf.Stritem2 = wproperty;
              MyFilms.conf.Strlabel2 = BaseMesFilms.Translate_Column(wproperty);
              LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.Stritem2 + "', Label: '" + conf.Strlabel2 + "'.");
              break;
            case "useritem3":
              MyFilms.conf.Stritem3 = wproperty;
              MyFilms.conf.Strlabel3 = BaseMesFilms.Translate_Column(wproperty);
              LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.Stritem3 + "', Label: '" + conf.Strlabel3 + "'.");
              break;
            case "useritem4":
              MyFilms.conf.Stritem4 = wproperty;
              MyFilms.conf.Strlabel4 = BaseMesFilms.Translate_Column(wproperty);
              LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.Stritem4 + "', Label: '" + conf.Strlabel4 + "'.");
              break;
            case "useritem5":
              MyFilms.conf.Stritem5 = wproperty;
              MyFilms.conf.Strlabel5 = BaseMesFilms.Translate_Column(wproperty);
              LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.Stritem5 + "', Label: '" + conf.Strlabel5 + "'.");
              break;
            case "mastertitle":
              MyFilms.conf.StrTitle1 = wproperty;
              LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StrTitle1 + "'.");
              break;
            case "secondarytitle":
              MyFilms.conf.StrTitle2 = wproperty;
              LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StrTitle2 + "'.");
              break;
            case "sorttitle":
              MyFilms.conf.StrSTitle = wproperty;
              LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StrSTitle + "'.");
              break;
            default:
              break;
          }
          UpdateUserItems(); // save to currentconfig - save time for WinDeInit
          //Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
          //Load_Config(Configuration.CurrentConfig, true);
          MyFilmsDetail.Init_Detailed_DB(false); // clear properties 
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          return;


        case "userprofilename": // choose global user profile name
          GUIDialogMenu dlg4 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg4 == null) return;
          dlg4.Reset();
          dlg4.SetHeading(string.Format(GUILocalizeStrings.Get(1079840), conf.StrUserProfileName)); // Choose User Profile Name ... GUILocalizeStrings.Get(1079840), conf.StrUserProfileName)
          System.Collections.Generic.List<string> choiceGlobalUserProfileName = new System.Collections.Generic.List<string>();

          //// Add Configured UserProfileName from Setup  
          //if (conf.StrUserProfileName.Length > 0)
          //{
          //  dlg4.Add(conf.StrUserProfileName); // current Value
          //  choiceGlobalUserProfileName.Add(conf.StrUserProfileName);
          //}

          dlg4.Add("<" + GUILocalizeStrings.Get(10798630) + ">"); // New Value ...
          choiceGlobalUserProfileName.Add("");

          // add Trakt user, if there is any configured:
          if (Helper.IsTraktAvailableAndEnabled)
          {
            string currentTraktuser = Helper.GetTraktUser();
            if (!string.IsNullOrEmpty(currentTraktuser))
            {
              dlg4.Add(currentTraktuser + " (Trakt)");
              choiceGlobalUserProfileName.Add(currentTraktuser);
            }
          }

          // Add already existing UserProfileNames - example of string value: "Global:3|Mike:0|Sandy:1"
          foreach (DataRow sr in r)
          {
            string strEnhancedWatchedValue = sr[conf.StrWatchedField].ToString().Trim();
            string[] split1 = strEnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split1)
            {
              if (s.Contains(":"))
              {
                string userprofilename = s.Substring(0, s.IndexOf(":")); // extract userprofilename
                if (!choiceGlobalUserProfileName.Contains(userprofilename) && userprofilename != "Global")
                {
                  dlg4.Add(userprofilename);
                  choiceGlobalUserProfileName.Add(userprofilename);
                }
              }
            }
          }
          dlg4.DoModal(GetID);
          if (dlg4.SelectedLabel == -1)
            return;
          string strUserProfileNameSelection = choiceGlobalUserProfileName[dlg4.SelectedLabel];
          switch (strUserProfileNameSelection)
          {
            case "": // new value
              VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
              if (null == keyboard) return;
              keyboard.Reset();
              keyboard.Text = ""; // Default string is empty
              keyboard.DoModal(GetID);
              if (keyboard.IsConfirmed && (!string.IsNullOrEmpty(keyboard.Text)))
                conf.StrUserProfileName = keyboard.Text;
              else 
                return;
              break;
            default:
              conf.StrUserProfileName = strUserProfileNameSelection;
              break;
          }
          XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "UserProfileName", MyFilms.conf.StrUserProfileName);
          LogMyFilms.Debug("UserProfileName - change to '" + conf.StrUserProfileName.ToString() + "'");
          //Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
          //Load_Config(Configuration.CurrentConfig, true);
          MyFilmsDetail.Init_Detailed_DB(false); // clear properties 
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          return;


        case "globalwikihelp":
          var hasRightPlugin = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "BrowseTheWeb");
          var hasRightVersion = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "BrowseTheWeb" && plugin.GetType().Assembly.GetName().Version.Minor >= 0);
          if (hasRightPlugin && hasRightVersion)
          {
            //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser
            //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
            string url = "http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films";
            string zoom = "150";

            //Load Webbrowserplugin with the URL
            LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
            GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
            GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
            MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
            GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false); //54537689
            MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
            GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
            GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
          }
          else
          {
            ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0");
          }
          break;

        case "about":
          string infoBackgroundProcess = string.Empty;
          if (bgUpdateFanart.IsBusy)
            infoBackgroundProcess = "running (fanart & artwork)";
          else
            infoBackgroundProcess = "not active";
          GUIDialogOK dlgok = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
          if (dlgok == null) return;
          dlgok.Reset();
          dlgok.SetHeading(GUILocalizeStrings.Get(10798624)); // MyFilms System Information

          System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
          dlgok.SetLine(1, "MyFilms Version = 'V" + asm.GetName().Version.ToString() + "'");
          dlgok.SetLine(2, "MyFilms Operations Mode = '" + Configuration.PluginMode + "'");
          dlgok.SetLine(3, "MyFilms Background Process = '" + infoBackgroundProcess + "'");
          dlgok.DoModal(GetID);
          break;

        case "globalunwatchedfilter":
          // Global overlayfilter for unwatched movies ...
          MyFilms.conf.GlobalUnwatchedOnly = !MyFilms.conf.GlobalUnwatchedOnly;
          if (conf.GlobalUnwatchedOnly)
          {
            if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
              GlobalFilterStringUnwatched = conf.StrWatchedField + " like '*" + conf.StrUserProfileName + ":0*" + "' AND ";
            else
              GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
            MyFilmsDetail.setGUIProperty("globalfilter.unwatched", "true");
          }
          else
          {
            GlobalFilterStringUnwatched = String.Empty;
            MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
          }
          LogMyFilms.Info("Global filter for Unwatched Only is now set to '" + GlobalFilterStringUnwatched + "'");
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu)
            Change_view("globaloptions");
          else
            Context_Menu = false;
          break;

        case "filterdbisonline":
          // GlobalFilterIsOnline
          GlobalFilterIsOnlineOnly = !GlobalFilterIsOnlineOnly;
          LogMyFilms.Info("Global filter for IsOnline available media files is now set to '" + GlobalFilterIsOnlineOnly + "'");
          if (GlobalFilterIsOnlineOnly)
          {
            GlobalFilterStringIsOnline = "IsOnline like 'true' AND ";
            MyFilmsDetail.setGUIProperty("globalfilter.isonline", "true");
          }
          else
          {
            GlobalFilterStringIsOnline = String.Empty;
            MyFilmsDetail.clearGUIProperty("globalfilter.isonline");
          }
          LogMyFilms.Info("(SetGlobalFilterString IsOnline) - 'GlobalFilterStringIsOnline' = '" + GlobalFilterStringIsOnline + "'");
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu)
            Change_view("globaloptions");
          else
            Context_Menu = false;
          break;

        case "filterdbtrailer":
          // GlobalFilterTrailersOnly
          GlobalFilterTrailersOnly = !GlobalFilterTrailersOnly;
          LogMyFilms.Info("Global filter for Trailers Only is now set to '" + GlobalFilterTrailersOnly + "'");
          //if (GlobalFilterTrailersOnly) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798628));
          //if (!GlobalFilterTrailersOnly) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798629));
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          if (GlobalFilterTrailersOnly)
          {
            GlobalFilterStringTrailersOnly = conf.StrStorageTrailer + " not like '' AND ";
            MyFilmsDetail.setGUIProperty("globalfilter.trailersonly", "true");
          }
          else
          {
            GlobalFilterStringTrailersOnly = String.Empty;
            MyFilmsDetail.clearGUIProperty("globalfilter.trailersonly");
          }
          LogMyFilms.Info("(SetGlobalFilterString Trailers) - 'GlobalFilterStringTrailersOnly' = '" + GlobalFilterStringTrailersOnly + "'");
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu)
            Change_view("globaloptions");
          else
            Context_Menu = false;
          break;

        case "filterdbrating":
          // GlobalFilterMinRating
          GlobalFilterMinRating = !GlobalFilterMinRating;
          LogMyFilms.Info("Global filter for MinimumRating is now set to '" + GlobalFilterMinRating + "'");
          if (GlobalFilterMinRating)
          {
            GlobalFilterStringMinRating = "Rating > " + MyFilms.conf.StrAntFilterMinRating.Replace(",", ".") + " AND ";
            MyFilmsDetail.setGUIProperty("globalfilter.minrating", "true");
            MyFilmsDetail.setGUIProperty("globalfilter.minratingvalue", MyFilms.conf.StrAntFilterMinRating);
          }
          else
          {
            GlobalFilterStringMinRating = String.Empty;
            MyFilmsDetail.clearGUIProperty("globalfilter.minrating");
            MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue");
          }
          LogMyFilms.Info("(SetGlobalFilterString MinRating) - 'GlobalFilterStringMinRating' = '" + GlobalFilterStringMinRating + "'");
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu)
            Change_view("globaloptions");
          else
            Context_Menu = false;
          break;

        case "filterdbsetrating":
          // Set global value for minimum Rating to restrict movielist
          LogMyFilms.Info("(FilterDbSetRating) - 'AntFilterMinRating' current setting = '" + MyFilms.conf.StrAntFilterMinRating + "', current decimalseparator: '" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString() + "'");
          MyFilmsDialogSetRating dlgRating = (MyFilmsDialogSetRating)GUIWindowManager.GetWindow(ID_MyFilmsDialogRating);
          if (MyFilms.conf.StrAntFilterMinRating.Length > 0)
          {
            decimal wrating = 0;
            wrating = Convert.ToDecimal(MyFilms.conf.StrAntFilterMinRating.Replace(",", "."), CultureInfo.InvariantCulture);
            dlgRating.Rating = wrating;
          }
          else
            dlgRating.Rating = 0;
          dlgRating.SetTitle(GUILocalizeStrings.Get(1079881));
          dlgRating.DoModal(GetID);

          MyFilms.conf.StrAntFilterMinRating = dlgRating.Rating.ToString("0.0", CultureInfo.InvariantCulture);
          XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntFilterMinRating", MyFilms.conf.StrAntFilterMinRating);
          LogMyFilms.Info("(FilterDbSetRating) - 'AntFilterMinRating' changed to '" + MyFilms.conf.StrAntFilterMinRating + "'");
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          if (GlobalFilterMinRating)
          {
            GlobalFilterStringMinRating = "Rating > " + MyFilms.conf.StrAntFilterMinRating.Replace(",", ".") + " AND ";
            MyFilmsDetail.setGUIProperty("globalfilter.minrating", "true");
            MyFilmsDetail.setGUIProperty("globalfilter.minratingvalue", MyFilms.conf.StrAntFilterMinRating);
          }
          else
          {
            GlobalFilterStringMinRating = String.Empty;
            MyFilmsDetail.clearGUIProperty("globalfilter.minrating");
            MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue");
          }
          LogMyFilms.Info("(SetGlobalFilterString) - 'GlobalFilterStringMinRating' = '" + GlobalFilterStringMinRating + "'");
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu)
            Change_view("globaloptions");
          else
            Context_Menu = false;
          break;

        case "isonlinecheck":
          // Launch IsOnlineCheck in batch mode
          if (bgIsOnlineCheck.IsBusy)
          {
            ShowMessageDialog(GUILocalizeStrings.Get(1079850), GUILocalizeStrings.Get(875), GUILocalizeStrings.Get(330)); //action already launched
            break;
          }
          this.AsynIsOnlineCheck();
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          break;

        case "updatedb":
          // Launch AMCUpdater in batch mode
          if (bgUpdateDB.IsBusy)
          {
            ShowMessageDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(875), GUILocalizeStrings.Get(330)); //action already launched
            break;
          }
          if (MyFilmsDetail.GlobalLockIsActive(MyFilms.conf.StrFileXml))
          {
            ShowMessageDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(1079854), GUILocalizeStrings.Get(330)); //movie db already in use (locked)
            break;
          }
          AsynUpdateDatabase();
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          break;

        case "cancelupdatedb":
          // stop background worker
          if (bgUpdateDB.IsBusy && bgUpdateDB.WorkerSupportsCancellation)
          {
            bgUpdateDB.CancelAsync();
          }
          else
          {
            ShowMessageDialog("", "AMC Updater cannot be stopped!", ""); // AMC Updater is stopping!
          }
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          break;
        
        case "downfanart":
          // Launch Fanart download in batch mode
          if (bgUpdateFanart.IsBusy)
          {
            ShowMessageDialog(GUILocalizeStrings.Get(1079862), GUILocalizeStrings.Get(921), GUILocalizeStrings.Get(330)); //action already launched
            break;
          }
          AsynUpdateFanart();
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          break;

        case "personinfos-all":
          // Search and update all personinfos from IMDB
          // Todo: Call Singlepersonupdate - maybe this function should also be available for single movies ? (less traffic)
          // ToDo: - first implement singlepersonupdate ...
          break;

        case "trailer-all":
          // Launch "Search and register all trailers for all movies in DB" in batch mode
          //if (bgUpdateTrailer.IsBusy)
          //{
          //    ShowMessageDialog(GUILocalizeStrings.Get(10798694), GUILocalizeStrings.Get(921), GUILocalizeStrings.Get(330)); //action already launched
          //    break;
          //}
          //AsynUpdateTrailer();
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          //break;

          AntMovieCatalog ds = new AntMovieCatalog();
          ArrayList w_index = new ArrayList();
          int w_index_count = 0;
          string t_number_id = "";
          DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
          //Now build a list of valid movies in w_index with Number registered
          foreach (DataRow wsr in wr)
          {
            foreach (DataColumn dc in ds.Movie.Columns)
            {
              //LogMyFilms.Debug("(GlobalSearchTrailerLocal) - dc.ColumnName '" + dc.ColumnName.ToString() + "'");
              if (dc.ColumnName.ToString() == "Number")
              {
                t_number_id = wsr[dc.ColumnName].ToString();
                //LogMyFilms.Debug("(GlobalSearchTrailerLocal) - Movienumber stored as '" + t_number_id + "'");
              }
            }
            foreach (DataColumn dc in ds.Movie.Columns)
            {
              if (dc.ColumnName.ToLower() == "translatedtitle")
              {
                w_index.Add(t_number_id);
                LogMyFilms.Debug("(GlobalSearchTrailerLocal) - Add MovieIDs to indexlist: dc: '" + dc + "' and Number(ID): '" + t_number_id + "'");
                w_index_count = w_index_count + 1;
              }
            }
          }
          LogMyFilms.Debug("(GlobalSearchTrailerLocal) - Number of Records found: " + w_index_count);

          GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(10798800)); // Warning: Long runtime !
          dlgYesNo.SetLine(1, GUILocalizeStrings.Get(10798801)); //should really the trailer search be started
          dlgYesNo.SetLine(2, string.Format(GUILocalizeStrings.Get(10798802), w_index_count.ToString())); // for <xx> movies ?
          dlgYesNo.DoModal(GetID);
          if (!(dlgYesNo.IsConfirmed))
            break;
          MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
          for (i = 0; i < w_index_count; i++)
          {
            LogMyFilms.Debug("(GlobalSearchTrailerLocal) - Number: '" + i.ToString() + "' - Index to search: '" + w_index[i] + "'");
            //MyFilmsDetail.SearchTrailerLocal((DataRow[])MesFilms.r, Convert.ToInt32(w_index[i]));
            MyFilmsDetail.SearchTrailerLocal((DataRow[])MyFilms.r, Convert.ToInt32(i), false);
          }
          MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798695)); //Traiersearch finished!
          break;

        case "incomplete-movie-data":
          SearchIncompleteMovies(MyFilms.conf.StrSearchList);
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_List); // set focus to list
          break;
        
        case "choosescript":
          MyFilms.conf.StrGrabber_ChooseScript = !MyFilms.conf.StrGrabber_ChooseScript;
          XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_ChooseScript", MyFilms.conf.StrGrabber_ChooseScript);
          LogMyFilms.Info("Grabber Option 'use always that script' changed to " + MyFilms.conf.StrGrabber_ChooseScript.ToString());
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          Change_view("globaloptions");
          break;
        case "findbestmatch":
          MyFilms.conf.StrGrabber_Always = !MyFilms.conf.StrGrabber_Always;
          XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_Always", MyFilms.conf.StrGrabber_Always);
          LogMyFilms.Info("Grabber Option 'try to find best match...' changed to " + MyFilms.conf.StrGrabber_Always.ToString());
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          Change_view("globaloptions");
          break;
        case "windowsfiledialog":
          MyFilms.conf.WindowsFileDialog = !MyFilms.conf.WindowsFileDialog;
          XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "WindowsFileDialog", MyFilms.conf.WindowsFileDialog);
          LogMyFilms.Info("Update Option 'use Windows File Dialog...' changed to " + MyFilms.conf.WindowsFileDialog.ToString());
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          Change_view("globaloptions");
          break;
        case "alwaysdefaultview":
          MyFilms.conf.AlwaysDefaultView = !MyFilms.conf.AlwaysDefaultView;
          XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AlwaysDefaultView", MyFilms.conf.AlwaysDefaultView);
          LogMyFilms.Info("Update Option 'use always default view...' changed to " + MyFilms.conf.AlwaysDefaultView.ToString());

          //if (MesFilms.conf.AlwaysDefaultView) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798628));
          //if (!MesFilms.conf.AlwaysDefaultView) ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798629));

          //if (MyFilms.conf.AlwaysDefaultView)
          //  Fin_Charge_Init(true, true); //DefaultSelect, reload
          //else
          //  Fin_Charge_Init(true, true); //NotDefaultSelect, Only reload
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
          Change_view("globaloptions");
          break;
        case "alwayslistforgroups":
          MyFilms.conf.UseListViewForGoups = !MyFilms.conf.UseListViewForGoups;
          XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "UseListviewForGroups", MyFilms.conf.UseListViewForGoups);
          LogMyFilms.Info("Update Option 'use list view for groups ...' changed to " + MyFilms.conf.UseListViewForGoups);
          Change_view("globaloptions");
          break;

        case "woluserdialog":
          MyFilms.conf.StrCheckWOLuserdialog = !MyFilms.conf.StrCheckWOLuserdialog;
          XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "WOL-Userdialog", MyFilms.conf.StrCheckWOLuserdialog);
          LogMyFilms.Info("Update Option 'use WOL userdialog ...' changed to " + MyFilms.conf.StrCheckWOLuserdialog);
          Change_view("globaloptions");
          break;

      }
      LogMyFilms.Debug("Change_View(" + choiceView + ") -> End");
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
      dlg.SetHeading(GUILocalizeStrings.Get(1079904)); // Context options ...
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

        if (facadeView.Focus && !facadeView.SelectedListItem.IsFolder) // 112 = "p", 120 = "x"
        {
          dlg.Add(GUILocalizeStrings.Get(10798709));//play movie 
          upd_choice[ichoice] = "playmovie";
          ichoice++;
        }

        if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
        {
          string trailercount = "";
          if (string.IsNullOrEmpty(MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrStorageTrailer].ToString().Trim()))
            trailercount = "0";
          else
          {
            string[] split1 = MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrStorageTrailer].ToString().Trim().Split(new Char[] { ';' });
            trailercount = split1.Count().ToString();
            if (trailercount != "0")
            {
              dlg.Add(GUILocalizeStrings.Get(10798710) + " (" + trailercount + ")");//play trailer (<number trailers present>)
              upd_choice[ichoice] = "playtrailer";
              ichoice++;
            }
          }
        }

        if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
        {
          if (facadeView.SelectedListItem.IsPlayed) // show only the required option
            dlg.Add(GUILocalizeStrings.Get(1079895)); // set unwatched
          else dlg.Add(GUILocalizeStrings.Get(1079894)); // set watched
          upd_choice[ichoice] = "togglewatchedstatus";
          ichoice++;
        }

        // Enable/disable global overlay filter could be added here for faster access ?
        // ...
        //dlg.Add(GUILocalizeStrings.Get(10798714)); // "global filters ..."
        //upd_choice[ichoice] = "globalfilters";
        //ichoice++;

        if (MyFilmsDetail.ExtendedStartmode("Context: IMDB Trailer and Pictures")) // check if specialmode is configured for disabled features
        {
          dlg.Add(GUILocalizeStrings.Get(1079887));
          upd_choice[ichoice] = "movieimdbtrailer";
          ichoice++;
        }

        dlg.Add(GUILocalizeStrings.Get(1079888));
        upd_choice[ichoice] = "movieimdbbilder";
        ichoice++;

        dlg.Add(GUILocalizeStrings.Get(1079889));
        upd_choice[ichoice] = "movieimdbinternet";
        ichoice++;

        if (MyFilmsDetail.ExtendedStartmode("Context: Personlist in facade")) // check if specialmode is configured for disabled features
        {
          dlg.Add(GUILocalizeStrings.Get(1079879));//Search Infos to related persons (load persons in facadeview) - only available in filmlist
          upd_choice[ichoice] = "moviepersonlist";
          ichoice++;
        }

        if (MyFilmsDetail.ExtendedStartmode("Context: IMDB Update for all persons of movie")) // check if specialmode is configured for disabled features
        {
          dlg.Add(GUILocalizeStrings.Get(1079883)); // update personinfos for all envolved persons of a selected movie from IMDB and/or TMDB
          upd_choice[ichoice] = "updatepersonmovie";
          ichoice++;
        }
      }

      // Artistcontext
      if (facadeView.SelectedListItemIndex > -1 && facadeView.SelectedListItem.IsFolder && (conf.WStrSort.ToLower().Contains("actor") || conf.WStrSort.ToLower().Contains("director") || conf.WStrSort.ToLower().Contains("producer")))
      {
        if (MyFilmsDetail.ExtendedStartmode("Context Artist: Show Infos of person via person dialog")) // check if specialmode is configured for disabled features
        {
          dlg.Add(GUILocalizeStrings.Get(1079884));
          //Show Infos of person (load persons detail dialog - MesFilmsActorDetails) - only available in personlist
          upd_choice[ichoice] = "artistdetail";
          ichoice++;
        }

        dlg.Add(GUILocalizeStrings.Get(1079886));//Show IMDB internetinfos http://www.imdb.com/name/nm0000288/
        upd_choice[ichoice] = "artistimdbinternet";
        ichoice++;

        dlg.Add(GUILocalizeStrings.Get(1079890));//Show IMDB clips http://www.imdb.com/name/nm0000288/videogallery
        upd_choice[ichoice] = "artistimdbclips";
        ichoice++;

        dlg.Add(GUILocalizeStrings.Get(1079891));//Show IMDB pictures http://www.imdb.com/name/nm0000288/mediaindex
        upd_choice[ichoice] = "artistimdbbilder";
        ichoice++;

        if (MyFilmsDetail.ExtendedStartmode("Context Artist: IMDB all sort of details and updates (several entries)")) // check if specialmode is configured for disabled features
        {
          dlg.Add(GUILocalizeStrings.Get(1079885));//Show IMDB filmlist in facadeview and add availabilityinformations to it
          upd_choice[ichoice] = "artistimdbfilmlist";
          ichoice++;

          dlg.Add(GUILocalizeStrings.Get(1079882)); // update personinfo from IMDB and create actorthumbs - optional: load mediathek for person backdrops etc.
          upd_choice[ichoice] = "updateperson";
          ichoice++;
        }
      }

      if (MyFilms.conf.StrSuppress && facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
      {
        dlg.Add(GUILocalizeStrings.Get(432));
        upd_choice[ichoice] = "suppress";
        ichoice++;
      }

      if (MyFilms.conf.StrFanart && facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
      {
        dlg.Add(GUILocalizeStrings.Get(1079862));
        upd_choice[ichoice] = "fanart";
        ichoice++;
        dlg.Add(GUILocalizeStrings.Get(1079874));
        upd_choice[ichoice] = "deletefanart";
        ichoice++;
      }

      //if (MyFilmsDetail.ExtendedStartmode("Context Movie: Internet data update")) // check if specialmode is configured for disabled features
      //{
        //Disabled due to problems of updating DB (requires debugging before reenabling...)
        if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
        {
          dlg.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
          upd_choice[ichoice] = "grabber";
          ichoice++;
        }
      //}

      //if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
      //{
      //    dlg.Add(GUILocalizeStrings.Get(1079892)); // Update ...
      //    upd_choice[ichoice] = "updatemenu";
      //}

      ichoice++;

      dlg.DoModal(GetID);

      if (dlg.SelectedLabel == -1)
        return;
      GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
      switch (upd_choice[dlg.SelectedLabel])
      {
        case "playmovie":
          MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
          break;

        case "playtrailer":
          // first check, if trailer files are available, offer options
          //if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
          if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim()))
          {
            MyFilmsDetail.trailerPlayed = true;
            MyFilmsDetail.Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
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
              //setProcessAnimationStatus(true, m_SearchAnimation);
              //LogMyFilms.Debug("(SearchTrailerLocal) SelectedItemInfo from (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(): '" + (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() + "'"));
              LogMyFilms.Debug("(Auto search trailer after selecting PLAY) title: '" + (MyFilms.r[MyFilms.conf.StrIndex].ToString() + "'"));
              MyFilmsDetail.SearchTrailerLocal((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true);
              //afficher_detail(true);
              //setProcessAnimationStatus(false, m_SearchAnimation);
              MyFilmsDetail.trailerPlayed = true;
              MyFilmsDetail.Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
            }
          }
          break;

        case "analogyperson":
          {
            SearchRelatedMoviesbyPersons((int)facadeView.SelectedListItem.ItemId, Context_Menu);
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
            dlg.DeInit();
            break;
          }

        case "analogyproperty":
          {
            SearchRelatedMoviesbyProperties((int)facadeView.SelectedListItem.ItemId, Context_Menu);
            //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
            dlg.DeInit();
            break;
          }

        case "movieimdbtrailer":
          // Example: (http://www.imdb.com/title/tt0438488/videogallery)
          {
            if (Helper.IsBrowseTheWebAvailableAndEnabled)
            {
              string url = ImdbBaseUrl + "";
              string zoom = "150";
              IMDB _imdb = new IMDB();
              IMDB.IMDBUrl wurl;
              _imdb.Find(facadeView.SelectedListItem.Label);
              IMDBMovie imdbMovie = new IMDBMovie();
              if (_imdb.Count > 0)
              {
                wurl = (IMDB.IMDBUrl)_imdb[0];
                if (wurl.URL.Length != 0)
                {
                  url = wurl.URL + @"videogallery";
                  url = ImdbBaseUrl + url.Substring(url.IndexOf("title")); // redirect to base www.imdb.com server and remove localized returns...
                }

                LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
                GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false); //54537689
                MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
                GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
                GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
              }
              else
              {
                ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
                break;
              }

            }
            else
            {
              ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
            }
            break;
          }

        case "movieimdbbilder":
          // example: http://www.imdb.com/title/tt0133093/mediaindex
          {
            if (Helper.IsBrowseTheWebAvailableAndEnabled)
            {
              string url = ImdbBaseUrl + "";
              string zoom = "150";
              IMDB _imdb = new IMDB();
              IMDB.IMDBUrl wurl;
              _imdb.Find(facadeView.SelectedListItem.Label);
              IMDBMovie imdbMovie = new IMDBMovie();
              if (_imdb.Count > 0)
              {
                wurl = (IMDB.IMDBUrl)_imdb[0];
                if (wurl.URL.Length != 0)
                {
                  url = wurl.URL + @"mediaindex";
                  url = ImdbBaseUrl + url.Substring(url.IndexOf("title")); // redirect to base www.imdb.com server and remove localized returns...
                }
                LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
                GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false); //54537689
                MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
                GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
                GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
              }
              else
              {
                ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
                break;
              }
            }
            else
            {
              ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
            }
            break;
          }

        case "movieimdbinternet":
          if (Helper.IsBrowseTheWebAvailableAndEnabled)
          {
            //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser
            //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
            string url = ImdbBaseUrl + "";
            string zoom = "150";

            //First search corresponding URL for the actor ...
            IMDB _imdb = new IMDB();
            IMDB.IMDBUrl wurl;

            _imdb.Find(facadeView.SelectedListItem.Label);
            IMDBMovie imdbMovie = new IMDBMovie();
            for (int i = 0; i < _imdb.Count; i++)
            {
              LogMyFilms.Debug("movie imdb internet search - found: '" + _imdb[i].Title + "', URL = '" + _imdb[i].URL + "'");
            }
            if (_imdb.Count > 0)
            {
              wurl = (IMDB.IMDBUrl)_imdb[0];
              if (wurl.URL.Length != 0)
              {
                url = wurl.URL; // Assign proper Webpage for Actorinfos
                //url = ImdbBaseUrl + url.Substring(url.IndexOf(".com") + 5); // redirect to base www.imdb.com server and remove localized returns...
              }
              //Load Webbrowserplugin with the URL
              LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
              GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
              GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
              MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
              GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false); //54537689
              MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
              GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
              GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
            }
            else
            {
              ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
              break;
            }

          }
          else
          {
            ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
          }
          break;

        case "moviepersonlist":
          {
            //To be modified to call new class with personlist by type and call MyFilmsActors with facade
            //Temporarily disabled - will be required to create Person List later ....
            //SearchRelatedMoviesbyPersons((int)facadeView.SelectedListItem.ItemId);
            if (!facadeView.SelectedListItem.IsFolder && !conf.Boolselect)
            // Load Facade with movie actors 
            // ToDo: Load all artists, including producers and directors - add infos about function into facade item2 or 3
            {
              conf.StrIndex = facadeView.SelectedListItem.ItemId;
              conf.StrTIndex = facadeView.SelectedListItem.Label;

              conf.WStrSort = "ACTORS";
              conf.Wselectedlabel = string.Empty;
              conf.WStrSortSens = " ASC";
              BtnSrtBy.IsAscending = true;
              conf.StrActors = "*";
              currentListLevel = Listlevel.Person;
              getSelectFromDivx("TranslatedTitle like '*" + conf.StrTIndex + "*'", conf.WStrSort, conf.WStrSortSens, conf.StrActors, true, string.Empty);
            }
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
            //dlg.DeInit();
            break;
          }

        case "artistdetail":
          LogMyFilms.Debug("artistdetail: searching infos for '" + facadeView.SelectedListItem.Label.ToString() + "'");
          {
            string actorSearchname = MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(facadeView.SelectedListItem.Label.ToString());
            PersonInfo(actorSearchname);
            break;
          }

        case "artistimdbclips": // Example: http://www.imdb.com/name/nm0000206/videogallery
          {
            if (Helper.IsBrowseTheWebAvailableAndEnabled)
            { //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
              string url = ImdbBaseUrl + "";
              string zoom = "150";
              //First search corresponding URL for the actor ...
              Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
              Grabber.MyFilmsIMDB.IMDBUrl wurl;
              _imdb.FindActor(facadeView.SelectedListItem.Label);
              Grabber.MyFilmsIMDBActor imdbActor = new Grabber.MyFilmsIMDBActor();

              if (_imdb.Count > 0)
              {
                wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
                if (wurl.URL.Length != 0)
                {
                  url = wurl.URL + "videogallery"; // Assign proper Webpage for Actorinfos
                  url = ImdbBaseUrl + url.Substring(url.IndexOf("name")); // redirect to base www.imdb.com server and remove localized returns...
                }
                //_imdb.GetActorDetails(_imdb[index], false, out imdbActor); // Details here not needed - we just want the URL !
                LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
                GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false); //54537689
                MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
                GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
                GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
              }
              else
              {
                ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
                break;
              }
            }
            else
            {
              ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
            }
            break;
          }

        case "artistimdbbilder": // Example: http://www.imdb.com/name/nm0000206/mediaindex
          {
            if (Helper.IsBrowseTheWebAvailableAndEnabled)
            { //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
              string url = ImdbBaseUrl + "";
              string zoom = "150";
              //First search corresponding URL for the actor ...
              Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
              Grabber.MyFilmsIMDB.IMDBUrl wurl;
              _imdb.FindActor(facadeView.SelectedListItem.Label);
              Grabber.MyFilmsIMDBActor imdbActor = new Grabber.MyFilmsIMDBActor();

              if (_imdb.Count > 0)
              {
                wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
                if (wurl.URL.Length != 0)
                {
                  url = wurl.URL + "mediaindex"; // Assign proper Webpage for Actorinfos
                  url = ImdbBaseUrl + url.Substring(url.IndexOf("name")); // redirect to base www.imdb.com server and remove localized returns...
                }
                //_imdb.GetActorDetails(_imdb[index], false, out imdbActor); // Details here not needed - we just want the URL !
                LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
                GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
                GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
                MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
                GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false); //54537689
                MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
                GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
                GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
              }
              else
              {
                ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
                break;
              }
            }
            else
            {
              ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
            }
            break;
          }

        case "artistimdbinternet": // Example: http://www.imdb.com/name/nm0000206/

          if (Helper.IsBrowseTheWebAvailableAndEnabled)
          { //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
            string url = ImdbBaseUrl + "";
            string zoom = "150";

            //First search corresponding URL for the actor ...
            Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
            Grabber.MyFilmsIMDB.IMDBUrl wurl;

            _imdb.FindActor(facadeView.SelectedListItem.Label);
            Grabber.MyFilmsIMDBActor imdbActor = new Grabber.MyFilmsIMDBActor();

            if (_imdb.Count > 0)
            {
              //int index = IMDBFetcher.FuzzyMatch(actor);
              //int index;
              //for (index = 0; index < _imdb.Count; ++index)
              //{
              //    wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[index];
              //    if (wurl.URL.Length != 0) url = wurl.URL; // Assign proper Webpage for Actorinfos
              //    _imdb.GetActorDetails(_imdb[index], false, out imdbActor); // Details here not needed - we just want the URL !
              //}
              wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
              if (wurl.URL.Length != 0)
              {
                url = wurl.URL; // Assign proper Webpage for Actorinfos
                //url = ImdbBaseUrl + url.Substring(url.IndexOf(".com" + 4)); // redirect to base www.imdb.com server and remove localized returns...
              }
              //_imdb.GetActorDetails(_imdb[index], false, out imdbActor); // Details here not needed - we just want the URL !
              //Load Webbrowserplugin with the URL
              LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
              GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
              GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
              MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
              GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false); //54537689
              MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
              GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
              GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
            }
            else
            {
              ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798640)); // MyFilmsSystemInformation - no result found
              break;
            }
          }
          else
          {
            ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: 0.27");
          }
          break;

        case "artistimdbfilmlist":
          // ToDo: Launch IMDB-Internetpage via actor-URL in Webbrowserplugin - check InfoPlugin how to implement ...
          // Share loadingclass with actorclips and use parameter "mode"
          {
            break;
          }

        case "updateperson":
          {
            //Todo: add calls to update the personinfos from IMDB - use database and grabberclasses from MePo / Deda
            ArtistIMDBpictures(facadeView.SelectedListItem.Label); // Call Updategrabber with Textlabel/Actorname
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
            dlg.DeInit();


            //First search corresponding URL for the actor ...
            // bool director = false; // Actor is director // Currently not used...
            IMDB _imdb = new IMDB();
            //IMDB.IMDBUrl wurl;
            //newGrab.FindActor(facadeView.SelectedListItem.Label);
            ArrayList actorList = new ArrayList();
            //if (_imdb.Count > 0)
            {
              string actor = facadeView.SelectedListItem.Label;
              //string test = _imdb[0].IMDBURL;
              _imdb.FindActor(actor);
              IMDBActor imdbActor = new IMDBActor();
              string ttt = imdbActor.ThumbnailUrl;
              if (_imdb.Count > 0)
              {
                //int index = IMDBFetcher.FuzzyMatch(actor);
                int index;
                int matchingIndex = -1;
                int matchingDistance = int.MaxValue;
                bool isAmbiguous = false;

                for (index = 0; index < _imdb.Count; ++index)
                {
                  int distance = Levenshtein.Match(actor, _imdb[index].Title);

                  if (distance == matchingDistance && matchingDistance != int.MaxValue)
                  {
                    isAmbiguous = true;
                  }

                  if (distance < matchingDistance)
                  {
                    isAmbiguous = false;
                    matchingDistance = distance;
                    matchingIndex = index;
                  }
                }

                if (isAmbiguous)
                {
                  matchingIndex = 0;
                }


                //LogMyFilms.Info("Getting actor:{0}", _imdb[index].Title);
                //_imdb.GetActorDetails(_imdb[index], director, out imdbActor);

#if MP11
                                //_imdb.GetActorDetails(_imdb[index], out imdbActor);
#else
                //_imdb.GetActorDetails(_imdb[index], out imdbActor);
#endif

                //LogMyFilms.Info("Adding actor:{0}({1}),{2}", imdbActor.Name, actor, percent);
                int actorId = MediaPortal.Video.Database.VideoDatabase.AddActor(imdbActor.Name);
                if (actorId > 0)
                {
                  MediaPortal.Video.Database.VideoDatabase.SetActorInfo(actorId, imdbActor);
                  //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId); // Guzzi: Removed, only updating Actorinfos

                  if (imdbActor.ThumbnailUrl != string.Empty)
                  {
                    string largeCoverArt = Utils.GetLargeCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                    string coverArt = Utils.GetCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                    Utils.FileDelete(largeCoverArt);
                    Utils.FileDelete(coverArt);
                    MediaPortal.Video.Database.IMDBFetcher.DownloadCoverArt(Thumbs.MovieActors, imdbActor.ThumbnailUrl, imdbActor.Name);
                  }
                }
              }
              else
              {
                int actorId = MediaPortal.Video.Database.VideoDatabase.AddActor(actor);
                imdbActor.Name = actor;
                IMDBActor.IMDBActorMovie imdbActorMovie = new IMDBActor.IMDBActorMovie();
                //imdbActorMovie.MovieTitle = _movieDetails.Title;
                //imdbActorMovie.Year = _movieDetails.Year;
                //imdbActorMovie.Role = role;
                imdbActor.Add(imdbActorMovie);
                MediaPortal.Video.Database.VideoDatabase.SetActorInfo(actorId, imdbActor);
                //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);
              }
            }

            MyFilmsDetail.GetActorByName(facadeView.SelectedListItem.Label, actorList);
            //MediaPortal.Video.Database.VideoDatabase.GetActorByName(facadeView.SelectedListItem.Label, actorList);

            if (actorList.Count == 0)
            {
              GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
              dlgOk.SetHeading("Info");
              dlgOk.SetLine(1, string.Empty);
              dlgOk.SetLine(2, "Keine Personen Infos vorhanden !");
              dlgOk.DoModal(GetID);
              return;
            }
            actorID = 0;
            string actorname = string.Empty;
            char[] splitter = { '|' };
            foreach (string act in actorList)
            {
              string[] strActor = act.Split(splitter);
              actorID = Convert.ToInt32(strActor[0]);
              actorname = strActor[1];
            }

            //MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorID);
            //MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(1);
            //if (actor != null)

            //OnVideoArtistInfoGuzzi(actor);



            IMDB GrabArtist = new IMDB();
            //IMDB.IMDBUrl wwurl;

            GrabArtist.FindActor(facadeView.SelectedListItem.Label);

            //int listCount = listUrl.Count;

            //url = new IMDBSearch();
            //MediaPortal.Video.Database.IMDB.IMDBUrl .FindActor(facadeView.SelectedListItem.Label));
            //Load Webbrowserplugin with the URL
            //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
            string url = ImdbBaseUrl + string.Empty;
            string zoom = "100";
            //value = value.Replace("%link%", url);
            //value = value.Replace("%zoom%", zoom);
            LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url.ToString() + "'");
            GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
            GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
            MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
            GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false); //54537689
            MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
            GUIPropertyManager.SetProperty("#btWeb.startup.link", "");
            GUIPropertyManager.SetProperty("#btWeb.link.zoom", "");
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
            MyFilmsDetail.Suppress_Entry((DataRow[])MyFilms.r, (int)facadeView.SelectedListItem.ItemId);
            Fin_Charge_Init(true, true);
          }
          break;
        case "grabber":
          string title = MyFilmsDetail.GetSearchTitle(r, facadeView.SelectedListItem.ItemId, "titleoption");
          string mediapath = mediapath = MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrStorage].ToString();
          if (mediapath.Contains(";")) // take the forst source file
          {
            mediapath = mediapath.Substring(0, mediapath.IndexOf(";"));
          }

          conf.StrIndex = facadeView.SelectedListItem.ItemId;
          conf.StrTIndex = facadeView.SelectedListItem.Label;

          MyFilmsDetail.grabb_Internet_Informations(title, GetID, MyFilms.conf.StrGrabber_ChooseScript, MyFilms.conf.StrGrabber_cnf, mediapath);
          //Fin_Charge_Init(false, true); // Guzzi: This might be required to reload facade and details ?
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          break;

        case "fanart":
          if (!MyFilmsDetail.IsInternetConnectionAvailable()) 
            break; // stop, if no internet available
          else
          {
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            
            string fanartTitle, personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
            fanartTitle = MyFilmsDetail.GetFanartTitle(r[facadeView.SelectedListItem.ItemId], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
            if (!string.IsNullOrEmpty(fanartTitle) && MyFilms.conf.StrFanart)
            {
              LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download Fanart: originaltitle: '" + wtitle + "' - translatedtitle: '" + wttitle + "' - (started from main menu)");
              if (conf.StrPersons && !string.IsNullOrEmpty(conf.StrPathArtist))
              {
                personartworkpath = MyFilms.conf.StrPathArtist;
                LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download PersonArtwork 'enabled' - destination: '" + personartworkpath + "'");
              }
              this.doUpdateMainViewByFinishEvent = true; // makes sure, message handler will be triggered after backgroundthread is finished
              MyFilmsDetail.Download_Backdrops_Fanart(wtitle, wttitle, wftitle, wdirector, wyear.ToString(), true, GetID, fanartTitle, personartworkpath);
            }
          }
          break;

        case "deletefanart":
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079874));//delete fanart (current film)
          dlgYesNo.SetLine(1, "");
          dlgYesNo.SetLine(2, GUILocalizeStrings.Get(433));//confirm suppression
          dlgYesNo.DoModal(GetID);
          if (dlgYesNo.IsConfirmed)
          {
            //MyFilmsDetail.Remove_Backdrops_Fanart(MyFilms.r[facadeView.SelectedListItem.ItemId]["TranslatedTitle"].ToString(), false);
            //string fanartTitle = MyFilms.r[facadeView.SelectedListItem.ItemId][MyFilms.conf.StrTitle1].ToString();  // Fixed, as it should delete content of master title...
            string fanartTitle, personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
            fanartTitle = MyFilmsDetail.GetFanartTitle(r[facadeView.SelectedListItem.ItemId], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
            MyFilmsDetail.Remove_Backdrops_Fanart(fanartTitle, false);

            // reload fanart
            string[] wfanart = MyFilmsDetail.Search_Fanart(facadeView.SelectedListItem.Label, true, "file", false, facadeView.SelectedListItem.ThumbnailImage, string.Empty);
            backdrop.Filename = wfanart[0];
            MyFilmsDetail.setGUIProperty("currentfanart", wfanart[0].ToString());
          }
          break;

        case "togglewatchedstatus":
          // first set index so that details calls don't fail (as they are relying on it)
          conf.StrIndex = facadeView.SelectedListItem.ItemId;
          conf.StrTIndex = facadeView.SelectedListItem.Label;

          if (facadeView.SelectedListItem.IsPlayed)
          {
            facadeView.SelectedListItem.IsPlayed = false;
            // MyFilmsDetail.Watched_Toggle((DataRow[])MyFilms.r, (int)facadeView.SelectedListItem.ItemId, false);
            MyFilmsDetail.Watched_Toggle((int)facadeView.SelectedListItem.ItemId, false);
          }
          else
          {
            facadeView.SelectedListItem.IsPlayed = true;
            // MyFilmsDetail.Watched_Toggle((DataRow[])MyFilms.r, (int)facadeView.SelectedListItem.ItemId, true);
            MyFilmsDetail.Watched_Toggle((int)facadeView.SelectedListItem.ItemId, true);
          }
          //Fin_Charge_Init(true, true);
          //this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          break;

        case "updatemenu":
          GUIDialogMenu dlgupdate = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlgupdate == null) return;
          Context_Menu = true;
          dlgupdate.Reset();
          dlgupdate.SetHeading(GUILocalizeStrings.Get(1079892)); // Update ...


          if (MyFilms.conf.StrSuppress && facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
          {
            dlg.Add(GUILocalizeStrings.Get(432));
            upd_choice[ichoice] = "suppress";
            ichoice++;
          }
          if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
          {
            dlg.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
            upd_choice[ichoice] = "grabber";
            ichoice++;
          }
          if (MyFilms.conf.StrFanart && facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
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
          {
            Context_Menu = false;
            return;
          }
          break;

        case "globalfilters":
          GUIDialogMenu dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg1 == null) return;
          dlg1.Reset();
          dlg1.SetHeading(GUILocalizeStrings.Get(10798689)); // Global Options ...
          System.Collections.Generic.List<string> choiceViewGlobalOptions = new System.Collections.Generic.List<string>();

          // Change global Unwatchedfilteroption
          // if ((MesFilms.conf.CheckWatched) || (MesFilms.conf.StrSupPlayer))// Make it conditoional, so only displayed, if options enabled in setup !
          if (MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798628)));
          if (!MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("globalunwatchedfilter");

          // Change global MovieFilter (Only Movies with media files reachable (requires at least initial scan!)
          if (InitialIsOnlineScan)
          {
            if (GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798628)));
            if (!GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798629)));
            choiceViewGlobalOptions.Add("filterdbisonline");
          }

          // Change global MovieFilter (Only Movies with Trailer)
          if (MyFilms.conf.StrStorageTrailer.Length > 0 && MyFilms.conf.StrStorageTrailer != "(none)") // StrDirStorTrailer only required for extended search
          {
            if (GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798628)));
            if (!GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798629)));
            choiceViewGlobalOptions.Add("filterdbtrailer");
          }

          // Change global MovieFilter (Only Movies with highRating)
          if (GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798628)));
          if (!GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("filterdbrating");

          //// Change Value for global MovieFilter (Only Movies with highRating)
          //dlg1.Add(string.Format(GUILocalizeStrings.Get(10798693), MyFilms.conf.StrAntFilterMinRating.ToString()));
          //choiceViewGlobalOptions.Add("filterdbsetrating");

          //if (MyFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798628)));
          //if (!MyFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798629)));
          //choiceViewGlobalOptions.Add("alwaysdefaultview");

          dlg1.DoModal(GetID);
          if (dlg1.SelectedLabel == -1)
          {
            return;
          }

          LogMyFilms.Debug("Call global menu with option: '" + choiceViewGlobalOptions[dlg1.SelectedLabel].ToString() + "'");

          Change_view(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
          //Context_Menu = false;
          return;

      }
    }

    //*****************************************************************************************
    //*  search related movies by persons                                                     *
    //*****************************************************************************************
    private void SearchRelatedMoviesbyPersons(int Index, bool returnToContextmenu)
    {
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079867)); // menu
      ArrayList w_tableau = new ArrayList();
      System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
      if (MyFilms.r[Index]["Producer"].ToString().Length > 0)
      {
        w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MyFilms.r[Index]["Producer"].ToString())));
        foreach (object t in w_tableau)
        {
          dlg.Add(GUILocalizeStrings.Get(10798612) + " : " + t);
          choiceSearch.Add(t.ToString());
        }
      }
      if (MyFilms.r[Index]["Director"].ToString().Length > 0)
      {
        w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MyFilms.r[Index]["Director"].ToString())));
        foreach (object t in w_tableau)
        {
          dlg.Add(GUILocalizeStrings.Get(1079869) + " : " + t);
          choiceSearch.Add(t.ToString());
        }
      }
      if (MyFilms.r[Index]["Writer"].ToString().Length > 0)
      {
        w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MyFilms.r[Index]["Writer"].ToString())));
        foreach (object t in w_tableau)
        {
          dlg.Add(GUILocalizeStrings.Get(10798684) + " : " + t);
          choiceSearch.Add(t.ToString());
        }
      }
      if (MyFilms.r[Index]["Actors"].ToString().Length > 0)
      {
        w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MyFilms.r[Index]["Actors"].ToString())));
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
      {
        if (returnToContextmenu) // only call if it was called from context menu
          Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
        else 
          Change_Search_Options();
        return;
      }

      string wperson = choiceSearch[dlg.SelectedLabel];
      dlg.Reset();
      choiceSearch.Clear();
      dlg.SetHeading(GUILocalizeStrings.Get(10798611) + wperson); // function selection (actor, director, producer)

      //First add general option to show MP Actor Infos
      if (wperson.Length > 0)
      {
        // First check if actor exists... - this only works with MePo V1.1.5+
        ArrayList actorList = new ArrayList(); // Search with searchName parameter which contain wanted actor name, result(s) is in array which conatin id and name separated with char "|"
        //System.Collections.Generic.List<string> actorList = new System.Collections.Generic.List<string>();
        try
        {
          MyFilmsDetail.GetActorByName(wperson, actorList);
        }
        catch (Exception)
        { }

        if (MyFilmsDetail.ExtendedStartmode("relatedpersonsearch: add person option to dialog menu for personinfodialog"))
        {
          dlg.Add(GUILocalizeStrings.Get(10798731) + " (" + actorList.Count.ToString() + ")");
          choiceSearch.Add("PersonInfo");
        }
      }

      DataRow[] wr = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, "Producer like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
      if (wr.Length > 0)
      {
        dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(10798612) + "  (" + wr.Length + ")");
        choiceSearch.Add("Producer");
      }
      wr = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, "Director like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
      if (wr.Length > 0)
      {
        dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(1079869) + "  (" + wr.Length + ")");
        choiceSearch.Add("Director");
      }
      wr = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, "Writer like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
      if (wr.Length > 0)
      {
        dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(10798684) + "  (" + wr.Length + ")");
        choiceSearch.Add("Writer");
      }
      wr = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, "Actors like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
      if (wr.Length > 0)
      {
        dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(1079868) + "  (" + wr.Length + ")");
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
        string actorSearchname = MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(wperson);
        this.PersonInfo(actorSearchname);
        return;
      }
      conf.StrSelect = choiceSearch[dlg.SelectedLabel].ToString() + " like '*" + wperson + "*'";
      switch (choiceSearch[dlg.SelectedLabel])
      {
        case "Actors":
          conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(1079868) + " [*" + wperson + @"*]"; // "Seletion"
          break;
        case "Director":
          conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(1079869) + " [*" + wperson + @"*]";
          break;
        case "Writer":
          conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(10798684) + " [*" + wperson + @"*]";
          break;
        default:
          conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(10798612) + " [*" + wperson + @"*]";
          break;
      }
      conf.StrTitleSelect = "";
      SetLabelView("search"); // show "search"
      GetFilmList();
    }

    private void PersonInfo(string PersonName)
    {
      IMDBActor imdbActor = new IMDBActor();
      IMDB imdb = new IMDB();
      imdb.FindActor(PersonName);
      for (int i = 0; i < imdb.Count; ++i)
      {
        //if (_guiListItem.ItemId == Movie.DirectorID)
        //{
        //  imdb.GetActorDetails(imdb[i], true, out imdbActor);
        //}
        //else
        //{
        imdb.GetActorDetails(imdb[i], false, out imdbActor);
        //}
        //VideoDatabase.SetActorInfo(_guiListItem.ItemId, imdbActor);
        if (!string.IsNullOrEmpty(imdbActor.ThumbnailUrl))
        {
          break;
        }
      }
      if (imdbActor.ThumbnailUrl != null)
      {
        if (imdbActor.ThumbnailUrl.Length != 0)
        {
          if (imdbActor.ThumbnailUrl.Length != 0 && !string.IsNullOrEmpty(conf.StrPathArtist))
          {
            string filename1person = GrabUtil.DownloadPersonArtwork(conf.StrPathArtist, imdbActor.ThumbnailUrl, PersonName, true, true, out filename1person);
            LogMyFilms.Info("Person Artwork " + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + " downloaded for '" + PersonName + "', path='" + filename1person + "'");
            // DownloadCoverArt(Thumbs.MovieActors, imdbActor.ThumbnailUrl, _actor); // original Deda call
          }
          else
          {
            LogMyFilms.Debug("IMDBFetcher single actor fetch could not be done - no person artwork path defined in MyFilms config");
          }

        }
        else
        {
          LogMyFilms.Debug("IMDBFetcher single actor fetch: url=empty for actor {0}", PersonName);
        }
      }
      else
      {
        Log.Debug("IMDBFetcher single actor fetch: url=null for actor {0}", PersonName);
      }
      //_imdbActor = imdbActor;


      // Add actor to datbbase to get infos in person facades later...
      int actorId = VideoDatabase.AddActor(imdbActor.Name);
      if (actorId > 0)
      {
        VideoDatabase.SetActorInfo(actorId, imdbActor);
        //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);

        //if (imdbActor.ThumbnailUrl != string.Empty)
        //{
        //  string largeCoverArt = Utils.GetLargeCoverArtName(Thumbs.MovieActors, imdbActor.Name);
        //  string coverArt = Utils.GetCoverArtName(Thumbs.MovieActors, imdbActor.Name);
        //  Utils.FileDelete(largeCoverArt);
        //  Utils.FileDelete(coverArt);
        //  //DownloadCoverArt(Thumbs.MovieActors, imdbActor.ThumbnailUrl, imdbActor.Name);
        //}
      }

      //// Last working Guzzi Code
      ////ArrayList actorList = new ArrayList();
      ////// Search with searchName parameter which contain wanted actor name, result(s) is in array
      ////// which conatin id and name separated with char "|"
      ////MyFilmsDetail.GetActorByName(wperson, actorList);

      ////// Check result
      ////if (actorList.Count == 0)
      ////{
      ////  LogMyFilms.Debug("(Person Info): No ActorIDs found for '" + wperson + "'");
      ////  GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      ////  dlgOk.SetHeading("Info");
      ////  dlgOk.SetLine(1, string.Empty);
      ////  dlgOk.SetLine(2, "Keine Personen Infos vorhanden !");
      ////  dlgOk.DoModal(GetID);
      ////  return;
      ////}
      ////LogMyFilms.Debug("(Person Info): " + actorList.Count + " ActorID(s) found for '" + wperson + "'");
      //////int actorID;
      ////actorID = 0;
      ////string actorname = string.Empty;
      ////// Define splitter for string
      ////char[] splitter = { '|' };
      ////// Iterate through list
      ////foreach (string act in actorList)
      ////{
      ////  // Split id from actor name (two substrings, [0] is id and [1] is name)
      ////  string[] strActor = act.Split(splitter);
      ////  // From here we have all what we want, now we can populate datatable, gridview, listview....)
      ////  // actorID originally is integer in the databse (it can be string in results but if we want get details from
      ////  // IMDBActor  GetActorInfo(int idActor) we need integer)
      ////  actorID = Convert.ToInt32(strActor[0]);
      ////  actorname = strActor[1];
      ////  LogMyFilms.Debug("(Person Info): ActorID: '" + actorID + "' with ActorName: '" + actorname + "' found found for '" + wperson + "'");
      ////}

      ////MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorID);
      //////if (actor != null)
      ////OnVideoArtistInfoGuzzi(actor);

      OnVideoArtistInfoGuzzi(imdbActor);
      //OnVideoArtistInfoGuzzi(wperson);
    }

    private void OnVideoArtistInfoGuzzi(MediaPortal.Video.Database.IMDBActor actor)
    {
      MyFilmsActorInfo infoDlg = (MyFilmsActorInfo)GUIWindowManager.GetWindow(ID_MyFilmsActorsInfo);
      LogMyFilms.Debug("(OnVideoArtistInfoGuzzi): Creating (MyFilmsActorInfo)GUIWindowManager.GetWindow(ID_MyFilmsActorsInfo)");
      if (infoDlg == null)
      {
        LogMyFilms.Debug("(OnVideoArtistInfoGuzzi): infoDlg == null -> returning without action");
        return;
      }
      if (actor == null)
      {
        LogMyFilms.Debug("(OnVideoArtistInfoGuzzi): actor == null -> returning without action");
        return;
      }
      infoDlg.Actor = actor;
      infoDlg.DoModal(GetID);
    }

    ////*****************************************************************************************
    ////*  search related movies by properties  (From ZebonsMerge, renamed to ...Zebons)		  *
    ////*****************************************************************************************
    //private void SearchRelatedMoviesbyPropertiesZebons(int Index, IEnumerable<string> wSearchList)
    //{
    //    // first select the property to be searching on

    //    AntMovieCatalog ds = new AntMovieCatalog();
    //    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
    //    System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
    //    GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
    //    ArrayList w_tableau = new ArrayList();
    //    if (dlg == null) return;
    //    dlg.Reset();
    //    dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
    //    foreach (string wSearch in wSearchList)
    //    {
    //        dlg.Add(GUILocalizeStrings.Get(10798617) + "'" + BaseMesFilms.Translate_Column(wSearch) + "'");
    //        choiceSearch.Add(wSearch);
    //    }
    //    dlg.DoModal(GetID);
    //    if (dlg.SelectedLabel == -1)
    //        return;
    //    string wproperty = choiceSearch[dlg.SelectedLabel];
    //    dlg.Reset();
    //    if (choiceSearch.Count == 0)
    //    {
    //        if (dlg1 == null) return;
    //        dlg1.SetHeading(GUILocalizeStrings.Get(10798613));
    //        dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
    //        dlg1.SetLine(2, BaseMesFilms.Translate_Column(wproperty));
    //        dlg1.DoModal(GUIWindowManager.ActiveWindow);
    //        return;
    //    }
    //    choiceSearch.Clear();
    //    if (ds.Movie.Columns[wproperty].DataType.Name == "string")
    //        w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MesFilms.r[Index][wproperty].ToString())));
    //    else
    //        w_tableau.Add(MesFilms.r[Index][wproperty].ToString());
    //    foreach (object t in w_tableau)
    //    {
    //        dlg.Add(wproperty + " : " + t);
    //        choiceSearch.Add(t.ToString());
    //    }
    //    if (choiceSearch.Count == 0)
    //    {
    //        if (dlg1 == null) return;
    //        dlg1.SetHeading(GUILocalizeStrings.Get(10798613));
    //        dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
    //        dlg1.SetLine(2, BaseMesFilms.Translate_Column(wproperty));
    //        dlg1.DoModal(GUIWindowManager.ActiveWindow);
    //        return;
    //    }
    //    dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // property selection
    //    dlg.DoModal(GetID);
    //    if (dlg.SelectedLabel == -1)
    //        return;
    //    if (ds.Movie.Columns[wproperty].DataType.Name == "string")
    //    {
    //        conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel].ToString() + "*'";
    //        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + choiceSearch[dlg.SelectedLabel].ToString() + @"*]";
    //    }
    //    else
    //    {
    //        conf.StrSelect = wproperty + " = '" + choiceSearch[dlg.SelectedLabel].ToString() + "'";
    //        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [" + choiceSearch[dlg.SelectedLabel].ToString() + @"]";
    //    }

    //    conf.StrTitleSelect = "";
    //    GetFilmList();
    //}


    //*****************************************************************************************
    //*  Global search movies by properties (ZebonsMerge - Renamed to ....Zebons)             *
    //*****************************************************************************************
    //     private void SearchMoviesbyPropertiesZebons(IEnumerable<string> wSearchList)
    //     {
    //         // first select the property to be searching on

    //         AntMovieCatalog ds = new AntMovieCatalog();
    //         GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
    //         GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
    //         System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
    //         ArrayList w_tableau = new ArrayList();
    //         ArrayList w_tablabel = new ArrayList();
    //         if (dlg == null) return;
    //         dlg.Reset();
    //         dlg.SetHeading(GUILocalizeStrings.Get(10798615)); // menu
    //         dlg.Add(GUILocalizeStrings.Get(10798616)); // search on all fields
    //         choiceSearch.Add("all");
    //         foreach (string wSearch in wSearchList)
    //         {
    //             dlg.Add(GUILocalizeStrings.Get(10798617) + "'" + BaseMesFilms.Translate_Column(wSearch) + "'");
    //             choiceSearch.Add(wSearch);
    //         }
    //         dlg.DoModal(GetID);
    //         if (dlg.SelectedLabel == -1)
    //             return;
    //         string wproperty = choiceSearch[dlg.SelectedLabel];
    //         VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
    //         if (null == keyboard) return;
    //         keyboard.Reset();
    //         keyboard.Text = "";
    //         keyboard.DoModal(GetID);
    //         if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
    //         {
    //             switch (choiceSearch[dlg.SelectedLabel])
    //             {
    //                 case "all":
    //                     ArrayList w_count = new ArrayList();
    //                     if (dlg == null) return;
    //                     dlg.Reset();
    //                     dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
    //                     DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
    //                     foreach (DataRow wsr in wr)
    //                     {
    //                         foreach (string wsearch in wSearchList)
    //                         {
    //                             if (System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wsr[wsearch].ToString().ToLower())).Contains(keyboard.Text.ToLower()))
    //                                 // column contains text serached on : added to w_tableau + w_count
    //                                 if (w_tableau.Contains(wsearch))
    //                                 // search position in w_tableau for adding +1 to w_count
    //                                 {
    //                                     for (int i = 0; i < w_tableau.Count; i++)
    //                                     {
    //                                         if (w_tableau[i].ToString() == wsearch)
    //                                         {
    //                                             w_count[i] = (int)w_count[i] + 1;
    //                                             break;
    //                                         }
    //                                     }
    //                                 }
    //                                 else
    //                                 // add to w_tableau and move 1 to w_count
    //                                 {
    //                                     w_tableau.Add(wsearch.ToString());
    //                                     w_count.Add(1);
    //                                 }
    //                         }
    //                         //foreach (DataColumn dc in ds.Movie.Columns)
    //                         //{
    //                         //    if (System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wsr[dc.ColumnName].ToString())).Contains(keyboard.Text))
    //                         //        // column contains text serached on : added to w_tableau + w_count
    //                         //        if (w_tableau.Contains(dc.ColumnName))
    //                         //        // search position in w_tableau for adding +1 to w_count
    //                         //        {
    //                         //            for (int i = 0; i < w_tableau.Count; i++)
    //                         //            {
    //                         //                if (w_tableau[i].ToString() == dc.ColumnName)
    //                         //                {
    //                         //                    w_count[i] = (int)w_count[i] + 1;
    //                         //                    break;
    //                         //                }
    //                         //            }
    //                         //        }
    //                         //        else
    //                         //        // add to w_tableau and move 1 to w_count
    //                         //        {
    //                         //            w_tableau.Add(dc.ColumnName.ToString());
    //                         //            w_count.Add(1);
    //                         //        }
    //                         //}
    //                     }
    //                     if (w_tableau.Count == 0)
    //                     {
    //                         if (dlg1 == null) return;
    //                         dlg1.SetHeading(GUILocalizeStrings.Get(10798613));
    //                         dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
    //                         dlg1.SetLine(2, keyboard.Text);
    //                         dlg1.DoModal(GUIWindowManager.ActiveWindow);
    //                         return;
    //                     }
    //                     dlg.Reset();
    //                     dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618),keyboard.Text)); // menu
    //                     choiceSearch.Clear();
    //                     for (int i = 0; i < w_tableau.Count; i++)
    //                     {
    //                         dlg.Add(string.Format(GUILocalizeStrings.Get(10798619), w_count[i], BaseMesFilms.Translate_Column(w_tableau[i].ToString())));
    //                         choiceSearch.Add(w_tableau[i].ToString());
    //                     }
    //                     dlg.DoModal(GetID);
    //                     if (dlg.SelectedLabel == -1)
    //                         return;
    //                     wproperty = choiceSearch[dlg.SelectedLabel];
    //                     if (control_searchText(keyboard.Text))
    //                     {
    //                         if (ds.Movie.Columns[wproperty].DataType.Name == "string")
    //                         {
    //                             conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel].ToString() + "*'";
    //                             conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + choiceSearch[dlg.SelectedLabel].ToString() + @"*]";
    //                         }
    //                         else
    //                         {
    //                             conf.StrSelect = wproperty + " = '" + keyboard.Text + "'";
    //                             conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [" + keyboard.Text + @"]";
    //                         }
    //                     //}
    //                     //if (control_searchText(keyboard.Text))
    //                     //{
    //                     //    if (wproperty == "Rating")
    //                     //        conf.StrSelect = wproperty + " = " + keyboard.Text;
    //                     //    else
    //                     //        if (wproperty == "Number")
    //                     //            conf.StrSelect = wproperty + " = " + keyboard.Text;
    //                     //        else
    //                     //            conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
    //                         //    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + keyboard.Text + @"*]";
    //                         conf.StrTitleSelect = "";
    //                         //                         getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
    //                         GetFilmList();
    //                     }
    //                     break;
    //                 default:
    //                     if (control_searchText(keyboard.Text))
    //                     {
    //                         switch (wproperty)
    //                         {
    //                             case "Rating":
    //                                 conf.StrSelect = wproperty + " = " + keyboard.Text;
    //                                 break;
    //                             case "Number":
    //                                 conf.StrSelect = wproperty + " = " + keyboard.Text;
    //                                 break;
    //                             default:
    //                                 conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
    //                                 break;
    //                         }
    //                         conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + dlg.SelectedLabelText + " [*" + keyboard.Text + @"*]";
    //                         conf.StrTitleSelect = "";
    ////                         getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
    //                         GetFilmList();
    //                     }
    //                     break;
    //             }
    //         }
    //     }


    //*****************************************************************************************
    //*  search related movies by properties  (Guzzi Version)                                 *
    //*****************************************************************************************
    private void SearchRelatedMoviesbyProperties(int Index, bool returnToContextmenu) // (int Index, IEnumerable<string> wSearchList)
    {
      // first select the property to be searching on
      AntMovieCatalog ds = new AntMovieCatalog();
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      //GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
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

      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
      //<entry name="SearchList">TranslatedTitle|OriginalTitle|Description|Comments|Actors|Director|Producer|Year|Date|Category|Country|Rating|Checked|MediaLabel|MediaType|URL|Borrower|Length|VideoFormat|VideoBitrate|AudioFormat|AudioBitrate|Resolution|Framerate|Size|Disks|Languages|Subtitles|Number</entry>
      //<entry name="UpdateList">TranslatedTitle|OriginalTitle|Category|Year|Date|Country|Rating|Checked|MediaLabel|MediaType|Actors|Director|Producer</entry>
      //<entry name="AllItems..">TranslatedTitle|OriginalTitle|FormattedTitle|Description|Comments|Actors|Director|Producer|Rating|Country|Category|Year|Checked|MediaLabel|MediaType|Source|Date|Borrower|Length|URL|VideoFormat|VideoBitrate|AudioFormat|AudioBitrate|Resolution|Framerate|Languages|Subtitles|DateAdded|Size|Disks|Picture|Contents_Id|Number</entry>
      //Sorted lists - manually adding items to have them in right order
      //if (conf.StrTitle1 == "OriginalTitle")
      //{
      //}
      string[] PropertyList = new string[] { "OriginalTitle", "TranslatedTitle", "Description", "Comments", "Actors", "Director", "Producer", "Year", "Date", "Category", "Country", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked", "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate", "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "Source", "Borrower" };
      string[] PropertyListLabel = new string[] { "10798658", "10798659", "10798669", "10798670", "10798667", "10798661", "10798662", "10798665", "10798655", "10798664", "10798663", "10798657", "10798677", "10798678", "10798660", "10798651", "10798652", "10798653", "10798666", "10798671", "10798672", "10798673", "10798674", "10798675", "10798676", "10798680", "10798681", "10798650", "10798668", "10798654", "10798656" };
      // Former order was translated title first ...
      //string[] PropertyList = new string[] { "TranslatedTitle", "OriginalTitle", "Description", "Comments", "Actors", "Director", "Producer", "Year", "Date", "Category", "Country", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked", "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate", "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "Source", "Borrower" };
      //string[] PropertyListLabel = new string[] { "10798659", "10798658", "10798669", "10798670", "10798667", "10798661", "10798662", "10798665", "10798655", "10798664", "10798663", "10798657", "10798677", "10798678", "10798660", "10798651", "10798652", "10798653", "10798666", "10798671", "10798672", "10798673", "10798674", "10798675", "10798676", "10798680", "10798681", "10798650", "10798668", "10798654", "10798656" };
      for (int ii = 0; ii < 31; ii++)
      {
        dlg.Add(GUILocalizeStrings.Get(10798617) + GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))));
        choiceSearch.Add(PropertyList[ii]);
      }

      // Dont use the propertylist...
      //foreach (string wSearch in wSearchList)
      //{
      //    dlg.Add(GUILocalizeStrings.Get(10798617) + BaseMesFilms.Translate_Column(wSearch));
      //    choiceSearch.Add(wSearch);
      //}
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
      {
        if (returnToContextmenu) // only call if it was called from context menu
          Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
        else 
          Change_Search_Options();
        return;
      }
      string wproperty = choiceSearch[dlg.SelectedLabel];
      dlg.Reset();
      choiceSearch.Clear();
      LogMyFilms.Debug("(RelatedPropertySearch): Searchstring in Property: '" + MyFilms.r[Index][wproperty] + "'");
      //PersonTitle Grabbing (Double Words)
      if (wproperty.ToLower() != "description" && wproperty.ToLower() != "comments" && wproperty.ToLower() != "rating")
      {
        w_tableau = Search_String(MyFilms.r[Index][wproperty].ToString());
        foreach (object t in w_tableau)
        {
          for (int ii = 0; ii < 30; ii++)
          {
            if (wproperty.ToLower().Equals(PropertyList[ii].ToLower()))
            {
              dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + ": " + t);
              //dlg.Add(wproperty + " : " + w_tableau[wi]);
              choiceSearch.Add(t.ToString());
              LogMyFilms.Debug("(RelatedPropertySearch): Searchstring Result Add: '" + t + "'");
              break;
            }
          }
        }
      }
      //SubWordGrabbing for more Details, if necessary
      if (wproperty.ToLower() != "description" && wproperty.ToLower() != "comments")
        MinChars = 2;
      else
        MinChars = 5;
      if (MyFilms.r[Index][wproperty].ToString().Length > 0) //To avoid exception in subgrabbing
        wsub_tableau = SubWordGrabbing(MyFilms.r[Index][wproperty].ToString(), MinChars, Filter);
      if ((wproperty.ToLower() == "rating"))
      {
        dlg.Add(GUILocalizeStrings.Get(10798657) + ": = " + MyFilms.r[Index][wproperty].ToString().Replace(",", "."));
        choiceSearch.Add("RatingExact");
        dlg.Add(GUILocalizeStrings.Get(10798657) + ": > " + MyFilms.r[Index][wproperty].ToString().Replace(",", "."));
        choiceSearch.Add("RatingBetter");
      }
      else
      {
        LogMyFilms.Debug("(RelatedPropertySearch): Length: '" + MyFilms.r[Index][wproperty].ToString().Length.ToString() + "'");
        if (MyFilms.r[Index][wproperty].ToString().Length > 0)
        {
          foreach (object t in wsub_tableau)
          {
            if (w_tableau.Contains(t)) // Only Add SubWordItems if not already present in SearchStrin Table
            {
              LogMyFilms.Debug("(RelatedPropertySearch): Searchstring Result already Present: '" + t + "'");
              break;
            }
            else
            {
              for (int ii = 0; ii < 30; ii++)
              {
                if (wproperty.ToLower().Equals(PropertyList[ii].ToLower()))
                {
                  dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + " (" + GUILocalizeStrings.Get(10798627) + "): '" + t + "'");
                  //dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + ": '" + wsub_tableau[wi] + "'");
                  choiceSearch.Add(t.ToString());
                  LogMyFilms.Debug("(RelatedPropertySearch): Searchstring Result Add: '" + t + "'");
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
      LogMyFilms.Debug("(Related Search by properties - ChoiceSearch.Count: " + choiceSearch.Count);
      if (choiceSearch.Count > 0)
      {
        dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // property selection
        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1)
          return;
      }
      else
        dlg.SelectedLabel = 0;
      LogMyFilms.Debug("(Related Search by properties - Selected wproperty: " + wproperty + "'");
      string w_rating = "0";

      if (choiceSearch.Count == 0) //Use Special "is NULL" handling if property is empty ...
        conf.StrSelect = wproperty + " is NULL";
      else
        w_rating = MyFilms.r[Index][wproperty].ToString().Replace(",", ".");
      if ((wproperty == "Rating") && (choiceSearch[dlg.SelectedLabel] == "RatingExact"))
        conf.StrSelect = wproperty.ToString() + " = " + w_rating;
      else
        if ((wproperty == "Rating") && (choiceSearch[dlg.SelectedLabel] == "RatingBetter"))
          conf.StrSelect = wproperty + " > " + w_rating;
        else
          if (wproperty == "Number")
            conf.StrSelect = wproperty + " = " + choiceSearch[dlg.SelectedLabel];
          else
            conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel] + "*'";
      if (choiceSearch.Count == 0)
        //conf.StrTxtSelect = "Selection " + wproperty + "(none)";
        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " (none)";
      else
        //conf.StrTxtSelect = "Selection " + wproperty + " [*" + choiceSearch[dlg.SelectedLabel] + @"*]";
        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + choiceSearch[dlg.SelectedLabel] + @"*]";
      conf.StrTitleSelect = string.Empty;
      SetLabelView("search"); // show "search"
      GetFilmList();
    }

    //******************************************************************************************************
    //*  Global search movies by RANDOM (Random Search with Options, e.g. Trailer, Rating) - Guzzi Version *
    //******************************************************************************************************
    private void SearchMoviesbyRandomWithTrailer(bool returnToContextmenu)
    {
      currentTrailerMoviesList.Clear();
      // first select the area where to make random search on - "all", "category", "year", "country" - then populate the currentTrailerMovieList
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
      {
        if (returnToContextmenu)
          this.Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
        else
          this.Change_Search_Options();
        return;
      }
      string wproperty = choiceSearch[dlg.SelectedLabel];

      ArrayList w_count = new ArrayList();
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
      string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
      DataRow[] wr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
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
              if (GetSubItems)
              {
                // LogMyFilms.Debug("SubItemGrabber: Input: " + wsr[dc.ColumnName]);
                wsub_tableau = SubItemGrabbing(wsr[dc.ColumnName].ToString()); //Grab SubItems
                foreach (object t in wsub_tableau)
                {
                  // LogMyFilms.Debug("SubItemGrabber: Output: " + t);
                  {
                    if (w_tableau.Contains(t.ToString())) // search position in w_tableau for adding +1 to w_count
                    {
                      //if (!w_index.Contains(
                      for (int i = 0; i < w_tableau.Count; i++)
                      {
                        if (w_tableau[i].ToString() == t.ToString())
                        {
                          w_count[i] = (int)w_count[i] + 1;
                          //LogMyFilms.Debug("SubItemGrabber: add Counter for '" + wsub_tableau[wi].ToString() + "'");
                          break;
                        }
                      }
                    }
                    else
                    // add to w_tableau and move 1 to w_count
                    {
                      LogMyFilms.Debug("SubItemGrabber: add new Entry for '" + wsr[dc.ColumnName] + "'");
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
                      //LogMyFilms.Debug("(Guzzi) Class already present, adding Counter for Property: " + dc.ToString() + "Value: '" + wsr[dc.ColumnName].ToString() + "'");
                      break;
                    }
                  }
                }
                else
                // add to w_tableau and move 1 to w_count
                {
                  //LogMyFilms.Debug("AddDistinctClasses with Property: '" + dc.ToString() + "' and Value '" + wsr[dc.ColumnName].ToString() + "'");
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
        LogMyFilms.Debug("PropertyClassCount is 0");
        //return;
      }


      string wproperty2 = "";

      if (wproperty != "randomall")
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

      LogMyFilms.Debug("(RandomMovies) - Chosen Subcategory: '" + wproperty2 + "' selecting in '" + wproperty + "'");
      switch (wproperty)
      {
        case "randomall":
          conf.StrSelect = string.Empty;
          conf.StrTxtSelect = "Selection [*]";
          conf.StrTitleSelect = string.Empty;
          SetLabelView("search"); // show "search"
          break;
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

      GetFilmList(); // load facade with selected group of movies

      LogMyFilms.Debug("(RandomMovies) - resulting conf.StrSelect: '" + conf.StrSelect + "'");
      conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + wproperty2 + @"*]";
      conf.StrTitleSelect = string.Empty;

      // we have now: wproperty = selected category (randomall for all) and wproperty2 = value to search after

      foreach (DataRow sr in r)
      {
        // if trailer present, add to list
        if (!string.IsNullOrEmpty(MyFilms.conf.StrStorageTrailer) && MyFilms.conf.StrStorageTrailer != "(none)" && sr[MyFilms.conf.StrStorageTrailer].ToString().Trim() != "")
        {
          try
          {
            MFMovie movie = new MFMovie();
            movie.Config = Configuration.CurrentConfig; // MF config context
            if (!string.IsNullOrEmpty(sr["Number"].ToString()))
              movie.ID = Int32.Parse(sr["Number"].ToString());
            else movie.ID = 0;
            movie.Year = Int32.Parse(sr["Year"].ToString());
            movie.Title = sr["OriginalTitle"].ToString();

            string mediapath = string.Empty;
            if (!string.IsNullOrEmpty(MyFilms.conf.StrStorageTrailer) && MyFilms.conf.StrStorageTrailer != "(none)")
            {
              mediapath = sr[MyFilms.conf.StrStorageTrailer].ToString();
              if (mediapath.Contains(";")) // take the first source file
                mediapath = mediapath.Substring(0, mediapath.IndexOf(";"));
            }
            movie.File = mediapath;
            movie.DateAdded = sr["Date"].ToString();

            currentTrailerMoviesList.Add(movie);
          }
          catch (Exception mex)
          {
            LogMyFilms.Error("add movie exception - err:{0} stack:{1}", mex.Message, mex.StackTrace);
          }
        }
      }
      // Remove any blocked movies
      //moviesToPlay.RemoveAll(movie => TraktSettings.BlockedFolders.Any(f => movie.Path.Contains(f)));

      //// get the movies that we have watched
      //List<MFMovie> MovieList = (from MFMovie movie in allmovies select movie).ToList();
      //List<MFMovie> movielist = moviesToPlay.Where(m => m.Watched == true).ToList();

      //// Populte currentmovieslist as GUIListItems
      //for (int i = 0; i < facadeView.Count; i++)
      //{
      //}
      
      // we now have a list with movies matching the choice and their index/number value -> now do loop for selection
      LogMyFilms.Debug("(ResultBuildIndex) Found " + currentTrailerMoviesList.Count + " Records matching '" + wproperty2 + "' in '" + wproperty + "'");
      for (int i = 0; i < currentTrailerMoviesList.Count; i++)
        LogMyFilms.Debug("(ResultList) - Index: '" + i + "' - Number: '" + currentTrailerMoviesList[i].ID + "'");
      if (currentTrailerMoviesList.Count == 0)
      {
        ShowMessageDialog(GUILocalizeStrings.Get(10798621), "Suchergebnis: 0", "Keine Filme in der Auswahl vorhanden"); // menu for random search
        return;
      }

      PlayRandomTrailer(false);

      return;

      //Choose Random Movie from Resultlist
      System.Random rnd = new System.Random();
      Int32 RandomNumber = rnd.Next(currentTrailerMoviesList.Count);
      LogMyFilms.Debug("RandomNumber: '" + RandomNumber + "', Record: '" + currentTrailerMoviesList[RandomNumber].ID + "', RandomTitle: '" + currentTrailerMoviesList[RandomNumber].Title + "'");

      //Set Filmlist to random Movie:
      conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = string.Empty; //clear all selects
      conf.WStrSort = conf.StrSTitle;
      conf.Boolselect = false;
      conf.Boolreturn = false;

      conf.StrSelect = "number = " + Convert.ToInt32(currentTrailerMoviesList[RandomNumber].ID);
      conf.StrTxtSelect = "Selection number [" + Convert.ToInt32(currentTrailerMoviesList[RandomNumber].ID) + "]";
      conf.StrTitleSelect = string.Empty;
      //getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
      LogMyFilms.Debug("(Guzzi): Change_View filter - " + "StrSelect: " + conf.StrSelect + " | WStrSort: " + conf.WStrSort);
      SetLabelView("search"); // show "search"
      GetFilmList(); // Added to update facade

      //Set Context to first and only title in facadeview
      facadeView.SelectedListItemIndex = 0; //(Auf ersten und einzigen Film setzen, der dem Suchergebnis entsprechen sollte)
      if (!facadeView.SelectedListItem.IsFolder && !conf.Boolselect)
      // New Window for detailed selected item information
      {
        conf.StrIndex = facadeView.SelectedListItem.ItemId;
        conf.StrTIndex = facadeView.SelectedListItem.Label;
        GUITextureManager.CleanupThumbs();
        //GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
      }
      else
      // View List as selected
      {
        conf.Wselectedlabel = facadeView.SelectedListItem.Label;
        Change_LayOut(MyFilms.conf.StrLayOut);
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

      PlayRandomTrailer(false);

      LogMyFilms.Debug("(SearchRandomWithTrailer-Info): Here should happen the handling of menucontext....");
    }

    public void PlayRandomTrailer(bool showMenu)
    {
      if (showMenu)
      {
        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
        dlgYesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
        //dlgYesNo.SetLine(1, MyFilms.r[Convert.ToInt32(w_index[RandomNumber])]["Originaltitle"].ToString());
        //dlgYesNo.SetLine(2, "Current ID = '" + w_index[RandomNumber] + "'");
        dlgYesNo.SetLine(2, "Und nu ?");
        dlgYesNo.TimeOut = 5;
        dlgYesNo.SetYesLabel("Options");
        dlgYesNo.SetNoLabel("Next Trailer");
        dlgYesNo.SetDefaultToYes(false);
        dlgYesNo.DoModal(ID_MyFilms);
        if (dlgYesNo.IsConfirmed)
          this.TrailerScrobbleOptionsMenu(0);
        //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
      }

      //Choose Random Movie from Resultlist
      System.Random rnd = new System.Random();
      Int32 RandomNumber = rnd.Next(currentTrailerMoviesList.Count);
      LogMyFilms.Debug("RandomNumber: '" + RandomNumber + "', Record: '" + currentTrailerMoviesList[RandomNumber].ID + "', RandomTitle: '" + currentTrailerMoviesList[RandomNumber].Title + "'");
      //for (int i = 0; i < facadeView.Count; i++)
      //{
      //  facadeView[i].Label
      //  // Do something here
      //}


      ArrayList trailer = new ArrayList();
      trailer.Add(currentTrailerMoviesList[RandomNumber].File);
      currentTrailerPlayingItem = currentTrailerMoviesList[RandomNumber];
      trailerscrobbleactive = true;
      MyFilmsDetail.Launch_Movie_Trailer_Scrobbling(trailer, ID_MyFilms); 
    }

    private void TrailerScrobbleOptionsMenu(int currentNumber)
    {
      //// Exit fullscreen Video so we can see main facade again			
      //if (GUIGraphicsContext.IsFullScreenVideo) {
      //  GUIGraphicsContext.IsFullScreenVideo = false;
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
      //Before showing menu, first play the trailer
      //conf.Wselectedlabel = facadeView.SelectedListItem.Label;
      //Change_LayOut(MesFilms.conf.StrLayOut);
      //conf.Boolreturn = true;
      //if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
      //conf.StrTitleSelect += conf.Wselectedlabel;
      //while (GetFilmList() == false) ; //keep calling while single folders found
      //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[currentNumber]), GetID, null);

      while (dlg.SelectedLabel != -1)
      {
        dlg.Reset();
        choiceSearch.Clear();
        //dlg.SetHeading(string.Format(MesFilms.r[Convert.ToInt32(w_index[currentNumber])]["Originaltitle"].ToString())); // menu
        dlg.SetHeading(string.Format("Wählen Sie eine Aktion aus")); // menu
        //if (TrailerIsAvailable) // Später den Eintrag nur anzeigen, wenn auch ein Trailer verfügbar ist (siehe Klassen option)
        dlg.Add(string.Format("Hauptfilm spielen"));
        choiceSearch.Add("PlayMovie");
        dlg.Add(string.Format("Trailer spielen"));
        choiceSearch.Add("PlayMovieTrailer");
        dlg.Add(string.Format("Zeige Filmdetails")); //(goes to MesFilmsDetails ID7987 with selected record to show DetailScreen)
        choiceSearch.Add("ShowMovieDetails");
        dlg.Add(string.Format("Zeige alle Filme des gewählten Bereiches"));
        choiceSearch.Add("ShowMovieList");
        dlg.Add(string.Format("Neue Zufallssuche in gleicher Kategorie"));
        choiceSearch.Add("RepeatSearch");
        dlg.Add(string.Format("Neue globale Zufallssuche"));
        choiceSearch.Add("NewSearch");
        dlg.Add(string.Format("Zurück zum Hauptmenü", "Back"));
        choiceSearch.Add("Back");
        dlg.DoModal(ID_MyFilms);
        if ((dlg.SelectedLabel == -1) || (dlg.SelectedLabel == 6))
          return;

        switch (choiceSearch[dlg.SelectedLabel])
        {
          case "PlayMovie":
            MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
            //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            return;
          case "PlayMovieTrailer":
            //Hier muß irgendwie sichergestellt werden, daß nach Rückkehr keine Neuinitialisierung erfolgt (analog return von Details 7988
            MovieScrobbling = true; //Set True to avoid reload menu after Return ...    
            //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            //conf.Wselectedlabel = facadeView.SelectedListItem.Label;
            //Change_LayOut(MesFilms.conf.StrLayOut);
            //conf.Boolreturn = true;
            //if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
            //conf.StrTitleSelect += conf.Wselectedlabel;
            //while (GetFilmList() == false) ; //keep calling while single folders found

            MyFilmsDetail.trailerPlayed = true;
            MyFilmsDetail.Launch_Movie_Trailer(facadeView.SelectedListItem.ItemId, 7990, null); //7990 To Return to this Dialog
            // MyFilmsDetail.Launch_Movie_Trailer(1, GetID, m_SearchAnimation);
            //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[currentNumber]), GetID, null);    
            //GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dlgYesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
            dlgYesNo.SetLine(1, MyFilms.r[Convert.ToInt32(currentTrailerMoviesList[currentNumber].ID)]["Originaltitle"].ToString());
            dlgYesNo.SetLine(2, "Current ID = '" + currentTrailerMoviesList[currentNumber].ID + "'");
            dlgYesNo.DoModal(GetID);
            if (dlgYesNo.IsConfirmed)
              MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, m_SearchAnimation);
            //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            break;
          case "ShowMovieDetails":
            // New Window for detailed selected item information
            //conf.StrIndex = facadeView.SelectedListItem.ItemId;
            //conf.StrTIndex = facadeView.SelectedListItem.Label;
            GUITextureManager.CleanupThumbs();
            GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
            return;

          case "ShowMovieList":
            //GetFilmList(); //Is this necessary????
            //conf.StrIndex = facadeView.SelectedListItem.ItemId;
            //conf.StrTIndex = facadeView.SelectedListItem.Label;
            GUIControl.FocusControl(ID_MyFilms, (int)Controls.CTRL_List);
            dlg.DeInit();
            return;

          case "RepeatSearch":
            PlayRandomTrailer(false);
            //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            GUIDialogYesNo dlg1YesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dlg1YesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
            dlg1YesNo.SetLine(1, GUILocalizeStrings.Get(219));
            dlg1YesNo.SetLine(2, "Zufällige Film ID = '" + currentTrailerMoviesList[currentNumber].ID + "'");
            dlg1YesNo.DoModal(ID_MyFilms);
            if (dlg1YesNo.IsConfirmed)
              //Launch_Movie(select_item, GetID, m_SearchAnimation);
              MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
            //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            break;
          case "NewSearch":
            SearchMoviesbyRandomWithTrailer(false);
            return;

          case "Back":
            return;

          default:
            {
              break;
            }
        }
      }
      
    }

    //******************************************************************************************************
    //*  Global search movies by RANDOM (Random Search with Options, e.g. Trailer, Rating) - Guzzi Version *
    //******************************************************************************************************
    private void SearchMoviesbyRandomWithTrailerOriginalVersion(bool returnToContextmenu)
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
      {
        if (returnToContextmenu)
          this.Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
        else
          this.Change_Search_Options();
        return;
      }
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
          string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
          DataRow[] wr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
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
                  //LogMyFilms.Debug("(Guzzi) AddDistinctClasses: " + "Property: " + dc.ToString() + " and Value: '" + wsr[dc.ColumnName].ToString() + "'");
                  // column Name contains propertyname : added to w_tableau + w_count
                  if (GetSubItems)
                  {
                    LogMyFilms.Debug("SubItemGrabber: Input: " + wsr[dc.ColumnName]);
                    wsub_tableau = SubItemGrabbing(wsr[dc.ColumnName].ToString()); //Grab SubItems
                    foreach (object t in wsub_tableau)
                    {
                      LogMyFilms.Debug("SubItemGrabber: Output: " + t);
                      {
                        if (w_tableau.Contains(t.ToString())) // search position in w_tableau for adding +1 to w_count
                        {
                          //if (!w_index.Contains(
                          for (int i = 0; i < w_tableau.Count; i++)
                          {
                            if (w_tableau[i].ToString() == t.ToString())
                            {
                              w_count[i] = (int)w_count[i] + 1;
                              //LogMyFilms.Debug("SubItemGrabber: add Counter for '" + wsub_tableau[wi].ToString() + "'");
                              break;
                            }
                          }
                        }
                        else
                        // add to w_tableau and move 1 to w_count
                        {
                          LogMyFilms.Debug("SubItemGrabber: add new Entry for '" + wsr[dc.ColumnName] + "'");
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
                          //LogMyFilms.Debug("(Guzzi) Class already present, adding Counter for Property: " + dc.ToString() + "Value: '" + wsr[dc.ColumnName].ToString() + "'");
                          break;
                        }
                      }
                    }
                    else
                    // add to w_tableau and move 1 to w_count
                    {
                      //LogMyFilms.Debug("AddDistinctClasses with Property: '" + dc.ToString() + "' and Value '" + wsr[dc.ColumnName].ToString() + "'");
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
            LogMyFilms.Debug("PropertyClassCount is 0");
            break;
          }


          string wproperty2 = "";

          if (wproperty != "randomall")
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

          LogMyFilms.Debug("(RandomMovies) - Chosen Subcategory: '" + wproperty2 + "' selecting in '" + wproperty + "'");
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
          LogMyFilms.Debug("(RandomMovies) - resulting conf.StrSelect: '" + conf.StrSelect + "'");
          conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + wproperty2 + @"*]";
          //conf.StrTxtSelect = "Selection " + wproperty + " [*" + wproperty2 + @"*]";
          conf.StrTitleSelect = string.Empty;

          // we have: wproperty = selected category (randomall for all) and wproperty2 = value to search after

          ArrayList w_index = new ArrayList();
          int w_index_count = 0;
          string t_number_id = string.Empty;
          //Now build a list of valid movies in w_index with Number registered
          foreach (DataRow wsr in wr)
          {
            foreach (DataColumn dc in ds.Movie.Columns)
            {
              //LogMyFilms.Debug("dc.ColumnName '" + dc.ColumnName.ToString() + "'");
              if (dc.ColumnName == "Number")
              {
                t_number_id = wsr[dc.ColumnName].ToString();
                //LogMyFilms.Debug("Movienumber stored as '" + t_number_id + "'");
              }
            }
            foreach (DataColumn dc in ds.Movie.Columns)
            {
              if ((wproperty == "randomall") && (dc.ColumnName.ToLower() == "translatedtitle"))
              {
                w_index.Add(t_number_id);
                LogMyFilms.Debug("(RamdomSearch - randomall!!!) - Add MovieIDs to indexlist: dc: '" + dc + "' and Number(ID): '" + t_number_id + "'");
                w_index_count = w_index_count + 1;
              }
              else
                if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623))) // Check, if emptypropertystring is set
                {
                  if ((dc.ColumnName == wproperty) && (wsr[dc.ColumnName].ToString().Length == 0)) // column Name contains propertyname : add movie number (for later selection) to w_index
                  {
                    w_index.Add(t_number_id);
                    LogMyFilms.Debug("(RamdomSearch - (none)!!!) Add MovieIDs to indexlist: dc: '" + dc + "' and Number(ID): '" + t_number_id + "'");
                    w_index_count = w_index_count + 1;
                  }
                }
                else
                {
                  //LogMyFilms.Debug("(searchmatches) - dc '" + dc.ToString() + "' - dc.ColumnName '" + dc.ColumnName.ToString() + "' - wproperty '" + wproperty + "' and Number(ID): '" + t_number_id + "'");
                  if (dc.ColumnName == wproperty)
                  {
                    //LogMyFilms.Debug("(searfhmatches with subitems) property2: '" + wproperty2 + "' - DB-Content: '" + wsr[dc.ColumnName].ToString() + "'"); 
                    if (wsr[dc.ColumnName].ToString().Contains(wproperty2)) // column Name contains propertyname : add movie number (for later selection) to w_index
                    {
                      w_index.Add(t_number_id);
                      LogMyFilms.Debug("(RamdomSearch - Standard) Counter '" + w_index_count + "' Added as '" + w_index[w_index_count] + "'");
                      w_index_count = w_index_count + 1;
                    }
                  }
                }
            }
          }
          // we now have a list with movies matching the choice and their index/number value -> now do loop for selection
          LogMyFilms.Debug("(ResultBuildIndex) Found " + w_index.Count + " Records matching '" + wproperty2 + "' in '" + wproperty + "'");
          for (int i = 0; i < w_index.Count; i++)
            LogMyFilms.Debug("(ResultList) - Index: '" + i + "' - Number: '" + w_index[i] + "'");
          if (w_index.Count == 0)
          {
            ShowMessageDialog(GUILocalizeStrings.Get(10798621), "Suchergebnis: 0", "Keine Filme in der Auswahl vorhanden"); // menu for random search
            return;
          }

          //Choose Random Movie from Resultlist
          System.Random rnd = new System.Random();
          Int32 RandomNumber = rnd.Next(w_index.Count + 1);
          LogMyFilms.Debug("RandomNumber: '" + RandomNumber + "'");
          LogMyFilms.Debug("RandomTitle: '" + RandomNumber + "'");

          //Set Filmlist to random Movie:
          conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = string.Empty; //clear all selects
          conf.WStrSort = conf.StrSTitle;
          conf.Boolselect = false;
          conf.Boolreturn = false;

          conf.StrSelect = "number = " + Convert.ToInt32(w_index[RandomNumber]);
          conf.StrTxtSelect = "Selection number [" + Convert.ToInt32(w_index[RandomNumber]) + "]";
          conf.StrTitleSelect = string.Empty;
          //getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
          LogMyFilms.Debug("(Guzzi): Change_View filter - " + "StrSelect: " + conf.StrSelect + " | WStrSort: " + conf.WStrSort);
          SetLabelView("search"); // show "search"
          GetFilmList(); // Added to update view ????

          //Set Context to first and only title in facadeview
          facadeView.SelectedListItemIndex = 0; //(Auf ersten und einzigen Film setzen, der dem Suchergebnis entsprechen sollte)
          if (!facadeView.SelectedListItem.IsFolder && !conf.Boolselect)
          // New Window for detailed selected item information
          {
            conf.StrIndex = facadeView.SelectedListItem.ItemId;
            conf.StrTIndex = facadeView.SelectedListItem.Label;
            GUITextureManager.CleanupThumbs();
            //GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
          }
          else
          // View List as selected
          {
            conf.Wselectedlabel = facadeView.SelectedListItem.Label;
            Change_LayOut(MyFilms.conf.StrLayOut);
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
          //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);

          while (dlg.SelectedLabel != -1)
          {
            dlg.Reset();
            choiceSearch.Clear();
            //dlg.SetHeading(string.Format(MesFilms.r[Convert.ToInt32(w_index[RandomNumber])]["Originaltitle"].ToString())); // menu
            dlg.SetHeading(string.Format("Wählen Sie eine Aktion aus")); // menu
            //if (TrailerIsAvailable) // Später den Eintrag nur anzeigen, wenn auch ein Trailer verfügbar ist (siehe Klassen option)
            dlg.Add(string.Format("Hauptfilm spielen"));
            choiceSearch.Add("PlayMovie");
            dlg.Add(string.Format("Trailer spielen"));
            choiceSearch.Add("PlayMovieTrailer");
            dlg.Add(string.Format("Zeige Filmdetails")); //(goes to MesFilmsDetails ID7987 with selected record to show DetailScreen)
            choiceSearch.Add("ShowMovieDetails");
            dlg.Add(string.Format("Zeige alle Filme des gewählten Bereiches"));
            choiceSearch.Add("ShowMovieList");
            dlg.Add(string.Format("Neue Zufallssuche in gleicher Kategorie"));
            choiceSearch.Add("RepeatSearch");
            dlg.Add(string.Format("Neue globale Zufallssuche"));
            choiceSearch.Add("NewSearch");
            dlg.Add(string.Format("Zurück zum Hauptmenü", "Back"));
            choiceSearch.Add("Back");
            dlg.DoModal(GetID);
            if ((dlg.SelectedLabel == -1) || (dlg.SelectedLabel == 6))
              return;

            switch (choiceSearch[dlg.SelectedLabel])
            {
              case "PlayMovie":
                MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
                //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                return;
              case "PlayMovieTrailer":
                //Hier muß irgendwie sichergestellt werden, daß nach Rückkehr keine Neuinitialisierung erfolgt (analog return von Details 7988
                MovieScrobbling = true; //Set True to avoid reload menu after Return ...    
                //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                //conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                //Change_LayOut(MesFilms.conf.StrLayOut);
                //conf.Boolreturn = true;
                //if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                //conf.StrTitleSelect += conf.Wselectedlabel;
                //while (GetFilmList() == false) ; //keep calling while single folders found

                MyFilmsDetail.trailerPlayed = true;
                MyFilmsDetail.Launch_Movie_Trailer(facadeView.SelectedListItem.ItemId, 7990, null); //7990 To Return to this Dialog
                // MyFilmsDetail.Launch_Movie_Trailer(1, GetID, m_SearchAnimation);
                //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);    
                //GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                dlgYesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
                dlgYesNo.SetLine(1, MyFilms.r[Convert.ToInt32(w_index[RandomNumber])]["Originaltitle"].ToString());
                dlgYesNo.SetLine(2, "Current ID = '" + w_index[RandomNumber] + "'");
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)
                  MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, m_SearchAnimation);
                //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                break;
              case "ShowMovieDetails":
                // New Window for detailed selected item information
                conf.StrIndex = facadeView.SelectedListItem.ItemId; //Guzzi: Muß hier erst der facadeview geladen werden?
                conf.StrTIndex = facadeView.SelectedListItem.Label;
                GUITextureManager.CleanupThumbs();
                GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
                return;

              case "ShowMovieList":
                //GetFilmList(); //Is this necessary????
                conf.StrIndex = facadeView.SelectedListItem.ItemId; //Guzzi: Muß hier erst der facadeview geladen werden?
                conf.StrTIndex = facadeView.SelectedListItem.Label;
                GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                dlg.DeInit();
                return;

              case "RepeatSearch":
                RandomNumber = rnd.Next(w_index.Count + 1);
                LogMyFilms.Debug("RandomNumber: '" + RandomNumber + "'");
                //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);


                GUIDialogYesNo dlg1YesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                dlg1YesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
                dlg1YesNo.SetLine(1, GUILocalizeStrings.Get(219));
                dlg1YesNo.SetLine(2, "Zufällige Film ID = '" + w_index[RandomNumber] + "'");
                dlg1YesNo.DoModal(GetID);
                if (dlg1YesNo.IsConfirmed)
                  //Launch_Movie(select_item, GetID, m_SearchAnimation);
                  MyFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
                //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                break;
              case "NewSearch":
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetLine(1, string.Empty);
                dlgOk.SetLine(2, "Not yet implemented - be patient ....");
                SearchMoviesbyRandomWithTrailer(false);
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
      LogMyFilms.Debug("(SearchRandomWithTrailer-Info): Here should happen the handling of menucontext....");
    }

    private void SearchMoviesbyAreas(bool returnToContextmenu)
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
      {
        if (returnToContextmenu)
          this.Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
        else
          this.Change_Search_Options();
        return;
      }
      string wproperty = choiceSearch[dlg.SelectedLabel];
      switch (choiceSearch[dlg.SelectedLabel])
      {
        case "randomall":
          conf.StrSelect = string.Empty;
          conf.StrTxtSelect = "Selection [*]";
          conf.StrTitleSelect = string.Empty;
          SetLabelView("search"); // show "search"
          GetFilmList();
          break;

        default:
          ArrayList w_count = new ArrayList();
          if (dlg == null) return;
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
          //Modified to checked for GlobalFilterString
          string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
          DataRow[] wr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
          //DataColumn[] wc = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
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
                  //LogMyFilms.Debug("(Guzzi) AddDistinctClasses: " + "Property: " + dc.ToString() + " and Value: '" + wsr[dc.ColumnName].ToString() + "'");
                  // column Name contains propertyname : added to w_tableau + w_count
                  if (GetSubItems)
                  {
                    LogMyFilms.Debug("SubItemGrabber: Input: " + wsr[dc.ColumnName]);
                    wsub_tableau = SubItemGrabbing(wsr[dc.ColumnName].ToString()); //Grab SubItems
                    foreach (object t in wsub_tableau)
                    {
                      LogMyFilms.Debug("SubItemGrabber: Output: " + t);
                      {
                        if (w_tableau.Contains(t.ToString())) // search position in w_tableau for adding +1 to w_count
                        {
                          //if (!w_index.Contains(
                          for (int i = 0; i < w_tableau.Count; i++)
                          {
                            if (w_tableau[i].ToString() == t.ToString())
                            {
                              w_count[i] = (int)w_count[i] + 1;
                              //LogMyFilms.Debug("SubItemGrabber: add Counter for '" + wsub_tableau[wi].ToString() + "'");
                              break;
                            }
                          }
                        }
                        else
                        // add to w_tableau and move 1 to w_count
                        {
                          LogMyFilms.Debug("SubItemGrabber: add new Entry for '" + wsr[dc.ColumnName] + "'");
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
                          //LogMyFilms.Debug("(Guzzi) Clas already present, adding Counter for Property: " + dc.ToString() + "Value: '" + wsr[dc.ColumnName].ToString() + "'");
                          break;
                        }
                      }
                    }
                    else
                    // add to w_tableau and move 1 to w_count
                    {
                      //LogMyFilms.Debug("(Guzzi) AddDistinctClasses with Property: '" + dc.ToString() + "' and Value '" + wsr[dc.ColumnName].ToString() + "'");
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
            LogMyFilms.Debug("PropertyClassCount is 0");
            break;
          }


          string wproperty2 = "";

          if (wproperty != "randomall")
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

          LogMyFilms.Debug("(RandomMovies) - Chosen Subcategory: '" + wproperty2 + "' selecting in '" + wproperty + "'");
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
          LogMyFilms.Debug("(RandomMovies) - resulting conf.StrSelect: '" + conf.StrSelect + "'");
          //conf.StrTxtSelect = "Selection " + wproperty + " [*" + wproperty2 + @"*]";
          conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + wproperty2 + @"*]";

          conf.StrTitleSelect = string.Empty;

          SetLabelView("search"); // show "search"
          GetFilmList();
          break;
      }
    }

    //*****************************************************************************************
    //*  Global search movies by properties     (Guzzi Version)                               *
    //*****************************************************************************************
    private void SearchMoviesbyProperties(IEnumerable<string> wSearchList, bool returnToContextmenu) // Old hardcoded searchlist: "TranslatedTitle|OriginalTitle|Description|Comments|Actors|Director|Producer|Rating|Year|Date|Category|Country"
    {
      // first select the property to be searching on
      AntMovieCatalog ds = new AntMovieCatalog();
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
      ArrayList w_tableau = new ArrayList();
      //ArrayList w_tablabel = new ArrayList();
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798615)); // menu

      if (MyFilms.SearchHistory.Count > 0) // only show dialog, if there was a searchword stored before ...
      {
        dlg.Add(GUILocalizeStrings.Get(10798609));//last recent searches
        choiceSearch.Add("recentsearch");
      }

      dlg.Add(GUILocalizeStrings.Get(10798616)); // search on all fields
      choiceSearch.Add("all");

      foreach (string wSearch in wSearchList)
      {
        dlg.Add(GUILocalizeStrings.Get(10798617) + BaseMesFilms.Translate_Column(wSearch.Trim()));
        choiceSearch.Add(wSearch.Trim());
      }

      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
      {
        if (returnToContextmenu)
          this.Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
        else 
          this.Change_Search_Options();
          return;
      }

      string wproperty = choiceSearch[dlg.SelectedLabel];
      string searchstring = string.Empty;
      if (wproperty == "recentsearch") // if user choose recent search, set searchname = choice of user instead of asking via vkeyboard
      {
        GUIDialogMenu dlgrecent = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        dlgrecent.Reset();
        dlgrecent.SetHeading(GUILocalizeStrings.Get(10798609)); // Last Searches ...
        if (dlgrecent == null) return;

        for (int i = SearchHistory.Count; i > 0; i--)
        {
          dlg.Add(SearchHistory[i - 1]);
        }
        dlgrecent.DoModal(GetID);
        if (dlg.SelectedId == -1) 
          return;
        // if (dlg.SelectedLabel == -1) return;
        searchstring = dlgrecent.SelectedLabelText;
      }

      VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      if (null == keyboard) return;
      keyboard.Reset();
      keyboard.Text = searchstring;
      keyboard.DoModal(GetID);
      if (keyboard.IsConfirmed && (!string.IsNullOrEmpty(keyboard.Text) || !string.IsNullOrEmpty(searchstring)))
      {
        UpdateRecentSearch(keyboard.Text, 20); // makes sure, searchstring will go to first place, if already present ... maxItems 20
        ArrayList w_count = new ArrayList();
        string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
        switch (wproperty)
        {
          case "all":
          case "recentsearch":
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
            //DataRow[] wr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
            DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
            LogMyFilms.Debug("(GlobalSearchAll) - GlobalFilterString: '" + GlobalFilterString + "' (INACTIVE for SEARCH !!!)");
            LogMyFilms.Debug("(GlobalSearchAll) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
            LogMyFilms.Debug("(GlobalSearchAll) - conf.StrTitle1    : [" + conf.StrTitle1 + " like '*']");
            LogMyFilms.Debug("(GlobalSearchAll) - conf.StrSorta     : '" + conf.StrSorta + "'");
            LogMyFilms.Debug("(GlobalSearchAll) - conf.StrSortSens  : '" + conf.StrSortSens + "'");
            LogMyFilms.Debug("(GlobalSearchAll) - searchStringKBD   : '" + keyboard.Text + "'");
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
                        //LogMyFilms.Debug("(GlobalSearchAll) - AddCount for: '" + i.ToString() + "' - '" + dc.ColumnName.ToString() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                        break;
                      }
                    }
                  }
                  else
                  // add to w_tableau and move 1 to w_count
                  {
                    w_tableau.Add(dc.ColumnName.ToLower());
                    w_count.Add(1);
                    //LogMyFilms.Debug("(GlobalSearchAll) - AddProperty for: '" + dc.ColumnName.ToString().ToLower() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                  }
              }
            }
            LogMyFilms.Debug("(GlobalSearchAll) - Result of Search in all properties (w_tableau.Count): '" + w_tableau.Count + "'");
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
              //LogMyFilms.Debug("(GlobalSearchAll) - OutputSort: Property is '" + PropertyList[ii] + "' - '" + GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + "' (" + PropertyListLabel[ii] + ")");
              for (int i = 0; i < w_tableau.Count; i++)
              {
                //LogMyFilms.Debug("(GlobalSearchAll) - OutputSort: w_tableau is '" + w_tableau[i] + "'"); 
                if (w_tableau[i].ToString().ToLower().Equals(PropertyList[ii].ToLower()))
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

              //LogMyFilms.Debug("(GlobalSearchAll) - ChosenProperty: wproperty is '" + wproperty + "'"); 
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
              //conf.StrTxtSelect = "Selection " + wproperty + " [*" + keyboard.Text + @"*]";
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + keyboard.Text + @"*]";

              conf.StrTitleSelect = string.Empty;
              // getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
              SetLabelView("search"); // show "search"
              GetFilmList();
            }
            break;

          case "all-Zebons":
            //ArrayList w_count = new ArrayList();
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
            DataRow[] wrz = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
            foreach (DataRow wsr in wrz)
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
                    w_tableau.Add(wsearch);
                    w_count.Add(1);
                  }
              }
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
            dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), keyboard.Text)); // menu
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
                conf.StrSelect = wproperty + " like '*" + choiceSearch[dlg.SelectedLabel] + "*'";
                conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + choiceSearch[dlg.SelectedLabel] + @"*]";
              }
              else
              {
                conf.StrSelect = wproperty + " = '" + keyboard.Text + "'";
                conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [" + keyboard.Text + @"]";
              }
              conf.StrTitleSelect = string.Empty;
              // getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
              SetLabelView("search"); // show "search"
              GetFilmList();
            }
            break;

          default:
            LogMyFilms.Debug("(GlobalSearchAll) - ChosenProperty: wproperty is '" + wproperty + "'");
            LogMyFilms.Debug("(GlobalSearchAll) - ChosenProperty: SearchTest is '" + keyboard.Text + "'");
            if (control_searchText(keyboard.Text))
            {
              DataRow[] wdr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
              LogMyFilms.Debug("(GlobalSearchAll) - GlobalFilterString: '" + GlobalFilterString + "' (INACTIVE for SEARCH !!!)");
              LogMyFilms.Debug("(GlobalSearchAll) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
              LogMyFilms.Debug("(GlobalSearchAll) - conf.StrTitle1    : [" + conf.StrTitle1 + " like '*']");
              LogMyFilms.Debug("(GlobalSearchAll) - conf.StrSorta     : '" + conf.StrSorta + "'");
              LogMyFilms.Debug("(GlobalSearchAll) - conf.StrSortSens  : '" + conf.StrSortSens + "'");
              LogMyFilms.Debug("(GlobalSearchAll) - searchStringKBD   : '" + keyboard.Text + "'");
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
                            //LogMyFilms.Debug("(GlobalSearchAll) - AddCount for: '" + i.ToString() + "' - '" + dc.ColumnName.ToString() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                            break;
                          }
                        }
                      }
                      else
                      // add to w_tableau and move 1 to w_count
                      {
                        w_tableau.Add(dc.ColumnName.ToLower());
                        w_count.Add(1);
                        //LogMyFilms.Debug("(GlobalSearchAll) - AddProperty for: '" + dc.ColumnName.ToString().ToLower() + "' - Content found: '" + wsr[dc.ColumnName].ToString() + "'");
                      }
                  }
                }
              }
              LogMyFilms.Debug("(GlobalSearchAll) - Result of Search in all properties (w_tableau.Count): '" + w_tableau.Count + "'");
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
                  //conf.StrSelect = wproperty + " = " + keyboard.Text; // Zebons version
                  conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                  break;
                case "Number":
                  //conf.StrSelect = wproperty + " = " + keyboard.Text; // Zebons Version
                  conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                  break;
                default:
                  conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
                  break;
              }

              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + dlg.SelectedLabelText + " [*" + keyboard.Text + @"*]"; // Zebons Version
              //conf.StrTxtSelect = "Selection " + wproperty + " [*" + keyboard.Text + @"*]"; // Guzzi Version
              conf.StrTitleSelect = "";
              // getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
              SetLabelView("search"); // show "search"
              GetFilmList();
            }
            break;
        }
      }
    }

    //*****************************************************************************************
    //*  Global search movies with missing/empty properties                                   *
    //*****************************************************************************************
    private void SearchIncompleteMovies(IEnumerable<string> wSearchList) // Old hardcoded searchlist: "TranslatedTitle|OriginalTitle|Description|Comments|Actors|Director|Producer|Rating|Year|Date|Category|Country"
    {
      // first select the property to be searching on
      AntMovieCatalog ds = new AntMovieCatalog();
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
      ArrayList w_tableau = new ArrayList();
      ArrayList w_count = new ArrayList();
      string wproperty = string.Empty;

      if (dlg == null) return;
      dlg.Reset();

      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798717)); // incomplete movie data
      DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
      LogMyFilms.Debug("(SearchIncompleteMovies) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
      LogMyFilms.Debug("(SearchIncompleteMovies) - conf.StrTitle1    : [" + conf.StrTitle1 + " like '*']");
      LogMyFilms.Debug("(SearchIncompleteMovies) - conf.StrSorta     : '" + conf.StrSorta + "'");
      LogMyFilms.Debug("(SearchIncompleteMovies) - conf.StrSortSens  : '" + conf.StrSortSens + "'");
      foreach (DataRow wsr in wr)
      {
        foreach (DataColumn dc in ds.Movie.Columns)
        {
          //if (wsr[dc.ColumnName].ToString().ToLower().Contains(keyboard.Text.ToLower()))
          if (wsr[dc.ColumnName].ToString().Length == 0) //Check if field is empty
            if (w_tableau.Contains(dc.ColumnName.ToLower()))
            // search position in w_tableau for adding +1 to w_count
            {
              for (int i = 0; i < w_tableau.Count; i++)
              {
                if (w_tableau[i].ToString() == dc.ColumnName.ToLower())
                {
                  w_count[i] = (int)w_count[i] + 1;
                  break;
                }
              }
            }
            else
            // add to w_tableau and move 1 to w_count
            {
              w_tableau.Add(dc.ColumnName.ToLower());
              w_count.Add(1);
            }
        }
      }
      LogMyFilms.Debug("(SearchIncompleteMovies) - Result of Search in all properties (w_tableau.Count): '" + w_tableau.Count + "'");
      if (w_tableau.Count == 0) // NodeLabelEditEventArgs Results found
      {
        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624)); //InfoPanel
        dlgOk.SetLine(1, GUILocalizeStrings.Get(10798625)); // no result found for searched items
        dlgOk.DoModal(GetID);
        if (dlg.SelectedLabel == -1)
          return;
        return;
      }
      dlg.Reset();
      dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798717))); // incomplete movie data
      choiceSearch.Clear();
      string[] PropertyList = new string[] { "TranslatedTitle", "OriginalTitle", "Description", "Comments", "Actors", "Director", "Producer", "Year", "Date", "Category", "Country", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked", "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate", "Resolution", "Framerate", "Size", "Disks", "Number", "URL" };
      string[] PropertyListLabel = new string[] { "10798659", "10798658", "10798669", "10798670", "10798667", "10798661", "10798662", "10798665", "10798655", "10798664", "10798663", "10798657", "10798677", "10798678", "10798660", "10798651", "10798652", "10798653", "10798666", "10798671", "10798672", "10798673", "10798674", "10798675", "10798676", "10798680", "10798681", "10798650", "10798668" };
      for (int ii = 0; ii < 29; ii++)
      {
        //LogMyFilms.Debug("(GlobalSearchAll) - OutputSort: Property is '" + PropertyList[ii] + "' - '" + GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + "' (" + PropertyListLabel[ii] + ")");
        for (int i = 0; i < w_tableau.Count; i++)
        {
          //LogMyFilms.Debug("(GlobalSearchAll) - OutputSort: w_tableau is '" + w_tableau[i] + "'"); 
          if (w_tableau[i].ToString().ToLower().Equals(PropertyList[ii].ToLower()))
          {
            dlg.Add(string.Format(GUILocalizeStrings.Get(10798718), w_count[i], GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii])))));
            choiceSearch.Add(w_tableau[i].ToString());
          }
        }
      }
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
        return;
      wproperty = choiceSearch[dlg.SelectedLabel];
      LogMyFilms.Debug("(SearchIncompleteMovies) - ChosenProperty is '" + wproperty + "'"); 

      conf.StrSelect = wproperty + " is NULL";
      // conf.StrTxtSelect = "Selection " + wproperty + " [*empty*]";
      conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*empty*]";
      conf.StrTitleSelect = string.Empty;
      SetLabelView("search"); // show "search"
      GetFilmList();
    }

    //*****************************************************************************************
    //*  Update recent used search terms
    //*****************************************************************************************
    private void UpdateRecentSearch(string newsearchstring, int maxItems)
    {
      if (SearchHistory.Contains(newsearchstring.Trim()))
        SearchHistory.Remove(newsearchstring.Trim());
      SearchHistory.Add(newsearchstring.Trim());

      while (SearchHistory.Count > maxItems)
      {
        LogMyFilms.Debug("UpdateRecentSearch: Item limit is: '" + maxItems + "', remove '" + SearchHistory[0] + "' from list");
        SearchHistory.RemoveAt(0);
      }

      string his = "";
      foreach (string s in MyFilms.SearchHistory)
      {
        if (String.IsNullOrEmpty(his))
          his = s;
        else
          his += "|" + s;
      }
      conf.StrSearchHistory = his;
      
      XmlConfig XmlConfig = new XmlConfig();
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "SearchHistory", conf.StrSearchHistory);
    }

    //*****************************************************************************************
    //*  Update userdefined mappings
    //*****************************************************************************************
    private void UpdateUserItems()
    {
      XmlConfig XmlConfig = new XmlConfig();
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem1", MyFilms.conf.Stritem1);
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel1", MyFilms.conf.Strlabel1);
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem2", MyFilms.conf.Stritem2);
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel2", MyFilms.conf.Strlabel2);
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem3", MyFilms.conf.Stritem3);
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel3", MyFilms.conf.Strlabel3);
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem4", MyFilms.conf.Stritem4);
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel4", MyFilms.conf.Strlabel4);
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem5", MyFilms.conf.Stritem5);
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel5", MyFilms.conf.Strlabel5);

      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntTitle1", MyFilms.conf.StrTitle1);
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntTitle2", MyFilms.conf.StrTitle2);
      XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntSTitle", MyFilms.conf.StrSTitle);
    }


    //*****************************************************************************************
    //*  No Movie found to display. Display all movies
    //*****************************************************************************************
    private static void DisplayAllMovies()
    {
      MyFilms.conf.StrFilmSelect = MyFilms.conf.StrTitle1 + " not like ''";
      conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = string.Empty; //clear all selects
      conf.WStrSort = conf.StrSTitle;
      conf.Boolselect = false;
      conf.Boolreturn = false;
      r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, true);
    }

    //*****************************************************************************************
    //*  Initialize Fields on Main screen                                                     *
    //*****************************************************************************************
    private void InitMainScreen(bool log)
    {
      LogMyFilms.Debug("(InitMainScreen) - Initialize all properties !!!");

      MyFilmsDetail.Init_Detailed_DB(log);  // Includes clear of db & user properties

      currentListLevel = Listlevel.None;

      backdrop.Filename = String.Empty;
      filmcover.Filename = String.Empty;
      groupcover.Filename = String.Empty;
      personcover.Filename = String.Empty;

      BtnGlobalOverlayFilter.Label = GUILocalizeStrings.Get(10798714); // Global Filters ...

      MyFilmsDetail.clearGUIProperty("logos_id2001", log);
      MyFilmsDetail.clearGUIProperty("logos_id2002", log);
      MyFilmsDetail.clearGUIProperty("logos_id2003", log);
      MyFilmsDetail.clearGUIProperty("logos_id2012", log); // Combined Logo
      MyFilmsDetail.clearGUIProperty("nbobjects.value", log);
      MyFilmsDetail.clearGUIProperty("Fanart", log);
      MyFilmsDetail.clearGUIProperty("Fanart2", log);
      MyFilmsDetail.clearGUIProperty("db.rating", log);
      MyFilmsDetail.clearGUIProperty("view", log); // Try to properly clean main view when entering
      MyFilmsDetail.clearGUIProperty("select", log);
      LogMyFilms.Debug("(InitMainScreen) - Initialize all properties - Finished !");
    }

    //*****************************************************************************************
    //*  Reset global Filters
    //*****************************************************************************************
    private void InitGlobalFilters(bool log)
    {
      LogMyFilms.Debug("(InitGlobalFilters) - Reset global Filters ...");
      MovieScrobbling = false; //Reset MovieScrobbling
      GlobalFilterStringUnwatched = string.Empty;
      // Will be later initialized from setting MyFilms.conf.GlobalUnwatchedOnly
      MyFilmsDetail.clearGUIProperty("globalfilter.unwatched", log);
      if (!GlobalFilterTrailersOnly)
      {
        GlobalFilterStringTrailersOnly = "";
        MyFilmsDetail.clearGUIProperty("globalfilter.trailersonly", log);
      }

      if (!GlobalFilterIsOnlineOnly)
      {
        GlobalFilterStringIsOnline = "";
        MyFilmsDetail.clearGUIProperty("globalfilter.isonline", log);
      }

      if (!GlobalFilterMinRating)
      {
        GlobalFilterStringMinRating = "";
        MyFilmsDetail.clearGUIProperty("globalfilter.minrating", log);
        MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue", log);
      }
    }

    //*****************************************************************************************
    //*  Ask for Title search and grab information on the NET base on the grab configuration  *
    //*****************************************************************************************
    private void GetTitleGrab()
    {
    }

    private void InitializeBackgroundWorker()
    {
      if (bgUpdateDB == null) bgUpdateDB = new BackgroundWorker();
      bgUpdateDB.DoWork += new DoWorkEventHandler(bgUpdateDB_DoWork);
      bgUpdateDB.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateDB_RunWorkerCompleted);
      bgUpdateDB.WorkerSupportsCancellation = true;
      bgUpdateDB.WorkerReportsProgress = true;
      // bgUpdateDB.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(bgUpdateDB_ProgressChanged);

      if (bgUpdateFanart == null) bgUpdateFanart = new System.ComponentModel.BackgroundWorker();
      bgUpdateFanart.DoWork += new DoWorkEventHandler(bgUpdateFanart_DoWork);
      bgUpdateFanart.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateFanart_RunWorkerCompleted);

      if (bgUpdateActors == null) bgUpdateActors = new System.ComponentModel.BackgroundWorker();
      bgUpdateActors.DoWork += new DoWorkEventHandler(bgUpdateActors_DoWork);
      bgUpdateActors.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateActors_RunWorkerCompleted);

      if (bgUpdateTrailer == null) bgUpdateTrailer = new System.ComponentModel.BackgroundWorker();
      bgUpdateTrailer.DoWork += new DoWorkEventHandler(bgUpdateTrailer_DoWork);
      bgUpdateTrailer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateTrailer_RunWorkerCompleted);

      if (bgLoadMovieList == null) bgLoadMovieList = new System.ComponentModel.BackgroundWorker();
      bgLoadMovieList.DoWork += new DoWorkEventHandler(bgLoadMovieList_DoWork);
      bgLoadMovieList.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgLoadMovieList_RunWorkerCompleted);

      if (bgIsOnlineCheck == null) bgIsOnlineCheck = new System.ComponentModel.BackgroundWorker();
      bgIsOnlineCheck.DoWork += new DoWorkEventHandler(bgIsOnlineCheck_DoWork);
      bgIsOnlineCheck.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgIsOnlineCheck_RunWorkerCompleted);

    }

      private delegate void UpdateWatchTextDelegate(string newText);

      public static void UpdateWatchText(string newText)
      {
          MyFilmsDetail.setGUIProperty("statusmessage", newText, true);
      }

    
      private void InitBSHandler()
      {
        try
        {
          if (!NetworkAvailabilityChanged_Subscribed)
          {
            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;
            NetworkAvailabilityChanged_Subscribed = true;
            LogMyFilms.Debug("InitBSHandler() - Successfully subscribed NetworkAvailabilityChanged Handler !");
          }
        }
        catch (Exception ex)
        {
          NetworkAvailabilityChanged_Subscribed = false;
          LogMyFilms.Debug("InitBSHandler() - Error on initializing NetworkAvailabilityChanged Handler: '" + ex.Message + "', stackstrace: '" + ex.StackTrace + "'");
        }
        try
        {
          if (!PowerModeChanged_Subscribed)
          {
            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            PowerModeChanged_Subscribed = true;
            LogMyFilms.Debug("InitBSHandler() - Successfully subscribed PowerModeChanged Handler !");
          }
        }
        catch (Exception ex)
        {
          PowerModeChanged_Subscribed = false;
          LogMyFilms.Debug("InitBSHandler() - Error on initializing PowerModeChanged Handler: '" + ex.Message + "', stackstrace: '" + ex.StackTrace + "'");
          // throw;
        }
        //// subscribe events for main window trailer scrobble playback
        //if (!PlayerEvents_Subscribed)
        //{
        //  MediaPortal.Player.g_Player.PlayBackEnded += new MediaPortal.Player.g_Player.EndedHandler(OnPlayBackEnded);
        //  MediaPortal.Player.g_Player.PlayBackStopped += new MediaPortal.Player.g_Player.StoppedHandler(OnPlayBackStopped);
        //  PlayerEvents_Subscribed = true;
        //  LogMyFilms.Debug("InitBSHandler() - Successfully subscribed PlayerEvents !");
        //}
      }
  
      private void InitFSwatcher()
      {
        if (FSwatcher.EnableRaisingEvents && FSwatcher.Filter == System.IO.Path.GetFileName(conf.StrFileXml))
          return; // return, if it's already enabled and DB name has not changed
        else
        {
          FSwatcher.EnableRaisingEvents = false;
          FSwatcher.Changed -= new FileSystemEventHandler(FSwatcherChanged);
          FSwatcher.Error -= new ErrorEventHandler(FSwatcherError);
          //FSwatcher.Created -= new FileSystemEventHandler(FSwatcherCreated);
          //FSwatcher.Deleted -= new FileSystemEventHandler(FSwatcherDeleted);
          //FSwatcher.Renamed -= new RenamedEventHandler(FSwatcherRenamed);
        }

        // Init FileSystem Watcher

        // ***** Change this as required
        string path = System.IO.Path.GetDirectoryName(conf.StrFileXml);
        string filename = System.IO.Path.GetFileName(conf.StrFileXml);

        FSwatcher.Path = path;
        FSwatcher.IncludeSubdirectories = false;
        FSwatcher.Filter = filename; // FSwatcher.Filter = "*.xml";
        FSwatcher.NotifyFilter = NotifyFilters.LastWrite; // FSwatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
        //FSwatcher.InternalBufferSize = 64;

        // Add event handlers: 1 for the event raised when a file is created, and 1 for when it detects an error.
        FSwatcher.Changed += new FileSystemEventHandler(FSwatcherChanged);
        FSwatcher.Error += new ErrorEventHandler(FSwatcherError);
        //FSwatcher.Created += new FileSystemEventHandler(FSwatcherCreated);
        //FSwatcher.Deleted += new FileSystemEventHandler(FSwatcherDeleted);
        //FSwatcher.Renamed += new RenamedEventHandler(FSwatcherRenamed);

        FSwatcher.EnableRaisingEvents = true; // Begin watching.
        LogMyFilms.Debug("InitFSwatcher() - FSwatcher started watching - DB-file: '" + conf.StrFileXml + "'");
      }
    
      private void FSwatcherChanged(object source, FileSystemEventArgs e)
      {
        FSwatcher.EnableRaisingEvents = false;

        Thread.Sleep(250);
        //FileInfo objFileInfo = new FileInfo(e.FullPath);
        //if (!objFileInfo.Exists) 
        //  return; // ignore the file changed event

        LogMyFilms.Debug("WatcherChanged() - New FSwatcher Event: " + e.ChangeType + ": '" + e.FullPath + "'");

        if (GUIWindowManager.ActiveWindow != MyFilms.ID_MyFilms)
        {
          // load dataset
          BaseMesFilms.LoadMyFilms(conf.StrFileXml); // Will be automatically loaded, if not yet done - save time on reentering MyFilms GUI !!!
          MyFilmsDetail.SetGlobalLock(false, MyFilms.conf.StrFileXml); // make sure, no global lock is left
          // (re)populate films
          r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens);
        }
        else
        {
          // alternatively RefreshFacade() to be called to also update facade (only when main window is active)
          Refreshfacade(); // loading threaded : Fin_Charge_Init(false, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
        }

        // this.BeginInvoke(new UpdateWatchTextDelegate(UpdateWatchText), "WatcherChanged() - New FSwatcher Event: " + e.ChangeType + ": '" + e.FullPath + "'");
        FSwatcher.EnableRaisingEvents = true;
      }

      private void FSwatcherCreated(object source, FileSystemEventArgs e)
      {
        LogMyFilms.Debug("WatcherCreated() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
      }

      private void FSwatcherDeleted(object source, FileSystemEventArgs e)
      {
        LogMyFilms.Debug("WatcherDeleted() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
      }

      private void FSwatcherRenamed(object source, FileSystemEventArgs e)
      {
        LogMyFilms.Debug("WatcherRenamed() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
      }

      private void FSwatcherError(object source, ErrorEventArgs e) // The error event handler
      {
         Exception watchException = e.GetException();
         LogMyFilms.Debug("WatcherError() - A FileSystemWatcher error has occurred: " + watchException.Message);
         // We need to create new version of the object because the old one is now corrupted
         FSwatcher = new FileSystemWatcher();
         while (!FSwatcher.EnableRaisingEvents)
         {
            try
            {
              InitFSwatcher(); // This will throw an error at the watcher.NotifyFilter line if it can't get the path.
             LogMyFilms.Debug("WatcherError() - FSwatcher restarted after error !");
            }
            catch
            {
              System.Threading.Thread.Sleep(5000); // Sleep for a bit; otherwise, it takes a bit of processor time
            }
         }
      }


    
      void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
      {
        if (e.Mode == Microsoft.Win32.PowerModes.Resume)
        {
          LogMyFilms.Debug("PowerModeChanged() - MyFilms is resuming from standby");
          conf.IsResumeFromStandby = true;

          Thread.Sleep(250);

          int maxretries = 10; // max retries 10 * 500 = 5 seconds
          int i = 0;
          bool success = false; // result of update operation

          while (!success && i < maxretries)
          {
            // first check, if the network is ready and DB is accessible
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() && System.IO.File.Exists(conf.StrFileXml))
            {
              LogMyFilms.Debug("PowerModeChanged() - MyFilms is reloading movie data to memory cache.");
              FSwatcher.EnableRaisingEvents = false;
              if (GUIWindowManager.ActiveWindow != MyFilms.ID_MyFilms)
              {
                // reload data, as it might have changed while sleeping
                BaseMesFilms.LoadMyFilms(conf.StrFileXml); // load dataset
                MyFilmsDetail.SetGlobalLock(false, MyFilms.conf.StrFileXml); // make sure, no global lock is left
                r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // (re)populate films
              }
              else
              {
                Refreshfacade(); // loading threaded : Fin_Charge_Init(false, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
              }
              success = true;
            }
            else
            {
              i += 1;
              LogMyFilms.Info("PowerModeChanged() - Network not yet ready or file not accessible on try '" + i + " of " + maxretries + "' to reload - waiting for next retry");
              Thread.Sleep(500);
            }
          }
          try
          {
            FSwatcher.EnableRaisingEvents = true;
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("PowerModeChanged()- FSwatcher - problem enabling Raisingevents - Message:  '" + ex.Message);
          }
        }

        else if (e.Mode == Microsoft.Win32.PowerModes.Suspend)
        {
          LogMyFilms.Debug("PowerModeChanged() - MyFilms is entering standby");
        }
        else
          LogMyFilms.Debug("PowerModeChanged() - MyFilms detected unhandled PowerModeChanged event - no action.");
      }

      void NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
      {
        if (e.IsAvailable)
        {
          LogMyFilms.Debug("MyFilms is connected to the network");
          conf.IsNetworkAvailable = true;
        }
        else
        {
          LogMyFilms.Debug("MyFilms is disconnected from the network");
          // DBOnlineMirror.IsMirrorsAvailable = false; // Force to recheck later
          conf.IsNetworkAvailable = false;
        }
      }

      private void OnPlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename)
      {
        LogMyFilms.Debug("OnPlayBackEnded was initiated - filename: '" + filename + "'");
        OnPlayEndAction(type, 0, filename, true, false);
      }
      private void OnPlayBackStopped(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename)
      {
        LogMyFilms.Debug("OnPlayBackStopped was initiated - filename: '" + filename + "'");
        OnPlayEndAction(type, timeMovieStopped, filename, false, true);
      }

      private void OnPlayEndAction(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename, bool ended, bool stopped)
      {
        if (currentTrailerMoviesList.Count > 0 && currentTrailerPlayingItem != null)
        {
          if (filename == currentTrailerPlayingItem.File && type == MediaPortal.Player.g_Player.MediaType.Video) // is it the last played trailer?
          {
            LogMyFilms.Debug("OnPlayEndAction is launching PlayRandomTrailer()");
            currentTrailerPlayingItem = null;
            PlayRandomTrailer(true); // show GUI dialog for user selection
          }
          else
          {
            // some other playback ended, and a playlist is still set here -> clear it
            currentTrailerMoviesList.Clear();
            currentTrailerPlayingItem = null;
          }
        }
        else
        {
          currentTrailerPlayingItem = null;
        }
      }


    //*****************************************************************************************
    //*  Update Database in batch mode                                                        *
    //*****************************************************************************************
    public void AsynUpdateDatabase()
    {
      if (!bgUpdateDB.IsBusy && !bgUpdateDB.CancellationPending)
      {
        MyFilmsDetail.SetGlobalLock(true, MyFilms.conf.StrFileXml); // also disabled local FSwatcher
        bgUpdateDB.RunWorkerAsync(MyFilms.conf.StrTIndex);
        MyFilmsDetail.setGUIProperty("statusmessage", "global update active", false);
        if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
        {
          MyFilmsDetail.ShowNotificationDialog("MyFilms Info ...", "Global Update started !");
        }
        LogMyFilms.Info("AsynUpdateDatabase() - Launching AMCUpdater in batch mode");
      }
      else
        LogMyFilms.Info("AsynUpdateDatabase() - AMCUpdater cannot be started in batch mode - either already running or cancellation pending");
    }

    void bgUpdateDB_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;
      // MyFilmsDetail.RunAMCupdater(Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCUpdater.exe", "\"" + MyFilms.conf.StrAMCUpd_cnf + "\" \"" + MediaPortal.Configuration.Config.GetDirectoryInfo(Config.Dir.Log) + "\""); // Add Logpath to commandlineparameters
      string exeName = Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCUpdater.exe"; 
      string argsLine = "\"" + MyFilms.conf.StrAMCUpd_cnf + "\" \"" + MediaPortal.Configuration.Config.GetDirectoryInfo(Config.Dir.Log) + "\"";
      //static public void RunAMCupdater(string exeName, string argsLine)
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
              // psi.RedirectStandardOutput = true; // redirect output to streamreader - ToDo: add reader to use and add console output to AMCU !
              p.StartInfo = psi;
              LogMyFilms.Debug("RunAMCupdater - Starting external command: {0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);
              try
              {
                p.Start();
                while (!p.HasExited)
                {
                  if (bgUpdateDB.CancellationPending)
                  {
                    LogMyFilms.Debug("RunAMCupdater - Cancellation requested by user - try to kill process 'AMCupdater.exe' !");
                    e.Cancel = true;
                    p.Kill();
                    p.WaitForExit();
                    //Process[] aProc = System.Diagnostics.Process.GetProcessesByName("AMCupdater");
                    //if (aProc.Length > 0) {  Process myprc = aProc[0]; myprc.Kill(); }
                  }
                  Thread.Sleep(1000);
                }
              }
              catch (Exception ex)
              {
                  LogMyFilms.Debug(ex.Message.ToString());
              }
              LogMyFilms.Debug("RunAMCupdater - External command finished");
          }
      }
    }

    void bgUpdateDB_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      if (e.Cancelled)
      {
        LogMyFilms.Info("RunAMCupdater - Update database with AMCUpdater cancelled by user request. (GetID = '" + GetID + "')");
        if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
        {
          MyFilmsDetail.ShowNotificationDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(1079856));
          // ShowMessageDialog(GUILocalizeStrings.Get(1079861), "", GUILocalizeStrings.Get(1079856)); // Global Update was cancelled !
        }
      }
      else
      {
        LogMyFilms.Info("RunAMCupdater - Update database with AMCUpdater sucessfully finished. (GetID = '" + GetID + "')");
        if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
          {
            MyFilmsDetail.ShowNotificationDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(10798748));
            // ShowMessageDialog(GUILocalizeStrings.Get(1079861), "", GUILocalizeStrings.Get(10798748)); // Global Update finished !
          }
      }
      MyFilmsDetail.clearGUIProperty("statusmessage");

      Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);

      Load_Config(Configuration.CurrentConfig, true, null);
      Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
      MyFilmsDetail.SetGlobalLock(false, MyFilms.conf.StrFileXml);
    }

    //*****************************************************************************************
    //*  Download Backdrop Fanart in Batch mode                                               *
    //*****************************************************************************************
    public void AsynUpdateFanart()
    {
      if (!bgUpdateFanart.IsBusy)
      {
        bgUpdateFanart.RunWorkerAsync(MyFilms.r);
        LogMyFilms.Info(": Downloading backdrop fanart in batch mode");
      }
    }

    static void bgUpdateFanart_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;
      Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
      string wtitle = string.Empty;
      string personartworkpath = string.Empty;
      if (MyFilms.conf.StrPersons && !string.IsNullOrEmpty(MyFilms.conf.StrPathArtist)) // if persoin artwork path present and person thumbs enabled, also load person images
      {
        personartworkpath = MyFilms.conf.StrPathArtist;
        LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download PersonArtwork 'enabled' - destination: '" + personartworkpath + "'");
      }
      foreach (DataRow t in MyFilms.r)
      {
        if (t["OriginalTitle"] != null && t["OriginalTitle"].ToString().Length > 0)
          wtitle = t["OriginalTitle"].ToString();
        if (wtitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
          wtitle = wtitle.Substring(wtitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
        string wttitle = string.Empty;
        if (t["TranslatedTitle"] != null && t["TranslatedTitle"].ToString().Length > 0)
          wttitle = t["TranslatedTitle"].ToString();
        if (wttitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
          wttitle = wttitle.Substring(wttitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
        if (t["OriginalTitle"].ToString().Length > 0)
        {
          int wyear = 0;
          try { wyear = System.Convert.ToInt16(t["Year"]); }
          catch { }
          string wdirector = string.Empty;
          try { wdirector = (string)t["Director"]; }
          catch { }
          System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, wdirector, MyFilms.conf.StrPathFanart, true, false, MyFilms.conf.StrTitle1.ToString(), personartworkpath);
        }
      }
    }

    static void bgUpdateFanart_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("Backdrop Fanart download finished");
    }


    //*****************************************************************************************
    //*  Download Actors Artwork in Batch mode                                               *
    //*****************************************************************************************
    public void AsynUpdateActors(ArrayList actors)
    {
      if (!bgUpdateActors.IsBusy)
      {
        bgUpdateActors.RunWorkerAsync(actors);
        LogMyFilms.Info(": Downloading actors artwork in batch mode");
      }
    }

    static void bgUpdateActors_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;
      Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
      string wtitle = string.Empty;
      //foreach (string actor in actors)
      //{
      //  if (t["OriginalTitle"] != null && t["OriginalTitle"].ToString().Length > 0)
      //    wtitle = t["OriginalTitle"].ToString();
      //  if (wtitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
      //    wtitle = wtitle.Substring(wtitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
      //  string wttitle = string.Empty;
      //  if (t["TranslatedTitle"] != null && t["TranslatedTitle"].ToString().Length > 0)
      //    wttitle = t["TranslatedTitle"].ToString();
      //  if (wttitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
      //    wttitle = wttitle.Substring(wttitle.IndexOf(MyFilms.conf.TitleDelim) + 1);
      //  if (t["OriginalTitle"].ToString().Length > 0)
      //  {
      //    int wyear = 0;
      //    try { wyear = System.Convert.ToInt16(t["Year"]); }
      //    catch { }
      //    string wdirector = string.Empty;
      //    try { wdirector = (string)t["Director"]; }
      //    catch { }
      //    System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, wdirector, conf.StrPathFanart, true, false, conf.StrTitle1);
      //  }
      //}
    }

    static void bgUpdateActors_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("actors artwork download finished");
    }


    //*****************************************************************************************
    //*  Load List movie file in batch mode                                                   *
    //*****************************************************************************************
    public void AsynLoadMovieList()
    {
      if (!bgLoadMovieList.IsBusy)
      {
        LogMyFilms.Info("AsynLoadMovieList() - Loading Movie List in batch mode");
        bgLoadMovieList.RunWorkerAsync();
      }
      else
        LogMyFilms.Debug(("AsynLoadMovieList() could not be launched because already running !"));
    }

    static void bgLoadMovieList_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;
      string searchrep = MyFilms.conf.StrDirStor;
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
      System.Text.RegularExpressions.Regex oRegex = new System.Text.RegularExpressions.Regex(";");
      string[] SearchDir = oRegex.Split(searchrep);
      foreach (string path in SearchDir)
      {
        MyFilms.conf.MovieList.Add(System.IO.Directory.GetFiles(path));
        if ((MyFilms.conf.SearchSubDirs == "no") || (!System.IO.Directory.Exists(path))) continue;
        foreach (string sFolderSub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
        {
          MyFilms.conf.MovieList.Add(System.IO.Directory.GetFiles(sFolderSub));
        }
      }
    }

    static void bgLoadMovieList_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("Loading Movie List in batch mode finished");
    }

    public void AMCupdaterStartEventHandler()
    {
        // AMCUpdater.AntProcessor.bgwFolderScanUpdate.ProgressChanged += new ProgressChangedEventHandler(AMCupdater_ProgressChanged);
        // AMCUpdater.AntProcessor.bgwFolderScanUpdate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(AMCupdater_RunWorkerCompleted);
    }

    void AMCupdater_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
    {
      LogMyFilms.Info("AMCupdater_ProgressChanged. (GetID = '" + GetID + "', Message: " + e.UserState + ", Progress: " + e.ProgressPercentage + ")");
      if (GetID == ID_MyFilms)
      {
        MyFilmsDetail.setGUIProperty("statusmessage", "AMCupdater Running - Importing Movies");
      }
    }
    void AMCupdater_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("AMCupdater RunWorkerCompleted");
      MyFilmsDetail.clearGUIProperty("statusmessage");
      if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
      {
        MyFilmsDetail.ShowNotificationDialog("MyFilms Info ...", "Global Update finished !");
      }
    }

    //*****************************************************************************************
    //*  Check availability status of media files in batch mode                                                   *
    //*****************************************************************************************
    public void AsynIsOnlineCheck()
    {
      if (!bgIsOnlineCheck.IsBusy)
      {
        LogMyFilms.Info("AsynIsOnlineCheck() - Check IsOnline  started in batch mode");
        MyFilmsDetail.setGUIProperty("statusmessage", "Availability Scanner started ...", false);
        bgIsOnlineCheck.RunWorkerAsync(MyFilms.r);
      }
      else
        LogMyFilms.Debug(("AsynIsOnlineCheck() could not be launched because already running !"));
    }

    static void bgIsOnlineCheck_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;
      DateTime startTime = DateTime.Now;
      Regex oRegex = new Regex(";"); 
      LogMyFilms.Info("bgIsOnlineCheck_DoWork: Now checking Online Status - Source field <film> is: '" + conf.StrStorage + "' - Source field <trailer> is: '" + conf.StrStorageTrailer + "'");

      // Check Config
      bool filmSource = true;
      if (string.IsNullOrEmpty(conf.StrStorage) || conf.StrStorage == "(none)") // no source path set?
        filmSource = false;
      bool filmSearch = true;
      if (string.IsNullOrEmpty(conf.ItemSearchFile) || string.IsNullOrEmpty(MyFilms.conf.SearchFile) || conf.SearchFile == "False" || conf.SearchFile == "no")
        filmSearch = false;
      bool trailerSource = true;
      if (string.IsNullOrEmpty(MyFilms.conf.StrStorageTrailer) || MyFilms.conf.StrStorageTrailer == "(none)")
        trailerSource = false;
      bool trailerSearch = true;
      if (string.IsNullOrEmpty(conf.ItemSearchFileTrailer) || string.IsNullOrEmpty(MyFilms.conf.SearchFileTrailer) || conf.SearchFileTrailer == "False" || conf.SearchFileTrailer == "no")
        trailerSearch = false;
      LogMyFilms.Debug("bgIsOnlineCheck_DoWork: filmSource = '" + filmSource + "', filmSearch = '" + filmSearch + "', trailerSource = '" + trailerSource + "', trailerSearch = '" + trailerSearch + "'");

      // Build MovieList (files)
      if (filmSearch) // if search by filename is activated in setup ...
      {
        LogMyFilms.Debug("bgIsOnlineCheck_DoWork: Now checking Searchpathes, adding CDrom(s) and build MovieList ...");
        string searchrep = conf.StrDirStor; // Searchpath for movies 
        DriveInfo[] allDrives = DriveInfo.GetDrives(); // get local drives to find CDrom(s)
        foreach (DriveInfo d in allDrives)
        {
          if ((d.DriveType.ToString() == "CDRom") && d.IsReady) // if drive is a CDrom and is "ready/media present" add it to search pathes
          {
            if (searchrep.Length > 0) searchrep = searchrep + ";" + d.Name;
            else searchrep = d.Name;
          }
        }
        LogMyFilms.Debug("bgIsOnlineCheck_DoWork: Resulting Searchpathes after adding active CDrom(s): '" + searchrep + "'");
        
        try
        {
          string[] SearchDir = oRegex.Split(searchrep);
          foreach (string path in SearchDir)
          {
            if (System.IO.Directory.Exists(path))
            {
              MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner - GetDir: '" + path + "'");
              conf.MovieList.Add(System.IO.Directory.GetFiles(path));
              if (MyFilms.conf.SearchSubDirs == "no") continue;
              foreach (string sFolderSub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
              {
                MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner - GetDir: '" + sFolderSub + "'");
                conf.MovieList.Add(System.IO.Directory.GetFiles(sFolderSub));
              }
            }
            else
            {
              LogMyFilms.Error("bgIsOnlineCheck_DoWork: Search Directory (Movie) not found: '" + path + "' - check your config!");
            }
          }
        }
        catch (Exception ex) {LogMyFilms.Debug("bgIsOnlineCheck_DoWork: SearchDir exception: " + ex.Message + ", " + ex.StackTrace.ToString());}
        LogMyFilms.Debug("bgIsOnlineCheck_DoWork: Movie Files found: '" + conf.MovieList.Count + "'");
      }

      // Build TrailerList (files)
      if (trailerSearch) // if search by filename is activated in setup ...
      {
        LogMyFilms.Info("bgIsOnlineCheck_DoWork: Now checking Searchpathes for Trailers and build TrailerList ...");
        string searchrepTrailer = conf.StrDirStorTrailer; // Searchpath for trailer 
        string[] SearchDirTrailer = oRegex.Split(searchrepTrailer);
        foreach (string path in SearchDirTrailer)
        {
          if (Directory.Exists(path))
          {
            MyFilms.conf.TrailerList.Add(System.IO.Directory.GetFiles(path));
            if (MyFilms.conf.SearchSubDirsTrailer == "no") continue;
            foreach (string sFolderSub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
              conf.TrailerList.Add(System.IO.Directory.GetFiles(sFolderSub));
            }
          }
          else
          {
            LogMyFilms.Error("bgIsOnlineCheck_DoWork: Search Directory (Trailer) not found: '" + path + "' - check your config!");
          }
        }
        LogMyFilms.Debug("bgIsOnlineCheck_DoWork: Trailer Files found: '" + conf.TrailerList.Count + "'");
      }

      LogMyFilms.Info("bgIsOnlineCheck_DoWork: Read MyFilms Database ...");
      DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens, true);
      string TotalRecords = wr.Count().ToString();
      LogMyFilms.Debug("bgIsOnlineCheck_DoWork: DB-Records found: '" + TotalRecords + "'");

      // Now scan for availability
      int counter = 0;
      foreach (DataRow t in wr)
      {
        counter++;
        // Check Movie availability
        if (!filmSource && !filmSearch)
        {
          //LogMyFilms.Error("bgIsOnlineCheck_DoWork: Error checking Movie Online Status - Source or Search not properly defined in setup!");
        }
        else 
        {
          bool isonline = true; // set true as default
          string fileName = string.Empty;

          if (filmSource)
          {
            try
            {
              fileName = t[MyFilms.conf.StrStorage].ToString().Trim();
            }
            catch (Exception ex)
            {
              LogMyFilms.Error("bgIsOnlineCheck_DoWork: Error getting source media files from DB - exception: " + ex.Message);
              fileName = string.Empty;
            }
            //fileName = fileName.Substring(0, fileName.LastIndexOf(";")).Trim();
            string[] Mediafiles = oRegex.Split(fileName);
            foreach (string mediafile in Mediafiles)
            {
              if (mediafile.Length > 0 && System.IO.File.Exists(mediafile))
              {
                // isonline = true;
                LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SOURCE check   media AVAILABLE for title '" + t[conf.StrTitle1] + "' - file: '" + mediafile + "'");
              }
              else
              {
                isonline = false;
                LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SOURCE check   media NOT AVAILABLE for title '" + t[conf.StrTitle1] + "' - file: '" + mediafile + "'");
              }
            }
          }
          if (!isonline && filmSearch) // if movie not found via source and search is enabled...
          {
              string movieName = t[MyFilms.conf.ItemSearchFile].ToString();
              movieName = movieName.Substring(movieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim().ToLower();
              string[] result = conf.MovieList.Find(
              delegate(string[] files)
              {
                return files.Where(n => n.ToLower().Contains(@"\" + movieName + @".") || n.ToLower().Contains(@"\" + movieName + @"\")).Count() > 0;
              }
              );
              if (result != null)
              {
                foreach (string file in result)
                {
                  if (string.IsNullOrEmpty(file.Trim())) continue;
                  if (MediaPortal.Util.Utils.IsVideo(file))
                  {
                    isonline = true;
                    LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SEARCH check   file FOUND      for title '" + t[conf.StrTitle1] + "' - file found: '" + file + "'");
                  }
                }
              }
              else
              {
                isonline = false;
                LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SEARCH check   media NOT AVAILABLE for title '" + t[conf.StrTitle1] + "' - file found: (none)");
              }
              
              //MyFilmsDetail.result = new ArrayList();
              //MyFilmsDetail.SearchFiles(movieName, conf.StrDirStor, false, false);
              //if (MyFilmsDetail.result.Count != 0) // found!
              //{
              //  // isonline = true;
              //  LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SEARCH check   file FOUND      for title '" + t[conf.StrTitle1] + "' - file found: '" + MyFilmsDetail.result[0].ToString().Trim() + "'");
              //}
              //else
              //{
              //  isonline = false;
              //  LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SEARCH check   media NOT AVAILABLE for title '" + t[conf.StrTitle1] + "' - file found: (none)");
              //}
          }
          t["IsOnline"] = isonline.ToString();
          MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner - (" + counter + " of " + TotalRecords + ") - Film: '" + t[conf.StrTitle1] + "'", false);
        }

        // Check Trailer availability
        if (!trailerSource && !trailerSearch)
        {
          //LogMyFilms.Error("bgIsOnlineCheck_DoWork: Error checking Trailer Online Status - Source or Search not properly defined in setup!");
        }
        else
        {
          bool isonline = true; // set true as default - even if it might not be like that ...
          string fileName = string.Empty;

          if (trailerSource)
          {
            try {fileName = t[MyFilms.conf.StrStorageTrailer].ToString().Trim();}
            catch {fileName = string.Empty;}

            string[] Mediafiles = oRegex.Split(fileName);
            foreach (string mediafile in Mediafiles)
            {
              if (mediafile.Length > 0 && System.IO.File.Exists(mediafile))
              {
                LogMyFilms.Debug("bgIsOnlineCheck_DoWork - trailer (" + counter + ") media AVAILABLE for title '" + t[conf.StrTitle1] + "' - file: '" + mediafile + "'");
              }
              else
              {
                isonline = false;
                LogMyFilms.Debug("bgIsOnlineCheck_DoWork - trailer (" + counter + ") media NOT AVAILABLE for title '" + t[conf.StrTitle1] + "' - file: '" + mediafile + "'");
              }
            }
          }
          if (!isonline && trailerSearch)
          {
            // Currently no autosearch implemented
          }
          t["IsOnlineTrailer"] = isonline.ToString();
          MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner (" + counter + " of " + TotalRecords + " - film): '" + t[conf.StrTitle1] + "'", false);
        }
      }
      DateTime stopTime = DateTime.Now;
      TimeSpan duration = stopTime - startTime;
      LogMyFilms.Info("bgIsOnlineCheck_DoWork - total runtime was: '" + duration + "', runtime in seconds: '" + duration.TotalSeconds + "'");
      MyFilmsDetail.Update_XML_database();
    }

    void bgIsOnlineCheck_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("Check IsOnline in batch mode finished. (GetID = '" + GetID + "')");
      if (GetID == ID_MyFilms)
      {
        //Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
        //Load_Config(Configuration.CurrentConfig, true);
        InitialIsOnlineScan = true; // let MF know, the status has been retrieved !
        Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
        MyFilmsDetail.clearGUIProperty("statusmessage");
      }
    }


    //*****************************************************************************************
    //*  Search and register Trailers in Batch mode                                               *
    //*****************************************************************************************
    public void AsynUpdateTrailer()
    {
      if (!bgUpdateTrailer.IsBusy)
      {
        bgUpdateTrailer.RunWorkerAsync(MyFilms.r);
        LogMyFilms.Info("starting 'Search and register Trailer' in batch mode");
      }
    }

    static void bgUpdateTrailer_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      // ToDo: Check fanart worker thread to implement same way !!!
    }

    static void bgUpdateTrailer_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("'Search and register Trailer' Thread finished");
    }
    
    private void FanartTimerEvent(object state)
    {
      if (backdrop.Filename != null)
      {
        //LogMyFilms.Debug("(FanartTimerEvent): ToggleFanart triggered for '" + facadeView.SelectedListItem.Label + "' !");
        bool success = ToggleFanart();
        if (!success)
          LogMyFilms.Debug("(FanartTimerEvent): ToggleFanart triggered, but not executed due to backdrop not enabled or set properly !");
      }
      else
      {
        LogMyFilms.Debug("(FanartTimerEvent): ToggleFanart NOT triggered, but not executed due to backdrop filename not set !");
      }
    }

    private bool ToggleFanart()
    {
      if (backdrop == null)
      {
        Fanartstatus(false);
        return false; // Fanart not supported by skin, exit now
      }
      string newfanart = GetNewRandomFanart(false, true);
      if (currentFanartList.Count > 1)
      {
        backdrop.Filename = newfanart;
        LogMyFilms.Debug("(ToggleFanart): Loaded fanart is: '" + backdrop.Filename + "'");
        return true;
      }
      else
      {
        _fanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
        return false;
      }
    }

    private void Fanartstatus(bool status)
    {
      if (backdrop == null)
        backdrop = new ImageSwapper();

      if (status == true)
      {
        if (!backdrop.Active)
          backdrop.Active = true;
        if (currentFanartList.Count > 0)
          _fanartTimer.Change(0, RandomFanartDelay); //default 15000 = 15 secs
        else
          _fanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
      }
      else
      {
        if (backdrop.Active)
          backdrop.Active = false;
        // Disable Random Fanart Timer
        _fanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
        // clear current fanarts
        currentFanartList.Clear();
        // Disable Fanart                
        backdrop.Filename = String.Empty;
        MyFilmsDetail.clearGUIProperty("currentfanart");
      }
      LogMyFilms.Debug("Fanartstatus switched to '" + status + "'");
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

    //*****************************************************************************************
    //*  search and download artist imdb pictures from mediaindex                             *
    //*****************************************************************************************
    private void ArtistIMDBpictures(string wperson)
    {
      if (wperson.Length > 0)
      {
        // First check if actror exists...
        ArrayList actorList = new ArrayList();
        // Search with searchName parameter which contain wanted actor name, result(s) is in array which conatin id and name separated with char "|"
        MyFilmsDetail.GetActorByName(wperson, actorList);
        if (actorList.Count != 0)
        {
          actorID = 0;
          string actorname = string.Empty;
          char[] splitter = { '|' };
          foreach (string act in actorList)
          {
            string[] strActor = act.Split(splitter);
            // Split id from actor name (two substrings, [0] is id and [1] is name)
            actorID = Convert.ToInt32(strActor[0]); // IMDBActor  GetActorInfo(int idActor) we need integer)
            actorname = strActor[1];
          }
          MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorID);
        }
      }
    }

    private void OnDetailsUpdated(bool searchPicture)
    {
      LogMyFilms.Debug("OnDetailsUpdated(): Received DetailUpdated event in context '" + GetID + "', doUpdateMainViewByFinishEvent '" + doUpdateMainViewByFinishEvent + "'");
      if (GetID == MyFilms.ID_MyFilms && doUpdateMainViewByFinishEvent)
      {
        LogMyFilms.Debug("OnDetailsUpdated(): now reloading Details");
        doUpdateMainViewByFinishEvent = false;
        Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
      }
      else
        LogMyFilms.Debug("OnDetailsUpdated(): Skipping reloading Details");
    }
    
    //*****************************************************************************************
    //*  set the #myfilms.view label
    //*****************************************************************************************
    private static void SetLabelView(string viewLabel)
    {
      string newViewLabel = viewLabel; // use the parameter as default ...
      viewLabel = viewLabel.ToLower();
      if (viewLabel == GUILocalizeStrings.Get(342).ToLower() || string.IsNullOrEmpty(viewLabel)) // case "films" or empty should show "all" = "filmes"
        viewLabel = "all";
      switch (viewLabel)
      {
        case "search":
          newViewLabel = GUILocalizeStrings.Get(137);// "search"
          break;
        case "all":
          newViewLabel = GUILocalizeStrings.Get(342); //videos
          break;
        case "year":
          newViewLabel = GUILocalizeStrings.Get(345); //year
          break;
        case "category":
          newViewLabel = GUILocalizeStrings.Get(10798664); //category
          break;
        case "country":
          newViewLabel = GUILocalizeStrings.Get(200026); //country
          break;
        case "storage":
          conf.StrTxtSelect = GUILocalizeStrings.Get(10798736);
          newViewLabel = GUILocalizeStrings.Get(154) + " " + GUILocalizeStrings.Get(1951); //storage
          break;
        case "actors":
          newViewLabel = GUILocalizeStrings.Get(10798667); //actors
          break;

        case "view0":
        case "view1":
        case "view2":
        case "view3":
        case "view4":
          // specific user View
          int i = -1;
          if (viewLabel == "view0")
            i = 0;
          if (viewLabel == "view1")
            i = 1;
          if (viewLabel == "view2")
            i = 2;
          if (viewLabel == "view3")
            i = 3;
          if (viewLabel == "view4")
            i = 4;

          //if (viewDefaultItem == conf.StrViewText[0].ToLower() || viewDefaultItem == conf.StrViewItem[0].ToLower())
          //  i = 0;
          //if (viewDefaultItem == conf.StrViewText[1].ToLower() || viewDefaultItem == conf.StrViewItem[1].ToLower())
          //  i = 1;
          //if (viewDefaultItem == conf.StrViewText[2].ToLower() || viewDefaultItem == conf.StrViewItem[2].ToLower())
          //  i = 2;
          //if (viewDefaultItem == conf.StrViewText[3].ToLower() || viewDefaultItem == conf.StrViewItem[3].ToLower())
          //  i = 3;
          //if (viewDefaultItem == conf.StrViewText[4].ToLower() || viewDefaultItem == conf.StrViewItem[4].ToLower())
          //  i = 4;

          if (i != -1)
          {
            if (string.IsNullOrEmpty(conf.StrViewText[i]))
              newViewLabel = conf.StrViewItem[i];   // specific user View1
            else
              newViewLabel = conf.StrViewText[i];   // specific Text for View1
          }
          break;

        default:
          break;
      }
      MyFilmsDetail.setGUIProperty("view", newViewLabel);
      MyFilms.conf.StrTxtView = newViewLabel;
      GUIPropertyManager.SetProperty("#currentmodule", newViewLabel);
      LogMyFilms.Debug("SetLabelView has been called with '" + viewLabel + "' -> set view to '" + newViewLabel + "'");
    }

    //*****************************************************************************************
    //*  set the #myfilms.view label
    //*****************************************************************************************
    private void SetLabelSelect(string selectLabel)
    {
      string newSelectLabel = selectLabel;
      selectLabel = selectLabel.ToLower();
      switch (selectLabel)
      {
        case "root":

          if ((GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating).Length > 0)
          {
            //conf.StrTxtSelect = GUILocalizeStrings.Get(10798622) + " " + GUILocalizeStrings.Get(10798632); // 10798622 all films // 10798632 (global filter active) 
            conf.StrTxtSelect = GUILocalizeStrings.Get(10798632); // 10798622 all films // 10798632 (global filter active) / Filtered
          }
          else
          {
            conf.StrTxtSelect = GUILocalizeStrings.Get(10798622); //10798622 All 
          }
          MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
          break;

        default:
          conf.StrTxtSelect = newSelectLabel;
          MyFilmsDetail.setGUIProperty("select", newSelectLabel);
          break;
      }
    }

    /// <summary>
    /// Setup logger. This funtion made by the team behind Moving Pictures 
    /// (http://code.google.com/p/moving-pictures/)
    /// </summary>
    private static void InitLogger()
    {
      // LoggingConfiguration config = new LoggingConfiguration();
      // Fix suggested in forum to avoid breaking other plugins logging...
      LoggingConfiguration config = LogManager.Configuration ?? new LoggingConfiguration();

      try
      {
        FileInfo logFile = new FileInfo(Config.GetFile(Config.Dir.Log, LogFileName));
        if (logFile.Exists)
        {
          if (File.Exists(Config.GetFile(Config.Dir.Log, OldLogFileName)))
            File.Delete(Config.GetFile(Config.Dir.Log, OldLogFileName));

          logFile.CopyTo(Config.GetFile(Config.Dir.Log, OldLogFileName));
          logFile.Delete();
        }
      }
      catch (Exception) { }

      FileTarget fileTarget = new FileTarget();
      fileTarget.FileName = Config.GetFile(Config.Dir.Log, LogFileName);
      fileTarget.Layout = "${date:format=dd-MMM-yyyy HH\\:mm\\:ss,fff} " + "${level:fixedLength=true:padding=5} " +
                          "[${logger:fixedLength=true:padding=20:shortName=true}]: ${message} " +
                          "${exception:format=tostring}";
                          //"${qpc}";
                         //${qpc:normalize=Boolean:difference=Boolean:alignDecimalPoint=Boolean:precision=Integer:seconds=Boolean}

      config.AddTarget("file", fileTarget);

      // Get current Log Level from MediaPortal 
      NLog.LogLevel logLevel;
      MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"));

      //string myThreadPriority = xmlreader.GetValue("general", "ThreadPriority");

      //if (myThreadPriority != null && myThreadPriority.Equals("Normal", StringComparison.CurrentCulture))
      //{
      //    FHThreadPriority = "Lowest";
      //}
      //else if (myThreadPriority != null && myThreadPriority.Equals("BelowNormal", StringComparison.CurrentCulture))
      //{
      //    FHThreadPriority = "Lowest";
      //}
      //else
      //{
      //    FHThreadPriority = "BelowNormal";
      //}

      switch ((Level)xmlreader.GetValueAsInt("general", "loglevel", 0))
      {
        case Level.Error:
          logLevel = LogLevel.Error;
          break;
        case Level.Warning:
          logLevel = LogLevel.Warn;
          break;
        case Level.Information:
          logLevel = LogLevel.Info;
          break;
        case Level.Debug:
        default:
          logLevel = LogLevel.Debug;
          break;
      }

#if DEBUG
            logLevel = LogLevel.Debug;
#endif

      LoggingRule rule = new LoggingRule("*", logLevel, fileTarget);
      config.LoggingRules.Add(rule);

      LogManager.Configuration = config;
    }

    private static void InitGUIPropertyLabels()
    {
      using (
        MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        MyFilmsDetail.setGUIProperty("config.pluginname", xmlreader.GetValueAsString("MyFilms", "PluginName", "Films"));
        MyFilmsDetail.setGUIProperty("config.pluginmode", xmlreader.GetValueAsString("MyFilms", "PluginMode", "normal"));
        LogMyFilms.Info("Startmode: '" + xmlreader.GetValueAsString("MyFilms", "PluginMode", "normal") + "'");
      }
      AntMovieCatalog ds = new AntMovieCatalog();
      foreach (DataColumn dc in ds.Movie.Columns)
      {
        MyFilmsDetail.setGUIProperty("db." + dc.ColumnName.ToLower() + ".label", BaseMesFilms.Translate_Column(dc.ColumnName));
      }

      GUIPropertyManager.SetProperty("#btWeb.startup.link", "");
      GUIPropertyManager.SetProperty("#btWeb.link.zoom", "");
      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
      GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");

      MyFilmsDetail.setGUIProperty("db.calc.aspectratio.label", GUILocalizeStrings.Get(10798697));
      MyFilmsDetail.setGUIProperty("db.calc.imageformat.label", GUILocalizeStrings.Get(10798698));
      MyFilmsDetail.setGUIProperty("user.sourcetrailer.label", GUILocalizeStrings.Get(10798649));
      MyFilmsDetail.setGUIProperty("user.source.label", GUILocalizeStrings.Get(10798648));
      MyFilmsDetail.setGUIProperty("nbobjects.unit", GUILocalizeStrings.Get(127));
      MyFilmsDetail.setGUIProperty("db.length.unit", GUILocalizeStrings.Get(2998));
      MyFilmsDetail.setGUIProperty("user.watched.label", GUILocalizeStrings.Get(200027));

      MyFilmsDetail.clearGUIProperty("user.watched.value");
      MyFilmsDetail.clearGUIProperty("user.watched.count");
      MyFilmsDetail.clearGUIProperty("user.watched.name");
      MyFilmsDetail.clearGUIProperty("user.watched.global");

      MyFilmsDetail.clearGUIProperty("user.source.isonline");
      MyFilmsDetail.clearGUIProperty("user.sourcetrailer.isonline");
      MyFilmsDetail.clearGUIProperty("logos_id2001");
      MyFilmsDetail.clearGUIProperty("logos_id2002");
      MyFilmsDetail.clearGUIProperty("logos_id2003");
      MyFilmsDetail.clearGUIProperty("logos_id2012"); // Combined Logo
      MyFilmsDetail.clearGUIProperty("nbobjects.value");
      MyFilmsDetail.clearGUIProperty("Fanart");
      MyFilmsDetail.clearGUIProperty("Fanart2");
      MyFilmsDetail.clearGUIProperty("config.currentconfig");
      MyFilmsDetail.clearGUIProperty("config.configfilter");
      MyFilmsDetail.clearGUIProperty("view");
      MyFilmsDetail.clearGUIProperty("select");
      MyFilmsDetail.clearGUIProperty("picture");
      MyFilmsDetail.clearGUIProperty("currentfanart");
      MyFilmsDetail.clearGUIProperty("statusmessage");
      // MyFilmsDetail.clearGUIProperty(guiProperty.statusmessage);
    }

    private void Load_Logos(DataRow row, bool showLogos)
    {
      LogMyFilms.Debug("Using Logos -> '" + conf.StrLogos + "'");

      if (!showLogos)
      {
        MyFilmsDetail.clearGUIProperty("logos_id2001");
        MyFilmsDetail.clearGUIProperty("logos_id2002");
        MyFilmsDetail.clearGUIProperty("logos_id2003");
        MyFilmsDetail.clearGUIProperty("logos_id2012");
        return;
      }
      else
      {

        if ((ImgID2001 != null) && (MyFilms.conf.StrLogos))
        {
          try
          {
            MyFilmsDetail.setGUIProperty(
              "logos_id2001",
              Logos.Build_Logos(
                row, "ID2001", ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, GetID));
          }
          catch (Exception e)
          {
            LogMyFilms.Error("" + e.Message);
          }
        }
        else MyFilmsDetail.clearGUIProperty("logos_id2001");

        if ((ImgID2002 != null) && (MyFilms.conf.StrLogos))
        {
          try
          {
            MyFilmsDetail.setGUIProperty(
              "logos_id2002",
              Logos.Build_Logos(
                row, "ID2002", ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, GetID));
          }
          catch (Exception e)
          {
            LogMyFilms.Error("" + e.Message);
          }
        }
        else MyFilmsDetail.clearGUIProperty("logos_id2002");

        if ((ImgID2003 != null) && (MyFilms.conf.StrLogos))
        {
          try
          {
            MyFilmsDetail.setGUIProperty(
              "logos_id2003",
              Logos.Build_Logos(
                row, "ID2003", ImgID2003.Height, ImgID2003.Width, ImgID2003.XPosition, ImgID2003.YPosition, GetID));
          }
          catch (Exception e)
          {
            LogMyFilms.Error("" + e.Message);
          }
        }
        else MyFilmsDetail.clearGUIProperty("logos_id2003");

        if ((ImgID2012 != null) && (MyFilms.conf.StrLogos))
        {
          try
          {
            MyFilmsDetail.setGUIProperty(
              "logos_id2012",
              Logos.Build_Logos(
                row, "ID2012", ImgID2012.Height, ImgID2012.Width, ImgID2012.XPosition, ImgID2012.YPosition, GetID));
          }
          catch (Exception e)
          {
            LogMyFilms.Error("" + e.Message);
          }
        }
        else MyFilmsDetail.clearGUIProperty("logos_id2012");
      }
    }

    public class FswHandler
    {
      public void OnEvent(Object source, FileSystemEventArgs Args)
      {
        // ToDo: Add required actions here ?
        //Console.Out.WriteLine(Args.ChangeType.ToString());
      }
    }

    delegate void SetDummyControlsForFacadeWorker(Listlevel listlevel);
    private void SetDummyControlsForFacade(Listlevel listlevel)
      {
        SetDummyControlsForFacadeWorker listlevelsetter = this.SetDummyControls;
        listlevelsetter.BeginInvoke(listlevel, null, null);
      }

    private void SetDummyControls(Listlevel listlevel)
    {
      LogMyFilms.Debug("SetDummyControlsForFacade(): listlevel = '" + listlevel + "'");
      if (dummyFacadeFilm == null || dummyFacadeGroup == null || dummyFacadePerson == null)
      {
        LogMyFilms.Debug("SetDummyControlsForFacade() - error, null detected - cannot set !");
        return;
      }
      switch (listlevel)
      {
        case Listlevel.Movie:
          LogMyFilms.Debug("SetDummyControlsForFacade() setting Listlevel to 'film'");
          if (!dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = true;
          if (dummyFacadeGroup.Visible) dummyFacadeGroup.Visible = false;
          if (dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          break;
        case Listlevel.Group:
          LogMyFilms.Debug("SetDummyControlsForFacade() setting Listlevel to 'group'");
          if (dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (!dummyFacadeGroup.Visible) dummyFacadeGroup.Visible = true;
          if (dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          break;
        case Listlevel.Person:
          LogMyFilms.Debug("SetDummyControlsForFacade() setting Listlevel to 'person'");
          if (dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeGroup.Visible) dummyFacadeGroup.Visible = false;
          if (!dummyFacadePerson.Visible) dummyFacadePerson.Visible = true;
          break;
        case Listlevel.None:
          LogMyFilms.Debug("SetDummyControlsForFacade() setting Listlevel to 'none'");
          if (dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeGroup.Visible) dummyFacadeGroup.Visible = false;
          if (dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          break;
        default:
          LogMyFilms.Debug("SetDummyControlsForFacade() setting Listlevel to 'default' (all false)");
          dummyFacadeFilm.Visible = false;
          dummyFacadeGroup.Visible = false;
          dummyFacadePerson.Visible = false;
          break;
      }
    }

    #endregion

    public static int ShowMenuDialog(string heading, List<GUIListItem> items)
    {
      return ShowMenuDialog(heading, items, -1);
    }

    private delegate int ShowMenuDialogDelegate(string heading, List<GUIListItem> items);

    /// <summary>
    /// Displays a menu dialog from list of items
    /// </summary>
    /// <returns>Selected item index, -1 if exited</returns>
    public static int ShowMenuDialog(string heading, List<GUIListItem> items, int selectedItemIndex)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowMenuDialogDelegate d = ShowMenuDialog;
        return (int)GUIGraphicsContext.form.Invoke(d, heading, items);
      }

      GUIDialogMenu dlgMenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlgMenu == null) return -1;

      dlgMenu.Reset();

      dlgMenu.SetHeading(heading);

      foreach (GUIListItem item in items)
      {
        dlgMenu.Add(item);
      }

      if (selectedItemIndex >= 0)
        dlgMenu.SelectedLabel = selectedItemIndex;

      dlgMenu.DoModal(GUIWindowManager.ActiveWindow);

      if (dlgMenu.SelectedLabel < 0)
      {
        return -1;
      }

      return dlgMenu.SelectedLabel;
    }

    /// <summary>
    /// Displays a notification dialog.
    /// </summary>
    public static void ShowNotifyDialog(string heading, string text)
    {
      ShowNotifyDialog(heading, text, string.Empty);
    }

    private delegate void ShowNotifyDialogDelegate(string heading, string text, string image);

    /// <summary>
    /// Displays a notification dialog.
    /// </summary>
    public static void ShowNotifyDialog(string heading, string text, string image)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowNotifyDialogDelegate d = ShowNotifyDialog;
        GUIGraphicsContext.form.Invoke(d, heading, text, image);
        return;
      }

      GUIDialogNotify pDlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
      if (pDlgNotify == null) return;

      // if image is empty, attempt to load the default
      string defaultLogo = Path.Combine(GUIGraphicsContext.Skin, @"Media\Logos\tvseries.png");
      if (File.Exists(defaultLogo))
      {
        image = defaultLogo;
      }

      pDlgNotify.SetHeading(heading);
      pDlgNotify.SetImage(image);
      pDlgNotify.SetText(text);
      pDlgNotify.DoModal(GUIWindowManager.ActiveWindow);
    }

    private delegate void ShowDialogOkDelegate(string heading, string[] lines);

    /// <summary>
    /// Displays a ok dialog.
    /// </summary>
    public static void ShowDialogOk(string heading, string[] lines)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowDialogOkDelegate d = ShowDialogOk;
        GUIGraphicsContext.form.Invoke(d, heading, lines);
        return;
      }

      GUIDialogOK pDlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      if (pDlgOk == null) return;

      pDlgOk.SetHeading(heading);
      for (int i = 1; i <= lines.Length; i++)
      {
        pDlgOk.SetLine(i, lines[i - 1]);
      }
      pDlgOk.DoModal(GUIWindowManager.ActiveWindow);
    }

    //enum BackGroundLoadingArgumentType
    //{
    //  None,
    //  FullElement,
    //  ElementForDelayedImgLoading,
    //  DelayedImgLoading,
    //  DelayedImgInit,
    //  ElementSelection,
    //  SkipSeasonDown,
    //  SkipSeasonUp,
    //  SetFacadeMode
    //}

    //class BackgroundFacadeLoadingArgument
    //{
    //  public BackGroundLoadingArgumentType Type = BackGroundLoadingArgumentType.None;

    //  public object Argument = null;
    //  public int IndexArgument = 0;
    //}

    //private void GUIWindowManager_OnNewMessage(GUIMessage message)
    //{
    //  switch (message.Message)
    //  {
    //    case GUIMessage.MessageType.GUI_MSG_AUTOPLAY_VOLUME:
    //      if (message.Param1 == (int)Ripper.AutoPlay.MediaType.VIDEO)
    //      {
    //        if (message.Param2 == (int)Ripper.AutoPlay.MediaSubType.DVD)
    //          OnPlayDVD(message.Label, GetID);
    //        else if (message.Param2 == (int)Ripper.AutoPlay.MediaSubType.VCD ||
    //                  message.Param2 == (int)Ripper.AutoPlay.MediaSubType.FILES)
    //          OnPlayFiles((System.Collections.ArrayList)message.Object);
    //      }
    //      break;

    //    case GUIMessage.MessageType.GUI_MSG_VOLUME_REMOVED:
    //      if (g_Player.Playing && g_Player.IsVideo && message.Label.Equals(g_Player.CurrentFile.Substring(0, 2), StringComparison.InvariantCultureIgnoreCase))
    //      {
    //        Log.Info("GUIVideoFiles: Stop since media is ejected");
    //        g_Player.Stop();
    //        _playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_VIDEO_TEMP).Clear();
    //      }
    //      break;
    //  }
    //}
  }

  
  //public class GUIMyFilmsListItem : GUIListItem
  //{
  //  #region Facade Item
  //  public GUIMyFilmsListItem(string strLabel) : base(strLabel) { }

  //  public object Item
  //  {
  //    get { return _Item; }
  //    set
  //    {
  //      _Item = value;
  //      INotifyPropertyChanged notifier = value as INotifyPropertyChanged;
  //      if (notifier != null)
  //      {
  //        notifier.PropertyChanged += (s, e) =>
  //        {
  //          if (s is DBActor && e.PropertyName == "ThumbnailImage")
  //            SetImageToGui((s as DBActor).ThumbnailImage);
  //        };
  //      }
  //    }
  //  } protected object _Item;

  //  protected void SetImageToGui(string imageFilePath)
  //  {
  //    if (string.IsNullOrEmpty(imageFilePath)) return;

  //    string texture = GetTextureFromFile(imageFilePath);

  //    if (GUITextureManager.LoadFromMemory(ImageFast.FastFromFile(imageFilePath), texture, 0, 0, 0) > 0)
  //    {
  //      ThumbnailImage = texture;
  //      IconImage = texture;
  //      IconImageBig = texture;
  //    }

  //    // if selected and GUIActors is current window force an update of thumbnail
  //    GUIActors actorWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow) as GUIActors;
  //    if (actorWindow != null)
  //    {
  //      GUIListItem selectedItem = GUIControl.GetSelectedListItem(9816, 50);
  //      if (selectedItem == this)
  //      {
  //        GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, ItemId, 0, null));
  //      }
  //    }
  //  }

  //  private string GetTextureFromFile(string filename)
  //  {
  //    return "[MyFilms:" + filename.GetHashCode() + "]";
  //  }

  //  #endregion
  //}


}