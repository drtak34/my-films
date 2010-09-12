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
using System.Collections.Generic;
using System.IO;
using System.Threading;

using MediaPortal.GUI.Library;
using MediaPortal.Util;
using MediaPortal.GUI.Pictures;
using MediaPortal.Video.Database;
using MesFilms;

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
            CTRL_TxtSelect = 10401,
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
            //CTRL_BtnChangeDB = 7, Not used, done in options instead!
            //CTRL_TxtSelect = 12,
            CTRL_Fanart1 = 11,
            CTRL_Fanart2 = 21,
            CTRL_Image = 1020,
            CTRL_Image2 = 1021,
            CTRL_List = 1026,
            // ID 3004 aus MesFilms für Wait Symbol (Setvisilility)
        }

        [SkinControl(10101)]
        protected GUIButtonControl CTRL_TxtSelect = null;
        [SkinControl(10102)]
        protected GUISortButtonControl CTRL_BtnReturn = null;

        [SkinControlAttribute((int)Controls.CTRL_FanartDir)]
        protected GUIMultiImage ImgFanartDir = null;
        //[SkinControlAttribute((int)Controls.CTRL_MovieThumbs)]
        //protected GUIImage ImgMovieThumbs = null;
        //[SkinControlAttribute((int)Controls.CTRL_MovieThumbsDir)]
        //protected GUIMultiImage ImgMovieThumbsDir = null;

        [SkinControlAttribute((int)Controls.CTRL_BtnSrtBy)]
        protected GUISortButtonControl ActorBtnSrtBy = null;

        [SkinControlAttribute((int)Controls.CTRL_List)]
        protected GUIFacadeControl facadeView = null;

        [SkinControlAttribute((int)Controls.CTRL_Image)]
        protected GUIImage ImgLstFilm = null;

        [SkinControlAttribute((int)Controls.CTRL_Image2)]
        protected GUIImage ImgLstFilm2 = null;

        [SkinControlAttribute((int)Controls.CTRL_Fanart1)]
        protected GUIImage ImgFanart1 = null;

        [SkinControlAttribute((int)Controls.CTRL_Fanart2)]
        protected GUIImage ImgFanart2 = null;

        [SkinControlAttribute(3004)]
        protected GUIAnimation m_SearchAnimation = null;


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
        public Cornerstone.MP.ImageSwapper backdrop;

        
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
            GetID = (int) 7989;
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
            Log.Debug("MyFilmsActors: OnAction " + actionType.wID.ToString());
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
    }
}