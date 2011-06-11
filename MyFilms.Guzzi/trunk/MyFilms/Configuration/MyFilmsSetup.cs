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

namespace MyFilmsPlugin.MyFilms.Configuration
{
  using System;
  using System.Data;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Net;
  using System.Windows.Forms;
  using System.Xml;

  using IWshRuntimeLibrary;

  using MediaPortal.Configuration;

  using MyFilmsPlugin.MyFilms.CatalogConverter;
  using MyFilmsPlugin.MyFilms.Utils;
  
  using TaskScheduler;

  public partial class MyFilmsSetup : Form
    {
        //private WshShellClass WshShell; // Added for creating Desktop icon via wsh

        //fmu   private MediaPortal.Profile.Settings MyFilms_xmlwriter = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"));
        //fmu   private MediaPortal.Profile.Settings MyFilms_xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"));
        XmlConfig XmlConfig = new XmlConfig();
        //XmlSettings XmlConfig = new XmlSettings(Config.GetFile(Config.Dir.Config, "MyFilms.xml")); //Guzzi

        private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

        static ScheduledTasks st = null;

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private int MesFilms_nb_config = 0;
        private string MyFilms_PluginMode = String.Empty; // "normal" for standard operations, "test" for extended functionalities, to be set manually in MyFilms.XML
        private string StrDfltSelect = string.Empty;
        private AntMovieCatalog mydivx = new AntMovieCatalog();
        private Crypto crypto = new Crypto();
        public int selected_Logo_Item;
        public bool load = true;
        public DataSet AMCdsSettings = new DataSet();
        TabPage tabPageSave = null; // Zwischenspeicher für TabPage
        private bool StoreFullLogoPath = false;

        private bool WizardActive = false; // Status of running new config wizard (to control check behaviour)
        private bool RunWizardAfterInstall = false; // Will only be set true after first blank install, when no MyFilms.xml config is present to launch Wizard.
        private bool NewConfigButton = false; // Will avid that catalogselectedindex-changed will be run on "New!" config...
        private string ActiveLogoPath = String.Empty;

        public MyFilmsSetup()
        {
            InitializeComponent();
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            this.label_VersionNumber.Text = "Version " + asm.GetName().Version.ToString() + " beta";
        }


        private void MesFilmsSetup_Load(object sender, EventArgs e)
        {
            Refresh_Items(true);
            if (!System.IO.File.Exists(Config.GetFolder(Config.Dir.Config) + @"\" + "MyFilms" + ".xml"))
              RunWizardAfterInstall = true;
            textBoxPluginName.Text = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "PluginName", "Films");
            MyFilms_PluginMode = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "PluginMode", "normal"); // Read Plugin Start Mode to diable/anable normal vs. testfeatures
            LogMyFilms.Info("MyFilms Setup ********** OperationsMode (PluginMode): '" + MyFilms_PluginMode + "' **********");

            if (MyFilms_PluginMode == "normal") // disable Trakt and other controls in standardmode
            {
              //hide a tab by removing it from the TabPages collection
              this.tabPageSave = General.TabPages[10];
              this.General.TabPages.Remove(this.tabPageSave); // Disable Trakt Tab, as it's not yet implemented
              this.checkWatchedInProfile.Visible = false; // Disable Watched options for Userprofiles, as it's not yet implemented
              this.Label_UserProfileName.Visible = false;
              this.UserProfileName.Visible = false;
              this.SearchSubDirsTrailer.Visible = false; // Disable Trailer options, that are not yet implemented
              this.SearchFileNameTrailer.Visible = false;
              this.ItemSearchFileNameTrailer.Visible = false;
              this.ShowTrailerPlayDialog.Visible = false;
              this.ShowTrailerWhenStartingMovie.Visible = false;
              this.btnGrabberInterface.Visible = false; // disable grabber interface in normal mode
              this.buttonOpenTmpFile.Visible = false; // disable button to open tmp catalog in editor on EC tab
              this.buttonDeleteTmpCatalog.Visible = false; // disable button to delete tmp catalog on EC tab
              this.groupBoxAMCsettings.Visible = false; // disable groupbox with setting for AMC exe path
              this.buttonOpenTmpFileAMC.Visible = false; // disable Launch Button to start AMC with Catalogs externally
              // Remove unused Catalog types -- also changes index, so doesn't work with existing code !
              CatalogType.Items.Remove("MyFilms extended Database");
              CatalogType.Items.Remove("XBMC nfo reader");
              //CatalogType.Items.Remove(CatalogType.Items[7]); // MF internal DB
              //CatalogType.Items.RemoveAt(8); // XBMC nfo reader (deparate files)
              //CatalogType.Items.Add("test");
            }
            //else
            //{
            //  //show a tab by adding it to the TabPages collection
            //  if (this.tabPageSave != null)
            //  {
            //    int loc = General.SelectedIndex;
            //    this.General.TabPages.Insert(loc, this.tabPageSave);
            //  }
            //}
//"Ant Movie Catalog (V3.5.1.2)",
//"DVD Profiler (V3.7.2)",
//"Movie Collector (V7.1.4)",
//"MyMovies (V3.18)",
//"Eax Movie Catalog (2.5.0)",
//"eXtreme Movie Manager (V7.1.1.1)",
//"XBMC (V10.0)",
//"MyFilms extended Database",
//"XBMC nfo reader",
//"Eax Movie Catalog (3.0.9 b5)",
//"PVD - Personal Video Database (0.9.9.21)",
//"MovingPicturesXML (V1.2 process plugin)"});

            //this.label_VersionNumber.Text = "Version 5.1.0 alpha";
            LogMyFilms.Info("MFsetup: Started with version '" + label_VersionNumber.Text + "'");


            MesFilms_nb_config = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
            //            for (int i = 0; i < (int)MesFilms_nb_config; i++)
            //            {
            ////                Config_Name.Items.Add(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, ""));
            //                XmlConfig.RemoveEntry("MyFilms", "MyFilms", "ConfigName" + i);
            //            }
            if (MesFilms_nb_config > 0)
            {
                if (XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Menu_Config", false))
                    Config_Menu.Checked = true;
                else
                    Config_Menu.Checked = false;
            }
            for (int i = 0; i < (int)MesFilms_nb_config; i++)
            {
              Config_Name.Items.Add(XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, string.Empty));
            }

            AntMovieCatalog ds = new AntMovieCatalog();
            AntStorage.Items.Add("(none)");
            AntStorageTrailer.Items.Add("(none)");
            AntTitle2.Items.Add("(none)");
            AntSort1.Items.Add("(none)");
            AntSort2.Items.Add("(none)");
            Sort.Items.Add("(none)");
            AntIdentItem.Items.Add("(none)");
            //AntFilterMinRating.Items.Add("(none)");
            AntFilterItem1.Items.Add("(none)");
            AntFilterItem2.Items.Add("(none)");
            AntFilterItem3.Items.Add("(none)");
            AntFilterItem4.Items.Add("(none)");
            AntViewItem1.Items.Add("(none)");
            AntViewItem2.Items.Add("(none)");
            AntViewItem3.Items.Add("(none)");
            AntViewItem4.Items.Add("(none)");
            AntViewItem5.Items.Add("(none)");
            AntSearchItem1.Items.Add("(none)");
            AntSearchItem2.Items.Add("(none)");
            AntUpdItem1.Items.Add("(none)");
            AntUpdItem2.Items.Add("(none)");
            AntItem1.Items.Add("(none)");
            AntItem2.Items.Add("(none)");
            AntItem3.Items.Add("(none)");
            AntItem4.Items.Add("(none)");
            AntItem5.Items.Add("(none)");
            CmdPar.Items.Add("(none)");
            CatalogType.SelectedIndex = 0;

            foreach (DataColumn dc in ds.Movie.Columns)
            {
              if (dc.ColumnName != "Picture" && dc.ColumnName != "Fanart" && dc.ColumnName != "Contents_Id" && dc.ColumnName != "IMDB_Id" && dc.ColumnName != "TMDB_Id" 
                && dc.ColumnName != "Watched" && dc.ColumnName != "DateWatched" && dc.ColumnName != "Certification" && dc.ColumnName != "Writer" && dc.ColumnName != "SourceTrailer" 
                && dc.ColumnName != "TagLine" && dc.ColumnName != "Tags" && dc.ColumnName != "RatingUser" && dc.ColumnName != "Studio" && dc.ColumnName != "IMDB_Rank" 
                && dc.ColumnName != "Edition" && dc.ColumnName != "IsOnline" && dc.ColumnName != "IsOnlineTrailer" && dc.ColumnName != "Aspectratio") 
                // All those fieds are currently not supported by ANT-MC - they will be added, if CatalogType changes to external catalog to the respective fields
                // Also removed Contents_Id and Pictures, as they mostly useless.
              {
                if ((dc.ColumnName == "MediaLabel") || (dc.ColumnName == "MediaType") || (dc.ColumnName == "Source") ||
                    (dc.ColumnName == "URL") || (dc.ColumnName == "Comments") || (dc.ColumnName == "Borrower") ||
                    (dc.ColumnName == "Languages") || (dc.ColumnName == "Subtitles"))
                {
                  AntStorage.Items.Add(dc.ColumnName);
                  AntStorageTrailer.Items.Add(dc.ColumnName);
                }

                if ((dc.ColumnName == "TranslatedTitle") || (dc.ColumnName == "OriginalTitle") || (dc.ColumnName == "FormattedTitle"))
                {
                  //AntTitle1.Items.Add(dc.ColumnName); // Fields already added in Controls definition
                  AntTitle2.Items.Add(dc.ColumnName);
                  AntSTitle.Items.Add(dc.ColumnName);
                }
                if ((dc.ColumnName != "Contents_Id") && (dc.ColumnName != "Length_Num") && (dc.ColumnName != "DateAdded")) // added "DatedAdded" to remove filter
                {
                  AntFilterItem1.Items.Add(dc.ColumnName);
                  AntFilterItem2.Items.Add(dc.ColumnName);
                  AntItem1.Items.Add(dc.ColumnName);
                  AntItem2.Items.Add(dc.ColumnName);
                  AntItem3.Items.Add(dc.ColumnName);
                  AntItem4.Items.Add(dc.ColumnName);
                  AntItem5.Items.Add(dc.ColumnName);
                }
                if ((dc.ColumnName != "Contents_Id") && (dc.ColumnName != "DateAdded") && (dc.ColumnName != "Length_Num"))
                {
                  AntSearchField.Items.Add(dc.ColumnName);
                  AntUpdField.Items.Add(dc.ColumnName);
                }
                if ((dc.ColumnName != "Contents_Id") && (dc.ColumnName != "Number") &&
                    (dc.ColumnName != "OriginalTitle") && (dc.ColumnName != "TranslatedTitle") &&
                    (dc.ColumnName != "Comments") && (dc.ColumnName != "Description") &&
                    (dc.ColumnName != "FormattedTitle") && (dc.ColumnName != "Date") && (dc.ColumnName != "DateAdded") &&
                    (dc.ColumnName != "Rating") && (dc.ColumnName != "Size") && (dc.ColumnName != "Picture") &&
                    (dc.ColumnName != "URL") && (dc.ColumnName != "Length_Num"))
                {
                  SField1.Items.Add(dc.ColumnName);
                  SField2.Items.Add(dc.ColumnName);
                }
                if ((dc.ColumnName != "Contents_Id") && dc.ColumnName != "TranslatedTitle" &&
                    dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" &&
                    dc.ColumnName != "Description" && dc.ColumnName != "Comments" && dc.ColumnName != "Picture" &&
                    dc.ColumnName != "Length_Num" && (dc.ColumnName != "Number"))
                {
                  AntViewItem1.Items.Add(dc.ColumnName);
                  AntViewItem2.Items.Add(dc.ColumnName);
                  AntViewItem3.Items.Add(dc.ColumnName);
                  AntViewItem4.Items.Add(dc.ColumnName);
                  AntViewItem5.Items.Add(dc.ColumnName);
                }
                if ((dc.ColumnName != "Contents_Id") && dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle"
                  && dc.ColumnName != "Actors" && dc.ColumnName != "Length_Num" && dc.ColumnName != "Picture" && dc.ColumnName != "DateAdded")
                {
                  AntSearchItem1.Items.Add(dc.ColumnName);
                  AntSearchItem2.Items.Add(dc.ColumnName);
                }
                if ((dc.ColumnName != "Contents_Id") && dc.ColumnName != "TranslatedTitle" &&
                    dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" && dc.ColumnName != "Year" &&
                    dc.ColumnName != "Picture" && dc.ColumnName != "Length" && dc.ColumnName != "Rating" &&
                    dc.ColumnName != "DateAdded" && // disabled for Doug testing
                    dc.ColumnName != "Date")
                {
                  AntSort1.Items.Add(dc.ColumnName);
                  AntSort2.Items.Add(dc.ColumnName);
                  AntIdentItem.Items.Add(dc.ColumnName);
                }
                if ((dc.ColumnName != "Length_Num") && (dc.ColumnName != "DateAdded"))
                {
                  AntUpdItem1.Items.Add(dc.ColumnName);
                  AntUpdItem2.Items.Add(dc.ColumnName);
                  cbfdupdate.Items.Add(dc.ColumnName);
                  cbWatched.Items.Add(dc.ColumnName);
                  CmdPar.Items.Add(dc.ColumnName);
                }
              }
              else // Fields not supported by AMC - (dc.ColumnName != "Picture") && (dc.ColumnName != "Contents_Id") && (dc.ColumnName != "IMDB_Id") && (dc.ColumnName != "TMDB_Id") && (dc.ColumnName != "Watched") && (dc.ColumnName != "Certification")
              {
                if (CatalogType.SelectedIndex == 7 && (dc.ColumnName == "IMDB_Id" || dc.ColumnName == "TMDB_Id" || dc.ColumnName == "Watched" || dc.ColumnName == "Certification"))
                {
                  AntSearchField.Items.Add(dc.ColumnName);
                  AntUpdField.Items.Add(dc.ColumnName);
                }
              }
            }
            AntViewText_Change();
            AntSort_Change();
            Config_Name.Text = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", string.Empty);
            chkLogos.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Logos", true); // Changed default to "true" - hope, it works even without fiull working config ....

            // Show number of configs available ...
            textBoxNBconfigs.Text = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", string.Empty);

            st = new ScheduledTasks();
            Task t = null; ;
            string name = string.Empty;
            if (MesFilmsCat.Text.LastIndexOf("\\") > 0)
            {
                name = MesFilmsCat.Text.Substring(MesFilmsCat.Text.LastIndexOf("\\") + 1);
                name = name.Substring(0, name.Length - 4);
            }
            try
            {
                t = st.OpenTask("MyFilms_AMCUpdater_" + name);
            }
            catch (ArgumentException)
            {
            }
            if (t != null)
                scheduleAMCUpdater.Checked = true;
            else
                scheduleAMCUpdater.Checked = false;
            load = false;
            // Now start SetupWizard, if it's a clean new install
            if (RunWizardAfterInstall)
            {
              WizardActive = true;
              newCatalogWizard();
              RunWizardAfterInstall = false;
              WizardActive = false;
            }
        }
        private void ButCat_Click(object sender, System.EventArgs e)
        {
          if (!string.IsNullOrEmpty(MesFilmsCat.Text))
              openFileDialog1.FileName = MesFilmsCat.Text;
            else
            {
              openFileDialog1.FileName = String.Empty;
              //if (System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config) + @"\Thumbs\MyFilms\Catalog\"))
              //  openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\Thumbs\MyFilms\Catalog\";
              //else
                // openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\";
                openFileDialog1.InitialDirectory = "";
            }

            if (MesFilmsCat.Text.Contains("\\"))
              openFileDialog1.InitialDirectory = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\") + 1);

            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "xml";
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Select Movie Catalog File (xml)";
            openFileDialog1.CheckFileExists = false;
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                Control_Database(openFileDialog1.FileName);
            }
        }
        private void Control_Database(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                //Ant Movie Catalog (V3.5.1.2)
                //DVD Profiler
                //Movie Collector
                //MyMovies
                //Eax Movie Catalog
                //eXtreme Movie Manager (V7.1.0.2)
                //XBMC (V10.0)
                //MyFilms DB (ANT with extended Database Fields)
              if (CatalogType.SelectedIndex == 0 || CatalogType.SelectedIndex == 10 || CatalogType.SelectedIndex == 11) // AMC, AMCextended or XBMC NFO reader
                {
                    if (System.Windows.Forms.MessageBox.Show("That File doesn't exists, do you want to create it ?", "Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        XmlTextWriter destXml = new XmlTextWriter(filename, System.Text.Encoding.Default);
                        destXml.Formatting = Formatting.Indented;
                        destXml.WriteStartDocument();
                        destXml.WriteStartElement("AntMovieCatalog");
                        destXml.WriteStartElement("Catalog");
                        destXml.WriteElementString("Properties", string.Empty);
                        destXml.WriteStartElement("Contents");
                        destXml.Close();
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("You have to select a valid file !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        MesFilmsCat.Focus();
                        return;
                    }
                }
                else
                {

                    System.Windows.Forms.MessageBox.Show("You have to selected a valid file !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    MesFilmsCat.Focus();
                    return;

                }
            }
            MesFilmsCat.Text = filename;
            if (MesFilmsImg.Text.Length == 0)
                MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf(@"\"));

        }

        private void ButImg_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(MesFilmsImg.Text))
              folderBrowserDialog1.SelectedPath = MesFilmsImg.Text;
            else
              folderBrowserDialog1.SelectedPath = String.Empty;
            folderBrowserDialog1.Description = "Select Cover Images Path"; 
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
              MesFilmsImg.Text = folderBrowserDialog1.SelectedPath;
              if (!string.IsNullOrEmpty(MesFilmsCat.Text))
              {
                string catalogDirectory = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\"));
                string imgPrefixFilenameOnly = "";
                if (MesFilmsImg.Text == catalogDirectory && CatalogType.SelectedIndex == 0)  // only if not external catalog
                  cbPictureHandling.Text = "Relative Path";
                else
                  cbPictureHandling.Text = "Full Path"; // set as default, unless other is possible
                if (MesFilmsImg.Text.StartsWith(catalogDirectory) && MesFilmsImg.Text != catalogDirectory && CatalogType.SelectedIndex == 0) // only if not external catalog
                {
                  MesFilmsImg.Text = catalogDirectory;
                  cbPictureHandling.Text = "Relative Path";
                  if (string.IsNullOrEmpty(txtPicturePrefix.Text))
                  {
                    txtPicturePrefix.Text = "";
                    txtPicturePrefix.Text = folderBrowserDialog1.SelectedPath.Substring(catalogDirectory.Length + 1) + "\\";
                  }
                  else if (!txtPicturePrefix.Text.Contains("\\")) 
                  {
                      txtPicturePrefix.Text = folderBrowserDialog1.SelectedPath.Substring(catalogDirectory.Length + 1) + "\\" + txtPicturePrefix.Text;
                  }
                  else
                  {
                    imgPrefixFilenameOnly = txtPicturePrefix.Text.Substring(txtPicturePrefix.Text.LastIndexOf("\\") + 1);
                    txtPicturePrefix.Text = folderBrowserDialog1.SelectedPath.Substring(catalogDirectory.Length + 1) + "\\" + imgPrefixFilenameOnly;
                  }
                }
                else if (txtPicturePrefix.Text.Contains("\\"))
                {
                  txtPicturePrefix.Text = txtPicturePrefix.Text.Substring(txtPicturePrefix.Text.LastIndexOf("\\") + 1);
                }
              }
            }
        }
        // Display any warnings or errors.

        private void ButSave_Click(object sender, EventArgs e)
        {
            // update AMCupdater config with latest MyFilms settings
            // Read_XML_AMCconfig(Config_Name.Text); // read current (or create new default) config file // reading already done in "load MF settings)
            CreateMyFilmsDefaultsForAMCconfig(Config_Name.Text); //create MF defaults
            Save_XML_AMCconfig(Config_Name.Text); // save new config

            // Save MF config
            Save_Config();
            Read_XML_AMCconfig(Config_Name.Text); // reread config file with new defaults
        }

