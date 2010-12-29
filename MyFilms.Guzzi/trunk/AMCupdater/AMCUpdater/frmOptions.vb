Public Class frmOptions

    Shared MediaData As Hashtable
    Shared InternetData As Hashtable

    Private Sub frmOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        MediaData = New Hashtable
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

        InternetData = New Hashtable
        InternetData.Add("rating", "rating")
        InternetData.Add("year", "year")
        InternetData.Add("originaltitle", "originaltitle")
        InternetData.Add("translatedtitle", "translatedtitle")
        InternetData.Add("director", "director")
        InternetData.Add("producer", "producer")
        InternetData.Add("country", "country")
        InternetData.Add("category", "category")
        InternetData.Add("actors", "actors")
        InternetData.Add("url", "url")
        InternetData.Add("description", "description")
        InternetData.Add("comments", "comments")
        InternetData.Add("languages", "languages")
        InternetData.Add("subtitles", "subtitles")
        InternetData.Add("picture", "picture")



        txtDefaultMoviesFolder.Text = CurrentSettings.Movie_Scan_Path
        txtDefaultXMLPath.Text = CurrentSettings.XML_File
        txtDefaultParserPath.Text = CurrentSettings.Internet_Parser_Path
        txtDefaultMediaLabel.Text = CurrentSettings.Ant_Media_Label
        txtDefaultMediaType.Text = CurrentSettings.Ant_Media_Type
        txtDefaultOverridePath.Text = CurrentSettings.Override_Path
        txtDefaultExcludePath.Text = CurrentSettings.Excluded_Movies_File
        chkDefaultOverwriteXML.Checked = CurrentSettings.Overwrite_XML_File
        chkDefaultBackupXML.Checked = CurrentSettings.Backup_XML_First
        chkShortNames.Checked = CurrentSettings.Store_Short_Names_Only
        chkDefaultPurgeDatabase.Checked = CurrentSettings.Purge_Missing_Files

        txtDefaultFileTypes.Text = CurrentSettings.File_Types_Media
        txtDefaultFileTypesNonMedia.Text = CurrentSettings.File_Types_Non_Media

        txtDefaultSourceField.Text = CurrentSettings.Ant_Database_Source_Field

        txtRegExSearchMultiPart.Text = CurrentSettings.RegEx_Check_For_MultiPart_Files

        chkCheckDVDFolders.Checked = CurrentSettings.Scan_For_DVD_Folders

        If chkDefaultOverwriteXML.Checked = True Then
            chkDefaultBackupXML.Enabled = True
        Else
            chkDefaultBackupXML.Checked = False
            chkDefaultBackupXML.Enabled = False
        End If

        chkExecuteProgram.Checked = CurrentSettings.Execute_Program
        chkExecuteOnlyForOrphans.Checked = CurrentSettings.Execute_Only_For_Orphans
        txtExecuteProgramPath.Text = CurrentSettings.Execute_Program_Path

        cbLogLevel.SelectedItem = CurrentSettings.Log_Level

        If CurrentSettings.Date_Handling <> "" Then
            cbDateHandling.SelectedItem = CurrentSettings.Date_Handling
        Else
            cbDateHandling.SelectedIndex = 0
            'File Created Date
            'Current System Date
            'No Date
        End If

        If chkExecuteProgram.Checked Then
            txtExecuteProgramPath.Enabled = True
            btnExecuteProgramSelectPath.Enabled = True
            chkExecuteOnlyForOrphans.Enabled = True
        Else
            txtExecuteProgramPath.Enabled = False
            btnExecuteProgramSelectPath.Enabled = False
            chkExecuteOnlyForOrphans.Enabled = False
        End If

        Dim DBFields() As String
        DBFields = CurrentSettings.Database_Fields_To_Import.Split(";")
        Dim FieldName As String
        Dim FieldSelected As Boolean
        cbDatabaseFields.Items.Clear()

        For i As Integer = 0 To DBFields.Length - 1
            If DBFields(i).Length > 0 Then
                FieldName = DBFields(i).Substring(0, DBFields(i).IndexOf("|"))
                FieldSelected = DBFields(i).Substring(DBFields(i).IndexOf("|") + 1)

                cbDatabaseFields.Items.Add(FieldName, FieldSelected)
            End If
        Next

        chkImportOnInternetFail.Checked = CurrentSettings.Import_File_On_Internet_Lookup_Failure

        Dim dtInternetLookupBehaviour As New DataTable
        With dtInternetLookupBehaviour
            .Columns.Add("Value", System.Type.GetType("System.Boolean"))
            .Columns.Add("Display", System.Type.GetType("System.String"))
            .Rows.Add(True, "Always offer choice of movie")
            .Rows.Add(False, "Try to find best match automagically")
        End With
        With cbInternetLookupBehaviour
            .DataSource = dtInternetLookupBehaviour
            .DisplayMember = "Display"
            .ValueMember = "Value"
            .SelectedIndex = -1
        End With

        If CurrentSettings.Internet_Lookup_Always_Prompt = "True" Then
            cbInternetLookupBehaviour.SelectedValue = True
        Else
            cbInternetLookupBehaviour.SelectedValue = False
        End If

    End Sub

    Public Sub btnSaveOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveOptions.Click

        Dim ShowDVDWarning As Boolean = False
        Dim MsgTxt As String
        MsgTxt = "Warning - it looks like you've added .ifo as a filetype to scan."
        MsgTxt += vbCrLf & "This could cause duplicate entries if you also have the 'Check for DVD Copies' option selected."
        MsgTxt += vbCrLf & "Are you sure you wish to save this configuration?"

        If txtDefaultFileTypes.Text <> "" Then
            If txtDefaultFileTypes.Text.IndexOf("ifo") > 0 Then
                ShowDVDWarning = True
            End If
        End If
        If txtDefaultFileTypesNonMedia.Text <> "" Then
            If txtDefaultFileTypesNonMedia.Text.IndexOf("ifo") > 0 Then
                ShowDVDWarning = True
            End If
        End If

        If ShowDVDWarning = True Then
            If MsgBox(MsgTxt, MsgBoxStyle.OkCancel) = MsgBoxResult.Cancel Then
                Exit Sub
            End If
        End If


        Dim ants As New AntSettings
        Try
            With OpenFileDialog1
                .InitialDirectory = Environment.SpecialFolder.Desktop
                .FileName = "AMCUpdater_Settings.xml"
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
                .Title = "Select a configuration file to save"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        ants.UserSettingsFile = .FileName
                        CurrentSettings.Options_Current_Settings_Load("Options")
                        CurrentSettings.SaveUserSettings(ants.UserSettingsFile)
                    Catch fileException As Exception
                        fnLogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            fnLogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            Console.WriteLine(ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try

        Me.Close()

    End Sub

    Private Sub chkDefaultOverwriteXML_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDefaultOverwriteXML.CheckedChanged
        If chkDefaultOverwriteXML.Checked = True Then
            chkDefaultBackupXML.Enabled = True
        Else
            chkDefaultBackupXML.Checked = False
            chkDefaultBackupXML.Enabled = False
        End If
    End Sub

    Private Sub txtDefaultMoviesFolder_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDefaultMoviesFolder.LostFocus
        'If txtDefaultMoviesFolder.Text <> "" And txtDefaultMoviesFolder.Text.EndsWith("\") <> True Then
        'txtDefaultMoviesFolder.Text = txtDefaultMoviesFolder.Text + "\"
        'End If

        Dim Path As String() = txtDefaultMoviesFolder.Text.Split(";")
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

        txtDefaultMoviesFolder.Text = ReturnPath

    End Sub

    Private Sub txtDefaultOverridePath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDefaultOverridePath.LostFocus
        If txtDefaultOverridePath.Text <> "" And txtDefaultOverridePath.Text.EndsWith("\") <> True Then
            txtDefaultOverridePath.Text = txtDefaultOverridePath.Text + "\"
        End If

    End Sub

    Private Sub btnCancelOptions_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancelOptions.Click
        Me.Dispose()
    End Sub

    Private Sub btnOptionsSelectFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOptionsSelectFolder.Click
        Dim wpath As String = String.Empty
        Try
            With FolderBrowserDialog1
                .RootFolder = Environment.SpecialFolder.Desktop
                .Description = "Select the directory where your movie files are stored."
                .ShowNewFolderButton = False

                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    If .SelectedPath.EndsWith("\") <> True Then
                        wpath = .SelectedPath + "\"
                    Else
                        wpath = .SelectedPath
                    End If
                End If

            End With
        Catch ex As Exception
            fnLogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
            Exit Sub
        End Try
        If txtDefaultMoviesFolder.Text = "" Then
            txtDefaultMoviesFolder.Text = wpath
        Else
            'txtMovieFolder.Text not empty ask if we want to add or replace
            Dim rep As Int32 = MsgBox("Do you want to Add that entry to the existing one ?", MsgBoxStyle.YesNoCancel)
            If rep = MsgBoxResult.Cancel Then
                Exit Sub
            Else
                If rep = MsgBoxResult.Yes Then
                    txtDefaultMoviesFolder.Text = txtDefaultMoviesFolder.Text + ";" + wpath
                Else
                    txtDefaultMoviesFolder.Text = wpath
                End If
            End If
        End If
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
                        fnLogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            fnLogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            Console.WriteLine(ex.Message)
            '            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
    End Sub

    Private Sub btnOptionsSelectConfigFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOptionsSelectConfigFile.Click
        Try
            With OpenFileDialog1
                .InitialDirectory = Environment.SpecialFolder.Desktop
                .FileName = ""
                .CheckFileExists = False
                .CheckPathExists = True
                .DefaultExt = "xml"
                .DereferenceLinks = True
                .Filter = "XML files (*.xml)|*.xml|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = False
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtDefaultXMLPath.Text = .FileName
                        'txtConfigFilePath.Text = .FileName
                    Catch fileException As Exception
                        fnLogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            fnLogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            Console.WriteLine(ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
    End Sub

    Private Sub btnApplyDefaults_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApplyDefaults.Click

        If txtDefaultMoviesFolder.Text <> "" Then
            Form1.txtMovieFolder.Text = txtDefaultMoviesFolder.Text
        End If

        If txtDefaultXMLPath.Text <> "" Then
            Form1.txtConfigFilePath.Text = txtDefaultXMLPath.Text
        End If

        If txtDefaultExcludePath.Text <> "" Then
            Form1.txtExcludeFilePath.Text = txtDefaultExcludePath.Text
        End If
        If txtDefaultOverridePath.Text <> "" Then
            Form1.txtOverridePath.Text = txtDefaultOverridePath.Text
        End If
        If txtDefaultMediaLabel.Text <> "" Then
            Form1.txtMediaLabel.Text = txtDefaultMediaLabel.Text
        End If

        If txtDefaultMediaType.Text <> "" Then
            Form1.txtMediaType.Text = txtDefaultMediaType.Text
        End If

        If txtDefaultParserPath.Text <> "" Then
            Form1.txtParserFilePath.Text = txtDefaultParserPath.Text
        End If

        If txtDefaultSourceField.Text <> "" Then
            Form1.txtSourceField.Text = txtDefaultSourceField.Text
        End If

        Form1.chkOverwriteXML.Checked = chkDefaultOverwriteXML.Checked
        Form1.chkBackupXMLFirst.Checked = chkDefaultBackupXML.Checked
        Form1.chkPurgeMissing.Checked = chkDefaultPurgeDatabase.Checked
        Form1.chkShortNames.Checked = chkShortNames.Checked
        Form1.chkImportOnInternetFail.Checked = chkImportOnInternetFail.Checked

        Form1.cbInternetLookupBehaviour.SelectedValue = cbInternetLookupBehaviour.SelectedValue

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
    End Sub

    Private Sub btnOptionsSelectParserFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOptionsSelectParserFile.Click
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
                .RestoreDirectory = False
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file to open"
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtDefaultParserPath.Text = .FileName
                    Catch fileException As Exception
                        fnLogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            fnLogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            Console.WriteLine(ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
    End Sub

    Private Sub btnOptionsSelectExcludedFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOptionsSelectExcludedFile.Click
        Try
            With OpenFileDialog1
                .InitialDirectory = Environment.SpecialFolder.Desktop
                .FileName = ""
                .CheckFileExists = True
                .CheckPathExists = True
                .DefaultExt = "txt"
                .DereferenceLinks = True
                .Filter = "txt files (*.txt)|*.txt|All files|*.*"
                .Multiselect = False
                .RestoreDirectory = False
                .ShowHelp = True
                .ShowReadOnly = False
                .ReadOnlyChecked = False
                .Title = "Select a file for excluded Movies "
                .ValidateNames = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtDefaultExcludePath.Text = .FileName
                    Catch fileException As Exception
                        fnLogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            fnLogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            Console.WriteLine(ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
    End Sub

    Private Sub btnDBFieldsSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectAll.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            cbDatabaseFields.SetItemChecked(i, True)
        Next
    End Sub

    Private Sub btnDBFieldsSelectNone_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectNone.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            cbDatabaseFields.SetItemChecked(i, False)
        Next

    End Sub

    Private Sub btnDBFieldsSelectAllMedia_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectAllMedia.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If MediaData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                cbDatabaseFields.SetItemChecked(i, True)
            End If
        Next
    End Sub

    Private Sub btnDBFieldsSelectNoMedia_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectNoMedia.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If MediaData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                cbDatabaseFields.SetItemChecked(i, False)
            End If
        Next
    End Sub

    Private Sub btnDBFieldsSelectAllInternet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectAllInternet.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If InternetData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                cbDatabaseFields.SetItemChecked(i, True)
            End If
        Next
    End Sub

    Private Sub btnDBFieldsSelectNoInternet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDBFieldsSelectNoInternet.Click
        For i As Integer = 0 To cbDatabaseFields.Items.Count - 1
            If InternetData.ContainsKey(cbDatabaseFields.Items(i).ToString.ToLower) Then
                cbDatabaseFields.SetItemChecked(i, False)
            End If
        Next
    End Sub

    Private Sub txtDefaultExcludePath_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDefaultExcludePath.Leave
        If Not (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(txtDefaultExcludePath.Text))) Then
            MsgBox("The directory " + System.IO.Path.GetDirectoryName(txtDefaultExcludePath.Text) + " doesn't exist. Please correct it ", MsgBoxStyle.Exclamation, Me.Text)
            txtDefaultExcludePath.Focus()
        End If
    End Sub

End Class

