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

namespace MyFilmsPlugin.MyFilms.CatalogConverter
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Xml;
  using System.Globalization;

  class XMM
    {
        public Dictionary<string, string> ProfilerDict;

        public XMM()
        {
            ProfilerDict = new Dictionary<string, string>();
            ProfilerDict.Add("MovieID", "Number");
            ProfilerDict.Add("Seen", "Checked");
            ProfilerDict.Add("MediaLabel", "MediaLabel");
            ProfilerDict.Add("Media", "MediaType");
            ProfilerDict.Add("MovieFile", "Source");
            ProfilerDict.Add("DateInsert", "Date");
            //ProfilerDict.Add("Borrower", "Borrower");
            ProfilerDict.Add("Rating", "Rating");
            ProfilerDict.Add("Title", "OriginalTitle");
            ProfilerDict.Add("TTitle", "TranslatedTitle");
            ProfilerDict.Add("STitle", "FormattedTitle");
            ProfilerDict.Add("Director", "Director");
            ProfilerDict.Add("Producer", "Producer");
            ProfilerDict.Add("Country", "Country");
            ProfilerDict.Add("Genre", "Category");
            ProfilerDict.Add("Year", "Year");
            ProfilerDict.Add("Length", "Length");
            ProfilerDict.Add("Actors", "Actors");
            ProfilerDict.Add("InternetID", "URL");
            ProfilerDict.Add("Plot", "Description");
            ProfilerDict.Add("Comments", "Comments");
            ProfilerDict.Add("Codec", "VideoFormat");
            ProfilerDict.Add("Bitrate", "VideoBitrate");
            ProfilerDict.Add("AudioCodec", "AudioFormat");
            ProfilerDict.Add("AudioBitRate", "AudioBitrate");
            ProfilerDict.Add("Resolution", "Resolution");
            ProfilerDict.Add("FPS", "Framerate");
            ProfilerDict.Add("Language", "Languages");
            ProfilerDict.Add("Subtitles", "Subtitles");
            ProfilerDict.Add("Filesize", "Size");
            ProfilerDict.Add("Disks", "Disks");
            ProfilerDict.Add("Cover", "Picture");

            //int Number, 
            //string Checked, 
            //string MediaLabel, 
            //string MediaType, 
            //string Source, 
            //string Date, 
            //string Borrower, 
            //decimal Rating, 
            //string OriginalTitle, 
            //string TranslatedTitle, 
            //string FormattedTitle, 
            //string Director, 
            //string Producer, 
            //string Country, 
            //string Category, 
            //string Year, 
            //string Length, 
            //string Actors, 
            //string URL, 
            //string Description, 
            //string Comments, 
            //string VideoFormat, 
            //string VideoBitrate, 
            //string AudioFormat, 
            //string AudioBitrate, 
            //string Resolution, 
            //string Framerate, 
            //string Languages, 
            //string Subtitles, 
            //string Size, 
            //string Disks, 
            //string Picture
        }
        public string ConvertXMM(string source, string folderimage, bool SortTitle, bool OnlyFile)
        {
            string WStrPath = System.IO.Path.GetDirectoryName(source);
            string destFile = WStrPath + "\\" + source.Substring(source.LastIndexOf(@"\") + 1, source.Length - source.LastIndexOf(@"\") - 5) + "_tmp.xml";
            XmlTextWriter destXml = new XmlTextWriter(destFile, Encoding.Default);
            destXml.Formatting = Formatting.Indented;
            destXml.WriteStartDocument();
            destXml.WriteStartElement("AntMovieCatalog");
            destXml.WriteStartElement("Catalog");
            destXml.WriteElementString("Properties", string.Empty);
            destXml.WriteStartElement("Contents");
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(source);
                XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/XMM_Movie_Database/Movie");
                foreach (XmlNode nodeDVD in dvdList)
                {
                    destXml.WriteStartElement("Movie");

                    //int Number, 
                    XmlNode nodeID = nodeDVD.SelectSingleNode("ID");
                    XmlNode nodeNumber = nodeDVD.SelectSingleNode("MovieID");
                    if (nodeNumber != null && !string.IsNullOrEmpty(nodeNumber.InnerText))
                        WriteAntAtribute(destXml, "MovieID", nodeNumber.InnerText);
                    else
                        WriteAntAtribute(destXml, "MovieID", "9999");

                    //string Checked, 
                    XmlNode nodeChecked = nodeDVD.SelectSingleNode("Seen");
                    if (nodeChecked != null && nodeChecked.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Seen", nodeChecked.InnerText);

                    //string MediaLabel, 
                    XmlNode nodeMediaLabel = nodeDVD.SelectSingleNode("MediaLabel");
                    if (nodeMediaLabel != null && nodeMediaLabel.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "MediaLabel", nodeMediaLabel.InnerText);

                    //string MediaType, 
                    XmlNode nodeMediaType = nodeDVD.SelectSingleNode("Media");
                    if (nodeMediaType != null && nodeMediaType.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Media", nodeMediaType.InnerText);

                    //string Source, 
                    string strSource = String.Empty;
                    for (int i = 1; i < 7; i++)
                    {
                        XmlNode nodeSource = nodeDVD.SelectSingleNode("MovieFile" + i.ToString());
                        if (strSource.Length > 0 && nodeSource.InnerText.Length > 0) 
                            strSource += ";";
                        if (nodeSource != null && nodeSource.InnerText.Length > 0)
                            strSource += nodeSource.InnerText;
                    }
                    WriteAntAtribute(destXml, "MovieFile", strSource);

                    //string Date, 
                    XmlNode nodeDate = nodeDVD.SelectSingleNode("DateInsert");
                    try
                    {
                        DateTime dt = new DateTime();
                        dt = DateTime.Parse(nodeDate.InnerText.ToString());
                        WriteAntAtribute(destXml, "DateInsert", dt.ToShortDateString());
                    }
                    catch
                    {
                    }

                    //string Borrower, 

                    //decimal Rating, 
                    string Rating = string.Empty;
                    decimal wrating = 0;
                    CultureInfo ci = new CultureInfo("en-us");
                    XmlNode nodeRating = nodeDVD.SelectSingleNode("Rating");
                    if (nodeRating != null && nodeRating.InnerText != null)
                    {
                        try { wrating = Convert.ToDecimal(nodeRating.InnerText); }
                        catch
                        {
                            try { wrating = Convert.ToDecimal(nodeRating.InnerText, ci); }
                            catch { }
                        }
                    }
                    Rating = wrating.ToString("0.0", ci);
                    WriteAntAtribute(destXml, "Rating", Rating);

                    //string OriginalTitle, 
                    //string TranslatedTitle, 
                    //string FormattedTitle, 
                    XmlNode nodeOTitle = nodeDVD.SelectSingleNode("OriginalTitle");
                    XmlNode nodeTitle = nodeDVD.SelectSingleNode("Title");
                    XmlNode nodeSTitle = nodeDVD.SelectSingleNode("SortTitle");
                    if (nodeOTitle != null && nodeOTitle.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Title", nodeOTitle.InnerText);
                    else
                        WriteAntAtribute(destXml, "Title", nodeTitle.InnerText);
                    WriteAntAtribute(destXml, "TTitle", nodeTitle.InnerText);
                    if (nodeSTitle != null && nodeSTitle.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "STitle", nodeSTitle.InnerText);
                    else
                        WriteAntAtribute(destXml, "STitle", nodeTitle.InnerText);

                    //string Director, 
                    XmlNode nodeDirector = nodeDVD.SelectSingleNode("Director");
                    if (nodeDirector != null && nodeDirector.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Director", nodeDirector.InnerText);

                    //string Producer, 
                    XmlNode nodeProducer = nodeDVD.SelectSingleNode("Producer");
                    if (nodeProducer != null && nodeProducer.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Producer", nodeProducer.InnerText);

                    //string Country, 
                    XmlNode nodeCountry = nodeDVD.SelectSingleNode("Country");
                    if (nodeCountry != null && nodeCountry.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Country", nodeCountry.InnerText);

                    //string Category, 
                    string genre = String.Empty;
                    XmlNode genreGenre = nodeDVD.SelectSingleNode("Genre");
                    XmlNode genreSubgenre = nodeDVD.SelectSingleNode("Subgenre");
                    XmlNode genreCategory = nodeDVD.SelectSingleNode("Category");
                    if (genreGenre != null && genreGenre.InnerText.Length > 0)
                        genre = genreGenre.InnerText;
                    if (genreSubgenre != null && genreSubgenre.InnerText.Length > 0)
                    {
                        foreach (string subgenre in genreSubgenre.InnerText.Split(new Char[] { '/' }))
                        {
                            if (genre.Length > 0) genre += ", ";
                            genre += subgenre;
                        }
                    }
                    if (genre.Length == 0 && genreCategory != null && genreCategory.InnerText.Length > 0)
                        genre = genreCategory.InnerText;
                    //XmlNodeList genreList = nodeDVD.SelectNodes("Genres/Genre");
                    //foreach (XmlNode nodeGenre in genreList)
                    //{
                    //    if (genre.Length > 0) genre += ", ";
                    //    genre += nodeGenre.InnerText;
                    //}
                    WriteAntAtribute(destXml, "Genre", genre);

                    //string Year, 
                    XmlNode nodeYear = nodeDVD.SelectSingleNode("Year");
                    if (nodeYear != null)
                        WriteAntAtribute(destXml, "Year", nodeYear.InnerText);

                    //string Length, 
                    int strLengthnum = 0;
                    XmlNode nodeDuration = nodeDVD.SelectSingleNode("Length");

                    try
                    { strLengthnum = Int32.Parse(nodeDuration.InnerText); }
                    catch
                    { strLengthnum = 0; }
                    if (nodeDuration != null && nodeDuration.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Length", strLengthnum.ToString());

                    //string Actors, 
                    string Actor = String.Empty;
                    XmlNodeList creditsList = nodeDVD.SelectNodes("Actors/Actor");
                    foreach (XmlNode nodeCredit in creditsList)
                    {
                        string line = String.Empty;
                        if (nodeCredit.SelectSingleNode("ActorName") != null)
                            line = nodeCredit.SelectSingleNode("ActorName").InnerText;
                        if ((nodeCredit.SelectSingleNode("ActorRole") != null) && (nodeCredit.SelectSingleNode("ActorRole").InnerText.Length > 0))
                            line += " (" + nodeCredit.SelectSingleNode("ActorRole").InnerText + ")";
                        if (line.Length > 0)
                        {
                            if (Actor.Length > 0) Actor += ", ";
                            Actor += line;
                        }
                    }
                    WriteAntAtribute(destXml, "Actors", Actor);

                    //string URL, 
                    string strURL = String.Empty;
                    XmlNode nodeURL = nodeDVD.SelectSingleNode("InternetID");
                    if (nodeURL != null && nodeURL.InnerText != null)
                        strURL = @"http://www.imdb.com/title/tt0" + nodeURL.InnerText;
                        WriteAntAtribute(destXml, "InternetID", strURL);

                    //string Description, 
                    XmlNode nodePlot = nodeDVD.SelectSingleNode("Plot");
                    if (nodePlot != null && nodePlot.InnerText != null)
                        WriteAntAtribute(destXml, "Plot", nodePlot.InnerText);

                    //string Comments, 
                    XmlNode nodeComments = nodeDVD.SelectSingleNode("Comments");
                    if (nodeComments != null && nodeComments.InnerText != null)
                        WriteAntAtribute(destXml, "Comments", nodeComments.InnerText);

                    //string VideoFormat, Codec
                    XmlNode nodeVideoFormat = nodeDVD.SelectSingleNode("Codec");
                    if (nodeVideoFormat != null && nodeVideoFormat.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Codec", nodeVideoFormat.InnerText);

                    //string VideoBitrate, Bitrate 
                    XmlNode nodeVideoBitrate = nodeDVD.SelectSingleNode("Bitrate");
                    if (nodeVideoBitrate != null && nodeVideoBitrate.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Bitrate", nodeVideoBitrate.InnerText);

                    //string AudioFormat, 
                    XmlNode nodeAudioFormat = nodeDVD.SelectSingleNode("AudioCodec");
                    if (nodeAudioFormat != null && nodeAudioFormat.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "AudioCodec", nodeAudioFormat.InnerText);

                    //string AudioBitrate, 
                    XmlNode nodeAudioBitrate = nodeDVD.SelectSingleNode("AudioBitRate");
                    if (nodeAudioBitrate != null && nodeAudioBitrate.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "AudioBitRate", nodeAudioBitrate.InnerText);

                    //string Resolution, 
                    XmlNode nodeResolution = nodeDVD.SelectSingleNode("Resolution");
                    if (nodeResolution != null && nodeResolution.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Resolution", nodeResolution.InnerText);

                    //string Framerate, 
                    XmlNode nodeFramerate = nodeDVD.SelectSingleNode("FPS");
                    if (nodeFramerate != null && nodeFramerate.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "FPS", nodeFramerate.InnerText);

                    //string Languages, 
                    XmlNode nodeLanguages = nodeDVD.SelectSingleNode("Language");
                    if (nodeLanguages != null && nodeLanguages.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Language", nodeLanguages.InnerText);

                    //string Subtitles, 
                    XmlNode nodeSubtitles = nodeDVD.SelectSingleNode("Subtitles");
                    if (nodeSubtitles != null && nodeSubtitles.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Subtitles", nodeSubtitles.InnerText);

                    //string Size, 
                    XmlNode nodeSize = nodeDVD.SelectSingleNode("Filesize");
                    if (nodeSize != null && nodeSize.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Filesize", nodeSize.InnerText);

                    //string Disks, 
                    XmlNode nodeDisks = nodeDVD.SelectSingleNode("Disks");
                    if (nodeDisks != null && nodeDisks.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Disks", nodeDisks.InnerText);

                    //string Picture
                    string Image = String.Empty;
                    if (nodeDVD.SelectSingleNode("Cover") != null)
                        Image = nodeDVD.SelectSingleNode("Cover").InnerText;
                    WriteAntAtribute(destXml, "Cover", Image);

                    destXml.WriteEndElement();
                }

            }
            catch
            {
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
                //LogMyFilms.Debug("MF: XMM Importer: Writing Property '" + key + "' with Value '" + value.ToString() + "' to DB.");
            }
            else
            {
                //LogMyFilms.Debug("MF: XMM Importer Property '" + key + "' not found in dictionary ! - Attribute not written to DB !");
            }
        }
    }

}
