namespace MyFilmsPlugin.MyFilms.Configuration
{
  using System;
  using System.Windows.Forms;

  public partial class MyFilmsInputBox : Form
    {
        public MyFilmsInputBox()
        {
            InitializeComponent();
        }

        public string UserName
        {
            get { return textBoxNewName.Text; }
            set { textBoxNewName.Text = value; }
        }

        private void textBoxNewName_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxNewName_KeyUp(object sender, KeyEventArgs e)
        {
          try
          {
            if (e.KeyCode == Keys.Enter)
            {
              this.Close();
            }
            if (e.KeyCode == Keys.Escape)
            {
              this.Close();
            }
          }

          catch (Exception ex)
          {
            MessageBox.Show(ex.Message);
          }
        }
    }
}
