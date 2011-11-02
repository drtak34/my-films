Imports System.Threading
Imports System.ComponentModel
Imports System.Text
Imports System.Security.Cryptography
Imports System.Management
Imports System.Windows.Forms
Imports MediaPortal.Configuration
'Imports MyFilmsPlugin

Public Class Form1

    Public XMLMovies As New ArrayList
    Public PhysicalMovies As New ArrayList
    Public OrphanedMovies As New ArrayList

    Public AntProcessor As New AntProcessor
    Public mydivx As AntMovieCatalog = New AntMovieCatalog()
    'Public mydivx As MyFilmsPlugin.AntMovieCatalog = New MyFilmsPlugin.AntMovieCatalog()

    Public OverridePath As String
    Public SourceField As String

    Private ValidOptions As Boolean = True

    Shared MediaData As Hashtable
    Shared InternetData As Hashtable

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
        Label_VersionNumber.Text = "V" + asm.GetName().Version.ToString()

#If CONFIG = "Release" Then
        ToolStripMenuItemDebug.Visible = False
        ToolStripMenuItemOptions.Visible = False
        ViewPersons.Visible = False
        ViewCatalog.Visible = False
#Else
        ToolStripMenuItemDebug.Visible = True
        ToolStripMenuItemOptions.Visible = True
        ViewPersons.Visible = True
        ViewCatalog.Visible = True
#End If


    End Sub

    Private Sub Form1_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Enter

    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If My.Settings.MainFormSize.Height <> 0 And My.Settings.MainFormSize.Width <> 0 Then
            Me.Size = My.Settings.MainFormSize
        End If
        If My.Settings.MainFormLocation.X <> 0 And My.Settings.MainFormLocation.Y <> 0 Then
            Me.Location = My.Settings.MainFormLocation
        End If

        Me.AddOwnedForm(dgLogWindow)
        If My.Settings.LogFormVisible = True Then
            dgLogWindow.Visible = True
        Else
            dgLogWindow.Visible = False
        End If
        If My.Settings.LogFormSize.Height <> 0 And My.Settings.LogFormSize.Width <> 0 Then
            dgLogWindow.Size = My.Settings.LogFormSize
        End If
        If My.Settings.LogFormLocation.X <> 0 And My.Settings.LogFormLocation.Y <> 0 Then
            dgLogWindow.Location = My.Settings.LogFormLocation
        End If

        'Dim lngLeft As Long
        'Dim lngTop As Long
        'Dim lngWidth As Long
        'Dim lngHeight As Long

        '' Get the latest position and size values
        'lngLeft = Val(GetSetting(App.EXEName, "Settings", "Left", "0"))
        'lngTop = Val(GetSetting(App.EXEName, "Settings", "Top", "0"))
        'lngWidth = Val(GetSetting(App.EXEName, "Settings", "Width", "0"))
        'lngHeight = Val(GetSetting(App.EXEName, "Settings", "Height", "0"))

        'If lngWidth > 0 Then
        '    Me.Move(lngLeft, lngTop, lngWidth, lngHeight)
        'End If


        'Me.Visible = True
        'Dim x As Integer
        'Dim y As Integer
        'x = Screen.PrimaryScreen.WorkingArea.Width
        'y = Screen.PrimaryScreen.WorkingArea.Height - Me.Height

        'Do Until x = Screen.PrimaryScreen.WorkingArea.Width - Me.Width
        '    x = x - 1
        '    Me.Location = New Point(x, y)
        'Loop

        If ToolStripStatusLabel.Text = "" Then
            ToolStripStatusLabel.Text = "Ready"
        End If
        If dgLogWindow.Visible = True Then
            btnShowHideLog.Text = "<< Hide Log"
        Else
            btnShowHideLog.Text = "Show Log >>"
        End If

        cbManualSelectField.DataSource = New DataView(AntProcessor.GetAntFieldNames())
        cbManualSelectField.DisplayMember = "FieldName"
        cbManualSelectField.ValueMember = "FieldName"
        cbManualSelectField.SelectedIndex = -1

        cbManualSelectFieldDestination.DataSource = New DataView(AntProcessor.GetAntFieldNames())
        cbManualSelectFieldDestination.DisplayMember = "FieldName"
        cbManualSelectFieldDestination.ValueMember = "FieldName"
        cbManualSelectFieldDestination.SelectedIndex = -1

        cbManualParameterFieldList1.DataSource = New DataView(AntProcessor.GetAntFieldNames())
        cbManualParameterFieldList1.DisplayMember = "FieldName"
        cbManualParameterFieldList1.ValueMember = "FieldName"
        cbManualParameterFieldList1.SelectedIndex = -1

        cbManualParameterFieldList2.DataSource = New DataView(AntProcessor.GetAntFieldNames())
        cbManualParameterFieldList2.DisplayMember = "FieldName"
        cbManualParameterFieldList2.ValueMember = "FieldName"
        cbManualParameterFieldList2.SelectedIndex = -1

        Dim dtInternetLookupBehaviour As New DataTable
        With dtInternetLookupBehaviour
            .Columns.Add("Value", System.Type.GetType("System.Boolean"))
            .Columns.Add("Display", System.Type.GetType("System.String"))
            .Rows.Add(True, "Always offer choice of movie")
            .Rows.Add(False, "Try to find best match automatically")
        End With
        Dim dvInternetLookupBehaviour1 As New DataView(dtInternetLookupBehaviour)
        Dim dvInternetLookupBehaviour2 As New DataView(dtInternetLookupBehaviour)
        With cbInternetLookupBehaviour
            .DataSource = dvInternetLookupBehaviour1
            .DisplayMember = "Display"
            .ValueMember = "Value"
            .SelectedIndex = -1
        End With
        With cbManualInternetLookupBehaviour
            .DataSource = dvInternetLookupBehaviour2
            .DisplayMember = "Display"
            .ValueMember = "Value"
            .SelectedIndex = -1
        End With

        MediaData = New Hashtable
        'MediaData.Add("originaltitle", "originaltitle")
        MediaData.Add("date", "date")
        MediaData.Add("length", "length")
        MediaData.Add("videobitrate", "videobitrate")
        MediaData.Add("audiobitrate", "audiobitrate")
        MediaData.Add("disks", "disks")
        MediaData.Add("checked", "checked")
        MediaData.Add("medialabel", "medialabel")
        MediaData.Add("mediatype", "mediatype")
        MediaData.Add("videoformat", "videoformat")
        MediaData.Add("audioformat", "audioformat")
        MediaData.Add("resolution", "resolution")
        MediaData.Add("framerate", "framerate")
        MediaData.Add("size", "size")
        MediaData.Add("source", "source")
        MediaData.Add("subtitles", "subtitles")
        MediaData.Add("languages", "languages")

        InternetData = New Hashtable
        InternetData.Add("rating", "rating")
        InternetData.Add("year", "year")
        'InternetData.Add("originaltitle", "originaltitle")
        InternetData.Add("translatedtitle", "translatedtitle")
        InternetData.Add("director", "director")
        InternetData.Add("producer", "producer")
        InternetData.Add("country", "country")
        InternetData.Add("category", "category")
        InternetData.Add("actors", "actors")
        InternetData.Add("url", "url")
        InternetData.Add("description", "description")
        InternetData.Add("comments", "comments")
        InternetData.Add("picture", "picture")
        ' Guzzi added: Extended fields
        InternetData.Add("writer", "writer")
        InternetData.Add("certification", "certification")
        InternetData.Add("languages", "languages")
        InternetData.Add("tagline", "tagline")
        InternetData.Add("imdb_id", "imdb_id")
        InternetData.Add("tmdb_id", "tmdb_id")
        InternetData.Add("imdbrank", "imdbrank")
        InternetData.Add("studio", "studio")
        InternetData.Add("fanart", "fanart") ' Guzzi: added on request Dadeo

        SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)

        LoadSettings()

        If chkExecuteProgram.Checked = False Then
            chkExecuteOnlyForOrphans.Enabled = False
            txtExecuteProgramPath.Enabled = False
        Else
            chkExecuteOnlyForOrphans.Enabled = True
            txtExecuteProgramPath.Enabled = True
        End If

        Me.ValidateChildren()

    End Sub
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        Try
            'Set my user settings to save size and position
            My.Settings.MainFormSize = Me.Size
            My.Settings.MainFormLocation = Me.Location
            My.Settings.LogFormSize = dgLogWindow.Size
            My.Settings.LogFormLocation = dgLogWindow.Location

            If dgLogWindow.Visible = True Then
                My.Settings.LogFormVisible = True 'save visible status 
            Else
                My.Settings.LogFormVisible = False
            End If

            My.Settings.Save() 'Save the user settings so next time the window will be the same size and location
            'MsgBox("Your settings were saved successfully.", _
            'MsgBoxStyle.OkOnly, "Save...")
        Catch ex As Exception
            MsgBox("There was a problem saving your settings.", _
            MsgBoxStyle.Critical, "Save Error...")
        End Try

        If dgLogWindow.Visible = True Then
            dgLogWindow.Close()
        End If
        ApplySettings()
        CurrentSettings.SaveDefaultSettings()
    End Sub

    ''Save position and size values to a system registry
    'Private Sub Form_QueryUnload(ByVal Cancel As Integer, ByVal UnloadMode As Integer)
    '    SaveSetting(App.EXEName, "Settings", "Left", Me.Left)
    '    SaveSetting(App.EXEName, "Settings", "Top", Me.Top)
    '    SaveSetting(AMCUpdater.EXEName, "Settings", "Width", Me.Width)
    '    SaveSetting(App.EXEName, "Settings", "Height", Me.Height)
    'End Sub

    ' Save the form's size and position.
    Private Sub SavePosition(ByVal frm As Form, ByVal app_name As String)
        SaveSetting(app_name, "Geometry", "WindowState", frm.WindowState)
        If frm.WindowState = FormWindowState.Normal Then
            SaveSetting(app_name, "Geometry", "Left", frm.Left)
            SaveSetting(app_name, "Geometry", "Top", frm.Top)
            SaveSetting(app_name, "Geometry", "Width", frm.Width)
            SaveSetting(app_name, "Geometry", "Height", frm.Height)
        Else
            SaveSetting(app_name, "Geometry", "Left", frm.RestoreBounds.Left)
            SaveSetting(app_name, "Geometry", "Top", frm.RestoreBounds.Top)
            SaveSetting(app_name, "Geometry", "Width", frm.RestoreBounds.Width)
            SaveSetting(app_name, "Geometry", "Height", frm.RestoreBounds.Height)
        End If
    End Sub
    ' Restore the form's size and position.
    Private Sub RestorePosition(ByVal frm As Form, ByVal app_name As String)
        frm.SetBounds( _
            GetSetting(app_name, "Geometry", "Left", Me.RestoreBounds.Left), _
            GetSetting(app_name, "Geometry", "Top", Me.RestoreBounds.Top), _
            GetSetting(app_name, "Geometry", "Width", Me.RestoreBounds.Width), _
            GetSetting(app_name, "Geometry", "Height", Me.RestoreBounds.Height) _
        )
        Me.WindowState = GetSetting(app_name, "Geometry", "WindowState", Me.WindowState)
    End Sub


