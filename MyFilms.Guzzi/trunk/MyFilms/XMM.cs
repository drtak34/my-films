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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using MediaPortal.GUI.Library;
using System.Windows.Forms;
using System.Globalization;
using MediaPortal.Configuration;


namespace MesFilms
{
    class XMM
    {
        public Dictionary<string, string> ProfilerDict;

        public XMM()
        {
            ProfilerDict = new Dictionary<string, string>();
            ProfilerDict.Add("Format", "MediaType");
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
            //ProfilerDict.Add("Credits1", "Producer");
            ProfilerDict.Add("Overview", "Description");
            ProfilerDict.Add("Picture", "Picture");
            ProfilerDict.Add("Date", "Date");
            ProfilerDict.Add("Viewed", "Checked");
            //ProfilerDict.Add("Borrower", "Borrower");
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
            destXml.WriteElementString("Properties", "");
            destXml.WriteStartElement("Contents");
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(source);
                XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/Titles/Title");
                foreach (XmlNode nodeDVD in dvdList)
                {
                    destXml.WriteStartElement("Movie");
                    XmlNode nodeID = nodeDVD.SelectSingleNode("ID");
                    XmlNode nodeNumber = nodeDVD.SelectSingleNode("CollectionNumber");
                    XmlNode nodeFormat = nodeDVD.SelectSingleNode("Type");
                    XmlNode nodeTitle = nodeDVD.SelectSingleNode("LocalTitle");
                    XmlNode nodeOTitle = nodeDVD.SelectSingleNode("OriginalTitle");
                    XmlNode nodeSTitle = nodeDVD.SelectSingleNode("SortTitle");
                    XmlNode nodeYear = nodeDVD.SelectSingleNode("ProductionYear");
                    XmlNodeList LinksKist = nodeDVD.SelectNodes("Discs/Disc");

                    string url = String.Empty;
                    foreach (XmlNode nodeFile in LinksKist)
                    {
                        if (nodeFile.SelectSingleNode("LocationSideA").InnerText.Length > 0)
                        {
                            if (url.Length > 0) url += ";";
                            url += nodeFile.SelectSingleNode("LocationSideA").InnerText;
                        }
                    }
                    XmlNode nodeDuration = nodeDVD.SelectSingleNode("RunningTime");
                    XmlNode nodeCountry = nodeDVD.SelectSingleNode("Country");
                    XmlNode nodeOverview = nodeDVD.SelectSingleNode("Description");
                    string genre = String.Empty;
                    XmlNodeList genreList = nodeDVD.SelectNodes("Genres/Genre");
                    foreach (XmlNode nodeGenre in genreList)
                    {
                        if (genre.Length > 0) genre += ", ";
                        genre += nodeGenre.InnerText;
                    }
                    string Actor = String.Empty;
                    string Director = String.Empty;
                    string Producer = String.Empty;
                    XmlNodeList creditsList = nodeDVD.SelectNodes("Persons/Person");
                    foreach (XmlNode nodeCredit in creditsList)
                    {
                        string line = String.Empty;
                        if (nodeCredit.SelectSingleNode("Type") != null && nodeCredit.SelectSingleNode("Type").InnerText == "Actor")
                        {
                            if (nodeCredit.SelectSingleNode("Name") != null)
                                line = nodeCredit.SelectSingleNode("Name").InnerText;
                            if ((nodeCredit.SelectSingleNode("Role") != null) && (nodeCredit.SelectSingleNode("Role").InnerText.Length > 0))
                                line += " (" +nodeCredit.SelectSingleNode("Role").InnerText + ")";
                            if (line.Length > 0)
                            {
                                if (Actor.Length > 0) Actor += ", ";
                                Actor += line;
                            }
                        }
                        else
                        {
                            if (nodeCredit.SelectSingleNode("Type") != null && nodeCredit.SelectSingleNode("Type").InnerText == "Director")
                            {
                                if (nodeCredit.SelectSingleNode("Name") != null)
                                    line = nodeCredit.SelectSingleNode("Name").InnerText;
                                if (line.Length > 0)
                                {
                                    if (Director.Length > 0) Director += ", ";
                                    Director += line;
                                }
                            }
                            else
                            {
                                if (nodeCredit.SelectSingleNode("Type") != null && nodeCredit.SelectSingleNode("Type").InnerText == "Producer")
                                {
                                    if (nodeCredit.SelectSingleNode("Name") != null)
                                        line = nodeCredit.SelectSingleNode("Name").InnerText;
                                    if (line.Length > 0)
                                    {
                                        if (Producer.Length > 0) Producer += ", ";
                                        Producer += line;
                                    }
                                }
                            }
                        }
                    }
                    string Image = String.Empty;
                    if (nodeDVD.SelectSingleNode("Covers/Front") != null)
                        Image = nodeDVD.SelectSingleNode("Covers/Front").InnerText;
                    string Rating = string.Empty;
                    decimal wrating = 0;
                    CultureInfo ci = new CultureInfo("en-us");
                    XmlNode nodeRating = nodeDVD.SelectSingleNode("ParentalRating/value");
                    if (nodeRating != null && nodeRating.InnerText != null)
                    {
                        try {wrating = Convert.ToDecimal(nodeRating.InnerText);}
                        catch
                        {
                            try {wrating = Convert.ToDecimal(nodeRating.InnerText, ci);}
                            catch {}
                        }
                    }
                    Rating = wrating.ToString("0.0", ci);
                    if (nodeNumber != null && nodeNumber.InnerText != null && nodeNumber.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "CollectionNumber", nodeNumber.InnerText);
                    else
                        WriteAntAtribute(destXml, "CollectionNumber", "9999");
                    if (nodeOTitle != null && nodeOTitle.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Title", nodeOTitle.InnerText);
                    else
                        WriteAntAtribute(destXml, "Title", nodeTitle.InnerText);
                    WriteAntAtribute(destXml, "TTitle", nodeTitle.InnerText);
                    if (nodeSTitle != null && nodeSTitle.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "STitle", nodeSTitle.InnerText);
                    else
                        WriteAntAtribute(destXml, "STitle", nodeTitle.InnerText);
                    XmlNode nodeDate = nodeDVD.SelectSingleNode("Added");
                    
                    try
                    {
                        DateTime dt = new DateTime();
                        dt = DateTime.Parse(nodeDate.InnerText.ToString());
                        WriteAntAtribute(destXml, "Date", dt.ToShortDateString());
                    }
                    catch
                    {
                    }
                    WriteAntAtribute(destXml, "Viewed", "false");
                    if (nodeCountry != null)
                        WriteAntAtribute(destXml, "Country", nodeCountry.InnerText);
                    WriteAntAtribute(destXml, "Review/ReviewFilm", Rating);
                    if (nodeYear != null)
                        WriteAntAtribute(destXml, "Year", nodeYear.InnerText);
                    if (nodeDuration != null && nodeDuration.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "RunningTime", nodeDuration.InnerText);
                    if (nodeFormat != null)
                        WriteAntAtribute(destXml, "Format", nodeFormat.InnerText);
                    WriteAntAtribute(destXml, "Genres", genre);
                    WriteAntAtribute(destXml, "Credits", Director);
                    WriteAntAtribute(destXml, "Actors", Actor);
                    WriteAntAtribute(destXml, "Picture", Image);
                    WriteAntAtribute(destXml, "MovieFile", url);
                    if (nodeOverview != null && nodeOverview.InnerText != null)
                        WriteAntAtribute(destXml, "Overview", nodeOverview.InnerText);

                    destXml.WriteEndElement();
                }

            }
            catch
            {
                return "";
            }
            destXml.WriteEndElement();
            destXml.WriteEndElement();
            destXml.Close();
            return destFile;
        }

        private void WriteAntAtribute(XmlTextWriter tw, string key, string value)
        {
            string at = "";
            if (ProfilerDict.TryGetValue(key, out at))
            {
                tw.WriteAttributeString(at, value);
            }
        }
    }

}
