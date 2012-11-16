#region Copyright (C) 2005-2011 Team MediaPortal

// Copyright (C) 2005-2011 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#endregion

namespace MusicShareWatcher
{
  using System;
  using System.Threading;
  using System.Windows.Forms;

  using MediaPortal.GUI.Library;
  using MediaPortal.Services;

  using global::ShareWatcherHelper;

  public partial class ShareWatcher : Form
  {
    private bool bMonitoring;
    private static global::ShareWatcherHelper.ShareWatcherHelper watcher = null;

    public ShareWatcher()
    {
      this.InitializeComponent();
      Thread.CurrentThread.Name = "ShareWatcher";
      this.bMonitoring = true;
      // Setup the Watching
      watcher = new global::ShareWatcherHelper.ShareWatcherHelper();
      watcher.SetMonitoring(true);
      watcher.StartMonitor();
    }

    private void monitoringEnabledMenuItem_Click(object sender, EventArgs e)
    {

    }

  }
}