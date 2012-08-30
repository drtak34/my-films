<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DialogRename
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.OK_Button = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.TextBoxDirectoryNameCurrent = New System.Windows.Forms.TextBox
        Me.TextBoxDirectoryNameNew = New System.Windows.Forms.TextBox
        Me.LabelDirectoryCurrent = New System.Windows.Forms.Label
        Me.LabelFileNameCurrent = New System.Windows.Forms.Label
        Me.LabelHeadlineInfo = New System.Windows.Forms.Label
        Me.GroupBoxRenameDirectory = New System.Windows.Forms.GroupBox
        Me.LabelDirectoryNew = New System.Windows.Forms.Label
        Me.GroupBoxRenameFile = New System.Windows.Forms.GroupBox
        Me.TextBoxFileNameNew = New System.Windows.Forms.TextBox
        Me.TextBoxFileNameCurrent = New System.Windows.Forms.TextBox
        Me.LabelFileNameNew = New System.Windows.Forms.Label
        Me.LabelHeadline = New System.Windows.Forms.Label
        Me.TextBoxResultingPath = New System.Windows.Forms.TextBox
        Me.LabelResult = New System.Windows.Forms.Label
        Me.TextBoxStartPathCurrent = New System.Windows.Forms.TextBox
        Me.LabelResultShort = New System.Windows.Forms.Label
        Me.TextBoxResultingPathShort = New System.Windows.Forms.TextBox
        Me.LabelWarningMultiPartFiles = New System.Windows.Forms.Label
        Me.TextBoxAllPathWithMultiFiles = New System.Windows.Forms.TextBox
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBoxRenameDirectory.SuspendLayout()
        Me.GroupBoxRenameFile.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(435, 309)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'TextBoxDirectoryNameCurrent
        '
        Me.TextBoxDirectoryNameCurrent.Enabled = False
        Me.TextBoxDirectoryNameCurrent.Location = New System.Drawing.Point(95, 19)
        Me.TextBoxDirectoryNameCurrent.Name = "TextBoxDirectoryNameCurrent"
        Me.TextBoxDirectoryNameCurrent.Size = New System.Drawing.Size(307, 20)
        Me.TextBoxDirectoryNameCurrent.TabIndex = 1
        '
        'TextBoxDirectoryNameNew
        '
        Me.TextBoxDirectoryNameNew.Location = New System.Drawing.Point(95, 45)
        Me.TextBoxDirectoryNameNew.Name = "TextBoxDirectoryNameNew"
        Me.TextBoxDirectoryNameNew.Size = New System.Drawing.Size(307, 20)
        Me.TextBoxDirectoryNameNew.TabIndex = 2
        '
        'LabelDirectoryCurrent
        '
        Me.LabelDirectoryCurrent.AutoSize = True
        Me.LabelDirectoryCurrent.Location = New System.Drawing.Point(6, 22)
        Me.LabelDirectoryCurrent.Name = "LabelDirectoryCurrent"
        Me.LabelDirectoryCurrent.Size = New System.Drawing.Size(86, 13)
        Me.LabelDirectoryCurrent.TabIndex = 3
        Me.LabelDirectoryCurrent.Text = "Current Directory"
        '
        'LabelFileNameCurrent
        '
        Me.LabelFileNameCurrent.AutoSize = True
        Me.LabelFileNameCurrent.Location = New System.Drawing.Point(6, 22)
        Me.LabelFileNameCurrent.Name = "LabelFileNameCurrent"
        Me.LabelFileNameCurrent.Size = New System.Drawing.Size(86, 13)
        Me.LabelFileNameCurrent.TabIndex = 4
        Me.LabelFileNameCurrent.Text = "Current Filename"
        '
        'LabelHeadlineInfo
        '
        Me.LabelHeadlineInfo.AutoSize = True
        Me.LabelHeadlineInfo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelHeadlineInfo.Location = New System.Drawing.Point(106, 8)
        Me.LabelHeadlineInfo.Name = "LabelHeadlineInfo"
        Me.LabelHeadlineInfo.Size = New System.Drawing.Size(248, 26)
        Me.LabelHeadlineInfo.TabIndex = 5
        Me.LabelHeadlineInfo.Text = "Your source media will be modified !" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Do NOT use this for multi Part Files (not s" & _
            "upported)."
        '
        'GroupBoxRenameDirectory
        '
        Me.GroupBoxRenameDirectory.Controls.Add(Me.TextBoxDirectoryNameNew)
        Me.GroupBoxRenameDirectory.Controls.Add(Me.TextBoxDirectoryNameCurrent)
        Me.GroupBoxRenameDirectory.Controls.Add(Me.LabelDirectoryCurrent)
        Me.GroupBoxRenameDirectory.Controls.Add(Me.LabelDirectoryNew)
        Me.GroupBoxRenameDirectory.Location = New System.Drawing.Point(12, 37)
        Me.GroupBoxRenameDirectory.Name = "GroupBoxRenameDirectory"
        Me.GroupBoxRenameDirectory.Size = New System.Drawing.Size(414, 79)
        Me.GroupBoxRenameDirectory.TabIndex = 6
        Me.GroupBoxRenameDirectory.TabStop = False
        Me.GroupBoxRenameDirectory.Text = "Rename Directory"
        '
        'LabelDirectoryNew
        '
        Me.LabelDirectoryNew.AutoSize = True
        Me.LabelDirectoryNew.Location = New System.Drawing.Point(6, 48)
        Me.LabelDirectoryNew.Name = "LabelDirectoryNew"
        Me.LabelDirectoryNew.Size = New System.Drawing.Size(74, 13)
        Me.LabelDirectoryNew.TabIndex = 8
        Me.LabelDirectoryNew.Text = "New Directory"
        '
        'GroupBoxRenameFile
        '
        Me.GroupBoxRenameFile.Controls.Add(Me.TextBoxFileNameNew)
        Me.GroupBoxRenameFile.Controls.Add(Me.LabelFileNameCurrent)
        Me.GroupBoxRenameFile.Controls.Add(Me.TextBoxFileNameCurrent)
        Me.GroupBoxRenameFile.Controls.Add(Me.LabelFileNameNew)
        Me.GroupBoxRenameFile.Location = New System.Drawing.Point(12, 129)
        Me.GroupBoxRenameFile.Name = "GroupBoxRenameFile"
        Me.GroupBoxRenameFile.Size = New System.Drawing.Size(414, 79)
        Me.GroupBoxRenameFile.TabIndex = 7
        Me.GroupBoxRenameFile.TabStop = False
        Me.GroupBoxRenameFile.Text = "Rename File"
        '
        'TextBoxFileNameNew
        '
        Me.TextBoxFileNameNew.Location = New System.Drawing.Point(95, 45)
        Me.TextBoxFileNameNew.Name = "TextBoxFileNameNew"
        Me.TextBoxFileNameNew.Size = New System.Drawing.Size(307, 20)
        Me.TextBoxFileNameNew.TabIndex = 11
        '
        'TextBoxFileNameCurrent
        '
        Me.TextBoxFileNameCurrent.Enabled = False
        Me.TextBoxFileNameCurrent.Location = New System.Drawing.Point(95, 19)
        Me.TextBoxFileNameCurrent.Name = "TextBoxFileNameCurrent"
        Me.TextBoxFileNameCurrent.Size = New System.Drawing.Size(307, 20)
        Me.TextBoxFileNameCurrent.TabIndex = 10
        '
        'LabelFileNameNew
        '
        Me.LabelFileNameNew.AutoSize = True
        Me.LabelFileNameNew.Location = New System.Drawing.Point(6, 48)
        Me.LabelFileNameNew.Name = "LabelFileNameNew"
        Me.LabelFileNameNew.Size = New System.Drawing.Size(74, 13)
        Me.LabelFileNameNew.TabIndex = 9
        Me.LabelFileNameNew.Text = "New Filename"
        '
        'LabelHeadline
        '
        Me.LabelHeadline.AutoSize = True
        Me.LabelHeadline.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelHeadline.Location = New System.Drawing.Point(12, 9)
        Me.LabelHeadline.Name = "LabelHeadline"
        Me.LabelHeadline.Size = New System.Drawing.Size(74, 13)
        Me.LabelHeadline.TabIndex = 8
        Me.LabelHeadline.Text = "! Attention !"
        '
        'TextBoxResultingPath
        '
        Me.TextBoxResultingPath.Enabled = False
        Me.TextBoxResultingPath.Location = New System.Drawing.Point(58, 222)
        Me.TextBoxResultingPath.Name = "TextBoxResultingPath"
        Me.TextBoxResultingPath.Size = New System.Drawing.Size(520, 20)
        Me.TextBoxResultingPath.TabIndex = 9
        '
        'LabelResult
        '
        Me.LabelResult.AutoSize = True
        Me.LabelResult.Location = New System.Drawing.Point(15, 225)
        Me.LabelResult.Name = "LabelResult"
        Me.LabelResult.Size = New System.Drawing.Size(37, 13)
        Me.LabelResult.TabIndex = 10
        Me.LabelResult.Text = "Result"
        '
        'TextBoxStartPathCurrent
        '
        Me.TextBoxStartPathCurrent.Location = New System.Drawing.Point(478, 32)
        Me.TextBoxStartPathCurrent.Name = "TextBoxStartPathCurrent"
        Me.TextBoxStartPathCurrent.Size = New System.Drawing.Size(100, 20)
        Me.TextBoxStartPathCurrent.TabIndex = 11
        Me.TextBoxStartPathCurrent.Visible = False
        '
        'LabelResultShort
        '
        Me.LabelResultShort.AutoSize = True
        Me.LabelResultShort.Location = New System.Drawing.Point(15, 259)
        Me.LabelResultShort.Name = "LabelResultShort"
        Me.LabelResultShort.Size = New System.Drawing.Size(32, 13)
        Me.LabelResultShort.TabIndex = 12
        Me.LabelResultShort.Text = "Short"
        '
        'TextBoxResultingPathShort
        '
        Me.TextBoxResultingPathShort.Enabled = False
        Me.TextBoxResultingPathShort.Location = New System.Drawing.Point(58, 256)
        Me.TextBoxResultingPathShort.Name = "TextBoxResultingPathShort"
        Me.TextBoxResultingPathShort.Size = New System.Drawing.Size(520, 20)
        Me.TextBoxResultingPathShort.TabIndex = 13
        '
        'LabelWarningMultiPartFiles
        '
        Me.LabelWarningMultiPartFiles.AutoSize = True
        Me.LabelWarningMultiPartFiles.Location = New System.Drawing.Point(435, 151)
        Me.LabelWarningMultiPartFiles.Name = "LabelWarningMultiPartFiles"
        Me.LabelWarningMultiPartFiles.Size = New System.Drawing.Size(149, 39)
        Me.LabelWarningMultiPartFiles.TabIndex = 14
        Me.LabelWarningMultiPartFiles.Text = "Movie has more than one file !" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Filename renaming " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "has been disabled."
        '
        'TextBoxAllPathWithMultiFiles
        '
        Me.TextBoxAllPathWithMultiFiles.Location = New System.Drawing.Point(478, 6)
        Me.TextBoxAllPathWithMultiFiles.Name = "TextBoxAllPathWithMultiFiles"
        Me.TextBoxAllPathWithMultiFiles.Size = New System.Drawing.Size(100, 20)
        Me.TextBoxAllPathWithMultiFiles.TabIndex = 15
        Me.TextBoxAllPathWithMultiFiles.Visible = False
        '
        'DialogRename
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(593, 346)
        Me.Controls.Add(Me.TextBoxAllPathWithMultiFiles)
        Me.Controls.Add(Me.LabelWarningMultiPartFiles)
        Me.Controls.Add(Me.TextBoxResultingPathShort)
        Me.Controls.Add(Me.LabelResultShort)
        Me.Controls.Add(Me.TextBoxStartPathCurrent)
        Me.Controls.Add(Me.LabelResult)
        Me.Controls.Add(Me.TextBoxResultingPath)
        Me.Controls.Add(Me.LabelHeadline)
        Me.Controls.Add(Me.GroupBoxRenameFile)
        Me.Controls.Add(Me.GroupBoxRenameDirectory)
        Me.Controls.Add(Me.LabelHeadlineInfo)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DialogRename"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Rename and Ignore"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.GroupBoxRenameDirectory.ResumeLayout(False)
        Me.GroupBoxRenameDirectory.PerformLayout()
        Me.GroupBoxRenameFile.ResumeLayout(False)
        Me.GroupBoxRenameFile.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents TextBoxDirectoryNameCurrent As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxDirectoryNameNew As System.Windows.Forms.TextBox
    Friend WithEvents LabelDirectoryCurrent As System.Windows.Forms.Label
    Friend WithEvents LabelFileNameCurrent As System.Windows.Forms.Label
    Friend WithEvents LabelHeadlineInfo As System.Windows.Forms.Label
    Friend WithEvents GroupBoxRenameDirectory As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBoxRenameFile As System.Windows.Forms.GroupBox
    Friend WithEvents LabelDirectoryNew As System.Windows.Forms.Label
    Friend WithEvents LabelFileNameNew As System.Windows.Forms.Label
    Friend WithEvents TextBoxFileNameCurrent As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxFileNameNew As System.Windows.Forms.TextBox
    Friend WithEvents LabelHeadline As System.Windows.Forms.Label
    Friend WithEvents TextBoxResultingPath As System.Windows.Forms.TextBox
    Friend WithEvents LabelResult As System.Windows.Forms.Label
    Friend WithEvents TextBoxStartPathCurrent As System.Windows.Forms.TextBox
    Friend WithEvents LabelResultShort As System.Windows.Forms.Label
    Friend WithEvents TextBoxResultingPathShort As System.Windows.Forms.TextBox
    Friend WithEvents LabelWarningMultiPartFiles As System.Windows.Forms.Label
    Friend WithEvents TextBoxAllPathWithMultiFiles As System.Windows.Forms.TextBox

End Class
