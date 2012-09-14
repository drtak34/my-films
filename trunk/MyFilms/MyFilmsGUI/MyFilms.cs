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
  using System.Xml;
  using System.Xml.Serialization;

  using Grabber;

  using MediaPortal.Configuration;
  using MediaPortal.Dialogs;
  using MediaPortal.ExtensionMethods;
  using MediaPortal.GUI.Library;
  using MediaPortal.Playlists;
  using MediaPortal.Services;
  using MediaPortal.Util;
  using MediaPortal.Video.Database;

  using MyFilmsPlugin.DataBase;
  using MyFilmsPlugin.MyFilms.Configuration;
  using MyFilmsPlugin.MyFilms.Utils;
  using MyFilmsPlugin.MyFilms.Utils.Cornerstone.MP;
  using MyFilmsPlugin.MyFilmsGUI;
  using NLog;
  using NLog.Config;
  using NLog.Targets;

  using WatTmdb.V3;

  using Action = MediaPortal.GUI.Library.Action;
  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;
  using ImageFast = MyFilmsPlugin.MyFilms.Utils.ImageFast;
  // using AntMovieCatalog = MyFilmsPlugin.MyFilms.AntMovieCatalog;
  using Layout = MediaPortal.GUI.Library.GUIFacadeControl.Layout;
  using TmdbPerson = Grabber.TheMovieDbAPI.TmdbPerson;
  using TMDB = WatTmdb.V3;

  /// <summary>
  /// Summary description for GUIMyFilms.
  /// </summary>
  //[PluginIcons("MesFilms.MyFilms.Resources.clapperboard-128x128.png", "MesFilms.MyFilms.Resources.clapperboard-128x128-faded.png")]
  //[PluginIcons("MesFilms.MyFilms.Resources.logo_mesfilms.png", "MesFilms.MyFilms.Resources.logo_mesfilms-faded.png")]
  [PluginIcons("MyFilmsPlugin.Resources.film-reel-128x128.png", "MyFilmsPlugin.Resources.film-reel-128x128-faded.png")]

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

      // create Menu image swapper
      menucover = new AsyncImageResource();
      menucover.Property = "#myfilms.menuimage";
      menucover.Delay = 125;

      // create Film Cover image swapper
      filmcover = new AsyncImageResource();
      filmcover.Property = "#myfilms.coverimage";
      filmcover.Delay = 125;

      // create Group Cover image swapper
      viewcover = new AsyncImageResource();
      viewcover.Property = "#myfilms.viewcoverimage";
      viewcover.Delay = 75;

      // create Person Cover image swapper
      personcover = new AsyncImageResource();
      personcover.Property = "#myfilms.personcoverimage";
      personcover.Delay = 125;

      // create Group Cover image swapper
      groupcover = new AsyncImageResource();
      groupcover.Property = "#myfilms.groupcoverimage";
      groupcover.Delay = 50;

    }
    #endregion

    #region TMDB online movie variables

    DateTime LastRequestLatestMovies = new DateTime();
    IEnumerable<MovieResult> LatestMovies
    {
      get
      {
        if (this._LatestMovies == null || LastRequestLatestMovies < DateTime.UtcNow.Subtract(new TimeSpan(0, MyFilmsSettings.WebRequestCacheMinutes, 0)))
        {
          // _LatestMovies = GetLatestMovies(true);
          LastRequestLatestMovies = DateTime.UtcNow;
          // PreviousSelectedIndex = 0;
        }
        return _LatestMovies;
      }
    }
    private IEnumerable<MovieResult> _LatestMovies = null;

    DateTime LastRequestPopularMovies = new DateTime();
    IEnumerable<PopularMovie> PopularMovies
    {
      get
      {
        if (this._PopularMovies == null || LastRequestPopularMovies < DateTime.UtcNow.Subtract(new TimeSpan(0, MyFilmsSettings.WebRequestCacheMinutes, 0)))
        {
          _PopularMovies = GetPopularMovies(true);
          LastRequestPopularMovies = DateTime.UtcNow;
          // PreviousSelectedIndex = 0;
        }
        return _PopularMovies;
      }
    }
    private IEnumerable<PopularMovie> _PopularMovies = null;

    DateTime LastRequestNowPlayingMovies = new DateTime();
    IEnumerable<NowPlaying> NowPlayingMovies
    {
      get
      {
        if (this._NowPlayingMovies == null || LastRequestNowPlayingMovies < DateTime.UtcNow.Subtract(new TimeSpan(0, MyFilmsSettings.WebRequestCacheMinutes, 0)))
        {
          // _NowPlayingMovies = GetNowPlayingMovies(true);
          LastRequestNowPlayingMovies = DateTime.UtcNow;
          // PreviousSelectedIndex = 0;
        }
        return _NowPlayingMovies;
      }
    }
    private IEnumerable<NowPlaying> _NowPlayingMovies = null;

    DateTime LastRequestTopRatedMovies = new DateTime();
    IEnumerable<TopRated> TopRatedMovies
    {
      get
      {
        if (this._TopRatedMovies == null || LastRequestTopRatedMovies < DateTime.UtcNow.Subtract(new TimeSpan(0, MyFilmsSettings.WebRequestCacheMinutes, 0)))
        {
          // _TopRatedMovies = GetTopRatedMovies(true);
          LastRequestTopRatedMovies = DateTime.UtcNow;
          // PreviousSelectedIndex = 0;
        }
        return _TopRatedMovies;
      }
    }
    private IEnumerable<TopRated> _TopRatedMovies = null;

    DateTime LastRequestUpcomingMovies = new DateTime();
    IEnumerable<UpcomingResult> UpcomingMovies
    {
      get
      {
        if (this._UpcomingMovies == null || LastRequestUpcomingMovies < DateTime.UtcNow.Subtract(new TimeSpan(0, MyFilmsSettings.WebRequestCacheMinutes, 0)))
        {
          // _UpcomingMovies = GetUpcomingMovies(true);
          LastRequestUpcomingMovies = DateTime.UtcNow;
          // PreviousSelectedIndex = 0;
        }
        return _UpcomingMovies;
      }
    }
    private IEnumerable<UpcomingResult> _UpcomingMovies = null;

    #endregion

    #region MapSettings class
    [Serializable]
    public class MapSettings
    {
      protected int _SortBy;
      protected int _ViewAs;
      protected bool _SortAscending;

      public MapSettings()
      {
        // Set default layout
        _SortBy = 0;
        _ViewAs = (int)Layout.List;
        _SortAscending = true;
      }

      [XmlElement("SortBy")]
      public int SortBy
      {
        get { return _SortBy; }
        set { _SortBy = value; }
      }

      [XmlElement("ViewAs")]
      public int ViewAs
      {
        get { return _ViewAs; }
        set { _ViewAs = value; }
      }

      [XmlElement("SortAscending")]
      public bool SortAscending
      {
        get { return _SortAscending; }
        set { _SortAscending = value; }
      }
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

    // Log declarations
    private static Logger LogMyFilms = LogManager.GetCurrentClassLogger();
    // public XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true);

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
    public const int ID_MyFilmsCoverManager = 7992;
    public const int ID_MyFilmsDialogMultiSelect = 7993;
    public const int ID_MyFilmsFanartManager = 7994;

    public const int ID_BrowseTheWeb = 54537689;
    public const int ID_OnlineVideos = 4755;
    public const int ID_SubCentral = 84623;
    public const int ID_BluRayPlayerLauncher = 8080;

    public const int cacheThumbWith = 400;
    public const int cacheThumbHeight = 600;

    public const string ImdbBaseUrl = "http://www.imdb.com/";
    public const string TmdbApiKey = "1e66c0cc99696feaf2ea56695e134eae";

    internal const string GlobalUsername = "Global";
    internal const string DefaultUsername = "Default";

    enum Controls : int
    {
      CTRL_BtnLayout = 2,
      CTRL_BtnSortBy = 3,
      CTRL_BtnSearch = 4,
      CTRL_BtnViewAs = 5,
      CTRL_BtnOptions = 6,
      CTRL_BtnGlobalOverlayFilter = 7,
      CTRL_BtnGlobalUpdates = 8,
      CTRL_BtnToggleGlobalUnwatchedStatus = 9,
      //CTRL_TxtSelect = 12,
      CTRL_Fanart = 11,
      CTRL_Fanart2 = 21,
      CTRL_LoadingImage = 22,
      CTRL_DummyFacadeFilm = 36,
      CTRL_DummyFacadeView = 37,
      CTRL_DummyFacadePerson = 38,
      CTRL_DummyFacadeHierarchy = 39,
      CTRL_DummyFacadeMenu = 40,
      CTRL_DummyFacadeIndex = 41,
      CTRL_ListFilms = 50, // Changed from 1026 to 50 due to meeting MePo Standards - holds film lists and hierarchies
      //CTRL_ListMenu = 51, // added as separate list control for new menu - holds menu and extended menu
      //CTRL_ListViews = 52, // added as separate list control for Views - holds normal and indexed views (groupings)
      CTRL_logos_id2001 = 2001,
      CTRL_logos_id2002 = 2002,
      CTRL_logos_id2003 = 2003,
      CTRL_logos_id2012 = 2012,
      CTRL_GuiWaitCursor = 2080,
    }

    #region Skin Variables
    //[SkinControlAttribute((int)Controls.CTRL_TxtSelect)]
    //protected GUIFadeLabel TxtSelect;

    [SkinControlAttribute((int)Controls.CTRL_BtnLayout)]
    protected GUIButtonControl BtnLayout; // ToDo: can be replaceed by "GUIMenuButton" when dropping MP1.2 compatibility - requires skin change too

    [SkinControlAttribute((int)Controls.CTRL_BtnSortBy)]
    protected GUISortButtonControl BtnSrtBy;

    [SkinControlAttribute((int)Controls.CTRL_BtnViewAs)]
    protected GUIButtonControl BtnViewAs; // ToDo: can be replaceed by "GUIMenuButton" when dropping MP1.2 compatibility - requires skin change too

    [SkinControlAttribute((int)Controls.CTRL_BtnGlobalOverlayFilter)]
    protected GUIButtonControl BtnGlobalOverlayFilter;

    [SkinControlAttribute((int)Controls.CTRL_BtnGlobalUpdates)]
    protected GUIButtonControl BtnGlobalUpdates;

    //[SkinControlAttribute((int)Controls.CTRL_BtnToggleGlobalUnwatchedStatus)]
    //protected GUIButtonControl BtnToggleGlobalWatched;

    [SkinControlAttribute((int)Controls.CTRL_ListFilms)]
    protected GUIFacadeControl facadeFilms;

    //[SkinControlAttribute((int)Controls.CTRL_ListMenu)]
    //protected GUIFacadeControl facadeMenu;

    //[SkinControlAttribute((int)Controls.CTRL_ListViews)]
    //protected GUIFacadeControl facadeViews;

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

    [SkinControlAttribute((int)Controls.CTRL_DummyFacadeHierarchy)]
    protected GUILabelControl dummyFacadeHierarchy = null;

    [SkinControlAttribute((int)Controls.CTRL_DummyFacadeView)]
    protected GUILabelControl dummyFacadeView = null;

    [SkinControlAttribute((int)Controls.CTRL_DummyFacadePerson)]
    protected GUILabelControl dummyFacadePerson = null;

    [SkinControlAttribute((int)Controls.CTRL_DummyFacadeMenu)]
    protected GUILabelControl dummyFacadeMenu = null;

    [SkinControlAttribute((int)Controls.CTRL_DummyFacadeIndex)]
    protected GUILabelControl dummyFacadeIndex = null;

    #endregion

    #region Private/Public Properties

    private bool StopLoadingMenuDetails { get; set; } // to cancel menu counts, if user makes selection before it's finished ....
    private bool StopLoadingViewDetails { get; set; }
    private bool StopLoadingFilmlistDetails { get; set; }

    private bool doUpdateMainViewByFinishEvent = false;

    //private Layout CurrentLayout { get; set; }

    // public static ReaderWriterLockSlim _rw = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    public static int Prev_ItemID = -1;
    public static string Prev_Label = string.Empty;

    public static GUIListItem itemToPublish = null;

    public static Configuration conf;
    //// keeps track of last loaded config to make sure that on page load the DB is reloaded, if it was changed by e.g. the default config
    //private static Configuration confPrevious = null;

    private string PreviousConfig = ""; // keeps last loaded config for checks in OnPageLoad()

    public static Logos confLogos;

    public static DataRow[] r; // will hold current recordset to traverse
    public static MFMovie currentMovie = new MFMovie(); // will hold current recordset to traverse

    //Imageswapperdefinitions for fanart and cover
    private ImageSwapper backdrop;

    private AsyncImageResource menucover = null;
    private AsyncImageResource filmcover = null;
    private AsyncImageResource viewcover = null;
    private AsyncImageResource personcover = null;
    private AsyncImageResource groupcover = null;

    //Added to jump back to correct Menu (Either Basichome or MyHome - or others...)
    private bool Context_Menu = false;
    //private string currentConfig;
    private const string strPluginName = "MyFilms";

    private System.Threading.Timer _fanartTimer = null;

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
    public string GlobalFilterStringCustomFilter = string.Empty;

    public static bool InitialStart = false;                  //Added to implement InitialViewSetup
    public static bool InitialIsOnlineScan = false;           //Added to implement switch if facade should display media availability

    // current Random Fanart
    public static List<string> currentFanartList = new List<string>();

    // current Trailer List for Scrobbling
    private static List<MFMovie> currentTrailerMoviesList = new List<MFMovie>();
    public static MFMovie currentTrailerPlayingItem = null;
    public static bool trailerscrobbleactive = false;

    //PlayList currentPlaylist = null;
    //PlayListItem currentPlayingItem = null;

    private bool NetworkAvailabilityChanged_Subscribed = false;
    private bool PowerModeChanged_Subscribed = false;

    private double lastPublished = 0;
    private Timer publishTimer;

    private static Stopwatch watch = new Stopwatch();

    private const int RandomFanartDelay = 15000;

    // Version for Skin Interface
    private const int SkinInterfaceVersionMajor = 1;
    private const int SkinInterfaceVersionMinor = 0;

    public static bool DebugPropertyLogging { get; set; }

    public static bool SendTraktUpdateMessage { get; set; } // to tell FSwatcher, if Trakt message should be sent

    // keeps track of currently loaded skin name to (re)initiate skin interface check on pageload
    private string currentSkin = null;

    public static string[] PersonTypes = new string[] { "Persons", "Actors", "Producer", "Director", "Writer", "Borrower" };

    private static string EmptyFacadeValue = "(empty)";

    // View History for facade navigation support
    // private static List<ViewState> ViewHistory = new List<ViewState>();
    private Stack ViewHistory = new Stack();

    // new method for back navigation
    private Stack NavigationStack = new Stack();
    MapSettings mapSettings = new MapSettings();
    // static GUIDialogProgress dlgProgress;

    // cache to store viewstate params from current session per view
    Dictionary<string, ViewState> ViewStateCache = new Dictionary<string, ViewState>();  // public static Dictionary<string, ViewState> ViewStateCache = new Dictionary<string, ViewState>();

    // string list for search history
    public static List<string> SearchHistory = new List<string>();
    LoadParameterInfo loadParamInfo;

    // last update to catalog - used to know, if the backnavigation needs to reload the facade -  LoadFacade();
    public static DateTime LastDbUpdate { get; set; }


    #endregion

    #region Enums
    public enum FieldType
    {
      Title,
      Date,
      AlphaNumeric,
      Decimal,
      Person,
      Default
    }

    public enum ExternalPluginWindows : int
    {
      BrowseTheWeb = 54537689,
      OnlineVideos = 4755,
      SubCentral = 84623
    }

    public enum ViewContext
    {
      Movie,
      MovieCollection,
      Group,
      Person,
      Menu,
      MenuAll,
      StartView,
      None
    }

    public enum ViewSortType
    {
      Name = 0,
      Frequency = 1
    }

    //enum of all possible layouts
    public enum Layout
    {
      List = 0,
      SmallIcons = 1,
      LargeIcons = 2,
      Filmstrip = 3,
      AlbumView = 4,
      Playlist = 5,
      CoverFlow = 6
    }

    //public enum View
    //{
    //  List = 0,
    //  Icons = 1,
    //  BigIcons = 2,
    //  Albums = 3,
    //  Filmstrip = 4,
    //  CoverFlow = 5
    //}

    enum eContextItems
    {
      toggleWatched,
      cycleMoviePoster,
      downloadSubtitle,
      actionMarkAllWatched,
      actionMarkAllUnwatched,
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
    #endregion

    #endregion

    #region handler and backgroundworker

    public delegate void FilmsStoppedHandler(int stoptime, string filename);
    public delegate void FilmsEndedHandler(string filename);

    public delegate void EventHandler(object sender, EventArgs e);

    BackgroundWorker bgUpdateDB = new BackgroundWorker();
    BackgroundWorker bgUpdateFanart = new BackgroundWorker();
    BackgroundWorker bgUpdateActors = new BackgroundWorker();
    BackgroundWorker bgUpdateTrailer = new BackgroundWorker();
    BackgroundWorker bgLoadMovieList = new BackgroundWorker();
    BackgroundWorker bgIsOnlineCheck = new BackgroundWorker();

    public static FileSystemWatcher FSwatcher = new FileSystemWatcher();

    public delegate void DelegateUpdateProgress(string message);
    public static void UpdateMyProgressbar(string message)
    {
      MyFilmsDetail.setGUIProperty("statusmessage", message);
    }

    public static event ImportCompleteEventDelegate ImportComplete;
    public delegate void ImportCompleteEventDelegate();


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
      string currentmodule = GUILocalizeStrings.Get(MyFilms.ID_MyFilms);
      string view = (conf != null) ? conf.StrTxtView : "";
      string select = (conf != null) ? conf.StrTxtSelect : "";
      if (conf != null)
      {
        currentmodule = GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + view + ((select.Length > 0) ? "/" : "") + select;
      }
      LogMyFilms.Debug("GetModuleName()  - conf.StrTxtView = '" + view + "', conf.StrTxtSelect = '" + select + "', currentmodule = '" + currentmodule + "'");

      return currentmodule; // return localized string for Module ID
    }

    public override bool Init() //This Method is only loaded ONCE when starting Mediaportal !
    {
      bool result = Load(GUIGraphicsContext.Skin + @"\MyFilms.xml");
      InitLogger(); // Initialize Logger 
      Log.Info("MyFilms.Init() started. See MyFilms.log for further Details.");
      LogMyFilms.Info("MyFilms.Init() started on '" + System.Environment.MachineName + "'.");
      LogMyFilms.Info("MyFilms     Version: 'V" + MyFilmsSettings.Version + "', BuildDate: '" + MyFilmsSettings.BuildDate + "'");
      LogMyFilms.Info("MediaPortal Version: 'V" + MyFilmsSettings.MPVersion + "',    BuildDate: '" + MyFilmsSettings.MPBuildDate + "'");
      LogMyFilms.Info("MyFilms Skin Interface Version: 'V" + SkinInterfaceVersionMajor + "." + SkinInterfaceVersionMinor + "'");

      // check, if remote config file should be copied to local MP data dir (MyFilms Server Setup)
      SyncConfigFromRemoteServer();

      // clean orphaned files for all configs if any leftovers
      CleanOrphanedDBlocks();

      // Fanart Timer
      _fanartTimer = new System.Threading.Timer(new TimerCallback(FanartTimerEvent), null, Timeout.Infinite, Timeout.Infinite);

      // Set Variable for FirstTimeView Setup
      InitialStart = true;

      // by default, enable trakt message handling
      SendTraktUpdateMessage = true;

      //Add localized labels for DB Columns
      InitGUIPropertyLabels();

      // Init Eventhandler for Background Updates
      MyFilmsDetail.DetailsUpdated += new MyFilmsDetail.DetailsUpdatedEventDelegate(OnDetailsUpdated);

      // Init Eventhandler for Trailer Scrobbling
      MyFilmsDetail.TrailerEnded += new MyFilmsDetail.TrailerEndedEventDelegate(OnTrailerEnded);

      // Register Messagehandler for CD-Inserted-Messages
      //GUIWindowManager.Receivers += new SendMessageHandler(GUIWindowManager_OnNewMessage);

      // Register Eventhandler for AMCupdater Background progress reporting
      //AMCupdaterStartEventHandler();

      // Initialize Backgroundworker
      InitializeBackgroundWorker();
      InitFolders();

      //// launch TMDB data loader in background ...
      //new System.Threading.Thread(delegate()
      //{
      //  {
      //    try
      //    {
      //      IEnumerable<PopularMovie> movies = PopularMovies;
      //    }
      //    catch (Exception)
      //    {
      //    }
      //  }
      //  GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
      //  {
      //    {
      //      // this after thread finished ...
      //    }
      //    return 0;
      //  }, 0, 0, null);
      //}) { Name = "MyFilmsTmdbDataLoader", IsBackground = true, Priority = ThreadPriority.BelowNormal }.Start();

      LogMyFilms.Debug("MyFilms.Init() completed. Loading main skin file.");
      return result;
    }

    public override void DeInit()
    {
      LogMyFilms.Debug("MyFilms.DeInit() - Saving Config changes ...");
      XmlSettings.SaveCache();
      BaseMesFilms.CancelMyFilms();
      CleanOrphanedDBlocks(); // clean orphaned files for all configs if any leftovers

      if (BaseMesFilms.UpdateWorker != null && BaseMesFilms.UpdateWorker.IsBusy)
      {
        LogMyFilms.Info("MyFilms.DeInit() - DB updates still active ! - waiting for background worker to complete ...");
        BaseMesFilms.UpdateWorkerDoneEvent.WaitOne(60000);
        LogMyFilms.Info("MyFilms.DeInit() - DB updates in background worker thread finished");
        BaseMesFilms.UpdateWorkerDoneEvent.WaitOne(1000); // wait another second to finish log entries
      }
      LogMyFilms.Debug("MyFilms.DeInit() - Shutdown completed...");
      base.DeInit();
    }

    //protected override string SerializeName
    //{
    //  get { return "myfilms"; }
    //}

    protected override void OnPageLoad() //This is loaded each time, the plugin is entered - can be used to reset certain settings etc.
    {
      LogMyFilms.Debug("MyFilms.OnPageLoad() started."); Log.Debug("MyFilms.OnPageLoad() started. See MyFilms.log for further Details.");

      // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms));

      CheckSkinInterfaceVersion();

      if (InitialStart)
      {
        CheckAndLogEnhancedSkinControls();
        InitMainScreen(false); // don't log to MyFilms.log Property clear
        //InitGlobalFilters(false);
      }

      InitBSHandler(); // Register PowerEventMode and Networkavailability Changed Handler

      #region Support for StartParameters - load  them ...

      if (PreviousWindowId != ID_MyFilms && PreviousWindowId != ID_MyFilmsDetail)
      {
        // check if running version of mediaportal support loading with parameter           
        //{
        //  LoadWithParameterSupported = true;
        //}

        loadParamInfo = null; // reset the LoadParameterInfo

        string loadParam = null;
        // check if running version of mediaportal supports loading with parameter and handle _loadParameter
        System.Reflection.FieldInfo fi = typeof(GUIWindow).GetField("_loadParameter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fi != null)
          loadParam = (string)fi.GetValue(this);

        // check for LoadParameters by GUIproperties if nothing was set by the _loadParameter
        if (string.IsNullOrEmpty(loadParam))
          loadParam = LoadParameterInfo.FromGuiProperties();
        if (!string.IsNullOrEmpty(loadParam))
          loadParamInfo = new LoadParameterInfo(loadParam);
      }
      #endregion

      //MyFilmsDetail.clearGUIProperty("picture");

      #region init buttons and register sort button handler ...
      GUIButtonControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnSearch, GUILocalizeStrings.Get(137));
      BtnSrtBy.SortChanged -= new SortEventHandler(SortChanged);
      BtnSrtBy.SortChanged += new SortEventHandler(SortChanged);

      GUIButtonControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnGlobalOverlayFilter, GUILocalizeStrings.Get(10798714)); // BtnGlobalOverlayFilter.Label = GUILocalizeStrings.Get(10798714); // Global Filters ...
      GUIButtonControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnGlobalUpdates, GUILocalizeStrings.Get(10798690));

      //try { BtnGlobalUpdates.Label = GUILocalizeStrings.Get(10798690); } // Global Updates ...  
      //catch (Exception) { LogMyFilms.Warn("(InitMainScreen) - 'Global Updates' button label cannot be set - not defined in skin?"); }
      #endregion

      #region (re)link backdrop image controls to the backdrop image swapper
      backdrop.GUIImageOne = ImgFanart;
      backdrop.GUIImageTwo = ImgFanart2;
      //backdrop.LoadingImage = loadingImage;  // --> Not used - could be used to show other image while loading destination thumb

      if (!menucover.Active) menucover.Active = true;
      if (!filmcover.Active) filmcover.Active = true;
      if (!viewcover.Active) viewcover.Active = true;
      if (!personcover.Active) personcover.Active = true;
      if (!groupcover.Active) groupcover.Active = true;
      #endregion

      bool IsDefaultConfig = false;

      if (PreviousWindowId != ID_MyFilmsDetail && PreviousWindowId != ID_MyFilmsActors && PreviousWindowId != ID_OnlineVideos && PreviousWindowId != ID_BrowseTheWeb)
      {
        #region Load or change MyFilms Config

        PreviousConfig = Configuration.CurrentConfig;
        IsDefaultConfig = Configuration.Current_Config(true); // also sets "Configuration.CurrentConfig" - "true" brings up config menu, if nothing is set ...

        if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Config)) // config given in load params
        {
          #region change config by load parameter overload

          string currentconfig = (!string.IsNullOrEmpty(Configuration.CurrentConfig)) ? Configuration.CurrentConfig : "";
          LogMyFilms.Debug("Load_Config() - LoadParams - try override current config '" + currentconfig + "' with LoadParameter config: '" + loadParamInfo.Config + "'");
          string newConfig = Configuration.Control_Access_Config(loadParamInfo.Config, GetID);
          if (newConfig != string.Empty) // if user escapes dialog or bad value leave system unchanged
          {
            if (Configuration.CurrentConfig != newConfig)
            {
              if (!string.IsNullOrEmpty(Configuration.CurrentConfig)) // if there is an active config, save it !
              {
                if (this.facadeFilms.SelectedListItem != null)
                  Configuration.SaveConfiguration(Configuration.CurrentConfig, this.facadeFilms.SelectedListItem.ItemId, this.facadeFilms.SelectedListItem.Label);
                else
                  Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, string.Empty);
              }
              // set new config
              Configuration.CurrentConfig = newConfig;
              // initialize ...
              InitMainScreen(false); // reset all properties and values
              InitGlobalFilters(false); // reset global filters, when loading new config !
              InitialIsOnlineScan = false; // set false, so facade does not display false media status !!!
              InitialStart = (string.IsNullOrEmpty(loadParamInfo.MovieID));
              //Set to true to make sure initial View is initialized for new DB view
            }
          }

          #endregion
        }

        Load_Config(Configuration.CurrentConfig, true, loadParamInfo);

        if (string.IsNullOrEmpty(Configuration.CurrentConfig))
        {
          LogMyFilms.Error("OnPageLoad(): Config is empty - returning calling ShowPreviousWindow() !");
          GUIWindowManager.ShowPreviousWindow();
          return;
        }

        InitFSwatcher(); // load DB watcher for multiseat
        #endregion

        Fanartstatus(MyFilms.conf.StrFanart);
        if (!InitialStart && IsDefaultConfig) InitMainScreen(false); // clear all properties, if a defaultconfig is loaded - otherwise we might run into display problems due to old properties remaining
        if (Configuration.CurrentConfig != PreviousConfig) InitialStart = true; // if a default config is set, otherwise the DB gets not proerly initialized
      }
      base.OnPageLoad(); // let animations run

      if (NavigationStack.Count > 0 && !conf.AlwaysDefaultView && !InitialStart)
      {
        DoBack();
        GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        // GUIControl.FocusControl(GetID, facadeFilms.GetID);
        OnPageload_Step_2();
      }
      else
      {
        NavigationStack.Clear();
        // Fin_Charge_Init((conf.AlwaysDefaultView || InitialStart), (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Config) || IsDefaultConfig)); // reloadFromDisk is true, if a config is set in MF setup (not default view!) or loadparams are set
        new Thread(delegate()
        {
          {
            #region load Fin_Charge_Init with progress dialog
            //MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
            //GUIWaitCursor.Init(); GUIWaitCursor.Show();
            GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
            if (InitialStart && GetID == ID_MyFilms)
            {
              #region Show loading in progress dialog
              if (dlgPrgrs != null)
              {
                dlgPrgrs.Reset();
                dlgPrgrs.DisplayProgressBar = false;
                dlgPrgrs.ShowWaitCursor = true;
                dlgPrgrs.DisableCancel(true);
                dlgPrgrs.SetHeading(string.Format("{0} - {1}", "MyFilms", "Loading ..."));
                // dlgPrgrs.SetLine(1, "Loading DB ...");
                // dlgPrgrs.Percentage = 0;
                dlgPrgrs.NeedRefresh();
                dlgPrgrs.ShouldRenderLayer();
                dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
              }
              #endregion
            }

            if (InitialStart)
            {
              conf.StrSelect = "";
              conf.StrFilmSelect = "";
              conf.StrViewSelect = "";
              // loaded already in FinChargeInit() with reload = true if InitialStart !
              // r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // if (InitialStart) BaseMesFilms.LoadMyFilms(conf.StrFileXml);
            }

            Fin_Charge_Init((conf.AlwaysDefaultView || InitialStart), ((loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Config)) || conf.IsDbReloadRequired)); // || InitialStart)); // reloadFromDisk is true, if the loaded config and it's used DB file is physically a different one than the previous one
            // Fin_Charge_Init((conf.AlwaysDefaultView || InitialStart), ((loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Config)))); // || InitialStart)); // reloadFromDisk is true, if a config is set in MF setup (not default view!) or loadparams are set
            // Fin_Charge_Init((conf.AlwaysDefaultView || InitialStart || requireFullReload), requireFullReload || Configuration.Current_Config()); // reloadFromDisk is true, if a config is set in MF setup (not default view!) or loadparams are set

            // GUIWaitCursor.Hide();
            //MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
            //GUIControl.ShowControl(GetID, 34);

            if (GetID == ID_MyFilms)
            {
              if (dlgPrgrs != null)
              {
                dlgPrgrs.ShowWaitCursor = false;
                dlgPrgrs.Close();
              }
            }
            #endregion
          }
          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              GUIPropertyManager.SetProperty("#currentmodule", this.GetModuleName());  // reload current modulename, as otherwise it is reset to base one...
              OnPageload_Step_2();
            }
            return 0;
          }, 0, 0, null);
        }) { Name = "MyFilmsOnPageLoadWorker", IsBackground = true }.Start();

      }

      // OnPageload_Step_2();


      // base.OnPageLoad(); // let animations run  
    }

    private void OnPageload_Step_2()
    {
      LogMyFilms.Debug("OnPageload_Step_2() started.");
      // Launch Background availability scanner, if configured in setup
      if (MyFilms.conf.ScanMediaOnStart)
      {
        this.AsynIsOnlineCheck();
      }

      #region LoadParam processing for single movie and search
      if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.MovieID) && loadParamInfo.Config == Configuration.CurrentConfig && PreviousWindowId != ID_MyFilmsDetail) // movieID given in load params -> jump to details screen !
      {
        LogMyFilms.Debug("OnPageLoad() - LoadParams - try override loading movieid: '" + loadParamInfo.MovieID + "', play: '" + loadParamInfo.Play + "'");
        // facade index is alreadey set in filmlist loading - only launching details necessary !

        if (!string.IsNullOrEmpty(loadParamInfo.MovieID)) // if load params for movieid exist, set current index to the movie detected
        {
          if (r.Length == 0)
            GUIWindowManager.ShowPreviousWindow();
          if (loadParamInfo.Play == "true")
            MyFilmsDetail.Launch_Movie(conf.StrIndex, GetID, null, false);
          else
            GUIWindowManager.ActivateWindow((int)ID_MyFilmsDetail, true);
        }
      }
      else if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.Search)) // search expression given in load params -> do global search !
      {
        LogMyFilms.Debug("OnPageLoad() - LoadParams - try loading search with search expression: '" + loadParamInfo.Search + "'");
        conf.Boolselect = false;
        SearchMoviesbyProperties(false, loadParamInfo.Search, string.Empty);
      }
      #endregion

      MyFilmsDetail.setGUIProperty("user.onlinestatus", Helper.GetUserOnlineStatus(conf.StrUserProfileName));

      loadParamInfo = null; // all done, so "null" it to allow "normal browsing" from now on ...

      if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
      {
        // Originally Deactivated by Zebons    
        // ********************************
        //if (!bgLoadMovieList.IsBusy)
        //{
        //  LogMyFilms.Debug("Launching AsynLoadMovieList");
        //  AsynLoadMovieList();
        //}
        // ********************************

        int itemIndex = facadeFilms.SelectedListItemIndex;
        // LogMyFilms.Debug("MyFilms.OnPageLoad() - itemindex = '" + itemIndex + "', facadeCount = '" + facadeFilms.Count + "'");
        if (itemIndex > -1) GUIControl.SelectItemControl(GetID, facadeFilms.GetID, itemIndex);
        else if (facadeFilms.Count > 0) GUIControl.SelectItemControl(GetID, facadeFilms.GetID, 0);

        LogMyFilms.Debug("MyFilms.OnPageLoad() completed.");
      }
    }

    private void SyncConfigFromRemoteServer()
    {
      if (!System.IO.File.Exists(Config.GetFolder(Config.Dir.Config) + @"\MyFilmsServer.xml"))
      {
        LogMyFilms.Warn("SyncConfigFromRemoteServer() - local file MyFilmsServer.xml not found - cannot read sync settings - exit sync.");
        return;
      }
      XmlConfig MyFilmsServer = new XmlConfig();
      string MyFilmsCentralConfigDir = MyFilmsServer.ReadXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "MyFilmsCentralConfigFile", "");
      bool SyncFromServerOnStartup = MyFilmsServer.ReadXmlConfig("MyFilmsServer", "MyFilmsServerConfig", "SyncOnStartup", false);
      LogMyFilms.Info("SyncConfigFromRemoteServer() - SyncOnStartup = '" + SyncFromServerOnStartup + "'");
      if (SyncFromServerOnStartup && System.IO.File.Exists(MyFilmsCentralConfigDir + @"\MyFilms.xml"))
      {
        LogMyFilms.Info("SyncConfigFromRemoteServer() - Server Sync is enabled - remote directory: '" + MyFilmsCentralConfigDir + "'");
        string serverConfigFile = MyFilmsCentralConfigDir + @"\MyFilms.xml";
        string localConfigFile = Config.GetFolder(Config.Dir.Config) + @"\MyFilms.xml";
        if (!System.IO.Directory.Exists(MyFilmsCentralConfigDir))
        {
          LogMyFilms.Error("SyncConfigFromRemoteServer() - remote directory is not accessible !");
        }
        else if (!System.IO.File.Exists(serverConfigFile))
        {
          LogMyFilms.Error("SyncConfigFromRemoteServer() - remote config file note found !");
        }
        else
        {
          if (System.IO.File.Exists(localConfigFile))
          {
            try
            {
              string backupfile = localConfigFile.Replace(".xml", " - " + DateTime.Now.ToString("u").Replace(":", "-") + ".xml").Replace("/", "-");
              System.IO.File.Copy(localConfigFile, backupfile, true);
            }
            catch (Exception)
            {
              LogMyFilms.Error("SyncConfigFromRemoteServer() - could not backup local MyFilms.xml config file !");
            }
          }
          try
          {
            System.IO.File.Copy(serverConfigFile, localConfigFile, true);
            LogMyFilms.Info("SyncConfigFromRemoteServer() - Successfully copied remote config to local config");
          }
          catch (Exception)
          {
            LogMyFilms.Error("SyncConfigFromRemoteServer() - could not copy remote config to local config file !");
          }
        }
      }
      else
      {
        LogMyFilms.Info("SyncConfigFromRemoteServer() - Server Sync is disabled - running on local config only !");
      }
    }

    private void CleanOrphanedDBlocks()
    {
      using (XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        int MesFilms_nb_config = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
        ArrayList configs = new ArrayList();
        for (int i = 0; i < MesFilms_nb_config; i++)
          configs.Add(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));
        XmlSettings xmlSettings = XmlConfig;
        foreach (string StrFileXml in from string config in configs where xmlSettings != null select xmlSettings.ReadXmlConfig("MyFilms", config, "AntCatalog", string.Empty))
        {
          MyFilmsDetail.SetGlobalLock(false, StrFileXml); // release global lock, if there is any, after initializing (this is cleanup for older leftovers)
        }
      }
    }

    protected override void OnPageDestroy(int new_windowId)
    {
      LogMyFilms.Debug("MyFilms.OnPageDestroy(" + new_windowId.ToString() + ") started.");

      // stop any background tasks
      StopLoadingViewDetails = true;
      StopLoadingMenuDetails = true;
      StopLoadingFilmlistDetails = true;
      GUIConnector.Instance.StopBackgroundTask();

      loadParamInfo.SafeDispose();
      //// Reset to force republishing details on reentering
      //Prev_ItemID = -1;
      //Prev_Label = string.Empty;
      //if (!bgOnPageLoad.CancellationPending) // cancel pageload worker thread - otherwise null ref exception when trying to populate facade ...
      //  bgOnPageLoad.CancelAsync();
      //Thread.Sleep(5); // sleep 5 milliseconds
      // Set Facadevisibilities false ...
      //SetDummyControlsForFacade(ViewContext.None);

      //Disable Random Fanart Timer
      _fanartTimer.Change(Timeout.Infinite, Timeout.Infinite);

      //LogMyFilms.Debug("GUIMessage: GUI_MSG_WINDOW_DEINIT - Start");

      // save current GUIlist in navigation cache
      SaveListState(false);

      MyFilmsDetail.clearGUIProperty("nbobjects.value"); // clear counts for the next start to fix "visibility animations" ....

      if (Configuration.CurrentConfig != "")
      {
        if (this.facadeFilms == null || this.facadeFilms.SelectedListItemIndex == -1)
          Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, "");
        else
          Configuration.SaveConfiguration(Configuration.CurrentConfig, this.facadeFilms.SelectedListItem.ItemId, this.facadeFilms.SelectedListItem.Label);
      }

      //ImgFanart.SetFileName(string.Empty);
      //ImgFanart2.SetFileName(string.Empty);

      //facadeFilms.Clear();
      //backdrop.PropertyOne = " ";
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
      if (this.facadeFilms.SelectedListItemIndex > -1)
      {
        if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        Context_Menu_Movie(this.facadeFilms.SelectedListItem.ItemId);
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
    //    GUIListItem currentitem = this.facadeFilms.SelectedListItem;
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
      LogMyFilms.Debug("OnAction() -  " + action.wID);
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
          if (!facadeFilms.Focus)
          {
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // set focus to facade, if e.g. menu buttons had focus (after global options etc.)
            return;
          }

          // LogStatusVars("PreviousMenu");
          string viewStateCacheName = (null != GetCustomViewFromViewLabel(conf.CurrentView)) ? "CustomView_" + conf.WStrSort : conf.WStrSort;
          if (!string.IsNullOrEmpty(viewStateCacheName))
            SaveLastView(viewStateCacheName);

          if (NavigationStack.Count > 0) //_backPos
          {
            StopLoadingViewDetails = true;
            StopLoadingMenuDetails = true;
            StopLoadingFilmlistDetails = true;

            DoBack();
            if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            //UpdateGui();
            return;
          }

          if (ViewHistory.Count > 0)
          {
            RestoreLastView();
            r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // load dataset with restored former filters to get proper counts ...
            this.Refreshfacade();
            return;
          }
          switch (conf.ViewContext)
          {
            case ViewContext.None: // do nothing, if no valid context is there (e.g. because there is still backgroundloading of menu active...)
            case ViewContext.StartView:
              #region None
              StopLoadingViewDetails = true;
              StopLoadingMenuDetails = true;
              StopLoadingFilmlistDetails = true;
              GUIWindowManager.ShowPreviousWindow();
              #endregion
              return;
            case ViewContext.MenuAll:
              #region MenuAll
              StopLoadingMenuDetails = true;
              // if there is no views defined :
              if (MyFilms.conf.CustomViews.View.Rows.Count == 0)
              {
                if (conf.AlwaysShowConfigMenu) // show config menu selection, if selected in setup on "leaving"
                {
                  if (!ChooseNewConfig()) GUIWindowManager.ShowPreviousWindow(); // if user "escapes", return to previous window / quit
                  return;
                }
                base.OnAction(action); // return to previous window ... // GUIWindowManager.ShowPreviousWindow(); 
                return;
              }
              conf.MenuSelectedID = -2; // -2 means coming from MenuAll
              GetSelectFromMenuView(false); // Call simple Menu ...
              #endregion
              break;
            case ViewContext.Menu:
              #region Menu
              StopLoadingMenuDetails = true;
              if (conf.AlwaysShowConfigMenu) // show config menu selection, if selected in setup on "leaving"
              {
                if (!ChooseNewConfig()) GUIWindowManager.ShowPreviousWindow(); // if user "escapes", return to previous window / quit
                return;
              }
              base.OnAction(action); // return to previous window ... // GUIWindowManager.ShowPreviousWindow(); 
              #endregion
              return;

            default:
              #region default backnavigation
              if (GetPrevFilmList()) // try moving up in film list hierarchy - is false, when already on top
                return;

              if (conf.StrTxtSelect == "" || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798622)) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798632))) //"All" or "Global Filter"
              {
                #region this was already "root" view - so jumping back should leave the plugin !
                base.OnAction(action); // GUIWindowManager.ShowPreviousWindow();
                return;
                #endregion
              }

              // if (conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(1079870)) || (conf.StrTxtSelect == "" && conf.Boolselect)) //original code block refactored // 1079870 = "Selection" 
              if (conf.StrTxtSelect == (GUILocalizeStrings.Get(1079870)) || (conf.StrTxtSelect == "" && conf.Boolselect) || (conf.Boolreturn && IsPersonField(conf.WStrSort)))
              {
                #region switch to selected root node here ... full list or menu
                if (Helper.FieldIsSet(conf.StrViewDfltItem) && conf.StrViewDfltItem != GUILocalizeStrings.Get(1079819) && conf.AlwaysDefaultView) // 1079819 = Menu
                {
                  // DisplayAllMovies();

                  conf.Boolselect = false;
                  conf.Boolreturn = false;
                  conf.StrSelect = conf.StrTxtSelect = conf.StrFilmSelect = "";
                  conf.StrIndex = 0;
                  conf.Boolindexed = false;
                  conf.Boolindexedreturn = false;

                  viewcover.Filename = "";
                  personcover.Filename = "";
                  groupcover.Filename = "";

                  GetFilmList();
                  SetLabelView("all");
                  // SetDummyControlsForFacade(conf.ViewContext);

                  // base.OnAction(action); // return to previous window ... // GUIWindowManager.ShowPreviousWindow(); 
                }
                else
                {
                  GetSelectFromMenuView(conf.BoolMenuShowAll);  // Call Menu with last detail state ...
                }
                return;
                #endregion
              }

              if (conf.Boolreturn)
              #region boolreturn returns to view here
              {
                conf.Boolreturn = false;
                Change_View_Action(conf.WStrSort);
                return;
              }
              #endregion

              if (conf.Boolselect)
              #region boolselect returns to defaultview or menu here
              {
                {
                  // switch to selected root node here ...
                  if (Helper.FieldIsSet(conf.StrViewDfltItem) && conf.AlwaysDefaultView)
                  {
                    SetLabelView("all");

                    conf.ViewContext = MyFilms.ViewContext.StartView;
                    conf.StrIndex = -1;
                    conf.LastID = -1;
                    conf.Wstar = "";
                    conf.Boolreturn = false;
                    conf.Boolselect = true;
                    conf.Boolindexed = false;
                    conf.Boolindexedreturn = false;

                    viewcover.Filename = "";
                    personcover.Filename = "";
                    groupcover.Filename = "";

                    conf.Wselectedlabel = conf.StrViewDfltText;
                    Change_View_Action(conf.StrViewDfltItem);
                  }
                  else
                  {
                    GetSelectFromMenuView(conf.BoolMenuShowAll);
                  }
                  return;
                }
              }
              #endregion

              // switch to selected root node here ...
              if (Helper.FieldIsSet(conf.StrViewDfltItem) && conf.StrViewDfltItem != GUILocalizeStrings.Get(1079819) && conf.AlwaysDefaultView) // 1079819 = Menu
              {
                base.OnAction(action); // return to previous window ... // GUIWindowManager.ShowPreviousWindow(); 
              }
              else
              {
                GetSelectFromMenuView(conf.BoolMenuShowAll);  // Call Menu with last detail state ...
              }
              return;

            // GetSelectFromMenuView(conf.BoolMenuShowAll);  // Call Menu with last detail state ...
            // base.OnAction(action); // return to previous window, if no backnavigation rule applies
              #endregion
          }
          break;
        case Action.ActionType.ACTION_KEY_PRESSED:
          base.OnAction(action);
          break;
        case Action.ActionType.ACTION_MUSIC_PLAY:
        case Action.ActionType.ACTION_PLAY:
          // Play groups as playlist (ToDo)
          if (this.facadeFilms.Focus && !this.facadeFilms.SelectedListItem.IsFolder)
          {
            conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
            conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
            MyFilmsDetail.Launch_Movie(this.facadeFilms.SelectedListItem.ItemId, GetID, null, false);
          }
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
            if ((action.m_key.KeyChar == 112) && this.facadeFilms.Focus && !this.facadeFilms.SelectedListItem.IsFolder) // 112 = "p", 120 = "x"
            {
              conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
              conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
              MyFilmsDetail.Launch_Movie(this.facadeFilms.SelectedListItem.ItemId, GetID, null, false);
            }
            if ((action.m_key.KeyChar == 120) && Context_Menu)
            {
              Context_Menu = false;
              return;
            }
            if (action.m_key.KeyChar == 120 && this.facadeFilms.Focus && !this.facadeFilms.SelectedListItem.IsFolder)
            {
              Context_Menu_Movie(this.facadeFilms.SelectedListItem.ItemId); // context menu for update or suppress entry
              return;
            }
          }
          if (action.wID.ToString().Substring(0, 6) == "REMOTE") return;
          base.OnAction(action);
          break;
      }
    }

    ////---------------------------------------------------------------------------------------
    ////   Handle Clicked Events
    ////---------------------------------------------------------------------------------------
    //protected override void OnClicked(int iControl, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
    //{
    //  LogMyFilms.Debug("OnClicked() started");
    //  //if (control == this.viewMenuButton)
    //  //{
    //  //  showViewSwitchDialog();
    //  //  viewMenuButton.Focus = false;
    //  //  return;
    //  //}
    //  //---------------------------------------------------------------------------------------
    //  // Mouse/Keyboard Clicked
    //  //---------------------------------------------------------------------------------------
    //  if (iControl == (int)Controls.CTRL_ListFilms)
    //  #region Item selected in facade - do action ...
    //  {
    //    if (facadeFilms.SelectedListItemIndex > -1)
    //    {
    //      LogStatusVars("SelectItem");
    //      if (actionType == Action.ActionType.ACTION_SELECT_ITEM)
    //      {
    //        switch (conf.ViewContext)
    //        {
    //          case ViewContext.None:
    //            break;
    //          case ViewContext.Menu:
    //          case ViewContext.MenuAll:
    //            if (this.facadeFilms.SelectedListItem.DVDLabel == "showall")
    //            {
    //              conf.MenuSelectedID = -1;
    //              GetSelectFromMenuView(true);
    //            }
    //            else
    //            {
    //              conf.MenuSelectedID = this.facadeFilms.SelectedListItemIndex; // remember last menu position ...
    //              Change_View_Action(this.facadeFilms.SelectedListItem.DVDLabel);
    //            }
    //            break;

    //          default:
    //            if (conf.IndexedChars > 0 && conf.Boolindexed && conf.Wstar == "*") // enter indexed view ...
    //            {
    //              conf.Boolindexed = false;
    //              conf.Boolindexedreturn = true;
    //              getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, this.facadeFilms.SelectedListItem.Label, true, ""); // conf.StrSelect = conf.WStrSort + " like '*" + conf.Wstar + "*'"; // only for actors ! - so do it later in method ...
    //            }
    //            else if (!this.facadeFilms.SelectedListItem.IsFolder && !conf.Boolselect) // New Window for detailed selected item information
    //            {
    //              conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
    //              conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
    //              GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
    //            }
    //            else // View List as selected
    //            {
    //              conf.Wselectedlabel = (conf.BoolReverseNames && this.facadeFilms.SelectedListItem.Label != EmptyFacadeValue) ? ReReverseName(conf.Wselectedlabel) : this.facadeFilms.SelectedListItem.Label.Replace(EmptyFacadeValue, ""); // Replace "pseudolabel" with empty value
    //              Change_Layout_Action(MyFilms.conf.StrLayOut);
    //              conf.Boolreturn = (!this.facadeFilms.SelectedListItem.IsFolder);
    //              do
    //              {
    //                if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
    //                conf.StrTitleSelect += conf.Wselectedlabel;
    //              }
    //              while (GetFilmList() == false); //keep calling while single folders found
    //            }
    //            break;
    //        }
    //      }
    //      else if(actionType == Action.ActionType.ACTION_MUSIC_PLAY || actionType == Action.ActionType.ACTION_PLAY)
    //      {
    //        // add filmstart option here
    //      }
    //    }
    //  }
    //  #endregion
    //  else if (iControl == (int)Controls.CTRL_BtnSearch)
    //  {
    //    if (conf.Boolselect) conf.Boolselect = false;
    //    Change_Search_Options();
    //  }
    //  else if (iControl == (int)Controls.CTRL_BtnSortBy)
    //  {
    //    this.Change_Sort_Option_Menu();
    //  }
    //  else if (iControl == (int)Controls.CTRL_BtnViewAs)
    //  {
    //    this.Change_View_Menu();
    //  }
    //  else if (iControl == (int)Controls.CTRL_BtnOptions)
    //  {
    //    Change_Option();
    //    // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // Added to return to facade // Reremoved for Doug !
    //  }
    //  else if (iControl == (int)Controls.CTRL_BtnGlobalUpdates)
    //  {
    //    Change_Menu_Action("globalupdates");
    //    // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // Added to return to facade // Reremoved for Doug !
    //  }

    //  if (iControl == (int)Controls.CTRL_BtnGlobalOverlayFilter)
    //  {
    //    Change_Global_Filters();
    //    // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // Added to return to facade // Reremoved for Doug !
    //  }

    //  else if (iControl == (int)Controls.CTRL_BtnToggleGlobalUnwatchedStatus)
    //  {
    //    ToggleGlobalUnwatched();
    //    // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // Added to return to facade // Reremoved for Doug !
    //  }
    //  else if (iControl == (int)Controls.CTRL_BtnLayout)
    //  {
    //    Change_Layout();
    //    GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
    //  }
    //  if (actionType != MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?
    //  base.OnClicked(iControl, control, actionType);
    //}

    //---------------------------------------------------------------------------------------
    //   Handle posted Messages
    //---------------------------------------------------------------------------------------
    public override bool OnMessage(GUIMessage message)
    {
      int dControl = message.TargetControlId;
      int iControl = message.SenderControlId;
      LogMyFilms.Debug("GUIMessage: " + message.Message + ", Param1: " + message.Param1 + ", Sender: " + iControl + ", Target: " + dControl + "");
      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
          return base.OnMessage(message);

        case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
          return base.OnMessage(message);

        case GUIMessage.MessageType.GUI_MSG_CLICKED:
          // return base.OnMessage(message); // enable this, if clicked messages should be handled in OnClicked override ...
          #region MSG_CLICKED
          //---------------------------------------------------------------------------------------
          // Mouse/Keyboard Clicked
          //---------------------------------------------------------------------------------------
          // LogMyFilms.Debug("GUI_MSG_CLICKED recognized !");

          if (iControl == (int)Controls.CTRL_ListFilms && message.Param1 != 7) return true;  // we only handle "SELECT_ITEM" here - some other events raised onClicked too for some reason?

          switch (iControl)
          {
            case (int)Controls.CTRL_ListFilms:
              #region Item selected in facade - do action ...
              if (this.facadeFilms.SelectedListItemIndex > -1) // if (facadeFilms.SelectedListItemIndex > -1 && !bgOnPageLoad.IsBusy) // do not allow going to details when loading thread still active !!!
              {
                // LogStatusVars("SelectItem");
                #region context dependant actions
                switch (conf.ViewContext)
                {
                  case ViewContext.None:
                    break;
                  case ViewContext.Menu:
                  case ViewContext.MenuAll:
                    // if (conf.ViewContext == ViewContext.MenuAll) 
                    SaveListState(false); // save current display to navigation cache
                    switch (this.facadeFilms.SelectedListItem.DVDLabel)
                    {
                      case "showall":
                        conf.MenuSelectedID = -1;
                        this.GetSelectFromMenuView(true);
                        break;
                      case "onlineinfo":
                        this.GetSelectFromOnlineMenuView();
                        break;
                      case "TMDBaction":
                        this.GetSelectFromTMDB(this.facadeFilms.SelectedListItem.Label);
                        break;
                      default:
                        conf.MenuSelectedID = this.facadeFilms.SelectedListItemIndex; // remember last menu position ...
                        this.StopLoadingMenuDetails = true;
                        this.Change_View_Action(this.facadeFilms.SelectedListItem.DVDLabel);
                        break;
                    }
                    break;

                  default:
                    if (conf.IndexedChars > 0 && conf.Boolindexed && conf.Wstar == "*" && !conf.BoolSkipViewState)
                    {
                      #region enter indexed view ...
                      StopLoadingMenuDetails = true;
                      StopLoadingViewDetails = true;
                      SaveListState(false);
                      conf.Boolindexed = false;
                      conf.Boolindexedreturn = true;

                      new Thread(delegate()
                        {
                          GUIWaitCursor.Init(); GUIWaitCursor.Show();
                          getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, facadeFilms.SelectedListItem.Label, true, ""); // conf.StrSelect = conf.WStrSort + " like '*" + conf.Wstar + "*'"; // only for actors ! - so do it later in method ...
                          GUIWaitCursor.Hide();
                          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) => { return 0; }, 0, 0, null);
                        }) { Name = "GetSelectFromDivx", IsBackground = true }.Start();
                      #endregion
                    }
                    else if (!this.facadeFilms.SelectedListItem.IsFolder && !conf.Boolselect)
                    {
                      #region open detailed window for selected item details information
                      // SaveListState(false); // already done on OnPageDestroy !
                      conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
                      conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
                      GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
                      #endregion
                    }
                    else
                    {
                      #region  View List as selected
                      StopLoadingViewDetails = true;
                      StopLoadingMenuDetails = true;
                      //conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
                      //conf.Boolselect = true;
                      //conf.Boolreturn = true;
                      //conf.Wstar = "*";
                      //if (conf.Wstar != "*") conf.StrTxtSelect += " " + GUILocalizeStrings.Get(344) + " [*" + conf.Wstar + "*]";
                      //// TxtSelect.Label = conf.StrTxtSelect;
                      //MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
                      //conf.StrSelect = conf.WStrSort;

                      //conf.StrFilmSelect = string.Empty;
                      //conf.Wselectedlabel = (selectedCustomView.Value != "*") ? selectedCustomView.Value : "";
                      //do
                      //{
                      //  if (conf.StrTitleSelect != string.Empty) conf.StrTitleSelect += conf.TitleDelim;
                      //  conf.StrTitleSelect += selectedCustomView.Value;
                      //}
                      //while (GetFilmList() == false); //keep calling while single folders found

                      SaveListState(false); // don't cache GetFilmList()
                      conf.Wselectedlabel = (conf.BoolReverseNames && facadeFilms.SelectedListItem.Label != EmptyFacadeValue) ? ReReverseName(facadeFilms.SelectedListItem.Label) : facadeFilms.SelectedListItem.Label.Replace(EmptyFacadeValue, ""); // Replace "pseudolabel" with empty value
                      conf.Boolreturn = (!this.facadeFilms.SelectedListItem.IsFolder);
                      new Thread(delegate()
                      {
                        do
                        {
                          if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                          conf.StrTitleSelect += conf.Wselectedlabel;
                        }
                        while (GetFilmList() == false); //keep calling while single folders found
                        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) => { return 0; }, 0, 0, null);
                      }) { Name = "GetFilmList", IsBackground = true }.Start();
                      #endregion
                    }
                    break;
                }
                #endregion
              }
              #endregion
              break;
            case (int)Controls.CTRL_BtnSearch: // Search dialog search
              // conf.Boolselect = false; // removed, as it was breaking the back navigation stack !!!
              Change_Search_Options();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnSearch);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnSortBy:
              Change_Sort_Option_Menu();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnSortBy);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnViewAs:
              Change_View_Menu();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnViewAs);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnOptions:
              Change_Option();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnOptions);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnGlobalUpdates:
              Change_Menu_Action("globalupdates");
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnGlobalUpdates);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnGlobalOverlayFilter:
              Change_Global_Filters();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnGlobalOverlayFilter);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnToggleGlobalUnwatchedStatus:
              ToggleGlobalUnwatched();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnToggleGlobalUnwatchedStatus);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
            case (int)Controls.CTRL_BtnLayout:
              Change_Layout();
              GUIControl.UnfocusControl(GetID, (int)Controls.CTRL_BtnLayout);
              // GUIControl.RefreshControl(GetID, (int)Controls.CTRL_BtnLayout);
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              break;
          }
          #endregion
          return base.OnMessage(message);
      }
      return base.OnMessage(message);
    }
    #endregion

    #region TMDB methods
    private static IEnumerable<PopularMovie> GetPopularMovies(bool all)
    {
      //string response = Transmit(TraktURIs.TrendingMovies, GetUserAuthentication());
      //return response.FromJSONArray<TraktTrendingMovie>();
      const int maxresults = 100;
      List<PopularMovie> movies = new List<PopularMovie>();
      string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
      LogMyFilms.Debug("GetPopularMovies - detected language = '" + language + "', all = '" + all + "'");
      Tmdb api = new TMDB.Tmdb(TmdbApiKey, language); // language is optional, default is "en"
      TmdbConfiguration tmdbConf = api.GetConfiguration();

      watch.Reset(); watch.Start();
      TmdbPopular popular;
      int ipage = 1;
      while (true)
      {
        popular = api.GetPopularMovies(ipage, language);
        LogMyFilms.Debug("GetPopularMovies() - Loaded Page: " + ipage + " (of " + popular.total_pages + "), Total Results = '" + popular.total_results + "', (" + (watch.ElapsedMilliseconds) + " ms)");
        movies.AddRange(popular.results);
        ipage++;
        if (ipage > popular.total_pages || !all || popular.total_results > maxresults) break;
      }
      watch.Stop();
      LogMyFilms.Debug("'loaded all movies from TMDB (" + (watch.ElapsedMilliseconds) + " ms)");
      return movies.AsEnumerable();
    }


    #endregion

    private void Change_Layout()
    {
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      List<GUIFacadeControl.Layout> choiceLayoutMenu = new List<GUIFacadeControl.Layout>();

      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079901)); // View (Layout) ...

      //List = 0,
      //SmallIcons = 1,
      //LargeIcons = 2,
      //Filmstrip = 3,
      //AlbumView = 4,
      //Playlist = 5,
      //CoverFlow = 6

      dlg.Add(GUILocalizeStrings.Get(101));//List
      choiceLayoutMenu.Add(GUIFacadeControl.Layout.List);

      if (conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll) // if (!conf.UseListViewForGoups || !conf.Boolselect)
      {
        dlg.Add(GUILocalizeStrings.Get(529));//Cover list - used as extended list
        choiceLayoutMenu.Add(GUIFacadeControl.Layout.AlbumView);
        dlg.Add(GUILocalizeStrings.Get(100));//Icons
        choiceLayoutMenu.Add(GUIFacadeControl.Layout.SmallIcons);
        dlg.Add(GUILocalizeStrings.Get(417));//Large Icons
        choiceLayoutMenu.Add(GUIFacadeControl.Layout.LargeIcons);
        dlg.Add(GUILocalizeStrings.Get(733));//Filmstrip
        choiceLayoutMenu.Add(GUIFacadeControl.Layout.Filmstrip);
        dlg.Add(GUILocalizeStrings.Get(791));//Coverflow
        choiceLayoutMenu.Add(GUIFacadeControl.Layout.CoverFlow);
      }
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1) return;
      // int iLayout = (int)choiceLayoutMenu[dlg.SelectedLabel];

      if (facadeFilms.AlbumListLayout == null && dlg.SelectedLabel == 1) // return, if layout not supported by skin !
      {
        MyFilmsDetail.ShowNotificationDialog("MyFilms System Information", "Your skin does not support this layout !");
        dlg.SelectedLabel = 0;
      }

      conf.StrIndex = 0; // reset movie index
      int wselectindex = facadeFilms.SelectedListItemIndex;

      Change_Layout_Action(dlg.SelectedLabel);

      if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll)
      {
        MyFilms.conf.WStrLayOut = dlg.SelectedLabel; // we share Layout for menu with Views ...
      }
      else if (conf.Boolselect)
        MyFilms.conf.WStrLayOut = dlg.SelectedLabel;
      else if (conf.BoolCollection)
        MyFilms.conf.StrLayOutInHierarchies = dlg.SelectedLabel;
      else
        MyFilms.conf.StrLayOut = dlg.SelectedLabel;
      dlg.DeInit();

      if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll)
        GetSelectFromMenuView(conf.BoolMenuShowAll);
      else if (conf.Boolselect)
        getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, true, "");
      else
        GetFilmList();
      GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, (int)wselectindex);
    }

    private void ToggleGlobalUnwatched()
    {
      MyFilms.conf.GlobalUnwatchedOnly = !MyFilms.conf.GlobalUnwatchedOnly;
      if (conf.GlobalUnwatchedOnly)
      {
        //GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
        GlobalFilterStringUnwatched = CreateGlobalUnwatchedFilter();
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
    }

    private string ReverseName(string nonreversedName)
    {
      if (nonreversedName.Contains(" ")) // Reverse Names "Bruce Willis" -> "Willis, Bruce"
        return nonreversedName.Substring(nonreversedName.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1) + ", " + nonreversedName.Substring(0, nonreversedName.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase));
      else
        return nonreversedName;
    }

    private string ReReverseName(string reversedName)
    {
      if (reversedName.Contains(", ")) // "Re"Reverse Names "Willis, Bruce" --> "Bruce Willis"
        return reversedName.Substring(reversedName.LastIndexOf(", ", System.StringComparison.Ordinal) + 2) + " " + reversedName.Substring(0, reversedName.LastIndexOf(", ", System.StringComparison.Ordinal));
      else
        return reversedName;
    }

    public static ArrayList GetDisplayItems(string displayoption)
    {
      var itemsToDisplay = new ArrayList();
      using (var ds = new AntMovieCatalog())
      {
        foreach (DataColumn dc in ds.Movie.Columns)
        {
          if (
              (dc.ColumnName != null && dc.ColumnName != "Contents_Id" && dc.ColumnName != "Movie_Id" &&
               dc.ColumnName != "IsOnline" && dc.ColumnName != "IsOnlineTrailer" && dc.ColumnName != "LastPosition" && dc.ColumnName != "MultiUserState" && dc.ColumnName != "VirtualPathTitle")
              &&
              ((conf.StrFileType != Configuration.CatalogType.AntMovieCatalog3 || MyFilmsDetail.ExtendedStartmode("Restrict DB field Selection for AMC3")) ||
              (dc.ColumnName != "IMDB_Id" && dc.ColumnName != "TMDB_Id" && dc.ColumnName != "Watched" && dc.ColumnName != "Certification" &&
               dc.ColumnName != "Writer" && dc.ColumnName != "SourceTrailer" && dc.ColumnName != "TagLine" && dc.ColumnName != "Tags" && dc.ColumnName != "RatingUser" && dc.ColumnName != "Studio" &&
               dc.ColumnName != "IMDB_Rank" && dc.ColumnName != "Edition" && dc.ColumnName != "Aspectratio" && dc.ColumnName != "AudioChannelCount" && dc.ColumnName != "CategoryTrakt" &&
               dc.ColumnName != "Favorite" && dc.ColumnName != "Fanart" && dc.ColumnName != "AlternateTitles" && dc.ColumnName != "DateWatched"))
            )
          {
            switch (displayoption)
            {
              case "sort":
                if (//dc.ColumnName != "OriginalTitle" && dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "FormattedTitle" && 
                    dc.ColumnName != "IndexedTitle" && dc.ColumnName != "Comments" &&
                    dc.ColumnName != "Description" && // dc.ColumnName != "URL" && 
                    dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "Picture" && dc.ColumnName != "Fanart" && dc.ColumnName != "AlternateTitles"
                  )
                  itemsToDisplay.Add(new string[] { dc.ColumnName, BaseMesFilms.Translate_Column(dc.ColumnName) });
                break;
              case "view":
                if (
                  //dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
                    dc.ColumnName != "Description" && dc.ColumnName != "Comments" && dc.ColumnName != "Picture" && dc.ColumnName != "Fanart" && dc.ColumnName != "AlternateTitles"
                  )
                  itemsToDisplay.Add(new string[] { dc.ColumnName, BaseMesFilms.Translate_Column(dc.ColumnName) });
                break;
              case "deletedetails":
                if (
                    dc.ColumnName != MyFilms.conf.StrStorage && dc.ColumnName != MyFilms.conf.StrStorageTrailer &&
                    dc.ColumnName != MyFilms.conf.StrTitle1 && // dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
                    dc.ColumnName != "Number"
                  )
                  itemsToDisplay.Add(new string[] { dc.ColumnName, BaseMesFilms.Translate_Column(dc.ColumnName) });
                break;
              case "titles":
                if (dc.ColumnName == "TranslatedTitle" || dc.ColumnName == "OriginalTitle" || dc.ColumnName == "FormattedTitle") // || dc.ColumnName == "VirtualPathTitle" || dc.ColumnName == "AlternateTitles"
                  itemsToDisplay.Add(new string[] { dc.ColumnName, BaseMesFilms.Translate_Column(dc.ColumnName) });
                break;
              case "viewitems":
                if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "Picture" && dc.ColumnName != "Fanart") // added "DatedAdded" to remove filter
                  itemsToDisplay.Add(new string[] { dc.ColumnName, BaseMesFilms.Translate_Column(dc.ColumnName) });
                break;

              default:
                if (dc.ColumnName == "MediaLabel" || dc.ColumnName == "MediaType" || dc.ColumnName == "Source" || (dc.ColumnName == "SourceTrailer" && conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended) ||
                    dc.ColumnName == "URL" || dc.ColumnName == "Comments" || dc.ColumnName == "Borrower" ||
                    dc.ColumnName == "Languages" || dc.ColumnName == "Subtitles")
                {
                  //AntStorage.Items.Add(dc.ColumnName);
                  //AntStorageTrailer.Items.Add(dc.ColumnName);
                }

                if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded") // added "DatedAdded" to remove filter
                {
                  //AntFilterItem1.Items.Add(dc.ColumnName);
                  //AntFilterItem2.Items.Add(dc.ColumnName);
                }
                if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded")
                {
                  //AntSearchField.Items.Add(dc.ColumnName);
                  //AntUpdField.Items.Add(dc.ColumnName);
                }
                if (dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
                    dc.ColumnName != "Actors" && dc.ColumnName != "DateAdded" &&
                    dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "AgeAdded" && dc.ColumnName != "IndexedTitle")
                {
                  //AntSearchItem1.Items.Add(dc.ColumnName);
                  //AntSearchItem2.Items.Add(dc.ColumnName);
                }
                if (dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
                    dc.ColumnName != "Year" && dc.ColumnName != "Date" && dc.ColumnName != "DateAdded" && // disabled for Doug testing
                    dc.ColumnName != "Length" && dc.ColumnName != "Rating" &&
                    dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "AgeAdded" && dc.ColumnName != "IndexedTitle")
                {
                  //AntSort1.Items.Add(dc.ColumnName);
                  //AntSort2.Items.Add(dc.ColumnName);
                  //AntIdentItem.Items.Add(dc.ColumnName);
                }
                if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded" && dc.ColumnName != "AgeAdded" && dc.ColumnName != "IndexedTitle")
                {
                  //AntUpdItem1.Items.Add(dc.ColumnName);
                  //AntUpdItem2.Items.Add(dc.ColumnName);
                  //cbfdupdate.Items.Add(dc.ColumnName);
                  //cbWatched.Items.Add(dc.ColumnName);
                  //CmdPar.Items.Add(dc.ColumnName);
                }
                break;
            }
          }
        }
      }
      // foreach (string[] strings in ItemsToDisplay) LogMyFilms.Debug("GetDisplayItems() - Unsorted: '" + strings[0] + "', '" + strings[1] + "'");
      IComparer myComp = new myDBfieldComparer();
      itemsToDisplay.Sort(0, itemsToDisplay.Count, myComp);
      // foreach (string[] strings in ItemsToDisplay) LogMyFilms.Debug("GetDisplayItems() - Sorted  : '" + strings[0] + "', '" + strings[1] + "'");
      LogMyFilms.Debug("GetDisplayItems() - returning '" + itemsToDisplay.Count + "' display items (sorted)");
      return itemsToDisplay;
    }

    private void LogStatusVars(string caller)
    {
      LogMyFilms.Debug(caller + string.Format("() - Lst_Detail - Prev_ItemID - Prev_Label : '{0}' - '{1}'", Prev_ItemID, Prev_Label));

      LogMyFilms.Debug(caller + string.Format("() - StrSelect          : '{0}'", conf.StrSelect));
      LogMyFilms.Debug(caller + string.Format("() - StrTitleSelect     : '{0}'", conf.StrTitleSelect));
      LogMyFilms.Debug(caller + string.Format("() - StrFilmSelect      : '{0}'", conf.StrFilmSelect));
      LogMyFilms.Debug(caller + string.Format("() - StrDfltSelect      : '{0}'", conf.StrDfltSelect));
      LogMyFilms.Debug(caller + string.Format("() - StrPersons         : '{0}'", conf.StrPersons));

      LogMyFilms.Debug(caller + string.Format("() - ViewContext        : '{0}'", Enum.GetName(typeof(MyFilms.ViewContext), MyFilms.conf.ViewContext)));
      LogMyFilms.Debug(caller + string.Format("() - StrTxtView         : '{0}' (View)", conf.StrTxtView));
      LogMyFilms.Debug(caller + string.Format("() - StrTxtSelect       : '{0}' (Selection)", conf.StrTxtSelect));
      //LogMyFilms.Debug(caller + string.Format("() - CurrentView        : '{0}'", conf.CurrentView.SaveToString()));

      LogMyFilms.Debug(caller + string.Format("() - Boolselect         : '{0}'", conf.Boolselect));
      LogMyFilms.Debug(caller + string.Format("() - Boolreturn         : '{0}'", conf.Boolreturn));
      LogMyFilms.Debug(caller + string.Format("() - Boolindexed        : '{0}'", conf.Boolindexed));
      LogMyFilms.Debug(caller + string.Format("() - Boolindexedreturn  : '{0}'", conf.Boolindexedreturn));
      LogMyFilms.Debug(caller + string.Format("() - IndexedChars       : '{0}'", conf.IndexedChars));

      LogMyFilms.Debug(caller + string.Format("() - WStrSort           : '{0}'", conf.WStrSort));
      LogMyFilms.Debug(caller + string.Format("() - Wselectedlabel     : '{0}'", conf.Wselectedlabel));
      LogMyFilms.Debug(caller + string.Format("() - Wstar              : '{0}'", conf.Wstar));

      LogMyFilms.Debug(caller + string.Format("() - StrSorta                       : '{0}'", conf.StrSorta));
      LogMyFilms.Debug(caller + string.Format("() - StrSortSens                    : '{0}'", conf.StrSortSens));

      LogMyFilms.Debug(caller + string.Format("() - StrSortaInHierarchies          : '{0}'", conf.StrSortaInHierarchies));
      LogMyFilms.Debug(caller + string.Format("() - StrSortSensInHierarchies       : '{0}'", conf.StrSortSensInHierarchies));

      LogMyFilms.Debug(caller + string.Format("() - StrLayOut (Layout)             : '{0}'", conf.StrLayOut));
      LogMyFilms.Debug(caller + string.Format("() - WStrLayOut               : '{0}'", conf.WStrLayOut));
      LogMyFilms.Debug(caller + string.Format("() - StrLayOutInHierarchies         : '{0}'", conf.StrLayOutInHierarchies));

      //LogMyFilms.Debug(caller + string.Format("() - LastID            : '{0}'", conf.LastID));
      //LogMyFilms.Debug(caller + string.Format("() - IndexItem: '{0}'", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItem.ToString()) : "-1"));
      //LogMyFilms.Debug(caller + string.Format("() - TitleItem: '{0}'", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItemLabel) : string.Empty));

      // LogMyFilms.Debug(caller + string.Format("() - StrUserProfileName: '{0}'", conf.StrUserProfileName));
      // LogMyFilms.Debug(caller + string.Format("() - StrSearchHistory: '{0}'", conf.StrSearchHistory));
    }

    /// <summary>Jumps to prev folder in FilmList  by modifying Selects and calling GetFilmList</summary>
    /// <returns>If returns false means cannot jump back any further, so caller must exit plugin to main menu.</returns>
    bool GetPrevFilmList()
    {
      // LogStatusVars("GetPrevFilmList");
      Prev_ItemID = -1;
      Prev_Label = string.Empty;
      // lastPublished = 0; // set timer to 0 to make sure no delayed publish is done (instead immediately !)

      string SelItem = "";

      if (!string.IsNullOrEmpty(conf.StrTitleSelect))
      {
        #region moving back in film list hierarchies
        SelItem = NewString.NPosRight(conf.TitleDelim, conf.StrTitleSelect, -1, false, false); // get last substring
        conf.StrTitleSelect = NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) > 0 ? NewString.NPosLeft(conf.TitleDelim, conf.StrTitleSelect, -1, false, false) : "";
        if (GetFilmList(SelItem) == false) // if single folder then call this func to jump back again
          return GetPrevFilmList();
        #endregion
      }
      else
      {
        return false;
        #region view handling (disabled for new backnavigation)
        //if (conf.StrTxtSelect == (GUILocalizeStrings.Get(1079870)) || (conf.StrTxtSelect == "" && conf.Boolselect)) //original code block refactored // 1079870 = "Selection" // if (conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(1079870)) || (conf.StrTxtSelect == "" && conf.Boolselect)) //original code block refactored // 1079870 = "Selection"
        //{
        //  #region jump back to main full list or menu
        //  conf.Boolselect = false;
        //  conf.Boolreturn = false;
        //  conf.StrSelect = conf.StrTxtSelect = conf.StrFilmSelect = "";
        //  conf.StrIndex = 0;
        //  conf.Boolindexed = false;
        //  conf.Boolindexedreturn = false;
        //  GetFilmList();
        //  SetLabelView("all"); // if back on "root", show view as "movies"
        //  SetLabelSelect("root");
        //  // SetDummyControlsForFacade(conf.ViewContext);
        //  #endregion
        //  return false;
        //}

        //if (conf.StrTxtSelect == "" || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798622)) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798632))) //"All" or "Global Filter"
        //{
        //  return false; //this was already "root" view - so jumping back should leave the plugin !
        //}
        //return false;
        //if (GetCustomViewFromViewLabel(conf.CurrentView) != null && GetCustomViewFromViewLabel(conf.CurrentView).Value.Length > 0) // if it's a custom view and a filter value is set
        //  return false;
        //else
        //{
        //  #region Jump back to prev view_display (categorised by year, genre etc) // Removed ACTORS special handling
        //  // SelItem = NewString.StripChars(@"[]", conf.StrTxtSelect); // Moved one up to first set SelItem to the actor and thus get back to correct facade position
        //  if (conf.StrTxtSelect.IndexOf(@"[") >= 0 && conf.StrTxtSelect.IndexOf(@"]") > 0)
        //  {
        //    SelItem = conf.StrTxtSelect.Substring(conf.StrTxtSelect.IndexOf(@"["));
        //    SelItem = SelItem.Substring(0, SelItem.IndexOf(@"]"));
        //    SelItem = NewString.StripChars(@"[]", SelItem);
        //    if (IsPersonField(conf.WStrSort) && conf.BoolReverseNames)
        //      SelItem = ReverseName(SelItem); // SelItem = ReReverseName(SelItem);
        //  }
        //  conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
        //  #endregion

        //  if (IsPersonField(conf.WStrSort) && !string.IsNullOrEmpty(conf.StrPersons)) // Specialhandling for Actors search view
        //  {
        //    conf.StrSelect = conf.WStrSort + " like '*" + StringExtensions.EscapeLikeValue(conf.StrPersons) + "*'";
        //    getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.StrPersons, true, SelItem);
        //    conf.StrPersons = "";
        //  }
        //  else
        //  {
        //    #region return indexed view
        //    if (conf.IndexedChars > 0 && conf.Boolindexedreturn)
        //    {
        //      if (conf.StrSelect == "")
        //      {
        //        conf.Boolindexedreturn = false;
        //        conf.Boolindexed = true;
        //        conf.Wstar = "*";
        //        // conf.Wstar = conf.WStrSort.Substring(0, 1); 
        //        SelItem = NewString.StripChars(@"*", SelItem); // remove stars, e.g. "K*" should be "K"
        //        // SelItem = SelItem.Substring(0, 1);
        //      }
        //      else
        //      {
        //        conf.StrSelect = ""; // reset view filter ...
        //      }
        //    }
        //    else
        //    {
        //      conf.StrSelect = ""; // reset view filter ...
        //      conf.Wstar = "*";
        //    }
        //    #endregion
        //    getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, true, SelItem); // getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, "*", true, SelItem);
        //  }
        //}
        #endregion
      }
      // SetDummyControlsForFacade(conf.ViewContext); // will be set in OnItemSelected handler / Lst_Details ...
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
        bool LabelNotEmpty = (sLabel != "");
        Type ColumnType = GetColumnType(conf.WStrSort);
        string datatype = "";
        using (AntMovieCatalog ds = new AntMovieCatalog())
        {
          datatype = ds.Movie.Columns[conf.WStrSort].DataType.Name; // == "string"
        }

        LogMyFilms.Debug(string.Format("(SetFilmSelect) - DB-Field: '{0}', datatype = '{1}', ColumnType = '{2}'", conf.WStrSort, datatype, ColumnType));
        if (ColumnType != typeof(string))
          conf.StrSelect = (LabelNotEmpty) ? conf.WStrSort + " = '" + sLabel + "'" : conf.WStrSort + " is NULL";
        else if (conf.WStrSort == "Date")
          conf.StrSelect = (LabelNotEmpty) ? "DateAdded" + " = '" + DateTime.Parse(sLabel).ToShortDateString() + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        else if (conf.WStrSort == "DateWatched")
          conf.StrSelect = (LabelNotEmpty) ? "DateWatched" + " = '" + DateTime.Parse(sLabel).ToShortDateString() + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        else if (IsDateField(conf.WStrSort))
          conf.StrSelect = (LabelNotEmpty) ? conf.WStrSort + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(sLabel).ToShortDateString()) + "*'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        else if (IsAlphaNumericalField(conf.WStrSort))
          conf.StrSelect = (LabelNotEmpty) ? conf.WStrSort + " like '" + StringExtensions.EscapeLikeValue(sLabel) + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        else
        {
          if (conf.BoolDontSplitValuesInViews)
            conf.StrSelect = (LabelNotEmpty) ? conf.WStrSort + " like '" + StringExtensions.EscapeLikeValue(sLabel) + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
          else
            conf.StrSelect = (LabelNotEmpty) ? conf.WStrSort + " like '*" + StringExtensions.EscapeLikeValue(sLabel) + "*'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        }

        conf.StrTxtSelect = (LabelNotEmpty) ? "[" + sLabel + "]" : "[" + EmptyFacadeValue + "]";
        conf.StrTitleSelect = "";
        conf.Boolselect = false;
        if (conf.BoolSkipViewState)
        {
          conf.StrSelect = (LabelNotEmpty) ? conf.WStrSort + " like '" + StringExtensions.EscapeLikeValue(sLabel) + "*'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
          // conf.StrTxtSelect = (LabelNotEmpty) ? "[" + sLabel + "*]" : "[" + EmptyFacadeValue + "]";
        }
      }
      else
      {
        conf.StrTxtSelect = NewString.NPosLeft(@"\", conf.StrTxtSelect, 1, false, false); //strip old path if any
        if (conf.StrTitleSelect != "") conf.StrTxtSelect += @"\" + conf.StrTitleSelect; // append new path if any
      }

      if (conf.StrSelect != "") s = conf.StrSelect + " And ";
      if (conf.StrTitleSelect != "")
        s = s + String.Format("{0} like '{1}{2}*'", (conf.BoolVirtualPathBrowsing) ? "VirtualPathTitle" : conf.StrTitle1, StringExtensions.EscapeLikeValue(conf.StrTitleSelect), conf.TitleDelim);
      else
        s = s + conf.StrTitle1 + " not like ''";
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
      MyFilmsDetail.clearGUIProperty("nbobjects.value"); // clear counts for the start ....
      GUIControl.ClearControl(GetID, facadeFilms.GetID); // ClearFacade(); // facadeFilms.Clear();

      SetFilmSelect();

      // set online filter only, if scan is done already ...
      MyFilms.conf.StrGlobalFilterString = (InitialIsOnlineScan) ? GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating : GlobalFilterStringUnwatched + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;

      LogMyFilms.Debug("(GetFilmList) - conf.GlobalFilterString:        '" + conf.StrGlobalFilterString + "'");
      LogMyFilms.Debug("(GetFilmList) - conf.StrViewSelect:             '" + conf.StrViewSelect + "'");
      LogMyFilms.Debug("(GetFilmList) - conf.StrDfltSelect:             '" + conf.StrDfltSelect + "'");
      LogMyFilms.Debug("(GetFilmList) - conf.StrFilmSelect:             '" + conf.StrFilmSelect + "'");

      #region set status of Collection for global use ...
      conf.BoolCollection = (conf.StrTitleSelect != "" && (NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) + 1) > 0); // here we set, if it is a collection or normal filmlist ...
      bool isvirtualpathview = (conf.BoolVirtualPathBrowsing);
      string sortascending = "";
      string sortfield = "";
      if (isvirtualpathview) // special mode "virtual path title browsing"
      {
        sortfield = "VirtualPathTitle";
        sortascending = " ASC";
        conf.BoolCollection = false;
        LogMyFilms.Debug("(GetFilmList) - special case: set sort field to 'VirtualPathTitle' for virtual path browsing ! -> sortfield = '" + sortfield + "'");
      }
      else
      {
        if (conf.BoolCollection && Helper.FieldIsSet(conf.StrSortaInHierarchies)) // only use Collection sort, if there is a value - otherwise use default
        {
          LogMyFilms.Debug("(GetFilmList) - conf.StrSortaInHierarchies:    '" + conf.StrSortaInHierarchies + "'");
          LogMyFilms.Debug("(GetFilmList) - conf.StrSortSensInHierarchies: '" + conf.StrSortSensInHierarchies + "'");
          sortascending = conf.StrSortSensInHierarchies;
          sortfield = conf.StrSortaInHierarchies;
        }
        else
        {
          LogMyFilms.Debug("(GetFilmList) - conf.StrSorta:                  '" + conf.StrSorta + "'");
          LogMyFilms.Debug("(GetFilmList) - conf.StrSortSens:               '" + conf.StrSortSens + "'");
          sortascending = conf.StrSortSens;
          sortfield = conf.StrSorta;

          // special handling for Box Sets ! -> use mastertitle for sorting to get proper grouping of the collections/groups/box-sets
          if (conf.StrViewSelect.Contains(@"*\*"))
          {
            sortfield = conf.StrTitle1;
            LogMyFilms.Debug("(GetFilmList) - special case: set sort field to master title for box set ! -> sortfield = '" + sortfield + "'");
          }
        }
      }

      if (string.IsNullOrEmpty(sortfield))
      {
        sortfield = MyFilms.conf.StrTitle1;
        LogMyFilms.Error("(GetFilmList) - sort field not properly set - resetting to mastertitle to avoid sort exception !");
      }
      #endregion

      #region Set sort buttons (movie vs. collection) and set Layout

      BtnSrtBy.IsEnabled = !isvirtualpathview;
      BtnSrtBy.Label = (conf.BoolCollection) ? (GUILocalizeStrings.Get(96) + ((conf.StrSortaInHierarchies == conf.StrSTitle) ? GUILocalizeStrings.Get(103) : BaseMesFilms.Translate_Column(conf.StrSortaInHierarchies))) : (GUILocalizeStrings.Get(96) + ((conf.StrSorta == conf.StrSTitle) ? GUILocalizeStrings.Get(103) : BaseMesFilms.Translate_Column(conf.StrSorta)));
      BtnSrtBy.IsAscending = (conf.BoolCollection) ? (conf.StrSortSensInHierarchies == " ASC") : (conf.StrSortSens == " ASC");
      // BtnSrtBy.Disabled = conf.BoolCollection;

      Change_Layout_Action((conf.BoolCollection) ? conf.StrLayOutInHierarchies : conf.StrLayOut);
      #endregion

      conf.DbSelection = new string[] { conf.StrGlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, conf.StrFilmSelect, sortfield, sortascending, false.ToString(), true.ToString() };
      r = BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, conf.StrFilmSelect, sortfield, sortascending, false, true); // moved sorting to BaseFilms.ReadDataMovies - last boolean parameter added

      #region Additional sorting ... // MOVED TO BASE METHOD (READDATAMOVIES)
      //FieldType fieldType = GetFieldType(sortfield);
      //Type columnType = GetColumnType(sortfield);
      //string strColumnType = (columnType == null) ? "<invalid>" : columnType.ToString();

      //if (!string.IsNullOrEmpty(sortfield) && columnType == typeof(string)) // don't apply special sorting on "native" types - only on string types !
      //{
      //  LogMyFilms.Debug("GetFilmList() - sorting fieldtype = '" + fieldType + "', vartype = '" + strColumnType + "', sortfield = '" + sortfield + "', sortascending = '" + sortascending + "'");
      //  watch.Reset(); watch.Start();
      //  switch (fieldType)
      //  {
      //    case FieldType.Decimal:
      //      if (sortascending == " ASC")
      //      {
      //        IComparer myComparer = new myRatingComparer();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(a[sortfield], b[sortfield]));
      //      }
      //      else
      //      {
      //        IComparer myComparer = new myRatingComparer();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(b[sortfield], a[sortfield]));
      //        //r.Reverse();
      //      }
      //      break;
      //    case FieldType.AlphaNumeric:
      //      if (sortascending == " ASC")
      //      {
      //        IComparer myComparer = new AlphanumComparatorFast();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(a[sortfield], b[sortfield]));
      //      }
      //      else
      //      {
      //        IComparer myComparer = new myReverserAlphanumComparatorFast();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(a[sortfield], b[sortfield]));
      //        //r.Reverse();
      //      }
      //      break;
      //    case FieldType.Date:
      //      if (sortascending == " ASC")
      //      {
      //        IComparer myComparer = new myDateComparer();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(a[sortfield], b[sortfield]));
      //      }
      //      else
      //      {
      //        IComparer myComparer = new myDateReverseComparer();
      //        Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(a[sortfield], b[sortfield]));
      //        //IComparer myComparer = new myDateComparer();
      //        //Array.Sort<DataRow>(r, (a, b) => myComparer.Compare(b[sortfield], a[sortfield]));
      //      }
      //      break;
      //  }
      //  watch.Stop();
      //  LogMyFilms.Debug("GetFilmList() - additional sorting finished (" + (watch.ElapsedMilliseconds) + " ms)");
      //}
      #endregion

      int iCnt = 0;
      int delimCnt = 0;
      var item = new GUIListItem();
      string selItem = gSelItem.ToString();
      int iSelItem = (typeof(T) == typeof(int)) ? (Int32.Parse(selItem)) : -2;
      LogMyFilms.Debug("(GetFilmList) - SelItem/iSelItem:               '" + selItem + "'/'" + iSelItem + "'");

      var facadeDownloadItems = new List<GUIListItem>();

      // setlabels
      // MyFilmsDetail.setGUIProperty("select", (conf.StrTxtSelect == "") ? " " : conf.StrTxtSelect.Replace(conf.TitleDelim, @"\"));// always show as though folder path using \ regardless what sep is used
      SetLabelSelect((conf.StrTxtSelect == "") ? "" : conf.StrTxtSelect.Replace(conf.TitleDelim, @"\"));

      if (conf.StrTitleSelect != "") delimCnt = NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) + 1; //get num .'s in title

      #region Load the facade
      int number = -1;
      string sPrevTitle = "";
      int wfacadewiew = 0;
      bool isdate = IsDateField(conf.WStrSort);
      bool isPinIconsAvailable = LoadWatchedFlagPossible(); // do it only once, as it requires 4 IO ops

      foreach (DataRow sr in r)
      {
        number++;
        #region filter list by Wselectedlabel, if user is coming from group view
        if (conf.Boolreturn) //in case of selection by view verify if value correspond excatly to the searched string
        {
          ArrayList wTableau = Search_String(sr[conf.WStrSort].ToString());
          for (int wi = 0; wi < wTableau.Count; wi++)
          {
            string s = wTableau[wi].ToString();
            if (isdate)
            {
              if (string.Format("{0:dd/MM/yyyy}", DateTime.Parse(s).ToShortDateString()) ==
                  string.Format("{0:dd/MM/yyyy}", DateTime.Parse(conf.Wselectedlabel).ToShortDateString()))
              {
                goto suite;
              }
            }
            else
            {
              if (s.IndexOf(conf.Wselectedlabel, StringComparison.OrdinalIgnoreCase) >= 0) // if (s.ToLower().Contains(conf.Wselectedlabel.Trim().ToLower())) // if (string.Compare(s, conf.Wselectedlabel.Trim(), true) >= 0) // string.Compare(champselect, wchampselect, true) == 0
              {
                goto suite;
              }
            }
          }
          goto fin;
        }
        #endregion

      suite:
        #region suite - process facade item ...
        string currentImage = "";
        string sTitle;
        string masterTitle = sr[conf.StrTitle1].ToString();
        string sFullTitle = sTitle = (!isvirtualpathview) ? masterTitle : sr["VirtualPathTitle"] + ((masterTitle.Length > 0) ? @"\" : "") + ((masterTitle.LastIndexOf(conf.TitleDelim, System.StringComparison.Ordinal) > 0) ? masterTitle.Substring(masterTitle.LastIndexOf(conf.TitleDelim, System.StringComparison.Ordinal) + 1) : masterTitle);

        int delimCnt2 = NewString.PosCount(conf.TitleDelim, sTitle, false);
        if (delimCnt <= delimCnt2)
        {
          sTitle = NewString.NPosMid(conf.TitleDelim, sTitle, delimCnt, delimCnt + 1, false, false); //get current substring (folder) within path
          sFullTitle = NewString.NPosRight(conf.TitleDelim, sFullTitle, delimCnt, false, false); //current rest of path (if only 1 entry in subfolders will present entry ignoring folders)
        }

        if (iCnt > 0 && delimCnt < delimCnt2 && sTitle == sPrevTitle) // don't stack items already at lowest folder level
        {
          iCnt++;
          if (!isvirtualpathview)
          {
            item.Label2 = "(" + iCnt + ")  " + NewString.PosRight(")  ", item.Label2);// prepend (items in folder count)
            if (iCnt == 2) // iCnt == 1 is never reached here - changes to be done in main item load below !
            {
              item.Label = sTitle; //reset to current single folder as > 1 entries
              item.IsFolder = true;
              item.ThumbnailImage = currentImage;
              item.IconImage = currentImage;
            }
          }
          else // virtual path titles
          {
            item.Label2 = "(" + iCnt + ")";
            if (iCnt == 2) // iCnt == 1 is never reached here - changes to be done in main item load below !
            {
              item.Label = sTitle; //reset to current single folder as > 1 entries
              item.IsFolder = true;
              item.ThumbnailImage = "";
              item.IconImage = "";
              item.PinImage = "";
              item.IsRemote = false;
              item.TVTag = ""; // setting to not null skips imagesearch
              Utils.SetDefaultIcons(item);
            }
          }
        }
        else
        {
          iCnt = 1;
          item = new GUIListItem();
          item.ItemId = number;
          if (!isvirtualpathview)
            item.Label = (delimCnt < delimCnt2) ? sFullTitle.Substring(sFullTitle.LastIndexOf(conf.TitleDelim, System.StringComparison.Ordinal) + 1) : sFullTitle; // Set = full subfolders path initially - new: set only to last name
          else
            item.Label = (delimCnt < delimCnt2) ? sTitle : sr[conf.StrSTitle].ToString(); //reset to current single folder as > 1 entries

          item.DVDLabel = sTitle; // used by background thread
          item.IsFolder = (isvirtualpathview && delimCnt + 1 < delimCnt2); // show folders in virtual path browsing
          #region Label2 ...
          if (!MyFilms.conf.OnlyTitleList)
          {
            switch (sortfield)
            {
              case "VirtualPathTitle":
                item.Label2 = ((delimCnt + 1) < delimCnt2) ? "(" + iCnt + ")" : "";
                break;
              case "TranslatedTitle":
              case "OriginalTitle":
              case "FormattedTitle":
                item.Label2 = sr["Year"].ToString();
                break;
              case "Year":
                item.Label2 = sr["Year"].ToString();
                break;
              case "Date":
              case "DateAdded":
                try
                {
                  item.Label2 = DateTime.Parse(sr["Date"].ToString()).ToShortDateString(); //item.Label2 = ((DateTime)sr["DateAdded"]).ToShortDateString();
                }
                catch { }
                break;
              case "Rating":
                item.Label2 = sr["Rating"].ToString();
                break;
              default:
                string label2 = sr[sortfield].ToString(); // string label2 = sr[conf.WStrSort].ToString();
                if (IsSplittableField(sortfield))
                {
                  ArrayList wtab = Search_String(label2, false);
                  if (wtab.Count > 0) label2 = wtab[0].ToString();
                  if (wtab.Count > 1) label2 += " ...";
                }
                if (label2.Length > 34) label2 = label2.Substring(0, 30) + " ...";
                item.Label2 = label2;
                break;
            }
            if (!isvirtualpathview && delimCnt < delimCnt2) item.Label2 = "(" + iCnt + ")  " + NewString.PosRight(")  ", item.Label2);// prepend (items in folder count) //  && iCnt > 1 ??
          }
          #endregion
          if (!(isvirtualpathview && item.IsFolder))
          {
            #region Label3 ...

            item.Label3 = sr["Edition"].ToString() ?? "";

            #endregion

            #region Watched Status
            if (conf.EnhancedWatchedStatusHandling)
            {
              if (!sr[conf.StrMultiUserStateField].ToString().Contains(":")) // not yet migrated/created!
              {
                #region migrate status from configured (enhanced)watched field to new MultiUserStates
                MultiUserData userData;
                if (sr[MyFilms.conf.StrWatchedField].ToString().Contains(":"))
                {
                  // old field was already multiuserdata - migrate it!
                  userData = new MultiUserData(sr[conf.StrWatchedField].ToString());
                  sr[MyFilms.conf.StrWatchedField] = userData.GetUserState(MyFilms.conf.StrUserProfileName).Watched ? "true" : MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower();
                }
                else
                {
                  // old field was standard watched data - create MUS and add watched for current user
                  bool tmpwatched = (!string.IsNullOrEmpty(conf.GlobalUnwatchedOnlyValue) &&
                                sr[conf.StrWatchedField].ToString().ToLower() != conf.GlobalUnwatchedOnlyValue.ToLower() &&
                                sr[conf.StrWatchedField].ToString().Length > 0);
                  userData = new MultiUserData("");
                  userData.SetWatched(MyFilms.conf.StrUserProfileName, tmpwatched);
                  if (sr["RatingUser"] != Convert.DBNull)
                    userData.SetRating(MyFilms.conf.StrUserProfileName, (decimal)sr["RatingUser"]);
                }
                sr[MyFilms.conf.StrMultiUserStateField] = userData.ResultValueString();
                sr["DateWatched"] = userData.GetUserState(MyFilms.conf.StrUserProfileName).WatchedDate;
                sr["RatingUser"] = userData.GetUserState(MyFilms.conf.StrUserProfileName).UserRating == MultiUserData.NoRating ? Convert.DBNull : userData.GetUserState(MyFilms.conf.StrUserProfileName).UserRating;
                #endregion
              }
              item.IsPlayed = (EnhancedWatched(sr[conf.StrMultiUserStateField].ToString(), conf.StrUserProfileName));
            }
            else
            {
              //if (sr[conf.StrWatchedField].ToString().StartsWith("Global:"))
              //{
              //  string s = sr[conf.StrWatchedField].ToString();
              //  string count = s.Substring(s.IndexOf(":", System.StringComparison.Ordinal) + 1, 1);
              //  sr[conf.StrWatchedField] = (count == "0") ? conf.GlobalUnwatchedOnlyValue : "true";
              //}
              if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
                if (sr[conf.StrWatchedField].ToString().ToLower() != conf.GlobalUnwatchedOnlyValue.ToLower()) // changed to take setup config into consideration
                  item.IsPlayed = true;
            }
            if (MyFilms.conf.StrSuppress && MyFilms.conf.StrSuppressField.Length > 0)
              if ((sr[MyFilms.conf.StrSuppressField].ToString() == MyFilms.conf.StrSuppressValue) &&
                  (MyFilms.conf.StrSupPlayer)) item.IsPlayed = true;

            #endregion

            #region Availability Status

            // set availability status // only display media status, if onlinescan was done // if its online, set IsRemote to false !
            if (InitialIsOnlineScan)
              item.IsRemote = !(string.IsNullOrEmpty(sr["IsOnline"].ToString())) &&
                              !bool.Parse(sr["IsOnline"].ToString());
            // load special icons to indicate watched/available flags in listcontrol
            if (isPinIconsAvailable) LoadWatchedFlag(item, item.IsPlayed, !item.IsRemote);

            #endregion

            #region Cover Picture

            if (sr["Picture"].ToString().Length > 0)
            {
              if ((sr["Picture"].ToString().IndexOf(":\\", System.StringComparison.Ordinal) == -1) &&
                  (sr["Picture"].ToString().Substring(0, 2) != "\\\\")) currentImage = conf.StrPathImg + "\\" + sr["Picture"];
              else currentImage = sr["Picture"].ToString();
            }
            else
            {
              currentImage = string.Empty;
            }
            item.ThumbnailImage = currentImage;
            item.IconImageBig = currentImage;
            item.IconImage = currentImage;

            #endregion

            facadeDownloadItems.Add(item);
          }
          else
          {
            item.ThumbnailImage = "";
            item.IconImage = "";
            item.PinImage = "";
            item.IsRemote = false;
            item.TVTag = ""; // setting to not null skips imagesearch
            Utils.SetDefaultIcons(item);
          }

          item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
          facadeFilms.Add(item);

          if (iSelItem == -2) // selected item is passed as string in "SelItem"
          {
            if (sTitle == selItem) wfacadewiew = facadeFilms.Count - 1; //test if this item is one to select
          }
        }

        if (iSelItem >= 0) //-1 = ignore, >=0 = itemID to locate (test every item in case item is from within a folder)
        {
          if (!(conf.StrTIndex.Length > 0))
          {
            if (number == iSelItem) wfacadewiew = facadeFilms.Count - 1; //test if this item is one to select
          }
          else
          {
            if (number == iSelItem && sFullTitle == conf.StrTIndex) wfacadewiew = facadeFilms.Count - 1; //test if this item is one to select
          }
        }
        sPrevTitle = sTitle;
        #endregion
      fin: ;
      }
      #endregion

      #region post facade settings ...
      if (this.facadeFilms.Count == 0)
      {
        item = new GUIListItem();
        item.Label = GUILocalizeStrings.Get(10798639);
        item.IsRemote = true;
        this.facadeFilms.Add(item);
        ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798639));
        conf.ViewContext = ViewContext.Menu;
        GUIControl.ShowControl(GetID, 34); // hides certain GUI elements
      }
      else
      {
        Fanartstatus(true);
        GUIControl.HideControl(GetID, 34);
      }
      item.FreeMemory();
      MyFilmsDetail.setGUIProperty("nbobjects.value", r.Length.ToString(CultureInfo.InvariantCulture));

      GUIPropertyManager.SetProperty("#itemcount", this.facadeFilms.Count.ToString(CultureInfo.InvariantCulture));
      // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + "/" + GUIPropertyManager.GetProperty("#myfilms.select")); 
      if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
      //if (wfacadewiew == -1 && facadeFilms.Count > 0)
      //  GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, 0);
      //else
      //  GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, (int)wfacadewiew);
      GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, (int)wfacadewiew);

      if (this.facadeFilms.Count == 1 && item.IsFolder)
      {
        conf.Boolreturn = false;
        conf.Wselectedlabel = item.Label;
      }
      conf.ViewContext = (conf.BoolCollection) ? ViewContext.MovieCollection : ViewContext.Movie;
      LogMyFilms.Debug("GetFilmList finished!");

      // GetOnlineMoviesFromTMDBforFilmList(); // add Online Movies from TMDB
      GetImagesFilmList(facadeDownloadItems);
      #endregion
      return !(this.facadeFilms.Count == 1 && item.IsFolder); //ret false if single folder found
    }

    public static bool EnhancedWatched(string strEnhancedWatchedValue, string strUserProfileName)
    {
      if (strEnhancedWatchedValue.Contains(strUserProfileName + ":0")) return false;
      if (strEnhancedWatchedValue.Contains(strUserProfileName + ":-1")) return false;
      if (!strEnhancedWatchedValue.Contains(strUserProfileName + ":")) return false;
      return true; // count > 0 -> return true
    }

    private void GetOnlineMoviesFromTMDBforFilmList()
    {
      #region add online movies from TMDB
      if (MyFilmsDetail.ExtendedStartmode("GetFilmlist() - Online info on person film list") && facadeFilms.Count < 200 && IsPersonsField(conf.WStrSort)) // only in "related display"
      {
        var facadeDownloadItems = new List<GUIListItem>();
        bool IsPinIconsAvailable = LoadWatchedFlagPossible(); // do it only once, as it requires 4 IO ops
        string currentImage = "";
        //sortascending = conf.StrSortSens;
        //sortfield = conf.StrSorta;
        string sortascending;
        string sortfield;
        if (conf.BoolCollection && Helper.FieldIsSet(conf.StrSortaInHierarchies)) // only use Collection sort, if there is a value - otherwise use default
        {
          LogMyFilms.Debug("(GetFilmList) - conf.StrSortaInHierarchies:    '" + conf.StrSortaInHierarchies + "'");
          LogMyFilms.Debug("(GetFilmList) - conf.StrSortSensInHierarchies: '" + conf.StrSortSensInHierarchies + "'");
          sortascending = conf.StrSortSensInHierarchies;
          sortfield = conf.StrSortaInHierarchies;
        }
        else
        {
          LogMyFilms.Debug("(GetFilmList) - conf.StrSorta:                  '" + conf.StrSorta + "'");
          LogMyFilms.Debug("(GetFilmList) - conf.StrSortSens:               '" + conf.StrSortSens + "'");
          sortascending = conf.StrSortSens;
          sortfield = conf.StrSorta;
          // special handling for Box Sets ! -> use mastertitle for sorting to get proper grouping of the collections/groups/box-sets
          if (conf.StrViewSelect.Contains(@"*\*"))
          {
            sortfield = conf.StrTitle1;
            LogMyFilms.Debug("(GetFilmList) - special case: set sort field to master title for box set ! -> sortfield = '" + sortfield + "'");
          }
        }
        if (string.IsNullOrEmpty(sortfield))
        {
          sortfield = MyFilms.conf.StrTitle1;
          LogMyFilms.Error("(GetFilmList) - sort field not properly set - resetting to mastertitle to avoid sort exception !");
        }

        IComparer myComparer = new AlphanumComparatorFast();
        string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
        grabber.TheMoviedb tmdbapi = new grabber.TheMoviedb();
        TMDB.Tmdb api = new TMDB.Tmdb(TmdbApiKey, language); // language is optional, default is "en"
        TMDB.TmdbConfiguration tmdbConf = api.GetConfiguration();
        TmdbPersonSearch personSearch = api.SearchPerson(conf.Wselectedlabel, 1);
        GUIListItem item = null;
        LogMyFilms.Debug("GetFilmList() - OnlineInfo - '" + personSearch.results.Count + "' results found for '" + (conf.Wselectedlabel ?? "") + "' ('" + conf.WStrSort + "')!");
        if (personSearch.results.Count > 0)
        {
          TmdbPersonCredits personCredits = api.GetPersonCredits(personSearch.results[0].id);
          LogMyFilms.Debug("GetFilmList() - OnlineInfo - '" + personCredits.cast.Count + "' cast movies found for '" + (conf.Wselectedlabel ?? "") + "' ('" + conf.WStrSort + "')!");
          LogMyFilms.Debug("GetFilmList() - OnlineInfo - '" + personCredits.crew.Count + "' crew movies found for '" + (conf.Wselectedlabel ?? "") + "' ('" + conf.WStrSort + "')!");
          foreach (CastCredit personMovie in personCredits.cast)
          {
            bool toBeAddedToList = true;
            int iInsertAt = int.MaxValue;
            for (int i = 0; i < facadeFilms.Count; i++)
            {
              if (facadeFilms[i].Label == personMovie.title)
                toBeAddedToList = false;
              else
              {
                try
                {
                  if (sortfield == "Year" && personMovie.release_date.Length > 3)
                  {
                    if (sortascending == " ASC")
                    {
                      //if (Convert.ToInt32(personMovie.release_date.Substring(0, 4)) > Convert.ToInt32(facadeFilms[i].Label2)) iInsertAt = i;
                      if (myComparer.Compare(personMovie.release_date, facadeFilms[i].Label2) < 0) iInsertAt = i;
                    }
                    else
                    {
                      //if (Convert.ToInt32(personMovie.release_date.Substring(0, 4)) < Convert.ToInt32(facadeFilms[i].Label2)) iInsertAt = i;
                      if (myComparer.Compare(personMovie.release_date, facadeFilms[i].Label2) > 0) iInsertAt = i;
                    }
                  }
                }
                catch (Exception ex)
                {
                  LogMyFilms.Debug("GetFilmList() - Error in year parsing: " + ex.Message + " - " + ex.StackTrace);
                }
              }
            }
            LogMyFilms.Debug("GetFilmList() - OnlineInfo - add = '" + toBeAddedToList + "' for '" + personMovie.title + "' (" + personMovie.release_date + ") to facade at position '" + ((iInsertAt != int.MaxValue) ? iInsertAt.ToString() : "end") + "'");
            if (!toBeAddedToList) continue;

            #region add item
            item = new GUIListItem();
            item.Label = personMovie.title; //reset to current single folder as > 1 entries
            item.IsFolder = true;
            item.ThumbnailImage = conf.DefaultCover;
            item.IconImage = conf.DefaultCover;
            #region Label2 ...
            if (!MyFilms.conf.OnlyTitleList)
            {
              switch (sortfield)
              {
                case "TranslatedTitle":
                case "OriginalTitle":
                case "FormattedTitle":
                case "Year":
                  item.Label2 = (personMovie != null && personMovie.release_date.Length > 3) ? personMovie.release_date.Substring(0, 4) : (personMovie.release_date ?? "");
                  break;
                case "Date":
                case "DateAdded":
                  break;
                case "Rating":
                  break;
                default:
                  break;
              }
            }
            #endregion
            #region Label3 ...
            item.Label3 = personMovie.character + " (cast)";
            #endregion
            #region Watched Status
            item.IsPlayed = false;
            #endregion

            #region Availability Status
            item.IsRemote = true;
            // load special icons to indicate watched/available flags in listcontrol
            if (IsPinIconsAvailable) LoadWatchedFlag(item, item.IsPlayed, !item.IsRemote);
            #endregion

            #region Cover Picture
            currentImage = string.Empty;
            currentImage = tmdbConf.images.base_url + "w500" + personMovie.poster_path;
            item.ThumbnailImage = currentImage;
            item.IconImageBig = currentImage;
            item.IconImage = currentImage;
            item.TVTag = personMovie;
            #endregion

            facadeDownloadItems.Add(item);
            item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
            if (iInsertAt == int.MaxValue)
              facadeFilms.Add(item);
            else
              facadeFilms.Insert(0, item);
            #endregion
          }
          foreach (CrewCredit personMovie in personCredits.crew)
          {
            bool toBeAddedToList = true;
            int iInsertAt = int.MaxValue;
            for (int i = 0; i < facadeFilms.Count; i++)
            {
              if (facadeFilms[i].Label == personMovie.title)
                toBeAddedToList = false;
              else
              {
                try
                {
                  if (sortfield == "Year" && personMovie.release_date.Length > 3)
                  {
                    if (sortascending == " ASC")
                    {
                      //if (Convert.ToInt32(personMovie.release_date.Substring(0, 4)) > Convert.ToInt32(facadeFilms[i].Label2)) iInsertAt = i;
                      if (myComparer.Compare(personMovie.release_date, facadeFilms[i].Label2) < 0) iInsertAt = i;
                    }
                    else
                    {
                      //if (Convert.ToInt32(personMovie.release_date.Substring(0, 4)) < Convert.ToInt32(facadeFilms[i].Label2)) iInsertAt = i;
                      if (myComparer.Compare(personMovie.release_date, facadeFilms[i].Label2) > 0) iInsertAt = i;
                    }
                  }
                }
                catch (Exception ex)
                {
                  LogMyFilms.Debug("GetFilmList() - Error in year parsing: " + ex.Message + " - " + ex.StackTrace);
                }
              }
            }
            LogMyFilms.Debug("GetFilmList() - OnlineInfo - add = '" + toBeAddedToList + "' for '" + personMovie.title + "' (" + personMovie.release_date + ") to facade at position '" + ((iInsertAt != int.MaxValue) ? iInsertAt.ToString() : "end") + "'");
            if (!toBeAddedToList) continue;

            #region add item
            item = new GUIListItem();
            item.Label = personMovie.title; //reset to current single folder as > 1 entries
            item.IsFolder = true;
            item.ThumbnailImage = conf.DefaultCover;
            item.IconImage = conf.DefaultCover;
            #region Label2 ...
            if (!MyFilms.conf.OnlyTitleList)
            {
              switch (sortfield)
              {
                case "TranslatedTitle":
                case "OriginalTitle":
                case "FormattedTitle":
                case "Year":
                  item.Label2 = (personMovie != null && personMovie.release_date.Length > 3) ? personMovie.release_date.Substring(0, 4) : (personMovie.release_date ?? "");
                  break;
                case "Date":
                case "DateAdded":
                  break;
                case "Rating":
                  break;
                default:
                  break;
              }
            }
            #endregion
            #region Label3 ...
            item.Label3 = personMovie.job + " (" + personMovie.department + ")";
            #endregion
            #region Watched Status
            item.IsPlayed = false;
            #endregion

            #region Availability Status
            item.IsRemote = true;
            // load special icons to indicate watched/available flags in listcontrol
            if (IsPinIconsAvailable) LoadWatchedFlag(item, item.IsPlayed, !item.IsRemote);
            #endregion

            #region Cover Picture
            currentImage = string.Empty;
            currentImage = tmdbConf.images.base_url + "w500" + personMovie.poster_path;
            item.ThumbnailImage = currentImage;
            item.IconImageBig = currentImage;
            item.IconImage = currentImage;
            item.TVTag = personMovie;
            #endregion

            facadeDownloadItems.Add(item);
            item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
            facadeFilms.Insert(0, item);
            #endregion
          }
        }
        GetImagesFilmList(facadeDownloadItems);
      }
      #endregion
    }

    private void GetImagesFilmList(List<GUIListItem> itemsWithThumbs)
    {
      StopLoadingFilmlistDetails = false;
      string coverThumbDir = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies";

      // split the downloads in X+ groups and do multithreaded downloading
      int groupSize = (int)Math.Max(1, Math.Floor((double)itemsWithThumbs.Count / 2)); // Guzzi: Set group to x to only allow x thread(s)
      int groups = (int)Math.Ceiling((double)itemsWithThumbs.Count / groupSize);


      for (int i = 0; i < groups; i++)
      {
        var groupList = new List<GUIListItem>();
        for (int j = groupSize * i; j < groupSize * i + (groupSize * (i + 1) > itemsWithThumbs.Count ? itemsWithThumbs.Count - groupSize * i : groupSize); j++)
        {
          groupList.Add(itemsWithThumbs[j]);
        }

        new Thread(delegate(object o)
        {
          var items = (List<GUIListItem>)o;
          foreach (GUIListItem item in items)
          {
            // stop download if we have exited window
            if (StopLoadingFilmlistDetails)
            {
              items.SafeDispose();
              break;
            }
            if (item.TVTag != null) continue; // skip for online videos and virtual path title browsing

            #region group-collection image handling
            //if (!File.Exists(item.ThumbnailImage)) // No Coverart in DB - so handle it !
            if (item.IsFolder) // special handling for groups (movie collections - NOT views!)
            {
              string strThumbGroup = GetGroupImage(item); // thumbnail for Groups/collections
              item.IconImage = strThumbGroup;
              item.IconImageBig = strThumbGroup;
              item.ThumbnailImage = strThumbGroup;
            }
            #endregion

            #region create artifical covers with title - inactive
            if (item.ThumbnailImage == "" || !File.Exists(item.ThumbnailImage)) // No Coverart in DB - so handle it !
            {
              //string strlabel = item.Label;
              //MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(ref strlabel);
              //strThumb = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Views\" + strlabel;
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
            #endregion

            item.MusicTag = item.ThumbnailImage; // keep Original one in music tag for big list thumb ...
            // strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, item.DVDLabel); // item.DVDLabel is sTitle
            string strThumb = MediaPortal.Util.Utils.GetCoverArtName(coverThumbDir, item.DVDLabel); // cached cover
            if (string.IsNullOrEmpty(strThumb) || !strThumb.Contains(".")) continue;
            string strThumbSmall = strThumb.Substring(0, strThumb.LastIndexOf(".")) + "_s" + Path.GetExtension(strThumb);  // cached cover for Icons - small resolution

            if (!string.IsNullOrEmpty(item.ThumbnailImage) && item.ThumbnailImage != conf.DefaultCover && !File.Exists(strThumb))
            {
              Picture.CreateThumbnail(item.ThumbnailImage, strThumbSmall, 100, 150, 0, Thumbs.SpeedThumbsSmall);
              Picture.CreateThumbnail(item.ThumbnailImage, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
              LogMyFilms.Debug("GetFimList: Background thread creating thumbimage for sTitle: '" + item.DVDLabel + "'");
            }
            if (File.Exists(strThumb))
            {
              item.IconImage = strThumbSmall;
              item.IconImageBig = strThumb;
              item.ThumbnailImage = strThumb;
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
          IsBackground = true,
          Priority = ThreadPriority.BelowNormal,
          Name = "MyFilms FilmList Image Detector " + i
        }.Start(groupList);
      }
    }

    private string GetGroupImage(GUIListItem item)
    {
      // LogMyFilms.Debug("GetGroupImage() - item.Label = '" + (item.Label ?? "") + "', conf.StrTitleSelect = '" + conf.StrTitleSelect + "'");
      string strThumbGroup = ""; // thumbnail for Groups/collections
      if (File.Exists(conf.StrPathImg + "\\" + conf.StrTitleSelect.Replace(conf.TitleDelim, ".") + "." + item.Label + ".jpg")) // check for longnames for nested groups
      {
        strThumbGroup = conf.StrPathImg + "\\" + conf.StrTitleSelect.Replace(conf.TitleDelim, ".") + "." + item.Label + ".jpg";
      }
      else if (File.Exists(conf.StrPathImg + "\\" + conf.StrTitleSelect + "." + item.Label + ".jpg")) // check for nested directories
      {
        strThumbGroup = conf.StrPathImg + "\\" + conf.StrTitleSelect + "." + item.Label + ".jpg";
      }
      else if (File.Exists(conf.StrPathImg + "\\" + item.Label + ".jpg"))
      {
        strThumbGroup = conf.StrPathImg + "\\" + item.Label + ".jpg";
      }
      else if (File.Exists(conf.StrPathImg + "\\" + item.Label + ".png"))
      {
        strThumbGroup = conf.StrPathImg + "\\" + item.Label + ".png";
      }
      else if (File.Exists(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + item.Label + ".jpg")) // if (MyFilms.conf.PictureHandling == "Relative Path" || string.IsNullOrEmpty(MyFilms.conf.PictureHandling))
      {
        strThumbGroup = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + item.Label + ".jpg";
      }
      else if (MyFilms.conf.StrPicturePrefix.Contains("\\"))
      {
        if (File.Exists(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix.Substring(0, MyFilms.conf.StrPicturePrefix.LastIndexOf("\\")) + "\\" + item.Label + ".jpg"))
          strThumbGroup = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix.Substring(0, MyFilms.conf.StrPicturePrefix.LastIndexOf("\\")) + "\\" + item.Label + ".jpg";
        // if (strThumbGroup == "") strThumbGroup = MyFilms.conf.DefaultCover; // ToDo: Add default cover for hierarchies / box sets
      }
      LogMyFilms.Debug("GetGroupImage() - returning image = '" + strThumbGroup + "' for item.Label = '" + (item.Label ?? "") + "', conf.StrTitleSelect = '" + conf.StrTitleSelect + "'");
      return strThumbGroup;
    }

    bool GetMoviesInGroup(ref ArrayList fanartItems)
    {
      LogMyFilms.Debug("GetRandomFanartForGroups started ...");

      fanartItems.Clear();
      Type columnType = GetColumnType(conf.WStrSort);
      string s = "";
      string strSelect = string.Empty;
      if (conf.Boolselect)
      {
        string sLabel = this.facadeFilms.SelectedListItem.Label;
        bool labelNotEmpty = sLabel.Length > 0;

        if (columnType != typeof(string))
          strSelect = (labelNotEmpty) ? conf.WStrSort + " = '" + sLabel + "'" : conf.WStrSort + " is NULL";
        else if (conf.WStrSort == "Date")
          strSelect = (labelNotEmpty) ? "DateAdded" + " = '" + DateTime.Parse(sLabel).ToShortDateString() + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        else if (IsDateField(conf.WStrSort))
          strSelect = (labelNotEmpty) ? conf.WStrSort + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(sLabel).ToShortDateString()) + "*'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        else if (IsAlphaNumericalField(conf.WStrSort))
          strSelect = (labelNotEmpty) ? conf.WStrSort + " like '" + StringExtensions.EscapeLikeValue(sLabel) + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        else
        {
          if (conf.BoolDontSplitValuesInViews)
            strSelect = (labelNotEmpty) ? conf.WStrSort + " like '" + StringExtensions.EscapeLikeValue(sLabel) + "'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
          else
            strSelect = (labelNotEmpty) ? conf.WStrSort + " like '*" + StringExtensions.EscapeLikeValue(sLabel) + "*'" : "(" + conf.WStrSort + " is NULL OR " + conf.WStrSort + " like '')";
        }
      }

      if (strSelect != "") s = strSelect + " And ";
      if (conf.StrTitleSelect != "")
        s = s + String.Format("{0} like '{1}{2}*'", conf.StrTitle1, StringExtensions.EscapeLikeValue(conf.StrTitleSelect), conf.TitleDelim);
      else
        s = s + conf.StrTitle1 + " not like ''";
      string StrFilmSelect = s;

      string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
      DataRow[] rFanart = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, StrFilmSelect, conf.StrSorta, conf.StrSortSens, false);
      LogMyFilms.Debug("(GetRandomFanartForGroups) - Count: '" + rFanart.Length + "'");
      int iCnt = 0;
      int DelimCnt = 0, DelimCnt2;
      var item = new GUIListItem();
      string sTitle;
      string sFullTitle;
      string sPrevTitle = "";

      if (conf.StrTitleSelect != "") DelimCnt = NewString.PosCount(conf.TitleDelim, conf.StrTitleSelect, false) + 1; //get num .'s in title
      int number = -1;
      var w_tableau = new ArrayList();
      bool isdate = IsDateField(conf.WStrSort);

      foreach (DataRow sr in rFanart)
      {
        number++;
        if (conf.Boolreturn)//in case of selection by view verify if value correspond excatly to the searched string
        {
          w_tableau = Search_String(sr[conf.WStrSort].ToString(), false);
          foreach (string val in w_tableau)
          {
            if (isdate && string.Format("{0:dd/MM/yyyy}", DateTime.Parse(val).ToShortDateString()) == string.Format("{0:dd/MM/yyyy}", DateTime.Parse(conf.Wselectedlabel).ToShortDateString()))
              goto suite;
            if (val.IndexOf(conf.Wselectedlabel, StringComparison.OrdinalIgnoreCase) >= 0) // if (val.ToLower().Contains(conf.Wselectedlabel.Trim().ToLower()))
              goto suite;
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
          item.Label2 = "(" + iCnt + ")  " + NewString.PosRight(")  ", item.Label2);// prepend (items in folder count)
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
            fanartItems.Add(item);
        }
      fin: ;
      }
      return !(fanartItems.Count == 1 && item.IsFolder);
    }

    private bool GetRandomFanartForGroups(int limit)
    {
      var fanartItems = new ArrayList();
      int i = 0;
      try
      {
        GetMoviesInGroup(ref fanartItems);
      }
      catch (Exception ex)
      {
        //LogMyFilms.DebugException("GetRandomFanartForGroups(): exception on calling GetMoviesInGroup() - ", ex);
        LogMyFilms.Warn("GetRandomFanartForGroups(): exception on calling GetMoviesInGroup() - " + ex.Message);
        LogMyFilms.DebugException("GetRandomFanartForGroups() - inner exception: ", ex);
        fanartItems.SafeDispose();
        return false;
      }
      currentFanartList.Clear(); // clear list from former content
      foreach (string[] wfanart in fanartItems.Cast<GUIListItem>().Select(randomFanartItem => MyFilmsDetail.Search_Fanart(randomFanartItem.Label, true, "file", false, string.Empty, string.Empty)).Where(wfanart => wfanart[0] != " " && wfanart[0] != MyFilms.conf.DefaultFanartImage && !currentFanartList.Contains(wfanart[0])))
      {
        currentFanartList.Add(wfanart[0]);
        // LogMyFilms.Debug("GetRandomFanartForGroups() - added fanart #" + currentFanartList.Count + " : '" + wfanart[0] + "'");
        i += 1;
        if (i >= limit && limit != 0)
        {
          return true;
        }
      }
      fanartItems.SafeDispose();
      return currentFanartList.Count > 0;
    }

    private bool GetRandomFanartForFilms(int limit)
    {
      int i = 0;
      string fanartDirectory = string.Empty;
      currentFanartList.Clear(); // clear list from former content
      var wfanart = new string[2];
      int index = this.facadeFilms.SelectedListItem.ItemId;
      if (MyFilms.r == null || index > MyFilms.r.Length - 1)
      {
        LogMyFilms.Warn("GetRandomFanartForFilms() - Failed loading random fanart - index '" + index + "' not within current dataset ... ");
        return false;
      }
      string personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
      string fanartTitle = MyFilmsDetail.GetFanartTitle(r[index], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
      wfanart = MyFilmsDetail.Search_Fanart(fanartTitle, false, "dir", false, "false", string.Empty);

      if (wfanart[1] == "dir")
        fanartDirectory = wfanart[0];

      if (!string.IsNullOrEmpty(fanartDirectory) && Directory.Exists(fanartDirectory))
      {
        const string searchname = "*.jpg";
        string[] files = Directory.GetFiles(fanartDirectory, searchname, SearchOption.TopDirectoryOnly);
        if (files.Length > 0)
          foreach (string file in files.Where(file => !currentFanartList.Contains(file)))
          {
            currentFanartList.Add(file);
            //LogMyFilms.Debug("GetRandomFanartForFilms() - added fanart #" + currentFanartList.Count + " : '" + file + "'");
            i += 1;
            if (i >= limit && limit != 0)
              return true;
          }
      }
      return currentFanartList.Count > 0;
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
      switch (currentFanartList.Count)
      {
        case 0:
          newFanart = " ";
          break;
        case 1:
          newFanart = currentFanartList[0];
          break;
        default:
          if (currentFanartList.Count > 1)
          {
            Int32 randomFanartIndex = -1;
            while (!success && errorcount < 5)
            {
              try
              {
                //Choose Random Fanart from Resultlist
                var rnd = new System.Random();
                randomFanartIndex = rnd.Next(currentFanartList.Count);
                if (currentFanartList.Count <= randomFanartIndex)
                {
                  LogMyFilms.Debug("GetNewRandomFanart() - error, invalid index !  - Available: '" + currentFanartList.Count + "', selected ID: '" + randomFanartIndex + "' - returning empty fanart !");
                  return " ";
                }
                newFanart = currentFanartList[randomFanartIndex];
                if (newFanart != this.backdrop.Filename)
                {
                  success = true;
                  LogMyFilms.Debug("GetNewRandomFanart() - Available: '" + currentFanartList.Count + "', selected ID: '" + randomFanartIndex + "', selected Path: '" + newFanart + "'");
                }
                else
                {
                  errorcount += 1;
                  Thread.Sleep(50);
                }
              }
              catch (Exception ex)
              {
                LogMyFilms.Warn("GetNewRandomFanart() - error, invalid index !  - Available: '" + currentFanartList.Count + "', selected ID: '" + randomFanartIndex + "' - " + ex.Message);
                errorcount += 1;
                success = false;
              }
            }
          }
          break;
      }
      return newFanart;
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

    static readonly Regex groupCountMatches = new Regex(@"\([0-9]*\)", RegexOptions.Compiled);

    //----------------------------------------------------------------------------------------
    //    Display Detailed Info (Image, Description, Year, Category)
    //----------------------------------------------------------------------------------------
    private void Load_Lstdetail(GUIListItem currentItem, bool forceLoading)
    {
      //int waitcounter = 0;
      //while (itemToPublish == null)
      //{
      //  LogMyFilms.Debug("Load_Lstdetail() - Waitcounter =  '" + waitcounter + "'");
      //  Thread.Sleep(5);
      //  waitcounter++;
      //  if (waitcounter > 100) return;
      //}

      #region sanity checks ...
      if (currentItem == null)
      {
        LogMyFilms.Warn("Load_Lstdetail() - Skipping: currentItem is null ! -> return");
        return;
      }
      if (currentItem.ItemId == -1)
      {
        // reinit some fields //filmcover.Filename = ""; //backdrop.Filename = ""; //MyFilmsDetail.Init_Detailed_DB(false);
        LogMyFilms.Warn("Load_Lstdetail() - Skipping: ItemId == -1 -> return");
        return;
      }
      if (r == null || currentItem.ItemId > r.Length - 1)
      {
        LogMyFilms.Warn("Load_Lstdetail() - Failed loading data row  - index not within current dataset !");
      }
      #endregion

      LogMyFilms.Debug("Load_Lstdetail() - Start: ItemId = '" + currentItem.ItemId + "', label = '" + currentItem.Label + "', ViewContext = '" + conf.ViewContext + "', forceLoading = '" + forceLoading + "'");

      if ((currentItem.ItemId == Prev_ItemID && currentItem.Label == Prev_Label) && !forceLoading)
      {
        LogMyFilms.Warn("Load_Lstdetail() - Skipping: ItemId == Prev_ItemID ('" + Prev_ItemID + "') AND  currentItem.Label == Prev_Label ('" + Prev_Label + "') -> return");
        return;
      }
      Prev_ItemID = currentItem.ItemId;
      Prev_Label = currentItem.Label;

      switch (conf.ViewContext)
      {
        case ViewContext.Menu:
        case ViewContext.MenuAll:
          #region menu types
          //conf.MenuSelectedID = currentItem.ItemId; // remember last menu position ...  
          MyFilmsDetail.Init_Detailed_DB(false);
          // GUIControl.ShowControl(GetID, 34);
          Clear_Logos();
          menucover.Filename = currentItem.ThumbnailImage;
          filmcover.Filename = "";
          viewcover.Filename = "";
          personcover.Filename = "";
          groupcover.Filename = "";
          SetDummyControlsForFacade(conf.ViewContext);

          MFview.ViewRow selectedView = GetCustomViewFromViewLabel(currentItem.Label);
          if (selectedView != null) MyFilmsDetail.setGUIProperty("index", selectedView.Index.ToString());
          else MyFilmsDetail.clearGUIProperty("index");

          if (conf.StrFanartDefaultViewsUseRandom)
          {
            string MenuFanart = GetNewRandomFanart(true, false); // resets and populates fanart list and selects a random one
            if (MenuFanart != " " && !string.IsNullOrEmpty(MenuFanart))
            {
              backdrop.Filename = MenuFanart;
              MyFilmsDetail.setGUIProperty("currentfanart", MenuFanart);
              Fanartstatus(true);
            }
            else
            {
              Fanartstatus(false);
              backdrop.Filename = " ";
              MyFilmsDetail.setGUIProperty("currentfanart", " ");
            }
          }
          #endregion
          break;
        //case ViewContext.Movie:
        //case ViewContext.MovieCollection:
        //case ViewContext.Group:
        //case ViewContext.Person:
        //  break;
        case ViewContext.None:
          break;
        default:
          if (currentItem.IsFolder && MyFilms.conf.Boolselect)
          #region Views with grouping ...
          {
            LogMyFilms.Debug("Load_Lstdetail() - Item is Folder and BoolSelect is true");
            MyFilmsDetail.Init_Detailed_DB(false);

            switch (conf.ViewContext)
            {
              case ViewContext.Person:
                personcover.Filename = currentItem.ThumbnailImage;
                //MatchCollection matchList = groupCountMatches.Matches(currentItem.Label2);
                //if (matchList.Count > 0)
                //{
                //  Match matcher = matchList[0];
                //  string personcount = matcher.Value.Trim(new Char[] { '(', ')' }).Trim();
                //  MyFilmsDetail.setGUIProperty("user.mastertitle.groupcount", personcount, true);
                //}
                break;
              case ViewContext.Group:
                MyFilmsDetail.setGUIProperty("user.mastertitle.groupcount", currentItem.Label2, true);
                viewcover.Filename = currentItem.ThumbnailImage;
                break;
              default:
                //conf.FileImage = currentItem.ThumbnailImage;
                //viewcover.Filename = conf.FileImage;
                //personcover.Filename = conf.FileImage;
                //filmcover.Filename = conf.FileImage;
                //groupcover.Filename = conf.FileImage;
                break;
            }
            MyFilmsDetail.setGUIProperty("picture", currentItem.ThumbnailImage, true);

            #region fanart
            new System.Threading.Thread(delegate()
            {
              {
                LogMyFilms.Debug("Load_Lstdetail() - Sleep 500 ms to let animations go ...");
                Thread.Sleep(conf.ViewContext == ViewContext.Person ? 1000 : 500);
                try
                {
                  var wfanart = MyFilmsDetail.Search_Fanart(currentItem.Label, true, "file", true, currentItem.ThumbnailImage, currentItem.Path);
                  if (conf.StrFanartDefaultViewsUseRandom)
                  {
                    var groupFanart = GetNewRandomFanart(true, false); // resets and populates fanart list and selects a random one
                    if (groupFanart != " ") wfanart[0] = groupFanart;
                  }
                  Fanartstatus(wfanart[0] != " ");
                  backdrop.Filename = wfanart[0];
                  MyFilmsDetail.setGUIProperty("currentfanart", wfanart[0]);
                  LogMyFilms.Debug("Load_Lstdetail() - Backdrop status: '" + backdrop.Active + "', backdrop.Filename = wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                }
                catch (Exception ex)
                {
                  LogMyFilms.Warn("Load_Lstdetail() - Fanart-exception: '" + ex.Message + "'");
                }
              }
              GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
              {
                {
                  // after thread is finished ...
                }
                return 0;
              }, 0, 0, null);
            }) { Name = "MyFilmsSetFanartOnPageLoadWorker", IsBackground = true, Priority = ThreadPriority.BelowNormal }.Start();
            #endregion

            // Load_Rating(0); // old method - nor more used
            Clear_Logos(); // reset logos

            GUIControl.ShowControl(GetID, 34);
            SetDummyControlsForFacade(conf.ViewContext);

            switch (conf.ViewContext)
            {
              case ViewContext.Person:
                MyFilmsDetail.clearGUIProperty("user.source.isonline");
                MyFilmsDetail.clearGUIProperty("user.sourcetrailer.isonline");

                // MyFilmsDetail.Init_Detailed_DB(false);
                // MyFilmsDetail.Load_Detailed_DB(currentItem.ItemId, false);
                string personname = (conf.BoolReverseNames && currentItem.Label != EmptyFacadeValue) ? ReReverseName(currentItem.Label) : currentItem.Label.Replace(EmptyFacadeValue, ""); // Replace "pseudolabel" with empty value
                MyFilmsDetail.Load_Detailed_PersonInfo(personname, currentItem);
                break;
              case ViewContext.Group:
                MyFilmsDetail.clearGUIProperty("user.source.isonline");
                MyFilmsDetail.clearGUIProperty("user.sourcetrailer.isonline");
                // MyFilmsDetail.Load_Detailed_DB(currentItem.ItemId, false);
                break;
            }
          }
          #endregion
          else
          #region Movie display ...
          {
            LogMyFilms.Debug("Load_Lstdetail() - Item is Film List Item - contains hierarchy = '" + currentItem.IsFolder + "'");
            if (currentItem.IsFolder) // if it is a group/box-set, clear all properties
            {
              // GUIControl.ShowControl(GetID, 34);
              MyFilmsDetail.Init_Detailed_DB(false);
            }

            #region set groupcount for movies
            if (currentItem.IsFolder) // contains collection for a hierarchy
            {
              Regex p = new Regex(@"\([0-9]*\)"); // count in brackets
              MatchCollection matchList = p.Matches(currentItem.Label2);
              if (matchList.Count > 0)
              {
                Match matcher = matchList[0];
                string groupcount = matcher.Value.Trim(new Char[] { '(', ')' }).Trim();
                MyFilmsDetail.setGUIProperty("user.mastertitle.groupcount", groupcount, true);
              }
            }
            else
              MyFilmsDetail.clearGUIProperty("user.mastertitle.groupcount");
            #endregion

            string currentFilmCover = "";
            if (currentItem.MusicTag != null && !string.IsNullOrEmpty(currentItem.MusicTag.ToString()))
              currentFilmCover = currentItem.MusicTag.ToString();
            else if (currentItem.ThumbnailImage != null && !string.IsNullOrEmpty(currentItem.ThumbnailImage))
              currentFilmCover = currentItem.ThumbnailImage;
            //currentFilmCover = (currentItem.MusicTag != null && !string.IsNullOrEmpty(currentItem.MusicTag.ToString()))
            //                            ? currentItem.MusicTag.ToString()
            //                            : ((!string.IsNullOrEmpty(currentItem.ThumbnailImage)) ? currentItem.ThumbnailImage : "");
            LogMyFilms.Debug("Load_Lstdetail() - currentFilmCover = '" + currentFilmCover + "'");

            if (currentItem.IsFolder) //  && conf.ViewContext == ViewContext.Movie
            {
              string currentGroupCover = GetGroupImage(currentItem); // HierarchyImage for selected item
              if (currentGroupCover.Length > 0) currentFilmCover = currentGroupCover;
              LogMyFilms.Debug("Load_Lstdetail() - currentFilmCover (GroupImage) = '" + currentFilmCover + "'");
            }
            filmcover.Filename = currentFilmCover;
            MyFilmsDetail.setGUIProperty("picture", currentFilmCover, true);
            if (currentItem.IsFolder && conf.ViewContext == ViewContext.Movie)
              groupcover.Filename = currentFilmCover;

            Load_Logos(MyFilms.r, currentItem.ItemId); // set logos

            #region set fanart
            new System.Threading.Thread(delegate()
            {
              {
                LogMyFilms.Debug("Load_Lstdetail() - Sleep 500 ms to let animations go ...");
                Thread.Sleep(500); // wait, so animations don't stutter
                try
                {
                  if (conf.StrFanartDefaultViewsUseRandom) currentFanartList.Clear();
                  string fanartTitle = (currentItem.IsFolder) ? currentItem.Label : MyFilmsDetail.GetSearchTitles(MyFilms.r[currentItem.ItemId], "").FanartTitle;
                  LogMyFilms.Debug("Load_Lstdetail() - FanartTitle from facadeview = '" + fanartTitle + "'");
                  #region disabled code
                  //string fanartTitle, personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
                  //fanartTitle = MyFilmsDetail.GetFanartTitle(r[facadeFilms.SelectedListItem.ItemId], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
                  // fanartTitle = currentItem.Label;
                  //if (MyFilms.conf.StrTitle1 == "FormattedTitle") // added to get fanart displayed when mastertitle is set to formattedtitle
                  //{
                  //  fanartTitle = MyFilms.r[facadeFilms.SelectedListItem.ItemId]["OriginalTitle"].ToString();
                  //  //Added by Guzzi to fix Fanartproblem when Mastertitle is set to OriginalTitle
                  //  if (MyFilms.conf.StrTitle1 != "OriginalTitle")
                  //  {

                  //    if (MyFilms.r[facadeFilms.SelectedListItem.ItemId]["TranslatedTitle"] != null && MyFilms.r[facadeFilms.SelectedListItem.ItemId]["TranslatedTitle"].ToString().Length > 0)
                  //      fanartTitle = MyFilms.r[facadeFilms.SelectedListItem.ItemId]["TranslatedTitle"].ToString();
                  //  }
                  //}
                  #endregion
                  string[] wfanart = MyFilmsDetail.Search_Fanart(fanartTitle, true, "file", false, currentFilmCover, string.Empty);
                  if (conf.StrFanartDefaultViewsUseRandom)
                  {
                    string FilmFanart = GetNewRandomFanart(true, true);
                    // resets and populates fanart list and selects a random one
                    if (FilmFanart != " ") wfanart[0] = FilmFanart;
                  }

                  LogMyFilms.Debug("Load_Lstdetail() - Backdrops-File: wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                  this.Fanartstatus(wfanart[0] != " ");
                  backdrop.Filename = wfanart[0];
                  MyFilmsDetail.setGUIProperty("currentfanart", wfanart[0]);
                  LogMyFilms.Debug("Load_Lstdetail() - Fanart-Status: '" + backdrop.Active + "', Backdrops-File: backdrop.Filename = wfanart[X]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                }
                catch (Exception ex)
                {
                  LogMyFilms.Warn("Load_Lstdetail() - Fanart-exception: '" + ex.Message + "'");
                }
              }
              GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
                {
                  {
                    // after thread is finished ...
                  }
                  return 0;
                }, 0, 0, null);
            }) { Name = "MyFilmsSetFanartOnPageLoadWorker", IsBackground = true }.Start();
            #endregion

            SetDummyControlsForFacade(conf.ViewContext);
            // Load_Rating(conf.W_rating); // old method - no more used

            if (!currentItem.IsFolder)
            {
              // GUIControl.HideControl(GetID, 34);
              MyFilmsDetail.Load_Detailed_DB(currentItem.ItemId); // load details, if it is not a hierarchyentry (folder)
            }
          }
          #endregion
          break;
      }
      // itemToPublish = null;
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
      if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll) return; // no sorting for menu types !
      LogMyFilms.Debug("SortChanged() - handler called with order = '" + e.Order + "'");
      LogMyFilms.Debug(string.Format("SortChanged() - current sort orders - WStrSortSensCount        : '{0}'", conf.WStrSortSensCount));
      LogMyFilms.Debug(string.Format("SortChanged() - current sort orders - WStrSortSens             : '{0}'", conf.WStrSortSens));
      LogMyFilms.Debug(string.Format("SortChanged() - current sort orders - StrSortSensInHierarchies : '{0}'", conf.StrSortSensInHierarchies));
      LogMyFilms.Debug(string.Format("SortChanged() - current sort orders - StrSortSens              : '{0}'", conf.StrSortSens));
      if (conf.Boolselect) //in Views / Groupviews
      {
        if (conf.BoolSortCountinViews)
        {
          if (e.Order.ToString().Substring(0, 3).ToLower() == conf.WStrSortSensCount.Trim().Substring(0, 3).ToLower())
            return;
          conf.WStrSortSensCount = (BtnSrtBy.IsAscending) ? " ASC" : " DESC";
        }
        else // normal sort direction for views/groups
        {
          if (e.Order.ToString().Substring(0, 3).ToLower() == conf.WStrSortSens.Trim().Substring(0, 3).ToLower())
            return;
          conf.WStrSortSens = (BtnSrtBy.IsAscending) ? " ASC" : " DESC";
        }
      }
      else
      {
        if (conf.BoolCollection) // film groups/collections
        {
          if (e.Order.ToString().Substring(0, 3).ToLower() == conf.StrSortSensInHierarchies.Trim().Substring(0, 3).ToLower())
            return;
          conf.StrSortSensInHierarchies = (BtnSrtBy.IsAscending) ? " ASC" : " DESC";
        }
        else // normal sortdirection in film groups/collections
        {
          if (e.Order.ToString().Substring(0, 3).ToLower() == conf.StrSortSens.Trim().Substring(0, 3).ToLower())
            return;
          conf.StrSortSens = (BtnSrtBy.IsAscending) ? " ASC" : " DESC";
        }
      }

      if (conf.Boolselect)
        getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, true, "");
      else
        GetFilmList();
    }

    private void item_OnItemSelected(GUIListItem item, GUIControl parent)
    {
      // LogMyFilms.Debug("Call item_OnItemSelected()with options - item: '" + item.ItemId + "', SelectedListItemIndex: '" + facadeFilms.SelectedListItemIndex.ToString() + "', Label: '" + facadeFilms.SelectedListItem.Label + "', TVtag: '" + item.TVTag.ToString() + "'");

      GUIFilmstripControl filmstrip = parent as GUIFilmstripControl;
      if (filmstrip != null) filmstrip.InfoImageFileName = item.ThumbnailImage;

      //if (facadeFilms.SelectedListItem.ItemId == Prev_ItemID && facadeFilms.SelectedListItem.Label == Prev_Label)
      //{
      //  LogMyFilms.Debug("(item_OnItemSelected): ItemId == Prev_ItemID (" + Prev_ItemID + ") && label == Prev_Label (" + Prev_Label + ") -> return without action !");
      //  return;
      //}

      MovieDetailsPublisher(item);

      //if (!(conf.Boolselect || (facadeFilms.SelectedListItemIndex > -1 && facadeFilms.SelectedListItem.IsFolder))) //xxxx
      //{
      //  if (facadeFilms.SelectedListItemIndex > -1)
      //    MovieDetailsPublisher(item, true); //Load_Lstdetail(facadeFilms.SelectedListItem.ItemId, true, facadeFilms.SelectedListItem.Label);
      //}
      //else
      //{
      //  if (facadeFilms.SelectedListItemIndex > -1 && !conf.Boolselect)
      //  {
      //    MovieDetailsPublisher(item, false); //Load_Lstdetail(facadeFilms.SelectedListItem.ItemId, false, facadeFilms.SelectedListItem.Label);
      //  }
      //  else
      //  {
      //    MovieDetailsPublisher(item, false); //Load_Lstdetail(facadeFilms.SelectedListItem.ItemId, false, facadeFilms.SelectedListItem.Label);
      //    //GUIControl.ShowControl(GetID, 34);
      //  }
      //}
      // Load_Lstdetail(item.ItemId, true, item.Label);
    }

    delegate void MovieDetailsPublisherWorker(GUIListItem item, bool wrep);

    private void MovieDetailsPublisher(GUIListItem item)
    {
      LogMyFilms.Debug("Call MovieDetailsPublisher() with options - ItemId    : '" + item.ItemId + "', label: '" + item.Label + "'");
      var tickCount = System.Windows.Media.Animation.AnimationTimer.TickCount;
      // Update instance of delayed item with current position
      itemToPublish = item;
      if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.MovieID)) // if loaded movie via loadparams, never use delayed loading ...
      {
        LogMyFilms.Debug("MovieDetailsPublisher() - direct call by load paramaters");
        Load_Lstdetail(itemToPublish, false);
      }
      // Publish instantly when previous request has passed the required delay
      else if ((150 < (int)(tickCount - lastPublished)) || loadParamInfo != null) // wait 150 ms to load details... if loaded via loadparams, never use delayed loading ...
      {
        lastPublished = tickCount;
        var publisher = new MovieDetailsPublisherWorker(Load_Lstdetail);
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
    private void Change_View_Menu()
    {
      var choiceView = new List<string>();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079903)); // Change View ...
      #region old hardcoded entries - disabled
      //dlg.Add(GUILocalizeStrings.Get(342));//videos
      //choiceView.Add("All");
      //dlg.Add(GUILocalizeStrings.Get(345));//year
      //choiceView.Add("Year");
      //dlg.Add(GUILocalizeStrings.Get(10798664));//genre -> category
      //choiceView.Add("Category");
      //dlg.Add(GUILocalizeStrings.Get(200026));//countries
      //choiceView.Add("Country");
      //dlg.Add(GUILocalizeStrings.Get(10798954));//recentlyadded
      //choiceView.Add("RecentlyAdded");
      //dlg.Add(GUILocalizeStrings.Get(10798667));//actors 
      //choiceView.Add("Actors");
      //dlg1.Add(GUILocalizeStrings.Get(200027));//Watched

      // Commented, as we have replaced this feature with global overlay filter for "media available"
      //if (Helper.FieldIsSet(conf.StrStorage.Length))
      //{
      //    dlg1.Add(GUILocalizeStrings.Get(154) + " " + GUILocalizeStrings.Get(1951));//storage
      //    SelectedView.Add("storage");
      //}
      #endregion

      if (MyFilms.conf.CustomViews.View.Rows.Count > 0)
      {
        foreach (MFview.ViewRow customView in Enumerable.Where(MyFilms.conf.CustomViews.View, customView => Helper.FieldIsSet(customView.DBfield) && customView.ViewEnabled))
        {
          choiceView.Add(customView.Label); //choiceView.Add(string.Format("View{0}", i));
          dlg.Add(string.IsNullOrEmpty(customView.Label) ? customView.DBfield : customView.Label);
        }

        dlg.Add(GUILocalizeStrings.Get(10798765)); // *** show all ***
        choiceView.Add("showall");

        // add menu as option
        dlg.Add(GUILocalizeStrings.Get(924)); // menu
        choiceView.Add("Menu");

        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;
      }

      // show all sort options, if selected or no custom views available ...
      if (MyFilms.conf.CustomViews.View.Rows.Count == 0 || choiceView[dlg.SelectedLabel] == "showall")
      {
        dlg.Reset();
        dlg.SetHeading(GUILocalizeStrings.Get(1079903)); // Change View (films) ...
        choiceView.Clear();
        var displayItems = GetDisplayItems("view");
        foreach (string[] displayItem in displayItems)
        {
          string entry = (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1];
          dlg.Add(entry);
          choiceView.Add(displayItem[0]);
          LogMyFilms.Debug("View Menu - add '{0}' as '{1}'", displayItem[0], entry);
        }
        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;
      }
      conf.StrSelect = ""; // reset movie context filter for person views
      conf.StrPersons = ""; // reset person list filter
      viewcover.Filename = "";
      personcover.Filename = "";
      groupcover.Filename = "";

      Change_View_Action(choiceView[dlg.SelectedLabel]);
    }

    private int CountViewItems(DataRow[] filmrows, string WStrSort)
    {
      int wi;
      int count = 0;
      string champselect;
      HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //List<string> itemList = new List<string>();

      bool issplitfield = IsSplittableField(WStrSort);
      bool dontsplitvalues = MyFilms.conf.BoolDontSplitValuesInViews;
      bool showEmptyValues = MyFilms.conf.BoolShowEmptyValuesInViews;

      watch.Reset(); watch.Start();
      foreach (DataRow row in filmrows)
      {
        champselect = row[WStrSort].ToString().Trim();
        if (issplitfield && !dontsplitvalues)
        {
          ArrayList wtab = Search_String(champselect, false);
          if (wtab.Count > 0) // itemList.AddRange(wtab);
          {
            for (wi = 0; wi < wtab.Count; wi++)
            {
              if (set.Add((string)wtab[wi])) count++; // itemList.Add((string)wtab[wi]);
            }
          }
          else if (showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            if (set.Add(champselect)) count++;  // itemList.Add(champselect);
          }
        }
        else
        {
          if (champselect.Length > 0 || showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            if (set.Add(champselect)) count++;  // itemList.Add(champselect);
          }
        }
      }
      watch.Stop();
      LogMyFilms.Debug("CountViewItems - Finished Count ('" + count + "') (" + (watch.ElapsedMilliseconds) + " ms)  - Read View Names for '" + WStrSort + "' finished (splittable items = '" + issplitfield + "', dontsplitvalues = '" + dontsplitvalues + "')");
      return count;

      // int count = itemList.Distinct().Count();
      // int count = itemList.Distinct(MfStringComparer).Count();

      //HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      //foreach (string s in itemList) if (set.Add(s)) count++;

      //Dictionary<object, int> counts = new Dictionary<object, int>();
      //foreach (object item in ItemList)
      //{
      //  if (!counts.ContainsKey(item)) counts.Add(item, 1);
      //  // else counts[item]++; // we don't need the number of the items itself ...
      //}
      //int count = counts.Count;
    }

    private Dictionary<T, int> CountOccurences<T>(IEnumerable<T> items)
    {
      var occurences = new Dictionary<T, int>();
      foreach (T item in items)
      {
        if (occurences.ContainsKey(item))
        {
          occurences[item]++;
        }
        else
        {
          occurences.Add(item, 1);
        }
      }
      return occurences;
    }

    //--------------------------------------------------------------------------------------------
    //  Load Online Info Menu into facade list ...
    //--------------------------------------------------------------------------------------------
    private void GetSelectFromOnlineMenuView()
    {
      LogMyFilms.Debug("GetSelectFromOnlineMenuView() - launched ...");

      Change_Layout_Action(0); // always use list view // Change_Layout_Action(MyFilms.conf.WStrLayOut);  // we share the layout with Views ...

      BtnSrtBy.Label = GUILocalizeStrings.Get(98) + GUILocalizeStrings.Get(103); // sort: name
      //BtnSrtBy.IsAscending = true;
      BtnSrtBy.IsEnabled = false;

      // SetDummyControlsForFacade(ViewContext.Menu); // reset all covers ...
      GUIControl.ShowControl(GetID, 34); // hide film controls ...
      SetLabelView("menu");
      SetLabelSelect("");

      conf.ViewContext = ViewContext.Menu;

      if (conf.StrFanart && conf.StrFanartDfltImage)
      {
        if (backdrop.Filename == "") backdrop.Filename = conf.DefaultFanartImage;
        Fanartstatus(true);
      }

      ClearFacade(); // facadeFilms.Clear();
      GUIListItem item = null;

      // Latest Movies -  retrieve the newest movie that was added to TMDbitem = new GUIListItem();
      item = new GUIListItem();
      item.Label = "Latest Movies";
      item.DVDLabel = "TMDBaction";
      item.IsFolder = true;
      item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
      if (facadeFilms != null) facadeFilms.Add(item);

      // Popular Movies - retrieve the daily movie popularity list
      item = new GUIListItem();
      item.Label = "Popular Movies";
      item.DVDLabel = "TMDBaction";
      item.IsFolder = true;
      item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
      if (facadeFilms != null) facadeFilms.Add(item);

      //Now Playing Movies -  retrieve the movies currently in theatres
      item = new GUIListItem();
      item.Label = "Now Playing Movies";
      item.DVDLabel = "TMDBaction";
      item.IsFolder = true;
      item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
      if (facadeFilms != null) facadeFilms.Add(item);

      //Top Rated Movies -  retrieve the top rated movies that have over 10 votes on TMDb
      item = new GUIListItem();
      item.Label = "Top rated movies";
      item.DVDLabel = "TMDBaction";
      item.IsFolder = true;
      item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
      if (facadeFilms != null) facadeFilms.Add(item);

      //Upcoming Movies - retrieve the movies arriving to theatres within the next few weeks
      item = new GUIListItem();
      item.Label = "Upcoming Movies";
      item.DVDLabel = "TMDBaction";
      item.IsFolder = true;
      item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
      if (facadeFilms != null) facadeFilms.Add(item);

      if (facadeFilms == null) return;

      MyFilmsDetail.setGUIProperty("nbobjects.value", facadeFilms.Count.ToString());
      GUIPropertyManager.SetProperty("#itemcount", facadeFilms.Count.ToString());
      // GUIPropertyManager.SetProperty("#itemtype", "Views"); // disabled, as we otherwise have to set it in all facade listings ...
      // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + "/" + GUIPropertyManager.GetProperty("#myfilms.select")); // will be set in SetLabel
      LogMyFilms.Debug("GetSelectFromOnlineMenuView() - end facade load ...");
    }


    //--------------------------------------------------------------------------------------------
    //  Load Views for Video from facade list ...
    //--------------------------------------------------------------------------------------------
    private void GetSelectFromMenuView(bool showall)
    {
      LogMyFilms.Debug("GetSelectFromMenuView() - launched with showall = '" + showall + "'");
      conf.BoolMenuShowAll = showall; // remember state
      if (MyFilms.conf.CustomViews.View.Rows.Count == 0) showall = true; // show generic views, if there is no Custom Views defined in config - but keep it as "base menu"

      Prev_ItemID = -1;
      Prev_Label = string.Empty;

      Change_Layout_Action(0); // always use list view // Change_Layout_Action(MyFilms.conf.WStrLayOut);  // we share the layout with Views ...

      BtnSrtBy.Label = GUILocalizeStrings.Get(96) + GUILocalizeStrings.Get(103); // sort: name
      //BtnSrtBy.IsAscending = true;
      BtnSrtBy.IsEnabled = false;

      // SetDummyControlsForFacade(ViewContext.Menu); // reset all covers ...
      GUIControl.ShowControl(GetID, 34); // hide film controls ...
      SetLabelView("menu");
      SetLabelSelect("");

      conf.ViewContext = (showall) ? ViewContext.MenuAll : ViewContext.Menu;
      // conf.StrSelect = ""; // reset movie filter for views
      // conf.StrFilmSelect = "";

      if (conf.StrFanart && conf.StrFanartDfltImage)
      {
        if (backdrop.Filename == "") backdrop.Filename = conf.DefaultFanartImage;
        Fanartstatus(true);
      }

      LogMyFilms.Debug("GetSelectFromMenuView() - start facade load ...");
      ClearFacade(); // facadeFilms.Clear();

      GUIListItem item = null;
      if (!showall)
      {
        #region old hardcoded entries
        //item = new GUIListItem(); 
        //item.Label = GUILocalizeStrings.Get(342);//videos
        //item.DVDLabel = "All";
        //item.Label2 = r.Select(p => p[conf.StrTitle1] != DBNull.Value).Count().ToString();  // Select(row => row.Field<int?>("F1")).Where(val => val.HasValue).Select(val => val.Value).Distinct()
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item); 

        //item = new GUIListItem();
        //item.Label = GUILocalizeStrings.Get(345);//year
        //item.DVDLabel = "Year";
        //// item.Label2 = r.Select(p => p["Year"] != DBNull.Value).Distinct().Count().ToString(); 
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item);

        //item = new GUIListItem();
        //item.Label = GUILocalizeStrings.Get(10798664);//genre -> category
        //item.DVDLabel = "Category";
        //// item.Label2 = r.Select(p => p["Category"] != DBNull.Value).Distinct(StringComparer.CurrentCultureIgnoreCase).Count().ToString();
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item);

        //item = new GUIListItem();
        //item.Label = GUILocalizeStrings.Get(200026);//countries
        //item.DVDLabel = "Country";
        //// item.Label2 = r.Select(p => (string)p["Country"]).Distinct(StringComparer.CurrentCultureIgnoreCase).Count().ToString();
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item);

        //item = new GUIListItem();
        //item.Label = GUILocalizeStrings.Get(10798954);//genre -> recentlyadded
        //item.DVDLabel = "RecentlyAdded";
        ////item.Label2 = r.Select(p => p["RecentlyAdded"]).Distinct().Count().ToString();
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item);

        //item = new GUIListItem();
        //item.Label = GUILocalizeStrings.Get(10798667);//actors
        //item.DVDLabel = "Actors";
        //// item.Label2 = r.Select(p => (string)p["Actors"]).Distinct(StringComparer.CurrentCultureIgnoreCase).Count().ToString();
        //item.IsFolder = true;
        //item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        //this.facadeFilms.Add(item);
        #endregion

        foreach (MFview.ViewRow customView in Enumerable.Where(MyFilms.conf.CustomViews.View, customView => Helper.FieldIsSet(customView.DBfield) && customView.ViewEnabled))
        {
          item = new GUIListItem();
          item.DVDLabel = customView.Label; // (string.Format("View{0}", i));
          item.Label = (string.IsNullOrEmpty(customView.Label)) ? customView.DBfield : customView.Label;
          //item.Label2 = customView.Label2 ?? "";
          //item.Label3 = customView.Label3 ?? "";
          item.IsFolder = true;
          if (!string.IsNullOrEmpty(customView.ImagePath))
          {
            item.ThumbnailImage = customView.ImagePath;
            item.IconImage = customView.ImagePath;
            item.IconImageBig = customView.ImagePath;
          }
          else
          {
            string menuimage = this.GetImageforMenu(item);
            item.ThumbnailImage = menuimage;
            item.IconImage = menuimage;
            item.IconImageBig = menuimage;
          }
          item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(this.item_OnItemSelected);
          if (this.facadeFilms != null) this.facadeFilms.Add(item);
        }
        #region add showall entry
        item = new GUIListItem();
        item.Label = GUILocalizeStrings.Get(10798765); // *** show all ***
        item.DVDLabel = "showall";
        item.IsFolder = true;

        item.ThumbnailImage = GetImageforMenu(item);
        item.IconImage = item.ThumbnailImage;
        item.IconImageBig = item.ThumbnailImage;
        item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        if (facadeFilms != null) facadeFilms.Add(item);
        #endregion

        #region add online info entry
        if (MyFilmsDetail.ExtendedStartmode("Context Menu: Edit Value and Filter via GUI")) // check if specialmode is configured for disabled features
        {
          item = new GUIListItem();
          item.Label = "*** Online Informationen ***"; // online info
          item.DVDLabel = "onlineinfo";
          item.IsFolder = true;
          item.ThumbnailImage = GetImageforMenu(item);
          item.IconImage = item.ThumbnailImage;
          item.IconImageBig = item.ThumbnailImage;
          item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
          if (facadeFilms != null) facadeFilms.Add(item);
        }
        #endregion
      }
      else
      {
        #region show all view options, if selected ...
        ArrayList DisplayItems = GetDisplayItems("view");
        foreach (string[] displayItem in DisplayItems)
        {
          item = new GUIListItem();
          item.Label = (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1];
          item.DVDLabel = displayItem[0];
          // item.Label2 = (!conf.OnlyTitleList) ? r.Select(p => (string)p[item.DVDLabel]).Distinct(StringComparer.CurrentCultureIgnoreCase).Count().ToString() : "";
          // item.Label2 = CountViewItems(item.DVDLabel).ToString(); 
          item.IsFolder = true;
          string menuimage = GetImageforMenu(item);
          item.ThumbnailImage = menuimage;
          item.IconImage = menuimage;
          item.IconImageBig = menuimage;
          item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
          if (facadeFilms != null) this.facadeFilms.Add(item);
        }
        #endregion
      }

      if (facadeFilms == null) return;
      if (conf.MenuSelectedID == -2)
        conf.MenuSelectedID = facadeFilms.Count - 1; // if -2 means coming from details menu -> set to "show all"/last position
      else if ((conf.MenuSelectedID > facadeFilms.Count - 1) || (conf.MenuSelectedID < 0)) //check index within bounds
        conf.MenuSelectedID = 0;
      GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, (int)conf.MenuSelectedID);

      MyFilmsDetail.setGUIProperty("nbobjects.value", (!showall ? facadeFilms.Count : facadeFilms.Count - 1).ToString());
      GUIPropertyManager.SetProperty("#itemcount", (!showall ? facadeFilms.Count : facadeFilms.Count - 1).ToString());
      // GUIPropertyManager.SetProperty("#itemtype", "Views"); // disabled, as we otherwise have to set it in all facade listings ...
      // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + "/" + GUIPropertyManager.GetProperty("#myfilms.select")); // will be set in SetLabel
      LogMyFilms.Debug("GetSelectFromMenuView() - end facade load ...");

      GetCountsForMenuView();
    }

    private void GetCountsForMenuView()
    {
      // load dataset and counts threaded ...
      LogMyFilms.Debug("GetCountsForMenuView() - start thread for menu counts ...");
      StopLoadingMenuDetails = false;

      new Thread(delegate()
      {
        #region Load Counts threaded
        // set online filter only, if scan is done already ...
        MyFilms.conf.StrGlobalFilterString = (InitialIsOnlineScan) ? GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating : GlobalFilterStringUnwatched + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;

        // DataRow[] allrows = BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens); // load dataset without filters
        r = BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + conf.StrDfltSelect, conf.StrSelect, conf.StrSorta, conf.StrSortSens);
        // r = BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, conf.StrFilmSelect, sortfield, sortascending, false, true); 
        // DataRow[] r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // load dataset with filters
        for (int i = 0; i < this.facadeFilms.Count; i++)
        {
          if (StopLoadingMenuDetails) break;
          try
          {
            #region count items
            bool success = false;
            GUIListItem countitem = this.facadeFilms[i];
            if (countitem.Label == GUILocalizeStrings.Get(10798765) || countitem.Label == "*** Online Informationen ***") continue; // skip the menu only entry "showall" and "onlineinfo"
            if (!conf.OnlyTitleList && string.IsNullOrEmpty(countitem.Label2)) // first try to get info for custom views ...
            {
              #region get counts for menu item
              string newLabel = countitem.Label2;
              foreach (MFview.ViewRow mfView in Enumerable.Where(MyFilms.conf.CustomViews.View, mfView => Helper.FieldIsSet(mfView.DBfield)).Where(mfView => countitem.DVDLabel == mfView.Label))
              {
                success = true;
                if (string.IsNullOrEmpty(mfView.Value)) // no "Value" filter
                {
                  if (string.IsNullOrEmpty(mfView.Filter))
                    newLabel = this.CountViewItems(r, mfView.DBfield).ToString(); // newLabel = r.Select(p => (string)p[conf.StrViewItem[ii]]).Distinct(MfStringComparer).Count().ToString(); // StringComparer.CurrentCultureIgnoreCase
                  else
                    newLabel = "* " + this.CountViewItems(BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + mfView.Filter + " AND " + conf.StrDfltSelect, "", conf.StrSorta, conf.StrSortSens), mfView.DBfield).ToString();
                }
                else if (mfView.Value == "*") // filmlist show all (possible "Value" filter) -> Count films, as it jumps directly to films
                {
                  if (string.IsNullOrEmpty(mfView.Filter))
                    newLabel = r.Select(p => p[conf.StrTitle1] != DBNull.Value).Count().ToString();  // Select(row => row.Field<int?>("F1")).Where(val => val.HasValue).Select(val => val.Value).Distinct() // newLabel = r.Length.ToString(); 
                  else
                    newLabel = "* " + BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + mfView.Filter + " AND " + conf.StrDfltSelect, "", conf.StrSorta, conf.StrSortSens).Select(p => p[conf.StrTitle1] != DBNull.Value).Count().ToString();
                }
                else // "Value" filter present - use it !
                {
                  string ValueFilter = "";
                  if (GetColumnType(mfView.DBfield) != typeof(string))
                    ValueFilter = mfView.DBfield + " = '" + mfView.Value + "'";
                  else if (IsDateField(mfView.DBfield))
                    ValueFilter = mfView.DBfield + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(mfView.Value).ToShortDateString()) + "*'";
                  else if (IsAlphaNumericalField(mfView.DBfield))
                    ValueFilter = mfView.DBfield + " like '" + mfView.Value + "'";
                  else
                    ValueFilter = mfView.DBfield + " like '*" + mfView.Value + "*'";

                  if (string.IsNullOrEmpty(mfView.Filter))
                    newLabel = "(" + mfView.Value + ") " + BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + conf.StrDfltSelect, ValueFilter, conf.StrSorta, conf.StrSortSens).Select(p => p[conf.StrTitle1] != DBNull.Value).Count().ToString();
                  else
                    newLabel = "* " + "(" + mfView.Value + ") " + BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + mfView.Filter + " AND " + conf.StrDfltSelect, ValueFilter, conf.StrSorta, conf.StrSortSens).Select(p => p[conf.StrTitle1] != DBNull.Value).Count().ToString();

                  //newLabel = "(" + viewRow.Value + ")";
                  //newLabel = "(" + viewRow.Value + ") " + r.Select(p => p[viewRow.DBfield].Equals(viewRow.Value)).Count().ToString();
                  //var ttt = r.Select(p => p[viewRow.DBfield].Equals(viewRow.Value)).Count().ToString();
                  //where d.Element("ProductName").Value.IndexOf(textBox1.Text, StringComparison.InvariantCultureIgnoreCase) > 0
                  // movies = data.Movie.Select(StrDfltSelect + StrSelect, StrSort + " " + StrSortSens)
                }
                countitem.Label2 = newLabel;
                // mfView.Label2 = newLabel; // update view cache
                if (this.StopLoadingMenuDetails) break;
              }
              if (!success) // get standard count, if no custom views match ...
              {
                newLabel = CountViewItems(r, countitem.DVDLabel).ToString();
                countitem.Label2 = newLabel;
                if (StopLoadingMenuDetails) break;
              }
              #endregion
            }
            #endregion
          }
          catch (Exception ex)
          {
            LogMyFilms.DebugException("MyFilmsMenuCountWorker() - error setting counts to facadelist item '" + i + "': " + ex.Message, ex);
          }
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          // Do this after thread finished ...
          // GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, (int)conf.MenuSelectedID);
          return 0;
        }, 0, 0, null);
        #endregion
      }) { Name = "MyFilmsMenuCountWorker", IsBackground = true, Priority = ThreadPriority.BelowNormal }.Start();
    }

    private string GetFieldFromViewLabel(string viewlabel)
    {
      foreach (MFview.ViewRow viewRow in Enumerable.Where(MyFilms.conf.CustomViews.View, viewRow => viewRow.Label == viewlabel))
      {
        return viewRow.DBfield;
      }
      return viewlabel;
    }

    private string GetImageforMenu(GUIListItem item)
    {
      string menuimage = "";
      if (item == null) return "";
      try
      {
        if (string.IsNullOrEmpty(item.ThumbnailImage))
        {
          // string strThumbDirectory = MyFilmsSettings.GetPath(MyFilmsSettings.Path.thumbsGroups) + WStrSort.ToLower() + @"\";
          if (conf.StrPathViews.Length > 0)
          {
            // Check, if default group cover is present
            if (MyFilms.conf.StrViewsDflt)
            {
              string strMenuImage;
              // special handling for funxtion entries
              switch (item.DVDLabel)
              {
                case "showall":
                  strMenuImage = Path.Combine(MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages), "ShowAll.jpg");
                  break;
                case "onlineinfo":
                  strMenuImage = Path.Combine(MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages), "OnlineInfo.jpg");
                  break;
                default:
                  strMenuImage = (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\") ? conf.StrPathViews : conf.StrPathViews + "\\";
                  strMenuImage = strMenuImage + this.GetFieldFromViewLabel(item.DVDLabel).ToLower() + "\\" + "Default.jpg";
                  break;
              }
              if (!File.Exists(strMenuImage))
              {
                if (IsPersonField(GetFieldFromViewLabel(item.DVDLabel)))
                {
                  strMenuImage = MyFilms.conf.DefaultCoverArtist.Length > 0 ? MyFilms.conf.DefaultCoverArtist : MyFilms.conf.DefaultCover;
                }
                else
                {
                  strMenuImage = MyFilms.conf.DefaultCoverViews.Length > 0 ? MyFilms.conf.DefaultCoverViews : MyFilms.conf.DefaultCover; //MyFilmsSettings.GetPath(MyFilmsSettings.Path.DefaultImages) + "DefaultArtist.jpg";
                }
              }
              if (File.Exists(strMenuImage))
              {
                menuimage = strMenuImage;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("GetImageforMenu() - Error: " + ex.Message);
      }
      return menuimage;
    }

    private MFview.ViewRow GetCustomViewFromViewLabel(string viewlabel)
    {
      foreach (MFview.ViewRow viewRow in Enumerable.Where(MyFilms.conf.CustomViews.View, viewRow => viewRow.Label == viewlabel))
      {
        return viewRow;
      }
      LogMyFilms.Debug("GetCustomViewFromViewLabel() - no customvie found for viewlabel '" + viewlabel + "' - returning 'null'");
      return null;
    }

    //--------------------------------------------------------------------------------------------
    //  Change Sort Option for films, vies or collections/groups
    //--------------------------------------------------------------------------------------------
    private void Change_Sort_Option_Menu()
    {
      if (conf.Boolselect) // view sort (grouping) // No change of normal filmlist sort method and no searchs in grouped views (views, e.g. country, year, etc.) - change count sorting instead ...
      {
        #region sorting by counts
        //       BtnSrtBy.Label = (conf.BoolSortCountinViews) ? (GUILocalizeStrings.Get(96) + GUILocalizeStrings.Get(1079910)) : (GUILocalizeStrings.Get(96) + GUILocalizeStrings.Get(103)); // sort: count / sort: name

        var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        if (dlg == null) return;
        dlg.Reset();
        dlg.SetHeading(GUILocalizeStrings.Get(98)); // "Sort By ..."
        var choiceSort = new List<string>();
        dlg.Add(GUILocalizeStrings.Get(103)); // Name
        choiceSort.Add("Name");
        dlg.Add(GUILocalizeStrings.Get(1079910)); // Count
        choiceSort.Add("Count");
        dlg.SelectedLabel = (conf.BoolSortCountinViews) ? 1 : 0; // set position to active sort type
        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;

        // new sort was selected ...
        conf.StrIndex = 0; // reset facadeposition to first line, as after sort position isn't valid anymore ...

        bool boolSortCountInViews = conf.BoolSortCountinViews;
        switch (choiceSort[dlg.SelectedLabel])
        {
          case "Name":
            boolSortCountInViews = false;
            break;
          case "Count":
            boolSortCountInViews = true;
            break;
          default:
            boolSortCountInViews = false;
            break;
        }
        dlg.DeInit();
        if (conf.BoolSortCountinViews == boolSortCountInViews) return;

        conf.BoolSortCountinViews = !conf.BoolSortCountinViews;
        new Thread(delegate()
        {
          {
            MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation); // GUIWaitCursor.Init(); GUIWaitCursor.Show();
            getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, true, string.Empty); // getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, "*", true, string.Empty);
            MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
          }
          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // removed, as it was causing trouble with menu focus control ...
            }
            return 0;
          }, 0, 0, null);
        })
        {
          Name = "Change_Sort_Option_Menu",
          IsBackground = true,
          Priority = ThreadPriority.Normal // Priority = ThreadPriority.AboveNormal
        }.Start();
        // getSelectFromDivxThreaded(); // getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, "*", true, "");
        #endregion
      }
      else
      {
        #region film list sorting (normal and collections)
        var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        if (dlg == null) return;
        dlg.Reset();
        dlg.SetHeading(conf.BoolCollection ? GUILocalizeStrings.Get(1079905) : GUILocalizeStrings.Get(1079902));
        var choiceSort = new List<string>();
        dlg.Add(GUILocalizeStrings.Get(103));// Name (=Title)
        choiceSort.Add("Title");
        dlg.Add(GUILocalizeStrings.Get(366));//Year
        choiceSort.Add("Year");
        dlg.Add(GUILocalizeStrings.Get(104));//Date
        choiceSort.Add("Date");
        dlg.Add(GUILocalizeStrings.Get(367));//Rating
        choiceSort.Add("Rating");
        dlg.Add(GUILocalizeStrings.Get(10798765)); // *** show all ***
        choiceSort.Add("showall");

        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;

        if (choiceSort[dlg.SelectedLabel] == "showall") // show all sort options, if selected ...
        {
          dlg.Reset();
          dlg.SetHeading(conf.BoolCollection ? GUILocalizeStrings.Get(1079905) : GUILocalizeStrings.Get(1079902)); // Sort by (Colletion) ... // Sort by ... 
          choiceSort.Clear();
          ArrayList displayItems = GetDisplayItems("sort");
          foreach (string[] displayItem in displayItems)
          {
            string entry = (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1];
            dlg.Add(entry);
            choiceSort.Add(displayItem[0]);
            LogMyFilms.Debug("Sort Menu - add '{0}' as '{1}'", displayItem[0], entry);
          }
          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1) return;
        }

        // new sort was selected ...
        conf.StrIndex = 0; // reset facadeposition to first line, as after sort position isn't valid anymore ...
        string tmpStrSorta;
        string tmpStrSortSens;

        // set special handling for certain fields ...
        switch (choiceSort[dlg.SelectedLabel])
        {
          case "Title":
            //conf.StrSorta = conf.StrSTitle;
            //conf.StrSortSens = " ASC";
            tmpStrSorta = conf.StrSTitle;
            tmpStrSortSens = " ASC";
            break;
          case "Year":
            tmpStrSorta = "Year";
            tmpStrSortSens = " DESC";
            break;
          case "Date":
            tmpStrSorta = "Date"; // tmpStrSorta = "DateAdded";
            tmpStrSortSens = " DESC";
            break;
          case "Rating":
            tmpStrSorta = "Rating";
            tmpStrSortSens = " DESC";
            break;
          default:
            tmpStrSorta = choiceSort[dlg.SelectedLabel];
            tmpStrSortSens = " ASC";
            break;
        }
        dlg.DeInit();
        if (conf.BoolCollection)
        {
          conf.StrSortaInHierarchies = tmpStrSorta;
          conf.StrSortSensInHierarchies = tmpStrSortSens;
        }
        else
        {
          conf.StrSorta = tmpStrSorta;
          conf.StrSortSens = tmpStrSortSens;
        }
        if (conf.Boolselect) // cannot happen, as sorting for views only changes "counts" above ...
          getSelectFromDivx(conf.StrTitle1 + " not like ''", conf.StrSorta, conf.StrSortSens, "*", true, ""); // getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, true, "");
        else
          GetFilmList();
        GUIWindowManager.Process();
        GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        #endregion
      }
    }

    //--------------------------------------------------------------------------------------------
    //  Select main Options
    //--------------------------------------------------------------------------------------------
    private void Change_Option()
    {
      var dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg1 == null) return;
      var choiceView = new List<string>();
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
      //if (MyFilmsDetail.ExtendedStartmode("Contextmenu for Wiki Onlinehelp")) // check if specialmode is configured for disabled features
      // {
      dlg1.Add(string.Format(GUILocalizeStrings.Get(10798699)));
      choiceView.Add("globalwikihelp");
      //}

      dlg1.Add(string.Format(GUILocalizeStrings.Get(10798700))); // About ...
      choiceView.Add("about");

      dlg1.DoModal(GetID);

      if (dlg1.SelectedLabel == -1)
      {
        return;
      }
      Change_Menu_Action(choiceView[dlg1.SelectedLabel].ToLower());
    }

    //--------------------------------------------------------------------------------------------
    //  Select main global filters
    //--------------------------------------------------------------------------------------------
    private void Change_Global_Filters()
    {
      Context_Menu = true; // make sure, it's set, as otherwise we fall back to first menu level ...
      var dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg1 == null) return;
      dlg1.Reset();
      dlg1.SetHeading(GUILocalizeStrings.Get(10798714)); // Global Filters ...
      var choiceViewGlobalOptions = new List<string>();

      // Change global Unwatchedfilteroption
      // if ((MesFilms.conf.CheckWatched) || (MesFilms.conf.StrSupPlayer))// Make it conditoional, so only displayed, if options enabled in setup !
      if (MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798628)));
      if (!MyFilms.conf.GlobalUnwatchedOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798696), GUILocalizeStrings.Get(10798629)));
      choiceViewGlobalOptions.Add("globalunwatchedfilter");


      // Change global MovieFilter (Only Movies with media files reachable 
      if (InitialIsOnlineScan) // (requires at least initial scan! - //  || MyFilms.conf.ScanMediaOnStart -> or enabled startup option)
      {
        if (GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798628)));
        if (!GlobalFilterIsOnlineOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798770), GUILocalizeStrings.Get(10798629)));
        choiceViewGlobalOptions.Add("filterdbisonline");
      }

      // Change global MovieFilter (Only Movies with Trailer)
      if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
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

      //if (MyFilms.conf.BoolShowEmptyValuesInViews) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079871), GUILocalizeStrings.Get(10798628))); // show empty values in views
      //if (!MyFilms.conf.BoolShowEmptyValuesInViews) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079871), GUILocalizeStrings.Get(10798629)));
      //choiceViewGlobalOptions.Add("showemptyvaluesinviews");

      //// Select or disable a custom filter (e.g. category value, year value, etc.)
      //if (GlobalFilterStringCustomFilter.Length > 0) dlg1.Add(string.Format(GUILocalizeStrings.Get(0), GUILocalizeStrings.Get(0))); // disable user filter 'field-filter'
      //if (GlobalFilterStringCustomFilter.Length == 0) dlg1.Add(GUILocalizeStrings.Get(0)); // select user filter
      //choiceViewGlobalOptions.Add("filterdbcustomfilter");

      dlg1.DoModal(GetID);
      if (dlg1.SelectedLabel == -1) return;
      LogMyFilms.Debug("Call change_global_filters menu with option: '" + choiceViewGlobalOptions[dlg1.SelectedLabel].ToString() + "'");
      this.Change_Menu_Action(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
    }

    private void Change_Search_Options()
    {
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return; // true;

      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(137) + " ..."); // Search ...
      var choiceSearch = new List<string>();

      dlg.Add(GUILocalizeStrings.Get(10798615)); //Search global movies by property 
      choiceSearch.Add("globalproperty");

      dlg.Add(GUILocalizeStrings.Get(10798608)); //Search global persons by role
      choiceSearch.Add("globalpersons");

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

      //if (MyFilmsDetail.ExtendedStartmode("User defined search items 1 and 2"))
      //{
      //  for (int i = 0; i < 2; i++)
      //  {
      //    if (Helper.FieldIsSet(MyFilms.conf.StrSearchItem[i]))
      //    {
      //      if (MyFilms.conf.StrSearchText[i].Length == 0)
      //        dlg.Add(GUILocalizeStrings.Get(137) + " " + MyFilms.conf.StrSearchItem[i]); //Specific search with no text
      //      else
      //        dlg.Add(GUILocalizeStrings.Get(137) + " " + MyFilms.conf.StrSearchText[i]); //Specific search  text
      //      choiceSearch.Add(string.Format("search{0}", i.ToString()));
      //    }
      //  }
      //}

      if (this.facadeFilms.SelectedListItemIndex > -1 && !this.facadeFilms.SelectedListItem.IsFolder)
      {
        dlg.Add(GUILocalizeStrings.Get(1079866)); //Search related movies by persons
        choiceSearch.Add("analogyperson");

        dlg.Add(GUILocalizeStrings.Get(10798614)); //Search related movies by property
        choiceSearch.Add("analogyproperty");
      }

      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1) return; // true;

      if (choiceSearch[dlg.SelectedLabel] == "analogyperson")
      {
        SearchRelatedMoviesbyPersons((int)this.facadeFilms.SelectedListItem.ItemId, false);
        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        dlg.DeInit();
        return;
      }

      if (choiceSearch[dlg.SelectedLabel] == "analogyproperty")
      {
        SearchRelatedMoviesbyProperties((int)this.facadeFilms.SelectedListItem.ItemId, false);
        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        dlg.DeInit();
        return;
      }
      if (choiceSearch[dlg.SelectedLabel] == "randomsearch")
      {
        //SearchMoviesbyRandomWithTrailer(false); 
        SearchMoviesbyRandomWithTrailerOriginalVersion(false);
        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        dlg.DeInit();
        return;
      }
      if (choiceSearch[dlg.SelectedLabel] == "globalareas")
      {
        SearchMoviesbyAreas(false);
        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        dlg.DeInit();
        return;
      }
      if (choiceSearch[dlg.SelectedLabel] == "globalproperty")
      {
        SearchMoviesbyProperties(false, string.Empty, string.Empty);
        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        dlg.DeInit();
        return;
      }

      if (choiceSearch[dlg.SelectedLabel] == "globalpersons")
      {
        SearchMoviesbyPersons(false, string.Empty, string.Empty);
        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        dlg.DeInit();
        return;
      }

      var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      if (null == keyboard) return; //if (null == keyboard) return true;
      keyboard.Reset();
      keyboard.Text = "";
      keyboard.DoModal(GetID);
      if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
      {
        UpdateRecentSearch(keyboard.Text, 20);
        switch (choiceSearch[dlg.SelectedLabel])
        {
          case "title":
            if (control_searchText(keyboard.Text))
            {
              SaveListState(false);
              conf.ViewContext = ViewContext.Movie;
              conf.StrSelect = conf.StrTitle1 + " like '*" + keyboard.Text + "*'";
              conf.StrTxtSelect = GUILocalizeStrings.Get(369) + " [*" + keyboard.Text + @"*]"; // selection ...
              conf.StrTitleSelect = "";
              SetLabelView("search"); // show "search"
              conf.CurrentView = "MovieNameSearch";
              GetFilmList();
            }
            else return; // false;
            break;
          case "actors":
            if (control_searchText(keyboard.Text))
            {
              SaveListState(false);
              conf.Boolselect = true;
              conf.CurrentView = "ActorNameSearch";
              conf.StrTitleSelect = "";
              conf.StrViewSelect = "";
              conf.IndexedChars = 0;
              conf.Boolindexed = false;
              conf.BoolSkipViewState = false;

              filmcover.Filename = "";
              viewcover.Filename = "";
              personcover.Filename = "";
              groupcover.Filename = "";

              conf.ViewContext = ViewContext.Person;
              conf.WStrSort = "Actors";
              conf.WStrSortSens = " ASC";
              conf.Wselectedlabel = "";
              conf.StrPersons = ""; // conf.StrPersons = keyboard.Text;
              conf.StrTxtSelect = GUILocalizeStrings.Get(344) + " [*" + keyboard.Text + @"*]"; // selection ...
              conf.StrTitleSelect = "";
              SetLabelSelect(conf.StrTxtSelect);
              SetLabelView("search"); // show "search"
              conf.StrSelect = "Actors like '*" + keyboard.Text + "*'";
              getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, keyboard.Text, true, "");
            }
            else return; // false;
            break;
          case "search0":
          case "search1":
            //int i = 0;
            //if (choiceSearch[dlg.SelectedLabel] == "search1") i = 1;
            //var ds = new AntMovieCatalog();
            //if (control_searchText(keyboard.Text))
            //{
            //  if (ds.Movie.Columns[conf.StrSearchItem[i]].DataType.Name == "string") 
            //    conf.StrSelect = conf.StrSearchItem[i] + " like '*" + keyboard.Text + "*'";
            //  else 
            //    conf.StrSelect = conf.StrSearchItem[i] + " = '" + keyboard.Text + "'";
            //  conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + conf.StrSearchText[i] + " [*" + keyboard.Text + @"*]";
            //  conf.StrTitleSelect = "";
            //  GetFilmList();
            //}
            //else return; // false;
            break;
        }
      }
      else
      {
        dlg.DeInit();
        Change_Search_Options(); // recall search menu
      }
    }

    public static string GetDayRange(int age)
    {
      if (age < 0) return "7" + GUILocalizeStrings.Get(10798960); //unknown
      if (age == 0) return "0" + GUILocalizeStrings.Get(10798961); //today
      if (age <= 1) return "1" + GUILocalizeStrings.Get(10798962); //yesterday
      if (age <= 7) return "2" + GUILocalizeStrings.Get(10798963); //last week
      if (age <= 30) return "3" + GUILocalizeStrings.Get(10798964); //last month
      if (age <= 90) return "4" + GUILocalizeStrings.Get(10798965); //last 3 month
      if (age <= 365) return "5" + GUILocalizeStrings.Get(10798966); //last year
      if (age <= 3 * 365) return "6" + GUILocalizeStrings.Get(10798967); //last 3 years
      return "7" + GUILocalizeStrings.Get(10798968); //older than 3 years
    }

    private static int GetValueRange(int iValue)
    {
      if (iValue == 0) return 0;
      if (iValue <= 1) return 1;
      if (iValue <= 5) return 5;
      if (iValue <= 10) return 10;
      if (iValue <= 20) return 20;
      if (iValue <= 50) return 50;
      if (iValue <= 100) return 100;
      if (iValue <= 200) return 200;
      if (iValue <= 500) return 500;
      if (iValue <= 1000) return 1000;
      if (iValue <= 2000) return 2000;
      if (iValue <= 5000) return 5000;
      if (iValue <= 10000) return 10000;
      if (iValue <= 20000) return 20000;
      return iValue;
    }

    public static Type GetColumnType(string fieldname)
    {
      using (AntMovieCatalog ds = new AntMovieCatalog())
      {
        foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ColumnName == fieldname))
        {
          return dc.DataType;
        }
      }
      return null;
    }

    public static FieldType GetFieldType(string fieldname)
    {
      if (IsDateField(fieldname)) return FieldType.Date;
      if (IsAlphaNumericalField(fieldname)) return FieldType.AlphaNumeric;
      if (IsDecimalField(fieldname)) return FieldType.Decimal;
      if (IsPersonField(fieldname)) return FieldType.Person;
      if (IsTitleField(fieldname)) return FieldType.Title;
      return FieldType.Default;
    }

    private static bool IsPersonField(FieldType fieldtype)
    {
      return (fieldtype == FieldType.Person);
    }
    private static bool IsPersonField(string fieldname)  // "Persons", "Actors", "Producer", "Director", "Writer", "Borrower"
    {
      if (string.Compare(fieldname, "Persons", true) == 0) return true;
      if (string.Compare(fieldname, "Actors", true) == 0) return true;
      if (string.Compare(fieldname, "Producer", true) == 0) return true;
      if (string.Compare(fieldname, "Director", true) == 0) return true;
      if (string.Compare(fieldname, "Writer", true) == 0) return true;
      if (string.Compare(fieldname, "Borrower", true) == 0) return true;
      return false;
    }

    private static bool IsCategoryYearCountryField(string fieldname)  // "Category", "Year", "Country"
    {
      if (string.Compare(fieldname, "Category", true) == 0) return true;
      if (string.Compare(fieldname, "Year", true) == 0) return true;
      if (string.Compare(fieldname, "Country", true) == 0) return true;
      return false;
    }

    private static bool IsPersonsField(string fieldname)
    {
      return PersonTypes.Any(personField => string.Compare(fieldname, personField, true) == 0); // "Persons", "Actors", "Producer", "Director", "Writer", "Borrower"
    }

    private static bool IsAlphaNumericalField(FieldType fieldtype)
    {
      return (fieldtype == FieldType.AlphaNumeric);
    }
    private static bool IsSplittableField(string fieldname)
    {
      if (string.Compare(fieldname, "OriginalTitle", true) == 0) return false;
      if (string.Compare(fieldname, "TranslatedTitle", true) == 0) return false;
      if (string.Compare(fieldname, "FormattedTitle", true) == 0) return false;
      if (string.Compare(fieldname, "VirtualPathTitle", true) == 0) return false;
      if (string.Compare(fieldname, "Description", true) == 0) return false;
      if (string.Compare(fieldname, "Comment", true) == 0) return false;
      if (string.Compare(fieldname, "Source", true) == 0) return false;
      if (string.Compare(fieldname, "SourceTrailer", true) == 0) return false;
      if (string.Compare(fieldname, "Date", true) == 0) return false;
      if (string.Compare(fieldname, "DateWatched", true) == 0) return false;
      if (string.Compare(fieldname, "DateAdded", true) == 0) return false;
      if (string.Compare(fieldname, "Year", true) == 0) return false;
      if (string.Compare(fieldname, "RecentlyAdded", true) == 0) return false;
      if (IsDecimalField(fieldname)) return false;
      if (IsAlphaNumericalField(fieldname)) return false;
      return true;
    }

    private static bool IsAlphaNumericalField(string fieldname)
    {
      // if (string.Compare(fieldname, "Year", true) == 0) return true;
      if (string.Compare(fieldname, "Length", true) == 0) return true;
      if (string.Compare(fieldname, "Size", true) == 0) return true;
      if (string.Compare(fieldname, "Disks", true) == 0) return true;
      if (string.Compare(fieldname, "AudioChannelCount", true) == 0) return true;
      if (string.Compare(fieldname, "AudioBitrate", true) == 0) return true;
      if (string.Compare(fieldname, "VideoBitrate", true) == 0) return true;
      if (string.Compare(fieldname, "Aspectratio", true) == 0) return true;
      if (string.Compare(fieldname, "Framerate", true) == 0) return true;
      if (string.Compare(fieldname, "Resolution", true) == 0) return true;
      if (string.Compare(fieldname, "Number", true) == 0) return true;
      if (string.Compare(fieldname, "LastPosition", true) == 0) return true;
      //if (string.Compare(fieldname, "CustomField1", true) == 0) return true;
      //if (string.Compare(fieldname, "CustomField2", true) == 0) return true;
      //if (string.Compare(fieldname, "CustomField3", true) == 0) return true;
      // if (string.Compare(fieldname, "Languages", true) == 0) return true; // removed, as it was not splitted otherwise ...
      return false;
    }

    private static bool IsDecimalField(string fieldname)
    {
      if (string.Compare(fieldname, "AgeAdded", true) == 0) return true;
      if (string.Compare(fieldname, "Rating", true) == 0) return true;
      if (string.Compare(fieldname, "RatingUser", true) == 0) return true;
      return false;
    }

    private static bool IsTitleField(FieldType fieldtype)
    {
      return (fieldtype == FieldType.Title);
    }
    private static bool IsTitleField(string fieldname)
    {
      if (string.Compare(fieldname, "OriginalTitle", true) == 0) return true;
      if (string.Compare(fieldname, "TranslatedTitle", true) == 0) return true;
      if (string.Compare(fieldname, "FormattedTitle", true) == 0) return true;
      // if (string.Compare(fieldname, "VirtualPathTitle", true) == 0) return true;
      return false;
    }

    private static bool IsDateField(FieldType fieldtype)
    {
      return (fieldtype == FieldType.Date);
    }
    private static bool IsDateField(string fieldname)
    {
      if (string.Compare(fieldname, "Date", true) == 0) return true;
      if (string.Compare(fieldname, "DateAdded", true) == 0) return true;
      if (string.Compare(fieldname, "DateWatched", true) == 0) return true;
      return false;
    }

    private static bool IsDateTimeField(string fieldname)
    {
      if (string.Compare(fieldname, "DateAdded", true) == 0) return true;
      if (string.Compare(fieldname, "DateWatched", true) == 0) return true;
      if (string.Compare(fieldname, "DateWatched", true) == 0) return true;
      return false;
    }


    static readonly Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]", RegexOptions.Compiled);
    static readonly Regex oRegexReplace = new Regex("[,;]", RegexOptions.Compiled);

    public static ArrayList Search_String(string champselect)
    {
      return Search_String(champselect, false);
    }
    public static ArrayList Search_String(string champselect, bool reverseNames)
    {
      //Match theMatch = Regex.Match(source, pattern, RegexOptions.Compiled);
      //MatchCollection theMatches = Regex.Matches(source, pattern, RegexOptions.Compiled);
      // Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]", RegexOptions.Compiled);
      // MatchCollection oMatches = Regex.Matches(champselect, "\\([^\\)]*?[,;].*?[\\(\\)]", RegexOptions.Compiled);

      MatchCollection oMatches = oRegex.Matches(champselect);
      champselect = oMatches.Cast<Match>().Aggregate(champselect, (current, oMatch) => current.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, string.Empty)));
      ArrayList wtab = new ArrayList();

      int wi;
      string[] Sep = conf.ListSeparator;
      string[] roleSep = conf.RoleSeparator;
      //char[] SepAsChars; string tS = String.Empty; for (int i = 0; i <= Sep.Length; i++) tS += Sep[i]; SepAsChars = tS.ToCharArray(); string[] arSplit = champselect.Split(SepAsChars, StringSplitOptions.RemoveEmptyEntries);
      string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
      string wzone = string.Empty;
      int wzoneIndexPosition;
      for (wi = 0; wi < arSplit.Length; wi++)
      {
        if (arSplit[wi].Length > 0)
        {
          // wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim()); // Replaced for performancereasons - HTML cleanup was not necessary and was VERY slow!
          wzone = arSplit[wi].Replace("  ", " ").Trim();
          for (int i = 0; i <= 4; i++)
          {
            if (roleSep[i].Length > 0)
            {
              wzoneIndexPosition = wzone.IndexOf(roleSep[i], StringComparison.OrdinalIgnoreCase); //wzoneIndexPosition = wzone.IndexOf(conf.RoleSeparator[i],StringComparison.OrdinalIgnoreCase);

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
          if (reverseNames && wzone.Contains(" ")) // Reverse Names "Bruce Willis" -> "Willis, Bruce"
          {
            wzone = wzone.Substring(wzone.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1) + ", " + wzone.Substring(0, wzone.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase));
          }

          if (wzone.Length > 0)
          {
            wtab.Add(wzone);
          }
        }
      }
      return wtab;
    }

    public static List<grabber.DBPersonInfo> Search_String_Persons(string champselect, bool reverseNames)
    {

      MatchCollection oMatches = oRegex.Matches(champselect);
      champselect = oMatches.Cast<Match>().Aggregate(champselect, (current, oMatch) => current.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, string.Empty)));
      List<grabber.DBPersonInfo> wtab = new List<grabber.DBPersonInfo>();

      int wi;
      string[] Sep = conf.ListSeparator;
      string[] roleSep = conf.RoleSeparator;
      //char[] SepAsChars; string tS = String.Empty; for (int i = 0; i <= Sep.Length; i++) tS += Sep[i]; SepAsChars = tS.ToCharArray(); string[] arSplit = champselect.Split(SepAsChars, StringSplitOptions.RemoveEmptyEntries);
      string[] arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
      string wzone = string.Empty;
      string wzoneRole = string.Empty;
      int wzoneIndexPosition;
      for (wi = 0; wi < arSplit.Length; wi++)
      {
        if (arSplit[wi].Length > 0)
        {
          // wzone = MediaPortal.Util.HTMLParser.removeHtml(arSplit[wi].Trim()); // Replaced for performancereasons - HTML cleanup was not necessary and was VERY slow!
          wzone = arSplit[wi].Replace("  ", " ").Trim();
          for (int i = 0; i <= 4; i++)
          {
            if (roleSep[i].Length > 0)
            {
              wzoneIndexPosition = wzone.IndexOf(roleSep[i], StringComparison.OrdinalIgnoreCase); //wzoneIndexPosition = wzone.IndexOf(conf.RoleSeparator[i],StringComparison.OrdinalIgnoreCase);

              if (wzoneIndexPosition == wzone.Length - 1)
              {
                wzone = string.Empty;
                wzoneRole = string.Empty;
                break;
              }
              if (wzoneIndexPosition > 1 && wzoneIndexPosition < wzone.Length)
              {
                wzoneRole = wzone.Substring(wzoneIndexPosition + 1).Trim().Trim('(').Trim(')').Replace("as", "").Replace("  ", " ").Trim();
                wzone = wzone.Substring(0, wzoneIndexPosition).Trim();
              }
            }
          }
          if (reverseNames && wzone.Contains(" ")) // Reverse Names "Bruce Willis" -> "Willis, Bruce"
          {
            wzone = wzone.Substring(wzone.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1) + ", " + wzone.Substring(0, wzone.LastIndexOf(" ", StringComparison.OrdinalIgnoreCase));
          }

          if (wzone.Length > 0)
          {
            grabber.DBPersonInfo person = new grabber.DBPersonInfo();
            person.Name = wzone;
            person.Job = wzoneRole;
            wtab.Add(person);
          }
        }
      }
      return wtab;
    }

    public static ArrayList SubItemGrabbing(string champselect)
    {
      var oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
      MatchCollection oMatches = oRegex.Matches(champselect);
      foreach (Match oMatch in oMatches)
      {
        var oRegexReplace = new Regex("[,;]");
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
        }
      }
      return wtab;
    }

    public static ArrayList SubTitleGrabbing(string champselect)
    {
      var oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
      MatchCollection oMatches = oRegex.Matches(champselect);
      foreach (Match oMatch in oMatches)
      {
        var oRegexReplace = new Regex("[,;]");
        champselect = champselect.Replace(oMatch.Value, oRegexReplace.Replace(oMatch.Value, ""));
      }
      var wtab = new ArrayList();

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
        }
      }
      return wtab;
    }

    public static ArrayList SubWordGrabbing(string champselect, int minchars, bool filter)
    {
      LogMyFilms.Debug("(SubWordGrabbing): InputString: '" + champselect + "'");
      Regex oRegex = new Regex("\\([^\\)]*?[,;].*?[\\(\\)]");
      MatchCollection oMatches = oRegex.Matches(champselect);
      foreach (Match oMatch in oMatches)
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
        }
      }
      return wtab;
    }

    private bool LoadWatchedFlagPossible()
    {
      // Available (Files are Local) Images
      string sWatchedFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlaywatched.png";
      string sUnWatchedFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayunwatched.png";

      // Not Available (Files are not Local) Images
      string sWatchedNAFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayNAwatched.png";
      string sUnWatchedNAFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayNAunwatched.png";

      // return if images dont exists
      if (!(File.Exists(sWatchedFilename) &&
            File.Exists(sUnWatchedFilename) &&
            File.Exists(sWatchedNAFilename) &&
            File.Exists(sUnWatchedNAFilename)))
        return false;

      return true;
    }

    private bool LoadWatchedFlag(GUIListItem item, bool bWatched, bool bAvailable)
    {
      // Available (Files are Local) Images
      string sWatchedFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlaywatched.png";
      string sUnWatchedFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayunwatched.png";

      // Not Available (Files are not Local) Images
      string sWatchedNaFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayNAwatched.png";
      string sUnWatchedNaFilename = GUIGraphicsContext.Skin + @"\Media\MyFilms\overlayNAunwatched.png";

      item.PinImage = bWatched
                        ? (!bAvailable ? sWatchedNaFilename : sWatchedFilename)
                        : (!bAvailable ? sUnWatchedNaFilename : sUnWatchedFilename);
      return true;
    }

    private bool LoadIndexSkinThumbs(GUIListItem item)
    {
      if (!File.Exists(GUIGraphicsContext.Skin + @"\Media\alpha\a.png")) return false; // return, if skin does not support index thumbs

      //item.ThumbnailImage = GUIGraphicsContext.Skin + @"\Media\alpha\" + "Logo leer.png";
      //return true;

      string strStartLetter = (item.Label != EmptyFacadeValue && item.Label.Length > 0) ? item.Label.Substring(0, 1) : "Logo leer";
      if (strStartLetter.IsNumerical()) strStartLetter = "#";
      string indexThumb = GUIGraphicsContext.Skin + @"\Media\alpha\" + strStartLetter + ".png";
      if (!File.Exists(indexThumb)) indexThumb = GUIGraphicsContext.Skin + @"\Media\alpha\" + "Logo leer" + ".png";
      item.ThumbnailImage = indexThumb;
      // disable the following two, if you don't want index thumbs in list view
      item.IconImage = indexThumb;
      item.IconImageBig = indexThumb;
      return true;
    }

    /// <summary>Performs 'getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, "*", true, string.Empty)' in background thread</summary>
    private void getSelectFromDivxThreaded()
    {
      new Thread(delegate()
      {
        {
          MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation); // GUIWaitCursor.Init(); GUIWaitCursor.Show();
          getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, "*", true, string.Empty);
          MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              // GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // removed, as it was causing trouble with menu focus control ...
            }
            return 0;
          }, 0, 0, null);
      })
        {
          Name = "MyFilmsgetSelectFromDivx",
          IsBackground = true,
          Priority = ThreadPriority.Normal // Priority = ThreadPriority.AboveNormal
        }.Start();

    }

    private void ClearFacade()
    {
      try
      {
        if (this.facadeFilms.ListLayout != null)
          this.facadeFilms.ListLayout.Clear();

        if (this.facadeFilms.AlbumListLayout != null)
          this.facadeFilms.AlbumListLayout.Clear();

        if (this.facadeFilms.ThumbnailLayout != null)
          this.facadeFilms.ThumbnailLayout.Clear();

        if (this.facadeFilms.FilmstripLayout != null)
          this.facadeFilms.FilmstripLayout.Clear();

        if (this.facadeFilms.CoverFlowLayout != null)
          this.facadeFilms.CoverFlowLayout.Clear();

        if (this.facadeFilms != null)
          this.facadeFilms.Clear();

        if (this.facadeFilms != null) this.facadeFilms.Focus = true;
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("Error preparing Facade... " + ex.Message + ex.StackTrace);
      }
    }


    /// <summary>Loads online movie lists from TMDB and links them to local DB entries</summary>
    /// <param name="TMDBfunction">Select online movie list from TMDB to be loaded - e.g. latest movies, person movies, etc.</param>
    /// 
    private void GetSelectFromTMDB(string TMDBfunction)
    {
      conf.WStrSort = conf.StrTitle1; // needed for forwardnavigation to local movies
      string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
      LogMyFilms.Debug("(GetSelectFromTMDB) - CallerFeature           : '" + TMDBfunction ?? "" + "'");
      LogMyFilms.Debug("(GetSelectFromTMDB) - GlobalFilterString      : '" + GlobalFilterString + "'");
      LogMyFilms.Debug("(GetSelectFromTMDB) - conf.StrDfltSelect      : '" + conf.StrDfltSelect + "'");

      #region Setup variables and configure sorting and buttons
      Prev_ItemID = -1;
      Prev_Label = string.Empty;

      MyFilmsDetail.clearGUIProperty("nbobjects.value"); // clear counts for the start ....
      GUIPropertyManager.SetProperty("#itemcount", "0");

      BtnSrtBy.IsEnabled = true;
      BtnSrtBy.Label = (conf.BoolSortCountinViews) ? (GUILocalizeStrings.Get(98) + GUILocalizeStrings.Get(1079910)) : (GUILocalizeStrings.Get(98) + GUILocalizeStrings.Get(103)); // sort: count / sort: name
      BtnSrtBy.IsAscending = (conf.BoolSortCountinViews) ? (conf.WStrSortSensCount == " ASC") : (conf.WStrSortSens == " ASC");

      conf.Boolselect = true;
      conf.Wselectedlabel = "";
      conf.StrTxtSelect = TMDBfunction;
      GUIControl.ClearControl(GetID, facadeFilms.GetID);
      // ClearFacade(); // facadeFilms.Clear();
      Change_Layout_Action(MyFilms.conf.WStrLayOut);
      #endregion

      #region get movie lists from TMDB ...
      switch (TMDBfunction)
      {
        case "Latest Movies":
          #region TmdbPopular GetLatestMovies(int page)
          watch.Reset(); watch.Start();

          GUIBackgroundTask.Instance.ExecuteInBackgroundAndCallback(() =>
          {
            return LatestMovies;
          },
          delegate(bool success, object result)
          {
            if (!success) DoBack();
            else
            {
              IEnumerable<PopularMovie> movies = result as IEnumerable<PopularMovie>;
              // clear facade
              GUIControl.ClearControl(GetID, facadeFilms.GetID);

              if (!movies.Any())
              {
                GUIUtils.ShowNotifyDialog(GUIUtils.PluginName(), "NoLatestMovies");
                DoBack(); return;
              }

              // Add each movie mark remote if not in collection            
              #region Populate the facade ...

              int itemId = 0;
              GUIListItem item = null;
              OnlineMovie ovMovie = null;
              foreach (PopularMovie movie in movies)
              {
                item = new GUIListItem();
                ovMovie = new OnlineMovie();
                ovMovie.PopMovie = movie;
                // AntMovieCatalog.MovieRow AntMovie = new AntMovieCatalog.MovieDataTable().NewMovieRow();
                item.ItemId = Int32.MaxValue - itemId;
                item.TVTag = ovMovie;
                item.Label = (movie.title.Length > 0) ? movie.title : movie.original_title;
                item.Label2 = movie.vote_average + " (" + movie.vote_count + ")";
                item.Label3 = movie.release_date;
                item.Path = "";
                item.IsRemote = true;
                item.IsFolder = true;
                item.IconImage = conf.DefaultCover;
                item.IconImageBig = conf.DefaultCover;
                item.ThumbnailImage = conf.DefaultCover;
                // Utils.SetDefaultIcons(item);
                //item.Item = movie.Images;
                //item.IsPlayed = movie.Watched;
                item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                this.facadeFilms.Add(item);
                itemId++;

                // add image for download
                // movieImages.Add(movie);
              }
              #endregion

              MyFilmsDetail.setGUIProperty("nbobjects.value", this.facadeFilms.Count.ToString());
              GUIPropertyManager.SetProperty("#itemcount", this.facadeFilms.Count.ToString());
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              // SetDummyControlsForFacade(conf.ViewContext); // set them here, as we don't need to change them in Lst_Detailed...

              // Download movie images Async and set to facade
              // GetImages(movieImages);
              GetImagesForTMDB();
            }
          }, "GettingLatestMovies", true); // false = no timeout !

          watch.Stop();
          LogMyFilms.Debug("'loaded all movies from TMDB (" + (watch.ElapsedMilliseconds) + " ms)");
          break;
          #endregion
        case "Popular Movies":
          #region TmdbPopular GetPopularMovies(int page)
          watch.Reset(); watch.Start();

          GUIBackgroundTask.Instance.ExecuteInBackgroundAndCallback(() =>
          {
            return PopularMovies;
          },
          delegate(bool success, object result)
          {
            if (!success) DoBack();
            else
            {
              IEnumerable<PopularMovie> movies = result as IEnumerable<PopularMovie>;
              // clear facade
              GUIControl.ClearControl(GetID, facadeFilms.GetID);

              if (!movies.Any())
              {
                GUIUtils.ShowNotifyDialog(GUIUtils.PluginName(), "NoPopularMovies");
                DoBack(); return;
              }

              // Add each movie mark remote if not in collection            
              #region Populate the facade ...

              int itemId = 0;
              GUIListItem item = null;
              OnlineMovie ovMovie = null;
              foreach (PopularMovie movie in movies)
              {
                item = new GUIListItem();
                ovMovie = new OnlineMovie();
                ovMovie.PopMovie = movie;
                // AntMovieCatalog.MovieRow AntMovie = new AntMovieCatalog.MovieDataTable().NewMovieRow();
                item.ItemId = Int32.MaxValue - itemId;
                item.TVTag = ovMovie;
                item.Label = (movie.title.Length > 0) ? movie.title : movie.original_title;
                item.Label2 = movie.vote_average + " (" + movie.vote_count + ")";
                item.Label3 = movie.release_date;
                item.Path = "";
                item.IsRemote = true;
                item.IsFolder = true;
                item.IconImage = conf.DefaultCover;
                item.IconImageBig = conf.DefaultCover;
                item.ThumbnailImage = conf.DefaultCover;
                // Utils.SetDefaultIcons(item);
                //item.Item = movie.Images;
                //item.IsPlayed = movie.Watched;
                item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                this.facadeFilms.Add(item);
                itemId++;

                // add image for download
                // movieImages.Add(movie);
              }
              #endregion

              MyFilmsDetail.setGUIProperty("nbobjects.value", this.facadeFilms.Count.ToString());
              GUIPropertyManager.SetProperty("#itemcount", this.facadeFilms.Count.ToString());
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              // SetDummyControlsForFacade(conf.ViewContext); // set them here, as we don't need to change them in Lst_Detailed...

              // Download movie images Async and set to facade
              // GetImages(movieImages);
              GetImagesForTMDB();
            }
          }, "GettingPopularMovies", true); // false = no timeout !

          watch.Stop();
          LogMyFilms.Debug("'loaded all movies from TMDB (" + (watch.ElapsedMilliseconds) + " ms)");
          break;
          #endregion
        case "Now Playing Movies":
          #region TmdbNowPlaying GetNowPlayingMovies(int page)
          watch.Reset(); watch.Start();

          GUIBackgroundTask.Instance.ExecuteInBackgroundAndCallback(() =>
          {
            return NowPlayingMovies;
          },
          delegate(bool success, object result)
          {
            if (!success) DoBack();
            else
            {
              IEnumerable<PopularMovie> movies = result as IEnumerable<PopularMovie>;
              // clear facade
              GUIControl.ClearControl(GetID, facadeFilms.GetID);

              if (!movies.Any())
              {
                GUIUtils.ShowNotifyDialog(GUIUtils.PluginName(), "NoNowPlayingMovies");
                DoBack(); return;
              }

              // Add each movie mark remote if not in collection            
              #region Populate the facade ...

              int itemId = 0;
              GUIListItem item = null;
              OnlineMovie ovMovie = null;
              foreach (PopularMovie movie in movies)
              {
                item = new GUIListItem();
                ovMovie = new OnlineMovie();
                ovMovie.PopMovie = movie;
                // AntMovieCatalog.MovieRow AntMovie = new AntMovieCatalog.MovieDataTable().NewMovieRow();
                item.ItemId = Int32.MaxValue - itemId;
                item.TVTag = ovMovie;
                item.Label = (movie.title.Length > 0) ? movie.title : movie.original_title;
                item.Label2 = movie.vote_average + " (" + movie.vote_count + ")";
                item.Label3 = movie.release_date;
                item.Path = "";
                item.IsRemote = true;
                item.IsFolder = true;
                item.IconImage = conf.DefaultCover;
                item.IconImageBig = conf.DefaultCover;
                item.ThumbnailImage = conf.DefaultCover;
                // Utils.SetDefaultIcons(item);
                //item.Item = movie.Images;
                //item.IsPlayed = movie.Watched;
                item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                this.facadeFilms.Add(item);
                itemId++;

                // add image for download
                // movieImages.Add(movie);
              }
              #endregion

              MyFilmsDetail.setGUIProperty("nbobjects.value", this.facadeFilms.Count.ToString());
              GUIPropertyManager.SetProperty("#itemcount", this.facadeFilms.Count.ToString());
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              // SetDummyControlsForFacade(conf.ViewContext); // set them here, as we don't need to change them in Lst_Detailed...

              // Download movie images Async and set to facade
              // GetImages(movieImages);
              GetImagesForTMDB();
            }
          }, "GettingNowPlayingMovies", true); // false = no timeout !

          watch.Stop();
          LogMyFilms.Debug("'loaded all movies from TMDB (" + (watch.ElapsedMilliseconds) + " ms)");
          break;
          #endregion
        case "Top rated movies":
          #region TmdbTopRated GetTopRatedMovies(int page)
          watch.Reset(); watch.Start();

          GUIBackgroundTask.Instance.ExecuteInBackgroundAndCallback(() =>
          {
            return TopRatedMovies;
          },
          delegate(bool success, object result)
          {
            if (!success) DoBack();
            else
            {
              IEnumerable<PopularMovie> movies = result as IEnumerable<PopularMovie>;
              // clear facade
              GUIControl.ClearControl(GetID, facadeFilms.GetID);

              if (!movies.Any())
              {
                GUIUtils.ShowNotifyDialog(GUIUtils.PluginName(), "NoTopRatedMovies");
                DoBack(); return;
              }

              // Add each movie mark remote if not in collection            
              #region Populate the facade ...

              int itemId = 0;
              GUIListItem item = null;
              OnlineMovie ovMovie = null;
              foreach (PopularMovie movie in movies)
              {
                item = new GUIListItem();
                ovMovie = new OnlineMovie();
                ovMovie.PopMovie = movie;
                // AntMovieCatalog.MovieRow AntMovie = new AntMovieCatalog.MovieDataTable().NewMovieRow();
                item.ItemId = Int32.MaxValue - itemId;
                item.TVTag = ovMovie;
                item.Label = (movie.title.Length > 0) ? movie.title : movie.original_title;
                item.Label2 = movie.vote_average + " (" + movie.vote_count + ")";
                item.Label3 = movie.release_date;
                item.Path = "";
                item.IsRemote = true;
                item.IsFolder = true;
                item.IconImage = conf.DefaultCover;
                item.IconImageBig = conf.DefaultCover;
                item.ThumbnailImage = conf.DefaultCover;
                // Utils.SetDefaultIcons(item);
                //item.Item = movie.Images;
                //item.IsPlayed = movie.Watched;
                item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                this.facadeFilms.Add(item);
                itemId++;

                // add image for download
                // movieImages.Add(movie);
              }
              #endregion

              MyFilmsDetail.setGUIProperty("nbobjects.value", this.facadeFilms.Count.ToString());
              GUIPropertyManager.SetProperty("#itemcount", this.facadeFilms.Count.ToString());
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              // SetDummyControlsForFacade(conf.ViewContext); // set them here, as we don't need to change them in Lst_Detailed...

              // Download movie images Async and set to facade
              // GetImages(movieImages);
              GetImagesForTMDB();
            }
          }, "GettingTopRatedMovies", true); // false = no timeout !

          watch.Stop();
          LogMyFilms.Debug("'loaded all movies from TMDB (" + (watch.ElapsedMilliseconds) + " ms)");
          break;
          #endregion
        case "Upcoming Movies":
          #region TmdbUpcoming GetUpcomingMovies(int page)
          watch.Reset(); watch.Start();

          GUIBackgroundTask.Instance.ExecuteInBackgroundAndCallback(() =>
          {
            return UpcomingMovies;
          },
          delegate(bool success, object result)
          {
            if (!success) DoBack();
            else
            {
              IEnumerable<PopularMovie> movies = result as IEnumerable<PopularMovie>;
              // clear facade
              GUIControl.ClearControl(GetID, facadeFilms.GetID);

              if (!movies.Any())
              {
                GUIUtils.ShowNotifyDialog(GUIUtils.PluginName(), "NoUpcomingMovies");
                DoBack(); return;
              }

              // Add each movie mark remote if not in collection            
              #region Populate the facade ...

              int itemId = 0;
              GUIListItem item = null;
              OnlineMovie ovMovie = null;
              foreach (PopularMovie movie in movies)
              {
                item = new GUIListItem();
                ovMovie = new OnlineMovie();
                ovMovie.PopMovie = movie;
                // AntMovieCatalog.MovieRow AntMovie = new AntMovieCatalog.MovieDataTable().NewMovieRow();
                item.ItemId = Int32.MaxValue - itemId;
                item.TVTag = ovMovie;
                item.Label = (movie.title.Length > 0) ? movie.title : movie.original_title;
                item.Label2 = movie.vote_average + " (" + movie.vote_count + ")";
                item.Label3 = movie.release_date;
                item.Path = "";
                item.IsRemote = true;
                item.IsFolder = true;
                item.IconImage = conf.DefaultCover;
                item.IconImageBig = conf.DefaultCover;
                item.ThumbnailImage = conf.DefaultCover;
                // Utils.SetDefaultIcons(item);
                //item.Item = movie.Images;
                //item.IsPlayed = movie.Watched;
                item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                this.facadeFilms.Add(item);
                itemId++;

                // add image for download
                // movieImages.Add(movie);
              }
              #endregion

              MyFilmsDetail.setGUIProperty("nbobjects.value", this.facadeFilms.Count.ToString());
              GUIPropertyManager.SetProperty("#itemcount", this.facadeFilms.Count.ToString());
              GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
              // SetDummyControlsForFacade(conf.ViewContext); // set them here, as we don't need to change them in Lst_Detailed...

              // Download movie images Async and set to facade
              // GetImages(movieImages);
              GetImagesForTMDB();
            }
          }, "GettingUpcomingMovies", true); // false = no timeout !

          watch.Stop();
          LogMyFilms.Debug("'loaded all movies from TMDB (" + (watch.ElapsedMilliseconds) + " ms)");
          break;
          #endregion

        default:
          return;
      }

      #endregion
    }

    private void GetImagesForTMDB()
    {
      StopLoadingViewDetails = false;

      new Thread(delegate()
      {
        string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
        LogMyFilms.Debug("GetImagesForTMDB - detected language = '" + language + "'");
        Tmdb api = new TMDB.Tmdb(TmdbApiKey, language); // language is optional, default is "en"
        TmdbConfiguration tmdbConf = api.GetConfiguration();

        #region images
        Thread.Sleep(25);
        Stopwatch watch = new Stopwatch(); watch.Reset(); watch.Start();
        int i;
        DataRow[] rtemp = null;
        rtemp = BaseMesFilms.ReadDataMovies(GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + conf.StrDfltSelect, "", conf.WStrSort, conf.WStrSortSens);
        // DataRow[] rtemp = r;
        List<GUIListItem> itemlist = new List<GUIListItem>();

        for (i = 0; i < facadeFilms.Count; i++)
        {
          if (StopLoadingViewDetails) break; // stop download if we have exited window
          try
          {
            GUIListItem item = facadeFilms[i];
            itemlist.Add(item);

            #region check if local available ...
            int iMoviesLocally = 0;
            //if (rtemp != null)
            //{
            //  int count = rtemp.Count(x => x[wStrSort].ToString().Contains(item.Label));
            //  // int count = rtemp.Count(x => x[wStrSort].ToString().IndexOf(item.Label, StringComparison.OrdinalIgnoreCase) > 0);
            //  if (count > 0) item.Label2 = BaseMesFilms.Translate_Column(wStrSort) + " (" + count + ")"; // LogMyFilms.Debug("role: '" + WStrSort + "', count: '" + count + "'");

            //  //foreach (string role in PersonTypes)
            //  //{
            //  // if (rtemp != null && (conf.StrPersons.Length > 0 || countItems)) {}
            //  //  count = rtemp.Count(x => x[role].ToString().Contains(item.Label));
            //  //  // LogMyFilms.Debug("role: '" + role + "', count: '" + count + "'");
            //  //  if (count > 0) item.Label2 = (string.IsNullOrEmpty(item.Label2)) ? BaseMesFilms.Translate_Column(role) + " (" + count + ")" : item.Label2 + ", " + BaseMesFilms.Translate_Column(role) + " (" + count + ")";
            //  //}
            //}
            OnlineMovie movie = item.TVTag as OnlineMovie;
            var year = DateTime.Parse(movie.PopMovie.release_date).Year;
            // iMoviesLocally = rtemp.Select("Year like '" + year + "' AND TranslatedTitle like '*" + item.Label + "*'", conf.StrSorta + conf.StrSortSens).Select(p => p[conf.StrTitle1] != DBNull.Value).Count();
            iMoviesLocally = rtemp.Count(x => x["Year"].ToString().Contains(year.ToString()) && x["TranslatedTitle"].ToString().Contains(movie.PopMovie.title));
            LogMyFilms.Debug("CountLocalItems - found '" + iMoviesLocally + "' items for (" + year.ToString() + ") '" + movie.PopMovie.title + "' !");
            item.IsRemote = (iMoviesLocally == 0);
            // newLabel = "* " + BaseMesFilms.ReadDataMovies(conf.StrGlobalFilterString + mfView.Filter + " AND " + conf.StrDfltSelect, "", conf.StrSorta, conf.StrSortSens).Select(p => p[conf.StrTitle1] != DBNull.Value).Count().ToString();

            #endregion

            #region load trailer info from TMDB
            TmdbMovieTrailers MovieTrailers = api.GetMovieTrailers(movie.PopMovie.id, language);
            LogMyFilms.Debug("GetTrailers - total Results: '" + MovieTrailers.youtube.Count + "' YouTube Trailers found");
            for (int j = 0; j < MovieTrailers.youtube.Count; j++)
            {
              LogMyFilms.Debug("GetTrailers - Trailer '" + j + "', name = '" + MovieTrailers.youtube[j].name + "', size = '" + MovieTrailers.youtube[j].size + "', source = '" + MovieTrailers.youtube[j].source + "'");
              if (j == 0) item.Path = MovieTrailers.youtube[j].source;
            }
            LogMyFilms.Debug("'loaded all movies from TMDB (" + (watch.ElapsedMilliseconds) + " ms)");
            #endregion
          }
          catch (Exception ex)
          {
            LogMyFilms.Warn("GetImages() - error setting facadelist item '" + i + "': " + ex.Message);
          }
        }
        watch.Stop();
        LogMyFilms.Debug("GetImages() - Threaded facade images loader finished after '" + i + "' items (" + (watch.ElapsedMilliseconds) + " ms)");
        #endregion

        #region counts for person lists
        if (rtemp != null && conf.StrPersons.Length > 0)
        {
          Thread.Sleep(50);
          watch.Reset(); watch.Start();
          int[] facadeCounts = new int[facadeFilms.Count];
          try
          {
            for (int j = 0; j < facadeFilms.Count; j++) facadeCounts[j] = 0;

            for (int j = 0; j < rtemp.Length; j++)
            {
              if (StopLoadingViewDetails) break; // stop download if we have exited window
              string title = rtemp[j][conf.StrTitle1].ToString();
              for (i = 0; i < facadeFilms.Count; i++)
              {
                OnlineMovie movie = facadeFilms[i].TVTag as OnlineMovie;
                if (title.Contains(movie.PopMovie.title)) // if (value.IndexOf(facadeLabels[i], StringComparison.OrdinalIgnoreCase) > 0)
                {
                  facadeCounts[i]++;
                  facadeFilms[i].Label3 = " (" + facadeCounts[i] + ")";
                }
              }
            }
            // for (int j = 0; j < facadeFilms.Count; j++) facadeFilms[j].Label2 = label2NamePrefix + " (" + facadeCounts[j] + ")";
          }
          catch (Exception ex)
          {
            LogMyFilms.Warn("GetCounts() - error setting facadelist item '" + i + "': " + ex.Message);
          }
          watch.Stop();
          LogMyFilms.Debug("GetCounts() - Threaded facade details loader exit after '" + i + "' items (" + (watch.ElapsedMilliseconds) + " ms)");
        }
        #endregion

        #region get thumbs for TMDB Items
        for (i = 0; i < facadeFilms.Count; i++)
        {
          try
          {
            if (facadeFilms[i] != null)
            {
              GUIListItem item = facadeFilms[i];
              OnlineMovie movie = facadeFilms[i].TVTag as OnlineMovie;
              if (!string.IsNullOrEmpty(movie.PopMovie.poster_path))
              {
                //string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";
                //if (GUITextureManager.LoadFromMemory(ImageFast.FastFromFile(strActiveFacadeImages[0]), texture, 0, 0, 0) > 0)
                //{
                //  item.ThumbnailImage = texture;
                //  item.IconImage = texture;
                //  item.IconImageBig = texture;
                //}

                //item.IconImage = strActiveFacadeImages[1];
                //item.IconImageBig = strActiveFacadeImages[0];
                //item.ThumbnailImage = strActiveFacadeImages[0];

                // if selected force an update of thumbnail
                //GUIListItem selectedItem = GUIControl.GetSelectedListItem(ID_MyFilms, 50);
                //if (selectedItem == item) GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, selectedItem.ItemId, 0, null));
              }
            }
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
          catch (Exception ex)
          {
            LogMyFilms.Warn("GetImagesTMDB() - error setting facadelist item '" + i + "': " + ex.Message);
          }
        }
        GetImagesforTMDBFilmList(itemlist);

        #endregion
      })
      {
        IsBackground = true,
        Name = "MyFilms Image Detector and Downloader",
        Priority = ThreadPriority.BelowNormal
      }.Start();
    }

    private void GetImagesforTMDBFilmList(List<GUIListItem> itemsWithThumbs)
    {
      StopLoadingFilmlistDetails = false;
      string CoverThumbDir = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies";
      const string wStrSort = "TMDB";

      // if there is a Default.jpg in the view subfolder
      // string strPathViewsRoot = (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\") ? conf.StrPathViews : (conf.StrPathViews + "\\");
      bool isperson = false;
      string strThumbDirectory = (isperson) ? MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Persons\" : MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Views\" + wStrSort.ToLower() + @"\";
      if (!Directory.Exists(strThumbDirectory)) Directory.CreateDirectory(strThumbDirectory);

      // split the downloads in X+ groups and do multithreaded downloading
      int groupSize = (int)Math.Max(1, Math.Floor((double)itemsWithThumbs.Count / 2)); // Guzzi: Set group to x to only allow x thread(s)
      int groups = (int)Math.Ceiling((double)itemsWithThumbs.Count / groupSize);

      for (int i = 0; i < groups; i++)
      {
        var groupList = new List<GUIListItem>();
        for (int j = groupSize * i; j < groupSize * i + (groupSize * (i + 1) > itemsWithThumbs.Count ? itemsWithThumbs.Count - groupSize * i : groupSize); j++)
        {
          groupList.Add(itemsWithThumbs[j]);
        }

        new Thread(delegate(object o)
        {
          string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
          grabber.TheMoviedb tmdbapi = new grabber.TheMoviedb();
          TMDB.Tmdb api = new TMDB.Tmdb(TmdbApiKey, language); // language is optional, default is "en"
          TMDB.TmdbConfiguration tmdbConf = api.GetConfiguration();
          foreach (string posterSize in tmdbConf.images.poster_sizes)
          {
            LogMyFilms.Debug("Available TMDB Poster Size: '" + posterSize + "'");
          }
          var items = (List<GUIListItem>)o;
          foreach (GUIListItem item in items)
          {
            // stop download if we have exited window
            if (StopLoadingFilmlistDetails)
            {
              items.SafeDispose();
              break;
            }
            LogMyFilms.Debug("GetImagesTMDB() - loading TMDB Details for '" + item.Label + "'");
            OnlineMovie movie = item.TVTag as OnlineMovie;
            if (movie == null)
            {
              LogMyFilms.Error("GetImagesTMDB() - OnlineMovie object is 'null' for movie '" + item.Label + "'");
              continue;
            }
            movie.MovieImages = api.GetMovieImages(movie.PopMovie.id, language);
            if (movie.MovieImages.posters.Count == 0)
            {
              movie.MovieImages = api.GetMovieImages(movie.PopMovie.id);
              LogMyFilms.Debug("GetImagesTMDB() - no german posters found - used default and found '" + movie.MovieImages.posters.Count + "'");
            }
            movie.Trailers = api.GetMovieTrailers(movie.PopMovie.id);
            // movie.AlternateTitles = api.GetMovieAlternateTitles(movie.PopMovie.id);
            movie.MovieCast = api.GetMovieCast(movie.PopMovie.id);

            #region Poster
            // stop download if we have exited window
            if (StopLoadingFilmlistDetails) break;

            LogMyFilms.Debug("GetImagesTMDB() - loading TMDB Details for '" + item.Label + "'");
            string remoteThumb = (movie.MovieImages.posters.Count > 0) ? tmdbConf.images.base_url + "w500" + movie.MovieImages.posters[0].file_path : "";
            LogMyFilms.Debug("GetImagesTMDB() - remoteThumb = '" + remoteThumb + "'");
            string localThumb = Path.Combine(strThumbDirectory, item.Label + ".jpg");
            LogMyFilms.Debug("GetImagesTMDB() - localThumb = '" + localThumb + "'");


            if (!string.IsNullOrEmpty(remoteThumb) && !string.IsNullOrEmpty(localThumb))
            {
              if (Grabber.GrabUtil.DownloadImage(remoteThumb, localThumb))
              {
                //// notify that image has been downloaded
                //item.NotifyPropertyChanged("PosterImageFilename");
                item.IconImage = localThumb;
                item.IconImageBig = localThumb;
                item.ThumbnailImage = localThumb;
              }
            }
            #endregion

            #region Fanart
            // stop download if we have exited window
            if (StopLoadingFilmlistDetails) break;
            //if (!TraktSettings.DownloadFanart) continue;

            //string remoteFanart = item.Fanart;
            //string localFanart = item.FanartImageFilename;

            //if (!string.IsNullOrEmpty(remoteFanart) && !string.IsNullOrEmpty(localFanart))
            //{
            //    if (GUIImageHandler.DownloadImage(remoteFanart, localFanart))
            //    {
            //        // notify that image has been downloaded
            //        item.NotifyPropertyChanged("FanartImageFilename");
            //    }
            //}
            #endregion

            // string[] strActiveFacadeImages = SetViewThumbs(wStrSort, item.Label, strThumbDirectory, false, null, conf.DefaultCover, false);
            //string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";
            //if (GUITextureManager.LoadFromMemory(ImageFast.FastFromFile(strActiveFacadeImages[0]), texture, 0, 0, 0) > 0)
            //{
            //  item.ThumbnailImage = texture;
            //  item.IconImage = texture;
            //  item.IconImageBig = texture;
            //}

            //item.IconImage = strActiveFacadeImages[1];
            //item.IconImageBig = strActiveFacadeImages[0];
            //item.ThumbnailImage = strActiveFacadeImages[0];

            // if selected force an update of thumbnail
            //GUIListItem selectedItem = GUIControl.GetSelectedListItem(ID_MyFilms, 50);
            //if (selectedItem == item) GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, selectedItem.ItemId, 0, null));


            // string strThumb; // cached cover
            // string strThumbSmall; // cached cover for Icons - small resolution

            #region group-collection image handling (disabled)
            ////if (!File.Exists(item.ThumbnailImage)) // No Coverart in DB - so handle it !
            //if (item.IsFolder) // special handling for groups (movie collections - NOT views!)
            //{
            //  string strThumbGroup = GetGroupImage(item); // thumbnail for Groups/collections
            //  item.IconImage = strThumbGroup;
            //  item.IconImageBig = strThumbGroup;
            //  item.ThumbnailImage = strThumbGroup;
            //}
            #endregion

            item.MusicTag = item.ThumbnailImage; // keep Original one in music tag for big list thumb ...
            //// strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, item.DVDLabel); // item.DVDLabel is sTitle
            //strThumb = MediaPortal.Util.Utils.GetCoverArtName(CoverThumbDir, item.DVDLabel); // item.DVDLabel is sTitle
            //strThumbSmall = strThumb.Substring(0, strThumb.LastIndexOf(".")) + "_s" + Path.GetExtension(strThumb);

            //if (!string.IsNullOrEmpty(item.ThumbnailImage) && item.ThumbnailImage != conf.DefaultCover && !File.Exists(strThumb))
            //{
            //  Picture.CreateThumbnail(item.ThumbnailImage, strThumbSmall, 100, 150, 0, Thumbs.SpeedThumbsSmall);
            //  Picture.CreateThumbnail(item.ThumbnailImage, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
            //  LogMyFilms.Debug("GetFimList: Background thread creating thumbimage for sTitle: '" + item.DVDLabel + "'");
            //}
            //if (File.Exists(strThumb))
            //{
            //  item.IconImage = strThumbSmall;
            //  item.IconImageBig = strThumb;
            //  item.ThumbnailImage = strThumb;
            //}
            //else
            //{
            //  if (conf.DefaultCover.Length > 0)
            //  {
            //    item.IconImage = conf.DefaultCover;
            //    item.IconImageBig = conf.DefaultCover;
            //    item.ThumbnailImage = conf.DefaultCover;
            //  }
            //}

            #region thumb downloads (no active)
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
            #endregion

            // Thread.Sleep(10);
          }
        })
        {
          IsBackground = true,
          Priority = ThreadPriority.BelowNormal,
          Name = "MyFilms FilmList Image Detector " + i
        }.Start(groupList);
      }
    }

    /// <summary>Selects records for display grouping them as required</summary>
    /// <param name="WstrSelect">Select this kind of records</param>
    /// <param name="WStrSort">Sort based on this</param>
    /// <param name="WStrSortSens">Asc/Desc. Ascending or descending sort order</param>
    /// <param name="NewWstar">Entries must contain this string to be included</param>
    /// <param name="ClearIndex">Reset Selected Item Index</param>
    /// <param name="SelItem">Select entry matching this string if not empty</param>
    /// 
    public void getSelectFromDivx(string WstrSelect, string WStrSort, string WStrSortSens, string NewWstar, bool ClearIndex, string SelItem)
    {
      #region example
      //Example for Actors list:
      //conf.WStrSort = "ACTORS";
      //conf.Wselectedlabel = "";
      //conf.WStrSortSens = " ASC";
      //conf.StrPersons = keyboard.Text;
      //getSelectFromDivx("Actors like '*" + keyboard.Text + "*'", conf.WStrSort, conf.WStrSortSens, keyboard.Text, true, "");
      #endregion

      string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
      LogMyFilms.Debug("(GetSelectFromDivx) - GlobalFilterString      : '" + GlobalFilterString + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - conf.StrViewSelect      : '" + conf.StrViewSelect + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - conf.StrDfltSelect      : '" + conf.StrDfltSelect + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - WstrSelect              : '" + WstrSelect + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - WStrSort - WStrSortSens : '" + WStrSort + "' - '" + WStrSortSens + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - NewWstar - ClearIndex   : '" + NewWstar + "' - '" + ClearIndex + "'");
      LogMyFilms.Debug("(GetSelectFromDivx) - SelItem  - StrPersons   : '" + SelItem + "' - '" + conf.StrPersons + "'");

      #region Setup variables and configure sorting and buttons
      Prev_ItemID = -1;
      Prev_Label = string.Empty;

      MyFilmsDetail.clearGUIProperty("nbobjects.value"); // clear counts for the start ....
      GUIPropertyManager.SetProperty("#itemcount", "0");

      GUIListItem item = null;
      string champselect = "";
      string wchampselect = "";
      string wchampselectIndexed = "";
      ArrayList w_tableau = new ArrayList();

      FieldType fieldType = GetFieldType(WStrSort);
      bool isperson = IsPersonField(fieldType);
      bool isdate = IsDateField(fieldType);
      bool issplitfield = IsSplittableField(WStrSort);
      bool istitlefield = IsTitleField(WStrSort);
      bool dontsplitvalues = MyFilms.conf.BoolDontSplitValuesInViews;
      bool showEmptyValues = MyFilms.conf.BoolShowEmptyValuesInViews;
      bool isrecentlyadded = (WStrSort == "RecentlyAdded"); // calculate recently added fields
      bool isindexedtitle = (WStrSort == "IndexedTitle"); // calculate recently added fields
      bool reversenames = (isperson && conf.BoolReverseNames);
      // string defaultcover = (isperson) ? conf.DefaultCoverArtist : ((istitlefield) ? conf.DefaultCover : conf.DefaultCoverViews);

      //DateTime now = DateTime.Now;

      BtnSrtBy.IsEnabled = true;
      BtnSrtBy.Label = (conf.BoolSortCountinViews) ? (GUILocalizeStrings.Get(98) + GUILocalizeStrings.Get(1079910)) : (GUILocalizeStrings.Get(98) + GUILocalizeStrings.Get(103)); // sort: count / sort: name
      BtnSrtBy.IsAscending = (conf.BoolSortCountinViews) ? (conf.WStrSortSensCount == " ASC") : (WStrSortSens == " ASC");
      conf.ViewContext = (isperson) ? ViewContext.Person : ViewContext.Group;

      //if (isperson && !string.IsNullOrEmpty(conf.StrPersons))
      //{
      //  conf.StrSelect = conf.WStrSort + " like '*" + conf.StrPersons + "*'";
      //}

      if (MyFilms.conf.IndexedChars == 0) MyFilms.conf.Boolindexed = false;
      bool isindexed = conf.Boolindexed;
      int indexedChars = MyFilms.conf.IndexedChars;
      conf.Wstar = NewWstar;
      conf.Boolselect = true;
      conf.Wselectedlabel = "";
      if (ClearIndex) conf.StrIndex = 0;
      // if (conf.UseListViewForGoups) Change_Layout_Action(0); else 
      GUIControl.ClearControl(GetID, facadeFilms.GetID);
      // ClearFacade(); // facadeFilms.Clear();
      Change_Layout_Action(MyFilms.conf.WStrLayOut);
      #endregion

      #region Collection of all items or subitems
      // Collect List of all attributes in w_tableau
      int Wnb_enr = 0;
      int Wnb_enrIndexed = 0; // counts for indexed views
      int wi = 0;
      conf.DbSelection = new string[] { GlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, WstrSelect, WStrSort, WStrSortSens, false.ToString(), false.ToString() };

      watch.Reset(); watch.Start();
      foreach (DataRow row in BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, WstrSelect, WStrSort, WStrSortSens))
      {
        if (isdate)
        {
          // champselect = string.Format("{0:yyyy/MM/dd}", row["DateAdded"]);
          DateTime tmpdate;
          champselect = (DateTime.TryParse(row[WStrSort].ToString(), out tmpdate)) ? string.Format("{0:yyyy/MM/dd}", tmpdate.ToShortDateString()) : ""; // .ToString("o", CultureInfo.InvariantCulture)
          //try { champselect = string.Format("{0:dd/MM/yyyy}", DateTime.Parse(row[WStrSort].ToString()).ToShortDateString()); }
          //catch (Exception) { champselect = ""; }
        }
        else
          champselect = row[WStrSort].ToString().Trim();

        if (issplitfield && !dontsplitvalues)
        {
          ArrayList wtab = Search_String(champselect, reversenames); //ArrayList wtab = Search_String(champselect, isperson);
          if (wtab.Count > 0)
          {
            for (wi = 0; wi < wtab.Count; wi++) w_tableau.Add(wtab[wi]); //w_tableau.AddRange(wtab);
          }
          else if (showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            w_tableau.Add(champselect);
          }
        }
        else
        {
          if (champselect.Length > 0 || showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            w_tableau.Add(champselect);
          }
        }
      }
      watch.Stop();
      LogMyFilms.Debug("(GetSelectFromDivx) - Read View Names for '" + WStrSort + "' finished (splittable items = '" + issplitfield + "', dontsplitvalues = '" + dontsplitvalues + "', reversenames = '" + reversenames + "') (" + (watch.ElapsedMilliseconds) + " ms)");
      #endregion

      #region Sorting based on FieldType using custom IComparer
      watch.Reset(); watch.Start();
      switch (fieldType)
      {
        case FieldType.AlphaNumeric:
        case FieldType.Decimal:
          if (WStrSortSens == " ASC")
          {
            IComparer myComparer = new AlphanumComparatorFast();
            w_tableau.Sort(0, w_tableau.Count, myComparer);
          }
          else
          {
            IComparer myComparer = new AlphanumComparatorFast();
            w_tableau.Sort(0, w_tableau.Count, myComparer);
            w_tableau.Reverse();
          }
          break;

        default: // default sorter
          if (WStrSortSens == " ASC")
          {
            w_tableau.Sort(0, w_tableau.Count, StringComparer.OrdinalIgnoreCase);
            //IComparer myComparer = new myForwarderClass();
            //w_tableau.Sort(0, w_tableau.Count, myComparer);
            //w_tableau.Sort(0, w_tableau.Count, null);
          }
          else
          {
            w_tableau.Sort(0, w_tableau.Count, StringComparer.OrdinalIgnoreCase);
            w_tableau.Reverse();
            //IComparer myComparer = new myReverserClass();
            //w_tableau.Sort(0, w_tableau.Count, myComparer);
          }
          break;
      }
      watch.Stop();
      LogMyFilms.Debug("(GetSelectFromDivx) - Base Sorting Finished (FieldType = '" + fieldType + "') (" + (watch.ElapsedMilliseconds) + " ms)");
      #endregion

      #region Sort by occurencies, if requested
      if (conf.BoolSortCountinViews) // if sorting by counts is selected ... do it...
      {
        watch.Reset(); watch.Start();
        int itemcount = 0;
        string currentitem = "";
        string itemValue = "";

        var list = new List<KeyValuePair<string, int>>();
        for (wi = 0; wi != w_tableau.Count; wi++)
        {
          itemValue = w_tableau[wi].ToString();
          if (!isindexed && string.Compare(currentitem, itemValue, StringComparison.OrdinalIgnoreCase) == 0) // Are the strings equal? Then add count!
            itemcount++;
          else if (isindexed && string.Compare(itemValue, 0, currentitem, 0, indexedChars, StringComparison.OrdinalIgnoreCase) == 0) //  CultureInfo.CurrentCulture, CompareOptions.OrdinalIgnoreCase
          {
            itemcount++; // count items of distinct property
            //if (string.Compare(champselect, wchampselectIndexed, StringComparison.OrdinalIgnoreCase) != 0)
            //{
            //  itemcountIndexed++;
            //  currentitemIndexed = itemValue;
            //}
          }
          else
          {
            if (itemcount > 0 && itemValue.Length > 0)
            {
              for (int i = (wi - itemcount); i != wi; i++)
              {
                list.Add(new KeyValuePair<string, int>(currentitem, itemcount));
              }
            }
            // LogMyFilms.Debug("(GetSelectFromDivx) - Counting item: '" + currentitem + "', Count = '" + itemcount + "'");
            itemcount = 1;
            currentitem = w_tableau[wi].ToString();
          }
        }
        if (itemcount > 0 && itemValue.Length > 0)
        {
          for (int i = (wi - itemcount); i != wi; i++)
          {
            list.Add(new KeyValuePair<string, int>(currentitem, itemcount));
          }
        }
        watch.Stop();
        LogMyFilms.Debug("(GetSelectFromDivx) - Collecting Countlist finished (" + (watch.ElapsedMilliseconds) + " ms)");

        watch.Reset(); watch.Start();
        if (conf.WStrSortSensCount == " ASC") list.Sort(CompareCount);
        else list.Sort(CompareCountDescending);
        watch.Stop();
        LogMyFilms.Debug("(GetSelectFromDivx) - Sorting by ocurrencies finished (" + (watch.ElapsedMilliseconds) + " ms)");

        watch.Reset(); watch.Start();
        for (wi = 0; wi != w_tableau.Count; wi++)
        {
          w_tableau[wi] = list[wi].Key;
        }
        watch.Stop();
        LogMyFilms.Debug("(GetSelectFromDivx) - Copy back to display array finished (" + (watch.ElapsedMilliseconds) + " ms)");
      }
      #endregion

      #region count objects for indexed views ...
      //watch.Reset(); watch.Start();
      //Dictionary<object, int> counts = new Dictionary<object, int>();
      //foreach (object item in w_tableau)
      //{
      //  if (!counts.ContainsKey(item)) counts.Add(item, 1);
      //  else counts[item]++;
      //}
      //int count = counts.Count;
      //watch.Stop();
      //LogMyFilms.Debug("(GetSelectFromDivx) - Count objects for indexed view finished (" + (watch.ElapsedMilliseconds) + " ms)");
      #endregion

      #region directory handling
      string strThumbDirectory = (isperson) ? MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Persons\" : MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Views\" + WStrSort.ToLower() + @"\";

      bool getThumbs = ((MyFilms.conf.UseThumbsForPersons && isperson) || (MyFilms.conf.UseThumbsForViews && (MyFilms.conf.StrViewsDfltAll || IsCategoryYearCountryField(WStrSort)) || MyFilms.conf.StrViewsShowIndexedImgInIndViews));
      bool createFanartDir = IsCategoryYearCountryField(WStrSort);

      if (!Directory.Exists(strThumbDirectory)) // Check groupview thumbs cache directories and create them
        try { Directory.CreateDirectory(strThumbDirectory); }
        catch (Exception) { }
      if (conf.StrPathViews.Length > 0 && !Directory.Exists(conf.StrPathViews + @"\" + WStrSort.ToLower())) // Check groupview thumbs (sub)directories and create them
        try { Directory.CreateDirectory(conf.StrPathViews + @"\" + WStrSort.ToLower()); }
        catch (Exception) { }
      #endregion

      // if (isperson) { AsynUpdateActors(w_tableau); } // Launch Backgroundworker to (off)-load actors artwork and create cache thumbs

      #region populating facade
      LogMyFilms.Debug("(GetSelectFromDivx) - Facadesetup Groups Started");
      //int number = -1;
      bool countItems = (isperson && !WstrSelect.Contains("not like") && !string.IsNullOrEmpty(WstrSelect) && !WstrSelect.Contains("**")); // extensive counts only for conditional lists, otherwise takes too much time!
      watch.Reset(); watch.Start();
      for (wi = 0; wi != w_tableau.Count; wi++)
      {
        champselect = w_tableau[wi].ToString();
        if (!isindexed && string.Compare(champselect, wchampselect, StringComparison.OrdinalIgnoreCase) == 0)  // if (!MyFilms.conf.Boolindexed && string.Compare(champselect, wchampselect, true) == 0) // Are the strings equal? Then add count!
        {
          Wnb_enr++; // count items of distinct property
        }
        else if (isindexed && string.Compare(champselect, 0, wchampselect, 0, indexedChars, StringComparison.OrdinalIgnoreCase) == 0) //  CultureInfo.CurrentCulture, CompareOptions.OrdinalIgnoreCase
        {
          Wnb_enr++; // count items of distinct property
          if (string.Compare(champselect, wchampselectIndexed, StringComparison.OrdinalIgnoreCase) != 0)
          {
            Wnb_enrIndexed++;
            wchampselectIndexed = champselect;
          }
        }
        else
        {
          if (conf.Wstar == "*" || ListConditionTrue(champselect, conf.Wstar, conf.Boolindexedreturn, reversenames))
          {
            if (Wnb_enr > 0 && (wchampselect.Length > 0 || conf.BoolShowEmptyValuesInViews))
            {
              //number++;
              item = new GUIListItem();
              //item.ItemId = number;
              string label = (isrecentlyadded) ? wchampselect.Substring(1) : wchampselect;
              string label3 = (istitlefield && !isindexed && label.IndexOf(conf.TitleDelim) > 0) ? label.Substring(0, label.LastIndexOf(conf.TitleDelim)) : ""; // loads title path
              if (istitlefield && label.IndexOf(conf.TitleDelim) > 0) label = label.Substring(label.LastIndexOf(conf.TitleDelim) + 1);
              if (isindexed && indexedChars > 0 && label.Length >= indexedChars) label = label.Substring(0, indexedChars).ToUpperInvariant();
              item.Label = (label.Length == 0) ? EmptyFacadeValue : label; // show <empty> value if empty
              item.Label2 = (isindexed) ? (Wnb_enrIndexed + " (" + Wnb_enr + ")") : Wnb_enr.ToString(); // preset - will be repopulated with counts, if requested in thread ...
              item.Label3 = label3;
              item.Path = WStrSort.ToLower();
              //item.TVTag = (isperson) ? "person" : "group";
              item.IsFolder = true;
              //item.ThumbnailImage = defaultcover;
              //item.IconImage = defaultcover;
              //item.IconImageBig = defaultcover;
              item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
              this.facadeFilms.Add(item);
              if (SelItem != "" && item.Label == SelItem) conf.StrIndex = this.facadeFilms.Count - 1; //test if this item is one to select
            }
            Wnb_enr = 1;
            wchampselect = champselect;
            Wnb_enrIndexed = 1;
            wchampselectIndexed = champselect;
          }
        }
      }

      if (Wnb_enr > 0 && (wchampselect.Length > 0 || conf.BoolShowEmptyValuesInViews))
      {
        //number++;
        item = new GUIListItem();
        //item.ItemId = number; // Only used in GetFilmList
        // item.Label = (wchampselect.Length == 0) ? EmptyFacadeValue : (isrecentlyadded) ? wchampselect.Substring(1) : wchampselect;
        string label = (isrecentlyadded) ? wchampselect.Substring(1) : wchampselect;
        string label3 = (istitlefield && !isindexed && label.IndexOf(conf.TitleDelim) > 0) ? label.Substring(0, label.LastIndexOf(conf.TitleDelim)) : ""; // loads title path
        if (istitlefield && label.IndexOf(conf.TitleDelim) > 0) label = label.Substring(label.LastIndexOf(conf.TitleDelim) + 1);
        if (isindexed && indexedChars > 0 && label.Length >= indexedChars) label = label.Substring(0, indexedChars).ToUpperInvariant();
        item.Label = (label.Length == 0) ? EmptyFacadeValue : label; // show <empty> value if empty
        item.Label2 = (isindexed) ? (Wnb_enrIndexed + " (" + Wnb_enr + ")") : Wnb_enr.ToString(); // preset - will be repopulated with counts, if requested in thread ...
        item.Label3 = label3;
        //item.Label3 = WStrSort;
        //if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll) MediaPortal.Util.Utils.SetDefaultIcons(item);
        item.Path = WStrSort.ToLower();
        //item.TVTag = (isperson) ? "person" : "group";
        item.IsFolder = true;
        //item.ThumbnailImage = defaultcover;
        //item.IconImage = defaultcover;
        //item.IconImageBig = defaultcover;
        item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
        this.facadeFilms.Add(item);
        if (SelItem != "" && item.Label == SelItem) conf.StrIndex = facadeFilms.Count - 1; //test if this item is one to select
        Wnb_enr = 0;
      }
      if (item != null) item.FreeMemory();
      watch.Stop();
      LogMyFilms.Debug("(GetSelectFromDivx) - Facadesetup Groups Finished (" + (watch.ElapsedMilliseconds) + " ms)");
      #endregion

      #region Final settings and thumbs
      conf.StrTxtSelect = (string.IsNullOrEmpty(conf.StrSelect) || conf.StrSelect == (conf.StrTitle1 + " not like ''")) ? GUILocalizeStrings.Get(1079870) : ("[" + conf.StrSelect.Substring(conf.StrSelect.IndexOf("'")).Trim(new Char[] { '\'', '*' }) + "]");  // "Selection"
      if (conf.Wstar != "*" && !string.IsNullOrEmpty(conf.Wstar))
        conf.StrTxtSelect += (" [" + ((conf.Wstar.Length > 1) ? "*" : "") + conf.Wstar + "*]"); // conf.StrTxtSelect += " " + BaseMesFilms.Translate_Column(WStrSort) + (" [" + ((conf.Wstar.Length > 1) ? "*" : "") + conf.Wstar + "*]"); // conf.StrTxtSelect += " " + GUILocalizeStrings.Get(1079896) + " [*" + conf.Wstar + "*]"; // add to "Selection": Persons with Filter
      SetLabelSelect(conf.StrTxtSelect);
      conf.StrSelect = WstrSelect;
      // if (conf.StrPersons.Length == 0) conf.StrFilmSelect = string.Empty; // disabled, as otherwise we don't get filtered results for single movies ...

      if ((conf.StrIndex > this.facadeFilms.Count - 1) || (conf.StrIndex < 0)) //check index within bounds, will be unless xml file heavily edited
        conf.StrIndex = 0;

      // will be done in Lst_Details later ...GUIControl.ShowControl(GetID, 34); // hide film controls ....

      if (this.facadeFilms.Count == 0)
      {
        ShowMessageDialog(GUILocalizeStrings.Get(10798624), GUILocalizeStrings.Get(10798637), GUILocalizeStrings.Get(10798638)); //"no movies matching the view" - " show filmlist"
        DisplayAllMovies();
        GetFilmList();
        SetLabelView("all");
      }
      else
      {
        if (getThumbs)
        {
          #region load first image syncronously, as asyncloading might cause flicker or even let it disappear
          if (conf.IndexedChars > 0 && conf.Boolindexed && !conf.Boolindexedreturn && MyFilms.conf.StrViewsShowIndexedImgInIndViews)
            LoadIndexSkinThumbs(facadeFilms[conf.StrIndex]);
          else
          {
            // if there is a Default.jpg in the view subfolder
            string strPathViewsRoot = (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\") ? conf.StrPathViews : (conf.StrPathViews + "\\");
            string strImageInViewsDefaultFolder = strPathViewsRoot + WStrSort.ToLower() + ".jpg";
            string DefaultViewImage = (System.IO.File.Exists(strImageInViewsDefaultFolder)) ? strImageInViewsDefaultFolder : null;
            string[] strActiveFacadeImages = SetViewThumbs(WStrSort, facadeFilms[conf.StrIndex].Label, strThumbDirectory, isperson, GetCustomViewFromViewLabel(conf.CurrentView), DefaultViewImage, conf.BoolReverseNames);
            // string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";
            this.facadeFilms[conf.StrIndex].ThumbnailImage = strActiveFacadeImages[0];
            this.facadeFilms[conf.StrIndex].IconImage = strActiveFacadeImages[1];
            this.facadeFilms[conf.StrIndex].IconImageBig = strActiveFacadeImages[0];
          }
          #endregion
          GetImages(WStrSort, strThumbDirectory, isperson, getThumbs, createFanartDir, countItems); // load the rest of images asynchronously!
        }
        //if (IsPersonField(fieldType)) MyFilmsDetail.Load_Detailed_PersonInfo(facadeFilms.SelectedListItem.Label, false);
        //else MyFilmsDetail.Load_Detailed_DB(0, false);
        //this.Load_Rating(0); // old method - nor more used
        MyFilmsDetail.setGUIProperty("nbobjects.value", this.facadeFilms.Count.ToString());
        GUIPropertyManager.SetProperty("#itemcount", this.facadeFilms.Count.ToString());
      }
      // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + "/" + GUIPropertyManager.GetProperty("#myfilms.select"));
      GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, (int)conf.StrIndex);
      if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
      // SetDummyControlsForFacade(conf.ViewContext); // set them here, as we don't need to change them in Lst_Detailed...
      #endregion
    }

    private bool ListConditionTrue(string champselect, string filterlist, bool indexedview, bool reversenames) // checks, if a single string or comma separated strings do a matching for the list ...
    {
      if (filterlist.IndexOf(',') >= 0) // comma separated list ... so process it ...
      {
        // string[] split = filterlist.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        ArrayList split = Search_String(filterlist, reversenames);
        foreach (string s in split)
        {
          if (indexedview)
          {
            if (champselect.StartsWith(s, StringComparison.OrdinalIgnoreCase))
              return true;
          }
          else
          {
            // if (champselect.ToUpper().Contains(s.Trim().ToUpper())) return true;
            if (champselect.IndexOf(s.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
              return true;
          }
        }
      }
      else
      {
        if (indexedview) // if (filterlist.Length == 1) // if it's single character compare, use it as "first character"
        {
          if (champselect.StartsWith(filterlist, StringComparison.OrdinalIgnoreCase))
            return true;
          else
            return false;
        }
        else
        {
          // if (champselect.ToUpper().Contains(filterlist.ToUpper()))
          if (champselect.IndexOf(filterlist, StringComparison.OrdinalIgnoreCase) >= 0)
            return true;
          else
            return false;
        }
      }
      return false;
    }

    private void GetImages(string wStrSort, string strThumbDirectory, bool isperson, bool getThumbs, bool createFanartDir, bool countItems)
    {
      StopLoadingViewDetails = false;

      new Thread(delegate()
      {
        #region images
        Thread.Sleep(25);
        Stopwatch watch = new Stopwatch(); watch.Reset(); watch.Start();
        int i;
        DataRow[] rtemp = null;
        if (conf.StrPersons.Length > 0) // reading full dataset only required, if personcounts are requested...
          rtemp = BaseMesFilms.ReadDataMovies(GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + conf.StrViewSelect + conf.StrDfltSelect, "", wStrSort, conf.WStrSortSens);
        // DataRow[] rtemp = r;

        MFview.ViewRow currentCustomView = null;
        currentCustomView = GetCustomViewFromViewLabel(conf.CurrentView); // Views - check, which one is active

        // if there is a Default.jpg in the view subfolder
        string strPathViewsRoot = (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\") ? conf.StrPathViews : (conf.StrPathViews + "\\");
        string strImageInViewsDefaultFolder = strPathViewsRoot + wStrSort.ToLower() + ".jpg";
        string defaultViewImage = (File.Exists(strImageInViewsDefaultFolder)) ? strImageInViewsDefaultFolder : null;
        bool reversenames = conf.BoolReverseNames;

        for (i = 0; i < facadeFilms.Count; i++)
        {
          if (StopLoadingViewDetails) break; // stop download if we have exited window
          try
          {
            GUIListItem item = facadeFilms[i];

            #region get thumbs
            if (getThumbs && facadeFilms[i] != null)
            {
              if (string.IsNullOrEmpty(facadeFilms[i].ThumbnailImage))
              {
                if (conf.IndexedChars > 0 && conf.Boolindexed && !conf.Boolindexedreturn && MyFilms.conf.StrViewsShowIndexedImgInIndViews)
                {
                  LoadIndexSkinThumbs(facadeFilms[i]);
                }
                else
                {
                  string[] strActiveFacadeImages = SetViewThumbs(wStrSort, item.Label, strThumbDirectory, isperson, currentCustomView, defaultViewImage, reversenames);
                  //string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";
                  //if (GUITextureManager.LoadFromMemory(ImageFast.FastFromFile(strActiveFacadeImages[0]), texture, 0, 0, 0) > 0)
                  //{
                  //  item.ThumbnailImage = texture;
                  //  item.IconImage = texture;
                  //  item.IconImageBig = texture;
                  //}

                  item.IconImage = strActiveFacadeImages[1];
                  item.IconImageBig = strActiveFacadeImages[0];
                  item.ThumbnailImage = strActiveFacadeImages[0];

                  // if selected force an update of thumbnail
                  //GUIListItem selectedItem = GUIControl.GetSelectedListItem(ID_MyFilms, 50);
                  //if (selectedItem == item) GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, selectedItem.ItemId, 0, null));
                }
              }
            }
            #endregion

            if (createFanartDir) MyFilmsDetail.Search_Fanart(item.Label, true, "file", true, item.ThumbnailImage, wStrSort.ToLower());

            #region get counts (disabled)
            //if (rtemp != null && conf.StrPersons.Length > 0)
            //{
            //  int count = rtemp.Count(x => x[wStrSort].ToString().Contains(item.Label));
            //  // int count = rtemp.Count(x => x[wStrSort].ToString().IndexOf(item.Label, StringComparison.OrdinalIgnoreCase) > 0);
            //  if (count > 0) item.Label2 = BaseMesFilms.Translate_Column(wStrSort) + " (" + count + ")"; // LogMyFilms.Debug("role: '" + WStrSort + "', count: '" + count + "'");

            //  //foreach (string role in PersonTypes)
            //  //{
            //  // if (rtemp != null && (conf.StrPersons.Length > 0 || countItems)) {}
            //  //  count = rtemp.Count(x => x[role].ToString().Contains(item.Label));
            //  //  // LogMyFilms.Debug("role: '" + role + "', count: '" + count + "'");
            //  //  if (count > 0) item.Label2 = (string.IsNullOrEmpty(item.Label2)) ? BaseMesFilms.Translate_Column(role) + " (" + count + ")" : item.Label2 + ", " + BaseMesFilms.Translate_Column(role) + " (" + count + ")";
            //  //}
            //}
            #endregion

            #region thumb downloads (no active)
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
            #endregion
          }
          catch (Exception ex)
          {
            LogMyFilms.Warn("GetImages() - error setting facadelist item '" + i + "': " + ex.Message);
          }
        }
        watch.Stop();
        LogMyFilms.Debug("GetImages() - Threaded facade images loader finished after '" + i + "' items (" + (watch.ElapsedMilliseconds) + " ms)");
        #endregion

        #region counts for person lists
        if (rtemp != null && conf.StrPersons.Length > 0)
        {
          Thread.Sleep(50);
          watch.Reset(); watch.Start();
          bool isReversed = conf.BoolReverseNames;
          string[] FacadeLabel = new string[facadeFilms.Count];
          int[] facadeCounts = new int[facadeFilms.Count];
          string label2NamePrefix = BaseMesFilms.Translate_Column(wStrSort);
          try
          {
            for (int j = 0; j < facadeFilms.Count; j++)
            {
              facadeCounts[j] = 0;
              FacadeLabel[j] = (isReversed) ? ReReverseName(facadeFilms[j].Label) : facadeFilms[j].Label;
            }

            for (int j = 0; j < rtemp.Length; j++)
            {
              if (StopLoadingViewDetails) break; // stop download if we have exited window
              string value = rtemp[j][wStrSort].ToString();
              for (i = 0; i < facadeFilms.Count; i++)
              {
                if (value.Contains(FacadeLabel[i])) // if (value.IndexOf(facadeLabels[i], StringComparison.OrdinalIgnoreCase) > 0)
                {
                  facadeCounts[i]++;
                  facadeFilms[i].Label2 = label2NamePrefix + " (" + facadeCounts[i] + ")";
                }
              }
            }
            // for (int j = 0; j < facadeFilms.Count; j++) facadeFilms[j].Label2 = label2NamePrefix + " (" + facadeCounts[j] + ")";
          }
          catch (Exception ex)
          {
            LogMyFilms.Warn("GetCounts() - error setting facadelist item '" + i + "': " + ex.Message);
          }
          watch.Stop();
          LogMyFilms.Debug("GetCounts() - Threaded facade details loader exit after '" + i + "' items (" + (watch.ElapsedMilliseconds) + " ms)");
        }
        #endregion

        #region load actor details for person lists
        Thread.Sleep(25);
        watch.Reset(); watch.Start();
        if (Win32API.IsConnectedToInternet() && MyFilms.conf.PersonsEnableDownloads && isperson && conf.StrPersons.Length > 0 && (!(conf.IndexedChars > 0 && conf.Boolindexed && !conf.Boolindexedreturn && MyFilms.conf.StrViewsShowIndexedImgInIndViews)))
        {
          string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
          grabber.TheMoviedb tmdbapi = new grabber.TheMoviedb();
          TMDB.Tmdb api = new TMDB.Tmdb(TmdbApiKey, language); // language is optional, default is "en"
          TMDB.TmdbConfiguration tmdbConf = api.GetConfiguration();
          IMDB _imdb = new IMDB();

          for (i = 0; i < facadeFilms.Count; i++)
          {
            if (StopLoadingViewDetails) break; // stop download if we have exited window
            try
            {
              GUIListItem item = facadeFilms[i];
              IMDBActor person = null;

              string personFacadeName = item.Label.Replace(EmptyFacadeValue, "");
              string personname = (conf.BoolReverseNames && item.Label != EmptyFacadeValue) ? ReReverseName(item.Label) : item.Label.Replace(EmptyFacadeValue, "");
              string filename = MyFilms.conf.StrPathArtist + "\\" + personname + ".jpg";  // string filename = Path.Combine(MyFilms.conf.StrPathArtist, personname); //File.Exists(MyFilms.conf.StrPathArtist + "\\" + personsname + ".jpg")))
              bool VDBexists = false;

              item.Label = personFacadeName + " (updating...)";

              #region get person info from VDB
              item.Label3 = "Loading details from VDB ...";
              ArrayList actorList = new ArrayList();
              VideoDatabase.GetActorByName(personname, actorList);
              LogMyFilms.Debug("GetImages() - found '" + actorList.Count + "' results for '" + personname + "'");
              if (actorList.Count > 0 && actorList.Count < 5)
              {
                LogMyFilms.Debug("IMDB first search result: '" + actorList[0] + "'");
                string[] strActor = actorList[0].ToString().Split(new char[] { '|' });
                // int actorID = (strActor[0].Length > 0 && strActor.Count() > 1) ? Convert.ToInt32(strActor[0]) : 0; // string actorname = strActor[1];
                int actorID = 0;
                int.TryParse(strActor[0], out actorID);
                if (actorID > 0)
                {
                  person = VideoDatabase.GetActorInfo(actorID);
                }
                if (person != null)
                {
                  item.Label3 = "ID = " + actorID + ", URL = " + person.ThumbnailUrl;
                  VDBexists = true;
                }
                else
                {
                  item.Label3 = "ID = " + actorID;
                }
              }
              #endregion

              if (person != null && File.Exists(filename))
              {
                LogMyFilms.Debug("Skip update for '" + personname + "' - VDB entry and image already present !");
                item.MusicTag = person;
                item.Label3 = "";
                item.Label = personFacadeName;
                if (StopLoadingViewDetails) break; // stop download if we have exited window
                continue;
              }

              // region update person detail infos or load new ones ...
              if ((person == null || person.DateOfBirth.Length < 1 || person.DateOfBirth.ToLower().StartsWith("unknown") || !File.Exists(filename)) && facadeFilms[i] != null)
              {
                if (person == null) person = new IMDBActor();

                #region IMDB internet search
                item.Label3 = "Searching IMDB ...";
                _imdb.FindActor(personname);

                if (_imdb.Count > 0)
                {
                  if (_imdb[0].URL.Length != 0)
                  {
                    item.Label3 = "Loading IMDB details ...";
                    //#if MP1X
                    //                    _imdb.GetActorDetails(_imdb[0], out person);
                    //#else
                    //                    _imdb.GetActorDetails(_imdb[0], false, out person);
                    //#endif
                    GUIUtils.GetActorDetails(_imdb, _imdb[0], false, out person);
                    LogMyFilms.Debug("Value foud - birthday = " + person.DateOfBirth ?? "");
                    LogMyFilms.Debug("Value foud - birthplace = " + person.PlaceOfBirth ?? "");
                  }
                }
                if (StopLoadingViewDetails) break; // stop download if we have exited window

                #endregion

                #region TMDB V3 API description
                // Search
                //TmdbMovieSearch SearchMovie(string query, int page)
                //TmdbPersonSearch SearchPerson(string query, int page)
                //TmdbCompanySearch SearchCompany(string query, int page);             

                // Person Info
                //TmdbPerson GetPersonInfo(int PersonID)
                //TmdbPersonCredits GetPersonCredits(int PersonID)
                //TmdbPersonImages GetPersonImages(int PersonID)
                //Movie Info
                //TmdbMovie GetMovieInfo(int MovieID)
                //TmdbMovie GetMovieByIMDB(string IMDB_ID)
                //TmdbMovieAlternateTitles GetMovieAlternateTitles(int MovieID, string Country)
                //TmdbMovieCast GetMovieCast(int MovieID)
                //TmdbMovieImages GetMovieImages(int MovieID)
                //TmdbMovieKeywords GetMovieKeywords(int MovieID)
                //TmdbMovieReleases GetMovieReleases(int MovieID)
                //TmdbMovieTrailers GetMovieTrailers(int MovieID)
                //TmdbSimilarMovies GetSimilarMovies(int MovieID, int page)
                //TmdbTranslations GetMovieTranslations(int MovieID)

                // Social Movie Info
                //TmdbNowPlaying GetNowPlayingMovies(int page)
                //TmdbPopular GetPopularMovies(int page)
                //TmdbTopRated GetTopRatedMovies(int page)
                //TmdbUpcoming GetUpcomingMovies(int page)
                #endregion

                #region TMDB v2 loading ...
                //List<grabber.DBPersonInfo> personlist = tmdbapi.getPersonsByName(personname, false, language);
                //if (personlist.Count > 0)
                //{
                //  grabber.DBPersonInfo f = personlist[0];

                //  if (f != null && !File.Exists(filename))
                //  {
                //    if (f.Images.Count > 0)
                //    {
                //      //grabber.DBPersonInfo persondetails = new DBPersonInfo();
                //      //persondetails = tmdbapi.getPersonsById(f.Id, string.Empty);
                //      //LogMyFilms.Info("Person Artwork - " + f.Images.Count + " Images found for '" + f.Name + "'");
                //      item.Label3 = "Loading TMDBv2 image ...";
                //      string filename1person = Grabber.GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, f.Images[0], f.Name, false, true, out filename);
                //      LogMyFilms.Debug("Person Image (TMDB) '" + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + "' downloaded for '" + f.Name + "', path = '" + filename1person + "', filename = '" + filename + "'");
                //    }
                //  }
                //}
                #endregion

                #region experimental TMDB v3 code...
                try
                {
                  item.Label3 = "Searching TMDB ...";
                  TMDB.TmdbPersonSearch tmdbPerson = api.SearchPerson(personname, 1);
                  List<TMDB.PersonResult> persons = tmdbPerson.results;

                  if (persons != null && persons.Count > 0)
                  {
                    TMDB.PersonResult pinfo = persons[0];
                    TMDB.TmdbPerson singleperson = api.GetPersonInfo(pinfo.id);
                    // TMDB.TmdbPersonImages images = api.GetPersonImages(pinfo.id);
                    // TMDB.TmdbPersonCredits personFilmList = api.GetPersonCredits(pinfo.id);
                    SetActorDetailsFromTMDB(singleperson, tmdbConf, ref person);
                    if (!string.IsNullOrEmpty(singleperson.profile_path) && !File.Exists(filename))
                    {
                      item.Label3 = "Loading TMDB image ...";
                      string filename1person = GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, person.ThumbnailUrl, personname, false, true, out filename);
                      LogMyFilms.Debug("Person Image (TMDB) '" + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + "' downloaded for '" + personname + "', path = '" + filename1person + "', filename = '" + filename + "'");
                      item.IconImage = filename;
                      item.IconImageBig = filename;
                      item.ThumbnailImage = filename;
                      item.Label3 = "TMDB ID = " + singleperson.id + ", URL = " + singleperson.profile_path;
                    }
                  }
                }
                catch (Exception tex)
                {
                  LogMyFilms.DebugException("GetImages() - error in TMDB grabbing item '" + i + "': " + tex.Message, tex);
                }
                if (StopLoadingViewDetails) break; // stop download if we have exited window
                #endregion

                #region Add actor to database to get infos in person facades later...

                item.Label3 = "Save detail info to VDB ...";

                try
                {
                  //#if MP1X
                  //                  int actorId = VideoDatabase.AddActor(person.Name);
                  //#else
                  //                  int actorId = VideoDatabase.AddActor(null, person.Name);
                  //#endif
                  int actorId = GUIUtils.AddActor(null, person.Name);
                  if (actorId > 0)
                  {
                    if (!string.IsNullOrEmpty(person.Biography)) // clean up before saving ...
                    {
                      if (person.Biography.StartsWith("From Wikipedia, the free encyclopedia."))
                      {
                        person.Biography = person.Biography.Replace("From Wikipedia, the free encyclopedia", "").TrimStart(new char[] { '.' }).Trim(new char[] { ' ', '\r', '\n' });
                      }
                    }

                    VideoDatabase.SetActorInfo(actorId, person);
                    //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);
                    item.Label = personFacadeName;
                    item.Label3 = (VDBexists) ? ("Updated ID" + actorId + ", URL = " + person.ThumbnailUrl) : ("Added ID" + actorId + ", URL = " + person.ThumbnailUrl);
                    // continue; // proceed with next item - no downloads ...
                  }
                }
                catch (Exception ex)
                {
                  item.Label = personFacadeName;
                  LogMyFilms.Debug("Error adding person to VDB: " + ex.Message, ex.StackTrace);
                }
                if (StopLoadingViewDetails) break; // stop download if we have exited window
                #endregion

                #region load missing images ...
                if (person.ThumbnailUrl.Contains("http:") && !File.Exists(filename))
                {
                  #region Thumb download deactivated, as downloading not yet working !!!
                  //if (imdbActor.ThumbnailUrl != string.Empty)
                  //{
                  //  string largeCoverArt = Utils.GetLargeCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                  //  string coverArt = Utils.GetCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                  //  Utils.FileDelete(largeCoverArt);
                  //  Utils.FileDelete(coverArt);
                  //  //DownloadCoverArt(Thumbs.MovieActors, imdbActor.ThumbnailUrl, imdbActor.Name);
                  //}
                  #endregion
                  item.Label3 = "Loading missing image ...";
                  LogMyFilms.Debug(" Image found for person '" + personname + "', URL = '" + person.ThumbnailUrl + "'");
                  string filename1person = GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, person.ThumbnailUrl, personname, false, true, out filename);
                  LogMyFilms.Debug("Person Image (IMDB) '" + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + "' downloaded for '" + personname + "', path = '" + filename1person + "', filename = '" + filename + "'");
                  item.IconImage = filename;
                  item.IconImageBig = filename;
                  item.ThumbnailImage = filename;
                  // item.NotifyPropertyChanged("ThumbnailImage");

                  item.Label3 = "URL = " + person.ThumbnailUrl;
                  item.Label = personFacadeName;
                  // continue; // proceed with next item - no downloads ...
                }
                #endregion

                // create small thumbs and assign to facade icons ...
                string[] strActiveFacadeImages = SetViewThumbs(wStrSort, item.Label, strThumbDirectory, isperson, currentCustomView, defaultViewImage, reversenames);
                item.IconImage = strActiveFacadeImages[1];
                item.IconImageBig = strActiveFacadeImages[0];
                item.ThumbnailImage = strActiveFacadeImages[0];

                item.MusicTag = person;
                item.Label3 = ""; // item.Label3 = "Update finished.";
                item.Label = personFacadeName;

                #region old stuff deactivated
                //string[] strActiveFacadeImages = SetViewThumbs(wStrSort, item.Label, strThumbDirectory, isperson, currentCustomView, defaultViewImage, reversenames);
                ////string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";
                ////if (GUITextureManager.LoadFromMemory(ImageFast.FastFromFile(strActiveFacadeImages[0]), texture, 0, 0, 0) > 0)
                ////{
                ////  item.ThumbnailImage = texture;
                ////  item.IconImage = texture;
                ////  item.IconImageBig = texture;
                ////}

                //item.IconImage = strActiveFacadeImages[1];
                //item.IconImageBig = strActiveFacadeImages[0];
                //item.ThumbnailImage = strActiveFacadeImages[0];

                //// if selected force an update of thumbnail
                ////GUIListItem selectedItem = GUIControl.GetSelectedListItem(ID_MyFilms, 50);
                ////if (selectedItem == item) GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, selectedItem.ItemId, 0, null));
                #endregion
              }

              #region thumb downloads (no active)
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
              #endregion
            }
            catch (Exception ex)
            {
              LogMyFilms.Warn("GetImages() - error setting facadelist item '" + i + "': " + ex.Message);
              LogMyFilms.DebugException("GetImages() - error setting facadelist item '" + i + "': " + ex.Message, ex);
            }
          }
        }

        watch.Stop();
        LogMyFilms.Debug("GetImages() - Threaded person details loader finished after '" + i + "' items (" + (watch.ElapsedMilliseconds) + " ms)");
        #endregion
      })
      {
        IsBackground = true,
        Name = "MyFilms Image Detector and Downloader",
        Priority = ThreadPriority.BelowNormal
      }.Start();
    }

    private void SetActorDetailsFromTMDB(TMDB.TmdbPerson tmdbPerson, TMDB.TmdbConfiguration conf, ref IMDBActor imdbPerson)
    {
      if (tmdbPerson == null)
      {
        LogMyFilms.Debug("SetActorDetailsFromTMDB() - TMDB person is 'null' - return");
        return;
      }
      if (imdbPerson == null)
      {
        LogMyFilms.Debug("SetActorDetailsFromTMDB() - IMDB person is 'null' - return");
        return;
      }
      string tmdbProfileSize = "original";
      foreach (string profileSize in conf.images.profile_sizes.Where(profileSize => profileSize == "h632"))
      {
        tmdbProfileSize = profileSize;
      }

      // imdbPerson.IMDBActorID = 
      // imdbPerson.Name = tmdbPerson.name;
      // imdbPerson.MiniBiography = tmdbPerson.biography;
      // LogMyFilms.Debug("SetActorDetailsFromTMDB() - update IMDB name     - old : '" + imdbPerson.Name + "', new: '" + tmdbPerson.name + "'");
      if (!string.IsNullOrEmpty(tmdbPerson.biography))
      {
        // LogMyFilms.Debug("SetActorDetailsFromTMDB() - update IMDB bio      - old : '" + imdbPerson.Biography + "', new: '" + tmdbPerson.biography + "'");
        imdbPerson.Biography = tmdbPerson.biography;
      }

      if (!string.IsNullOrEmpty(tmdbPerson.birthday))
      {
        LogMyFilms.Debug("SetActorDetailsFromTMDB() - update IMDB birthday - old : '" + imdbPerson.DateOfBirth + "', new: '" + tmdbPerson.birthday + "'");
        imdbPerson.DateOfBirth = tmdbPerson.birthday + ((!string.IsNullOrEmpty(tmdbPerson.deathday)) ? " (" + tmdbPerson.deathday + ")" : "");
      }
      if (!string.IsNullOrEmpty(tmdbPerson.place_of_birth))
      {
        LogMyFilms.Debug("SetActorDetailsFromTMDB() - update IMDB b-place  - old : '" + imdbPerson.PlaceOfBirth + "', new: '" + tmdbPerson.place_of_birth + "'");
        imdbPerson.PlaceOfBirth = tmdbPerson.place_of_birth;
      }
      if (!string.IsNullOrEmpty(tmdbPerson.profile_path))
      {
        LogMyFilms.Debug("SetActorDetailsFromTMDB() - update IMDB thumb    - old : '" + imdbPerson.ThumbnailUrl + "', new: '" + conf.images.base_url + tmdbProfileSize + tmdbPerson.profile_path + "'");
        imdbPerson.ThumbnailUrl = conf.images.base_url + tmdbProfileSize + tmdbPerson.profile_path;
      }
    }

    private void GetImagesOld(List<GUIListItem> itemsWithThumbs, string wStrSort, string strThumbDirectory, bool isperson, bool getThumbs, bool createFanartDir, bool countItems)
    {
      //StopLoadingViewDetails = false;

      //var groupSize = (int)Math.Max(1, Math.Floor((double)itemsWithThumbs.Count / 2)); // split the downloads in X+ groups and do multithreaded downloading // Guzzi: Set group to x to only allow x thread(s)
      //var groups = (int)Math.Ceiling((double)itemsWithThumbs.Count / groupSize);

      //for (int i = 0; i < groups; i++)
      //{
      //  var groupList = new List<GUIListItem>();
      //  for (int j = groupSize * i; j < groupSize * i + (groupSize * (i + 1) > itemsWithThumbs.Count ? itemsWithThumbs.Count - groupSize * i : groupSize); j++)
      //  {
      //    groupList.Add(itemsWithThumbs[j]);
      //  }

      //  new Thread(delegate(object o)
      //  {
      //    #region taskaction
      //    var items = (List<GUIListItem>)o;
      //    Thread.Sleep(25);
      //    DataRow[] rtemp = null;
      //    if (conf.StrPersons.Length > 0) // reading full dataset only required, if personcounts are requested...
      //      rtemp = BaseMesFilms.ReadDataMovies(GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating + conf.StrDfltSelect, "", wStrSort, conf.WStrSortSens);
      //    // DataRow[] rtemp = r;

      //    foreach (GUIListItem item in items)
      //    {
      //      // stop download if we have exited window
      //      if (StopLoadingViewDetails)
      //        break;

      //      if (getThumbs)
      //      {
      //        string[] strActiveFacadeImages = SetViewThumbs(wStrSort, item.Label, strThumbDirectory, isperson);
      //        //string texture = "[MyFilms:" + strActiveFacadeImages[0].GetHashCode() + "]";

      //        //if (GUITextureManager.LoadFromMemory(ImageFast.FastFromFile(strActiveFacadeImages[0]), texture, 0, 0, 0) > 0)
      //        //{
      //        //  item.ThumbnailImage = texture;
      //        //  item.IconImage = texture;
      //        //  item.IconImageBig = texture;
      //        //}
      //        item.ThumbnailImage = strActiveFacadeImages[0];
      //        item.IconImage = strActiveFacadeImages[1];
      //        item.IconImageBig = strActiveFacadeImages[0];

      //        // if selected force an update of thumbnail
      //        //GUIListItem selectedItem = GUIControl.GetSelectedListItem(ID_MyFilms, 50);
      //        //if (selectedItem == item) GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, selectedItem.ItemId, 0, null));
      //      }

      //      if (createFanartDir) MyFilmsDetail.Search_Fanart(item.Label, true, "file", true, item.ThumbnailImage, wStrSort.ToLower());

      //      if (conf.StrPersons.Length > 0 && rtemp != null)
      //      {
      //        int count = rtemp.Count(x => x[wStrSort].ToString().IndexOf(item.Label, StringComparison.OrdinalIgnoreCase) > 0);
      //        if (count > 0) item.Label2 = BaseMesFilms.Translate_Column(wStrSort) + " (" + count + ")"; // LogMyFilms.Debug("role: '" + WStrSort + "', count: '" + count + "'");

      //        //foreach (string role in PersonTypes)
      //        //{
      //        //  count = rtemp.Count(x => x[role].ToString().Contains(item.Label));
      //        //  // LogMyFilms.Debug("role: '" + role + "', count: '" + count + "'");
      //        //  if (count > 0) item.Label2 = (string.IsNullOrEmpty(item.Label2)) ? BaseMesFilms.Translate_Column(role) + " (" + count + ")" : item.Label2 + ", " + BaseMesFilms.Translate_Column(role) + " (" + count + ")";
      //        //}
      //      }

      //      // ToDo: Add downloader to SetViewThumbs - or here ...

      //      //string remoteThumb = item.ImageRemotePath;
      //      //if (string.IsNullOrEmpty(remoteThumb)) continue;

      //      //string localThumb = item.Image;
      //      //if (string.IsNullOrEmpty(localThumb)) continue;

      //      //if (Helper.DownloadFile(remoteThumb, localThumb))
      //      //{
      //      //  // notify that thumbnail image has been downloaded
      //      //  item.ThumbnailImage = localThumb;
      //      //  item.NotifyPropertyChanged("ThumbnailImage");
      //      //}
      //    }
      //    items.SafeDispose();
      //    items = null;
      //    #endregion
      //    return;
      //  })
      //  {
      //    IsBackground = true,
      //    Name = "MyFilms Image Detector and Downloader" + i,
      //    Priority = ThreadPriority.BelowNormal
      //  }.Start(groupList);
      //}
    }

    private string[] SetViewThumbs(string WStrSort, string itemlabel, string strThumbDirectory, bool isPerson, MFview.ViewRow currentCustomView, string DefaultViewImage, bool reversenames)
    {
      if (isPerson && reversenames) itemlabel = ReReverseName(itemlabel);

      string[] thumbimages = new string[2];
      thumbimages[0] = string.Empty; // ThumbnailImage
      thumbimages[1] = string.Empty; //IconImage
      string strThumb = strThumbDirectory + itemlabel + ".png";
      string strThumbSource = string.Empty;

      if (isPerson)
      #region thumbs for person views
      {
        //strThumbSource = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieActors, itemlabel); // check for actors images in MyVideos...
        //LogMyFilms.Debug("Artist thumbs - GetCoverName(Thumbs, MovieActors) - strThumb = '" + strThumb + "'");
        //if (!System.IO.File.Exists(strThumb))
        //  strThumb = strThumbDirectory + itemlabel + ".png";
        //else
        //  return strThumb;

        if (File.Exists(strThumb)) // If there is thumbs in cache folder ...
        {
          thumbimages[0] = strThumb;
          thumbimages[1] = strThumbDirectory + itemlabel + "_s.png";
          return thumbimages;
        }
        if (conf.StrPathArtist.Length > 0)
        {
          string strPathArtist;
          if (conf.StrPathArtist.Substring(conf.StrPathArtist.Length - 1) == "\\") strPathArtist = conf.StrPathArtist;
          else strPathArtist = conf.StrPathArtist + "\\";
          if (System.IO.File.Exists(strPathArtist + itemlabel + "\\folder.jpg")) strThumbSource = strPathArtist + itemlabel + "\\folder.jpg";
          else if (System.IO.File.Exists(strPathArtist + itemlabel + "\\folder.png")) strThumbSource = strPathArtist + itemlabel + "\\folder.png";
          else if (System.IO.File.Exists(strPathArtist + itemlabel + "L" + ".jpg")) strThumbSource = strPathArtist + itemlabel + "L" + ".jpg";
          else if (System.IO.File.Exists(strPathArtist + itemlabel + ".jpg")) strThumbSource = strPathArtist + itemlabel + ".jpg";
          else if (System.IO.File.Exists(strPathArtist + itemlabel + ".png")) strThumbSource = strPathArtist + itemlabel + ".png";
          else
            #region perform catalog specific searches ...
            switch (conf.StrFileType)
            {
              case Configuration.CatalogType.AntMovieCatalog3:
                break;
              case Configuration.CatalogType.AntMovieCatalog4Xtended:
                break;
              case Configuration.CatalogType.DVDProfiler:
                break;
              case Configuration.CatalogType.MovieCollector:
                break;
              case Configuration.CatalogType.MyMovies:
                break;
              case Configuration.CatalogType.EaxMovieCatalog2:
              case Configuration.CatalogType.EaxMovieCatalog3:
                if (System.IO.File.Exists(strPathArtist + itemlabel.Replace(" ", ".") + ".jpg")) strThumbSource = strPathArtist + itemlabel.Replace(" ", ".") + ".jpg";
                break;
              case Configuration.CatalogType.PersonalVideoDatabase: // PVD artist thumbs: e.g. Natalie Portman_1.jpg , then Natalie Portman_2.jpg 
                if (!string.IsNullOrEmpty(conf.StrPathArtist)) //Search matching files in PVD picture directory
                {
                  string searchname = HTMLParser.removeHtml(itemlabel); // replaces special character "á" and other special chars !
                  searchname = searchname + "*.jpg";
                  string[] files = Directory.GetFiles(conf.StrPathArtist, searchname, SearchOption.TopDirectoryOnly);
                  if (files.Any())
                    strThumbSource = files[0];
                }
                break;
              case Configuration.CatalogType.eXtremeMovieManager: // XMM artist thumbs: e.g. Alex-Revan_101640.jpg
                if (!string.IsNullOrEmpty(conf.StrPathArtist)) //Search matching files in XMM picture directory
                {
                  string searchname = HTMLParser.removeHtml(itemlabel); // replaces special character "á" and other special chars !
                  searchname = searchname.Replace(" ", "-");
                  searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
                  string[] files = Directory.GetFiles(conf.StrPathArtist, searchname, SearchOption.TopDirectoryOnly);
                  if (files.Any())
                    strThumbSource = files[0];
                }
                break;
              case Configuration.CatalogType.XBMC:
                break;
              case Configuration.CatalogType.MovingPicturesXML:
                break;
              case Configuration.CatalogType.XBMCnfoReader:
                break;
              default:
                break;
            }
            #endregion

          //if (strThumbSource == string.Empty)
          //{
          //  strThumbSource = MyFilmsDetail.CreateOrUpdateActor(itemlabel);
          //}

          if (strThumbSource != string.Empty)
          {
            //Picture.CreateThumbnail(strThumbSource, strThumbDirectory + itemlabel + "_s.png", 100, 150, 0, Thumbs.SpeedThumbsSmall);
            createCacheThumb(strThumbSource, strThumbDirectory + itemlabel + "_s.png", 100, 150, "small");
            //Picture.CreateThumbnail(strThumbSource, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);
            createCacheThumb(strThumbSource, strThumb, cacheThumbWith, cacheThumbHeight, "large");
            //thumbimages[0] = strThumbSource;
            //thumbimages[1] = strThumbDirectory + itemlabel + "_s.png";
            //return thumbimages;

            if (File.Exists(strThumb)) // (re)check if thumbs exist...
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

        // Check, if default person cover is present
        if (conf.StrArtistDflt)
        {
          string strPathArtist;
          if (conf.StrPathArtist.Substring(conf.StrPathArtist.Length - 1) == "\\") strPathArtist = conf.StrPathArtist;
          else strPathArtist = conf.StrPathArtist + "\\";

          //ImageFast.CreateImage(strThumb, item.Label); // this is to create a pseudo cover with name of label added to it
          //Picture.CreateThumbnail(conf.DefaultCoverArtist, strThumbDirectory + itemlabel + "_s.png", 100, 150, 0, Thumbs.SpeedThumbsSmall);
          //Picture.CreateThumbnail(conf.DefaultCoverArtist, strThumb, cacheThumbWith, cacheThumbHeight, 0, Thumbs.SpeedThumbsLarge);

          // if there is a Default.jpg in the view subfolder
          if (DefaultViewImage != null)
          {
            thumbimages[0] = DefaultViewImage;
            thumbimages[1] = DefaultViewImage;
            return thumbimages;
          }

          // if there is an image defined in Custom View
          if (currentCustomView != null)
          {
            thumbimages[0] = currentCustomView.ImagePath;
            thumbimages[1] = currentCustomView.ImagePath;
            return thumbimages;
          }

          // Otherwise use Default image
          if (conf.DefaultCoverArtist.Length > 0)
          {
            thumbimages[0] = conf.DefaultCoverArtist;
            thumbimages[1] = conf.DefaultCoverArtist;
            return thumbimages;
          }
          thumbimages[0] = "";
          thumbimages[1] = "";
          return thumbimages;
        }
        else
        {
          thumbimages[0] = "";
          thumbimages[1] = "";
          return thumbimages;
        }
      }
      #endregion
      else if (MyFilms.conf.StrViewsDfltAll || IsCategoryYearCountryField(WStrSort))
      {
        if (File.Exists(strThumb)) // If there is thumbs in cache folder ...
        {
          thumbimages[0] = strThumb;
          thumbimages[1] = strThumb;
          return thumbimages;
        }

        if (conf.StrPathViews.Length > 0)
        {
          string strPathViews = (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\") ? conf.StrPathViews : (conf.StrPathViews + "\\");
          string strPathViewsRoot = strPathViews;
          strPathViews = strPathViews + WStrSort.ToLower() + "\\"; // added view subfolder to searchpath
          if (File.Exists(strPathViews + itemlabel + ".jpg"))
            createCacheThumb(strPathViews + itemlabel + ".jpg", strThumb, cacheThumbWith, cacheThumbHeight, "large");
          else if (File.Exists(strPathViews + itemlabel + ".png"))
            createCacheThumb(strPathViews + itemlabel + ".png", strThumb, cacheThumbWith, cacheThumbHeight, "large");
          if (File.Exists(strThumb))
          {
            thumbimages[0] = strThumb;
            thumbimages[1] = strThumb;
            return thumbimages;
          }

          // Check, if default group cover is present
          if (MyFilms.conf.StrViewsDflt)
          {
            // if there is a Default.jpg in the view subfolder
            if (DefaultViewImage != null)
            {
              thumbimages[0] = DefaultViewImage;
              thumbimages[1] = DefaultViewImage;
              return thumbimages;
            }
            // if there is an image defined in Custom View
            if (currentCustomView != null)
            {
              thumbimages[0] = currentCustomView.ImagePath;
              thumbimages[1] = currentCustomView.ImagePath;
              return thumbimages;
            }

            // Otherwise use Default image
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
      var bmp = new System.Drawing.Bitmap(ThumbSource);
      LogMyFilms.Debug("(SetViewThumb) - Image Width x Height = '" + bmp.Width + "x" + bmp.Height + "' (" + ThumbSource + ")");
      if (bmp.Width < ThumbWidth && bmp.Height < ThumbHeight)
      {
        if (!SaveThumbnailFile(ThumbSource, ThumbTarget)) // if copy unsuccessful, try to create speedthumb
        {
          Picture.CreateThumbnail(ThumbSource, ThumbTarget, ThumbWidth, ThumbHeight, 0, SpeedThumbSize == "small" ? Thumbs.SpeedThumbsSmall : Thumbs.SpeedThumbsLarge);
        }
      }
      else
        if (SpeedThumbSize == "small")
          Picture.CreateThumbnail(ThumbSource, ThumbTarget, ThumbWidth, ThumbHeight, 0, Thumbs.SpeedThumbsSmall);
        else
          Picture.CreateThumbnail(ThumbSource, ThumbTarget, ThumbWidth, ThumbHeight, 0, Thumbs.SpeedThumbsLarge);
      bmp.SafeDispose();
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
        Thread.Sleep(MediaPortal.Player.g_Player.Playing ? 100 : 1);
        return true;
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("(SaveThumbnailFile) - Error saving new thumbnail {0} - {1}", ThumbTarget, ex.Message);
        return false;
      }
    }

    #region Sorter and Comparer Classes
    //----------------------------------------------------------------------------------------------
    //  Normal Sort
    //----------------------------------------------------------------------------------------------
    static int NormalCompare(object x, object y)
    {
      return string.Compare(x.ToString(), y.ToString(), StringComparison.OrdinalIgnoreCase); // StringComparison.CurrentCultureIgnoreCase
    }

    //----------------------------------------------------------------------------------------------
    //  Forward Sort
    //----------------------------------------------------------------------------------------------
    public class myForwarderClass : IComparer
    {
      // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
      int IComparer.Compare(Object x, Object y)
      {
        //CompareOptions compareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth | CompareOptions.IgnoreNonSpace;
        //string.Compare(strA, strB, CultureInfo.CurrentCulture, compareOptions) == 0
        //int i = String.Compare(x.ToString(), y.ToString(), StringComparison.OrdinalIgnoreCase);
        //if (i == 0) 
        //  return String.CompareOrdinal(x.ToString(), y.ToString());
        //else
        //  return i;  // return ((new CaseInsensitiveComparer(CultureInfo.InvariantCulture)).Compare(x, y));
        return String.Compare(x.ToString(), y.ToString(), StringComparison.OrdinalIgnoreCase);
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
        return String.Compare(y.ToString(), x.ToString(), StringComparison.OrdinalIgnoreCase);  // return String.CompareOrdinal(y.ToString(), x.ToString());  //return ((new CaseInsensitiveComparer()).Compare(y, x));
      }
    }

    static int CompareCount(KeyValuePair<string, int> a, KeyValuePair<string, int> b)
    {
      if (a.Value == b.Value) return (a.Key.CompareTo(b.Key));
      return (a.Value.CompareTo(b.Value));
    }

    static int CompareCountDescending(KeyValuePair<string, int> a, KeyValuePair<string, int> b)
    {
      if (b.Value == a.Value) return (b.Key.CompareTo(a.Key)); // alphabetical here ...
      return (b.Value.CompareTo(a.Value));
    }

    //----------------------------------------------------------------------------------------------
    //  GUIListItem Sort
    //----------------------------------------------------------------------------------------------
    public class GUIListLabelComparer : IComparer<GUIListItem>
    {
      public int Compare(GUIListItem x, GUIListItem y)
      {
        return x.Label.CompareTo(y.Label);
      }
    }

    public class GUIListLabel2Comparer : IComparer<GUIListItem>
    {
      public int Compare(GUIListItem x, GUIListItem y)
      {
        return x.Label2.CompareTo(y.Label2);
      }
    }

    private class MyDataRowComparer : IComparer<DataRow>
    {
      public int Compare(DataRow x, DataRow y)
      {
        return String.Compare(x.Field<string>("c1"), y.Field<string>("c1"));
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Sort for translated DB field lists
    //----------------------------------------------------------------------------------------------
    public class myDBfieldComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        int result;
        string[] s1 = x as string[];
        if (s1 == null) return 0;
        string[] s2 = y as string[];
        if (s2 == null) return 0;
        try
        {
          result = String.Compare(s1[1], s2[1], true);
        }
        catch
        {
          result = String.Compare(s1[0], s2[0], true);
        }
        return result;
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Date Sort for string types
    //----------------------------------------------------------------------------------------------
    public class myDateComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        int result;
        string s1 = x as string;
        string s2 = y as string;

        //if (s1 == null) return 0;
        //if (s2 == null) return 0;

        if (s1 == null)
          return (s2 == null) ? 0 : 1;
        if (s2 == null)
          return -1;

        try
        {
          result = Convert.ToDateTime(s1).CompareTo(Convert.ToDateTime(s2));
          // result = DateTime.Parse(s1).CompareTo(DateTime.Parse(s2));
        }
        catch // compare as strings, if no conversion possible ...
        {
          result = s1.CompareTo(s2);
        }
        return result;
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Reverse Date Sort for string types
    //----------------------------------------------------------------------------------------------
    public class myDateReverseComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        int result;
        string s1 = x as string;
        string s2 = y as string;

        //if (s1 == null) return 0;
        //if (s2 == null) return 0;

        if (s1 == null)
          return (s2 == null) ? 0 : 1;
        if (s2 == null)
          return -1;

        try
        {
          result = Convert.ToDateTime(s2).CompareTo(Convert.ToDateTime(s1));
          // result = DateTime.Parse(s1).CompareTo(DateTime.Parse(s2));
        }
        catch // compare as strings, if no conversion possible ...
        {
          result = s2.CompareTo(s1);
        }
        return result;
        // return (new myDateComparer()).Compare(y, x);
      }
    }

    public sealed class NullsLastComparer<T> : Comparer<T>
    {
      private readonly IComparer<T> _comparer;

      public NullsLastComparer() : this(null) { }

      public NullsLastComparer(IComparer<T> comparer)
      {
        _comparer = comparer ?? Comparer<T>.Default;
      }

      public override int Compare(T x, T y)
      {
        if (x == null)
          return (y == null) ? 0 : 1;

        if (y == null)
          return -1;

        return _comparer.Compare(x, y);
      }
    }

    ////----------------------------------------------------------------------------------------------
    ////  Rating Sort for decimal types
    ////----------------------------------------------------------------------------------------------
    //public class myRatingComparer : IComparer
    //{
    //  public int Compare(object x, object y)
    //  {
    //    int result;
    //    decimal s1 = x as Decimal;
    //    int s2 = y as string;

    //    if (s1 == null) return 0;
    //    if (s2 == null) return 0;

    //    //if (s1 == null)
    //    //  return (s2 == null) ? 0 : 1;
    //    //if (s2 == null)
    //    //  return -1;

    //    try
    //    {
    //      result = Convert.ToDateTime(s1).CompareTo(Convert.ToDateTime(s2));
    //      // result = DateTime.Parse(s1).CompareTo(DateTime.Parse(s2));
    //    }
    //    catch // compare as strings, if no conversion possible ...
    //    {
    //      // result = s1.CompareTo(s2);
    //      result = 0;
    //    }
    //    return result;
    //  }
    //}

    public class myRatingComparer : IComparer
    {

      // int IComparer.Compare(string a, string b)
      public int Compare(object a, object b)
      {
        decimal aDec;
        decimal bDec;
        if (decimal.TryParse(a.ToString(), out aDec) && decimal.TryParse(b.ToString(), out bDec))
        {
          return aDec.CompareTo(bDec);
        }
        else
        {
          // return a.ToString().CompareTo(b.ToString());
          return (new AlphanumComparatorFast()).Compare(a, b);
        }
      }

      //public static IComparer NumericStringSorter()
      //{
      //  return (IComparer)new myRatingComparer();
      //}
    }

    public class myRatingReverseComparer : IComparer
    {

      // int IComparer.Compare(string a, string b)
      public int Compare(object a, object b)
      {
        decimal aDec;
        decimal bDec;
        if (decimal.TryParse(b.ToString(), out aDec) && decimal.TryParse(a.ToString(), out bDec))
        {
          return bDec.CompareTo(aDec);
        }
        else
        {
          // return a.ToString().CompareTo(b.ToString());
          return (new AlphanumComparatorFast()).Compare(b, a);
        }
      }

      //public static IComparer NumericStringSorter()
      //{
      //  return (IComparer)new myRatingComparer();
      //}
    }

    //----------------------------------------------------------------------------------------------
    //  Date Sort for string types based on non DateTime Conversion
    //----------------------------------------------------------------------------------------------
    public class myDateStringComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        var s1 = x as string;
        if (s1 == null) return 0;
        var s2 = y as string;
        if (s2 == null) return 0;

        string date1 = string.Format("{0:dd/MM/yyyy}", s1);
        string date2 = string.Format("{0:dd/MM/yyyy}", s2);
        return date1.CompareTo(date2);
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Reverse Alphanumeric Sort
    //----------------------------------------------------------------------------------------------
    public class myoldReverserAlphanumComparatorFast : IComparer // Calls AlphanumComparatorFast.Compare with the parameters reversed.
    {
      int IComparer.Compare(Object x, Object y)
      {
        return (new AlphanumComparatorFast()).Compare(y, x);
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Alphanumeric Sort
    //----------------------------------------------------------------------------------------------
    public class AlphanumComparatorFast : IComparer
    {
      public int Compare(object x, object y)
      {
        //if (x == null)
        //  return (y == null) ? 0 : 1;
        //if (y == null)
        //  return -1;

        string s1 = x as string;
        string s2 = y as string;

        if (s1 == null)
          return (s2 == null) ? 0 : 1;
        if (s2 == null)
          return -1;

        //if (s1 == null) return 0;
        //if (s2 == null) return 0;

        int len1 = s1.Length;
        int len2 = s2.Length;
        int marker1 = 0;
        int marker2 = 0;

        // Walk through two the strings with two markers.
        while (marker1 < len1 && marker2 < len2)
        {
          char ch1 = s1[marker1];
          char ch2 = s2[marker2];

          // Some buffers we can build up characters in for each chunk.
          char[] space1 = new char[len1];
          int loc1 = 0;
          char[] space2 = new char[len2];
          int loc2 = 0;

          // Walk through all following characters that are digits or
          // characters in BOTH strings starting at the appropriate marker.
          // Collect char arrays.
          do
          {
            space1[loc1++] = ch1;
            marker1++;

            if (marker1 < len1)
            {
              ch1 = s1[marker1];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

          do
          {
            space2[loc2++] = ch2;
            marker2++;

            if (marker2 < len2)
            {
              ch2 = s2[marker2];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

          // If we have collected numbers, compare them numerically.
          // Otherwise, if we have strings, compare them alphabetically.
          string str1 = new string(space1);
          string str2 = new string(space2);

          int result;

          if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
          {
            int thisNumericChunk = int.Parse(str1);
            int thatNumericChunk = int.Parse(str2);
            result = thisNumericChunk.CompareTo(thatNumericChunk);
          }
          else
          {
            result = str1.CompareTo(str2);
          }

          if (result != 0)
          {
            return result;
          }
        }
        return len1 - len2;
      }
    }

    //----------------------------------------------------------------------------------------------
    //  Alphanumeric Sort
    //----------------------------------------------------------------------------------------------
    public class myReverserAlphanumComparatorFast : IComparer
    {
      public int Compare(object x, object y)
      {
        //if (x == null)
        //  return (y == null) ? 0 : 1;
        //if (y == null)
        //  return -1;

        string s1 = y as string;
        string s2 = x as string;

        if (s2 == null)
          return (s1 == null) ? 0 : 1;
        if (s1 == null)
          return -1;

        //if (s1 == null) return 0;
        //if (s2 == null) return 0;

        int len1 = s1.Length;
        int len2 = s2.Length;
        int marker1 = 0;
        int marker2 = 0;

        // Walk through two the strings with two markers.
        while (marker1 < len1 && marker2 < len2)
        {
          char ch1 = s1[marker1];
          char ch2 = s2[marker2];

          // Some buffers we can build up characters in for each chunk.
          char[] space1 = new char[len1];
          int loc1 = 0;
          char[] space2 = new char[len2];
          int loc2 = 0;

          // Walk through all following characters that are digits or
          // characters in BOTH strings starting at the appropriate marker.
          // Collect char arrays.
          do
          {
            space1[loc1++] = ch1;
            marker1++;

            if (marker1 < len1)
            {
              ch1 = s1[marker1];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

          do
          {
            space2[loc2++] = ch2;
            marker2++;

            if (marker2 < len2)
            {
              ch2 = s2[marker2];
            }
            else
            {
              break;
            }
          } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

          // If we have collected numbers, compare them numerically.
          // Otherwise, if we have strings, compare them alphabetically.
          string str1 = new string(space1);
          string str2 = new string(space2);

          int result;

          if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
          {
            int thisNumericChunk = int.Parse(str1);
            int thatNumericChunk = int.Parse(str2);
            result = thisNumericChunk.CompareTo(thatNumericChunk);
          }
          else
          {
            result = str1.CompareTo(str2);
          }

          if (result != 0)
          {
            return result;
          }
        }
        return len1 - len2;
      }
    }
    #endregion

    private static void Load_Config(string CurrentConfig, bool create_temp, LoadParameterInfo loadParams)
    {
      Stopwatch watch = new Stopwatch(); watch.Reset(); watch.Start();
      string oldXmlDB = (conf != null) ? conf.StrFileXml : "";
      conf = new Configuration(CurrentConfig, true, create_temp, loadParams);
      conf.IsDbReloadRequired = (oldXmlDB != conf.StrFileXml); // set reload flag, if the underlying DB has changed (it might not, if user switches config using the same DB)
      watch.Stop();
      LogMyFilms.Debug("Load_Config(): Finished loading config '" + CurrentConfig + "' (" + (watch.ElapsedMilliseconds) + " ms)");

      if (conf.Boolreturn && conf.Wselectedlabel == string.Empty)
      {
        conf.Boolselect = true;
        conf.Boolreturn = false;
      }
      if (conf.StrLogos)
        confLogos = new Logos();
      MyFilmsDetail.setGUIProperty("config.currentconfig", CurrentConfig);
      if (conf.EnhancedWatchedStatusHandling)
      {
        MyFilmsDetail.setGUIProperty("user.name.value", conf.StrUserProfileName);
      }

      if (conf.StrDfltSelect.Length > 0)
      {
        string userfilter = conf.StrDfltSelect.Substring(0, conf.StrDfltSelect.LastIndexOf("AND")).Trim();
        MyFilmsDetail.setGUIProperty("config.configfilter", userfilter);
        LogMyFilms.Debug("userfilter from setup: StrDfltSelect = '" + userfilter + "'");
      }
      else
        MyFilmsDetail.clearGUIProperty("config.configfilter");

      // check, if Traktuser has to be "switched"
      if (Helper.IsTraktAvailableAndEnabledAndVersion1311)
      {
        if (conf.StrUserProfileName == Helper.GetTraktUser())
        {
          LogMyFilms.Debug("Load_Config(): Current MyFilms user '" + conf.StrUserProfileName + "' is already the active Trakt user - no switch!");
        }
        else if (Helper.GetTraktUserList().Contains(conf.StrUserProfileName))
        {
          if (Helper.ChangeTraktUser(conf.StrUserProfileName))
            LogMyFilms.Debug("Load_Config(): Successfully switched Trakt to user '" + conf.StrUserProfileName + "'");
          else
            LogMyFilms.Error("Load_Config(): An error occurred changing current Trakt user login credentials!");
        }
        else
        {
          LogMyFilms.Debug("Load_Config(): Current MyFilms user '" + conf.StrUserProfileName + "' is not a trakt user - cannot switch!");
        }
      }
      // set user online status
      MyFilmsDetail.setGUIProperty("user.onlinestatus", Helper.GetUserOnlineStatus(conf.StrUserProfileName));
    }

    private void Refreshfacade()
    {
      new Thread(delegate()
      {
        {
          MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
          Fin_Charge_Init(false, false);
          MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              // what to do after it has finished here ....
            }
            return 0;
          }, 0, 0, null);
      }) { Name = "MyFilmsBGWorker", IsBackground = true, Priority = ThreadPriority.Normal }.Start();

    }

    private void Worker_Refreshfacade()
    {
      MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
      Fin_Charge_Init(false, false);
      MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
    }

    private void Loadfacade()
    {
      new Thread(delegate()
      {
        {
          MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
          Fin_Charge_Init(false, true);
          MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          {
            // what to do after it has finished here ....
          }
          return 0;
        }, 0, 0, null);
      }) { Name = "MyFilmsBGWorker", IsBackground = true }.Start();
    }

    private void Worker_Loadfacade()
    {
      MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
      Fin_Charge_Init(false, true);
      MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
    }

    //--------------------------------------------------------------------------------------------
    //  Initial Windows load. If LoadDfltSlct = true => load default select if any
    //                           LoadDfltSlct = false => return from  MyFilmsDetail
    //--------------------------------------------------------------------------------------------
    private void Fin_Charge_Init(bool LoadDfltSlct, bool reload)
    {
      LogMyFilms.Debug("Fin_Charge_Init() called with LoadDfltSlct = '" + LoadDfltSlct + "', reload = '" + reload + "', currentViewContext = '" + conf.ViewContext + "'");
      //if (publishTimer != null) publishTimer.SafeDispose();

      // reset view
      if (conf.ViewContext == ViewContext.StartView) // if View or Movie_ID loadparams are present, it is already set to "Startview"
      {
        conf.Boolselect = false; // Groupviews = false
        conf.StrSelect = ""; // reset filter, if there is one left from before ...
        ResetGlobalFilters();
      }

      #region (re)load Dataset ...
      if (reload || (PreviousWindowId != ID_MyFilmsDetail && PreviousWindowId != ID_MyFilmsActors))
      {
        if (reload) // ReLoad MyFilms DB from disk 
        {
          currentFanartList.Clear(); // as items do change, reset fanart list ...
          conf.DbSelection = new string[] { conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens, false.ToString(), false.ToString() };
          BaseMesFilms.LoadMyFilms(conf.StrFileXml); // Will be automatically loaded, if not yet done - save time on reentering MyFilms GUI !!!
          // r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // load dataset with filters
        }
        r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // load dataset with filters
        if (reload)
        {
          #region inform Trakt ro start a sync ...
          {
            if (conf.StrFileType != Configuration.CatalogType.AntMovieCatalog3 && conf.StrFileType != Configuration.CatalogType.AntMovieCatalog4Xtended)
            {
              if (ImportComplete != null && MyFilms.conf.AllowTraktSync) // trigger sync to trakt page after importer finished
              {
                ImportComplete();
                LogMyFilms.Debug("Fin_Charge_Init(): Fired 'ImportCompleted' event to trigger sync to trakt page after initial DB import of external Catalog is finished !");
              }
            }
          }
          #endregion
        }
      }
      #endregion

      #region Configure Default Button Labels ...
      BtnSrtBy.Label = (conf.BoolCollection) ? (GUILocalizeStrings.Get(96) + ((conf.StrSortaInHierarchies == conf.StrSTitle) ? GUILocalizeStrings.Get(103) : BaseMesFilms.Translate_Column(conf.StrSortaInHierarchies))) : (GUILocalizeStrings.Get(96) + ((conf.StrSorta == conf.StrSTitle) ? GUILocalizeStrings.Get(103) : BaseMesFilms.Translate_Column(conf.StrSorta)));
      // BtnToggleGlobalWatched.Label = (MyFilms.conf.GlobalUnwatchedOnly) ? string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798628)) : string.Format(GUILocalizeStrings.Get(10798713), GUILocalizeStrings.Get(10798629));
      #endregion

      InitialStart = false; // Guzzi: Set to false after first initialization to be able to return to noninitialized View - Make sure to set true if changing DB config

      LogMyFilms.Debug("Fin_Charge_Init(): (re)load done - now do action for currentViewContext = '" + conf.ViewContext + "'");

      switch (conf.ViewContext)
      {
        case ViewContext.None:
          #region Default, if nothing is configured
          conf.StrSelect = conf.StrTitle1 + " not like ''";
          conf.Boolselect = false;
          conf.Boolindexed = false;
          conf.Boolindexedreturn = false;
          SetLabelView("all");
          Change_Layout_Action(0);
          GetFilmList(conf.StrIndex);
          #endregion
          break;

        case ViewContext.Menu:
          GetSelectFromMenuView(false); // load views into facade ...
          break;

        case ViewContext.MenuAll:
          GetSelectFromMenuView(true); // load views into facade ...
          break;

        case ViewContext.StartView:
          if (loadParamInfo != null && !string.IsNullOrEmpty(loadParamInfo.MovieID) && loadParamInfo.Config == Configuration.CurrentConfig) // movieID given in load params -> set index to selected film!
          #region If LoadParams - Check and set single movie in current config ...
          {
            LogMyFilms.Debug("Fin_Charge_Init() - LoadParams - try override loading movieid: '" + loadParamInfo.MovieID + "', play: '" + loadParamInfo.Play + "'");
            // load dataset with default filters
            r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens);
            // facade index is set in filmlist loading - only launching details necessary !
            this.Change_Layout_Action(MyFilms.conf.StrLayOut);
            if (!string.IsNullOrEmpty(loadParamInfo.MovieID)) // if load params for movieid exist, set current index to the movie detected
            {
              int index = -1;
              foreach (DataRow sr in r)
              {
                index += 1;
                // string movieNumber = sr["Number"].ToString();
                // string movieName = sr[conf.StrTitle1].ToString();
                if (loadParamInfo.MovieID == Int32.Parse(sr["Number"].ToString()).ToString())
                {
                  // bool success = int.TryParse(loadParamInfo.MovieID, out index);
                  conf.StrIndex = index;
                  conf.StrTIndex = sr[conf.StrTitle1].ToString();
                  LogMyFilms.Debug("Fin_Charge_Init(): loadParam - set movie '" + conf.StrTIndex + "' by index '" + conf.StrIndex + "'");
                }
              }
            }
            GetFilmList(conf.StrIndex);
            SetLabelView(MyFilms.conf.StrTxtView); // Reload view name from configfile...
          }
          #endregion
          else
          #region Load Default View via config or LoadParameter ...
          {
            LogMyFilms.Debug("Fin_Charge_Init() - load default view - (config or loadparameter)");
            Change_Layout_Action(MyFilms.conf.StrLayOut);
            if (!Helper.FieldIsSet(conf.StrViewDfltItem) || conf.StrViewDfltItem == GUILocalizeStrings.Get(342)) // no Defaultitem defined for defaultview or "films" -> normal movielist
            {
              conf.StrSelect = conf.StrTitle1 + " not like ''"; // was: TxtSelect.Label = conf.StrTxtSelect = "";
              conf.Boolselect = false;
              conf.Boolindexed = false;
              conf.Boolindexedreturn = false;
              conf.ViewContext = ViewContext.Movie;
              SetLabelView("all");
              GetFilmList(conf.StrIndex);
              // if (conf.StrIndex == -1 && facadeFilms.Count > 0) GUIControl.SelectItemControl(GetID, facadeFilms.GetID, 0); // make sure, selecteditem is initialized !
            }
            else // called with userdefined views - so launch them ...
            {
              #region Set and Call userdefined Views
              if (conf.StrViewDfltItem == GUILocalizeStrings.Get(1079819)) // Views Menu
              {
                GetSelectFromMenuView(conf.BoolMenuShowAll);
              }
              else if (string.IsNullOrEmpty(conf.StrViewDfltText)) // no filteritem defined for the defaultview
              {
                if (IsCategoryYearCountryField(conf.StrViewDfltItem) || conf.StrViewDfltItem == "Storage" || conf.StrViewDfltItem == "Actors" || conf.StrViewDfltItem == "RecentlyAdded")
                  Change_View_Action(conf.StrViewDfltItem);
                else
                {
                  //for (int i = 0; i < 5; i++)
                  //{
                  //  if (conf.StrViewDfltItem.ToLower() == conf.StrViewText[i].ToLower() || conf.StrViewDfltItem.ToLower() == conf.StrViewItem[i].ToLower())
                  //    Change_View_Action(string.Format("View{0}", i));
                  //}
                  // MFview.ViewRow customView = this.GetCustomViewFromViewLabel(conf.StrViewDfltItem);
                  foreach (MFview.ViewRow customView in Enumerable.Where(conf.CustomViews.View, customView => conf.StrViewDfltItem == customView.Label || conf.StrViewDfltItem == customView.DBfield))
                  {
                    Change_View_Action(customView.Label);
                  }
                }
              }
              else // filteritem IS defined for the defaultview
              {
                string wStrViewDfltItem = conf.StrViewDfltItem;
                //for (int i = 0; i < 5; i++)
                //{
                //  if (conf.StrViewDfltItem == conf.StrViewText[i])
                //  {
                //    wStrViewDfltItem = conf.StrViewItem[i];
                //    SetLabelView("View" + i);
                //    break;
                //  }
                //}
                foreach (MFview.ViewRow customView in Enumerable.Where(conf.CustomViews.View, customView => conf.StrViewDfltItem == customView.Label))
                {
                  wStrViewDfltItem = customView.DBfield;
                  SetLabelView(customView.Label);
                  break;
                }
                conf.Boolselect = true;
                conf.Boolreturn = true;
                conf.WStrSort = wStrViewDfltItem;
                string sLabel = conf.Wselectedlabel;
                if (GetColumnType(wStrViewDfltItem) != typeof(string))
                  conf.StrSelect = wStrViewDfltItem + " = '" + conf.StrViewDfltText + "'";
                else if (IsDateField(wStrViewDfltItem))
                  conf.StrSelect = wStrViewDfltItem + " like '*" + string.Format("{0:dd/MM/yyyy}", DateTime.Parse(conf.StrViewDfltText).ToShortDateString()) + "*'";
                else if (IsAlphaNumericalField(wStrViewDfltItem))
                  conf.StrSelect = wStrViewDfltItem + " like '" + conf.StrViewDfltText + "'";
                else
                  conf.StrSelect = wStrViewDfltItem + " like '*" + conf.StrViewDfltText + "*'";
                // TxtSelect.Label = conf.StrTxtSelect = "[" + conf.StrViewDfltText + "]";
                conf.StrTxtSelect = "[" + conf.StrViewDfltText + "]";

                if (wStrViewDfltItem.Length > 0) SetLabelView(wStrViewDfltItem); // replaces st with localized set - old: MyFilmsDetail.setGUIProperty("view", conf.StrViewDfltItem); // set default view config to #myfilms.view
                GetFilmList(conf.StrIndex);
              }
              #endregion
            }
          }
          #endregion
          break;

        default:
          #region Normally load or refresh the facade ...
          {
            LogMyFilms.Debug("Fin_Charge_Init() - normal load, conf.Boolselect = '" + conf.Boolselect + "'");
            if (conf.Boolselect) // Groupviews / Persons
            {
              this.Change_Layout_Action(MyFilms.conf.WStrLayOut);
              SetLabelView(MyFilms.conf.StrTxtView); // Reload view name from configfile...
              getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, false, ""); // preserve index from last time
              LogMyFilms.Debug("Fin_Charge_Init() - Boolselect = true -> StrTxtSelect = '" + MyFilms.conf.StrTxtSelect + "', StrTxtView = '" + MyFilms.conf.StrTxtView + "'");
            }
            else
            {
              this.Change_Layout_Action(MyFilms.conf.StrLayOut);
              SetLabelView(MyFilms.conf.StrTxtView); // Reload view name from configfile...
              conf.ViewContext = ViewContext.Movie;
              GetFilmList(conf.StrIndex);
              if (this.facadeFilms.Count == 0)
              {
                LogMyFilms.Error("Fin_Charge_Init(): Movie list seems empty - return!");
                return;
              }
            }
          }
          #endregion
          break;
      }

      LogMyFilms.Debug("Fin_Charge_Init: StrSelect = '" + conf.StrSelect + "', StrTxtSelect = '" + conf.StrTxtSelect + "'");

      //if (string.IsNullOrEmpty(conf.StrTxtSelect) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798622)) || conf.StrTxtSelect.StartsWith(GUILocalizeStrings.Get(10798632))) // empty or starts with "all" or "filtered" ... 
      //  SetLabelSelect("");
      //else
      //  SetLabelSelect(conf.StrTxtSelect);

      switch (conf.LastID)
      {
        case ID_MyFilmsDetail:
          GUIWindowManager.ActivateWindow(ID_MyFilmsDetail); // if last window in use was detailed one display that one again
          break;
        case ID_MyFilmsActors:
          GUIWindowManager.ActivateWindow(ID_MyFilmsActors); // if last window in use was actor one display that one again
          break;
        default:
          GUIControl.FocusControl(this.GetID, (int)Controls.CTRL_ListFilms);
          // if (facadeFilms.SelectedListItemIndex < 0 && facadeFilms.Count > 0) GUIControl.SelectItemControl(GetID, facadeFilms.GetID, 0); // make sure, selecteditem is initialized !
          //int itemIndex = facadeFilms.SelectedListItemIndex;
          //if (itemIndex > -1)
          //  GUIControl.SelectItemControl(GetID, facadeFilms.GetID, itemIndex);
          //else if (facadeFilms.Count > 0)
          //  GUIControl.SelectItemControl(GetID, facadeFilms.GetID, 0);
          break;
      }
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
        GlobalFilterStringUnwatched = CreateGlobalUnwatchedFilter();
      }
      else
      {
        GlobalFilterStringUnwatched = String.Empty;
        MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
      }

      GlobalFilterIsOnlineOnly = conf.GlobalAvailableOnly;

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
    private void Change_Layout_Action(int wLayout)
    {
      LogMyFilms.Debug("Change_Layout_Action() - change facade layout to '" + wLayout + "'");
      int itemIndex = facadeFilms.SelectedListItemIndex;
      switch (wLayout)
      {
        // MP:
        //List = 0,
        //SmallIcons = 1,
        //LargeIcons = 2,
        //Filmstrip = 3,
        //AlbumView = 4,
        //Playlist = 5,
        //CoverFlow = 6
        //MyFilms:
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.List);
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.AlbumView);
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.SmallIcons);
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.LargeIcons);
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.Filmstrip);
        //choiceLayoutMenu.Add(GUIFacadeControl.Layout.CoverFlow);

        case 0:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(101)); // "Layout: " + "List"
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.List;
          break;
        case 1:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(529));
          facadeFilms.CurrentLayout = facadeFilms.AlbumListLayout != null ? GUIFacadeControl.Layout.AlbumView : GUIFacadeControl.Layout.List;
          break;
        case 2:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(100));
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
          break;
        case 3:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(417));
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
          break;
        case 4:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(733));
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
          break;
        case 5:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(791));
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
          break;

        default:
          GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(101));
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.List;
          break;
      }
      if (itemIndex > -1) GUIControl.SelectItemControl(GetID, facadeFilms.GetID, itemIndex);
    }

    //--------------------------------------------------------------------------------------------
    //   Change View Response  (and process corresponding filter list)
    //--------------------------------------------------------------------------------------------
    private void Change_View_Action(string selectedView)
    {
      LogMyFilms.Debug("Change_View_Action called with '" + selectedView + "'");
      conf.CurrentView = selectedView;
      conf.StrSelect = ""; // reset view filter
      conf.StrViewSelect = "";
      conf.StrPersons = ""; // reset person filter
      conf.IndexedChars = 0;
      conf.Boolindexed = false;
      conf.BoolSkipViewState = false;

      MFview.ViewRow selectedCustomView = MyFilms.conf.CustomViews.View.NewViewRow();
      foreach (MFview.ViewRow customView in Enumerable.Where(MyFilms.conf.CustomViews.View, customView => selectedView == customView.Label))
      {
        selectedView = "CustomView"; // if a CustomView definition is found ...
        selectedCustomView = customView;
        break;
      }
      if (selectedView == GUILocalizeStrings.Get(1079819)) selectedView = "Menu"; // Views Menu
      switch (selectedView)
      {
        #region Special Views ...
        case "All": //  Change View All Films - this is not a "DB field", buT a synthetic view ...
          conf.ViewContext = ViewContext.Movie;
          conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
          conf.WStrSort = conf.StrSTitle;
          conf.Boolselect = false;
          conf.Boolreturn = false;
          LogMyFilms.Debug("Change_View filter - " + "StrSelect: " + conf.StrSelect + " | WStrSort: " + conf.WStrSort);
          SetLabelView("all");
          GetFilmList();
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          SetDummyControlsForFacade(conf.ViewContext);
          break;

        case "Storage":
          #region storage currently not used ...
          // Change View by "Storage":
          conf.ViewContext = ViewContext.Movie;
          conf.StrSelect = "((" + conf.StrTitle1 + " not like '') and (" + conf.StrStorage + " not like ''))";
          conf.StrTxtSelect = GUILocalizeStrings.Get(10798736);
          conf.Boolselect = false;
          conf.Boolreturn = false;
          conf.WStrSort = conf.StrSTitle;
          GetFilmList();
          SetLabelView("storage");
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion
        #endregion

        case "Menu": //  Change View to "Menu"
          conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
          conf.WStrSort = conf.StrSTitle;
          conf.Boolselect = false;
          conf.Boolreturn = false;
          GetSelectFromMenuView(conf.BoolMenuShowAll); // load views into facade ...
          break;

        case "CustomView":
          #region New CustomViews ...
          if (selectedCustomView == null) return;

          switch (selectedCustomView.DBfield)
          {
            case "Producer":
            case "Director":
            case "Writer":
            case "Actors":
            case "Borrower":
              conf.ViewContext = ViewContext.Person;
              break;
            case "":
            case "OriginalTitle":
            case "TranslatedTitle":
            case "FormattedTitle":
              conf.ViewContext = ViewContext.Movie;
              break;
            default:
              conf.ViewContext = ViewContext.Group;
              break;
          }

          conf.WStrSort = selectedCustomView.DBfield;
          conf.BoolSortCountinViews = (selectedCustomView.SortFieldViewType == "Count");
          if (conf.BoolSortCountinViews)
            conf.WStrSortSensCount = selectedCustomView.SortDirectionView;
          else
            conf.WStrSortSens = selectedCustomView.SortDirectionView;

          conf.IndexedChars = selectedCustomView.Index;
          //int iLayout = 0;
          //int.TryParse(selectedCustomView.LayoutView, out iLayout);
          //conf.WStrLayOut = iLayout;
          conf.WStrLayOut = int.Parse(selectedCustomView.LayoutView);

          if (selectedCustomView.Filter.Length > 0)
          {
            //if (conf.StrDfltSelect.Length == 0) conf.StrDfltSelect = selectedCustomView.Filter + " AND ";
            //else conf.StrDfltSelect += selectedCustomView.Filter + " AND ";
            conf.StrViewSelect = selectedCustomView.Filter + " AND ";
          }

          conf.BoolSkipViewState = (selectedCustomView.Value.Length > 0 && IsTitleField(conf.WStrSort));

          // RestoreLastView(conf.CurrentView); // restore saved settings, if there are any - might be "more" than the ones defined in Custom Views

          // now view film list, filtered list or view ...
          if (conf.IndexedChars == 0 && selectedCustomView.Value == "*") // Film list view without value filter - but only, if no indexed view is selected
          {
            conf.ViewContext = ViewContext.Movie;
            conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
            conf.WStrSort = conf.StrSTitle;
            conf.Boolselect = false;
            conf.Boolreturn = false;
            LogMyFilms.Debug("Change_View filter - " + "StrSelect: " + conf.StrSelect + " | WStrSort: " + conf.WStrSort);
            SetLabelView("all");
            GetFilmList();
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            SetDummyControlsForFacade(conf.ViewContext);
          }
          else if (conf.IndexedChars == 0 && selectedCustomView.Value.Length > 0 && !selectedCustomView.Value.Contains(",")) // Film list view with value filter
          {
            conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
            conf.Boolselect = true;
            conf.Boolreturn = false;
            conf.Wstar = "*";
            if (conf.Wstar != "*") conf.StrTxtSelect += " " + GUILocalizeStrings.Get(344) + " [*" + conf.Wstar + "*]";
            // TxtSelect.Label = conf.StrTxtSelect;
            conf.StrSelect = conf.WStrSort;

            conf.StrFilmSelect = string.Empty;
            conf.Wselectedlabel = (selectedCustomView.Value != "*") ? selectedCustomView.Value : "";
            do
            {
              if (conf.StrTitleSelect != string.Empty) conf.StrTitleSelect += conf.TitleDelim;
              conf.StrTitleSelect += selectedCustomView.Value;
            }
            while (GetFilmList() == false); //keep calling while single folders found
          }
          else
          // Indexed Views
          {
            if (conf.IndexedChars > 0) // if indexed view is enabled ...
            {
              conf.Boolindexed = true;
              conf.Boolindexedreturn = false;
            }
            string listfilter = (selectedCustomView.Value.Length == 0) ? "*" : selectedCustomView.Value;
            if (selectedCustomView.Value.Length == 0)
              getSelectFromDivxThreaded();
            else
              getSelectFromDivx(conf.StrTitle1 + " not like ''", conf.WStrSort, conf.WStrSortSens, listfilter, true, string.Empty);
          }

          SetLabelView(!string.IsNullOrEmpty(selectedCustomView.Label) ? selectedCustomView.Label : selectedCustomView.DBfield);
          GUIWindowManager.Process(); // required for the next statement to work correctly, so the skinengine has correct state for visibility and focus
          // GUIControl.FocusControl(GetID, facadeFilms.GetID);
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        #region OLD Userdefined Views ... (disabled)
        //case "View0":
        //case "View1":
        //case "View2":
        //case "View3":
        //case "View4":
        //  // specific user View
        //  int i = 0;
        //  if (selectedView == "View1") i = 1;
        //  if (selectedView == "View2") i = 2;
        //  if (selectedView == "View3") i = 3;
        //  if (selectedView == "View4") i = 4;

        //  conf.WStrSort = conf.StrViewItem[i];
        //  SetLabelView(selectedView.ToLower());
        //  conf.WStrSortSens = " ASC";
        //  if (conf.StrViewValue[i].Length > 0 && !conf.StrViewValue[i].Contains(","))
        //  {
        //    conf.StrTxtSelect = GUILocalizeStrings.Get(1079870); // "Selection"
        //    conf.Boolselect = true;
        //    conf.Wstar = "*";
        //    if (conf.Wstar != "*") conf.StrTxtSelect += " " + GUILocalizeStrings.Get(344) + " [*" + conf.Wstar + "*]";
        //    // TxtSelect.Label = conf.StrTxtSelect;
        //    MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
        //    conf.StrSelect = conf.WStrSort;

        //    conf.StrFilmSelect = string.Empty;
        //    conf.Wselectedlabel = conf.StrViewValue[i];
        //    conf.Boolreturn = true;
        //    do
        //    {
        //      if (conf.StrTitleSelect != string.Empty) conf.StrTitleSelect += conf.TitleDelim;
        //      conf.StrTitleSelect += conf.StrViewValue[i];
        //    }
        //    while (GetFilmList() == false); //keep calling while single folders found
        //  }
        //  else
        //  {
        //    if (conf.IndexedChars > 0) // if indexed view is enabled ...
        //    {
        //      conf.Boolindexed = true;
        //      conf.Boolindexedreturn = false;
        //    }
        //    string listfilter = (conf.StrViewValue[i].Length == 0) ? "*" : conf.StrViewValue[i];
        //    if (conf.StrViewValue[i].Length == 0)
        //      getSelectFromDivxThreaded();
        //    else
        //      getSelectFromDivx(conf.StrTitle1 + " not like ''", conf.WStrSort, conf.WStrSortSens, listfilter, true, string.Empty);
        //  }

        //  //if ((conf.StrViewText[i] == null) || (conf.StrViewText[i].Length == 0))
        //  //{
        //  //    MyFilmsDetail.setGUIProperty("view", conf.StrViewItem[i]);   // specific user View1
        //  //    GUIPropertyManager.SetProperty("#currentmodule", conf.StrViewItem[i]);
        //  //}
        //  //else
        //  //{
        //  //    MyFilmsDetail.setGUIProperty("view", conf.StrViewText[i]);   // specific Text for View1
        //  //    GUIPropertyManager.SetProperty("#currentmodule", conf.StrViewText[i]);
        //  //}
        //  GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
        //  break;
        #endregion

        default: // generic views ...
          #region Default Generic View Handling ...
          {
            conf.WStrSort = selectedView;
            conf.WStrSortSens = " ASC";
            SetLabelView(selectedView.ToLower());

            switch (selectedView)
            {
              case "Producer":
              case "Director":
              case "Writer":
              case "Actors":
              case "Borrower":
              case "Persons":
                conf.ViewContext = ViewContext.Person;
                break;
              case "OriginalTitle":
              case "TranslatedTitle":
              case "FormattedTitle":
                conf.ViewContext = ViewContext.Movie;
                break;
              case "Category":
              case "RecentlyAdded":
              case "Country":
                conf.ViewContext = ViewContext.Group;
                break;
              case "Year":
              case "Date":
              case "DateAdded":
                conf.ViewContext = ViewContext.Group;
                conf.WStrSortSens = " DESC";
                break;
              default:
                conf.ViewContext = ViewContext.Group;
                break;
            }

            if (conf.IndexedChars > 0) // if indexed view is enabled ...
            {
              conf.Boolindexed = true;
              conf.Boolindexedreturn = false;
            }

            RestoreLastView(conf.CurrentView);
            getSelectFromDivxThreaded(); //getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, "*", true, string.Empty);
            break;
          }
          #endregion
      }
      // SetDummyControlsForFacade(conf.ViewContext); will be set in Lst_Details later ...
      LogMyFilms.Debug("Change_View_Action(" + selectedView + ") -> End");
    }

    //--------------------------------------------------------------------------------------------
    //   Menu actions ...
    //--------------------------------------------------------------------------------------------
    private void Change_Menu_Action(string choiceView)
    {
      LogMyFilms.Debug("Change_View called with '" + choiceView + "'");
      // XmlConfig XmlConfig = new XmlConfig(); // no more used - replaced by using ... (was not compatible to cached writing in other places)
      switch (choiceView.ToLower())
      {
        case "config":
          #region Choose Database
          ChooseNewConfig();
          break;
          #endregion

        case "nasstatus": //Check and show status of NAS Servers
          #region nasstatus
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
              dlg.Add("'" + MyFilms.conf.StrNasName1 + "' - " + GUILocalizeStrings.Get(10798741));
            // srv name + " - (active)" 
            else
              dlg.Add("'" + MyFilms.conf.StrNasName1 + "' - " + GUILocalizeStrings.Get(10798742));
            // srv name + " - (offline) - start ?"
            choiceSearch.Add("NAS1");
          }

          if (MyFilms.conf.StrNasName2.Length > 0)
          {
            if (WakeOnLanManager.Ping(MyFilms.conf.StrNasName2, intTimeOut))
              dlg.Add("'" + MyFilms.conf.StrNasName2 + "' - " + GUILocalizeStrings.Get(10798741));
            // srv name + " - (active)" 
            else
              dlg.Add("'" + MyFilms.conf.StrNasName2 + "' - " + GUILocalizeStrings.Get(10798742));
            // srv name + " - (offline) - start ?"
            choiceSearch.Add("NAS2");
          }

          if (MyFilms.conf.StrNasName3.Length > 0)
          {
            if (WakeOnLanManager.Ping(MyFilms.conf.StrNasName3, intTimeOut))
              dlg.Add("'" + MyFilms.conf.StrNasName3 + "' - " + GUILocalizeStrings.Get(10798741));
            // srv name + " - (active)" 
            else
              dlg.Add("'" + MyFilms.conf.StrNasName3 + "' - " + GUILocalizeStrings.Get(10798742));
            // srv name + " - (offline) - start ?"
            choiceSearch.Add("NAS3");
          }

          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1) return;

          switch (choiceSearch[dlg.SelectedLabel])
          {
            case "BACK":
              return;

            default:

              string NasServerName;
              string NasMACAddress;
              switch (choiceSearch[dlg.SelectedLabel])
              {
                case "NAS1":
                  NasServerName = MyFilms.conf.StrNasName1;
                  NasMACAddress = MyFilms.conf.StrNasMAC1;
                  break;
                case "NAS2":
                  NasServerName = MyFilms.conf.StrNasName2;
                  NasMACAddress = MyFilms.conf.StrNasMAC2;
                  break;
                case "NAS3":
                  NasServerName = MyFilms.conf.StrNasName3;
                  NasMACAddress = MyFilms.conf.StrNasMAC3;
                  break;
                default:
                  NasServerName = String.Empty;
                  NasMACAddress = String.Empty;
                  break;
              }

              GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
              dlgOk.SetHeading(GUILocalizeStrings.Get(10798624)); // MyFilms System Information
              dlgOk.SetLine(1, string.Empty);

              if (NasMACAddress.Length > 0)
              {
                if (wakeOnLanManager.WakeupSystem(
                  wakeOnLanManager.GetHwAddrBytes(NasMACAddress), NasServerName, intTimeOut))
                {
                  dlgOk.SetLine(2, "'" + NasServerName + "' " + GUILocalizeStrings.Get(10798743));
                  //successfully started 
                }
                else
                  dlgOk.SetLine(2, "'" + NasServerName + "' " + GUILocalizeStrings.Get(10798744));
                // could not be started 
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
          #endregion

        case "globaloptions":
          #region Submenu Globaloptions
          LogMyFilms.Debug("Building (sub)menu globaloptions");

          var dlg1 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg1 == null) return;
          dlg1.Reset();
          dlg1.SetHeading(GUILocalizeStrings.Get(10798689)); // Global Options ...
          var choiceViewGlobalOptions = new List<string>();

          // Choose UserProfileName
          if (MyFilms.conf.EnhancedWatchedStatusHandling)
          {
            dlg1.Add(string.Format(GUILocalizeStrings.Get(1079840), conf.StrUserProfileName));
            choiceViewGlobalOptions.Add("userprofilename");
          }

          // Delete UserProfileName
          if (MyFilmsDetail.ExtendedStartmode("Global Settings - delete user profile"))
          {
            if (MyFilms.conf.EnhancedWatchedStatusHandling)
            {
              dlg1.Add(GUILocalizeStrings.Get(1079818));
              choiceViewGlobalOptions.Add("userprofilenamedelete");
            }
          }

          // Choose grabber script for that session
          if (MyFilms.conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798628)));
          if (!MyFilms.conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("choosescript");

          // Change grabber find trying best match option
          if (MyFilms.conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798628)));
          if (!MyFilms.conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("findbestmatch");

          if (MyFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798628)));
          if (!MyFilms.conf.AlwaysDefaultView) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079880), GUILocalizeStrings.Get(10798629)));
          choiceViewGlobalOptions.Add("alwaysdefaultview");

          dlg1.Add(string.Format(GUILocalizeStrings.Get(1079841), conf.StrViewDfltItem)); // change default view
          choiceViewGlobalOptions.Add("changedefaultview");

          if (MyFilmsDetail.ExtendedStartmode("Global Settings for default config and always default config"))
          {
            if (MyFilms.conf.AlwaysShowConfigMenu) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079920), GUILocalizeStrings.Get(10798628)));
            if (!MyFilms.conf.AlwaysShowConfigMenu) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079920), GUILocalizeStrings.Get(10798629)));
            choiceViewGlobalOptions.Add("alwaysdefaultconfig");

            dlg1.Add(string.Format(GUILocalizeStrings.Get(1079842), conf.StrViewDfltItem)); // change default start config
            choiceViewGlobalOptions.Add("changedefaultconfig");
          }

          //if (MyFilms.conf.UseListViewForGoups) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079897), GUILocalizeStrings.Get(10798628)));
          //if (!MyFilms.conf.UseListViewForGoups) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079897), GUILocalizeStrings.Get(10798629)));
          //choiceViewGlobalOptions.Add("alwayslistforgroups");

          if (MyFilms.conf.StrCheckWOLenable)
          {
            if (MyFilms.conf.StrCheckWOLuserdialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079898), GUILocalizeStrings.Get(10798628)));
            if (!MyFilms.conf.StrCheckWOLuserdialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079898), GUILocalizeStrings.Get(10798629)));
            choiceViewGlobalOptions.Add("woluserdialog");
          }

          dlg1.DoModal(GetID);
          if (dlg1.SelectedLabel == -1) return;

          LogMyFilms.Debug("Call global menu with option: '" + choiceViewGlobalOptions[dlg1.SelectedLabel].ToString() + "'");
          this.Change_Menu_Action(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
          return;
          #endregion

        case "globalupdates":
          #region Submenu GlobalUpdates
          GUIDialogMenu dlg2 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg2 == null) return;
          dlg2.Reset();
          dlg2.SetHeading(GUILocalizeStrings.Get(10798690)); // Global Updates ...
          List<string> choiceViewGlobalUpdates = new List<string>();

          dlg2.Add(GUILocalizeStrings.Get(1079850)); // Update Online status - to check availability if media files
          choiceViewGlobalUpdates.Add("isonlinecheck");

          if (MyFilms.conf.StrAMCUpd)
          {
            if (bgUpdateDB.IsBusy && bgUpdateDB.WorkerSupportsCancellation)
            {
              // stop background worker
              dlg2.Add(GUILocalizeStrings.Get(1079855)); // Stop Database Updater (active)
              choiceViewGlobalUpdates.Add("cancelupdatedb");
            }
            else
            {
              dlg2.Add(GUILocalizeStrings.Get(1079861)); // Update Database with external AMCupdater
              choiceViewGlobalUpdates.Add("updatedb");

              if (MyFilmsDetail.ExtendedStartmode("Global AMCU Custom Update")) // check if specialmode is configured for disabled features
              {
                dlg2.Add(GUILocalizeStrings.Get(1079843)); // Userdefined DB Update (AMCUpdater)
                // Search all personinfos
                choiceViewGlobalUpdates.Add("updatedbselect");
              }
            }
          }

          if (MyFilms.conf.StrFanart)
          {
            dlg2.Add(GUILocalizeStrings.Get(4514)); // Download all Fanart
            choiceViewGlobalUpdates.Add("downfanart");
          }

          if (MyFilmsDetail.ExtendedStartmode("Global Update all PersonInfos")) // check if specialmode is configured for disabled features
          {
            dlg2.Add(GUILocalizeStrings.Get(10798715)); // Load Person infos - all persons
            // Search all personinfos
            choiceViewGlobalUpdates.Add("personinfos-all");
          }

          if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
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
          this.Change_Menu_Action(choiceViewGlobalUpdates[dlg2.SelectedLabel].ToLower());
          return;
          #endregion

        case "globalmappings": // map useritems from GUI
          #region globalmappings
          GUIDialogMenu dlg3 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg3 == null) return;
          dlg3.Reset();
          dlg3.SetHeading(GUILocalizeStrings.Get(10798771)); // Display options ...
          List<string> choiceGlobalMappings = new List<string>();

          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 1 (" + MyFilms.conf.Stritem1 + "-" + MyFilms.conf.Strlabel1 + ")"); // Display Item ....
          choiceGlobalMappings.Add("useritem1");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 2 (" + MyFilms.conf.Stritem2 + "-" + MyFilms.conf.Strlabel2 + ")");
          choiceGlobalMappings.Add("useritem2");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 3 (" + MyFilms.conf.Stritem3 + "-" + MyFilms.conf.Strlabel3 + ")");
          choiceGlobalMappings.Add("useritem3");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 4 (" + MyFilms.conf.Stritem4 + "-" + MyFilms.conf.Strlabel4 + ")");
          choiceGlobalMappings.Add("useritem4");
          dlg3.Add(GUILocalizeStrings.Get(10798773) + " 5 (" + MyFilms.conf.Stritem5 + "-" + MyFilms.conf.Strlabel5 + ")");
          choiceGlobalMappings.Add("useritem5");

          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 1 (" + MyFilms.conf.StritemDetails1 + "-" + MyFilms.conf.StrlabelDetails1 + ")"); // Details Display Item ....
          choiceGlobalMappings.Add("useritemdetails1");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 2 (" + MyFilms.conf.StritemDetails2 + "-" + MyFilms.conf.StrlabelDetails2 + ")");
          choiceGlobalMappings.Add("useritemdetails2");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 3 (" + MyFilms.conf.StritemDetails3 + "-" + MyFilms.conf.StrlabelDetails3 + ")");
          choiceGlobalMappings.Add("useritemdetails3");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 4 (" + MyFilms.conf.StritemDetails4 + "-" + MyFilms.conf.StrlabelDetails4 + ")");
          choiceGlobalMappings.Add("useritemdetails4");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 5 (" + MyFilms.conf.StritemDetails5 + "-" + MyFilms.conf.StrlabelDetails5 + ")");
          choiceGlobalMappings.Add("useritemdetails5");
          dlg3.Add(GUILocalizeStrings.Get(10798820) + " 6 (" + MyFilms.conf.StritemDetails6 + "-" + MyFilms.conf.StrlabelDetails6 + ")");
          choiceGlobalMappings.Add("useritemdetails6");

          // master-, secondary-  and sorttitle
          //dlg3.Add(GUILocalizeStrings.Get(10798790) + " (" + MyFilms.conf.StrTitle1 + "-" + BaseMesFilms.Translate_Column(MyFilms.conf.StrTitle1) + ")"); // mastertitle
          dlg3.Add(
            GUILocalizeStrings.Get(10798790) + " (" + MyFilms.conf.StrTitle1 + "-" +
            BaseMesFilms.Translate_Column(MyFilms.conf.StrTitle1) + ")"); // mastertitle
          choiceGlobalMappings.Add("mastertitle");
          dlg3.Add(
            GUILocalizeStrings.Get(10798791) + " (" + MyFilms.conf.StrTitle2 + "-" +
            BaseMesFilms.Translate_Column(MyFilms.conf.StrTitle2) + ")"); // secondary title
          choiceGlobalMappings.Add("secondarytitle");
          dlg3.Add(
            GUILocalizeStrings.Get(10798792) + " (" + MyFilms.conf.StrSTitle + "-" +
            BaseMesFilms.Translate_Column(MyFilms.conf.StrSTitle) + ")"); // sort title
          choiceGlobalMappings.Add("sorttitle");

          dlg3.DoModal(GetID);
          if (dlg3.SelectedLabel == -1)
          {
            return;
          }
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
          if (selection > 10) // title fields
          {
            ArrayList DisplayItems = GetDisplayItems("titles");
            foreach (string[] displayItem in DisplayItems)
            {
              dlg3.Add(displayItem[0] + "-" + displayItem[1]);
              choiceGlobalMappings.Add(displayItem[0]);
            }
          }
          else // display item fields
          {
            ArrayList DisplayItems = GetDisplayItems("viewitems");
            foreach (string[] displayItem in DisplayItems)
            {
              dlg3.Add(displayItem[1] + " (" + displayItem[0] + ")");
              choiceGlobalMappings.Add(displayItem[0]);
            }
          }

          dlg3.DoModal(GetID);
          if (dlg3.SelectedLabel == -1) return;
          string wproperty = choiceGlobalMappings[dlg3.SelectedLabel];
          dlg3.Reset();
          choiceGlobalMappings.Clear();
          LogMyFilms.Debug("Display Options - new field: '" + wproperty + "', new Label: '" + BaseMesFilms.Translate_Column(wproperty) + "'.");
          UpdateUseritemWithValue(strUserItemSelection, wproperty);
          UpdateUserItems(); // save to currentconfig - save time for WinDeInit
          //Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeFilms.SelectedListItem.ItemId, facadeFilms.SelectedListItem.Label);
          //Load_Config(Configuration.CurrentConfig, true);
          MyFilmsDetail.Init_Detailed_DB(false); // clear properties 
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          return;
          #endregion

        case "userprofilename":
          #region choose user profile name
          Change_UserProfileName();
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "UserProfileName", MyFilms.conf.StrUserProfileName);
          using (var xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "UserProfileName", MyFilms.conf.StrUserProfileName);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Debug("UserProfileName - change to '" + conf.StrUserProfileName + "'");
          //Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeFilms.SelectedListItem.ItemId, facadeFilms.SelectedListItem.Label);
          //Load_Config(Configuration.CurrentConfig, true);
          MyFilmsDetail.Init_Detailed_DB(false); // clear properties 
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          MyFilmsDetail.setGUIProperty("user.onlinestatus", Helper.GetUserOnlineStatus(conf.StrUserProfileName));
          return;
          #endregion

        case "userprofilenamedelete":
          #region delete user profile name
          if (Delete_UserProfileName()) return;
          break;
          #endregion

        case "globalwikihelp":
          #region Wiki Help
          if (Helper.IsBrowseTheWebAvailableAndEnabled)
          {
            //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser
            //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
            const string url = "http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films";
            const string zoom = "150";

            //Load Webbrowserplugin with the URL
            LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url + "'");
            GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
            GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
            MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
            GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false);
            MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
            GUIPropertyManager.SetProperty("#btWeb.startup.link", string.Empty);
            GUIPropertyManager.SetProperty("#btWeb.link.zoom", string.Empty);
          }
          else
          {
            ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: " + MyFilmsSettings.GetRequiredMinimumVersion(MyFilmsSettings.MinimumVersion.BrowseTheWeb));
          }
          break;
          #endregion

        case "about":
          #region About Box
          string infoBackgroundProcess = string.Empty;
          infoBackgroundProcess = bgUpdateFanart.IsBusy ? "running (fanart & artwork)" : "not active";
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
          #endregion

        case "globalunwatchedfilter":
          #region Global overlayfilter for unwatched movies ...
          MyFilms.conf.GlobalUnwatchedOnly = !MyFilms.conf.GlobalUnwatchedOnly;
          if (conf.GlobalUnwatchedOnly)
          {
            GlobalFilterStringUnwatched = CreateGlobalUnwatchedFilter();
            MyFilmsDetail.setGUIProperty("globalfilter.unwatched", "true");
          }
          else
          {
            GlobalFilterStringUnwatched = String.Empty;
            MyFilmsDetail.clearGUIProperty("globalfilter.unwatched");
          }
          LogMyFilms.Info("Global filter for Unwatched Only is now set to '" + GlobalFilterStringUnwatched + "'");
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu) this.Change_Menu_Action("globaloptions");
          else Context_Menu = false;
          break;
          #endregion

        case "filterdbisonline":
          #region GlobalFilterIsOnline
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
          if (!Context_Menu) this.Change_Menu_Action("globaloptions");
          else Context_Menu = false;
          break;
          #endregion

        case "filterdbtrailer":
          #region GlobalFilterTrailersOnly
          GlobalFilterTrailersOnly = !GlobalFilterTrailersOnly;
          LogMyFilms.Info("Global filter for Trailers Only is now set to '" + GlobalFilterTrailersOnly + "'");
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
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
          if (!Context_Menu) this.Change_Menu_Action("globaloptions");
          else Context_Menu = false;
          break;
          #endregion

        case "filterdbrating":
          #region GlobalFilterMinRating
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
          LogMyFilms.Info(
            "(SetGlobalFilterString MinRating) - 'GlobalFilterStringMinRating' = '" + GlobalFilterStringMinRating + "'");
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu) this.Change_Menu_Action("globaloptions");
          else Context_Menu = false;
          break;
          #endregion

        case "filterdbsetrating":
          #region Set global value for minimum Rating to restrict movielist
          LogMyFilms.Info(
            "(FilterDbSetRating) - 'AntFilterMinRating' current setting = '" + MyFilms.conf.StrAntFilterMinRating +
            "', current decimalseparator: '" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString() +
            "'");
          MyFilmsDialogSetRating dlgRating = (MyFilmsDialogSetRating)GUIWindowManager.GetWindow(ID_MyFilmsDialogRating);
          if (MyFilms.conf.StrAntFilterMinRating.Length > 0)
          {
            decimal wrating = 0;
            wrating = Convert.ToDecimal(
              MyFilms.conf.StrAntFilterMinRating.Replace(",", "."), CultureInfo.InvariantCulture);
            dlgRating.Rating = wrating;
          }
          else dlgRating.Rating = 0;
          dlgRating.SetTitle(GUILocalizeStrings.Get(1079881));
          dlgRating.DoModal(GetID);

          MyFilms.conf.StrAntFilterMinRating = dlgRating.Rating.ToString("0.0", CultureInfo.InvariantCulture);
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntFilterMinRating", MyFilms.conf.StrAntFilterMinRating);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntFilterMinRating", MyFilms.conf.StrAntFilterMinRating);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("(FilterDbSetRating) - 'AntFilterMinRating' changed to '" + MyFilms.conf.StrAntFilterMinRating + "'");
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
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
          LogMyFilms.Info(
            "(SetGlobalFilterString) - 'GlobalFilterStringMinRating' = '" + GlobalFilterStringMinRating + "'");
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          if (!Context_Menu) this.Change_Menu_Action("globaloptions");
          else Context_Menu = false;
          break;
          #endregion

        case "isonlinecheck":
          #region Launch IsOnlineCheck in batch mode
          if (bgIsOnlineCheck.IsBusy)
          {
            ShowMessageDialog(GUILocalizeStrings.Get(1079850), GUILocalizeStrings.Get(875), GUILocalizeStrings.Get(330)); //action already launched
            break;
          }
          AsynIsOnlineCheck();
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        case "updatedb":
          #region Launch AMCUpdater in batch mode
          if (bgUpdateDB.IsBusy)
          {
            ShowMessageDialog(GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(875), GUILocalizeStrings.Get(330));
            //action already launched
            break;
          }
          if (MyFilmsDetail.GlobalLockIsActive(MyFilms.conf.StrFileXml))
          {
            ShowMessageDialog(
              GUILocalizeStrings.Get(1079861), GUILocalizeStrings.Get(1079854), GUILocalizeStrings.Get(330));
            //movie db already in use (locked)
            break;
          }
          AsynUpdateDatabase("");
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        case "updatedbselect":
          #region Launch AMCUpdater in batch mode with selection of profile - e.g. to only update values etc.
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
          string selectedprofile = "";

          GUIDialogMenu dlgprofile = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlgprofile == null) return;
          dlgprofile.Reset();
          dlgprofile.SetHeading(GUILocalizeStrings.Get(1079843)); // Userdefined DB Update (AMCUpdater)
          List<string> choiceAMCconfig = new List<string>();

          DirectoryInfo dirsInf = new DirectoryInfo(MyFilms.conf.StrAMCUpd_cnf.Substring(0, MyFilms.conf.StrAMCUpd_cnf.LastIndexOf("\\")));
          bool isMePoDataDir = (MyFilms.conf.StrAMCUpd_cnf.Substring(0, MyFilms.conf.StrAMCUpd_cnf.LastIndexOf("\\")) == Config.GetDirectoryInfo(Config.Dir.Config).ToString());
          FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();

          foreach (FileSystemInfo sfi in sfiles.Where(sfi => sfi.Extension.ToLower() == ".xml" && (sfi.Name.StartsWith("MyFilmsAMCSettings")) || !isMePoDataDir))
          {
            dlgprofile.Add(sfi.Name);
            choiceAMCconfig.Add(sfi.FullName);
            LogMyFilms.Info("Custom AMCupdater update) - Add config: '" + sfi.FullName + "'");
          }

          dlgprofile.DoModal(GetID);
          if (dlgprofile.SelectedLabel == -1) return;
          //int selection = dlgprofile.SelectedLabel;
          selectedprofile = choiceAMCconfig[dlgprofile.SelectedLabel];
          dlgprofile.Reset();
          choiceAMCconfig.Clear();

          AsynUpdateDatabase(selectedprofile);
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        case "cancelupdatedb":
          #region stop background worker for AMCupdater
          if (bgUpdateDB.IsBusy && bgUpdateDB.WorkerSupportsCancellation)
          {
            bgUpdateDB.CancelAsync();
          }
          else
          {
            ShowMessageDialog("", "AMC Updater cannot be stopped!", ""); // AMC Updater is stopping!
          }
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        case "downfanart":
          #region Launch Fanart download in batch mode
          if (bgUpdateFanart.IsBusy)
          {
            ShowMessageDialog(GUILocalizeStrings.Get(1079862), GUILocalizeStrings.Get(921), GUILocalizeStrings.Get(330));
            //action already launched
            break;
          }
          AsynUpdateFanart();
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          break;
          #endregion

        case "personinfos-all":
          // Search and update all personinfos from IMDB
          // Todo: Call Singlepersonupdate - maybe this function should also be available for single movies ? (less traffic)
          // ToDo: - first implement singlepersonupdate ...
          break;

        case "trailer-all":
          #region Launch "Search and register all trailers for all movies in DB" in batch mode
          //if (bgUpdateTrailer.IsBusy)
          //{
          //    ShowMessageDialog(GUILocalizeStrings.Get(10798694), GUILocalizeStrings.Get(921), GUILocalizeStrings.Get(330)); //action already launched
          //    break;
          //}
          //AsynUpdateTrailer();
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          //break;

          AntMovieCatalog ds = new AntMovieCatalog();
          ArrayList w_index = new ArrayList();
          int w_index_count = 0;
          string t_number_id = "";
          DataRow[] wr = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
          //Now build a list of valid movies in w_index with Number registered
          foreach (DataRow wsr in wr)
          {
            foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ColumnName.ToString() == "Number"))
            {
              t_number_id = wsr[dc.ColumnName].ToString();
              //LogMyFilms.Debug("(GlobalSearchTrailerLocal) - Movienumber stored as '" + t_number_id + "'");
            }
            foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ColumnName.ToLower() == "translatedtitle"))
            {
              w_index.Add(t_number_id);
              LogMyFilms.Debug("(GlobalSearchTrailerLocal) - Add MovieIDs to indexlist: dc: '" + dc + "' and Number(ID): '" + t_number_id + "'");
              w_index_count = w_index_count + 1;
            }
          }
          ds.Dispose();
          LogMyFilms.Debug("(GlobalSearchTrailerLocal) - Number of Records found: " + w_index_count);

          bool doExtendedSearch = false;
          GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(10798940)); // Trailer
          dlgYesNo.SetLine(2, GUILocalizeStrings.Get(10798803)); // Include extended directories ?
          dlgYesNo.DoModal(GetID);
          if ((dlgYesNo.IsConfirmed)) doExtendedSearch = true;

          dlgYesNo.Reset();
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(10798800)); // Warning: Long runtime !
          dlgYesNo.SetLine(1, GUILocalizeStrings.Get(10798801)); //should really the trailer search be started
          // dlgYesNo.SetLine(2, string.Format(GUILocalizeStrings.Get(10798802), w_index_count.ToString()))); // for <xx> movies ?
          dlgYesNo.SetLine(2, string.Format(GUILocalizeStrings.Get(10798802), wr.Length.ToString()));
          // for <xx> movies ?
          dlgYesNo.DoModal(GetID);
          if (!(dlgYesNo.IsConfirmed)) break;

          GUIDialogProgress dlgPrgrs =
            (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
          if (dlgPrgrs != null)
          {
            dlgPrgrs.Reset();
            dlgPrgrs.DisplayProgressBar = true;
            dlgPrgrs.ShowWaitCursor = true;
            dlgPrgrs.DisableCancel(true);
            dlgPrgrs.SetHeading("MyFilms Trailer Registration");
            dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
            //dlgPrgrs.SetLine(1, "Register Trailers ...");
            dlgPrgrs.Percentage = 0;
          }
          new Thread(delegate()
              {
                // MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
                for (int i = 0; i < w_index_count; i++)
                {
                  try
                  {
                    string title = wr[i][MyFilms.conf.StrTitle1].ToString();
                    LogMyFilms.Debug("(GlobalSearchTrailerLocal) - Number: '" + i + "' - Index to search: '" + w_index[i] + "'");
                    if (dlgPrgrs != null) dlgPrgrs.SetLine(1, "Register trailer for '" + title + "'");
                    if (dlgPrgrs != null) dlgPrgrs.Percentage = i * 100 / w_index_count;
                    // MyFilmsDetail.setGUIProperty("statusmessage", "Register trailer for '" + title + "'");

                    //MyFilmsDetail.SearchTrailerLocal((DataRow[])MesFilms.r, Convert.ToInt32(w_index[i]));
                    MyFilmsDetail.SearchTrailerLocal((DataRow[])MyFilms.r, Convert.ToInt32(i), doExtendedSearch);
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.Debug("(GlobalSearchTrailerLocal) - index: '" + i + "', Exception: '" + ex.Message + "', Stacktrace: '" + ex.StackTrace + "'");
                  }
                }
                // MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);

                if (dlgPrgrs != null) dlgPrgrs.Percentage = 100;
                dlgPrgrs.ShowWaitCursor = false;
                dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846));
                Thread.Sleep(50);
                dlgPrgrs.Close(); // Done...
                GUIWindowManager.SendThreadCallbackAndWait(
                  (p1, p2, data) =>
                  {
                    // this will be executed after background thread finished
                    Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    MyFilmsDetail.clearGUIProperty("statusmessage");
                    // MyFilmsDetail.ShowNotificationDialog(GUILocalizeStrings.Get(10798624), "Trailer Search finished");
                    ShowMessageDialog(GUILocalizeStrings.Get(10798624), "", GUILocalizeStrings.Get(10798695));
                    return 0;
                  },
                  0,
                  0,
                  null);
              }) { Name = "GlobalTrailerUpdate", IsBackground = true }.Start();
          return;
        // break;
          #endregion

        case "incomplete-movie-data":
          SearchIncompleteMovies();
          GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // set focus to list
          break;

        case "showemptyvaluesinviews":
          MyFilms.conf.BoolShowEmptyValuesInViews = !MyFilms.conf.BoolShowEmptyValuesInViews;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "ShowEmptyValuesInViews", MyFilms.conf.StrGrabber_ChooseScript); // disabled, as we do not keep the setting persistant in config
          LogMyFilms.Info("Grabber Option 'show empty values in views' changed to " + MyFilms.conf.BoolShowEmptyValuesInViews);
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          Refreshfacade();
          this.Change_Menu_Action("globaloptions");
          break;
        case "choosescript":
          MyFilms.conf.StrGrabber_ChooseScript = !MyFilms.conf.StrGrabber_ChooseScript;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_ChooseScript", MyFilms.conf.StrGrabber_ChooseScript);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_ChooseScript", MyFilms.conf.StrGrabber_ChooseScript);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Grabber Option 'use always that script' changed to " + MyFilms.conf.StrGrabber_ChooseScript);
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          this.Change_Menu_Action("globaloptions");
          break;
        case "findbestmatch":
          MyFilms.conf.StrGrabber_Always = !MyFilms.conf.StrGrabber_Always;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_Always", MyFilms.conf.StrGrabber_Always);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "Grabber_Always", MyFilms.conf.StrGrabber_Always);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Grabber Option 'try to find best match...' changed to " + MyFilms.conf.StrGrabber_Always.ToString());
          //GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
          this.Change_Menu_Action("globaloptions");
          break;

        //case "alwayslistforgroups":
        //  MyFilms.conf.UseListViewForGoups = !MyFilms.conf.UseListViewForGoups;
        //  XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "UseListviewForGroups", MyFilms.conf.UseListViewForGoups);
        //  LogMyFilms.Info("Update Option 'use list view for groups ...' changed to " + MyFilms.conf.UseListViewForGoups);
        //  this.Change_Menu_Action("globaloptions");
        //  break;

        case "alwaysdefaultview":
          #region alwaysdefaultview
          MyFilms.conf.AlwaysDefaultView = !MyFilms.conf.AlwaysDefaultView;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AlwaysDefaultView", MyFilms.conf.AlwaysDefaultView);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AlwaysDefaultView", MyFilms.conf.AlwaysDefaultView);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'use always default view...' changed to " + MyFilms.conf.AlwaysDefaultView.ToString());
          this.Change_Menu_Action("globaloptions");
          #endregion
          break;
        case "changedefaultview":
          #region change default view
          GUIDialogMenu dlgdef = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlgdef == null) return;
          dlgdef.Reset();

          dlgdef.SetHeading(string.Format(GUILocalizeStrings.Get(1079841), conf.StrViewDfltItem)); // change default view (current default vew)
          List<string> choiceViewDefaultItems = new List<string>();
          if (conf.StrViewDfltItem == GUILocalizeStrings.Get(1079819))
            dlgdef.Add(GUILocalizeStrings.Get(1079819) + " (*)"); // menu
          else
            dlgdef.Add(GUILocalizeStrings.Get(1079819)); // menu
          choiceViewDefaultItems.Add(GUILocalizeStrings.Get(1079819));
          foreach (MFview.ViewRow customView in conf.CustomViews.View)
          {
            if (conf.StrViewDfltItem == customView.Label)
              dlgdef.Add(customView.Label + " (*)");
            else
              dlgdef.Add(customView.Label);
            choiceViewDefaultItems.Add(customView.Label);

          }
          dlgdef.DoModal(GetID);
          if (dlgdef.SelectedLabel == -1) return;

          if (choiceViewDefaultItems[dlgdef.SelectedLabel] == GUILocalizeStrings.Get(1079819)) // Views Menu
          {
            conf.StrViewDfltItem = choiceViewDefaultItems[dlgdef.SelectedLabel];
            conf.StrViewDfltText = "";
          }
          else
            foreach (MFview.ViewRow viewRow in Enumerable.Where(conf.CustomViews.View, viewRow => choiceViewDefaultItems[dlgdef.SelectedLabel] == viewRow.Label))
            {
              conf.StrViewDfltItem = choiceViewDefaultItems[dlgdef.SelectedLabel];
              conf.StrViewDfltText = (viewRow.Value.Length > 0) ? viewRow.Value : "";
            }

          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "ViewDfltItem", MyFilms.conf.StrViewDfltItem);
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "ViewDfltText", MyFilms.conf.StrViewDfltText);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'change default view' changed to " + MyFilms.conf.StrViewDfltItem);
          Change_Menu_Action("globaloptions");
          #endregion
          break;

        case "alwaysdefaultconfig":
          MyFilms.conf.AlwaysShowConfigMenu = !MyFilms.conf.AlwaysShowConfigMenu;
          // AlwaysShowConfigMenu = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Menu_Config", false);
          // XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Menu_Config", MyFilms.conf.AlwaysShowConfigMenu);
          using (var xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", "MyFilms", "Menu_Config", MyFilms.conf.AlwaysShowConfigMenu);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'use always default config...' changed to " + MyFilms.conf.AlwaysShowConfigMenu.ToString());
          this.Change_Menu_Action("globaloptions");
          break;

        case "changedefaultconfig":
          string currentDefaultConfig = "";
          using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
          {
            currentDefaultConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", string.Empty);
          }
          var dlgcfg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlgcfg == null) return;
          dlgcfg.Reset();
          dlgcfg.SetHeading(string.Format(GUILocalizeStrings.Get(1079842), currentDefaultConfig)); // change default config (current default config)

          using (var xmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
          {
            int mesFilmsNbConfig = xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
            for (int i = 0; i < (int)mesFilmsNbConfig; i++)
            {
              dlgcfg.Add(xmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));
              if (dlgcfg.SelectedLabelText == currentDefaultConfig) dlgcfg.SelectedLabel = i;
            }

            dlgcfg.DoModal(GetID);
            if (dlgcfg.SelectedLabel == -1) return;
            if (dlgcfg.SelectedLabelText.Length > 0)
            {
              string catalog = xmlConfig.ReadXmlConfig("MyFilms", dlgcfg.SelectedLabelText, "AntCatalog", string.Empty);
              if (!File.Exists(catalog))
              {
                var dlgOk = (MediaPortal.Dialogs.GUIDialogOK)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(10798624);
                dlgOk.SetLine(1, "Cannot set this Configuration:");
                dlgOk.SetLine(2, "'" + dlgcfg.SelectedLabelText + "'");
                dlgOk.SetLine(3, "Verify your settings !");
                dlgOk.DoModal(MediaPortal.GUI.Library.GUIWindowManager.ActiveWindow);
                return;
              }
              else
              {
                currentDefaultConfig = dlgcfg.SelectedLabelText;
              }
            }
          }

          using (var xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", "MyFilms", "Default_Config", currentDefaultConfig);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'change default config' changed to " + currentDefaultConfig);
          Change_Menu_Action("globaloptions");
          break;

        case "woluserdialog":
          MyFilms.conf.StrCheckWOLuserdialog = !MyFilms.conf.StrCheckWOLuserdialog;
          // XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "WOL-Userdialog", MyFilms.conf.StrCheckWOLuserdialog);
          using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true))
          {
            xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "WOL-Userdialog", MyFilms.conf.StrCheckWOLuserdialog);
          }
          // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
          LogMyFilms.Info("Update Option 'use WOL userdialog ...' changed to " + MyFilms.conf.StrCheckWOLuserdialog);
          this.Change_Menu_Action("globaloptions");
          break;

        default: // other nonspecific views ...
          {
            LogMyFilms.Error("Change_View(" + choiceView + ") is not a known Menu Action !!!");
            break;
          }
      }
    }

    private string CreateGlobalUnwatchedFilter()
    {
      string username = conf.StrUserProfileName;
      string filter = (MyFilms.conf.EnhancedWatchedStatusHandling) ? conf.StrMultiUserStateField + " like '*" + StringExtensions.EscapeLikeValue(username) + ":0*" + "' AND " : this.GlobalFilterStringUnwatched = conf.StrWatchedField + " like '" + conf.GlobalUnwatchedOnlyValue + "' AND ";
      LogMyFilms.Debug("CreateGlobalUnwatchedFilter() - filter = '" + filter + "'");
      return filter;
    }

    private bool ChooseNewConfig()
    {
      // bool success = false;
      GUIControl.ShowControl(GetID, 34); // hide elements in skin
      currentFanartList.Clear();
      Fanartstatus(false); // disable Fanart
      string newConfig = Configuration.Choice_Config(GetID);
      newConfig = Configuration.Control_Access_Config(newConfig, GetID);
      if (string.IsNullOrEmpty(newConfig)) // if user escapes dialog or bad value leave system unchanged
      {
        GUIControl.HideControl(GetID, 34); // show elements in skin
        return false;
      }
      else
      {
        //success = true;
        currentFanartList.Clear(); // clear fanart list
        NavigationStack.Clear(); // clear navigation stack
        InitMainScreen(false); // reset all properties and values
        InitGlobalFilters(false); // reset global filters, when loading new config !
        //Change "Config":
        if (this.facadeFilms.SelectedListItem != null) Configuration.SaveConfiguration(Configuration.CurrentConfig, this.facadeFilms.SelectedListItem.ItemId, this.facadeFilms.SelectedListItem.Label);
        else Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, string.Empty);
        Configuration.CurrentConfig = newConfig;
        ClearFacade(); // facadeFilms.Clear();        facadeFilms.ListLayout.Clear();
        InitialIsOnlineScan = false; // set false, so facade does not display false media status !!!
        InitialStart = true; //Set to true to make sure initial View is initialized for new DB view

        Load_Config(newConfig, true, null);

        new Thread(delegate()
        {
          {
            //MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation); //GUIWaitCursor.Init(); GUIWaitCursor.Show();
            GUIWaitCursor.Init(); GUIWaitCursor.Show();

            if (InitialStart)
            {
              Fin_Charge_Init(true, true); //Guzzi: need to always load default view on initial start, even if always default view is disabled ...
              //Loadfacade(); // load facade threaded...

              if (conf.StrFileType != Configuration.CatalogType.AntMovieCatalog3 && conf.StrFileType != Configuration.CatalogType.AntMovieCatalog4Xtended)
              {
                if (ImportComplete != null && MyFilms.conf.AllowTraktSync) // trigger sync to trakt page after importer finished
                {
                  ImportComplete();
                  LogMyFilms.Debug("Change_View(): Fired 'ImportCompleted' event to trigger sync to trakt page after initial DB import of external Catalog is finished !");
                }
              }
            }
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

            Fanartstatus(MyFilms.conf.StrFanart);

            GUIWaitCursor.Hide();
            //MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation); //GUIWaitCursor.Hide();
            //GUIControl.ShowControl(GetID, 34);
          }
          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              // after thread finished ...
            }
            return 0;
          }, 0, 0, null);
        }) { Name = "MyFilmsOnPageLoadWorker", IsBackground = true }.Start();

      }
      return true;
    }

    private void Change_UserProfileName()
    {
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(string.Format(GUILocalizeStrings.Get(1079840), conf.StrUserProfileName)); // Choose User Profile Name ... GUILocalizeStrings.Get(1079840), conf.StrUserProfileName)
      var choiceGlobalUserProfileName = new List<string>();
      string oldUserProfileName = conf.StrUserProfileName;

      dlg.Add("<" + GUILocalizeStrings.Get(10798630) + ">"); // Add User ...
      choiceGlobalUserProfileName.Add("");

      // add Trakt user, if there is any configured:
      if (Helper.IsTraktAvailableAndEnabled)
      {
        if (Helper.IsTraktAvailableAndEnabledAndVersion1311)
        {
          List<string> userlist = Helper.GetTraktUserList();

          // Show List of users to login as
          foreach (var userlogin in userlist)
          {
            dlg.Add(userlogin + " (Trakt)");
            choiceGlobalUserProfileName.Add(userlogin);
          }
        }
        else
        {
          string currentTraktuser = Helper.GetTraktUser();
          if (!string.IsNullOrEmpty(currentTraktuser))
          {
            dlg.Add(currentTraktuser + " (Trakt)");
            choiceGlobalUserProfileName.Add(currentTraktuser);
          }
        }
      }

      // Add already existing UserProfileNames - example of string value: "Global:3|Mike:0|Sandy:1"
      foreach (var userprofilename in BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens).Select(sr => sr[conf.StrMultiUserStateField].ToString().Trim()).Select(strEnhancedWatchedValue => strEnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)).SelectMany(split1 => split1.Where(s => s.Contains(":")).Select(s => s.Substring(0, s.IndexOf(":"))).Where(userprofilename => !choiceGlobalUserProfileName.Contains(userprofilename) && userprofilename != MyFilms.GlobalUsername)))
      {
        if (userprofilename == "")
        {
          dlg.Add("<" + GUILocalizeStrings.Get(10798774) + ">");
          choiceGlobalUserProfileName.Add("<" + GUILocalizeStrings.Get(10798774) + ">");
        }
        else
        {
          dlg.Add(userprofilename);
          choiceGlobalUserProfileName.Add(userprofilename);
        }
      }
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
        return;
      string strUserProfileNameSelection = choiceGlobalUserProfileName[dlg.SelectedLabel];
      switch (strUserProfileNameSelection)
      {
        case "": // new value
          var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
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
          conf.StrUserProfileName = (strUserProfileNameSelection != ("<" + GUILocalizeStrings.Get(10798774) + ">")) ? strUserProfileNameSelection : "";
          // check, if Traktuser has to be "switched"
          if (Helper.IsTraktAvailableAndEnabledAndVersion1311)
          {
            if (conf.StrUserProfileName != Helper.GetTraktUser())
            {
              bool success = Helper.ChangeTraktUser(conf.StrUserProfileName);
              if (!success)
                LogMyFilms.Info("An error occurred changing current Trakt user login credentials!");
            }
          }
          break;
      }

      #region switch user data to dedicated fields (WatchedDate, RatingUser, userdefined 'Watched' field)
      LogMyFilms.Debug("Change_UserProfileName() - Switch user data to dedicated fields (WatchedDate, RatingUser, userdefined 'Watched' field)");
      var watch = new Stopwatch(); watch.Reset(); watch.Start();
      foreach (AntMovieCatalog.MovieRow sr in BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens))
      {
        #region sync MUS state with direct DB fields for user rating, watched and Favorite
        if (conf.EnhancedWatchedStatusHandling)
        {
          MultiUserData userData;
          if (sr["MultiUserState"] != System.Convert.DBNull) userData = new MultiUserData(sr.MultiUserState);
          else
          {
            #region if the former user was the default user, migrate it to MUS!
            if (oldUserProfileName == DefaultUsername)
            {
              #region migrate status from configured (enhanced)watched field to new MultiUserStates
              if (sr[MyFilms.conf.StrWatchedField].ToString().Contains(":"))
              {
                // old field was already multiuserdata - migrate it!
                userData = new MultiUserData(sr[conf.StrWatchedField].ToString());
                sr[MyFilms.conf.StrWatchedField] = userData.GetUserState(DefaultUsername).Watched ? "true" : MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower();
              }
              else
              {
                // old field was standard watched data - create MUS and add watched for current user
                bool tmpwatched = (!string.IsNullOrEmpty(conf.GlobalUnwatchedOnlyValue) &&
                              sr[conf.StrWatchedField].ToString().ToLower() != conf.GlobalUnwatchedOnlyValue.ToLower() &&
                              sr[conf.StrWatchedField].ToString().Length > 0);
                userData = new MultiUserData(string.Empty);
                userData.SetWatched(DefaultUsername, tmpwatched);
                if (sr["RatingUser"] != Convert.DBNull)
                  userData.SetRating(DefaultUsername, (decimal)sr["RatingUser"]);
              }
              sr[MyFilms.conf.StrMultiUserStateField] = userData.ResultValueString();
              sr["DateWatched"] = userData.GetUserState(DefaultUsername).WatchedDate;
              sr["RatingUser"] = userData.GetUserState(DefaultUsername).UserRating == MultiUserData.NoRating ? Convert.DBNull : userData.GetUserState(DefaultUsername).UserRating;
              #endregion
            }
            #endregion
            else
            {
              userData = new MultiUserData(string.Empty);
            }
          }

          var user = userData.GetUserState(conf.StrUserProfileName);
          sr["DateWatched"] = user.WatchedDate == MultiUserData.NoWatchedDate ? System.Convert.DBNull : user.WatchedDate;
          sr["RatingUser"] = user.UserRating == -1 ? System.Convert.DBNull : user.UserRating;
          if (conf.StrWatchedField.Length > 0) sr[conf.StrWatchedField] = user.Watched ? "true" : conf.GlobalUnwatchedOnlyValue;
          if (conf.StrUserProfileName.Length > 0 && sr["RatingUser"] != System.Convert.DBNull && sr.RatingUser != MultiUserData.NoRating)
          {
            sr.Favorite = sr.RatingUser > MultiUserData.FavoriteRating
                            ? MultiUserData.Add(sr.Favorite, conf.StrUserProfileName)
                            : MultiUserData.Remove(sr.Favorite, conf.StrUserProfileName);
          }
          sr["MultiUserState"] = userData.MultiUserStatesValue;
        }
        #endregion
      }
      watch.Stop();
      LogMyFilms.Debug("Change_UserProfileName() - finished updating DB fields... (" + (watch.ElapsedMilliseconds) + " ms)");
      #endregion
    }

    private bool Delete_UserProfileName()
    {
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return false;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079818)); // Delete User Profile Name
      var choiceGlobalUserProfileName = new List<string>();

      // add Trakt user, if there is any configured:
      if (Helper.IsTraktAvailableAndEnabled)
      {
        if (Helper.IsTraktAvailableAndEnabledAndVersion1311)
        {
          List<string> userlist = Helper.GetTraktUserList();

          // Show List of users to login as
          foreach (var userlogin in userlist)
          {
            if (userlogin != conf.StrUserProfileName)
            {
              dlg.Add(userlogin + " (Trakt)");
              choiceGlobalUserProfileName.Add(userlogin);
            }
          }
        }
        else
        {
          string currentTraktuser = Helper.GetTraktUser();
          if (!string.IsNullOrEmpty(currentTraktuser) && currentTraktuser != conf.StrUserProfileName)
          {
            dlg.Add(currentTraktuser + " (Trakt)");
            choiceGlobalUserProfileName.Add(currentTraktuser);
          }
        }
      }

      // Add already existing UserProfileNames - example of string value: "Global:3|Mike:0|Sandy:1"
      foreach (var userprofilename in BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens).Select(sr => sr[conf.StrMultiUserStateField].ToString().Trim()).Select(strEnhancedWatchedValue => strEnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)).SelectMany(split1 => split1.Where(s => s.Contains(":")).Select(s => s.Substring(0, s.IndexOf(":"))).Where(userprofilename => !choiceGlobalUserProfileName.Contains(userprofilename) && userprofilename != MyFilms.GlobalUsername)))
      {
        if (userprofilename != conf.StrUserProfileName)
        {
          if (userprofilename == "")
          {
            dlg.Add("<" + GUILocalizeStrings.Get(10798774) + ">");
          }
          else
          {
            dlg.Add(userprofilename);
          }
          choiceGlobalUserProfileName.Add(userprofilename);
        }
      }
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
        return false;
      string strUserProfileNameSelection = choiceGlobalUserProfileName[dlg.SelectedLabel];

      #region delete all userdata for that selected user profile name
      LogMyFilms.Debug("Delete_UserProfileName() - Delete username '" + strUserProfileNameSelection + "' from DB");
      var watch = new Stopwatch(); watch.Reset(); watch.Start();
      foreach (AntMovieCatalog.MovieRow sr in BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens))
      {
        if (conf.EnhancedWatchedStatusHandling)
        {
          MultiUserData userData;
          if (sr["MultiUserState"] != System.Convert.DBNull)
          {
            userData = new MultiUserData(sr.MultiUserState);
            userData.DeleteUser(strUserProfileNameSelection);
            sr[MyFilms.conf.StrMultiUserStateField] = userData.ResultValueString();
          }
        }
      }
      watch.Stop();
      LogMyFilms.Debug("Delete_UserProfileName() - finished updating DB fields... (" + (watch.ElapsedMilliseconds) + " ms)");
      #endregion

      return true;
    }

    private string Change_MovieGroupName(string movieTitle)
    {
      List<string> choiceMovieGroupNames = new List<string>();
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return "";
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079836) + " - " + movieTitle); // Add to box set ...
      dlg.Add("<" + GUILocalizeStrings.Get(1079838) + ">"); //     <String id="1079838">Enter new box set name ...</String> // choiceMovieGroupNames.Add("");

      foreach (DataRow sr in BaseMesFilms.ReadDataMovies("", "", conf.StrSorta, conf.StrSortSens)) // Add already existing Movie Group Names - example of string value: "24\Season 1"
      {
        string sFullTitle = sr[conf.StrTitle1].ToString();
        int DelimCnt = NewString.PosCount(conf.TitleDelim, sFullTitle, false);
        if (DelimCnt > 0)
        {
          for (int i = 1; i < DelimCnt + 1; i++)
          {
            string strMovieGroupName = NewString.NPosLeft(conf.TitleDelim, sFullTitle, i, false, false);
            if (!choiceMovieGroupNames.Contains(strMovieGroupName))
              choiceMovieGroupNames.Add(strMovieGroupName);
          }
        }
      }
      choiceMovieGroupNames.Sort();
      foreach (string choiceMovieGroupName in choiceMovieGroupNames)
      {
        dlg.Add(choiceMovieGroupName);
      }
      dlg.DoModal(GetID);

      if (dlg.SelectedLabel == -1) return "";
      if (dlg.SelectedLabel == 0) // new value
      {
        VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
        if (null == keyboard) return "";
        keyboard.Reset();
        keyboard.Text = ""; // Default string is empty
        keyboard.DoModal(GetID);
        if (keyboard.IsConfirmed && (!string.IsNullOrEmpty(keyboard.Text))) return keyboard.Text;
        else return "";
      }
      else
        return dlg.SelectedLabelText;
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
      if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll) conf.MenuSelectedID = facadeFilms.SelectedListItemIndex; // remember current facade position for Menu refresh

      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079904)); // Context options ...
      List<string> updChoice = new List<string>();
      MyFilmsDetail.Searchtitles sTitles;


      #region Menu Context ...
      if ((this.facadeFilms.SelectedListItemIndex > -1 && this.facadeFilms.SelectedListItem.IsFolder && MyFilms.conf.Boolselect) || conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll)
      {
        if (conf.ViewContext == ViewContext.Menu)
        {
          dlg.Add(GUILocalizeStrings.Get(1079827)); // Disable Menu Entry
          updChoice.Add("menudisable");

          if (MyFilms.conf.CustomViews.View.Any(viewRow => !viewRow.ViewEnabled))
          {
            dlg.Add(GUILocalizeStrings.Get(1079828)); // Enable Menu Entry
            updChoice.Add("menuenable");
          }

          dlg.Add(GUILocalizeStrings.Get(1079820)); // Delete Menu Entry
          updChoice.Add("menudelete");
          dlg.Add(GUILocalizeStrings.Get(1079821)); // Move Menu Entry Up
          updChoice.Add("menumoveup");
          dlg.Add(GUILocalizeStrings.Get(1079822)); // Move Menu Entry Down
          updChoice.Add("menumovedown");
          dlg.Add(GUILocalizeStrings.Get(1079824)); // Rename Menu Entry
          updChoice.Add("menurename");
          if (MyFilmsDetail.ExtendedStartmode("Context Menu: Edit Value and Filter via GUI")) // check if specialmode is configured for disabled features
          {
            dlg.Add(GUILocalizeStrings.Get(1079825)); // Set Film Filter Value
            updChoice.Add("menusetvalue");
            dlg.Add(GUILocalizeStrings.Get(1079826)); // Set View Filter Expression
            updChoice.Add("menusetfilter");
          }
        }
      }
      #endregion

      #region Views context
      if ((this.facadeFilms.SelectedListItemIndex > -1 && this.facadeFilms.SelectedListItem.IsFolder && MyFilms.conf.Boolselect) && conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll)
      {
        if (MyFilms.conf.BoolShowEmptyValuesInViews) dlg.Add(string.Format(GUILocalizeStrings.Get(1079871), GUILocalizeStrings.Get(10798628))); // show empty values in views
        if (!MyFilms.conf.BoolShowEmptyValuesInViews) dlg.Add(string.Format(GUILocalizeStrings.Get(1079871), GUILocalizeStrings.Get(10798629)));
        updChoice.Add("showemptyvaluesinviewscontext");

        if (MyFilms.conf.IndexedChars > 0) dlg.Add(string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798628) + "/" + MyFilms.conf.IndexedChars)); // Show only indexed items in view
        if (MyFilms.conf.IndexedChars == 0) dlg.Add(string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798629)));
        updChoice.Add("showindexedvalues");

        if (MyFilms.conf.BoolDontSplitValuesInViews) dlg.Add(string.Format(GUILocalizeStrings.Get(1079845), GUILocalizeStrings.Get(10798628))); // Don't split values
        if (!MyFilms.conf.BoolDontSplitValuesInViews) dlg.Add(string.Format(GUILocalizeStrings.Get(1079845), GUILocalizeStrings.Get(10798629)));
        updChoice.Add("dontsplitvaluesinviews");
      }
      #endregion

      #region Moviecontext
      if (this.facadeFilms.SelectedListItemIndex > -1 && !this.facadeFilms.SelectedListItem.IsFolder)
      {
        dlg.Add(GUILocalizeStrings.Get(1079866));//Search related movies by persons
        updChoice.Add("analogyperson");

        dlg.Add(GUILocalizeStrings.Get(10798614));//Search related movies by property
        updChoice.Add("analogyproperty");

        dlg.Add(GUILocalizeStrings.Get(1079879));//Search Infos to related persons (load persons in facadeview) - only available in filmlist
        updChoice.Add("moviepersonlist");

        if (this.facadeFilms.Focus && !this.facadeFilms.SelectedListItem.IsFolder) // 112 = "p", 120 = "x"
        {
          dlg.Add(GUILocalizeStrings.Get(10798709));//play movie 
          updChoice.Add("playmovie");

          if (MyFilms.conf.ExternalPlayerPath.Length > 0 && System.IO.File.Exists(MyFilms.conf.ExternalPlayerPath))
          {
            dlg.Add(GUILocalizeStrings.Get(10798500));//play movie  (external player)
            updChoice.Add("playmovieexternal");
          }
        }

        if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
        {
          string trailercount = "";
          if (string.IsNullOrEmpty(MyFilms.r[this.facadeFilms.SelectedListItem.ItemId][MyFilms.conf.StrStorageTrailer].ToString().Trim()))
            trailercount = "0";
          else
          {
            string[] split1 = MyFilms.r[this.facadeFilms.SelectedListItem.ItemId][MyFilms.conf.StrStorageTrailer].ToString().Trim().Split(new Char[] { ';' });
            trailercount = split1.Count().ToString();
            if (trailercount != "0")
            {
              dlg.Add(GUILocalizeStrings.Get(10798710) + " (" + trailercount + ")");//play trailer (<number trailers present>)
              updChoice.Add("playtrailer");
            }
          }

          // play random movies or trailers in "view context" (selected group)
          if (MyFilmsDetail.ExtendedStartmode("Context: random trailer scrobbling in views context"))
          {
            dlg.Add(GUILocalizeStrings.Get(10798980)); // play random trailers
            updChoice.Add("playrandomtrailers");
          }
        }

        if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
        {
          // set unwatched // set watched
          dlg.Add(facadeFilms.SelectedListItem.IsPlayed ? GUILocalizeStrings.Get(1079895) : GUILocalizeStrings.Get(1079894));
          updChoice.Add("togglewatchedstatus");
        }

        // Enable/disable global overlay filter could be added here for faster access ?
        // ...
        //dlg.Add(GUILocalizeStrings.Get(10798714)); // "global filters ..."
        //upd_choice[ichoice] = "globalfilters";
        //ichoice++;

        dlg.Add(GUILocalizeStrings.Get(1079889));
        updChoice.Add("movieimdbinternet");

        dlg.Add(GUILocalizeStrings.Get(1079888));
        updChoice.Add("movieimdbbilder");

        if (MyFilmsDetail.ExtendedStartmode("Context: IMDB Trailer and Pictures")) // check if specialmode is configured for disabled features
        {
          dlg.Add(GUILocalizeStrings.Get(1079887));
          updChoice.Add("movieimdbtrailer");
        }

        if (MyFilmsDetail.ExtendedStartmode("Context: IMDB Update for all persons of movie")) // check if specialmode is configured for disabled features
        {
          dlg.Add(GUILocalizeStrings.Get(1079883)); // update personinfos for all involved persons of a selected movie from IMDB and/or TMDB
          updChoice.Add("updatepersonmovie");
        }
      }
      #endregion

      #region Artistcontext
      if (this.facadeFilms.SelectedListItemIndex > -1 && this.facadeFilms.SelectedListItem.IsFolder && IsPersonField(conf.WStrSort) && (conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll))
      {
        if (MyFilmsDetail.ExtendedStartmode("Context Artist: Show Infos of person via person dialog")) // check if specialmode is configured for disabled features
        {
          dlg.Add(GUILocalizeStrings.Get(1079884)); //Show Infos of person (load persons detail dialog - MesFilmsActorDetails) - only available in personlist
          updChoice.Add("artistdetail");
        }

        dlg.Add(GUILocalizeStrings.Get(1079886)); //Show IMDB internetinfos http://www.imdb.com/name/nm0000288/
        updChoice.Add("artistimdbinternet");

        dlg.Add(GUILocalizeStrings.Get(1079885)); //Show IMDB internetinfos http://www.imdb.com/name/nm0000288/filmoyear
        updChoice.Add("artistimdbfilmlist");

        dlg.Add(GUILocalizeStrings.Get(1079891)); //Show IMDB pictures http://www.imdb.com/name/nm0000288/mediaindex
        updChoice.Add("artistimdbbilder");

        dlg.Add(GUILocalizeStrings.Get(1079890)); //Show IMDB clips http://www.imdb.com/name/nm0000288/videogallery
        updChoice.Add("artistimdbclips");

        if (MyFilmsDetail.ExtendedStartmode("Context Artist: IMDB all sort of details and updates (several entries)"))
        {
          dlg.Add(GUILocalizeStrings.Get(1079882)); // update personinfo from IMDB and create actorthumbs - optional: load mediathek for person backdrops etc.
          updChoice.Add("updateperson");

          if (conf.StrFileType == Configuration.CatalogType.AntMovieCatalog3 || conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended)
          {
            dlg.Add(GUILocalizeStrings.Get(1079899)); //Update Internet Person Details
            updChoice.Add("grabber-person");
          }
        }

        if (MyFilms.conf.BoolReverseNames) dlg.Add(string.Format(GUILocalizeStrings.Get(1079839), GUILocalizeStrings.Get(10798628))); // Reverse names
        if (!MyFilms.conf.BoolReverseNames) dlg.Add(string.Format(GUILocalizeStrings.Get(1079839), GUILocalizeStrings.Get(10798629)));
        updChoice.Add("reversenames");
      }
      #endregion

      #region Moviecontext or Views
      if (facadeFilms.SelectedListItemIndex > -1 && (conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll))
      {
        if (MyFilmsDetail.ExtendedStartmode("Context: Use Virtual Path Browsing"))
        {
          if (MyFilms.conf.BoolVirtualPathBrowsing) dlg.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798628))); // virtual path browsing
          if (!MyFilms.conf.BoolVirtualPathBrowsing) dlg.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798629)));
          updChoice.Add("virtualpathbrowsing");
        }
      }
      #endregion

      #region Moviecontext
      if (this.facadeFilms.SelectedListItemIndex > -1 && !this.facadeFilms.SelectedListItem.IsFolder)
      {
        if (MyFilms.conf.StrSuppress || MyFilms.conf.StrSuppressManual)
        {
          dlg.Add(GUILocalizeStrings.Get(1079830));
          updChoice.Add("suppress");
        }

        if (conf.UseThumbsForPersons && !string.IsNullOrEmpty(conf.StrPathArtist))
        {
          dlg.Add(GUILocalizeStrings.Get(1079900)); // Download person images (selected film)
          updChoice.Add("personimages");

          if (MyFilmsDetail.ExtendedStartmode("Dadeo test for loading person images with backgroundworker queue"))
          {
            dlg.Add(GUILocalizeStrings.Get(1079900) + " (test Dadeo)"); // Download person images (selected film)
            updChoice.Add("personimagestest");
          }
        }

        if (File.Exists(GUIGraphicsContext.Skin + @"\MyFilmsCoverManager.xml"))
        {
          dlg.Add(GUILocalizeStrings.Get(10798763)); // Cover Manager ...
          updChoice.Add("covermanager");
        }

        if (MyFilms.conf.StrFanart)
        {
          dlg.Add(GUILocalizeStrings.Get(1079862));
          updChoice.Add("fanart");
          dlg.Add(GUILocalizeStrings.Get(1079874));
          updChoice.Add("deletefanart");
        }

        if (conf.StrFileType == Configuration.CatalogType.AntMovieCatalog3 || conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended)
        {
          dlg.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
          updChoice.Add("grabber");
        }

        //    dlg.Add(GUILocalizeStrings.Get(1079892)); // Update ...
        //    upd_choice[ichoice] = "updatemenu";

        if (conf.StrFileType == Configuration.CatalogType.AntMovieCatalog3 || conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended) // add or remove movie from/to box set (hierarchy)
        {
          if (MyFilms.r[this.facadeFilms.SelectedListItem.ItemId][MyFilms.conf.StrTitle1].ToString().IndexOf(MyFilms.conf.TitleDelim) > 0)
          {
            dlg.Add(GUILocalizeStrings.Get(1079837)); // Remove from box set
            updChoice.Add("removefromcollection");
          }
          else
          {
            dlg.Add(GUILocalizeStrings.Get(1079836)); // Add to box set ...
            updChoice.Add("addtocollection");
          }
        }
      }
      #endregion

      if (conf.ViewContext != ViewContext.Menu)
      {
        dlg.Add(GUILocalizeStrings.Get(1079823)); // Add to Menu as Custom View
        updChoice.Add("menuadd");

        if (null != GetCustomViewFromViewLabel(conf.CurrentView))
        {
          dlg.Add(GUILocalizeStrings.Get(1079829)); // Save current custom view settings
          updChoice.Add("menusavecurrentsettingstoview");
        }
      }

      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1) return;
      GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);

      #region Context actions
      switch (updChoice[dlg.SelectedLabel])
      {
        case "playmovie":
          conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
          MyFilmsDetail.Launch_Movie(this.facadeFilms.SelectedListItem.ItemId, GetID, m_SearchAnimation, false);
          break;

        case "playmovieexternal":
          conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
          MyFilmsDetail.Launch_Movie(this.facadeFilms.SelectedListItem.ItemId, GetID, m_SearchAnimation, true);
          break;

        case "playtrailer":
          // first check, if trailer files are available, offer options
          //if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer) // StrDirStorTrailer only required for extended search
          if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim()))
          {
            MyFilmsDetail.trailerPlayed = true;
            MyFilmsDetail.Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
          }
          else
          {
            // ToDo: Can add autosearch&register logic here before try starting trailers

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

        case "playrandomtrailers":  // only in views and intended for "multiple movies" // dlg.Add(GUILocalizeStrings.Get(10798980)); // play random trailers
          //if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer) // StrDirStorTrailer only required for extended search
          PlayRandomTrailersInit(facadeFilms.SelectedListItem.Label, false);
          break;

        case "showemptyvaluesinviewscontext":
          {
            MyFilms.conf.BoolShowEmptyValuesInViews = !MyFilms.conf.BoolShowEmptyValuesInViews;
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'show empty values' changed to " + MyFilms.conf.BoolShowEmptyValuesInViews);
            Refreshfacade();
            break;
          }

        case "showindexedvalues":
          {
            #region show indexed values
            GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            List<string> choiceViewMenu = new List<string>();

            if (dlgmenu == null) return;
            dlgmenu.Reset();
            choiceViewMenu.Clear();
            MFview.ViewRow currentCustomView = null;
            int currentIndex = conf.IndexedChars;

            if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll) currentCustomView = GetCustomViewFromViewLabel(facadeFilms.SelectedListItem.Label);
            else currentCustomView = GetCustomViewFromViewLabel(conf.CurrentView); // Views - check, which one is active
            if (currentCustomView != null) currentIndex = currentCustomView.Index;

            string headline = (currentIndex == 0) ? (string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798629))) : (string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798628) + "/" + currentIndex));
            dlgmenu.SetHeading(headline);

            dlgmenu.Add(string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798629))); // disabled indexed view
            choiceViewMenu.Add("0");

            dlgmenu.Add(string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798628) + "/" + "1"));
            choiceViewMenu.Add("1");

            dlgmenu.Add(string.Format(GUILocalizeStrings.Get(1079844), GUILocalizeStrings.Get(10798628) + "/" + "2"));
            choiceViewMenu.Add("2");

            dlgmenu.DoModal(GetID);
            if (dlgmenu.SelectedLabel == -1)
            {
              break;
            }
            switch (choiceViewMenu[dlgmenu.SelectedLabel])
            {
              case "0":
                if (currentIndex == 0) return;
                currentIndex = 0;
                break;
              case "1":
                if (currentIndex == 1) return;
                currentIndex = 1;
                break;
              case "2":
                if (currentIndex == 2) return;
                currentIndex = 2;
                break;
              default:
                return;
            }

            //// if it is a view, save the settings for it
            //if (currentCustomView != null)
            //{
            //  currentCustomView.Index = currentIndex;
            //  LogMyFilms.Debug("Context_Menu_Movie() : Option 'show indexed values' changed for Custom View '" + currentCustomView.Label + "' to '" + currentIndex + "'");
            //  SaveCustomViews();
            //}

            // now change settings for GUI
            MyFilms.conf.IndexedChars = currentIndex;
            if (MyFilms.conf.IndexedChars > 0)
            {
              MyFilms.conf.Boolindexed = true;
              MyFilms.conf.Boolindexedreturn = false;
            }
            else
            {
              MyFilms.conf.Boolindexed = false;
              MyFilms.conf.Boolindexedreturn = false;
              MyFilms.conf.Wstar = "*";
            }
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'show indexed values' changed to '" + MyFilms.conf.IndexedChars + "'");
            Refreshfacade();
            #endregion
            break;
          }

        case "dontsplitvaluesinviews":
          {
            MyFilms.conf.BoolDontSplitValuesInViews = !MyFilms.conf.BoolDontSplitValuesInViews;
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'Don't split values' changed to " + MyFilms.conf.BoolDontSplitValuesInViews);
            Refreshfacade();
            break;
          }

        case "virtualpathbrowsing":
          {
            MyFilms.conf.BoolVirtualPathBrowsing = !MyFilms.conf.BoolVirtualPathBrowsing;
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'Use virtual Path Browsing' changed to " + MyFilms.conf.BoolVirtualPathBrowsing);
            Refreshfacade();
            break;
          }

        case "reversenames":
          {
            MyFilms.conf.BoolReverseNames = !MyFilms.conf.BoolReverseNames;
            LogMyFilms.Debug("Context_Menu_Movie() : Option 'Reverse Names' changed to '" + MyFilms.conf.BoolReverseNames + "'");
            Refreshfacade();
            break;
          }

        #region Menu - all operations

        case "menusavecurrentsettingstoview":
          {
            #region Menu - save settings
            MFview.ViewRow newRow = GetCustomViewFromViewLabel(conf.CurrentView);
            newRow.SortDirectionView = (conf.BoolSortCountinViews) ? conf.WStrSortSensCount : conf.WStrSortSens;
            newRow.SortFieldViewType = (conf.BoolSortCountinViews) ? "Count" : "Name";
            newRow.Index = conf.IndexedChars;
            newRow.LayoutView = conf.WStrLayOut.ToString();
            LogMyFilms.Debug("Context_Menu_Movie() - Update Custom View - DB Field '" + newRow.DBfield + "', Label '" + newRow.Label + "', Value '" + newRow.Value + "'");
            SaveCustomViews();
            NavigationStack.Clear();
            break;
            #endregion
          }

        case "menuadd":
          {
            #region Menu - add item
            MFview.ViewRow newRow = MyFilms.conf.CustomViews.View.NewViewRow();
            newRow.ViewEnabled = true;
            //newRow.ShowEmpty = conf.BoolShowEmptyValuesInViews;
            switch (conf.ViewContext)
            {
              case ViewContext.MenuAll:
                newRow.Label = facadeFilms.SelectedListItem.Label + " *";
                newRow.ImagePath = facadeFilms.SelectedListItem.ThumbnailImage;
                if (!System.IO.File.Exists(newRow.ImagePath)) newRow.ImagePath = conf.DefaultCoverViews;
                newRow.DBfield = facadeFilms.SelectedListItem.DVDLabel;
                newRow.SortDirectionView = " ASC";
                newRow.SortFieldViewType = "Name";
                newRow.Index = 0;
                newRow.LayoutView = "0"; // List view
                newRow.Value = "";
                newRow.Filter = "";
                break;
              case ViewContext.Movie:
              case ViewContext.MovieCollection:
              case ViewContext.Group:
              case ViewContext.Person:
                newRow.Label = BaseMesFilms.Translate_Column(conf.WStrSort) + " - " + conf.StrTxtSelect + " *";
                newRow.DBfield = conf.WStrSort;
                newRow.SortDirectionView = (conf.BoolSortCountinViews) ? conf.WStrSortSensCount : conf.WStrSortSens;
                newRow.SortFieldViewType = (conf.BoolSortCountinViews) ? "Count" : "Name";
                newRow.Index = conf.IndexedChars;
                newRow.LayoutView = conf.WStrLayOut.ToString();
                switch (conf.ViewContext)
                {
                  case ViewContext.Movie:
                    //newRow.Value = Prev_Label;
                    newRow.Value = conf.Wselectedlabel;
                    newRow.Filter = conf.StrSelect;
                    if (IsPersonField(newRow.DBfield))
                      newRow.ImagePath = (!string.IsNullOrEmpty(personcover.Filename)) ? personcover.Filename : conf.DefaultCoverArtist;
                    else
                      newRow.ImagePath = (!string.IsNullOrEmpty(viewcover.Filename)) ? viewcover.Filename : conf.DefaultCoverViews;
                    break;
                  case ViewContext.MovieCollection:
                    //newRow.Value = Prev_Label;
                    newRow.Value = conf.Wselectedlabel;
                    newRow.Filter = conf.StrSelect;
                    newRow.ImagePath = (!string.IsNullOrEmpty(groupcover.Filename)) ? groupcover.Filename : (!string.IsNullOrEmpty(filmcover.Filename)) ? filmcover.Filename : conf.DefaultCover;
                    break;
                  case ViewContext.Group:
                    newRow.Value = "";
                    newRow.Filter = conf.StrViewSelect;
                    newRow.ImagePath = conf.DefaultCoverViews;
                    break;
                  case ViewContext.Person:
                    newRow.Value = "";
                    newRow.Filter = conf.StrViewSelect;
                    newRow.ImagePath = conf.DefaultCoverArtist;
                    break;
                }
                break;
              default:
                return;
            }

            if (newRow.DBfield.Length == 0 || newRow.Label.Length == 0) return;
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            if (null == keyboard) return;
            keyboard.Reset();
            keyboard.Text = newRow.Label;
            keyboard.DoModal(GetID);
            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
            {
              newRow.Label = keyboard.Text;
              MyFilms.conf.CustomViews.View.AddViewRow(newRow);
              LogMyFilms.Debug("Context_Menu_Movie() - Add View with DB Field '" + newRow.DBfield + "', Label '" + newRow.Label + "', Value '" + newRow.Value + "'");
              SaveCustomViews();
              GetSelectFromMenuView(conf.BoolMenuShowAll);
              NavigationStack.Clear();
            }
            break;
            #endregion
          }

        case "menudisable":
          {
            #region Menu - disable item
            foreach (MFview.ViewRow viewRow in Enumerable.Where(MyFilms.conf.CustomViews.View, viewRow => this.facadeFilms.SelectedListItem.Label == viewRow.Label))
            {
              viewRow.ViewEnabled = false;
              break;
            }
            SaveCustomViews();
            GetSelectFromMenuView(conf.BoolMenuShowAll);
            NavigationStack.Clear();
            break;
            #endregion
          }

        case "menuenable":
          {
            #region Menu - enable item
            GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);

            if (dlgmenu == null) return;
            dlgmenu.Reset();
            dlgmenu.SetHeading(GUILocalizeStrings.Get(1079828)); // Enable Menu Entry

            foreach (MFview.ViewRow viewRow in Enumerable.Where(MyFilms.conf.CustomViews.View, viewRow => viewRow.ViewEnabled == false))
            {
              dlgmenu.Add(viewRow.Label);
            }

            dlgmenu.DoModal(GetID);
            if (dlgmenu.SelectedLabel == -1) break;

            foreach (MFview.ViewRow viewRow in MyFilms.conf.CustomViews.View)
            {
              if (dlgmenu.SelectedLabelText == viewRow.Label && viewRow.ViewEnabled == false)
              {
                viewRow.ViewEnabled = true;
                break;
              }
            }
            SaveCustomViews();
            GetSelectFromMenuView(conf.BoolMenuShowAll);
            NavigationStack.Clear();
            break;
            #endregion
          }

        case "menudelete":
          {
            #region Menu - delete item
            foreach (MFview.ViewRow viewRow in Enumerable.Where(MyFilms.conf.CustomViews.View, viewRow => this.facadeFilms.SelectedListItem.Label == viewRow.Label))
            {
              MyFilms.conf.CustomViews.View.RemoveViewRow(viewRow);
              break;
            }
            SaveCustomViews();
            GetSelectFromMenuView(conf.BoolMenuShowAll);
            NavigationStack.Clear();
            break;
            #endregion
          }

        case "menumoveup":
          {
            #region Menu - movie item up
            int rowIndex = -1;
            foreach (MFview.ViewRow viewRow in Enumerable.Where(MyFilms.conf.CustomViews.View, viewRow => this.facadeFilms.SelectedListItem.Label == viewRow.Label))
            {
              rowIndex = MyFilms.conf.CustomViews.View.Rows.IndexOf(viewRow);
              break;
            }
            if (rowIndex > 0)
            {
              DataRow selectedRow = MyFilms.conf.CustomViews.View.Rows[rowIndex];
              DataRow newRow = MyFilms.conf.CustomViews.View.NewRow();
              newRow.ItemArray = selectedRow.ItemArray; // copy data
              MyFilms.conf.CustomViews.View.Rows.Remove(selectedRow);
              MyFilms.conf.CustomViews.View.Rows.InsertAt(newRow, rowIndex - 1);
            }
            SaveCustomViews();
            conf.MenuSelectedID--;
            GetSelectFromMenuView(conf.BoolMenuShowAll);
            NavigationStack.Clear();
            break;
            #endregion
          }

        case "menumovedown":
          {
            #region Menu - movie item down
            int rowIndex = -1;
            foreach (MFview.ViewRow viewRow in Enumerable.Where(MyFilms.conf.CustomViews.View, viewRow => this.facadeFilms.SelectedListItem.Label == viewRow.Label))
            {
              rowIndex = MyFilms.conf.CustomViews.View.Rows.IndexOf(viewRow);
              break;
            }
            if (rowIndex < MyFilms.conf.CustomViews.View.Rows.Count - 1)
            {
              DataRow selectedRow = MyFilms.conf.CustomViews.View.Rows[rowIndex];
              DataRow newRow = MyFilms.conf.CustomViews.View.NewRow();
              newRow.ItemArray = selectedRow.ItemArray; // copy data
              MyFilms.conf.CustomViews.View.Rows.Remove(selectedRow);
              MyFilms.conf.CustomViews.View.Rows.InsertAt(newRow, rowIndex + 1);
            }
            SaveCustomViews();
            conf.MenuSelectedID++;
            GetSelectFromMenuView(conf.BoolMenuShowAll);
            NavigationStack.Clear();
            break;
            #endregion
          }

        case "menurename":
          {
            #region Menu - rename tem
            foreach (MFview.ViewRow viewRow in MyFilms.conf.CustomViews.View)
            {
              if (facadeFilms.SelectedListItem.Label == viewRow.Label)
              {
                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                if (null == keyboard) return;
                keyboard.Reset();
                keyboard.Text = viewRow.Label;
                keyboard.DoModal(GetID);
                if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                {
                  viewRow.Label = keyboard.Text;
                  SaveCustomViews();
                  GetSelectFromMenuView(conf.BoolMenuShowAll);
                  NavigationStack.Clear();
                }
                break;
              }
            }
            break;
            #endregion
          }

        case "menusetvalue":
          {
            #region Menu - set value
            foreach (MFview.ViewRow viewRow in MyFilms.conf.CustomViews.View)
            {
              if (facadeFilms.SelectedListItem.Label == viewRow.Label)
              {
                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                if (null == keyboard) return;
                keyboard.Reset();
                keyboard.Text = viewRow.Value;
                keyboard.DoModal(GetID);
                if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                {
                  viewRow.Value = keyboard.Text;
                  SaveCustomViews();
                  GetSelectFromMenuView(conf.BoolMenuShowAll);
                  NavigationStack.Clear();
                }
                break;
              }
            }
            break;
            #endregion
          }

        case "menusetfilter":
          {
            #region Menu - set filter
            foreach (MFview.ViewRow viewRow in MyFilms.conf.CustomViews.View)
            {
              if (facadeFilms.SelectedListItem.Label == viewRow.Label)
              {
                // ToDo:
                // ändern
                // löschen
                // auswählen via "field" und "wert - wert wahlweise manuell
                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                if (null == keyboard) return;
                keyboard.Reset();
                keyboard.Text = viewRow.Filter;
                keyboard.DoModal(GetID);
                if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                {
                  viewRow.Filter = keyboard.Text;
                  SaveCustomViews();
                  GetSelectFromMenuView(conf.BoolMenuShowAll);
                  NavigationStack.Clear();
                }
                break;
              }
            }
            break;
            #endregion
          }
        #endregion

        case "analogyperson":
          {
            SearchRelatedMoviesbyPersons((int)this.facadeFilms.SelectedListItem.ItemId, Context_Menu);
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            dlg.DeInit();
            break;
          }

        case "analogyproperty":
          {
            SearchRelatedMoviesbyProperties((int)this.facadeFilms.SelectedListItem.ItemId, Context_Menu);
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            dlg.DeInit();
            break;
          }

        case "movieimdbtrailer": // Example: (http://www.imdb.com/title/tt0438488/videogallery)
          {
            BrowseTheWebLauncher("movie", "videogallery", this.facadeFilms.SelectedListItem.Label);
            break;
          }

        case "movieimdbbilder": // example: http://www.imdb.com/title/tt0133093/mediaindex
          {
            BrowseTheWebLauncher("movie", "mediaindex", this.facadeFilms.SelectedListItem.Label);
            break;
          }

        case "movieimdbinternet":
          {
            BrowseTheWebLauncher("movie", "", this.facadeFilms.SelectedListItem.Label);
            break;
          }

        case "moviepersonlist":
          {
            if (!this.facadeFilms.SelectedListItem.IsFolder && !conf.Boolselect)
            {
              conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
              conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
              string persontype = SelectPersonType((int)facadeFilms.SelectedListItem.ItemId);
              if (string.IsNullOrEmpty(persontype))
                break;
              SaveListState(false);
              conf.BoolSkipViewState = false;
              conf.StrPersons = r[conf.StrIndex][persontype].ToString();
              //conf.StrFilmSelect = conf.StrTitle1 + " not like ''";
              conf.StrSelect = conf.StrTitle1 + " like '*" + StringExtensions.EscapeLikeValue(conf.StrTIndex) + "'"; // set view filter to current movie name - use "*" at the beginning to include movies with hierarchies !
              //conf.StrSelect = "";

              conf.WStrSort = persontype;
              conf.WStrSortSens = " ASC";
              SetLabelView(persontype.ToLower());
              conf.ViewContext = ViewContext.Person;
              conf.CurrentView = "MoviePersons";
              //getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, "*", true, string.Empty);
              //getSelectFromDivxThreaded();


              // this.Change_View_Action(persontype);

              getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.StrPersons, true, string.Empty);
              // getSelectFromDivx(conf.StrTitle1 + " not like ''", persontype, conf.WStrSortSens, conf.StrPersons, true, string.Empty);
              // getSelectFromDivx(conf.StrFilmSelect, conf.WStrSort, conf.WStrSortSens, conf.StrPersons, true, string.Empty);
            }
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            break;
          }

        case "artistdetail":
          LogMyFilms.Debug("artistdetail: searching infos for '" + this.facadeFilms.SelectedListItem.Label + "'");
          {
            string actorSearchname = MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(this.facadeFilms.SelectedListItem.Label);
            PersonInfo(actorSearchname);
            break;
          }

        case "artistimdbclips": // Example: http://www.imdb.com/name/nm0000206/videogallery
          {
            BrowseTheWebLauncher("person", "videogallery", this.facadeFilms.SelectedListItem.Label);
            break;
          }

        case "artistimdbbilder": // Example: http://www.imdb.com/name/nm0000206/mediaindex
          {
            BrowseTheWebLauncher("person", "mediaindex", this.facadeFilms.SelectedListItem.Label);
            break;
          }

        case "artistimdbinternet": // Example: http://www.imdb.com/name/nm0000206/
          {
            BrowseTheWebLauncher("person", "", this.facadeFilms.SelectedListItem.Label);
            break;
          }

        case "artistimdbfilmlist": // http://www.imdb.com/name/nm0000288/filmoyear
          {
            BrowseTheWebLauncher("person", "filmoyear", this.facadeFilms.SelectedListItem.Label);
            break;
          }

        case "updateperson":
          {
            #region update person info from internet
            //Todo: add calls to update the personinfos from IMDB - use database and grabberclasses from MePo / Deda
            ArtistIMDBpictures(this.facadeFilms.SelectedListItem.Label); // Call Updategrabber with Textlabel/Actorname
            GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
            dlg.DeInit();


            //First search corresponding URL for the actor ...
            // bool director = false; // Actor is director // Currently not used...
            IMDB _imdb = new IMDB();
            //IMDB.IMDBUrl wurl;
            //newGrab.FindActor(facadeFilms.SelectedListItem.Label);
            ArrayList actorList = new ArrayList();
            //if (_imdb.Count > 0)
            {
              string actor = this.facadeFilms.SelectedListItem.Label;
              //string test = _imdb[0].IMDBURL;
              _imdb.FindActor(actor);
              IMDBActor imdbActor = new IMDBActor();
              // string ttt = imdbActor.ThumbnailUrl;
              if (_imdb.Count > 0)
              {
                //int index = IMDBFetcher.FuzzyMatch(actor);
                int index;
                int matchingDistance = int.MaxValue;

                for (index = 0; index < _imdb.Count; ++index)
                {
                  int distance = Levenshtein.Match(actor, _imdb[index].Title);

                  if (distance < matchingDistance)
                  {
                    matchingDistance = distance;
                  }
                }

                //LogMyFilms.Info("Getting actor:{0}", _imdb[index].Title);
                //_imdb.GetActorDetails(_imdb[index], director, out imdbActor);

                //LogMyFilms.Info("Adding actor:{0}({1}),{2}", imdbActor.Name, actor, percent);
                //#if MP1X
                //                int actorId = MediaPortal.Video.Database.VideoDatabase.AddActor(imdbActor.Name);
                //#else
                //                int actorId = MediaPortal.Video.Database.VideoDatabase.AddActor(imdbActor.IMDBActorID, imdbActor.Name);
                //#endif
                int actorId = GUIUtils.AddActor(null, imdbActor.Name);
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
                //#if MP1X
                //                int actorId = VideoDatabase.AddActor(actor);
                //#else
                //                int actorId = VideoDatabase.AddActor(null, actor);
                //#endif
                int actorId = GUIUtils.AddActor(null, actor);
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

            MyFilmsDetail.GetActorByName(this.facadeFilms.SelectedListItem.Label, actorList);
            //MediaPortal.Video.Database.VideoDatabase.GetActorByName(facadeFilms.SelectedListItem.Label, actorList);

            if (actorList.Count == 0)
            {
              GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
              dlgOk.SetHeading("Info");
              dlgOk.SetLine(1, string.Empty);
              dlgOk.SetLine(2, "Keine Personen Infos vorhanden !");
              dlgOk.DoModal(GetID);
              return;
            }
            int actorID = 0;
            string actorname = string.Empty;
            char[] splitter = { '|' };
            foreach (string[] strActor in from string act in actorList select act.Split(splitter))
            {
              actorID = Convert.ToInt32(strActor[0]);
              actorname = strActor[1];
            }

            //MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorID);
            //MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(1);
            //if (actor != null)

            //OnVideoArtistInfoGuzzi(actor);



            IMDB GrabArtist = new IMDB();
            //IMDB.IMDBUrl wwurl;

            GrabArtist.FindActor(this.facadeFilms.SelectedListItem.Label);

            //int listCount = listUrl.Count;

            //url = new IMDBSearch();
            //MediaPortal.Video.Database.IMDB.IMDBUrl .FindActor(facadeFilms.SelectedListItem.Label));
            //Load Webbrowserplugin with the URL
            //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
            string url = ImdbBaseUrl + string.Empty;
            const string zoom = "100";
            //value = value.Replace("%link%", url);
            //value = value.Replace("%zoom%", zoom);
            LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url + "'");
            GUIPropertyManager.SetProperty("#btWeb.startup.link", url);
            GUIPropertyManager.SetProperty("#btWeb.link.zoom", zoom);
            MyFilmsDetail.setProcessAnimationStatus(true, m_SearchAnimation);
            GUIWindowManager.ActivateWindow((int)ExternalPluginWindows.BrowseTheWeb, false); //54537689
            MyFilmsDetail.setProcessAnimationStatus(false, m_SearchAnimation);
            GUIPropertyManager.SetProperty("#btWeb.startup.link", "");
            GUIPropertyManager.SetProperty("#btWeb.link.zoom", "");
          }
            #endregion
          break;

        case "updatepersonmovie":
          // ToDo: Update personinfo for all involve artists (takes longer!)
          {
            break;
          }

        case "covermanager":
          {
            conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
            conf.StrTIndex = facadeFilms.SelectedListItem.Label;
            LogMyFilms.Debug("Switching to Cover Manager Window");
            MyFilmsCoverManager cm = null;
            if (cm == null)
            {
              cm = new MyFilmsCoverManager();
              GUIWindow cmwindow = (GUIWindow)cm;
              GUIWindowManager.Add(ref cmwindow);
              cm.Init();
            }
            // cm.MovieID = (int)facadeFilms.SelectedListItem.ItemId; // will be set later in cm class
            cm.setPageTitle("Cover Manager");
            //sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex));
            //cm.ArtworkFileName = GetOrCreateCoverFilename(MyFilms.r, MyFilms.conf.StrIndex, sTitles.MasterTitle);
            GUIWindowManager.ActivateWindow(cm.GetID, false);
            break;
          }

        case "suppress":
          {
            GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            List<string> choiceViewMenu = new List<string>();

            if (dlgmenu == null) return;
            dlgmenu.Reset();
            choiceViewMenu.Clear();
            dlgmenu.SetHeading(GUILocalizeStrings.Get(1079830)); // Delete movie ...

            dlgmenu.Add(GUILocalizeStrings.Get(1079831)); // Remove movie from catalog
            choiceViewMenu.Add("removefromdb");

            dlgmenu.Add(GUILocalizeStrings.Get(1079832)); // Delete movie file(s) from disk
            choiceViewMenu.Add("deletefromdisk");

            dlgmenu.Add(GUILocalizeStrings.Get(1079833)); // Delete from catalog and disk
            choiceViewMenu.Add("deletefromdbanddisk");

            dlgmenu.DoModal(GetID);
            if (dlgmenu.SelectedLabel == -1)
            {
              break;
            }
            switch (choiceViewMenu[dlgmenu.SelectedLabel].ToLower())
            {
              case "removefromdb":
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079831));//Remove movie from catalog
                dlgYesNo.SetLine(2, GUILocalizeStrings.Get(433));//confirm suppression
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)
                {
                  MyFilmsDetail.Manual_Delete((DataRow[])MyFilms.r, (int)this.facadeFilms.SelectedListItem.ItemId, true, false);
                  // Fin_Charge_Init(true, true);
                  Loadfacade(); //Fin_Charge_Init(false, true);
                }
                break;
              case "deletefromdisk":
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079832));//Delete movie file(s) from disk
                dlgYesNo.SetLine(1, GUILocalizeStrings.Get(927));// warning
                dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079834));//If you confirm, you media files will physically be deleted !
                dlgYesNo.SetLine(3, GUILocalizeStrings.Get(1079835));//Are you sure you want to delete movie ?
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)
                {
                  MyFilmsDetail.Manual_Delete((DataRow[])MyFilms.r, (int)this.facadeFilms.SelectedListItem.ItemId, false, true);
                  // Fin_Charge_Init(true, true);
                  Loadfacade(); //Fin_Charge_Init(false, true);
                }
                break;
              case "deletefromdbanddisk":
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079833));//Delete from catalog and disk
                dlgYesNo.SetLine(1, GUILocalizeStrings.Get(927));// warning
                dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079834));//If you confirm, you media files will physically be deleted !
                dlgYesNo.SetLine(3, GUILocalizeStrings.Get(1079835));//Are you sure you want to delete movie ?
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)
                {
                  // old "suppress approach" MyFilmsDetail.Suppress_Entry((DataRow[])MyFilms.r, (int)facadeFilms.SelectedListItem.ItemId);
                  MyFilmsDetail.Manual_Delete((DataRow[])MyFilms.r, (int)this.facadeFilms.SelectedListItem.ItemId, true, true);
                  // Fin_Charge_Init(true, true);
                  Loadfacade(); //Fin_Charge_Init(false, true);
                }
                break;
              default:
                break;
            }
            break;
          }

        case "grabber":
          conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
          string title = MyFilmsDetail.GetSearchTitle(r, this.facadeFilms.SelectedListItem.ItemId, "titleoption");
          string mediapath = MyFilms.r[this.facadeFilms.SelectedListItem.ItemId][MyFilms.conf.StrStorage].ToString();
          if (mediapath.Contains(";")) // take the forst source file
          {
            mediapath = mediapath.Substring(0, mediapath.IndexOf(";"));
          }

          sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);

          this.doUpdateMainViewByFinishEvent = true; // makes sure, message handler will be triggered after backgroundthread is finished
          MyFilmsDetail.grabb_Internet_Informations(title, GetID, MyFilms.conf.StrGrabber_ChooseScript, MyFilms.conf.StrGrabber_cnf, mediapath, MyFilmsDetail.GrabType.All, false, sTitles);
          // Fin_Charge_Init(false, true); // Guzzi: This might be required to reload facade and details ?
          // this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          // RefreshFacade() will be initiated by OnDetailsUpdated handler, if threaded loading is finished ! - so should NOT be loaded here ...
          break;

        case "grabber-person":
          conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
          string personname = this.facadeFilms.SelectedListItem.Label;

          this.doUpdateMainViewByFinishEvent = true; // makes sure, message handler will be triggered after backgroundthread is finished
          MyFilmsDetail.grabb_Internet_Informations(personname, GetID, MyFilms.conf.StrGrabber_ChooseScript, "", "", MyFilmsDetail.GrabType.Person, false, null);
          // Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          // RefreshFacade() will be initiated by OnDetailsUpdated handler, if threaded loading is finished ! - so should NOT be loaded here ...
          break;

        case "addtocollection":
          conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
          string groupname = Change_MovieGroupName(conf.StrTIndex);
          if (groupname == "") break;
          MyFilmsDetail.AddMovieToCollection(groupname);
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          break;

        case "removefromcollection":
          conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
          MyFilmsDetail.RemoveMovieFromCollection();
          Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          break;

        case "fanart":
          conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
          if (!MyFilmsDetail.IsInternetConnectionAvailable())
            break; // stop, if no internet available
          else
          {
            // Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();

            string personartworkpath = string.Empty;
            string imdbid = MyFilmsDetail.GetIMDB_Id(MyFilms.r[MyFilms.conf.StrIndex]);
            sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
            //string fanartTitle, personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
            //fanartTitle = MyFilmsDetail.GetFanartTitle(r[facadeFilms.SelectedListItem.ItemId], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
            if (!string.IsNullOrEmpty(sTitles.FanartTitle) && MyFilms.conf.StrFanart)
            {
              LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download Fanart: originaltitle: '" + sTitles.OriginalTitle + "' - translatedtitle: '" + sTitles.TranslatedTitle + "' - (started from main menu)");
              if (conf.UseThumbsForPersons && !string.IsNullOrEmpty(conf.StrPathArtist))
              {
                personartworkpath = MyFilms.conf.StrPathArtist;
                LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download PersonArtwork 'enabled' - destination: '" + personartworkpath + "'");
              }
              this.doUpdateMainViewByFinishEvent = true; // makes sure, message handler will be triggered after backgroundthread is finished
              MyFilmsDetail.Download_Backdrops_Fanart(sTitles.OriginalTitle, sTitles.TranslatedTitle, sTitles.FormattedTitle, sTitles.Director, imdbid, sTitles.Year.ToString(), true, GetID, sTitles.FanartTitle, personartworkpath, true, false);
            }
          }
          break;

        case "personimages":
          conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
          if (!MyFilmsDetail.IsInternetConnectionAvailable())
            break; // stop, if no internet available
          else
          {
            string personartworkpath = string.Empty;
            string imdbid = MyFilmsDetail.GetIMDB_Id(MyFilms.r[MyFilms.conf.StrIndex]);
            sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
            if (!string.IsNullOrEmpty(sTitles.FanartTitle) && MyFilms.conf.StrFanart)
            {
              LogMyFilms.Debug("Download PersonImages: originaltitle: '" + sTitles.OriginalTitle + "' - translatedtitle: '" + sTitles.TranslatedTitle + "' - (started from main menu)");
              personartworkpath = MyFilms.conf.StrPathArtist;
              LogMyFilms.Debug("Download PersonArtwork to path: '" + personartworkpath + "'");
              this.doUpdateMainViewByFinishEvent = true; // makes sure, message handler will be triggered after backgroundthread is finished
              MyFilmsDetail.Download_Backdrops_Fanart(sTitles.OriginalTitle, sTitles.TranslatedTitle, sTitles.FormattedTitle, sTitles.Director, imdbid, sTitles.Year.ToString(), true, GetID, sTitles.FanartTitle, personartworkpath, false, true);
            }
          }
          break;

        case "personimagestest":
          conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
          MyFilmsDetail.AddPersonsToDownloadQueue();
          break;

        case "deletefanart":
          dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079874));//delete fanart (current film)
          dlgYesNo.SetLine(1, "");
          dlgYesNo.SetLine(2, GUILocalizeStrings.Get(433));//confirm suppression
          dlgYesNo.DoModal(GetID);
          if (dlgYesNo.IsConfirmed)
          {
            //MyFilmsDetail.Remove_Backdrops_Fanart(MyFilms.r[facadeFilms.SelectedListItem.ItemId]["TranslatedTitle"].ToString(), false);
            //string fanartTitle = MyFilms.r[facadeFilms.SelectedListItem.ItemId][MyFilms.conf.StrTitle1].ToString();  // Fixed, as it should delete content of master title...
            string fanartTitle, personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty; int wyear = 0;
            fanartTitle = MyFilmsDetail.GetFanartTitle(r[this.facadeFilms.SelectedListItem.ItemId], out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
            MyFilmsDetail.Remove_Backdrops_Fanart(fanartTitle, false);

            // reload fanart
            string[] wfanart = MyFilmsDetail.Search_Fanart(this.facadeFilms.SelectedListItem.Label, true, "file", false, this.facadeFilms.SelectedListItem.ThumbnailImage, string.Empty);
            backdrop.Filename = wfanart[0];
            MyFilmsDetail.setGUIProperty("currentfanart", wfanart[0].ToString());
          }
          break;

        case "togglewatchedstatus":
          // first set index so that details calls don't fail (as they are relying on it)
          conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;

          if (this.facadeFilms.SelectedListItem.IsPlayed)
          {
            this.facadeFilms.SelectedListItem.IsPlayed = false;
            MyFilmsDetail.Watched_Toggle((int)this.facadeFilms.SelectedListItem.ItemId, false);
          }
          else
          {
            this.facadeFilms.SelectedListItem.IsPlayed = true;
            MyFilmsDetail.Watched_Toggle((int)this.facadeFilms.SelectedListItem.ItemId, true);
          }
          //Fin_Charge_Init(true, true);
          this.Refreshfacade(); // loads threaded: Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
          break;

        case "updatemenu":
          GUIDialogMenu dlgupdate = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          updChoice.Clear();
          if (dlgupdate == null) return;
          Context_Menu = true;
          dlgupdate.Reset();
          dlgupdate.SetHeading(GUILocalizeStrings.Get(1079892)); // Update ...

          if ((MyFilms.conf.StrSuppress || MyFilms.conf.StrSuppressManual) && this.facadeFilms.SelectedListItemIndex > -1 && !this.facadeFilms.SelectedListItem.IsFolder)
          {
            dlg.Add(GUILocalizeStrings.Get(1079830));
            updChoice.Add("suppress");
          }
          if ((this.facadeFilms.SelectedListItemIndex > -1 && !this.facadeFilms.SelectedListItem.IsFolder) && (conf.StrFileType == Configuration.CatalogType.AntMovieCatalog3 || conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended))
          {
            dlg.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
            updChoice.Add("grabber");
          }
          if (MyFilms.conf.StrFanart && this.facadeFilms.SelectedListItemIndex > -1 && !this.facadeFilms.SelectedListItem.IsFolder)
          {
            dlg.Add(GUILocalizeStrings.Get(1079862));
            updChoice.Add("fanart");
            dlg.Add(GUILocalizeStrings.Get(1079874));
            updChoice.Add("deletefanart");
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
          List<string> choiceViewGlobalOptions = new List<string>();

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
          if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
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

          this.Change_Menu_Action(choiceViewGlobalOptions[dlg1.SelectedLabel].ToLower());
          //Context_Menu = false;
          return;

      }
      #endregion
    }

    private void BrowseTheWebLauncher(string type, string extension, string searchexpression)
    {
      if (!Helper.IsBrowseTheWebAvailableAndEnabled)
      {
        ShowMessageDialog("MyFilms", "BrowseTheWeb plugin not installed or wrong version", "Minimum Version required: " + MyFilmsSettings.GetRequiredMinimumVersion(MyFilmsSettings.MinimumVersion.BrowseTheWeb));
        return;
      }

      //int webBrowserWindowID = 16002; // WindowID for GeckoBrowser 
      //int webBrowserWindowID = 54537689; // WindowID for BrowseTheWeb
      //string url = ImdbBaseUrl + "";
      string url = string.Empty;
      const string zoom = "150";

      //First search corresponding URL for the movie/person ...
      switch (type)
      {
        case "movie":
          {
            var imdb = new IMDB();
            imdb.Find(searchexpression);
            foreach (IMDB.IMDBUrl t in imdb) LogMyFilms.Debug("movie imdb internet search - found: '" + t.Title + "', URL = '" + t.URL + "'");
            if (imdb.Count > 0)
            {
              var wurl = imdb[0];
              if (wurl.URL.Length != 0)
              {
                url = wurl.URL + extension; // Assign proper Webpage for infos
                url = ImdbBaseUrl + url.Substring(url.IndexOf("title")); // redirect to base www.imdb.com server and remove localized returns...
              }
            }
            break;
          }
        case "person":
          {
            var imdb = new IMDB();
            imdb.FindActor(searchexpression);
            if (imdb.Count > 0)
            {
              IMDB.IMDBUrl wurl = imdb[0];
              if (wurl.URL.Length != 0)
              {
                url = wurl.URL + extension; // Assign proper Webpage for Actorinfos
                url = ImdbBaseUrl + url.Substring(url.IndexOf("name")); // redirect to base www.imdb.com server and remove localized returns...
              }
              //_imdb.GetActorDetails(_imdb[0], false, out imdbActor); // Details here not needed - we just want the URL !
            }
            break;
          }
      }

      if (!string.IsNullOrEmpty(url))
      {
        //Load Webbrowserplugin with the URL
        LogMyFilms.Debug("Launching BrowseTheWeb with URL = '" + url + "'");
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
      }
    }

    //*****************************************************************************************
    //*  select person type dialog
    //*****************************************************************************************
    private string SelectPersonType(int Index)
    {
      string persontype = string.Empty;

      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      if (dlg == null) return "";
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(1079909)); // select role
      var w_tableau = new ArrayList();
      var choiceSearch = new List<string>();

      foreach (string personfield in PersonTypes.Where(personfield => MyFilms.r[Index][personfield].ToString().Length > 0))
      {
        w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MyFilms.r[Index][personfield].ToString())));
        w_tableau = new ArrayList(w_tableau.ToArray().Distinct().ToList()); // make list unique/distinct

        dlg.Add(BaseMesFilms.Translate_Column(personfield) + " (" + w_tableau.Count + ")");
        choiceSearch.Add(personfield);
      }
      if (choiceSearch.Count == 0)
      {
        if (dlg1 == null) return "";
        dlg1.SetHeading(GUILocalizeStrings.Get(1079909));
        dlg1.SetLine(1, GUILocalizeStrings.Get(10798641));
        dlg1.DoModal(GUIWindowManager.ActiveWindow);
        return "";
      }
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
        return "";

      persontype = choiceSearch[dlg.SelectedLabel];
      return persontype;
    }

    //*****************************************************************************************
    //*  search related movies by persons                                                     *
    //*****************************************************************************************
    private void SearchRelatedMoviesbyPersons(int Index, bool returnToContextmenu)
    {
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
      string[] roles = { "Actors", "Producer", "Director", "Writer" }; // , "Persons"

      new Thread(delegate()
      {
        {
          if (dlg == null) return;
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(1079867)); // menu
          var w_tableau = new ArrayList();
          var choiceSearch = new List<string>();

          foreach (string role in roles.Where(role => MyFilms.r[Index][role].ToString().Length > 0))
          {
            w_tableau = Search_String(System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(MyFilms.r[Index][role].ToString())));
            foreach (object t in w_tableau)
            {
              dlg.Add(BaseMesFilms.Translate_Column(role) + " : " + t);
              choiceSearch.Add(t.ToString());
            }
          }

          if (choiceSearch.Count == 0)
          {
            if (dlgOK == null) return;
            dlgOK.SetHeading(GUILocalizeStrings.Get(1079867));
            dlgOK.SetLine(1, GUILocalizeStrings.Get(10798641));
            dlgOK.DoModal(GUIWindowManager.ActiveWindow);
            return;
          }
          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1)
          {
            if (returnToContextmenu) // only call if it was called from context menu
              Context_Menu_Movie(this.facadeFilms.SelectedListItem.ItemId);
            else
              Change_Search_Options();
            return;
          }

          // GUIWaitCursor.Init(); GUIWaitCursor.Show();
          if (dlgPrgrs != null)
          {
            dlgPrgrs.Reset();
            dlgPrgrs.DisplayProgressBar = false;
            dlgPrgrs.ShowWaitCursor = true;
            dlgPrgrs.DisableCancel(true);
            dlgPrgrs.SetHeading(GUILocalizeStrings.Get(1079867)); // menu
            dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
            dlgPrgrs.Percentage = 0;
          }

          string wperson = choiceSearch[dlg.SelectedLabel];
          dlg.Reset();
          choiceSearch.Clear();
          dlg.SetHeading(GUILocalizeStrings.Get(10798611) + wperson); // function selection (actor, director, producer)

          //First add general option to show MP Actor Infos
          if (wperson.Length > 0)
          {
            dlgPrgrs.Percentage = 20;
            dlgPrgrs.SetLine(1, "search person on IMDB ...");

            if (MyFilmsDetail.ExtendedStartmode("relatedpersonsearch: add person option to dialog menu for personinfodialog"))
            {
              // First check if actor exists... - this only works with MePo V1.1.5+
              LogMyFilms.Debug("Check, if Person is found in IMDB-DB (using MP actors DB)");
              var actorList = new ArrayList(); // Search with searchName parameter which contain wanted actor name, result(s) is in array which conatin id and name separated with char "|"
              try
              {
                MyFilmsDetail.GetActorByName(wperson, actorList);
              }
              catch (Exception)
              { }
              dlg.Add(GUILocalizeStrings.Get(10798731) + " (" + actorList.Count.ToString() + ")");
              choiceSearch.Add("PersonInfo");
            }
          }

          LogMyFilms.Debug("Adding search results for roles to menu ...");

          dlgPrgrs.SetLine(1, "search actors ...");
          dlgPrgrs.Percentage = 40;

          DataRow[] wr = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, "Actors like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
          if (wr.Length > 0)
          {
            dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(1079868) + "  (" + wr.Length + ")");
            choiceSearch.Add("Actors");
          }
          dlgPrgrs.SetLine(1, "search producers ...");
          dlgPrgrs.Percentage = 60;
          wr = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, "Producer like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
          if (wr.Length > 0)
          {
            dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(10798612) + "  (" + wr.Length + ")");
            choiceSearch.Add("Producer");
          }
          dlgPrgrs.SetLine(1, "search directors ...");
          dlgPrgrs.Percentage = 80;
          wr = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, "Director like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
          if (wr.Length > 0)
          {
            dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(1079869) + "  (" + wr.Length + ")");
            choiceSearch.Add("Director");
          }
          dlgPrgrs.SetLine(1, "search writers ...");
          dlgPrgrs.Percentage = 99;
          wr = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, "Writer like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
          if (wr.Length > 0)
          {
            dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(10798684) + "  (" + wr.Length + ")");
            choiceSearch.Add("Writer");
          }
          //wr = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, "Persons like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
          //if (wr.Length > 0)
          //{
          //  dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(10798951) + "  (" + wr.Length + ")");
          //  choiceSearch.Add("Persons");
          //}

          GUIWaitCursor.Hide();
          dlgPrgrs.Percentage = 100;
          dlgPrgrs.ShowWaitCursor = false;
          dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846));
          Thread.Sleep(10);
          dlgPrgrs.Close(); // Done ...

          if (choiceSearch.Count == 0)
          {
            if (dlgOK == null) return;
            dlgOK.SetHeading(GUILocalizeStrings.Get(1079867));
            dlgOK.SetLine(1, GUILocalizeStrings.Get(10798640));
            dlgOK.DoModal(GUIWindowManager.ActiveWindow);
            return;
          }
          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1) return;
          if (choiceSearch[dlg.SelectedLabel] == "PersonInfo")
          {
            string actorSearchname = MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(wperson);
            this.PersonInfo(actorSearchname);
            return;
          }

          SaveListState(false); // SaveLastView(); // to restore to current context after search results ...

          conf.StrSelect = choiceSearch[dlg.SelectedLabel] + " like '*" + wperson + "*'";
          switch (choiceSearch[dlg.SelectedLabel])
          {
            case "Actors":
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(1079868) + " [*" + wperson + @"*]"; // "Seletion"
              break;
            case "Producer":
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(10798612) + " [*" + wperson + @"*]";
              break;
            case "Director":
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(1079869) + " [*" + wperson + @"*]";
              break;
            case "Writer":
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(10798684) + " [*" + wperson + @"*]";
              break;
            case "Persons":
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(10798951) + " [*" + wperson + @"*]";
              break;
            default:
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + GUILocalizeStrings.Get(10798951) + " [*" + wperson + @"*]";
              break;
          }
          conf.StrTitleSelect = "";
          conf.StrViewSelect = "";
          // conf.StrFilmSelect = "";
          // conf.ViewContext = ViewContext.Person;
          //conf.WStrSort = choiceSearch[dlg.SelectedLabel];
          //conf.Wselectedlabel = "";
          //conf.WStrSortSens = " ASC";
          //conf.StrPersons = wperson;
          //conf.StrTxtSelect = BaseMesFilms.Translate_Column(choiceSearch[dlg.SelectedLabel]) + " [*" + wperson + @"*]";

          SetLabelView("search"); // show "search"
          GetFilmList();
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
        {
          {
            // this after thread finished ...
          }
          return 0;
        }, 0, 0, null);
      }) { Name = "SearchRelatedMoviesbyPersons", IsBackground = true }.Start();
    }

    private void PersonInfo(string PersonName)
    {
      var imdbActor = new IMDBActor();
      GetAndStorePersonInfo(PersonName, ref imdbActor);
      if (string.IsNullOrEmpty(imdbActor.Name))
      {
        LogMyFilms.Debug("(Person Info): No ActorIDs found for '" + PersonName + "'");
        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
        dlgOk.SetHeading("Info");
        dlgOk.SetLine(1, string.Empty);
        dlgOk.SetLine(2, "Keine Personen Infos vorhanden !");
        dlgOk.DoModal(GetID);
        return;
      }
      OnVideoArtistInfoGuzzi(imdbActor);
    }

    private void OnVideoArtistInfoGuzzi(IMDBActor actor)
    {
      // MediaPortal.GUI.Video.GUIVideoArtistInfo infoDlg = (MediaPortal.GUI.Video.GUIVideoArtistInfo)GUIWindowManager.GetWindow((int)Window.WINDOW_VIDEO_ARTIST_INFO);
      var infoDlg = (MyFilmsActorInfo)GUIWindowManager.GetWindow(ID_MyFilmsActorsInfo);
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
      infoDlg.DoModal(GetID); //infoDlg.DoModal(GUIWindowManager.ActiveWindow);
    }

    private void GetAndStorePersonInfo(string PersonName, ref IMDBActor imdbActor)
    {
      var imdb = new IMDB();
      var imdbfetcher = new IMDBFetcher(null);
      bool fetchActors = imdbfetcher.FetchActors();
      imdb.FindActor(PersonName);
      foreach (var t in imdb)
      {
        //#if MP1X
        //        imdb.GetActorDetails(imdb[i], false, out imdbActor);
        //#else
        //        imdb.GetActorDetails(imdb[i], out imdbActor);
        //#endif
        GUIUtils.GetActorDetails(imdb, t, false, out imdbActor);
        if (!string.IsNullOrEmpty(imdbActor.ThumbnailUrl))
        {
          break;
        }
      }
      if (!string.IsNullOrEmpty(imdbActor.ThumbnailUrl))
      {
        if (!string.IsNullOrEmpty(conf.StrPathArtist))
        {
          //  string largeCoverArt = Utils.GetLargeCoverArtName(Thumbs.MovieActors, imdbActor.Name);
          //  string coverArt = Utils.GetCoverArtName(Thumbs.MovieActors, imdbActor.Name);
          //  Utils.FileDelete(largeCoverArt);
          //  Utils.FileDelete(coverArt);
          //  //DownloadCoverArt(Thumbs.MovieActors, imdbActor.ThumbnailUrl, imdbActor.Name);
          string filename1person = GrabUtil.DownloadPersonArtwork(conf.StrPathArtist, imdbActor.ThumbnailUrl, PersonName, true, true, out filename1person);
          LogMyFilms.Info("Person Artwork '" + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + "' downloaded for '" + PersonName + "', path = '" + filename1person + "'");
        }
        else
        {
          LogMyFilms.Debug("IMDBFetcher single actor fetch could not be done - no person artwork path defined in MyFilms config");
        }
      }
      else
      {
        Log.Debug("IMDBFetcher single actor fetch: url=null or empty for actor {0}", PersonName);
      }
      // Add actor to datbbase to get infos in person facades later...
      //#if MP1X
      //      int actorId = VideoDatabase.AddActor(imdbActor.Name);
      //#else
      //      int actorId = VideoDatabase.AddActor(imdbActor.IMDBActorID, imdbActor.Name);
      //#endif
      int actorId = GUIUtils.AddActor(imdbActor.IMDBActorID, imdbActor.Name);
      if (actorId > 0)
      {
        VideoDatabase.SetActorInfo(actorId, imdbActor);
        //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);
      }

      //MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorID);
    }

    private IMDBActor grabb_Person_Details(string name)
    {
      var person = new IMDBActor();
      MyFilmsDetail.grabb_Internet_Informations_Search(name, GetID, conf.StrGrabber_cnf, "", MyFilmsDetail.GrabType.Person, null);
      return person;
    }

    #region Old Zebons methods (disabled)
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
    #endregion

    //*****************************************************************************************
    //*  search related movies by properties  (Guzzi Version)                                 *
    //*****************************************************************************************
    private void SearchRelatedMoviesbyProperties(int Index, bool returnToContextmenu) // (int Index, IEnumerable<string> wSearchList)
    {
      // first select the property to be searching on
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var choiceSearch = new List<string>();
      var wTableau = new ArrayList();
      var wsubTableau = new ArrayList();
      int minChars = 2;
      bool Filter = true;
      if (dlg == null) return;

      conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
      conf.WStrSort = conf.StrSTitle;
      conf.Boolselect = false;
      conf.Boolreturn = false;
      ArrayList displayItems = MyFilms.GetDisplayItems("view");

      #region build menu
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // Choose property for search

      dlg.Add(BaseMesFilms.Translate_Column(MyFilms.conf.StrTitle1));
      choiceSearch.Add(MyFilms.conf.StrTitle1);

      if (Helper.FieldIsSet(MyFilms.conf.StrTitle2))
      {
        dlg.Add(BaseMesFilms.Translate_Column(MyFilms.conf.StrTitle2));
        choiceSearch.Add(MyFilms.conf.StrTitle2);
      }

      string[] defaultupdateitems = { "Category", "Year", "Date", "Country", "Rating" };
      foreach (string wupd in defaultupdateitems)
      {
        dlg.Add(BaseMesFilms.Translate_Column(wupd.Trim()));
        choiceSearch.Add(wupd.Trim());
      }

      dlg.Add(GUILocalizeStrings.Get(10798765)); // *** show all ***
      choiceSearch.Add("showall");

      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1)
      {
        if (returnToContextmenu) // only call if it was called from context menu
          Context_Menu_Movie(this.facadeFilms.SelectedListItem.ItemId);
        else
          Change_Search_Options();
        return;
      }

      // show all search fields, if selected ...
      if (choiceSearch[dlg.SelectedLabel] == "showall")
      {
        dlg.Reset();
        dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
        choiceSearch.Clear();
        foreach (string[] displayItem in displayItems)
        {
          if (!string.IsNullOrEmpty(MyFilms.r[Index][displayItem[0]].ToString()))
          {
            string entry = (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1];
            dlg.Add(entry);
            choiceSearch.Add(displayItem[0]);
            LogMyFilms.Debug("search related movie properties menu - add '{0}' as '{1}'", displayItem[0], entry);
          }
          else
            LogMyFilms.Debug("SearchRelatedMoviesByProperties: Property '" + displayItem[0] + "' not added to menu list, as it is null or empty !");
        }
        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1)
        {
          if (returnToContextmenu) // only call if it was called from context menu
            Context_Menu_Movie(this.facadeFilms.SelectedListItem.ItemId);
          else
            Change_Search_Options();
          return;
        }
      }
      #endregion
      string wproperty = choiceSearch[dlg.SelectedLabel];

      #region perform search action for related properties and show items (and optional subitems)
      dlg.Reset();
      choiceSearch.Clear();
      LogMyFilms.Debug("(RelatedPropertySearch): Searchstring in Property: '" + MyFilms.r[Index][wproperty] + "'");
      //PersonTitle Grabbing (Double Words)
      if (wproperty.ToLower() != "description" && wproperty.ToLower() != "comments" && wproperty.ToLower() != "rating")
      {
        wTableau = Search_String(MyFilms.r[Index][wproperty].ToString());
        foreach (var t in wTableau)
        {
          foreach (string entry in from string[] displayItem in displayItems where wproperty.ToLower().Equals(displayItem[0].ToLower()) select (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1])
          {
            dlg.Add(entry + ": " + t);
            choiceSearch.Add(t.ToString());
            LogMyFilms.Debug("(RelatedPropertySearch): Searchstring Result Add: '" + t + "'");
            break;
          }
        }
      }
      //SubWordGrabbing for more Details, if necessary
      if (wproperty.ToLower() != "description" && wproperty.ToLower() != "comments")
        minChars = 2;
      else
        minChars = 5;
      if (MyFilms.r[Index][wproperty].ToString().Length > 0) //To avoid exception in subgrabbing
        wsubTableau = SubWordGrabbing(MyFilms.r[Index][wproperty].ToString(), minChars, Filter);
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
          foreach (object t in wsubTableau)
          {
            if (wTableau.Contains(t)) // Only Add SubWordItems if not already present in SearchStrin Table
            {
              LogMyFilms.Debug("(RelatedPropertySearch): Searchstring Result already Present: '" + t + "'");
              break;
            }
            else
            {
              foreach (string entry in from string[] displayItem in displayItems where wproperty.ToLower().Equals(displayItem[0].ToLower()) select (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1])
              {
                dlg.Add(entry + " (" + GUILocalizeStrings.Get(10798627) + "): '" + t + "'"); // property (singleword): 'value'
                choiceSearch.Add(t.ToString());
                LogMyFilms.Debug("(RelatedPropertySearch): Searchstring Result Add: '" + t + "'");
                break;
              }
            }
          }
        }
      }
      if ((choiceSearch.Count == 0) && (1 == 2)) // Temporarily Disabled
      {
        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));//MyFilms System Information
        dlgOk.SetLine(1, GUILocalizeStrings.Get(10798625));
        dlgOk.DoModal(GetID);
        if (dlg.SelectedLabel == -1)
          return;
        //break;
      }
      #endregion

      #region now search the user selection (value)
      SaveListState(false);

      LogMyFilms.Debug("(Related Search by properties - ChoiceSearch.Count: " + choiceSearch.Count);
      if (choiceSearch.Count > 0)
      {
        dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // property selection
        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;
      }
      else dlg.SelectedLabel = 0;
      LogMyFilms.Debug("(Related Search by properties - Selected wproperty: " + wproperty + "'");
      string w_rating = "0";

      if (choiceSearch.Count == 0) //Use Special "is NULL" handling if property is empty ...
        conf.StrSelect = wproperty + " is NULL";
      else
        w_rating = MyFilms.r[Index][wproperty].ToString().Replace(",", ".");
      if ((wproperty == "Rating") && (choiceSearch[dlg.SelectedLabel] == "RatingExact"))
        conf.StrSelect = wproperty + " = " + w_rating;
      else
        if ((wproperty == "Rating") && (choiceSearch[dlg.SelectedLabel] == "RatingBetter"))
          conf.StrSelect = wproperty + " > " + w_rating;
        else
        {
          UpdateRecentSearch(choiceSearch[dlg.SelectedLabel], 20);
          conf.StrSelect = (wproperty == "Number") ? wproperty + " = " + choiceSearch[dlg.SelectedLabel] : wproperty + " like '*" + choiceSearch[dlg.SelectedLabel] + "*'";
        }
      if (choiceSearch.Count == 0)
        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " (none)";
      else
        conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + choiceSearch[dlg.SelectedLabel] + @"*]";
      conf.StrTitleSelect = string.Empty;
      #endregion
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
      var ds = new AntMovieCatalog();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var choiceSearch = new List<string>();
      var wTableau = new ArrayList();
      var wsubTableau = new ArrayList();
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
          this.Context_Menu_Movie(this.facadeFilms.SelectedListItem.ItemId);
        else
          this.Change_Search_Options();
        return;
      }
      string wproperty = choiceSearch[dlg.SelectedLabel];

      var w_count = new ArrayList();
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
      string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
      DataRow[] wr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
      wTableau.Add(string.Format(GUILocalizeStrings.Get(10798623))); //Add Defaultgroup for invalid or empty properties
      w_count.Add(0);
      foreach (DataRow wsr in wr)
      {
        foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ToString().Contains(wproperty)))
        {
          if (wsr[dc.ColumnName].ToString().Length == 0) //Empty property special handling
          {
            w_count[0] = (int)w_count[0] + 1;
            break;
          }
          else
          {
            wsubTableau = SubItemGrabbing(wsr[dc.ColumnName].ToString()); //Grab SubItems
            foreach (object t in wsubTableau)
            {
              if (wTableau.Contains(t.ToString())) // search position in w_tableau for adding +1 to w_count
              {
                for (int i = 0; i < wTableau.Count; i++)
                {
                  if (wTableau[i].ToString() == t.ToString())
                  {
                    w_count[i] = (int)w_count[i] + 1;
                    //LogMyFilms.Debug("SubItemGrabber: add Counter for '" + wsub_tableau[wi].ToString() + "'");
                    break;
                  }
                }
              }
              else // add to w_tableau and move 1 to w_count
              {
                LogMyFilms.Debug("SubItemGrabber: add new Entry for '" + wsr[dc.ColumnName] + "'");
                wTableau.Add(t.ToString());
                w_count.Add(1);
              }
            }
          }
        }
      }
      if (wTableau.Count == 0) LogMyFilms.Debug("PropertyClassCount is 0");

      string wproperty2 = "";

      if (wproperty != "randomall")
      {
        dlg.Reset();
        dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), wproperty)); // menu
        choiceSearch.Clear();
        //w_tableau = Array.Sort(w_tableau);
        for (int i = 0; i < wTableau.Count; i++)
        {
          dlg.Add(string.Format(GUILocalizeStrings.Get(10798626), w_count[i], wTableau[i]));
          choiceSearch.Add(wTableau[i].ToString());
        }
        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;
        wproperty2 = choiceSearch[dlg.SelectedLabel];
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

      foreach (DataRow sr in r.Where(sr => Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer) && sr[MyFilms.conf.StrStorageTrailer].ToString().Trim() != ""))
      {
        try
        {
          var movie = new MFMovie();
          movie.Config = Configuration.CurrentConfig; // MF config context
          movie.ID = !string.IsNullOrEmpty(sr["Number"].ToString()) ? Int32.Parse(sr["Number"].ToString()) : 0;
          movie.Year = Int32.Parse(sr["Year"].ToString());
          movie.Title = sr["OriginalTitle"].ToString();

          string mediapath = string.Empty;
          if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
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
      // Remove any blocked movies
      //moviesToPlay.RemoveAll(movie => TraktSettings.BlockedFolders.Any(f => movie.Path.Contains(f)));

      //// get the movies that we have watched
      //List<MFMovie> MovieList = (from MFMovie movie in allmovies select movie).ToList();
      //List<MFMovie> movielist = moviesToPlay.Where(m => m.Watched == true).ToList();

      //// Populte currentmovieslist as GUIListItems
      //for (int i = 0; i < facadeFilms.Count; i++)
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

      //////Choose Random Movie from Resultlist
      ////System.Random rnd = new System.Random();
      ////Int32 RandomNumber = rnd.Next(currentTrailerMoviesList.Count);
      ////LogMyFilms.Debug("RandomNumber: '" + RandomNumber + "', Record: '" + currentTrailerMoviesList[RandomNumber].ID + "', RandomTitle: '" + currentTrailerMoviesList[RandomNumber].Title + "'");

      //////Set Filmlist to random Movie:
      ////conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = string.Empty; //clear all selects
      ////conf.WStrSort = conf.StrSTitle;
      ////conf.Boolselect = false;
      ////conf.Boolreturn = false;

      ////conf.StrSelect = "number = " + Convert.ToInt32(currentTrailerMoviesList[RandomNumber].ID);
      ////conf.StrTxtSelect = "Selection number [" + Convert.ToInt32(currentTrailerMoviesList[RandomNumber].ID) + "]";
      ////conf.StrTitleSelect = string.Empty;
      //////getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
      ////LogMyFilms.Debug("(Guzzi): Change_View filter - " + "StrSelect: " + conf.StrSelect + " | WStrSort: " + conf.WStrSort);
      ////SetLabelView("search"); // show "search"
      ////GetFilmList(); // Added to update facade

      //////Set Context to first and only title in facadeview
      ////facadeFilms.SelectedListItemIndex = 0; //(Auf ersten und einzigen Film setzen, der dem Suchergebnis entsprechen sollte)
      ////if (!facadeFilms.SelectedListItem.IsFolder && !conf.Boolselect)
      ////// New Window for detailed selected item information
      ////{
      ////  conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
      ////  conf.StrTIndex = facadeFilms.SelectedListItem.Label;
      ////  GUITextureManager.CleanupThumbs();
      ////  //GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
      ////}
      ////else
      ////// View List as selected
      ////{
      ////  conf.Wselectedlabel = facadeFilms.SelectedListItem.Label;
      ////  Change_Layout_Action(MyFilms.conf.StrLayOut);
      ////  if (facadeFilms.SelectedListItem.IsFolder)
      ////    conf.Boolreturn = false;
      ////  else
      ////    conf.Boolreturn = true;
      ////  do
      ////  {
      ////    if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
      ////    conf.StrTitleSelect += conf.Wselectedlabel;
      ////  } while (GetFilmList() == false); //keep calling while single folders found
      ////}

      ////PlayRandomTrailer(false);

      ////LogMyFilms.Debug("(SearchRandomWithTrailer-Info): Here should happen the handling of menucontext....");
    }

    private void PlayRandomTrailersInit(string currentLabel, bool showCategorySelection)
    {
      LogMyFilms.Debug("PlayRandomTrailersInit() - currentLabel = '" + currentLabel + "', showCategorySelection = '" + showCategorySelection + "'");
      currentTrailerMoviesList.Clear();
      // string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
      // DataRow[] wr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);

      foreach (DataRow sr in r.Where(sr => Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer) && sr[MyFilms.conf.StrStorageTrailer].ToString().Trim() != ""))
      {
        try
        {
          MFMovie movie = new MFMovie();
          movie.Config = Configuration.CurrentConfig; // MF config context
          movie.ID = !string.IsNullOrEmpty(sr["Number"].ToString()) ? Int32.Parse(sr["Number"].ToString()) : 0;
          movie.Year = Int32.Parse(sr["Year"].ToString());
          movie.Title = sr["OriginalTitle"].ToString();

          string mediapath = string.Empty;
          if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
          {
            mediapath = sr[MyFilms.conf.StrStorageTrailer].ToString();
            if (mediapath.Contains(";")) // take the first source file
              mediapath = mediapath.Substring(0, mediapath.IndexOf(";")).Trim();
          }
          movie.File = mediapath;
          movie.Trailer = mediapath;
          movie.DateAdded = sr["Date"].ToString();

          currentTrailerMoviesList.Add(movie);
        }
        catch (Exception mex)
        {
          LogMyFilms.Error("add movie exception - err:{0} stack:{1}", mex.Message, mex.StackTrace);
        }
      }
      #region Remove any blocked movies (disabled)
      //moviesToPlay.RemoveAll(movie => TraktSettings.BlockedFolders.Any(f => movie.Path.Contains(f)));

      //// get the movies that we have watched
      //List<MFMovie> MovieList = (from MFMovie movie in allmovies select movie).ToList();
      //List<MFMovie> movielist = moviesToPlay.Where(m => m.Watched == true).ToList();

      //// Populte currentmovieslist as GUIListItems
      //for (int i = 0; i < facadeFilms.Count; i++)
      //{
      //}
      #endregion

      // we now have a list with movies matching the choice and their index/number value -> now do loop for selection
      // for (int i = 0; i < currentTrailerMoviesList.Count; i++) LogMyFilms.Debug("(ResultList) - Index: '" + i + "' - Number: '" + currentTrailerMoviesList[i].ID + "'");
      LogMyFilms.Debug("(ResultBuildIndex) Found " + currentTrailerMoviesList.Count + " Records");
      if (currentTrailerMoviesList.Count == 0)
      {
        ShowMessageDialog(GUILocalizeStrings.Get(10798621), "Suchergebnis: 0", "Keine Filme in der Auswahl vorhanden"); // menu for random search
        return;
      }
      PlayRandomTrailer(false);
    }

    private void OnTrailerEnded(string filename)
    {
      LogMyFilms.Debug("OnTrailerEnded(): Received TrailerEnded event with filename '" + filename + "', trailerscrobbleactive = '" + trailerscrobbleactive + "'");
      // if (MyFilms.trailerscrobbleactive) MyFilms.trailerscrobbleactive = false;

      new Thread(delegate()
      {
        {
          // GUIWaitCursor.Init(); GUIWaitCursor.Show(); 
          Thread.Sleep(2000);
          // GUIControl.ShowControl(GetID, 34);
        }
        GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              PlayRandomTrailer(true);
            }
            return 0;
          }, 0, 0, null);
      }) { Name = "MyFilmsLaunchPlayRandomTrailer", IsBackground = true }.Start();
    }

    private void PlayRandomTrailer(bool showMenu)
    {
      LogMyFilms.Debug("PlayRandomTrailer() - showMenu = '" + showMenu + "'");
      if (showMenu)
      {
        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
        dlgYesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
        //dlgYesNo.SetLine(1, MyFilms.r[Convert.ToInt32(w_index[RandomNumber])]["Originaltitle"].ToString());
        //dlgYesNo.SetLine(2, "Current ID = '" + w_index[RandomNumber] + "'");
        dlgYesNo.SetLine(2, "Und nu ?");
        dlgYesNo.TimeOut = 10;
        dlgYesNo.SetYesLabel("Options");
        dlgYesNo.SetNoLabel("Next Trailer");
        dlgYesNo.SetDefaultToYes(false);
        dlgYesNo.DoModal(ID_MyFilms);
        if (dlgYesNo.IsConfirmed)
        {
          TrailerScrobbleOptionsMenu(0);
          //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
          return;
        }
      }

      //Choose Random Movie from Resultlist
      System.Random rnd = new System.Random();
      Int32 RandomNumber = rnd.Next(currentTrailerMoviesList.Count);
      LogMyFilms.Debug("RandomNumber: '" + RandomNumber + "', Record: '" + currentTrailerMoviesList[RandomNumber].ID + "', RandomTitle: '" + currentTrailerMoviesList[RandomNumber].Title + "'");
      //for (int i = 0; i < facadeFilms.Count; i++)
      //{
      //  facadeFilms[i].Label
      //  // Do something here
      //}

      var trailer = new ArrayList();
      trailer.Add(currentTrailerMoviesList[RandomNumber].File);
      currentTrailerPlayingItem = currentTrailerMoviesList[RandomNumber];
      trailerscrobbleactive = true;

      MyFilmsDetail.trailerPlayed = true;
      // MyFilmsDetail.trailerScrobblingMode = true;
      // MyFilmsDetail.Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);

      MyFilmsDetail.Launch_Movie_Trailer_Scrobbling(trailer, ID_MyFilms);
    }

    private void TrailerScrobbleOptionsMenu(int currentNumber)
    {
      LogMyFilms.Debug("TrailerScrobbleOptionsMenu() - currentNumber = '" + currentNumber + "'");
      //// Exit fullscreen Video so we can see main facade again			
      //if (GUIGraphicsContext.IsFullScreenVideo) {
      //  GUIGraphicsContext.IsFullScreenVideo = false;
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var choiceSearch = new List<string>();

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
            MyFilmsDetail.Launch_Movie(this.facadeFilms.SelectedListItem.ItemId, GetID, null, false);
            //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            return;
          case "PlayMovieTrailer":
            //Hier muß irgendwie sichergestellt werden, daß nach Rückkehr keine Neuinitialisierung erfolgt (analog return von Details 7988
            //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            //conf.Wselectedlabel = facadeFilms.SelectedListItem.Label;
            //Change_Layout_Action(MesFilms.conf.StrLayOut);
            //conf.Boolreturn = true;
            //if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
            //conf.StrTitleSelect += conf.Wselectedlabel;
            //while (GetFilmList() == false) ; //keep calling while single folders found

            MyFilmsDetail.trailerPlayed = true;
            MyFilmsDetail.Launch_Movie_Trailer(this.facadeFilms.SelectedListItem.ItemId, 7990, null); //7990 To Return to this Dialog
            // MyFilmsDetail.Launch_Movie_Trailer(1, GetID, m_SearchAnimation);
            //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[currentNumber]), GetID, null);    
            //GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            var dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dlgYesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
            dlgYesNo.SetLine(1, MyFilms.r[Convert.ToInt32(currentTrailerMoviesList[currentNumber].ID)]["Originaltitle"].ToString());
            dlgYesNo.SetLine(2, "Current ID = '" + currentTrailerMoviesList[currentNumber].ID + "'");
            dlgYesNo.DoModal(GetID);
            if (dlgYesNo.IsConfirmed)
              MyFilmsDetail.Launch_Movie(this.facadeFilms.SelectedListItem.ItemId, GetID, m_SearchAnimation, false);
            //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            break;
          case "ShowMovieDetails":
            // New Window for detailed selected item information
            //conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
            //conf.StrTIndex = facadeFilms.SelectedListItem.Label;
            GUITextureManager.CleanupThumbs();
            GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
            return;

          case "ShowMovieList":
            //GetFilmList(); //Is this necessary????
            //conf.StrIndex = facadeFilms.SelectedListItem.ItemId;
            //conf.StrTIndex = facadeFilms.SelectedListItem.Label;
            GUIControl.FocusControl(ID_MyFilms, (int)Controls.CTRL_ListFilms);
            dlg.DeInit();
            return;

          case "RepeatSearch":
            PlayRandomTrailer(false);
            //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            var dlg1YesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dlg1YesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
            dlg1YesNo.SetLine(1, GUILocalizeStrings.Get(219));
            dlg1YesNo.SetLine(2, "Zufällige Film ID = '" + currentTrailerMoviesList[currentNumber].ID + "'");
            dlg1YesNo.DoModal(ID_MyFilms);
            if (dlg1YesNo.IsConfirmed)
              //Launch_Movie(select_item, GetID, m_SearchAnimation);
              MyFilmsDetail.Launch_Movie(this.facadeFilms.SelectedListItem.ItemId, GetID, null, false);
            //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[currentNumber]), GetID, null);
            break;
          case "NewSearch":
            SearchMoviesbyRandomWithTrailer(false);
            return;

          case "Back":
            return;

          default:
            break;
        }
      }
    }

    //******************************************************************************************************
    //*  Global search movies by RANDOM (Random Search with Options, e.g. Trailer, Rating) - Guzzi Version *
    //******************************************************************************************************
    private void SearchMoviesbyRandomWithTrailerOriginalVersion(bool returnToContextmenu)
    {
      // first select the area where to make random search on - "all", "category", "year", "country"
      var ds = new AntMovieCatalog();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var choiceSearch = new List<string>();
      var wTableau = new ArrayList();
      var wsub_tableau = new ArrayList();
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
          this.Context_Menu_Movie(this.facadeFilms.SelectedListItem.ItemId);
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
          var w_count = new ArrayList();
          if (dlg == null) return;
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
          string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
          DataRow[] wr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
          wTableau.Add(string.Format(GUILocalizeStrings.Get(10798623))); //Add Defaultgroup for invalid or empty properties
          w_count.Add(0);
          foreach (DataRow wsr in wr)
          {
            foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ToString().Contains(wproperty)))
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
                      if (wTableau.Contains(t.ToString())) // search position in w_tableau for adding +1 to w_count
                      {
                        //if (!w_index.Contains(
                        for (int i = 0; i < wTableau.Count; i++)
                        {
                          if (wTableau[i].ToString() == t.ToString())
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
                        wTableau.Add(t.ToString());
                        w_count.Add(1);
                      }
                    }
                  }

                }
                if (GetItems)
                {
                  if (wTableau.Contains(wsr[dc.ColumnName])) // search position in w_tableau for adding +1 to w_count
                  {
                    for (int i = 0; i < wTableau.Count; i++)
                    {
                      if (wTableau[i].ToString() == wsr[dc.ColumnName].ToString())
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
                    wTableau.Add(wsr[dc.ColumnName].ToString());
                    w_count.Add(1);
                  }
                }
              }
            }
          }
          if (wTableau.Count == 0)
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
            for (int i = 0; i < wTableau.Count; i++)
            {
              dlg.Add(string.Format(GUILocalizeStrings.Get(10798626), w_count[i], wTableau[i]));
              choiceSearch.Add(wTableau[i].ToString());
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
          conf.StrTitleSelect = string.Empty;

          // we have: wproperty = selected category (randomall for all) and wproperty2 = value to search after

          var w_index = new ArrayList();
          int w_index_count = 0;
          string t_number_id = string.Empty;
          //Now build a list of valid movies in w_index with Number registered
          foreach (DataRow wsr in wr)
          {
            foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ColumnName == "Number"))
            {
              t_number_id = wsr[dc.ColumnName].ToString();
              //LogMyFilms.Debug("Movienumber stored as '" + t_number_id + "'");
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
          this.facadeFilms.SelectedListItemIndex = 0; //(Auf ersten und einzigen Film setzen, der dem Suchergebnis entsprechen sollte)
          if (!this.facadeFilms.SelectedListItem.IsFolder && !conf.Boolselect)
          // New Window for detailed selected item information
          {
            conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
            conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
            GUITextureManager.CleanupThumbs();
            //GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
          }
          else
          // View List as selected
          {
            conf.Wselectedlabel = this.facadeFilms.SelectedListItem.Label;
            this.Change_Layout_Action(MyFilms.conf.StrLayOut);
            conf.Boolreturn = !this.facadeFilms.SelectedListItem.IsFolder;
            do
            {
              if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
              conf.StrTitleSelect += conf.Wselectedlabel;
            } while (GetFilmList() == false); //keep calling while single folders found
          }

          //Before showing menu, first play the trailer
          //conf.Wselectedlabel = facadeFilms.SelectedListItem.Label;
          //Change_Layout_Action(MesFilms.conf.StrLayOut);
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
                MyFilmsDetail.Launch_Movie(this.facadeFilms.SelectedListItem.ItemId, GetID, null, false);
                //MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                return;
              case "PlayMovieTrailer":
                //MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                //conf.Wselectedlabel = facadeFilms.SelectedListItem.Label;
                //Change_Layout_Action(MesFilms.conf.StrLayOut);
                //conf.Boolreturn = true;
                //if (conf.StrTitleSelect != "") conf.StrTitleSelect += conf.TitleDelim;
                //conf.StrTitleSelect += conf.Wselectedlabel;
                //while (GetFilmList() == false) ; //keep calling while single folders found

                MyFilmsDetail.trailerPlayed = true;
                MyFilmsDetail.Launch_Movie_Trailer(this.facadeFilms.SelectedListItem.ItemId, 7990, null); //7990 To Return to this Dialog
                //// MyFilmsDetail.Launch_Movie_Trailer(1, GetID, m_SearchAnimation);
                ////MyFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);    
                ////GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                //GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                //dlgYesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
                //dlgYesNo.SetLine(1, MyFilms.r[Convert.ToInt32(w_index[RandomNumber])]["Originaltitle"].ToString());
                //dlgYesNo.SetLine(2, "Current ID = '" + w_index[RandomNumber] + "'");
                //dlgYesNo.DoModal(GetID);
                //if (dlgYesNo.IsConfirmed)
                //  MyFilmsDetail.Launch_Movie(this.facadeFilms.SelectedListItem.ItemId, GetID, m_SearchAnimation);
                ////MyFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                break;
              case "ShowMovieDetails":
                // New Window for detailed selected item information
                conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId; //Guzzi: Muß hier erst der facadeview geladen werden?
                conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
                GUITextureManager.CleanupThumbs();
                GUIWindowManager.ActivateWindow(ID_MyFilmsDetail);
                return;

              case "ShowMovieList":
                //GetFilmList(); //Is this necessary????
                conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId; //Guzzi: Muß hier erst der facadeview geladen werden?
                conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
                GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
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
                  MyFilmsDetail.Launch_Movie(this.facadeFilms.SelectedListItem.ItemId, GetID, null, false);
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
                break;
            }
          }
          break;
      }
      LogMyFilms.Debug("(SearchRandomWithTrailer-Info): Here should happen the handling of menucontext....");
    }

    private void SearchMoviesbyAreas(bool returnToContextmenu)
    {
      // first select the area where to make random search on - "all", "category", "year", "country"
      var ds = new AntMovieCatalog();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var choiceSearch = new List<string>();
      var w_tableau = new ArrayList();
      var wsub_tableau = new ArrayList();
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
          this.Context_Menu_Movie(this.facadeFilms.SelectedListItem.ItemId);
        else
          this.Change_Search_Options();
        return;
      }
      string wproperty = choiceSearch[dlg.SelectedLabel];
      switch (choiceSearch[dlg.SelectedLabel])
      {
        case "randomall":
          SaveListState(false);
          conf.StrSelect = string.Empty;
          conf.StrTxtSelect = "Selection [*]";
          conf.StrTitleSelect = string.Empty;
          SetLabelView("search"); // show "search"
          GetFilmList();
          break;

        default:
          var w_count = new ArrayList();
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
            foreach (DataColumn dc in ds.Movie.Columns.Cast<DataColumn>().Where(dc => dc.ToString().Contains(wproperty)))
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
          SaveListState(false);
          switch (wproperty)
          {
            case "Rating":
            case "Number":
              conf.StrSelect = wproperty + " = " + wproperty2;
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
          conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + wproperty2 + @"*]"; // selection ...

          conf.StrTitleSelect = string.Empty;

          SetLabelView("search"); // show "search"
          GetFilmList();
          break;
      }
    }

    //*****************************************************************************************
    //*  Global search persons
    //*****************************************************************************************
    private void SearchMoviesbyPersons(bool returnToContextmenu, string searchExpression, string searchField)
    {
      LogMyFilms.Debug("SearchMoviesbyPersons() - started");
      string wperson = "";
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      var choiceSearch = new List<string>();

      if (null == keyboard) return; //if (null == keyboard) return true;
      keyboard.Reset();
      keyboard.Text = "";
      keyboard.DoModal(GetID);
      if (!keyboard.IsConfirmed || keyboard.Text.Length == 0) return;

      UpdateRecentSearch(keyboard.Text, 20);
      if (control_searchText(keyboard.Text))
        wperson = keyboard.Text;
      else
        return;

      #region function selection (actor, director, producer)

      string[] roles = { "Producer", "Director", "Writer", "Actors", "Persons" };
      dlg.Reset();
      choiceSearch.Clear();
      dlg.SetHeading(GUILocalizeStrings.Get(10798611) + wperson);

      LogMyFilms.Debug("Adding search results for roles to menu ...");
      DataRow[] wr;
      int personCount;
      foreach (string role in roles)
      {
        wr = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, role + " like '*" + wperson + "*'", MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens, false);
        if (wr.Length > 0)
        {
          personCount = CountPersonsFound(wr, role, wperson);
          dlg.Add(BaseMesFilms.Translate_Column(role) + " - " + personCount + " in " + wr.Length + " movie(s)");
          choiceSearch.Add(role);
        }
      }
      if (choiceSearch.Count == 0)
      {
        if (dlg1 == null) return;
        dlg1.SetHeading(GUILocalizeStrings.Get(1079867));
        dlg1.SetLine(1, GUILocalizeStrings.Get(10798640));
        dlg1.DoModal(GUIWindowManager.ActiveWindow);
        return;
      }
      #endregion

      dlg.DoModal(GetID);

      if (dlg.SelectedLabel == -1) return;

      SaveListState(false);

      conf.Boolselect = true;
      conf.CurrentView = "PersonsSearch";
      conf.StrSelect = ""; // reset view filter
      conf.StrViewSelect = "";
      conf.StrPersons = ""; // reset person filter
      conf.IndexedChars = 0;
      conf.Boolindexed = false;
      conf.BoolSkipViewState = false;


      conf.StrSelect = choiceSearch[dlg.SelectedLabel] + " like '*" + wperson + "*'";


      conf.StrTitleSelect = "";
      conf.ViewContext = ViewContext.Person;
      conf.WStrSort = choiceSearch[dlg.SelectedLabel];
      conf.WStrSortSens = " ASC";
      conf.Wselectedlabel = "";
      conf.StrPersons = ""; // conf.StrPersons = wperson;
      SetLabelView("search"); // show "search" // SetLabelView(persontype.ToLower());
      // this.Change_View_Action(persontype);
      getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, wperson, true, ""); // getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.StrPersons, true, string.Empty);
    }

    private int CountPersonsFound(DataRow[] wr, string WStrSort, string searchexpression)
    {
      int wi;
      int count = 0;
      string champselect;
      HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //List<string> itemList = new List<string>();

      bool issplitfield = IsSplittableField(WStrSort);
      bool dontsplitvalues = MyFilms.conf.BoolDontSplitValuesInViews;
      bool showEmptyValues = MyFilms.conf.BoolShowEmptyValuesInViews;

      watch.Reset(); watch.Start();
      foreach (DataRow row in wr)
      {
        champselect = row[WStrSort].ToString().Trim();
        if (issplitfield && !dontsplitvalues)
        {
          ArrayList wtab = Search_String(champselect, false);
          if (wtab.Count > 0) // itemList.AddRange(wtab);
          {
            for (wi = 0; wi < wtab.Count; wi++)
            {
              if (set.Add((string)wtab[wi])) count++; // itemList.Add((string)wtab[wi]);
            }
          }
          else if (showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            if (set.Add(champselect)) count++;  // itemList.Add(champselect);
          }
        }
        else
        {
          if (champselect.Length > 0 || showEmptyValues)  // only add empty entries, if they should show - speeds up sorting otherwise ...
          {
            if (set.Add(champselect)) count++;  // itemList.Add(champselect);
          }
        }
      }
      LogMyFilms.Debug("CountViewItems - Finished Count ('" + count + "') (" + (watch.ElapsedMilliseconds) + " ms)  - Read View Names for '" + WStrSort + "' finished (splittable items = '" + issplitfield + "', dontsplitvalues = '" + dontsplitvalues + "')");
      count = set.Count(t => t.IndexOf(searchexpression, StringComparison.OrdinalIgnoreCase) >= 0); // count = set.Count(t => t.ToUpperInvariant().Contains(searchexpression.ToUpperInvariant()));
      watch.Stop();
      LogMyFilms.Debug("CountViewItems - '" + count + "' Items found for SearchExpression '" + searchexpression + "' (" + (watch.ElapsedMilliseconds) + " ms)");
      return count;
    }

    //*****************************************************************************************
    //*  Global search movies by properties     (Guzzi Version)                               *
    //*****************************************************************************************
    private void SearchMoviesbyProperties(bool returnToContextmenu, string searchExpression, string searchField) // Old hardcoded searchlist: "TranslatedTitle|OriginalTitle|Description|Comments|Actors|Director|Producer|Rating|Year|Date|Category|Country"
    {
      LogMyFilms.Debug("SearchMoviesbyProperties() - started");
      // first select the property to be searching on
      var ds = new AntMovieCatalog();
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      var dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      var keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      var choiceSearch = new List<string>();
      var w_tableau = new ArrayList();
      string searchstring = string.Empty;
      string wproperty = "all";
      bool continueSearch = false;

      if (string.IsNullOrEmpty(searchExpression)) // if no search startparameter is set ...
      {
        if (dlg == null) return;
        dlg.Reset();
        dlg.SetHeading(GUILocalizeStrings.Get(10798615)); // global search (films)

        if (SearchHistory.Count > 0) // only show dialog, if there was a searchword stored before ...
        {
          dlg.Add(GUILocalizeStrings.Get(10798609));//last recent searches
          choiceSearch.Add("recentsearch");
        }

        dlg.Add(GUILocalizeStrings.Get(10798616)); // search on all fields
        choiceSearch.Add("all");

        dlg.Add(BaseMesFilms.Translate_Column(conf.StrTitle1));
        choiceSearch.Add(conf.StrTitle1);

        if (Helper.FieldIsSet(conf.StrTitle2))
        {
          dlg.Add(BaseMesFilms.Translate_Column(conf.StrTitle2));
          choiceSearch.Add(conf.StrTitle2);
        }

        dlg.Add(GUILocalizeStrings.Get(10798765)); // *** show all ***
        choiceSearch.Add("showall");

        dlg.DoModal(GetID);
        if (dlg.SelectedLabel == -1)
        {
          if (returnToContextmenu) this.Context_Menu_Movie(this.facadeFilms.SelectedListItem.ItemId);
          else this.Change_Search_Options();
          return;
        }

        // show all search fields, if selected ...
        if (choiceSearch[dlg.SelectedLabel] == "showall")
        {
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(10798615)); // menu
          choiceSearch.Clear();
          var displayItems = GetDisplayItems("view");
          foreach (string[] displayItem in displayItems)
          {
            string entry = (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1];
            dlg.Add(entry);
            choiceSearch.Add(displayItem[0]);
            LogMyFilms.Debug("Search Menu - add '{0}' as '{1}'", displayItem[0], entry);
          }
          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1)
          {
            if (returnToContextmenu)
              this.Context_Menu_Movie(this.facadeFilms.SelectedListItem.ItemId);
            else
              this.Change_Search_Options();
            return;
          }
        }

        wproperty = choiceSearch[dlg.SelectedLabel];

        #region perform selected search action
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
          if (dlg.SelectedId == -1) return;
          // if (dlg.SelectedLabel == -1) return;
          searchstring = dlgrecent.SelectedLabelText;
        }
        if (keyboard == null) return;
        keyboard.Reset();
        keyboard.Text = searchstring;
        keyboard.DoModal(GetID);
        if (keyboard.IsConfirmed) continueSearch = true;
      }
      else
      {
        searchstring = searchExpression;
        keyboard.Text = searchstring;
        continueSearch = true;
      }

      // if all prerequisites are met, do the search itself
      if (continueSearch && (!string.IsNullOrEmpty(keyboard.Text) || !string.IsNullOrEmpty(searchstring)))
      {
        UpdateRecentSearch(keyboard.Text, 20);
        // makes sure, searchstring will go to first place, if already present ... maxItems 20
        ArrayList w_count = new ArrayList();
        string GlobalFilterString = GlobalFilterStringUnwatched + GlobalFilterStringIsOnline +
                                    GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating;
        switch (wproperty)
        {
          case "all":
          case "recentsearch":

            #region all and recent search

            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
            //DataRow[] wr = BaseMesFilms.ReadDataMovies(GlobalFilterString + conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
            DataRow[] wr = BaseMesFilms.ReadDataMovies(
              conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
            LogMyFilms.Debug(
              "(GlobalSearchAll) - GlobalFilterString: '" + GlobalFilterString + "' (INACTIVE for SEARCH !!!)");
            LogMyFilms.Debug("(GlobalSearchAll) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
            LogMyFilms.Debug("(GlobalSearchAll) - conf.StrTitle1    : [" + conf.StrTitle1 + " like '*']");
            LogMyFilms.Debug("(GlobalSearchAll) - conf.StrSorta     : '" + conf.StrSorta + "'");
            LogMyFilms.Debug("(GlobalSearchAll) - conf.StrSortSens  : '" + conf.StrSortSens + "'");
            LogMyFilms.Debug("(GlobalSearchAll) - searchStringKBD   : '" + keyboard.Text + "'");
            foreach (DataColumn dc in from wsr in wr from DataColumn dc in ds.Movie.Columns where wsr[dc.ColumnName].ToString().ToLower().Contains(keyboard.Text.ToLower()) select dc)
            {
              if (w_tableau.Contains(dc.ColumnName.ToLower())) // search position in w_tableau for adding +1 to w_count
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
            LogMyFilms.Debug("(GlobalSearchAll) - Result of Search in all properties (w_tableau.Count): '" + w_tableau.Count + "'");
            if (w_tableau.Count == 0) // NodeLabelEditEventArgs Results found
            {
              GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
              dlgOk.SetHeading(GUILocalizeStrings.Get(10798624)); //InfoPanel
              dlgOk.SetLine(1, GUILocalizeStrings.Get(10798625));
              dlgOk.DoModal(GetID);
              if (dlg.SelectedLabel == -1) return;
              break;
            }
            dlg.Reset();
            dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798618), keyboard.Text)); // menu & SearchString
            choiceSearch.Clear();
            string[] PropertyList = new string[]
              {
                "TranslatedTitle", "OriginalTitle", "Description", "Comments", "Actors", "Director", "Producer", "Year",
                "Date", "Category", "Country", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked",
                "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate",
                "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "Borrower"
              };
            string[] PropertyListLabel = new string[]
              {
                "10798659", "10798658", "10798669", "10798670", "10798667", "10798661", "10798662", "10798665",
                "10798655", "10798664", "10798663", "10798657", "10798677", "10798678", "10798660", "10798651",
                "10798652", "10798653", "10798666", "10798671", "10798672", "10798673", "10798674", "10798675",
                "10798676", "10798680", "10798681", "10798650", "10798668", "10798656"
              };
            for (int ii = 0; ii < 30; ii++)
            {
              //LogMyFilms.Debug("(GlobalSearchAll) - OutputSort: Property is '" + PropertyList[ii] + "' - '" + GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + "' (" + PropertyListLabel[ii] + ")");
              for (int i = 0; i < w_tableau.Count; i++)
              {
                //LogMyFilms.Debug("(GlobalSearchAll) - OutputSort: w_tableau is '" + w_tableau[i] + "'"); 
                if (w_tableau[i].ToString().ToLower().Equals(PropertyList[ii].ToLower()))
                {
                  dlg.Add(
                    string.Format(
                      GUILocalizeStrings.Get(10798619),
                      w_count[i],
                      GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii])))));
                  choiceSearch.Add(w_tableau[i].ToString());
                }
              }
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1) return;
            wproperty = choiceSearch[dlg.SelectedLabel];


            // from former search definition:
            //int i = 0;
            //if (choiceSearch[dlg.SelectedLabel] == "search1") i = 1;
            //var ds = new AntMovieCatalog();
            //if (control_searchText(keyboard.Text))
            //{
            //  if (ds.Movie.Columns[conf.StrSearchItem[i]].DataType.Name == "string")
            //    conf.StrSelect = conf.StrSearchItem[i] + " like '*" + keyboard.Text + "*'";
            //  else
            //    conf.StrSelect = conf.StrSearchItem[i] + " = '" + keyboard.Text + "'";
            //  conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + conf.StrSearchText[i] + " [*" + keyboard.Text + @"*]";
            //  conf.StrTitleSelect = "";
            //  GetFilmList();
            //}
            //else return; // false;

            if (control_searchText(keyboard.Text))
            {
              SaveListState(false);

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
              conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + BaseMesFilms.Translate_Column(wproperty) + " [*" + keyboard.Text + @"*]";

              conf.StrTitleSelect = string.Empty;
              // getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
              SetLabelView("search"); // show "search"
              GetFilmList();
            }
            break;

            #endregion

          case "all-Zebons":

            #region All-Zebons

            //ArrayList w_count = new ArrayList();
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798613)); // menu
            DataRow[] wrz = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
            foreach (string wsearch in from wsr in wrz from string wsearch in GetDisplayItems("view") where System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wsr[wsearch].ToString().ToLower())).Contains(keyboard.Text.ToLower()) select wsearch)
            {
              if (w_tableau.Contains(wsearch)) // search position in w_tableau for adding +1 to w_count
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
            if (dlg.SelectedLabel == -1) return;
            wproperty = choiceSearch[dlg.SelectedLabel];
            if (control_searchText(keyboard.Text))
            {
              SaveListState(false);
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

            #endregion

          default:

            #region default search handling

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
              foreach (DataColumn dc in wdr.SelectMany(wsr => (from DataColumn dc in ds.Movie.Columns where dc.ColumnName.ToLower() == wproperty.ToLower() where wsr[dc.ColumnName].ToString().ToLower().Contains(keyboard.Text.ToLower()) select dc)))
              {
                if (w_tableau.Contains(dc.ColumnName.ToLower())) // search position in w_tableau for adding +1 to w_count
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
              LogMyFilms.Debug("(GlobalSearchAll) - Result of Search in all properties (w_tableau.Count): '" + w_tableau.Count + "'");
              if (w_tableau.Count == 0) // No Results found
              {
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(10798624)); //InfoPanel
                dlgOk.SetLine(1, GUILocalizeStrings.Get(10798625));
                dlgOk.DoModal(GetID);
                if (dlg.SelectedLabel == -1) return;
                break;
              }

              // this was former search stuff:
              //int i = 0;
              //if (choiceSearch[dlg.SelectedLabel] == "search1") i = 1;
              //var ds = new AntMovieCatalog();
              //if (control_searchText(keyboard.Text))
              //{
              //  if (ds.Movie.Columns[conf.StrSearchItem[i]].DataType.Name == "string")
              //    conf.StrSelect = conf.StrSearchItem[i] + " like '*" + keyboard.Text + "*'";
              //  else
              //    conf.StrSelect = conf.StrSearchItem[i] + " = '" + keyboard.Text + "'";
              //  conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + conf.StrSearchText[i] + " [*" + keyboard.Text + @"*]";
              //  conf.StrTitleSelect = "";
              //  GetFilmList();
              //}
              //else return; // false;

              SaveListState(false);

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

              conf.StrTxtSelect = dlg.SelectedLabelText + " [*" + keyboard.Text + @"*]"; // conf.StrTxtSelect = GUILocalizeStrings.Get(1079870) + " " + dlg.SelectedLabelText + " [*" + keyboard.Text + @"*]"; // Zebons Version
              //conf.StrTxtSelect = "Selection " + wproperty + " [*" + keyboard.Text + @"*]"; // Guzzi Version
              conf.StrTitleSelect = "";
              // getSelectFromDivx(conf.StrSelect, wproperty, conf.WStrSortSens, keyboard.Text, true, "");
              SetLabelView("search"); // show "search"
              GetFilmList();
            }
            break;

            #endregion
        }
      }
        #endregion
    }

    //*****************************************************************************************
    //*  Global search movies with missing/empty properties                                   *
    //*****************************************************************************************
    private void SearchIncompleteMovies()
    {
      AntMovieCatalog ds = new AntMovieCatalog();
      List<string> choiceSearch = new List<string>();
      ArrayList w_tableau = new ArrayList();
      ArrayList w_count = new ArrayList();
      string wproperty = string.Empty;
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      if (dlg == null) return;
      dlg.Reset();
      dlg.SetHeading(GUILocalizeStrings.Get(10798717)); // incomplete movie data
      DataRow[] dataRows = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrTitle1 + " like '*'", conf.StrSorta, conf.StrSortSens);
      foreach (DataColumn dc in dataRows.SelectMany(dataRow => ds.Movie.Columns.Cast<DataColumn>().Where(dc => string.IsNullOrEmpty(dataRow[dc.ColumnName].ToString()))))
      {
        if (w_tableau.Contains(dc.ColumnName.ToLower()))
        {
          for (int i = 0; i < w_tableau.Count; i++) // search position in w_tableau for adding +1 to w_count
          {
            if (w_tableau[i].ToString() == dc.ColumnName.ToLower())
            {
              w_count[i] = (int)w_count[i] + 1;
              break;
            }
          }
        }
        else // new item - add it to w_tableau and with count 1 to w_count
        {
          w_tableau.Add(dc.ColumnName.ToLower());
          w_count.Add(1);
        }
      }
      LogMyFilms.Debug("SearchIncompleteMovies() - Result of Search in all properties (w_tableau.Count): '" + w_tableau.Count + "'");
      if (w_tableau.Count == 0) // no results found
      {
        var dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624)); //InfoPanel
        dlgOk.SetLine(1, GUILocalizeStrings.Get(10798625)); // no result found for searched items
        dlgOk.DoModal(GetID);
        if (dlg.SelectedLabel == -1) return;
        return;
      }
      dlg.Reset();
      dlg.SetHeading(string.Format(GUILocalizeStrings.Get(10798717))); // incomplete movie data
      choiceSearch.Clear();
      var propertyList = new string[] { "TranslatedTitle", "OriginalTitle", "Edition", "Description", "Comments", "Certification", "TagLine", "Actors", "Director", "Producer", "Writer", "Year", "Date", "Category", "Country", "Studio", "Rating", "Languages", "Subtitles", "FormattedTitle", "Checked", "MediaLabel", "MediaType", "Length", "VideoFormat", "VideoBitrate", "AudioFormat", "AudioBitrate", "AudioChannelCount", "Resolution", "Framerate", "Size", "Disks", "Number", "URL", "IMDB_Id", "IMDB_Rank", "TMDB_Id", "Picture", "Fanart" };
      for (int ii = 0; ii < 31; ii++)
      {
        //LogMyFilms.Debug("(GlobalSearchAll) - OutputSort: Property is '" + PropertyList[ii] + "' - '" + GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + "' (" + PropertyListLabel[ii] + ")");
        for (int i = 0; i < w_tableau.Count; i++)
        {
          //LogMyFilms.Debug("(GlobalSearchAll) - OutputSort: w_tableau is '" + w_tableau[i] + "'"); 
          if (w_tableau[i].ToString().ToLower().Equals(propertyList[ii].ToLower()))
          {
            dlg.Add(string.Format(GUILocalizeStrings.Get(10798718), w_count[i], BaseMesFilms.Translate_Column(propertyList[ii])));
            choiceSearch.Add(w_tableau[i].ToString());
          }
        }
      }
      dlg.DoModal(GetID);
      if (dlg.SelectedLabel == -1) return;
      wproperty = choiceSearch[dlg.SelectedLabel];
      LogMyFilms.Debug("SearchIncompleteMovies() - ChosenProperty is '" + wproperty + "'");

      SaveListState(false);
      conf.ViewContext = ViewContext.Movie;
      conf.StrSelect = conf.StrTitleSelect = conf.StrTxtSelect = ""; //clear all selects
      conf.WStrSort = conf.StrSTitle;
      conf.Boolselect = false;
      conf.Boolreturn = false;

      conf.StrSelect = "(" + wproperty + " is NULL OR " + wproperty + " like '')";
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

      var xmlConfig = new XmlConfig();
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "SearchHistory", conf.StrSearchHistory);
    }


    internal static void UpdateUseritemWithValue(string strUserItemSelection, string wproperty)
    {
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
        case "useritemdetails1":
          MyFilms.conf.StritemDetails1 = wproperty;
          MyFilms.conf.StrlabelDetails1 = BaseMesFilms.Translate_Column(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails1 + "', Label: '" + conf.StrlabelDetails1 + "'.");
          break;
        case "useritemdetails2":
          MyFilms.conf.StritemDetails2 = wproperty;
          MyFilms.conf.StrlabelDetails2 = BaseMesFilms.Translate_Column(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails2 + "', Label: '" + conf.StrlabelDetails2 + "'.");
          break;
        case "useritemdetails3":
          MyFilms.conf.StritemDetails3 = wproperty;
          MyFilms.conf.StrlabelDetails3 = BaseMesFilms.Translate_Column(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails3 + "', Label: '" + conf.StrlabelDetails3 + "'.");
          break;
        case "useritemdetails4":
          MyFilms.conf.StritemDetails4 = wproperty;
          MyFilms.conf.StrlabelDetails4 = BaseMesFilms.Translate_Column(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails4 + "', Label: '" + conf.StrlabelDetails4 + "'.");
          break;
        case "useritemdetails5":
          MyFilms.conf.StritemDetails5 = wproperty;
          MyFilms.conf.StrlabelDetails5 = BaseMesFilms.Translate_Column(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails5 + "', Label: '" + conf.StrlabelDetails5 + "'.");
          break;
        case "useritemdetails6":
          MyFilms.conf.StritemDetails6 = wproperty;
          MyFilms.conf.StrlabelDetails6 = BaseMesFilms.Translate_Column(wproperty);
          LogMyFilms.Debug("Display Options - change '" + strUserItemSelection + "' to DB-field: '" + conf.StritemDetails6 + "', Label: '" + conf.StrlabelDetails6 + "'.");
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
    }

    //*****************************************************************************************
    //*  Update userdefined mappings
    //*****************************************************************************************
    internal static void UpdateUserItems()
    {
      using (XmlSettings xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true)) // Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true)
      {
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem1", MyFilms.conf.Stritem1);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel1", MyFilms.conf.Strlabel1);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem2", MyFilms.conf.Stritem2);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel2", MyFilms.conf.Strlabel2);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem3", MyFilms.conf.Stritem3);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel3", MyFilms.conf.Strlabel3);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem4", MyFilms.conf.Stritem4);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel4", MyFilms.conf.Strlabel4);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItem5", MyFilms.conf.Stritem5);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabel5", MyFilms.conf.Strlabel5);

        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails1", MyFilms.conf.StritemDetails1);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails1", MyFilms.conf.StrlabelDetails1);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails2", MyFilms.conf.StritemDetails2);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails2", MyFilms.conf.StrlabelDetails2);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails3", MyFilms.conf.StritemDetails3);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails3", MyFilms.conf.StrlabelDetails3);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails4", MyFilms.conf.StritemDetails4);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails4", MyFilms.conf.StrlabelDetails4);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails5", MyFilms.conf.StritemDetails5);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails5", MyFilms.conf.StrlabelDetails5);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntItemDetails6", MyFilms.conf.StritemDetails6);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntLabelDetails6", MyFilms.conf.StrlabelDetails6);

        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntTitle1", MyFilms.conf.StrTitle1);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntTitle2", MyFilms.conf.StrTitle2);
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntSTitle", MyFilms.conf.StrSTitle);
      }
      //XmlConfig.SafeDispose();
    }

    //*****************************************************************************************
    //*  Update userdefined views
    //*****************************************************************************************
    private void SaveCustomViews()
    {
      using (var xmlSettings = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true)) // Config.GetFile(Config.Dir.Config, "MyFilms.xml"), true)
      {
        int iViewsCount = MyFilms.conf.CustomViews.View.Count;
        LogMyFilms.Debug("SaveCustomViews() - Current Total Views: '" + iViewsCount + "'");
        xmlSettings.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntViewTotalCount", iViewsCount);
        int index = 1;
        foreach (MFview.ViewRow viewRow in MyFilms.conf.CustomViews.View)
        {
          LogMyFilms.Debug("SaveCustomViews() - Saving view #'" + index + "' of " + iViewsCount + ", ViewLabel '" + viewRow.Label + "'");
          this.SaveView(xmlSettings, index, viewRow);
          index++;
        }
        for (int i = index; i < index + 2; i++)
        {
          LogMyFilms.Debug("SaveCustomViews() - Try removing view #'" + i + "'");
          this.RemoveView(xmlSettings, i);  // cleanup config file by removing unused view entries
        }
      }
      // XmlSettings.SaveCache(); // need to save to disk, as we did not write immediately
    }

    private void RemoveView(XmlSettings xmlConfig, int index)
    {
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewItem{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewEnabled{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewImagePath{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewText{0}", index));
      // XmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewLabel2{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewValue{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewFilter{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewIndex{0}", index));
      //XmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewShowEmpty{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewSortFieldViewType{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewSortDirectionView{0}", index));
      xmlConfig.RemoveEntry("MyFilms", Configuration.CurrentConfig, string.Format("AntViewLayoutView{0}", index));
    }

    private void SaveView(XmlSettings xmlConfig, int index, MFview.ViewRow viewRow)
    {
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewText{0}", index), viewRow.Label);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewEnabled{0}", index), viewRow.ViewEnabled);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewImagePath{0}", index), viewRow.ImagePath);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewItem{0}", index), viewRow.DBfield);
      // XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewLabel2{0}", index), viewRow.Label2);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewValue{0}", index), viewRow.Value);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewFilter{0}", index), viewRow.Filter);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewIndex{0}", index), viewRow.Index);
      //XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewShowEmpty{0}", index), viewRow.ShowEmpty);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewSortFieldViewType{0}", index), viewRow.SortFieldViewType);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewSortDirectionView{0}", index), viewRow.SortDirectionView);
      xmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, string.Format("AntViewLayoutView{0}", index), this.GetLayoutFromName(viewRow.LayoutView));
    }

    private int GetLayoutFromName(string layoutname)
    {
      int wLayout = 0;
      if (layoutname == "List") wLayout = 0;
      if (layoutname == "Small Icons") wLayout = 1;
      if (layoutname == "Large Icons") wLayout = 2;
      if (layoutname == "Filmstrip") wLayout = 3;
      if (layoutname == "Cover Flow") wLayout = 4;
      return wLayout;
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

      backdrop.Filename = String.Empty;
      menucover.Filename = String.Empty;
      filmcover.Filename = String.Empty;
      viewcover.Filename = String.Empty;
      personcover.Filename = String.Empty;
      groupcover.Filename = String.Empty;

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
      GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnViewAs, GUILocalizeStrings.Get(97) + "Home"); // "View by: " + <view>
      LogMyFilms.Debug("(InitMainScreen) - Initialize all properties - Finished !");
    }

    //*****************************************************************************************
    //*  Reset global Filters
    //*****************************************************************************************
    private void InitGlobalFilters(bool log)
    {
      LogMyFilms.Debug("(InitGlobalFilters) - Reset global Filters ...");
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
      if (!GlobalFilterMinRating)
      {
        GlobalFilterStringMinRating = "";
        MyFilmsDetail.clearGUIProperty("globalfilter.minrating", log);
        MyFilmsDetail.clearGUIProperty("globalfilter.minratingvalue", log);
      }
    }

    private void InitFolders()
    {
      // Check and create Group thumb folder ...
      if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies")) Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies");
      if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Views")) Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Views");
      if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Persons")) Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Persons");
      if (!Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Logos")) Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Logos");
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
      LogMyFilms.Info("InitFSwatcher() - FSwatcher started watching - DB-file: '" + conf.StrFileXml + "'");
    }

    private void FSwatcherChanged(object source, FileSystemEventArgs e)
    {
      if (FSwatcher.EnableRaisingEvents == false) // ignore event, if notification is switched off
        return;
      FSwatcher.EnableRaisingEvents = false;
      LogMyFilms.Info("WatcherChanged() - New FSwatcher Event: " + e.ChangeType + ": '" + e.FullPath + "'");

      Thread.Sleep(3000);
      //FileInfo objFileInfo = new FileInfo(e.FullPath);
      //if (!objFileInfo.Exists) 
      //  return; // ignore the file changed event

      bool success = false; // result of try open for read
      const int maxretries = 10; // max retries 10 * 3000 = 30 seconds
      int i = 0;

      while (!success && i < maxretries)
      {
        success = !Helper.IsFileUsedbyAnotherProcess(conf.StrFileXml);
        i += 1;
        if (!success) LogMyFilms.Debug("FSwatcherChanged() - Attempt '" + i + "'to open Movie Database in read mode unsuccessful, waiting for next retry");
        Thread.Sleep(3000);
      }

      if (success)
      {
        if (GUIWindowManager.ActiveWindow != ID_MyFilms)
        {
          // load dataset
          BaseMesFilms.LoadMyFilms(conf.StrFileXml);
          // disabled - we do cleanup on init phase now - and there might be writings in progress by Trakt API !
          // MyFilmsDetail.SetGlobalLock(false, MyFilms.conf.StrFileXml); // make sure, no global lock is left
          // (re)populate films
          r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens);
        }
        else
        {
          // save current position of the facade
          if (facadeFilms != null)
          {
            conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
            conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
          }
          // alternatively RefreshFacade() to be called to also update facade (only when main window is active)
          Loadfacade(); // loading threaded : Fin_Charge_Init(false, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
        }

        // this.BeginInvoke(new UpdateWatchTextDelegate(UpdateWatchText), "WatcherChanged() - New FSwatcher Event: " + e.ChangeType + ": '" + e.FullPath + "'");

        try
        {
          if (FSwatcher.Path.Length > 0) FSwatcher.EnableRaisingEvents = true;
        }
        catch (Exception ex)
        {
          LogMyFilms.DebugException("FSwatcherChanged()- FSwatcher - problem enabling Raisingevents - Message: '" + ex.Message + "'", ex);
        }

        if (SendTraktUpdateMessage)
        {
          if (ImportComplete != null && MyFilms.conf.AllowTraktSync) // trigger sync to trakt page after importer finished
          {
            ImportComplete();
            LogMyFilms.Debug("FSwatcherChanged(): Fired 'ImportCompleted' event to trigger sync to trakt page after reloading database content !");
          }
        }
        else
        {
          LogMyFilms.Debug("FSwatcherChanged(): No 'ImportCompleted' event sent (SendTraktUpdateMessage = '" + SendTraktUpdateMessage + "')");
          SendTraktUpdateMessage = true;
        }
      }
      else
      {
        LogMyFilms.Debug("FSwatcherChanged(): Reloading data not possible - cannot open file for reading ! No Update of dataset done.");
      }
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
          Thread.Sleep(5000); // Sleep for a bit; otherwise, it takes a bit of processor time
        }
      }
    }

    void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
    {
      switch (e.Mode)
      {
        case Microsoft.Win32.PowerModes.Resume:
          {
            LogMyFilms.Debug("PowerModeChanged() - MyFilms is resuming from standby");
            conf.IsResumeFromStandby = true;

            Thread.Sleep(250);

            const int maxretries = 10; // max retries 10 * 500 = 5 seconds
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
                  // disabled, we do it only on init phase!
                  // MyFilmsDetail.SetGlobalLock(false, MyFilms.conf.StrFileXml); // make sure, no global lock is left
                  r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // (re)populate films
                }
                else
                {
                  this.Loadfacade(); // loading threaded : Fin_Charge_Init(false, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
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
          break;
        case Microsoft.Win32.PowerModes.Suspend:
          if (BaseMesFilms.UpdateWorker != null && BaseMesFilms.UpdateWorker.IsBusy)
          {
            LogMyFilms.Info("PowerModeChanged() - DB updates still active ! - waiting for background worker to complete ...");
            BaseMesFilms.UpdateWorkerDoneEvent.WaitOne(60000);
            LogMyFilms.Info("PowerModeChanged() - DB updates in background worker thread finished");
            BaseMesFilms.UpdateWorkerDoneEvent.WaitOne(1000); // wait another second to finish log entries
          }
          LogMyFilms.Debug("PowerModeChanged() - MyFilms is entering standby");
          break;
        default:
          LogMyFilms.Debug("PowerModeChanged() - MyFilms detected unhandled PowerModeChanged event - no action.");
          break;
      }
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
    public void AsynUpdateDatabase(string config)
    {
      LogMyFilms.Info("AsynUpdateDatabase() - Launch global AMCU update with config = '" + config + "'");
      if (!bgUpdateDB.IsBusy && !bgUpdateDB.CancellationPending)
      {
        MyFilmsDetail.SetGlobalLock(true, MyFilms.conf.StrFileXml); // also disabled local FSwatcher
        bgUpdateDB.RunWorkerAsync(config); // bgUpdateDB.RunWorkerAsync(MyFilms.conf.StrTIndex);
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

    void bgUpdateDB_DoWork(object sender, DoWorkEventArgs e)
    {
      // BackgroundWorker worker = sender as BackgroundWorker;
      // MyFilmsDetail.RunAMCupdater(Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCUpdater.exe", "\"" + MyFilms.conf.StrAMCUpd_cnf + "\" \"" + MediaPortal.Configuration.Config.GetDirectoryInfo(Config.Dir.Log) + "\""); // Add Logpath to commandlineparameters
      string exeName = Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCUpdater.exe";
      string amcConfig = (string.IsNullOrEmpty(e.Argument.ToString()))
                   ? "\"" + MyFilms.conf.StrAMCUpd_cnf + "\""
                   : "\"" + e.Argument.ToString() + "\"";
      string argsLine = amcConfig + " " + "\"" + MediaPortal.Configuration.Config.GetDirectoryInfo(Config.Dir.Log) + "\"";
      //static public void RunAMCupdater(string exeName, string argsLine)
      if (exeName.Length > 0)
      {
        using (var p = new Process())
        {
          var psi = new ProcessStartInfo();
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

      if (GetID == ID_MyFilms)
      {
        if (Configuration.CurrentConfig != "")
        {
          if (this.facadeFilms == null || this.facadeFilms.SelectedListItemIndex == -1)
            Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, "");
          else
            Configuration.SaveConfiguration(Configuration.CurrentConfig, this.facadeFilms.SelectedListItem.ItemId, this.facadeFilms.SelectedListItem.Label);
        }

        Load_Config(Configuration.CurrentConfig, true, null);
        Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
      }

      MyFilmsDetail.SetGlobalLock(false, MyFilms.conf.StrFileXml);
      if (ImportComplete != null && MyFilms.conf.AllowTraktSync) // trigger sync to trakt page after importer finished
      {
        ImportComplete();
        LogMyFilms.Debug("bgUpdateDB_RunWorkerCompleted(): Fired 'ImportCompleted' event to trigger sync to trakt page after importer finished !");
      }
    }

    //*****************************************************************************************
    //*  Download Backdrop Fanart in Batch mode                                               *
    //*****************************************************************************************
    public void AsynUpdateFanart()
    {
      if (!bgUpdateFanart.IsBusy)
      {
        bgUpdateFanart.RunWorkerAsync(MyFilms.r);
        LogMyFilms.Info("Downloading backdrop fanart in batch mode");
      }
    }

    static void bgUpdateFanart_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      // BackgroundWorker worker = sender as BackgroundWorker;
      Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
      string fanartTitle, personartworkpath = string.Empty, wtitle = string.Empty, wttitle = string.Empty, wftitle = string.Empty, wdirector = string.Empty, wimdbid = string.Empty; int wyear = 0;
      if (MyFilms.conf.UseThumbsForPersons && !string.IsNullOrEmpty(MyFilms.conf.StrPathArtist)) // if persoin artwork path present and person thumbs enabled, also load person images
      {
        personartworkpath = MyFilms.conf.StrPathArtist;
        LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download PersonArtwork 'enabled' - destination: '" + personartworkpath + "'");
      }
      foreach (DataRow movieRecord in MyFilms.r)
      {
        fanartTitle = wtitle = wttitle = wftitle = wdirector = string.Empty;
        wyear = 0;
        wimdbid = MyFilmsDetail.GetIMDB_Id(movieRecord);
        fanartTitle = MyFilmsDetail.GetFanartTitle(movieRecord, out wtitle, out wttitle, out wftitle, out wyear, out wdirector);
        if (fanartTitle.Length <= 0)
        {
          continue;
        }
        MyFilmsDetail.setGUIProperty("statusmessage", "Updating Fanart for '" + fanartTitle + "'");
        var listemovies = Grab.GetFanart(wtitle, fanartTitle, wyear, wdirector, wimdbid, MyFilms.conf.StrPathFanart, true, false, MyFilms.conf.StrTitle1, personartworkpath);
      }
    }

    static void bgUpdateFanart_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("Backdrop Fanart download finished");
      MyFilmsDetail.clearGUIProperty("statusmessage");
      MyFilmsDetail.ShowNotificationDialog(GUILocalizeStrings.Get(10798757), "Fanart Updates finished");
    }

    //*****************************************************************************************
    //*  Download Actors Artwork in Batch mode                                               *
    //*****************************************************************************************
    public void AsynUpdateActors(ArrayList actors)
    {
      if (!bgUpdateActors.IsBusy)
      {
        bgUpdateActors.RunWorkerAsync(actors);
        LogMyFilms.Info("AsynUpdateActors() : Downloading actors artwork in batch mode");
      }
    }

    void bgUpdateActors_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      ////BackgroundWorker worker = sender as BackgroundWorker;
      ////Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
      //ArrayList persons = e.Argument as ArrayList;
      //Grabber.TheMovieDb.TmdbAPI api = new TmdbAPI(TmdbApiKey);


      //foreach (string person in persons)
      //{
      //  // ToDo: Reenable AddActor() to scrape actor infos - make sure, it's not causing too much load...
      //  // AddActor(person);
      //  TmdbPerson[] personsearchresult = api.PersonSearch(person);
      //  foreach (TmdbPerson tmdbPerson in personsearchresult)
      //  {
      //  }
      //}
    }

    static void bgUpdateActors_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("actors artwork download finished");
    }

    private void AddActor(string name)
    {
      var ds = new AntMovieCatalog();

      //List<AntMovieCatalog.PersonRow> persons = new List<AntMovieCatalog.PersonRow>();
      //persons = (from AntMovieCatalog.PersonRow person in ds.Person select person).ToList();
      //persons = ds.Person.Where(x => x.Name == name).ToList();
      if (ds.Person.Count(x => x.Name == name) == 0)
      {
        AntMovieCatalog.PersonRow person = ds.Person.NewPersonRow();
        person.Name = name;
        //ToDo: grab data here ...
        ds.Person.AddPersonRow(person);
        LogMyFilms.Info("AddActor() - Added '" + name + "' to Person table");
      }
      else
        LogMyFilms.Info("AddActor() - Not Added '" + name + "' already exists");

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
      // BackgroundWorker worker = sender as BackgroundWorker;
      string searchrep = MyFilms.conf.StrDirStor;
      var allDrives = DriveInfo.GetDrives();

      searchrep = allDrives.Where(d => (d.DriveType.ToString() == "CDRom") && d.IsReady).Aggregate(searchrep, (current, d) => current.Length > 0 ? current + ";" + d.Name : d.Name);
      var oRegex = new Regex(";");
      string[] searchDir = oRegex.Split(searchrep);
      foreach (string path in searchDir)
      {
        MyFilms.conf.MovieList.Add(Directory.GetFiles(path));
        if (MyFilms.conf.SearchSubDirs == false || !Directory.Exists(path)) continue;
        foreach (string sFolderSub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
        {
          MyFilms.conf.MovieList.Add(Directory.GetFiles(sFolderSub));
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
      LogMyFilms.Debug("AsynIsOnlineCheck() - Try Launching Availabilityscanner - InitialIsOnlineScan = '" + InitialIsOnlineScan + "'");
      if (!InitialIsOnlineScan)
      {
        if (!bgIsOnlineCheck.IsBusy) // only launch, if not yet completed and not already running
        {
          LogMyFilms.Info("AsynIsOnlineCheck() - Check IsOnline  started in batch mode");
          MyFilmsDetail.setGUIProperty("statusmessage", "Availability Scanner started ...", false);
          bgIsOnlineCheck.RunWorkerAsync(MyFilms.r);
        }
        else
          LogMyFilms.Debug(("AsynIsOnlineCheck() could not be launched because already running !"));
      }
      else
        LogMyFilms.Debug(("AsynIsOnlineCheck() not launched because 'InitialIsOnlineScan' is already done ...!"));
    }

    static void bgIsOnlineCheck_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      // BackgroundWorker worker = sender as BackgroundWorker;
      var startTime = DateTime.Now;
      var oRegex = new Regex(";");
      LogMyFilms.Info("bgIsOnlineCheck_DoWork: Now checking Online Status - Source field <film> is: '" + conf.StrStorage + "' - Source field <trailer> is: '" + conf.StrStorageTrailer + "'");

      // Check Config
      bool filmSource = Helper.FieldIsSet(conf.StrStorage);
      bool filmSearch = !(string.IsNullOrEmpty(conf.ItemSearchFile) || string.IsNullOrEmpty(MyFilms.conf.SearchFile) || conf.SearchFile == "False" || conf.SearchFile == "no");
      bool trailerSource = Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer);
      bool trailerSearch = !(string.IsNullOrEmpty(conf.ItemSearchFileTrailer) || !MyFilms.conf.SearchFileTrailer);
      LogMyFilms.Debug("bgIsOnlineCheck_DoWork: filmSource = '" + filmSource + "', filmSearch = '" + filmSearch + "', trailerSource = '" + trailerSource + "', trailerSearch = '" + trailerSearch + "'");

      // Build MovieList (files)
      if (filmSearch) // if search by filename is activated in setup ...
      {
        LogMyFilms.Debug("bgIsOnlineCheck_DoWork: Now checking Searchpathes, adding CDrom(s) and build MovieList ...");
        string searchrep = conf.StrDirStor; // Searchpath for movies 
        var allDrives = DriveInfo.GetDrives(); // get local drives to find CDrom(s)
        foreach (DriveInfo d in allDrives.Where(d => (d.DriveType.ToString() == "CDRom") && d.IsReady))
        {
          if (searchrep.Length > 0) searchrep = searchrep + ";" + d.Name;
          else searchrep = d.Name;
        }
        LogMyFilms.Debug("bgIsOnlineCheck_DoWork: Resulting Searchpathes after adding active CDrom(s): '" + searchrep + "'");

        try
        {
          string[] searchDir = oRegex.Split(searchrep);
          foreach (string path in searchDir)
          {
            if (Directory.Exists(path))
            {
              MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner - GetDir: '" + path + "'");
              conf.MovieList.Add(System.IO.Directory.GetFiles(path));
              if (MyFilms.conf.SearchSubDirs == false) continue;
              foreach (string sFolderSub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
              {
                MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner - GetDir: '" + sFolderSub + "'");
                conf.MovieList.Add(Directory.GetFiles(sFolderSub));
              }
            }
            else
            {
              LogMyFilms.Error("bgIsOnlineCheck_DoWork: Search Directory (Movie) not found: '" + path + "' - check your config!");
            }
          }
        }
        catch (Exception ex) { LogMyFilms.Debug("bgIsOnlineCheck_DoWork: SearchDir exception: " + ex.Message + ", " + ex.StackTrace.ToString()); }
        LogMyFilms.Debug("bgIsOnlineCheck_DoWork: Movie Files found: '" + conf.MovieList.Count + "'");
      }

      // Build TrailerList (files)
      if (trailerSearch) // if search by filename is activated in setup ...
      {
        LogMyFilms.Info("bgIsOnlineCheck_DoWork: Now checking Searchpathes for Trailers and build TrailerList ...");
        var searchrepTrailer = conf.StrDirStorTrailer; // Searchpath for trailer 
        var searchDirTrailer = oRegex.Split(searchrepTrailer);
        foreach (string path in searchDirTrailer)
        {
          if (Directory.Exists(path))
          {
            MyFilms.conf.TrailerList.Add(Directory.GetFiles(path));
            if (MyFilms.conf.SearchSubDirsTrailer == false) continue;
            foreach (string sFolderSub in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
              conf.TrailerList.Add(Directory.GetFiles(sFolderSub));
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
      string totalRecords = wr.Count().ToString();
      LogMyFilms.Debug("bgIsOnlineCheck_DoWork: DB-Records found: '" + totalRecords + "'");

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
            movieName = movieName.Substring(movieName.LastIndexOf(MyFilms.conf.TitleDelim, System.StringComparison.Ordinal) + 1).Trim().ToLower();
            string[] result = conf.MovieList.Find(files => files.Count(n => n.ToLower().Contains(@"\" + movieName + @".") || n.ToLower().Contains(@"\" + movieName + @"\")) > 0);
            if (result != null)
            {
              foreach (string file in result.Where(file => !string.IsNullOrEmpty(file.Trim())).Where(Utils.IsVideo))
              {
                isonline = true;
                LogMyFilms.Debug("bgIsOnlineCheck_DoWork - movie (" + counter + ") SEARCH check   file FOUND      for title '" + t[conf.StrTitle1] + "' - file found: '" + file + "'");
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
          MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner - (" + counter + " of " + totalRecords + ") - Film: '" + t[conf.StrTitle1] + "'", false);
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
            try { fileName = t[MyFilms.conf.StrStorageTrailer].ToString().Trim(); }
            catch { fileName = string.Empty; }

            string[] Mediafiles = oRegex.Split(fileName);
            foreach (string mediafile in Mediafiles)
            {
              if (mediafile.Length > 0 && File.Exists(mediafile))
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
          MyFilmsDetail.setGUIProperty("statusmessage", "Onlinescanner (" + counter + " of " + totalRecords + " - film): '" + t[conf.StrTitle1] + "'", false);
        }
      }
      DateTime stopTime = DateTime.Now;
      TimeSpan duration = stopTime - startTime;
      LogMyFilms.Info("bgIsOnlineCheck_DoWork - total runtime was: '" + duration + "', runtime in seconds: '" + duration.TotalSeconds + "'");
      MyFilmsDetail.Update_XML_database();
    }

    void bgIsOnlineCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LogMyFilms.Info("Check IsOnline in batch mode finished. (GetID = '" + GetID + "')");
      //if (GetID == ID_MyFilms || GetID == ID_MyFilmsDetail)
      //{
      //  MyFilmsDetail.ShowNotificationDialog(GUILocalizeStrings.Get(10798948), "Finished !");
      //}
      if (GetID == ID_MyFilms)
      {
        //Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeFilms.SelectedListItem.ItemId, facadeFilms.SelectedListItem.Label);
        //Load_Config(Configuration.CurrentConfig, true);
        InitialIsOnlineScan = true; // let MF know, the status has been retrieved !
        MyFilmsDetail.clearGUIProperty("statusmessage");
        MyFilmsDetail.ShowNotificationDialog(GUILocalizeStrings.Get(10798948), "Online Check finished");
        // Fin_Charge_Init(conf.AlwaysDefaultView, true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position

        // save current position of the facade
        if (facadeFilms != null)
        {
          conf.StrIndex = this.facadeFilms.SelectedListItem.ItemId;
          conf.StrTIndex = this.facadeFilms.SelectedListItem.Label;
        }
        Refreshfacade(); // Fin_Charge_Init(false, true); //need to reload the facade, but NOT default select, as it otherwise will reset global filters the user might have set...
      }
      if (ImportComplete != null && MyFilms.conf.AllowTraktSync) // trigger sync to trakt page after importer finished
      {
        ImportComplete();
        LogMyFilms.Debug("bgIsOnlineCheck_RunWorkerCompleted(): Fired 'ImportCompleted' event to trigger sync to trakt page after importer finished !");
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
      if (backdrop.Filename != null && GUIWindowManager.ActiveWindow == ID_MyFilms)
      {
        //LogMyFilms.Debug("(FanartTimerEvent): ToggleFanart triggered for '" + facadeFilms.SelectedListItem.Label + "' !");
        bool success = ToggleFanart();
        if (!success)
          LogMyFilms.Debug("(FanartTimerEvent): ToggleFanart triggered, but not executed due to backdrop not enabled or set properly !");
      }
      else
      {
        LogMyFilms.Debug("(FanartTimerEvent): ToggleFanart triggered, but NOT executed due to MyFilms Main window not active (or backdrop filename not set)!");
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
        // LogMyFilms.Debug("(ToggleFanart): Loaded fanart is: '" + backdrop.Filename + "'");
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

      if (status)
      {
        if (!backdrop.Active)
          backdrop.Active = true;
        if (currentFanartList.Count > 0)
          _fanartTimer.Change(0, RandomFanartDelay); //default 15000 = 15 secs
        else
          _fanartTimer.Change(Timeout.Infinite, Timeout.Infinite);
        GUIControl.ShowControl(GetID, 35);
      }
      else
      {
        GUIControl.HideControl(GetID, 35);
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
      ShowMessageDialog(headline, line1, line2, "");
    }

    private void ShowMessageDialog(string headline, string line1, string line2, string line3)
    {
      GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      if (dlgOK != null)
      {
        dlgOK.SetHeading(headline);
        dlgOK.SetLine(1, line1);
        dlgOK.SetLine(2, line2);
        dlgOK.SetLine(3, line3);
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
          int actorId = 0;
          string actorname = string.Empty;
          char[] splitter = { '|' };
          foreach (string[] strActor in from string act in actorList select act.Split(splitter))
          {
            // Split id from actor name (two substrings, [0] is id and [1] is name)
            actorId = Convert.ToInt32(strActor[0]); // IMDBActor  GetActorInfo(int idActor) we need integer)
            actorname = strActor[1];
          }
          MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorId);
        }
      }
    }

    private void OnDetailsUpdated(bool searchPicture)
    {
      LogMyFilms.Debug("OnDetailsUpdated(): Received DetailUpdated event in context '" + GetID + "', doUpdateMainViewByFinishEvent '" + doUpdateMainViewByFinishEvent + "'");
      if (GetID == MyFilms.ID_MyFilms && doUpdateMainViewByFinishEvent)
      {
        LogMyFilms.Debug("OnDetailsUpdated(): now reloading Details in Main screen");
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
      string txtSelect = GUIPropertyManager.GetProperty("#myfilms.select");
      // LogMyFilms.Debug("SetLabelView() - called with parameter '" + viewLabel + "', txtSelect = '" + txtSelect + "'");
      // LogMyFilms.Debug("Status: #currentmodule = '" + GUIPropertyManager.GetProperty("#currentmodule") + "'");
      if (viewLabel == GUILocalizeStrings.Get(342) || string.IsNullOrEmpty(viewLabel)) // case "films" or empty should show "all" = "filmes"
        viewLabel = "all";
      switch (viewLabel.ToLower())
      {
        case "search":
          newViewLabel = GUILocalizeStrings.Get(137);// "search"
          break;
        case "menu":
          newViewLabel = GUILocalizeStrings.Get(924);// "menu"
          break;
        case "all":
          newViewLabel = GUILocalizeStrings.Get(342); //videos
          break;
        case "storage":
          conf.StrTxtSelect = GUILocalizeStrings.Get(10798736);
          newViewLabel = GUILocalizeStrings.Get(154) + " " + GUILocalizeStrings.Get(1951); //storage
          break;

        default:
          if (BaseMesFilms.Translate_Column(viewLabel) != "")
            newViewLabel = BaseMesFilms.Translate_Column(viewLabel);
          break;
      }
      MyFilmsDetail.setGUIProperty("view", newViewLabel);
      MyFilms.conf.StrTxtView = newViewLabel;
      GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + ((txtSelect.Length > 0) ? "/" : "") + txtSelect); // GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view"));
      GUIControl.SetControlLabel(MyFilms.ID_MyFilms, (int)Controls.CTRL_BtnViewAs, GUILocalizeStrings.Get(97) + newViewLabel); // "View by: " + <view>
      LogMyFilms.Debug("SetLabelView has been called with '" + viewLabel + "' -> set view to '" + newViewLabel + "'");
      // LogMyFilms.Debug("Status: #currentmodule = '" + GUIPropertyManager.GetProperty("#currentmodule") + "'");
    }

    //*****************************************************************************************
    //*  set the #myfilms.select label
    //*****************************************************************************************
    private void SetLabelSelect(string selectLabel)
    {
      string initialSelectLabel = selectLabel;
      if (selectLabel == null) selectLabel = "";
      // LogMyFilms.Debug("SetLabelSelect() - called with parameter  '" + selectLabel + "', conf.StrTxtSelect = '" + conf.StrTxtSelect + "'");
      // LogMyFilms.Debug("Status: #currentmodule = '" + GUIPropertyManager.GetProperty("#currentmodule") + "'");

      if (selectLabel == GUILocalizeStrings.Get(10798632) || selectLabel == GUILocalizeStrings.Get(10798622)) // 10798622 all films // 10798632 (global filter active) 
        selectLabel = ""; // will reassign proper value

      switch (selectLabel.ToLower())
      {
        case "": // also includes "all" and "filtered"
        case "root":
          //conf.StrTxtSelect = GUILocalizeStrings.Get(10798622) + " " + GUILocalizeStrings.Get(10798632); // 10798622 all films // 10798632 (global filter active) 
          // 10798632 (global filter active) / Filtered  //10798622 All
          conf.StrTxtSelect = GUILocalizeStrings.Get((GlobalFilterStringUnwatched + GlobalFilterStringIsOnline + GlobalFilterStringTrailersOnly + GlobalFilterStringMinRating).Length > 0 ? 10798632 : 10798622);
          break;

        case "menu":
          conf.StrTxtSelect = GUILocalizeStrings.Get(1079819); // views menu
          break;

        default:
          conf.StrTxtSelect = selectLabel;
          break;
      }
      MyFilmsDetail.setGUIProperty("select", conf.StrTxtSelect);
      GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(MyFilms.ID_MyFilms) + "/" + GUIPropertyManager.GetProperty("#myfilms.view") + ((conf.StrTxtSelect.Length > 0) ? "/" : "") + conf.StrTxtSelect);
      // LogMyFilms.Debug("SetLabelSelect has been called with '" + initialSelectLabel + "', processed as '" + selectLabel + "' -> set select to '" + conf.StrTxtSelect + "'");
      // LogMyFilms.Debug("Status: #currentmodule = '" + GUIPropertyManager.GetProperty("#currentmodule") + "'");
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
      // Filter logFilter = new Filter(); // use to only log MyFilms messages ...
      fileTarget.FileName = Config.GetFile(Config.Dir.Log, LogFileName);
      fileTarget.Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss,fff} " +  // "${date:format=yyyy-mm-dd HH\\:mm\\:ss,fff} " + 
        // "| ${qpc:normalize=true:difference=true:alignDecimalPoint=true:precision=3:seconds=true} " + 
                          "${level:fixedLength=true:padding=5} [" +
        //"[${threadname:fixedLength=true:padding=10} | " + 
                          "${threadid:fixedLength=true:padding=3} | ${logger:fixedLength=true:padding=13:shortName=true} ]: " +
                          "${message} ${exception:format=tostring}";
      //"${qpc}";
      // ${date:format=dd-MMM-yyyy HH\\:mm\\:ss,fff}
      //${qpc:normalize=Boolean:difference=Boolean:alignDecimalPoint=Boolean:precision=Integer:seconds=Boolean}
      // ${threadid} - The identifier of the current thread.
      // ${threadname} - The name of the current thread.

      config.AddTarget("file", fileTarget);

      // Get current Log Level from MediaPortal 
      NLog.LogLevel logLevel;
      var xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"));

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

      var rule = new LoggingRule("MyFilms*", logLevel, fileTarget); // only push logging from namespace "MyFilms*" to log file
      config.LoggingRules.Add(rule);
      var rule2 = new LoggingRule("Grabber*", logLevel, fileTarget); // add Grabber classes to logging
      config.LoggingRules.Add(rule2);

      LogManager.Configuration = config;
    }

    private void CheckAndLogEnhancedSkinControls()
    {
      if (dummyFacadeMenu == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'Menu'");
      if (dummyFacadeView == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'View'");
      if (dummyFacadePerson == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'Person'");
      if (dummyFacadeIndex == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'Index'");
      if (dummyFacadeFilm == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'Film'");
      if (dummyFacadeHierarchy == null) LogMyFilms.Warn("SetDummyControlsForFacade() - control missing in skin - 'Hierarchy'");
    }

    private static void InitGUIPropertyLabels()
    {
      using (var xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")))
      {
        MyFilmsDetail.setGUIProperty("config.pluginname", xmlreader.GetValueAsString("MyFilms", "PluginName", "Films"));
        MyFilmsDetail.setGUIProperty("config.pluginmode", xmlreader.GetValueAsString("MyFilms", "PluginMode", "normal"));
        MyFilmsDetail.setGUIProperty("config.version", MyFilmsSettings.Version.ToString());
        MyFilmsDetail.setGUIProperty("config.mpversion", MyFilmsSettings.MPVersion.ToString());

        LogMyFilms.Info("Startmode: '" + xmlreader.GetValueAsString("MyFilms", "PluginMode", "normal") + "'");
        DebugPropertyLogging = (xmlreader.GetValueAsString("MyFilms", "PropertyLogging", "false").ToLower() == "true");
        LogMyFilms.Info("Property Logging: '" + DebugPropertyLogging + "'");
      }

      EmptyFacadeValue = "(<" + GUILocalizeStrings.Get(10798774) + ">)";
      using (var ds = new AntMovieCatalog())
      {
        foreach (DataColumn dc in ds.Movie.Columns)
        {
          MyFilmsDetail.setGUIProperty("db." + dc.ColumnName.ToLower() + ".label", BaseMesFilms.Translate_Column(dc.ColumnName));
        }
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

      MyFilmsDetail.setGUIProperty("user.name.label", GUILocalizeStrings.Get(10799310));
      MyFilmsDetail.setGUIProperty("user.rating.label", GUILocalizeStrings.Get(10798944));
      MyFilmsDetail.setGUIProperty("user.watched.label", GUILocalizeStrings.Get(200027));
      MyFilmsDetail.setGUIProperty("user.watchedcount.label", GUILocalizeStrings.Get(1079910));
      MyFilmsDetail.setGUIProperty("user.watcheddate.label", GUILocalizeStrings.Get(10798686));

      MyFilmsDetail.setGUIProperty("person.name.label", GUILocalizeStrings.Get(10799301));
      MyFilmsDetail.setGUIProperty("person.dateofbirth.label", GUILocalizeStrings.Get(10799302));
      MyFilmsDetail.setGUIProperty("person.placeofbirth.label", GUILocalizeStrings.Get(10799303));
      MyFilmsDetail.setGUIProperty("person.biography.label", GUILocalizeStrings.Get(10799304));

      MyFilmsDetail.clearGUIProperty("user.name.value");
      MyFilmsDetail.clearGUIProperty("user.onlinestatus");
      MyFilmsDetail.clearGUIProperty("user.rating.value");
      MyFilmsDetail.clearGUIProperty("user.watched.value");
      MyFilmsDetail.clearGUIProperty("user.watchedcount.value");
      MyFilmsDetail.clearGUIProperty("user.watchedcountglobal.value");
      MyFilmsDetail.clearGUIProperty("user.watcheddate.value");

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
      MyFilmsDetail.clearGUIProperty("index");
      MyFilmsDetail.clearGUIProperty("isplaying");
      MyFilmsDetail.clearGUIProperty("picture");
      MyFilmsDetail.clearGUIProperty("currentfanart");
      MyFilmsDetail.clearGUIProperty("statusmessage");
      MyFilmsDetail.clearGUIProperty("details.downloads.status");
      MyFilmsDetail.clearGUIProperty("details.downloads.count");
      MyFilmsDetail.clearGUIProperty("details.downloads.name");
      // MyFilmsDetail.clearGUIProperty(guiProperty.statusmessage);

      // init Details GUIproperties
      MyFilmsDetail.setGUIProperty("menu.overview", GUILocalizeStrings.Get(10798751));
      MyFilmsDetail.setGUIProperty("menu.description", GUILocalizeStrings.Get(10798752));
      MyFilmsDetail.setGUIProperty("menu.comments", GUILocalizeStrings.Get(10798753));
      MyFilmsDetail.setGUIProperty("menu.actors", GUILocalizeStrings.Get(10798754));
      MyFilmsDetail.setGUIProperty("menu.techinfos", GUILocalizeStrings.Get(10798755));
      MyFilmsDetail.setGUIProperty("menu.extradetails", GUILocalizeStrings.Get(10798756));
      MyFilmsDetail.setGUIProperty("menu.fanart", GUILocalizeStrings.Get(10798757));

    }

    private void Clear_Logos()
    {
      LogMyFilms.Debug("Clear_Logos() - clearing logos ...");
      MyFilmsDetail.clearGUIProperty("logos_id2001");
      MyFilmsDetail.clearGUIProperty("logos_id2002");
      MyFilmsDetail.clearGUIProperty("logos_id2003");
      MyFilmsDetail.clearGUIProperty("logos_id2012");
    }

    private void Load_Logos(DataRow[] dr, int index)
    {
      LogMyFilms.Debug("Load_Logos() - Using Logos -> '" + conf.StrLogos + "'");
      if (dr == null || index > dr.Length - 1)
      {
        LogMyFilms.Warn("Load_Logos() - Failed loading Logos - index not within current dataset ...");
        Clear_Logos();
        return;
      }

      DataRow row = dr[index];
      if ((ImgID2001 != null) && (MyFilms.conf.StrLogos))
      {
        try
        {
          MyFilmsDetail.setGUIProperty("logos_id2001", Logos.BuildLogos(row, "ID2001", ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, GetID));
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
          MyFilmsDetail.setGUIProperty("logos_id2002", Logos.BuildLogos(row, "ID2002", ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, GetID));
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
          MyFilmsDetail.setGUIProperty("logos_id2003", Logos.BuildLogos(row, "ID2003", ImgID2003.Height, ImgID2003.Width, ImgID2003.XPosition, ImgID2003.YPosition, GetID));
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
          MyFilmsDetail.setGUIProperty("logos_id2012", Logos.BuildLogos(row, "ID2012", ImgID2012.Height, ImgID2012.Width, ImgID2012.XPosition, ImgID2012.YPosition, GetID));
        }
        catch (Exception e)
        {
          LogMyFilms.Error("" + e.Message);
        }
      }
      else MyFilmsDetail.clearGUIProperty("logos_id2012");
    }

    public class FswHandler
    {
      public void OnEvent(Object source, FileSystemEventArgs Args)
      {
        // ToDo: Add required actions here ?
        //Console.Out.WriteLine(Args.ChangeType.ToString());
      }
    }

    delegate void SetDummyControlsForFacadeWorker(ViewContext viewContext);
    private void SetDummyControlsForFacade(ViewContext viewContext)
    {
      SetDummyControlsForFacadeWorker listlevelsetter = this.SetDummyControls;
      listlevelsetter.BeginInvoke(viewContext, null, null);
      // SetDummyControls(ViewContext);
    }

    private void SetDummyControls(ViewContext viewContext)
    {
      LogMyFilms.Debug("SetDummyControlsForFacade() setting ViewContext to '" + viewContext + "'");
      //if (dummyFacadeFilm == null || dummyFacadeHierarchy == null || dummyFacadeView == null || dummyFacadePerson == null || dummyFacadeIndex == null)
      //{
      //  LogMyFilms.Warn("SetDummyControlsForFacade() - : ViewContext = '" + viewContext + "' - Warning ! - null detected (controls missing in skin?)");
      //  // return;
      //}

      //if (viewContext == ViewContext.Movie) GUIControl.ShowControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeFilm);
      //else GUIControl.HideControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeFilm);
      //if (viewContext == ViewContext.MovieCollection) GUIControl.ShowControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeHierarchy);
      //else GUIControl.HideControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeHierarchy);
      //if (viewContext == ViewContext.Group) GUIControl.ShowControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeView);
      //else GUIControl.HideControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadeView);
      //if (viewContext == ViewContext.Person) GUIControl.ShowControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadePerson);
      //else GUIControl.HideControl(ID_MyFilms, (int)Controls.CTRL_DummyFacadePerson);
      //if (viewContext == ViewContext.Menu || viewContext == ViewContext.None)
      //{
      //  // Do Nothing ...
      //}

      switch (viewContext)
      {
        case ViewContext.Menu:
        case ViewContext.MenuAll:
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && !dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = true;
          break;
        case ViewContext.Movie:
          if (dummyFacadeFilm != null && !dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = true;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          break;
        case ViewContext.MovieCollection:
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && !dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = true;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          break;
        case ViewContext.Group:
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && !dummyFacadeView.Visible) dummyFacadeView.Visible = true;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          break;
        case ViewContext.Person:
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && !dummyFacadePerson.Visible) dummyFacadePerson.Visible = true;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          break;
        case ViewContext.None:
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          if (dummyFacadeIndex != null && dummyFacadeIndex.Visible) dummyFacadeIndex.Visible = false;
          break;
        default:
          LogMyFilms.Debug("SetDummyControlsForFacade() setting ViewContext to 'Default' (all false)");
          if (dummyFacadeFilm != null && dummyFacadeFilm.Visible) dummyFacadeFilm.Visible = false;
          if (dummyFacadeHierarchy != null && dummyFacadeHierarchy.Visible) dummyFacadeHierarchy.Visible = false;
          if (dummyFacadeView != null && dummyFacadeView.Visible) dummyFacadeView.Visible = false;
          if (dummyFacadePerson != null && dummyFacadePerson.Visible) dummyFacadePerson.Visible = false;
          if (dummyFacadeMenu != null && dummyFacadeMenu.Visible) dummyFacadeMenu.Visible = false;
          if (dummyFacadeIndex != null && dummyFacadeIndex.Visible) dummyFacadeIndex.Visible = false;
          break;
      }
      if (dummyFacadeIndex != null)
      {
        bool indexed = (conf.Boolindexed && !conf.Boolindexedreturn);
        if (!indexed && dummyFacadeIndex.Visible) dummyFacadeIndex.Visible = false;
        if (indexed && !dummyFacadeIndex.Visible) dummyFacadeIndex.Visible = true;
        // LogMyFilms.Debug("SetDummyControlsForFacade() setting dummyFacadeIndex to '" + indexed + "'");
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

      var dlgMenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_MENU);
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

      var pDlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
      if (pDlgNotify == null) return;

      // if image is empty, attempt to load the default
      string defaultLogo = Path.Combine(GUIGraphicsContext.Skin, @"Media\Logos\myfilms.png");
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

      var pDlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      if (pDlgOk == null) return;

      pDlgOk.SetHeading(heading);
      for (int i = 1; i <= lines.Length; i++)
      {
        pDlgOk.SetLine(i, lines[i - 1]);
      }
      pDlgOk.DoModal(GUIWindowManager.ActiveWindow);
    }

    private void CheckSkinInterfaceVersion()
    {
      string Skin = GUIGraphicsContext.Skin.Substring(GUIGraphicsContext.Skin.LastIndexOf("\\") + 1);
      if (currentSkin == null || currentSkin != Skin)
      {
        int VersionMajor = 0;
        int VersionMinor = 0;
        int VersionBuild = 0;
        int VersionRevision = 0;
        currentSkin = Skin;
        bool success = GetSkinInterfaceVersion(ref VersionMajor, ref VersionMinor, ref VersionBuild, ref VersionRevision);
        if (success)
        {
          LogMyFilms.Info("CheckSkinInterfaceVersion(): Current  Skin Interface Version = 'V" + VersionMajor + "." + VersionMinor + "' for skin '" + currentSkin + "'");
          LogMyFilms.Info("CheckSkinInterfaceVersion(): Required Skin Interface Version = 'V" + SkinInterfaceVersionMajor + "." + SkinInterfaceVersionMinor + "'");
          if (VersionMajor != SkinInterfaceVersionMajor || VersionMajor == 0)
          {
            InitMainScreen(false);
            this.ShowMessageDialog(GUILocalizeStrings.Get(10798624), "Your MyFilms skin is not compatible!", "Current Version: 'V" + VersionMajor + "." + VersionMinor + "'", "Required Version: 'V" + SkinInterfaceVersionMajor + "." + SkinInterfaceVersionMinor + "'");

            if (VersionMajor > 0) GUIWindowManager.ShowPreviousWindow(); // leave plugin
          }
          else if (VersionMinor != SkinInterfaceVersionMinor)
          {
            InitMainScreen(false);
            this.ShowMessageDialog(GUILocalizeStrings.Get(10798624), VersionMinor < SkinInterfaceVersionMinor
                ? "Your MyFilms skin should be updated to support all features !"
                : "Your MyFilms skin should be downgraded to properly work with this version !",
                "Current Version: 'V" + VersionMajor + "." + VersionMinor + "'", "Required Version: 'V" + SkinInterfaceVersionMajor + "." + SkinInterfaceVersionMinor + "'");
          }
        }
        else
        {
          LogMyFilms.Info("CheckSkinInterfaceVersion(): Cannot read Current Skin Interface Version for skin '" + currentSkin + "'");
          InitMainScreen(false);
          this.ShowMessageDialog(GUILocalizeStrings.Get(10798624), "Your MyFilms skin should be updated to support all features !", "Current Version: 'V" + VersionMajor + "." + VersionMinor + "'", "Required Version: 'V" + SkinInterfaceVersionMajor + "." + SkinInterfaceVersionMinor + "'");
        }
      }
      else
      {
        LogMyFilms.Info("CheckSkinInterfaceVersion(): skipping check - no skin change - Current Skin is still '" + currentSkin + "'");
      }
    }

    private bool GetSkinInterfaceVersion(ref int VersionMajor, ref int VersionMinor, ref int VersionBuild, ref int VersionRevision)
    {
      string _versionMajor = "";
      string _versionMinor = "";
      string _versionBuild = "";
      string _versionRevision = "";
      bool success = true;
      VersionMajor = 0;
      VersionMinor = 0;
      VersionBuild = 0;
      VersionRevision = 0;
      string name = GUIGraphicsContext.Skin.Substring(GUIGraphicsContext.Skin.LastIndexOf("\\") + 1);
      var doc = new XmlDocument();
      try
      {
        string file = Config.GetFile(Config.Dir.Skin, name, @"MyFilms.xml");
        doc.PreserveWhitespace = true;
        doc.Load(file);
        XmlElement root = doc.DocumentElement;
        if (root != null)
        {
          XmlNode controlsNode = GenericExtensions.SelectSingleNodeFast(root, "controls");
          if (controlsNode != null)
          {
            XmlNode settingsNode = GenericExtensions.SelectSingleNodeFast(controlsNode, "settings");
            if (settingsNode != null)
            {
              XmlNode skininterfaceversionNode = GenericExtensions.SelectSingleNodeFast(settingsNode, "skininterfaceversion");
              if (skininterfaceversionNode != null)
              {
                XmlNode versionNode = GenericExtensions.SelectSingleNodeFast(skininterfaceversionNode, "version");
                if (versionNode != null)
                {
                  XmlNode majorVersionNode = GenericExtensions.SelectSingleNodeFast(versionNode, "major");
                  if (majorVersionNode != null)
                    _versionMajor = majorVersionNode.InnerText;
                  XmlNode minorVersionNode = GenericExtensions.SelectSingleNodeFast(versionNode, "minor");
                  if (minorVersionNode != null)
                    _versionMinor = minorVersionNode.InnerText;
                  XmlNode buildVersionNode = GenericExtensions.SelectSingleNodeFast(versionNode, "build");
                  if (buildVersionNode != null)
                    _versionBuild = buildVersionNode.InnerText;
                  XmlNode revisionVersionNode = GenericExtensions.SelectSingleNodeFast(versionNode, "revision");
                  if (revisionVersionNode != null)
                    _versionRevision = revisionVersionNode.InnerText;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("OnPageLoad(): Cannot read Current Skin Interface Version for skin '" + currentSkin + "' - exception: " + ex.Message + ", Stacktrace: " + ex.StackTrace);
        _versionMajor = "";
        _versionMinor = "";
        _versionBuild = "";
        _versionRevision = "";
        VersionMajor = 0;
        VersionMinor = 0;
        VersionBuild = 0;
        VersionRevision = 0;
        success = false;
      }
      finally
      {
        doc = null;
      }
      if (!success) return false;
      success = int.TryParse(_versionMajor.Trim(), out VersionMajor);
      if (!success) return false;
      success = int.TryParse(_versionMinor.Trim(), out VersionMinor);
      if (!success) return false;
      success = int.TryParse(_versionBuild.Trim(), out VersionBuild);
      success = int.TryParse(_versionRevision.Trim(), out VersionRevision);
      return true;
    }

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

    private bool SaveLastView()
    { return SaveLastView(string.Empty); }
    private bool SaveLastView(string viewname)
    {
      if (null == viewname) return false;

      LogMyFilms.Debug("SaveLastView() called with '" + viewname + "', current ViewHistory stack count is: '" + ViewHistory.Count + "'");
      // Configuration conf = new Configuration();
      var state = new ViewState();

      state.Boolselect = conf.Boolselect;
      state.Boolreturn = conf.Boolreturn;
      state.Boolindexed = conf.Boolindexed;
      state.Boolindexedreturn = conf.Boolindexedreturn;
      state.IndexedChars = conf.IndexedChars;
      state.BoolReverseNames = conf.BoolReverseNames;
      state.BoolVirtualPathBrowsing = conf.BoolVirtualPathBrowsing;
      state.BoolShowEmptyValuesInViews = conf.BoolShowEmptyValuesInViews;

      state.StrSelect = conf.StrSelect;
      state.StrPersons = conf.StrPersons;
      state.StrTitleSelect = conf.StrTitleSelect;
      state.StrFilmSelect = conf.StrFilmSelect;
      state.ViewContext = conf.ViewContext;
      state.StrTxtView = conf.StrTxtView;
      state.StrTxtSelect = conf.StrTxtSelect;

      state.Wselectedlabel = conf.Wselectedlabel;
      state.WStrSort = conf.WStrSort;
      state.WStrSortSensCount = conf.WStrSortSensCount;
      state.BoolSortCountinViews = conf.BoolSortCountinViews;
      state.Wstar = conf.Wstar;

      state.StrLayOut = conf.StrLayOut;
      state.WStrLayOut = conf.WStrLayOut;
      state.StrLayOutInHierarchies = conf.StrLayOutInHierarchies;
      state.LastID = conf.LastID;

      state.IndexItem = (this.facadeFilms.SelectedItem > -1) ? ((MyFilms.conf.Boolselect) ? this.facadeFilms.SelectedListItemIndex : 0) : -1; //may need to check if there is no item selected and so save -1
      state.TitleItem = (this.facadeFilms.SelectedItem > -1) ? ((MyFilms.conf.Boolselect) ? this.facadeFilms.SelectedItem.ToString() : this.facadeFilms.SelectedListItem.Label) : string.Empty; //may need to check if there is no item selected and so save ""
      if (!string.IsNullOrEmpty(viewname))
      {
        if (ViewStateCache.ContainsKey(viewname)) ViewStateCache.Remove(viewname);
        ViewStateCache.Add(viewname, state);
        LogMyFilms.Debug("SaveLastView() - saved viewstate for '" + viewname + "'");
      }
      else
      {
        // ViewHistory.Add(state);
        ViewHistory.Push(state);
        LogMyFilms.Debug("SaveLastView() - saved state to history stack - stack count is now: '" + ViewHistory.Count + "'");
      }
      return true;
    }

    private bool RestoreLastView()
    { return RestoreLastView(string.Empty); }
    private bool RestoreLastView(string viewname)
    {
      LogMyFilms.Debug("RestoreLastView() called with '" + viewname + "', current ViewHistory stack count is: '" + ViewHistory.Count + "'");
      var state = new ViewState();
      if (!string.IsNullOrEmpty(viewname))
      {
        if (ViewStateCache.TryGetValue(viewname, out state))
        {
          conf.Boolselect = state.Boolselect;
          conf.Boolreturn = state.Boolreturn;
          conf.Boolindexed = state.Boolindexed;
          conf.Boolindexedreturn = state.Boolindexedreturn;
          conf.IndexedChars = state.IndexedChars;
          // conf.BoolReverseNames = state.BoolReverseNames; don't restore for Generic view state cache, as it is more a "global" setting
          // conf.BoolShowEmptyValuesInViews = state.BoolShowEmptyValuesInViews;

          //conf.StrSelect = state.StrSelect;
          //conf.StrPersons = state.StrPersons;
          //conf.StrTitleSelect = state.StrTitleSelect;
          //conf.StrFilmSelect = state.StrFilmSelect;
          //conf.ViewContext = state.ViewContext;
          //conf.StrTxtView = state.StrTxtView;
          //conf.StrTxtSelect = state.StrTxtSelect;

          conf.Wselectedlabel = state.Wselectedlabel;
          conf.WStrSort = state.WStrSort;
          conf.WStrSortSensCount = state.WStrSortSensCount;
          conf.BoolSortCountinViews = state.BoolSortCountinViews;
          conf.Wstar = state.Wstar;

          conf.StrLayOut = state.StrLayOut;
          conf.WStrLayOut = state.WStrLayOut;
          conf.StrLayOutInHierarchies = state.StrLayOutInHierarchies;
          conf.LastID = state.LastID;

          int IndexItem = state.IndexItem;
          string TitleItem = state.TitleItem;

          //IndexItem", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItem.ToString()) : "-1"); //may need to check if there is no item selected and so save -1
          //TitleItem", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItemLabel) : string.Empty); //may need to check if there is no item selected and so save ""
          LogMyFilms.Debug("SaveLastView() - restored viewstate for '" + viewname + "'");
          return true;
        }
        else return false;
      }
      else if (ViewHistory.Count > 0)
      {
        // state = ViewHistory.Last();
        state = ViewHistory.Pop() as ViewState;
        conf.Boolselect = state.Boolselect;
        conf.Boolreturn = state.Boolreturn;
        conf.Boolindexed = state.Boolindexed;
        conf.Boolindexedreturn = state.Boolindexedreturn;
        conf.IndexedChars = state.IndexedChars;
        conf.BoolReverseNames = state.BoolReverseNames;
        conf.BoolVirtualPathBrowsing = state.BoolVirtualPathBrowsing;
        conf.BoolShowEmptyValuesInViews = state.BoolShowEmptyValuesInViews;

        conf.StrSelect = state.StrSelect;
        conf.StrPersons = state.StrPersons;
        conf.StrTitleSelect = state.StrTitleSelect;
        conf.StrFilmSelect = state.StrFilmSelect;
        conf.ViewContext = state.ViewContext;
        conf.StrTxtView = state.StrTxtView;
        conf.StrTxtSelect = state.StrTxtSelect;

        conf.Wselectedlabel = state.Wselectedlabel;
        conf.WStrSort = state.WStrSort;
        conf.WStrSortSensCount = state.WStrSortSensCount;
        conf.BoolSortCountinViews = state.BoolSortCountinViews;
        conf.Wstar = state.Wstar;

        conf.StrLayOut = state.StrLayOut;
        conf.WStrLayOut = state.WStrLayOut;
        conf.StrLayOutInHierarchies = state.StrLayOutInHierarchies;
        conf.LastID = state.LastID;

        try { conf.StrIndex = state.IndexItem; }
        catch { conf.StrIndex = -1; }
        conf.StrTIndex = state.TitleItem;

        // ViewHistory.Remove(ViewHistory.Last());
        LogMyFilms.Debug("RestoreLastView() ViewHistory after restore is: '" + ViewHistory.Count + "'");
        return true;
      }
      return false;
    }

    private void DoBack()
    {
      LogMyFilms.Debug("DoBack() - Called with '" + NavigationStack.Count + "' items in navigation stack.");
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Reset(); stopwatch.Start();

      if (NavigationStack.Count > 0)
      {
        GUIControl.ClearControl(GetID, facadeFilms.GetID);
        //Fanartstatus(false);
        //currentFanartList.Clear();
        var obj = NavigationStack.Pop() as MyFilmsPlugin.Utils.NavigationObject;

        #region restore state properties
        mapSettings.ViewAs = (int)obj.CurrentView;
        // obj.SetCoverStatus(ref menucover, ref filmcover, ref viewcover, ref personcover, ref groupcover);
        menucover.Filename = obj.CoverStatus.MenuCover;
        // filmcover.Filename = obj.CoverStatus.FilmCover; // do not reset filmcover, as this "breaks" animations 
        viewcover.Filename = obj.CoverStatus.ViewCover;
        personcover.Filename = obj.CoverStatus.PersonCover;
        groupcover.Filename = obj.CoverStatus.GroupCover;
        conf.DbSelection = new string[] { obj.DbDfltSelect, obj.DbSelect, obj.DbField, obj.DbSort, obj.DbShowAll.ToString(), obj.DbExtraSort.ToString() };
        // Change_Layout_Action((int)obj.CurrentView); // switch here already the layout to BEFORE facade is populated !
        ShowPanel();  // switches to proper layout
        obj.SetItems(facadeFilms); // populate facade with former content
        obj.SetViewStatus(conf); // sets the context environment

        BtnSrtBy.IsEnabled = obj.SortButtonEnabled;
        BtnSrtBy.IsAscending = obj.SortButtonAsc;
        BtnSrtBy.Label = obj.SortButtonLabel;

        facadeFilms.SelectedListItemIndex = obj.Position;
        Prev_ItemID = (facadeFilms == null || facadeFilms.SelectedListItemIndex == -1) ? -1 : facadeFilms.SelectedListItem.ItemId;
        Prev_Label = (facadeFilms == null || facadeFilms.SelectedListItemIndex == -1) ? string.Empty : facadeFilms.SelectedListItem.Label;
        LogMyFilms.Debug("DoBack() - restore facade position - SelectedListItemIndex = '" + obj.Position + "', Prev_ItemID = '" + Prev_ItemID + "', Prev_Label = '" + Prev_Label + "'");
        //Prev_ItemID = -1;
        //Prev_Label = string.Empty;

        // GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_ListFilms, obj.Position);
        GUIPropertyManager.SetProperty("#currentmodule", obj.Title);
        GUIPropertyManager.SetProperty("#itemtype", obj.ItemType);
        GUIPropertyManager.SetProperty("#itemcount", facadeFilms.Count.ToString());
        GUIPropertyManager.SetProperty("#myfilms.nbobjects.value", obj.NbObjects);

        SetLabelView(conf.StrTxtView);
        SetLabelSelect(conf.StrTxtSelect);
        #endregion

        #region reload dataset threaded
        if (obj.LastDbUpdate < MyFilms.LastDbUpdate && conf.ViewContext != ViewContext.Menu && conf.ViewContext != ViewContext.MenuAll) // check if a full reload of the facade is required, e.g. due to watched status changes
        {
          #region restore facade by reloading facade
          LogMyFilms.Debug("DoBack() - DB was updated ! - cached date: '" + obj.LastDbUpdate.ToLongTimeString() + "', last update: '" + MyFilms.LastDbUpdate.ToLongTimeString() + "'");
          new Thread(delegate()
          {
            {
              //Loadfacade();
              //this.Refreshfacade();
              if (conf.Boolselect) // Groupviews / Persons
              {
                // Change_Layout_Action(MyFilms.conf.WStrLayOut);
                // SetLabelView(MyFilms.conf.StrTxtView); // Reload view name from configfile...
                getSelectFromDivx(conf.StrSelect, conf.WStrSort, conf.WStrSortSens, conf.Wstar, false, Prev_Label); // preserve index from last time
              }
              else
              {
                // Change_Layout_Action(MyFilms.conf.StrLayOut);
                // SetLabelView(MyFilms.conf.StrTxtView); // Reload view name from configfile...
                // conf.ViewContext = ViewContext.Movie;
                GetFilmList(Prev_Label);
              }
            }
            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
            {
              {
                Fanartstatus(true);
                //Prev_ItemID = -1;
                //Prev_Label = string.Empty;
                //ShowPanel();
                if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms);
                stopwatch.Stop();
                LogMyFilms.Debug("DoBack() - finished reloading dataset (" + (stopwatch.ElapsedMilliseconds) + " ms)");
              }
              return 0;
            }, 0, 0, null);
          }) { Name = "DoBackReloadDataset", IsBackground = true }.Start();
          #endregion
        }
        else
        {
          #region restore facade from navigation cache
          new Thread(delegate()
          {
            {
              if (Helper.FieldIsSet(obj.DbField))
                r = BaseMesFilms.ReadDataMovies(obj.DbDfltSelect, obj.DbSelect, obj.DbField, obj.DbSort, obj.DbShowAll, obj.DbExtraSort);
              // r = BaseMesFilms.ReadDataMovies(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens); // load dataset with filters
            }
            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
            {
              {
                Fanartstatus(true);
                Prev_ItemID = -1;
                Prev_Label = string.Empty;
                ShowPanel();
                if (!facadeFilms.Focus) GUIControl.FocusControl(GetID, (int)Controls.CTRL_ListFilms); // make sure, the details publisher is called !
                stopwatch.Stop();
                LogMyFilms.Debug("DoBack() - finished reloading dataset (" + (stopwatch.ElapsedMilliseconds) + " ms)");
                if (conf.ViewContext == ViewContext.Menu || conf.ViewContext == ViewContext.MenuAll) GetCountsForMenuView();
              }
              return 0;
            }, 0, 0, null);
          }) { Name = "DoBackReloadDataset", IsBackground = true }.Start();
          #endregion
        }
        #endregion

      }
      // stopwatch.Stop();
      LogMyFilms.Debug("DoBack() - finished restoring previous navigation cache (" + (stopwatch.ElapsedMilliseconds) + " ms)");
    }

    void ShowPanel()
    {
      //  case 0:
      //    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(101));
      //    this.facadeFilms.CurrentLayout = GUIFacadeControl.Layout.List;
      //    break;

      //List = 0,
      //SmallIcons = 1,
      //LargeIcons = 2,
      //Filmstrip = 3,
      //AlbumView = 4,
      //Playlist = 5,
      //CoverFlow = 6


      int itemIndex = facadeFilms.SelectedListItemIndex;
      switch (mapSettings.ViewAs)
      {
        case (int)Layout.LargeIcons:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
          break;
        case (int)Layout.AlbumView:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.AlbumView;
          break;
        case (int)Layout.SmallIcons:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
          break;
        case (int)Layout.List:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.List;
          break;
        case (int)Layout.Filmstrip:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
          break;
        case (int)Layout.CoverFlow:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
          break;
        default:
          facadeFilms.CurrentLayout = GUIFacadeControl.Layout.List;
          break;
      }
      if (itemIndex > -1)
      {
        GUIControl.SelectItemControl(GetID, facadeFilms.GetID, itemIndex);
      }
    }

    private void SaveListState(bool clear)
    {
      var stopwatch = new Stopwatch();
      stopwatch.Reset();
      stopwatch.Start();
      if (facadeFilms.ListLayout.ListItems.Count > 0)
      {
        mapSettings.ViewAs = (int)(Layout)facadeFilms.CurrentLayout;
        NavigationStack.Push(
          new MyFilmsPlugin.Utils.NavigationObject(
            facadeFilms.ListLayout,
            GUIPropertyManager.GetProperty("#currentmodule"),
            GUIPropertyManager.GetProperty("#itemtype"),
            GUIPropertyManager.GetProperty("#myfilms.nbobjects.value"),
            facadeFilms.SelectedListItemIndex,
            (Layout)mapSettings.ViewAs,
            conf,
            BtnSrtBy,
            new CoverState(menucover.Filename, filmcover.Filename, viewcover.Filename, personcover.Filename, groupcover.Filename),
            MyFilms.LastDbUpdate
            ));
      }
      if (clear)
      {
        // ClearLabels("Curent");
        GUIControl.ClearControl(GetID, facadeFilms.GetID);
        //MyFilms.temp_player.Reset();
        //MyFilms.temp_player.GetPlaylist(PlayListType.PLAYLIST_MUSIC_VIDEO).Clear();
      }
      stopwatch.Stop();
      LogMyFilms.Debug("SaveListState() - finished saving current state to navigation cache (clear = '" + clear + "') (" + (stopwatch.ElapsedMilliseconds) + " ms)");
    }
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


  public class GUIMovieListItem : GUIListItem
  {
    public GUIMovieListItem(string strLabel) : base(strLabel) { }

    public object Item
    {
      get { return _Item; }
      set
      {
        _Item = value;
        var notifier = value as INotifyPropertyChanged;
        if (notifier != null) notifier.PropertyChanged += (s, e) =>
        {
          //if (s is TraktMovie.MovieImages && e.PropertyName == "PosterImageFilename")
          //  SetImageToGui((s as TraktMovie.MovieImages).PosterImageFilename);
          //if (s is TraktMovie.MovieImages && e.PropertyName == "FanartImageFilename")
          //  UpdateCurrentSelection();
        };
      }
    } protected object _Item;

    /// <summary>
    /// Loads an Image from memory into a facade item
    /// </summary>
    /// <param name="imageFilePath">Filename of image</param>
    protected void SetImageToGui(string imageFilePath)
    {
      if (string.IsNullOrEmpty(imageFilePath)) return;

      // determine the overlay to add to poster
      // [...]
      // get a reference to a MediaPortal Texture Identifier
      //string suffix = mainOverlay.ToString().Replace(", ", string.Empty) + Enum.GetName(typeof(RatingOverlayImage), ratingOverlay);
      //string texture = GUIImageHandler.GetTextureIdentFromFile(imageFilePath, suffix);

      // build memory image
      //Image memoryImage = null;
      //if (mainOverlay != MainOverlayImage.None || ratingOverlay != RatingOverlayImage.None)
      //{
      //  memoryImage = GUIImageHandler.DrawOverlayOnPoster(imageFilePath, mainOverlay, ratingOverlay);
      //  if (memoryImage == null) return;

      //  // load texture into facade item
      //  if (GUITextureManager.LoadFromMemory(memoryImage, texture, 0, 0, 0) > 0)
      //  {
      //    ThumbnailImage = texture;
      //    IconImage = texture;
      //    IconImageBig = texture;
      //  }
      //}
      //else
      //{
      //  ThumbnailImage = imageFilePath;
      //  IconImage = imageFilePath;
      //  IconImageBig = imageFilePath;
      //}

      // if selected and is current window force an update of thumbnail
      UpdateCurrentSelection();
    }

    protected void UpdateCurrentSelection()
    {
      var window = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow);
      if (window != null)
      {
        GUIListItem selectedItem = GUIControl.GetSelectedListItem(MyFilms.ID_MyFilms, 50);
        if (selectedItem == this)
        {
          GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, ItemId, 0, null));
        }
      }
    }
  }

}