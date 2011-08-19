<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    <STAThread()> _
    Private Sub Main()

    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle13 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle14 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.Interactive = New System.Windows.Forms.TabPage
        Me.GroupBox24 = New System.Windows.Forms.GroupBox
        Me.txtOverridePath = New System.Windows.Forms.TextBox
        Me.lblOverridePath = New System.Windows.Forms.Label
        Me.btnSelectMovieFolder = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtMovieFolder = New System.Windows.Forms.TextBox
        Me.Label17 = New System.Windows.Forms.Label
        Me.GroupBox23 = New System.Windows.Forms.GroupBox
        Me.GroupBox27 = New System.Windows.Forms.GroupBox
        Me.chkDontAskInteractive = New System.Windows.Forms.CheckBox
        Me.Label24 = New System.Windows.Forms.Label
        Me.chkImportOnInternetFailIgnoreWhenInteractive = New System.Windows.Forms.CheckBox
        Me.cbInternetLookupBehaviour = New System.Windows.Forms.ComboBox
        Me.chkImportOnInternetFail = New System.Windows.Forms.CheckBox
        Me.txtParserFilePathDisplay = New System.Windows.Forms.TextBox
        Me.cbTitleHandling = New System.Windows.Forms.ComboBox
        Me.GroupBox26 = New System.Windows.Forms.GroupBox
        Me.Label87 = New System.Windows.Forms.Label
        Me.Label86 = New System.Windows.Forms.Label
        Me.Label85 = New System.Windows.Forms.Label
        Me.Label84 = New System.Windows.Forms.Label
        Me.chkGrabberOverrideTitleLimit = New System.Windows.Forms.ComboBox
        Me.chkGrabberOverridePersonLimit = New System.Windows.Forms.ComboBox
        Me.chkGrabberOverrideGetRoles = New System.Windows.Forms.ComboBox
        Me.chkGrabberOverrideLanguage = New System.Windows.Forms.ComboBox
        Me.Label43 = New System.Windows.Forms.Label
        Me.Button1 = New System.Windows.Forms.Button
        Me.btnSelectParserFile = New System.Windows.Forms.Button
        Me.Label23 = New System.Windows.Forms.Label
        Me.txtParserFilePath = New System.Windows.Forms.TextBox
        Me.GroupBox17 = New System.Windows.Forms.GroupBox
        Me.chkRescanMovedFiles = New System.Windows.Forms.CheckBox
        Me.chkProhibitInternetLookup = New System.Windows.Forms.CheckBox
        Me.chkPurgeMissing = New System.Windows.Forms.CheckBox
        Me.chkOverwriteXML = New System.Windows.Forms.CheckBox
        Me.chkBackupXMLFirst = New System.Windows.Forms.CheckBox
        Me.ToolStripProgressMessage = New System.Windows.Forms.Label
        Me.btnShowHideLog = New System.Windows.Forms.Button
        Me.GroupBox8 = New System.Windows.Forms.GroupBox
        Me.Label16 = New System.Windows.Forms.Label
        Me.btnParseXML = New System.Windows.Forms.Button
        Me.btnFindOrphans = New System.Windows.Forms.Button
        Me.btnCancelProcessing = New System.Windows.Forms.Button
        Me.btnUpdateXML = New System.Windows.Forms.Button
        Me.btnProcessMovieList = New System.Windows.Forms.Button
        Me.btnJustDoIt = New System.Windows.Forms.Button
        Me.Options = New System.Windows.Forms.TabPage
        Me.GroupBox4 = New System.Windows.Forms.GroupBox
        Me.chkParseTrailers = New System.Windows.Forms.CheckBox
        Me.GroupBox25 = New System.Windows.Forms.GroupBox
        Me.chkCheckDVDFolders = New System.Windows.Forms.CheckBox
        Me.chkParseSubtitleFiles = New System.Windows.Forms.CheckBox
        Me.chkParsePlaylistFiles = New System.Windows.Forms.CheckBox
        Me.txtTrailerIentificationStrings = New System.Windows.Forms.TextBox
        Me.Label79 = New System.Windows.Forms.Label
        Me.txtDefaultFileTypesNonMedia = New System.Windows.Forms.TextBox
        Me.Label14 = New System.Windows.Forms.Label
        Me.txtDefaultFileTypes = New System.Windows.Forms.TextBox
        Me.Label15 = New System.Windows.Forms.Label
        Me.GroupBox7 = New System.Windows.Forms.GroupBox
        Me.btnExcludeFileDelete = New System.Windows.Forms.Button
        Me.btnExcludeFileShow = New System.Windows.Forms.Button
        Me.btnSelectPersonArtworkFolder = New System.Windows.Forms.Button
        Me.Label28 = New System.Windows.Forms.Label
        Me.txtPersonArtworkFolder = New System.Windows.Forms.TextBox
        Me.btnSelectFanartFolder = New System.Windows.Forms.Button
        Me.txtFanartFolder = New System.Windows.Forms.TextBox
        Me.Label48 = New System.Windows.Forms.Label
        Me.txtExcludeFilePath = New System.Windows.Forms.TextBox
        Me.Label22 = New System.Windows.Forms.Label
        Me.btnSelectExcludeFile = New System.Windows.Forms.Button
        Me.txtConfigFilePath = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnSelectConfigFile = New System.Windows.Forms.Button
        Me.GroupBox13 = New System.Windows.Forms.GroupBox
        Me.cbLogLevel = New System.Windows.Forms.ComboBox
        Me.Label29 = New System.Windows.Forms.Label
        Me.GroupBox12 = New System.Windows.Forms.GroupBox
        Me.chkExecuteOnlyForOrphans = New System.Windows.Forms.CheckBox
        Me.btnExecuteProgramSelectPath = New System.Windows.Forms.Button
        Me.txtExecuteProgramPath = New System.Windows.Forms.TextBox
        Me.chkExecuteProgram = New System.Windows.Forms.CheckBox
        Me.Label27 = New System.Windows.Forms.Label
        Me.GroupBox5 = New System.Windows.Forms.GroupBox
        Me.chkShortNames = New System.Windows.Forms.CheckBox
        Me.chkReadDVDLabel = New System.Windows.Forms.CheckBox
        Me.txtMediaLabel = New System.Windows.Forms.TextBox
        Me.Label12 = New System.Windows.Forms.Label
        Me.txtMediaType = New System.Windows.Forms.TextBox
        Me.Label13 = New System.Windows.Forms.Label
        Me.txtDefaultSourceField = New System.Windows.Forms.TextBox
        Me.Label26 = New System.Windows.Forms.Label
        Me.ScanFilters = New System.Windows.Forms.TabPage
        Me.GroupBox6 = New System.Windows.Forms.GroupBox
        Me.Label25 = New System.Windows.Forms.Label
        Me.txtRegExSearchMultiPart = New System.Windows.Forms.TextBox
        Me.Label46 = New System.Windows.Forms.Label
        Me.GroupBox20 = New System.Windows.Forms.GroupBox
        Me.Label83 = New System.Windows.Forms.Label
        Me.dgFilterStrings = New System.Windows.Forms.DataGridView
        Me.GroupBox19 = New System.Windows.Forms.GroupBox
        Me.dgExcludedFolderStrings = New System.Windows.Forms.DataGridView
        Me.Label44 = New System.Windows.Forms.Label
        Me.Label45 = New System.Windows.Forms.Label
        Me.dgExcludedFileStrings = New System.Windows.Forms.DataGridView
        Me.DatabaseFields = New System.Windows.Forms.TabPage
        Me.cbMasterTitle = New System.Windows.Forms.ComboBox
        Me.lblMasterTitle = New System.Windows.Forms.Label
        Me.lblPicturePrefix = New System.Windows.Forms.Label
        Me.txtPictureFilenamePrefix = New System.Windows.Forms.TextBox
        Me.cbPictureHandling = New System.Windows.Forms.ComboBox
        Me.lblPictureHandling = New System.Windows.Forms.Label
        Me.cbGroupNameAppliesTo = New System.Windows.Forms.ComboBox
        Me.lblGroupNameAppliesTo = New System.Windows.Forms.Label
        Me.chkFolderNameIsGroupName = New System.Windows.Forms.CheckBox
        Me.cbCheckHandling = New System.Windows.Forms.ComboBox
        Me.lblCheckFieldHandling = New System.Windows.Forms.Label
        Me.lblInternetLookupRequired = New System.Windows.Forms.Label
        Me.cbDateHandling = New System.Windows.Forms.ComboBox
        Me.Label30 = New System.Windows.Forms.Label
        Me.GroupBox16 = New System.Windows.Forms.GroupBox
        Me.Label33 = New System.Windows.Forms.Label
        Me.Label32 = New System.Windows.Forms.Label
        Me.btnDBFieldsSelectNoInternet = New System.Windows.Forms.Button
        Me.Label31 = New System.Windows.Forms.Label
        Me.btnDBFieldsSelectAllInternet = New System.Windows.Forms.Button
        Me.btnDBFieldsSelectNoMedia = New System.Windows.Forms.Button
        Me.btnDBFieldsSelectNone = New System.Windows.Forms.Button
        Me.btnDBFieldsSelectAllMedia = New System.Windows.Forms.Button
        Me.btnDBFieldsSelectAll = New System.Windows.Forms.Button
        Me.cbDatabaseFields = New System.Windows.Forms.CheckedListBox
        Me.Label34 = New System.Windows.Forms.Label
        Me.Manual = New System.Windows.Forms.TabPage
        Me.btnManualCancel = New System.Windows.Forms.Button
        Me.grpManualInternetLookupSettings = New System.Windows.Forms.GroupBox
        Me.txtManualInternetParserPathDisplay = New System.Windows.Forms.TextBox
        Me.Button2 = New System.Windows.Forms.Button
        Me.Label42 = New System.Windows.Forms.Label
        Me.btnManualSelectInternetParserPath = New System.Windows.Forms.Button
        Me.cbManualInternetLookupBehaviour = New System.Windows.Forms.ComboBox
        Me.txtManualInternetParserPath = New System.Windows.Forms.TextBox
        Me.Label41 = New System.Windows.Forms.Label
        Me.btnShowHideLogTest = New System.Windows.Forms.Button
        Me.btnManualApplyChanges = New System.Windows.Forms.Button
        Me.btnManualDoTest = New System.Windows.Forms.Button
        Me.grpManualUpdatesParameters = New System.Windows.Forms.GroupBox
        Me.cbManualParameterAndOr = New System.Windows.Forms.ComboBox
        Me.lblManualParametersValue2 = New System.Windows.Forms.Label
        Me.lblManualParametersOperator2 = New System.Windows.Forms.Label
        Me.txtManualParameterValue2 = New System.Windows.Forms.TextBox
        Me.cbManualParameterOperator2 = New System.Windows.Forms.ComboBox
        Me.lblManualParametersField2 = New System.Windows.Forms.Label
        Me.cbManualParameterFieldList2 = New System.Windows.Forms.ComboBox
        Me.Label21 = New System.Windows.Forms.Label
        Me.chkManualParametersUpdateAll = New System.Windows.Forms.CheckBox
        Me.lblManualParametersValue1 = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.txtManualParameterValue1 = New System.Windows.Forms.TextBox
        Me.cbManualParameterOperator1 = New System.Windows.Forms.ComboBox
        Me.Label18 = New System.Windows.Forms.Label
        Me.cbManualParameterFieldList1 = New System.Windows.Forms.ComboBox
        Me.GroupBox10 = New System.Windows.Forms.GroupBox
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData = New System.Windows.Forms.CheckBox
        Me.chkManualUpdateRecordsOnlyMissingData = New System.Windows.Forms.CheckBox
        Me.txtManualOldValue = New System.Windows.Forms.TextBox
        Me.lblManualEnterOldValue = New System.Windows.Forms.Label
        Me.chkManualMissingTrailer = New System.Windows.Forms.CheckBox
        Me.chkManualMissingFanartDownload = New System.Windows.Forms.CheckBox
        Me.lblManualDatabaseFieldsPrompt = New System.Windows.Forms.Label
        Me.lblManualSelectField = New System.Windows.Forms.Label
        Me.cbManualSelectField = New System.Windows.Forms.ComboBox
        Me.txtManualNewValue = New System.Windows.Forms.TextBox
        Me.lblManualEnterNewValue = New System.Windows.Forms.Label
        Me.cbManualSelectOperation = New System.Windows.Forms.ComboBox
        Me.GroupBox9 = New System.Windows.Forms.GroupBox
        Me.btnManualExcludedMoviesFileDelete = New System.Windows.Forms.Button
        Me.btnManualExcludedMoviesFileShow = New System.Windows.Forms.Button
        Me.txtManualXMLPath = New System.Windows.Forms.TextBox
        Me.Label20 = New System.Windows.Forms.Label
        Me.Label40 = New System.Windows.Forms.Label
        Me.txtManualExcludedMoviesPath = New System.Windows.Forms.TextBox
        Me.btnManualSelectXMLFile = New System.Windows.Forms.Button
        Me.btnManualSelectExcludedMoviesFile = New System.Windows.Forms.Button
        Me.Test = New System.Windows.Forms.TabPage
        Me.Label47 = New System.Windows.Forms.Label
        Me.GroupBox18 = New System.Windows.Forms.GroupBox
        Me.txtSampleTextLanguageList = New System.Windows.Forms.TextBox
        Me.txtSampleTextCodecList = New System.Windows.Forms.TextBox
        Me.Label39 = New System.Windows.Forms.Label
        Me.Label38 = New System.Windows.Forms.Label
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.txtSampleFileLength = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.txtSampleFileSize = New System.Windows.Forms.TextBox
        Me.Label9 = New System.Windows.Forms.Label
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.Label37 = New System.Windows.Forms.Label
        Me.Label36 = New System.Windows.Forms.Label
        Me.txtSampleAudioLanguageList = New System.Windows.Forms.TextBox
        Me.txtSampleAudioStreamList = New System.Windows.Forms.TextBox
        Me.txtSampleAudioStreamCount = New System.Windows.Forms.TextBox
        Me.Label35 = New System.Windows.Forms.Label
        Me.txtSampleAudioBitrate = New System.Windows.Forms.TextBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.txtSampleAudioCodec = New System.Windows.Forms.TextBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.btnTestAnalyse = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.txtSampleVideoResolution = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.txtSampleVideoFramerate = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtSampleVideoBitrate = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.txtSampleVideoCodec = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.btnGetSampleFile = New System.Windows.Forms.Button
        Me.txtSampleFile = New System.Windows.Forms.TextBox
        Me.ViewCollection = New System.Windows.Forms.TabPage
        Me.GroupBoxMovieDetails = New System.Windows.Forms.GroupBox
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.VideoBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.TextBox31 = New System.Windows.Forms.TextBox
        Me.TextBox2 = New System.Windows.Forms.TextBox
        Me.Label82 = New System.Windows.Forms.Label
        Me.Label49 = New System.Windows.Forms.Label
        Me.TextBox30 = New System.Windows.Forms.TextBox
        Me.TextBox5 = New System.Windows.Forms.TextBox
        Me.Label81 = New System.Windows.Forms.Label
        Me.Label50 = New System.Windows.Forms.Label
        Me.Label80 = New System.Windows.Forms.Label
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox
        Me.TextBox29 = New System.Windows.Forms.TextBox
        Me.RichTextBox2 = New System.Windows.Forms.RichTextBox
        Me.GroupBox14 = New System.Windows.Forms.GroupBox
        Me.Label53 = New System.Windows.Forms.Label
        Me.Label52 = New System.Windows.Forms.Label
        Me.Label51 = New System.Windows.Forms.Label
        Me.TextBox7 = New System.Windows.Forms.TextBox
        Me.TextBox6 = New System.Windows.Forms.TextBox
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.Label78 = New System.Windows.Forms.Label
        Me.RichTextBox3 = New System.Windows.Forms.RichTextBox
        Me.TextBox28 = New System.Windows.Forms.TextBox
        Me.TextBox3 = New System.Windows.Forms.TextBox
        Me.Label77 = New System.Windows.Forms.Label
        Me.Label54 = New System.Windows.Forms.Label
        Me.TextBox27 = New System.Windows.Forms.TextBox
        Me.TextBox4 = New System.Windows.Forms.TextBox
        Me.Label74 = New System.Windows.Forms.Label
        Me.Label55 = New System.Windows.Forms.Label
        Me.TextBox24 = New System.Windows.Forms.TextBox
        Me.TextBox9 = New System.Windows.Forms.TextBox
        Me.Label73 = New System.Windows.Forms.Label
        Me.Label57 = New System.Windows.Forms.Label
        Me.TextBox23 = New System.Windows.Forms.TextBox
        Me.TextBox8 = New System.Windows.Forms.TextBox
        Me.GroupBox22 = New System.Windows.Forms.GroupBox
        Me.Label75 = New System.Windows.Forms.Label
        Me.Label76 = New System.Windows.Forms.Label
        Me.TextBox25 = New System.Windows.Forms.TextBox
        Me.TextBox26 = New System.Windows.Forms.TextBox
        Me.Label56 = New System.Windows.Forms.Label
        Me.GroupBox21 = New System.Windows.Forms.GroupBox
        Me.Label72 = New System.Windows.Forms.Label
        Me.TextBox22 = New System.Windows.Forms.TextBox
        Me.Label71 = New System.Windows.Forms.Label
        Me.TextBox21 = New System.Windows.Forms.TextBox
        Me.Label69 = New System.Windows.Forms.Label
        Me.Label70 = New System.Windows.Forms.Label
        Me.TextBox19 = New System.Windows.Forms.TextBox
        Me.TextBox20 = New System.Windows.Forms.TextBox
        Me.Label58 = New System.Windows.Forms.Label
        Me.GroupBox15 = New System.Windows.Forms.GroupBox
        Me.Label68 = New System.Windows.Forms.Label
        Me.Label67 = New System.Windows.Forms.Label
        Me.TextBox17 = New System.Windows.Forms.TextBox
        Me.TextBox18 = New System.Windows.Forms.TextBox
        Me.TextBox10 = New System.Windows.Forms.TextBox
        Me.Label66 = New System.Windows.Forms.Label
        Me.Label60 = New System.Windows.Forms.Label
        Me.TextBox16 = New System.Windows.Forms.TextBox
        Me.TextBox11 = New System.Windows.Forms.TextBox
        Me.Label64 = New System.Windows.Forms.Label
        Me.Label61 = New System.Windows.Forms.Label
        Me.TextBox14 = New System.Windows.Forms.TextBox
        Me.TextBox13 = New System.Windows.Forms.TextBox
        Me.Label65 = New System.Windows.Forms.Label
        Me.Label63 = New System.Windows.Forms.Label
        Me.TextBox15 = New System.Windows.Forms.TextBox
        Me.TextBox12 = New System.Windows.Forms.TextBox
        Me.Label62 = New System.Windows.Forms.Label
        Me.XionPanel1 = New XionControls.XionPanel
        Me.DataGridView1 = New System.Windows.Forms.DataGridView
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Category = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Country = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Rating = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Label59 = New System.Windows.Forms.Label
        Me.VidéoBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.BindingNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel
        Me.BindingNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator
        Me.BindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox
        Me.BindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.BindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.VidéoBindingNavigatorSaveItem = New System.Windows.Forms.ToolStripButton
        Me.mnuFile = New System.Windows.Forms.MenuStrip
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.LoadConfigurationFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveConfigFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveConfigFileAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem
        Me.InternetLinksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MyFilmsWikiToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AMCUpdaterSourceforgeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AntMovieCatalogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MediaInfodllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.UserManualToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DebugToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.XMLToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MediaFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NonMediaFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TrailerFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.OrphanMediaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.OrphanNonMediaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.OrphanTrailerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MultiPartFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.OrphanAntToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MultiPartProcessedFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AntFieldsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NodesToProcessToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ListMediaInfoParamsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.ToolStripFixedText = New System.Windows.Forms.ToolStripStatusLabel
        Me.ToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel
        Me.ToolStripProgressBar = New System.Windows.Forms.ToolStripProgressBar
        Me.epInteractive = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.epManualUpdater = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog
        Me.epOptions = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.AntMovieCatalog = New AMCUpdater.AntMovieCatalog
        Me.ListVideos = New System.Windows.Forms.DataGridView
        Me.NumberDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.OriginalTitleDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.TranslatedTitleDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.YearDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DateAddedDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.TabControl1.SuspendLayout()
        Me.Interactive.SuspendLayout()
        Me.GroupBox24.SuspendLayout()
        Me.GroupBox23.SuspendLayout()
        Me.GroupBox27.SuspendLayout()
        Me.GroupBox26.SuspendLayout()
        Me.GroupBox17.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        Me.Options.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox25.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.GroupBox13.SuspendLayout()
        Me.GroupBox12.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.ScanFilters.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GroupBox20.SuspendLayout()
        CType(Me.dgFilterStrings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox19.SuspendLayout()
        CType(Me.dgExcludedFolderStrings, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgExcludedFileStrings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DatabaseFields.SuspendLayout()
        Me.GroupBox16.SuspendLayout()
        Me.Manual.SuspendLayout()
        Me.grpManualInternetLookupSettings.SuspendLayout()
        Me.grpManualUpdatesParameters.SuspendLayout()
        Me.GroupBox10.SuspendLayout()
        Me.GroupBox9.SuspendLayout()
        Me.Test.SuspendLayout()
        Me.GroupBox18.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.ViewCollection.SuspendLayout()
        Me.GroupBoxMovieDetails.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.VideoBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox14.SuspendLayout()
        Me.GroupBox22.SuspendLayout()
        Me.GroupBox21.SuspendLayout()
        Me.GroupBox15.SuspendLayout()
        Me.XionPanel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.VidéoBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.VidéoBindingNavigator.SuspendLayout()
        Me.mnuFile.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.epInteractive, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.epManualUpdater, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.epOptions, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AntMovieCatalog, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ListVideos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'FolderBrowserDialog1
        '
        Me.FolderBrowserDialog1.ShowNewFolderButton = False
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.Interactive)
        Me.TabControl1.Controls.Add(Me.Options)
        Me.TabControl1.Controls.Add(Me.ScanFilters)
        Me.TabControl1.Controls.Add(Me.DatabaseFields)
        Me.TabControl1.Controls.Add(Me.Manual)
        Me.TabControl1.Controls.Add(Me.Test)
        Me.TabControl1.Controls.Add(Me.ViewCollection)
        Me.TabControl1.Location = New System.Drawing.Point(0, 27)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(608, 606)
        Me.TabControl1.TabIndex = 1
        '
        'Interactive
        '
        Me.Interactive.Controls.Add(Me.GroupBox24)
        Me.Interactive.Controls.Add(Me.GroupBox23)
        Me.Interactive.Controls.Add(Me.GroupBox17)
        Me.Interactive.Controls.Add(Me.ToolStripProgressMessage)
        Me.Interactive.Controls.Add(Me.btnShowHideLog)
        Me.Interactive.Controls.Add(Me.GroupBox8)
        Me.Interactive.Location = New System.Drawing.Point(4, 22)
        Me.Interactive.Name = "Interactive"
        Me.Interactive.Padding = New System.Windows.Forms.Padding(3)
        Me.Interactive.Size = New System.Drawing.Size(600, 580)
        Me.Interactive.TabIndex = 0
        Me.Interactive.Text = "Interactive"
        Me.Interactive.UseVisualStyleBackColor = True
        '
        'GroupBox24
        '
        Me.GroupBox24.Controls.Add(Me.txtOverridePath)
        Me.GroupBox24.Controls.Add(Me.lblOverridePath)
        Me.GroupBox24.Controls.Add(Me.btnSelectMovieFolder)
        Me.GroupBox24.Controls.Add(Me.Label1)
        Me.GroupBox24.Controls.Add(Me.txtMovieFolder)
        Me.GroupBox24.Controls.Add(Me.Label17)
        Me.GroupBox24.Location = New System.Drawing.Point(11, 88)
        Me.GroupBox24.Name = "GroupBox24"
        Me.GroupBox24.Size = New System.Drawing.Size(572, 88)
        Me.GroupBox24.TabIndex = 106
        Me.GroupBox24.TabStop = False
        Me.GroupBox24.Text = "Movie Folder(s) ..."
        '
        'txtOverridePath
        '
        Me.txtOverridePath.Location = New System.Drawing.Point(103, 60)
        Me.txtOverridePath.Name = "txtOverridePath"
        Me.txtOverridePath.Size = New System.Drawing.Size(410, 20)
        Me.txtOverridePath.TabIndex = 40
        Me.ToolTip1.SetToolTip(Me.txtOverridePath, resources.GetString("txtOverridePath.ToolTip"))
        '
        'lblOverridePath
        '
        Me.lblOverridePath.AutoSize = True
        Me.lblOverridePath.Location = New System.Drawing.Point(11, 63)
        Me.lblOverridePath.Name = "lblOverridePath"
        Me.lblOverridePath.Size = New System.Drawing.Size(71, 13)
        Me.lblOverridePath.TabIndex = 41
        Me.lblOverridePath.Text = "Override path"
        '
        'btnSelectMovieFolder
        '
        Me.btnSelectMovieFolder.CausesValidation = False
        Me.btnSelectMovieFolder.Location = New System.Drawing.Point(519, 34)
        Me.btnSelectMovieFolder.Name = "btnSelectMovieFolder"
        Me.btnSelectMovieFolder.Size = New System.Drawing.Size(32, 20)
        Me.btnSelectMovieFolder.TabIndex = 1
        Me.btnSelectMovieFolder.Text = "..."
        Me.btnSelectMovieFolder.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(11, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(130, 13)
        Me.Label1.TabIndex = 28
        Me.Label1.Text = "Path to Movies Folder(s)  :"
        '
        'txtMovieFolder
        '
        Me.txtMovieFolder.Location = New System.Drawing.Point(12, 34)
        Me.txtMovieFolder.Name = "txtMovieFolder"
        Me.txtMovieFolder.Size = New System.Drawing.Size(501, 20)
        Me.txtMovieFolder.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.txtMovieFolder, "Enter the paths (local or UNC) to the folders you want to scan." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Separate multipl" & _
                "e folders with semi-colons.")
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(273, 19)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(202, 12)
        Me.Label17.TabIndex = 39
        Me.Label17.Text = "Note : Separate movie folders with semi-colons."
        '
        'GroupBox23
        '
        Me.GroupBox23.Controls.Add(Me.GroupBox27)
        Me.GroupBox23.Controls.Add(Me.txtParserFilePathDisplay)
        Me.GroupBox23.Controls.Add(Me.cbTitleHandling)
        Me.GroupBox23.Controls.Add(Me.GroupBox26)
        Me.GroupBox23.Controls.Add(Me.Label43)
        Me.GroupBox23.Controls.Add(Me.Button1)
        Me.GroupBox23.Controls.Add(Me.btnSelectParserFile)
        Me.GroupBox23.Controls.Add(Me.Label23)
        Me.GroupBox23.Controls.Add(Me.txtParserFilePath)
        Me.GroupBox23.Location = New System.Drawing.Point(11, 187)
        Me.GroupBox23.Name = "GroupBox23"
        Me.GroupBox23.Size = New System.Drawing.Size(572, 245)
        Me.GroupBox23.TabIndex = 104
        Me.GroupBox23.TabStop = False
        Me.GroupBox23.Text = "Internet Grabber Options ..."
        '
        'GroupBox27
        '
        Me.GroupBox27.Controls.Add(Me.chkDontAskInteractive)
        Me.GroupBox27.Controls.Add(Me.Label24)
        Me.GroupBox27.Controls.Add(Me.chkImportOnInternetFailIgnoreWhenInteractive)
        Me.GroupBox27.Controls.Add(Me.cbInternetLookupBehaviour)
        Me.GroupBox27.Controls.Add(Me.chkImportOnInternetFail)
        Me.GroupBox27.Location = New System.Drawing.Point(8, 70)
        Me.GroupBox27.Name = "GroupBox27"
        Me.GroupBox27.Size = New System.Drawing.Size(558, 78)
        Me.GroupBox27.TabIndex = 63
        Me.GroupBox27.TabStop = False
        Me.GroupBox27.Text = "Importer Options ..."
        '
        'chkDontAskInteractive
        '
        Me.chkDontAskInteractive.AutoSize = True
        Me.chkDontAskInteractive.Location = New System.Drawing.Point(304, 11)
        Me.chkDontAskInteractive.Name = "chkDontAskInteractive"
        Me.chkDontAskInteractive.Size = New System.Drawing.Size(129, 17)
        Me.chkDontAskInteractive.TabIndex = 59
        Me.chkDontAskInteractive.Text = "Don't ask, if no match"
        Me.ToolTip1.SetToolTip(Me.chkDontAskInteractive, resources.GetString("chkDontAskInteractive.ToolTip"))
        Me.chkDontAskInteractive.UseVisualStyleBackColor = True
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(6, 25)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(85, 26)
        Me.Label24.TabIndex = 51
        Me.Label24.Text = "Internet Lookup " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Behaviour"
        '
        'chkImportOnInternetFailIgnoreWhenInteractive
        '
        Me.chkImportOnInternetFailIgnoreWhenInteractive.AutoSize = True
        Me.chkImportOnInternetFailIgnoreWhenInteractive.Location = New System.Drawing.Point(304, 57)
        Me.chkImportOnInternetFailIgnoreWhenInteractive.Name = "chkImportOnInternetFailIgnoreWhenInteractive"
        Me.chkImportOnInternetFailIgnoreWhenInteractive.Size = New System.Drawing.Size(196, 17)
        Me.chkImportOnInternetFailIgnoreWhenInteractive.TabIndex = 61
        Me.chkImportOnInternetFailIgnoreWhenInteractive.Text = "Don't Import File in Interactive Mode"
        Me.ToolTip1.SetToolTip(Me.chkImportOnInternetFailIgnoreWhenInteractive, "If you want AMCupdater to import files in background mode (MyFIlms global update)" & _
                ", " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "but do NOT want AMCupdater to import files for AMCupdater interactive GUI mo" & _
                "de, " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "you can set this option.")
        Me.chkImportOnInternetFailIgnoreWhenInteractive.UseVisualStyleBackColor = True
        '
        'cbInternetLookupBehaviour
        '
        Me.cbInternetLookupBehaviour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbInternetLookupBehaviour.FormattingEnabled = True
        Me.cbInternetLookupBehaviour.Location = New System.Drawing.Point(95, 30)
        Me.cbInternetLookupBehaviour.Name = "cbInternetLookupBehaviour"
        Me.cbInternetLookupBehaviour.Size = New System.Drawing.Size(198, 21)
        Me.cbInternetLookupBehaviour.TabIndex = 9
        Me.ToolTip1.SetToolTip(Me.cbInternetLookupBehaviour, resources.GetString("cbInternetLookupBehaviour.ToolTip"))
        '
        'chkImportOnInternetFail
        '
        Me.chkImportOnInternetFail.AutoSize = True
        Me.chkImportOnInternetFail.Location = New System.Drawing.Point(304, 34)
        Me.chkImportOnInternetFail.Name = "chkImportOnInternetFail"
        Me.chkImportOnInternetFail.Size = New System.Drawing.Size(184, 17)
        Me.chkImportOnInternetFail.TabIndex = 0
        Me.chkImportOnInternetFail.Text = "Import File if Internet Lookup Fails"
        Me.ToolTip1.SetToolTip(Me.chkImportOnInternetFail, resources.GetString("chkImportOnInternetFail.ToolTip"))
        Me.chkImportOnInternetFail.UseVisualStyleBackColor = True
        '
        'txtParserFilePathDisplay
        '
        Me.txtParserFilePathDisplay.Enabled = False
        Me.txtParserFilePathDisplay.Location = New System.Drawing.Point(103, 17)
        Me.txtParserFilePathDisplay.Name = "txtParserFilePathDisplay"
        Me.txtParserFilePathDisplay.Size = New System.Drawing.Size(135, 20)
        Me.txtParserFilePathDisplay.TabIndex = 62
        '
        'cbTitleHandling
        '
        Me.cbTitleHandling.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbTitleHandling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbTitleHandling.FormattingEnabled = True
        Me.cbTitleHandling.Items.AddRange(New Object() {"File Name", "Folder Name", "Relative Name", "File Name + Internet Lookup", "Folder Name + Internet Lookup", "Relative Name + Internet Lookup"})
        Me.cbTitleHandling.Location = New System.Drawing.Point(103, 43)
        Me.cbTitleHandling.Name = "cbTitleHandling"
        Me.cbTitleHandling.Size = New System.Drawing.Size(198, 21)
        Me.cbTitleHandling.TabIndex = 40
        Me.ToolTip1.SetToolTip(Me.cbTitleHandling, resources.GetString("cbTitleHandling.ToolTip"))
        '
        'GroupBox26
        '
        Me.GroupBox26.Controls.Add(Me.Label87)
        Me.GroupBox26.Controls.Add(Me.Label86)
        Me.GroupBox26.Controls.Add(Me.Label85)
        Me.GroupBox26.Controls.Add(Me.Label84)
        Me.GroupBox26.Controls.Add(Me.chkGrabberOverrideTitleLimit)
        Me.GroupBox26.Controls.Add(Me.chkGrabberOverridePersonLimit)
        Me.GroupBox26.Controls.Add(Me.chkGrabberOverrideGetRoles)
        Me.GroupBox26.Controls.Add(Me.chkGrabberOverrideLanguage)
        Me.GroupBox26.Location = New System.Drawing.Point(8, 153)
        Me.GroupBox26.Name = "GroupBox26"
        Me.GroupBox26.Size = New System.Drawing.Size(558, 86)
        Me.GroupBox26.TabIndex = 60
        Me.GroupBox26.TabStop = False
        Me.GroupBox26.Text = "Grabber Override Options ..."
        Me.ToolTip1.SetToolTip(Me.GroupBox26, resources.GetString("GroupBox26.ToolTip"))
        '
        'Label87
        '
        Me.Label87.AutoSize = True
        Me.Label87.Location = New System.Drawing.Point(339, 62)
        Me.Label87.Name = "Label87"
        Me.Label87.Size = New System.Drawing.Size(66, 13)
        Me.Label87.TabIndex = 7
        Me.Label87.Text = "Limit # Titles"
        '
        'Label86
        '
        Me.Label86.AutoSize = True
        Me.Label86.Location = New System.Drawing.Point(339, 38)
        Me.Label86.Name = "Label86"
        Me.Label86.Size = New System.Drawing.Size(79, 13)
        Me.Label86.TabIndex = 6
        Me.Label86.Text = "Limit # Persons"
        '
        'Label85
        '
        Me.Label85.AutoSize = True
        Me.Label85.Location = New System.Drawing.Point(339, 14)
        Me.Label85.Name = "Label85"
        Me.Label85.Size = New System.Drawing.Size(82, 13)
        Me.Label85.TabIndex = 5
        Me.Label85.Text = "Get Actor Roles"
        '
        'Label84
        '
        Me.Label84.AutoSize = True
        Me.Label84.Location = New System.Drawing.Point(12, 27)
        Me.Label84.Name = "Label84"
        Me.Label84.Size = New System.Drawing.Size(68, 39)
        Me.Label84.TabIndex = 4
        Me.Label84.Text = "Preferred " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Languages /" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & " Countries"
        '
        'chkGrabberOverrideTitleLimit
        '
        Me.chkGrabberOverrideTitleLimit.FormattingEnabled = True
        Me.chkGrabberOverrideTitleLimit.Items.AddRange(New Object() {"0", "1", "2", "3", "4", "5", "10", "15", "20", "999"})
        Me.chkGrabberOverrideTitleLimit.Location = New System.Drawing.Point(436, 59)
        Me.chkGrabberOverrideTitleLimit.Name = "chkGrabberOverrideTitleLimit"
        Me.chkGrabberOverrideTitleLimit.Size = New System.Drawing.Size(69, 21)
        Me.chkGrabberOverrideTitleLimit.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.chkGrabberOverrideTitleLimit, "Limits the number of translated titles grabbed." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "You may also set a preferred lan" & _
                "guage/country " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "for grabbing in the override options.")
        '
        'chkGrabberOverridePersonLimit
        '
        Me.chkGrabberOverridePersonLimit.FormattingEnabled = True
        Me.chkGrabberOverridePersonLimit.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "10", "15", "20", "999"})
        Me.chkGrabberOverridePersonLimit.Location = New System.Drawing.Point(436, 35)
        Me.chkGrabberOverridePersonLimit.Name = "chkGrabberOverridePersonLimit"
        Me.chkGrabberOverridePersonLimit.Size = New System.Drawing.Size(69, 21)
        Me.chkGrabberOverridePersonLimit.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.chkGrabberOverridePersonLimit, "Limits the number of person names grabbed." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "This settings applies to all fields t" & _
                "hat grab person names, " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "like actors, producers, writers, directors.")
        '
        'chkGrabberOverrideGetRoles
        '
        Me.chkGrabberOverrideGetRoles.FormattingEnabled = True
        Me.chkGrabberOverrideGetRoles.Items.AddRange(New Object() {"true", "false"})
        Me.chkGrabberOverrideGetRoles.Location = New System.Drawing.Point(436, 11)
        Me.chkGrabberOverrideGetRoles.Name = "chkGrabberOverrideGetRoles"
        Me.chkGrabberOverrideGetRoles.Size = New System.Drawing.Size(69, 21)
        Me.chkGrabberOverrideGetRoles.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.chkGrabberOverrideGetRoles, "If checked, add Roles to actor infos." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Depends on the grabber script supporting t" & _
                "hat option.")
        '
        'chkGrabberOverrideLanguage
        '
        Me.chkGrabberOverrideLanguage.FormattingEnabled = True
        Me.chkGrabberOverrideLanguage.Items.AddRange(New Object() {"Argentina", "Australia", "Austria", "Belgium", "Brazil", "Canada", "Chile", "Croatia", "Czech Republic", "Denmark", "Estonia", "Finland", "France", "Germany", "Greece", "Hong Kong", "Hungary", "Iceland", "India", "Ireland", "Israel", "Italy", "Japan", "Malaysia", "Mexico", "Netherlands", "New Zealand", "Norway", "Peru", "Philippines", "Poland", "Portugal", "Romania", "Russia", "Singapore", "Slovakia", "Slovenia", "South Africa", "South Korea", "Spain", "Sweden", "Switzerland", "Turkey", "UK", "Uruguay", "USA"})
        Me.chkGrabberOverrideLanguage.Location = New System.Drawing.Point(95, 38)
        Me.chkGrabberOverrideLanguage.Name = "chkGrabberOverrideLanguage"
        Me.chkGrabberOverrideLanguage.Size = New System.Drawing.Size(198, 21)
        Me.chkGrabberOverrideLanguage.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.chkGrabberOverrideLanguage, resources.GetString("chkGrabberOverrideLanguage.ToolTip"))
        '
        'Label43
        '
        Me.Label43.AutoSize = True
        Me.Label43.Location = New System.Drawing.Point(11, 41)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(86, 26)
        Me.Label43.TabIndex = 39
        Me.Label43.Text = "Title and " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Search Handling"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(377, 17)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(116, 20)
        Me.Button1.TabIndex = 51
        Me.Button1.Text = "Grabber Options"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btnSelectParserFile
        '
        Me.btnSelectParserFile.CausesValidation = False
        Me.btnSelectParserFile.Location = New System.Drawing.Point(320, 17)
        Me.btnSelectParserFile.Name = "btnSelectParserFile"
        Me.btnSelectParserFile.Size = New System.Drawing.Size(32, 20)
        Me.btnSelectParserFile.TabIndex = 5
        Me.btnSelectParserFile.Text = "..."
        Me.btnSelectParserFile.UseVisualStyleBackColor = True
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Location = New System.Drawing.Point(11, 20)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(73, 13)
        Me.Label23.TabIndex = 42
        Me.Label23.Text = "Grabber script"
        '
        'txtParserFilePath
        '
        Me.txtParserFilePath.Location = New System.Drawing.Point(103, 17)
        Me.txtParserFilePath.Name = "txtParserFilePath"
        Me.txtParserFilePath.Size = New System.Drawing.Size(196, 20)
        Me.txtParserFilePath.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.txtParserFilePath, resources.GetString("txtParserFilePath.ToolTip"))
        Me.txtParserFilePath.Visible = False
        '
        'GroupBox17
        '
        Me.GroupBox17.Controls.Add(Me.chkRescanMovedFiles)
        Me.GroupBox17.Controls.Add(Me.chkProhibitInternetLookup)
        Me.GroupBox17.Controls.Add(Me.chkPurgeMissing)
        Me.GroupBox17.Controls.Add(Me.chkOverwriteXML)
        Me.GroupBox17.Controls.Add(Me.chkBackupXMLFirst)
        Me.GroupBox17.Location = New System.Drawing.Point(11, 6)
        Me.GroupBox17.Name = "GroupBox17"
        Me.GroupBox17.Size = New System.Drawing.Size(572, 67)
        Me.GroupBox17.TabIndex = 103
        Me.GroupBox17.TabStop = False
        Me.GroupBox17.Text = "Options for File Handling ..."
        '
        'chkRescanMovedFiles
        '
        Me.chkRescanMovedFiles.AutoSize = True
        Me.chkRescanMovedFiles.Location = New System.Drawing.Point(224, 42)
        Me.chkRescanMovedFiles.Name = "chkRescanMovedFiles"
        Me.chkRescanMovedFiles.Size = New System.Drawing.Size(128, 17)
        Me.chkRescanMovedFiles.TabIndex = 8
        Me.chkRescanMovedFiles.Text = "Re-Scan Moved Files"
        Me.ToolTip1.SetToolTip(Me.chkRescanMovedFiles, "Forces a refresh of file-based and online data for a file when it is found to hav" & _
                "e moved.")
        Me.chkRescanMovedFiles.UseVisualStyleBackColor = True
        '
        'chkProhibitInternetLookup
        '
        Me.chkProhibitInternetLookup.AutoSize = True
        Me.chkProhibitInternetLookup.Location = New System.Drawing.Point(406, 19)
        Me.chkProhibitInternetLookup.Name = "chkProhibitInternetLookup"
        Me.chkProhibitInternetLookup.Size = New System.Drawing.Size(139, 17)
        Me.chkProhibitInternetLookup.TabIndex = 4
        Me.chkProhibitInternetLookup.Text = "Prohibit Internet Lookup"
        Me.ToolTip1.SetToolTip(Me.chkProhibitInternetLookup, "Global override option to disable all Internet-based functionality.")
        Me.chkProhibitInternetLookup.UseVisualStyleBackColor = True
        '
        'chkPurgeMissing
        '
        Me.chkPurgeMissing.AutoSize = True
        Me.chkPurgeMissing.Location = New System.Drawing.Point(224, 19)
        Me.chkPurgeMissing.Name = "chkPurgeMissing"
        Me.chkPurgeMissing.Size = New System.Drawing.Size(135, 17)
        Me.chkPurgeMissing.TabIndex = 5
        Me.chkPurgeMissing.Text = "Purge Orphan Records"
        Me.ToolTip1.SetToolTip(Me.chkPurgeMissing, resources.GetString("chkPurgeMissing.ToolTip"))
        Me.chkPurgeMissing.UseVisualStyleBackColor = True
        '
        'chkOverwriteXML
        '
        Me.chkOverwriteXML.AutoSize = True
        Me.chkOverwriteXML.Location = New System.Drawing.Point(12, 19)
        Me.chkOverwriteXML.Name = "chkOverwriteXML"
        Me.chkOverwriteXML.Size = New System.Drawing.Size(115, 17)
        Me.chkOverwriteXML.TabIndex = 6
        Me.chkOverwriteXML.Text = "Overwrite XML File"
        Me.ToolTip1.SetToolTip(Me.chkOverwriteXML, "Directly over-write the given Ant Movie Catalog database file.")
        Me.chkOverwriteXML.UseVisualStyleBackColor = True
        '
        'chkBackupXMLFirst
        '
        Me.chkBackupXMLFirst.AutoSize = True
        Me.chkBackupXMLFirst.Checked = True
        Me.chkBackupXMLFirst.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkBackupXMLFirst.Enabled = False
        Me.chkBackupXMLFirst.Location = New System.Drawing.Point(12, 42)
        Me.chkBackupXMLFirst.Name = "chkBackupXMLFirst"
        Me.chkBackupXMLFirst.Size = New System.Drawing.Size(107, 17)
        Me.chkBackupXMLFirst.TabIndex = 7
        Me.chkBackupXMLFirst.Text = "Backup XML File"
        Me.ToolTip1.SetToolTip(Me.chkBackupXMLFirst, "Back up the Ant Movie Catalog database file before modifying it.")
        Me.chkBackupXMLFirst.UseVisualStyleBackColor = True
        '
        'ToolStripProgressMessage
        '
        Me.ToolStripProgressMessage.AutoSize = True
        Me.ToolStripProgressMessage.Location = New System.Drawing.Point(16, 552)
        Me.ToolStripProgressMessage.Name = "ToolStripProgressMessage"
        Me.ToolStripProgressMessage.Size = New System.Drawing.Size(35, 13)
        Me.ToolStripProgressMessage.TabIndex = 55
        Me.ToolStripProgressMessage.Text = "status"
        Me.ToolStripProgressMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnShowHideLog
        '
        Me.btnShowHideLog.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnShowHideLog.Location = New System.Drawing.Point(491, 547)
        Me.btnShowHideLog.Name = "btnShowHideLog"
        Me.btnShowHideLog.Size = New System.Drawing.Size(92, 22)
        Me.btnShowHideLog.TabIndex = 100
        Me.btnShowHideLog.Text = "Show Log >>"
        Me.btnShowHideLog.UseVisualStyleBackColor = True
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.Label16)
        Me.GroupBox8.Controls.Add(Me.btnParseXML)
        Me.GroupBox8.Controls.Add(Me.btnFindOrphans)
        Me.GroupBox8.Controls.Add(Me.btnCancelProcessing)
        Me.GroupBox8.Controls.Add(Me.btnUpdateXML)
        Me.GroupBox8.Controls.Add(Me.btnProcessMovieList)
        Me.GroupBox8.Controls.Add(Me.btnJustDoIt)
        Me.GroupBox8.Location = New System.Drawing.Point(11, 438)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(572, 105)
        Me.GroupBox8.TabIndex = 2
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "Begin Import ..."
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(326, 16)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(178, 13)
        Me.Label16.TabIndex = 66
        Me.Label16.Text = "Or...  Run process steps individually:"
        '
        'btnParseXML
        '
        Me.btnParseXML.Location = New System.Drawing.Point(301, 36)
        Me.btnParseXML.Name = "btnParseXML"
        Me.btnParseXML.Size = New System.Drawing.Size(106, 23)
        Me.btnParseXML.TabIndex = 2
        Me.btnParseXML.Text = "1 - Scan Movie DB"
        Me.btnParseXML.UseVisualStyleBackColor = True
        '
        'btnFindOrphans
        '
        Me.btnFindOrphans.Enabled = False
        Me.btnFindOrphans.Location = New System.Drawing.Point(301, 66)
        Me.btnFindOrphans.Name = "btnFindOrphans"
        Me.btnFindOrphans.Size = New System.Drawing.Size(106, 23)
        Me.btnFindOrphans.TabIndex = 4
        Me.btnFindOrphans.Text = "3 - Find Orphans"
        Me.btnFindOrphans.UseVisualStyleBackColor = True
        '
        'btnCancelProcessing
        '
        Me.btnCancelProcessing.Enabled = False
        Me.btnCancelProcessing.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancelProcessing.Image = CType(resources.GetObject("btnCancelProcessing.Image"), System.Drawing.Image)
        Me.btnCancelProcessing.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnCancelProcessing.Location = New System.Drawing.Point(139, 32)
        Me.btnCancelProcessing.Name = "btnCancelProcessing"
        Me.btnCancelProcessing.Size = New System.Drawing.Size(99, 58)
        Me.btnCancelProcessing.TabIndex = 1
        Me.btnCancelProcessing.Text = "Cancel"
        Me.btnCancelProcessing.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCancelProcessing.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.btnCancelProcessing.UseVisualStyleBackColor = True
        '
        'btnUpdateXML
        '
        Me.btnUpdateXML.Enabled = False
        Me.btnUpdateXML.Location = New System.Drawing.Point(413, 66)
        Me.btnUpdateXML.Name = "btnUpdateXML"
        Me.btnUpdateXML.Size = New System.Drawing.Size(117, 23)
        Me.btnUpdateXML.TabIndex = 5
        Me.btnUpdateXML.Text = "4 - Update Movie DB"
        Me.btnUpdateXML.UseVisualStyleBackColor = True
        '
        'btnProcessMovieList
        '
        Me.btnProcessMovieList.Enabled = False
        Me.btnProcessMovieList.Location = New System.Drawing.Point(413, 36)
        Me.btnProcessMovieList.Name = "btnProcessMovieList"
        Me.btnProcessMovieList.Size = New System.Drawing.Size(117, 23)
        Me.btnProcessMovieList.TabIndex = 3
        Me.btnProcessMovieList.Text = "2. Scan Movie Files"
        Me.btnProcessMovieList.UseVisualStyleBackColor = True
        '
        'btnJustDoIt
        '
        Me.btnJustDoIt.Image = CType(resources.GetObject("btnJustDoIt.Image"), System.Drawing.Image)
        Me.btnJustDoIt.Location = New System.Drawing.Point(13, 32)
        Me.btnJustDoIt.Name = "btnJustDoIt"
        Me.btnJustDoIt.Size = New System.Drawing.Size(103, 57)
        Me.btnJustDoIt.TabIndex = 0
        Me.btnJustDoIt.Text = "Begin Import"
        Me.btnJustDoIt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnJustDoIt.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.btnJustDoIt.UseVisualStyleBackColor = True
        '
        'Options
        '
        Me.Options.Controls.Add(Me.GroupBox4)
        Me.Options.Controls.Add(Me.GroupBox7)
        Me.Options.Controls.Add(Me.GroupBox13)
        Me.Options.Controls.Add(Me.GroupBox12)
        Me.Options.Controls.Add(Me.GroupBox5)
        Me.Options.Location = New System.Drawing.Point(4, 22)
        Me.Options.Name = "Options"
        Me.Options.Padding = New System.Windows.Forms.Padding(3)
        Me.Options.Size = New System.Drawing.Size(600, 580)
        Me.Options.TabIndex = 1
        Me.Options.Text = "Options"
        Me.Options.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.chkParseTrailers)
        Me.GroupBox4.Controls.Add(Me.GroupBox25)
        Me.GroupBox4.Controls.Add(Me.txtTrailerIentificationStrings)
        Me.GroupBox4.Controls.Add(Me.Label79)
        Me.GroupBox4.Controls.Add(Me.txtDefaultFileTypesNonMedia)
        Me.GroupBox4.Controls.Add(Me.Label14)
        Me.GroupBox4.Controls.Add(Me.txtDefaultFileTypes)
        Me.GroupBox4.Controls.Add(Me.Label15)
        Me.GroupBox4.Location = New System.Drawing.Point(8, 195)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(575, 138)
        Me.GroupBox4.TabIndex = 106
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "File Type Identification ..."
        '
        'chkParseTrailers
        '
        Me.chkParseTrailers.AutoSize = True
        Me.chkParseTrailers.Enabled = False
        Me.chkParseTrailers.Location = New System.Drawing.Point(377, 112)
        Me.chkParseTrailers.Name = "chkParseTrailers"
        Me.chkParseTrailers.Size = New System.Drawing.Size(134, 17)
        Me.chkParseTrailers.TabIndex = 31
        Me.chkParseTrailers.Text = "Check for Trailers (wip)"
        Me.chkParseTrailers.UseVisualStyleBackColor = True
        Me.chkParseTrailers.Visible = False
        '
        'GroupBox25
        '
        Me.GroupBox25.Controls.Add(Me.chkCheckDVDFolders)
        Me.GroupBox25.Controls.Add(Me.chkParseSubtitleFiles)
        Me.GroupBox25.Controls.Add(Me.chkParsePlaylistFiles)
        Me.GroupBox25.Location = New System.Drawing.Point(367, 16)
        Me.GroupBox25.Name = "GroupBox25"
        Me.GroupBox25.Size = New System.Drawing.Size(187, 91)
        Me.GroupBox25.TabIndex = 31
        Me.GroupBox25.TabStop = False
        Me.GroupBox25.Text = "Media Scan Options ..."
        '
        'chkCheckDVDFolders
        '
        Me.chkCheckDVDFolders.AutoSize = True
        Me.chkCheckDVDFolders.Location = New System.Drawing.Point(10, 23)
        Me.chkCheckDVDFolders.Name = "chkCheckDVDFolders"
        Me.chkCheckDVDFolders.Size = New System.Drawing.Size(132, 17)
        Me.chkCheckDVDFolders.TabIndex = 10
        Me.chkCheckDVDFolders.Text = "Check for DVD folders"
        Me.ToolTip1.SetToolTip(Me.chkCheckDVDFolders, "Enable this option to search for DVD rips." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "The system looks for a VIDEO_TS.IFO f" & _
                "ile and attempts to work out" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "the movie title from the folder structure.")
        Me.chkCheckDVDFolders.UseVisualStyleBackColor = True
        '
        'chkParseSubtitleFiles
        '
        Me.chkParseSubtitleFiles.AutoSize = True
        Me.chkParseSubtitleFiles.Location = New System.Drawing.Point(10, 46)
        Me.chkParseSubtitleFiles.Name = "chkParseSubtitleFiles"
        Me.chkParseSubtitleFiles.Size = New System.Drawing.Size(145, 17)
        Me.chkParseSubtitleFiles.TabIndex = 30
        Me.chkParseSubtitleFiles.Text = "Try to Parse Subtitle Files"
        Me.ToolTip1.SetToolTip(Me.chkParseSubtitleFiles, resources.GetString("chkParseSubtitleFiles.ToolTip"))
        Me.chkParseSubtitleFiles.UseVisualStyleBackColor = True
        '
        'chkParsePlaylistFiles
        '
        Me.chkParsePlaylistFiles.AutoSize = True
        Me.chkParsePlaylistFiles.Enabled = False
        Me.chkParsePlaylistFiles.Location = New System.Drawing.Point(10, 69)
        Me.chkParsePlaylistFiles.Name = "chkParsePlaylistFiles"
        Me.chkParsePlaylistFiles.Size = New System.Drawing.Size(156, 17)
        Me.chkParsePlaylistFiles.TabIndex = 11
        Me.chkParsePlaylistFiles.Text = "Check for Playlist Files (wip)"
        Me.chkParsePlaylistFiles.UseVisualStyleBackColor = True
        '
        'txtTrailerIentificationStrings
        '
        Me.txtTrailerIentificationStrings.Location = New System.Drawing.Point(12, 110)
        Me.txtTrailerIentificationStrings.Name = "txtTrailerIentificationStrings"
        Me.txtTrailerIentificationStrings.Size = New System.Drawing.Size(296, 20)
        Me.txtTrailerIentificationStrings.TabIndex = 29
        Me.ToolTip1.SetToolTip(Me.txtTrailerIentificationStrings, "if a file contains one of these strings, it will be identified as trailer instead" & _
                " of movie" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "several values are possible - use "";"" as delimiter.")
        '
        'Label79
        '
        Me.Label79.AutoSize = True
        Me.Label79.Location = New System.Drawing.Point(9, 94)
        Me.Label79.Name = "Label79"
        Me.Label79.Size = New System.Drawing.Size(183, 13)
        Me.Label79.TabIndex = 28
        Me.Label79.Text = "Accepted Trailer Identification Strings"
        '
        'txtDefaultFileTypesNonMedia
        '
        Me.txtDefaultFileTypesNonMedia.Location = New System.Drawing.Point(12, 71)
        Me.txtDefaultFileTypesNonMedia.Name = "txtDefaultFileTypesNonMedia"
        Me.txtDefaultFileTypesNonMedia.Size = New System.Drawing.Size(296, 20)
        Me.txtDefaultFileTypesNonMedia.TabIndex = 9
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(9, 55)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(268, 13)
        Me.Label14.TabIndex = 26
        Me.Label14.Text = "Accepted non-Media Files (import without media details)"
        '
        'txtDefaultFileTypes
        '
        Me.txtDefaultFileTypes.Location = New System.Drawing.Point(12, 32)
        Me.txtDefaultFileTypes.Name = "txtDefaultFileTypes"
        Me.txtDefaultFileTypes.Size = New System.Drawing.Size(296, 20)
        Me.txtDefaultFileTypes.TabIndex = 8
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(9, 16)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(257, 13)
        Me.Label15.TabIndex = 25
        Me.Label15.Text = "Accepted Media Files. Separate List with semi-colons"
        Me.ToolTip1.SetToolTip(Me.Label15, "Accepted Media Files." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Separate List with semi-colons, e.g. wmv,avi,mpg")
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.btnExcludeFileDelete)
        Me.GroupBox7.Controls.Add(Me.btnExcludeFileShow)
        Me.GroupBox7.Controls.Add(Me.btnSelectPersonArtworkFolder)
        Me.GroupBox7.Controls.Add(Me.Label28)
        Me.GroupBox7.Controls.Add(Me.txtPersonArtworkFolder)
        Me.GroupBox7.Controls.Add(Me.btnSelectFanartFolder)
        Me.GroupBox7.Controls.Add(Me.txtFanartFolder)
        Me.GroupBox7.Controls.Add(Me.Label48)
        Me.GroupBox7.Controls.Add(Me.txtExcludeFilePath)
        Me.GroupBox7.Controls.Add(Me.Label22)
        Me.GroupBox7.Controls.Add(Me.btnSelectExcludeFile)
        Me.GroupBox7.Controls.Add(Me.txtConfigFilePath)
        Me.GroupBox7.Controls.Add(Me.Label2)
        Me.GroupBox7.Controls.Add(Me.btnSelectConfigFile)
        Me.GroupBox7.Location = New System.Drawing.Point(8, 6)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(575, 183)
        Me.GroupBox7.TabIndex = 103
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Folders and Files"
        '
        'btnExcludeFileDelete
        '
        Me.btnExcludeFileDelete.Location = New System.Drawing.Point(333, 72)
        Me.btnExcludeFileDelete.Name = "btnExcludeFileDelete"
        Me.btnExcludeFileDelete.Size = New System.Drawing.Size(56, 20)
        Me.btnExcludeFileDelete.TabIndex = 57
        Me.btnExcludeFileDelete.Text = "delete"
        Me.btnExcludeFileDelete.UseVisualStyleBackColor = True
        '
        'btnExcludeFileShow
        '
        Me.btnExcludeFileShow.Location = New System.Drawing.Point(409, 72)
        Me.btnExcludeFileShow.Name = "btnExcludeFileShow"
        Me.btnExcludeFileShow.Size = New System.Drawing.Size(56, 20)
        Me.btnExcludeFileShow.TabIndex = 56
        Me.btnExcludeFileShow.Text = "show"
        Me.btnExcludeFileShow.UseVisualStyleBackColor = True
        '
        'btnSelectPersonArtworkFolder
        '
        Me.btnSelectPersonArtworkFolder.Enabled = False
        Me.btnSelectPersonArtworkFolder.Location = New System.Drawing.Point(517, 153)
        Me.btnSelectPersonArtworkFolder.Name = "btnSelectPersonArtworkFolder"
        Me.btnSelectPersonArtworkFolder.Size = New System.Drawing.Size(37, 20)
        Me.btnSelectPersonArtworkFolder.TabIndex = 53
        Me.btnSelectPersonArtworkFolder.Text = "..."
        Me.btnSelectPersonArtworkFolder.UseVisualStyleBackColor = True
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Enabled = False
        Me.Label28.Location = New System.Drawing.Point(6, 137)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(117, 13)
        Me.Label28.TabIndex = 52
        Me.Label28.Text = "Path to Person Images:"
        '
        'txtPersonArtworkFolder
        '
        Me.txtPersonArtworkFolder.Enabled = False
        Me.txtPersonArtworkFolder.Location = New System.Drawing.Point(6, 153)
        Me.txtPersonArtworkFolder.Name = "txtPersonArtworkFolder"
        Me.txtPersonArtworkFolder.Size = New System.Drawing.Size(503, 20)
        Me.txtPersonArtworkFolder.TabIndex = 51
        '
        'btnSelectFanartFolder
        '
        Me.btnSelectFanartFolder.CausesValidation = False
        Me.btnSelectFanartFolder.Location = New System.Drawing.Point(517, 112)
        Me.btnSelectFanartFolder.Name = "btnSelectFanartFolder"
        Me.btnSelectFanartFolder.Size = New System.Drawing.Size(37, 20)
        Me.btnSelectFanartFolder.TabIndex = 50
        Me.btnSelectFanartFolder.Text = "..."
        Me.btnSelectFanartFolder.UseVisualStyleBackColor = True
        '
        'txtFanartFolder
        '
        Me.txtFanartFolder.Location = New System.Drawing.Point(6, 112)
        Me.txtFanartFolder.Name = "txtFanartFolder"
        Me.txtFanartFolder.Size = New System.Drawing.Size(503, 20)
        Me.txtFanartFolder.TabIndex = 49
        Me.ToolTip1.SetToolTip(Me.txtFanartFolder, "Enter the paths (local or UNC) to the folders you want to scan." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Separate multipl" & _
                "e folders with semi-colons.")
        '
        'Label48
        '
        Me.Label48.AutoSize = True
        Me.Label48.Location = New System.Drawing.Point(6, 95)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(80, 13)
        Me.Label48.TabIndex = 48
        Me.Label48.Text = "Path to Fanart :"
        '
        'txtExcludeFilePath
        '
        Me.txtExcludeFilePath.Location = New System.Drawing.Point(6, 72)
        Me.txtExcludeFilePath.Name = "txtExcludeFilePath"
        Me.txtExcludeFilePath.Size = New System.Drawing.Size(302, 20)
        Me.txtExcludeFilePath.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.txtExcludeFilePath, resources.GetString("txtExcludeFilePath.ToolTip"))
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(6, 56)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(173, 13)
        Me.Label22.TabIndex = 45
        Me.Label22.Text = "Path to Excluded Movies File (.txt) :"
        '
        'btnSelectExcludeFile
        '
        Me.btnSelectExcludeFile.CausesValidation = False
        Me.btnSelectExcludeFile.Location = New System.Drawing.Point(517, 72)
        Me.btnSelectExcludeFile.Name = "btnSelectExcludeFile"
        Me.btnSelectExcludeFile.Size = New System.Drawing.Size(37, 20)
        Me.btnSelectExcludeFile.TabIndex = 7
        Me.btnSelectExcludeFile.Text = "..."
        Me.btnSelectExcludeFile.UseVisualStyleBackColor = True
        '
        'txtConfigFilePath
        '
        Me.txtConfigFilePath.Location = New System.Drawing.Point(6, 32)
        Me.txtConfigFilePath.Name = "txtConfigFilePath"
        Me.txtConfigFilePath.Size = New System.Drawing.Size(503, 20)
        Me.txtConfigFilePath.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.txtConfigFilePath, "Enter the location of your Ant Movie Database file." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "This must be saved in .xml f" & _
                "ormat." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "If you do not have one, enter the desired location and a new database fi" & _
                "le will be created for you.")
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(168, 13)
        Me.Label2.TabIndex = 38
        Me.Label2.Text = "Path to AMC Database File (.xml) :"
        '
        'btnSelectConfigFile
        '
        Me.btnSelectConfigFile.CausesValidation = False
        Me.btnSelectConfigFile.Location = New System.Drawing.Point(517, 32)
        Me.btnSelectConfigFile.Name = "btnSelectConfigFile"
        Me.btnSelectConfigFile.Size = New System.Drawing.Size(37, 20)
        Me.btnSelectConfigFile.TabIndex = 3
        Me.btnSelectConfigFile.Text = "..."
        Me.btnSelectConfigFile.UseVisualStyleBackColor = True
        '
        'GroupBox13
        '
        Me.GroupBox13.Controls.Add(Me.cbLogLevel)
        Me.GroupBox13.Controls.Add(Me.Label29)
        Me.GroupBox13.Location = New System.Drawing.Point(8, 530)
        Me.GroupBox13.Name = "GroupBox13"
        Me.GroupBox13.Size = New System.Drawing.Size(575, 40)
        Me.GroupBox13.TabIndex = 5
        Me.GroupBox13.TabStop = False
        Me.GroupBox13.Text = "Other"
        '
        'cbLogLevel
        '
        Me.cbLogLevel.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbLogLevel.FormattingEnabled = True
        Me.cbLogLevel.Items.AddRange(New Object() {"All Events with Grabbing", "All Events", "Major Events", "Errors Only"})
        Me.cbLogLevel.Location = New System.Drawing.Point(137, 13)
        Me.cbLogLevel.Name = "cbLogLevel"
        Me.cbLogLevel.Size = New System.Drawing.Size(171, 21)
        Me.cbLogLevel.TabIndex = 20
        Me.ToolTip1.SetToolTip(Me.cbLogLevel, "The logging level option controls what information is saved to the logfile")
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.Location = New System.Drawing.Point(9, 16)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(80, 13)
        Me.Label29.TabIndex = 34
        Me.Label29.Text = "Logging Level :"
        '
        'GroupBox12
        '
        Me.GroupBox12.Controls.Add(Me.chkExecuteOnlyForOrphans)
        Me.GroupBox12.Controls.Add(Me.btnExecuteProgramSelectPath)
        Me.GroupBox12.Controls.Add(Me.txtExecuteProgramPath)
        Me.GroupBox12.Controls.Add(Me.chkExecuteProgram)
        Me.GroupBox12.Controls.Add(Me.Label27)
        Me.GroupBox12.Location = New System.Drawing.Point(8, 445)
        Me.GroupBox12.Name = "GroupBox12"
        Me.GroupBox12.Size = New System.Drawing.Size(575, 79)
        Me.GroupBox12.TabIndex = 4
        Me.GroupBox12.TabStop = False
        Me.GroupBox12.Text = "Post Processing"
        '
        'chkExecuteOnlyForOrphans
        '
        Me.chkExecuteOnlyForOrphans.AutoSize = True
        Me.chkExecuteOnlyForOrphans.Location = New System.Drawing.Point(377, 16)
        Me.chkExecuteOnlyForOrphans.Name = "chkExecuteOnlyForOrphans"
        Me.chkExecuteOnlyForOrphans.Size = New System.Drawing.Size(136, 17)
        Me.chkExecuteOnlyForOrphans.TabIndex = 19
        Me.chkExecuteOnlyForOrphans.Text = "Only if changes applied"
        Me.chkExecuteOnlyForOrphans.UseVisualStyleBackColor = True
        '
        'btnExecuteProgramSelectPath
        '
        Me.btnExecuteProgramSelectPath.Location = New System.Drawing.Point(476, 51)
        Me.btnExecuteProgramSelectPath.Name = "btnExecuteProgramSelectPath"
        Me.btnExecuteProgramSelectPath.Size = New System.Drawing.Size(37, 20)
        Me.btnExecuteProgramSelectPath.TabIndex = 18
        Me.btnExecuteProgramSelectPath.Text = "..."
        Me.btnExecuteProgramSelectPath.UseVisualStyleBackColor = True
        '
        'txtExecuteProgramPath
        '
        Me.txtExecuteProgramPath.Location = New System.Drawing.Point(12, 51)
        Me.txtExecuteProgramPath.Name = "txtExecuteProgramPath"
        Me.txtExecuteProgramPath.Size = New System.Drawing.Size(458, 20)
        Me.txtExecuteProgramPath.TabIndex = 17
        '
        'chkExecuteProgram
        '
        Me.chkExecuteProgram.AutoSize = True
        Me.chkExecuteProgram.Location = New System.Drawing.Point(12, 16)
        Me.chkExecuteProgram.Name = "chkExecuteProgram"
        Me.chkExecuteProgram.Size = New System.Drawing.Size(217, 17)
        Me.chkExecuteProgram.TabIndex = 16
        Me.chkExecuteProgram.Text = "Launch a file after processing completed"
        Me.chkExecuteProgram.UseVisualStyleBackColor = True
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Location = New System.Drawing.Point(10, 35)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(143, 13)
        Me.Label27.TabIndex = 31
        Me.Label27.Text = "Path to executable to launch"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.chkShortNames)
        Me.GroupBox5.Controls.Add(Me.chkReadDVDLabel)
        Me.GroupBox5.Controls.Add(Me.txtMediaLabel)
        Me.GroupBox5.Controls.Add(Me.Label12)
        Me.GroupBox5.Controls.Add(Me.txtMediaType)
        Me.GroupBox5.Controls.Add(Me.Label13)
        Me.GroupBox5.Controls.Add(Me.txtDefaultSourceField)
        Me.GroupBox5.Controls.Add(Me.Label26)
        Me.GroupBox5.Location = New System.Drawing.Point(8, 339)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(575, 102)
        Me.GroupBox5.TabIndex = 3
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "AMC Options ..."
        '
        'chkShortNames
        '
        Me.chkShortNames.AutoSize = True
        Me.chkShortNames.Location = New System.Drawing.Point(377, 19)
        Me.chkShortNames.Name = "chkShortNames"
        Me.chkShortNames.Size = New System.Drawing.Size(145, 17)
        Me.chkShortNames.TabIndex = 69
        Me.chkShortNames.Text = "Store files with name only"
        Me.ToolTip1.SetToolTip(Me.chkShortNames, "If checked, only file name will be stored to your DB " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "without path information." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "You have to make sure, your MyFilms settings support searching" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "for matching fi" & _
                "les in your setup.")
        Me.chkShortNames.UseVisualStyleBackColor = True
        '
        'chkReadDVDLabel
        '
        Me.chkReadDVDLabel.AutoSize = True
        Me.chkReadDVDLabel.Location = New System.Drawing.Point(377, 71)
        Me.chkReadDVDLabel.Name = "chkReadDVDLabel"
        Me.chkReadDVDLabel.Size = New System.Drawing.Size(156, 17)
        Me.chkReadDVDLabel.TabIndex = 66
        Me.chkReadDVDLabel.Text = "Try to read DVD Disk Label"
        Me.ToolTip1.SetToolTip(Me.chkReadDVDLabel, "Used when importing movies from CD or DVD." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Will attempt to read the disk label a" & _
                "nd use that in the Media Label field." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "If not found then the value entered for '" & _
                "Media Label' will be used (if any).")
        Me.chkReadDVDLabel.UseVisualStyleBackColor = True
        '
        'txtMediaLabel
        '
        Me.txtMediaLabel.Location = New System.Drawing.Point(137, 69)
        Me.txtMediaLabel.Name = "txtMediaLabel"
        Me.txtMediaLabel.Size = New System.Drawing.Size(171, 20)
        Me.txtMediaLabel.TabIndex = 65
        Me.ToolTip1.SetToolTip(Me.txtMediaLabel, "Sets a value to be stored in the Ant Movie Database under the 'Media Label' field" & _
                ".")
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(9, 46)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(105, 13)
        Me.Label12.TabIndex = 67
        Me.Label12.Text = "Media Type (Preset):"
        '
        'txtMediaType
        '
        Me.txtMediaType.Location = New System.Drawing.Point(137, 43)
        Me.txtMediaType.Name = "txtMediaType"
        Me.txtMediaType.Size = New System.Drawing.Size(171, 20)
        Me.txtMediaType.TabIndex = 64
        Me.ToolTip1.SetToolTip(Me.txtMediaType, "Sets a value to be stored in the Ant Movie Database under the 'Media Type' field." & _
                "")
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(9, 72)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(71, 13)
        Me.Label13.TabIndex = 68
        Me.Label13.Text = "Media Label :"
        '
        'txtDefaultSourceField
        '
        Me.txtDefaultSourceField.Location = New System.Drawing.Point(137, 17)
        Me.txtDefaultSourceField.Name = "txtDefaultSourceField"
        Me.txtDefaultSourceField.Size = New System.Drawing.Size(171, 20)
        Me.txtDefaultSourceField.TabIndex = 12
        Me.ToolTip1.SetToolTip(Me.txtDefaultSourceField, "This is the Ant Movie Catalog field which references the location of your moviel " & _
                "files." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "The default is 'Source' - do not change this unless you are sure you nee" & _
                "d to!")
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Location = New System.Drawing.Point(9, 20)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(122, 13)
        Me.Label26.TabIndex = 25
        Me.Label26.Text = "Item for Storage File Info"
        '
        'ScanFilters
        '
        Me.ScanFilters.Controls.Add(Me.GroupBox6)
        Me.ScanFilters.Controls.Add(Me.Label46)
        Me.ScanFilters.Controls.Add(Me.GroupBox20)
        Me.ScanFilters.Controls.Add(Me.GroupBox19)
        Me.ScanFilters.Location = New System.Drawing.Point(4, 22)
        Me.ScanFilters.Name = "ScanFilters"
        Me.ScanFilters.Size = New System.Drawing.Size(600, 580)
        Me.ScanFilters.TabIndex = 5
        Me.ScanFilters.Text = "Scan Filters"
        Me.ScanFilters.UseVisualStyleBackColor = True
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.Label25)
        Me.GroupBox6.Controls.Add(Me.txtRegExSearchMultiPart)
        Me.GroupBox6.Location = New System.Drawing.Point(8, 92)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(575, 63)
        Me.GroupBox6.TabIndex = 58
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Multi Part Files Detection  ..."
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Location = New System.Drawing.Point(6, 16)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(210, 13)
        Me.Label25.TabIndex = 57
        Me.Label25.Text = "RegEx Expression to detect multi-part files :"
        '
        'txtRegExSearchMultiPart
        '
        Me.txtRegExSearchMultiPart.Location = New System.Drawing.Point(9, 32)
        Me.txtRegExSearchMultiPart.Name = "txtRegExSearchMultiPart"
        Me.txtRegExSearchMultiPart.Size = New System.Drawing.Size(551, 20)
        Me.txtRegExSearchMultiPart.TabIndex = 56
        Me.ToolTip1.SetToolTip(Me.txtRegExSearchMultiPart, "AMCUpdater uses this Regular Expression to detect multi-part movies." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Example: 'm" & _
                "oviename - CD1.avi;'moviename - CD2.avi '")
        '
        'Label46
        '
        Me.Label46.Location = New System.Drawing.Point(14, 23)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(506, 54)
        Me.Label46.TabIndex = 55
        Me.Label46.Text = resources.GetString("Label46.Text")
        '
        'GroupBox20
        '
        Me.GroupBox20.Controls.Add(Me.Label83)
        Me.GroupBox20.Controls.Add(Me.dgFilterStrings)
        Me.GroupBox20.Location = New System.Drawing.Point(8, 172)
        Me.GroupBox20.Name = "GroupBox20"
        Me.GroupBox20.Size = New System.Drawing.Size(575, 212)
        Me.GroupBox20.TabIndex = 54
        Me.GroupBox20.TabStop = False
        Me.GroupBox20.Text = "Strip Characters From Title ..."
        '
        'Label83
        '
        Me.Label83.AutoSize = True
        Me.Label83.Location = New System.Drawing.Point(6, 20)
        Me.Label83.Name = "Label83"
        Me.Label83.Size = New System.Drawing.Size(233, 13)
        Me.Label83.TabIndex = 52
        Me.Label83.Text = "Cleanup TITLEs based on keyword and RegEx:"
        '
        'dgFilterStrings
        '
        Me.dgFilterStrings.AllowUserToResizeRows = False
        Me.dgFilterStrings.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgFilterStrings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgFilterStrings.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.dgFilterStrings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgFilterStrings.ColumnHeadersVisible = False
        Me.dgFilterStrings.Location = New System.Drawing.Point(9, 41)
        Me.dgFilterStrings.Name = "dgFilterStrings"
        Me.dgFilterStrings.RowHeadersWidth = 25
        Me.dgFilterStrings.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.dgFilterStrings.Size = New System.Drawing.Size(551, 165)
        Me.dgFilterStrings.TabIndex = 51
        '
        'GroupBox19
        '
        Me.GroupBox19.Controls.Add(Me.dgExcludedFolderStrings)
        Me.GroupBox19.Controls.Add(Me.Label44)
        Me.GroupBox19.Controls.Add(Me.Label45)
        Me.GroupBox19.Controls.Add(Me.dgExcludedFileStrings)
        Me.GroupBox19.Location = New System.Drawing.Point(8, 390)
        Me.GroupBox19.Name = "GroupBox19"
        Me.GroupBox19.Size = New System.Drawing.Size(575, 176)
        Me.GroupBox19.TabIndex = 53
        Me.GroupBox19.TabStop = False
        Me.GroupBox19.Text = "File and Folder Exclusions ..."
        '
        'dgExcludedFolderStrings
        '
        Me.dgExcludedFolderStrings.AllowUserToResizeRows = False
        Me.dgExcludedFolderStrings.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgExcludedFolderStrings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgExcludedFolderStrings.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.dgExcludedFolderStrings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgExcludedFolderStrings.ColumnHeadersVisible = False
        Me.dgExcludedFolderStrings.Location = New System.Drawing.Point(9, 41)
        Me.dgExcludedFolderStrings.Name = "dgExcludedFolderStrings"
        Me.dgExcludedFolderStrings.RowHeadersWidth = 25
        Me.dgExcludedFolderStrings.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.dgExcludedFolderStrings.Size = New System.Drawing.Size(259, 125)
        Me.dgExcludedFolderStrings.TabIndex = 48
        '
        'Label44
        '
        Me.Label44.AutoSize = True
        Me.Label44.Location = New System.Drawing.Point(6, 19)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(194, 13)
        Me.Label44.TabIndex = 46
        Me.Label44.Text = "Exclude FOLDERs based on keywords:"
        '
        'Label45
        '
        Me.Label45.AutoSize = True
        Me.Label45.Location = New System.Drawing.Point(296, 19)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(173, 13)
        Me.Label45.TabIndex = 47
        Me.Label45.Text = "Exclude FILEs based on keywords:"
        '
        'dgExcludedFileStrings
        '
        Me.dgExcludedFileStrings.AllowUserToResizeRows = False
        Me.dgExcludedFileStrings.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgExcludedFileStrings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgExcludedFileStrings.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.dgExcludedFileStrings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgExcludedFileStrings.ColumnHeadersVisible = False
        Me.dgExcludedFileStrings.Location = New System.Drawing.Point(299, 41)
        Me.dgExcludedFileStrings.Name = "dgExcludedFileStrings"
        Me.dgExcludedFileStrings.RowHeadersWidth = 25
        Me.dgExcludedFileStrings.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.dgExcludedFileStrings.Size = New System.Drawing.Size(261, 125)
        Me.dgExcludedFileStrings.TabIndex = 49
        '
        'DatabaseFields
        '
        Me.DatabaseFields.Controls.Add(Me.cbMasterTitle)
        Me.DatabaseFields.Controls.Add(Me.lblMasterTitle)
        Me.DatabaseFields.Controls.Add(Me.lblPicturePrefix)
        Me.DatabaseFields.Controls.Add(Me.txtPictureFilenamePrefix)
        Me.DatabaseFields.Controls.Add(Me.cbPictureHandling)
        Me.DatabaseFields.Controls.Add(Me.lblPictureHandling)
        Me.DatabaseFields.Controls.Add(Me.cbGroupNameAppliesTo)
        Me.DatabaseFields.Controls.Add(Me.lblGroupNameAppliesTo)
        Me.DatabaseFields.Controls.Add(Me.chkFolderNameIsGroupName)
        Me.DatabaseFields.Controls.Add(Me.cbCheckHandling)
        Me.DatabaseFields.Controls.Add(Me.lblCheckFieldHandling)
        Me.DatabaseFields.Controls.Add(Me.lblInternetLookupRequired)
        Me.DatabaseFields.Controls.Add(Me.cbDateHandling)
        Me.DatabaseFields.Controls.Add(Me.Label30)
        Me.DatabaseFields.Controls.Add(Me.GroupBox16)
        Me.DatabaseFields.Controls.Add(Me.cbDatabaseFields)
        Me.DatabaseFields.Controls.Add(Me.Label34)
        Me.DatabaseFields.Location = New System.Drawing.Point(4, 22)
        Me.DatabaseFields.Name = "DatabaseFields"
        Me.DatabaseFields.Size = New System.Drawing.Size(600, 580)
        Me.DatabaseFields.TabIndex = 4
        Me.DatabaseFields.Text = "Database Fields"
        Me.DatabaseFields.UseVisualStyleBackColor = True
        '
        'cbMasterTitle
        '
        Me.cbMasterTitle.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbMasterTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbMasterTitle.FormattingEnabled = True
        Me.cbMasterTitle.Items.AddRange(New Object() {"TranslatedTitle", "OriginalTitle", "FormattedTitle"})
        Me.cbMasterTitle.Location = New System.Drawing.Point(376, 410)
        Me.cbMasterTitle.Name = "cbMasterTitle"
        Me.cbMasterTitle.Size = New System.Drawing.Size(174, 21)
        Me.cbMasterTitle.TabIndex = 51
        Me.ToolTip1.SetToolTip(Me.cbMasterTitle, "Select the title that is used for fanart search and creation.")
        '
        'lblMasterTitle
        '
        Me.lblMasterTitle.AutoSize = True
        Me.lblMasterTitle.Location = New System.Drawing.Point(373, 394)
        Me.lblMasterTitle.Name = "lblMasterTitle"
        Me.lblMasterTitle.Size = New System.Drawing.Size(116, 13)
        Me.lblMasterTitle.TabIndex = 50
        Me.lblMasterTitle.Text = "Master Title (for fanart):"
        '
        'lblPicturePrefix
        '
        Me.lblPicturePrefix.AutoSize = True
        Me.lblPicturePrefix.Location = New System.Drawing.Point(372, 518)
        Me.lblPicturePrefix.Name = "lblPicturePrefix"
        Me.lblPicturePrefix.Size = New System.Drawing.Size(148, 13)
        Me.lblPicturePrefix.TabIndex = 49
        Me.lblPicturePrefix.Text = "Add Prefix to Picture Filename"
        '
        'txtPictureFilenamePrefix
        '
        Me.txtPictureFilenamePrefix.Location = New System.Drawing.Point(376, 534)
        Me.txtPictureFilenamePrefix.Name = "txtPictureFilenamePrefix"
        Me.txtPictureFilenamePrefix.Size = New System.Drawing.Size(174, 20)
        Me.txtPictureFilenamePrefix.TabIndex = 48
        Me.ToolTip1.SetToolTip(Me.txtPictureFilenamePrefix, resources.GetString("txtPictureFilenamePrefix.ToolTip"))
        '
        'cbPictureHandling
        '
        Me.cbPictureHandling.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbPictureHandling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPictureHandling.FormattingEnabled = True
        Me.cbPictureHandling.Items.AddRange(New Object() {"Full Path", "Full Path & Create Moviethumb", "Relative Path", "Relative Path & Create Moviethumb", "Use Folder.jpg", "Create Moviethumb"})
        Me.cbPictureHandling.Location = New System.Drawing.Point(376, 488)
        Me.cbPictureHandling.Name = "cbPictureHandling"
        Me.cbPictureHandling.Size = New System.Drawing.Size(174, 21)
        Me.cbPictureHandling.TabIndex = 46
        Me.ToolTip1.SetToolTip(Me.cbPictureHandling, resources.GetString("cbPictureHandling.ToolTip"))
        '
        'lblPictureHandling
        '
        Me.lblPictureHandling.AutoSize = True
        Me.lblPictureHandling.Location = New System.Drawing.Point(372, 472)
        Me.lblPictureHandling.Name = "lblPictureHandling"
        Me.lblPictureHandling.Size = New System.Drawing.Size(91, 13)
        Me.lblPictureHandling.TabIndex = 45
        Me.lblPictureHandling.Text = "Picture Handling :"
        '
        'cbGroupNameAppliesTo
        '
        Me.cbGroupNameAppliesTo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbGroupNameAppliesTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbGroupNameAppliesTo.FormattingEnabled = True
        Me.cbGroupNameAppliesTo.Items.AddRange(New Object() {"Original Title", "Translated Title", "Both Titles"})
        Me.cbGroupNameAppliesTo.Location = New System.Drawing.Point(376, 355)
        Me.cbGroupNameAppliesTo.Name = "cbGroupNameAppliesTo"
        Me.cbGroupNameAppliesTo.Size = New System.Drawing.Size(174, 21)
        Me.cbGroupNameAppliesTo.TabIndex = 44
        '
        'lblGroupNameAppliesTo
        '
        Me.lblGroupNameAppliesTo.AutoSize = True
        Me.lblGroupNameAppliesTo.Location = New System.Drawing.Point(373, 339)
        Me.lblGroupNameAppliesTo.Name = "lblGroupNameAppliesTo"
        Me.lblGroupNameAppliesTo.Size = New System.Drawing.Size(113, 13)
        Me.lblGroupNameAppliesTo.TabIndex = 43
        Me.lblGroupNameAppliesTo.Text = "Add Group Name to ..."
        '
        'chkFolderNameIsGroupName
        '
        Me.chkFolderNameIsGroupName.AutoSize = True
        Me.chkFolderNameIsGroupName.Location = New System.Drawing.Point(376, 319)
        Me.chkFolderNameIsGroupName.Name = "chkFolderNameIsGroupName"
        Me.chkFolderNameIsGroupName.Size = New System.Drawing.Size(175, 17)
        Me.chkFolderNameIsGroupName.TabIndex = 42
        Me.chkFolderNameIsGroupName.Text = "Folder Name as Group Identifier"
        Me.ToolTip1.SetToolTip(Me.chkFolderNameIsGroupName, resources.GetString("chkFolderNameIsGroupName.ToolTip"))
        Me.chkFolderNameIsGroupName.UseVisualStyleBackColor = True
        '
        'cbCheckHandling
        '
        Me.cbCheckHandling.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbCheckHandling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbCheckHandling.FormattingEnabled = True
        Me.cbCheckHandling.Items.AddRange(New Object() {"Checked", "Unchecked"})
        Me.cbCheckHandling.Location = New System.Drawing.Point(376, 238)
        Me.cbCheckHandling.Name = "cbCheckHandling"
        Me.cbCheckHandling.Size = New System.Drawing.Size(174, 21)
        Me.cbCheckHandling.TabIndex = 41
        Me.ToolTip1.SetToolTip(Me.cbCheckHandling, "The Ant Movie Catalog database includes a boolean 'Checked' field." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "If you select" & _
                " this option in the Database Fields list you can choose " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "whether to set it to '" & _
                "True' or 'False'.")
        '
        'lblCheckFieldHandling
        '
        Me.lblCheckFieldHandling.AutoSize = True
        Me.lblCheckFieldHandling.Location = New System.Drawing.Point(373, 222)
        Me.lblCheckFieldHandling.Name = "lblCheckFieldHandling"
        Me.lblCheckFieldHandling.Size = New System.Drawing.Size(120, 13)
        Me.lblCheckFieldHandling.TabIndex = 40
        Me.lblCheckFieldHandling.Text = "Checked Field Handling"
        '
        'lblInternetLookupRequired
        '
        Me.lblInternetLookupRequired.AutoSize = True
        Me.lblInternetLookupRequired.ForeColor = System.Drawing.Color.Red
        Me.lblInternetLookupRequired.Location = New System.Drawing.Point(389, 188)
        Me.lblInternetLookupRequired.Name = "lblInternetLookupRequired"
        Me.lblInternetLookupRequired.Size = New System.Drawing.Size(147, 26)
        Me.lblInternetLookupRequired.TabIndex = 39
        Me.lblInternetLookupRequired.Text = "-- Internet lookup is required --" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "-- with these settings --"
        Me.lblInternetLookupRequired.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cbDateHandling
        '
        Me.cbDateHandling.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbDateHandling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDateHandling.FormattingEnabled = True
        Me.cbDateHandling.Items.AddRange(New Object() {"File Created Date", "File Modified Date", "Current System Date", "No Date"})
        Me.cbDateHandling.Location = New System.Drawing.Point(376, 281)
        Me.cbDateHandling.Name = "cbDateHandling"
        Me.cbDateHandling.Size = New System.Drawing.Size(174, 21)
        Me.cbDateHandling.TabIndex = 36
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.Location = New System.Drawing.Point(373, 265)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(106, 13)
        Me.Label30.TabIndex = 35
        Me.Label30.Text = "Date Field Handling :"
        '
        'GroupBox16
        '
        Me.GroupBox16.Controls.Add(Me.Label33)
        Me.GroupBox16.Controls.Add(Me.Label32)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectNoInternet)
        Me.GroupBox16.Controls.Add(Me.Label31)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectAllInternet)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectNoMedia)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectNone)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectAllMedia)
        Me.GroupBox16.Controls.Add(Me.btnDBFieldsSelectAll)
        Me.GroupBox16.Location = New System.Drawing.Point(371, 35)
        Me.GroupBox16.Name = "GroupBox16"
        Me.GroupBox16.Size = New System.Drawing.Size(187, 148)
        Me.GroupBox16.TabIndex = 9
        Me.GroupBox16.TabStop = False
        Me.GroupBox16.Text = "Quick Select ..."
        '
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.Location = New System.Drawing.Point(5, 100)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(176, 13)
        Me.Label33.TabIndex = 4
        Me.Label33.Text = "Only data retrieved from the Internet"
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Location = New System.Drawing.Point(6, 58)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(83, 13)
        Me.Label32.TabIndex = 4
        Me.Label32.Text = "Only media data"
        '
        'btnDBFieldsSelectNoInternet
        '
        Me.btnDBFieldsSelectNoInternet.Location = New System.Drawing.Point(95, 116)
        Me.btnDBFieldsSelectNoInternet.Name = "btnDBFieldsSelectNoInternet"
        Me.btnDBFieldsSelectNoInternet.Size = New System.Drawing.Size(80, 23)
        Me.btnDBFieldsSelectNoInternet.TabIndex = 30
        Me.btnDBFieldsSelectNoInternet.Text = "Select None"
        Me.btnDBFieldsSelectNoInternet.UseVisualStyleBackColor = True
        '
        'Label31
        '
        Me.Label31.AutoSize = True
        Me.Label31.Location = New System.Drawing.Point(6, 16)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(45, 13)
        Me.Label31.TabIndex = 4
        Me.Label31.Text = "All items"
        '
        'btnDBFieldsSelectAllInternet
        '
        Me.btnDBFieldsSelectAllInternet.Location = New System.Drawing.Point(8, 116)
        Me.btnDBFieldsSelectAllInternet.Name = "btnDBFieldsSelectAllInternet"
        Me.btnDBFieldsSelectAllInternet.Size = New System.Drawing.Size(81, 23)
        Me.btnDBFieldsSelectAllInternet.TabIndex = 25
        Me.btnDBFieldsSelectAllInternet.Text = "Select All"
        Me.btnDBFieldsSelectAllInternet.UseVisualStyleBackColor = True
        '
        'btnDBFieldsSelectNoMedia
        '
        Me.btnDBFieldsSelectNoMedia.Location = New System.Drawing.Point(95, 74)
        Me.btnDBFieldsSelectNoMedia.Name = "btnDBFieldsSelectNoMedia"
        Me.btnDBFieldsSelectNoMedia.Size = New System.Drawing.Size(80, 23)
        Me.btnDBFieldsSelectNoMedia.TabIndex = 20
        Me.btnDBFieldsSelectNoMedia.Text = "Select None"
        Me.btnDBFieldsSelectNoMedia.UseVisualStyleBackColor = True
        '
        'btnDBFieldsSelectNone
        '
        Me.btnDBFieldsSelectNone.Location = New System.Drawing.Point(95, 32)
        Me.btnDBFieldsSelectNone.Name = "btnDBFieldsSelectNone"
        Me.btnDBFieldsSelectNone.Size = New System.Drawing.Size(80, 23)
        Me.btnDBFieldsSelectNone.TabIndex = 11
        Me.btnDBFieldsSelectNone.Text = "Select None"
        Me.btnDBFieldsSelectNone.UseVisualStyleBackColor = True
        '
        'btnDBFieldsSelectAllMedia
        '
        Me.btnDBFieldsSelectAllMedia.Location = New System.Drawing.Point(9, 74)
        Me.btnDBFieldsSelectAllMedia.Name = "btnDBFieldsSelectAllMedia"
        Me.btnDBFieldsSelectAllMedia.Size = New System.Drawing.Size(80, 23)
        Me.btnDBFieldsSelectAllMedia.TabIndex = 15
        Me.btnDBFieldsSelectAllMedia.Text = "Select All"
        Me.btnDBFieldsSelectAllMedia.UseVisualStyleBackColor = True
        '
        'btnDBFieldsSelectAll
        '
        Me.btnDBFieldsSelectAll.Location = New System.Drawing.Point(9, 32)
        Me.btnDBFieldsSelectAll.Name = "btnDBFieldsSelectAll"
        Me.btnDBFieldsSelectAll.Size = New System.Drawing.Size(80, 23)
        Me.btnDBFieldsSelectAll.TabIndex = 10
        Me.btnDBFieldsSelectAll.Text = "Select All"
        Me.btnDBFieldsSelectAll.UseVisualStyleBackColor = True
        '
        'cbDatabaseFields
        '
        Me.cbDatabaseFields.CheckOnClick = True
        Me.cbDatabaseFields.ColumnWidth = 140
        Me.cbDatabaseFields.FormattingEnabled = True
        Me.cbDatabaseFields.Location = New System.Drawing.Point(11, 24)
        Me.cbDatabaseFields.MultiColumn = True
        Me.cbDatabaseFields.Name = "cbDatabaseFields"
        Me.cbDatabaseFields.Size = New System.Drawing.Size(322, 544)
        Me.cbDatabaseFields.TabIndex = 8
        '
        'Label34
        '
        Me.Label34.AutoSize = True
        Me.Label34.Location = New System.Drawing.Point(8, 10)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(329, 13)
        Me.Label34.TabIndex = 7
        Me.Label34.Text = "Here you can select which database fields will be imported/updated."
        '
        'Manual
        '
        Me.Manual.Controls.Add(Me.btnManualCancel)
        Me.Manual.Controls.Add(Me.grpManualInternetLookupSettings)
        Me.Manual.Controls.Add(Me.btnShowHideLogTest)
        Me.Manual.Controls.Add(Me.btnManualApplyChanges)
        Me.Manual.Controls.Add(Me.btnManualDoTest)
        Me.Manual.Controls.Add(Me.grpManualUpdatesParameters)
        Me.Manual.Controls.Add(Me.GroupBox10)
        Me.Manual.Controls.Add(Me.GroupBox9)
        Me.Manual.Location = New System.Drawing.Point(4, 22)
        Me.Manual.Name = "Manual"
        Me.Manual.Size = New System.Drawing.Size(600, 580)
        Me.Manual.TabIndex = 3
        Me.Manual.Text = "Manual Update"
        Me.Manual.UseVisualStyleBackColor = True
        '
        'btnManualCancel
        '
        Me.btnManualCancel.Enabled = False
        Me.btnManualCancel.Image = CType(resources.GetObject("btnManualCancel.Image"), System.Drawing.Image)
        Me.btnManualCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnManualCancel.Location = New System.Drawing.Point(269, 494)
        Me.btnManualCancel.Name = "btnManualCancel"
        Me.btnManualCancel.Size = New System.Drawing.Size(110, 57)
        Me.btnManualCancel.TabIndex = 102
        Me.btnManualCancel.Text = "Cancel"
        Me.btnManualCancel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnManualCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.btnManualCancel.UseVisualStyleBackColor = True
        '
        'grpManualInternetLookupSettings
        '
        Me.grpManualInternetLookupSettings.Controls.Add(Me.txtManualInternetParserPathDisplay)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.Button2)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.Label42)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.btnManualSelectInternetParserPath)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.cbManualInternetLookupBehaviour)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.txtManualInternetParserPath)
        Me.grpManualInternetLookupSettings.Controls.Add(Me.Label41)
        Me.grpManualInternetLookupSettings.Location = New System.Drawing.Point(10, 371)
        Me.grpManualInternetLookupSettings.Name = "grpManualInternetLookupSettings"
        Me.grpManualInternetLookupSettings.Size = New System.Drawing.Size(573, 107)
        Me.grpManualInternetLookupSettings.TabIndex = 101
        Me.grpManualInternetLookupSettings.TabStop = False
        Me.grpManualInternetLookupSettings.Text = "Internet Grabber Options ..."
        '
        'txtManualInternetParserPathDisplay
        '
        Me.txtManualInternetParserPathDisplay.Enabled = False
        Me.txtManualInternetParserPathDisplay.Location = New System.Drawing.Point(99, 35)
        Me.txtManualInternetParserPathDisplay.Name = "txtManualInternetParserPathDisplay"
        Me.txtManualInternetParserPathDisplay.Size = New System.Drawing.Size(210, 20)
        Me.txtManualInternetParserPathDisplay.TabIndex = 55
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(406, 34)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(94, 21)
        Me.Button2.TabIndex = 54
        Me.Button2.Text = "Grabber Options"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Label42
        '
        Me.Label42.AutoSize = True
        Me.Label42.Location = New System.Drawing.Point(8, 60)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(85, 26)
        Me.Label42.TabIndex = 53
        Me.Label42.Text = "Internet Lookup " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Behaviour"
        '
        'btnManualSelectInternetParserPath
        '
        Me.btnManualSelectInternetParserPath.CausesValidation = False
        Me.btnManualSelectInternetParserPath.Location = New System.Drawing.Point(342, 35)
        Me.btnManualSelectInternetParserPath.Name = "btnManualSelectInternetParserPath"
        Me.btnManualSelectInternetParserPath.Size = New System.Drawing.Size(36, 20)
        Me.btnManualSelectInternetParserPath.TabIndex = 47
        Me.btnManualSelectInternetParserPath.Text = "..."
        Me.btnManualSelectInternetParserPath.UseVisualStyleBackColor = True
        '
        'cbManualInternetLookupBehaviour
        '
        Me.cbManualInternetLookupBehaviour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualInternetLookupBehaviour.FormattingEnabled = True
        Me.cbManualInternetLookupBehaviour.Location = New System.Drawing.Point(99, 63)
        Me.cbManualInternetLookupBehaviour.Name = "cbManualInternetLookupBehaviour"
        Me.cbManualInternetLookupBehaviour.Size = New System.Drawing.Size(227, 21)
        Me.cbManualInternetLookupBehaviour.TabIndex = 50
        '
        'txtManualInternetParserPath
        '
        Me.txtManualInternetParserPath.Location = New System.Drawing.Point(99, 35)
        Me.txtManualInternetParserPath.Name = "txtManualInternetParserPath"
        Me.txtManualInternetParserPath.Size = New System.Drawing.Size(227, 20)
        Me.txtManualInternetParserPath.TabIndex = 46
        Me.txtManualInternetParserPath.Visible = False
        '
        'Label41
        '
        Me.Label41.AutoSize = True
        Me.Label41.Location = New System.Drawing.Point(8, 38)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(75, 13)
        Me.Label41.TabIndex = 51
        Me.Label41.Text = "Grabber Script"
        '
        'btnShowHideLogTest
        '
        Me.btnShowHideLogTest.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnShowHideLogTest.Location = New System.Drawing.Point(489, 529)
        Me.btnShowHideLogTest.Name = "btnShowHideLogTest"
        Me.btnShowHideLogTest.Size = New System.Drawing.Size(94, 22)
        Me.btnShowHideLogTest.TabIndex = 100
        Me.btnShowHideLogTest.Text = "Show Log >>"
        Me.btnShowHideLogTest.UseVisualStyleBackColor = True
        '
        'btnManualApplyChanges
        '
        Me.btnManualApplyChanges.Enabled = False
        Me.btnManualApplyChanges.Image = CType(resources.GetObject("btnManualApplyChanges.Image"), System.Drawing.Image)
        Me.btnManualApplyChanges.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnManualApplyChanges.Location = New System.Drawing.Point(130, 494)
        Me.btnManualApplyChanges.Name = "btnManualApplyChanges"
        Me.btnManualApplyChanges.Size = New System.Drawing.Size(110, 57)
        Me.btnManualApplyChanges.TabIndex = 55
        Me.btnManualApplyChanges.Text = "Apply Changes"
        Me.btnManualApplyChanges.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnManualApplyChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.btnManualApplyChanges.UseVisualStyleBackColor = True
        '
        'btnManualDoTest
        '
        Me.btnManualDoTest.Image = CType(resources.GetObject("btnManualDoTest.Image"), System.Drawing.Image)
        Me.btnManualDoTest.ImageAlign = System.Drawing.ContentAlignment.BottomRight
        Me.btnManualDoTest.Location = New System.Drawing.Point(14, 494)
        Me.btnManualDoTest.Name = "btnManualDoTest"
        Me.btnManualDoTest.Size = New System.Drawing.Size(110, 57)
        Me.btnManualDoTest.TabIndex = 50
        Me.btnManualDoTest.Text = "Test " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Params"
        Me.btnManualDoTest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnManualDoTest.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        Me.btnManualDoTest.UseVisualStyleBackColor = True
        '
        'grpManualUpdatesParameters
        '
        Me.grpManualUpdatesParameters.Controls.Add(Me.cbManualParameterAndOr)
        Me.grpManualUpdatesParameters.Controls.Add(Me.lblManualParametersValue2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.lblManualParametersOperator2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.txtManualParameterValue2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.cbManualParameterOperator2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.lblManualParametersField2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.cbManualParameterFieldList2)
        Me.grpManualUpdatesParameters.Controls.Add(Me.Label21)
        Me.grpManualUpdatesParameters.Controls.Add(Me.chkManualParametersUpdateAll)
        Me.grpManualUpdatesParameters.Controls.Add(Me.lblManualParametersValue1)
        Me.grpManualUpdatesParameters.Controls.Add(Me.Label19)
        Me.grpManualUpdatesParameters.Controls.Add(Me.txtManualParameterValue1)
        Me.grpManualUpdatesParameters.Controls.Add(Me.cbManualParameterOperator1)
        Me.grpManualUpdatesParameters.Controls.Add(Me.Label18)
        Me.grpManualUpdatesParameters.Controls.Add(Me.cbManualParameterFieldList1)
        Me.grpManualUpdatesParameters.Location = New System.Drawing.Point(10, 236)
        Me.grpManualUpdatesParameters.Name = "grpManualUpdatesParameters"
        Me.grpManualUpdatesParameters.Size = New System.Drawing.Size(573, 129)
        Me.grpManualUpdatesParameters.TabIndex = 2
        Me.grpManualUpdatesParameters.TabStop = False
        Me.grpManualUpdatesParameters.Text = "Parameters ..."
        Me.grpManualUpdatesParameters.Visible = False
        '
        'cbManualParameterAndOr
        '
        Me.cbManualParameterAndOr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualParameterAndOr.FormattingEnabled = True
        Me.cbManualParameterAndOr.Items.AddRange(New Object() {"and", "or", " "})
        Me.cbManualParameterAndOr.Location = New System.Drawing.Point(6, 49)
        Me.cbManualParameterAndOr.Name = "cbManualParameterAndOr"
        Me.cbManualParameterAndOr.Size = New System.Drawing.Size(53, 21)
        Me.cbManualParameterAndOr.TabIndex = 3
        '
        'lblManualParametersValue2
        '
        Me.lblManualParametersValue2.AutoSize = True
        Me.lblManualParametersValue2.Location = New System.Drawing.Point(339, 57)
        Me.lblManualParametersValue2.Name = "lblManualParametersValue2"
        Me.lblManualParametersValue2.Size = New System.Drawing.Size(34, 13)
        Me.lblManualParametersValue2.TabIndex = 48
        Me.lblManualParametersValue2.Text = "Value"
        Me.lblManualParametersValue2.Visible = False
        '
        'lblManualParametersOperator2
        '
        Me.lblManualParametersOperator2.AutoSize = True
        Me.lblManualParametersOperator2.Location = New System.Drawing.Point(219, 58)
        Me.lblManualParametersOperator2.Name = "lblManualParametersOperator2"
        Me.lblManualParametersOperator2.Size = New System.Drawing.Size(54, 13)
        Me.lblManualParametersOperator2.TabIndex = 47
        Me.lblManualParametersOperator2.Text = "Operator :"
        Me.lblManualParametersOperator2.Visible = False
        '
        'txtManualParameterValue2
        '
        Me.txtManualParameterValue2.Location = New System.Drawing.Point(341, 75)
        Me.txtManualParameterValue2.Name = "txtManualParameterValue2"
        Me.txtManualParameterValue2.Size = New System.Drawing.Size(207, 20)
        Me.txtManualParameterValue2.TabIndex = 6
        Me.txtManualParameterValue2.Visible = False
        Me.txtManualParameterValue2.WordWrap = False
        '
        'cbManualParameterOperator2
        '
        Me.cbManualParameterOperator2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualParameterOperator2.FormattingEnabled = True
        Me.cbManualParameterOperator2.Items.AddRange(New Object() {"=", "!=", "LIKE", "NOT LIKE", "EXISTS", "NOT EXISTS", ">", "<"})
        Me.cbManualParameterOperator2.Location = New System.Drawing.Point(215, 74)
        Me.cbManualParameterOperator2.Name = "cbManualParameterOperator2"
        Me.cbManualParameterOperator2.Size = New System.Drawing.Size(111, 21)
        Me.cbManualParameterOperator2.TabIndex = 5
        Me.cbManualParameterOperator2.Visible = False
        '
        'lblManualParametersField2
        '
        Me.lblManualParametersField2.AutoSize = True
        Me.lblManualParametersField2.Location = New System.Drawing.Point(68, 58)
        Me.lblManualParametersField2.Name = "lblManualParametersField2"
        Me.lblManualParametersField2.Size = New System.Drawing.Size(54, 13)
        Me.lblManualParametersField2.TabIndex = 46
        Me.lblManualParametersField2.Text = "Ant Field :"
        Me.lblManualParametersField2.Visible = False
        '
        'cbManualParameterFieldList2
        '
        Me.cbManualParameterFieldList2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualParameterFieldList2.FormattingEnabled = True
        Me.cbManualParameterFieldList2.Location = New System.Drawing.Point(68, 74)
        Me.cbManualParameterFieldList2.Name = "cbManualParameterFieldList2"
        Me.cbManualParameterFieldList2.Size = New System.Drawing.Size(132, 21)
        Me.cbManualParameterFieldList2.TabIndex = 4
        Me.cbManualParameterFieldList2.Visible = False
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(18, 102)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(25, 13)
        Me.Label21.TabIndex = 13
        Me.Label21.Text = "or..."
        '
        'chkManualParametersUpdateAll
        '
        Me.chkManualParametersUpdateAll.AutoSize = True
        Me.chkManualParametersUpdateAll.Location = New System.Drawing.Point(49, 101)
        Me.chkManualParametersUpdateAll.Name = "chkManualParametersUpdateAll"
        Me.chkManualParametersUpdateAll.Size = New System.Drawing.Size(115, 17)
        Me.chkManualParametersUpdateAll.TabIndex = 7
        Me.chkManualParametersUpdateAll.Text = "Update all records."
        Me.chkManualParametersUpdateAll.UseVisualStyleBackColor = True
        '
        'lblManualParametersValue1
        '
        Me.lblManualParametersValue1.AutoSize = True
        Me.lblManualParametersValue1.Location = New System.Drawing.Point(339, 15)
        Me.lblManualParametersValue1.Name = "lblManualParametersValue1"
        Me.lblManualParametersValue1.Size = New System.Drawing.Size(34, 13)
        Me.lblManualParametersValue1.TabIndex = 10
        Me.lblManualParametersValue1.Text = "Value"
        Me.lblManualParametersValue1.Visible = False
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(219, 16)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(54, 13)
        Me.Label19.TabIndex = 9
        Me.Label19.Text = "Operator :"
        '
        'txtManualParameterValue1
        '
        Me.txtManualParameterValue1.Location = New System.Drawing.Point(341, 32)
        Me.txtManualParameterValue1.Name = "txtManualParameterValue1"
        Me.txtManualParameterValue1.Size = New System.Drawing.Size(207, 20)
        Me.txtManualParameterValue1.TabIndex = 2
        Me.txtManualParameterValue1.Visible = False
        '
        'cbManualParameterOperator1
        '
        Me.cbManualParameterOperator1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualParameterOperator1.FormattingEnabled = True
        Me.cbManualParameterOperator1.Items.AddRange(New Object() {"=", "!=", "LIKE", "NOT LIKE", "EXISTS", "NOT EXISTS", ">", "<", ">Num", "<Num"})
        Me.cbManualParameterOperator1.Location = New System.Drawing.Point(215, 32)
        Me.cbManualParameterOperator1.Name = "cbManualParameterOperator1"
        Me.cbManualParameterOperator1.Size = New System.Drawing.Size(111, 21)
        Me.cbManualParameterOperator1.TabIndex = 1
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(68, 16)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(54, 13)
        Me.Label18.TabIndex = 6
        Me.Label18.Text = "Ant Field :"
        '
        'cbManualParameterFieldList1
        '
        Me.cbManualParameterFieldList1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualParameterFieldList1.FormattingEnabled = True
        Me.cbManualParameterFieldList1.Location = New System.Drawing.Point(68, 32)
        Me.cbManualParameterFieldList1.Name = "cbManualParameterFieldList1"
        Me.cbManualParameterFieldList1.Size = New System.Drawing.Size(132, 21)
        Me.cbManualParameterFieldList1.TabIndex = 0
        '
        'GroupBox10
        '
        Me.GroupBox10.Controls.Add(Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData)
        Me.GroupBox10.Controls.Add(Me.chkManualUpdateRecordsOnlyMissingData)
        Me.GroupBox10.Controls.Add(Me.txtManualOldValue)
        Me.GroupBox10.Controls.Add(Me.lblManualEnterOldValue)
        Me.GroupBox10.Controls.Add(Me.chkManualMissingTrailer)
        Me.GroupBox10.Controls.Add(Me.chkManualMissingFanartDownload)
        Me.GroupBox10.Controls.Add(Me.lblManualDatabaseFieldsPrompt)
        Me.GroupBox10.Controls.Add(Me.lblManualSelectField)
        Me.GroupBox10.Controls.Add(Me.cbManualSelectField)
        Me.GroupBox10.Controls.Add(Me.txtManualNewValue)
        Me.GroupBox10.Controls.Add(Me.lblManualEnterNewValue)
        Me.GroupBox10.Controls.Add(Me.cbManualSelectOperation)
        Me.GroupBox10.Location = New System.Drawing.Point(10, 117)
        Me.GroupBox10.Name = "GroupBox10"
        Me.GroupBox10.Size = New System.Drawing.Size(573, 113)
        Me.GroupBox10.TabIndex = 1
        Me.GroupBox10.TabStop = False
        Me.GroupBox10.Text = "Operation ..."
        '
        'chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData
        '
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.AutoSize = True
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Location = New System.Drawing.Point(49, 90)
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Name = "chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData"
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Size = New System.Drawing.Size(181, 17)
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.TabIndex = 13
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Text = "Only update with non-empty data"
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.UseVisualStyleBackColor = True
        Me.chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Visible = False
        '
        'chkManualUpdateRecordsOnlyMissingData
        '
        Me.chkManualUpdateRecordsOnlyMissingData.AutoSize = True
        Me.chkManualUpdateRecordsOnlyMissingData.Location = New System.Drawing.Point(49, 73)
        Me.chkManualUpdateRecordsOnlyMissingData.Name = "chkManualUpdateRecordsOnlyMissingData"
        Me.chkManualUpdateRecordsOnlyMissingData.Size = New System.Drawing.Size(129, 17)
        Me.chkManualUpdateRecordsOnlyMissingData.TabIndex = 12
        Me.chkManualUpdateRecordsOnlyMissingData.Text = "Only add missing data"
        Me.ToolTip1.SetToolTip(Me.chkManualUpdateRecordsOnlyMissingData, "If checked, instead of owerwriting all fields selected on ""Database Fields"" tab w" & _
                "ith the new grabbed data," & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "only fields that are empty are filled with data.")
        Me.chkManualUpdateRecordsOnlyMissingData.UseVisualStyleBackColor = True
        Me.chkManualUpdateRecordsOnlyMissingData.Visible = False
        '
        'txtManualOldValue
        '
        Me.txtManualOldValue.Location = New System.Drawing.Point(342, 53)
        Me.txtManualOldValue.Name = "txtManualOldValue"
        Me.txtManualOldValue.Size = New System.Drawing.Size(206, 20)
        Me.txtManualOldValue.TabIndex = 11
        Me.txtManualOldValue.Visible = False
        '
        'lblManualEnterOldValue
        '
        Me.lblManualEnterOldValue.AutoSize = True
        Me.lblManualEnterOldValue.Location = New System.Drawing.Point(251, 56)
        Me.lblManualEnterOldValue.Name = "lblManualEnterOldValue"
        Me.lblManualEnterOldValue.Size = New System.Drawing.Size(53, 13)
        Me.lblManualEnterOldValue.TabIndex = 10
        Me.lblManualEnterOldValue.Text = "Old Value"
        Me.lblManualEnterOldValue.Visible = False
        '
        'chkManualMissingTrailer
        '
        Me.chkManualMissingTrailer.AutoSize = True
        Me.chkManualMissingTrailer.Checked = True
        Me.chkManualMissingTrailer.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkManualMissingTrailer.Location = New System.Drawing.Point(279, 23)
        Me.chkManualMissingTrailer.Name = "chkManualMissingTrailer"
        Me.chkManualMissingTrailer.Size = New System.Drawing.Size(194, 17)
        Me.chkManualMissingTrailer.TabIndex = 9
        Me.chkManualMissingTrailer.Text = "Update only Movies missing Trailers"
        Me.chkManualMissingTrailer.UseVisualStyleBackColor = True
        Me.chkManualMissingTrailer.Visible = False
        '
        'chkManualMissingFanartDownload
        '
        Me.chkManualMissingFanartDownload.AutoSize = True
        Me.chkManualMissingFanartDownload.Checked = True
        Me.chkManualMissingFanartDownload.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkManualMissingFanartDownload.Location = New System.Drawing.Point(279, 23)
        Me.chkManualMissingFanartDownload.Name = "chkManualMissingFanartDownload"
        Me.chkManualMissingFanartDownload.Size = New System.Drawing.Size(218, 17)
        Me.chkManualMissingFanartDownload.TabIndex = 8
        Me.chkManualMissingFanartDownload.Text = "Download only for Movies missing Fanart"
        Me.chkManualMissingFanartDownload.UseVisualStyleBackColor = True
        Me.chkManualMissingFanartDownload.Visible = False
        '
        'lblManualDatabaseFieldsPrompt
        '
        Me.lblManualDatabaseFieldsPrompt.AutoSize = True
        Me.lblManualDatabaseFieldsPrompt.Location = New System.Drawing.Point(5, 44)
        Me.lblManualDatabaseFieldsPrompt.Name = "lblManualDatabaseFieldsPrompt"
        Me.lblManualDatabaseFieldsPrompt.Size = New System.Drawing.Size(205, 26)
        Me.lblManualDatabaseFieldsPrompt.TabIndex = 5
        Me.lblManualDatabaseFieldsPrompt.Text = "Note : Ensure the fields you want updated" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "are selected on the Database Fields ta" & _
            "b." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.lblManualDatabaseFieldsPrompt.Visible = False
        '
        'lblManualSelectField
        '
        Me.lblManualSelectField.AutoSize = True
        Me.lblManualSelectField.Location = New System.Drawing.Point(262, 22)
        Me.lblManualSelectField.Name = "lblManualSelectField"
        Me.lblManualSelectField.Size = New System.Drawing.Size(54, 13)
        Me.lblManualSelectField.TabIndex = 4
        Me.lblManualSelectField.Text = "Ant Field :"
        Me.lblManualSelectField.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.lblManualSelectField.Visible = False
        '
        'cbManualSelectField
        '
        Me.cbManualSelectField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualSelectField.FormattingEnabled = True
        Me.cbManualSelectField.Location = New System.Drawing.Point(342, 19)
        Me.cbManualSelectField.Name = "cbManualSelectField"
        Me.cbManualSelectField.Size = New System.Drawing.Size(206, 21)
        Me.cbManualSelectField.TabIndex = 1
        Me.cbManualSelectField.Visible = False
        '
        'txtManualNewValue
        '
        Me.txtManualNewValue.Location = New System.Drawing.Point(342, 81)
        Me.txtManualNewValue.Name = "txtManualNewValue"
        Me.txtManualNewValue.Size = New System.Drawing.Size(206, 20)
        Me.txtManualNewValue.TabIndex = 2
        Me.txtManualNewValue.Visible = False
        '
        'lblManualEnterNewValue
        '
        Me.lblManualEnterNewValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblManualEnterNewValue.AutoSize = True
        Me.lblManualEnterNewValue.Location = New System.Drawing.Point(252, 84)
        Me.lblManualEnterNewValue.Name = "lblManualEnterNewValue"
        Me.lblManualEnterNewValue.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblManualEnterNewValue.Size = New System.Drawing.Size(65, 13)
        Me.lblManualEnterNewValue.TabIndex = 1
        Me.lblManualEnterNewValue.Text = "New Value :"
        Me.lblManualEnterNewValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblManualEnterNewValue.Visible = False
        '
        'cbManualSelectOperation
        '
        Me.cbManualSelectOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbManualSelectOperation.FormattingEnabled = True
        Me.cbManualSelectOperation.Items.AddRange(New Object() {"Update Value", "Update Value - Replace String", "Update Value - Add String", "Update Value - Insert String", "Update Value - Remove String", "Delete Value", "Update Record", "Delete Record", "Download Fanart", "Register Trailer"})
        Me.cbManualSelectOperation.Location = New System.Drawing.Point(8, 19)
        Me.cbManualSelectOperation.Name = "cbManualSelectOperation"
        Me.cbManualSelectOperation.Size = New System.Drawing.Size(195, 21)
        Me.cbManualSelectOperation.TabIndex = 0
        '
        'GroupBox9
        '
        Me.GroupBox9.Controls.Add(Me.btnManualExcludedMoviesFileDelete)
        Me.GroupBox9.Controls.Add(Me.btnManualExcludedMoviesFileShow)
        Me.GroupBox9.Controls.Add(Me.txtManualXMLPath)
        Me.GroupBox9.Controls.Add(Me.Label20)
        Me.GroupBox9.Controls.Add(Me.Label40)
        Me.GroupBox9.Controls.Add(Me.txtManualExcludedMoviesPath)
        Me.GroupBox9.Controls.Add(Me.btnManualSelectXMLFile)
        Me.GroupBox9.Controls.Add(Me.btnManualSelectExcludedMoviesFile)
        Me.GroupBox9.Location = New System.Drawing.Point(10, 3)
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.Size = New System.Drawing.Size(573, 108)
        Me.GroupBox9.TabIndex = 0
        Me.GroupBox9.TabStop = False
        Me.GroupBox9.Text = "Folders and Files ..."
        '
        'btnManualExcludedMoviesFileDelete
        '
        Me.btnManualExcludedMoviesFileDelete.Location = New System.Drawing.Point(342, 74)
        Me.btnManualExcludedMoviesFileDelete.Name = "btnManualExcludedMoviesFileDelete"
        Me.btnManualExcludedMoviesFileDelete.Size = New System.Drawing.Size(56, 20)
        Me.btnManualExcludedMoviesFileDelete.TabIndex = 54
        Me.btnManualExcludedMoviesFileDelete.Text = "delete"
        Me.btnManualExcludedMoviesFileDelete.UseVisualStyleBackColor = True
        '
        'btnManualExcludedMoviesFileShow
        '
        Me.btnManualExcludedMoviesFileShow.Location = New System.Drawing.Point(416, 74)
        Me.btnManualExcludedMoviesFileShow.Name = "btnManualExcludedMoviesFileShow"
        Me.btnManualExcludedMoviesFileShow.Size = New System.Drawing.Size(56, 20)
        Me.btnManualExcludedMoviesFileShow.TabIndex = 53
        Me.btnManualExcludedMoviesFileShow.Text = "show"
        Me.btnManualExcludedMoviesFileShow.UseVisualStyleBackColor = True
        '
        'txtManualXMLPath
        '
        Me.txtManualXMLPath.Enabled = False
        Me.txtManualXMLPath.Location = New System.Drawing.Point(8, 32)
        Me.txtManualXMLPath.Name = "txtManualXMLPath"
        Me.txtManualXMLPath.Size = New System.Drawing.Size(464, 20)
        Me.txtManualXMLPath.TabIndex = 0
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(6, 16)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(168, 13)
        Me.Label20.TabIndex = 38
        Me.Label20.Text = "Path to AMC Database File (.xml) :"
        '
        'Label40
        '
        Me.Label40.AutoSize = True
        Me.Label40.Location = New System.Drawing.Point(6, 58)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(173, 13)
        Me.Label40.TabIndex = 52
        Me.Label40.Text = "Path to Excluded Movies File (.txt) :"
        '
        'txtManualExcludedMoviesPath
        '
        Me.txtManualExcludedMoviesPath.Location = New System.Drawing.Point(8, 74)
        Me.txtManualExcludedMoviesPath.Name = "txtManualExcludedMoviesPath"
        Me.txtManualExcludedMoviesPath.Size = New System.Drawing.Size(318, 20)
        Me.txtManualExcludedMoviesPath.TabIndex = 48
        '
        'btnManualSelectXMLFile
        '
        Me.btnManualSelectXMLFile.CausesValidation = False
        Me.btnManualSelectXMLFile.Location = New System.Drawing.Point(478, 32)
        Me.btnManualSelectXMLFile.Name = "btnManualSelectXMLFile"
        Me.btnManualSelectXMLFile.Size = New System.Drawing.Size(36, 20)
        Me.btnManualSelectXMLFile.TabIndex = 1
        Me.btnManualSelectXMLFile.Text = "..."
        Me.btnManualSelectXMLFile.UseVisualStyleBackColor = True
        '
        'btnManualSelectExcludedMoviesFile
        '
        Me.btnManualSelectExcludedMoviesFile.CausesValidation = False
        Me.btnManualSelectExcludedMoviesFile.Location = New System.Drawing.Point(478, 74)
        Me.btnManualSelectExcludedMoviesFile.Name = "btnManualSelectExcludedMoviesFile"
        Me.btnManualSelectExcludedMoviesFile.Size = New System.Drawing.Size(36, 20)
        Me.btnManualSelectExcludedMoviesFile.TabIndex = 49
        Me.btnManualSelectExcludedMoviesFile.Text = "..."
        Me.btnManualSelectExcludedMoviesFile.UseVisualStyleBackColor = True
        '
        'Test
        '
        Me.Test.Controls.Add(Me.Label47)
        Me.Test.Controls.Add(Me.GroupBox18)
        Me.Test.Controls.Add(Me.GroupBox3)
        Me.Test.Controls.Add(Me.GroupBox2)
        Me.Test.Controls.Add(Me.btnTestAnalyse)
        Me.Test.Controls.Add(Me.GroupBox1)
        Me.Test.Controls.Add(Me.Label3)
        Me.Test.Controls.Add(Me.btnGetSampleFile)
        Me.Test.Controls.Add(Me.txtSampleFile)
        Me.Test.Location = New System.Drawing.Point(4, 22)
        Me.Test.Name = "Test"
        Me.Test.Padding = New System.Windows.Forms.Padding(3)
        Me.Test.Size = New System.Drawing.Size(600, 580)
        Me.Test.TabIndex = 2
        Me.Test.Text = "Tests"
        Me.Test.UseVisualStyleBackColor = True
        '
        'Label47
        '
        Me.Label47.Location = New System.Drawing.Point(14, 11)
        Me.Label47.Name = "Label47"
        Me.Label47.Size = New System.Drawing.Size(495, 43)
        Me.Label47.TabIndex = 16
        Me.Label47.Text = "This page allows you to test that the MediaInfo dll is able to read your movie fi" & _
            "le correctly.  Relevant information from the file will be displayed below if pos" & _
            "sible."
        '
        'GroupBox18
        '
        Me.GroupBox18.Controls.Add(Me.txtSampleTextLanguageList)
        Me.GroupBox18.Controls.Add(Me.txtSampleTextCodecList)
        Me.GroupBox18.Controls.Add(Me.Label39)
        Me.GroupBox18.Controls.Add(Me.Label38)
        Me.GroupBox18.Location = New System.Drawing.Point(10, 397)
        Me.GroupBox18.Name = "GroupBox18"
        Me.GroupBox18.Size = New System.Drawing.Size(522, 99)
        Me.GroupBox18.TabIndex = 15
        Me.GroupBox18.TabStop = False
        Me.GroupBox18.Text = "Subtitles ..."
        '
        'txtSampleTextLanguageList
        '
        Me.txtSampleTextLanguageList.Location = New System.Drawing.Point(87, 45)
        Me.txtSampleTextLanguageList.Name = "txtSampleTextLanguageList"
        Me.txtSampleTextLanguageList.ReadOnly = True
        Me.txtSampleTextLanguageList.Size = New System.Drawing.Size(397, 20)
        Me.txtSampleTextLanguageList.TabIndex = 17
        Me.txtSampleTextLanguageList.TabStop = False
        '
        'txtSampleTextCodecList
        '
        Me.txtSampleTextCodecList.Location = New System.Drawing.Point(87, 19)
        Me.txtSampleTextCodecList.Name = "txtSampleTextCodecList"
        Me.txtSampleTextCodecList.ReadOnly = True
        Me.txtSampleTextCodecList.Size = New System.Drawing.Size(397, 20)
        Me.txtSampleTextCodecList.TabIndex = 16
        Me.txtSampleTextCodecList.TabStop = False
        '
        'Label39
        '
        Me.Label39.AutoSize = True
        Me.Label39.Location = New System.Drawing.Point(18, 22)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(63, 13)
        Me.Label39.TabIndex = 1
        Me.Label39.Text = "Codec List :"
        '
        'Label38
        '
        Me.Label38.AutoSize = True
        Me.Label38.Location = New System.Drawing.Point(15, 48)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(66, 13)
        Me.Label38.TabIndex = 0
        Me.Label38.Text = "Languages :"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.txtSampleFileLength)
        Me.GroupBox3.Controls.Add(Me.Label8)
        Me.GroupBox3.Controls.Add(Me.txtSampleFileSize)
        Me.GroupBox3.Controls.Add(Me.Label9)
        Me.GroupBox3.Location = New System.Drawing.Point(10, 502)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(522, 58)
        Me.GroupBox3.TabIndex = 12
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "File ..."
        '
        'txtSampleFileLength
        '
        Me.txtSampleFileLength.Location = New System.Drawing.Point(327, 22)
        Me.txtSampleFileLength.Name = "txtSampleFileLength"
        Me.txtSampleFileLength.ReadOnly = True
        Me.txtSampleFileLength.Size = New System.Drawing.Size(157, 20)
        Me.txtSampleFileLength.TabIndex = 11
        Me.txtSampleFileLength.TabStop = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(275, 25)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(46, 13)
        Me.Label8.TabIndex = 5
        Me.Label8.Text = "Length :"
        '
        'txtSampleFileSize
        '
        Me.txtSampleFileSize.Location = New System.Drawing.Point(87, 22)
        Me.txtSampleFileSize.Name = "txtSampleFileSize"
        Me.txtSampleFileSize.ReadOnly = True
        Me.txtSampleFileSize.Size = New System.Drawing.Size(139, 20)
        Me.txtSampleFileSize.TabIndex = 10
        Me.txtSampleFileSize.TabStop = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(47, 25)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(33, 13)
        Me.Label9.TabIndex = 3
        Me.Label9.Text = "Size :"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label37)
        Me.GroupBox2.Controls.Add(Me.Label36)
        Me.GroupBox2.Controls.Add(Me.txtSampleAudioLanguageList)
        Me.GroupBox2.Controls.Add(Me.txtSampleAudioStreamList)
        Me.GroupBox2.Controls.Add(Me.txtSampleAudioStreamCount)
        Me.GroupBox2.Controls.Add(Me.Label35)
        Me.GroupBox2.Controls.Add(Me.txtSampleAudioBitrate)
        Me.GroupBox2.Controls.Add(Me.Label10)
        Me.GroupBox2.Controls.Add(Me.txtSampleAudioCodec)
        Me.GroupBox2.Controls.Add(Me.Label11)
        Me.GroupBox2.Location = New System.Drawing.Point(10, 237)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(522, 154)
        Me.GroupBox2.TabIndex = 11
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Audio ..."
        '
        'Label37
        '
        Me.Label37.AutoSize = True
        Me.Label37.Location = New System.Drawing.Point(19, 125)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(66, 13)
        Me.Label37.TabIndex = 15
        Me.Label37.Text = "Languages :"
        '
        'Label36
        '
        Me.Label36.AutoSize = True
        Me.Label36.Location = New System.Drawing.Point(30, 99)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(51, 13)
        Me.Label36.TabIndex = 14
        Me.Label36.Text = "Streams :"
        '
        'txtSampleAudioLanguageList
        '
        Me.txtSampleAudioLanguageList.Location = New System.Drawing.Point(87, 122)
        Me.txtSampleAudioLanguageList.Name = "txtSampleAudioLanguageList"
        Me.txtSampleAudioLanguageList.ReadOnly = True
        Me.txtSampleAudioLanguageList.Size = New System.Drawing.Size(397, 20)
        Me.txtSampleAudioLanguageList.TabIndex = 13
        Me.txtSampleAudioLanguageList.TabStop = False
        '
        'txtSampleAudioStreamList
        '
        Me.txtSampleAudioStreamList.Location = New System.Drawing.Point(87, 96)
        Me.txtSampleAudioStreamList.Name = "txtSampleAudioStreamList"
        Me.txtSampleAudioStreamList.ReadOnly = True
        Me.txtSampleAudioStreamList.Size = New System.Drawing.Size(397, 20)
        Me.txtSampleAudioStreamList.TabIndex = 12
        Me.txtSampleAudioStreamList.TabStop = False
        '
        'txtSampleAudioStreamCount
        '
        Me.txtSampleAudioStreamCount.Location = New System.Drawing.Point(359, 48)
        Me.txtSampleAudioStreamCount.Name = "txtSampleAudioStreamCount"
        Me.txtSampleAudioStreamCount.ReadOnly = True
        Me.txtSampleAudioStreamCount.Size = New System.Drawing.Size(125, 20)
        Me.txtSampleAudioStreamCount.TabIndex = 11
        Me.txtSampleAudioStreamCount.TabStop = False
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.Location = New System.Drawing.Point(275, 51)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(51, 13)
        Me.Label35.TabIndex = 10
        Me.Label35.Text = "Streams :"
        '
        'txtSampleAudioBitrate
        '
        Me.txtSampleAudioBitrate.Location = New System.Drawing.Point(86, 48)
        Me.txtSampleAudioBitrate.Name = "txtSampleAudioBitrate"
        Me.txtSampleAudioBitrate.ReadOnly = True
        Me.txtSampleAudioBitrate.Size = New System.Drawing.Size(157, 20)
        Me.txtSampleAudioBitrate.TabIndex = 9
        Me.txtSampleAudioBitrate.TabStop = False
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(36, 51)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(43, 13)
        Me.Label10.TabIndex = 5
        Me.Label10.Text = "Bitrate :"
        '
        'txtSampleAudioCodec
        '
        Me.txtSampleAudioCodec.Location = New System.Drawing.Point(87, 19)
        Me.txtSampleAudioCodec.Name = "txtSampleAudioCodec"
        Me.txtSampleAudioCodec.ReadOnly = True
        Me.txtSampleAudioCodec.Size = New System.Drawing.Size(397, 20)
        Me.txtSampleAudioCodec.TabIndex = 8
        Me.txtSampleAudioCodec.TabStop = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(37, 22)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(44, 13)
        Me.Label11.TabIndex = 3
        Me.Label11.Text = "Codec :"
        '
        'btnTestAnalyse
        '
        Me.btnTestAnalyse.Location = New System.Drawing.Point(370, 108)
        Me.btnTestAnalyse.Name = "btnTestAnalyse"
        Me.btnTestAnalyse.Size = New System.Drawing.Size(124, 31)
        Me.btnTestAnalyse.TabIndex = 3
        Me.btnTestAnalyse.Text = "Analyse"
        Me.btnTestAnalyse.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtSampleVideoResolution)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.txtSampleVideoFramerate)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.txtSampleVideoBitrate)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.txtSampleVideoCodec)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Location = New System.Drawing.Point(10, 150)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(522, 81)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Video ..."
        '
        'txtSampleVideoResolution
        '
        Me.txtSampleVideoResolution.Location = New System.Drawing.Point(232, 48)
        Me.txtSampleVideoResolution.Name = "txtSampleVideoResolution"
        Me.txtSampleVideoResolution.ReadOnly = True
        Me.txtSampleVideoResolution.Size = New System.Drawing.Size(88, 20)
        Me.txtSampleVideoResolution.TabIndex = 6
        Me.txtSampleVideoResolution.TabStop = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(163, 51)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(63, 13)
        Me.Label7.TabIndex = 9
        Me.Label7.Text = "Resolution :"
        '
        'txtSampleVideoFramerate
        '
        Me.txtSampleVideoFramerate.Location = New System.Drawing.Point(396, 48)
        Me.txtSampleVideoFramerate.Name = "txtSampleVideoFramerate"
        Me.txtSampleVideoFramerate.ReadOnly = True
        Me.txtSampleVideoFramerate.Size = New System.Drawing.Size(88, 20)
        Me.txtSampleVideoFramerate.TabIndex = 7
        Me.txtSampleVideoFramerate.TabStop = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(326, 51)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(65, 13)
        Me.Label6.TabIndex = 7
        Me.Label6.Text = "FrameRate :"
        '
        'txtSampleVideoBitrate
        '
        Me.txtSampleVideoBitrate.Location = New System.Drawing.Point(87, 48)
        Me.txtSampleVideoBitrate.Name = "txtSampleVideoBitrate"
        Me.txtSampleVideoBitrate.ReadOnly = True
        Me.txtSampleVideoBitrate.Size = New System.Drawing.Size(69, 20)
        Me.txtSampleVideoBitrate.TabIndex = 5
        Me.txtSampleVideoBitrate.TabStop = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(37, 51)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(43, 13)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "Bitrate :"
        '
        'txtSampleVideoCodec
        '
        Me.txtSampleVideoCodec.Location = New System.Drawing.Point(87, 22)
        Me.txtSampleVideoCodec.Name = "txtSampleVideoCodec"
        Me.txtSampleVideoCodec.ReadOnly = True
        Me.txtSampleVideoCodec.Size = New System.Drawing.Size(397, 20)
        Me.txtSampleVideoCodec.TabIndex = 4
        Me.txtSampleVideoCodec.TabStop = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(36, 25)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(44, 13)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Codec :"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(15, 54)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(126, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Select sample movie file :"
        '
        'btnGetSampleFile
        '
        Me.btnGetSampleFile.Location = New System.Drawing.Point(456, 69)
        Me.btnGetSampleFile.Name = "btnGetSampleFile"
        Me.btnGetSampleFile.Size = New System.Drawing.Size(38, 20)
        Me.btnGetSampleFile.TabIndex = 2
        Me.btnGetSampleFile.Text = "..."
        Me.btnGetSampleFile.UseVisualStyleBackColor = True
        '
        'txtSampleFile
        '
        Me.txtSampleFile.Location = New System.Drawing.Point(17, 69)
        Me.txtSampleFile.Name = "txtSampleFile"
        Me.txtSampleFile.Size = New System.Drawing.Size(433, 20)
        Me.txtSampleFile.TabIndex = 1
        '
        'ViewCollection
        '
        Me.ViewCollection.Controls.Add(Me.GroupBoxMovieDetails)
        Me.ViewCollection.Controls.Add(Me.XionPanel1)
        Me.ViewCollection.Controls.Add(Me.Label59)
        Me.ViewCollection.Controls.Add(Me.VidéoBindingNavigator)
        Me.ViewCollection.Location = New System.Drawing.Point(4, 22)
        Me.ViewCollection.Name = "ViewCollection"
        Me.ViewCollection.Padding = New System.Windows.Forms.Padding(3)
        Me.ViewCollection.Size = New System.Drawing.Size(600, 580)
        Me.ViewCollection.TabIndex = 6
        Me.ViewCollection.Text = "View Collection"
        Me.ViewCollection.UseVisualStyleBackColor = True
        '
        'GroupBoxMovieDetails
        '
        Me.GroupBoxMovieDetails.Controls.Add(Me.PictureBox1)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox31)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox2)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label82)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label49)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox30)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox5)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label81)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label50)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label80)
        Me.GroupBoxMovieDetails.Controls.Add(Me.RichTextBox1)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox29)
        Me.GroupBoxMovieDetails.Controls.Add(Me.RichTextBox2)
        Me.GroupBoxMovieDetails.Controls.Add(Me.GroupBox14)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label78)
        Me.GroupBoxMovieDetails.Controls.Add(Me.RichTextBox3)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox28)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox3)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label77)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label54)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox27)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox4)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label74)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label55)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox24)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox9)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label73)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label57)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox23)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox8)
        Me.GroupBoxMovieDetails.Controls.Add(Me.GroupBox22)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label56)
        Me.GroupBoxMovieDetails.Controls.Add(Me.GroupBox21)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label58)
        Me.GroupBoxMovieDetails.Controls.Add(Me.GroupBox15)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox10)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label66)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label60)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox16)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox11)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label64)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label61)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox14)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox13)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label65)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label63)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox15)
        Me.GroupBoxMovieDetails.Controls.Add(Me.TextBox12)
        Me.GroupBoxMovieDetails.Controls.Add(Me.Label62)
        Me.GroupBoxMovieDetails.Dock = System.Windows.Forms.DockStyle.Right
        Me.GroupBoxMovieDetails.Location = New System.Drawing.Point(93, 28)
        Me.GroupBoxMovieDetails.Name = "GroupBoxMovieDetails"
        Me.GroupBoxMovieDetails.Size = New System.Drawing.Size(504, 549)
        Me.GroupBoxMovieDetails.TabIndex = 74
        Me.GroupBoxMovieDetails.TabStop = False
        '
        'PictureBox1
        '
        Me.PictureBox1.DataBindings.Add(New System.Windows.Forms.Binding("ImageLocation", Me.VideoBindingSource, "Picture", True))
        Me.PictureBox1.Location = New System.Drawing.Point(6, 11)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(158, 218)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 5
        Me.PictureBox1.TabStop = False
        '
        'VideoBindingSource
        '
        Me.VideoBindingSource.DataSource = GetType(AMCUpdater.AntMovieCatalog.MovieDataTable)
        '
        'TextBox31
        '
        Me.TextBox31.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Tagline", True))
        Me.TextBox31.Location = New System.Drawing.Point(227, 213)
        Me.TextBox31.Name = "TextBox31"
        Me.TextBox31.Size = New System.Drawing.Size(93, 20)
        Me.TextBox31.TabIndex = 73
        '
        'TextBox2
        '
        Me.TextBox2.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Number", True))
        Me.TextBox2.Location = New System.Drawing.Point(202, 20)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(49, 20)
        Me.TextBox2.TabIndex = 4
        '
        'Label82
        '
        Me.Label82.AutoSize = True
        Me.Label82.Location = New System.Drawing.Point(184, 216)
        Me.Label82.Name = "Label82"
        Me.Label82.Size = New System.Drawing.Size(42, 13)
        Me.Label82.TabIndex = 72
        Me.Label82.Text = "Tagline"
        '
        'Label49
        '
        Me.Label49.AutoSize = True
        Me.Label49.Location = New System.Drawing.Point(172, 23)
        Me.Label49.Name = "Label49"
        Me.Label49.Size = New System.Drawing.Size(24, 13)
        Me.Label49.TabIndex = 8
        Me.Label49.Text = "Nbr"
        '
        'TextBox30
        '
        Me.TextBox30.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Certification", True))
        Me.TextBox30.Location = New System.Drawing.Point(227, 187)
        Me.TextBox30.Name = "TextBox30"
        Me.TextBox30.Size = New System.Drawing.Size(93, 20)
        Me.TextBox30.TabIndex = 71
        '
        'TextBox5
        '
        Me.TextBox5.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Rating", True))
        Me.TextBox5.Location = New System.Drawing.Point(457, 20)
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New System.Drawing.Size(40, 20)
        Me.TextBox5.TabIndex = 9
        '
        'Label81
        '
        Me.Label81.AutoSize = True
        Me.Label81.Location = New System.Drawing.Point(184, 190)
        Me.Label81.Name = "Label81"
        Me.Label81.Size = New System.Drawing.Size(26, 13)
        Me.Label81.TabIndex = 70
        Me.Label81.Text = "Cert"
        '
        'Label50
        '
        Me.Label50.AutoSize = True
        Me.Label50.Location = New System.Drawing.Point(413, 23)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(38, 13)
        Me.Label50.TabIndex = 10
        Me.Label50.Text = "Rating"
        '
        'Label80
        '
        Me.Label80.AutoSize = True
        Me.Label80.Location = New System.Drawing.Point(329, 216)
        Me.Label80.Name = "Label80"
        Me.Label80.Size = New System.Drawing.Size(35, 13)
        Me.Label80.TabIndex = 69
        Me.Label80.Text = "Writer"
        '
        'RichTextBox1
        '
        Me.RichTextBox1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Description", True))
        Me.RichTextBox1.Location = New System.Drawing.Point(187, 316)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.Size = New System.Drawing.Size(310, 106)
        Me.RichTextBox1.TabIndex = 13
        Me.RichTextBox1.Text = ""
        '
        'TextBox29
        '
        Me.TextBox29.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Writer", True))
        Me.TextBox29.Location = New System.Drawing.Point(376, 213)
        Me.TextBox29.Name = "TextBox29"
        Me.TextBox29.Size = New System.Drawing.Size(121, 20)
        Me.TextBox29.TabIndex = 68
        '
        'RichTextBox2
        '
        Me.RichTextBox2.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Comments", True))
        Me.RichTextBox2.Location = New System.Drawing.Point(187, 430)
        Me.RichTextBox2.Name = "RichTextBox2"
        Me.RichTextBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        Me.RichTextBox2.Size = New System.Drawing.Size(310, 56)
        Me.RichTextBox2.TabIndex = 14
        Me.RichTextBox2.Text = ""
        '
        'GroupBox14
        '
        Me.GroupBox14.Controls.Add(Me.Label53)
        Me.GroupBox14.Controls.Add(Me.Label52)
        Me.GroupBox14.Controls.Add(Me.Label51)
        Me.GroupBox14.Controls.Add(Me.TextBox7)
        Me.GroupBox14.Controls.Add(Me.TextBox6)
        Me.GroupBox14.Controls.Add(Me.TextBox1)
        Me.GroupBox14.Location = New System.Drawing.Point(172, 43)
        Me.GroupBox14.Name = "GroupBox14"
        Me.GroupBox14.Size = New System.Drawing.Size(236, 86)
        Me.GroupBox14.TabIndex = 15
        Me.GroupBox14.TabStop = False
        Me.GroupBox14.Text = "Titles"
        '
        'Label53
        '
        Me.Label53.AutoSize = True
        Me.Label53.Location = New System.Drawing.Point(6, 63)
        Me.Label53.Name = "Label53"
        Me.Label53.Size = New System.Drawing.Size(54, 13)
        Me.Label53.TabIndex = 11
        Me.Label53.Text = "Formatted"
        '
        'Label52
        '
        Me.Label52.AutoSize = True
        Me.Label52.Location = New System.Drawing.Point(6, 39)
        Me.Label52.Name = "Label52"
        Me.Label52.Size = New System.Drawing.Size(57, 13)
        Me.Label52.TabIndex = 10
        Me.Label52.Text = "Translated"
        '
        'Label51
        '
        Me.Label51.AutoSize = True
        Me.Label51.Location = New System.Drawing.Point(7, 15)
        Me.Label51.Name = "Label51"
        Me.Label51.Size = New System.Drawing.Size(42, 13)
        Me.Label51.TabIndex = 9
        Me.Label51.Text = "Original"
        '
        'TextBox7
        '
        Me.TextBox7.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "FormattedTitle", True))
        Me.TextBox7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox7.Location = New System.Drawing.Point(65, 60)
        Me.TextBox7.Name = "TextBox7"
        Me.TextBox7.Size = New System.Drawing.Size(165, 20)
        Me.TextBox7.TabIndex = 4
        '
        'TextBox6
        '
        Me.TextBox6.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "TranslatedTitle", True))
        Me.TextBox6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox6.Location = New System.Drawing.Point(65, 36)
        Me.TextBox6.Name = "TextBox6"
        Me.TextBox6.Size = New System.Drawing.Size(165, 20)
        Me.TextBox6.TabIndex = 3
        '
        'TextBox1
        '
        Me.TextBox1.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "OriginalTitle", True))
        Me.TextBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox1.Location = New System.Drawing.Point(65, 12)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(165, 20)
        Me.TextBox1.TabIndex = 2
        '
        'Label78
        '
        Me.Label78.AutoSize = True
        Me.Label78.Location = New System.Drawing.Point(179, 523)
        Me.Label78.Name = "Label78"
        Me.Label78.Size = New System.Drawing.Size(33, 13)
        Me.Label78.TabIndex = 64
        Me.Label78.Text = "Disks"
        '
        'RichTextBox3
        '
        Me.RichTextBox3.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Actors", True))
        Me.RichTextBox3.Location = New System.Drawing.Point(227, 251)
        Me.RichTextBox3.Name = "RichTextBox3"
        Me.RichTextBox3.Size = New System.Drawing.Size(270, 59)
        Me.RichTextBox3.TabIndex = 16
        Me.RichTextBox3.Text = ""
        '
        'TextBox28
        '
        Me.TextBox28.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Disks", True))
        Me.TextBox28.Location = New System.Drawing.Point(215, 520)
        Me.TextBox28.Name = "TextBox28"
        Me.TextBox28.Size = New System.Drawing.Size(31, 20)
        Me.TextBox28.TabIndex = 63
        '
        'TextBox3
        '
        Me.TextBox3.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "DateAdded", True))
        Me.TextBox3.Location = New System.Drawing.Point(332, 20)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(76, 20)
        Me.TextBox3.TabIndex = 17
        '
        'Label77
        '
        Me.Label77.AutoSize = True
        Me.Label77.Location = New System.Drawing.Point(179, 497)
        Me.Label77.Name = "Label77"
        Me.Label77.Size = New System.Drawing.Size(27, 13)
        Me.Label77.TabIndex = 62
        Me.Label77.Text = "Size"
        '
        'Label54
        '
        Me.Label54.AutoSize = True
        Me.Label54.Location = New System.Drawing.Point(262, 23)
        Me.Label54.Name = "Label54"
        Me.Label54.Size = New System.Drawing.Size(64, 13)
        Me.Label54.TabIndex = 18
        Me.Label54.Text = "Date Added"
        '
        'TextBox27
        '
        Me.TextBox27.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Size", True))
        Me.TextBox27.Location = New System.Drawing.Point(212, 494)
        Me.TextBox27.Name = "TextBox27"
        Me.TextBox27.Size = New System.Drawing.Size(34, 20)
        Me.TextBox27.TabIndex = 61
        '
        'TextBox4
        '
        Me.TextBox4.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Year", True))
        Me.TextBox4.Location = New System.Drawing.Point(457, 53)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(40, 20)
        Me.TextBox4.TabIndex = 20
        '
        'Label74
        '
        Me.Label74.AutoSize = True
        Me.Label74.Location = New System.Drawing.Point(10, 523)
        Me.Label74.Name = "Label74"
        Me.Label74.Size = New System.Drawing.Size(47, 13)
        Me.Label74.TabIndex = 60
        Me.Label74.Text = "Subtitles"
        Me.Label74.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label55
        '
        Me.Label55.AutoSize = True
        Me.Label55.Location = New System.Drawing.Point(413, 56)
        Me.Label55.Name = "Label55"
        Me.Label55.Size = New System.Drawing.Size(29, 13)
        Me.Label55.TabIndex = 21
        Me.Label55.Text = "Year"
        '
        'TextBox24
        '
        Me.TextBox24.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Subtitles", True))
        Me.TextBox24.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox24.Location = New System.Drawing.Point(70, 520)
        Me.TextBox24.Name = "TextBox24"
        Me.TextBox24.Size = New System.Drawing.Size(93, 20)
        Me.TextBox24.TabIndex = 59
        '
        'TextBox9
        '
        Me.TextBox9.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Checked", True))
        Me.TextBox9.Location = New System.Drawing.Point(457, 79)
        Me.TextBox9.Name = "TextBox9"
        Me.TextBox9.Size = New System.Drawing.Size(40, 20)
        Me.TextBox9.TabIndex = 22
        '
        'Label73
        '
        Me.Label73.AutoSize = True
        Me.Label73.Location = New System.Drawing.Point(7, 497)
        Me.Label73.Name = "Label73"
        Me.Label73.Size = New System.Drawing.Size(60, 13)
        Me.Label73.TabIndex = 58
        Me.Label73.Text = "Languages"
        Me.Label73.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label57
        '
        Me.Label57.AutoSize = True
        Me.Label57.Location = New System.Drawing.Point(413, 82)
        Me.Label57.Name = "Label57"
        Me.Label57.Size = New System.Drawing.Size(32, 13)
        Me.Label57.TabIndex = 23
        Me.Label57.Text = "Chkd"
        Me.Label57.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TextBox23
        '
        Me.TextBox23.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Languages", True))
        Me.TextBox23.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox23.Location = New System.Drawing.Point(70, 494)
        Me.TextBox23.Name = "TextBox23"
        Me.TextBox23.Size = New System.Drawing.Size(93, 20)
        Me.TextBox23.TabIndex = 57
        '
        'TextBox8
        '
        Me.TextBox8.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Length", True))
        Me.TextBox8.Location = New System.Drawing.Point(457, 105)
        Me.TextBox8.Name = "TextBox8"
        Me.TextBox8.Size = New System.Drawing.Size(40, 20)
        Me.TextBox8.TabIndex = 24
        '
        'GroupBox22
        '
        Me.GroupBox22.Controls.Add(Me.Label75)
        Me.GroupBox22.Controls.Add(Me.Label76)
        Me.GroupBox22.Controls.Add(Me.TextBox25)
        Me.GroupBox22.Controls.Add(Me.TextBox26)
        Me.GroupBox22.Location = New System.Drawing.Point(11, 424)
        Me.GroupBox22.Name = "GroupBox22"
        Me.GroupBox22.Size = New System.Drawing.Size(152, 62)
        Me.GroupBox22.TabIndex = 56
        Me.GroupBox22.TabStop = False
        Me.GroupBox22.Text = "Audio"
        '
        'Label75
        '
        Me.Label75.AutoSize = True
        Me.Label75.Location = New System.Drawing.Point(9, 39)
        Me.Label75.Name = "Label75"
        Me.Label75.Size = New System.Drawing.Size(37, 13)
        Me.Label75.TabIndex = 45
        Me.Label75.Text = "Bitrate"
        Me.Label75.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label76
        '
        Me.Label76.AutoSize = True
        Me.Label76.Location = New System.Drawing.Point(6, 16)
        Me.Label76.Name = "Label76"
        Me.Label76.Size = New System.Drawing.Size(39, 13)
        Me.Label76.TabIndex = 43
        Me.Label76.Text = "Format"
        Me.Label76.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TextBox25
        '
        Me.TextBox25.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "AudioFormat", True))
        Me.TextBox25.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox25.Location = New System.Drawing.Point(59, 13)
        Me.TextBox25.Name = "TextBox25"
        Me.TextBox25.Size = New System.Drawing.Size(86, 20)
        Me.TextBox25.TabIndex = 42
        '
        'TextBox26
        '
        Me.TextBox26.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "AudioBitrate", True))
        Me.TextBox26.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox26.Location = New System.Drawing.Point(59, 36)
        Me.TextBox26.Name = "TextBox26"
        Me.TextBox26.Size = New System.Drawing.Size(86, 20)
        Me.TextBox26.TabIndex = 44
        '
        'Label56
        '
        Me.Label56.AutoSize = True
        Me.Label56.Location = New System.Drawing.Point(411, 108)
        Me.Label56.Name = "Label56"
        Me.Label56.Size = New System.Drawing.Size(40, 13)
        Me.Label56.TabIndex = 25
        Me.Label56.Text = "Length"
        Me.Label56.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'GroupBox21
        '
        Me.GroupBox21.Controls.Add(Me.Label72)
        Me.GroupBox21.Controls.Add(Me.TextBox22)
        Me.GroupBox21.Controls.Add(Me.Label71)
        Me.GroupBox21.Controls.Add(Me.TextBox21)
        Me.GroupBox21.Controls.Add(Me.Label69)
        Me.GroupBox21.Controls.Add(Me.Label70)
        Me.GroupBox21.Controls.Add(Me.TextBox19)
        Me.GroupBox21.Controls.Add(Me.TextBox20)
        Me.GroupBox21.Location = New System.Drawing.Point(11, 316)
        Me.GroupBox21.Name = "GroupBox21"
        Me.GroupBox21.Size = New System.Drawing.Size(152, 106)
        Me.GroupBox21.TabIndex = 55
        Me.GroupBox21.TabStop = False
        Me.GroupBox21.Text = "Video"
        '
        'Label72
        '
        Me.Label72.AutoSize = True
        Me.Label72.Location = New System.Drawing.Point(3, 84)
        Me.Label72.Name = "Label72"
        Me.Label72.Size = New System.Drawing.Size(54, 13)
        Me.Label72.TabIndex = 49
        Me.Label72.Text = "Framerate"
        Me.Label72.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TextBox22
        '
        Me.TextBox22.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Framerate", True))
        Me.TextBox22.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox22.Location = New System.Drawing.Point(59, 82)
        Me.TextBox22.Name = "TextBox22"
        Me.TextBox22.Size = New System.Drawing.Size(86, 20)
        Me.TextBox22.TabIndex = 48
        '
        'Label71
        '
        Me.Label71.AutoSize = True
        Me.Label71.Location = New System.Drawing.Point(1, 62)
        Me.Label71.Name = "Label71"
        Me.Label71.Size = New System.Drawing.Size(57, 13)
        Me.Label71.TabIndex = 47
        Me.Label71.Text = "Resolution"
        Me.Label71.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TextBox21
        '
        Me.TextBox21.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Resolution", True))
        Me.TextBox21.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox21.Location = New System.Drawing.Point(59, 59)
        Me.TextBox21.Name = "TextBox21"
        Me.TextBox21.Size = New System.Drawing.Size(86, 20)
        Me.TextBox21.TabIndex = 46
        '
        'Label69
        '
        Me.Label69.AutoSize = True
        Me.Label69.Location = New System.Drawing.Point(9, 39)
        Me.Label69.Name = "Label69"
        Me.Label69.Size = New System.Drawing.Size(37, 13)
        Me.Label69.TabIndex = 45
        Me.Label69.Text = "Bitrate"
        Me.Label69.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label70
        '
        Me.Label70.AutoSize = True
        Me.Label70.Location = New System.Drawing.Point(6, 16)
        Me.Label70.Name = "Label70"
        Me.Label70.Size = New System.Drawing.Size(39, 13)
        Me.Label70.TabIndex = 43
        Me.Label70.Text = "Format"
        Me.Label70.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TextBox19
        '
        Me.TextBox19.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "VideoFormat", True))
        Me.TextBox19.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox19.Location = New System.Drawing.Point(59, 13)
        Me.TextBox19.Name = "TextBox19"
        Me.TextBox19.Size = New System.Drawing.Size(86, 20)
        Me.TextBox19.TabIndex = 42
        '
        'TextBox20
        '
        Me.TextBox20.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "VideoBitrate", True))
        Me.TextBox20.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox20.Location = New System.Drawing.Point(59, 36)
        Me.TextBox20.Name = "TextBox20"
        Me.TextBox20.Size = New System.Drawing.Size(86, 20)
        Me.TextBox20.TabIndex = 44
        '
        'Label58
        '
        Me.Label58.AutoSize = True
        Me.Label58.Location = New System.Drawing.Point(184, 251)
        Me.Label58.Name = "Label58"
        Me.Label58.Size = New System.Drawing.Size(37, 13)
        Me.Label58.TabIndex = 26
        Me.Label58.Text = "Actors"
        '
        'GroupBox15
        '
        Me.GroupBox15.Controls.Add(Me.Label68)
        Me.GroupBox15.Controls.Add(Me.Label67)
        Me.GroupBox15.Controls.Add(Me.TextBox17)
        Me.GroupBox15.Controls.Add(Me.TextBox18)
        Me.GroupBox15.Location = New System.Drawing.Point(10, 251)
        Me.GroupBox15.Name = "GroupBox15"
        Me.GroupBox15.Size = New System.Drawing.Size(153, 59)
        Me.GroupBox15.TabIndex = 54
        Me.GroupBox15.TabStop = False
        Me.GroupBox15.Text = "Media"
        '
        'Label68
        '
        Me.Label68.AutoSize = True
        Me.Label68.Location = New System.Drawing.Point(9, 39)
        Me.Label68.Name = "Label68"
        Me.Label68.Size = New System.Drawing.Size(31, 13)
        Me.Label68.TabIndex = 45
        Me.Label68.Text = "Type"
        '
        'Label67
        '
        Me.Label67.AutoSize = True
        Me.Label67.Location = New System.Drawing.Point(9, 15)
        Me.Label67.Name = "Label67"
        Me.Label67.Size = New System.Drawing.Size(33, 13)
        Me.Label67.TabIndex = 43
        Me.Label67.Text = "Label"
        '
        'TextBox17
        '
        Me.TextBox17.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "MediaLabel", True))
        Me.TextBox17.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox17.Location = New System.Drawing.Point(53, 12)
        Me.TextBox17.Name = "TextBox17"
        Me.TextBox17.Size = New System.Drawing.Size(93, 20)
        Me.TextBox17.TabIndex = 42
        '
        'TextBox18
        '
        Me.TextBox18.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "MediaType", True))
        Me.TextBox18.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox18.Location = New System.Drawing.Point(53, 36)
        Me.TextBox18.Name = "TextBox18"
        Me.TextBox18.Size = New System.Drawing.Size(93, 20)
        Me.TextBox18.TabIndex = 44
        '
        'TextBox10
        '
        Me.TextBox10.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Director", True))
        Me.TextBox10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox10.Location = New System.Drawing.Point(376, 135)
        Me.TextBox10.Name = "TextBox10"
        Me.TextBox10.Size = New System.Drawing.Size(121, 20)
        Me.TextBox10.TabIndex = 27
        '
        'Label66
        '
        Me.Label66.AutoSize = True
        Me.Label66.Location = New System.Drawing.Point(326, 190)
        Me.Label66.Name = "Label66"
        Me.Label66.Size = New System.Drawing.Size(49, 13)
        Me.Label66.TabIndex = 41
        Me.Label66.Text = "Borrower"
        '
        'Label60
        '
        Me.Label60.AutoSize = True
        Me.Label60.Location = New System.Drawing.Point(326, 138)
        Me.Label60.Name = "Label60"
        Me.Label60.Size = New System.Drawing.Size(44, 13)
        Me.Label60.TabIndex = 29
        Me.Label60.Text = "Director"
        '
        'TextBox16
        '
        Me.TextBox16.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Borrower", True))
        Me.TextBox16.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox16.Location = New System.Drawing.Point(376, 187)
        Me.TextBox16.Name = "TextBox16"
        Me.TextBox16.Size = New System.Drawing.Size(121, 20)
        Me.TextBox16.TabIndex = 40
        '
        'TextBox11
        '
        Me.TextBox11.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Producer", True))
        Me.TextBox11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox11.Location = New System.Drawing.Point(376, 161)
        Me.TextBox11.Name = "TextBox11"
        Me.TextBox11.Size = New System.Drawing.Size(121, 20)
        Me.TextBox11.TabIndex = 30
        '
        'Label64
        '
        Me.Label64.AutoSize = True
        Me.Label64.Location = New System.Drawing.Point(264, 523)
        Me.Label64.Name = "Label64"
        Me.Label64.Size = New System.Drawing.Size(29, 13)
        Me.Label64.TabIndex = 39
        Me.Label64.Text = "URL"
        '
        'Label61
        '
        Me.Label61.AutoSize = True
        Me.Label61.Location = New System.Drawing.Point(326, 164)
        Me.Label61.Name = "Label61"
        Me.Label61.Size = New System.Drawing.Size(50, 13)
        Me.Label61.TabIndex = 31
        Me.Label61.Text = "Producer"
        '
        'TextBox14
        '
        Me.TextBox14.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "URL", True))
        Me.TextBox14.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox14.Location = New System.Drawing.Point(299, 520)
        Me.TextBox14.Name = "TextBox14"
        Me.TextBox14.Size = New System.Drawing.Size(198, 20)
        Me.TextBox14.TabIndex = 38
        '
        'TextBox13
        '
        Me.TextBox13.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Country", True))
        Me.TextBox13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox13.Location = New System.Drawing.Point(227, 161)
        Me.TextBox13.Name = "TextBox13"
        Me.TextBox13.Size = New System.Drawing.Size(93, 20)
        Me.TextBox13.TabIndex = 32
        '
        'Label65
        '
        Me.Label65.AutoSize = True
        Me.Label65.Location = New System.Drawing.Point(252, 497)
        Me.Label65.Name = "Label65"
        Me.Label65.Size = New System.Drawing.Size(41, 13)
        Me.Label65.TabIndex = 37
        Me.Label65.Text = "Source"
        '
        'Label63
        '
        Me.Label63.AutoSize = True
        Me.Label63.Location = New System.Drawing.Point(172, 164)
        Me.Label63.Name = "Label63"
        Me.Label63.Size = New System.Drawing.Size(43, 13)
        Me.Label63.TabIndex = 33
        Me.Label63.Text = "Country"
        '
        'TextBox15
        '
        Me.TextBox15.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Source", True))
        Me.TextBox15.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox15.Location = New System.Drawing.Point(299, 494)
        Me.TextBox15.Name = "TextBox15"
        Me.TextBox15.Size = New System.Drawing.Size(198, 20)
        Me.TextBox15.TabIndex = 36
        '
        'TextBox12
        '
        Me.TextBox12.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.VideoBindingSource, "Category", True))
        Me.TextBox12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox12.Location = New System.Drawing.Point(227, 135)
        Me.TextBox12.Name = "TextBox12"
        Me.TextBox12.Size = New System.Drawing.Size(93, 20)
        Me.TextBox12.TabIndex = 34
        '
        'Label62
        '
        Me.Label62.AutoSize = True
        Me.Label62.Location = New System.Drawing.Point(172, 138)
        Me.Label62.Name = "Label62"
        Me.Label62.Size = New System.Drawing.Size(49, 13)
        Me.Label62.TabIndex = 35
        Me.Label62.Text = "Category"
        '
        'XionPanel1
        '
        Me.XionPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.XionPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.XionPanel1.Controls.Add(Me.DataGridView1)
        Me.XionPanel1.Location = New System.Drawing.Point(3, 31)
        Me.XionPanel1.Movable = False
        Me.XionPanel1.Name = "XionPanel1"
        Me.XionPanel1.Orientation = XionControls.XionPanel.PanelOrientation.Horizontal
        Me.XionPanel1.Padding = New System.Windows.Forms.Padding(0, 27, 0, 0)
        Me.XionPanel1.Sizable = True
        Me.XionPanel1.Size = New System.Drawing.Size(84, 546)
        Me.XionPanel1.State = XionControls.XionPanel.PanelState.Expand
        Me.XionPanel1.TabIndex = 67
        Me.XionPanel1.Text = "Movie List"
        Me.XionPanel1.Title = "Movie List"
        Me.XionPanel1.TitleBackColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.XionPanel1.TitleFont = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XionPanel1.TitleHeight = 27
        '
        'DataGridView1
        '
        Me.DataGridView1.AutoGenerateColumns = False
        Me.DataGridView1.ColumnHeadersHeight = 21
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn4, Me.DataGridViewTextBoxColumn2, Me.DataGridViewTextBoxColumn3, Me.DataGridViewTextBoxColumn5, Me.Category, Me.Country, Me.Rating})
        Me.DataGridView1.DataSource = Me.VideoBindingSource
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridView1.DefaultCellStyle = DataGridViewCellStyle11
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.Location = New System.Drawing.Point(0, 27)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle12.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle12
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.RowHeadersWidth = 20
        Me.DataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridView1.RowTemplate.Height = 16
        Me.DataGridView1.Size = New System.Drawing.Size(80, 515)
        Me.DataGridView1.TabIndex = 1
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.DataPropertyName = "Number"
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.DataGridViewTextBoxColumn1.DefaultCellStyle = DataGridViewCellStyle8
        Me.DataGridViewTextBoxColumn1.HeaderText = "N°"
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.ReadOnly = True
        Me.DataGridViewTextBoxColumn1.Width = 35
        '
        'DataGridViewTextBoxColumn4
        '
        Me.DataGridViewTextBoxColumn4.DataPropertyName = "Year"
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.DataGridViewTextBoxColumn4.DefaultCellStyle = DataGridViewCellStyle9
        Me.DataGridViewTextBoxColumn4.HeaderText = "Year"
        Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
        Me.DataGridViewTextBoxColumn4.ReadOnly = True
        Me.DataGridViewTextBoxColumn4.Width = 35
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.DataPropertyName = "OriginalTitle"
        Me.DataGridViewTextBoxColumn2.HeaderText = "Original Title"
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.ReadOnly = True
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.DataPropertyName = "TranslatedTitle"
        Me.DataGridViewTextBoxColumn3.HeaderText = "Translated Title"
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        Me.DataGridViewTextBoxColumn3.ReadOnly = True
        '
        'DataGridViewTextBoxColumn5
        '
        Me.DataGridViewTextBoxColumn5.DataPropertyName = "DateAdded"
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.DataGridViewTextBoxColumn5.DefaultCellStyle = DataGridViewCellStyle10
        Me.DataGridViewTextBoxColumn5.HeaderText = "Date Added"
        Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
        Me.DataGridViewTextBoxColumn5.ReadOnly = True
        Me.DataGridViewTextBoxColumn5.Width = 66
        '
        'Category
        '
        Me.Category.DataPropertyName = "Category"
        Me.Category.HeaderText = "Category"
        Me.Category.Name = "Category"
        Me.Category.ReadOnly = True
        Me.Category.Width = 110
        '
        'Country
        '
        Me.Country.DataPropertyName = "Country"
        Me.Country.HeaderText = "Country"
        Me.Country.Name = "Country"
        Me.Country.ReadOnly = True
        Me.Country.Width = 70
        '
        'Rating
        '
        Me.Rating.DataPropertyName = "Rating"
        Me.Rating.HeaderText = "Rating"
        Me.Rating.Name = "Rating"
        Me.Rating.ReadOnly = True
        Me.Rating.Width = 50
        '
        'Label59
        '
        Me.Label59.AutoSize = True
        Me.Label59.Location = New System.Drawing.Point(-55, 223)
        Me.Label59.Name = "Label59"
        Me.Label59.Size = New System.Drawing.Size(42, 13)
        Me.Label59.TabIndex = 28
        Me.Label59.Text = "Original"
        '
        'VidéoBindingNavigator
        '
        Me.VidéoBindingNavigator.AddNewItem = Me.BindingNavigatorAddNewItem
        Me.VidéoBindingNavigator.BindingSource = Me.VideoBindingSource
        Me.VidéoBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.VidéoBindingNavigator.DeleteItem = Me.BindingNavigatorDeleteItem
        Me.VidéoBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem, Me.VidéoBindingNavigatorSaveItem})
        Me.VidéoBindingNavigator.Location = New System.Drawing.Point(3, 3)
        Me.VidéoBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.VidéoBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.VidéoBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.VidéoBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.VidéoBindingNavigator.Name = "VidéoBindingNavigator"
        Me.VidéoBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
        Me.VidéoBindingNavigator.Size = New System.Drawing.Size(594, 25)
        Me.VidéoBindingNavigator.TabIndex = 3
        Me.VidéoBindingNavigator.Text = "BindingNavigator1"
        '
        'BindingNavigatorAddNewItem
        '
        Me.BindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorAddNewItem.Enabled = False
        Me.BindingNavigatorAddNewItem.Image = CType(resources.GetObject("BindingNavigatorAddNewItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorAddNewItem.Name = "BindingNavigatorAddNewItem"
        Me.BindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorAddNewItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorAddNewItem.Text = "Add Entry"
        Me.BindingNavigatorAddNewItem.Visible = False
        '
        'BindingNavigatorCountItem
        '
        Me.BindingNavigatorCountItem.Name = "BindingNavigatorCountItem"
        Me.BindingNavigatorCountItem.Size = New System.Drawing.Size(44, 22)
        Me.BindingNavigatorCountItem.Text = "von {0}"
        Me.BindingNavigatorCountItem.ToolTipText = "Total Number of entries"
        '
        'BindingNavigatorDeleteItem
        '
        Me.BindingNavigatorDeleteItem.CheckOnClick = True
        Me.BindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorDeleteItem.Image = CType(resources.GetObject("BindingNavigatorDeleteItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorDeleteItem.Name = "BindingNavigatorDeleteItem"
        Me.BindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorDeleteItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorDeleteItem.Text = "Delete Entry"
        '
        'BindingNavigatorMoveFirstItem
        '
        Me.BindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveFirstItem.Image = CType(resources.GetObject("BindingNavigatorMoveFirstItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveFirstItem.Name = "BindingNavigatorMoveFirstItem"
        Me.BindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveFirstItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMoveFirstItem.Text = "Move First"
        '
        'BindingNavigatorMovePreviousItem
        '
        Me.BindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMovePreviousItem.Image = CType(resources.GetObject("BindingNavigatorMovePreviousItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMovePreviousItem.Name = "BindingNavigatorMovePreviousItem"
        Me.BindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMovePreviousItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMovePreviousItem.Text = "Move Previous"
        '
        'BindingNavigatorSeparator
        '
        Me.BindingNavigatorSeparator.Name = "BindingNavigatorSeparator"
        Me.BindingNavigatorSeparator.Size = New System.Drawing.Size(6, 25)
        '
        'BindingNavigatorPositionItem
        '
        Me.BindingNavigatorPositionItem.AccessibleName = "Position"
        Me.BindingNavigatorPositionItem.AutoSize = False
        Me.BindingNavigatorPositionItem.Name = "BindingNavigatorPositionItem"
        Me.BindingNavigatorPositionItem.Size = New System.Drawing.Size(70, 21)
        Me.BindingNavigatorPositionItem.Text = "0"
        Me.BindingNavigatorPositionItem.ToolTipText = "Current Position"
        '
        'BindingNavigatorSeparator1
        '
        Me.BindingNavigatorSeparator1.Name = "BindingNavigatorSeparator1"
        Me.BindingNavigatorSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'BindingNavigatorMoveNextItem
        '
        Me.BindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveNextItem.Image = CType(resources.GetObject("BindingNavigatorMoveNextItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveNextItem.Name = "BindingNavigatorMoveNextItem"
        Me.BindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveNextItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMoveNextItem.Text = "Move Next"
        '
        'BindingNavigatorMoveLastItem
        '
        Me.BindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveLastItem.Image = CType(resources.GetObject("BindingNavigatorMoveLastItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveLastItem.Name = "BindingNavigatorMoveLastItem"
        Me.BindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveLastItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMoveLastItem.Text = "Move Last"
        '
        'BindingNavigatorSeparator2
        '
        Me.BindingNavigatorSeparator2.Name = "BindingNavigatorSeparator2"
        Me.BindingNavigatorSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'VidéoBindingNavigatorSaveItem
        '
        Me.VidéoBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.VidéoBindingNavigatorSaveItem.Image = CType(resources.GetObject("VidéoBindingNavigatorSaveItem.Image"), System.Drawing.Image)
        Me.VidéoBindingNavigatorSaveItem.Name = "VidéoBindingNavigatorSaveItem"
        Me.VidéoBindingNavigatorSaveItem.Size = New System.Drawing.Size(23, 22)
        Me.VidéoBindingNavigatorSaveItem.Text = "Save Datas"
        '
        'mnuFile
        '
        Me.mnuFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1, Me.ToolStripMenuItem2, Me.DebugToolStripMenuItem})
        Me.mnuFile.Location = New System.Drawing.Point(0, 0)
        Me.mnuFile.Name = "mnuFile"
        Me.mnuFile.Size = New System.Drawing.Size(608, 24)
        Me.mnuFile.TabIndex = 0
        Me.mnuFile.Text = "File"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LoadConfigurationFileToolStripMenuItem, Me.SaveConfigFileToolStripMenuItem, Me.SaveConfigFileAsToolStripMenuItem, Me.ToolStripSeparator1, Me.ExitToolStripMenuItem})
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(35, 20)
        Me.ToolStripMenuItem1.Text = "File"
        '
        'LoadConfigurationFileToolStripMenuItem
        '
        Me.LoadConfigurationFileToolStripMenuItem.Name = "LoadConfigurationFileToolStripMenuItem"
        Me.LoadConfigurationFileToolStripMenuItem.Size = New System.Drawing.Size(189, 22)
        Me.LoadConfigurationFileToolStripMenuItem.Text = "Load config File"
        '
        'SaveConfigFileToolStripMenuItem
        '
        Me.SaveConfigFileToolStripMenuItem.Name = "SaveConfigFileToolStripMenuItem"
        Me.SaveConfigFileToolStripMenuItem.Size = New System.Drawing.Size(189, 22)
        Me.SaveConfigFileToolStripMenuItem.Text = "Save config file"
        '
        'SaveConfigFileAsToolStripMenuItem
        '
        Me.SaveConfigFileAsToolStripMenuItem.Name = "SaveConfigFileAsToolStripMenuItem"
        Me.SaveConfigFileAsToolStripMenuItem.Size = New System.Drawing.Size(189, 22)
        Me.SaveConfigFileAsToolStripMenuItem.Text = "Save config File as ..."
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(186, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(189, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InternetLinksToolStripMenuItem, Me.UserManualToolStripMenuItem, Me.AboutToolStripMenuItem})
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(40, 20)
        Me.ToolStripMenuItem2.Text = "Help"
        '
        'InternetLinksToolStripMenuItem
        '
        Me.InternetLinksToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MyFilmsWikiToolStripMenuItem, Me.AMCUpdaterSourceforgeToolStripMenuItem, Me.AntMovieCatalogToolStripMenuItem, Me.MediaInfodllToolStripMenuItem})
        Me.InternetLinksToolStripMenuItem.Name = "InternetLinksToolStripMenuItem"
        Me.InternetLinksToolStripMenuItem.Size = New System.Drawing.Size(151, 22)
        Me.InternetLinksToolStripMenuItem.Text = "Internet Links"
        '
        'MyFilmsWikiToolStripMenuItem
        '
        Me.MyFilmsWikiToolStripMenuItem.Name = "MyFilmsWikiToolStripMenuItem"
        Me.MyFilmsWikiToolStripMenuItem.Size = New System.Drawing.Size(219, 22)
        Me.MyFilmsWikiToolStripMenuItem.Text = "MyFilms Wiki"
        '
        'AMCUpdaterSourceforgeToolStripMenuItem
        '
        Me.AMCUpdaterSourceforgeToolStripMenuItem.Name = "AMCUpdaterSourceforgeToolStripMenuItem"
        Me.AMCUpdaterSourceforgeToolStripMenuItem.Size = New System.Drawing.Size(219, 22)
        Me.AMCUpdaterSourceforgeToolStripMenuItem.Text = "AMC Updater (Sourceforge)"
        '
        'AntMovieCatalogToolStripMenuItem
        '
        Me.AntMovieCatalogToolStripMenuItem.Name = "AntMovieCatalogToolStripMenuItem"
        Me.AntMovieCatalogToolStripMenuItem.Size = New System.Drawing.Size(219, 22)
        Me.AntMovieCatalogToolStripMenuItem.Text = "Ant Movie Catalog"
        '
        'MediaInfodllToolStripMenuItem
        '
        Me.MediaInfodllToolStripMenuItem.Name = "MediaInfodllToolStripMenuItem"
        Me.MediaInfodllToolStripMenuItem.Size = New System.Drawing.Size(219, 22)
        Me.MediaInfodllToolStripMenuItem.Text = "MediaInfo.dll"
        '
        'UserManualToolStripMenuItem
        '
        Me.UserManualToolStripMenuItem.Name = "UserManualToolStripMenuItem"
        Me.UserManualToolStripMenuItem.Size = New System.Drawing.Size(151, 22)
        Me.UserManualToolStripMenuItem.Text = "User Manual"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(151, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'DebugToolStripMenuItem
        '
        Me.DebugToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.XMLToolStripMenuItem, Me.MediaFileToolStripMenuItem, Me.NonMediaFilesToolStripMenuItem, Me.TrailerFilesToolStripMenuItem, Me.OrphanMediaToolStripMenuItem, Me.OrphanNonMediaToolStripMenuItem, Me.OrphanTrailerToolStripMenuItem, Me.MultiPartFilesToolStripMenuItem, Me.OrphanAntToolStripMenuItem, Me.MultiPartProcessedFilesToolStripMenuItem, Me.AntFieldsToolStripMenuItem, Me.NodesToProcessToolStripMenuItem, Me.ListMediaInfoParamsToolStripMenuItem})
        Me.DebugToolStripMenuItem.Name = "DebugToolStripMenuItem"
        Me.DebugToolStripMenuItem.Size = New System.Drawing.Size(50, 20)
        Me.DebugToolStripMenuItem.Text = "Debug"
        '
        'XMLToolStripMenuItem
        '
        Me.XMLToolStripMenuItem.Name = "XMLToolStripMenuItem"
        Me.XMLToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.XMLToolStripMenuItem.Text = "XML"
        '
        'MediaFileToolStripMenuItem
        '
        Me.MediaFileToolStripMenuItem.Name = "MediaFileToolStripMenuItem"
        Me.MediaFileToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.MediaFileToolStripMenuItem.Text = "Media Files"
        '
        'NonMediaFilesToolStripMenuItem
        '
        Me.NonMediaFilesToolStripMenuItem.Name = "NonMediaFilesToolStripMenuItem"
        Me.NonMediaFilesToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.NonMediaFilesToolStripMenuItem.Text = "Non Media Files"
        '
        'TrailerFilesToolStripMenuItem
        '
        Me.TrailerFilesToolStripMenuItem.Name = "TrailerFilesToolStripMenuItem"
        Me.TrailerFilesToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.TrailerFilesToolStripMenuItem.Text = "Trailer Media Files"
        '
        'OrphanMediaToolStripMenuItem
        '
        Me.OrphanMediaToolStripMenuItem.Name = "OrphanMediaToolStripMenuItem"
        Me.OrphanMediaToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.OrphanMediaToolStripMenuItem.Text = "Orphan Media"
        '
        'OrphanNonMediaToolStripMenuItem
        '
        Me.OrphanNonMediaToolStripMenuItem.Name = "OrphanNonMediaToolStripMenuItem"
        Me.OrphanNonMediaToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.OrphanNonMediaToolStripMenuItem.Text = "Orphan Non Media"
        '
        'OrphanTrailerToolStripMenuItem
        '
        Me.OrphanTrailerToolStripMenuItem.Name = "OrphanTrailerToolStripMenuItem"
        Me.OrphanTrailerToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.OrphanTrailerToolStripMenuItem.Text = "Orphan Trailer"
        '
        'MultiPartFilesToolStripMenuItem
        '
        Me.MultiPartFilesToolStripMenuItem.Name = "MultiPartFilesToolStripMenuItem"
        Me.MultiPartFilesToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.MultiPartFilesToolStripMenuItem.Text = "Multi-Part Files"
        '
        'OrphanAntToolStripMenuItem
        '
        Me.OrphanAntToolStripMenuItem.Name = "OrphanAntToolStripMenuItem"
        Me.OrphanAntToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.OrphanAntToolStripMenuItem.Text = "Orphan Ant"
        '
        'MultiPartProcessedFilesToolStripMenuItem
        '
        Me.MultiPartProcessedFilesToolStripMenuItem.Name = "MultiPartProcessedFilesToolStripMenuItem"
        Me.MultiPartProcessedFilesToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.MultiPartProcessedFilesToolStripMenuItem.Text = "MultiPartProcessedFiles"
        '
        'AntFieldsToolStripMenuItem
        '
        Me.AntFieldsToolStripMenuItem.Name = "AntFieldsToolStripMenuItem"
        Me.AntFieldsToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.AntFieldsToolStripMenuItem.Text = "AntFields"
        '
        'NodesToProcessToolStripMenuItem
        '
        Me.NodesToProcessToolStripMenuItem.Name = "NodesToProcessToolStripMenuItem"
        Me.NodesToProcessToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.NodesToProcessToolStripMenuItem.Text = "NodesToProcess"
        '
        'ListMediaInfoParamsToolStripMenuItem
        '
        Me.ListMediaInfoParamsToolStripMenuItem.Name = "ListMediaInfoParamsToolStripMenuItem"
        Me.ListMediaInfoParamsToolStripMenuItem.Size = New System.Drawing.Size(197, 22)
        Me.ListMediaInfoParamsToolStripMenuItem.Text = "List MediaInfo Params"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripFixedText, Me.ToolStripStatusLabel, Me.ToolStripProgressBar})
        Me.StatusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 636)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(608, 22)
        Me.StatusStrip1.SizingGrip = False
        Me.StatusStrip1.TabIndex = 54
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripFixedText
        '
        Me.ToolStripFixedText.Name = "ToolStripFixedText"
        Me.ToolStripFixedText.Size = New System.Drawing.Size(48, 17)
        Me.ToolStripFixedText.Text = "Status : "
        '
        'ToolStripStatusLabel
        '
        Me.ToolStripStatusLabel.Name = "ToolStripStatusLabel"
        Me.ToolStripStatusLabel.Size = New System.Drawing.Size(0, 17)
        '
        'ToolStripProgressBar
        '
        Me.ToolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripProgressBar.Name = "ToolStripProgressBar"
        Me.ToolStripProgressBar.Size = New System.Drawing.Size(200, 16)
        '
        'epInteractive
        '
        Me.epInteractive.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.epInteractive.ContainerControl = Me
        '
        'epManualUpdater
        '
        Me.epManualUpdater.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.epManualUpdater.ContainerControl = Me
        '
        'epOptions
        '
        Me.epOptions.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.epOptions.ContainerControl = Me
        '
        'ToolTip1
        '
        Me.ToolTip1.AutoPopDelay = 15000
        Me.ToolTip1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ToolTip1.InitialDelay = 1000
        Me.ToolTip1.IsBalloon = True
        Me.ToolTip1.ReshowDelay = 100
        Me.ToolTip1.ToolTipTitle = "MyFilms AMCupdater Help ..."
        '
        'AntMovieCatalog
        '
        Me.AntMovieCatalog.DataSetName = "AntMovieCatalog"
        Me.AntMovieCatalog.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'ListVideos
        '
        Me.ListVideos.AutoGenerateColumns = False
        Me.ListVideos.ColumnHeadersHeight = 21
        Me.ListVideos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.ListVideos.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.NumberDataGridViewTextBoxColumn, Me.OriginalTitleDataGridViewTextBoxColumn, Me.TranslatedTitleDataGridViewTextBoxColumn, Me.YearDataGridViewTextBoxColumn, Me.DateAddedDataGridViewTextBoxColumn})
        Me.ListVideos.DataSource = Me.VideoBindingSource
        DataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.ListVideos.DefaultCellStyle = DataGridViewCellStyle13
        Me.ListVideos.Location = New System.Drawing.Point(-2, 28)
        Me.ListVideos.Name = "ListVideos"
        Me.ListVideos.ReadOnly = True
        Me.ListVideos.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle14.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ListVideos.RowHeadersDefaultCellStyle = DataGridViewCellStyle14
        Me.ListVideos.RowHeadersVisible = False
        Me.ListVideos.RowHeadersWidth = 20
        Me.ListVideos.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.ListVideos.RowTemplate.Height = 16
        Me.ListVideos.Size = New System.Drawing.Size(184, 441)
        Me.ListVideos.TabIndex = 1
        '
        'NumberDataGridViewTextBoxColumn
        '
        Me.NumberDataGridViewTextBoxColumn.DataPropertyName = "Number"
        Me.NumberDataGridViewTextBoxColumn.HeaderText = "N°"
        Me.NumberDataGridViewTextBoxColumn.Name = "NumberDataGridViewTextBoxColumn"
        Me.NumberDataGridViewTextBoxColumn.ReadOnly = True
        Me.NumberDataGridViewTextBoxColumn.Width = 35
        '
        'OriginalTitleDataGridViewTextBoxColumn
        '
        Me.OriginalTitleDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.OriginalTitleDataGridViewTextBoxColumn.DataPropertyName = "OriginalTitle"
        Me.OriginalTitleDataGridViewTextBoxColumn.HeaderText = "Original Title"
        Me.OriginalTitleDataGridViewTextBoxColumn.Name = "OriginalTitleDataGridViewTextBoxColumn"
        Me.OriginalTitleDataGridViewTextBoxColumn.ReadOnly = True
        Me.OriginalTitleDataGridViewTextBoxColumn.Width = 90
        '
        'TranslatedTitleDataGridViewTextBoxColumn
        '
        Me.TranslatedTitleDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.TranslatedTitleDataGridViewTextBoxColumn.DataPropertyName = "TranslatedTitle"
        Me.TranslatedTitleDataGridViewTextBoxColumn.HeaderText = "Translated Title"
        Me.TranslatedTitleDataGridViewTextBoxColumn.Name = "TranslatedTitleDataGridViewTextBoxColumn"
        Me.TranslatedTitleDataGridViewTextBoxColumn.ReadOnly = True
        Me.TranslatedTitleDataGridViewTextBoxColumn.Width = 105
        '
        'YearDataGridViewTextBoxColumn
        '
        Me.YearDataGridViewTextBoxColumn.DataPropertyName = "Year"
        Me.YearDataGridViewTextBoxColumn.HeaderText = "Year"
        Me.YearDataGridViewTextBoxColumn.Name = "YearDataGridViewTextBoxColumn"
        Me.YearDataGridViewTextBoxColumn.ReadOnly = True
        Me.YearDataGridViewTextBoxColumn.Width = 35
        '
        'DateAddedDataGridViewTextBoxColumn
        '
        Me.DateAddedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.DateAddedDataGridViewTextBoxColumn.DataPropertyName = "DateAdded"
        Me.DateAddedDataGridViewTextBoxColumn.HeaderText = "Date Added"
        Me.DateAddedDataGridViewTextBoxColumn.Name = "DateAddedDataGridViewTextBoxColumn"
        Me.DateAddedDataGridViewTextBoxColumn.ReadOnly = True
        Me.DateAddedDataGridViewTextBoxColumn.Width = 89
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(608, 658)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.mnuFile)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.mnuFile
        Me.Name = "Form1"
        Me.Text = "Ant Movie Catalog Auto-Updater"
        Me.TabControl1.ResumeLayout(False)
        Me.Interactive.ResumeLayout(False)
        Me.Interactive.PerformLayout()
        Me.GroupBox24.ResumeLayout(False)
        Me.GroupBox24.PerformLayout()
        Me.GroupBox23.ResumeLayout(False)
        Me.GroupBox23.PerformLayout()
        Me.GroupBox27.ResumeLayout(False)
        Me.GroupBox27.PerformLayout()
        Me.GroupBox26.ResumeLayout(False)
        Me.GroupBox26.PerformLayout()
        Me.GroupBox17.ResumeLayout(False)
        Me.GroupBox17.PerformLayout()
        Me.GroupBox8.ResumeLayout(False)
        Me.GroupBox8.PerformLayout()
        Me.Options.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox25.ResumeLayout(False)
        Me.GroupBox25.PerformLayout()
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.GroupBox13.ResumeLayout(False)
        Me.GroupBox13.PerformLayout()
        Me.GroupBox12.ResumeLayout(False)
        Me.GroupBox12.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.ScanFilters.ResumeLayout(False)
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.GroupBox20.ResumeLayout(False)
        Me.GroupBox20.PerformLayout()
        CType(Me.dgFilterStrings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox19.ResumeLayout(False)
        Me.GroupBox19.PerformLayout()
        CType(Me.dgExcludedFolderStrings, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgExcludedFileStrings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.DatabaseFields.ResumeLayout(False)
        Me.DatabaseFields.PerformLayout()
        Me.GroupBox16.ResumeLayout(False)
        Me.GroupBox16.PerformLayout()
        Me.Manual.ResumeLayout(False)
        Me.grpManualInternetLookupSettings.ResumeLayout(False)
        Me.grpManualInternetLookupSettings.PerformLayout()
        Me.grpManualUpdatesParameters.ResumeLayout(False)
        Me.grpManualUpdatesParameters.PerformLayout()
        Me.GroupBox10.ResumeLayout(False)
        Me.GroupBox10.PerformLayout()
        Me.GroupBox9.ResumeLayout(False)
        Me.GroupBox9.PerformLayout()
        Me.Test.ResumeLayout(False)
        Me.Test.PerformLayout()
        Me.GroupBox18.ResumeLayout(False)
        Me.GroupBox18.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ViewCollection.ResumeLayout(False)
        Me.ViewCollection.PerformLayout()
        Me.GroupBoxMovieDetails.ResumeLayout(False)
        Me.GroupBoxMovieDetails.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.VideoBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox14.ResumeLayout(False)
        Me.GroupBox14.PerformLayout()
        Me.GroupBox22.ResumeLayout(False)
        Me.GroupBox22.PerformLayout()
        Me.GroupBox21.ResumeLayout(False)
        Me.GroupBox21.PerformLayout()
        Me.GroupBox15.ResumeLayout(False)
        Me.GroupBox15.PerformLayout()
        Me.XionPanel1.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.VidéoBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
        Me.VidéoBindingNavigator.ResumeLayout(False)
        Me.VidéoBindingNavigator.PerformLayout()
        Me.mnuFile.ResumeLayout(False)
        Me.mnuFile.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        CType(Me.epInteractive, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.epManualUpdater, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.epOptions, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AntMovieCatalog, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ListVideos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents Interactive As System.Windows.Forms.TabPage
    Friend WithEvents Options As System.Windows.Forms.TabPage
    Friend WithEvents btnProcessMovieList As System.Windows.Forms.Button
    Friend WithEvents btnParseXML As System.Windows.Forms.Button
    Friend WithEvents Test As System.Windows.Forms.TabPage
    Friend WithEvents btnGetSampleFile As System.Windows.Forms.Button
    Friend WithEvents txtSampleFile As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnTestAnalyse As System.Windows.Forms.Button
    Friend WithEvents txtSampleVideoCodec As System.Windows.Forms.TextBox
    Friend WithEvents txtSampleVideoResolution As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtSampleVideoFramerate As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtSampleVideoBitrate As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents txtSampleAudioBitrate As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtSampleAudioCodec As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents txtSampleFileLength As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtSampleFileSize As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents btnFindOrphans As System.Windows.Forms.Button
    Friend WithEvents btnUpdateXML As System.Windows.Forms.Button
    Friend WithEvents btnJustDoIt As System.Windows.Forms.Button
    Friend WithEvents mnuFile As System.Windows.Forms.MenuStrip
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents InternetLinksToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AMCUpdaterSourceforgeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AntMovieCatalogToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MediaInfodllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnCancelProcessing As System.Windows.Forms.Button
    Friend WithEvents btnShowHideLog As System.Windows.Forms.Button
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripProgressBar As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents ToolStripFixedText As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents GroupBox8 As System.Windows.Forms.GroupBox
    Friend WithEvents Manual As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox9 As System.Windows.Forms.GroupBox
    Friend WithEvents txtManualXMLPath As System.Windows.Forms.TextBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents btnManualSelectXMLFile As System.Windows.Forms.Button
    Friend WithEvents GroupBox10 As System.Windows.Forms.GroupBox
    Friend WithEvents cbManualSelectOperation As System.Windows.Forms.ComboBox
    Friend WithEvents btnManualApplyChanges As System.Windows.Forms.Button
    Friend WithEvents btnManualDoTest As System.Windows.Forms.Button
    Friend WithEvents grpManualUpdatesParameters As System.Windows.Forms.GroupBox
    Friend WithEvents txtManualNewValue As System.Windows.Forms.TextBox
    Friend WithEvents lblManualEnterNewValue As System.Windows.Forms.Label
    Friend WithEvents lblManualSelectField As System.Windows.Forms.Label
    Friend WithEvents cbManualSelectField As System.Windows.Forms.ComboBox
    Friend WithEvents lblManualParametersValue1 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents txtManualParameterValue1 As System.Windows.Forms.TextBox
    Friend WithEvents cbManualParameterOperator1 As System.Windows.Forms.ComboBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents cbManualParameterFieldList1 As System.Windows.Forms.ComboBox
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents chkManualParametersUpdateAll As System.Windows.Forms.CheckBox
    Friend WithEvents DebugToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents XMLToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MediaFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TrailerFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NonMediaFilesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OrphanMediaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OrphanNonMediaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MultiPartFilesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OrphanAntToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MultiPartProcessedFilesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AntFieldsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NodesToProcessToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnShowHideLogTest As System.Windows.Forms.Button
    Friend WithEvents epInteractive As System.Windows.Forms.ErrorProvider
    Friend WithEvents epManualUpdater As System.Windows.Forms.ErrorProvider
    Friend WithEvents LoadConfigurationFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveConfigFileAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents txtDefaultSourceField As System.Windows.Forms.TextBox
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents GroupBox12 As System.Windows.Forms.GroupBox
    Friend WithEvents chkExecuteOnlyForOrphans As System.Windows.Forms.CheckBox
    Friend WithEvents btnExecuteProgramSelectPath As System.Windows.Forms.Button
    Friend WithEvents txtExecuteProgramPath As System.Windows.Forms.TextBox
    Friend WithEvents chkExecuteProgram As System.Windows.Forms.CheckBox
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents GroupBox13 As System.Windows.Forms.GroupBox
    Friend WithEvents cbLogLevel As System.Windows.Forms.ComboBox
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents DatabaseFields As System.Windows.Forms.TabPage
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents btnDBFieldsSelectNoInternet As System.Windows.Forms.Button
    Friend WithEvents btnDBFieldsSelectAllInternet As System.Windows.Forms.Button
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents btnDBFieldsSelectNoMedia As System.Windows.Forms.Button
    Friend WithEvents btnDBFieldsSelectAllMedia As System.Windows.Forms.Button
    Friend WithEvents GroupBox16 As System.Windows.Forms.GroupBox
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents btnDBFieldsSelectNone As System.Windows.Forms.Button
    Friend WithEvents btnDBFieldsSelectAll As System.Windows.Forms.Button
    Friend WithEvents cbDatabaseFields As System.Windows.Forms.CheckedListBox
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents cbDateHandling As System.Windows.Forms.ComboBox
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents txtSampleAudioStreamCount As System.Windows.Forms.TextBox
    Friend WithEvents Label35 As System.Windows.Forms.Label
    Friend WithEvents Label37 As System.Windows.Forms.Label
    Friend WithEvents Label36 As System.Windows.Forms.Label
    Friend WithEvents txtSampleAudioLanguageList As System.Windows.Forms.TextBox
    Friend WithEvents txtSampleAudioStreamList As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox18 As System.Windows.Forms.GroupBox
    Friend WithEvents txtSampleTextLanguageList As System.Windows.Forms.TextBox
    Friend WithEvents txtSampleTextCodecList As System.Windows.Forms.TextBox
    Friend WithEvents Label39 As System.Windows.Forms.Label
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents UserManualToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ListMediaInfoParamsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblManualParametersValue2 As System.Windows.Forms.Label
    Friend WithEvents lblManualParametersOperator2 As System.Windows.Forms.Label
    Friend WithEvents txtManualParameterValue2 As System.Windows.Forms.TextBox
    Friend WithEvents cbManualParameterOperator2 As System.Windows.Forms.ComboBox
    Friend WithEvents lblManualParametersField2 As System.Windows.Forms.Label
    Friend WithEvents cbManualParameterFieldList2 As System.Windows.Forms.ComboBox
    Friend WithEvents cbManualParameterAndOr As System.Windows.Forms.ComboBox
    Friend WithEvents grpManualInternetLookupSettings As System.Windows.Forms.GroupBox
    Friend WithEvents Label42 As System.Windows.Forms.Label
    Friend WithEvents txtManualExcludedMoviesPath As System.Windows.Forms.TextBox
    Friend WithEvents Label40 As System.Windows.Forms.Label
    Friend WithEvents btnManualSelectExcludedMoviesFile As System.Windows.Forms.Button
    Friend WithEvents btnManualSelectInternetParserPath As System.Windows.Forms.Button
    Friend WithEvents cbManualInternetLookupBehaviour As System.Windows.Forms.ComboBox
    Friend WithEvents txtManualInternetParserPath As System.Windows.Forms.TextBox
    Friend WithEvents Label41 As System.Windows.Forms.Label
    Friend WithEvents lblManualDatabaseFieldsPrompt As System.Windows.Forms.Label
    Friend WithEvents btnManualCancel As System.Windows.Forms.Button
    Friend WithEvents lblInternetLookupRequired As System.Windows.Forms.Label
    Friend WithEvents ScanFilters As System.Windows.Forms.TabPage
    Friend WithEvents dgExcludedFileStrings As System.Windows.Forms.DataGridView
    Friend WithEvents dgExcludedFolderStrings As System.Windows.Forms.DataGridView
    Friend WithEvents Label45 As System.Windows.Forms.Label
    Friend WithEvents Label44 As System.Windows.Forms.Label
    Friend WithEvents dgFilterStrings As System.Windows.Forms.DataGridView
    Friend WithEvents epOptions As System.Windows.Forms.ErrorProvider
    Friend WithEvents cbCheckHandling As System.Windows.Forms.ComboBox
    Friend WithEvents lblCheckFieldHandling As System.Windows.Forms.Label
    Friend WithEvents GroupBox19 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox20 As System.Windows.Forms.GroupBox
    Friend WithEvents Label46 As System.Windows.Forms.Label
    Friend WithEvents Label47 As System.Windows.Forms.Label
    Friend WithEvents chkFolderNameIsGroupName As System.Windows.Forms.CheckBox
    Friend WithEvents cbGroupNameAppliesTo As System.Windows.Forms.ComboBox
    Friend WithEvents lblGroupNameAppliesTo As System.Windows.Forms.Label
    Friend WithEvents cbPictureHandling As System.Windows.Forms.ComboBox
    Friend WithEvents lblPictureHandling As System.Windows.Forms.Label
    Friend WithEvents txtPictureFilenamePrefix As System.Windows.Forms.TextBox
    Friend WithEvents lblPicturePrefix As System.Windows.Forms.Label
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents chkManualMissingFanartDownload As System.Windows.Forms.CheckBox
    Friend WithEvents ViewCollection As System.Windows.Forms.TabPage
    Friend WithEvents AntMovieCatalog As AMCUpdater.AntMovieCatalog
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents VidéoBindingNavigator As System.Windows.Forms.BindingNavigator
    Friend WithEvents BindingNavigatorAddNewItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorCountItem As System.Windows.Forms.ToolStripLabel
    Friend WithEvents BindingNavigatorDeleteItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveFirstItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMovePreviousItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorPositionItem As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents BindingNavigatorSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorMoveNextItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveLastItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents VidéoBindingNavigatorSaveItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents VideoBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents Label50 As System.Windows.Forms.Label
    Friend WithEvents TextBox5 As System.Windows.Forms.TextBox
    Friend WithEvents Label49 As System.Windows.Forms.Label
    Friend WithEvents RichTextBox2 As System.Windows.Forms.RichTextBox
    Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
    Friend WithEvents GroupBox14 As System.Windows.Forms.GroupBox
    Friend WithEvents TextBox7 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox6 As System.Windows.Forms.TextBox
    Friend WithEvents RichTextBox3 As System.Windows.Forms.RichTextBox
    Friend WithEvents Label53 As System.Windows.Forms.Label
    Friend WithEvents Label52 As System.Windows.Forms.Label
    Friend WithEvents Label51 As System.Windows.Forms.Label
    Friend WithEvents Label54 As System.Windows.Forms.Label
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents Label56 As System.Windows.Forms.Label
    Friend WithEvents TextBox8 As System.Windows.Forms.TextBox
    Friend WithEvents Label57 As System.Windows.Forms.Label
    Friend WithEvents TextBox9 As System.Windows.Forms.TextBox
    Friend WithEvents Label55 As System.Windows.Forms.Label
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Friend WithEvents Label58 As System.Windows.Forms.Label
    Friend WithEvents ListVideos As System.Windows.Forms.DataGridView
    Friend WithEvents NumberDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents OriginalTitleDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TranslatedTitleDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents YearDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DateAddedDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Label61 As System.Windows.Forms.Label
    Friend WithEvents TextBox11 As System.Windows.Forms.TextBox
    Friend WithEvents Label60 As System.Windows.Forms.Label
    Friend WithEvents Label59 As System.Windows.Forms.Label
    Friend WithEvents TextBox10 As System.Windows.Forms.TextBox
    Friend WithEvents Label64 As System.Windows.Forms.Label
    Friend WithEvents TextBox14 As System.Windows.Forms.TextBox
    Friend WithEvents Label65 As System.Windows.Forms.Label
    Friend WithEvents TextBox15 As System.Windows.Forms.TextBox
    Friend WithEvents Label62 As System.Windows.Forms.Label
    Friend WithEvents TextBox12 As System.Windows.Forms.TextBox
    Friend WithEvents Label63 As System.Windows.Forms.Label
    Friend WithEvents TextBox13 As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox15 As System.Windows.Forms.GroupBox
    Friend WithEvents Label68 As System.Windows.Forms.Label
    Friend WithEvents TextBox18 As System.Windows.Forms.TextBox
    Friend WithEvents Label67 As System.Windows.Forms.Label
    Friend WithEvents TextBox17 As System.Windows.Forms.TextBox
    Friend WithEvents Label66 As System.Windows.Forms.Label
    Friend WithEvents TextBox16 As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox21 As System.Windows.Forms.GroupBox
    Friend WithEvents Label69 As System.Windows.Forms.Label
    Friend WithEvents Label70 As System.Windows.Forms.Label
    Friend WithEvents TextBox19 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox20 As System.Windows.Forms.TextBox
    Friend WithEvents Label72 As System.Windows.Forms.Label
    Friend WithEvents TextBox22 As System.Windows.Forms.TextBox
    Friend WithEvents Label71 As System.Windows.Forms.Label
    Friend WithEvents TextBox21 As System.Windows.Forms.TextBox
    Friend WithEvents Label74 As System.Windows.Forms.Label
    Friend WithEvents TextBox24 As System.Windows.Forms.TextBox
    Friend WithEvents Label73 As System.Windows.Forms.Label
    Friend WithEvents TextBox23 As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox22 As System.Windows.Forms.GroupBox
    Friend WithEvents Label75 As System.Windows.Forms.Label
    Friend WithEvents Label76 As System.Windows.Forms.Label
    Friend WithEvents TextBox25 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox26 As System.Windows.Forms.TextBox
    Friend WithEvents Label78 As System.Windows.Forms.Label
    Friend WithEvents TextBox28 As System.Windows.Forms.TextBox
    Friend WithEvents Label77 As System.Windows.Forms.Label
    Friend WithEvents TextBox27 As System.Windows.Forms.TextBox
    Friend WithEvents XionPanel1 As XionControls.XionPanel
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents cbMasterTitle As System.Windows.Forms.ComboBox
    Friend WithEvents lblMasterTitle As System.Windows.Forms.Label
    Friend WithEvents chkManualMissingTrailer As System.Windows.Forms.CheckBox
    Friend WithEvents SaveConfigFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripProgressMessage As System.Windows.Forms.Label
    Friend WithEvents txtManualOldValue As System.Windows.Forms.TextBox
    Friend WithEvents lblManualEnterOldValue As System.Windows.Forms.Label
    Friend WithEvents Label80 As System.Windows.Forms.Label
    Friend WithEvents TextBox29 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox31 As System.Windows.Forms.TextBox
    Friend WithEvents Label82 As System.Windows.Forms.Label
    Friend WithEvents TextBox30 As System.Windows.Forms.TextBox
    Friend WithEvents Label81 As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents TrailerFilesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OrphanTrailerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MyFilmsWikiToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label83 As System.Windows.Forms.Label
    Friend WithEvents GroupBox23 As System.Windows.Forms.GroupBox
    Friend WithEvents chkDontAskInteractive As System.Windows.Forms.CheckBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents btnSelectParserFile As System.Windows.Forms.Button
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents txtParserFilePath As System.Windows.Forms.TextBox
    Friend WithEvents cbInternetLookupBehaviour As System.Windows.Forms.ComboBox
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents GroupBox17 As System.Windows.Forms.GroupBox
    Friend WithEvents chkRescanMovedFiles As System.Windows.Forms.CheckBox
    Friend WithEvents chkProhibitInternetLookup As System.Windows.Forms.CheckBox
    Friend WithEvents chkImportOnInternetFail As System.Windows.Forms.CheckBox
    Friend WithEvents chkPurgeMissing As System.Windows.Forms.CheckBox
    Friend WithEvents chkOverwriteXML As System.Windows.Forms.CheckBox
    Friend WithEvents chkBackupXMLFirst As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Friend WithEvents btnSelectFanartFolder As System.Windows.Forms.Button
    Friend WithEvents txtFanartFolder As System.Windows.Forms.TextBox
    Friend WithEvents Label48 As System.Windows.Forms.Label
    Friend WithEvents txtExcludeFilePath As System.Windows.Forms.TextBox
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents btnSelectExcludeFile As System.Windows.Forms.Button
    Friend WithEvents txtConfigFilePath As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnSelectConfigFile As System.Windows.Forms.Button
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents txtRegExSearchMultiPart As System.Windows.Forms.TextBox
    Friend WithEvents chkReadDVDLabel As System.Windows.Forms.CheckBox
    Friend WithEvents txtMediaLabel As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents txtMediaType As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents GroupBox24 As System.Windows.Forms.GroupBox
    Friend WithEvents btnSelectMovieFolder As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtMovieFolder As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents txtPersonArtworkFolder As System.Windows.Forms.TextBox
    Friend WithEvents btnSelectPersonArtworkFolder As System.Windows.Forms.Button
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents txtOverridePath As System.Windows.Forms.TextBox
    Friend WithEvents lblOverridePath As System.Windows.Forms.Label
    Friend WithEvents chkShortNames As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents chkParseSubtitleFiles As System.Windows.Forms.CheckBox
    Friend WithEvents txtTrailerIentificationStrings As System.Windows.Forms.TextBox
    Friend WithEvents chkParsePlaylistFiles As System.Windows.Forms.CheckBox
    Friend WithEvents Label79 As System.Windows.Forms.Label
    Friend WithEvents chkCheckDVDFolders As System.Windows.Forms.CheckBox
    Friend WithEvents txtDefaultFileTypesNonMedia As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents txtDefaultFileTypes As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents GroupBox25 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox26 As System.Windows.Forms.GroupBox
    Friend WithEvents chkGrabberOverrideTitleLimit As System.Windows.Forms.ComboBox
    Friend WithEvents chkGrabberOverridePersonLimit As System.Windows.Forms.ComboBox
    Friend WithEvents chkGrabberOverrideGetRoles As System.Windows.Forms.ComboBox
    Friend WithEvents chkGrabberOverrideLanguage As System.Windows.Forms.ComboBox
    Friend WithEvents Label84 As System.Windows.Forms.Label
    Friend WithEvents Label86 As System.Windows.Forms.Label
    Friend WithEvents Label85 As System.Windows.Forms.Label
    Friend WithEvents Label87 As System.Windows.Forms.Label
    Friend WithEvents chkParseTrailers As System.Windows.Forms.CheckBox
    Friend WithEvents cbTitleHandling As System.Windows.Forms.ComboBox
    Friend WithEvents Label43 As System.Windows.Forms.Label
    Friend WithEvents chkImportOnInternetFailIgnoreWhenInteractive As System.Windows.Forms.CheckBox
    Friend WithEvents txtParserFilePathDisplay As System.Windows.Forms.TextBox
    Friend WithEvents txtManualInternetParserPathDisplay As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox27 As System.Windows.Forms.GroupBox
    Friend WithEvents btnManualExcludedMoviesFileShow As System.Windows.Forms.Button
    Friend WithEvents btnExcludeFileDelete As System.Windows.Forms.Button
    Friend WithEvents btnExcludeFileShow As System.Windows.Forms.Button
    Friend WithEvents btnManualExcludedMoviesFileDelete As System.Windows.Forms.Button
    Friend WithEvents chkManualUpdateRecordsOnlyMissingData As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBoxMovieDetails As System.Windows.Forms.GroupBox
    Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn5 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Category As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Country As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Rating As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData As System.Windows.Forms.CheckBox
End Class
