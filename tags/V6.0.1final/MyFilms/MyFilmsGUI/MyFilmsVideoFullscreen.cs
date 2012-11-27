using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Video;
using MediaPortal.Util;
using Action = System.Action;
using MyFilmsPlugin.MyFilms;
using MyFilmsPlugin.MyFilms.Utils;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  public class MyFilmsGUIVideoFullscreen : GUIVideoFullscreen
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
    
    public override string GetModuleName()
    {
      return  "MyFilms Fullscreen";
    }

    public override int GetID { get { return MyFilms.ID_MyFilmsFullScreen; } set { } }

    public override bool Load(string skinFileName)
    {
      return base.Load(GUIGraphicsContext.Skin + @"\MyFilmsFullScreen.xml");
    }

    public override void OnAction(MediaPortal.GUI.Library.Action action)
    {
      if (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_VOLUME_UP ||
          action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_VOLUME_DOWN ||
          action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_VOLUME_MUTE)
      {
        // MediaPortal core sends this message to the Fullscreenwindow, we need to do it ourselves to make the Volume OSD show up
        base.OnAction(new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.ACTION_SHOW_VOLUME, 0, 0));
        return;
      }
      else if (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU)
      {
        OnShowContextMenu();
        return;
      }
      else
      {
        var translatedAction = new MediaPortal.GUI.Library.Action();
        if (ActionTranslator.GetAction((int) GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO, action.m_key,
                                       ref translatedAction))
        {
          if (translatedAction.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_SHOW_OSD)
          {
            GUIPropertyManager.SetProperty("#Youtube.fm.FullScreen.ShowTitle", "false");
            GUIPropertyManager.SetProperty("#Youtube.fm.FullScreen.ShowNextTitle", "false");
            base.OnAction(translatedAction);
            if (GUIWindowManager.VisibleOsd == GUIWindow.Window.WINDOW_OSD)
            {
              GUIWindowManager.VisibleOsd = (GUIWindow.Window) MyFilms.ID_MyFilmsTrailerOSD;
            }
            return;
          }
          if (translatedAction.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_ASPECT_RATIO)
          {
            base.OnAction(translatedAction);
            return;
          }
        }
      }
      if (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_ITEM || action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_NEXT_CHAPTER)
      {
        if (MyFilms.currentTrailerMoviesList.Count > 1)
        {
          // MyFilms.player.PlayNext();
          return;
        }
      }

      if (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREV_ITEM || action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREV_CHAPTER)
      {
        if (MyFilms.currentTrailerMoviesList.Count > 1)
        {
          // MyFilms.player.PlayPrevious();
          return;
        }
      }

      base.OnAction(action);
    }

    public override bool OnMessage(GUIMessage message)
    {
      bool result = base.OnMessage(message);

      if (message.Message == GUIMessage.MessageType.GUI_MSG_WINDOW_INIT)
      {
        var osd = (MyFilmsTrailerGUIOSD)GUIWindowManager.GetWindow(MyFilms.ID_MyFilmsTrailerOSD);
        typeof(GUIVideoFullscreen).InvokeMember("_osdWindow", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.SetField, null, this, new object[] { osd });
      }

      if (message.Message == GUIMessage.MessageType.GUI_MSG_PLAYBACK_STARTED && GUIWindowManager.ActiveWindow == GetID)
      {
        GUIGraphicsContext.IsFullScreenVideo = true;
      }
      return result;
    }

    protected override void OnShowContextMenu()
    {
      if (MyFilms.currentTrailerPlayingItem == null)
      {
        base.OnShowContextMenu();
        return;
      }
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
      if (dlg == null)
      {
        return;
      }
      dlg.Reset();
      dlg.SetHeading("menu"); // menu
      dlg.AddLocalizedString(941);
      dlg.AddLocalizedString(970);
      dlg.Add("Show Info");
      dlg.DoModal(GetID);
      if (dlg.SelectedId == -1) return;
      if (dlg.SelectedLabelText == "Show Info")
      {
        //MyFilmsGuiInfoEx scr = (MyFilmsGuiInfoEx)GUIWindowManager.GetWindow(29053);
        //scr.MyFilmsEntry = videoEntry;
        //GUIWindowManager.ActivateWindow(29053);
      }
      if (dlg.SelectedLabelText == "option")
      {
        try
        {
          // ToDo: Do something here
        }
        catch (Exception ex)
        {
          LogMyFilms.Debug("Error: " + ex.Message);
        }
      }
      
      if (dlg.SelectedLabelText == "Go to main movie details")
      {
        // 
      }

      if (dlg.SelectedId == 941)
      {
        ShowAspectRatioMenu();
      }
      if (dlg.SelectedId == 970)
      {
        GUIWindowManager.IsOsdVisible = false;
        GUIGraphicsContext.IsFullScreenVideo = false;
        GUIWindowManager.ShowPreviousWindow();
      }
    }

    private void ShowAspectRatioMenu()
    {
      var dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
      if (dlg == null)
      {
        return;
      }
      dlg.Reset();
      dlg.SetHeading(941); // Change aspect ratio
      dlg.AddLocalizedString(942); // Stretch
      dlg.AddLocalizedString(943); // Normal
      dlg.AddLocalizedString(944); // Original
      dlg.AddLocalizedString(945); // Letterbox
      dlg.AddLocalizedString(946); // Smart stretch
      dlg.AddLocalizedString(947); // Zoom
      dlg.AddLocalizedString(1190); //14:9

      // set the focus to currently used mode
      dlg.SelectedLabel = dlg.IndexOfItem(MediaPortal.Util.Utils.GetAspectRatioLocalizedString(GUIGraphicsContext.ARType));
      // show dialog and wait for result
//      _IsDialogVisible = true;
      dlg.DoModal(GetID);
//      _IsDialogVisible = false;

      if (dlg.SelectedId == -1)
      {
        return;
      }
      //_timeStatusShowTime = (DateTime.Now.Ticks / 10000);

      //string strStatus = "";

      GUIGraphicsContext.ARType = MediaPortal.Util.Utils.GetAspectRatioByLangID(dlg.SelectedId);
      //strStatus = GUILocalizeStrings.Get(dlg.SelectedId);

      //GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.LABEL_ROW1, 0, 0,
      //                                null);
      //msg.Label = strStatus;
      //OnMessage(msg);
    }
  }
}
