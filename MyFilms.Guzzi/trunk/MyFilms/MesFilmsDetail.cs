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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Cornerstone.MP;
using Grabber;
using MediaPortal.Configuration;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Playlists;
using MediaPortal.Util;
using MediaPortal.Video.Database;
using MesFilms.MyFilms;
using NewStringLib;
using SQLite.NET;

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
            CTRL_BtnPlay1 = 10001,
            CTRL_BtnPlay2 = 10002,
            CTRL_BtnPlay3 = 10003,
            CTRL_BtnPlay4 = 10004,
            CTRL_BtnPlay5 = 10005,
            CTRL_BtnReturn = 102,
            CTRL_BtnNext = 103,
            CTRL_BtnPrior = 104,
            CTRL_BtnLast = 105,
            CTRL_BtnFirst = 106,
            CTRL_BtnMaj = 107,
            CTRL_BtnActors = 108,
            CTRL_BtnPlayTrailer = 109,
            CTRL_Fanart = 1000,
            CTRL_FanartDir = 1001,
            CTRL_logos_id2001 = 2001,
            CTRL_logos_id2002 = 2002,
            CTRL_Title = 2025,
            CTRL_OTitle = 2026,
            CTRL_lblGenre = 2032,
            CTRL_Genre = 2062,
            CTRL_Format = 2069,
            CTRL_ImgDD = 2072,
        }
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
        static string wzone = null;
        int StrMax = 0;
        public const int ID_MesFilms = 7986;
        public int ID_MesFilmsDetail = 7987;
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
                    if (iControl == (int)Controls.CTRL_BtnPlay1)
                    // Search File to play
                    {
                        Launch_Movie(MesFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay2)
                    // Search File to play
                    {
                        Launch_Movie(MesFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay3)
                    // Search File to play
                    {
                        Launch_Movie(MesFilms.conf.StrIndex, GetID, m_SearchAnimation);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_BtnPlay4)
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
                    if (iControl == (int)Controls.CTRL_BtnPlayTrailer)
                    // Search File to play
                    {
                        Log.Debug("MyFilmsDetails (OnAtion-CTRL_BtnPlaytrailer: MesFilms.conf.StrIndex '" + MesFilms.conf.StrIndex + "'"); 
                        Launch_Movie_Trailer(MesFilms.conf.StrIndex, GetID, m_SearchAnimation);
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
                        // Update items
                        Update_XML_Items();
                        return true;
                    if (iControl == (int)Controls.CTRL_BtnActors)
                        // Show Actror Details Screen
                        Update_XML_Items(); //To me changed, when DetailScreen is done!!!
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
            if (MesFilms.conf.StrGrabber == true)
            {
                dlg.Add(GUILocalizeStrings.Get(5910));        //Update Internet Movie Details
                upd_choice[ichoice] = "grabber";
                ichoice++;
            }
            if (MesFilms.conf.StrFanart == true)            // Download Fanart
            {
                dlg.Add(GUILocalizeStrings.Get(1079862));
                upd_choice[ichoice] = "fanart";
                ichoice++;
            }
            if (MesFilms.conf.StrFanart == true)            // Remove Fanart
            {
                dlg.Add(GUILocalizeStrings.Get(1079874));
                upd_choice[ichoice] = "deletefanart";
                ichoice++;
            }

            dlg.DoModal(GetID);
            if (dlg.SelectedLabel == -1) return;
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            if (null == keyboard) return;
            keyboard.Reset();

            ItemID = (int)MesFilms.r[MesFilms.conf.StrIndex]["Number"]; //set unique id num (ant allows it to be non-unique but that is a bad idea)
            //May wish to completely re-load the dataset before updating any fields if used in multi-user system, but would req concurrency locks etc so...
            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
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
                        MesFilms.r[MesFilms.conf.StrIndex][StrUpdItem1] = keyboard.Text.ToString();
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
                        MesFilms.r[MesFilms.conf.StrIndex][StrUpdItem2] = keyboard.Text.ToString();
                        Update_XML_database();
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
                            MesFilms.r = BaseMesFilms.LectureDonn�es(MesFilms.conf.StrDfltSelect, MesFilms.conf.StrFilmSelect, MesFilms.conf.StrSorta, MesFilms.conf.StrSortSens);
                            afficher_detail(true);

                        }
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
                        MesFilmsDetail.Download_Backdrops_Fanart(wtitle, wttitle, wdirector.ToString(), wyear.ToString(), true, GetID);

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
                for (int i = 0; i < newItems.Count; i++)
                {
                    // for each entry test if it's a file, a directory or a dvd copy
                    // no action for files on amovible media or image files
                    if (System.IO.File.Exists(newItems[i].ToString()))
                        try
                        {
                            System.IO.File.Delete(newItems[i].ToString());
                            Log.Info("MyFilms : file deleted : " + newItems[i].ToString());
                        }
                        catch
                        {
                            Log.Info("MyFilms : unable to delete file : " + newItems[i].ToString());
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
            string title =  string.Empty;
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
                    Remove_Backdrops_Fanart(wtitle,true);
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
                    wtitle = wtitle.Substring(wtitle.LastIndexOf(MesFilms.conf.TitleDelim) +1);
                if (wtitle != ttitle)
                    Remove_Backdrops_Fanart(wtitle,true);
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
                System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(title, ttitle, (int)year, director, MesFilms.conf.StrPathFanart, true,false);
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
            System.Collections.Generic.List<grabber.DBMovieInfo> listemovies = Grab.GetFanart(wtitle, wttitle, wyear, director, MesFilms.conf.StrPathFanart, true, choose);
            int listCount = listemovies.Count;
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
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(GUILocalizeStrings.Get(924)); // menu
                    dlg.Add("  *****  " + GUILocalizeStrings.Get(200036) + "  *****  "); //choice for changing movie filename
                    for (int i = 0; i < listemovies.Count; i++)
                    {
//                        dlg.Add(listemovies[i].Name + "  (" + listemovies[i].Year + ") ");
                    }
                    if (!(dlg.SelectedLabel > -1))
                    {
                        dlg.SelectedLabel = -1;
                        dlg.DoModal(wGetID);
                    }
                    if (dlg.SelectedLabel == 0)
                    {
                        VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                        if (null == keyboard) return;
                        keyboard.Reset();
                        keyboard.Text = wtitle;
                        keyboard.DoModal(wGetID);
                        if ((keyboard.IsConfirmed) && (keyboard.Text.Length > 0))
                        {
                            Remove_Backdrops_Fanart(wttitle,true);
                            Remove_Backdrops_Fanart(wtitle, true);
                            Download_Backdrops_Fanart(keyboard.Text, wttitle, string.Empty, string.Empty, true, wGetID);
                        }
                        break;
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
        //                          rep = true if the selected Item is a grouped Item (henre, year...) on main screen
        //                          filecover = name of the file cover for using 'Default Cover for missing Fanart'
        //      value returned string[2]
        //                          [0] = file or directory found (if not " ")
        //                          [1] = file or dir
        //-------------------------------------------------------------------------------------------
        public static string[] Search_Fanart(string wtitle2, bool main, string searched, bool rep, string filecover, string group)
        {
            string[] wfanart = new string[2];
            wfanart[0] = " ";
            wfanart[1] = " ";
            if (MesFilms.conf.StrFanart)
            {
                if (wtitle2.Contains(MesFilms.conf.TitleDelim))
                    wtitle2 = wtitle2.Substring(wtitle2.LastIndexOf(MesFilms.conf.TitleDelim) + 1).Trim();
                wtitle2 = Grabber.GrabUtil.CreateFilename(wtitle2.ToLower()).Replace(' ', '.');
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
                FileInfo wfile = new FileInfo(safeName + "\\{" + wtitle2 + "}.jpg");
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
                }
            }
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
            GUIPropertyManager.SetProperty("#myfilms.picture", file);

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
            
            ImageSwapper backdrop = new ImageSwapper();
            string[] wfanart = new string[2];
            string wtitle = MesFilms.r[MesFilms.conf.StrIndex]["OriginalTitle"].ToString();
            if (MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"] != null && MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString().Length > 0)
                wtitle = MesFilms.r[MesFilms.conf.StrIndex]["TranslatedTitle"].ToString();
            if (ImgFanartDir != null)
                wfanart = Search_Fanart(wtitle, false, "dir", false, file, string.Empty);
            else
                wfanart = Search_Fanart(wtitle, false, "file", false, file, string.Empty);

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
                Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
            }
            
            GUIPropertyManager.SetProperty("#myfilms.aspectratio", wstring);
            Log.Debug("MyFilms : Property loaded #myfilms.aspectratio with " + wstring);
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
                                    wstring = MesFilms.r[ItemId]["Actors"].ToString().Replace('|', '\n');
                            GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                            //Guzzi: Temporarily disabled
                            // Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                            break;
                        case "description":
                        case "comments":
                            if (wrep)
                                if (MesFilms.r[ItemId][dc.ColumnName].ToString().Length > 0)
                                {
                                    wstring = System.Web.HttpUtility.HtmlEncode(MesFilms.r[ItemId][dc.ColumnName].ToString().Replace('�', '\''));
                                    wstring = wstring.ToString().Replace('|', '\n');
                                    wstring = wstring.ToString().Replace('�', '.');
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
                            if ((wrep) && (MesFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                                wstring = MesFilms.r[ItemId][dc.ColumnName].ToString();
                            Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring); 
                            GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                            //Log.Debug("MyFilms (Load_Detailed_DB): Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring);
                            decimal aspectratio;
                            decimal w_hsize;
                            decimal w_vsize;
                            string[] arSplit;
                            string[] Sep = new string[] { "x" };
                            arSplit = MesFilms.r[ItemId][dc.ColumnName].ToString().Split(Sep, StringSplitOptions.RemoveEmptyEntries); // remove entries empty // StringSplitOptions.None);//will add "" entries also
                            w_hsize = (decimal)Convert.ToInt32(arSplit[0]);
                            w_vsize = (decimal)Convert.ToInt32(arSplit[1]);
                            aspectratio = (w_hsize / w_vsize);
                            aspectratio = Math.Round(aspectratio, 2);
                            wstring = aspectratio.ToString();
                            GUIPropertyManager.SetProperty("#myfilms.aspectratio with " + aspectratio, wstring);
                            Log.Debug("MyFilms : Property loaded #myfilms.aspectratio with " + wstring); 
                            //Log.Debug("MyFilms (Load_Detailed_DB): Split for aspectratio: '" + (arSplit[0]) + "', '" + (arSplit[1]) + "' --> '" + wstring + "'");
                            break;
                        default:
                            if ((wrep) && (MesFilms.r[ItemId][dc.ColumnName].ToString().Length > 0))
                                wstring = MesFilms.r[ItemId][dc.ColumnName].ToString();
                            GUIPropertyManager.SetProperty("#myfilms." + dc.ColumnName.ToLower(), wstring);
                            Log.Debug("MyFilms : Property loaded #myfilms." + dc.ColumnName.ToLower() + " with " + wstring); 
                            break;
                    }
                }
            }
        }
        #endregion
        #region  Lecture du film demand�

        public static void Launch_Movie(int select_item, int GetID, GUIAnimation m_SearchAnimation)
        //-------------------------------------------------------------------------------------------
        // Play Movie
        //-------------------------------------------------------------------------------------------
        {
            // Run externaly Program before Playing if defined in setup
            setProcessAnimationStatus(true, m_SearchAnimation);
            if ((MesFilms.conf.CmdPar.Length > 0) && (MesFilms.conf.CmdPar != "(none)"))
                RunProgram(MesFilms.conf.CmdExe, MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.CmdPar].ToString());
            if (g_Player.Playing)
                g_Player.Stop();
            // search all files
            ArrayList newItems = new ArrayList();
            bool NoResumeMovie = true;
            int IMovieIndex = 0;

            Search_All_Files(select_item, false, ref NoResumeMovie, ref newItems, ref IMovieIndex, false);
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

                for (int i = 0; i < newItems.Count; ++i)
                {
                    string movieFileName = (string)newItems[i];
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
                    dlgYesNo.SetLine(2, "' disc n� = " + MesFilms.r[select_item]["Number"].ToString());//ANT Number for Identification Media 
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
            if ((MesFilms.conf.CmdPar.Length > 0) && (MesFilms.conf.CmdPar != "(none)"))
                RunProgram(MesFilms.conf.CmdExe, MesFilms.r[MesFilms.conf.StrIndex][MesFilms.conf.CmdPar].ToString());
            if (g_Player.Playing)
                g_Player.Stop();
            // search all files
            ArrayList newItems = new ArrayList();
            bool NoResumeMovie = true;
            int IMovieIndex = 0;

            Log.Debug("MyFilmsDetails (Launch_Movie_Trailer): new do Moviesearch with '" + select_item + "' (Selected_Item"); 
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

                for (int i = 0; i < newItems.Count; ++i)
                {
                    string movieFileName = (string)newItems[i];
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
                    dlgYesNo.SetLine(2, "' disc n� = " + MesFilms.r[select_item]["Number"].ToString());//ANT Number for Identification Media 
                dlgYesNo.DoModal(GetID);
                if (dlgYesNo.IsConfirmed)
                    Launch_Movie(select_item, GetID, m_SearchAnimation);
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
            // update the MP Video Databse for OSD view during playing
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
                DataRow[] r1 = BaseMesFilms.LectureDonn�es(MesFilms.conf.StrPlayedDfltSelect, MesFilms.conf.StrPlayedSelect, MesFilms.conf.StrPlayedSort, MesFilms.conf.StrPlayedSens);
                // Handle all movie files from idMovie
                ArrayList movies = new ArrayList();
                int iidMovie = VideoDatabase.GetMovieId(filename);
                if (iidMovie >= 0)
                {
                    VideoDatabase.GetFiles(iidMovie, ref movies);
                    if (movies.Count <= 0)
                        return;
                    for (int i = 0; i < movies.Count; i++)
                    {
                        string strFilePath = (string)movies[i];
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
            for (int i = 0; i < playlist.Count; ++i)
            {
                PlayListItem playListItem = playlist[i];
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
                string wdescription = System.Web.HttpUtility.HtmlEncode(MesFilms.r[select_item]["Description"].ToString().Replace('�', '\''));
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
                string strThumb = MediaPortal.Util.Utils.GetCoverArtName(Thumbs.MovieTitle, movieDetails.Title);
                string LargeThumb = MediaPortal.Util.Utils.GetLargeCoverArtName(Thumbs.MovieTitle, movieDetails.Title);

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
            for (int wi = 0; wi < w_tableau.Count; wi++)
            {
                update_fieldDB(w_tableau[wi].ToString(), iidmovie, field);
            }
        }

        private static void Search_All_Files(int select_item, bool delete,ref bool NoResumeMovie, ref ArrayList newItems, ref int IMovieIndex, bool Trailer)
        {
            string fileName = string.Empty;
            string[] split1;
            string[] split2;
            string[] split3;
            Log.Debug("MyFilmsDetails (Search_All_Files) - StrDirStor: " + MesFilms.conf.StrDirStor);
            string strDir = MesFilms.conf.StrDirStor;
            int IdMovie = -1;
            int timeMovieStopped = 0;
                // retrieve filename information stored in the DB
            Log.Debug("MyFilmsDetails (Search_All_Files) - try filename MesFilms.r[select_item][MesFilms.conf.StrStorage]: '" + (string)MesFilms.r[select_item][MesFilms.conf.StrStorage].ToString().Trim() + "' - ConfStorage: '" + MesFilms.conf.StrStorage + "'");
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
                    // detect if fisrt part is an iso file
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
                            //On retourne une liste d'informations sur les fichiers contenus dans le r�pertoire
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

            SearchFiles(file, storage, false);

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
        /// <param name="fileName">le nom du fichier recherch�</param>
        /// <param name="path">le chemin du r�pertoire dans lequel chercher</param>
        /// <param name="recur">sp�cifie s'il s'agit d'une relance recursive</param>
        public static void SearchFiles(string fileName, string searchrep, bool recur)
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
                    //On retourne une liste d'informations sur les fichiers contenus dans le r�pertoire
                    FileSystemInfo[] filesInf = dirsInf.GetFileSystemInfos();

                    //Si le nom d'un fichier correspond avec le nom du fichier recherch� 
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
                    //Si le parametre SearchSubDirs vaut true on r�alise une 
                    //recherche r�cursive sur tous les sous-r�pertoires
                    if (MesFilms.conf.SearchSubDirs == "no") continue;
                    foreach (DirectoryInfo dir in dirsInf.GetDirectories())
                    {
                        //On rappelle la m�thode SearchFiles pour tous les sous-r�pertoires  
                        SearchFiles(file, dir.FullName, true);
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
                    if ((d.DriveType.ToString() == "CDRom") && (d.IsReady == true))
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
            file = file.Replace("�", "[e�]");
            file = file.Replace("�", "[e�]");
            file = file.Replace("�", "[e�]");
            file = file.Replace("�", "[e�]");
            file = file.Replace("�", "[o�]");
            file = file.Replace("�", "[o�]");
            file = file.Replace("�", "[a�]");
            file = file.Replace("�", "[a�]");
            file = file.Replace("�", "[a�]");
            file = file.Replace("�", "[u�]");
            file = file.Replace("�", "[u�]");
            file = file.Replace("�", "[u�]");
            file = file.Replace("�", "[i�]");
            file = file.Replace("�", "[i�]");
            file = file.Replace("�", "[c�]");
            return file;
        }
        static public void RunProgram(string exeName, string argsLine)
        {
            if (exeName.Length > 0)
            {
                ProcessStartInfo psI = new ProcessStartInfo(exeName, argsLine);
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
    }

}
        #endregion
