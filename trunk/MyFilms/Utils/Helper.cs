#region GNU license
// MP-TVSeries - Plugin for Mediaportal
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

namespace MyFilmsPlugin.MyFilms.Utils
{
  using System;
  using System.Collections;
  using System.IO;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Diagnostics;
  using System.Text;

  using MediaPortal.Configuration;
  using MediaPortal.GUI.Library;
  using MediaPortal.Util;
  using MediaPortal.Ripper;

  using System.Globalization;
  using System.Reflection;
  using System.Security.Cryptography;

  using TraktPlugin;
  using TraktPlugin.TraktAPI;
  using TraktPlugin.TraktAPI.DataStructures;

  #region String Extension Methods
    public static class StringExtensions
    {        
        public static bool IsNumerical(this string number)
        {
            double isNumber = 0;
            return System.Double.TryParse(number, out isNumber);
        }

        /// <summary>
        /// ASCII chars that are considered "special" in the context of CleanStringOfSpecialChars
        /// </summary>
        static int[] specialCharsFromTo = new int[] { 0,  31, 
                                                     33,  47, 
                                                     58,  64, 
                                                     91,  96, 
                                                    123, 127 };
        /// <summary>
        /// Cleans a string of Punctuation and other Special Characters (removes them)
        /// Leaves ASCII Chars: 0-9, a-z, A-Z, space
        /// Leaves non-ASCII Chars
        /// </summary>
        /// <param name="input">The string to clean</param>
        /// <returns>The cleaned string</returns>
        public static string CleanStringOfSpecialChars(this string input)
        {
            char[] cInput = input.ToCharArray();
            int removed = 0;
            bool isRemoved = false;
            for (int i = 0; i < cInput.Length; i++)
            {
                isRemoved = false;
                for (int j = 0; j < specialCharsFromTo.Length; j += 2)
                {
                    if (cInput[i] >= specialCharsFromTo[j] && cInput[i] <= specialCharsFromTo[j + 1])
                    {
                        removed++;
                        isRemoved = true;
                        break;
                    }
                }
                if (!isRemoved)
                    cInput[i - removed] = cInput[i];
            }

            // shrink the result
            char[] newLenghtChars = new char[cInput.Length - removed];
            for (int i = 0; i < newLenghtChars.Length; i++)
                newLenghtChars[i] = cInput[i];

            return new string(newLenghtChars);
        }
        
        /// <summary>
        /// TitleCases a string
        /// </summary>
        /// <param name="input">The string to TitleCase</param>
        /// <returns>The TitleCased String</returns>
        public static string ToTitleCase(this string input)
        {
            return textInfo.ToTitleCase(input.ToLower());
        }
        static TextInfo textInfo = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo;

        public static string ToSHA1Hash(this string password)
        {
            // don't store the hash if password is empty
            if (string.IsNullOrEmpty(password)) return string.Empty;

            byte[] buffer = Encoding.Default.GetBytes(password);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
        }

        public static string EscapeLikeValue(string valueWithoutWildcards)
        {
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < valueWithoutWildcards.Length; i++)
          {
            char c = valueWithoutWildcards[i];
            if (c == '*' || c == '%' || c == '[' || c == ']')
              sb.Append("[").Append(c).Append("]");
            else if (c == '\'')
              sb.Append("''");
            else
              sb.Append(c);
          }
          return sb.ToString();
        }

        /// <summary>
        /// Returns cleaned string valid for XML
        /// </summary>
        /// <param name="in_string">The string to clean</param>
        /// <returns>The Whitelisted String</returns>
        public static string XmlCharacterWhitelist(string in_string)
        {
          if (in_string == null) return null;

          StringBuilder sbOutput = new StringBuilder();
          char ch;

          for (int i = 0; i < in_string.Length; i++)
          {
            ch = in_string[i];
            if ((ch >= 0x0020 && ch <= 0xD7FF) ||
              (ch >= 0xE000 && ch <= 0xFFFD) ||
              ch == 0x0009 ||
              ch == 0x000A ||
              ch == 0x000D)
            {
              sbOutput.Append(ch);
            }
          }
          return sbOutput.ToString();
        }


