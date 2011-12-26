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
using MediaPortal.GUI.Library;
using Action = MediaPortal.GUI.Library.Action;
using System.ComponentModel;
using System.Drawing;
using MyFilmsPlugin.MyFilms;
using MyFilmsPlugin.MyFilms.MyFilmsGUI;
using MyFilmsPlugin.MyFilms.Utils;
using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  using System.Collections;
  using System.Data;
  using System.IO;

  using Grabber;

  class MyFilmsCoverManager : GUIWindow
    {
        [SkinControlAttribute(50)]
        protected GUIFacadeControl m_Facade = null;

        [SkinControlAttribute(2)]
        protected GUIButtonControl buttonLayouts = null;

        [SkinControlAttribute(11)]
        protected GUILabelControl labelResolution = null;

        [SkinControlAttribute(12)]
        protected GUIButtonControl buttonFilters = null;

        enum menuAction
        {
          LoadSingle,
          LoadMultiple,
          CreateFromMovie,
          CreateFromMovieAsMosaic,
          LoadFromTmdb,
          DeleteSelected,
          DeleteAllLow,
          DeleteAllMedium,
          DeleteAllHigh,
          DeleteAllExceptSelected,
          UseAsDefault,
          Filters,
          ClearMovieCover
        }

        enum menuFilterAction
        {
            all,
            hd,
            fullhd,
            high,
            medium,
            low
        }

        public enum View
        {
            List = 0,
            Icons = 1,
            LargeIcons = 2,
            FilmStrip = 3,
            AlbumView = 4,
            PlayList = 5
        }

        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
        int movieId = -1;
        private string movieLabel = string.Empty;
        private string artworkFileName = string.Empty;
        private string newArtworkFileName = string.Empty;
        private string displayFilter = string.Empty;
        BackgroundWorker loadingWorker = null; // to fetch list and thumbnails
        public static BackgroundWorker downloadingWorker = new BackgroundWorker(); // to do the actual downloading
        static Queue<MFCover> toDownload = new Queue<MFCover>();
        int m_PreviousSelectedItem = -1;
        private View currentView = View.LargeIcons;
        private int initialView = -1;
        bool m_bQuickSelect = false;

        # region DownloadWorker
        static MyFilmsCoverManager()
        {         
            // lets set up the downloader            
            downloadingWorker.WorkerSupportsCancellation = true;
            downloadingWorker.WorkerReportsProgress = true;
            downloadingWorker.DoWork += new DoWorkEventHandler(downloadingWorker_DoWork);
            downloadingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(downloadingWorker_RunWorkerCompleted);
            
            setDownloadStatus();
        }

        void downloadingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (loadingWorker != null && !loadingWorker.IsBusy)
            {                
                m_PreviousSelectedItem = m_Facade.SelectedListItemIndex;

                if (m_Facade != null) m_Facade.Clear();
                loadingWorker.RunWorkerAsync(this.MovieID);
            }
        }

        static void downloadingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            setDownloadStatus();
        }

        static void downloadingWorker_DoWork(object sender, DoWorkEventArgs e)
        {            
            //do
            //{
            //    DBFanart f;
            //    setDownloadStatus();
            //    lock (toDownload)
            //    { 
            //        f = toDownload.Dequeue();                     
            //    }

            //    bool bDownloadSuccess = true;
            //    // ZF: async download of the Artwork. Cancelling now works
            //    if (f != null && !f.isAvailableLocally)
            //    {
            //        string filename = f[DBFanart.cBannerPath];
            //        filename = filename.Replace("/", @"\");
            //        string fullURL = (DBOnlineMirror.Banners.EndsWith("/") ? DBOnlineMirror.Banners : (DBOnlineMirror.Banners + "/")) + filename;
            //        int nDownloadGUID = Online_Parsing_Classes.OnlineAPI.StartFileDownload(fullURL, Settings.Path.Artwork, filename);
            //        while (Online_Parsing_Classes.OnlineAPI.CheckFileDownload(nDownloadGUID))
            //        {
            //            if (downloadingWorker.CancellationPending) 
            //            {
            //                // Cancel, clean up pending download
            //                bDownloadSuccess = false;
            //                Online_Parsing_Classes.OnlineAPI.CancelFileDownload(nDownloadGUID);
            //                MPTVSeriesLog.Write("cancel Artwork download: " + f.FullLocalPath);
            //            }
            //            System.Windows.Forms.Application.DoEvents();
            //        }
            //        // Download is either completed or canceled
            //        if (bDownloadSuccess) 
            //        {
            //            f[DBFanart.cLocalPath] = filename.Replace(Settings.GetPath(Settings.Path.Artwork), string.Empty);
            //            f.Commit();
            //            MPTVSeriesLog.Write("Successfully downloaded Artwork: " + f.FullLocalPath);
            //            downloadingWorker.ReportProgress(0, f[DBFanart.cIndex]);                      
            //        }
            //        else 
            //            MPTVSeriesLog.Write("Error downloading Artwork: " + f.FullLocalPath);
            //    }
            //} 
            //while (toDownload.Count > 0 && !downloadingWorker.CancellationPending);
        }

        static void setDownloadStatus()
        {
          lock (toDownload)
          {
            if (toDownload.Count > 0)
            {
              MyFilmsDetail.setGUIProperty("cover.downloadingstatus", string.Format("Download Status [{0}]", toDownload.Count));
            }
            else
              MyFilmsDetail.setGUIProperty("cover.downloadingstatus", " ");
          }
        }

        #endregion

        public static int GetWindowID
        { get { return MyFilms.ID_MyFilmsCoverManager; } }
        
        public override int GetID
        { get { return MyFilms.ID_MyFilmsCoverManager; } }

        public int GetWindowId()
        { return MyFilms.ID_MyFilmsCoverManager; }

        public override bool Init()
        {
            String xmlSkin = GUIGraphicsContext.Skin + @"\MyFilmsCoverManager.xml";            
            return Load(xmlSkin);
        }

		/// <summary>
		/// MediaPortal will set #currentmodule with GetModuleName()
		/// </summary>
		/// <returns>Localized Window Name</returns>
		//  public override string GetModuleName() {
		//	return Translation.Artwork;
		//}

        protected View CurrentView
        {
            get { return currentView; }
            set { currentView = value; }
        }

        protected override void OnPageLoad()
        {            
          AllocResources();
          MyFilmsDetail.Searchtitles sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MovieID));
          ArtworkFileName = MyFilmsDetail.GetOrCreateCoverFilename(MyFilms.r, MyFilms.conf.StrIndex, sTitles.MasterTitle);

          MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#currentmodule", "MyFilms Cover Manager");

          loadingWorker = new BackgroundWorker();            
          loadingWorker.WorkerReportsProgress = true;
          loadingWorker.WorkerSupportsCancellation = true;
          loadingWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
          loadingWorker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
          loadingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

          if (m_Facade != null)
          {
            if (initialView == -1)
            {
              initialView = 2; // bigthumbs as default ...
              CurrentView = (View)initialView;
            }
            m_Facade.CurrentLayout = (GUIFacadeControl.Layout)CurrentView;                
          }            

          base.OnPageLoad();
          
          // update skin controls
          UpdateLayoutButton();
          if (labelResolution != null) labelResolution.Label = "LabelResolution";
          if (buttonFilters != null) buttonFilters.Label = "ArtworkFilter";

          ClearProperties();
          UpdateFilterProperty(false);

          setDownloadStatus();

          string movielabel = MyFilms.r[MovieID][MyFilms.conf.StrTitle1].ToString();
          if (!string.IsNullOrEmpty(MyFilms.r[MovieID][MyFilms.conf.StrTitle2].ToString()))
            movielabel += " (" + MyFilms.r[MovieID][MyFilms.conf.StrTitle2] + ")";
          if (!string.IsNullOrEmpty(MyFilms.r[MovieID]["Year"].ToString()))
            movielabel += " - [" + MyFilms.r[MovieID]["Year"].ToString() + "]";
          MovieLabel = movielabel;

          MyFilmsDetail.setGUIProperty("cover.currentmoviename", MovieLabel);
          if (!string.IsNullOrEmpty(MyFilms.r[MovieID]["Picture"].ToString())) // only set preview, if DB is not empty ...
            MyFilmsDetail.setGUIProperty("picture", ArtworkFileName);
          LogMyFilms.Debug("Cover Manager Window initializing");            
             
          loadingWorker.RunWorkerAsync(this.MovieID);            
          downloadingWorker.ProgressChanged += new ProgressChangedEventHandler(downloadingWorker_ProgressChanged);            
        }

        protected bool AllowView(View view)
        {
            if (view == View.List)
                return false;

            if (view == View.AlbumView)
                return false;

            if (view == View.PlayList)
                return false;
            
            return true;
        }

        private void UpdateLayoutButton()
        {
            string strLine = string.Empty;
            View view = CurrentView;
            switch (view)
            {
                case View.List:
                    strLine = MediaPortal.GUI.Library.GUILocalizeStrings.Get(101);
                    break;
                case View.Icons:
                    strLine = MediaPortal.GUI.Library.GUILocalizeStrings.Get(100);
                    break;
                case View.LargeIcons:
                    strLine = MediaPortal.GUI.Library.GUILocalizeStrings.Get(417);
                    break;
                case View.FilmStrip:
                    strLine = MediaPortal.GUI.Library.GUILocalizeStrings.Get(733);
                    break;
                case View.PlayList:
                    strLine = MediaPortal.GUI.Library.GUILocalizeStrings.Get(101);
                    break;
            }
            if (buttonLayouts != null)
                GUIControl.SetControlLabel(GetID, buttonLayouts.GetID, strLine);
        }

        private void ClearProperties()
        {
            MyFilmsDetail.clearGUIProperty("cover.currentmoviename");
            MyFilmsDetail.clearGUIProperty("cover.count");
            MyFilmsDetail.clearGUIProperty("cover.loadingstatus");
            MyFilmsDetail.clearGUIProperty("cover.selectedcoverresolution");
            MyFilmsDetail.clearGUIProperty("cover.selectedcoverresolutionclass");
            MyFilmsDetail.clearGUIProperty("cover.selectedcovername");

            MyFilmsDetail.clearGUIProperty("cover.selectedcoversize");
            MyFilmsDetail.clearGUIProperty("cover.selectedcoversizenum");

        }

        private void UpdateFilterProperty(bool btnEnabled)
        {
            if (buttonFilters != null)
                buttonFilters.IsEnabled = btnEnabled;

            string resolution = (string.IsNullOrEmpty(DisplayFilter)) ? "All" : DisplayFilter;
            MyFilmsDetail.setGUIProperty("cover.filterresolution", resolution);            
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
          MyFilmsDetail.setGUIProperty("cover.currentmoviename", MovieLabel);
          MyFilmsDetail.setGUIProperty("cover.loadingstatus", string.Empty);
          MyFilmsDetail.setGUIProperty("cover.count", m_Facade.Count.ToString());

          totalFanart = int.Parse(m_Facade.Count.ToString());
          if (totalFanart == 0)
            {
              MyFilmsDetail.setGUIProperty("cover.loadingstatus", GUILocalizeStrings.Get(10798768)); // no covers found !
                // Enable Filters button in case Artwork is filtered
                if (DisplayFilter != "All" && !string.IsNullOrEmpty(DisplayFilter) && buttonFilters != null)
                {
                    OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
                    OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));                    
                }
            }
            totalFanart = 0;

            // Load the selected facade so it's not black by default
            if (m_Facade != null && m_Facade.SelectedListItem != null && m_Facade.SelectedListItem.TVTag != null)
            {
                if (m_Facade.Count > m_PreviousSelectedItem)
                {
                    if (m_PreviousSelectedItem <= 0)
                        m_Facade.SelectedListItemIndex = 0;
                    else
                        m_Facade.SelectedListItemIndex = m_PreviousSelectedItem;                    

                    // Work around for Filmstrip not allowing to programmatically select item
                    if (m_Facade.CurrentLayout == GUIFacadeControl.Layout.Filmstrip)
                    {
                        m_bQuickSelect = true;
                        for (int i = 0; i < m_PreviousSelectedItem; i++)
                        {
                            OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
                        }
                        m_bQuickSelect = false;
                        // Note: this is better way, but Scroll offset wont work after set
                        //GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, m_Facade.WindowId, 0, m_Facade.FilmstripLayout.GetID, m_PreviousSelectedItem, 0, null);
                        //GUIGraphicsContext.SendMessage(msg);
                        //MPTVSeriesLog.Write("Sending a selection postcard to FilmStrip.", MPTVSeriesLog.LogLevel.Debug);
                    }                   
                    m_PreviousSelectedItem = -1;
                }

                MFCover selectedFanart = m_Facade.SelectedListItem.TVTag as MFCover;
                if (selectedFanart != null)
                {
                    setFanartPreviewBackground(selectedFanart);
                }
            }
            UpdateFilterProperty(true);            
        }

        protected override void OnPageDestroy(int new_windowId)
        {
          // MFCover selectedFanart = m_Facade.SelectedListItem.TVTag as MFCover;
          if (!string.IsNullOrEmpty(NewArtworkFileName) && File.Exists(NewArtworkFileName))
          {
            saveChangesToDB();
          }
          else
          {
            LogMyFilms.Debug("OnPageDestroy - saveChangesToDB() - Cover file does not exist - not saving Cover '" + NewArtworkFileName + "' to DB !");
          }

          if (loadingWorker.IsBusy)
              loadingWorker.CancelAsync();
          while (loadingWorker.IsBusy)
            System.Windows.Forms.Application.DoEvents();

          loadingWorker = null;
          Helper.enableNativeAutoplay();
          base.OnPageDestroy(new_windowId);
        }

        public void setPageTitle(string Title)
        {
          MyFilmsDetail.setGUIProperty("cover.pagetitle", Title);
        }

        #region Context Menu
        protected override void OnShowContextMenu()
        {
            try
            {
                bool bCoverSelected = true;
                MFCover selectedCover = new MFCover();
                GUIListItem currentitem = this.m_Facade.SelectedListItem;
                if (currentitem == null || !(currentitem.TVTag is MFCover))
                  bCoverSelected = false; //return;
                else
                  selectedCover = currentitem.TVTag as MFCover;

                IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading("MyFilms Cover Manager");

                GUIListItem pItem;
                if (bCoverSelected)
                {
                  pItem = new GUIListItem(GUILocalizeStrings.Get(10798769)); // Select Cover as Default
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.UseAsDefault;
                }
                pItem = new GUIListItem(GUILocalizeStrings.Get(10798766));  // Load single Cover ...
                dlg.Add(pItem);
                pItem.ItemId = (int)menuAction.LoadSingle;

                pItem = new GUIListItem(GUILocalizeStrings.Get(10798764)); // Load multiple Covers ...
                dlg.Add(pItem);
                pItem.ItemId = (int)menuAction.LoadMultiple;

                pItem = new GUIListItem(GUILocalizeStrings.Get(10798761)); // Load Covers (TMDB)
                dlg.Add(pItem);
                pItem.ItemId = (int)menuAction.LoadFromTmdb;

                if (MyFilmsDetail.ExtendedStartmode("CoverManager: Creation of Covers from Movie not yet supported"))
                {
                  pItem = new GUIListItem(GUILocalizeStrings.Get(10798728)); // create cover from movie ...
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.CreateFromMovie;

                  pItem = new GUIListItem(GUILocalizeStrings.Get(10798729)); // Create cover from film as mosaic
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.CreateFromMovieAsMosaic;

                }
                if (bCoverSelected)
                {
                  //if (MyFilmsDetail.ExtendedStartmode("CoverManager: Filtering Covers/Artwork not yet supported"))
                  //{
                    if (!loadingWorker.IsBusy) // Dont allowing filtering until DB has all data
                    {
                      pItem = new GUIListItem("Filter ...");
                      dlg.Add(pItem);
                      pItem.ItemId = (int)menuAction.Filters;
                    }
                  //}

                  if (!loadingWorker.IsBusy)
                  {
                    pItem = new GUIListItem(GUILocalizeStrings.Get(10798810)); // Delete Movie Cover from DB
                    dlg.Add(pItem);
                    pItem.ItemId = (int)menuAction.ClearMovieCover;
                  }

                  pItem = new GUIListItem("Delete selected Cover");
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.DeleteSelected;

                  pItem = new GUIListItem("Delete all 'Low'");
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.DeleteAllLow;

                  pItem = new GUIListItem("Delete all 'Medium'");
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.DeleteAllMedium;

                  pItem = new GUIListItem("Delete all 'High'");
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.DeleteAllHigh;

                  pItem = new GUIListItem("Delete all except selected");
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.DeleteAllExceptSelected;
                }

                // lets show it
                dlg.DoModal(GUIWindowManager.ActiveWindow);
                string title = "";
                string mediapath = "";
                MyFilmsDetail.Searchtitles sTitles;
                switch (dlg.SelectedId) // what was chosen?
                {
                  case (int)menuAction.LoadSingle:
                    //downloadFanart(selectedCover);
                    title = MyFilmsDetail.GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
                    mediapath = MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
                    sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);
                    MyFilmsDetail.grabb_Internet_Informations(title, GetID, true, MyFilms.conf.StrGrabber_cnf, mediapath, MyFilmsDetail.GrabType.Cover, false, sTitles);
                    m_Facade.Clear();
                    loadingWorker.RunWorkerAsync(this.MovieID);
                    break;
                  case (int)menuAction.LoadMultiple:
                    //downloadFanart(selectedCover);
                    title = MyFilmsDetail.GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
                    mediapath = MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
                    sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);
                    MyFilmsDetail.grabb_Internet_Informations(title, GetID, true, MyFilms.conf.StrGrabber_cnf, mediapath, MyFilmsDetail.GrabType.MultiCovers, false, sTitles);
                    m_Facade.Clear();
                    loadingWorker.RunWorkerAsync(this.MovieID);
                    break;
                  case (int)menuAction.LoadFromTmdb:
                    //downloadFanart(selectedCover);
                    sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
                    if (string.IsNullOrEmpty(sTitles.FanartTitle) && MyFilms.conf.StrFanart)
                      return;
                    if (MyFilms.conf.StrFanart)
                    {
                      MyFilmsDetail.Download_TMDB_Posters(sTitles.OriginalTitle, sTitles.TranslatedTitle, sTitles.Director, sTitles.year.ToString(), true, GetID, sTitles.OriginalTitle);
                    }
                    m_Facade.Clear();
                    loadingWorker.RunWorkerAsync(this.MovieID);
                    break;
                  case (int)menuAction.CreateFromMovie:
                    //downloadFanart(selectedCover);
                    //ToDo: Add Code for single image thumbnailer
                    m_Facade.Clear();
                    loadingWorker.RunWorkerAsync(this.MovieID);
                    break;
                  case (int)menuAction.CreateFromMovieAsMosaic:
                    //downloadFanart(selectedCover);
                    MyFilmsDetail.CreateThumbFromMovie();
                    m_Facade.Clear();
                    loadingWorker.RunWorkerAsync(this.MovieID);
                    break;
                  case (int)menuAction.DeleteSelected:
                    selectedCover.Delete();
                    // and reinit the display to get rid of it
                    m_Facade.Clear();
                    loadingWorker.RunWorkerAsync(this.MovieID);
                    break;
                  case (int)menuAction.DeleteAllLow:
                    this.DeleteSelectedFromGroup("Low");
                    m_Facade.Clear();
                    loadingWorker.RunWorkerAsync(this.MovieID);
                    break;
                  case (int)menuAction.DeleteAllMedium:
                    this.DeleteSelectedFromGroup("Medium");
                    m_Facade.Clear();
                    loadingWorker.RunWorkerAsync(this.MovieID);
                    break;
                  case (int)menuAction.DeleteAllHigh:
                    this.DeleteSelectedFromGroup("High");
                    m_Facade.Clear();
                    loadingWorker.RunWorkerAsync(this.MovieID);
                    break;
                  case (int)menuAction.DeleteAllExceptSelected:
                    DeleteAllExceptSelected(m_Facade.SelectedListItemIndex);
                    m_Facade.Clear();
                    loadingWorker.RunWorkerAsync(this.MovieID);
                    break;
                  case (int)menuAction.UseAsDefault:
                    SetFacadeItemAsChosen(m_Facade.SelectedListItemIndex);
                    selectedCover.Chosen = true;
                    setDefaultCover(selectedCover);
                    break;
                  case (int)menuAction.Filters:
                    dlg.Reset();
                    ShowFiltersMenu();
                    break;
                  case (int)menuAction.ClearMovieCover:
                    dlg.Reset();
                    RemoveMovieCoverFromDB();
                    m_Facade.Clear();
                    ClearProperties();
                    UpdateFilterProperty(false);
                    loadingWorker.RunWorkerAsync(this.MovieID);
                    break;
                }
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug("Exception in Artwork Chooser Context Menu: " + ex.Message);
              return;
            }
        }
        #endregion


        #region Context Menu - Filters
        private void ShowFiltersMenu()
        {
            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return;

            dlg.Reset();
            dlg.SetHeading("ArtworkFilter");

            GUIListItem pItem = new GUIListItem("All");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.all;

            pItem = new GUIListItem("High");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.high;

            pItem = new GUIListItem("Medium");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.medium;

            pItem = new GUIListItem("Low");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.low;

            //pItem = new GUIListItem("1280x720");
            //dlg.Add(pItem);
            //pItem.ItemId = (int)menuFilterAction.hd;            

            //pItem = new GUIListItem("1920x1080");
            //dlg.Add(pItem);
            //pItem.ItemId = (int)menuFilterAction.fullhd;            
            
            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId >= 0)
            {
                switch (dlg.SelectedId)
                {
                  case (int)menuFilterAction.all:
                      DisplayFilter = "All";
                      break;
                  case (int)menuFilterAction.hd:
                      DisplayFilter = "High";
                      break;
                  case (int)menuFilterAction.fullhd:
                      //DBOption.SetOptions(DBOption.cArtworkThumbnailResolutionFilter, "2");
                      break;
                  case (int)menuFilterAction.high:
                      DisplayFilter = "High";
                      break;
                  case (int)menuFilterAction.medium:
                      DisplayFilter = "Medium";
                      break;
                  case (int)menuFilterAction.low:
                      DisplayFilter = "Low";
                      break;
                  default:
                    DisplayFilter = "";
                    break;
                }
                m_Facade.Clear();
                ClearProperties();

                UpdateFilterProperty(false);
                loadingWorker.RunWorkerAsync(this.MovieID);                   
            }
        }
        #endregion

        int totalFanart;
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try {
                if (m_Facade != null) {
                    GUIListItem loadedItem = e.UserState as GUIListItem;
                    if (loadedItem != null) {                        
                        m_Facade.Add(loadedItem);                   
                        // we use this to tell the gui how many Artwork we are loading
                        MyFilmsDetail.setGUIProperty("cover.loadingstatus", string.Format("ArtworkOnlineLoading", e.ProgressPercentage, totalFanart));
                        MyFilmsDetail.setGUIProperty("cover.count", e.ProgressPercentage.ToString());
                        if (m_Facade != null) 
                          this.m_Facade.Focus = true;
                    }
                    else if (e.ProgressPercentage > 0) {
                        // we use this to tell the gui how many Artwork we are loading
                      MyFilmsDetail.setGUIProperty("cover.loadingstatus", string.Format("ArtworkOnlineLoading", 0, e.ProgressPercentage));
                        totalFanart = e.ProgressPercentage;
                    }
                }
            }
            catch (Exception ex) {
                LogMyFilms.Debug("Error: Artwork Chooser worker_ProgressChanged() experienced an error: " + ex.Message);               
            }
        }


        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadThumbnails((int)e.Argument);
        }

        public int MovieID
        {
          get { return this.movieId; }
          set { this.movieId = value; }
        }

        public string MovieLabel
        {
          get { return this.movieLabel; }
          set { this.movieLabel = value; }
        }

        public string ArtworkFileName
        {
          get { return this.artworkFileName; }
          set { this.artworkFileName = value; }
        }

        public string NewArtworkFileName
        {
          get { return this.newArtworkFileName; }
          set { this.newArtworkFileName = value; }
        }

        public string DisplayFilter
        {
          get { return this.displayFilter; }
          set { this.displayFilter = value; }
        }

        public override bool OnMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                // Can't use OnMessage when using Filmstrip - it doesn't work!!
                case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED: {
                    int iControl = message.SenderControlId;
                    if (iControl == (int)m_Facade.GetID && m_Facade.SelectedListItem != null) {
                      MFCover selectedFanart = m_Facade.SelectedListItem.TVTag as MFCover;
                        if (selectedFanart != null) {
                            setFanartPreviewBackground(selectedFanart);
                        }
                    }
                    return true;
                } 
                default:
                    return base.OnMessage(message);
            }
        }

        private void onFacadeItemSelected(GUIListItem item, GUIControl parent) // triggered when a selection change was made on the facade
        {
            if (m_bQuickSelect) return;

            // if this is not a message from the facade, exit
            if (parent != m_Facade && parent != m_Facade.FilmstripLayout &&
                parent != m_Facade.ThumbnailLayout && parent != m_Facade.ListLayout)
                return;

            MFCover selectedCover = item.TVTag as MFCover;
            if (selectedCover != null)
            {
                setFanartPreviewBackground(selectedCover);
            }      
            
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            if (control == buttonFilters)
            {
                ShowFiltersMenu();
                buttonFilters.Focus = false;
                return;
            }
            
            if (control == buttonLayouts)
            {
                bool shouldContinue = false;
                do
                {
                    shouldContinue = false;
                    switch (CurrentView)
                    {
                        case View.List:
                            CurrentView = View.PlayList;
                            if (!AllowView(CurrentView) || m_Facade.PlayListLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.Playlist;
                            }
                            break;

                        case View.PlayList:
                            CurrentView = View.Icons;
                            if (!AllowView(CurrentView) || m_Facade.ThumbnailLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
                            }
                            break;

                        case View.Icons:
                            CurrentView = View.LargeIcons;
                            if (!AllowView(CurrentView) || m_Facade.ThumbnailLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
                            }
                            break;

                        case View.LargeIcons:
                            CurrentView = View.FilmStrip;
                            if (!AllowView(CurrentView) || m_Facade.FilmstripLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
                            }
                            break;

                        case View.FilmStrip:
                            CurrentView = View.List;
                            if (!AllowView(CurrentView) || m_Facade.ListLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.List;
                            }
                            break;
                    }
                } while (shouldContinue);
                UpdateLayoutButton();
                GUIControl.FocusControl(GetID, controlId);
            }

            if (actionType != Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?
            if (control == this.m_Facade)
            {
              MFCover chosen;
              if ((chosen = this.m_Facade.SelectedListItem.TVTag as MFCover) != null)
                {
                    SetFacadeItemAsChosen(m_Facade.SelectedListItemIndex);
                    chosen.Chosen = true; // if we already have it, we simply set the chosen property (will itself "unchoose" all the others)
                    setDefaultCover(chosen);
                }
            }
        }


        void RemoveMovieCoverFromDB()
        {
          NewArtworkFileName = "";
          MyFilmsDetail.Searchtitles sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MovieID], MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MovieID));
          ArtworkFileName = MyFilmsDetail.GetOrCreateCoverFilename(MyFilms.r, MovieID, sTitles.MasterTitle);
          MyFilmsDetail.clearGUIProperty("picture");
          LogMyFilms.Debug("RemoveMovieCoverFromDB() - Removed Cover from DB - new Covername (empty) = '" + ArtworkFileName + "'");
          saveChangesToDB();
        }

        void SetFacadeItemAsChosen(int iSelectedItem)
        {
          try
          {
            for (int i = 0; i < m_Facade.Count; i++)
            {
              if (i == iSelectedItem)
                m_Facade[i].IsRemote = true;
              else
              {
                m_Facade[i].IsRemote = false;
                MFCover item;
                item = m_Facade[i].TVTag as MFCover;
                item.Chosen = false;
              }
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("Failed to set Facade Item as chosen: " + ex.Message);
          }
        }

        void DeleteSelectedFromGroup(string strQuality)
        {
          try
          {
            for (int i = 0; i < m_Facade.Count; i++)
            {
                MFCover item;
                item = m_Facade[i].TVTag as MFCover;
                if (item != null && item.ImageResolutionClass == strQuality)
                  item.Delete();
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("Failed to set Facade Item as chosen: " + ex.Message);
          }
        }

        void DeleteAllExceptSelected(int iSelectedItem)
        {
          try
          {
            for (int i = 0; i < m_Facade.Count; i++)
            {
              if (i != iSelectedItem)
              {
                MFCover item;
                item = m_Facade[i].TVTag as MFCover;
                if (item != null)
                  item.Delete();
              }
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("Failed to set Facade Item as chosen: " + ex.Message);
          }
        }

        void downloadFanart(MFCover fanart)
        {
            // we need to get it, let's queue them up and download in the background
            lock (toDownload)
            {
                toDownload.Enqueue(fanart);
            }
            setDownloadStatus();
            // don't return, user can queue up multiple Artwork to download
            // the last he selects to download will be the chosen one by default

            // finally lets check if the downloader is already running, and if not start it
            if (!downloadingWorker.IsBusy)
                downloadingWorker.RunWorkerAsync();
        }

        void loadThumbnails(int MovieID)
        {
            if (MovieID > -1)
            {                
                if (loadingWorker.CancellationPending)
                    return;

                GUIListItem item = null;
                List<MFCover> covers = GetLocalCover(MyFilms.r, MyFilms.conf.StrIndex, ArtworkFileName);

                int i = 0;
                foreach (MFCover f in covers)
                {
                  bool add = false;
                  switch (DisplayFilter)
                  {
                    case "High":
                      if (f.ImageResolutionClass == "High")
                        add = true;
                      break;
                    case "Medium":
                      if (f.ImageResolutionClass == "Medium")
                        add = true;
                      break;
                    case "Low":
                      if (f.ImageResolutionClass == "Low")
                        add = true;
                      break;
                    default:
                      add = true;
                      break;
                  }

                  if (add)
                  {
                    item = new GUIListItem(f.ImageResolution);
                    item.IsRemote = false;
                    item.TVTag = f;
                    // item.IconImage = item.IconImageBig = ImageAllocator.GetOtherImage(filename, new System.Drawing.Size(0, 0), false);
                    item.IconImage = item.IconImageBig = f.FullPath;

                    item.OnItemSelected += new GUIListItem.ItemSelectedHandler(onFacadeItemSelected);
                  }
                  loadingWorker.ReportProgress((i < 100 ? ++i : 100), item);
                  if (loadingWorker.CancellationPending)
                    return;
                }
            }
        }

        void setFanartPreviewBackground(MFCover cover)
        {
          MyFilmsDetail.setGUIProperty("cover.selectedcoverresolution", cover.ImageResolution);
          MyFilmsDetail.setGUIProperty("cover.selectedcoverresolutionclass", cover.ImageResolutionClass);
          MyFilmsDetail.setGUIProperty("cover.selectedcoversize", cover.ImageSizeFriendly);
          MyFilmsDetail.setGUIProperty("cover.selectedcoversizenum", cover.ImageSize.ToString());
          MyFilmsDetail.setGUIProperty("cover.selectedcovername", cover.FileName);

          string preview = string.Empty;
          //if (ImageAllocator.LoadImageFastFromFile(cover.FullPath) == null) // Ensure Artwork on Disk is valid as well
          //{
          //  LogMyFilms.Debug("Artwork is invalid, deleting...");
          //  cover.Delete();
          //  cover.Chosen = false;
          //  m_Facade.SelectedListItem.Label = "Deleted";
          //}

          //// Should be safe to assign fullsize Artwork if available
          //preview = ImageAllocator.GetOtherImage(cover.FullPath, default(System.Drawing.Size), false);
          preview = m_Facade.SelectedListItem.IconImageBig;
          MyFilmsDetail.setGUIProperty("cover.selectedpreview", preview);
        }

        void setDefaultCover(MFCover cover)
        {
          if (!File.Exists(cover.FullPath))
            NewArtworkFileName = "";
          else
            NewArtworkFileName = cover.FullPath;
          MyFilmsDetail.setGUIProperty("picture", NewArtworkFileName);
          return;

          //MyFilmsDetail.Searchtitles sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
          //string currentPicture = MyFilmsDetail.getGUIProperty("picture");
          ////if ((r1[Index]["Picture"].ToString().IndexOf(":\\") == -1) && (r1[Index]["Picture"].ToString().Substring(0, 2) != "\\\\"))
          ////  MyFilms.conf.FileImage = MyFilms.conf.StrPathImg + "\\" + r1[Index]["Picture"].ToString();
          ////else
          ////  MyFilms.conf.FileImage = r1[Index]["Picture"].ToString();
          //if ((MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().IndexOf(":\\") == -1) && (MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString().Substring(0, 2) != "\\\\"))
          //  currentPicture = MyFilms.conf.StrPathImg + "\\" + MyFilms.r[MyFilms.conf.StrIndex]["Picture"];
          //else
          //  currentPicture = MyFilms.r[MyFilms.conf.StrIndex]["Picture"].ToString();

          //if (string.IsNullOrEmpty(currentPicture))
          //  currentPicture = sTitles.FanartTitle + ".jpg";

          //LogMyFilms.Debug("setDefaultCover - currentPicture = '" + currentPicture + "'");
          //if (System.IO.File.Exists(currentPicture))
          //{
          //  //string safeName = Grabber.GrabUtil.CreateFilename(title);
          //  //filename = dirname + safeName + " [" + imageUrl.GetHashCode() + "].jpg";

          //  string newFile = System.IO.Path.GetFileNameWithoutExtension(currentPicture) + " [" + currentPicture.GetHashCode() + "]." + System.IO.Path.GetExtension(currentPicture);
          //  LogMyFilms.Debug("setDefaultCover - newFile = '" + newFile + "'");
          //  try
          //  {
          //    System.IO.File.Copy(currentPicture, newFile, true);
          //    File.Delete(currentPicture);
          //  }
          //  catch (Exception ex)
          //  {
          //    LogMyFilms.Debug("setDefaultCover - error renaming currentPicture: " + ex.Message + ex.StackTrace);
          //  }
          //}
          //try
          //{
          //  File.Copy(cover.FullPath, currentPicture, true);
          //  File.Delete(cover.FullPath);
          //  LogMyFilms.Debug("setDefaultCover - sucessfully set new Default Cover: '" + currentPicture + "'");
          //}
          //catch (Exception ex)
          //{
          //  LogMyFilms.Debug("setDefaultCover - error renaming currentPicture: " + ex.Message + ex.StackTrace);
          //}
        }

        void saveChangesToDB()
        {
          string newPictureCatalogname = string.IsNullOrEmpty(NewArtworkFileName) ? "" : MyFilmsDetail.GetPictureCatalogNameFromFilename(NewArtworkFileName);
          MyFilms.r[MovieID]["Picture"] = newPictureCatalogname;
          MyFilmsDetail.Update_XML_database();
          LogMyFilms.Info("saveChangesToDB() - Database updated with '" + newPictureCatalogname + "' for Cover file '" + NewArtworkFileName + "'");
          //MyFilmsDetail.afficher_detail(true);
          return;
        }

        //-------------------------------------------------------------------------------------------
        //  Get local Cover Image
        //-------------------------------------------------------------------------------------------        
        public static List<MFCover> GetLocalCover(DataRow[] r1, int Index, string currentPictureFilename)
        {
          List<MFCover> result = new List<MFCover>();
          string directoryname = "";
          string[] files = null;
          Int64 wsize = 0; // Temporary Filesize detection
          //string currentPicture = MyFilmsDetail.getGUIProperty("picture");
          
          //if ((r1[Index]["Picture"].ToString().IndexOf(":\\") == -1) && (r1[Index]["Picture"].ToString().Substring(0, 2) != "\\\\"))
          //  currentPictureFilename = MyFilms.conf.StrPathImg + "\\" + r1[Index]["Picture"];
          //else
          //  currentPictureFilename = r1[Index]["Picture"].ToString();

          if (string.IsNullOrEmpty(currentPictureFilename))
            return null;
          string searchPattern = GetCoverSearchPattern(currentPictureFilename);
          if (searchPattern == null)
            return null;

          directoryname = currentPictureFilename.Substring(0, currentPictureFilename.LastIndexOf("\\"));
          LogMyFilms.Debug("CoverManager - searchPattern = '" + searchPattern + "', directoryname = '" + directoryname + "'");

          if (!string.IsNullOrEmpty(directoryname))
          {
            files = Directory.GetFiles(directoryname, @"*" + searchPattern + @"*.jpg", SearchOption.TopDirectoryOnly);
            foreach (string filefound in files)
            {
              wsize = new System.IO.FileInfo(filefound).Length;
              MFCover item = new MFCover();
              item.FileName = System.IO.Path.GetFileNameWithoutExtension(filefound);
              item.FullPath = filefound;

              //if (!File.Exists(localFile) || ImageFast.FastFromFile(localFile) == null) {}
              Image newCover = Image.FromFile(filefound);
              item.ImageResolution = newCover.Width + " x " + newCover.Height;
              item.ImageWith = newCover.Width;
              item.ImageHeight = newCover.Height;

              if (newCover.Height > 800) item.ImageResolutionClass = "High";
              else if (newCover.Height > 400) item.ImageResolutionClass = "Medium";
              else if (newCover.Height > 0) item.ImageResolutionClass = "Low";
              else item.ImageResolutionClass = "Unknown";
              newCover.Dispose();
              
              item.ImageSize = new System.IO.FileInfo(filefound).Length;
              item.ImageSizeFriendly = Helper.GetFileSize(wsize);
              result.Add(item);
            }
          }
          return result;
        }

        private static string GetCoverSearchPattern(string currentPictureFilename)
        {
          string startPattern = "";
          string currentStorePath = MyFilms.conf.StrPathImg + "\\" + MyFilms.conf.StrPicturePrefix;

          if (!currentPictureFilename.StartsWith(currentStorePath))
            return null;
          startPattern = (currentStorePath.EndsWith("\\")) ? "" : currentPictureFilename.Substring(currentPictureFilename.LastIndexOf("\\") + 1);
          string searchPattern = currentPictureFilename.Substring(currentStorePath.Length);

          int patternLength2 = 999;
          int patternLength = searchPattern.LastIndexOf(".");
          if (searchPattern.Contains("["))
            patternLength2 = searchPattern.LastIndexOf("[") - 1;
          if (patternLength2 < patternLength)
            patternLength = patternLength2;
          searchPattern = searchPattern.Substring(0, patternLength);
          return searchPattern;
        }
    }

    public class MFCover
    {
      private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
      
      public MFCover() {}

      public void Delete()
      {
        try
        {
          System.IO.File.Delete(FullPath);
          LogMyFilms.Debug("Artwork Deleted: " + FullPath);
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug("Failed to delete file: " + FullPath + " (" + ex.Message + ")");
        }
      }

      enum Quality
      {
        Unknown,
        Low,
        Medium,
        High
      }

      private string fullPath;
      public string FullPath
      {
        get { return fullPath; }
        set { fullPath = value; }
      }

      private string fileName;
      public string FileName
      {
        get { return fileName; }
        set { fileName = value; }
      }

      private long imageSize;
      public long ImageSize
      {
        get { return imageSize; }
        set { imageSize = value; }
      }

      private string imageSizeFriendly;
      public string ImageSizeFriendly
      {
        get { return imageSizeFriendly; }
        set { imageSizeFriendly = value; }
      }

      private int imageWith;
      public int ImageWith
      {
        get { return imageWith; }
        set { imageWith = value; }
      }

      private int imageHeight;
      public int ImageHeight
      {
        get { return imageHeight; }
        set { imageHeight = value; }
      }

      private string imageResolution;
      public string ImageResolution
      {
        get { return imageResolution; }
        set { imageResolution = value; }
      }

      private string imageResolutionClass;
      public string ImageResolutionClass
      {
        get { return imageResolutionClass; }
        set { imageResolutionClass = value; }
      }

      private bool chosen;
      public bool Chosen
      {
        get { return chosen; }
        set { chosen = value; }
      }

    }

}
