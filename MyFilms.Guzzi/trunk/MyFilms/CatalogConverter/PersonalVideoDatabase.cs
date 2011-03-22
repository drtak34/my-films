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

  class PersonalVideoDatabase
    {
        public Dictionary<string, string> ProfilerDict;

        public PersonalVideoDatabase()
        {
            ProfilerDict = new Dictionary<string, string>();
            ProfilerDict.Add("MediaType", "MediaType");
            ProfilerDict.Add("MediaLabel", "MediaLabel");
            ProfilerDict.Add("OriginalTitle", "OriginalTitle");
            ProfilerDict.Add("TranslatedTitle", "TranslatedTitle");
            ProfilerDict.Add("FormattedTitle", "FormattedTitle");
            ProfilerDict.Add("Number", "Number");
            ProfilerDict.Add("Rating", "Rating");
            ProfilerDict.Add("URL", "URL");
            ProfilerDict.Add("Country", "Country");
            ProfilerDict.Add("Year", "Year");
            ProfilerDict.Add("Length", "Length");
            ProfilerDict.Add("Actors", "Actors");
            ProfilerDict.Add("Category", "Category");
            ProfilerDict.Add("Director", "Director");
            ProfilerDict.Add("Producer", "Producer");
            ProfilerDict.Add("Description", "Description");
            ProfilerDict.Add("Picture", "Picture");
            ProfilerDict.Add("Date", "Date");
            ProfilerDict.Add("Checked", "Checked");
            ProfilerDict.Add("Source", "Source");
            ProfilerDict.Add("VideoFormat", "VideoFormat");
            ProfilerDict.Add("AudioFormat", "AudioFormat");
            ProfilerDict.Add("Resolution", "Resolution");
            ProfilerDict.Add("Size", "Size");
            ProfilerDict.Add("VideoBitrate", "VideoBitrate");
            ProfilerDict.Add("AudioBitrate", "AudioBitrate");
            ProfilerDict.Add("Disks", "Disks");
            ProfilerDict.Add("Framerate", "Framerate");
            ProfilerDict.Add("Comments", "Comments");
            ProfilerDict.Add("Languages", "Languages");
            ProfilerDict.Add("Subtitles", "Subtitles");
            //ProfilerDict.Add("Borrower", "Borrower");

        }
        public string ConvertPersonalVideoDatabase(string source, string folderimage, bool SortTitle, bool OnlyFile, string TitleDelim)
        {
            if (TitleDelim.Length == 0)
                TitleDelim = "\\";
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
                XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/xml/viddb/movie");
                foreach (XmlNode nodeDVD in dvdList)
                {

//+		[0]	{Element, Name="num"}	object {System.Xml.XmlElement}
//+		[1]	{Element, Name="title"}	object {System.Xml.XmlElement}
//+		[2]	{Element, Name="origtitle"}	object {System.Xml.XmlElement}
//+		[3]	{Element, Name="aka"}	object {System.Xml.XmlElement}
//+		[4]	{Element, Name="year"}	object {System.Xml.XmlElement}
//+		[5]	{Element, Name="genre"}	object {System.Xml.XmlElement}
//+		[6]	{Element, Name="country"}	object {System.Xml.XmlElement}
//+		[7]	{Element, Name="studio"}	object {System.Xml.XmlElement}
//+		[8]	{Element, Name="release"}	object {System.Xml.XmlElement}
//+		[9]	{Element, Name="mpaa"}	object {System.Xml.XmlElement}
//+		[10]	{Element, Name="tags"}	object {System.Xml.XmlElement}
//+		[11]	{Element, Name="director"}	object {System.Xml.XmlElement}
//+		[12]	{Element, Name="producer"}	object {System.Xml.XmlElement}
//+		[13]	{Element, Name="scenario"}	object {System.Xml.XmlElement}
//+		[14]	{Element, Name="music"}	object {System.Xml.XmlElement}
//+		[15]	{Element, Name="actors"}	object {System.Xml.XmlElement}
//+		[16]	{Element, Name="description"}	object {System.Xml.XmlElement}
//+		[17]	{Element, Name="comment"}	object {System.Xml.XmlElement}
//+		[18]	{Element, Name="tagline"}	object {System.Xml.XmlElement}
//+		[19]	{Element, Name="url"}	object {System.Xml.XmlElement}
//+		[20]	{Element, Name="imdbrating"}	object {System.Xml.XmlElement}
//+		[21]	{Element, Name="rating"}	object {System.Xml.XmlElement}
//+		[22]	{Element, Name="orating"}	object {System.Xml.XmlElement}
//+		[23]	{Element, Name="dateadded"}	object {System.Xml.XmlElement}
//+		[24]	{Element, Name="path"}	object {System.Xml.XmlElement}
//+		[25]	{Element, Name="count"}	object {System.Xml.XmlElement}
//+		[26]	{Element, Name="type"}	object {System.Xml.XmlElement}
//+		[27]	{Element, Name="rip"}	object {System.Xml.XmlElement}
//+		[28]	{Element, Name="length"}	object {System.Xml.XmlElement}
//+		[29]	{Element, Name="size"}	object {System.Xml.XmlElement}
//+		[30]	{Element, Name="langs"}	object {System.Xml.XmlElement}
//+		[31]	{Element, Name="subs"}	object {System.Xml.XmlElement}
//+		[32]	{Element, Name="translation"}	object {System.Xml.XmlElement}
//+		[33]	{Element, Name="resolution"}	object {System.Xml.XmlElement}
//+		[34]	{Element, Name="framerate"}	object {System.Xml.XmlElement}
//+		[35]	{Element, Name="videocodec"}	object {System.Xml.XmlElement}
//+		[36]	{Element, Name="videobitrate"}	object {System.Xml.XmlElement}
//+		[37]	{Element, Name="audiocodec"}	object {System.Xml.XmlElement}
//+		[38]	{Element, Name="audiobitrate"}	object {System.Xml.XmlElement}
//+		[39]	{Element, Name="label"}	object {System.Xml.XmlElement}
//+		[40]	{Element, Name="features"}	object {System.Xml.XmlElement}
//+		[41]	{Element, Name="viewed"}	object {System.Xml.XmlElement}
//+		[42]	{Element, Name="viewdate"}	object {System.Xml.XmlElement}
//+		[43]	{Element, Name="wish"}	object {System.Xml.XmlElement}
//+		[44]	{Element, Name="bookmark"}	object {System.Xml.XmlElement}
//+		[45]	{Element, Name="poster"}	object {System.Xml.XmlElement}
                  
  

                    CultureInfo ci = new CultureInfo("en-us");
                    XmlNodeList DiscsList = nodeDVD.SelectNodes("Disc");
                    string wfile = string.Empty;
                    string wVideoCodec = string.Empty;
                    string wAudioCodec = string.Empty;
                    string wResolution = string.Empty;
                    string wFileSize = string.Empty;
                    string wVideobitrate = string.Empty;
                    string wAudiobitrate = string.Empty;
                    string wFramerate = string.Empty;

                    if ((DiscsList != null) && (DiscsList.Count != 0))
                    {
                        foreach (XmlNode nodeDisc in DiscsList)
                        {
                            if (nodeDisc.Attributes["VideoCodec"] != null)
                                wVideoCodec = nodeDisc.Attributes["VideoCodec"].Value;
                            if (nodeDisc.Attributes["AudioCodec"] != null)
                                wAudioCodec = nodeDisc.Attributes["AudioCodec"].Value;
                            if (nodeDisc.Attributes["VideoResolution"] != null)
                                wResolution = nodeDisc.Attributes["VideoResolution"].Value;
                            if (nodeDisc.Attributes["VideoFilesize"] != null)
                                if (wFileSize.Length > 0)
                                    wFileSize = wFileSize + "+" + nodeDisc.Attributes["VideoFilesize"].Value;
                                else
                                    wFileSize = nodeDisc.Attributes["VideoFilesize"].Value;
                            if (nodeDisc.Attributes["VideoBitrate"] != null)
                                wVideobitrate = nodeDisc.Attributes["VideoBitrate"].Value;
                            if (nodeDisc.Attributes["AudioBitrate"] != null)
                                wAudiobitrate = nodeDisc.Attributes["AudioBitrate"].Value;
                            if (nodeDisc.Attributes["VideoFramerate"] != null)
                                wFramerate = nodeDisc.Attributes["VideoFramerate"].Value;
                            if (nodeDisc.Attributes["VideoFullPath"] != null)
                                if (wfile.Length > 0)
                                    wfile = wfile + ";" + nodeDisc.Attributes["VideoFullPath"].Value;
                                else
                                    wfile = nodeDisc.Attributes["VideoFullPath"].Value;
                        }
                    }
                    destXml.WriteStartElement("Movie");
                    string wID = nodeDVD.Attributes["Label"].Value;
                    if (wID != null && wID.Length > 3 && wID.ToLower().Contains("no:"))
                        WriteAntAtribute(destXml, "Number", wID.Substring(3));
                    else 
                        if (!string.IsNullOrEmpty(wID))
                            WriteAntAtribute(destXml, "Number", wID);
                        else
                            WriteAntAtribute(destXml, "Number", "9999");
                    if (nodeDVD.Attributes["OriginalTitle"] != null && nodeDVD.Attributes["OriginalTitle"].Value.Length > 0)
                        WriteAntAtribute(destXml, "OriginalTitle", nodeDVD.Attributes["OriginalTitle"].Value);
                    else
                        if (nodeDVD.Attributes["TranslatedTitle"] != null && nodeDVD.Attributes["TranslatedTitle"].Value.Length > 0)
                            WriteAntAtribute(destXml, "OriginalTitle", nodeDVD.Attributes["TranslatedTitle"].Value);
                    if (nodeDVD.Attributes["TranslatedTitle"] != null && nodeDVD.Attributes["TranslatedTitle"].Value.Length > 0)
                        WriteAntAtribute(destXml, "TranslatedTitle", nodeDVD.Attributes["TranslatedTitle"].Value);
                    else
                        if (nodeDVD.Attributes["OriginalTitle"] != null && nodeDVD.Attributes["OriginalTitle"].Value.Length > 0)
                            WriteAntAtribute(destXml, "TranslatedTitle", nodeDVD.Attributes["OriginalTitle"].Value);
                    if (nodeDVD.Attributes["FormattedTitle"] != null && nodeDVD.Attributes["FormattedTitle"].Value.Length > 0)
                        WriteAntAtribute(destXml, "FormattedTitle", nodeDVD.Attributes["FormattedTitle"].Value);
                    //WriteAntAtribute(destXml, "Notes/File", File);
                    if (nodeDVD.Attributes["User1Seen"] != null)
                        if (nodeDVD.Attributes["User1Seen"].Value.ToUpper() == "YES")
                            WriteAntAtribute(destXml, "Checked", "true");
                        else
                            WriteAntAtribute(destXml, "Checked", "false");
                    DateTime dt = new DateTime();
                    if (nodeDVD.Attributes["ModifiedDate"] != null)
                        try
                        {
                            dt = DateTime.Parse(nodeDVD.Attributes["ModifiedDate"].Value.ToString());
                        }
                        catch
                        {
                            try
                            {
                                dt = DateTime.Parse(nodeDVD.Attributes["ModifiedDate"].Value.ToString(), ci);
                            }
                            catch
                            {
                                dt = DateTime.Today;
                            }
                        }
                    WriteAntAtribute(destXml, "Date", dt.ToShortDateString());
                    if (nodeDVD.Attributes["Country"] != null)
                        if (nodeDVD.Attributes["Country"].Value.StartsWith("|"))
                            WriteAntAtribute(destXml, "Country", nodeDVD.Attributes["Country"].Value.Substring(1));
                        else
                            WriteAntAtribute(destXml, "Country", nodeDVD.Attributes["Country"].Value);
                    string wRating = "0";
                    if (nodeDVD.Attributes["Rating"] != null)
                        wRating = nodeDVD.Attributes["Rating"].Value;
                    decimal wrating = 0;
                    try { wrating = Convert.ToDecimal(wRating, ci); }
                    catch
                    {
                        try { wrating = Convert.ToDecimal(wRating); }
                        catch { }
                    }
                    WriteAntAtribute(destXml, "Rating", wrating.ToString().Replace(",", "."));
                    string wYear = nodeDVD.Attributes["Year"].Value;
                    if (!string.IsNullOrEmpty(wYear))
                        WriteAntAtribute(destXml, "Year", wYear);
                    if (nodeDVD.Attributes["RunningTime"] != null)
                        WriteAntAtribute(destXml, "Length", nodeDVD.Attributes["RunningTime"].Value);
                    if (nodeDVD.Attributes["Genre"] != null)
                        if (nodeDVD.Attributes["Genre"].Value.StartsWith("|"))
                            WriteAntAtribute(destXml, "Category", nodeDVD.Attributes["Genre"].Value.Substring(1).Replace("|", ""));
                        else
                            WriteAntAtribute(destXml, "Category", nodeDVD.Attributes["Genre"].Value.Replace("|", ","));
                    if (nodeDVD.Attributes["Director"] != null)
                        WriteAntAtribute(destXml, "Director", nodeDVD.Attributes["Director"].Value);
                    if (nodeDVD.Attributes["Producer"] != null)
                        WriteAntAtribute(destXml, "Producer", nodeDVD.Attributes["Producer"].Value);
                    if (nodeDVD.Attributes["Cast"] != null)
                        WriteAntAtribute(destXml, "Actors", nodeDVD.Attributes["Cast"].Value);
                    if (nodeDVD.Attributes["Picture"] != null)
                        WriteAntAtribute(destXml, "Picture", nodeDVD.Attributes["Picture"].Value);
                    if (nodeDVD.Attributes["Format"] != null)
                        WriteAntAtribute(destXml, "MediaType", nodeDVD.Attributes["Format"].Value);
                    if (nodeDVD.Attributes["Media"] != null)
                        WriteAntAtribute(destXml, "MediaLabel", nodeDVD.Attributes["Media"].Value);
                    if (nodeDVD.Attributes["Website"] != null)
                        WriteAntAtribute(destXml, "URL", nodeDVD.Attributes["Website"].Value);
                    if (nodeDVD.Attributes["PlotOriginal"] != null)
                        WriteAntAtribute(destXml, "Description", nodeDVD.Attributes["PlotOriginal"].Value);
                    if (nodeDVD.Attributes["Comments"] != null)
                        WriteAntAtribute(destXml, "Comments", nodeDVD.Attributes["Comments"].Value);
                    if (nodeDVD.Attributes["Language"] != null)
                        if (nodeDVD.Attributes["Language"].Value.StartsWith("|"))
                            WriteAntAtribute(destXml, "Languages", nodeDVD.Attributes["Language"].Value.Substring(1));
                        else
                            WriteAntAtribute(destXml, "Languages", nodeDVD.Attributes["Language"].Value);
                    if (nodeDVD.Attributes["Subtitles"] != null)
                        WriteAntAtribute(destXml, "Subtitles", nodeDVD.Attributes["Subtitles"].Value);
                    WriteAntAtribute(destXml, "Source", wfile);
                    if (wVideoCodec.Length > 0)
                        WriteAntAtribute(destXml, "VideoFormat", wVideoCodec);
                    if (wAudioCodec.Length > 0)
                        WriteAntAtribute(destXml, "AudioFormat", wAudioCodec);
                    if (wResolution.Length > 0)
                        WriteAntAtribute(destXml, "Resolution", wResolution);
                    if (wVideobitrate.Length > 0)
                        WriteAntAtribute(destXml, "VideoBitrate", wVideobitrate.Substring(0, wVideobitrate.IndexOf("K") - 1).Trim().Replace(" ", ""));
                    if (wAudiobitrate.Length > 0)
                      WriteAntAtribute(destXml, "AudioBitrate", wAudiobitrate.Substring(0, wAudiobitrate.IndexOf("K") - 1).Trim().Replace(" ", ""));
                    if (wFileSize.Length > 0)
                        WriteAntAtribute(destXml, "Size", wFileSize);
                    WriteAntAtribute(destXml, "Disks", DiscsList.Count.ToString());
                    if (wFramerate.Length > 0)
                        WriteAntAtribute(destXml, "Framerate", wFramerate);

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
            }
        }
    }

}
