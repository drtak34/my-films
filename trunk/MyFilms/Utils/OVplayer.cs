﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OnlineVideos;
using OnlineVideos.Hoster.Base;
using OnlineVideos.MediaPortal1;
using OnlineVideos.MediaPortal1.Player;
using PlayerFactory = OnlineVideos.MediaPortal1.Player.PlayerFactory;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using System.Threading;
//using TraktPlugin.GUI;
//using TraktPlugin.TraktAPI;
//using TraktPlugin.TraktAPI.DataStructures;

namespace MyFilmsPlugin.Utils
{
  using MyFilmsPlugin.MyFilms.MyFilmsGUI;

  using PlayerFactory = OnlineVideos.MediaPortal1.Player.PlayerFactory;

  public class OVplayer
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    #region Static Methods

    public static bool Play(string stream)
    {
      if (string.IsNullOrEmpty(stream)) return false;

      // use onlinevideo youtube siteutils to get
      // playback urls as myfilms only gives us the html page
      if (stream.Contains("youtube.com"))
      {
        // get playback url from stream
        LogMyFilms.Info("Getting playback url from page '{0}'", stream);

        var ovHosterProxy = OnlineVideosAppDomain.Domain.CreateInstanceAndUnwrap(typeof(OnlineVideosHosterProxy).Assembly.FullName, typeof(OnlineVideosHosterProxy).FullName) as OnlineVideosHosterProxy;
        stream = ovHosterProxy.GetVideoUrls(stream);

        if (string.IsNullOrEmpty(stream))
        {
          LogMyFilms.Info("Unable to find playback url from stream!", stream);
          return false;
        }
        LogMyFilms.Info("Found playback url: '{0}'", stream);
      }

      LogMyFilms.Info("Preparing graph for playback of '{0}'", stream);

      var factory = new PlayerFactory(PlayerType.Internal, stream);
      bool? prepareResult = ((OnlineVideosPlayer)factory.PreparedPlayer).PrepareGraph();

      if (prepareResult != true)
      {
        LogMyFilms.Info("Failed to create Player graph.");
        return false;
      }

      GUIBackgroundTask.Instance.ExecuteInBackgroundAndCallback(() =>
      {
        LogMyFilms.Info("OnlineVideo Pre-Buffering Started");
        if (((OnlineVideosPlayer)factory.PreparedPlayer).BufferFile())
        {
          LogMyFilms.Info("OnlineVideo Pre-Buffering Complete");
          return true;
        }
        else
        {
          return null;
        }

      },
      delegate(bool success, object result)
      {
        if ((result as bool?) != null)
        {
          (factory.PreparedPlayer as OVSPLayer).GoFullscreen = true;

          IPlayerFactory savedFactory = g_Player.Factory;
          g_Player.Factory = factory;
          try
          {
            g_Player.Play(stream, g_Player.MediaType.Video);
          }
          catch (Exception e)
          {
            LogMyFilms.Warn("Exception while playing stream: {0}", e.Message);
          }
          g_Player.Factory = savedFactory;
        }
        else
        {
          LogMyFilms.Error("Failed to Buffer stream.");
          factory.PreparedPlayer.Dispose();
        }
      },
      "PlayTrailerStream", false);

      return true;
    }

    #endregion
  }

  /// <summary>
  /// this class is needed because the Hoster class lives in the second appdomain, 
  /// and is not marked Serialiable and does not inherit from MarshalByRefObject, 
  /// so the object cannot cross appdomains
  /// </summary>
  class OnlineVideosHosterProxy : MarshalByRefObject
  {
    public OnlineVideosHosterProxy() { }

    public string GetVideoUrls(string url)
    {
      return HosterFactory.GetHoster("Youtube").getVideoUrls(url);
    }
  }

}