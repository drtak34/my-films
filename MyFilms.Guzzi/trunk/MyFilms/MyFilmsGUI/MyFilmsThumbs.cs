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

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  using MediaPortal.Configuration;
  using MediaPortal.GUI.Library;

  using MyFilmsPlugin.MyFilms;

  /// <summary>
    /// Opens a separate page to display Movie Thumbs
    /// </summary>
    public class MyFilmsThumbs : GUIWindow
    {
        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

        #region Skin ID descriptions

        enum Controls : int
        {
            CTRL_TxtSelect = 10101,
            CTRL_BtnReturn = 10102,
            CTRL_Fanart = 1000,
            CTRL_FanartDir = 1001,
            CTRL_MovieThumb1 = 10301,
            CTRL_MovieThumb2 = 10302,
            CTRL_MovieThumb3 = 10303,
            CTRL_MovieThumb4 = 10304,
            CTRL_MovieThumb5 = 10305,
            CTRL_MovieThumb6 = 10306,
            CTRL_MovieThumb7 = 10307,
            CTRL_MovieThumb8 = 10308,
            CTRL_MovieThumb9 = 10309,
            CTRL_MovieThumb10 = 10310,
            CTRL_MovieThumb11 = 10311,
            CTRL_MovieThumb12 = 10312,
            CTRL_MovieThumb13 = 10313,
            CTRL_MovieThumb14 = 10314,
            CTRL_MovieThumb15 = 10315,
            CTRL_MovieThumb16 = 10316
        }
      
        [SkinControl(10101)]
        protected GUIButtonControl CTRL_TxtSelect = null;
        [SkinControl(10102)]
        protected GUISortButtonControl CTRL_BtnReturn = null;

        [SkinControlAttribute((int)Controls.CTRL_Fanart)]
        protected GUIImage ImgFanart = null;
        [SkinControlAttribute((int)Controls.CTRL_FanartDir)]
        protected GUIMultiImage ImgFanartDir = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb1)]
        protected GUIImage ImgThumb1 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb2)]
        protected GUIImage ImgThumb2 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb3)]
        protected GUIImage ImgThumb3 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb4)]
        protected GUIImage ImgThumb4 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb5)]
        protected GUIImage ImgThumb5 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb6)]
        protected GUIImage ImgThumb6 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb7)]
        protected GUIImage ImgThumb7 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb8)]
        protected GUIImage ImgThumb8 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb9)]
        protected GUIImage ImgThumb9 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb10)]
        protected GUIImage ImgThumb10 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb11)]
        protected GUIImage ImgThumb11 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb12)]
        protected GUIImage ImgThumb12 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb13)]
        protected GUIImage ImgThumb13 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb14)]
        protected GUIImage ImgThumb14 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb15)]
        protected GUIImage ImgThumb15 = null;
        [SkinControlAttribute((int)Controls.CTRL_MovieThumb16)]
        protected GUIImage ImgThumb16 = null;
        
        #endregion



        #region Base Dialog Variables

        #endregion


        public MyFilmsThumbs()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public override int GetID
        {
            get { return MyFilms.ID_MyFilmsThumbs; }
            set { base.GetID = value; }
        }

        public override string GetModuleName()
        {
          return GUILocalizeStrings.Get(MyFilms.ID_MyFilmsThumbs); // return localized string for Modul ID
        }

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\MyFilmsThumbs.xml");
        }

        public override void PreInit() { }

        #region Action
        //---------------------------------------------------------------------------------------
        //   Handle Keyboard Actions
        //---------------------------------------------------------------------------------------
        public override void OnAction(MediaPortal.GUI.Library.Action actionType)
        {
            LogMyFilms.Debug("MyFilmsThumbs: OnAction " + actionType.wID.ToString());
            if ((actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) || (actionType.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PARENT_DIR))
            {
                MyFilms.conf.LastID = MyFilms.ID_MyFilmsDetail;
                GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilmsDetail);
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
            string moviename;
            string directoryname;
            switch (messageType.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
                    //---------------------------------------------------------------------------------------
                    // Windows Init
                    //---------------------------------------------------------------------------------------
                    base.OnMessage(messageType);
                    LogMyFilms.Debug("MyFilmsThumbs: CurrentMovie: '" + MyFilms.CurrentMovie + "'");

                    //Retrieve original directory of mediafiles
                    //directoryname
                    moviename = MyFilms.CurrentMovie.Substring(MyFilms.CurrentMovie.LastIndexOf(";") + 1);
                    LogMyFilms.Debug("MyFilmsThumbs (GetThumbDirectory) splits media directory name: '" + moviename.ToString() + "'");

                    try
                    { directoryname = System.IO.Path.GetDirectoryName(moviename); }
                    catch
                    { directoryname = string.Empty; }

                    LogMyFilms.Debug("MyFilmsThumbs (GetThumbDirectory) get thumb directory name: '" + directoryname.ToString() + "'");
                    
                    LoadThumbs(directoryname);
                    LogMyFilms.Debug("MyFilmsThumbs: PropertyLoaded !");
                    MyFilms.conf.LastID = MyFilms.ID_MyFilmsThumbs;
                    return true;

                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT: //called when exiting plugin either by prev menu or pressing home button
                    if (global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig != string.Empty)
                        global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.SaveConfiguration(global::MyFilmsPlugin.MyFilms.MyFilmsGUI.Configuration.CurrentConfig, MyFilms.conf.StrIndex, MyFilms.conf.StrTIndex);
                    using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
                    {
                        string currentmoduleid = "7990";
                        bool currentmodulefullscreen = (GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_TVFULLSCREEN || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_MUSIC || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO || GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_TELETEXT);
                        string currentmodulefullscreenstate = GUIPropertyManager.GetProperty("#currentmodulefullscreenstate");
                        // if MP was closed/hibernated by the use of remote control, we have to retrieve the fullscreen state in an alternative manner.
                        if (!currentmodulefullscreen && currentmodulefullscreenstate == "True")
                            currentmodulefullscreen = true;
                        xmlreader.SetValue("general", "lastactivemodule", currentmoduleid);
                        xmlreader.SetValueAsBool("general", "lastactivemodulefullscreen", currentmodulefullscreen);
                        LogMyFilms.Debug("MyFilms : SaveLastActiveModule - module {0}", currentmoduleid);
                        LogMyFilms.Debug("MyFilms : SaveLastActiveModule - fullscreen {0}", currentmodulefullscreen);
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
                        MyFilms.conf.LastID = MyFilms.ID_MyFilms;
                        GUITextureManager.CleanupThumbs();
                        GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilms);
                        return true;
                    }
                    if (iControl == (int)Controls.CTRL_TxtSelect)
                        GUIWindowManager.ActivateWindow(MyFilms.ID_MyFilmsActors);
                    GUIWindowManager.ShowPreviousWindow();
                    return true;

            }
            base.OnMessage(messageType);
            return true;
        }


        #endregion



        protected override void OnPageLoad()
        {
        }

        protected override void OnPageDestroy(int newWindowId)
        {
        }


        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
        {
        }

        protected void LoadThumbs(string MovieThumbPath)
        {
            MovieThumbPath = MovieThumbPath + "\\extrathumbs\\";
            string defaultthumb = "\\\\xvs-gmi-fs\\media-server\\AMC-Appl\\AMC-DefaultCover\\videonotavailable.jpg";
            LogMyFilms.Debug("MyFilms (LoadThumbs) : Set default Thumb: '" + defaultthumb + "'");
            
            //string strDir = MyFilms.conf.StrDirStorActorThumbs;

            string thumb1 = MovieThumbPath + "thumb1.jpg";
            LogMyFilms.Debug("thumb1: '" + thumb1 + "'");
            if (!System.IO.File.Exists(thumb1))
                thumb1 = defaultthumb;
            ImgThumb1.SetFileName(thumb1);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb1", thumb1);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb1);
            LogMyFilms.Debug("thumb1: '" + thumb1 + "'");

            string thumb2 = MovieThumbPath + "thumb2.jpg";
            if (!System.IO.File.Exists(thumb2))
                thumb2 = defaultthumb;
            ImgThumb2.SetFileName(thumb2);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb2", thumb2);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb2);

            string thumb3 = MovieThumbPath + "thumb3.jpg";
            if (!System.IO.File.Exists(thumb3))
                thumb3 = defaultthumb;
            ImgThumb3.SetFileName(thumb3);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb3", thumb3);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb3);

            string thumb4 = MovieThumbPath + "thumb4.jpg";
            if (!System.IO.File.Exists(thumb4))
                thumb4 = defaultthumb;
            ImgThumb4.SetFileName(thumb4);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb4", thumb4);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb4);

            string thumb5 = MovieThumbPath + "thumb5.jpg";
            if (!System.IO.File.Exists(thumb5))
                thumb5 = defaultthumb;
            ImgThumb5.SetFileName(thumb5);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb5", thumb5);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb5);

            string thumb6 = MovieThumbPath + "thumb6.jpg";
            if (!System.IO.File.Exists(thumb6))
                thumb6 = defaultthumb;
            ImgThumb6.SetFileName(thumb6);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb6", thumb6);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb6);

            string thumb7 = MovieThumbPath + "thumb7.jpg";
            if (!System.IO.File.Exists(thumb7))
                thumb7 = defaultthumb;
            ImgThumb7.SetFileName(thumb7);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb7", thumb7);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb7);

            string thumb8 = MovieThumbPath + "thumb8.jpg";
            if (!System.IO.File.Exists(thumb8))
                thumb8 = defaultthumb;
            ImgThumb8.SetFileName(thumb8);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb8", thumb8);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb8);

            string thumb9 = MovieThumbPath + "thumb9.jpg";
            if (!System.IO.File.Exists(thumb9))
                thumb9 = defaultthumb;
            ImgThumb9.SetFileName(thumb9);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb9", thumb9);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb9);

            string thumb10 = MovieThumbPath + "thumb10.jpg";
            if (!System.IO.File.Exists(thumb10))
                thumb10 = defaultthumb;
            ImgThumb10.SetFileName(thumb10);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb10", thumb10);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb10);

            string thumb11 = MovieThumbPath + "thumb11.jpg";
            if (!System.IO.File.Exists(thumb11))
                thumb11 = defaultthumb;
            ImgThumb11.SetFileName(thumb11);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb11", thumb11);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb11);

            string thumb12 = MovieThumbPath + "thumb12.jpg";
            if (!System.IO.File.Exists(thumb12))
                thumb12 = defaultthumb;
            ImgThumb12.SetFileName(thumb12);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb12", thumb12);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb12);

            string thumb13 = MovieThumbPath + "thumb13.jpg";
            if (!System.IO.File.Exists(thumb13))
                thumb13 = defaultthumb;
            ImgThumb13.SetFileName(thumb13);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb13", thumb13);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb13);

            string thumb14 = MovieThumbPath + "thumb14.jpg";
            if (!System.IO.File.Exists(thumb14))
                thumb14 = defaultthumb;
            ImgThumb14.SetFileName(thumb14);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb14", thumb14);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb14);

            string thumb15 = MovieThumbPath + "thumb15.jpg";
            if (!System.IO.File.Exists(thumb15))
                thumb15 = defaultthumb;
            ImgThumb15.SetFileName(thumb15);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb15", thumb15);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb15);

            string thumb16 = MovieThumbPath + "thumb16.jpg";
            if (!System.IO.File.Exists(thumb16))
                thumb16 = defaultthumb;
            ImgThumb16.SetFileName(thumb16);
            GUIPropertyManager.SetProperty("#myfilms.moviethumb16", thumb16);
            GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb16);
        }

        protected void ClearThumbs()
        {
            GUIPropertyManager.SetProperty("#myfilms.moviethumb1", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb2", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb3", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb4", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb5", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb6", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb7", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb8", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb9", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb10", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb11", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb12", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb13", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb14", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb15", " ");
            GUIPropertyManager.SetProperty("#myfilms.moviethumb16", " ");
        }

    }
}