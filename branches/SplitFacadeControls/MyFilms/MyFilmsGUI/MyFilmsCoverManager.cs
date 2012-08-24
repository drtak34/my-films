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
using MediaPortal.Dialogs;
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

  using MyFilmsPlugin.Utils;
  using MyFilmsPlugin.MyFilms.Utils.Cornerstone.MP;

  class MyFilmsCoverManager : GUIWindow
  {

      #region Skin Controls

        [SkinControlAttribute(50)]
        protected GUIFacadeControl m_Facade = null;

        [SkinControlAttribute(2)]
        protected GUIButtonControl buttonLayouts = null;

        [SkinControlAttribute(11)]
        protected GUILabelControl labelResolution = null;

        [SkinControlAttribute(12)]
        protected GUIButtonControl buttonFilters = null;

        [SkinControlAttribute(13)]
        protected GUIButtonControl buttonDownloadCover = null;

      #endregion

      #region Enums

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
            PlayList = 5,
            CoverFlow = 6
        }

      #endregion

        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
        int movieId = -1;
        private string movieLabel = string.Empty;
        private string artworkFileName = string.Empty;
        private string newArtworkFileName = string.Empty;
        private string displayFilter = string.Empty;
        BackgroundWorker loadingWorker = null; // to fetch list and thumbnails
        int m_PreviousSelectedItem = -1;
        private View currentView = View.LargeIcons;
        private int initialView = -1;
        bool m_bQuickSelect = false;

        // rivate ImageSwapper CovermanagerBackground;
        // private Utils.Cornerstone.MP.AsyncImageResource CovermanagerBackground = null;

        # region DownloadWorker
        static MyFilmsCoverManager()
        {
            // lets set up the downloader
          //// create Backdrop image swapper
          //CovermanagerBackground = new ImageSwapper();
          //CovermanagerBackground.ImageResource.Delay = 250;
          //CovermanagerBackground.PropertyOne = "#myfilms.cover.fanart";
          //CovermanagerBackground.PropertyTwo = "#myfilms.cover.fanart2";

        }
        #endregion

        public override int GetID
        { get { return MyFilms.ID_MyFilmsCoverManager; } }

        public override bool Init()
        {
            String xmlSkin = GUIGraphicsContext.Skin + @"\MyFilmsCoverManager.xml";            
            return Load(xmlSkin);
        }

	      /// <summary>
	      /// MediaPortal will set #currentmodule with GetModuleName()
	      /// </summary>
	      /// <returns>Localized Window Name</returns>
        public override string GetModuleName()
        {
          return GUILocalizeStrings.Get(MyFilms.ID_MyFilmsCoverManager);
        }

        protected View CurrentView
        {
            get { return currentView; }
            set { currentView = value; }
        }

        protected override void OnPageLoad()
        {
          LogMyFilms.Debug("OnPageLoad() - Cover Manager Window initialization started ...");
          AllocResources();
          MovieID = MyFilms.conf.StrIndex;
          MyFilmsDetail.Searchtitles sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MovieID));
          ArtworkFileName = MyFilmsDetail.GetOrCreateCoverFilename(MyFilms.r, MyFilms.conf.StrIndex, sTitles.MasterTitle);

          MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(10799201)); // MyFilms Cover Manager

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

          MyFilmsDetail.DetailsUpdated += new MyFilmsDetail.DetailsUpdatedEventDelegate(OnDetailsUpdated);

          base.OnPageLoad();
          
          // update skin controls
          UpdateLayoutButton();
          if (labelResolution != null) labelResolution.Label = GUILocalizeStrings.Get(10799202); //  resolution

          if (buttonFilters != null) buttonFilters.Label = GUILocalizeStrings.Get(10799203); //  filter ...
          if (buttonDownloadCover != null) buttonDownloadCover.Label = GUILocalizeStrings.Get(10799204); //  Download covers ...

          ClearProperties();
          UpdateFilterProperty(false);

          string movielabel = MyFilms.r[MovieID][MyFilms.conf.StrTitle1].ToString();
          if (!string.IsNullOrEmpty(MyFilms.r[MovieID][MyFilms.conf.StrTitle2].ToString())) movielabel += " (" + MyFilms.r[MovieID][MyFilms.conf.StrTitle2] + ")";
          if (!string.IsNullOrEmpty(MyFilms.r[MovieID]["Year"].ToString())) movielabel += " - [" + MyFilms.r[MovieID]["Year"] + "]";
          MovieLabel = movielabel;

          MyFilmsDetail.setGUIProperty("cover.currentmoviename", MovieLabel);
          if (!string.IsNullOrEmpty(MyFilms.r[MovieID]["Picture"].ToString())) MyFilmsDetail.setGUIProperty("picture", ArtworkFileName); // only set preview, if DB is not empty ...
             
          loadingWorker.RunWorkerAsync(this.MovieID);
          LogMyFilms.Debug("OnPageLoad() - Cover Manager Window initialization finished ...");
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
                strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(101); // strLine = MediaPortal.GUI.Library.GUILocalizeStrings.Get(101); // translations from Mediaportal !
                    break;
                case View.Icons:
                    strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(100);
                    break;
                case View.LargeIcons:
                    strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(417);
                    break;
                case View.FilmStrip:
                    strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(733);
                    break;
                case View.PlayList:
                    strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(101);
                    break;
                case View.CoverFlow:
                    strLine = GUILocalizeStrings.Get(95) + GUILocalizeStrings.Get(791);
                    break;
            }
            if (buttonLayouts != null)
                GUIControl.SetControlLabel(GetID, buttonLayouts.GetID, strLine);
        }

        private void ClearProperties()
        {
            MyFilmsDetail.clearGUIProperty("cover.currentmoviename");
            MyFilmsDetail.clearGUIProperty("cover.count");
            MyFilmsDetail.clearGUIProperty("cover.selectedcoverresolution");
            MyFilmsDetail.clearGUIProperty("cover.selectedcoverresolutionclass");
            MyFilmsDetail.clearGUIProperty("cover.selectedcovername");

            MyFilmsDetail.clearGUIProperty("cover.selectedcoversize");
            MyFilmsDetail.clearGUIProperty("cover.selectedcoversizenum");
            MyFilmsDetail.clearGUIProperty("cover.selectedpreview");

        }

        private void UpdateFilterProperty(bool btnEnabled)
        {
            if (buttonFilters != null) buttonFilters.IsEnabled = btnEnabled;
            string resolution = (string.IsNullOrEmpty(DisplayFilter)) ? "All" : DisplayFilter;
            MyFilmsDetail.setGUIProperty("cover.filterresolution", resolution);            
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
          MyFilmsDetail.setGUIProperty("cover.currentmoviename", MovieLabel);
          MyFilmsDetail.setGUIProperty("cover.count", m_Facade.Count.ToString());

          if (int.Parse(m_Facade.Count.ToString()) == 0)
            {
                // Enable Filters button in case Artwork is filtered
                if (DisplayFilter != "All" && !string.IsNullOrEmpty(DisplayFilter) && buttonFilters != null)
                {
                    OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
                    OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));                    
                }
            }

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
                          LogMyFilms.Debug("worker_RunWorkerCompleted() - move right to focus correct item");  
                          OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
                        }
                        m_bQuickSelect = false;
                        // Note: this is better way, but Scroll offset wont work after set
                        //GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, m_Facade.WindowId, 0, m_Facade.FilmstripLayout.GetID, m_PreviousSelectedItem, 0, null);
                        //GUIGraphicsContext.SendMessage(msg);
                    }                   
                    m_PreviousSelectedItem = -1;
                }

                MFCover selectedCover = m_Facade.SelectedListItem.TVTag as MFCover;
                if (selectedCover != null) this.setItemProperties(selectedCover);
            }
            UpdateFilterProperty(true);            
        }

        protected override void OnPageDestroy(int new_windowId)
        {
          MyFilmsDetail.DetailsUpdated -= new MyFilmsDetail.DetailsUpdatedEventDelegate(OnDetailsUpdated);
          // MFCover selectedFanart = m_Facade.SelectedListItem.TVTag as MFCover;
          if (!string.IsNullOrEmpty(NewArtworkFileName) && File.Exists(NewArtworkFileName) && MyFilmsDetail.getGUIProperty("picture") == NewArtworkFileName)
            saveChangesToDB();
          else
            LogMyFilms.Debug("OnPageDestroy - saveChangesToDB() - Cover file does not exist - not saving Cover '" + NewArtworkFileName + "' to DB !");

          if (loadingWorker.IsBusy) loadingWorker.CancelAsync();
          while (loadingWorker.IsBusy)
            System.Windows.Forms.Application.DoEvents();

          loadingWorker = null;
          // Helper.enableNativeAutoplay();
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
                dlg.SetHeading(GUILocalizeStrings.Get(10799201)); // MyFilms Cover Manager

                GUIListItem pItem;

                if (!loadingWorker.IsBusy) // don't allowing filtering until all data is loaded
                {
                  pItem = new GUIListItem(GUILocalizeStrings.Get(10799203)); // Filter...
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.Filters;
                }
              
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

                if (!loadingWorker.IsBusy && !string.IsNullOrEmpty(MyFilms.r[MovieID]["Picture"].ToString()))
                {
                  pItem = new GUIListItem(GUILocalizeStrings.Get(10798810)); // Delete Movie Cover from DB
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.ClearMovieCover;
                }

                if (bCoverSelected)
                {
                  pItem = new GUIListItem(GUILocalizeStrings.Get(10799212)); //"Delete all 'Low'"
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.DeleteAllLow;

                  pItem = new GUIListItem(GUILocalizeStrings.Get(10799213)); // "Delete all 'Medium'"
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.DeleteAllMedium;

                  pItem = new GUIListItem(GUILocalizeStrings.Get(10799214)); // "Delete all 'High'"
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.DeleteAllHigh;

                  pItem = new GUIListItem(GUILocalizeStrings.Get(10799211)); // Delete selected Cover
                  dlg.Add(pItem);
                  pItem.ItemId = (int)menuAction.DeleteSelected;

                  pItem = new GUIListItem(GUILocalizeStrings.Get(10799215)); // "Delete all except currently selected"
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
                    // this.RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
                    break;
                  case (int)menuAction.LoadMultiple:
                    //downloadFanart(selectedCover);
                    title = MyFilmsDetail.GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
                    mediapath = MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
                    sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);
                    MyFilmsDetail.grabb_Internet_Informations(title, GetID, true, MyFilms.conf.StrGrabber_cnf, mediapath, MyFilmsDetail.GrabType.MultiCovers, false, sTitles);
                    // this.RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
                    break;
                  case (int)menuAction.LoadFromTmdb:
                    sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
                    MyFilmsDetail.Download_TMDB_Posters(sTitles.OriginalTitle, sTitles.TranslatedTitle, sTitles.Director, sTitles.year.ToString(), false, GetID, sTitles.OriginalTitle);
                    // this.RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
                    break;
                  case (int)menuAction.CreateFromMovie:
                    //ToDo: Add Code for single image thumbnailer
                    // this.RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
                    break;
                  case (int)menuAction.CreateFromMovieAsMosaic:
                    //downloadFanart(selectedCover);
                    MyFilmsDetail.CreateThumbFromMovie();
                    // this.RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
                    break;
                  case (int)menuAction.DeleteSelected:
                    // delete from DB, if it is the one currently selected
                    if (NewArtworkFileName == selectedCover.FullPath)
                    {
                      NewArtworkFileName = "";
                      sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MovieID], MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MovieID));
                      ArtworkFileName = MyFilmsDetail.GetOrCreateCoverFilename(MyFilms.r, MovieID, sTitles.MasterTitle);
                      MyFilmsDetail.clearGUIProperty("picture");
                      saveChangesToDB();
                    }
                    selectedCover.Delete(AllTitles());
                    // and reinit the display to get rid of it
                    this.RefreshFacade();
                    break;
                  case (int)menuAction.DeleteAllLow:
                    this.DeleteSelectedFromGroup("Low");
                    this.RefreshFacade();
                    break;
                  case (int)menuAction.DeleteAllMedium:
                    this.DeleteSelectedFromGroup("Medium");
                    this.RefreshFacade();
                    break;
                  case (int)menuAction.DeleteAllHigh:
                    this.DeleteSelectedFromGroup("High");
                    this.RefreshFacade();
                    break;
                  case (int)menuAction.DeleteAllExceptSelected:
                    DeleteAllExceptSelected(m_Facade.SelectedListItemIndex);
                    this.RefreshFacade();
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
                    ClearProperties();
                    UpdateFilterProperty(false);
                    this.RefreshFacade();
                    break;
                }
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug("Exception in Artwork Chooser Context Menu: " + ex.Message);
              return;
            }
        }

        private void ShowCoverContextMenu()
        {
            try
            {

                IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading(GUILocalizeStrings.Get(10799201)); // MyFilms Cover Manager

                GUIListItem pItem;

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

                // lets show it
                dlg.DoModal(GUIWindowManager.ActiveWindow);
                string title = "";
                string mediapath = "";
                MyFilmsDetail.Searchtitles sTitles;
                switch (dlg.SelectedId) // what was chosen?
                {
                    case (int)menuAction.LoadSingle:
                        title = MyFilmsDetail.GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
                        mediapath = MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
                        sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);
                        MyFilmsDetail.grabb_Internet_Informations(title, GetID, true, MyFilms.conf.StrGrabber_cnf, mediapath, MyFilmsDetail.GrabType.Cover, false, sTitles);
                        // this.RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
                        break;
                    case (int)menuAction.LoadMultiple:
                        title = MyFilmsDetail.GetSearchTitle(MyFilms.r, MyFilms.conf.StrIndex, "");
                        mediapath = MyFilmsDetail.GetMediaPathOfFirstFile(MyFilms.r, MyFilms.conf.StrIndex);
                        sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], mediapath);
                        MyFilmsDetail.grabb_Internet_Informations(title, GetID, true, MyFilms.conf.StrGrabber_cnf, mediapath, MyFilmsDetail.GrabType.MultiCovers, false, sTitles);
                        // this.RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
                        break;
                    case (int)menuAction.LoadFromTmdb:
                        sTitles = MyFilmsDetail.GetSearchTitles(MyFilms.r[MyFilms.conf.StrIndex], "");
                        MyFilmsDetail.Download_TMDB_Posters(sTitles.OriginalTitle, sTitles.TranslatedTitle, sTitles.Director, sTitles.year.ToString(), false, GetID, sTitles.OriginalTitle);
                        // this.RefreshFacade(); // will be done by OnDetailsUpdated Message Handler
                        break;
                    case (int)menuAction.CreateFromMovie:
                        //ToDo: Add Code for single image thumbnailer
                        this.RefreshFacade();
                        break;
                    case (int)menuAction.CreateFromMovieAsMosaic:
                        MyFilmsDetail.CreateThumbFromMovie();
                        this.RefreshFacade();
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
            dlg.SetHeading(GUILocalizeStrings.Get(10799203)); // Filter...

            GUIListItem pItem = new GUIListItem(GUILocalizeStrings.Get(10799221)); // All
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.all;

            pItem = new GUIListItem(GUILocalizeStrings.Get(10799222)); // High
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.high;

            pItem = new GUIListItem(GUILocalizeStrings.Get(10799223)); // Medium
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.medium;

            pItem = new GUIListItem(GUILocalizeStrings.Get(10799224)); // Low
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.low;

            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId >= 0)
            {
                switch (dlg.SelectedId)
                {
                  case (int)menuFilterAction.all:
                      DisplayFilter = "All";
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
                ClearProperties();
                UpdateFilterProperty(false);
                RefreshFacade();
            }
        }
        #endregion

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
          try
          {
            if (m_Facade != null)
            {
              GUIListItem loadedItem = e.UserState as GUIListItem;
              if (loadedItem != null)
              {
                m_Facade.Add(loadedItem);
                LogMyFilms.Debug("worker_ProgressChanged() - item '" + loadedItem.Label + "' added to facade");

                // we use this to tell the gui how many Artwork we are loading
                MyFilmsDetail.setGUIProperty("cover.count", m_Facade.Count.ToString());
                if (m_Facade != null) this.m_Facade.Focus = true;
              }
              else
              {
                LogMyFilms.Debug("worker_ProgressChanged() - item not added (null)");
              }
              //else if (e.ProgressPercentage > 0)
              //{
              //  // we use this to tell the gui how many Artwork we are loading
              //  MyFilmsDetail.setGUIProperty("cover.counttotal", e.ProgressPercentage.ToString()); 
              //}
            }
          }
          catch (Exception ex)
          {
            LogMyFilms.Debug("Error: Artwork Chooser worker_ProgressChanged() experienced an error: " + ex.Message);
          }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadThumbnails((int)e.Argument);
        }

        public int MovieID
        {
          get { return movieId; }
          set { movieId = value; }
        }

        public string MovieLabel
        {
          get { return movieLabel; }
          set { movieLabel = value; }
        }

        public string ArtworkFileName
        {
          get { return artworkFileName; }
          set { artworkFileName = value; }
        }

        public string NewArtworkFileName
        {
          get { return newArtworkFileName; }
          set { newArtworkFileName = value; }
        }

        public string DisplayFilter
        {
          get { return displayFilter; }
          set { displayFilter = value; }
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
                            this.setItemProperties(selectedFanart);
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
            if (parent != m_Facade && parent != m_Facade.FilmstripLayout && parent != m_Facade.ThumbnailLayout && parent != m_Facade.ListLayout) return; // if this is not a message from the facade, exit
            MFCover selectedCover = item.TVTag as MFCover;
            if (selectedCover != null) setItemProperties(selectedCover);
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
              Change_Layout();
              UpdateLayoutButton();
              RefreshFacade();
              GUIControl.FocusControl(GetID, controlId);
              return;
            }


            if (control == buttonDownloadCover)
            {
                ShowCoverContextMenu();
                buttonDownloadCover.Focus = false;
                GUIControl.FocusControl(GetID, 50);
                return;
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

        private void Change_Layout()
        {
          GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          if (dlg == null) return;
          List<int> menuentries = new List<int>();
          
          dlg.Reset();
          dlg.SetHeading(GUILocalizeStrings.Get(1079901)); // View (Layout) ...
          if (AllowView(View.List) && m_Facade.ListLayout != null)
          {
            dlg.Add(GUILocalizeStrings.Get(101));//List
            menuentries.Add((int)View.List);
          }
          if (AllowView(View.Icons) && m_Facade.ThumbnailLayout != null)
          {
            dlg.Add(GUILocalizeStrings.Get(100));//Icons
            menuentries.Add((int)View.Icons);
          }
          if (AllowView(View.LargeIcons) && m_Facade.ThumbnailLayout != null)
          {
            dlg.Add(GUILocalizeStrings.Get(417));//Large Icons
            menuentries.Add((int)View.LargeIcons);
          }
          if (AllowView(View.FilmStrip) && m_Facade.FilmstripLayout != null)
          {
            dlg.Add(GUILocalizeStrings.Get(733));//Filmstrip
            menuentries.Add((int)View.FilmStrip);
          }
          if (AllowView(View.AlbumView) && m_Facade.AlbumListLayout != null)
          {
            dlg.Add(GUILocalizeStrings.Get(529));//Cover list - used as extended list
            menuentries.Add((int)View.AlbumView);
          }
          if (AllowView(View.PlayList) && m_Facade.PlayListLayout != null)
          {
            dlg.Add(GUILocalizeStrings.Get(529));//Cover list - used as extended list
            menuentries.Add((int)View.AlbumView);
          }
          if (AllowView(View.CoverFlow) && m_Facade.CoverFlowLayout != null)
          {
            dlg.Add(GUILocalizeStrings.Get(791));//Coverflow
            menuentries.Add((int)View.CoverFlow);
          }

          dlg.DoModal(GetID);
          if (dlg.SelectedLabel == -1) return;

          Change_Layout_Action(menuentries[dlg.SelectedLabel]);
          dlg.DeInit();
        }

        private void Change_Layout_Action(int iLayout)
        {
          LogMyFilms.Debug("Change_Layout_Action() - change facade layout to '" + iLayout + "'");
          switch (iLayout)
          {
            case 0:
              CurrentView = View.List;
              m_Facade.CurrentLayout = GUIFacadeControl.Layout.List;
              break;
            case 1:
              CurrentView = View.Icons;
              m_Facade.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
              break;
            case 2:
              CurrentView = View.LargeIcons;
              m_Facade.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
              break;
            case 3:
              CurrentView = View.FilmStrip;
              m_Facade.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
              break;
            case 4:
              CurrentView = View.AlbumView;
              m_Facade.CurrentLayout = GUIFacadeControl.Layout.AlbumView;
              break;
            case 5:
              CurrentView = View.PlayList;
              m_Facade.CurrentLayout = GUIFacadeControl.Layout.Playlist;
              break;
            case 6:
              CurrentView = View.CoverFlow;
              m_Facade.CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
              break;

            default:
              CurrentView = View.LargeIcons;
              m_Facade.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
              break;
          }
        }
    
        private void RefreshFacade() // (re)load facade
        {
          if (loadingWorker.IsBusy)
          {
            loadingWorker.CancelAsync();
            System.Threading.Thread.Sleep(500);
          }
          if (!loadingWorker.IsBusy)
          {
            m_Facade.Clear();
            loadingWorker.RunWorkerAsync(this.MovieID);
          }
        }

        private void OnDetailsUpdated(bool searchPicture)
        {
          LogMyFilms.Debug("OnDetailsUpdated(): Received DetailUpdated event in context '" + GetID + "'");
          if (GetID == MyFilms.ID_MyFilmsCoverManager)
          {
            LogMyFilms.Debug("OnDetailsUpdated(): now reloading Covermanager Facade");
            RefreshFacade();
          }
          else
            LogMyFilms.Debug("OnDetailsUpdated(): Skipping reloading Details");
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
          catch (Exception ex) { LogMyFilms.Debug("Failed to set Facade Item as chosen: " + ex.Message); }
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
                  item.Delete(AllTitles());
            }
          }
          catch (Exception ex) { LogMyFilms.Debug("Failed to set Facade Item as chosen: " + ex.Message); }
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
                  item.Delete(AllTitles());
              }
            }
          }
          catch (Exception ex) { LogMyFilms.Debug("Failed to set Facade Item as chosen: " + ex.Message); }
        }

        void loadThumbnails(int MovieID)
        {
            if (MovieID > -1)
            {                
                if (loadingWorker.CancellationPending) return;

                GUIListItem item = null;
                GUIWaitCursor.Init(); GUIWaitCursor.Show(); // show waitcorsor while covers are searched on local drive...
                List<MFCover> covers = GetLocalCover(MyFilms.r, MyFilms.conf.StrIndex, ArtworkFileName);
                GUIWaitCursor.Hide();

                int i = 0;
                foreach (MFCover mfCover in covers)
                {
                  bool add = false;
                  item = null; // reset item to null
                  switch (DisplayFilter)
                  {
                    case "High":
                      if (mfCover.ImageResolutionClass == "High")
                        add = true;
                      break;
                    case "Medium":
                      if (mfCover.ImageResolutionClass == "Medium")
                        add = true;
                      break;
                    case "Low":
                      if (mfCover.ImageResolutionClass == "Low")
                        add = true;
                      break;
                    default:
                      add = true;
                      break;
                  }

                  if (add)
                  {
                      item = new GUIListItem(mfCover.ImageResolution);
                      item.IsRemote = false;
                      item.TVTag = mfCover;
                      item.Path = mfCover.FullPath;
                      item.IconImageBig = ImageAllocator.GetOtherImage(mfCover.FullPath, new System.Drawing.Size(400, 600), false);
                      item.IconImage = item.IconImageBig;
                      item.ThumbnailImage = item.IconImageBig;
                      item.OnItemSelected += new GUIListItem.ItemSelectedHandler(onFacadeItemSelected);
                  }
                  loadingWorker.ReportProgress((i < 100 ? ++i : 100), item);
                  if (loadingWorker.CancellationPending) return;
                }
            }
        }

        void setItemProperties(MFCover cover)
        {
          MyFilmsDetail.setGUIProperty("cover.selectedcoverresolution", cover.ImageResolution);
          MyFilmsDetail.setGUIProperty("cover.selectedcoverresolutionclass", cover.ImageResolutionClass);
          MyFilmsDetail.setGUIProperty("cover.selectedcoversize", cover.ImageSizeFriendly);
          MyFilmsDetail.setGUIProperty("cover.selectedcoversizenum", cover.ImageSize.ToString());
          MyFilmsDetail.setGUIProperty("cover.selectedcovername", cover.FileName);
          MyFilmsDetail.setGUIProperty("cover.selectedpreview", m_Facade.SelectedListItem.Path);

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
              #region add found files
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
              #endregion
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

        private List<string> AllTitles()
        {
          List<string> allTitles = new List<string>();
          string title = null;
          title = MyFilms.r[MovieID]["OriginalTitle"].ToString();
          if (!string.IsNullOrEmpty(title)) allTitles.Add(title);
          title = MyFilms.r[MovieID]["TranslatedTitle"].ToString();
          if (!string.IsNullOrEmpty(title)) allTitles.Add(title);
          title = MyFilms.r[MovieID]["FormattedTitle"].ToString();
          if (!string.IsNullOrEmpty(title)) allTitles.Add(title);
          return allTitles;
        }
    }

    public class MFCover
    {
      private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
      
      public MFCover() {}

      public void Delete(List<string> titles)
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

        try // also delete image from thumbscache directory
        {
          string CoverThumbDir = MyFilmsSettings.GetPath(MyFilmsSettings.Path.ThumbsCache) + @"\MyFilms_Movies";
          foreach (string title in titles)
          {
            string strThumb = MediaPortal.Util.Utils.GetCoverArtName(CoverThumbDir, title);
            if (System.IO.File.Exists(strThumb)) System.IO.File.Delete(strThumb);
            string strThumbSmall = strThumb.Substring(0, strThumb.LastIndexOf(".")) + "_s" + Path.GetExtension(strThumb);
            if (System.IO.File.Exists(strThumbSmall)) System.IO.File.Delete(strThumbSmall);
            LogMyFilms.Debug("Cached thumbs deleted for title = '" + title + "'");
          }
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug("Failed to delete cache thumb file - exception: " + ex.Message + "");
        }
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
