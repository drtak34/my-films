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

namespace MesFilms.MyFilms.CatalogConverter
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Xml;
  using System.Globalization;

  class MovieCollector
    {
        public Dictionary<string, string> ProfilerDict;

        public MovieCollector()
        {
            ProfilerDict = new Dictionary<string, string>();
            ProfilerDict.Add("Format", "MediaType");
            ProfilerDict.Add("Storage", "MediaLabel");
            ProfilerDict.Add("Title", "OriginalTitle");
            ProfilerDict.Add("TTitle", "TranslatedTitle");
            ProfilerDict.Add("STitle", "FormattedTitle");
            ProfilerDict.Add("CollectionNumber", "Number");
            ProfilerDict.Add("Review/ReviewFilm", "Rating");
            ProfilerDict.Add("MovieFile", "URL");
            ProfilerDict.Add("Country", "Country");
            ProfilerDict.Add("Year", "Year");
            ProfilerDict.Add("RunningTime", "Length");
            ProfilerDict.Add("Actors", "Actors");
            ProfilerDict.Add("Genres", "Category");
            ProfilerDict.Add("Credits", "Director");
            ProfilerDict.Add("Credits1", "Producer");
            ProfilerDict.Add("Overview", "Description");
            ProfilerDict.Add("Picture", "Picture");
            ProfilerDict.Add("Date", "Date");
            ProfilerDict.Add("Viewed", "Checked");
            ProfilerDict.Add("Borrower", "Borrower");
        }
        public string ConvertMovieCollector(string source, string folderimage, bool SortTitle, bool OnlyFile,string WMCCovers,string TitleDelim)
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
                XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/movieinfo/movielist/movie");
                foreach (XmlNode nodeDVD in dvdList)
                {
                    XmlNode nodeID = nodeDVD.SelectSingleNode("id");
                    XmlNode nodeMediaType = nodeDVD.SelectSingleNode("format/displayname");
                    XmlNode nodeNumber = null;
                    try
                    {
                        nodeNumber = nodeDVD.SelectSingleNode("index");
                    }
                    catch
                    {
                    }
                    XmlNode nodeFormat = nodeDVD.SelectSingleNode("format/displayname");
                    XmlNode nodeTitle = nodeDVD.SelectSingleNode("title");
                    XmlNode nodeOTitle = nodeDVD.SelectSingleNode("originaltitle");
                    XmlNode nodeSTitle = nodeDVD.SelectSingleNode("titlesort");
                    XmlNode nodeViewed = nodeDVD.SelectSingleNode("seenit");
                    XmlNode nodeYear = nodeDVD.SelectSingleNode("releasedate/year/displayname");
                    XmlNodeList LinksKist = nodeDVD.SelectNodes("links/link");
                    string url = String.Empty;
                    foreach (XmlNode nodeFile in LinksKist)
                    {
                        if (nodeFile.SelectSingleNode("urltype").InnerText == "Movie")
                        {
                            if (url.Length > 0) url += ";";
                            url += nodeFile.SelectSingleNode("url").InnerText;
                        }
                    }
                    XmlNode nodeBorrower = nodeDVD.SelectSingleNode("loan/loanedto/displayname");
                    XmlNode nodeDuration = nodeDVD.SelectSingleNode("runtime");
                    XmlNode nodeCountry = nodeDVD.SelectSingleNode("country");
                    XmlNode nodeOverview = nodeDVD.SelectSingleNode("plot");
                    string genre = String.Empty;
                    XmlNodeList genreList = nodeDVD.SelectNodes("genres/genre");
                    foreach (XmlNode nodeGenre in genreList)
                    {
                        if (nodeGenre.SelectSingleNode("displayname") != null)
                            if (genre.Length > 0) genre += ", ";
                        genre += nodeGenre.SelectSingleNode("displayname").InnerText;
                    }
                    string cast = String.Empty;
                    XmlNodeList actorsList = nodeDVD.SelectNodes("cast/star");
                    foreach (XmlNode nodeActor in actorsList)
                    {
                        string line = String.Empty;
                        if (nodeActor.SelectSingleNode("person/displayname") != null)
                            line = nodeActor.SelectSingleNode("person/displayname").InnerText;
                        if (nodeActor.SelectSingleNode("character") != null && nodeActor.SelectSingleNode("character").InnerText.Length > 0)
                            line += " (" + nodeActor.SelectSingleNode("character").InnerText + ")";
                        if (line.Length > 0)
                        {
                            if (cast.Length > 0) cast += ", ";
                            cast += line;
                        }
                    }
                    string Director = String.Empty;
                    string Producer = String.Empty;
                    XmlNodeList creditsList = nodeDVD.SelectNodes("crew/crewmember");
                    foreach (XmlNode nodeCredit in creditsList)
                    {
                        string line = String.Empty;
                        if (nodeCredit.SelectSingleNode("roleid") != null && nodeCredit.SelectSingleNode("roleid").InnerText == "dfDirector")
                        {
                            if (nodeCredit.SelectSingleNode("person/displayname") != null)
                                line = nodeCredit.SelectSingleNode("person/displayname").InnerText;
                            if (line.Length > 0)
                            {
                                if (Director.Length > 0) Director += ", ";
                                Director += line;
                            }
                        }
                        else
                            if (nodeCredit.SelectSingleNode("roleid") != null && nodeCredit.SelectSingleNode("roleid").InnerText == "dfProducer")
                            {
                                if (nodeCredit.SelectSingleNode("person/displayname") != null)
                                    line = nodeCredit.SelectSingleNode("person/displayname").InnerText;
                                if (line.Length > 0)
                                {
                                    if (Producer.Length > 0) Producer += ", ";
                                    Producer += line;
                                }
                            }
                    }
                    string Image = String.Empty;
                    if (WMCCovers == "Thumbnails")
                    {
                        if (nodeDVD.SelectSingleNode("thumbfilepath") != null)
                            Image = nodeDVD.SelectSingleNode("thumbfilepath").InnerText;
                    }
                    else
                    {
                        if (nodeDVD.SelectSingleNode("coverfront") != null)
                            Image = nodeDVD.SelectSingleNode("coverfront").InnerText;
                    }

                    string Rating = string.Empty;
                    decimal wrating = 0;
                    CultureInfo ci = new CultureInfo("en-us");
                    XmlNode nodeRating = nodeDVD.SelectSingleNode("myrating");
                    if (nodeRating != null && nodeRating.InnerText != null)
                    {
                        try { wrating = Convert.ToDecimal(nodeRating.InnerText); }
                        catch
                        {
                            try { wrating = Convert.ToDecimal(nodeRating.InnerText, ci); }
                            catch { }
                        }
                    }
                    if (wrating == 0)
                    {
                        nodeRating = nodeDVD.SelectSingleNode("imdbrating");
                        if (nodeRating != null && nodeRating.InnerText != null)
                        {
                            try { wrating = Convert.ToDecimal(nodeRating.InnerText); }
                            catch
                            {
                                try { wrating = Convert.ToDecimal(nodeRating.InnerText, ci); }
                                catch { }
                            }
                        }
                    }
                    Rating = wrating.ToString("0.0", ci);
                    XmlNodeList DiscsList = nodeDVD.SelectNodes("discs/disc");
                    string medialabel = String.Empty;
                    int nodisc = 0;

                    foreach (XmlNode nodeDisc in DiscsList)
                    {
                        destXml.WriteStartElement("Movie");
                        nodisc++;
                        string wmedialabel = string.Empty;
                        if (nodeDisc.SelectSingleNode("storagedevice/displayname") != null)
                            wmedialabel = nodeDisc.SelectSingleNode("storagedevice/displayname").InnerText;
                        if (nodeDisc.SelectSingleNode("storageslot") != null)
                            if (wmedialabel.Length > 0)
                                wmedialabel = wmedialabel + "\\" + nodeDisc.SelectSingleNode("storageslot").InnerText;
                            else
                                wmedialabel = nodeDisc.SelectSingleNode("storageslot").InnerText;

                        if (nodeNumber != null && !string.IsNullOrEmpty(nodeNumber.InnerText))
                            WriteAntAtribute(destXml, "CollectionNumber", nodeNumber.InnerText + nodisc.ToString());
                        else
                            WriteAntAtribute(destXml, "CollectionNumber", "9999");
                        if (DiscsList.Count > 1)
                        {
                            if (nodeOTitle != null && nodeOTitle.InnerText.Length > 0)
                                WriteAntAtribute(destXml, "Title", nodeOTitle.InnerText + TitleDelim + "Disc" + nodisc.ToString());
                            else
                                WriteAntAtribute(destXml, "Title", nodeTitle.InnerText + TitleDelim + "Disc" + nodisc.ToString());
                            if (nodeTitle != null && nodeTitle.InnerText.Length > 0)
                                WriteAntAtribute(destXml, "TTitle", nodeTitle.InnerText + TitleDelim + "Disc" + nodisc.ToString());
                            if (nodeSTitle != null && nodeSTitle.InnerText.Length > 0)
                                WriteAntAtribute(destXml, "STitle", nodeSTitle.InnerText + TitleDelim + "Disc" + nodisc.ToString());
                            else
                                WriteAntAtribute(destXml, "STitle", nodeTitle.InnerText + TitleDelim + "Disc" + nodisc.ToString());
                        }
                        else
                        {
                            if (nodeOTitle != null && nodeOTitle.InnerText.Length > 0)
                                WriteAntAtribute(destXml, "Title", nodeOTitle.InnerText);
                            else
                                WriteAntAtribute(destXml, "Title", nodeTitle.InnerText);
                            if (nodeTitle != null && nodeTitle.InnerText.Length > 0)
                                WriteAntAtribute(destXml, "TTitle", nodeTitle.InnerText);
                            if (nodeSTitle != null && nodeSTitle.InnerText.Length > 0)
                                WriteAntAtribute(destXml, "STitle", nodeSTitle.InnerText);
                            else
                                WriteAntAtribute(destXml, "STitle", nodeTitle.InnerText);
                        }
                            //WriteAntAtribute(destXml, "Notes/File", File);
                        if (wmedialabel.Length > 0)
                            WriteAntAtribute(destXml, "Storage", wmedialabel);
                        if (nodeViewed.Attributes != null)
                            if (nodeViewed.Attributes["boolvalue"].Value == "1")
                                WriteAntAtribute(destXml, "Viewed", "true");
                            else
                                WriteAntAtribute(destXml, "Viewed", "false");
                        else
                            WriteAntAtribute(destXml, "Viewed", "false");
                        XmlNode nodeDate = nodeDVD.SelectSingleNode("lastmodified/date");
                        try
                        {
                            DateTime dt = new DateTime();
                            dt = DateTime.Parse(nodeDate.InnerText.ToString());
                            WriteAntAtribute(destXml, "Date", dt.ToShortDateString());
                        }
                        catch
                        {
                        }
                        if (nodeCountry.SelectSingleNode("displayname") != null)
                            WriteAntAtribute(destXml, "Country", nodeCountry.SelectSingleNode("displayname").InnerText);
                        WriteAntAtribute(destXml, "Review/ReviewFilm", Rating);
                        if (nodeYear != null && nodeYear.InnerText != null)
                            WriteAntAtribute(destXml, "Year", nodeYear.InnerText);
                        if (nodeDuration != null && nodeDuration.InnerText.Substring(0, nodeDuration.InnerText.IndexOf(" ")).Length > 0)
                        {
                            if (nodeDuration.InnerText.IndexOf(" hr ") != -1)
                            {
                                int duree = int.Parse(nodeDuration.InnerText.Substring(0, nodeDuration.InnerText.IndexOf(" hr ")).Trim()) * 60 + int.Parse(nodeDuration.InnerText.Substring(nodeDuration.InnerText.IndexOf(" hr ") + 4, 2).Trim());
                                WriteAntAtribute(destXml, "RunningTime", duree.ToString());
                            }
                            else
                                WriteAntAtribute(destXml, "RunningTime", nodeDuration.InnerText.Substring(0, nodeDuration.InnerText.IndexOf(" ")).ToString());
                        }
                        if (nodeBorrower != null && nodeBorrower.InnerText != null)
                            WriteAntAtribute(destXml, "Borrower", nodeBorrower.InnerText);
                        if (nodeFormat != null && nodeFormat.InnerText != null)
                            WriteAntAtribute(destXml, "Format", nodeFormat.InnerText);
                        WriteAntAtribute(destXml, "Genres", genre);
                        WriteAntAtribute(destXml, "Credits", Director);
                        WriteAntAtribute(destXml, "Credits1", Producer);
                        WriteAntAtribute(destXml, "Actors", cast);
                        WriteAntAtribute(destXml, "Picture", Image);
                        WriteAntAtribute(destXml, "MovieFile", url);
                        if (nodeOverview != null && nodeOverview.InnerText != null)
                            WriteAntAtribute(destXml, "Overview", nodeOverview.InnerText);
                        else
                          WriteAntAtribute(destXml, "Overview", string.Empty);

                        destXml.WriteEndElement();
                    }
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
