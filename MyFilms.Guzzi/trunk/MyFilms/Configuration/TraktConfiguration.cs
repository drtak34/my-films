namespace MesFilms.MyFilms.Configuration
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows.Forms;
  using System.Reflection;

  using MyFilmsPlugin.MyFilms.Trakt;
  using MyFilmsPlugin.MyFilms.Trakt.Show;
  using MyFilmsPlugin.MyFilms.Trakt.User;

  public partial class TraktConfiguration : UserControl
    {
        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log
        public TraktConfiguration()
        {
            InitializeComponent();
            LoadFromDB();

            // register text change event after loading password field as we dont want to re-hash
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
        }

        private void LoadFromDB()
        {
            // ToDo: Add MyFilms COnfigfile access here !!!
          textBoxUsername.Text = "MikePlanet";
          textBoxPassword.Text = "Password";
          bool isConfig = false;
          try
          {
            //isConfig = !System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location).Equals("mediaportal", StringComparison.InvariantCultureIgnoreCase);
          }
          catch (Exception)
          {
            {}
            throw;
          }
          TraktAPI.UserAgent = string.Format("MyFilms{0}/{1}", isConfig ? "Config" : string.Empty, Assembly.GetCallingAssembly().GetName().Version);
          TraktAPI.Username = "TraktUsername";
          TraktAPI.Password = "TraktPassword";
          //textBoxUsername.Text = DBOption.GetOptions(DBOption.cTraktUsername);
          //textBoxPassword.Text = DBOption.GetOptions(DBOption.cTraktPassword);

          //TraktAPI.UserAgent = Settings.UserAgent;
          //TraktAPI.Username = DBOption.GetOptions(DBOption.cTraktUsername);
          //TraktAPI.Password = DBOption.GetOptions(DBOption.cTraktPassword);
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            // set DBOption.SetOptions(DBOption.cTraktUsername, textBoxUsername.Text);
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            // Hash Password
            // hash DBOption.SetOptions(DBOption.cTraktPassword, textBoxPassword.Text.ToSHA1Hash());
        }

        private void textBoxPassword_Enter(object sender, EventArgs e)
        {
            // clear password field so can be re-entered easily, when re-entering config
            // it wont look like original because its hashed, so it's less confusing if cleared
            textBoxPassword.Text = string.Empty;
        }

        private void linkLabelSignUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {            
            System.Diagnostics.Process.Start(@"http://trakt.tv");
        }
        
        #region testing
        private void buttonTestAPI_Click(object sender, EventArgs e)
        {
            string username = TraktAPI.Username;

            if (string.IsNullOrEmpty(username)) return;

            LogMyFilms.Debug("Trakt: Getting Shows for user '{0}'", username);
            IEnumerable<TraktLibraryShows> showsForUser = TraktAPI.GetSeriesForUser(username);
            LogMyFilms.Debug("Show Count: {0}", showsForUser.Count().ToString());

            LogMyFilms.Debug("Trakt: Getting Show overview for Series ID: '79488'");
            TraktSeriesOverview seriesOverview = TraktAPI.GetSeriesOverview("79488");
            if (seriesOverview.Status != "failure") 
              LogMyFilms.Debug("Successfully got data for {0}", seriesOverview.Title);

            LogMyFilms.Debug("Trakt: Getting User Profile for user '{0}'", username);
            TraktUserProfile userProfile = TraktAPI.GetUserProfile(username);
            if (userProfile != null && !string.IsNullOrEmpty(userProfile.Protected)) 
              LogMyFilms.Debug("Successfully got data for {0}", userProfile.FullName);

            LogMyFilms.Debug("Trakt: Getting watched History for user '{0}'", username);
            IEnumerable<TraktWatchedEpisodeHistory> watchedHistory = TraktAPI.GetUserWatchedHistory(username);
            LogMyFilms.Debug("Watched History Count: {0}", watchedHistory.Count().ToString());
        }
        #endregion
    }
}
