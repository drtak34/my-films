using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MediaPortal.GUI.Library;
using System.IO;

namespace MesFilms
{
    partial class MesFilmsSetup
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        public string PluginName()
        {
            return "Mes Films";
        }

        public string Description()
        {
            return "Mes Films Ant Movie Catalog";
        }

        public string Author()
        {
            return "Zebons";
        }

        public void ShowPlugin()
        {
            ShowDialog();
        }
        public bool DefaultEnabled()
        {
            return false;
        }
        public bool CanEnable()
        {
            return true;
        }

        public bool HasSetup()
        {
            return true;
        }
        public int GetWindowId()
        {
            return 7986;
        }
        /// <summary>
        /// If the plugin should have its own button on the home screen then it
        /// should return true to this method, otherwise if it should not be on home
        /// it should return false
        /// </summary>
        /// <param name="strButtonText">text the button should have</param>
        /// <param name="strButtonImage">image for the button, or empty for default</param>
        /// <param name="strButtonImageFocus">image for the button, or empty for default</param>
        /// <param name="strPictureImage">subpicture for the button or empty for none</param>
        /// <returns>true  : plugin needs its own button on home
        ///          false : plugin does not need its own button on home</returns>
        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
        {

            strButtonText = GUILocalizeStrings.Get(50);
            if (strButtonText == "")
            {
                strButtonText = "Mes Films";
            }
            strButtonImage = "";
            strButtonImageFocus = "";
            strPictureImage = "hover_My videostack.png";
            return true;
        }
        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MesFilmsSetup));
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Config_Name = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Dwp = new System.Windows.Forms.TextBox();
            this.SearchSubDirs = new System.Windows.Forms.CheckBox();
            this.PathStorage = new System.Windows.Forms.TextBox();
            this.AntStorage = new System.Windows.Forms.ComboBox();
            this.CatalogType = new System.Windows.Forms.ComboBox();
            this.MesFilmsCat = new System.Windows.Forms.TextBox();
            this.groupBox_SortByItem = new System.Windows.Forms.GroupBox();
            this.AntSort2 = new System.Windows.Forms.ComboBox();
            this.AntTSort2 = new System.Windows.Forms.TextBox();
            this.AntSort1 = new System.Windows.Forms.ComboBox();
            this.AntTSort1 = new System.Windows.Forms.TextBox();
            this.groupBox_AntSelectedEnreg = new System.Windows.Forms.GroupBox();
            this.Selected_Enreg = new System.Windows.Forms.TextBox();
            this.AntFilterComb = new System.Windows.Forms.ComboBox();
            this.AntFilterSign2 = new System.Windows.Forms.ComboBox();
            this.AntFilterSign1 = new System.Windows.Forms.ComboBox();
            this.AntFilterItem2 = new System.Windows.Forms.ComboBox();
            this.AntFilterText2 = new System.Windows.Forms.TextBox();
            this.AntFilterItem1 = new System.Windows.Forms.ComboBox();
            this.AntFilterText1 = new System.Windows.Forms.TextBox();
            this.groupBox_DefaultView = new System.Windows.Forms.GroupBox();
            this.chkGlobalUnwatchedOnly = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.SortSens = new System.Windows.Forms.ComboBox();
            this.label32 = new System.Windows.Forms.Label();
            this.Sort = new System.Windows.Forms.ComboBox();
            this.AlwaysDefaultView = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.LayOut = new System.Windows.Forms.ComboBox();
            this.View_Dflt_Item = new System.Windows.Forms.ComboBox();
            this.View_Dflt_Text = new System.Windows.Forms.TextBox();
            this.groupBox_SupplementaryView = new System.Windows.Forms.GroupBox();
            this.AntViewValue5 = new System.Windows.Forms.TextBox();
            this.AntViewText5 = new System.Windows.Forms.TextBox();
            this.AntViewItem5 = new System.Windows.Forms.ComboBox();
            this.AntViewValue4 = new System.Windows.Forms.TextBox();
            this.AntViewText4 = new System.Windows.Forms.TextBox();
            this.AntViewItem4 = new System.Windows.Forms.ComboBox();
            this.AntViewValue3 = new System.Windows.Forms.TextBox();
            this.AntViewText3 = new System.Windows.Forms.TextBox();
            this.AntViewItem3 = new System.Windows.Forms.ComboBox();
            this.AntViewValue2 = new System.Windows.Forms.TextBox();
            this.AntViewValue1 = new System.Windows.Forms.TextBox();
            this.AntViewText2 = new System.Windows.Forms.TextBox();
            this.AntViewText1 = new System.Windows.Forms.TextBox();
            this.AntViewItem2 = new System.Windows.Forms.ComboBox();
            this.AntViewItem1 = new System.Windows.Forms.ComboBox();
            this.groupBox_DetailedSupplementaryInformations = new System.Windows.Forms.GroupBox();
            this.chkOnlyTitle = new System.Windows.Forms.CheckBox();
            this.AntItem3 = new System.Windows.Forms.ComboBox();
            this.AntLabel2 = new System.Windows.Forms.TextBox();
            this.AntItem2 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AntLabel1 = new System.Windows.Forms.TextBox();
            this.AntItem1 = new System.Windows.Forms.ComboBox();
            this.groupBox_DatabaseUpdateOptions = new System.Windows.Forms.GroupBox();
            this.groupBox_AntUpdatingItems = new System.Windows.Forms.GroupBox();
            this.AntUpdFieldReset = new System.Windows.Forms.Button();
            this.label33 = new System.Windows.Forms.Label();
            this.AntUpdList = new System.Windows.Forms.TextBox();
            this.AntUpdField = new System.Windows.Forms.ComboBox();
            this.chkWindowsFileDialog = new System.Windows.Forms.CheckBox();
            this.AntUpdDflT2 = new System.Windows.Forms.TextBox();
            this.AntUpdDflT1 = new System.Windows.Forms.TextBox();
            this.AntUpdItem2 = new System.Windows.Forms.ComboBox();
            this.AntUpdText2 = new System.Windows.Forms.TextBox();
            this.AntUpdItem1 = new System.Windows.Forms.ComboBox();
            this.AntUpdText1 = new System.Windows.Forms.TextBox();
            this.chksupplaystop = new System.Windows.Forms.CheckBox();
            this.gpspfield = new System.Windows.Forms.GroupBox();
            this.txtfdupdate = new System.Windows.Forms.TextBox();
            this.cbfdupdate = new System.Windows.Forms.ComboBox();
            this.gpsuppress = new System.Windows.Forms.GroupBox();
            this.rbsuppress2 = new System.Windows.Forms.RadioButton();
            this.rbsuppress4 = new System.Windows.Forms.RadioButton();
            this.rbsuppress3 = new System.Windows.Forms.RadioButton();
            this.rbsuppress1 = new System.Windows.Forms.RadioButton();
            this.chkSuppress = new System.Windows.Forms.CheckBox();
            this.groupBox_Separators = new System.Windows.Forms.GroupBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.RoleSeparator5 = new System.Windows.Forms.ComboBox();
            this.RoleSeparator4 = new System.Windows.Forms.ComboBox();
            this.ListSeparator5 = new System.Windows.Forms.ComboBox();
            this.ListSeparator4 = new System.Windows.Forms.ComboBox();
            this.ListSeparator3 = new System.Windows.Forms.ComboBox();
            this.ListSeparator2 = new System.Windows.Forms.ComboBox();
            this.RoleSeparator2 = new System.Windows.Forms.ComboBox();
            this.RoleSeparator3 = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.RoleSeparator1 = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.ListSeparator1 = new System.Windows.Forms.ComboBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.CheckBox();
            this.radioButton1 = new System.Windows.Forms.CheckBox();
            this.DVDPTagField = new System.Windows.Forms.ComboBox();
            this.groupBox20 = new System.Windows.Forms.GroupBox();
            this.scheduleAMCUpdater = new System.Windows.Forms.CheckBox();
            this.btnParameters = new System.Windows.Forms.Button();
            this.btnAMCUpd_cnf = new System.Windows.Forms.Button();
            this.txtAMCUpd_cnf = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.btnAMCUpd_exe = new System.Windows.Forms.Button();
            this.txtAMCUpd_exe = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.chkAMCUpd = new System.Windows.Forms.CheckBox();
            this.groupBox_GrabberOptions = new System.Windows.Forms.GroupBox();
            this.chkGrabber_ChooseScript = new System.Windows.Forms.CheckBox();
            this.btnDirGrab = new System.Windows.Forms.Button();
            this.txtDirGrab = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.chkGrabber_Always = new System.Windows.Forms.CheckBox();
            this.btnGrabber = new System.Windows.Forms.Button();
            this.txtGrabber = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.chkGrabber = new System.Windows.Forms.CheckBox();
            this.Fanart = new System.Windows.Forms.GroupBox();
            this.chkFanart = new System.Windows.Forms.CheckBox();
            this.btnFanart = new System.Windows.Forms.Button();
            this.MesFilmsFanart = new System.Windows.Forms.TextBox();
            this.labelFanart = new System.Windows.Forms.Label();
            this.chkDfltFanart = new System.Windows.Forms.CheckBox();
            this.txtLogosPath = new System.Windows.Forms.TextBox();
            this.chkLogos = new System.Windows.Forms.CheckBox();
            this.SLogo_Type = new System.Windows.Forms.ComboBox();
            this.ButQuit = new System.Windows.Forms.Button();
            this.ButDelet = new System.Windows.Forms.Button();
            this.Config_Dflt = new System.Windows.Forms.CheckBox();
            this.Config_Menu = new System.Windows.Forms.CheckBox();
            this.SearchFileName = new System.Windows.Forms.CheckBox();
            this.AntIdentItem = new System.Windows.Forms.ComboBox();
            this.AntSTitle = new System.Windows.Forms.ComboBox();
            this.TitleDelim = new System.Windows.Forms.TextBox();
            this.AntTitle2 = new System.Windows.Forms.ComboBox();
            this.AntTitle1 = new System.Windows.Forms.ComboBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.btnResetThumbs = new System.Windows.Forms.Button();
            this.chkViews = new System.Windows.Forms.CheckBox();
            this.btnViews = new System.Windows.Forms.Button();
            this.chkDfltViews = new System.Windows.Forms.CheckBox();
            this.label29 = new System.Windows.Forms.Label();
            this.MesFilmsViews = new System.Windows.Forms.TextBox();
            this.groupBox_SupplementarySearch = new System.Windows.Forms.GroupBox();
            this.AntSearchFieldReset = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.AntSearchList = new System.Windows.Forms.TextBox();
            this.AntSearchItem2 = new System.Windows.Forms.ComboBox();
            this.AntSearchField = new System.Windows.Forms.ComboBox();
            this.AntSearchText2 = new System.Windows.Forms.TextBox();
            this.AntSearchItem1 = new System.Windows.Forms.ComboBox();
            this.AntSearchText1 = new System.Windows.Forms.TextBox();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.SearchSubDirsTrailer = new System.Windows.Forms.CheckBox();
            this.btnTrailer = new System.Windows.Forms.Button();
            this.PathStorageTrailer = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.ShowTrailerWhenStartingMovie = new System.Windows.Forms.CheckBox();
            this.ShowTrailerPlayDialog = new System.Windows.Forms.CheckBox();
            this.SearchFileNameTrailer = new System.Windows.Forms.CheckBox();
            this.ItemSearchFileNameTrailer = new System.Windows.Forms.ComboBox();
            this.AntStorageTrailer = new System.Windows.Forms.ComboBox();
            this.label35 = new System.Windows.Forms.Label();
            this.check_WOL_enable = new System.Windows.Forms.CheckBox();
            this.groupBox25 = new System.Windows.Forms.GroupBox();
            this.label37 = new System.Windows.Forms.Label();
            this.comboWOLtimeout = new System.Windows.Forms.ComboBox();
            this.buttonSendMagicPacket3 = new System.Windows.Forms.Button();
            this.buttonSendMagicPacket2 = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.buttonSendMagicPacket1 = new System.Windows.Forms.Button();
            this.buttonGetMACadresses = new System.Windows.Forms.Button();
            this.label_NAS_Server_3_MAC = new System.Windows.Forms.Label();
            this.NAS_MAC_3 = new System.Windows.Forms.TextBox();
            this.label_NAS_Server_3_Name = new System.Windows.Forms.Label();
            this.NAS_Name_3 = new System.Windows.Forms.TextBox();
            this.label_NAS_Server_2_MAC = new System.Windows.Forms.Label();
            this.NAS_MAC_2 = new System.Windows.Forms.TextBox();
            this.label_NAS_Server_2_Name = new System.Windows.Forms.Label();
            this.NAS_Name_2 = new System.Windows.Forms.TextBox();
            this.label_NAS_Server_1_MAC = new System.Windows.Forms.Label();
            this.NAS_MAC_1 = new System.Windows.Forms.TextBox();
            this.label_NAS_Server_1_Name = new System.Windows.Forms.Label();
            this.check_WOL_Userdialog = new System.Windows.Forms.CheckBox();
            this.NAS_Name_1 = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label_VersionNumber = new System.Windows.Forms.Label();
            this.DefaultCover = new System.Windows.Forms.TextBox();
            this.MesFilmsImg = new System.Windows.Forms.TextBox();
            this.CheckWatched = new System.Windows.Forms.CheckBox();
            this.DefaultCoverArtist = new System.Windows.Forms.TextBox();
            this.MesFilmsImgArtist = new System.Windows.Forms.TextBox();
            this.groupBox_PreLaunchingCommand = new System.Windows.Forms.GroupBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.CmdPar = new System.Windows.Forms.ComboBox();
            this.CmdExe = new System.Windows.Forms.TextBox();
            this.groupBox_ArtistImages = new System.Windows.Forms.GroupBox();
            this.btnResetThumbsArtist = new System.Windows.Forms.Button();
            this.chkDfltArtist = new System.Windows.Forms.CheckBox();
            this.ButDefCovArtist = new System.Windows.Forms.Button();
            this.ButImgArtist = new System.Windows.Forms.Button();
            this.label_DefaultArtistImage = new System.Windows.Forms.Label();
            this.label_ArtistImagePath = new System.Windows.Forms.Label();
            this.checkWatchedInProfile = new System.Windows.Forms.CheckBox();
            this.AntFilterMinRating = new System.Windows.Forms.ComboBox();
            this.AntFilterSign4 = new System.Windows.Forms.ComboBox();
            this.AntFilterSign3 = new System.Windows.Forms.ComboBox();
            this.AntFilterItem4 = new System.Windows.Forms.ComboBox();
            this.AntFilterText4 = new System.Windows.Forms.TextBox();
            this.AntFilterItem3 = new System.Windows.Forms.ComboBox();
            this.AntFilterText3 = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.General = new System.Windows.Forms.TabControl();
            this.Tab_General = new System.Windows.Forms.TabPage();
            this.groupBox_Security = new System.Windows.Forms.GroupBox();
            this.label_Security = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.Rpt_Dwp = new System.Windows.Forms.TextBox();
            this.groupBox_PlayMovieInfos = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Label_UserProfileName = new System.Windows.Forms.Label();
            this.UserProfileName = new System.Windows.Forms.TextBox();
            this.AntIdentLabel = new System.Windows.Forms.TextBox();
            this.butPath = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ItemSearchFileName = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox_TitleOrder = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ButCat = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Tab_Artwork = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ButDefCov = new System.Windows.Forms.Button();
            this.label_DefaulCover = new System.Windows.Forms.Label();
            this.ButImg = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.Tab_Optional1 = new System.Windows.Forms.TabPage();
            this.Tab_Optional2 = new System.Windows.Forms.TabPage();
            this.Tab_DVDprofilerMovieCollector = new System.Windows.Forms.TabPage();
            this.groupBox_MovieCollector = new System.Windows.Forms.GroupBox();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.Images = new System.Windows.Forms.RadioButton();
            this.Thumbnails = new System.Windows.Forms.RadioButton();
            this.groupBox_DVDprofiler = new System.Windows.Forms.GroupBox();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.OnlyFile = new System.Windows.Forms.CheckBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.SortTitle = new System.Windows.Forms.CheckBox();
            this.Tab_Grabber = new System.Windows.Forms.TabPage();
            this.groupBox_nfoGrabber = new System.Windows.Forms.GroupBox();
            this.Tab_Logos = new System.Windows.Forms.TabPage();
            this.btnLogosPath = new System.Windows.Forms.Button();
            this.lblLogosPath = new System.Windows.Forms.Label();
            this.SFilePicture = new System.Windows.Forms.TextBox();
            this.LogoView = new System.Windows.Forms.ListView();
            this.LogoType = new System.Windows.Forms.ColumnHeader();
            this.Field1 = new System.Windows.Forms.ColumnHeader();
            this.Op1 = new System.Windows.Forms.ColumnHeader();
            this.Value1 = new System.Windows.Forms.ColumnHeader();
            this.And_Or = new System.Windows.Forms.ColumnHeader();
            this.Field2 = new System.Windows.Forms.ColumnHeader();
            this.Op2 = new System.Windows.Forms.ColumnHeader();
            this.Value2 = new System.Windows.Forms.ColumnHeader();
            this.Image = new System.Windows.Forms.ColumnHeader();
            this.PathImage = new System.Windows.Forms.ColumnHeader();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.SAnd_Or = new System.Windows.Forms.ComboBox();
            this.SValue2 = new System.Windows.Forms.ComboBox();
            this.SOp2 = new System.Windows.Forms.ComboBox();
            this.SField2 = new System.Windows.Forms.ComboBox();
            this.SValue1 = new System.Windows.Forms.ComboBox();
            this.SOp1 = new System.Windows.Forms.ComboBox();
            this.SField1 = new System.Windows.Forms.ComboBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.SPicture = new System.Windows.Forms.PictureBox();
            this.Tab_TrailerOptions = new System.Windows.Forms.TabPage();
            this.Tab_WakeOnLan = new System.Windows.Forms.TabPage();
            this.ButSave = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label21 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox_SortByItem.SuspendLayout();
            this.groupBox_AntSelectedEnreg.SuspendLayout();
            this.groupBox_DefaultView.SuspendLayout();
            this.groupBox_SupplementaryView.SuspendLayout();
            this.groupBox_DetailedSupplementaryInformations.SuspendLayout();
            this.groupBox_DatabaseUpdateOptions.SuspendLayout();
            this.groupBox_AntUpdatingItems.SuspendLayout();
            this.gpspfield.SuspendLayout();
            this.gpsuppress.SuspendLayout();
            this.groupBox_Separators.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox20.SuspendLayout();
            this.groupBox_GrabberOptions.SuspendLayout();
            this.Fanart.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox_SupplementarySearch.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.groupBox25.SuspendLayout();
            this.groupBox_PreLaunchingCommand.SuspendLayout();
            this.groupBox_ArtistImages.SuspendLayout();
            this.General.SuspendLayout();
            this.Tab_General.SuspendLayout();
            this.groupBox_Security.SuspendLayout();
            this.groupBox_PlayMovieInfos.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox_TitleOrder.SuspendLayout();
            this.Tab_Artwork.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.Tab_Optional1.SuspendLayout();
            this.Tab_Optional2.SuspendLayout();
            this.Tab_DVDprofilerMovieCollector.SuspendLayout();
            this.groupBox_MovieCollector.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.groupBox_DVDprofiler.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.Tab_Grabber.SuspendLayout();
            this.Tab_Logos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPicture)).BeginInit();
            this.Tab_TrailerOptions.SuspendLayout();
            this.Tab_WakeOnLan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ToolTip1
            // 
            this.ToolTip1.AutoPopDelay = 15000;
            this.ToolTip1.InitialDelay = 500;
            this.ToolTip1.ReshowDelay = 100;
            // 
            // Config_Name
            // 
            this.Config_Name.FormattingEnabled = true;
            this.Config_Name.Location = new System.Drawing.Point(195, 39);
            this.Config_Name.Name = "Config_Name";
            this.Config_Name.Size = new System.Drawing.Size(222, 21);
            this.Config_Name.Sorted = true;
            this.Config_Name.TabIndex = 1;
            this.ToolTip1.SetToolTip(this.Config_Name, resources.GetString("Config_Name.ToolTip"));
            this.Config_Name.SelectedIndexChanged += new System.EventHandler(this.Config_Name_SelectedIndexChanged);
            this.Config_Name.Leave += new System.EventHandler(this.Config_Name_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(195, 11);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(222, 20);
            this.textBox1.TabIndex = 5;
            this.ToolTip1.SetToolTip(this.textBox1, "Name of the plugin displayed in MP.\r\nBy default Films, but you can choose a bette" +
                    "r name");
            // 
            // Dwp
            // 
            this.Dwp.Location = new System.Drawing.Point(104, 43);
            this.Dwp.Name = "Dwp";
            this.Dwp.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Dwp.Size = new System.Drawing.Size(164, 20);
            this.Dwp.TabIndex = 69;
            this.ToolTip1.SetToolTip(this.Dwp, "Enter a password here if you wish to restrict access (from children, for example)" +
                    " \r\nto this particular configuration.  \r\nThe password must be repeated in the sec" +
                    "ond field.\r\n");
            this.Dwp.UseSystemPasswordChar = true;
            // 
            // SearchSubDirs
            // 
            this.SearchSubDirs.AutoSize = true;
            this.SearchSubDirs.Location = new System.Drawing.Point(506, 73);
            this.SearchSubDirs.Name = "SearchSubDirs";
            this.SearchSubDirs.Size = new System.Drawing.Size(130, 17);
            this.SearchSubDirs.TabIndex = 68;
            this.SearchSubDirs.Text = "Search in Sub Folders";
            this.ToolTip1.SetToolTip(this.SearchSubDirs, "Select this option if you want the search for the movie files \r\nto include sub-fo" +
                    "lders of the defined path.\r\n");
            this.SearchSubDirs.UseVisualStyleBackColor = true;
            // 
            // PathStorage
            // 
            this.PathStorage.Location = new System.Drawing.Point(172, 44);
            this.PathStorage.Name = "PathStorage";
            this.PathStorage.Size = new System.Drawing.Size(268, 20);
            this.PathStorage.TabIndex = 54;
            this.ToolTip1.SetToolTip(this.PathStorage, resources.GetString("PathStorage.ToolTip"));
            this.PathStorage.TextChanged += new System.EventHandler(this.PathStorage_TextChanged);
            // 
            // AntStorage
            // 
            this.AntStorage.FormattingEnabled = true;
            this.AntStorage.Location = new System.Drawing.Point(172, 16);
            this.AntStorage.Name = "AntStorage";
            this.AntStorage.Size = new System.Drawing.Size(223, 21);
            this.AntStorage.TabIndex = 53;
            this.ToolTip1.SetToolTip(this.AntStorage, resources.GetString("AntStorage.ToolTip"));
            // 
            // CatalogType
            // 
            this.CatalogType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CatalogType.FormattingEnabled = true;
            this.CatalogType.Items.AddRange(new object[] {
            "Ant Movie Catalog",
            "DVD Profiler",
            "MovieCollector",
            "MyMovies",
            "Eax Movie Catalog",
            "eXtreme Movie Manager"});
            this.CatalogType.Location = new System.Drawing.Point(528, 15);
            this.CatalogType.Name = "CatalogType";
            this.CatalogType.Size = new System.Drawing.Size(175, 21);
            this.CatalogType.TabIndex = 62;
            this.ToolTip1.SetToolTip(this.CatalogType, resources.GetString("CatalogType.ToolTip"));
            this.CatalogType.SelectedIndexChanged += new System.EventHandler(this.CatalogType_SelectedIndexChanged);
            // 
            // MesFilmsCat
            // 
            this.MesFilmsCat.Location = new System.Drawing.Point(116, 15);
            this.MesFilmsCat.Name = "MesFilmsCat";
            this.MesFilmsCat.Size = new System.Drawing.Size(285, 20);
            this.MesFilmsCat.TabIndex = 48;
            this.ToolTip1.SetToolTip(this.MesFilmsCat, "Enter the full path and name of your AMC xml database file.\r\nYou can use the brow" +
                    "se button to find the file");
            this.MesFilmsCat.TextChanged += new System.EventHandler(this.MesFilmsCat_TextChanged);
            this.MesFilmsCat.Leave += new System.EventHandler(this.MesFilmsCat_Leave);
            // 
            // groupBox_SortByItem
            // 
            this.groupBox_SortByItem.Controls.Add(this.AntSort2);
            this.groupBox_SortByItem.Controls.Add(this.AntTSort2);
            this.groupBox_SortByItem.Controls.Add(this.AntSort1);
            this.groupBox_SortByItem.Controls.Add(this.AntTSort1);
            this.groupBox_SortByItem.Location = new System.Drawing.Point(371, 271);
            this.groupBox_SortByItem.Name = "groupBox_SortByItem";
            this.groupBox_SortByItem.Size = new System.Drawing.Size(370, 77);
            this.groupBox_SortByItem.TabIndex = 29;
            this.groupBox_SortByItem.TabStop = false;
            this.groupBox_SortByItem.Text = "Sort by Item";
            this.ToolTip1.SetToolTip(this.groupBox_SortByItem, resources.GetString("groupBox_SortByItem.ToolTip"));
            // 
            // AntSort2
            // 
            this.AntSort2.FormattingEnabled = true;
            this.AntSort2.Location = new System.Drawing.Point(10, 45);
            this.AntSort2.Name = "AntSort2";
            this.AntSort2.Size = new System.Drawing.Size(160, 21);
            this.AntSort2.TabIndex = 38;
            this.AntSort2.SelectedIndexChanged += new System.EventHandler(this.AntSort2_SelectedIndexChanged);
            this.AntSort2.Leave += new System.EventHandler(this.MesFilmsSetup_Load);
            // 
            // AntTSort2
            // 
            this.AntTSort2.Location = new System.Drawing.Point(185, 45);
            this.AntTSort2.Name = "AntTSort2";
            this.AntTSort2.Size = new System.Drawing.Size(173, 20);
            this.AntTSort2.TabIndex = 39;
            this.AntTSort2.TextChanged += new System.EventHandler(this.AntTSort2_TextChanged);
            // 
            // AntSort1
            // 
            this.AntSort1.FormattingEnabled = true;
            this.AntSort1.Location = new System.Drawing.Point(10, 18);
            this.AntSort1.Name = "AntSort1";
            this.AntSort1.Size = new System.Drawing.Size(160, 21);
            this.AntSort1.TabIndex = 36;
            this.AntSort1.SelectedIndexChanged += new System.EventHandler(this.AntSort1_SelectedIndexChanged);
            this.AntSort1.Leave += new System.EventHandler(this.MesFilmsSetup_Load);
            // 
            // AntTSort1
            // 
            this.AntTSort1.Location = new System.Drawing.Point(185, 18);
            this.AntTSort1.Name = "AntTSort1";
            this.AntTSort1.Size = new System.Drawing.Size(173, 20);
            this.AntTSort1.TabIndex = 37;
            this.AntTSort1.TextChanged += new System.EventHandler(this.AntTSort1_TextChanged);
            // 
            // groupBox_AntSelectedEnreg
            // 
            this.groupBox_AntSelectedEnreg.Controls.Add(this.Selected_Enreg);
            this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterComb);
            this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterSign2);
            this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterSign1);
            this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterItem2);
            this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterText2);
            this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterItem1);
            this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterText1);
            this.groupBox_AntSelectedEnreg.Location = new System.Drawing.Point(371, 6);
            this.groupBox_AntSelectedEnreg.Name = "groupBox_AntSelectedEnreg";
            this.groupBox_AntSelectedEnreg.Size = new System.Drawing.Size(370, 117);
            this.groupBox_AntSelectedEnreg.TabIndex = 27;
            this.groupBox_AntSelectedEnreg.TabStop = false;
            this.groupBox_AntSelectedEnreg.Text = "Ant Selected Enreg.";
            this.ToolTip1.SetToolTip(this.groupBox_AntSelectedEnreg, resources.GetString("groupBox_AntSelectedEnreg.ToolTip"));
            this.groupBox_AntSelectedEnreg.Leave += new System.EventHandler(this.Selected_Enreg_Changed);
            // 
            // Selected_Enreg
            // 
            this.Selected_Enreg.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Selected_Enreg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Selected_Enreg.Enabled = false;
            this.Selected_Enreg.Location = new System.Drawing.Point(8, 73);
            this.Selected_Enreg.Multiline = true;
            this.Selected_Enreg.Name = "Selected_Enreg";
            this.Selected_Enreg.ReadOnly = true;
            this.Selected_Enreg.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.Selected_Enreg.Size = new System.Drawing.Size(355, 38);
            this.Selected_Enreg.TabIndex = 28;
            this.Selected_Enreg.TabStop = false;
            // 
            // AntFilterComb
            // 
            this.AntFilterComb.DisplayMember = "or";
            this.AntFilterComb.FormattingEnabled = true;
            this.AntFilterComb.ItemHeight = 13;
            this.AntFilterComb.Items.AddRange(new object[] {
            "or",
            "and"});
            this.AntFilterComb.Location = new System.Drawing.Point(6, 34);
            this.AntFilterComb.Name = "AntFilterComb";
            this.AntFilterComb.Size = new System.Drawing.Size(43, 21);
            this.AntFilterComb.TabIndex = 18;
            // 
            // AntFilterSign2
            // 
            this.AntFilterSign2.DisplayMember = "#";
            this.AntFilterSign2.FormattingEnabled = true;
            this.AntFilterSign2.Items.AddRange(new object[] {
            "=",
            "#",
            ">",
            "<",
            "like",
            "not like"});
            this.AntFilterSign2.Location = new System.Drawing.Point(176, 46);
            this.AntFilterSign2.Name = "AntFilterSign2";
            this.AntFilterSign2.Size = new System.Drawing.Size(60, 21);
            this.AntFilterSign2.TabIndex = 23;
            // 
            // AntFilterSign1
            // 
            this.AntFilterSign1.DisplayMember = "#";
            this.AntFilterSign1.FormattingEnabled = true;
            this.AntFilterSign1.Items.AddRange(new object[] {
            "=",
            "#",
            ">",
            "<",
            "like",
            "not like"});
            this.AntFilterSign1.Location = new System.Drawing.Point(176, 19);
            this.AntFilterSign1.Name = "AntFilterSign1";
            this.AntFilterSign1.Size = new System.Drawing.Size(60, 21);
            this.AntFilterSign1.TabIndex = 20;
            // 
            // AntFilterItem2
            // 
            this.AntFilterItem2.FormattingEnabled = true;
            this.AntFilterItem2.Location = new System.Drawing.Point(55, 46);
            this.AntFilterItem2.Name = "AntFilterItem2";
            this.AntFilterItem2.Size = new System.Drawing.Size(115, 21);
            this.AntFilterItem2.TabIndex = 22;
            // 
            // AntFilterText2
            // 
            this.AntFilterText2.Location = new System.Drawing.Point(242, 46);
            this.AntFilterText2.Name = "AntFilterText2";
            this.AntFilterText2.Size = new System.Drawing.Size(113, 20);
            this.AntFilterText2.TabIndex = 24;
            // 
            // AntFilterItem1
            // 
            this.AntFilterItem1.FormattingEnabled = true;
            this.AntFilterItem1.Location = new System.Drawing.Point(55, 19);
            this.AntFilterItem1.Name = "AntFilterItem1";
            this.AntFilterItem1.Size = new System.Drawing.Size(115, 21);
            this.AntFilterItem1.TabIndex = 19;
            // 
            // AntFilterText1
            // 
            this.AntFilterText1.Location = new System.Drawing.Point(242, 19);
            this.AntFilterText1.Name = "AntFilterText1";
            this.AntFilterText1.Size = new System.Drawing.Size(113, 20);
            this.AntFilterText1.TabIndex = 21;
            // 
            // groupBox_DefaultView
            // 
            this.groupBox_DefaultView.Controls.Add(this.chkGlobalUnwatchedOnly);
            this.groupBox_DefaultView.Controls.Add(this.label10);
            this.groupBox_DefaultView.Controls.Add(this.SortSens);
            this.groupBox_DefaultView.Controls.Add(this.label32);
            this.groupBox_DefaultView.Controls.Add(this.Sort);
            this.groupBox_DefaultView.Controls.Add(this.AlwaysDefaultView);
            this.groupBox_DefaultView.Controls.Add(this.label14);
            this.groupBox_DefaultView.Controls.Add(this.LayOut);
            this.groupBox_DefaultView.Controls.Add(this.View_Dflt_Item);
            this.groupBox_DefaultView.Controls.Add(this.View_Dflt_Text);
            this.groupBox_DefaultView.Location = new System.Drawing.Point(9, 169);
            this.groupBox_DefaultView.Name = "groupBox_DefaultView";
            this.groupBox_DefaultView.Size = new System.Drawing.Size(347, 179);
            this.groupBox_DefaultView.TabIndex = 26;
            this.groupBox_DefaultView.TabStop = false;
            this.groupBox_DefaultView.Text = "Default Start View";
            this.ToolTip1.SetToolTip(this.groupBox_DefaultView, resources.GetString("groupBox_DefaultView.ToolTip"));
            // 
            // chkGlobalUnwatchedOnly
            // 
            this.chkGlobalUnwatchedOnly.AutoSize = true;
            this.chkGlobalUnwatchedOnly.Location = new System.Drawing.Point(10, 135);
            this.chkGlobalUnwatchedOnly.Name = "chkGlobalUnwatchedOnly";
            this.chkGlobalUnwatchedOnly.Size = new System.Drawing.Size(299, 17);
            this.chkGlobalUnwatchedOnly.TabIndex = 80;
            this.chkGlobalUnwatchedOnly.Text = "Show only unwatched movies (can be changed from GUI)";
            this.ToolTip1.SetToolTip(this.chkGlobalUnwatchedOnly, "Unwatched option acts as global overlayfilter for all views\r\nand setting is used " +
                    "for start view.\r\nIt can be switched on/off from the GUI during runtime.");
            this.chkGlobalUnwatchedOnly.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label10.Location = new System.Drawing.Point(7, 94);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(291, 13);
            this.label10.TabIndex = 79;
            this.label10.Text = "Enabling a DefaultView will disable \'remember last item/view\'";
            // 
            // SortSens
            // 
            this.SortSens.FormattingEnabled = true;
            this.SortSens.Items.AddRange(new object[] {
            "ASC",
            "DESC"});
            this.SortSens.Location = new System.Drawing.Point(285, 44);
            this.SortSens.Name = "SortSens";
            this.SortSens.Size = new System.Drawing.Size(55, 21);
            this.SortSens.TabIndex = 78;
            this.SortSens.Text = "ASC";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(157, 47);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(26, 13);
            this.label32.TabIndex = 77;
            this.label32.Text = "Sort";
            // 
            // Sort
            // 
            this.Sort.FormattingEnabled = true;
            this.Sort.Location = new System.Drawing.Point(189, 44);
            this.Sort.Name = "Sort";
            this.Sort.Size = new System.Drawing.Size(90, 21);
            this.Sort.TabIndex = 76;
            this.Sort.SelectedIndexChanged += new System.EventHandler(this.Sort_SelectedIndexChanged);
            // 
            // AlwaysDefaultView
            // 
            this.AlwaysDefaultView.AutoSize = true;
            this.AlwaysDefaultView.Location = new System.Drawing.Point(10, 71);
            this.AlwaysDefaultView.Name = "AlwaysDefaultView";
            this.AlwaysDefaultView.Size = new System.Drawing.Size(253, 17);
            this.AlwaysDefaultView.TabIndex = 75;
            this.AlwaysDefaultView.Text = "Display Always that view when using  this config";
            this.AlwaysDefaultView.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 47);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(39, 13);
            this.label14.TabIndex = 67;
            this.label14.Text = "Layout";
            // 
            // LayOut
            // 
            this.LayOut.FormattingEnabled = true;
            this.LayOut.Items.AddRange(new object[] {
            "List",
            "Small Icons",
            "Large Icons",
            "Filmstrip",
            "Coverflow"});
            this.LayOut.Location = new System.Drawing.Point(50, 44);
            this.LayOut.Name = "LayOut";
            this.LayOut.Size = new System.Drawing.Size(103, 21);
            this.LayOut.TabIndex = 66;
            this.LayOut.Text = "List";
            // 
            // View_Dflt_Item
            // 
            this.View_Dflt_Item.FormattingEnabled = true;
            this.View_Dflt_Item.Location = new System.Drawing.Point(10, 18);
            this.View_Dflt_Item.Name = "View_Dflt_Item";
            this.View_Dflt_Item.Size = new System.Drawing.Size(170, 21);
            this.View_Dflt_Item.TabIndex = 16;
            this.View_Dflt_Item.SelectedIndexChanged += new System.EventHandler(this.View_Dflt_Item_SelectedIndexChanged);
            // 
            // View_Dflt_Text
            // 
            this.View_Dflt_Text.Location = new System.Drawing.Point(189, 18);
            this.View_Dflt_Text.Name = "View_Dflt_Text";
            this.View_Dflt_Text.Size = new System.Drawing.Size(151, 20);
            this.View_Dflt_Text.TabIndex = 17;
            // 
            // groupBox_SupplementaryView
            // 
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewValue5);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewText5);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewItem5);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewValue4);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewText4);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewItem4);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewValue3);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewText3);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewItem3);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewValue2);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewValue1);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewText2);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewText1);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewItem2);
            this.groupBox_SupplementaryView.Controls.Add(this.AntViewItem1);
            this.groupBox_SupplementaryView.Location = new System.Drawing.Point(9, 6);
            this.groupBox_SupplementaryView.Name = "groupBox_SupplementaryView";
            this.groupBox_SupplementaryView.Size = new System.Drawing.Size(347, 157);
            this.groupBox_SupplementaryView.TabIndex = 24;
            this.groupBox_SupplementaryView.TabStop = false;
            this.groupBox_SupplementaryView.Text = "Supplementary View";
            this.ToolTip1.SetToolTip(this.groupBox_SupplementaryView, resources.GetString("groupBox_SupplementaryView.ToolTip"));
            // 
            // AntViewValue5
            // 
            this.AntViewValue5.Location = new System.Drawing.Point(237, 122);
            this.AntViewValue5.Name = "AntViewValue5";
            this.AntViewValue5.Size = new System.Drawing.Size(103, 20);
            this.AntViewValue5.TabIndex = 26;
            // 
            // AntViewText5
            // 
            this.AntViewText5.Location = new System.Drawing.Point(126, 122);
            this.AntViewText5.Name = "AntViewText5";
            this.AntViewText5.Size = new System.Drawing.Size(105, 20);
            this.AntViewText5.TabIndex = 25;
            this.AntViewText5.Leave += new System.EventHandler(this.AntViewText5_Leave);
            // 
            // AntViewItem5
            // 
            this.AntViewItem5.FormattingEnabled = true;
            this.AntViewItem5.Location = new System.Drawing.Point(10, 122);
            this.AntViewItem5.Name = "AntViewItem5";
            this.AntViewItem5.Size = new System.Drawing.Size(110, 21);
            this.AntViewItem5.TabIndex = 24;
            this.AntViewItem5.SelectedIndexChanged += new System.EventHandler(this.AntViewItem5_SelectedIndexChanged);
            // 
            // AntViewValue4
            // 
            this.AntViewValue4.Location = new System.Drawing.Point(237, 96);
            this.AntViewValue4.Name = "AntViewValue4";
            this.AntViewValue4.Size = new System.Drawing.Size(103, 20);
            this.AntViewValue4.TabIndex = 23;
            // 
            // AntViewText4
            // 
            this.AntViewText4.Location = new System.Drawing.Point(126, 96);
            this.AntViewText4.Name = "AntViewText4";
            this.AntViewText4.Size = new System.Drawing.Size(105, 20);
            this.AntViewText4.TabIndex = 22;
            this.AntViewText4.Leave += new System.EventHandler(this.AntViewText4_Leave);
            // 
            // AntViewItem4
            // 
            this.AntViewItem4.FormattingEnabled = true;
            this.AntViewItem4.Location = new System.Drawing.Point(10, 96);
            this.AntViewItem4.Name = "AntViewItem4";
            this.AntViewItem4.Size = new System.Drawing.Size(110, 21);
            this.AntViewItem4.TabIndex = 21;
            this.AntViewItem4.SelectedIndexChanged += new System.EventHandler(this.AntViewItem4_SelectedIndexChanged);
            // 
            // AntViewValue3
            // 
            this.AntViewValue3.Location = new System.Drawing.Point(237, 70);
            this.AntViewValue3.Name = "AntViewValue3";
            this.AntViewValue3.Size = new System.Drawing.Size(103, 20);
            this.AntViewValue3.TabIndex = 20;
            // 
            // AntViewText3
            // 
            this.AntViewText3.Location = new System.Drawing.Point(126, 70);
            this.AntViewText3.Name = "AntViewText3";
            this.AntViewText3.Size = new System.Drawing.Size(105, 20);
            this.AntViewText3.TabIndex = 19;
            this.AntViewText3.Leave += new System.EventHandler(this.AntViewText3_Leave);
            // 
            // AntViewItem3
            // 
            this.AntViewItem3.FormattingEnabled = true;
            this.AntViewItem3.Location = new System.Drawing.Point(10, 70);
            this.AntViewItem3.Name = "AntViewItem3";
            this.AntViewItem3.Size = new System.Drawing.Size(110, 21);
            this.AntViewItem3.TabIndex = 18;
            this.AntViewItem3.SelectedIndexChanged += new System.EventHandler(this.AntViewItem3_SelectedIndexChanged);
            // 
            // AntViewValue2
            // 
            this.AntViewValue2.Location = new System.Drawing.Point(237, 44);
            this.AntViewValue2.Name = "AntViewValue2";
            this.AntViewValue2.Size = new System.Drawing.Size(103, 20);
            this.AntViewValue2.TabIndex = 17;
            // 
            // AntViewValue1
            // 
            this.AntViewValue1.Location = new System.Drawing.Point(237, 18);
            this.AntViewValue1.Name = "AntViewValue1";
            this.AntViewValue1.Size = new System.Drawing.Size(103, 20);
            this.AntViewValue1.TabIndex = 14;
            // 
            // AntViewText2
            // 
            this.AntViewText2.Location = new System.Drawing.Point(126, 44);
            this.AntViewText2.Name = "AntViewText2";
            this.AntViewText2.Size = new System.Drawing.Size(105, 20);
            this.AntViewText2.TabIndex = 16;
            this.AntViewText2.Leave += new System.EventHandler(this.AntViewText2_Leave);
            // 
            // AntViewText1
            // 
            this.AntViewText1.Location = new System.Drawing.Point(126, 18);
            this.AntViewText1.Name = "AntViewText1";
            this.AntViewText1.Size = new System.Drawing.Size(105, 20);
            this.AntViewText1.TabIndex = 13;
            this.AntViewText1.TextChanged += new System.EventHandler(this.AntViewText1_Leave);
            this.AntViewText1.Leave += new System.EventHandler(this.AntViewText1_Leave);
            // 
            // AntViewItem2
            // 
            this.AntViewItem2.FormattingEnabled = true;
            this.AntViewItem2.Location = new System.Drawing.Point(10, 44);
            this.AntViewItem2.Name = "AntViewItem2";
            this.AntViewItem2.Size = new System.Drawing.Size(110, 21);
            this.AntViewItem2.TabIndex = 15;
            this.AntViewItem2.SelectedIndexChanged += new System.EventHandler(this.AntViewItem2_SelectedIndexChanged);
            // 
            // AntViewItem1
            // 
            this.AntViewItem1.ItemHeight = 13;
            this.AntViewItem1.Location = new System.Drawing.Point(10, 18);
            this.AntViewItem1.Name = "AntViewItem1";
            this.AntViewItem1.Size = new System.Drawing.Size(110, 21);
            this.AntViewItem1.TabIndex = 12;
            this.AntViewItem1.SelectedIndexChanged += new System.EventHandler(this.AntViewItem1_SelectedIndexChanged);
            // 
            // groupBox_DetailedSupplementaryInformations
            // 
            this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.chkOnlyTitle);
            this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntItem3);
            this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntLabel2);
            this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntItem2);
            this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.label7);
            this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.label3);
            this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntLabel1);
            this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntItem1);
            this.groupBox_DetailedSupplementaryInformations.Location = new System.Drawing.Point(371, 129);
            this.groupBox_DetailedSupplementaryInformations.Name = "groupBox_DetailedSupplementaryInformations";
            this.groupBox_DetailedSupplementaryInformations.Size = new System.Drawing.Size(370, 136);
            this.groupBox_DetailedSupplementaryInformations.TabIndex = 22;
            this.groupBox_DetailedSupplementaryInformations.TabStop = false;
            this.groupBox_DetailedSupplementaryInformations.Text = "Detailed Supplementary Informations";
            this.ToolTip1.SetToolTip(this.groupBox_DetailedSupplementaryInformations, resources.GetString("groupBox_DetailedSupplementaryInformations.ToolTip"));
            // 
            // chkOnlyTitle
            // 
            this.chkOnlyTitle.AutoSize = true;
            this.chkOnlyTitle.Location = new System.Drawing.Point(9, 111);
            this.chkOnlyTitle.Name = "chkOnlyTitle";
            this.chkOnlyTitle.Size = new System.Drawing.Size(230, 17);
            this.chkOnlyTitle.TabIndex = 76;
            this.chkOnlyTitle.Text = "Display Only Movie\'s Title within List Layout";
            this.chkOnlyTitle.UseVisualStyleBackColor = true;
            // 
            // AntItem3
            // 
            this.AntItem3.FormattingEnabled = true;
            this.AntItem3.Location = new System.Drawing.Point(185, 87);
            this.AntItem3.Name = "AntItem3";
            this.AntItem3.Size = new System.Drawing.Size(173, 21);
            this.AntItem3.TabIndex = 29;
            // 
            // AntLabel2
            // 
            this.AntLabel2.Location = new System.Drawing.Point(9, 60);
            this.AntLabel2.Name = "AntLabel2";
            this.AntLabel2.Size = new System.Drawing.Size(161, 20);
            this.AntLabel2.TabIndex = 27;
            // 
            // AntItem2
            // 
            this.AntItem2.FormattingEnabled = true;
            this.AntItem2.Location = new System.Drawing.Point(185, 60);
            this.AntItem2.Name = "AntItem2";
            this.AntItem2.Size = new System.Drawing.Size(173, 21);
            this.AntItem2.TabIndex = 28;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(182, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Ant Item to Display";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Label";
            // 
            // AntLabel1
            // 
            this.AntLabel1.Location = new System.Drawing.Point(9, 33);
            this.AntLabel1.Name = "AntLabel1";
            this.AntLabel1.Size = new System.Drawing.Size(161, 20);
            this.AntLabel1.TabIndex = 25;
            // 
            // AntItem1
            // 
            this.AntItem1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.AntItem1.FormattingEnabled = true;
            this.AntItem1.Location = new System.Drawing.Point(185, 33);
            this.AntItem1.Name = "AntItem1";
            this.AntItem1.Size = new System.Drawing.Size(173, 21);
            this.AntItem1.Sorted = true;
            this.AntItem1.TabIndex = 26;
            // 
            // groupBox_DatabaseUpdateOptions
            // 
            this.groupBox_DatabaseUpdateOptions.Controls.Add(this.groupBox_AntUpdatingItems);
            this.groupBox_DatabaseUpdateOptions.Controls.Add(this.chksupplaystop);
            this.groupBox_DatabaseUpdateOptions.Controls.Add(this.gpspfield);
            this.groupBox_DatabaseUpdateOptions.Controls.Add(this.gpsuppress);
            this.groupBox_DatabaseUpdateOptions.Controls.Add(this.chkSuppress);
            this.groupBox_DatabaseUpdateOptions.Location = new System.Drawing.Point(11, 6);
            this.groupBox_DatabaseUpdateOptions.Name = "groupBox_DatabaseUpdateOptions";
            this.groupBox_DatabaseUpdateOptions.Size = new System.Drawing.Size(370, 342);
            this.groupBox_DatabaseUpdateOptions.TabIndex = 32;
            this.groupBox_DatabaseUpdateOptions.TabStop = false;
            this.groupBox_DatabaseUpdateOptions.Text = "Database update options";
            this.ToolTip1.SetToolTip(this.groupBox_DatabaseUpdateOptions, resources.GetString("groupBox_DatabaseUpdateOptions.ToolTip"));
            // 
            // groupBox_AntUpdatingItems
            // 
            this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdFieldReset);
            this.groupBox_AntUpdatingItems.Controls.Add(this.label33);
            this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdList);
            this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdField);
            this.groupBox_AntUpdatingItems.Controls.Add(this.chkWindowsFileDialog);
            this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdDflT2);
            this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdDflT1);
            this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdItem2);
            this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdText2);
            this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdItem1);
            this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdText1);
            this.groupBox_AntUpdatingItems.Location = new System.Drawing.Point(9, 164);
            this.groupBox_AntUpdatingItems.Name = "groupBox_AntUpdatingItems";
            this.groupBox_AntUpdatingItems.Size = new System.Drawing.Size(354, 168);
            this.groupBox_AntUpdatingItems.TabIndex = 33;
            this.groupBox_AntUpdatingItems.TabStop = false;
            this.groupBox_AntUpdatingItems.Text = "Ant Updating Items.";
            this.ToolTip1.SetToolTip(this.groupBox_AntUpdatingItems, resources.GetString("groupBox_AntUpdatingItems.ToolTip"));
            // 
            // AntUpdFieldReset
            // 
            this.AntUpdFieldReset.Location = new System.Drawing.Point(297, 113);
            this.AntUpdFieldReset.Name = "AntUpdFieldReset";
            this.AntUpdFieldReset.Size = new System.Drawing.Size(51, 23);
            this.AntUpdFieldReset.TabIndex = 84;
            this.AntUpdFieldReset.Text = "Reset";
            this.ToolTip1.SetToolTip(this.AntUpdFieldReset, "That Action\'ll clear the field.");
            this.AntUpdFieldReset.UseVisualStyleBackColor = true;
            this.AntUpdFieldReset.Click += new System.EventHandler(this.AntUpdFieldReset_Click);
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(7, 118);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(153, 13);
            this.label33.TabIndex = 83;
            this.label33.Text = "Update by Properties Selection";
            // 
            // AntUpdList
            // 
            this.AntUpdList.BackColor = System.Drawing.SystemColors.Control;
            this.AntUpdList.Location = new System.Drawing.Point(10, 142);
            this.AntUpdList.Name = "AntUpdList";
            this.AntUpdList.Size = new System.Drawing.Size(338, 20);
            this.AntUpdList.TabIndex = 82;
            // 
            // AntUpdField
            // 
            this.AntUpdField.FormattingEnabled = true;
            this.AntUpdField.Location = new System.Drawing.Point(166, 115);
            this.AntUpdField.Name = "AntUpdField";
            this.AntUpdField.Size = new System.Drawing.Size(113, 21);
            this.AntUpdField.TabIndex = 81;
            this.AntUpdField.SelectedIndexChanged += new System.EventHandler(this.AntUpdField_SelectedIndexChanged);
            // 
            // chkWindowsFileDialog
            // 
            this.chkWindowsFileDialog.AutoSize = true;
            this.chkWindowsFileDialog.Location = new System.Drawing.Point(10, 80);
            this.chkWindowsFileDialog.Name = "chkWindowsFileDialog";
            this.chkWindowsFileDialog.Size = new System.Drawing.Size(299, 17);
            this.chkWindowsFileDialog.TabIndex = 77;
            this.chkWindowsFileDialog.Text = "Use Standard Windows File Dialog for Movie file Updating";
            this.chkWindowsFileDialog.UseVisualStyleBackColor = true;
            // 
            // AntUpdDflT2
            // 
            this.AntUpdDflT2.Location = new System.Drawing.Point(231, 43);
            this.AntUpdDflT2.Name = "AntUpdDflT2";
            this.AntUpdDflT2.Size = new System.Drawing.Size(117, 20);
            this.AntUpdDflT2.TabIndex = 35;
            // 
            // AntUpdDflT1
            // 
            this.AntUpdDflT1.Location = new System.Drawing.Point(231, 18);
            this.AntUpdDflT1.Name = "AntUpdDflT1";
            this.AntUpdDflT1.Size = new System.Drawing.Size(117, 20);
            this.AntUpdDflT1.TabIndex = 32;
            // 
            // AntUpdItem2
            // 
            this.AntUpdItem2.FormattingEnabled = true;
            this.AntUpdItem2.Location = new System.Drawing.Point(10, 43);
            this.AntUpdItem2.Name = "AntUpdItem2";
            this.AntUpdItem2.Size = new System.Drawing.Size(123, 21);
            this.AntUpdItem2.TabIndex = 33;
            // 
            // AntUpdText2
            // 
            this.AntUpdText2.Location = new System.Drawing.Point(139, 44);
            this.AntUpdText2.Name = "AntUpdText2";
            this.AntUpdText2.Size = new System.Drawing.Size(86, 20);
            this.AntUpdText2.TabIndex = 34;
            // 
            // AntUpdItem1
            // 
            this.AntUpdItem1.FormattingEnabled = true;
            this.AntUpdItem1.Location = new System.Drawing.Point(10, 18);
            this.AntUpdItem1.Name = "AntUpdItem1";
            this.AntUpdItem1.Size = new System.Drawing.Size(123, 21);
            this.AntUpdItem1.TabIndex = 30;
            // 
            // AntUpdText1
            // 
            this.AntUpdText1.Location = new System.Drawing.Point(139, 19);
            this.AntUpdText1.Name = "AntUpdText1";
            this.AntUpdText1.Size = new System.Drawing.Size(86, 20);
            this.AntUpdText1.TabIndex = 31;
            // 
            // chksupplaystop
            // 
            this.chksupplaystop.AutoSize = true;
            this.chksupplaystop.Enabled = false;
            this.chksupplaystop.Location = new System.Drawing.Point(245, 41);
            this.chksupplaystop.Name = "chksupplaystop";
            this.chksupplaystop.Size = new System.Drawing.Size(93, 30);
            this.chksupplaystop.TabIndex = 19;
            this.chksupplaystop.Text = "use it when \r\nplayer finished";
            this.chksupplaystop.UseVisualStyleBackColor = true;
            this.chksupplaystop.CheckedChanged += new System.EventHandler(this.chksupplaystop_CheckedChanged);
            // 
            // gpspfield
            // 
            this.gpspfield.Controls.Add(this.txtfdupdate);
            this.gpspfield.Controls.Add(this.cbfdupdate);
            this.gpspfield.Enabled = false;
            this.gpspfield.Location = new System.Drawing.Point(230, 86);
            this.gpspfield.Name = "gpspfield";
            this.gpspfield.Size = new System.Drawing.Size(133, 71);
            this.gpspfield.TabIndex = 18;
            this.gpspfield.TabStop = false;
            this.gpspfield.Text = "Field to update/Value";
            // 
            // txtfdupdate
            // 
            this.txtfdupdate.Location = new System.Drawing.Point(15, 45);
            this.txtfdupdate.Name = "txtfdupdate";
            this.txtfdupdate.Size = new System.Drawing.Size(110, 20);
            this.txtfdupdate.TabIndex = 16;
            // 
            // cbfdupdate
            // 
            this.cbfdupdate.FormattingEnabled = true;
            this.cbfdupdate.Location = new System.Drawing.Point(15, 18);
            this.cbfdupdate.Name = "cbfdupdate";
            this.cbfdupdate.Size = new System.Drawing.Size(110, 21);
            this.cbfdupdate.TabIndex = 15;
            this.cbfdupdate.SelectedIndexChanged += new System.EventHandler(this.cbfdupdate_SelectedIndexChanged);
            // 
            // gpsuppress
            // 
            this.gpsuppress.Controls.Add(this.rbsuppress2);
            this.gpsuppress.Controls.Add(this.rbsuppress4);
            this.gpsuppress.Controls.Add(this.rbsuppress3);
            this.gpsuppress.Controls.Add(this.rbsuppress1);
            this.gpsuppress.Enabled = false;
            this.gpsuppress.Location = new System.Drawing.Point(10, 41);
            this.gpsuppress.Name = "gpsuppress";
            this.gpsuppress.Size = new System.Drawing.Size(213, 117);
            this.gpsuppress.TabIndex = 2;
            this.gpsuppress.TabStop = false;
            // 
            // rbsuppress2
            // 
            this.rbsuppress2.AutoSize = true;
            this.rbsuppress2.Location = new System.Drawing.Point(15, 41);
            this.rbsuppress2.Name = "rbsuppress2";
            this.rbsuppress2.Size = new System.Drawing.Size(189, 17);
            this.rbsuppress2.TabIndex = 5;
            this.rbsuppress2.TabStop = true;
            this.rbsuppress2.Text = "Delete both db entry and movie file";
            this.rbsuppress2.UseVisualStyleBackColor = true;
            // 
            // rbsuppress4
            // 
            this.rbsuppress4.AutoSize = true;
            this.rbsuppress4.Location = new System.Drawing.Point(15, 87);
            this.rbsuppress4.Name = "rbsuppress4";
            this.rbsuppress4.Size = new System.Drawing.Size(201, 17);
            this.rbsuppress4.TabIndex = 4;
            this.rbsuppress4.TabStop = true;
            this.rbsuppress4.Text = "Update db entry and delete movie file";
            this.rbsuppress4.UseVisualStyleBackColor = true;
            this.rbsuppress4.CheckedChanged += new System.EventHandler(this.rbsuppress_CheckedChanged);
            // 
            // rbsuppress3
            // 
            this.rbsuppress3.AutoSize = true;
            this.rbsuppress3.Location = new System.Drawing.Point(15, 64);
            this.rbsuppress3.Name = "rbsuppress3";
            this.rbsuppress3.Size = new System.Drawing.Size(196, 17);
            this.rbsuppress3.TabIndex = 2;
            this.rbsuppress3.TabStop = true;
            this.rbsuppress3.Text = "Update db entry and keep movie file";
            this.rbsuppress3.UseVisualStyleBackColor = true;
            this.rbsuppress3.CheckedChanged += new System.EventHandler(this.rbsuppress_CheckedChanged);
            // 
            // rbsuppress1
            // 
            this.rbsuppress1.AutoSize = true;
            this.rbsuppress1.Location = new System.Drawing.Point(15, 19);
            this.rbsuppress1.Name = "rbsuppress1";
            this.rbsuppress1.Size = new System.Drawing.Size(119, 17);
            this.rbsuppress1.TabIndex = 1;
            this.rbsuppress1.TabStop = true;
            this.rbsuppress1.Text = "Delete db entry only";
            this.rbsuppress1.UseVisualStyleBackColor = true;
            // 
            // chkSuppress
            // 
            this.chkSuppress.AutoSize = true;
            this.chkSuppress.Location = new System.Drawing.Point(10, 19);
            this.chkSuppress.Name = "chkSuppress";
            this.chkSuppress.Size = new System.Drawing.Size(183, 17);
            this.chkSuppress.TabIndex = 0;
            this.chkSuppress.Text = "Enable database deletion options";
            this.chkSuppress.UseVisualStyleBackColor = true;
            this.chkSuppress.CheckedChanged += new System.EventHandler(this.chkSuppress_CheckedChanged);
            // 
            // groupBox_Separators
            // 
            this.groupBox_Separators.Controls.Add(this.label25);
            this.groupBox_Separators.Controls.Add(this.label23);
            this.groupBox_Separators.Controls.Add(this.RoleSeparator5);
            this.groupBox_Separators.Controls.Add(this.RoleSeparator4);
            this.groupBox_Separators.Controls.Add(this.ListSeparator5);
            this.groupBox_Separators.Controls.Add(this.ListSeparator4);
            this.groupBox_Separators.Controls.Add(this.ListSeparator3);
            this.groupBox_Separators.Controls.Add(this.ListSeparator2);
            this.groupBox_Separators.Controls.Add(this.RoleSeparator2);
            this.groupBox_Separators.Controls.Add(this.RoleSeparator3);
            this.groupBox_Separators.Controls.Add(this.label22);
            this.groupBox_Separators.Controls.Add(this.RoleSeparator1);
            this.groupBox_Separators.Controls.Add(this.label24);
            this.groupBox_Separators.Controls.Add(this.ListSeparator1);
            this.groupBox_Separators.Location = new System.Drawing.Point(391, 6);
            this.groupBox_Separators.Name = "groupBox_Separators";
            this.groupBox_Separators.Size = new System.Drawing.Size(350, 125);
            this.groupBox_Separators.TabIndex = 27;
            this.groupBox_Separators.TabStop = false;
            this.groupBox_Separators.Text = "Separators";
            this.ToolTip1.SetToolTip(this.groupBox_Separators, resources.GetString("groupBox_Separators.ToolTip"));
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(18, 91);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(87, 13);
            this.label25.TabIndex = 84;
            this.label25.Text = "for Actors search";
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(18, 41);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(100, 26);
            this.label23.TabIndex = 83;
            this.label23.Text = "for Genre, Country and Actors views";
            // 
            // RoleSeparator5
            // 
            this.RoleSeparator5.FormattingEnabled = true;
            this.RoleSeparator5.Items.AddRange(new object[] {
            " as ",
            "(",
            "....",
            ")"});
            this.RoleSeparator5.Location = new System.Drawing.Point(306, 78);
            this.RoleSeparator5.Name = "RoleSeparator5";
            this.RoleSeparator5.Size = new System.Drawing.Size(40, 21);
            this.RoleSeparator5.TabIndex = 82;
            this.RoleSeparator5.Text = "List";
            // 
            // RoleSeparator4
            // 
            this.RoleSeparator4.FormattingEnabled = true;
            this.RoleSeparator4.Items.AddRange(new object[] {
            " as ",
            "(",
            "....",
            ")"});
            this.RoleSeparator4.Location = new System.Drawing.Point(260, 78);
            this.RoleSeparator4.Name = "RoleSeparator4";
            this.RoleSeparator4.Size = new System.Drawing.Size(40, 21);
            this.RoleSeparator4.TabIndex = 81;
            this.RoleSeparator4.Text = "List";
            // 
            // ListSeparator5
            // 
            this.ListSeparator5.FormattingEnabled = true;
            this.ListSeparator5.Items.AddRange(new object[] {
            ",",
            ";",
            "|"});
            this.ListSeparator5.Location = new System.Drawing.Point(306, 28);
            this.ListSeparator5.Name = "ListSeparator5";
            this.ListSeparator5.Size = new System.Drawing.Size(40, 21);
            this.ListSeparator5.TabIndex = 80;
            this.ListSeparator5.Text = "List";
            // 
            // ListSeparator4
            // 
            this.ListSeparator4.FormattingEnabled = true;
            this.ListSeparator4.Items.AddRange(new object[] {
            ",",
            ";",
            "|"});
            this.ListSeparator4.Location = new System.Drawing.Point(260, 28);
            this.ListSeparator4.Name = "ListSeparator4";
            this.ListSeparator4.Size = new System.Drawing.Size(40, 21);
            this.ListSeparator4.TabIndex = 79;
            this.ListSeparator4.Text = "List";
            // 
            // ListSeparator3
            // 
            this.ListSeparator3.FormattingEnabled = true;
            this.ListSeparator3.Items.AddRange(new object[] {
            ",",
            ";",
            "|"});
            this.ListSeparator3.Location = new System.Drawing.Point(214, 28);
            this.ListSeparator3.Name = "ListSeparator3";
            this.ListSeparator3.Size = new System.Drawing.Size(40, 21);
            this.ListSeparator3.TabIndex = 78;
            this.ListSeparator3.Text = "List";
            // 
            // ListSeparator2
            // 
            this.ListSeparator2.FormattingEnabled = true;
            this.ListSeparator2.Items.AddRange(new object[] {
            ",",
            ";",
            "|"});
            this.ListSeparator2.Location = new System.Drawing.Point(168, 28);
            this.ListSeparator2.Name = "ListSeparator2";
            this.ListSeparator2.Size = new System.Drawing.Size(40, 21);
            this.ListSeparator2.TabIndex = 77;
            this.ListSeparator2.Text = "List";
            // 
            // RoleSeparator2
            // 
            this.RoleSeparator2.FormattingEnabled = true;
            this.RoleSeparator2.Items.AddRange(new object[] {
            " as ",
            "(",
            ")",
            "...."});
            this.RoleSeparator2.Location = new System.Drawing.Point(168, 78);
            this.RoleSeparator2.Name = "RoleSeparator2";
            this.RoleSeparator2.Size = new System.Drawing.Size(40, 21);
            this.RoleSeparator2.Sorted = true;
            this.RoleSeparator2.TabIndex = 76;
            this.RoleSeparator2.Text = "List";
            // 
            // RoleSeparator3
            // 
            this.RoleSeparator3.FormattingEnabled = true;
            this.RoleSeparator3.Items.AddRange(new object[] {
            " as ",
            "(",
            ")",
            "...."});
            this.RoleSeparator3.Location = new System.Drawing.Point(214, 78);
            this.RoleSeparator3.Name = "RoleSeparator3";
            this.RoleSeparator3.Size = new System.Drawing.Size(40, 21);
            this.RoleSeparator3.Sorted = true;
            this.RoleSeparator3.TabIndex = 75;
            this.RoleSeparator3.Text = "List";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(9, 78);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(107, 13);
            this.label22.TabIndex = 74;
            this.label22.Text = "Role Text Separators";
            // 
            // RoleSeparator1
            // 
            this.RoleSeparator1.FormattingEnabled = true;
            this.RoleSeparator1.Items.AddRange(new object[] {
            " as ",
            "(",
            ")",
            "...."});
            this.RoleSeparator1.Location = new System.Drawing.Point(122, 78);
            this.RoleSeparator1.Name = "RoleSeparator1";
            this.RoleSeparator1.Size = new System.Drawing.Size(40, 21);
            this.RoleSeparator1.Sorted = true;
            this.RoleSeparator1.TabIndex = 73;
            this.RoleSeparator1.Text = "List";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(9, 28);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(72, 13);
            this.label24.TabIndex = 71;
            this.label24.Text = "List Separator";
            // 
            // ListSeparator1
            // 
            this.ListSeparator1.FormattingEnabled = true;
            this.ListSeparator1.Items.AddRange(new object[] {
            ",",
            ";",
            "|"});
            this.ListSeparator1.Location = new System.Drawing.Point(122, 28);
            this.ListSeparator1.Name = "ListSeparator1";
            this.ListSeparator1.Size = new System.Drawing.Size(40, 21);
            this.ListSeparator1.TabIndex = 70;
            this.ListSeparator1.Text = "List";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.radioButton2);
            this.groupBox9.Controls.Add(this.radioButton1);
            this.groupBox9.Controls.Add(this.DVDPTagField);
            this.groupBox9.Location = new System.Drawing.Point(34, 34);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(327, 92);
            this.groupBox9.TabIndex = 0;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Use Tags";
            this.ToolTip1.SetToolTip(this.groupBox9, "With those options, you can use specifics Tags DVDP\'s Fields\r\nand store either wi" +
                    "th Genre field either in another field\r\nof the generated DB.");
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(24, 49);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(96, 17);
            this.radioButton2.TabIndex = 13;
            this.radioButton2.Text = "Store in  Field :";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(24, 24);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(138, 17);
            this.radioButton1.TabIndex = 12;
            this.radioButton1.Text = "Merge With Genre Field";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // DVDPTagField
            // 
            this.DVDPTagField.FormattingEnabled = true;
            this.DVDPTagField.Location = new System.Drawing.Point(132, 47);
            this.DVDPTagField.Name = "DVDPTagField";
            this.DVDPTagField.Size = new System.Drawing.Size(171, 21);
            this.DVDPTagField.TabIndex = 11;
            // 
            // groupBox20
            // 
            this.groupBox20.BackColor = System.Drawing.Color.Transparent;
            this.groupBox20.Controls.Add(this.scheduleAMCUpdater);
            this.groupBox20.Controls.Add(this.btnParameters);
            this.groupBox20.Controls.Add(this.btnAMCUpd_cnf);
            this.groupBox20.Controls.Add(this.txtAMCUpd_cnf);
            this.groupBox20.Controls.Add(this.label28);
            this.groupBox20.Controls.Add(this.btnAMCUpd_exe);
            this.groupBox20.Controls.Add(this.txtAMCUpd_exe);
            this.groupBox20.Controls.Add(this.label26);
            this.groupBox20.Controls.Add(this.chkAMCUpd);
            this.groupBox20.Location = new System.Drawing.Point(21, 144);
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.Size = new System.Drawing.Size(693, 93);
            this.groupBox20.TabIndex = 2;
            this.groupBox20.TabStop = false;
            this.groupBox20.Text = "AMCUpdater Options";
            this.ToolTip1.SetToolTip(this.groupBox20, "For updating the whole database and add automaticly new folders entries,\r\nyou can" +
                    " use and launch AMCUpdater in batch mode.\r\nYou have to define the path to the ex" +
                    "e file and the parameter XML file.");
            // 
            // scheduleAMCUpdater
            // 
            this.scheduleAMCUpdater.AutoSize = true;
            this.scheduleAMCUpdater.Location = new System.Drawing.Point(23, 63);
            this.scheduleAMCUpdater.Name = "scheduleAMCUpdater";
            this.scheduleAMCUpdater.Size = new System.Drawing.Size(138, 17);
            this.scheduleAMCUpdater.TabIndex = 61;
            this.scheduleAMCUpdater.Text = "Schedule AMCUpdater ";
            this.scheduleAMCUpdater.UseVisualStyleBackColor = true;
            this.scheduleAMCUpdater.CheckedChanged += new System.EventHandler(this.scheduleAMCUpdater_CheckedChanged);
            // 
            // btnParameters
            // 
            this.btnParameters.Location = new System.Drawing.Point(167, 57);
            this.btnParameters.Name = "btnParameters";
            this.btnParameters.Size = new System.Drawing.Size(97, 30);
            this.btnParameters.TabIndex = 60;
            this.btnParameters.Text = "Task Parameters";
            this.btnParameters.UseVisualStyleBackColor = true;
            this.btnParameters.Click += new System.EventHandler(this.btnParameters_Click);
            // 
            // btnAMCUpd_cnf
            // 
            this.btnAMCUpd_cnf.Enabled = false;
            this.btnAMCUpd_cnf.Location = new System.Drawing.Point(646, 44);
            this.btnAMCUpd_cnf.Name = "btnAMCUpd_cnf";
            this.btnAMCUpd_cnf.Size = new System.Drawing.Size(32, 23);
            this.btnAMCUpd_cnf.TabIndex = 58;
            this.btnAMCUpd_cnf.Text = "...";
            this.btnAMCUpd_cnf.UseVisualStyleBackColor = true;
            this.btnAMCUpd_cnf.Click += new System.EventHandler(this.btnAMCUpd_cnf_Click);
            // 
            // txtAMCUpd_cnf
            // 
            this.txtAMCUpd_cnf.Enabled = false;
            this.txtAMCUpd_cnf.Location = new System.Drawing.Point(406, 46);
            this.txtAMCUpd_cnf.Name = "txtAMCUpd_cnf";
            this.txtAMCUpd_cnf.Size = new System.Drawing.Size(234, 20);
            this.txtAMCUpd_cnf.TabIndex = 57;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(305, 50);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(87, 13);
            this.label28.TabIndex = 56;
            this.label28.Text = "Config File (XML)";
            // 
            // btnAMCUpd_exe
            // 
            this.btnAMCUpd_exe.Enabled = false;
            this.btnAMCUpd_exe.Location = new System.Drawing.Point(646, 17);
            this.btnAMCUpd_exe.Name = "btnAMCUpd_exe";
            this.btnAMCUpd_exe.Size = new System.Drawing.Size(32, 23);
            this.btnAMCUpd_exe.TabIndex = 55;
            this.btnAMCUpd_exe.Text = "...";
            this.btnAMCUpd_exe.UseVisualStyleBackColor = true;
            this.btnAMCUpd_exe.Click += new System.EventHandler(this.btnAMCUpd_exe_Click);
            // 
            // txtAMCUpd_exe
            // 
            this.txtAMCUpd_exe.Enabled = false;
            this.txtAMCUpd_exe.Location = new System.Drawing.Point(406, 19);
            this.txtAMCUpd_exe.Name = "txtAMCUpd_exe";
            this.txtAMCUpd_exe.Size = new System.Drawing.Size(234, 20);
            this.txtAMCUpd_exe.TabIndex = 54;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(305, 23);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(88, 13);
            this.label26.TabIndex = 53;
            this.label26.Text = "AMCUpdater.exe";
            // 
            // chkAMCUpd
            // 
            this.chkAMCUpd.AutoSize = true;
            this.chkAMCUpd.Location = new System.Drawing.Point(23, 17);
            this.chkAMCUpd.Name = "chkAMCUpd";
            this.chkAMCUpd.Size = new System.Drawing.Size(199, 30);
            this.chkAMCUpd.TabIndex = 0;
            this.chkAMCUpd.Text = "AMCUpdater used \r\n(for global  Internet update functions)";
            this.chkAMCUpd.UseVisualStyleBackColor = true;
            this.chkAMCUpd.CheckedChanged += new System.EventHandler(this.chkAMCUpd_CheckedChanged);
            // 
            // groupBox_GrabberOptions
            // 
            this.groupBox_GrabberOptions.Controls.Add(this.chkGrabber_ChooseScript);
            this.groupBox_GrabberOptions.Controls.Add(this.btnDirGrab);
            this.groupBox_GrabberOptions.Controls.Add(this.txtDirGrab);
            this.groupBox_GrabberOptions.Controls.Add(this.label31);
            this.groupBox_GrabberOptions.Controls.Add(this.chkGrabber_Always);
            this.groupBox_GrabberOptions.Controls.Add(this.btnGrabber);
            this.groupBox_GrabberOptions.Controls.Add(this.txtGrabber);
            this.groupBox_GrabberOptions.Controls.Add(this.label27);
            this.groupBox_GrabberOptions.Controls.Add(this.chkGrabber);
            this.groupBox_GrabberOptions.Location = new System.Drawing.Point(21, 10);
            this.groupBox_GrabberOptions.Name = "groupBox_GrabberOptions";
            this.groupBox_GrabberOptions.Size = new System.Drawing.Size(693, 128);
            this.groupBox_GrabberOptions.TabIndex = 1;
            this.groupBox_GrabberOptions.TabStop = false;
            this.groupBox_GrabberOptions.Text = "Grabber Options";
            this.ToolTip1.SetToolTip(this.groupBox_GrabberOptions, resources.GetString("groupBox_GrabberOptions.ToolTip"));
            // 
            // chkGrabber_ChooseScript
            // 
            this.chkGrabber_ChooseScript.AutoSize = true;
            this.chkGrabber_ChooseScript.Location = new System.Drawing.Point(406, 82);
            this.chkGrabber_ChooseScript.Name = "chkGrabber_ChooseScript";
            this.chkGrabber_ChooseScript.Size = new System.Drawing.Size(127, 17);
            this.chkGrabber_ChooseScript.TabIndex = 60;
            this.chkGrabber_ChooseScript.Text = "always use that script";
            this.chkGrabber_ChooseScript.UseVisualStyleBackColor = true;
            // 
            // btnDirGrab
            // 
            this.btnDirGrab.Location = new System.Drawing.Point(646, 17);
            this.btnDirGrab.Name = "btnDirGrab";
            this.btnDirGrab.Size = new System.Drawing.Size(32, 23);
            this.btnDirGrab.TabIndex = 59;
            this.btnDirGrab.Text = "...";
            this.btnDirGrab.UseVisualStyleBackColor = true;
            this.btnDirGrab.Click += new System.EventHandler(this.btnDirGrab_Click);
            // 
            // txtDirGrab
            // 
            this.txtDirGrab.Enabled = false;
            this.txtDirGrab.Location = new System.Drawing.Point(406, 19);
            this.txtDirGrab.Name = "txtDirGrab";
            this.txtDirGrab.Size = new System.Drawing.Size(234, 20);
            this.txtDirGrab.TabIndex = 58;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(255, 22);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(145, 13);
            this.label31.TabIndex = 57;
            this.label31.Text = "Directory Grabber Files (XML)";
            // 
            // chkGrabber_Always
            // 
            this.chkGrabber_Always.AutoSize = true;
            this.chkGrabber_Always.Location = new System.Drawing.Point(406, 105);
            this.chkGrabber_Always.Name = "chkGrabber_Always";
            this.chkGrabber_Always.Size = new System.Drawing.Size(188, 17);
            this.chkGrabber_Always.TabIndex = 56;
            this.chkGrabber_Always.Text = "try to find best match automatically";
            this.chkGrabber_Always.UseVisualStyleBackColor = true;
            // 
            // btnGrabber
            // 
            this.btnGrabber.Enabled = false;
            this.btnGrabber.Location = new System.Drawing.Point(646, 50);
            this.btnGrabber.Name = "btnGrabber";
            this.btnGrabber.Size = new System.Drawing.Size(32, 23);
            this.btnGrabber.TabIndex = 55;
            this.btnGrabber.Text = "...";
            this.btnGrabber.UseVisualStyleBackColor = true;
            this.btnGrabber.Click += new System.EventHandler(this.btnGrabber_Click);
            // 
            // txtGrabber
            // 
            this.txtGrabber.Enabled = false;
            this.txtGrabber.Location = new System.Drawing.Point(406, 52);
            this.txtGrabber.Name = "txtGrabber";
            this.txtGrabber.Size = new System.Drawing.Size(234, 20);
            this.txtGrabber.TabIndex = 54;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(305, 56);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(95, 13);
            this.label27.TabIndex = 53;
            this.label27.Text = "Grabber File (XML)";
            // 
            // chkGrabber
            // 
            this.chkGrabber.AutoSize = true;
            this.chkGrabber.Location = new System.Drawing.Point(23, 23);
            this.chkGrabber.Name = "chkGrabber";
            this.chkGrabber.Size = new System.Drawing.Size(183, 30);
            this.chkGrabber.TabIndex = 0;
            this.chkGrabber.Text = "Grabber used \r\n(for Movie Detail Internet Update)";
            this.chkGrabber.UseVisualStyleBackColor = true;
            this.chkGrabber.CheckedChanged += new System.EventHandler(this.chkGrabber_CheckedChanged);
            // 
            // Fanart
            // 
            this.Fanart.Controls.Add(this.chkFanart);
            this.Fanart.Controls.Add(this.btnFanart);
            this.Fanart.Controls.Add(this.MesFilmsFanart);
            this.Fanart.Controls.Add(this.labelFanart);
            this.Fanart.Controls.Add(this.chkDfltFanart);
            this.Fanart.Location = new System.Drawing.Point(17, 188);
            this.Fanart.Name = "Fanart";
            this.Fanart.Size = new System.Drawing.Size(710, 70);
            this.Fanart.TabIndex = 71;
            this.Fanart.TabStop = false;
            this.Fanart.Text = "Fanart (Backdrops)";
            this.ToolTip1.SetToolTip(this.Fanart, resources.GetString("Fanart.ToolTip"));
            // 
            // chkFanart
            // 
            this.chkFanart.AutoSize = true;
            this.chkFanart.Location = new System.Drawing.Point(18, 19);
            this.chkFanart.Name = "chkFanart";
            this.chkFanart.Size = new System.Drawing.Size(78, 17);
            this.chkFanart.TabIndex = 71;
            this.chkFanart.Text = "Use Fanart";
            this.chkFanart.UseVisualStyleBackColor = true;
            this.chkFanart.CheckedChanged += new System.EventHandler(this.chkFanart_CheckedChanged);
            // 
            // btnFanart
            // 
            this.btnFanart.Enabled = false;
            this.btnFanart.Location = new System.Drawing.Point(656, 14);
            this.btnFanart.Name = "btnFanart";
            this.btnFanart.Size = new System.Drawing.Size(32, 23);
            this.btnFanart.TabIndex = 70;
            this.btnFanart.Text = "...";
            this.btnFanart.UseVisualStyleBackColor = true;
            this.btnFanart.Click += new System.EventHandler(this.btnFanart_Click);
            // 
            // MesFilmsFanart
            // 
            this.MesFilmsFanart.Enabled = false;
            this.MesFilmsFanart.Location = new System.Drawing.Point(315, 17);
            this.MesFilmsFanart.Name = "MesFilmsFanart";
            this.MesFilmsFanart.Size = new System.Drawing.Size(315, 20);
            this.MesFilmsFanart.TabIndex = 68;
            this.MesFilmsFanart.TextChanged += new System.EventHandler(this.MesFilmsFanart_TextChanged);
            // 
            // labelFanart
            // 
            this.labelFanart.AutoSize = true;
            this.labelFanart.Location = new System.Drawing.Point(162, 20);
            this.labelFanart.Name = "labelFanart";
            this.labelFanart.Size = new System.Drawing.Size(147, 13);
            this.labelFanart.TabIndex = 69;
            this.labelFanart.Text = "Fanart Picture Path  (Images) ";
            // 
            // chkDfltFanart
            // 
            this.chkDfltFanart.AutoSize = true;
            this.chkDfltFanart.Enabled = false;
            this.chkDfltFanart.Location = new System.Drawing.Point(42, 42);
            this.chkDfltFanart.Name = "chkDfltFanart";
            this.chkDfltFanart.Size = new System.Drawing.Size(244, 17);
            this.chkDfltFanart.TabIndex = 67;
            this.chkDfltFanart.Text = "Use the default movie cover for missing Fanart\r\n";
            this.chkDfltFanart.UseVisualStyleBackColor = true;
            // 
            // txtLogosPath
            // 
            this.txtLogosPath.Enabled = false;
            this.txtLogosPath.Location = new System.Drawing.Point(240, 8);
            this.txtLogosPath.Name = "txtLogosPath";
            this.txtLogosPath.Size = new System.Drawing.Size(449, 20);
            this.txtLogosPath.TabIndex = 92;
            this.ToolTip1.SetToolTip(this.txtLogosPath, resources.GetString("txtLogosPath.ToolTip"));
            // 
            // chkLogos
            // 
            this.chkLogos.AutoSize = true;
            this.chkLogos.Location = new System.Drawing.Point(27, 10);
            this.chkLogos.Name = "chkLogos";
            this.chkLogos.Size = new System.Drawing.Size(91, 17);
            this.chkLogos.TabIndex = 74;
            this.chkLogos.Text = "Enable Logos";
            this.ToolTip1.SetToolTip(this.chkLogos, "Select this option if you want the following logos to be displayed for selected i" +
                    "tems.");
            this.chkLogos.UseVisualStyleBackColor = true;
            this.chkLogos.CheckedChanged += new System.EventHandler(this.chkLogos_CheckedChanged);
            // 
            // SLogo_Type
            // 
            this.SLogo_Type.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.SLogo_Type.Enabled = false;
            this.SLogo_Type.FormattingEnabled = true;
            this.SLogo_Type.Items.AddRange(new object[] {
            "ID2001",
            "ID2002"});
            this.SLogo_Type.Location = new System.Drawing.Point(9, 289);
            this.SLogo_Type.Name = "SLogo_Type";
            this.SLogo_Type.Size = new System.Drawing.Size(102, 21);
            this.SLogo_Type.TabIndex = 73;
            this.ToolTip1.SetToolTip(this.SLogo_Type, resources.GetString("SLogo_Type.ToolTip"));
            // 
            // ButQuit
            // 
            this.ButQuit.Location = new System.Drawing.Point(679, 452);
            this.ButQuit.Name = "ButQuit";
            this.ButQuit.Size = new System.Drawing.Size(64, 31);
            this.ButQuit.TabIndex = 40;
            this.ButQuit.Text = "Quit";
            this.ToolTip1.SetToolTip(this.ButQuit, "No comments...");
            this.ButQuit.UseVisualStyleBackColor = true;
            this.ButQuit.Click += new System.EventHandler(this.ButQuit_Click);
            // 
            // ButDelet
            // 
            this.ButDelet.Location = new System.Drawing.Point(599, 452);
            this.ButDelet.Name = "ButDelet";
            this.ButDelet.Size = new System.Drawing.Size(64, 31);
            this.ButDelet.TabIndex = 39;
            this.ButDelet.Text = "Delete";
            this.ToolTip1.SetToolTip(this.ButDelet, "Delete all informations for that configuration.");
            this.ButDelet.UseVisualStyleBackColor = true;
            this.ButDelet.Click += new System.EventHandler(this.ButDelet_Click);
            // 
            // Config_Dflt
            // 
            this.Config_Dflt.AutoSize = true;
            this.Config_Dflt.Location = new System.Drawing.Point(460, 13);
            this.Config_Dflt.Name = "Config_Dflt";
            this.Config_Dflt.Size = new System.Drawing.Size(125, 17);
            this.Config_Dflt.TabIndex = 73;
            this.Config_Dflt.Text = "Default Configuration";
            this.ToolTip1.SetToolTip(this.Config_Dflt, resources.GetString("Config_Dflt.ToolTip"));
            this.Config_Dflt.UseVisualStyleBackColor = true;
            // 
            // Config_Menu
            // 
            this.Config_Menu.AutoSize = true;
            this.Config_Menu.Location = new System.Drawing.Point(460, 41);
            this.Config_Menu.Name = "Config_Menu";
            this.Config_Menu.Size = new System.Drawing.Size(297, 17);
            this.Config_Menu.TabIndex = 74;
            this.Config_Menu.Text = "Display Always Configuration\'s Menu (if no Default Config)";
            this.ToolTip1.SetToolTip(this.Config_Menu, resources.GetString("Config_Menu.ToolTip"));
            this.Config_Menu.UseVisualStyleBackColor = true;
            this.Config_Menu.CheckedChanged += new System.EventHandler(this.Config_Menu_CheckedChanged);
            // 
            // SearchFileName
            // 
            this.SearchFileName.AutoSize = true;
            this.SearchFileName.Location = new System.Drawing.Point(506, 39);
            this.SearchFileName.Name = "SearchFileName";
            this.SearchFileName.Size = new System.Drawing.Size(93, 30);
            this.SearchFileName.TabIndex = 66;
            this.SearchFileName.Text = "Search by \r\nMovie\'s Name";
            this.ToolTip1.SetToolTip(this.SearchFileName, "If file is not found with the \'Ant Item for Storage Info\' field, you \r\ncan search" +
                    " the file with the movie Name.\r\nValidate that option for it and give the Title u" +
                    "sed for the search.\r\n");
            this.SearchFileName.UseVisualStyleBackColor = true;
            // 
            // AntIdentItem
            // 
            this.AntIdentItem.FormattingEnabled = true;
            this.AntIdentItem.Location = new System.Drawing.Point(172, 70);
            this.AntIdentItem.Name = "AntIdentItem";
            this.AntIdentItem.Size = new System.Drawing.Size(157, 21);
            this.AntIdentItem.TabIndex = 56;
            this.ToolTip1.SetToolTip(this.AntIdentItem, resources.GetString("AntIdentItem.ToolTip"));
            // 
            // AntSTitle
            // 
            this.AntSTitle.FormattingEnabled = true;
            this.AntSTitle.Location = new System.Drawing.Point(106, 70);
            this.AntSTitle.Name = "AntSTitle";
            this.AntSTitle.Size = new System.Drawing.Size(181, 21);
            this.AntSTitle.TabIndex = 70;
            this.ToolTip1.SetToolTip(this.AntSTitle, "Select the ANT database field that you want to be used in the ‘Sort by name’ fiel" +
                    "d in the MediaPortal menu.");
            // 
            // TitleDelim
            // 
            this.TitleDelim.Location = new System.Drawing.Point(401, 18);
            this.TitleDelim.MaxLength = 1;
            this.TitleDelim.Name = "TitleDelim";
            this.TitleDelim.Size = new System.Drawing.Size(20, 20);
            this.TitleDelim.TabIndex = 20;
            this.ToolTip1.SetToolTip(this.TitleDelim, resources.GetString("TitleDelim.ToolTip"));
            // 
            // AntTitle2
            // 
            this.AntTitle2.FormattingEnabled = true;
            this.AntTitle2.Location = new System.Drawing.Point(106, 43);
            this.AntTitle2.Name = "AntTitle2";
            this.AntTitle2.Size = new System.Drawing.Size(181, 21);
            this.AntTitle2.TabIndex = 11;
            this.ToolTip1.SetToolTip(this.AntTitle2, "Select the ANT database field that you want to be displayed as the ‘Alternate Tit" +
                    "le’.");
            // 
            // AntTitle1
            // 
            this.AntTitle1.FormattingEnabled = true;
            this.AntTitle1.Items.AddRange(new object[] {
            "OriginalTitle",
            "TranslatedTitle",
            "FormattedTitle"});
            this.AntTitle1.Location = new System.Drawing.Point(106, 16);
            this.AntTitle1.Name = "AntTitle1";
            this.AntTitle1.Size = new System.Drawing.Size(181, 21);
            this.AntTitle1.TabIndex = 10;
            this.ToolTip1.SetToolTip(this.AntTitle1, "Select the ANT database field that you want to be displayed as the ‘Master Title’" +
                    ".");
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.btnResetThumbs);
            this.groupBox22.Controls.Add(this.chkViews);
            this.groupBox22.Controls.Add(this.btnViews);
            this.groupBox22.Controls.Add(this.chkDfltViews);
            this.groupBox22.Controls.Add(this.label29);
            this.groupBox22.Controls.Add(this.MesFilmsViews);
            this.groupBox22.Location = new System.Drawing.Point(17, 264);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(710, 84);
            this.groupBox22.TabIndex = 75;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Thumbs for Grouped Views (Genre, Year, Country, ...)";
            this.ToolTip1.SetToolTip(this.groupBox22, resources.GetString("groupBox22.ToolTip"));
            // 
            // btnResetThumbs
            // 
            this.btnResetThumbs.Location = new System.Drawing.Point(315, 50);
            this.btnResetThumbs.Name = "btnResetThumbs";
            this.btnResetThumbs.Size = new System.Drawing.Size(84, 23);
            this.btnResetThumbs.TabIndex = 76;
            this.btnResetThumbs.Text = "Reset Thumbs";
            this.ToolTip1.SetToolTip(this.btnResetThumbs, "That Action\'ll remove all generated Thumbs");
            this.btnResetThumbs.UseVisualStyleBackColor = true;
            this.btnResetThumbs.Click += new System.EventHandler(this.btnResetThumbs_Click);
            // 
            // chkViews
            // 
            this.chkViews.AutoSize = true;
            this.chkViews.Location = new System.Drawing.Point(18, 19);
            this.chkViews.Name = "chkViews";
            this.chkViews.Size = new System.Drawing.Size(104, 30);
            this.chkViews.TabIndex = 71;
            this.chkViews.Text = "Use Thumbs for \r\ngrouped views";
            this.chkViews.UseVisualStyleBackColor = true;
            // 
            // btnViews
            // 
            this.btnViews.Location = new System.Drawing.Point(656, 22);
            this.btnViews.Name = "btnViews";
            this.btnViews.Size = new System.Drawing.Size(32, 23);
            this.btnViews.TabIndex = 74;
            this.btnViews.Text = "...";
            this.btnViews.UseVisualStyleBackColor = true;
            this.btnViews.Click += new System.EventHandler(this.btnViews_Click);
            // 
            // chkDfltViews
            // 
            this.chkDfltViews.AutoSize = true;
            this.chkDfltViews.Location = new System.Drawing.Point(42, 54);
            this.chkDfltViews.Name = "chkDfltViews";
            this.chkDfltViews.Size = new System.Drawing.Size(252, 17);
            this.chkDfltViews.TabIndex = 75;
            this.chkDfltViews.Text = "Use the default movie cover for missing Thumbs";
            this.chkDfltViews.UseVisualStyleBackColor = true;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(162, 27);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(140, 13);
            this.label29.TabIndex = 73;
            this.label29.Text = "Grouped Views Picture Path";
            // 
            // MesFilmsViews
            // 
            this.MesFilmsViews.Location = new System.Drawing.Point(315, 24);
            this.MesFilmsViews.Name = "MesFilmsViews";
            this.MesFilmsViews.Size = new System.Drawing.Size(315, 20);
            this.MesFilmsViews.TabIndex = 72;
            // 
            // groupBox_SupplementarySearch
            // 
            this.groupBox_SupplementarySearch.Controls.Add(this.AntSearchFieldReset);
            this.groupBox_SupplementarySearch.Controls.Add(this.label18);
            this.groupBox_SupplementarySearch.Controls.Add(this.AntSearchList);
            this.groupBox_SupplementarySearch.Controls.Add(this.AntSearchItem2);
            this.groupBox_SupplementarySearch.Controls.Add(this.AntSearchField);
            this.groupBox_SupplementarySearch.Controls.Add(this.AntSearchText2);
            this.groupBox_SupplementarySearch.Controls.Add(this.AntSearchItem1);
            this.groupBox_SupplementarySearch.Controls.Add(this.AntSearchText1);
            this.groupBox_SupplementarySearch.Location = new System.Drawing.Point(391, 170);
            this.groupBox_SupplementarySearch.Name = "groupBox_SupplementarySearch";
            this.groupBox_SupplementarySearch.Size = new System.Drawing.Size(350, 178);
            this.groupBox_SupplementarySearch.TabIndex = 33;
            this.groupBox_SupplementarySearch.TabStop = false;
            this.groupBox_SupplementarySearch.Text = "Supplementary Search";
            this.ToolTip1.SetToolTip(this.groupBox_SupplementarySearch, resources.GetString("groupBox_SupplementarySearch.ToolTip"));
            // 
            // AntSearchFieldReset
            // 
            this.AntSearchFieldReset.Location = new System.Drawing.Point(286, 113);
            this.AntSearchFieldReset.Name = "AntSearchFieldReset";
            this.AntSearchFieldReset.Size = new System.Drawing.Size(51, 23);
            this.AntSearchFieldReset.TabIndex = 85;
            this.AntSearchFieldReset.Text = "Reset";
            this.ToolTip1.SetToolTip(this.AntSearchFieldReset, "That Action\'ll clear the field.");
            this.AntSearchFieldReset.UseVisualStyleBackColor = true;
            this.AntSearchFieldReset.Click += new System.EventHandler(this.AntSearchFieldReset_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 118);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(152, 13);
            this.label18.TabIndex = 80;
            this.label18.Text = "Search by Properties Selection";
            // 
            // AntSearchList
            // 
            this.AntSearchList.BackColor = System.Drawing.SystemColors.Control;
            this.AntSearchList.Location = new System.Drawing.Point(9, 142);
            this.AntSearchList.Name = "AntSearchList";
            this.AntSearchList.Size = new System.Drawing.Size(328, 20);
            this.AntSearchList.TabIndex = 79;
            // 
            // AntSearchItem2
            // 
            this.AntSearchItem2.FormattingEnabled = true;
            this.AntSearchItem2.Location = new System.Drawing.Point(10, 45);
            this.AntSearchItem2.Name = "AntSearchItem2";
            this.AntSearchItem2.Size = new System.Drawing.Size(145, 21);
            this.AntSearchItem2.TabIndex = 38;
            // 
            // AntSearchField
            // 
            this.AntSearchField.FormattingEnabled = true;
            this.AntSearchField.Location = new System.Drawing.Point(163, 115);
            this.AntSearchField.Name = "AntSearchField";
            this.AntSearchField.Size = new System.Drawing.Size(113, 21);
            this.AntSearchField.TabIndex = 78;
            this.AntSearchField.SelectedIndexChanged += new System.EventHandler(this.AntSearchField_SelectedIndexChanged);
            // 
            // AntSearchText2
            // 
            this.AntSearchText2.Location = new System.Drawing.Point(164, 46);
            this.AntSearchText2.Name = "AntSearchText2";
            this.AntSearchText2.Size = new System.Drawing.Size(173, 20);
            this.AntSearchText2.TabIndex = 39;
            // 
            // AntSearchItem1
            // 
            this.AntSearchItem1.FormattingEnabled = true;
            this.AntSearchItem1.Location = new System.Drawing.Point(10, 18);
            this.AntSearchItem1.Name = "AntSearchItem1";
            this.AntSearchItem1.Size = new System.Drawing.Size(145, 21);
            this.AntSearchItem1.TabIndex = 36;
            // 
            // AntSearchText1
            // 
            this.AntSearchText1.Location = new System.Drawing.Point(164, 19);
            this.AntSearchText1.Name = "AntSearchText1";
            this.AntSearchText1.Size = new System.Drawing.Size(173, 20);
            this.AntSearchText1.TabIndex = 37;
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.label19);
            this.groupBox23.Controls.Add(this.SearchSubDirsTrailer);
            this.groupBox23.Controls.Add(this.btnTrailer);
            this.groupBox23.Controls.Add(this.PathStorageTrailer);
            this.groupBox23.Controls.Add(this.label34);
            this.groupBox23.Location = new System.Drawing.Point(29, 200);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Size = new System.Drawing.Size(681, 121);
            this.groupBox23.TabIndex = 72;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "Trailer Searchpath (for registering trailers with movies)";
            this.ToolTip1.SetToolTip(this.groupBox23, resources.GetString("groupBox23.ToolTip"));
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(24, 33);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(322, 13);
            this.label19.TabIndex = 71;
            this.label19.Text = "Default is searching trailers in movie directory and it\'s subdirectories";
            // 
            // SearchSubDirsTrailer
            // 
            this.SearchSubDirsTrailer.AutoSize = true;
            this.SearchSubDirsTrailer.Location = new System.Drawing.Point(27, 83);
            this.SearchSubDirsTrailer.Name = "SearchSubDirsTrailer";
            this.SearchSubDirsTrailer.Size = new System.Drawing.Size(130, 17);
            this.SearchSubDirsTrailer.TabIndex = 74;
            this.SearchSubDirsTrailer.Text = "Search in Sub Folders";
            this.ToolTip1.SetToolTip(this.SearchSubDirsTrailer, "Select this option if you want the search for the trailer files\r\nto include sub-f" +
                    "olders of the defined path.\r\n");
            this.SearchSubDirsTrailer.UseVisualStyleBackColor = true;
            this.SearchSubDirsTrailer.CheckedChanged += new System.EventHandler(this.SearchSubDirsTrailer_CheckedChanged);
            // 
            // btnTrailer
            // 
            this.btnTrailer.Location = new System.Drawing.Point(634, 52);
            this.btnTrailer.Name = "btnTrailer";
            this.btnTrailer.Size = new System.Drawing.Size(32, 23);
            this.btnTrailer.TabIndex = 70;
            this.btnTrailer.Text = "...";
            this.btnTrailer.UseVisualStyleBackColor = true;
            this.btnTrailer.Click += new System.EventHandler(this.btnTrailer_Click);
            // 
            // PathStorageTrailer
            // 
            this.PathStorageTrailer.Location = new System.Drawing.Point(190, 54);
            this.PathStorageTrailer.Name = "PathStorageTrailer";
            this.PathStorageTrailer.Size = new System.Drawing.Size(428, 20);
            this.PathStorageTrailer.TabIndex = 68;
            this.PathStorageTrailer.TextChanged += new System.EventHandler(this.PathStorageTrailer_TextChanged);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(24, 57);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(143, 13);
            this.label34.TabIndex = 69;
            this.label34.Text = "Additional Trailer Searchpath";
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.ShowTrailerWhenStartingMovie);
            this.groupBox24.Controls.Add(this.ShowTrailerPlayDialog);
            this.groupBox24.Controls.Add(this.SearchFileNameTrailer);
            this.groupBox24.Controls.Add(this.ItemSearchFileNameTrailer);
            this.groupBox24.Controls.Add(this.AntStorageTrailer);
            this.groupBox24.Controls.Add(this.label35);
            this.groupBox24.Location = new System.Drawing.Point(29, 30);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(681, 164);
            this.groupBox24.TabIndex = 73;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "ANT item for storing trailerinfos (borrower recommended)";
            this.ToolTip1.SetToolTip(this.groupBox24, resources.GetString("groupBox24.ToolTip"));
            // 
            // ShowTrailerWhenStartingMovie
            // 
            this.ShowTrailerWhenStartingMovie.AutoSize = true;
            this.ShowTrailerWhenStartingMovie.Enabled = false;
            this.ShowTrailerWhenStartingMovie.Location = new System.Drawing.Point(27, 125);
            this.ShowTrailerWhenStartingMovie.Name = "ShowTrailerWhenStartingMovie";
            this.ShowTrailerWhenStartingMovie.Size = new System.Drawing.Size(226, 17);
            this.ShowTrailerWhenStartingMovie.TabIndex = 73;
            this.ShowTrailerWhenStartingMovie.Text = "Show Trailer as movie intro (Cinema mode)";
            this.ToolTip1.SetToolTip(this.ShowTrailerWhenStartingMovie, "If checked, a trailer from same category as movie \r\nwill be played before startin" +
                    "g the movie itself.");
            this.ShowTrailerWhenStartingMovie.UseVisualStyleBackColor = true;
            // 
            // ShowTrailerPlayDialog
            // 
            this.ShowTrailerPlayDialog.AutoSize = true;
            this.ShowTrailerPlayDialog.Enabled = false;
            this.ShowTrailerPlayDialog.Location = new System.Drawing.Point(27, 94);
            this.ShowTrailerPlayDialog.Name = "ShowTrailerPlayDialog";
            this.ShowTrailerPlayDialog.Size = new System.Drawing.Size(138, 17);
            this.ShowTrailerPlayDialog.TabIndex = 72;
            this.ShowTrailerPlayDialog.Text = "Show Trailer Userdialog";
            this.ToolTip1.SetToolTip(this.ShowTrailerPlayDialog, "If checked, a dialog will be displayed to select trailer, \r\nif more than one trai" +
                    "ler is present.");
            this.ShowTrailerPlayDialog.UseVisualStyleBackColor = true;
            // 
            // SearchFileNameTrailer
            // 
            this.SearchFileNameTrailer.AutoSize = true;
            this.SearchFileNameTrailer.Location = new System.Drawing.Point(27, 62);
            this.SearchFileNameTrailer.Name = "SearchFileNameTrailer";
            this.SearchFileNameTrailer.Size = new System.Drawing.Size(144, 17);
            this.SearchFileNameTrailer.TabIndex = 71;
            this.SearchFileNameTrailer.Text = "Search by Movie\'s Name";
            this.ToolTip1.SetToolTip(this.SearchFileNameTrailer, resources.GetString("SearchFileNameTrailer.ToolTip"));
            this.SearchFileNameTrailer.UseVisualStyleBackColor = true;
            // 
            // ItemSearchFileNameTrailer
            // 
            this.ItemSearchFileNameTrailer.FormattingEnabled = true;
            this.ItemSearchFileNameTrailer.Items.AddRange(new object[] {
            "OriginalTitle",
            "TranslatedTitle",
            "FormattedTitle"});
            this.ItemSearchFileNameTrailer.Location = new System.Drawing.Point(190, 60);
            this.ItemSearchFileNameTrailer.Name = "ItemSearchFileNameTrailer";
            this.ItemSearchFileNameTrailer.Size = new System.Drawing.Size(185, 21);
            this.ItemSearchFileNameTrailer.TabIndex = 70;
            // 
            // AntStorageTrailer
            // 
            this.AntStorageTrailer.Location = new System.Drawing.Point(190, 25);
            this.AntStorageTrailer.Name = "AntStorageTrailer";
            this.AntStorageTrailer.Size = new System.Drawing.Size(185, 21);
            this.AntStorageTrailer.TabIndex = 68;
            this.AntStorageTrailer.TextChanged += new System.EventHandler(this.AntStorageTrailer_TextChanged);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(24, 28);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(153, 13);
            this.label35.TabIndex = 69;
            this.label35.Text = "Ant Item for storing Trailer Infos";
            // 
            // check_WOL_enable
            // 
            this.check_WOL_enable.AutoSize = true;
            this.check_WOL_enable.Location = new System.Drawing.Point(27, 36);
            this.check_WOL_enable.Name = "check_WOL_enable";
            this.check_WOL_enable.Size = new System.Drawing.Size(126, 17);
            this.check_WOL_enable.TabIndex = 74;
            this.check_WOL_enable.Text = "Enable WakeOnLAN";
            this.ToolTip1.SetToolTip(this.check_WOL_enable, "Select this option if you want MyFilms to automatically \r\nsend WOL magic packets " +
                    "to start NAS server from S3.\r\n");
            this.check_WOL_enable.UseVisualStyleBackColor = true;
            this.check_WOL_enable.CheckedChanged += new System.EventHandler(this.check_WOL_enable_CheckedChanged);
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.label37);
            this.groupBox25.Controls.Add(this.comboWOLtimeout);
            this.groupBox25.Controls.Add(this.buttonSendMagicPacket3);
            this.groupBox25.Controls.Add(this.buttonSendMagicPacket2);
            this.groupBox25.Controls.Add(this.label30);
            this.groupBox25.Controls.Add(this.buttonSendMagicPacket1);
            this.groupBox25.Controls.Add(this.buttonGetMACadresses);
            this.groupBox25.Controls.Add(this.label_NAS_Server_3_MAC);
            this.groupBox25.Controls.Add(this.NAS_MAC_3);
            this.groupBox25.Controls.Add(this.label_NAS_Server_3_Name);
            this.groupBox25.Controls.Add(this.NAS_Name_3);
            this.groupBox25.Controls.Add(this.label_NAS_Server_2_MAC);
            this.groupBox25.Controls.Add(this.NAS_MAC_2);
            this.groupBox25.Controls.Add(this.label_NAS_Server_2_Name);
            this.groupBox25.Controls.Add(this.NAS_Name_2);
            this.groupBox25.Controls.Add(this.label_NAS_Server_1_MAC);
            this.groupBox25.Controls.Add(this.NAS_MAC_1);
            this.groupBox25.Controls.Add(this.label_NAS_Server_1_Name);
            this.groupBox25.Controls.Add(this.check_WOL_Userdialog);
            this.groupBox25.Controls.Add(this.check_WOL_enable);
            this.groupBox25.Controls.Add(this.NAS_Name_1);
            this.groupBox25.Controls.Add(this.label36);
            this.groupBox25.Location = new System.Drawing.Point(22, 29);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Size = new System.Drawing.Size(705, 274);
            this.groupBox25.TabIndex = 73;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "WakeOnLAN Options to Support NAS Storage";
            this.ToolTip1.SetToolTip(this.groupBox25, "If enabled you can start NAS when launching movies");
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(414, 60);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(165, 13);
            this.label37.TabIndex = 94;
            this.label37.Text = "Timeout for NAS Server WakeUp";
            // 
            // comboWOLtimeout
            // 
            this.comboWOLtimeout.FormattingEnabled = true;
            this.comboWOLtimeout.Items.AddRange(new object[] {
            "5",
            "10",
            "15",
            "20",
            "30",
            "60"});
            this.comboWOLtimeout.Location = new System.Drawing.Point(603, 57);
            this.comboWOLtimeout.Name = "comboWOLtimeout";
            this.comboWOLtimeout.Size = new System.Drawing.Size(81, 21);
            this.comboWOLtimeout.TabIndex = 96;
            this.comboWOLtimeout.SelectedIndexChanged += new System.EventHandler(this.comboWOLtimeout_SelectedIndexChanged);
            // 
            // buttonSendMagicPacket3
            // 
            this.buttonSendMagicPacket3.Location = new System.Drawing.Point(603, 233);
            this.buttonSendMagicPacket3.Name = "buttonSendMagicPacket3";
            this.buttonSendMagicPacket3.Size = new System.Drawing.Size(81, 23);
            this.buttonSendMagicPacket3.TabIndex = 91;
            this.buttonSendMagicPacket3.Text = "Start Server 3";
            this.buttonSendMagicPacket3.UseVisualStyleBackColor = true;
            this.buttonSendMagicPacket3.Click += new System.EventHandler(this.buttonSendMagicPacket3_Click);
            // 
            // buttonSendMagicPacket2
            // 
            this.buttonSendMagicPacket2.Location = new System.Drawing.Point(603, 207);
            this.buttonSendMagicPacket2.Name = "buttonSendMagicPacket2";
            this.buttonSendMagicPacket2.Size = new System.Drawing.Size(81, 23);
            this.buttonSendMagicPacket2.TabIndex = 90;
            this.buttonSendMagicPacket2.Text = "Start Server 2";
            this.buttonSendMagicPacket2.UseVisualStyleBackColor = true;
            this.buttonSendMagicPacket2.Click += new System.EventHandler(this.buttonSendMagicPacket2_Click);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(414, 114);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(272, 13);
            this.label30.TabIndex = 89;
            this.label30.Text = "Auto-MAC-discover only works if NAS Storage is running";
            // 
            // buttonSendMagicPacket1
            // 
            this.buttonSendMagicPacket1.Location = new System.Drawing.Point(603, 183);
            this.buttonSendMagicPacket1.Name = "buttonSendMagicPacket1";
            this.buttonSendMagicPacket1.Size = new System.Drawing.Size(81, 23);
            this.buttonSendMagicPacket1.TabIndex = 88;
            this.buttonSendMagicPacket1.Text = "Start Server 1";
            this.buttonSendMagicPacket1.UseVisualStyleBackColor = true;
            this.buttonSendMagicPacket1.Click += new System.EventHandler(this.buttonSendMagicPacket1_Click);
            // 
            // buttonGetMACadresses
            // 
            this.buttonGetMACadresses.Location = new System.Drawing.Point(417, 143);
            this.buttonGetMACadresses.Name = "buttonGetMACadresses";
            this.buttonGetMACadresses.Size = new System.Drawing.Size(160, 28);
            this.buttonGetMACadresses.TabIndex = 87;
            this.buttonGetMACadresses.Text = "Try getting MAC addresses";
            this.buttonGetMACadresses.UseVisualStyleBackColor = true;
            this.buttonGetMACadresses.Click += new System.EventHandler(this.buttonGetMACadresses_Click);
            // 
            // label_NAS_Server_3_MAC
            // 
            this.label_NAS_Server_3_MAC.AutoSize = true;
            this.label_NAS_Server_3_MAC.Location = new System.Drawing.Point(292, 238);
            this.label_NAS_Server_3_MAC.Name = "label_NAS_Server_3_MAC";
            this.label_NAS_Server_3_MAC.Size = new System.Drawing.Size(119, 13);
            this.label_NAS_Server_3_MAC.TabIndex = 86;
            this.label_NAS_Server_3_MAC.Text = "Server 3 - MAC address";
            // 
            // NAS_MAC_3
            // 
            this.NAS_MAC_3.Enabled = false;
            this.NAS_MAC_3.Location = new System.Drawing.Point(417, 235);
            this.NAS_MAC_3.Name = "NAS_MAC_3";
            this.NAS_MAC_3.Size = new System.Drawing.Size(160, 20);
            this.NAS_MAC_3.TabIndex = 85;
            // 
            // label_NAS_Server_3_Name
            // 
            this.label_NAS_Server_3_Name.AutoSize = true;
            this.label_NAS_Server_3_Name.Location = new System.Drawing.Point(24, 238);
            this.label_NAS_Server_3_Name.Name = "label_NAS_Server_3_Name";
            this.label_NAS_Server_3_Name.Size = new System.Drawing.Size(84, 13);
            this.label_NAS_Server_3_Name.TabIndex = 84;
            this.label_NAS_Server_3_Name.Text = "Server 3 - Name";
            // 
            // NAS_Name_3
            // 
            this.NAS_Name_3.Enabled = false;
            this.NAS_Name_3.Location = new System.Drawing.Point(114, 235);
            this.NAS_Name_3.Name = "NAS_Name_3";
            this.NAS_Name_3.Size = new System.Drawing.Size(160, 20);
            this.NAS_Name_3.TabIndex = 83;
            this.NAS_Name_3.TextChanged += new System.EventHandler(this.NAS_Name_3_TextChanged);
            // 
            // label_NAS_Server_2_MAC
            // 
            this.label_NAS_Server_2_MAC.AutoSize = true;
            this.label_NAS_Server_2_MAC.Location = new System.Drawing.Point(292, 212);
            this.label_NAS_Server_2_MAC.Name = "label_NAS_Server_2_MAC";
            this.label_NAS_Server_2_MAC.Size = new System.Drawing.Size(119, 13);
            this.label_NAS_Server_2_MAC.TabIndex = 82;
            this.label_NAS_Server_2_MAC.Text = "Server 2 - MAC address";
            // 
            // NAS_MAC_2
            // 
            this.NAS_MAC_2.Enabled = false;
            this.NAS_MAC_2.Location = new System.Drawing.Point(417, 209);
            this.NAS_MAC_2.Name = "NAS_MAC_2";
            this.NAS_MAC_2.Size = new System.Drawing.Size(160, 20);
            this.NAS_MAC_2.TabIndex = 81;
            // 
            // label_NAS_Server_2_Name
            // 
            this.label_NAS_Server_2_Name.AutoSize = true;
            this.label_NAS_Server_2_Name.Location = new System.Drawing.Point(24, 212);
            this.label_NAS_Server_2_Name.Name = "label_NAS_Server_2_Name";
            this.label_NAS_Server_2_Name.Size = new System.Drawing.Size(84, 13);
            this.label_NAS_Server_2_Name.TabIndex = 80;
            this.label_NAS_Server_2_Name.Text = "Server 2 - Name";
            // 
            // NAS_Name_2
            // 
            this.NAS_Name_2.Enabled = false;
            this.NAS_Name_2.Location = new System.Drawing.Point(114, 209);
            this.NAS_Name_2.Name = "NAS_Name_2";
            this.NAS_Name_2.Size = new System.Drawing.Size(160, 20);
            this.NAS_Name_2.TabIndex = 79;
            this.NAS_Name_2.TextChanged += new System.EventHandler(this.NAS_Name_2_TextChanged);
            // 
            // label_NAS_Server_1_MAC
            // 
            this.label_NAS_Server_1_MAC.AutoSize = true;
            this.label_NAS_Server_1_MAC.Location = new System.Drawing.Point(292, 186);
            this.label_NAS_Server_1_MAC.Name = "label_NAS_Server_1_MAC";
            this.label_NAS_Server_1_MAC.Size = new System.Drawing.Size(119, 13);
            this.label_NAS_Server_1_MAC.TabIndex = 78;
            this.label_NAS_Server_1_MAC.Text = "Server 1 - MAC address";
            // 
            // NAS_MAC_1
            // 
            this.NAS_MAC_1.Enabled = false;
            this.NAS_MAC_1.Location = new System.Drawing.Point(417, 183);
            this.NAS_MAC_1.Name = "NAS_MAC_1";
            this.NAS_MAC_1.Size = new System.Drawing.Size(160, 20);
            this.NAS_MAC_1.TabIndex = 77;
            // 
            // label_NAS_Server_1_Name
            // 
            this.label_NAS_Server_1_Name.AutoSize = true;
            this.label_NAS_Server_1_Name.Location = new System.Drawing.Point(24, 186);
            this.label_NAS_Server_1_Name.Name = "label_NAS_Server_1_Name";
            this.label_NAS_Server_1_Name.Size = new System.Drawing.Size(84, 13);
            this.label_NAS_Server_1_Name.TabIndex = 76;
            this.label_NAS_Server_1_Name.Text = "Server 1 - Name";
            // 
            // check_WOL_Userdialog
            // 
            this.check_WOL_Userdialog.AutoSize = true;
            this.check_WOL_Userdialog.Enabled = false;
            this.check_WOL_Userdialog.Location = new System.Drawing.Point(27, 59);
            this.check_WOL_Userdialog.Name = "check_WOL_Userdialog";
            this.check_WOL_Userdialog.Size = new System.Drawing.Size(343, 17);
            this.check_WOL_Userdialog.TabIndex = 75;
            this.check_WOL_Userdialog.Text = "Enable WakeOnLAN UserDialog (Autoinvoke WOL when disabled)";
            this.ToolTip1.SetToolTip(this.check_WOL_Userdialog, "Enable WakeOnLAN UserDialog whn launching a movie");
            this.check_WOL_Userdialog.UseVisualStyleBackColor = true;
            this.check_WOL_Userdialog.CheckedChanged += new System.EventHandler(this.check_WOL_Userdialog_CheckedChanged);
            // 
            // NAS_Name_1
            // 
            this.NAS_Name_1.Enabled = false;
            this.NAS_Name_1.Location = new System.Drawing.Point(114, 183);
            this.NAS_Name_1.Name = "NAS_Name_1";
            this.NAS_Name_1.Size = new System.Drawing.Size(160, 20);
            this.NAS_Name_1.TabIndex = 68;
            this.NAS_Name_1.TextChanged += new System.EventHandler(this.NAS_1_Name_TextChanged);
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(24, 151);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(235, 13);
            this.label36.TabIndex = 69;
            this.label36.Text = "Supported Servers (make sure, it supports WOL)";
            this.ToolTip1.SetToolTip(this.label36, "MyFilms will autodetect Servername from Filepath, \r\nif UNC notation is used and s" +
                    "erver is defined here.");
            // 
            // label_VersionNumber
            // 
            this.label_VersionNumber.AutoSize = true;
            this.label_VersionNumber.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label_VersionNumber.Location = new System.Drawing.Point(659, 14);
            this.label_VersionNumber.Name = "label_VersionNumber";
            this.label_VersionNumber.Size = new System.Drawing.Size(98, 13);
            this.label_VersionNumber.TabIndex = 34;
            this.label_VersionNumber.Text = "Version 5.1.0 alpha";
            this.ToolTip1.SetToolTip(this.label_VersionNumber, "Alphaversion for Testing only !");
            this.label_VersionNumber.Click += new System.EventHandler(this.label10_Click);
            // 
            // DefaultCover
            // 
            this.DefaultCover.Location = new System.Drawing.Point(208, 43);
            this.DefaultCover.Name = "DefaultCover";
            this.DefaultCover.Size = new System.Drawing.Size(422, 20);
            this.DefaultCover.TabIndex = 89;
            this.ToolTip1.SetToolTip(this.DefaultCover, resources.GetString("DefaultCover.ToolTip"));
            this.DefaultCover.TextChanged += new System.EventHandler(this.DefaultCover_TextChanged);
            // 
            // MesFilmsImg
            // 
            this.MesFilmsImg.Location = new System.Drawing.Point(208, 17);
            this.MesFilmsImg.Name = "MesFilmsImg";
            this.MesFilmsImg.Size = new System.Drawing.Size(422, 20);
            this.MesFilmsImg.TabIndex = 85;
            this.ToolTip1.SetToolTip(this.MesFilmsImg, "Enter the full path to the folder containing the DVD cover image files – by defau" +
                    "lt it is the same as the path to your database file.\r\nYou can use the browse but" +
                    "ton to find the correct path.\r\n");
            // 
            // CheckWatched
            // 
            this.CheckWatched.AutoSize = true;
            this.CheckWatched.Location = new System.Drawing.Point(9, 19);
            this.CheckWatched.Name = "CheckWatched";
            this.CheckWatched.Size = new System.Drawing.Size(270, 17);
            this.CheckWatched.TabIndex = 69;
            this.CheckWatched.Text = "Update the ‘Checked’ field when movie is launched";
            this.ToolTip1.SetToolTip(this.CheckWatched, "Select this option if you want the “Checked” field of your database \r\nto be updat" +
                    "ed each time a movie is launched.\r\n");
            this.CheckWatched.UseVisualStyleBackColor = true;
            // 
            // DefaultCoverArtist
            // 
            this.DefaultCoverArtist.Location = new System.Drawing.Point(208, 44);
            this.DefaultCoverArtist.Name = "DefaultCoverArtist";
            this.DefaultCoverArtist.Size = new System.Drawing.Size(422, 20);
            this.DefaultCoverArtist.TabIndex = 92;
            this.ToolTip1.SetToolTip(this.DefaultCoverArtist, resources.GetString("DefaultCoverArtist.ToolTip"));
            this.DefaultCoverArtist.TextChanged += new System.EventHandler(this.DefaultCoverArtist_TextChanged);
            // 
            // MesFilmsImgArtist
            // 
            this.MesFilmsImgArtist.Location = new System.Drawing.Point(208, 17);
            this.MesFilmsImgArtist.Name = "MesFilmsImgArtist";
            this.MesFilmsImgArtist.Size = new System.Drawing.Size(422, 20);
            this.MesFilmsImgArtist.TabIndex = 95;
            this.ToolTip1.SetToolTip(this.MesFilmsImgArtist, "Enter the full path to the folder containing the Artist image files.\r\n(Check: By " +
                    "default it is the same as the path to your database file.)\r\nYou can use the brow" +
                    "se button to find the correct path.");
            // 
            // groupBox_PreLaunchingCommand
            // 
            this.groupBox_PreLaunchingCommand.Controls.Add(this.label39);
            this.groupBox_PreLaunchingCommand.Controls.Add(this.label38);
            this.groupBox_PreLaunchingCommand.Controls.Add(this.CmdPar);
            this.groupBox_PreLaunchingCommand.Controls.Add(this.CmdExe);
            this.groupBox_PreLaunchingCommand.Location = new System.Drawing.Point(397, 96);
            this.groupBox_PreLaunchingCommand.Name = "groupBox_PreLaunchingCommand";
            this.groupBox_PreLaunchingCommand.Size = new System.Drawing.Size(324, 91);
            this.groupBox_PreLaunchingCommand.TabIndex = 70;
            this.groupBox_PreLaunchingCommand.TabStop = false;
            this.groupBox_PreLaunchingCommand.Text = "Pre-Launching Command";
            this.ToolTip1.SetToolTip(this.groupBox_PreLaunchingCommand, "You can define here a command batch file to be executed before\r\neach movie launch" +
                    ". The following field give the item of your \r\ndatabase passed as  parameter to t" +
                    "he command file.\r\n\r\nVery unused...");
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(19, 59);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(55, 13);
            this.label39.TabIndex = 39;
            this.label39.Text = "Parameter";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(19, 28);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(70, 13);
            this.label38.TabIndex = 38;
            this.label38.Text = "Commandline";
            // 
            // CmdPar
            // 
            this.CmdPar.FormattingEnabled = true;
            this.CmdPar.Location = new System.Drawing.Point(110, 56);
            this.CmdPar.Name = "CmdPar";
            this.CmdPar.Size = new System.Drawing.Size(124, 21);
            this.CmdPar.TabIndex = 36;
            // 
            // CmdExe
            // 
            this.CmdExe.Location = new System.Drawing.Point(110, 25);
            this.CmdExe.Name = "CmdExe";
            this.CmdExe.Size = new System.Drawing.Size(204, 20);
            this.CmdExe.TabIndex = 37;
            // 
            // groupBox_ArtistImages
            // 
            this.groupBox_ArtistImages.Controls.Add(this.btnResetThumbsArtist);
            this.groupBox_ArtistImages.Controls.Add(this.chkDfltArtist);
            this.groupBox_ArtistImages.Controls.Add(this.ButDefCovArtist);
            this.groupBox_ArtistImages.Controls.Add(this.ButImgArtist);
            this.groupBox_ArtistImages.Controls.Add(this.MesFilmsImgArtist);
            this.groupBox_ArtistImages.Controls.Add(this.label_DefaultArtistImage);
            this.groupBox_ArtistImages.Controls.Add(this.label_ArtistImagePath);
            this.groupBox_ArtistImages.Controls.Add(this.DefaultCoverArtist);
            this.groupBox_ArtistImages.Location = new System.Drawing.Point(17, 88);
            this.groupBox_ArtistImages.Name = "groupBox_ArtistImages";
            this.groupBox_ArtistImages.Size = new System.Drawing.Size(710, 94);
            this.groupBox_ArtistImages.TabIndex = 86;
            this.groupBox_ArtistImages.TabStop = false;
            this.groupBox_ArtistImages.Text = "Artist Images";
            this.ToolTip1.SetToolTip(this.groupBox_ArtistImages, "Use this area to define a path where artist thumbs (images for persons) should be" +
                    " stored.\r\nWorks same way as cover images for movies.");
            // 
            // btnResetThumbsArtist
            // 
            this.btnResetThumbsArtist.Location = new System.Drawing.Point(315, 68);
            this.btnResetThumbsArtist.Name = "btnResetThumbsArtist";
            this.btnResetThumbsArtist.Size = new System.Drawing.Size(84, 23);
            this.btnResetThumbsArtist.TabIndex = 99;
            this.btnResetThumbsArtist.Text = "Reset Thumbs";
            this.ToolTip1.SetToolTip(this.btnResetThumbsArtist, "That action\'ll remove all generated artist thumbs");
            this.btnResetThumbsArtist.UseVisualStyleBackColor = true;
            this.btnResetThumbsArtist.Click += new System.EventHandler(this.btnResetThumbsArtist_Click);
            // 
            // chkDfltArtist
            // 
            this.chkDfltArtist.AutoSize = true;
            this.chkDfltArtist.Location = new System.Drawing.Point(42, 73);
            this.chkDfltArtist.Name = "chkDfltArtist";
            this.chkDfltArtist.Size = new System.Drawing.Size(247, 17);
            this.chkDfltArtist.TabIndex = 98;
            this.chkDfltArtist.Text = "Use the default artist image for missing Thumbs";
            this.chkDfltArtist.UseVisualStyleBackColor = true;
            // 
            // ButDefCovArtist
            // 
            this.ButDefCovArtist.Location = new System.Drawing.Point(656, 44);
            this.ButDefCovArtist.Name = "ButDefCovArtist";
            this.ButDefCovArtist.Size = new System.Drawing.Size(32, 22);
            this.ButDefCovArtist.TabIndex = 97;
            this.ButDefCovArtist.Text = "...";
            this.ButDefCovArtist.UseVisualStyleBackColor = true;
            this.ButDefCovArtist.Click += new System.EventHandler(this.ButDefCovArtist_Click);
            // 
            // ButImgArtist
            // 
            this.ButImgArtist.Location = new System.Drawing.Point(656, 15);
            this.ButImgArtist.Name = "ButImgArtist";
            this.ButImgArtist.Size = new System.Drawing.Size(32, 23);
            this.ButImgArtist.TabIndex = 96;
            this.ButImgArtist.Text = "...";
            this.ButImgArtist.UseVisualStyleBackColor = true;
            this.ButImgArtist.Click += new System.EventHandler(this.ButImgArtist_Click);
            // 
            // label_DefaultArtistImage
            // 
            this.label_DefaultArtistImage.AutoSize = true;
            this.label_DefaultArtistImage.Location = new System.Drawing.Point(40, 47);
            this.label_DefaultArtistImage.Name = "label_DefaultArtistImage";
            this.label_DefaultArtistImage.Size = new System.Drawing.Size(99, 13);
            this.label_DefaultArtistImage.TabIndex = 94;
            this.label_DefaultArtistImage.Text = "Default Artist Image";
            // 
            // label_ArtistImagePath
            // 
            this.label_ArtistImagePath.AutoSize = true;
            this.label_ArtistImagePath.Location = new System.Drawing.Point(40, 20);
            this.label_ArtistImagePath.Name = "label_ArtistImagePath";
            this.label_ArtistImagePath.Size = new System.Drawing.Size(92, 13);
            this.label_ArtistImagePath.TabIndex = 93;
            this.label_ArtistImagePath.Text = "Artist Images Path";
            // 
            // checkWatchedInProfile
            // 
            this.checkWatchedInProfile.AutoSize = true;
            this.checkWatchedInProfile.Enabled = false;
            this.checkWatchedInProfile.Location = new System.Drawing.Point(9, 42);
            this.checkWatchedInProfile.Name = "checkWatchedInProfile";
            this.checkWatchedInProfile.Size = new System.Drawing.Size(338, 17);
            this.checkWatchedInProfile.TabIndex = 70;
            this.checkWatchedInProfile.Text = "Update the movie status in the userprofile when movie is launched";
            this.ToolTip1.SetToolTip(this.checkWatchedInProfile, "Select this option if you want to update the watched status in the\r\nuserprofile. " +
                    "Requires a userprofilename to choose.\r\nTo be updated each time a movie is launch" +
                    "ed.");
            this.checkWatchedInProfile.UseVisualStyleBackColor = true;
            // 
            // AntFilterMinRating
            // 
            this.AntFilterMinRating.Location = new System.Drawing.Point(0, 0);
            this.AntFilterMinRating.Name = "AntFilterMinRating";
            this.AntFilterMinRating.Size = new System.Drawing.Size(121, 21);
            this.AntFilterMinRating.TabIndex = 0;
            // 
            // AntFilterSign4
            // 
            this.AntFilterSign4.Location = new System.Drawing.Point(0, 0);
            this.AntFilterSign4.Name = "AntFilterSign4";
            this.AntFilterSign4.Size = new System.Drawing.Size(121, 21);
            this.AntFilterSign4.TabIndex = 0;
            // 
            // AntFilterSign3
            // 
            this.AntFilterSign3.Location = new System.Drawing.Point(0, 0);
            this.AntFilterSign3.Name = "AntFilterSign3";
            this.AntFilterSign3.Size = new System.Drawing.Size(121, 21);
            this.AntFilterSign3.TabIndex = 0;
            // 
            // AntFilterItem4
            // 
            this.AntFilterItem4.Location = new System.Drawing.Point(0, 0);
            this.AntFilterItem4.Name = "AntFilterItem4";
            this.AntFilterItem4.Size = new System.Drawing.Size(121, 21);
            this.AntFilterItem4.TabIndex = 0;
            // 
            // AntFilterText4
            // 
            this.AntFilterText4.Location = new System.Drawing.Point(0, 0);
            this.AntFilterText4.Name = "AntFilterText4";
            this.AntFilterText4.Size = new System.Drawing.Size(100, 20);
            this.AntFilterText4.TabIndex = 0;
            // 
            // AntFilterItem3
            // 
            this.AntFilterItem3.Location = new System.Drawing.Point(0, 0);
            this.AntFilterItem3.Name = "AntFilterItem3";
            this.AntFilterItem3.Size = new System.Drawing.Size(121, 21);
            this.AntFilterItem3.TabIndex = 0;
            // 
            // AntFilterText3
            // 
            this.AntFilterText3.Location = new System.Drawing.Point(0, 0);
            this.AntFilterText3.Name = "AntFilterText3";
            this.AntFilterText3.Size = new System.Drawing.Size(100, 20);
            this.AntFilterText3.TabIndex = 0;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(80, 14);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 13);
            this.label11.TabIndex = 36;
            this.label11.Text = "Menu Plugin Name";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(80, 42);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 13);
            this.label12.TabIndex = 38;
            this.label12.Text = "Configuration Name";
            // 
            // General
            // 
            this.General.Controls.Add(this.Tab_General);
            this.General.Controls.Add(this.Tab_Artwork);
            this.General.Controls.Add(this.Tab_Optional1);
            this.General.Controls.Add(this.Tab_Optional2);
            this.General.Controls.Add(this.Tab_DVDprofilerMovieCollector);
            this.General.Controls.Add(this.Tab_Grabber);
            this.General.Controls.Add(this.Tab_Logos);
            this.General.Controls.Add(this.Tab_TrailerOptions);
            this.General.Controls.Add(this.Tab_WakeOnLan);
            this.General.Location = new System.Drawing.Point(12, 66);
            this.General.Name = "General";
            this.General.SelectedIndex = 0;
            this.General.Size = new System.Drawing.Size(755, 380);
            this.General.TabIndex = 46;
            this.General.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.General_Selected);
            // 
            // Tab_General
            // 
            this.Tab_General.Controls.Add(this.groupBox_Security);
            this.Tab_General.Controls.Add(this.groupBox_PlayMovieInfos);
            this.Tab_General.Controls.Add(this.label13);
            this.Tab_General.Controls.Add(this.CatalogType);
            this.Tab_General.Controls.Add(this.groupBox_TitleOrder);
            this.Tab_General.Controls.Add(this.ButCat);
            this.Tab_General.Controls.Add(this.MesFilmsCat);
            this.Tab_General.Controls.Add(this.label1);
            this.Tab_General.Location = new System.Drawing.Point(4, 22);
            this.Tab_General.Name = "Tab_General";
            this.Tab_General.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_General.Size = new System.Drawing.Size(747, 354);
            this.Tab_General.TabIndex = 0;
            this.Tab_General.Text = "General";
            this.Tab_General.UseVisualStyleBackColor = true;
            // 
            // groupBox_Security
            // 
            this.groupBox_Security.Controls.Add(this.label_Security);
            this.groupBox_Security.Controls.Add(this.label16);
            this.groupBox_Security.Controls.Add(this.label15);
            this.groupBox_Security.Controls.Add(this.Rpt_Dwp);
            this.groupBox_Security.Controls.Add(this.Dwp);
            this.groupBox_Security.Location = new System.Drawing.Point(455, 242);
            this.groupBox_Security.Name = "groupBox_Security";
            this.groupBox_Security.Size = new System.Drawing.Size(277, 101);
            this.groupBox_Security.TabIndex = 75;
            this.groupBox_Security.TabStop = false;
            this.groupBox_Security.Text = "Security";
            // 
            // label_Security
            // 
            this.label_Security.AutoSize = true;
            this.label_Security.Location = new System.Drawing.Point(101, 19);
            this.label_Security.Name = "label_Security";
            this.label_Security.Size = new System.Drawing.Size(167, 13);
            this.label_Security.TabIndex = 73;
            this.label_Security.Text = "Enter password to protect the DB:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 72);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(91, 13);
            this.label16.TabIndex = 72;
            this.label16.Text = "Repeat Password";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(44, 47);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 13);
            this.label15.TabIndex = 71;
            this.label15.Text = "Password";
            // 
            // Rpt_Dwp
            // 
            this.Rpt_Dwp.Location = new System.Drawing.Point(104, 69);
            this.Rpt_Dwp.Name = "Rpt_Dwp";
            this.Rpt_Dwp.Size = new System.Drawing.Size(164, 20);
            this.Rpt_Dwp.TabIndex = 70;
            this.Rpt_Dwp.UseSystemPasswordChar = true;
            // 
            // groupBox_PlayMovieInfos
            // 
            this.groupBox_PlayMovieInfos.Controls.Add(this.groupBox2);
            this.groupBox_PlayMovieInfos.Controls.Add(this.groupBox_PreLaunchingCommand);
            this.groupBox_PlayMovieInfos.Controls.Add(this.SearchSubDirs);
            this.groupBox_PlayMovieInfos.Controls.Add(this.AntIdentLabel);
            this.groupBox_PlayMovieInfos.Controls.Add(this.butPath);
            this.groupBox_PlayMovieInfos.Controls.Add(this.PathStorage);
            this.groupBox_PlayMovieInfos.Controls.Add(this.label5);
            this.groupBox_PlayMovieInfos.Controls.Add(this.label6);
            this.groupBox_PlayMovieInfos.Controls.Add(this.AntStorage);
            this.groupBox_PlayMovieInfos.Controls.Add(this.ItemSearchFileName);
            this.groupBox_PlayMovieInfos.Controls.Add(this.SearchFileName);
            this.groupBox_PlayMovieInfos.Controls.Add(this.label4);
            this.groupBox_PlayMovieInfos.Controls.Add(this.AntIdentItem);
            this.groupBox_PlayMovieInfos.Location = new System.Drawing.Point(7, 43);
            this.groupBox_PlayMovieInfos.Name = "groupBox_PlayMovieInfos";
            this.groupBox_PlayMovieInfos.Size = new System.Drawing.Size(725, 193);
            this.groupBox_PlayMovieInfos.TabIndex = 74;
            this.groupBox_PlayMovieInfos.TabStop = false;
            this.groupBox_PlayMovieInfos.Text = "Play Movie Infos";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Label_UserProfileName);
            this.groupBox2.Controls.Add(this.UserProfileName);
            this.groupBox2.Controls.Add(this.checkWatchedInProfile);
            this.groupBox2.Controls.Add(this.CheckWatched);
            this.groupBox2.Location = new System.Drawing.Point(10, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(370, 91);
            this.groupBox2.TabIndex = 71;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MovieUpdates - watched-status";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // Label_UserProfileName
            // 
            this.Label_UserProfileName.AutoSize = true;
            this.Label_UserProfileName.Location = new System.Drawing.Point(27, 64);
            this.Label_UserProfileName.Name = "Label_UserProfileName";
            this.Label_UserProfileName.Size = new System.Drawing.Size(92, 13);
            this.Label_UserProfileName.TabIndex = 72;
            this.Label_UserProfileName.Text = "User Profile Name";
            // 
            // UserProfileName
            // 
            this.UserProfileName.Enabled = false;
            this.UserProfileName.Location = new System.Drawing.Point(143, 61);
            this.UserProfileName.Name = "UserProfileName";
            this.UserProfileName.Size = new System.Drawing.Size(204, 20);
            this.UserProfileName.TabIndex = 71;
            // 
            // AntIdentLabel
            // 
            this.AntIdentLabel.Location = new System.Drawing.Point(335, 71);
            this.AntIdentLabel.Name = "AntIdentLabel";
            this.AntIdentLabel.Size = new System.Drawing.Size(144, 20);
            this.AntIdentLabel.TabIndex = 57;
            // 
            // butPath
            // 
            this.butPath.Location = new System.Drawing.Point(447, 43);
            this.butPath.Name = "butPath";
            this.butPath.Size = new System.Drawing.Size(32, 23);
            this.butPath.TabIndex = 55;
            this.butPath.Text = "...";
            this.butPath.UseVisualStyleBackColor = true;
            this.butPath.Click += new System.EventHandler(this.butPath_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(145, 13);
            this.label5.TabIndex = 60;
            this.label5.Text = "Paths for Movies File Search ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(144, 13);
            this.label6.TabIndex = 59;
            this.label6.Text = "Ant Item for Storage  File Info";
            // 
            // ItemSearchFileName
            // 
            this.ItemSearchFileName.FormattingEnabled = true;
            this.ItemSearchFileName.Items.AddRange(new object[] {
            "OriginalTitle",
            "TranslatedTitle",
            "FormattedTitle"});
            this.ItemSearchFileName.Location = new System.Drawing.Point(599, 44);
            this.ItemSearchFileName.Name = "ItemSearchFileName";
            this.ItemSearchFileName.Size = new System.Drawing.Size(112, 21);
            this.ItemSearchFileName.TabIndex = 67;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 58;
            this.label4.Text = "Ant Identification Item";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(452, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 63;
            this.label13.Text = "Catalog Type";
            // 
            // groupBox_TitleOrder
            // 
            this.groupBox_TitleOrder.Controls.Add(this.label20);
            this.groupBox_TitleOrder.Controls.Add(this.AntSTitle);
            this.groupBox_TitleOrder.Controls.Add(this.label17);
            this.groupBox_TitleOrder.Controls.Add(this.TitleDelim);
            this.groupBox_TitleOrder.Controls.Add(this.label9);
            this.groupBox_TitleOrder.Controls.Add(this.label8);
            this.groupBox_TitleOrder.Controls.Add(this.AntTitle2);
            this.groupBox_TitleOrder.Controls.Add(this.AntTitle1);
            this.groupBox_TitleOrder.Location = new System.Drawing.Point(7, 242);
            this.groupBox_TitleOrder.Name = "groupBox_TitleOrder";
            this.groupBox_TitleOrder.Size = new System.Drawing.Size(432, 101);
            this.groupBox_TitleOrder.TabIndex = 61;
            this.groupBox_TitleOrder.TabStop = false;
            this.groupBox_TitleOrder.Text = "Title Order";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(7, 72);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(65, 13);
            this.label20.TabIndex = 71;
            this.label20.Text = "Ant SortTitle";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(294, 21);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(101, 13);
            this.label17.TabIndex = 66;
            this.label17.Text = "Hierarchy Separator";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 46);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(97, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Ant SecondaryTitle";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Ant Master Title";
            // 
            // ButCat
            // 
            this.ButCat.Location = new System.Drawing.Point(407, 13);
            this.ButCat.Name = "ButCat";
            this.ButCat.Size = new System.Drawing.Size(32, 23);
            this.ButCat.TabIndex = 49;
            this.ButCat.Text = "...";
            this.ButCat.UseVisualStyleBackColor = true;
            this.ButCat.Click += new System.EventHandler(this.ButCat_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 47;
            this.label1.Text = "Catalog File (XML)";
            // 
            // Tab_Artwork
            // 
            this.Tab_Artwork.Controls.Add(this.groupBox_ArtistImages);
            this.Tab_Artwork.Controls.Add(this.groupBox1);
            this.Tab_Artwork.Controls.Add(this.groupBox22);
            this.Tab_Artwork.Controls.Add(this.Fanart);
            this.Tab_Artwork.Location = new System.Drawing.Point(4, 22);
            this.Tab_Artwork.Name = "Tab_Artwork";
            this.Tab_Artwork.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Artwork.Size = new System.Drawing.Size(747, 354);
            this.Tab_Artwork.TabIndex = 5;
            this.Tab_Artwork.Text = "Artwork";
            this.Tab_Artwork.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ButDefCov);
            this.groupBox1.Controls.Add(this.DefaultCover);
            this.groupBox1.Controls.Add(this.label_DefaulCover);
            this.groupBox1.Controls.Add(this.ButImg);
            this.groupBox1.Controls.Add(this.MesFilmsImg);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(17, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(710, 76);
            this.groupBox1.TabIndex = 85;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cover Images";
            // 
            // ButDefCov
            // 
            this.ButDefCov.Location = new System.Drawing.Point(656, 42);
            this.ButDefCov.Name = "ButDefCov";
            this.ButDefCov.Size = new System.Drawing.Size(32, 22);
            this.ButDefCov.TabIndex = 90;
            this.ButDefCov.Text = "...";
            this.ButDefCov.UseVisualStyleBackColor = true;
            this.ButDefCov.Click += new System.EventHandler(this.ButDefCov_Click);
            // 
            // label_DefaulCover
            // 
            this.label_DefaulCover.AutoSize = true;
            this.label_DefaulCover.Location = new System.Drawing.Point(40, 47);
            this.label_DefaulCover.Name = "label_DefaulCover";
            this.label_DefaulCover.Size = new System.Drawing.Size(104, 13);
            this.label_DefaulCover.TabIndex = 88;
            this.label_DefaulCover.Text = "Default Movie Cover";
            // 
            // ButImg
            // 
            this.ButImg.Location = new System.Drawing.Point(656, 15);
            this.ButImg.Name = "ButImg";
            this.ButImg.Size = new System.Drawing.Size(32, 23);
            this.ButImg.TabIndex = 87;
            this.ButImg.Text = "...";
            this.ButImg.UseVisualStyleBackColor = true;
            this.ButImg.Click += new System.EventHandler(this.ButImg_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 86;
            this.label2.Text = "Cover Images Path";
            // 
            // Tab_Optional1
            // 
            this.Tab_Optional1.Controls.Add(this.groupBox_SortByItem);
            this.Tab_Optional1.Controls.Add(this.groupBox_AntSelectedEnreg);
            this.Tab_Optional1.Controls.Add(this.groupBox_DefaultView);
            this.Tab_Optional1.Controls.Add(this.groupBox_SupplementaryView);
            this.Tab_Optional1.Controls.Add(this.groupBox_DetailedSupplementaryInformations);
            this.Tab_Optional1.Location = new System.Drawing.Point(4, 22);
            this.Tab_Optional1.Name = "Tab_Optional1";
            this.Tab_Optional1.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Optional1.Size = new System.Drawing.Size(747, 354);
            this.Tab_Optional1.TabIndex = 1;
            this.Tab_Optional1.Text = "Optional 1";
            this.Tab_Optional1.UseVisualStyleBackColor = true;
            // 
            // Tab_Optional2
            // 
            this.Tab_Optional2.Controls.Add(this.groupBox_SupplementarySearch);
            this.Tab_Optional2.Controls.Add(this.groupBox_DatabaseUpdateOptions);
            this.Tab_Optional2.Controls.Add(this.groupBox_Separators);
            this.Tab_Optional2.Location = new System.Drawing.Point(4, 22);
            this.Tab_Optional2.Name = "Tab_Optional2";
            this.Tab_Optional2.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Optional2.Size = new System.Drawing.Size(747, 354);
            this.Tab_Optional2.TabIndex = 3;
            this.Tab_Optional2.Text = "Optional 2";
            this.Tab_Optional2.UseVisualStyleBackColor = true;
            // 
            // Tab_DVDprofilerMovieCollector
            // 
            this.Tab_DVDprofilerMovieCollector.Controls.Add(this.groupBox_MovieCollector);
            this.Tab_DVDprofilerMovieCollector.Controls.Add(this.groupBox9);
            this.Tab_DVDprofilerMovieCollector.Controls.Add(this.groupBox_DVDprofiler);
            this.Tab_DVDprofilerMovieCollector.Location = new System.Drawing.Point(4, 22);
            this.Tab_DVDprofilerMovieCollector.Name = "Tab_DVDprofilerMovieCollector";
            this.Tab_DVDprofilerMovieCollector.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_DVDprofilerMovieCollector.Size = new System.Drawing.Size(747, 354);
            this.Tab_DVDprofilerMovieCollector.TabIndex = 2;
            this.Tab_DVDprofilerMovieCollector.Text = "DVDProfiler/Movie Collecor";
            this.Tab_DVDprofilerMovieCollector.UseVisualStyleBackColor = true;
            // 
            // groupBox_MovieCollector
            // 
            this.groupBox_MovieCollector.Controls.Add(this.groupBox17);
            this.groupBox_MovieCollector.Location = new System.Drawing.Point(377, 17);
            this.groupBox_MovieCollector.Name = "groupBox_MovieCollector";
            this.groupBox_MovieCollector.Size = new System.Drawing.Size(349, 134);
            this.groupBox_MovieCollector.TabIndex = 29;
            this.groupBox_MovieCollector.TabStop = false;
            this.groupBox_MovieCollector.Text = "Movie Collector";
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.Images);
            this.groupBox17.Controls.Add(this.Thumbnails);
            this.groupBox17.Location = new System.Drawing.Point(11, 18);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(322, 91);
            this.groupBox17.TabIndex = 3;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "Covers";
            // 
            // Images
            // 
            this.Images.AutoSize = true;
            this.Images.Location = new System.Drawing.Point(24, 47);
            this.Images.Name = "Images";
            this.Images.Size = new System.Drawing.Size(168, 17);
            this.Images.TabIndex = 3;
            this.Images.TabStop = true;
            this.Images.Text = "Use\' Images\' Folder for Covers";
            this.Images.UseVisualStyleBackColor = true;
            // 
            // Thumbnails
            // 
            this.Thumbnails.AutoSize = true;
            this.Thumbnails.Location = new System.Drawing.Point(24, 22);
            this.Thumbnails.Name = "Thumbnails";
            this.Thumbnails.Size = new System.Drawing.Size(188, 17);
            this.Thumbnails.TabIndex = 2;
            this.Thumbnails.TabStop = true;
            this.Thumbnails.Text = "Use \'Thumbnails\' Folder for Covers";
            this.Thumbnails.UseVisualStyleBackColor = true;
            // 
            // groupBox_DVDprofiler
            // 
            this.groupBox_DVDprofiler.Controls.Add(this.groupBox13);
            this.groupBox_DVDprofiler.Controls.Add(this.groupBox10);
            this.groupBox_DVDprofiler.Location = new System.Drawing.Point(22, 17);
            this.groupBox_DVDprofiler.Name = "groupBox_DVDprofiler";
            this.groupBox_DVDprofiler.Size = new System.Drawing.Size(349, 314);
            this.groupBox_DVDprofiler.TabIndex = 28;
            this.groupBox_DVDprofiler.TabStop = false;
            this.groupBox_DVDprofiler.Text = "DVDProfiler";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.OnlyFile);
            this.groupBox13.Location = new System.Drawing.Point(12, 207);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(327, 66);
            this.groupBox13.TabIndex = 2;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Notes Field";
            // 
            // OnlyFile
            // 
            this.OnlyFile.AutoSize = true;
            this.OnlyFile.Location = new System.Drawing.Point(24, 31);
            this.OnlyFile.Name = "OnlyFile";
            this.OnlyFile.Size = new System.Drawing.Size(221, 17);
            this.OnlyFile.TabIndex = 0;
            this.OnlyFile.Text = "Notes Field contains only Movie Filename";
            this.OnlyFile.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.SortTitle);
            this.groupBox10.Location = new System.Drawing.Point(12, 127);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(327, 62);
            this.groupBox10.TabIndex = 1;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Use SortTitle";
            // 
            // SortTitle
            // 
            this.SortTitle.AutoSize = true;
            this.SortTitle.Location = new System.Drawing.Point(24, 30);
            this.SortTitle.Name = "SortTitle";
            this.SortTitle.Size = new System.Drawing.Size(199, 17);
            this.SortTitle.TabIndex = 0;
            this.SortTitle.Text = "Store SortTitle in FormattedTitle Field";
            this.SortTitle.UseVisualStyleBackColor = true;
            // 
            // Tab_Grabber
            // 
            this.Tab_Grabber.Controls.Add(this.groupBox_nfoGrabber);
            this.Tab_Grabber.Controls.Add(this.groupBox20);
            this.Tab_Grabber.Controls.Add(this.groupBox_GrabberOptions);
            this.Tab_Grabber.Location = new System.Drawing.Point(4, 22);
            this.Tab_Grabber.Name = "Tab_Grabber";
            this.Tab_Grabber.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Grabber.Size = new System.Drawing.Size(747, 354);
            this.Tab_Grabber.TabIndex = 4;
            this.Tab_Grabber.Text = "Grabber";
            this.Tab_Grabber.UseVisualStyleBackColor = true;
            // 
            // groupBox_nfoGrabber
            // 
            this.groupBox_nfoGrabber.Location = new System.Drawing.Point(21, 243);
            this.groupBox_nfoGrabber.Name = "groupBox_nfoGrabber";
            this.groupBox_nfoGrabber.Size = new System.Drawing.Size(693, 105);
            this.groupBox_nfoGrabber.TabIndex = 3;
            this.groupBox_nfoGrabber.TabStop = false;
            this.groupBox_nfoGrabber.Text = "XBMC nfo Grabber";
            // 
            // Tab_Logos
            // 
            this.Tab_Logos.Controls.Add(this.btnLogosPath);
            this.Tab_Logos.Controls.Add(this.txtLogosPath);
            this.Tab_Logos.Controls.Add(this.lblLogosPath);
            this.Tab_Logos.Controls.Add(this.SFilePicture);
            this.Tab_Logos.Controls.Add(this.LogoView);
            this.Tab_Logos.Controls.Add(this.btnDown);
            this.Tab_Logos.Controls.Add(this.btnUp);
            this.Tab_Logos.Controls.Add(this.btnDel);
            this.Tab_Logos.Controls.Add(this.btnAdd);
            this.Tab_Logos.Controls.Add(this.SAnd_Or);
            this.Tab_Logos.Controls.Add(this.SValue2);
            this.Tab_Logos.Controls.Add(this.SOp2);
            this.Tab_Logos.Controls.Add(this.SField2);
            this.Tab_Logos.Controls.Add(this.SValue1);
            this.Tab_Logos.Controls.Add(this.SOp1);
            this.Tab_Logos.Controls.Add(this.SField1);
            this.Tab_Logos.Controls.Add(this.textBox3);
            this.Tab_Logos.Controls.Add(this.chkLogos);
            this.Tab_Logos.Controls.Add(this.SLogo_Type);
            this.Tab_Logos.Controls.Add(this.SPicture);
            this.Tab_Logos.Location = new System.Drawing.Point(4, 22);
            this.Tab_Logos.Name = "Tab_Logos";
            this.Tab_Logos.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Logos.Size = new System.Drawing.Size(747, 354);
            this.Tab_Logos.TabIndex = 6;
            this.Tab_Logos.Text = "Logos";
            this.Tab_Logos.UseVisualStyleBackColor = true;
            // 
            // btnLogosPath
            // 
            this.btnLogosPath.Enabled = false;
            this.btnLogosPath.Location = new System.Drawing.Point(695, 8);
            this.btnLogosPath.Name = "btnLogosPath";
            this.btnLogosPath.Size = new System.Drawing.Size(32, 20);
            this.btnLogosPath.TabIndex = 94;
            this.btnLogosPath.Text = "...";
            this.btnLogosPath.UseVisualStyleBackColor = true;
            this.btnLogosPath.Click += new System.EventHandler(this.btnLogosPath_Click);
            // 
            // lblLogosPath
            // 
            this.lblLogosPath.AutoSize = true;
            this.lblLogosPath.Location = new System.Drawing.Point(124, 12);
            this.lblLogosPath.Name = "lblLogosPath";
            this.lblLogosPath.Size = new System.Drawing.Size(110, 13);
            this.lblLogosPath.TabIndex = 93;
            this.lblLogosPath.Text = "Path for storing Logos";
            // 
            // SFilePicture
            // 
            this.SFilePicture.Location = new System.Drawing.Point(498, 245);
            this.SFilePicture.Name = "SFilePicture";
            this.SFilePicture.Size = new System.Drawing.Size(79, 20);
            this.SFilePicture.TabIndex = 91;
            this.SFilePicture.Visible = false;
            // 
            // LogoView
            // 
            this.LogoView.AllowColumnReorder = true;
            this.LogoView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LogoType,
            this.Field1,
            this.Op1,
            this.Value1,
            this.And_Or,
            this.Field2,
            this.Op2,
            this.Value2,
            this.Image,
            this.PathImage});
            this.LogoView.Enabled = false;
            this.LogoView.FullRowSelect = true;
            this.LogoView.GridLines = true;
            this.LogoView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LogoView.Location = new System.Drawing.Point(7, 36);
            this.LogoView.MultiSelect = false;
            this.LogoView.Name = "LogoView";
            this.LogoView.Size = new System.Drawing.Size(734, 203);
            this.LogoView.TabIndex = 90;
            this.LogoView.UseCompatibleStateImageBehavior = false;
            this.LogoView.View = System.Windows.Forms.View.Details;
            this.LogoView.SelectedIndexChanged += new System.EventHandler(this.LogoView_SelectedIndexChanged);
            // 
            // LogoType
            // 
            this.LogoType.Text = "Type Logo";
            this.LogoType.Width = 66;
            // 
            // Field1
            // 
            this.Field1.Text = "Field";
            this.Field1.Width = 56;
            // 
            // Op1
            // 
            this.Op1.Text = "Operator";
            this.Op1.Width = 58;
            // 
            // Value1
            // 
            this.Value1.Text = "Value";
            this.Value1.Width = 73;
            // 
            // And_Or
            // 
            this.And_Or.Text = "And/Or";
            this.And_Or.Width = 51;
            // 
            // Field2
            // 
            this.Field2.Text = "Field";
            // 
            // Op2
            // 
            this.Op2.Text = "Operator";
            this.Op2.Width = 58;
            // 
            // Value2
            // 
            this.Value2.Text = "Value";
            this.Value2.Width = 68;
            // 
            // Image
            // 
            this.Image.Text = "Picture";
            this.Image.Width = 83;
            // 
            // PathImage
            // 
            this.PathImage.Text = "Path";
            this.PathImage.Width = 155;
            // 
            // btnDown
            // 
            this.btnDown.Enabled = false;
            this.btnDown.Location = new System.Drawing.Point(583, 303);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(64, 21);
            this.btnDown.TabIndex = 89;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Enabled = false;
            this.btnUp.Location = new System.Drawing.Point(583, 276);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(64, 21);
            this.btnUp.TabIndex = 88;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDel
            // 
            this.btnDel.Enabled = false;
            this.btnDel.Location = new System.Drawing.Point(663, 303);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(64, 21);
            this.btnDel.TabIndex = 87;
            this.btnDel.Text = "Delete";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(663, 276);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(64, 21);
            this.btnAdd.TabIndex = 86;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // SAnd_Or
            // 
            this.SAnd_Or.Enabled = false;
            this.SAnd_Or.FormattingEnabled = true;
            this.SAnd_Or.Items.AddRange(new object[] {
            "And",
            "Or",
            ""});
            this.SAnd_Or.Location = new System.Drawing.Point(117, 289);
            this.SAnd_Or.Name = "SAnd_Or";
            this.SAnd_Or.Size = new System.Drawing.Size(54, 21);
            this.SAnd_Or.TabIndex = 82;
            // 
            // SValue2
            // 
            this.SValue2.Enabled = false;
            this.SValue2.FormattingEnabled = true;
            this.SValue2.Location = new System.Drawing.Point(371, 303);
            this.SValue2.Name = "SValue2";
            this.SValue2.Size = new System.Drawing.Size(110, 21);
            this.SValue2.TabIndex = 81;
            // 
            // SOp2
            // 
            this.SOp2.Enabled = false;
            this.SOp2.FormattingEnabled = true;
            this.SOp2.Items.AddRange(new object[] {
            "equal",
            "not equal",
            "contains",
            "not contains",
            "greater",
            "lower",
            "filled",
            "not filled"});
            this.SOp2.Location = new System.Drawing.Point(291, 303);
            this.SOp2.Name = "SOp2";
            this.SOp2.Size = new System.Drawing.Size(74, 21);
            this.SOp2.TabIndex = 80;
            this.SOp2.SelectedIndexChanged += new System.EventHandler(this.SOp2_SelectedIndexChanged);
            // 
            // SField2
            // 
            this.SField2.Enabled = false;
            this.SField2.FormattingEnabled = true;
            this.SField2.Location = new System.Drawing.Point(175, 303);
            this.SField2.Name = "SField2";
            this.SField2.Size = new System.Drawing.Size(110, 21);
            this.SField2.Sorted = true;
            this.SField2.TabIndex = 79;
            this.SField2.SelectedIndexChanged += new System.EventHandler(this.SField2_SelectedIndexChanged);
            // 
            // SValue1
            // 
            this.SValue1.Enabled = false;
            this.SValue1.FormattingEnabled = true;
            this.SValue1.Location = new System.Drawing.Point(371, 276);
            this.SValue1.Name = "SValue1";
            this.SValue1.Size = new System.Drawing.Size(110, 21);
            this.SValue1.TabIndex = 78;
            // 
            // SOp1
            // 
            this.SOp1.Enabled = false;
            this.SOp1.FormattingEnabled = true;
            this.SOp1.Items.AddRange(new object[] {
            "equal",
            "not equal",
            "contains",
            "not contains",
            "greater",
            "lower",
            "filled",
            "not filled"});
            this.SOp1.Location = new System.Drawing.Point(291, 276);
            this.SOp1.Name = "SOp1";
            this.SOp1.Size = new System.Drawing.Size(74, 21);
            this.SOp1.TabIndex = 77;
            this.SOp1.SelectedIndexChanged += new System.EventHandler(this.SOp1_SelectedIndexChanged);
            // 
            // SField1
            // 
            this.SField1.Enabled = false;
            this.SField1.FormattingEnabled = true;
            this.SField1.Location = new System.Drawing.Point(175, 276);
            this.SField1.Name = "SField1";
            this.SField1.Size = new System.Drawing.Size(110, 21);
            this.SField1.Sorted = true;
            this.SField1.TabIndex = 76;
            this.SField1.SelectedIndexChanged += new System.EventHandler(this.SField1_SelectedIndexChanged);
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.White;
            this.textBox3.Enabled = false;
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(7, 245);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(474, 20);
            this.textBox3.TabIndex = 75;
            this.textBox3.TabStop = false;
            this.textBox3.Text = "* be carefull the Logo Configuration is available for all MyFilms configurations";
            // 
            // SPicture
            // 
            this.SPicture.BackColor = System.Drawing.Color.SteelBlue;
            this.SPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SPicture.Enabled = false;
            this.SPicture.Location = new System.Drawing.Point(498, 272);
            this.SPicture.Name = "SPicture";
            this.SPicture.Size = new System.Drawing.Size(79, 58);
            this.SPicture.TabIndex = 85;
            this.SPicture.TabStop = false;
            this.SPicture.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // Tab_TrailerOptions
            // 
            this.Tab_TrailerOptions.Controls.Add(this.groupBox24);
            this.Tab_TrailerOptions.Controls.Add(this.groupBox23);
            this.Tab_TrailerOptions.Location = new System.Drawing.Point(4, 22);
            this.Tab_TrailerOptions.Name = "Tab_TrailerOptions";
            this.Tab_TrailerOptions.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_TrailerOptions.Size = new System.Drawing.Size(747, 354);
            this.Tab_TrailerOptions.TabIndex = 7;
            this.Tab_TrailerOptions.Text = "Traileroptions";
            this.Tab_TrailerOptions.UseVisualStyleBackColor = true;
            // 
            // Tab_WakeOnLan
            // 
            this.Tab_WakeOnLan.Controls.Add(this.groupBox25);
            this.Tab_WakeOnLan.Location = new System.Drawing.Point(4, 22);
            this.Tab_WakeOnLan.Name = "Tab_WakeOnLan";
            this.Tab_WakeOnLan.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_WakeOnLan.Size = new System.Drawing.Size(747, 354);
            this.Tab_WakeOnLan.TabIndex = 8;
            this.Tab_WakeOnLan.Text = "WakeOnLAN";
            this.Tab_WakeOnLan.UseVisualStyleBackColor = true;
            // 
            // ButSave
            // 
            this.ButSave.Location = new System.Drawing.Point(529, 452);
            this.ButSave.Name = "ButSave";
            this.ButSave.Size = new System.Drawing.Size(64, 31);
            this.ButSave.TabIndex = 47;
            this.ButSave.Text = "Save";
            this.ButSave.UseVisualStyleBackColor = true;
            this.ButSave.Click += new System.EventHandler(this.ButSave_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(76, 71);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(253, 17);
            this.checkBox1.TabIndex = 75;
            this.checkBox1.Text = "Display Always that view when using  this config";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(82, 47);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(78, 13);
            this.label21.TabIndex = 67;
            this.label21.Text = "Default LayOut";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "List",
            "Small Icons",
            "Large Icons",
            "Filmstrip"});
            this.comboBox1.Location = new System.Drawing.Point(166, 44);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(112, 21);
            this.comboBox1.TabIndex = 66;
            this.comboBox1.Text = "List";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(10, 18);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(146, 21);
            this.comboBox2.TabIndex = 16;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(166, 18);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(170, 20);
            this.textBox2.TabIndex = 17;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MesFilms.Properties.Resources.film_reel_128x128;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(53, 52);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 75;
            this.pictureBox1.TabStop = false;
            // 
            // MesFilmsSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 495);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Config_Menu);
            this.Controls.Add(this.ButSave);
            this.Controls.Add(this.Config_Dflt);
            this.Controls.Add(this.ButDelet);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.Config_Name);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label_VersionNumber);
            this.Controls.Add(this.General);
            this.Controls.Add(this.ButQuit);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MesFilmsSetup";
            this.Text = "MesFilmsSetup";
            this.ToolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.MesFilmsSetup_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MesFilmsSetup_Quit);
            this.groupBox_SortByItem.ResumeLayout(false);
            this.groupBox_SortByItem.PerformLayout();
            this.groupBox_AntSelectedEnreg.ResumeLayout(false);
            this.groupBox_AntSelectedEnreg.PerformLayout();
            this.groupBox_DefaultView.ResumeLayout(false);
            this.groupBox_DefaultView.PerformLayout();
            this.groupBox_SupplementaryView.ResumeLayout(false);
            this.groupBox_SupplementaryView.PerformLayout();
            this.groupBox_DetailedSupplementaryInformations.ResumeLayout(false);
            this.groupBox_DetailedSupplementaryInformations.PerformLayout();
            this.groupBox_DatabaseUpdateOptions.ResumeLayout(false);
            this.groupBox_DatabaseUpdateOptions.PerformLayout();
            this.groupBox_AntUpdatingItems.ResumeLayout(false);
            this.groupBox_AntUpdatingItems.PerformLayout();
            this.gpspfield.ResumeLayout(false);
            this.gpspfield.PerformLayout();
            this.gpsuppress.ResumeLayout(false);
            this.gpsuppress.PerformLayout();
            this.groupBox_Separators.ResumeLayout(false);
            this.groupBox_Separators.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox20.ResumeLayout(false);
            this.groupBox20.PerformLayout();
            this.groupBox_GrabberOptions.ResumeLayout(false);
            this.groupBox_GrabberOptions.PerformLayout();
            this.Fanart.ResumeLayout(false);
            this.Fanart.PerformLayout();
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.groupBox_SupplementarySearch.ResumeLayout(false);
            this.groupBox_SupplementarySearch.PerformLayout();
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox24.ResumeLayout(false);
            this.groupBox24.PerformLayout();
            this.groupBox25.ResumeLayout(false);
            this.groupBox25.PerformLayout();
            this.groupBox_PreLaunchingCommand.ResumeLayout(false);
            this.groupBox_PreLaunchingCommand.PerformLayout();
            this.groupBox_ArtistImages.ResumeLayout(false);
            this.groupBox_ArtistImages.PerformLayout();
            this.General.ResumeLayout(false);
            this.Tab_General.ResumeLayout(false);
            this.Tab_General.PerformLayout();
            this.groupBox_Security.ResumeLayout(false);
            this.groupBox_Security.PerformLayout();
            this.groupBox_PlayMovieInfos.ResumeLayout(false);
            this.groupBox_PlayMovieInfos.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox_TitleOrder.ResumeLayout(false);
            this.groupBox_TitleOrder.PerformLayout();
            this.Tab_Artwork.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.Tab_Optional1.ResumeLayout(false);
            this.Tab_Optional2.ResumeLayout(false);
            this.Tab_DVDprofilerMovieCollector.ResumeLayout(false);
            this.groupBox_MovieCollector.ResumeLayout(false);
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            this.groupBox_DVDprofiler.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.Tab_Grabber.ResumeLayout(false);
            this.Tab_Logos.ResumeLayout(false);
            this.Tab_Logos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPicture)).EndInit();
            this.Tab_TrailerOptions.ResumeLayout(false);
            this.Tab_WakeOnLan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label_VersionNumber;
        private TextBox textBox1;
        private Label label11;
        private ComboBox Config_Name;
        private Label label12;
        private TabControl General;
        private TabPage Tab_General;
        private TabPage Tab_Optional1;
        private Button ButDelet;
        private Button ButQuit;
        private GroupBox groupBox_DetailedSupplementaryInformations;
        private ComboBox AntItem3;
        private TextBox AntLabel2;
        private ComboBox AntItem2;
        private Label label7;
        private Label label3;
        private TextBox AntLabel1;
        private ComboBox AntItem1;
        private TabPage Tab_DVDprofilerMovieCollector;
        private ComboBox ItemSearchFileName;
        private CheckBox SearchFileName;
        private Label label13;
        private ComboBox CatalogType;
        private GroupBox groupBox_TitleOrder;
        private TextBox TitleDelim;
        private Label label9;
        private Label label8;
        private ComboBox AntTitle2;
        private ComboBox AntTitle1;
        private TextBox AntIdentLabel;
        private Button butPath;
        private TextBox PathStorage;
        private Label label5;
        private Label label6;
        private ComboBox AntStorage;
        private Label label4;
        private ComboBox AntIdentItem;
        private Button ButCat;
        private TextBox MesFilmsCat;
        private Label label1;
        private GroupBox groupBox_SortByItem;
        private ComboBox AntSort1;
        private TextBox AntTSort1;
        private GroupBox groupBox_AntSelectedEnreg;
        private ComboBox AntFilterComb;
        private ComboBox AntFilterSign4;
        private ComboBox AntFilterSign3;
        private ComboBox AntFilterSign2;
        private ComboBox AntFilterSign1;
        private ComboBox AntFilterItem2;
        private TextBox AntFilterText2;
        private ComboBox AntFilterMinRating;
        private ComboBox AntFilterItem1;
        private TextBox AntFilterText1;
        private ComboBox AntFilterItem3;
        private TextBox AntFilterText3;
        private ComboBox AntFilterItem4;
        private TextBox AntFilterText4;
        private GroupBox groupBox_DefaultView;
        private ComboBox View_Dflt_Item;
        private TextBox View_Dflt_Text;
        private GroupBox groupBox_SupplementaryView;
        private TextBox AntViewText2;
        private TextBox AntViewText1;
        private ComboBox AntViewItem2;
        private ComboBox AntViewItem1;
        private Button ButSave;
        private Label label17;
        private CheckBox Config_Dflt;
        private TextBox Rpt_Dwp;
        private TextBox Dwp;
        private GroupBox groupBox_PlayMovieInfos;
        private GroupBox groupBox9;
        private GroupBox groupBox10;
        private ComboBox DVDPTagField;
        private Label label14;
        private ComboBox LayOut;
        private GroupBox groupBox_Security;
        private Label label16;
        private Label label15;
        private CheckBox SortTitle;
        private CheckBox radioButton2;
        private CheckBox radioButton1;
        private CheckBox Config_Menu;
        private TextBox Selected_Enreg;
        private CheckBox AlwaysDefaultView;
        private GroupBox groupBox13;
        private CheckBox OnlyFile;
        private Label label20;
        private ComboBox AntSTitle;
        private TabPage Tab_Optional2;
        private GroupBox groupBox_Separators;
        private CheckBox checkBox1;
        private Label label21;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private TextBox textBox2;
        private Label label24;
        private ComboBox ListSeparator1;
        private ComboBox RoleSeparator2;
        private ComboBox RoleSeparator3;
        private Label label22;
        private ComboBox RoleSeparator1;
        private ComboBox ListSeparator3;
        private ComboBox ListSeparator2;
        private ComboBox RoleSeparator5;
        private ComboBox RoleSeparator4;
        private ComboBox ListSeparator5;
        private ComboBox ListSeparator4;
        private Label label25;
        private Label label23;
        private TextBox AntViewValue1;
        private TextBox AntViewValue2;
        private TextBox AntViewValue3;
        private TextBox AntViewText3;
        private ComboBox AntViewItem3;
        private GroupBox groupBox_MovieCollector;
        private GroupBox groupBox17;
        private GroupBox groupBox_DVDprofiler;
        private RadioButton Thumbnails;
        private RadioButton Images;
        private CheckBox SearchSubDirs;
        private TabPage Tab_Grabber;
        private GroupBox groupBox_GrabberOptions;
        private Button btnGrabber;
        private TextBox txtGrabber;
        private Label label27;
        private CheckBox chkGrabber;
        private Button btnAMCUpd_exe;
        private TextBox txtAMCUpd_exe;
        private Label label26;
        private CheckBox chkAMCUpd;
        private Button btnAMCUpd_cnf;
        private TextBox txtAMCUpd_cnf;
        private Label label28;
        private CheckBox chkGrabber_Always;
        private TabPage Tab_Artwork;
        private CheckBox chkDfltFanart;
        private Button btnFanart;
        private TextBox MesFilmsFanart;
        private Label labelFanart;
        private GroupBox Fanart;
        private CheckBox chkFanart;
        private TabPage Tab_Logos;
        private TextBox textBox3;
        private CheckBox chkLogos;
        private ComboBox SLogo_Type;
        private ComboBox SOp1;
        private ComboBox SField1;
        private ComboBox SValue1;
        private ComboBox SAnd_Or;
        private ComboBox SValue2;
        private ComboBox SOp2;
        private ComboBox SField2;
        private PictureBox SPicture;
        private Button btnDown;
        private Button btnUp;
        private Button btnDel;
        private Button btnAdd;
        private ListView LogoView;
        private ColumnHeader LogoType;
        private ColumnHeader Field1;
        private ColumnHeader Op1;
        private ColumnHeader Value1;
        private ColumnHeader And_Or;
        private ColumnHeader Field2;
        private ColumnHeader Op2;
        private ColumnHeader Value2;
        private ColumnHeader Image;
        private TextBox SFilePicture;
        private ColumnHeader PathImage;
        private GroupBox groupBox_DatabaseUpdateOptions;
        private RadioButton rbsuppress1;
        private CheckBox chkSuppress;
        private GroupBox gpsuppress;
        private TextBox txtfdupdate;
        private ComboBox cbfdupdate;
        private RadioButton rbsuppress2;
        private RadioButton rbsuppress4;
        private RadioButton rbsuppress3;
        private GroupBox gpspfield;
        private CheckBox chksupplaystop;
        private Button btnLogosPath;
        private TextBox txtLogosPath;
        private Label lblLogosPath;
        private ToolTip ToolTip1;
        private TextBox AntViewValue5;
        private TextBox AntViewText5;
        private ComboBox AntViewItem5;
        private TextBox AntViewValue4;
        private TextBox AntViewText4;
        private ComboBox AntViewItem4;
        private GroupBox groupBox_AntUpdatingItems;
        private TextBox AntUpdDflT2;
        private TextBox AntUpdDflT1;
        private ComboBox AntUpdItem2;
        private TextBox AntUpdText2;
        private ComboBox AntUpdItem1;
        private TextBox AntUpdText1;
        private ComboBox AntSort2;
        private TextBox AntTSort2;
        private Button btnDirGrab;
        private TextBox txtDirGrab;
        private Label label31;
        private Label label32;
        private ComboBox Sort;
        private ComboBox SortSens;
        private GroupBox groupBox20;
        private Button btnParameters;
        private CheckBox scheduleAMCUpdater;
        private Button btnViews;
        private TextBox MesFilmsViews;
        private Label label29;
        private GroupBox groupBox22;
        private CheckBox chkDfltViews;
        private CheckBox chkViews;
        private CheckBox chkOnlyTitle;
        private CheckBox chkGrabber_ChooseScript;
        private CheckBox chkWindowsFileDialog;
        private Button btnResetThumbs;
        private GroupBox groupBox_SupplementarySearch;
        private Label label18;
        private TextBox AntSearchList;
        private ComboBox AntSearchItem2;
        private ComboBox AntSearchField;
        private TextBox AntSearchText2;
        private ComboBox AntSearchItem1;
        private TextBox AntSearchText1;
        private Label label33;
        private TextBox AntUpdList;
        private ComboBox AntUpdField;
        private TabPage Tab_TrailerOptions;
        private GroupBox groupBox23;
        private Button btnTrailer;
        private TextBox PathStorageTrailer;
        private Label label34;
        private GroupBox groupBox24;
        private ComboBox AntStorageTrailer;
        private Label label35;
        private CheckBox SearchSubDirsTrailer;
        private TabPage Tab_WakeOnLan;
        private GroupBox groupBox25;
        private CheckBox check_WOL_enable;
        private TextBox NAS_Name_1;
        private Label label36;
        private CheckBox check_WOL_Userdialog;
        private Label label19;
        private Label label_NAS_Server_1_Name;
        private Label label_NAS_Server_1_MAC;
        private TextBox NAS_MAC_1;
        private Label label_NAS_Server_3_MAC;
        private TextBox NAS_MAC_3;
        private Label label_NAS_Server_3_Name;
        private TextBox NAS_Name_3;
        private Label label_NAS_Server_2_MAC;
        private TextBox NAS_MAC_2;
        private Label label_NAS_Server_2_Name;
        private TextBox NAS_Name_2;
        private Label label_Security;
        private GroupBox groupBox1;
        private Button ButDefCov;
        private TextBox DefaultCover;
        private Label label_DefaulCover;
        private Button ButImg;
        private TextBox MesFilmsImg;
        private Label label2;
        private ComboBox ItemSearchFileNameTrailer;
        private CheckBox SearchFileNameTrailer;
        private CheckBox CheckWatched;
        private GroupBox groupBox_ArtistImages;
        private TextBox DefaultCoverArtist;
        private Label label_ArtistImagePath;
        private Button ButImgArtist;
        private TextBox MesFilmsImgArtist;
        private Label label_DefaultArtistImage;
        private Button ButDefCovArtist;
        private GroupBox groupBox_nfoGrabber;
        private CheckBox ShowTrailerPlayDialog;
        private Button AntUpdFieldReset;
        private Button AntSearchFieldReset;
        private GroupBox groupBox_PreLaunchingCommand;
        private ComboBox CmdPar;
        private TextBox CmdExe;
        private Label label10;
        private CheckBox ShowTrailerWhenStartingMovie;
        private CheckBox chkDfltArtist;
        private Button btnResetThumbsArtist;
        private Button buttonGetMACadresses;
        private Button buttonSendMagicPacket1;
        private Button buttonSendMagicPacket3;
        private Button buttonSendMagicPacket2;
        private Label label30;
        private Label label37;
        private ComboBox comboWOLtimeout;
        private PictureBox pictureBox1;
        private GroupBox groupBox2;
        private TextBox UserProfileName;
        private CheckBox checkWatchedInProfile;
        private Label Label_UserProfileName;
        private Label label39;
        private Label label38;
        private CheckBox chkGlobalUnwatchedOnly;

    }
}