        /// <summary>
        /// Remove illegal XML characters from a string.
        /// </summary>
        public static string SanitizeXmlString(string xml)
        {
          if (xml == null)
          {
            throw new ArgumentNullException("xml");
          }

          StringBuilder buffer = new StringBuilder(xml.Length);

          foreach (char c in xml)
          {
            if (IsLegalXmlChar(c))
            {
              buffer.Append(c);
            }
          }

          return buffer.ToString();
        }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        public static bool IsLegalXmlChar(int character)
        {
          return
          (
             character == 0x9 /* == '\t' == 9   */          ||
             character == 0xA /* == '\n' == 10  */          ||
             character == 0xD /* == '\r' == 13  */          ||
            (character >= 0x20 && character <= 0xD7FF) ||
            (character >= 0xE000 && character <= 0xFFFD) ||
            (character >= 0x10000 && character <= 0x10FFFF)
          );
        }

    }
    #endregion

    public class Helper
    {
        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
        #region List<T> Methods

        public static List<T> inverseList<T>(List<T> input)
        {
            List<T> result = new List<T>(input.Count);
            for (int i = input.Count - 1; i >= 0; i--)
                result.Add(input[i]);
            return result;
        }
       
        public static T getElementFromList<T, P>(P currPropertyValue, string PropertyName, int indexOffset, List<T> elements)
        {
            // takes care of "looping"
            if (elements.Count == 0) return default(T);
            int indexToGet = 0;
            P value = default(P);
            for (int i = 0; i < elements.Count; i++)
            {
                try
                {
                    value = (P)elements[i].GetType().InvokeMember(PropertyName, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField, null, elements[i], null);
                    if (value.Equals(currPropertyValue))
                    {
                        indexToGet = i + indexOffset;
                        break;
                    }
                }
                catch (Exception x)
                {
                    LogMyFilms.Debug("Wrong call of getElementFromList<T,P>: the Type " + elements[i].GetType().Name + " - " + x.Message);
                    return default(T);
                }
            }
            if (indexToGet < 0) indexToGet = elements.Count + indexToGet;
            if (indexToGet >= elements.Count) indexToGet = indexToGet - elements.Count;
            return elements[indexToGet];
        }


        public static List<P> getPropertyListFromList<T, P>(string PropertyNameToGet, List<T> elements)
        {
            List<P> results = new List<P>();
            foreach (T elem in elements)
            {
                try
                {
                    results.Add((P)elem.GetType().InvokeMember(PropertyNameToGet, System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField, null, elem, null));
                }
                catch (Exception)
                {
                  LogMyFilms.Debug("Wrong call of getPropertyListFromList<T,P>: Type " + elem.GetType().Name);
                }
            }
            return results;
        }

        public delegate void ForEachOperation<T>(T element, int currIndex);
        /// <summary>
        /// Performs an operation for each element in the list, by starting with a specific index and working its way around it (eg: n, n+1, n-1, n+2, n-2, ...)
        /// </summary>
        /// <typeparam name="T">The Type of elements in the IList</typeparam>
        /// <param name="elements">All elements, this value cannot be null</param>
        /// <param name="startElement">The starting point for the operation (0 operates like a traditional foreach loop)</param>
        /// <param name="operation">The operation to perform on each element</param>
        public static void ProximityForEach<T>(IList<T> elements, int startElement, ForEachOperation<T> operation)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");
            if ((startElement >= elements.Count && elements.Count > 0) || startElement < 0)
                throw new ArgumentOutOfRangeException("startElement", startElement, "startElement must be > 0 and <= elements.Count (" + elements.Count + ")");
            if (elements.Count > 0)                                      // if empty list, nothing to do, but legal, so not an exception
            {
                T item;
                for (int lower = startElement, upper = startElement + 1; // start with the selected, and then go down before going up
                     upper < elements.Count || lower >= 0;               // only exit once both ends have been reached
                     lower--, upper++)
                {
                    if (lower >= 0)                                      // are lower elems left?
                    {
                        item = elements[lower];
                        operation(item, lower);
                        elements[lower] = item;
                    }
                    if (upper < elements.Count)                          // are higher elems left?
                    {
                        item = elements[upper];
                        operation(item, upper);
                        elements[upper] = item;
                    }
                }
            }
        }      

        #endregion

        #region Other Public Methods

        /// <summary>
        /// Resolves skin\\ and thumbs\\ relative paths to absolute.
        /// Other relative paths are resolved using MediaPortal installation directory.
        /// Absolute paths are just cleaned.
        /// </summary>
        /// <param name="file">Relative or absolute path to resolve</param>
        /// <returns></returns>
        //public static string getCleanAbsolutePath(string file) {
        //    if (!System.IO.Path.IsPathRooted(file)) {
        //        // Respect custom skin folders
        //        if (file.ToLower().StartsWith("skin\\"))
        //          file = file.Replace("skin", Settings.GetPath(Config.Dir.Skin));
        //        else if (file.ToLower().StartsWith("thumbs\\"))
        //            file = file.Replace("thumbs", Settings.GetPath(Settings.Path.thumbs));
        //        else
        //            file = Helper.PathCombine(Settings.GetPath(Settings.Path.app), file);
        //    }

        //    foreach (char c in System.IO.Path.GetInvalidPathChars())
        //        file = file.Replace(c, '_');

        //    return file;
        //}

        /// <summary>
        /// Removes non-existant files from a list of filenames
        /// </summary>
        /// <param name="filenames"></param>
        /// <returns></returns>
        public static List<string> filterExistingFiles(List<string> filenames)
        {
            for (int f = 0; f < filenames.Count; f++) {
                bool wasCached = false;
                if ((wasCached = nonExistingFiles.Contains(filenames[f])) || !System.IO.File.Exists(filenames[f])) {
                    if (!wasCached) {
                        LogMyFilms.Debug("File does not exist: " + filenames[f]);
                        nonExistingFiles.Add(filenames[f]);
                    }
                    filenames.RemoveAt(f);
                    f--;
                }
            }
            return filenames;
        } static List<string> nonExistingFiles = new List<string>();

        /// <summary>
        /// Convertes a given amount of Milliseconds into humanly readable MM:SS format
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static System.String MSToMMSS(double milliseconds)
        {
            TimeSpan t = new TimeSpan(0, 0, 0, 0, (int)milliseconds);
            //cs1 anomalies or no disc/data available -> -:- 
            if (milliseconds <= 0)
            { return ("-- : --"); }
            //cs1 playtimes >= 1 hour -> 1:MM:SS 
            else if (milliseconds >= 3600000)
            { return t.Hours.ToString("0") + ":" + t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00"); }
            //cs1 playtimes < 1 hour -> MM:SS 
            else { return t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00"); }
        }

        /// <summary>
        /// Joins two parts of a path
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string PathCombine(string path1, string path2) {
            if (path1 == null && path2 == null) return string.Empty;
            if (path1 == null) return path2;
            if (path2 == null) return path1;
            if (path2.Length > 0 && (path2[0] == '\\' || path2[0] == '/')) path2 = path2.Substring(1);
            return System.IO.Path.Combine(path1, path2);
        }

        /// <summary>
        /// Cleans the path by removing invalid characters
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string cleanLocalPath(string path)
        {
          foreach (char c in System.IO.Path.GetInvalidFileNameChars())
          {
            path = path.Replace(c, invalidCharReplacement);
          }
          // Also remove trailing dots and spaces            
          return path.TrimEnd(new char[] { '.' }).Trim();
        } const char invalidCharReplacement = '_';

        /// <summary>
        /// checks for MyFilms fields being set or none/empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool FieldIsSet(string value)
        {
          if (!string.IsNullOrEmpty(value) && value != "(none)") 
            return true;
          else 
            return false;
        }

        /// <summary>
        /// Removes 'the' and other common words from the beginning of a series
        /// </summary>
        /// <param name="sName"></param>
        /// <returns></returns>
        public static string GetSortByName(string sName)
        {
            string SortBy = sName;
            string SortByStrings = "SortByStrings";

            // loop through and try to remove a preposition            
            string[] prepositions = SortByStrings.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);            
            foreach (string currWord in prepositions)
            {
                string word = currWord.ToLower() + " ";
                if (sName.ToLower().IndexOf(word) == 0)
                {
                    SortBy = sName.Substring(word.Length) + ", " + sName.Substring(0, currWord.Length);
                    break;
                }
            }
            return SortBy;
        }

        /// <summary>
        /// Converts a string of letters to corresponding numbers
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		public static string ConvertSMSInputToPinCode(string input) {
			switch (input.ToLower()) {
				case "a":
				case "b":
				case "c":
					return "2";

				case "d":
				case "e":
				case "f":
					return "3";

				case "g":
				case "h":
				case "i":
					return "4";

				case "j":
				case "k":
				case "l":
					return "5";

				case "m":
				case "n":
				case "o":
					return "6";

				case "p":
				case "q":
				case "r":
				case "s":
					return "7";

				case "t":
				case "u":
				case "v":
					return "8";

				case "w":
				case "x":
				case "y":
				case "z":
					return "9";

				default:
					return input;

			}
		}


        /// <summary>
        /// Removes duplicate items from a list
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns>A list with unique items</returns>
        public static List<string> RemoveDuplicates(List<string> inputList) {
            Dictionary<string, int> uniqueStore = new Dictionary<string, int>();
            List<string> finalList = new List<string>();
            foreach (string currValue in inputList) {
                if (!uniqueStore.ContainsKey(currValue)) {
                    uniqueStore.Add(currValue, 0);
                    finalList.Add(currValue);
                }
            }
            return finalList;
        }

        /// <summary>
        /// Returns a limited list of items
        /// </summary>
        /// <param name="list"></param>
        /// <param name="limit"></param>
        public static void LimitList(ref List<string> list, int limit) {
            if (limit >= list.Count) return;
            list.RemoveRange(list.Count - (list.Count - limit), (list.Count - limit));
        }

        /// <summary>
        /// Checks if a filename is an Image file e.g. ISO
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>Returns true if file is a image</returns>
        public static bool IsImageFile(string filename) {
            string extension = System.IO.Path.GetExtension(filename).ToLower();
            return VirtualDirectory.IsImageFile(extension);
        }

        /// <summary>
        /// Checks if Fullscreen video is active
        /// </summary>
        /// <returns></returns>
        public static bool IsFullscreenVideo {
            get {
                bool isFullscreen = false;
                if (GUIWindowManager.ActiveWindow == (int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO)
                    isFullscreen = true;

                return isFullscreen;
            }
        }

        public static void disableNativeAutoplay()
        {
            LogMyFilms.Debug("Disabling native autoplay.");
            AutoPlay.StopListening();
        }

        public static void enableNativeAutoplay()
        {
            if (GUIGraphicsContext.CurrentState == GUIGraphicsContext.State.RUNNING)
            {
                LogMyFilms.Debug("Re-enabling native autoplay.");
                AutoPlay.StartListening();
            }
        }

        public static string UppercaseFirst(string s) {
            if (string.IsNullOrEmpty(s)) {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static bool Contains(string source, string toCheck)
        {
          return Contains(source, toCheck, StringComparison.OrdinalIgnoreCase);
        }
        public static bool Contains(string source, string toCheck, StringComparison comp)
        {
          if (string.IsNullOrEmpty(toCheck) || string.IsNullOrEmpty(source))
            return true;
          return source.IndexOf(toCheck, comp) >= 0;
        } 

        #endregion

        #region Assembly methods
        public static bool IsAssemblyAvailable(string name, Version ver, bool onlymatchingversion) {
            bool result = false;

            LogMyFilms.Debug(string.Format("Checking whether assembly {0} is available and loaded...", name));

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
                try
                {
                    if (a.GetName().Name == name && a.GetName().Version >= ver)
                    {
                      LogMyFilms.Debug(string.Format("Assembly '{0}' with Version '{1}' is available and loaded.", name, a.GetName().Version.ToString()));
                        result = true;
                        break;
                    }
                }
                catch
                {
                    LogMyFilms.Debug(string.Format("Assembly.GetName() call failed for '{0}'!\n", a.Location));
                }

            if (!result && !onlymatchingversion) {
                LogMyFilms.Debug(string.Format("Assembly {0} is not loaded (not available?), trying to load it manually...", name));
                try {
                    //Assembly assembly = AppDomain.CurrentDomain.Reflection(new AssemblyName(name));
                    Assembly assembly = Assembly.ReflectionOnlyLoad(name);
                    LogMyFilms.Debug(string.Format("Assembly {0} is available and loaded successfully.", name));
                    result = true;
                }
                catch (Exception e) {
                    LogMyFilms.Debug(string.Format("Assembly {0} is unavailable, load unsuccessful: {1}:{2}", name, e.GetType(), e.Message));
                }
            }

            return result;
        }

        public static bool IsPluginEnabled(string name) {
            using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.MPSettings()) {
                return xmlreader.GetValueAsBool("plugins", name, false);
            }
        }

        public static bool IsSubCentralAvailableAndEnabled
        {
          get
          {
            bool status = Helper.IsAssemblyAvailable("SubCentral", new Version(1, 0, 0, 0), false) && IsPluginEnabled("SubCentral");
            LogMyFilms.Debug("Helper() - SubCentral available and enabled = '" + status + "'");
            return status;
          }
        }

        public static bool IsTraktAvailableAndEnabled
        {
          get
          {
            bool status = Helper.IsAssemblyAvailable("TraktPlugin", new Version(1, 0, 5, 1), false) && IsPluginEnabled("Trakt");
            LogMyFilms.Debug("Helper() - TraktPlugin available and enabled = '" + status + "'");
            return status;
          }
        }

        public static bool IsTraktAvailableAndEnabledAndNewVersion
        {
          get
          {
            bool status = Helper.IsAssemblyAvailable("TraktPlugin", new Version(1, 2, 1, 1), true) && IsPluginEnabled("Trakt");
            LogMyFilms.Debug("Helper() - TraktPlugin (new version) available and enabled = '" + status + "'");
            return status;
          }
        }

        public static bool IsTraktAvailableAndEnabledAndVersion1301
        {
          get
          {
            bool status = Helper.IsAssemblyAvailable("TraktPlugin", new Version(1, 3, 0, 1), true) && IsPluginEnabled("Trakt");
            LogMyFilms.Debug("Helper() - TraktPlugin (new version) available and enabled = '" + status + "'");
            return status;
          }
        }

        public static bool IsTraktAvailableAndEnabledAndVersion1311
        {
          get
          {
            bool status = Helper.IsAssemblyAvailable("TraktPlugin", new Version(1, 3, 1, 1), true) && IsPluginEnabled("Trakt");
            LogMyFilms.Debug("Helper() - TraktPlugin (new version) available and enabled = '" + status + "'");
            return status;
          }
        }

        public static bool IsBrowseTheWebAvailableAndEnabled
        {
          get
          {
            bool status = false;
            bool BrowseTheWebRightPlugin = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "BrowseTheWeb");
            bool BrowseTheWebRightVersion = PluginManager.SetupForms.Cast<ISetupForm>().Any(plugin => plugin.PluginName() == "BrowseTheWeb" && plugin.GetType().Assembly.GetName().Version.Minor >= 0);
            LogMyFilms.Debug("MyFilms.Init() - BrowseTheWebRightVersion = '" + BrowseTheWebRightVersion + "', BrowseTheWebRightVersion = '" + BrowseTheWebRightVersion + "'");
            if (BrowseTheWebRightPlugin && BrowseTheWebRightVersion)
              status = true;
            LogMyFilms.Debug("Helper() - BrowseTheWeb available and enabled = '" + status + "'");
            return status;
            // return Helper.IsAssemblyAvailable("BrowseTheWeb", new Version(0, 3, 0, 0)) && IsPluginEnabled("BrowseTheWeb");
          }
        }

        public static bool IsBDHandlerAvailableAndEnabled
        {
          get
          {
            bool status = Helper.IsAssemblyAvailable("BDHandler", new Version(0, 9, 7, 29), false) && IsPluginEnabled("Blu-ray Handler");
            LogMyFilms.Debug("Helper() - BDHandler available and enabled = '" + status + "'");
            return status;
          }
        }

        public static bool IsOnlineVideosAvailableAndEnabled
        {
          get
          {
            bool status = Helper.IsAssemblyAvailable("OnlineVideos", new Version(0, 27, 0, 0), true) && IsPluginEnabled("OnlineVideos");
            LogMyFilms.Debug("Helper() - OnlineVideos available and enabled = '" + status + "'");
            return status;
          }
        }

        public static string GetTraktUser()
        {
          if (Helper.IsTraktAvailableAndEnabled)
            return TraktSettings.Username;
          return string.Empty;
        }

        public static List<string> GetTraktUserList() // only available with Trakt 1.3.1+
        {
          // List<global::TraktPlugin.TraktAPI.DataStructures.TraktAuthentication> userlist = new List<TraktAuthentication>();
          List<string> userlist = new List<string>();
          if (Helper.IsTraktAvailableAndEnabled)
          {
            if (TraktSettings.UserLogins.Count > 0)
              foreach (string user in userlist)
              {
                userlist.Add(user);
              }
              return userlist;
          }
          return null;
        }

        public static bool ChangeTraktUser(string newUserName)
        {
          if (TraktSettings.UserLogins.Count == 0) return false;

          foreach (var userlogin in TraktSettings.UserLogins)
          {
            if (userlogin.Username == newUserName)
            {
              TraktSettings.AccountStatus = ConnectionState.Pending;
              TraktSettings.Username = userlogin.Username;
              TraktSettings.Password = userlogin.Password;
              if (TraktSettings.AccountStatus == ConnectionState.Connected)
              return true;
            }
          }
          return false;
        }


        public static string GetUserOnlineStatus(string username)
        {
          string status = "local";
          if (IsTraktAvailableAndEnabled)
          {
            try
            {
              if (TraktSettings.Username == username)
              {
                if (TraktSettings.AccountStatus == ConnectionState.Connected)
                  status = "online";
                else
                  status = "offline";
              }
            }
            catch (Exception ex)
            {
              LogMyFilms.Warn("GetUserOnlineStatus(): problem getting Trakt user status : " + ex.Message);
              status = "local";
            }
          }
          LogMyFilms.Debug("GetUserOnlineStatus(): connection status for '" + username + "' is set to '" + status + "'");
          return status;
        }

        #endregion

        #region Web Methods
        public static bool DownloadFile(string url, string localFile)
        {
          WebClient webClient = new WebClient();
          webClient.Headers.Add("user-agent", MyFilmsSettings.UserAgent);

          try
          {
            Directory.CreateDirectory(Path.GetDirectoryName(localFile));
            if (!File.Exists(localFile) || ImageFast.FastFromFile(localFile) == null)
            {
              LogMyFilms.Debug("Downloading new file from: " + url);
              webClient.DownloadFile(url, localFile);
            }
            return true;
          }
          catch (WebException)
          {
            LogMyFilms.Debug("File download failed from '{0}' to '{1}'", url, localFile);
            return false;
          }
        }
        #endregion


        public static bool IsFileUsedbyAnotherProcess(string filename)
        {
          bool inUse = true;
          try
          {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              if (fs.CanRead)
              {
                fs.Close();
                inUse = false;
              }
            }

            // File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read); // this could also be used ?
          }
          catch (System.IO.IOException exp)
          {
            LogMyFilms.Debug("IsFileUsedbyAnotherProcess() - cannot open file: '" + exp.Message + "'");
            return true;
          }
          return inUse;
        }

        static bool FileInUse(string path)
        {
          try
          {
            //Just opening the file as open/create
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
              //If required we can check for read/write by using fs.CanRead or fs.CanWrite
            }
            return false;
          }
          catch (IOException ex)
          {
            //check if message is for a File IO
            string __message = ex.Message.ToString();
            if (__message.Contains("The process cannot access the file"))
              return true;
            else
              throw;
          }
        }

        public static string GetFileSize(double byteCount)
        {
          string size = "0 Bytes";
          if (byteCount >= 1073741824.0)
            size = String.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
          else if (byteCount >= 1048576.0)
            size = String.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
          else if (byteCount >= 1024.0)
            size = String.Format("{0:##.##}", byteCount / 1024.0) + " KB";
          else if (byteCount > 0 && byteCount < 1024.0)
            size = byteCount.ToString() + " Bytes";

          return size;
        }

    }

}

