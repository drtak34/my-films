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
  using System.Text;
  using System.Threading;
  using System.Data;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Text.RegularExpressions;
  using System.Windows.Forms;

  using MediaPortal.Ripper;

  using OnlineVideos.MediaPortal1;

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
  using MyFilmsPlugin.DataBase;

  using MyFilmsPlugin.MyFilms.Utils;
  using SQLite.NET;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;
  using MediaInfo = Grabber.MediaInfo;

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
            CTRL_GuiWaitCursor = 2080,
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
        [SkinControlAttribute((int)Controls.CTRL_GuiWaitCursor)]
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

        public class Searchtitles
        {
          public string SearchTitle;
          public string FanartTitle;
          public string MasterTitle;
          public string SecondaryTitle;
          public string SortTitle;
          public string TranslatedTitle;
          public string OriginalTitle;
          public string FormattedTitle;
          public string MovieDirectoryTitle;
          public string MovieFileTitle;

          public string Director;
          public int year;
        } ;

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
        public static bool trailerPlayed = false;
        // public static bool trailerScrobblingMode = false;
        private bool PlayBackEvents_Subscribed = false;
        private bool doUpdateDetailsViewByFinishEvent = false;

        #region Enums
        private enum Grabber_DB_Field
        {
                //case "Number":
                //case "Checked":
                //case "MediaLabel":
                //case "MediaType":
                //case "Source":
                //case "Date":
                //case "Borrower":
                //case "Rating":
                //case "OriginalTitle":
                //case "TranslatedTitle":
                //case "FormattedTitle":
                //case "Director":
                //case "Producer":
                //case "Country":
                //case "Category":
                //case "Year":
                //case "Length":
                //case "Actors":
                //case "URL":
                //case "Description":
                //case "Comments":
                //case "VideoFormat":
                //case "VideoBitrate":
                //case "AudioFormat":
                //case "AudioBitrate":
                //case "Resolution":
                //case "Framerate":
                //case "Languages":
                //case "Subtitles":
                //case "DateAdded":
                //case "Size":
                //case "Disks":
                //case "Picture":
                //case "Certification":
                //case "Writer":
                //case "Watched":
                //case "WatchedDate":
                //case "IMDB_Id":
                //case "TMDB_Id":
                //case "SourceTrailer":
                //case "TagLine":
                //case "Tags":
                //case "Aspectratio":
                //case "RatingUser":
                //case "Fanart":
                //case "Studio":
                //case "IMDB_Rank":
                //case "IsOnline":
                //case "Edition":
                //case "IsOnlineTrailer":
        }

        private enum TraktGUIWindows
        {
          Main = 87258,
          Calendar = 87259,
          Friends = 87260,
          Recommendations = 87261,
          RecommendationsShows = 87262,
          RecommendationsMovies = 87263,
          Trending = 87264,
          TrendingShows = 87265,
          TrendingMovies = 87266,
          WatchedList = 87267,
          WatchedListShows = 87268,
          WatchedListEpisodes = 87269,
          WatchedListMovies = 87270,
          Settings = 87271,
          SettingsAccount = 87272,
          SettingsPlugins = 87273,
          SettingsGeneral = 87274,
          Lists = 87275,
          ListItems = 87276,
          RelatedMovies = 87277,
          RelatedShows = 87278,
          Shouts = 87280
        }
        public enum GrabType
        {
          All,
          Details,
          Cover,
          MultiCovers,
          Fanart,
          Photos,
          Person,
          Trailers
        }
        #endregion

        public static event WatchedEventDelegate WatchedItem;
        public delegate void WatchedEventDelegate(MFMovie movie, bool watched, int count);

        public static event RatingEventDelegate RateItem;
        public delegate void RatingEventDelegate(MFMovie movie, string rating);

        public static event MovieStartedEventDelegate MovieStarted;
        public delegate void MovieStartedEventDelegate(MFMovie movie);

        public static event MovieStoppedEventDelegate MovieStopped;
        public delegate void MovieStoppedEventDelegate(MFMovie movie);

        public static event MovieWatchedEventDelegate MovieWatched;
        public delegate void MovieWatchedEventDelegate(MFMovie movie);

        // MF event for async updates
        public static event DetailsUpdatedEventDelegate DetailsUpdated;
        public delegate void DetailsUpdatedEventDelegate(bool searchPicture);

        // MF event for trailer completed (to start next)
        public static event TrailerEndedEventDelegate TrailerEnded;
        public delegate void TrailerEndedEventDelegate(string filename);

        public static FileSystemWatcher Trailerwatcher = new FileSystemWatcher();

        static MyFilmsDetail()
        {
          playlistPlayer = PlayListPlayer.SingletonPlayer;
        }

        #endregion
        public static BackgroundWorker downloadingWorker = new BackgroundWorker(); // to do the  downloading from a queue for e.g. actor images


        static Queue<DBPersonInfo> PersonstoDownloadQueue = new Queue<DBPersonInfo>();
        private object locker = new object();

        public MyFilmsDetail()
        {
          //
          // TODO: Add constructor logic here
          //
          // lets set up the downloader            
          downloadingWorker.WorkerSupportsCancellation = true;
          downloadingWorker.WorkerReportsProgress = true;
          downloadingWorker.DoWork += new DoWorkEventHandler(downloadingWorker_DoWork);
          downloadingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(downloadingWorker_RunWorkerCompleted);
          setDownloadStatus();

        }

        void downloadingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
          // ToDo: Add Updater logic here
        }

        static void downloadingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
          LogMyFilms.Debug("downloadingWorker_RunWorkerCompleted() - Finished loading person  images !");  
          setDownloadStatus();
        }

        static void downloadingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
          grabber.TheMoviedb tmdbapi = new grabber.TheMoviedb();
          do
          {
            if (downloadingWorker.CancellationPending)
            {
              LogMyFilms.Debug("cancel person image download...");
              return;
            }

            DBPersonInfo f;
            setDownloadStatus();
            lock (PersonstoDownloadQueue)
            {
              f = PersonstoDownloadQueue.Dequeue();
            }
            bool bDownloadSuccess = true;

            try
            {
              List<DBPersonInfo> personlist = tmdbapi.getPersonsByName(f.Name, false, "en");
              if (personlist.Count == 0)
              {
                LogMyFilms.Debug("downloadingWorker_DoWork() - Person '" + f.Name + "' not found, remaining items: '" + PersonstoDownloadQueue.Count + "'");
                bDownloadSuccess = false;
              }
              else
              {
                f = personlist[0];

                if (f != null && !System.IO.File.Exists(Path.Combine(MyFilms.conf.StrPathArtist, f.Name)))
                {
                  if (f.Images.Count == 0)
                  {
                    LogMyFilms.Debug("downloadingWorker_DoWork() - Person '" + f.Name + "' found, but no images available on TMDB! - remaining items: '" + PersonstoDownloadQueue.Count + "'");
                    bDownloadSuccess = false;
                  }
                  else
                  {
                    string filename = Path.Combine(MyFilms.conf.StrPathArtist, f.Name);
                    //grabber.DBPersonInfo persondetails = new DBPersonInfo();
                    //persondetails = tmdbapi.getPersonsById(f.Id, string.Empty);
                    //LogMyFilms.Info("Person Artwork - " + f.Images.Count + " Images found for '" + f.Name + "'");
                    LogMyFilms.Debug("downloadingWorker_DoWork() - TMDB Image found for person '" + f.Name + "', URL = '" + f.Images[0] + "' - remaining items: '" + PersonstoDownloadQueue.Count + "'");
                    string filename1person = Grabber.GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, f.Images[0], f.Name, false, true, out filename);
                    LogMyFilms.Debug("Person Image '" + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + "' downloaded for '" + f.Name + "', path = '" + filename1person + "', filename = '" + filename + "'");

                    if (downloadingWorker.CancellationPending)
                    {
                      bDownloadSuccess = false;
                      LogMyFilms.Debug("cancel person image download - last person: " + f.Name);
                      lock (PersonstoDownloadQueue)
                      {
                        PersonstoDownloadQueue.Clear();
                      }
                      return;
                    }
                    System.Windows.Forms.Application.DoEvents();
                  }
                }
              }

              if (!bDownloadSuccess) // try IMDB, if TMDB was not successful !
              {
                LogMyFilms.Debug("downloadingWorker_DoWork() - TMDB unsuccessful - try IMDB ...");
                bDownloadSuccess = true;
                IMDB _imdb = new IMDB();
                _imdb.FindActor(f.Name);

                if (_imdb.Count == 0)
                {
                  LogMyFilms.Debug("downloadingWorker_DoWork() - Person '" + f.Name + "' not found, remaining items: '" + PersonstoDownloadQueue.Count + "'");
                  bDownloadSuccess = false;
                }
                else
                {
                  if (_imdb[0].URL.Length != 0)
                  {
                    IMDBActor imdbActor = new IMDBActor();
                    _imdb.GetActorDetails(_imdb[0], false, out imdbActor);
                    if (imdbActor.ThumbnailUrl.Length > 0)
                    {
                      LogMyFilms.Debug("downloadingWorker_DoWork() - IMDB Image found for person '" + f.Name + "', URL = '" + imdbActor.ThumbnailUrl + "' - remaining items: '" + PersonstoDownloadQueue.Count + "'");
                      string filename = Path.Combine(MyFilms.conf.StrPathArtist, f.Name);
                      string filename1person = GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, imdbActor.ThumbnailUrl, f.Name, false, true, out filename);
                      LogMyFilms.Debug("Person Image '" + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + "' downloaded for '" + f.Name + "', path = '" + filename1person + "', filename = '" + filename + "'");
                    }
                    else
                    {
                      LogMyFilms.Debug("downloadingWorker_DoWork() - Person '" + f.Name + "' found, but no images available on IMDB! - remaining items: '" + PersonstoDownloadQueue.Count + "'");
                      bDownloadSuccess = false;
                    }
                  }
                  else
                  {
                    LogMyFilms.Debug("downloadingWorker_DoWork() - Person '" + f.Name + "' found, but no images available on IMDB! - remaining items: '" + PersonstoDownloadQueue.Count + "'");
                    bDownloadSuccess = false;
                  }
                }
                if (downloadingWorker.CancellationPending)
                {
                  LogMyFilms.Debug("cancel person image download - last person: " + f.Name);
                  lock (PersonstoDownloadQueue)
                  {
                    PersonstoDownloadQueue.Clear();
                  }
                  return;
                }
              }

              LogMyFilms.Debug("Result of image download for '" + f.Name + "': success = '" + bDownloadSuccess + "'");
              if (bDownloadSuccess) downloadingWorker.ReportProgress(0, f.Name);
            }
            catch (Exception ex)
            {
              LogMyFilms.DebugException("Error loading person image: '" + ex.Message + "'", ex);
            }
          }
          while (PersonstoDownloadQueue.Count > 0 && !downloadingWorker.CancellationPending);
        }

        static void setDownloadStatus()
        {
          // LogMyFilms.Debug("setDownloadStatus() - remaining items in queue = '" + PersonstoDownloadQueue.Count + "'");
          lock (PersonstoDownloadQueue)
          {
            if (PersonstoDownloadQueue.Count > 0)
              MyFilmsDetail.setGUIProperty("details.downloadstatus", "Person Images to Download: " + PersonstoDownloadQueue.Count);                    
            else
              MyFilmsDetail.clearGUIProperty("details.downloadstatus");
          }
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
          LogMyFilms.Debug("MyFilmsDetail.Init() started.");
          LogMyFilms.Debug("MyFilmsDetail.Init() ended.");
          GUIWindowManager.Receivers += new SendMessageHandler(GUIWindowManager_OnNewMessage);
          return Load(GUIGraphicsContext.Skin + @"\MyFilmsDetail.xml");
        }

        protected override void OnPageLoad()
        {
            LogMyFilms.Debug("OnPageLoad() started.");

            if (!PlayBackEvents_Subscribed)
            {
              //g_Player.PlayBackStarted -= new g_Player.StartedHandler(OnPlayBackStarted);
              //g_Player.PlayBackEnded -= new g_Player.EndedHandler(OnPlayBackEnded);
              //g_Player.PlayBackStopped -= new g_Player.StoppedHandler(OnPlayBackStopped);
              g_Player.PlayBackStarted += new g_Player.StartedHandler(OnPlayBackStarted);
              g_Player.PlayBackEnded += new g_Player.EndedHandler(OnPlayBackEnded);
              g_Player.PlayBackStopped += new g_Player.StoppedHandler(OnPlayBackStopped);
              // Subscribe to GUI Events
              MyFilmsDetail.DetailsUpdated += new MyFilmsDetail.DetailsUpdatedEventDelegate(OnDetailsUpdated);

              PlayBackEvents_Subscribed = true;
            }

            //bool result = base.OnMessage(messageType);
            if (ImgDetFilm != null)
              if (ImgDetFilm.IsVisible)
                ImgDetFilm.Refresh();
              else if (ImgDetFilm2 != null)
                if (ImgDetFilm2.IsVisible)
                  ImgDetFilm2.Refresh();

            //base.OnMessage(messageType); // Guzzi: Removing does not work properly...

            setGUIProperty("menu.overview", GUILocalizeStrings.Get(10798751));
            setGUIProperty("menu.description", GUILocalizeStrings.Get(10798752));
            setGUIProperty("menu.comments", GUILocalizeStrings.Get(10798753));
            setGUIProperty("menu.actors", GUILocalizeStrings.Get(10798754));
            setGUIProperty("menu.techinfos", GUILocalizeStrings.Get(10798755));

            BtnFirst.Label = GUILocalizeStrings.Get(1079872); //GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnFirst, GUILocalizeStrings.Get(1079872));
            BtnLast.Label = GUILocalizeStrings.Get(1079873); //GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLast, GUILocalizeStrings.Get(1079873));

            wGetID = GetID;
            GUIControl.ShowControl(GetID, 35);
            // ToDo: Should be unhidden, if ActorThumbs are implemented
            GUIControl.HideControl(GetID, (int)Controls.CTRL_ActorMultiThumb);

            m_directory.SetExtensions(MediaPortal.Util.Utils.VideoExtensions);
            if (MyFilms.conf.StrTxtSelect.Length == 0)
              clearGUIProperty("select"); //GUIControl.HideControl(GetID, (int)Controls.CTRL_TxtSelect);
            else
              setGUIProperty("select", MyFilms.conf.StrTxtSelect.Replace(MyFilms.conf.TitleDelim, @"\")); //GUIControl.ShowControl(GetID, (int)Controls.CTRL_TxtSelect);
            afficher_init(MyFilms.conf.StrIndex); //Populate DataSet & Convert ItemId passed in initially to Index within DataSet
            int TitlePos = (MyFilms.conf.StrTitleSelect.Length > 0) ? MyFilms.conf.StrTitleSelect.Length + 1 : 0; //only display rest of title after selected part common to group


            //if (ImgFanartDir != null) ImgFanartDir.TexturePath = "";
            //if (ImgFanart != null) ImgFanart.SetFileName(MyFilms.conf.DefaultFanartImage);
            afficher_detail(true);

            MyFilms.conf.LastID = MyFilms.ID_MyFilmsDetail;

            if (MyFilms.conf.PersonsEnableDownloads)
            {
              AddPersonsToDownloadQueue(); // add persons of current movie to download queue
              downloadingWorker.ProgressChanged += new ProgressChangedEventHandler(downloadingWorker_ProgressChanged);
            }

            setProcessAnimationStatus(false, m_SearchAnimation);

            if (MyFilms.conf.AutoRegisterTrailer) AutoRegisterTrailer("");
            LogMyFilms.Debug("OnPageLoad() finished.");
            base.OnPageLoad(); // let animations run!
        }


        private void AutoRegisterTrailer(string newTrailerFile)
        {
          LogMyFilms.Debug("AutoRegisterTrailer() - called - enabled = '" + MyFilms.conf.AutoRegisterTrailer + "', newTrailerFile: '" + newTrailerFile + "'");
          if (MyFilms.conf.AutoRegisterTrailer)
          {
            if (newTrailerFile.Length > 0)
            {
              if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
              {
                LogMyFilms.Debug("AutoRegisterTrailer() - Old Trailersourcepath: '" + MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer] + "'");
                string trailersourcepath = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString();
                if (!trailersourcepath.Contains(newTrailerFile))
                {
                  if (trailersourcepath.Length > 0)
                    trailersourcepath = trailersourcepath + ";" + newTrailerFile;
                  else
                    trailersourcepath = newTrailerFile;
                  LogMyFilms.Debug("AutoRegisterTrailer() - Added Trailer to Trailersource: '" + newTrailerFile + "'");
                }
                MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer] = trailersourcepath;
                LogMyFilms.Debug("AutoRegisterTrailer() - New Trailersourcepath    : '" + MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer] + "'");
                Update_XML_database();
                LogMyFilms.Debug("AutoRegisterTrailer() - Database Updated !!!!");
              }
            }
            else
            {
              new System.Threading.Thread(delegate()
              {
                {
                  //GUIWaitCursor.Init(); GUIWaitCursor.Show();
                  SearchTrailerLocal((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, false);
                  // Todo: Update DB, if intended
                  // GUIWaitCursor.Hide();
                }
                GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
                {
                  {
                    // this after thread finished ...
                  }
                  return 0;
                }, 0, 0, null);
              }) { Name = "MyFilmsAutoRegisterTrailer", IsBackground = true }.Start();
            }
          }
        }

        protected override void OnPageDestroy(int new_windowId)
        {
          LogMyFilms.Debug("MyFilmsDetail.OnPageDestroy(" + new_windowId.ToString() + ") started.");
          if (MyFilms.conf.PersonsEnableDownloads)
          {
            if (downloadingWorker.IsBusy) downloadingWorker.CancelAsync();
            // while (downloadingWorker.IsBusy) System.Windows.Forms.Application.DoEvents();
            // downloadingWorker = null;
            downloadingWorker.ProgressChanged -= new ProgressChangedEventHandler(downloadingWorker_ProgressChanged);
          }
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
            LogMyFilms.Debug("SaveLastActiveModule - module {0}", currentmoduleid);
            LogMyFilms.Debug("SaveLastActiveModule - fullscreen {0}", currentmodulefullscreen);
          }
          LogMyFilms.Debug("MyFilms.OnPageDestroy(" + new_windowId.ToString() + ") completed.");
          base.OnPageDestroy(new_windowId);
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
              //if (BtnMaj.Focus) // first switch to main details window before returning to main window ....
              //{
              //  GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay);
              //  return;
              //}
              MyFilms.conf.LastID = MyFilms.ID_MyFilms;
              // GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilms);
              GUIWindowManager.ShowPreviousWindow();
              return;
            }

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU) // context menu for options  like PlayTrailers or Updates
            {
                LogMyFilms.Debug("MyFilmsDetail : ACTION_CONTEXT_MENU detected ! ");
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

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PLAY || actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_MUSIC_PLAY)
            {
              Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, false);
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
        }
        //---------------------------------------------------------------------------------------
        //   Handle posted Messages
        //---------------------------------------------------------------------------------------
        public override bool OnMessage(GUIMessage messageType)
        {
            LogMyFilms.Debug("OnMessage - MessageType: '" + messageType.Message.ToString() + "'");
            int dControl = messageType.TargetControlId;
            int iControl = messageType.SenderControlId;
            switch (messageType.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
                    base.OnMessage(messageType); 
                    return true; 
                    //break;

                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
                    base.OnMessage(messageType); 
                    return true;
                    //break;

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
                        Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, false);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay1Description)
                    // Search File to play
                    {
                      Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, false);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay2Comment)
                    // Search File to play
                    {
                      Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, false);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay3Persons)
                    // Search File to play
                    {
                      Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, false);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay4TecDetails)
                    // Search File to play
                    {
                      Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, false);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay5)
                    // Search File to play
                    {
                      Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, false);
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
                        Change_Menu("mainmenu");  // was: Update_XML_Items();
                        // GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay); // Added to return to main view after menu
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlayTrailer)
                    // Search Trailer File to play
                    {
                      if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
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
                        Launch_Movie(MyFilms.conf.StrIndex, GetID, m_SearchAnimation, false);
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
                      GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilmsActors);
                      // Hier Aktivitäten wie z.b. ListControl für Actors?
                      //GUIWindowManager.ShowPreviousWindow();
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
            if (downloadingWorker.IsBusy) downloadingWorker.CancelAsync(); // cancel person image download, if user does select any (other) action
            
            string StrUpdItem1 = null;
            string StrUpdText1 = null;
            string StrUpdDflT1 = null;
            string StrUpdItem2 = null;
            string StrUpdText2 = null;
            string StrUpdDflT2 = null;
            int ItemID;

            // Read Info from MyFilms Configfile
            //XmlConfig XmlConfig = new XmlConfig();
            //StrUpdItem1 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdItem1", "");
            //StrUpdText1 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdText1", "");
            //StrUpdDflT1 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdDflT1", "");
            //StrUpdItem2 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdItem2", "");
            //StrUpdText2 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdText2", "");
            //StrUpdDflT2 = XmlConfig.ReadXmlConfig("MyFilms", global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, "AntUpdDflT2", "");

            AntMovieCatalog ds = new AntMovieCatalog();
            ItemID = (int)MyFilms.r[MyFilms.conf.StrIndex]["Number"]; //set unique id num (ant allows it to be non-unique but that is a bad idea)
            //May wish to completely re-load the dataset before updating any fields if used in multi-user system, but would req concurrency locks etc so...

            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);

            GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            List<string> choiceViewMenu = new List<string>();

            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            if (null == keyboard) return;
            keyboard.Reset();

            string title = string.Empty; // variable for searchtitle creation
            string mediapath = string.Empty; // variable for searchpath creation (for nfo/xml/xbmc reader)
            Searchtitles sTitles; // variable to get all searchtitles 
            switch (choiceView)
            {
              case "mainmenu":

                if (dlgmenu == null) return;
                dlgmenu.Reset();
                dlgmenu.SetHeading(GUILocalizeStrings.Get(10798701)); // update menu

                dlgmenu.Add(GUILocalizeStrings.Get(10798704)); //trailer menu "Trailer ..."
                choiceViewMenu.Add("trailermenu");

                if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
                {
                  if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
                  {
                    if (GetWatchedCount(MyFilms.conf.StrIndex, MyFilms.conf.StrUserProfileName) > 0) // show only the required option
                      dlgmenu.Add(GUILocalizeStrings.Get(1079895)); // set unwatched
                    else dlgmenu.Add(GUILocalizeStrings.Get(1079894)); // set watched
                  }
                  else
                  {
                    if (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrWatchedField].ToString().ToLower() !=
                        MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower()) // show only the required option
                      dlgmenu.Add(GUILocalizeStrings.Get(1079895)); // set unwatched
                    else dlgmenu.Add(GUILocalizeStrings.Get(1079894)); // set watched
                  }
                  choiceViewMenu.Add("togglewatchedstatus");
                }

                dlgmenu.Add(GUILocalizeStrings.Get(931)); //rating
                choiceViewMenu.Add("rating");

                if (MyFilms.conf.StrSuppress || MyFilms.conf.StrSuppressManual)
                {
                  dlgmenu.Add(GUILocalizeStrings.Get(1079830)); // Delete movie ...
                  choiceViewMenu.Add("delete");
                }

                dlgmenu.Add(GUILocalizeStrings.Get(10798702)); // Updates ...
                choiceViewMenu.Add("updatesmenu");

                if (File.Exists(GUIGraphicsContext.Skin + @"\MyFilmsCoverManager.xml"))
                {
                  dlgmenu.Add(GUILocalizeStrings.Get(10798763)); // Cover Manager ...
                  choiceViewMenu.Add("covermanager");
                }

                dlgmenu.Add(GUILocalizeStrings.Get(10798767)); // Fanart Manager ...
                choiceViewMenu.Add("fanartmanager");

                //dlgmenu.Add(GUILocalizeStrings.Get(10798703)); // Fanart & Cover ...
                //choiceViewMenu.Add("fanartcovermenu");

                if (Helper.IsTraktAvailableAndEnabled) //  && MyFilms.conf.AllowTraktSync
                {
                  dlgmenu.Add(GUILocalizeStrings.Get(10798775)); // Trakt ...
                  choiceViewMenu.Add("trakt");
                }
                else LogMyFilms.Debug("trakt not found or wrong version - disabling context entry");

                if (Helper.IsSubCentralAvailableAndEnabled)
                {
                  dlgmenu.Add(GUILocalizeStrings.Get(10798707)); // Subtitles ...
                  choiceViewMenu.Add("subtitles");
                }
                else LogMyFilms.Debug("Subcentral not found or wrong version - disabling context entry");

                dlgmenu.DoModal(GetID);
                if (dlgmenu.SelectedLabel == -1)
                {
                  // GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay); // Added to return to main view after menu // Removed as it's causing an exception
                  // return;
                  break;
                }
                Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                break;

              case "playtrailer":
                // first check, if trailer files are available, offer options
                //if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
                if (
                  !string.IsNullOrEmpty(
                    MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim()))
                {
                  trailerPlayed = true;
                  Launch_Movie_Trailer(MyFilms.conf.StrIndex, GetID, m_SearchAnimation);
                }
                else
                {
                  // Can add autosearch&register logic here before try starting trailers

                  GUIDialogYesNo dlgYesNotrailersearch = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                  dlgYesNotrailersearch.SetHeading(GUILocalizeStrings.Get(10798704)); //trailer
                  dlgYesNotrailersearch.SetLine(1, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrSTitle].ToString()); //video title
                  dlgYesNotrailersearch.SetLine(2, GUILocalizeStrings.Get(10798737)); //no video found locally
                  dlgYesNotrailersearch.SetLine(3, GUILocalizeStrings.Get(10798739)); // Search local trailers  and update DB ?
                  dlgYesNotrailersearch.DoModal(GetID);
                  //dlgYesNotrailersearch.DoModal(GUIWindowManager.ActiveWindow);
                  if (dlgYesNotrailersearch.IsConfirmed)
                  {
                    setProcessAnimationStatus(true, m_SearchAnimation);
                    //LogMyFilms.Debug("(SearchTrailerLocal) SelectedItemInfo from (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(): '" + (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() + "'"));
                    LogMyFilms.Debug("(Auto search trailer after selecting PLAY) title: '" + (MyFilms.r[MyFilms.conf.StrIndex].ToString() + "'"));
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
                string path = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
                if (path.Contains(";")) path = path.Substring(0, path.IndexOf(";"));
                if (path.Contains("\\")) path = path.Substring(0, path.LastIndexOf("\\"));

                switch (choiceView)
                {
                  case "playtraileronlinevideos":
                    site = "YouTube";
                    titleextension = " " + MyFilms.r[MyFilms.conf.StrIndex]["Year"] + " trailer" + ((MyFilms.conf.GrabberOverrideLanguage.Length > 0) ? (" " + MyFilms.conf.GrabberOverrideLanguage) : "");
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

                // Load OnlineVideo Plugin with Searchparameters for YouTube and movie to Search ... 
                // OV reference for parameters: site:<sitename>|category:<categoryname>|search:<searchstring>|VKonfail:<true,false>|return:<Locked,Root>
                //if (PluginManager.IsPluginNameEnabled2("OnlineVideos"))
                if (Helper.IsOnlineVideosAvailableAndEnabled)
                {
                  title = GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");

                  string OVstartparams = "site:" + site + "|category:|search:" + title + titleextension + "|return:Locked" + "|downloaddir:" + path + "|downloadmenuentry:" + GUILocalizeStrings.Get(10798749) + " (" + title + ")";
                  //GUIPropertyManager.SetProperty("Onlinevideos.startparams", OVstartparams);
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", site);
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", title + titleextension);
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "Locked");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloaddir", path);
                  //GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadfilename", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadmenuentry", GUILocalizeStrings.Get(10798749) + " (" + title + ")"); // download to movie directory

                  InitTrailerwatcher(path); // enable Trailerwatcher for the movie path, in case the user is downloading a trailer there ...

                  if (Helper.IsOnlineVideosAvailableAndEnabledV12) InitOVEventHandler();
                  else LogMyFilms.Error("Error subscribing to 'VideoDownloaded' event from OnlineVideos - you need OV V1.2+ installed and enabled !");

                  LogMyFilms.Debug("Starting OnlineVideos with '" + OVstartparams + "'");
                  // should this be set here to make original movie doesn't get set to watched??
                  // trailerPlayed = true;
#if MP11
                  GUIWindowManager.ActivateWindow(MyFilms.ID_OnlineVideos, false); // 4755 is ID for OnlineVideos
#else
                  GUIWindowManager.ActivateWindow((int)MyFilms.ExternalPluginWindows.OnlineVideos, OVstartparams); // GUIWindowManager.ActivateWindow((int)MyFilms.ID_OnlineVideos, OVstartparams);
#endif
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloaddir", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadfilename", "");
                  GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadmenuentry", "");
                }
                else
                {
                  ShowMessageDialog("MyFilms", "OnlineVideo plugin not installed or wrong version", "Minimum Version required: " + MyFilmsSettings.GetRequiredMinimumVersion(MyFilmsSettings.MinimumVersion.OnlineVideos));
                }
                break;

              case "togglewatchedstatus":

                if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
                {
                  if (MyFilmsDetail.GetWatchedCount(MyFilms.conf.StrIndex, MyFilms.conf.StrUserProfileName) > 0)
                  {
                    // MyFilmsDetail.Watched_Toggle((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, false);
                    MyFilmsDetail.Watched_Toggle(MyFilms.conf.StrIndex, false);
                  }
                  else
                  {
                    // MyFilmsDetail.Watched_Toggle((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true);
                    MyFilmsDetail.Watched_Toggle(MyFilms.conf.StrIndex, true);
                  }
                }
                else
                {
                  if (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrWatchedField].ToString().ToLower() !=
                      MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower())
                  {
                    // MyFilmsDetail.Watched_Toggle((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, false);
                    MyFilmsDetail.Watched_Toggle(MyFilms.conf.StrIndex, false);
                  }
                  else
                  {
                    // MyFilmsDetail.Watched_Toggle((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true);
                    MyFilmsDetail.Watched_Toggle(MyFilms.conf.StrIndex, true);
                  }
                }
                afficher_detail(true);
                break;

              case "rating":
                MyFilmsDialogSetRating dlgRating =
                  (MyFilmsDialogSetRating)GUIWindowManager.GetWindow(MyFilms.ID_MyFilmsDialogRating);
                if (MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString().Length > 0)
                {
                  dlgRating.Rating = (decimal)MyFilms.r[MyFilms.conf.StrIndex]["Rating"];
                  if (dlgRating.Rating > 10) dlgRating.Rating = 10;
                }
                else dlgRating.Rating = 0;

                dlgRating.SetTitle(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString());
                dlgRating.DoModal(GetID);
                // if (dlgRating.SelectedLabel == -1 || dlgmenu.SelectedLabel != 2)  // If "ESC" or not returning from "ok"
                if (dlgRating.Result == MyFilmsDialogSetRating.ResultCode.Cancel) // If "ESC" or not returning from "ok"
                {
                  Change_Menu("mainmenu");
                  return;
                }
                MyFilms.r[MyFilms.conf.StrIndex]["Rating"] = dlgRating.Rating;

                Update_XML_database();
                afficher_detail(true);

                // tell any listeners that user rated the movie
                MFMovie movie = new MFMovie();
                movie = GetMovieFromRecord(MyFilms.r[MyFilms.conf.StrIndex]);
                string value = dlgRating.Rating.ToString();
                if (RateItem != null) RateItem(movie, value);
                break;

              case "updatesmenu":
                if (dlgmenu == null) return;
                dlgmenu.Reset();
                choiceViewMenu.Clear();
                dlgmenu.SetHeading(GUILocalizeStrings.Get(10798702)); // Updates ...

                if (MyFilms.conf.StrFileType == Configuration.CatalogType.AntMovieCatalog3 ||
                    MyFilms.conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended)
                {
                  dlgmenu.Add(GUILocalizeStrings.Get(5910)); //Update Internet Movie Details
                  choiceViewMenu.Add("grabber");
                }

                dlgmenu.Add(GUILocalizeStrings.Get(10798642)); // Update by Property (choosen within the UPdate List Property
                choiceViewMenu.Add("updproperty");

                if (ExtendedStartmode("Details context: remove all details"))
                {
                  dlgmenu.Add(GUILocalizeStrings.Get(10798795)); // remove all movie details (selected film)
                  choiceViewMenu.Add("updremovealldetails");
                }

                //if (Helper.FieldIsSet(MyFilms.conf.StrStorage) && (MyFilms.conf.WindowsFileDialog))
                if (Helper.FieldIsSet(MyFilms.conf.StrStorage))
                {
                  dlgmenu.Add(GUILocalizeStrings.Get(10798636)); //filename
                  choiceViewMenu.Add("fileselect");
                }

                if (ExtendedStartmode("Details context: update mediainfos"))
                {
                  dlgmenu.Add(GUILocalizeStrings.Get(10798708)); //update mediainfos
                  choiceViewMenu.Add("updmediainfos");
                }

                //No more needed because of updproperties !!! - so discussion about removal?
                if (ExtendedStartmode("Details context: Itemx update to labely (moved to testmode"))
                {
                  if (Helper.FieldIsSet(StrUpdItem1))
                  {
                    if (StrUpdText1.Length > 0) dlgmenu.Add(StrUpdText1); //Specific Item1 label to update
                    else dlgmenu.Add(StrUpdItem1); //Specific Item1 to update
                    choiceViewMenu.Add("item1");
                  }
                  if (Helper.FieldIsSet(StrUpdItem2))
                  {
                    if (StrUpdText2.Length > 0) dlgmenu.Add(StrUpdText2); //Specific Item2 label to update
                    else dlgmenu.Add(StrUpdItem2); //Specific Item2 to update
                    choiceViewMenu.Add("item2");
                  }
                }

                if (MyFilms.conf.UseThumbsForPersons && !string.IsNullOrEmpty(MyFilms.conf.StrPathArtist))
                {
                  dlgmenu.Add(GUILocalizeStrings.Get(1079900)); // Download person images (selected film)
                  choiceViewMenu.Add("personimages");
                }

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

                int iCovercount = 0;
                bool success =
                  int.TryParse(
                    ChangeLocalCover((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true, true), out iCovercount);
                if (iCovercount > 1)
                {
                  dlgmenu.Add(
                    GUILocalizeStrings.Get(10798762) + " " +
                    ChangeLocalCover((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true, true)); // Change Cover
                  choiceViewMenu.Add("changecover");
                }

                //dlgmenu.Add(GUILocalizeStrings.Get(10798763)); // Covermanager ...
                //choiceViewMenu.Add("covermanager");

                dlgmenu.Add(GUILocalizeStrings.Get(10798766)); // Load single Cover ...
                choiceViewMenu.Add("loadcover");

                dlgmenu.Add(GUILocalizeStrings.Get(10798764)); // Load Covers ...
                choiceViewMenu.Add("loadmulticover");

                dlgmenu.Add(GUILocalizeStrings.Get(10798761)); // Load Covers (TMDB)
                choiceViewMenu.Add("tmdbposter");

                if (ExtendedStartmode("Details context: Thumb creator)"))
                {
                  dlgmenu.Add(GUILocalizeStrings.Get(10798728));
                  //Create Thumb from movie - if no cover available, e.g. with documentaries
                  choiceViewMenu.Add("cover-thumbnailer");
                }

                if (MyFilms.conf.StrFanart)
                {
                  if (ExtendedStartmode("Details context: Fanart Manager ..."))
                  {
                    dlgmenu.Add(GUILocalizeStrings.Get(10798766)); // Fanart Manager ...
                    choiceViewMenu.Add("fanartmanager");
                  }

                  dlgmenu.Add(GUILocalizeStrings.Get(1079874)); // Remove Fanart
                  choiceViewMenu.Add("deletefanart");

                  dlgmenu.Add(GUILocalizeStrings.Get(1079851));
                    // Create single fanart from movie as multi image (local)
                  choiceViewMenu.Add("createfanartmultiimage");

                  dlgmenu.Add(GUILocalizeStrings.Get(1079849)); // create single images fanart from movie (local)
                  choiceViewMenu.Add("createfanartsingleimages");

                  dlgmenu.Add(GUILocalizeStrings.Get(1079853)); // create fanart from movie (local)
                  choiceViewMenu.Add("createfanart");

                  if (!g_Player.Stopped && g_Player.HasVideo && !string.IsNullOrEmpty(g_Player.CurrentFile))
                  {
                    dlgmenu.Add(GUILocalizeStrings.Get(1079852));
                      // Create single fanart from movie on current position (local)
                    choiceViewMenu.Add("createfanartonposition");
                  }

                  dlgmenu.Add(GUILocalizeStrings.Get(1079862)); // Download Fanart (online)
                  choiceViewMenu.Add("fanart");
                }

                dlgmenu.DoModal(GetID);
                if (dlgmenu.SelectedLabel == -1)
                {
                  Change_Menu("mainmenu");
                  return;
                }
                Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                break;

              case "trakt":
                if (dlgmenu == null) return;
                dlgmenu.Reset();
                choiceViewMenu.Clear();
                dlgmenu.SetHeading(GUILocalizeStrings.Get(10798775)); // Trakt...

                dlgmenu.Add(GUILocalizeStrings.Get(10798776)); // 
                choiceViewMenu.Add("trakt-Main");

                //dlgmenu.Add(GUILocalizeStrings.Get(10798777)); // 
                //choiceViewMenu.Add("trakt-Calendar");

                dlgmenu.Add(GUILocalizeStrings.Get(10798778)); // 
                choiceViewMenu.Add("trakt-Friends");

                dlgmenu.Add(GUILocalizeStrings.Get(10798779)); // 
                choiceViewMenu.Add("trakt-RecommendationsMovies");

                dlgmenu.Add(GUILocalizeStrings.Get(10798780)); // 
                choiceViewMenu.Add("trakt-TrendingMovies");

                dlgmenu.Add(GUILocalizeStrings.Get(10798781)); // 
                choiceViewMenu.Add("trakt-WatchedListMovies");

                dlgmenu.Add(GUILocalizeStrings.Get(10798783)); // 
                choiceViewMenu.Add("trakt-AddToWatchedListMovies");

                dlgmenu.Add(GUILocalizeStrings.Get(10798786)); // 
                choiceViewMenu.Add("trakt-Lists");

                dlgmenu.Add(GUILocalizeStrings.Get(10798785)); // 
                choiceViewMenu.Add("trakt-AddRemoveMovieInUserlist");

                dlgmenu.Add(GUILocalizeStrings.Get(10798782)); // Shouts
                choiceViewMenu.Add("trakt-Shouts");

                dlgmenu.Add(GUILocalizeStrings.Get(10798784)); // Rate
                choiceViewMenu.Add("trakt-Rate");

                dlgmenu.Add(GUILocalizeStrings.Get(10798787)); // 
                choiceViewMenu.Add("trakt-RelatedMovies");

                dlgmenu.DoModal(GetID);
                if (dlgmenu.SelectedLabel == -1)
                {
                  Change_Menu("mainmenu");
                  return;
                }
                Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel]);
                break;

              case "trakt-Main":
                GUIWindowManager.ActivateWindow((int)TraktGUIWindows.Main, "");
                break;

              case "trakt-Calendar":
                GUIWindowManager.ActivateWindow((int)TraktGUIWindows.Calendar, "");
                break;

              case "trakt-Friends":
                GUIWindowManager.ActivateWindow((int)TraktGUIWindows.Friends, "");
                break;

              case "trakt-RecommendationsMovies":
                GUIWindowManager.ActivateWindow((int)TraktGUIWindows.RecommendationsMovies, "");
                break;

              case "trakt-TrendingMovies":
                GUIWindowManager.ActivateWindow((int)TraktGUIWindows.TrendingMovies, "");
                break;

              case "trakt-WatchedListMovies":
                GUIWindowManager.ActivateWindow((int)TraktGUIWindows.WatchedListMovies, "");
                break;

              case "trakt-AddToWatchedListMovies":
                if (Helper.IsTraktAvailableAndEnabledAndNewVersion)
                {
                  TraktAddToWatchedList(MyFilms.currentMovie);
                }
                else
                {
                  this.ShowMessageDialog("Error !", "", "Your installed Trakt Version does not allow this feature!");
                }
                break;

              case "trakt-Lists":
                if (Helper.IsTraktAvailableAndEnabledAndNewVersion)
                {
                  GUIWindowManager.ActivateWindow((int)TraktGUIWindows.Lists, "");
                }
                else
                {
                  this.ShowMessageDialog("Error !", "", "Your installed Trakt Version does not allow this feature!");
                }
                break;

              case "trakt-AddRemoveMovieInUserlist":
                if (Helper.IsTraktAvailableAndEnabledAndNewVersion)
                {
                  TraktAddRemoveMovieInUserlist(MyFilms.currentMovie, false);
                }
                else
                {
                  this.ShowMessageDialog("Error !", "", "Your installed Trakt Version does not allow this feature!");
                }
                break;

              case "trakt-Shouts":
                movie = GetMovieFromRecord(MyFilms.r[MyFilms.conf.StrIndex]);
                this.TraktShout(movie);
                //TraktShout(MyFilms.currentMovie);
                //GUIWindowManager.ActivateWindow((int)TraktGUIWindows.Shouts);
                break;

              case "trakt-Rate":
                if (Helper.IsTraktAvailableAndEnabled)
                {
                  TraktRate(MyFilms.currentMovie);
                }
                else
                {
                  this.ShowMessageDialog("Error !", "", "Your installed Trakt Version does not allow this feature!");
                }
                break;

              case "trakt-RelatedMovies":
                if (Helper.IsTraktAvailableAndEnabledAndVersion1301)
                {
                  TraktRelatedMovies(MyFilms.currentMovie);
                }
                else
                {
                  this.ShowMessageDialog("Error !", "", "Your installed Trakt Version does not allow this feature!");
                }
                break;

              case "trailermenu":
                if (dlgmenu == null) return;
                dlgmenu.Reset();
                choiceViewMenu.Clear();
                dlgmenu.SetHeading(GUILocalizeStrings.Get(10798704)); // Trailer ...

                if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
                {
                  string trailercount = "";
                  if (
                    string.IsNullOrEmpty(
                      MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim())) trailercount = "0";
                  else
                  {
                    string[] split1 =
                      MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorageTrailer].ToString().Trim().Split(
                        new Char[] { ';' });
                    trailercount = split1.Count().ToString();
                  }
                  if (trailercount != "0")
                  {
                    dlgmenu.Add(GUILocalizeStrings.Get(10798710) + " (" + trailercount + ")");
                      //play trailer (<number trailers present>)
                    choiceViewMenu.Add("playtrailer");
                  }
                }
                dlgmenu.Add(GUILocalizeStrings.Get(10798711)); //search youtube trailer with onlinevideos
                choiceViewMenu.Add("playtraileronlinevideos");

                dlgmenu.Add(GUILocalizeStrings.Get(10798712)); //search apple itunes trailer with onlinevideos
                choiceViewMenu.Add("playtraileronlinevideosappleitunes");

                dlgmenu.Add(GUILocalizeStrings.Get(10798716)); //search IMDB trailer with onlinevideos
                choiceViewMenu.Add("playtraileronlinevideosimdbtrailer");


                if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer)) // StrDirStorTrailer only required for extended search
                {
                  dlgmenu.Add(GUILocalizeStrings.Get(10798723)); //Search local Trailer and Update DB (local)
                  choiceViewMenu.Add("trailer-register");

                  dlgmenu.Add(GUILocalizeStrings.Get(10798725)); //delete Trailer entries from DB record
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

              case "subtitles":
                if (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() != null)
                {
                  // ShowSubtitleMenu(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString());
                  if (Helper.IsSubCentralAvailableAndEnabled)
                  {
                    GUIWindowManager.ActivateWindow((int)MyFilms.ExternalPluginWindows.SubCentral);
                  }
                }
                break;

              case "fileselect":
                string wfile = string.Empty;
                string wdirectory = string.Empty;
                if (System.IO.File.Exists(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString())) // Check if Sourcefile exists
                {
                  wfile = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
                  wdirectory = System.IO.Path.GetDirectoryName(wfile);
                }
                keyboard.Reset();
                keyboard.Text = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
                keyboard.DoModal(GetID);
                if (keyboard.IsConfirmed)
                {
                  wfile = keyboard.Text.ToString();
                }
                else wfile = string.Empty;
                if (wfile != string.Empty)
                {
                  MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage] = wfile;
                  Update_XML_database();
                  afficher_detail(true);
                }
                break;

              case "item1":
                #region item1
                if (StrUpdDflT1.Length > 0) keyboard.Text = StrUpdDflT1;
                else keyboard.Text = MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem1].ToString();
                keyboard.DoModal(GetID);
                if (keyboard.IsConfirmed)
                {
                  switch (ds.Movie.Columns[StrUpdItem1].DataType.Name)
                  {
                    case "Decimal":
                      try
                      {
                        MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem1] = Convert.ToDecimal(keyboard.Text);
                      }
                      catch
                      {
                        dlgOK.SetHeading(GUILocalizeStrings.Get(924)); // menu
                        dlgOK.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                        dlgOK.DoModal(GetID);
                        return;
                      }
                      break;
                    case "Int32":
                      try
                      {
                        MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem1] = Convert.ToInt32(keyboard.Text);
                      }
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
                #endregion
                break;

              case "item2":
                #region item2
                if (StrUpdDflT1.Length > 0) keyboard.Text = StrUpdDflT2;
                else keyboard.Text = MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem2].ToString();
                keyboard.DoModal(GetID);
                if (keyboard.IsConfirmed)
                {
                  switch (ds.Movie.Columns[StrUpdItem2].DataType.Name)
                  {
                    case "Decimal":
                      try
                      {
                        MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem2] = Convert.ToDecimal(keyboard.Text);
                      }
                      catch
                      {
                        dlgOK.SetHeading(GUILocalizeStrings.Get(924)); // menu
                        dlgOK.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                        dlgOK.DoModal(GetID);
                        return;
                      }
                      break;
                    case "Int32":
                      try
                      {
                        MyFilms.r[MyFilms.conf.StrIndex][StrUpdItem2] = Convert.ToInt32(keyboard.Text);
                      }
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
                  }
                  Update_XML_database();
                  afficher_detail(true);
                }
                #endregion
                break;

              case "delete":
                {
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
                    Change_Menu("mainmenu");
                    return;
                  }
                  Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                  break;
                }

              case "removefromdb":
                dlgYesNo.Reset();
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079831)); //Remove movie from catalog
                dlgYesNo.SetLine(2, GUILocalizeStrings.Get(433)); //confirm suppression
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)
                {
                  // MyFilmsDetail.Suppress_Entry((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex);
                  MyFilmsDetail.Manual_Delete((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true, false);
                  // Update_XML_database();
                  MyFilms.r = BaseMesFilms.ReadDataMovies(
                    MyFilms.conf.StrDfltSelect,
                    MyFilms.conf.StrFilmSelect,
                    MyFilms.conf.StrSorta,
                    MyFilms.conf.StrSortSens);
                  afficher_detail(true);
                }
                if (dlgYesNo.SelectedLabel == -1)
                {
                  Change_Menu("delete");
                  return;
                }
                break;
              case "deletefromdisk":
                dlgYesNo.Reset();
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079832)); //Delete movie file(s) from disk
                dlgYesNo.SetLine(1, GUILocalizeStrings.Get(927)); // warning
                dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079834));
                  //If you confirm, you media files will physically be deleted !
                dlgYesNo.SetLine(3, GUILocalizeStrings.Get(1079835)); //Are you sure you want to delete movie ?
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)
                {
                  MyFilmsDetail.Manual_Delete((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, false, true);
                  MyFilms.r = BaseMesFilms.ReadDataMovies(
                    MyFilms.conf.StrDfltSelect,
                    MyFilms.conf.StrFilmSelect,
                    MyFilms.conf.StrSorta,
                    MyFilms.conf.StrSortSens);
                  afficher_detail(true);
                }
                if (dlgYesNo.SelectedLabel == -1)
                {
                  Change_Menu("delete");
                  return;
                }
                break;
              case "deletefromdbanddisk":
                dlgYesNo.Reset();
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079833)); //Delete from catalog and disk
                dlgYesNo.SetLine(1, GUILocalizeStrings.Get(927)); // warning
                dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079834));
                //If you confirm, you media files will physically be deleted !
                dlgYesNo.SetLine(3, GUILocalizeStrings.Get(1079835)); //Are you sure you want to delete movie ?
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)
                {
                  // old "suppress approach" MyFilmsDetail.Suppress_Entry((DataRow[])MyFilms.r, (int)facadeFilms.SelectedListItem.ItemId);
                  MyFilmsDetail.Manual_Delete((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true, true);
                  MyFilms.r = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, MyFilms.conf.StrFilmSelect, MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens);
                  afficher_detail(true);
                  break;
                }
                if (dlgYesNo.SelectedLabel == -1)
                {
                  Change_Menu("delete");
                  return;
                }
                break;

              case "updproperty":
                {
                  #region build menu
                  List<string> choiceUpd = new List<string>();
                  if (dlgmenu == null) return;
                  dlgmenu.Reset();
                  dlgmenu.SetHeading(GUILocalizeStrings.Get(10798643)); // menu

                  dlgmenu.Add(" " + BaseMesFilms.Translate_Column(MyFilms.conf.StrTitle1));
                  choiceUpd.Add(MyFilms.conf.StrTitle1);

                  if (Helper.FieldIsSet(MyFilms.conf.StrTitle2))
                  {
                    dlgmenu.Add(" " + BaseMesFilms.Translate_Column(MyFilms.conf.StrTitle2));
                    choiceUpd.Add(MyFilms.conf.StrTitle2);
                  }

                  if (Helper.FieldIsSet(MyFilms.conf.StrStorage)) // Source field / media file name
                  {
                    dlgmenu.Add(" " + BaseMesFilms.Translate_Column(MyFilms.conf.StrStorage));
                    choiceUpd.Add(MyFilms.conf.StrStorage);
                  }

                  string[] defaultupdateitems = new string[] {"Category", "Year", "Date", "Country", "Rating" };
                  foreach (string wupd in defaultupdateitems)
                  {
                    dlgmenu.Add(" " + BaseMesFilms.Translate_Column(wupd));
                    choiceUpd.Add(wupd.Trim());
                  }

                  dlgmenu.Add(GUILocalizeStrings.Get(10798765)); // *** show all ***
                  choiceUpd.Add("showall");

                  dlgmenu.DoModal(GetID);
                  if (dlgmenu.SelectedLabel == -1) Change_Menu("mainmenu"); // go back to main contextmenu

                  // show all search fields, if selected ...
                  if (choiceUpd[dlgmenu.SelectedLabel] == "showall")
                  {
                    dlgmenu.Reset();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798643)); // menu
                    choiceUpd.Clear();
                    ArrayList displayItems = MyFilms.GetDisplayItems("view");
                    foreach (string[] displayItem in displayItems)
                    {
                      string entry = (string.IsNullOrEmpty(displayItem[1])) ? displayItem[0] : displayItem[1];
                      dlgmenu.Add(" " + entry);
                      choiceUpd.Add(displayItem[0]);
                      LogMyFilms.Debug("Update properties menu - add '{0}' as '{1}'", displayItem[0], entry);
                    }
                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1) Change_Menu("mainmenu"); // go back to main contextmenu
                  }
                  #endregion
                  string wproperty = choiceUpd[dlgmenu.SelectedLabel];
                  #region update selected property

                  dlgmenu.Reset();
                  keyboard.Reset();
                  keyboard.Text = MyFilms.r[MyFilms.conf.StrIndex][wproperty].ToString();
                  keyboard.DoModal(GetID);
                  if (keyboard.IsConfirmed)
                  {
                    switch (ds.Movie.Columns[wproperty].DataType.Name)
                    {
                      case "Decimal":
                        try
                        {
                          MyFilms.r[MyFilms.conf.StrIndex][wproperty] = Convert.ToDecimal(keyboard.Text);
                        }
                        catch
                        {
                          dlgOK.SetHeading(GUILocalizeStrings.Get(10798642)); // menu
                          dlgOK.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                          dlgOK.DoModal(GetID);
                          return;
                        }
                        break;
                      case "Int32":
                        try
                        {
                          MyFilms.r[MyFilms.conf.StrIndex][wproperty] = Convert.ToInt32(keyboard.Text);
                        }
                        catch
                        {
                          dlgOK.SetHeading(GUILocalizeStrings.Get(10798642)); // menu
                          dlgOK.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                          dlgOK.DoModal(GetID);
                          return;
                        }
                        break;
                      default:
                        MyFilms.r[MyFilms.conf.StrIndex][wproperty] = keyboard.Text;
                        break;
                    }
                    Update_XML_database();
                    afficher_detail(true);
                  }
                  break;

                  #endregion
                }

              case "updremovealldetails":
                ArrayList deleteItems = MyFilms.GetDisplayItems("deletedetails");
                foreach (string[] displayItem in deleteItems)
                {
                  string wproperty = displayItem[0];
                  if (!string.IsNullOrEmpty(wproperty) && MyFilms.r[MyFilms.conf.StrIndex][wproperty] != DBNull.Value) // only reset fields existing in DB
                  {
                    try
                    {
                      switch (ds.Movie.Columns[wproperty].DataType.Name)
                      {
                        case "DateTime":
                          MyFilms.r[MyFilms.conf.StrIndex][wproperty] = DateTime.Now;
                          break;
                        case "Decimal":
                          MyFilms.r[MyFilms.conf.StrIndex][wproperty] = 0;
                          break;
                        case "Int32":
                          MyFilms.r[MyFilms.conf.StrIndex][wproperty] = 0;
                          break;
                        default:
                          MyFilms.r[MyFilms.conf.StrIndex][wproperty] = "";
                          break;
                      }
                      LogMyFilms.Debug("Delete value for property '" + wproperty + "'");
                    }
                    catch (Exception ex)
                    {
                      LogMyFilms.Debug("Error deleting value for property '" + wproperty + "', exception: ", ex.Message);
                    }
                  }
                }

                Update_XML_database();
                afficher_detail(true);
                break;
              
              case "updmediainfos":
                {
                  #region update mediainfo
                  string FileDate = string.Empty;
                  string VideoPlaytime = string.Empty;
                  string VideoCodec = string.Empty;
                  string VideoFormat = string.Empty;
                  string VideoFormatProfile = string.Empty;
                  string VideoBitRate = string.Empty;
                  string VideoFrameRate = string.Empty;
                  string VideoWidth = string.Empty;
                  string VideoHeight = string.Empty;
                  string VideoAspectRatio = string.Empty;

                  string AudioCodec = string.Empty;
                  string AudioFormat = string.Empty;
                  string AudioFormatProfile = string.Empty;
                  string AudioBitrate = string.Empty;
                  string AudioChannels = string.Empty;
                  string AudioTracks = string.Empty;

                  string TextCount = string.Empty;

                  long TotalSize = 0;
                  int TotalRuntime = 0;

                  string source = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
                  string[] files = source.Split(new Char[] { ';' });

                  foreach (string file in files)
                  {
                    LogMyFilms.Debug("Mediainfo - Filename: '" + file + "'");
                    if (System.IO.File.Exists(file))
                    {
                      // Get File Date Added/Created
                      FileDate = GetFileTimeStamps(file);
                      LogMyFilms.Debug("Mediainfo - FileDate: '" + FileDate + "'");

                      System.IO.FileInfo fi = new System.IO.FileInfo(file);
                      TotalSize += fi.Length;

                      // ReadMediaInfo(file, ref mediainfo);
                      MediaInfo mediainfo = MediaInfo.GetInstance();

                      // MediaInfo Object could not be created
                      if (null == mediainfo) 
                        return;

                      // Check if File Exists and is not an Image type e.g. ISO (we can't extract mediainfo from that)
                      if (System.IO.File.Exists(file) && !Helper.IsImageFile(file))
                      {
                        try
                        {
                          LogMyFilms.Debug("Attempting to read Mediainfo for ", file.ToString());

                          // open file in MediaInfo
                          mediainfo.Open(file);

                          // check number of failed attempts at mediainfo extraction                    
                          int noAttempts = 0;

                          // Get Playtime (runtime)
                          string result = mediainfo.VideoPlaytime;

                          string LocalPlaytime = result != "-1" ? result : noAttempts.ToString();

                          VideoPlaytime = mediainfo.VideoPlaytime;
                          LogMyFilms.Debug("Mediainfo - VideoPlaytime: '" + VideoPlaytime + "'");
                          TotalRuntime += Int32.Parse(VideoPlaytime);

                          VideoCodec = mediainfo.VideoCodec;
                          LogMyFilms.Debug("Mediainfo - Videocodec: '" + VideoCodec + "'");
                          VideoFormat = mediainfo.VideoCodecFormat;
                          LogMyFilms.Debug("Mediainfo - VideoCodecFormat: '" + VideoFormat + "'");
                          VideoFormatProfile = mediainfo.VideoFormatProfile;
                          LogMyFilms.Debug("Mediainfo - VideoFormatProfile: '" + VideoFormatProfile + "'");
                          VideoBitRate = mediainfo.VideoBitrate;
                          LogMyFilms.Debug("Mediainfo - VideoBitrate: '" + VideoBitRate + "'");
                          VideoFrameRate = mediainfo.VideoFramesPerSecond;
                          LogMyFilms.Debug("Mediainfo - VideoFramesPerSecond: '" + VideoFrameRate + "'");
                          VideoWidth = mediainfo.VideoWidth;
                          LogMyFilms.Debug("Mediainfo - VideoWidth: '" + VideoWidth + "'");
                          VideoHeight = mediainfo.VideoHeight;
                          LogMyFilms.Debug("Mediainfo - VideoHeight: '" + VideoHeight + "'");
                          VideoAspectRatio = mediainfo.VideoAspectRatio;
                          LogMyFilms.Debug("Mediainfo - VideoAspectRatio: '" + VideoAspectRatio + "'");

                          AudioCodec = mediainfo.AudioCodec;
                          LogMyFilms.Debug("Mediainfo - AudioCodec: '" + AudioCodec + "'");
                          AudioFormat = mediainfo.AudioCodecFormat;
                          LogMyFilms.Debug("Mediainfo - AudioCodecFormat: '" + AudioFormat + "'");
                          AudioFormatProfile = mediainfo.AudioFormatProfile;
                          LogMyFilms.Debug("Mediainfo - AudioFormatProfile: '" + AudioFormatProfile + "'");
                          AudioBitrate = mediainfo.AudioBitrate;
                          LogMyFilms.Debug("Mediainfo - AudioBitrate: '" + AudioBitrate + "'");
                          AudioChannels = mediainfo.AudioChannelCount;
                          LogMyFilms.Debug("Mediainfo - AudioChannelCount: '" + AudioChannels + "'");
                          AudioTracks = mediainfo.AudioStreamCount;
                          LogMyFilms.Debug("Mediainfo - AudioStreamCount: '" + AudioTracks + "'");

                          TextCount = mediainfo.SubtitleCount;
                          LogMyFilms.Debug("Mediainfo - SubtitleCount: '" + TextCount + "'");

                          // MediaInfo cleanup
                          mediainfo.Close();
                        }
                        catch (Exception ex)
                        {
                          LogMyFilms.Debug("Error reading MediaInfo: ", ex.Message);
                        }

                      }
                      LogMyFilms.Debug("File '" + file + "' does not exist or is an image file");


                    }
                    else
                      LogMyFilms.Debug("Mediainfo - File '" + file + "' does not exist !");
                  }
                  int size = (int)(TotalSize / 1024 / 1024);
                  string strSize = string.Format("{0}", size);
                  //string humanKBSize = string.Format("{0} KB", size);
                  //string humanMBSize = string.Format("{0} MB", size / 1024);
                  //string humanGBSize = string.Format("{0} GB", size / 1024 / 1024);
                  LogMyFilms.Debug("Mediainfo - TotalSize: '" + strSize + "'");
                  LogMyFilms.Debug("Mediainfo - TotalRuntime: '" + TotalRuntime.ToString() + "'");

                  //MyFilms.r[MyFilms.conf.StrIndex]["Length"] = TotalRuntime.ToString();
                  //MyFilms.r[MyFilms.conf.StrIndex]["VideoFormat"] = string.Empty;
                  //MyFilms.r[MyFilms.conf.StrIndex]["VideoBitrate"] = string.Empty;
                  //MyFilms.r[MyFilms.conf.StrIndex]["AudioFormat"] = string.Empty;
                  //MyFilms.r[MyFilms.conf.StrIndex]["AudioBitrate"] = string.Empty;
                  //MyFilms.r[MyFilms.conf.StrIndex]["Resolution"] = string.Empty;
                  //MyFilms.r[MyFilms.conf.StrIndex]["Framerate"] = string.Empty;
                  //MyFilms.r[MyFilms.conf.StrIndex]["Languages"] = string.Empty;
                  //MyFilms.r[MyFilms.conf.StrIndex]["Subtitles"] = string.Empty;
                  //MyFilms.r[MyFilms.conf.StrIndex]["DateAdded"] = string.Empty;
                  //MyFilms.r[MyFilms.conf.StrIndex]["Size"] = string.Format("{0}", size); ;
                  //MyFilms.r[MyFilms.conf.StrIndex]["Disks"] = split.Count().ToString();
                  //MyFilms.r[MyFilms.conf.StrIndex]["Aspectratio"] = string.Empty;
                  //Update_XML_database();
                  //afficher_detail(true);
                  #endregion
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
                    title = GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
                    mediapath = GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
                    sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);

                    // this will be executed after background thread finished
                    doUpdateDetailsViewByFinishEvent = true;
                    grabb_Internet_Informations(title, GetID, wChooseScript, MyFilms.conf.StrGrabber_cnf, mediapath, GrabType.All, false, sTitles);
                    // afficher_detail(true); // -> will be executes by OnDetailsUpdated message handler later ...
                    setProcessAnimationStatus(false, m_SearchAnimation); // make sure it's switched off
                    break;

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
                      //Zuerst Pfad lesen, dann Dateien suchen, in liste packen, Auswahlmenü präsentieren und zum Schluß Update des Records 
                      // Suchen nach Files mit folgendem Kriterium:
                      // 1.) ... die den Filmnamen im Filenamen haben und im Trailerverzeichnis gefunden werden (wahrscheinlich HD, daher an 1. Stelle setzen)
                      // 2.) Im Verzeichnis des Films suchen nach Filmdateien die das Wort "Trailer" im Namen haben (Endung beliebig: avi, mov, flv, etc.)
                      // 3.) Im (Trailer)-Suchpfad nach Verzeichnissen, die nach dem Filmnamen benannt sind - dann alle Files darin registrien

                      LogMyFilms.Debug("(SearchTrailerLocal) SelectedItemInfo from (MyFilms.r[MyFilms.conf.StrIndex]: '" + (MyFilms.r[MyFilms.conf.StrIndex].ToString() + "'"));
                      SearchTrailerLocal((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true);
                      setProcessAnimationStatus(false, m_SearchAnimation);
                      afficher_detail(true);
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
                        //MyFilms.r = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrDfltSelect, MyFilms.conf.StrFilmSelect, MyFilms.conf.StrSorta, MyFilms.conf.StrSortSens);
                        afficher_detail(true);
                    }
                    break;

                case "personimages":
                    if (!MyFilmsDetail.IsInternetConnectionAvailable()) break; // stop, if no internet available
                    Menu_LoadPersonImages();
                    break;
                    
                case "fanart":
                    if (!MyFilmsDetail.IsInternetConnectionAvailable()) break; // stop, if no internet available
                    // Remove_Backdrops_Fanart(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(), false); // do not remove, as user might only want to "update" fanart ...
                    Menu_LoadFanart(false);
                    break;

                case "createfanart": // create fanart from local media
                    Menu_CreateFanart(GrabUtil.Artwork_Fanart_Type.MultiImageWithMultipleSingleImages);
                    break;

                case "createfanartsingleimages": // create single images fanart from local media
                    Menu_CreateFanart(GrabUtil.Artwork_Fanart_Type.MultipleSingleImages);
                    break;

                case "createfanartmultiimage": // create single fanart from local media on pause position
                    Menu_CreateFanart(GrabUtil.Artwork_Fanart_Type.Multiimage);
                    break;

                case "createfanartonposition": // create single fanart from local media on pause position
                    Menu_CreateFanart_OnMoviePosition();
                    afficher_detail(true);
                    break;

                case "deletefanart":
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079874));//Delete fanart
                    dlgYesNo.SetLine(1, "");
                    dlgYesNo.SetLine(2, GUILocalizeStrings.Get(433));//confirm suppression
                    dlgYesNo.DoModal(GetID);
                    if (dlgYesNo.IsConfirmed)
                    {
                      sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
                      Remove_Backdrops_Fanart(sTitles.FanartTitle, false);
                      Remove_Backdrops_Fanart(sTitles.OriginalTitle, false);
                      Remove_Backdrops_Fanart(sTitles.TranslatedTitle, false);
                      Remove_Backdrops_Fanart(sTitles.FormattedTitle, false);
                      Remove_Backdrops_Fanart(sTitles.MasterTitle, false);
                      Remove_Backdrops_Fanart(sTitles.SecondaryTitle, false);
                      Remove_Backdrops_Fanart(sTitles.SortTitle, false);
                      Thread.Sleep(50);
                      afficher_detail(true);
                    }
                    break;

                case "changecover":
                    string covercount = ChangeLocalCover((DataRow[])MyFilms.r, (int)MyFilms.conf.StrIndex, true, false);
                    afficher_detail(true);
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    Change_Menu("fanartcovermenu"); // stay in cover toggle menu
                    break;

                case "covermanager":
                    LogMyFilms.Debug("Switching to Cover Manager Window");
                    MyFilmsCoverManager cm = null;
                    if (cm == null)
                    {
                      cm = new MyFilmsCoverManager();
                      GUIWindow cmwindow = (GUIWindow)cm;
                      GUIWindowManager.Add(ref cmwindow);
                      cm.Init();
                    }
                    // cm.MovieID = MyFilms.conf.StrIndex; // will be set later in cm class
                    cm.setPageTitle("Cover Manager");
                    //sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex));
                    //cm.ArtworkFileName = GetOrCreateCoverFilename(MyFilms.r, MyFilms.conf.StrIndex, sTitles.MasterTitle);
                    GUIWindowManager.ActivateWindow(cm.GetID, false);
                    break;

                case "fanartmanager":
                    LogMyFilms.Info("Fanart Manager : Not yet implemented - using old submenu instead!");
                    if (dlgmenu == null) return;
                    dlgmenu.Reset();
                    choiceViewMenu.Clear();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798767)); // Fanart Manager ...

                    if (MyFilms.conf.StrFanart)
                    {
                      dlgmenu.Add(GUILocalizeStrings.Get(1079874)); // Remove Fanart
                      choiceViewMenu.Add("deletefanart");

                      dlgmenu.Add(GUILocalizeStrings.Get(1079851)); // Create single fanart from movie as multi image (local)
                      choiceViewMenu.Add("createfanartmultiimage");

                      dlgmenu.Add(GUILocalizeStrings.Get(1079849)); // create single images fanart from movie (local)
                      choiceViewMenu.Add("createfanartsingleimages");

                      dlgmenu.Add(GUILocalizeStrings.Get(1079853)); // create fanart from movie (local)
                      choiceViewMenu.Add("createfanart");

                      if (!g_Player.Stopped && g_Player.HasVideo && !string.IsNullOrEmpty(g_Player.CurrentFile))
                      {
                        dlgmenu.Add(GUILocalizeStrings.Get(1079852)); // Create single fanart from movie on current position (local)
                        choiceViewMenu.Add("createfanartonposition");
                      }

                      dlgmenu.Add(GUILocalizeStrings.Get(1079862)); // Download Fanart (online)
                      choiceViewMenu.Add("fanart");
                    }

                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1)
                    {
                      Change_Menu("mainmenu");
                      return;
                    }
                    Change_Menu(choiceViewMenu[dlgmenu.SelectedLabel].ToLower());
                    break;

                case "loadcover":
                    title = GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
                    mediapath = GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
                    sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath); 
                    grabb_Internet_Informations(title, GetID, true, MyFilms.conf.StrGrabber_cnf, mediapath, GrabType.Cover, false, sTitles);
                    afficher_detail(true);
                    setProcessAnimationStatus(false, m_SearchAnimation); // make sure it's switched off
                    break;

                case "loadmulticover":
                    title = GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
                    mediapath = GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
                    sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);
                    grabb_Internet_Informations(title, GetID, true, MyFilms.conf.StrGrabber_cnf, mediapath, GrabType.MultiCovers, false, sTitles);
                    afficher_detail(true);
                    setProcessAnimationStatus(false, m_SearchAnimation); // make sure it's switched off
                    break;

                case "tmdbposter":
                    Menu_LoadTMDBposter();
                    afficher_detail(true);
                    break;

                default: // Main Contextmenu
                    ShowMessageDialog("Info", "", "Action not yet implemented");
                    return;
            }
        }

        private void Menu_LoadTMDBposter()
        {
          //Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
          Searchtitles stitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
          if (string.IsNullOrEmpty(stitles.FanartTitle) && MyFilms.conf.StrFanart) 
            return;
          if (MyFilms.conf.StrFanart)
          {
            Download_TMDB_Posters(stitles.OriginalTitle, stitles.TranslatedTitle, stitles.Director, stitles.year.ToString(), true, GetID, stitles.OriginalTitle);
          }
        }

        private void Menu_CreateFanart_OnMoviePosition()
        {
          int duration = (int)g_Player.Duration;
          int currentposition = (int)g_Player.CurrentPosition;
          string file = g_Player.CurrentFile;
          string title = g_Player.currentTitle;
          Menu_CreateFanart_OnMoviePosition_Parameterized(duration, currentposition, file, title);
        }

        private void Menu_CreateFanart_OnMoviePosition_Parameterized(int duration, int currentposition, string file, string title)
        {
          //int duration = (int)g_Player.Duration;
          //int currentposition = (int)g_Player.CurrentPosition;
          //string file = g_Player.CurrentFile;
          //string title = g_Player.currentTitle;

          if (!g_Player.Stopped && g_Player.HasVideo && !string.IsNullOrEmpty(g_Player.CurrentFile)) // g_Player.Paused && 
          {
            GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
            if (dlgPrgrs != null)
            {
              dlgPrgrs.Reset();
              dlgPrgrs.DisplayProgressBar = false;
              dlgPrgrs.ShowWaitCursor = true;
              dlgPrgrs.DisableCancel(true);
              dlgPrgrs.SetHeading(GUILocalizeStrings.Get(1079847)); // MyFilms Fanart Creator
              dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
              dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079848)); // Creating new Fanart from movie
              dlgPrgrs.Percentage = 0;
            }
            new System.Threading.Thread(delegate()
            {
              Searchtitles sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
              if (!string.IsNullOrEmpty(sTitles.FanartTitle) && MyFilms.conf.StrFanart)
              {
                // Remove_Backdrops_Fanart(fanartTitle, false); // old: // Remove_Backdrops_Fanart(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(), false);
                // Thread.Sleep(50);
                bool success = GrabUtil.GetFanartFromMovie(sTitles.FanartTitle, sTitles.year.ToString(), MyFilms.conf.StrPathFanart, GrabUtil.Artwork_Fanart_Type.Snapshotimage, file, "localfanart", currentposition);
              }
              if (dlgPrgrs != null)
                dlgPrgrs.Percentage = 100; dlgPrgrs.ShowWaitCursor = false; dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846)); Thread.Sleep(50); dlgPrgrs.Close(); // Done ...
              GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
              {
                // this will be executed after background thread finished
                doUpdateDetailsViewByFinishEvent = true;
                if (DetailsUpdated != null)
                  DetailsUpdated(true); // will launch afficher_detail(true) via message handler
                return 0;
              }, 0, 0, null);
            }) { Name = "MyFilmsFanartCreator", IsBackground = true }.Start();
            return;
          }
          else
          {
            ShowMessageDialog("MyFilms - '" + title + "'", "Unable to create fanart from movie position !", "No movie active?"); // Show menu with hints
          }
        }

        private void Menu_CreateFanart(GrabUtil.Artwork_Fanart_Type FanartType)
        {
          {
            GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
            if (dlgPrgrs != null)
            {
              dlgPrgrs.Reset();
              dlgPrgrs.DisplayProgressBar = false;
              dlgPrgrs.ShowWaitCursor = true;
              dlgPrgrs.DisableCancel(true);
              dlgPrgrs.SetHeading(GUILocalizeStrings.Get(1079847)); // MyFilms Fanart Creator
              dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
              dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079848)); // Creating new Fanart from movie
              dlgPrgrs.Percentage = 0;
            }
            new System.Threading.Thread(delegate()
              {
                string movieFile = GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);

                Searchtitles stitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
                LogMyFilms.Debug("Menu_CreateFanart(): movieFile: '" + movieFile + "', fanartTitle: '" + stitles.FanartTitle + "'");
                if (!string.IsNullOrEmpty(stitles.FanartTitle) && MyFilms.conf.StrFanart && !string.IsNullOrEmpty(movieFile))
                {
                  // Remove_Backdrops_Fanart(fanartTitle, false); // old: Remove_Backdrops_Fanart(MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString(), false);
                  // Thread.Sleep(50);
                  bool success = false;
                  switch (FanartType)
                  {
                      
                    case GrabUtil.Artwork_Fanart_Type.MultiImageWithMultipleSingleImages:
                      success = GrabUtil.GetFanartFromMovie(stitles.FanartTitle, stitles.year.ToString(), MyFilms.conf.StrPathFanart, GrabUtil.Artwork_Fanart_Type.MultiImageWithMultipleSingleImages, movieFile, "localfanart", 0);
                      break;
                    case GrabUtil.Artwork_Fanart_Type.Multiimage:
                      success = GrabUtil.GetFanartFromMovie(stitles.FanartTitle, stitles.year.ToString(), MyFilms.conf.StrPathFanart, GrabUtil.Artwork_Fanart_Type.Multiimage, movieFile, "localfanart", 0);
                      break;
                    case GrabUtil.Artwork_Fanart_Type.MultipleSingleImages:
                      success = GrabUtil.GetFanartFromMovie(stitles.FanartTitle, stitles.year.ToString(), MyFilms.conf.StrPathFanart, GrabUtil.Artwork_Fanart_Type.MultipleSingleImages, movieFile, "localfanart", 0);
                      break;
                  }
                }
                
                if (dlgPrgrs != null)
                  dlgPrgrs.Percentage = 100; dlgPrgrs.ShowWaitCursor = false; dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846)); Thread.Sleep(50); dlgPrgrs.Close(); // Done ...
                GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
                {
                  // this will be executed after background thread finished
                  doUpdateDetailsViewByFinishEvent = true;
                  if (DetailsUpdated != null)
                    DetailsUpdated(true); // will launch afficher_detail(true) via message handler
                  return 0;
                }, 0, 0, null);
            }) { Name = "MyFilmsFanartCreator", IsBackground = true }.Start();
            return;
          }  
        }

        private void Menu_LoadFanart(bool loadPersonImages)
        {
          Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
          string personartworkpath = string.Empty;
          Searchtitles sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
          string imdbid = GetIMDB_Id(MyFilms.r[MyFilms.conf.StrIndex]);
          if (!string.IsNullOrEmpty(sTitles.FanartTitle) && MyFilms.conf.StrFanart)
          {
            LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download Fanart: originaltitle: '" + sTitles.OriginalTitle + "' - translatedtitle: '" + sTitles.TranslatedTitle + "' - director: '" + sTitles.Director + "' - year: '" + sTitles.year.ToString() + "'");
            if (MyFilms.conf.UseThumbsForPersons && !string.IsNullOrEmpty(MyFilms.conf.StrPathArtist))
            {
              personartworkpath = MyFilms.conf.StrPathArtist;
              LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download PersonArtwork 'enabled' - destination: '" + personartworkpath + "'");
            }
            doUpdateDetailsViewByFinishEvent = true;
            Download_Backdrops_Fanart(sTitles.OriginalTitle, sTitles.TranslatedTitle, sTitles.FormattedTitle, sTitles.Director.ToString(), imdbid, sTitles.year.ToString(), true, GetID, sTitles.FanartTitle, personartworkpath, true, loadPersonImages);
          }
        }

        private void Menu_LoadPersonImages()
        {
          Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
          string personartworkpath = string.Empty;
          Searchtitles sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
          string imdbid = GetIMDB_Id(MyFilms.r[MyFilms.conf.StrIndex]);
          if (!string.IsNullOrEmpty(sTitles.FanartTitle) && MyFilms.conf.StrFanart)
          {
            LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download Fanart: originaltitle: '" + sTitles.OriginalTitle + "' - translatedtitle: '" + sTitles.TranslatedTitle + "' - director: '" + sTitles.Director + "' - year: '" + sTitles.year.ToString() + "'");
            personartworkpath = MyFilms.conf.StrPathArtist;
            LogMyFilms.Debug("MyFilmsDetails (fanart-menuselect) Download PersonArtwork 'enabled' - destination: '" + personartworkpath + "'");
            doUpdateDetailsViewByFinishEvent = true;
            Download_Backdrops_Fanart(sTitles.OriginalTitle, sTitles.TranslatedTitle, sTitles.FormattedTitle, sTitles.Director.ToString(), imdbid, sTitles.year.ToString(), true, GetID, sTitles.FanartTitle, personartworkpath, false, true);
          }
        }

        private void TraktShout(MFMovie movie)
        {
          LogMyFilms.Debug("TraktShout(): Call with Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "'");
          TraktPlugin.TraktHelper.ShowMovieShouts(movie.IMDBNumber, movie.Title, movie.Year.ToString(), movie.Fanart);
          // replaced by helper call
          //TraktPlugin.GUI.GUIShouts.ShoutType = TraktPlugin.GUI.GUIShouts.ShoutTypeEnum.movie;
          //TraktPlugin.GUI.GUIShouts.MovieInfo = new TraktPlugin.GUI.MovieShout { IMDbId = movie.IMDBNumber, TMDbId = "", Title = movie.Title, Year = movie.Year.ToString() };
          //TraktPlugin.GUI.GUIShouts.Fanart = movie.Fanart;
        }

        private void TraktRate(MFMovie movie)
        {
          LogMyFilms.Debug("TraktRate(): Call with Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "'");
          TraktPlugin.TraktAPI.DataStructures.TraktRateMovie rateObject = new TraktPlugin.TraktAPI.DataStructures.TraktRateMovie
          {
            IMDBID = movie.IMDBNumber,
            Title = movie.Title,
            Year = movie.Year.ToString(),
            Rating = "love",
            UserName = TraktPlugin.TraktSettings.Username,
            Password = TraktPlugin.TraktSettings.Password
          };
          TraktPlugin.GUI.GUIUtils.ShowRateDialog<TraktPlugin.TraktAPI.DataStructures.TraktRateMovie>(rateObject);
        }

        private void TraktAddToWatchedList(MFMovie movie)
        {
          LogMyFilms.Debug("TraktAddToWatchedList(): Call with Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "'");
          new Thread(delegate()
          {
            try
            {
              TraktPlugin.TraktAPI.TraktAPI.SyncMovieLibrary(TraktPlugin.TraktHandlers.BasicHandler.CreateMovieSyncData(movie.Title, movie.Year.ToString(), movie.IMDBNumber), TraktPlugin.TraktAPI.TraktSyncModes.watchlist);
              TraktPlugin.GUI.GUIWatchListMovies.ClearCache(TraktPlugin.TraktSettings.Username);
            }
            catch (Exception ex)
            {
              LogMyFilms.Error("TraktAddToWatchedList(): Error - Exception '" + ex.Message + "'");
            }
          }) { Name = "MyFilms-AddFilmToTraktWatchlist", IsBackground = true }.Start();
        }

        private void TraktRelatedMovies(MFMovie movie)
        {
          LogMyFilms.Debug("TraktRelatedMovies(): Call with Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "'");
          TraktPlugin.TraktHelper.ShowRelatedMovies(movie.IMDBNumber, movie.Title, movie.Year.ToString());
        }

        private void TraktAddRemoveMovieInUserlist(MFMovie movie, bool remove)
        {
          LogMyFilms.Debug("TraktAddRemoveMovieInUserlist(): Call with 'remove = " + remove + "' - Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "', file = '" + movie.File + "', path = '" + movie.Path + "'");
          TraktPlugin.TraktHelper.AddRemoveMovieInUserList(movie.Title, movie.Year.ToString(), movie.IMDBNumber, remove);
        }

        private static MFMovie GetMovieFromRecord(DataRow sr)
        {
          MFMovie movie = new MFMovie();
          movie.Config = Configuration.CurrentConfig;
          if (!string.IsNullOrEmpty(sr["Number"].ToString()))
            movie.ID = Int32.Parse(sr["Number"].ToString());
          else movie.ID = 0;

          movie.Title = sr["OriginalTitle"].ToString();

          int year = 1900;
          Int32.TryParse(sr["Year"].ToString(), out year);
          movie.Year = year;

          bool played = false;
          if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
          {
            if (GetWatchedCount(MyFilms.conf.StrIndex, MyFilms.conf.StrUserProfileName) > 0)
              played = true;
          }
          else
          {
            if (MyFilms.conf.GlobalUnwatchedOnlyValue != null && MyFilms.conf.StrWatchedField.Length > 0)
              if (sr[MyFilms.conf.StrWatchedField].ToString().ToLower() != MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower()) // changed to take setup config into consideration
                played = true;
            //if (MyFilms.conf.StrSuppress && MyFilms.conf.StrSuppressField.Length > 0)
            //  if ((sr[MyFilms.conf.StrSuppressField].ToString() == MyFilms.conf.StrSuppressValue.ToString()) && (MyFilms.conf.StrSupPlayer))
            //    played = true;
          }
          movie.Watched = played;

          float rating = 0;
          bool success = float.TryParse(sr["Rating"].ToString(), out rating);
          if (!success) rating = 0;

          movie.Rating = rating;
          // movie.Rating = (float)Double.Parse(sr["Rating"].ToString());

          string mediapath = string.Empty;
          if (Helper.FieldIsSet(MyFilms.conf.StrStorage))
          {
            mediapath = sr[MyFilms.conf.StrStorage].ToString();
            if (mediapath.Contains(";")) // take the first source file
              mediapath = mediapath.Substring(0, mediapath.IndexOf(";"));
          }
          movie.File = mediapath;

          if (string.IsNullOrEmpty(mediapath)) // e.g. offline media files
            movie.File = movie.Title + " {offline} [" + movie.ID + "]";

          string currentPlayerFile = g_Player.CurrentFile;
          if (!string.IsNullOrEmpty(currentPlayerFile))
            movie.File = currentPlayerFile;
            
          string path = "";
          if (!string.IsNullOrEmpty(mediapath))
          {
            try
            {
              path = System.IO.Path.GetDirectoryName(mediapath);
            }
            catch (Exception)
            {
              if (!string.IsNullOrEmpty(movie.File))
                try
                {
                  path = System.IO.Path.GetDirectoryName(movie.File);
                }
                catch (Exception)
                {
                  path = "{search}";
                }
            }
            movie.Path = path;
          }
          else
          {
            movie.Path = "{offline}";
          }

          string IMDB = "";
          if (!string.IsNullOrEmpty(sr["IMDB_Id"].ToString()))
            IMDB = sr["IMDB_Id"].ToString();
          if (!string.IsNullOrEmpty(sr["URL"].ToString()) && string.IsNullOrEmpty(IMDB))
          {
            string CleanString = sr["URL"].ToString();
            Regex CutText = new Regex("" + @"tt\d{7}" + "");
            Match m = CutText.Match(CleanString);
            if (m.Success)
              IMDB = m.Value;
          }
          movie.IMDBNumber = IMDB;

          if (!string.IsNullOrEmpty(sr["TMDB_Id"].ToString()))
            movie.TMDBNumber = sr["TMDB_Id"].ToString();
          movie.DateAdded = sr["Date"].ToString();

          LogMyFilms.Info("GetMovieFromRecord(): Title = '" + movie.Title + "', year = '" + movie.Year + "', imdb = '" + movie.IMDBNumber + "', file = '" + movie.File + "', path = '" + movie.Path + "'");
          return movie;
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
                  LogMyFilms.Info("file deleted : " + t.ToString());
                }
                catch
                {
                  LogMyFilms.Info("unable to delete file : " + t.ToString());
                }
            }
          }
          if ((MyFilms.conf.StrSuppressType == "1") || (MyFilms.conf.StrSuppressType == "2"))
          {
            string wdelTitle = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
            MyFilms.r[Index].Delete();

            LogMyFilms.Info("Database movie deleted : " + wdelTitle);
          }
          else
          {
            MyFilms.r[Index][MyFilms.conf.StrSuppressField] = MyFilms.conf.StrSuppressValue.ToString();
            LogMyFilms.Info("Database movie updated for deletion : " + MyFilms.r[Index][MyFilms.conf.StrTitle1]);
          }
          Update_XML_database();
        }

        //-------------------------------------------------------------------------------------------
        //  Suppress an entry from the database
        //-------------------------------------------------------------------------------------------        
        public static void Manual_Delete(DataRow[] r1, int Index, bool removefromDB, bool deletefromStorage)
        {
          LogMyFilms.Info("Manual_Delete(): remove from DB = '" + removefromDB + "', delete from storage = '" + deletefromStorage + "'");
          if (deletefromStorage)
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
                  LogMyFilms.Info("file deleted : " + t.ToString());
                }
                catch (Exception ex)
                {
                  LogMyFilms.Info("unable to delete file : " + t.ToString());
                  LogMyFilms.InfoException("Manual_Delete() - delete file exception: ", ex);
                }
            }
          }
          if (removefromDB)
          {
            string wdelTitle = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
            MyFilms.r[Index].Delete();
            LogMyFilms.Info("Database movie deleted : " + wdelTitle);
            Update_XML_database();
          }
        }

        //-------------------------------------------------------------------------------------------
        //  Set an entry from the database to watched/unwatched
        //-------------------------------------------------------------------------------------------        
        // public static void Watched_Toggle(DataRow[] r1, int Index, bool watched)
        public static void Watched_Toggle(int Index, bool watched)
        {
          if (watched)
          {
            if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
              SetWatchedCount(Index, MyFilms.conf.StrUserProfileName, 1); //set watchedcount for enhanced watched count handling to 1
            else
              MyFilms.r[Index][MyFilms.conf.StrWatchedField] = "true";
          }
          else
          {
            if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
              SetWatchedCount(Index, MyFilms.conf.StrUserProfileName, 0); //set watchedcount for enhanced watched count handling to 0
            else
              MyFilms.r[Index][MyFilms.conf.StrWatchedField] = MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower();
          }
          LogMyFilms.Info("Database movie changed 'watchedstatus' by setting '" + MyFilms.conf.StrWatchedField + "' to '" + MyFilms.r[Index][MyFilms.conf.StrWatchedField] + "' for movie: " + MyFilms.r[Index][MyFilms.conf.StrTitle1]);

          Update_XML_database();
          
          // tell any listeners that user rated the movie
          MFMovie movie = new MFMovie();
          movie = GetMovieFromRecord(MyFilms.r[Index]);
          int count = 0;
          if (watched)
            count = 1;
          if (WatchedItem != null)
            WatchedItem(movie, watched, count);
        }

        //-------------------------------------------------------------------------------------------
        //  Set enhanced watch count
        //-------------------------------------------------------------------------------------------        
        public static void SetWatchedCount(int Index, string userprofilename, int count)
        {
          string EnhancedWatchedValue = MyFilms.r[Index][MyFilms.conf.StrWatchedField].ToString();
          string newEnhancedWatchedValue = string.Empty;

          if (EnhancedWatchedValue.Contains(userprofilename)) // Update count value
          {
            string[] split = EnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
              if (s.Contains(":"))
              {
                string sNew = s;
                string tempuser = MyFilmsDetail.EnhancedWatchedValue(s, "username");
                string tempcount = MyFilmsDetail.EnhancedWatchedValue(s, "count");
                string temprating = MyFilmsDetail.EnhancedWatchedValue(s, "rating");
                string tempdatewatched = MyFilmsDetail.EnhancedWatchedValue(s, "datewatched");

                if (tempuser == userprofilename) // Update Count Value
                {
                  sNew = tempuser + ":" + count + ":" + temprating + ":" + tempdatewatched;
                }
                if (string.IsNullOrEmpty(newEnhancedWatchedValue))
                  newEnhancedWatchedValue = sNew;
                else
                  newEnhancedWatchedValue += "|" + sNew;
              }
            }
          }
          else
          {
            if (string.IsNullOrEmpty(EnhancedWatchedValue) || !EnhancedWatchedValue.Contains(":")) // add count value
              newEnhancedWatchedValue = userprofilename + ":" + count + ":" + "-1:";
            else
              newEnhancedWatchedValue = EnhancedWatchedValue + "|" + userprofilename + ":" + count + ":" + "-1:";
          }
          MyFilms.r[Index][MyFilms.conf.StrWatchedField] = newEnhancedWatchedValue;
        }

        public static string EnhancedWatchedValue(string s, string type)
        {
          // "Global:0:-1|MikePlanet:0:-1" or "Global:0:-1|MikePlanet:0:-1:2011-12-24"
          string value = "";
          string[] split = s.Split(new Char[] { ':' });
          switch (type)
          {
            case "username":
              value = (split.Length > 0) ? split[0] : "";
              break;
            case "count":
              value = (split.Length > 1) ? split[1] : "0";
              break;
            case "rating":
              value = (split.Length > 2) ? split[2] : "-1";
              break;
            case "datewatched":
              value = (split.Length > 3) ? split[3] : "";
              break;
          }
          return value;
        }

        //-------------------------------------------------------------------------------------------
        //  Get enhanced watch count
        //-------------------------------------------------------------------------------------------        
        private static int GetWatchedCount(int Index, string userprofilename)
        {
          int count = 0;
          string EnhancedWatchedValue = MyFilms.r[Index][MyFilms.conf.StrWatchedField].ToString();

          if (EnhancedWatchedValue.Contains(userprofilename)) // Update count value
          {
            string[] split = EnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
              if (s.Contains(":"))
              {
                string tempuser = MyFilmsDetail.EnhancedWatchedValue(s, "username");
                string tempcount = MyFilmsDetail.EnhancedWatchedValue(s, "count");
                string temprating = MyFilmsDetail.EnhancedWatchedValue(s, "rating");
                string tempdatewatched = MyFilmsDetail.EnhancedWatchedValue(s, "datewatched");

                if (tempuser == userprofilename)
                {
                  bool success = int.TryParse(tempcount, out count);
                  if (!success) count = 0;
                  return count;
                }
              }
            }
          }
          return count;
        }

        //-------------------------------------------------------------------------------------------
        //  Get enhanced watch date
        //-------------------------------------------------------------------------------------------        
        private static DateTime GetWatchedDate(int Index, string userprofilename)
        {
          DateTime datewatched = DateTime.MinValue;
          string EnhancedWatchedValue = MyFilms.r[Index][MyFilms.conf.StrWatchedField].ToString();

          if (EnhancedWatchedValue.Contains(userprofilename)) // Update count value
          {
            string[] split = EnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
              if (s.Contains(":"))
              {
                string tempuser = MyFilmsDetail.EnhancedWatchedValue(s, "username");
                string tempcount = MyFilmsDetail.EnhancedWatchedValue(s, "count");
                string temprating = MyFilmsDetail.EnhancedWatchedValue(s, "rating");
                string tempdatewatched = MyFilmsDetail.EnhancedWatchedValue(s, "datewatched");

                if (tempuser == userprofilename)
                {
                  bool success = DateTime.TryParse(tempcount, out datewatched);
                  if (!success) datewatched = DateTime.MinValue;
                  return datewatched;
                }
              }
            }
          }
          return datewatched;
        }

        //-------------------------------------------------------------------------------------------
        //  Get enhanced user rating
        //-------------------------------------------------------------------------------------------        
        private static string GetUserRating(int Index, string userprofilename)
        {
          string EnhancedWatchedValue = MyFilms.r[Index][MyFilms.conf.StrWatchedField].ToString();

          if (EnhancedWatchedValue.Contains(userprofilename))
          {
            string[] split = EnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
              if (s.Contains(":"))
              {
                string tempuser = MyFilmsDetail.EnhancedWatchedValue(s, "username");
                string tempcount = MyFilmsDetail.EnhancedWatchedValue(s, "count");
                string temprating = MyFilmsDetail.EnhancedWatchedValue(s, "rating");
                string tempdatewatched = MyFilmsDetail.EnhancedWatchedValue(s, "datewatched");
                if (tempuser == userprofilename)
                {
                  return temprating;
                }
              }
            }
          }
          return "-1";
        }

        //-------------------------------------------------------------------------------------------
        //  Set enhanced user rating
        //-------------------------------------------------------------------------------------------        
        public static void SetUserRating(int Index, string userprofilename, string rating)
        {
          string EnhancedWatchedValue = MyFilms.r[Index][MyFilms.conf.StrWatchedField].ToString();
          string newEnhancedWatchedValue = string.Empty;

          if (EnhancedWatchedValue.Contains(userprofilename))
          {
            string[] split = EnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
              if (s.Contains(":"))
              {
                string sNew = s;
                string tempuser = MyFilmsDetail.EnhancedWatchedValue(s, "username");
                string tempcount = MyFilmsDetail.EnhancedWatchedValue(s, "count");
                string temprating = MyFilmsDetail.EnhancedWatchedValue(s, "rating");
                string tempdatewatched = MyFilmsDetail.EnhancedWatchedValue(s, "datewatched");

                if (tempuser == userprofilename) // Update Count Value
                {
                  sNew = tempuser + ":" + tempcount + ":" + rating + ":" + tempdatewatched;
                }
                if (string.IsNullOrEmpty(newEnhancedWatchedValue))
                  newEnhancedWatchedValue = sNew;
                else
                  newEnhancedWatchedValue += "|" + sNew;
              }
            }
          }
          else
          {
            if (string.IsNullOrEmpty(EnhancedWatchedValue) || !EnhancedWatchedValue.Contains(":")) // add count value
              newEnhancedWatchedValue = userprofilename + ":" + "0" + ":" + rating + ":";
            else
              newEnhancedWatchedValue = EnhancedWatchedValue + "|" + userprofilename + ":" + "0" + ":" + rating + ":";
          }
          MyFilms.r[Index][MyFilms.conf.StrWatchedField] = newEnhancedWatchedValue;
        }

        //-------------------------------------------------------------------------------------------
        //  Add watch count by one
        //-------------------------------------------------------------------------------------------        
        private static void AddWatchedCount(int Index, string userprofilename)
        {
          if (userprofilename != "Global") // avoid to add twice, if user selected "Global" as profile name
            AddWatchedByOne(Index, userprofilename);
          AddWatchedByOne(Index, "Global"); // Global watched count will always be set
          // ToDo: Could also populate "watched history" here (future version)

          // tell any listeners that user watched the movie
          MFMovie movie = new MFMovie();
          movie = GetMovieFromRecord(MyFilms.r[Index]);
          if (WatchedItem != null)
            WatchedItem(movie, true, GetWatchedCount(Index, userprofilename) + 1);
        }

        //-------------------------------------------------------------------------------------------
        //  Add watch count by one
        //-------------------------------------------------------------------------------------------        
        private static void AddWatchedByOne(int Index, string userprofilename)
        {
          int count = GetWatchedCount(Index, userprofilename);
          string EnhancedWatchedValue = MyFilms.r[Index][MyFilms.conf.StrWatchedField].ToString();
          string newEnhancedWatchedValue = string.Empty;
          CultureInfo InvC = CultureInfo.InvariantCulture;
          string datenow = DateTime.Now.ToString("d", InvC); // short date as invariant culture

          if (EnhancedWatchedValue.Contains(userprofilename)) // Update count value and add/update datewatched
          {
            string[] split = EnhancedWatchedValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
              if (s.Contains(":"))
              {
                string sNew = s;
                string tempuser = MyFilmsDetail.EnhancedWatchedValue(s, "username");
                string tempcount = MyFilmsDetail.EnhancedWatchedValue(s, "count");
                string temprating = MyFilmsDetail.EnhancedWatchedValue(s, "rating");
                string tempdatewatched = MyFilmsDetail.EnhancedWatchedValue(s, "datewatched");
                if (tempuser == userprofilename)
                {
                  sNew = tempuser + ":" + (count + 1) + ":" + temprating + ":" + datenow;
                }
                if (string.IsNullOrEmpty(newEnhancedWatchedValue))
                  newEnhancedWatchedValue = sNew;
                else
                  newEnhancedWatchedValue += "|" + sNew;
              }
            }
          }
          else
          {
            if (string.IsNullOrEmpty(EnhancedWatchedValue) || !EnhancedWatchedValue.Contains(":")) // add count value
              newEnhancedWatchedValue = userprofilename + ":" + (count + 1) + ":" + "-1:" + DateTime.Now.Date.ToShortDateString();
            else
              newEnhancedWatchedValue = EnhancedWatchedValue + "|" + userprofilename + ":" + (count + 1) + ":" + "-1:" + datenow;
          }
          MyFilms.r[Index][MyFilms.conf.StrWatchedField] = newEnhancedWatchedValue;
        }

        //-------------------------------------------------------------------------------------------
        //  Update the XML database and refresh screen
        //-------------------------------------------------------------------------------------------        
        public static void Update_XML_database()
        {
          //int maxretries = 10; // max retries 10 * 1000 = 10 seconds
          //int i = 0;
          //bool success = false; // result of update operation

          //while (!success && i < maxretries)
          //{
          //  // first check, if there is a global manual lock
          //  if (!GlobalLockIsActive(MyFilms.conf.StrFileXml))
          //  {
          //    SetGlobalLock(true, MyFilms.conf.StrFileXml);
          //    bool writesuccessful = BaseMesFilms.SaveMyFilms();
          //    SetGlobalLock(false, MyFilms.conf.StrFileXml);
          //    if (writesuccessful)
          //    {
          //      LogMyFilms.Info("Movie Database updated to filesystem!");
          //      success = true;
          //    }
          //  }
          //  else
          //  {
          //    i += 1;
          //    LogMyFilms.Info("Movie Database locked on try '" + i + " of " + maxretries + "' to write, waiting for next retry");
          //    Thread.Sleep(1000);
          //  }
          //}

          new System.Threading.Thread(delegate()
          {
            bool successwrite = BaseMesFilms.SaveMyFilms(MyFilms.conf.StrFileXml, 10000);

            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
            {
              // this will be executed after background thread finished
              if (!successwrite)
              {
                MyFilmsDetail.ShowNotificationDialog(GUILocalizeStrings.Get(1079861), "DB could not be updated (locked) !");
                LogMyFilms.Warn("Movie Database NOT updated due to GlobalLock ! - timeout passed.");
              }
              //else
              //  MyFilmsDetail.ShowNotificationDialog(GUILocalizeStrings.Get(1079861), "DB successfully updated !");
              return 0;
            }, 0, 0, null);
          }) { Name = "MyFilmsUpdateXML", IsBackground = true }.Start();
          return;
        }

        //-------------------------------------------------------------------------------------------
        //  Get Global Lock
        //-------------------------------------------------------------------------------------------        
        public static bool GlobalLockIsActive(string config)
        {
          string DB = "";
          if (string.IsNullOrEmpty(config)) 
            DB = MyFilms.conf.StrFileXml;
          else 
            DB = config;
          if (string.IsNullOrEmpty(DB))
          {
            DB = "";
            LogMyFilms.Debug("GlobalLockIsActive() - No valid DB - returning 'false' (DB-Config: '" + DB + "')");
            return false;
          }

          string path = System.IO.Path.GetDirectoryName(DB);
          string filename = System.IO.Path.GetFileNameWithoutExtension(DB);
          string machineName = System.Environment.MachineName;
          string[] files = System.IO.Directory.GetFiles(path, filename + @"*.lck", SearchOption.TopDirectoryOnly);
          if (files.Length > 0)
          {
            LogMyFilms.Debug("GlobalLockIsActive() - Global Lock detected ! (DB-Config: '" + DB + "') - First LockFile: '" + files[0] + "', Number LockFiles: '" + files.Length + "', Local MachineName: '" + machineName + "'");
            return true;
          }
          else
          {
            LogMyFilms.Debug("GlobalLockIsActive() - No Global Lock detected ! (DB-Config: '" + DB + "')");
            return false;
          }
        }

        //-------------------------------------------------------------------------------------------
        //  Set Global Lock
        //-------------------------------------------------------------------------------------------        
        public static void SetGlobalLock(bool lockstate, string config)
        {
          if (lockstate)
          {
            
            MyFilms.FSwatcher.EnableRaisingEvents = false; // stop FSwatcher for local update, otherwise unneeded reread would be triggered

            try
            {
              File.Create(LockFilename(config)).Dispose();
              LogMyFilms.Debug("SetGlobalLock() - successfully created global lock ! - " + LockFilename(config));
            }
            catch (Exception ex)
            {
              LogMyFilms.FatalException("SetGlobalLock() - Error creating Lockfile - check if file system rights properly set! - Lockfile: '" + LockFilename(config) + "', exception: " + ex.Message, ex);
              // throw;
            }
          }
          else
          {
            try
            {
              File.Delete(LockFilename(config));
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug("RemoveGlobalLock() - there seems to be a problem removing global lock - Message: " + ex.Message);
            }
            try
            {
              MyFilms.FSwatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug("RemoveGlobalLock()- FSwatcher - problem enabling Raisingevents - Message:  '" + ex.Message);
            }
            LogMyFilms.Debug("RemoveGlobalLock() - removed global lock ! - " + LockFilename(config));
          }
        }

        //-------------------------------------------------------------------------------------------
        //  Create machine specific Lock Filename
        //-------------------------------------------------------------------------------------------        
        private static string LockFilename(string config)
        {
          string lockfilename = "";
          string path = System.IO.Path.GetDirectoryName(config);
          string filename = System.IO.Path.GetFileNameWithoutExtension(config);
          string machineName = System.Environment.MachineName;
          lockfilename = path + @"\" + filename + "_" + machineName + ".lck";
          // LogMyFilms.Debug("LockFilename() - created lock file name is: '" + lockfilename + "'");
          return lockfilename;
        }

        public static bool AddMovieToCollection(string newGroupName)
        {
          string oldtitle = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
          string newtitle = "";
          if (oldtitle.IndexOf(MyFilms.conf.TitleDelim) > 0) // already has a groupname
          {
            LogMyFilms.Debug("AddMovieToCollection() - cannot add movie to collection '" + newGroupName + "' - already part of collection - oldtitle = '" + oldtitle + "'");
            return false;
          }
          else
          {
            newtitle = newGroupName + MyFilms.conf.TitleDelim + oldtitle;
            LogMyFilms.Debug("AddMovieToCollection() - changing title from '" + oldtitle + "' to '" + newtitle + "'");
            MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1] = newtitle;
            Update_XML_database();
            return true;
          }
        }

        public static bool RemoveMovieFromCollection()
        {
          string oldtitle = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
          string newtitle = "";
          if (oldtitle.IndexOf(MyFilms.conf.TitleDelim) > 0)
          {
            newtitle = oldtitle.Substring(oldtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1);
            LogMyFilms.Debug("RemoveMovieFromCollection() - changing title from '" + oldtitle + "' to '" + newtitle + "'");
            MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1] = newtitle;
            Update_XML_database();
            return true;
          }
          else
          {
            LogMyFilms.Debug("RemoveMovieFromCollection() - cannot remove from collection - oldtitle = '" + oldtitle + "'");
            return false;
          }
        }

        //-------------------------------------------------------------------------------------------
        //  Get Search title
        //-------------------------------------------------------------------------------------------        
        public static string GetSearchTitle(DataRow[] r1, int index, string titleoption)
        {
          string title = "";
          if (Helper.FieldIsSet(MyFilms.conf.ItemSearchGrabber) && !string.IsNullOrEmpty(r1[index][MyFilms.conf.ItemSearchGrabber].ToString()))
          {
            title = r1[index][MyFilms.conf.ItemSearchGrabber].ToString(); // Configured GrabberTitle
            LogMyFilms.Debug("GetSearchTitle() - selecting searchtitle with '" + MyFilms.conf.ItemSearchGrabber + "' = '" + title.ToString() + "'");
          }
          else if (Helper.FieldIsSet(MyFilms.conf.StrTitle1) && !string.IsNullOrEmpty(r1[index][MyFilms.conf.StrTitle1].ToString())) // Master Title
          {
            title = r1[index][MyFilms.conf.StrTitle1].ToString();
            LogMyFilms.Debug("GetSearchTitle() - selecting searchtitle with (master)title = '" + title.ToString() + "'");
          }
          else if (Helper.FieldIsSet(MyFilms.conf.StrTitle2) && !string.IsNullOrEmpty(r1[index][MyFilms.conf.StrTitle2].ToString())) // Secondary title
          {
            title = r1[index][MyFilms.conf.StrTitle2].ToString();
            LogMyFilms.Debug("GetSearchTitle() - selecting searchtitle with (secondary)title = '" + title.ToString() + "'");
          }
          else if (Helper.FieldIsSet(MyFilms.conf.StrStorage) && !string.IsNullOrEmpty(r1[index][MyFilms.conf.StrStorage].ToString())) // Name from source (media)
          {
            title = r1[index][MyFilms.conf.StrStorage].ToString();
            if (title.Contains(";")) title = title.Substring(0, title.IndexOf(";"));
            if (title.Contains("\\")) title = title.Substring(title.LastIndexOf("\\") + 1);
            if (title.Contains(".")) title = title.Substring(0, title.LastIndexOf("."));
            LogMyFilms.Debug("GetSearchTitle() - selecting searchtitle with (media source)name = '" + title.ToString() + "'");
          }

          if (title.IndexOf(MyFilms.conf.TitleDelim) > 0)
            title = title.Substring(title.IndexOf(MyFilms.conf.TitleDelim) + 1);
          LogMyFilms.Debug("GetSearchTitle() - returning (search)title = '" + title + "'"); 
          return title;
        }

        public static string GetMediaPathOfFirstFile(DataRow[] r1, int index)
        {
          string mediapath = "";

          // for catalog using "storage field"
          if (Helper.FieldIsSet(MyFilms.conf.StrStorage))
          {
            try
            {
              mediapath = r1[index][MyFilms.conf.StrStorage].ToString();
              if (mediapath.Contains(";"))
                mediapath = mediapath.Substring(0, mediapath.IndexOf(";"));
            }
            catch
            {
              mediapath = string.Empty;
            }
          }

          // for catalog using "search" instead storage field
          if (string.IsNullOrEmpty(mediapath)) // use search method only if required...
          {
            if ((MyFilms.conf.SearchFile == "True") || (MyFilms.conf.SearchFile == "yes"))
            {
              string movieName = r1[index][MyFilms.conf.ItemSearchFile].ToString();
              movieName = movieName.Substring(movieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
              if (MyFilms.conf.ItemSearchFile.Length > 0)
              {
                mediapath = Search_FileName(movieName, MyFilms.conf.StrDirStor).Trim();
              }
            }
          }

          return mediapath;
        }

        private static bool ShouldGrabberBeAdded(GrabberScript script, GrabType grabtype, bool showallLanguages)
        {
          bool add = false;
          string currentlanguagefilter = "";
          if (!string.IsNullOrEmpty(MyFilms.conf.ItemSearchGrabberScriptsFilter))
            currentlanguagefilter = MyFilms.conf.ItemSearchGrabberScriptsFilter;
          string[] Sep = new string[] { ",", ";", "|", "/", ".", @"\", ":" };
          switch (grabtype)
          {
            case GrabType.All:
            case GrabType.Details:
            case GrabType.Cover:
            case GrabType.MultiCovers:
            case GrabType.Fanart:
            case GrabType.Person:
            case GrabType.Photos:
            case GrabType.Trailers:
              break;
            default:
              break;
          }

          // check, if it meets filter criteria
          string[] allowedlanguages = currentlanguagefilter.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
          string[] supportedlanguages = script.Language.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
          string[] supportedfunctions = script.Type.Split(Sep, StringSplitOptions.RemoveEmptyEntries);

          if (string.IsNullOrEmpty(currentlanguagefilter) || currentlanguagefilter.Contains("*")) // if there is no filter set in config or override add script anyway...
          {
            showallLanguages = true;
          }

          if (supportedfunctions.Length == 0 && (grabtype == GrabType.Details || grabtype == GrabType.All)) // if there is no functions set and trying legacy menu ...
              add = true;
          else
          {
            foreach (string supportedfunction in supportedfunctions)
            {
              if (supportedfunction.Trim().ToLower() == grabtype.ToString().ToLower() || (supportedfunction.Trim().ToLower() == "details" && grabtype.ToString().ToLower() == "all"))
              {
                if (showallLanguages)
                  add = true;
                else
                {
                  foreach (string allowedlanguage in allowedlanguages)
                  {
                    foreach (string supportedlanguage in supportedlanguages)
                    {
                      if (supportedlanguage.Trim().ToLower() == allowedlanguage.Trim().ToLower())
                        add = true;
                    }
                  }
                }
              }
            }
          }
          return add;
        }

        //-------------------------------------------------------------------------------------------
        //  Grab URL Internet Movie Informations and update the XML database and refresh screen
        //  -> Selection of grabber script
        //-------------------------------------------------------------------------------------------        
        public static void grabb_Internet_Informations(string FullMovieName, int GetID, bool choosescript, string wscript, string FullMoviePath, GrabType grabtype, bool showAll, Searchtitles sTitles)
        {
          LogMyFilms.Debug("(grabb_Internet_Informations) with grabtype = '" + grabtype + "', title = '" + FullMovieName + "', choosescript = '" + choosescript + "', grabberfile = '" + wscript + "'");
          if (choosescript)
          {
            if (!System.IO.Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts)))
            {
              GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
              dlgOk.SetHeading(GUILocalizeStrings.Get(645)); // menu
              dlgOk.SetLine(1, string.Format(GUILocalizeStrings.Get(1079876), MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts)));
              dlgOk.SetLine(2, GUILocalizeStrings.Get(1079877));
              dlgOk.DoModal(GetID);
              LogMyFilms.Info("My Films : The Directory grabber config files doesn't exists. Verify your Configuration !");
              return;
            }

            if (!System.IO.Directory.Exists(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\user"))
            {
              try { System.IO.Directory.CreateDirectory(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts) + @"\user"); }
              catch (Exception ex) { LogMyFilms.Debug("Error creating user script directory: '" + ex.Message + "'"); }
            }

            // Grabber Directory filled, search for XML scripts files
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(10798706)); // "Choose internet grabber script"
            if (dlg == null) return;
            ArrayList scriptfile = new ArrayList();

            if (MyFilms.conf.StrGrabber_cnf.Length > 0 && (grabtype == GrabType.Details || grabtype == GrabType.All))
            {
              try
              {
              GrabberScript defaultScript = new GrabberScript(MyFilms.conf.StrGrabber_cnf);
              defaultScript.Load(MyFilms.conf.StrGrabber_cnf);
              scriptfile.Add(MyFilms.conf.StrGrabber_cnf);
              dlg.Add(MyFilms.conf.StrGrabber_cnf.Substring(MyFilms.conf.StrGrabber_cnf.LastIndexOf("\\") + 1) + " (default)");
              dlg.SelectedLabel = 0;
              }
              catch (Exception)
              {
                LogMyFilms.Error("The default script is not compatible with current MyFilms version - please change your settings !");
              }
            }
            DirectoryInfo dirsInf = new DirectoryInfo(MyFilmsSettings.GetPath(MyFilmsSettings.Path.GrabberScripts));
            FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();

            foreach (FileSystemInfo sfi in sfiles)
            {
              if ((sfi.Extension.ToLower() == ".xml") && (sfi.FullName != MyFilms.conf.StrGrabber_cnf))
              {
                GrabberScript script = new GrabberScript(sfi.FullName);
                script.Load(sfi.FullName);
                string displayName = "";
                string displayNamePost = "";
                switch (grabtype)
                {
                  case GrabType.MultiCovers:
                    if (script.Type.ToLower().Contains("multicovers"))
                      displayNamePost = " - (Multi Cover)";
                    else
                      displayNamePost = " - (Single Cover)";
                    break;
                  default:
                    break;
                }
                if (ShouldGrabberBeAdded(script, grabtype, showAll))
                {
                  if (!string.IsNullOrEmpty(script.DBName))
                  {
                    displayName += script.DBName;
                    if (!string.IsNullOrEmpty(script.Language)) displayName += " (" + script.Language + ")";
                    displayName += displayNamePost;
                  }
                  else displayName += sfi.Name;
                  dlg.Add(displayName);
                  scriptfile.Add(sfi.FullName);
                }
              }
            }
            // add choice to show all languages
            if (!showAll && !string.IsNullOrEmpty(MyFilms.conf.ItemSearchGrabberScriptsFilter))
              dlg.Add(GUILocalizeStrings.Get(10798765)); // Show all

            if (scriptfile.Count > 0)
            {
              dlg.DoModal(GetID);
              if (dlg.SelectedLabel == -1)
                return;
              if (dlg.SelectedLabelText == GUILocalizeStrings.Get(10798765))
              {
                grabb_Internet_Informations(FullMovieName, GetID, true, wscript, FullMoviePath, grabtype, true, sTitles);
                return;
              }
              if (dlg.SelectedLabel > -1)
                wscript = scriptfile[dlg.SelectedLabel].ToString();
            }
          }
          grabb_Internet_Informations_Search(FullMovieName, GetID, wscript, FullMoviePath, grabtype, sTitles);
        }
      
        //-------------------------------------------------------------------------------------------
        //  Grab URL Internet Movie Informations and update the XML database and refresh screen
        //  -> Search and select matching movie
        //-------------------------------------------------------------------------------------------        
        public static void grabb_Internet_Informations_Search(string FullMovieName, int GetID, string wscript, string FullMoviePath, GrabType grabtype, Searchtitles sTitles)
        {
            if (string.IsNullOrEmpty(wscript)) return;
            LogMyFilms.Debug("grabb_Internet_Informations_Search() with title = '" + FullMovieName + "', grabberfile = '" + wscript + "'");
            string MovieName = FullMovieName;
            string MovieHierarchy = string.Empty;
            string MoviePath = FullMoviePath;
            if (MyFilms.conf.TitleDelim.Length > 0)
            {
                MovieName = FullMovieName.Substring(FullMovieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
                MovieHierarchy = FullMovieName.Substring(0, FullMovieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
            }
            Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            Grabber_URLClass.IMDBUrl wurl;
            ArrayList listUrl = new ArrayList();

            new System.Threading.Thread(delegate()
              {
                GUIWaitCursor.Init(); GUIWaitCursor.Show();
                try
                {
                  // listUrl = Grab.ReturnURL(MovieName, wscript, 1, !MyFilms.conf.StrGrabber_Always, MoviePath); // MoviePath only when nfo reader used !!!
                  listUrl = Grab.ReturnURL(MovieName, wscript, 1, !MyFilms.conf.StrGrabber_Always, "");
                }
                catch (Exception ex)
                {
                  LogMyFilms.ErrorException("grabb_Internet_Details_Search() - exception = '" + ex.Message + "'", ex);
                }
                GUIWaitCursor.Hide();

                int listCount = listUrl.Count;
                if (!MyFilms.conf.StrGrabber_Always)
                    listCount = 2;
                switch (listCount)
                {
                    case 1: // only one match -> grab details without user interaction
                        wurl = (Grabber.Grabber_URLClass.IMDBUrl)listUrl[0];
                        grabb_Internet_Details_Informations(wurl.URL, MovieHierarchy, wscript, GetID, false, grabtype, sTitles);
                        break;
                    case 0:
                        break;
                    default:
                        #region manual choice
                        GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        List<string> choiceViewMenu = new List<string>();
                        choiceViewMenu.Clear();

                        if (dlg == null) return;
                        dlg.Reset();
                        dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                        dlg.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
                        choiceViewMenu.Add("manual selection");

                        for (int i = 0; i < listUrl.Count; i++)
                        {
                          wurl = (Grabber.Grabber_URLClass.IMDBUrl)listUrl[i];
                          if (wurl.Director.Contains(MyFilms.r[MyFilms.conf.StrIndex]["Director"].ToString()) && wurl.Year.Contains(MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString()) && (!MyFilms.conf.StrGrabber_Always))
                          {
                              if (dlg.SelectedLabel == -1)
                                  dlg.SelectedLabel = i + 1;
                              else
                                  dlg.SelectedLabel = -2;
                          }
                          string viewTitle = wurl.Title;
                          if (!string.IsNullOrEmpty(wurl.Year)) viewTitle += " (" + wurl.Year + ")";
                          if (!string.IsNullOrEmpty(wurl.Options)) viewTitle += " - " + wurl.Options + "";
                          dlg.Add(viewTitle);  
                          choiceViewMenu.Add(wurl.Title);
                        }
                        string[] split = MovieName.Trim().Split(new Char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in split)
                        {
                          if (s.Length > 2)
                          {
                            dlg.Add(GUILocalizeStrings.Get(137) + " '" + s + "'"); // add words from title as search items
                            choiceViewMenu.Add(s);
                          }
                        }
                        if (grabtype != GrabType.Person)
                        {
                          #region add all titles as alternative search expressions
                          if (!string.IsNullOrEmpty(sTitles.OriginalTitle) && !choiceViewMenu.Contains(sTitles.OriginalTitle))
                          {
                            dlg.Add(GUILocalizeStrings.Get(137) + " '" + sTitles.OriginalTitle + "'"); //search
                            choiceViewMenu.Add(sTitles.OriginalTitle);
                          }
                          if (!string.IsNullOrEmpty(sTitles.TranslatedTitle) && !choiceViewMenu.Contains(sTitles.TranslatedTitle))
                          {
                            dlg.Add(GUILocalizeStrings.Get(137) + " '" + sTitles.TranslatedTitle + "'");
                            choiceViewMenu.Add(sTitles.TranslatedTitle);
                          }
                          if (!string.IsNullOrEmpty(sTitles.FormattedTitle) && !choiceViewMenu.Contains(sTitles.FormattedTitle))
                          {
                            dlg.Add(GUILocalizeStrings.Get(137) + " '" + sTitles.FormattedTitle + "'");
                            choiceViewMenu.Add(sTitles.FormattedTitle);
                          }
                          if (!string.IsNullOrEmpty(sTitles.MovieDirectoryTitle) && !choiceViewMenu.Contains(sTitles.MovieDirectoryTitle))
                          {
                            dlg.Add(GUILocalizeStrings.Get(137) + " '" + sTitles.MovieDirectoryTitle + "'");
                            choiceViewMenu.Add(sTitles.MovieDirectoryTitle);
                          }
                          if (!string.IsNullOrEmpty(sTitles.MovieFileTitle) && !choiceViewMenu.Contains(sTitles.MovieFileTitle))
                          {
                            dlg.Add(GUILocalizeStrings.Get(137) + " '" + sTitles.MovieFileTitle + "'");
                            choiceViewMenu.Add(sTitles.MovieFileTitle);
                          }
                          #endregion
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
                            if (keyboard.IsConfirmed && keyboard.Text.Length > 0)
                               grabb_Internet_Informations_Search(keyboard.Text, GetID, wscript, MoviePath, grabtype, sTitles);
                            break;
                        }
                        if (dlg.SelectedLabel > 0 && dlg.SelectedLabel <= listUrl.Count)
                        {
                            wurl = (Grabber_URLClass.IMDBUrl)listUrl[dlg.SelectedLabel - 1];
                            grabb_Internet_Details_Informations(wurl.URL, MovieHierarchy, wscript, GetID, true, grabtype, sTitles);
                            break;
                        }
                        if (dlg.SelectedLabel > listUrl.Count)
                        {
                          //VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                          //if (null == keyboard) return;
                          //keyboard.Reset();
                          //keyboard.Text = dlg.SelectedLabelText;
                          //// keyboard.Text = choiceViewMenu[dlg.SelectedLabel];
                          //keyboard.DoModal(GetID);
                          //if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                          //  grabb_Internet_Informations_Search(keyboard.Text, GetID, wscript, MoviePath, grabtype, sTitles);
                          string strChoice = choiceViewMenu[dlg.SelectedLabel];
                          LogMyFilms.Debug("grabb_Internet_Informations_Search(): (re)search with new search expression: '" + strChoice + "'");
                          grabb_Internet_Informations_Search(strChoice, GetID, wscript, MoviePath, grabtype, sTitles);
                          break;
                        }
                        break;
                        #endregion
                }
            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) => { return 0; }, 0, 0, null);
              }) { Name = "MyFilmsDetailsLoader", IsBackground = true }.Start();
            return;
        }

        //-------------------------------------------------------------------------------------------
        //  Grab Internet Movie Details Informations and update the XML database and refresh screen
        //-------------------------------------------------------------------------------------------        
        public static void grabb_Internet_Details_Informations(string url, string moviehead, string wscript, int GetID, bool interactive, GrabType grabtype, Searchtitles sTitles)
        {
          LogMyFilms.Debug("launching (grabb_Internet_Details_Informations) with url = '" + url + "', moviehead = '" + moviehead + "', wscript = '" + wscript + "', GetID = '" + GetID + "', interactive = '" + interactive + "'");

          #region set environment
          Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
          string[] Result = new string[80];
          string title = string.Empty;
          string ttitle = string.Empty;
          string wtitle = string.Empty;
          int year = 0;
          string director = string.Empty;
          // Those settings were used in the past from AMCupdater settings - now they exist in MF config as primary source!
          // XmlConfig XmlConfig = new XmlConfig();
          // string Img_Path = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Image_Download_Filename_Prefix", "");
          // string Img_Path_Type = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Store_Image_With_Relative_Path", "false");

          string downLoadPath; //downLoadPath = MyFilmsSettings.GetPath(MyFilmsSettings.Path.MyFilmsPath);
          if (interactive)
            downLoadPath = Path.GetTempPath();
          else
          {
            if (grabtype == GrabType.Person) downLoadPath = MyFilms.conf.StrPathArtist;
            else downLoadPath = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix;
          }
          LogMyFilms.Debug("Grabber - GetDetail: OverrideLanguage = '" + MyFilms.conf.GrabberOverrideLanguage + "', OverridePersonLimit = '" + MyFilms.conf.GrabberOverridePersonLimit + "', OverrideTitleLimit = '" + MyFilms.conf.GrabberOverrideTitleLimit + "', Get Roles = '" + MyFilms.conf.GrabberOverrideGetRoles + "'");
          LogMyFilms.Debug("Grabber - GetDetail: script = '" + wscript + "', url = '" + url + "', download path = '" + downLoadPath + "'");
          #endregion

          new System.Threading.Thread(delegate()
              {
                #region load internet data
                GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
                if (interactive)
                {
                  // GUIWaitCursor.Init(); GUIWaitCursor.Show();
                  if (dlgPrgrs != null)
                  {
                    dlgPrgrs.Reset();
                    dlgPrgrs.DisplayProgressBar = false;
                    dlgPrgrs.ShowWaitCursor = true;
                    dlgPrgrs.DisableCancel(true);
                    dlgPrgrs.SetHeading(string.Format("{0} - {1}", "MyFilms", "Internet Updates"));
                    dlgPrgrs.SetLine(1, "Loading Details ...");
                    dlgPrgrs.Percentage = 0;
                    dlgPrgrs.NeedRefresh();
                    dlgPrgrs.ShouldRenderLayer();
                    dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
                  }

                }
                try
                {
                  // Result = Grab.GetDetail(url, downLoadPath, wscript);
                  Result = Grab.GetDetail(url, downLoadPath, wscript, true, MyFilms.conf.GrabberOverrideLanguage, MyFilms.conf.GrabberOverridePersonLimit, MyFilms.conf.GrabberOverrideTitleLimit, MyFilms.conf.GrabberOverrideGetRoles);
                }
                catch (Exception ex) { LogMyFilms.ErrorException("grabb_Internet_Details_Information() - exception = '" + ex.Message + "'", ex); }

                if (interactive)
                {
                  GUIWaitCursor.Hide();
                  if (dlgPrgrs != null)
                  {
                    dlgPrgrs.ShowWaitCursor = false; 
                    dlgPrgrs.Close();
                  }
                }

                // copy mapped values to original values
                for (int i = 0; i < 40; i++)
                {
                  LogMyFilms.Debug("Grabber Details: original: '" + i + "' - '" + Result[i] + "'");
                  Result[i] = Result[i + 40];
                  LogMyFilms.Debug("Grabber Details: mapped  : '" + i + "' - '" + Result[i] + "'");
                }
                LogMyFilms.Debug("Grab Internet Information done for title/ttitle: " + MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] + "/" + MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"]);

                // string Title_Group = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Folder_Name_Is_Group_Name", "false");
                // string Title_Group_Apply = XmlConfig.ReadAMCUXmlConfig(MyFilms.conf.StrAMCUpd_cnf, "Group_Name_Applies_To", "");
                #endregion

                if (grabtype == GrabType.Details || grabtype == GrabType.All) // grabtype "all" includes cover
                {
                  #region Details

                  string strChoice = "all"; // defaults to "all", if no other choice
                  bool onlyselected = false;
                  bool onlymissing = false;
                  bool onlynonempty = false;
                  List<string> choiceViewMenu = new List<string>();
                  List<string> updateItems = new List<string>(); // store properties to update for later use ...
                  int iPropertyLengthLimit = 33;
                  string[] PropertyList = new string[] {
                        "OriginalTitle", "TranslatedTitle", "Picture", "Description", "Rating", "Actors", "Director",
                        "Producer", "Year", "Country", "Category", "URL", "ImageURL", "Writer", "Comments", "Languages",
                        "TagLine", "Certification", "IMDB_Id", "IMDB_Rank", "Studio", "Edition", "Fanart", "Generic1",
                        "Generic2", "Generic3", "TranslatedTitleAllNames", "TranslatedTitleAllValues",
                        "CertificationAllNames", "CertificationAllValues", "MultiPosters", "Photos", "PersonImages",
                        "MultiFanart", "Trailer", "TMDB_Id", "Empty36", "Empty37", "Empty38", "Empty39"
                      };

                  if (interactive) // Dialog only in interactive mode
                  {
                    #region interactive selection dialog
                    GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    bool returnToMainDialog = false; // by default do NOT repeat the dialog - only if returning from sub dialogs
                    do
                    {
                      #region set main selection dialog for options
                      returnToMainDialog = false;
                      choiceViewMenu.Clear();
                      dlgmenu.Reset();
                      dlgmenu.SetHeading(GUILocalizeStrings.Get(10798797)); // Choose update option ...
                      dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798734) + " *** ");
                      choiceViewMenu.Add("all");
                      dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798735) + " *** ");
                      choiceViewMenu.Add("missing");
                      dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798730) + " *** ");
                      choiceViewMenu.Add("all-onlynewdata");
                      // disabled, as we now have the multiselect dialog
                      //dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798798) + " *** "); // Select single field for update ...
                      //choiceViewMenu.Add("singlefield");
                      if (File.Exists(GUIGraphicsContext.Skin + @"\MyFilmsDialogMultiSelect.xml"))
                      {
                        dlgmenu.Add(" *** " + GUILocalizeStrings.Get(10798799) + " *** ");
                          // Select multiple fields for update ...
                        choiceViewMenu.Add("multiplefields");
                      }
                      dlgmenu.DoModal(GetID);
                      if (dlgmenu.SelectedLabel == -1) return;
                      strChoice = choiceViewMenu[dlgmenu.SelectedLabel];

                      #endregion

                      if (strChoice == "singlefield")
                      {
                        #region populate select menu, if user has chosen to ...

                        string strOldValue = "";
                        string strNewValue = "";

                        GUIDialogSelect dlgSelect = (GUIDialogSelect)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_SELECT); // GUIDialogSelect2 dlgmenu = (GUIDialogSelect2)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_SELECT2);  
                        if (dlgSelect == null) return;
                        choiceViewMenu.Clear();
                        dlgSelect.Reset();
                        dlgSelect.SetHeading(GUILocalizeStrings.Get(10798732)); // choose property to update
                        dlgSelect.SetButtonLabel("");

                        int i = 0;
                        foreach (string wProperty in PropertyList)
                        {
                          try
                          {
                            strOldValue = MyFilms.r[MyFilms.conf.StrIndex][wProperty].ToString();
                            strNewValue = Result[i];
                            if (i == 2) strNewValue = Result[12];
                            if (strNewValue == null) strNewValue = "";

                            if ( // make sure, only supported fields are offered to user for update
                              wProperty != "ImageURL" && !wProperty.Contains("Sub") && !wProperty.Contains("All") &&
                              !wProperty.Contains("Generic") && !wProperty.Contains("Empty") &&
                              ((wProperty != "TagLine" && wProperty != "Certification" && wProperty != "Writer" &&
                                wProperty != "Studio" && wProperty != "Edition" && wProperty != "IMDB_Id" &&
                                wProperty != "IMDB_Rank" && wProperty != "TMDB_Id") ||
                               MyFilms.conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended) &&
                              wProperty != "Fanart" && wProperty != "Aspectratio" && wProperty != "MultiPosters"
                              // set to enabled to get proper selection - WIP
                              && wProperty != "Photos" && wProperty != "PersonImages" && wProperty != "MultiFanart" &&
                              wProperty != "Trailer")
                            {
                              dlgSelect.Add(
                                BaseMesFilms.Translate_Column(wProperty) + ": '" +
                                Helper.LimitString(
                                  strOldValue.Replace(Environment.NewLine, " # "), iPropertyLengthLimit) + "' -> '" +
                                Helper.LimitString(
                                  strNewValue.Replace(Environment.NewLine, " # "), iPropertyLengthLimit) + "'");
                              choiceViewMenu.Add(wProperty);
                              LogMyFilms.Debug(
                                "GrabberUpdate - Add (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue +
                                "'");
                            }
                            else
                            {
                              LogMyFilms.Debug(
                                "GrabberUpdate - not added (unsupported) - (" + wProperty + "): '" + strOldValue +
                                "' -> '" + strNewValue + "'");
                            }
                          }
                          catch
                          {
                            LogMyFilms.Debug(
                              "GrabberUpdate - cannot adding Property '" + wProperty + "' to Selectionmenu");
                          }
                          i = i + 1;
                        }

                        dlgSelect.DoModal(GetID);
                        if (dlgSelect.SelectedLabel == -1) returnToMainDialog = true; // Nothing was selected - return to main selection menu
                        strChoice = choiceViewMenu[dlgmenu.SelectedLabel];

                        #endregion
                      }

                      if (choiceViewMenu[dlgmenu.SelectedLabel] == "multiplefields")
                      {
                        #region populate multi selection menu, if user has chosen to ...

                        string strOldValue = "";
                        string strNewValue = "";

                        GUIWindow dlgMultiSelectOld = (GUIWindow)GUIWindowManager.GetWindow(2100);
                        GUIDialogMultiSelect dlgMultiSelect = new GUIDialogMultiSelect();
                        if (dlgMultiSelect == null) return;
                        dlgMultiSelect.Init();
                        GUIWindowManager.Replace(2100, dlgMultiSelect);
                        try
                        {
                          dlgMultiSelect.Reset();
                          dlgMultiSelect.SetHeading(GUILocalizeStrings.Get(10798732)); // choose property to update

                          int i = 0;
                          foreach (string wProperty in PropertyList)
                          {
                            try
                            {
                              strOldValue = MyFilms.r[MyFilms.conf.StrIndex][wProperty].ToString();
                              strNewValue = Result[i];
                              if (i == (int)Grabber_URLClass.Grabber_Output.PicturePathLong) strNewValue = Result[(int)Grabber_URLClass.Grabber_Output.PicturePathShort];
                              if (strNewValue == null) strNewValue = "";

                              if ( // make sure, only supported fields are offered to user for update
                                wProperty != "ImageURL" && !wProperty.Contains("Sub") && !wProperty.Contains("All") &&
                                !wProperty.Contains("Generic") && !wProperty.Contains("Empty") &&
                                ((wProperty != "TagLine" && wProperty != "Certification" && wProperty != "Writer" &&
                                  wProperty != "Studio" && wProperty != "Edition" && wProperty != "IMDB_Id" &&
                                  wProperty != "IMDB_Rank" && wProperty != "TMDB_Id") ||
                                 MyFilms.conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended) &&
                                wProperty != "Fanart" && wProperty != "Aspectratio" && wProperty != "MultiPosters"
                                // set to enabled to get proper selection - WIP
                                && wProperty != "Photos" && wProperty != "PersonImages" && wProperty != "MultiFanart" &&
                                wProperty != "Trailer")
                              {
                                GUIListItem pItem = new GUIListItem(wProperty);
                                pItem.TVTag = wProperty;
                                if (i == (int)Grabber_URLClass.Grabber_Output.PicturePathLong) pItem.IconImage = Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong];
                                pItem.Selected = false;
                                pItem.Label = BaseMesFilms.Translate_Column(wProperty) + ": '" +
                                              Helper.LimitString(
                                                strOldValue.Replace(Environment.NewLine, " # "), iPropertyLengthLimit) +
                                              "' -> '" +
                                              Helper.LimitString(
                                                strNewValue.Replace(Environment.NewLine, " # "), iPropertyLengthLimit) +
                                              "'";
                                dlgMultiSelect.Add(pItem);
                                LogMyFilms.Debug("GrabberUpdate - Add (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                              }
                              else
                              {
                                LogMyFilms.Debug("GrabberUpdate - not added (unsupported) - (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                              }
                            }
                            catch
                            {
                              LogMyFilms.Debug("GrabberUpdate - cannot adding Property '" + wProperty + "' to Selectionmenu");
                            }
                            i = i + 1;
                          }

                          dlgMultiSelect.DoModal(GetID);
                          for (int ii = 0; ii < dlgMultiSelect.ListItems.Count; ii++)
                          {
                            if (dlgMultiSelect.ListItems[ii].Selected) updateItems.Add(dlgMultiSelect.ListItems[ii].TVTag.ToString());
                          }
                          LogMyFilms.Debug("GrabberUpdate - '" + updateItems.Count + "' updateItems selected !");
                          if (dlgMultiSelect.DialogModalResult == ModalResult.Cancel) return; // user cancelled
                          if (dlgMultiSelect.DialogModalResult == ModalResult.OK && updateItems.Count == 0) return; // Nothing was selected
                          if (dlgMultiSelect.DialogModalResult != ModalResult.OK) returnToMainDialog = true; // user wants to return to options menu
                        }
                        finally
                        {
                          GUIWindowManager.Replace(2100, dlgMultiSelectOld);
                        }

                        #endregion
                      }

                      switch (strChoice) // either an update type - or a single property to update
                      {
                          #region switch update options

                        case "all":
                          onlyselected = false;
                          onlymissing = false;
                          onlynonempty = false;
                          break;
                        case "missing":
                          onlyselected = false;
                          onlymissing = true;
                          onlynonempty = false;
                          break;
                        case "all-onlynewdata":
                          onlyselected = false;
                          onlymissing = false;
                          onlynonempty = true;
                          break;
                        default:
                          onlyselected = true;
                          onlymissing = false;
                          onlynonempty = false;
                          break;

                          #endregion
                      }
                    }
                    while (returnToMainDialog);
                    LogMyFilms.Debug("GrabInternetDetails - interactive choice: '" + strChoice + "', onlyselected = '" + onlyselected + "', onlymissing = '" + onlymissing + "', onlynonempty = '" + onlynonempty + "'");
                    #endregion
                  }

                  #region load details data

                  // ********************************** now load data, if requested ! ******************************
                  if (IsUpdateRequired("OriginalTitle", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  {
                    title = Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle];
                    wtitle = MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString();
                    if (wtitle.Contains(MyFilms.conf.TitleDelim)) wtitle = wtitle.Substring(wtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1);
                    if (wtitle != title) Remove_Backdrops_Fanart(wtitle, true);

                    if (MyFilms.conf.StrTitle1 == "OriginalTitle") MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] = moviehead + title;
                    else MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"] = title;
                  }

                  if (IsUpdateRequired("TranslatedTitle", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.TranslatedTitle], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  {
                    ttitle = Result[(int)Grabber_URLClass.Grabber_Output.TranslatedTitle];
                    if ("TranslatedTitle" == strChoice)
                    {
                      if (string.IsNullOrEmpty(ttitle) && MyFilms.conf.StrTitle1 == "TranslatedTitle" && !string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString())) // Added to fill ttitle with otitle in case ttitle is empty and mastertitle = ttitle and mastertitle is empty
                        ttitle = Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle];
                    }
                    wtitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                    if (wtitle.Contains(MyFilms.conf.TitleDelim)) wtitle = wtitle.Substring(wtitle.LastIndexOf(MyFilms.conf.TitleDelim) + 1);
                    if (wtitle != ttitle) Remove_Backdrops_Fanart(wtitle, true);
                    if (MyFilms.conf.StrTitle1 == "TranslatedTitle") MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] = moviehead + ttitle;
                    else MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"] = ttitle;
                  }

                  if (IsUpdateRequired("Description", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Description"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Description], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                    MyFilms.r[MyFilms.conf.StrIndex]["Description"] = Result[(int)Grabber_URLClass.Grabber_Output.Description];

                  if (IsUpdateRequired("Rating", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Rating], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  {
                    if (Result[(int)Grabber_URLClass.Grabber_Output.Rating].Length > 0)
                    {
                      NumberFormatInfo provider = new NumberFormatInfo();
                      provider.NumberDecimalSeparator = ".";
                      provider.NumberDecimalDigits = 1;
                      decimal wnote = Convert.ToDecimal(Result[(int)Grabber_URLClass.Grabber_Output.Rating], provider);
                      if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Rating"].ToString()) || !onlymissing) 
                        MyFilms.r[MyFilms.conf.StrIndex]["Rating"] = string.Format("{0:F1}", wnote);
                    }
                  }
                  if (IsUpdateRequired("Actors", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Actors"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Actors], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                    if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Actors"].ToString()) || !onlymissing)
                      MyFilms.r[MyFilms.conf.StrIndex]["Actors"] = Result[(int)Grabber_URLClass.Grabber_Output.Actors];
                  if (IsUpdateRequired("Director", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Director"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Director], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  {
                    director = Result[(int)Grabber_URLClass.Grabber_Output.Director];
                    MyFilms.r[MyFilms.conf.StrIndex]["Director"] = Result[(int)Grabber_URLClass.Grabber_Output.Director];
                  }
                  if (IsUpdateRequired("Producer", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Producer"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Producer], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    MyFilms.r[MyFilms.conf.StrIndex]["Producer"] = Result[(int)Grabber_URLClass.Grabber_Output.Producer];
                  if (IsUpdateRequired("Year", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Year], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  {
                    try { year = Convert.ToInt16(Result[(int)Grabber_URLClass.Grabber_Output.Year]); }
                    catch { year = 1900; }
                    MyFilms.r[MyFilms.conf.StrIndex]["Year"] = year.ToString();
                  }

                  if (IsUpdateRequired("Country", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Country"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Country], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    MyFilms.r[MyFilms.conf.StrIndex]["Country"] = Result[(int)Grabber_URLClass.Grabber_Output.Country];
                  if (IsUpdateRequired("Category", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Category"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Category], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    MyFilms.r[MyFilms.conf.StrIndex]["Category"] = Result[(int)Grabber_URLClass.Grabber_Output.Category];
                  if (IsUpdateRequired("URL", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["URL"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.URL], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    if (MyFilms.conf.StrStorage != "URL") 
                      MyFilms.r[MyFilms.conf.StrIndex]["URL"] = Result[(int)Grabber_URLClass.Grabber_Output.URL];
                  if (IsUpdateRequired("Comments", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Comments"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Comments], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    MyFilms.r[MyFilms.conf.StrIndex]["Comments"] = Result[(int)Grabber_URLClass.Grabber_Output.Comments];
                  if (IsUpdateRequired("Languages", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Languages"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Language], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                    MyFilms.r[MyFilms.conf.StrIndex]["Languages"] =
                      Result[(int)Grabber_URLClass.Grabber_Output.Language];

                  #region AMC4 extended fields
                  if (MyFilms.conf.StrFileType == Configuration.CatalogType.AntMovieCatalog4Xtended)
                  {
                    if (IsUpdateRequired("Writer", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Writer"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Writer], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                      MyFilms.r[MyFilms.conf.StrIndex]["Writer"] = Result[(int)Grabber_URLClass.Grabber_Output.Writer];
                    if (IsUpdateRequired("TagLine", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["TagLine"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Tagline], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                      MyFilms.r[MyFilms.conf.StrIndex]["TagLine"] = Result[(int)Grabber_URLClass.Grabber_Output.Tagline];
                    if (IsUpdateRequired("Certification", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Certification"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Certification], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                      MyFilms.r[MyFilms.conf.StrIndex]["Certification"] = Result[(int)Grabber_URLClass.Grabber_Output.Certification];
                    if (IsUpdateRequired("IMDB_Id", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Id"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.IMDB_Id], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                      MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Id"] = Result[(int)Grabber_URLClass.Grabber_Output.IMDB_Id];
                    if (IsUpdateRequired("IMDB_Rank", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Rank"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.IMDB_Rank], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                      MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Rank"] = Result[(int)Grabber_URLClass.Grabber_Output.IMDB_Rank];
                    if (IsUpdateRequired("TMDB_Id", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["TMDB_Id"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.TMDB_Id], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                      MyFilms.r[MyFilms.conf.StrIndex]["TMDB_Id"] = Result[(int)Grabber_URLClass.Grabber_Output.TMDB_Id];
                    if (IsUpdateRequired("Studio", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Studio"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Studio], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                      MyFilms.r[MyFilms.conf.StrIndex]["Studio"] = Result[(int)Grabber_URLClass.Grabber_Output.Studio];
                    if (IsUpdateRequired("Edition", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Edition"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Edition], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                      MyFilms.r[MyFilms.conf.StrIndex]["Edition"] = Result[(int)Grabber_URLClass.Grabber_Output.Edition];
                    //if (IsUpdateRequired("Fanart", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Fanart"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Fanart], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    //  MyFilms.r[MyFilms.conf.StrIndex]["Fanart"] = Result[(int)Grabber_URLClass.Grabber_Output.Fanart];
                    //if (IsUpdateRequired("Trailer", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Trailer"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Trailer], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    //  MyFilms.r[MyFilms.conf.StrIndex]["Trailer"] = Result[(int)Grabber_URLClass.Grabber_Output.Trailer];
                  }
                  #endregion

                  if (grabtype == GrabType.All && IsUpdateRequired("Picture", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    grabb_Internet_Details_Informations_Cover(Result, interactive, GetID, wscript, grabtype, sTitles);

                  #endregion

                  #endregion
                }

                if (grabtype == GrabType.Person)
                {
                  #region Person

                  string strChoice = "all"; // defaults to "all", if no other choice
                  bool onlyselected = true;
                  bool onlymissing = false;
                  bool onlynonempty = false;
                  List<string> updateItems = new List<string>(); // store properties to update for later use ... not yet used for persons

                  if (interactive) // Dialog only in interactive mode
                    #region interactive selection dialog

                  {
                    List<string> choiceViewMenu = new List<string>();
                    GUIDialogMenu dlgmenu =
                      (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    dlgmenu.Reset();
                    dlgmenu.SetHeading(GUILocalizeStrings.Get(10798732)); // Choose property to update
                    dlgmenu.Add(GUILocalizeStrings.Get(10798734));
                    choiceViewMenu.Add("all");
                    dlgmenu.Add(GUILocalizeStrings.Get(10798735));
                    choiceViewMenu.Add("missing");
                    dlgmenu.Add(GUILocalizeStrings.Get(10798730));
                    choiceViewMenu.Add("all-onlynewdata");

                    #region populate selection menu

                    string[] PropertyList = new string[]
                      {
                        "OriginalTitle", "TranslatedTitle", "Picture", "Description", "Rating", "Actors", "Director",
                        "Producer", "Year", "Country", "Category", "URL", "ImageURL", "Writer", "Comments", "Languages",
                        "TagLine", "Certification", "IMDB_Id", "IMDB_Rank", "Studio", "Edition", "Fanart", "Generic1",
                        "Generic2", "Generic3", "TranslatedTitleAllNames", "TranslatedTitleAllValues",
                        "CertificationAllNames", "CertificationAllValues", "MultiPosters", "Photos", "PersonImages",
                        "MultiFanart", "Trailer", "TMDB_Id", "Empty36", "Empty37", "Empty38", "Empty39"
                      };
                    string strOldValue = "";
                    string strNewValue = "";

                    int i = 0;
                    foreach (string wProperty in PropertyList)
                    {
                      try
                      {
                        strOldValue = MyFilms.r[MyFilms.conf.StrIndex][wProperty].ToString();
                        if (strOldValue == null) strOldValue = "";
                        strNewValue = Result[i].ToString();
                        if (i == 2) strNewValue = Result[12];
                        if (strNewValue == null) strNewValue = "";

                        if ( // make sure, only supported fields are offered to user for update
                          wProperty != "ImageURL" && !wProperty.Contains("Sub") && !wProperty.Contains("All") &&
                          !wProperty.Contains("Generic") && !wProperty.Contains("Empty") && wProperty != "TagLine" &&
                          wProperty != "Certification" && wProperty != "Writer" && wProperty != "Studio" &&
                          wProperty != "Edition" && wProperty != "Fanart" && wProperty != "Aspectratio" &&
                          wProperty != "MultiPosters" // set to enabled to get proper selection - WIP
                          && wProperty != "Photos" && wProperty != "PersonImages" && wProperty != "MultiFanart" &&
                          wProperty != "Trailer")
                        {
                          dlgmenu.Add(BaseMesFilms.Translate_Column(wProperty) + ": '" + strOldValue.Replace(Environment.NewLine, " # ") + "' -> '" + strNewValue.Replace(Environment.NewLine, " # ") + "'");
                          choiceViewMenu.Add(wProperty);
                          LogMyFilms.Debug("GrabberUpdate - Add to menu (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                        }
                        else
                        {
                          LogMyFilms.Debug("GrabberUpdate - Not added to menu (unsupported) - (" + wProperty + "): '" + strOldValue + "' -> '" + strNewValue + "'");
                        }
                      }
                      catch
                      {
                        LogMyFilms.Debug("GrabberUpdate - Error adding Property '" + wProperty + "' to Selectionmenu");
                      }
                      i = i + 1;
                    }

                    #endregion

                    dlgmenu.DoModal(GetID);
                    if (dlgmenu.SelectedLabel == -1) return;
                    strChoice = choiceViewMenu[dlgmenu.SelectedLabel];
                    LogMyFilms.Debug("GrabInternetDetails - interactive choice: '" + strChoice + "'");
                    if (strChoice == "missing")
                    {
                      onlyselected = false;
                      onlymissing = true;
                      onlynonempty = false;
                    }
                    if (strChoice == "all-onlynewdata")
                    {
                      onlyselected = false;
                      onlymissing = false;
                      onlynonempty = true;
                    }
                  }

                  #endregion

                  #region load details data for person

                  IMDBActor person = new IMDBActor();
                  if (IsUpdateRequired("OriginalTitle", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  {
                    title = Result[(int)Grabber_URLClass.Grabber_Output.OriginalTitle];
                    person.Name = title;
                  }

                  if (IsUpdateRequired("TranslatedTitle", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.TranslatedTitle], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  {
                    if (string.IsNullOrEmpty(person.Name)) person.Name = Result[(int)Grabber_URLClass.Grabber_Output.TranslatedTitle];
                  }

                  if (IsUpdateRequired("Description", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Description"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Description], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    person.Biography = Result[(int)Grabber_URLClass.Grabber_Output.Description];

                  if (IsUpdateRequired("Year", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Year], grabtype, onlyselected, onlymissing, onlynonempty, updateItems))
                  {
                    try
                    {
                      year = Convert.ToInt16(Result[(int)Grabber_URLClass.Grabber_Output.Year]);
                    }
                    catch {}
                  }
                  string temp = "";
                  if (IsUpdateRequired("Country", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Country"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Country], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    person.PlaceOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Country];
                  if (IsUpdateRequired("Category", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Category"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Category], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    temp = Result[(int)Grabber_URLClass.Grabber_Output.Category];
                  if (IsUpdateRequired("URL", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["URL"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.URL], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    if (MyFilms.conf.StrStorage != "URL") person.IMDBActorID = Result[(int)Grabber_URLClass.Grabber_Output.URL];
                  if (IsUpdateRequired("Comments", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Comments"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.Comments], grabtype, onlyselected, onlymissing, onlynonempty, updateItems)) 
                    person.DateOfBirth = Result[(int)Grabber_URLClass.Grabber_Output.Comments];
                  if (IsUpdateRequired("Picture", strChoice, MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString(), Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong], grabtype, onlyselected, onlymissing, onlynonempty, updateItems) && grabtype == GrabType.All)
                  {
                    person.ThumbnailUrl = Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong];
                  }
                  if (!string.IsNullOrEmpty(MyFilms.conf.StrPathArtist) && !string.IsNullOrEmpty(person.ThumbnailUrl))
                  {
                    string filenameperson;
                    string filename1person = GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, person.ThumbnailUrl, person.Name, true, true, out filenameperson);
                  }
                  // grabb_Internet_Details_Informations_Cover(Result, interactive, GetID, wscript, grabtype, sTitles);

                  #endregion

                  #endregion
                }

                if (grabtype == GrabType.Cover)
                {
                  #region Cover

                  if (string.IsNullOrEmpty(Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong]))
                  {
                    if (interactive)
                    {
                      GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                      dlgOk.SetHeading(GUILocalizeStrings.Get(10798624)); //MyFilms System Information
                      dlgOk.SetLine(2, GUILocalizeStrings.Get(10798625)); // no results found
                      dlgOk.DoModal(GetID);
                      if (dlgOk.SelectedLabel == -1) // grabb_Internet_Informations(FullMovieName, GetID, true, wscript, FullMoviePath, grabtype, showAll);
                        return;
                    }
                    return;
                  }
                  grabb_Internet_Details_Informations_Cover(Result, interactive, GetID, wscript, grabtype, sTitles);

                  #endregion
                }

                if (grabtype == GrabType.MultiCovers)
                {
                  #region MultiCovers

                  if (string.IsNullOrEmpty(Result[(int)Grabber_URLClass.Grabber_Output.MultiPosters]))
                  {
                    // no data found
                    if (interactive)
                    {
                      GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                      dlgOk.SetHeading(GUILocalizeStrings.Get(10798624)); //MyFilms System Information
                      dlgOk.SetLine(2, GUILocalizeStrings.Get(10798625)); // no results found
                      dlgOk.DoModal(GetID);
                      if (dlgOk.SelectedLabel == -1) // grabb_Internet_Informations(FullMovieName, GetID, true, wscript, FullMoviePath, grabtype, showAll);
                        return;
                    }
                    return;
                  }
                  grabb_Internet_Details_Informations_Cover(Result, interactive, GetID, wscript, grabtype, sTitles);

                  #endregion
                }

                Update_XML_database();
                LogMyFilms.Info("Database Updated for title/ttitle: " + title + "/" + ttitle);

                if (GetID != MyFilms.ID_MyFilmsCoverManager)
                  if (DetailsUpdated != null) DetailsUpdated(true); // will launch afficher_detail(true) via message handler

                if (title.Length > 0 && MyFilms.conf.StrFanart && grabtype != GrabType.Person) // Get Fanart
                {
                  #region fanart

                  // GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS); // already defined above !
                  if (dlgPrgrs != null)
                  {
                    dlgPrgrs.Reset();
                    dlgPrgrs.DisplayProgressBar = false;
                    dlgPrgrs.ShowWaitCursor = true;
                    dlgPrgrs.DisableCancel(true);
                    dlgPrgrs.SetHeading(string.Format("{0} - {1}", "MyFilms", "Artwork Updater"));
                    dlgPrgrs.SetLine(1, "Loading Artwork ...");
                    dlgPrgrs.Percentage = 0;
                    dlgPrgrs.NeedRefresh();
                    dlgPrgrs.ShouldRenderLayer();
                    dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);

                    new System.Threading.Thread(delegate()
                        {
                          string imdbid = GetIMDB_Id(MyFilms.r[MyFilms.conf.StrIndex]);
                          GrabArtwork(title, ttitle, (int)year, director, imdbid, MyFilms.conf.StrTitle1, dlgPrgrs);
                          //  dlgPrgrs.Percentage = 100;
                          //  dlgPrgrs.SetLine(1, "Finished loading Movie Details ...");
                          //  dlgPrgrs.NeedRefresh();
                          //  dlgPrgrs.ShouldRenderLayer();
                          //  Thread.Sleep(500);
                          //  dlgPrgrs.ShowWaitCursor = false;
                          //  dlgPrgrs.Close();

                          //GrabArtwork(title, ttitle, (int)year, director, MyFilms.conf.StrTitle1.ToString(), r => 
                          //{
                          //  dlgPrgrs.Percentage = r;
                          //  return dlgPrgrs.ShouldRenderLayer();
                          //});
                          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
                              { return 0; }, 0, 0, null);
                        }) { Name = "MyFilmsArtworkLoader", IsBackground = true }.Start();
                    return;
                  }
                  // System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, (int)year, director, MyFilms.conf.StrPathFanart, true, false, MyFilms.conf.StrTitle1.ToString());

                  #endregion
                }
                GUIWaitCursor.Hide();
                GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
                    {
                      return 0;
                    }, 0, 0, null);
              }) { Name = "MyFilmsDetailsLoader", IsBackground = true }.Start();
          return;
        }

        private static void grabb_Internet_Details_Informations_Cover(string[] Result, bool interactive, int GetID, string wscript, GrabType grabtype, Searchtitles sTitles)
        {
          string tmpPicture = "";
          string tmpPicturename = ""; // picturename only
          string newPicture = ""; // full path to new picture
          string newPictureCatalogname = ""; // entry to be stored in catalog
          string oldPicture = MyFilmsDetail.getGUIProperty("picture"); // "save" current picture for later restore...
          string oldPictureCatalogname = MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString();

          // set defaults...
          switch (grabtype)
          {
            case GrabType.Cover:
            case GrabType.All:
              tmpPicture = Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong];
              tmpPicturename = Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong].Substring(Result[(int)Grabber_URLClass.Grabber_Output.PicturePathLong].LastIndexOf("\\") + 1);
              break;
            case GrabType.MultiCovers:
              // make difference between existing cover and new one
              //if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString()))
              //{
              //  string tmp = MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString();
              //  if (tmp.Contains("\\"))
              //  {
              //    tmp = tmp.Substring(tmp.LastIndexOf("\\") + 1);
              //    if (tmp.Contains("["))
              //      tmpPicturename = tmp.Substring(0, tmp.LastIndexOf("[") - 1);
              //    else tmpPicturename = tmp;
              //  }
              //  else tmpPicturename = tmp;
              //}

              //if (title.EndsWith(".jpg"))
              //  title = title.Substring(0, title.Length - 4);
              //string safeName = CreateFilename(title);
              //string dirname = artFolder;
              //// string directory = "";
              ////if (dirname.Length > dirname.LastIndexOf("\\")) 
              ////  directory = dirname.Substring(dirname.LastIndexOf("\\"));
              ////if (!System.IO.Directory.Exists(directory))
              ////  System.IO.Directory.CreateDirectory(directory);
              //if (first && !System.IO.File.Exists(dirname + safeName + ".jpg"))
              //  filename = dirname + safeName + ".jpg";
              //else
              //  filename = dirname + safeName + " [" + imageUrl.GetHashCode() + "].jpg";
              //FileInfo newFile = new FileInfo(filename);
              //bool alreadyInFolder = newFile.Exists;

              if (string.IsNullOrEmpty(tmpPicturename))
              {
                tmpPicturename = sTitles.FanartTitle + ".jpg";
              }
              break;
          }

          if (MyFilms.conf.StrPicturePrefix.Length > 0) 
            newPicture = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + tmpPicturename;
          else 
            newPicture = Path.Combine(MyFilms.conf.StrPathImg, tmpPicturename);

          newPictureCatalogname = GetPictureCatalogNameFromFilename(newPicture);

          LogMyFilms.Debug("Cover Image path : '" + MyFilms.conf.StrPathImg + "'");
          LogMyFilms.Debug("Picturehandling  : '" + MyFilms.conf.PictureHandling + "'");
          LogMyFilms.Debug("PicturePrefix    : '" + MyFilms.conf.StrPicturePrefix + "'");
          LogMyFilms.Debug("Temp Cover Image : '" + tmpPicture + "'");
          LogMyFilms.Debug("New  Cover Image : '" + newPicture + "'");
          LogMyFilms.Debug("New Catalog Entry: '" + newPictureCatalogname + "'");


          switch (grabtype)
          {
            case GrabType.MultiCovers:
              #region multiple cover images
              Grabber.GrabberScript script = new GrabberScript(wscript);
              ArtworkInfo ArtworkImages = new ArtworkInfo(Result[(int)Grabber_URLClass.Grabber_Output.MultiPosters], script.URLPrefix);
              List<ArtworkInfoItem> testlist = new List<ArtworkInfoItem>();
              testlist = Grabber.GrabUtil.GetMultiImageList(script.URLPrefix, Result[(int)Grabber_URLClass.Grabber_Output.MultiPosters], "");

              List<string> choiceViewMenu = new List<string>();
              GUIDialogMenu dlgmenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
              dlgmenu.Reset();
              dlgmenu.SetHeading(GUILocalizeStrings.Get(10798764)); // Load Covers ...
              dlgmenu.Add(GUILocalizeStrings.Get(10798622) + " (" + testlist.Count.ToString() + ")"); //all
              choiceViewMenu.Add("all");

              int i = 0;
              foreach (Grabber.ArtworkInfoItem artworkImage in testlist)
              {
                try
                {
                  dlgmenu.Add(artworkImage.Name);
                  choiceViewMenu.Add(artworkImage.URL);
                  LogMyFilms.Debug("Coverdownload - Add to menu (" + i + ": " + artworkImage.Name + "): '" + artworkImage.URL + "'");
                }
                catch { }
                i = i + 1;
              }

              dlgmenu.DoModal(GetID);
              if (dlgmenu.SelectedLabel == -1) return;
              string strChoice = choiceViewMenu[dlgmenu.SelectedLabel];
              LogMyFilms.Debug("GrabInternetDetails - interactive choice: '" + strChoice + "'");

              //GrabberScript script = new GrabberScript(wscript);
              GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
              if (dlgPrgrs != null)
              {
                  dlgPrgrs.Reset();
                  dlgPrgrs.DisplayProgressBar = true;
                  dlgPrgrs.ShowWaitCursor = false;
                  dlgPrgrs.DisableCancel(true);
                  dlgPrgrs.SetHeading(string.Format("{0} - {1}", "MyFilms", "Internet Details Grabber"));
                  dlgPrgrs.SetLine(1, "Loading Cover(s) [" + script.DBName + "] ...");
                  dlgPrgrs.Percentage = 0;
                  dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);

                  new System.Threading.Thread(delegate()
                  {
                      try
                      {
                          string filename = string.Empty;
                          string filename1 = string.Empty;
                          string filename2 = string.Empty;
                          //if (MasterTitle == "OriginalTitle")
                          //  wtitle2 = wtitle1;
                          bool first = true;
                          int a = 0;

                          if (strChoice != "all")
                          {
                              filename1 = Grabber.GrabUtil.DownloadCovers(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix, strChoice, tmpPicturename, true, first, out filename);
                          }
                          else
                          {
                              foreach (Grabber.ArtworkInfoItem artworkImage in testlist)
                              {
                                  filename1 = GrabUtil.DownloadCovers(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix, artworkImage.URL, tmpPicturename, true, first, out filename);
                                  if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "loading '" + System.IO.Path.GetFileName(filename) + "'");
                                  if (dlgPrgrs != null) dlgPrgrs.Percentage = a * 100 / testlist.Count;
                                  LogMyFilms.Info("Poster " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + newPictureCatalogname);
                                  artworkImage.LocalPath = filename;
                                  if (first)
                                      newPicture = filename1;
                                  if (filename == string.Empty)
                                      filename = filename1;
                                  if (!(filename == "already" && filename1 == "already"))
                                      filename = "added";
                                  first = false;
                                  a++;
                              }
                          }
                      }
                      catch (Exception ex) { LogMyFilms.DebugException("Thread 'MyFilmsCoverLoader' - exception! - ", ex); }
                      if (dlgPrgrs != null)
                          dlgPrgrs.Percentage = 100; dlgPrgrs.ShowWaitCursor = false; dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846)); dlgPrgrs.SetLine(2, ""); Thread.Sleep(50); dlgPrgrs.Close(); // Done...
                      GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
                      {
                          dlgPrgrs.ShowWaitCursor = false;
                          // enter here what to load after background thread has finished !
                          // if (DetailsUpdated != null) DetailsUpdated(true); // already sent in details update
                          return 0;
                      }, 0, 0, null);
                  }) { Name = "MyFilmsCoverLoader", IsBackground = true }.Start();
                  return;
              }
              #endregion
              break;
            case GrabType.Cover:
              #region single cover without confirmation dialog
              if (System.IO.File.Exists(newPicture)) // if 'base file' already exists, create sub file name Result[(int)Grabber_URLClass.Grabber_Output.Country]
              {
                newPicture = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(newPicture), System.IO.Path.GetFileNameWithoutExtension(newPicture) + " [" + Result[(int)Grabber.Grabber_URLClass.Grabber_Output.PictureURL].GetHashCode() + "]" + System.IO.Path.GetExtension(newPicture));
                LogMyFilms.Debug("New Cover File Name = '" + newPicture + "'");
              }
              if (newPicture != tmpPicture)
              {
                if (!System.IO.Directory.Exists(newPicture.Substring(0, newPicture.LastIndexOf("\\"))))
                {
                  try { System.IO.Directory.CreateDirectory(newPicture.Substring(0, newPicture.LastIndexOf("\\"))); }
                  catch (Exception ex) { LogMyFilms.Debug("Could not create directory '" + newPicture.Substring(0, newPicture.LastIndexOf("\\")) + "' - Exception: " + ex); }
                }
                try
                {
                  LogMyFilms.Debug("Copy '" + tmpPicture + "' to '" + newPicture + "'");
                  File.Copy(tmpPicture, newPicture, true);
                }
                catch (Exception ex) { LogMyFilms.Debug("Error copy file: '" + tmpPicture + "' - Exception: " + ex); }
              }

              if (newPicture != tmpPicture)
              {
                try { File.Delete(tmpPicture); }
                catch (Exception ex) { LogMyFilms.Debug("Error deleting tmp file: '" + tmpPicture + "' - Exception: " + ex); }
              }
              #endregion
              break;
            case GrabType.All:
              #region single cover
              if (interactive)
              {
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
                else
                {
                  setGUIProperty("picture", "");
                  GUIWindowManager.Process();
                }
              }
              if (newPicture != tmpPicture)
              {
                if (!System.IO.Directory.Exists(newPicture.Substring(0, newPicture.LastIndexOf("\\"))))
                {
                  try { System.IO.Directory.CreateDirectory(newPicture.Substring(0, newPicture.LastIndexOf("\\"))); }
                  catch (Exception ex) { LogMyFilms.Debug("Could not create directory '" + newPicture.Substring(0, newPicture.LastIndexOf("\\")) + "' - Exception: " + ex); }
                }
                try { File.Copy(tmpPicture, newPicture, true); }
                catch (Exception ex) { LogMyFilms.Debug("Error copy file: '" + tmpPicture + "' - Exception: " + ex); }
              }

              if (newPicture != tmpPicture)
              {
                try { File.Delete(tmpPicture); }
                catch (Exception ex) { LogMyFilms.Debug("Error deleting tmp file: '" + tmpPicture + "' - Exception: " + ex); }
              }
              #endregion
              break;
            default:
              break;
          }
          // updtate catalog entry in memory
          if (string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString()))
            MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
          else if (interactive)
            MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
          else if (!oldPicture.Contains(oldPictureCatalogname))
            MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
          // set picture to new one (full path)
          setGUIProperty("picture", newPicture);
          GUIWindowManager.Process();
          if (GetID == MyFilms.ID_MyFilmsCoverManager)
          {
            if (DetailsUpdated != null) DetailsUpdated(true); // will launch afficher_detail(true) via message handler
          }
        }

        private static bool IsUpdateRequired(string currentField, string selectedField, string oldvalue, string newvalue, GrabType grabtype, bool onlyselected, bool onlymissing, bool onlynonempty, List<string> updateitems)
        {
          bool updaterequired = false;
          if (currentField == selectedField || !onlyselected || updateitems.Contains(currentField))
          {
            if (onlymissing && string.IsNullOrEmpty(oldvalue) || !onlymissing)
            {
              if (onlynonempty && !string.IsNullOrEmpty(newvalue) || !onlynonempty)
              {
                if (!string.IsNullOrEmpty(newvalue)) updaterequired = true;
              }
            }
          }
          LogMyFilms.Debug("IsUpdateRequired(): return '" + updaterequired + "' for: currentField = '" + currentField + "', selectedField = '" + selectedField + "', onlyselected = '" + onlyselected + "', onlymissing = '" + onlymissing + "', onlynonempty = '" + onlynonempty + "'");
          return updaterequired;
        }

        private static void GrabArtwork(string title, string ttitle, int year, string director, string imdbid, string StrTitle1, GUIDialogProgress dlgPrgrs)
        {
          try
          {
            if (dlgPrgrs != null) 
              dlgPrgrs.SetLine(1, "Now loading Artwork ...");

            if (dlgPrgrs != null && !dlgPrgrs.ShouldRenderLayer()) 
              return;

            if (dlgPrgrs != null) 
              dlgPrgrs.Percentage = 10;

            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, year, director, imdbid, MyFilms.conf.StrPathFanart, true, false, StrTitle1, string.Empty);
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("GrabArtwork() - Error getting fanart: " + ex.Message);
          }
          finally
          {
            if (dlgPrgrs != null)
            {
              dlgPrgrs.Percentage = 100; 
              dlgPrgrs.SetLine(1, "Finished loading Artwork !");
              dlgPrgrs.ShowWaitCursor = false;
              Thread.Sleep(500);
              dlgPrgrs.Close();
            }
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
              LogMyFilms.Debug("(CreateThumbFromMovie): Error - Moviefilesource is empty !");
              return;
            }
            if (fileName.Contains("VIDEO_TS\\VIDEO_TS.IFO")) // Do not try to create thumbnails for DVDs
            {
              LogMyFilms.Debug("(CreateThumbFromMovie): Moviesource is DVD - return without creating coverfile...");
              return;
            }
            if (fileName.Contains(";"))
              fileName = fileName.Substring(0, fileName.IndexOf(";")).Trim();
            if (!System.IO.File.Exists(fileName))
            {
              LogMyFilms.Debug("(CreateThumbFromMovie): Error - Moviefilesource: '" + fileName + "' does not exist !");
              return;
            }
            string tempImage = System.IO.Path.GetTempPath() + "MovieThumb_" + MyFilms.r[MyFilms.conf.StrIndex]["Number"] + ".jpg";
            if (System.IO.File.Exists(tempImage)) System.IO.File.Delete(tempImage);
            //if (System.IO.File.Exists(tempImage + "L")) System.IO.File.Delete(tempImage + "L");
            string strThumb = MyFilms.conf.StrPathImg + "\\MovieThumb_" + MyFilms.r[MyFilms.conf.StrIndex]["Number"] + ".jpg";
            LogMyFilms.Debug("(CreateThumbFromMovie): Moviefilesource: '" + fileName + "', Covernamedestination: '" + strThumb + "', TempImage: '" + tempImage + "'");

            //if (Img_Path_Type.ToLower() == "true")
            //    MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = strThumb;

            //if (File.Exists(strThumb))
            //{
            //  LogMyFilms.Debug("(CreateThumbFromMovie): Coverimagefile already exists - return without creating coverfile...");
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
              LogMyFilms.Debug("Failed getting aspectratio of movie");
            else
            {
              LogMyFilms.Debug("GetAspectratio: ar = '" + ar + "'");
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
            LogMyFilms.Debug("GetAspectratio: ar = '" + ar + "', columns = " + columns.ToString() + ", Rows = " + rows + ".");

            //System.Drawing.Image thumb = null;))))
            try
            {
                // CreateVideoThumb(string aVideoPath, string aThumbPath, bool aCacheThumb, bool aOmitCredits);
              bool success = Grabber.ThumbCreator.CreateVideoThumb(fileName, tempImage, true, false, columns, rows, false, "Cover");
                if (!success)
                {
                  LogMyFilms.Debug("(CreateThumbFromMovie): 'CreateVideoThumb' was NOT successful!");
                  return;
                }
                else
                {
                  LogMyFilms.Debug("(CreateThumbFromMovie): 'CreateVideoThumb' was successful!");
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
            LogMyFilms.Debug("Clean title '" + MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() + "' to '" + cleanedtitle + "'");

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
            LogMyFilms.Info("(Update_XML_database()) - Database Updated for created PictureThumb: " + strThumb);
        }
          
        private static string GetAspectRatio(string FileName)
        {
          try
          {
            MediaInfoWrapper minfo = new MediaInfoWrapper(FileName);

            LogMyFilms.Debug("Mediainfo width: " + minfo.Width.ToString());
            LogMyFilms.Debug("Mediainfo height: " + minfo.Height.ToString());
            //ToDo: Calculate aspect ratio here...

            return minfo.AspectRatio;
          }
          catch (Exception ex)
          {
            LogMyFilms.Error("GetAspectRatio: Error getting aspectratio via mediainfo.dll - Exception: " + ex.ToString());
          }
          return "";
        }

        //-------------------------------------------------------------------------------------------
        //  Returns existing or newly created filename for coverart
        //-------------------------------------------------------------------------------------------        
        public static string GetOrCreateCoverFilename(DataRow[] r1, int index, string mastertitle)
        {
          string file = string.Empty;
          if (r1[index]["Picture"].ToString().Length > 0)
          {
            if ((r1[index]["Picture"].ToString().IndexOf(":\\") == -1) && (r1[index]["Picture"].ToString().Substring(0, 2) != "\\\\"))
              file = MyFilms.conf.StrPathImg + "\\" + r1[index]["Picture"];
            else
              file = r1[index]["Picture"].ToString();
          }
          //if (!System.IO.File.Exists(file) || string.IsNullOrEmpty(file))
          //  file = MyFilms.conf.DefaultCover;
          if (string.IsNullOrEmpty(file))
          {
            // create new name, as none is existing yet ...
            string title = (mastertitle.EndsWith(".jpg")) ? mastertitle.Substring(0, mastertitle.Length - 4) : mastertitle;
            title = GrabUtil.CreateFilename(title) + ".jpg";
            //if (first && !System.IO.File.Exists(dirname + safeName + ".jpg"))
            //  filename = dirname + safeName + ".jpg";
            //else
            //  filename = dirname + safeName + " [" + imageUrl.GetHashCode() + "].jpg";
            
            
            if (MyFilms.conf.StrPicturePrefix.Length > 0)
              file = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + title;
            else
              file = MyFilms.conf.StrPathImg + "\\" + title;

            if (!System.IO.Directory.Exists(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix))
              try
              {
                System.IO.Directory.CreateDirectory(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix);
              }
              catch (Exception ex)
              {
                LogMyFilms.Debug("GetOrCreateCoverFilename() - Error with Exception  = '" + ex.Message + "'");
              }
          }
          LogMyFilms.Debug("GetOrCreateCoverFilename() - PictureFilename = '" + file + "'");
          return file;
        }

        //-------------------------------------------------------------------------------------------
        //  Returns filename to be stored in catalog, depending on settings
        //-------------------------------------------------------------------------------------------        
        public static string GetPictureCatalogNameFromFilename(string picturefilename)
        {
          string newPictureCatalogname = ""; // entry to be stored in catalog
          string tmpPicturename = picturefilename.Substring(picturefilename.LastIndexOf("\\") + 1);
          if (MyFilms.conf.PictureHandling == "Relative Path" || string.IsNullOrEmpty(MyFilms.conf.PictureHandling))
          {
            newPictureCatalogname = MyFilms.conf.StrPicturePrefix.Substring(0, MyFilms.conf.StrPicturePrefix.LastIndexOf("\\") + 1) + tmpPicturename;
          }
          if (MyFilms.conf.PictureHandling == "Full Path")
          {
            newPictureCatalogname = picturefilename;
          }
          LogMyFilms.Debug("GetPictureCatalogNameFromFilename() - PictureCatalogName = '" + newPictureCatalogname + "' for Picture '" + picturefilename + "'");
          return newPictureCatalogname;
        }

        //-------------------------------------------------------------------------------------------
        //  Dowload TMDBinfos (Poster(s), Movieinfos) on theMovieDB.org
        //-------------------------------------------------------------------------------------------        
        public static void Download_TMDB_Posters(string wtitle, string wttitle, string director, string year, bool choose, int wGetID, string savetitle)
        {
            string oldPicture = MyFilmsDetail.getGUIProperty("picture");
            string newPicture = ""; // full path to new picture
            string newPictureCatalogname = ""; // entry to be stored in catalog
            if (string.IsNullOrEmpty(oldPicture))
                oldPicture = "";

            if (!System.IO.Directory.Exists(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix))
                try
                {
                    Directory.CreateDirectory(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix);
                }
                catch (Exception) { }
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            string language = CultureInfo.CurrentCulture.Name.Substring(0, 2);
            int wyear = 0;
            try { wyear = Convert.ToInt32(year); }
            catch { }
            new Thread(delegate()
              {
                  try
                  {
                      GUIWaitCursor.Init(); GUIWaitCursor.Show();
                      List<grabber.DBMovieInfo> listemovies = Grab.GetTMDBinfos(wtitle, wttitle, wyear, director, MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix, true, choose, MyFilms.conf.StrTitle1, language);
                      GUIWaitCursor.Hide();
                      LogMyFilms.Debug("(TMDB-Infos) - listemovies: '" + wtitle + "', '" + wttitle + "', '" + wyear + "', '" + director + "', '" + MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix + "', 'true', '" + choose.ToString() + "', '" + MyFilms.conf.StrTitle1 + "', '" + language + "'");
                      int listCount = listemovies.Count;
                      LogMyFilms.Debug("(TMDB-Infos) - listemovies: Result Listcount: '" + listCount + "'");

                      if (choose)
                          listCount = 2;
                      switch (listCount)
                      {
                          case 0:
                              break;
                          case 1:
                              LogMyFilms.Info("Posters " + listemovies[0].Name.Substring(listemovies[0].Name.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);
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
                                  LogMyFilms.Debug("TMDB listemovies: " + t.Name + "  (" + t.Year + ") - Posters: " + t.Posters.Count + " - TMDB-Id: " + t.Identifier);
                              }
                              if (!(dlg.SelectedLabel > -1))
                              {
                                  dlg.SelectedLabel = -1;
                                  dlg.DoModal(wGetID);
                              }
                              if (dlg.SelectedLabel == 0)
                              {
                                #region Get SubTitles and Subwords from otitle and ttitle
                                wotitle_tableau = MyFilms.SubTitleGrabbing(wtitle);
                                  wttitle_tableau = MyFilms.SubTitleGrabbing(wttitle);
                                  wotitle_sub_tableau = MyFilms.SubWordGrabbing(wtitle, MinChars, Filter); // Min 3 Chars, Filter true (no der die das)
                                  wttitle_sub_tableau = MyFilms.SubWordGrabbing(wttitle, MinChars, Filter); // Min 3 Chars, Filter true (no der die das)
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
                                #endregion
                              }
                              if (dlg.SelectedLabel > 0)
                              {
                                  // Load Posters  -> show progress dialog !

                                  GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
                                  if (dlgPrgrs != null)
                                  {
                                      dlgPrgrs.Reset();
                                      dlgPrgrs.DisplayProgressBar = true;
                                      dlgPrgrs.ShowWaitCursor = false;
                                      dlgPrgrs.DisableCancel(true);
                                      dlgPrgrs.SetHeading("MyFilms Artwork Download");
                                      dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
                                      dlgPrgrs.SetLine(1, "Loading Artwork ...");
                                      dlgPrgrs.Percentage = 0;

                                      bool first = true;
                                      string filename = string.Empty;
                                      string filename1 = string.Empty;
                                      if (MyFilms.conf.StrTitle1 == "OriginalTitle")
                                          wttitle = savetitle; // Was wttitle = wtitle;
                                      int i = 0;
                                      if (dlgPrgrs != null) dlgPrgrs.SetLine(1, "Loading Poster for '" + savetitle + "'");
                                      foreach (string poster in listemovies[dlg.SelectedLabel - 1].Posters)
                                      {
                                          filename1 = Grabber.GrabUtil.DownloadCovers(MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix, poster, wttitle, true, first, out filename);
                                          if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "loading '" + System.IO.Path.GetFileName(filename) + "'");
                                          if (dlgPrgrs != null) dlgPrgrs.Percentage = i * 100 / listemovies[dlg.SelectedLabel - 1].Posters.Count;
                                          LogMyFilms.Info("Poster " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);
                                          if (first)
                                              newPicture = filename1;
                                          if (filename == string.Empty)
                                              filename = filename1;
                                          if (!(filename == "already" && filename1 == "already"))
                                              filename = "added";
                                          first = false;
                                          i++;
                                      }
                                      foreach (string person in listemovies[dlg.SelectedLabel - 1].Actors)
                                      {
                                          // ToDo: Load Actorinfodetails (API Call to be written in grabber) and download Person Artwork
                                      }
                                      listemovies[0].Name = filename;

                                      newPictureCatalogname = GetPictureCatalogNameFromFilename(newPicture);

                                      if (wGetID != MyFilms.ID_MyFilmsCoverManager)
                                      {
                                        if (!choose)
                                        {
                                          MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
                                          Update_XML_database();
                                          LogMyFilms.Info("(Update_XML_database()) - Database Updated for created Coverthumb: " + newPicture);
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
                                          LogMyFilms.Info("(Update_XML_database()) - Database Updated with '" + newPictureCatalogname + "' for created Coverthumb '" + newPicture + "'");
                                        }
                                      }
                                  }
                                  if (dlgPrgrs != null) dlgPrgrs.Percentage = 100; dlgPrgrs.ShowWaitCursor = false; dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846)); dlgPrgrs.SetLine(2, ""); Thread.Sleep(50); dlgPrgrs.Close(); // Done...
                                // return;
                              }
                              break;
                      }
                  }
                  catch (Exception ex)
                  {
                      LogMyFilms.DebugException("Thread 'MyFilmsTMDBLoader' - exception! - ", ex);
                  }
                  GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
                    {
                      // enter here what to load after background thread has finished !
                      if (DetailsUpdated != null) DetailsUpdated(true);
                      return 0;
                    }, 0, 0, null);
             }) { Name = "MyFilmsTMDBLoader", IsBackground = true }.Start();
        }

        //-------------------------------------------------------------------------------------------
        //  Change local Cover Image
        //-------------------------------------------------------------------------------------------        
        public static string ChangeLocalCover(DataRow[] r1, int Index, bool interactive, bool onlyReturnCount)
        {
          result = new ArrayList();
          ArrayList resultsize = new ArrayList();
          string[] filesfound = new string[100];
          Int64[] filesfoundsize = new Int64[100];
          int filesfoundcounter = 0;
          //string file = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
          //string titlename = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
          //string titlename2 = MyFilms.r[Index][MyFilms.conf.StrTitle2].ToString();
          //string file = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
          //string titlename = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString();
          //string titlename2 = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString();
          string directoryname = "";
          //string movieName = "";
          string[] files = null;
          Int64 wsize = 0; // Temporary Filesize detection
          string startPattern = "";
          string currentPicture = MyFilmsDetail.getGUIProperty("picture");
          //if ((sr["Picture"].ToString().IndexOf(":\\") == -1) && (sr["Picture"].ToString().Substring(0, 2) != "\\\\"))
          //  conf.FileImage = conf.StrPathImg + "\\" + sr["Picture"].ToString();
          //else
          //  conf.FileImage = sr["Picture"].ToString();

          if (string.IsNullOrEmpty(currentPicture)) 
            return "";
          string currentPictureName = currentPicture.Substring(currentPicture.LastIndexOf("\\") + 1);
          string currentStorePath = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix;
          string newPicture = ""; // full path to new picture
          string newPictureCatalogname = ""; // entry to be stored in catalog


          if (!currentPicture.StartsWith(currentStorePath)) 
            return "";
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

          LogMyFilms.Debug("(ChangeLocalCover) - startPattern = '" + startPattern + "', searchPattern = '" + searchPattern + "', currentStorePath = '" + currentStorePath + "'");
          
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

          if (onlyReturnCount)
            return filesfoundcounter.ToString();

          foreach (string n in filesfound)
          {
            if (!string.IsNullOrEmpty(n))
              LogMyFilms.Debug("(Sorted coverfiles) ******* : '" + n + "'");
          }

          Array.Sort(filesfoundsize);
          for (int i = 0; i < result.Count; i++)
          {
            if (!string.IsNullOrEmpty(filesfound[i]))
              LogMyFilms.Debug("(Sorted coverfiles) ******* : Number: '" + i + "' - Size: '" + filesfoundsize[i] + "' - Name: '" + filesfound[i] + "'");
          }

          if (filesfoundcounter == 1) return "(1)";
          if (result.Count > 0)
          {
            if (nCurrent == -1)
              return "(" + filesfoundcounter.ToString() + ")";
            nCurrent++;
            if (nCurrent >= result.Count) 
              nCurrent = 0;
            newPicture = filesfound[nCurrent];

            newPictureCatalogname = GetPictureCatalogNameFromFilename(newPicture);
            if (!interactive)
            {
              // add confirmation dialog here....
            }
            else
            {
              MyFilms.r[MyFilms.conf.StrIndex]["Picture"] = newPictureCatalogname;
              LogMyFilms.Debug("(ChangeLocalCover) : NewCatalogName: '" + newPictureCatalogname + "'");
              Update_XML_database();
            }
            return "(" + filesfoundcounter + ")";
          }
          else
            LogMyFilms.Debug("MyFilmsDetails (LocalCoverChange) - NO COVERS FOUND !!!!");
          return "";
        }

        public static Searchtitles GetSearchTitles(DataRow movieRecord, string mediapath)
        {
          return GetSearchTitles(movieRecord, mediapath, MyFilms.conf);
        }
        public static Searchtitles GetSearchTitles(DataRow movieRecord, string mediapath, Configuration tmpconf) // returns the first title name of the configured mastertitle field
        {
          Searchtitles stitles = new Searchtitles();
          stitles.SearchTitle = "";
          stitles.FanartTitle = "";
          stitles.MasterTitle = "";
          stitles.SecondaryTitle = "";
          stitles.SortTitle = "";
          stitles.MovieDirectoryTitle = "";
          stitles.MovieFileTitle = "";
          stitles.OriginalTitle = "";
          stitles.TranslatedTitle = "";
          stitles.FormattedTitle = "";
          stitles.year = 0;
          stitles.Director = "";

          if (movieRecord["OriginalTitle"] != null && movieRecord["OriginalTitle"].ToString().Length > 0)
            stitles.OriginalTitle = RemoveGroupNames(movieRecord["OriginalTitle"].ToString());
          if (tmpconf.StrTitle1 == "OriginalTitle")
            stitles.MasterTitle = stitles.OriginalTitle;
          if (tmpconf.StrTitle2 == "OriginalTitle")
            stitles.SecondaryTitle = stitles.OriginalTitle;
          if (tmpconf.StrSTitle == "OriginalTitle")
            stitles.SortTitle = stitles.OriginalTitle;

          if (movieRecord["TranslatedTitle"] != null && movieRecord["TranslatedTitle"].ToString().Length > 0)
            stitles.TranslatedTitle = RemoveGroupNames(movieRecord["TranslatedTitle"].ToString());
          if (tmpconf.StrTitle1 == "TranslatedTitle")
            stitles.MasterTitle = stitles.TranslatedTitle;
          if (tmpconf.StrTitle2 == "TranslatedTitle")
            stitles.SecondaryTitle = stitles.TranslatedTitle;
          if (tmpconf.StrSTitle == "TranslatedTitle")
            stitles.SortTitle = stitles.TranslatedTitle;

          if (movieRecord["FormattedTitle"] != null && movieRecord["FormattedTitle"].ToString().Length > 0)
            stitles.FormattedTitle = RemoveGroupNames(movieRecord["FormattedTitle"].ToString());
          if (tmpconf.StrTitle1 == "FormattedTitle")
            stitles.MasterTitle = stitles.FormattedTitle;
          if (tmpconf.StrTitle2 == "FormattedTitle")
            stitles.SecondaryTitle = stitles.FormattedTitle;
          if (tmpconf.StrSTitle == "FormattedTitle")
            stitles.SortTitle = stitles.FormattedTitle;

          if (movieRecord["OriginalTitle"].ToString().Length > 0 && tmpconf.StrFanart)
          {
            try
            {
              stitles.year = System.Convert.ToInt16(movieRecord["Year"]);
            }
            catch
            {
              stitles.year = 0;
            }
            try
            {
              stitles.Director = (string)movieRecord["Director"];
            }
            catch
            {
              stitles.Director = string.Empty;
            }
          }
          if (tmpconf.StrTitle1 == "FormattedTitle") // special setting for formatted title - we don't want to use it, as it is usually too long and causes problems with path length
          {
            if (!string.IsNullOrEmpty(stitles.TranslatedTitle))
              stitles.FanartTitle = stitles.TranslatedTitle;
            else if (!string.IsNullOrEmpty(stitles.OriginalTitle))
              stitles.FanartTitle = stitles.OriginalTitle;
            else
              stitles.FanartTitle = stitles.FormattedTitle;
          } 
          else
          {
            if (!string.IsNullOrEmpty(stitles.MasterTitle))
              stitles.FanartTitle = stitles.MasterTitle;
            else if (!string.IsNullOrEmpty(stitles.OriginalTitle))
              stitles.FanartTitle = stitles.OriginalTitle;
            else if (!string.IsNullOrEmpty(stitles.TranslatedTitle))
              stitles.FanartTitle = stitles.TranslatedTitle;
            else if (!string.IsNullOrEmpty(stitles.FormattedTitle))
              stitles.FanartTitle = stitles.FormattedTitle;
            else stitles.FanartTitle = "";
          }

          // if mediapath is given, create mediapath search titles
          if (!string.IsNullOrEmpty(mediapath) && mediapath.Contains(".") && mediapath.Contains(@"\"))
          {
            string CleanStringFile = "";
            string CleanStringDir = "";
            Regex CutText;
            Match m;

            // filename
            //Strip Path
            CleanStringFile = mediapath.Substring(mediapath.LastIndexOf(@"\") + 1);
            //Strip Extension
            CleanStringFile = CleanStringFile.Substring(0, CleanStringFile.LastIndexOf("."));
            CutText = new Regex(@"\(" + "[-|_|.| +][cC][dD][0-9]|[-|_|.| +][dD][iI][sS][kKcC][0-9]|[0-9]of[0-9]" + @"\)");
            m = CutText.Match(CleanStringFile);
            if (m.Success) //Finally remove anything which may be a multi-part indicator (e.g. 1of2)
              CleanStringFile = CutText.Replace(CleanStringFile, "");
            CutText = new Regex("[-|_|.| +][cC][dD][0-9]|[-|_|.| +][dD][iI][sS][kKcC][0-9]|[0-9]of[0-9]");
            m = CutText.Match(CleanStringFile);
            if (m.Success) CleanStringFile = CutText.Replace(CleanStringFile, "");
            stitles.MovieFileTitle = Utility.RemoveNastyCharacters(CleanStringFile.Trim());
            
            // foldername
            //Strip filename:
            CleanStringDir = mediapath.Substring(0, mediapath.LastIndexOf(@"\"));
            //Strip Path:
            CleanStringDir = CleanStringDir.Substring(CleanStringDir.LastIndexOf(@"\") + 1);
            CutText = new Regex(@"\(" + "[-|_|.| +][cC][dD][0-9]|[-|_|.| +][dD][iI][sS][kKcC][0-9]|[0-9]of[0-9]" + @"\)");
            m = CutText.Match(CleanStringDir);
            if (m.Success) //Finally remove anything which may be a multi-part indicator (e.g. 1of2)
              CleanStringDir = CutText.Replace(CleanStringDir, "");
            CutText = new Regex("[-|_|.| +][cC][dD][0-9]|[-|_|.| +][dD][iI][sS][kKcC][0-9]|[0-9]of[0-9]");
            m = CutText.Match(CleanStringDir);
            if (m.Success) CleanStringDir = CutText.Replace(CleanStringDir, "");
            stitles.MovieDirectoryTitle = Utility.RemoveNastyCharacters(CleanStringDir.Trim());

            // DVD_Folders
            if (mediapath.ToLower().Contains("video_ts"))
            {
              string CleanString = Utility.GetDVDFolderName(mediapath);
              stitles.MovieDirectoryTitle = Utility.RemoveNastyCharacters(CleanString.Trim());
            }
            if (mediapath.ToLower().Contains("index.bdmv"))
            {
              string CleanString = Utility.GetBRFolderName(mediapath);
              stitles.MovieDirectoryTitle = Utility.RemoveNastyCharacters(CleanString.Trim());
            }
          }

          if (MyFilms.conf == tmpconf) // only log if internal operation
            LogMyFilms.Debug("GetSearchTitles: returning Titles: '" + stitles.FanartTitle + "' - mastertitle (" + tmpconf.StrTitle1 + ") =  '" + stitles.MasterTitle + "' - originaltitle: '" + stitles.OriginalTitle + "' - translatedtitle: '" + stitles.TranslatedTitle + "' - formattedtitle: '" + stitles.FormattedTitle + "' - director: '" + stitles.Director + "' - year: '" + stitles.year.ToString() + "'");
          return stitles;
        }

        // returns the first title name of the configured mastertitle field
        public static string GetFanartTitle(DataRow movieRecord, out string wtitle, out string wttitle, out string wftitle, out int wyear, out string wdirector)
        {
          string fanartTitle = "";
          string mastertitle = string.Empty;
          wtitle = wttitle = wftitle = wdirector = string.Empty;
          wyear = 0;

          if (movieRecord["OriginalTitle"] != null && movieRecord["OriginalTitle"].ToString().Length > 0)
            wtitle = RemoveGroupNames(movieRecord["OriginalTitle"].ToString());
          if (MyFilms.conf.StrTitle1 == "OriginalTitle") 
            mastertitle = wtitle;

          if (movieRecord["TranslatedTitle"] != null && movieRecord["TranslatedTitle"].ToString().Length > 0)
            wttitle = RemoveGroupNames(movieRecord["TranslatedTitle"].ToString());
          if (MyFilms.conf.StrTitle1 == "TranslatedTitle")
            mastertitle = wttitle;

          if (movieRecord["FormattedTitle"] != null && movieRecord["FormattedTitle"].ToString().Length > 0)
            wftitle = RemoveGroupNames(movieRecord["FormattedTitle"].ToString());
          if (MyFilms.conf.StrTitle1 == "FormattedTitle")
            mastertitle = wftitle;

          if (movieRecord["OriginalTitle"].ToString().Length > 0 && MyFilms.conf.StrFanart)
          {
            try
            {
              wyear = System.Convert.ToInt16(movieRecord["Year"]);
            }
            catch
            {
              wyear = 0;
            }
            try
            {
              wdirector = (string)movieRecord["Director"];
            }
            catch
            {
              wdirector = string.Empty;
            }
          }
          if (MyFilms.conf.StrTitle1 == "FormattedTitle") // special setting for formatted title - we don't want to use it, as it is usually too long and causes problems with path length
          {
            if (!string.IsNullOrEmpty(wttitle))
              fanartTitle = wttitle;
            else if (!string.IsNullOrEmpty(wtitle))
              fanartTitle = wtitle;
            else
              fanartTitle = wftitle;
          } 
          else
          {
            if (!string.IsNullOrEmpty(mastertitle))
              fanartTitle = mastertitle;
            else if (!string.IsNullOrEmpty(wtitle))
              fanartTitle = wtitle;
            else if (!string.IsNullOrEmpty(wttitle))
              fanartTitle = wttitle;
            else if (!string.IsNullOrEmpty(wftitle))
              fanartTitle = wftitle;
            else fanartTitle = "";
          }

          LogMyFilms.Debug("GetFanartTitle: returning fanartTitle: '" + fanartTitle + "' - mastertitle (" + MyFilms.conf.StrTitle1 + ") =  '" + mastertitle + "' - originaltitle: '" + wtitle + "' - translatedtitle: '" + wttitle + "' - formattedtitle: '" + wftitle + "' - director: '" + wdirector + "' - year: '" + wyear.ToString() + "'");
          return fanartTitle;
        }

        // returns the first title name of the configured mastertitle field
        public static string GetIMDB_Id(DataRow movieRecord)
        {
          string IMDB = "";
          if (!string.IsNullOrEmpty(movieRecord["IMDB_Id"].ToString()))
          {
            IMDB = movieRecord["IMDB_Id"].ToString();
          }
          else if (!string.IsNullOrEmpty(movieRecord["URL"].ToString()))
          {
            string CleanString = movieRecord["URL"].ToString();
            Regex CutText = new Regex("" + @"tt\d{7}" + "");
            Match m = CutText.Match(CleanString);
            if (m.Success)
              IMDB = m.Value;
          }
          //LogMyFilms.Debug("GetIMDB_Id: returning IMDB_Id: '" + IMDB + "'");
          return IMDB;
        }

        private static string RemoveGroupNames(string FullName)
        {
          string Name = FullName;
          if (MyFilms.conf != null && !string.IsNullOrEmpty(MyFilms.conf.TitleDelim)) // if called from BaseFilms
          {
            if (FullName.IndexOf(MyFilms.conf.TitleDelim) > 0)
              Name = FullName.Substring(FullName.IndexOf(MyFilms.conf.TitleDelim) + 1);
          }
          return Name;
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
                  try
                  {
                    DirectoryInfo dirsInf = new DirectoryInfo(MyFilms.conf.StrPathFanart + "\\{" + wtitle + "}");
                    FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();
                    foreach (FileSystemInfo sfi in sfiles)
                    {
                      try
                      {
                        System.IO.File.Delete(sfi.FullName);
                      }
                      catch (Exception ex)
                      {
                        LogMyFilms.Error("Remove_Backdrops_Fanart() - error on deletion in directory: '" + wtitle + "', file: '" + sfi.FullName.ToString() + "', exception: '" + ex.Message + "'");
                      }
                    }
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.Error("Remove_Backdrops_Fanart() - error on getting directory info: '" + wtitle + "', exception: '" + ex.Message + "'");
                  }
                }
            }
        }
        //-------------------------------------------------------------------------------------------
        //  Dowload backdrops on theMovieDB.org
        //-------------------------------------------------------------------------------------------        
        public static void Download_Backdrops_Fanart(string wtitle, string wttitle, string wftitle, string director, string imdbid, string year, bool choose, int wGetID, string savetitle, string personartworkpath, bool loadFanart, bool loadPersonImages)
        {
            new System.Threading.Thread(delegate()
            {
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            int wyear = 0;
            try { wyear = Convert.ToInt32(year); }
            catch { }
            try
            {
              GUIWaitCursor.Init();
              GUIWaitCursor.Show();
              List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(
                wtitle,
                savetitle,
                wyear,
                director,
                imdbid,
                MyFilms.conf.StrPathFanart,
                true,
                choose,
                MyFilms.conf.StrTitle1,
                personartworkpath);
              GUIWaitCursor.Hide();
              //System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, director, MyFilms.conf.StrPathFanart, true, choose);
              LogMyFilms.Debug("(DownloadBackdrops) - listemovies: '" + wtitle + "', '" + wttitle + "', '" + wyear + "', '" + director + "', '" + MyFilms.conf.StrPathFanart + "', 'true', '" + choose.ToString() + "', '" + MyFilms.conf.StrTitle1 + "'");
              int listCount = listemovies.Count;
              LogMyFilms.Debug("(DownloadBackdrops) - listemovies: Result Listcount: '" + listCount.ToString() + "'");

              if (choose) listCount = 2;
              switch (listCount)
              {
                case 0:
                  break;
                case 1:
                  LogMyFilms.Debug("Fanart " + listemovies[0].Name.Substring(listemovies[0].Name.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);
                  if (listemovies[0].Persons.Count > 0)
                  {
                    LogMyFilms.Debug("PersonArtwork: " + listemovies[0].Persons.Count.ToString() + " Personas checked for " + wttitle);
                    foreach (DBPersonInfo person in listemovies[0].Persons)
                    {
                      LogMyFilms.Debug("PersonArtwork: " + person.Images.Count.ToString() + " images found for " + person.Name);
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

                  GUIDialogMenu dlg =
                    (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                  if (dlg == null) return;
                  dlg.Reset();
                  if (loadFanart) dlg.SetHeading(GUILocalizeStrings.Get(1079862)); // Load fanart (online)
                  else dlg.SetHeading(GUILocalizeStrings.Get(1079900)); // Download person images (selected film)
                  dlg.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
                  foreach (DBMovieInfo t in listemovies)
                  {
                    string dialoginfoline = "";
                    dialoginfoline = t.Name + "  (" + t.Year + ")";
                    if (loadFanart) dialoginfoline += " - Fanarts: " + t.Backdrops.Count;
                    if (loadPersonImages) dialoginfoline += " - Persons: " + t.Persons.Count.ToString();
                    dlg.Add(dialoginfoline);
                    LogMyFilms.Debug(
                      "TMDB listemovies: " + t.Name + "  (" + t.Year + ") - Fanarts: " + t.Backdrops.Count +
                      " - TMDB-Id: " + t.Identifier + " - Persons: " + t.Persons.Count.ToString());
                  }
                  if (!(dlg.SelectedLabel > -1))
                  {
                    dlg.SelectedLabel = -1;
                    dlg.DoModal(wGetID);
                  }
                  if (dlg.SelectedLabel == 0)
                  {
                    #region Get SubTitles and Subwords from otitle and ttitle

                    wotitle_tableau = MyFilms.SubTitleGrabbing(wtitle);
                    wttitle_tableau = MyFilms.SubTitleGrabbing(wttitle);
                    wotitle_sub_tableau = MyFilms.SubWordGrabbing(wtitle, MinChars, Filter);
                      // Min 3 Chars, Filter true (no der die das)
                    wttitle_sub_tableau = MyFilms.SubWordGrabbing(wttitle, MinChars, Filter);
                      // Min 3 Chars, Filter true (no der die das)
                    //First Show Dialog to choose Otitle, Ttitle or substrings - or Keyboard to manually enter searchstring!!!
                    GUIDialogMenu dlgSearchFilm =
                      (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlgSearchFilm == null) return;
                    dlgSearchFilm.Reset();
                    dlgSearchFilm.SetHeading(GUILocalizeStrings.Get(1079859)); // choose search expression
                    dlgSearchFilm.Add("  *****  " + GUILocalizeStrings.Get(1079858) + "  *****  ");
                      //manual selection with keyboard
                    //dlgs.Add(wtitle); //Otitle
                    dlgSearchFilm.Add(savetitle); //Otitle = savetitle
                    dlgSearchFilm.Add(wttitle); //Ttitle
                    foreach (object t in wotitle_tableau)
                    {
                      if (t.ToString().Length > 1) dlgSearchFilm.Add(t.ToString());
                    }
                    foreach (object t in wttitle_tableau)
                    {
                      if (t.ToString().Length > 1) dlgSearchFilm.Add(t.ToString());
                    }
                    foreach (object t in wotitle_sub_tableau)
                    {
                      if (t.ToString().Length > 1) dlgSearchFilm.Add(t.ToString());
                    }
                    foreach (object t in wttitle_sub_tableau)
                    {
                      if (t.ToString().Length > 1) dlgSearchFilm.Add(t.ToString());
                    }
                    //Now all titles and Substrings listed in dialog !
                    //dlgs.Add("  *****  " + GUILocalizeStrings.Get(1079860) + "  *****  "); //manual selection
                    if (!(dlgSearchFilm.SelectedLabel > -1))
                    {
                      dlgSearchFilm.SelectedLabel = -1;
                      dlgSearchFilm.DoModal(wGetID);
                    }
                    if (dlgSearchFilm.SelectedLabel == 0) // enter manual searchstring via VK
                    {
                      VirtualKeyboard keyboard =
                        (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                      if (null == keyboard) return;
                      keyboard.Reset();
                      keyboard.Text = wtitle;
                      keyboard.DoModal(wGetID);
                      if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                      {
                        //Remove_Backdrops_Fanart(wtitle, true);
                        //Remove_Backdrops_Fanart(wttitle, true);
                        //Remove_Backdrops_Fanart(wftitle, true);
                        Download_Backdrops_Fanart(
                          keyboard.Text,
                          string.Empty,
                          string.Empty,
                          string.Empty,
                          string.Empty,
                          string.Empty,
                          true,
                          wGetID,
                          savetitle,
                          personartworkpath,
                          loadFanart,
                          loadPersonImages);
                      }
                      break;
                    }
                    if (dlgSearchFilm.SelectedLabel > 0 && dlgSearchFilm.SelectedLabel < 3) // if one of otitle or ttitle selected, keep year and director
                    {
                      Download_Backdrops_Fanart(
                        dlgSearchFilm.SelectedLabelText,
                        wttitle,
                        wftitle,
                        year,
                        director,
                        string.Empty,
                        true,
                        wGetID,
                        savetitle,
                        personartworkpath,
                        loadFanart,
                        loadPersonImages);
                      //Download_Backdrops_Fanart(string wtitle, string wttitle, string director, string year, bool choose,int wGetID, string savetitle)
                      break;
                    }
                    if (dlgSearchFilm.SelectedLabel > 2) // For subitems, search without year and director !
                    {
                      Download_Backdrops_Fanart(
                        dlgSearchFilm.SelectedLabelText,
                        wttitle,
                        wftitle,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        true,
                        wGetID,
                        savetitle,
                        personartworkpath,
                        loadFanart,
                        loadPersonImages);
                      //Download_Backdrops_Fanart(string wtitle, string wttitle, string director, string year, bool choose,int wGetID, string savetitle)
                      break;
                    }

                    #endregion
                  }
                  if (dlg.SelectedLabel > 0)
                  {
                    // Load Fanart  -> show progress dialog !

                    GUIDialogProgress dlgPrgrs = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
                    if (dlgPrgrs != null)
                    {
                      dlgPrgrs.Reset();
                      dlgPrgrs.DisplayProgressBar = true;
                      dlgPrgrs.ShowWaitCursor = false;
                      dlgPrgrs.DisableCancel(true);
                      dlgPrgrs.SetHeading("MyFilms Artwork Download");
                      dlgPrgrs.StartModal(GUIWindowManager.ActiveWindow);
                      dlgPrgrs.SetLine(1, "Loading Artwork ...");
                      dlgPrgrs.Percentage = 0;

                      #region load fanarts ...
                      bool first = true;
                      string filename = string.Empty;
                      string filename1 = string.Empty;
                      //if (MyFilms.conf.StrTitle1 == "OriginalTitle")
                      //  wttitle = savetitle; // Was wttitle = wtitle;
                      int i = 0;
                      if (loadFanart) // Download Fanart
                      {
                        if (dlgPrgrs != null) dlgPrgrs.SetLine(1, "Loading Fanart for '" + savetitle + "'");

                        foreach (string backdrop in listemovies[dlg.SelectedLabel - 1].Backdrops)
                        {
                          filename1 = Grabber.GrabUtil.DownloadBacdropArt(
                            MyFilms.conf.StrPathFanart, backdrop, savetitle, true, first, out filename);
                          if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "loading '" + System.IO.Path.GetFileName(filename) + "'");
                          if (dlgPrgrs != null) dlgPrgrs.Percentage = i * 100 / listemovies[dlg.SelectedLabel - 1].Backdrops.Count;
                          LogMyFilms.Debug("Fanart " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + savetitle);

                          if (filename == string.Empty) filename = filename1;
                          if (!(filename == "already" && filename1 == "already")) filename = "added";
                          first = false;
                          i++;
                        }
                      }
                      #endregion

                      listemovies[0].Name = filename;

                      if (loadPersonImages) // Download PersonArtwork
                      {
                        string filenameperson = string.Empty;
                        string filename1person = string.Empty;
                        string filename2person = string.Empty;
                        LogMyFilms.Debug(
                          "Person Artwork - " + listemovies[0].Persons.Count + " persons found - now loading artwork");
                        if (!string.IsNullOrEmpty(personartworkpath) && listemovies[0].Persons != null &&
                            listemovies[0].Persons.Count > 0)
                        {
                          if (dlgPrgrs != null) dlgPrgrs.SetLine(1, "Loading person images for '" + wttitle + "'");
                          if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "");

                          foreach (grabber.DBPersonInfo person in listemovies[0].Persons)
                          {
                            bool firstpersonimage = true;
                            bool onlysinglepersonimage = true;
                            grabber.DBPersonInfo persondetails = new DBPersonInfo();
                            grabber.TheMoviedb TheMoviedb = new grabber.TheMoviedb();
                            persondetails = TheMoviedb.getPersonsById(person.Id, string.Empty);
                            LogMyFilms.Debug("Person Artwork: found '" + persondetails.Images.Count + "' TMDB images for '" + persondetails.Name + "' in movie '" + savetitle + "'");
                            if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "loading '" + persondetails.Name + "'");
                            if (dlgPrgrs != null) dlgPrgrs.Percentage = 0;

                            if (persondetails.Images.Count > 0)
                            {
                              i = 0;
                              foreach (var image in persondetails.Images)
                              {
                                filename1person = Grabber.GrabUtil.DownloadPersonArtwork(personartworkpath, image, persondetails.Name, true, firstpersonimage, out filenameperson);
                                if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "loading '" + persondetails.Name + "' (TMDB - #" + i + ")");
                                if (dlgPrgrs != null) dlgPrgrs.Percentage = i * 100 / persondetails.Images.Count;

                                LogMyFilms.Debug("Person Artwork " + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + " downloaded for '" + persondetails.Name + "' in movie '" + savetitle + "', path='" + filename1person + "'");
                                if (filenameperson == string.Empty) filenameperson = filename1person;
                                if (!(filenameperson == "already" && filename1person == "already")) filenameperson = "added";
                                firstpersonimage = false;
                                i++;
                                if (onlysinglepersonimage) break;
                              }
                            }
                            else
                            {
                              #region Get further IMDB images

                              if (dlgPrgrs != null) dlgPrgrs.SetLine(1, "Loading additional IMDB person images ...");
                              if (dlgPrgrs != null) dlgPrgrs.Percentage = 0;
                              IMDB _imdb = new IMDB();
                              IMDB.IMDBUrl wurl;
                              _imdb.FindActor(persondetails.Name);
                              //Grabber.MyFilmsIMDBActor imdbActor = new Grabber.MyFilmsIMDBActor();
                              IMDBActor imdbActor = new IMDBActor();

                              if (_imdb.Count > 0)
                              {
                                if (dlgPrgrs != null) dlgPrgrs.SetLine(2, "loading '" + persondetails.Name + "' (IMDB)");
                                string url = string.Empty;
                                wurl = (IMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
                                if (wurl.URL.Length != 0)
                                {
                                  url = wurl.URL;
                                  //url = wurl.URL + "videogallery"; // Assign proper Webpage for Actorinfos
                                  //url = MyFilms.ImdbBaseUrl + url.Substring(url.IndexOf("name"));
                                  Grabber.Grabber_URLClass fetchactor = new Grabber_URLClass();
                                  fetchactor.GetActorDetails(url, persondetails.Name, false, out imdbActor);
                                  if (dlgPrgrs != null) dlgPrgrs.Percentage = 50;
                                  if (imdbActor.ThumbnailUrl.Length > 0)
                                  {
                                    filename1person = GrabUtil.DownloadPersonArtwork(personartworkpath, imdbActor.ThumbnailUrl, persondetails.Name, true, firstpersonimage, out filenameperson);
                                    LogMyFilms.Debug("Person Artwork (IMDB) " + " downloaded for '" + persondetails.Name + "' in movie '" + wttitle + "', path='" + filename1person + "'"); // filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + 
                                  }
                                  if (dlgPrgrs != null) dlgPrgrs.Percentage = 100;

                                  // Add actor to datbbase to get infos in person facades later...
                                  try
                                  {
                                    int actorId = VideoDatabase.AddActor(imdbActor.Name);
                                    if (actorId > 0)
                                    {
                                      VideoDatabase.SetActorInfo(actorId, imdbActor);
                                      //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);

                                      // Deactivated, as downloading not yet working !!!
                                      //if (imdbActor.ThumbnailUrl != string.Empty)
                                      //{
                                      //  string largeCoverArt = Utils.GetLargeCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                                      //  string coverArt = Utils.GetCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                                      //  Utils.FileDelete(largeCoverArt);
                                      //  Utils.FileDelete(coverArt);
                                      //  //DownloadCoverArt(Thumbs.MovieActors, imdbActor.ThumbnailUrl, imdbActor.Name);
                                      //}
                                    }
                                  }
                                  catch (Exception ex)
                                  {
                                    LogMyFilms.Debug("Error adding person to VDB: " + ex.Message);
                                  }
                                }
                              }
                              else
                              {
                                try
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
                                catch (Exception ex)
                                {
                                  LogMyFilms.Debug("Error adding person to VDB: " + ex.Message);
                                }
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

                              #endregion
                            }
                          }
                        }
                        else if (string.IsNullOrEmpty(personartworkpath)) LogMyFilms.Debug("No Personartwork loaded - Personartworkpath is not set in setup!");
                      }
                      if (dlgPrgrs != null) dlgPrgrs.Percentage = 100;
                      dlgPrgrs.ShowWaitCursor = false;
                      dlgPrgrs.SetLine(1, GUILocalizeStrings.Get(1079846));
                      dlgPrgrs.SetLine(2, "");
                      Thread.Sleep(50);
                      dlgPrgrs.Close(); // Done...
                      return;
                    }
                  }
                  break;
              }
            }
            catch (Exception ex)
            {
              LogMyFilms.DebugException("Thread 'MyFilmsTMDBLoader' - exception! - ", ex);
            }
                GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
                {
                    //dlgPrgrs.ShowWaitCursor = false;
                    // enter here what to load after background thread has finished !
                    if (DetailsUpdated != null) DetailsUpdated(true);
                    return 0;
                }, 0, 0, null);
            }) { Name = "MyFilmsTMDBLoader", IsBackground = true }.Start();
        }

      
        //-------------------------------------------------------------------------------------------
        //  Search Fanart Thumbs 
        //                          title = Translated Title if any or Original Title
        //                          main = true for main screen and false for detailed
        //                          searched = dir for Directory searched (detail screen and control multiImage)or file 
        //                          rep = true if the selected Item is a grouped Item (country, genre, year...) on main screen
        //                          filecover = name of the file cover for using 'Default Cover for missing Fanart'
        //      value returned string[2]
        //                          [0] = file or directory found (if not " ")
        //                          [1] = file or dir
        //-------------------------------------------------------------------------------------------

        public static string[] Search_Fanart(string title, bool main, string searched, bool rep, string filecover, string group)
        {
          return Search_Fanart(title, main, searched, rep, filecover, group, MyFilms.conf);
        }
        public static string[] Search_Fanart(string title, bool main, string searched, bool rep, string filecover, string group, Configuration tmpconf)
        //                     Search_Fanart(wlabel, true, "file", false, facadeFilms.SelectedListItem.ThumbnailImage.ToString(), string.Empty);
        {
            //if (MyFilms.conf == tmpconf) LogMyFilms.Debug("Search_Fanart(): Using '" + title + "'");
            string[] wfanart = new string[2];
            wfanart[0] = " ";
            wfanart[1] = " ";
            if (tmpconf.StrFanart)
            {
              if (title.Contains(tmpconf.TitleDelim))
                title = title.Substring(title.LastIndexOf(tmpconf.TitleDelim) + 1); // Removed "trim", as there is no matching in details, if spacees are removed! old: title = title.Substring(title.LastIndexOf(tmpconf.TitleDelim) + 1).Trim();
                title = Grabber.GrabUtil.CreateFilename(title.ToLower()).Replace(' ', '.');

                if (!tmpconf.StrFanart)
                    return wfanart;

                string safeName = string.Empty;
                if (rep) // is group view 
                {
                  if ((group == "country" || group == "year" || group == "category") && tmpconf.StrFanartDefaultViews) // Default views and fanart for group view enabled?
                    {
                      if (!System.IO.Directory.Exists(tmpconf.StrPathFanart + "\\_View")) System.IO.Directory.CreateDirectory(tmpconf.StrPathFanart + "\\_View");
                      if (!System.IO.Directory.Exists(tmpconf.StrPathFanart + "\\_View\\" + group)) System.IO.Directory.CreateDirectory(tmpconf.StrPathFanart + "\\_View\\" + group);
                      safeName = tmpconf.StrPathFanart + "\\_View\\" + group + "\\{" + title + "}";
                    }
                    else
                    if (tmpconf.StrFanartDfltImageAll && (wfanart[0] == "" || wfanart[0] == " "))
                      {
                        wfanart[0] = tmpconf.DefaultFanartImage;
                        wfanart[1] = "file";
                        return wfanart;
                      }
                }
                else

                  if ((tmpconf.StrPathFanart.ToString() + title + "\\{" + title + "}.jpg").Length > 259) // Added to avoid crash with very long filenames - better is if user configures titledelimiters properly !
                     {
                         return wfanart;
                     }
                    else
                    {
                      safeName = tmpconf.StrPathFanart + "\\{" + title + "}";
                    }

                
                try //Added to avoid crash with very long filenames - better is if user configures titledelimiters properly !
                    {
                        FileInfo wfile = new FileInfo(safeName + "\\{" + title + "}.jpg");
                    }
                catch (Exception e)
                    {
                        LogMyFilms.Error("Title too long to create fanart path/filename from it - not loading fanart! - Exception: " + e.ToString());
                        return wfanart;
                    }
                
                //LogMyFilms.Debug("(SearchFanart) - safename(file) = '" + wfile + "'");
                //LogMyFilms.Debug("(SearchFanart) - safename(file&ext) = '" + (safeName + "\\{" + title + "}.jpg") + "'");
                if (((main) || (searched == "file")) && (System.IO.File.Exists(safeName + "\\{" + title + "}.jpg")))
                {
                    wfanart[0] = safeName + "\\{" + title + "}.jpg";
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
                            LogMyFilms.Debug("Search_Fanart(): File Mode - searchtitle = '" + title + "', safename = '" + safeName + "'wfanart[0,1]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
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
                switch (tmpconf.StrFileType)
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
                    if (!string.IsNullOrEmpty(tmpconf.StrPathFanart)) //Search matching files in XMM fanart directory
                    {
                      string searchname = HTMLParser.removeHtml(title).Replace(" ", "."); // replaces special character "á" and other special chars !
                      //searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
                      searchname = searchname + ".*.jpg";
                      string[] files = Directory.GetFiles(tmpconf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
                      if (files.Count() > 0)
                      {
                        wfanart[0] = files[0];
                        wfanart[1] = "file";
                        return wfanart;
                      }
                    }
                    break;
                  case Configuration.CatalogType.PersonalVideoDatabase: // PVD artist thumbs: e.g. Natalie Portman_1.jpg , then Natalie Portman_2.jpg 
                    if (!string.IsNullOrEmpty(tmpconf.StrPathFanart)) //Search matching files in XMM fanart directory
                    {
                      string searchname = HTMLParser.removeHtml(title); // replaces special character "á" and other special chars !
                      //searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
                      searchname = searchname + "*.jpg";
                      string[] files = Directory.GetFiles(tmpconf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
                      if (files.Count() > 0)
                      {
                        wfanart[0] = files[0];
                        wfanart[1] = "file";
                        return wfanart;
                      }
                    }
                    break;
                  case Configuration.CatalogType.eXtremeMovieManager:
                    if (!string.IsNullOrEmpty(tmpconf.StrPathFanart)) //Search matching files in XMM fanart directory
                    {
                      string searchname = HTMLParser.removeHtml(title).Replace(" ", "-"); // replaces special character "á" and other special chars !
                      searchname = searchname.Replace(" ", "-");
                      searchname = searchname.Replace(".", "-");
                      searchname = searchname.Replace("'", "-");
                      searchname = "*" + Regex.Replace(searchname, "[\n\r\t]", "-") + "_fanart*.jpg";
                      string[] files = Directory.GetFiles(tmpconf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
                      LogMyFilms.Debug("Search_Fanart - XMM - search for '" + searchname + "'");
                      if (files.Count() > 0)
                      {
                        wfanart[0] = files[0];
                        wfanart[1] = "file";
                        return wfanart;
                      }
                    }
                    break;

                  case Configuration.CatalogType.XBMC: // XBMC fulldb export (all movies in one DB)
                    break;

                  case Configuration.CatalogType.MovingPicturesXML:
                    if (!string.IsNullOrEmpty(tmpconf.StrPathFanart)) //Search matching files in MoPi fanart directory
                    {
                      string searchname = HTMLParser.removeHtml(title); // replaces special character "á" and other special chars !
                      //searchname = Regex.Replace(searchname, "[\n\r\t]", "-") + "_*.jpg";
                      searchname = "{" + searchname + "}" + "*.jpg";
                      string[] files = Directory.GetFiles(tmpconf.StrPathFanart, searchname, SearchOption.TopDirectoryOnly);
                      if (files.Count() > 0)
                      {
                        wfanart[0] = files[0];
                        wfanart[1] = "file";
                        return wfanart;
                      }
                    }
                    break;

                  case Configuration.CatalogType.XBMCnfoReader: // XBMC Nfo (separate nfo files, to scan dirs - MovingPictures or XBMC)
                    break;

                  default:
                    break;
                }
                if ((tmpconf.StrFanartDflt) && !(rep) && System.IO.File.Exists(filecover))
                {
                    wfanart[0] = filecover;
                    wfanart[1] = "file";
                    //Added Guzzi - Fix that no fanart was returned ...
                    return wfanart;
                }
                if (tmpconf.StrFanartDfltImage && (wfanart[0] == "" || wfanart[0] == " "))
                {
                  wfanart[0] = tmpconf.DefaultFanartImage;
                  wfanart[1] = "file";
                }
            }
            
            // LogMyFilms.Debug("(SearchFanart) - Fanart config for '" + title + "': wfanart[0,1]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
            return wfanart;
        }

        public static string CreateOrUpdateActor(string name)
        {
            // Get IMDB images
            IMDB _imdb = new IMDB();
            IMDB.IMDBUrl wurl;
            _imdb.FindActor(name);
            IMDBActor imdbActor = new IMDBActor();
            string filename1person = string.Empty;

            if (_imdb.Count == 0)
            {
              string url = string.Empty;
              wurl = (IMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
              if (wurl.URL.Length != 0)
              {
                url = wurl.URL;
                //url = wurl.URL + "videogallery"; // Assign proper Webpage for Actorinfos
                //url = MyFilms.ImdbBaseUrl + url.Substring(url.IndexOf("name"));
                Grabber.Grabber_URLClass fetchactor = new Grabber_URLClass();
                fetchactor.GetActorDetails(url, name, false, out imdbActor);
                string filenameperson;
                if (!string.IsNullOrEmpty(MyFilms.conf.StrPathArtist))
                  filename1person = GrabUtil.DownloadPersonArtwork(MyFilms.conf.StrPathArtist, imdbActor.ThumbnailUrl, imdbActor.Name, true, true, out filenameperson);
                else
                  LogMyFilms.Debug("CreateOrUpdateActor() - Personartworkpath not set - no image download possible !");

                LogMyFilms.Info("Person Artwork " + filename1person.Substring(filename1person.LastIndexOf("\\") + 1) + " downloaded for '" + name + "', path='" + filename1person + "'");
                // Add actor to datbbase to get infos in person facades later...
                int actorId = VideoDatabase.AddActor(imdbActor.Name);
                if (actorId > 0)
                {
                  VideoDatabase.SetActorInfo(actorId, imdbActor);
                  //VideoDatabase.AddActorToMovie(_movieDetails.ID, actorId);

                  // Deactivated, as downloading not yet working !!!
                  //if (imdbActor.ThumbnailUrl != string.Empty)
                  //{
                  //  string largeCoverArt = Utils.GetLargeCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                  //  string coverArt = Utils.GetCoverArtName(Thumbs.MovieActors, imdbActor.Name);
                  //  Utils.FileDelete(largeCoverArt);
                  //  Utils.FileDelete(coverArt);
                  //  //DownloadCoverArt(Thumbs.MovieActors, imdbActor.ThumbnailUrl, imdbActor.Name);
                  //}
                }
              }
            }
            else
            {
              int actorId = VideoDatabase.AddActor(imdbActor.Name);
              imdbActor.Name = name;
              VideoDatabase.SetActorInfo(actorId, imdbActor);
            }
          return filename1person;
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

        private void OnDetailsUpdated(bool searchPicture)
        {
          LogMyFilms.Debug("OnDetailsUpdated(): Received DetailUpdated event in context '" + GetID + "', doUpdateDetailsViewByFinishEvent '" + doUpdateDetailsViewByFinishEvent + "'"); 
          if (GetID == MyFilms.ID_MyFilmsDetail && doUpdateDetailsViewByFinishEvent)
          {
            LogMyFilms.Debug("OnDetailsUpdated(): now reloading Details"); 
            doUpdateDetailsViewByFinishEvent = false;
            afficher_detail(searchPicture, false);
          }
          else
            LogMyFilms.Debug("OnDetailsUpdated(): Skipping reloading Details");
        }

        private void afficher_detail(bool searchPicture)
        {
          afficher_detail(searchPicture, false);
        }

        private void afficher_detail(bool searchPicture, bool checkfileavailability)
        {
            //-----------------------------------------------------------------------------------------------------------------------
            //    Load Detailed Info
            //-----------------------------------------------------------------------------------------------------------------------
            MyFilms.currentMovie.Reset(); // clear currentmovie

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
                MyFilms.currentMovie.Title = MyFilms.conf.StrTIndex;
                GUIControl.ShowControl(GetID, (int)Controls.CTRL_Title);
            }
            else
                GUIControl.HideControl(GetID, (int)Controls.CTRL_Title);
            if (Helper.FieldIsSet(MyFilms.conf.StrTitle2))
            {
                if ((MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle1].ToString() == MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString()) || (MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrTitle2].ToString().Length == 0))
                    GUIControl.HideControl(GetID, (int)Controls.CTRL_OTitle);
                else
                    GUIControl.ShowControl(GetID, (int)Controls.CTRL_OTitle);
            }
            else
                GUIControl.HideControl(GetID, (int)Controls.CTRL_OTitle);


            int year = 1900;
            Int32.TryParse(MyFilms.r[MyFilms.conf.StrIndex]["Year"].ToString(), out year);
            MyFilms.currentMovie.Year = year;
            string IMDB = "";
            if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Id"].ToString()))
              IMDB = MyFilms.r[MyFilms.conf.StrIndex]["IMDB_Id"].ToString();
            if (!string.IsNullOrEmpty(MyFilms.r[MyFilms.conf.StrIndex]["URL"].ToString()) && string.IsNullOrEmpty(IMDB))
            {
              string CleanString = MyFilms.r[MyFilms.conf.StrIndex]["URL"].ToString();
              Regex CutText = new Regex("" + @"tt\d{7}" + "");
              Match m = CutText.Match(CleanString);
              if (m.Success)
                IMDB = m.Value;
            }
            MyFilms.currentMovie.IMDBNumber = IMDB;

            string file = "false";
            if (MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().Length > 0)
            {
                if ((MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().IndexOf(":\\") == -1) && (MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().Substring(0, 2) != "\\\\"))
                    file = MyFilms.conf.StrPathImg + "\\" + MyFilms.r[MyFilms.conf.StrIndex]["Picture"];
                else
                    file = MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString();
            }
            else
                file = string.Empty;
            if (!System.IO.File.Exists(file) || string.IsNullOrEmpty(file))
                file = MyFilms.conf.DefaultCover;
            //Should not Disable because of SpeedThumbs - Not working here .....
            setGUIProperty("picture", file);
            MyFilms.currentMovie.Picture = file;
            // ToDo: Add for ImageSwapper Coverart (coverImage)
            ////cover.Filename = file;
            //if (ImgDetFilm != null)
            //  ImgDetFilm.FileName = file;

            // load the rest threaded - logos and fanart might take a bit ...
            new System.Threading.Thread(delegate()
            {
              {
                #region threaded loading of logos, fanart and file availability check ...
                
                // load detailed DB infos
                Load_Detailed_DB(MyFilms.conf.StrIndex, true);

                // Logos:
                this.Load_Logos(MyFilms.r[MyFilms.conf.StrIndex]);

                //ImageSwapper backdrop = new ImageSwapper();
                string[] wfanart = new string[2];
                Searchtitles sTitles = GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");

                if (ImgFanartDir != null)
                {
                  wfanart = Search_Fanart(sTitles.FanartTitle, false, "dir", false, file, string.Empty);
                  LogMyFilms.Debug("(afficher_detail): Backdrops-File (dir): wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                }
                else
                {
                  wfanart = Search_Fanart(sTitles.FanartTitle, false, "file", false, file, string.Empty);
                  LogMyFilms.Debug("(afficher_detail): Backdrops-File (file): wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
                }

                if (wfanart[0] == " ")
                {
                  if (ImgFanartDir != null) ImgFanartDir.PreAllocResources();
                  // GUIControl.HideControl(GetID, 35);
                }
                else
                {
                  // GUIControl.ShowControl(GetID, 35);
                  if (wfanart[1] == "dir" && ImgFanartDir != null)
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
                    setGUIProperty("currentfanart", wfanart[0]);
                    GUIControl.ShowControl(GetID, (int)Controls.CTRL_Fanart);
                  }
                }
                MyFilms.currentMovie.Fanart = Search_Fanart(sTitles.FanartTitle, false, "file", false, file, string.Empty)[0];

                if (Helper.FieldIsSet(MyFilms.conf.StrStorage) && checkfileavailability)
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
                #endregion
              }
              GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
              {
                {
                  // Do this after thread finished ...
                }
                return 0;
              }, 0, 0, null);
            }) { Name = "MyFilmsOnPageLoadWorker", IsBackground = true }.Start();

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
          //        LogMyFilms.Error("" + e.Message);
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
          //            LogMyFilms.Error("" + e.Message);
          //        }
          //        try
          //        {
          //            setGUIProperty("logos_id2002", Logos.Build_Logos(MyFilms.r[MyFilms.conf.StrIndex], "ID2002", ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, 1, GetID));
          //            //GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2002);
          //            //GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2002);
          //        }
          //        catch (Exception e)
          //        {
          //            LogMyFilms.Error("" + e.Message);
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
              LogMyFilms.Error("" + e.Message);
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
              LogMyFilms.Error("" + e.Message);
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
              LogMyFilms.Error("" + e.Message);
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
              LogMyFilms.Error("" + e.Message);
            }
          }
          else
            clearGUIProperty("logos_id2012");
        }

        //-------------------------------------------------------------------------------------------
        //  initialize exported fields to skin as '#myfilms.<ant db column name> 
        //-------------------------------------------------------------------------------------------
        public static void Init_Detailed_DB(bool log)
        {
          LogMyFilms.Debug("Init_Detailed_DB() - log = '" + log + "'"); 
            AntMovieCatalog ds = new AntMovieCatalog();
            foreach (DataColumn dc in ds.Movie.Columns)
            {
              //clearGUIProperty("db." + dc.ColumnName.ToLower() + ".label"); // Don't Clear Labels - they're only set once when plugin start !
              clearGUIProperty("db." + dc.ColumnName.ToLower() + ".value", log);
            }
            
            MyFilms.currentMovie.Reset();

            //Clear userdefined properties
            clearGUIProperty("db.calc.format.value", log);
            clearGUIProperty("db.calc.aspectratio.value", log);
            clearGUIProperty("db.calc.imageformat.value", log);

            //for (int j = 1; j < 6; j++)
            //{
            //  clearGUIProperty("db.actors.actor" + j + ".name", log);
            //  clearGUIProperty("db.actors.actor" + j + ".image", log);
            //}
            clearGUIProperty("db.actors.actor1.name", log);
            clearGUIProperty("db.actors.actor1.image", log);
            clearGUIProperty("db.actors.actor2.name", log);
            clearGUIProperty("db.actors.actor2.image", log);
            clearGUIProperty("db.actors.actor3.name", log);
            clearGUIProperty("db.actors.actor3.image", log);
            clearGUIProperty("db.actors.actor4.name", log);
            clearGUIProperty("db.actors.actor4.image", log);
            clearGUIProperty("db.actors.actor5.name", log);
            clearGUIProperty("db.actors.actor5.image", log);

            clearGUIProperty("user.mastertitle.label", log);
            clearGUIProperty("user.mastertitle.value", log);
            clearGUIProperty("user.mastertitle.groupname", log);
            clearGUIProperty("user.mastertitle.groupcount", log);
            clearGUIProperty("user.secondarytitle.label", log);
            clearGUIProperty("user.secondarytitle.value", log);
            clearGUIProperty("user.secondarytitle.groupname", log);
            clearGUIProperty("user.item1.label", log);
            clearGUIProperty("user.item1.field", log);
            clearGUIProperty("user.item1.value", log);
            clearGUIProperty("user.item2.label", log);
            clearGUIProperty("user.item2.field", log);
            clearGUIProperty("user.item2.value", log);
            clearGUIProperty("user.item3.label", log);
            clearGUIProperty("user.item3.field", log);
            clearGUIProperty("user.item3.value", log);
            clearGUIProperty("user.item4.label", log);
            clearGUIProperty("user.item4.field", log);
            clearGUIProperty("user.item4.value", log);
            clearGUIProperty("user.item5.label", log);
            clearGUIProperty("user.item5.field", log);
            clearGUIProperty("user.item5.value", log);
            clearGUIProperty("user.source.value", log);
            clearGUIProperty("user.sourcetrailer.value", log);
            clearGUIProperty("user.sourcetrailer.count", log);
            clearGUIProperty("user.watched.value", log);
            clearGUIProperty("user.watched.count", log);
            clearGUIProperty("user.watched.date", log);
            clearGUIProperty("user.watched.name", log);
            clearGUIProperty("user.watched.global", log);
            clearGUIProperty("user.source.isonline", log);
            clearGUIProperty("user.sourcetrailer.isonline", log);
        }

        //-------------------------------------------------------------------------------------------
        //  Load detailed db fields : export fields to skin as '#myfilms.<ant db column name> 
        //-------------------------------------------------------------------------------------------
        public static void Load_Detailed_DB(int ItemId, bool wrep)
        {
            LogMyFilms.Debug("Load_Detailed_DB() - ItemId: '" + ItemId + "', Details (wrep): '" + wrep + "'");
			      Stopwatch stopwatch = new Stopwatch();
            stopwatch.Reset(); stopwatch.Start();
            string wstrformat = "";
            AntMovieCatalog ds = new AntMovieCatalog();

            if (MyFilms.r == null || ItemId > MyFilms.r.Length - 1)
            {
              LogMyFilms.Warn("Load_Detailed_DB() - Failed loading details - index '" + ItemId + "' not within current dataset ... now clearing properties ...");
              Init_Detailed_DB(false);
              return;
            }
            foreach (DataColumn dc in ds.Movie.Columns)
            {
              string wstring = "";
              string wstring2 = "";
              //LogMyFilms.Debug("PropertyManager: Set Properties for DB Column '" + dc.ColumnName + "' - '" + BaseMesFilms.Translate_Column(dc.ColumnName) + "'");

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
                            setGUIProperty("user.item3.label", MyFilms.conf.Strlabel3);
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
                    if (wrep && MyFilms.conf.StrStorage.ToLower() == (dc.ColumnName.ToLower()))
                    {
                      setGUIProperty("user.source.value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                      MyFilms.currentMovie.File = MyFilms.r[ItemId][dc.ColumnName].ToString();
                    }

                    if (wrep && MyFilms.conf.StrStorageTrailer.ToLower() == (dc.ColumnName.ToLower()))
                    {
                      setGUIProperty("user.sourcetrailer.value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                      MyFilms.currentMovie.Trailer = MyFilms.r[ItemId][dc.ColumnName].ToString();
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
                      if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
                      {
                        int count = GetWatchedCount(ItemId, MyFilms.conf.StrUserProfileName);
                        if (count > 0)
                        {
                          setGUIProperty("user.watched.value", "true");
                          DateTime watcheddate = GetWatchedDate(ItemId, MyFilms.conf.StrUserProfileName);
                          string strWatchedDate = (watcheddate > DateTime.MinValue) ? watcheddate.ToShortDateString() : "";
                          setGUIProperty("user.watched.date", strWatchedDate);
                        }
                        else
                        {
                          setGUIProperty("user.watched.value", ""); // set to empty, if movie is unwatched
                          setGUIProperty("user.watched.date", "");
                        }
                        setGUIProperty("user.watched.count", count.ToString());
                        setGUIProperty("user.watched.name", MyFilms.conf.StrUserProfileName);
                        setGUIProperty("user.watched.global", GetWatchedCount(ItemId, "Global").ToString());
                      }
                      else
                      {
                        if (MyFilms.r[ItemId][dc.ColumnName].ToString().ToLower() != MyFilms.conf.GlobalUnwatchedOnlyValue.ToLower())
                          setGUIProperty("user.watched.value", "true");
                        else
                          setGUIProperty("user.watched.value", ""); // set to empty, if movie is unwatched
                      }
                    }

                    switch (dc.ColumnName.ToLower())
                    {
                        case "translatedtitle":
                        case "originaltitle":
                        case "formattedtitle":
                            if (wrep)
                                if (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                                    if (MyFilms.r[ItemId][dc.ColumnName].ToString().Contains(MyFilms.conf.TitleDelim))
                                    {
                                      wstring = MyFilms.r[ItemId][dc.ColumnName].ToString().Substring(MyFilms.r[ItemId][dc.ColumnName].ToString().LastIndexOf(MyFilms.conf.TitleDelim) + 1);
                                      wstring2 = MyFilms.r[ItemId][dc.ColumnName].ToString().Substring(0, MyFilms.r[ItemId][dc.ColumnName].ToString().LastIndexOf(MyFilms.conf.TitleDelim));
                                    }
                                    else
                                    {
                                      wstring = MyFilms.r[ItemId][dc.ColumnName].ToString();
                                      wstring2 = "";
                                    }
                            setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);

                            if ((MyFilms.conf.StrTitle1.ToLower() == (dc.ColumnName.ToLower())))
                            {
                              // MyFilms.currentMovie.Title = wstring; // already set in afficher_detail()
                              if (wrep)
                              {
                                setGUIProperty("user.mastertitle.value", wstring);
                                setGUIProperty("user.mastertitle.groupname", wstring2);
                              }
                              else
                              {
                                clearGUIProperty("user.mastertitle.value");
                                clearGUIProperty("user.mastertitle.groupname");
                              }
                            }
                            if ((MyFilms.conf.StrTitle2.ToLower() == (dc.ColumnName.ToLower())))
                                if (wrep)
                                {
                                  setGUIProperty("user.secondarytitle.value", wstring);
                                  setGUIProperty("user.secondarytitle.groupname", wstring2);
                                }
                                    else
                                {
                                  clearGUIProperty("user.secondarytitle.value");
                                  clearGUIProperty("user.secondarytitle.groupname");
                                }
                            break;

                        case "length":
                            if (wrep)
                            {
                              int length = 0;
                              if (MyFilms.r[ItemId]["Length"].ToString().Length > 0)
                                wstring = MyFilms.r[ItemId]["Length"].ToString();
                              setGUIProperty("db.length.value", wstring);
                              bool success = int.TryParse(wstring, out length);
                              MyFilms.currentMovie.Length = (success) ? length : 0;
                            }
                            else
                            {
                              clearGUIProperty("db.length.value");
                            }
                            break;
                        case "actors":
                            if (wrep)
                                if (MyFilms.r[ItemId]["Actors"].ToString().Length > 0)
                                {
                                    wstring = MyFilms.r[ItemId]["Actors"].ToString().Replace('|', '\n');
                                    wstring = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wstring.ToString()));
                                }
                            setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                            Load_Detailed_DB_PushActorsToSkin(wstring);
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
                        case "fanart":
                            if ((MyFilms.currentMovie.Fanart.Length == 0) && (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                              MyFilms.currentMovie.Fanart = MyFilms.r[ItemId][dc.ColumnName].ToString();
                            break;
                        case "imdb_id":
                            if ((wrep) && (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                              MyFilms.currentMovie.IMDBNumber = MyFilms.r[ItemId][dc.ColumnName].ToString();
                            else
                              MyFilms.currentMovie.IMDBNumber = "";
                            break;
                        case "tmdb_id":
                        case "watched":
                            break;

                        case "isonline":
                            // old method
                            //if (wrep && MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                            //{
                            //  if (MyFilms.InitialIsOnlineScan)
                            //    setGUIProperty("user.source.isonline", MyFilms.r[ItemId][dc.ColumnName].ToString());
                            //  else
                            //    setGUIProperty("user.source.isonline", "");
                            //}

                            // new method
                            if (wrep)
                            {
                              if (MyFilms.InitialIsOnlineScan) // if availability scanner did run - either by autostart or manually
                              {
                                if (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                                {
                                  if (MyFilms.r[ItemId][dc.ColumnName].ToString() == "True")
                                    setGUIProperty("user.source.isonline", "available");
                                  else if (MyFilms.r[ItemId][dc.ColumnName].ToString() == "False")
                                  {
                                    if (Helper.FieldIsSet(MyFilms.conf.StrStorage)) // if there is source field set
                                    {
                                      if (MyFilms.r[ItemId][MyFilms.conf.StrStorage].ToString().Length > 0) // if there is source info available ...
                                        setGUIProperty("user.source.isonline", "offline");
                                      else
                                        setGUIProperty("user.source.isonline", "unavailable");
                                    }
                                    else
                                      setGUIProperty("user.source.isonline", "unavailable");
                                  }
                                }
                                else
                                  setGUIProperty("user.source.isonline", "unknown"); // should not happen, if scanner did run ...
                              }
                              else
                              {
                                if (Helper.FieldIsSet(MyFilms.conf.StrStorage)) // if there is source field set
                                {
                                  if (MyFilms.r[ItemId][MyFilms.conf.StrStorage].ToString().Length > 0) // if there is source info available ...
                                  {
                                    if (MyFilms.conf.ScanMediaOnStart) // if availability scanner is supposed to autostart
                                      setGUIProperty("user.source.isonline", "unknown"); // set to unknown - will be replaced by real status after scan has finished...
                                    else
                                      setGUIProperty("user.source.isonline", "available"); // set online as default, even if it's possibly not available ...
                                  }
                                  else
                                    setGUIProperty("user.source.isonline", "unavailable");
                                }
                                else if (MyFilms.conf.SearchFile.ToLower() == "true" || MyFilms.conf.SearchFile.ToLower() == "yes") // if search is enabled in setup
                                  setGUIProperty("user.source.isonline", "unknown");
                                else
                                  setGUIProperty("user.source.isonline", "unavailable");
                              }
                            }
                            else
                              setGUIProperty("user.source.isonline", "");
                            break;
                        case "isonlinetrailer":
                            //if (wrep && MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                            //{
                            //  if (MyFilms.InitialIsOnlineScan)
                            //    setGUIProperty("user.sourcetrailer.isonline", MyFilms.r[ItemId][dc.ColumnName].ToString());
                            //  else
                            //    setGUIProperty("user.sourcetrailer.isonline", "");
                            //}
                            if (wrep)
                            {
                              if (MyFilms.InitialIsOnlineScan)
                              {
                                if (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                                {
                                  if (MyFilms.r[ItemId][dc.ColumnName].ToString() == "True")
                                    setGUIProperty("user.sourcetrailer.isonline", "available");
                                  else
                                  {
                                    if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
                                    {
                                      if (MyFilms.r[ItemId][MyFilms.conf.StrStorageTrailer].ToString().Length > 0)
                                        setGUIProperty("user.sourcetrailer.isonline", "offline");
                                      else
                                        setGUIProperty("user.sourcetrailer.isonline", "unavailable");
                                    }
                                    else
                                      setGUIProperty("user.sourcetrailer.isonline", "unavailable");
                                  }
                                }
                                else
                                  setGUIProperty("user.sourcetrailer.isonline", "unknown"); // should not happen, if scanner did run ...
                              }
                              else
                              {
                                if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
                                {
                                  if (MyFilms.r[ItemId][MyFilms.conf.StrStorageTrailer].ToString().Length > 0)
                                  {
                                    if (MyFilms.conf.ScanMediaOnStart)
                                      setGUIProperty("user.sourcetrailer.isonline", "unknown");
                                    else
                                      setGUIProperty("user.sourcetrailer.isonline", "available"); // set online as default
                                  }
                                  else
                                    setGUIProperty("user.sourcetrailer.isonline", "unavailable");
                                }
                                //else if (MyFilms.conf.SearchFileTrailer.ToLower() == "true" || MyFilms.conf.SearchFileTrailer.ToLower() == "yes") // if search is enabled in setup
                                //  setGUIProperty("user.sourcetrailer.isonline", "unknown");
                                else
                                  setGUIProperty("user.sourcetrailer.isonline", "unavailable");
                              }
                            }
                            else
                              setGUIProperty("user.sourcetrailer.isonline", "");
                            break;
                        case "resolution":
                            decimal aspectratio = 0; 
                            string ar = "";
                            if ((wrep) && (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                                try
                                    {
                                        wstring = MyFilms.r[ItemId][dc.ColumnName].ToString();
                                        //LogMyFilms.Debug("Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                                        setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", wstring);
                                        //LogMyFilms.Debug("(Load_Detailed_DB): Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                                        decimal w_hsize;
                                        decimal w_vsize;
                                        string[] arSplit;
                                        string[] Sep = new string[] { "x" };
                                        arSplit = MyFilms.r[ItemId][dc.ColumnName].ToString().Split(Sep, StringSplitOptions.RemoveEmptyEntries); // remove entries empty // StringSplitOptions.None);//will add "" entries also
                                        w_hsize = (decimal)Convert.ToInt32(arSplit[0]);
                                        w_vsize = (decimal)Convert.ToInt32(arSplit[1]);
                                        // To Check if/why exception eccurs
                                        //LogMyFilms.Debug("hsize - wsize: '" + w_hsize + " - " + w_vsize + "'");
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
                                        LogMyFilms.Info("Error calculating aspectratio !");
                                    }
                            setGUIProperty("db.calc.aspectratio.value", wstring);
                            setGUIProperty("db.calc.imageformat.value", ar);
                            break;
                        case "year":
                            if ((wrep) && (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                            {
                              int year = 0;
                              setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                              Int32.TryParse(MyFilms.r[ItemId][dc.ColumnName].ToString(), out year);
                              MyFilms.currentMovie.Year = year;
                            }
                              
                            else
                            {
                              setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", "");
                              MyFilms.currentMovie.Year = 0;
                            }
                            break;

                        default:
                            if ((wrep) && (MyFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", MyFilms.r[ItemId][dc.ColumnName].ToString());
                            else
                                setGUIProperty("db." + dc.ColumnName.ToLower() + ".value", "");
                            break;

                    }
                }
            }
            //// for catalog using "search" instead storage field
            //if (string.IsNullOrEmpty(MyFilms.currentMovie.File)) // use search method only if required...
            //{
            //  if ((MyFilms.conf.SearchFile == "True") || (MyFilms.conf.SearchFile == "yes"))
            //  {
            //    string movieName = MyFilms.r[ItemId][MyFilms.conf.ItemSearchFile].ToString();
            //    movieName = movieName.Substring(movieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
            //    if (MyFilms.conf.ItemSearchFile.Length > 0)
            //    {
            //      MyFilms.currentMovie.File = Search_FileName(movieName, MyFilms.conf.StrDirStor).Trim();
            //    }
            //  }
            //}
			
			    stopwatch.Stop();
			    LogMyFilms.Debug("Load_Detailed_DB() - load details finished (" + stopwatch.ElapsedMilliseconds + " ms).");
        }

        private static void Load_Detailed_DB_PushActorsToSkin(string actors)
        {
          for (int j = 1; j < 6; j++)
          {
              clearGUIProperty("db.actors.actor" + j + ".name");
              clearGUIProperty("db.actors.actor" + j + ".image");
          }

          ArrayList w_tableau = new ArrayList();
          w_tableau = MyFilms.Search_String(actors);
          int i = 1;
          foreach (object t in w_tableau)
          {
            string actorname = t.ToString();
            if (i < 6)
            {
              if (MyFilms.conf.UseThumbsForPersons && !string.IsNullOrEmpty(MyFilms.conf.StrPathArtist))
              {
                string personartworkpath = MyFilms.conf.StrPathArtist;
                if (System.IO.File.Exists(personartworkpath + "\\" + actorname + ".jpg"))
                {
                  setGUIProperty("db.actors.actor" + i + ".name", actorname);
                  setGUIProperty("db.actors.actor" + i + ".image", personartworkpath + "\\" + actorname + ".jpg");
                  i = i + 1;
                }
              }
              else
              {
                setGUIProperty("db.actors.actor" + i + ".name", actorname);
                i = i + 1;
              }
            }
          }
        }

        //-------------------------------------------------------------------------------------------
        //  Load detailed infos for persons: export fields to skin as '#myfilms.<ant db column name> 
        //-------------------------------------------------------------------------------------------
        public static void Load_Detailed_PersonInfo(string artistname, bool wrep)
        {
          Stopwatch stopwatch = new Stopwatch(); stopwatch.Reset();
          stopwatch.Start();  
          //clearGUIProperty("db.description.value");
          LogMyFilms.Debug("Load_Detailed_PersonInfo for '" + artistname + "'");
          ArrayList actorList = new ArrayList();

          VideoDatabase.GetActorByName(artistname, actorList);
          //MyFilmsDetail.GetActorByName(artistname, actorList);
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
              LogMyFilms.Debug("load details for actor: '" + actorID.ToString() + "'");
              try
              {
                IMDBActor actor = VideoDatabase.GetActorInfo(actorID);
                if (actor.Biography.Length > 0) 
                  setGUIProperty("db.description.value", actor.Biography);
                else if (actor.MiniBiography.Length > 0) 
                  setGUIProperty("db.description.value", actor.MiniBiography);
                else
                  setGUIProperty("db.description.value", "");
                if (actor.Name.Length > 0)
                {
                  setGUIProperty("user.mastertitle.value", actor.Name);
                  setGUIProperty("user.secondarytitle.value", actor.Name);
                }
                else
                {
                  setGUIProperty("user.mastertitle.value", "");
                  setGUIProperty("user.secondarytitle.value", "");
                }

                if (actor.PlaceOfBirth.Length > 0) 
                  setGUIProperty("db.category.value", actor.PlaceOfBirth);
                else
                  setGUIProperty("db.category.value", "");
                if (actor.DateOfBirth.Length > 0) 
                  setGUIProperty("db.year.value", actor.DateOfBirth);
                else
                  setGUIProperty("db.year.value", "");
              }
              catch (Exception ex)
              {
                LogMyFilms.Debug("Exception while loading person details: " + ex.Message);
              }
            }
          }
          stopwatch.Stop();
          LogMyFilms.Debug("Load_Detailed_PersonInfo() - load details finished (" + stopwatch.ElapsedMilliseconds + " ms).");
        }

        #endregion

        private void InitTrailerwatcher(string directorypath)
        {
          if (!System.IO.Directory.Exists(directorypath))
          {
            LogMyFilms.Warn("InitTrailerwatcher() - Trailerwatcher cannot initialize - path does not exist: '" + directorypath + "'");
            return;
          }
          if (Trailerwatcher.EnableRaisingEvents && Trailerwatcher.Path == directorypath && directorypath != "")
            return; // return, if it's already enabled and file name has not changed
          else
          {
            Trailerwatcher.EnableRaisingEvents = false;
            Trailerwatcher.Changed -= new FileSystemEventHandler(TrailerwatcherChanged);
            Trailerwatcher.Error -= new ErrorEventHandler(TrailerwatcherError);
            Trailerwatcher.Created -= new FileSystemEventHandler(TrailerwatcherCreated);
            Trailerwatcher.Deleted -= new FileSystemEventHandler(TrailerwatcherDeleted);
            Trailerwatcher.Renamed -= new RenamedEventHandler(TrailerwatcherRenamed);
            if (directorypath == "") return;
          }

          // Init FileSystem Watcher
          // ***** Change this as required
          //string path = System.IO.Path.GetDirectoryName(conf.StrFileXml);
          //string filename = System.IO.Path.GetFileName(conf.StrFileXml);
          Trailerwatcher.Path = directorypath;
          Trailerwatcher.IncludeSubdirectories = true;
          Trailerwatcher.Filter = "*.*"; // Trailerwatcher.Filter = "*.xml";
          Trailerwatcher.NotifyFilter = NotifyFilters.LastWrite; // Trailerwatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
          //Trailerwatcher.InternalBufferSize = 64;

          // Add event handlers: 1 for the event raised when a file is created, and 1 for when it detects an error.
          Trailerwatcher.Changed += new FileSystemEventHandler(TrailerwatcherChanged);
          Trailerwatcher.Error += new ErrorEventHandler(TrailerwatcherError);
          //Trailerwatcher.Created += new FileSystemEventHandler(TrailerwatcherCreated);
          //Trailerwatcher.Deleted += new FileSystemEventHandler(TrailerwatcherDeleted);
          //Trailerwatcher.Renamed += new RenamedEventHandler(TrailerwatcherRenamed);

          Trailerwatcher.EnableRaisingEvents = true; // Begin watching.
          LogMyFilms.Debug("InitTrailerwatcher() - Trailerwatcher started watching - directory: '" + directorypath + "'");
        }

        private void TrailerwatcherChanged(object source, FileSystemEventArgs e)
        {
          LogMyFilms.Debug("WatcherChanged() - New Trailerwatcher Event: " + e.ChangeType + ": '" + e.FullPath + "'");

          if (Trailerwatcher.EnableRaisingEvents == false) // ignore event, if notification is switched off
            return;

          Thread.Sleep(250);
          FileInfo objFileInfo = new FileInfo(e.FullPath);
          if (!objFileInfo.Exists) return; // ignore the file changed event
          
          // Trailerwatcher.EnableRaisingEvents = false;
          this.InitTrailerwatcher(""); // reset trailer watcher
          AutoRegisterTrailer(e.FullPath);

          //// this.BeginInvoke(new UpdateWatchTextDelegate(UpdateWatchText), "WatcherChanged() - New FSwatcher Event: " + e.ChangeType + ": '" + e.FullPath + "'");
          //Trailerwatcher.EnableRaisingEvents = true;
          //if (ImportComplete != null && MyFilms.conf.AllowTraktSync) // trigger sync to trakt page after importer finished
          //{
          //  ImportComplete();
          //  LogMyFilms.Debug("FSwatcherChanged(): Fired 'ImportCompleted' event to trigger sync to trakt page after reloading database content !");
          //}
        }

        private void TrailerwatcherCreated(object source, FileSystemEventArgs e)
        {
          LogMyFilms.Debug("WatcherCreated() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
        }

        private void TrailerwatcherDeleted(object source, FileSystemEventArgs e)
        {
          LogMyFilms.Debug("WatcherDeleted() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
        }

        private void TrailerwatcherRenamed(object source, FileSystemEventArgs e)
        {
          LogMyFilms.Debug("WatcherRenamed() - New FSwatcher Event: '" + e.ChangeType + "', Name: '" + e.Name + "', Path: '" + e.FullPath + "'");
        }

        private void TrailerwatcherError(object source, ErrorEventArgs e) // The error event handler
        {
          Exception watchException = e.GetException();
          LogMyFilms.Debug("WatcherError() - A FileSystemWatcher error has occurred: " + watchException.Message);
          // We need to create new version of the object because the old one is now corrupted
          Trailerwatcher = new FileSystemWatcher();
          while (!Trailerwatcher.EnableRaisingEvents)
          {
            try
            {
              InitTrailerwatcher(""); // This will throw an error at the watcher.NotifyFilter line if it can't get the path.
              LogMyFilms.Debug("WatcherError() - Trailerwatcher restarted after error !");
            }
            catch
            {
              System.Threading.Thread.Sleep(5000); // Sleep for a bit; otherwise, it takes a bit of processor time
            }
          }
        }

        #region  Lecture du film demandé

        public static void PlayMovie(string config, MFMovie movie)
        {
          
        }

        public static void PlayMovie(string config, int movieid)
        //-------------------------------------------------------------------------------------------
        // Play Movie from external (wrapper to prepare environment)
        //-------------------------------------------------------------------------------------------
        {
          int GetID = GUIWindowManager.GetPreviousActiveWindow();
          try
          {
            Launch_Movie(movieid, GetID, null, false);
          }
          catch (Exception ex)
          {
            LogMyFilms.DebugException("PlayMovie() : Exception !", ex);
            // throw;
          }
        }

        public static void Launch_Movie(int select_item, int GetID, GUIAnimation m_SearchAnimation, bool bForceExternalPlayback)
        //-------------------------------------------------------------------------------------------
        // Play Movie
        //-------------------------------------------------------------------------------------------
        {
          LogMyFilms.Debug("Launch_Movie() select_item = '" + select_item + "' - GetID = '" + GetID + "' - m_SearchAnimation = '" + m_SearchAnimation + "', forceExternalPlayer = '" + bForceExternalPlayback + "'");
          //enableNativeAutoplay(); // in case, other plugin disabled it - removed, as we now do start external player ourselves ...

          #region Version Select Dialog
          string filestorage = MyFilms.r[select_item][MyFilms.conf.StrStorage].ToString();
          if (Helper.FieldIsSet(MyFilms.conf.StrStorage))
          {
            Regex filmver = new Regex(@"\[\[([^\#]*)##([^\]]*)\]\]");
            MatchCollection filmverMatches = filmver.Matches(filestorage);
            if (filmverMatches.Count > 0)
            {
              GUIDialogMenu versionmenu =
                (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
              versionmenu.Reset();
              versionmenu.SetHeading("Select Version");
              List<string> filestr = new List<string>();
              for (int i = 0; i < filmverMatches.Count; i++)
              {
                versionmenu.Add(filmverMatches[i].Groups[2].Value);
                filestr.Add(filmverMatches[i].Groups[1].Value);
              }
              versionmenu.DoModal(GetID);
              if (versionmenu.SelectedLabel == -1) return;
              filestorage = filestr[versionmenu.SelectedLabel];
            }
          }

          #endregion

          #region handle WOL
          // Guzzi: Added WOL to start remote host before playing the files
          // Wake up the TV server, if required
          // HandleWakeUpNas();
          // LogMyFilms.Info("Launched HandleWakeUpNas() to start movie'" + MyFilms.r[select_item][MyFilms.conf.StrSTitle.ToString()] + "'");

          if (MyFilms.conf.StrCheckWOLenable)
          {
            WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();
            int wTimeout = MyFilms.conf.StrWOLtimeout;
            bool isActive;
            string UNCpath = filestorage;
            string NasServerName;
            string NasMACAddress;

            if (UNCpath.StartsWith("\\\\")) UNCpath = (UNCpath.Substring(2, UNCpath.Substring(2).IndexOf("\\") + 0)).ToLower();
            if ((UNCpath.Equals(MyFilms.conf.StrNasName1, StringComparison.InvariantCultureIgnoreCase)) ||
                (UNCpath.Equals(MyFilms.conf.StrNasName2, StringComparison.InvariantCultureIgnoreCase)) ||
                (UNCpath.Equals(MyFilms.conf.StrNasName3, StringComparison.InvariantCultureIgnoreCase)))
            {
              if (WakeOnLanManager.Ping(UNCpath, wTimeout)) isActive = true;
              else isActive = false;

              if (!isActive) // Only if NAS server is not yet already rzunning !
              {
                if (MyFilms.conf.StrCheckWOLuserdialog)
                {
                  GUIDialogYesNo dlgOknas =
                    (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                  dlgOknas.SetHeading(GUILocalizeStrings.Get(107986)); //my films
                  dlgOknas.SetLine(1, "Film    : '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle.ToString()] + "'");
                    //video title
                  dlgOknas.SetLine(2, "Server  : '" + UNCpath + "'");
                  dlgOknas.SetLine(3, "Status  : '" + GUILocalizeStrings.Get(10798742));
                    // srv name + " - (offline) - start ?"
                  dlgOknas.DoModal(GetID);
                  if (!(dlgOknas.IsConfirmed)) return;
                }

                // Search the NAS where movie is located:
                if ((UNCpath.Equals(MyFilms.conf.StrNasName1, StringComparison.InvariantCultureIgnoreCase)) &&
                    (MyFilms.conf.StrNasMAC1.ToString().Length > 1))
                {
                  NasServerName = MyFilms.conf.StrNasName1;
                  NasMACAddress = MyFilms.conf.StrNasMAC1;
                }
                else if ((UNCpath.Equals(MyFilms.conf.StrNasName2, StringComparison.InvariantCultureIgnoreCase)) &&
                         (MyFilms.conf.StrNasMAC2.ToString().Length > 1))
                {
                  NasServerName = MyFilms.conf.StrNasName2;
                  NasMACAddress = MyFilms.conf.StrNasMAC2;
                }
                else if ((UNCpath.Equals(MyFilms.conf.StrNasName3, StringComparison.InvariantCultureIgnoreCase)) &&
                         (MyFilms.conf.StrNasMAC3.ToString().Length > 1))
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

                bool SuccessFulStart = wakeOnLanManager.WakeupSystem(
                  wakeOnLanManager.GetHwAddrBytes(NasMACAddress), NasServerName, wTimeout);

                if (MyFilms.conf.StrCheckWOLuserdialog)
                {
                  GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                  dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));
                  dlgOk.SetLine(1, "");
                  if (SuccessFulStart)
                    dlgOk.SetLine(2, "'" + NasServerName + "' " + GUILocalizeStrings.Get(10798743));
                      //successfully started 
                  else
                    dlgOk.SetLine(2, "'" + NasServerName + "' " + GUILocalizeStrings.Get(10798744));
                      // could not be started 
                  dlgOk.DoModal(GetID);
                }
              }
            }
            else
            {
              GUIDialogOK dlgOknas = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
              dlgOknas.SetHeading(GUILocalizeStrings.Get(107986)); //my films
              //dlgOknas.SetLine(1, "Movie   : '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle.ToString()].ToString() + "'"); //video title
              dlgOknas.SetLine(2, "Server '" + UNCpath + "' " + GUILocalizeStrings.Get(10798746));
                //is not configured for WakeOnLan ! 
              dlgOknas.SetLine(3, GUILocalizeStrings.Get(10798747)); // Automatic NAS start not possible ... 
              dlgOknas.DoModal(GetID);
              return;
            }
          }

          #endregion

          setProcessAnimationStatus(true, m_SearchAnimation);
          #region Run externaly Program before Playing if defined in setup
          if (Helper.FieldIsSet(MyFilms.conf.CmdPar)) 
            RunProgram(MyFilms.conf.CmdExe, MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.CmdPar].ToString());
          #endregion

          if (g_Player.Playing) g_Player.Stop();

          // search all files
          ArrayList newItems = new ArrayList();
          bool NoResumeMovie = true;
          int IMovieIndex = 0;

          //Guzzi: Added BoolType for Trailerlaunch
          Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex, false, filestorage);
          //Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex);
          if (newItems.Count > 20) // Maximum 20 entries (limitation for MP dialogFileStacking)
          {
            GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            dlgOk.SetHeading(GUILocalizeStrings.Get(107986)); //my films
            dlgOk.SetLine(1, MyFilms.r[select_item][MyFilms.conf.StrSTitle].ToString()); //video title
            dlgOk.SetLine(2, "maximum 20 entries for the playlist");
            dlgOk.DoModal(GetID);
            LogMyFilms.Info(
              "Too many entries found for movie '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle] +
              "', number of entries found = " + newItems.Count);
            return;
          }
          setProcessAnimationStatus(false, m_SearchAnimation);

          #region optional part selection dialog
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
          #endregion

          if (newItems.Count > 0)
          {
            // Check, if the content returned is a BR playlist to supress internal player and dialogs
            bool isBRcontent = false;
            string mediapath = filestorage;
            LogMyFilms.Info("Launch_Movie() - SingleItem found ('" + newItems[0] + "'), filestorage = '" + filestorage + "'");
            if (newItems[0].ToString().ToLower().EndsWith("bdmv")) isBRcontent = true;

            if ((!isBRcontent || Helper.IsBDHandlerAvailableAndEnabled) && !bForceExternalPlayback)
            {
              #region internal playback
              LogMyFilms.Info("Launch_Movie() - start internal playback");
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

              // Set Playbackhandler to active
              MyFilms.conf.MyFilmsPlaybackActive = true;
              // play movie...
              PlayMovieFromPlayList(NoResumeMovie, IMovieIndex - 1);
              #endregion
            }
            else if (MyFilms.conf.ExternalPlayerPath.Length > 0)
            {
              #region external player playback (myfilms)
              LogMyFilms.Info("Launch_Movie() - start external player - path = '" + MyFilms.conf.ExternalPlayerPath + "', argument (filestorage) = '" + filestorage + "'");

              if (bForceExternalPlayback)
              {
                try
                {
                  LaunchExternalPlayer(filestorage);
                  return;
                }
                catch (Exception ex) { LogMyFilms.Info("Launch_Movie() - calling external player ended with exception: " + ex); }
              }
              else
              {
                string[] split = MyFilms.conf.ExternalPlayerExtensions.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split)
                {
                  if (filestorage.ToLower().Contains(s.ToLower())) // if extension is defined to use external player
                  {
                    try
                    {
                      LaunchExternalPlayer(filestorage);
                      return;
                    }
                    catch (Exception ex) { LogMyFilms.Info("Launch_Movie() - calling external player ended with exception: " + ex); }
                  }
                }
              }
              #endregion
            }
            else if (Helper.IsBluRayPlayerLauncherAvailableAndEnabled)
            {
              LogMyFilms.Info("Launch_Movie() - activate blurayplayer plugin");
              GUIWindowManager.ActivateWindow(MyFilms.ID_BluRayPlayerLauncher);
            }
          }
          else
          {
            //if (first)
            //// ask for mounting file first time
            //{
            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986)); //my films
            dlgYesNo.SetLine(1, GUILocalizeStrings.Get(219)); //no disc
            if (Helper.FieldIsSet(MyFilms.conf.StrIdentItem))
              if (MyFilms.conf.StrIdentLabel.Length > 0)
                dlgYesNo.SetLine(2, MyFilms.conf.StrIdentLabel + " = " + MyFilms.r[select_item][MyFilms.conf.StrIdentItem]); //Label Identification for Media
              else
                dlgYesNo.SetLine(2, "'" + MyFilms.conf.StrIdentItem + "' = " + MyFilms.r[select_item][MyFilms.conf.StrIdentItem]); //ANT Item Identification for Media
            else 
              dlgYesNo.SetLine(2, "' disc n° = " + MyFilms.r[select_item]["Number"]); //ANT Number for Identification Media 
            dlgYesNo.DoModal(GetID);
            if (dlgYesNo.IsConfirmed) Launch_Movie(select_item, GetID, m_SearchAnimation, bForceExternalPlayback);
              //}
            else
            {
              GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
              dlgOk.SetHeading(GUILocalizeStrings.Get(107986)); //my films
              dlgOk.SetLine(1, GUILocalizeStrings.Get(1036)); //no video found
              dlgOk.SetLine(2, MyFilms.r[select_item][MyFilms.conf.StrSTitle].ToString());
              dlgOk.DoModal(GetID);
              LogMyFilms.Info("File not found for movie '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle]);
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
          LogMyFilms.Debug("(Play Movie Trailer) select_item = '" + select_item + "' - GetID = '" + GetID + "' - m_SearchAnimation = '" + m_SearchAnimation + "'");
          if (Helper.FieldIsSet(MyFilms.conf.CmdPar))
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
            LogMyFilms.Info("Too many entries found for movie '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle] + "', number of entries found = " + newItems.Count.ToString());
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
              LogMyFilms.Info("Play specific movie trailer: '" + movieFileName + "'");
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
                LogMyFilms.Info("Add trailer to playlist: '" + movieFileName + "'");
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
            LogMyFilms.Info("File not found for movie '" + MyFilms.r[select_item][MyFilms.conf.StrSTitle] + "'");
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
              string path = MyFilms.r[MyFilms.conf.StrIndex][MyFilms.conf.StrStorage].ToString();
              if (path.Contains(";"))
                path = path.Substring(0, path.IndexOf(";"));
              if (path.Contains("\\")) 
                path = path.Substring(0, path.LastIndexOf("\\"));

              switch (choiceViewMenu[dlgmenu.SelectedLabel].ToLower())
              {
                case "playtraileronlinevideos":
                  site = "YouTube";
                  titleextension = " " + MyFilms.r[MyFilms.conf.StrIndex]["Year"] + " trailer" + ((MyFilms.conf.GrabberOverrideLanguage.Length > 0) ? (" " + MyFilms.conf.GrabberOverrideLanguage) : "");
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
              if (Helper.IsOnlineVideosAvailableAndEnabled)
              {
                string title = GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
                string OVstartparams = "site:" + site + "|category:|search:" + title + titleextension + "|return:Locked" + "|downloaddir:" + path + "|downloadmenuentry:" + GUILocalizeStrings.Get(10798749) + " (" + title + ")";
                //GUIPropertyManager.SetProperty("Onlinevideos.startparams", OVstartparams);
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", site);
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", title + titleextension);
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "Locked");
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloaddir", path);
                //GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadfilename", "");
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadmenuentry", GUILocalizeStrings.Get(10798749) + " (" + title + ")"); // download to movie directory
                LogMyFilms.Debug("Starting OnlineVideos with '" + OVstartparams.ToString() + "'");
                // should this be set here to make original movie doesn't get set to watched??
                // trailerPlayed = true;
#if MP11
                  GUIWindowManager.ActivateWindow(MyFilms.ID_OnlineVideos, false); // 4755 is ID for OnlineVideos
#else
                GUIWindowManager.ActivateWindow((int)MyFilms.ExternalPluginWindows.OnlineVideos, OVstartparams);
#endif
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Site", "");
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Category", "");
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Search", "");
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.Return", "");
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloaddir", "");
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadfilename", "");
                GUIPropertyManager.SetProperty("#OnlineVideos.startparams.downloadmenuentry", "");
              }
              else
              {
                // ShowMessageDialog("MyFilms", "OnlineVideo plugin not installed or wrong version", "Minimum Version required: " + MyFilmsSettings.GetRequiredMinimumVersion(MyFilmsSettings.MinimumVersion.OnlineVideos));

                GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                if (dlgOK != null)
                {
                  dlgOK.SetHeading("MyFilms");
                  dlgOK.SetLine(1, "OnlineVideo plugin not installed or wrong version");
                  dlgOK.SetLine(2, "Minimum Version required: " + MyFilmsSettings.GetRequiredMinimumVersion(MyFilmsSettings.MinimumVersion.OnlineVideos));
                  dlgOK.DoModal(GetID);
                  return;
                }
              }
            }
          }
        }

        public static void Launch_Movie_Trailer_Scrobbling(ArrayList newItems, int GetID) // newitems contains filenames ...
        //-------------------------------------------------------------------------------------------
        // Play Movie Trailers for Scrobbling !!!
        //-------------------------------------------------------------------------------------------
        {
          // setProcessAnimationStatus(true, m_SearchAnimation);
          //LogMyFilms.Debug("(Play Movie Trailer) select_item = '" + select_item + "' - GetID = '" + GetID + "'");
          if (g_Player.Playing)
            g_Player.Stop();
          bool NoResumeMovie = true; //Modded by Guzzi for NonResuming Trailers 

          if (newItems.Count > 20)
          // Maximum 20 entries (limitation for MP dialogFileStacking)
          {
            LogMyFilms.Info("Too many entries found !', number of entries = " + newItems.Count.ToString());
            return;
          }
          // setProcessAnimationStatus(false, m_SearchAnimation);
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
              LogMyFilms.Info("Add trailer to playlist: '" + movieFileName + "'");
              newitem.Type = PlayListItem.PlayListItemType.Video;
              playlist.Add(newitem);
            }
            PlayMovieFromPlayListTrailer(NoResumeMovie, 0);
          }
        }

        public static void Launch_Movie_Trailer_Streams(int select_item, int GetID, GUIAnimation m_SearchAnimation)
        //-------------------------------------------------------------------------------------------
        // Play Movie Trailer Links (streaming) !!!
        //-------------------------------------------------------------------------------------------
        {  // ToDo: Add methods for Streaming - check OV for howto's...
          LogMyFilms.Debug("(Play Movie Trailer Streams) select_item = '" + select_item + "' - GetID = '" + GetID + "' - m_SearchAnimation = '" + m_SearchAnimation + "'");
          if (Helper.FieldIsSet(MyFilms.conf.CmdPar))
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
              LogMyFilms.Info("Play specific movie trailer: '" + movieFileName + "'");
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
                LogMyFilms.Info("Add trailer to playlist: '" + movieFileName + "'");
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
            // update the MP Video Database for OSD view during playing
            update_database(fileName, select_item, -1);
            newItems.Add(fileName);
        }

        bool PlayBackEventIsOfConcern(g_Player.MediaType type, string filename)
        {
          bool playbackeventIsOfConcern = false;
          if (string.IsNullOrEmpty(filename)) return false;
          if ((MyFilms.currentMovie != null && type == g_Player.MediaType.Video && (MyFilms.currentMovie.File.Contains(filename) || MyFilms.conf.MyFilmsPlaybackActive)))
            playbackeventIsOfConcern = true;
          return playbackeventIsOfConcern;
        }

        bool PlayBackEventIsOfConcernAsTrailer(g_Player.MediaType type, string filename)
        {
          bool playbackeventIsOfConcern = false;
          if (string.IsNullOrEmpty(filename)) return false;
          if (MyFilms.currentTrailerPlayingItem != null && type == g_Player.MediaType.Video && MyFilms.currentTrailerPlayingItem.Trailer.Contains(filename)) // if (MyFilms.currentMovie != null && type == g_Player.MediaType.Video && MyFilms.currentMovie.Trailer.Contains(filename)) // 
            playbackeventIsOfConcern = true;
          return playbackeventIsOfConcern;
        }

        private void OnPlayBackStarted(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            if (!PlayBackEventIsOfConcern(type, filename))
            {
              if (PlayBackEventIsOfConcernAsTrailer(type, filename))
              {
                LogMyFilms.Debug("OnPlayBackEnded was initiated, identified that MyFilms Trailer was played - filename: '" + filename + "'");
                GUIWindowManager.OnNewAction -= new OnActionHandler(GUIWindowManager_OnNewAction); // make sure it doesn't register twice ....
                GUIWindowManager.OnNewAction += new OnActionHandler(GUIWindowManager_OnNewAction);
              }
              else
                LogMyFilms.Debug("OnPlayBackStarted was initiated, but has no relevant event data for MyFilms - filename: '" + filename + "'");
              return;
            }
            LogMyFilms.Debug("OnPlayBackStarted was initiated - filename: '" + filename + "'");

            // indicate to skin, that it is MyFilms playing a file
            setGUIProperty("isplaying", "true", true);
            // attach to global action event, to handle remote keys during playback - e.g. trailer previews
            GUIWindowManager.OnNewAction -= new OnActionHandler(GUIWindowManager_OnNewAction); // make sure it doesn't register twice ....
            GUIWindowManager.OnNewAction += new OnActionHandler(GUIWindowManager_OnNewAction);

            // tell any listeners that user started the movie
            MFMovie movie = new MFMovie();
            movie = GetMovieFromRecord(MyFilms.r[MyFilms.conf.StrIndex]);
            if (MovieStarted != null && MyFilms.conf.AllowTraktSync)
            {
              MovieStarted(movie);
              LogMyFilms.Debug("OnPlayBackStarted(): Fired 'MovieStarted' event with movie = '" + movie.Title + "'");
            }

            string otitle = MyFilms.r[MyFilms.conf.StrIndex]["OriginalTitle"].ToString();
            string ttitle = MyFilms.r[MyFilms.conf.StrIndex]["TranslatedTitle"].ToString();
            LogMyFilms.Debug("OnPlayBackStarted() was initiated - MyFilms.conf.StrIndex = '" + MyFilms.conf.StrIndex + "', count = '" + MyFilms.r.Length + "', otitle = '" + otitle + "', ttitle = '" + ttitle + "'");

            // store informations for action at endplayback if any - Filmnlist filter is: r = BaseMesFilms.ReadDataMovies(conf.GlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, conf.StrFilmSelect, sortfield, sortascending, false); 

            MyFilms.conf.StrPlayedMovie = movie; // store movie object
            MyFilms.conf.StrPlayedIndex = MyFilms.conf.StrIndex;
            MyFilms.conf.StrPlayedGlobalFilterString = MyFilms.conf.StrGlobalFilterString;
            MyFilms.conf.StrPlayedViewSelect = MyFilms.conf.StrViewSelect;
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
          if (!PlayBackEventIsOfConcern(type, filename))
          {
            if (PlayBackEventIsOfConcernAsTrailer(type, filename))
            {
              LogMyFilms.Debug("OnPlayBackEnded was initiated, identified that MyFilms Trailer was played - filename: '" + filename + "'");
              if (TrailerEnded != null)
              {
                TrailerEnded(filename); // LogMyFilms.Debug("OnPlayBackEnded(): Fired 'TrailerEnded' event with filename = '" + filename + "'");
              }
            }
            else LogMyFilms.Debug("OnPlayBackEnded was initiated, but has no relevant movie event data for MyFilms - filename: '" + filename + "'");
            return;
          }
          LogMyFilms.Debug("OnPlayBackEnded was initiated - filename: '" + filename + "'");
          UpdateOnPlayEnd(type, 0, filename, true, false);
        }

        private void OnPlayBackStopped(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename)
        {
          if (!PlayBackEventIsOfConcern(type, filename))
          {
            LogMyFilms.Debug("OnPlayBackStopped was initiated, but has no relevant movie event data for MyFilms - filename: '" + filename + "'");
            MyFilms.trailerscrobbleactive = false;
            return;
          }
          LogMyFilms.Debug("OnPlayBackStopped was initiated - filename: '" + filename + "'");
          UpdateOnPlayEnd(type, timeMovieStopped, filename, false, true);
        }

        private void UpdateOnPlayEnd(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename, bool ended, bool stopped)
        {
            LogMyFilms.Debug("UpdateOnPlayEnd() was initiated - trailerPlayed = '" + trailerPlayed + "', filename: '" + filename + "', StrPlayedIndex: '" + MyFilms.conf.StrPlayedIndex + "'");

            // indicate to skin, that MyFilms isn't playing anymore
            setGUIProperty("isplaying", "false", true);
            MyFilms.conf.MyFilmsPlaybackActive = false;  
            // detach from global action event, to handle remote keys during playback - e.g. trailer previews
            try
            {
              GUIWindowManager.OnNewAction -= new OnActionHandler(this.GUIWindowManager_OnNewAction);
            }
            catch (Exception) { }  

            if (MyFilms.conf.StrPlayedIndex == -1)
                return;
            if (type != g_Player.MediaType.Video || filename.EndsWith("&txe=.wmv"))
              return;
            if (handleTrailer())
              return;

            //if (isTrailer)
            //{
            //  LogMyFilms.Debug("Skipping UpdateOnEnd - reason: isTrailer");
            //  isTrailer = false;
            //  return;
            //}
            try
            {
                //  r = BaseMesFilms.ReadDataMovies(conf.GlobalFilterString + conf.StrViewSelect + conf.StrDfltSelect, conf.StrFilmSelect, sortfield, sortascending, false); 
                DataRow[] r1 = BaseMesFilms.ReadDataMovies(MyFilms.conf.StrPlayedGlobalFilterString + MyFilms.conf.StrPlayedViewSelect + MyFilms.conf.StrPlayedDfltSelect, MyFilms.conf.StrPlayedSelect, MyFilms.conf.StrPlayedSort, MyFilms.conf.StrPlayedSens);
                string otitle = r1[MyFilms.conf.StrPlayedIndex]["OriginalTitle"].ToString();
                string ttitle = r1[MyFilms.conf.StrPlayedIndex]["TranslatedTitle"].ToString();
                LogMyFilms.Debug("UpdateOnPlayEnd() was initiated - MyFilms.conf.StrPlayedIndex = '" + MyFilms.conf.StrPlayedIndex + "', count = '" + r1.Length + "', otitle = '" + otitle + "', ttitle = '" + ttitle + "'");
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
                    //    LogMyFilms.Debug("Partial Movie Runtime = '" + movie.RunTime.ToString() + "'");
                    //    if (movie.RunTime > 0)
                    //      TotalRuntimeMovie += movie.RunTime;
                    //  }
                    //  LogMyFilms.Debug("TotalRuntimeMovie = '" + TotalRuntimeMovie.ToString() + "'");
                    //}

                    if (g_Player.Player.Duration >= 1)
                    {
                      LogMyFilms.Debug("TotalRuntimeMovie = '" + TotalRuntimeMovie.ToString() + "', g_player.Player.Duration = '" + g_Player.Player.Duration.ToString() + "'");
                      string runtimeFromDb = r1[MyFilms.conf.StrPlayedIndex]["Length"].ToString();
                      try
                      {
                        TotalRuntimeMovie = 60 * double.Parse(runtimeFromDb);
                      }
                      catch (Exception)
                      {
                        TotalRuntimeMovie = 0;
                      }
                      LogMyFilms.Debug("TotalRuntimeMovie = '" + TotalRuntimeMovie.ToString() + "', Runtime from DB = '" + runtimeFromDb + "'");

                      if (TotalRuntimeMovie > g_Player.Player.Duration)
                        playTimePercentage = (int)Math.Ceiling((timeMovieStopped / TotalRuntimeMovie) * 100);
                      else
                        playTimePercentage = (int)Math.Ceiling((timeMovieStopped / g_Player.Player.Duration) * 100);
                      LogMyFilms.Debug("Calculated playtimepercentage: '" + playTimePercentage + "' - g_player.Duration: '" + g_Player.Duration.ToString() + "' - playlistPlayer.g_Player.Duration: '" + playlistPlayer.g_Player.Duration.ToString() + "'");
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
                            LogMyFilms.Info("GUIVideoFiles: OnPlayBackEnded idFile={0} resumeData={1}", idFile, resumeData);
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
                                LogMyFilms.Info("GUIVideoFiles: OnPlayBackStopped idFile={0} timeMovieStopped={1} resumeData={2}", idFile, timeMovieStopped, resumeData);
                                VideoDatabase.SetMovieStopTimeAndResumeData(idFile, timeMovieStopped, resumeData);
                                LogMyFilms.Debug("GUIVideoFiles: OnPlayBackStopped store resume time");
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
                  LogMyFilms.Debug("Skipping UpdateOnEnd - reason: trailerPlayed = true");
                  trailerPlayed = false;
                  return;
                }
                
                if (MyFilms.conf.CheckWatched)
                {
                  if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
                  {
                    // int currentCount = GetWatchedCount(MyFilms.conf.StrPlayedIndex, MyFilms.conf.StrUserProfileName);
                    AddWatchedCount(MyFilms.conf.StrPlayedIndex, MyFilms.conf.StrUserProfileName);
                  }
                  else
                  {
                    r1[MyFilms.conf.StrPlayedIndex][MyFilms.conf.StrWatchedField] = "True";
                  }
                  LogMyFilms.Debug("Movie set to watched - reason: ended = " + ended + ", stopped = " + stopped + ", playTimePercentage = '" + playTimePercentage + "'" + ", 'update on movie start'");
                }
                if (ended)
                {
                  if (MyFilms.conf.StrSupPlayer)
                    Suppress_Entry(r1, MyFilms.conf.StrPlayedIndex);
                }
                if (ended && MyFilms.conf.CheckWatchedPlayerStopped)
                {
                  if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
                  {
                    // int currentCount = GetWatchedCount(MyFilms.conf.StrPlayedIndex, MyFilms.conf.StrUserProfileName);
                    AddWatchedCount(MyFilms.conf.StrPlayedIndex, MyFilms.conf.StrUserProfileName);
                  }
                  else
                  {
                    r1[MyFilms.conf.StrPlayedIndex][MyFilms.conf.StrWatchedField] = "True";
                  }
                  LogMyFilms.Debug("Movie set to watched - reason: ended = " + ended + ", stopped = " + stopped + ", playTimePercentage = '" + playTimePercentage + "'" + ", 'update on movie end'");
                }
                if (stopped && MyFilms.conf.CheckWatchedPlayerStopped && playTimePercentage >= 80)
                {
                  if (MyFilms.conf.StrEnhancedWatchedStatusHandling)
                  {
                    // int currentCount = GetWatchedCount(MyFilms.conf.StrPlayedIndex, MyFilms.conf.StrUserProfileName);
                    AddWatchedCount(MyFilms.conf.StrPlayedIndex, MyFilms.conf.StrUserProfileName);
                  }
                  else
                  {
                    r1[MyFilms.conf.StrPlayedIndex][MyFilms.conf.StrWatchedField] = "True";
                  }
                  LogMyFilms.Debug("Movie set to watched - reason: ended = " + ended + ", stopped = " + stopped + ", playTimePercentage = '" + playTimePercentage + "'" + ", 'update on movie end'");
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
                if (ended || (stopped && playTimePercentage >= 80))
                {
                  // tell any listeners that movie is watched
                  MFMovie movie = MyFilms.conf.StrPlayedMovie;
                  if (MovieWatched != null && MyFilms.conf.AllowTraktSync)
                  {
                    MovieWatched(movie);
                    LogMyFilms.Debug("UpdateOnPlayEnd(): Fired 'MovieWatched' event with movie = '" + movie.Title + "'");
                  }
                }
                else
                {
                  // tell any listeners that movie is watched
                  MFMovie movie = MyFilms.conf.StrPlayedMovie;
                  if (MovieStopped != null && MyFilms.conf.AllowTraktSync)
                  {
                    MovieStopped(movie);
                    LogMyFilms.Debug("UpdateOnPlayEnd(): Fired 'MovieStopped' event with movie = '" + movie.Title + "'");
                  }
                }

                MyFilms.conf.StrPlayedMovie = null;
                MyFilms.conf.StrPlayedIndex = -1;
                MyFilms.conf.StrPlayedGlobalFilterString = string.Empty;
                MyFilms.conf.StrPlayedViewSelect = string.Empty;
                MyFilms.conf.StrPlayedDfltSelect = string.Empty;
                MyFilms.conf.StrPlayedSelect = string.Empty;
                MyFilms.conf.StrPlayedSort = string.Empty;
                MyFilms.conf.StrPlayedSens = string.Empty;
            }
            catch (Exception ex)
            {
                LogMyFilms.Info("Error during PlayBackEnded - exception: " + ex.Message + ", stacktrace: " + ex.StackTrace.ToString());
            }
        }

        private void GUIWindowManager_OnNewAction(MediaPortal.GUI.Library.Action action)
        {
          LogMyFilms.Debug("GUIWindowManager_OnNewAction(): Action detected - '" + action.wID + "'");
          LogMyFilms.Debug("GUIWindowManager_OnNewAction(): trailerscrobbleactive = '" + MyFilms.trailerscrobbleactive + "'");
          switch (action.wID)
          {
            case MediaPortal.GUI.Library.Action.ActionType.ACTION_RECORD:
              {
                if (!MyFilms.trailerscrobbleactive)
                {
                  int duration = (int)g_Player.Duration;
                  int currentposition = (int)g_Player.CurrentPosition;
                  string file = g_Player.CurrentFile;
                  string title = g_Player.currentTitle;
                  LogMyFilms.Debug("GUIWindowManager_OnNewAction(): Movie Action: Get Fanart Snapshot from current position (position = '" + currentposition + "', duration = '" + duration + "')");

                  GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                  dlgYesNo.SetHeading(GUILocalizeStrings.Get(1079847)); // MyFilms Fanart Creator
                  dlgYesNo.SetLine(1, ""); // dlgYesNo.SetLine(1, "-> " + currentposition + " s.");
                  dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079852) + " ?"); // Create 'snapshot' fanart from current playback position
                  dlgYesNo.SetLine(3, "");
                  // dlgYesNo.SetNoLabel("Cancel");
                  dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                  //dlgYesNo.DoModal(GetID);
                  if (!(dlgYesNo.IsConfirmed))
                    return;
                  Menu_CreateFanart_OnMoviePosition_Parameterized(duration, currentposition, file, title);
                }
                break;
              }
            case MediaPortal.GUI.Library.Action.ActionType.ACTION_PLAY:
              {
                if (MyFilms.trailerscrobbleactive)
                {
                  LogMyFilms.Debug("GUIWindowManager_OnNewAction(): Trailer Action: Play main movie");
                  // ToDo: Play main movie
                }
                break;
              }
            case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU:
              {
                if (MyFilms.trailerscrobbleactive)
                {
                  LogMyFilms.Debug("GUIWindowManager_OnNewAction(): Trailer Action: Show Options menu");
                  // ToDo: Show Options menu

                }
                break;
              }
            case MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_CHAPTER:
              {
                if (MyFilms.trailerscrobbleactive)
                {
                  // ToDo: Play next Trailer
                }
                break;
              }
            case MediaPortal.GUI.Library.Action.ActionType.ACTION_PARENT_DIR:
            case MediaPortal.GUI.Library.Action.ActionType.ACTION_KEY_PRESSED:
            case MediaPortal.GUI.Library.Action.ActionType.ACTION_MUSIC_PLAY:
            case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREV_PICTURE:
            case MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_PICTURE:
            case MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU:
            case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREV_ITEM:
            default:
              break;
          }
        }

        private static string LoadPlaylist(string filename)
        {
            PlayList playlist = new PlayList();
            IPlayListIO loader = PlayListFactory.CreateIO(filename);

            if (!loader.Load(playlist, filename))
            {
                LogMyFilms.Info("Playlist not found for movie : '" + filename.ToString() + "'");
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
                // Cover save new method
                string titleExt = movieDetails.Title + "{" + idMovie + "}";

                //string strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, movieDetails.Title);
                //string LargeThumb = MediaPortal.Util.Utils.GetLargeCoverArtName(Thumbs.MovieTitle, movieDetails.Title);
                string strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, titleExt);
                string LargeThumb = MediaPortal.Util.Utils.GetLargeCoverArtName(Thumbs.MovieTitle, titleExt);
                //// Delete old thumbs
                //Utils.FileDelete(strThumb);
                //Utils.FileDelete(LargeThumb);

                try
                {
                    string wImage;
                  string DbPicture = MyFilms.r[select_item]["Picture"].ToString();
                  if (DbPicture.IndexOf(":\\") == -1 && ( DbPicture.Length < 2 || DbPicture.Substring(0, 2) != "\\\\"))
                    wImage = MyFilms.conf.StrPathImg + "\\" + DbPicture;
                  else
                    wImage = DbPicture;
                    if (!System.IO.File.Exists(wImage))
                        wImage = Config.Dir.Skin + "\\media\\Films_No_Cover.png";
                    if (System.IO.File.Exists(wImage)) // && wImage.Length < 249 check pathes are not too long!
                    {
                      if (!System.IO.File.Exists(strThumb)) Picture.CreateThumbnail(wImage, strThumb, (int)Thumbs.ThumbResolution, (int)Thumbs.ThumbResolution, 0, Thumbs.SpeedThumbsSmall);
                      if (!System.IO.File.Exists(LargeThumb)) Picture.CreateThumbnail(wImage, LargeThumb, (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0, Thumbs.SpeedThumbsLarge);
                    }
                }
                catch (Exception ex)
                {
                  LogMyFilms.DebugException("Error updating MyVideos DB! - ", ex);
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
            Search_All_Files(select_item, delete, ref NoResumeMovie, ref newItems, ref IMovieIndex, Trailer, "");
        }

        private static void Search_All_Files(int select_item, bool delete, ref bool NoResumeMovie, ref ArrayList newItems, ref int IMovieIndex, bool Trailer, string overrideFileName)
        {
            string fileName = string.Empty;
            string[] split1;
            string[] split2;
            string[] split3;
            string strDir = string.Empty;
            int IdMovie = -1;
            int timeMovieStopped = 0;
            if (Trailer)
              {
                if (MyFilms.conf.StrDirStorTrailer.Length > 0)
                  strDir = MyFilms.conf.StrDirStorTrailer;
              }
            else 
              strDir = MyFilms.conf.StrDirStor;

            LogMyFilms.Debug("(Search_All_Files) - Modus 'Trailer' = '" + Trailer + "' - Searchpath (strDir): '" + strDir + "'");
            // retrieve filename information stored in the DB
            if (Trailer)
            {
              if (Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
              {
                LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - try filename MyFilms.r[select_item][MyFilms.conf.StrStorageTrailer]: '" + MyFilms.r[select_item][MyFilms.conf.StrStorageTrailer].ToString().Trim() + "' - ConfStorageTrailer: '" + MyFilms.conf.StrStorageTrailer + "'"); 
                try { fileName = MyFilms.r[select_item][MyFilms.conf.StrStorageTrailer].ToString().Trim(); }
                catch { fileName = string.Empty; }
              }
            }
            else
            {
              if (Helper.FieldIsSet(MyFilms.conf.StrStorage))
              {
                LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - try filename MyFilms.r[select_item][MyFilms.conf.StrStorage]: '" + MyFilms.r[select_item][MyFilms.conf.StrStorage].ToString().Trim() + "' - ConfStorage: '" + MyFilms.conf.StrStorage + "'"); 
                try { fileName = MyFilms.r[select_item][MyFilms.conf.StrStorage].ToString().Trim(); }
                catch { fileName = string.Empty; }
              }
            }

            if (!string.IsNullOrEmpty(overrideFileName))
            {
                LogMyFilms.Debug("MyFilmsDetails (Search_All_Files) - override filename: '" + overrideFileName.Trim());
                fileName = overrideFileName.Trim();
            }

            if (string.IsNullOrEmpty(fileName) && !Trailer)
            {
                // search filename by Title movie
                if ((MyFilms.conf.SearchFile == "True") || (MyFilms.conf.SearchFile == "yes"))
                {
                    string movieName = MyFilms.r[select_item][MyFilms.conf.ItemSearchFile].ToString();
                    LogMyFilms.Debug("Search_All_Files: Search = active, movieName = '" + movieName + "'");
                    movieName = movieName.Substring(movieName.LastIndexOf(MyFilms.conf.TitleDelim) + 1).Trim();
                    LogMyFilms.Debug("Search_All_Files: Search = active, movieName (cleaned) = '" + movieName + "'");
                    if (MyFilms.conf.ItemSearchFile.Length > 0)
                    {
                      LogMyFilms.Debug("Search_All_Files: Search = active, movieName (cleaned) = '" + movieName + "', SearchDirectory: '" + MyFilms.conf.StrDirStor + "'");
                      fileName = Search_FileName(movieName, MyFilms.conf.StrDirStor).Trim();
                    }

                    if ((fileName.Length > 0) && Helper.FieldIsSet(MyFilms.conf.StrStorage))
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
            LogMyFilms.Debug("Search_FileName: Launched with parameters: filename = '" + filename + "', storage = '" + storage + "'");
            result = new ArrayList();
            string file = filename;

            SearchFiles(file, storage, false, false);

            //si des resultats existent on les affichent
            if (result.Count != 0)
            {
                if (result.Count == 1)
                {
                  if (result[0].ToString().ToLower() == filename.ToLower() || MyFilms.conf.SearchOnlyExactMatches == false)
                  {
                    LogMyFilms.Debug("only one match found - return result: '" + result[0].ToString() + "'");
                    return result[0].ToString();
                  }
                  else
                  {
                    LogMyFilms.Debug("only one match found - but no exact match: '" + result[0].ToString() + "'");
                  }
                }
                string wfile = null;
                result.Sort();
                ArrayList wresult = new ArrayList();
                foreach (String s in result)
                {
                  LogMyFilms.Debug("Search_FileName - Searchresult: '" + s.ToString() + "'");
                  if (string.IsNullOrEmpty(wfile))
                  {
                    wresult.Add(s);
                    wfile = s;
                  }
                  else if (!MediaPortal.Util.Utils.ShouldStack(s, wfile) && s.ToLower() != wfile.ToLower())
                  {
                    wresult.Add(s);
                    wfile = s;
                  }
                }
                LogMyFilms.Debug("Search_FileName - Total Searchresults: '" + wresult.Count + "'");
                if (wresult.Count == 1 && (wresult[0].ToString().ToLower() == filename.ToLower() || MyFilms.conf.SearchOnlyExactMatches == false))
                    return wresult[0].ToString();
                else
                {
                  string singlefilefound = string.Empty;
                  int count = 0;
                  foreach (string searchresult in wresult)
                  {
                    if (searchresult == filename || searchresult.ToLower().Contains(@"\" + filename.ToLower() + @".") || searchresult.ToLower().Contains(@"\" + filename.ToLower() + @"\"))
                    {
                      LogMyFilms.Debug("full match found via file/directory compare -> '" + searchresult + "'");
                      count = count + 1;
                      singlefilefound = searchresult;
                    }
                  }
                  if (count == 1)
                  {
                    LogMyFilms.Debug("Single match found via file/directory compare -> using: '" + singlefilefound + "'");
                    return singlefilefound;
                  }
                  // else if (MyFilms.conf.SearchOnlyExactMatches.ToLower() != "no") return ""; // activate, if no "near match" should be displayed in exact match mode
                }
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
            else // No Movie File Found
            {
                LogMyFilms.Info("File not found for movie '" + filename.ToString() + "'; string search '" + wsearchfile.ToString() + "'");
                return string.Empty;
            }
        }
        /// <summary>
        /// Permet de rechercher un fichier dans un dossier et ces sous dossier
        /// </summary>
        /// <param name="fileName">le nom du fichier recherché</param>
        /// <param name="searchrep">le chemin du répertoire dans lequel chercher</param>
        /// <param name="recur">spécifie s'il s'agit d'une relance recursive</param>
        /// <param name="Trailer">if called for trailers</param>
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
                    if (MyFilms.conf.SearchSubDirs == false) continue;
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
                    if ((MyFilms.conf.SearchSubDirs == false) || (!System.IO.Directory.Exists(wpath))) continue;
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
          LogMyFilms.Warn("MountImageFile() - called with WindowID = '" + WindowID + "', file = '" + file + "'");
          MPSettings mpSettings = new MPSettings();
          m_askBeforePlayingDVDImage = mpSettings.GetValueAsBool("daemon", "askbeforeplaying", false);
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
              //if (enable)
              //{
              //  GUIWaitCursor.Init(); 
              //  GUIWaitCursor.Show();
              //}
              //else
              //{
              //  GUIWaitCursor.Hide();
              //}
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
              LogMyFilms.Warn("setProcessAnimationStatus '" + enable + "' - skin control missing? : " + ex.Message);
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
                    // psi.RedirectStandardOutput = true; // redirect output to streamreader - ToDo: add reader to use and add console output to AMCU !
                    p.StartInfo = psi;
                    LogMyFilms.Debug("RunAMCupdater - Starting external command: {0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);
                    try
                    {
                        p.Start();
                        // p.Kill();
                        p.WaitForExit();
                    }
                    catch (Exception e)
                    {
                        LogMyFilms.Debug(e.ToString());
                    }
                    LogMyFilms.Debug("RunAMCupdater - External command finished");
                }
            }
        }

        //-------------------------------------------------------------------------------------------
        //  Search All Trailerfiles locally
        //-------------------------------------------------------------------------------------------        
        public static void SearchTrailerLocal(DataRow[] r1, int Index, bool ExtendedSearch)
        {
            LogMyFilms.Debug("(SearchTrailerLocal) - mastertitle      : '" + MyFilms.r[Index][MyFilms.conf.StrTitle1] + "'");
            if (Helper.FieldIsSet(MyFilms.conf.StrTitle2))
              LogMyFilms.Debug("(SearchTrailerLocal) - secondary title  : '" + MyFilms.r[Index][MyFilms.conf.StrTitle2] + "'");
            LogMyFilms.Debug("(SearchTrailerLocal) - Cleaned Title    : '" + MediaPortal.Util.Utils.FilterFileName(MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString().ToLower()) + "'");            
            LogMyFilms.Debug("(SearchtrailerLocal) - Index            : '" + Index + "'");
            LogMyFilms.Debug("(SearchtrailerLocal) - StrDirStortrailer: '" + MyFilms.conf.StrDirStorTrailer + "'");
            LogMyFilms.Debug("(SearchtrailerLocal) - FullMediasource  : '" + (string)MyFilms.r[Index][MyFilms.conf.StrStorage].ToString().Trim() + "'");

            result = new ArrayList();
            ArrayList resultsize = new ArrayList();
            string[] filesfound = new string[100];
            Int64[] filesfoundsize = new Int64[100];
            int filesfoundcounter = 0;
            string file = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
            string titlename = MyFilms.r[Index][MyFilms.conf.StrTitle1].ToString();
            string titlename2 = "";
            if (Helper.FieldIsSet(MyFilms.conf.StrTitle2))
              titlename2 = MyFilms.r[Index][MyFilms.conf.StrTitle2].ToString();
            string directoryname = "";
            string movieName = "";
            string[] files = null;
            Int64 wsize = 0; // Temporary Filesize detection
            LogMyFilms.Debug("(SearchtrailerLocal) Extended Search '" + ExtendedSearch + "' for movie '" + file + "' in search directories: '" + MyFilms.conf.StrDirStorTrailer + "'");

            //Retrieve original directory of mediafiles
            //directoryname
            movieName = (string)MyFilms.r[Index][MyFilms.conf.StrStorage].ToString().Trim();
            movieName = movieName.Substring(movieName.LastIndexOf(";") + 1);
            LogMyFilms.Debug("(SearchtrailerLocal) splits media directory name: '" + movieName + "'"); 
            try    
            { directoryname = System.IO.Path.GetDirectoryName(movieName); }
            catch
            { directoryname = ""; }
            LogMyFilms.Debug("(SearchtrailerLocal) get media directory name: '" + directoryname + "'");
            if (!System.IO.Directory.Exists(directoryname))
            {
              directoryname = "";
              LogMyFilms.Debug("(SearchtrailerLocal) directory of movie '" + movieName + "' doesn't exist anymore - check your DB");
            }

            //Search Files in Mediadirectory
            if (!string.IsNullOrEmpty(directoryname))
            {
              files = Directory.GetFiles(directoryname, "*.*", SearchOption.AllDirectories);
              foreach (string filefound in files)
              {
                if ((filefound.ToLower().Contains("trailer") || filefound.ToLower().Contains("trl")) && MediaPortal.Util.Utils.IsVideo(filefound))
                {
                  wsize = new System.IO.FileInfo(filefound).Length;
                  result.Add(filefound);
                  resultsize.Add(wsize);
                  filesfound[filesfoundcounter] = filefound;
                  filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                  filesfoundcounter = filesfoundcounter + 1;
                  LogMyFilms.Debug("(TrailersearchLocal) - FilesFound in MediaDir: Size '" + wsize + "' - Name '" + filefound + "'");
                }
              }
            }
            
            //Search Filenames with "title" in Trailer Searchpath
            if (ExtendedSearch && !string.IsNullOrEmpty(MyFilms.conf.StrDirStorTrailer))
            {
                LogMyFilms.Debug("SearchTrailerLocal - starting ExtendedSearch in Searchdirectory: '" + MyFilms.conf.StrDirStorTrailer.ToString() + "'");
                // split searchpath information delimited by semicolumn (multiple searchpathes from config)
                string[] Trailerdirectories = MyFilms.conf.StrDirStorTrailer.Split(new Char[] { ';' });
                foreach (string storage in Trailerdirectories)
                {
                  LogMyFilms.Debug(
                    "(TrailersearchLocal) - TrailerSearchDirectory: '" + storage + "', search title1: '" +
                    titlename.ToLower() + "', search title2: '" + titlename2.ToLower() + "'");
                  // First search rootdirectory
                  files = Directory.GetFiles(storage, "*.*", SearchOption.TopDirectoryOnly);
                  foreach (string filefound in files)
                  {
                    LogMyFilms.Debug(
                      "(TrailersearchLocal) - Files found in root dir to check matching: '" + filefound + "'");
                    if ((!string.IsNullOrEmpty(titlename) && filefound.ToLower().Contains(titlename.ToLower())) ||
                        (!string.IsNullOrEmpty(titlename2) && filefound.ToLower().Contains(titlename2.ToLower())) &&
                        (MediaPortal.Util.Utils.IsVideo(filefound)))
                    {
                      wsize = new System.IO.FileInfo(filefound).Length;
                      result.Add(filefound);
                      resultsize.Add(wsize);
                      filesfound[filesfoundcounter] = filefound;
                      filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                      filesfoundcounter = filesfoundcounter + 1;
                      LogMyFilms.Debug(
                        "(TrailersearchLocal) - Matching Singlefiles found in TrailerRootDIR: Size '" + wsize +
                        "' - Name '" + filefound + "'");
                    }
                  }

                  // Now search subdirectories
                  string[] directories = Directory.GetDirectories(storage, "*.*", SearchOption.AllDirectories);
                  foreach (string directoryfound in directories)
                  {
                    LogMyFilms.Debug(
                      "(TrailersearchLocal) - Directory found to check matching: '" + directoryfound + "'");
                    // check for directories with title name
                    // if ((!string.IsNullOrEmpty(titlename) && directoryfound.ToString().ToLower().Contains(titlename.ToLower())) || (!string.IsNullOrEmpty(titlename2) && directoryfound.ToString().ToLower().Contains(titlename2.ToLower())))
                    if (Utility.ContainsAll(directoryfound, titlename, ":") || Utility.ContainsAll(directoryfound, titlename2, ":"))
                    {
                      LogMyFilms.Debug("(TrailersearchLocal) - Matching Directory found : '" + directoryfound + "'");
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
                          LogMyFilms.Debug("(TrailersearchLocal) - Files added matching Directory: Size '" + wsize + "' - Name '" + filefound + "'");
                        }
                      }
                    }
                    else
                    {
                      files = Directory.GetFiles(directoryfound, "*.*", SearchOption.AllDirectories);
                      foreach (string filefound in files)
                      {
                        LogMyFilms.Debug(
                          "(TrailersearchLocal) - Files found in sub dir to check matching: '" + filefound + "'");
                        // if (((!string.IsNullOrEmpty(titlename) && filefound.ToLower().Contains(titlename.ToLower())) || (!string.IsNullOrEmpty(titlename2) && filefound.ToLower().Contains(titlename2.ToLower()))) && (MediaPortal.Util.Utils.IsVideo(filefound)))
                        if (Utility.ContainsAll(filefound, titlename, ":") || Utility.ContainsAll(filefound, titlename2, ":") && MediaPortal.Util.Utils.IsVideo(filefound))
                        {
                          wsize = new System.IO.FileInfo(filefound).Length;
                          result.Add(filefound);
                          resultsize.Add(wsize);
                          filesfound[filesfoundcounter] = filefound;
                          filesfoundsize[filesfoundcounter] = new System.IO.FileInfo(filefound).Length;
                          filesfoundcounter = filesfoundcounter + 1;
                          LogMyFilms.Debug("(TrailersearchLocal) - Matching Singlefiles found in TrailerDIR: Size '" + wsize + "' - Name '" + filefound + "'");
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
              if (!string.IsNullOrEmpty(n)) LogMyFilms.Debug("(Sorted Trailerfiles) ******* : '" + n + "'");
            }  

            Array.Sort(filesfoundsize);
            for (int i = 0; i < result.Count; i++)
            {
              if (!string.IsNullOrEmpty(filesfound[i])) LogMyFilms.Debug("(Sorted Trailerfiles) ******* : Number: '" + i + "' - Size: '" + filesfoundsize[i] + "' - Name: '" + filesfound[i] + "'");
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
              LogMyFilms.Debug("(SearchTrailerLocal) - Total Files found: '" + result.Count + "', TrailerSourcePath: '" + trailersourcepath + "'");
            }

            else
              LogMyFilms.Debug("MyFilmsDetails (SearchTrailerLocal) - NO TRAILERS FOUND !!!!");

            if ((trailersourcepath.Length > 0) && Helper.FieldIsSet(MyFilms.conf.StrStorageTrailer))
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
            using (MediaPortal.Profile.Settings xmlreader = new MPSettings())
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

                                LogMyFilms.Debug("(HandleWakeUpNas) : WOL - GetHostAddresses({0}) returns:", hostName);

                                foreach (IPAddress ip in ips)
                                {
                                    LogMyFilms.Debug("    {0}", ip);
                                }

                                //Use first valid IP address
                                ipAddress = ips[0];
                            }
                            catch (Exception ex)
                            {
                                LogMyFilms.Error("(HandleWakeUpNas) : WOL - Failed GetHostAddress - {0}", ex.Message);
                            }
                        }

                        //Check for valid IP address
                        if (ipAddress != null)
                        {
                            //Update the MAC address if possible
                            hwAddress = wakeOnLanManager.GetHardwareAddress(ipAddress);

                            if (wakeOnLanManager.IsValidEthernetAddress(hwAddress))
                            {
                                LogMyFilms.Debug("(HandleWakeUpNas) : WOL - Valid auto MAC address: {0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}"
                                          , hwAddress[0], hwAddress[1], hwAddress[2], hwAddress[3], hwAddress[4], hwAddress[5]);

                                //Store MAC address
                                macAddress = BitConverter.ToString(hwAddress).Replace("-", ":");

                                LogMyFilms.Debug("(HandleWakeUpNas) : WOL - Store MAC address: {0}", macAddress);

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
                    using (MediaPortal.Profile.Settings xmlreader = new MPSettings())
                    {
                        macAddress = xmlreader.GetValueAsString("nas", "macAddress", null);
                    }

                    LogMyFilms.Debug("(HandleWakeUpNas) : WOL - Use stored MAC address: {0}", macAddress);

                    try
                    {
                        hwAddress = wakeOnLanManager.GetHwAddrBytes(macAddress);

                        //Finally, start up the TV server
                        LogMyFilms.Info("(HandleWakeUpNas) : WOL - Start the NAS server");

                        if (wakeOnLanManager.WakeupSystem(hwAddress, hostName, intTimeOut))
                        {
                            LogMyFilms.Info("(HandleWakeUpNas) : WOL - The NAS server started successfully!");
                        }
                        else
                        {
                            LogMyFilms.Error("(HandleWakeUpNas) : WOL - Failed to start the NAS server");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMyFilms.Error("(HandleWakeUpNas) : WOL - Failed to start the NAS server - {0}", ex.Message);
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

        public static void ShowNotificationDialog(string headline, string line)
        {
            GUIDialogNotify dlg = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.SetImage(GetMyFilmsDefaultLogo());
                dlg.SetHeading(headline);
                dlg.SetText(line);
                dlg.DoModal(GUIWindowManager.ActiveWindow);
            }
            return;
        }

        internal static string GetMyFilmsDefaultLogo()
        {
            // first check subfolder of current skin (allows skinners to use custom icons)
            string image = string.Format(@"{0}\Media\MyFilms\MyFilms.png", GUIGraphicsContext.Skin);
            if (!System.IO.File.Exists(image))
            {
                // use png in thumbs folder
                image = string.Format(@"{0}\MyFilms\DefaultImages\MyFilms.png", Config.GetFolder(Config.Dir.Thumbs));
            }
            return image;
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
          setGUIProperty(name, value, MyFilms.DebugPropertyLogging);
        }

        public static void setGUIProperty(string name, string value, bool log)
        {
            string property = "#myfilms." + name;
            if (log)
            LogMyFilms.Debug("setGuiProperty [{0}]: '{1}' - nonsanitized input: '{2}'", property, StringExtensions.SanitizeXmlString(value), value);
            GUIPropertyManager.SetProperty(property, StringExtensions.SanitizeXmlString(value));
        }

        //void clearGUIProperty(guiProperty name)
        //{
        //    clearGUIProperty(name.ToString());
        //}

        private static readonly HashSet<char> badChars = new HashSet<char> { '!', '@', '#', '$', '%', '_' };
        public static string CleanString(string str)
        {
          var result = new StringBuilder(str.Length);
          for (int i = 0; i < str.Length; i++)
          {
            if (!badChars.Contains(str[i]))
              result.Append(str[i]);
          }
          return result.ToString();
        }
        
        public static void clearGUIProperty(string name)
        {
          setGUIProperty(name, string.Empty, MyFilms.DebugPropertyLogging);
        }

        public static void clearGUIProperty(string name, bool log)
        {
            setGUIProperty(name, string.Empty, log);
        }

        public static void GetActorByName(string strActorName, ArrayList actors)
        {
            strActorName = MediaPortal.Database.DatabaseUtility.RemoveInvalidChars(strActorName);

          IMDB actornames = new IMDB();
          actornames.FindActor(strActorName);

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
            LogMyFilms.Debug("Disabled feature due to startmode 'normal': '" + disabledfeature + "'");
          return false;
        }

        public static bool IsInternetConnectionAvailable()
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

      public static MFMovie GetCurrentMovie()
      {
        return MyFilms.currentMovie;
      }

      public bool ReadMediaInfo(string file, ref MediaInfo mediainfo)
      {
        Grabber.MediaInfo MI = Grabber.MediaInfo.GetInstance();

        // MediaInfo Object could not be created
        if (null == MI) return false;

        // Check if File Exists and is not an Image type e.g. ISO (we can't extract mediainfo from that)
        if (System.IO.File.Exists(file) && !Helper.IsImageFile(file))
        {
          try
          {
            LogMyFilms.Debug("Attempting to read Mediainfo for ", file.ToString());

            // open file in MediaInfo
            MI.Open(file);

            // check number of failed attempts at mediainfo extraction                    
            int noAttempts = 0;

            // Get Playtime (runtime)
            string result = MI.VideoPlaytime;
            mediainfo = MI;

            string LocalPlaytime = result != "-1" ? result : noAttempts.ToString();

            bool failed = false;
            if (result != "-1")
            {
              string VideoCodec = MI.VideoCodec;
              string VideoFormat = MI.VideoCodecFormat;
              string VideoFormatProfile = MI.VideoFormatProfile;
              string VideoBitRate = MI.VideoBitrate;
              string VideoFrameRate = MI.VideoFramesPerSecond;
              string VideoWidth = MI.VideoWidth;
              string VideoHeight = MI.VideoHeight;
              string VideoAspectRatio = MI.VideoAspectRatio;

              string AudioCodec = MI.AudioCodec;
              string AudioFormat = MI.AudioCodecFormat;
              string AudioFormatProfile = MI.AudioFormatProfile;
              string AudioBitrate = MI.AudioBitrate;
              string AudioChannels = MI.AudioChannelCount;
              string AudioTracks = MI.AudioStreamCount;

              string TextCount = MI.SubtitleCount;

              // check for subtitles in mediainfo
              List<string> files = new List<string>();
              files.Add(file);
              bool AvailableSubtitles = checkHasSubtitles(files, true);
            }
            else
              failed = true;

            // MediaInfo cleanup
            MI.Close();

            if (failed)
            {
              // Get number of retries left to report to user
              int retries = 3 - (noAttempts * -1);

              string retriesLeft = retries > 0 ? retries.ToString() : "No";
              retriesLeft = string.Format("Problem parsing MediaInfo for: {0}, ({1} retries left)", file.ToString(), retriesLeft);

              LogMyFilms.Debug(retriesLeft);
            }
            else
            {
              LogMyFilms.Debug("Succesfully read MediaInfo for ", file.ToString());
            }
            return true;
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("Error reading MediaInfo: ", ex.Message);
          }

        }
        LogMyFilms.Debug("File '" + file + "' does not exist or is an image file");
        return false;

      }

      private string GetFileTimeStamps(string file)
      {
        try
        {
          if (System.IO.File.Exists(file))
          {
            string FileDateCreated = System.IO.File.GetCreationTime(file).ToString("yyyy-MM-dd HH:mm:ss");
            return FileDateCreated;
          }
        }
        catch
        {
          LogMyFilms.Debug("Error: Unable to extract File Timestamps");
          string FileDateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
          return FileDateNow;
        }
        return string.Empty;
      }

      public bool checkHasLocalSubtitles(List<string> filenames)
      {
        return checkHasSubtitles(filenames, false);
      }

      public bool checkHasSubtitles(List<string> filenames, bool useMediaInfo)
      {
        if (filenames.Count == 0) 
          return false;

        int textCount = -1;
        if (useMediaInfo)
        {
          textCount = 0;
        }

        if (Helper.IsSubCentralAvailableAndEnabled)
        {
          return checkHasSubtitlesFromSubCentral(filenames, useMediaInfo, textCount);
        }

        if (textCount > 0)
          return true;
        return false;
      }

      private bool checkHasSubtitlesFromSubCentral(List<string> files, bool useMediaInfo, int textCount)
      {
        LogMyFilms.Debug(string.Format("Using SubCentral for checkHasSubtitles(), useMediaInfo = {0}, textCount = {1}", useMediaInfo.ToString(), textCount.ToString()));
        List<FileInfo> fiFiles = new List<FileInfo>();
        foreach (string file in files)
        {
          fiFiles.Add(new FileInfo(file));
        }
        bool result = SubCentral.Utils.SubCentralUtils.MediaHasSubtitles(fiFiles, false, textCount, !useMediaInfo);
        LogMyFilms.Debug(string.Format("SubCentral returned {0}", result));
        return result;
      }

      bool isWritable(FileInfo fileInfo)
      {
        FileStream stream = null;
        try
        {
          stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch
        {
          return true;
        }
        finally
        {
          if (stream != null) stream.Close();
        }
        return false;
      }

      public bool isWritable(string file)
      {
        if (string.IsNullOrEmpty(file)) return false;
        FileInfo fi = new FileInfo(file);
        if (fi != null)
        {
          return isWritable(fi);
        }
        return false;
      }

      private void GUIWindowManager_OnNewMessage(GUIMessage message)
      {
        //public enum MediaType // global category
        //{
        //  UNKNOWN = 0,
        //  PHOTO = 1,
        //  VIDEO = 2,
        //  AUDIO = 3
        //}
        //public enum MediaSubType // add here new formats
        //{
        //  UNKNOWN = 0,
        //  DVD = 1,
        //  AUDIO_CD = 2,
        //  BLURAY = 3,
        //  HDDVD = 4,
        //  VCD = 5,
        //  FILES = 6
        //}
        switch (message.Message)
        {
          case GUIMessage.MessageType.GUI_MSG_AUTOPLAY_VOLUME:
            LogMyFilms.Debug("GUIWindowManager_OnNewMessage() - New Message received ! - MessageType = '" + message.Message.ToString() + "', Param1 = '" + message.Param1 + "', Param2 = '" + message.Param2 + "', message label = '" + message.Label + "'");
            if (message.Param1 == (int)MediaPortal.Ripper.AutoPlay.MediaType.VIDEO)
            {
              switch (message.Param2)
              {
                case (int)AutoPlay.MediaSubType.AUDIO_CD:
                case (int)AutoPlay.MediaSubType.BLURAY:
                  LogMyFilms.Debug(
                    "GUIWindowManager_OnNewMessage() - New Message received - 'AutoPlay.MediaSubType.BLURAY'");
                  // GUIWindowManager.ActivateWindow(MyFilms.ID_BluRayPlayerLauncher);
                  break;
                case (int)AutoPlay.MediaSubType.DVD:
                  LogMyFilms.Debug(
                    "GUIWindowManager_OnNewMessage() - New Message received - 'AutoPlay.MediaSubType.DVD'");
                  // OnPlayDVD(message.Label, GetID);
                  break;
                case (int)AutoPlay.MediaSubType.FILES:
                case (int)AutoPlay.MediaSubType.VCD:
                  LogMyFilms.Debug(
                    "GUIWindowManager_OnNewMessage() - New Message received - 'AutoPlay.MediaSubType.VCD' or 'AutoPlay.MediaSubType.FILES'");
                  // OnPlayFiles((System.Collections.ArrayList)message.Object);
                  break;
                case (int)AutoPlay.MediaSubType.HDDVD:
                case (int)AutoPlay.MediaSubType.UNKNOWN:
                  break;
                default:
                  break;
              }
            }
            break;
          default:
            break;
        }
      }

      #region External Player

      private static void LaunchExternalPlayer(string videoPath)
      {
        LogMyFilms.Debug("LaunchExternalPlayer() - Launching external player.");

        // First check if the user supplied executable for the external player is valid
        string execPath = MyFilms.conf.ExternalPlayerPath;
        if (!File.Exists(execPath))
        {
          // if it's not show a dialog explaining the error
          ShowNotificationDialog("Error", "MissingExternalPlayerExe");
          LogMyFilms.Warn("The external player executable '{0}' is missing.", execPath);
          // do nothing
          resetPlayer();
          return;
        }

        // process the argument string and replace the 'filename' variable
        string arguments = MyFilms.conf.ExternalPlayerStartParams;
        string videoRoot = Utility.GetMovieBaseDirectory(new FileInfo(videoPath).Directory).FullName;
        string filename = Utility.IsDriveRoot(videoRoot) ? videoRoot : videoPath;
        string fps = GetFPS(videoPath);
        string virtualDrive = DaemonTools.GetVirtualDrive();
        if (virtualDrive.LastIndexOf("\\") != virtualDrive.Length - 1) virtualDrive = virtualDrive.Trim() + "\\";

        arguments = arguments.Replace("%filename%", filename);
        arguments = arguments.Replace("%fps%", fps);
        arguments = arguments.Replace("%root%", videoRoot);
        arguments = arguments.Replace("%drive%", virtualDrive);

        LogMyFilms.Debug("External Player: Video='{0}', Root={1}, FPS={2}, ExecPath={3}, CommandLine={4}, VirtualDrive={5}", filename, videoRoot, fps, execPath, arguments, virtualDrive);
        LogMyFilms.Debug("Command Line: '" + arguments + "'");

        // Set Refresh Rate Based On FPS if needed
        bool UseDynamicRefreshRateChangerWithExternalPlayer = true; // Todo: add setting "Settings.UseDynamicRefreshRateChangerWithExternalPlayer"
        if (UseDynamicRefreshRateChangerWithExternalPlayer) 
        {
          double framerate = double.Parse(GetFPS(videoPath).ToString(NumberFormatInfo.InvariantInfo), NumberFormatInfo.InvariantInfo);
          LogMyFilms.Info("Requesting new refresh rate: FPS={0}", framerate.ToString());
          RefreshRateChanger.SetRefreshRateBasedOnFPS(framerate, filename, RefreshRateChanger.MediaType.Video);
          if (RefreshRateChanger.RefreshRateChangePending)
          {
            TimeSpan ts = DateTime.Now - RefreshRateChanger.RefreshRateChangeExecutionTime;
            if (ts.TotalSeconds > RefreshRateChanger.WAIT_FOR_REFRESHRATE_RESET_MAX)
            {
              LogMyFilms.Info("Refresh rate change failed. Please check your mediaportal log and configuration", RefreshRateChanger.WAIT_FOR_REFRESHRATE_RESET_MAX);
              RefreshRateChanger.ResetRefreshRateState();
            }
          }
        }

        // Setup the external player process
        ProcessStartInfo processinfo = new ProcessStartInfo();
        processinfo.FileName = execPath;
        processinfo.Arguments = arguments;

        Process hdPlayer = new Process();
        hdPlayer.StartInfo = processinfo;
        hdPlayer.Exited += OnHDPlayerExited;
        hdPlayer.EnableRaisingEvents = true;

        try
        {
          // start external player process
          hdPlayer.Start();

          // disable mediaportal input devices
          MediaPortal.InputDevices.InputDevices.Stop();

          // hide mediaportal and suspend rendering to save resources for the external player
          GUIGraphicsContext.BlankScreen = true;
          GUIGraphicsContext.form.Hide();
          GUIGraphicsContext.CurrentState = GUIGraphicsContext.State.SUSPENDING;

          LogMyFilms.Info("HD Playback: External player started.");
          // onMediaStarted(queuedMedia);
        }
        catch (Exception e)
        {
          LogMyFilms.ErrorException("HD Playback: Could not start the external player process.", e);
          resetPlayer();
        }
      }

      private static void OnHDPlayerExited(object obj, EventArgs e)
      {
        // Restore refresh rate if it was changed
        bool UseDynamicRefreshRateChangerWithExternalPlayer = true; // Todo: add setting "Settings.UseDynamicRefreshRateChangerWithExternalPlayer"
        if (UseDynamicRefreshRateChangerWithExternalPlayer && RefreshRateChanger.RefreshRateChangePending)
          RefreshRateChanger.AdaptRefreshRate();

        // enable mediaportal input devices
        MediaPortal.InputDevices.InputDevices.Init();

        // show mediaportal and start rendering
        GUIGraphicsContext.BlankScreen = false;
        GUIGraphicsContext.form.Show();
        GUIGraphicsContext.ResetLastActivity();
        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GETFOCUS, 0, 0, 0, 0, 0, null);
        GUIWindowManager.SendThreadMessage(msg);
        GUIGraphicsContext.CurrentState = GUIGraphicsContext.State.RUNNING;
        LogMyFilms.Info("HD Playback: The external player has exited.");
      }

      private static void resetPlayer()
      {
        //// If we have an image mounted, unmount it
        //if (mountedPlayback)
        //{
        //  queuedMedia.UnMount();
        //  mountedPlayback = false;
        //}

        // reset player variables

        if (GUIGraphicsContext.IsFullScreenVideo)
          GUIGraphicsContext.IsFullScreenVideo = false;

        //activeMedia = null;
        //queuedMedia = null;
        //_playerState = MoviePlayerState.Idle;
        //_resumeActive = false;
        //listenToExternalPlayerEvents = false;
        //donePlayingCustomIntros = false;
        //customIntrosPlayed = 0;

        LogMyFilms.Debug("Reset.");
      }

      /// <summary>
      /// Disable MediaPortal AutoPlay
      /// </summary>
      private static void disableNativeAutoplay() 
      {
          LogMyFilms.Info("Disabling native autoplay.");
          AutoPlay.StopListening();
      }

      /// <summary>
      /// Enable MediaPortal AutoPlay
      /// </summary>
      private static void enableNativeAutoplay() 
      {
          if (GUIGraphicsContext.CurrentState == GUIGraphicsContext.State.RUNNING) 
          {
            LogMyFilms.Info("Re-enabling native autoplay.");
              AutoPlay.StartListening();
          }
      }

      /// <summary>
      /// Get refresh rate for file
      /// </summary>
      private static string GetFPS(string file)
      {
        LogMyFilms.Debug("GetFPS - Get RefreshRate for file '" + file + "'.");
        if (!System.IO.File.Exists(file))
        {
          return "";
        }

        double fps = -1;
        string _BD_Framerate = "";
        try
        {
          MediaInfo MI = new MediaInfo();
          MI.Open(file);
          _BD_Framerate = MI.Get(Grabber.StreamKind.Video, 0, "FrameRate");

          LogMyFilms.Info("GetFPS() - Framerate via Mediainfo: - {0}", _BD_Framerate);

          switch (_BD_Framerate)
          {
            case "23.976":
              fps = 23.976;
              break;
            case "24":
              fps = 24;
              break;
            case "24.000":
              fps = 24;
              break;
            case "25":
              fps = 25;
              break;
            case "25.000":
              fps = 25;
              break;
            case "29.970":
              fps = 29.97;
              break;
            case "50":
              fps = 50;
              break;
            case "50.000":
              fps = 50;
              break;
            case "59.94":
              fps = 59.94;
              break;
          }
        }
        catch (Exception e)
        {
          LogMyFilms.Error("GetFPS() - failed to get refresh rate from disk!");
          LogMyFilms.Error("GetFPS() - exception {0}", e);
          return "";
        }
        return _BD_Framerate;
      }

      #endregion

      private void AddPersonsToDownloadQueue() // add persons of current movie to download queue
      {
        new System.Threading.Thread(delegate()
        {
          {
            if (MyFilms.conf.UseThumbsForPersons && System.IO.Directory.Exists(MyFilms.conf.StrPathArtist))
            {
              string persons = MyFilms.r[MyFilms.conf.StrIndex]["Persons"].ToString();
              ArrayList w_tableau = new ArrayList();
              w_tableau = MyFilms.Search_String(persons);
              foreach (object t in w_tableau)
              {
                string personsname = t.ToString();
                if (!(System.IO.File.Exists(MyFilms.conf.StrPathArtist + "\\" + personsname + ".jpg")))
                {
                  downloadPersonImage(personsname);
                  LogMyFilms.Debug("Person '" + personsname + "' added to downloadQueue !");
                }
                else LogMyFilms.Debug("Person '" + personsname + "' NOT added to downloadQueue - image already exists !");
              }
              // if (PersonstoDownloadQueue.Count > 0 && !downloadingWorker.IsBusy) downloadingWorker.RunWorkerAsync(); // already doine in submethod !
            }
          }
          GUIWindowManager.SendThreadCallbackAndWait((p1, p2, data) =>
          {
            {
              // this after thread finished ...
            }
            return 0;
          }, 0, 0, null);
        }) { Name = "MyFilmsPersonToDLqueueLoader", IsBackground = true }.Start();
      }
      
      private void downloadPersonImage(string personname) //void downloadPersonImage(DBPersonInfo person)
      {
        // we need to get it, let's queue them up and download in the background
        DBPersonInfo person = new DBPersonInfo();
        person.Name = personname;
        lock (PersonstoDownloadQueue)
        {
          PersonstoDownloadQueue.Enqueue(person);
        }
        setDownloadStatus();
        if (!downloadingWorker.IsBusy) downloadingWorker.RunWorkerAsync(); // finally lets check if the downloader is already running, and if not start it
      }

      #region GUI Events
      private void InitOVEventHandler()
      {
        // Subscribe to Events
          try
          {
            GUIOnlineVideos OV = (GUIOnlineVideos)GUIWindowManager.GetWindow(MyFilms.ID_OnlineVideos);
            OV.VideoDownloaded += new OnlineVideos.MediaPortal1.GUIOnlineVideos.VideoDownloadedHandler(OnVideoDownloaded);
            LogMyFilms.Debug("Subscribed 'VideoDownloaded' event from OnlineVideos ...");
          }
          catch (Exception ex)
          {
            LogMyFilms.Error("Error subscribing to 'VideoDownloaded' event from OnlineVideos: " + ex.Message);
          }
      }
      
      private void OnVideoDownloaded(string file, string site, string categoryRecursiveName, string videoTitle)
      {
        LogMyFilms.Debug("OnVideoDownloaded() - file = '" + file + "', site = '" + site + "', categoryrecursivename = '" + categoryRecursiveName + "', videoTitle = '" + videoTitle + "'");
      }
      #endregion

    }

}
        #endregion
