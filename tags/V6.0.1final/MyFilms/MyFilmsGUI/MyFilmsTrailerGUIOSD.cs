using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Video;
using MediaPortal.Player;
using MediaPortal.Player.PostProcessing;
using Action = System.Action;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  public class MyFilmsTrailerGUIOSD : GUIVideoOSD
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    public override int GetID
    {
      get { return MyFilms.ID_MyFilmsTrailerOSD; }
      set { }
    }

    public override string GetModuleName()
    {
      return "MyFilms Trailer OSD";
    }

    public override bool Init()
    {
      bool bResult = Load(GUIGraphicsContext.Skin + @"\MyFilmsTrailerOSD.xml");
      return bResult;
    }

    public override void OnAction(MediaPortal.GUI.Library.Action action)
    {
      if (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_CONTEXT_MENU)
      {
        var videoWindow = (MyFilmsGUIVideoFullscreen)GUIWindowManager.GetWindow(29054);
        videoWindow.OnAction(new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.ACTION_SHOW_OSD, 0, 0));
        videoWindow.OnAction(action);
      }
      else
      {
        base.OnAction(action);
      }
    }

    public override bool OnMessage(GUIMessage message)
    {
      if (message.Message == GUIMessage.MessageType.GUI_MSG_WINDOW_INIT)
      {
        GUIPropertyManager.SetProperty("#currentmodule", GetModuleName());
        AllocResources();
        // if (g_application.m_pPlayer) g_application.m_pPlayer.ShowOSD(false);
        // ResetAllControls(); // make sure the controls are positioned relevant to the OSD Y offset
        FocusControl(GetID, 213, 0); // set focus to play button by default when window is shown
        QueueAnimation(AnimationType.WindowOpen);
        ToggleButton(213, g_Player.Paused);
        return true;
      }
      return base.OnMessage(message);
    }

    private void FocusControl(int dwSenderId, int dwControlID, int dwParam)
    {
      var msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SETFOCUS, GetID, dwSenderId, dwControlID, dwParam, 0, null);
      OnMessage(msg);
    }

    private void ToggleButton(int iButtonId, bool bSelected)
    {
      var pControl = (GUIControl)GetControl(iButtonId);

      if (pControl != null)
      {
        if (bSelected) // do we want the button to appear down?
        {
          var msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SELECTED, GetID, 0, iButtonId, 0, 0, null);
          OnMessage(msg);
        }
        else // or appear up?
        {
          var msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_DESELECTED, GetID, 0, iButtonId, 0, 0, null);
          OnMessage(msg);
        }
      }
    }
  }
}
