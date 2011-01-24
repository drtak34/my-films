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
  using System.Net;
  using System.Windows.Forms;
  using System.Xml;

  using IWshRuntimeLibrary;

  using MediaPortal.Configuration;

  using MyFilmsPlugin.MyFilms.CatalogConverter;
  using MyFilmsPlugin.MyFilms.Utils;

  using TaskScheduler;

  public partial class MesFilmsSetup : Form
    {
        //private WshShellClass WshShell; // Added for creating Desktop icon via wsh

        //fmu   private MediaPortal.Profile.Settings MyFilms_xmlwriter = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"));
        //fmu   private MediaPortal.Profile.Settings MyFilms_xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MyFilms.xml"));
        XmlConfig XmlConfig = new XmlConfig(); //fmu

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
        public MesFilmsSetup()
        {
            InitializeComponent();
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            this.label_VersionNumber.Text = "Version " + asm.GetName().Version.ToString() + " alpha";
        }


        private void MesFilmsSetup_Load(object sender, EventArgs e)
        {
            Refresh_Items(true);

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
            AntFilterMinRating.Items.Add("(none)");
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
            CmdPar.Items.Add("(none)");
            CatalogType.SelectedIndex = 0;
            foreach (DataColumn dc in ds.Movie.Columns)
            {
                if ((dc.ColumnName == "MediaLabel") || (dc.ColumnName == "MediaType") || (dc.ColumnName == "Source") || (dc.ColumnName == "URL") || (dc.ColumnName == "Comments") || (dc.ColumnName == "Borrower") || (dc.ColumnName == "Languages") || (dc.ColumnName == "Subtitles"))
                {
                    AntStorage.Items.Add(dc.ColumnName);
                    AntStorageTrailer.Items.Add(dc.ColumnName);
                    DVDPTagField.Items.Add(dc.ColumnName);
                    AntSTitle.Items.Add(dc.ColumnName);
                }
                AntIdentItem.Items.Add(dc.ColumnName);
                AntTitle2.Items.Add(dc.ColumnName);
                if ((dc.ColumnName != "Contents_Id") && (dc.ColumnName != "Length_Num"))
                {
                  AntFilterItem1.Items.Add(dc.ColumnName);
                  AntFilterItem2.Items.Add(dc.ColumnName);
                  AntItem1.Items.Add(dc.ColumnName);
                  AntItem2.Items.Add(dc.ColumnName);
                  AntItem3.Items.Add(dc.ColumnName);
                }
                cbfdupdate.Items.Add(dc.ColumnName);
                CmdPar.Items.Add(dc.ColumnName);
                if ((dc.ColumnName != "Contents_Id") && (dc.ColumnName != "DateAdded") && (dc.ColumnName != "Length_Num"))
                {
                    AntSearchField.Items.Add(dc.ColumnName);
                    AntUpdField.Items.Add(dc.ColumnName);
                }
                if ((dc.ColumnName != "Contents_Id") && (dc.ColumnName != "Number") && (dc.ColumnName != "OriginalTitle") && (dc.ColumnName != "TranslatedTitle") && (dc.ColumnName != "Comments") && (dc.ColumnName != "Description") && (dc.ColumnName != "FormattedTitle") && (dc.ColumnName != "Date") && (dc.ColumnName != "DateAdded") && (dc.ColumnName != "Rating") && (dc.ColumnName != "Size") && (dc.ColumnName != "Picture") && (dc.ColumnName != "URL"))
                {
                    SField1.Items.Add(dc.ColumnName);
                    SField2.Items.Add(dc.ColumnName);
                }
                if ((dc.ColumnName != "Contents_Id") && dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" && dc.ColumnName != "Description" && dc.ColumnName != "Comments")
                {
                    AntViewItem1.Items.Add(dc.ColumnName);
                    AntViewItem2.Items.Add(dc.ColumnName);
                    AntViewItem3.Items.Add(dc.ColumnName);
                    AntViewItem4.Items.Add(dc.ColumnName);
                    AntViewItem5.Items.Add(dc.ColumnName);
                }
                if ((dc.ColumnName != "Contents_Id") && dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" && dc.ColumnName != "Actors")
                {
                    AntSearchItem1.Items.Add(dc.ColumnName);
                    AntSearchItem2.Items.Add(dc.ColumnName);
                }
                if ((dc.ColumnName == "TranslatedTitle") || (dc.ColumnName == "OriginalTitle") || (dc.ColumnName == "FormattedTitle"))
                    AntSTitle.Items.Add(dc.ColumnName);
                if ((dc.ColumnName != "Contents_Id") && dc.ColumnName != "TranslatedTitle" && dc.ColumnName != "OriginalTitle" && dc.ColumnName != "FormattedTitle" && dc.ColumnName != "Year" && dc.ColumnName != "Rating" && dc.ColumnName != "DateAdded" && dc.ColumnName != "Date")
                {
                    AntSort1.Items.Add(dc.ColumnName);
                    AntSort2.Items.Add(dc.ColumnName);
                }
                AntUpdItem1.Items.Add(dc.ColumnName);
                AntUpdItem2.Items.Add(dc.ColumnName);
            }
            AntViewText_Change();
            AntSort_Change();
            Config_Name.Text = XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", string.Empty);
            chkLogos.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text, "Logos", true); // Changed default to "true" - hope, it works even without fiull working config ....
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
        }
        private void ButCat_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "xml";
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Find AMC Movies file (xml file)";
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
                if (CatalogType.Text == "Ant Movie Catalog" || CatalogType.Text == "Xbmc NFO")
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
                        System.Windows.Forms.MessageBox.Show("You have to done a valid file !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        MesFilmsCat.Focus();
                        return;
                    }
                }
                else
                {

                    System.Windows.Forms.MessageBox.Show("You have to done a valid file !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                MesFilmsImg.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        // Display any warnings or errors.

        private void ButSave_Click(object sender, EventArgs e)
        {
            Save_Config();
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
            if (textBoxPluginName.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("The Plugin's Name is Mandatory !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                textBoxPluginName.Focus();
                return;
            }
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
            if (CatalogType.SelectedIndex == 1)
                if ((radioButton2.Checked) && (DVDPTagField.Text.Length == 0))
                {
                    System.Windows.Forms.MessageBox.Show("Field Name is Mandatory for storing Tags Informations !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    DVDPTagField.Focus();
                    return;
                }
            if (chkGrabber.Checked)
                if (txtGrabber.Text.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Grabber Config File Name is Mandatory for detail Internet Update function !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtGrabber.Focus();
                    return;
                }
            if (chkAMCUpd.Checked)
            {
                if (txtAMCUpd_exe.Text.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Path to AMCUpdater program is Mandatory for detail Internet Update function !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtAMCUpd_exe.Focus();
                    return;
                }
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
            if (MesFilmsImg.Text.Length > 0 || MesFilmsFanart.Text.Length > 0)
            {
                if (MesFilmsImg.Text == MesFilmsFanart.Text)
                {
                    System.Windows.Forms.MessageBox.Show("Picture and Fanart Path can't be the same !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    MesFilmsFanart.Focus();
                    return;
                }
            }
            if ((chkFanart.Checked) && (MesFilmsFanart.Text.Length == 0))
            {
                System.Windows.Forms.MessageBox.Show("Fanart Path must be fill for using !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                MesFilmsFanart.Focus();
                return;
            }
            Selected_Enreg_TextChanged();
            if (View_Dflt_Item.Text.Length == 0)
                View_Dflt_Item.Text = "(none)";
            Verify_Config();
            int WLayOut = 0;
            if (LayOut.Text == "List")
                WLayOut = 0;
            if (LayOut.Text == "Small Icons")
                WLayOut = 1;
            if (LayOut.Text == "Large Icons")
                WLayOut = 2;
            if (LayOut.Text == "Filmstrip")
                WLayOut = 3;
            if (LayOut.Text == "Cover Flow")
                WLayOut = 4;
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
            // backup yhe XML Config bfore writing
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
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterMinRating", AntFilterMinRating.Text.ToString());
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
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel1", AntLabel1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem1", AntItem1.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel2", AntLabel2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem2", AntItem2.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel3", AntLabel3.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem3", AntItem3.Text.ToString());
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
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchFileName", SearchFileName.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchSubDirs", SearchSubDirs.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SearchSubDirsTrailer", SearchSubDirsTrailer.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "CheckWatched", CheckWatched.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AlwaysDefaultView", AlwaysDefaultView.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "GlobalUnwatchedOnly", chkGlobalUnwatchedOnly.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "GlobalUnwatchedOnlyValue", textBoxGlobalUnwatchedOnlyValue.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "OnlyTitleList", chkOnlyTitle.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "WindowsFileDialog", chkWindowsFileDialog.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ItemSearchFileName", ItemSearchFileName.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultCover", DefaultCover.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultCoverArtist", DefaultCoverArtist.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "CmdExe", CmdExe.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "CmdPar", CmdPar.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber", chkGrabber.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_Dir", txtDirGrab.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_cnf", txtGrabber.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_Always", chkGrabber_Always.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_ChooseScript", chkGrabber_ChooseScript.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd", chkAMCUpd.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd_exe", txtAMCUpd_exe.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd_cnf", txtAMCUpd_cnf.Text);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Fanart", chkFanart.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartDflt", chkDfltFanart.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartPicture", MesFilmsFanart.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewsPicture", MesFilmsViews.Text.ToString());
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Views", chkViews.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewsDflt", chkDfltViews.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "LastID", "7986");
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "Suppress", chkSuppress.Checked);
            XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressPlayed", chksupplaystop.Checked);
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

            if (CatalogType.SelectedIndex == 1)
            {
                XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "SortTitle", SortTitle.Checked);
                XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "OnlyFile", OnlyFile.Checked);
                if (radioButton1.Checked)
                    XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "DVDPTagField", "Category");
                else
                    if (radioButton2.Checked)
                        XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "DVDPTagField", DVDPTagField.Text);
                    else
                        XmlConfig.RemoveEntry("MyFilms", Config_Name.Text.ToString(), "DVDPTagField");
            }
            if (Thumbnails.Checked)
                XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "MCCovers", "Thumbnails");
            else
                XmlConfig.WriteXmlConfig("MyFilms", Config_Name.Text.ToString(), "MCCovers", "Images");
            string w_Config_Name = Config_Name.Text;
            Config_Name.Items.Remove(Config_Name.Text);
            Config_Name.Items.Add(w_Config_Name);
            Config_Name.Text = w_Config_Name;
            XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "NbConfig", Config_Name.Items.Count);
            if (chkLogos.Checked) // Save Logoconfig ...
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
                string logoPath;
                //using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.MPSettings())
                //{
                //  logoPath = MediaPortal.Configuration.Config.GetDirectoryInfo(Config.Dir.Skin) + @"\" + xmlreader.GetValueAsString("skin", "name", "NoSkin") + @"\Media\Logos"; // Get current path to logos in skindirectory
                //}
                //if (txtLogosPath.Text.Length > 0)
                logoPath = txtLogosPath.Text;
                if (comboBoxLogoPresets.Text == "Use Logos of currently selected skin")
                  logoPath = "";
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
                    if ((LogoView.Items[i].SubItems[9].Text.Length > 0) && (logoPath != ""))
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
            //if (chkAMCUpd.Checked)
            //{
            //  Save_XML_AMCconfig(currentconfig);
            //}
            System.Windows.Forms.MessageBox.Show("Configuration saved !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void butPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (this.folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") != this.folderBrowserDialog1.SelectedPath.Length - 1)
                    folderBrowserDialog1.SelectedPath = folderBrowserDialog1.SelectedPath + "\\";

                if (PathStorage.Text.Length == 0)
                    PathStorage.Text = folderBrowserDialog1.SelectedPath;
                else
                    PathStorage.Text = PathStorage.Text + ";" + folderBrowserDialog1.SelectedPath;
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
            Refresh_Items(false);
            CatalogType.SelectedIndex = Convert.ToInt16(XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "CatalogType", "0"));
            MesFilmsCat.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntCatalog", "");
            MesFilmsImg.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntPicture", "");
            MesFilmsImgArtist.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ArtistPicturePath", "");
            chkDfltArtist.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ArtistDflt", false);
            MesFilmsFanart.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartPicture", "");
            MesFilmsFanartViews.Text = MesFilmsFanart.Text + "\\_Group\\"; 
            MesFilmsViews.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewsPicture", "");
            chkDfltViews.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ViewsDflt", false);
            chkDfltArtist.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ArtistDflt", false);
            chkViews.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Views", false);
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
            AntFilterMinRating.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntFilterMinRating", "0");
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

            AntLabel1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel1", string.Empty);
            AntItem1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem1", string.Empty);
            AntLabel2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel2", string.Empty);
            AntItem2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem2", string.Empty);
            AntLabel3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntLabel3", string.Empty);
            AntItem3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AntItem3", string.Empty);
            ListSeparator1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator1", ",");
            ListSeparator2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator2", ";");
            ListSeparator3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator3", "[");
            ListSeparator4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator4", string.Empty);
            ListSeparator5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ListSeparator5", string.Empty);
            RoleSeparator1.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator1", "(");
            RoleSeparator2.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator2", ")");
            RoleSeparator3.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator3", " as ");
            RoleSeparator4.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator4", "....");
            RoleSeparator5.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "RoleSeparator5", string.Empty);
            CmdPar.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "CmdPar", "(none)");
            CmdExe.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "CmdExe", string.Empty);
            TitleDelim.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "TitleDelim", "\\");
            chkGrabber.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber", false);
            chkDfltFanart.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "FanartDflt", false);
            chkFanart.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Fanart", false);
            txtGrabber.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_cnf", string.Empty);
            txtDirGrab.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_Dir", Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\scripts\myfilms");
            chkGrabber_Always.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_Always", false);
            chkAMCUpd.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd", false);
            txtAMCUpd_exe.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd_exe", Config.GetDirectoryInfo(Config.Dir.Base).ToString() + @"\AMCupdater.exe"); 
            chkGrabber_ChooseScript.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Grabber_ChooseScript", false);
            txtAMCUpd_cnf.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AMCUpd_cnf", string.Empty);
            chkSuppress.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "Suppress", false);
            chksupplaystop.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SuppressPlayed", false);
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

            radioButton1.Checked = false;
            radioButton2.Checked = false;
            CheckWatched.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "CheckWatched", false);
            AlwaysDefaultView.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "AlwaysDefaultView", false);
            chkGlobalUnwatchedOnly.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "GlobalUnwatchedOnly", false);
            textBoxGlobalUnwatchedOnlyValue.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "GlobalUnwatchedOnlyValue", "false");
            chkOnlyTitle.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "OnlyTitleList", false);
            chkWindowsFileDialog.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "WindowsFileDialog", false);
            DVDPTagField.ResetText();
            if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "DVDPTagField", "") == "Category")
                radioButton1.Checked = true;
            else
                if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "DVDPTagField", "").Length > 0)
                {
                    radioButton2.Checked = true;
                    DVDPTagField.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "DVDPTagField", "");
                }
            if (XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "MCCovers", "") == "Images")
                Images.Checked = true;
            else
                Thumbnails.Checked = true;
            //if (MyFilms_XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SortTitle", "False") == "True")
            //    SortTitle.Checked = true;
            //else
            //    SortTitle.Checked = false;
            SortTitle.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "SortTitle", false);
            OnlyFile.Checked = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "OnlyFile", false);
            ItemSearchFileName.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "ItemSearchFileName", "");
            DefaultCover.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultCover", "");
            DefaultCoverArtist.Text = XmlConfig.ReadXmlConfig("MyFilms", Config_Name.Text.ToString(), "DefaultCoverArtist", "");
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
            if (WLayOut == 4)
                LayOut.Text = "Cover Flow";
            AntViewText_Change();
            AntSort_Change();
            
            if (Config_Name.Text.Length > 0) 
              chkLogos.Enabled = true;
            else 
              chkLogos.Enabled = false;

            if (PathStorage.Text.Length > 0 && AMCMovieScanPath.Text.Length == 0)
              AMCMovieScanPath.Text = PathStorage.Text;
            else
              AMCMovieScanPath.Text = String.Empty;
            if (AMCMovieScanPath.Text.Length == 0)
              btnCreateAMCDefaultConfig.Visible = true;
            else
              btnCreateAMCDefaultConfig.Visible = false;

            // Added by Guzzi to initialize the AMCupdater Default configuration and create default configfiles, if necessary.
            Read_XML_AMCconfig(Config_Name.Text); // To be enabled after proper testing !!!
          //Save_XML_AMCconfig(Config_Name.Text);
        }
        private void Refresh_Items(bool all)
        {
            CatalogType.SelectedIndex = 0;
            StrDfltSelect = "";
            //mydivx.Clear();
            View_Dflt_Item.Items.Clear();
            View_Dflt_Item.Items.Add("(none)");
            View_Dflt_Item.Items.Add(GUILocalizeStrings.Get(342)); // Filmy
            View_Dflt_Item.Items.Add("Year");
            View_Dflt_Item.Items.Add("Category");
            View_Dflt_Item.Items.Add("Country");
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
            AntFilterMinRating.ResetText();
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
            AntItem1.ResetText();
            AntItem2.ResetText();
            AntItem3.ResetText();
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
            Dwp.ResetText();
            Rpt_Dwp.ResetText();
            CmdExe.ResetText();
            CmdPar.ResetText();
            ItemSearchFileName.ResetText();
            SearchFileName.Checked = false;
            chkSuppress.Checked = false;
            chksupplaystop.Checked = false;
            cbfdupdate.ResetText();
            txtfdupdate.ResetText();
            rbsuppress1.Checked = true;
            SearchSubDirs.Checked = false;
            SearchSubDirsTrailer.Checked = false;
            check_WOL_enable.Checked = false;
            comboWOLtimeout.ResetText();
            check_WOL_Userdialog.Checked = false;
            CheckWatched.Checked = false;
            NAS_Name_1.ResetText();
            NAS_Name_2.ResetText();
            NAS_Name_3.ResetText();
            NAS_MAC_1.ResetText();
            NAS_MAC_2.ResetText();
            NAS_MAC_3.ResetText();
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            Thumbnails.Checked = true;
            DVDPTagField.ResetText();
            SortTitle.Checked = false;
            OnlyFile.Checked = false;
            AlwaysDefaultView.Checked = false;
            chkGlobalUnwatchedOnly.Checked = false;
            textBoxGlobalUnwatchedOnlyValue.Text = "false";
            chkOnlyTitle.Checked = false;
            chkWindowsFileDialog.Checked = false;
            chkGrabber.Checked = false;
            txtGrabber.ResetText();
            chkGrabber_Always.Checked = false;
            chkGrabber_ChooseScript.Checked = false;
            chkAMCUpd.Checked = false;
            txtAMCUpd_cnf.ResetText();
            txtAMCUpd_exe.ResetText();
            chkDfltFanart.Checked = false;
            chkFanart.Checked = false;
            chkDfltViews.Checked = false;
            chkDfltArtist.Checked = false;
            chkViews.Checked = false;
            chkAMC_Purge_Missing_Files.Checked = false;
            AMCMovieScanPath.ResetText();
            btnLaunchAMCupdater.Enabled = false;
            btnCreateAMCDesktopIcon.Enabled = false;
            btnCreateAMCDefaultConfig.Enabled = false;
        }

        private void ButDelet_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this configuration?", "Information", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Remove_Config();
                if ((Config_Name.Text) == XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Default_Config", ""))
                    XmlConfig.RemoveEntry("MyFilms", "MyFilms", "Default_Config");
                if ((Config_Name.Text) == XmlConfig.ReadXmlConfig("MyFilms", "MyFilms", "Current_Config", ""))
                    XmlConfig.RemoveEntry("MyFilms", "MyFilms", "Current_Config");
                Config_Name.Items.Remove(Config_Name.Text);
                Refresh_Items(true);
                Config_Name.ResetText();
            }
        }

        private void ButCopy_Click(object sender, EventArgs e)
        {
            MesFilmsInputBox input = new MesFilmsInputBox();
            input.ShowDialog(this);
            string newConfig_Name = input.UserName;
            if (newConfig_Name == Config_Name.Text)
            {
                System.Windows.Forms.MessageBox.Show("New Config Name must be different from the existing one !", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                Config_Name.Text = newConfig_Name;
                System.Windows.Forms.MessageBox.Show("Created a copy of current Configuration !", "Control Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Save_Config();
                Config_Name.Focus();
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
                    XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "PluginName", textBoxPluginName.Text.ToString());
                    XmlConfig.WriteXmlConfig("MyFilms", "MyFilms", "Default_Config", "");
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
        private void Verify_Config()
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
                            System.Windows.Forms.MessageBox.Show("Your XML file is valid with " + mydivx.Movie.Count + " Movies in your database and " + movies.Length + " Movies to display with your 'Selected Enreg' configuration", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            System.Windows.Forms.MessageBox.Show("Your XML file is valid with 0 Movie in your database but no Movie to display, you have to change the selected enregs or fill your database with AMCUpdater, AMC or your compatible Software", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    else
                    {
                        if (System.Windows.Forms.MessageBox.Show("There is no Movie to display with that file ! Do you Want to continue ?", "Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.No)
                        {
                            MesFilmsCat.Focus();
                            return;
                        }
                        System.Windows.Forms.MessageBox.Show("You have to fill your database with AMCUpdater, AMC or your compatible Software", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please give the XML file's name first");
                MesFilmsCat.Focus();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton2.Checked = false;
                DVDPTagField.ResetText();
            }
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
            LogMyFilms.Debug("MyFilms (Buid Selected Enreg) - Selected_Enreg: '" + Selected_Enreg.Text.ToString() + "'");
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton1.Checked = false;
            }
        }
        private void Selected_Enreg_Changed(object sender, EventArgs e)
        {
            Selected_Enreg_TextChanged();
        }


        private void CatalogType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (CatalogType.SelectedIndex)
            {
                case 0:
                    break;
                case 4:
                    AntStorage.Text = "Source";
                    AntStorageTrailer.Text = "Borrower";
                    AntTitle1.Text = "TranslatedTitle";
                    AntTitle2.Text = "OriginalTitle";
                    AntSTitle.Text = "FormattedTitle";
                    TitleDelim.Text = "\\";
                    ItemSearchFileName.Text = "TranslatedTitle";
                    if (MesFilmsCat.Text.Length > 0)
                        if (MesFilmsImg.Text.Length == 0)
                            MesFilmsImg.Text = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.LastIndexOf("\\")) + "\\Pictures";
                    SearchFileName.Checked = true;
                    break;

                case 5: // XMM
                    AntStorage.Text = "Source";
                    AntTitle1.Text = "TranslatedTitle";
                    AntTitle2.Text = "OriginalTitle";
                    AntSTitle.Text = "FormattedTitle";
                    TitleDelim.Text = "\\";
                    ItemSearchFileName.Text = "TranslatedTitle";
                    SearchFileName.Checked = true;
                    break;

                default:
                    AntStorage.Text = "URL";
                    AntStorageTrailer.Text = "Borrower";
                    AntTitle1.Text = "TranslatedTitle";
                    AntTitle2.Text = "OriginalTitle";
                    AntSTitle.Text = "FormattedTitle";
                    TitleDelim.Text = "\\";
                    ItemSearchFileName.Text = "TranslatedTitle";
                    SearchFileName.Checked = true;
                    break;

            }
        }

        private void btnGrabber_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "xml";
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Find Grabber Config file (xml file)";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                txtGrabber.Text = openFileDialog1.FileName;
        }

        private void btnAMCUpd_exe_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "AMCUpdater.exe";
            openFileDialog1.Filter = "exe Files|AMCUpdater.exe";
            openFileDialog1.Title = "Find AMCUpdater program";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                txtAMCUpd_exe.Text = openFileDialog1.FileName;
        }

        private void btnAMCUpd_cnf_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "xml";
            openFileDialog1.Filter = "XML AMCUpdater Config Files|*.xml";
            openFileDialog1.Title = "Find AMCUPdater Config file (xml file)";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                txtAMCUpd_cnf.Text = openFileDialog1.FileName;

        }

        private void chkAMCUpd_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAMCUpd.Checked)
            {
                txtAMCUpd_exe.Enabled = true;
                btnAMCUpd_exe.Enabled = true;
                txtAMCUpd_cnf.Enabled = true;
                btnAMCUpd_cnf.Enabled = true;
                scheduleAMCUpdater.Enabled = true;
                btnParameters.Enabled = true;
                //btnLaunchAMCupdater.Enabled = true;
                btnAMCMovieScanPathAdd.Enabled = true;
                chkAMC_Purge_Missing_Files.Enabled = true;
                btnCreateAMCDefaultConfig.Enabled = true;
                btnCreateAMCDesktopIcon.Enabled = true;
                AMCMovieScanPath.Enabled = true;
            }
            else
            {
                txtAMCUpd_exe.Enabled = false;
                btnAMCUpd_exe.Enabled = false;
                txtAMCUpd_cnf.Enabled = false;
                btnAMCUpd_cnf.Enabled = false;
                scheduleAMCUpdater.Enabled = false;
                btnParameters.Enabled = false;
                //btnLaunchAMCupdater.Enabled = false;
                btnAMCMovieScanPathAdd.Enabled = false;
                chkAMC_Purge_Missing_Files.Enabled = false;
                btnCreateAMCDefaultConfig.Enabled = false;
                btnCreateAMCDesktopIcon.Enabled = false;
                AMCMovieScanPath.Enabled = false;
            }
        }

        private void chkGrabber_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGrabber.Checked)
            {
                btnGrabber.Enabled = true;
                txtGrabber.Enabled = true;
                btnDirGrab.Enabled = true;
                txtDirGrab.Enabled = true;
                chkGrabber_Always.Enabled = true;
                chkGrabber_ChooseScript.Enabled = true;
            }
            else
            {
                btnGrabber.Enabled = false;
                txtGrabber.Enabled = false;
                btnDirGrab.Enabled = false;
                txtDirGrab.Enabled = false;
                chkGrabber_Always.Enabled = false;
                chkGrabber_ChooseScript.Enabled = false;
            }
        }

        private void btnFanart_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                MesFilmsFanart.Text = folderBrowserDialog1.SelectedPath;
                MesFilmsFanartViews.Text = MesFilmsFanart.Text + "\\_Group\\";
            }
        }

        private void chkFanart_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFanart.Checked)
            {
                MesFilmsFanart.Enabled = true;
                btnFanart.Enabled = true;
                chkDfltFanart.Enabled = true;
            }
            else
            {
                MesFilmsFanart.Enabled = false;
                btnFanart.Enabled = false;
                chkDfltFanart.Enabled = false;
            }
        }


        private void pictureBox_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "png";
            openFileDialog1.Filter = "PNG Files|*.png";
            openFileDialog1.Title = "Find Logos files (png file)";
            if (SFilePicture.Text.Length > 0)
                openFileDialog1.FileName = SFilePicture.Text;
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
                Edit_Item(selected_Logo_Item);
                selected_Logo_Item = -1;
            }
            else
            {
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
            }
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
            string wfile = string.Empty;
            if (LogoView.Items[select_item].SubItems[9].Text.Length > 0)
                wfile = LogoView.Items[select_item].SubItems[9].Text + "\\" + LogoView.Items[select_item].SubItems[8].Text;
            else
                wfile = LogoView.Items[select_item].SubItems[8].Text;
            SFilePicture.Text = wfile;
            if (System.IO.File.Exists(wfile))
                SPicture.BackgroundImage = ImageFast.FastFromFile(SFilePicture.Text);
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
            if (mydivx.Contents.Count == 0)
            {
                mydivx.Clear();
                mydivx = ReadXml();
                LogoView.Items.Clear();
                selected_Logo_Item = -1;
                Read_XML_Logos(Config_Name.Text);

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
                    switch (CatalogType.SelectedIndex)
                    {
                        case 0: // ANT Movie Catalog
                            mydivx.ReadXml(MesFilmsCat.Text);
                            break;
                        case 1: //DVD Profiler
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                                mydivx.ReadXml(destFile);
                                break;
                            }
                            if (radioButton1.Checked)
                                DVDPTagField.Text = "Category";
                            DvdProfiler cc1 = new DvdProfiler(DVDPTagField.Text);
                            mydivx.ReadXml(cc1.ConvertProfiler(MesFilmsCat.Text, MesFilmsImg.Text, SortTitle.Checked, DVDPTagField.Text, OnlyFile.Checked));
                            break;
                        case 2: // Movie Collector
                            MovieCollector cc2 = new MovieCollector();
                            if (Thumbnails.Checked)
                                mydivx.ReadXml(cc2.ConvertMovieCollector(MesFilmsCat.Text, MesFilmsImg.Text, SortTitle.Checked, OnlyFile.Checked, "Thumbnails", TitleDelim.Text));
                            else
                                mydivx.ReadXml(cc2.ConvertMovieCollector(MesFilmsCat.Text, MesFilmsImg.Text, SortTitle.Checked, OnlyFile.Checked, "Images", TitleDelim.Text));
                            break;
                        case 3: // MyMovies
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                                mydivx.ReadXml(destFile);
                                break;
                            }
                            MyMovies mm = new MyMovies();
                            mydivx.ReadXml(mm.ConvertMyMovies(MesFilmsCat.Text, MesFilmsImg.Text, SortTitle.Checked, OnlyFile.Checked));
                            break;
                        case 4: // EAX Movie Catalog
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                                mydivx.ReadXml(destFile);
                                break;
                            }
                            EaxMovieCatalog emc = new EaxMovieCatalog();
                            mydivx.ReadXml(emc.ConvertEaxMovieCatalog(MesFilmsCat.Text, MesFilmsImg.Text, SortTitle.Checked, OnlyFile.Checked, TitleDelim.Text));
                            break;
                        case 5: //eXtreme Movie Manager
                            destFile = MesFilmsCat.Text.Substring(0, MesFilmsCat.Text.Length - 4) + "_tmp.xml";
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                                mydivx.ReadXml(destFile);
                                break;
                            }
                            XMM xmm = new XMM();
                            mydivx.ReadXml(xmm.ConvertXMM(MesFilmsCat.Text, MesFilmsImg.Text, SortTitle.Checked, OnlyFile.Checked));
                            break;
                        case 6: // XBMC fulldb export (all movies in one DB)
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
                            mydivx.ReadXml(Xdb.ConvertXbmcDb(MesFilmsCat.Text, MesFilmsImg.Text, AntStorage.Text, SortTitle.Checked, OnlyFile.Checked, TitleDelim.Text));
                            break;
                        case 7: // XBMC Nfo (separate nfo files, to scan dirs - MovingPictures or XBMC)
                            destFile = MesFilmsCat.Text;
                            if ((System.IO.File.Exists(destFile) && (System.IO.File.GetLastWriteTime(destFile) > System.IO.File.GetLastWriteTime(MesFilmsCat.Text))))
                            {
                              mydivx.ReadXml(destFile);
                              break;
                            }
                            XbmcNfo nfo = new XbmcNfo();
                            mydivx.ReadXml(nfo.ConvertXbmcNfo(MesFilmsCat.Text, MesFilmsImg.Text, AntStorage.Text, SortTitle.Checked, OnlyFile.Checked, TitleDelim.Text));
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
                    Read_XML_Logos(Config_Name.Text);

                }
            }
        }

        private void Read_XML_AMCconfig(string currentconfig)
        {
          bool initialconfig = true;
          if (currentconfig.Length == 0) // Do not process, if no valid config is selected !
            return;

          string wfiledefault = Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings";
          if (System.IO.File.Exists(wfiledefault + ".xml"))
          {
            if (!System.IO.File.Exists(wfiledefault + "_" + currentconfig + ".xml"))
            {
              initialconfig = true;
              btnLaunchAMCupdater.Enabled = false;
              try
              {
                System.IO.File.Copy(XmlConfig.EntireFilenameConfig("MyFilmsAMCSettings"), wfiledefault + "_" + currentconfig + ".xml", true);
              }
              catch
              {
              }
            }
            else
              initialconfig = false;
            btnLaunchAMCupdater.Enabled = true;

          }
          else
            MessageBox.Show("The default AMCupdater configfile cannot be found! (" + Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings.xml" + ")", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);



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

          // if initialconfig set parameters from Myfilms, else just read existing configfile
          if (initialconfig)
          {
            // Set Parameters from MyFilms configuration
            AMCSetAttribute("Ant_Database_Source_Field", AntStorage.Text);
            AMCSetAttribute("Excluded_Movies_File", Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCExcludedMoviesFile.txt");
            AMCSetAttribute("Image_Download_Filename_Prefix", currentconfig + "_");
            AMCSetAttribute("Internet_Parser_Path", txtGrabber.Text);
            AMCSetAttribute("Manual_Excluded_Movies_File", Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCExcludedMoviesFile.txt");
            AMCSetAttribute("Manual_Internet_Parser_Path", txtAMCUpd_cnf.Text);
            AMCSetAttribute("Manual_XML_File", MesFilmsCat.Text);
            AMCSetAttribute("Master_Title", AntTitle1.Text);
            AMCSetAttribute("Movie_Fanart_Path", MesFilmsFanart.Text);
            AMCSetAttribute("XML_File", MesFilmsCat.Text);

            // Set specific userdefined Parameters from MyFilms GUI configuration
            AMCSetAttribute("Movie_Scan_Path", AMCMovieScanPath.Text);
            if (chkAMC_Purge_Missing_Files.Checked == true)
              AMCSetAttribute("Purge_Missing_Files", "true");
            else
              AMCSetAttribute("Purge_Missing_Files", "false");
            AMCSetAttribute("LogDirectory", Config.GetDirectoryInfo(Config.Dir.Config) + @"\log");

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

          // load dataset for GUI display
          AMCConfigView.Items.Clear();
          int i = 0;
          foreach (DataRow dr in AMCdsSettings.Tables[0].Rows)
          {
            AMCConfigView.Items.Add(dr[1].ToString());
            AMCConfigView.Items[i].SubItems.Add(dr[0].ToString());
            i = i + 1;
          }
          if (i > 0)
          {
            lblAMCupdaterConfigPreview.Visible = true;
            btnCreateAMCDefaultConfig.Enabled = true;
            btnCreateAMCDesktopIcon.Enabled = true;


            if (AMCMovieScanPath.Text.Contains("\\") || !initialconfig)
            {
              AMCConfigView.Visible = true;
              AMCMovieScanPath.Visible = false;
              lblAMCMovieScanPath.Visible = false;
              btnAMCMovieScanPathAdd.Visible = false;
              chkAMC_Purge_Missing_Files.Visible = false;
              txtAMCUpd_cnf.Text = AMCconfigFile;
              txtAMCUpd_cnf.Enabled = false;
            }
            else
            {
              AMCConfigView.Visible = false;
              AMCMovieScanPath.Visible = true;
              lblAMCMovieScanPath.Visible = true;
              btnAMCMovieScanPathAdd.Visible = true;
              chkAMC_Purge_Missing_Files.Visible = true;
            }
          }
          else
          {
            lblAMCupdaterConfigPreview.Visible = false;
            btnCreateAMCDefaultConfig.Enabled = true;
            btnCreateAMCDesktopIcon.Enabled = false;
            btnLaunchAMCupdater.Enabled = false;
            AMCConfigView.Visible = false;
          }
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
            LogMyFilms.Debug("AMCupdater Settings saved to file");
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

        private void Read_XML_Logos(string currentconfig)
        {
            //LogoView.Clear();
            string wfile = XmlConfig.EntireFilenameConfig("MyFilmsLogos").Substring(0, XmlConfig.EntireFilenameConfig("MyFilmsLogos").LastIndexOf("."));
            if (!System.IO.File.Exists(wfile + "_" + currentconfig + ".xml") && currentconfig.Length > 0)
            {
                try
                {
                    System.IO.File.Copy(XmlConfig.EntireFilenameConfig("MyFilmsLogos"), wfile + "_" + currentconfig + ".xml", true);
                    //wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1) + "_" + currentconfig;
                }
                catch
                {
                    wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
                }
            }
            //else
              //wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1) + "_" + currentconfig;

            wfile = wfile.Substring(wfile.LastIndexOf("\\") + 1);
            comboBoxLogoPresets.Text = XmlConfig.ReadXmlConfig(wfile, "ID0000", "LogoPresets", "Use Logos of currently selected skin");
            //txtLogosPath.Text = XmlConfig.ReadXmlConfig("MyFilmsLogos_" + Configuration.CurrentConfig, "ID0000", "LogosPath", Config.GetDirectoryInfo(Config.Dir.Thumbs).ToString() + "\\MyFilms_Logos");
            txtLogosPath.Text = XmlConfig.ReadXmlConfig(wfile, "ID0000", "LogosPath", "");
            comboBoxLogoSpacing.Text = XmlConfig.ReadXmlConfig(wfile, "ID0000", "Spacing", "1");
            selected_Logo_Item = -1;
            int i = 0;
            do
            {
                string wline = XmlConfig.ReadXmlConfig(wfile, "ID2001", "ID2001_" + i, null);
                if (wline == null)
                    break;
                string[] wtab = wline.Split(new Char[] { ';' });
                Charge_LogosView(ref wtab, i, "ID2001", txtLogosPath.Text);
                i++;
            } while (true);
            i = 0;
            do
            {
              string wline = XmlConfig.ReadXmlConfig(wfile, "ID2002", "ID2002_" + i, null);
              if (wline == null)
                break;
              string[] wtab = wline.Split(new Char[] { ';' });
              Charge_LogosView(ref wtab, i, "ID2002", txtLogosPath.Text);
              i++;
            } while (true);
            i = 0;
            do
            {
              string wline = XmlConfig.ReadXmlConfig(wfile, "ID2003", "ID2003_" + i, null);
              if (wline == null)
                break;
              string[] wtab = wline.Split(new Char[] { ';' });
              Charge_LogosView(ref wtab, i, "ID2003", txtLogosPath.Text);
              i++;
            } while (true);
            i = 0;
        }

        private void Charge_LogosView(ref string[] wtab, int i, string typelogo, string logopath)
        {
            string skinLogoPath;
            using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.MPSettings())
            {
                skinLogoPath = MediaPortal.Configuration.Config.GetDirectoryInfo(Config.Dir.Skin) + @"\" + xmlreader.GetValueAsString("skin", "name", "NoSkin") + @"\Media\Logos"; // Get current path to logos in skindirectory
            }
            if (logopath.Length == 0) 
              logopath = skinLogoPath;
            LogoView.Items.Add(typelogo);
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[0].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[1].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[2].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[3].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[4].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[5].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(wtab[6].ToString());
            LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetFileName(wtab[7].ToString()));
            if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(wtab[7].ToString())))
                LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(wtab[7].ToString()));
            else
                if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(logopath +  @"\" + wtab[7]))) // Check if Logofile exists in default media directory of current skin and add it, if found
                    LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(logopath + @"\" + wtab[7]));
                else
                  if (!wtab[7].Contains("\\")) // Check, if logo file is present in subdirectories of logo directory of current skin - only if not already full path defined !

                    {
                      string[] filePathsLogoSearch = System.IO.Directory.GetFiles(logopath + @"\", wtab[7], System.IO.SearchOption.AllDirectories);
                      if (filePathsLogoSearch.Length > 0)
                      {
                        LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(System.IO.Path.GetDirectoryName(filePathsLogoSearch[0]));
                      }
                    }
                  else
                    LogoView.Items[LogoView.Items.Count - 1].SubItems.Add(string.Empty); // Add empty field, if logofile istn't found anywhere
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
                    Read_XML_Logos(Config_Name.Text);
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
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                txtLogosPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnDirGrab_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                txtDirGrab.Text = folderBrowserDialog1.SelectedPath;
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
            if (CatalogType.SelectedIndex == 4)
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
            t.ApplicationName = txtAMCUpd_exe.Text;
            t.Parameters = txtAMCUpd_cnf.Text;
            t.Comment = "Updating the database for the MP plugin MyFilms";
            t.Creator = "MP plugin MyFilms";
            t.WorkingDirectory = txtAMCUpd_exe.Text.Substring(0, txtAMCUpd_exe.Text.LastIndexOf("\\"));
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
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                MesFilmsViews.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void ButDefCov_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "png";
            openFileDialog1.Filter = "PNG Files|*.png|JPG Files|*.jpg|BMP Files|*.bmp|All Files|*.*";
            openFileDialog1.Title = "Default display Cover";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                DefaultCover.Text = openFileDialog1.FileName;
        }

        private void btnResetThumbs_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to reset all generated Thumbs?", "Information", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (string wfile in System.IO.Directory.GetFiles(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups"))
                {
                    if (wfile != DefaultCover.Text)
                        System.IO.File.Delete(wfile);
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

        //private void ButDefaultCoverArtist_Click(object sender, EventArgs e)
        //{
        //    openFileDialog1.RestoreDirectory = true;
        //    openFileDialog1.DefaultExt = "png";
        //    openFileDialog1.Filter = "PNG Files|*.png|JPG Files|*.jpg|BMP Files|*.bmp|All Files|*.*";
        //    openFileDialog1.Title = "Default display Cover";
        //    if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
        //        DefaultCoverArtist.Text = openFileDialog1.FileName;
        //}

        private void ButImgArtist_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                MesFilmsImgArtist.Text = folderBrowserDialog1.SelectedPath;
            }

        }

        private void ButDefCovArtist_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.DefaultExt = "png";
            openFileDialog1.Filter = "PNG Files|*.png|JPG Files|*.jpg|BMP Files|*.bmp|All Files|*.*";
            openFileDialog1.Title = "Default Artist Cover";
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
                    if (wfile != DefaultCoverArtist.Text)
                        System.IO.File.Delete(wfile);
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
          if (btnLaunchAMCupdater.Visible == true)
          {
            launchAMCmanager();
          }
          else
          {
            MessageBox.Show("You have to configure Plugin and Grabber options first to launch AMC Manager", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            AMCMovieScanPath.Focus();
          }

        }
    
        private void launchAMCmanager()
        {
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

          LogMyFilms.Debug("MF: Setup - Successfully created Desktop Icon for '" + linkName + "'");
          DialogResult dialogResult = MessageBox.Show("Successfully created Desktop Icon for '" + linkName + "'", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

          //WshShellClass shortcut = new WshShellClass())
          //  {
          //    shortcut.Target = Application.ExecutablePath;
          //    shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
          //    shortcut.Description = "My Shorcut Name Here";
          //    shortcut.DisplayMode = ShellLink.LinkDisplayMode.edmNormal;
          //    shortcut.Save(STARTUP_SHORTCUT_FILEPATH);
          //  }

          //using (System.IO.StreamWriter writer = new System.IO.StreamWriter(deskDir + "\\" + linkName + ".url"))
          //{
          //  //string app = System.Reflection.Assembly.GetExecutingAssembly().Location;
          //  //string app = System.Reflection.Assembly.LoadFile(Config.GetDirectoryInfo(Config.Dir.Base).ToString() + @"\AMCupdater.exe").Location;
          //  string app = Config.GetDirectoryInfo(Config.Dir.Base).ToString() + @"\AMCupdater.exe";
          //  //string app = Config.GetDirectoryInfo(Config.Dir.Base).ToString() + @"\AMCupdater.exe";
          //  writer.WriteLine("[InternetShortcut]");
          //  writer.WriteLine("URL=file:///" + app.Replace(@"\", "/") + argument);
          //  //String.Format(@"file://{0}", Application.ExecutablePath.Replace(@"\", "/")); 
          //  writer.WriteLine("IconIndex=0");
          //  string icon = app.Replace('\\', '/');
          //  writer.WriteLine("IconFile=" + icon);
          //  writer.Flush();
          //  LogMyFilms.Debug("MF: Setup - Successfully created Desktop Icon for '" + linkName + "'");
          //  DialogResult dialogResult = MessageBox.Show("Successfully created Desktop Icon for '" + linkName + "'", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
          //}


          //try
          //{
          //    System.IO.File.Copy(shortcutName, desktopPath, true);
          //}

          //using (interop.ShellLink shortcut = new ShellLink())
          //{
          //    shortcut.Target = Application.ExecutablePath;
          //    shortcut.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
          //    shortcut.Description = "My Shorcut Name Here";
          //    shortcut.DisplayMode = ShellLink.LinkDisplayMode.edmNormal;
          //    shortcut.Save(STARTUP_SHORTCUT_FILEPATH);
          //}
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

        private void AMCMovieScanPath_TextChanged(object sender, EventArgs e)
        {
          if (AMCMovieScanPath.Text.Length > 0 && AMCMovieScanPath.Text.Contains("\\"))
            btnCreateAMCDefaultConfig.Enabled = true;
          else
            btnCreateAMCDefaultConfig.Enabled = false;
        }

        private void btnCreateAMCDefaultConfig_Click(object sender, EventArgs e)
        {

          if ((!AMCMovieScanPath.Text.Contains("\\")))
          {
            MessageBox.Show(
              "You first have to define the Search path for your movies to create a default config !",
              "Configuration",
              MessageBoxButtons.OK,
              MessageBoxIcon.Stop);
            AMCMovieScanPath.Visible = true;
            lblAMCMovieScanPath.Visible = true;
            btnAMCMovieScanPathAdd.Visible = true;
            chkAMC_Purge_Missing_Files.Visible = true;

            return;
          }
          string wfiledefault = Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings";
          if (System.IO.File.Exists(wfiledefault + ".xml"))
          {
            if (System.IO.File.Exists(wfiledefault + "_" + Config_Name.Text + ".xml"))
            {
              DialogResult dialogResult = MessageBox.Show("Are you sure you want to (re)create a default config? This will overwrite any existing configuration for this DB-config and you loose all customizations made!!!", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
              if (!(dialogResult == DialogResult.Yes))
              {
                return;
              }
              else
                System.IO.File.Copy(wfiledefault + "_" + Config_Name.Text + ".xml", wfiledefault + "_" + Config_Name.Text + ".xml.sav", true);
            }
            System.IO.File.Delete(wfiledefault + "_" + Config_Name.Text + ".xml");
            Read_XML_AMCconfig(Config_Name.Text);

            if (AMCMovieScanPath.Text.Contains("\\"))
            {
              Save_XML_AMCconfig(Config_Name.Text);
              MessageBox.Show("Successfully created an AMCupdater default config with your settings !", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {

            }

          }
          else
            System.Windows.Forms.MessageBox.Show("The default AMCupdater configfile cannot be found! (" + Config.GetDirectoryInfo(Config.Dir.Config).ToString() + @"\MyFilmsAMCSettings.xml" + ")", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
          if (comboBoxLogoPresets.Text == "Define your path to logo image files")
          {
            txtLogosPath.Visible = true;
            txtLogosPath.Enabled = true;
            btnLogosPath.Visible = true;
            lblLogosPath.Visible = true;
          }
          else
          {
            txtLogosPath.Visible = false;
            btnLogosPath.Visible = false;
            lblLogosPath.Visible = false;
          }

          if (comboBoxLogoPresets.Text == "Use Logos of currently selected skin")
          {
            txtLogosPath.Text = string.Empty;
            txtLogosPath.Visible = true;
            txtLogosPath.Enabled = false;
            btnLogosPath.Visible = false;
            lblLogosPath.Visible = false;
          }

          if (comboBoxLogoPresets.Text == "Use MyFilms Logo Pack")
          {
            txtLogosPath.Text = Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\DefaultLogos";
            txtLogosPath.Visible = true;
            txtLogosPath.Enabled = false;
            btnLogosPath.Visible = false;
            lblLogosPath.Visible = false;
          }

          if (comboBoxLogoPresets.Text == "Use Blue3Wide logos")
          {
            txtLogosPath.Text = Config.GetDirectoryInfo(Config.Dir.Skin) + @"\blue3wide\media\logos";
            txtLogosPath.Visible = true;
            txtLogosPath.Enabled = false;
            btnLogosPath.Visible = false;
            lblLogosPath.Visible = false;
          }
        }

        private void btnLogoClearCache_Click(object sender, EventArgs e)
        {
          int i = 0;
          foreach (string sFile in System.IO.Directory.GetFiles(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Logos"))
          {
            if (sFile.ToUpper().EndsWith(".PNG"))
              System.IO.File.Delete(sFile);
            i = i + 1;
          }
          if (i==0)
            MessageBox.Show("No cached logo files to delete, your logo cache is already empty !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
          else
            MessageBox.Show("Successfully cleared " + i.ToString() + " cached files in your logo cache directory! Be aware browsing your movies might be slower when rebuilding the logos in cache.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

    }
}
