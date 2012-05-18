<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMovieUpdate
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMovieUpdate))
        Me.DgvUpdateMovie = New System.Windows.Forms.DataGridView
        Me.AntMovieCatalog = New AMCUpdater.AntMovieCatalog
        Me.MovieBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ButtonOK = New System.Windows.Forms.Button
        Me.ButtonCancel = New System.Windows.Forms.Button
        Me.cbUpdate = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.tbItem = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.tbCurrent = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.tbNew = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.PictureBoxOld = New System.Windows.Forms.PictureBox
        Me.PictureBoxNew = New System.Windows.Forms.PictureBox
        CType(Me.DgvUpdateMovie, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AntMovieCatalog, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MovieBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBoxOld, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBoxNew, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DgvUpdateMovie
        '
        Me.DgvUpdateMovie.AllowUserToAddRows = False
        Me.DgvUpdateMovie.AllowUserToDeleteRows = False
        Me.DgvUpdateMovie.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DgvUpdateMovie.BackgroundColor = System.Drawing.SystemColors.Control
        Me.DgvUpdateMovie.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DgvUpdateMovie.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DgvUpdateMovie.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.cbUpdate, Me.tbItem, Me.tbCurrent, Me.tbNew})
        Me.DgvUpdateMovie.Location = New System.Drawing.Point(3, 4)
        Me.DgvUpdateMovie.Name = "DgvUpdateMovie"
        Me.DgvUpdateMovie.RowHeadersVisible = False
        Me.DgvUpdateMovie.Size = New System.Drawing.Size(904, 433)
        Me.DgvUpdateMovie.TabIndex = 0
        '
        'AntMovieCatalog
        '
        Me.AntMovieCatalog.DataSetName = "AntMovieCatalog"
        Me.AntMovieCatalog.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'MovieBindingSource
        '
        Me.MovieBindingSource.DataMember = "Movie"
        Me.MovieBindingSource.DataSource = Me.AntMovieCatalog
        '
        'ButtonOK
        '
        Me.ButtonOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonOK.Location = New System.Drawing.Point(821, 568)
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.Size = New System.Drawing.Size(75, 23)
        Me.ButtonOK.TabIndex = 1
        Me.ButtonOK.Text = "OK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Location = New System.Drawing.Point(728, 568)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButtonCancel.TabIndex = 2
        Me.ButtonCancel.Text = "Cancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'cbUpdate
        '
        Me.cbUpdate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.cbUpdate.HeaderText = "Update"
        Me.cbUpdate.Name = "cbUpdate"
        Me.cbUpdate.Width = 48
        '
        'tbItem
        '
        Me.tbItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.tbItem.HeaderText = "Item"
        Me.tbItem.Name = "tbItem"
        Me.tbItem.Width = 52
        '
        'tbCurrent
        '
        Me.tbCurrent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.tbCurrent.HeaderText = "Current Value"
        Me.tbCurrent.Name = "tbCurrent"
        '
        'tbNew
        '
        Me.tbNew.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.tbNew.HeaderText = "New Value"
        Me.tbNew.Name = "tbNew"
        '
        'PictureBoxOld
        '
        Me.PictureBoxOld.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PictureBoxOld.DataBindings.Add(New System.Windows.Forms.Binding("Image", Me.MovieBindingSource, "Picture", True))
        Me.PictureBoxOld.Location = New System.Drawing.Point(12, 450)
        Me.PictureBoxOld.Name = "PictureBoxOld"
        Me.PictureBoxOld.Size = New System.Drawing.Size(97, 141)
        Me.PictureBoxOld.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBoxOld.TabIndex = 3
        Me.PictureBoxOld.TabStop = False
        '
        'PictureBoxNew
        '
        Me.PictureBoxNew.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PictureBoxNew.Location = New System.Drawing.Point(148, 450)
        Me.PictureBoxNew.Name = "PictureBoxNew"
        Me.PictureBoxNew.Size = New System.Drawing.Size(97, 141)
        Me.PictureBoxNew.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBoxNew.TabIndex = 4
        Me.PictureBoxNew.TabStop = False
        '
        'frmMovieUpdate
        '
        Me.AcceptButton = Me.ButtonOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonCancel
        Me.ClientSize = New System.Drawing.Size(908, 603)
        Me.Controls.Add(Me.PictureBoxNew)
        Me.Controls.Add(Me.PictureBoxOld)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonOK)
        Me.Controls.Add(Me.DgvUpdateMovie)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMovieUpdate"
        Me.Text = "Movie Update"
        CType(Me.DgvUpdateMovie, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AntMovieCatalog, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MovieBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBoxOld, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBoxNew, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DgvUpdateMovie As System.Windows.Forms.DataGridView
    Friend WithEvents AntMovieCatalog As AMCUpdater.AntMovieCatalog
    Friend WithEvents MovieBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ButtonOK As System.Windows.Forms.Button
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button
    Friend WithEvents cbUpdate As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents tbItem As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents tbCurrent As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents tbNew As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PictureBoxOld As System.Windows.Forms.PictureBox
    Friend WithEvents PictureBoxNew As System.Windows.Forms.PictureBox
End Class
