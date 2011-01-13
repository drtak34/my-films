#region Banneer
/* 
 *	Copyright (C) 2008 framug
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */
#endregion

#region HowToUse
/* 
 *	Purpose of this class is to manage a xml config file.
 *  The generated file is MediaPortal xml config file compatible.
 *	It has been written in case of you have your own plugin save button or, you don't want
 *  to use MediaPortal.Configuration and MediaPortal.Profile in your plugin classes.
 *  ie : using (Settings ReadOrWriteXml = new Settings(Config.GetFile(Config.Dir.Config, PluginName())))
 *       { ReadOrWriteXml = ... }
 * 
 *  How to use :
 *  - Add this class to your project. 
 *  - Change namespace of this class with your own plugin name. 
 * 
 *  - Call methods in your own plugin this way for writing : 
 *  XmlConfig XmlConfig = new XmlConfig();
 *  XmlConfig.WriteXmlConfig(StringFileName, StringSectionName, StringEntryName, StringValueName);
 *  XmlConfig.WriteXmlConfig(StringFileName, StringSectionName, StringEntryName, BoolValueName);
 *  XmlConfig.WriteXmlConfig(StringFileName, StringSectionName, StringEntryName, IntValueName);
 * 
 *  - Call methods in your own plugin this way for reading : 
 *  XmlConfig XmlConfig = new XmlConfig();
 *  YourString = XmlConfig.ReadXmlConfig(StringFileName, StringSectionName, StringEntryName, StringValueDefaultName);
 *  YourBool = XmlConfig.ReadXmlConfig(StringFileName, StringSectionName, StringEntryName, BoolValueDefaultName);
 *  YourInt = XmlConfig.ReadXmlConfig(StringFileName, StringSectionName, StringEntryName, IntValueDefaultName);
 * 
 *  - Call methods in your own plugin this way for removing entry : 
 *  XmlConfig XmlConfig = new XmlConfig();
 *  XmlConfig.RemoveEntry(StringFileName, StringSectionName, StringEntryName);
 * 
 */
#endregion

#region using
using System;
using System.IO;                 // For I/O file
using System.Xml;                // For XmlDocument
using MediaPortal.Configuration; // For recover install MediaPortal path
#endregion

namespace MesFilms
{
    class XmlConfig
    {

#region <<DECLARATION>>
        XmlDocument configxml = new XmlDocument();
#endregion

#region <<public>>

        // Recover install MediaPortal path
        public string PathInstalMP()
        {
            string path = Config.GetFolder(Config.Dir.Config);
            return path;
        }

        // Build entire filename of config file
        public string EntireFilenameConfig(string FileName)
        {
            if (FileName.Contains(":\\"))
                return FileName;
            string entirefilename = PathInstalMP() + @"\" + FileName + ".xml";
            return entirefilename;
        }

        // Called with bool type
        public void WriteXmlConfig(string FileName, string Section, string Entry, bool Value)
        {
          string value = string.Empty;
            // Change true by "yes" and false by "no" for xml MediaPortal compatibility 
            if (Value)
            {
                value = "yes";
            }
            else
            {
                value = "no";
            }

            WriteXmlConfig(FileName, Section, Entry, value);
        }

        // Called with decimal type
        public void WriteXmlConfig(string FileName, string Section, string Entry, decimal Value)
        {
            string value = Value.ToString();

            WriteXmlConfig(FileName, Section, Entry, value);
        }

        // Write a config file with XmlDocument
        public void WriteXmlConfig(string FileName, string Section, string Entry, string Value)
        {
            // Create file if doesn't exist
            if (!File.Exists(EntireFilenameConfig(FileName)))
            {
                CreateXmlConfig(FileName);
            }

            //Open xml document
            configxml.Load(EntireFilenameConfig(FileName));
            //Recover profile node
            XmlElement profile = configxml.DocumentElement;
            //Create section if doesn't exist
            String XPath = @"/profile/section[@name='" + Section + "']";
            XmlNodeList ListSection = configxml.SelectNodes(XPath);
            if (ListSection.Count < 1)
            {
                CreateSection(Section);
            }
            //Posit on section node
            XmlNode section = profile.SelectSingleNode("section[@name='" + Section + "']");

            //Create Entry if doesn't exist
            XPath = @"/profile/section[@name='" + Section + "']/entry[@name='" + Entry + "']";
            XmlNodeList ListEntry = configxml.SelectNodes(XPath);
            if (ListEntry.Count < 1)
            {
                CreateEntry(Section, Entry);
            }
            //Posit on entry node
            XmlNode entry = section.SelectSingleNode("entry[@name='" + Entry + "']");

            //Store entry value
            entry.InnerText = Value;

            //Save xml config file   
            configxml.Save(EntireFilenameConfig(FileName));
        }

