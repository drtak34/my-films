Imports System.Windows.Forms

Public Class dgLogWindow




    Private Sub dgLogWindow_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Form1.btnShowHideLog.Text = "Show Log >>"
    End Sub

    Private Sub dgLogWindow_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.SetDesktopLocation(Form1.Location.X + Form1.Width, Form1.Location.Y)
        Me.Height = Form1.Height
        'dgLogWindow.SetDesktopLocation(Me.Location.X + Me.Width, Me.Location.Y)
    End Sub
End Class
