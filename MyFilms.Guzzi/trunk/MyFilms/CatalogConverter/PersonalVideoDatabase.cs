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
            ProfilerDict.Add("type", "MediaType");
            ProfilerDict.Add("label", "MediaLabel");
            ProfilerDict.Add("origtitle", "OriginalTitle");
            ProfilerDict.Add("title", "TranslatedTitle");
            ProfilerDict.Add("aka", "FormattedTitle");
            ProfilerDict.Add("num", "Number");
            ProfilerDict.Add("imdbrating", "Rating");
            ProfilerDict.Add("url", "URL");
            ProfilerDict.Add("country", "Country");
            ProfilerDict.Add("year", "Year");
            ProfilerDict.Add("length", "Length");
            ProfilerDict.Add("actors", "Actors");
            ProfilerDict.Add("genre", "Category");
            ProfilerDict.Add("director", "Director");
            ProfilerDict.Add("producer", "Producer");
            ProfilerDict.Add("description", "Description");
            ProfilerDict.Add("poster", "Picture");
            ProfilerDict.Add("dateadded", "Date");
            ProfilerDict.Add("viewed", "Checked");
            ProfilerDict.Add("path", "Source");
            ProfilerDict.Add("videocodec", "VideoFormat");
            ProfilerDict.Add("audiocodec", "AudioFormat");
            ProfilerDict.Add("resolution", "Resolution");
            ProfilerDict.Add("size", "Size");
            ProfilerDict.Add("videobitrate", "VideoBitrate");
            ProfilerDict.Add("audiobitrate", "AudioBitrate");
            ProfilerDict.Add("count", "Disks");
            ProfilerDict.Add("framerate", "Framerate");
            ProfilerDict.Add("comment", "Comments");
            ProfilerDict.Add("langs", "Languages");
            ProfilerDict.Add("subs", "Subtitles");
            ProfilerDict.Add("mpaa", "Certification");
            ProfilerDict.Add("tagline", "TagLine");
            ProfilerDict.Add("tags", "Tags");
            ProfilerDict.Add("scenario", "Writer");
            //ProfilerDict.Add("Borrower", "Borrower");
        }
        public string ConvertPersonalVideoDatabase(string source, string folderimage, string DestinationTagline, string DestinationTags, string DestinationCertification, string DestinationWriter, bool OnlyFile, string TitleDelim, bool AddTaglineToDescription)
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
                    string Tags = string.Empty;
                    string Tagline = string.Empty;
                    string Certification = string.Empty;
                  
                    destXml.WriteStartElement("Movie");
                    string wID = nodeDVD.SelectSingleNode("num").InnerText;
                    if (!string.IsNullOrEmpty(wID))
                        WriteAntAtribute(destXml, "num", wID);
                    else
                        WriteAntAtribute(destXml, "num", "9999");
                    if (nodeDVD.SelectSingleNode("origtitle") != null && !string.IsNullOrEmpty(nodeDVD.SelectSingleNode("origtitle").InnerText))
                      WriteAntAtribute(destXml, "origtitle", nodeDVD.SelectSingleNode("origtitle").InnerText);
                    if (nodeDVD.SelectSingleNode("title") != null && nodeDVD.SelectSingleNode("title").InnerText.Length > 0)
                      WriteAntAtribute(destXml, "title", nodeDVD.SelectSingleNode("title").InnerText);
                    if (nodeDVD.SelectSingleNode("aka") != null && nodeDVD.SelectSingleNode("aka").InnerText.Length > 0)
                      WriteAntAtribute(destXml, "aka", nodeDVD.SelectSingleNode("aka").InnerText);
                    if (nodeDVD.SelectSingleNode("viewed") != null)
                      if (nodeDVD.SelectSingleNode("viewed").InnerText.ToUpper() == "-1")
                        WriteAntAtribute(destXml, "viewed", "true");
                        else
                        WriteAntAtribute(destXml, "viewed", "false");
                    DateTime dt = new DateTime();
                    if (nodeDVD.SelectSingleNode("dateadded") != null)
                        try
                        {
                          dt = DateTime.Parse(nodeDVD.SelectSingleNode("dateadded").InnerText.ToString());
                        }
                        catch
                        {
                            try
                            {
                              dt = DateTime.Parse(nodeDVD.SelectSingleNode("dateadded").InnerText.ToString(), ci);
                            }
                            catch
                            {
                                dt = DateTime.Today;
                            }
                        }
                    WriteAntAtribute(destXml, "dateadded", dt.ToShortDateString());
                    if (nodeDVD.SelectSingleNode("country") != null)
                      if (nodeDVD.SelectSingleNode("country").InnerText.StartsWith("|"))
                        WriteAntAtribute(destXml, "country", nodeDVD.SelectSingleNode("country").InnerText.Substring(1));
                        else
                        WriteAntAtribute(destXml, "country", nodeDVD.SelectSingleNode("country").InnerText);
                    string wRating = "0";
                    if (nodeDVD.SelectSingleNode("imdbrating") != null && !string.IsNullOrEmpty(nodeDVD.SelectSingleNode("imdbrating").InnerText) && nodeDVD.SelectSingleNode("imdbrating").InnerText != "0")
                      wRating = nodeDVD.SelectSingleNode("imdbrating").InnerText;
                    else if (nodeDVD.SelectSingleNode("rating") != null)
                    {
                      wRating = nodeDVD.SelectSingleNode("rating").InnerText;
                    }
                    decimal wrating = 0;
                    try { wrating = Convert.ToDecimal(wRating, ci); }
                    catch
                    {
                        try { wrating = Convert.ToDecimal(wRating); }
                        catch { }
                    }
                    WriteAntAtribute(destXml, "imdbrating", wrating.ToString().Replace(",", "."));
                    string wYear = nodeDVD.SelectSingleNode("year").InnerText;
                    if (!string.IsNullOrEmpty(wYear))
                        WriteAntAtribute(destXml, "year", wYear);
                    if (nodeDVD.SelectSingleNode("length") != null)
                      WriteAntAtribute(destXml, "length", nodeDVD.SelectSingleNode("length").InnerText);
                    if (nodeDVD.SelectSingleNode("genre") != null)
                      if (nodeDVD.SelectSingleNode("genre").InnerText.StartsWith("|"))
                        WriteAntAtribute(destXml, "genre", nodeDVD.SelectSingleNode("genre").InnerText.Substring(1).Replace("|", ""));
                        else
                        WriteAntAtribute(destXml, "genre", nodeDVD.SelectSingleNode("genre").InnerText.Replace("|", ","));
                    if (nodeDVD.SelectSingleNode("director") != null)
                      WriteAntAtribute(destXml, "director", nodeDVD.SelectSingleNode("director").InnerText);
                    if (nodeDVD.SelectSingleNode("producer") != null)
                      WriteAntAtribute(destXml, "producer", nodeDVD.SelectSingleNode("producer").InnerText);
                    string Writer = string.Empty;
                    if (nodeDVD.SelectSingleNode("scenario") != null) // Writer
                      Writer = nodeDVD.SelectSingleNode("scenario").InnerText;
                    //WriteAntAtribute(destXml, "scenario", Writer);
                    if (nodeDVD.SelectSingleNode("actors") != null)
                      WriteAntAtribute(destXml, "actors", nodeDVD.SelectSingleNode("actors").InnerText);
                    if (nodeDVD.SelectSingleNode("poster") != null)
                      WriteAntAtribute(destXml, "poster", nodeDVD.SelectSingleNode("poster").InnerText);
                    if (nodeDVD.SelectSingleNode("type") != null)
                      WriteAntAtribute(destXml, "type", nodeDVD.SelectSingleNode("type").InnerText);
                    if (nodeDVD.SelectSingleNode("label") != null)
                      WriteAntAtribute(destXml, "label", nodeDVD.SelectSingleNode("label").InnerText);
                    if (nodeDVD.SelectSingleNode("url") != null)
                      WriteAntAtribute(destXml, "url", nodeDVD.SelectSingleNode("url").InnerText);
                    if (nodeDVD.SelectSingleNode("langs") != null)
                      if (nodeDVD.SelectSingleNode("langs").InnerText.StartsWith("|"))
                        WriteAntAtribute(destXml, "langs", nodeDVD.SelectSingleNode("langs").InnerText.Substring(1));
                        else
                        WriteAntAtribute(destXml, "langs", nodeDVD.SelectSingleNode("langs").InnerText);
                    if (nodeDVD.SelectSingleNode("subs") != null)
                      WriteAntAtribute(destXml, "subs", nodeDVD.SelectSingleNode("subs").InnerText);
                    if (nodeDVD.SelectSingleNode("videocodec") != null)
                      WriteAntAtribute(destXml, "videocodec", nodeDVD.SelectSingleNode("videocodec").InnerText);
                    if (nodeDVD.SelectSingleNode("audiocodec") != null)
                      WriteAntAtribute(destXml, "audiocodec", nodeDVD.SelectSingleNode("audiocodec").InnerText);
                    if (nodeDVD.SelectSingleNode("resolution") != null)
                      WriteAntAtribute(destXml, "resolution", nodeDVD.SelectSingleNode("resolution").InnerText);
                    if (nodeDVD.SelectSingleNode("videobitrate") != null)
                      WriteAntAtribute(destXml, "videobitrate", nodeDVD.SelectSingleNode("videobitrate").InnerText);
                    if (nodeDVD.SelectSingleNode("audiobitrate") != null)
                      WriteAntAtribute(destXml, "audiobitrate", nodeDVD.SelectSingleNode("audiobitrate").InnerText);
                    if (nodeDVD.SelectSingleNode("size") != null)
                      WriteAntAtribute(destXml, "size", nodeDVD.SelectSingleNode("size").InnerText);
                    if (nodeDVD.SelectSingleNode("framerate") != null)
                      WriteAntAtribute(destXml, "framerate", nodeDVD.SelectSingleNode("framerate").InnerText);
                    if (nodeDVD.SelectSingleNode("mpaa") != null)
                    {
                      Certification = nodeDVD.SelectSingleNode("mpaa").InnerText;
                      //WriteAntAtribute(destXml, "mpaa", Certification);
                    }
                    if (nodeDVD.SelectSingleNode("tagline") != null)
                    {
                      Tagline = nodeDVD.SelectSingleNode("tagline").InnerText;
                      //WriteAntAtribute(destXml, "tagline", Tagline);
                    }
                    if (nodeDVD.SelectSingleNode("tags") != null)
                    {
                      Tags = nodeDVD.SelectSingleNode("tags").InnerText;
                      //WriteAntAtribute(destXml, "tags", Tags);
                    }
                    if (nodeDVD.SelectSingleNode("count") != null)
                      WriteAntAtribute(destXml, "count", nodeDVD.SelectSingleNode("count").InnerText);

                    //string Description, 
                    string DescriptionMerged = string.Empty;
                    if (DestinationTagline == "Description")
                    {
                      if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
                      DescriptionMerged += Tagline;
                    }
                    if (DestinationTags == "Description")
                    {
                      if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
                      DescriptionMerged += Tags;
                    }
                    if (DestinationCertification == "Description")
                    {
                      if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
                      DescriptionMerged += Certification;
                    }
                    XmlNode nodePlot = nodeDVD.SelectSingleNode("Plot");
                    if (nodeDVD.SelectSingleNode("description") != null && nodeDVD.SelectSingleNode("description").InnerText != null)
                    {
                      if (DescriptionMerged.Length > 0) DescriptionMerged += System.Environment.NewLine;
                      DescriptionMerged += nodeDVD.SelectSingleNode("description").InnerText;
                    }
                    WriteAntAtribute(destXml, "description", DescriptionMerged);

                    //string Comments, 
                    string CommentsMerged = string.Empty;
                    if (DestinationTagline == "Comments")
                    {
                      if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
                      CommentsMerged += Tagline;
                    }
                    if (DestinationTags == "Comments")
                    {
                      if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
                      CommentsMerged += Tags;
                    }
                    if (DestinationCertification == "Comments")
                    {
                      if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
                      CommentsMerged += Certification;
                    }
                    if (nodeDVD.SelectSingleNode("comment") != null && nodeDVD.SelectSingleNode("comment").InnerText != null)
                    {
                      if (CommentsMerged.Length > 0) CommentsMerged += System.Environment.NewLine;
                      CommentsMerged += nodeDVD.SelectSingleNode("comment").InnerText;
                    }
                    WriteAntAtribute(destXml, "comment", CommentsMerged);

                    if (nodeDVD.SelectSingleNode("path") != null)
                      WriteAntAtribute(destXml, "path", nodeDVD.SelectSingleNode("path").InnerText);
                    destXml.WriteEndElement();

                    // Now writing MF extended attributes
                    WriteAntElement(destXml, "mpaa", Certification);
                    WriteAntElement(destXml, "tagline", Tagline);
                    WriteAntElement(destXml, "tags", Tags);
                    WriteAntElement(destXml, "scenario", Writer);
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

        private void WriteAntElement(XmlWriter tw, string key, string value)
        {
          string at = string.Empty;
          if (ProfilerDict.TryGetValue(key, out at))
          {
            tw.WriteElementString(at, value);
            //LogMyFilms.Debug("MF: XMM Importer: Writing Property '" + key + "' with Value '" + value.ToString() + "' to DB.");
          }
          else
          {
            //LogMyFilms.Debug("MF: XMM Importer Property '" + key + "' not found in dictionary ! - Element not written to DB !");
          }
        }

    }

}
