Public Class frmMovieUpdate

    Dim ImageViewer As New frmImageViewer()

    Private Sub ButtonOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonOK.Click
        Me.DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub ButtonCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub PictureBoxOld_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBoxOld.Click
        Dim imagefile = Me.PictureBoxOld.ImageLocation
        If Not System.IO.File.Exists(imagefile) Then
            'MsgBox("File '" + imagefile + "' does not exist !", MsgBoxStyle.OkOnly)
            Return
        End If
        ImageViewer.Text = "Image Viever"
        ImageViewer.PictureBoxImageViewer.ImageLocation = imagefile
        Try
            Dim f As New FileInfo(imagefile)
            ImageViewer.Text += " - Size = " + ByteString(f.Length)
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        Try
            Dim image As System.Drawing.Image = Drawing.Image.FromFile(imagefile)
            ImageViewer.Text += " - Format = " + image.Width.ToString + "x" + image.Height.ToString
            image.Dispose()
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        ImageViewer.ShowDialog()
    End Sub

    Private Sub PictureBoxNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBoxNew.Click
        Dim imagefile = Me.PictureBoxNew.ImageLocation
        If Not System.IO.File.Exists(imagefile) Then
            'MsgBox("File '" + imagefile + "' does not exist !", MsgBoxStyle.OkOnly)
            Return
        End If
        ImageViewer.Text = "Image Viever"
        ImageViewer.PictureBoxImageViewer.ImageLocation = imagefile
        Try
            Dim f As New FileInfo(imagefile)
            ImageViewer.Text += " - Size = " + ByteString(f.Length)
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        Try
            Dim image As System.Drawing.Image = Drawing.Image.FromFile(imagefile)
            ImageViewer.Text += " - Format = " + image.Width.ToString + "x" + image.Height.ToString
            image.Dispose()
        Catch ex As Exception
            MsgBox("Exception retrieving image data: " + ex.Message + ex.StackTrace, MsgBoxStyle.OkOnly)
        End Try
        ImageViewer.ShowDialog()
    End Sub
End Class