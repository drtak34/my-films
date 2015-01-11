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

using MediaBrowser.ApiInteraction;
//using MediaBrowser.ApiInteraction.net35;
using MediaBrowser.ApiInteraction.net35;
using MediaBrowser.ApiInteraction.WebSocket;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.Users;
using MediaPortal.GUI.Library;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using MediaPortal.Hardware;
using NLog;


namespace MyFilmsPlugin.MyFilms.CatalogConverter
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Xml;
  using System.Globalization;
  using System.Xml.Linq;

  class Mediabrowser
  {
    public Dictionary<string, string> ProfilerDict;
    private static Logger LogMyFilms = LogManager.GetCurrentClassLogger();


    public Mediabrowser()
    {
      ProfilerDict = new Dictionary<string, string>();
      ProfilerDict.Add("CollectionNumber", "Number");
      ProfilerDict.Add("Watched", "Checked");
      ProfilerDict.Add("Barcode", "MediaLabel");
      ProfilerDict.Add("Format", "MediaType");
      ProfilerDict.Add("Title", "OriginalTitle");
      ProfilerDict.Add("TTitle", "TranslatedTitle");
      ProfilerDict.Add("STitle", "FormattedTitle");
      ProfilerDict.Add("Rating", "Rating");
      ProfilerDict.Add("MovieFile", "Source");
      ProfilerDict.Add("IMDB", "URL");
      ProfilerDict.Add("Country", "Country");
      ProfilerDict.Add("Year", "Year");
      ProfilerDict.Add("RunningTime", "Length");
      ProfilerDict.Add("Actors", "Actors");
      ProfilerDict.Add("Genres", "Category");
      ProfilerDict.Add("Credits", "Director");
      ProfilerDict.Add("Credits1", "Producer");
      ProfilerDict.Add("Credits2", "Writer");
      ProfilerDict.Add("Description", "Description");
      ProfilerDict.Add("Comments", "Comments"); // not exported by mymovies - only for merging infos
      ProfilerDict.Add("Picture", "Picture");
      ProfilerDict.Add("Date", "Date");
      //ProfilerDict.Add("Borrower", "Borrower");

      //ProfilerDict.Add("Codec", "VideoFormat");
      //ProfilerDict.Add("Bitrate", "VideoBitrate");
      //ProfilerDict.Add("AudioCodec", "AudioFormat");
      //ProfilerDict.Add("AudioBitRate", "AudioBitrate");
      //ProfilerDict.Add("Resolution", "Resolution");
      //ProfilerDict.Add("FPS", "Framerate");
      ProfilerDict.Add("AudioTracks", "Languages");
      ProfilerDict.Add("Subtitles", "Subtitles");
      //ProfilerDict.Add("Filesize", "Size");
      //ProfilerDict.Add("Disks", "Disks");

      ProfilerDict.Add("ParentalRating/Description", "Certification");
      ProfilerDict.Add("TagLine", "TagLine");
      ProfilerDict.Add("Categories", "Tags");
      ProfilerDict.Add("LocalTrailer/URL", "SourceTrailer");
      //ProfilerDict.Add("watched", "Watched");
      //ProfilerDict.Add("watcheddate", "WatchedDate");
      ProfilerDict.Add("IMDB_Id", "IMDB_Id");
      //ProfilerDict.Add("TMDB_Id", "TMDB_Id");
    }
    public string ConvertMediabrowser(string source, string folderimage, string DestinationTagline, string DestinationTags, string DestinationCertification, string DestinationWriter)
    {
      string mbhost = "XPS-GMI-MAIN";
      
      string wStrPath = System.IO.Path.GetDirectoryName(source);
      string destFile = wStrPath + "\\" + source.Substring(source.LastIndexOf(@"\") + 1, source.Length - source.LastIndexOf(@"\") - 5) + "_tmp.xml";
      var destXml = new XmlTextWriter(destFile, Encoding.Default);
      destXml.Formatting = Formatting.Indented;
      destXml.WriteStartDocument();
      destXml.WriteStartElement("AntMovieCatalog");
      destXml.WriteStartElement("Catalog");
      destXml.WriteElementString("Properties", string.Empty);
      destXml.WriteStartElement("Contents");
      try
      {
        #region mediabrowser infos

        ////After authentication you'll need to set the CurrentUserId property, and you'll need to update that value anytime the user changes (or logs out).
        ////Web Socket

        ////In addition to http requests, you can also connect to the server's web socket to receive notifications of events from the server.

        ////The first thing you will need to do is get the SystemInfo resource to discover what port the web socket is running on.

        ////            var systemInfo = await client.GetSystemInfoAsync();

        ////            var webSocketPort = systemInfo.WebSocketPortNumber;

        ////Then you can simply instantiate ApiWebSocket and open a connection.

        ////            var webSocket = new ApiWebSocket("localhost", webSocketPort, deviceId, appName, appVersion, ClientWebSocketFactory.CreateWebSocket);

        ////The last constructor param is a factory method used to create an instance of IClientWebSocket. This will be called anytime a new connection is made. The full .net ApiClient library includes CilentWebSocketFactory. If using the portable version, you'll have to provide your own implementation.

        ////Once instantiated, simply call EnsureConnectionAsync. Even once connected, this method can be called at anytime to verify connection status and reconnect if needed.

        ////            await webSocket.EnsureConnectionAsync(CancellationToken.None);

        ////There is a Closed event that will fire anytime the connection is lost. From there you can attempt to reconnect. ApiWebSocket also supports the use of a timer to periodically call EnsureConnectionAsync:

        ////            webSocket.StartEnsureConnectionTimer(int intervalMs);

        ////            webSocket.StopEnsureConnectionTimer();

        ////ApiWebSocket has various events that can be used to receive notifications from the server:

        ////            webSocket.UserUpdated += webSocket_UserUpdated;

        ////Linking with ApiClient

        ////ApiClient has a WebSocketConnection property. After creating ApiWebSocket, if you set the property onto ApiClient, some commands will then be sent over the socket as opposed to the http api, resuling in lower overhead. This is optional and omitting this will not result in any loss of functionality with the http-based ApiClient.
        ////Logging and Interfaces

        ////ApiClient and ApiWebSocket both have additional constructors available allowing you to pass in your own implementation of ILogger. The default implementation is NullLogger, which provides no logging. In addition you can also pass in your own implementation of IJsonSerializer, or use our NewtonsoftJsonSerializer. ClientWebSocketFactory also has an additional overload allowing you to pass in your own ILogger
        //#endregion

        //// Developers are encouraged to create their own ILogger implementation
        //var logger = new NullLogger();

        //// This describes the device capabilities
        //var capabilities = new ClientCapabilities();

        //// If using the portable class library you'll need to supply your own IDevice implementation.
        //var device = new Device
        //{
        //    DeviceName = "My Device Name",
        //    DeviceId = "My Device Id"
        //};

        //// If using the portable class library you'll need to supply your own ICryptographyProvider implementation.
        //var cryptoProvider = new CryptographyProvider();

        //var client = new ApiClient(Logger, "http://localhost:8096", "My client name", device, capabilities, cryptoProvider);

        //var authResult = await AuthenticateUserAsync("username", passwordHash);

        //// RemoteLoggedOut indicates the user was logged out remotely by the server
        //ApiClient.RemoteLoggedOut += ApiClient_RemoteLoggedOut;

        //// Get the ten most recently added items for the current user
        //var items = await client.GetItemsAsync(new ItemQuery
        //{
        //    UserId = client.UserId,

        //    SortBy = new[] { ItemSortBy.DateCreated },
        //    SortOrder = SortOrder.Descending,

        //    // Get media only, don't return folder items
        //    Filters = new[] { ItemFilter.IsNotFolder },

        //    Limit = 10,

        //    // Search recursively through the user's library
        //    Recursive = true
        //});

        //await client.Logout();        
        
        
        ApiClient client = new ApiClient(mbhost, 8096, "MyFilms", "HTPC", "My device id", "1.0");

        // Get users
        // UserDto[] users = await client.GetUsersAsync(null);
        var users = client.GetUsers(new Action<UserDto[]>(null), null);

        var currentUser = users.Result.First();

        // Get the ten most recently added items for the current user
        // ItemsResult items = await client.GetItemsAsync(new ItemQuery
        var items = client.GetItemsAsync(new ItemQuery
        {
          UserId = currentUser.Id,

          SortBy = new[] { ItemSortBy.DateCreated },
          SortOrder = SortOrder.Descending,

          // Get media only, don't return folder items
          Filters = new[] { ItemFilter.IsNotFolder },

          Limit = 10,

          // Search recursively through the user's library
          Recursive = true
        });

        foreach (BaseItemDto item in items.Result.Items)
        {
          LogMyFilms.Debug("Mediabrowser - import - ID = '" + item.Id + "', Name = '" + item.Name + "'");
          destXml.WriteStartElement("Movie");
          // string nodeID = item.Id;

          //string nodeID = nodeDVD.Element("ID").Value;
          //XElement nodeNumber = nodeDVD.Element("CollectionNumber");
          //XElement nodeFormat = nodeDVD.Element("Type");
          //XElement nodeLabel = nodeDVD.Element("Barcode");
          //XElement nodeTitle = nodeDVD.Element("LocalTitle");
          //XElement nodeOTitle = nodeDVD.Element("OriginalTitle");
          //XElement nodeSTitle = nodeDVD.Element("SortTitle");
          //XElement nodeYear = nodeDVD.Element("ProductionYear");
          //XElement nodeIMDB = nodeDVD.Element("IMDB");
          //IEnumerable<XElement> LinksKist = nodeDVD.Elements("Discs/Disc");

          string Source = String.Empty;
          foreach (MediaSourceInfo mediaSourceInfo in item.MediaSources)
          {
            if (Source.Length > 0) Source += ";";
            Source += mediaSourceInfo.Path;
          }

          string SourceTrailer = String.Empty;
          foreach (MediaUrl remoteTrailer in item.RemoteTrailers.Where(remoteTrailer => remoteTrailer.IsDirectLink))
          {
            if (SourceTrailer.Length > 0) SourceTrailer += "; ";
            SourceTrailer += remoteTrailer.Url;
          }

          //XElement nodeDuration = nodeDVD.Element("RunningTime");
          //XElement nodeChecked = nodeDVD.Element("Watched");
          //XElement nodeCountry = nodeDVD.Element("Country");
          //XElement nodeOverview = nodeDVD.Element("Description");
          //string Overview = string.Empty;
          //if (nodeOverview != null && nodeOverview.Value != null)
          //  Overview = nodeOverview.Value;

          //string genre = String.Empty;
          //IEnumerable<XElement> genreList = nodeDVD.Elements("Genres/Genre");
          //foreach (XElement nodeGenre in genreList)
          //{
          //  if (genre.Length > 0) genre += ", ";
          //  genre += nodeGenre.Value;
          //}

          //XElement nodeCertification = nodeDVD.Element("ParentalRating/Description");
          //string Certification = string.Empty;
          //if (nodeCertification != null && nodeCertification.Value.Length > 0)
          //  Certification = nodeCertification.Value;

          //XElement nodeTagLine = nodeDVD.Element("TagLine");
          //string Tagline = string.Empty;
          //if (nodeTagLine != null && nodeTagLine.Value.Length > 0)
          //  Tagline = nodeTagLine.Value;

          //string Tags = String.Empty;
          //IEnumerable<XElement> categoryList = nodeDVD.Elements("Categories/Category");
          //foreach (XElement nodeCategory in categoryList)
          //{
          //  if (Tags.Length > 0) Tags += ", ";
          //  Tags += nodeCategory.Value;
          //}
          //string Actor = String.Empty;
          //string Director = String.Empty;
          //string Producer = String.Empty;
          //string Writer = string.Empty;
          //IEnumerable<XElement> creditsList = nodeDVD.Elements("Persons/Person");
          //foreach (XElement nodeCredit in creditsList)
          //{
          //  string line = String.Empty;
          //  if (nodeCredit.Element("Type") != null && nodeCredit.Element("Type").Value == "Actor")
          //  {
          //    if (nodeCredit.Element("Name") != null)
          //      line = nodeCredit.Element("Name").Value;
          //    if ((nodeCredit.Element("Role") != null) && (nodeCredit.Element("Role").Value.Length > 0))
          //      line += " (" + nodeCredit.Element("Role").Value + ")";
          //    if (line.Length > 0)
          //    {
          //      if (Actor.Length > 0) Actor += ", ";
          //      Actor += line;
          //    }
          //  }
          //  else
          //    if (nodeCredit.Element("Type") != null && nodeCredit.Element("Type").Value == "Director")
          //    {
          //      if (nodeCredit.Element("Name") != null)
          //        line = nodeCredit.Element("Name").Value;
          //      if (line.Length > 0)
          //      {
          //        if (Director.Length > 0) Director += ", ";
          //        Director += line;
          //      }
          //    }
          //    else
          //      if (nodeCredit.Element("Type") != null && nodeCredit.Element("Type").Value == "Producer")
          //      {
          //        if (nodeCredit.Element("Name") != null)
          //          line = nodeCredit.Element("Name").Value;
          //        if (line.Length > 0)
          //        {
          //          if (Producer.Length > 0) Producer += ", ";
          //          Producer += line;
          //        }
          //      }
          //      else
          //        if (nodeCredit.Element("Type") != null && nodeCredit.Element("Type").Value == "Writer")
          //        {
          //          if (nodeCredit.Element("Name") != null)
          //            line = nodeCredit.Element("Name").Value;
          //          if (line.Length > 0)
          //          {
          //            if (Writer.Length > 0) Writer += ", ";
          //            Writer += line;
          //          }
          //        }
          //}

          //string languages = string.Empty;
          //IEnumerable<XElement> LanguagesList = nodeDVD.Elements("AudioTracks/AudioTrack");
          //foreach (XElement nodeLanguage in LanguagesList)
          //{
          //  if (nodeLanguage.Attribute("Language") != null && nodeLanguage.Attribute("Language").Value != null)
          //  {
          //    if (languages.Length > 0) languages += ", ";
          //    languages += nodeLanguage.Attribute("Language").Value;
          //    if (nodeLanguage.Attribute("Type").Value != null)
          //      languages += " (" + nodeLanguage.Attribute("Type").Value + ")";
          //  }
          //}

          //string subtitles = String.Empty;
          //IEnumerable<XElement> subtitleList = nodeDVD.Elements("Subtitles/Subtitle");
          //foreach (XElement nodeSubtitle in subtitleList)
          //{
          //  if (nodeSubtitle.Attribute("Language") != null && nodeSubtitle.Attribute("Language").Value != null)
          //  {
          //    if (subtitles.Length > 0) subtitles += ", ";
          //    subtitles += nodeSubtitle.Attribute("Language").Value;
          //  }
          //}

          //string Image = String.Empty;
          //if (nodeDVD.Element("Covers/FrontMedium") != null) // try FrontMedium first, as better resolution
          //  Image = nodeDVD.Element("Covers/FrontMedium").Value;
          //if (string.IsNullOrEmpty(Image) && nodeDVD.Element("Covers/Front") != null) // Front
          //  Image = nodeDVD.Element("Covers/Front").Value;
          //string Rating = string.Empty;
          //decimal wrating = 0;
          //CultureInfo ci = new CultureInfo("en-us");
          //XElement nodeRating = nodeDVD.Element("Rating");
          //if (nodeRating != null && nodeRating.Value != null)
          //{
          //  try { wrating = Convert.ToDecimal(nodeRating.Value); }
          //  catch
          //  {
          //    try { wrating = Convert.ToDecimal(nodeRating.Value, ci); }
          //    catch { }
          //  }
          //}
          //Rating = wrating.ToString("0.0", ci);

          
          
          //if (nodeNumber != null && !string.IsNullOrEmpty(nodeNumber.Value))
          WriteAntAtribute(destXml, "CollectionNumber", item.Id);
          //else
          //  WriteAntAtribute(destXml, "CollectionNumber", "9999");
          
          //if (nodeOTitle != null && nodeOTitle.Value.Length > 0)
          WriteAntAtribute(destXml, "Title", item.Name);
          //else
          //  WriteAntAtribute(destXml, "Title", nodeTitle.Value);
          
          WriteAntAtribute(destXml, "TTitle", item.Name);
          
          //if (nodeSTitle != null && nodeSTitle.Value.Length > 0)
          WriteAntAtribute(destXml, "STitle", item.SortName);
          //else
          //  WriteAntAtribute(destXml, "STitle", nodeTitle.Value);
          
          //XElement nodeDate = nodeDVD.Element("Added");

          //string strDateAdded = string.Empty;
          //IFormatProvider culture = new CultureInfo("en-US", true);
          //if (nodeDate != null && nodeDate.Value.Length > 0)
          //{
          //  strDateAdded = nodeDate.Value.ToString();
          //  if (strDateAdded.Contains(" "))
          //    strDateAdded = strDateAdded.Substring(0, strDateAdded.IndexOf(" ")); // Remove time...
          //}

          //try
          //{
          //  DateTime dt = new DateTime();
          //  //dt = DateTime.Parse(strDateAdded);
          //  dt = DateTime.Parse(strDateAdded, culture, DateTimeStyles.NoCurrentDateDefault);
          //  //dt = DateTime.ParseExact(strDateAdded, "MM/dd/yyyy hh:mm:ss tt", culture, DateTimeStyles.NoCurrentDateDefault);
          //  WriteAntAtribute(destXml, "Date", dt.ToShortDateString());
          //}
          //catch
          //{
          //}

          //if (nodeChecked != null && nodeChecked.Value.Length > 0 && nodeChecked.Value.ToLower() == "true")
          //{
          //  WriteAntAtribute(destXml, "Watched", "true");
          //}
          //else
          //{
          //  WriteAntAtribute(destXml, "Watched", "false");
          //}

          //if (nodeCountry != null)
          //  WriteAntAtribute(destXml, "Country", nodeCountry.Value);
          //WriteAntAtribute(destXml, "Rating", Rating);
          //if (nodeYear != null)
          //  WriteAntAtribute(destXml, "Year", nodeYear.Value);
          //if (nodeDuration != null && nodeDuration.Value.Length > 0)
          //  WriteAntAtribute(destXml, "RunningTime", nodeDuration.Value);
          //if (nodeFormat != null)
          //  WriteAntAtribute(destXml, "Format", nodeFormat.Value);
          //if (nodeLabel != null)
          //  WriteAntAtribute(destXml, "Barcode", nodeLabel.Value);

          //WriteAntAtribute(destXml, "Genres", genre);
          //WriteAntAtribute(destXml, "Credits", Director);
          //WriteAntAtribute(destXml, "Credits1", Producer);
          ////WriteAntAtribute(destXml, "Credits2", Writer);
          //WriteAntAtribute(destXml, "Actors", Actor);

          //WriteAntAtribute(destXml, "AudioTracks", languages);
          //WriteAntAtribute(destXml, "Subtitles", subtitles);
          ////WriteAntAtribute(destXml, "ParentalRating/Description", Certification);
          ////WriteAntAtribute(destXml, "TagLine", Tagline);
          ////WriteAntAtribute(destXml, "Categories", Tags);
          //if (nodeIMDB != null)
          //{
          //  WriteAntAtribute(destXml, "IMDB", nodeIMDB.Value); // goes into "URL"
          //  WriteAntAtribute(destXml, "IMDB_Id", nodeIMDB.Value); // goes into "IMDB_Id" field
          //}
          WriteAntAtribute(destXml, "MovieFile", Source);
          WriteAntAtribute(destXml, "LocalTrailer/URL", SourceTrailer);

          //string DescriptionMerged = string.Empty;
          //if (DestinationTagline == "Description")
          //{
          //  if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
          //  DescriptionMerged += Tagline;
          //}
          //if (DestinationTags == "Description")
          //{
          //  if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
          //  DescriptionMerged += Tags;
          //}
          //if (DestinationCertification == "Description")
          //{
          //  if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
          //  DescriptionMerged += Certification;
          //}
          //if (Overview.Length > 0)
          //{
          //  if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
          //  DescriptionMerged += Overview;
          //}
          //WriteAntAtribute(destXml, "Description", DescriptionMerged);

          //string CommentsMerged = string.Empty;
          //if (DestinationTagline == "Comments")
          //{
          //  if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
          //  CommentsMerged += Tagline;
          //}
          //if (DestinationTags == "Comments")
          //{
          //  if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
          //  CommentsMerged += Tags;
          //}
          //if (DestinationCertification == "Comments")
          //{
          //  if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
          //  CommentsMerged += Certification;
          //}
          //WriteAntAtribute(destXml, "Comments", CommentsMerged);

          //WriteAntAtribute(destXml, "Picture", Image);

          //// Now writing MF extended attributes
          //WriteAntElement(destXml, "ParentalRating/Description", Certification);
          //WriteAntElement(destXml, "TagLine", Tagline);
          //WriteAntElement(destXml, "Categories", Tags);
          //WriteAntElement(destXml, "Credits2", Writer);

          destXml.WriteEndElement();
        }

      }
      catch (Exception ex)
      {
        LogMyFilms.Debug("Mediabrowser Import Filter Error - " + ex.Message);
        return string.Empty;
      }
      destXml.WriteEndElement();
      destXml.WriteEndElement();
      destXml.Close();
      return destFile;
    }

    private void WriteAntAtribute(XmlTextWriter tw, string key, string value)
    {
      string at = string.Empty;
      if (ProfilerDict.TryGetValue(key, out at))
      {
        tw.WriteAttributeString(at, value);
      }
    }

    private void WriteAntElement(XmlWriter tw, string key, string value)
    {
      string at = string.Empty;
      if (ProfilerDict.TryGetValue(key, out at))
      {
        tw.WriteElementString(at, value);
        //LogMyFilms.Debug("XMM Importer: Writing Property '" + key + "' with Value '" + value.ToString() + "' to DB.");
      }
      else
      {
        //LogMyFilms.Debug("XMM Importer Property '" + key + "' not found in dictionary ! - Element not written to DB !");
      }
    }

  }

}
