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
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

using MediaPortal.Configuration;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Util;
using MediaPortal.InputDevices;

using BDInfo;

namespace WindowPlugins.BluRayPlayerLauncher
{

  public enum Players
  {
    Arcsoft,
    Cyberlink
  }

  [PluginIcons("WindowPlugins.BluRayPlayerLauncher.Blu-ray.gif", "WindowPlugins.BluRayPlayerLauncher.Blu-ray_disabled.gif")]
  public class BluRayPlayerLauncherPlugin : GUIWindow, ISetupForm, IShowPlugin
  {
    const int PluginWindowId = 8080;

    int disc_count;

    Players _player;
    string _driveLetter;
    string _playerExecutable;
    string _pluginNameInMenu;
    string _mediatype;

    static bool _changeRefreshRate;

    private bool _startWithBasicHome = false;

    bool _restorePowerState;
    bool _showInMenu;
    bool _KeyboardTranslation;
    bool _ejectReceived;
    bool _minimizeDuringPlay;
    bool _suspendRenderingDuringPlay;
    bool _autoplayHDDVD;
    bool _autoplayBluray;
    bool _autoplayASK;
    bool _24hzRefreshRate;
    bool _refreshRateChangeSuccessful;

    IntPtr _ptrActivePSGuid = IntPtr.Zero; 

    Dictionary<string, TSPlaylistFile> PlaylistFiles = new Dictionary<string, TSPlaylistFile>();
    Dictionary<string, TSStreamClipFile> StreamClipFiles = new Dictionary<string, TSStreamClipFile>();
    Dictionary<string, TSStreamFile> StreamFiles = new Dictionary<string, TSStreamFile>();

    Process _playerProcess = null;

    [DllImport("powrprof.dll", EntryPoint = "PowerGetActiveScheme")]
    public static extern UInt32 PowerGetActiveScheme(IntPtr UserRootPowerKey, ref IntPtr SchemeGuid);

    [DllImport("powrprof.dll", EntryPoint = "PowerSetActiveScheme")]
    public static extern uint PowerSetActiveScheme([InAttribute()] IntPtr UserRootPowerKey,[InAttribute()] IntPtr SchemeGuid);

    /// <summary>
    /// Constructor
    /// </summary>
    public BluRayPlayerLauncherPlugin()
    {
      Log.Info("Blu-Ray Player Launcher: construct");
      GetID = PluginWindowId;

      using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
      {
        _player = (Players)xmlreader.GetValueAsInt("BluRayPlayerLauncher", "player", (int)Players.Arcsoft);

        _playerExecutable = xmlreader.GetValueAsString("BluRayPlayerLauncher", "executable", null);
        _minimizeDuringPlay = xmlreader.GetValueAsBool("BluRayPlayerLauncher", "minimizeDuringPlay", true);
        _suspendRenderingDuringPlay = xmlreader.GetValueAsBool("BluRayPlayerLauncher", "suspendRenderingDuringPlay", true);

        _restorePowerState = xmlreader.GetValueAsBool("BluRayPlayerLauncher", "restorePowerState", false);
        _showInMenu = xmlreader.GetValueAsBool("BluRayPlayerLauncher", "showInMenu", true);
        _KeyboardTranslation = xmlreader.GetValueAsBool("BluRayPlayerLauncher", "KeyboardTranslation", true);
        _pluginNameInMenu = xmlreader.GetValueAsString("BluRayPlayerLauncher", "pluginNameInMenu", "Blu-Ray Player Launcher");

        _autoplayHDDVD = xmlreader.GetValueAsBool("BluRayPlayerLauncher", "autoplayHDDVD", true);
        _autoplayBluray = xmlreader.GetValueAsBool("BluRayPlayerLauncher", "autoplayBluray", true);
        _autoplayASK = xmlreader.GetValueAsBool("BluRayPlayerLauncher", "autoplayASK", true);

        _changeRefreshRate = xmlreader.GetValueAsBool("BluRayPlayerLauncher", "changeRefreshRate", true);
        _24hzRefreshRate = xmlreader.GetValueAsBool("BluRayPlayerLauncher", "24hzRefreshRate", false);

        _startWithBasicHome = xmlreader.GetValueAsBool("general", "startbasichome", false);
        Log.Info("Blu-Ray Player Launcher: executable: {0}", _playerExecutable);
        Log.Info("Blu-Ray Player Launcher: minimizeDuringPlay: {0}", _minimizeDuringPlay);
        Log.Info("Blu-Ray Player Launcher: suspendRenderingDuringPlay: {0}", _suspendRenderingDuringPlay);
      }
    }
    
