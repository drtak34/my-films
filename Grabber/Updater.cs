using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using NLog;

namespace Grabber
{
  public class Updater
  {
    private static Logger LogMyFilms = LogManager.GetCurrentClassLogger();  //log

    private static void UpdateScripts(string localscriptpath)
    {
      if (!System.IO.Directory.Exists(localscriptpath))
      {
        LogMyFilms.Error("Invalid script path - cannot update scripts!");
        return;
      }
      Thread updateThread = new Thread(delegate(object obj)
      {
        LogMyFilms.Info("Checking for grabber script updates ...");

        const string remotedirectory = @"http://code.google.com/p/my-films/source/browse/trunk/Installer/Config/scripts/MyFilms/";
        // ToDo: Add routine to check for all remote files
        const string ScraperUpdateFile = remotedirectory + "IMDB.xml";
        string scraperLocalFile = Path.Combine(localscriptpath, @"IMDB.xml");

        string localFile = GetTempFilename();
        if (DownloadFile(ScraperUpdateFile, localFile))
        {
          if (!FilesAreEqual(localFile, scraperLocalFile))
          {
            try
            {
              File.Copy(localFile, scraperLocalFile, true);
              LogMyFilms.Info("Grabber Script successfully updated to latest version.");
            }
            catch (Exception e)
            {
              LogMyFilms.Error("Grabber Script update failed: {0}", e.Message);
            }
          }
          else
          {
            LogMyFilms.Info("Skipping update, latest version already installed.");
          }
          try { File.Delete(localFile); }
          catch { }
        }

      })
      {
        IsBackground = true,
        Name = "Check for Grabber Script Updates"
      };

      updateThread.Start();
    }

    public static bool DownloadFile(string url, string localFile)
    {
      var webClient = new WebClient();

      try
      {
        Directory.CreateDirectory(Path.GetDirectoryName(localFile));
        if (!File.Exists(localFile))
        {
          LogMyFilms.Debug("DownloadFile() - downloading file from: {0}", url);
          webClient.DownloadFile(url, localFile);
        }
        else
        {
          LogMyFilms.Debug("DownloadFile() - file already exists - {0}", localFile);
        }
        return true;
      }
      catch (Exception ex)
      {
        LogMyFilms.Error("Download failed from '{0}' to '{1}' - Error: '{2}'", url, localFile);
        LogMyFilms.Debug("Error Reason: '{0}'", ex.Message);
        try
        {
          if (File.Exists(localFile)) File.Delete(localFile);
        }
        catch { }

        return false;
      }
    }

    private static string GetTempFilename()
    {
      string localFile = string.Empty;
      try
      {
        localFile = string.Format(@"{0}myfilmsgrabberscript_{1}.xml", Path.GetTempPath(), Guid.NewGuid());
      }
      catch (IOException)
      {
        // can happen if more than 65K temp files already
        localFile = string.Format(@"C:\myfilmsgrabberscript_{0}.xml", Guid.NewGuid());
      }
      return localFile;
    }

    private static bool FilesAreEqual(string f1, string f2)
    {
      if (!File.Exists(f1)) return false;
      if (!File.Exists(f2)) return false;

      // get file length and make sure lengths are identical
      long length = new FileInfo(f1).Length;
      if (length != new FileInfo(f2).Length)
        return false;

      byte[] buf1 = new byte[4096];
      byte[] buf2 = new byte[4096];

      // open both for reading
      using (FileStream stream1 = File.OpenRead(f1))
      using (FileStream stream2 = File.OpenRead(f2))
      {
        // compare content for equality
        int b1, b2;
        while (length > 0)
        {
          // figure out how much to read
          int toRead = buf1.Length;
          if (toRead > length)
            toRead = (int)length;
          length -= toRead;

          // read a chunk from each and compare
          b1 = stream1.Read(buf1, 0, toRead);
          b2 = stream2.Read(buf2, 0, toRead);
          for (int i = 0; i < toRead; ++i)
            if (buf1[i] != buf2[i])
              return false;
        }
      }
      return true;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      const string url = "http://code.google.com/p/my-films/source/browse/trunk/Installer/Config/scripts/MyFilms/";
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
      {
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
          string html = reader.ReadToEnd();
          Regex regex = new Regex(GetDirectoryListingRegexForUrl(url));
          MatchCollection matches = regex.Matches(html);
          if (matches.Count > 0)
          {
            foreach (Match match in matches)
            {
              if (match.Success)
              {
                string fName = match.Groups["name"].ToString();
                Console.WriteLine(fName);

                // Create an instance of WebClient
                WebClient client = new WebClient();

                // Hookup DownloadFileCompleted Event
                client.DownloadFileCompleted +=
                  new System.ComponentModel.AsyncCompletedEventHandler(client_DownloadFileCompleted);

                // Start the download and copy the file to c:\temp
                client.DownloadFileAsync(new Uri(url + fName), @"c:\temp\" + fName);
              }
            }
          }
        }
      }
      Console.ReadLine();
    }

    void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
      //MessageBox.Show("File downloaded");
      //Console.WriteLine("File downloaded");
    }

    public string GetDirectoryListingRegexForUrl(string url)
    {
      if (url.Equals("http://code.google.com/p/my-films/source/browse/trunk/Installer/Config/scripts/MyFilms/"))
      {
        return "(?.*)";
      }
      throw new NotSupportedException();
    }

  }
}