#Region "Main Interactive Actions"
    Public Sub btnParseXML_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnParseXML.Click

        ApplySettings()

        If txtConfigFilePath.Text = String.Empty Then
            MsgBox("You must enter a path to your Ant database file.")
            Exit Sub
        End If


        Me.Cursor = Windows.Forms.Cursors.WaitCursor
        Me.ToolStripProgressBar.Value = Me.ToolStripProgressBar.Minimum
        Me.ToolStripProgressMessage.Text = "- idle -"

        Dim f As New IO.FileInfo(txtConfigFilePath.Text)
        If Not f.Exists Then
            Dim rep = MsgBox("File : " + f.FullName + " doesn't exist! Do you want to create it ?", MsgBoxStyle.YesNoCancel)
            If rep = 6 Then
                Dim destXml As New Xml.XmlTextWriter(txtConfigFilePath.Text, System.Text.Encoding.Default)
                destXml.WriteStartDocument(False)
                destXml.Formatting = Xml.Formatting.Indented
                destXml.WriteStartElement("AntMovieCatalog")
                destXml.WriteStartElement("Catalog")
                destXml.WriteElementString("Properties", "")
                destXml.WriteStartElement("Contents")
                destXml.WriteEndElement()
                destXml.WriteEndElement()
                destXml.WriteEndElement()
                destXml.Close()
                LogEvent("Creating XML File", EventLogLevel.ImportantEvent)
                LogEvent(" - FilePath : " + txtConfigFilePath.Text.ToString, EventLogLevel.ImportantEvent)
            Else
                MsgBox("XML File Not Found", MsgBoxStyle.Exclamation)
                SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)
                Me.Cursor = Windows.Forms.Cursors.Arrow
                Exit Sub
            End If
        End If

        AntProcessor.ProcessXML()

        SetCheckButtonStatus(ButtonStatus.ReadyToSearchFolders)

        Me.Cursor = Windows.Forms.Cursors.Arrow

    End Sub

    Public Sub btnProcessMovieList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProcessMovieList.Click

        ApplySettings()

        Dim MovieFolder As String
        Dim DoProcess As Boolean = True
        Dim ErrorText As String = ""
        If txtMovieFolder.Text = String.Empty Then
            MsgBox("You must select the folder you want to scan for movies!")
            Exit Sub
        End If

        For Each MovieFolder In txtMovieFolder.Text.Split(";")

            Dim f As New IO.DirectoryInfo(MovieFolder)

            If Not f.Exists Then
                DoProcess = False
                If ErrorText = "" Then
                    ErrorText = MovieFolder
                Else
                    ErrorText += "," & MovieFolder
                End If
            End If
        Next

        If DoProcess = True Then
            Me.ToolStripProgressBar.Value = Me.ToolStripProgressBar.Minimum
            Me.Cursor = Windows.Forms.Cursors.WaitCursor

            With AntProcessor
                .ProcessMovieFolder()
            End With

            SetCheckButtonStatus(ButtonStatus.ReadyToFindOrphans)
        Else
            MsgBox("Error : Movie Folder(s) Not Found : " & ErrorText, MsgBoxStyle.Exclamation)
        End If
        Me.Cursor = Windows.Forms.Cursors.Arrow

    End Sub

    Public Sub btnFindOrphans_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFindOrphans.Click

        Me.Cursor = Windows.Forms.Cursors.WaitCursor
        Me.ToolStripProgressBar.Value = Me.ToolStripProgressBar.Minimum

        ApplySettings()

        AntProcessor.ProcessOrphanFiles()

        If AntProcessor.CountOrphanFiles > 0 Or AntProcessor.CountOrphanRecords > 0 Then
            SetCheckButtonStatus(ButtonStatus.ReadyToDoImport)
        End If

        Me.Cursor = Windows.Forms.Cursors.Arrow

    End Sub

    Public Sub btnUpdateXML_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdateXML.Click

        Me.Cursor = Windows.Forms.Cursors.WaitCursor
        Me.ToolStripProgressBar.Value = Me.ToolStripProgressBar.Minimum

        ApplySettings()

        SetCheckButtonStatus(ButtonStatus.DisableAll)

        Try
            AntProcessor.UpdateXMLFile()
        Catch ex As Exception
            LogEvent("Error : " & ex.Message & " - Stacktrace: " & ex.StackTrace.ToString, EventLogLevel.ErrorOrSimilar)
            'Finally
            'fnSetCheckButtonStatus(ButtonStatus.ParseXML)
        End Try
        ' added to force refresh of View Collection Tab
        txtConfigFilePath_TextChanged(sender, e)
        'Me.txtConfigFilePath.Text = Me.txtConfigFilePath.Text
        Me.Cursor = Windows.Forms.Cursors.Arrow

    End Sub

    Private Sub btnJustDoIt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnJustDoIt.Click

        btnParseXML_Click(Nothing, Nothing)
        btnProcessMovieList_Click(Nothing, Nothing)
        'btnProcessTrailerList_Click(Nothing, Nothing) ' still has to be implemented - is just a remark
        btnFindOrphans_Click(Nothing, Nothing)
        If btnUpdateXML.Enabled = True Then
            btnUpdateXML_Click(Nothing, Nothing)
        End If

    End Sub


#End Region

#Region "Manual Updater"
    Private Sub btnManualDoTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualDoTest.Click

        btnManualApplyChanges.Enabled = False
        ApplySettings()

        'txtManualXMLPath - check for valid file.
        'cbManualSelectOperation - find out what they want to do - 'Update Value','Delete Record','Delete Value'
        'cbManualSelectField - field they might want to fiddle with
        'txtManualNewValue - value they may want to set.

        'chkManualParametersUpdateAll - means they want to apply to all entries (scary!)
        'cbManualParameterFieldList - otherwise use this Ant field as a parameter
        'cbManualParameterOperator - and this is the operation to perform - '=', '!=', 'EXISTS', 'NOT EXISTS', 'LIKE'
        'txtManualParameterValue - and this is the parameter to check for.
        If txtManualXMLPath.Text = String.Empty Then
            Exit Sub
        End If


        Dim f As New IO.FileInfo(txtManualXMLPath.Text)
        If Not f.Exists Then
            MsgBox("File : " + f.FullName + " doesn't exist !", MsgBoxStyle.Critical)
            txtManualXMLPath.Focus()
            Return
        End If
        If cbManualSelectOperation.SelectedIndex = -1 Then
            MsgBox("Operation Mandatory", MsgBoxStyle.Critical, "Missing Operation")
            cbManualSelectOperation.Focus()
            Return
        End If
        If cbManualSelectOperation.SelectedItem.ToString = "Update Value" Then
            If cbManualSelectField.SelectedIndex = -1 Then
                MsgBox("Field Mandatory with Update Operation", MsgBoxStyle.Critical, "Missing Update Field")
                cbManualSelectField.Focus()
                Return
            End If
            If txtManualNewValue.Text.Length = 0 Then
                MsgBox("New Value Mandatory with Update Operation", MsgBoxStyle.Critical, "Missing Update Value")
                txtManualNewValue.Focus()
                Return
            End If
        End If
        If cbManualSelectOperation.SelectedItem.ToString = "Copy Value" Then
            If cbManualSelectField.SelectedIndex = -1 Then
                MsgBox("Field Mandatory with Copy Operation", MsgBoxStyle.Critical, "Missing Source Field")
                cbManualSelectField.Focus()
                Return
            End If
            If cbManualSelectFieldDestination.SelectedIndex = -1 Then
                MsgBox("Field Mandatory with Copy Operation", MsgBoxStyle.Critical, "Missing Destination Field")
                cbManualSelectFieldDestination.Focus()
                Return
            End If
        End If
        If (chkManualParametersUpdateAll.Checked = False) Then
            If cbManualParameterFieldList1.SelectedIndex = -1 Then
                MsgBox("Field Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Field Parameter")
                cbManualParameterFieldList1.Focus()
                Return
            End If
            If cbManualParameterOperator1.SelectedIndex = -1 Then
                MsgBox("Operator Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Operator Parameter")
                cbManualParameterOperator1.Focus()
                Return
            End If
            If txtManualParameterValue1.Text.Length = 0 And (cbManualParameterOperator1.SelectedItem <> "EXISTS") And (cbManualParameterOperator1.SelectedItem <> "NOT EXISTS") Then
                MsgBox("Value Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Value Parameter")
                txtManualParameterValue1.Focus()
                Return
            End If
            If cbManualParameterAndOr.SelectedIndex <> -1 Then
                If cbManualParameterAndOr.SelectedItem.ToString <> " " Then
                    If cbManualParameterFieldList2.SelectedIndex = -1 Then
                        MsgBox("Field Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Field Parameter")
                        cbManualParameterFieldList2.Focus()
                        Return
                    End If
                    If cbManualParameterOperator2.SelectedIndex = -1 Then
                        MsgBox("Operator Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Operator Parameter")
                        cbManualParameterOperator2.Focus()
                        Return
                    End If
                    If txtManualParameterValue2.Text.Length = 0 And (cbManualParameterOperator2.SelectedItem <> "EXISTS") And (cbManualParameterOperator2.SelectedItem <> "NOT EXISTS") Then
                        MsgBox("Value Parameter Mandatory with single selection", MsgBoxStyle.Critical, "Missing Value Parameter")
                        txtManualParameterValue2.Focus()
                        Return
                    End If
                End If
            End If
        End If

        AntProcessor = New AntProcessor()

        AntProcessor.ManualOperation = cbManualSelectOperation.SelectedItem.ToString
        If cbManualSelectField.SelectedIndex > -1 Then
            AntProcessor.ManualFieldName = cbManualSelectField.SelectedValue.ToString
        End If
        If cbManualSelectFieldDestination.SelectedIndex > -1 Then
            AntProcessor.ManualFieldNameDestination = cbManualSelectFieldDestination.SelectedValue.ToString
        End If
        AntProcessor.ManualFieldValue = txtManualNewValue.Text
        AntProcessor.ManualFieldOldValue = txtManualOldValue.Text
        If cbManualParameterFieldList1.SelectedIndex > -1 Then
            AntProcessor.ManualParameterField1 = cbManualParameterFieldList1.SelectedValue.ToString
        End If
        If cbManualParameterOperator1.SelectedIndex > -1 Then
            AntProcessor.ManualParameterOperator1 = cbManualParameterOperator1.SelectedItem.ToString
        End If
        AntProcessor.ManualParameterValue1 = txtManualParameterValue1.Text
        If cbManualParameterFieldList2.SelectedIndex > -1 Then
            AntProcessor.ManualParameterField2 = cbManualParameterFieldList2.SelectedValue.ToString
        End If
        If cbManualParameterOperator2.SelectedIndex > -1 Then
            AntProcessor.ManualParameterOperator2 = cbManualParameterOperator2.SelectedItem.ToString
        End If
        AntProcessor.ManualParameterValue2 = txtManualParameterValue2.Text
        If cbManualParameterAndOr.SelectedIndex > -1 Then
            AntProcessor.cbManualParameterAndOr = cbManualParameterAndOr.SelectedItem.ToString
        End If
        AntProcessor.ManualParameterMatchAll = chkManualParametersUpdateAll.Checked
        AntProcessor.ManualMissingFanartDownload = chkManualMissingFanartDownload.Checked
        'AntProcessor.DateHandling = cbDateHandling.SelectedValue
        'If AntProcessor.ManualOperation = "Scan Media Data" Then
        'AntProcessor.ManualFieldValue = TxtManualPathToMovies.Text
        'End If
        AntProcessor.ManualXMLPath = txtManualXMLPath.Text


        AntProcessor.ManualTestOperation()

        If AntProcessor.ManualTestResultCount > 0 Then
            btnManualApplyChanges.Enabled = True
        Else
            btnManualApplyChanges.Enabled = False
        End If

    End Sub
    Private Sub btnManualApplyChanges_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualApplyChanges.Click
        ApplySettings()
        If AntProcessor.ManualTestResultCount > 0 Then
            AntProcessor.ManualRunOperation()
        End If
        'Me.txtConfigFilePath.Text = txtConfigFilePath.Text

    End Sub


#End Region

