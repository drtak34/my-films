//css_reference "MpeCore.dll";

using MpeCore.Classes;
using MpeCore;
using System.IO;
using System.Windows.Forms;
using MediaPortal.Common.Utils;

public class Script
{
  public static void Main(PackageClass packageClass, ActionItem actionItem)
  {
    System.Version MpSkinVersion = MediaPortal.Common.Utils.CompatibilityManager.SkinVersion;
    System.Version NewVersion = new System.Version(1, 3, 0, 0);
    if (MpeCore.Classes.VersionProvider.MediaPortalVersion.MinimumMPVersionRequired.ToString() == "1.2.100.0")
    {
      
    }

    if (MpSkinVersion.CompareTo(NewVersion) >= 0)
    {
      if (!packageClass.Silent)
      {
        MessageBox.Show("MP 1.3.x detected ! Now updating skin files ...");
      }
      string myFilmsDefaultWideFolder = Path.Combine(MpeCore.MpeInstaller.TransformInRealPath("%Config%"), @"MyFilms\Skin.12\DefaultWide");
      string defaultWideFolder = Path.Combine(MpeCore.MpeInstaller.TransformInRealPath("%Skin%"), "DefaultWide");
      if (Directory.Exists(myFilmsDefaultWideFolder) && Directory.Exists(defaultWideFolder))
      {
        CopyFiles(myFilmsDefaultWideFolder, defaultWideFolder, true);
      }
             
      myFilmsDefaultWideFolder = Path.Combine(MpeCore.MpeInstaller.TransformInRealPath("%Config%"), @"MyFilms\Skin.12\Default");
      defaultWideFolder = Path.Combine(MpeCore.MpeInstaller.TransformInRealPath("%Skin%"), "Default");
      if (Directory.Exists(myFilmsDefaultWideFolder) && Directory.Exists(defaultWideFolder))
      {
        CopyFiles(myFilmsDefaultWideFolder, defaultWideFolder, true);
      }

      return;
    }
    if (MpeCore.Classes.VersionProvider.MediaPortalVersion.MinimumMPVersionRequired.ToString() == "1.1.6.27644")
    {
      if (!packageClass.Silent)
      {
        MessageBox.Show("MP 1.2.x detected - using MP1.2 Skin Files!");
      }
    }
    return;
  }

  public static string FindSuitablePath(string folder)
  {
    string result = null;
    for (int i = 0; i < 100; i++)
    {
      string dir = folder + "_Backup" + i.ToString();
      if (!Directory.Exists(dir))
      {
        result = dir;
        break;
      }
    }
    return result;
  }

  public static void CopyFiles(string path, string nPath, bool recursive)
  {
    foreach (string file in Directory.GetFiles(path))
    {
      if ((File.GetAttributes(file) & FileAttributes.Hidden) == FileAttributes.Hidden)
        continue;
      if ((File.GetAttributes(file) & FileAttributes.System) == FileAttributes.System)
        continue;
      if (file.EndsWith("Thumbs.db"))
        continue;
      string destFile = Path.Combine(nPath, Path.GetFileName(file));
      File.Copy(file, destFile, true);

    }
    if (recursive)
    {
      foreach (string dir in Directory.GetDirectories(path))
      {
        if ((File.GetAttributes(dir) & FileAttributes.Hidden) == FileAttributes.Hidden)
          continue;
        string destDir = Path.Combine(nPath, dir.Substring(dir.LastIndexOf('\\') + 1));
        Directory.CreateDirectory(destDir);
        CopyFiles(dir, destDir, recursive);
      }
    }
  }

}