        private void Save_Config()
        {
            if (Config_Name.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("The Configuration's Name is Mandatory !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Config_Name.Focus();
                return;
            }
            if ((Config_Dflt.Checked) && (Config_Menu.Checked))
            {
                System.Windows.Forms.MessageBox.Show("Option 'Always Display Configuration Menu' not possible with a Default Configuration defined !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Config_Menu.Focus();
                return;
            }
            //if (textBoxPluginName.Text.Length == 0)
            //{
            //    System.Windows.Forms.MessageBox.Show("The Plugin's Name is Mandatory !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //    textBoxPluginName.Focus();
            //    return;
            //}
            if (AntTitle1.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("The Master Title is Mandatory !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntTitle1.Focus();
                return;
            }
            if ((SearchFileName.Checked) && (ItemSearchFileName.Text.Length == 0))
            {
              System.Windows.Forms.MessageBox.Show("The Field used for searching by Movie's File Name is mandatory  !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
              ItemSearchFileName.Focus();
              return;
            }
            if ((!string.IsNullOrEmpty(txtGrabber.Text)) && (ItemSearchGrabberName.Text.Length == 0))
            {
              System.Windows.Forms.MessageBox.Show("The Field used for searching internet data by Movie's Name is mandatory  !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
              General.SelectedIndex = 5;
              ItemSearchGrabberName.Focus();
              return;
            }
            if (!string.IsNullOrEmpty(txtGrabber.Text) && string.IsNullOrEmpty(cbPictureHandling.Text))
              {
              System.Windows.Forms.MessageBox.Show("The Field used for selecting if cover image name to be stored with relative or absolute path is mandatory  !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
              General.SelectedIndex = 6;
              cbPictureHandling.Focus();
              return;
            }
            if (AntViewItem1.Text == "Rating")
            {
                System.Windows.Forms.MessageBox.Show("View by Rating not possible", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntViewItem1.Text = null;
                AntViewText1.Clear();
                AntViewItem1.Focus();
                return;
            }
            if (AntViewItem2.Text == "Rating")
            {
                System.Windows.Forms.MessageBox.Show("View by Rating not possible", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntViewItem2.Text = null;
                AntViewText2.Clear();
                AntViewItem2.Focus();
                return;
            }
            if (AntViewItem3.Text == "Rating")
            {
                System.Windows.Forms.MessageBox.Show("View by Rating not possible", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntViewItem3.Text = null;
                AntViewText3.Clear();
                AntViewItem3.Focus();
                return;
            }
            if (AntViewItem4.Text == "Rating")
            {
                System.Windows.Forms.MessageBox.Show("View by Rating not possible", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntViewItem4.Text = null;
                AntViewText4.Clear();
                AntViewItem4.Focus();
                return;
            }
            if (AntViewItem5.Text == "Rating")
            {
                System.Windows.Forms.MessageBox.Show("View by Rating not possible", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntViewItem5.Text = null;
                AntViewText5.Clear();
                AntViewItem5.Focus();
                return;
            }
            if (AntViewItem1.Text.Length == 0)
                AntViewItem1.Text = "(none)";
            if (!(AntViewItem1.Text == "(none)") && (AntViewText1.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("The Supplementary View Label is mandatory with corresponding Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntViewText1.Focus();
                return;
            }
            if (AntViewItem2.Text.Length == 0)
                AntViewItem2.Text = "(none)";
            if (!(AntViewItem2.Text == "(none)") && (AntViewText2.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("The Supplementary View Label is mandatory with corresponding Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntViewText2.Focus();
                return;
            }
            if (AntViewItem3.Text.Length == 0)
                AntViewItem3.Text = "(none)";
            if (!(AntViewItem3.Text == "(none)") && (AntViewText3.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("The Supplementary View Label is mandatory with corresponding Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntViewText3.Focus();
                return;
            }
            if (AntViewItem4.Text.Length == 0)
                AntViewItem4.Text = "(none)";
            if (!(AntViewItem4.Text == "(none)") && (AntViewText4.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("The Supplementary View Label is mandatory with corresponding Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntViewText4.Focus();
                return;
            }
            if (AntViewItem5.Text.Length == 0)
                AntViewItem5.Text = "(none)";
            if (!(AntViewItem5.Text == "(none)") && (AntViewText5.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("The Supplementary View Label is mandatory with corresponding Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntViewText5.Focus();
                return;
            }
            if (View_Dflt_Item.Text.Length == 0)
                View_Dflt_Item.Text = "(none)";
            if (AntFilterItem1.Text.Length == 0)
                AntFilterItem1.Text = "(none)";
            if (!(AntFilterItem1.Text == "(none)") && (AntFilterSign1.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("Symbol for Filter comparison must be '=' or '#'", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntFilterSign1.Focus();
                return;
            }
            if (!(AntFilterItem1.Text == "(none)") && (AntFilterText1.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("Length of Filter Text Item must be > 0", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntFilterText1.Focus();
                return;
            }
            if (AntFilterItem2.Text.Length == 0)
                AntFilterItem2.Text = "(none)";
            if (!(AntFilterItem2.Text == "(none)") && (AntFilterSign2.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("Symbol for Filter comparison must be '=' or '#'", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntFilterSign2.Focus();
                return;
            }
            if (!(AntFilterItem2.Text == "(none)") && (AntFilterText2.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("Length of Filter Text Item must be > 0", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntFilterText2.Focus();
                return;
            }
            if (!(AntFilterItem1.Text == "(none)") && !(AntFilterItem1.Text == "(none)"))
            {
                if (AntFilterComb.Text.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Must be 'or' or 'and' for filter combination", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    AntFilterComb.Focus();
                    return;
                }
            }
            if (AntFilterItem1.Text == "DateAdded")
            {
                try
                {
                    DateTime wdate = Convert.ToDateTime(AntFilterText1.Text);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Your Date has not a valid format; try your local format (ex : DD/MM/YYYY)", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    AntFilterText1.Focus();
                }
            }
            if (AntFilterItem2.Text == "DateAdded")
            {
                try
                {
                    DateTime wdate = Convert.ToDateTime(AntFilterText2.Text);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Your Date has not a valid format; try your local format (ex : DD/MM/YYYY)", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    AntFilterText2.Focus();
                }
            }
            if (AntSort1.Text.Length == 0)
                AntSort1.Text = "(none)";
            if (!(AntSort1.Text == "(none)") && (AntTSort1.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("The Sortby Label is mandatory with Sortby Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntTSort1.Focus();
                return;
            }
            if (AntSort2.Text.Length == 0)
                AntSort2.Text = "(none)";
            if (!(AntSort2.Text == "(none)") && (AntTSort2.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("The Sortby Label is mandatory with Sortby Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntTSort2.Focus();
                return;
            } if (AntUpdItem1.Text.Length == 0)
                AntUpdItem1.Text = "(none)";
            if (!(AntUpdItem1.Text == "(none)") && (AntUpdText1.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("The Button Label is mandatory with the corresponding Update Field Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntUpdText1.Focus();
                return;
            }
            if (AntUpdItem2.Text.Length == 0)
                AntUpdItem2.Text = "(none)";
            if (!(AntUpdItem2.Text == "(none)") && (AntUpdText2.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("The Button Label is mandatory with the corresponding Update Field Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntUpdText2.Focus();
                return;
            }
            if (AntSearchItem1.Text.Length == 0)
                AntSearchItem1.Text = "(none)";
            if (!(AntSearchItem1.Text == "(none)") && (AntSearchText1.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("The Supplementary Search Label is mandatory with corresponding Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntSearchText1.Focus();
                return;
            }
            if (AntSearchItem2.Text.Length == 0)
                AntSearchItem2.Text = "(none)";
            if (!(AntSearchItem2.Text == "(none)") && (AntSearchText2.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("The Supplementary Search Label is mandatory with corresponding Item !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                AntSearchText2.Focus();
                return;
            }
            if (Dwp.Text.Length > 0)
                if (Dwp.Text != Rpt_Dwp.Text)
                {
                    System.Windows.Forms.MessageBox.Show("The two Passwords must be identical !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Dwp.Clear();
                    Rpt_Dwp.Focus();
                    Dwp.Focus();
                    return;
                }
            if (chkAMCUpd.Checked)
                if (txtGrabber.Text.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Grabber Config File Name is Mandatory for detail Internet Update function !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtGrabber.Focus();
                    return;
                }
            if (chkAMCUpd.Checked)
            {
                if (txtAMCUpd_cnf.Text.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("AMCUpdater Config File Name is Mandatory for detail Internet Update function !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtAMCUpd_cnf.Focus();
                    return;
                }
            }
            if ((chksupplaystop.Checked) && (!chkSuppress.Checked))
            {
                System.Windows.Forms.MessageBox.Show("Suppress action must be enabled for that choice !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                chksupplaystop.Focus();
                return;
            }
            if (chkSuppress.Checked)
                if (((rbsuppress3.Checked) || (rbsuppress4.Checked)) && ((cbfdupdate.Text.Length == 0) || (txtfdupdate.Text.Length == 0)))
                {
                    System.Windows.Forms.MessageBox.Show("For updating entry, field and value are mandatory !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    cbfdupdate.Focus();
                    return;
                }
            //if (MesFilmsImg.Text.Length > 0 || MesFilmsFanart.Text.Length > 0)
            //{
            //    if (MesFilmsImg.Text == MesFilmsFanart.Text && CatalogType.SelectedIndex != 0)
            //    {
            //        System.Windows.Forms.MessageBox.Show("Picture and Fanart Path can't be the same !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        MesFilmsFanart.Focus();
            //        return;
            //    }
            //}
            if ((chkFanart.Checked) && (MesFilmsFanart.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("Fanart Path must be fill for using !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                MesFilmsFanart.Focus();
                return;
            }
            Selected_Enreg_TextChanged();
            if (View_Dflt_Item.Text.Length == 0)
                View_Dflt_Item.Text = "(none)";

            // Delete temporary catalog file
            string destFile = string.Empty;
            switch (CatalogType.SelectedIndex)
            {
              case 0: // ANT Movie Catalog
              case 10:// Starter Settings ANT extended DB
                break;
              case 1: //DVD Profiler
              case 2: // Movie Collector V7.1.4
              case 3: // MyMovies
              case 4: // EAX Movie Catalog 2.5.0
              case 5: // EAX Movie Catalog 3.0.9 (beta5)
              case 6: // PVD PersonalVideoDatabase V0.9.9.21
              case 7: // eXtreme Movie Manager
              case 8: // XBMC fulldb export (all movies in one DB)
              case 9: // MovingPicturesXML V1.2
                destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                break;
              case 11: // XBMC Nfo (separate nfo files, to scan dirs - MovingPictures or XBMC)
                destFile = MesFilmsCat.Text;
                break;
              default:
                break;
            }
            if (destFile.Length > 0)
              if (System.IO.File.Exists(destFile))
              {
                try
                {
                  System.IO.File.Delete(destFile);
                }
                catch (Exception) {}
                LogMyFilms.Debug("MyFilmsSetup: Automatically deleted tmp catalog (Save action): '" + destFile + "'");
              }

            //Verify_Config(); // Also (re) imports the external catalog data
            if (Verify_Config() == false) // check if config successful and if config should be saved
              return;

            int WLayOut = 0;
            if (LayOut.Text == "List")
                WLayOut = 0;
            if (LayOut.Text == "Small Icons")
                WLayOut = 1;
            if (LayOut.Text == "Large Icons")
                WLayOut = 2;
            if (LayOut.Text == "Filmstrip")
                WLayOut = 3;
#if MP11
            if (LayOut.Text == "Cover Flow")
            {
              WLayOut = 3;
              LayOut.Text = "Filmstrip";
            }
#else
          if (LayOut.Text == "Cover Flow")
                WLayOut = 4;
#endif


            if (AntTitle2.Text.Length == 0)
                AntTitle2.Text = "(none)";
            string wDfltSortMethod = string.Empty;
            string wDfltSort = string.Empty;
            switch (Sort.Text.ToLower())
            {
                //if (AntSTitle.Text.Length > 0 && AntSTitle.Text != "(none)")
                //    Sort.Items.Add(AntSTitle.Text);
                //else
                //    Sort.Items.Add(AntTitle1);
                
                case "(none)":
                    break;
                case "title":
                    wDfltSortMethod = GUILocalizeStrings.Get(103);
                    if (AntSTitle.Text != "(none)" && AntSTitle.Text.Length > 0)
                        wDfltSort = AntSTitle.Text;
                    else
                        wDfltSort = AntTitle1.Text;
                    break;
                case "year":
                    wDfltSortMethod = GUILocalizeStrings.Get(366);
                    wDfltSort = "YEAR";
                    break;
                case "date":
                    wDfltSortMethod = GUILocalizeStrings.Get(621);
                    wDfltSort = "DateAdded";
                    break;
                case "rating":
                    wDfltSortMethod = GUILocalizeStrings.Get(367);
                    wDfltSort = "RATING";
                    break;
                default:
                    wDfltSort = Sort.Text; //Guzzi: Added to not reset mapped settings other than dropdown names
                    if (Sort.Text.ToLower() == AntSort1.Text.ToLower())
                    {
                        wDfltSortMethod = AntTSort1.Text;
                        wDfltSort = AntSort1.Text;
                    }
                    if (Sort.Text.ToLower() == AntSort2.Text.ToLower())
                    {
                        wDfltSortMethod = AntTSort2.Text;
                        wDfltSort = AntSort2.Text;
                    }
                    break;
                                       
            }
            // backup the XML Config before writing
            if (System.IO.File.Exists(XmlConfig.EntireFilenameConfig("MyFilms")))
                System.IO.File.Copy(XmlConfig.EntireFilenameConfig("MyFilms"), XmlConfig.EntireFilenameConfig("MyFilms") + ".bak", true);

            if (Config_Dflt.Checked)
            {
                XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Default_Config", Config_Name.Text.ToString());
            }
            else
            {
                if (XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", "") == Config_Name.Text)
                    XmlConfig.RemoveEntry("MyFilms", "MyFilms", "Default_Config");
            }
            if (Config_Menu.Checked)
            {
                XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Default_Config", "");
            }

            XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Menu_Config", Config_Menu.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Logos", chkLogos.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "CatalogType", CatalogType.SelectedIndex.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntCatalogExecutable", AMCexePath.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntCatalog", MesFilmsCat.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntPicture", MesFilmsImg.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ArtistPicturePath", MesFilmsImgArtist.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ArtistDflt", chkDfltArtist.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntStorage", AntStorage.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntStorageTrailer", AntStorageTrailer.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "PathStorage", PathStorage.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "PathStorageTrailer", PathStorageTrailer.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntIdentItem", AntIdentItem.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntIdentLabel", AntIdentLabel.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntTitle1", AntTitle1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntTitle2", AntTitle2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSTitle", AntSTitle.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSort1", AntSort1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntTSort1", AntTSort1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSort2", AntSort2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntTSort2", AntTSort2.Text.ToString());
            //XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterMinRating", AntFilterMinRating.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterItem1", AntFilterItem1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterSign1", AntFilterSign1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterText1", AntFilterText1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterItem2", AntFilterItem2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterSign2", AntFilterSign2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterText2", AntFilterText2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterItem3", AntFilterItem3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterSign3", AntFilterSign3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterText3", AntFilterText3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterItem4", AntFilterItem4.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterSign4", AntFilterSign4.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterText4", AntFilterText4.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterComb", AntFilterComb.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewItem1", AntViewItem1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewText1", AntViewText1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewValue1", AntViewValue1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewItem2", AntViewItem2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewText2", AntViewText2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewValue2", AntViewValue2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewItem3", AntViewItem3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewText3", AntViewText3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewValue3", AntViewValue3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewItem4", AntViewItem4.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewText4", AntViewText4.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewValue4", AntViewValue4.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewItem5", AntViewItem5.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewText5", AntViewText5.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewValue5", AntViewValue5.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSearchItem1", AntSearchItem1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSearchText1", AntSearchText1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSearchItem2", AntSearchItem2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSearchText2", AntSearchText2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntUpdItem1", AntUpdItem1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntUpdText1", AntUpdText1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntUpdDflT1", AntUpdDflT1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntUpdItem2", AntUpdItem2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntUpdText2", AntUpdText2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntUpdDflT2", AntUpdDflT2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem1", AntItem1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel1", AntLabel1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem2", AntItem2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel2", AntLabel2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem3", AntItem3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel3", AntLabel3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem4", AntItem4.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel4", AntLabel4.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem5", AntItem5.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel5", AntLabel5.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewDfltItem", View_Dflt_Item.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewDfltText", View_Dflt_Text.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchList", AntSearchList.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "UpdateList", AntUpdList.Text.ToString());

            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "WOL-Enable", check_WOL_enable.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "WOLtimeout", comboWOLtimeout.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "WOL-Userdialog", check_WOL_Userdialog.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-Name-1", NAS_Name_1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-MAC-1", NAS_MAC_1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-Name-2", NAS_Name_2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-MAC-2", NAS_MAC_2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-Name-3", NAS_Name_3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-MAC-3", NAS_MAC_3.Text.ToString());

            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "StrSelect", "");
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator1", ListSeparator1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator2", ListSeparator2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator3", ListSeparator3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator4", ListSeparator4.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator5", ListSeparator5.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator1", RoleSeparator1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator2", RoleSeparator2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator3", RoleSeparator3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator4", RoleSeparator4.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator5", RoleSeparator5.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Selection", "");
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntDfltStrSort", wDfltSort);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntDfltStrSortSens", SortSens.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntDfltSortMethod", wDfltSortMethod);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "StrSort", "");
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "StrSortSens", "");
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "CurrentSortMethod", "");
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "StrSortSens", "");
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "IndexItem", "");
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "TitleDelim", TitleDelim.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "LayOut", WLayOut);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "StrDfltSelect", StrDfltSelect);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Dwp", crypto.Crypter(Dwp.Text));
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "UseOriginaltitleForMissingTranslatedtitle", chkUseOriginalAsTranslatedTitle.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchFileName", SearchFileName.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchSubDirs", SearchSubDirs.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchOnlyExactMatches", SearchOnlyExactMatches.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchSubDirsTrailer", SearchSubDirsTrailer.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "CheckWatched", CheckWatched.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "CheckWatchedPlayerStopped", CheckWatchedPlayerStopped.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AlwaysDefaultView", AlwaysDefaultView.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "UseListviewForGroups", chkUseListviewForGroups.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "GlobalUnwatchedOnly", chkGlobalUnwatchedOnly.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "GlobalUnwatchedOnlyValue", textBoxGlobalUnwatchedOnlyValue.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "CheckMediaOnStart", chkScanMediaOnStart.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AllowTraktSync", cbAllowTraktSync.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "OnlyTitleList", chkOnlyTitle.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "WindowsFileDialog", chkWindowsFileDialog.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ItemSearchFileName", ItemSearchFileName.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ItemSearchGrabberName", ItemSearchGrabberName.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "GrabberOverrideLanguage", cbGrabberOverrideLanguage.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "GrabberOverridePersonLimit", cbGrabberOverridePersonLimit.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "GrabberOverrideTitleLimit", cbGrabberOverrideTitleLimit.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "GrabberOverrideGetRoles", cbGrabberOverrideGetRoles.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "PictureHandling", cbPictureHandling.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultCover", DefaultCover.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultCoverArtist", DefaultCoverArtist.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultCoverViews", DefaultCoverViews.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultFanartImage", DefaultFanartImage.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "CmdExe", CmdExe.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "CmdPar", CmdPar.Text);
            //XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber", chkGrabber.Checked);
            //XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_Dir", txtDirGrab.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_cnf", txtGrabber.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "PicturePrefix", txtPicturePrefix.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_Always", chkGrabber_Always.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_ChooseScript", chkGrabber_ChooseScript.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd", chkAMCUpd.Checked);
            //XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd_exe", txtAMCUpd_exe.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd_cnf", txtAMCUpd_cnf.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Fanart", chkFanart.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartDefaultViews", chkFanartDefaultViews.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartDflt", chkDfltFanart.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartDfltImage", chkDfltFanartImage.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartDfltImageAll", chkDfltFanartImageAll.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartPicture", MesFilmsFanart.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewsPicture", MesFilmsViews.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Views", chkViews.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Persons", chkPersons.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewsDflt", chkDfltViews.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewsDfltAll", chkDfltViewsAll.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "LastID", "7986");
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Suppress", chkSuppress.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressPlayed", chksupplaystop.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "WatchedField", cbWatched.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressField", cbfdupdate.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressValue", txtfdupdate.Text.ToString());
            if (rbsuppress1.Checked)
                XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressType", "1");
            if (rbsuppress2.Checked)
                XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressType", "2");
            if (rbsuppress3.Checked)
                XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressType", "3");
            if (rbsuppress4.Checked)
                XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressType", "4");

            if (CatalogType.SelectedIndex != 0) // common external catalog options
            {
              XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddTagline", chkAddTagline.Checked);
              XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddTags", chkAddTags.Checked);
              XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddCertification", chkAddCertification.Checked);
              XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddWriter", chkAddWriter.Checked);
              XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddDestinationTagline", ECMergeDestinationFieldTagline.Text);
              XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddDestinationTags", ECMergeDestinationFieldTags.Text);
              XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddDestinationCertification", ECMergeDestinationFieldCertification.Text);
              XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddDestinationWriter", ECMergeDestinationFieldWriter.Text);
            }
            if (CatalogType.SelectedIndex == 1)
            {
              XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "OnlyFile", chkDVDprofilerOnlyFile.Checked);
              if (this.chkDVDprofilerMergeWithGenreField.Checked)
                XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "DVDPTagField", "Category");
              else
                XmlConfig.RemoveEntry("MyFilms", Config_Name.Text.ToString(), "DVDPTagField");
            }
          
            string w_Config_Name = Config_Name.Text;
            Config_Name.Items.Remove(Config_Name.Text);
            Config_Name.Items.Add(w_Config_Name); 
            Config_Name.Text = w_Config_Name;
            XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "NbConfig", Config_Name.Items.Count);
            if (chkLogos.Checked) // Save Logoconfig ...
            {
              Save_XML_Logos();
            }
            //if (chkAMCUpd.Checked)
            //{
            //  Save_XML_AMCconfig(currentconfig);
            //}
            textBoxNBconfigs.Text = Config_Name.Items.Count.ToString();
            System.Windows.Forms.MessageBox.Show("Configuration saved !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Save_XML_Logos()
        {
            string wfile = XmlConfig.EntireFilenameConfig("MyFilmsLogos").Substring(0, XmlConfig.EntireFilenameConfig("MyFilmsLogos").LastIndexOf("."));
            //if (System.IO.File.Exists(wfile + "_" + Config_Name.Text + ".xml"))
            //  System.IO.File.Copy(wfile + "_" + Config_Name.Text + ".xml", wfile + "_" + Config_Name.Text + ".xml.sav", true);
            if (System.IO.File.Exists(wfile + ".xml"))
              System.IO.File.Copy(wfile + ".xml", wfile + ".xml.sav", true);
            //try
            //{
            //    System.IO.File.Copy(XmlConfig.EntireFilenameConfig("MyFilmsLogos"), wfile + "_" + Config_Name.Text + ".xml", true);
            //    //wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1) + "_" + Config_Name.Text; // commented, as we always want to write to the original file !
            //}
            //catch
            //{
            //    wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
            //}
            wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
            //using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.MPSettings())
            //{
            //  logoPath = MediaPortal.Configuration.Config.GetDirectoryInfo(Config.Dir.Skin) + @"\" + xmlreader.GetValueAsString("skin", "name", "NoSkin") + @"\Media\Logos"; // Get current path to logos in skindirectory
            //}
            //if (txtLogosPath.Text.Length > 0)
            XmlConfig.WriteXmlConfig(wfile, "ID0000", "LogoPresets", comboBoxLogoPresets.Text); // Added Presets to Logo Config
            XmlConfig.WriteXmlConfig(wfile, "ID0000", "LogosPath", txtLogosPath.Text);
            XmlConfig.WriteXmlConfig(wfile, "ID0000", "Spacing", comboBoxLogoSpacing.Text); // Added Spacing to Logo Config
            int iID2001 = 0;
            int iID2002 = 0;
            int iID2003 = 0;
            for (int i = 0; i < (int)LogoView.Items.Count; i++)
            {
              string wline = LogoView.Items[i].SubItems[1].Text + ";";
              wline = wline + LogoView.Items[i].SubItems[2].Text.ToLower() + ";";
              wline = wline + LogoView.Items[i].SubItems[3].Text + ";";
              wline = wline + LogoView.Items[i].SubItems[4].Text.ToLower() + ";";
              wline = wline + LogoView.Items[i].SubItems[5].Text + ";";
              wline = wline + LogoView.Items[i].SubItems[6].Text.ToLower() + ";";
              wline = wline + LogoView.Items[i].SubItems[7].Text + ";";
              if ((LogoView.Items[i].SubItems[9].Text.Length > 0) && (StoreFullLogoPath))
                wline = wline + LogoView.Items[i].SubItems[9].Text + "\\" + LogoView.Items[i].SubItems[8].Text;
              else
                wline = wline + LogoView.Items[i].SubItems[8].Text;
              switch (LogoView.Items[i].SubItems[0].Text)
              {
                case "ID2001":
                  XmlConfig.WriteXmlConfig(wfile, LogoView.Items[i].SubItems[0].Text, LogoView.Items[i].SubItems[0].Text + "_" + iID2001, wline);
                  iID2001++;
                  break;
                case "ID2002":
                  XmlConfig.WriteXmlConfig(wfile, LogoView.Items[i].SubItems[0].Text, LogoView.Items[i].SubItems[0].Text + "_" + iID2002, wline);
                  iID2002++;
                  break;
                case "ID2003":
                  XmlConfig.WriteXmlConfig(wfile, LogoView.Items[i].SubItems[0].Text, LogoView.Items[i].SubItems[0].Text + "_" + iID2003, wline);
                  iID2003++;
                  break;
              }
            }
        }
    
        private void butPath_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(PathStorage.Text))
              {
                if (PathStorage.Text.Contains(";"))
                  folderBrowserDialog1.SelectedPath = PathStorage.Text.Substring(PathStorage.Text.LastIndexOf(";") + 1).Trim();
                else
                  folderBrowserDialog1.SelectedPath = PathStorage.Text;
                if (folderBrowserDialog1.SelectedPath.LastIndexOf("\\") == folderBrowserDialog1.SelectedPath.Length)
                  folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath.Substring(folderBrowserDialog1.SelectedPath.Length - 1);
              }
            else
              folderBrowserDialog1.SelectedPath = String.Empty;
            folderBrowserDialog1.Description = "Path for Movies File Search";
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (this.folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") != this.folderBrowserDialog1.SelectedPath.Length - 1)
                    folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";

                if (PathStorage.Text.Length == 0)
                    PathStorage.Text = folderBrowserDialog1.SelectedPath;
                else
                    PathStorage.Text = PathStorage.Text + ";" + folderBrowserDialog1.SelectedPath;
                if (AMCMovieScanPath.Text.Length == 0) 
                  AMCMovieScanPath.Text = PathStorage.Text;
            }
        }

        private void AntFilterItem1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (AntFilterItem1.Text == "(none)")
                AntFilterText1.Clear();
        }

        private void AntFilterItem2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AntFilterItem2.Text == "(none)")
                AntFilterText2.Clear();
        }

        private void AntFilterItem3_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (AntFilterItem3.Text == "(none)")
                AntFilterText3.Clear();
        }

        private void AntFilterItem4_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (AntFilterItem4.Text == "(none)")
                AntFilterText4.Clear();
        }

        public void ButQuit_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void Config_Name_SelectedIndexChanged(object sender, EventArgs e)
        {
          Config_Name_Load();
        }

    private void Config_Name_Load()
        {
            Refresh_Tabs(true); // enable Tabs
            Refresh_Items(false);
            CatalogType.SelectedIndex = Convert.ToInt16(XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "CatalogType", "0"));
            MesFilmsCat.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntCatalog", "");
            AMCexePath.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntCatalogExecutable", "");
            MesFilmsImg.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntPicture", "");
            MesFilmsImgArtist.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ArtistPicturePath", "");
            MesFilmsFanart.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartPicture", "");
            lblResultingGroupViewsPathFanart.Text = MesFilmsFanart.Text + "\\_Group\\";
            MesFilmsViews.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewsPicture", "");
            chkDfltViews.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewsDflt", false);
            chkDfltViewsAll.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewsDfltAll", false);
            chkDfltArtist.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ArtistDflt", false);
            chkViews.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Views", false);
            chkPersons.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Persons", false);
            AntStorage.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntStorage", "");
            AntStorageTrailer.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntStorageTrailer", "");
            PathStorage.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "PathStorage", "");
            PathStorageTrailer.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "PathStorageTrailer", "");
            AntIdentItem.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntIdentItem", "");
            AntIdentLabel.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntIdentLabel", "");
            AntTitle1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntTitle1", "");
            AntTitle2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntTitle2", "");
            AntSTitle.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSTitle", "");
            if (AntSTitle.Text == "")
                AntSTitle.Text = AntTitle1.Text;
            AntSort1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSort1", "");
            AntTSort1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntTSort1", "");
            AntSort2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSort2", "");
            AntTSort2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntTSort2", "");
            Sort.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntDfltStrSort", "");
            SortSens.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntDfltStrSortSens", "");
            //AntFilterMinRating.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterMinRating", "5");
            AntFilterItem1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterItem1", "");
            AntFilterSign1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterSign1", "#");
            AntFilterText1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterText1", "");
            AntFilterItem2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterItem2", "");
            AntFilterSign2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterSign2", "#");
            AntFilterText2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterText2", "");
            AntFilterItem3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterItem3", "");
            AntFilterSign3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterSign3", "#");
            AntFilterText3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterText3", "");
            AntFilterItem4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterItem4", "");
            AntFilterSign4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterSign4", "#");
            AntFilterText4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterText4", "");
            AntFilterComb.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterComb", "and");
            AntViewItem1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewItem1", "");
            AntViewText1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewText1", "");
            AntViewValue1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewValue1", "");
            AntViewItem2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewItem2", "");
            AntViewText2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewText2", "");
            AntViewValue2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewValue2", "");
            AntViewItem3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewItem3", "");
            AntViewText3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewText3", "");
            AntViewValue3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewValue3", "");
            AntViewItem4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewItem4", "");
            AntViewText4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewText4", "");
            AntViewValue4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewValue4", "");
            AntViewItem5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewItem5", "");
            AntViewText5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewText5", "");
            AntViewValue5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntViewValue5", "");
            AntSearchItem1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSearchItem1", "");
            AntSearchText1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSearchText1", "");
            AntSearchItem2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSearchItem2", "");
            AntSearchText2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntSearchText2", "");
            AntUpdItem1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntUpdItem1", "");
            AntUpdText1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntUpdText1", "");
            AntUpdDflT1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntUpdDflT1", "");
            AntUpdItem2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntUpdItem2", "");
            AntUpdText2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntUpdText2", "");
            AntUpdDflT2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "AntUpdDflT2", string.Empty);
            AntSearchList.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SearchList", "");
            AntUpdList.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "UpdateList", "");

            if (AntTitle1.Text.Length > 0)
            {
              if (AntTitle1.Text == "TranslatedTitle")
              {
                if (AntSearchList.Text.Length == 0)
                  AntSearchList.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SearchList", "TranslatedTitle, OriginalTitle, Description, Comments, Actors, Director, Producer, Year, Date, Category, Country, Rating, Checked, MediaLabel, MediaType, URL, Borrower, Length, VideoFormat, VideoBitrate, AudioFormat, AudioBitrate, Resolution, Framerate, Size, Disks, Languages, Subtitles, Number");
                if (AntUpdList.Text.Length == 0)
                  AntUpdList.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "UpdateList", "TranslatedTitle, OriginalTitle, Category, Year, Date, Country, Rating, Checked, MediaLabel, MediaType, Actors, Director, Producer");
              }
              if (AntTitle1.Text == "OriginalTitle" || AntTitle1.Text == "FormattedTitle")
              {
                if (AntSearchList.Text.Length == 0)
                  AntSearchList.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SearchList", "OriginalTitle, TranslatedTitle, Description, Comments, Actors, Director, Producer, Year, Date, Category, Country, Rating, Checked, MediaLabel, MediaType, URL, Borrower, Length, VideoFormat, VideoBitrate, AudioFormat, AudioBitrate, Resolution, Framerate, Size, Disks, Languages, Subtitles, Number");
                if (AntUpdList.Text.Length == 0)
                  AntUpdList.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "UpdateList", "OriginalTitle, TranslatedTitle, Category, Year, Date, Country, Rating, Checked, MediaLabel, MediaType, Actors, Director, Producer");
              }
            }
            check_WOL_enable.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "WOL-Enable", false);
            comboWOLtimeout.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "WOLtimeout", "15");
            check_WOL_Userdialog.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "WOL-Userdialog", false);
            NAS_Name_1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-Name-1", string.Empty);
            NAS_MAC_1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-MAC-1", string.Empty);
            NAS_Name_2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-Name-2", string.Empty);
            NAS_MAC_2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-MAC-2", string.Empty);
            NAS_Name_3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-Name-3", string.Empty);
            NAS_MAC_3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "NAS-MAC-3", string.Empty);

            AntItem1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem1", string.Empty);
            AntLabel1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel1", string.Empty);
            AntItem2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem2", string.Empty);
            AntLabel2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel2", string.Empty);
            AntItem3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem3", string.Empty);
            AntLabel3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel3", string.Empty);
            AntItem4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem4", string.Empty);
            AntLabel4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel4", string.Empty);
            AntItem5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem5", string.Empty);
            AntLabel5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel5", string.Empty);
            ListSeparator1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator1", ",");
            ListSeparator2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator2", ";");
            ListSeparator3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator3", "[");
            ListSeparator4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator4", "|");
            ListSeparator5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator5", string.Empty);
            RoleSeparator1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator1", "(");
            RoleSeparator2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator2", ")");
            RoleSeparator3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator3", " as ");
            RoleSeparator4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator4", "....");
            RoleSeparator5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator5", "|");
            CmdPar.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "CmdPar", "(none)");
            CmdExe.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "CmdExe", string.Empty);
            TitleDelim.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "TitleDelim", "\\");
            //chkGrabber.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber", false);
            chkDfltFanart.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartDflt", false);
            chkDfltFanartImage.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartDfltImage", false);
            chkDfltFanartImageAll.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartDfltImageAll", false);
            chkFanart.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Fanart", false);
            chkFanartDefaultViews.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartDefaultViews", false);
            txtGrabber.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_cnf", string.Empty);
            txtPicturePrefix.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "PicturePrefix", string.Empty);
            //txtDirGrab.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_Dir", Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\scripts\myfilms");
            chkGrabber_Always.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_Always", false);
            chkAMCUpd.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd", false);
            //txtAMCUpd_exe.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd_exe", Config.GetDirectoryInfo(Config.Dir.Base).ToString() + @"\AMCupdater.exe"); 
            chkGrabber_ChooseScript.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_ChooseScript", false);
            txtAMCUpd_cnf.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd_cnf", string.Empty);
            chkSuppress.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Suppress", false);
            chksupplaystop.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressPlayed", false);
            cbWatched.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "WatchedField", "Checked");
            cbfdupdate.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressField", string.Empty);
            txtfdupdate.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressValue", string.Empty);
            chkLogos.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Logos", true);  // Changed default to "true" to use default logo config file // had to reset to false, as logos are heavily relying on an existing DB config ! // Rechanged to true, as now config name should be required anymore for logo config!
            string wsuppressType = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressType", "1");
            switch (wsuppressType)
            {
                case "1":
                    rbsuppress1.Checked = true;
                    break;
                case "2":
                    rbsuppress2.Checked = true;
                    break;
                case "3":
                    rbsuppress3.Checked = true;
                    break;
                default:
                    rbsuppress4.Checked = true;
                    break;
            }
            Dwp.Text = crypto.Decrypter(XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Dwp", string.Empty));
            Rpt_Dwp.Text = Dwp.Text;
            //fmu pour etre coherent avec MP qui stocke les booleens en yes/no au lieu de true/false
            //fmu       if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchFileName", "False") == "True")
            if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "UseOriginaltitleForMissingTranslatedtitle", "False") == "True" //fmu
            || XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "UseOriginaltitleForMissingTranslatedtitle", "False") == "yes")  //fmu
              chkUseOriginalAsTranslatedTitle.Checked = true;
            else
              chkUseOriginalAsTranslatedTitle.Checked = false;
            if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchFileName", "False") == "True" //fmu
            || XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchFileName", "False") == "yes")  //fmu
                SearchFileName.Checked = true;
            else
                SearchFileName.Checked = false;
            if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchSubDirs", "False") == "True" //fmu
            || XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchSubDirs", "False") == "yes")  //fmu
              SearchSubDirs.Checked = true;
            else
              SearchSubDirs.Checked = false;
            if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchOnlyExactMatches", "False") == "True"
            || XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchOnlyExactMatches", "False") == "yes")
              SearchOnlyExactMatches.Checked = true;
            else
              SearchOnlyExactMatches.Checked = false;
            if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchSubDirsTrailer", "False") == "True" //fmu
            || XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchSubDirsTrailer", "False") == "yes")  //fmu
                SearchSubDirsTrailer.Checked = true;
            else
                SearchSubDirsTrailer.Checked = false;
            if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "WOL-Enable", "False") == "True"
            || XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "WOL-Enable", "False") == "yes")
                check_WOL_enable.Checked = true;
            else
                check_WOL_enable.Checked = false;

            comboWOLtimeout.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "WOLtimeout", "15");

            if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "WOL-Userdialog", "False") == "True"
            || XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "WOL-Userdialog", "False") == "yes")
                check_WOL_Userdialog.Checked = true;
            else
                check_WOL_Userdialog.Checked = false;

            chkDVDprofilerMergeWithGenreField.Checked = false;
            if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "DVDPTagField", "") == "Category")
              chkDVDprofilerMergeWithGenreField.Checked = true;
            else
              chkDVDprofilerMergeWithGenreField.Checked = false;
            CheckWatched.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "CheckWatched", false);
            CheckWatchedPlayerStopped.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "CheckWatchedPlayerStopped", false);
            AlwaysDefaultView.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AlwaysDefaultView", false);
            chkUseListviewForGroups.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "UseListviewForGroups", true);
            chkGlobalUnwatchedOnly.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "GlobalUnwatchedOnly", false);
            textBoxGlobalUnwatchedOnlyValue.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "GlobalUnwatchedOnlyValue", "false");
            chkScanMediaOnStart.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "CheckMediaOnStart", false);
            cbAllowTraktSync.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AllowTraktSync", false);
            chkOnlyTitle.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "OnlyTitleList", false);
            chkWindowsFileDialog.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "WindowsFileDialog", false);
            // common external catalog options
            chkAddTagline.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddTagline", false);
            chkAddTags.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddTags", false);
            chkAddCertification.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddCertification", false);
            chkAddWriter.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddWriter", false);
            ECMergeDestinationFieldTagline.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddDestinationTagline", "");
            ECMergeDestinationFieldTags.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddDestinationTags", "");
            ECMergeDestinationFieldCertification.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddDestinationCertification", "");
            ECMergeDestinationFieldWriter.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ECoptionAddDestinationWriter", "");
            chkDVDprofilerOnlyFile.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "OnlyFile", false);
            ItemSearchFileName.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ItemSearchFileName", "");
            ItemSearchGrabberName.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ItemSearchGrabberName", "");
            cbGrabberOverrideLanguage.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "GrabberOverrideLanguage", string.Empty);
            cbGrabberOverridePersonLimit.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "GrabberOverridePersonLimit", string.Empty);
            cbGrabberOverrideTitleLimit.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "GrabberOverrideTitleLimit", string.Empty);
            cbGrabberOverrideGetRoles.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "GrabberOverrideGetRoles", string.Empty);

            cbPictureHandling.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "PictureHandling", "");
            DefaultCover.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultCover", "");
            DefaultCoverArtist.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultCoverArtist", "");
            DefaultCoverViews.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultCoverViews", "");
            DefaultFanartImage.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultFanartImage", "");
            View_Dflt_Item.Items.Remove(View_Dflt_Item.Text);
            View_Dflt_Item.Items.Add(View_Dflt_Item.Text);
            View_Dflt_Item.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewDfltItem", "(none)");
            View_Dflt_Text.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewDfltText", "");
            if ((Config_Name.Text) == XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", ""))
                Config_Dflt.Checked = true;
            else
                Config_Dflt.Checked = false;

            //if (!(AntViewItem1.Text == "Country") & !(AntViewItem1.Text == "Category") & !(AntViewItem1.Text == "Year") & !(AntViewItem1.Text == "(none)"))
            //    View_Dflt_Item.Items.Add(AntViewItem1.Text);
            //if (!(AntViewItem2.Text == "Country") & !(AntViewItem2.Text == "Category") & !(AntViewItem2.Text == "Year") & !(AntViewItem2.Text == "(none)"))
            //    View_Dflt_Item.Items.Add(AntViewItem2.Text);
            int WLayOut = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "LayOut", 0);
            if (WLayOut == 0)
                LayOut.Text = "List";
            if (WLayOut == 1)
                LayOut.Text = "Small Icons";
            if (WLayOut == 2)
                LayOut.Text = "Large Icons";
            if (WLayOut == 3)
                LayOut.Text = "Filmstrip";
