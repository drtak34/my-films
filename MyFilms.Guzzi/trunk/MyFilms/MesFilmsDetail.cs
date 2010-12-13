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
using System.Data;
using System.Diagnostics;
using System.IO;
using grabber;
using MediaPortal.Util;
using MediaPortal.Playlists;
using MediaPortal.Dialogs;
using MediaPortal.Player;

using System.Globalization;
using MediaPortal.GUI.Library;
using SQLite.NET;
using MediaPortal.Configuration;
using System.Xml;
using System.Linq;
using MesFilms.MyFilms;
using MediaPortal.Profile;
using System.Net;
using MesFilms.WakeOnLan;
using MediaPortal.Video.Database;

//using Cornerstone.MP;

namespace MesFilms
{
    /// <summary>
    /// Summary description for GUIMesFilms.
    /// </summary>
    public class MesFilmsDetail : GUIWindow
    {
        #region Descriptif zones Ecran

        enum Controls : int
        {
            CTRL_TxtSelect = 12,
            CTRL_BtnPlay = 101,
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
            CTRL_Title = 2025,
            CTRL_OTitle = 2026,
            CTRL_lblGenre = 2032,
            CTRL_Genre = 2062,
            CTRL_Format = 2069,
            CTRL_ImgDD = 2072,
            CTRL_ActorMultiThumb = 3333,
        }
        [SkinControlAttribute((int)Controls.CTRL_BtnMaj)]
        protected GUIButtonControl BtnMaj = null;
        [SkinControlAttribute(2024)]
        protected GUIImage ImgDetFilm = null;
        [SkinControlAttribute(2023)]
        protected GUIImage ImgDetFilm2 = null;
        [SkinControlAttribute((int)Controls.CTRL_logos_id2001)]
        protected GUIImage ImgID2001 = null;
        [SkinControlAttribute((int)Controls.CTRL_logos_id2002)]
        protected GUIImage ImgID2002 = null;
        [SkinControlAttribute((int)Controls.CTRL_ImgDD)]
        protected GUIImage ImgDD = null;
        [SkinControlAttribute(2080)]
        protected GUIAnimation m_SearchAnimation = null;
        [SkinControlAttribute((int)Controls.CTRL_Fanart)]
        protected GUIImage ImgFanart = null;
        [SkinControlAttribute((int)Controls.CTRL_FanartDir)]
        protected GUIMultiImage ImgFanartDir = null;
        //[SkinControlAttribute((int)Controls.CTRL_MovieThumbs)]
        //protected GUIImage ImgMovieThumbs = null;
        //[SkinControlAttribute((int)Controls.CTRL_MovieThumbsDir)]
        //protected GUIMultiImage ImgMovieThumbsDir = null;
        [SkinControlAttribute(1030)]
        protected GUILabelControl TxtLabel1 = null;
        [SkinControlAttribute(1031)]
        protected GUIFadeLabel TxtItem1 = null;
        [SkinControlAttribute(1032)]
        protected GUILabelControl TxtLabel2 = null;
        [SkinControlAttribute(1033)]
        protected GUIFadeLabel TxtItem2 = null;
        [SkinControlAttribute(1034)]
        protected GUIFadeLabel TxtItem3 = null;
        [SkinControlAttribute((int)Controls.CTRL_ActorMultiThumb)]
        protected GUIMultiImage ActorMultiThumb = null;

        static string wzone = null;
        int StrMax = 0;
        public const int ID_MesFilms = 7986;
        public int ID_MesFilmsDetail = 7987;
        public int ID_MesFilmsActors = 7989;
        public int ID_MesFilmsThumbs = 7990;
        public int ID_MesFilmsActorsInfo = 7991;
        public SQLiteClient m_db;
        public class IMDBActorMovie
        {
            public string MovieTitle;
            public string Role;
            public int Year;
        };
        static PlayListPlayer playlistPlayer;
        static VirtualDirectory m_directory = new VirtualDirectory();
        System.ComponentModel.BackgroundWorker bgPicture = new System.ComponentModel.BackgroundWorker();

        static System.Windows.Forms.OpenFileDialog openFileDialog1 = new OpenFileDialog();
        static string _virtualStartDirectory = String.Empty;
        static VirtualDirectory virtualDirectory = new VirtualDirectory();
        static bool m_askBeforePlayingDVDImage = false;
        public static ArrayList result;
        public static string wsearchfile;
        public static int wGetID;
        static MesFilmsDetail()
        {
            playlistPlayer = PlayListPlayer.SingletonPlayer;
        }

        #endregion

        public MesFilmsDetail()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public override int GetID
        {
            get { return ID_MesFilmsDetail; }
            set { base.GetID = value; }
        }
        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\MesFilmsDetail.xml");
        }