#Region "Misc GUI Actions"
    Private Sub btnSelectMovieFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectMovieFolder.Click
        Dim wpath As String
        Dim currentPath As String
        currentPath = txtMovieFolder.Text
        If currentPath.Contains(";") = True Then
            currentPath = currentPath.Substring(currentPath.IndexOf(";") + 1)
        End If

        Try
            With FolderBrowserDialog1
                .RootFolder = Environment.SpecialFolder.Desktop
                .SelectedPath = currentPath
                .Description = "Select the directory where your movie files are stored."
                .ShowNewFolderButton = False

                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    If .SelectedPath.EndsWith("\") <> True Then
                        wpath = .SelectedPath + "\"
                    Else
                        wpath = .SelectedPath
                    End If
                    'Moved this logic here since it would prompt if there already was a path listed even if the user cancels the dialog box.  OH.
                    If txtMovieFolder.Text = "" Then
                        txtMovieFolder.Text = wpath
                    Else
                        'txtMovieFolder.Text not empty ask if we want to add or replace
                        Dim rep As Int32 = MsgBox("Do you want to Add that entry to the existing one ?", MsgBoxStyle.YesNoCancel)
                        If rep = MsgBoxResult.Cancel Then
                            Exit Sub
                        Else
                            If rep = MsgBoxResult.Yes Then
                                txtMovieFolder.Text = txtMovieFolder.Text + ";" + wpath
                            Else
                                txtMovieFolder.Text = wpath
                            End If
                        End If
                    End If

                End If
            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub
    Private Sub btnSelectConfigFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectConfigFile.Click
        Dim currentDirectory As String
        currentDirectory = txtConfigFilePath.Text
        Try
            With OpenFileDialog1
                .InitialDirectory = Environment.SpecialFolder.Desktop
                .FileName = currentDirectory
                .CheckFileExists = False
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtConfigFilePath.Text = .FileName
                        txtManualXMLPath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            Console.WriteLine(ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub
    Private Sub btnGetSampleFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetSampleFile.Click
        Try
            With OpenFileDialog1
                .CheckFileExists = True
                .CheckPathExists = True
                .FileName = ""
                .DefaultExt = "avi"
                .DereferenceLinks = True
                .Filter = "All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtSampleFile.Text = .FileName
                    Catch fileException As Exception
                        Throw fileException
                    End Try
                End If

            End With
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub
    Private Sub btnTestAnalyse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTestAnalyse.Click
        If File.Exists(txtSampleFile.Text) Then
            Dim StartTime As Date = DateTime.Now

            Try
                txtSampleVideoCodec.Text = GetFileData(txtSampleFile.Text, "VideoFormat")
                txtSampleVideoBitrate.Text = GetFileData(txtSampleFile.Text, "VideoBitrate")
                txtSampleVideoFramerate.Text = GetFileData(txtSampleFile.Text, "Framerate")
                txtSampleVideoResolution.Text = GetFileData(txtSampleFile.Text, "Resolution")
                txtSampleAudioBitrate.Text = GetFileData(txtSampleFile.Text, "AudioBitrate")
                txtSampleAudioCodec.Text = GetFileData(txtSampleFile.Text, "AudioFormat")
                txtSampleFileLength.Text = GetFileData(txtSampleFile.Text, "runtime")
                txtSampleFileSize.Text = GetFileData(txtSampleFile.Text, "filesize")
                txtSampleAudioStreamCount.Text = GetFileData(txtSampleFile.Text, "audiostreamcount")
                txtSampleAudioStreamList.Text = GetFileData(txtSampleFile.Text, "audiostreamcodeclist")
                txtSampleAudioLanguageList.Text = GetFileData(txtSampleFile.Text, "audiostreamlanguagelist")
                txtSampleTextCodecList.Text = GetFileData(txtSampleFile.Text, "textstreamcodeclist")
                txtSampleTextLanguageList.Text = GetFileData(txtSampleFile.Text, "textstreamlanguagelist")

            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try

            If 1 = 0 Then
                Dim fs As FileStream = New FileStream(txtSampleFile.Text, FileMode.Open, FileAccess.Read)
                Dim length As Long = fs.Length
                'Falls over here because length is 734,208,000 bytes.
                length = 1024000
                Dim arr(length) As Byte
                fs.Read(arr, 0, length)
                fs.Close()
                Dim tmpHash() As Byte
                tmpHash = New MD5CryptoServiceProvider().ComputeHash(arr)
                'txtTestScanChecksumBox.Text = ByteArrayToString(tmpHash)
            End If

            Dim EndTime As Date = DateTime.Now
            MsgBox("Operation took : " & (EndTime - StartTime).ToString)

        End If
    End Sub
    Private Sub btnManualSelectXMLFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualSelectXMLFile.Click
        Try
            With OpenFileDialog1
                .InitialDirectory = Environment.SpecialFolder.Desktop
                .FileName = ""
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtManualXMLPath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            Console.WriteLine(ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub
    Private Sub btnSelectParserFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectParserFile.Click
        Dim currentPath As String
        currentPath = txtParserFilePath.Text
        If currentPath.Contains(";") = True Then
            currentPath.Substring(currentPath.IndexOf(";") + 1)
        End If
        Try
            With OpenFileDialog1
                .InitialDirectory = My.Application.Info.DirectoryPath
                .FileName = currentPath
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtParserFilePath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub
    Private Sub btnSelectExcludeFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectExcludeFile.Click
        Dim currentPath As String
        currentPath = txtExcludeFilePath.Text
        If currentPath.Contains(";") = True Then
            currentPath.Substring(currentPath.IndexOf(";") + 1)
        End If
        Try
            With OpenFileDialog1
                .InitialDirectory = My.Application.Info.DirectoryPath
                .FileName = "AMCUpdater_Excluded_Files.txt"
                .CheckFileExists = False
                .CheckPathExists = True
                .DefaultExt = "txt"
                .DereferenceLinks = True
                .Filter = "txt files (*.txt)|*.txt|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file for Excluded Movies Files"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtExcludeFilePath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub
    Private Sub btnExecuteProgramSelectPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExecuteProgramSelectPath.Click
        Try
            With OpenFileDialog1
                .InitialDirectory = Environment.SpecialFolder.Desktop
                .FileName = ""
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "All files|*.*"
                .Multiselect = False
                .RestoreDirectory = False
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtExecuteProgramPath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If
            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            Console.WriteLine(ex.Message)
            '            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub

    Private Sub btnCancelProcessing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancelProcessing.Click
        AntProcessor.bgwFolderScanUpdate_Cancel()
    End Sub

    Private Sub btnManualCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualCancel.Click
        AntProcessor.bgwManualUpdate_Cancel()
    End Sub


    Private Sub btnShowHideLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnShowHideLogTest.Click, btnShowHideLog.Click
        If dgLogWindow.Visible = True Then
            dgLogWindow.Hide()
            btnShowHideLog.Text = "Show Log >>"
            btnShowHideLogTest.Text = "Show Log >>"
        Else
            'dgLogWindow.SetDesktopLocation(Me.Location.X, Me.Location.Y + Me.Height)
            'dgLogWindow.Width = Me.Width
            ''dgLogWindow.Height = 150
            dgLogWindow.Show()

            'dgLogWindow.Height = Me.Height
            'dgLogWindow.SetDesktopLocation(Me.Location.X + Me.Width, Me.Location.Y)
            'dgLogWindow.Show()
            ''Me.AddOwnedForm(dgLogWindow)

            btnShowHideLog.Text = "<< Hide Log"
            btnShowHideLogTest.Text = "<< Hide Log"
        End If
    End Sub

    Private Sub Form1_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
        If dgLogWindow.Visible = True And dgLogWindow.Location.X = Me.Location.X Then

            dgLogWindow.SetDesktopLocation(Me.Location.X, Me.Location.Y + Me.Height)
            'dgLogWindow.SetDesktopLocation(Me.Location.X + Me.Width, Me.Location.Y)

            'dgLogWindow.BringToFront()
            'Me.BringToFront()
        End If
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
    End Sub
    Private Sub btnDBFieldsSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectAll.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If Not cbDatabaseFields.Items(i).ToString = txtDefaultSourceField.Text Then
                cbDatabaseFields.SetItemChecked(i, True)
            End If
        Next
        Database_Fields_Validation()
    End Sub
    Private Sub btnDBFieldsSelectNone_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectNone.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            cbDatabaseFields.SetItemChecked(i, False)
        Next
        Database_Fields_Validation()
    End Sub
    Private Sub btnDBFieldsSelectAllMedia_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectAllMedia.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If MediaData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                If Not cbDatabaseFields.Items(i).ToString = txtDefaultSourceField.Text Then
                    cbDatabaseFields.SetItemChecked(i, True)
                End If
            End If
        Next
        Database_Fields_Validation()
    End Sub
    Private Sub btnDBFieldsSelectNoMedia_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectNoMedia.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If MediaData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                cbDatabaseFields.SetItemChecked(i, False)
            End If
        Next
        Database_Fields_Validation()
    End Sub
    Private Sub btnDBFieldsSelectAllInternet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectAllInternet.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If InternetData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                If Not cbDatabaseFields.Items(i).ToString = txtDefaultSourceField.Text Then
                    cbDatabaseFields.SetItemChecked(i, True)
                End If
            End If
        Next
        Database_Fields_Validation()
    End Sub
    Private Sub btnDBFieldsSelectNoInternet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectNoInternet.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If InternetData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                cbDatabaseFields.SetItemChecked(i, False)
            End If
        Next
        Database_Fields_Validation()
    End Sub

    Private Sub btnManualSelectInternetParserPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualSelectInternetParserPath.Click
        Dim currentPath As String
        currentPath = txtManualInternetParserPath.Text
        If currentPath.Contains(";") = True Then
            currentPath.Substring(currentPath.IndexOf(";") + 1)
        End If
        Try
            With OpenFileDialog1
                .InitialDirectory = My.Application.Info.DirectoryPath
                .FileName = currentPath
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtManualInternetParserPath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub

    Private Sub btnManualSelectExcludedMoviesFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualSelectExcludedMoviesFile.Click
        Try
            With OpenFileDialog1
                .InitialDirectory = My.Application.Info.DirectoryPath
                .FileName = "AMCUpdater_Excluded_Files.txt"
                .CheckFileExists = False
                .CheckPathExists = True
                .DefaultExt = "txt"
                .DereferenceLinks = True
                .Filter = "txt files (*.txt)|*.txt|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file for Excluded Movies Files"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtManualExcludedMoviesPath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub

#End Region

#Region "Validation"
    Private Sub chkOverwriteXML_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkOverwriteXML.CheckedChanged

        'AntProcessor.OverwriteXMLFile = chkOverwriteXML.Checked

        If chkOverwriteXML.Checked = True Then
            chkBackupXMLFirst.Enabled = True
        Else
            chkBackupXMLFirst.Checked = False
            chkBackupXMLFirst.Enabled = False
        End If
    End Sub
    Private Sub txtOverridePath_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtOverridePath.LostFocus
        If txtOverridePath.Text <> String.Empty And txtOverridePath.Text.EndsWith("\") = False Then
            txtOverridePath.Text += "\"
        End If
    End Sub
    Private Sub chkPurgeMissing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPurgeMissing.Click

        If chkPurgeMissing.Checked = True Then
            Dim Warning As String = "Warning : The 'Purge missing' option removes invalid entries from your database file."
            Warning += vbCrLf + "It is therefore potentially dangerous - please proceed with caution."
            Warning += vbCrLf + vbCrLf + "Do you wish to continue?"

            Dim ReturnValue As Integer

            ReturnValue = MsgBox(Warning, MsgBoxStyle.OkCancel, "Warning")

            If Not ReturnValue = 1 Then
                chkPurgeMissing.Checked = False
            End If
        End If
    End Sub
    Private Sub cbManualSelectOperation_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualSelectOperation.SelectedIndexChanged
        If cbManualSelectOperation.SelectedIndex > -1 Then
            lblManualEnterNewValue.Visible = False
            txtManualNewValue.Visible = False
            lblManualEnterOldValue.Visible = False
            txtManualOldValue.Visible = False
            lblManualSelectField.Visible = False
            cbManualSelectField.Visible = False
            lblManualSelectFieldDestination.Visible = False
            cbManualSelectFieldDestination.Visible = False

            grpManualInternetLookupSettings.Visible = False
            lblManualDatabaseFieldsPrompt.Visible = False
            chkManualMissingFanartDownload.Visible = False

            chkManualUpdateRecordsOnlyMissingData.Visible = False
            chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Visible = False

            grpManualUpdatesParameters.Visible = True

            If cbManualSelectOperation.SelectedItem = "Update Value" Then
                lblManualEnterNewValue.Visible = True
                txtManualNewValue.Visible = True
                txtManualNewValue.Clear()
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                cbManualSelectField.SelectedIndex = -1
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Replace String" Then
                lblManualEnterOldValue.Visible = True
                txtManualOldValue.Visible = True
                txtManualOldValue.Clear()
                lblManualEnterNewValue.Visible = True
                txtManualNewValue.Visible = True
                txtManualNewValue.Clear()
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                cbManualSelectField.SelectedIndex = -1
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Add String" Then
                lblManualEnterNewValue.Visible = True
                txtManualNewValue.Visible = True
                txtManualNewValue.Clear()
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                cbManualSelectField.SelectedIndex = -1
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Insert String" Then
                lblManualEnterNewValue.Visible = True
                txtManualNewValue.Visible = True
                txtManualNewValue.Clear()
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                cbManualSelectField.SelectedIndex = -1
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Remove String" Then
                lblManualEnterNewValue.Visible = True
                txtManualNewValue.Visible = True
                txtManualNewValue.Clear()
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                cbManualSelectField.SelectedIndex = -1
            ElseIf cbManualSelectOperation.SelectedItem = "Delete Value" Then
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
            ElseIf cbManualSelectOperation.SelectedItem = "Copy Value" Then
                lblManualSelectField.Visible = True
                cbManualSelectField.Visible = True
                lblManualSelectFieldDestination.Visible = True
                cbManualSelectFieldDestination.Visible = True
                chkManualUpdateRecordsOnlyMissingData.Visible = True
                chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Visible = True
            ElseIf cbManualSelectOperation.SelectedItem = "Update Record" Then
                grpManualInternetLookupSettings.Visible = True
                lblManualDatabaseFieldsPrompt.Visible = True
                chkManualUpdateRecordsOnlyMissingData.Visible = True
                chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Visible = True
            ElseIf cbManualSelectOperation.SelectedItem = "Download Fanart" Then
                chkManualMissingFanartDownload.Visible = True
                'grpManualInternetLookupSettings.Visible = True
                lblManualDatabaseFieldsPrompt.Visible = True
            End If
        End If
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualOperator_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualParameterOperator1.SelectedIndexChanged
        If cbManualParameterOperator1.SelectedIndex >= 0 Then
            If cbManualParameterOperator1.SelectedItem <> "EXISTS" And cbManualParameterOperator1.SelectedItem <> "NOT EXISTS" Then
                txtManualParameterValue1.Visible = True
                lblManualParametersValue1.Visible = True
            Else
                txtManualParameterValue1.Visible = False
                lblManualParametersValue1.Visible = False
            End If
        End If
        Me.ValidateChildren()
    End Sub
    Private Sub chkManualParametersUpdateAll_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkManualParametersUpdateAll.CheckedChanged
        If chkManualParametersUpdateAll.Checked = True Then
            cbManualParameterFieldList1.Enabled = False
            cbManualParameterOperator1.Enabled = False
            txtManualParameterValue1.Enabled = False
            cbManualParameterFieldList2.Enabled = False
            cbManualParameterOperator2.Enabled = False
            txtManualParameterValue2.Enabled = False
            cbManualParameterAndOr.Enabled = False
        Else
            cbManualParameterFieldList1.Enabled = True
            cbManualParameterOperator1.Enabled = True
            txtManualParameterValue1.Enabled = True
            cbManualParameterFieldList2.Enabled = True
            cbManualParameterOperator2.Enabled = True
            txtManualParameterValue2.Enabled = True
            cbManualParameterAndOr.Enabled = True
        End If
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualParameterFieldList2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualParameterFieldList2.SelectedIndexChanged
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualParameterOperator2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualParameterOperator2.SelectedIndexChanged
        If cbManualParameterOperator2.SelectedIndex >= 0 Then
            If cbManualParameterOperator2.SelectedItem <> "EXISTS" And cbManualParameterOperator2.SelectedItem <> "NOT EXISTS" Then
                txtManualParameterValue2.Visible = True
                lblManualParametersValue2.Visible = True
            Else
                txtManualParameterValue2.Visible = False
                lblManualParametersValue2.Visible = False
            End If
        End If
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualParameterAndOr_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualParameterAndOr.SelectedIndexChanged
        If cbManualParameterAndOr.SelectedIndex >= 0 Then
            If cbManualParameterAndOr.SelectedItem.ToString = " " Then
                cbManualParameterAndOr.SelectedIndex = -1
                Me.ValidateChildren()
                Exit Sub
            Else
                lblManualParametersField2.Visible = True
                lblManualParametersOperator2.Visible = True
                cbManualParameterFieldList2.Visible = True
                cbManualParameterOperator2.Visible = True
            End If
        Else
            lblManualParametersField2.Visible = False
            lblManualParametersOperator2.Visible = False
            lblManualParametersValue2.Visible = False
            cbManualParameterFieldList2.Visible = False
            cbManualParameterOperator2.Visible = False
            txtManualParameterValue2.Visible = False
            cbManualParameterFieldList2.Text = ""
            cbManualParameterOperator2.Text = ""
            txtManualParameterValue2.Text = ""
        End If
        Me.ValidateChildren()

    End Sub
    Private Sub txtManualParameterValue2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtManualParameterValue2.TextChanged
        Me.ValidateChildren()
    End Sub
    Private Sub txtManualParameterValue1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtManualParameterValue1.TextChanged
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualParameterFieldList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualParameterFieldList1.SelectedIndexChanged
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualSelectField_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualSelectField.SelectedIndexChanged
        Me.ValidateChildren()
    End Sub
    Private Sub cbManualSelectFieldDestination_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbManualSelectFieldDestination.SelectedIndexChanged
        Me.ValidateChildren()
    End Sub
    Private Sub txtMovieFolder_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtMovieFolder.LostFocus
        'txtOverridePath_LostFocus(sender, e)
        Dim Path As String() = txtMovieFolder.Text.Split(";")
        Dim PathItem As String = String.Empty
        Dim ReturnPath As String = String.Empty
        For i As Integer = 0 To Path.Length - 1
            PathItem = Path(i)
            If Not ReturnPath.Contains(PathItem) Then
                If Not PathItem.EndsWith("\") = True Then
                    PathItem += "\"
                End If
                ReturnPath += PathItem & ";"
            End If
        Next
        If ReturnPath.EndsWith(";") = True Then
            ReturnPath = ReturnPath.Substring(0, ReturnPath.Length - 1)
        End If

        txtMovieFolder.Text = ReturnPath

    End Sub
    Private Sub chkExecuteProgram_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkExecuteProgram.CheckedChanged
        If chkExecuteProgram.Checked Then
            txtExecuteProgramPath.Enabled = True
            btnExecuteProgramSelectPath.Enabled = True
            chkExecuteOnlyForOrphans.Enabled = True
        Else
            txtExecuteProgramPath.Enabled = False
            btnExecuteProgramSelectPath.Enabled = False
            chkExecuteOnlyForOrphans.Enabled = False
        End If
        Me.ValidateChildren()
    End Sub
    Private Sub txtDefaultFileTypes_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDefaultFileTypes.LostFocus
        Dim FileTypes() As String
        Dim OutputString As String = String.Empty
        If txtDefaultFileTypes.Text <> String.Empty Then
            FileTypes = txtDefaultFileTypes.Text.Split(";")
            For Each blah As String In FileTypes
                If blah.ToLower.Trim = "ifo" Then
                    MsgBox("You cannot scan for filetype 'ifo' as this is used for DVD image detection.")
                ElseIf blah.ToLower.Trim = "bdmv" Then
                    MsgBox("You cannot scan for filetype 'bdmv' as this is used for BR image detection.")
                Else
                    OutputString += blah & ";"
                End If
            Next
            If OutputString.EndsWith(";") Then
                OutputString = OutputString.Substring(0, OutputString.Length - 1)
            End If

            txtDefaultFileTypes.Text = OutputString

        End If
        'Me.ValidateChildren()
    End Sub
    Private Sub txtDefaultFileTypesNonMedia_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDefaultFileTypesNonMedia.LostFocus
        Dim FileTypes() As String
        Dim OutputString As String = String.Empty
        If txtDefaultFileTypesNonMedia.Text <> String.Empty Then
            FileTypes = txtDefaultFileTypesNonMedia.Text.Split(";")
            For Each blah As String In FileTypes
                If blah.ToLower.Trim = "ifo" Then
                    MsgBox("You cannot scan for filetype 'ifo' as this is used for DVD image detection.")
                ElseIf blah.ToLower.Trim = "bdmv" Then
                    MsgBox("You cannot scan for filetype 'bdmv' as this is used for BR image detection.")
                Else
                    OutputString += blah & ";"
                End If
            Next
            If OutputString.EndsWith(";") Then
                OutputString = OutputString.Substring(0, OutputString.Length - 1)
            End If

            txtDefaultFileTypesNonMedia.Text = OutputString

        End If
    End Sub
    Private Sub txtManualInternetParserPath_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtManualInternetParserPath.LostFocus
        Me.ValidateChildren()
    End Sub
    Private Sub txtManualExcludedMoviesPath_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtManualExcludedMoviesPath.LostFocus
        Me.ValidateChildren()
    End Sub
    Private Sub cbDatabaseFields_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles cbDatabaseFields.ItemCheck
        Database_Fields_Validation()
    End Sub
    Private Sub cbDatabaseFields_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbDatabaseFields.LostFocus
        Database_Fields_Validation()
    End Sub
    Private Sub cbTitleHandling_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbTitleHandling.SelectedIndexChanged
        Database_Fields_Validation()
    End Sub
    Private Sub cbDatabaseFields_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cbDatabaseFields.MouseUp
        Database_Fields_Validation()
    End Sub
    Private Sub chkFolderNameIsGroupName_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkFolderNameIsGroupName.CheckedChanged
        Database_Fields_Validation()
    End Sub
    Private Sub txtPictureFilenamePrefix_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPictureFilenamePrefix.TextChanged
        Me.ValidateChildren()
    End Sub
    Private Sub cbPictureHandling_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbPictureHandling.SelectedIndexChanged
        Database_Fields_Validation()
    End Sub

    Private Sub Interactive_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtOverridePath.Validating, txtMovieFolder.Validating, txtConfigFilePath.Validating, txtParserFilePath.Validating, txtExcludeFilePath.Validating
        'Override Path and Movie Folder (Check for empty; check that no override path used with multiple scan folders
        Dim IsValid As Boolean = True


        If txtOverridePath.Text <> String.Empty And txtMovieFolder.Text.Contains(";") Then
            epInteractive.SetError(txtOverridePath, "Cannot have an Override Path when multiple folders are scanned")
            epInteractive.SetError(txtMovieFolder, "Cannot have an Override Path when multiple folders are scanned")
            IsValid = False
        Else
            If txtMovieFolder.Text = String.Empty Then
                epInteractive.SetError(txtMovieFolder, "Path to folder containing movie files must be entered.")
                IsValid = False
            Else
                epInteractive.SetError(txtMovieFolder, "")
            End If
            epInteractive.SetError(txtOverridePath, "")
        End If
        'XML file path required.
        If txtConfigFilePath.Text = String.Empty Then
            epInteractive.SetError(txtConfigFilePath, "Path to Ant XML database file must be entered")
            IsValid = False
        Else
            epInteractive.SetError(txtConfigFilePath, "")
        End If
        'Parser File Path Required
        If txtParserFilePath.Text = String.Empty Then
            epInteractive.SetError(txtParserFilePath, "Path to Internet Parser File must be entered")
            IsValid = False
        Else
            Dim wpath As String = txtParserFilePath.Text
            If Not wpath.Contains("\") Then
                'Not a path without a backslash!
                epInteractive.SetError(txtParserFilePath, "Please enter a valid Internet Parser File path.")
                IsValid = False
            Else
                If System.IO.File.Exists(wpath) Then
                    epInteractive.SetError(txtParserFilePath, "")
                Else
                    epInteractive.SetError(txtParserFilePath, "Please enter a valid Internet Parser File path.")
                    IsValid = False
                End If
            End If
        End If

        'Excluded Movie File Path Required
        If txtExcludeFilePath.Text = String.Empty Then
            epInteractive.SetError(txtExcludeFilePath, "Path to excluded movies file must be entered")
            IsValid = False
        Else
            'Check it's a valid path:
            Dim wpath As String = txtExcludeFilePath.Text
            If Not wpath.Contains("\") Then
                'Not a path without a backslash!
                epInteractive.SetError(txtExcludeFilePath, "Please enter a valid file path")
                IsValid = False
            Else
                wpath = wpath.Substring(0, wpath.LastIndexOf("\") + 1)
                If System.IO.Directory.Exists(wpath) Then
                    If txtExcludeFilePath.Text.EndsWith("\") = True Then
                        txtExcludeFilePath.Text += "AMCUpdater_Excluded_Files.txt"
                    ElseIf txtExcludeFilePath.Text.EndsWith(".txt") = True Then
                    Else
                        txtExcludeFilePath.Text += "\AMCUpdater_Excluded_Files.txt"
                    End If
                    epInteractive.SetError(txtExcludeFilePath, "")
                Else
                    epInteractive.SetError(txtExcludeFilePath, "Please enter a valid file path")
                    IsValid = False
                End If
            End If
        End If

        If IsValid = False Then
            btnJustDoIt.Enabled = False
            SetCheckButtonStatus(ButtonStatus.DisableAll)
        Else
            btnJustDoIt.Enabled = True
            SetCheckButtonStatus(ButtonStatus.ReadyToParseXML)
        End If

    End Sub
    Private Sub ManualUpdater_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtManualXMLPath.Validating, cbManualParameterOperator1.Validating, cbManualParameterFieldList1.Validating, txtManualNewValue.Validating, txtManualParameterValue1.Validating, cbManualSelectField.Validating, cbManualSelectOperation.Validating, chkManualParametersUpdateAll.Validating
        Dim IsValid As Boolean = True
        Dim TestState As Boolean = btnManualDoTest.Enabled
        Dim ApplyState As Boolean = btnManualApplyChanges.Enabled

        'Check a path has been entered:
        If txtManualXMLPath.Text = String.Empty Then
            epManualUpdater.SetError(txtManualXMLPath, "Path to Ant XML database file must be entered")
            IsValid = False
        Else
            epManualUpdater.SetError(txtManualXMLPath, "")
            If Not File.Exists(txtManualXMLPath.Text) Then
                epManualUpdater.SetError(txtManualXMLPath, "Path to Ant XML database file is not valid")
            End If
        End If

        'Check an operation is selected:
        If cbManualSelectOperation.SelectedIndex < 0 Then
            epManualUpdater.SetError(cbManualSelectOperation, "You must select an operation.")
            IsValid = False
            grpManualInternetLookupSettings.Visible = False
        Else
            epManualUpdater.SetError(cbManualSelectOperation, "")
            'If so, check the cbManualSelectField and txtManualNewValue fields:
            If cbManualSelectOperation.SelectedItem = "Update Value" Then
                'Update value : requires cbManualSelectField and txtManualNewValue
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to update")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If txtManualNewValue.Text = String.Empty Then
                    epManualUpdater.SetError(txtManualNewValue, "Please enter the new value")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualNewValue, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Replace String" Then
                'Update value replace : requires cbManualSelectField and txtManualOldValue and txtManualNewValue
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to update")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If txtManualOldValue.Text = String.Empty Then
                    epManualUpdater.SetError(txtManualOldValue, "Please enter the old value to be replaced")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualOldValue, "")
                End If
                If txtManualNewValue.Text = txtManualOldValue.Text Then
                    epManualUpdater.SetError(txtManualNewValue, "Please enter the new value to replace DIFFERENT than old one")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualNewValue, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Add String" Then
                'Update value : requires cbManualSelectField and txtManualNewValue
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to update")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If txtManualNewValue.Text = String.Empty Then
                    epManualUpdater.SetError(txtManualNewValue, "Please enter the new value to add")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualNewValue, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Insert String" Then
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to update")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If txtManualNewValue.Text = String.Empty Then
                    epManualUpdater.SetError(txtManualNewValue, "Please enter the new value to insert")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualNewValue, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Update Value - Remove String" Then
                'Update value : requires cbManualSelectField and txtManualNewValue
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to update")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If txtManualNewValue.Text = String.Empty Then
                    epManualUpdater.SetError(txtManualNewValue, "Please enter the new value to remove")
                    IsValid = False
                Else
                    epManualUpdater.SetError(txtManualNewValue, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Delete Value" Then
                'Delete Value : requires cbManualSelectField
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field to delete")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Copy Value" Then
                'Copy Value : requires cbManualSelectField and cbManualSelectFieldDestination
                If cbManualSelectField.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectField, "Please select a field as source to copy from")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectField, "")
                End If
                If cbManualSelectFieldDestination.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualSelectFieldDestination, "Please select a field as destination to copy to")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualSelectFieldDestination, "")
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Download Fanart" Then
                'Delete Value : requires cbManualSelectField
                If txtFanartFolder.Text = String.Empty Then
                    epManualUpdater.SetError(cbManualSelectOperation, "Please select a Path to Fanart download")
                    IsValid = False
                End If
            ElseIf cbManualSelectOperation.SelectedItem = "Update Record" Then
                If IsInternetLookupNeeded() = True Then
                    grpManualInternetLookupSettings.Visible = True
                    'Excluded Movie File Path Required:
                    If txtManualExcludedMoviesPath.Text = String.Empty Then
                        epManualUpdater.SetError(txtManualExcludedMoviesPath, "Path to excluded movies file must be entered")
                        IsValid = False
                    Else
                        'Check it's a valid path:
                        Dim wpath As String = txtManualExcludedMoviesPath.Text
                        If Not wpath.Contains("\") Then
                            'Not a path without a backslash!
                            epManualUpdater.SetError(txtManualExcludedMoviesPath, "Please enter a valid file path")
                            IsValid = False
                        Else
                            wpath = wpath.Substring(0, wpath.LastIndexOf("\"))
                            If Directory.Exists(wpath) Then
                                If txtManualExcludedMoviesPath.Text.EndsWith("\") = True Then
                                    txtManualExcludedMoviesPath.Text += "AMCUpdater_Excluded_Files.txt"
                                ElseIf txtManualExcludedMoviesPath.Text.EndsWith(".txt") = True Then
                                    'Else
                                    'txtManualExcludedMoviesPath.Text += "\AMCUpdater_Excluded_Files.txt"
                                End If
                                epInteractive.SetError(txtManualExcludedMoviesPath, "")
                            Else
                                epInteractive.SetError(txtManualExcludedMoviesPath, "Please enter a valid file path")
                                IsValid = False
                            End If
                        End If
                    End If

                    'Internet parser file required:
                    If txtManualInternetParserPath.Text = String.Empty Then
                        epManualUpdater.SetError(txtManualInternetParserPath, "Path to Internet Grabber Script must be entered.")
                        epManualUpdater.SetError(txtManualInternetParserPathDisplay, "Path to Internet Grabber Script must be entered.")
                        IsValid = False
                    Else
                        Dim wpath As String = txtManualInternetParserPath.Text
                        If Not wpath.Contains("\") Then
                            'Not a path without a backslash!
                            epManualUpdater.SetError(txtManualInternetParserPath, "Please enter a valid Internet Grabber Script path.")
                            epManualUpdater.SetError(txtManualInternetParserPathDisplay, "Please enter a valid Internet Grabber Script path.")
                            IsValid = False
                        Else
                            If File.Exists(wpath) Then
                                epManualUpdater.SetError(txtManualInternetParserPath, "")
                                epManualUpdater.SetError(txtManualInternetParserPathDisplay, "")
                            Else
                                epManualUpdater.SetError(txtManualInternetParserPath, "Please enter a valid Internet Grabber Script path.")
                                epManualUpdater.SetError(txtManualInternetParserPathDisplay, "Please enter a valid Internet Grabber Script path.")
                                IsValid = False
                            End If
                        End If
                    End If
                End If
            End If
        End If

        If chkManualParametersUpdateAll.Checked Then
            epManualUpdater.SetError(cbManualParameterFieldList1, "")
            epManualUpdater.SetError(cbManualParameterOperator1, "")
            epManualUpdater.SetError(txtManualParameterValue1, "")
            epManualUpdater.SetError(cbManualParameterFieldList2, "")
            epManualUpdater.SetError(cbManualParameterOperator2, "")
            epManualUpdater.SetError(txtManualParameterValue2, "")
        Else
            If cbManualParameterFieldList1.SelectedIndex < 0 Then
                epManualUpdater.SetError(cbManualParameterFieldList1, "Please select a field")
                IsValid = False
            Else
                epManualUpdater.SetError(cbManualParameterFieldList1, "")
            End If
            If cbManualParameterOperator1.SelectedIndex < 0 Then
                epManualUpdater.SetError(cbManualParameterOperator1, "Please select a comparison operator")
                IsValid = False
            Else
                epManualUpdater.SetError(cbManualParameterOperator1, "")
                If cbManualParameterOperator1.SelectedItem <> "EXISTS" And cbManualParameterOperator1.SelectedItem <> "NOT EXISTS" Then
                    If txtManualParameterValue1.Text = String.Empty Then
                        epManualUpdater.SetError(txtManualParameterValue1, "Please enter a value")
                        IsValid = False
                    Else
                        epManualUpdater.SetError(txtManualParameterValue1, "")
                    End If
                Else
                    epManualUpdater.SetError(txtManualParameterValue1, "")
                End If
            End If
            If cbManualParameterAndOr.SelectedIndex >= 0 Then
                If cbManualParameterFieldList2.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualParameterFieldList2, "Please select a field")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualParameterFieldList2, "")
                End If
                If cbManualParameterOperator2.SelectedIndex < 0 Then
                    epManualUpdater.SetError(cbManualParameterOperator2, "Please select a comparison operator")
                    IsValid = False
                Else
                    epManualUpdater.SetError(cbManualParameterOperator2, "")
                    If cbManualParameterOperator2.SelectedItem <> "EXISTS" And cbManualParameterOperator2.SelectedItem <> "NOT EXISTS" Then
                        If txtManualParameterValue2.Text = String.Empty Then
                            epManualUpdater.SetError(txtManualParameterValue2, "Please enter a value")
                            IsValid = False
                        Else
                            epManualUpdater.SetError(txtManualParameterValue2, "")
                        End If
                    Else
                        epManualUpdater.SetError(txtManualParameterValue2, "")
                    End If
                End If
            End If

        End If

        If IsValid = True Then
            'btnManualApplyChanges.Enabled = True
            btnManualDoTest.Enabled = True
            btnManualCancel.Enabled = False
        Else
            btnManualApplyChanges.Enabled = False
            btnManualDoTest.Enabled = False
        End If

    End Sub
    Private Sub Options_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtDefaultSourceField.Validating, chkExecuteProgram.Validating, txtExecuteProgramPath.Validating
        'txtDefaultFileType
        'txtDefaultSourceField
        'chkExecuteProgram
        'txtExecuteProgramPath
        'Dim IsValid As Boolean = True

        ValidOptions = True

        If txtDefaultFileTypes.Text.Length < 1 Then
            epOptions.SetError(txtDefaultFileTypes, "Some file types must be entered.")
            ValidOptions = False
        ElseIf txtDefaultFileTypes.Text.ToLower.Contains("ifo") Then
            epOptions.SetError(txtDefaultFileTypes, "You cannot scan for filetype 'ifo' as this is used for DVD image detection.")
            ValidOptions = False
        ElseIf txtDefaultFileTypes.Text.ToLower.Contains("bdmv") Then
            epOptions.SetError(txtDefaultFileTypes, "You cannot scan for filetype 'bdmv' as this is used for BR image detection.")
            ValidOptions = False
        Else
            epOptions.SetError(txtDefaultFileTypes, "")
        End If

        If txtDefaultFileTypesNonMedia.Text.ToLower.Contains("ifo") Then
            epOptions.SetError(txtDefaultFileTypesNonMedia, "You cannot scan for filetype 'ifo' as this is used for DVD image detection.")
            ValidOptions = False
        ElseIf txtDefaultFileTypesNonMedia.Text.ToLower.Contains("bdmv") Then
            epOptions.SetError(txtDefaultFileTypesNonMedia, "You cannot scan for filetype 'bdmv' as this is used for BR image detection.")
            ValidOptions = False
        Else
            epOptions.SetError(txtDefaultFileTypesNonMedia, "")
        End If

        Dim SourceChecked As String
        Dim SourceIndex As Integer = cbDatabaseFields.Items.IndexOf(txtDefaultSourceField.Text)
        If SourceIndex > -1 Then
            SourceChecked = cbDatabaseFields.GetItemChecked(SourceIndex).ToString
        Else
            SourceChecked = ""
        End If

        If txtDefaultSourceField.Text.Length < 1 Then
            epOptions.SetError(txtDefaultSourceField, "Source Field is mandatory. The default is 'Source'.")
            ValidOptions = False
        ElseIf SourceChecked = "True" Then
            epOptions.SetError(txtDefaultSourceField, "Make sure the media Source Field is not selected to be updated in database fields.")
            ValidOptions = False
        Else
            epOptions.SetError(txtDefaultSourceField, "")
        End If

        Dim DuplicateMediaTypes As Boolean = False
        Dim MediaTypes() As String
        Dim NonMediaTypes() As String
        MediaTypes = txtDefaultFileTypes.Text.Split(";")
        NonMediaTypes = txtDefaultFileTypesNonMedia.Text.Split(";")
        If MediaTypes.Length > 0 And NonMediaTypes.Length > 0 Then
            For i As Integer = 0 To MediaTypes.Length - 1
                For ii As Integer = 0 To NonMediaTypes.Length - 1
                    If MediaTypes(i).ToLower = NonMediaTypes(ii).ToLower Then
                        DuplicateMediaTypes = True
                    End If
                Next
            Next
        End If

        If DuplicateMediaTypes = True Then
            epOptions.SetError(txtDefaultFileTypes, "Duplicate entries detected.")
            epOptions.SetError(txtDefaultFileTypesNonMedia, "Duplicate entries detected.")
        Else
            epOptions.SetError(txtDefaultFileTypes, "")
            epOptions.SetError(txtDefaultFileTypesNonMedia, "")
        End If

        'Dim DBFields() As String
        'DBFields = CurrentSettings.Database_Fields_To_Import.Split(";")
        'Dim FieldName As String
        'Dim FieldSelected As Boolean
        'cbDatabaseFields.Items.Clear()
        'For i As Integer = 0 To DBFields.Length - 1
        '    If DBFields(i).Length > 0 Then
        '        FieldName = DBFields(i).Substring(0, DBFields(i).IndexOf("|"))
        '        FieldSelected = DBFields(i).Substring(DBFields(i).IndexOf("|") + 1)
        '        cbDatabaseFields.Items.Add(FieldName, FieldSelected)
        '    End If
        'Next

        If chkExecuteProgram.Checked = True Then
            If txtExecuteProgramPath.Text.Length < 1 Then
                epOptions.SetError(txtExecuteProgramPath, "Program path must be entered if 'execute program' is enabled.")
                ValidOptions = False
            Else
                epOptions.SetError(txtExecuteProgramPath, "")
            End If
        Else
            epOptions.SetError(txtExecuteProgramPath, "")
        End If

        Dim ErrorText As String = String.Empty
        ErrorText = "Prefix cannot begin with a '\' character." & vbCrLf
        ErrorText += "Entry must be either a string to append, or a relative path, or both." & vbCrLf
        ErrorText += "Valid entries include:" & vbCrLf
        ErrorText += "'prefix' to add a prefix to each filename." & vbCrLf
        ErrorText += "'Folder\' to store images in a subfolder.  Only one subfolder is permitted" & vbCrLf
        ErrorText += "'Folder\prefix' to do both." & vbCrLf

        Dim PathCounter As Integer = 0
        For i As Integer = 0 To txtPictureFilenamePrefix.Text.Length - 1
            If txtPictureFilenamePrefix.Text.Chars(i) = "\" Then
                PathCounter += 1
            End If
        Next

        If txtPictureFilenamePrefix.Text.StartsWith("\") = True Or PathCounter > 1 Then
            epOptions.SetError(txtPictureFilenamePrefix, ErrorText)
        Else
            epOptions.SetError(txtPictureFilenamePrefix, "")
            ValidOptions = False
        End If

    End Sub
    Private Sub Database_Fields_Validation()
        Dim item As Object
        Dim FieldName As String
        Dim FieldChecked As String
        Dim InternetLookupNeeded As Boolean = False

        For Each item In cbDatabaseFields.Items
            FieldName = item.ToString
            FieldChecked = cbDatabaseFields.GetItemChecked(cbDatabaseFields.Items.IndexOf(FieldName)).ToString

            If FieldChecked = "True" Then
                If InternetData.ContainsValue(FieldName.ToLower) Then
                    InternetLookupNeeded = True
                End If
            End If


            If FieldName = txtDefaultSourceField.Text Then
                If FieldChecked = "True" Then
                    epOptions.SetError(txtDefaultSourceField, "Make sure the media Source Field is not selected to be updated in database fields.")
                    ' cbDatabaseFields.SetItemChecked(cbDatabaseFields.Items.IndexOf(FieldName), False)
                Else
                    epOptions.SetError(txtDefaultSourceField, "")
                End If
            End If

            If FieldName = "OriginalTitle" Then
                'Original Title needs special handling - only required if TitleHandling says to use Internet lookup:
                If FieldChecked = True Then
                    If cbTitleHandling.SelectedIndex > -1 Then
                        If cbTitleHandling.SelectedItem.ToString.Contains("Internet Lookup") = True Then
                            InternetLookupNeeded = True
                        End If
                    End If
                    cbTitleHandling.Enabled = True
                Else
                    cbTitleHandling.Enabled = False
                End If
            ElseIf FieldName = "Date" Then
                If FieldChecked = True Then
                    cbDateHandling.Enabled = True
                Else
                    cbDateHandling.Enabled = False
                End If
            ElseIf FieldName = "Checked" Then
                If FieldChecked = True Then
                    cbCheckHandling.Enabled = True
                Else
                    cbCheckHandling.Enabled = False
                End If
            ElseIf FieldName = "Picture" Then
                If FieldChecked = True Then
                    cbPictureHandling.Enabled = True
                    txtPictureFilenamePrefix.Enabled = True
                    lblPictureHandling.Enabled = True
                    lblPicturePrefix.Enabled = True
                Else
                    cbPictureHandling.Enabled = False
                    txtPictureFilenamePrefix.Enabled = False
                    lblPictureHandling.Enabled = False
                    lblPicturePrefix.Enabled = False
                End If
            ElseIf FieldName = "Fanart" Then
                If FieldChecked = True Then
                    cbPictureHandling.Enabled = True
                    lblPictureHandling.Enabled = True
                    cbFanartLimitResolutionMin.Enabled = True
                    cbFanartLimitResolutionMax.Enabled = True
                    cbNumFanartLimitNumber.Enabled = True

                Else
                    'cbPictureHandling.Enabled = False
                    'lblPictureHandling.Enabled = False
                    cbFanartLimitResolutionMin.Enabled = False
                    cbFanartLimitResolutionMax.Enabled = False
                    cbNumFanartLimitNumber.Enabled = False
                End If
            End If

        Next

        If InternetLookupNeeded = True Then
            lblInternetLookupRequired.Visible = True
        Else
            lblInternetLookupRequired.Visible = False
        End If

        If chkFolderNameIsGroupName.Checked Then
            lblGroupNameAppliesTo.Enabled = True
            cbGroupNameAppliesTo.Enabled = True
        Else
            lblGroupNameAppliesTo.Enabled = False
            cbGroupNameAppliesTo.Enabled = False
        End If

        If cbPictureHandling.SelectedIndex > -1 Then
            If cbPictureHandling.SelectedItem = "Use Folder.jpg" Then
                txtPictureFilenamePrefix.Enabled = False
                lblPicturePrefix.Enabled = False
            Else
                txtPictureFilenamePrefix.Enabled = True
                lblPicturePrefix.Enabled = True
            End If
        Else
            txtPictureFilenamePrefix.Enabled = True
            lblPicturePrefix.Enabled = True
        End If

        If chkProhibitInternetLookup.Checked = True Then
            lblInternetLookupRequired.Visible = False
        End If


    End Sub

#End Region

#Region "Menus etc."
    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub
    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        frmAbout.ShowDialog()
    End Sub
    Private Sub MyFilmsWikiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyFilmsWikiToolStripMenuItem.Click
        System.Diagnostics.Process.Start("http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films/Updating_AMC_Data/AMC_Updater")
    End Sub
    Private Sub AMCUpdaterSourceforgeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AMCUpdaterSourceforgeToolStripMenuItem.Click
        System.Diagnostics.Process.Start("http://sourceforge.net/projects/amcupdater/")
    End Sub
    Private Sub AntMovieCatalogToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AntMovieCatalogToolStripMenuItem.Click
        System.Diagnostics.Process.Start("http://www.antp.be/software/moviecatalog")

    End Sub
    Private Sub MediaInfodllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MediaInfodllToolStripMenuItem.Click
        System.Diagnostics.Process.Start("http://mediainfo.sourceforge.net")
    End Sub
    Private Sub XMLToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles XMLToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblXml")
    End Sub
    Private Sub MediaFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MediaFileToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblFoundMediaFiles")
    End Sub
    Private Sub NonMediaFilesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NonMediaFilesToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblFoundNonMediaFiles")
    End Sub
    Private Sub TrailerFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MediaFileToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblFoundTrailerFiles")
    End Sub
    Private Sub OrphanMediaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OrphanMediaToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblOrphanedMediaFiles")
    End Sub
    Private Sub OrphanNonMediaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OrphanNonMediaToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblOrphanedNonMediaFiles")
    End Sub
    Private Sub OrphanTrailerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OrphanTrailerToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblOrphanedTrailerMediaFiles")
    End Sub
    Private Sub MultiPartFilesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MultiPartFilesToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblMultiPartFiles")
    End Sub
    Private Sub OrphanAntToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OrphanAntToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblOrphanedAntRecords")
    End Sub
    Private Sub MultipartProcessedFilesToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MultiPartProcessedFilesToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblMultiPartProcessedFiles")
    End Sub
    Private Sub AntFieldsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AntFieldsToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblAntFields")
    End Sub
    Private Sub NodesToProcessToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NodesToProcessToolStripMenuItem.Click
        AntProcessor.TEST_ListTable("tblNodesToProcess")
    End Sub
    Public Sub LoadConfigurationFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadConfigurationFileToolStripMenuItem.Click
        Dim OldSettingsFile As String
        OldSettingsFile = CurrentSettings.UserSettingsFile
        Dim SettingsFile As String = String.Empty
        Dim InitialDirectory As String
        Dim MePoConfigPath As String = Config.GetDirectoryInfo(Config.Dir.Config).ToString
        If System.IO.Directory.Exists(MePoConfigPath) Then
            InitialDirectory = MePoConfigPath
        Else
            InitialDirectory = My.Application.Info.DirectoryPath
        End If
        Try
            With OpenFileDialog1
                .InitialDirectory = InitialDirectory
                .FileName = "*.xml"
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a configuration file to load"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        SettingsFile = .FileName
                    Catch fileException As Exception
                        LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            Console.WriteLine(ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        If SettingsFile.Length > 0 Then
            'CurrentSettings.UserSettingsFile = SettingsFile
            CurrentSettings.LoadUserSettings(SettingsFile)
            LoadSettings()
            ' Add config file name to window title
            If (SettingsFile.ToString.Length > 0) Then
                Me.Text = "AMC Updater - " & SettingsFile.ToString.Substring(SettingsFile.ToString.LastIndexOf("\") + 1)
            Else
                Me.Text = "Ant Movie Catalog Auto-Updater"
            End If

            Me.ValidateChildren()
        End If

    End Sub
    Private Sub SaveConfigFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveConfigFileAsToolStripMenuItem.Click
        ApplySettings()
        Try
            With SaveFileDialog1
                .InitialDirectory = My.Application.Info.DirectoryPath
                .FileName = "AMCUpdater_Settings.xml"
                .CheckFileExists = False
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                '.Multiselect = False
                .RestoreDirectory = True
                .ShowHelp = True
                '.ShowReadOnly = False
                '.ReadOnlyChecked = False
                .Title = "Select a configuration file to save"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        CurrentSettings.UserSettingsFile = .FileName
                        CurrentSettings.SaveUserSettings()
                    Catch fileException As Exception
                        LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            Console.WriteLine(ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try

    End Sub
    Private Sub UserManualToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserManualToolStripMenuItem.Click
        Try
            If My.Computer.FileSystem.FileExists(My.Application.Info.DirectoryPath & "\AMCUpdater User Guide.pdf") Then
                System.Diagnostics.Process.Start(My.Application.Info.DirectoryPath & "\AMCUpdater User Guide.pdf")
            Else
                MsgBox("Sorry - User guide not found.")
            End If
        Catch ex As Exception
            MsgBox("Sorry - Cannot load user guide.  Do you have Acrobat Reader installed?")
        End Try
    End Sub
#End Region

#Region "Settings"
    Private Sub ApplySettings()
        CurrentSettings.Ant_Database_Source_Field = txtDefaultSourceField.Text
        CurrentSettings.Ant_Media_Label = txtMediaLabel.Text
        CurrentSettings.Ant_Media_Type = txtMediaType.Text
        CurrentSettings.Backup_XML_First = chkBackupXMLFirst.Checked
        CurrentSettings.Date_Handling = cbDateHandling.SelectedItem
        CurrentSettings.Movie_Title_Handling = cbTitleHandling.SelectedItem
        CurrentSettings.Excluded_Movies_File = txtExcludeFilePath.Text
        CurrentSettings.Manual_Excluded_Movies_File = txtManualExcludedMoviesPath.Text
        CurrentSettings.Execute_Only_For_Orphans = chkExecuteOnlyForOrphans.Checked
        CurrentSettings.Execute_Program = chkExecuteProgram.Checked
        CurrentSettings.Execute_Program_Path = txtExecuteProgramPath.Text
        CurrentSettings.File_Types_Media = txtDefaultFileTypes.Text
        CurrentSettings.File_Types_Non_Media = txtDefaultFileTypesNonMedia.Text
        CurrentSettings.File_Types_Trailer = txtTrailerIentificationStrings.Text
        CurrentSettings.Internet_Parser_Path = txtParserFilePath.Text
        CurrentSettings.Dont_Ask_Interactive = chkDontAskInteractive.Checked
        CurrentSettings.Manual_Dont_Ask_Interactive = chkManualDontAskInteractive.Checked
        CurrentSettings.Log_Level = cbLogLevel.SelectedItem
        CurrentSettings.Movie_Fanart_Path = txtFanartFolder.Text
        CurrentSettings.Movie_Fanart_Resolution_Min = cbFanartLimitResolutionMin.Text
        CurrentSettings.Movie_Fanart_Resolution_Max = cbFanartLimitResolutionMax.Text
        CurrentSettings.Movie_Fanart_Number_Limit = cbNumFanartLimitNumber.Value
        CurrentSettings.Movie_PersonArtwork_Path = txtPersonArtworkFolder.Text
        CurrentSettings.Movie_Scan_Path = txtMovieFolder.Text
        CurrentSettings.Override_Path = txtOverridePath.Text
        CurrentSettings.Overwrite_XML_File = chkOverwriteXML.Checked
        CurrentSettings.Purge_Missing_Files = chkPurgeMissing.Checked
        CurrentSettings.RegEx_Check_For_MultiPart_Files = txtRegExSearchMultiPart.Text
        CurrentSettings.Scan_For_DVD_Folders = chkCheckDVDFolders.Checked
        CurrentSettings.Store_Short_Names_Only = chkShortNames.Checked
        CurrentSettings.XML_File = txtConfigFilePath.Text
        CurrentSettings.Read_DVD_Label = chkReadDVDLabel.Checked
        CurrentSettings.Folder_Name_Is_Group_Name = chkFolderNameIsGroupName.Checked
        CurrentSettings.Parse_Playlist_Files = chkParsePlaylistFiles.Checked
        CurrentSettings.Parse_Trailers = chkParseTrailers.Checked
        CurrentSettings.Image_Download_Filename_Prefix = txtPictureFilenamePrefix.Text
        CurrentSettings.Prohibit_Internet_Lookup = chkProhibitInternetLookup.Checked
        CurrentSettings.Parse_Subtitle_Files = chkParseSubtitleFiles.Checked
        CurrentSettings.Rescan_Moved_Files = chkRescanMovedFiles.Checked
        CurrentSettings.Only_Add_Missing_Data = chkManualUpdateRecordsOnlyMissingData.Checked
        CurrentSettings.Only_Update_With_Nonempty_Data = chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Checked

        CurrentSettings.Manual_Internet_Lookup_Always_Prompt = cbManualInternetLookupBehaviour.SelectedValue
        CurrentSettings.Manual_Internet_Parser_Path = txtManualInternetParserPath.Text
        CurrentSettings.Manual_XML_File = txtManualXMLPath.Text

        CurrentSettings.Import_File_On_Internet_Lookup_Failure = chkImportOnInternetFail.Checked
        CurrentSettings.Dont_Import_File_On_Internet_Lookup_Failure_In_Guimode = chkImportOnInternetFailIgnoreWhenInteractive.Checked
        CurrentSettings.Internet_Lookup_Always_Prompt = cbInternetLookupBehaviour.SelectedValue

        CurrentSettings.Group_Name_Identifier = txtGroupNameIdentifier.Text




        Select Case cbPictureHandling.SelectedItem
            Case "Relative Path"
                CurrentSettings.Store_Image_With_Relative_Path = True
                CurrentSettings.Use_Folder_Dot_Jpg = False
                CurrentSettings.Create_Cover_From_Movie = False
            Case "Relative Path & Create Moviethumb"
                CurrentSettings.Store_Image_With_Relative_Path = True
                CurrentSettings.Use_Folder_Dot_Jpg = False
                CurrentSettings.Create_Cover_From_Movie = True
            Case "Use Folder.jpg"
                CurrentSettings.Use_Folder_Dot_Jpg = True
                CurrentSettings.Create_Cover_From_Movie = False
            Case "Create Moviethumb"
                CurrentSettings.Use_Folder_Dot_Jpg = True
                CurrentSettings.Create_Cover_From_Movie = True
            Case "Full Path"
                CurrentSettings.Store_Image_With_Relative_Path = False
                CurrentSettings.Use_Folder_Dot_Jpg = False
                CurrentSettings.Create_Cover_From_Movie = False
            Case "Full Path & Create Moviethumb"
                CurrentSettings.Store_Image_With_Relative_Path = False
                CurrentSettings.Use_Folder_Dot_Jpg = False
                CurrentSettings.Create_Cover_From_Movie = True
        End Select


        If cbCheckHandling.SelectedItem = "Checked" Then
            CurrentSettings.Check_Field_Handling = True
        Else
            CurrentSettings.Check_Field_Handling = False
        End If


        Dim DBFields As String = ""
        Dim FieldName As String
        Dim FieldChecked As String
        Dim item As Object

        'My.Settings.DatabaseFields = ""
        For Each item In cbDatabaseFields.Items
            FieldName = item.ToString
            FieldChecked = cbDatabaseFields.GetItemChecked(cbDatabaseFields.Items.IndexOf(FieldName)).ToString
            If Not DBFields.Contains(FieldName) Then
                DBFields += FieldName & "|" & FieldChecked & ";"
            End If
        Next
        If DBFields.EndsWith(";") Then
            DBFields = DBFields.Substring(0, DBFields.Length - 1)
        End If
        CurrentSettings.Database_Fields_To_Import = DBFields

        'Console.WriteLine(dgExcludedFileStrings.Rows.Count)
        Dim dgRow As System.Windows.Forms.DataGridViewRow
        Dim ExcludedFileString As String = String.Empty
        For Each dgRow In dgExcludedFileStrings.Rows
            If dgRow.Cells(0).Value IsNot Nothing Then
                If dgRow.Cells(0).Value.ToString.Length > 0 Then
                    ExcludedFileString += dgRow.Cells(0).Value.ToString & "|"
                End If
            End If
        Next
        If ExcludedFileString.Length > 0 Then
            ExcludedFileString = ExcludedFileString.Substring(0, ExcludedFileString.Length - 1)
        End If
        CurrentSettings.Excluded_File_Strings = ExcludedFileString
        'CurrentSettings.Excluded_Files_Table = dgExcludedFileStrings.DataSource

        Dim ExcludedFolderString As String = String.Empty
        For Each dgRow In dgExcludedFolderStrings.Rows
            If dgRow.Cells(0).Value IsNot Nothing Then
                If dgRow.Cells(0).Value.ToString.Length > 0 Then
                    ExcludedFolderString += dgRow.Cells(0).Value.ToString & "|"
                End If
            End If
        Next
        If ExcludedFolderString.Length > 0 Then
            ExcludedFolderString = ExcludedFolderString.Substring(0, ExcludedFolderString.Length - 1)
        End If
        CurrentSettings.Excluded_Folder_Strings = ExcludedFolderString

        Dim FilterString As String = String.Empty
        For Each dgRow In dgFilterStrings.Rows
            If dgRow.Cells(0).Value IsNot Nothing Then
                If dgRow.Cells(0).Value.ToString.Length > 0 Then
                    FilterString += dgRow.Cells(0).Value.ToString & "|"
                End If
            End If
        Next
        If FilterString.Length > 0 Then
            FilterString = FilterString.Substring(0, FilterString.Length - 1)
        End If
        CurrentSettings.Filter_Strings = FilterString

        CurrentSettings.Group_Name_Applies_To = cbGroupNameAppliesTo.SelectedItem.ToString
        CurrentSettings.Master_Title = cbMasterTitle.SelectedItem.ToString
        CurrentSettings.Grabber_Override_Language = chkGrabberOverrideLanguage.Text.ToString()
        CurrentSettings.Grabber_Override_GetRoles = chkGrabberOverrideGetRoles.Text.ToString
        CurrentSettings.Grabber_Override_PersonLimit = chkGrabberOverridePersonLimit.Text.ToString
        CurrentSettings.Grabber_Override_TitleLimit = chkGrabberOverrideTitleLimit.Text.ToString

    End Sub
    Private Sub LoadSettings()
        If Not CurrentSettings Is Nothing Then
            txtDefaultSourceField.Text = CurrentSettings.Ant_Database_Source_Field
            txtMediaLabel.Text = CurrentSettings.Ant_Media_Label
            txtMediaType.Text = CurrentSettings.Ant_Media_Type
            chkBackupXMLFirst.Checked = CurrentSettings.Backup_XML_First
            cbDateHandling.SelectedItem = CurrentSettings.Date_Handling
            cbTitleHandling.SelectedItem = CurrentSettings.Movie_Title_Handling
            txtExcludeFilePath.Text = CurrentSettings.Excluded_Movies_File
            chkExecuteOnlyForOrphans.Checked = CurrentSettings.Execute_Only_For_Orphans
            chkExecuteProgram.Checked = CurrentSettings.Execute_Program
            txtExecuteProgramPath.Text = CurrentSettings.Execute_Program_Path
            txtDefaultFileTypes.Text = CurrentSettings.File_Types_Media
            txtDefaultFileTypesNonMedia.Text = CurrentSettings.File_Types_Non_Media
            txtTrailerIentificationStrings.Text = CurrentSettings.File_Types_Trailer
            chkImportOnInternetFail.Checked = CurrentSettings.Import_File_On_Internet_Lookup_Failure
            chkImportOnInternetFailIgnoreWhenInteractive.Checked = CurrentSettings.Dont_Import_File_On_Internet_Lookup_Failure_In_Guimode
            cbInternetLookupBehaviour.SelectedValue = CurrentSettings.Internet_Lookup_Always_Prompt
            txtParserFilePath.Text = CurrentSettings.Internet_Parser_Path
            cbLogLevel.SelectedItem = CurrentSettings.Log_Level
            txtMovieFolder.Text = CurrentSettings.Movie_Scan_Path
            txtFanartFolder.Text = CurrentSettings.Movie_Fanart_Path
            cbFanartLimitResolutionMin.Text = CurrentSettings.Movie_Fanart_Resolution_Min
            cbFanartLimitResolutionMax.Text = CurrentSettings.Movie_Fanart_Resolution_Max
            cbNumFanartLimitNumber.Value = CurrentSettings.Movie_Fanart_Number_Limit
            txtPersonArtworkFolder.Text = CurrentSettings.Movie_PersonArtwork_Path
            txtOverridePath.Text = CurrentSettings.Override_Path
            chkOverwriteXML.Checked = CurrentSettings.Overwrite_XML_File
            chkPurgeMissing.Checked = CurrentSettings.Purge_Missing_Files
            txtRegExSearchMultiPart.Text = CurrentSettings.RegEx_Check_For_MultiPart_Files
            chkCheckDVDFolders.Checked = CurrentSettings.Scan_For_DVD_Folders
            chkShortNames.Checked = CurrentSettings.Store_Short_Names_Only
            txtConfigFilePath.Text = CurrentSettings.XML_File
            txtManualXMLPath.Text = CurrentSettings.XML_File
            chkReadDVDLabel.Checked = CurrentSettings.Read_DVD_Label
            chkDontAskInteractive.Checked = CurrentSettings.Dont_Ask_Interactive
            chkManualDontAskInteractive.Checked = CurrentSettings.Manual_Dont_Ask_Interactive
            chkFolderNameIsGroupName.Checked = CurrentSettings.Folder_Name_Is_Group_Name
            chkParsePlaylistFiles.Checked = CurrentSettings.Parse_Playlist_Files
            chkParseTrailers.Checked = CurrentSettings.Parse_Trailers
            txtPictureFilenamePrefix.Text = CurrentSettings.Image_Download_Filename_Prefix
            chkProhibitInternetLookup.Checked = CurrentSettings.Prohibit_Internet_Lookup
            chkParseSubtitleFiles.Checked = CurrentSettings.Parse_Subtitle_Files
            chkRescanMovedFiles.Checked = CurrentSettings.Rescan_Moved_Files
            chkManualUpdateRecordsOnlyMissingData.Checked = CurrentSettings.Only_Add_Missing_Data
            chkManualUpdateRecordsOnlyUpdateWhithNonEmptyData.Checked = CurrentSettings.Only_Update_With_Nonempty_Data

            txtManualExcludedMoviesPath.Text = CurrentSettings.Manual_Excluded_Movies_File
            txtManualInternetParserPath.Text = CurrentSettings.Manual_Internet_Parser_Path
            'txtManualXMLPath.Text = CurrentSettings.Manual_XML_File
            If CurrentSettings.Manual_Internet_Lookup_Always_Prompt = "True" Then
                cbManualInternetLookupBehaviour.SelectedValue = True
            Else
                cbManualInternetLookupBehaviour.SelectedValue = False
            End If
            If CurrentSettings.Check_Field_Handling = True Then
                cbCheckHandling.SelectedItem = "Checked"
            Else
                cbCheckHandling.SelectedItem = "Unchecked"
            End If

            cbGroupNameAppliesTo.SelectedItem = CurrentSettings.Group_Name_Applies_To
            cbMasterTitle.SelectedItem = CurrentSettings.Master_Title
            chkGrabberOverrideLanguage.Text = CurrentSettings.Grabber_Override_Language
            chkGrabberOverrideGetRoles.Text = CurrentSettings.Grabber_Override_GetRoles
            chkGrabberOverridePersonLimit.Text = CurrentSettings.Grabber_Override_PersonLimit
            chkGrabberOverrideTitleLimit.Text = CurrentSettings.Grabber_Override_TitleLimit

            txtGroupNameIdentifier.Text = CurrentSettings.Group_Name_Identifier

            Dim DBFields() As String
            DBFields = CurrentSettings.Database_Fields_To_Import.Split(";")
            Dim FieldName As String
            Dim FieldSelected As Boolean
            Dim FieldFanart As Boolean = False
            Dim FormattedTitle As Boolean = False
            cbDatabaseFields.Items.Clear()
            For i As Integer = 0 To DBFields.Length - 1
                If DBFields(i).Length > 0 Then
                    FieldName = DBFields(i).Substring(0, DBFields(i).IndexOf("|"))
                    If (FieldName = "Fanart") Then
                        FieldFanart = True
                    End If
                    If (FieldName = "FormattedTitle") Then
                        FormattedTitle = True
                    End If
                    FieldSelected = DBFields(i).Substring(DBFields(i).IndexOf("|") + 1)
                    cbDatabaseFields.Items.Add(FieldName, FieldSelected)
                End If
            Next
            If Not FieldFanart Then
                cbDatabaseFields.Items.Add("Fanart", False)
            End If
            If Not FormattedTitle Then
                cbDatabaseFields.Items.Add("FormattedTitle", False)
            End If
            Dim dtFolders As New DataTable
            dtFolders.Columns.Add("FolderString", System.Type.GetType("System.String"))
            If CurrentSettings.Excluded_Folder_Strings.Length > 0 Then
                Dim ExcludeFolders() As String
                ExcludeFolders = CurrentSettings.Excluded_Folder_Strings.Split("|")
                For i As Integer = 0 To ExcludeFolders.Length - 1
                    dtFolders.Rows.Add(ExcludeFolders(i))
                Next
            End If
            dgExcludedFolderStrings.DataSource = dtFolders
            dgExcludedFolderStrings.Columns(0).Width = dgExcludedFolderStrings.Width - dgExcludedFolderStrings.RowHeadersWidth - 20

            Dim dtFiles As New DataTable
            dtFiles.Columns.Add("FileString", System.Type.GetType("System.String"))
            If CurrentSettings.Excluded_File_Strings.Length > 0 Then
                Dim ExcludeFiles() As String
                ExcludeFiles = CurrentSettings.Excluded_File_Strings.Split("|")
                If ExcludeFiles.Length > 0 Then
                    For i As Integer = 0 To ExcludeFiles.Length - 1
                        dtFiles.Rows.Add(ExcludeFiles(i))
                    Next
                End If
            End If
            dgExcludedFileStrings.DataSource = dtFiles
            dgExcludedFileStrings.Columns(0).Width = dgExcludedFileStrings.Width - dgExcludedFileStrings.RowHeadersWidth - 20

            Dim dtFilterStrings As New DataTable
            dtFilterStrings.Columns.Add("FilterString", System.Type.GetType("System.String"))
            If CurrentSettings.Filter_Strings.Length > 0 Then
                Dim FilterStrings() As String
                FilterStrings = CurrentSettings.Filter_Strings.Split("|")
                If FilterStrings.Length > 0 Then
                    For i As Integer = 0 To FilterStrings.Length - 1
                        dtFilterStrings.Rows.Add(FilterStrings(i))
                    Next
                End If
            End If
            dgFilterStrings.DataSource = dtFilterStrings
            dgFilterStrings.Columns(0).Width = dgFilterStrings.Width - dgFilterStrings.RowHeadersWidth - 20


            If (CurrentSettings.Import_File_On_Internet_Lookup_Failure = "True") Then
                chkImportOnInternetFail.Checked = True
            Else
                chkImportOnInternetFail.Checked = False
            End If

            If (CurrentSettings.Dont_Import_File_On_Internet_Lookup_Failure_In_Guimode = "True") Then
                chkImportOnInternetFailIgnoreWhenInteractive.Checked = True
            Else
                chkImportOnInternetFailIgnoreWhenInteractive.Checked = False
            End If

            If chkOverwriteXML.Checked = True Then
                chkBackupXMLFirst.Enabled = True
            Else
                chkBackupXMLFirst.Checked = False
                chkBackupXMLFirst.Enabled = False
            End If

            If CurrentSettings.Internet_Lookup_Always_Prompt = "True" Then
                cbInternetLookupBehaviour.SelectedValue = True
            Else
                cbInternetLookupBehaviour.SelectedValue = False
            End If


            If CurrentSettings.Store_Image_With_Relative_Path = True Then
                cbPictureHandling.SelectedItem = "Relative Path"
            Else
                cbPictureHandling.SelectedItem = "Full Path"
            End If

            If CurrentSettings.Use_Folder_Dot_Jpg = True Then
                cbPictureHandling.SelectedItem = "Use Folder.jpg"
            End If

            If CurrentSettings.Create_Cover_From_Movie = True Then
                cbPictureHandling.SelectedItem = "Create Moviethumb"
            End If

            Me.ValidateChildren()

        End If
    End Sub

#End Region

    Private Sub btnSelectFanartFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectFanartFolder.Click
        Dim currentPath As String
        currentPath = txtFanartFolder.Text
        If currentPath.Contains(";") = True Then
            currentPath.Substring(currentPath.IndexOf(";") + 1)
        End If
        Try
            With FolderBrowserDialog1
                .RootFolder = Environment.SpecialFolder.Desktop
                .SelectedPath = currentPath
                .Description = "Select the directory where your movie backdrops fanart are stored."
                .ShowNewFolderButton = False

                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    txtFanartFolder.Text = .SelectedPath
                End If
            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()
    End Sub

    Private Sub btnSelectPersonArtworkFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectPersonArtworkFolder.Click
        Dim currentPath As String
        currentPath = txtPersonArtworkFolder.Text
        If currentPath.Contains(";") = True Then
            currentPath.Substring(currentPath.IndexOf(";") + 1)
        End If
        Try
            With FolderBrowserDialog1
                .RootFolder = Environment.SpecialFolder.Desktop
                .SelectedPath = currentPath
                .Description = "Select the directory where your person artwork thumbs are stored."
                .ShowNewFolderButton = False

                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    txtPersonArtworkFolder.Text = .SelectedPath
                End If
            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
        Me.ValidateChildren()

    End Sub

    Public Sub txtConfigFilePath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtConfigFilePath.TextChanged

        txtManualXMLPath.Text = txtConfigFilePath.Text
        CurrentSettings.XML_File = txtConfigFilePath.Text

    End Sub

    Private Sub VidéoBindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VidéoBindingNavigatorSaveItem.Click

        BindingNavigatorPositionItem.Focus()
        Dim destXml As New Xml.XmlTextWriter(CurrentSettings.XML_File, System.Text.Encoding.Default)
        destXml.WriteStartDocument(False)
        destXml.Formatting = Xml.Formatting.Indented
        mydivx.WriteXml(destXml)
        destXml.Close()
    End Sub

    Private Sub SpeichernToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SpeichernToolStripButton.Click
        BindingNavigatorPositionItem.Focus()
        Dim destXml As New Xml.XmlTextWriter(CurrentSettings.XML_File, System.Text.Encoding.Default)
        destXml.WriteStartDocument(False)
        destXml.Formatting = Xml.Formatting.Indented
        mydivx.WriteXml(destXml)
        destXml.Close()
    End Sub
    Private Sub BindingNavigatorAddNewItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BindingNavigatorAddNewItem.Click
        LogEvent("ERROR : ", EventLogLevel.Informational)
    End Sub
    Private Sub BindingNavigatorAddNewItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BindingNavigatorAddNewItem1.Click
        LogEvent("ERROR : ", EventLogLevel.Informational)
    End Sub

    Private Sub TabControl1_Selected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles TabControl1.Selected

        If TabControl1.SelectedIndex = 6 Or TabControl1.SelectedIndex = 7 Or TabControl1.SelectedIndex = 8 Then
            Dim myMovieTable As DataTable = Nothing
            'Dim mymovieview As DataView
            Dim myPersonTable As DataTable = Nothing
            'Dim mypersonview As DataView
            Dim myCustomFieldsProperties As DataTable = Nothing
            Dim myCustomField As DataTable = Nothing
            Dim myProperties As DataTable = Nothing
            Dim wdir As String

            If (txtConfigFilePath.Text.Length > 0) Then
                mydivx = New AntMovieCatalog()
                Try
                    wdir = System.IO.Path.GetDirectoryName(txtConfigFilePath.Text)
                Catch ex As Exception
                    wdir = String.Empty
                End Try
                If (System.IO.Directory.Exists(wdir)) Then
                    Directory.SetCurrentDirectory(wdir)
                End If

                If (System.IO.File.Exists(txtConfigFilePath.Text)) Then
                    mydivx.ReadXml(txtConfigFilePath.Text)

                    myMovieTable = mydivx.Tables("Movie")
                    'mymovieview = New DataView(myMovieTable)
                    MovieBindingSource.DataSource = myMovieTable

                    myPersonTable = mydivx.Tables("Person")
                    'mypersonview = New DataView(myPersonTable)
                    PersonBindingSource.DataSource = myPersonTable

                    myCustomField = mydivx.Tables("CustomField")
                    CustomFieldBindingSource.DataSource = myCustomField

                    myCustomFieldsProperties = mydivx.Tables("CustomFieldsProperties")
                    CustomFieldsPropertiesBindingSource.DataSource = myCustomFieldsProperties

                    myProperties = mydivx.Tables("Properties")
                    PropertiesBindingSource.DataSource = myProperties
                Else
                    MovieBindingSource.DataSource = myMovieTable
                    PersonBindingSource.DataSource = myPersonTable
                    CustomFieldBindingSource.DataSource = myCustomField
                    CustomFieldsPropertiesBindingSource.DataSource = myCustomFieldsProperties
                    PropertiesBindingSource.DataSource = myProperties
                End If
            End If
            MovieBindingSource.ResumeBinding()
            PersonBindingSource.ResumeBinding()
            CustomFieldBindingSource.ResumeBinding()
            CustomFieldsPropertiesBindingSource.ResumeBinding()
            PropertiesBindingSource.ResumeBinding()
        Else
            MovieBindingSource.SuspendBinding()
            PersonBindingSource.SuspendBinding()
            CustomFieldBindingSource.SuspendBinding()
            CustomFieldsPropertiesBindingSource.SuspendBinding()
            PropertiesBindingSource.SuspendBinding()
        End If
    End Sub

    Private Sub BindingNavigatorPositionItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BindingNavigatorPositionItem.Click

    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        '        TextBox2.Text = 1
    End Sub

    Private Sub SaveConfigFileToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveConfigFileToolStripMenuItem.Click
        ApplySettings()
        If My.Application.CommandLineArgs.Count > 0 Then
            Try
                Try
                    CurrentSettings.UserSettingsFile = My.Application.CommandLineArgs.Item(0)
                    CurrentSettings.SaveUserSettings()
                Catch fileException As Exception
                    LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                End Try

            Catch ex As Exception
                LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
                Console.WriteLine(ex.Message)
                MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
            End Try
        Else
            Try
                With SaveFileDialog1
                    .InitialDirectory = My.Application.Info.DirectoryPath
                    .FileName = "AMCUpdater_Settings.xml"
                    .CheckFileExists = False
                    .CheckPathExists = True
                    .DefaultExt = "xml"
                    .DereferenceLinks = True
                    .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                    '.Multiselect = False
                    .RestoreDirectory = True
                    .ShowHelp = True
                    '.ShowReadOnly = False
                    '.ReadOnlyChecked = False
                    .Title = "Select a configuration file to save"
                    .ValidateNames = True
                    If .ShowDialog = Windows.Forms.DialogResult.OK Then
                        Try
                            CurrentSettings.UserSettingsFile = .FileName
                            CurrentSettings.SaveUserSettings()
                        Catch fileException As Exception
                            LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                        End Try
                    End If

                End With
            Catch ex As Exception
                LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
                Console.WriteLine(ex.Message)
                MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
            End Try

        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Using p = New Process
            Dim psi As New ProcessStartInfo
            psi.FileName = Config.GetDirectoryInfo(Config.Dir.Base).ToString & "\MyFilms_Grabber_Interface.exe"
            psi.UseShellExecute = True
            psi.WindowStyle = ProcessWindowStyle.Normal
            psi.Arguments = """" & txtParserFilePath.Text & """"
            psi.ErrorDialog = True
            If (OSInfo.OSInfo.VistaOrLater()) Then
                psi.Verb = "runas"
            End If
            p.StartInfo = psi
            Try
                p.Start()
            Catch
            End Try
        End Using
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Using p = New Process
            Dim psi As New ProcessStartInfo
            psi.FileName = Config.GetDirectoryInfo(Config.Dir.Base).ToString & "\MyFilms_Grabber_Interface.exe"
            psi.UseShellExecute = True
            psi.WindowStyle = ProcessWindowStyle.Normal
            psi.Arguments = """" & txtManualInternetParserPath.Text & """"
            psi.ErrorDialog = True
            If (OSInfo.OSInfo.VistaOrLater()) Then
                psi.Verb = "runas"
            End If
            p.StartInfo = psi
            Try
                p.Start()
            Catch
            End Try
        End Using
    End Sub

    Private Sub txtParserFilePath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtParserFilePath.TextChanged
        If txtParserFilePath.Text <> "" Then
            txtParserFilePathDisplay.Text = Path.GetFileName(txtParserFilePath.Text)
        Else
            txtParserFilePathDisplay.Text = ""
        End If
    End Sub

    Private Sub btnManualExcludedMoviesFileShow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualExcludedMoviesFileShow.Click
        Dim t As String = txtManualExcludedMoviesPath.Text
        Try
            Process.Start(t)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnExcludeFileShow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExcludeFileShow.Click
        Dim t As String = txtExcludeFilePath.Text
        Try
            Process.Start(t)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnExcludeFileDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExcludeFileDelete.Click
        Dim dialogResult As Windows.Forms.DialogResult = Windows.Forms.MessageBox.Show("Are you sure you want to delete the file ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If dialogResult = Windows.Forms.DialogResult.OK Then
            Try
                My.Computer.FileSystem.DeleteFile(txtExcludeFilePath.Text)
                MsgBox("File deleted !", MsgBoxStyle.Information)
            Catch deleteException As Exception
                MsgBox("An error occurred ! - Error: " + deleteException.Message, MsgBoxStyle.Exclamation)
            End Try
        End If
    End Sub

    Private Sub btnManualExcludedMoviesFileDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnManualExcludedMoviesFileDelete.Click
        Dim dialogResult As Windows.Forms.DialogResult = Windows.Forms.MessageBox.Show("Are you sure you want to delete the file ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If dialogResult = Windows.Forms.DialogResult.OK Then
            Try
                My.Computer.FileSystem.DeleteFile(txtManualExcludedMoviesPath.Text)
                MsgBox("File deleted !", MsgBoxStyle.Information)
            Catch deleteException As Exception
                MsgBox("An error occurred ! - Error: " + deleteException.Message, MsgBoxStyle.Exclamation)
            End Try
        End If
    End Sub

    Private Sub txtManualInternetParserPath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtManualInternetParserPath.TextChanged
        If txtManualInternetParserPath.Text <> "" Then
            txtManualInternetParserPathDisplay.Text = Path.GetFileName(txtManualInternetParserPath.Text)
        Else
            txtManualInternetParserPathDisplay.Text = ""
        End If
    End Sub

    Private Sub LinkLabelMyFilmsWiki_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelMyFilmsWiki.LinkClicked
        System.Diagnostics.Process.Start("http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films/Updating_AMC_Data/AMC_Updater")
    End Sub

    Private Sub ToolStripButtonAddMissingPersons_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButtonAddMissingPersons.Click
        Dim Persons As String()
        For Each row As AntMovieCatalog.MovieRow In MovieBindingSource.DataSource
            If row.Actors IsNot Nothing Then
                Persons = row.Actors.Split(",")
                For Each Person As String In Persons
                    AddOrUpdatePerson(Person, PersonType.Actor)
                Next
            End If
            If row.Producer IsNot Nothing Then
                Persons = row.Producer.Split(",")
                For Each Person As String In Persons
                    AddOrUpdatePerson(Person, PersonType.Producer)
                Next
            End If
            If row.Director IsNot Nothing Then
                Persons = row.Director.Split(",")
                For Each Person As String In Persons
                    AddOrUpdatePerson(Person, PersonType.Director)
                Next
            End If
            If row.Writer IsNot Nothing Then
                Persons = row.Writer.Split(",")
                For Each Person As String In Persons
                    AddOrUpdatePerson(Person, PersonType.Writer)
                Next
            End If
        Next
    End Sub

    Private Enum PersonType
        Actor
        Producer
        Director
        Writer
    End Enum
    Private Sub AddOrUpdatePerson(ByVal person As String, ByVal type As PersonType)
        person = person.Trim
        If person.Contains("(") Then
            person = person.Substring(0, person.IndexOf("(")).Trim
        End If
        Dim personRows() As AntMovieCatalog.PersonRow
        personRows = PersonBindingSource.DataSource.Select("Name = '" + person + "'")

        'Dim personRow As MyFilmsPlugin.AntMovieCatalog.PersonRow
        'personRow = PersonBindingSource.DataSource.FindByName(name) ' .FindByMiniBiography()

        If personRows.Count > 0 Then
            personRows(0).Name = person
            Select Case type   ' Must be a primitive data type
                Case PersonType.Actor
                    personRows(0).IsActor = True
                Case PersonType.Producer
                    personRows(0).IsProducer = True
                Case PersonType.Director
                    personRows(0).IsDirector = True
                Case PersonType.Writer
                    personRows(0).IsWriter = True
            End Select
        Else
            'Dim newPerson As AMCUpdater.AntMovieCatalog.MovieRow
            Dim newPerson As AMCUpdater.AntMovieCatalog.PersonRow
            newPerson = PersonBindingSource.DataSource.NewPersonRow()
            newPerson.Name = person
            Select Case type   ' Must be a primitive data type
                Case PersonType.Actor
                    newPerson.IsActor = True
                Case PersonType.Producer
                    newPerson.IsProducer = True
                Case PersonType.Director
                    newPerson.IsDirector = True
                Case PersonType.Writer
                    newPerson.IsWriter = True
            End Select
            PersonBindingSource.DataSource.Rows.Add(newPerson)
        End If
    End Sub

    Private Sub ToolStripButtonGrabPersons_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButtonGrabPersons.Click

    End Sub
End Class

