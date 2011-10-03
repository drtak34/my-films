namespace MyFilmsPlugin.MyFilms.Configuration
{
  using System.Windows.Forms;

  using MyFilmsPlugin.MyFilms.Configuration;

  using MyFilmsPlugin.MyFilms.Utils;

  partial class MyFilmsSetup
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
            return "MyFilms";
        }

        public string Description()
        {
            return "MyFilms Ant Movie Catalog";
        }

        public string Author()
        {
            return "Zebons (Mod by Guzzi)";
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
                strButtonText = "MyFilms";
            }
            strButtonImage = "";
            strButtonImageFocus = "";
            strPictureImage = "hover_MyFilms.png";
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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyFilmsSetup));
          this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
          this.Config_Name = new System.Windows.Forms.ComboBox();
          this.textBoxPluginName = new System.Windows.Forms.TextBox();
          this.ButQuit = new System.Windows.Forms.Button();
          this.ButDelet = new System.Windows.Forms.Button();
          this.Config_Dflt = new System.Windows.Forms.CheckBox();
          this.Config_Menu = new System.Windows.Forms.CheckBox();
          this.label_VersionNumber = new System.Windows.Forms.Label();
          this.ButCopy = new System.Windows.Forms.Button();
          this.linkLabelMyFilmsWiki = new System.Windows.Forms.LinkLabel();
          this.Tab_Trakt = new System.Windows.Forms.TabPage();
          this.groupBoxTrakt = new System.Windows.Forms.GroupBox();
          this.linkLabelUsingTraktInMyFilmsWiki = new System.Windows.Forms.LinkLabel();
          this.linkLabelTraktWiki = new System.Windows.Forms.LinkLabel();
          this.labelTraktDescription = new System.Windows.Forms.Label();
          this.cbAllowTraktSync = new System.Windows.Forms.CheckBox();
          this.pictureBox1 = new System.Windows.Forms.PictureBox();
          this.linkLabelTrakt = new System.Windows.Forms.LinkLabel();
          this.groupBox9 = new System.Windows.Forms.GroupBox();
          this.chkDVDprofilerMergeWithGenreField = new System.Windows.Forms.CheckBox();
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
          this.check_WOL_enable = new System.Windows.Forms.CheckBox();
          this.NAS_Name_1 = new System.Windows.Forms.TextBox();
          this.label36 = new System.Windows.Forms.Label();
          this.SLogo_Type = new System.Windows.Forms.ComboBox();
          this.chkLogos = new System.Windows.Forms.CheckBox();
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
          this.txtLogosPath = new System.Windows.Forms.TextBox();
          this.comboBoxLogoSpacing = new System.Windows.Forms.ComboBox();
          this.comboBoxLogoPresets = new System.Windows.Forms.ComboBox();
          this.btnLogoClearCache = new System.Windows.Forms.Button();
          this.Fanart = new System.Windows.Forms.GroupBox();
          this.chkFanartDefaultViewsUseRandom = new System.Windows.Forms.CheckBox();
          this.chkFanartDefaultViews = new System.Windows.Forms.CheckBox();
          this.chkDfltFanartImageAll = new System.Windows.Forms.CheckBox();
          this.label48 = new System.Windows.Forms.Label();
          this.chkDfltFanartImage = new System.Windows.Forms.CheckBox();
          this.ButDefFanart = new System.Windows.Forms.Button();
          this.DefaultFanartImage = new System.Windows.Forms.TextBox();
          this.lblResultingGroupViewsPathFanart = new System.Windows.Forms.Label();
          this.label40 = new System.Windows.Forms.Label();
          this.chkFanart = new System.Windows.Forms.CheckBox();
          this.btnFanart = new System.Windows.Forms.Button();
          this.MesFilmsFanart = new System.Windows.Forms.TextBox();
          this.labelFanart = new System.Windows.Forms.Label();
          this.chkDfltFanart = new System.Windows.Forms.CheckBox();
          this.groupBox22 = new System.Windows.Forms.GroupBox();
          this.chkDfltViewsAll = new System.Windows.Forms.CheckBox();
          this.label47 = new System.Windows.Forms.Label();
          this.ButDefCovViews = new System.Windows.Forms.Button();
          this.DefaultCoverViews = new System.Windows.Forms.TextBox();
          this.btnResetThumbs = new System.Windows.Forms.Button();
          this.chkViews = new System.Windows.Forms.CheckBox();
          this.btnViews = new System.Windows.Forms.Button();
          this.chkDfltViews = new System.Windows.Forms.CheckBox();
          this.label29 = new System.Windows.Forms.Label();
          this.MesFilmsViews = new System.Windows.Forms.TextBox();
          this.groupBox1 = new System.Windows.Forms.GroupBox();
          this.lblPicturePrefix = new System.Windows.Forms.Label();
          this.txtPicturePrefix = new System.Windows.Forms.TextBox();
          this.lblPictureHandling = new System.Windows.Forms.Label();
          this.cbPictureHandling = new System.Windows.Forms.ComboBox();
          this.ButDefCov = new System.Windows.Forms.Button();
          this.DefaultCover = new System.Windows.Forms.TextBox();
          this.label_DefaulCover = new System.Windows.Forms.Label();
          this.ButImg = new System.Windows.Forms.Button();
          this.MesFilmsImg = new System.Windows.Forms.TextBox();
          this.label2 = new System.Windows.Forms.Label();
          this.groupBox_ArtistImages = new System.Windows.Forms.GroupBox();
          this.chkPersons = new System.Windows.Forms.CheckBox();
          this.btnResetThumbsArtist = new System.Windows.Forms.Button();
          this.chkDfltArtist = new System.Windows.Forms.CheckBox();
          this.ButDefCovArtist = new System.Windows.Forms.Button();
          this.ButImgArtist = new System.Windows.Forms.Button();
          this.MesFilmsImgArtist = new System.Windows.Forms.TextBox();
          this.label_DefaultArtistImage = new System.Windows.Forms.Label();
          this.label_ArtistImagePath = new System.Windows.Forms.Label();
          this.DefaultCoverArtist = new System.Windows.Forms.TextBox();
          this.groupBox_GrabberOptions = new System.Windows.Forms.GroupBox();
          this.txtGrabberDisplay = new System.Windows.Forms.TextBox();
          this.btnEditScript = new System.Windows.Forms.Button();
          this.groupBox6 = new System.Windows.Forms.GroupBox();
          this.cbGrabberOverrideGetRoles = new System.Windows.Forms.ComboBox();
          this.label55 = new System.Windows.Forms.Label();
          this.label51 = new System.Windows.Forms.Label();
          this.cbGrabberOverrideTitleLimit = new System.Windows.Forms.ComboBox();
          this.label50 = new System.Windows.Forms.Label();
          this.label49 = new System.Windows.Forms.Label();
          this.cbGrabberOverridePersonLimit = new System.Windows.Forms.ComboBox();
          this.cbGrabberOverrideLanguage = new System.Windows.Forms.ComboBox();
          this.lblSearchGrabberName = new System.Windows.Forms.Label();
          this.ItemSearchGrabberName = new System.Windows.Forms.ComboBox();
          this.chkGrabber_ChooseScript = new System.Windows.Forms.CheckBox();
          this.chkGrabber_Always = new System.Windows.Forms.CheckBox();
          this.btnGrabber = new System.Windows.Forms.Button();
          this.txtGrabber = new System.Windows.Forms.TextBox();
          this.label27 = new System.Windows.Forms.Label();
          this.groupBox_AMCupdater = new System.Windows.Forms.GroupBox();
          this.groupBox_AMCupdaterScheduer = new System.Windows.Forms.GroupBox();
          this.txtAMCUpd_cnf_Display = new System.Windows.Forms.TextBox();
          this.scheduleAMCUpdater = new System.Windows.Forms.CheckBox();
          this.label28 = new System.Windows.Forms.Label();
          this.btnParameters = new System.Windows.Forms.Button();
          this.txtAMCUpd_cnf = new System.Windows.Forms.TextBox();
          this.btnAMCUpd_cnf = new System.Windows.Forms.Button();
          this.groupBox_AMCupdater_ExternalApplication = new System.Windows.Forms.GroupBox();
          this.label56 = new System.Windows.Forms.Label();
          this.AmcTitleSearchHandling = new System.Windows.Forms.ComboBox();
          this.btnCreateAMCDefaultConfig = new System.Windows.Forms.Button();
          this.lblAMCMovieScanPath = new System.Windows.Forms.Label();
          this.chkAMC_Purge_Missing_Files = new System.Windows.Forms.CheckBox();
          this.btnAMCMovieScanPathAdd = new System.Windows.Forms.Button();
          this.AMCMovieScanPath = new System.Windows.Forms.TextBox();
          this.btnCreateAMCDesktopIcon = new System.Windows.Forms.Button();
          this.chkAMCUpd = new System.Windows.Forms.CheckBox();
          this.groupBoxDeletionOptions = new System.Windows.Forms.GroupBox();
          this.chkSuppress = new System.Windows.Forms.CheckBox();
          this.gpsuppress = new System.Windows.Forms.GroupBox();
          this.rbsuppress2 = new System.Windows.Forms.RadioButton();
          this.rbsuppress4 = new System.Windows.Forms.RadioButton();
          this.rbsuppress3 = new System.Windows.Forms.RadioButton();
          this.rbsuppress1 = new System.Windows.Forms.RadioButton();
          this.gpspfield = new System.Windows.Forms.GroupBox();
          this.chksupplaystop = new System.Windows.Forms.CheckBox();
          this.txtfdupdate = new System.Windows.Forms.TextBox();
          this.lblUpdateValue = new System.Windows.Forms.Label();
          this.cbfdupdate = new System.Windows.Forms.ComboBox();
          this.groupBoxUpdateByProperties = new System.Windows.Forms.GroupBox();
          this.AntUpdFieldReset = new System.Windows.Forms.Button();
          this.AntUpdFieldClear = new System.Windows.Forms.Button();
          this.label33 = new System.Windows.Forms.Label();
          this.AntUpdField = new System.Windows.Forms.ComboBox();
          this.AntUpdList = new System.Windows.Forms.TextBox();
          this.groupBox2 = new System.Windows.Forms.GroupBox();
          this.btnWatchedImport = new System.Windows.Forms.Button();
          this.btnWatchedExport = new System.Windows.Forms.Button();
          this.lblUnwatchedItemsValue = new System.Windows.Forms.Label();
          this.textBoxGlobalUnwatchedOnlyValue = new System.Windows.Forms.TextBox();
          this.CheckWatchedPlayerStopped = new System.Windows.Forms.CheckBox();
          this.Label_UserProfileName = new System.Windows.Forms.Label();
          this.UserProfileName = new System.Windows.Forms.TextBox();
          this.label19 = new System.Windows.Forms.Label();
          this.cbWatched = new System.Windows.Forms.ComboBox();
          this.chkEnhancedWatchedStatusHandling = new System.Windows.Forms.CheckBox();
          this.CheckWatched = new System.Windows.Forms.CheckBox();
          this.groupBox_AntUpdatingItems = new System.Windows.Forms.GroupBox();
          this.label46 = new System.Windows.Forms.Label();
          this.label45 = new System.Windows.Forms.Label();
          this.label44 = new System.Windows.Forms.Label();
          this.chkWindowsFileDialog = new System.Windows.Forms.CheckBox();
          this.AntUpdDflT2 = new System.Windows.Forms.TextBox();
          this.AntUpdDflT1 = new System.Windows.Forms.TextBox();
          this.AntUpdItem2 = new System.Windows.Forms.ComboBox();
          this.AntUpdText2 = new System.Windows.Forms.TextBox();
          this.AntUpdItem1 = new System.Windows.Forms.ComboBox();
          this.AntUpdText1 = new System.Windows.Forms.TextBox();
          this.Tab_Search = new System.Windows.Forms.TabPage();
          this.groupBox_SortByItem = new System.Windows.Forms.GroupBox();
          this.AntSort2 = new System.Windows.Forms.ComboBox();
          this.AntTSort2 = new System.Windows.Forms.TextBox();
          this.AntSort1 = new System.Windows.Forms.ComboBox();
          this.AntTSort1 = new System.Windows.Forms.TextBox();
          this.groupBoxSearchByProperties = new System.Windows.Forms.GroupBox();
          this.AntSearchFieldReset = new System.Windows.Forms.Button();
          this.AntSearchFieldClear = new System.Windows.Forms.Button();
          this.label18 = new System.Windows.Forms.Label();
          this.AntSearchField = new System.Windows.Forms.ComboBox();
          this.AntSearchList = new System.Windows.Forms.TextBox();
          this.groupBox_SupplementarySearch = new System.Windows.Forms.GroupBox();
          this.label43 = new System.Windows.Forms.Label();
          this.label42 = new System.Windows.Forms.Label();
          this.lblUserdefinedSearchItems = new System.Windows.Forms.Label();
          this.AntSearchItem2 = new System.Windows.Forms.ComboBox();
          this.AntSearchText2 = new System.Windows.Forms.TextBox();
          this.AntSearchItem1 = new System.Windows.Forms.ComboBox();
          this.AntSearchText1 = new System.Windows.Forms.TextBox();
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
          this.Tab_Views = new System.Windows.Forms.TabPage();
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
          this.chkGlobalAvailableOnly = new System.Windows.Forms.CheckBox();
          this.chkUseListviewForGroups = new System.Windows.Forms.CheckBox();
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
          this.label41 = new System.Windows.Forms.Label();
          this.label31 = new System.Windows.Forms.Label();
          this.label26 = new System.Windows.Forms.Label();
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
          this.AntLabel5 = new System.Windows.Forms.TextBox();
          this.AntLabel4 = new System.Windows.Forms.TextBox();
          this.AntItem5 = new System.Windows.Forms.ComboBox();
          this.AntItem4 = new System.Windows.Forms.ComboBox();
          this.AntLabel3 = new System.Windows.Forms.TextBox();
          this.chkOnlyTitle = new System.Windows.Forms.CheckBox();
          this.AntItem3 = new System.Windows.Forms.ComboBox();
          this.AntItem2 = new System.Windows.Forms.ComboBox();
          this.AntLabel2 = new System.Windows.Forms.TextBox();
          this.label7 = new System.Windows.Forms.Label();
          this.AntItem1 = new System.Windows.Forms.ComboBox();
          this.label3 = new System.Windows.Forms.Label();
          this.AntLabel1 = new System.Windows.Forms.TextBox();
          this.Tab_Trailer = new System.Windows.Forms.TabPage();
          this.groupBox24 = new System.Windows.Forms.GroupBox();
          this.labelTrailers = new System.Windows.Forms.Label();
          this.SearchSubDirsTrailer = new System.Windows.Forms.CheckBox();
          this.ShowTrailerWhenStartingMovie = new System.Windows.Forms.CheckBox();
          this.btnTrailer = new System.Windows.Forms.Button();
          this.ShowTrailerPlayDialog = new System.Windows.Forms.CheckBox();
          this.PathStorageTrailer = new System.Windows.Forms.TextBox();
          this.SearchFileNameTrailer = new System.Windows.Forms.CheckBox();
          this.label34 = new System.Windows.Forms.Label();
          this.ItemSearchFileNameTrailer = new System.Windows.Forms.ComboBox();
          this.AntStorageTrailer = new System.Windows.Forms.ComboBox();
          this.label35 = new System.Windows.Forms.Label();
          this.Tab_General = new System.Windows.Forms.TabPage();
          this.groupBoxSetupWizard = new System.Windows.Forms.GroupBox();
          this.btnFirstTimeSetup = new System.Windows.Forms.Button();
          this.lblYellowShowRequiredItems = new System.Windows.Forms.Label();
          this.groupBox_Security = new System.Windows.Forms.GroupBox();
          this.label_Security = new System.Windows.Forms.Label();
          this.label16 = new System.Windows.Forms.Label();
          this.label15 = new System.Windows.Forms.Label();
          this.Rpt_Dwp = new System.Windows.Forms.TextBox();
          this.Dwp = new System.Windows.Forms.TextBox();
          this.groupBox_PreLaunchingCommand = new System.Windows.Forms.GroupBox();
          this.label39 = new System.Windows.Forms.Label();
          this.label38 = new System.Windows.Forms.Label();
          this.CmdPar = new System.Windows.Forms.ComboBox();
          this.CmdExe = new System.Windows.Forms.TextBox();
          this.groupBox_PlayMovieInfos = new System.Windows.Forms.GroupBox();
          this.chkScanMediaOnStart = new System.Windows.Forms.CheckBox();
          this.groupBoxMoviePathInfos = new System.Windows.Forms.GroupBox();
          this.SearchOnlyExactMatches = new System.Windows.Forms.CheckBox();
          this.label5 = new System.Windows.Forms.Label();
          this.PathStorage = new System.Windows.Forms.TextBox();
          this.SearchSubDirs = new System.Windows.Forms.CheckBox();
          this.butPath = new System.Windows.Forms.Button();
          this.SearchFileName = new System.Windows.Forms.CheckBox();
          this.ItemSearchFileName = new System.Windows.Forms.ComboBox();
          this.AntIdentLabel = new System.Windows.Forms.TextBox();
          this.label6 = new System.Windows.Forms.Label();
          this.AntStorage = new System.Windows.Forms.ComboBox();
          this.label4 = new System.Windows.Forms.Label();
          this.AntIdentItem = new System.Windows.Forms.ComboBox();
          this.label13 = new System.Windows.Forms.Label();
          this.CatalogType = new System.Windows.Forms.ComboBox();
          this.groupBox_TitleOrder = new System.Windows.Forms.GroupBox();
          this.chkUseOriginalAsTranslatedTitle = new System.Windows.Forms.CheckBox();
          this.label20 = new System.Windows.Forms.Label();
          this.AntSTitle = new System.Windows.Forms.ComboBox();
          this.label17 = new System.Windows.Forms.Label();
          this.TitleDelim = new System.Windows.Forms.TextBox();
          this.label9 = new System.Windows.Forms.Label();
          this.label8 = new System.Windows.Forms.Label();
          this.AntTitle2 = new System.Windows.Forms.ComboBox();
          this.AntTitle1 = new System.Windows.Forms.ComboBox();
          this.ButCat = new System.Windows.Forms.Button();
          this.MesFilmsCat = new System.Windows.Forms.TextBox();
          this.label1 = new System.Windows.Forms.Label();
          this.butNew = new System.Windows.Forms.Button();
          this.groupBoxExtendedFieldHandling = new System.Windows.Forms.GroupBox();
          this.ECMergeDestinationFieldWriter = new System.Windows.Forms.ComboBox();
          this.ECMergeDestinationFieldCertification = new System.Windows.Forms.ComboBox();
          this.ECMergeDestinationFieldTags = new System.Windows.Forms.ComboBox();
          this.chkAddWriter = new System.Windows.Forms.CheckBox();
          this.ECMergeDestinationFieldTagline = new System.Windows.Forms.ComboBox();
          this.chkAddTagline = new System.Windows.Forms.CheckBox();
          this.chkAddCertification = new System.Windows.Forms.CheckBox();
          this.label54 = new System.Windows.Forms.Label();
          this.chkAddTags = new System.Windows.Forms.CheckBox();
          this.label53 = new System.Windows.Forms.Label();
          this.btnLaunchAMCglobal = new System.Windows.Forms.Button();
          this.SPicture = new System.Windows.Forms.PictureBox();
          this.groupBox_DVDprofiler = new System.Windows.Forms.GroupBox();
          this.groupBox13 = new System.Windows.Forms.GroupBox();
          this.chkDVDprofilerOnlyFile = new System.Windows.Forms.CheckBox();
          this.buttonDeleteTmpCatalog = new System.Windows.Forms.Button();
          this.buttonOpenTmpFileAMC = new System.Windows.Forms.Button();
          this.AMCConfigView = new System.Windows.Forms.ListView();
          this.Option = new System.Windows.Forms.ColumnHeader();
          this.Value = new System.Windows.Forms.ColumnHeader();
          this.cbAllowRecentAddedAPI = new System.Windows.Forms.CheckBox();
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
          this.ButSave = new System.Windows.Forms.Button();
          this.checkBox1 = new System.Windows.Forms.CheckBox();
          this.label21 = new System.Windows.Forms.Label();
          this.comboBox1 = new System.Windows.Forms.ComboBox();
          this.comboBox2 = new System.Windows.Forms.ComboBox();
          this.textBox2 = new System.Windows.Forms.TextBox();
          this.Tab_ExternalCatalogs = new System.Windows.Forms.TabPage();
          this.groupBoxAMCsettings = new System.Windows.Forms.GroupBox();
          this.AMCexePath = new System.Windows.Forms.TextBox();
          this.lblAMCpath = new System.Windows.Forms.Label();
          this.buttonAMCpathSearch = new System.Windows.Forms.Button();
          this.buttonOpenTmpFile = new System.Windows.Forms.Button();
          this.Tab_WakeOnLan = new System.Windows.Forms.TabPage();
          this.Tab_Logos = new System.Windows.Forms.TabPage();
          this.btnUpdate = new System.Windows.Forms.Button();
          this.lblLogoPresets = new System.Windows.Forms.Label();
          this.lbl_LogoSpacing = new System.Windows.Forms.Label();
          this.lblSetupLogoRules = new System.Windows.Forms.Label();
          this.lblInfoLogosForAll = new System.Windows.Forms.Label();
          this.lblSelectLogoFile = new System.Windows.Forms.Label();
          this.btnLogosPath = new System.Windows.Forms.Button();
          this.SFilePicture = new System.Windows.Forms.TextBox();
          this.lblLogosPath = new System.Windows.Forms.Label();
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
          this.Tab_Artwork = new System.Windows.Forms.TabPage();
          this.Tab_AMCupdater = new System.Windows.Forms.TabPage();
          this.Tab_Update = new System.Windows.Forms.TabPage();
          this.General = new System.Windows.Forms.TabControl();
          this.Tab_Other = new System.Windows.Forms.TabPage();
          this.groupBox3 = new System.Windows.Forms.GroupBox();
          this.tbEditorStartParamsOutput = new System.Windows.Forms.TextBox();
          this.btnSaveEditorStartParams = new System.Windows.Forms.Button();
          this.label60 = new System.Windows.Forms.Label();
          this.tbEditorSearchExpression = new System.Windows.Forms.TextBox();
          this.label59 = new System.Windows.Forms.Label();
          this.cbEditorLayout = new System.Windows.Forms.ComboBox();
          this.label58 = new System.Windows.Forms.Label();
          this.label57 = new System.Windows.Forms.Label();
          this.label52 = new System.Windows.Forms.Label();
          this.btnLoadEditorValues = new System.Windows.Forms.Button();
          this.cbEditorViews = new System.Windows.Forms.ComboBox();
          this.cbEditorConfigs = new System.Windows.Forms.ComboBox();
          this.cbEditorViewValues = new System.Windows.Forms.ComboBox();
          this.groupBoxExternal = new System.Windows.Forms.GroupBox();
          this.labelRecentlyAddedAPI = new System.Windows.Forms.Label();
          this.btnGrabberInterface = new System.Windows.Forms.Button();
          this.lblAMCupdaterConfigPreview = new System.Windows.Forms.Label();
          this.textBoxNBconfigs = new System.Windows.Forms.TextBox();
          this.lblNbConfig = new System.Windows.Forms.Label();
          this.pictureBoxMyFilms = new System.Windows.Forms.PictureBox();
          this.chkSuppressManual = new System.Windows.Forms.CheckBox();
          this.label61 = new System.Windows.Forms.Label();
          this.Tab_Trakt.SuspendLayout();
          this.groupBoxTrakt.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
          this.groupBox9.SuspendLayout();
          this.groupBox25.SuspendLayout();
          this.Fanart.SuspendLayout();
          this.groupBox22.SuspendLayout();
          this.groupBox1.SuspendLayout();
          this.groupBox_ArtistImages.SuspendLayout();
          this.groupBox_GrabberOptions.SuspendLayout();
          this.groupBox6.SuspendLayout();
          this.groupBox_AMCupdater.SuspendLayout();
          this.groupBox_AMCupdaterScheduer.SuspendLayout();
          this.groupBox_AMCupdater_ExternalApplication.SuspendLayout();
          this.groupBoxDeletionOptions.SuspendLayout();
          this.gpsuppress.SuspendLayout();
          this.gpspfield.SuspendLayout();
          this.groupBoxUpdateByProperties.SuspendLayout();
          this.groupBox2.SuspendLayout();
          this.groupBox_AntUpdatingItems.SuspendLayout();
          this.Tab_Search.SuspendLayout();
          this.groupBox_SortByItem.SuspendLayout();
          this.groupBoxSearchByProperties.SuspendLayout();
          this.groupBox_SupplementarySearch.SuspendLayout();
          this.groupBox_Separators.SuspendLayout();
          this.Tab_Views.SuspendLayout();
          this.groupBox_AntSelectedEnreg.SuspendLayout();
          this.groupBox_DefaultView.SuspendLayout();
          this.groupBox_SupplementaryView.SuspendLayout();
          this.groupBox_DetailedSupplementaryInformations.SuspendLayout();
          this.Tab_Trailer.SuspendLayout();
          this.groupBox24.SuspendLayout();
          this.Tab_General.SuspendLayout();
          this.groupBoxSetupWizard.SuspendLayout();
          this.groupBox_Security.SuspendLayout();
          this.groupBox_PreLaunchingCommand.SuspendLayout();
          this.groupBox_PlayMovieInfos.SuspendLayout();
          this.groupBoxMoviePathInfos.SuspendLayout();
          this.groupBox_TitleOrder.SuspendLayout();
          this.groupBoxExtendedFieldHandling.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.SPicture)).BeginInit();
          this.groupBox_DVDprofiler.SuspendLayout();
          this.groupBox13.SuspendLayout();
          this.Tab_ExternalCatalogs.SuspendLayout();
          this.groupBoxAMCsettings.SuspendLayout();
          this.Tab_WakeOnLan.SuspendLayout();
          this.Tab_Logos.SuspendLayout();
          this.Tab_Artwork.SuspendLayout();
          this.Tab_AMCupdater.SuspendLayout();
          this.Tab_Update.SuspendLayout();
          this.General.SuspendLayout();
          this.Tab_Other.SuspendLayout();
          this.groupBox3.SuspendLayout();
          this.groupBoxExternal.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMyFilms)).BeginInit();
          this.SuspendLayout();
          // 
          // ToolTip1
          // 
          this.ToolTip1.AutoPopDelay = 15000;
          this.ToolTip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
          this.ToolTip1.InitialDelay = 500;
          this.ToolTip1.IsBalloon = true;
          this.ToolTip1.ReshowDelay = 100;
          this.ToolTip1.ToolTipTitle = "MyFilms Help ...";
          // 
          // Config_Name
          // 
          this.Config_Name.BackColor = System.Drawing.SystemColors.Info;
          this.Config_Name.FormattingEnabled = true;
          this.Config_Name.Location = new System.Drawing.Point(195, 39);
          this.Config_Name.Name = "Config_Name";
          this.Config_Name.Size = new System.Drawing.Size(172, 21);
          this.Config_Name.Sorted = true;
          this.Config_Name.TabIndex = 1;
          this.ToolTip1.SetToolTip(this.Config_Name, resources.GetString("Config_Name.ToolTip"));
          this.Config_Name.SelectedIndexChanged += new System.EventHandler(this.Config_Name_SelectedIndexChanged);
          this.Config_Name.Leave += new System.EventHandler(this.Config_Name_SelectedIndexChanged);
          // 
          // textBoxPluginName
          // 
          this.textBoxPluginName.Location = new System.Drawing.Point(195, 11);
          this.textBoxPluginName.Name = "textBoxPluginName";
          this.textBoxPluginName.Size = new System.Drawing.Size(172, 20);
          this.textBoxPluginName.TabIndex = 5;
          this.ToolTip1.SetToolTip(this.textBoxPluginName, "Name of the plugin displayed in MP.\r\nBy default Films, but you can choose a bette" +
                  "r name");
          // 
          // ButQuit
          // 
          this.ButQuit.Location = new System.Drawing.Point(679, 466);
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
          this.ButDelet.Location = new System.Drawing.Point(599, 466);
          this.ButDelet.Name = "ButDelet";
          this.ButDelet.Size = new System.Drawing.Size(64, 31);
          this.ButDelet.TabIndex = 39;
          this.ButDelet.Text = "Delete";
          this.ToolTip1.SetToolTip(this.ButDelet, "Delete all information for that configuration.");
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
          // 
          // label_VersionNumber
          // 
          this.label_VersionNumber.AutoSize = true;
          this.label_VersionNumber.ForeColor = System.Drawing.SystemColors.HotTrack;
          this.label_VersionNumber.Location = new System.Drawing.Point(650, 16);
          this.label_VersionNumber.Name = "label_VersionNumber";
          this.label_VersionNumber.Size = new System.Drawing.Size(89, 13);
          this.label_VersionNumber.TabIndex = 34;
          this.label_VersionNumber.Text = "Version x.x.x.xxxx";
          this.ToolTip1.SetToolTip(this.label_VersionNumber, "MyFilms Version Number");
          // 
          // ButCopy
          // 
          this.ButCopy.Location = new System.Drawing.Point(521, 466);
          this.ButCopy.Name = "ButCopy";
          this.ButCopy.Size = new System.Drawing.Size(64, 31);
          this.ButCopy.TabIndex = 76;
          this.ButCopy.Text = "Copy";
          this.ToolTip1.SetToolTip(this.ButCopy, "Creates a copy of the current selected configuration.\r\nCan be used e.g. if you wa" +
                  "nt to have more than one \r\nconfiguration available with different settings or fi" +
                  "lters, \r\nbut based on same movie DB.");
          this.ButCopy.UseVisualStyleBackColor = true;
          this.ButCopy.Click += new System.EventHandler(this.ButCopy_Click);
          // 
          // linkLabelMyFilmsWiki
          // 
          this.linkLabelMyFilmsWiki.AutoSize = true;
          this.linkLabelMyFilmsWiki.Location = new System.Drawing.Point(216, 475);
          this.linkLabelMyFilmsWiki.Name = "linkLabelMyFilmsWiki";
          this.linkLabelMyFilmsWiki.Size = new System.Drawing.Size(134, 13);
          this.linkLabelMyFilmsWiki.TabIndex = 78;
          this.linkLabelMyFilmsWiki.TabStop = true;
          this.linkLabelMyFilmsWiki.Text = "MyFilms Wiki Online Pages";
          this.ToolTip1.SetToolTip(this.linkLabelMyFilmsWiki, "Opens the MyFilms Wiki in your browser...");
          this.linkLabelMyFilmsWiki.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelMyFilmsWiki_LinkClicked);
          // 
          // Tab_Trakt
          // 
          this.Tab_Trakt.Controls.Add(this.groupBoxTrakt);
          this.Tab_Trakt.Location = new System.Drawing.Point(4, 22);
          this.Tab_Trakt.Name = "Tab_Trakt";
          this.Tab_Trakt.Padding = new System.Windows.Forms.Padding(3);
          this.Tab_Trakt.Size = new System.Drawing.Size(747, 354);
          this.Tab_Trakt.TabIndex = 10;
          this.Tab_Trakt.Text = "Trakt";
          this.ToolTip1.SetToolTip(this.Tab_Trakt, "Setup for Trakt user settings");
          this.Tab_Trakt.UseVisualStyleBackColor = true;
          this.Tab_Trakt.Visible = false;
          // 
          // groupBoxTrakt
          // 
          this.groupBoxTrakt.Controls.Add(this.linkLabelUsingTraktInMyFilmsWiki);
          this.groupBoxTrakt.Controls.Add(this.linkLabelTraktWiki);
          this.groupBoxTrakt.Controls.Add(this.labelTraktDescription);
          this.groupBoxTrakt.Controls.Add(this.cbAllowTraktSync);
          this.groupBoxTrakt.Controls.Add(this.pictureBox1);
          this.groupBoxTrakt.Controls.Add(this.linkLabelTrakt);
          this.groupBoxTrakt.Location = new System.Drawing.Point(29, 25);
          this.groupBoxTrakt.Name = "groupBoxTrakt";
          this.groupBoxTrakt.Size = new System.Drawing.Size(698, 296);
          this.groupBoxTrakt.TabIndex = 5;
          this.groupBoxTrakt.TabStop = false;
          this.groupBoxTrakt.Text = "Trakt ...";
          // 
          // linkLabelUsingTraktInMyFilmsWiki
          // 
          this.linkLabelUsingTraktInMyFilmsWiki.AutoSize = true;
          this.linkLabelUsingTraktInMyFilmsWiki.Location = new System.Drawing.Point(518, 172);
          this.linkLabelUsingTraktInMyFilmsWiki.Name = "linkLabelUsingTraktInMyFilmsWiki";
          this.linkLabelUsingTraktInMyFilmsWiki.Size = new System.Drawing.Size(113, 13);
          this.linkLabelUsingTraktInMyFilmsWiki.TabIndex = 5;
          this.linkLabelUsingTraktInMyFilmsWiki.TabStop = true;
          this.linkLabelUsingTraktInMyFilmsWiki.Text = "Using Trakt in MyFilms";
          this.linkLabelUsingTraktInMyFilmsWiki.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelUsingTraktInMyFilmsWiki_LinkClicked);
          // 
          // linkLabelTraktWiki
          // 
          this.linkLabelTraktWiki.AutoSize = true;
          this.linkLabelTraktWiki.Location = new System.Drawing.Point(518, 208);
          this.linkLabelTraktWiki.Name = "linkLabelTraktWiki";
          this.linkLabelTraktWiki.Size = new System.Drawing.Size(56, 13);
          this.linkLabelTraktWiki.TabIndex = 4;
          this.linkLabelTraktWiki.TabStop = true;
          this.linkLabelTraktWiki.Text = "Trakt Wiki";
          this.linkLabelTraktWiki.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelTraktWiki_LinkClicked);
          // 
          // labelTraktDescription
          // 
          this.labelTraktDescription.AutoSize = true;
          this.labelTraktDescription.Location = new System.Drawing.Point(35, 106);
          this.labelTraktDescription.Name = "labelTraktDescription";
          this.labelTraktDescription.Size = new System.Drawing.Size(407, 156);
          this.labelTraktDescription.TabIndex = 2;
          this.labelTraktDescription.Text = resources.GetString("labelTraktDescription.Text");
          // 
          // cbAllowTraktSync
          // 
          this.cbAllowTraktSync.AutoSize = true;
          this.cbAllowTraktSync.Location = new System.Drawing.Point(38, 50);
          this.cbAllowTraktSync.Name = "cbAllowTraktSync";
          this.cbAllowTraktSync.Size = new System.Drawing.Size(242, 17);
          this.cbAllowTraktSync.TabIndex = 0;
          this.cbAllowTraktSync.Text = "Enable TRAKT Synchronisation for this config";
          this.ToolTip1.SetToolTip(this.cbAllowTraktSync, resources.GetString("cbAllowTraktSync.ToolTip"));
          this.cbAllowTraktSync.UseVisualStyleBackColor = true;
          // 
          // pictureBox1
          // 
          this.pictureBox1.Image = global::MyFilmsPlugin.Properties.Resources.trakt;
          this.pictureBox1.Location = new System.Drawing.Point(521, 32);
          this.pictureBox1.Name = "pictureBox1";
          this.pictureBox1.Size = new System.Drawing.Size(52, 50);
          this.pictureBox1.TabIndex = 3;
          this.pictureBox1.TabStop = false;
          // 
          // linkLabelTrakt
          // 
          this.linkLabelTrakt.AutoSize = true;
          this.linkLabelTrakt.Location = new System.Drawing.Point(518, 137);
          this.linkLabelTrakt.Name = "linkLabelTrakt";
          this.linkLabelTrakt.Size = new System.Drawing.Size(74, 13);
          this.linkLabelTrakt.TabIndex = 1;
          this.linkLabelTrakt.TabStop = true;
          this.linkLabelTrakt.Text = "Trakt Website";
          this.linkLabelTrakt.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelTrakt_LinkClicked);
          // 
          // groupBox9
          // 
          this.groupBox9.Controls.Add(this.chkDVDprofilerMergeWithGenreField);
          this.groupBox9.Location = new System.Drawing.Point(13, 86);
          this.groupBox9.Name = "groupBox9";
          this.groupBox9.Size = new System.Drawing.Size(307, 59);
          this.groupBox9.TabIndex = 0;
          this.groupBox9.TabStop = false;
          this.groupBox9.Text = "Tags ...";
          this.ToolTip1.SetToolTip(this.groupBox9, "To merge the DVD Profiler Tags field data with the My Films category field, enabl" +
                  "e this option. \r\nAlternatively you may use the general External Catalogs options" +
                  " to add Tags to another field.");
          // 
          // chkDVDprofilerMergeWithGenreField
          // 
          this.chkDVDprofilerMergeWithGenreField.AutoSize = true;
          this.chkDVDprofilerMergeWithGenreField.Location = new System.Drawing.Point(23, 24);
          this.chkDVDprofilerMergeWithGenreField.Name = "chkDVDprofilerMergeWithGenreField";
          this.chkDVDprofilerMergeWithGenreField.Size = new System.Drawing.Size(151, 17);
          this.chkDVDprofilerMergeWithGenreField.TabIndex = 12;
          this.chkDVDprofilerMergeWithGenreField.Text = "Merge With Category Field";
          this.chkDVDprofilerMergeWithGenreField.UseVisualStyleBackColor = true;
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
          this.check_WOL_Userdialog.Size = new System.Drawing.Size(346, 17);
          this.check_WOL_Userdialog.TabIndex = 75;
          this.check_WOL_Userdialog.Text = "Enable WakeOnLAN User Dialog (Autoinvoke WOL when disabled)";
          this.ToolTip1.SetToolTip(this.check_WOL_Userdialog, "Enable WakeOnLAN User Dialog when launching a movie");
          this.check_WOL_Userdialog.UseVisualStyleBackColor = true;
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
          this.ToolTip1.SetToolTip(this.label36, "MyFilms will autodetect server name from file path, \r\nif UNC notation is used and" +
                  " server is defined here.");
          // 
          // SLogo_Type
          // 
          this.SLogo_Type.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
          this.SLogo_Type.Enabled = false;
          this.SLogo_Type.FormattingEnabled = true;
          this.SLogo_Type.Items.AddRange(new object[] {
            "ID2001",
            "ID2002",
            "ID2003"});
          this.SLogo_Type.Location = new System.Drawing.Point(9, 289);
          this.SLogo_Type.Name = "SLogo_Type";
          this.SLogo_Type.Size = new System.Drawing.Size(102, 21);
          this.SLogo_Type.TabIndex = 73;
          this.ToolTip1.SetToolTip(this.SLogo_Type, resources.GetString("SLogo_Type.ToolTip"));
          // 
          // chkLogos
          // 
          this.chkLogos.AutoSize = true;
          this.chkLogos.Location = new System.Drawing.Point(9, 10);
          this.chkLogos.Name = "chkLogos";
          this.chkLogos.Size = new System.Drawing.Size(91, 17);
          this.chkLogos.TabIndex = 74;
          this.chkLogos.Text = "Enable Logos";
          this.ToolTip1.SetToolTip(this.chkLogos, resources.GetString("chkLogos.ToolTip"));
          this.chkLogos.UseVisualStyleBackColor = true;
          this.chkLogos.CheckedChanged += new System.EventHandler(this.chkLogos_CheckedChanged);
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
          this.LogoView.Location = new System.Drawing.Point(7, 59);
          this.LogoView.MultiSelect = false;
          this.LogoView.Name = "LogoView";
          this.LogoView.Size = new System.Drawing.Size(734, 170);
          this.LogoView.TabIndex = 90;
          this.ToolTip1.SetToolTip(this.LogoView, resources.GetString("LogoView.ToolTip"));
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
          this.PathImage.Text = "Image Path (autosearch)";
          this.PathImage.Width = 155;
          // 
          // txtLogosPath
          // 
          this.txtLogosPath.Enabled = false;
          this.txtLogosPath.Location = new System.Drawing.Point(220, 33);
          this.txtLogosPath.Name = "txtLogosPath";
          this.txtLogosPath.Size = new System.Drawing.Size(469, 20);
          this.txtLogosPath.TabIndex = 92;
          this.ToolTip1.SetToolTip(this.txtLogosPath, resources.GetString("txtLogosPath.ToolTip"));
          this.txtLogosPath.TextChanged += new System.EventHandler(this.txtLogosPath_TextChanged);
          // 
          // comboBoxLogoSpacing
          // 
          this.comboBoxLogoSpacing.FormattingEnabled = true;
          this.comboBoxLogoSpacing.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "25"});
          this.comboBoxLogoSpacing.Location = new System.Drawing.Point(644, 8);
          this.comboBoxLogoSpacing.Name = "comboBoxLogoSpacing";
          this.comboBoxLogoSpacing.Size = new System.Drawing.Size(45, 21);
          this.comboBoxLogoSpacing.TabIndex = 100;
          this.ToolTip1.SetToolTip(this.comboBoxLogoSpacing, "You can choose the distance between the generated logos in \"pixel\".\r\nDefault is \"" +
                  "1\".");
          // 
          // comboBoxLogoPresets
          // 
          this.comboBoxLogoPresets.FormattingEnabled = true;
          this.comboBoxLogoPresets.Items.AddRange(new object[] {
            "Use Logos of currently selected skin",
            "Use MP logos",
            "Use MyFilms Logo Pack",
            "Define your path to logo image files"});
          this.comboBoxLogoPresets.Location = new System.Drawing.Point(220, 8);
          this.comboBoxLogoPresets.Name = "comboBoxLogoPresets";
          this.comboBoxLogoPresets.Size = new System.Drawing.Size(211, 21);
          this.comboBoxLogoPresets.TabIndex = 3;
          this.ToolTip1.SetToolTip(this.comboBoxLogoPresets, resources.GetString("comboBoxLogoPresets.ToolTip"));
          this.comboBoxLogoPresets.SelectedIndexChanged += new System.EventHandler(this.comboBoxLogoPresets_SelectedIndexChanged);
          // 
          // btnLogoClearCache
          // 
          this.btnLogoClearCache.Location = new System.Drawing.Point(461, 8);
          this.btnLogoClearCache.Name = "btnLogoClearCache";
          this.btnLogoClearCache.Size = new System.Drawing.Size(116, 21);
          this.btnLogoClearCache.TabIndex = 102;
          this.btnLogoClearCache.Text = "Clear Logo Cache";
          this.ToolTip1.SetToolTip(this.btnLogoClearCache, "If you have redefined logo parameters like spacing, you should clear the logo cac" +
                  "he,\r\nso the logos will be rebuild with the new parameters");
          this.btnLogoClearCache.UseVisualStyleBackColor = true;
          this.btnLogoClearCache.Click += new System.EventHandler(this.btnLogoClearCache_Click);
          // 
          // Fanart
          // 
          this.Fanart.Controls.Add(this.chkFanartDefaultViewsUseRandom);
          this.Fanart.Controls.Add(this.chkFanartDefaultViews);
          this.Fanart.Controls.Add(this.chkDfltFanartImageAll);
          this.Fanart.Controls.Add(this.label48);
          this.Fanart.Controls.Add(this.chkDfltFanartImage);
          this.Fanart.Controls.Add(this.ButDefFanart);
          this.Fanart.Controls.Add(this.DefaultFanartImage);
          this.Fanart.Controls.Add(this.lblResultingGroupViewsPathFanart);
          this.Fanart.Controls.Add(this.label40);
          this.Fanart.Controls.Add(this.chkFanart);
          this.Fanart.Controls.Add(this.btnFanart);
          this.Fanart.Controls.Add(this.MesFilmsFanart);
          this.Fanart.Controls.Add(this.labelFanart);
          this.Fanart.Controls.Add(this.chkDfltFanart);
          this.Fanart.Location = new System.Drawing.Point(6, 231);
          this.Fanart.Name = "Fanart";
          this.Fanart.Size = new System.Drawing.Size(735, 120);
          this.Fanart.TabIndex = 71;
          this.Fanart.TabStop = false;
          this.Fanart.Text = "Fanart (Backdrops)";
          this.ToolTip1.SetToolTip(this.Fanart, resources.GetString("Fanart.ToolTip"));
          // 
          // chkFanartDefaultViewsUseRandom
          // 
          this.chkFanartDefaultViewsUseRandom.AutoSize = true;
          this.chkFanartDefaultViewsUseRandom.Location = new System.Drawing.Point(150, 36);
          this.chkFanartDefaultViewsUseRandom.Name = "chkFanartDefaultViewsUseRandom";
          this.chkFanartDefaultViewsUseRandom.Size = new System.Drawing.Size(86, 30);
          this.chkFanartDefaultViewsUseRandom.TabIndex = 81;
          this.chkFanartDefaultViewsUseRandom.Text = "Use random \r\nfilm fanart";
          this.ToolTip1.SetToolTip(this.chkFanartDefaultViewsUseRandom, "If selected, MyFilms will show random fanart from the movies contained in the sel" +
                  "ected group.");
          this.chkFanartDefaultViewsUseRandom.UseVisualStyleBackColor = true;
          // 
          // chkFanartDefaultViews
          // 
          this.chkFanartDefaultViews.AutoSize = true;
          this.chkFanartDefaultViews.Location = new System.Drawing.Point(37, 36);
          this.chkFanartDefaultViews.Name = "chkFanartDefaultViews";
          this.chkFanartDefaultViews.Size = new System.Drawing.Size(109, 30);
          this.chkFanartDefaultViews.TabIndex = 80;
          this.chkFanartDefaultViews.Text = "Fanart for default \r\ngroup views";
          this.ToolTip1.SetToolTip(this.chkFanartDefaultViews, resources.GetString("chkFanartDefaultViews.ToolTip"));
          this.chkFanartDefaultViews.UseVisualStyleBackColor = true;
          // 
          // chkDfltFanartImageAll
          // 
          this.chkDfltFanartImageAll.AutoSize = true;
          this.chkDfltFanartImageAll.Location = new System.Drawing.Point(150, 66);
          this.chkDfltFanartImageAll.Name = "chkDfltFanartImageAll";
          this.chkDfltFanartImageAll.Size = new System.Drawing.Size(67, 30);
          this.chkDfltFanartImageAll.TabIndex = 79;
          this.chkDfltFanartImageAll.Text = "Use for \r\nall Views";
          this.ToolTip1.SetToolTip(this.chkDfltFanartImageAll, "Use default fanart image for all group views like persons, date added, etc.");
          this.chkDfltFanartImageAll.UseVisualStyleBackColor = true;
          // 
          // label48
          // 
          this.label48.AutoSize = true;
          this.label48.Location = new System.Drawing.Point(242, 72);
          this.label48.Name = "label48";
          this.label48.Size = new System.Drawing.Size(69, 26);
          this.label48.TabIndex = 78;
          this.label48.Text = "Default \r\nFanart Image";
          // 
          // chkDfltFanartImage
          // 
          this.chkDfltFanartImage.AutoSize = true;
          this.chkDfltFanartImage.Location = new System.Drawing.Point(37, 66);
          this.chkDfltFanartImage.Name = "chkDfltFanartImage";
          this.chkDfltFanartImage.Size = new System.Drawing.Size(114, 30);
          this.chkDfltFanartImage.TabIndex = 77;
          this.chkDfltFanartImage.Text = "Use default image \r\nfor missing fanart";
          this.ToolTip1.SetToolTip(this.chkDfltFanartImage, "Use default fanart image if no fanart found for movies and group views like year," +
                  " genre, country.");
          this.chkDfltFanartImage.UseVisualStyleBackColor = true;
          this.chkDfltFanartImage.CheckedChanged += new System.EventHandler(this.chkDfltFanartImage_CheckedChanged);
          // 
          // ButDefFanart
          // 
          this.ButDefFanart.AllowDrop = true;
          this.ButDefFanart.Location = new System.Drawing.Point(625, 76);
          this.ButDefFanart.Name = "ButDefFanart";
          this.ButDefFanart.Size = new System.Drawing.Size(32, 20);
          this.ButDefFanart.TabIndex = 76;
          this.ButDefFanart.Text = "...";
          this.ButDefFanart.UseVisualStyleBackColor = true;
          this.ButDefFanart.Click += new System.EventHandler(this.ButDefFanart_Click);
          // 
          // DefaultFanartImage
          // 
          this.DefaultFanartImage.Location = new System.Drawing.Point(315, 76);
          this.DefaultFanartImage.Name = "DefaultFanartImage";
          this.DefaultFanartImage.Size = new System.Drawing.Size(304, 20);
          this.DefaultFanartImage.TabIndex = 75;
          // 
          // lblResultingGroupViewsPathFanart
          // 
          this.lblResultingGroupViewsPathFanart.AutoSize = true;
          this.lblResultingGroupViewsPathFanart.ForeColor = System.Drawing.SystemColors.ControlDark;
          this.lblResultingGroupViewsPathFanart.Location = new System.Drawing.Point(312, 48);
          this.lblResultingGroupViewsPathFanart.Name = "lblResultingGroupViewsPathFanart";
          this.lblResultingGroupViewsPathFanart.Size = new System.Drawing.Size(137, 13);
          this.lblResultingGroupViewsPathFanart.TabIndex = 74;
          this.lblResultingGroupViewsPathFanart.Text = "<Group Views Fanart Path>";
          this.ToolTip1.SetToolTip(this.lblResultingGroupViewsPathFanart, "To use Fanart for \"Views\" put fanart in the subfolder for year, category, country" +
                  ", \r\ne.g. <fanartfolder>\\\\_group\\\\year\\2010\";");
          // 
          // label40
          // 
          this.label40.AutoSize = true;
          this.label40.ForeColor = System.Drawing.SystemColors.ControlDark;
          this.label40.Location = new System.Drawing.Point(242, 48);
          this.label40.Name = "label40";
          this.label40.Size = new System.Drawing.Size(32, 13);
          this.label40.TabIndex = 73;
          this.label40.Text = "Path:";
          // 
          // chkFanart
          // 
          this.chkFanart.AutoSize = true;
          this.chkFanart.Location = new System.Drawing.Point(18, 18);
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
          this.btnFanart.Location = new System.Drawing.Point(625, 13);
          this.btnFanart.Name = "btnFanart";
          this.btnFanart.Size = new System.Drawing.Size(32, 20);
          this.btnFanart.TabIndex = 70;
          this.btnFanart.Text = "...";
          this.btnFanart.UseVisualStyleBackColor = true;
          this.btnFanart.Click += new System.EventHandler(this.btnFanart_Click);
          // 
          // MesFilmsFanart
          // 
          this.MesFilmsFanart.Enabled = false;
          this.MesFilmsFanart.Location = new System.Drawing.Point(315, 13);
          this.MesFilmsFanart.Name = "MesFilmsFanart";
          this.MesFilmsFanart.Size = new System.Drawing.Size(304, 20);
          this.MesFilmsFanart.TabIndex = 68;
          // 
          // labelFanart
          // 
          this.labelFanart.AutoSize = true;
          this.labelFanart.Location = new System.Drawing.Point(242, 14);
          this.labelFanart.Name = "labelFanart";
          this.labelFanart.Size = new System.Drawing.Size(65, 26);
          this.labelFanart.TabIndex = 69;
          this.labelFanart.Text = "Fanart Path \r\n(Images)";
          // 
          // chkDfltFanart
          // 
          this.chkDfltFanart.AutoSize = true;
          this.chkDfltFanart.Enabled = false;
          this.chkDfltFanart.Location = new System.Drawing.Point(37, 95);
          this.chkDfltFanart.Name = "chkDfltFanart";
          this.chkDfltFanart.Size = new System.Drawing.Size(191, 17);
          this.chkDfltFanart.TabIndex = 67;
          this.chkDfltFanart.Text = "Use movie cover for missing Fanart";
          this.chkDfltFanart.UseVisualStyleBackColor = true;
          this.chkDfltFanart.CheckedChanged += new System.EventHandler(this.chkDfltFanart_CheckedChanged);
          // 
          // groupBox22
          // 
          this.groupBox22.Controls.Add(this.chkDfltViewsAll);
          this.groupBox22.Controls.Add(this.label47);
          this.groupBox22.Controls.Add(this.ButDefCovViews);
          this.groupBox22.Controls.Add(this.DefaultCoverViews);
          this.groupBox22.Controls.Add(this.btnResetThumbs);
          this.groupBox22.Controls.Add(this.chkViews);
          this.groupBox22.Controls.Add(this.btnViews);
          this.groupBox22.Controls.Add(this.chkDfltViews);
          this.groupBox22.Controls.Add(this.label29);
          this.groupBox22.Controls.Add(this.MesFilmsViews);
          this.groupBox22.Location = new System.Drawing.Point(6, 141);
          this.groupBox22.Name = "groupBox22";
          this.groupBox22.Size = new System.Drawing.Size(735, 87);
          this.groupBox22.TabIndex = 75;
          this.groupBox22.TabStop = false;
          this.groupBox22.Text = "Thumbs for Group Views (Genre, Year, Country, ...)";
          this.ToolTip1.SetToolTip(this.groupBox22, resources.GetString("groupBox22.ToolTip"));
          // 
          // chkDfltViewsAll
          // 
          this.chkDfltViewsAll.AutoSize = true;
          this.chkDfltViewsAll.Location = new System.Drawing.Point(57, 67);
          this.chkDfltViewsAll.Name = "chkDfltViewsAll";
          this.chkDfltViewsAll.Size = new System.Drawing.Size(104, 17);
          this.chkDfltViewsAll.TabIndex = 80;
          this.chkDfltViewsAll.Text = "Use for all Views";
          this.ToolTip1.SetToolTip(this.chkDfltViewsAll, "If unchecked, group thumbs will only be used for genre, year and country.\r\nIf che" +
                  "cked, it will be used for all group views except \r\nthe person views (actor, dire" +
                  "ctor, producer, borrower).");
          this.chkDfltViewsAll.UseVisualStyleBackColor = true;
          // 
          // label47
          // 
          this.label47.AutoSize = true;
          this.label47.Location = new System.Drawing.Point(184, 44);
          this.label47.Name = "label47";
          this.label47.Size = new System.Drawing.Size(105, 13);
          this.label47.TabIndex = 79;
          this.label47.Text = "Default Group Image";
          // 
          // ButDefCovViews
          // 
          this.ButDefCovViews.Location = new System.Drawing.Point(625, 41);
          this.ButDefCovViews.Name = "ButDefCovViews";
          this.ButDefCovViews.Size = new System.Drawing.Size(32, 20);
          this.ButDefCovViews.TabIndex = 78;
          this.ButDefCovViews.Text = "...";
          this.ButDefCovViews.UseVisualStyleBackColor = true;
          this.ButDefCovViews.Click += new System.EventHandler(this.ButDefCovViews_Click);
          // 
          // DefaultCoverViews
          // 
          this.DefaultCoverViews.Location = new System.Drawing.Point(315, 41);
          this.DefaultCoverViews.Name = "DefaultCoverViews";
          this.DefaultCoverViews.Size = new System.Drawing.Size(304, 20);
          this.DefaultCoverViews.TabIndex = 77;
          // 
          // btnResetThumbs
          // 
          this.btnResetThumbs.Location = new System.Drawing.Point(665, 11);
          this.btnResetThumbs.Name = "btnResetThumbs";
          this.btnResetThumbs.Size = new System.Drawing.Size(64, 36);
          this.btnResetThumbs.TabIndex = 76;
          this.btnResetThumbs.Text = "Reset Thumbs";
          this.ToolTip1.SetToolTip(this.btnResetThumbs, resources.GetString("btnResetThumbs.ToolTip"));
          this.btnResetThumbs.UseVisualStyleBackColor = true;
          this.btnResetThumbs.Click += new System.EventHandler(this.btnResetThumbs_Click);
          // 
          // chkViews
          // 
          this.chkViews.AutoSize = true;
          this.chkViews.Location = new System.Drawing.Point(18, 15);
          this.chkViews.Name = "chkViews";
          this.chkViews.Size = new System.Drawing.Size(155, 17);
          this.chkViews.TabIndex = 71;
          this.chkViews.Text = "use thumbs for group views";
          this.chkViews.UseVisualStyleBackColor = true;
          // 
          // btnViews
          // 
          this.btnViews.Location = new System.Drawing.Point(625, 14);
          this.btnViews.Name = "btnViews";
          this.btnViews.Size = new System.Drawing.Size(32, 20);
          this.btnViews.TabIndex = 74;
          this.btnViews.Text = "...";
          this.btnViews.UseVisualStyleBackColor = true;
          this.btnViews.Click += new System.EventHandler(this.btnViews_Click);
          // 
          // chkDfltViews
          // 
          this.chkDfltViews.AutoSize = true;
          this.chkDfltViews.Location = new System.Drawing.Point(37, 35);
          this.chkDfltViews.Name = "chkDfltViews";
          this.chkDfltViews.Size = new System.Drawing.Size(112, 30);
          this.chkDfltViews.TabIndex = 75;
          this.chkDfltViews.Text = "Use default image\r\nfor missing thumbs";
          this.chkDfltViews.UseVisualStyleBackColor = true;
          // 
          // label29
          // 
          this.label29.AutoSize = true;
          this.label29.Location = new System.Drawing.Point(184, 18);
          this.label29.Name = "label29";
          this.label29.Size = new System.Drawing.Size(128, 13);
          this.label29.TabIndex = 73;
          this.label29.Text = "Group Views Picture Path";
          // 
          // MesFilmsViews
          // 
          this.MesFilmsViews.Location = new System.Drawing.Point(315, 14);
          this.MesFilmsViews.Name = "MesFilmsViews";
          this.MesFilmsViews.Size = new System.Drawing.Size(304, 20);
          this.MesFilmsViews.TabIndex = 72;
          this.ToolTip1.SetToolTip(this.MesFilmsViews, resources.GetString("MesFilmsViews.ToolTip"));
          // 
          // groupBox1
          // 
          this.groupBox1.Controls.Add(this.lblPicturePrefix);
          this.groupBox1.Controls.Add(this.txtPicturePrefix);
          this.groupBox1.Controls.Add(this.lblPictureHandling);
          this.groupBox1.Controls.Add(this.cbPictureHandling);
          this.groupBox1.Controls.Add(this.ButDefCov);
          this.groupBox1.Controls.Add(this.DefaultCover);
          this.groupBox1.Controls.Add(this.label_DefaulCover);
          this.groupBox1.Controls.Add(this.ButImg);
          this.groupBox1.Controls.Add(this.MesFilmsImg);
          this.groupBox1.Controls.Add(this.label2);
          this.groupBox1.Location = new System.Drawing.Point(6, 1);
          this.groupBox1.Name = "groupBox1";
          this.groupBox1.Size = new System.Drawing.Size(735, 67);
          this.groupBox1.TabIndex = 85;
          this.groupBox1.TabStop = false;
          this.groupBox1.Text = "Cover Images";
          this.ToolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
          // 
          // lblPicturePrefix
          // 
          this.lblPicturePrefix.AutoSize = true;
          this.lblPicturePrefix.Location = new System.Drawing.Point(508, 17);
          this.lblPicturePrefix.Name = "lblPicturePrefix";
          this.lblPicturePrefix.Size = new System.Drawing.Size(69, 13);
          this.lblPicturePrefix.TabIndex = 98;
          this.lblPicturePrefix.Text = "Picture Prefix";
          // 
          // txtPicturePrefix
          // 
          this.txtPicturePrefix.Location = new System.Drawing.Point(583, 14);
          this.txtPicturePrefix.Name = "txtPicturePrefix";
          this.txtPicturePrefix.Size = new System.Drawing.Size(146, 20);
          this.txtPicturePrefix.TabIndex = 97;
          this.ToolTip1.SetToolTip(this.txtPicturePrefix, resources.GetString("txtPicturePrefix.ToolTip"));
          // 
          // lblPictureHandling
          // 
          this.lblPictureHandling.AutoSize = true;
          this.lblPictureHandling.Location = new System.Drawing.Point(508, 43);
          this.lblPictureHandling.Name = "lblPictureHandling";
          this.lblPictureHandling.Size = new System.Drawing.Size(85, 13);
          this.lblPictureHandling.TabIndex = 96;
          this.lblPictureHandling.Text = "Picture Handling";
          // 
          // cbPictureHandling
          // 
          this.cbPictureHandling.FormattingEnabled = true;
          this.cbPictureHandling.Items.AddRange(new object[] {
            "Full Path",
            "Relative Path"});
          this.cbPictureHandling.Location = new System.Drawing.Point(599, 41);
          this.cbPictureHandling.Name = "cbPictureHandling";
          this.cbPictureHandling.Size = new System.Drawing.Size(130, 21);
          this.cbPictureHandling.TabIndex = 95;
          this.ToolTip1.SetToolTip(this.cbPictureHandling, resources.GetString("cbPictureHandling.ToolTip"));
          // 
          // ButDefCov
          // 
          this.ButDefCov.Location = new System.Drawing.Point(455, 39);
          this.ButDefCov.Name = "ButDefCov";
          this.ButDefCov.Size = new System.Drawing.Size(32, 20);
          this.ButDefCov.TabIndex = 90;
          this.ButDefCov.Text = "...";
          this.ButDefCov.UseVisualStyleBackColor = true;
          this.ButDefCov.Click += new System.EventHandler(this.ButDefCov_Click);
          // 
          // DefaultCover
          // 
          this.DefaultCover.Location = new System.Drawing.Point(145, 39);
          this.DefaultCover.Name = "DefaultCover";
          this.DefaultCover.Size = new System.Drawing.Size(304, 20);
          this.DefaultCover.TabIndex = 89;
          this.ToolTip1.SetToolTip(this.DefaultCover, resources.GetString("DefaultCover.ToolTip"));
          // 
          // label_DefaulCover
          // 
          this.label_DefaulCover.AutoSize = true;
          this.label_DefaulCover.Location = new System.Drawing.Point(15, 43);
          this.label_DefaulCover.Name = "label_DefaulCover";
          this.label_DefaulCover.Size = new System.Drawing.Size(104, 13);
          this.label_DefaulCover.TabIndex = 88;
          this.label_DefaulCover.Text = "Default Movie Cover";
          // 
          // ButImg
          // 
          this.ButImg.Location = new System.Drawing.Point(455, 13);
          this.ButImg.Name = "ButImg";
          this.ButImg.Size = new System.Drawing.Size(32, 20);
          this.ButImg.TabIndex = 87;
          this.ButImg.Text = "...";
          this.ButImg.UseVisualStyleBackColor = true;
          this.ButImg.Click += new System.EventHandler(this.ButImg_Click);
          // 
          // MesFilmsImg
          // 
          this.MesFilmsImg.Location = new System.Drawing.Point(145, 13);
          this.MesFilmsImg.Name = "MesFilmsImg";
          this.MesFilmsImg.Size = new System.Drawing.Size(304, 20);
          this.MesFilmsImg.TabIndex = 85;
          this.ToolTip1.SetToolTip(this.MesFilmsImg, resources.GetString("MesFilmsImg.ToolTip"));
          // 
          // label2
          // 
          this.label2.AutoSize = true;
          this.label2.Location = new System.Drawing.Point(15, 16);
          this.label2.Name = "label2";
          this.label2.Size = new System.Drawing.Size(97, 13);
          this.label2.TabIndex = 86;
          this.label2.Text = "Cover Images Path";
          // 
          // groupBox_ArtistImages
          // 
          this.groupBox_ArtistImages.Controls.Add(this.chkPersons);
          this.groupBox_ArtistImages.Controls.Add(this.btnResetThumbsArtist);
          this.groupBox_ArtistImages.Controls.Add(this.chkDfltArtist);
          this.groupBox_ArtistImages.Controls.Add(this.ButDefCovArtist);
          this.groupBox_ArtistImages.Controls.Add(this.ButImgArtist);
          this.groupBox_ArtistImages.Controls.Add(this.MesFilmsImgArtist);
          this.groupBox_ArtistImages.Controls.Add(this.label_DefaultArtistImage);
          this.groupBox_ArtistImages.Controls.Add(this.label_ArtistImagePath);
          this.groupBox_ArtistImages.Controls.Add(this.DefaultCoverArtist);
          this.groupBox_ArtistImages.Location = new System.Drawing.Point(6, 68);
          this.groupBox_ArtistImages.Name = "groupBox_ArtistImages";
          this.groupBox_ArtistImages.Size = new System.Drawing.Size(735, 69);
          this.groupBox_ArtistImages.TabIndex = 86;
          this.groupBox_ArtistImages.TabStop = false;
          this.groupBox_ArtistImages.Text = "Thumbs for Person Views (actors, directors, producer)";
          this.ToolTip1.SetToolTip(this.groupBox_ArtistImages, "Use this area to define a path where artist thumbs (images for persons) should be" +
                  " stored.\r\nWorks same way as cover images for movies.");
          // 
          // chkPersons
          // 
          this.chkPersons.AutoSize = true;
          this.chkPersons.Location = new System.Drawing.Point(18, 15);
          this.chkPersons.Name = "chkPersons";
          this.chkPersons.Size = new System.Drawing.Size(160, 17);
          this.chkPersons.TabIndex = 100;
          this.chkPersons.Text = "use thumbs for person views";
          this.chkPersons.UseVisualStyleBackColor = true;
          // 
          // btnResetThumbsArtist
          // 
          this.btnResetThumbsArtist.Location = new System.Drawing.Point(665, 12);
          this.btnResetThumbsArtist.Name = "btnResetThumbsArtist";
          this.btnResetThumbsArtist.Size = new System.Drawing.Size(64, 34);
          this.btnResetThumbsArtist.TabIndex = 99;
          this.btnResetThumbsArtist.Text = "Reset Thumbs";
          this.ToolTip1.SetToolTip(this.btnResetThumbsArtist, resources.GetString("btnResetThumbsArtist.ToolTip"));
          this.btnResetThumbsArtist.UseVisualStyleBackColor = true;
          this.btnResetThumbsArtist.Click += new System.EventHandler(this.btnResetThumbsArtist_Click);
          // 
          // chkDfltArtist
          // 
          this.chkDfltArtist.AutoSize = true;
          this.chkDfltArtist.Location = new System.Drawing.Point(37, 32);
          this.chkDfltArtist.Name = "chkDfltArtist";
          this.chkDfltArtist.Size = new System.Drawing.Size(114, 30);
          this.chkDfltArtist.TabIndex = 98;
          this.chkDfltArtist.Text = "Use default image \r\nfor missing thumbs";
          this.chkDfltArtist.UseVisualStyleBackColor = true;
          // 
          // ButDefCovArtist
          // 
          this.ButDefCovArtist.Location = new System.Drawing.Point(625, 40);
          this.ButDefCovArtist.Name = "ButDefCovArtist";
          this.ButDefCovArtist.Size = new System.Drawing.Size(32, 20);
          this.ButDefCovArtist.TabIndex = 97;
          this.ButDefCovArtist.Text = "...";
          this.ButDefCovArtist.UseVisualStyleBackColor = true;
          this.ButDefCovArtist.Click += new System.EventHandler(this.ButDefCovArtist_Click);
          // 
          // ButImgArtist
          // 
          this.ButImgArtist.Location = new System.Drawing.Point(625, 15);
          this.ButImgArtist.Name = "ButImgArtist";
          this.ButImgArtist.Size = new System.Drawing.Size(32, 20);
          this.ButImgArtist.TabIndex = 96;
          this.ButImgArtist.Text = "...";
          this.ButImgArtist.UseVisualStyleBackColor = true;
          this.ButImgArtist.Click += new System.EventHandler(this.ButImgArtist_Click);
          // 
          // MesFilmsImgArtist
          // 
          this.MesFilmsImgArtist.Location = new System.Drawing.Point(315, 14);
          this.MesFilmsImgArtist.Name = "MesFilmsImgArtist";
          this.MesFilmsImgArtist.Size = new System.Drawing.Size(304, 20);
          this.MesFilmsImgArtist.TabIndex = 95;
          this.ToolTip1.SetToolTip(this.MesFilmsImgArtist, resources.GetString("MesFilmsImgArtist.ToolTip"));
          // 
          // label_DefaultArtistImage
          // 
          this.label_DefaultArtistImage.AutoSize = true;
          this.label_DefaultArtistImage.Location = new System.Drawing.Point(184, 43);
          this.label_DefaultArtistImage.Name = "label_DefaultArtistImage";
          this.label_DefaultArtistImage.Size = new System.Drawing.Size(109, 13);
          this.label_DefaultArtistImage.TabIndex = 94;
          this.label_DefaultArtistImage.Text = "Default Person Image";
          // 
          // label_ArtistImagePath
          // 
          this.label_ArtistImagePath.AutoSize = true;
          this.label_ArtistImagePath.Location = new System.Drawing.Point(184, 18);
          this.label_ArtistImagePath.Name = "label_ArtistImagePath";
          this.label_ArtistImagePath.Size = new System.Drawing.Size(102, 13);
          this.label_ArtistImagePath.TabIndex = 93;
          this.label_ArtistImagePath.Text = "Person Images Path";
          // 
          // DefaultCoverArtist
          // 
          this.DefaultCoverArtist.Location = new System.Drawing.Point(315, 40);
          this.DefaultCoverArtist.Name = "DefaultCoverArtist";
          this.DefaultCoverArtist.Size = new System.Drawing.Size(304, 20);
          this.DefaultCoverArtist.TabIndex = 92;
          this.ToolTip1.SetToolTip(this.DefaultCoverArtist, resources.GetString("DefaultCoverArtist.ToolTip"));
          // 
          // groupBox_GrabberOptions
          // 
          this.groupBox_GrabberOptions.Controls.Add(this.txtGrabberDisplay);
          this.groupBox_GrabberOptions.Controls.Add(this.btnEditScript);
          this.groupBox_GrabberOptions.Controls.Add(this.groupBox6);
          this.groupBox_GrabberOptions.Controls.Add(this.lblSearchGrabberName);
          this.groupBox_GrabberOptions.Controls.Add(this.ItemSearchGrabberName);
          this.groupBox_GrabberOptions.Controls.Add(this.chkGrabber_ChooseScript);
          this.groupBox_GrabberOptions.Controls.Add(this.chkGrabber_Always);
          this.groupBox_GrabberOptions.Controls.Add(this.btnGrabber);
          this.groupBox_GrabberOptions.Controls.Add(this.txtGrabber);
          this.groupBox_GrabberOptions.Controls.Add(this.label27);
          this.groupBox_GrabberOptions.Location = new System.Drawing.Point(6, 19);
          this.groupBox_GrabberOptions.Name = "groupBox_GrabberOptions";
          this.groupBox_GrabberOptions.Size = new System.Drawing.Size(735, 123);
          this.groupBox_GrabberOptions.TabIndex = 1;
          this.groupBox_GrabberOptions.TabStop = false;
          this.groupBox_GrabberOptions.Text = "MyFilms Internal Internet Grabbing";
          this.ToolTip1.SetToolTip(this.groupBox_GrabberOptions, resources.GetString("groupBox_GrabberOptions.ToolTip"));
          // 
          // txtGrabberDisplay
          // 
          this.txtGrabberDisplay.Location = new System.Drawing.Point(145, 22);
          this.txtGrabberDisplay.Name = "txtGrabberDisplay";
          this.txtGrabberDisplay.ReadOnly = true;
          this.txtGrabberDisplay.Size = new System.Drawing.Size(100, 20);
          this.txtGrabberDisplay.TabIndex = 67;
          this.ToolTip1.SetToolTip(this.txtGrabberDisplay, resources.GetString("txtGrabberDisplay.ToolTip"));
          // 
          // btnEditScript
          // 
          this.btnEditScript.Location = new System.Drawing.Point(289, 22);
          this.btnEditScript.Name = "btnEditScript";
          this.btnEditScript.Size = new System.Drawing.Size(95, 20);
          this.btnEditScript.TabIndex = 66;
          this.btnEditScript.Text = "Grabber Options";
          this.ToolTip1.SetToolTip(this.btnEditScript, resources.GetString("btnEditScript.ToolTip"));
          this.btnEditScript.UseVisualStyleBackColor = true;
          this.btnEditScript.Click += new System.EventHandler(this.btnEditScript_Click);
          // 
          // groupBox6
          // 
          this.groupBox6.Controls.Add(this.cbGrabberOverrideGetRoles);
          this.groupBox6.Controls.Add(this.label55);
          this.groupBox6.Controls.Add(this.label51);
          this.groupBox6.Controls.Add(this.cbGrabberOverrideTitleLimit);
          this.groupBox6.Controls.Add(this.label50);
          this.groupBox6.Controls.Add(this.label49);
          this.groupBox6.Controls.Add(this.cbGrabberOverridePersonLimit);
          this.groupBox6.Controls.Add(this.cbGrabberOverrideLanguage);
          this.groupBox6.Location = new System.Drawing.Point(398, 15);
          this.groupBox6.Name = "groupBox6";
          this.groupBox6.Size = new System.Drawing.Size(330, 96);
          this.groupBox6.TabIndex = 65;
          this.groupBox6.TabStop = false;
          this.groupBox6.Text = "MyFilms Grabber Override Options ...";
          this.ToolTip1.SetToolTip(this.groupBox6, resources.GetString("groupBox6.ToolTip"));
          // 
          // cbGrabberOverrideGetRoles
          // 
          this.cbGrabberOverrideGetRoles.FormattingEnabled = true;
          this.cbGrabberOverrideGetRoles.Items.AddRange(new object[] {
            "true",
            "false"});
          this.cbGrabberOverrideGetRoles.Location = new System.Drawing.Point(276, 13);
          this.cbGrabberOverrideGetRoles.Name = "cbGrabberOverrideGetRoles";
          this.cbGrabberOverrideGetRoles.Size = new System.Drawing.Size(45, 21);
          this.cbGrabberOverrideGetRoles.TabIndex = 71;
          this.ToolTip1.SetToolTip(this.cbGrabberOverrideGetRoles, "If checked, add Roles to actor infos.\r\nDepends on the grabber script supporting t" +
                  "hat option.");
          // 
          // label55
          // 
          this.label55.AutoSize = true;
          this.label55.Location = new System.Drawing.Point(191, 19);
          this.label55.Name = "label55";
          this.label55.Size = new System.Drawing.Size(82, 13);
          this.label55.TabIndex = 70;
          this.label55.Text = "Get Actor Roles";
          // 
          // label51
          // 
          this.label51.AutoSize = true;
          this.label51.Location = new System.Drawing.Point(191, 71);
          this.label51.Name = "label51";
          this.label51.Size = new System.Drawing.Size(66, 13);
          this.label51.TabIndex = 68;
          this.label51.Text = "Limit # Titles";
          // 
          // cbGrabberOverrideTitleLimit
          // 
          this.cbGrabberOverrideTitleLimit.FormattingEnabled = true;
          this.cbGrabberOverrideTitleLimit.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "10",
            "15",
            "20",
            "999"});
          this.cbGrabberOverrideTitleLimit.Location = new System.Drawing.Point(276, 68);
          this.cbGrabberOverrideTitleLimit.Name = "cbGrabberOverrideTitleLimit";
          this.cbGrabberOverrideTitleLimit.Size = new System.Drawing.Size(45, 21);
          this.cbGrabberOverrideTitleLimit.TabIndex = 67;
          this.ToolTip1.SetToolTip(this.cbGrabberOverrideTitleLimit, "Limits the number of translated titles grabbed.\r\nYou may also set a preferred lan" +
                  "guage/country \r\nfor grabbing in the override options.");
          // 
          // label50
          // 
          this.label50.AutoSize = true;
          this.label50.Location = new System.Drawing.Point(12, 20);
          this.label50.Name = "label50";
          this.label50.Size = new System.Drawing.Size(161, 13);
          this.label50.TabIndex = 66;
          this.label50.Text = "Preferred Languages / Countries";
          // 
          // label49
          // 
          this.label49.AutoSize = true;
          this.label49.Location = new System.Drawing.Point(191, 44);
          this.label49.Name = "label49";
          this.label49.Size = new System.Drawing.Size(79, 13);
          this.label49.TabIndex = 65;
          this.label49.Text = "Limit # Persons";
          // 
          // cbGrabberOverridePersonLimit
          // 
          this.cbGrabberOverridePersonLimit.FormattingEnabled = true;
          this.cbGrabberOverridePersonLimit.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "10",
            "15",
            "20",
            "999"});
          this.cbGrabberOverridePersonLimit.Location = new System.Drawing.Point(276, 40);
          this.cbGrabberOverridePersonLimit.Name = "cbGrabberOverridePersonLimit";
          this.cbGrabberOverridePersonLimit.Size = new System.Drawing.Size(45, 21);
          this.cbGrabberOverridePersonLimit.TabIndex = 63;
          this.ToolTip1.SetToolTip(this.cbGrabberOverridePersonLimit, "Limits the number of person names grabbed.\r\nThis settings applies to all fields t" +
                  "hat grab person names, \r\nlike actors, producers, writers, directors.");
          // 
          // cbGrabberOverrideLanguage
          // 
          this.cbGrabberOverrideLanguage.FormattingEnabled = true;
          this.cbGrabberOverrideLanguage.Items.AddRange(new object[] {
            "Argentina",
            "Australia",
            "Austria",
            "Belgium",
            "Brazil",
            "Canada",
            "Chile",
            "Croatia",
            "Czech Republic",
            "Denmark",
            "Estonia",
            "Finland",
            "France",
            "Germany",
            "Greece",
            "Hong Kong",
            "Hungary",
            "Iceland",
            "India",
            "Ireland",
            "Israel",
            "Italy",
            "Japan",
            "Malaysia",
            "Mexico",
            "Netherlands",
            "New Zealand",
            "Norway",
            "Peru",
            "Philippines",
            "Poland",
            "Portugal",
            "Romania",
            "Russia",
            "Singapore",
            "Slovakia",
            "Slovenia",
            "South Africa",
            "South Korea",
            "Spain",
            "Sweden",
            "Switzerland",
            "Turkey",
            "UK",
            "Uruguay",
            "USA"});
          this.cbGrabberOverrideLanguage.Location = new System.Drawing.Point(11, 41);
          this.cbGrabberOverrideLanguage.Name = "cbGrabberOverrideLanguage";
          this.cbGrabberOverrideLanguage.Size = new System.Drawing.Size(167, 21);
          this.cbGrabberOverrideLanguage.TabIndex = 64;
          this.ToolTip1.SetToolTip(this.cbGrabberOverrideLanguage, resources.GetString("cbGrabberOverrideLanguage.ToolTip"));
          // 
          // lblSearchGrabberName
          // 
          this.lblSearchGrabberName.AutoSize = true;
          this.lblSearchGrabberName.Location = new System.Drawing.Point(16, 50);
          this.lblSearchGrabberName.Name = "lblSearchGrabberName";
          this.lblSearchGrabberName.Size = new System.Drawing.Size(124, 13);
          this.lblSearchGrabberName.TabIndex = 62;
          this.lblSearchGrabberName.Text = "Title for Internet Updates";
          // 
          // ItemSearchGrabberName
          // 
          this.ItemSearchGrabberName.FormattingEnabled = true;
          this.ItemSearchGrabberName.Items.AddRange(new object[] {
            "OriginalTitle",
            "TranslatedTitle",
            "FormattedTitle"});
          this.ItemSearchGrabberName.Location = new System.Drawing.Point(145, 47);
          this.ItemSearchGrabberName.Name = "ItemSearchGrabberName";
          this.ItemSearchGrabberName.Size = new System.Drawing.Size(138, 21);
          this.ItemSearchGrabberName.TabIndex = 61;
          this.ToolTip1.SetToolTip(this.ItemSearchGrabberName, resources.GetString("ItemSearchGrabberName.ToolTip"));
          // 
          // chkGrabber_ChooseScript
          // 
          this.chkGrabber_ChooseScript.AutoSize = true;
          this.chkGrabber_ChooseScript.Location = new System.Drawing.Point(145, 74);
          this.chkGrabber_ChooseScript.Name = "chkGrabber_ChooseScript";
          this.chkGrabber_ChooseScript.Size = new System.Drawing.Size(193, 17);
          this.chkGrabber_ChooseScript.TabIndex = 60;
          this.chkGrabber_ChooseScript.Text = "don\'t use default script (always ask)";
          this.ToolTip1.SetToolTip(this.chkGrabber_ChooseScript, resources.GetString("chkGrabber_ChooseScript.ToolTip"));
          this.chkGrabber_ChooseScript.UseVisualStyleBackColor = true;
          // 
          // chkGrabber_Always
          // 
          this.chkGrabber_Always.AutoSize = true;
          this.chkGrabber_Always.Location = new System.Drawing.Point(145, 97);
          this.chkGrabber_Always.Name = "chkGrabber_Always";
          this.chkGrabber_Always.Size = new System.Drawing.Size(188, 17);
          this.chkGrabber_Always.TabIndex = 56;
          this.chkGrabber_Always.Text = "try to find best match automatically";
          this.ToolTip1.SetToolTip(this.chkGrabber_Always, resources.GetString("chkGrabber_Always.ToolTip"));
          this.chkGrabber_Always.UseVisualStyleBackColor = true;
          // 
          // btnGrabber
          // 
          this.btnGrabber.Location = new System.Drawing.Point(251, 22);
          this.btnGrabber.Name = "btnGrabber";
          this.btnGrabber.Size = new System.Drawing.Size(32, 20);
          this.btnGrabber.TabIndex = 55;
          this.btnGrabber.Text = "...";
          this.ToolTip1.SetToolTip(this.btnGrabber, "Browse and choose the script you want MyFilms \r\nto use as default.");
          this.btnGrabber.UseVisualStyleBackColor = true;
          this.btnGrabber.Click += new System.EventHandler(this.btnGrabber_Click);
          // 
          // txtGrabber
          // 
          this.txtGrabber.Location = new System.Drawing.Point(145, 22);
          this.txtGrabber.Name = "txtGrabber";
          this.txtGrabber.Size = new System.Drawing.Size(68, 20);
          this.txtGrabber.TabIndex = 54;
          this.ToolTip1.SetToolTip(this.txtGrabber, resources.GetString("txtGrabber.ToolTip"));
          this.txtGrabber.Visible = false;
          this.txtGrabber.TextChanged += new System.EventHandler(this.txtGrabber_TextChanged);
          // 
          // label27
          // 
          this.label27.AutoSize = true;
          this.label27.Location = new System.Drawing.Point(16, 25);
          this.label27.Name = "label27";
          this.label27.Size = new System.Drawing.Size(112, 13);
          this.label27.TabIndex = 53;
          this.label27.Text = "Default Grabber Script";
          // 
          // groupBox_AMCupdater
          // 
          this.groupBox_AMCupdater.BackColor = System.Drawing.Color.Transparent;
          this.groupBox_AMCupdater.Controls.Add(this.groupBox_AMCupdaterScheduer);
          this.groupBox_AMCupdater.Controls.Add(this.groupBox_AMCupdater_ExternalApplication);
          this.groupBox_AMCupdater.Controls.Add(this.chkAMCUpd);
          this.groupBox_AMCupdater.Location = new System.Drawing.Point(6, 148);
          this.groupBox_AMCupdater.Name = "groupBox_AMCupdater";
          this.groupBox_AMCupdater.Size = new System.Drawing.Size(735, 200);
          this.groupBox_AMCupdater.TabIndex = 2;
          this.groupBox_AMCupdater.TabStop = false;
          this.groupBox_AMCupdater.Text = "AMCUpdater Options (for global internet update functions)";
          this.ToolTip1.SetToolTip(this.groupBox_AMCupdater, resources.GetString("groupBox_AMCupdater.ToolTip"));
          // 
          // groupBox_AMCupdaterScheduer
          // 
          this.groupBox_AMCupdaterScheduer.Controls.Add(this.txtAMCUpd_cnf_Display);
          this.groupBox_AMCupdaterScheduer.Controls.Add(this.scheduleAMCUpdater);
          this.groupBox_AMCupdaterScheduer.Controls.Add(this.label28);
          this.groupBox_AMCupdaterScheduer.Controls.Add(this.btnParameters);
          this.groupBox_AMCupdaterScheduer.Controls.Add(this.txtAMCUpd_cnf);
          this.groupBox_AMCupdaterScheduer.Controls.Add(this.btnAMCUpd_cnf);
          this.groupBox_AMCupdaterScheduer.Location = new System.Drawing.Point(302, 13);
          this.groupBox_AMCupdaterScheduer.Name = "groupBox_AMCupdaterScheduer";
          this.groupBox_AMCupdaterScheduer.Size = new System.Drawing.Size(427, 74);
          this.groupBox_AMCupdaterScheduer.TabIndex = 62;
          this.groupBox_AMCupdaterScheduer.TabStop = false;
          this.groupBox_AMCupdaterScheduer.Text = "Scheduler for AMCupdater";
          // 
          // txtAMCUpd_cnf_Display
          // 
          this.txtAMCUpd_cnf_Display.Enabled = false;
          this.txtAMCUpd_cnf_Display.Location = new System.Drawing.Point(108, 45);
          this.txtAMCUpd_cnf_Display.Name = "txtAMCUpd_cnf_Display";
          this.txtAMCUpd_cnf_Display.Size = new System.Drawing.Size(272, 20);
          this.txtAMCUpd_cnf_Display.TabIndex = 62;
          // 
          // scheduleAMCUpdater
          // 
          this.scheduleAMCUpdater.AutoSize = true;
          this.scheduleAMCUpdater.Location = new System.Drawing.Point(107, 20);
          this.scheduleAMCUpdater.Name = "scheduleAMCUpdater";
          this.scheduleAMCUpdater.Size = new System.Drawing.Size(71, 17);
          this.scheduleAMCUpdater.TabIndex = 61;
          this.scheduleAMCUpdater.Text = "Schedule";
          this.ToolTip1.SetToolTip(this.scheduleAMCUpdater, resources.GetString("scheduleAMCUpdater.ToolTip"));
          this.scheduleAMCUpdater.UseVisualStyleBackColor = true;
          this.scheduleAMCUpdater.CheckedChanged += new System.EventHandler(this.scheduleAMCUpdater_CheckedChanged);
          // 
          // label28
          // 
          this.label28.AutoSize = true;
          this.label28.Location = new System.Drawing.Point(11, 48);
          this.label28.Name = "label28";
          this.label28.Size = new System.Drawing.Size(90, 13);
          this.label28.TabIndex = 56;
          this.label28.Text = "AMCU Config File";
          // 
          // btnParameters
          // 
          this.btnParameters.Location = new System.Drawing.Point(275, 15);
          this.btnParameters.Name = "btnParameters";
          this.btnParameters.Size = new System.Drawing.Size(141, 24);
          this.btnParameters.TabIndex = 60;
          this.btnParameters.Text = "Task Parameters";
          this.btnParameters.UseVisualStyleBackColor = true;
          this.btnParameters.Click += new System.EventHandler(this.btnParameters_Click);
          // 
          // txtAMCUpd_cnf
          // 
          this.txtAMCUpd_cnf.Location = new System.Drawing.Point(107, 45);
          this.txtAMCUpd_cnf.Name = "txtAMCUpd_cnf";
          this.txtAMCUpd_cnf.Size = new System.Drawing.Size(168, 20);
          this.txtAMCUpd_cnf.TabIndex = 57;
          this.ToolTip1.SetToolTip(this.txtAMCUpd_cnf, resources.GetString("txtAMCUpd_cnf.ToolTip"));
          this.txtAMCUpd_cnf.Visible = false;
          this.txtAMCUpd_cnf.TextChanged += new System.EventHandler(this.txtAMCUpd_cnf_TextChanged);
          // 
          // btnAMCUpd_cnf
          // 
          this.btnAMCUpd_cnf.Location = new System.Drawing.Point(384, 45);
          this.btnAMCUpd_cnf.Name = "btnAMCUpd_cnf";
          this.btnAMCUpd_cnf.Size = new System.Drawing.Size(32, 20);
          this.btnAMCUpd_cnf.TabIndex = 58;
          this.btnAMCUpd_cnf.Text = "...";
          this.btnAMCUpd_cnf.UseVisualStyleBackColor = true;
          this.btnAMCUpd_cnf.Click += new System.EventHandler(this.btnAMCUpd_cnf_Click);
          // 
          // groupBox_AMCupdater_ExternalApplication
          // 
          this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.label56);
          this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.AmcTitleSearchHandling);
          this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.btnCreateAMCDefaultConfig);
          this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.lblAMCMovieScanPath);
          this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.chkAMC_Purge_Missing_Files);
          this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.btnAMCMovieScanPathAdd);
          this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.AMCMovieScanPath);
          this.groupBox_AMCupdater_ExternalApplication.Controls.Add(this.btnCreateAMCDesktopIcon);
          this.groupBox_AMCupdater_ExternalApplication.ImeMode = System.Windows.Forms.ImeMode.Off;
          this.groupBox_AMCupdater_ExternalApplication.Location = new System.Drawing.Point(6, 93);
          this.groupBox_AMCupdater_ExternalApplication.Name = "groupBox_AMCupdater_ExternalApplication";
          this.groupBox_AMCupdater_ExternalApplication.Size = new System.Drawing.Size(723, 100);
          this.groupBox_AMCupdater_ExternalApplication.TabIndex = 3;
          this.groupBox_AMCupdater_ExternalApplication.TabStop = false;
          this.groupBox_AMCupdater_ExternalApplication.Text = "AMC Updater external application";
          // 
          // label56
          // 
          this.label56.AutoSize = true;
          this.label56.Location = new System.Drawing.Point(10, 51);
          this.label56.Name = "label56";
          this.label56.Size = new System.Drawing.Size(112, 13);
          this.label56.TabIndex = 86;
          this.label56.Text = "Title & Search Handling";
          // 
          // AmcTitleSearchHandling
          // 
          this.AmcTitleSearchHandling.FormattingEnabled = true;
          this.AmcTitleSearchHandling.Items.AddRange(new object[] {
            "File Name",
            "Folder Name",
            "Relative Name",
            "File Name + Internet Lookup",
            "Folder Name + Internet Lookup",
            "Relative Name + Internet Lookup"});
          this.AmcTitleSearchHandling.Location = new System.Drawing.Point(139, 48);
          this.AmcTitleSearchHandling.Name = "AmcTitleSearchHandling";
          this.AmcTitleSearchHandling.Size = new System.Drawing.Size(200, 21);
          this.AmcTitleSearchHandling.TabIndex = 85;
          // 
          // btnCreateAMCDefaultConfig
          // 
          this.btnCreateAMCDefaultConfig.AllowDrop = true;
          this.btnCreateAMCDefaultConfig.Location = new System.Drawing.Point(571, 51);
          this.btnCreateAMCDefaultConfig.Name = "btnCreateAMCDefaultConfig";
          this.btnCreateAMCDefaultConfig.Size = new System.Drawing.Size(143, 36);
          this.btnCreateAMCDefaultConfig.TabIndex = 77;
          this.btnCreateAMCDefaultConfig.Text = "(re)Create AMC Updater \r\nDefault Settings";
          this.ToolTip1.SetToolTip(this.btnCreateAMCDefaultConfig, resources.GetString("btnCreateAMCDefaultConfig.ToolTip"));
          this.btnCreateAMCDefaultConfig.UseVisualStyleBackColor = true;
          this.btnCreateAMCDefaultConfig.Click += new System.EventHandler(this.btnCreateAMCDefaultConfig_Click);
          // 
          // lblAMCMovieScanPath
          // 
          this.lblAMCMovieScanPath.AutoSize = true;
          this.lblAMCMovieScanPath.Location = new System.Drawing.Point(10, 24);
          this.lblAMCMovieScanPath.Name = "lblAMCMovieScanPath";
          this.lblAMCMovieScanPath.Size = new System.Drawing.Size(100, 13);
          this.lblAMCMovieScanPath.TabIndex = 84;
          this.lblAMCMovieScanPath.Text = "Movie Scan Path(s)";
          // 
          // chkAMC_Purge_Missing_Files
          // 
          this.chkAMC_Purge_Missing_Files.AutoSize = true;
          this.chkAMC_Purge_Missing_Files.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
          this.chkAMC_Purge_Missing_Files.ForeColor = System.Drawing.Color.Red;
          this.chkAMC_Purge_Missing_Files.Location = new System.Drawing.Point(139, 75);
          this.chkAMC_Purge_Missing_Files.Name = "chkAMC_Purge_Missing_Files";
          this.chkAMC_Purge_Missing_Files.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
          this.chkAMC_Purge_Missing_Files.Size = new System.Drawing.Size(135, 17);
          this.chkAMC_Purge_Missing_Files.TabIndex = 83;
          this.chkAMC_Purge_Missing_Files.Text = "Purge Orphan Records";
          this.ToolTip1.SetToolTip(this.chkAMC_Purge_Missing_Files, resources.GetString("chkAMC_Purge_Missing_Files.ToolTip"));
          this.chkAMC_Purge_Missing_Files.UseVisualStyleBackColor = true;
          this.chkAMC_Purge_Missing_Files.CheckedChanged += new System.EventHandler(this.chkAMC_Purge_Missing_Files_CheckedChanged);
          // 
          // btnAMCMovieScanPathAdd
          // 
          this.btnAMCMovieScanPathAdd.Location = new System.Drawing.Point(682, 21);
          this.btnAMCMovieScanPathAdd.Name = "btnAMCMovieScanPathAdd";
          this.btnAMCMovieScanPathAdd.Size = new System.Drawing.Size(32, 20);
          this.btnAMCMovieScanPathAdd.TabIndex = 82;
          this.btnAMCMovieScanPathAdd.Text = "...";
          this.btnAMCMovieScanPathAdd.UseVisualStyleBackColor = true;
          this.btnAMCMovieScanPathAdd.Click += new System.EventHandler(this.btnAMCMovieScanPathAdd_Click);
          // 
          // AMCMovieScanPath
          // 
          this.AMCMovieScanPath.Location = new System.Drawing.Point(139, 21);
          this.AMCMovieScanPath.Name = "AMCMovieScanPath";
          this.AMCMovieScanPath.Size = new System.Drawing.Size(537, 20);
          this.AMCMovieScanPath.TabIndex = 81;
          this.ToolTip1.SetToolTip(this.AMCMovieScanPath, "Enter the path(s) to your media files.\r\nMore than one path, separated by semicolo" +
                  "n, are allowed.");
          // 
          // btnCreateAMCDesktopIcon
          // 
          this.btnCreateAMCDesktopIcon.Location = new System.Drawing.Point(414, 51);
          this.btnCreateAMCDesktopIcon.Name = "btnCreateAMCDesktopIcon";
          this.btnCreateAMCDesktopIcon.Size = new System.Drawing.Size(143, 36);
          this.btnCreateAMCDesktopIcon.TabIndex = 78;
          this.btnCreateAMCDesktopIcon.Text = "Create Desktop Icon ";
          this.ToolTip1.SetToolTip(this.btnCreateAMCDesktopIcon, "By creating a desktop icon you can launch AMC Updater directly \r\nfrom Windows wit" +
                  "hout first starting MyFilms setup.");
          this.btnCreateAMCDesktopIcon.UseVisualStyleBackColor = true;
          this.btnCreateAMCDesktopIcon.Click += new System.EventHandler(this.btnCreateAMCDesktopIcon_Click);
          // 
          // chkAMCUpd
          // 
          this.chkAMCUpd.AutoSize = true;
          this.chkAMCUpd.BackColor = System.Drawing.Color.Transparent;
          this.chkAMCUpd.Location = new System.Drawing.Point(19, 42);
          this.chkAMCUpd.Name = "chkAMCUpd";
          this.chkAMCUpd.Size = new System.Drawing.Size(123, 17);
          this.chkAMCUpd.TabIndex = 0;
          this.chkAMCUpd.Text = "Enable AMCUpdater";
          this.ToolTip1.SetToolTip(this.chkAMCUpd, resources.GetString("chkAMCUpd.ToolTip"));
          this.chkAMCUpd.UseVisualStyleBackColor = false;
          this.chkAMCUpd.CheckedChanged += new System.EventHandler(this.chkAMCUpd_CheckedChanged);
          // 
          // groupBoxDeletionOptions
          // 
          this.groupBoxDeletionOptions.Controls.Add(this.gpsuppress);
          this.groupBoxDeletionOptions.Controls.Add(this.chkSuppress);
          this.groupBoxDeletionOptions.Controls.Add(this.gpspfield);
          this.groupBoxDeletionOptions.Location = new System.Drawing.Point(378, 31);
          this.groupBoxDeletionOptions.Name = "groupBoxDeletionOptions";
          this.groupBoxDeletionOptions.Size = new System.Drawing.Size(350, 228);
          this.groupBoxDeletionOptions.TabIndex = 35;
          this.groupBoxDeletionOptions.TabStop = false;
          this.groupBoxDeletionOptions.Text = "Deletion Options for automatic updates and/or deletions ...";
          this.ToolTip1.SetToolTip(this.groupBoxDeletionOptions, resources.GetString("groupBoxDeletionOptions.ToolTip"));
          // 
          // chkSuppress
          // 
          this.chkSuppress.AutoSize = true;
          this.chkSuppress.Location = new System.Drawing.Point(15, 19);
          this.chkSuppress.Name = "chkSuppress";
          this.chkSuppress.Size = new System.Drawing.Size(294, 17);
          this.chkSuppress.TabIndex = 0;
          this.chkSuppress.Text = "Enable automatic deletion options after movie is watched";
          this.ToolTip1.SetToolTip(this.chkSuppress, "WHen enabled, the selected actions are automatically performed after a movie is w" +
                  "atched.");
          this.chkSuppress.UseVisualStyleBackColor = true;
          this.chkSuppress.CheckedChanged += new System.EventHandler(this.chkSuppress_CheckedChanged);
          // 
          // gpsuppress
          // 
          this.gpsuppress.Controls.Add(this.rbsuppress2);
          this.gpsuppress.Controls.Add(this.rbsuppress4);
          this.gpsuppress.Controls.Add(this.rbsuppress3);
          this.gpsuppress.Controls.Add(this.rbsuppress1);
          this.gpsuppress.Enabled = false;
          this.gpsuppress.Location = new System.Drawing.Point(15, 35);
          this.gpsuppress.Name = "gpsuppress";
          this.gpsuppress.Size = new System.Drawing.Size(321, 93);
          this.gpsuppress.TabIndex = 2;
          this.gpsuppress.TabStop = false;
          // 
          // rbsuppress2
          // 
          this.rbsuppress2.AutoSize = true;
          this.rbsuppress2.Location = new System.Drawing.Point(18, 30);
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
          this.rbsuppress4.Location = new System.Drawing.Point(18, 72);
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
          this.rbsuppress3.Location = new System.Drawing.Point(18, 51);
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
          this.rbsuppress1.Location = new System.Drawing.Point(18, 9);
          this.rbsuppress1.Name = "rbsuppress1";
          this.rbsuppress1.Size = new System.Drawing.Size(119, 17);
          this.rbsuppress1.TabIndex = 1;
          this.rbsuppress1.TabStop = true;
          this.rbsuppress1.Text = "Delete db entry only";
          this.rbsuppress1.UseVisualStyleBackColor = true;
          // 
          // gpspfield
          // 
          this.gpspfield.Controls.Add(this.label61);
          this.gpspfield.Controls.Add(this.chksupplaystop);
          this.gpspfield.Controls.Add(this.txtfdupdate);
          this.gpspfield.Controls.Add(this.lblUpdateValue);
          this.gpspfield.Controls.Add(this.cbfdupdate);
          this.gpspfield.Enabled = false;
          this.gpspfield.Location = new System.Drawing.Point(15, 134);
          this.gpspfield.Name = "gpspfield";
          this.gpspfield.Size = new System.Drawing.Size(321, 87);
          this.gpspfield.TabIndex = 18;
          this.gpspfield.TabStop = false;
          this.gpspfield.Text = "Player finished Update Action";
          this.ToolTip1.SetToolTip(this.gpspfield, resources.GetString("gpspfield.ToolTip"));
          // 
          // chksupplaystop
          // 
          this.chksupplaystop.AutoSize = true;
          this.chksupplaystop.Enabled = false;
          this.chksupplaystop.Location = new System.Drawing.Point(10, 19);
          this.chksupplaystop.Name = "chksupplaystop";
          this.chksupplaystop.Size = new System.Drawing.Size(184, 17);
          this.chksupplaystop.TabIndex = 19;
          this.chksupplaystop.Text = "Update Field when player finishes";
          this.ToolTip1.SetToolTip(this.chksupplaystop, "You can update a DB field with a specified value after a movie is watched.");
          this.chksupplaystop.UseVisualStyleBackColor = true;
          this.chksupplaystop.CheckedChanged += new System.EventHandler(this.chksupplaystop_CheckedChanged);
          // 
          // txtfdupdate
          // 
          this.txtfdupdate.Location = new System.Drawing.Point(170, 58);
          this.txtfdupdate.Name = "txtfdupdate";
          this.txtfdupdate.Size = new System.Drawing.Size(144, 20);
          this.txtfdupdate.TabIndex = 16;
          // 
          // lblUpdateValue
          // 
          this.lblUpdateValue.AutoSize = true;
          this.lblUpdateValue.Location = new System.Drawing.Point(9, 40);
          this.lblUpdateValue.Name = "lblUpdateValue";
          this.lblUpdateValue.Size = new System.Drawing.Size(93, 13);
          this.lblUpdateValue.TabIndex = 20;
          this.lblUpdateValue.Text = "DB Item to update";
          // 
          // cbfdupdate
          // 
          this.cbfdupdate.FormattingEnabled = true;
          this.cbfdupdate.Location = new System.Drawing.Point(10, 58);
          this.cbfdupdate.Name = "cbfdupdate";
          this.cbfdupdate.Size = new System.Drawing.Size(150, 21);
          this.cbfdupdate.TabIndex = 15;
          this.cbfdupdate.SelectedIndexChanged += new System.EventHandler(this.cbfdupdate_SelectedIndexChanged);
          // 
          // groupBoxUpdateByProperties
          // 
          this.groupBoxUpdateByProperties.Controls.Add(this.AntUpdFieldReset);
          this.groupBoxUpdateByProperties.Controls.Add(this.AntUpdFieldClear);
          this.groupBoxUpdateByProperties.Controls.Add(this.label33);
          this.groupBoxUpdateByProperties.Controls.Add(this.AntUpdField);
          this.groupBoxUpdateByProperties.Controls.Add(this.AntUpdList);
          this.groupBoxUpdateByProperties.Location = new System.Drawing.Point(17, 265);
          this.groupBoxUpdateByProperties.Name = "groupBoxUpdateByProperties";
          this.groupBoxUpdateByProperties.Size = new System.Drawing.Size(711, 83);
          this.groupBoxUpdateByProperties.TabIndex = 34;
          this.groupBoxUpdateByProperties.TabStop = false;
          this.groupBoxUpdateByProperties.Text = "Update by Properties";
          this.ToolTip1.SetToolTip(this.groupBoxUpdateByProperties, resources.GetString("groupBoxUpdateByProperties.ToolTip"));
          // 
          // AntUpdFieldReset
          // 
          this.AntUpdFieldReset.Location = new System.Drawing.Point(582, 17);
          this.AntUpdFieldReset.Name = "AntUpdFieldReset";
          this.AntUpdFieldReset.Size = new System.Drawing.Size(108, 22);
          this.AntUpdFieldReset.TabIndex = 86;
          this.AntUpdFieldReset.Text = "Reset to Default";
          this.ToolTip1.SetToolTip(this.AntUpdFieldReset, "This will reset the update llist to the MyFilms default values.");
          this.AntUpdFieldReset.UseVisualStyleBackColor = true;
          this.AntUpdFieldReset.Click += new System.EventHandler(this.AntUpdFieldReset_Click_1);
          // 
          // AntUpdFieldClear
          // 
          this.AntUpdFieldClear.Location = new System.Drawing.Point(488, 18);
          this.AntUpdFieldClear.Name = "AntUpdFieldClear";
          this.AntUpdFieldClear.Size = new System.Drawing.Size(64, 21);
          this.AntUpdFieldClear.TabIndex = 84;
          this.AntUpdFieldClear.Text = "Clear";
          this.ToolTip1.SetToolTip(this.AntUpdFieldClear, "This will clear the items from the update list.");
          this.AntUpdFieldClear.UseVisualStyleBackColor = true;
          this.AntUpdFieldClear.Click += new System.EventHandler(this.AntUpdFieldReset_Click);
          // 
          // label33
          // 
          this.label33.AutoSize = true;
          this.label33.Location = new System.Drawing.Point(12, 21);
          this.label33.Name = "label33";
          this.label33.Size = new System.Drawing.Size(153, 13);
          this.label33.TabIndex = 83;
          this.label33.Text = "Update by Properties Selection";
          // 
          // AntUpdField
          // 
          this.AntUpdField.FormattingEnabled = true;
          this.AntUpdField.Location = new System.Drawing.Point(197, 18);
          this.AntUpdField.Name = "AntUpdField";
          this.AntUpdField.Size = new System.Drawing.Size(151, 21);
          this.AntUpdField.TabIndex = 81;
          this.ToolTip1.SetToolTip(this.AntUpdField, "Choose a DB item to add to the property update list.");
          this.AntUpdField.SelectedIndexChanged += new System.EventHandler(this.AntUpdField_SelectedIndexChanged);
          // 
          // AntUpdList
          // 
          this.AntUpdList.BackColor = System.Drawing.SystemColors.Control;
          this.AntUpdList.Location = new System.Drawing.Point(15, 51);
          this.AntUpdList.Name = "AntUpdList";
          this.AntUpdList.Size = new System.Drawing.Size(675, 20);
          this.AntUpdList.TabIndex = 82;
          this.ToolTip1.SetToolTip(this.AntUpdList, "This field cannot be edited directly, but shows you the chosen DB items for updat" +
                  "es.\r\n");
          // 
          // groupBox2
          // 
          this.groupBox2.Controls.Add(this.btnWatchedImport);
          this.groupBox2.Controls.Add(this.btnWatchedExport);
          this.groupBox2.Controls.Add(this.lblUnwatchedItemsValue);
          this.groupBox2.Controls.Add(this.textBoxGlobalUnwatchedOnlyValue);
          this.groupBox2.Controls.Add(this.CheckWatchedPlayerStopped);
          this.groupBox2.Controls.Add(this.Label_UserProfileName);
          this.groupBox2.Controls.Add(this.UserProfileName);
          this.groupBox2.Controls.Add(this.label19);
          this.groupBox2.Controls.Add(this.cbWatched);
          this.groupBox2.Controls.Add(this.chkEnhancedWatchedStatusHandling);
          this.groupBox2.Controls.Add(this.CheckWatched);
          this.groupBox2.Location = new System.Drawing.Point(17, 116);
          this.groupBox2.Name = "groupBox2";
          this.groupBox2.Size = new System.Drawing.Size(354, 143);
          this.groupBox2.TabIndex = 72;
          this.groupBox2.TabStop = false;
          this.groupBox2.Text = "Watched-Status Handling";
          this.ToolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
          // 
          // btnWatchedImport
          // 
          this.btnWatchedImport.Location = new System.Drawing.Point(273, 33);
          this.btnWatchedImport.Name = "btnWatchedImport";
          this.btnWatchedImport.Size = new System.Drawing.Size(75, 21);
          this.btnWatchedImport.TabIndex = 86;
          this.btnWatchedImport.Text = "import";
          this.ToolTip1.SetToolTip(this.btnWatchedImport, resources.GetString("btnWatchedImport.ToolTip"));
          this.btnWatchedImport.UseVisualStyleBackColor = true;
          this.btnWatchedImport.Click += new System.EventHandler(this.btnWatchedImport_Click);
          // 
          // btnWatchedExport
          // 
          this.btnWatchedExport.Location = new System.Drawing.Point(273, 11);
          this.btnWatchedExport.Name = "btnWatchedExport";
          this.btnWatchedExport.Size = new System.Drawing.Size(75, 21);
          this.btnWatchedExport.TabIndex = 85;
          this.btnWatchedExport.Text = "export";
          this.ToolTip1.SetToolTip(this.btnWatchedExport, resources.GetString("btnWatchedExport.ToolTip"));
          this.btnWatchedExport.UseVisualStyleBackColor = true;
          this.btnWatchedExport.Click += new System.EventHandler(this.btnWatchedExport_Click);
          // 
          // lblUnwatchedItemsValue
          // 
          this.lblUnwatchedItemsValue.AutoSize = true;
          this.lblUnwatchedItemsValue.Location = new System.Drawing.Point(183, 57);
          this.lblUnwatchedItemsValue.Name = "lblUnwatchedItemsValue";
          this.lblUnwatchedItemsValue.Size = new System.Drawing.Size(165, 13);
          this.lblUnwatchedItemsValue.TabIndex = 84;
          this.lblUnwatchedItemsValue.Text = "Value to identify unwatched items";
          // 
          // textBoxGlobalUnwatchedOnlyValue
          // 
          this.textBoxGlobalUnwatchedOnlyValue.Location = new System.Drawing.Point(186, 75);
          this.textBoxGlobalUnwatchedOnlyValue.Name = "textBoxGlobalUnwatchedOnlyValue";
          this.textBoxGlobalUnwatchedOnlyValue.Size = new System.Drawing.Size(162, 20);
          this.textBoxGlobalUnwatchedOnlyValue.TabIndex = 83;
          this.ToolTip1.SetToolTip(this.textBoxGlobalUnwatchedOnlyValue, resources.GetString("textBoxGlobalUnwatchedOnlyValue.ToolTip"));
          // 
          // CheckWatchedPlayerStopped
          // 
          this.CheckWatchedPlayerStopped.AutoSize = true;
          this.CheckWatchedPlayerStopped.Location = new System.Drawing.Point(10, 34);
          this.CheckWatchedPlayerStopped.Name = "CheckWatchedPlayerStopped";
          this.CheckWatchedPlayerStopped.Size = new System.Drawing.Size(238, 17);
          this.CheckWatchedPlayerStopped.TabIndex = 75;
          this.CheckWatchedPlayerStopped.Text = "Update when playback is stopped or finished";
          this.ToolTip1.SetToolTip(this.CheckWatchedPlayerStopped, "Watched status (by updating the field configured for Watched) will be set if:\r\n- " +
                  "movie ended\r\n- movie was stopped at a playtime >= 80%");
          this.CheckWatchedPlayerStopped.UseVisualStyleBackColor = true;
          this.CheckWatchedPlayerStopped.CheckedChanged += new System.EventHandler(this.CheckWatchedPlayerStopped_CheckedChanged);
          // 
          // Label_UserProfileName
          // 
          this.Label_UserProfileName.AutoSize = true;
          this.Label_UserProfileName.Location = new System.Drawing.Point(28, 120);
          this.Label_UserProfileName.Name = "Label_UserProfileName";
          this.Label_UserProfileName.Size = new System.Drawing.Size(125, 13);
          this.Label_UserProfileName.TabIndex = 72;
          this.Label_UserProfileName.Text = "Active User Profile Name";
          // 
          // UserProfileName
          // 
          this.UserProfileName.Enabled = false;
          this.UserProfileName.Location = new System.Drawing.Point(186, 117);
          this.UserProfileName.Name = "UserProfileName";
          this.UserProfileName.Size = new System.Drawing.Size(162, 20);
          this.UserProfileName.TabIndex = 71;
          this.ToolTip1.SetToolTip(this.UserProfileName, resources.GetString("UserProfileName.ToolTip"));
          // 
          // label19
          // 
          this.label19.AutoSize = true;
          this.label19.Location = new System.Drawing.Point(8, 57);
          this.label19.Name = "label19";
          this.label19.Size = new System.Drawing.Size(150, 13);
          this.label19.TabIndex = 74;
          this.label19.Text = "Field used for Watched Status";
          // 
          // cbWatched
          // 
          this.cbWatched.FormattingEnabled = true;
          this.cbWatched.Location = new System.Drawing.Point(10, 74);
          this.cbWatched.Name = "cbWatched";
          this.cbWatched.Size = new System.Drawing.Size(123, 21);
          this.cbWatched.TabIndex = 73;
          this.ToolTip1.SetToolTip(this.cbWatched, resources.GetString("cbWatched.ToolTip"));
          // 
          // chkEnhancedWatchedStatusHandling
          // 
          this.chkEnhancedWatchedStatusHandling.AutoSize = true;
          this.chkEnhancedWatchedStatusHandling.Location = new System.Drawing.Point(10, 98);
          this.chkEnhancedWatchedStatusHandling.Name = "chkEnhancedWatchedStatusHandling";
          this.chkEnhancedWatchedStatusHandling.Size = new System.Drawing.Size(200, 17);
          this.chkEnhancedWatchedStatusHandling.TabIndex = 70;
          this.chkEnhancedWatchedStatusHandling.Text = "Enhanced Watched Status Handling";
          this.ToolTip1.SetToolTip(this.chkEnhancedWatchedStatusHandling, resources.GetString("chkEnhancedWatchedStatusHandling.ToolTip"));
          this.chkEnhancedWatchedStatusHandling.UseVisualStyleBackColor = true;
          this.chkEnhancedWatchedStatusHandling.CheckedChanged += new System.EventHandler(this.chkEnhancedWatchedStatusHandling_CheckedChanged);
          // 
          // CheckWatched
          // 
          this.CheckWatched.AutoSize = true;
          this.CheckWatched.Location = new System.Drawing.Point(10, 15);
          this.CheckWatched.Name = "CheckWatched";
          this.CheckWatched.Size = new System.Drawing.Size(178, 17);
          this.CheckWatched.TabIndex = 69;
          this.CheckWatched.Text = "Update when movie is launched";
          this.ToolTip1.SetToolTip(this.CheckWatched, "Watched status (by updating the field configured for Watched) will be set if:\r\n- " +
                  "movie is started");
          this.CheckWatched.UseVisualStyleBackColor = true;
          this.CheckWatched.CheckedChanged += new System.EventHandler(this.CheckWatched_CheckedChanged);
          // 
          // groupBox_AntUpdatingItems
          // 
          this.groupBox_AntUpdatingItems.Controls.Add(this.label46);
          this.groupBox_AntUpdatingItems.Controls.Add(this.label45);
          this.groupBox_AntUpdatingItems.Controls.Add(this.label44);
          this.groupBox_AntUpdatingItems.Controls.Add(this.chkWindowsFileDialog);
          this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdDflT2);
          this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdDflT1);
          this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdItem2);
          this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdText2);
          this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdItem1);
          this.groupBox_AntUpdatingItems.Controls.Add(this.AntUpdText1);
          this.groupBox_AntUpdatingItems.Location = new System.Drawing.Point(17, 6);
          this.groupBox_AntUpdatingItems.Name = "groupBox_AntUpdatingItems";
          this.groupBox_AntUpdatingItems.Size = new System.Drawing.Size(354, 104);
          this.groupBox_AntUpdatingItems.TabIndex = 33;
          this.groupBox_AntUpdatingItems.TabStop = false;
          this.groupBox_AntUpdatingItems.Text = "User defined Updating Items for manual updates";
          this.ToolTip1.SetToolTip(this.groupBox_AntUpdatingItems, resources.GetString("groupBox_AntUpdatingItems.ToolTip"));
          // 
          // label46
          // 
          this.label46.AutoSize = true;
          this.label46.Location = new System.Drawing.Point(228, 15);
          this.label46.Name = "label46";
          this.label46.Size = new System.Drawing.Size(72, 13);
          this.label46.TabIndex = 80;
          this.label46.Text = "Update Value";
          // 
          // label45
          // 
          this.label45.AutoSize = true;
          this.label45.Location = new System.Drawing.Point(136, 15);
          this.label45.Name = "label45";
          this.label45.Size = new System.Drawing.Size(33, 13);
          this.label45.TabIndex = 79;
          this.label45.Text = "Label";
          // 
          // label44
          // 
          this.label44.AutoSize = true;
          this.label44.Location = new System.Drawing.Point(9, 15);
          this.label44.Name = "label44";
          this.label44.Size = new System.Drawing.Size(93, 13);
          this.label44.TabIndex = 78;
          this.label44.Text = "DB Item to update";
          // 
          // chkWindowsFileDialog
          // 
          this.chkWindowsFileDialog.AutoSize = true;
          this.chkWindowsFileDialog.Location = new System.Drawing.Point(10, 82);
          this.chkWindowsFileDialog.Name = "chkWindowsFileDialog";
          this.chkWindowsFileDialog.Size = new System.Drawing.Size(328, 17);
          this.chkWindowsFileDialog.TabIndex = 77;
          this.chkWindowsFileDialog.Text = "Use Standard Windows File Dialog for Movie file name Updating";
          this.ToolTip1.SetToolTip(this.chkWindowsFileDialog, resources.GetString("chkWindowsFileDialog.ToolTip"));
          this.chkWindowsFileDialog.UseVisualStyleBackColor = true;
          // 
          // AntUpdDflT2
          // 
          this.AntUpdDflT2.Location = new System.Drawing.Point(231, 57);
          this.AntUpdDflT2.Name = "AntUpdDflT2";
          this.AntUpdDflT2.Size = new System.Drawing.Size(117, 20);
          this.AntUpdDflT2.TabIndex = 35;
          // 
          // AntUpdDflT1
          // 
          this.AntUpdDflT1.Location = new System.Drawing.Point(231, 32);
          this.AntUpdDflT1.Name = "AntUpdDflT1";
          this.AntUpdDflT1.Size = new System.Drawing.Size(117, 20);
          this.AntUpdDflT1.TabIndex = 32;
          // 
          // AntUpdItem2
          // 
          this.AntUpdItem2.FormattingEnabled = true;
          this.AntUpdItem2.Location = new System.Drawing.Point(10, 57);
          this.AntUpdItem2.Name = "AntUpdItem2";
          this.AntUpdItem2.Size = new System.Drawing.Size(123, 21);
          this.AntUpdItem2.TabIndex = 33;
          // 
          // AntUpdText2
          // 
          this.AntUpdText2.Location = new System.Drawing.Point(139, 58);
          this.AntUpdText2.Name = "AntUpdText2";
          this.AntUpdText2.Size = new System.Drawing.Size(86, 20);
          this.AntUpdText2.TabIndex = 34;
          // 
          // AntUpdItem1
          // 
          this.AntUpdItem1.FormattingEnabled = true;
          this.AntUpdItem1.Location = new System.Drawing.Point(10, 32);
          this.AntUpdItem1.Name = "AntUpdItem1";
          this.AntUpdItem1.Size = new System.Drawing.Size(123, 21);
          this.AntUpdItem1.TabIndex = 30;
          // 
          // AntUpdText1
          // 
          this.AntUpdText1.Location = new System.Drawing.Point(139, 33);
          this.AntUpdText1.Name = "AntUpdText1";
          this.AntUpdText1.Size = new System.Drawing.Size(86, 20);
          this.AntUpdText1.TabIndex = 31;
          // 
          // Tab_Search
          // 
          this.Tab_Search.Controls.Add(this.groupBox_SortByItem);
          this.Tab_Search.Controls.Add(this.groupBoxSearchByProperties);
          this.Tab_Search.Controls.Add(this.groupBox_SupplementarySearch);
          this.Tab_Search.Controls.Add(this.groupBox_Separators);
          this.Tab_Search.Location = new System.Drawing.Point(4, 22);
          this.Tab_Search.Name = "Tab_Search";
          this.Tab_Search.Size = new System.Drawing.Size(747, 354);
          this.Tab_Search.TabIndex = 9;
          this.Tab_Search.Text = "Sort/Search";
          this.ToolTip1.SetToolTip(this.Tab_Search, "Setup for user defined searches \r\nand user defined sort items");
          this.Tab_Search.UseVisualStyleBackColor = true;
          // 
          // groupBox_SortByItem
          // 
          this.groupBox_SortByItem.Controls.Add(this.AntSort2);
          this.groupBox_SortByItem.Controls.Add(this.AntTSort2);
          this.groupBox_SortByItem.Controls.Add(this.AntSort1);
          this.groupBox_SortByItem.Controls.Add(this.AntTSort1);
          this.groupBox_SortByItem.Location = new System.Drawing.Point(15, 11);
          this.groupBox_SortByItem.Name = "groupBox_SortByItem";
          this.groupBox_SortByItem.Size = new System.Drawing.Size(350, 77);
          this.groupBox_SortByItem.TabIndex = 36;
          this.groupBox_SortByItem.TabStop = false;
          this.groupBox_SortByItem.Text = "User defined Sort Item";
          this.ToolTip1.SetToolTip(this.groupBox_SortByItem, resources.GetString("groupBox_SortByItem.ToolTip"));
          // 
          // AntSort2
          // 
          this.AntSort2.FormattingEnabled = true;
          this.AntSort2.Location = new System.Drawing.Point(10, 45);
          this.AntSort2.Name = "AntSort2";
          this.AntSort2.Size = new System.Drawing.Size(145, 21);
          this.AntSort2.TabIndex = 38;
          // 
          // AntTSort2
          // 
          this.AntTSort2.Location = new System.Drawing.Point(164, 45);
          this.AntTSort2.Name = "AntTSort2";
          this.AntTSort2.Size = new System.Drawing.Size(173, 20);
          this.AntTSort2.TabIndex = 39;
          // 
          // AntSort1
          // 
          this.AntSort1.FormattingEnabled = true;
          this.AntSort1.Location = new System.Drawing.Point(10, 18);
          this.AntSort1.Name = "AntSort1";
          this.AntSort1.Size = new System.Drawing.Size(145, 21);
          this.AntSort1.TabIndex = 36;
          // 
          // AntTSort1
          // 
          this.AntTSort1.Location = new System.Drawing.Point(164, 19);
          this.AntTSort1.Name = "AntTSort1";
          this.AntTSort1.Size = new System.Drawing.Size(173, 20);
          this.AntTSort1.TabIndex = 37;
          // 
          // groupBoxSearchByProperties
          // 
          this.groupBoxSearchByProperties.Controls.Add(this.AntSearchFieldReset);
          this.groupBoxSearchByProperties.Controls.Add(this.AntSearchFieldClear);
          this.groupBoxSearchByProperties.Controls.Add(this.label18);
          this.groupBoxSearchByProperties.Controls.Add(this.AntSearchField);
          this.groupBoxSearchByProperties.Controls.Add(this.AntSearchList);
          this.groupBoxSearchByProperties.Location = new System.Drawing.Point(15, 225);
          this.groupBoxSearchByProperties.Name = "groupBoxSearchByProperties";
          this.groupBoxSearchByProperties.Size = new System.Drawing.Size(712, 114);
          this.groupBoxSearchByProperties.TabIndex = 35;
          this.groupBoxSearchByProperties.TabStop = false;
          this.groupBoxSearchByProperties.Text = "Global Search by Properties";
          this.ToolTip1.SetToolTip(this.groupBoxSearchByProperties, "In opposition to the \"User defined Search Items\", this is a more generic search \r" +
                  "\nand it is based on searching the whole catalog, including items, that are not v" +
                  "isible in the current view\r\n");
          // 
          // AntSearchFieldReset
          // 
          this.AntSearchFieldReset.Location = new System.Drawing.Point(591, 34);
          this.AntSearchFieldReset.Name = "AntSearchFieldReset";
          this.AntSearchFieldReset.Size = new System.Drawing.Size(106, 21);
          this.AntSearchFieldReset.TabIndex = 86;
          this.AntSearchFieldReset.Text = "Reset to Defaults";
          this.ToolTip1.SetToolTip(this.AntSearchFieldReset, "This will reset the search list to the MyFilms default values.");
          this.AntSearchFieldReset.UseVisualStyleBackColor = true;
          this.AntSearchFieldReset.Click += new System.EventHandler(this.AntSearchFieldResetToDefault_Click);
          // 
          // AntSearchFieldClear
          // 
          this.AntSearchFieldClear.Location = new System.Drawing.Point(490, 34);
          this.AntSearchFieldClear.Name = "AntSearchFieldClear";
          this.AntSearchFieldClear.Size = new System.Drawing.Size(64, 21);
          this.AntSearchFieldClear.TabIndex = 85;
          this.AntSearchFieldClear.Text = "Clear";
          this.ToolTip1.SetToolTip(this.AntSearchFieldClear, "This will clear the items from the search list.");
          this.AntSearchFieldClear.UseVisualStyleBackColor = true;
          this.AntSearchFieldClear.Click += new System.EventHandler(this.AntSearchFieldClear_Click);
          // 
          // label18
          // 
          this.label18.AutoSize = true;
          this.label18.Location = new System.Drawing.Point(13, 38);
          this.label18.Name = "label18";
          this.label18.Size = new System.Drawing.Size(152, 13);
          this.label18.TabIndex = 80;
          this.label18.Text = "Search by Properties Selection";
          // 
          // AntSearchField
          // 
          this.AntSearchField.FormattingEnabled = true;
          this.AntSearchField.Location = new System.Drawing.Point(189, 34);
          this.AntSearchField.Name = "AntSearchField";
          this.AntSearchField.Size = new System.Drawing.Size(148, 21);
          this.AntSearchField.TabIndex = 78;
          this.ToolTip1.SetToolTip(this.AntSearchField, "Choose a DB item to add to the property search list.");
          this.AntSearchField.SelectedIndexChanged += new System.EventHandler(this.AntSearchField_SelectedIndexChanged);
          // 
          // AntSearchList
          // 
          this.AntSearchList.BackColor = System.Drawing.SystemColors.Control;
          this.AntSearchList.Location = new System.Drawing.Point(16, 72);
          this.AntSearchList.Name = "AntSearchList";
          this.AntSearchList.Size = new System.Drawing.Size(681, 20);
          this.AntSearchList.TabIndex = 79;
          this.ToolTip1.SetToolTip(this.AntSearchList, "This field cannot be edited directly, but shows you the chosen DB items for searc" +
                  "h.");
          // 
          // groupBox_SupplementarySearch
          // 
          this.groupBox_SupplementarySearch.Controls.Add(this.label43);
          this.groupBox_SupplementarySearch.Controls.Add(this.label42);
          this.groupBox_SupplementarySearch.Controls.Add(this.lblUserdefinedSearchItems);
          this.groupBox_SupplementarySearch.Controls.Add(this.AntSearchItem2);
          this.groupBox_SupplementarySearch.Controls.Add(this.AntSearchText2);
          this.groupBox_SupplementarySearch.Controls.Add(this.AntSearchItem1);
          this.groupBox_SupplementarySearch.Controls.Add(this.AntSearchText1);
          this.groupBox_SupplementarySearch.Location = new System.Drawing.Point(15, 94);
          this.groupBox_SupplementarySearch.Name = "groupBox_SupplementarySearch";
          this.groupBox_SupplementarySearch.Size = new System.Drawing.Size(350, 125);
          this.groupBox_SupplementarySearch.TabIndex = 34;
          this.groupBox_SupplementarySearch.TabStop = false;
          this.groupBox_SupplementarySearch.Text = "User defined Search Items";
          this.ToolTip1.SetToolTip(this.groupBox_SupplementarySearch, resources.GetString("groupBox_SupplementarySearch.ToolTip"));
          // 
          // label43
          // 
          this.label43.AutoSize = true;
          this.label43.Location = new System.Drawing.Point(164, 22);
          this.label43.Name = "label43";
          this.label43.Size = new System.Drawing.Size(161, 13);
          this.label43.TabIndex = 42;
          this.label43.Text = "Search Label to display on menu";
          // 
          // label42
          // 
          this.label42.AutoSize = true;
          this.label42.Location = new System.Drawing.Point(8, 22);
          this.label42.Name = "label42";
          this.label42.Size = new System.Drawing.Size(92, 13);
          this.label42.TabIndex = 41;
          this.label42.Text = "DB Item to search";
          // 
          // lblUserdefinedSearchItems
          // 
          this.lblUserdefinedSearchItems.AutoSize = true;
          this.lblUserdefinedSearchItems.Location = new System.Drawing.Point(25, 102);
          this.lblUserdefinedSearchItems.Name = "lblUserdefinedSearchItems";
          this.lblUserdefinedSearchItems.Size = new System.Drawing.Size(293, 13);
          this.lblUserdefinedSearchItems.TabIndex = 40;
          this.lblUserdefinedSearchItems.Text = "Those Items will show up as separate choice in search menu";
          // 
          // AntSearchItem2
          // 
          this.AntSearchItem2.FormattingEnabled = true;
          this.AntSearchItem2.Location = new System.Drawing.Point(10, 67);
          this.AntSearchItem2.Name = "AntSearchItem2";
          this.AntSearchItem2.Size = new System.Drawing.Size(145, 21);
          this.AntSearchItem2.TabIndex = 38;
          this.ToolTip1.SetToolTip(this.AntSearchItem2, "Choose a DB item for search");
          // 
          // AntSearchText2
          // 
          this.AntSearchText2.Location = new System.Drawing.Point(164, 68);
          this.AntSearchText2.Name = "AntSearchText2";
          this.AntSearchText2.Size = new System.Drawing.Size(173, 20);
          this.AntSearchText2.TabIndex = 39;
          this.ToolTip1.SetToolTip(this.AntSearchText2, "Label to be displayed in search menu");
          // 
          // AntSearchItem1
          // 
          this.AntSearchItem1.FormattingEnabled = true;
          this.AntSearchItem1.Location = new System.Drawing.Point(10, 40);
          this.AntSearchItem1.Name = "AntSearchItem1";
          this.AntSearchItem1.Size = new System.Drawing.Size(145, 21);
          this.AntSearchItem1.TabIndex = 36;
          this.ToolTip1.SetToolTip(this.AntSearchItem1, "Choose a DB item for Search");
          // 
          // AntSearchText1
          // 
          this.AntSearchText1.Location = new System.Drawing.Point(164, 41);
          this.AntSearchText1.Name = "AntSearchText1";
          this.AntSearchText1.Size = new System.Drawing.Size(173, 20);
          this.AntSearchText1.TabIndex = 37;
          this.ToolTip1.SetToolTip(this.AntSearchText1, "Label to be displayed in search menu");
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
          this.groupBox_Separators.Location = new System.Drawing.Point(377, 94);
          this.groupBox_Separators.Name = "groupBox_Separators";
          this.groupBox_Separators.Size = new System.Drawing.Size(350, 125);
          this.groupBox_Separators.TabIndex = 28;
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
          // Tab_Views
          // 
          this.Tab_Views.Controls.Add(this.groupBox_AntSelectedEnreg);
          this.Tab_Views.Controls.Add(this.groupBox_DefaultView);
          this.Tab_Views.Controls.Add(this.groupBox_SupplementaryView);
          this.Tab_Views.Controls.Add(this.groupBox_DetailedSupplementaryInformations);
          this.Tab_Views.Location = new System.Drawing.Point(4, 22);
          this.Tab_Views.Name = "Tab_Views";
          this.Tab_Views.Padding = new System.Windows.Forms.Padding(3);
          this.Tab_Views.Size = new System.Drawing.Size(747, 354);
          this.Tab_Views.TabIndex = 1;
          this.Tab_Views.Text = "Views/Filter";
          this.ToolTip1.SetToolTip(this.Tab_Views, "Setup for userdefined views and filters");
          this.Tab_Views.UseVisualStyleBackColor = true;
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
          this.groupBox_AntSelectedEnreg.Text = "User defined Config Filters";
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
          this.Selected_Enreg.Size = new System.Drawing.Size(355, 35);
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
          this.AntFilterText2.Size = new System.Drawing.Size(116, 20);
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
          this.AntFilterText1.Size = new System.Drawing.Size(116, 20);
          this.AntFilterText1.TabIndex = 21;
          // 
          // groupBox_DefaultView
          // 
          this.groupBox_DefaultView.Controls.Add(this.chkGlobalAvailableOnly);
          this.groupBox_DefaultView.Controls.Add(this.chkUseListviewForGroups);
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
          this.groupBox_DefaultView.Location = new System.Drawing.Point(9, 6);
          this.groupBox_DefaultView.Name = "groupBox_DefaultView";
          this.groupBox_DefaultView.Size = new System.Drawing.Size(347, 158);
          this.groupBox_DefaultView.TabIndex = 26;
          this.groupBox_DefaultView.TabStop = false;
          this.groupBox_DefaultView.Text = "Default Start View";
          this.ToolTip1.SetToolTip(this.groupBox_DefaultView, resources.GetString("groupBox_DefaultView.ToolTip"));
          // 
          // chkGlobalAvailableOnly
          // 
          this.chkGlobalAvailableOnly.AutoSize = true;
          this.chkGlobalAvailableOnly.Location = new System.Drawing.Point(183, 130);
          this.chkGlobalAvailableOnly.Name = "chkGlobalAvailableOnly";
          this.chkGlobalAvailableOnly.Size = new System.Drawing.Size(156, 17);
          this.chkGlobalAvailableOnly.TabIndex = 82;
          this.chkGlobalAvailableOnly.Text = "Show only available movies";
          this.ToolTip1.SetToolTip(this.chkGlobalAvailableOnly, resources.GetString("chkGlobalAvailableOnly.ToolTip"));
          this.chkGlobalAvailableOnly.UseVisualStyleBackColor = true;
          this.chkGlobalAvailableOnly.CheckedChanged += new System.EventHandler(this.chkGlobalAvailableOnly_CheckedChanged);
          // 
          // chkUseListviewForGroups
          // 
          this.chkUseListviewForGroups.AutoSize = true;
          this.chkUseListviewForGroups.Location = new System.Drawing.Point(10, 107);
          this.chkUseListviewForGroups.Name = "chkUseListviewForGroups";
          this.chkUseListviewForGroups.Size = new System.Drawing.Size(211, 17);
          this.chkUseListviewForGroups.TabIndex = 81;
          this.chkUseListviewForGroups.Text = "Always use List Layout for Group Views";
          this.ToolTip1.SetToolTip(this.chkUseListviewForGroups, "If checked, MyFilms will always use list view for group views,\r\nlike \"years\", \"co" +
                  "untry\" etc.\r\nIf unchecked, MyFilms will use same layout as used for films.");
          this.chkUseListviewForGroups.UseVisualStyleBackColor = true;
          // 
          // chkGlobalUnwatchedOnly
          // 
          this.chkGlobalUnwatchedOnly.AutoSize = true;
          this.chkGlobalUnwatchedOnly.Location = new System.Drawing.Point(10, 130);
          this.chkGlobalUnwatchedOnly.Name = "chkGlobalUnwatchedOnly";
          this.chkGlobalUnwatchedOnly.Size = new System.Drawing.Size(167, 17);
          this.chkGlobalUnwatchedOnly.TabIndex = 80;
          this.chkGlobalUnwatchedOnly.Text = "Show only unwatched movies";
          this.ToolTip1.SetToolTip(this.chkGlobalUnwatchedOnly, resources.GetString("chkGlobalUnwatchedOnly.ToolTip"));
          this.chkGlobalUnwatchedOnly.UseVisualStyleBackColor = true;
          // 
          // label10
          // 
          this.label10.AutoSize = true;
          this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
          this.label10.Location = new System.Drawing.Point(27, 91);
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
            "Cover Flow"});
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
          this.groupBox_SupplementaryView.Controls.Add(this.label41);
          this.groupBox_SupplementaryView.Controls.Add(this.label31);
          this.groupBox_SupplementaryView.Controls.Add(this.label26);
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
          this.groupBox_SupplementaryView.Location = new System.Drawing.Point(9, 170);
          this.groupBox_SupplementaryView.Name = "groupBox_SupplementaryView";
          this.groupBox_SupplementaryView.Size = new System.Drawing.Size(347, 178);
          this.groupBox_SupplementaryView.TabIndex = 24;
          this.groupBox_SupplementaryView.TabStop = false;
          this.groupBox_SupplementaryView.Text = "User defined Views";
          this.ToolTip1.SetToolTip(this.groupBox_SupplementaryView, resources.GetString("groupBox_SupplementaryView.ToolTip"));
          // 
          // label41
          // 
          this.label41.AutoSize = true;
          this.label41.Location = new System.Drawing.Point(238, 23);
          this.label41.Name = "label41";
          this.label41.Size = new System.Drawing.Size(71, 13);
          this.label41.TabIndex = 29;
          this.label41.Text = "Default Value";
          // 
          // label31
          // 
          this.label31.AutoSize = true;
          this.label31.Location = new System.Drawing.Point(123, 23);
          this.label31.Name = "label31";
          this.label31.Size = new System.Drawing.Size(33, 13);
          this.label31.TabIndex = 28;
          this.label31.Text = "Label";
          // 
          // label26
          // 
          this.label26.AutoSize = true;
          this.label26.Location = new System.Drawing.Point(7, 23);
          this.label26.Name = "label26";
          this.label26.Size = new System.Drawing.Size(82, 13);
          this.label26.TabIndex = 27;
          this.label26.Text = "DB Item to view";
          // 
          // AntViewValue5
          // 
          this.AntViewValue5.Location = new System.Drawing.Point(237, 146);
          this.AntViewValue5.Name = "AntViewValue5";
          this.AntViewValue5.Size = new System.Drawing.Size(103, 20);
          this.AntViewValue5.TabIndex = 26;
          // 
          // AntViewText5
          // 
          this.AntViewText5.Location = new System.Drawing.Point(126, 146);
          this.AntViewText5.Name = "AntViewText5";
          this.AntViewText5.Size = new System.Drawing.Size(105, 20);
          this.AntViewText5.TabIndex = 25;
          this.AntViewText5.Leave += new System.EventHandler(this.AntViewText5_Leave);
          // 
          // AntViewItem5
          // 
          this.AntViewItem5.FormattingEnabled = true;
          this.AntViewItem5.Location = new System.Drawing.Point(10, 146);
          this.AntViewItem5.Name = "AntViewItem5";
          this.AntViewItem5.Size = new System.Drawing.Size(110, 21);
          this.AntViewItem5.TabIndex = 24;
          this.AntViewItem5.SelectedIndexChanged += new System.EventHandler(this.AntViewItem5_SelectedIndexChanged);
          // 
          // AntViewValue4
          // 
          this.AntViewValue4.Location = new System.Drawing.Point(237, 120);
          this.AntViewValue4.Name = "AntViewValue4";
          this.AntViewValue4.Size = new System.Drawing.Size(103, 20);
          this.AntViewValue4.TabIndex = 23;
          // 
          // AntViewText4
          // 
          this.AntViewText4.Location = new System.Drawing.Point(126, 120);
          this.AntViewText4.Name = "AntViewText4";
          this.AntViewText4.Size = new System.Drawing.Size(105, 20);
          this.AntViewText4.TabIndex = 22;
          this.AntViewText4.Leave += new System.EventHandler(this.AntViewText4_Leave);
          // 
          // AntViewItem4
          // 
          this.AntViewItem4.FormattingEnabled = true;
          this.AntViewItem4.Location = new System.Drawing.Point(10, 120);
          this.AntViewItem4.Name = "AntViewItem4";
          this.AntViewItem4.Size = new System.Drawing.Size(110, 21);
          this.AntViewItem4.TabIndex = 21;
          this.AntViewItem4.SelectedIndexChanged += new System.EventHandler(this.AntViewItem4_SelectedIndexChanged);
          // 
          // AntViewValue3
          // 
          this.AntViewValue3.Location = new System.Drawing.Point(237, 94);
          this.AntViewValue3.Name = "AntViewValue3";
          this.AntViewValue3.Size = new System.Drawing.Size(103, 20);
          this.AntViewValue3.TabIndex = 20;
          // 
          // AntViewText3
          // 
          this.AntViewText3.Location = new System.Drawing.Point(126, 94);
          this.AntViewText3.Name = "AntViewText3";
          this.AntViewText3.Size = new System.Drawing.Size(105, 20);
          this.AntViewText3.TabIndex = 19;
          this.AntViewText3.Leave += new System.EventHandler(this.AntViewText3_Leave);
          // 
          // AntViewItem3
          // 
          this.AntViewItem3.FormattingEnabled = true;
          this.AntViewItem3.Location = new System.Drawing.Point(10, 94);
          this.AntViewItem3.Name = "AntViewItem3";
          this.AntViewItem3.Size = new System.Drawing.Size(110, 21);
          this.AntViewItem3.TabIndex = 18;
          this.AntViewItem3.SelectedIndexChanged += new System.EventHandler(this.AntViewItem3_SelectedIndexChanged);
          // 
          // AntViewValue2
          // 
          this.AntViewValue2.Location = new System.Drawing.Point(237, 68);
          this.AntViewValue2.Name = "AntViewValue2";
          this.AntViewValue2.Size = new System.Drawing.Size(103, 20);
          this.AntViewValue2.TabIndex = 17;
          // 
          // AntViewValue1
          // 
          this.AntViewValue1.Location = new System.Drawing.Point(237, 42);
          this.AntViewValue1.Name = "AntViewValue1";
          this.AntViewValue1.Size = new System.Drawing.Size(103, 20);
          this.AntViewValue1.TabIndex = 14;
          // 
          // AntViewText2
          // 
          this.AntViewText2.Location = new System.Drawing.Point(126, 68);
          this.AntViewText2.Name = "AntViewText2";
          this.AntViewText2.Size = new System.Drawing.Size(105, 20);
          this.AntViewText2.TabIndex = 16;
          this.AntViewText2.Leave += new System.EventHandler(this.AntViewText2_Leave);
          // 
          // AntViewText1
          // 
          this.AntViewText1.Location = new System.Drawing.Point(126, 42);
          this.AntViewText1.Name = "AntViewText1";
          this.AntViewText1.Size = new System.Drawing.Size(105, 20);
          this.AntViewText1.TabIndex = 13;
          this.AntViewText1.TextChanged += new System.EventHandler(this.AntViewText1_Leave);
          this.AntViewText1.Leave += new System.EventHandler(this.AntViewText1_Leave);
          // 
          // AntViewItem2
          // 
          this.AntViewItem2.FormattingEnabled = true;
          this.AntViewItem2.Location = new System.Drawing.Point(10, 68);
          this.AntViewItem2.Name = "AntViewItem2";
          this.AntViewItem2.Size = new System.Drawing.Size(110, 21);
          this.AntViewItem2.TabIndex = 15;
          this.AntViewItem2.SelectedIndexChanged += new System.EventHandler(this.AntViewItem2_SelectedIndexChanged);
          // 
          // AntViewItem1
          // 
          this.AntViewItem1.ItemHeight = 13;
          this.AntViewItem1.Location = new System.Drawing.Point(10, 42);
          this.AntViewItem1.Name = "AntViewItem1";
          this.AntViewItem1.Size = new System.Drawing.Size(110, 21);
          this.AntViewItem1.TabIndex = 12;
          this.AntViewItem1.SelectedIndexChanged += new System.EventHandler(this.AntViewItem1_SelectedIndexChanged);
          // 
          // groupBox_DetailedSupplementaryInformations
          // 
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntLabel5);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntLabel4);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntItem5);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntItem4);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntLabel3);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.chkOnlyTitle);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntItem3);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntItem2);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntLabel2);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.label7);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntItem1);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.label3);
          this.groupBox_DetailedSupplementaryInformations.Controls.Add(this.AntLabel1);
          this.groupBox_DetailedSupplementaryInformations.Location = new System.Drawing.Point(371, 129);
          this.groupBox_DetailedSupplementaryInformations.Name = "groupBox_DetailedSupplementaryInformations";
          this.groupBox_DetailedSupplementaryInformations.Size = new System.Drawing.Size(370, 219);
          this.groupBox_DetailedSupplementaryInformations.TabIndex = 22;
          this.groupBox_DetailedSupplementaryInformations.TabStop = false;
          this.groupBox_DetailedSupplementaryInformations.Text = "User defined Display Items";
          this.ToolTip1.SetToolTip(this.groupBox_DetailedSupplementaryInformations, resources.GetString("groupBox_DetailedSupplementaryInformations.ToolTip"));
          // 
          // AntLabel5
          // 
          this.AntLabel5.Location = new System.Drawing.Point(185, 152);
          this.AntLabel5.Name = "AntLabel5";
          this.AntLabel5.Size = new System.Drawing.Size(173, 20);
          this.AntLabel5.TabIndex = 35;
          // 
          // AntLabel4
          // 
          this.AntLabel4.Location = new System.Drawing.Point(185, 125);
          this.AntLabel4.Name = "AntLabel4";
          this.AntLabel4.Size = new System.Drawing.Size(173, 20);
          this.AntLabel4.TabIndex = 34;
          // 
          // AntItem5
          // 
          this.AntItem5.FormattingEnabled = true;
          this.AntItem5.Location = new System.Drawing.Point(10, 151);
          this.AntItem5.Name = "AntItem5";
          this.AntItem5.Size = new System.Drawing.Size(160, 21);
          this.AntItem5.TabIndex = 33;
          // 
          // AntItem4
          // 
          this.AntItem4.FormattingEnabled = true;
          this.AntItem4.Location = new System.Drawing.Point(10, 124);
          this.AntItem4.Name = "AntItem4";
          this.AntItem4.Size = new System.Drawing.Size(160, 21);
          this.AntItem4.TabIndex = 32;
          // 
          // AntLabel3
          // 
          this.AntLabel3.Location = new System.Drawing.Point(185, 97);
          this.AntLabel3.Name = "AntLabel3";
          this.AntLabel3.Size = new System.Drawing.Size(173, 20);
          this.AntLabel3.TabIndex = 30;
          // 
          // chkOnlyTitle
          // 
          this.chkOnlyTitle.AutoSize = true;
          this.chkOnlyTitle.Location = new System.Drawing.Point(11, 187);
          this.chkOnlyTitle.Name = "chkOnlyTitle";
          this.chkOnlyTitle.Size = new System.Drawing.Size(230, 17);
          this.chkOnlyTitle.TabIndex = 31;
          this.chkOnlyTitle.Text = "Display Only Movie\'s Title within List Layout";
          this.chkOnlyTitle.UseVisualStyleBackColor = true;
          // 
          // AntItem3
          // 
          this.AntItem3.FormattingEnabled = true;
          this.AntItem3.Location = new System.Drawing.Point(10, 97);
          this.AntItem3.Name = "AntItem3";
          this.AntItem3.Size = new System.Drawing.Size(160, 21);
          this.AntItem3.TabIndex = 29;
          // 
          // AntItem2
          // 
          this.AntItem2.FormattingEnabled = true;
          this.AntItem2.Location = new System.Drawing.Point(10, 70);
          this.AntItem2.Name = "AntItem2";
          this.AntItem2.Size = new System.Drawing.Size(160, 21);
          this.AntItem2.TabIndex = 27;
          // 
          // AntLabel2
          // 
          this.AntLabel2.Location = new System.Drawing.Point(185, 70);
          this.AntLabel2.Name = "AntLabel2";
          this.AntLabel2.Size = new System.Drawing.Size(173, 20);
          this.AntLabel2.TabIndex = 28;
          // 
          // label7
          // 
          this.label7.AutoSize = true;
          this.label7.Location = new System.Drawing.Point(8, 25);
          this.label7.Name = "label7";
          this.label7.Size = new System.Drawing.Size(94, 13);
          this.label7.TabIndex = 22;
          this.label7.Text = "DB Item to Display";
          // 
          // AntItem1
          // 
          this.AntItem1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
          this.AntItem1.FormattingEnabled = true;
          this.AntItem1.Location = new System.Drawing.Point(10, 43);
          this.AntItem1.Name = "AntItem1";
          this.AntItem1.Size = new System.Drawing.Size(160, 21);
          this.AntItem1.Sorted = true;
          this.AntItem1.TabIndex = 25;
          // 
          // label3
          // 
          this.label3.AutoSize = true;
          this.label3.Location = new System.Drawing.Point(187, 25);
          this.label3.Name = "label3";
          this.label3.Size = new System.Drawing.Size(33, 13);
          this.label3.TabIndex = 21;
          this.label3.Text = "Label";
          // 
          // AntLabel1
          // 
          this.AntLabel1.Location = new System.Drawing.Point(185, 43);
          this.AntLabel1.Name = "AntLabel1";
          this.AntLabel1.Size = new System.Drawing.Size(173, 20);
          this.AntLabel1.TabIndex = 26;
          // 
          // Tab_Trailer
          // 
          this.Tab_Trailer.Controls.Add(this.groupBox24);
          this.Tab_Trailer.Location = new System.Drawing.Point(4, 22);
          this.Tab_Trailer.Name = "Tab_Trailer";
          this.Tab_Trailer.Padding = new System.Windows.Forms.Padding(3);
          this.Tab_Trailer.Size = new System.Drawing.Size(747, 354);
          this.Tab_Trailer.TabIndex = 7;
          this.Tab_Trailer.Text = "Trailer";
          this.ToolTip1.SetToolTip(this.Tab_Trailer, "Setup for trailer options like search paths and options for behaviour in GUI");
          this.Tab_Trailer.UseVisualStyleBackColor = true;
          // 
          // groupBox24
          // 
          this.groupBox24.Controls.Add(this.labelTrailers);
          this.groupBox24.Controls.Add(this.SearchSubDirsTrailer);
          this.groupBox24.Controls.Add(this.ShowTrailerWhenStartingMovie);
          this.groupBox24.Controls.Add(this.btnTrailer);
          this.groupBox24.Controls.Add(this.ShowTrailerPlayDialog);
          this.groupBox24.Controls.Add(this.PathStorageTrailer);
          this.groupBox24.Controls.Add(this.SearchFileNameTrailer);
          this.groupBox24.Controls.Add(this.label34);
          this.groupBox24.Controls.Add(this.ItemSearchFileNameTrailer);
          this.groupBox24.Controls.Add(this.AntStorageTrailer);
          this.groupBox24.Controls.Add(this.label35);
          this.groupBox24.Location = new System.Drawing.Point(23, 37);
          this.groupBox24.Name = "groupBox24";
          this.groupBox24.Size = new System.Drawing.Size(681, 266);
          this.groupBox24.TabIndex = 73;
          this.groupBox24.TabStop = false;
          this.groupBox24.Text = "DB Item for storing trailerinfo (borrower recommended)";
          this.ToolTip1.SetToolTip(this.groupBox24, resources.GetString("groupBox24.ToolTip"));
          // 
          // labelTrailers
          // 
          this.labelTrailers.AutoSize = true;
          this.labelTrailers.Location = new System.Drawing.Point(24, 27);
          this.labelTrailers.Name = "labelTrailers";
          this.labelTrailers.Size = new System.Drawing.Size(379, 13);
          this.labelTrailers.TabIndex = 74;
          this.labelTrailers.Text = "To enable Trailer support, you must select a DB Item/field for storing Trailer in" +
              "fo";
          // 
          // SearchSubDirsTrailer
          // 
          this.SearchSubDirsTrailer.AutoSize = true;
          this.SearchSubDirsTrailer.Location = new System.Drawing.Point(27, 121);
          this.SearchSubDirsTrailer.Name = "SearchSubDirsTrailer";
          this.SearchSubDirsTrailer.Size = new System.Drawing.Size(130, 17);
          this.SearchSubDirsTrailer.TabIndex = 74;
          this.SearchSubDirsTrailer.Text = "Search in Sub Folders";
          this.ToolTip1.SetToolTip(this.SearchSubDirsTrailer, "Select this option if you want the search for the trailer files\r\nto include sub-f" +
                  "olders of the defined path.\r\n");
          this.SearchSubDirsTrailer.UseVisualStyleBackColor = true;
          // 
          // ShowTrailerWhenStartingMovie
          // 
          this.ShowTrailerWhenStartingMovie.AutoSize = true;
          this.ShowTrailerWhenStartingMovie.Location = new System.Drawing.Point(27, 224);
          this.ShowTrailerWhenStartingMovie.Name = "ShowTrailerWhenStartingMovie";
          this.ShowTrailerWhenStartingMovie.Size = new System.Drawing.Size(228, 17);
          this.ShowTrailerWhenStartingMovie.TabIndex = 73;
          this.ShowTrailerWhenStartingMovie.Text = "Show Trailer as Movie Intro (Cinema mode)";
          this.ToolTip1.SetToolTip(this.ShowTrailerWhenStartingMovie, "-- Coming in future version --\r\nIf checked, a trailer from same category as movie" +
                  " \r\nwill be played before starting the movie itself.");
          this.ShowTrailerWhenStartingMovie.UseVisualStyleBackColor = true;
          // 
          // btnTrailer
          // 
          this.btnTrailer.Location = new System.Drawing.Point(634, 92);
          this.btnTrailer.Name = "btnTrailer";
          this.btnTrailer.Size = new System.Drawing.Size(32, 20);
          this.btnTrailer.TabIndex = 70;
          this.btnTrailer.Text = "...";
          this.btnTrailer.UseVisualStyleBackColor = true;
          this.btnTrailer.Click += new System.EventHandler(this.btnTrailer_Click);
          // 
          // ShowTrailerPlayDialog
          // 
          this.ShowTrailerPlayDialog.AutoSize = true;
          this.ShowTrailerPlayDialog.Location = new System.Drawing.Point(27, 195);
          this.ShowTrailerPlayDialog.Name = "ShowTrailerPlayDialog";
          this.ShowTrailerPlayDialog.Size = new System.Drawing.Size(138, 17);
          this.ShowTrailerPlayDialog.TabIndex = 72;
          this.ShowTrailerPlayDialog.Text = "Show Trailer Userdialog";
          this.ToolTip1.SetToolTip(this.ShowTrailerPlayDialog, "-- Coming in future version --\r\nIf checked, a dialog will be displayed to select " +
                  "trailer, \r\nif more than one trailer is present.");
          this.ShowTrailerPlayDialog.UseVisualStyleBackColor = true;
          // 
          // PathStorageTrailer
          // 
          this.PathStorageTrailer.Location = new System.Drawing.Point(190, 92);
          this.PathStorageTrailer.Name = "PathStorageTrailer";
          this.PathStorageTrailer.Size = new System.Drawing.Size(428, 20);
          this.PathStorageTrailer.TabIndex = 68;
          this.ToolTip1.SetToolTip(this.PathStorageTrailer, resources.GetString("PathStorageTrailer.ToolTip"));
          // 
          // SearchFileNameTrailer
          // 
          this.SearchFileNameTrailer.AutoSize = true;
          this.SearchFileNameTrailer.Location = new System.Drawing.Point(27, 169);
          this.SearchFileNameTrailer.Name = "SearchFileNameTrailer";
          this.SearchFileNameTrailer.Size = new System.Drawing.Size(144, 17);
          this.SearchFileNameTrailer.TabIndex = 71;
          this.SearchFileNameTrailer.Text = "Search by Movie\'s Name";
          this.ToolTip1.SetToolTip(this.SearchFileNameTrailer, resources.GetString("SearchFileNameTrailer.ToolTip"));
          this.SearchFileNameTrailer.UseVisualStyleBackColor = true;
          // 
          // label34
          // 
          this.label34.AutoSize = true;
          this.label34.Location = new System.Drawing.Point(24, 95);
          this.label34.Name = "label34";
          this.label34.Size = new System.Drawing.Size(142, 13);
          this.label34.TabIndex = 69;
          this.label34.Text = "Extended Trailer Searchpath";
          // 
          // ItemSearchFileNameTrailer
          // 
          this.ItemSearchFileNameTrailer.FormattingEnabled = true;
          this.ItemSearchFileNameTrailer.Items.AddRange(new object[] {
            "OriginalTitle",
            "TranslatedTitle",
            "FormattedTitle"});
          this.ItemSearchFileNameTrailer.Location = new System.Drawing.Point(190, 167);
          this.ItemSearchFileNameTrailer.Name = "ItemSearchFileNameTrailer";
          this.ItemSearchFileNameTrailer.Size = new System.Drawing.Size(185, 21);
          this.ItemSearchFileNameTrailer.TabIndex = 70;
          this.ToolTip1.SetToolTip(this.ItemSearchFileNameTrailer, "Choose the title that should be used for matching mediafiles to the movies in you" +
                  "r DB.\r\n");
          // 
          // AntStorageTrailer
          // 
          this.AntStorageTrailer.Location = new System.Drawing.Point(190, 52);
          this.AntStorageTrailer.Name = "AntStorageTrailer";
          this.AntStorageTrailer.Size = new System.Drawing.Size(185, 21);
          this.AntStorageTrailer.TabIndex = 68;
          this.ToolTip1.SetToolTip(this.AntStorageTrailer, resources.GetString("AntStorageTrailer.ToolTip"));
          // 
          // label35
          // 
          this.label35.AutoSize = true;
          this.label35.Location = new System.Drawing.Point(24, 55);
          this.label35.Name = "label35";
          this.label35.Size = new System.Drawing.Size(147, 13);
          this.label35.TabIndex = 69;
          this.label35.Text = "DB Item for storing Trailer Info";
          // 
          // Tab_General
          // 
          this.Tab_General.Controls.Add(this.groupBoxSetupWizard);
          this.Tab_General.Controls.Add(this.lblYellowShowRequiredItems);
          this.Tab_General.Controls.Add(this.groupBox_Security);
          this.Tab_General.Controls.Add(this.groupBox_PreLaunchingCommand);
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
          this.ToolTip1.SetToolTip(this.Tab_General, "Basic configuration like selecting movie catalog, titles to display, etc.");
          this.Tab_General.UseVisualStyleBackColor = true;
          // 
          // groupBoxSetupWizard
          // 
          this.groupBoxSetupWizard.Controls.Add(this.btnFirstTimeSetup);
          this.groupBoxSetupWizard.Location = new System.Drawing.Point(477, 283);
          this.groupBoxSetupWizard.Name = "groupBoxSetupWizard";
          this.groupBoxSetupWizard.Size = new System.Drawing.Size(259, 64);
          this.groupBoxSetupWizard.TabIndex = 78;
          this.groupBoxSetupWizard.TabStop = false;
          this.groupBoxSetupWizard.Text = "Setup Wizard";
          this.ToolTip1.SetToolTip(this.groupBoxSetupWizard, resources.GetString("groupBoxSetupWizard.ToolTip"));
          // 
          // btnFirstTimeSetup
          // 
          this.btnFirstTimeSetup.Location = new System.Drawing.Point(13, 19);
          this.btnFirstTimeSetup.Name = "btnFirstTimeSetup";
          this.btnFirstTimeSetup.Size = new System.Drawing.Size(236, 35);
          this.btnFirstTimeSetup.TabIndex = 77;
          this.btnFirstTimeSetup.Text = "Setup Wizard\r\n(new configuration with presets)";
          this.btnFirstTimeSetup.UseVisualStyleBackColor = true;
          this.btnFirstTimeSetup.Click += new System.EventHandler(this.btnFirstTimeSetup_Click);
          // 
          // lblYellowShowRequiredItems
          // 
          this.lblYellowShowRequiredItems.AutoSize = true;
          this.lblYellowShowRequiredItems.BackColor = System.Drawing.SystemColors.Info;
          this.lblYellowShowRequiredItems.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
          this.lblYellowShowRequiredItems.Location = new System.Drawing.Point(533, 252);
          this.lblYellowShowRequiredItems.Name = "lblYellowShowRequiredItems";
          this.lblYellowShowRequiredItems.Size = new System.Drawing.Size(140, 28);
          this.lblYellowShowRequiredItems.TabIndex = 76;
          this.lblYellowShowRequiredItems.Text = "Yellow fields are mandatory \r\nfor the plugin to work !";
          this.lblYellowShowRequiredItems.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
          // 
          // groupBox_Security
          // 
          this.groupBox_Security.Controls.Add(this.label_Security);
          this.groupBox_Security.Controls.Add(this.label16);
          this.groupBox_Security.Controls.Add(this.label15);
          this.groupBox_Security.Controls.Add(this.Rpt_Dwp);
          this.groupBox_Security.Controls.Add(this.Dwp);
          this.groupBox_Security.Location = new System.Drawing.Point(475, 44);
          this.groupBox_Security.Name = "groupBox_Security";
          this.groupBox_Security.Size = new System.Drawing.Size(262, 101);
          this.groupBox_Security.TabIndex = 75;
          this.groupBox_Security.TabStop = false;
          this.groupBox_Security.Text = "Security";
          this.ToolTip1.SetToolTip(this.groupBox_Security, resources.GetString("groupBox_Security.ToolTip"));
          // 
          // label_Security
          // 
          this.label_Security.AutoSize = true;
          this.label_Security.Location = new System.Drawing.Point(74, 21);
          this.label_Security.Name = "label_Security";
          this.label_Security.Size = new System.Drawing.Size(167, 13);
          this.label_Security.TabIndex = 73;
          this.label_Security.Text = "Enter password to protect the DB:";
          // 
          // label16
          // 
          this.label16.AutoSize = true;
          this.label16.Location = new System.Drawing.Point(12, 72);
          this.label16.Name = "label16";
          this.label16.Size = new System.Drawing.Size(42, 13);
          this.label16.TabIndex = 72;
          this.label16.Text = "Repeat";
          // 
          // label15
          // 
          this.label15.AutoSize = true;
          this.label15.Location = new System.Drawing.Point(12, 46);
          this.label15.Name = "label15";
          this.label15.Size = new System.Drawing.Size(53, 13);
          this.label15.TabIndex = 71;
          this.label15.Text = "Password";
          // 
          // Rpt_Dwp
          // 
          this.Rpt_Dwp.Location = new System.Drawing.Point(77, 69);
          this.Rpt_Dwp.Name = "Rpt_Dwp";
          this.Rpt_Dwp.Size = new System.Drawing.Size(174, 20);
          this.Rpt_Dwp.TabIndex = 70;
          this.Rpt_Dwp.UseSystemPasswordChar = true;
          // 
          // Dwp
          // 
          this.Dwp.Location = new System.Drawing.Point(77, 43);
          this.Dwp.Name = "Dwp";
          this.Dwp.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
          this.Dwp.Size = new System.Drawing.Size(174, 20);
          this.Dwp.TabIndex = 69;
          this.ToolTip1.SetToolTip(this.Dwp, "Enter a password here if you wish to restrict access (from children, for example)" +
                  " \r\nto this particular configuration.  \r\nThe password must be repeated in the sec" +
                  "ond field.\r\n");
          this.Dwp.UseSystemPasswordChar = true;
          // 
          // groupBox_PreLaunchingCommand
          // 
          this.groupBox_PreLaunchingCommand.Controls.Add(this.label39);
          this.groupBox_PreLaunchingCommand.Controls.Add(this.label38);
          this.groupBox_PreLaunchingCommand.Controls.Add(this.CmdPar);
          this.groupBox_PreLaunchingCommand.Controls.Add(this.CmdExe);
          this.groupBox_PreLaunchingCommand.Location = new System.Drawing.Point(475, 155);
          this.groupBox_PreLaunchingCommand.Name = "groupBox_PreLaunchingCommand";
          this.groupBox_PreLaunchingCommand.Size = new System.Drawing.Size(262, 87);
          this.groupBox_PreLaunchingCommand.TabIndex = 70;
          this.groupBox_PreLaunchingCommand.TabStop = false;
          this.groupBox_PreLaunchingCommand.Text = "Pre-Launching Command";
          this.ToolTip1.SetToolTip(this.groupBox_PreLaunchingCommand, resources.GetString("groupBox_PreLaunchingCommand.ToolTip"));
          // 
          // label39
          // 
          this.label39.AutoSize = true;
          this.label39.Location = new System.Drawing.Point(12, 51);
          this.label39.Name = "label39";
          this.label39.Size = new System.Drawing.Size(55, 13);
          this.label39.TabIndex = 39;
          this.label39.Text = "Parameter";
          // 
          // label38
          // 
          this.label38.AutoSize = true;
          this.label38.Location = new System.Drawing.Point(12, 22);
          this.label38.Name = "label38";
          this.label38.Size = new System.Drawing.Size(77, 13);
          this.label38.TabIndex = 38;
          this.label38.Text = "Command Line";
          // 
          // CmdPar
          // 
          this.CmdPar.FormattingEnabled = true;
          this.CmdPar.Location = new System.Drawing.Point(93, 48);
          this.CmdPar.Name = "CmdPar";
          this.CmdPar.Size = new System.Drawing.Size(158, 21);
          this.CmdPar.TabIndex = 37;
          this.ToolTip1.SetToolTip(this.CmdPar, "The value of the  chosen DB-field will be used as commandline option for the appl" +
                  "ication laiunched");
          // 
          // CmdExe
          // 
          this.CmdExe.Location = new System.Drawing.Point(93, 19);
          this.CmdExe.Name = "CmdExe";
          this.CmdExe.Size = new System.Drawing.Size(158, 20);
          this.CmdExe.TabIndex = 36;
          this.ToolTip1.SetToolTip(this.CmdExe, "Enter the commandline to an application you want to launch before the movie is st" +
                  "arted in internal player");
          // 
          // groupBox_PlayMovieInfos
          // 
          this.groupBox_PlayMovieInfos.Controls.Add(this.chkScanMediaOnStart);
          this.groupBox_PlayMovieInfos.Controls.Add(this.groupBoxMoviePathInfos);
          this.groupBox_PlayMovieInfos.Controls.Add(this.AntIdentLabel);
          this.groupBox_PlayMovieInfos.Controls.Add(this.label6);
          this.groupBox_PlayMovieInfos.Controls.Add(this.AntStorage);
          this.groupBox_PlayMovieInfos.Controls.Add(this.label4);
          this.groupBox_PlayMovieInfos.Controls.Add(this.AntIdentItem);
          this.groupBox_PlayMovieInfos.Location = new System.Drawing.Point(7, 155);
          this.groupBox_PlayMovieInfos.Name = "groupBox_PlayMovieInfos";
          this.groupBox_PlayMovieInfos.Size = new System.Drawing.Size(458, 193);
          this.groupBox_PlayMovieInfos.TabIndex = 74;
          this.groupBox_PlayMovieInfos.TabStop = false;
          this.groupBox_PlayMovieInfos.Text = "Play Movie Info";
          // 
          // chkScanMediaOnStart
          // 
          this.chkScanMediaOnStart.AutoSize = true;
          this.chkScanMediaOnStart.Location = new System.Drawing.Point(27, 37);
          this.chkScanMediaOnStart.Name = "chkScanMediaOnStart";
          this.chkScanMediaOnStart.Size = new System.Drawing.Size(120, 17);
          this.chkScanMediaOnStart.TabIndex = 83;
          this.chkScanMediaOnStart.Text = "Scan media on start";
          this.ToolTip1.SetToolTip(this.chkScanMediaOnStart, resources.GetString("chkScanMediaOnStart.ToolTip"));
          this.chkScanMediaOnStart.UseVisualStyleBackColor = true;
          // 
          // groupBoxMoviePathInfos
          // 
          this.groupBoxMoviePathInfos.Controls.Add(this.SearchOnlyExactMatches);
          this.groupBoxMoviePathInfos.Controls.Add(this.label5);
          this.groupBoxMoviePathInfos.Controls.Add(this.PathStorage);
          this.groupBoxMoviePathInfos.Controls.Add(this.SearchSubDirs);
          this.groupBoxMoviePathInfos.Controls.Add(this.butPath);
          this.groupBoxMoviePathInfos.Controls.Add(this.SearchFileName);
          this.groupBoxMoviePathInfos.Controls.Add(this.ItemSearchFileName);
          this.groupBoxMoviePathInfos.Location = new System.Drawing.Point(6, 61);
          this.groupBoxMoviePathInfos.Name = "groupBoxMoviePathInfos";
          this.groupBoxMoviePathInfos.Size = new System.Drawing.Size(446, 96);
          this.groupBoxMoviePathInfos.TabIndex = 71;
          this.groupBoxMoviePathInfos.TabStop = false;
          this.groupBoxMoviePathInfos.Text = "Movie Path Configuration";
          // 
          // SearchOnlyExactMatches
          // 
          this.SearchOnlyExactMatches.AutoSize = true;
          this.SearchOnlyExactMatches.Location = new System.Drawing.Point(338, 38);
          this.SearchOnlyExactMatches.Name = "SearchOnlyExactMatches";
          this.SearchOnlyExactMatches.Size = new System.Drawing.Size(79, 30);
          this.SearchOnlyExactMatches.TabIndex = 69;
          this.SearchOnlyExactMatches.Text = "Only exact \r\nMatches";
          this.ToolTip1.SetToolTip(this.SearchOnlyExactMatches, resources.GetString("SearchOnlyExactMatches.ToolTip"));
          this.SearchOnlyExactMatches.UseVisualStyleBackColor = true;
          // 
          // label5
          // 
          this.label5.AutoSize = true;
          this.label5.Location = new System.Drawing.Point(6, 17);
          this.label5.Name = "label5";
          this.label5.Size = new System.Drawing.Size(145, 13);
          this.label5.TabIndex = 60;
          this.label5.Text = "Paths for Movies File Search ";
          // 
          // PathStorage
          // 
          this.PathStorage.Location = new System.Drawing.Point(166, 14);
          this.PathStorage.Name = "PathStorage";
          this.PathStorage.Size = new System.Drawing.Size(236, 20);
          this.PathStorage.TabIndex = 54;
          this.ToolTip1.SetToolTip(this.PathStorage, resources.GetString("PathStorage.ToolTip"));
          // 
          // SearchSubDirs
          // 
          this.SearchSubDirs.AutoSize = true;
          this.SearchSubDirs.Location = new System.Drawing.Point(21, 68);
          this.SearchSubDirs.Name = "SearchSubDirs";
          this.SearchSubDirs.Size = new System.Drawing.Size(130, 17);
          this.SearchSubDirs.TabIndex = 68;
          this.SearchSubDirs.Text = "Search in Sub Folders";
          this.ToolTip1.SetToolTip(this.SearchSubDirs, "Select this option if you want the search for the movie files \r\nto include sub-fo" +
                  "lders of the defined path.\r\n");
          this.SearchSubDirs.UseVisualStyleBackColor = true;
          // 
          // butPath
          // 
          this.butPath.Location = new System.Drawing.Point(408, 14);
          this.butPath.Name = "butPath";
          this.butPath.Size = new System.Drawing.Size(32, 20);
          this.butPath.TabIndex = 55;
          this.butPath.Text = "...";
          this.butPath.UseVisualStyleBackColor = true;
          this.butPath.Click += new System.EventHandler(this.butPath_Click);
          // 
          // SearchFileName
          // 
          this.SearchFileName.AutoSize = true;
          this.SearchFileName.Location = new System.Drawing.Point(9, 45);
          this.SearchFileName.Name = "SearchFileName";
          this.SearchFileName.Size = new System.Drawing.Size(144, 17);
          this.SearchFileName.TabIndex = 66;
          this.SearchFileName.Text = "Search by Movie\'s Name";
          this.ToolTip1.SetToolTip(this.SearchFileName, "Select this option if you want to search the file in cases it\'s not found \r\nwith " +
                  "the \'Item for Storage Info\' field. \r\nThe movie name will be used for file search" +
                  ".");
          this.SearchFileName.UseVisualStyleBackColor = true;
          // 
          // ItemSearchFileName
          // 
          this.ItemSearchFileName.FormattingEnabled = true;
          this.ItemSearchFileName.Items.AddRange(new object[] {
            "OriginalTitle",
            "TranslatedTitle",
            "FormattedTitle"});
          this.ItemSearchFileName.Location = new System.Drawing.Point(166, 43);
          this.ItemSearchFileName.Name = "ItemSearchFileName";
          this.ItemSearchFileName.Size = new System.Drawing.Size(157, 21);
          this.ItemSearchFileName.TabIndex = 67;
          this.ToolTip1.SetToolTip(this.ItemSearchFileName, "If file is not found with the \'Ant Item for Storage Info\' field, you \r\ncan search" +
                  " the file with the movie Name.\r\nChoose the Title used for the search.");
          // 
          // AntIdentLabel
          // 
          this.AntIdentLabel.Location = new System.Drawing.Point(335, 165);
          this.AntIdentLabel.Name = "AntIdentLabel";
          this.AntIdentLabel.Size = new System.Drawing.Size(117, 20);
          this.AntIdentLabel.TabIndex = 57;
          // 
          // label6
          // 
          this.label6.AutoSize = true;
          this.label6.Location = new System.Drawing.Point(11, 18);
          this.label6.Name = "label6";
          this.label6.Size = new System.Drawing.Size(125, 13);
          this.label6.TabIndex = 59;
          this.label6.Text = "Item for Storage  File Info";
          // 
          // AntStorage
          // 
          this.AntStorage.BackColor = System.Drawing.SystemColors.Info;
          this.AntStorage.FormattingEnabled = true;
          this.AntStorage.Location = new System.Drawing.Point(172, 16);
          this.AntStorage.Name = "AntStorage";
          this.AntStorage.Size = new System.Drawing.Size(222, 21);
          this.AntStorage.TabIndex = 53;
          this.ToolTip1.SetToolTip(this.AntStorage, resources.GetString("AntStorage.ToolTip"));
          // 
          // label4
          // 
          this.label4.AutoSize = true;
          this.label4.Location = new System.Drawing.Point(11, 167);
          this.label4.Name = "label4";
          this.label4.Size = new System.Drawing.Size(109, 13);
          this.label4.TabIndex = 58;
          this.label4.Text = "Ant Identification Item";
          // 
          // AntIdentItem
          // 
          this.AntIdentItem.FormattingEnabled = true;
          this.AntIdentItem.Location = new System.Drawing.Point(172, 164);
          this.AntIdentItem.Name = "AntIdentItem";
          this.AntIdentItem.Size = new System.Drawing.Size(157, 21);
          this.AntIdentItem.TabIndex = 56;
          this.ToolTip1.SetToolTip(this.AntIdentItem, resources.GetString("AntIdentItem.ToolTip"));
          // 
          // label13
          // 
          this.label13.AutoSize = true;
          this.label13.Location = new System.Drawing.Point(457, 17);
          this.label13.Name = "label13";
          this.label13.Size = new System.Drawing.Size(70, 13);
          this.label13.TabIndex = 63;
          this.label13.Text = "Catalog Type";
          // 
          // CatalogType
          // 
          this.CatalogType.BackColor = System.Drawing.SystemColors.Info;
          this.CatalogType.FormattingEnabled = true;
          this.CatalogType.Items.AddRange(new object[] {
            "Ant Movie Catalog (V3.5.1.2)",
            "DVD Profiler (V3.7.2)",
            "Movie Collector (V7.1.4)",
            "MyMovies (V3.18)",
            "Eax Movie Catalog (2.5.0)",
            "Eax Movie Catalog (3.0.9 b5)",
            "PVD - Personal Video Database (0.9.9.21)",
            "eXtreme Movie Manager (V7.1.1.1)",
            "XBMC (V10.0)",
            "MovingPicturesXML (V1.2 process plugin)",
            "MyFilms extended Database",
            "XBMC nfo reader"});
          this.CatalogType.Location = new System.Drawing.Point(533, 14);
          this.CatalogType.Name = "CatalogType";
          this.CatalogType.Size = new System.Drawing.Size(193, 21);
          this.CatalogType.TabIndex = 62;
          this.ToolTip1.SetToolTip(this.CatalogType, resources.GetString("CatalogType.ToolTip"));
          this.CatalogType.SelectedIndexChanged += new System.EventHandler(this.CatalogType_SelectedIndexChanged);
          // 
          // groupBox_TitleOrder
          // 
          this.groupBox_TitleOrder.Controls.Add(this.chkUseOriginalAsTranslatedTitle);
          this.groupBox_TitleOrder.Controls.Add(this.label20);
          this.groupBox_TitleOrder.Controls.Add(this.AntSTitle);
          this.groupBox_TitleOrder.Controls.Add(this.label17);
          this.groupBox_TitleOrder.Controls.Add(this.TitleDelim);
          this.groupBox_TitleOrder.Controls.Add(this.label9);
          this.groupBox_TitleOrder.Controls.Add(this.label8);
          this.groupBox_TitleOrder.Controls.Add(this.AntTitle2);
          this.groupBox_TitleOrder.Controls.Add(this.AntTitle1);
          this.groupBox_TitleOrder.Location = new System.Drawing.Point(7, 44);
          this.groupBox_TitleOrder.Name = "groupBox_TitleOrder";
          this.groupBox_TitleOrder.Size = new System.Drawing.Size(458, 101);
          this.groupBox_TitleOrder.TabIndex = 61;
          this.groupBox_TitleOrder.TabStop = false;
          this.groupBox_TitleOrder.Text = "Title Order";
          this.ToolTip1.SetToolTip(this.groupBox_TitleOrder, resources.GetString("groupBox_TitleOrder.ToolTip"));
          // 
          // chkUseOriginalAsTranslatedTitle
          // 
          this.chkUseOriginalAsTranslatedTitle.AutoSize = true;
          this.chkUseOriginalAsTranslatedTitle.Enabled = false;
          this.chkUseOriginalAsTranslatedTitle.Location = new System.Drawing.Point(297, 46);
          this.chkUseOriginalAsTranslatedTitle.Name = "chkUseOriginalAsTranslatedTitle";
          this.chkUseOriginalAsTranslatedTitle.Size = new System.Drawing.Size(103, 43);
          this.chkUseOriginalAsTranslatedTitle.TabIndex = 72;
          this.chkUseOriginalAsTranslatedTitle.Text = "Use original title \r\nif translated title \r\nis empty";
          this.ToolTip1.SetToolTip(this.chkUseOriginalAsTranslatedTitle, resources.GetString("chkUseOriginalAsTranslatedTitle.ToolTip"));
          this.chkUseOriginalAsTranslatedTitle.UseVisualStyleBackColor = true;
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
          // AntSTitle
          // 
          this.AntSTitle.FormattingEnabled = true;
          this.AntSTitle.Location = new System.Drawing.Point(109, 70);
          this.AntSTitle.Name = "AntSTitle";
          this.AntSTitle.Size = new System.Drawing.Size(178, 21);
          this.AntSTitle.TabIndex = 70;
          this.ToolTip1.SetToolTip(this.AntSTitle, "Select the ANT database field that you want to be used in the ‘Sort by name’ fiel" +
                  "d in the MediaPortal menu.");
          // 
          // label17
          // 
          this.label17.AutoSize = true;
          this.label17.Location = new System.Drawing.Point(294, 14);
          this.label17.Name = "label17";
          this.label17.Size = new System.Drawing.Size(101, 26);
          this.label17.TabIndex = 66;
          this.label17.Text = "Hierarchy Separator\r\n(Grouping)";
          this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
          this.ToolTip1.SetToolTip(this.label17, resources.GetString("label17.ToolTip"));
          // 
          // TitleDelim
          // 
          this.TitleDelim.Location = new System.Drawing.Point(406, 18);
          this.TitleDelim.MaxLength = 1;
          this.TitleDelim.Name = "TitleDelim";
          this.TitleDelim.Size = new System.Drawing.Size(20, 20);
          this.TitleDelim.TabIndex = 20;
          this.ToolTip1.SetToolTip(this.TitleDelim, resources.GetString("TitleDelim.ToolTip"));
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
          // AntTitle2
          // 
          this.AntTitle2.FormattingEnabled = true;
          this.AntTitle2.Location = new System.Drawing.Point(109, 43);
          this.AntTitle2.Name = "AntTitle2";
          this.AntTitle2.Size = new System.Drawing.Size(178, 21);
          this.AntTitle2.TabIndex = 11;
          this.ToolTip1.SetToolTip(this.AntTitle2, "Select the ANT database field that you want to be displayed as the ‘Alternate Tit" +
                  "le’.");
          // 
          // AntTitle1
          // 
          this.AntTitle1.BackColor = System.Drawing.SystemColors.Info;
          this.AntTitle1.FormattingEnabled = true;
          this.AntTitle1.Items.AddRange(new object[] {
            "OriginalTitle",
            "TranslatedTitle",
            "FormattedTitle"});
          this.AntTitle1.Location = new System.Drawing.Point(109, 16);
          this.AntTitle1.Name = "AntTitle1";
          this.AntTitle1.Size = new System.Drawing.Size(178, 21);
          this.AntTitle1.TabIndex = 10;
          this.ToolTip1.SetToolTip(this.AntTitle1, resources.GetString("AntTitle1.ToolTip"));
          this.AntTitle1.SelectedIndexChanged += new System.EventHandler(this.AntTitle1_SelectedIndexChanged);
          // 
          // ButCat
          // 
          this.ButCat.Location = new System.Drawing.Point(407, 15);
          this.ButCat.Name = "ButCat";
          this.ButCat.Size = new System.Drawing.Size(32, 20);
          this.ButCat.TabIndex = 49;
          this.ButCat.Text = "...";
          this.ButCat.UseVisualStyleBackColor = true;
          this.ButCat.Click += new System.EventHandler(this.ButCat_Click);
          // 
          // MesFilmsCat
          // 
          this.MesFilmsCat.BackColor = System.Drawing.SystemColors.Info;
          this.MesFilmsCat.Location = new System.Drawing.Point(116, 15);
          this.MesFilmsCat.Name = "MesFilmsCat";
          this.MesFilmsCat.Size = new System.Drawing.Size(285, 20);
          this.MesFilmsCat.TabIndex = 48;
          this.ToolTip1.SetToolTip(this.MesFilmsCat, "Enter the full path and name of your AMC xml database file.\r\nYou can use the brow" +
                  "se button to find the file");
          this.MesFilmsCat.TextChanged += new System.EventHandler(this.MesFilmsCat_TextChanged);
          this.MesFilmsCat.Leave += new System.EventHandler(this.MesFilmsCat_Leave);
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
          // butNew
          // 
          this.butNew.Location = new System.Drawing.Point(367, 466);
          this.butNew.Name = "butNew";
          this.butNew.Size = new System.Drawing.Size(64, 31);
          this.butNew.TabIndex = 79;
          this.butNew.Text = "New ...";
          this.ToolTip1.SetToolTip(this.butNew, "To create a new MyFilms configuration.");
          this.butNew.UseVisualStyleBackColor = true;
          this.butNew.Click += new System.EventHandler(this.butNew_Click);
          // 
          // groupBoxExtendedFieldHandling
          // 
          this.groupBoxExtendedFieldHandling.Controls.Add(this.ECMergeDestinationFieldWriter);
          this.groupBoxExtendedFieldHandling.Controls.Add(this.ECMergeDestinationFieldCertification);
          this.groupBoxExtendedFieldHandling.Controls.Add(this.ECMergeDestinationFieldTags);
          this.groupBoxExtendedFieldHandling.Controls.Add(this.chkAddWriter);
          this.groupBoxExtendedFieldHandling.Controls.Add(this.ECMergeDestinationFieldTagline);
          this.groupBoxExtendedFieldHandling.Controls.Add(this.chkAddTagline);
          this.groupBoxExtendedFieldHandling.Controls.Add(this.chkAddCertification);
          this.groupBoxExtendedFieldHandling.Controls.Add(this.label54);
          this.groupBoxExtendedFieldHandling.Controls.Add(this.chkAddTags);
          this.groupBoxExtendedFieldHandling.Controls.Add(this.label53);
          this.groupBoxExtendedFieldHandling.Location = new System.Drawing.Point(19, 17);
          this.groupBoxExtendedFieldHandling.Name = "groupBoxExtendedFieldHandling";
          this.groupBoxExtendedFieldHandling.Size = new System.Drawing.Size(355, 197);
          this.groupBoxExtendedFieldHandling.TabIndex = 30;
          this.groupBoxExtendedFieldHandling.TabStop = false;
          this.groupBoxExtendedFieldHandling.Text = "External Catalog Import Options ...";
          this.ToolTip1.SetToolTip(this.groupBoxExtendedFieldHandling, resources.GetString("groupBoxExtendedFieldHandling.ToolTip"));
          // 
          // ECMergeDestinationFieldWriter
          // 
          this.ECMergeDestinationFieldWriter.FormattingEnabled = true;
          this.ECMergeDestinationFieldWriter.Items.AddRange(new object[] {
            "Producer",
            "Director",
            "Borrower"});
          this.ECMergeDestinationFieldWriter.Location = new System.Drawing.Point(192, 138);
          this.ECMergeDestinationFieldWriter.Name = "ECMergeDestinationFieldWriter";
          this.ECMergeDestinationFieldWriter.Size = new System.Drawing.Size(134, 21);
          this.ECMergeDestinationFieldWriter.TabIndex = 7;
          this.ECMergeDestinationFieldWriter.Visible = false;
          // 
          // ECMergeDestinationFieldCertification
          // 
          this.ECMergeDestinationFieldCertification.FormattingEnabled = true;
          this.ECMergeDestinationFieldCertification.Items.AddRange(new object[] {
            "Description",
            "Comments"});
          this.ECMergeDestinationFieldCertification.Location = new System.Drawing.Point(192, 108);
          this.ECMergeDestinationFieldCertification.Name = "ECMergeDestinationFieldCertification";
          this.ECMergeDestinationFieldCertification.Size = new System.Drawing.Size(134, 21);
          this.ECMergeDestinationFieldCertification.TabIndex = 6;
          // 
          // ECMergeDestinationFieldTags
          // 
          this.ECMergeDestinationFieldTags.FormattingEnabled = true;
          this.ECMergeDestinationFieldTags.Items.AddRange(new object[] {
            "Description",
            "Comments"});
          this.ECMergeDestinationFieldTags.Location = new System.Drawing.Point(192, 78);
          this.ECMergeDestinationFieldTags.Name = "ECMergeDestinationFieldTags";
          this.ECMergeDestinationFieldTags.Size = new System.Drawing.Size(134, 21);
          this.ECMergeDestinationFieldTags.TabIndex = 5;
          // 
          // chkAddWriter
          // 
          this.chkAddWriter.AutoSize = true;
          this.chkAddWriter.Location = new System.Drawing.Point(39, 140);
          this.chkAddWriter.Name = "chkAddWriter";
          this.chkAddWriter.Size = new System.Drawing.Size(88, 17);
          this.chkAddWriter.TabIndex = 0;
          this.chkAddWriter.Text = "Add Writer ...";
          this.chkAddWriter.UseVisualStyleBackColor = true;
          this.chkAddWriter.Visible = false;
          // 
          // ECMergeDestinationFieldTagline
          // 
          this.ECMergeDestinationFieldTagline.FormattingEnabled = true;
          this.ECMergeDestinationFieldTagline.Items.AddRange(new object[] {
            "Description",
            "Comments"});
          this.ECMergeDestinationFieldTagline.Location = new System.Drawing.Point(192, 49);
          this.ECMergeDestinationFieldTagline.Name = "ECMergeDestinationFieldTagline";
          this.ECMergeDestinationFieldTagline.Size = new System.Drawing.Size(134, 21);
          this.ECMergeDestinationFieldTagline.TabIndex = 1;
          this.ToolTip1.SetToolTip(this.ECMergeDestinationFieldTagline, "Choose a MyFilms field, where you want to add data from \r\nexternal catalog fields" +
                  ".");
          // 
          // chkAddTagline
          // 
          this.chkAddTagline.AutoSize = true;
          this.chkAddTagline.Location = new System.Drawing.Point(39, 51);
          this.chkAddTagline.Name = "chkAddTagline";
          this.chkAddTagline.Size = new System.Drawing.Size(95, 17);
          this.chkAddTagline.TabIndex = 0;
          this.chkAddTagline.Text = "Add Tagline ...";
          this.chkAddTagline.UseVisualStyleBackColor = true;
          // 
          // chkAddCertification
          // 
          this.chkAddCertification.AutoSize = true;
          this.chkAddCertification.Location = new System.Drawing.Point(39, 110);
          this.chkAddCertification.Name = "chkAddCertification";
          this.chkAddCertification.Size = new System.Drawing.Size(115, 17);
          this.chkAddCertification.TabIndex = 0;
          this.chkAddCertification.Text = "Add Certification ...";
          this.chkAddCertification.UseVisualStyleBackColor = true;
          // 
          // label54
          // 
          this.label54.AutoSize = true;
          this.label54.Location = new System.Drawing.Point(194, 21);
          this.label54.Name = "label54";
          this.label54.Size = new System.Drawing.Size(125, 13);
          this.label54.TabIndex = 3;
          this.label54.Text = "MyFilms Destination Field";
          // 
          // chkAddTags
          // 
          this.chkAddTags.AutoSize = true;
          this.chkAddTags.Location = new System.Drawing.Point(39, 80);
          this.chkAddTags.Name = "chkAddTags";
          this.chkAddTags.Size = new System.Drawing.Size(84, 17);
          this.chkAddTags.TabIndex = 4;
          this.chkAddTags.Text = "Add Tags ...";
          this.chkAddTags.UseVisualStyleBackColor = true;
          // 
          // label53
          // 
          this.label53.AutoSize = true;
          this.label53.Location = new System.Drawing.Point(16, 21);
          this.label53.Name = "label53";
          this.label53.Size = new System.Drawing.Size(146, 13);
          this.label53.TabIndex = 2;
          this.label53.Text = "External Catalog Source Field";
          // 
          // btnLaunchAMCglobal
          // 
          this.btnLaunchAMCglobal.Image = ((System.Drawing.Image)(resources.GetObject("btnLaunchAMCglobal.Image")));
          this.btnLaunchAMCglobal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
          this.btnLaunchAMCglobal.Location = new System.Drawing.Point(45, 465);
          this.btnLaunchAMCglobal.Name = "btnLaunchAMCglobal";
          this.btnLaunchAMCglobal.Size = new System.Drawing.Size(152, 31);
          this.btnLaunchAMCglobal.TabIndex = 77;
          this.btnLaunchAMCglobal.Text = "Launch AMC Updater";
          this.btnLaunchAMCglobal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
          this.ToolTip1.SetToolTip(this.btnLaunchAMCglobal, resources.GetString("btnLaunchAMCglobal.ToolTip"));
          this.btnLaunchAMCglobal.UseVisualStyleBackColor = true;
          this.btnLaunchAMCglobal.Click += new System.EventHandler(this.btnLaunchAMCglobal_Click);
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
          this.ToolTip1.SetToolTip(this.SPicture, "Click here to open file dialog and select image for logo");
          this.SPicture.Click += new System.EventHandler(this.pictureBox_Click);
          // 
          // groupBox_DVDprofiler
          // 
          this.groupBox_DVDprofiler.Controls.Add(this.groupBox13);
          this.groupBox_DVDprofiler.Controls.Add(this.groupBox9);
          this.groupBox_DVDprofiler.Location = new System.Drawing.Point(392, 17);
          this.groupBox_DVDprofiler.Name = "groupBox_DVDprofiler";
          this.groupBox_DVDprofiler.Size = new System.Drawing.Size(335, 197);
          this.groupBox_DVDprofiler.TabIndex = 28;
          this.groupBox_DVDprofiler.TabStop = false;
          this.groupBox_DVDprofiler.Text = "DVDProfiler";
          this.ToolTip1.SetToolTip(this.groupBox_DVDprofiler, "Those setting are kept to keep backward compatibility to older MyFilms versions.\r" +
                  "\nThey only apply, if the catalog type you selected is \"DVDProfiler\".");
          // 
          // groupBox13
          // 
          this.groupBox13.Controls.Add(this.chkDVDprofilerOnlyFile);
          this.groupBox13.Location = new System.Drawing.Point(13, 19);
          this.groupBox13.Name = "groupBox13";
          this.groupBox13.Size = new System.Drawing.Size(307, 49);
          this.groupBox13.TabIndex = 2;
          this.groupBox13.TabStop = false;
          this.groupBox13.Text = "Map Notes field to Source";
          // 
          // chkDVDprofilerOnlyFile
          // 
          this.chkDVDprofilerOnlyFile.AutoSize = true;
          this.chkDVDprofilerOnlyFile.Location = new System.Drawing.Point(24, 21);
          this.chkDVDprofilerOnlyFile.Name = "chkDVDprofilerOnlyFile";
          this.chkDVDprofilerOnlyFile.Size = new System.Drawing.Size(221, 17);
          this.chkDVDprofilerOnlyFile.TabIndex = 0;
          this.chkDVDprofilerOnlyFile.Text = "Notes Field contains only Movie Filename";
          this.chkDVDprofilerOnlyFile.UseVisualStyleBackColor = true;
          // 
          // buttonDeleteTmpCatalog
          // 
          this.buttonDeleteTmpCatalog.Location = new System.Drawing.Point(29, 302);
          this.buttonDeleteTmpCatalog.Name = "buttonDeleteTmpCatalog";
          this.buttonDeleteTmpCatalog.Size = new System.Drawing.Size(152, 27);
          this.buttonDeleteTmpCatalog.TabIndex = 8;
          this.buttonDeleteTmpCatalog.Text = "Delete tmp Catalog";
          this.ToolTip1.SetToolTip(this.buttonDeleteTmpCatalog, resources.GetString("buttonDeleteTmpCatalog.ToolTip"));
          this.buttonDeleteTmpCatalog.UseVisualStyleBackColor = true;
          this.buttonDeleteTmpCatalog.Click += new System.EventHandler(this.buttonDeleteTmpCatalog_Click);
          // 
          // buttonOpenTmpFileAMC
          // 
          this.buttonOpenTmpFileAMC.Location = new System.Drawing.Point(29, 236);
          this.buttonOpenTmpFileAMC.Name = "buttonOpenTmpFileAMC";
          this.buttonOpenTmpFileAMC.Size = new System.Drawing.Size(152, 27);
          this.buttonOpenTmpFileAMC.TabIndex = 32;
          this.buttonOpenTmpFileAMC.Text = "Open tmp Catalog in AMC";
          this.ToolTip1.SetToolTip(this.buttonOpenTmpFileAMC, resources.GetString("buttonOpenTmpFileAMC.ToolTip"));
          this.buttonOpenTmpFileAMC.UseVisualStyleBackColor = true;
          this.buttonOpenTmpFileAMC.Click += new System.EventHandler(this.buttonOpenTmpFileAMC_Click);
          // 
          // AMCConfigView
          // 
          this.AMCConfigView.AllowColumnReorder = true;
          this.AMCConfigView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Option,
            this.Value});
          this.AMCConfigView.FullRowSelect = true;
          this.AMCConfigView.GridLines = true;
          this.AMCConfigView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
          this.AMCConfigView.Location = new System.Drawing.Point(20, 73);
          this.AMCConfigView.MultiSelect = false;
          this.AMCConfigView.Name = "AMCConfigView";
          this.AMCConfigView.Size = new System.Drawing.Size(413, 263);
          this.AMCConfigView.TabIndex = 89;
          this.ToolTip1.SetToolTip(this.AMCConfigView, resources.GetString("AMCConfigView.ToolTip"));
          this.AMCConfigView.UseCompatibleStateImageBehavior = false;
          this.AMCConfigView.View = System.Windows.Forms.View.Details;
          // 
          // Option
          // 
          this.Option.Text = "Option";
          this.Option.Width = 171;
          // 
          // Value
          // 
          this.Value.Text = "Value";
          this.Value.Width = 515;
          // 
          // cbAllowRecentAddedAPI
          // 
          this.cbAllowRecentAddedAPI.AutoSize = true;
          this.cbAllowRecentAddedAPI.Location = new System.Drawing.Point(15, 21);
          this.cbAllowRecentAddedAPI.Name = "cbAllowRecentAddedAPI";
          this.cbAllowRecentAddedAPI.Size = new System.Drawing.Size(167, 17);
          this.cbAllowRecentAddedAPI.TabIndex = 4;
          this.cbAllowRecentAddedAPI.Text = "Enable \"Recently added\" API";
          this.ToolTip1.SetToolTip(this.cbAllowRecentAddedAPI, "If checked, this config will be included when external plugins ask for \"recenty a" +
                  "dded\" movies.\r\nYou can enable this for multiple configs.");
          this.cbAllowRecentAddedAPI.UseVisualStyleBackColor = true;
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
          // ButSave
          // 
          this.ButSave.Location = new System.Drawing.Point(442, 466);
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
          // Tab_ExternalCatalogs
          // 
          this.Tab_ExternalCatalogs.Controls.Add(this.groupBoxAMCsettings);
          this.Tab_ExternalCatalogs.Controls.Add(this.buttonOpenTmpFileAMC);
          this.Tab_ExternalCatalogs.Controls.Add(this.buttonOpenTmpFile);
          this.Tab_ExternalCatalogs.Controls.Add(this.buttonDeleteTmpCatalog);
          this.Tab_ExternalCatalogs.Controls.Add(this.groupBoxExtendedFieldHandling);
          this.Tab_ExternalCatalogs.Controls.Add(this.groupBox_DVDprofiler);
          this.Tab_ExternalCatalogs.Location = new System.Drawing.Point(4, 22);
          this.Tab_ExternalCatalogs.Name = "Tab_ExternalCatalogs";
          this.Tab_ExternalCatalogs.Padding = new System.Windows.Forms.Padding(3);
          this.Tab_ExternalCatalogs.Size = new System.Drawing.Size(747, 354);
          this.Tab_ExternalCatalogs.TabIndex = 2;
          this.Tab_ExternalCatalogs.Text = "External Catalogs";
          this.Tab_ExternalCatalogs.ToolTipText = "Options for external catalogs (only applies if not using ANT movie catalog)";
          this.Tab_ExternalCatalogs.UseVisualStyleBackColor = true;
          // 
          // groupBoxAMCsettings
          // 
          this.groupBoxAMCsettings.Controls.Add(this.AMCexePath);
          this.groupBoxAMCsettings.Controls.Add(this.lblAMCpath);
          this.groupBoxAMCsettings.Controls.Add(this.buttonAMCpathSearch);
          this.groupBoxAMCsettings.Location = new System.Drawing.Point(392, 236);
          this.groupBoxAMCsettings.Name = "groupBoxAMCsettings";
          this.groupBoxAMCsettings.Size = new System.Drawing.Size(335, 93);
          this.groupBoxAMCsettings.TabIndex = 36;
          this.groupBoxAMCsettings.TabStop = false;
          this.groupBoxAMCsettings.Text = "Ant Movie Catalog (AMC)";
          // 
          // AMCexePath
          // 
          this.AMCexePath.Location = new System.Drawing.Point(14, 52);
          this.AMCexePath.Name = "AMCexePath";
          this.AMCexePath.Size = new System.Drawing.Size(261, 20);
          this.AMCexePath.TabIndex = 34;
          // 
          // lblAMCpath
          // 
          this.lblAMCpath.AutoSize = true;
          this.lblAMCpath.Location = new System.Drawing.Point(11, 33);
          this.lblAMCpath.Name = "lblAMCpath";
          this.lblAMCpath.Size = new System.Drawing.Size(213, 13);
          this.lblAMCpath.TabIndex = 33;
          this.lblAMCpath.Text = "Path to AMC Ant Movie Catalog Executable";
          // 
          // buttonAMCpathSearch
          // 
          this.buttonAMCpathSearch.Location = new System.Drawing.Point(281, 52);
          this.buttonAMCpathSearch.Name = "buttonAMCpathSearch";
          this.buttonAMCpathSearch.Size = new System.Drawing.Size(36, 20);
          this.buttonAMCpathSearch.TabIndex = 35;
          this.buttonAMCpathSearch.Text = "...";
          this.buttonAMCpathSearch.UseVisualStyleBackColor = true;
          this.buttonAMCpathSearch.Click += new System.EventHandler(this.buttonAMCpathSearch_Click);
          // 
          // buttonOpenTmpFile
          // 
          this.buttonOpenTmpFile.Location = new System.Drawing.Point(29, 269);
          this.buttonOpenTmpFile.Name = "buttonOpenTmpFile";
          this.buttonOpenTmpFile.Size = new System.Drawing.Size(152, 27);
          this.buttonOpenTmpFile.TabIndex = 31;
          this.buttonOpenTmpFile.Text = "Open tmp Catalog in Editor";
          this.buttonOpenTmpFile.UseVisualStyleBackColor = true;
          this.buttonOpenTmpFile.Click += new System.EventHandler(this.buttonOpenTmpFile_Click);
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
          this.Tab_WakeOnLan.ToolTipText = "Setup for WakeOnLan features";
          this.Tab_WakeOnLan.UseVisualStyleBackColor = true;
          // 
          // Tab_Logos
          // 
          this.Tab_Logos.Controls.Add(this.btnUpdate);
          this.Tab_Logos.Controls.Add(this.btnLogoClearCache);
          this.Tab_Logos.Controls.Add(this.lblLogoPresets);
          this.Tab_Logos.Controls.Add(this.lbl_LogoSpacing);
          this.Tab_Logos.Controls.Add(this.comboBoxLogoPresets);
          this.Tab_Logos.Controls.Add(this.comboBoxLogoSpacing);
          this.Tab_Logos.Controls.Add(this.lblSetupLogoRules);
          this.Tab_Logos.Controls.Add(this.lblInfoLogosForAll);
          this.Tab_Logos.Controls.Add(this.lblSelectLogoFile);
          this.Tab_Logos.Controls.Add(this.btnLogosPath);
          this.Tab_Logos.Controls.Add(this.txtLogosPath);
          this.Tab_Logos.Controls.Add(this.SFilePicture);
          this.Tab_Logos.Controls.Add(this.lblLogosPath);
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
          this.Tab_Logos.Controls.Add(this.chkLogos);
          this.Tab_Logos.Controls.Add(this.SLogo_Type);
          this.Tab_Logos.Controls.Add(this.SPicture);
          this.Tab_Logos.Location = new System.Drawing.Point(4, 22);
          this.Tab_Logos.Name = "Tab_Logos";
          this.Tab_Logos.Padding = new System.Windows.Forms.Padding(3);
          this.Tab_Logos.Size = new System.Drawing.Size(747, 354);
          this.Tab_Logos.TabIndex = 6;
          this.Tab_Logos.Text = "Logos";
          this.Tab_Logos.ToolTipText = "Setup for logos";
          this.Tab_Logos.UseVisualStyleBackColor = true;
          // 
          // btnUpdate
          // 
          this.btnUpdate.Location = new System.Drawing.Point(583, 331);
          this.btnUpdate.Name = "btnUpdate";
          this.btnUpdate.Size = new System.Drawing.Size(144, 21);
          this.btnUpdate.TabIndex = 103;
          this.btnUpdate.Text = "Update Logo Rule";
          this.btnUpdate.UseVisualStyleBackColor = true;
          this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
          // 
          // lblLogoPresets
          // 
          this.lblLogoPresets.AutoSize = true;
          this.lblLogoPresets.Location = new System.Drawing.Point(108, 11);
          this.lblLogoPresets.Name = "lblLogoPresets";
          this.lblLogoPresets.Size = new System.Drawing.Size(42, 13);
          this.lblLogoPresets.TabIndex = 4;
          this.lblLogoPresets.Text = "Presets";
          // 
          // lbl_LogoSpacing
          // 
          this.lbl_LogoSpacing.AutoSize = true;
          this.lbl_LogoSpacing.Location = new System.Drawing.Point(592, 11);
          this.lbl_LogoSpacing.Name = "lbl_LogoSpacing";
          this.lbl_LogoSpacing.Size = new System.Drawing.Size(46, 13);
          this.lbl_LogoSpacing.TabIndex = 101;
          this.lbl_LogoSpacing.Text = "Spacing";
          // 
          // lblSetupLogoRules
          // 
          this.lblSetupLogoRules.AutoSize = true;
          this.lblSetupLogoRules.Location = new System.Drawing.Point(6, 253);
          this.lblSetupLogoRules.Name = "lblSetupLogoRules";
          this.lblSetupLogoRules.Size = new System.Drawing.Size(333, 13);
          this.lblSetupLogoRules.TabIndex = 98;
          this.lblSetupLogoRules.Text = "Set conditional Logo rules, select Logo Image and add rule to ruleset:";
          // 
          // lblInfoLogosForAll
          // 
          this.lblInfoLogosForAll.AutoSize = true;
          this.lblInfoLogosForAll.BackColor = System.Drawing.SystemColors.Control;
          this.lblInfoLogosForAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
          this.lblInfoLogosForAll.ForeColor = System.Drawing.SystemColors.ControlText;
          this.lblInfoLogosForAll.Location = new System.Drawing.Point(6, 334);
          this.lblInfoLogosForAll.Name = "lblInfoLogosForAll";
          this.lblInfoLogosForAll.Size = new System.Drawing.Size(297, 13);
          this.lblInfoLogosForAll.TabIndex = 96;
          this.lblInfoLogosForAll.Text = "(The Logo Configuration is used for all MyFilms configurations)";
          // 
          // lblSelectLogoFile
          // 
          this.lblSelectLogoFile.AutoSize = true;
          this.lblSelectLogoFile.Location = new System.Drawing.Point(486, 253);
          this.lblSelectLogoFile.Name = "lblSelectLogoFile";
          this.lblSelectLogoFile.Size = new System.Drawing.Size(100, 13);
          this.lblSelectLogoFile.TabIndex = 95;
          this.lblSelectLogoFile.Text = "Click to select Logo";
          // 
          // btnLogosPath
          // 
          this.btnLogosPath.Enabled = false;
          this.btnLogosPath.Location = new System.Drawing.Point(695, 33);
          this.btnLogosPath.Name = "btnLogosPath";
          this.btnLogosPath.Size = new System.Drawing.Size(32, 20);
          this.btnLogosPath.TabIndex = 94;
          this.btnLogosPath.Text = "...";
          this.btnLogosPath.UseVisualStyleBackColor = true;
          this.btnLogosPath.Click += new System.EventHandler(this.btnLogosPath_Click);
          // 
          // SFilePicture
          // 
          this.SFilePicture.BackColor = System.Drawing.SystemColors.Control;
          this.SFilePicture.Location = new System.Drawing.Point(7, 230);
          this.SFilePicture.Name = "SFilePicture";
          this.SFilePicture.Size = new System.Drawing.Size(734, 20);
          this.SFilePicture.TabIndex = 91;
          // 
          // lblLogosPath
          // 
          this.lblLogosPath.AutoSize = true;
          this.lblLogosPath.Location = new System.Drawing.Point(108, 36);
          this.lblLogosPath.Name = "lblLogosPath";
          this.lblLogosPath.Size = new System.Drawing.Size(106, 13);
          this.lblLogosPath.TabIndex = 93;
          this.lblLogosPath.Text = "Searchpath to Logos";
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
          this.Tab_Artwork.ToolTipText = "Setup for artwork, like pathes to coverart, person images etc.";
          this.Tab_Artwork.UseVisualStyleBackColor = true;
          // 
          // Tab_AMCupdater
          // 
          this.Tab_AMCupdater.Controls.Add(this.groupBox_AMCupdater);
          this.Tab_AMCupdater.Controls.Add(this.groupBox_GrabberOptions);
          this.Tab_AMCupdater.Location = new System.Drawing.Point(4, 22);
          this.Tab_AMCupdater.Name = "Tab_AMCupdater";
          this.Tab_AMCupdater.Padding = new System.Windows.Forms.Padding(3);
          this.Tab_AMCupdater.Size = new System.Drawing.Size(747, 354);
          this.Tab_AMCupdater.TabIndex = 4;
          this.Tab_AMCupdater.Text = "Internet Grabbing";
          this.Tab_AMCupdater.ToolTipText = "Setup for internet updates and basic configuration for AMC Updater";
          this.Tab_AMCupdater.UseVisualStyleBackColor = true;
          // 
          // Tab_Update
          // 
          this.Tab_Update.Controls.Add(this.chkSuppressManual);
          this.Tab_Update.Controls.Add(this.groupBox_AntUpdatingItems);
          this.Tab_Update.Controls.Add(this.groupBox2);
          this.Tab_Update.Controls.Add(this.groupBoxUpdateByProperties);
          this.Tab_Update.Controls.Add(this.groupBoxDeletionOptions);
          this.Tab_Update.Location = new System.Drawing.Point(4, 22);
          this.Tab_Update.Name = "Tab_Update";
          this.Tab_Update.Padding = new System.Windows.Forms.Padding(3);
          this.Tab_Update.Size = new System.Drawing.Size(747, 354);
          this.Tab_Update.TabIndex = 3;
          this.Tab_Update.Text = "Update";
          this.Tab_Update.ToolTipText = "Setup for update options (updates to values from GUI)";
          this.Tab_Update.UseVisualStyleBackColor = true;
          // 
          // General
          // 
          this.General.Controls.Add(this.Tab_General);
          this.General.Controls.Add(this.Tab_Trailer);
          this.General.Controls.Add(this.Tab_Views);
          this.General.Controls.Add(this.Tab_Search);
          this.General.Controls.Add(this.Tab_Update);
          this.General.Controls.Add(this.Tab_AMCupdater);
          this.General.Controls.Add(this.Tab_Artwork);
          this.General.Controls.Add(this.Tab_Logos);
          this.General.Controls.Add(this.Tab_WakeOnLan);
          this.General.Controls.Add(this.Tab_ExternalCatalogs);
          this.General.Controls.Add(this.Tab_Trakt);
          this.General.Controls.Add(this.Tab_Other);
          this.General.Location = new System.Drawing.Point(12, 78);
          this.General.Name = "General";
          this.General.SelectedIndex = 0;
          this.General.Size = new System.Drawing.Size(755, 380);
          this.General.TabIndex = 46;
          this.General.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.General_Selected);
          // 
          // Tab_Other
          // 
          this.Tab_Other.Controls.Add(this.groupBox3);
          this.Tab_Other.Controls.Add(this.groupBoxExternal);
          this.Tab_Other.Controls.Add(this.btnGrabberInterface);
          this.Tab_Other.Controls.Add(this.lblAMCupdaterConfigPreview);
          this.Tab_Other.Controls.Add(this.AMCConfigView);
          this.Tab_Other.Location = new System.Drawing.Point(4, 22);
          this.Tab_Other.Name = "Tab_Other";
          this.Tab_Other.Padding = new System.Windows.Forms.Padding(3);
          this.Tab_Other.Size = new System.Drawing.Size(747, 354);
          this.Tab_Other.TabIndex = 11;
          this.Tab_Other.Text = "Other";
          this.Tab_Other.UseVisualStyleBackColor = true;
          // 
          // groupBox3
          // 
          this.groupBox3.Controls.Add(this.tbEditorStartParamsOutput);
          this.groupBox3.Controls.Add(this.btnSaveEditorStartParams);
          this.groupBox3.Controls.Add(this.label60);
          this.groupBox3.Controls.Add(this.tbEditorSearchExpression);
          this.groupBox3.Controls.Add(this.label59);
          this.groupBox3.Controls.Add(this.cbEditorLayout);
          this.groupBox3.Controls.Add(this.label58);
          this.groupBox3.Controls.Add(this.label57);
          this.groupBox3.Controls.Add(this.label52);
          this.groupBox3.Controls.Add(this.btnLoadEditorValues);
          this.groupBox3.Controls.Add(this.cbEditorViews);
          this.groupBox3.Controls.Add(this.cbEditorConfigs);
          this.groupBox3.Controls.Add(this.cbEditorViewValues);
          this.groupBox3.Location = new System.Drawing.Point(444, 116);
          this.groupBox3.Name = "groupBox3";
          this.groupBox3.Size = new System.Drawing.Size(288, 220);
          this.groupBox3.TabIndex = 93;
          this.groupBox3.TabStop = false;
          this.groupBox3.Text = "Basic Home Editor API test ...";
          // 
          // tbEditorStartParamsOutput
          // 
          this.tbEditorStartParamsOutput.Location = new System.Drawing.Point(15, 183);
          this.tbEditorStartParamsOutput.Name = "tbEditorStartParamsOutput";
          this.tbEditorStartParamsOutput.Size = new System.Drawing.Size(253, 20);
          this.tbEditorStartParamsOutput.TabIndex = 104;
          // 
          // btnSaveEditorStartParams
          // 
          this.btnSaveEditorStartParams.Location = new System.Drawing.Point(168, 23);
          this.btnSaveEditorStartParams.Name = "btnSaveEditorStartParams";
          this.btnSaveEditorStartParams.Size = new System.Drawing.Size(100, 23);
          this.btnSaveEditorStartParams.TabIndex = 103;
          this.btnSaveEditorStartParams.Text = "Save StartParams";
          this.btnSaveEditorStartParams.UseVisualStyleBackColor = true;
          this.btnSaveEditorStartParams.Click += new System.EventHandler(this.btnSaveEditorStartParams_Click);
          // 
          // label60
          // 
          this.label60.AutoSize = true;
          this.label60.Location = new System.Drawing.Point(170, 131);
          this.label60.Name = "label60";
          this.label60.Size = new System.Drawing.Size(95, 13);
          this.label60.TabIndex = 102;
          this.label60.Text = "Search Expression";
          // 
          // tbEditorSearchExpression
          // 
          this.tbEditorSearchExpression.Location = new System.Drawing.Point(168, 147);
          this.tbEditorSearchExpression.Name = "tbEditorSearchExpression";
          this.tbEditorSearchExpression.Size = new System.Drawing.Size(100, 20);
          this.tbEditorSearchExpression.TabIndex = 101;
          // 
          // label59
          // 
          this.label59.AutoSize = true;
          this.label59.Location = new System.Drawing.Point(184, 51);
          this.label59.Name = "label59";
          this.label59.Size = new System.Drawing.Size(39, 13);
          this.label59.TabIndex = 100;
          this.label59.Text = "Layout";
          // 
          // cbEditorLayout
          // 
          this.cbEditorLayout.FormattingEnabled = true;
          this.cbEditorLayout.Items.AddRange(new object[] {
            "",
            "List View",
            "Big Thumbs",
            "Small Thumbs",
            "Filmstrip",
            "Coverflow"});
          this.cbEditorLayout.Location = new System.Drawing.Point(182, 67);
          this.cbEditorLayout.Name = "cbEditorLayout";
          this.cbEditorLayout.Size = new System.Drawing.Size(77, 21);
          this.cbEditorLayout.TabIndex = 99;
          this.cbEditorLayout.SelectedIndexChanged += new System.EventHandler(this.cbEditorLayout_SelectedIndexChanged);
          // 
          // label58
          // 
          this.label58.AutoSize = true;
          this.label58.Location = new System.Drawing.Point(38, 51);
          this.label58.Name = "label58";
          this.label58.Size = new System.Drawing.Size(82, 13);
          this.label58.TabIndex = 98;
          this.label58.Text = "Config selection";
          // 
          // label57
          // 
          this.label57.AutoSize = true;
          this.label57.Location = new System.Drawing.Point(38, 91);
          this.label57.Name = "label57";
          this.label57.Size = new System.Drawing.Size(77, 13);
          this.label57.TabIndex = 97;
          this.label57.Text = "View Selection";
          // 
          // label52
          // 
          this.label52.AutoSize = true;
          this.label52.Location = new System.Drawing.Point(38, 131);
          this.label52.Name = "label52";
          this.label52.Size = new System.Drawing.Size(104, 13);
          this.label52.TabIndex = 96;
          this.label52.Text = "Value to filter movies";
          // 
          // btnLoadEditorValues
          // 
          this.btnLoadEditorValues.Location = new System.Drawing.Point(36, 23);
          this.btnLoadEditorValues.Name = "btnLoadEditorValues";
          this.btnLoadEditorValues.Size = new System.Drawing.Size(121, 23);
          this.btnLoadEditorValues.TabIndex = 93;
          this.btnLoadEditorValues.Text = "Load MyFilms Data";
          this.btnLoadEditorValues.UseVisualStyleBackColor = true;
          this.btnLoadEditorValues.Click += new System.EventHandler(this.btnLoadEditorValues_Click);
          // 
          // cbEditorViews
          // 
          this.cbEditorViews.FormattingEnabled = true;
          this.cbEditorViews.Location = new System.Drawing.Point(36, 107);
          this.cbEditorViews.Name = "cbEditorViews";
          this.cbEditorViews.Size = new System.Drawing.Size(121, 21);
          this.cbEditorViews.TabIndex = 95;
          this.cbEditorViews.SelectedIndexChanged += new System.EventHandler(this.cbEditorViews_SelectedIndexChanged);
          // 
          // cbEditorConfigs
          // 
          this.cbEditorConfigs.FormattingEnabled = true;
          this.cbEditorConfigs.Location = new System.Drawing.Point(36, 67);
          this.cbEditorConfigs.Name = "cbEditorConfigs";
          this.cbEditorConfigs.Size = new System.Drawing.Size(121, 21);
          this.cbEditorConfigs.TabIndex = 94;
          this.cbEditorConfigs.SelectedIndexChanged += new System.EventHandler(this.cbEditorConfigs_SelectedIndexChanged);
          // 
          // cbEditorViewValues
          // 
          this.cbEditorViewValues.FormattingEnabled = true;
          this.cbEditorViewValues.Location = new System.Drawing.Point(36, 147);
          this.cbEditorViewValues.Name = "cbEditorViewValues";
          this.cbEditorViewValues.Size = new System.Drawing.Size(121, 21);
          this.cbEditorViewValues.TabIndex = 6;
          // 
          // groupBoxExternal
          // 
          this.groupBoxExternal.Controls.Add(this.labelRecentlyAddedAPI);
          this.groupBoxExternal.Controls.Add(this.cbAllowRecentAddedAPI);
          this.groupBoxExternal.Location = new System.Drawing.Point(444, 12);
          this.groupBoxExternal.Name = "groupBoxExternal";
          this.groupBoxExternal.Size = new System.Drawing.Size(288, 89);
          this.groupBoxExternal.TabIndex = 92;
          this.groupBoxExternal.TabStop = false;
          this.groupBoxExternal.Text = "Recentrly Added API ...";
          // 
          // labelRecentlyAddedAPI
          // 
          this.labelRecentlyAddedAPI.AutoSize = true;
          this.labelRecentlyAddedAPI.Location = new System.Drawing.Point(16, 45);
          this.labelRecentlyAddedAPI.Name = "labelRecentlyAddedAPI";
          this.labelRecentlyAddedAPI.Size = new System.Drawing.Size(252, 26);
          this.labelRecentlyAddedAPI.TabIndex = 5;
          this.labelRecentlyAddedAPI.Text = "You can enable or disable per config, if you want to \r\ninclude the catalog to the" +
              " recently added API.";
          // 
          // btnGrabberInterface
          // 
          this.btnGrabberInterface.Location = new System.Drawing.Point(29, 12);
          this.btnGrabberInterface.Name = "btnGrabberInterface";
          this.btnGrabberInterface.Size = new System.Drawing.Size(152, 27);
          this.btnGrabberInterface.TabIndex = 91;
          this.btnGrabberInterface.Text = "Grabber Interface";
          this.btnGrabberInterface.UseVisualStyleBackColor = true;
          this.btnGrabberInterface.Click += new System.EventHandler(this.btnGrabberInterface_Click);
          // 
          // lblAMCupdaterConfigPreview
          // 
          this.lblAMCupdaterConfigPreview.AutoSize = true;
          this.lblAMCupdaterConfigPreview.Location = new System.Drawing.Point(25, 57);
          this.lblAMCupdaterConfigPreview.Name = "lblAMCupdaterConfigPreview";
          this.lblAMCupdaterConfigPreview.Size = new System.Drawing.Size(225, 13);
          this.lblAMCupdaterConfigPreview.TabIndex = 90;
          this.lblAMCupdaterConfigPreview.Text = "Preview of current AMC Updater Configuration";
          // 
          // textBoxNBconfigs
          // 
          this.textBoxNBconfigs.Enabled = false;
          this.textBoxNBconfigs.Location = new System.Drawing.Point(421, 39);
          this.textBoxNBconfigs.Name = "textBoxNBconfigs";
          this.textBoxNBconfigs.Size = new System.Drawing.Size(28, 20);
          this.textBoxNBconfigs.TabIndex = 80;
          this.textBoxNBconfigs.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
          // 
          // lblNbConfig
          // 
          this.lblNbConfig.AutoSize = true;
          this.lblNbConfig.Enabled = false;
          this.lblNbConfig.Location = new System.Drawing.Point(373, 42);
          this.lblNbConfig.Name = "lblNbConfig";
          this.lblNbConfig.Size = new System.Drawing.Size(42, 13);
          this.lblNbConfig.TabIndex = 81;
          this.lblNbConfig.Text = "Configs";
          // 
          // pictureBoxMyFilms
          // 
          this.pictureBoxMyFilms.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxMyFilms.Image")));
          this.pictureBoxMyFilms.Location = new System.Drawing.Point(12, 7);
          this.pictureBoxMyFilms.Name = "pictureBoxMyFilms";
          this.pictureBoxMyFilms.Size = new System.Drawing.Size(62, 65);
          this.pictureBoxMyFilms.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
          this.pictureBoxMyFilms.TabIndex = 75;
          this.pictureBoxMyFilms.TabStop = false;
          // 
          // chkSuppressManual
          // 
          this.chkSuppressManual.AutoSize = true;
          this.chkSuppressManual.Location = new System.Drawing.Point(393, 10);
          this.chkSuppressManual.Name = "chkSuppressManual";
          this.chkSuppressManual.Size = new System.Drawing.Size(216, 17);
          this.chkSuppressManual.TabIndex = 19;
          this.chkSuppressManual.Text = "Enable manual deletion / removal option";
          this.ToolTip1.SetToolTip(this.chkSuppressManual, "Enables an entry in context menu to manually delete DB record and/or media files." +
                  "");
          this.chkSuppressManual.UseVisualStyleBackColor = true;
          // 
          // label61
          // 
          this.label61.AutoSize = true;
          this.label61.Location = new System.Drawing.Point(169, 40);
          this.label61.Name = "label61";
          this.label61.Size = new System.Drawing.Size(72, 13);
          this.label61.TabIndex = 21;
          this.label61.Text = "Update Value";
          // 
          // MyFilmsSetup
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(779, 506);
          this.Controls.Add(this.lblNbConfig);
          this.Controls.Add(this.textBoxNBconfigs);
          this.Controls.Add(this.butNew);
          this.Controls.Add(this.linkLabelMyFilmsWiki);
          this.Controls.Add(this.btnLaunchAMCglobal);
          this.Controls.Add(this.ButCopy);
          this.Controls.Add(this.pictureBoxMyFilms);
          this.Controls.Add(this.Config_Menu);
          this.Controls.Add(this.ButSave);
          this.Controls.Add(this.Config_Dflt);
          this.Controls.Add(this.ButDelet);
          this.Controls.Add(this.label12);
          this.Controls.Add(this.Config_Name);
          this.Controls.Add(this.label11);
          this.Controls.Add(this.textBoxPluginName);
          this.Controls.Add(this.label_VersionNumber);
          this.Controls.Add(this.General);
          this.Controls.Add(this.ButQuit);
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
          this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
          this.Name = "MyFilmsSetup";
          this.Text = "MyFilms Setup";
          this.Load += new System.EventHandler(this.MesFilmsSetup_Load);
          this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MesFilmsSetup_Quit);
          this.Tab_Trakt.ResumeLayout(false);
          this.groupBoxTrakt.ResumeLayout(false);
          this.groupBoxTrakt.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
          this.groupBox9.ResumeLayout(false);
          this.groupBox9.PerformLayout();
          this.groupBox25.ResumeLayout(false);
          this.groupBox25.PerformLayout();
          this.Fanart.ResumeLayout(false);
          this.Fanart.PerformLayout();
          this.groupBox22.ResumeLayout(false);
          this.groupBox22.PerformLayout();
          this.groupBox1.ResumeLayout(false);
          this.groupBox1.PerformLayout();
          this.groupBox_ArtistImages.ResumeLayout(false);
          this.groupBox_ArtistImages.PerformLayout();
          this.groupBox_GrabberOptions.ResumeLayout(false);
          this.groupBox_GrabberOptions.PerformLayout();
          this.groupBox6.ResumeLayout(false);
          this.groupBox6.PerformLayout();
          this.groupBox_AMCupdater.ResumeLayout(false);
          this.groupBox_AMCupdater.PerformLayout();
          this.groupBox_AMCupdaterScheduer.ResumeLayout(false);
          this.groupBox_AMCupdaterScheduer.PerformLayout();
          this.groupBox_AMCupdater_ExternalApplication.ResumeLayout(false);
          this.groupBox_AMCupdater_ExternalApplication.PerformLayout();
          this.groupBoxDeletionOptions.ResumeLayout(false);
          this.groupBoxDeletionOptions.PerformLayout();
          this.gpsuppress.ResumeLayout(false);
          this.gpsuppress.PerformLayout();
          this.gpspfield.ResumeLayout(false);
          this.gpspfield.PerformLayout();
          this.groupBoxUpdateByProperties.ResumeLayout(false);
          this.groupBoxUpdateByProperties.PerformLayout();
          this.groupBox2.ResumeLayout(false);
          this.groupBox2.PerformLayout();
          this.groupBox_AntUpdatingItems.ResumeLayout(false);
          this.groupBox_AntUpdatingItems.PerformLayout();
          this.Tab_Search.ResumeLayout(false);
          this.groupBox_SortByItem.ResumeLayout(false);
          this.groupBox_SortByItem.PerformLayout();
          this.groupBoxSearchByProperties.ResumeLayout(false);
          this.groupBoxSearchByProperties.PerformLayout();
          this.groupBox_SupplementarySearch.ResumeLayout(false);
          this.groupBox_SupplementarySearch.PerformLayout();
          this.groupBox_Separators.ResumeLayout(false);
          this.groupBox_Separators.PerformLayout();
          this.Tab_Views.ResumeLayout(false);
          this.groupBox_AntSelectedEnreg.ResumeLayout(false);
          this.groupBox_AntSelectedEnreg.PerformLayout();
          this.groupBox_DefaultView.ResumeLayout(false);
          this.groupBox_DefaultView.PerformLayout();
          this.groupBox_SupplementaryView.ResumeLayout(false);
          this.groupBox_SupplementaryView.PerformLayout();
          this.groupBox_DetailedSupplementaryInformations.ResumeLayout(false);
          this.groupBox_DetailedSupplementaryInformations.PerformLayout();
          this.Tab_Trailer.ResumeLayout(false);
          this.groupBox24.ResumeLayout(false);
          this.groupBox24.PerformLayout();
          this.Tab_General.ResumeLayout(false);
          this.Tab_General.PerformLayout();
          this.groupBoxSetupWizard.ResumeLayout(false);
          this.groupBox_Security.ResumeLayout(false);
          this.groupBox_Security.PerformLayout();
          this.groupBox_PreLaunchingCommand.ResumeLayout(false);
          this.groupBox_PreLaunchingCommand.PerformLayout();
          this.groupBox_PlayMovieInfos.ResumeLayout(false);
          this.groupBox_PlayMovieInfos.PerformLayout();
          this.groupBoxMoviePathInfos.ResumeLayout(false);
          this.groupBoxMoviePathInfos.PerformLayout();
          this.groupBox_TitleOrder.ResumeLayout(false);
          this.groupBox_TitleOrder.PerformLayout();
          this.groupBoxExtendedFieldHandling.ResumeLayout(false);
          this.groupBoxExtendedFieldHandling.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.SPicture)).EndInit();
          this.groupBox_DVDprofiler.ResumeLayout(false);
          this.groupBox13.ResumeLayout(false);
          this.groupBox13.PerformLayout();
          this.Tab_ExternalCatalogs.ResumeLayout(false);
          this.groupBoxAMCsettings.ResumeLayout(false);
          this.groupBoxAMCsettings.PerformLayout();
          this.Tab_WakeOnLan.ResumeLayout(false);
          this.Tab_Logos.ResumeLayout(false);
          this.Tab_Logos.PerformLayout();
          this.Tab_Artwork.ResumeLayout(false);
          this.Tab_AMCupdater.ResumeLayout(false);
          this.Tab_Update.ResumeLayout(false);
          this.Tab_Update.PerformLayout();
          this.General.ResumeLayout(false);
          this.Tab_Other.ResumeLayout(false);
          this.Tab_Other.PerformLayout();
          this.groupBox3.ResumeLayout(false);
          this.groupBox3.PerformLayout();
          this.groupBoxExternal.ResumeLayout(false);
          this.groupBoxExternal.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMyFilms)).EndInit();
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private Label label_VersionNumber;
        private TextBox textBoxPluginName;
        private Label label11;
        private ComboBox Config_Name;
        private Label label12;
        private Button ButDelet;
        private Button ButQuit;
        private ComboBox AntFilterSign4;
        private ComboBox AntFilterSign3;
        //private ComboBox AntFilterMinRating;
        private ComboBox AntFilterItem3;
        private TextBox AntFilterText3;
        private ComboBox AntFilterItem4;
        private TextBox AntFilterText4;
        private Button ButSave;
        private CheckBox Config_Dflt;
        private CheckBox Config_Menu;
        private CheckBox checkBox1;
        private Label label21;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private TextBox textBox2;
        private ToolTip ToolTip1;
        private PictureBox pictureBoxMyFilms;
        private Button ButCopy;
        private Button btnLaunchAMCglobal;
        private LinkLabel linkLabelMyFilmsWiki;
        private TabPage Tab_Trakt;
        private TabPage Tab_ExternalCatalogs;
        private GroupBox groupBox9;
        private CheckBox chkDVDprofilerMergeWithGenreField;
        private GroupBox groupBox_DVDprofiler;
        private GroupBox groupBox13;
        private CheckBox chkDVDprofilerOnlyFile;
        private TabPage Tab_WakeOnLan;
        private GroupBox groupBox25;
        private Label label37;
        private ComboBox comboWOLtimeout;
        private Button buttonSendMagicPacket3;
        private Button buttonSendMagicPacket2;
        private Label label30;
        private Button buttonSendMagicPacket1;
        private Button buttonGetMACadresses;
        private Label label_NAS_Server_3_MAC;
        private TextBox NAS_MAC_3;
        private Label label_NAS_Server_3_Name;
        private TextBox NAS_Name_3;
        private Label label_NAS_Server_2_MAC;
        private TextBox NAS_MAC_2;
        private Label label_NAS_Server_2_Name;
        private TextBox NAS_Name_2;
        private Label label_NAS_Server_1_MAC;
        private TextBox NAS_MAC_1;
        private Label label_NAS_Server_1_Name;
        private CheckBox check_WOL_Userdialog;
        private CheckBox check_WOL_enable;
        private TextBox NAS_Name_1;
        private Label label36;
        private TabPage Tab_Logos;
        private Button btnUpdate;
        private Button btnLogoClearCache;
        private Label lblLogoPresets;
        private Label lbl_LogoSpacing;
        private ComboBox comboBoxLogoPresets;
        private ComboBox comboBoxLogoSpacing;
        private Label lblSetupLogoRules;
        private Label lblInfoLogosForAll;
        private Label lblSelectLogoFile;
        private Button btnLogosPath;
        private TextBox txtLogosPath;
        private TextBox SFilePicture;
        private Label lblLogosPath;
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
        private ColumnHeader PathImage;
        private Button btnDown;
        private Button btnUp;
        private Button btnDel;
        private Button btnAdd;
        private ComboBox SAnd_Or;
        private ComboBox SValue2;
        private ComboBox SOp2;
        private ComboBox SField2;
        private ComboBox SValue1;
        private ComboBox SOp1;
        private ComboBox SField1;
        private CheckBox chkLogos;
        private ComboBox SLogo_Type;
        private PictureBox SPicture;
        private TabPage Tab_Artwork;
        private GroupBox groupBox_ArtistImages;
        private Button btnResetThumbsArtist;
        private CheckBox chkDfltArtist;
        private Button ButDefCovArtist;
        private Button ButImgArtist;
        private TextBox MesFilmsImgArtist;
        private Label label_DefaultArtistImage;
        private Label label_ArtistImagePath;
        private TextBox DefaultCoverArtist;
        private GroupBox groupBox1;
        private Button ButDefCov;
        private TextBox DefaultCover;
        private Label label_DefaulCover;
        private Button ButImg;
        private TextBox MesFilmsImg;
        private Label label2;
        private GroupBox groupBox22;
        private Button btnResetThumbs;
        private CheckBox chkViews;
        private Button btnViews;
        private CheckBox chkDfltViews;
        private Label label29;
        private TextBox MesFilmsViews;
        private GroupBox Fanart;
        private Label label40;
        private CheckBox chkFanart;
        private Button btnFanart;
        private TextBox MesFilmsFanart;
        private Label labelFanart;
        private CheckBox chkDfltFanart;
        private TabPage Tab_AMCupdater;
        private GroupBox groupBox_AMCupdater;
        private GroupBox groupBox_AMCupdaterScheduer;
        private CheckBox scheduleAMCUpdater;
        private Label label28;
        private Button btnParameters;
        private TextBox txtAMCUpd_cnf;
        private Button btnAMCUpd_cnf;
        private GroupBox groupBox_AMCupdater_ExternalApplication;
        private Label lblAMCMovieScanPath;
        private CheckBox chkAMC_Purge_Missing_Files;
        private Button btnAMCMovieScanPathAdd;
        private TextBox AMCMovieScanPath;
        private Button btnCreateAMCDesktopIcon;
        private Button btnCreateAMCDefaultConfig;
        private CheckBox chkAMCUpd;
        private GroupBox groupBox_GrabberOptions;
        private CheckBox chkGrabber_ChooseScript;
        private CheckBox chkGrabber_Always;
        private Button btnGrabber;
        private TextBox txtGrabber;
        private Label label27;
        private TabPage Tab_Update;
        private GroupBox groupBox_AntUpdatingItems;
        private CheckBox chkWindowsFileDialog;
        private TextBox AntUpdDflT2;
        private TextBox AntUpdDflT1;
        private ComboBox AntUpdItem2;
        private TextBox AntUpdText2;
        private ComboBox AntUpdItem1;
        private TextBox AntUpdText1;
        private GroupBox groupBox2;
        private Label lblUnwatchedItemsValue;
        private TextBox textBoxGlobalUnwatchedOnlyValue;
        private CheckBox CheckWatchedPlayerStopped;
        private Label Label_UserProfileName;
        private TextBox UserProfileName;
        private Label label19;
        private ComboBox cbWatched;
        private CheckBox chkEnhancedWatchedStatusHandling;
        private CheckBox CheckWatched;
        private GroupBox groupBoxUpdateByProperties;
        private Button AntUpdFieldReset;
        private Button AntUpdFieldClear;
        private Label label33;
        private ComboBox AntUpdField;
        private TextBox AntUpdList;
        private GroupBox groupBoxDeletionOptions;
        private CheckBox chkSuppress;
        private GroupBox gpsuppress;
        private RadioButton rbsuppress2;
        private RadioButton rbsuppress4;
        private RadioButton rbsuppress3;
        private RadioButton rbsuppress1;
        private GroupBox gpspfield;
        private CheckBox chksupplaystop;
        private TextBox txtfdupdate;
        private Label lblUpdateValue;
        private ComboBox cbfdupdate;
        private TabPage Tab_Search;
        private GroupBox groupBoxSearchByProperties;
        private Button AntSearchFieldReset;
        private Button AntSearchFieldClear;
        private Label label18;
        private ComboBox AntSearchField;
        private TextBox AntSearchList;
        private GroupBox groupBox_SupplementarySearch;
        private Label lblUserdefinedSearchItems;
        private ComboBox AntSearchItem2;
        private TextBox AntSearchText2;
        private ComboBox AntSearchItem1;
        private TextBox AntSearchText1;
        private GroupBox groupBox_Separators;
        private Label label25;
        private Label label23;
        private ComboBox RoleSeparator5;
        private ComboBox RoleSeparator4;
        private ComboBox ListSeparator5;
        private ComboBox ListSeparator4;
        private ComboBox ListSeparator3;
        private ComboBox ListSeparator2;
        private ComboBox RoleSeparator2;
        private ComboBox RoleSeparator3;
        private Label label22;
        private ComboBox RoleSeparator1;
        private Label label24;
        private ComboBox ListSeparator1;
        private TabPage Tab_Views;
        private GroupBox groupBox_AntSelectedEnreg;
        private TextBox Selected_Enreg;
        private ComboBox AntFilterComb;
        private ComboBox AntFilterSign2;
        private ComboBox AntFilterSign1;
        private ComboBox AntFilterItem2;
        private TextBox AntFilterText2;
        private ComboBox AntFilterItem1;
        private TextBox AntFilterText1;
        private GroupBox groupBox_DefaultView;
        private CheckBox chkGlobalUnwatchedOnly;
        private Label label10;
        private ComboBox SortSens;
        private Label label32;
        private ComboBox Sort;
        private CheckBox AlwaysDefaultView;
        private Label label14;
        private ComboBox LayOut;
        private ComboBox View_Dflt_Item;
        private TextBox View_Dflt_Text;
        private GroupBox groupBox_SupplementaryView;
        private TextBox AntViewValue5;
        private TextBox AntViewText5;
        private ComboBox AntViewItem5;
        private TextBox AntViewValue4;
        private TextBox AntViewText4;
        private ComboBox AntViewItem4;
        private TextBox AntViewValue3;
        private TextBox AntViewText3;
        private ComboBox AntViewItem3;
        private TextBox AntViewValue2;
        private TextBox AntViewValue1;
        private TextBox AntViewText2;
        private TextBox AntViewText1;
        private ComboBox AntViewItem2;
        private ComboBox AntViewItem1;
        private GroupBox groupBox_DetailedSupplementaryInformations;
        private TextBox AntLabel3;
        private CheckBox chkOnlyTitle;
        private ComboBox AntItem3;
        private ComboBox AntItem2;
        private TextBox AntLabel2;
        private Label label7;
        private ComboBox AntItem1;
        private Label label3;
        private TextBox AntLabel1;
        private TabPage Tab_Trailer;
        private GroupBox groupBox24;
        private Label labelTrailers;
        private CheckBox SearchSubDirsTrailer;
        private CheckBox ShowTrailerWhenStartingMovie;
        private Button btnTrailer;
        private CheckBox ShowTrailerPlayDialog;
        private TextBox PathStorageTrailer;
        private CheckBox SearchFileNameTrailer;
        private Label label34;
        private ComboBox ItemSearchFileNameTrailer;
        private ComboBox AntStorageTrailer;
        private Label label35;
        private TabPage Tab_General;
        private Label lblYellowShowRequiredItems;
        private GroupBox groupBox_Security;
        private Label label_Security;
        private Label label16;
        private Label label15;
        private TextBox Rpt_Dwp;
        private TextBox Dwp;
        private GroupBox groupBox_PreLaunchingCommand;
        private Label label39;
        private Label label38;
        private ComboBox CmdPar;
        private TextBox CmdExe;
        private GroupBox groupBox_PlayMovieInfos;
        private GroupBox groupBoxMoviePathInfos;
        private Label label5;
        private TextBox PathStorage;
        private CheckBox SearchSubDirs;
        private Button butPath;
        private CheckBox SearchFileName;
        private ComboBox ItemSearchFileName;
        private TextBox AntIdentLabel;
        private Label label6;
        private ComboBox AntStorage;
        private Label label4;
        private ComboBox AntIdentItem;
        private Label label13;
        private ComboBox CatalogType;
        private GroupBox groupBox_TitleOrder;
        private Label label20;
        private ComboBox AntSTitle;
        private Label label17;
        private TextBox TitleDelim;
        private Label label9;
        private Label label8;
        private ComboBox AntTitle2;
        private ComboBox AntTitle1;
        private Button ButCat;
        private TextBox MesFilmsCat;
        private Label label1;
        private TabControl General;
        private Button btnFirstTimeSetup;
        private GroupBox groupBoxSetupWizard;
        private Label lblResultingGroupViewsPathFanart;
        private Button butNew;
        private Label label41;
        private Label label31;
        private Label label26;
        private Label label43;
        private Label label42;
        private Label label46;
        private Label label45;
        private Label label44;
        private TextBox DefaultFanartImage;
        private TextBox DefaultCoverViews;
        private Button ButDefFanart;
        private Button ButDefCovViews;
        private Label label47;
        private Label label48;
        private CheckBox chkDfltFanartImage;
        private CheckBox chkPersons;
        private Label lblSearchGrabberName;
        private ComboBox ItemSearchGrabberName;
        private TextBox textBoxNBconfigs;
        private Label lblNbConfig;
        private CheckBox chkDfltViewsAll;
        private CheckBox chkDfltFanartImageAll;
        private CheckBox chkFanartDefaultViews;
        private Label lblPicturePrefix;
        private TextBox txtPicturePrefix;
        private Label lblPictureHandling;
        private ComboBox cbPictureHandling;
        private GroupBox groupBoxExtendedFieldHandling;
        private CheckBox chkUseOriginalAsTranslatedTitle;
        private CheckBox chkAddTagline;
        private CheckBox chkUseListviewForGroups;
        private CheckBox chkAddCertification;
        private ComboBox ECMergeDestinationFieldTagline;
        private ComboBox cbGrabberOverrideLanguage;
        private ComboBox cbGrabberOverridePersonLimit;
        private GroupBox groupBox6;
        private Label label49;
        private Label label50;
        private ComboBox cbGrabberOverrideTitleLimit;
        private Button btnEditScript;
        private Label label51;
        private CheckBox chkAddTags;
        private Label label53;
        private Label label54;
        private ComboBox ECMergeDestinationFieldCertification;
        private ComboBox ECMergeDestinationFieldTags;
        private ComboBox ECMergeDestinationFieldWriter;
        private CheckBox chkAddWriter;
        private Button buttonDeleteTmpCatalog;
        private Button buttonOpenTmpFile;
        private Button buttonOpenTmpFileAMC;
        private GroupBox groupBoxAMCsettings;
        private TextBox AMCexePath;
        private Label lblAMCpath;
        private Button buttonAMCpathSearch;
        private GroupBox groupBox_SortByItem;
        private ComboBox AntSort2;
        private TextBox AntTSort2;
        private ComboBox AntSort1;
        private TextBox AntTSort1;
        private TextBox AntLabel5;
        private TextBox AntLabel4;
        private ComboBox AntItem5;
        private ComboBox AntItem4;
        private CheckBox chkScanMediaOnStart;
        private TextBox txtGrabberDisplay;
        private CheckBox cbAllowTraktSync;
        private Label label55;
        private ComboBox cbGrabberOverrideGetRoles;
        private CheckBox SearchOnlyExactMatches;
        private ComboBox AmcTitleSearchHandling;
        private Label label56;
        private TextBox txtAMCUpd_cnf_Display;
        private LinkLabel linkLabelTrakt;
        private TabPage Tab_Other;
        private Button btnGrabberInterface;
        private Label lblAMCupdaterConfigPreview;
        private ListView AMCConfigView;
        private ColumnHeader Option;
        private ColumnHeader Value;
        private Label labelTraktDescription;
        private PictureBox pictureBox1;
        private CheckBox chkGlobalAvailableOnly;
        private Button btnWatchedImport;
        private Button btnWatchedExport;
        private CheckBox chkFanartDefaultViewsUseRandom;
        private GroupBox groupBoxTrakt;
        private GroupBox groupBoxExternal;
        private Label labelRecentlyAddedAPI;
        private CheckBox cbAllowRecentAddedAPI;
        private LinkLabel linkLabelTraktWiki;
        private LinkLabel linkLabelUsingTraktInMyFilmsWiki;
        private Button btnLoadEditorValues;
        private ComboBox cbEditorViews;
        private ComboBox cbEditorViewValues;
        private ComboBox cbEditorConfigs;
        private GroupBox groupBox3;
        private Label label58;
        private Label label57;
        private Label label52;
        private Label label60;
        private TextBox tbEditorSearchExpression;
        private Label label59;
        private ComboBox cbEditorLayout;
        private TextBox tbEditorStartParamsOutput;
        private Button btnSaveEditorStartParams;
        private CheckBox chkSuppressManual;
        private Label label61;
    }
}