    // Invoke needs to be used so that previus view is changed from correct thread.
    private delegate void PlayerExitedInvoke();

    #region BaseWindow Members

    /// <summary>
    /// Init
    /// </summary>
    public override bool Init()
    {
      bool bResult = Load(GUIGraphicsContext.Skin + @"\BluRayPlayerLauncher.xml");
      GUIWindowManager.Receivers += OnGlobalMessage;
      return bResult;
    }

    //
    // Handle global messages
    //
    private void OnGlobalMessage(GUIMessage message)
    {
      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_BLURAY_DISK_INSERTED:
          {
            Log.Info("Blu-Ray Player Launcher: GUI_MSG_BLURAY_DISK_INSERTED");
            _driveLetter = message.Label;
            _mediatype = "Blu-Ray";

            string string_autoplay_bd = "Autoplay Blu-Ray";
            string string_playback_bd_disc = "Start Blu-Ray playback?";

            try
            {
                string_autoplay_bd = LocalizeStrings.Get(4);
                string_playback_bd_disc = LocalizeStrings.Get(5);
            }
            catch
            {
                Log.Error("Blu-Ray Player Launcher: error accessing localization strings");
            }

            if (!_autoplayBluray)
            {
              Log.Info("Blu-Ray Player Launcher: Autoplay for Blu-ray is disabled");
            }

            if (_autoplayASK && _autoplayBluray)
            {
                GUIDialogYesNo dlg = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                dlg.SetHeading(string_autoplay_bd);
                dlg.SetLine(1, string_playback_bd_disc);
                dlg.SetDefaultToYes(true);
                dlg.DoModal(GUIWindowManager.ActiveWindow);

                if (dlg.IsConfirmed)
                {
                    GUIWindowManager.ActivateWindow(GetID); // continue with OnPageLoad()
                }
            }

            if (_autoplayBluray && !_autoplayASK)
            {
                GUIWindowManager.ActivateWindow(GetID);
            }

            _mediatype = null; // clear 
            _driveLetter = null; // clear
            break;
          }

        case GUIMessage.MessageType.GUI_MSG_HDDVD_DISK_INSERTED:
          {
              Log.Info("Blu-Ray Player Launcher: GUI_MSG_HDDVD_DISK_INSERTED");
              _driveLetter = message.Label;
              _mediatype = "HD-DVD";

              string string_autoplay_hddvd = "Autoplay HD-DVD";
              string string_playback_hddvd_disc = "Start HD-DVD playback?";

              try
              {
                  string_autoplay_hddvd = LocalizeStrings.Get(6);
                  string_playback_hddvd_disc = LocalizeStrings.Get(7);
              }
              catch
              {
                  Log.Error("Blu-Ray Player Launcher: error accessing localization strings");
              }


              if (!_autoplayHDDVD)
              {
                  Log.Info("Blu-Ray Player Launcher: Autoplay for HD-DVD is disabled");
              }

              if (_autoplayASK && _autoplayHDDVD)
              {
                  GUIDialogYesNo dlg = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                  dlg.SetHeading(string_autoplay_hddvd);
                  dlg.SetLine(1,string_playback_hddvd_disc);
                  dlg.SetDefaultToYes(true);
                  dlg.DoModal(GUIWindowManager.ActiveWindow);

                  if (dlg.IsConfirmed)
                  {
                      GUIWindowManager.ActivateWindow(GetID); // continue with OnPageLoad()
                  }
              }

              if (_autoplayHDDVD && !_autoplayASK)
              {
                  GUIWindowManager.ActivateWindow(GetID);
              }

              _mediatype = null; // clear 
              _driveLetter = null; // clear
              break;
          }

        case GUIMessage.MessageType.GUI_MSG_VOLUME_REMOVED:
          {
            Log.Info("Blu-Ray Player Launcher: GUI_MSG_VOLUME_REMOVED");
            if( _driveLetter == message.Label )
            {
              Log.Info("Blu-Ray Player Launcher: drive that received eject was our drive");
              _ejectReceived = true;
              StopPlayer();
              ResumeFromPlayback();
              RestoreRefreshRate();
            }
            break;
          }
       }
    }