#if MP11
            if (WLayOut == 4)
            {
              WLayOut = 3;
              LayOut.Text = "Filmstrip";
            }
#else
            if (WLayOut == 4)
                LayOut.Text = "Cover Flow";
#endif

            AntViewText_Change();
            AntSort_Change();
            
            if (Config_Name.Text.Length > 0) 
              chkLogos.Enabled = true;
            else 
              chkLogos.Enabled = false;

            if (chkAMCUpd.Checked)
            {
              groupBox_AMCupdater_ExternalApplication.Enabled = true;
              groupBox_AMCupdaterScheduer.Enabled = true;
            }
            else
            {
              groupBox_AMCupdater_ExternalApplication.Enabled = false;
              groupBox_AMCupdaterScheduer.Enabled = false;
            }
            //if (PathStorage.Text.Length > 0 && AMCMovieScanPath.Text.Length == 0) // disabled, because users don't want to always have that path copied to AMC scan path
            //  AMCMovieScanPath.Text = PathStorage.Text;
            //else
            //  AMCMovieScanPath.Text = String.Empty;
            //if (AMCMovieScanPath.Text.Length == 0)
            //  btnCreateAMCDefaultConfig.Visible = true;
            //else
            //  btnCreateAMCDefaultConfig.Visible = false;

            // Added by Guzzi to load or initialize the AMCupdater Default configuration and create default configfiles, if necessary.
            Read_XML_AMCconfig(Config_Name.Text); // read current (or create new default) config file

            textBoxNBconfigs.Text = Config_Name.Items.Count.ToString();
        }

        private void Refresh_Tabs(bool enable)
        {
          if (!enable)
          {
            Tab_Trailer.Enabled = false;
            Tab_Logos.Enabled = false;
            Tab_Views.Enabled = false;
            Tab_Search.Enabled = false;
            Tab_Update.Enabled = false;
            Tab_AMCupdater.Enabled = false;
            Tab_Artwork.Enabled = false;
            Tab_ExternalCatalogs.Enabled = false;
            Tab_WakeOnLan.Enabled = false;
            Tab_Trakt.Enabled = false;
            MesFilmsCat.Enabled = false;
            CatalogType.Enabled = false;
            ButCat.Enabled = false;
            groupBox_TitleOrder.Enabled = false;
            groupBox_Security.Enabled = false;
            groupBox_PlayMovieInfos.Enabled = false;
            groupBox_PreLaunchingCommand.Enabled = false;

          }
          else
          {
            Tab_Trailer.Enabled = true;
            Tab_Logos.Enabled = true;
            Tab_Views.Enabled = true;
            Tab_Search.Enabled = true;
            Tab_Update.Enabled = true;
            Tab_AMCupdater.Enabled = true;
            Tab_Artwork.Enabled = true;
            Tab_ExternalCatalogs.Enabled = true;
            Tab_WakeOnLan.Enabled = true;
            Tab_Trakt.Enabled = true;
            MesFilmsCat.Enabled = true;
            CatalogType.Enabled = true;
            ButCat.Enabled = true;
            groupBox_TitleOrder.Enabled = true;
            groupBox_Security.Enabled = true;
            groupBox_PlayMovieInfos.Enabled = true;
            groupBox_PreLaunchingCommand.Enabled = true;
          }
        }

        private void Refresh_Items(bool all)
        {
            CatalogType.SelectedIndex = 0;
            StrDfltSelect = "";
            //mydivx.Clear();
            View_Dflt_Item.Items.Clear();
            View_Dflt_Item.Items.Add("(none)");
            View_Dflt_Item.Items.Add(GUILocalizeStrings.Get(342)); // Films
            View_Dflt_Item.Items.Add("Year");
            View_Dflt_Item.Items.Add("Category");
            View_Dflt_Item.Items.Add("Country");
            if (Config_Name.Text.Length == 0)
            {
              //btnFirstTimeSetup.Enabled = true;
              Tab_Trailer.Enabled = false;
              Tab_Logos.Enabled = false;
              Tab_Views.Enabled = false;
              Tab_Search.Enabled = false;
              Tab_Update.Enabled = false;
              Tab_AMCupdater.Enabled = false;
              Tab_Artwork.Enabled = false;
              Tab_ExternalCatalogs.Enabled = false;
              Tab_WakeOnLan.Enabled = false;
              Tab_Trakt.Enabled = false;
              MesFilmsCat.Enabled = false;
              CatalogType.Enabled = false;
              ButCat.Enabled = false;
              groupBox_TitleOrder.Enabled = false;
              groupBox_Security.Enabled = false;
              groupBox_PlayMovieInfos.Enabled = false;
              groupBox_PreLaunchingCommand.Enabled = false;

            }
            else
            {
              //btnFirstTimeSetup.Enabled = false;
              Tab_Trailer.Enabled = true;
              Tab_Logos.Enabled = true;
              Tab_Views.Enabled = true;
              Tab_Search.Enabled = true;
              Tab_Update.Enabled = true;
              Tab_AMCupdater.Enabled = true;
              Tab_Artwork.Enabled = true;
              Tab_ExternalCatalogs.Enabled = true;
              Tab_WakeOnLan.Enabled = true;
              Tab_Trakt.Enabled = true;
              MesFilmsCat.Enabled = true;
              CatalogType.Enabled = true;
              ButCat.Enabled = true;
              groupBox_TitleOrder.Enabled = true;
              groupBox_Security.Enabled = true;
              groupBox_PlayMovieInfos.Enabled = true;
              groupBox_PreLaunchingCommand.Enabled = true;
            }
            if (!all)
                return;
            Config_Dflt.Checked = false;
            MesFilmsCat.ResetText();
            MesFilmsImg.ResetText();
            MesFilmsImgArtist.ResetText();
            MesFilmsFanart.ResetText();
            MesFilmsViews.ResetText();
            AntStorage.ResetText();
            AntStorageTrailer.ResetText();
            PathStorage.ResetText();
            PathStorageTrailer.ResetText();
            AntIdentItem.ResetText();
            AntIdentLabel.ResetText();
            AntTitle1.ResetText();
            AntTitle2.ResetText();
            AntSTitle.ResetText();
            AntSort1.ResetText();
            AntTSort1.ResetText();
            AntSort2.ResetText();
            AntTSort2.ResetText();
            Sort.ResetText();
            //AntFilterMinRating.ResetText();
            AntFilterItem1.ResetText();
            AntFilterItem2.ResetText();
            AntFilterItem3.ResetText();
            AntFilterItem4.ResetText();
            AntFilterText1.ResetText();
            AntFilterText2.ResetText();
            AntFilterText3.ResetText();
            AntFilterText4.ResetText();
            AntFilterSign1.ResetText();
            AntFilterSign2.ResetText();
            AntFilterSign3.ResetText();
            AntFilterSign4.ResetText();
            AntFilterComb.ResetText();
            AntViewItem1.ResetText();
            AntViewItem2.ResetText();
            AntViewItem3.ResetText();
            AntViewItem4.ResetText();
            AntViewItem5.ResetText();
            AntViewText1.ResetText();
            AntViewText2.ResetText();
            AntViewText3.ResetText();
            AntViewText4.ResetText();
            AntViewText5.ResetText();
            AntViewValue1.ResetText();
            AntViewValue2.ResetText();
            AntViewValue3.ResetText();
            AntViewValue4.ResetText();
            AntViewValue5.ResetText();
            AntSearchItem1.ResetText();
            AntSearchText1.ResetText();
            AntSearchItem2.ResetText();
            AntSearchText2.ResetText();
            AntSearchList.ResetText();
            AntUpdList.ResetText();
            AntUpdItem1.ResetText();
            AntUpdItem2.ResetText();
            AntUpdText1.ResetText();
            AntUpdText2.ResetText();
            AntUpdDflT1.ResetText();
            AntUpdDflT2.ResetText();
            AntLabel1.ResetText();
            AntLabel2.ResetText();
            AntLabel3.ResetText();
            AntLabel4.ResetText();
            AntLabel5.ResetText();
            AntItem1.ResetText();
            AntItem2.ResetText();
            AntItem3.ResetText();
            AntItem4.ResetText();
            AntItem5.ResetText();
            TitleDelim.ResetText();
            LayOut.ResetText();
            ListSeparator1.ResetText();
            ListSeparator2.ResetText();
            ListSeparator3.ResetText();
            ListSeparator4.ResetText();
            ListSeparator5.ResetText();
            RoleSeparator1.ResetText();
            RoleSeparator2.ResetText();
            RoleSeparator3.ResetText();
            RoleSeparator4.ResetText();
            RoleSeparator5.ResetText();
            View_Dflt_Item.ResetText();
            View_Dflt_Text.ResetText();
            DefaultCover.ResetText();
            DefaultCoverArtist.ResetText();
            DefaultCoverViews.ResetText();
            DefaultFanartImage.ResetText();
            Dwp.ResetText();
            Rpt_Dwp.ResetText();
            CmdExe.ResetText();
            CmdPar.ResetText();
            ItemSearchFileName.ResetText();
            ItemSearchGrabberName.ResetText();
            cbPictureHandling.ResetText();
            chkUseOriginalAsTranslatedTitle.Checked = false;
            SearchFileName.Checked = false;
            chkSuppress.Checked = false;
            chksupplaystop.Checked = false;
            cbfdupdate.ResetText();
            cbWatched.ResetText();
            txtfdupdate.ResetText();
            rbsuppress1.Checked = true;
            SearchSubDirs.Checked = false;
            SearchOnlyExactMatches.Checked = false;
            SearchSubDirsTrailer.Checked = false;
            check_WOL_enable.Checked = false;
            comboWOLtimeout.ResetText();
            check_WOL_Userdialog.Checked = false;
            CheckWatched.Checked = false;
            CheckWatchedPlayerStopped.Checked = false;
            NAS_Name_1.ResetText();
            NAS_Name_2.ResetText();
            NAS_Name_3.ResetText();
            NAS_MAC_1.ResetText();
            NAS_MAC_2.ResetText();
            NAS_MAC_3.ResetText();
            chkAddTagline.Checked = false;
            chkAddTags.Checked = false;
            chkAddCertification.Checked = false;
            chkAddWriter.Checked = false;
            ECMergeDestinationFieldTagline.Text = string.Empty;
            ECMergeDestinationFieldTags.Text = string.Empty;
            ECMergeDestinationFieldCertification.Text = string.Empty;
            ECMergeDestinationFieldWriter.Text = string.Empty;
            cbGrabberOverrideLanguage.Text = string.Empty;
            cbGrabberOverrideTitleLimit.Text = string.Empty;
            cbGrabberOverridePersonLimit.Text = string.Empty;
            cbGrabberOverrideGetRoles.Text = string.Empty;
            chkDVDprofilerMergeWithGenreField.Checked = false;
            chkDVDprofilerOnlyFile.Checked = false;
            AlwaysDefaultView.Checked = false;
            chkUseListviewForGroups.Checked = true;
            chkGlobalUnwatchedOnly.Checked = false;
            chkScanMediaOnStart.Checked = false;
            cbAllowTraktSync.Checked = false;
            textBoxGlobalUnwatchedOnlyValue.Text = "false";
            chkOnlyTitle.Checked = false;
            chkWindowsFileDialog.Checked = false;
            //chkGrabber.Checked = false;
            txtGrabber.ResetText();
            txtPicturePrefix.ResetText();
            chkGrabber_Always.Checked = false;
            chkGrabber_ChooseScript.Checked = false;
            chkAMCUpd.Checked = false;
            txtAMCUpd_cnf.ResetText();
            //txtAMCUpd_exe.ResetText();
            chkDfltFanart.Checked = false;
            chkDfltFanartImage.Checked = false;
            chkDfltFanartImageAll.Checked = false;
            chkFanart.Checked = false;
            chkFanartDefaultViews.Checked = false;
            chkDfltViews.Checked = false;
            chkDfltViewsAll.Checked = false;
            chkDfltArtist.Checked = false;
            chkViews.Checked = false;
            chkPersons.Checked = false;
            chkAMC_Purge_Missing_Files.Checked = false;
            AMCMovieScanPath.ResetText();
            //btnLaunchAMCupdater.Enabled = false;
            //btnCreateAMCDesktopIcon.Enabled = false;
            //btnCreateAMCDefaultConfig.Enabled = false;
            //comboBoxLogoSpacing.ResetText();
        }

        private void ButDelet_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this configuration? \n\nYour corresponding AMCupdater configuration and \nyour desktop icon will also be deleted!", "Warning", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
              string DeleteName = Config_Name.Text;
              Remove_Config();
              if ((Config_Name.Text) == XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", "")) XmlConfig.RemoveEntry("MyFilms", "MyFilms", "Default_Config");
              if ((Config_Name.Text) == XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Current_Config", "")) XmlConfig.RemoveEntry("MyFilms", "MyFilms", "Current_Config");
              Config_Name.Items.Remove(Config_Name.Text);
              Refresh_Items(true);
              Config_Name.ResetText();
              textBoxNBconfigs.Text = Config_Name.Items.Count.ToString();
              // Remove desktop Icon and AMCupdater config
              string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
              string linkName = "AMC-Updater '" + DeleteName + "'";
              string shortcutFile = deskDir + @"\AMC-Updater (" + DeleteName + ")" + ".lnk";
              if (System.IO.File.Exists(deskDir + "\\" + linkName + ".url"))
                try {System.IO.File.Delete(deskDir + "\\" + linkName + ".url");} catch{}
              if (System.IO.File.Exists(shortcutFile))
                try {System.IO.File.Delete(shortcutFile);} catch{}
              string AMCupdaterConfigFile = Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings" + "_" + DeleteName + ".xml";
              if (System.IO.File.Exists(AMCupdaterConfigFile))
                try { System.IO.File.Delete(AMCupdaterConfigFile); } catch{}
            }
        }

        private void ButCopy_Click(object sender, EventArgs e)
        {
            MyFilmsInputBox input = new MyFilmsInputBox();
            input.CatalogTypeSelectedIndex = CatalogType.SelectedIndex ; // preset to currently chosen catalog type 
            input.CatalogType = CatalogType.Text; // preset to currently chosen catalog name
            input.ShowDialog(this);
            string newCatalogType = input.CatalogType;
            int newCatalogSelectedIndex = input.CatalogTypeSelectedIndex;
            string newConfig_Name = input.ConfigName;
            if (newConfig_Name == Config_Name.Text)
            {
                System.Windows.Forms.MessageBox.Show("New Config Name must be different from the existing one !", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                Config_Name.Text = newConfig_Name;
                System.Windows.Forms.MessageBox.Show("Created a copy of current Configuration !", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                CatalogType.SelectedIndex = newCatalogSelectedIndex; // set to selected catalog type 
                Save_Config();
                // Create matching AMCupdater config
                Read_XML_AMCconfig(Config_Name.Text); // read current (or create new default) config file
                CreateMyFilmsDefaultsForAMCconfig(Config_Name.Text); //create MF defaults
                Save_XML_AMCconfig(Config_Name.Text); // save new config
                Read_XML_AMCconfig(Config_Name.Text); // reread config file with new defaults

                Config_Name.Focus();
                textBoxNBconfigs.Text = Config_Name.Items.Count.ToString();
            }
        }

        private void Remove_Config()
        {
            XmlConfig.RemoveEntry("MyFilms", Config_Name.Text, string.Empty);
        }

        private void MesFilmsSetup_Quit(object sender, FormClosedEventArgs e)
        {
            if (Config_Name.Items.Count == 0)
            {
                DialogResult dialogResult = MessageBox.Show("No Configuration defined; the plugin'll not work !", "Information", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Cancel)
                    return;
                else
                {
                    XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "NbConfig", Config_Name.Items.Count);
                    if (textBoxPluginName.Text == "") 
                      textBoxPluginName.Text = "MyFilms"; // Make sure, a plugin name is given - assign default, if user didn't choose any!
                    XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "PluginName", textBoxPluginName.Text.ToString());
                    XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Default_Config", "");
                    LogMyFilms.Debug("(Setup) - Quit - created default empty config!");
                    mydivx.Dispose();
                    Close();
                }
            }
            MesFilms_nb_config = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "NbConfig", -1);
            for (int i = 0; i < (int)MesFilms_nb_config; i++)
            {
                XmlConfig.RemoveEntry("MyFilms", "MyFilms", "ConfigName" + i);
            }
            for (int i = 0; i < (int)Config_Name.Items.Count; i++)
            {
                XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "ConfigName" + i, Config_Name.Items[i].ToString());
                if (Config_Name.Items.Count == 1)
                {
                    XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Default_Config", Config_Name.Items[i].ToString());
                    XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Menu_Config", Config_Name.Items[i].ToString());
                }
            }
            XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "NbConfig", Config_Name.Items.Count);
            XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "PluginName", textBoxPluginName.Text.ToString());
            LogMyFilms.Debug("(Setup) - Quit - saved base config!");
            Close();
        }

        private void ButQuit_Click(object sender, FormClosingEventArgs e)
        {
            MesFilmsSetup_Quit();
        }

        private static void MesFilmsSetup_Quit()
        {
        }
        private void Config_Name_Control(object sender, EventArgs e)
        {
            if (Config_Name.Text.Length > 0)
                return;
            System.Windows.Forms.MessageBox.Show("Give the Configuration's Name first !", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            Config_Name.Focus();
        }
        private bool Verify_Config()
        {
            if (MesFilmsCat.Text.Length > 0)
            {
                try
                {
                    mydivx.Clear();

                }
                catch { }
                mydivx = ReadXml();
                if (mydivx != null)
                {
                    DataRow[] movies = mydivx.Movie.Select(StrDfltSelect + AntTitle1.Text + " not like ''");
                    if (mydivx.Movie.Count > 0)
                        if (movies.Length > 0)
                            System.Windows.Forms.MessageBox.Show("Your XML file is valid with " + mydivx.Movie.Count + " Movies in your database and " + movies.Length + " Movies to display with your 'User defined Config Filter' configuration", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                          System.Windows.Forms.MessageBox.Show("Your XML file is valid with 0 Movie in your database but no Movie to display, you have to change the 'User defined Config Filter' or fill your database with AMCUpdater, AMC or your compatible Software", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    else if (!WizardActive)
                    {
                        if (System.Windows.Forms.MessageBox.Show("There is no Movie to display with that file ! Do you Want to continue ?", "Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.No)
                        {
                            MesFilmsCat.Focus();
                            return false;
                        }
                        System.Windows.Forms.MessageBox.Show("You have to fill your database with AMCUpdater, AMC or your compatible Software", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
              System.Windows.Forms.MessageBox.Show("Please give the XML file's name first");
              MesFilmsCat.Focus();
              return false;
            }
            return true; // save config in calling routine
        }

        private void Selected_Enreg_TextChanged()
        {
            StrDfltSelect = "";
            string wAntFilterSign;
            if (AntFilterSign1.Text == "#")
                wAntFilterSign = "<>";
            else
                wAntFilterSign = AntFilterSign1.Text;
            if ((AntFilterItem1.Text.Length > 0) && !(AntFilterItem1.Text == "(none)"))
                if (AntFilterItem1.Text == "DateAdded")
                    if ((AntFilterSign1.Text == "#") || (AntFilterSign1.Text == "not like"))
                        StrDfltSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText1.Text) + "# or " + AntFilterItem1.Text + " is null) ";
                    else
                        StrDfltSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText1.Text) + "# ) ";
                else
                    if ((AntFilterSign1.Text == "#") || (AntFilterSign1.Text == "not like"))
                        StrDfltSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " '" + AntFilterText1.Text + "' or " + AntFilterItem1.Text + " is null) ";
                    else
                        StrDfltSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " '" + AntFilterText1.Text + "') ";
            if ((AntFilterComb.Text == "or") && (StrDfltSelect.Length > 0))
                StrDfltSelect = StrDfltSelect + " OR ";
            else
                if (StrDfltSelect.Length > 0)
                    StrDfltSelect = StrDfltSelect + " AND ";
            if (AntFilterSign2.Text == "#")
                wAntFilterSign = "<>";
            else
                wAntFilterSign = AntFilterSign2.Text;
            if ((AntFilterItem2.Text.Length > 0) && !(AntFilterItem2.Text == "(none)"))
                if (AntFilterItem2.Text == "DateAdded")
                    if ((AntFilterSign2.Text == "#") || (AntFilterSign2.Text == "not like"))
                        StrDfltSelect = "(" + StrDfltSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText2.Text) + "# or " + AntFilterItem2.Text + " is null)) AND ";
                    else
                        StrDfltSelect = "(" + StrDfltSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText2.Text) + "# )) AND ";
                else
                    if ((AntFilterSign2.Text == "#") || (AntFilterSign2.Text == "not like"))
                        StrDfltSelect = "(" + StrDfltSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " '" + AntFilterText2.Text + "' or " + AntFilterItem2.Text + " is null)) AND ";
                    else
                        StrDfltSelect = "(" + StrDfltSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " '" + AntFilterText2.Text + "' )) AND ";
            Selected_Enreg.Text = StrDfltSelect + AntTitle1.Text + " not like ''";
            LogMyFilms.Debug("MyFilms (Build Selected Enreg) - Selected_Enreg: '" + Selected_Enreg.Text.ToString() + "'");
        }

        private void Selected_Enreg_Changed(object sender, EventArgs e)
        {
            Selected_Enreg_TextChanged();
        }


        private void CatalogType_SelectedIndexChanged(object sender, EventArgs e)
        {
          if (CatalogType.SelectedIndex != 0) // all presets for "Non-ANT-MC-Catalogs/External Catalogs"
          {
            if (!NewConfigButton)
            {
              chkFanart.Checked = true;
              chkDfltFanart.Checked = false;
              chkDfltFanartImage.Checked = true;
              chkDfltFanartImageAll.Checked = true;
              chkPersons.Checked = true; // enable person thumbs
              Tab_AMCupdater.Enabled = false;
              Tab_Update.Enabled = false;
              if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "en" || AntTitle1.Text == "OriginalTitle")
              {
                //if (string.IsNullOrEmpty(AntTitle1.Text))
                AntTitle1.Text = "OriginalTitle";
                AntTitle2.Text = "TranslatedTitle";
                ItemSearchFileName.Text = "OriginalTitle";
                AntSTitle.Text = "OriginalTitle";
              }
              else
              {
                AntTitle1.Text = "TranslatedTitle";
                AntTitle2.Text = "OriginalTitle";
                ItemSearchFileName.Text = "TranslatedTitle";
                AntSTitle.Text = "TranslatedTitle";
              }
              //if (string.IsNullOrEmpty(AntSTitle.Text)) AntSTitle.Text = "FormattedTitle";
              TitleDelim.Text = "\\";
              SearchFileName.Checked = true;

              //Presets for useritems:
              AntItem1.Text = "Country";
              AntLabel1.Text = BaseMesFilms.Translate_Column(AntItem1.Text.Trim());
              AntItem2.Text = "Certification";
              AntLabel2.Text = BaseMesFilms.Translate_Column(AntItem2.Text.Trim());
              AntItem3.Text = "Category";
              AntLabel3.Text = BaseMesFilms.Translate_Column(AntItem3.Text.Trim());
              AntItem4.Text = "Writer";
              AntLabel4.Text = BaseMesFilms.Translate_Column(AntItem4.Text.Trim());
              AntItem5.Text = "Producer";
              AntLabel5.Text = BaseMesFilms.Translate_Column(AntItem5.Text.Trim());
            }
            Tab_ExternalCatalogs.Enabled = true;
            txtPicturePrefix.Text = "";
            if (!AntViewItem1.Items.Contains("Writer")) // only add if not already done!
              AddExtendedFieldsForExternalCatalogs();

            groupBoxExtendedFieldHandling.Enabled = true;
          }
          groupBox_DVDprofiler.Enabled = false; // deaktivates DVDprofiler options as default...

            switch (CatalogType.SelectedIndex)
            {
              case 0:
                    groupBoxExtendedFieldHandling.Enabled = false; 
                    Tab_AMCupdater.Enabled = true;
                    Tab_Update.Enabled = true;
                    break;
              case 1: // DVDprofiler
                    groupBox_DVDprofiler.Enabled = true;
                    AntStorage.Text = "Source";
                    AntStorageTrailer.Text = ""; // Disable Trailers ...
                    if (MesFilmsCat.Text.Length > 0)
                    {
                      MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Images"; // cover path
                      //MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Persons"; // person thumb path
                      //MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Fanart"; // fanart path
                    }
                    cbWatched.Text = "Checked";
                    if (!string.IsNullOrEmpty(AntSearchList.Text)) AntSearchList.Text.Replace(", Borrower", ""); // remove Borrower, as it's not supported...
                    break;
              case 3: // MyMovies
                    AntStorage.Text = "Source";
                    AntStorageTrailer.Text = "SourceTrailer";
                    if (MesFilmsCat.Text.Length > 0)
                    {
                      MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // cover path
                      MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // person thumb path
                      //MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // fanart path
                    }
                    if (!string.IsNullOrEmpty(AntSearchList.Text)) AntSearchList.Text.Replace(", Borrower", ""); // remove Borrower, as it's not supported...
                    chkAddTagline.Checked = true;
                    ECMergeDestinationFieldTagline.Text = "Description";
                    break;
              case 4: // EAX MC 2.5.0
                    AntStorage.Text = "Source";
                    AntStorageTrailer.Text = "SourceTrailer";
                    if (MesFilmsCat.Text.Length > 0)
                    {
                      MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Pictures"; // cover path
                      MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\NamePictures"; // person thumb path
                      // Did Work! MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Thumbnails"; // fanart path
                    }
                    if (!string.IsNullOrEmpty(AntSearchList.Text)) AntSearchList.Text.Replace(", Borrower", ""); // remove Borrower, as it's not supported...
                    break;
              case 5: // XMM Extreme Movie Manager
                    AntStorage.Text = "Source";
                    AntStorageTrailer.Text = "SourceTrailer";
                    if (MesFilmsCat.Text.Length > 0)
                    { // C:\WinApps\Video\eXtreme Movie Manager 7\Databases\<DB-name>_cover etc.
                      //string strDatadirectory = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\"));
                      //string[] directories = System.IO.Directory.GetDirectories(strDatadirectory);
                      //string lastDirectory = string.Empty;
                      //foreach (string directory in directories)
                      //{
                      //  if (string.Compare(directory, lastDirectory) >= 1)
                      //  {
                      //    lastDirectory = directory;
                      //  }
                      //}
                      //string strDbName = MesFilmsCat.Text.Substring(MesFilmsCat.Text.LastIndexOf("\\") + 1);
                      //strDbName = strDbName.Substring(0, strDbName.LastIndexOf(".")); // results in filename only without extension
                      //MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\" + strDbName + "_cover"; // cover path
                      //MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\" + strDbName + "_photos"; // person thumb path
                      //MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\" + strDbName + "_thumbs"; // fanart path - better use _cover, as otherwise only small thumbs !
                      //a.	…eXtreme Movie Manager 7\Databases\Test_cover – for movie covers
                      //b.	… eXtreme Movie Manager 7\Databases\Test_photos – for person thumbs – filename format is Steve-Martin_143135.jpg
                      //c.	… eXtreme Movie Manager 7\Databases\Test_thumbs – for covers and thumbnails – this is where fanart might be stored as #-[movietitle]_fanart.jpg
                      MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // cover path
                      MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // person thumb path
                      //MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")); // fanart path
                      chkAddTagline.Checked = true;
                      ECMergeDestinationFieldTagline.Text = "Description";
                    }
                    break;

                case 7: // MyFilms DB (currently same as ANT Movie Catalog, but possible to extend DB fields in the future
                    break;

                case 9: // EAX MC 3.0.9
                    AntStorage.Text = "Source";
                    AntStorageTrailer.Text = "SourceTrailer";
                    if (MesFilmsCat.Text.Length > 0)
                    {
                      //if (MesFilmsImg.Text.Length == 0)
                      MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Pictures"; // cover path
                      MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\NamePictures"; // person thumb path
                      // Did Work! MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Thumbnails"; // fanart path
                      //a.	Cover image folder = D:My Documents\Data\Eax Movie Catalog \Pictures
                      //b.	Person Thumbs = D:My Documents\Data\Eax Movie Catalog\NamePictures
                      //c.	Fanart = D:My Documents\Eax Movie Catalog\Thumbnails – depends on the script i.e. grabs photos, but TMDB scrip grabs fanart but if users have fanart in EAX this folder is where it will be stored.
                    }
                    cbWatched.Text = "Checked";
                    if (!string.IsNullOrEmpty(AntSearchList.Text)) AntSearchList.Text.Replace(", Borrower", ""); // remove Borrower, as it's not supported...
                    break;

                case 10: // PVD Personal Video Database V0.9.9.21
                    AntStorage.Text = "Source";
                    AntStorageTrailer.Text = "Borrower";
                    if (MesFilmsCat.Text.Length > 0)
                    {
                      MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\"));
                      MesFilmsImgArtist.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Photos"; // person thumb path
                      // Did Work! MesFilmsFanart.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Screenshots"; // fanart path
                      // o	Posters - D:\My Documents\Personal Video Database\TEST\Posters – movie covers used in PVD DB - but setting to DB path required !
                      //o	D:\My Documents\Personal Video Database\TEST\Photos – actor/person images
                      //o	D:\My Documents\Personal Video Database\TEST\Screenshots – could be video thumbnails, fanart, manually saved thumbs
                      //o	D:\My Documents\Personal Video Database\TEST\images – PVD copies covers to this folder when you export to xml and these are the filenames exported for cover images, i.e.:
                      //	<poster>images/image1_66_-1x-1.jpg</poster>
                    }
                    cbWatched.Text = "Checked";
                    chkAddTagline.Checked = true;
                    ECMergeDestinationFieldTagline.Text = "Description";
                    break;

                case 11: // MovingPicturesXML V1.2
                    AntStorage.Text = "Source";
                    AntStorageTrailer.Text = "SourceTrailer";
                    if (MesFilmsCat.Text.Length > 0)
                    {
                      MesFilmsImg.Text = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MovingPictures\Covers\FullSize"; // Covers path
                      MesFilmsImgArtist.Text = ""; // person thumb path
                      // Did Work! MesFilmsFanart.Text = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MovingPictures\Backdrops\FullSize"; // fanart path
                    }
                    cbWatched.Text = "Checked";
                    break;

                default:
                    if (string.IsNullOrEmpty(AntStorage.Text))
                      AntStorage.Text = "Source";
                    if (string.IsNullOrEmpty(AntStorageTrailer.Text))
                      AntStorageTrailer.Text = "Borrower";
                    break;

            }
        }


        private void AddExtendedFieldsForExternalCatalogs()
        {
          AntStorageTrailer.Items.Add("SourceTrailer");

          //Fanart,
          //RatingUser,
          //Studio,
          //IMDB_Rank,
          //IsOnline,
          //Aspectratio,
          AntViewItem1.Items.Add("Writer");
          AntViewItem2.Items.Add("Writer");
          AntViewItem3.Items.Add("Writer");
          AntViewItem4.Items.Add("Writer");
          AntViewItem5.Items.Add("Writer");
          AntViewItem1.Items.Add("Certification");
          AntViewItem2.Items.Add("Certification");
          AntViewItem3.Items.Add("Certification");
          AntViewItem4.Items.Add("Certification");
          AntViewItem5.Items.Add("Certification");
          AntViewItem1.Items.Add("TagLine");
          AntViewItem2.Items.Add("TagLine");
          AntViewItem3.Items.Add("TagLine");
          AntViewItem4.Items.Add("TagLine");
          AntViewItem5.Items.Add("TagLine");
          AntViewItem1.Items.Add("Tags");
          AntViewItem2.Items.Add("Tags");
          AntViewItem3.Items.Add("Tags");
          AntViewItem4.Items.Add("Tags");
          AntViewItem5.Items.Add("Tags");

          AntFilterItem1.Items.Add("Writer");
          AntFilterItem2.Items.Add("Writer");
          AntFilterItem1.Items.Add("Certification");
          AntFilterItem2.Items.Add("Certification");
          AntFilterItem1.Items.Add("Tags");
          AntFilterItem2.Items.Add("Tags");
          AntItem1.Items.Add("Writer");
          AntItem2.Items.Add("Writer");
          AntItem3.Items.Add("Writer");
          AntItem4.Items.Add("Writer");
          AntItem5.Items.Add("Writer");
          AntItem1.Items.Add("Certification");
          AntItem2.Items.Add("Certification");
          AntItem3.Items.Add("Certification");
          AntItem4.Items.Add("Certification");
          AntItem5.Items.Add("Certification");
          AntItem1.Items.Add("TagLine");
          AntItem2.Items.Add("TagLine");
          AntItem3.Items.Add("TagLine");
          AntItem4.Items.Add("TagLine");
          AntItem5.Items.Add("TagLine");
          AntItem1.Items.Add("Tags");
          AntItem2.Items.Add("Tags");
          AntItem3.Items.Add("Tags");
          AntItem4.Items.Add("Tags");
          AntItem5.Items.Add("Tags");

          AntSort1.Items.Add("Writer");
          AntSort2.Items.Add("Writer");
          AntSort1.Items.Add("Certification");
          AntSort2.Items.Add("Certification");

          AntSearchItem1.Items.Add("Writer");
          AntSearchItem2.Items.Add("Writer");
          AntSearchItem1.Items.Add("Certification");
          AntSearchItem2.Items.Add("Certification");
          AntSearchItem1.Items.Add("TagLine");
          AntSearchItem2.Items.Add("TagLine");
          AntSearchItem1.Items.Add("Tags");
          AntSearchItem2.Items.Add("Tags");

          AntSearchField.Items.Add("Writer");
          AntSearchField.Items.Add("Certification");

          cbWatched.Items.Add("Watched");
          //cbfdupdate.Items.Add(dc.ColumnName);
          //AntUpdItem1.Items.Add(dc.ColumnName);
          //AntUpdItem2.Items.Add(dc.ColumnName);
          //AntUpdField.Items.Add(dc.ColumnName);
          //AntIdentItem.Items.Add(dc.ColumnName);
        }

        private void btnGrabber_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtGrabber.Text))
              openFileDialog1.FileName = txtGrabber.Text;
            else
            {
              openFileDialog1.FileName = String.Empty;
              openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms";
            }

            if (txtGrabber.Text.Contains("\\"))
              openFileDialog1.InitialDirectory = txtGrabber.Text.Substring(0, txtGrabber.Text.LastIndexOf("\\") + 1);

            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "xml";
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Select Default Internet Grabber Script (xml file)";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
              txtGrabber.Text = openFileDialog1.FileName;
              if (!string.IsNullOrEmpty(txtGrabber.Text))
                txtGrabberDisplay.Text = Path.GetFileName(txtGrabber.Text);
              else
                txtGrabberDisplay.Text = string.Empty;
            }
        }

        //private void btnAMCUpd_exe_Click(object sender, EventArgs e)
        //{
        //    openFileDialog1.RestoreDirectory = true;
        //    openFileDialog1.DefaultExt = "AMCUpdater.exe";
        //    openFileDialog1.Filter = "exe Files|AMCUpdater.exe";
        //    openFileDialog1.Title = "Find AMCUpdater program";
        //    if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
        //        txtAMCUpd_exe.Text = openFileDialog1.FileName;
        //}

        private void btnAMCUpd_cnf_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAMCUpd_cnf.Text))
              openFileDialog1.FileName = txtAMCUpd_cnf.Text;
            else
            {
              openFileDialog1.FileName = String.Empty;
              openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config).ToString();
            }

            if (txtAMCUpd_cnf.Text.Contains("\\"))
              openFileDialog1.InitialDirectory = txtAMCUpd_cnf.Text.Substring(0, txtAMCUpd_cnf.Text.LastIndexOf("\\") + 1);

            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "xml";
            openFileDialog1.Filter = "XML AMCUpdater Config Files|*.xml";
            openFileDialog1.Title = "Select AMCUPdater Config file (xml file)";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                txtAMCUpd_cnf.Text = openFileDialog1.FileName;
        }

        private void chkAMCUpd_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAMCUpd.Checked)
            {
                groupBox_AMCupdater_ExternalApplication.Enabled = true;
                groupBox_AMCupdaterScheduer.Enabled = true;
                //txtAMCUpd_exe.Enabled = true;
                //btnAMCUpd_exe.Enabled = true;
                //txtAMCUpd_cnf.Enabled = true;
                //btnAMCUpd_cnf.Enabled = true;
                //scheduleAMCUpdater.Enabled = true;
                //btnParameters.Enabled = true;
                //btnLaunchAMCupdater.Enabled = true;
                //btnAMCMovieScanPathAdd.Enabled = true;
                //chkAMC_Purge_Missing_Files.Enabled = true;
                //btnCreateAMCDefaultConfig.Enabled = true;
                //btnCreateAMCDesktopIcon.Enabled = true;
                //AMCMovieScanPath.Enabled = true;
            }
            else
            {
                groupBox_AMCupdater_ExternalApplication.Enabled = false;
                groupBox_AMCupdaterScheduer.Enabled = false;
                //txtAMCUpd_exe.Enabled = false;
                //btnAMCUpd_exe.Enabled = false;
                //txtAMCUpd_cnf.Enabled = false;
                //btnAMCUpd_cnf.Enabled = false;
                //scheduleAMCUpdater.Enabled = false;
                //btnParameters.Enabled = false;
                //btnLaunchAMCupdater.Enabled = false;
                //btnAMCMovieScanPathAdd.Enabled = false;
                //chkAMC_Purge_Missing_Files.Enabled = false;
                //btnCreateAMCDefaultConfig.Enabled = false;
                //btnCreateAMCDesktopIcon.Enabled = false;
                //AMCMovieScanPath.Enabled = false;
            }
        }

        private void btnFanart_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(MesFilmsFanart.Text))
              folderBrowserDialog1.SelectedPath = MesFilmsFanart.Text;
            else
              folderBrowserDialog1.SelectedPath = String.Empty;

            folderBrowserDialog1.Description = "Select Fanart Backdrop Path";
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                MesFilmsFanart.Text = folderBrowserDialog1.SelectedPath;
                lblResultingGroupViewsPathFanart.Text = MesFilmsFanart.Text + "\\_Group\\";
            }
        }

        private void chkFanart_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFanart.Checked)
            {
                MesFilmsFanart.Enabled = true;
                DefaultFanartImage.Enabled = true;
                btnFanart.Enabled = true;
                ButDefFanart.Enabled = true;
                chkDfltFanart.Enabled = true;
                chkDfltFanartImage.Enabled = true;
                chkDfltFanartImageAll.Enabled = true;
                chkFanartDefaultViews.Enabled = true;
            }
            else
            {
                MesFilmsFanart.Enabled = false;
                DefaultFanartImage.Enabled = false;
                btnFanart.Enabled = false;
                ButDefFanart.Enabled = false;
                chkDfltFanart.Enabled = false;
                chkDfltFanartImage.Enabled = false;
                chkDfltFanartImageAll.Enabled = false;
                chkFanartDefaultViews.Enabled = false;
            }
        }


        private void pictureBox_Click(object sender, EventArgs e)
        {

          if (!string.IsNullOrEmpty(SFilePicture.Text))
            openFileDialog1.FileName = SFilePicture.Text;
          else
          {
            openFileDialog1.FileName = String.Empty;
            //openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\DefaultImages";
          }
          if (SFilePicture.Text.Contains("\\"))
            openFileDialog1.InitialDirectory = SFilePicture.Text.Substring(0, SFilePicture.Text.LastIndexOf("\\") + 1);
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "png";
            openFileDialog1.Filter = "PNG Files|*.png";
            openFileDialog1.Title = "Find Logos files (png file)";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (MediaPortal.Util.Utils.IsPicture(openFileDialog1.FileName))
                {
                    SPicture.BackgroundImage = ImageFast.FastFromFile(openFileDialog1.FileName);
                    SPicture.BackgroundImageLayout = ImageLayout.Stretch;
                    SFilePicture.Text = openFileDialog1.FileName;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("File choosen isn't a Picture !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    SPicture.Focus();
                    return;
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (SLogo_Type.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("Logo Type must be defined !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                SLogo_Type.Focus();
                return;
            }
            if ((SField1.Text.Length == 0) || (SOp1.Text.Length == 0) || (SValue1.Text.Length == 0))
            {
                if ((SField1.Text.Length == 0 || SOp1.Text.Length == 0) && (SOp1.Text != "filled" && SOp1.Text != "not filled"))
                {
                    System.Windows.Forms.MessageBox.Show("The three Fields for comparison must be defined !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    SField1.Focus();
                    return;
                }
            }
            if ((SAnd_Or.Text.Length > 0) && ((SField2.Text.Length == 0) || (SOp2.Text.Length == 0) || (SValue2.Text.Length == 0)))
            {
                if ((SField2.Text.Length == 0 || SOp2.Text.Length == 0) && (SOp2.Text != "filled" && SOp2.Text != "not filled"))
                {
                    System.Windows.Forms.MessageBox.Show("The three Fields for comparison must be defined !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    SField2.Focus();
                    return;
                }
            }
            if ((SAnd_Or.Text.Length == 0) && ((SField2.Text.Length > 0) || (SOp2.Text.Length > 0) || (SValue2.Text.Length > 0)))
            {
                System.Windows.Forms.MessageBox.Show("The Operator 'AND' or 'OR' must be defined for Two conditions !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                SAnd_Or.Focus();
                return;
            }
            if (SPicture.BackgroundImage == null)
            {
                System.Windows.Forms.MessageBox.Show("Logo must be defined !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                SPicture.Focus();
                return;
            }
            if (selected_Logo_Item > -1)
            {
                //Edit_Item(selected_Logo_Item);
                selected_Logo_Item = -1;
            }
            //else
            //{
                LogoView.Items.Add(SLogo_Type.Text);
                LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SField1.Text);
                LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SOp1.Text.ToLower());
                LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SValue1.Text.ToLower());
                LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SAnd_Or.Text.ToLower());
                LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SField2.Text);
                LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SField2.Text.ToLower());
                LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(SValue2.Text.ToLower());
                LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetFileName(SFilePicture.Text));
                LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(SFilePicture.Text));
            //}
            SField1.Text = string.Empty;
            SOp1.Text = string.Empty;
            SValue1.Text = string.Empty;
            SAnd_Or.Text = string.Empty;
            SField2.Text = string.Empty;
            SOp2.Text = string.Empty;
            SValue2.Text = string.Empty;
            SFilePicture.Text = string.Empty;
            SPicture.BackgroundImage = null;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (LogoView.SelectedItems.Count != 0)
            {
                System.Windows.Forms.DialogResult rc = System.Windows.Forms.MessageBox.Show("Focused Item'll be remove, do you confirm ?", "Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rc == DialogResult.Yes)
                {
                    LogoView.SelectedItems[0].Remove();
                    selected_Logo_Item = -1;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please select an Item for re ordering rules !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
          if (selected_Logo_Item != -1)
          {
            //Select_Item(LogoView.SelectedItems[0].Index, false);
            //Move_Item(LogoView.SelectedItems[0].Index, LogoView.SelectedItems[0].Index); // this is a move to "same position"
            Edit_Item(selected_Logo_Item);
            Select_Item(selected_Logo_Item, true);
            Save_XML_Logos();
          }
          else
          {
            System.Windows.Forms.MessageBox.Show("Please first select an Item for updating !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return;
          }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (LogoView.SelectedItems.Count != 0)
            {
                if (LogoView.SelectedItems[0].Index == 0)
                    return;
                Select_Item(LogoView.SelectedItems[0].Index, false);
                Move_Item(LogoView.SelectedItems[0].Index - 1, LogoView.SelectedItems[0].Index);
                Edit_Item(LogoView.SelectedItems[0].Index - 1);
                LogoView.Items[LogoView.SelectedItems[0].Index - 1].Selected = true;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please select an Item for re ordering rules !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (LogoView.SelectedItems.Count != 0)
            {
                if (LogoView.SelectedItems[0].Index == LogoView.Items.Count - 1)
                    return;
                Select_Item(LogoView.SelectedItems[0].Index, false);
                Move_Item(LogoView.SelectedItems[0].Index + 1, LogoView.SelectedItems[0].Index);
                Edit_Item(LogoView.SelectedItems[0].Index + 1);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please select an Item for re ordering rules !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            LogoView.Items[LogoView.SelectedItems[0].Index + 1].Selected = true;
        }
        private void LogoView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (LogoView.Items.Count != 0)
            if (LogoView.SelectedItems.Count != 0)
                if ((LogoView.SelectedItems[0].Index > -1) && (LogoView.SelectedItems[0].Index < LogoView.Items.Count))
                    Select_Item(LogoView.SelectedItems[0].Index, true);
        }

        private void Select_Item(int select_item, bool select)
        {
            SLogo_Type.Text = LogoView.Items[select_item].SubItems[0].Text;
            SField1.Text = LogoView.Items[select_item].SubItems[1].Text;
            SOp1.Text = LogoView.Items[select_item].SubItems[2].Text;
            SValue1.Text = LogoView.Items[select_item].SubItems[3].Text;
            SAnd_Or.Text = LogoView.Items[select_item].SubItems[4].Text;
            SField2.Text = LogoView.Items[select_item].SubItems[5].Text;
            SOp2.Text = LogoView.Items[select_item].SubItems[6].Text;
            SValue2.Text = LogoView.Items[select_item].SubItems[7].Text;

            // Search logo file according settings, rule and active skin
            //textBoxActiveLogoPath.Text = "";
            //if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(LogoView.Items[select_item].SubItems[7].Text)))
            //  textBoxActiveLogoPath.Text = System.IO.Path.GetDirectoryName(LogoView.Items[select_item].SubItems[7].Text);
            //else if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(ActiveLogoPath + @"\" + LogoView.Items[select_item].SubItems[7].Text))) // Check if Logofile exists in default media directory of current skin and add it, if found
            //  textBoxActiveLogoPath.Text = System.IO.Path.GetDirectoryName(ActiveLogoPath + @"\" + LogoView.Items[select_item].SubItems[7].Text);
            //else if (!LogoView.Items[select_item].SubItems[7].Text.Contains("\\")) // Check, if logo file is present in subdirectories of logo directory of current skin - only if not already full path defined !
            //  {
            //    string[] filePathsLogoSearch = System.IO.Directory.GetFiles(ActiveLogoPath + @"\", LogoView.Items[select_item].SubItems[7].Text, System.IO.SearchOption.AllDirectories);
            //    if (filePathsLogoSearch.Length > 0)
            //    {
            //      textBoxActiveLogoPath.Text = System.IO.Path.GetDirectoryName(filePathsLogoSearch[0]);
            //    }
            //  }
            //else textBoxActiveLogoPath.Text = String.Empty;
            //if (textBoxActiveLogoPath.Text.Length > 0)
            //  LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(ActiveLogoPath);
            //  else
            //  LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(string.Empty); // Add empty field, if logofile istn't found anywhere


            string wfile = string.Empty;
            if (LogoView.Items[select_item].SubItems[9].Text.Length > 0)
                wfile = LogoView.Items[select_item].SubItems[9].Text + "\\" + LogoView.Items[select_item].SubItems[8].Text;
            else
                wfile = LogoView.Items[select_item].SubItems[8].Text;
            SFilePicture.Text = wfile;
            if (System.IO.File.Exists(wfile))
            {
              SPicture.BackgroundImage = ImageFast.FastFromFile(SFilePicture.Text);
            }
            else
            {
              if (SPicture.BackgroundImage != null)
              {
                SPicture.BackgroundImage.Dispose();
                SPicture.BackgroundImage = null;
              }
            }
          if (select)
            {
                selected_Logo_Item = select_item;
            }
        }
        private void Move_Item(int select_item, int dest_item)
        {
            LogoView.Items[dest_item].SubItems[0].Text = LogoView.Items[select_item].SubItems[0].Text;
            LogoView.Items[dest_item].SubItems[1].Text = LogoView.Items[select_item].SubItems[1].Text;
            LogoView.Items[dest_item].SubItems[2].Text = LogoView.Items[select_item].SubItems[2].Text;
            LogoView.Items[dest_item].SubItems[3].Text = LogoView.Items[select_item].SubItems[3].Text;
            LogoView.Items[dest_item].SubItems[4].Text = LogoView.Items[select_item].SubItems[4].Text;
            LogoView.Items[dest_item].SubItems[5].Text = LogoView.Items[select_item].SubItems[5].Text;
            LogoView.Items[dest_item].SubItems[6].Text = LogoView.Items[select_item].SubItems[6].Text;
            LogoView.Items[dest_item].SubItems[7].Text = LogoView.Items[select_item].SubItems[7].Text;
            LogoView.Items[dest_item].SubItems[8].Text = LogoView.Items[select_item].SubItems[8].Text;
            LogoView.Items[dest_item].SubItems[9].Text = LogoView.Items[select_item].SubItems[9].Text;
        }
        private void Edit_Item(int dest_item)
        {
            LogoView.Items[dest_item].SubItems[0].Text = SLogo_Type.Text;
            LogoView.Items[dest_item].SubItems[1].Text = SField1.Text;
            LogoView.Items[dest_item].SubItems[2].Text = SOp1.Text.ToLower();
            LogoView.Items[dest_item].SubItems[3].Text = SValue1.Text.ToLower();
            LogoView.Items[dest_item].SubItems[4].Text = SAnd_Or.Text.ToLower();
            LogoView.Items[dest_item].SubItems[5].Text = SField2.Text;
            LogoView.Items[dest_item].SubItems[6].Text = SOp2.Text.ToLower();
            LogoView.Items[dest_item].SubItems[7].Text = SValue2.Text.ToLower();

            LogoView.Items[dest_item].SubItems[8].Text = System.IO.Path.GetFileName(SFilePicture.Text);
            LogoView.Items[dest_item].SubItems[9].Text = System.IO.Path.GetDirectoryName(SFilePicture.Text);
            selected_Logo_Item = dest_item;
            LogoView.Items[dest_item].Checked = true;
            SField1.Text = string.Empty;
            SOp1.Text = string.Empty;
            SValue1.Text = string.Empty;
            SAnd_Or.Text = string.Empty;
            SField2.Text = string.Empty;
            SOp2.Text = string.Empty;
            SValue2.Text = string.Empty;
            SFilePicture.Text = string.Empty;
            SPicture.BackgroundImage = null;
            LogoView.Focus();
        }

        private void SField1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update_Svalue(SField1, SOp1, ref SValue1);
        }
        private void SField2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update_Svalue(SField2, SOp2, ref SValue2);
        }
        private void Update_Svalue(ComboBox SField, ComboBox SOp, ref ComboBox SValue)
        {
            string WSelect = string.Empty;
            switch (SOp.Text.ToLower())
            {
                case "equal":
                    WSelect = SField.Text + " = " + SValue.Text;
                    break;
                case "not equal":
                    WSelect = SField.Text + " # " + SValue.Text;
                    break;
                case "contains":
                    WSelect = SField.Text + " like '*" + SValue.Text + "*'";
                    break;
                case "not contains":
                    WSelect = SField.Text + " not like '*" + SValue.Text + "*'";
                    break;
                case "greater":
                    WSelect = SField.Text + " greater than '*" + SValue.Text + "*'";
                    break;
                case "lower":
                    WSelect = SField.Text + " lower than '*" + SValue.Text + "*'";
                    break;
                default:
                    break;
            }
            //if (WSelect.Length > 0)
            //    WSelect = " and " + WSelect;
            if (mydivx.Contents.Count == 0 && Config_Name.Text.Length > 0)
            {
                mydivx.Clear();
                if (!string.IsNullOrEmpty(MesFilmsCat.Text)) // only read, if catalog file is defined !
                  mydivx = ReadXml();
                LogoView.Items.Clear();
                selected_Logo_Item = -1;
                //Read_XML_Logos(Config_Name.Text);
                Read_XML_Logos();
            }
            DataRow[] movies = mydivx.Tables["Movie"].Select(SField.Text + " is not null", SField.Text.ToString() + " ASC");
            string wsfield = null;
            SValue.Items.Clear();
            foreach (DataRow enr in movies)
            {
                if (enr[SField.Text].ToString().ToLower() != wsfield)
                {
                    wsfield = enr[SField.Text].ToString().ToLower();
                    SValue.Items.Add(wsfield);
                }
            }
            //Guzzi: Added aspectratio (e.g. 2,39) and ar (e.g. 16:9)
            // Disables, because it's not yet working - requires changes in logos.cs of plugincode
            //SValue.Items.Add("aspectratio");
            //SValue.Items.Add("ar");
        }
        public AntMovieCatalog ReadXml()
        {
            XmlTextReader reader = new XmlTextReader(MesFilmsCat.Text);
            try
            {
                while (reader.Read())
                {
                }
                if (reader != null)
                {
                    reader.Close();
                    string destFile = "";
                    // ec options
                    string DestinationTagline = "";
                    string DestinationTags = "";
                    string DestinationCertification = "";
                    string DestinationWriter = "";

                    if (CatalogType.SelectedIndex != 0)
                    {
                      if (chkAddTagline.Checked)
                        DestinationTagline = ECMergeDestinationFieldTagline.Text;
                      else DestinationTagline = "";
                      if (chkAddTags.Checked)
                        DestinationTags = ECMergeDestinationFieldTags.Text;
                      else DestinationTags = "";
                      if (chkAddCertification.Checked)
                        DestinationCertification = ECMergeDestinationFieldCertification.Text;
                      else DestinationCertification = "";
                      if (chkAddWriter.Checked)
                        DestinationWriter = ECMergeDestinationFieldWriter.Text;
                      else DestinationWriter = "";
                    }

                    switch (CatalogType.SelectedIndex)
                    {
                        case 0: // ANT Movie Catalog
                        case 10: // Starter Settings ANT DB
                            mydivx.ReadXml(MesFilmsCat.Text);
                            break;
                        case 1: //DVD Profiler
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                                mydivx.ReadXml(destFile);
                                break;
                            }
                            DvdProfiler cc1 = new DvdProfiler("Category");
                            mydivx.ReadXml(cc1.ConvertProfiler(MesFilmsCat.Text, MesFilmsImg.Text, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, "Category", chkDVDprofilerOnlyFile.Checked));
                            break;
                        case 2: // Movie Collector V7.1.4
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                              mydivx.ReadXml(destFile);
                              break;
                            }
                            MovieCollector cc2 = new MovieCollector();
                            mydivx.ReadXml(cc2.ConvertMovieCollector(MesFilmsCat.Text, MesFilmsImg.Text, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text));
                            break;
                        case 3: // MyMovies
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                                mydivx.ReadXml(destFile);
                                break;
                            }
                            MyMovies mm = new MyMovies();
                            mydivx.ReadXml(mm.ConvertMyMovies(MesFilmsCat.Text, MesFilmsImg.Text, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, chkDVDprofilerOnlyFile.Checked));
                            break;
                        case 4: // EAX Movie Catalog 2.5.0
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                                mydivx.ReadXml(destFile);
                                break;
                            }
                            EaxMovieCatalog emc = new EaxMovieCatalog();
                            mydivx.ReadXml(emc.ConvertEaxMovieCatalog(MesFilmsCat.Text, MesFilmsImg.Text, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text));
                            break;
                        case 5: // EAX Movie Catalog 3.0.9 (beta5)
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                              mydivx.ReadXml(destFile);
                              break;
                            }
                            EaxMovieCatalog3 emc3 = new EaxMovieCatalog3();
                            mydivx.ReadXml(emc3.ConvertEaxMovieCatalog3(MesFilmsCat.Text, MesFilmsImg.Text, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text));
                            break;
                        case 6: // PVD PersonalVideoDatabase V0.9.9.21
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                              mydivx.ReadXml(destFile);
                              break;
                            }
                            PersonalVideoDatabase pvd = new PersonalVideoDatabase();
                            mydivx.ReadXml(pvd.ConvertPersonalVideoDatabase(MesFilmsCat.Text, MesFilmsImg.Text, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text, this.chkAddTagline.Checked));
                            break;
                        case 7: //eXtreme Movie Manager
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                                mydivx.ReadXml(destFile);
                                break;
                            }
                            XMM xmm = new XMM();
                            mydivx.ReadXml(xmm.ConvertXMM(MesFilmsCat.Text, MesFilmsImg.Text, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, chkDVDprofilerOnlyFile.Checked));
                            break;
                        case 8: // XBMC fulldb export (all movies in one DB)
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if (System.IO.File.Exists(destFile))
                            {
                              System.IO.FileInfo fInfo = new System.IO.FileInfo(destFile);
                              long size = fInfo.Length;
                              if (size == 0)
                                System.IO.File.Delete(destFile);
                            }
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                              mydivx.ReadXml(destFile);
                              break;
                            }
                            XbmcDb Xdb = new XbmcDb();
                            mydivx.ReadXml(Xdb.ConvertXbmcDb(MesFilmsCat.Text, MesFilmsImg.Text, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, AntStorage.Text, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text));
                            break;
                        case 9: // MovingPicturesXML
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                              mydivx.ReadXml(destFile);
                              break;
                            }
                            MovingPicturesXML mopi = new MovingPicturesXML();
                            mydivx.ReadXml(mopi.ConvertMovingPicturesXML(MesFilmsCat.Text, MesFilmsImg.Text, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, chkDVDprofilerOnlyFile.Checked));
                            break;
                        case 11: // XBMC Nfo (separate nfo files, to scan dirs - MovingPictures or XBMC)
                            destFile = MesFilmsCat.Text;
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                              mydivx.ReadXml(destFile);
                              break;
                            }
                            XbmcNfo nfo = new XbmcNfo();
                            mydivx.ReadXml(nfo.ConvertXbmcNfo(MesFilmsCat.Text, MesFilmsImg.Text, DestinationTagline, DestinationTags, DestinationCertification, DestinationWriter, AntStorage.Text, chkDVDprofilerOnlyFile.Checked, TitleDelim.Text));
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                int pos = reader.LinePosition;
                reader.MoveToNextAttribute();
                System.Windows.Forms.MessageBox.Show("Invalid Character for Movie Number " + reader.Value + " at position " + pos.ToString() + ", number of records read : " + mydivx.Movie.Count.ToString() + ". You have to correct the Movie's information with ANT Movie Catalog software !. Exception Message : " + ex.Message, "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return mydivx;
            }
            return mydivx;
        }
        private void General_Selected(object sender, TabControlCancelEventArgs e)
        {
            if (General.SelectedTab.Text == "Logos")
            {
                if (LogoView.Items.Count == 0)
                {
                    if (MesFilmsCat.Text.Length == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("You must first define a valid database configuration !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        MesFilmsCat.Focus();
                        e.Cancel = true;
                        return;
                    }
                    mydivx.Clear();
                    mydivx = ReadXml();
                    //Read_XML_Logos(Config_Name.Text);
                    Read_XML_Logos();
                }
            }
        }

        private void Read_XML_AMCconfig(string currentconfig)
        {
          if (currentconfig.Length == 0) // Do not process, if no valid config is selected !
            return;

          string wfiledefault = Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings";
          if (!System.IO.File.Exists(wfiledefault + "_" + currentconfig + ".xml"))
          {
            try
            {
              
              if (System.IO.File.Exists(wfiledefault + ".xml"))
              {
                System.IO.File.Copy(XmlConfig.EntireFilenameConfig("MyFilmsAMCSettings"), wfiledefault + "_" + currentconfig + ".xml", true);
              }
              else
              {
                MessageBox.Show("The default AMCupdater configfile cannot be found! (" + Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings.xml" + ")", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
              }
                
            }
            catch
            {
            }
          }

          string AMCconfigFile = wfiledefault + "_" + currentconfig + ".xml";
          //DataSet ds = new DataSet();
          AMCdsSettings.Clear();

          // Datei öffnen
          System.IO.FileStream fs = new System.IO.FileStream(AMCconfigFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
          // Datei einlesen
          System.IO.StreamReader sr = new System.IO.StreamReader(fs);
          // Stream in DataSet einlesen
          AMCdsSettings.ReadXml(sr, XmlReadMode.InferSchema);
          AMCdsSettings.CaseSensitive = false;
          sr.Close();
          fs.Close();

          if (AMCdsSettings.Tables.Count == 1)
            AMCdsSettings.Tables[0].PrimaryKey = new DataColumn[] { AMCdsSettings.Tables[0].Columns["Option"] };

          // load dataset for GUI display
          AMCConfigView.Items.Clear();
          int i = 0;
          foreach (DataRow dr in AMCdsSettings.Tables[0].Rows)
          {
            AMCConfigView.Items.Add(dr[1].ToString());
            AMCConfigView.Items[i].SubItems.Add(dr[0].ToString());
            i = i + 1;
          }
          // set MF GUI to values from AMC file, if values are present
          if (!string.IsNullOrEmpty(this.AMCGetAttribute("Movie_Scan_Path")))
            AMCMovieScanPath.Text = this.AMCGetAttribute("Movie_Scan_Path");
          if (!string.IsNullOrEmpty(this.AMCGetAttribute("Purge_Missing_Files")))
          {
            if (AMCGetAttribute("Purge_Missing_Files").ToLower() == "true") chkAMC_Purge_Missing_Files.Checked = true;
            else chkAMC_Purge_Missing_Files.Checked = false;
          }

        //  if (i > 0)
        //  {
        //    lblAMCupdaterConfigPreview.Visible = true;
        //    btnCreateAMCDefaultConfig.Enabled = true;
        //    btnCreateAMCDesktopIcon.Enabled = true;


        //    if (AMCMovieScanPath.Text.Length != 0 || !initialconfig)
        //    {
        //      AMCConfigView.Visible = true;
        //      AMCMovieScanPath.Visible = false;
        //      lblAMCMovieScanPath.Visible = false;
        //      btnAMCMovieScanPathAdd.Visible = false;
        //      chkAMC_Purge_Missing_Files.Visible = false;
        //      txtAMCUpd_cnf.Text = AMCconfigFile;
        //      txtAMCUpd_cnf.Enabled = false;
        //    }
        //    else
        //    {
        //      AMCConfigView.Visible = false;
        //      AMCMovieScanPath.Visible = true;
        //      lblAMCMovieScanPath.Visible = true;
        //      btnAMCMovieScanPathAdd.Visible = true;
        //      chkAMC_Purge_Missing_Files.Visible = true;
        //    }
        //  }
        //  else
        //  {
        //    lblAMCupdaterConfigPreview.Visible = false;
        //    btnCreateAMCDefaultConfig.Enabled = true;
        //    btnCreateAMCDesktopIcon.Enabled = false;
        //    btnLaunchAMCupdater.Enabled = false;
        //    AMCConfigView.Visible = false;
        //  }
        }

        private void Save_XML_AMCconfig(string currentconfig)
        {
          //Save AMC configuration to file (before launching AMCupdater with it)
          string UserSettingsFile = Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings" + "_" + currentconfig + ".xml";

          if (System.IO.File.Exists(UserSettingsFile))
            System.IO.File.Delete(UserSettingsFile);
          try
          {
            AMCdsSettings.WriteXml(UserSettingsFile);
            LogMyFilms.Debug("AMCupdater Settings saved to file: '" + UserSettingsFile + "'");
          }
          catch
          {
          }
        }

        private void AMCSetAttribute(string OptionName, string OptionValue)
        {
          DataRow row = null;
          if (AMCdsSettings.Tables.Count > 0)
            row = AMCdsSettings.Tables[0].Rows.Find(OptionName);
          if (row != null)
            row["Value"] = OptionValue;
          else
            //AMCdsSettings.Tables[0].Rows.Add();
            AMCdsSettings.Tables[0].Rows.Add(OptionValue, OptionName);
        }

        private string AMCGetAttribute(string OptionName)
        {
          DataRow row = null;
          if (AMCdsSettings.Tables.Count > 0)
            row = AMCdsSettings.Tables[0].Rows.Find(OptionName);
          if (row != null)
            return row["Value"].ToString().ToLower();
          else 
            return string.Empty;
        }

        //private void Read_XML_Logos(string currentconfig)
        private void Read_XML_Logos()
        {
          //LogoView.Clear();
          string wfile = XmlConfig.EntireFilenameConfig("MyFilmsLogos").Substring(0, XmlConfig.EntireFilenameConfig("MyFilmsLogos").LastIndexOf("."));
          //if (!System.IO.File.Exists(wfile + "_" + currentconfig + ".xml") && currentconfig.Length > 0)
          //{
          //    try
          //    {
          //        System.IO.File.Copy(XmlConfig.EntireFilenameConfig("MyFilmsLogos"), wfile + "_" + currentconfig + ".xml", true);
          //        //wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1) + "_" + currentconfig;
          //    }
          //    catch
          //    {
          //        wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
          //    }
          //}
          //else
          //wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1) + "_" + currentconfig;

          wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
          //txtLogosPath.Text = XmlConfig.ReadXmlConfig("MyFilmsLogos_" + Configuration.CurrentConfig, "ID0000", "LogosPath", Config.GetDirectoryInfo(Config.Dir.Thumbs).ToString() + "\\MyFilms_Logos");
          txtLogosPath.Text = XmlConfig.ReadXmlConfig(wfile, "ID0000", "LogosPath", "");
          comboBoxLogoSpacing.Text = XmlConfig.ReadXmlConfig(wfile, "ID0000", "Spacing", "1");
          selected_Logo_Item = -1;
          comboBoxLogoPresets.Text = XmlConfig.ReadXmlConfig(wfile, "ID0000", "LogoPresets", "Use Logos of currently selected skin");
          LogosPresetSelect();
          Read_XML_Logos_Details(wfile);
        }

        private void Read_XML_Logos_Details(string wfile)
        {
          LogoView.Items.Clear();
          //LogoView.SelectedItems[0].Remove();
          selected_Logo_Item = -1;
          //string logoConfigPathSkin;
          string skinLogoPath;
          using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.MPSettings())
          {
            skinLogoPath = Config.GetDirectoryInfo(Config.Dir.Skin) + @"\" + xmlreader.GetValueAsString("skin", "name", "NoSkin") + @"\Media\Logos";
            // Get current path to logos in skindirectory
            //logoConfigPathSkin = Config.GetDirectoryInfo(Config.Dir.Skin) + @"\" + xmlreader.GetValueAsString("skin", "name", "NoSkin"); // Get current path to active skin directory
          }
          ActiveLogoPath = XmlConfig.ReadXmlConfig(wfile, "ID0000", "LogosPath", "");
          //Recreate the path to make it OS independant...
          if (ActiveLogoPath.Length < 1) // Fall back to default skin logos !
          {
            ActiveLogoPath = skinLogoPath;
          }
          else
          {
            if (ActiveLogoPath.ToLower().Contains(@"Team Mediaportal\Mediaportal".ToLower()))
            {
              int pos = ActiveLogoPath.ToLower().LastIndexOf(@"Team Mediaportal\Mediaportal".ToLower());
              ActiveLogoPath = ActiveLogoPath.Substring(pos + @"Team Mediaportal\Mediaportal".Length);
              ActiveLogoPath = Config.GetDirectoryInfo(Config.Dir.Config) + ActiveLogoPath;
            }
          }
          if (ActiveLogoPath.LastIndexOf("\\") != ActiveLogoPath.Length - 1) ActiveLogoPath = ActiveLogoPath + "\\";
          LogMyFilms.Debug("Logo path for reading logos        : '" + ActiveLogoPath + "'");

          int i = 0;
          do
          {
            string wline = XmlConfig.ReadXmlConfig(wfile, "ID2001", "ID2001_" + i, null);
            if (wline == null) break;
            string[] wtab = wline.Split(new Char[] { ';' });
            Charge_LogosView(ref wtab, i, "ID2001", ActiveLogoPath);
            i++;
          }
          while (true);
          i = 0;
          do
          {
            string wline = XmlConfig.ReadXmlConfig(wfile, "ID2002", "ID2002_" + i, null);
            if (wline == null) break;
            string[] wtab = wline.Split(new Char[] { ';' });
            Charge_LogosView(ref wtab, i, "ID2002", ActiveLogoPath);
            i++;
          }
          while (true);
          i = 0;
          do
          {
            string wline = XmlConfig.ReadXmlConfig(wfile, "ID2003", "ID2003_" + i, null);
            if (wline == null) break;
            string[] wtab = wline.Split(new Char[] { ';' });
            Charge_LogosView(ref wtab, i, "ID2003", ActiveLogoPath);
            i++;
          }
          while (true);
          i = 0;
        }

        private void Charge_LogosView(ref string[] wtab, int i, string typelogo, string logopath)
        {
            LogoView.Items.Add(typelogo);
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[0].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[1].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[2].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[3].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[4].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[5].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[6].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetFileName(wtab[7].ToString()));
            if (System.IO.File.Exists(wtab[7].ToString()))
              LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(wtab[7].ToString()));
            else
                if (System.IO.File.Exists(logopath +  wtab[7])) // Check if Logofile exists in default media directory of current skin and add it, if found
                    LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(logopath + wtab[7]));
                else
                  if (!wtab[7].Contains("\\")) // Check, if logo file is present in subdirectories of logo directory of current skin - only if not already full path defined !

                    {
                      string[] filePathsLogoSearch = System.IO.Directory.GetFiles(logopath, wtab[7], System.IO.SearchOption.AllDirectories);
                      if (filePathsLogoSearch.Length > 0)
                      {
                        //LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(filePathsLogoSearch[0]));
                        LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(filePathsLogoSearch[0]));
                      }
                      else
                        LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(""); // Add empty field, if logofile istn't found anywhere // Add Space, as otherwise there is outofindex error later ...
                    }
                  else
                    LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(wtab[7]));
        }
        private void chkLogos_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLogos.Checked)
            {
                LogoView.Enabled = true;
                SLogo_Type.Enabled = true;
                SField1.Enabled = true;
                SOp1.Enabled = true;
                SValue1.Enabled = true;
                SField2.Enabled = true;
                SOp2.Enabled = true;
                SValue2.Enabled = true;
                SAnd_Or.Enabled = true;
                SPicture.Enabled = true;
                btnAdd.Enabled = true;
                btnDel.Enabled = true;
                btnUp.Enabled = true;
                btnDown.Enabled = true;
                btnUpdate.Enabled = true;
                txtLogosPath.Enabled = true;
                btnLogosPath.Enabled = true;
                comboBoxLogoSpacing.Enabled = true;
                btnLogoClearCache.Enabled = true;
                txtLogosPath.Enabled = true;
                comboBoxLogoPresets.Enabled = true;
                if (LogoView.Items.Count == 0)
                  Read_XML_Logos(); // Old: //Read_XML_Logos(Config_Name.Text);
            }
            else
            {
                LogoView.Enabled = false;
                SLogo_Type.Enabled = false;
                SField1.Enabled = false;
                SOp1.Enabled = false;
                SValue1.Enabled = false;
                SField2.Enabled = false;
                SOp2.Enabled = false;
                SValue2.Enabled = false;
                SAnd_Or.Enabled = false;
                SPicture.Enabled = false;
                btnAdd.Enabled = false;
                btnDel.Enabled = false;
                btnUp.Enabled = false;
                btnDown.Enabled = false;
                btnUpdate.Enabled = false;
                txtLogosPath.Enabled = true;
                btnLogosPath.Enabled = true;
                comboBoxLogoSpacing.Enabled = false;
                btnLogoClearCache.Enabled = false;
                txtLogosPath.Enabled = false;
                comboBoxLogoPresets.Enabled = false;
            }
        }

        private void chkSuppress_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSuppress.Checked)
            {
                gpsuppress.Enabled = true;
                gpspfield.Enabled = true;
                chksupplaystop.Enabled = true;
            }
            else
            {
                gpsuppress.Enabled = false;
                chksupplaystop.Checked = false;
                gpspfield.Enabled = false;
                chksupplaystop.Enabled = false;
            }

        }

        private void rbsuppress_CheckedChanged(object sender, EventArgs e)
        {
            if ((rbsuppress3.Checked) || (rbsuppress4.Checked))
                gpspfield.Enabled = true;
            else
                gpspfield.Enabled = false;
        }

        private void cbfdupdate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbfdupdate.SelectedItem.ToString() == "Checked" && !(load))
            {
                General.SelectTab(4);
                cbfdupdate.Focus();
                System.Windows.Forms.MessageBox.Show("Be carefull, if you use the field 'Checked' for deleted movies, you cann't get any difference between deleted and launching movies !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void chksupplaystop_CheckedChanged(object sender, EventArgs e)
        {
            if (chksupplaystop.Checked && !(load))
            {
                General.SelectTab(4);
                chksupplaystop.Focus();
                System.Windows.Forms.MessageBox.Show("Be carefull, that deletion action'll be done each time ended watching movie !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void MesFilmsCat_Leave(object sender, EventArgs e)
        {
            if (MesFilmsCat.Text.Length > 0)
                Control_Database(MesFilmsCat.Text);
        }

        private void btnLogosPath_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLogosPath.Text))
              openFileDialog1.FileName = txtLogosPath.Text;
            else
            {
              openFileDialog1.FileName = String.Empty;
              openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config).ToString();
            }

            if (txtLogosPath.Text.Contains("\\"))
              openFileDialog1.InitialDirectory = txtLogosPath.Text.Substring(0, txtLogosPath.Text.LastIndexOf("\\") + 1);

            folderBrowserDialog1.Description = "Select Search Path to Logos";
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                txtLogosPath.Text = folderBrowserDialog1.SelectedPath;
                LogosPresetSelect();
                //Read_XML_Logos(Config_Name.Text);
                string wfile = XmlConfig.EntireFilenameConfig("MyFilmsLogos").Substring(0, XmlConfig.EntireFilenameConfig("MyFilmsLogos").LastIndexOf("."));
                wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
                Save_XML_Logos();
                Read_XML_Logos_Details(wfile);
                LogoClearCache(false);
                SFilePicture.Text = String.Empty;
                if (SPicture.BackgroundImage != null)
                {
                  SPicture.BackgroundImage.Dispose();
                  SPicture.BackgroundImage = null;
                }
                selected_Logo_Item = -1;
            }
        }

        private void SOp1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SOp1.Text == "filled" || SOp1.Text == "not filled")
            {
                SValue1.ResetText();
                SValue1.Enabled = false;
            }
            else
                SValue1.Enabled = true;
        }

        private void SOp2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SOp2.Text == "filled" || SOp2.Text == "not filled")
            {
                SValue2.ResetText();
                SValue2.Enabled = false;
            }
            else
                SValue2.Enabled = true;
        }

        private void MesFilmsCat_TextChanged(object sender, EventArgs e)
        {
          if (CatalogType.SelectedIndex == 4 || CatalogType.SelectedIndex == 5) // EAX Movie Catalog 2.5.0 or 3.0.9
                if (MesFilmsImg.Text.Length == 0)
                    MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Pictures";
        }

        private void AntViewText1_Leave(object sender, EventArgs e)
        {
            AntViewText_Change();
        }

        private void AntViewText2_Leave(object sender, EventArgs e)
        {
            AntViewText_Change();
        }

        private void AntViewText3_Leave(object sender, EventArgs e)
        {
            AntViewText_Change();
        }

        private void AntViewText4_Leave(object sender, EventArgs e)
        {
            AntViewText_Change();
        }

        private void AntViewText5_Leave(object sender, EventArgs e)
        {
            AntViewText_Change();
        }

        private void AntViewText_Change()
        {
            View_Dflt_Item.Items.Clear();
            View_Dflt_Item.Items.Add("(none)");
            View_Dflt_Item.Items.Add(GUILocalizeStrings.Get(342)); //Films
            View_Dflt_Item.Items.Add("Year");
            View_Dflt_Item.Items.Add("Category");
            View_Dflt_Item.Items.Add("Country");
            if (this.AntStorage.Text.Length != 0 && !(AntStorage.Text == "(none)"))
                View_Dflt_Item.Items.Add("Storage");
            if (!(AntViewItem1.Text == "(none)") && this.AntViewItem1.Text.Length != 0 && this.AntViewText1.Text.Length != 0)
                View_Dflt_Item.Items.Add(AntViewText1.Text);
            if (!(AntViewItem2.Text == "(none)") && this.AntViewItem2.Text.Length != 0 && this.AntViewText2.Text.Length != 0)
                View_Dflt_Item.Items.Add(AntViewText2.Text);
            if (!(AntViewItem3.Text == "(none)") && this.AntViewItem3.Text.Length != 0 && this.AntViewText3.Text.Length != 0)
                View_Dflt_Item.Items.Add(AntViewText3.Text);
            if (!(AntViewItem4.Text == "(none)") && this.AntViewItem4.Text.Length != 0 && this.AntViewText4.Text.Length != 0)
                View_Dflt_Item.Items.Add(AntViewText4.Text);
            if (!(AntViewItem5.Text == "(none)") && this.AntViewItem5.Text.Length != 0 && this.AntViewText5.Text.Length != 0)
                View_Dflt_Item.Items.Add(AntViewText5.Text);
            if (!(View_Dflt_Item.Items.Contains(View_Dflt_Item.Text)))
            {
                View_Dflt_Item.Text = "(none)";
                View_Dflt_Text.Text = string.Empty;
            }
        }
        private void AntSort_Change()
        {
            Sort.Items.Clear();
            //if (AntSTitle.Text.Length > 0 && AntSTitle.Text != "(none)")
            //    Sort.Items.Add(AntSTitle.Text);
            //else
            //    Sort.Items.Add(AntTitle1);
            Sort.Items.Add("Title");
            Sort.Items.Add("Year");
            Sort.Items.Add("Date");
            Sort.Items.Add("Rating");
            if (!(AntSort1.Text == "(none)") && this.AntSort1.Text.Length != 0)
                Sort.Items.Add(AntSort1.Text);
            if (!(AntSort2.Text == "(none)") && this.AntSort2.Text.Length != 0)
                Sort.Items.Add(AntSort2.Text);

            //Guzzi: Added to not Reset setting when localized strings present
            if (
                !(Sort.Text.ToLower().Contains("title")) && 
                !(Sort.Text.ToLower() == "year") &&
                !(Sort.Text.ToLower().Contains("date")) &&
                !(Sort.Text.ToLower() == "rating") &&
                !(AntSort1.Text.Length == 0) &&
                !(AntSort2.Text.Length == 0) &&
                !(Sort.Text.ToLower() == AntSort1.Text.ToLower()) &&
                !(Sort.Text.ToLower() == AntSort2.Text.ToLower())
                )
                    {
                        Sort.Text = "(none)";
                    }
            
//            if (!(Sort.Items.Contains(Sort.Text)))
//            {
//                Sort.Text = "(none)";
//            }
        }


        private void View_Dflt_Item_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (View_Dflt_Item.Text == "(none)")
            {
                View_Dflt_Text.Clear();
                return;
            }
            if (View_Dflt_Item.Text == AntViewText1.Text && AntViewValue1.Text.Length > 0)
            {
                View_Dflt_Text.Text = AntViewValue1.Text;
                View_Dflt_Text.Enabled = false;
                return;
            }
            if (View_Dflt_Item.Text == AntViewText2.Text && AntViewValue2.Text.Length > 0)
            {
                View_Dflt_Text.Text = AntViewValue2.Text;
                View_Dflt_Text.Enabled = false;
                return;
            }
            if (View_Dflt_Item.Text == AntViewText3.Text && AntViewValue3.Text.Length > 0)
            {
                View_Dflt_Text.Text = AntViewValue3.Text;
                View_Dflt_Text.Enabled = false;
                return;
            }
            if (View_Dflt_Item.Text == AntViewText4.Text && AntViewValue4.Text.Length > 0)
            {
                View_Dflt_Text.Text = AntViewValue4.Text;
                View_Dflt_Text.Enabled = false;
                return;
            }
            if (View_Dflt_Item.Text == AntViewText5.Text && AntViewValue5.Text.Length > 0)
            {
                View_Dflt_Text.Text = AntViewValue5.Text;
                View_Dflt_Text.Enabled = false;
                return;
            }
            View_Dflt_Text.Enabled = true;

        }

        private void AntSort1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AntSort_Change();
        }

        private void AntSort2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AntSort_Change();
        }

        private void AntViewItem1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AntViewText_Change();
        }

        private void AntViewItem2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AntViewText_Change();
        }

        private void AntViewItem3_SelectedIndexChanged(object sender, EventArgs e)
        {
            AntViewText_Change();
        }

        private void AntViewItem4_SelectedIndexChanged(object sender, EventArgs e)
        {
            AntViewText_Change();
        }

        private void AntViewItem5_SelectedIndexChanged(object sender, EventArgs e)
        {
            AntViewText_Change();
        }

        public Task CreateTask(string name)
        {
            Task t;
            try
            {
                t = st.CreateTask(name);
            }
            catch (ArgumentException)
            {
                return null;
            }
            string AMCupdaterExecutable = Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCUpdater.exe";
            t.ApplicationName = AMCupdaterExecutable;
            t.Parameters = txtAMCUpd_cnf.Text;
            t.Comment = "Updating the database for the MP plugin MyFilms";
            t.Creator = "MP-Plugin MyFilms";
            t.WorkingDirectory = AMCupdaterExecutable.Substring(0, AMCupdaterExecutable.LastIndexOf("\\"));
            t.SetAccountInformation(Environment.UserName, (string)null);
            t.Flags = TaskFlags.RunOnlyIfLoggedOn;
            t.IdleWaitDeadlineMinutes = 20;
            t.IdleWaitMinutes = 10;
            t.MaxRunTime = new TimeSpan(1, 0, 0);
            t.Priority = System.Diagnostics.ProcessPriorityClass.High;
            t.Triggers.Add(new RunOnceTrigger(DateTime.Now + TimeSpan.FromDays(1.0) + TimeSpan.FromMinutes(3.0)));
            t.Triggers.Add(new DailyTrigger(8, 30, 1));
            t.Triggers.Add(new WeeklyTrigger(6, 0, DaysOfTheWeek.Sunday));
            t.Triggers.Add(new MonthlyDOWTrigger(8, 0, DaysOfTheWeek.Monday | DaysOfTheWeek.Thursday, WhichWeek.FirstWeek | WhichWeek.ThirdWeek));
            int[] days = { 1, 8, 15, 22, 29 };
            t.Triggers.Add(new MonthlyTrigger(9, 0, days, MonthsOfTheYear.July));
            t.Triggers.Add(new OnIdleTrigger());
            t.Triggers.Add(new OnLogonTrigger());
            t.Triggers.Add(new OnSystemStartTrigger());
            return t;
        }

        private void btnParameters_Click(object sender, EventArgs e)
        {
            st = new ScheduledTasks();
            Task t;
            string name = MesFilmsCat.Text.Substring(MesFilmsCat.Text.LastIndexOf("\\") + 1);
            name = name.Substring(0, name.Length - 4);
            try
            {
                t = st.OpenTask("MyFilms_AMCUpdater_" + name);
            }
            catch (ArgumentException)
            {
                t = CreateTask("MyFilms_AMCUpdater_" + name);
            }
            if (t != null)
            {
                bool OK = t.DisplayPropertySheet();
                if (OK)
                {
                    t.Save();
                    MessageBox.Show("Scheduled Task saved !");
                }
            }
        }

        private void scheduleAMCUpdater_CheckedChanged(object sender, EventArgs e)
        {
            if (load)
                return;
            st = new ScheduledTasks();
            Task t = null;
            string name = MesFilmsCat.Text.Substring(MesFilmsCat.Text.LastIndexOf("\\") + 1);
            name = name.Substring(0, name.Length - 4);
            if (scheduleAMCUpdater.Checked)
            {
                btnParameters.Enabled = true;
                try
                {
                    t = st.OpenTask("MyFilms_AMCUpdater_" + name);
                }
                catch (ArgumentException)
                {
                }
                if (t == null)
                {
                    t = CreateTask("MyFilms_AMCUpdater_" + name);
                }
                if (t != null)
                {
                    bool OK = t.DisplayPropertySheet();
                    if (OK)
                    {
                        t.Save();
                        MessageBox.Show("Scheduled Task saved !");
                    }
                }
            }
            else
            {
                btnParameters.Enabled = false;
                try
                {
                    t = st.OpenTask("MyFilms_AMCUpdater_" + name);
                    bool OK = st.DeleteTask("MyFilms_AMCUpdater_" + name);
                    if (OK)
                        MessageBox.Show("Scheduled Task deleted !");
                }
                catch (ArgumentException)
                {
                }
            }
        }

        private void btnViews_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(MesFilmsViews.Text))
              folderBrowserDialog1.SelectedPath = MesFilmsViews.Text;
            else
              folderBrowserDialog1.SelectedPath = String.Empty;
            folderBrowserDialog1.Description = "Select Group Views Picture Path";
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                MesFilmsViews.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void ButDefCov_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DefaultCover.Text))
              openFileDialog1.FileName = DefaultCover.Text;
            else
            {
              openFileDialog1.FileName = String.Empty;
              openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\DefaultImages";
            }

            if (DefaultCover.Text.Contains("\\"))
              openFileDialog1.InitialDirectory = DefaultCover.Text.Substring(0, DefaultCover.Text.LastIndexOf("\\") + 1);

            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "jpg";
            openFileDialog1.Filter = "JPG Files|*.jpg|PNG Files|*.png|BMP Files|*.bmp|All Files|*.*";
            openFileDialog1.Title = "Select Default Movie Cover";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                DefaultCover.Text = openFileDialog1.FileName;
        }

        private void btnResetThumbs_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to reset all generated Thumbs?", "Information", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (string wfile in System.IO.Directory.GetFiles(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups", "*.*", SearchOption.AllDirectories))
                {
                  try
                  {
                    if (wfile != DefaultCover.Text && wfile.Substring(wfile.LastIndexOf("\\")).ToLower() != "default.jpg")
                      System.IO.File.Delete(wfile);
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.Debug("Setup: Error deleting file '" + wfile.ToString() + "' - Exception: " + ex.ToString());
                  }  
                }
            }
        }

        private void AntSearchField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AntSearchList.Text.Contains(AntSearchField.Text))
            {
                if (AntSearchList.Text.Length > 0)
                    AntSearchList.Text = AntSearchList.Text + ", " + AntSearchField.Text;
                else
                    AntSearchList.Text = AntSearchField.Text;
            }
        }
        private void AntUpdField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AntUpdList.Text.Contains(AntUpdField.Text))
            {
                if (AntUpdList.Text.Length > 0)
                    AntUpdList.Text = AntUpdList.Text + ", " + AntUpdField.Text;
                else
                    AntUpdList.Text = AntUpdField.Text;
            }
        }

        private void btnTrailer_Click(object sender, EventArgs e)
        {
          if (!String.IsNullOrEmpty(PathStorageTrailer.Text))
            {
              if (PathStorageTrailer.Text.Contains(";"))
                folderBrowserDialog1.SelectedPath = PathStorageTrailer.Text.Substring(PathStorageTrailer.Text.LastIndexOf(";") + 1).Trim();
              else
                folderBrowserDialog1.SelectedPath = PathStorageTrailer.Text;
              if (folderBrowserDialog1.SelectedPath.LastIndexOf("\\") == folderBrowserDialog1.SelectedPath.Length)
                folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath.Substring(folderBrowserDialog1.SelectedPath.Length - 1);
            }
            else
              folderBrowserDialog1.SelectedPath = String.Empty;

            folderBrowserDialog1.Description = "Select Extended Trailer Searchpath";
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (this.folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") != this.folderBrowserDialog1.SelectedPath.Length - 1)
                    folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";

                if (PathStorageTrailer.Text.Length == 0)
                    PathStorageTrailer.Text = folderBrowserDialog1.SelectedPath;
                else
                    PathStorageTrailer.Text = PathStorageTrailer.Text + ";" + folderBrowserDialog1.SelectedPath;
            }
        }

        private void NAS_1_Name_TextChanged(object sender, EventArgs e)
        {
            if (NAS_Name_1.Text.Length == 0)
            NAS_MAC_1.Text = "";
        }

        private void check_WOL_enable_CheckedChanged(object sender, EventArgs e)
        {
            if (check_WOL_enable.Checked)
            {
                check_WOL_Userdialog.Enabled = true;
                NAS_Name_1.Enabled = true;
                NAS_Name_2.Enabled = true;
                NAS_Name_3.Enabled = true;
                NAS_MAC_1.Enabled = true;
                NAS_MAC_2.Enabled = true;
                NAS_MAC_3.Enabled = true;
            }
            else
            {
                check_WOL_Userdialog.Enabled = false;
                NAS_Name_1.Enabled = false;
                NAS_Name_2.Enabled = false;
                NAS_Name_3.Enabled = false;
                NAS_MAC_1.Enabled = false;
                NAS_MAC_2.Enabled = false;
                NAS_MAC_3.Enabled = false;
            }

        }

        private void ButImgArtist_Click(object sender, EventArgs e)
        {
          if (!String.IsNullOrEmpty(MesFilmsImgArtist.Text))
            folderBrowserDialog1.SelectedPath = MesFilmsImgArtist.Text;
          else
            folderBrowserDialog1.SelectedPath = String.Empty;
          folderBrowserDialog1.Description = "Select Person Images Path";
          if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
          {
              MesFilmsImgArtist.Text = folderBrowserDialog1.SelectedPath;
          }

        }

        private void ButDefCovArtist_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DefaultCoverArtist.Text))
              openFileDialog1.FileName = DefaultCoverArtist.Text;
            else
            {
              openFileDialog1.FileName = String.Empty;
              openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\DefaultImages";
            }

            if (DefaultCoverArtist.Text.Contains("\\"))
              openFileDialog1.InitialDirectory = DefaultCoverArtist.Text.Substring(0, DefaultCoverArtist.Text.LastIndexOf("\\") + 1);

            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "jpg";
            openFileDialog1.Filter = "JPG Files|*.jpg|PNG Files|*.png|BMP Files|*.bmp|All Files|*.*";
            openFileDialog1.Title = "Select Default Person Cover Image";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                DefaultCoverArtist.Text = openFileDialog1.FileName;
        }

        private void btnResetThumbsArtist_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to reset all generated artist thumbs?", "Information", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (string wfile in System.IO.Directory.GetFiles(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Persons"))
                {
                  try
                  {
                    if (wfile != DefaultCoverArtist.Text)
                      System.IO.File.Delete(wfile);
                  }
                  catch (Exception ex)
                  {
                    LogMyFilms.Debug("Setup: Error deleting file '" + wfile.ToString() + "' - Exception: " + ex.ToString());
                  }  
                }
            }
        }

        private void buttonGetMACadresses_Click(object sender, EventArgs e)
        {
            WakeOnLanManager wakeOnLanManager = new WakeOnLanManager(); 
            String macAddress;
            byte[] hwAddress;
            IPAddress ipAddress = null;

            if (!IPAddress.TryParse(NAS_Name_1.Text, out ipAddress))
            {
                try
                {
                    IPAddress[] ips;
                    ips = Dns.GetHostAddresses(NAS_Name_1.Text);
                    foreach (IPAddress ip in ips)
                    {
                        LogMyFilms.Debug("    {0}", ip);
                    }
                    //Use first valid IP address
                    ipAddress = ips[0];
                }
                catch (Exception ex)
                {
                    LogMyFilms.Error("MyFilmsSetup - GetMACaddresses: Failed GetHostAddress - {0}", ex.Message);
                }
            }

            //Check for valid IP address
            if (ipAddress != null)
            {
                //Update the MAC address if possible
                hwAddress = wakeOnLanManager.GetHardwareAddress(ipAddress);

                if (wakeOnLanManager.IsValidEthernetAddress(hwAddress))
                {
                    LogMyFilms.Debug("TVHome: WOL - Valid auto MAC address: {0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}"
                              , hwAddress[0], hwAddress[1], hwAddress[2], hwAddress[3], hwAddress[4], hwAddress[5]);

                    //Store MAC address
                    macAddress = BitConverter.ToString(hwAddress).Replace("-", ":");

                    LogMyFilms.Debug("MyFilmsSetup - GetMACaddresses: Store MAC address: {0}", macAddress);
                    NAS_MAC_1.Text = macAddress;

                }
            }


            ipAddress = null;
            if (!IPAddress.TryParse(NAS_Name_2.Text, out ipAddress))
            {
                //Get IP address of the TV server
                try
                {
                    IPAddress[] ips;
                    ips = Dns.GetHostAddresses(NAS_Name_2.Text);
                    foreach (IPAddress ip in ips)
                    {
                        LogMyFilms.Debug("    {0}", ip);
                    }
                    //Use first valid IP address
                    ipAddress = ips[0];
                }
                catch (Exception ex)
                {
                    LogMyFilms.Error("MyFilmsSetup - GetMACaddresses: Failed GetHostAddress - {0}", ex.Message);
                }
            }

            //Check for valid IP address
            if (ipAddress != null)
            {
                //Update the MAC address if possible
                hwAddress = wakeOnLanManager.GetHardwareAddress(ipAddress);

                if (wakeOnLanManager.IsValidEthernetAddress(hwAddress))
                {
                    LogMyFilms.Debug("TVHome: WOL - Valid auto MAC address: {0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}"
                              , hwAddress[0], hwAddress[1], hwAddress[2], hwAddress[3], hwAddress[4], hwAddress[5]);

                    //Store MAC address
                    macAddress = BitConverter.ToString(hwAddress).Replace("-", ":");

                    LogMyFilms.Debug("MyFilmsSetup - GetMACaddresses: Store MAC address: {0}", macAddress);
                    NAS_MAC_2.Text = macAddress;

                }
            }

            ipAddress = null;
            if (!IPAddress.TryParse(NAS_Name_3.Text, out ipAddress))
            {
                //Get IP address of the TV server
                try
                {
                    IPAddress[] ips;
                    ips = Dns.GetHostAddresses(NAS_Name_3.Text);
                    foreach (IPAddress ip in ips)
                    {
                        LogMyFilms.Debug("    {0}", ip);
                    }
                    //Use first valid IP address
                    ipAddress = ips[0];
                }
                catch (Exception ex)
                {
                    LogMyFilms.Error("MyFilmsSetup - GetMACaddresses: Failed GetHostAddress - {0}", ex.Message);
                }
            }

            //Check for valid IP address
            if (ipAddress != null)
            {
                //Update the MAC address if possible
                hwAddress = wakeOnLanManager.GetHardwareAddress(ipAddress);

                if (wakeOnLanManager.IsValidEthernetAddress(hwAddress))
                {
                    LogMyFilms.Debug("TVHome: WOL - Valid auto MAC address: {0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}"
                              , hwAddress[0], hwAddress[1], hwAddress[2], hwAddress[3], hwAddress[4], hwAddress[5]);

                    //Store MAC address
                    macAddress = BitConverter.ToString(hwAddress).Replace("-", ":");

                    LogMyFilms.Debug("MyFilmsSetup - GetMACaddresses: Store MAC address: {0}", macAddress);
                    NAS_MAC_3.Text = macAddress;

                }
            }
        }

        private void buttonSendMagicPacket1_Click(object sender, EventArgs e)
        {
            WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();
            int intTimeOut = 30; //Timeout für WOL
            String macAddress;
            byte[] hwAddress;

            macAddress = NAS_MAC_1.Text;
            if (macAddress.Length > 1)
            {
                try
                {
                    hwAddress = wakeOnLanManager.GetHwAddrBytes(macAddress);

                    //Finally, start up the TV server
                    LogMyFilms.Info("MyFilmsSetup - WOL: Start the NAS-Server");

                    if (wakeOnLanManager.WakeupSystem(hwAddress, NAS_Name_1.Text, intTimeOut))
                    {
                        LogMyFilms.Info("MyFilmsSetup - WOL: The NAS-Server started successfully!");
                    }
                    else
                    {
                        LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server");
                    }
                }
                catch (Exception ex)
                {
                    LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server - {0}", ex.Message);
                }
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("No MAC address available for '" + NAS_Name_1 + "' to try sending Magicpacket to NAS-Server !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonSendMagicPacket2_Click(object sender, EventArgs e)
        {
            WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();
            int intTimeOut = 30; //Timeout für WOL
            String macAddress;
            byte[] hwAddress;

            macAddress = NAS_MAC_2.Text;
            if (macAddress.Length > 1)
            {
                try
                {
                    hwAddress = wakeOnLanManager.GetHwAddrBytes(macAddress);

                    //Finally, start up the TV server
                    LogMyFilms.Info("MyFilmsSetup - WOL: Start the NAS-Server");

                    if (wakeOnLanManager.WakeupSystem(hwAddress, NAS_Name_2.Text, intTimeOut))
                    {
                        LogMyFilms.Info("MyFilmsSetup - WOL: The NAS-Server started successfully!");
                    }
                    else
                    {
                        LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server");
                    }
                }
                catch (Exception ex)
                {
                    LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server - {0}", ex.Message);
                }
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("No MAC address available for '" + NAS_Name_2 + "' to try sending Magicpacket to NAS-Server !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonSendMagicPacket3_Click(object sender, EventArgs e)
        {
            WakeOnLanManager wakeOnLanManager = new WakeOnLanManager();
            int intTimeOut = 30; //Timeout für WOL
            String macAddress;
            byte[] hwAddress;

            macAddress = NAS_MAC_3.Text;
            if (macAddress.Length > 1)
            {
                try
                {
                    hwAddress = wakeOnLanManager.GetHwAddrBytes(macAddress);

                    //Finally, start up the TV server
                    LogMyFilms.Info("MyFilmsSetup - WOL: Start the NAS-Server");

                    if (wakeOnLanManager.WakeupSystem(hwAddress, NAS_Name_3.Text, intTimeOut))
                    {
                        LogMyFilms.Info("MyFilmsSetup - WOL: The NAS-Server started successfully!");
                    }
                    else
                    {
                        LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server");
                    }
                }
                catch (Exception ex)
                {
                    LogMyFilms.Error("MyFilmsSetup - WOL: Failed to start the NAS-Server - {0}", ex.Message);
                }
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("No MAC address available for '" + NAS_Name_3 + "' to try sending Magicpacket to NAS-Server !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void NAS_Name_2_TextChanged(object sender, EventArgs e)
        {
            if (NAS_Name_2.Text.Length == 0)
                NAS_MAC_2.Text = "";
        }

        private void NAS_Name_3_TextChanged(object sender, EventArgs e)
        {
            if (NAS_Name_3.Text.Length == 0)
                NAS_MAC_3.Text = "";
        }


        private void btnLaunchAMCupdater_Click(object sender, EventArgs e)
        {
            launchAMCmanager();
        }

        private void btnLaunchAMCglobal_Click(object sender, EventArgs e)
        {
            launchAMCmanager();
        }
    
        private void launchAMCmanager()
        {
          string wfiledefault = Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings";
          if (!System.IO.File.Exists(wfiledefault + "_" + Config_Name.Text + ".xml"))
          {
            MessageBox.Show("Cannot launch AMC Updater !  \n You first have to create a config for AMCupdater (create Default Config on AMC Updater Tab) !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
          }
          
          using (Process p = new Process())
          {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Config.GetDirectoryInfo(Config.Dir.Base) + @"\AMCupdater.exe";
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.Arguments = "\"" + Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings_" + Config_Name.Text + ".xml" + "\"" + " " + "LogDirectory" + " " + "GUI";
            //psi.Arguments = " \"" + Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings_" + Config_Name.Text + "\" \"" + Config.GetDirectoryInfo(Config.Dir.Log).ToString() + "\" \"GUI\"";
            psi.ErrorDialog = true;
            if (OSInfo.OSInfo.VistaOrLater())
            {
              psi.Verb = "runas";
            }

            p.StartInfo = psi;
            LogMyFilms.Debug("MyFilmsSetup: Launch AMCupdater from PluginSetup with argument: {0}", string.Empty);
            try
            {
              p.Start();
              //p.WaitForExit();
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug(ex.ToString());
            }
            LogMyFilms.Debug("MyFilmsSetup: Launch AMCupdater from PluginSetup done");
          }

        }

        private void btnCreateAMCDesktopIcon_Click(object sender, EventArgs e)
        {
          if (Config_Name.Text.Length > 0)
          {
            CreateAMCDesktopIcon();
          }
          else
          {
            System.Windows.Forms.MessageBox.Show("You need an active Configuration Name first !", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          }
        }

        private void CreateAMCDesktopIcon()
        {
          string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
          string linkName = "AMC-Updater '" + Config_Name.Text + "'";
          string commandLine = "\"" + Config.GetDirectoryInfo(Config.Dir.Base).ToString() + @"\AMCupdater.exe""" + " \"" +
                               Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings_" +
                               Config_Name.Text + "\" \"" + Config.GetDirectoryInfo(Config.Dir.Log).ToString() +
                               "\" \"GUI\"";
          string argument = "\"" + Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings_" +
                            Config_Name.Text + "\" \"" + Config.GetDirectoryInfo(Config.Dir.Log).ToString() + "\" \"GUI";

          string shortcutFile = deskDir + @"\AMC-Updater (" + Config_Name.Text + ")" + ".lnk";
          string soureFile = Config.GetDirectoryInfo(Config.Dir.Base).ToString() + @"\AMCupdater.exe";
          string description = "AMC: '" + Config_Name.Text + "'";

          string arguments = "\"" + Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings_" +
                             Config_Name.Text + ".xml" + "\"" + " " + "LogDirectory" + " " + "GUI";

          //string arguments = "\"" + Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings_" + Config_Name.Text + "\"" + " " + "\"" + Config.GetDirectoryInfo(Config.Dir.Log).ToString() + "\"" + " " + "\"" + "GUI\"";
          string hotKey = String.Empty;
          string workingDirectory = Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\scripts\MyFilms";

          if (System.IO.File.Exists(deskDir + "\\" + linkName + ".url"))
            try
            {
              System.IO.File.Delete(deskDir + "\\" + linkName + ".url");
            }
            catch
            {
            }

          try
          {
            CreateShortcut(soureFile, shortcutFile, description, arguments, hotKey, workingDirectory);
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.Message);
          }

          LogMyFilms.Debug("Setup - Successfully created Desktop Icon for '" + linkName + "'");
          if (!WizardActive)
            MessageBox.Show("Successfully created Desktop Icon for " + linkName + "", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Create Windows Shorcut
        /// </summary>
        /// <param name="SourceFile">A file you want to make shortcut to</param>
        /// <param name="ShortcutFile">Path and shorcut file name including file extension (.lnk)</param>
        /// <param name="Description">Shortcut description</param>
        /// <param name="Arguments">Command line arguments</param>
        /// <param name="HotKey">Shortcut hot key as a string, for example "Ctrl+F"</param>
        /// <param name="WorkingDirectory">"Start in" shorcut parameter</param>
        public void CreateShortcut(string SourceFile, string ShortcutFile, string Description,
           string Arguments, string HotKey, string WorkingDirectory)
        {
          // Check necessary parameters first:
          if (String.IsNullOrEmpty(SourceFile))
            throw new ArgumentNullException("SourceFile");
          if (String.IsNullOrEmpty(ShortcutFile))
            throw new ArgumentNullException("ShortcutFile");

          // Create WshShellClass instance:
          WshShellClass wshShell = new WshShellClass();

          // Create shortcut object:
          IWshRuntimeLibrary.IWshShortcut shorcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(ShortcutFile);

          // Assign shortcut properties:
          shorcut.TargetPath = SourceFile;
          shorcut.Description = Description;
          if (!String.IsNullOrEmpty(Arguments))
            shorcut.Arguments = Arguments;
          if (!String.IsNullOrEmpty(HotKey))
            shorcut.Hotkey = HotKey;
          if (!String.IsNullOrEmpty(WorkingDirectory))
            shorcut.WorkingDirectory = WorkingDirectory;

          // Save the shortcut:
          shorcut.Save();
        }

        private void btnAMCMovieScanPathAdd_Click(object sender, EventArgs e)
        {
          if (!String.IsNullOrEmpty(AMCMovieScanPath.Text))
          {
            if (AMCMovieScanPath.Text.Contains(";"))
              folderBrowserDialog1.SelectedPath = AMCMovieScanPath.Text.Substring(AMCMovieScanPath.Text.LastIndexOf(";") + 1).Trim();
            else
              folderBrowserDialog1.SelectedPath = AMCMovieScanPath.Text;
            if (folderBrowserDialog1.SelectedPath.LastIndexOf("\\") == folderBrowserDialog1.SelectedPath.Length)
              folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath.Substring(folderBrowserDialog1.SelectedPath.Length - 1);
          }
          else
            folderBrowserDialog1.SelectedPath = String.Empty;

          folderBrowserDialog1.Description = "Scan Path(es)for Movies Search";
          if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
          {
            if (!(folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") == folderBrowserDialog1.SelectedPath.Length - 1))
              folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";

            if (AMCMovieScanPath.Text.Length == 0)
              AMCMovieScanPath.Text = folderBrowserDialog1.SelectedPath;
            else
              AMCMovieScanPath.Text = AMCMovieScanPath.Text + ";" + folderBrowserDialog1.SelectedPath;
          }
        }

        private void btnCreateAMCDefaultConfig_Click(object sender, EventArgs e)
        {

          if (AMCMovieScanPath.Text.Length == 0)
          {
            MessageBox.Show(
              "You first have to define the Search path for your movies to create or sync a config !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            AMCMovieScanPath.Focus();
            return;
          }
          string wfiledefault = Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings";
          if (System.IO.File.Exists(wfiledefault + ".xml"))
          {
            if (System.IO.File.Exists(wfiledefault + "_" + Config_Name.Text + ".xml"))
            {
              DialogResult dialogResult = MessageBox.Show("Are you sure you want to sync your MyFilms settings to AMC Updater? You might loose come customizations made!", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
              if (!(dialogResult == DialogResult.Yes))
              {
                return;
              }
              else
                System.IO.File.Copy(wfiledefault + "_" + Config_Name.Text + ".xml", wfiledefault + "_" + Config_Name.Text + ".xml.sav", true);
            }
            System.IO.File.Delete(wfiledefault + "_" + Config_Name.Text + ".xml");
            Read_XML_AMCconfig(Config_Name.Text); // read current (or create new default) config file
            CreateMyFilmsDefaultsForAMCconfig(Config_Name.Text); //create MF defaults
            Save_XML_AMCconfig(Config_Name.Text); // save new config
            Read_XML_AMCconfig(Config_Name.Text); // reread config file with new defaults

            if (AMCMovieScanPath.Text.Length != 0)
            {
              MessageBox.Show("Successfully created an AMCupdater default config with your settings ! \nIf you change your MyFilms settings, recreate the AMCupdater config to get config updated!", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
              MessageBox.Show("You have not set up the movie scan path for AMCupdater. \n Change setting and recreate AMCupdater config !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

          }
          else
            System.Windows.Forms.MessageBox.Show("The default AMCupdater configfile cannot be found! (" + Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings.xml" + ")", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void CreateMyFilmsDefaultsForAMCconfig(string currentconfig)
        {
          // Set Parameters from MyFilms configuration
          AMCSetAttribute("Ant_Database_Source_Field", AntStorage.Text);
          AMCSetAttribute("Excluded_Movies_File", Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCExcludedMoviesFile.txt");
          
          if (txtGrabber.Text.Length != 0)
            AMCSetAttribute("Internet_Parser_Path", txtGrabber.Text);
          else
            AMCSetAttribute("Internet_Parser_Path", Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms\IMDB.xml");
          AMCSetAttribute("Manual_Excluded_Movies_File", Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCExcludedMoviesFile.txt");
          AMCSetAttribute("Manual_Internet_Parser_Path", txtGrabber.Text);
          AMCSetAttribute("Manual_XML_File", MesFilmsCat.Text);
          AMCSetAttribute("Master_Title", AntTitle1.Text); // Check, if this should be replaced by ItemSearchGrabber...
          AMCSetAttribute("Movie_Fanart_Path", MesFilmsFanart.Text);
          AMCSetAttribute("Movie_PersonArtwork_Path", MesFilmsImgArtist.Text);
          AMCSetAttribute("Grabber_Override_Language", cbGrabberOverrideLanguage.Text);
          AMCSetAttribute("Grabber_Override_PersonLimit", cbGrabberOverridePersonLimit.Text);
          AMCSetAttribute("Grabber_Override_TitleLimit", cbGrabberOverrideTitleLimit.Text);
          AMCSetAttribute("Grabber_Override_GetRoles", cbGrabberOverrideGetRoles.Text);

          AMCSetAttribute("XML_File", MesFilmsCat.Text);

          // Set specific userdefined Parameters from MyFilms GUI configuration
          AMCSetAttribute("Movie_Scan_Path", AMCMovieScanPath.Text);
          if (chkAMC_Purge_Missing_Files.Checked)
            AMCSetAttribute("Purge_Missing_Files", "true");
          else
            AMCSetAttribute("Purge_Missing_Files", "false");
          AMCSetAttribute("LogDirectory", Config.GetDirectoryInfo(Config.Dir.Config) + @"\log");
          if (!string.IsNullOrEmpty(cbPictureHandling.Text))
          {
            AMCSetAttribute("Use_Folder_Dot_Jpg", "false");
            AMCSetAttribute("Store_Image_With_Relative_Path", cbPictureHandling.Text);

            //AMCSetAttribute("Image_Download_Filename_Prefix", currentconfig + "_");
            if (cbPictureHandling.Text == "Relative Path")
            {
              AMCSetAttribute("Image_Download_Filename_Prefix", txtPicturePrefix.Text); // just prefix
            }
            else if (cbPictureHandling.Text == "Full Path")
            {
              AMCSetAttribute("Image_Download_Filename_Prefix", MesFilmsImg.Text + "\\" + txtPicturePrefix); // Full path including "\\" excluding picturefilename 
            }
            else if (cbPictureHandling.Text == "Use Folder.jpg")
            {
              
            }
            else AMCSetAttribute("Image_Download_Filename_Prefix", ""); 
          }

          // Those Parameters already set via MyFilmsAMCSettings.xml (Defaultconfigfile):
          //Option	Status / Source	DefaultValue
          //Ant_Media_Label	Default	HDD
          //Ant_Media_Type	Default	File
          //Backup_XML_First	Default	True
          //Check_Field_Handling	Default	True
          //Database_Fields_To_Import	Default	###############################################################################################################################################################################################################################################################
          //Date_Handling	Default	Current System Date
          //DVD_Drive_Letter	Default	
          //Excluded_File_Strings	Default	
          //Excluded_Folder_Strings	Default	
          //Execute_Only_For_Orphans	Default	True
          //Execute_Program	Default	False
          //Execute_Program_Path	Default	
          //File_Types_Media	Default	avi;mpg;divx;mpeg;wmv;mkv
          //File_Types_Non_Media	Default	iso;img
          //Filter_Strings	Default	\([0-9][0-9][0-9][0-9]\)
          //Folder_Name_Is_Group_Name	Default	False
          //Group_Name_Applies_To	Default	Both Titles
          //Import_File_On_Internet_Lookup_Failure	Default	True
          //Internet_Lookup_Always_Prompt	Default	False
          //Log_Level	Default	All Events
          //Manual_Internet_Lookup_Always_Prompt	Default	True
          //Movie_Title_Handling	Default	Folder Name + Internet Lookup
          //Override_Path	Default	
          //Overwrite_XML_File	Default	True
          //Parse_Playlist_Files	Default	False
          //Parse_Subtitle_Files	Default	True
          //Prohibit_Internet_Lookup	Default	False
          //Read_DVD_Label	Default	False
          //RegEx_Check_For_MultiPart_Files	Default	[-|_]cd[0-9]|[-|_]disk[0-9]|[0-9]of[0-9]
          //Rescan_Moved_Files	Default	False
          //Scan_For_DVD_Folders	Default	True
          //Store_Image_With_Relative_Path	Default	True
          //Store_Short_Names_Only	Default	False
          //Use_Folder_Dot_Jpg	Default	False
          //Use_Page_Grabber	Default	False
          //Use_XBMC_nfo	Default	False
          //LogDirectory Default

        }

        private void linkLabelMyFilmsWiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
          System.Diagnostics.Process.Start("http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films");
        }

        private void AntUpdFieldReset_Click(object sender, EventArgs e)
        {
          AntUpdList.Text = String.Empty;
        }

        private void AntSearchFieldClear_Click(object sender, EventArgs e)
        {
          AntSearchList.Text = String.Empty;
        }

        private void AntUpdFieldReset_Click_1(object sender, EventArgs e)
        {
          AntUpdList.Text =
            "OriginalTitle, TranslatedTitle, Category, Year, Date, Country, Rating, Checked, MediaLabel, MediaType, Actors, Director, Producer";
        }

        private void AntSearchFieldResetToDefault_Click(object sender, EventArgs e)
        {
          AntSearchList.Text =
            "OriginalTitle, TranslatedTitle, Description, Comments, Actors, Director, Producer, Year, Date, Category, Country, Rating, Checked, MediaLabel, MediaType, URL, Borrower, Length, VideoFormat, VideoBitrate, AudioFormat, AudioBitrate, Resolution, Framerate, Size, Disks, Languages, Subtitles, Number";
        }

        private void comboBoxLogoPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
          LogosPresetSelect();
          string wfile = XmlConfig.EntireFilenameConfig("MyFilmsLogos").Substring(0, XmlConfig.EntireFilenameConfig("MyFilmsLogos").LastIndexOf("."));
          wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
          //XmlConfig.WriteXmlConfig(wfile, "ID0000", "LogoPresets", comboBoxLogoPresets.Text); // Added Presets to Logo Config
          //XmlConfig.WriteXmlConfig(wfile, "ID0000", "LogosPath", txtLogosPath.Text);
          Save_XML_Logos(); // Write Logos with new pathes for Images !
          Read_XML_Logos_Details(wfile);
          LogoClearCache(false);
          SFilePicture.Text = String.Empty;
          if (SPicture.BackgroundImage != null)
          {
            SPicture.BackgroundImage.Dispose();
            SPicture.BackgroundImage = null;
          }
          selected_Logo_Item = -1;
        }

        private void LogosPresetSelect()
        {
            //"Use Logos of currently selected skin",
            //"Use Blue3Wide logos",
            //"Use MyFilms Logo Pack",
            //"Define your path to logo image files"});
          if (comboBoxLogoPresets.Text == "Define your path to logo image files")
            {
              //txtLogosPath.Text = string.Empty;
              //StoreFullLogoPath = true;
              StoreFullLogoPath = false; // Changed on request of Dadeo
              txtLogosPath.Visible = true;
              txtLogosPath.Enabled = true;
              btnLogosPath.Visible = true;
              lblLogosPath.Visible = true;
            }
            else if (comboBoxLogoPresets.Text == "Use Logos of currently selected skin")
            {
              txtLogosPath.Text = string.Empty;
              StoreFullLogoPath = false;
              txtLogosPath.Visible = true;
              txtLogosPath.Enabled = false;
              btnLogosPath.Visible = false;
              lblLogosPath.Visible = false;
            }
            else if (comboBoxLogoPresets.Text == "Use MyFilms Logo Pack")
            {
              txtLogosPath.Text = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\DefaultLogos";
              StoreFullLogoPath = false;
              txtLogosPath.Visible = true;
              txtLogosPath.Enabled = false;
              btnLogosPath.Visible = false;
              lblLogosPath.Visible = false;
            }
            else if (comboBoxLogoPresets.Text == "Use Blue3Wide logos")
            {
#if MP11
              txtLogosPath.Text = Config.GetDirectoryInfo(Config.Dir.Skin) + @"\blue3wide\media\logos";
#else
              txtLogosPath.Text = Config.GetDirectoryInfo(Config.Dir.Skin) + @"\defaultwide\media\logos";
#endif
              StoreFullLogoPath = false;
              txtLogosPath.Visible = true;
              txtLogosPath.Enabled = false;
              btnLogosPath.Visible = false;
              lblLogosPath.Visible = false;
            }
            else
            {
              txtLogosPath.Text = string.Empty; 
              StoreFullLogoPath = false;
              txtLogosPath.Visible = false;
              btnLogosPath.Visible = false;
              lblLogosPath.Visible = false;
            }

        }

        private void btnLogoClearCache_Click(object sender, EventArgs e)
        {
          LogoClearCache(true);
        }

        private void LogoClearCache(bool showmessage)
        {
          if (!System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Logos"))
            System.IO.Directory.CreateDirectory(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Logos");
          int i = 0;
          foreach (string sFile in System.IO.Directory.GetFiles(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Logos"))
          {
            if (sFile.ToUpper().EndsWith(".PNG"))
              System.IO.File.Delete(sFile);
            i = i + 1;
          }
          if (showmessage)
          {
            if (i == 0)
              MessageBox.Show(
                "No cached logo files to delete, your logo cache is already empty !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
              MessageBox.Show("Successfully cleared " + i.ToString() + " cached files in your logo cache directory! Be aware browsing your movies might be slower when rebuilding the logos in cache.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
          }
        }

        private void AntTitle1_SelectedIndexChanged(object sender, EventArgs e)
        {
          if (AntTitle1.Text == "TranslatedTitle")
          {
            if (AntSearchList.Text.Length == 0)
              AntSearchList.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SearchList", "TranslatedTitle, OriginalTitle, Description, Comments, Actors, Director, Producer, Year, Date, Category, Country, Rating, Checked, MediaLabel, MediaType, URL, Borrower, Length, VideoFormat, VideoBitrate, AudioFormat, AudioBitrate, Resolution, Framerate, Size, Disks, Languages, Subtitles, Number");
            if (AntUpdList.Text.Length == 0)
              AntUpdList.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "UpdateList", "TranslatedTitle, OriginalTitle, Category, Year, Date, Country, Rating, Checked, MediaLabel, MediaType, Actors, Director, Producer");
          }
          if (AntTitle1.Text == "OriginalTitle" || AntTitle1.Text == "FormattedTitle")
          {
            if (AntSearchList.Text.Length == 0)
              AntSearchList.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "SearchList", "OriginalTitle, TranslatedTitle, Description, Comments, Actors, Director, Producer, Year, Date, Category, Country, Rating, Checked, MediaLabel, MediaType, URL, Borrower, Length, VideoFormat, VideoBitrate, AudioFormat, AudioBitrate, Resolution, Framerate, Size, Disks, Languages, Subtitles, Number");
            if (AntUpdList.Text.Length == 0)
              AntUpdList.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "UpdateList", "OriginalTitle, TranslatedTitle, Category, Year, Date, Country, Rating, Checked, MediaLabel, MediaType, Actors, Director, Producer");
          }
        }

        private void btnFirstTimeSetup_Click(object sender, EventArgs e)
        {
          WizardActive = true;
          newCatalogWizard();
          WizardActive = false;
        }

        private void newCatalogWizard()
        {
          bool newCatalog = true;
          if (Config_Name.Text.Length != 0 || RunWizardAfterInstall)
          {
            if (System.Windows.Forms.MessageBox.Show("Do you want to create a new MyFilms Configuration ? \n\nThis wizard helps you to setup a new configuration with default settings. \nIf you select 'yes', enter a name for the configuration.\nIf you select 'no' you can relaunch the wizard later with the 'Setup Wizard' button.", "MyFilms Configuration Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
              return;
          }
          MyFilmsInputBox input = new MyFilmsInputBox();
          input.CatalogTypeSelectedIndex = 0; // preset to ANT MC 
          input.CatalogType = "Ant Movie Catalog (V3.5.1.2)"; // preset to Ant Movie Catalog (V3.5.1.2)
          input.Country = "USA";
          input.ShowDialog(this);
          string newConfig_Name = input.ConfigName;
          string newCatalogType = input.CatalogType;
          string newCountry = input.Country;
          int newCatalogSelectedIndex = input.CatalogTypeSelectedIndex;
          if (string.IsNullOrEmpty(newConfig_Name))
          {
            MessageBox.Show("New Config Name must not be empty ! No Config created !", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
          }
          if (newConfig_Name == Config_Name.Text)
          {
            MessageBox.Show("Config Name must be different from existing ones ! No Config created !", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
          }

          // Set Configuration Name & initialize new current config
          Config_Name.Text = newConfig_Name;
          this.Refresh_Items(true);

          CatalogType.SelectedIndex = 0; // can be "10" = "MyFilms DB", if standalone with extended features/DB-fields is supported...

          // Ask user to select existing or create new catalog...
          bool useExistingCatalog = true;
          if (newCatalogSelectedIndex == 0 || newCatalogSelectedIndex == 7)
          {
            if (System.Windows.Forms.MessageBox.Show("Do you want to use an existing catalog? \n\nIf you select 'yes', you will be asked to select the path to your existing catalog file.\n If you select 'no' you will create a new empty catalog.", "MyFilms Configuration Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
              useExistingCatalog = true;
            else
              useExistingCatalog = false;
          }
          else
          {
            System.Windows.Forms.MessageBox.Show(
            "Please select the path to your existing catalog file. \n (You have to export your movie collection to xml format in your catalog manager first to use it in myfilms.)",
            "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          }
          if (useExistingCatalog)
          {
            string CatalogDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\thumbs\MyFilms\Catalog";
            //if (!System.IO.Directory.Exists(CatalogDirectory))
            //{
            //  try {System.IO.Directory.CreateDirectory(CatalogDirectory);}
            //  catch {}
            //}
            // Ask User for existing database file
            newCatalog = false;
            openFileDialog1.FileName = String.Empty;
            if (System.IO.Directory.Exists(CatalogDirectory))
              openFileDialog1.InitialDirectory = CatalogDirectory;
            else
              openFileDialog1.InitialDirectory = "";
            if (!string.IsNullOrEmpty(MesFilmsCat.Text) && MesFilmsCat.Text.Contains("\\"))
              openFileDialog1.InitialDirectory = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\") + 1);
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "xml";
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Select Movie Catalog File (xml)";
            openFileDialog1.CheckFileExists = false;
            
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
              Control_Database(openFileDialog1.FileName);
            }
          }
          else
          {
            string CatalogDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\thumbs\MyFilms\Catalog";
            if (!System.IO.Directory.Exists(CatalogDirectory))
            {
              try {System.IO.Directory.CreateDirectory(CatalogDirectory);}
              catch {}
            }
            string CatalogName = Config.GetDirectoryInfo(Config.Dir.Config) + @"\thumbs\MyFilms\Catalog\" + Config_Name.Text + ".xml";
            if (!System.IO.File.Exists(CatalogName))
            {
              XmlTextWriter destXml = new XmlTextWriter(CatalogName, System.Text.Encoding.Default);
              destXml.Formatting = Formatting.Indented;
              destXml.WriteStartDocument();
              destXml.WriteStartElement("AntMovieCatalog");
              destXml.WriteStartElement("Catalog");
              destXml.WriteElementString("Properties", string.Empty);
              destXml.WriteStartElement("Contents");
              destXml.Close();
            }
            MesFilmsCat.Text = CatalogName;
            if (MesFilmsCat.Text.Length > 0)
              if (MesFilmsImg.Text.Length == 0)
                MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\"));
            newCatalog = true;
          }

          // Set values and Presets ...
          AntStorage.Text = "Source";
          cbPictureHandling.Text = "Relative Path"; // set option for picture path handling (grabber)
          AntStorageTrailer.Text = "Borrower";
          if (System.Windows.Forms.MessageBox.Show("Do you want to use OriginalTitle as Mastertitle ? \n(If you select no, TranslatedTitle will be used)", "MyFilms Configuration Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
          {
            AntTitle1.Text = "OriginalTitle";
            AntTitle2.Text = "TranslatedTitle";
            ItemSearchFileName.Text = "OriginalTitle";
            ItemSearchGrabberName.Text = "OriginalTitle";
            chkUseOriginalAsTranslatedTitle.Checked = false;
          }
          else
          {
            AntTitle1.Text = "TranslatedTitle";
            AntTitle2.Text = "OriginalTitle";
            ItemSearchFileName.Text = "TranslatedTitle";
            ItemSearchGrabberName.Text = "TranslatedTitle";
            chkUseOriginalAsTranslatedTitle.Checked = true;
          }

          AntSTitle.Text = "FormattedTitle";
          TitleDelim.Text = "\\";

          SearchFileName.Checked = false;

          cbPictureHandling.Text = "Relative Path";


          if (AntTitle1.Text == "TranslatedTitle")
          {
            AntSearchList.Text = "TranslatedTitle, OriginalTitle, Description, Comments, Actors, Director, Producer, Year, Date, Category, Country, Rating, Checked, MediaLabel, MediaType, URL, Borrower, Length, VideoFormat, VideoBitrate, AudioFormat, AudioBitrate, Resolution, Framerate, Size, Disks, Languages, Subtitles, Number";
            AntUpdList.Text = "TranslatedTitle, OriginalTitle, Category, Year, Date, Country, Rating, Checked, MediaLabel, MediaType, Actors, Director, Producer";
          }
          if (AntTitle1.Text == "OriginalTitle" || AntTitle1.Text == "FormattedTitle")
          {
            AntSearchList.Text = "OriginalTitle, TranslatedTitle, Description, Comments, Actors, Director, Producer, Year, Date, Category, Country, Rating, Checked, MediaLabel, MediaType, URL, Borrower, Length, VideoFormat, VideoBitrate, AudioFormat, AudioBitrate, Resolution, Framerate, Size, Disks, Languages, Subtitles, Number";
            AntUpdList.Text = "OriginalTitle, TranslatedTitle, Category, Year, Date, Country, Rating, Checked, MediaLabel, MediaType, Actors, Director, Producer";
          }

          // Preset separators:
          ListSeparator1.Text = ",";
          ListSeparator2.Text = ";";
          ListSeparator3.Text = "[";
          ListSeparator4.Text = "|";
          ListSeparator5.Text = string.Empty;
          RoleSeparator1.Text = "(";
          RoleSeparator2.Text = ")";
          RoleSeparator3.Text = " as ";
          RoleSeparator4.Text = "....";
          RoleSeparator5.Text = "|";

          chkFanart.Checked = true;
          chkFanartDefaultViews.Checked = false;
          chkDfltFanart.Checked = true;
          chkDfltFanartImage.Checked = true;
          chkDfltFanartImageAll.Checked = true;

          string FanartDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\thumbs\MyFilms\Fanart";
          if (!System.IO.Directory.Exists(FanartDirectory))
          {
            try
            {
              System.IO.Directory.CreateDirectory(FanartDirectory);
            }
            catch
            {
            }
          }
          MesFilmsFanart.Text = FanartDirectory;

          string ArtistImagesDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\thumbs\MyFilms\PersonImages";
          if (!System.IO.Directory.Exists(ArtistImagesDirectory))
          {
            try
            {
              System.IO.Directory.CreateDirectory(ArtistImagesDirectory);
            }
            catch
            {
            }
          }
          MesFilmsImgArtist.Text = ArtistImagesDirectory;
          DefaultCover.Text = Config.GetDirectoryInfo(Config.Dir.Config) + @"\thumbs\MyFilms\DefaultImages\DefaultCover.jpg";
          DefaultCoverArtist.Text = Config.GetDirectoryInfo(Config.Dir.Config) + @"\thumbs\MyFilms\DefaultImages\DefaultArtist.jpg";
          DefaultCoverViews.Text = Config.GetDirectoryInfo(Config.Dir.Config) + @"\thumbs\MyFilms\DefaultImages\DefaultGroup.jpg";
          DefaultFanartImage.Text = Config.GetDirectoryInfo(Config.Dir.Config) + @"\thumbs\MyFilms\DefaultImages\DefaultFanartImage.jpg";
          chkDfltArtist.Checked = true; // Use default person cover if missing artwork...

          string GroupViewImagesDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\thumbs\MyFilms\GroupViewImages";
          if (!System.IO.Directory.Exists(GroupViewImagesDirectory))
          {
            try
            {
              System.IO.Directory.CreateDirectory(GroupViewImagesDirectory);
              // Will be autocreated by GUI plugin
              //System.IO.Directory.CreateDirectory(GroupViewImagesDirectory + @"\category");
              //System.IO.Directory.CreateDirectory(GroupViewImagesDirectory + @"\country");
              //System.IO.Directory.CreateDirectory(GroupViewImagesDirectory + @"\year");
            }
            catch
            {
            }
          }

          MesFilmsViews.Text = GroupViewImagesDirectory;
          chkViews.Checked = true; // Use Thumbs for views
          chkPersons.Checked = false; // Don't use Thumbs for persons views
          chkDfltViews.Checked = true; // Use default cover for missing thumbs
          chkDfltViewsAll.Checked = false; // Use group view thumbs for all group views
          // Logos
          chkLogos.Checked = true;
          comboBoxLogoSpacing.Text = "2";
          comboBoxLogoPresets.Text = "Use Logos of currently selected skin";
          //GrabberConfig
          //txtGrabber.Text = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms\IMDB.xml";
          chkGrabber_ChooseScript.Checked = true; //Don't use default script (ask)

          AntItem1.Text = "Country";
          AntLabel1.Text = BaseMesFilms.Translate_Column(AntItem1.Text.Trim());
          AntItem2.Text = "Producer";
          AntLabel2.Text = BaseMesFilms.Translate_Column(AntItem2.Text.Trim());
          AntItem3.Text = "Category";
          AntLabel3.Text = BaseMesFilms.Translate_Column(AntItem3.Text.Trim());
          AntItem4.Text = "Languages";
          AntLabel4.Text = BaseMesFilms.Translate_Column(AntItem4.Text.Trim());
          AntItem5.Text = "Date";
          AntLabel5.Text = BaseMesFilms.Translate_Column(AntItem5.Text.Trim());

          AntViewItem1.Text = "Producer";
          AntViewText1.Text = BaseMesFilms.Translate_Column(AntViewItem1.Text.Trim());

          cbWatched.Text = "Checked";

          // Now ask user for his movie directory...
          if (newCatalogSelectedIndex == 0 || newCatalogSelectedIndex == 7)
          {
            MessageBox.Show("Now choose the folder containing your movies.", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string samplemovies =  Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\SampleMovies";
            if (System.IO.Directory.Exists(samplemovies))
              folderBrowserDialog1.SelectedPath = samplemovies;

            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
              if (this.folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") !=
                  this.folderBrowserDialog1.SelectedPath.Length - 1) folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";

              if (PathStorage.Text.Length == 0)
                PathStorage.Text = folderBrowserDialog1.SelectedPath;
              else PathStorage.Text = PathStorage.Text + ";" + folderBrowserDialog1.SelectedPath;
            }
            //MessageBox.Show("Successfully created a new Configuration ! You may now run AMCupdater to populate or update your catalog.", "MyFilms Configuration Wizard", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          }
          else
          {
            PathStorage.Text = "";
          }

          // Create country specific settings
          cbGrabberOverrideLanguage.Text = "";
          cbGrabberOverrideGetRoles.Text = "";
          cbGrabberOverridePersonLimit.Text = "";
          cbGrabberOverrideTitleLimit.Text = "";
          switch (newCountry)
          {
            case "Germany":
              txtGrabber.Text = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms\Cinefacts-Kino.xml";
              break;
            case "Canada":
              cbGrabberOverrideLanguage.Text = "Canada";
              cbGrabberOverrideGetRoles.Text = "true";
              cbGrabberOverridePersonLimit.Text = "10";
              cbGrabberOverrideTitleLimit.Text = "3";
              txtGrabber.Text = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms\IMDB.xml";
              break;
            default:
              txtGrabber.Text = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms\IMDB-Full.xml";
              break;
          //Argentina
          //Australia
          //Austria
          //Belgium
          //Brazil
          //Canada
          //Chile
          //Croatia
          //Czech Republic
          //Denmark
          //Estonia
          //Finland
          //France
          //Germany
          //Greece
          //Hong Kong
          //Hungary
          //Iceland
          //India
          //Ireland
          //Israel
          //Italy
          //Japan
          //Malaysia
          //Mexico
          //Netherlands
          //New Zealand
          //Norway
          //Peru
          //Philippines
          //Poland
          //Portugal
          //Romania
          //Russia
          //Singapore
          //Slovakia
          //Slovenia
          //South Africa
          //South Korea
          //Spain
          //Sweden
          //Switzerland
          //Turkey
          //UK
          //Uruguay
          //USA          
          }

          //AMCupdater
          chkAMCUpd.Checked = true; // Use AMCupdater
          AMCMovieScanPath.Text = PathStorage.Text;
          chkAMC_Purge_Missing_Files.Checked = false;
          txtPicturePrefix.Text = Config_Name.Text + "_"; // Use configname as default prefix
          txtAMCUpd_cnf.Text = Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings" + "_" + Config_Name.Text + ".xml";
          // Make controls visible:
          //txtAMCUpd_cnf.Enabled = true;
          //btnAMCUpd_cnf.Enabled = true;
          //scheduleAMCUpdater.Enabled = true;
          //btnParameters.Enabled = true;
          ////btnLaunchAMCupdater.Enabled = true;
          //btnAMCMovieScanPathAdd.Enabled = true;
          //chkAMC_Purge_Missing_Files.Enabled = true;
          //btnCreateAMCDefaultConfig.Enabled = true;
          //btnCreateAMCDesktopIcon.Enabled = true;
          //AMCMovieScanPath.Enabled = true;


          // Create config file for AMCupdater
          string wfiledefault = Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings";
          if (System.IO.File.Exists(wfiledefault + ".xml"))
          {
            if (System.IO.File.Exists(wfiledefault + "_" + Config_Name.Text + ".xml"))
            {
              System.IO.File.Copy(wfiledefault + "_" + Config_Name.Text + ".xml", wfiledefault + "_" + Config_Name.Text + ".xml.sav", true);
            }
            System.IO.File.Delete(wfiledefault + "_" + Config_Name.Text + ".xml");

            Read_XML_AMCconfig(Config_Name.Text); // read current (or create new default) config file
            CreateMyFilmsDefaultsForAMCconfig(Config_Name.Text); //create MF defaults
            Save_XML_AMCconfig(Config_Name.Text); // save new config
            Read_XML_AMCconfig(Config_Name.Text); // reread config file with new defaults
          }

          // Create Desktop Icon for AMCupdater with config created...
          CreateAMCDesktopIcon();

          // Change Config to selected catalog type
          CatalogType.SelectedIndex = newCatalogSelectedIndex;
          // Set as default config, if it's the only config - but this would always have this config set as default, even if adding more configs....
          //if (Config_Name.Items.Count == 1) 
          //  Config_Dflt.Checked = true;

          Save_Config();
          //Config_Name.Focus();
          switch (newCatalogSelectedIndex)
          {
            case 0:
              System.Windows.Forms.MessageBox.Show(
              "Successfully created a new Configuration with default settings ! \n\nPlease review your settings in MyFilms and AMC Updater to match your personal needs. \n You may run AMCupdater to populate or update your catalog. \nAMCUpdater will be autostarted, if you created an empty catalog.",
              "MyFilms Configuration Wizard - Finished !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
              break;
            default:
              System.Windows.Forms.MessageBox.Show(
                "Successfully created a new Configuration for '" + CatalogType.Text + "' with default settings ! \n\nPlease verify the settings to artwork pathes to match your personal needs.", "MyFilms Configuration Wizard - Finished !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
              General.SelectedIndex = 6;
              MesFilmsImg.Focus(); // Set focus to cover path                
              break;
          }
              
          if (newCatalog && CatalogType.SelectedIndex != 0)
          {
            launchAMCmanager();
          }
          
        }

        private void butNew_Click(object sender, EventArgs e)
        {
          MyFilmsInputBox input = new MyFilmsInputBox();
          input.CatalogTypeSelectedIndex = 0; // preset to ANT MC 
          input.CatalogType = "Ant Movie Catalog (V3.5.1.2)"; // preset to Ant Movie Catalog (V3.5.1.2)
          input.ShowDialog(this);
          string newConfig_Name = input.ConfigName;
          string newCatalogType = input.CatalogType;
          int newCatalogSelectedIndex = input.CatalogTypeSelectedIndex;

          if (string.IsNullOrEmpty(newConfig_Name))
          {
            System.Windows.Forms.MessageBox.Show("New Config Name must not be empty !", "MyFilms - New Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          }
          else
          {
            if (newConfig_Name == Config_Name.Text)
            {
              System.Windows.Forms.MessageBox.Show("New Config Name must be different from the existing one !", "MyFilms - New Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
              NewConfigButton = true;
              Refresh_Items(true); // Reset all
              Refresh_Tabs(true); // enable Tabs
              Config_Name.Text = newConfig_Name;
              //Config_Name_Load();
              System.Windows.Forms.MessageBox.Show("Created a new Configuration ! \n You must do proper setup to use it.", "MyFilms - New Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
              CatalogType.SelectedIndex = newCatalogSelectedIndex; // set selected CatalogType
              //Config_Name.Focus();
              MesFilmsCat.Focus(); // change focus away from config to initialize ...
              textBoxNBconfigs.Text = Config_Name.Items.Count.ToString();
              NewConfigButton = true;
            }
          }
        }

        private void chkAMC_Purge_Missing_Files_CheckedChanged(object sender, EventArgs e)
        {
          if (chkAMC_Purge_Missing_Files.Checked && !WizardActive && AMCGetAttribute("Purge_Missing_Files") != "true")
          {
            if (System.Windows.Forms.MessageBox.Show("Are you sure, you want to purge records from your DB \n where media files are not accessible during AMC Updater scans ?",
                "Control Configuration",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
              return;
            }
            else
            {
              chkAMC_Purge_Missing_Files.Checked = false;
            }
          }
        }

        private void txtLogosPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void ButDefCovViews_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DefaultCoverViews.Text))
              openFileDialog1.FileName = DefaultCoverViews.Text;
            else
            {
              openFileDialog1.FileName = String.Empty;
              openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\DefaultImages";
            }

            if (DefaultCoverViews.Text.Contains("\\"))
              openFileDialog1.InitialDirectory = DefaultCoverViews.Text.Substring(0, DefaultCoverViews.Text.LastIndexOf("\\") + 1);

          openFileDialog1.RestoreDirectory = true;
          openFileDialog1.DefaultExt = "jpg";
          openFileDialog1.Filter = "JPG Files|*.jpg|PNG Files|*.png|BMP Files|*.bmp|All Files|*.*";
          openFileDialog1.Title = "Select Default Group Views Cover Image";
          if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            DefaultCoverViews.Text = openFileDialog1.FileName;
        }

        private void ButDefFanart_Click(object sender, EventArgs e)
        {
          if (!string.IsNullOrEmpty(DefaultFanartImage.Text))
            openFileDialog1.FileName = DefaultFanartImage.Text;
          else
          {
            openFileDialog1.FileName = String.Empty;
            openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\DefaultImages";
          }

          if (DefaultFanartImage.Text.Contains("\\"))
            openFileDialog1.InitialDirectory = DefaultFanartImage.Text.Substring(0, DefaultFanartImage.Text.LastIndexOf("\\") + 1);

          openFileDialog1.RestoreDirectory = true;
          openFileDialog1.DefaultExt = "jpg";
          openFileDialog1.Filter = "JPG Files|*.jpg|PNG Files|*.png|BMP Files|*.bmp|All Files|*.*";
          openFileDialog1.Title = "Select Default Fanart Image";
          if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            DefaultFanartImage.Text = openFileDialog1.FileName;
        }

        private void CheckWatched_CheckedChanged(object sender, EventArgs e)
        {
          if (CheckWatched.Checked) 
            CheckWatchedPlayerStopped.Checked = false;
        }

        private void CheckWatchedPlayerStopped_CheckedChanged(object sender, EventArgs e)
        {
          if (CheckWatchedPlayerStopped.Checked) 
            CheckWatched.Checked = false;
        }

        private void chkDfltFanartImage_CheckedChanged(object sender, EventArgs e)
        {
          if (chkDfltFanartImage.Checked)
          {
            chkDfltFanart.Checked = false;
            chkDfltFanartImageAll.Enabled = true;
          }
          else
          {
            chkDfltFanartImageAll.Enabled = false;
          }
        }

        private void chkDfltFanart_CheckedChanged(object sender, EventArgs e)
        {
          if (chkDfltFanart.Checked)
            chkDfltFanartImage.Checked = false;
        }

        private void btnGrabberInterface_Click(object sender, EventArgs e)
        {
          using (Process p = new Process())
          {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Config.GetDirectoryInfo(Config.Dir.Base) + @"\MyFilms_Grabber_Interface.exe";
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            //psi.Arguments = "\"" + Config.GetDirectoryInfo(Config.Dir.Config) + @"\MyFilmsAMCSettings_" + Config_Name.Text + ".xml" + "\"" + " " + "LogDirectory" + " " + "GUI";
            //psi.Arguments = " \"" + Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings_" + Config_Name.Text + "\" \"" + Config.GetDirectoryInfo(Config.Dir.Log).ToString() + "\" \"GUI\"";
            psi.ErrorDialog = true;
            if (OSInfo.OSInfo.VistaOrLater())
            {
              psi.Verb = "runas";
            }

            p.StartInfo = psi;
            LogMyFilms.Debug("MyFilmsSetup: Launch Grabber_Interface from PluginSetup");
            try
            {
              p.Start();
              //p.WaitForExit();
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug(ex.ToString());
            }
            LogMyFilms.Debug("MyFilmsSetup: Launch Grabber_Interface from PluginSetup done");
          }
        }

        private void cbPictureHandling_SelectedIndexChanged(object sender, EventArgs e)
        {
          // options:
          // cbPictureHandling = "Full Path"
          // cbPictureHandling = "Relative Path"
          // cbPictureHandling = "Use Folder.jpg"
        }

        private void btnEditScript_Click(object sender, EventArgs e)
        {
          using (Process p = new Process())
          {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Config.GetDirectoryInfo(Config.Dir.Base) + @"\MyFilms_Grabber_Interface.exe";
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.Arguments = "\"" + txtGrabber.Text + "\"";
            psi.ErrorDialog = true;
            if (OSInfo.OSInfo.VistaOrLater())
            {
              psi.Verb = "runas";
            }

            p.StartInfo = psi;
            LogMyFilms.Debug("MyFilmsSetup: Launch Grabber_Interface from PluginSetup with command param '" + psi.Arguments.ToString() + "'");
            try
            {
              p.Start();
              //p.WaitForExit();
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug(ex.ToString());
            }
            LogMyFilms.Debug("MyFilmsSetup: Launch Grabber_Interface from PluginSetup done");
          }

        }

        private void buttonDeleteTmpCatalog_Click(object sender, EventArgs e)
        {
          string destFile = string.Empty;
          switch (CatalogType.SelectedIndex)
          {
            case 0: // ANT Movie Catalog
            case 10:// Starter Settings ANT extnded DB
              break;
            case 1: // DVD Profiler
            case 2: // Movie Collector V7.1.4
            case 3: // MyMovies
            case 4: // EAX Movie Catalog 2.5.0
            case 5: // EAX Movie Catalog 3.0.9 (beta5)
            case 6: // PVD PersonalVideoDatabase V0.9.9.21
            case 7: // eXtreme Movie Manager
            case 8: // XBMC fulldb export (all movies in one DB)
            case 9: // MovingPicturesXML V1.2
              destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
              break;
            case 11: // XBMC Nfo (separate nfo files, to scan dirs - MovingPictures or XBMC)
              destFile = MesFilmsCat.Text;
              break;
            default:
              break;
          }
          if (destFile.Length > 0)
            if (System.IO.File.Exists(destFile))
            {
              DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the tmp cache file for this config? \n\n(File will be recreated on next 'save' action.)", "Warning", MessageBoxButtons.YesNo,
              MessageBoxIcon.Question);
              if (dialogResult == DialogResult.Yes)
              {
                System.IO.File.Delete(destFile);
                LogMyFilms.Debug("MyFilmsSetup: Manually deleted tmp catalog: '" + destFile + "'");
                MessageBox.Show("Deleted tmp imported data: '" + destFile + "' \n MyFilms will reimport data on next launch or when saving the config.", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
              }
            }
        }

        private void buttonOpenTmpFile_Click(object sender, EventArgs e)
        {
          string destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
          if (!System.IO.File.Exists(destFile))
          {
            MessageBox.Show("The file '" + destFile + "' does not exist!", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
          }
          using (Process p = new Process())
          {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "notepad.exe";
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.Arguments = "\"" + MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml" + "\"";
            psi.ErrorDialog = true;
            if (OSInfo.OSInfo.VistaOrLater())
            {
              psi.Verb = "runas";
            }

            p.StartInfo = psi;
            try
            {
              p.Start();
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug(ex.ToString());
            }
          }

        }

        private void buttonOpenTmpFileAMC_Click(object sender, EventArgs e)
        {
          string destFile = string.Empty;
          if (CatalogType.SelectedIndex == 0)
            destFile = MesFilmsCat.Text;
          else
            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
          if (!System.IO.File.Exists(destFile))
          {
            MessageBox.Show("The file '" + destFile + "' does not exist!", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
          }
          if (string.IsNullOrEmpty(AMCexePath.Text))
          {
            MessageBox.Show("Unable to launch Ant Movie Catalog ! \nPlease set the path accordingly first!", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
          }
          if (CatalogType.SelectedIndex != 0) // show warning when launching AMC with EC tmp file !
          {
            MessageBox.Show("Info: You are launching AMC with a temporary cache catalog file !\nPlease be aware, that changes in MC will be overwritten on next export of your external catalog !", "AMC Launcher Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
          }
          //System.Diagnostics.Process.Start(AMCexePath.Text); // More detailed startup below ...
          using (Process p = new Process())
          {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = AMCexePath.Text;
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.Arguments = "\"" + destFile + "\"";
            psi.ErrorDialog = true;
            if (OSInfo.OSInfo.VistaOrLater())
            {
              psi.Verb = "runas";
            }

            p.StartInfo = psi;
            try
            {
              p.Start();
            }
            catch (Exception ex)
            {
              LogMyFilms.Debug(ex.ToString());
            }
          }
        }

        private void buttonAMCpathSearch_Click(object sender, EventArgs e)
        {
          if (!string.IsNullOrEmpty(AMCexePath.Text))
            openFileDialog1.FileName = AMCexePath.Text;
          else
          {
            openFileDialog1.FileName = String.Empty;
            openFileDialog1.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
          }
          openFileDialog1.RestoreDirectory = true;
          openFileDialog1.DefaultExt = "exe";
          openFileDialog1.Filter = "exe Files|*.exe";
          openFileDialog1.Title = "Select AMC Movie Catalog Executable (.exe)";
          openFileDialog1.CheckFileExists = false;
          if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
          {
            AMCexePath.Text = openFileDialog1.FileName;
          }
        }

        private void txtGrabber_TextChanged(object sender, EventArgs e)
        {
          if (!string.IsNullOrEmpty(txtGrabber.Text))
            txtGrabberDisplay.Text = Path.GetFileName(txtGrabber.Text);
          else 
            txtGrabberDisplay.Text = string.Empty;
        }
    }
}
