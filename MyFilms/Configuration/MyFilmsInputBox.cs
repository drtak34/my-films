﻿namespace MyFilmsPlugin.MyFilms.Configuration
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

        public int SetupType
        {
          get { return cbSetupType.SelectedIndex; }
          set { cbSetupType.SelectedIndex = value; }
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
              lblSetupType.Visible = false;
              cbSetupType.Visible = false;
            }
            else
            {
              cbCountry.Enabled = true;
              cbCatalogType.Enabled = true;
              cbUseNfoGrabber.Enabled = true;
              lblSetupType.Visible = testMode;
              cbSetupType.Visible = testMode;
            }
          }
        }
        private bool showOnlyName = false;

        public bool TestMode
        {
          get { return testMode; }
          set
          {
            testMode = value;
            lblSetupType.Visible = testMode;
            cbSetupType.Visible = testMode;
          }
        }
        private bool testMode = false;

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

        private void cbCatalogType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbSetupType.SelectedIndex)
            {
              case 0: // local config
              case 1: // network master
                cbUseNfoGrabber.Visible = cbCatalogType.Text.Contains("Ant Movie Catalog");
                break;
              case 2: // network client
                cbUseNfoGrabber.Visible = false;
                break;
            }
        }

        private void cbSetupType_SelectedIndexChanged(object sender, EventArgs e)
        {
          switch (cbSetupType.SelectedIndex)
          {
            case 0: // local config
            case 1: // network master
              label1.Visible = true;
              label2.Visible = true;
              label3.Visible = true;
              label4.Visible = true;
              lblCountry.Visible = true;
              cbCatalogType.Visible = true;
              cbCountry.Visible = true;
              cbUseNfoGrabber.Visible = cbCatalogType.Text.Contains("Ant Movie Catalog");
              textBoxNewName.Visible = true;
              lblNetworkClientInfo.Visible = false;
              break;
            case 2: // network client
              label1.Visible = false;
              label2.Visible = false;
              label3.Visible = false;
              label4.Visible = false;
              lblCountry.Visible = false;
              cbCatalogType.Visible = false;
              cbCountry.Visible = false;
              cbUseNfoGrabber.Visible = false;
              textBoxNewName.Visible = false;
              lblNetworkClientInfo.Visible = true;
              break;
            default:
              break;
          }
        }
    }
}
