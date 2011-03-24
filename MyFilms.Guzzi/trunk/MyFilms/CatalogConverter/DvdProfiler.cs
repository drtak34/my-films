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
  using System.IO;
  using System.Text;
  using System.Web;
  using System.Xml;

  class DvdProfiler
    {
        public Dictionary<string, string> ProfilerDict;
        
        public DvdProfiler(string TagField)
        {
            ProfilerDict = new Dictionary<string, string>();
            ProfilerDict.Add("Title", "OriginalTitle");
            ProfilerDict.Add("TTitle", "TranslatedTitle");
            ProfilerDict.Add("STitle", "FormattedTitle");
            ProfilerDict.Add("CollectionNumber", "Number");
            ProfilerDict.Add("MediaTypes", "MediaType");
            //ProfilerDict.Add("label", "MediaLabel");
            ProfilerDict.Add("Review/ReviewFilm", "Rating");
            ProfilerDict.Add("Notes/File", "Source");
            ProfilerDict.Add("Notes/Country", "Country");
            ProfilerDict.Add("Year", "Year");
            ProfilerDict.Add("RunningTime", "Length");            
            ProfilerDict.Add("Actors", "Actors");
            ProfilerDict.Add("Genres", "Category");
            ProfilerDict.Add("Credits", "Director");
            ProfilerDict.Add("Credits1", "Producer");
            ProfilerDict.Add("Credits2", "Writer");
            ProfilerDict.Add("Overview", "Description");
            ProfilerDict.Add("Picture", "Picture");
            ProfilerDict.Add("Date", "Date");
            ProfilerDict.Add("EventType", "Checked");
            ProfilerDict.Add("Tag", TagField);
        }
       public string TagFullName;

        public string ConvertProfiler(string source, string folderimage, bool SortTitle, string TagField, bool OnlyFile)
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
                XmlNodeList dvdList = doc.DocumentElement.SelectNodes("/Collection/DVD");
                foreach (XmlNode nodeDVD in dvdList)
                {
                    destXml.WriteStartElement("Movie");
                    XmlNode nodeID = nodeDVD.SelectSingleNode("ID");
                    XmlNode nodeMediaType = nodeDVD.SelectSingleNode("MediaTypes/DVD");
                    XmlNode nodeNumber = null;
                    try
                    {
                        nodeNumber = nodeDVD.SelectSingleNode("CollectionNumber");
                    }
                    catch
                    {
                    }
                    XmlNode nodeTitle = nodeDVD.SelectSingleNode("Title");
                    XmlNode nodeOTitle = nodeDVD.SelectSingleNode("OriginalTitle");
                    XmlNode nodeSTitle = nodeDVD.SelectSingleNode("SortTitle");

                    string mediatype = String.Empty;
                    XmlNode mediaTypes = nodeDVD.SelectSingleNode("MediaTypes");
                    foreach (XmlElement type in mediaTypes)
                    {
                      if (type.InnerText != null)
                      {
                        
                        if (type.InnerText == "true")
                        {
                            if (mediatype.Length > 0) mediatype += ", ";
                            mediatype += type.Name.ToString();
                        }
                      }
                    }
                  
                    XmlNode nodeNotes = nodeDVD.SelectSingleNode("Notes"); 
                    XmlNode nodeYear = nodeDVD.SelectSingleNode("ProductionYear");
                    XmlNode nodeDuration = nodeDVD.SelectSingleNode("RunningTime");
                    string Overview = string.Empty;
                    XmlNode nodeOverview = nodeDVD.SelectSingleNode("Overview");
                    if (nodeOverview != null && nodeOverview.InnerText.Length > 0)
                    {
                      Encoding encoding = Encoding.GetEncoding("windows-1252");
                      Overview = System.Web.HttpUtility.UrlDecode ( nodeOverview.InnerText, encoding );
                      //StringWriter myWriter = new StringWriter();
                      //// Decode the encoded string.
                      //HttpUtility.HtmlDecode(nodeOverview.InnerText, myWriter);
                      //Overview = myWriter.ToString();
                    }

                    string File = null;
                    string Rating = null;
                    string Country = null;
                    if (nodeNotes != null && nodeNotes.InnerText != null)
                    {
                        if (nodeNotes.InnerText.IndexOf("<File>") != -1)
                            File = nodeNotes.InnerText.Substring(nodeNotes.InnerText.IndexOf("<File>") + 6, nodeNotes.InnerText.IndexOf("</File>") - nodeNotes.InnerText.IndexOf("<File>") - 6);
                        else
                            if (OnlyFile)
                                File = nodeNotes.InnerText.Trim();
                        if (nodeNotes.InnerText.IndexOf("<Country>") != -1)
                            Country = nodeNotes.InnerText.Substring(nodeNotes.InnerText.IndexOf("<Country>") + 9, nodeNotes.InnerText.IndexOf("</Country>") - nodeNotes.InnerText.IndexOf("<Country>") - 9);
                    }
                    if (nodeMediaType != null)
                    {
                        XmlNode nodeCountryOfOrigin = nodeDVD.SelectSingleNode("CountryOfOrigin");
                        Country = nodeCountryOfOrigin.InnerText;
                    }
                    string genre = String.Empty;
                    XmlNodeList genreList = nodeDVD.SelectNodes("Genres/Genre");
                    foreach (XmlNode nodeGenre in genreList)
                    {
                            if (genre.Length > 0) genre += ", ";
                            genre += nodeGenre.InnerText;
                    }
                    if (TagField.Length > 0)
                    {
                        XmlNodeList TagList = nodeDVD.SelectNodes("Tags/Tag");
                        TagFullName = string.Empty;
                        
                        foreach (XmlNode nodeTag in TagList)
                        {
                            if (nodeTag.Attributes["FullName"] != null && nodeTag.Attributes["FullName"].Value != null)
                            {
                                if (TagFullName.Length > 0) TagFullName += ", ";
                                TagFullName += nodeTag.Attributes["FullName"].Value;
                            }
                        }
                        if (TagField == "Category")
                        {
                            if (genre.Length > 0) genre += ", ";
                            genre += TagFullName;
                        }
                    }
                    string cast = String.Empty;
                    XmlNodeList actorsList = nodeDVD.SelectNodes("Actors/Actor");
                    foreach (XmlNode nodeActor in actorsList)
                    {
                        string firstname = String.Empty;
                        string lastname = String.Empty;
                        string role = String.Empty;
                        if (nodeMediaType != null)
                        {
                            if (nodeActor.Attributes["FirstName"] != null && nodeActor.Attributes["FirstName"].Value != null) firstname = nodeActor.Attributes["FirstName"].Value;
                            if (nodeActor.Attributes["LastName"] != null && nodeActor.Attributes["LastName"].Value != null) lastname = nodeActor.Attributes["LastName"].Value ;
                            if (nodeActor.Attributes["Role"] != null && nodeActor.Attributes["Role"].Value != null) role = nodeActor.Attributes["Role"].Value;
                        }
                        else
                        {
                            XmlNode nodeFirstName = nodeActor.SelectSingleNode("FirstName");
                            XmlNode nodeLastName = nodeActor.SelectSingleNode("LastName");
                            XmlNode nodeRole = nodeActor.SelectSingleNode("Role");
                            if (nodeFirstName != null && nodeFirstName.InnerText != null) firstname = nodeFirstName.InnerText;
                            if (nodeLastName != null && nodeLastName.InnerText != null) lastname = nodeLastName.InnerText;
                            if (nodeRole != null && nodeRole.InnerText != null) role = nodeRole.InnerText;
                        }
                        string line;
                        if (role.Length == 0)
                            line = String.Format("{0} {1}", firstname, lastname);
                        else
                            line = String.Format("{0} {1} ({2})", firstname, lastname, role);
                        if (cast.Length > 0) cast += ", ";
                        cast += line;
                    }
                    string Director = String.Empty;
                    string Producer = String.Empty;
                    string Writer = String.Empty;
                    XmlNodeList creditsList = nodeDVD.SelectNodes("Credits/Credit");
                    foreach (XmlNode nodeCredit in creditsList)
                    {
                        string firstname = String.Empty;
                        string lastname = String.Empty;
                        if (nodeMediaType != null)
                        {
                            firstname = nodeCredit.Attributes["FirstName"].Value;
                            lastname = nodeCredit.Attributes["LastName"].Value;
                            if (nodeCredit.Attributes["CreditSubtype"] != null && nodeCredit.Attributes["CreditSubtype"].Value != null && nodeCredit.Attributes["CreditSubtype"].Value == "Director")
                            {
                                if (Director.Length > 0) Director += ", ";
                                Director += String.Format("{0} {1}", firstname, lastname);
                            }
                            if (nodeCredit.Attributes["CreditSubtype"] != null && nodeCredit.Attributes["CreditSubtype"].Value != null && nodeCredit.Attributes["CreditSubtype"].Value == "Producer")
                            {
                                if (Producer.Length > 0) Producer += ", ";
                                Producer += String.Format("{0} {1}", firstname, lastname);
                            }
                            if (nodeCredit.Attributes["CreditSubtype"] != null && nodeCredit.Attributes["CreditSubtype"].Value != null && nodeCredit.Attributes["CreditSubtype"].Value == "Screenwriter")
                            {
                              if (Writer.Length > 0) Writer += ", ";
                              Writer += String.Format("{0} {1}", firstname, lastname);
                            }
                        }
                        else
                        {
                            XmlNode nodeFirstName = nodeCredit.SelectSingleNode("FirstName");
                            XmlNode nodeLastName = nodeCredit.SelectSingleNode("LastName");
                            XmlNode nodeType = nodeCredit.SelectSingleNode("CreditSubtype");
                            if (nodeType != null && nodeType.InnerText != null && nodeType.InnerText == "Director")
                            {
                                if (Director.Length > 0) Director += ", ";
                                if (nodeFirstName != null && nodeFirstName.InnerText != null) firstname = nodeFirstName.InnerText;
                                if (nodeLastName != null && nodeLastName.InnerText != null) lastname = nodeLastName.InnerText;
                                Director += String.Format("{0} {1}", firstname, lastname);
                            }
                            if (nodeType != null && nodeType.InnerText != null && nodeType.InnerText == "Producer")
                            {
                              if (Producer.Length > 0) Producer += ", ";
                              if (nodeFirstName != null && nodeFirstName.InnerText != null) firstname = nodeFirstName.InnerText;
                              if (nodeLastName != null && nodeLastName.InnerText != null) lastname = nodeLastName.InnerText;
                              Producer += String.Format("{0} {1}", firstname, lastname);
                            }
                            if (nodeType != null && nodeType.InnerText != null && nodeType.InnerText == "Screenwriter")
                            {
                              if (Writer.Length > 0) Writer += ", ";
                              if (nodeFirstName != null && nodeFirstName.InnerText != null) firstname = nodeFirstName.InnerText;
                              if (nodeLastName != null && nodeLastName.InnerText != null) lastname = nodeLastName.InnerText;
                              Writer += String.Format("{0} {1}", firstname, lastname);
                            }
                        }
                    }
                    string Image = folderimage + @"\" + nodeID.InnerText.Trim() + "f.jpg";
                    if (nodeMediaType != null)
                    {
                        XmlNode nodeRating = nodeDVD.SelectSingleNode("Review");
                        if (nodeRating.Attributes["Film"] != null && nodeRating.Attributes["Film"].Value != null) 
                            Rating = nodeRating.Attributes["Film"].Value + ".0";
                        else
                            Rating = "0.0";
                    }
                    else
                    {
                        XmlNode nodeRating = nodeDVD.SelectSingleNode("Review/ReviewFilm");
                        if (nodeRating != null && nodeRating.InnerText != null)
                            Rating = nodeRating.InnerText + ".0";
                        else
                            Rating = "0.0";
                    }
                    if (nodeNumber != null && nodeNumber.InnerText != null && nodeNumber.InnerText.Length > 0)
                        WriteAntAtribute(destXml,"CollectionNumber",nodeNumber.InnerText);
                    else
                        WriteAntAtribute(destXml, "CollectionNumber", "9999");
                    if (nodeOTitle != null && nodeOTitle.InnerText.Length > 0)
                        WriteAntAtribute(destXml, "Title", nodeOTitle.InnerText);
                    else
                        WriteAntAtribute(destXml, "Title", nodeTitle.InnerText);
                    WriteAntAtribute(destXml, "TTitle", nodeTitle.InnerText);
                    if (SortTitle)
                        WriteAntAtribute(destXml, "STitle", nodeSTitle.InnerText);
                    else
                        WriteAntAtribute(destXml, "STitle", nodeTitle.InnerText);
                    if (nodeDVD.SelectSingleNode("EventType") != null && nodeDVD.SelectSingleNode("EventType").InnerText.Length > 0)
                      WriteAntAtribute(destXml, "EventType", nodeDVD.SelectSingleNode("EventType").InnerText);

                    WriteAntAtribute(destXml, "MediaTypes", mediatype);
                    WriteAntAtribute(destXml, "Notes/File", File);
                    WriteAntAtribute(destXml, "Notes/Country", Country);
                    WriteAntAtribute(destXml, "Review/ReviewFilm", Rating);
                    if (nodeYear != null)
                        WriteAntAtribute(destXml, "Year", nodeYear.InnerText);
                    if (nodeDuration != null)
                        WriteAntAtribute(destXml, "RunningTime", nodeDuration.InnerText);
                    WriteAntAtribute(destXml, "Genres", genre);
                    WriteAntAtribute(destXml, "Credits", Director);
                    WriteAntAtribute(destXml, "Credits1", Producer);
                    WriteAntAtribute(destXml, "Credits2", Writer);
                    WriteAntAtribute(destXml, "Actors", cast);
                    XmlNode nodeDate = nodeDVD.SelectSingleNode("PurchaseInfo/PurchaseDate");
                    try
                    {
                        DateTime dt = new DateTime();
                        dt = DateTime.Parse(nodeDate.InnerText.ToString());
                        WriteAntAtribute(destXml, "Date", dt.ToShortDateString());
                    }
                    catch
                    {
                    }
                    WriteAntAtribute(destXml, "Picture", Image);
                    if ((TagField.Length > 0) && (TagField != "Category") && (TagFullName.Length > 0))
                        WriteAntAtribute(destXml, "Tag", TagFullName);
                    WriteAntAtribute(destXml, "Overview", Overview);
                        
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
        
        private void  WriteAntAtribute(XmlTextWriter tw,string key, string value)
        {
          string at = string.Empty;
        if (ProfilerDict.TryGetValue(key, out at))
        {
            tw.WriteAttributeString(at, value);
        }
        }
    }

}
