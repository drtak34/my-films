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

        private void buttonOK_Click(object sender, EventArgs e)
        {

        }
    }
}
