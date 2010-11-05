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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using MediaPortal.Configuration;

using MediaPortal.GUI.Library;



namespace MesFilms
{
    public class Logos
    {
        public Logos()
        {
            System.ComponentModel.BackgroundWorker bgLogos = new System.ComponentModel.BackgroundWorker();
            using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilmsLogos_" + Configuration.CurrentConfig + ".xml")))
            {
                XmlConfig XmlConfig = new XmlConfig();
                LogosPath = XmlConfig.ReadXmlConfig("MyFilmsLogos_" + Configuration.CurrentConfig, "ID0000", "LogosPath", XmlConfig.PathInstalMP() + @"\thumbs\");
                if (LogosPath.LastIndexOf("\\") != LogosPath.Length - 1)
                    LogosPath = LogosPath + "\\";
                Log.Debug("MyFilms: Logo path for storing picture created " + LogosPath.ToString());
                int i = 0;
                do
                {
                    string wline = XmlConfig.ReadXmlConfig("MyFilmsLogos_" + Configuration.CurrentConfig, "ID2001", "ID2001_" + i, null);
                    if (wline == null)
                        break;
                    ID2001Logos.Add(wline);
                    i++;
                } while (true);

                i = 0;
                do
                {
                    string wline = XmlConfig.ReadXmlConfig("MyFilmsLogos_" + Configuration.CurrentConfig, "ID2002", "ID2002_" + i, null);
                    if (wline == null)
                        break;
                    ID2002Logos.Add(wline);
                    i++;
                } while (true);
                i = 0;
                ArrayList countriesLogos = new ArrayList();
                do
                {
                    string wline = XmlConfig.ReadXmlConfig("MyFilmsLogos_" + Configuration.CurrentConfig, "ID2003", "ID2003_" + i, null);
                    if (wline == null)
                        break;
                    ID2003Logos.Add(wline);
                    i++;
                } while (true);
            }
        }
        public static string Build_Logos(DataRow r, string Logo_Type, int imgHeight, int imgWidth, int XPos, int YPos, int spacer, int ID)
        {
            List<Image> imgs = new List<Image>();
            List<Size> imgSizes = new List<Size>();
            List<string> listelogos = new List<string>();
            string fileLogoName = string.Empty;
            switch (Logo_Type)
            {
                case "ID2001":
                    fileLogoName = get_Logos(r, ref listelogos, Logos.ID2001Logos);
                    break;
                case "ID2002":
                    fileLogoName = get_Logos(r, ref listelogos, Logos.ID2002Logos);
                    break;
                case "ID2003":
                    fileLogoName = get_Logos(r, ref listelogos, Logos.ID2001Logos);
                    fileLogoName = fileLogoName + get_Logos(r, ref listelogos, Logos.ID2002Logos);
                    break;
              
            }
            if (listelogos.Count > 0)
            {
                Log.Debug("MyFilms: Logo picture to be added " + fileLogoName.ToString());
                string skinName = GUIGraphicsContext.Skin.Substring(GUIGraphicsContext.Skin.LastIndexOf("\\") + 1);
                if (ID == MesFilms.ID_MesFilms)
                    fileLogoName = "MyFilms_" + skinName + "_M_" + fileLogoName + ".png";
                else
                    fileLogoName = "MyFilms_" + skinName + "_D_" + fileLogoName + ".png";
                XmlConfig XmlConfig = new XmlConfig();
                Image single = null;
                int wHeight = 0;
                int wWidth = 0;
                if (!System.IO.Directory.Exists(LogosPath))
                {
                    try { System.IO.Directory.CreateDirectory(LogosPath); }
                    catch {}
                }
                try
                {
                    single = ImageFast.FastFromFile(LogosPath + fileLogoName);
                    wHeight = single.Height;
                    wWidth = single.Width;
                    single.Dispose();
                }
                catch (Exception)
                {
                }
                if (!System.IO.File.Exists(LogosPath + fileLogoName) || (wHeight != imgHeight) || (wWidth != imgWidth))
                {
                    Bitmap b = new Bitmap(imgWidth, imgHeight);
                    Image img = b;
                    Graphics g = Graphics.FromImage(img);
                    append_Logos(listelogos, ref g, imgHeight, imgWidth, spacer);
                    try
                    {
                        b.Save(LogosPath + fileLogoName);
                        Log.Debug("MyFilms: Concatened Logo saved " + fileLogoName);
                        System.Threading.Thread.Sleep(10);
                    }
                    catch (Exception e)
                    {
                        Log.Info("MyFilms: Unable to save Logo file ! error : " + e.Message.ToString());
                    }
                }
                return LogosPath + fileLogoName;
            }
            else
                return string.Empty;
        }
        static string get_Logos(DataRow r, ref List<string> listelogos, ArrayList RulesLogos)
        {
            string fileLogoName = string.Empty;
            foreach (string wline in RulesLogos)
            {
                string[] wtab = wline.Split(new Char[] { ';' });
                if (System.IO.File.Exists(wtab[7]) && System.IO.Path.GetDirectoryName(wtab[7]).Length > 0)
                {
                    bool cond1 = get_record_rule(r, wtab[0], wtab[1], wtab[2]);
                    bool cond2 = get_record_rule(r, wtab[4], wtab[5], wtab[6]);
                    if ((wtab[3].Length == 0) && (cond1))
                    {
                        if (!(listelogos.Contains(wtab[7])))
                        {
                            listelogos.Add(wtab[7]);
                            fileLogoName = fileLogoName + "_" + System.IO.Path.GetFileNameWithoutExtension(wtab[7]);
                        }
                        continue;
                    }
                    if ((wtab[3] == "or") && ((cond1) || (cond2)))
                    {
                        if (!(listelogos.Contains(wtab[7])))
                        {
                            listelogos.Add(wtab[7]);
                            fileLogoName = fileLogoName + "_" + System.IO.Path.GetFileNameWithoutExtension(wtab[7]);
                        }
                        continue;
                    }
                    if ((wtab[3] == "and") && ((cond1) && (cond2)))
                    {
                        if (!(listelogos.Contains(wtab[7])))
                        {
                            listelogos.Add(wtab[7]);
                            fileLogoName = fileLogoName + "_" + System.IO.Path.GetFileNameWithoutExtension(wtab[7]);
                        }
                        continue;
                    }
                }
            }
            return fileLogoName;
        }
        
        static bool get_record_rule(DataRow r, string field, string compar, string value)
        {
            switch (compar)
            {
                case "equal":
                    if (r[field].ToString().ToLower() == value.ToLower())
                        return true;
                    break;
                case "not equal":
                    if (r[field].ToString().ToLower() != value.ToLower())
                        return true;
                    break;
                case "contains":
                    if (r[field].ToString().ToLower().Contains(value.ToLower()))
                        return true;
                    break;
                case "not contains":
                    if (!(r[field].ToString().ToLower().Contains(value.ToLower())))
                        return true;
                    break;
                case "greater":
                    switch (r[field].GetType().Name )
                    {
                        case "Int32":
                            if ((int)r[field].ToString().ToLower().CompareTo(value.ToLower()) > 0)
                                return true;
                            break;
                        case "DateTime":
                            break;
                        default:
                            if (r[field].ToString().ToLower().CompareTo(value.ToLower()) >  0)
                                return true;
                            break;
                    }
                    break;
                case "lower":
                    if (!(r[field].ToString().ToLower().CompareTo(value.ToLower()) > 0))
                        return true;
                    break;
                case "filled":
                    if (r[field].ToString().Length > 0)
                        return true;
                    break;
                case "not filled":
                    if (r[field].ToString().Length == 0)
                        return true;
                    break;
            }
            return false;
        }
        static void append_Logos(List<string> listelogos, ref Graphics g, int totalHeight, int totalWidth, int spacer)
        {
            int noImgs = listelogos.Count;
            List<Image> imgs = new List<Image>();
            List<Size> imgSizes = new List<Size>();
            int checkWidth = 0, checkHeigth = 0;
            // step one: get all sizes (not all logos are obviously square) and scale them to fit vertically
            Image single = null;

            // Create source
            float scale = 0;
            float totalWidthf = (float) totalWidth;
            float totalHeightf = (float)totalHeight;
            Size tmp = default(Size);
            int x_pos = 0, y_pos = 0;
            foreach (string t in listelogos)
            {
                try
                {
                    single = ImageFast.FastFromFile(t);
                }
                catch (Exception)
                {
                    Log.Error("MyFilms : Could not load Logo Image file... " + t);
                    return;
                }
                if (totalWidth > totalHeight)
                    scale = totalHeightf / (float)single.Size.Height;
                else
                    scale = totalWidthf / (float)single.Size.Width;
                tmp = new Size((int)(single.Width * scale), (int)(single.Height * scale));
                checkWidth += tmp.Width;
                checkHeigth += tmp.Height;
                imgSizes.Add(tmp);
                imgs.Add(single);
            }
            // step two: check if we are too big horizontally and if so scale again
            checkHeigth += imgSizes.Count * spacer;
            checkWidth += imgSizes.Count * spacer;
            if (totalWidth > totalHeight)
            {
                if (checkWidth > totalWidth)
                {
                    scale = (float)checkWidth / (float)totalWidth;
                    for (int i = 0; i < imgSizes.Count; i++)
                    {
                        imgSizes[i] = new Size((int)(imgSizes[i].Width / scale), (int)(imgSizes[i].Height / scale));
                    }
                }
            }
            else
                if (checkHeigth > totalHeight)
                {
                    scale = (float)checkHeigth / (float)totalHeight;
                    for (int i = 0; i < imgSizes.Count; i++)
                    {
                        imgSizes[i] = new Size((int)(imgSizes[i].Width / scale), (int)(imgSizes[i].Height / scale));
                    }
                }

            // step three: finally draw them
            for (int i = 0; i < imgs.Count; i++)
            {
                if (totalWidth > totalHeight)
                {
                    g.DrawImage(imgs[i], x_pos, totalHeight - imgSizes[i].Height, imgSizes[i].Width, imgSizes[i].Height);
                    x_pos += imgSizes[i].Width + spacer;
                }
                else
                {
                    g.DrawImage(imgs[i], totalWidth - imgSizes[i].Width, y_pos, imgSizes[i].Width, imgSizes[i].Height);
                    y_pos += imgSizes[i].Height + spacer;
                }

            }
        }
    
        #region conteneurs
        
        public static ArrayList ID2001Logos = new ArrayList();
        public static ArrayList ID2002Logos = new ArrayList();
        public static ArrayList ID2003Logos = new ArrayList();
        public static string LogosPath = string.Empty;

        #endregion
    }
}
