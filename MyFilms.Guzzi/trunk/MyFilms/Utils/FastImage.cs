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
namespace MyFilmsPlugin.MyFilms.Utils
{
  using System;
  using System.Drawing;
  using System.IO;
  using System.Reflection;
  using System.Runtime.InteropServices;

  using MesFilms;

  using MyFilmsPlugin.MyFilms.MyFilmsGUI;

  public class ImageFast
    {
        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
        public static extern int GdipLoadImageFromFile(string filename, out IntPtr image);

        private ImageFast()
        {
        }

        private static Type imageType = typeof(System.Drawing.Bitmap);

        public static Image FastFromFile(string filename)
        {
            filename = Path.GetFullPath(filename);
            IntPtr loadingImage = IntPtr.Zero;

            // We are not using ICM at all, fudge that, this should be FAAAAAST!
            if (GdipLoadImageFromFile(filename, out loadingImage) != 0)
            {
                throw new Exception("GDI+ threw a status error code.");
            }

            return (Bitmap)imageType.InvokeMember("FromGDIplus", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { loadingImage });
        }
        public static Image CreateImage(string path, string texte)
        {
            // Loading the image to draw 
            if (!System.IO.File.Exists(path.Substring(0, path.LastIndexOf("\\") + 1) + "Default.jpg"))
                MediaPortal.Util.Picture.CreateThumbnail(MesFilms.conf.DefaultCover, path.Substring(0, path.LastIndexOf("\\") + 1) + "\\Default.jpg", 200, 400, 0, MediaPortal.Util.Thumbs.SpeedThumbsLarge);
            Image image = null;
            try
            { image = Image.FromFile(path.Substring(0, path.LastIndexOf("\\") + 1) + "\\Default.jpg"); }
            catch
            {
                return null;
            }

            // Creation of the bitmap to make the drawing 
            
  //          Bitmap bmp = new Bitmap(image.Width, image.Height);
            Bitmap bmp = new Bitmap(150, 200);
            // Creation of graphics to draw
            Graphics g = Graphics.FromImage(bmp);

            // It draws the image 
            g.DrawImage(image, 0, 0);
            // the text is cut by separator space
            int wi = 0;
            string[] Sep = {" ","-","_","&","|",",",";"};
            string[] arSplit = texte.Split(Sep, StringSplitOptions.RemoveEmptyEntries);
            float xfont  = 0;
            float  wfont = 0;
            for (wi = 0; wi < arSplit.Length; wi++)
            {
               wfont = (90) / arSplit[wi].Length * 3 / 2;
               if (xfont == 0 || wfont < xfont)
                   xfont = wfont;
            }
            // It adds a text 
            if (xfont > 64)
                xfont = 64;
            for (wi = 0; wi < arSplit.Length; wi++)
            {
                g.DrawString(arSplit[wi].ToUpper(), new Font("Courier New", xfont, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel), Brushes.White, new PointF(20, 30 + wi * xfont));
            }
            //g.DrawString("+", new Font("Arial", 64, System.Drawing.FontStyle.Bold), Brushes.White, new PointF(1, 1));
            //g.DrawString("+", new Font("Arial", 64, System.Drawing.FontStyle.Bold), Brushes.White, new PointF(1,image.Height - 64));
            //g.DrawString("+", new Font("Arial", 64, System.Drawing.FontStyle.Bold), Brushes.White, new PointF(image.Width -64, 1));
            //g.DrawString("+", new Font("Arial", 64, System.Drawing.FontStyle.Bold), Brushes.White, new PointF(image.Width-64, image.Height - 64));

            bmp.Save(path);
            return bmp;
        }
    }
}
