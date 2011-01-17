﻿namespace MyFilmsPlugin.MyFilms.Utils {
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Threading;

  using MediaPortal.GUI.Library;

  class Utility {

        #region Ctor / Private variables
        
       
        private Utility() {

        }

        #endregion        

        #region Enums

        public enum VideoDiscType {
            [Description(@"\video_ts\video_ts.ifo")]
            DVD,
            [Description(@"\bdmv\index.bdmv")]
            Bluray,
            [Description(@"\adv_obj\discid.dat")] // or adv_obj\vplst000.xpl ?
            HDDVD,
            [Description("")]
            UnknownFormat
        }

        #endregion

        #region Enum Helper Methods

        public static List<T> EnumToList<T>() {
            Type enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            return new List<T>(Enum.GetValues(enumType) as IEnumerable<T>);
        }

        public static string GetEnumValueDescription(object value) {
            Type objType = value.GetType();
            FieldInfo fieldInfo = objType.GetField(Enum.GetName(objType, value));
            DescriptionAttribute attribute = (DescriptionAttribute)
            (fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)[0]);

            // Return the description.
            return attribute.Description;
        }

        #endregion

        #region FileSystem Methods

        /// <summary>
        /// Get all files from directory and it's subdirectories.
        /// </summary>
        /// <param name="inputDir"></param>
        /// <returns></returns>
        public static List<FileInfo> GetFilesRecursive(DirectoryInfo directory) {
            List<FileInfo> fileList = new List<FileInfo>();
            DirectoryInfo[] subdirectories = new DirectoryInfo[] { };

            try {
                fileList.AddRange(directory.GetFiles("*"));
                subdirectories = directory.GetDirectories();
            }
            catch (Exception e) {
                if (e.GetType() == typeof(ThreadAbortException))
                    throw e;

                Log.Error("Error while retrieving files/directories for: " + directory.FullName, e);
            }

            foreach (DirectoryInfo subdirectory in subdirectories) {
                try {
                    if ((subdirectory.Attributes & FileAttributes.System) == 0)
                        fileList.AddRange(GetFilesRecursive(subdirectory));
                    else
                        Log.Debug("Rejecting directory " + subdirectory.FullName + " because it is flagged as a System folder.");
                }
                catch (Exception e) {
                    if (e.GetType() == typeof(ThreadAbortException))
                        throw e;
                    Log.Error("Error during attribute check for: " + subdirectory.FullName, e);
                }
            }

            return fileList;
        }
        
        /// <summary>
        /// Get all video files from directory and it's subdirectories.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static List<FileInfo> GetVideoFilesRecursive(DirectoryInfo directory) {
            List<FileInfo> fileList = GetFilesRecursive(directory);
            List<FileInfo> videoFileList = new List<FileInfo>();
            foreach (FileInfo file in fileList) {
                if (Utility.IsVideoFile(file))
                    videoFileList.Add(file);
            }
            return videoFileList;
        }
        
        /// <summary>
        /// This method will create a string that can be safely used as a filename.
        /// </summary>
        /// <param name="subject">the string to process</param>
        /// <returns>the processed string</returns>
        public static string CreateFilename(string subject) {
            if (String.IsNullOrEmpty(subject))
                return string.Empty;

            string rtFilename = subject;

            char[] invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char invalidFileChar in invalidFileChars)
                rtFilename = rtFilename.Replace(invalidFileChar, '_');

            return rtFilename;
        }

        /// <summary>
        /// Removes the extension from a filename
        /// @todo: remove this method
        /// </summary>
        /// <param name="file"></param>
        /// <returns>filename without extension</returns>
        public static string RemoveFileExtension(FileInfo file) {
            return Path.GetFileNameWithoutExtension(file.Name);
        }

        public static string RemoveFileExtension(string filename) {
            return RemoveFileExtension(new FileInfo(filename));
        }

        /// <summary>
        /// Remove extension and stackmarkers from a filename
        /// </summary>
        /// <param name="file">target file</param>
        /// <returns>the filename without stackmarkers and extension</returns>
        public static string RemoveFileStackMarkers(FileInfo file) {
            // Remove the file extension from the filename
            string fileName = RemoveFileExtension(file);

            // If file is classified as multipart clean the stack markers.
            if (isFileMultiPart(fileName)) {
                Regex expr = new Regex(rxFileStackMarkers, RegexOptions.IgnoreCase);
                Match match = expr.Match(fileName);
                // if we have a match on this expression we will remove the complete match.
                if (match.Success)
                    fileName = expr.Replace(fileName, "");
                // no match means we just remove one character
                else
                    fileName = fileName.Substring(0, (fileName.Length - 1));
            }

            // Return the cleaned filename
            return fileName;
        }

        public static string RemoveFileStackMarkers(string filename) {
            return RemoveFileStackMarkers(new FileInfo(filename));
        }

        // Regular expression patterns used by the multipart detection and cleaning methods
        private const string rxFileStackMarkers = @"([\s\-]*(cd|disk|disc|part)[\s\-]*([a-c]|\d+|i+))|[\(\[]\d(of|-)\d[\)\]]$";

        /// <summary>
        /// Checks if a file has stack markers (and is multi-part)
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true if multipart, false if not</returns>
        public static bool isFileMultiPart(FileInfo file) {
            return isFileMultiPart(RemoveFileExtension(file));
        }

        /// <summary>
        /// Checks if a filename has stack markers (and is multi-part)
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>true if multipart, false if not</returns>
        public static bool isFileMultiPart(string filename) {
            Regex expr = new Regex(rxFileStackMarkers + @"|[^\s\d](\d+)$|([a-c])$", RegexOptions.IgnoreCase);
            return expr.Match(filename).Success;
        }

        #endregion

        #region String Modification / Regular Expressions Methods

        // Regular expression pattern that matches an "article" that need to be moved for title conversions
        // todo: the articles should really be a user definable setting in the future
        private const string rxTitleSortPrefix = "(the|a|an|ein|das|die|der|les|la|le|el|une|de|het)";

        /// <summary>
        /// Converts a movie title to the display name.
        /// </summary>
        /// <example>
        /// Changes "Movie, The" into "The Movie"
        /// </example>
        /// <param name="title"></param>
        /// <returns>display name</returns>
        public static string TitleToDisplayName(string title) {
            Regex expr = new Regex(@"(.+?)(?:, " + rxTitleSortPrefix + @")?\s*$", RegexOptions.IgnoreCase);
            return expr.Replace(title, "$2 $1").Trim();
        }

        /// <summary>
        /// Converts a title to the archive name (sortable title)
        /// </summary>
        /// <example>
        /// Changes "The Movie" into "Movie, The"
        /// </example>
        /// <param name="title"></param>
        /// <returns>archive name</returns>
        public static string TitleToArchiveName(string title) {
            Regex expr = new Regex(@"^" + rxTitleSortPrefix + @"\s(.+)", RegexOptions.IgnoreCase);
            return expr.Replace(title, "$2, $1").Trim();
        }

        /// <summary>
        /// Converts a title string to a common format to be used in comparison.
        /// </summary>
        /// <param name="title">the original title</param>
        /// <returns>the normalized title</returns>
        public static string normalizeTitle(string title) {
            // Convert title to lowercase culture invariant
            string newTitle = title.ToLowerInvariant();

            // Swap article
            newTitle = TitleToDisplayName(newTitle);

            // Replace non-descriptive characters with spaces
            newTitle = Regex.Replace(newTitle, @"[\.:;\+\-\–\—\―\˜\*]", " ");

            // Remove other non-descriptive characters completely
            newTitle = Regex.Replace(newTitle, @"[\(\)\[\]'`,""\#\$\?]", "");

            // Equalize: Convert to base character string
            newTitle = RemoveDiacritics(newTitle);

            // Equalize: Common characters with words of the same meaning
            newTitle = Regex.Replace(newTitle, @"\b(and|und|en|et|y)\b", " & ");

            // Equalize: Roman Numbers To Numeric
            newTitle = Regex.Replace(newTitle, @"\si(\b)", @" 1$1");
            newTitle = Regex.Replace(newTitle, @"\sii(\b)", @" 2$1");
            newTitle = Regex.Replace(newTitle, @"\siii(\b)", @" 3$1");
            newTitle = Regex.Replace(newTitle, @"\siv(\b)", @" 4$1");
            newTitle = Regex.Replace(newTitle, @"\sv(\b)", @" 5$1");
            newTitle = Regex.Replace(newTitle, @"\svi(\b)", @" 6$1");
            newTitle = Regex.Replace(newTitle, @"\svii(\b)", @" 7$1");
            newTitle = Regex.Replace(newTitle, @"\sviii(\b)", @" 8$1");
            newTitle = Regex.Replace(newTitle, @"\six(\b)", @" 9$1");

            // Remove the number 1 from the end of a title string
            newTitle = Regex.Replace(newTitle, @"\s(1)$","");

            // Remove double spaces and trim
            newTitle = trimSpaces(newTitle);

            // return the cleaned title
            return newTitle;
        }

        /// <summary>
        /// Translates characters to their base form.
        /// </summary>
        /// <example>
        /// characters: ë, é, è
        /// result: e
        /// </example>
        /// <remarks>
        /// source: http://blogs.msdn.com/michkap/archive/2007/05/14/2629747.aspx
        /// </remarks>
        public static string RemoveDiacritics(string title) {
            string stFormD = title.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++) {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark) {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }


        /// <summary>
        /// Removes multiple spaces and replaces them with one space   
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string trimSpaces(string input) {
            return Regex.Replace(input, @"\s{2,}", " ").Trim();
        }

        #endregion

        #region General Methods (Unsorted)


        /// <summary>
        /// Checks if the foldername is a multipart marker.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool isFolderMultipart(string name) {
            Regex expr = new Regex(@"^(cd|dvd)\s*\d+$", RegexOptions.IgnoreCase);
            return expr.Match(name).Success;
        }

        /// <summary>
        /// Checks if the foldername is ambigious (non descriptive)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool isFolderAmbiguous(string name) {
            string[] folders = new string[] { 
                "video_ts", "hvdvd_ts", "adv_obj", "bdmv", 
                "stream", "playlist", "clipinf", "backup"
            };

            // Name is too short or is marked as being multi-part
            if (name.Length == 1 || isFolderMultipart(name))
                return true;

            // Ignore specific names
            foreach (string folderName in folders) {
                if (name.Equals(folderName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the file is a valid video file.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static bool IsVideoFile(FileInfo fileInfo) {
            string fullPath = fileInfo.FullName;
            // Video Disc Standards Are OK
            if (IsVideoDiscPath(fullPath))
                return true;

            // Check files that pass the MediaPortal Video Extension list
            if (IsMediaPortalVideoFile(fileInfo)) {
                string ext = fileInfo.Extension.ToLower();
                string name = fileInfo.Name.ToLower(); ;

                // DVD: Non-Standalone content is invalid
                if (ext == ".vob" && Regex.Match(name, @"(video_ts|vts_).+", RegexOptions.IgnoreCase).Success)
                     return false;

                // DVD: Filter ifo's that are not called video_ts.ifo
                // but allow them when we don't have a video_ts.ifo in the same folder
                if (ext == ".ifo" && name != "video_ts.ifo")
                    if (File.Exists(fileInfo.FullName.ToLower().Replace(name, "video_ts.ifo")))
                        return false;
               
                // Bluray: the only valid bluray file would already passed the method, we filter the rest
                if (ext == ".bdmv")
                    return false;

                // Bluray: m2ts files sitting in a stream folder are part of a bluray disc
                if (ext == ".m2ts" && fileInfo.Directory.Name.Equals("stream", StringComparison.OrdinalIgnoreCase))
                    return false;

                // HD-DVD: evo files sitting in a hvdvd_ts folder are part of a hddvd disc
                if (ext == ".evo" && fileInfo.Directory.Name.Equals("hvdvd_ts", StringComparison.OrdinalIgnoreCase))
                    return false;

                // HD-DVD: .dat files other than discid.dat should be ignored
                if (ext == ".dat" && name != "discid.dat")
                    return false;

                // if we made it this far we have a winner
                return true;
            }

            // we did not pass so return false
            return false;
        }

        /// <summary>
        /// Returns the Video Disc Type
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static VideoDiscType GetVideoDiscType(string path) {
            foreach (VideoDiscType format in EnumToList<VideoDiscType>()) {
                if (format != VideoDiscType.UnknownFormat) {
                    if (path.EndsWith(GetEnumValueDescription(format),StringComparison.OrdinalIgnoreCase))
                        return format;
                }
            }
            return VideoDiscType.UnknownFormat;
        }

        /// <summary>
        /// Check if the path specified is a video disc standard
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsVideoDiscPath(string path) {
            foreach(VideoDiscType format in EnumToList<VideoDiscType>()) {
                if (format != VideoDiscType.UnknownFormat)
                    if (path.EndsWith(GetEnumValueDescription(format), StringComparison.OrdinalIgnoreCase))
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the full path to the video disc or null if it doesn't find one.
        /// </summary>
        /// <param name="drive"></param>
        /// <returns></returns>
        public static string GetVideoDiscPath(string drive) {
           FileInfo discPath;
           foreach (VideoDiscType format in EnumToList<VideoDiscType>()) {
               if (format != VideoDiscType.UnknownFormat) {
                   discPath = new FileInfo(drive + GetEnumValueDescription(format));
                   discPath.Refresh();
                   if (discPath.Exists)
                       return discPath.FullName;
               }
           }
           return null;
        }

        /// <summary>
        /// Checks if the directory is a logical drive root
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if this is a root</returns>
        public static bool IsDriveRoot(DirectoryInfo directory) {
            return IsDriveRoot(directory.FullName);
        }

        /// <summary>
        /// Checks if the given path is a logical drive root
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if this is a root</returns>
        public static bool IsDriveRoot(string path) {
            if (path.Length < 4)
                return (path.Substring(1, 1) == ":");
            
            return false;
        }

        /// <summary>
        /// Returns the base directory for the movie files that are part of the input directory
        /// </summary>
        /// <param name="directory">directory to start in</param>
        /// <returns>the base directory for the movie files</returns>
        public static DirectoryInfo GetMovieBaseDirectory(DirectoryInfo directory) {
            DirectoryInfo dirLevel = directory;
            while (isFolderAmbiguous(dirLevel.Name) && dirLevel.Root != dirLevel)
                dirLevel = dirLevel.Parent;

            return dirLevel;
        }

        /// <summary>
        /// Check if the file is classified as sample
        /// </summary>
        /// <param name="file">file to check</param>
        /// <returns>True if file is a sample file</returns>
        //public static bool isSampleFile(FileInfo file) {
        //    // Set sample max size in bytes
        //    long sampleMaxSize = long.Parse(MovingPicturesCore.SettingsManager["importer_sample_maxsize"].Value.ToString()) * 1024 * 1024;
        //    // Create the sample filter regular expression
        //    Regex expr = new Regex(MovingPicturesCore.SettingsManager["importer_sample_keyword"].Value.ToString(), RegexOptions.IgnoreCase);
        //    // Return result of given conditions         
        //    return ((file.Length < sampleMaxSize) && expr.Match(file.Name).Success);
        //}

        ///// <summary>
        ///// Get the number of video files (excluding sample files) that are in a folder
        ///// </summary>
        ///// <param name="folder">the directory to count video files in</param>
        ///// <returns>total number of files found in the folder</returns>
        //public static int GetVideoFileCount(DirectoryInfo folder) {
        //    int rtn = 0;
        //    FileInfo[] fileList = folder.GetFiles("*");
        //    foreach (FileInfo currFile in fileList) {
        //        // count the number of non-sample video files in the folder
        //        if (IsVideoFile(currFile))
        //            if (!isSampleFile(currFile))
        //                rtn++;
        //    }

        //    // Return count
        //    return rtn;
        //}       

        ///// <summary>
        ///// Checks if a folder contains a maximum amount of video files
        ///// This is used to determine if a folder is dedicated to one movie
        ///// </summary>
        ///// <param name="folder">the directory to check</param>
        ///// <param name="expectedCount">maximum count</param>
        ///// <returns>True if folder is dedicated</returns>
        //public static bool isFolderDedicated(DirectoryInfo folder, int expectedCount) {
        //    return (GetVideoFileCount(folder) <= expectedCount);
        //}

        #endregion

        #region MediaPortal

        /// <summary>
        /// Checks if file has valid video extensions (as specified by media portal
        /// </summary>
        /// <remarks>
        /// MediaPortal Dependency!
        /// </remarks>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsMediaPortalVideoFile(FileInfo file) {
            foreach (string ext in MediaPortal.Util.Utils.VideoExtensions) {
                if (file.Extension.Equals(ext, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        #endregion

        //#region DirectShowLib

        ///// <summary>
        ///// Get Disc ID as a string
        ///// </summary>
        ///// <param name="path">CD/DVD path</param>
        ///// <returns>Disc ID</returns>
        //public static string GetDiscIdString(string path) {
        //    long id = GetDiscId(path);
        //    if (id != 0)
        //        return Convert.ToString(id, 16); // HEX

        //    return null;
        //}

        ///// <summary>
        ///// Get Disc ID as 64-bit signed integer
        ///// </summary>
        ///// <param name="path">CD/DVD path</param>
        ///// <returns>Disc ID</returns>
        //public static long GetDiscId(string path) {
        //    long discID = 0;
        //    Log.Debug("Generating DiscId for: " + path);
        //    try {
        //        IDvdInfo2 dvdInfo = (IDvdInfo2)new DVDNavigator();
        //        dvdInfo.GetDiscID(path, out discID);
        //    }
        //    catch (Exception e) {
        //        Log.Error("Error while retrieving disc id for: " + path, e);
        //    }
        //    return discID;
        //}

        //#endregion

        #region MovieHash
        
        public static string GetMovieHashString(string filename) {
            string hash;
            try {
                Log.Debug("Generating FileHash for: " + filename);
                byte[] moviehash = ComputeMovieHash(filename);
                hash = ToHexadecimal(moviehash);
            }
            catch (Exception e) {
                Log.Error("Error while generating FileHash for: " + filename, e);
                hash = null;
            }
            return hash;
        }
        
        private static byte[] ComputeMovieHash(string filename) {
            byte[] result;
            using (Stream input = File.OpenRead(filename)) {
                result = ComputeMovieHash(input);
            }
            return result;
        }

        private static byte[] ComputeMovieHash(Stream input) {
            ulong lhash;
            long streamsize;
            streamsize = input.Length;
            lhash = (ulong)streamsize;

            long i = 0;
            byte[] buffer = new byte[sizeof(long)];
            input.Position = 0;
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0)) {
                i++;
                unchecked { lhash += BitConverter.ToUInt64(buffer, 0); }
            }

            input.Position = Math.Max(0, streamsize - 65536);
            i = 0;
            while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0)) {
                i++;
                unchecked { lhash += BitConverter.ToUInt64(buffer, 0); }
            }
            byte[] result = BitConverter.GetBytes(lhash);
            Array.Reverse(result);
            return result;
        }

        private static string ToHexadecimal(byte[] bytes) {
            StringBuilder hexBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) {
                hexBuilder.Append(bytes[i].ToString("x2"));
            }
            return hexBuilder.ToString();
        }

        #endregion


    }
}
