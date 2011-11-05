#region GNU license
// MP-TVSeries - Plugin for Mediaportal
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
using MyFilmsPlugin.MyFilms.Utils;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
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

        [SkinControlAttribute(13)]
        protected GUIToggleButtonControl togglebuttonRandom = null;

        [SkinControlAttribute(14)]
        protected GUILabelControl labelDisabled = null;

        [SkinControlAttribute(15)]
        protected GUILabelControl labelChosen = null;

        enum menuAction
        {
            use,
            download,
            delete,
            optionRandom,
            disable,
            enable,
            filters,
            interval,
            clearcache
        }

        enum menuFilterAction
        {
            all,
            hd,
            fullhd
        }

        enum menuIntervalAction {
            FiveSeconds,
            TenSeconds,
            FifteenSeconds,
            ThirtySeconds,
            FortyFiveSeconds,
            SixtySeconds
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
        int seriesID = -1;
        BackgroundWorker loadingWorker = null; // to fetch list and thumbnails
        public static BackgroundWorker downloadingWorker = new BackgroundWorker(); // to do the actual downloading
        static Queue<DBFanart> toDownload = new Queue<DBFanart>();
        int m_PreviousSelectedItem = -1;
        private View currentView = View.LargeIcons;
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
                loadingWorker.RunWorkerAsync(SeriesID);
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
            //lock (toDownload)
            //{
            //    if (toDownload.Count > 0)
            //    {
            //        TVSeriesPlugin.setGUIProperty("Artwork.DownloadingStatus", string.Format(Translation.FanDownloadingStatus, toDownload.Count));                    
            //    }
            //    else                
            //        TVSeriesPlugin.setGUIProperty("Artwork.DownloadingStatus", " ");
            //}
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
		//public override string GetModuleName() {
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

            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#currentmodule", "MyFilmsCoverManager");

            loadingWorker = new BackgroundWorker();            
            loadingWorker.WorkerReportsProgress = true;
            loadingWorker.WorkerSupportsCancellation = true;
            loadingWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
            loadingWorker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            loadingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            if (m_Facade != null)
            {
                int defaultView = 2;
                //if (int.TryParse(DBOption.GetOptions(DBOption.cFanartCurrentView), out defaultView))
                {
                    CurrentView = (View)defaultView;                    
                }                
                m_Facade.CurrentLayout = (GUIFacadeControl.Layout)CurrentView;                
            }            

            base.OnPageLoad();
            
            // update skin controls
            UpdateLayoutButton();
            if (labelResolution != null) labelResolution.Label = "LabelResolution";
            if (labelChosen != null) labelChosen.Label = "LabelChosen";
            if (labelDisabled != null) labelDisabled.Label = "LabelDisabled";            
            if (buttonFilters != null) buttonFilters.Label = "ArtworkFilter";
            if (togglebuttonRandom != null)
            {
                togglebuttonRandom.Label = "ButtonRandomArtwork";
              togglebuttonRandom.Selected = true; // DBOption.GetOptions(DBOption.cFanartRandom);
            }

            ClearProperties();
            UpdateFilterProperty(false);

            setDownloadStatus();
			
            LogMyFilms.Debug("Artwork Chooser Window initializing");            
               
            fetchList(SeriesID);
            loadingWorker.RunWorkerAsync(SeriesID);            

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
            MyFilmsDetail.setGUIProperty("Artwork.Count", " ");
            MyFilmsDetail.setGUIProperty("Artwork.LoadingStatus", " ");
            MyFilmsDetail.setGUIProperty("Artwork.SelectedArtworkInfo", " ");
            MyFilmsDetail.setGUIProperty("Artwork.SelectedArtworkResolution", " ");
            MyFilmsDetail.setGUIProperty("Artwork.SelectedArtworkIsChosen", " ");
            MyFilmsDetail.setGUIProperty("Artwork.SelectedArtworkIsDisabled", " ");
            MyFilmsDetail.setGUIProperty("Artwork.SelectedArtworkColors", " ");            
        }

        private void UpdateFilterProperty(bool btnEnabled)
        {
            if (buttonFilters != null)
                buttonFilters.IsEnabled = btnEnabled;

            string resolution = string.Empty;
            //if (DBOption.GetOptions(DBOption.cArtworkThumbnailResolutionFilter) == "0")
            //{
            //    resolution = Translation.ArtworkFilterAll;
            //}
            //else if (DBOption.GetOptions(DBOption.cFanartThumbnailResolutionFilter) == "1")
            //{
            //    resolution = "1280x720";
            //}
            //else
            //    resolution = "1920x1080";

            resolution = "ArtworkFilterAll";
            MyFilmsDetail.setGUIProperty("Artwork.FilterResolution", resolution);            
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
          MyFilmsDetail.setGUIProperty("Artwork.LoadingStatus", string.Empty);
          MyFilmsDetail.setGUIProperty("Artwork.Count", totalFanart.ToString());

            if (totalFanart == 0)
            {
              MyFilmsDetail.setGUIProperty("Artwork.LoadingStatus", "ArtworkNoneFound");
                // Enable Filters button in case Artwork is filtered
              //if (DBOption.GetOptions(DBOption.cArtworkThumbnailResolutionFilter) != "0" && buttonFilters != null)
                if (buttonFilters != null)
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


                DBFanart selectedFanart = m_Facade.SelectedListItem.TVTag as DBFanart;
                if (selectedFanart != null)
                {
                    setFanartPreviewBackground(selectedFanart);
                }
            }
            UpdateFilterProperty(true);            
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            //DBOption.SetOptions(DBOption.cFanartCurrentView, (int)CurrentView);

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
          MyFilmsDetail.setGUIProperty("Artwork.PageTitle", Title);
        }

        #region Context Menu
        protected override void OnShowContextMenu()
        {
            try
            {
                GUIListItem currentitem = this.m_Facade.SelectedListItem;
                if (currentitem == null || !(currentitem.TVTag is DBFanart)) return;
                DBFanart selectedFanart = currentitem.TVTag as DBFanart;

                IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading("MyFilmsCoverManager");

                //GUIListItem pItem;
                //if (DBOption.GetOptions(DBOption.cFanartRandom))
                //{
                //    // if random it doesnt make sense to offer an option to explicitally use an available Artwork
                //    if (!selectedFanart.isAvailableLocally)
                //    {
                //        pItem = new GUIListItem(Translation.ArtworkGetAndUse);
                //        dlg.Add(pItem);
                //        pItem.ItemId = (int)menuAction.download;
                //    }        
                //}
                //else
                //{
                //    // if we are not random, we can choose available Artwork
                //    if (selectedFanart.isAvailableLocally && !selectedFanart.Disabled)
                //    {
                //        pItem = new GUIListItem(Translation.ArtworkUse);
                //        dlg.Add(pItem);
                //        pItem.ItemId = (int)menuAction.use;
                //    }
                //    else if (!selectedFanart.isAvailableLocally)
                //    {
                //        pItem = new GUIListItem(Translation.ArtworkGet);
                //        dlg.Add(pItem);
                //        pItem.ItemId = (int)menuAction.download;
                //    }
                //}

                //if (selectedFanart.isAvailableLocally)
                //{
                //    if (selectedFanart.Disabled)
                //    {
                //        pItem = new GUIListItem(Translation.ArtworkMenuEnable);
                //        dlg.Add(pItem);
                //        pItem.ItemId = (int)menuAction.enable;
                //    }
                //    else
                //    {
                //        pItem = new GUIListItem(Translation.ArtworkMenuDisable);
                //        dlg.Add(pItem);
                //        pItem.ItemId = (int)menuAction.disable;
                //    }
                //}

                //pItem = new GUIListItem(Translation.ArtworkRandom + " (" + (DBOption.GetOptions(DBOption.cFanartRandom) ? Translation.on : Translation.off) + ")");
                //dlg.Add(pItem);
                //pItem.ItemId = (int)menuAction.optionRandom;

                //// Dont allowing filtering until DB has all data
                //if (!loadingWorker.IsBusy)
                //{
                //    pItem = new GUIListItem(Translation.ArtworkFilter + " ...");
                //    dlg.Add(pItem);
                //    pItem.ItemId = (int)menuAction.filters;
                //}

                //pItem = new GUIListItem(Translation.ArtworkRandomInterval + " ...");
                //dlg.Add(pItem);
                //pItem.ItemId = (int)menuAction.interval;

                //if (!loadingWorker.IsBusy) {
                //    pItem = new GUIListItem(Translation.ClearArtworkCache);
                //    dlg.Add(pItem);
                //    pItem.ItemId = (int)menuAction.clearcache;
                //}

                //// lets show it
                //dlg.DoModal(GUIWindowManager.ActiveWindow);
                //switch (dlg.SelectedId) // what was chosen?
                //{
                //    case (int)menuAction.delete:
                //        if (selectedFanart.isAvailableLocally)
                //        {                            
                //            selectedFanart.Delete();  
                //            // and reinit the display to get rid of it
                //            m_Facade.Clear();
                //            loadingWorker.RunWorkerAsync(SeriesID);
                //        }
                //        break;
                //    case (int)menuAction.download:
                //        if (!selectedFanart.isAvailableLocally)
                //            downloadFanart(selectedFanart);                        
                //        break;
                //    case (int)menuAction.use:
                //        if (selectedFanart.isAvailableLocally)
                //        {                           
                //            TVSeriesPlugin.setGUIProperty("Artwork.SelectedArtworkIsChosen", Translation.Yes);
                //            SetFacadeItemAsChosen(m_Facade.SelectedListItemIndex);

                //            selectedFanart.Chosen = true;
                //            Fanart.RefreshFanart(SeriesID);
                //        }                        
                //        break;
                //    case (int)menuAction.optionRandom:
                //        DBOption.SetOptions(DBOption.cFanartRandom, !DBOption.GetOptions(DBOption.cFanartRandom));
                //        if (togglebuttonRandom != null)
                //            togglebuttonRandom.Selected = DBOption.GetOptions(DBOption.cFanartRandom);
                //        break;
                //    case (int)menuAction.disable:
                //        selectedFanart.Disabled = true;
                //        selectedFanart.Chosen = false;
                //        currentitem.Label = Translation.FanartDisableLabel;
                //        TVSeriesPlugin.setGUIProperty("Artwork.SelectedArtworkIsDisabled", Translation.Yes);
                //        TVSeriesPlugin.setGUIProperty("Artwork.SelectedArtworkIsChosen", Translation.No);
                //        break;
                //    case (int)menuAction.enable:
                //        selectedFanart.Disabled = false;                        
                //        currentitem.Label = Translation.FanArtLocal;
                //        TVSeriesPlugin.setGUIProperty("Artwork.SelectedArtworkIsDisabled", Translation.No);
                //        break;
                //    case (int)menuAction.filters:
                //        dlg.Reset();
                //        ShowFiltersMenu();
                //        break;
                //    case (int)menuAction.interval:
                //        dlg.Reset();
                //        ShowIntervalMenu();
                //        break;
                //    case (int)menuAction.clearcache:
                //        dlg.Reset();
                //        Fanart.ClearFanartCache(SeriesID);                                               
                //        m_Facade.Clear();                                  
                //        fetchList(SeriesID);
                //        loadingWorker.RunWorkerAsync(SeriesID);
                //        break;

                //}
            }
            catch (Exception ex)
            {
                LogMyFilms.Debug("Exception in Artwork Chooser Context Menu: " + ex.Message);
                return;
            }
        }
        #endregion

        #region Context Menu - Random Artwork Interval
        private void ShowIntervalMenu() {
            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return;

            dlg.Reset();
            dlg.SetHeading("ArtworkRandomInterval");

            GUIListItem pItem = new GUIListItem("ArtworkIntervalFiveSeconds");            
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.FiveSeconds;

            pItem = new GUIListItem("ArtworkIntervalTenSeconds");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.TenSeconds;

            pItem = new GUIListItem("ArtworkIntervalFifteenSeconds");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.FifteenSeconds;

            pItem = new GUIListItem("ArtworkIntervalThirtySeconds");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.ThirtySeconds;

            pItem = new GUIListItem("ArtworkIntervalFortyFiveSeconds");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.FortyFiveSeconds;

            pItem = new GUIListItem("ArtworkIntervalSixtySeconds");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.SixtySeconds;

            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId >= 0) {
                switch (dlg.SelectedId) {
                    case (int)menuIntervalAction.FiveSeconds:
                        //DBOption.SetOptions(DBOption.cRandomFanartInterval, "5000");
                        break;
                    case (int)menuIntervalAction.TenSeconds:
                        break;
                    case (int)menuIntervalAction.FifteenSeconds:
                        break;
                    case (int)menuIntervalAction.ThirtySeconds:
                        break;
                    case (int)menuIntervalAction.FortyFiveSeconds:
                        break;
                    case (int)menuIntervalAction.SixtySeconds:
                        break;                    
                }               
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

            GUIListItem pItem = new GUIListItem("ArtworkFilterAll");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.all;            

            pItem = new GUIListItem("1280x720");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.hd;            

            pItem = new GUIListItem("1920x1080");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.fullhd;            
            
            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId >= 0)
            {
                switch (dlg.SelectedId)
                {
                    case (int)menuFilterAction.all:
                        //DBOption.SetOptions(DBOption.cArtworkThumbnailResolutionFilter, "0");
                        break;
                    case (int)menuFilterAction.hd:
                        //DBOption.SetOptions(DBOption.cArtworkThumbnailResolutionFilter, "1");
                        break;
                    case (int)menuFilterAction.fullhd:
                        //DBOption.SetOptions(DBOption.cArtworkThumbnailResolutionFilter, "2");
                        break;                  
                }
                m_Facade.Clear();
                //DBFanart.ClearAll();
                ClearProperties();

                UpdateFilterProperty(false);
                loadingWorker.RunWorkerAsync(SeriesID);                   
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
                        MyFilmsDetail.setGUIProperty("Artwork.LoadingStatus", string.Format("ArtworkOnlineLoading", e.ProgressPercentage, totalFanart));
                        MyFilmsDetail.setGUIProperty("Artwork.Count", e.ProgressPercentage.ToString());
                        if (m_Facade != null) this.m_Facade.Focus = true;
                    }
                    else if (e.ProgressPercentage > 0) {
                        // we use this to tell the gui how many Artwork we are loading
                      MyFilmsDetail.setGUIProperty("Artwork.LoadingStatus", string.Format("ArtworkOnlineLoading", 0, e.ProgressPercentage));
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

        public int SeriesID
        { 
            get { return seriesID; }
            set { seriesID = value; }
        }

        public override bool OnMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                // Can't use OnMessage when using Filmstrip - it doesn't work!!
                case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED: {
                    int iControl = message.SenderControlId;
                    if (iControl == (int)m_Facade.GetID && m_Facade.SelectedListItem != null) {
                        DBFanart selectedFanart = m_Facade.SelectedListItem.TVTag as DBFanart;
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

        // triggered when a selection change was made on the facade
        private void onFacadeItemSelected(GUIListItem item, GUIControl parent)
        {
            if (m_bQuickSelect) return;

            // if this is not a message from the facade, exit
            if (parent != m_Facade && parent != m_Facade.FilmstripLayout &&
                parent != m_Facade.ThumbnailLayout && parent != m_Facade.ListLayout)
                return;
           
            DBFanart selectedFanart = item.TVTag as DBFanart;
            if (selectedFanart != null)
            {
                setFanartPreviewBackground(selectedFanart);
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
            
            if (control == togglebuttonRandom)
            {
                //DBOption.SetOptions(DBOption.cFanartRandom, togglebuttonRandom.Selected);
                togglebuttonRandom.Focus = false;
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
                DBFanart chosen;
                if ((chosen = this.m_Facade.SelectedListItem.TVTag as DBFanart) != null)
                {
                    if (chosen.isAvailableLocally)
                    {
                      MyFilmsDetail.setGUIProperty("Artwork.SelectedArtworkIsChosen", "Yes");                        
                        SetFacadeItemAsChosen(m_Facade.SelectedListItemIndex);
                        
                        // if we already have it, we simply set the chosen property (will itself "unchoose" all the others)
                        chosen.Chosen = true;
                        // ZF: be sure to update the list of downloaded data in the cache - otherwise the selected Artwork won't show up for new Artworks until restarted
                        //Fanart.RefreshFanart(SeriesID);                        

                    }
                    else if (!chosen.isAvailableLocally)
                    {
                        downloadFanart(chosen);
                    }
                }
            }
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
                        DBFanart item;
                        item = m_Facade[i].TVTag as DBFanart;
                        item.Chosen = false;
                    }
                }
            }
            catch ( Exception ex )
            {
                LogMyFilms.Debug("Failed to set Facade Item as chosen: " + ex.Message);
            }
        }

        void downloadFanart(DBFanart fanart)
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

        void fetchList(int seriesID)
        {
            // Fetch a fresh list online and save info about them to the db 
            //GetFanart gf = new GetFanart(seriesID);
            //foreach (DBFanart f in gf.Fanart) {
            //    f.Commit();
            //}
        }

        void loadThumbnails(int seriesID)
        {
            if (seriesID > 0)
            {                
                if (loadingWorker.CancellationPending)
                    return;

                GUIListItem item = null;
                List<DBFanart> onlineFanart = DBFanart.GetAll(seriesID, false);

                // Filter Fanart Thumbnails to be displayed by resolution
                //if (DBOption.GetOptions(DBOption.cFanartThumbnailResolutionFilter) != 0)
                //{
                //    string filteredRes = (DBOption.GetOptions(DBOption.cFanartThumbnailResolutionFilter) == "1" ? "1280x720" : "1920x1080");
                //    for (int j = onlineFanart.Count - 1; j >= 0; j--)
                //    {
                //        if (onlineFanart[j][DBFanart.cResolution] != filteredRes)
                //            onlineFanart.Remove(onlineFanart[j]);
                //    }
                //}

                // Inform skin message how many Artworks are online
                loadingWorker.ReportProgress(onlineFanart.Count < 100 ? onlineFanart.Count : 100);
                
                // let's get all the ones we have available locally (from online)
                int i = 0;
                foreach (DBFanart f in onlineFanart)
                {                    
                    if(f.isAvailableLocally)
                    {
                        item = new GUIListItem("FanArtLocal");
                        item.IsRemote = false;
                        
                        if (f.Chosen) 
                            item.IsRemote = true;
                        else 
                            item.IsDownloading = false;
                    }
                    else 
                    {
                        item = new GUIListItem("ArtworkOnline");
                        item.IsRemote = false;
                        item.IsDownloading = true;
                    }                    
                    //string filename = f[DBFanart.cThumbnailPath];
                    //filename = filename.Replace("/", @"\");
                    //string fullURL = (DBOnlineMirror.Banners.EndsWith("/") ? DBOnlineMirror.Banners : (DBOnlineMirror.Banners + "/")) + filename;

                    //bool bDownloadSuccess = true;
                    //int nDownloadGUID = Online_Parsing_Classes.OnlineAPI.StartFileDownload(fullURL, Settings.Path.Artwork, filename);
                    //while (Online_Parsing_Classes.OnlineAPI.CheckFileDownload(nDownloadGUID))
                    //{
                    //    if (loadingWorker.CancellationPending)
                    //    {
                    //        // ZF: Cancel, clean up pending download
                    //        bDownloadSuccess = false;
                    //        Online_Parsing_Classes.OnlineAPI.CancelFileDownload(nDownloadGUID);
                    //        MPTVSeriesLog.Write("Cancelling Artwork thumbnail download: " + filename);
                    //    }
                    //    System.Windows.Forms.Application.DoEvents();
                    //}

                    //// ZF: should be downloaded now
                    //filename = Helper.PathCombine(Settings.GetPath(Settings.Path.Artwork), filename);
                    //if (bDownloadSuccess)
                    //{
                    //    item.IconImage = item.IconImageBig = ImageAllocator.GetOtherImage(filename, new System.Drawing.Size(0, 0), false);
                    //}
                    item.TVTag = f;
                    
                    // Subscribe to Item Selected Event
                    item.OnItemSelected += new GUIListItem.ItemSelectedHandler(onFacadeItemSelected);
                    
                    // This will need to be tweaked for more than 100 Artworks
                    loadingWorker.ReportProgress((i < 100 ? ++i: 100), item);                    

                    if (loadingWorker.CancellationPending)
                        return;
                }
            }
        }

        void setFanartPreviewBackground(DBFanart fanart)
        {
            string fanartInfo = fanart.isAvailableLocally ? "FanArtLocal": "FanArtOnline";
            fanartInfo += Environment.NewLine;

            //foreach (KeyValuePair<string, DBField> kv in fanart.m_fields)
            //{
            //    switch (kv.Key)
            //    {
            //        case DBFanart.cResolution:
            //        MyFilmsDetail.setGUIProperty("Artwork.SelectedArtworkResolution", kv.Value.Value);                        
            //            break;

            //        case DBFanart.cColors:
            //            MyFilmsDetail.setGUIProperty("Artwork.SelectedArtworkColors", kv.Value.Value);
            //            break;

            //        case DBFanart.cChosen:
            //            MyFilmsDetail.setGUIProperty("Artwork.SelectedArtworkIsChosen", kv.Value.Value ? Translation.Yes : Translation.No);
            //            break;

            //        case DBFanart.cDisabled:
            //            MyFilmsDetail.setGUIProperty("Artwork.SelectedArtworkIsDisabled", kv.Value.Value ? Translation.Yes : Translation.No);
            //            break;
                    
            //    }
            //    fanartInfo += kv.Key + ": " + kv.Value.Value + Environment.NewLine;
            //}

            MyFilmsDetail.setGUIProperty("Artwork.SelectedArtworkInfo", fanartInfo);

            string preview = string.Empty;
            
            //if (fanart.isAvailableLocally)
            //{
            //    // Ensure Artwork on Disk is valid as well
            //    if (ImageAllocator.LoadImageFastFromFile(fanart.FullLocalPath) == null)
            //    {
            //        MPTVSeriesLog.Write("Artwork is invalid, deleting...");
            //        fanart.Delete();
            //        fanart.Chosen = false;
            //        m_Facade.SelectedListItem.Label = Translation.ArtworkOnline;
            //    }                    
                
            //    // Should be safe to assign fullsize Artwork if available
            //    preview = fanart.isAvailableLocally ?
            //              ImageAllocator.GetOtherImage(fanart.FullLocalPath, default(System.Drawing.Size), false) :
            //              m_Facade.SelectedListItem.IconImageBig;
            //}
            //else
                preview = m_Facade.SelectedListItem.IconImageBig;
                      
            MyFilmsDetail.setGUIProperty("Artwork.SelectedPreview", preview);
        }

    }

    public class DBFanart
    {
      public const String cTableName = "Fanart";

      public const String cIndex = "id"; // comes from online
      public const String cSeriesID = "seriesID";
      public const String cChosen = "Chosen";
      public const String cLocalPath = "LocalPath";
      public const String cBannerPath = "BannerPath"; // online
      public const String cThumbnailPath = "ThumbnailPath"; // online
      public const String cColors = "Colors"; // online
      public const String cResolution = "BannerType2"; // online
      public const String cDisabled = "Disabled";
      public const String cSeriesName = "SeriesName"; // online
      public const String cRating = "Rating"; // online
      public const String cRatingCount = "RatingCount"; // online

      enum FanartResolution
      {
        BOTH,
        HD,
        FULLHD
      }

      public DBFanart()
      {
        //InitColumns();
        //InitValues();
      }

      public DBFanart(long ID)
      {
        //InitColumns();
        //if (!ReadPrimary(ID.ToString()))
        //  InitValues();
      }

      public static void ClearAll()
      {
        cache.Clear();
      }

      public static void Clear(int Index)
      {
        DBFanart dummy = new DBFanart(Index);
        //Clear(dummy, new SQLCondition(dummy, DBFanart.cIndex, Index, SQLConditionType.Equal));
        cache.Remove(Index);
      }

      public static void ClearDB(int seriesID)
      {
        DBFanart dummy = new DBFanart(seriesID);
        //Clear(dummy, new SQLCondition(dummy, DBFanart.cSeriesID, seriesID, SQLConditionType.Equal));
        ClearAll();
      }

      public void Delete()
      {
        // first let's delete the physical file
        if (this.isAvailableLocally)
        {
          try
          {
            System.IO.File.Delete(FullLocalPath);
            //LogMyFilms.Debug("Fanart Deleted: " + FullLocalPath);
          }
          catch (Exception)
          {
            //LogMyFilms.Debug("Failed to delete file: " + FullLocalPath + " (" + ex.Message + ")");
          }
        }
        //Clear(this[cIndex]);
      }

      public bool Commit()
      {
        //lock (cache)
        //{
        //  if (cache.ContainsKey(this[DBFanart.cSeriesID]))
        //    cache.Remove(this[DBFanart.cSeriesID]);
        //}
        //return base.Commit();
        return true;
      }

      static Dictionary<int, List<DBFanart>> cache = new Dictionary<int, List<DBFanart>>();

      public static List<int> GetSeriesWithFanart()
      {
        List<int> seriesids = new List<int>();

        //string sqlQuery = "SELECT DISTINCT seriesID FROM Fanart";
        //SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);

        //if (results.Rows.Count > 0)
        //{
        //  for (int index = 0; index < results.Rows.Count; index++)
        //  {
        //    int result = 0;
        //    if (int.TryParse(results.Rows[index].fields[0], out result))
        //      seriesids.Add(result);
        //  }
        //}
        return seriesids;
      }

      public static List<DBFanart> GetAll(int SeriesID, bool availableOnly)
      {
        lock (cache)
        {
          if (SeriesID < 0) return new List<DBFanart>();

          //if (cache == null || !cache.ContainsKey(SeriesID))
          //{
          //  try
          //  {
          //    // make sure the table is created - create a dummy object
          //    DBFanart dummy = new DBFanart();

          //    // retrieve all fields in the table
          //    String sqlQuery = "select * from " + cTableName;
          //    sqlQuery += " where " + cSeriesID + " = " + SeriesID.ToString();
          //    if (availableOnly)
          //      sqlQuery += " and " + cLocalPath + " != ''";
          //    sqlQuery += " order by " + cIndex;

          //    SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);
          //    if (results.Rows.Count > 0)
          //    {
          //      List<DBFanart> ourFanart = new List<DBFanart>(results.Rows.Count);

          //      for (int index = 0; index < results.Rows.Count; index++)
          //      {
          //        ourFanart.Add(new DBFanart());
          //        ourFanart[index].Read(ref results, index);
          //      }
          //      if (cache == null) cache = new Dictionary<int, List<DBFanart>>();
          //      cache.Add(SeriesID, ourFanart);
          //    }
          //    MPTVSeriesLog.Write("Found " + results.Rows.Count + " Fanart from Database", MPTVSeriesLog.LogLevel.Debug);

          //  }
          //  catch (Exception ex)
          //  {
          //    MPTVSeriesLog.Write("Error in DBFanart.Get (" + ex.Message + ").");
          //  }
          //}
          List<DBFanart> faForSeries = null;
          if (cache != null && cache.TryGetValue(SeriesID, out faForSeries))
            return faForSeries;
          return new List<DBFanart>();
        }
      }

      public List<DBFanart> FanartsToDownload(int SeriesID)
      {
        //// Only get a list of fanart that is available for download
        //String sqlQuery = "select * from " + cTableName;
        //sqlQuery += " where " + cSeriesID + " = " + SeriesID.ToString();

        //// Get Preferred Resolution
        //int res = DBOption.GetOptions(DBOption.cAutoDownloadFanartResolution);
        //bool getSeriesNameFanart = DBOption.GetOptions(DBOption.cAutoDownloadFanartSeriesNames);

        //if (res == (int)FanartResolution.HD)
        //  sqlQuery += " and " + cResolution + " = " + "\"1280x720\"";
        //if (res == (int)FanartResolution.FULLHD)
        //  sqlQuery += " and " + cResolution + " = " + "\"1920x1080\"";
        //if (!getSeriesNameFanart)
        //  sqlQuery += " and " + cSeriesName + " != " + "\"true\"";

        //SQLiteResultSet results = DBTVSeries.Execute(sqlQuery);

        //if (results.Rows.Count > 0)
        //{
        //  int iFanartCount = 0;
        //  List<DBFanart> AvailableFanarts = new List<DBFanart>(results.Rows.Count);
        //  for (int index = 0; index < results.Rows.Count; index++)
        //  {
        //    if (results.GetField(index, (int)results.ColumnIndices[cLocalPath]).Length > 0)
        //      iFanartCount++;
        //    else
        //    {
        //      // Add 'Available to Download' fanart to list
        //      AvailableFanarts.Add(new DBFanart());
        //      AvailableFanarts[AvailableFanarts.Count - 1].Read(ref results, index);
        //    }
        //  }

        //  // sort by highest rated
        //  AvailableFanarts.Sort();

        //  // Only return the fanarts that we want to download
        //  int AutoDownloadCount = DBOption.GetOptions(DBOption.cAutoDownloadFanartCount);

        //  for (int i = 0; i < AvailableFanarts.Count; i++)
        //  {
        //    // Dont get more than the user wants
        //    if (iFanartCount >= AutoDownloadCount)
        //      break;
        //    _FanartsToDownload.Add(AvailableFanarts[i]);
        //    iFanartCount++;
        //  }
        //}
        return _FanartsToDownload;

      } List<DBFanart> _FanartsToDownload = new List<DBFanart>();

      /// <summary>
      /// Checks if a Series Fanart contains a Series Name
      /// </summary>
      public bool HasSeriesName
      {
        get
        {
          //if (this[cSeriesName] = "true")
          //  return true;
          //else
            return false;
        }
      }

      public bool Chosen
      {
        get
        {
          //return this[cChosen];
          return false;
        }
        set
        {
          //GlobalSet(new DBFanart(), cChosen, false, new SQLCondition(new DBFanart(), cSeriesID, this[cSeriesID], SQLConditionType.Equal));
          //this[cChosen] = value;
          //this.Commit();
        }
      }

      //public bool Disabled
      //{
      //  get
      //  {
      //    if (this[cDisabled])
      //      return true;
      //    return false;
      //  }
      //  set
      //  {
      //    this[cDisabled] = value;
      //    this.Commit();
      //  }
      //}

      public bool isAvailableLocally
      {
        get
        {
          //if (String.IsNullOrEmpty(this[DBFanart.cLocalPath])) return false;

          //// Check if file in path exists, remove it from database if not
          //if (System.IO.File.Exists(Settings.GetPath(Settings.Path.fanart) + @"\" + this[DBFanart.cLocalPath])) return true;
          //this[DBFanart.cLocalPath] = string.Empty;
          return false;
        }
      }

      public string FullLocalPath
      {
        get
        {
          //if (String.IsNullOrEmpty(this[cLocalPath])) return string.Empty;
          //return Helper.PathCombine(Settings.GetPath(Settings.Path.fanart), this[cLocalPath]);
          return "";
        }
      }

      //public bool HasColorInfo
      //{
      //  get
      //  {
      //    return !String.IsNullOrEmpty(this[cColors]);
      //  }
      //}

      //public System.Drawing.Color GetColor(int which)
      //{
      //  if (HasColorInfo && which <= 3 && which > 0)
      //  {
      //    string[] split = this[cColors].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
      //    if (split.Length != 3) return default(System.Drawing.Color);
      //    string[] rgbValues = split[--which].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
      //    return System.Drawing.Color.FromArgb(100, Int32.Parse(rgbValues[0]), Int32.Parse(rgbValues[1]), Int32.Parse(rgbValues[2]));
      //  }
      //  else return default(System.Drawing.Color);
      //}

      //public override string ToString()
      //{
      //  return this[cSeriesID] + " -> " + this[cIndex];
      //}

      //#region IComparable
      //public int CompareTo(DBFanart other)
      //{
      //  // Sort by:
      //  // 1. Highest Rated
      //  // 2. Number of Votes

      //  double thisFanart = 0.0;
      //  double otherFanart = 0.0;

      //  if (this[cRating] == other[cRating])
      //  {
      //    thisFanart += this[cRatingCount];
      //    otherFanart += other[cRatingCount];
      //  }

      //  thisFanart += this[cRating];
      //  otherFanart += other[cRating];

      //  return otherFanart.CompareTo(thisFanart);
      //}
      //#endregion

    }
}
