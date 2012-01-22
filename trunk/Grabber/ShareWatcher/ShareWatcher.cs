using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using global::ShareWatcherHelper;

namespace ShareWatcherHelper
{
  using System.Net.Mime;
  using System.Threading;

  public class ShareWatcher
  {
    private bool bMonitoring;
    private static global::ShareWatcherHelper.ShareWatcherHelper watcher = null;

    public ShareWatcher()
    {
      Thread.CurrentThread.Name = "ShareWatcher";
      this.bMonitoring = true;
      // Setup the Watching
      watcher = new global::ShareWatcherHelper.ShareWatcherHelper();
      watcher.SetMonitoring(true);
      watcher.StartMonitor();
    }

    #region CommonMethods

    // Enable / Disable the monitoring of shares
    private void monitoringEnabledMenuItem_Click(object sender, EventArgs e)
    {
      if (this.bMonitoring)
      {
        this.bMonitoring = false;
        //this.monitoringEnabledMenuItem.Checked = false;
        watcher.ChangeMonitoring(false);
      }
      else
      {
        this.bMonitoring = true;
        //this.monitoringEnabledMenuItem.Checked = true;
        watcher.ChangeMonitoring(true);
      }
    }

    //// React on Windows System Shutdown
    //private const int WM_QUERYENDSESSION = 0x11;

    //protected override void WndProc(ref Message msg)
    //{
    //  if (msg.Msg == WM_QUERYENDSESSION)
    //  {
    //    // If system is shutting down, allow exit.
    //    MediaTypeNames.Application.Exit();
    //  }
    //  base.WndProc(ref msg);
    //}

    #endregion CommonMethods
  }
 
}
