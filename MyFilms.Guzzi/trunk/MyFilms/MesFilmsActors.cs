#region Copyright (C) 2005-2008 Team MediaPortal

/* 
 *	Copyright (C) 2005-2008 Team MediaPortal
 *	http://www.team-mediaportal.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using MediaPortal.Util;
using MediaPortal.Video.Database;
using MediaPortal.Configuration;

namespace MesFilms.Actors

{
    /// <summary>
    /// Opens a separate page to display Actor Infos
    /// </summary>
    public class MesFilmsActors : GUIWindow
    {
        #region Skin ID descriptions

        enum Controls : int
        {
            CTRL_BtnReturn = 10402,
            CTRL_Fanart = 1000,
            CTRL_FanartDir = 1001,
            CTRL_MovieThumbs = 10201,
            CTRL_MovieThumbsDir = 10202,

            CTRL_BtnSrtBy = 2,
            CTRL_BtnViewAs = 3,
            CTRL_BtnSearchT = 4,
            CTRL_BtnOptions = 5,
            CTRL_BtnLayout = 6,
            CTRL_TxtSelect = 10412,
            //CTRL_TxtSelect = 12,
            CTRL_Fanart1 = 11,
            CTRL_Fanart2 = 21,
            CTRL_Image = 1020,
            CTRL_Image2 = 1021,
            CTRL_List = 10401,
            // ID 3004 aus MesFilms für Wait Symbol (Setvisilility)
        }

        [SkinControl(10101)]
        protected GUIButtonControl CTRL_TxtSelect;
        [SkinControl(10102)]
        protected GUISortButtonControl CTRL_BtnReturn;

        [SkinControlAttribute((int)Controls.CTRL_FanartDir)]
        protected GUIMultiImage ImgFanartDir;
        //[SkinControlAttribute((int)Controls.CTRL_MovieThumbs)]
        //protected GUIImage ImgMovieThumbs = null;
        //[SkinControlAttribute((int)Controls.CTRL_MovieThumbsDir)]
        //protected GUIMultiImage ImgMovieThumbsDir = null;

        [SkinControlAttribute((int)Controls.CTRL_BtnSrtBy)]
        protected GUISortButtonControl BtnSrtBy;

        [SkinControlAttribute((int)Controls.CTRL_List)]
        protected GUIFacadeControl facadeView;

        [SkinControlAttribute((int)Controls.CTRL_Image)]
        protected GUIImage ImgLstFilm;

        [SkinControlAttribute((int)Controls.CTRL_Image2)]
        protected GUIImage ImgLstFilm2;

        [SkinControlAttribute((int)Controls.CTRL_Fanart1)]
        protected GUIImage ImgFanart1;

        [SkinControlAttribute((int)Controls.CTRL_Fanart2)]
        protected GUIImage ImgFanart2;

        [SkinControlAttribute(3004)]
        protected GUIAnimation m_SearchAnimation;


        public const int ID_MesFilms = 7986;
        public int ID_MesFilmsDetail = 7987;
        public int ID_MesFilmsActors = 7989;
        public int ID_MesFilmsThumbs = 7990;
        public int ID_MesFilmsActorsInfo = 7991;

        #endregion

        public static string wsearchfile;
        public static int wGetID;

        public int Layout = 0;
        public static int Prev_ItemID = -1;
        public bool Context_Menu = false;
        //public static Configuration conf;
        //public static Logos confLogos;
        //private string currentConfig;
        //private Cornerstone.MP.ImageSwapper backdrop;

        
        private List<string> list;

        enum View : int
        {
            List = 0,
            Icons = 1,
            LargeIcons = 2,
        }


        private enum ViewMode
        {
            Biography,
            Movies,
        }

        #region Base Dialog Variables

        private bool m_bRunning = false;
        private int m_dwParentWindowID = 0;
        private GUIWindow m_pParentWindow = null;

        #endregion

        private IMDBActor currentActor = null;
        private string imdbCoverArtUrl = string.Empty;

        //Pfad für ActorThumbs
        //MesFilms.conf.StrDirStorActorThumbs.ToString()
        //string strDir = MesFilms.conf.StrDirStorActorThumbs;


        public MesFilmsActors()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public override int GetID
        {
            get { return ID_MesFilmsActors; }
            set { base.GetID = value; }
        }

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\MesFilmsActors.xml");
        }

        public override void PreInit() { }


        //---------------------------------------------------------------------------------------
        //   Handle Keyboard Actions
        //---------------------------------------------------------------------------------------
        public override void OnAction(MediaPortal.GUI.Library.Action actionType)
        {
            Log.Debug("MyFilmsActors: OnAction " + actionType.wID);
            if ((actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) || (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PARENT_DIR))
            {
                MesFilms.conf.LastID = MesFilms.ID_MesFilms;
                GUIWindowManager.ActivateWindow(ID_MesFilms);
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
                    base.OnMessage(messageType);
                    wGetID = GetID;
                    MesFilms.conf.LastID = MesFilms.ID_MesFilmsActors;


                    //Temporary set item in facadeview...
                    GUIListItem item = new GUIListItem();
                    Prev_ItemID = -1;
                    ArrayList w_tableau = new ArrayList();

                    BtnSrtBy.Label = GUILocalizeStrings.Get(103);
                    MesFilms.conf.Boolselect = true;
                    MesFilms.conf.Wselectedlabel = "";
                    Change_LayOut(0);
                    facadeView.Clear();

                    item = new GUIListItem();
                    //item.Label = wchampselect.ToString();
                    //item.Label2 = Wnb_enr.ToString();
                    item.Label = "Arnold Schwarzenegger";
                    item.Label2 = "als Terminator";
                    item.Label3 = "n/a";
                    item.IsFolder = true;
                    facadeView.Add(item);

                    item.FreeMemory();
                    item.Label = "Jim Knopf";
                    item.Label2 = "als Lukas, der Lokomotivführer";
                    item.Label3 = "n/a";
                    item.IsFolder = true;
                    item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                    facadeView.Add(item);
                    //End Testfacadeinfos

                    return true;

                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT: //called when exiting plugin either by prev menu or pressing home button
                    if (Configuration.CurrentConfig != "")
                        Configuration.SaveConfiguration(Configuration.CurrentConfig, MesFilms.conf.StrIndex, MesFilms.conf.StrTIndex);
                    using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml")))
                    {
                        string currentmoduleid = "7989";
                        bool currentmodulefullscreen = (GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_TVFULLSCREEN || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_MUSIC || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_TELETEXT);
                        string currentmodulefullscreenstate = GUIPropertyManager.GetProperty("#currentmodulefullscreenstate");
                        // if MP was closed/hibernated by the use of remote control, we have to retrieve the fullscreen state in an alternative manner.
                        if (!currentmodulefullscreen && currentmodulefullscreenstate == "True")
                            currentmodulefullscreen = true;
                        xmlreader.SetValue("general", "lastactivemodule", currentmoduleid);
                        xmlreader.SetValueAsBool("general", "lastactivemodulefullscreen", currentmodulefullscreen);
                        Log.Debug("MyFilms : SaveLastActiveModule - module {0}", currentmoduleid);
                        Log.Debug("MyFilms : SaveLastActiveModule - fullscreen {0}", currentmodulefullscreen);
                    }
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
                        MesFilms.conf.LastID = MesFilms.ID_MesFilms;
                        GUITextureManager.CleanupThumbs();
                        GUIWindowManager.ActivateWindow(ID_MesFilms);
                        return true;
                    }

                    if (iControl == (int)Controls.CTRL_BtnReturn)

                        // Show Actor Details Screen
                        //GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_HOME);
                        GUIWindowManager.ActivateWindow(ID_MesFilmsActors);
                    // Hier Aktivitäten wie z.b. ListControl für Actors?
                    GUIWindowManager.ShowPreviousWindow();
                    //Update_XML_Items(); //To be changed, when DetailScreen is done!!!
                    return true;

                    if ((iControl == (int)Controls.CTRL_BtnLayout) && !MesFilms.conf.Boolselect)
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
                        // dlg.Add(GUILocalizeStrings.Get(791));//Coverflow - add when Coverflow gets incorporated
                        dlg.DoModal(GetID);

                        if (dlg.SelectedLabel == -1)
                            return true;
                        //conf.StrIndex = 0;
                        int wselectindex = facadeView.SelectedListItemIndex;
                        Change_LayOut(dlg.SelectedLabel);
                        MesFilms.conf.StrLayOut = dlg.SelectedLabel;
                        dlg.DeInit();
                        //GetFilmList();
                        GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, (int)wselectindex);
                        GUIControl.FocusControl(GetID, (int)Controls.CTRL_List);
                        return base.OnMessage(messageType);
                    }

                    if (iControl == (int)Controls.CTRL_List)
                    {
                        if (facadeView.SelectedListItemIndex > -1)
                        {
                            if (!facadeView.SelectedListItem.IsFolder && !MesFilms.conf.Boolselect)
                            // New Window for detailed selected item information
                            {
                                MesFilms.conf.StrIndex = facadeView.SelectedListItem.ItemId;
                                MesFilms.conf.StrTIndex = facadeView.SelectedListItem.Label;
                                GUITextureManager.CleanupThumbs();
                                GUIWindowManager.ActivateWindow(ID_MesFilmsDetail);
                            }
                            else
                            // View List as selected
                            {
                                MesFilms.conf.Wselectedlabel = facadeView.SelectedListItem.Label;
                                Change_LayOut(MesFilms.conf.StrLayOut);
                                if (facadeView.SelectedListItem.IsFolder)
                                    MesFilms.conf.Boolreturn = false;
                                else
                                    MesFilms.conf.Boolreturn = true;
                                //do
                                //{
                                //    if (MesFilms.conf.StrTitleSelect != "") MesFilms.conf.StrTitleSelect += MesFilms.conf.TitleDelim;
                                //    MesFilms.conf.StrTitleSelect += MesFilms.conf.Wselectedlabel;
                                //} while (GetFilmList() == false); //keep calling while single folders found
                            }
                        }
                    }

            }
            base.OnMessage(messageType);
            return true;
        }
        
        
        
        #region Base Dialog Members

        public void DoModal(int dwParentId)
        {
            m_dwParentWindowID = dwParentId;
            m_pParentWindow = GUIWindowManager.GetWindow(m_dwParentWindowID);
            if (null == m_pParentWindow)
            {
                m_dwParentWindowID = 0;
                return;
            }

            GUIWindowManager.IsSwitchingToNewWindow = true;
            GUIWindowManager.RouteToWindow(GetID);

            // active this window...
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_INIT, GetID, 0, 0, 0, 0, null);
            OnMessage(msg);

            //GUILayerManager.RegisterLayer(this, GUILayerManager.LayerType.Dialog);

            GUIWindowManager.IsSwitchingToNewWindow = false;
            m_bRunning = true;
            while (m_bRunning && GUIGraphicsContext.CurrentState == GUIGraphicsContext.State.RUNNING)
            {
                GUIWindowManager.Process();
            }
            //GUILayerManager.UnRegisterLayer(this);
        }

        #endregion

        protected override void OnPageLoad()
        {
            list = new List<string>();

            base.OnPageLoad();
            Update();
        }

        protected override void OnPageDestroy(int newWindowId)
        {
            if (m_bRunning)
            {
                // User probably pressed H (SWITCH_HOME)
                m_bRunning = false;
                GUIWindowManager.UnRoute();
                m_pParentWindow = null;
            }

            base.OnPageDestroy(newWindowId);
            currentActor = null;
        }


        public IMDBActor Actor
        {
            get { return currentActor; }
            set { currentActor = value; }
        }

        private void Update()
        {
            if (currentActor == null)
            {
                return;
            }

        }

        private int GetSelectedItemNo()
        {
            return facadeView.SelectedListItemIndex;
        }

        private GUIListItem GetSelectedItem()
        {
            return facadeView.SelectedListItem;
        }




        /// <summary>Selects records for display grouping them as required</summary>
        /// <param name="WstrSelect">Select this kind of records</param>
        /// <param name="WStrSort">Sort based on this</param>
        /// <param name="WStrSortSens">Asc/Desc. Ascending or descending sort order</param>
        /// <param name="NewWstar">Entries must contain this string to be included</param>
        /// <param name="ClearIndex">Reset Selected Item Index</param>
        /// <param name="SelItem">Select entry matching this string if not empty</param>
        public void getSelectFromActors(string WstrSelect, string WStrSort, string WStrSortSens, string NewWstar, bool ClearIndex, string SelItem)
        {
            GUIListItem item = new GUIListItem();
            Prev_ItemID = -1;
            string champselect = "";
            string wchampselect = "";
            ArrayList w_tableau = new ArrayList();
            int Wnb_enr = 0;

            MesFilms.conf.Wstar = NewWstar;
            BtnSrtBy.Label = GUILocalizeStrings.Get(103);
            MesFilms.conf.Boolselect = true;
            MesFilms.conf.Wselectedlabel = "";
            if (ClearIndex)
                MesFilms.conf.StrIndex = 0;
            Change_LayOut(0);
            facadeView.Clear();
            int wi = 0;

            foreach (DataRow enr in BaseMesFilms.LectureDonnées(MesFilms.conf.StrDfltSelect, WstrSelect, WStrSort, WStrSortSens))
            {
                if ((WStrSort == "Date") || (WStrSort == "DateAdded"))
                    champselect = string.Format("{0:yyyy/MM/dd}", enr["DateAdded"]);
                else
                    champselect = enr[WStrSort].ToString().Trim();
                ArrayList wtab = MesFilms.Search_String(champselect);
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
                if (string.Compare(champselect, wchampselect, true) == 0)
                    Wnb_enr++;
                else
                {
                    if (MesFilms.conf.Wstar == "*" || champselect.ToUpper().Contains(MesFilms.conf.Wstar.ToUpper()))
                    {
                        if ((Wnb_enr > 0) && (wchampselect.Length > 0))
                        {
                            item = new GUIListItem();
                            item.Label = wchampselect;
                            item.Label2 = Wnb_enr.ToString();
                            if (MesFilms.conf.StrViews)
                            {
                                if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others"))
                                    System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others");
                                string strThumb = Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others\\" + item.Label;

                                if (!System.IO.File.Exists(strThumb + ".png"))
                                {
                                    if (MesFilms.conf.StrPathViews.Length > 0)
                                        if (MesFilms.conf.StrPathViews.Substring(MesFilms.conf.StrPathViews.Length - 1) == "\\")
                                        {
                                            if (System.IO.File.Exists(MesFilms.conf.StrPathViews + item.Label + ".jpg"))
                                                Picture.CreateThumbnail(MesFilms.conf.StrPathViews + item.Label + ".jpg", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                            else
                                                if (System.IO.File.Exists(MesFilms.conf.StrPathViews + item.Label + ".png"))
                                                    Picture.CreateThumbnail(MesFilms.conf.StrPathViews + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                        }
                                        else
                                        {
                                            if (System.IO.File.Exists(MesFilms.conf.StrPathViews + "\\" + item.Label + ".jpg"))
                                                Picture.CreateThumbnail(MesFilms.conf.StrPathViews + "\\" + item.Label + ".jpg", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                            else
                                                if (System.IO.File.Exists(MesFilms.conf.StrPathViews + "\\" + item.Label + ".png"))
                                                    Picture.CreateThumbnail(MesFilms.conf.StrPathViews + "\\" + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                        }
                                }
                                item.ThumbnailImage = strThumb + ".png";
                            }
                            //Guzzi temp disabled
                            //string[] wfanart;
                            //if (WStrSort.ToLower() == "category" || WStrSort.ToLower() == "year" || WStrSort.ToLower() == "country")
                                //wfanart = MesFilmsDetail.Search_Fanart(item.Label, true, "file", true, item.ThumbnailImage, WStrSort.ToLower());
                            item.IsFolder = true;
                            item.Path = WStrSort.ToLower();
                            item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                            facadeView.Add(item);
                            if (SelItem != "" && item.Label == SelItem) MesFilms.conf.StrIndex = facadeView.Count - 1; //test if this item is one to select
                        }
                        Wnb_enr = 1;
                        wchampselect = champselect;
                    }
                }
            }

            if ((Wnb_enr > 0) && (wchampselect.Length > 0))
            {
                item = new GUIListItem();
                item.Label = wchampselect;
                item.Label2 = Wnb_enr.ToString();
                if (MesFilms.conf.StrViews)
                {
                    if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others"))
                        System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others");
                    string strThumb = Config.GetDirectoryInfo(Config.Dir.Thumbs) + "\\MyFilms_Others\\" + item.Label;
                    if (!System.IO.File.Exists(strThumb + ".png"))
                    {
                        if (MesFilms.conf.StrPathViews.Length > 0)
                            if (MesFilms.conf.StrPathViews.Substring(MesFilms.conf.StrPathViews.Length - 1) == "\\")
                            {
                                if (System.IO.File.Exists(MesFilms.conf.StrPathViews + item.Label + ".jpg"))
                                    Picture.CreateThumbnail(MesFilms.conf.StrPathViews + item.Label + ".jpg", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                else
                                    if (System.IO.File.Exists(MesFilms.conf.StrPathViews + item.Label + ".png"))
                                        Picture.CreateThumbnail(MesFilms.conf.StrPathViews + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                            }
                            else
                            {
                                if (System.IO.File.Exists(MesFilms.conf.StrPathViews + "\\" + item.Label + ".jpg"))
                                    Picture.CreateThumbnail(MesFilms.conf.StrPathViews + "\\" + item.Label + ".jpg", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                                else
                                    if (System.IO.File.Exists(MesFilms.conf.StrPathViews + "\\" + item.Label + ".png"))
                                        Picture.CreateThumbnail(MesFilms.conf.StrPathViews + "\\" + item.Label + ".png", strThumb + ".png", 400, 600, 0, Thumbs.SpeedThumbsLarge);
                            }
                    }
                    item.ThumbnailImage = strThumb + ".png";
                }
                item.IsFolder = true;
                item.OnItemSelected += new MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                facadeView.Add(item);
                if (SelItem != "" && item.Label == SelItem) MesFilms.conf.StrIndex = facadeView.Count - 1; //test if this item is one to select
                Wnb_enr = 0;
            }
            item.FreeMemory();
            MesFilms.conf.StrTxtSelect = "Selection";
            if (MesFilms.conf.Wstar != "*") MesFilms.conf.StrTxtSelect += " " + GUILocalizeStrings.Get(344) + " [*" + MesFilms.conf.Wstar + "*]";
            GUIPropertyManager.SetProperty("#myfilms.select", MesFilms.conf.StrTxtSelect);
            //            TxtSelect.Label = conf.StrTxtSelect;
            MesFilms.conf.StrSelect = WstrSelect;
            MesFilms.conf.StrFilmSelect = "";

            if ((MesFilms.conf.StrIndex > facadeView.Count - 1) || (MesFilms.conf.StrIndex < 0)) //check index within bounds, will be unless xml file heavily edited
                MesFilms.conf.StrIndex = 0;
            if (facadeView.Count == 0)
                GUIControl.HideControl(GetID, 34);
            else
            {
                GUIControl.ShowControl(GetID, 34);
                //GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2001);
                //GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2002);
                //backdrop.Active = false;
                MesFilmsDetail.Load_Detailed_DB(0, false);

                // Disabled because replaced by SpeedLoader
                //ImgLstFilm.SetFileName("#myfilms.picture");
                //ImgLstFilm2.SetFileName("#myfilms.picture");
                //affichage_rating(0);
            }
            GUIPropertyManager.SetProperty("#myfilms.nbobjects", facadeView.Count + " " + GUILocalizeStrings.Get(127));
            GUIControl.SelectItemControl(GetID, (int)Controls.CTRL_List, MesFilms.conf.StrIndex);

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
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(791));
                    facadeView.View = GUIFacadeControl.ViewMode.Filmstrip;
                    // To be changed when Coverflow is available in CORE Files ....
                    //facadeView.View = GUIFacadeControl.ViewMode.CoverFlow;
                    break;
                default:
                    GUIControl.SetControlLabel(GetID, (int)Controls.CTRL_BtnLayout, GUILocalizeStrings.Get(101));
                    facadeView.View = GUIFacadeControl.ViewMode.List;
                    break;
            }
        }

        private void item_OnItemSelected(GUIListItem item, GUIControl parent)
        {
            GUIFilmstripControl filmstrip = parent as GUIFilmstripControl;
            if (filmstrip != null)
                filmstrip.InfoImageFileName = item.ThumbnailImage;
            if (!(MesFilms.conf.Boolselect || (facadeView.SelectedListItemIndex > -1 && facadeView.SelectedListItem.IsFolder))) //xxxx
            {
                if (facadeView.SelectedListItemIndex > -1)
                    affichage_Lstdetail(facadeView.SelectedListItem.ItemId, true, facadeView.SelectedListItem.Label);
            }
            else
            {
                if (facadeView.SelectedListItemIndex > -1 && !MesFilms.conf.Boolselect)
                    affichage_Lstdetail(facadeView.SelectedListItem.ItemId, false, facadeView.SelectedListItem.Label);
                else
                {
                    affichage_Lstdetail(facadeView.SelectedListItem.ItemId, false, facadeView.SelectedListItem.Label);
                    GUIControl.ShowControl(GetID, 34);
                    //affichage_rating(0);
                }
            }
            //affichage_Lstdetail(item.ItemId, true, item.Label);
        }

        private static void affichage_Lstdetail(int ItemId, bool wrep, string wlabel)//wrep = false display only image
        {
            return;
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



        //*****************************************************************************************
        //*  Build Personlist with function of current movie
        //*****************************************************************************************
        private void Actors_SearchRelatedMoviesbyPersons(int Index)
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(1079867)); // menu
            ArrayList w_tableau = new ArrayList();
            System.Collections.Generic.List<string> choiceSearch = new System.Collections.Generic.List<string>();
            if (MesFilms.r[Index]["Producer"].ToString().Length > 0)
            {
                w_tableau = MesFilms.Search_String(MesFilms.r[Index]["Producer"].ToString());
                foreach (object t in w_tableau)
                {
                    dlg.Add(GUILocalizeStrings.Get(10798612) + " : " + t);
                    choiceSearch.Add(t.ToString());
                }
            }
            if (MesFilms.r[Index]["Director"].ToString().Length > 0)
            {
                w_tableau = MesFilms.Search_String(MesFilms.r[Index]["Director"].ToString());
                foreach (object t in w_tableau)
                {
                    dlg.Add(GUILocalizeStrings.Get(1079869) + " : " + t);
                    choiceSearch.Add(t.ToString());
                }
            }
            if (MesFilms.r[Index]["Actors"].ToString().Length > 0)
            {
                w_tableau = MesFilms.Search_String(MesFilms.r[Index]["Actors"].ToString());
                foreach (object t in w_tableau)
                {
                    dlg.Add(GUILocalizeStrings.Get(1079868) + " : " + t);
                    choiceSearch.Add(t.ToString());
                }
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

            DataRow[] wr = BaseMesFilms.LectureDonnées(MesFilms.conf.StrDfltSelect, "Producer like '*" + wperson + "*'", MesFilms.conf.StrSorta, MesFilms.conf.StrSortSens, false);
            if (wr.Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(10798612) + "  (" + wr.Length + ")");
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
                dlg.Add(GUILocalizeStrings.Get(10798610) + GUILocalizeStrings.Get(1079868) + "  (" + wr.Length + ")");
                choiceSearch.Add("Actors");
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1)
                return;
            MesFilms.conf.StrSelect = choiceSearch[dlg.SelectedLabel].ToString() + " like '*" + wperson + "*'";
            switch (choiceSearch[dlg.SelectedLabel])
            {
                case "Actors":
                    MesFilms.conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(1079868) + " [*" + wperson + @"*]";
                    break;
                case "Director":
                    MesFilms.conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(1079869) + " [*" + wperson + @"*]";
                    break;
                default:
                    MesFilms.conf.StrTxtSelect = "Selection " + GUILocalizeStrings.Get(10798612) + " [*" + wperson + @"*]";
                    break;
            }
            MesFilms.conf.StrTitleSelect = "";
            //GetFilmList();
        }



        private void personinfo(string wperson, int actorID)
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
                Log.Debug("MyFilms (Person Info): " + actorList.Count + " ActorID(s) found for '" + wperson + "'");
                //int actorID;
                actorID = 0;
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
                    string actorname = strActor[1];
                    Log.Debug("MyFilms (ActorDetails - Person Info): ActorID: '" + actorID + "' with ActorName: '" + actorname + "' found found for '" + wperson + "'");
                }
                
                MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(actorID);
                //MediaPortal.Video.Database.IMDBActor actor = MediaPortal.Video.Database.VideoDatabase.GetActorInfo(1);
                //if (actor != null)

                //OnVideoArtistInfoGuzzi(actor); // hier nicht erreichbar, Ersatz notwendig !!!
                return;
            }


    }
}