    /// <summary>
    /// OnPageLoad
    /// </summary>
    protected override void OnPageLoad()
    {
        base.OnPageLoad();

        if (_mediatype != null && _driveLetter != null) // set because of autoplay 
        {
            Log.Info("Blu-Ray Player Launcher: begin autoplay");
            BeginPlayback();
        }
        else // plugin launched manually
        {
            Log.Info("Blu-Ray Player Launcher: launched manually");
            disc_count = 0;
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

            string[,] _disc = new string[20, 3];

            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.DriveType == System.IO.DriveType.CDRom)
                {
                    // search for BDMV Folder
                    if (Directory.Exists(drive.Name.Substring(0, 2) + "\\BDMV"))
                    {
                        disc_count++;
                        _disc[disc_count, 0] = "Blu-Ray";
                        _disc[disc_count, 1] = drive.VolumeLabel;
                        _disc[disc_count, 2] = drive.Name.Substring(0, 2);
                    }

                    // search for HVDVD Folder
                    else if (Directory.Exists(drive.Name.Substring(0, 2) + "\\HVDVD_TS"))
                    {
                        disc_count++;
                        _disc[disc_count, 0] = "HD-DVD";
                        _disc[disc_count, 1] = drive.VolumeLabel;
                        _disc[disc_count, 2] = drive.Name.Substring(0, 2);
                    }
                }
            }

            //localisation strings

            string string_nodisc_1 = "No disc found";
            string string_nodisc_2 = "No Blu-Ray or HD-DVD Disc found.";
            string string_nodisc_3 = "Start the Player anyway?";
            string string_select_disc_1 = "Select disc to play";
            string string_select_disc_2 = "in drive";

            try
            {
                string_nodisc_1 = LocalizeStrings.Get(8);
                string_nodisc_2 = LocalizeStrings.Get(9);
                string_nodisc_3 = LocalizeStrings.Get(10);
                string_select_disc_1 = LocalizeStrings.Get(11);
                string_select_disc_2 = LocalizeStrings.Get(12);
            }
            catch
            {
                Log.Error("Blu-Ray Player Launcher: error accessing localization strings");
            }

            if (disc_count == 0) // no Disc found
            {
                Log.Info("Blu-Ray Player Launcher: 0 HD-DVD/BD Disc(s) found, waiting for user selection.");
                GUIDialogYesNo dlg = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                dlg.SetHeading(string_nodisc_1);
                dlg.SetLine(1,string_nodisc_2);
                dlg.SetLine(2,string_nodisc_3);
                dlg.SetDefaultToYes(false);
                dlg.DoModal(GUIWindowManager.ActiveWindow);

                if (dlg.IsConfirmed)
                {
                    BeginPlayback();
                }
                else
                {
                    GUIWindowManager.ShowPreviousWindow();
                }
            }

            if (disc_count == 1) // one Disc found
            {
                _driveLetter = _disc[1, 2];
                _mediatype = _disc[1, 0];
                Log.Info("Blu-Ray Player Launcher: 1 HD-DVD/BD Disc(s) found, starting Playback.");
                BeginPlayback();
            }

            if (disc_count > 1) // more Discs found
            {
                Log.Info("Blu-Ray Player Launcher: {0} HD-DVD/BD Disc(s), showing drive selection Dialog.", disc_count);

                GUIDialogMenu dlgMenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlgMenu != null)
                {
                    dlgMenu.Reset();
                    dlgMenu.SetHeading(string_select_disc_1);
                    for (int i = 1; i <= disc_count; i++)
                    {
                        dlgMenu.Add(_disc[i, 0] + ": " + _disc[i, 1] + " " + string_select_disc_2 + " " + _disc[i, 2]);
                    }
                    dlgMenu.DoModal(GUIWindowManager.ActiveWindow);

                    // Set Driveletter and Mediatype according to selected Disc
                    _driveLetter = _disc[dlgMenu.SelectedId, 2];
                    _mediatype = _disc[dlgMenu.SelectedId, 0];
                }