        #region Action
        //---------------------------------------------------------------------------------------
        //   Handle Keyboard Actions
        //---------------------------------------------------------------------------------------
        public override void OnAction(MediaPortal.GUI.Library.Action actionType)
        {
            Log.Debug("MyFilmsDetail: OnAction " + actionType.wID.ToString());
            if ((actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) || (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PARENT_DIR))
            {
                MesFilms.conf.LastID = MesFilms.ID_MesFilms;
                GUIWindowManager.ActivateWindow(ID_MesFilms);
                return;
            }

            if (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU)
            {
                Log.Debug("MyFilmsDetail : ACTION_CONTEXT_MENU erkannt !!! ");
                // context menu for options  like PlayTrailers or Updates
                //MesFilms.Context_Menu_Movie();
                if (BtnMaj.Focus)
                {
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay);
                    Update_XML_Items();
                }
                else
                {
                    GUIControl.FocusControl(GetID, (int)Controls.CTRL_BtnPlay);
                    Update_XML_Items();
                }
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
                    if (ImgDetFilm != null)
                        if (ImgDetFilm.IsVisible)
                            ImgDetFilm.Refresh();
                        else if (ImgDetFilm2!= null)
                            ImgDetFilm2.Refresh();
                    base.OnMessage(messageType);

                    wGetID = GetID;
                    GUIControl.ShowControl(GetID, 35);
                    // ToDo: Should be unhidden, if ActorThumbs are implemented
                    GUIControl.HideControl(GetID, (int)Controls.CTRL_ActorMultiThumb);
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    g_Player.PlayBackStarted += new g_Player.StartedHandler(OnPlayBackStarted);
                    g_Player.PlayBackEnded += new g_Player.EndedHandler(OnPlayBackEnded);
                    g_Player.PlayBackStopped += new g_Player.StoppedHandler(OnPlayBackStopped);
                    m_directory.SetExtensions(Utils.VideoExtensions);
                    if (MesFilms.conf.StrTxtSelect.Length == 0)
                        GUIControl.HideControl(GetID, (int)Controls.CTRL_TxtSelect);
                    else
                    {
                        GUIPropertyManager.SetProperty("#myfilms.select", MesFilms.conf.StrTxtSelect.Replace(MesFilms.conf.TitleDelim, @"\"));
                        GUIControl.ShowControl(GetID, (int)Controls.CTRL_TxtSelect);
                    }
                    afficher_init(MesFilms.conf.StrIndex); //Populate DataSet & Convert ItemId passed in initially to Index within DataSet
                    int TitlePos = (MesFilms.conf.StrTitleSelect.Length > 0) ? MesFilms.conf.StrTitleSelect.Length + 1 : 0; //only display rest of title after selected part common to group
                    if (TxtItem1 != null)
                        TxtItem1.Label =  " ";
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
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    afficher_detail(true);
                    MesFilms.conf.LastID = MesFilms.ID_MesFilmsDetail;
                    return true;                

                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT: //called when exiting plugin either by prev menu or pressing home button
                    if (Configuration.CurrentConfig != "")
                        Configuration.SaveConfiguration(Configuration.CurrentConfig, MesFilms.conf.StrIndex, MesFilms.conf.StrTIndex);
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
                        Log.Debug("MyFilms : SaveLastActiveModule - module {0}", currentmoduleid);
                        Log.Debug("MyFilms : SaveLastActiveModule - fullscreen {0}", currentmodulefullscreen);
                    }
                    return true;

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
                        MesFilms.conf.LastID = MesFilms.ID_MesFilms;
                        GUITextureManager.CleanupThumbs();
                        GUIWindowManager.ActivateWindow(ID_MesFilms);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay)
                    // Search File to play
                    {
                        Launch_Movie(MesFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay1Description)
                    // Search File to play
                    {
                        Launch_Movie(MesFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay2Comment)
                    // Search File to play
                    {
                        Launch_Movie(MesFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay3Persons)
                    // Search File to play
                    {
                        Launch_Movie(MesFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay4TecDetails)
                    // Search File to play
                    {
                        Launch_Movie(MesFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay5)
                    // Search File to play
                    {
                        Launch_Movie(MesFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnNext)
                    // Display Next Film (If last do nothing)
                    {
                        if (MesFilms.conf.StrIndex == StrMax - 1)
                            return true;
                        MesFilms.conf.StrIndex = MesFilms.conf.StrIndex + 1;
                        GUITextureManager.CleanupThumbs();
                        afficher_detail(true);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPrior)
                    // Display Prior Film (If first do nothing)
                    {
                        if (MesFilms.conf.StrIndex == 0)
                            return true;
                        MesFilms.conf.StrIndex = MesFilms.conf.StrIndex - 1;
                        GUITextureManager.CleanupThumbs();
                        afficher_detail(true);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnLast)
                    // Display Next Film (If last do nothing)
                    {
                        if (MesFilms.conf.StrIndex == StrMax - 1)
                            return true;
                        MesFilms.conf.StrIndex = StrMax - 1;
                        GUITextureManager.CleanupThumbs();
                        afficher_detail(true);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnFirst)
                    // Display Next Film (If First do nothing)
                    {
                        if (MesFilms.conf.StrIndex == 0)
                            return true;
                        MesFilms.conf.StrIndex = 0;
                        GUITextureManager.CleanupThumbs();
                        afficher_detail(true);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnMaj)
                    {    // Update items
                        Update_XML_Items();
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlayTrailer)
                    // Search Trailer File to play
                    {
                        Launch_Movie_Trailer(MesFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_ViewFanart)
                    // On Button goto MesFilmsThumbs
                    {
                        GUIWindowManager.ActivateWindow(ID_MesFilmsThumbs);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnMovieThumbs)
                    {
                        GUIWindowManager.ActivateWindow(ID_MesFilmsThumbs);
                        return true;
                    }

                    if (iControl == (int)Controls.CTRL_BtnActors)
                    {
                        GUIWindowManager.ActivateWindow(ID_MesFilmsActors);
                        return true;
                    }

                    if (iControl == (int)Controls.CTRL_BtnActorThumbs)
                    
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
        
        //--------------------------------------------------------------------------------------------
        //  Update specifics Infos
        //--------------------------------------------------------------------------------------------
        private void Update_XML_Items()
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return;
            string StrUpdItem1 = null;
            string StrUpdText1 = null;
            string StrUpdDflT1 = null;
            string StrUpdItem2 = null;
            string StrUpdText2 = null;
            string StrUpdDflT2 = null;
            int ItemID;

            dlg.Reset();
            XmlConfig XmlConfig = new XmlConfig();    
            StrUpdItem1 = XmlConfig.ReadXmlConfig("MyFilms", Configuration.CurrentConfig, "AntUpdItem1", "");
            StrUpdText1 = XmlConfig.ReadXmlConfig("MyFilms", Configuration.CurrentConfig, "AntUpdText1", "");
            StrUpdDflT1 = XmlConfig.ReadXmlConfig("MyFilms", Configuration.CurrentConfig, "AntUpdDflT1", "");
            StrUpdItem2 = XmlConfig.ReadXmlConfig("MyFilms", Configuration.CurrentConfig, "AntUpdItem2", "");
            StrUpdText2 = XmlConfig.ReadXmlConfig("MyFilms", Configuration.CurrentConfig, "AntUpdText2", "");
            StrUpdDflT2 = XmlConfig.ReadXmlConfig("MyFilms", Configuration.CurrentConfig, "AntUpdDflT2", "");

            string[] upd_choice = new string[20];
            int ichoice = 0;
            dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
            dlg.Add(GUILocalizeStrings.Get(931));//rating
            upd_choice[ichoice] = "rating";
            ichoice++;
            if (!(MesFilms.conf.StrStorage.Length == 0) && !(MesFilms.conf.StrStorage == "(none)") && (MesFilms.conf.WindowsFileDialog))
            {
                dlg.Add(GUILocalizeStrings.Get(863));//file
                upd_choice[ichoice] = "file";
                ichoice++;
            }
            if (!(StrUpdItem1 == "(none)"))
            {
                upd_choice[ichoice] = "item1";
                ichoice++;
                if (StrUpdText1.Length > 0)
                    dlg.Add(StrUpdText1);        //Specific Item1 label to update
                else
                    dlg.Add(StrUpdItem1);        //Specific Item1 to update
            }
            if (!(StrUpdItem2 == "(none)"))
            {
                upd_choice[ichoice] = "item2";
                ichoice++;
                if (StrUpdText2.Length > 0)
                    dlg.Add(StrUpdText2);        //Specific Item2 label to update
                else
                    dlg.Add(StrUpdItem2);        //Specific Item2 to update
            }
            if (MesFilms.conf.StrSuppress)
            {
                dlg.Add(GUILocalizeStrings.Get(432));
                upd_choice[ichoice] = "delete";
                ichoice++;
            }
            if (MesFilms.conf.StrGrabber)
            {
                dlg.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
                upd_choice[ichoice] = "grabber";
                ichoice++;
            }
            dlg.Add(GUILocalizeStrings.Get(10798719));             //Update missing Moviedetails from nfo-file - also download actor thumbs, Fanart, etc. if available
            upd_choice[ichoice] = "nfo-reader-update";
            ichoice++;

            dlg.Add(GUILocalizeStrings.Get(10798720));             //Overwrite all Moviedetails in DB-set from nfo-file - also download actor thumbs, Fanart, etc. if available
            upd_choice[ichoice] = "nfo-reader-overwrite";
            ichoice++;

//            dlg.Add(GUILocalizeStrings.Get(10798721));             //Update Moviedetails from ant.info file
//            upd_choice[ichoice] = "ant-nfo-reader";
//            ichoice++;

//            dlg.Add(GUILocalizeStrings.Get(10798722));             //Save Moviedetails to ant.info file
//            upd_choice[ichoice] = "ant-nfo-writer";
//            ichoice++;

            dlg.Add(GUILocalizeStrings.Get(10798723));             //Search local Trailer and Update DB (local)
            upd_choice[ichoice] = "trailer";
            ichoice++;

//            dlg.Add(GUILocalizeStrings.Get(10798726));             //Search local Trailer for all movies and Update DB (local) -> Moved to Main Screen !!!
//            upd_choice[ichoice] = "trailer-all";
//            ichoice++;

            //            dlg.Add(GUILocalizeStrings.Get(10798724));             //load IMDB Trailer, store locally and update DB
//            upd_choice[ichoice] = "trailer-imdb";
//            ichoice++;

            dlg.Add(GUILocalizeStrings.Get(10798728));             //Create Thumb from movie - if no cover available, e.g. with documentaries
            upd_choice[ichoice] = "cover-thumbnailer";
            ichoice++;

            dlg.Add(GUILocalizeStrings.Get(10798725));             //delete Trailer entries from DB record
            upd_choice[ichoice] = "trailer-delete";
            ichoice++;

            if (MesFilms.conf.StrFanart)            // Download Fanart
            {
                dlg.Add(GUILocalizeStrings.Get(1079862));
                upd_choice[ichoice] = "fanart";
                ichoice++;
            }
            if (MesFilms.conf.StrFanart)            // Remove Fanart
            {
                dlg.Add(GUILocalizeStrings.Get(1079874));
                upd_choice[ichoice] = "deletefanart";
                ichoice++;
            }
            if (MesFilms.conf.StrUpdList[0].Length > 0)
            {
                dlg.Add(GUILocalizeStrings.Get(10798642));  // Update by Property (choosen within the UPdate List Property
                upd_choice[ichoice] = "updproperty";
                ichoice++;
            }
            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1) return;
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            if (null == keyboard) return;
            keyboard.Reset();

            AntMovieCatalog ds = new AntMovieCatalog();
            ItemID = (int)MesFilms.r[MesFilms.conf.StrIndex]["Number"]; //set unique id num (ant allows it to be non-unique but that is a bad idea)
            //May wish to completely re-load the dataset before updating any fields if used in multi-user system, but would req concurrency locks etc so...
            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            GUIDialogOK dlg1 = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            switch (upd_choice[dlg.SelectedLabel])
            {
                case "rating":
                    MesFilmsDialogSetRating dlgRating = (MesFilmsDialogSetRating)GUIWindowManager.GetWindow(7988);
                    if (MesFilms.r[MesFilms.conf.StrIndex]["Rating"].ToString().Length > 0)
                        dlgRating.Rating = (decimal)MesFilms.r[MesFilms.conf.StrIndex]["Rating"];
                    else
                        dlgRating.Rating = 0;
                    dlgRating.SetTitle(MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString());
                    dlgRating.DoModal(GetID);
                    MesFilms.r[MesFilms.conf.StrIndex]["Rating"] = dlgRating.Rating.ToString();
                    Update_XML_database();
                    afficher_detail(true);
                    break;
                case "file":
                    string wfile = string.Empty;
                    if (MesFilms.conf.WindowsFileDialog)
                    {
                        openFileDialog1.RestoreDirectory = true;
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                            wfile = openFileDialog1.FileName;
                    }
                    if (wfile != string.Empty)
                    {
                        MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrStorage] = wfile;
                        Update_XML_database();
                        afficher_detail(true);
                    }
                    break;

                case "item1":
                    if (StrUpdDflT1.Length > 0)
                        keyboard.Text = StrUpdDflT1;
                    else
                        keyboard.Text = MesFilms.r[MesFilms.conf.StrIndex][StrUpdItem1].ToString();
                    keyboard.DoModal(GetID);
                    if (keyboard.IsConfirmed)
                    {
                        switch (ds.Movie.Columns[StrUpdItem1].DataType.Name)
                        {
                            case "Decimal":
                                try { MesFilms.r[MesFilms.conf.StrIndex][StrUpdItem1] = Convert.ToDecimal(keyboard.Text); }
                                catch
                                {
                                    dlg1.SetHeading(GUILocalizeStrings.Get(924)); // menu
                                    dlg1.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlg1.DoModal(GetID);
                                    return;
                                }
                                break;
                            case "Int32":
                                try { MesFilms.r[MesFilms.conf.StrIndex][StrUpdItem1] = Convert.ToInt32(keyboard.Text); }
                                catch
                                {
                                    dlg1.SetHeading(GUILocalizeStrings.Get(924)); // menu
                                    dlg1.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlg1.DoModal(GetID);
                                    return;
                                }
                                break;
                            default:
                                MesFilms.r[MesFilms.conf.StrIndex][StrUpdItem1] = keyboard.Text.ToString();
                                break;
                        }

                        Update_XML_database();
                        afficher_detail(true);
                    }
                    break;

                case "item2":
                    if (StrUpdDflT1.Length > 0)
                        keyboard.Text = StrUpdDflT2;
                    else
                        keyboard.Text = MesFilms.r[MesFilms.conf.StrIndex][StrUpdItem2].ToString();
                    keyboard.DoModal(GetID);
                    if (keyboard.IsConfirmed)
                    {
                        switch (ds.Movie.Columns[StrUpdItem2].DataType.Name)
                        {
                            case "Decimal":
                                try { MesFilms.r[MesFilms.conf.StrIndex][StrUpdItem2] = Convert.ToDecimal(keyboard.Text); }
                                catch
                                {
                                    dlg1.SetHeading(GUILocalizeStrings.Get(924)); // menu
                                    dlg1.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlg1.DoModal(GetID);
                                    return;
                                }
                                break;
                            case "Int32":
                                try { MesFilms.r[MesFilms.conf.StrIndex][StrUpdItem2] = Convert.ToInt32(keyboard.Text); }
                                catch
                                {
                                    dlg1.SetHeading(GUILocalizeStrings.Get(924)); // menu
                                    dlg1.SetLine(1, GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlg1.DoModal(GetID);
                                    return;
                                }
                                break;
                            default:
                                MesFilms.r[MesFilms.conf.StrIndex][StrUpdItem2] = keyboard.Text.ToString();
                                break;
                        } Update_XML_database();
                        afficher_detail(true);
                    }
                    break;

                case "delete":
                    if (MesFilms.conf.StrSuppress)
                    {
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                        dlgYesNo.SetLine(1, GUILocalizeStrings.Get(433));//confirm suppression
                        dlgYesNo.DoModal(GetID);
                        if (dlgYesNo.IsConfirmed)
                        {
                            MesFilmsDetail.Suppress_Entry((DataRow[])MesFilms.r, (int)MesFilms.conf.StrIndex);
 //                           Update_XML_database();
                            MesFilms.r = BaseMesFilms.LectureDonnées(MesFilms.conf.StrDfltSelect, MesFilms.conf.StrFilmSelect, MesFilms.conf.StrSorta, MesFilms.conf.StrSortSens);
                            afficher_detail(true);

                        }
                    }
                    break;
                case "updproperty":
                    System.Collections.Generic.List<string> choiceUpd = new System.Collections.Generic.List<string>();
                    ArrayList w_tableau = new ArrayList();
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(10798643)); // menu
                    foreach (string wupd in MesFilms.conf.StrUpdList)
                    {
                        dlg.Add(GUILocalizeStrings.Get(184) + " '" + BaseMesFilms.Translate_Column(wupd) + "'");
                        choiceUpd.Add(wupd);
                    }
                    dlg.DoModal(GetID);
                    if (dlg.SelectedLabel == -1)
                        return;
                    string wproperty = choiceUpd[dlg.SelectedLabel];
                    dlg.Reset();
                    keyboard.Reset();
                    keyboard.Text = MesFilms.r[MesFilms.conf.StrIndex][wproperty].ToString();
                    keyboard.DoModal(GetID);
                    if (keyboard.IsConfirmed)
                    {
                        switch (ds.Movie.Columns[wproperty].DataType.Name)
                        {
                            case "Decimal":
                                try { MesFilms.r[MesFilms.conf.StrIndex][wproperty] = Convert.ToDecimal(keyboard.Text); }
                                catch
                                {
                                    dlg1.SetHeading(GUILocalizeStrings.Get(10798642)); // menu
                                    dlg1.SetLine(1,GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlg1.DoModal(GetID);
                                    return;
                                }
                                break;
                            case "Int32":
                                try { MesFilms.r[MesFilms.conf.StrIndex][wproperty] = Convert.ToInt32(keyboard.Text); }
                                catch
                                {
                                    dlg1.SetHeading(GUILocalizeStrings.Get(10798642)); // menu
                                    dlg1.SetLine(1,GUILocalizeStrings.Get(10798644)); // wrong input
                                    dlg1.DoModal(GetID);
                                    return;
                                }
                                break;
                            default:
                                MesFilms.r[MesFilms.conf.StrIndex][wproperty] = keyboard.Text.ToString();
                                break;
                        }
                        Update_XML_database();
                        afficher_detail(true);
                    }
                    break;

                case "grabber":
                    bool wChooseScript = MesFilms.conf.StrGrabber_ChooseScript;
                    if (!System.IO.File.Exists(MesFilms.conf.StrGrabber_cnf) && !MesFilms.conf.StrGrabber_ChooseScript)
                    {
                        dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                        dlgYesNo.SetLine(1, string.Format(GUILocalizeStrings.Get(1079875), MesFilms.conf.StrGrabber_cnf));//File doesn't exist. Do you want to choose it ?
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(1079878));//confirm suppression
                        dlgYesNo.DoModal(GetID);
                        if (dlgYesNo.IsConfirmed)
                            wChooseScript = true;
                        else
                        {
                            Log.Info("My Films : The grabber config file doesn't exists. No grab Information done !");
                            break;
                        }
                    }
                    setProcessAnimationStatus(true, m_SearchAnimation);
                    string title = string.Empty;
                    if (MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"] != null && MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                        title = MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                    if (title.IndexOf(MesFilms.conf.TitleDelim) > 0)
                        title = title.Substring(title.IndexOf(MesFilms.conf.TitleDelim) + 1);
                    grabb_Internet_Informations(title, GetID, wChooseScript, MesFilms.conf.StrGrabber_cnf);
                    afficher_detail(true);
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    break;

                case "nfo-reader-update":
                    {
                        Grab_Nfo_Details((DataRow[])MesFilms.r, (int)MesFilms.conf.StrIndex, false);
                        break;
                    }
                
                case "nfo-reader-overwrite":
                    {
                        Grab_Nfo_Details((DataRow[])MesFilms.r, (int)MesFilms.conf.StrIndex, true);
                        break;
                    }

                case "ant-nfo-reader":
                    break;

                case "ant-nfo-writer":
                    break;

                case "trailer":
                    {
                        setProcessAnimationStatus(true, m_SearchAnimation);
                        //Zuerst Pfad lesen, dann Dateien suchen, in liste packen, Auswahlmenü präsenteiren und zum Schluß Update des Records
                        // Suchen nach Files mit folgendem Kriterium:
                        // 1.) ... die den Filmnamen im Filenamen haben und im Trailerverzeichnis gefunden werden (wahrscheinlich HD, daher an 1. Stelle setzen)
                        // 2.) Im Verzeichnis des Films suchen nach Filmdateien die das Wort "Trailer" im Namen haben (Endung beliebig: avi, mov, flv, etc.)
                        // 3.) Im (Trailer)-Suchpfad nach Verzeichnissen, die nach dem Filmnamen benannt sind - dann alle Files darin registrien

                        //Log.Debug("MyFilms (SearchTrailerLocal) SelectedItemInfo from (MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString(): '" + (MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString() + "'"));
                        Log.Debug("MyFilms (SearchTrailerLocal) SelectedItemInfo from (MesFilms.r[MesFilms.conf.StrIndex]: '" + (MesFilms.r[MesFilms.conf.StrIndex].ToString() + "'"));
                        Log.Debug("MyFilms (SearchTrailerLocal) Parameter 1 - '(DataRow[])MesFilms.r': '" + (DataRow[])MesFilms.r);
                        Log.Debug("MyFilms (SearchTrailerLocal) Parameter 2 - '(int)MesFilms.conf.StrIndex': '" + (int)MesFilms.conf.StrIndex);
                        MesFilmsDetail.SearchTrailerLocal((DataRow[])MesFilms.r, (int)MesFilms.conf.StrIndex, true);
                        afficher_detail(true);
                        //GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        setProcessAnimationStatus(false, m_SearchAnimation);
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                        dlgOk.SetLine(1, MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrSTitle.ToString()].ToString());//video title
                        dlgOk.SetLine(2, result.Count.ToString() + " Trailers gefunden !");
                        dlgOk.DoModal(GetID);

                        //if (dlg == null) return;
                        //dlg.Reset();
                        //dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                        //dlg.Add(GUILocalizeStrings.Get(10798723));             //Search local Trailer and Update DB (local)
                        //upd_choice[ichoice] = "trailer";
                        //ichoice++;

                        //dlg.DoModal(GetID);
                        //if (dlg.SelectedLabel == -1) return;
                        break;
                    }
                case "trailer-all": //To Be Deleted - Moved to Main Screen !!!!
                    {
                        setProcessAnimationStatus(true, m_SearchAnimation);
                        //Log.Debug("MyFilms (SearchTrailerLocal) SelectedItemInfo from (MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString(): '" + (MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString() + "'"));
                        //Log.Debug("MyFilms (SearchTrailerLocal) SelectedItemInfo from (MesFilms.r[MesFilms.conf.StrIndex]: '" + (MesFilms.r[MesFilms.conf.StrIndex].ToString() + "'"));
                   //dsgsdgsgsg
                        Log.Debug("MyFilms (SearchTrailerLocal) SelectedItemInfo from (MesFilms.r[MesFilms.conf.StrIndex]: '" + (MesFilms.r[MesFilms.conf.StrIndex].ToString() + "'"));
                        Log.Debug("MyFilms (SearchTrailerLocal) Parameter 1 - '(DataRow[])MesFilms.r': '" + (DataRow[])MesFilms.r);
                        Log.Debug("MyFilms (SearchTrailerLocal) Parameter 2 - '(int)MesFilms.conf.StrIndex': '" + (int)MesFilms.conf.StrIndex);
                        
                        //MesFilmsDetail.SearchTrailerLocal((DataRow[])MesFilms.r, (int)MesFilms.conf.StrIndex);
                        afficher_detail(true);
                        //GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        setProcessAnimationStatus(false, m_SearchAnimation);
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                        dlgOk.SetLine(1, "");
                        dlgOk.SetLine(2, " Trailer in Datenbank aktualisiert !");
                        dlgOk.DoModal(GetID);
                        break;
                    }
                case "trailer-imdb":
                    break;

                case "cover-thumbnailer":

                    CreateThumbFromMovie();
                    
                    dlg1.SetHeading(GUILocalizeStrings.Get(107986));//my films
                    dlg1.SetLine(2, "Cover created from movie");
                    dlg1.DoModal(GetID);

                    break;

                case "trailer-delete":
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                    dlgYesNo.SetLine(1, GUILocalizeStrings.Get(433));//confirm suppression
                    dlgYesNo.DoModal(GetID);
                    if (dlgYesNo.IsConfirmed)
                    {
                        MesFilmsDetail.DeleteTrailerFromDB((DataRow[])MesFilms.r, (int)MesFilms.conf.StrIndex);
                        //MesFilms.r = BaseMesFilms.LectureDonnées(MesFilms.conf.StrDfltSelect, MesFilms.conf.StrFilmSelect, MesFilms.conf.StrSorta, MesFilms.conf.StrSortSens);
                        afficher_detail(true);
                    }
                    break;

                case "fanart":
                    setProcessAnimationStatus(true, m_SearchAnimation);
                    Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
                    string wtitle = string.Empty;
                    if (MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"] != null && MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"].ToString().Length > 0)
                        wtitle = MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"].ToString();
                    if (wtitle.IndexOf(MesFilms.conf.TitleDelim) > 0)
                        wtitle = wtitle.Substring(wtitle.IndexOf(MesFilms.conf.TitleDelim) + 1);
                    string wttitle = string.Empty;
                    if (MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"] != null && MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                        wttitle = MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                    if (wttitle.IndexOf(MesFilms.conf.TitleDelim) > 0)
                        wttitle = wttitle.Substring(wttitle.IndexOf(MesFilms.conf.TitleDelim) + 1);
                    if (MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"].ToString().Length > 0 && MesFilms.conf.StrFanart)
                    {
                        int wyear = 0;
                        try { wyear = System.Convert.ToInt16(MesFilms.r[MesFilms.conf.StrIndex]["Year"]); }
                        catch { }
                        string wdirector = string.Empty;
                        try { wdirector = (string)MesFilms.r[MesFilms.conf.StrIndex]["Director"]; }
                        catch { }
                        Log.Debug("MyFilmsDetails (fanart-menuselect) Download Fanart: originaltitle: '" + wtitle + "' - translatedtitle: '" + wttitle + "' - director: '" + wdirector + "' - year: '" + wyear.ToString() + "'");
                        Download_Backdrops_Fanart(wtitle, wttitle, wdirector.ToString(), wyear.ToString(), true, GetID);

                    }
                    afficher_detail(true);
                    setProcessAnimationStatus(false, m_SearchAnimation);
                    break;
                case "deletefanart":
                    dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                    dlgYesNo.SetLine(1, GUILocalizeStrings.Get(433));//confirm suppression
                    dlgYesNo.DoModal(GetID);
                    if (dlgYesNo.IsConfirmed)
                        Remove_Backdrops_Fanart(MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString(), false);
                    afficher_detail(true);
                    break;

            }
        }
        //-------------------------------------------------------------------------------------------
        //  Suppress an entry from the database
        //-------------------------------------------------------------------------------------------        
        public static void Suppress_Entry(DataRow[] r1, int Index)
        {

            if ((MesFilms.conf.StrSuppressType == "2") || (MesFilms.conf.StrSuppressType == "4"))
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
                            Log.Info("MyFilms : file deleted : " + t.ToString());
                        }
                        catch
                        {
                            Log.Info("MyFilms : unable to delete file : " + t.ToString());
                        }
                }
            }
            if ((MesFilms.conf.StrSuppressType == "1") || (MesFilms.conf.StrSuppressType == "2"))
            {
                string wdelTitle = MesFilms.r[Index][MesFilms.conf.StrTitle1].ToString();
                MesFilms.r[Index].Delete();
                
                Log.Info("MyFilms : Database movie deleted : " + wdelTitle);
            }
            else
            {
                MesFilms.r[Index][MesFilms.conf.StrSuppressField] = MesFilms.conf.StrSuppressValue.ToString();
                Log.Info("MyFilms : Database movie updated for deletion : " + MesFilms.r[Index][MesFilms.conf.StrTitle1]);
            }
            Update_XML_database();
        }
        //-------------------------------------------------------------------------------------------
        //  Update the XML database and refresh screen
        //-------------------------------------------------------------------------------------------        
        public static void Update_XML_database()
        {
            BaseMesFilms.SaveMesFilms();
            Log.Info("MyFilms : Movie Database updated");
        }

        //-------------------------------------------------------------------------------------------
        //  Grab URL Internet Movie Informations and update the XML database and refresh screen
        //-------------------------------------------------------------------------------------------        
        public static void grabb_Internet_Informations(string FullMovieName, int GetID, bool choosescript, string wscript)
        {
            if (choosescript)
            {
                if (MesFilms.conf.StrGrabber_Dir.Length > 0 && System.IO.Directory.Exists(MesFilms.conf.StrGrabber_Dir))
                {
                    // Grabber Directory filled, search for XML scripts files
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(645)); // menu
                    if (dlg == null) return;
                    ArrayList scriptfile = new ArrayList();
                    if (MesFilms.conf.StrGrabber_cnf.Length > 0)
                    {
                        scriptfile.Add(MesFilms.conf.StrGrabber_cnf);
                        dlg.Add(MesFilms.conf.StrGrabber_cnf.Substring(MesFilms.conf.StrGrabber_cnf.LastIndexOf("\\") + 1));
                        dlg.SelectedLabel = 0;
                    }
                    DirectoryInfo dirsInf = new DirectoryInfo(MesFilms.conf.StrGrabber_Dir);
                    FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();
                    foreach (FileSystemInfo sfi in sfiles)
                    {
                        if ((sfi.Extension.ToLower() == ".xml") && (sfi.FullName != MesFilms.conf.StrGrabber_cnf))
                        {
                            dlg.Add(sfi.Name);
                            scriptfile.Add(sfi.FullName);
                        }
                    }
                    if (scriptfile.Count > 1)
                    {
                        dlg.DoModal(GetID);
                        if (dlg.SelectedLabel > 0)
                            wscript = scriptfile[dlg.SelectedLabel].ToString();
                    }
                }
                else
                {
                    GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    dlgOk.SetHeading(GUILocalizeStrings.Get(645)); // menu
                    dlgOk.SetLine(1, string.Format(GUILocalizeStrings.Get(1079876), MesFilms.conf.StrGrabber_Dir));
                    dlgOk.SetLine(2, GUILocalizeStrings.Get(1079877));
                    dlgOk.DoModal(GetID);
                    Log.Info("My Films : The Directory grabber config files doesn't exists. Verify your Configuration !");
                    return;                    
                }
            }
            string MovieName = FullMovieName;
            string MovieHierarchy = string.Empty;
            if (MesFilms.conf.TitleDelim.Length > 0)
            {
                MovieName = FullMovieName.Substring(FullMovieName.LastIndexOf(MesFilms.conf.TitleDelim) + 1).Trim();
                MovieHierarchy = FullMovieName.Substring(0, FullMovieName.LastIndexOf(MesFilms.conf.TitleDelim) + 1).Trim();
            }
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            Grabber.Grabber_URLClass.IMDBUrl wurl;

            ArrayList listUrl = Grab.ReturnURL(MovieName, wscript, 1, !MesFilms.conf.StrGrabber_Always);
            int listCount = listUrl.Count;
            if (!MesFilms.conf.StrGrabber_Always)
                listCount = 2;
            switch (listCount)
            {
                case 1:
                    wurl = (Grabber.Grabber_URLClass.IMDBUrl)listUrl[0];
                    grabb_Internet_Details_Informations(wurl.URL, MovieHierarchy, wscript);
                    break;
                case 0:
                    break;
                default:
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                    dlg.Add("  *****  " + GUILocalizeStrings.Get(200036) + "  *****  "); //choice for changing movie filename
                    for (int i = 0; i < listUrl.Count; i++)
                    {
                        wurl = (Grabber.Grabber_URLClass.IMDBUrl)listUrl[i];
                        if (wurl.Title.Contains(MesFilms.r[MesFilms.conf.StrIndex]["Director"].ToString()) && wurl.Title.Contains(MesFilms.r[MesFilms.conf.StrIndex]["Year"].ToString()) && (!MesFilms.conf.StrGrabber_Always))
                        {
                            if (dlg.SelectedLabel == -1)
                                dlg.SelectedLabel = i + 1;
                            else
                                dlg.SelectedLabel = -2;
                        }
                        dlg.Add(wurl.Title.ToString());
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
                        if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                            grabb_Internet_Informations(keyboard.Text.ToString(), GetID, false, wscript);
                        break;
                    }
                    if (dlg.SelectedLabel > 0)
                    {
                        wurl = (Grabber.Grabber_URLClass.IMDBUrl)listUrl[dlg.SelectedLabel - 1];
                        grabb_Internet_Details_Informations(wurl.URL, MovieHierarchy,wscript);
                    }
                    break;
            }
        }
        //-------------------------------------------------------------------------------------------
        //  Grab Internet Movie Details Informations and update the XML database and refresh screen
        //-------------------------------------------------------------------------------------------        
        public static void grabb_Internet_Details_Informations(string url, string moviehead, string wscript)
        {
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            string[] Result = new string[20];
            string title = string.Empty;
            string ttitle = string.Empty;
            string wtitle = string.Empty;
            int year = 0;
            string director = string.Empty;
            XmlConfig XmlConfig = new XmlConfig();
            string Img_Path = XmlConfig.ReadAMCUXmlConfig(MesFilms.conf.StrAMCUpd_cnf, "Image_Download_Filename_Prefix", "");
            string Img_Path_Type = XmlConfig.ReadAMCUXmlConfig(MesFilms.conf.StrAMCUpd_cnf, "Store_Image_With_Relative_Path", "false");

            Result = Grab.GetDetail(url, MesFilms.conf.StrPathImg + Img_Path, wscript);
            Log.Info("MyFilms : Grabb Internet Information done for : " + ttitle);

            //            string Title_Group = XmlConfig.ReadAMCUXmlConfig(MesFilms.conf.StrAMCUpd_cnf, "Folder_Name_Is_Group_Name", "false");
            //            string Title_Group_Apply = XmlConfig.ReadAMCUXmlConfig(MesFilms.conf.StrAMCUpd_cnf, "Group_Name_Applies_To", "");
            if (Result[0] != string.Empty && Result[0] != null)
            {
                title = Result[0].ToString();
                wtitle = MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"].ToString();
                if (wtitle.Contains(MesFilms.conf.TitleDelim))
                    wtitle = wtitle.Substring(wtitle.LastIndexOf(MesFilms.conf.TitleDelim) + 1);
                if (wtitle != title)
                    Remove_Backdrops_Fanart(wtitle, true);
                if (MesFilms.conf.StrTitle1 == "OriginalTitle")
                    MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"] = moviehead + title;
                else
                    MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"] = title;
            }
            if (Result[1] != string.Empty && Result[1] != null)
            {
                ttitle = Result[1].ToString();
                wtitle = MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                if (wtitle.Contains(MesFilms.conf.TitleDelim))
                    wtitle = wtitle.Substring(wtitle.LastIndexOf(MesFilms.conf.TitleDelim) + 1);
                if (wtitle != ttitle)
                    Remove_Backdrops_Fanart(wtitle, true);
                if (MesFilms.conf.StrTitle1 == "TranslatedTitle")
                    MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"] = moviehead + ttitle;
                else
                    MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"] = ttitle;
            }
            if (Result[2] != string.Empty && Result[2] != null)
            {
                if (Img_Path_Type.ToLower() == "true")
                    Result[2] = Result[2].Substring(Result[2].LastIndexOf("\\") + 1).ToString();
                if (Img_Path.Length > 0)
                    if (Img_Path.EndsWith("\\"))
                        Result[2] = Img_Path + Result[2];
                    else
                        Result[2] = Img_Path + "\\" + Result[2];
                MesFilms.r[MesFilms.conf.StrIndex]["Picture"] = Result[2].ToString();
            }

            if (Result[3] != string.Empty && Result[3] != null)
                MesFilms.r[MesFilms.conf.StrIndex]["Description"] = Result[3].ToString();
            if (Result[4] != string.Empty && Result[4] != null)
            {
                if (Result[4].ToString().Length > 0)
                {
                    NumberFormatInfo provider = new NumberFormatInfo();
                    provider.NumberDecimalSeparator = ".";
                    provider.NumberDecimalDigits = 1;
                    decimal wnote = Convert.ToDecimal(Result[4], provider);
                    MesFilms.r[MesFilms.conf.StrIndex]["Rating"] = string.Format("{0:F1}", wnote);
                }
            }
            if (Result[5] != string.Empty && Result[5] != null)
                MesFilms.r[MesFilms.conf.StrIndex]["Actors"] = Result[5].ToString();
            if (Result[6] != string.Empty && Result[6] != null)
            {
                director = Result[6].ToString();
                MesFilms.r[MesFilms.conf.StrIndex]["Director"] = Result[6].ToString();
            }
            if (Result[7] != string.Empty && Result[7] != null)
                MesFilms.r[MesFilms.conf.StrIndex]["Producer"] = Result[7].ToString();
            if (Result[8] != string.Empty && Result[8] != null)
            {
                try
                {
                    year = Convert.ToInt16(Result[8].ToString());
                }
                catch { }
                MesFilms.r[MesFilms.conf.StrIndex]["Year"] = Result[8].ToString();
            }
            if (Result[9] != string.Empty && Result[9] != null)
                MesFilms.r[MesFilms.conf.StrIndex]["Country"] = Result[9].ToString();
            if (Result[10] != string.Empty && Result[10] != null)
                MesFilms.r[MesFilms.conf.StrIndex]["Category"] = Result[10].ToString();
            if (Result[11] != string.Empty && Result[11] != null)
                if (MesFilms.conf.StrStorage != "URL")
                    MesFilms.r[MesFilms.conf.StrIndex]["URL"] = Result[11].ToString();
            Update_XML_database();
            Log.Info("MyFilms : Database Updated for : " + ttitle);
            if (title.Length > 0 && MesFilms.conf.StrFanart)
            {
                System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, (int)year, director, MesFilms.conf.StrPathFanart, true, false, MesFilms.conf.StrTitle1.ToString());
                //System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, (int)year, director, MesFilms.conf.StrPathFanart, true, false);
            }

        }

        //-------------------------------------------------------------------------------------------
        //  Create Thumb via MTN (MovieThumbNailer) from movie itself
        //-------------------------------------------------------------------------------------------        
        public static void CreateThumbFromMovie()
        {
            //XmlConfig XmlConfig = new XmlConfig();
            //string Img_Path = XmlConfig.ReadAMCUXmlConfig(MesFilms.conf.StrAMCUpd_cnf, "Image_Download_Filename_Prefix", "");
            //string Img_Path_Type = XmlConfig.ReadAMCUXmlConfig(MesFilms.conf.StrAMCUpd_cnf, "Store_Image_With_Relative_Path", "false");

            string path = MesFilms.r[MesFilms.conf.StrIndex]["Source"].ToString();
            string strThumb = MesFilms.conf.StrPathImg + "\\" + MesFilms.r[MesFilms.conf.StrIndex]["Number"].ToString() + ".jpg";


            //if (Img_Path_Type.ToLower() == "true")
            //    MesFilms.r[MesFilms.conf.StrIndex]["Picture"] = strThumb;

            MesFilms.r[MesFilms.conf.StrIndex]["Picture"] = strThumb;
            
            
            if (File.Exists(strThumb))
            {
                return ;
            }

            // Do not try to create thumbnails for DVDs
            if (path.Contains("VIDEO_TS\\VIDEO_TS.IFO"))
            {
                return ;
            }

            MediaPortal.Services.IVideoThumbBlacklist blacklist = MediaPortal.Services.GlobalServiceProvider.Get<MediaPortal.Services.IVideoThumbBlacklist>();
            if (blacklist != null && blacklist.Contains(path))
            {
                Log.Debug("Skipped creating thumbnail for {0}, it has been blacklisted because last attempt failed", path);
                return ;
            }


            System.Drawing.Image thumb = null;
            try
            {
                // CreateVideoThumb(string aVideoPath, string aThumbPath, bool aCacheThumb, bool aOmitCredits);
                bool success = VideoThumbCreator.CreateVideoThumb(path, strThumb, true, false);
                if (!success)
                    return;
                else
                    return;
            }
            catch (System.Runtime.InteropServices.COMException comex)
            {
                if (comex.ErrorCode == unchecked((int)0x8004B200))
                {
                    Log.Warn("Could not create thumbnail for {0} [Unknown error 0x8004B200]", path);
                }
                else
                {
                    Log.Error("Could not create thumbnail for {0}", path);
                    Log.Error(comex);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Could not create thumbnail for {0}", path);
                Log.Error(ex);
            }
            finally
            {
                if (thumb != null)
                    thumb.Dispose();
                if (!File.Exists(strThumb) && blacklist != null)
                {
                    blacklist.Add(path);
                }
            }

            Update_XML_database();
            Log.Info("MyFilms : Database Updated for created PictureThumb: " + strThumb);
        }

        //-------------------------------------------------------------------------------------------
        //  Grab XBMC (movie.nfo) kompatible Movie Details Informations and update the XML database and refresh screen
        // Last Parameter is set to overwrite all existing data - when set to false it only updates missing infos (important for batch import)
        //-------------------------------------------------------------------------------------------        
        public static void Grab_Nfo_Details(DataRow[] r1, int Index, bool overwrite)
        //public static void grabb_Nfo_Details(string url, string moviehead, string wscript)
        //    		private void GetMovieNfoInfo(string file, ref IMDBMovie details)  // Technick 04-2010

        {

            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass(); 
            string[] Result = new string[20]; // Array für die nfo-grabberresults - analog dem internetgrabber
            string[] ResultName = new string[20];
            string[] ActorsName = new string[100]; //(Actors Name)
            string[] ActorsRole = new string[100]; //(Actors Role)
            string[] ActorsThumb = new string[100]; //(Actors Thumblink)
            string ActorName = "";
            string ActorRole = "";
            string ActorThumb = "";
            string titlename = MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString();
            string titlename2 = MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle2].ToString();
            string directoryname = "";
            string movieName = "";
            string nfofile = "";
            int Actornumber = 0;
            
            //Retrieve original directory of mediafiles
            //directoryname
            movieName = (string)MesFilms.r[Index][MesFilms.conf.StrStorage].ToString().Trim();
            movieName = movieName.Substring(movieName.LastIndexOf(";") + 1);
            Log.Debug("MyFilmsDetails (grabb_Nfo_Details) Splittet Mediadirectoryname: '" + movieName.ToString() + "'");
            try
            { directoryname = System.IO.Path.GetDirectoryName(movieName); }
            catch
            { directoryname = ""; }
            Log.Debug("MyFilmsDetails (grabb_Nfo_Details) Get Mediadirectoryname: '" + directoryname.ToString() + "'");

            nfofile = directoryname + "\\movie.nfo";
            Log.Debug("MyFilmsDetails (grabb_Nfo_Details) nfo-Filename: '" + nfofile.ToString() + "'");

            if (!System.IO.File.Exists(nfofile))
                {
                Log.Debug("MyFilmsDetails (grabb_Nfo_Details) File cannot be opened, nfo-Filename: '" + nfofile.ToString() + "'");
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                dlgOk.SetLine(1, " ");//video title
                dlgOk.SetLine(2, "Cannot find file: '" + nfofile.ToString() + "'");
                dlgOk.DoModal(GUIWindowManager.ActiveWindow);
                return;
                }

            for (int i = 0; i < 20; ++i)
            {
                Result[i] = "";
                ResultName[i] = "(none)";
            }
            
            //Temporary Methods to check XML File

            XmlTextReader reader = new XmlTextReader(nfofile);
            string element = "";
            string value = "";
            while (reader.Read())
            {
                XmlNodeType nodeType = reader.NodeType;
                switch (nodeType)
                    {
                    case XmlNodeType.Element:
                            element = reader.Name;
                            //Log.Debug("MyFilmsDetails XMLtextReader1 - Element name is '" + reader.Name + "'");
                        if (reader.HasAttributes)
                            {
                            Log.Debug("MyFilmsDetails XMLtextReader1 - *** " + reader.AttributeCount + " Attributes found! ***");
                            for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                reader.MoveToAttribute(i);
                                //Log.Debug("                Attribute is '" + reader.Name + "' with Value '" + reader.Value + "'");
                                }
                            }
                        break;
                    case XmlNodeType.Text:
                        value = reader.Value;
                        //Log.Debug("                Value is: " + reader.Value);
                        break;
                    }
                if (element.Length > 0 && value.Length > 0)
                {
                    Log.Debug("MyFilmsDetail (XML-Readertest) Attribute is '" + element + "' with Value '" + value + "'");
                    if (element == "title")
                    {
                        Result[1] = value;
                        ResultName[1] = "translatedtitle";
                    }
                    if (element == "originaltitle")
                    {
                        Result[0] = value;
                        ResultName[0] = "originaltitle";
                    }
                    if (element == "id")
                    {
                        Result[11] = value;
                        ResultName[11] = "url";
                    }
                    if (element == "year")
                    {
                        Result[8] = value.ToString();
                        ResultName[8] = "year";
                    }
                    if (element == "releasedate")
                    {
                        Result[13] = value;
                        ResultName[13] = "date";
                    }
                    if (element == "rating")
                    {
                        Result[4] = value;
                        ResultName[4] = "rating";
                    }
                    if (element == "votes")
                    {
                        Result[14] = value;
                        ResultName[14] = "votes";
                    }
                    if (element == "mpaa")
                    {
                        Result[15] = value;
                        ResultName[15] = "mpaa";
                    }
                    if (element == "certification")
                    {
                        Result[16] = value;
                        ResultName[16] = "certification";
                    }
                    if (element == "genre")
                    {
                        Result[10] = value;
                        ResultName[10] = "category";
                    }
                    if (element == "studio")
                    {
                        Result[17] = value;
                        ResultName[17] = "studio";
                    }
                    if (element == "director")
                    {
                        Result[6] = value;
                        ResultName[6] = "director";
                    }
                    if (element == "credits")
                    {
                        Result[7] = value;
                        ResultName[7] = "producer (credits)";
                    }
                    if (element == "tagline")
                    {
                        Result[18] = value;
                        ResultName[18] = "tagline";
                    }
                    if (element == "outline")
                    {
                        Result[19] = value;
                        ResultName[19] = "outline";
                    }
                    if (element == "plot")
                    {
                        Result[3] = value;
                        ResultName[3] = "description";
                    }
                    if (element == "name") // actor infos
                    {
                        if (ActorName.Length == 0) 
                            ActorName = value;
                        else
                        {
                            ActorsName[Actornumber] = ActorName;
                            ActorsRole[Actornumber] = ActorRole;
                            ActorsThumb[Actornumber] = ActorThumb;
                            Actornumber = Actornumber + 1;
                            ActorName = "";
                            ActorRole = "";
                            ActorThumb = "";
                        }
                    }
                    if (element == "role") // actor infos
                    {
                        ActorRole = value;
                    }
                    if (element == "thumb") // actor infos
                    {
                        if (ActorName.Length > 0)
                        {
                            ActorThumb = value;
                            ActorsName[Actornumber] = ActorName;
                            ActorsRole[Actornumber] = ActorRole;
                            ActorsThumb[Actornumber] = ActorThumb;
                            Actornumber = Actornumber + 1;
                            ActorName = "";
                            ActorRole = "";
                            ActorThumb = "";
                        }
                    }

                    element = "";
                    value = "";
                }
            //Log.Debug("MyFilmsDetail (XML-Readertest) Attribute is '" + element + "' with Value '" + value + "'");
            }
            
            

            Log.Debug("MyFilmsDetails (grabb_Nfo_Details) Actors found: '" + (Actornumber).ToString() + "'");
            ResultName[5] = "actors";
            if (Actornumber > 1)
                Result[5] = ActorsName[0] + " (als " + ActorsRole[0] + ")"; 
            for (int wi = 1; wi < Actornumber - 1; ++wi)
            {
                Result[5] = Result[5] + ", " + ActorsName[wi] + " (als " + ActorsRole[wi] + ")"; 
                Log.Debug("MyFilmsDetails (grabb_Nfo_Details) Actors: '" + ActorsName[wi] + "' (als " + ActorsRole[wi] + ") - Thumb = '" + ActorsThumb[wi] + "'");
            }


            for (int i = 0; i < 20; ++i)
            {
                Log.Debug("MyFilmsDetails (grabb_Nfo_Details) Summary (" + i.ToString() + ", " + Result[i].Length.ToString() + "): " + ResultName[i] + " = '" + Result[i] + "'");
            }


			//char[] charsToTrim = { '|' };
			//string str1 = details.Genre.TrimEnd(charsToTrim);
			//string str2 = str1.TrimStart(charsToTrim);
			//details.Genre = str2.Replace("|", ", ");
			//str1 = //details.Director.TrimEnd(charsToTrim);
			//str2 = str1.TrimStart(charsToTrim);
			//details.Director = str2.Replace("|", ", ");
            for (int i = 0; i < 20; ++i)
            {
                Log.Debug("MyFilmsDetails (grabb_Nfo_Details) Summary (" + i.ToString() + ", " + Result[i].Length.ToString() + "): " + ResultName[i] + " = '" + Result[i] + "'");
            }

// Old Code            
            //string url = "";
            string moviehead = "";
            //string wscript = "";

            //Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            string title = string.Empty;
            string ttitle = string.Empty;
            string wtitle = string.Empty;
            int year = 0;
            string director = string.Empty;
            XmlConfig XmlConfig = new XmlConfig();
            string Img_Path = XmlConfig.ReadAMCUXmlConfig(MesFilms.conf.StrAMCUpd_cnf, "Image_Download_Filename_Prefix", "");
            string Img_Path_Type = XmlConfig.ReadAMCUXmlConfig(MesFilms.conf.StrAMCUpd_cnf, "Store_Image_With_Relative_Path", "false");

            //Result = Grab.GetDetail(url, MesFilms.conf.StrPathImg + Img_Path, wscript);
            //Log.Info("MyFilms : Grabb Internet Information done for : " + ttitle);

            //            string Title_Group = XmlConfig.ReadAMCUXmlConfig(MesFilms.conf.StrAMCUpd_cnf, "Folder_Name_Is_Group_Name", "false");
            //            string Title_Group_Apply = XmlConfig.ReadAMCUXmlConfig(MesFilms.conf.StrAMCUpd_cnf, "Group_Name_Applies_To", "");
            if (Result[0] != string.Empty && Result[0] != null)
            {
                title = Result[0].ToString();
                wtitle = MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"].ToString();
                if (wtitle.Contains(MesFilms.conf.TitleDelim))
                    wtitle = wtitle.Substring(wtitle.LastIndexOf(MesFilms.conf.TitleDelim) + 1);
                if (wtitle != title)
                    Remove_Backdrops_Fanart(wtitle, true);
                if (MesFilms.conf.StrTitle1 == "OriginalTitle")
                    MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"] = moviehead + title;
                else
                    MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"] = title;
            }
            if (Result[1] != string.Empty && Result[1] != null)
            {
                ttitle = Result[1].ToString();
                wtitle = MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString();
                if (wtitle.Contains(MesFilms.conf.TitleDelim))
                    wtitle = wtitle.Substring(wtitle.LastIndexOf(MesFilms.conf.TitleDelim) + 1);
                if (wtitle != ttitle)
                    Remove_Backdrops_Fanart(wtitle, true);
                if (MesFilms.conf.StrTitle1 == "TranslatedTitle")
                    MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"] = moviehead + ttitle;
                else
                    MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"] = ttitle;
            }
            if (Result[2] != string.Empty && Result[2] != null)
            {
                if (Img_Path_Type.ToLower() == "true")
                    Result[2] = Result[2].Substring(Result[2].LastIndexOf("\\") + 1).ToString();
                if (Img_Path.Length > 0)
                    if (Img_Path.EndsWith("\\"))
                        Result[2] = Img_Path + Result[2];
                    else
                        Result[2] = Img_Path + "\\" + Result[2];
                MesFilms.r[MesFilms.conf.StrIndex]["Picture"] = Result[2].ToString();
            }

            if (Result[3] != string.Empty && Result[3] != null)
                MesFilms.r[MesFilms.conf.StrIndex]["Description"] = Result[3].ToString();
            if (Result[4] != string.Empty && Result[4] != null)
            {
                if (Result[4].ToString().Length > 0)
                {
                    NumberFormatInfo provider = new NumberFormatInfo();
                    provider.NumberDecimalSeparator = ".";
                    provider.NumberDecimalDigits = 1;
                    decimal wnote = Convert.ToDecimal(Result[4], provider);
                    MesFilms.r[MesFilms.conf.StrIndex]["Rating"] = string.Format("{0:F1}", wnote);
                }
            }
            if (Result[5] != string.Empty && Result[5] != null)
                MesFilms.r[MesFilms.conf.StrIndex]["Actors"] = Result[5].ToString();
            if (Result[6] != string.Empty && Result[6] != null)
            {
                director = Result[6].ToString();
                MesFilms.r[MesFilms.conf.StrIndex]["Director"] = Result[6].ToString();
            }
            if (Result[7] != string.Empty && Result[7] != null)
                MesFilms.r[MesFilms.conf.StrIndex]["Producer"] = Result[7].ToString();
            if (Result[8] != string.Empty && Result[8] != null)
            {
                try
                {
                    year = Convert.ToInt16(Result[8].ToString());
                }
                catch { }
                MesFilms.r[MesFilms.conf.StrIndex]["Year"] = Result[8].ToString();
            }
            if (Result[9] != string.Empty && Result[9] != null)
                MesFilms.r[MesFilms.conf.StrIndex]["Country"] = Result[9].ToString();
            if (Result[10] != string.Empty && Result[10] != null)
                MesFilms.r[MesFilms.conf.StrIndex]["Category"] = Result[10].ToString();
            if (Result[11] != string.Empty && Result[11] != null)
                if (MesFilms.conf.StrStorage != "URL")
                    MesFilms.r[MesFilms.conf.StrIndex]["URL"] = Result[11].ToString();
            //Update_XML_database();
            Log.Info("MyFilms : (Inactive) Database Updated for : " + ttitle);
            if (title.Length > 0 && MesFilms.conf.StrFanart)
            {
                System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, (int)year, director, MesFilms.conf.StrPathFanart, true, false, MesFilms.conf.StrTitle1.ToString());
                //System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, (int)year, director, MesFilms.conf.StrPathFanart, true, false);
            }

        }
		
        //-------------------------------------------------------------------------------------------
        //  Remove Old backdrops Fanart already downloaded (case in title change)
        //-------------------------------------------------------------------------------------------        
        public static void Remove_Backdrops_Fanart(string wtitle,bool suppressDir)
        {
            if (wtitle.Length > 0)
            {
                if (wtitle.Contains(MesFilms.conf.TitleDelim))
                    wtitle = wtitle.Substring(wtitle.LastIndexOf(MesFilms.conf.TitleDelim) + 1).Trim();
                wtitle = Grabber.GrabUtil.CreateFilename(wtitle.ToLower()).Replace(' ', '.');
                if (suppressDir)
                {
                    try { System.IO.Directory.Delete(MesFilms.conf.StrPathFanart + "\\{" + wtitle + "}"); }
                    catch { }
                }
                else
                {
                    DirectoryInfo dirsInf = new DirectoryInfo(MesFilms.conf.StrPathFanart + "\\{" + wtitle + "}");
                    FileSystemInfo[] sfiles = dirsInf.GetFileSystemInfos();
                    foreach (FileSystemInfo sfi in sfiles)
                    {
                        System.IO.File.Delete(sfi.FullName);
                    }   
                }
            }
        }
        //-------------------------------------------------------------------------------------------
        //  Dowload backdrops on theMovieDB.org
        //-------------------------------------------------------------------------------------------        
        public static void Download_Backdrops_Fanart(string wtitle, string wttitle, string director, string year, bool choose,int wGetID)
        {
            Grabber.Grabber_URLClass Grab = new Grabber.Grabber_URLClass();
            int wyear = 0;
            try {  wyear = Convert.ToInt32(year);}
            catch { }
            System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, director, MesFilms.conf.StrPathFanart, true, choose, MesFilms.conf.StrTitle1);
            //System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, director, MesFilms.conf.StrPathFanart, true, choose);
            Log.Debug("MyFilms (DownloadBackdrops) - listemovies: '" + wtitle + "', '" + wttitle + "', '" + wyear + "', '" + director + "', '" + MesFilms.conf.StrPathFanart + "', 'true', '" + choose.ToString() + "', '" + MesFilms.conf.StrTitle1 + "'");
            int listCount = listemovies.Count;
            Log.Debug("MyFilms (DownloadBackdrops) - listemovies: Result Listcount: '" + listCount.ToString() + "'");
            
            //if ((listCount == 0) && (choose))
            //{
            //    //MesFilmsDetail.ShowMessageDialog("", " No results found", "");
            //    GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            //    if (dlgOK != null)
            //    {
            //        dlgOK.SetHeading("");
            //        dlgOK.SetLine(1, "No results found !");
            //        dlgOK.SetLine(2, "");
            //        dlgOK.DoModal(wGetID);
            //    }
            //}

            if (choose)
                listCount = 2;
            switch (listCount)
            {
                case 0:
                    break;
                case 1:
                    Log.Info("MyFilms : Fanart " + listemovies[0].Name.Substring(listemovies[0].Name.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);
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
                    dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                    dlg.Add("  *****  " + GUILocalizeStrings.Get(200036) + "  *****  "); //choice for changing movie filename
                    foreach (DBMovieInfo t in listemovies)
                    {
                        dlg.Add(t.Name + "  (" + t.Year + ") ");
                    }
                    if (!(dlg.SelectedLabel > -1))
                    {
                        dlg.SelectedLabel = -1;
                        dlg.DoModal(wGetID);
                    }
                    if (dlg.SelectedLabel == 0)
                    {
                        //Get SubTitles and Subwords from otitle and ttitle
                        wotitle_tableau = MesFilms.SubTitleGrabbing(wtitle.ToString());
                        wttitle_tableau = MesFilms.SubTitleGrabbing(wttitle.ToString());
                        wotitle_sub_tableau = MesFilms.SubWordGrabbing(wtitle.ToString(),MinChars,Filter); // Min 3 Chars, Filter true (no der die das)
                        wttitle_sub_tableau = MesFilms.SubWordGrabbing(wttitle.ToString(),MinChars,Filter); // Min 3 Chars, Filter true (no der die das)
                        //First Show Dialog to choose Otitle, Ttitle or substrings - or Keyboard to manually enter searchstring!!!
                        GUIDialogMenu dlgs = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        if (dlgs == null) return;
                        dlgs.Reset();
                        dlgs.SetHeading(GUILocalizeStrings.Get(924) + " - Wähle Suchbegriff"); // menu
                        dlgs.Add("  *****  " + "Manuelle Eingabe über Tastatur" + "  *****  ");
                        dlgs.Add(wtitle); //Otitle
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
                        //dlgs.Add("  *****  " + GUILocalizeStrings.Get(200036) + "  *****  "); //choice for changing movie filename
                        if (!(dlgs.SelectedLabel > -1))
                        {
                            dlgs.SelectedLabel = -1;
                            dlgs.DoModal(wGetID);
                        }
                        //Log.Debug("MyFilms (SingleFanartGrabber) - Info about Selected DIalog Searchstring: DialofSelectedLabelText: '" + dlgs.SelectedLabelText.ToString() + "'");
                        //Log.Debug("MyFilms (SingleFanartGrabber) - Info about Selected DIalog Searchstring: DialofSelectedLabel: '" + dlgs.SelectedLabel.ToString() + "'");
                        if (dlgs.SelectedLabel == 0)
                        {
                            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                            if (null == keyboard) return;
                            keyboard.Reset();
                            keyboard.Text = wtitle;
                            keyboard.DoModal(wGetID);
                            if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                            {
                                Remove_Backdrops_Fanart(wttitle, true);
                                Remove_Backdrops_Fanart(wtitle, true);
                                Download_Backdrops_Fanart(keyboard.Text, wttitle, string.Empty, string.Empty, true, wGetID);
                            }
                            break;
                        }
                        if (dlgs.SelectedLabel > 0)
                        {
                            Download_Backdrops_Fanart(dlgs.SelectedLabelText, wttitle, string.Empty, string.Empty, true, wGetID);
                            //Download_Backdrops_Fanart(string wtitle, string wttitle, string director, string year, bool choose,int wGetID)
                            break;
                        }
                    }
                    if (dlg.SelectedLabel > 0)
                    {
                        bool first = true;
                        string filename = string.Empty;
                        string filename1 = string.Empty;
                        if (MesFilms.conf.StrTitle1 == "OriginalTitle")
                            wttitle = wtitle;
                        foreach (string backdrop in listemovies[dlg.SelectedLabel - 1].Backdrops)
                        {
                            filename1 = Grabber.GrabUtil.DownloadBacdropArt(MesFilms.conf.StrPathFanart, backdrop, wttitle, true, first, out filename);
                            Log.Info("MyFilms : Fanart " + filename1.Substring(filename1.LastIndexOf("\\") + 1) + " downloaded for " + wttitle);

                            if (filename == string.Empty)
                                filename = filename1;
                            if (!(filename == "already" && filename1 == "already"))
                                filename = "added";
                            first = false;
                        }
                        listemovies[0].Name = filename;
                    }
                    break;
            }
        }
        //-------------------------------------------------------------------------------------------
        //  Search Fanart Thumbs 
        //                          wtitle2 = Translated Title if any or Original Title
        //                          main = true for main screen and false for detailed
        //                          searched = dir for Directory searched (detail screen and control multiImage)or file 
        //                          rep = true if the selected Item is a grouped Item (country, genre, year...) on main screen
        //                          filecover = name of the file cover for using 'Default Cover for missing Fanart'
        //      value returned string[2]
        //                          [0] = file or directory found (if not " ")
        //                          [1] = file or dir
        //-------------------------------------------------------------------------------------------

        public static string[] Search_Fanart(string wtitle2, bool main, string searched, bool rep, string filecover, string group)
        //                     Search_Fanart(wlabel, true, "file", false, facadeView.SelectedListItem.ThumbnailImage.ToString(), string.Empty);
        {
            //Log.Debug("MyFilms (SearchFanart) - Vars: wtitle2 = '" + wtitle2 + "'");
            //Log.Debug("MyFilms (SearchFanart) - Vars: main (true for mainscreen, false for Detail) = '" + main + "'");
            //Log.Debug("MyFilms (SearchFanart) - Vars: searched (dir or file) = '" + searched + "'");
            //Log.Debug("MyFilms (SearchFanart) - Vars: rep (true for grouped view) = '" + rep + "'");
            //Log.Debug("MyFilms (SearchFanart) - Vars: filecover = '" + filecover + "'");
            //Log.Debug("MyFilms (SearchFanart) - Vars: group = '" + group + "'");
            //Log.Debug("MyFilms (SearchFanart) - Config: MesFilms.conf.StrFanart = '" + MesFilms.conf.StrFanart + "'");
            string[] wfanart = new string[2];
            wfanart[0] = " ";
            wfanart[1] = " ";
            if (MesFilms.conf.StrFanart)
            {
                if (wtitle2.Contains(MesFilms.conf.TitleDelim))
                    wtitle2 = wtitle2.Substring(wtitle2.LastIndexOf(MesFilms.conf.TitleDelim) + 1).Trim();
                wtitle2 = Grabber.GrabUtil.CreateFilename(wtitle2.ToLower()).Replace(' ', '.');
                //Log.Debug("MyFilms (SearchFanart) - wtitle2-cleaned = '" + wtitle2 + "'");
                //Log.Debug("MyFilms (SearchFanart) - MesFilms.conf.StrFanart = '" + MesFilms.conf.StrFanart + "'");

                if (!MesFilms.conf.StrFanart)
                    return wfanart;

                string safeName = string.Empty;
                if (rep)
                {
                    if (group == "country" || group == "year" || group == "category")
                    {
                        if (!System.IO.Directory.Exists(MesFilms.conf.StrPathFanart + "\\_Group"))
                            System.IO.Directory.CreateDirectory(MesFilms.conf.StrPathFanart + "\\_Group");
                        if (!System.IO.Directory.Exists(MesFilms.conf.StrPathFanart + "\\_Group\\" + group))
                            System.IO.Directory.CreateDirectory(MesFilms.conf.StrPathFanart + "\\_Group\\" + group);
                        safeName = MesFilms.conf.StrPathFanart + "\\_Group\\" + group + "\\{" + wtitle2 + "}";
                    }
                    else
                        return wfanart;
                }
                else
                    safeName = MesFilms.conf.StrPathFanart + "\\{" + wtitle2 + "}";
                //Log.Debug("MyFilms (SearchFanart) - safename = '" + safeName + "'");
                FileInfo wfile = new FileInfo(safeName + "\\{" + wtitle2 + "}.jpg");
                //Log.Debug("MyFilms (SearchFanart) - safename(file) = '" + wfile + "'");
                Log.Debug("MyFilms (SearchFanart) - safename(file&ext) = '" + (safeName + "\\{" + wtitle2 + "}.jpg") + "'");
                if (((main) || (searched == "file")) && (System.IO.File.Exists(safeName + "\\{" + wtitle2 + "}.jpg")))
                {
                    wfanart[0] = safeName + "\\{" + wtitle2 + "}.jpg";
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
                if ((MesFilms.conf.StrFanartDflt) && !(rep))
                {
                    wfanart[0] = filecover.ToString();
                    wfanart[1] = "file";
                    //Added Guzzi - Fix that no fanart was returned ...
                    return wfanart;
                }
            }
            
            Log.Debug("MesFilm (SearchFanart) - Fanart not configured: wfanart[0,1]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
            return wfanart;
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
            StrMax = MesFilms.r.Length;
        }

        private void afficher_detail(bool searchPicture)
        {
            //-----------------------------------------------------------------------------------------------------------------------
            //    Load Detailed Info
            //-----------------------------------------------------------------------------------------------------------------------
            if (MesFilms.conf.StrIndex > MesFilms.r.Length - 1)
                MesFilms.conf.StrIndex = MesFilms.r.Length - 1;
            if (MesFilms.conf.StrIndex == -1)
            {
                MediaPortal.GUI.Library.Action actionType = new MediaPortal.GUI.Library.Action();
                actionType.wID = MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU;
                base.OnAction(actionType);
                return;
            }
            if (MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString().Length > 0)
            {
                int TitlePos = (MesFilms.conf.StrTitleSelect.Length > 0) ? MesFilms.conf.StrTitleSelect.Length + 1 : 0; //only display rest of title after selected part common to group
                MesFilms.conf.StrTIndex = MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString().Substring(TitlePos);
                GUIControl.ShowControl(GetID, (int)Controls.CTRL_Title);
            }
            else
                GUIControl.HideControl(GetID, (int)Controls.CTRL_Title);
            if (!(MesFilms.conf.StrTitle2 == "(none)"))
            {
                if ((MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString() == MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle2].ToString()) || (MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle2].ToString().Length == 0))
                    GUIControl.HideControl(GetID, (int)Controls.CTRL_OTitle);
                else
                    GUIControl.ShowControl(GetID, (int)Controls.CTRL_OTitle);
            }
            else
                GUIControl.HideControl(GetID, (int)Controls.CTRL_OTitle);

            string file = "false";
            if (MesFilms.r[MesFilms.conf.StrIndex]["Picture"].ToString().Length > 0)
            {
                if ((MesFilms.r[MesFilms.conf.StrIndex]["Picture"].ToString().IndexOf(":\\") == -1) && (MesFilms.r[MesFilms.conf.StrIndex]["Picture"].ToString().Substring(0, 2) != "\\\\"))
                    file = MesFilms.conf.StrPathImg + "\\" + MesFilms.r[MesFilms.conf.StrIndex]["Picture"].ToString();
                else
                    file = MesFilms.r[MesFilms.conf.StrIndex]["Picture"].ToString();
            }
            else
                file = "";
            if (!System.IO.File.Exists(file))
                file = MesFilms.conf.DefaultCover;
            
            //Should not Disable because of SpeedThumbs - Not working here .....
            GUIPropertyManager.SetProperty("#myfilms.picture", file);
            // ToDo: Add for ImageSwapper Coverart (coverImage)
            //cover.Filename = file;
            if ((ImgID2001 != null) && (ImgID2002 != null) && (MesFilms.conf.StrLogos))
            {
                if ((ImgID2001.XPosition == ImgID2002.XPosition) && (ImgID2001.YPosition == ImgID2002.YPosition))
                {
                    try
                    {
                        string wlogos = Logos.Build_Logos(MesFilms.r[MesFilms.conf.StrIndex], "ID2003", ImgID2001.Height + ImgID2002.Height, ImgID2001.Width + ImgID2002.Width, ImgID2001.XPosition, ImgID2001.YPosition, 1, GetID);
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2001", wlogos);
                        GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2001);
                        GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2002);
                        GUIControl.HideControl(GetID, (int)Controls.CTRL_Format);
                        GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
                    }
                    catch (Exception e)
                    {
                        Log.Error("MyFilms : " + e.Message);
                    }
                }
                else
                {
                    try
                    {
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2001", Logos.Build_Logos(MesFilms.r[MesFilms.conf.StrIndex], "ID2001", ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, 1, GetID));
                        GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2001);
                        GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
                        GUIControl.HideControl(GetID, (int)Controls.CTRL_Format);
                    }
                    catch (Exception e)
                    {
                        Log.Error("MyFilms : " + e.Message);
                    }
                    try
                    {
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2002", Logos.Build_Logos(MesFilms.r[MesFilms.conf.StrIndex], "ID2002", ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, 1, GetID));
                        GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2002);
                        GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2002);
                    }
                    catch (Exception e)
                    {
                        Log.Error("MyFilms : " + e.Message);
                    }
                }
            }
            else
            {
                if ((ImgID2001 != null) && (MesFilms.conf.StrLogos))
                {
                    try
                    {
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2001", Logos.Build_Logos(MesFilms.r[MesFilms.conf.StrIndex], "ID2001", ImgID2001.Height, ImgID2001.Width, ImgID2001.XPosition, ImgID2001.YPosition, 1, GetID));
                        GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2001);
                        GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2001);
                        GUIControl.HideControl(GetID, (int)Controls.CTRL_Format);
                    }
                    catch (Exception e)
                    {
                        Log.Error("MyFilms : " + e.Message);
                    }
                }
                else
                {
                    GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2001);
                    GUIControl.ShowControl(GetID, (int)Controls.CTRL_Format);
                }
                if ((ImgID2002 != null) && (MesFilms.conf.StrLogos))
                {
                    try
                    {
                        GUIPropertyManager.SetProperty("#myfilms.logos_id2002", Logos.Build_Logos(MesFilms.r[MesFilms.conf.StrIndex], "ID2002", ImgID2002.Height, ImgID2002.Width, ImgID2002.XPosition, ImgID2002.YPosition, 1, GetID));
                        GUIControl.ShowControl(GetID, (int)Controls.CTRL_logos_id2002);
                        GUIControl.RefreshControl(GetID, (int)Controls.CTRL_logos_id2002);
                    }
                    catch (Exception e)
                    {
                        Log.Error("MyFilms : " + e.Message);
                    }
                }
                else
                    GUIControl.HideControl(GetID, (int)Controls.CTRL_logos_id2002);
            }
            
            //ImageSwapper backdrop = new ImageSwapper();
            string[] wfanart = new string[2];
            string wtitle = MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"].ToString();
			//Added by Guzzi to fix Fanartproblem when Mastertitle is set to OriginalTitle
            if (!(MesFilms.conf.StrTitle1 == "OriginalTitle"))
            {

                if (MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"] != null && MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                    wtitle = MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString();
            }
            if (ImgFanartDir != null)
            {
                wfanart = Search_Fanart(wtitle, false, "dir", false, file, string.Empty);
                Log.Debug("MesFilm (afficher_detail): Backdrops-File (dir): wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
            }
            else
            {
                wfanart = Search_Fanart(wtitle, false, "file", false, file, string.Empty);
                Log.Debug("MesFilm (afficher_detail): Backdrops-File (file): wfanart[0]: '" + wfanart[0] + "', '" + wfanart[1] + "'");
            }

            if (wfanart[0] == " ")
            {
                if (ImgFanartDir != null)
                    ImgFanartDir.PreAllocResources();
                GUIControl.HideControl(GetID, 35);
            }
            else
            {
                GUIControl.ShowControl(GetID, 35);
                if (wfanart[1] == "dir")
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
                    GUIPropertyManager.SetProperty("#myfilms.fanart", wfanart[0].ToString()); 
                    GUIControl.ShowControl(GetID, (int)Controls.CTRL_Fanart);
                }
            }
            if (!(MesFilms.conf.StrStorage.Length == 0) && !(MesFilms.conf.StrStorage == "(none)"))
            {
                if (MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrStorage].ToString().Length > 0)
                {
                    int at = MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrStorage].ToString().IndexOf(";", 0, MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrStorage].ToString().Length);
                    if (at == -1)
                        file = SearchMovie(MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrStorage].ToString().Substring(0, MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrStorage].ToString().Length).Trim().ToString(), MesFilms.conf.StrDirStor);
                    else
                        file = SearchMovie(MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrStorage].ToString().Substring(0, at).Trim().ToString(), MesFilms.conf.StrDirStor);
                }
                else
                    file = "false";
                if (!(file == "false") && (file.Length > 0))
                    GUIControl.ShowControl(GetID, (int)Controls.CTRL_ImgDD);
                else
                    GUIControl.HideControl(GetID, (int)Controls.CTRL_ImgDD);
            }
            else
                GUIControl.HideControl(GetID, (int)Controls.CTRL_ImgDD);

            Load_Detailed_DB(MesFilms.conf.StrIndex, true);
            if (MesFilms.conf.StrIndex == StrMax - 1)
            {
                GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnNext);
                GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnLast);
            }
            else
            {
                GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnNext);
                GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnLast);
            }
            if (MesFilms.conf.StrIndex == 0)
            {
                GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnPrior);
                GUIControl.DisableControl(GetID, (int)Controls.CTRL_BtnFirst);
            }
            else
            {
                GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnPrior);
                GUIControl.EnableControl(GetID, (int)Controls.CTRL_BtnFirst);
            }
        }
        //-------------------------------------------------------------------------------------------
        //  initialize exported fields to skin as '#myfilms.<ant db column name> 
        //-------------------------------------------------------------------------------------------
        public static void Init_Detailed_DB()
        {
            AntMovieCatalog ds = new AntMovieCatalog();
            string wstring = " "; 
            foreach (DataColumn dc in ds.Movie.Columns)
            {
                //string wstring = " ";
                GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                //Guzzi:temporarily diabled
                //Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
            }

            GUIPropertyManager.SetProperty("#myfilms.aspectratio", wstring);
            //Log.Debug("MyFilms : Property loaded #myfilms.aspectratio with " + wstring);
            GUIPropertyManager.SetProperty("#myfilms.ar", wstring);
            //Log.Debug("MyFilms : Property loaded #myfilms.ar with " + wstring);

            GUIPropertyManager.SetProperty("#myfilms.mastertitle", wstring);
            //Log.Debug("MyFilms : Property loaded #myfilms.mastertitle with " + wstring);
            GUIPropertyManager.SetProperty("#myfilms.secondarytitle", wstring);
            //Log.Debug("MyFilms : Property loaded #myfilms.secondarytitle with " + wstring);

        }

        //-------------------------------------------------------------------------------------------
        //  Load detailed db fields : export fields to skin as '#myfilms.<ant db column name> 
        //-------------------------------------------------------------------------------------------
        public static void Load_Detailed_DB(int ItemId, bool wrep)
        {
     
            string wstrformat = " ";
            AntMovieCatalog ds = new AntMovieCatalog();
            
            foreach (DataColumn dc in ds.Movie.Columns)
            {
                string wstring = " ";

                if (MesFilms.r[ItemId][dc.ColumnName] != null)
                {
                    switch (dc.ColumnName.ToLower())
                    {
                        case "translatedtitle":
                        case "originaltitle":
                            if (wrep)
                                if (MesFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                                    if (MesFilms.r[ItemId][dc.ColumnName].ToString().Contains(MesFilms.conf.TitleDelim))
                                        wstring = MesFilms.r[ItemId][dc.ColumnName].ToString().Substring(MesFilms.r[ItemId][dc.ColumnName].ToString().LastIndexOf(MesFilms.conf.TitleDelim) + 1);
                                    else
                                        wstring = MesFilms.r[ItemId][dc.ColumnName].ToString();
                            GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                            //Guzzi: Temporarily disabled
                            // Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);

                            if ((MesFilms.conf.StrTitle1.ToLower() == (dc.ColumnName.ToLower())))
                            {
                                GUIPropertyManager.SetProperty("#myfilms.mastertitle", wstring);
                                //Log.Debug("MyFilms : Property loaded #myfilms.mastertitle with " + wstring);
                            }
                            if ((MesFilms.conf.StrTitle2.ToLower() == (dc.ColumnName.ToLower())))
                            {
                                GUIPropertyManager.SetProperty("#myfilms.secondarytitle", wstring);
                                //Log.Debug("MyFilms : Property loaded #myfilms.secondarytitle with " + wstring);
                            }

                            break;
                        case "length":
                        case "length_num":
                            if (wrep)
                                if (MesFilms.r[ItemId]["Length"].ToString().Length > 0)
                                    wstring = MesFilms.r[ItemId]["Length"].ToString() + GUILocalizeStrings.Get(2998);
                            GUIPropertyManager.SetProperty("#myfilms.length", wstring);
                            //Guzzi: Temporarily disabled
                            // Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                            break;
                        case "actors":
                            if (wrep)
                                if (MesFilms.r[ItemId]["Actors"].ToString().Length > 0)
                                {
                                    wstring = MesFilms.r[ItemId]["Actors"].ToString().Replace('|', '\n');
                                    wstring = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wstring.ToString()));
                                }
                            GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                            //Guzzi: Temporarily disabled
                            // Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                            break;
                        case "description":
                        case "comments":
                            if (wrep)
                                if (MesFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                                {
                                    wstring = System.Web.HttpUtility.HtmlEncode(MesFilms.r[ItemId][dc.ColumnName].ToString().Replace('’', '\''));
                                    wstring = wstring.ToString().Replace('|', '\n');
                                    wstring = wstring.ToString().Replace('…', '.');
                                    wstring = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wstring.ToString()));
                                }
                            GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                            //Guzzi: Temporarily disabled
                            // Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                            break;
                        case "date":
                            if (wrep)
                                if (MesFilms.r[ItemId]["Date"].ToString().Length > 0)
                                    wstring = MesFilms.r[ItemId][dc.ColumnName].ToString();
                            GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                            //Guzzi: Temporarily disabled
                            // Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                            break;
                        case "videoformat":
                            if (wrep)
                                if (MesFilms.r[ItemId]["VideoFormat"].ToString().Length > 0)
                                    wstring = MesFilms.r[ItemId][dc.ColumnName].ToString();
                            GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                            wstrformat = "V:" + MesFilms.r[ItemId]["VideoFormat"].ToString();
                            //Guzzi: Temporarily disabled
                            // Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                            break;
                        case "audioformat":
                            if (wrep)
                                if (MesFilms.r[ItemId]["AudioFormat"].ToString().Length > 0)
                                {
                                    wstring = MesFilms.r[ItemId][dc.ColumnName].ToString(); 
                                    if (wstrformat.Length > 1)
                                        wstrformat = "Format " + wstrformat + ",A:" + MesFilms.r[ItemId]["AudioFormat"].ToString();
                                    else
                                        wstrformat = "Format A:" + MesFilms.r[ItemId]["AudioFormat"].ToString();
                                }
                            GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                            //Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                            GUIPropertyManager.SetProperty("#myfilms.format", wstrformat);
                            //Log.Debug("MyFilms : Property loaded #myfilms.format with " + wstrformat);
                            break;
                        case "rating":
                            wstring = "0";
                            if ((wrep) && (MesFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                                wstring = MesFilms.r[ItemId][dc.ColumnName].ToString();
                            try
                            {
                                MesFilms.conf.W_rating = (decimal)MesFilms.r[ItemId][dc.ColumnName];
                            }
                            catch
                            {
                                MesFilms.conf.W_rating = 0;
                            }
                            GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                            //Guzzi: Temporarily disabled
                            // Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                            break;
                        case "contents_id":
                        case "dateadded":
                        case "picture":
                            break;
                        case "resolution":
                            decimal aspectratio = 0; 
                            string ar = " ";
                            if ((wrep) && (MesFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                                try
                                    {
                                        wstring = MesFilms.r[ItemId][dc.ColumnName].ToString();
                                        //Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                                        GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                                        //Log.Debug("MyFilms (Load_Detailed_DB): Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                                        decimal w_hsize;
                                        decimal w_vsize;
                                        string[] arSplit;
                                        string[] Sep = new string[] { "x" };
                                        arSplit = MesFilms.r[ItemId][dc.ColumnName].ToString().Split(Sep, StringSplitOptions.RemoveEmptyEntries); // remove entries empty // StringSplitOptions.None);//will add "" entries also
                                        w_hsize = (decimal)Convert.ToInt32(arSplit[0]);
                                        w_vsize = (decimal)Convert.ToInt32(arSplit[1]);
                                        // To Check if/why exception eccurs
                                        //Log.Debug("MyFilms : hsize - wsize: '" + w_hsize + " - " + w_vsize + "'");
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
                                        Log.Info("MyFilms: Error calculating aspectratio !");
                                    }

                            GUIPropertyManager.SetProperty("#myfilms.aspectratio", wstring);
                            //Log.Debug("MyFilms : Property loaded #myfilms.aspectratio with " + wstring);
                            GUIPropertyManager.SetProperty("#myfilms.ar", ar);
                            //Log.Debug("MyFilms : Property loaded #myfilms.ar with " + ar);
                            //Log.Debug("MyFilms (Load_Detailed_DB): Split for aspectratio: '" + (arSplit[0]) + "', '" + (arSplit[1]) + "' --> '" + wstring + "'");
                            break;
                        default:
                            if ((wrep) && (MesFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                                wstring = MesFilms.r[ItemId][dc.ColumnName].ToString();
                            GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                            // Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);

                            if ((MesFilms.conf.StrTitle1.ToLower() == (dc.ColumnName.ToLower())))
                            {
                                GUIPropertyManager.SetProperty("#myfilms.mastertitle", wstring);
                                //Log.Debug("MyFilms : Property loaded #myfilms.mastertitle with " + wstring);
                            }
                            if ((MesFilms.conf.StrTitle2.ToLower() == (dc.ColumnName.ToLower())))
                            {
                                GUIPropertyManager.SetProperty("#myfilms.secondarytitle", wstring);
                                //Log.Debug("MyFilms : Property loaded #myfilms.secondarytitle with " + wstring);
                            }
                            break;
                    }
                }
            }
        }

        //-------------------------------------------------------------------------------------------
        //  Load detailed infos for persons: export fields to skin as '#myfilms.<ant db column name> 
        //-------------------------------------------------------------------------------------------
        public static void Load_Detailed_PersonInfo(string artistname, bool wrep)
        {
            //Todo: Add new properties for publishing actorinfos (MiniBio, Bio, Birthdate, Image, Multiimage, etc.

            GUIPropertyManager.SetProperty("#myfilms.description", " ");
            ArrayList actorList = new ArrayList();
            VideoDatabase.GetActorByName(artistname, actorList);
            if (actorList.Count == 0)
            {
                return;
            }
            int actorID;
            string actorname = "";
            // Define splitter for string
            char[] splitter = { '|' };
            // Iterate through list
            foreach (string act in actorList)
            {
                string[] strActor = act.Split(splitter);
                actorID = Convert.ToInt32(strActor[0]);
                actorname = strActor[1];
                if (actorID.ToString().Length > 0)
                {
                    IMDBActor actor = VideoDatabase.GetActorInfo(actorID);
                    if (actor.Biography.Length > 0) GUIPropertyManager.SetProperty("#myfilms.description", actor.Biography);
                }
            }
        }

        #endregion
        #region  Lecture du film demandé

        public static void Launch_Movie(int select_item, int GetID, GUIAnimation m_SearchAnimation)
        //-------------------------------------------------------------------------------------------
        // Play Movie
        //-------------------------------------------------------------------------------------------
        {
           
            // Guzzi: Added WOL to start remote host before playing the files
            // Wake up the TV server, if required
            // HandleWakeUpNas();
            Log.Info("MyFilms: Launched HandleWakeUpNas() to start movie'" + MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()] + "'");

            if (MesFilms.conf.StrCheckWOLuserdialog)
            {
                int wTimeout = MesFilms.conf.StrWOLtimeout;
                bool isActive;
                string UNCpath = MesFilms.r[select_item][MesFilms.conf.StrStorage].ToString();
                WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();

                if (UNCpath.StartsWith("\\\\"))
                UNCpath = (UNCpath.Substring(2, UNCpath.Substring(2).IndexOf("\\") + 0)).ToLower();
                if ((UNCpath.Equals(MesFilms.conf.StrNasName1, StringComparison.InvariantCultureIgnoreCase)) || (UNCpath.Equals(MesFilms.conf.StrNasName2, StringComparison.InvariantCultureIgnoreCase)) || (UNCpath.Equals(MesFilms.conf.StrNasName3, StringComparison.InvariantCultureIgnoreCase)))
                {

                    if (WakeOnLanManager.Ping(UNCpath, wTimeout))
                        isActive = true;
                    else
                        isActive = false;

                    if ((!isActive) || (true)) // Todo: DIsable "Always Show dialog"
                    {
                        GUIDialogYesNo dlgOknas =
                            (GUIDialogYesNo) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                        dlgOknas.SetHeading(GUILocalizeStrings.Get(107986)); //my films
                        dlgOknas.SetLine(1,
                                         "Movie   : '" + MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()] +
                                         "'"); //video title
                        dlgOknas.SetLine(2, "Storage: '" + UNCpath + "' - Status: '" + isActive.ToString() + "'");
                        //Filename/Storagepath
                        dlgOknas.SetLine(3, "Wollen Sie den NAS Server starten ?");
                        //dlgOknas.SetLine(3, "'" + MesFilms.conf.StrNasMAC1 + "', '" + MesFilms.conf.StrWOLtimeout.ToString() + "'");
                        dlgOknas.DoModal(GetID);
                        if (!(dlgOknas.IsConfirmed))
                            return;

                        // Start NAS Server
                        GUIDialogOK dlgOk =
                            (GUIDialogOK) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlgOk.SetHeading(GUILocalizeStrings.Get(10798624));
                        dlgOk.SetLine(1, "");

                        if ((UNCpath.Equals(MesFilms.conf.StrNasName1.ToLower())) && (MesFilms.conf.StrNasMAC1.ToString().Length > 1))
                        {
                            if (wakeOnLanManager.WakeupSystem(
                                wakeOnLanManager.GetHwAddrBytes(MesFilms.conf.StrNasMAC1.ToString()), MesFilms.conf.StrNasName1,
                                wTimeout))
                            {
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName1 + " erfolgreich gestartet!");
                            }
                            else
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName1 + " konnte nicht gestartet werden (Timeout)!");
                        }

                        if ((UNCpath.Equals(MesFilms.conf.StrNasName2.ToLower())) && (MesFilms.conf.StrNasMAC2.ToString().Length > 1))
                        {
                            if (wakeOnLanManager.WakeupSystem(
                                wakeOnLanManager.GetHwAddrBytes(MesFilms.conf.StrNasMAC2), MesFilms.conf.StrNasName2,
                                wTimeout))
                            {
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName2 + " erfolgreich gestartet!");
                            }
                            else
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName2 + " konnte nicht gestartet werden (Timeout)!");
                        }

                        if ((UNCpath.Equals(MesFilms.conf.StrNasName3.ToLower())) && (MesFilms.conf.StrNasMAC3.ToString().Length > 0))
                        {
                            if (wakeOnLanManager.WakeupSystem(
                                wakeOnLanManager.GetHwAddrBytes(MesFilms.conf.StrNasMAC3), MesFilms.conf.StrNasName3,
                                wTimeout))
                            {
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName3 + " erfolgreich gestartet!");
                            }
                            else
                                dlgOk.SetLine(2, MesFilms.conf.StrNasName3 + " konnte nicht gestartet werden (Timeout)!");
                        }

                        dlgOk.DoModal(GetID);
                    }
                }
                else
                {
                    GUIDialogOK dlgOknas = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    dlgOknas.SetHeading(GUILocalizeStrings.Get(107986)); //my films
                    //dlgOknas.SetLine(1, "Movie   : '" + MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()].ToString() + "'"); //video title
                    dlgOknas.SetLine(2, "Storage: '" + UNCpath + "' ist offline nicht für WOL konfiguriert!"); //Filename/Storagepath
                    dlgOknas.SetLine(3, "Automatischer NAS Server Start nicht möglich...");
                    dlgOknas.DoModal(GetID);
                    return;
                }
            }

            // Run externaly Program before Playing if defined in setup
            Log.Debug("MyFilms (Play Movie) select_item = '" + select_item + "' - GetID = '" + GetID + "' - m_SearchAnimation = '" + m_SearchAnimation + "'");
            setProcessAnimationStatus(true, m_SearchAnimation);
            if ((MesFilms.conf.CmdPar.Length > 0) && (MesFilms.conf.CmdPar != "(none)"))
                RunProgram(MesFilms.conf.CmdExe, MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.CmdPar].ToString());
            if (g_Player.Playing)
                g_Player.Stop();

            // search all files
            ArrayList newItems = new ArrayList();
            bool NoResumeMovie = true;
            int IMovieIndex = 0;

            //Guzzi: Added BoolType for Trailerlaunch
			Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex, false);
            //Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex);
            if (newItems.Count > 20)
            // Maximum 20 entries (limitation for MP dialogFileStacking)
            {
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                dlgOk.SetLine(1, MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()].ToString());//video title
                dlgOk.SetLine(2, "maximum 20 entries for the playlist");
                dlgOk.DoModal(GetID);
                Log.Info("MyFilms: Too many entries found for movie '" + MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()] + "', number of entries found = " + newItems.Count.ToString());
                return;
            }
            setProcessAnimationStatus(false, m_SearchAnimation);
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
                    newitem.Type = PlayListItem.PlayListItemType.Video;
                    playlist.Add(newitem);
                }
                // ask for start movie Index

                // play movie...
                PlayMovieFromPlayList(NoResumeMovie, IMovieIndex - 1);
            }
            else
            {
                //if (first)
                //// ask for mounting file first time
                //{
                GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(107986));//my films
                dlgYesNo.SetLine(1, GUILocalizeStrings.Get(219));//no disc
                if (!(MesFilms.conf.StrIdentItem == null) && !(MesFilms.conf.StrIdentItem == "(none)") && !(MesFilms.conf.StrIdentItem == ""))
                    if (MesFilms.conf.StrIdentLabel.Length > 0)
                        dlgYesNo.SetLine(2, MesFilms.conf.StrIdentLabel.ToString() + " = " + MesFilms.r[select_item][MesFilms.conf.StrIdentItem].ToString());//Label Identification for Media
                    else
                        dlgYesNo.SetLine(2, "'" + MesFilms.conf.StrIdentItem.ToString() + "' = " + MesFilms.r[select_item][MesFilms.conf.StrIdentItem].ToString());//ANT Item Identification for Media
                else
                    dlgYesNo.SetLine(2, "' disc n° = " + MesFilms.r[select_item]["Number"].ToString());//ANT Number for Identification Media 
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)
                    Launch_Movie(select_item, GetID,m_SearchAnimation);
                //}
                else
                {
                    GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                    dlgOk.SetLine(1, GUILocalizeStrings.Get(1036));//no video found
                    dlgOk.SetLine(2, MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()].ToString());
                    dlgOk.DoModal(GetID);
                    Log.Info("MyFilms: File not found for movie '" + MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()]);
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
            Log.Debug("MyFilms (Play Movie Trailer) select_item = '" + select_item + "' - GetID = '" + GetID + "' - m_SearchAnimation = '" + m_SearchAnimation + "'");
            if ((MesFilms.conf.CmdPar.Length > 0) && (MesFilms.conf.CmdPar != "(none)"))
                RunProgram(MesFilms.conf.CmdExe, MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.CmdPar].ToString());
            if (g_Player.Playing)
                g_Player.Stop();
            // search all files
            ArrayList newItems = new ArrayList();
            bool NoResumeMovie = true; //Modded by Guzzi for NonResuming Trailers 
            int IMovieIndex = 0;

            Log.Debug("MyFilmsDetails (Launch_Movie_Trailer): new do Moviesearch with '" + select_item + "' (Selected_Item"); 
            //Change back, if method in original properly adapted with bool Trailer
            Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex, true);
            Log.Debug("MyFilmsDetails (Launch_Movie_Trailer): newItems.Count: '" + newItems.Count.ToString() + "'"); 
            if (newItems.Count > 20)
            // Maximum 20 entries (limitation for MP dialogFileStacking)
            {
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                dlgOk.SetLine(1, MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()].ToString());//video title
                dlgOk.SetLine(2, "maximum 20 entries for the playlist");
                dlgOk.DoModal(GetID);
                Log.Info("MyFilms: Too many entries found for movie '" + MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()] + "', number of entries found = " + newItems.Count.ToString());
                return;
            }
            setProcessAnimationStatus(false, m_SearchAnimation);
            if (newItems.Count > 1)
            {
                if ((NoResumeMovie == true)) //Modded by Guzzi to always get Selectiondialog for Trailer Choice
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
                    newitem.Type = PlayListItem.PlayListItemType.Video;
                    playlist.Add(newitem);
                }
                // ask for start movie Index
                
                // play movie...
                PlayMovieFromPlayListTrailer(NoResumeMovie, IMovieIndex - 1);
            }
            else
            {
                GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                dlgOk.SetLine(1, GUILocalizeStrings.Get(1036));//no video found
                dlgOk.SetLine(2, MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()].ToString());
                dlgOk.DoModal(GetID);
                Log.Info("MyFilms: File not found for movie '" + MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()]);
                return;
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
                if ((Utils.ShouldStack(fileName, wfile)) && (Utils.IsVideo(wfile)))
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
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(936) + Utils.SecondsToHMSString(timeMovieStopped));
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
            // update the MP Video Database for OSD view during playing // ToDo: Check proper fit for changes in Mediaportal 1.2 by Deda !!!
            update_database(fileName, select_item, -1);
            newItems.Add(fileName);
        }
        private void OnPlayBackStarted(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            if (type != g_Player.MediaType.Video) return;
            // store informations for action at endplayback if any
            MesFilms.conf.StrPlayedIndex = MesFilms.conf.StrIndex;
            MesFilms.conf.StrPlayedDfltSelect = MesFilms.conf.StrDfltSelect;
            MesFilms.conf.StrPlayedSelect = MesFilms.conf.StrFilmSelect;
            MesFilms.conf.StrPlayedSort = MesFilms.conf.StrSorta;
            MesFilms.conf.StrPlayedSens = MesFilms.conf.StrSortSens;
            int idFile = VideoDatabase.GetFileId(filename);
            if (idFile != -1)
            {
                int movieDuration = (int)g_Player.Duration;
                VideoDatabase.SetMovieDuration(idFile, movieDuration);
            }
        }
        private void OnPlayBackEnded(MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            UpdateOnPlayEnd(type, 0, filename, true);
        }
        private void OnPlayBackStopped(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename)
        {
            UpdateOnPlayEnd(type, timeMovieStopped, filename, false);
        }
        private void UpdateOnPlayEnd(MediaPortal.Player.g_Player.MediaType type, int timeMovieStopped, string filename, bool ended)
        {

            if (MesFilms.conf.StrPlayedIndex == -1)
                return;
            if (type != g_Player.MediaType.Video || filename.EndsWith("&txe=.wmv"))
                return;
            try
            {
                DataRow[] r1 = BaseMesFilms.LectureDonnées(MesFilms.conf.StrPlayedDfltSelect, MesFilms.conf.StrPlayedSelect, MesFilms.conf.StrPlayedSort, MesFilms.conf.StrPlayedSens);
                // Handle all movie files from idMovie
                ArrayList movies = new ArrayList();
                int iidMovie = VideoDatabase.GetMovieId(filename);
                if (iidMovie >= 0)
                {
                    VideoDatabase.GetFiles(iidMovie, ref movies);
                    if (movies.Count <= 0)
                        return;
                    foreach (object t in movies)
                    {
                        string strFilePath = (string)t;
                        int idFile = VideoDatabase.GetFileId(strFilePath);
                        byte[] resumeData = null;
                        if (idFile < 0)
                            break;
                        if (ended)
                        {
                            // Set resumedata to zero
                            VideoDatabase.GetMovieStopTimeAndResumeData(idFile, out resumeData);
                            VideoDatabase.SetMovieStopTimeAndResumeData(idFile, 0, resumeData);
                            Log.Info("MyFilms: GUIVideoFiles: OnPlayBackEnded idFile={0} resumeData={1}", idFile, resumeData);
                        }
                        else
                        {
                            if ((filename == strFilePath) && (timeMovieStopped > 0))
                            {
                                g_Player.Player.GetResumeState(out resumeData);
                                Log.Info("MyFilms: GUIVideoFiles: OnPlayBackStopped idFile={0} timeMovieStopped={1} resumeData={2}", idFile, timeMovieStopped, resumeData);
                                VideoDatabase.SetMovieStopTimeAndResumeData(idFile, timeMovieStopped, resumeData);
                                Log.Debug("MyFilms: GUIVideoFiles: OnPlayBackStopped store resume time");
                            }
                            else
                                VideoDatabase.DeleteMovieStopTime(idFile);
                        }
                    }
                    if (ended)
                    {
                        IMDBMovie details = new IMDBMovie();
                        VideoDatabase.GetMovieInfoById(iidMovie, ref details);
                        details.Watched++;
                        VideoDatabase.SetWatched(details);
                    }
                }
                if (MesFilms.conf.CheckWatched)
                    r1[MesFilms.conf.StrPlayedIndex]["Checked"] = "True";
                if (ended)
                {
                    if (MesFilms.conf.StrSupPlayer)
                        Suppress_Entry(r1, MesFilms.conf.StrPlayedIndex);
                }
                if ((MesFilms.conf.CheckWatched) || (MesFilms.conf.StrSupPlayer))
                    Update_XML_database();
                MesFilms.conf.StrPlayedIndex = -1;
                MesFilms.conf.StrPlayedDfltSelect = string.Empty;
                MesFilms.conf.StrPlayedSelect = string.Empty;
                MesFilms.conf.StrPlayedSort = string.Empty;
                MesFilms.conf.StrPlayedSens = string.Empty;
            }
            catch
            {
                Log.Info("MyFilms: Error during PlayBackEnded ");
            }
        }

        private static string LoadPlaylist(string filename)
        {
            PlayList playlist = new PlayList();
            IPlayListIO loader = PlayListFactory.CreateIO(filename);

            if (!loader.Load(playlist, filename))
            {
                Log.Info("MyFilms: Playlist not found for movie : '" + filename.ToString() + "'");
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
                if (MesFilms.r[select_item]["Subtitles"].ToString().Length > 0)
                    idMovie = VideoDatabase.AddMovie(filename, true);
                else
                    idMovie = VideoDatabase.AddMovie(filename, false);
                string wdescription = System.Web.HttpUtility.HtmlEncode(MesFilms.r[select_item]["Description"].ToString().Replace('’', '\''));
                wdescription = wdescription.ToString().Replace('|', '\n');
                movieDetails.PlotOutline = movieDetails.Plot = System.Web.HttpUtility.HtmlDecode(MediaPortal.Util.HTMLParser.removeHtml(wdescription.ToString()));

                movieDetails.Title = MesFilms.r[select_item][MesFilms.conf.StrTitle1].ToString();
                try
                { movieDetails.RunTime = Int32.Parse(MesFilms.r[select_item]["Length"].ToString()); }
                catch
                { movieDetails.RunTime = 0; }
                try
                { movieDetails.Rating = (float)Double.Parse(MesFilms.r[select_item]["Rating"].ToString()); }
                catch
                { movieDetails.Rating = 0; }
                try
                { movieDetails.Year = Int32.Parse(MesFilms.r[select_item]["Year"].ToString()); }
                catch
                { movieDetails.Year = 0; }


                // Modified to match changes by Deda in MyVideos (New Thumbformat)

                string strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, movieDetails.Title);
                string LargeThumb = MediaPortal.Util.Utils.GetLargeCoverArtName(Thumbs.MovieTitle, movieDetails.Title);
                //string strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, movieDetails.Title) + "{" + idMovie.ToString() + "}";;
                //string LargeThumb = MediaPortal.Util.Utils.GetLargeCoverArtName(Thumbs.MovieTitle, movieDetails.Title) + "{" + idMovie.ToString() + "}";;
                //Log.Debug("MyFilms (ThumbCreation): strThumb: '" + strThumb.ToString() + "'");
                //Log.Debug("MyFilms (ThumbCreation): LargeThumb: '" + LargeThumb.ToString() + "'");

                try
                {
                    string wImage;
                    if ((MesFilms.r[select_item]["Picture"].ToString().IndexOf(":\\") == -1) && (MesFilms.r[select_item]["Picture"].ToString().Substring(0, 2) != "\\\\"))
                        wImage = MesFilms.conf.StrPathImg + "\\" + MesFilms.r[select_item]["Picture"].ToString();
                    else
                        wImage = MesFilms.r[select_item]["Picture"].ToString();
                    if (!System.IO.File.Exists(wImage))
                        wImage = Config.Dir.Skin + "\\media\\Films_No_Cover.png";
                    if (System.IO.File.Exists(wImage))
                    {
                        if (!System.IO.File.Exists(strThumb))
                            Picture.CreateThumbnail(wImage, strThumb, (int)Thumbs.ThumbResolution, (int)Thumbs.ThumbResolution, 0, Thumbs.SpeedThumbsSmall);
                        if (!System.IO.File.Exists(LargeThumb))
                            Picture.CreateThumbnail(wImage, LargeThumb, (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0, Thumbs.SpeedThumbsLarge);
                        // CreateThumbnail Version 0.2.3 + SVN
                        //if (Picture.CreateThumbnail(wImage, strThumb, (int)Thumbs.ThumbResolution, (int)Thumbs.ThumbResolution, 0, Thumbs.SpeedThumbsSmall))
                        //Picture.CreateThumbnail(wImage, LargeThumb, (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0, Thumbs.SpeedThumbsLarge);
                        //// CreateThumbnail Version 0.2.3 Stable
                        //if (Picture.CreateThumbnail(wImage, strThumb, (int)Thumbs.ThumbResolution, (int)Thumbs.ThumbResolution, 0))
                        //    Picture.CreateThumbnail(wImage, LargeThumb, (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0);

                    }
                }
                catch
                {
                }
                movieDetails.Director = MesFilms.r[select_item]["Director"].ToString();
                wzone = null;
                Analyze(MesFilms.r[select_item]["Category"].ToString(), idMovie, "Genre");
                movieDetails.Genre = wzone;
                wzone = null;
                Analyze(MesFilms.r[select_item]["Actors"].ToString(), idMovie, "Actor");
                movieDetails.Cast = wzone;
                VideoDatabase.SetMovieInfoById(idMovie, ref movieDetails);
            }
            else
            {
                int pathId, widMovie;
                widMovie = VideoDatabase.GetFile(filename, out pathId, out widMovie, true);
                if (!(widMovie == idMovie))
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
            w_tableau = MesFilms.Search_String(champselect);
            foreach (object t in w_tableau)
            {
                update_fieldDB(t.ToString(), iidmovie, field);
            }
        }

        private static void Search_All_Files(int select_item, bool delete, ref bool NoResumeMovie, ref ArrayList newItems, ref int IMovieIndex, bool Trailer)
        {
            string fileName = string.Empty;
            string[] split1;
            string[] split2;
            string[] split3;
            string strDir = MesFilms.conf.StrDirStor;
            int IdMovie = -1;
            int timeMovieStopped = 0;
            if (Trailer) strDir = MesFilms.conf.StrDirStorTrailer;
            else strDir = MesFilms.conf.StrDirStor;

            Log.Debug("MyFilmsDetails (Search_All_Files) - StrDirStor: " + MesFilms.conf.StrDirStor);
            Log.Debug("MyFilmsDetails (Search_All_Files) - StrDirStortrailer: " + MesFilms.conf.StrDirStorTrailer);
            Log.Debug("MyFilmsDetails (Search_All_Files) - Modus 'Trailer' = '" + Trailer.ToString() + "' - strDir: '" + strDir + "'");
            // retrieve filename information stored in the DB
            if (Trailer) Log.Debug("MyFilmsDetails (Search_All_Files) - try filename MesFilms.r[select_item][MesFilms.conf.StrStorageTrailer]: '" + (string)MesFilms.r[select_item][MesFilms.conf.StrStorageTrailer].ToString().Trim() + "' - ConfStorageTrailer: '" + MesFilms.conf.StrStorageTrailer + "'");
            else Log.Debug("MyFilmsDetails (Search_All_Files) - try filename MesFilms.r[select_item][MesFilms.conf.StrStorage]: '" + (string)MesFilms.r[select_item][MesFilms.conf.StrStorage].ToString().Trim() + "' - ConfStorage: '" + MesFilms.conf.StrStorage + "'");
            if (Trailer)
            {
                try
                { fileName = (string)MesFilms.r[select_item][MesFilms.conf.StrStorageTrailer].ToString().Trim(); }
                catch
                { fileName = ""; }
            }
            else
                if (!(MesFilms.conf.StrStorage == null) && !(MesFilms.conf.StrStorage == "(none)") && !(MesFilms.conf.StrStorage == ""))
                {
                    try
                    { fileName = (string)MesFilms.r[select_item][MesFilms.conf.StrStorage].ToString().Trim(); }
                    catch
                    { fileName = ""; }
                }
            if (fileName.Length == 0)
            {
                // search filename by Title movie
                if ((MesFilms.conf.SearchFile == "True") || (MesFilms.conf.SearchFile == "yes"))
                {
                    string movieName = MesFilms.r[select_item][MesFilms.conf.ItemSearchFile].ToString();
                    movieName = movieName.Substring(movieName.LastIndexOf(MesFilms.conf.TitleDelim) + 1).Trim();
                    if (MesFilms.conf.ItemSearchFile.Length > 0)
                        fileName = Search_FileName(movieName, MesFilms.conf.StrDirStor).Trim();
                    if ((fileName.Length > 0) && (!(MesFilms.conf.StrStorage.Length == 0) && !(MesFilms.conf.StrStorage == "(none)")))
                    {
                        MesFilms.r[select_item][MesFilms.conf.StrStorage] = fileName;
                        Update_XML_database();
                    }
                }
            }
            bool wisomounted = false;
            string wisofile = string.Empty;
            // split filename information delimited by semicolumn (multifile detection)
            split1 = fileName.ToString().Split(new Char[] { ';' });
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
                    if ((System.IO.File.Exists(fileName)) && (Utils.IsVideo(fileName)) && (!VirtualDirectory.IsImageFile(System.IO.Path.GetExtension(fileName))))
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
                    if (!(fileName.Substring(0, 2) == "\\\\") && !(fileName.Substring(1, 1) == ":"))
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
                    split3 = fileName.ToString().Split(new Char[] { ';' });
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
                        if ((System.IO.File.Exists(wfile3)) && (Utils.IsVideo(wfile3)))
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

                        if (System.IO.Directory.Exists(wfile3))
                        {
                            // it's a Directory so all files included are added to playlist
                            DirectoryInfo dirsInf = new DirectoryInfo(wfile3);
                            //On retourne une liste d'informations sur les fichiers contenus dans le répertoire
                            FileSystemInfo[] filesInf = dirsInf.GetFileSystemInfos();
                            foreach (FileSystemInfo fi in filesInf)
                            {
                                if (Utils.IsVideo(fi.FullName))
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
            result = new ArrayList();
            string file = filename;

            SearchFiles(file, storage, false, false);

            //si des resultats existent on les affichent
            if (result.Count != 0)
            {
                if (result.Count == 1)
                    return result[0].ToString();
                string wfile = null;
                result.Sort();
                ArrayList wresult = new ArrayList();
                foreach (String s in result)
                {
                    if (!Utils.ShouldStack(s, wfile) && s.ToString().ToLower() != wfile.ToLower())
                    {
                        wresult.Add(s);
                        wfile = s;
                    }
                }
                if (wresult.Count == 1)
                    return wresult[0].ToString();
                else
                {
                    // Many files found; ask for the good file
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg == null) return "";
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
            else
            // No Movie File Found
            {
                Log.Info("MyFilms: File not found for movie '" + filename.ToString() + "'; string search '" + wsearchfile.ToString() + "'");
                return "";
            }
        }
        /// <summary>
        /// Permet de rechercher un fichier dans un dossier et ces sous dossier
        /// </summary>
        /// <param name="fileName">le nom du fichier recherché</param>
        /// <param name="path">le chemin du répertoire dans lequel chercher</param>
        /// <param name="recur">spécifie s'il s'agit d'une relance recursive</param>
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
                file = Replace_String(file.ToString());
                wsearchfile = file;
            }
            string[] SearchDir = oRegex.Split(searchrep);
            oRegex = new System.Text.RegularExpressions.Regex(file);
            string wpath;
            foreach (string path in SearchDir)
            {
                if (!(path.LastIndexOf(@"\") == path.Length - 1))
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
                    if (MesFilms.conf.SearchSubDirs == "no") continue;
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
                    wzone = champselect.ToString();
                else
                    wzone = wzone + "/" + champselect.ToString();
            }
            else
            {
                VideoDatabase.GetMovieInfoById(iidmovie, ref movieDetails);
                IMDBActor.IMDBActorMovie actorMovie = new IMDBActor.IMDBActorMovie();
                actorMovie.MovieTitle = movieDetails.Title;
                actorMovie.Year = movieDetails.Year;
                int iiActor = VideoDatabase.AddActor(champselect.ToString());
                VideoDatabase.AddActorToMovie(iidmovie, iiActor);
                VideoDatabase.AddActorInfoMovie(iiActor, actorMovie);
                if (wzone == null)
                    wzone = champselect.ToString();
                else
                    wzone = wzone + "\n" + champselect.ToString();
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
                    if ((MesFilms.conf.SearchSubDirs == "no") || (!System.IO.Directory.Exists(wpath))) continue;
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

            if (!Utils.IsDVD(movieDetails.Path)) return true;
            string cdlabel = String.Empty;
            cdlabel = Utils.GetDriveSerial(movieDetails.Path);
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
                            cdlabel = Utils.GetDriveSerial(movieDetails.Path);
                            VideoDatabase.UpdateCDLabel(movieDetails, cdlabel);
                            movieDetails.CDLabel = cdlabel;
                            return true;
                        }
                    }
                    else
                    {
                        cdlabel = Utils.GetDriveSerial(movieDetails.Path);
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
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(936) + Utils.SecondsToHMSString(timeMovieStopped));
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
                        dlgYesNo.SetLine(2, GUILocalizeStrings.Get(936) + Utils.SecondsToHMSString(timeMovieStopped));
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
            XmlConfig XmlConfig = new XmlConfig();    
            m_askBeforePlayingDVDImage = XmlConfig.ReadXmlConfig("MyFilms", "daemon", "askbeforeplaying", false);
//            }
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
//                ProcessStartInfo psI = new ProcessStartInfo(exeName, argsLine);
                Process newProcess = new Process();

                 try
                {
                    newProcess.StartInfo.FileName = exeName;
                    newProcess.StartInfo.Arguments = argsLine;
                    newProcess.StartInfo.UseShellExecute = true;
                    newProcess.StartInfo.CreateNoWindow = true;
                    newProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
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

        //-------------------------------------------------------------------------------------------
        //  Search All Trailerfiles locally
        //-------------------------------------------------------------------------------------------        
        public static void SearchTrailerLocal(DataRow[] r1, int Index, bool ExtendedSearch)
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

            result = new ArrayList();
            ArrayList resultsize = new ArrayList();
            string[] filesfound = new string[100];
            Int64[] filesfoundsize = new Int64[100];
            int filesfoundcounter = 0;
            string file = MesFilms.r[Index][MesFilms.conf.StrTitle1].ToString();
            string titlename = MesFilms.r[Index][MesFilms.conf.StrTitle1].ToString();
            string titlename2 = MesFilms.r[Index][MesFilms.conf.StrTitle2].ToString();
            //string file = MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString();
            //string titlename = MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString();
            //string titlename2 = MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle2].ToString();
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
                    if (((filefound.ToString().ToLower().Contains("trailer")) || (filefound.ToString().ToLower().Contains("trlr")))&& (Utils.IsVideo(filefound)))
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
                    {for (int i = 1; i < result.Count; i++)
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
                Update_XML_database();
                Log.Debug("MyFilmsDetails (SearchTrailerLocal) - Database Updatewd !!!!");
            }
        }
        

        //-------------------------------------------------------------------------------------------
        //  Delete Trailer Entries From DB
        //-------------------------------------------------------------------------------------------        
        public static void DeleteTrailerFromDB(DataRow[] r1, int Index)
        {
            MesFilms.r[Index][MesFilms.conf.StrStorageTrailer] = "";
            Log.Debug("MyFilmsDetails (DeleteTrailerFromDB) - Trailer entries Deleted for Current Movie !!!");
            Update_XML_database();
            Log.Debug("MyFilmsDetails (DeleteTrailerFromDB) - Database Updatewd !!!!");
        }
        
        
        public static void SearchTrailerLocaltemp(int select_item, int GetID, GUIAnimation m_SearchAnimation)
        {
            setProcessAnimationStatus(true, m_SearchAnimation);
            // search all (Trailer)files 
            ArrayList newItems = new ArrayList();
            bool NoResumeMovie = true;
            int IMovieIndex = 0;

            //(int)MesFilms.conf.StrIndex)
            //(MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString()
            Log.Debug("SearchTrailerLocal - SelectedItemInfo from (MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString(): '" + (MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.StrTitle1].ToString() + "'"));
            Log.Debug("SearchTrailerLocal - SelectedItemInfo from (MesFilms.r[MesFilms.conf.StrIndex].ToString(): '" + (MesFilms.r[MesFilms.conf.StrIndex].ToString() + "'"));

            Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex, true);
            //Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex, false);
            if (newItems.Count > 20)
            // Maximum 20 entries (limitation for MP dialogFileStacking)
            {
                    GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                    dlgOk.SetHeading(GUILocalizeStrings.Get(107986));//my films
                    dlgOk.SetLine(1, MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()].ToString());//video title
                    dlgOk.SetLine(2, "maximum 20 entries for the playlist");
                    dlgOk.DoModal(GetID);
                    Log.Info("MyFilms: Too many entries found for movie '" + MesFilms.r[select_item][MesFilms.conf.StrSTitle.ToString()] + "', number of entries found = " + newItems.Count.ToString());
                return;
            }
            setProcessAnimationStatus(false, m_SearchAnimation);
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
                    newitem.Type = PlayListItem.PlayListItemType.Video;
                    playlist.Add(newitem);
                }
                // ask for start movie Index

                // play movie...
                PlayMovieFromPlayList(NoResumeMovie, IMovieIndex - 1);
            }

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
            using (Settings xmlreader = new MPSettings())
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

                                Log.Debug("MyFilms (HandleWakeUpNas) : WOL - GetHostAddresses({0}) returns:", hostName);

                                foreach (IPAddress ip in ips)
                                {
                                    Log.Debug("    {0}", ip);
                                }

                                //Use first valid IP address
                                ipAddress = ips[0];
                            }
                            catch (Exception ex)
                            {
                                Log.Error("MyFilms (HandleWakeUpNas) : WOL - Failed GetHostAddress - {0}", ex.Message);
                            }
                        }

                        //Check for valid IP address
                        if (ipAddress != null)
                        {
                            //Update the MAC address if possible
                            hwAddress = wakeOnLanManager.GetHardwareAddress(ipAddress);

                            if (wakeOnLanManager.IsValidEthernetAddress(hwAddress))
                            {
                                Log.Debug("MyFilms (HandleWakeUpNas) : WOL - Valid auto MAC address: {0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}"
                                          , hwAddress[0], hwAddress[1], hwAddress[2], hwAddress[3], hwAddress[4], hwAddress[5]);

                                //Store MAC address
                                macAddress = BitConverter.ToString(hwAddress).Replace("-", ":");

                                Log.Debug("MyFilms (HandleWakeUpNas) : WOL - Store MAC address: {0}", macAddress);

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
                    using (Settings xmlreader = new MPSettings())
                    {
                        macAddress = xmlreader.GetValueAsString("nas", "macAddress", null);
                    }

                    Log.Debug("MyFilms (HandleWakeUpNas) : WOL - Use stored MAC address: {0}", macAddress);

                    try
                    {
                        hwAddress = wakeOnLanManager.GetHwAddrBytes(macAddress);

                        //Finally, start up the TV server
                        Log.Info("MyFilms (HandleWakeUpNas) : WOL - Start the NAS server");

                        if (wakeOnLanManager.WakeupSystem(hwAddress, hostName, intTimeOut))
                        {
                            Log.Info("MyFilms (HandleWakeUpNas) : WOL - The NAS server started successfully!");
                        }
                        else
                        {
                            Log.Error("MyFilms (HandleWakeUpNas) : WOL - Failed to start the NAS server");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("MyFilms (HandleWakeUpNas) : WOL - Failed to start the NAS server - {0}", ex.Message);
                    }
                }
            }
        }

        //private void ShowMessageDialog(string headline, string line1, string line2)
        //{
        //    GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
        //    if (dlgOK != null)
        //    {
        //        dlgOK.SetHeading(headline);
        //        dlgOK.SetLine(1, line1);
        //        dlgOK.SetLine(2, line2);
        //        dlgOK.DoModal(GetID);
        //        return;
        //    }
        //}

    }

}
        #endregion
