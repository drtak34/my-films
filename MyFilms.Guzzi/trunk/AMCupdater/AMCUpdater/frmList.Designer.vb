<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmList
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

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lstOptions = New System.Windows.Forms.ListBox
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnOK = New System.Windows.Forms.Button
        Me.txtSearchString = New System.Windows.Forms.TextBox
        Me.btnSearchAgain = New System.Windows.Forms.Button
        Me.txtTmpParserFilePath = New System.Windows.Forms.TextBox
        Me.btnSelectParserFile = New System.Windows.Forms.Button
        Me.btnDontAskAgain = New System.Windows.Forms.Button
        Me.chkDontAskAgain = New System.Windows.Forms.CheckBox
        Me.ButtonGrabberOptions = New System.Windows.Forms.Button
        Me.txtSearchintYear = New System.Windows.Forms.TextBox
        Me.txtSearchhintIMDB_Id = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnCancelFromDialog = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'lstOptions
        '
        Me.lstOptions.FormattingEnabled = True
        Me.lstOptions.Location = New System.Drawing.Point(12, 73)
        Me.lstOptions.Name = "lstOptions"
        Me.lstOptions.Size = New System.Drawing.Size(693, 199)
        Me.lstOptions.TabIndex = 30
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(504, 283)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(94, 23)
        Me.btnCancel.TabIndex = 50
        Me.btnCancel.Text = "Ignore"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOK.Location = New System.Drawing.Point(618, 283)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(86, 23)
        Me.btnOK.TabIndex = 70
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'txtSearchString
        '
        Me.txtSearchString.Location = New System.Drawing.Point(12, 12)
        Me.txtSearchString.Name = "txtSearchString"
        Me.txtSearchString.Size = New System.Drawing.Size(353, 20)
        Me.txtSearchString.TabIndex = 0
        '
        'btnSearchAgain
        '
        Me.btnSearchAgain.Location = New System.Drawing.Point(24, 283)
        Me.btnSearchAgain.Name = "btnSearchAgain"
        Me.btnSearchAgain.Size = New System.Drawing.Size(94, 23)
        Me.btnSearchAgain.TabIndex = 40
        Me.btnSearchAgain.Text = "Search Again"
        Me.btnSearchAgain.UseVisualStyleBackColor = True
        '
        'txtTmpParserFilePath
        '
        Me.txtTmpParserFilePath.Location = New System.Drawing.Point(12, 42)
        Me.txtTmpParserFilePath.Name = "txtTmpParserFilePath"
        Me.txtTmpParserFilePath.Size = New System.Drawing.Size(522, 20)
        Me.txtTmpParserFilePath.TabIndex = 10
        '
        'btnSelectParserFile
        '
        Me.btnSelectParserFile.CausesValidation = False
        Me.btnSelectParserFile.Location = New System.Drawing.Point(540, 42)
        Me.btnSelectParserFile.Name = "btnSelectParserFile"
        Me.btnSelectParserFile.Size = New System.Drawing.Size(58, 20)
        Me.btnSelectParserFile.TabIndex = 20
        Me.btnSelectParserFile.Text = "Browse..."
        Me.btnSelectParserFile.UseVisualStyleBackColor = True
        '
        'btnDontAskAgain
        '
        Me.btnDontAskAgain.Location = New System.Drawing.Point(398, 283)
        Me.btnDontAskAgain.Name = "btnDontAskAgain"
        Me.btnDontAskAgain.Size = New System.Drawing.Size(94, 23)
        Me.btnDontAskAgain.TabIndex = 60
        Me.btnDontAskAgain.Text = "Always Ignore"
        Me.btnDontAskAgain.UseVisualStyleBackColor = True
        '
        'chkDontAskAgain
        '
        Me.chkDontAskAgain.AutoSize = True
        Me.chkDontAskAgain.Location = New System.Drawing.Point(215, 288)
        Me.chkDontAskAgain.Name = "chkDontAskAgain"
        Me.chkDontAskAgain.Size = New System.Drawing.Size(15, 14)
        Me.chkDontAskAgain.TabIndex = 49
        Me.chkDontAskAgain.TabStop = False
        Me.chkDontAskAgain.UseVisualStyleBackColor = True
        Me.chkDontAskAgain.Visible = False
        '
        'ButtonGrabberOptions
        '
        Me.ButtonGrabberOptions.Location = New System.Drawing.Point(604, 42)
        Me.ButtonGrabberOptions.Name = "ButtonGrabberOptions"
        Me.ButtonGrabberOptions.Size = New System.Drawing.Size(101, 20)
        Me.ButtonGrabberOptions.TabIndex = 71
        Me.ButtonGrabberOptions.Text = "Grabber Options"
        Me.ButtonGrabberOptions.UseVisualStyleBackColor = True
        '
        'txtSearchintYear
        '
        Me.txtSearchintYear.Enabled = False
        Me.txtSearchintYear.Location = New System.Drawing.Point(462, 12)
        Me.txtSearchintYear.Name = "txtSearchintYear"
        Me.txtSearchintYear.Size = New System.Drawing.Size(72, 20)
        Me.txtSearchintYear.TabIndex = 72
        '
        'txtSearchhintIMDB_Id
        '
        Me.txtSearchhintIMDB_Id.Enabled = False
        Me.txtSearchhintIMDB_Id.Location = New System.Drawing.Point(604, 12)
        Me.txtSearchhintIMDB_Id.Name = "txtSearchhintIMDB_Id"
        Me.txtSearchhintIMDB_Id.Size = New System.Drawing.Size(100, 20)
        Me.txtSearchhintIMDB_Id.TabIndex = 73
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(395, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(61, 26)
        Me.Label1.TabIndex = 74
        Me.Label1.Text = "Searchhint " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Year"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(540, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(61, 26)
        Me.Label2.TabIndex = 75
        Me.Label2.Text = "Searchhint " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "IMDB_Id"
        '
        'btnCancelFromDialog
        '
        Me.btnCancelFromDialog.Location = New System.Drawing.Point(268, 283)
        Me.btnCancelFromDialog.Name = "btnCancelFromDialog"
        Me.btnCancelFromDialog.Size = New System.Drawing.Size(97, 23)
        Me.btnCancelFromDialog.TabIndex = 76
        Me.btnCancelFromDialog.Text = "Cancel Grabbing"
        Me.btnCancelFromDialog.UseVisualStyleBackColor = True
        '
        'frmList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(717, 318)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnCancelFromDialog)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtSearchhintIMDB_Id)
        Me.Controls.Add(Me.txtSearchintYear)
        Me.Controls.Add(Me.ButtonGrabberOptions)
        Me.Controls.Add(Me.chkDontAskAgain)
        Me.Controls.Add(Me.btnDontAskAgain)
        Me.Controls.Add(Me.btnSelectParserFile)
        Me.Controls.Add(Me.txtTmpParserFilePath)
        Me.Controls.Add(Me.btnSearchAgain)
        Me.Controls.Add(Me.txtSearchString)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.lstOptions)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.KeyPreview = True
        Me.Name = "frmList"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Please select the correct entry"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lstOptions As System.Windows.Forms.ListBox
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents txtSearchString As System.Windows.Forms.TextBox
    Friend WithEvents btnSearchAgain As System.Windows.Forms.Button
    Friend WithEvents txtTmpParserFilePath As System.Windows.Forms.TextBox
    Friend WithEvents btnSelectParserFile As System.Windows.Forms.Button
    Friend WithEvents btnDontAskAgain As System.Windows.Forms.Button
    Friend WithEvents chkDontAskAgain As System.Windows.Forms.CheckBox
    Friend WithEvents ButtonGrabberOptions As System.Windows.Forms.Button
    Friend WithEvents txtSearchintYear As System.Windows.Forms.TextBox
    Friend WithEvents txtSearchhintIMDB_Id As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnCancelFromDialog As System.Windows.Forms.Button
End Class
