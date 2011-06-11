Imports MediaPortal.Configuration

Public Class frmList

    Private SearchTextChanged As Boolean = False

    Private Sub lstOptions_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstOptions.DoubleClick
        Me.DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub btnSearchAgain_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearchAgain.Click
        If txtSearchString.Text <> "" Then
            Dim Gb As Grabber.Grabber_URLClass = New Grabber.Grabber_URLClass
            'Dim wurl As ArrayList
            wurl.Clear()
            wurl = Gb.ReturnURL(txtSearchString.Text, txtTmpParserFilePath.Text, 1, CurrentSettings.Internet_Lookup_Always_Prompt)
            If (wurl.Count > 0) Then
                lstOptions.Items.Clear()
                For i As Integer = 0 To wurl.Count - 1
                    lstOptions.Items.Add(wurl.Item(i).Title)
                Next
                lstOptions.SelectedIndex = 0
                btnOK.Enabled = True
            Else
                btnOK.Enabled = False
            End If
            SearchTextChanged = False
        Else
            MsgBox("You must enter some text in the search box!", MsgBoxStyle.OkOnly)
        End If
    End Sub

    Private Sub btnDontAskAgain_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDontAskAgain.Click
        'Add to xml file to ignore in future scans:
        Me.chkDontAskAgain.Checked() = True
        Me.DialogResult = Windows.Forms.DialogResult.Ignore
    End Sub

    Private Sub btnSelectParserFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectParserFile.Click
        Dim OpenParserFileDialog = New System.Windows.Forms.OpenFileDialog
        Try
            With OpenParserFileDialog
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
                        txtTmpParserFilePath.Text = .FileName
                    Catch fileException As Exception
                        LogEvent("ERROR : " + fileException.Message, EventLogLevel.ErrorOrSimilar)
                    End Try
                End If

            End With
        Catch ex As Exception
            LogEvent("ERROR : " + ex.Message, EventLogLevel.ErrorOrSimilar)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub txtSearchString_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtSearchString.KeyPress
        If e.KeyChar = Chr(13) Then
            If SearchTextChanged = True Then
                'User has changed the text - default to Search Again:
                e.Handled = True
                btnSearchAgain_Click(Me, EventArgs.Empty)
            Else
                'Current results match search text - return OK if an item is selected:
                e.Handled = True
                If lstOptions.Items.Count > 0 Then
                    If lstOptions.Items(0).ToString <> "Movie not found..." Then
                        Me.DialogResult = Windows.Forms.DialogResult.OK
                    End If
                End If
            End If
        Else
            SearchTextChanged = True
        End If
    End Sub

    Private Sub frmList_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        'Console.WriteLine(e.KeyCode.ToString)
        If e.KeyCode = Windows.Forms.Keys.Up Or e.KeyCode = Windows.Forms.Keys.Down Then
            If e.KeyCode = Windows.Forms.Keys.Up Then
                If Me.lstOptions.SelectedIndex > 0 Then
                    Me.lstOptions.SelectedIndex -= 1
                End If
            ElseIf e.KeyCode = Windows.Forms.Keys.Down Then
                If Me.lstOptions.SelectedIndex < Me.lstOptions.Items.Count - 1 Then
                    Me.lstOptions.SelectedIndex += 1
                End If
            End If
            e.Handled = True
        End If
    End Sub

    Private Sub frmList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.txtSearchString.Focus()
    End Sub

    Private Sub ButtonGrabberOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonGrabberOptions.Click
        Using p = New Process
            Dim psi As New ProcessStartInfo
            psi.FileName = Config.GetDirectoryInfo(Config.Dir.Base).ToString & "\MyFilms_Grabber_Interface.exe"
            psi.UseShellExecute = True
            psi.WindowStyle = ProcessWindowStyle.Normal
            psi.Arguments = """" & txtTmpParserFilePath.Text & """"
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

    Private Sub btnCancelFromDialog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancelFromDialog.Click
        Form1.AntProcessor.bgwFolderScanUpdate_Cancel()
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub
End Class