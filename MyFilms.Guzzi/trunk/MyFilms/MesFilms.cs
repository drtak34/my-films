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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Xml;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;

using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using MediaPortal.Player;
using MediaPortal.Configuration;
using MediaPortal.Util;

using NewStringLib;
using Cornerstone.MP;
using MesFilms.MyFilms;

namespace MesFilms
{
    /// <summary>
    /// Summary description for GUIMesFilms.
    /// </summary>
    public class MesFilms : GUIWindow, ISetupForm
    {

        //private BaseMesFilms films;

        #region Descriptif zones Ecran

        public const int ID_MesFilms = 7986;
        public const int ID_MesFilmsDetail = 7987;
        public const int ID_MesFilmsActors = 7989;
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
        public bool Context_Menu = false;
        public static Configuration conf;
        public static Logos confLogos;
        //private string currentConfig;
        private string strPluginName = "GuzziThek";
        public static DataRow[] r; // will hold current recordset to traverse
        public ImageSwapper backdrop;
        //Guzzi Addons for Global nonpermanent Trailer and MinRating Filters
        public bool GlobalFilterTrailersOnly = false;
        public bool GlobalFilterMinRating = false;
        public string GlobalFilterString = "";
        public bool MovieScrobbling = false;
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
            string strBtnFile;
            strPictureImage = String.Format("hover_{0}.png", "Films");
            strBtnFile = String.Format(@"{0}\media\{1}", GUIGraphicsContext.Skin, strPictureImage);
            if (!System.IO.File.Exists(strBtnFile))
                strPictureImage = "";
            return true;
        }
        public override int GetID
        {
            get {return ID_MesFilms;}
        }

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\MesFilms.xml");
        }

        #endregion

        #region Action
        
        //---------------------------------------------------------------------------------------
        //   Handle Keyboard Actions
        //---------------------------------------------------------------------------------------

        public override void OnAction(MediaPortal.GUI.Library.Action actionType)
        {
            Log.Debug("MyFilms : OnAction " + actionType.wID.ToString());
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
                    GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_HOME);
                    return;
                }
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

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU)
                if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
                    {
                        Log.Debug("MyFilms : ACTION_CONTEXT_MENU erkannt !!! ");
                        // context menu for update or suppress entry
                        Context_Menu_Movie(facadeView.SelectedListItem.ItemId);
                        return;
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
                    //Hier muß irgendwie gemerkt werden, daß eine Rückkehr vom TrailerIsAvailable erfolgt - CheckAccess WIndowsID des Conterxts via LOGs
                    if ((PreviousWindowId != ID_MesFilmsDetail) && !MovieScrobbling )
                    {
                        MovieScrobbling = false; //Reset MovieScrobbling
                        MesFilmsDetail.Init_Detailed_DB();
                        backdrop = new ImageSwapper();
                        backdrop.Active = false;
                        backdrop.PropertyOne = " ";
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2001", " ");
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2002", " ");
                        GUIPropertyManager.SetProperty("#myfilms.nbobjects", " ");
                        GUIPropertyManager.SetProperty("#myfilms.label1", " ");
                        GUIPropertyManager.SetProperty("#myfilms.label2", " ");
                        GUIPropertyManager.SetProperty("#myfilms.item1", " ");
                        GUIPropertyManager.SetProperty("#myfilms.item2", " ");
                        GUIPropertyManager.SetProperty("#myfilms.item3", " ");
                        GUIPropertyManager.SetProperty("#myfilms.select", " ");
                        GUIPropertyManager.SetProperty("#myfilms.rating", "0");
                        affichage_rating(0);
                        setProcessAnimationStatus(false, m_SearchAnimation); 
                        GUIControl.HideControl(GetID, 34);
                        Configuration.Current_Config();
                        Load_Config(Configuration.CurrentConfig, true);
                    }
                    if (Configuration.CurrentConfig.Length == 0)
                        GUIWindowManager.ShowPreviousWindow();
 //                   if (!bgLoadMovieList.IsBusy)
 //                   {
 //                       this.bgLoadMovieList.DoWork += new DoWorkEventHandler(bgLoadMovieList_DoWork);
 //                       this.bgLoadMovieList.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgLoadMovieList_RunWorkerCompleted);
 //                       AsynLoadMovieList();
 //                   }
                    GUIControl.ShowControl(GetID, 34);
                    setProcessAnimationStatus(false, m_SearchAnimation); 
                    backdrop = new ImageSwapper();
                    backdrop.Active = true;
                    backdrop.ImageResource.Delay = 250;
                    backdrop.PropertyOne = "#myfilms.fanart";
                    //backdrop.PropertyTwo = "#Fanart2";
                    //backdrop.LoadingImage = loadingImage;
                    // (re)link our backdrop image controls to the backdrop image swapper
                    backdrop.GUIImageOne = ImgFanart;
                    backdrop.GUIImageTwo = ImgFanart2;
  //                  backdrop.LoadingImage = loadingImage;

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
                    if ((conf.AlwaysDefaultView) && (PreviousWindowId != ID_MesFilmsDetail) && !MovieScrobbling)
                        Fin_Charge_Init(true,false);
                    else
                        Fin_Charge_Init(false, false);
                    return true;

                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT: //called when exiting plugin either by prev menu or pressing home button
                    GUITextureManager.CleanupThumbs();

                    if (Configuration.CurrentConfig != "")
                    {
                        if (facadeView == null || facadeView.SelectedListItemIndex == -1)
                            Configuration.SaveConfiguration(Configuration.CurrentConfig, -1, "");
                        else
                            Configuration.SaveConfiguration(Configuration.CurrentConfig, facadeView.SelectedListItem.ItemId, facadeView.SelectedListItem.Label);
                    }
                    facadeView.Resources.Clear();
                    facadeView.Clear();
                    backdrop.PropertyOne = " ";
                    return true; // fall through to call base class?

                case GUIMessage.MessageType.GUI_MSG_CLICKED:
                    //---------------------------------------------------------------------------------------
                    // Mouse/Keyboard Clicked
                    //---------------------------------------------------------------------------------------
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
                        if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
                        {
                            dlg.Add(GUILocalizeStrings.Get(10798614));//Search related movies by property
                            choiceSearch.Add("analogyproperty");
                        }
//Guzzi: RandomMovie(Trailer)Search added
                        dlg.Add(GUILocalizeStrings.Get(10798621));//Search global movies by random
                        choiceSearch.Add("randomsearch");

                        dlg.Add(GUILocalizeStrings.Get(10798645));//Search global movies by areas
                        choiceSearch.Add("globalareas");

                        dlg.Add(GUILocalizeStrings.Get(10798615));//Search global movies by property
                        choiceSearch.Add("globalproperty");
                        
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
                                    if (control_searchText(keyboard.Text))
                                    {
                                        conf.StrSelect = conf.StrSearchItem[i].ToString() + " like '*" + keyboard.Text + "*'";
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
        void SetFilmSelect()
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
            ImgLstFilm.SetFileName("#myfilms.picture");
            ImgLstFilm2.SetFileName("#myfilms.picture");

            int iCnt = 0;
            int DelimCnt = 0, DelimCnt2;
            GUIListItem item = new GUIListItem();
            string sTitle;
            string sFullTitle;
            string sPrevTitle = "";
            string SelItem = gSelItem.ToString();
            int iSelItem = -2;
            if (typeof(T) == typeof(int)) iSelItem = Int32.Parse(SelItem);

            SetFilmSelect();
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
            //Added GlobalFilterList for Trailer & Rating Filters !!!
            r = BaseMesFilms.LectureDonnées(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens);
            //r = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens);
            Log.Debug("MyFilms (GetFilmList) - GlobalFilterString: '" + GlobalFilterString + "'");
            Log.Debug("MyFilms (GetFilmList) - conf.StrDfltSelect: '" + conf.StrDfltSelect + "'");
            Log.Debug("MyFilms (GetFilmList) - conf.StrFilmSelect: '" + conf.StrFilmSelect + "'");
            Log.Debug("MyFilms (GetFilmList) - conf.StrSorta:      '" + conf.StrSorta + "'");
            Log.Debug("MyFilms (GetFilmList) - conf.StrSortSens:   '" + conf.StrSortSens + "'");
            foreach (DataRow sr in r)
            {
                number++;
                if (conf.Boolreturn)//in case of selection by view verify if value correspond excatly to the searched string
                {
                    w_tableau = Search_String(sr[conf.WStrSort].ToString());
                    for (int wi = 0; wi < w_tableau.Count;wi++ )
                    {
                        if ((conf.WStrSort == "Date") || (conf.WStrSort == "DateAdded"))
                        {
                            if (string.Format("{0:dd/MM/yyyy}", DateTime.Parse(w_tableau[wi].ToString()).ToShortDateString()) == string.Format("{0:dd/MM/yyyy}", DateTime.Parse(conf.Wselectedlabel).ToShortDateString()))
                                goto suite;
                        }
                        else
                        {
                            if (w_tableau[wi].ToString().ToLower().Contains(conf.Wselectedlabel.Trim().ToLower()))
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
                GUIControl.ShowControl(GetID, 34);
            else
                GUIControl.HideControl(GetID, 34);
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
        {
            //if (facadeView.SelectedListItem.ItemId == Prev_ItemID)
            //    return;
            Log.Debug("MyFilms : ItemId = " + ItemId.ToString() + ", wrep = " + wrep.ToString() + ", wlabel = " + wlabel);
            if (ItemId == -1)
            {
                // reinit some fields
                return;
            }
            if ((facadeView.SelectedListItem.IsFolder) && (MesFilms.conf.Boolselect))
            {
                string[] wfanart = MesFilmsDetail.Search_Fanart(wlabel, true, "file", true, facadeView.SelectedListItem.ThumbnailImage.ToString(), facadeView.SelectedListItem.Path);
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
                backdrop.Filename = wfanart[0]; 
                backdrop.Active = true;
                GUIControl.HideControl(GetID, 34);
                Prev_ItemID = facadeView.SelectedListItem.ItemId;
                GUIPropertyManager.SetProperty("#myfilms.picture", facadeView.SelectedListItem.ThumbnailImage.ToString()); 
                affichage_rating(0);
                GUIPropertyManager.SetProperty("#myfilms.logos_id2001", " ");
                GUIPropertyManager.SetProperty("#myfilms.logos_id2002", " ");
                //               return;
            }
            else
            {
                //string wtitle = MesFilms.r[ItemId]["OriginalTitle"].ToString();
                //if (MesFilms.r[ItemId]["TranslatedTitle"] != null && MesFilms.r[ItemId]["TranslatedTitle"].ToString().Length > 0)
                //    wtitle = MesFilms.r[ItemId]["TranslatedTitle"].ToString();
                ////if (wtitle.IndexOf(MesFilms.conf.TitleDelim) > 0)
                ////    wtitle = wtitle.Substring(wtitle.IndexOf(MesFilms.conf.TitleDelim) + 1);
                //string[] wfanart = MesFilmsDetail.Search_Fanart(wtitle, true, "file", false, facadeView.SelectedListItem.ThumbnailImage.ToString(), string.Empty);
                string[] wfanart = MesFilmsDetail.Search_Fanart(wlabel, true, "file", false, facadeView.SelectedListItem.ThumbnailImage.ToString(), string.Empty);
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
                backdrop.Filename = wfanart[0];
                if (facadeView.SelectedListItem.IsFolder)
                    Prev_ItemID = facadeView.SelectedListItem.ItemId;
                Prev_ItemID = facadeView.SelectedListItem.ItemId;
                conf.FileImage = facadeView.SelectedListItem.ThumbnailImage.ToString();
                GUIPropertyManager.SetProperty("#myfilms.picture", MesFilms.conf.FileImage.ToString());

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
        private bool control_searchText(string stext)
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

        #region Accès Données

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
            // Change global MovieFilter (Only Movies with Trailer)
            if (GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798628)));
            if (!GlobalFilterTrailersOnly) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798691), GUILocalizeStrings.Get(10798629)));
            choiceView.Add("filterdbtrailer");

            // Change global MovieFilter (Only Movies with highRating)
            if (GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798628)));
            if (!GlobalFilterMinRating) dlg1.Add(string.Format(GUILocalizeStrings.Get(10798692), GUILocalizeStrings.Get(10798629)));
            choiceView.Add("filterdbrating");

            // Change Value for global MovieFilter (Only Movies with highRating)
            dlg1.Add(string.Format(GUILocalizeStrings.Get(10798693), MesFilms.conf.StrAntFilterMinRating.ToString()));
            choiceView.Add("filterdbsetrating");

            if (MesFilms.conf.StrAMCUpd)
            {
                dlg1.Add(GUILocalizeStrings.Get(1079861));   // Change Config 
                choiceView.Add("updatedb");
            }
            if (MesFilms.conf.StrFanart)
            {
                dlg1.Add(GUILocalizeStrings.Get(4514));   // Download all Fanart
                choiceView.Add("downfanart");
            }

            dlg1.Add(GUILocalizeStrings.Get(10798694));   // Search and register all trailers for all movies in DB
            choiceView.Add("trailer-all");

            if (MesFilms.conf.StrGrabber)
            {
                if (MesFilms.conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798628)));   // Choose grabber script for that session (status on)
                if (!MesFilms.conf.StrGrabber_ChooseScript) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079863), GUILocalizeStrings.Get(10798629)));   // Choose grabber script for that session (status off)
                choiceView.Add("choosescript");

                if (MesFilms.conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798628)));   // Change grabber find trying best match option (status on)
                if (!MesFilms.conf.StrGrabber_Always) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079864), GUILocalizeStrings.Get(10798629)));   // Change grabber find trying best match option (status off)
                choiceView.Add("findbestmatch");
            }

            if (MesFilms.conf.WindowsFileDialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798628)));   // Using Windows File Dialog File for that session (status on)
            if (!MesFilms.conf.WindowsFileDialog) dlg1.Add(string.Format(GUILocalizeStrings.Get(1079865), GUILocalizeStrings.Get(10798629)));   // Using Windows File Dialog File for that session (status off)
            choiceView.Add("windowsfiledialog");
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

            string[] arSplit;
            int wi = 0;
            string[] Sep = conf.ListSeparator;
            arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries); // remove entries empty // StringSplitOptions.None);//will add "" entries also
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

            string[] arSplit;
            int wi = 0;
            string[] Sep = conf.ListSeparator;
            arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries); // remove entries empty // StringSplitOptions.None);//will add "" entries also
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

            string[] arSplit;
            int wi = 0;
            string[] Sep = {" - ", ":"}; //Only Dash as Separator for Movietitles !!!
            //string[] CleanerList = new string[] { "Der ", "der ", "Die ", "die ", "Das ", "das", "des", " so", "sich", " a ", " A ", "The ", "the ","- "," -"," AT "};
            arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries); // remove entries empty // StringSplitOptions.None);//will add "" entries also
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

            string[] arSplit;
            int wi = 0;
            //string[] Sep = conf.ListSeparator;
            string[] Sep = new string[]
                {" ",",",";","|","/","(",")",".",@"\",":"};
            arSplit = champselect.Split(Sep, StringSplitOptions.RemoveEmptyEntries); // remove entries empty // StringSplitOptions.None);//will add "" entries also
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
        /// <param name="p">Position in string to begin search/replacing from</param>
        /// <param name="ClearIndex">Reset Selected Item Index</param>
        /// <param name="SelItem">Select entry matching this string if not empty</param>
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
            //foreach (DataRow enr in BaseMesFilms.LectureDonnées(conf.StrDfltSelect, WstrSelect, WStrSort, WStrSortSens))
            foreach (DataRow enr in BaseMesFilms.LectureDonnées(GlobalFilterString + " " + conf.StrDfltSelect, WstrSelect, WStrSort, WStrSortSens))
                {
                if ((WStrSort == "Date") || (WStrSort == "DateAdded"))
                    champselect = string.Format("{0:yyyy/MM/dd}", enr["DateAdded"]);
                else
                    champselect = enr[WStrSort].ToString().Trim();
                ArrayList wtab = Search_String(champselect);
                for (wi = 0; wi < wtab.Count; wi++)
                {
                        w_tableau.Add(wtab[wi].ToString().Trim());
                }

            }
            if (WStrSortSens == " ASC")
                w_tableau.Sort(0, w_tableau.Count, null);
            else
            {
                IComparer myComparer = new myReverserClass();
                w_tableau.Sort(0, w_tableau.Count, myComparer);
            }
            item = new GUIListItem();
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
                            if (MesFilms.conf.StrViews)
                            {
                                if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others"))
                                    System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others");
                                string strThumb = Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others\\" + item.Label;

                                if (!System.IO.File.Exists(strThumb + ".png"))
                                {
                                    if (conf.StrPathViews.Length > 0)
                                        if (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\")
                                        {
                                            if (System.IO.File.Exists(conf.StrPathViews + item.Label + ".jpg"))
                                                Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".jpg", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                            else
                                                if (System.IO.File.Exists(conf.StrPathViews + item.Label + ".png"))
                                                    Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                        }
                                        else
                                        {
                                            if (System.IO.File.Exists(conf.StrPathViews + "\\" + item.Label + ".jpg"))
                                                Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".jpg", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                            else
                                                if (System.IO.File.Exists(conf.StrPathViews + "\\" + item.Label + ".png"))
                                                    Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                        }
                                    if (!System.IO.File.Exists(strThumb + ".png"))
                                        if (MesFilms.conf.StrViewsDflt && System.IO.File.Exists(MesFilms.conf.DefaultCover))
                                            ImageFast.CreateImage(strThumb + ".png", item.Label);

                                }
                                item.ThumbnailImage = strThumb + ".png";
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
            
            if ((Wnb_enr > 0) && (wchampselect.Length > 0))
            {
                item = new GUIListItem();
                item.Label = wchampselect.ToString();
                item.Label2 = Wnb_enr.ToString();
                if (MesFilms.conf.StrViews)
                {
                    if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others"))
                        System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others");
                    string strThumb = Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others\\" + item.Label;
                    if (!System.IO.File.Exists(strThumb + ".png"))
                    {
                        if (conf.StrPathViews.Length > 0)
                            if (conf.StrPathViews.Substring(conf.StrPathViews.Length - 1) == "\\")
                            {
                                if (System.IO.File.Exists(conf.StrPathViews + item.Label + ".jpg"))
                                    Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".jpg", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                else
                                    if (System.IO.File.Exists(conf.StrPathViews + item.Label + ".png"))
                                        Picture.CreateThumbnail(conf.StrPathViews + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                            }
                            else
                            {
                                if (System.IO.File.Exists(conf.StrPathViews + "\\" + item.Label + ".jpg"))
                                    Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".jpg", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                else
                                    if (System.IO.File.Exists(conf.StrPathViews + "\\" + item.Label + ".png"))
                                        Picture.CreateThumbnail(conf.StrPathViews + "\\" + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                            }
                        if (!System.IO.File.Exists(strThumb + ".png"))
                            ImageFast.CreateImage(strThumb + ".png", item.Label);
                    }
                    item.ThumbnailImage = strThumb + ".png";
                }
                item.IsFolder = true;
                item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                facadeView.Add(item);
                if (SelItem != "" && item.Label == SelItem) conf.StrIndex = facadeView.Count - 1; //test if this item is one to select
                Wnb_enr = 0;
            }
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
                GUIControl.HideControl(GetID, 34);
            else
            {
                GUIControl.ShowControl(GetID, 34);
                GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2001);
                GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2002);
                backdrop.Active = false;
                MesFilmsDetail.Load_Detailed_DB(0, false);
                ImgLstFilm.SetFileName("#myfilms.picture");
                ImgLstFilm2.SetFileName("#myfilms.picture");
                affichage_rating(0);
            }
            GUIPropertyManager.SetProperty("#myfilms.nbobjects", facadeView.Count.ToString() + " " + GUILocalizeStrings.Get(127));
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
        private void Load_Config(string CurrentConfig, bool create_temp)
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
                GlobalFilterString = "";
                MovieScrobbling = false;
            }
            if ((PreviousWindowId != ID_MesFilmsDetail) || (reload))
            {
                //chargement des films
                BaseMesFilms.LoadFilm(conf.StrFileXml);
                 r = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrFilmSelect, conf.StrSorta, conf.StrSortSens);
            }
            //Layout = conf.StrLayOut;
            
            if ((conf.CurrentSortMethod == "") || (conf.CurrentSortMethod == null))
                conf.CurrentSortMethod = GUILocalizeStrings.Get(103);
            else
                BtnSrtBy.Label = conf.CurrentSortMethod.ToString();
            string BtnSearchT = GUILocalizeStrings.Get(137);
            GUIButtonControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnSearchT, BtnSearchT);
            BtnSrtBy.SortChanged += new SortEventHandler(SortChanged);
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
            if (conf.LastID == ID_MesFilmsDetail)
            {
                GUIWindowManager.ActivateWindow(ID_MesFilmsDetail); // if last window in use was detailed one display that one again
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

                case "config":
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
                        Load_Config(newConfig, true);
                        Fin_Charge_Init(conf.AlwaysDefaultView,true); //need to load default view as asked in setup or load current selection as reloaded from myfilms.xml file to remember position
                    }
                    
                    break;

                case "filterdbtrailer":
                    // GlobalFilterTrailersOnly
                    GlobalFilterTrailersOnly = !GlobalFilterTrailersOnly;
                    Log.Info("MyFilms : Global filter for Trailers Only is now set to '" + GlobalFilterTrailersOnly.ToString() + "'");
                    if (1 == 1)
                    {
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));
                        dlgOk.SetLine(1, "");
                        if (GlobalFilterTrailersOnly) dlgOk.SetLine(2, GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798628));
                        if (!GlobalFilterTrailersOnly) dlgOk.SetLine(2, GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798629));
                        dlgOk.DoModal(GetID);
                    }
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    GlobalFilterString = "";
                    if (GlobalFilterMinRating) GlobalFilterString = GlobalFilterString + "Rating > " + MesFilms.conf.StrAntFilterMinRating.ToString() + " AND ";
                    if (GlobalFilterTrailersOnly) GlobalFilterString = GlobalFilterString + "Borrower not like '' AND ";
                    //if ((GlobalFilterMinRating) && (GlobalFilterTrailersOnly)) GlobalFilterString = GlobalFilterString + " AND ";
                    Log.Info("MyFilms (SetGlobalFilterString Trailers) - 'GlobalFilterString' = '" + GlobalFilterString + "'");
                    setProcessAnimationStatus(true, m_SearchAnimation);
                    Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    break;

                case "filterdbrating":
                    // GlobalFilterMinRating
                    GlobalFilterMinRating = !GlobalFilterMinRating;
                    Log.Info("MyFilms : Global filter for MinimumRating is now set to '" + GlobalFilterMinRating.ToString() + "'");
                    if (1 == 1)
                    {
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));
                        dlgOk.SetLine(1, "");
                        if (GlobalFilterMinRating) dlgOk.SetLine(2, GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798628));
                        if (!GlobalFilterMinRating) dlgOk.SetLine(2, GUILocalizeStrings.Get(10798630) + " = " + GUILocalizeStrings.Get(10798629));
                        dlgOk.DoModal(GetID);
                    }
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    GlobalFilterString = "";
                    if (GlobalFilterMinRating) GlobalFilterString = GlobalFilterString + "Rating > " + MesFilms.conf.StrAntFilterMinRating.ToString() + " AND ";
                    if (GlobalFilterTrailersOnly) GlobalFilterString = GlobalFilterString + "Borrower not like '' AND ";
                    //if ((GlobalFilterMinRating) && (GlobalFilterTrailersOnly)) GlobalFilterString = GlobalFilterString + " AND ";
                    Log.Info("MyFilms (SetGlobalFilterString MinRating) - 'GlobalFilterString' = '" + GlobalFilterString + "'");
                    setProcessAnimationStatus(true, m_SearchAnimation);
                    Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    break;
                case "filterdbsetrating":
                    // Set global value for minimum Rating to restrict movielist
                    Log.Info("MyFilms (FilterDbSetRating) - 'AntFilterMinRating' current setting = '" + MesFilms.conf.StrAntFilterMinRating.ToString() + "'");
                    MesFilmsDialogSetRating dlgRating = (MesFilmsDialogSetRating)GUIWindowManager.GetWindow(7988);
                    if (MesFilms.conf.StrAntFilterMinRating.ToString().Length > 0)
                        //dlgRating.Rating = (decimal)Convert.ToInt32(MesFilms.conf.StrAntFilterMinRating);
                        dlgRating.Rating = Convert.ToDecimal(MesFilms.conf.StrAntFilterMinRating.Replace(".", ","));
                    else
                        dlgRating.Rating = 0;
                    dlgRating.SetTitle("Bitte wählen sie den globalen Wert für die Minimalbewertung");
                    dlgRating.DoModal(GetID);
                    MesFilms.conf.StrAntFilterMinRating = dlgRating.Rating.ToString().Replace("," , ".");
                    XmlConfig.WriteXmlConfig("MyFilms", Configuration.CurrentConfig, "AntFilterMinRating", MesFilms.conf.StrAntFilterMinRating);
                    Log.Info("MyFilms (FilterDbSetRating) - 'AntFilterMinRating' changed to '" + MesFilms.conf.StrAntFilterMinRating.ToString() + "'");
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    GlobalFilterString = "";
                    if (GlobalFilterMinRating) GlobalFilterString = GlobalFilterString + "Rating > " + MesFilms.conf.StrAntFilterMinRating.ToString() + " AND ";
                    if (GlobalFilterTrailersOnly) GlobalFilterString = GlobalFilterString + "Borrower not like '' AND ";
                    //if ((GlobalFilterMinRating) && (GlobalFilterTrailersOnly)) GlobalFilterString = GlobalFilterString + " AND ";
                    Log.Info("MyFilms (SetGlobalFilterString) - 'GlobalFilterString' = '" + GlobalFilterString + "'");
                    if (GlobalFilterMinRating)
                    {
                        setProcessAnimationStatus(true, m_SearchAnimation);
                        Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                        setProcessAnimationStatus(false, m_SearchAnimation);
                    }
                    break;
                case "updatedb":
                    // Launch AMCUpdater in batch mode
                    if (bgUpdateDB.IsBusy)
                    {
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(1079861));//action already launched
                        dlgOk.SetLine(1, GUILocalizeStrings.Get(875));
                        dlgOk.SetLine(2, GUILocalizeStrings.Get(330));
                        dlgOk.DoModal(GetID);
                        break;
                    }
                    this.bgUpdateDB.DoWork += new DoWorkEventHandler(bgUpdateDB_DoWork);
                    this.bgUpdateDB.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateDB_RunWorkerCompleted);
                    AsynUpdateDatabase();
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;
                case "downfanart":
                    // Launch Fanart download in batch mode
                    if (bgUpdateFanart.IsBusy)
                    {
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(1079862));//action already launched
                        dlgOk.SetLine(1, GUILocalizeStrings.Get(921));
                        dlgOk.SetLine(2, GUILocalizeStrings.Get(330));
                        dlgOk.DoModal(GetID);
                        break;
                    }
                    this.bgUpdateFanart.DoWork += new DoWorkEventHandler(bgUpdateFanart_DoWork);
                    this.bgUpdateFanart.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgUpdateFanart_RunWorkerCompleted);
                    AsynUpdateFanart();
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    break;


                case "trailer-all":
                    // Search and register all trailers for all movies in DB
                    
                    AntMovieCatalog ds = new AntMovieCatalog();
                    ArrayList w_index = new ArrayList();
                    int w_index_count = 0;
                    string t_number_id = "";

                    DataRow[] wr = BaseMesFilms.LectureDonnées(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
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
                    dlgYesNo.SetLine(2, "Wollen Sie wirklich die Trailersuche für " + w_index_count + " Filme starten?");
                    dlgYesNo.DoModal(GetID);
                    if (!(dlgYesNo.IsConfirmed))
                        break;
                    //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                    //setProcessAnimationStatus(true, m_SearchAnimation);
                    
                    for (i = 0; i < w_index_count; i++)
                    {
                        Log.Debug("MyFilms (GlobalSearchTrailerLocal) - Number: '" + i.ToString() + "' - Index to search: '" + w_index[i] + "'");
                        //MesFilmsDetail.SearchTrailerLocal((DataRow[])MesFilms.r, Convert.ToInt32(w_index[i]));
                        MesFilmsDetail.SearchTrailerLocal((DataRow[])MesFilms.r, Convert.ToInt32(i), false);
                    }

                    GUIDialogOK dlgOk1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    Fin_Charge_Init(false, true); //NotDefaultSelect, Only reload
                    //setProcessAnimationStatus(false, m_SearchAnimation);
                    dlgOk1.SetHeading("Info");
                    dlgOk1.SetLine(1, "");
                    dlgOk1.SetLine(2, "Trailersuche beendet !");
                    dlgOk1.DoModal(GetID);
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
            }
        }
        //--------------------------------------------------------------------------------------------
        //   Display Context Menu for Movie 
        //--------------------------------------------------------------------------------------------
        // Changed from private to PUBLIC - GUZZI
        public void Context_Menu_Movie(int selecteditem)
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return;
            Context_Menu = true;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
            string[] upd_choice = new string[20];
            int ichoice = 0;

            if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
            {
                dlg.Add(GUILocalizeStrings.Get(1079866));//Search related movies by persons
                upd_choice[ichoice] = "analogyperson";
                ichoice++;
            }

            if (facadeView.SelectedListItemIndex > -1 && !facadeView.SelectedListItem.IsFolder)
            {
                dlg.Add(GUILocalizeStrings.Get(10798614));//Search related movies by property
                upd_choice[ichoice] = "analogyproperty";
                ichoice++;
            }

            if (MesFilms.conf.StrSuppress)
            {
                dlg.Add(GUILocalizeStrings.Get(432));
                upd_choice[ichoice] = "suppress";
                ichoice++;
            }
            if (MesFilms.conf.StrGrabber)
            {
                dlg.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
                upd_choice[ichoice] = "grabber";
                ichoice++;
            }
            if (MesFilms.conf.StrFanart == true)
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
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(1079867)); // menu
            ArrayList w_tableau = new ArrayList();
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            if (MesFilms.r[Index]["Producer"].ToString().Length > 0)
            {
                w_tableau = Search_String(MesFilms.r[Index]["Producer"].ToString());
                for (int wi = 0; wi < w_tableau.Count; wi++)
                {
                    dlg.Add(GUILocalizeStrings.Get(10798612) + " : " + w_tableau[wi]);
                    choiceSearch.Add(w_tableau[wi].ToString());
                }
            }
            if (MesFilms.r[Index]["Director"].ToString().Length > 0)
            {
                w_tableau = Search_String(MesFilms.r[Index]["Director"].ToString());
                for (int wi = 0; wi < w_tableau.Count; wi++)
                {
                    dlg.Add(GUILocalizeStrings.Get(1079869) + " : " + w_tableau[wi]);
                    choiceSearch.Add(w_tableau[wi].ToString());
                }
            }
            if (MesFilms.r[Index]["Actors"].ToString().Length > 0)
            {
                w_tableau = Search_String(MesFilms.r[Index]["Actors"].ToString());
                for (int wi = 0; wi < w_tableau.Count; wi++)
                {
                    dlg.Add(GUILocalizeStrings.Get(1079868) + " : " + w_tableau[wi]);
                    choiceSearch.Add(w_tableau[wi].ToString());
                }
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            string wperson = choiceSearch[dlg.SelectedLabel];
            dlg.Reset();
            choiceSearch.Clear();
            dlg.SetHeading(GUILocalizeStrings.Get(10798611) + wperson); // function selection (actor, director, producer)
            DataRow[] wr = BaseMesFilms.LectureDonnées(MesFilms.conf.StrDfltSelect, "Producer like '*" + wperson + "*'", MesFilms.conf.StrSorta, MesFilms.conf.StrSortSens, false);
            if (wr.Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(10798612) + "  (" + wr.Length.ToString() + ")");
                choiceSearch.Add("Producer");
            }
            wr = BaseMesFilms.LectureDonnées(MesFilms.conf.StrDfltSelect, "Director like '*" + wperson + "*'", MesFilms.conf.StrSorta, MesFilms.conf.StrSortSens, false);
            if (wr.Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(1079869) + "  (" + wr.Length.ToString() + ")");
                choiceSearch.Add("Director");
            }
            wr = BaseMesFilms.LectureDonnées(MesFilms.conf.StrDfltSelect, "Actors like '*" + wperson + "*'", MesFilms.conf.StrSorta, MesFilms.conf.StrSortSens, false);
            if (wr.Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(1079868) + "  (" + wr.Length.ToString() + ")");
                choiceSearch.Add("Actors");
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            conf.StrSelect = choiceSearch[dlg.SelectedLabel].ToString() + " like '*" + wperson + "*'";
            if (choiceSearch[dlg.SelectedLabel] == "Actors")
                conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(1079868) + " [*" + wperson + @"*]";
                else
                    if (choiceSearch[dlg.SelectedLabel] == "Director")
                        conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(1079869) + " [*" + wperson + @"*]";
                            else
                                conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(10798612) + " [*" + wperson + @"*]";
            conf.StrTitleSelect = "";
            GetFilmList();
        }
        //*****************************************************************************************
        //*  search related movies by properties                                                  *
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
                for (int wi = 0; wi < w_tableau.Count; wi++)
                {
                    for (int ii = 0; ii < 30; ii++)
                    {
                        if (wproperty.ToLower().Equals(PropertyList[ii].ToString().ToLower()))
                        {
                            dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + ": " + w_tableau[wi]);
                            //dlg.Add(wproperty + " : " + w_tableau[wi]);
                            choiceSearch.Add(w_tableau[wi].ToString());
                            Log.Debug("MyFilms (RelatedPropertySearch): Searchstring Result Add: '" + w_tableau[wi].ToString() + "'");
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
                    for (int wi = 0; wi < wsub_tableau.Count; wi++)
                    {
                        if (w_tableau.Contains(wsub_tableau[wi])) // Only Add SubWordItems if not already present in SearchStrin Table
                        {
                            Log.Debug("MyFilms (RelatedPropertySearch): Searchstring Result already Present: '" + wsub_tableau[wi].ToString() + "'");
                            break;
                        }
                        else
                        {
                            for (int ii = 0; ii < 30; ii++)
                            {
                                if (wproperty.ToLower().Equals(PropertyList[ii].ToString().ToLower()))
                                {
                                    dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + " (" + GUILocalizeStrings.Get(10798627) + "): '" + wsub_tableau[wi] + "'");
                                    //dlg.Add(GUILocalizeStrings.Get(Convert.ToInt32((PropertyListLabel[ii]))) + ": '" + wsub_tableau[wi] + "'");
                                    choiceSearch.Add(wsub_tableau[wi].ToString());
                                    Log.Debug("MyFilms (RelatedPropertySearch): Searchstring Result Add: '" + wsub_tableau[wi].ToString() + "'");
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
        //*****************************************************************************************
        //*  Global search movies by RANDOM (Random Search with Options, e.g. Trailer, Rating)    *
        //*****************************************************************************************

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
                    DataRow[] wr = BaseMesFilms.LectureDonnées(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                    //DataColumn[] wc = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
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
                                    if (GetSubItems == true)
                                    {
                                        Log.Debug("MyFilms SubItemGrabber: Input: " + wsr[dc.ColumnName].ToString());
                                        wsub_tableau = SubItemGrabbing(wsr[dc.ColumnName].ToString()); //Grab SubItems
                                        for (int wi = 0; wi < wsub_tableau.Count; wi++)
                                        {
                                            Log.Debug("MyFilms SubItemGrabber: Output: " + wsub_tableau[wi].ToString());
                                            {
                                                if (w_tableau.Contains(wsub_tableau[wi].ToString())) // search position in w_tableau for adding +1 to w_count
                                                {
                                                    //if (!w_index.Contains(
                                                    for (int i = 0; i < w_tableau.Count; i++)
                                                    {
                                                        if (w_tableau[i].ToString() == wsub_tableau[wi].ToString())
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
                                                    w_tableau.Add(wsub_tableau[wi].ToString());
                                                    w_count.Add(1);
                                                }
                                            }

                                        }

                                    }
                                    if (GetItems == true)
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
                    if (wproperty == "Rating")
                        conf.StrSelect = wproperty + " = " + wproperty2;
                    else
                        if (wproperty == "Number")
                            conf.StrSelect = wproperty + " = " + wproperty2;
                        else
                            if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623))) // Check, if emptypropertystring is set
                                conf.StrSelect = wproperty + " like ''";
                            else
                                conf.StrSelect = wproperty + " like '*" + wproperty2 + "*'";
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
                        //GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        //dlg.DeInit();
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        //dlgOk.Init();
                        dlgOk.SetHeading(GUILocalizeStrings.Get(10798621)); // menu for random search
                        dlgOk.SetLine(1, "Suchergebnis: 0");
                        dlgOk.SetLine(2, "Keine Filme in der Auswahl vorhanden");
                        dlgOk.DoModal(GetID);
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
                                MesFilmsDetail.Launch_Movie(facadeView.SelectedListItem.ItemId, GetID, null);
                                //MesFilmsDetail.Launch_Movie(Convert.ToInt32(w_index[RandomNumber]), GetID, null);
                                return;
                            case "PlayMovieTrailer":
                                //Hier muß irgendwie sichergestellt werden, daß nach Rückkehr keine Neuinitialisierung erfolgt (analog return von Details 7988
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
                                conf.StrIndex = facadeView.SelectedListItem.ItemId; //Guzzi: Muß hier erst der facadeview geladen werden?
                                conf.StrTIndex = facadeView.SelectedListItem.Label;
                                GUITextureManager.CleanupThumbs();
                                GUIWindowManager.ActivateWindow(ID_MesFilmsDetail);
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
                                Log.Debug("MyFilms RandomNumber: '" + RandomNumber + "'");
                                //MesFilmsDetail.Launch_Movie_Trailer(Convert.ToInt32(w_index[RandomNumber]), GetID, null);


                                GUIDialogYesNo dlg1YesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                                dlg1YesNo.SetHeading("Wollen Sie den Hauptfilm sehen?");
                                dlg1YesNo.SetLine(1, GUILocalizeStrings.Get(219));
                                dlg1YesNo.SetLine(2, "Zufällige Film ID = '" + w_index[RandomNumber] + "'");
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
            Log.Debug("MyFilms (SearRandomWithTrailer-Info): Here should happen the handling of menucontext....");
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
                    DataRow[] wr = BaseMesFilms.LectureDonnées(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
                    //DataColumn[] wc = BaseMesFilms.LectureDonnées(conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
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
                                    if (GetSubItems == true)
                                    {
                                        Log.Debug("MyFilms SubItemGrabber: Input: " + wsr[dc.ColumnName].ToString());
                                        wsub_tableau = SubItemGrabbing(wsr[dc.ColumnName].ToString()); //Grab SubItems
                                        for (int wi = 0; wi < wsub_tableau.Count; wi++)
                                        {
                                            Log.Debug("MyFilms SubItemGrabber: Output: " + wsub_tableau[wi].ToString());
                                            {
                                                if (w_tableau.Contains(wsub_tableau[wi].ToString())) // search position in w_tableau for adding +1 to w_count
                                                {
                                                    //if (!w_index.Contains(
                                                    for (int i = 0; i < w_tableau.Count; i++)
                                                    {
                                                        if (w_tableau[i].ToString() == wsub_tableau[wi].ToString())
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
                                                    w_tableau.Add(wsub_tableau[wi].ToString());
                                                    w_count.Add(1);
                                                }
                                            }

                                        }

                                    }
                                    if (GetItems == true)
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
                    if (wproperty == "Rating")
                        conf.StrSelect = wproperty + " = " + wproperty2;
                    else
                        if (wproperty == "Number")
                            conf.StrSelect = wproperty + " = " + wproperty2;
                        else
                            if (wproperty2 == string.Format(GUILocalizeStrings.Get(10798623))) // Check, if emptypropertystring is set
                                conf.StrSelect = wproperty + " is NULL";
                            else
                                conf.StrSelect = wproperty + " like '*" + wproperty2 + "*'";
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
        //*  Global search movies by properties                                                   *
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
                        DataRow[] wr = BaseMesFilms.LectureDonnées(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
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
                            if (wproperty == "rating")
                            //if (wproperty == GUILocalizeStrings.Get(10798658)) //If "Rating" selected ...
                                conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                            else
                                if (wproperty == "number")
                                //if (wproperty == GUILocalizeStrings.Get(10798650)) //If "Number" selected ...
                                    conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                                else
                                    conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
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
                            DataRow[] wdr = BaseMesFilms.LectureDonnées(GlobalFilterString + " " + conf.StrDfltSelect, conf.StrTitle1.ToString() + " like '*'", conf.StrSorta, conf.StrSortSens);
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

                            if (wproperty == "Rating")
                                conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                            else
                                if (wproperty == "Number")
                                    conf.StrSelect = wproperty + " = " + Convert.ToInt32(keyboard.Text);
                                else
                                    conf.StrSelect = wproperty + " like '*" + keyboard.Text + "*'";
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

        void bgUpdateDB_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
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

        void bgUpdateFanart_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            string wtitle = string.Empty;
            for (int i = 0; i < MesFilms.r.Length; i++)
            {
                if (MesFilms.r[i]["OriginalTitle"] != null && MesFilms.r[i]["OriginalTitle"].ToString().Length > 0)
                    wtitle = MesFilms.r[i]["OriginalTitle"].ToString();
                if (wtitle.IndexOf(MesFilms.conf.TitleDelim) > 0)
                    wtitle = wtitle.Substring(wtitle.IndexOf(MesFilms.conf.TitleDelim) + 1);
                string wttitle = string.Empty;
                if (MesFilms.r[i]["TranslatedTitle"] != null && MesFilms.r[i]["TranslatedTitle"].ToString().Length > 0)
                    wttitle = MesFilms.r[i]["TranslatedTitle"].ToString();
                if (wttitle.IndexOf(MesFilms.conf.TitleDelim) > 0)
                    wttitle = wttitle.Substring(wttitle.IndexOf(MesFilms.conf.TitleDelim) + 1);
                if (MesFilms.r[i]["OriginalTitle"].ToString().Length > 0)

                {
                    int wyear = 0;
                    try { wyear = System.Convert.ToInt16(MesFilms.r[i]["Year"]); }
                    catch { }
                    string wdirector = string.Empty;
                    try { wdirector = (string)MesFilms.r[i]["Director"]; }
                    catch { }
                    System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, wdirector, MesFilms.conf.StrPathFanart, true, false);
                }
            }
        }
        void bgUpdateFanart_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
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

        void bgLoadMovieList_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string searchrep = MesFilms.conf.StrDirStor;
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if ((d.DriveType.ToString() == "CDRom") && (d.IsReady == true))
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
        void bgLoadMovieList_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Log.Info("MyFilms : Loading Movie List in batch mode finished");
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

#endregion
    }
}