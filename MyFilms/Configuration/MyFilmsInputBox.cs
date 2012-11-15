namespace MyFilmsPlugin.MyFilms.Configuration
{
  using System;
  using System.Windows.Forms;

  using MyFilmsPlugin.MyFilms.Utils;

  public partial class MyFilmsInputBox : Form
    {
        public MyFilmsInputBox()
        {
            InitializeComponent();
        }

        public string ConfigName
        {
          get { return textBoxNewName.Text; }
          set { textBoxNewName.Text = value; }
        }

        public string CatalogType
        {
          get { return cbCatalogType.Text; }
          set { cbCatalogType.Text = value; }
        }
        public int CatalogTypeSelectedIndex
        {
          get { return cbCatalogType.SelectedIndex; }
          set { cbCatalogType.SelectedIndex = value; }
        }

        public string Country
        {
          get { return cbCountry.Text; }
          set { cbCountry.Text = value; }
        }

        public bool UseNfoGrabber
        {
          get { return cbUseNfoGrabber.Checked; }
          set { cbUseNfoGrabber.Checked = value; }
        }

        public bool HideNfoCheckBox
        {
          get { return hideNfoCheckBox; }
          set
          {
            hideNfoCheckBox = value;
            this.cbUseNfoGrabber.Enabled = !this.hideNfoCheckBox;
          }
        }
        private bool hideNfoCheckBox = false;

        public bool ShowOnlyName
        {
          get { return showOnlyName; }
          set
          {
            showOnlyName = value;
            if (showOnlyName)
            {
              cbCountry.Enabled = false;
              cbCatalogType.Enabled = false;
              cbUseNfoGrabber.Enabled = false;
            }
            else
            {
              cbCountry.Enabled = true;
              cbCatalogType.Enabled = true;
              cbUseNfoGrabber.Enabled = true;
            }
          }
        }
        private bool showOnlyName = false;


        private void textBoxNewName_TextChanged(object sender, EventArgs e)
        {
          textBoxNewName.Text = StringExtensions.XmlCharacterWhitelist(textBoxNewName.Text).Replace(@"'", "");
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
