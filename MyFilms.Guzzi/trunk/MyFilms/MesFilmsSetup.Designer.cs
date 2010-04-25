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
            this.CheckWatched = new System.Windows.Forms.CheckBox();
            this.SearchSubDirs = new System.Windows.Forms.CheckBox();
            this.PathStorage = new System.Windows.Forms.TextBox();
            this.AntStorage = new System.Windows.Forms.ComboBox();
            this.CatalogType = new System.Windows.Forms.ComboBox();
            this.MesFilmsImg = new System.Windows.Forms.TextBox();
            this.MesFilmsCat = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.AntSort2 = new System.Windows.Forms.ComboBox();
            this.AntTSort2 = new System.Windows.Forms.TextBox();
            this.AntSort1 = new System.Windows.Forms.ComboBox();
            this.AntTSort1 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Selected_Enreg = new System.Windows.Forms.TextBox();
            this.AntFilterComb = new System.Windows.Forms.ComboBox();
            this.AntFilterMinRating = new System.Windows.Forms.ComboBox();
            this.AntFilterSign4 = new System.Windows.Forms.ComboBox();
            this.AntFilterSign3 = new System.Windows.Forms.ComboBox();
            this.AntFilterSign2 = new System.Windows.Forms.ComboBox();
            this.AntFilterSign1 = new System.Windows.Forms.ComboBox();
            this.AntFilterItem4 = new System.Windows.Forms.ComboBox();
            this.AntFilterText4 = new System.Windows.Forms.TextBox();
            this.AntFilterItem3 = new System.Windows.Forms.ComboBox();
            this.AntFilterText3 = new System.Windows.Forms.TextBox();
            this.AntFilterItem2 = new System.Windows.Forms.ComboBox();
            this.AntFilterText2 = new System.Windows.Forms.TextBox();
            this.AntFilterItem1 = new System.Windows.Forms.ComboBox();
            this.AntFilterText1 = new System.Windows.Forms.TextBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.SortSens = new System.Windows.Forms.ComboBox();
            this.label32 = new System.Windows.Forms.Label();
            this.Sort = new System.Windows.Forms.ComboBox();
            this.AlwaysDefaultView = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.LayOut = new System.Windows.Forms.ComboBox();
            this.View_Dflt_Item = new System.Windows.Forms.ComboBox();
            this.View_Dflt_Text = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkOnlyTitle = new System.Windows.Forms.CheckBox();
            this.AntItem3 = new System.Windows.Forms.ComboBox();
            this.AntLabel2 = new System.Windows.Forms.TextBox();
            this.AntItem2 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AntLabel1 = new System.Windows.Forms.TextBox();
            this.AntItem1 = new System.Windows.Forms.ComboBox();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
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
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.CmdPar = new System.Windows.Forms.ComboBox();
            this.CmdExe = new System.Windows.Forms.TextBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
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
            this.groupBox19 = new System.Windows.Forms.GroupBox();
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
            this.label30 = new System.Windows.Forms.Label();
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
            this.DefaultCover = new System.Windows.Forms.TextBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.chkViews = new System.Windows.Forms.CheckBox();
            this.btnViews = new System.Windows.Forms.Button();
            this.chkDfltViews = new System.Windows.Forms.CheckBox();
            this.label29 = new System.Windows.Forms.Label();
            this.MesFilmsViews = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.General = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ButDefCov = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.Rpt_Dwp = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.AntIdentLabel = new System.Windows.Forms.TextBox();
            this.butPath = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ItemSearchFileName = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ButImg = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ButCat = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.Images = new System.Windows.Forms.RadioButton();
            this.Thumbnails = new System.Windows.Forms.RadioButton();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.SortTitle = new System.Windows.Forms.CheckBox();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.OnlyFile = new System.Windows.Forms.CheckBox();
            this.Grabber = new System.Windows.Forms.TabPage();
            this.Picture = new System.Windows.Forms.TabPage();
            this.Logos = new System.Windows.Forms.TabPage();
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
            this.SPicture = new System.Windows.Forms.PictureBox();
            this.SAnd_Or = new System.Windows.Forms.ComboBox();
            this.SValue2 = new System.Windows.Forms.ComboBox();
            this.SOp2 = new System.Windows.Forms.ComboBox();
            this.SField2 = new System.Windows.Forms.ComboBox();
            this.SValue1 = new System.Windows.Forms.ComboBox();
            this.SOp1 = new System.Windows.Forms.ComboBox();
            this.SField1 = new System.Windows.Forms.ComboBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.ButSave = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label21 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.btnResetThumbs = new System.Windows.Forms.Button();
            this.groupBox18 = new System.Windows.Forms.GroupBox();
            this.AntSearchItem2 = new System.Windows.Forms.ComboBox();
            this.AntSearchText2 = new System.Windows.Forms.TextBox();
            this.AntSearchItem1 = new System.Windows.Forms.ComboBox();
            this.AntSearchText1 = new System.Windows.Forms.TextBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.groupBox6.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.gpspfield.SuspendLayout();
            this.gpsuppress.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox20.SuspendLayout();
            this.groupBox19.SuspendLayout();
            this.Fanart.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.General.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.Grabber.SuspendLayout();
            this.Picture.SuspendLayout();
            this.Logos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPicture)).BeginInit();
            this.groupBox18.SuspendLayout();
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
            this.Config_Name.Location = new System.Drawing.Point(126, 15);
            this.Config_Name.Name = "Config_Name";
            this.Config_Name.Size = new System.Drawing.Size(202, 21);
            this.Config_Name.Sorted = true;
            this.Config_Name.TabIndex = 1;
            this.ToolTip1.SetToolTip(this.Config_Name, resources.GetString("Config_Name.ToolTip"));
            this.Config_Name.SelectedIndexChanged += new System.EventHandler(this.Config_Name_SelectedIndexChanged);
            this.Config_Name.Leave += new System.EventHandler(this.Config_Name_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(645, 14);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(98, 20);
            this.textBox1.TabIndex = 5;
            this.ToolTip1.SetToolTip(this.textBox1, "Name of the plugin displayed in MP.\r\nBy default Films, but you can choose a bette" +
                    "r name");
            // 
            // Dwp
            // 
            this.Dwp.Location = new System.Drawing.Point(103, 21);
            this.Dwp.Name = "Dwp";
            this.Dwp.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Dwp.Size = new System.Drawing.Size(112, 20);
            this.Dwp.TabIndex = 69;
            this.ToolTip1.SetToolTip(this.Dwp, "Enter a password here if you wish to restrict access (from children, for example)" +
                    " \r\nto this particular configuration.  \r\nThe password must be repeated in the sec" +
                    "ond field.\r\n");
            this.Dwp.UseSystemPasswordChar = true;
            // 
            // CheckWatched
            // 
            this.CheckWatched.AutoSize = true;
            this.CheckWatched.Location = new System.Drawing.Point(496, 69);
            this.CheckWatched.Name = "CheckWatched";
            this.CheckWatched.Size = new System.Drawing.Size(156, 30);
            this.CheckWatched.TabIndex = 69;
            this.CheckWatched.Text = "Update the ‘Checked’ field \r\nwhen movie is launched";
            this.ToolTip1.SetToolTip(this.CheckWatched, "Select this option if you want the “Checked” field of your database \r\nto be updat" +
                    "ed each time a movie is launched.\r\n");
            this.CheckWatched.UseVisualStyleBackColor = true;
            // 
            // SearchSubDirs
            // 
            this.SearchSubDirs.AutoSize = true;
            this.SearchSubDirs.Location = new System.Drawing.Point(496, 48);
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
            this.PathStorage.Location = new System.Drawing.Point(168, 47);
            this.PathStorage.Name = "PathStorage";
            this.PathStorage.Size = new System.Drawing.Size(268, 20);
            this.PathStorage.TabIndex = 54;
            this.ToolTip1.SetToolTip(this.PathStorage, resources.GetString("PathStorage.ToolTip"));
            // 
            // AntStorage
            // 
            this.AntStorage.FormattingEnabled = true;
            this.AntStorage.Location = new System.Drawing.Point(168, 19);
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
            this.CatalogType.Location = new System.Drawing.Point(561, 25);
            this.CatalogType.Name = "CatalogType";
            this.CatalogType.Size = new System.Drawing.Size(166, 21);
            this.CatalogType.TabIndex = 62;
            this.ToolTip1.SetToolTip(this.CatalogType, resources.GetString("CatalogType.ToolTip"));
            this.CatalogType.SelectedIndexChanged += new System.EventHandler(this.CatalogType_SelectedIndexChanged);
            // 
            // MesFilmsImg
            // 
            this.MesFilmsImg.Location = new System.Drawing.Point(139, 53);
            this.MesFilmsImg.Name = "MesFilmsImg";
            this.MesFilmsImg.Size = new System.Drawing.Size(259, 20);
            this.MesFilmsImg.TabIndex = 50;
            this.ToolTip1.SetToolTip(this.MesFilmsImg, "Enter the full path to the folder containing the DVD cover image files – by defau" +
                    "lt it is the same as the path to your database file.\r\nYou can use the browse but" +
                    "ton to find the correct path.\r\n");
            // 
            // MesFilmsCat
            // 
            this.MesFilmsCat.Location = new System.Drawing.Point(139, 27);
            this.MesFilmsCat.Name = "MesFilmsCat";
            this.MesFilmsCat.Size = new System.Drawing.Size(259, 20);
            this.MesFilmsCat.TabIndex = 48;
            this.ToolTip1.SetToolTip(this.MesFilmsCat, "Enter the full path and name of your AMC xml database file.\r\nYou can use the brow" +
                    "se button to find the file");
            this.MesFilmsCat.TextChanged += new System.EventHandler(this.MesFilmsCat_TextChanged);
            this.MesFilmsCat.Leave += new System.EventHandler(this.MesFilmsCat_Leave);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.AntSort2);
            this.groupBox6.Controls.Add(this.AntTSort2);
            this.groupBox6.Controls.Add(this.AntSort1);
            this.groupBox6.Controls.Add(this.AntTSort1);
            this.groupBox6.Location = new System.Drawing.Point(371, 271);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(370, 77);
            this.groupBox6.TabIndex = 29;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Sort by Item";
            this.ToolTip1.SetToolTip(this.groupBox6, resources.GetString("groupBox6.ToolTip"));
            // 
            // AntSort2
            // 
            this.AntSort2.FormattingEnabled = true;
            this.AntSort2.Location = new System.Drawing.Point(10, 45);
            this.AntSort2.Name = "AntSort2";
            this.AntSort2.Size = new System.Drawing.Size(160, 21);
            this.AntSort2.TabIndex = 38;
            this.AntSort2.SelectedIndexChanged += new System.EventHandler(this.AntSort2_SelectedIndexChanged);
            // 
            // AntTSort2
            // 
            this.AntTSort2.Location = new System.Drawing.Point(185, 45);
            this.AntTSort2.Name = "AntTSort2";
            this.AntTSort2.Size = new System.Drawing.Size(173, 20);
            this.AntTSort2.TabIndex = 39;
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
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Selected_Enreg);
            this.groupBox2.Controls.Add(this.AntFilterComb);
            this.groupBox2.Controls.Add(this.AntFilterSign2);
            this.groupBox2.Controls.Add(this.AntFilterSign1);
            this.groupBox2.Controls.Add(this.AntFilterItem2);
            this.groupBox2.Controls.Add(this.AntFilterText2);
            this.groupBox2.Controls.Add(this.AntFilterItem1);
            this.groupBox2.Controls.Add(this.AntFilterText1);
            this.groupBox2.Location = new System.Drawing.Point(374, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(370, 117);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ant Selected Enreg.";
            this.ToolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            this.groupBox2.Leave += new System.EventHandler(this.Selected_Enreg_Changed);
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
            this.AntFilterText2.Size = new System.Drawing.Size(119, 20);
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
            this.AntFilterText1.Size = new System.Drawing.Size(119, 20);
            this.AntFilterText1.TabIndex = 21;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.SortSens);
            this.groupBox8.Controls.Add(this.label32);
            this.groupBox8.Controls.Add(this.Sort);
            this.groupBox8.Controls.Add(this.AlwaysDefaultView);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.Controls.Add(this.LayOut);
            this.groupBox8.Controls.Add(this.View_Dflt_Item);
            this.groupBox8.Controls.Add(this.View_Dflt_Text);
            this.groupBox8.Location = new System.Drawing.Point(9, 169);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(347, 96);
            this.groupBox8.TabIndex = 26;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Default View";
            this.ToolTip1.SetToolTip(this.groupBox8, resources.GetString("groupBox8.ToolTip"));
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
            this.AlwaysDefaultView.Location = new System.Drawing.Point(76, 71);
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
            "Filmstrip"});
            this.LayOut.Location = new System.Drawing.Point(55, 44);
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.AntViewValue5);
            this.groupBox3.Controls.Add(this.AntViewText5);
            this.groupBox3.Controls.Add(this.AntViewItem5);
            this.groupBox3.Controls.Add(this.AntViewValue4);
            this.groupBox3.Controls.Add(this.AntViewText4);
            this.groupBox3.Controls.Add(this.AntViewItem4);
            this.groupBox3.Controls.Add(this.AntViewValue3);
            this.groupBox3.Controls.Add(this.AntViewText3);
            this.groupBox3.Controls.Add(this.AntViewItem3);
            this.groupBox3.Controls.Add(this.AntViewValue2);
            this.groupBox3.Controls.Add(this.AntViewValue1);
            this.groupBox3.Controls.Add(this.AntViewText2);
            this.groupBox3.Controls.Add(this.AntViewText1);
            this.groupBox3.Controls.Add(this.AntViewItem2);
            this.groupBox3.Controls.Add(this.AntViewItem1);
            this.groupBox3.Location = new System.Drawing.Point(9, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(347, 157);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Supplementary View";
            this.ToolTip1.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
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
            this.AntViewItem5.Location = new System.Drawing.Point(9, 122);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkOnlyTitle);
            this.groupBox1.Controls.Add(this.AntItem3);
            this.groupBox1.Controls.Add(this.AntLabel2);
            this.groupBox1.Controls.Add(this.AntItem2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.AntLabel1);
            this.groupBox1.Controls.Add(this.AntItem1);
            this.groupBox1.Location = new System.Drawing.Point(374, 129);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(347, 136);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Detailed Supplementary Informations";
            this.ToolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // chkOnlyTitle
            // 
            this.chkOnlyTitle.AutoSize = true;
            this.chkOnlyTitle.Location = new System.Drawing.Point(22, 111);
            this.chkOnlyTitle.Name = "chkOnlyTitle";
            this.chkOnlyTitle.Size = new System.Drawing.Size(230, 17);
            this.chkOnlyTitle.TabIndex = 76;
            this.chkOnlyTitle.Text = "Display Only Movie\'s Title within List Layout";
            this.chkOnlyTitle.UseVisualStyleBackColor = true;
            // 
            // AntItem3
            // 
            this.AntItem3.FormattingEnabled = true;
            this.AntItem3.Location = new System.Drawing.Point(166, 87);
            this.AntItem3.Name = "AntItem3";
            this.AntItem3.Size = new System.Drawing.Size(170, 21);
            this.AntItem3.TabIndex = 29;
            // 
            // AntLabel2
            // 
            this.AntLabel2.Location = new System.Drawing.Point(9, 60);
            this.AntLabel2.Name = "AntLabel2";
            this.AntLabel2.Size = new System.Drawing.Size(146, 20);
            this.AntLabel2.TabIndex = 27;
            // 
            // AntItem2
            // 
            this.AntItem2.FormattingEnabled = true;
            this.AntItem2.Location = new System.Drawing.Point(166, 60);
            this.AntItem2.Name = "AntItem2";
            this.AntItem2.Size = new System.Drawing.Size(170, 21);
            this.AntItem2.TabIndex = 28;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(170, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Ant Item to Display";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Label";
            // 
            // AntLabel1
            // 
            this.AntLabel1.Location = new System.Drawing.Point(9, 33);
            this.AntLabel1.Name = "AntLabel1";
            this.AntLabel1.Size = new System.Drawing.Size(146, 20);
            this.AntLabel1.TabIndex = 25;
            // 
            // AntItem1
            // 
            this.AntItem1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.AntItem1.FormattingEnabled = true;
            this.AntItem1.Location = new System.Drawing.Point(166, 33);
            this.AntItem1.Name = "AntItem1";
            this.AntItem1.Size = new System.Drawing.Size(170, 21);
            this.AntItem1.Sorted = true;
            this.AntItem1.TabIndex = 26;
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.groupBox4);
            this.groupBox21.Controls.Add(this.chksupplaystop);
            this.groupBox21.Controls.Add(this.gpspfield);
            this.groupBox21.Controls.Add(this.gpsuppress);
            this.groupBox21.Controls.Add(this.chkSuppress);
            this.groupBox21.Location = new System.Drawing.Point(11, 6);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(370, 342);
            this.groupBox21.TabIndex = 32;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "Database update options";
            this.ToolTip1.SetToolTip(this.groupBox21, resources.GetString("groupBox21.ToolTip"));
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label33);
            this.groupBox4.Controls.Add(this.textBox5);
            this.groupBox4.Controls.Add(this.comboBox4);
            this.groupBox4.Controls.Add(this.chkWindowsFileDialog);
            this.groupBox4.Controls.Add(this.AntUpdDflT2);
            this.groupBox4.Controls.Add(this.AntUpdDflT1);
            this.groupBox4.Controls.Add(this.AntUpdItem2);
            this.groupBox4.Controls.Add(this.AntUpdText2);
            this.groupBox4.Controls.Add(this.AntUpdItem1);
            this.groupBox4.Controls.Add(this.AntUpdText1);
            this.groupBox4.Location = new System.Drawing.Point(9, 164);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(354, 168);
            this.groupBox4.TabIndex = 33;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Ant Updating Items.";
            this.ToolTip1.SetToolTip(this.groupBox4, resources.GetString("groupBox4.ToolTip"));
            // 
            // chkWindowsFileDialog
            // 
            this.chkWindowsFileDialog.AutoSize = true;
            this.chkWindowsFileDialog.Location = new System.Drawing.Point(16, 74);
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
            this.chksupplaystop.Location = new System.Drawing.Point(245, 31);
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
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.CmdPar);
            this.groupBox12.Controls.Add(this.CmdExe);
            this.groupBox12.Location = new System.Drawing.Point(391, 300);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(350, 48);
            this.groupBox12.TabIndex = 31;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Pre-Launching Command";
            this.ToolTip1.SetToolTip(this.groupBox12, "You can define here a command batch file to be executed before\r\neach movie launch" +
                    ". The following field give the item of your \r\ndatabase passed as  parameter to t" +
                    "he command file.\r\n\r\nVery unused...");
            // 
            // CmdPar
            // 
            this.CmdPar.FormattingEnabled = true;
            this.CmdPar.Location = new System.Drawing.Point(222, 17);
            this.CmdPar.Name = "CmdPar";
            this.CmdPar.Size = new System.Drawing.Size(124, 21);
            this.CmdPar.TabIndex = 36;
            // 
            // CmdExe
            // 
            this.CmdExe.Location = new System.Drawing.Point(12, 18);
            this.CmdExe.Name = "CmdExe";
            this.CmdExe.Size = new System.Drawing.Size(204, 20);
            this.CmdExe.TabIndex = 37;
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.label25);
            this.groupBox14.Controls.Add(this.label23);
            this.groupBox14.Controls.Add(this.RoleSeparator5);
            this.groupBox14.Controls.Add(this.RoleSeparator4);
            this.groupBox14.Controls.Add(this.ListSeparator5);
            this.groupBox14.Controls.Add(this.ListSeparator4);
            this.groupBox14.Controls.Add(this.ListSeparator3);
            this.groupBox14.Controls.Add(this.ListSeparator2);
            this.groupBox14.Controls.Add(this.RoleSeparator2);
            this.groupBox14.Controls.Add(this.RoleSeparator3);
            this.groupBox14.Controls.Add(this.label22);
            this.groupBox14.Controls.Add(this.RoleSeparator1);
            this.groupBox14.Controls.Add(this.label24);
            this.groupBox14.Controls.Add(this.ListSeparator1);
            this.groupBox14.Location = new System.Drawing.Point(391, 6);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(350, 114);
            this.groupBox14.TabIndex = 27;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Separators";
            this.ToolTip1.SetToolTip(this.groupBox14, resources.GetString("groupBox14.ToolTip"));
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(18, 89);
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
            this.RoleSeparator5.Location = new System.Drawing.Point(306, 76);
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
            this.RoleSeparator4.Location = new System.Drawing.Point(260, 76);
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
            this.RoleSeparator2.Location = new System.Drawing.Point(168, 76);
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
            this.RoleSeparator3.Location = new System.Drawing.Point(214, 76);
            this.RoleSeparator3.Name = "RoleSeparator3";
            this.RoleSeparator3.Size = new System.Drawing.Size(40, 21);
            this.RoleSeparator3.Sorted = true;
            this.RoleSeparator3.TabIndex = 75;
            this.RoleSeparator3.Text = "List";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(9, 76);
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
            this.RoleSeparator1.Location = new System.Drawing.Point(122, 76);
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
            this.groupBox9.Size = new System.Drawing.Size(327, 81);
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
            this.groupBox20.Location = new System.Drawing.Point(7, 149);
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
            this.scheduleAMCUpdater.Location = new System.Drawing.Point(23, 65);
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
            // groupBox19
            // 
            this.groupBox19.Controls.Add(this.chkGrabber_ChooseScript);
            this.groupBox19.Controls.Add(this.btnDirGrab);
            this.groupBox19.Controls.Add(this.txtDirGrab);
            this.groupBox19.Controls.Add(this.label31);
            this.groupBox19.Controls.Add(this.chkGrabber_Always);
            this.groupBox19.Controls.Add(this.btnGrabber);
            this.groupBox19.Controls.Add(this.txtGrabber);
            this.groupBox19.Controls.Add(this.label27);
            this.groupBox19.Controls.Add(this.chkGrabber);
            this.groupBox19.Location = new System.Drawing.Point(7, 15);
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.Size = new System.Drawing.Size(693, 128);
            this.groupBox19.TabIndex = 1;
            this.groupBox19.TabStop = false;
            this.groupBox19.Text = "grabber options";
            this.ToolTip1.SetToolTip(this.groupBox19, resources.GetString("groupBox19.ToolTip"));
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
            this.chkGrabber.Size = new System.Drawing.Size(178, 30);
            this.chkGrabber.TabIndex = 0;
            this.chkGrabber.Text = "grabber used \r\n(for movie detail Internet update)";
            this.chkGrabber.UseVisualStyleBackColor = true;
            this.chkGrabber.CheckedChanged += new System.EventHandler(this.chkGrabber_CheckedChanged);
            // 
            // Fanart
            // 
            this.Fanart.Controls.Add(this.chkFanart);
            this.Fanart.Controls.Add(this.btnFanart);
            this.Fanart.Controls.Add(this.MesFilmsFanart);
            this.Fanart.Controls.Add(this.label30);
            this.Fanart.Controls.Add(this.chkDfltFanart);
            this.Fanart.Location = new System.Drawing.Point(17, 26);
            this.Fanart.Name = "Fanart";
            this.Fanart.Size = new System.Drawing.Size(474, 104);
            this.Fanart.TabIndex = 71;
            this.Fanart.TabStop = false;
            this.Fanart.Text = "Fanart (Backdrops)";
            this.ToolTip1.SetToolTip(this.Fanart, resources.GetString("Fanart.ToolTip"));
            // 
            // chkFanart
            // 
            this.chkFanart.AutoSize = true;
            this.chkFanart.Location = new System.Drawing.Point(27, 19);
            this.chkFanart.Name = "chkFanart";
            this.chkFanart.Size = new System.Drawing.Size(171, 17);
            this.chkFanart.TabIndex = 71;
            this.chkFanart.Text = "Use Fanart (bacdrops pictures)";
            this.chkFanart.UseVisualStyleBackColor = true;
            this.chkFanart.CheckedChanged += new System.EventHandler(this.chkFanart_CheckedChanged);
            // 
            // btnFanart
            // 
            this.btnFanart.Enabled = false;
            this.btnFanart.Location = new System.Drawing.Point(430, 45);
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
            this.MesFilmsFanart.Location = new System.Drawing.Point(190, 47);
            this.MesFilmsFanart.Name = "MesFilmsFanart";
            this.MesFilmsFanart.Size = new System.Drawing.Size(234, 20);
            this.MesFilmsFanart.TabIndex = 68;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(40, 50);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(133, 13);
            this.label30.TabIndex = 69;
            this.label30.Text = "Fanart Picture Path  (IMG) ";
            // 
            // chkDfltFanart
            // 
            this.chkDfltFanart.AutoSize = true;
            this.chkDfltFanart.Enabled = false;
            this.chkDfltFanart.Location = new System.Drawing.Point(43, 73);
            this.chkDfltFanart.Name = "chkDfltFanart";
            this.chkDfltFanart.Size = new System.Drawing.Size(241, 17);
            this.chkDfltFanart.TabIndex = 67;
            this.chkDfltFanart.Text = "Use the Default DVD cover for missing Fanart\r\n";
            this.chkDfltFanart.UseVisualStyleBackColor = true;
            // 
            // txtLogosPath
            // 
            this.txtLogosPath.Enabled = false;
            this.txtLogosPath.Location = new System.Drawing.Point(389, 11);
            this.txtLogosPath.Name = "txtLogosPath";
            this.txtLogosPath.Size = new System.Drawing.Size(234, 20);
            this.txtLogosPath.TabIndex = 92;
            this.ToolTip1.SetToolTip(this.txtLogosPath, resources.GetString("txtLogosPath.ToolTip"));
            // 
            // chkLogos
            // 
            this.chkLogos.AutoSize = true;
            this.chkLogos.Location = new System.Drawing.Point(42, 13);
            this.chkLogos.Name = "chkLogos";
            this.chkLogos.Size = new System.Drawing.Size(90, 17);
            this.chkLogos.TabIndex = 74;
            this.chkLogos.Text = "enable Logos";
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
            this.SLogo_Type.Location = new System.Drawing.Point(27, 247);
            this.SLogo_Type.Name = "SLogo_Type";
            this.SLogo_Type.Size = new System.Drawing.Size(121, 21);
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
            this.Config_Dflt.Location = new System.Drawing.Point(351, 17);
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
            this.Config_Menu.Location = new System.Drawing.Point(351, 43);
            this.Config_Menu.Name = "Config_Menu";
            this.Config_Menu.Size = new System.Drawing.Size(297, 17);
            this.Config_Menu.TabIndex = 74;
            this.Config_Menu.Text = "Display Always Configuration\'s Menu (if no Default Config)";
            this.ToolTip1.SetToolTip(this.Config_Menu, resources.GetString("Config_Menu.ToolTip"));
            this.Config_Menu.UseVisualStyleBackColor = true;
            // 
            // SearchFileName
            // 
            this.SearchFileName.AutoSize = true;
            this.SearchFileName.Location = new System.Drawing.Point(449, 23);
            this.SearchFileName.Name = "SearchFileName";
            this.SearchFileName.Size = new System.Drawing.Size(144, 17);
            this.SearchFileName.TabIndex = 66;
            this.SearchFileName.Text = "Search by Movie\'s Name";
            this.ToolTip1.SetToolTip(this.SearchFileName, "If file is not found with the \'Ant Item for Storage Info\' field, you \r\ncan search" +
                    " the file with the movie Name.\r\nValidate that option for it and give the Title u" +
                    "sed for the search.\r\n");
            this.SearchFileName.UseVisualStyleBackColor = true;
            // 
            // AntIdentItem
            // 
            this.AntIdentItem.FormattingEnabled = true;
            this.AntIdentItem.Location = new System.Drawing.Point(168, 73);
            this.AntIdentItem.Name = "AntIdentItem";
            this.AntIdentItem.Size = new System.Drawing.Size(157, 21);
            this.AntIdentItem.TabIndex = 56;
            this.ToolTip1.SetToolTip(this.AntIdentItem, resources.GetString("AntIdentItem.ToolTip"));
            // 
            // AntSTitle
            // 
            this.AntSTitle.FormattingEnabled = true;
            this.AntSTitle.Location = new System.Drawing.Point(132, 70);
            this.AntSTitle.Name = "AntSTitle";
            this.AntSTitle.Size = new System.Drawing.Size(155, 21);
            this.AntSTitle.TabIndex = 70;
            this.ToolTip1.SetToolTip(this.AntSTitle, "Select the ANT database field that you want to be used in the ‘Sort by name’ fiel" +
                    "d in the MediaPortal menu.");
            // 
            // TitleDelim
            // 
            this.TitleDelim.Location = new System.Drawing.Point(413, 17);
            this.TitleDelim.MaxLength = 1;
            this.TitleDelim.Name = "TitleDelim";
            this.TitleDelim.Size = new System.Drawing.Size(20, 20);
            this.TitleDelim.TabIndex = 20;
            this.ToolTip1.SetToolTip(this.TitleDelim, "Character used for hierachical presentation.\r\nSearch on forums to have an idea ab" +
                    "out this.");
            // 
            // AntTitle2
            // 
            this.AntTitle2.FormattingEnabled = true;
            this.AntTitle2.Location = new System.Drawing.Point(132, 43);
            this.AntTitle2.Name = "AntTitle2";
            this.AntTitle2.Size = new System.Drawing.Size(155, 21);
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
            this.AntTitle1.Location = new System.Drawing.Point(132, 16);
            this.AntTitle1.Name = "AntTitle1";
            this.AntTitle1.Size = new System.Drawing.Size(155, 21);
            this.AntTitle1.TabIndex = 10;
            this.ToolTip1.SetToolTip(this.AntTitle1, "Select the ANT database field that you want to be displayed as the ‘Master Title’" +
                    ".");
            // 
            // DefaultCover
            // 
            this.DefaultCover.Location = new System.Drawing.Point(139, 79);
            this.DefaultCover.Name = "DefaultCover";
            this.DefaultCover.Size = new System.Drawing.Size(259, 20);
            this.DefaultCover.TabIndex = 77;
            this.ToolTip1.SetToolTip(this.DefaultCover, resources.GetString("DefaultCover.ToolTip"));
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.btnResetThumbs);
            this.groupBox22.Controls.Add(this.chkViews);
            this.groupBox22.Controls.Add(this.btnViews);
            this.groupBox22.Controls.Add(this.chkDfltViews);
            this.groupBox22.Controls.Add(this.label29);
            this.groupBox22.Controls.Add(this.MesFilmsViews);
            this.groupBox22.Location = new System.Drawing.Point(17, 136);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(474, 123);
            this.groupBox22.TabIndex = 75;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Thumbs for Views";
            this.ToolTip1.SetToolTip(this.groupBox22, resources.GetString("groupBox22.ToolTip"));
            // 
            // chkViews
            // 
            this.chkViews.AutoSize = true;
            this.chkViews.Location = new System.Drawing.Point(27, 19);
            this.chkViews.Name = "chkViews";
            this.chkViews.Size = new System.Drawing.Size(244, 17);
            this.chkViews.TabIndex = 71;
            this.chkViews.Text = "Use Thumbs for grouped views (genre, year...)";
            this.chkViews.UseVisualStyleBackColor = true;
            // 
            // btnViews
            // 
            this.btnViews.Location = new System.Drawing.Point(430, 44);
            this.btnViews.Name = "btnViews";
            this.btnViews.Size = new System.Drawing.Size(32, 23);
            this.btnViews.TabIndex = 74;
            this.btnViews.Text = "...";
            this.btnViews.UseVisualStyleBackColor = true;
            this.btnViews.Click += new System.EventHandler(this.btnGenre_Click);
            // 
            // chkDfltViews
            // 
            this.chkDfltViews.AutoSize = true;
            this.chkDfltViews.Location = new System.Drawing.Point(43, 88);
            this.chkDfltViews.Name = "chkDfltViews";
            this.chkDfltViews.Size = new System.Drawing.Size(249, 17);
            this.chkDfltViews.TabIndex = 75;
            this.chkDfltViews.Text = "Use the Default DVD cover for missing Thumbs";
            this.chkDfltViews.UseVisualStyleBackColor = true;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(21, 49);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(162, 26);
            this.label29.TabIndex = 73;
            this.label29.Text = "Grouped Views Picture Path  \r\n(IMG for Genre, Year, Country...) ";
            // 
            // MesFilmsViews
            // 
            this.MesFilmsViews.Location = new System.Drawing.Point(190, 46);
            this.MesFilmsViews.Name = "MesFilmsViews";
            this.MesFilmsViews.Size = new System.Drawing.Size(231, 20);
            this.MesFilmsViews.TabIndex = 72;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(676, 42);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 13);
            this.label10.TabIndex = 34;
            this.label10.Text = "Version 5.0.0 beta";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(516, 17);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(123, 13);
            this.label11.TabIndex = 36;
            this.label11.Text = "MP\'s Menu Plugin Name";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(20, 19);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 13);
            this.label12.TabIndex = 38;
            this.label12.Text = "Configuration Name";
            // 
            // General
            // 
            this.General.Controls.Add(this.tabPage1);
            this.General.Controls.Add(this.tabPage2);
            this.General.Controls.Add(this.tabPage4);
            this.General.Controls.Add(this.tabPage3);
            this.General.Controls.Add(this.Grabber);
            this.General.Controls.Add(this.Picture);
            this.General.Controls.Add(this.Logos);
            this.General.Location = new System.Drawing.Point(12, 66);
            this.General.Name = "General";
            this.General.SelectedIndex = 0;
            this.General.Size = new System.Drawing.Size(755, 380);
            this.General.TabIndex = 46;
            this.General.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.General_Selected);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ButDefCov);
            this.tabPage1.Controls.Add(this.DefaultCover);
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Controls.Add(this.groupBox11);
            this.tabPage1.Controls.Add(this.groupBox7);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.CatalogType);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.ButImg);
            this.tabPage1.Controls.Add(this.MesFilmsImg);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.ButCat);
            this.tabPage1.Controls.Add(this.MesFilmsCat);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(747, 354);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ButDefCov
            // 
            this.ButDefCov.Location = new System.Drawing.Point(404, 78);
            this.ButDefCov.Name = "ButDefCov";
            this.ButDefCov.Size = new System.Drawing.Size(32, 22);
            this.ButDefCov.TabIndex = 78;
            this.ButDefCov.Text = "...";
            this.ButDefCov.UseVisualStyleBackColor = true;
            this.ButDefCov.Click += new System.EventHandler(this.ButDefCov_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(15, 83);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(97, 13);
            this.label19.TabIndex = 76;
            this.label19.Text = "Default DVD cover";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.label16);
            this.groupBox11.Controls.Add(this.label15);
            this.groupBox11.Controls.Add(this.Rpt_Dwp);
            this.groupBox11.Controls.Add(this.Dwp);
            this.groupBox11.Location = new System.Drawing.Point(503, 247);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(229, 76);
            this.groupBox11.TabIndex = 75;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Security";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 46);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(91, 13);
            this.label16.TabIndex = 72;
            this.label16.Text = "Repeat Password";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(44, 25);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 13);
            this.label15.TabIndex = 71;
            this.label15.Text = "Password";
            // 
            // Rpt_Dwp
            // 
            this.Rpt_Dwp.Location = new System.Drawing.Point(103, 43);
            this.Rpt_Dwp.Name = "Rpt_Dwp";
            this.Rpt_Dwp.Size = new System.Drawing.Size(112, 20);
            this.Rpt_Dwp.TabIndex = 70;
            this.Rpt_Dwp.UseSystemPasswordChar = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.CheckWatched);
            this.groupBox7.Controls.Add(this.SearchSubDirs);
            this.groupBox7.Controls.Add(this.AntIdentLabel);
            this.groupBox7.Controls.Add(this.butPath);
            this.groupBox7.Controls.Add(this.PathStorage);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Controls.Add(this.label6);
            this.groupBox7.Controls.Add(this.AntStorage);
            this.groupBox7.Controls.Add(this.ItemSearchFileName);
            this.groupBox7.Controls.Add(this.SearchFileName);
            this.groupBox7.Controls.Add(this.label4);
            this.groupBox7.Controls.Add(this.AntIdentItem);
            this.groupBox7.Location = new System.Drawing.Point(7, 119);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(725, 104);
            this.groupBox7.TabIndex = 74;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Play Movie Infos";
            // 
            // AntIdentLabel
            // 
            this.AntIdentLabel.Location = new System.Drawing.Point(331, 74);
            this.AntIdentLabel.Name = "AntIdentLabel";
            this.AntIdentLabel.Size = new System.Drawing.Size(144, 20);
            this.AntIdentLabel.TabIndex = 57;
            // 
            // butPath
            // 
            this.butPath.Location = new System.Drawing.Point(443, 44);
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
            this.label5.Location = new System.Drawing.Point(8, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(145, 13);
            this.label5.TabIndex = 60;
            this.label5.Text = "Paths for Movies File Search ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 21);
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
            this.ItemSearchFileName.Location = new System.Drawing.Point(599, 21);
            this.ItemSearchFileName.Name = "ItemSearchFileName";
            this.ItemSearchFileName.Size = new System.Drawing.Size(112, 21);
            this.ItemSearchFileName.TabIndex = 67;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 58;
            this.label4.Text = "Ant Identification Item";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(485, 30);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 63;
            this.label13.Text = "Catalog Type";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label20);
            this.groupBox5.Controls.Add(this.AntSTitle);
            this.groupBox5.Controls.Add(this.label17);
            this.groupBox5.Controls.Add(this.TitleDelim);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.AntTitle2);
            this.groupBox5.Controls.Add(this.AntTitle1);
            this.groupBox5.Location = new System.Drawing.Point(7, 238);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(453, 101);
            this.groupBox5.TabIndex = 61;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Title Order";
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
            this.label17.Location = new System.Drawing.Point(306, 21);
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
            // ButImg
            // 
            this.ButImg.Location = new System.Drawing.Point(404, 51);
            this.ButImg.Name = "ButImg";
            this.ButImg.Size = new System.Drawing.Size(32, 23);
            this.ButImg.TabIndex = 52;
            this.ButImg.Text = "...";
            this.ButImg.UseVisualStyleBackColor = true;
            this.ButImg.Click += new System.EventHandler(this.ButImg_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 51;
            this.label2.Text = "Cover Images Path";
            // 
            // ButCat
            // 
            this.ButCat.Location = new System.Drawing.Point(404, 25);
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
            this.label1.Location = new System.Drawing.Point(14, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 47;
            this.label1.Text = "Catalog File (XML)";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox8);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(747, 354);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Optional 1";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox18);
            this.tabPage4.Controls.Add(this.groupBox21);
            this.tabPage4.Controls.Add(this.groupBox12);
            this.tabPage4.Controls.Add(this.groupBox14);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(747, 354);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Optional 2";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox16);
            this.tabPage3.Controls.Add(this.groupBox10);
            this.tabPage3.Controls.Add(this.groupBox9);
            this.tabPage3.Controls.Add(this.groupBox15);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(747, 354);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "DVDProfiler/Movie Collecor";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.groupBox17);
            this.groupBox16.Location = new System.Drawing.Point(377, 17);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(349, 96);
            this.groupBox16.TabIndex = 29;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Movie Collector";
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.Images);
            this.groupBox17.Controls.Add(this.Thumbnails);
            this.groupBox17.Location = new System.Drawing.Point(6, 18);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(327, 72);
            this.groupBox17.TabIndex = 3;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "Covers";
            // 
            // Images
            // 
            this.Images.AutoSize = true;
            this.Images.Location = new System.Drawing.Point(24, 42);
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
            this.Thumbnails.Location = new System.Drawing.Point(24, 19);
            this.Thumbnails.Name = "Thumbnails";
            this.Thumbnails.Size = new System.Drawing.Size(188, 17);
            this.Thumbnails.TabIndex = 2;
            this.Thumbnails.TabStop = true;
            this.Thumbnails.Text = "Use \'Thumbnails\' Folder for Covers";
            this.Thumbnails.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.SortTitle);
            this.groupBox10.Location = new System.Drawing.Point(34, 121);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(327, 51);
            this.groupBox10.TabIndex = 1;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Use SortTitle";
            // 
            // SortTitle
            // 
            this.SortTitle.AutoSize = true;
            this.SortTitle.Location = new System.Drawing.Point(24, 19);
            this.SortTitle.Name = "SortTitle";
            this.SortTitle.Size = new System.Drawing.Size(199, 17);
            this.SortTitle.TabIndex = 0;
            this.SortTitle.Text = "Store SortTitle in FormattedTitle Field";
            this.SortTitle.UseVisualStyleBackColor = true;
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.groupBox13);
            this.groupBox15.Location = new System.Drawing.Point(22, 17);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(349, 314);
            this.groupBox15.TabIndex = 28;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "DVDProfiler";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.OnlyFile);
            this.groupBox13.Location = new System.Drawing.Point(12, 161);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(327, 50);
            this.groupBox13.TabIndex = 2;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Notes Field";
            // 
            // OnlyFile
            // 
            this.OnlyFile.AutoSize = true;
            this.OnlyFile.Location = new System.Drawing.Point(24, 19);
            this.OnlyFile.Name = "OnlyFile";
            this.OnlyFile.Size = new System.Drawing.Size(221, 17);
            this.OnlyFile.TabIndex = 0;
            this.OnlyFile.Text = "Notes Field contains only Movie Filename";
            this.OnlyFile.UseVisualStyleBackColor = true;
            // 
            // Grabber
            // 
            this.Grabber.Controls.Add(this.groupBox20);
            this.Grabber.Controls.Add(this.groupBox19);
            this.Grabber.Location = new System.Drawing.Point(4, 22);
            this.Grabber.Name = "Grabber";
            this.Grabber.Padding = new System.Windows.Forms.Padding(3);
            this.Grabber.Size = new System.Drawing.Size(747, 354);
            this.Grabber.TabIndex = 4;
            this.Grabber.Text = "Grabber";
            this.Grabber.UseVisualStyleBackColor = true;
            // 
            // Picture
            // 
            this.Picture.Controls.Add(this.groupBox22);
            this.Picture.Controls.Add(this.Fanart);
            this.Picture.Location = new System.Drawing.Point(4, 22);
            this.Picture.Name = "Picture";
            this.Picture.Padding = new System.Windows.Forms.Padding(3);
            this.Picture.Size = new System.Drawing.Size(747, 354);
            this.Picture.TabIndex = 5;
            this.Picture.Text = "Picture";
            this.Picture.UseVisualStyleBackColor = true;
            // 
            // Logos
            // 
            this.Logos.Controls.Add(this.btnLogosPath);
            this.Logos.Controls.Add(this.txtLogosPath);
            this.Logos.Controls.Add(this.lblLogosPath);
            this.Logos.Controls.Add(this.SFilePicture);
            this.Logos.Controls.Add(this.LogoView);
            this.Logos.Controls.Add(this.btnDown);
            this.Logos.Controls.Add(this.btnUp);
            this.Logos.Controls.Add(this.btnDel);
            this.Logos.Controls.Add(this.btnAdd);
            this.Logos.Controls.Add(this.SPicture);
            this.Logos.Controls.Add(this.SAnd_Or);
            this.Logos.Controls.Add(this.SValue2);
            this.Logos.Controls.Add(this.SOp2);
            this.Logos.Controls.Add(this.SField2);
            this.Logos.Controls.Add(this.SValue1);
            this.Logos.Controls.Add(this.SOp1);
            this.Logos.Controls.Add(this.SField1);
            this.Logos.Controls.Add(this.textBox3);
            this.Logos.Controls.Add(this.chkLogos);
            this.Logos.Controls.Add(this.SLogo_Type);
            this.Logos.Location = new System.Drawing.Point(4, 22);
            this.Logos.Name = "Logos";
            this.Logos.Padding = new System.Windows.Forms.Padding(3);
            this.Logos.Size = new System.Drawing.Size(747, 354);
            this.Logos.TabIndex = 6;
            this.Logos.Text = "Logos";
            this.Logos.UseVisualStyleBackColor = true;
            // 
            // btnLogosPath
            // 
            this.btnLogosPath.Enabled = false;
            this.btnLogosPath.Location = new System.Drawing.Point(629, 9);
            this.btnLogosPath.Name = "btnLogosPath";
            this.btnLogosPath.Size = new System.Drawing.Size(32, 23);
            this.btnLogosPath.TabIndex = 94;
            this.btnLogosPath.Text = "...";
            this.btnLogosPath.UseVisualStyleBackColor = true;
            this.btnLogosPath.Click += new System.EventHandler(this.btnLogosPath_Click);
            // 
            // lblLogosPath
            // 
            this.lblLogosPath.AutoSize = true;
            this.lblLogosPath.Location = new System.Drawing.Point(239, 14);
            this.lblLogosPath.Name = "lblLogosPath";
            this.lblLogosPath.Size = new System.Drawing.Size(110, 13);
            this.lblLogosPath.TabIndex = 93;
            this.lblLogosPath.Text = "Path for storing Logos";
            // 
            // SFilePicture
            // 
            this.SFilePicture.Location = new System.Drawing.Point(606, 227);
            this.SFilePicture.Name = "SFilePicture";
            this.SFilePicture.Size = new System.Drawing.Size(98, 20);
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
            this.LogoView.Size = new System.Drawing.Size(734, 185);
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
            this.btnDown.Location = new System.Drawing.Point(663, 312);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(47, 25);
            this.btnDown.TabIndex = 89;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Enabled = false;
            this.btnUp.Location = new System.Drawing.Point(610, 312);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(47, 25);
            this.btnUp.TabIndex = 88;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDel
            // 
            this.btnDel.Enabled = false;
            this.btnDel.Location = new System.Drawing.Point(557, 312);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(47, 25);
            this.btnDel.TabIndex = 87;
            this.btnDel.Text = "Delete";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(511, 312);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(40, 25);
            this.btnAdd.TabIndex = 86;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // SPicture
            // 
            this.SPicture.BackColor = System.Drawing.Color.SteelBlue;
            this.SPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SPicture.Enabled = false;
            this.SPicture.Location = new System.Drawing.Point(606, 247);
            this.SPicture.Name = "SPicture";
            this.SPicture.Size = new System.Drawing.Size(75, 48);
            this.SPicture.TabIndex = 85;
            this.SPicture.TabStop = false;
            this.SPicture.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // SAnd_Or
            // 
            this.SAnd_Or.Enabled = false;
            this.SAnd_Or.FormattingEnabled = true;
            this.SAnd_Or.Items.AddRange(new object[] {
            "And",
            "Or",
            ""});
            this.SAnd_Or.Location = new System.Drawing.Point(183, 274);
            this.SAnd_Or.Name = "SAnd_Or";
            this.SAnd_Or.Size = new System.Drawing.Size(54, 21);
            this.SAnd_Or.TabIndex = 82;
            // 
            // SValue2
            // 
            this.SValue2.Enabled = false;
            this.SValue2.FormattingEnabled = true;
            this.SValue2.Location = new System.Drawing.Point(439, 274);
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
            this.SOp2.Location = new System.Drawing.Point(359, 274);
            this.SOp2.Name = "SOp2";
            this.SOp2.Size = new System.Drawing.Size(74, 21);
            this.SOp2.TabIndex = 80;
            this.SOp2.SelectedIndexChanged += new System.EventHandler(this.SOp2_SelectedIndexChanged);
            // 
            // SField2
            // 
            this.SField2.Enabled = false;
            this.SField2.FormattingEnabled = true;
            this.SField2.Location = new System.Drawing.Point(243, 274);
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
            this.SValue1.Location = new System.Drawing.Point(439, 247);
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
            this.SOp1.Location = new System.Drawing.Point(359, 247);
            this.SOp1.Name = "SOp1";
            this.SOp1.Size = new System.Drawing.Size(74, 21);
            this.SOp1.TabIndex = 77;
            this.SOp1.SelectedIndexChanged += new System.EventHandler(this.SOp1_SelectedIndexChanged);
            // 
            // SField1
            // 
            this.SField1.Enabled = false;
            this.SField1.FormattingEnabled = true;
            this.SField1.Location = new System.Drawing.Point(243, 247);
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
            this.textBox3.Location = new System.Drawing.Point(7, 221);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(368, 20);
            this.textBox3.TabIndex = 75;
            this.textBox3.TabStop = false;
            this.textBox3.Text = "* be carefull the Logo Configuration is available for all MyFilms configurations";
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
            // btnResetThumbs
            // 
            this.btnResetThumbs.Location = new System.Drawing.Point(378, 84);
            this.btnResetThumbs.Name = "btnResetThumbs";
            this.btnResetThumbs.Size = new System.Drawing.Size(84, 23);
            this.btnResetThumbs.TabIndex = 76;
            this.btnResetThumbs.Text = "Reset Thumbs";
            this.ToolTip1.SetToolTip(this.btnResetThumbs, "That Action\'ll remove all generated Thumbs");
            this.btnResetThumbs.UseVisualStyleBackColor = true;
            this.btnResetThumbs.Click += new System.EventHandler(this.btnResetThumbs_Click);
            // 
            // groupBox18
            // 
            this.groupBox18.Controls.Add(this.label18);
            this.groupBox18.Controls.Add(this.textBox4);
            this.groupBox18.Controls.Add(this.AntSearchItem2);
            this.groupBox18.Controls.Add(this.comboBox3);
            this.groupBox18.Controls.Add(this.AntSearchText2);
            this.groupBox18.Controls.Add(this.AntSearchItem1);
            this.groupBox18.Controls.Add(this.AntSearchText1);
            this.groupBox18.Location = new System.Drawing.Point(391, 126);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.Size = new System.Drawing.Size(347, 150);
            this.groupBox18.TabIndex = 33;
            this.groupBox18.TabStop = false;
            this.groupBox18.Text = "Supplementary Search";
            this.ToolTip1.SetToolTip(this.groupBox18, resources.GetString("groupBox18.ToolTip"));
            // 
            // AntSearchItem2
            // 
            this.AntSearchItem2.FormattingEnabled = true;
            this.AntSearchItem2.Location = new System.Drawing.Point(10, 45);
            this.AntSearchItem2.Name = "AntSearchItem2";
            this.AntSearchItem2.Size = new System.Drawing.Size(145, 21);
            this.AntSearchItem2.TabIndex = 38;
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
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(225, 88);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(113, 21);
            this.comboBox3.TabIndex = 78;
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.Control;
            this.textBox4.Location = new System.Drawing.Point(10, 115);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(328, 20);
            this.textBox4.TabIndex = 79;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(64, 92);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(152, 13);
            this.label18.TabIndex = 80;
            this.label18.Text = "Search by Properties Selection";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(74, 119);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(153, 13);
            this.label33.TabIndex = 83;
            this.label33.Text = "Update by Properties Selection";
            // 
            // textBox5
            // 
            this.textBox5.BackColor = System.Drawing.SystemColors.Control;
            this.textBox5.Location = new System.Drawing.Point(10, 142);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(338, 20);
            this.textBox5.TabIndex = 82;
            // 
            // comboBox4
            // 
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Location = new System.Drawing.Point(235, 115);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(113, 21);
            this.comboBox4.TabIndex = 81;
            // 
            // MesFilmsSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 495);
            this.Controls.Add(this.Config_Menu);
            this.Controls.Add(this.ButSave);
            this.Controls.Add(this.Config_Dflt);
            this.Controls.Add(this.ButDelet);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.Config_Name);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.General);
            this.Controls.Add(this.ButQuit);
            this.Name = "MesFilmsSetup";
            this.Text = "MesFilmsSetup";
            this.ToolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.MesFilmsSetup_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MesFilmsSetup_Quit);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox21.ResumeLayout(false);
            this.groupBox21.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.gpspfield.ResumeLayout(false);
            this.gpspfield.PerformLayout();
            this.gpsuppress.ResumeLayout(false);
            this.gpsuppress.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox20.ResumeLayout(false);
            this.groupBox20.PerformLayout();
            this.groupBox19.ResumeLayout(false);
            this.groupBox19.PerformLayout();
            this.Fanart.ResumeLayout(false);
            this.Fanart.PerformLayout();
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.General.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox16.ResumeLayout(false);
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox15.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.Grabber.ResumeLayout(false);
            this.Picture.ResumeLayout(false);
            this.Logos.ResumeLayout(false);
            this.Logos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPicture)).EndInit();
            this.groupBox18.ResumeLayout(false);
            this.groupBox18.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label10;
        private TextBox textBox1;
        private Label label11;
        private ComboBox Config_Name;
        private Label label12;
        private TabControl General;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button ButDelet;
        private Button ButQuit;
        private GroupBox groupBox1;
        private ComboBox AntItem3;
        private TextBox AntLabel2;
        private ComboBox AntItem2;
        private Label label7;
        private Label label3;
        private TextBox AntLabel1;
        private ComboBox AntItem1;
        private TabPage tabPage3;
        private ComboBox ItemSearchFileName;
        private CheckBox SearchFileName;
        private Label label13;
        private ComboBox CatalogType;
        private GroupBox groupBox5;
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
        private Button ButImg;
        private TextBox MesFilmsImg;
        private Label label2;
        private Button ButCat;
        private TextBox MesFilmsCat;
        private Label label1;
        private GroupBox groupBox6;
        private ComboBox AntSort1;
        private TextBox AntTSort1;
        private GroupBox groupBox2;
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
        private GroupBox groupBox8;
        private ComboBox View_Dflt_Item;
        private TextBox View_Dflt_Text;
        private GroupBox groupBox3;
        private TextBox AntViewText2;
        private TextBox AntViewText1;
        private ComboBox AntViewItem2;
        private ComboBox AntViewItem1;
        private Button ButSave;
        private Label label17;
        private CheckBox Config_Dflt;
        private TextBox Rpt_Dwp;
        private TextBox Dwp;
        private GroupBox groupBox7;
        private GroupBox groupBox9;
        private GroupBox groupBox10;
        private ComboBox DVDPTagField;
        private Label label14;
        private ComboBox LayOut;
        private GroupBox groupBox11;
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
        private TabPage tabPage4;
        private GroupBox groupBox14;
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
        private GroupBox groupBox12;
        private ComboBox CmdPar;
        private TextBox CmdExe;
        private TextBox AntViewValue1;
        private TextBox AntViewValue2;
        private TextBox AntViewValue3;
        private TextBox AntViewText3;
        private ComboBox AntViewItem3;
        private GroupBox groupBox16;
        private GroupBox groupBox17;
        private GroupBox groupBox15;
        private RadioButton Thumbnails;
        private RadioButton Images;
        private CheckBox SearchSubDirs;
        private TabPage Grabber;
        private GroupBox groupBox19;
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
        private TabPage Picture;
        private CheckBox chkDfltFanart;
        private Button btnFanart;
        private TextBox MesFilmsFanart;
        private Label label30;
        private GroupBox Fanart;
        private CheckBox chkFanart;
        private TabPage Logos;
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
        private CheckBox CheckWatched;
        private GroupBox groupBox21;
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
        private GroupBox groupBox4;
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
        private Button ButDefCov;
        private TextBox DefaultCover;
        private Label label19;
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
        private GroupBox groupBox18;
        private Label label18;
        private TextBox textBox4;
        private ComboBox AntSearchItem2;
        private ComboBox comboBox3;
        private TextBox AntSearchText2;
        private ComboBox AntSearchItem1;
        private TextBox AntSearchText1;
        private Label label33;
        private TextBox textBox5;
        private ComboBox comboBox4;

    }
}