                BeginPlayback();
            }

        }
    }


    /// <summary>
    /// Begin Playback: PreparePlayback(), Change Refresh Rate & LaunchPlayer
    /// </summary>
    private void BeginPlayback()
    {
      _ejectReceived = false;
      PreparePlayback();

      // set Refresh Rates depending on HDDVD or BD Message
      _refreshRateChangeSuccessful = false;
      switch (_mediatype)
      {
          case "Blu-Ray": // BD inserted - GUI_MSG_BLURAY_DISK_INSERTED
              SetRefreshRateBD();
              break;
          case "HD-DVD": // HD-DVD inserted - GUI_MSG_HDDVD_DISK_INSERTED
              SetRefreshRateHD();
              break;
      }
      // if Refresh Rate change was unsuccessful and the 24hz option is checked, Refresh Rate will be changed to 24hz
      // or: if Refresh Rate change is disabled and 24hz option is checked, refresh rate will be changed to 24hz by default
      // actually: 23.976 hz
      if ( _24hzRefreshRate && !_refreshRateChangeSuccessful)
      {
          try
          {
              Log.Info("Blu-Ray Player Launcher: Disc analysis failed/disabled, using 23.976 Hz by default");
              Log.Info("Blu-Ray Player Launcher: calling SetRefreshRateBasedOnFPS() - 23.976Hz");
              RefreshRateChanger.SetRefreshRateBasedOnFPS(23.976, "", RefreshRateChanger.MediaType.Video);
              _refreshRateChangeSuccessful = true;
          }
          catch (Exception e)
          {
              Log.Error("Blu-Ray Player Launcher:SetRefreshRate - exception {0}", e);
          }
      }
      LaunchPlayer();
    }

    /// <summary>
    /// Translate Keys to Keyboard
    /// </summary>
    public override void OnAction(MediaPortal.GUI.Library.Action action)
    {
      Log.Debug("Blu-Ray Player Launcher: OnAction -> action.wID = {0}", action.wID);

      if (_KeyboardTranslation == true)
      {
          RemoteControl.ProcessAction(_player, _playerProcess, action.wID);
      }
      else
      {
          Log.Debug("Blu-Ray Player Launcher: Remote to Keyboard Translation disabled");
      }

      base.OnAction(action);
    }

    #endregion

    #region Private helper methods

    /// <summary>
    /// Suspend GUI rendering
    /// </summary>
    private void PreparePlayback()
    {
      Log.Info("Blu-Ray Player Launcher: PreparePlayback");
      if (_driveLetter == null)
      {
        // should never happen, or should it ?
      }

      GUIGraphicsContext.BlankScreen = true;
      if (_suspendRenderingDuringPlay)
      {
        Log.Info("Blu-Ray Player Launcher: Suspend rendering");
        GUIGraphicsContext.CurrentState = GUIGraphicsContext.State.SUSPENDING;
      }
      InputDevices.Stop();
      g_Player.Stop();
      if (_minimizeDuringPlay)
      {
        Log.Info("Blu-Ray Player Launcher: Minimize MediaPortal");
        GUIGraphicsContext.form.Hide();
      }
      if (_restorePowerState)
      {
        try
        {
          uint result = PowerGetActiveScheme(IntPtr.Zero, ref _ptrActivePSGuid);
          if (result != 0)
          {
            Log.Error("Blu-Ray Player Launcher: failed to get currently active power scheme. Error code {0}", result);
          }
        }
        catch (Exception exp)
        {
          Log.Error("Blu-Ray Player Launcher: Exception on PowerGetActiveScheme -> {0}", exp.Message);
        }
      }
    }

    /// <summary>
    /// Resume GUI rendering, restore power state
    /// </summary>
    private void ResumeFromPlayback()
    {
      Log.Info("Blu-Ray Player Launcher: ResumeFromPlayback");
      _driveLetter = null;
      GUIGraphicsContext.BlankScreen = false;
      if (_minimizeDuringPlay)
      {
        Log.Info("Blu-Ray Player Launcher: Restore MediaPortal");
        GUIGraphicsContext.form.Show();
        GUIGraphicsContext.ResetLastActivity();
        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GETFOCUS, 0, 0, 0, 0, 0, null);
        GUIWindowManager.SendThreadMessage(msg);
      }
      if (_suspendRenderingDuringPlay)
      {
        Log.Info("Blu-Ray Player Launcher: Resume rendering");
        GUIGraphicsContext.CurrentState = GUIGraphicsContext.State.RUNNING;
      }
      InputDevices.Init();
      if (_restorePowerState && _ptrActivePSGuid != IntPtr.Zero)
      {
        try
        {
          uint result = PowerSetActiveScheme(IntPtr.Zero, _ptrActivePSGuid); 
          if (result != 0)
          {
            Log.Error("Blu-Ray Player Launcher: failed to restore previous active power scheme. Error code {0}", result);
          }
        }
        catch (Exception exp)
        {
          Log.Error("Blu-Ray Player Launcher: Exception on PowerSetActiveScheme -> {0}", exp.Message);
        }
      }
    }

    /// <summary>
    /// Set new refresh rate for BDs
    /// </summary>
    private void SetRefreshRateBD()
    {
        if (!_changeRefreshRate)
        {
            return;
        }

        Log.Info("Blu-Ray Player Launcher: SetRefreshRate for BluRay.");

        if (_driveLetter == null)
        {
            Log.Error("Blu-Ray Player Launcher: SetRefreshRate for BluRay - drive letter not assigned!");
            return;
        }

        double fps = -1;
        try
        {
            FileInfo[] files = null;
            double maxFileSize = 0;
            string maxFileName = "";

            DirectoryInfo dir = new DirectoryInfo(_driveLetter + "\\BDMV\\STREAM");
            files = dir.GetFiles("*.M2TS");
            foreach (FileInfo file in files)
            {
                if (file.Length > maxFileSize)
                {
                    maxFileSize = file.Length;
                    maxFileName = file.Name;
                }
            }

            MediaInfo MI = new MediaInfo();
            MI.Open(_driveLetter + "\\BDMV\\STREAM\\" + maxFileName);
            string _BD_Framerate = MI.Get(StreamKind.Video, 0, "FrameRate");

            Log.Info("Blu-Ray Player Launcher: BD Framerate via Mediainfo: - {0}", _BD_Framerate);

            switch (_BD_Framerate)
            {
                case "23.976":
                    fps = 23.976;
                    break;
                case "24":
                    fps = 24;
                    break;
                case "24.000":
                    fps = 24;
                    break;
                case "25":
                    fps = 25;
                    break;
                case "25.000":
                    fps = 25;
                    break;
                case "29.970":
                    fps = 29.97;
                    break;
                case "50":
                    fps = 50;
                    break;
                case "50.000":
                    fps = 50;
                    break;
                case "59.94":
                    fps = 59.94;
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error("Blu-Ray Player Launcher: SetRefreshRate - failed to get refresh rate from disk!");
            Log.Error("Blu-Ray Player Launcher: SetRefreshRate - exception {0}", e);
            return;
        }

        if (fps != -1)
        {
            try
            {
                Log.Info("Blu-Ray Player Launcher: calling SetRefreshRateBasedOnFPS() - {0}Hz", fps);
                RefreshRateChanger.SetRefreshRateBasedOnFPS(fps, "", RefreshRateChanger.MediaType.Video);
                _refreshRateChangeSuccessful = true;
            }
            catch (Exception e)
            {
                Log.Error("Blu-Ray Player Launcher: SetRefreshRate - exception {0}", e);
            }
        }
        else
        {
            Log.Error("Blu-Ray Player Launcher: Invalid Refresh Rate: {0} fps", fps);
            _refreshRateChangeSuccessful = false;
        }

    }

    /// <summary>
    /// Set new refresh rate for HDDVDs
    /// </summary>
    private void SetRefreshRateHD()
    {
        if (!_changeRefreshRate)
        {
            return;
        }

        Log.Info("Blu-Ray Player Launcher: SetRefreshRate for HD-DVD");

        if (_driveLetter == null)
        {
            Log.Error("Blu-Ray Player Launcher: SetRefreshRate for HD-DVD - drive letter not assigned!");
            return;
        }

        double fps = -1;
        try
        {
            FileInfo[] files = null;
            double maxFileSize = 0;
            string maxFileName = "";

            DirectoryInfo dir = new DirectoryInfo(_driveLetter + "\\HVDVD_TS");
            files = dir.GetFiles("*.EVO");
            foreach (FileInfo file in files)
            {
                Log.Info("Blu-Ray Player Launcher: Files: - {0} with {1} Bytes", file.Name, file.Length);
                if (file.Length > maxFileSize)
                {
                    maxFileSize = file.Length;
                    maxFileName = file.Name;
                }
            }
            Log.Info("Blu-Ray Player Launcher: Biggest file: - {0} with {1} bytes", maxFileName, maxFileSize);
            MediaInfo MI = new MediaInfo();
            MI.Open(_driveLetter + "\\HVDVD_TS\\" + maxFileName);
            string _HDDVD_Framerate = MI.Get(StreamKind.Video, 0, "FrameRate");
            Log.Info("Blu-Ray Player Launcher: HD-DVD Framerate via Mediainfo: - {0}", _HDDVD_Framerate);
            switch (_HDDVD_Framerate)
            {
                case "23.976":
                    fps = 23.976;
                    break;
                case "24":
                    fps = 24;
                    break;
                case "25":
                    fps = 25;
                    break;

                case "29.970":    // HD-DVDs are sometimes reported as 29.97hz even if they are 24hz.
                    fps = 23.976; // probably due hd-dvds need are in 29.97hz format with interlaced and telecine flags (repeat_first_field, top_field_first and progressive_frame for MPEG-2).
                    break;        // dirty workaround: set refresh rate to 23.976hz, possibly makes real 29.97hz discs (if available) stutter

                case "50":
                    fps = 50;
                    break;
                case "59.94":
                    fps = 59.94;
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error("Blu-Ray Player Launcher: SetRefreshRate - failed to get refresh rate from disk!");
            Log.Error("Blu-Ray Player Launcher: SetRefreshRate - exception {0}", e);
            return;
        }

        if (fps != -1)
        {
            try
            {
                Log.Info("Blu-Ray Player Launcher: calling SetRefreshRateBasedOnFPS() - {0}Hz", fps);
                RefreshRateChanger.SetRefreshRateBasedOnFPS(fps, "", RefreshRateChanger.MediaType.Video);
                _refreshRateChangeSuccessful = true;
            }
            catch (Exception e)
            {
                Log.Error("Blu-Ray Player Launcher: SetRefreshRate - exception {0}", e);
            }
        }
        else
        {
            Log.Error("Blu-Ray Player Launcher: Invalid Refresh Rate: {0} fps", fps);
            _refreshRateChangeSuccessful = false;
        }

    }

    private void SetRefreshRateBDold() // not used anymore, but still there ;)
    {
        if (!_changeRefreshRate)
        {
            return;
        }

        Log.Info("Blu-Ray Player Launcher: SetRefreshRate for BDs");

        if (_driveLetter == null)
        {
            Log.Error("Blu-Ray Player Launcher: SetRefreshRate - drive letter not assigned!");
            return;
        }

        FileInfo[] files = null;
        double fps = -1;
        try
        {
            DirectoryInfo dir = new DirectoryInfo(_driveLetter + "\\BDMV\\PLAYLIST");
            PlaylistFiles.Clear();
            StreamFiles.Clear();
            StreamClipFiles.Clear();

            files = dir.GetFiles("*.MPLS");
            foreach (FileInfo file in files)
            {
                TSPlaylistFile playlistFile = new TSPlaylistFile(file);
                PlaylistFiles.Add(file.Name.ToUpper(), playlistFile);
                //Log.Info("Blu-Ray Player Launcher: MPLS - {0} {1}", file.Name, playlistFile.TotalSize);
            }

            dir = new DirectoryInfo(_driveLetter + "\\BDMV\\STREAM");
            files = dir.GetFiles("*.M2TS");
            foreach (FileInfo file in files)
            {
                TSStreamFile streamFile = new TSStreamFile(file);
                StreamFiles.Add(file.Name.ToUpper(), streamFile);
            }

            dir = new DirectoryInfo(_driveLetter + "\\BDMV\\CLIPINF");
            files = dir.GetFiles("*.CLPI");
            foreach (FileInfo file in files)
            {
                TSStreamClipFile streamClipFile = new TSStreamClipFile(file);
                StreamClipFiles.Add(file.Name.ToUpper(), streamClipFile);
            }

            foreach (TSStreamClipFile streamClipFile in StreamClipFiles.Values)
            {
                streamClipFile.Scan();
            }
            foreach (TSStreamFile streamFile in StreamFiles.Values)
            {
                streamFile.Scan(null, false);
            }

            double maxLenght = 0;

            foreach (TSPlaylistFile playlistFile in PlaylistFiles.Values)
            {
                playlistFile.Scan(StreamFiles, StreamClipFiles);
                Log.Info("Blu-Ray Player Launcher: total size - {0} - {1}", playlistFile.TotalLength, playlistFile.VideoStreams[0].FrameRate.ToString());

                if (maxLenght < playlistFile.TotalLength)
                {
                    // find the largest movie clip, might not work with seamless branching movies... 
                    maxLenght = playlistFile.TotalLength;

                    switch (playlistFile.VideoStreams[0].FrameRate)
                    {
                        case TSFrameRate.FRAMERATE_23_976:
                            fps = 23.976;
                            break;
                        case TSFrameRate.FRAMERATE_24:
                            fps = 24;
                            break;
                        case TSFrameRate.FRAMERATE_25:
                            fps = 25;
                            break;
                        case TSFrameRate.FRAMERATE_29_97:
                            fps = 29.97;
                            break;
                        case TSFrameRate.FRAMERATE_50:
                            fps = 50;
                            break;
                        case TSFrameRate.FRAMERATE_59_94:
                            fps = 59.94;
                            break;
                    }
                }
                playlistFile.ClearBitrates();
            }
        }
        catch (Exception e)
        {
            Log.Error("Blu-Ray Player Launcher: SetRefreshRate - failed to get refresh rate from disk!");
            Log.Error("Blu-Ray Player Launcher: SetRefreshRate - exception {0}", e);
            return;
        }

        try
        {
            Log.Info("Blu-Ray Player Launcher: calling SetRefreshRateBasedOnFPS() - {0}Hz", fps);
            RefreshRateChanger.SetRefreshRateBasedOnFPS(fps, "", RefreshRateChanger.MediaType.Video);
            _refreshRateChangeSuccessful = true;
        }
        catch (Exception e)
        {
            Log.Error("Blu-Ray Player Launcher:SetRefreshRate - exception {0}", e);
        }
    }

    /// <summary>
    /// Restore previous refresh rate
    /// </summary>
    private static void RestoreRefreshRate()
    {
      if (!_changeRefreshRate)
      {
        return;
      }
      
      Log.Info("Blu-Ray Player Launcher: RestoreRefreshRate");

      try
      {
        RefreshRateChanger.AdaptRefreshRate();
      }
      catch (Exception e)
      {
        Log.Error("Blu-Ray Player Launcher: RestoreRefreshRate - exception {0}", e);
      }
    }

    /// <summary>
    /// Player exited event is rised when player process is exiting
    /// </summary>
    private void PlayerExited(object o, EventArgs e)
    {
      Log.Info("Blu-Ray Player Launcher: PlayerExited");
      // Cleanup only if no eject has been received
      if (!_ejectReceived)
      {
        Log.Info("Blu-Ray Player Launcher: No eject has been received, cleaning up");
        ResumeFromPlayback();
        RestoreRefreshRate();
      }
      GUIGraphicsContext.form.Invoke(new PlayerExitedInvoke(PreviousView));

      _playerProcess = null;
    }

    /// <summary>
    /// Go back to previous view
    /// </summary>
    public void PreviousView()
    {
        if (_startWithBasicHome)
        {
            GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_SECOND_HOME);
        }
        else
        {
            GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_HOME);
        }
    }

    /// <summary>
    /// Launch external player
    /// </summary>
    private void LaunchPlayer()
    {
      Log.Info("Blu-Ray Player Launcher: LaunchPlayer");
      ProcessStartInfo psi = new ProcessStartInfo();
      _playerProcess = new Process();
      psi.FileName = _playerExecutable;
      psi.Arguments = _driveLetter;

      if (!File.Exists(psi.FileName))
      {
        Log.Info("Blu-Ray Player Launcher: missing player executable - {0}", psi.FileName);
        ResumeFromPlayback();
        RestoreRefreshRate();
        return;
      }

      _playerProcess.EnableRaisingEvents = true;
      _playerProcess.Exited += PlayerExited;
      _playerProcess.StartInfo = psi;

      string heading = "Blu-Ray Player Launcher";
      string line1 = "Error during starting up the external player.";
      string line2 = "Please make sure correct executable has been selected in config.";

      try
      {
        heading = LocalizeStrings.Get(1);
        line1 = LocalizeStrings.Get(2);
        line2 = LocalizeStrings.Get(3);
      }
      catch
      {
        Log.Error("Blu-Ray Player Launcher: error accessing localization strings");
      }

      try
      {
        _playerProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
        _playerProcess.Start();
        _playerProcess.WaitForInputIdle();

        if (_player == Players.Cyberlink)
        { 
          //PowerDVD doesn't start in fullscreen mode and doesn't play automatically like the Arcsoft player
          //and because PowerDVD is too stupid to automatically select a drive to play from, we have to choose one by
          //ourselfs and select it through the menu

          RemoteControl.ProcessAction(_player, _playerProcess, MediaPortal.GUI.Library.Action.ActionType.ACTION_SHOW_FULLSCREEN);

          DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
          List<string> cdromDriveLetters = new List<string>();

          bool driveFound = false;
          bool driveReady = false;

          foreach (DriveInfo drive in drives)
          {
            if (drive.DriveType == DriveType.CDRom)
            {
              cdromDriveLetters.Add(drive.Name.Substring(0, 2));

              if (drive.Name.Substring(0, 2) == _driveLetter)
              {
                driveFound = true;
                driveReady = drive.IsReady;
              }
            }
          }

          if (driveFound && driveReady)
          {
            if (cdromDriveLetters.Count <= 1)
            {
              //when just 1 drive is available we just can send the play action...
              RemoteControl.ProcessAction(_player, _playerProcess, MediaPortal.GUI.Library.Action.ActionType.ACTION_PLAY);
            }
            else
            {
              //There is no real matching MP action for this "source selection" in PowerDVD so I just used ACTION_SHOW_PLAYLIST
              RemoteControl.ProcessAction(_player, _playerProcess, MediaPortal.GUI.Library.Action.ActionType.ACTION_SHOW_PLAYLIST);
              RemoteControl.ProcessAction(_player, _playerProcess, MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_DOWN);

              foreach (string drive in cdromDriveLetters)
              {
                if (drive == _driveLetter)
                {
                  RemoteControl.ProcessAction(_player, _playerProcess, MediaPortal.GUI.Library.Action.ActionType.ACTION_PLAY);
                }
                else
                {
                  RemoteControl.ProcessAction(_player, _playerProcess, MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_DOWN);
                }
              }
            }
          }
          else
          {
            //even if we didn't found a disc in the current drive. Maybe the user just want's to start the disc that was
            //previously running through autorun or whatever. PowerDVD should still know the drive so we can just send
            //the play action

            RemoteControl.ProcessAction(_player, _playerProcess, MediaPortal.GUI.Library.Action.ActionType.ACTION_PLAY);
          }
        }

        // with TMT the cursor gets visible when a keystroke is send. So we just move it out of view (Cursor.Hide unfortunately doesn't work)
        // TMT5 needs to be set to fullscreen 

        Cursor.Position = new System.Drawing.Point(0, SystemInformation.PrimaryMonitorSize.Height);

      }
      catch (Exception e)
      {
        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_OK);
        dlgOk.SetHeading(heading);
        dlgOk.SetLine(1, line1);
        dlgOk.SetLine(2, line2);
        dlgOk.DoModal(GetID);
        ResumeFromPlayback();
        RestoreRefreshRate();

        if (_startWithBasicHome)
        {
            GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_SECOND_HOME);
        }
        else
        {
            GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_HOME);
        }

        Log.Error("Blu-Ray Player Launcher: LaunchPlayer - exception {0}", e);
      }
    }

    /// <summary>
    /// Player exited event is rised when player process is exiting
    /// </summary>
    private void StopPlayer()
    {
        _playerProcess.Kill();
    }

    #endregion
    
    #region ISetupForm Members

    public int GetWindowId()
    {
      return GetID;
    }

    public string PluginName()
    {
      return "Blu-Ray Player Launcher";
    }

    public string Description()
    {
      return "Provide support for launching Arcsoft's MCE plugin and Cyberlink PowerDVD inside MediaPortal.";
    }

    public string Author()
    {
      return "tourettes / GhoSe";
    }

    public bool CanEnable()
    {
      return true;
    }

    public bool DefaultEnabled()
    {
      return true;
    }

    public bool HasSetup()
    {
      return true;
    }

    public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
    {
      if (!_showInMenu)
      {
        strButtonImage = String.Empty;
        strButtonImageFocus = String.Empty;
        strPictureImage = String.Empty;
        strButtonText = "";
        return false;
      }

      if (_pluginNameInMenu != "")
      {
        strButtonText = _pluginNameInMenu;
      }
      else
      {
        try
        {
          strButtonText = LocalizeStrings.Get(0);  // play blu-ray localized string
        }
        catch
        {
          Log.Error("Blu-Ray Player Launcher: failed to load localization strings!");
          strButtonText = "Play Blu-ray";
        }
      }

      strButtonImage = String.Empty;
      strButtonImageFocus = String.Empty;
      strPictureImage = "hover_my videos.png";
      return true;
    }

    // show the setup dialog
    public void ShowPlugin()
    {
      PluginSetupForm setupForm = new PluginSetupForm();
      setupForm.ShowDialog();
    }

    #endregion

    #region IShowPlugin Members

    public bool ShowDefaultHome()
    {
      return false;
    }

    #endregion
  }
}