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
using MediaPortal.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace MyFilmsPlugin.MyFilms
{
    sealed class MyFilmsSettings
    {
        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
        //public const bool newAPI = true;

        public enum Path
        {
          log,
          logBackup,
          lang,
          MyFilmsPath,
          MyFilmsThumbsPath,
          ThumbsCache,
          DefaultImages,
          ViewImages,
          GrabberScripts,
          app,
          skin
        };

        public enum MinimumVersion
        {
          BrowseTheWeb,
          OnlineVideos,
          SubCentral,
          Trakt
        };

        #region Path Vars
        static string logPath = string.Empty;
        static string backupLogPath = string.Empty;
        static string langPath = string.Empty;
        static string ThumbsCachePath = string.Empty;
        static string MyFilmsPath = string.Empty;
        static string MyFilmsThumbsPath = string.Empty;
        static string DefaultImages = string.Empty;
        static string ViewImages = string.Empty;
        static string GrabberScripts = string.Empty;
        static string apppath = string.Empty;
        static string skinPath = string.Empty;

        #endregion

        #region Vars
        static Assembly _entryAssembly = null;
        static bool _isConfig;
        static Version _version = null;
        static Version _mpVersion = null;
        static DateTime _buildDate;
        static DateTime _mpBuildDate;
        static string _userAgent = null;
        #endregion

        #region Constructors
        private MyFilmsSettings() {}

        static MyFilmsSettings()
        {
          try
          {
            apppath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            _entryAssembly = Assembly.GetEntryAssembly();
            _isConfig = !System.IO.Path.GetFileNameWithoutExtension(EntryAssembly.Location).Equals("mediaportal", StringComparison.InvariantCultureIgnoreCase);
            _version = Assembly.GetCallingAssembly().GetName().Version;
            _mpVersion = Assembly.GetEntryAssembly().GetName().Version;
            _buildDate = getLinkerTimeStamp(Assembly.GetAssembly(typeof(MyFilmsSettings)).Location);
            _mpBuildDate = getLinkerTimeStamp(System.IO.Path.Combine(MyFilmsSettings.GetPath(Path.app), "MediaPortal.exe"));
            _userAgent = string.Format("MyFilms{0}/{1}", isConfig ? "Config" : string.Empty, Version);
          }
          catch (Exception)
          {
          }

          logPath           = Config.GetFile(Config.Dir.Log, "MyFilms.log");
          backupLogPath     = Config.GetFile(Config.Dir.Log, "MyFilms.bak");
          langPath          = Config.GetSubFolder(Config.Dir.Language, "MyFilms");

          MyFilmsPath       = Config.GetFolder(Config.Dir.Config) + @"\MyFilms";
          MyFilmsThumbsPath = Config.GetFolder(Config.Dir.Thumbs) + @"\MyFilms";
          DefaultImages     = Config.GetFolder(Config.Dir.Thumbs) + @"\MyFilms\DefaultImages";
          ViewImages        = Config.GetFolder(Config.Dir.Thumbs) + @"\MyFilms\ViewImages";

          ThumbsCachePath   = Config.GetFolder(Config.Dir.Thumbs) + @"\MyFilms\ThumbsCache";

          GrabberScripts    = Config.GetFolder(Config.Dir.Config) + @"\scripts\MyFilms";
          skinPath          = Config.GetFolder(Config.Dir.Skin);

          initFolders();
        }

      #endregion

        #region Properties
        /// <summary>
        /// Gets the Assembly that loaded the Plugin (usually Mediaportal.exe or Configuration.exe)
        /// </summary>
        public static Assembly EntryAssembly
        { get { return _entryAssembly; } }

        /// <summary>
        /// Gets a bool indicating wether or not the plugin has been loaded inside the configuration (checks EntryAssembly)
        /// </summary>
        public static bool isConfig
        { get { return _isConfig; } }

        /// <summary>
        /// Gets a the Version of the Plugin
        /// </summary>
        public static Version Version
        { get { return _version; } }

        public static Version MPVersion 
        { get { return _mpVersion; } }

        public static DateTime BuildDate
        { get { return _buildDate; } }

        public static DateTime MPBuildDate
        { get { return _mpBuildDate; } }

        public static string UserAgent
        { get { return _userAgent; } }

        public static int WebRequestCacheMinutes
        { get { return 180; } }


        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the requested Path
        /// </summary>
        /// <param name="path">The Settings.Path to get</param>
        /// <returns>The fully qualified Path as a String</returns>
        public static string GetPath(Path path)
        {
          switch (path)
          {
            case Path.log:
              return logPath;
            case Path.logBackup:
              return backupLogPath;
            case Path.lang:
              return langPath;
            case Path.DefaultImages:
              return DefaultImages;
            case Path.ViewImages:
              return ViewImages;
            case Path.MyFilmsPath:
              return MyFilmsPath;
            case Path.MyFilmsThumbsPath:
              return MyFilmsThumbsPath;
            case Path.ThumbsCache:
              return ThumbsCachePath;
            case Path.GrabberScripts:
              return GrabberScripts;
            case Path.app:
              return apppath;
            case Path.skin:
              return skinPath;
            default: return string.Empty;
          }
        }

        /// <summary>
        /// Gets the required minimum version for third party plugins
        /// </summary>
        /// <param name="plugin">The enum of the plugin to get the minimum version for</param>
        /// <returns>String with minimum version required</returns>
        public static string GetRequiredMinimumVersion(MinimumVersion plugin)
        {
          switch (plugin)
          {
            case MinimumVersion.BrowseTheWeb:
              return "0.3.0";
            case MinimumVersion.OnlineVideos:
              return "0.29";
            case MinimumVersion.SubCentral:
              return "1.1";
            case MinimumVersion.Trakt:
              return "1.0.5.1";
            default: return "unknown";
          }
        }
        #endregion

        # region Helpers
        private static void initFolders()
        {
            try
            {
                createDirIfNotExists(langPath);
            }
            catch (Exception ex)
            {
                LogMyFilms.Error("Error initiating Paths: " + ex.Message);
            }
        }

        private static void createDirIfNotExists(string dir)
        {
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(dir))) 
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dir));
        }

        private static DateTime getLinkerTimeStamp(string filePath)
        {
            const int PeHeaderOffset = 60;
            const int LinkerTimestampOffset = 8;

            byte[] b = new byte[2047];
            using (System.IO.Stream s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                s.Read(b, 0, 2047);
            }

            int secondsSince1970 = BitConverter.ToInt32(b, BitConverter.ToInt32(b, PeHeaderOffset) + LinkerTimestampOffset);

            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(secondsSince1970);
        }

        #endregion
    }
}