        // Remove an Entry
        public void RemoveEntry(string FileName, string Section, string Entry)
        {
            // Return if xml file doesn't exist
            if (!File.Exists(EntireFilenameConfig(FileName)))
            {
                return;
            }

            //Open xml document
            configxml.Load(EntireFilenameConfig(FileName));
            //Recover profile node
            XmlElement profile = configxml.DocumentElement;

            //Posit on value
            string XPath = string.Empty;
            if (Entry.Length > 0)
                XPath = @"/profile/section[@name='" + Section + "']/entry[@name='" + Entry + "']";
            else
                XPath = @"/profile/section[@name='" + Section + "']";
            XmlNodeList ListEntry = configxml.SelectNodes(XPath);

            // If value exist, remove it otherwise, return
            if (ListEntry.Count > 0)
            {
                //Posit on section node
                XmlNode section = profile.SelectSingleNode("section[@name='" + Section + "']");
                if (Entry.Length > 0)
                {
                    //Posit on entry node
                    XmlNode entry = section.SelectSingleNode("entry[@name='" + Entry + "']");
                    //Remove the entry node for section
                    section.RemoveChild(entry);
                }
                else
                    //Remove the entry node for section
                    section.RemoveAll();

                //Save xml config file   
                configxml.Save(EntireFilenameConfig(FileName));
            }
            return;
        }

        // Called with bool type
        public bool ReadXmlConfig(string FileName, string Section, string Entry, bool Value)
        {
            // Change true by "yes" and false by "no" for xml MediaPortal compatibility 
            string value = Value.ToString();
            if (Value)
            {
                value = "yes";
            }
            else
            {
                value = "no";
            }

            string result = ReadXmlConfig(FileName, Section, Entry, value);

            // Change "yes" by true and "no" by false for xml MediaPortal compatibility 
            if (result == "yes")
            {
                Value = true;
            }
            else
            {
                Value = false;
            }

            return Value;
        }

        // Called with int type
        public int ReadXmlConfig(string FileName, string Section, string Entry, int Value)
        {
            string value = Value.ToString();

            string result = ReadXmlConfig(FileName, Section, Entry, value);

            try { Value = Convert.ToInt32(result); }
            catch { }

            return Value;
        }

        // Read xml config file with XmlDocument
        public string ReadXmlConfig(string FileName, string Section, string Entry, string Value)
        {
            // Default value if xml file doesn't exist
            if (!File.Exists(EntireFilenameConfig(FileName)))
            {
                return Value;
            }

            //Open xml document
            configxml.Load(EntireFilenameConfig(FileName));
            //Recover profile node
            XmlElement profile = configxml.DocumentElement;

            //Posit on value
            String XPath = @"/profile/section[@name='" + Section + "']/entry[@name='" + Entry + "']";
            XmlNodeList ListEntry = configxml.SelectNodes(XPath);

            // If value exist, return it otherwise, return default value
            if (ListEntry.Count > 0)
            {
                //Posit on section node
                XmlNode section = profile.SelectSingleNode("section[@name='" + Section + "']");
                //Posit on entry node
                XmlNode entry = section.SelectSingleNode("entry[@name='" + Entry + "']");
                //Recover value with entry data
                Value = entry.InnerText;
            }

            return Value;
        }
        // Read xml config file with XmlDocument
        public string ReadAMCUXmlConfig(string FileName, string Option, string Value)
        {
            // Default value if xml file doesn't exist
            if (!File.Exists(EntireFilenameConfig(FileName)))
            {
                return Value;
            }

            //Open xml document
            configxml.Load(EntireFilenameConfig(FileName));
            //Recover profile node
            XmlElement profile = configxml.DocumentElement;

            //Posit on value
            String XPath = @"/NewDataSet/Values[@Option='" + Option + "']";
            XmlNodeList ListEntry = configxml.SelectNodes(XPath);

            // If value exist, return it otherwise, return default value
            if (ListEntry.Count > 0)
                Value = ListEntry.Item(0).SelectSingleNode("Value").InnerText;

            return Value;
        }
#endregion

#region <<private>>

        // Create xml config file with profile node
        private void CreateXmlConfig(string FileName)
        {
            XmlDocument configxml = new XmlDocument();
            //Declaration of XML document type (utf-8, same as MediaPortal)
            XmlDeclaration declaration = configxml.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
            //Add declaration to document
            configxml.AppendChild(declaration);
            //Create profile node
            XmlNode profile = configxml.CreateNode(System.Xml.XmlNodeType.Element, "profile", string.Empty);
            //Add node to document
            configxml.AppendChild(profile);

            //Save xml config file
            configxml.Save(EntireFilenameConfig(FileName));
        }

        // create section node
        private void CreateSection(string Section)
        {
            //Recover profile node
            XmlElement profile = configxml.DocumentElement;
            //Create new section node
            XmlNode section = configxml.CreateElement("section");
            //Add "name" attribute to section node
            XmlAttribute name = configxml.CreateAttribute("name");
            //value is section name
            name.Value = Section;
            //Add value to section
            section.Attributes.Append(name);
            //Add section to document
            profile.AppendChild(section);
        }

        // create entry node
        private void CreateEntry(string Section, string Entry)
        {
            //Recover profile node
            XmlElement profile = configxml.DocumentElement;
            //Posit on section node
            XmlNode section = profile.SelectSingleNode("section[@name='" + Section + "']");
            //Create new node for entry
            XmlNode entry = configxml.CreateElement("entry");
            //Add "name" attribute to entry node
            XmlAttribute name = configxml.CreateAttribute("name");
            //value is entry name
            name.Value = Entry;
            //Add value to entry
            entry.Attributes.Append(name);
            //Add entry to document
            section.AppendChild(entry);
        }

#endregion

    } // end of class
} // end of namespace
