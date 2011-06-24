using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Web;
using System.Net;
using Grabber;
using System.Collections;
using System.Globalization;
using System.Reflection;


namespace Grabber_Interface
{
  using System.Diagnostics;

  using MediaPortal.Configuration;
  using MediaPortal.Util;

  public partial class GrabConfig : Form
  {
    private System.Resources.ResourceManager RM = new System.Resources.ResourceManager("Localisation.Form1", System.Reflection.Assembly.GetExecutingAssembly());
    private CultureInfo EnglishCulture = new CultureInfo("en-US");
    private CultureInfo FrenchCulture = new CultureInfo("fr-FR");

    static System.Windows.Forms.OpenFileDialog openFileDialog1 = new OpenFileDialog();
    static System.Windows.Forms.SaveFileDialog saveFileDialog1 = new SaveFileDialog();

    private int GLiSearch = 0;
    private int GLiSearchD = 0;
    //block auto text changed.
    private bool GLbBlock = false;
    //block auto selection changed in body
    private bool GLbBlockSelect = false;
    private string Body = string.Empty;
    private string BodyDetail = string.Empty;
    private string BodyDetail2 = string.Empty;
    private string BodyLinkImg = string.Empty;
    private string BodyLinkGeneric1 = string.Empty;
    private string BodyLinkGeneric2 = string.Empty;
    private string BodyLinkPersons = string.Empty;
    private string BodyLinkTitles = string.Empty;
    private string BodyLinkCertification = string.Empty;
    private string BodyLinkComment = string.Empty;
    private string BodyLinkSyn = string.Empty;

    private string URLBodyDetail = string.Empty;
    private string URLBodyDetail2 = string.Empty;
    private string URLBodyLinkImg = string.Empty;
    private string URLBodyLinkGeneric1 = string.Empty;
    private string URLBodyLinkGeneric2 = string.Empty;
    private string URLBodyLinkPersons = string.Empty;
    private string URLBodyLinkTitles = string.Empty;
    private string URLBodyLinkCertification = string.Empty;
    private string URLBodyLinkComment = string.Empty;
    private string URLBodyLinkSyn = string.Empty;

    private string TimeBodyDetail = string.Empty;
    private string TimeBodyDetail2 = string.Empty;
    private string TimeBodyLinkImg = string.Empty;
    private string TimeBodyLinkGeneric1 = string.Empty;
    private string TimeBodyLinkGeneric2 = string.Empty;
    private string TimeBodyLinkPersons = string.Empty;
    private string TimeBodyLinkTitles = string.Empty;
    private string TimeBodyLinkCertification = string.Empty;
    private string TimeBodyLinkComment = string.Empty;
    private string TimeBodyLinkSyn = string.Empty;
    
    private bool ExpertModeOn = true; // to toggle GUI for simplification

    private XmlConf xmlConf;
    private ArrayList listUrl = new ArrayList();
    private CookieContainer cookie = new CookieContainer();

    //TabPage tabPageSaveMovie = null;
    //TabPage tabPageSaveDetails = null;

    private string[] Fields = new string[30]; // List to hold all possible grab fields ...

    public GrabConfig(string[] args)
    {
      System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
      InitializeComponent();
      this.InitMappingTable(); // load Mappingtable with values and other initialization

      if (CultureInfo.InstalledUICulture.ToString() == FrenchCulture.ToString())
        radioButtonFR.Checked = true;
      else
        radioButtonEN.Checked = true;
      tabPageSearchPage.Enabled = false;
      tabPageDetailPage.Enabled = false;
      // Test if input arguments were supplied:
      if (args.Length > 0)
      {
        ExpertModeOn = false;
        ChangeVisibility(false);
        ResetFormControlValues(this);
        if (System.IO.File.Exists(args[0]))
        {
          textConfig.Text = args[0]; // load command line file
          LoadXml();
          //button_Load_Click(this, e);
        }
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms"))
      {
        openFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms";
      }
      else
      {
        openFileDialog1.RestoreDirectory = true;
      }
      openFileDialog1.Filter = "XML Files (*.xml)|*.xml";
      openFileDialog1.Title = "Find Internet Grabber Script file (xml file)";
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        ResetFormControlValues(this);
        textConfig.Text = openFileDialog1.FileName;
        LoadXml();
        button_Load_Click(this, e);
      }
    }

    private void comboSearch_SelectedIndexChanged(object sender, EventArgs e)
    {
      GLbBlock = true;
      buttonPrevParam1.Visible = true;

      switch (cb_Parameter.SelectedIndex)
      {
        case 0:
          TextKeyStart.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartList)._Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyEndList)._Value;
          buttonPrevParam1.Visible = false;
          break;
        case 1:
          TextKeyStart.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartTitle)._Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyEndTitle)._Value;
          textReplace.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartTitle)._Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartTitle)._Param2;
          break;
        case 2:
          TextKeyStart.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartYear)._Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyEndYear)._Value;
          textReplace.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartYear)._Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartYear)._Param2;
          break;
        case 3:
          TextKeyStart.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartDirector)._Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyEndDirector)._Value;
          textReplace.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartDirector)._Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartDirector)._Param2;
          break;
        case 4: // details page
          TextKeyStart.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartLink)._Value;
          TextKeyStop.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyEndLink)._Value;
          textReplace.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartLink)._Param1;
          textReplaceWith.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartLink)._Param2;
          break;

        default:
          TextKeyStart.Text = "";
          TextKeyStop.Text = "";
          textReplace.Text = "";
          textReplaceWith.Text = "";
          break;

      }

      if (cb_Parameter.SelectedIndex > 0)
      {
        textReplace.Visible = true;
        textReplaceWith.Visible = true;
        //btReset.Visible = true;
        labelReplace.Visible = true;
        labelReplaceWith.Visible = true;
      }
      else
      {
        textReplace.Text = "";
        textReplaceWith.Text = "";
        textReplace.Visible = false;
        textReplaceWith.Visible = false;
        labelReplace.Visible = false;
        labelReplaceWith.Visible = false;
        //btReset.Visible = false;
      }

      GLbBlock = false;
    }

    private void button_Load_Click(object sender, EventArgs e)
    {
      if (!TextURL.Text.Contains("#Search#"))
      {
        MessageBox.Show("Please, replace search keyword by #Search# in URL !", "Error");
        return;
      }

      if (TextSearch.Text.Length == 0)
      {
        MessageBox.Show("Please, insert search keyword !", "Error");
        return;
      }
      if (TextURL.Text.Contains("#Page#") && (textPage.Text.Length == 0))
      {
        MessageBox.Show("Please, give the page number to load !", "Error");
        return;
      }

      listPreview.Items.Clear();
      if (TextURL.Text.Length > 0)
      {
        string absoluteUri;
        string strSearch;
        int iStart;
        int iEnd;

        if (TextURL.Text.StartsWith("http://") == false)
          TextURL.Text = "http://" + TextURL.Text;


        strSearch = GrabUtil.encodeSearch(TextSearch.Text);
        string wurl = TextURL.Text.Replace("#Search#", strSearch);
        wurl = wurl.Replace("#Page#", textPage.Text);

        Body = GrabUtil.GetPage(wurl, null, out absoluteUri, cookie);

        //1 resultat -> redirection automatique vers la fiche
        if (!wurl.Equals(absoluteUri))
        {
          Body = "";
          textBody.Text = "";
          listUrl.Clear();
          listUrl.Add(new Grabber_URLClass.IMDBUrl(absoluteUri, TextSearch.Text + " (AutoRedirect)", null, null));

          listPreview.Items.Clear();

          listPreview.Items.Add(((Grabber_URLClass.IMDBUrl)listUrl[0]).Title);

          return;

        }

        if (textRedir.Text.Length > 0)
          Body = GrabUtil.GetPage(textRedir.Text, null, out absoluteUri, cookie);

        if (xmlConf.find(xmlConf.listSearch, TagName.KeyStartList)._Value.Length > 0)
        {
          iStart = Body.IndexOf(xmlConf.find(xmlConf.listSearch, TagName.KeyStartList)._Value);
          //Si la clé de début a été trouvé
          if (iStart > 0)
          {
            //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
            if (xmlConf.find(xmlConf.listSearch, TagName.KeyEndList)._Value != "")
            {
              iEnd = Body.IndexOf(xmlConf.find(xmlConf.listSearch, TagName.KeyEndList)._Value, iStart);
            }
            else
              iEnd = Body.Length;

            if (iEnd == -1)
              iEnd = Body.Length;

            //Découpage du body
            iStart += xmlConf.find(xmlConf.listSearch, TagName.KeyStartList)._Value.Length;
            textBody.Text = Body.Substring(iStart, iEnd - iStart);

          }
          else
            textBody.Text = Body;
        }
        else
          textBody.Text = Body;


      }
    }

    public void LoadXml()
    {
      // InitMappingTable();
      xmlConf = new XmlConf(textConfig.Text);

      textName.Text = xmlConf.find(xmlConf.listGen, TagName.DBName)._Value;
      textURLPrefix.Text = xmlConf.find(xmlConf.listGen, TagName.URLPrefix)._Value;
      TextURL.Text = xmlConf.find(xmlConf.listSearch, TagName.URL)._Value;
      textRedir.Text = xmlConf.find(xmlConf.listSearch, TagName.URL)._Param1;
      textNextPage.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyNextPage)._Value;
      textStartPage.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartPage)._Value;
      textStepPage.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStepPage)._Value;
      textPage.Text = textStartPage.Text;
      // Load User Settings Page...
      try { cbMaxActors.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsMaxItems)._Value; }
      catch { cbMaxActors.Text = string.Empty; };
      if (string.IsNullOrEmpty(cbMaxActors.Text)) cbMaxActors.Enabled = false;
      else cbMaxActors.Enabled = true;

      string strGrabActorRoles = "";
      string strGrabActorRegex = "";
      try
      {
        strGrabActorRoles = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsGrabActorRoles)._Value;
        strGrabActorRegex = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsRegExp)._Value;
        if (strGrabActorRoles == "true") chkGrabActorRoles.Checked = true;
        else chkGrabActorRoles.Checked = false;
      }
      catch 
      {
        chkGrabActorRoles.Checked = false;
        chkGrabActorRoles.Enabled = false;
      };
      if (string.IsNullOrEmpty(strGrabActorRoles) || string.IsNullOrEmpty(strGrabActorRegex)) chkGrabActorRoles.Enabled = false;
      else chkGrabActorRoles.Enabled = true;

      try { cbMaxProducers.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyProductMaxItems)._Value; }
      catch { cbMaxProducers.Text = string.Empty;};
      if (string.IsNullOrEmpty(cbMaxProducers.Text)) cbMaxProducers.Enabled = false;
      else cbMaxProducers.Enabled = true;

      try { cbMaxDirectors.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseMaxItems)._Value; }
      catch {cbMaxDirectors.Text = string.Empty;};
      if (string.IsNullOrEmpty(cbMaxDirectors.Text)) cbMaxDirectors.Enabled = false;
      else cbMaxDirectors.Enabled = true;

      try { cbMaxWriters.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyWriterMaxItems)._Value; }
      catch {cbMaxWriters.Text = string.Empty;};
      if (string.IsNullOrEmpty(cbMaxWriters.Text)) cbMaxWriters.Enabled = false;
      else cbMaxWriters.Enabled = true;

      try { cbTtitlePreferredLanguage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguage)._Value; }
      catch { cbTtitlePreferredLanguage.Text = string.Empty; };
      //if (string.IsNullOrEmpty(cbTtitlePreferredLanguage.Text)) cbTtitlePreferredLanguage.Enabled = false;
      //else cbTtitlePreferredLanguage.Enabled = true;
      
      try { cbTtitleMaxTitles.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleMaxItems)._Value; }
      catch {cbTtitleMaxTitles.Text = string.Empty;};
      if (string.IsNullOrEmpty(cbTtitleMaxTitles.Text)) cbTtitleMaxTitles.Enabled = false;
      else cbTtitleMaxTitles.Enabled = true;
      
      try { cbCertificationPreferredLanguage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguage)._Value; }
      catch { cbCertificationPreferredLanguage.Text = string.Empty; };
      //if (string.IsNullOrEmpty(cbCertificationPreferredLanguage.Text)) cbCertificationPreferredLanguage.Enabled = false;
      //else cbCertificationPreferredLanguage.Enabled = true;

      // Add Dropdownentries to User Options
      cbTtitlePreferredLanguage.Items.Clear();
      string strTemp;
      try { strTemp = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguageAll)._Value; }
      catch { strTemp = string.Empty; };
      string[] split = strTemp.Split(new Char[] { ',', ';', '/' }, StringSplitOptions.RemoveEmptyEntries);
      Array.Sort(split);
      foreach (var strDroptext in split)
      {
        if (!cbTtitlePreferredLanguage.Items.Contains(strDroptext.Trim()))
          cbTtitlePreferredLanguage.Items.Add(strDroptext.Trim());
      }
      if (cbTtitlePreferredLanguage.Items.Count > 0) cbTtitlePreferredLanguage.Enabled = true;
      else cbTtitlePreferredLanguage.Enabled = false;
      
      cbCertificationPreferredLanguage.Items.Clear();
      try { strTemp = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguageAll)._Value; }
      catch { strTemp = string.Empty; };
      split = strTemp.Split(new Char[] { ',', ';', '/' }, StringSplitOptions.RemoveEmptyEntries);
      Array.Sort(split);
      foreach (var strDroptext in split)
      {
        if (!cbCertificationPreferredLanguage.Items.Contains(strDroptext.Trim()))
          cbCertificationPreferredLanguage.Items.Add(strDroptext.Trim());
      }
      if (cbCertificationPreferredLanguage.Items.Count > 0) cbCertificationPreferredLanguage.Enabled = true;
      else cbCertificationPreferredLanguage.Enabled = false;

        // Read Mapping Infos
        for (int i = 0; i < 30; i++)
        {
          try
          {
            string val1 = string.Empty, val2 = string.Empty, val3 = string.Empty, val4 = string.Empty, val5 = string.Empty, val6 = string.Empty, val7 = string.Empty;
            val1 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param1;
            val2 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param2;
            val3 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param3;
            val4 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param4;
            val5 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param5;
            val6 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param6;
            val7 = xmlConf.find(xmlConf.listMapping, "Field_" + i)._Param7;

            dataGridViewMapping.Rows[i].Cells[1].Value = val1;
            dataGridViewMapping.Rows[i].Cells[2].Value = val2;
            dataGridViewMapping.Rows[i].Cells[3].Value = Convert.ToBoolean(val3);
            dataGridViewMapping.Rows[i].Cells[4].Value = Convert.ToBoolean(val4);
            dataGridViewMapping.Rows[i].Cells[5].Value = Convert.ToBoolean(val5);
            dataGridViewMapping.Rows[i].Cells[6].Value = Convert.ToBoolean(val6);
            dataGridViewMapping.Rows[i].Cells[7].Value = Convert.ToBoolean(val7);
          }
          catch (Exception) {}
        }
    }

    public void SaveXml(string File)
    {
      xmlConf.find(xmlConf.listGen, TagName.DBName)._Value = textName.Text;
      xmlConf.find(xmlConf.listGen, TagName.URLPrefix)._Value = textURLPrefix.Text;
      xmlConf.find(xmlConf.listSearch, TagName.URL)._Value = TextURL.Text;
      xmlConf.find(xmlConf.listSearch, TagName.URL)._Param1 = textRedir.Text;
      xmlConf.find(xmlConf.listSearch, TagName.KeyNextPage)._Value = textNextPage.Text;
      xmlConf.find(xmlConf.listSearch, TagName.KeyStartPage)._Value = textStartPage.Text;
      xmlConf.find(xmlConf.listSearch, TagName.KeyStepPage)._Value = textStepPage.Text;


      XmlTextWriter tw = new XmlTextWriter(File, UTF8Encoding.UTF8);
      tw.Formatting = Formatting.Indented;
      tw.WriteStartDocument(true);
      tw.WriteStartElement("Profile");
      tw.WriteStartElement("Section");

      for (int i = 0; i < xmlConf.listGen.Count; i++)
      {
        tw.WriteStartElement(xmlConf.listGen[i]._Tag);
        tw.WriteString(xmlConf.listGen[i]._Value);
        tw.WriteEndElement();
      }

      tw.WriteStartElement("URLSearch");

      for (int i = 0; i < xmlConf.listSearch.Count; i++)
      {
        tw.WriteStartElement(xmlConf.listSearch[i]._Tag);
        if (xmlConf.listSearch[i]._Tag.StartsWith("KeyStart") || xmlConf.listSearch[i]._Tag.Equals("URL"))
        {
          tw.WriteAttributeString("Param1", XmlConvert.EncodeName(xmlConf.listSearch[i]._Param1));
          tw.WriteAttributeString("Param2", XmlConvert.EncodeName(xmlConf.listSearch[i]._Param2));
        }
        tw.WriteString(XmlConvert.EncodeName(xmlConf.listSearch[i]._Value));
        tw.WriteEndElement();

      }

      tw.WriteEndElement();
      tw.WriteStartElement("Details");

      for (int i = 0; i < xmlConf.listDetail.Count; i++)
      {
        tw.WriteStartElement(xmlConf.listDetail[i]._Tag);
        if (xmlConf.listDetail[i]._Tag.StartsWith("KeyStart"))
        {
          tw.WriteAttributeString("Param1", XmlConvert.EncodeName(xmlConf.listDetail[i]._Param1));
          tw.WriteAttributeString("Param2", XmlConvert.EncodeName(xmlConf.listDetail[i]._Param2));
        }
        tw.WriteString(XmlConvert.EncodeName(xmlConf.listDetail[i]._Value));
        tw.WriteEndElement();
      }
      tw.WriteEndElement();

      // Write Mapping Infos
      tw.WriteStartElement("Mapping");
      for (int i = 0; i < dataGridViewMapping.Rows.Count; i++)
      {
        tw.WriteStartElement("Field_" + i.ToString()); // fieldnumer
        string val1 = string.Empty, val2 = string.Empty, val3 = string.Empty, val4 = string.Empty, val5 = string.Empty, val6 = string.Empty, val7 = string.Empty;
        if (dataGridViewMapping.Rows[i].Cells[1].Value != null) val1 = dataGridViewMapping.Rows[i].Cells[1].Value.ToString(); // source
        if (dataGridViewMapping.Rows[i].Cells[2].Value != null) val2 = dataGridViewMapping.Rows[i].Cells[2].Value.ToString(); // destination
        if (dataGridViewMapping.Rows[i].Cells[3].Value != null) val3 = dataGridViewMapping.Rows[i].Cells[3].Value.ToString(); // replace
        if (dataGridViewMapping.Rows[i].Cells[4].Value != null) val4 = dataGridViewMapping.Rows[i].Cells[4].Value.ToString(); // add before
        if (dataGridViewMapping.Rows[i].Cells[5].Value != null) val5 = dataGridViewMapping.Rows[i].Cells[5].Value.ToString(); // add after
        if (dataGridViewMapping.Rows[i].Cells[6].Value != null) val6 = dataGridViewMapping.Rows[i].Cells[6].Value.ToString(); // merge prefer source
        if (dataGridViewMapping.Rows[i].Cells[7].Value != null) val7 = dataGridViewMapping.Rows[i].Cells[7].Value.ToString(); // merge prefer destination

        tw.WriteAttributeString("source", XmlConvert.EncodeName(val1));
        tw.WriteAttributeString("destination", XmlConvert.EncodeName(val2));
        tw.WriteAttributeString("replace", XmlConvert.EncodeName(val3));
        tw.WriteAttributeString("addstart", XmlConvert.EncodeName(val4));
        tw.WriteAttributeString("addend", XmlConvert.EncodeName(val5));
        tw.WriteAttributeString("mergeprefersource", XmlConvert.EncodeName(val6));
        tw.WriteAttributeString("mergepreferdestination", XmlConvert.EncodeName(val7));
        tw.WriteEndElement();
      }

      tw.WriteEndElement();
      tw.WriteEndElement();
      tw.WriteEndDocument();
      // on ferme ensuite le fichier xml
      tw.Flush();
      // pour finir on va vider le buffer , et on ferme le fichier
      tw.Close();

    }

    private void ResetFormControlValues(Control parent)
    {
      cb_ParamDetail.SelectedIndex = -1;
      cb_Parameter.SelectedIndex = -1;

      foreach (Control c in parent.Controls)
      {
        if (c.Controls.Count > 0)
        {
          ResetFormControlValues(c);
        }
        else
        {
          switch (c.GetType().ToString())
          {
            case "System.Windows.Forms.ComboBox":
              ((ComboBox)c).SelectedIndex = -1;
              break;
            case "System.Windows.Forms.TextBox":
              if (c.Name.ToString() != "TextSearch" && c.Name.ToString() != "textPage")
                ((TextBox)c).Text = "";
              break;
            case "System.Windows.Forms.RichTextBox":
              ((RichTextBox)c).Text = "";
              break;
            case "System.Windows.Forms.ListBox":
              ((ListBox)c).Items.Clear();
              break;
          }
        }
      }

      GLiSearch = 0;
      GLiSearchD = 0;
      Body = string.Empty;
      BodyDetail = string.Empty;

      xmlConf = null;
      listUrl = new ArrayList();
      pictureBoxPreviewCover.ImageLocation = "";

    }



    private void button1_Click_1(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textConfig.Text))
      {
        MessageBox.Show("No Config loaded !", "Error");
        return;
      }
      else
      {
        SaveXml(textConfig.Text + ".tmp");
        Load_Preview(false);
      }
    }

    private void Load_Preview(bool AlwaysAsk)
    {
      listPreview.Items.Clear();
      Grabber.Grabber_URLClass Grab = new Grabber_URLClass();
      Grabber_URLClass.IMDBUrl wurl;
      int pageNumber = -1;
      if (!string.IsNullOrEmpty(textPage.Text)) 
        pageNumber = Convert.ToInt16(textPage.Text);
      try
      {
        listUrl = Grab.ReturnURL(TextSearch.Text, textConfig.Text + ".tmp", pageNumber, AlwaysAsk);
      }
      catch (Exception ex)
      {
        DialogResult dlgResult = DialogResult.None;

        dlgResult = MessageBox.Show("Grabber ERROR - check your definitions! \n\nException Message: " + ex.Message + "\nStacktrace: " + ex.StackTrace, "Error", MessageBoxButtons.OK);
        if (dlgResult == DialogResult.OK)
        {
        }
      }

      for (int i = 0; i < listUrl.Count; i++)
      {
        wurl = (Grabber_URLClass.IMDBUrl)listUrl[i];
        listPreview.Items.Add(wurl.Title + " (" + wurl.Year.ToString() + ") " + wurl.Director);
      }
    }

    private void button_Find_Click(object sender, EventArgs e)
    {

      int i = textBody.Find(textBox5.Text, GLiSearch, RichTextBoxFinds.None);
      if (i > 0)
      {
        textBody.Select(i, textBox5.Text.Length);
        GLiSearch = i + textBox5.Text.Length;
      }
      else
        GLiSearch = 0;
    }

    private void textBody_Click(object sender, EventArgs e)
    {

      GLiSearch = ((RichTextBox)sender).SelectionStart;

    }

    private void button2_Click(object sender, EventArgs e)
    {
      DialogResult dlgResult = DialogResult.None;

      if (textConfig.Text.Length > 0)
      {
        dlgResult = MessageBox.Show("Save current configuration ?", "Save", MessageBoxButtons.YesNoCancel);
        if (dlgResult == DialogResult.Yes)
        {
          SaveXml(textConfig.Text);
          ResetFormControlValues(this);
        }
        if (dlgResult == DialogResult.No)
          ResetFormControlValues(this);

      }

      if ((textConfig.Text.Length == 0) && (dlgResult != DialogResult.Cancel))
      {
        saveFileDialog1.RestoreDirectory = true;
        saveFileDialog1.Filter = "Load xml (*.xml)|*.xml";
        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        {
          textConfig.Text = saveFileDialog1.FileName;
          LoadXml();
        }
      }
    }

    private void button3_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textConfig.Text))
      {
        MessageBox.Show("No Config loaded !", "Error");
        return;
      }
      else
      {
        SaveXml(textConfig.Text);
        MessageBox.Show("Config saved !", "Info");
      }
    }

    private void button_SaveAs_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textConfig.Text))
      {
        MessageBox.Show("No Config loaded !", "Error");
        return;
      }
      else
      {
        if (System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms"))
        {
          saveFileDialog1.InitialDirectory = Config.GetDirectoryInfo(Config.Dir.Config) + @"\scripts\MyFilms";
        }
        else
        {
          saveFileDialog1.RestoreDirectory = true;
        }
        saveFileDialog1.FileName = textConfig.Text;
        saveFileDialog1.Filter = "XML Files (*.xml)|*.xml";
        saveFileDialog1.Title = "Save Internet Grabber Script file (xml file)";
        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        {
          textConfig.Text = saveFileDialog1.FileName;
          SaveXml(textConfig.Text);
          MessageBox.Show("Config saved !", "Info");
        }
      }
    }


    private void TextKeyStart_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock == true)
        return;

      int iStart;
      int iEnd;
      switch (cb_Parameter.SelectedIndex)
      {
        case 0:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartList)._Value = TextKeyStart.Text;
          if (TextKeyStart.Text.Length > 0)
          {
            iStart = Body.IndexOf(TextKeyStart.Text);
            //Si la clé de début a été trouvé
            if (iStart > 0)
            {
              //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
              if (TextKeyStop.Text != "")
              {
                iEnd = Body.IndexOf(TextKeyStop.Text, iStart);
              }
              else
                iEnd = Body.Length;

              if (iEnd == -1)
                iEnd = Body.Length;

              //Découpage du body
              iStart += TextKeyStart.Text.Length;
              textBody.Text = Body.Substring(iStart, iEnd - iStart);

            }
            else
              textBody.Text = Body;
          }

          break;
        case 1:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartTitle)._Value = TextKeyStart.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartYear)._Value = TextKeyStart.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartDirector)._Value = TextKeyStart.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartLink)._Value = TextKeyStart.Text;
          break;
        default:
          TextKeyStart.Text = "";
          break;

      }

      if (cb_Parameter.SelectedIndex > 0 && TextKeyStop.Text.Length > 0)
        textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text);
      // ToDo: Also mark Regex selection on Textbody

    }

    private void TextKeyStop_TextChanged(object sender, EventArgs e)
    {
      int iStart;
      int iEnd;
      switch (cb_Parameter.SelectedIndex)
      {
        case 0:
          xmlConf.find(xmlConf.listSearch, TagName.KeyEndList)._Value = TextKeyStop.Text;
          if (TextKeyStart.Text.Length > 0)
          {
            iStart = Body.IndexOf(TextKeyStart.Text);
            //Si la clé de début a été trouvé
            if (iStart > 0)
            {
              //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
              if (TextKeyStop.Text != "")
              {
                iEnd = Body.IndexOf(TextKeyStop.Text, iStart);
              }
              else
                iEnd = Body.Length;

              if (iEnd == -1)
                iEnd = Body.Length;

              //Découpage du body
              iStart += TextKeyStart.Text.Length;

              try { textBody.Text = Body.Substring(iStart, iEnd - iStart); }
              catch { textBody.Text = Body; }
            }
            else
              textBody.Text = Body;
          }
          break;
        case 1:
          xmlConf.find(xmlConf.listSearch, TagName.KeyEndTitle)._Value = TextKeyStop.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listSearch, TagName.KeyEndYear)._Value = TextKeyStop.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listSearch, TagName.KeyEndDirector)._Value = TextKeyStop.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listSearch, TagName.KeyEndLink)._Value = TextKeyStop.Text;
          break;
        default:
          TextKeyStop.Text = "";
          break;

      }

      if (cb_Parameter.SelectedIndex > 0)
        textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text);
    }

    private void textBody_SelectionChanged(object sender, EventArgs e)
    {
      if (GLbBlockSelect == true)
        return;

      int nb = 0;
      int i = 0;
      i = textBody.Find(textBody.SelectedText, 0, RichTextBoxFinds.NoHighlight);
      while (i > 0)
      {
        nb++;
        i = textBody.Find(textBody.SelectedText, i + textBody.SelectedText.Length, RichTextBoxFinds.NoHighlight);
      }
      label9.Text = nb.ToString() + " match found";
    }

    private void textBody_NewSelection(string starttext, string endtext)
    {

      if (textBody.Text.Length > 0 && starttext.Length > 0 && endtext.Length > 0)
      {
        GLbBlockSelect = true;

        int iStart;
        int iEnd;
        iStart = textBody.Find(starttext, 0, RichTextBoxFinds.None) + starttext.Length;
        iEnd = textBody.Find(endtext, iStart, RichTextBoxFinds.None);
        if (iStart == -1)
          iStart = iEnd;
        if (iEnd == -1)
          iEnd = iStart;
        if ((iEnd == -1) && (iStart == -1))
          iStart = iEnd = 0;
        textBody.Select(iStart, iEnd - iStart);

        //if (textReplace.Text.Length > 0 && textReplaceWith.Text.Length > 0)
        //{
        //    textBody.SelectedText = textBody.SelectedText.Replace(textReplace.Text, textReplaceWith.Text);

        //    iStart = textBody.Find(starttext, 0, RichTextBoxFinds.None) + starttext.Length;
        //    iEnd = textBody.Find(endtext, iStart, RichTextBoxFinds.None);
        //    if (iStart == -1)
        //        iStart = 0;
        //    if (iEnd == -1)
        //        iEnd = 0;

        //    textBody.Select(iStart, iEnd - iStart);
        //}

        GLbBlockSelect = false;
        textBody_SelectionChanged(null, null);
      }



    }

    private void GrabConfig_FormClosing(object sender, FormClosingEventArgs e)
    {
      DialogResult dlgResult = DialogResult.None;

      if (textConfig.Text.Length > 0)
      {
        dlgResult = MessageBox.Show("Save current configuration ?", "Save", MessageBoxButtons.YesNoCancel);
        if (dlgResult == DialogResult.Yes)
        {
          SaveXml(textConfig.Text);

        }
        if (dlgResult == DialogResult.Cancel)
          e.Cancel = true;
      }
    }



    /*
     *
     * DETAIL PAGE
     * 
     */

    private void ButtonLoad_Click(object sender, EventArgs e)
    {
      string absoluteUri;
      int iStart;
      int iEnd;
      string strStart = string.Empty;
      string strEnd = string.Empty;
      string strParam1 = string.Empty;
      string strParam2 = string.Empty;
      string strIndex = string.Empty;
      string strPage = string.Empty;
      string strActivePage = string.Empty;

      URLBodyDetail = string.Empty;
      URLBodyDetail2 = string.Empty;
      URLBodyLinkImg = string.Empty;
      URLBodyLinkGeneric1 = string.Empty;
      URLBodyLinkGeneric2 = string.Empty;
      URLBodyLinkPersons = string.Empty;
      URLBodyLinkTitles = string.Empty;
      URLBodyLinkCertification = string.Empty;
      URLBodyLinkComment = string.Empty;
      URLBodyLinkSyn = string.Empty;

      TimeBodyDetail = string.Empty;
      TimeBodyDetail2 = string.Empty;
      TimeBodyLinkImg = string.Empty;
      TimeBodyLinkGeneric1 = string.Empty;
      TimeBodyLinkGeneric2 = string.Empty;
      TimeBodyLinkPersons = string.Empty;
      TimeBodyLinkTitles = string.Empty;
      TimeBodyLinkCertification = string.Empty;
      TimeBodyLinkComment = string.Empty;
      TimeBodyLinkSyn = string.Empty;

      Stopwatch watch = new Stopwatch();

      if (TextURLDetail.Text.Length > 0)
      {
        watch.Reset();
        watch.Start();
        textPreview.ResetText();
        URLBodyDetail = TextURLDetail.Text;
        if (TextURLDetail.Text.ToLower().StartsWith("http"))
          BodyDetail = GrabUtil.GetPage(TextURLDetail.Text, null, out absoluteUri, new CookieContainer());
        else
          BodyDetail = this.ReadFile(TextURLDetail.Text); // Read html page from file !
        if (xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value.Length > 0)
        {
          iStart = BodyDetail.IndexOf(xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value);
          //Si la clé de début a été trouvé
          if (iStart > 0)
          {
            //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
            if (xmlConf.find(xmlConf.listDetail, TagName.KeyEndBody)._Value != "")
            {
              iEnd = BodyDetail.IndexOf(xmlConf.find(xmlConf.listDetail, TagName.KeyEndBody)._Value, iStart);
            }
            else
              iEnd = BodyDetail.Length;

            if (iEnd == -1)
              iEnd = BodyDetail.Length;

            //Découpage du body
            iStart += xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value.Length;
            BodyDetail = BodyDetail.Substring(iStart, iEnd - iStart);
            textBodyDetail.Text = BodyDetail;
          }
          else
            textBodyDetail.Text = BodyDetail;
        }
        else
          textBodyDetail.Text = BodyDetail;

        watch.Stop();
        TimeBodyDetail = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      }

      // Test if there is a page for Secondary Details (like OFDB GW) and load page in BodyDetails2
      try
      {
        watch.Reset();
        watch.Start();
        strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Value;
        strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndDetails2)._Value;
        strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param1;
        strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param2;
        strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Index)._Value;
        strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Page)._Value;

        strActivePage = this.LoadPage(strPage);
        if (strStart.Length > 0)
        {
          string strTemp = string.Empty;
          if (strParam1.Length > 0 && strParam2.Length > 0)
            strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
          else
            strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
          URLBodyDetail2 = strTemp;
          BodyDetail2 = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
        }
        else
          BodyDetail2 = "";
        watch.Stop();
        TimeBodyDetail2 = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      }
      catch
      {
        BodyDetail2 = "";
      }

      // Test if there is a page for Generic 1 Page
      try
      {
        watch.Reset();
        watch.Start();
        strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Value;
        strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric1)._Value;
        strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param1;
        strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param2;
        strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Index)._Value;
        strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Page)._Value;
        strActivePage = this.LoadPage(strPage);
        if (strStart.Length > 0)
        {
          string strTemp = string.Empty;
          if (strParam1.Length > 0 && strParam2.Length > 0)
            strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
          else
            strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
          URLBodyLinkGeneric1 = strTemp;
          BodyLinkGeneric1 = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
        }
        else
          BodyLinkGeneric1 = "";
        watch.Stop();
        TimeBodyLinkGeneric1 = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      }
      catch
      {
        BodyLinkGeneric1 = "";
      }

      // Test if there is a page for Generic 2 Page
      try
      {
        watch.Reset();
        watch.Start();
        strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Value;
        strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric2)._Value;
        strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param1;
        strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param2;
        strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Index)._Value;
        strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Page)._Value;
        strActivePage = this.LoadPage(strPage);
        if (strStart.Length > 0)
        {
          string strTemp = string.Empty;
          if (strParam1.Length > 0 && strParam2.Length > 0)
            strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
          else
            strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
          URLBodyLinkGeneric2 = strTemp;
          BodyLinkGeneric2 = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
        }
        else
          BodyLinkGeneric2 = "";
        watch.Stop();
        TimeBodyLinkGeneric2 = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      }
      catch
      {
        BodyLinkGeneric2 = "";
      }

      // Test if there is a redirection page for Covers and load page in BodyLinkImg
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Value;
      strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkImg)._Value;
      strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param1;
      strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param2;
      strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgIndex)._Value;
      strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgPage)._Value;
      strActivePage = this.LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkImg = strTemp;
        BodyLinkImg = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
      }
      else
        BodyLinkImg = "";
      watch.Stop();
      TimeBodyLinkImg = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";

      // Test if there is a redirection page for Persons and load page in BodyLinkPersons
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Value;
      strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPersons)._Value;
      strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param1;
      strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param2;
      strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsIndex)._Value;
      strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsPage)._Value;
      strActivePage = this.LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkPersons = strTemp;
        BodyLinkPersons = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
      }
      else
        BodyLinkPersons = "";
      watch.Stop();
      TimeBodyLinkPersons = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";

      // Test if there is a redirection page for Titles and load page in BodyLinkTitles
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Value;
      strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkTitles)._Value;
      strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param1;
      strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param2;
      strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesIndex)._Value;
      strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesPage)._Value;
      strActivePage = this.LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkTitles = strTemp;
        BodyLinkTitles = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
      }
      else
        BodyLinkTitles = "";
      watch.Stop();
      TimeBodyLinkTitles = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";

      // Test if there is a redirection page for Certification and load page in BodyLinkCertification
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Value;
      strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkCertification)._Value;
      strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param1;
      strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param2;
      strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationIndex)._Value;
      strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationPage)._Value;
      strActivePage = this.LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkCertification = strTemp;
        BodyLinkCertification = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
      }
      else
        BodyLinkCertification = "";
      watch.Stop();
      TimeBodyLinkCertification = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";

      // Test if there is a redirection page for Synopsis/Description and load page in BodyLinkSyn
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Value;
      strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkSyn)._Value;
      strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param1;
      strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param2;
      strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynIndex)._Value;
      strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynPage)._Value;
      strActivePage = this.LoadPage(strPage);
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkComment = strTemp;
        BodyLinkSyn = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
      }
      else
        BodyLinkSyn = "";
      watch.Stop();
      TimeBodyLinkSyn = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
      
      // Test if there is a redirection page for Comment and load page in BodyLinkComment
      watch.Reset();
      watch.Start();
      strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Value;
      strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkComment)._Value;
      strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param1;
      strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param2;
      strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentIndex)._Value;
      strPage = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentPage)._Value;
      strActivePage = this.LoadPage(strPage); 
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(strActivePage, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(strActivePage, strStart, strEnd).Trim();
        URLBodyLinkSyn = strTemp;
        BodyLinkComment = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
      }
      else
        BodyLinkComment = "";
      watch.Stop();
      TimeBodyLinkComment = " (" + (watch.ElapsedMilliseconds).ToString() + " ms)";
    }


    private string LoadPage(string Page)
    {
      string strActivePage = string.Empty;
      switch (Page)
      {
        case "URL Gateway":
          strActivePage = BodyDetail2;
          break;
        case "URL Redirection Generic 1":
          strActivePage = BodyLinkGeneric1;
          break;
        case "URL Redirection Generic 2":
          strActivePage = BodyLinkGeneric2;
          break;
        case "URL Redirection Cover":
          strActivePage = BodyLinkImg;
          break;
        case "URL Redirection Persons":
          strActivePage = BodyLinkPersons;
          break;
        case "URL Redirection Title":
          strActivePage = BodyLinkTitles;
          break;
        case "URL Redirection Certification":
          strActivePage = BodyLinkCertification;
          break;
        case "URL Redirection Comment":
          strActivePage = BodyLinkComment;
          break;
        case "URL Redirection Description":
          strActivePage = BodyLinkSyn;
          break;
        default:
          strActivePage = BodyDetail;
          break;
      }
      return strActivePage;
    }
    
    private void textBodyDetail_NewSelection(string starttext, string endtext, int bodystart, string param1)
    {

      if (textBodyDetail.Text.Length > 0 && starttext.Length > 0 && endtext.Length > 0)
      {
        GLbBlockSelect = true;

        int iStart = 0;
        int iEnd = 0;
        int iLength = 0;

        string strTemp = String.Empty;
        HTMLUtil htmlUtil = new HTMLUtil();
        bool bregexs = false;
        bool bregexe = false;
        if (starttext.StartsWith("#REGEX#"))
          bregexs = true;
        if (endtext.StartsWith("#REGEX#"))
          bregexe = true;

        if (starttext != "" && endtext != "")
        {
          iLength = starttext.Length;
          if (param1.StartsWith("#REVERSE#"))
          {
            if (bregexs) iStart = GrabUtil.FindRegEx(textBodyDetail.Text, starttext, iStart, ref iLength, false);
            else iStart = textBodyDetail.Text.LastIndexOf(starttext);
          }
          else if (bregexs) iStart = GrabUtil.FindRegEx(textBodyDetail.Text, starttext, iStart, ref iLength, true);
          else iStart = textBodyDetail.Text.IndexOf(starttext);

          if (iStart > 0)
          {
            if (param1.StartsWith("#REVERSE#"))
            {
              iLength = endtext.Length;
              if (bregexe) iEnd = GrabUtil.FindRegEx(textBodyDetail.Text, endtext, iStart, ref iLength, false) + iStart;
              else iEnd = textBodyDetail.Text.LastIndexOf(endtext, iStart);
            }
            else
            {
              iStart += iLength;
              if (bregexe) iEnd = GrabUtil.FindRegEx(textBodyDetail.Text, endtext, iStart, ref iLength, true) + iStart;
              else iEnd = textBodyDetail.Text.IndexOf(endtext, iStart);
            }
          }
        }
        // Old method (not using regex)
        //try
        //{
        //  iStart = textBodyDetail.Text.IndexOf(starttext, bodystart) + starttext.Length;
        //  iEnd = textBodyDetail.Find(endtext, iStart, RichTextBoxFinds.None);
        //}
        //catch
        //{
        //  MessageBox.Show("Cannot find searchtext with given parameter, please change !", "Error");
        //}
        if (iStart == -1)
          iStart = 0;
        if (iEnd == -1)
          iEnd = 0;

        textBodyDetail.Select(iStart, iEnd - iStart);

        //if (textDReplace.Text.Length > 0 && textDReplaceWith.Text.Length > 0)
        //{
        //    textBodyDetail.SelectedText = textBodyDetail.SelectedText.Replace(textDReplace.Text, textDReplaceWith.Text);

        //    iStart = textBodyDetail.Text.IndexOf(starttext, bodystart) + starttext.Length;
        //    iEnd = textBodyDetail.Find(endtext, iStart, RichTextBoxFinds.None);
        //    if (iStart == -1)
        //        iStart = 0;
        //    if (iEnd == -1)
        //        iEnd = 0;

        //    textBodyDetail.Select(iStart, iEnd - iStart);
        //}

        GLbBlockSelect = false;
        textBodyDetail_SelectionChanged(null, null);
      }
    }

    private void cbParamDetail_SelectedIndexChanged(object sender, EventArgs e)
    {
      GLbBlock = true;

      textComplement.Clear();
      textMaxItems.Clear();
      textLanguages.Clear();
      textLanguagesAll.Clear();
      lblComplement.Visible = false;
      lblMaxItems.Visible = false;
      lblLanguages.Visible = false;
      lblLanguagesAll.Visible = false;
      textComplement.Visible = false;
      textMaxItems.Visible = false;
      textLanguages.Visible = false;
      textLanguagesAll.Visible = false;
      buttonPrevParamDetail.Visible = true;
      //lblComplement.Text = "Complement";
      if (!textBodyDetail.Text.Equals(BodyDetail))
        textBodyDetail.Text = BodyDetail;

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 0: // Start/end Page
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndBody)._Value;
          buttonPrevParamDetail.Visible = false;
          break;
        case 1: // Original Title
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyOTitlePage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndOTitle)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyOTitleIndex)._Value;
          break;
        case 2: // Translated Title
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitlePage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkTitles)) textBodyDetail.Text = BodyLinkTitles;
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguage)._Value; }
          catch { textLanguages.Text = string.Empty; };
          try { textLanguagesAll.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguageAll)._Value; }
          catch { textLanguagesAll.Text = string.Empty; };
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndTTitle)._Value;
          lblLanguages.Visible = true;
          lblLanguagesAll.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          textLanguagesAll.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleRegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleIndex)._Value;
          break;
        case 3: // Coverimage
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyImgPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkImg)) textBodyDetail.Text = BodyLinkImg;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndImg)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyImgIndex)._Value;
          break;
        case 4: // Rating 1
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRatePage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndRate)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "Base Rating";
          textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.BaseRating)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRateIndex)._Value;
          break;
        case 5: // Rating 2
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRate2Page)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndRate2)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "Base Rating";
          textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.BaseRating)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRate2Index)._Value;
          break;
        case 6: // Director
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRealisePage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndRealise)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseRegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseIndex)._Value;
          break;
        case 7: // Producer
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyProductPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndProduct)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyProductRegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyProductMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyProductIndex)._Value;
          break;
        case 8: // Writer
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyWriterPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndWriter)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyWriterRegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyWriterMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyWriterIndex)._Value;
          break;
        case 9: // Actors (Credits)
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkPersons)) textBodyDetail.Text = BodyLinkPersons;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndCredits)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblMaxItems.Visible = true;
          textMaxItems.Visible = true;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsRegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsMaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; };
          string strActorRoles = string.Empty;
          try 
          {
            strActorRoles = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsGrabActorRoles)._Value;
            if (strActorRoles == "true") chkACTORROLES.Checked = true;
            else chkACTORROLES.Checked = false;
          }
          catch {chkACTORROLES.Checked = false;};
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsIndex)._Value;
          break;
        case 10: // Country
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCountryPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndCountry)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCountryRegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCountryIndex)._Value;
          break;
        case 11: // Categories / Genre
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGenrePage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndGenre)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGenreRegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGenreIndex)._Value;
          break;
        case 12: // Year
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyYearPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndYear)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyYearIndex)._Value;
          break;
        case 13: // IMDB_Id
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_IdPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndIMDB_Id)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_IdIndex)._Value;
          break;
        case 14: // Description
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeySynPage)._Value;
          //if (!textBodyDetail.Text.Equals(BodyLinkSyn)) textBodyDetail.Text = BodyLinkSyn;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndSyn)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeySynIndex)._Value;
          break;
        case 15: // Comment
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCommentPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkComment)) textBodyDetail.Text = BodyLinkComment;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndComment)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCommentRegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCommentIndex)._Value;
          break;
        case 16: // Language
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLanguagePage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLanguage)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLanguageIndex)._Value;
          break;
        case 17: // Tagline
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTaglinePage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndTagline)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTaglineIndex)._Value;
          break;
        case 18: // Certification
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationPage)._Value;
          // if (!textBodyDetail.Text.Equals(BodyLinkCertification)) textBodyDetail.Text = BodyLinkCertification;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndCertification)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          lblLanguages.Visible = true;
          lblLanguagesAll.Visible = true;
          textLanguages.Visible = true;
          textLanguagesAll.Visible = true;
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguage)._Value; }
          catch { textLanguages.Text = string.Empty; };
          try { textLanguagesAll.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguageAll)._Value; }
          catch { textLanguagesAll.Text = string.Empty; };
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationRegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationIndex)._Value;
          break;
        case 19: // Studio
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStudioPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndStudio)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStudioIndex)._Value;
          break;
        case 20: // Edition
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEditionPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndEdition)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEditionIndex)._Value;
          break;
        case 21: // IMDB_Rank
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_RankPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndIMDB_Rank)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_RankIndex)._Value;
          break;
        case 22: // TMDB_Id
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTMDB_IdPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndTMDB_Id)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTMDB_IdIndex)._Value;
          break;
        case 23: // Generic Field 1
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Page)._Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric1)._Value;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1RegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Language)._Value; }
          catch { textLanguages.Text = string.Empty; };
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1MaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Index)._Value;
          break;
        case 24: // Generic Field 2
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Page)._Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric2)._Value;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2RegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Language)._Value; }
          catch { textLanguages.Text = string.Empty; };
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2MaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Index)._Value;
          break;
        case 25: // Generic Field 3
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Page)._Value;
          lblLanguages.Visible = true;
          lblComplement.Visible = true;
          lblMaxItems.Visible = true;
          textComplement.Visible = true;
          textMaxItems.Visible = true;
          textLanguages.Visible = true;
          lblComplement.Text = "RegExp";
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric3)._Value;
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3RegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          try { textLanguages.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Language)._Value; }
          catch { textLanguages.Text = string.Empty; };
          try { textMaxItems.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3MaxItems)._Value; }
          catch { textMaxItems.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Index)._Value;
          break;
        case 26: // Link Secondary Details Base Page
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Page)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndDetails2)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Index)._Value;
          break;
        case 27: // Link Generic 1
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Page)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric1)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Index)._Value;
          break;
        case 28: // Link Generic 2
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Page)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric2)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Index)._Value;
          break;
        case 29: // Link Coverart-Secondary Page
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkImg)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgIndex)._Value;
          break;
        case 30: // Link Persons Page
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPersons)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsIndex)._Value;
          break;
        case 31: // Link Titles-Secondary Page
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkTitles)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesIndex)._Value;
          break;
        case 32: // Link Certification-Secondary Page
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkCertification)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationIndex)._Value;
          break;
        case 33: // Link Comment-Secondary Page
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkComment)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentIndex)._Value;
          break;
        case 34: // Link Synopsis/Description-Secondary Page
          URLpage.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynPage)._Value;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkSyn)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynIndex)._Value;
          break;

        default:
          URLpage.Text = "";
          textDReplace.Text = "";
          textDReplaceWith.Text = "";
          TextKeyStartD.Text = "";
          TextKeyStopD.Text = "";
          Index.Text = "";
          break;

      }

      if (cb_ParamDetail.SelectedIndex > 0)
      {
        textDReplace.Visible = true;
        textDReplaceWith.Visible = true;
        labelDReplace.Visible = true;
        labelDReplaceWith.Visible = true;
        //btResetDetail.Visible = true;
      }
      else
      {
        textDReplace.Text = "";
        textDReplaceWith.Text = "";
        textDReplace.Visible = false;
        textDReplaceWith.Visible = false;
        labelDReplace.Visible = false;
        labelDReplaceWith.Visible = false;
        //btResetDetail.Visible = false;
      }

      // load selected Page into webpage window
      textBodyDetail.Text = this.LoadPage(URLpage.Text);

      // Mark Selection, if it's valid
      if (cb_ParamDetail.SelectedIndex > 0 && TextKeyStopD.Text.Length > 0)
        textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text), textDReplace.Text); // Added textDReplace = param1

      GLbBlock = false;
    }

    private void textKeyStartD_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      int iStart;
      int iEnd;
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 0:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value = TextKeyStartD.Text;
          if (TextKeyStartD.Text.Length > 0)
          {
            iStart = BodyDetail.IndexOf(TextKeyStartD.Text);
            //If the key was found early
            if (iStart > 0)
            {
              //If a key purpose has been set, it is used if no one takes the rest of the body
              if (TextKeyStopD.Text != "")
              {
                iEnd = BodyDetail.IndexOf(TextKeyStopD.Text, iStart);
              }
              else
                iEnd = BodyDetail.Length;

              if (iEnd == -1)
                iEnd = BodyDetail.Length;

              //Cutting the Body
              iStart += TextKeyStartD.Text.Length;
              textBodyDetail.Text = BodyDetail.Substring(iStart, iEnd - iStart);

            }
            else
              textBodyDetail.Text = BodyDetail;
          }

          break;
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Value = TextKeyStartD.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Value = TextKeyStartD.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Value = TextKeyStartD.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Value = TextKeyStartD.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Value = TextKeyStartD.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Value = TextKeyStartD.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Value = TextKeyStartD.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Value = TextKeyStartD.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Value = TextKeyStartD.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Value = TextKeyStartD.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Value = TextKeyStartD.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Value = TextKeyStartD.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Value = TextKeyStartD.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Value = TextKeyStartD.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Value = TextKeyStartD.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Value = TextKeyStartD.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Value = TextKeyStartD.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Value = TextKeyStartD.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Value = TextKeyStartD.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Value = TextKeyStartD.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Value = TextKeyStartD.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Value = TextKeyStartD.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Value = TextKeyStartD.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Value = TextKeyStartD.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Value = TextKeyStartD.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Value = TextKeyStartD.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Value = TextKeyStartD.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Value = TextKeyStartD.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Value = TextKeyStartD.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Value = TextKeyStartD.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Value = TextKeyStartD.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Value = TextKeyStartD.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Value = TextKeyStartD.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Value = TextKeyStartD.Text;
          break;
        default:
          TextKeyStartD.Text = "";
          break;

      }

      if (cb_ParamDetail.SelectedIndex > 0 && TextKeyStopD.Text.Length > 0)
        textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text), textDReplace.Text); // Added textDReplace = param1
    }

    private void TextKeyStopD_TextChanged(object sender, EventArgs e)
    {
      int iStart;
      int iEnd;
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 0:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value = TextKeyStartD.Text;
          if (TextKeyStartD.Text.Length > 0)
          {
            iStart = BodyDetail.IndexOf(TextKeyStartD.Text);
            //Si la clé de début a été trouvé
            if (iStart > 0)
            {
              //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
              if (TextKeyStopD.Text != "")
              {
                iEnd = BodyDetail.IndexOf(TextKeyStopD.Text, iStart);
              }
              else
                iEnd = BodyDetail.Length;

              if (iEnd == -1)
                iEnd = BodyDetail.Length;

              //Découpage du body
              iStart += TextKeyStartD.Text.Length;
              BodyDetail = BodyDetail.Substring(iStart, iEnd - iStart);
              textBodyDetail.Text = BodyDetail;

            }
            else
              textBodyDetail.Text = BodyDetail;
          }

          break;
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndOTitle)._Value = TextKeyStopD.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndTTitle)._Value = TextKeyStopD.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndImg)._Value = TextKeyStopD.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndRate)._Value = TextKeyStopD.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndRate2)._Value = TextKeyStopD.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndRealise)._Value = TextKeyStopD.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndProduct)._Value = TextKeyStopD.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndWriter)._Value = TextKeyStopD.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndCredits)._Value = TextKeyStopD.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndCountry)._Value = TextKeyStopD.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndGenre)._Value = TextKeyStopD.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndYear)._Value = TextKeyStopD.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndIMDB_Id)._Value = TextKeyStopD.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndSyn)._Value = TextKeyStopD.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndComment)._Value = TextKeyStopD.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLanguage)._Value = TextKeyStopD.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndTagline)._Value = TextKeyStopD.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndCertification)._Value = TextKeyStopD.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndStudio)._Value = TextKeyStopD.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndEdition)._Value = TextKeyStopD.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndIMDB_Rank)._Value = TextKeyStopD.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndTMDB_Id)._Value = TextKeyStopD.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric1)._Value = TextKeyStopD.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric2)._Value = TextKeyStopD.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndGeneric3)._Value = TextKeyStopD.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndDetails2)._Value = TextKeyStopD.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric1)._Value = TextKeyStopD.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkGeneric2)._Value = TextKeyStopD.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkImg)._Value = TextKeyStopD.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkPersons)._Value = TextKeyStopD.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkTitles)._Value = TextKeyStopD.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkCertification)._Value = TextKeyStopD.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkComment)._Value = TextKeyStopD.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkSyn)._Value = TextKeyStopD.Text;
          break;
        default:
          TextKeyStopD.Text = "";
          break;

      }

      if (cb_ParamDetail.SelectedIndex > 0)
      {
        textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text), textDReplace.Text);
      }
    }

    private void buttonFind_Click(object sender, EventArgs e)
    {
      int i = textBodyDetail.Find(textFind.Text, GLiSearchD, RichTextBoxFinds.None);
      if (i > 0)
      {
        textBodyDetail.Select(i, textFind.Text.Length);
        GLiSearchD = i + textFind.Text.Length;
      }
      else
        GLiSearchD = 0;
    }

    private void textBodyDetail_Click(object sender, EventArgs e)
    {
      GLiSearchD = ((RichTextBox)sender).SelectionStart;
    }

    private void textBodyDetail_SelectionChanged(object sender, EventArgs e)
    {
      if (GLbBlockSelect == true)
        return;

      if (textBodyDetail.SelectedText.Trim().Length > 0)
      {
        int nb = 0;
        int i = 0;
        i = textBodyDetail.Find(textBodyDetail.SelectedText, 0, RichTextBoxFinds.NoHighlight);
        while (i > 0)
        {
          nb++;
          i = textBodyDetail.Find(textBodyDetail.SelectedText, i + textBodyDetail.SelectedText.Length, RichTextBoxFinds.NoHighlight);
        }
        label10.Text = nb.ToString() + " match found";
      }

    }

    private void listPreview_SelectedIndexChanged(object sender, EventArgs e)
    {
      if ((listPreview.SelectedIndex >= 0) && (listPreview.SelectedItem.ToString() == "+++"))
        button_GoDetailPage.Text = "Display Next Page";
      else
        if ((listPreview.SelectedIndex >= 0) && (listPreview.SelectedItem.ToString() == "---"))
          button_GoDetailPage.Text = "Display Previous Page";
        else
          button_GoDetailPage.Text = "Use with Detail Page";
      if (listPreview.SelectedIndex >= 0)
        button_GoDetailPage.Enabled = true;
      else
        button_GoDetailPage.Enabled = false;
    }

    private void button4_Click(object sender, EventArgs e)
    {
      if (listPreview.SelectedIndex >= 0)
      {
        if (listPreview.SelectedItem.ToString() == "+++")
        {
          textPage.Text = Convert.ToString(Convert.ToInt16(textPage.Text) + Convert.ToInt16(textStepPage.Text));
          Grabber_URLClass.IMDBUrl wurl;
          wurl = (Grabber_URLClass.IMDBUrl)listUrl[listPreview.SelectedIndex];
          Load_Preview(false);
          button_GoDetailPage.Enabled = false;
        }
        else
        {
          if (listPreview.SelectedItem.ToString() == "---")
          {
            textPage.Text = Convert.ToString(Convert.ToInt16(textPage.Text) - Convert.ToInt16(textStepPage.Text));
            Grabber_URLClass.IMDBUrl wurl;
            wurl = (Grabber_URLClass.IMDBUrl)listUrl[listPreview.SelectedIndex];
            Load_Preview(false);
            button_GoDetailPage.Enabled = false;
          }
          else
          {
            System.IO.File.Delete(textConfig.Text + ".tmp");
            Grabber_URLClass.IMDBUrl wurl;
            wurl = (Grabber_URLClass.IMDBUrl)listUrl[listPreview.SelectedIndex];
            TextURLDetail.Text = wurl.URL;
            EventArgs ea = new EventArgs();
            ButtonLoad_Click(Button_Load_URL, ea);
            tabControl1.SelectTab(2);
          }
        }
      }
    }

    private void buttonPreview_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textConfig.Text))
      {
        MessageBox.Show("No Config loaded !", "Error");
        return;
      }
      
      Stopwatch watch = new Stopwatch();
      string totalruntime = string.Empty;
      watch.Reset();
      watch.Start();
      textPreview.Clear();
      pictureBoxPreviewCover.ImageLocation = "";
      labelImageSize.Text = "";

      SaveXml(textConfig.Text + ".tmp");
      Grabber.Grabber_URLClass Grab = new Grabber_URLClass();
      string[] Result = new string[60];

      try // http://akas.imdb.com/title/tt0133093/
      {
        Result = Grab.GetDetail(TextURLDetail.Text, Environment.GetEnvironmentVariable("TEMP"), textConfig.Text + ".tmp", true);
      }
      catch (Exception ex)
      {
        DialogResult dlgResult = DialogResult.None;
        dlgResult = MessageBox.Show("An error ocurred - check your definitions!\n Exception: " + ex.ToString() + ", Stacktrace: " + ex.StackTrace.ToString(), "Error", MessageBoxButtons.OK);
        if (dlgResult == DialogResult.OK)
        {
        }
      }
      watch.Stop();
      totalruntime = "Total Runtime: " + (watch.ElapsedMilliseconds).ToString() + " ms.";

      string mapped;
      for (int i = 0; i < Result.Length; i++)
      {
        textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline);
        if (i > 29) mapped = " (mapped)";
        else mapped = "";
 
        switch (i)
        {
          case 0:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Original Title" + mapped + Environment.NewLine;
            break;
          case 30:
            textPreview.SelectedText += Environment.NewLine;
            textPreview.SelectedText += Environment.NewLine;
            textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline); 
            textPreview.SelectedText += "MAPPED OUTPUT FIELDS:" + Environment.NewLine;
            textPreview.SelectedText += Environment.NewLine;
            textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline);
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Original Title" + mapped + Environment.NewLine;
            break;
          case 1:
          case 31:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Translated Title" + mapped + Environment.NewLine;
            break;
          case 2:
          case 32:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Cover" + mapped + Environment.NewLine;
            try
            {
              pictureBoxPreviewCover.ImageLocation = Result[i];
            }
            catch
            {
            }
            try
            {
              // Create new FileInfo object and get the Length.
              FileInfo f = new FileInfo(Result[i]);
              long s1 = f.Length;
              labelImageSize.Text = s1.ToString();
            }
            catch
            {
            }
            break;
          case 3:
          case 33:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Description" + mapped + Environment.NewLine;
            break;
          case 4:
          case 34:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Rating" + mapped + Environment.NewLine;
            break;
          case 5:
          case 35:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Actors" + mapped + Environment.NewLine;
            break;
          case 6:
          case 36:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Director" + mapped + Environment.NewLine;
            break;
          case 7:
          case 37:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Producer" + mapped + Environment.NewLine;
            break;
          case 8:
          case 38:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Year" + mapped + Environment.NewLine;
            break;
          case 9:
          case 39:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Country" + mapped + Environment.NewLine;
            break;
          case 10:
          case 40:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Genre" + mapped + Environment.NewLine;
            break;
          case 11:
          case 41:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "URL" + mapped + Environment.NewLine;
            break;
          case 12:
          case 42:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Image" + mapped + Environment.NewLine;
            break;
          case 13:
          case 43:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Writer" + mapped + Environment.NewLine;
            break;
          case 14:
          case 44:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Comment" + mapped + Environment.NewLine;
            break;
          case 15:
          case 45:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Language" + mapped + Environment.NewLine;
            break;
          case 16:
          case 46:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Tagline" + mapped + Environment.NewLine;
            break;
          case 17:
          case 47:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Certification" + mapped + Environment.NewLine;
            break;
          case 18:
          case 48:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "IMDB_Id" + mapped + Environment.NewLine;
            break;
          case 19:
          case 49:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "IMDBrank" + mapped + Environment.NewLine;
            break;
          case 20:
          case 50:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Studio" + mapped + Environment.NewLine;
            break;
          case 21:
          case 51:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Edition" + mapped + Environment.NewLine;
            break;
          case 22:
          case 52:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Fanart" + mapped + Environment.NewLine;
            break;
          case 23:
          case 53:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Generic 1" + mapped + Environment.NewLine;
            break;
          case 24:
          case 54:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Generic 2" + mapped + Environment.NewLine;
            break;
          case 25:
          case 55:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Generic 3" + mapped + Environment.NewLine;
            break;
          case 26:
          case 56:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Names: Countries for 'Translated Title'" + mapped + Environment.NewLine;
            break;
          case 27:
          case 57:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Values: Countries for 'Translated Title'" + mapped + Environment.NewLine;
            break;
          case 28:
          case 58:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Names: Countries for 'Certification'" + mapped + Environment.NewLine;
            break;
          case 29:
          case 59:
            textPreview.SelectedText += "(" + i.ToString() + ") " + "Values: Countries for 'Certification'" + mapped + Environment.NewLine;
            break;
          default:
            textPreview.SelectedText += "(" + (i).ToString() + ") " + "Mapping Output Field '" + (i - 30).ToString() + "'" + mapped + Environment.NewLine;
            break;

        }
        if (i <= 60) // Changed to support new fields...
          textPreview.AppendText(Result[i] + Environment.NewLine);
      }
      // List of Grab Pages used for Grabber results:
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline);
      textPreview.SelectedText += "*** Infos about used Grab Pages ***" + Environment.NewLine;
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("Base Page:" + TimeBodyDetail + Environment.NewLine);
      textPreview.AppendText(URLBodyDetail + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Gateway:" + TimeBodyDetail2 + Environment.NewLine);
      textPreview.AppendText(URLBodyDetail2 + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Generic 1:" + TimeBodyLinkGeneric1 + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkGeneric1 + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Generic 2:" + TimeBodyLinkGeneric2 + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkGeneric2 + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Cover:" + TimeBodyLinkImg + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkImg + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Persons:" + TimeBodyLinkPersons + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkPersons + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Title:" + TimeBodyLinkTitles + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkTitles + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Certification:" + TimeBodyLinkCertification + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkCertification + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Comment:" + TimeBodyLinkComment + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkComment + Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText("URL Redirection Description:" + TimeBodyLinkSyn + Environment.NewLine);
      textPreview.AppendText(URLBodyLinkSyn + Environment.NewLine);

      textPreview.AppendText(Environment.NewLine);
      textPreview.AppendText(Environment.NewLine);
      textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline);
      textPreview.SelectedText += totalruntime + Environment.NewLine;

      System.IO.File.Delete(textConfig.Text + ".tmp");
    }

    private void textComplement_TextChanged(object sender, EventArgs e)
    {
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 4:
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.BaseRating)._Value = textComplement.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleRegExp)._Value = textComplement.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseRegExp)._Value = textComplement.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyProductRegExp)._Value = textComplement.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyWriterRegExp)._Value = textComplement.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsRegExp)._Value = textComplement.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCountryRegExp)._Value = textComplement.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGenreRegExp)._Value = textComplement.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCommentRegExp)._Value = textComplement.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationRegExp)._Value = textComplement.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1RegExp)._Value = textComplement.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2RegExp)._Value = textComplement.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3RegExp)._Value = textComplement.Text;
          break;
        default:
          break;
      }
    }

    private void textMaxItems_TextChanged(object sender, EventArgs e)
    {
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleMaxItems)._Value = textMaxItems.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseMaxItems)._Value = textMaxItems.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyProductMaxItems)._Value = textMaxItems.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyWriterMaxItems)._Value = textMaxItems.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsMaxItems)._Value = textMaxItems.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1MaxItems)._Value = textMaxItems.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1MaxItems)._Value = textMaxItems.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1MaxItems)._Value = textMaxItems.Text;
          break;
        default:
          break;
      }
    }

    private void chkACTORROLES_CheckedChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 9:
          if (chkACTORROLES.Checked)
            xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsGrabActorRoles)._Value = "true";
          else
            xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsGrabActorRoles)._Value = "false";
          break;
        default:
          break;
      }
    }

    private void textLanguages_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguage)._Value = textLanguages.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguage)._Value = textLanguages.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Language)._Value = textLanguages.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Language)._Value = textLanguages.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Language)._Value = textLanguages.Text;
          break;
        default:
          break;
      }
    }

    private void textLanguagesAll_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock)
        return;

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguageAll)._Value = textLanguagesAll.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguageAll)._Value = textLanguagesAll.Text;
          break;
        default:
          break;
      }
    }


    private void radioButtonFR_CheckedChanged(object sender, EventArgs e)
    {
      Application.CurrentCulture = FrenchCulture;
      ApplyCulture(FrenchCulture);
    }

    private void ApplyCulture(CultureInfo culture)
    {
      // Applies culture to current Thread.
      System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

      // Create a resource manager for this Form and determine its fields via reflection.
      ComponentResourceManager resources = new ComponentResourceManager(this.GetType());
      FieldInfo[] fieldInfos = this.GetType().GetFields(BindingFlags.Instance |
          BindingFlags.DeclaredOnly | BindingFlags.NonPublic);

      // Call SuspendLayout for Form and all fields derived from Control, so assignment of 
      //   localized text doesn't change layout immediately.
      this.SuspendLayout();
      for (int index = 0; index < fieldInfos.Length; index++)
      {
        if (fieldInfos[index].FieldType.IsSubclassOf(typeof(Control)))
        {
          fieldInfos[index].FieldType.InvokeMember("SuspendLayout",
              BindingFlags.InvokeMethod, null,
              fieldInfos[index].GetValue(this), null);
        }
      }

      // If available, assign localized text to Form and fields with Text property.
      String text = resources.GetString("$this.Text");
      if (text != null)
        this.Text = text;
      for (int index = 0; index < fieldInfos.Length; index++)
      {
        if (fieldInfos[index].FieldType.GetProperty("Text", typeof(String)) != null)
        {
          text = resources.GetString(fieldInfos[index].Name + ".Text");
          if (text != null)
          {
            fieldInfos[index].FieldType.InvokeMember("Text",
                BindingFlags.SetProperty, null,
                fieldInfos[index].GetValue(this), new object[] { text });
          }
        }
      }

      // Call ResumeLayout for Form and all fields derived from Control to resume layout logic.
      // Call PerformLayout, so layout changes due to assignment of localized text are performed.
      for (int index = 0; index < fieldInfos.Length; index++)
      {
        if (fieldInfos[index].FieldType.IsSubclassOf(typeof(Control)))
        {
          fieldInfos[index].FieldType.InvokeMember("ResumeLayout",
                  BindingFlags.InvokeMethod, null,
                  fieldInfos[index].GetValue(this), new object[] { false });
        }
      }
      this.ResumeLayout(true);

    }

    private void radioButtonEN_CheckedChanged(object sender, EventArgs e)
    {
      Application.CurrentCulture = EnglishCulture;
      ApplyCulture(EnglishCulture);
    }

    private void textDReplace_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock == true)
        return;

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Param1 = textDReplace.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Param1 = textDReplace.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Param1 = textDReplace.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Param1 = textDReplace.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Param1 = textDReplace.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param1 = textDReplace.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param1 = textDReplace.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Param1 = textDReplace.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Param1 = textDReplace.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Param1 = textDReplace.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Param1 = textDReplace.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Param1 = textDReplace.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Param1 = textDReplace.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param1 = textDReplace.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Param1 = textDReplace.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Param1 = textDReplace.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Param1 = textDReplace.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Param1 = textDReplace.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Param1 = textDReplace.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Param1 = textDReplace.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Param1 = textDReplace.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Param1 = textDReplace.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Param1 = textDReplace.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Param1 = textDReplace.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Param1 = textDReplace.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param1 = textDReplace.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param1 = textDReplace.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param1 = textDReplace.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param1 = textDReplace.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param1 = textDReplace.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param1 = textDReplace.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param1 = textDReplace.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param1 = textDReplace.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param1 = textDReplace.Text;
          break;
        default:
          break;

      }
      //if (cbParamDetail.SelectedIndex > 0 && textDReplaceWith.Text.Length > 0)
      //    textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text));

    }

    private void textDReplaceWith_TextChanged(object sender, EventArgs e)
    {
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Param2 = textDReplaceWith.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Param2 = textDReplaceWith.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Param2 = textDReplaceWith.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Param2 = textDReplaceWith.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Param2 = textDReplaceWith.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param2 = textDReplaceWith.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param2 = textDReplaceWith.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Param2 = textDReplaceWith.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Param2 = textDReplaceWith.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Param2 = textDReplaceWith.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Param2 = textDReplaceWith.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Param2 = textDReplaceWith.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Param2 = textDReplaceWith.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param2 = textDReplaceWith.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Param2 = textDReplaceWith.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Param2 = textDReplaceWith.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Param2 = textDReplaceWith.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Param2 = textDReplaceWith.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Param2 = textDReplaceWith.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Param2 = textDReplaceWith.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Param2 = textDReplaceWith.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartTMDB_Id)._Param2 = textDReplaceWith.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Param2 = textDReplaceWith.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Param2 = textDReplaceWith.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Param2 = textDReplaceWith.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartDetails2)._Param2 = textDReplaceWith.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric1)._Param2 = textDReplaceWith.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkGeneric2)._Param2 = textDReplaceWith.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param2 = textDReplaceWith.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkPersons)._Param2 = textDReplaceWith.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Param2 = textDReplaceWith.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkCertification)._Param2 = textDReplaceWith.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkComment)._Param2 = textDReplaceWith.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkSyn)._Param2 = textDReplaceWith.Text;
          break;
        default:
          break;

      }
      //if (cbParamDetail.SelectedIndex > 0)
      //    textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text));
    }

    private void textReplace_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock == true)
        return;

      switch (cb_Parameter.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartTitle)._Param1 = textReplace.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartYear)._Param1 = textReplace.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartDirector)._Param1 = textReplace.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartLink)._Param1 = textReplace.Text;
          break;
        default:
          break;

      }
      //if (comboBox1.SelectedIndex > 0 && textReplaceWith.Text.Length > 0)
      //    textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text);

    }

    private void textReplaceWith_TextChanged(object sender, EventArgs e)
    {
      switch (cb_Parameter.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartTitle)._Param2 = textReplaceWith.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartYear)._Param2 = textReplaceWith.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartDirector)._Param2 = textReplaceWith.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listSearch, TagName.KeyStartLink)._Param2 = textReplaceWith.Text;
          break;
        default:
          break;

      }
      //if (comboBox1.SelectedIndex > 0)
      //    textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text);
    }
    private int ExtractBody(string Body, string ParamStart)
    {
      string strStart = string.Empty;
      switch (ParamStart)
      {
        case "Original Title":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Value;
          break;
        case "Translated Title":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Value;
          break;
        case "URL cover":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Value;
          break;
        case "Rate 1":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate)._Value;
          break;
        case "Rate 2":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRate2)._Value;
          break;
        case "Synopsys":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Value;
          break;
        case "Director":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Value;
          break;
        case "Producer":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Value;
          break;
        case "Actors":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Value;
          break;
        case "Country":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Value;
          break;
        case "Genre":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Value;
          break;
        case "Year":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Value;
          break;
        // New Added
        case "Comment":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartComment)._Value;
          break;
        case "Language":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLanguage)._Value;
          break;
        case "Tagline":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTagline)._Value;
          break;
        case "Certification":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCertification)._Value;
          break;
        case "Writer":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartWriter)._Value;
          break;
        case "Studio":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartStudio)._Value;
          break;
        case "Edition":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartEdition)._Value;
          break;
        case "IMDB_Rank":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Rank)._Value;
          break;
        case "IMDB_Id":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartIMDB_Id)._Value;
          break;
        case "Generic Field 1":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric1)._Value;
          break;
        case "Generic Field 2":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric2)._Value;
          break;
        case "Generic Field 3":
          strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGeneric3)._Value;
          break;
        //case "TitlesURL":
        //  strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkTitles)._Value;
        //  break;
        default:
          break;
      }
      if (strStart.Length > 0)
        return Body.IndexOf(strStart);
      else
        return 0;

    }

    private void Index_SelectedIndexChanged(object sender, EventArgs e)
    {
      switch (cb_ParamDetail.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyOTitleIndex)._Value = Index.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleIndex)._Value = Index.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyImgIndex)._Value = Index.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRateIndex)._Value = Index.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRate2Index)._Value = Index.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseIndex)._Value = Index.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyProductIndex)._Value = Index.Text;
          break;
        case 8: // writer
          xmlConf.find(xmlConf.listDetail, TagName.KeyWriterIndex)._Value = Index.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsIndex)._Value = Index.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCountryIndex)._Value = Index.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGenreIndex)._Value = Index.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyYearIndex)._Value = Index.Text;
          break;
        case 13: // added for IMDB_Id
          xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_IdIndex)._Value = Index.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeySynIndex)._Value = Index.Text;
          break;
        case 15: // added for comments
          xmlConf.find(xmlConf.listDetail, TagName.KeyCommentIndex)._Value = Index.Text;
          break;
        case 16: // added for languages
          xmlConf.find(xmlConf.listDetail, TagName.KeyLanguageIndex)._Value = Index.Text;
          break;
        case 17: // added for tagline
          xmlConf.find(xmlConf.listDetail, TagName.KeyTaglineIndex)._Value = Index.Text;
          break;
        case 18: // added for certification 
          xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationIndex)._Value = Index.Text;
          break;
        case 19: // added for Studio
          xmlConf.find(xmlConf.listDetail, TagName.KeyStudioIndex)._Value = Index.Text;
          break;
        case 20: // added for Edition
          xmlConf.find(xmlConf.listDetail, TagName.KeyEditionIndex)._Value = Index.Text;
          break;
        case 21: // added for IMDB_Rank
          xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_RankIndex)._Value = Index.Text;
          break;
        case 22: // added for TMDB_Id
          xmlConf.find(xmlConf.listDetail, TagName.KeyTMDB_IdIndex)._Value = Index.Text;
          break;
        case 23: // added for Generic Field 1
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Index)._Value = Index.Text;
          break;
        case 24: // added for Generic Field 2
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Index)._Value = Index.Text;
          break;
        case 25: // added for Generic Field 3
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Index)._Value = Index.Text;
          break;
        case 26: // added for details base page 
          xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Index)._Value = Index.Text;
          break;
        case 27: // added for secondary Generic 1 linkpage
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Index)._Value = Index.Text;
          break;
        case 28: // added for secondary Generic 2 linkpage
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Index)._Value = Index.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgIndex)._Value = Index.Text;
          break;
        case 30: // added for secondary persons page
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsIndex)._Value = Index.Text;
          break;
        case 31: // added for secondary titles page 
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesIndex)._Value = Index.Text;
          break;
        case 32: // added for secondary certification 
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationIndex)._Value = Index.Text;
          break;
        case 33: // added for secondary comment 
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentIndex)._Value = Index.Text;
          break;
        case 34: // added for secondary Synopsis/description 
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynIndex)._Value = Index.Text;
          break;
        default:
          break;
      }
    }

    private void URLpage_SelectedIndexChanged(object sender, EventArgs e)
    {
      //Base
      //URL Gateway
      //URL Redirection Generic1
      //URL Redirection Generic2
      //URL Redirection Cover
      //URL Redirection Persons
      //URL Redirection Title
      //URL Redirection Certification
      //URL Redirection Comment
      //URL Redirection Description

      switch (cb_ParamDetail.SelectedIndex)
      {
        case 1:
          xmlConf.find(xmlConf.listDetail, TagName.KeyOTitlePage)._Value = URLpage.Text;
          break;
        case 2:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTTitlePage)._Value = URLpage.Text;
          break;
        case 3:
          xmlConf.find(xmlConf.listDetail, TagName.KeyImgPage)._Value = URLpage.Text;
          break;
        case 4:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRatePage)._Value = URLpage.Text;
          break;
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRate2Page)._Value = URLpage.Text;
          break;
        case 6:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRealisePage)._Value = URLpage.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyProductPage)._Value = URLpage.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyWriterPage)._Value = URLpage.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsPage)._Value = URLpage.Text;
          break;
        case 10:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCountryPage)._Value = URLpage.Text;
          break;
        case 11:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGenrePage)._Value = URLpage.Text;
          break;
        case 12:
          xmlConf.find(xmlConf.listDetail, TagName.KeyYearPage)._Value = URLpage.Text;
          break;
        case 13:
          xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_IdPage)._Value = URLpage.Text;
          break;
        case 14:
          xmlConf.find(xmlConf.listDetail, TagName.KeySynPage)._Value = URLpage.Text;
          break;
        case 15:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCommentPage)._Value = URLpage.Text;
          break;
        case 16:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLanguagePage)._Value = URLpage.Text;
          break;
        case 17:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTaglinePage)._Value = URLpage.Text;
          break;
        case 18:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationPage)._Value = URLpage.Text;
          break;
        case 19:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStudioPage)._Value = URLpage.Text;
          break;
        case 20:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEditionPage)._Value = URLpage.Text;
          break;
        case 21:
          xmlConf.find(xmlConf.listDetail, TagName.KeyIMDB_RankPage)._Value = URLpage.Text;
          break;
        case 22:
          xmlConf.find(xmlConf.listDetail, TagName.KeyTMDB_IdPage)._Value = URLpage.Text;
          break;
        case 23:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric1Page)._Value = URLpage.Text;
          break;
        case 24:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric2Page)._Value = URLpage.Text;
          break;
        case 25:
          xmlConf.find(xmlConf.listDetail, TagName.KeyGeneric3Page)._Value = URLpage.Text;
          break;
        case 26:
          xmlConf.find(xmlConf.listDetail, TagName.KeyDetails2Page)._Value = URLpage.Text;
          break;
        case 27:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric1Page)._Value = URLpage.Text;
          break;
        case 28:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkGeneric2Page)._Value = URLpage.Text;
          break;
        case 29:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgPage)._Value = URLpage.Text;
          break;
        case 30:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkPersonsPage)._Value = URLpage.Text;
          break;
        case 31:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkTitlesPage)._Value = URLpage.Text;
          break;
        case 32:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCertificationPage)._Value = URLpage.Text;
          break;
        case 33:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkCommentPage)._Value = URLpage.Text;
          break;
        case 34:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkSynPage)._Value = URLpage.Text;
          break;
        default:
          break;
      }

      // if (!GLbBlock)
      textBodyDetail.Text = this.LoadPage(URLpage.Text);

      // Mark Selection, if valid
      if (cb_ParamDetail.SelectedIndex > 0 && TextKeyStopD.Text.Length > 0)
        textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text), textDReplace.Text); // Added textDReplace = param1

    }


    private void button5_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textConfig.Text))
      {
        MessageBox.Show("No Config loaded !", "Error");
        return;
      }
      else
      {
        SaveXml(textConfig.Text + ".tmp");
        Load_Preview(true);
      }
    }

    //private void btReset_Click(object sender, EventArgs e)
    //{
    //    textReplace.Text = "";
    //    textReplaceWith.Text = "";
    //    button_Load_Click(null, null);
    //    TextKeyStop_TextChanged(null, null);
    //}

    //private void btResetDetail_Click(object sender, EventArgs e)
    //{
    //    textDReplace.Text = "";
    //    textDReplaceWith.Text = "";
    //    ButtonLoad_Click(null, null);
    //    TextKeyStopD_TextChanged(null, null);
    //}

    private void button6_Click(object sender, EventArgs e)
    {
      pictureBoxPreviewCover.ImageLocation = ""; // clear picture
      labelImageSize.Text = "";

      if (TextKeyStartD.Text.Length > 0 && TextKeyStopD.Text.Length > 0)
      {
        string find;
        string allNames;
        string allRoles;


        if (textDReplace.Text.Length > 0) // if (textComplement.Text.Length > 0)
        //  find = GrabUtil.FindWithAction(textBodyDetail.Text, TextKeyStartD.Text, TextKeyStopD.Text, textDReplace.Text, textDReplaceWith.Text, textComplement.Text, textMaxItems.Text, textLanguages.Text);
          find = GrabUtil.FindWithAction(textBodyDetail.Text, TextKeyStartD.Text, TextKeyStopD.Text, textDReplace.Text, textDReplaceWith.Text, textComplement.Text, textMaxItems.Text, textLanguages.Text, out allNames, out allRoles, chkACTORROLES.Checked);
        else
          find = GrabUtil.Find(textBodyDetail.Text, TextKeyStartD.Text, TextKeyStopD.Text);

        MessageBox.Show(find, "Preview", MessageBoxButtons.OK);
        textURLPreview.Text = find;
        if (find.EndsWith("jpg") || find.EndsWith("png"))
          try
          {
            pictureBoxPreviewCover.ImageLocation = find;
          }
          catch
          {
          }
      }

    }

    private void buttonPrevParam1_Click(object sender, EventArgs e)
    {
      if (TextKeyStart.Text.Length > 0 && TextKeyStop.Text.Length > 0)
      {
        string find;
        if (textReplace.Text.Length > 0 && textReplaceWith.Text.Length > 0)
          find = GrabUtil.FindWithAction(textBody.Text, TextKeyStart.Text, TextKeyStop.Text, textReplace.Text, textReplaceWith.Text);
        else
          find = GrabUtil.Find(textBody.Text, TextKeyStart.Text, TextKeyStop.Text);

        MessageBox.Show(find, "Preview", MessageBoxButtons.OK);
      }

    }

    private void btnLoadPreview_Click(object sender, EventArgs e)
    {
      string absoluteUri;
      int iStart;
      int iEnd;

      if (textURLPreview.Text.Length > 0)
      {

        textPreview.ResetText();

        BodyDetail = GrabUtil.GetPage(textURLPreview.Text, null, out absoluteUri, new CookieContainer());

        if (xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value.Length > 0)
        {
          iStart = BodyDetail.IndexOf(xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value);
          //Si la clé de début a été trouvé
          if (iStart > 0)
          {
            //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
            if (xmlConf.find(xmlConf.listDetail, TagName.KeyEndBody)._Value != "")
            {
              iEnd = BodyDetail.IndexOf(xmlConf.find(xmlConf.listDetail, TagName.KeyEndBody)._Value, iStart);
            }
            else iEnd = BodyDetail.Length;

            if (iEnd == -1) iEnd = BodyDetail.Length;

            //Découpage du body
            iStart += xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value.Length;
            BodyDetail = BodyDetail.Substring(iStart, iEnd - iStart);
            textBodyDetail.Text = BodyDetail;

          }
          else textBodyDetail.Text = BodyDetail;
        }
        else textBodyDetail.Text = BodyDetail;

      }
    }

    private void pictureBoxPreviewCover_Click(object sender, EventArgs e)
    {
      pictureBoxPreviewCover.ImageLocation = "";
      labelImageSize.Text = "";
    }

    private void pictureBoxFranceFlag_Click(object sender, EventArgs e)
    {

    }

    private void pictureBoxUSFlag_Click(object sender, EventArgs e)
    {

    }

    private void textConfig_TextChanged(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textConfig.Text))
      {
        tabPageSearchPage.Enabled = false;
        tabPageDetailPage.Enabled = false;
      }
      else if (ExpertModeOn)
      {
        tabPageSearchPage.Enabled = true;
        tabPageDetailPage.Enabled = true;
      }
    }

    private void button_Load_File_Click(object sender, EventArgs e)
    {
      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.Filter = "Web Files (*.htm)|*.htm";
      openFileDialog1.Title = "Load HTML File";
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        TextURLDetail.Text = openFileDialog1.FileName;
        button_Load_Click(this, e);
      }

    }

    ///<summary>
    /// Liefert den Inhalt der Datei zurück.
    ///</summary>
    ///<param name="sFilename">Dateipfad</param>
    public string ReadFile(String sFilename)
    {
      string sContent = "";

      if (File.Exists(sFilename))
      {
        StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
        sContent = myFile.ReadToEnd();
        myFile.Close();
      }
      return sContent;
    }

    private void linkLabelMFwiki_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start("http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/My_Films");
    }

    private void buttonExpertMode_Click(object sender, EventArgs e)
    {
      if (ExpertModeOn)
      {
        ChangeVisibility(false);
        ExpertModeOn = false;
      }
      else
      {
        ChangeVisibility(true);
        ExpertModeOn = true;
      }
    }
    private void ChangeVisibility(bool visibleForExpert)
    {
      if (visibleForExpert == false)
      {
        buttonExpertMode.Text = "ExpertMode";
        textURLPrefix.Enabled = false;
        textURLPrefix.Visible = false;
        label2.Visible = false;
        textName.Enabled = false;
        tabPageSearchPage.Enabled = false;
        tabPageDetailPage.Enabled = false;
        tabPageSearchPage.Visible = false;
        tabPageDetailPage.Visible = false;
        //tabPageSaveDetails = tabControl1.TabPages[2];
        //tabControl1.TabPages.Remove(tabPageSaveDetails);
        //tabPageSaveMovie = tabControl1.TabPages[1];
        //tabControl1.TabPages.Remove(tabPageSaveMovie);
      }
      else
      {
        buttonExpertMode.Text = "SimpleMode";
        textURLPrefix.Enabled = true;
        textURLPrefix.Visible = true;
        label2.Visible = true;
        textName.Enabled = true;
        tabPageSearchPage.Enabled = true;
        tabPageDetailPage.Enabled = true;
        tabPageSearchPage.Visible = true;
        tabPageDetailPage.Visible = true;
        //tabControl1.TabPages.Add(tabPageSaveMovie);
        //tabControl1.TabPages.Add(tabPageSaveDetails);
      }
    }

    private void cbMaxActors_SelectedIndexChanged(object sender, EventArgs e)
    {
      //List<ListNode> list = xmlConf.listDetail;
      //string value = TagName.KeyCreditsMaxItems;
      //if (xmlConf.listDetail != null && TagName.KeyCreditsMaxItems != null && cbMaxActors.Text != null && cbMaxActors != null)
      if (cbMaxActors.SelectedIndex != -1)
          xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsMaxItems)._Value = cbMaxActors.Text;
    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e)
    {
      if (chkGrabActorRoles.Checked)
      {
        xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsGrabActorRoles)._Value = "true";
      }
      else
      {
        xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsGrabActorRoles)._Value = "false";        
      }
    }

    private void cbMaxProducers_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbMaxProducers.SelectedIndex != -1)
        xmlConf.find(xmlConf.listDetail, TagName.KeyProductMaxItems)._Value = cbMaxProducers.Text;
    }

    private void cbMaxDirectors_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbMaxDirectors.SelectedIndex != -1) 
        xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseMaxItems)._Value = cbMaxDirectors.Text;
    }

    private void cbMaxWriters_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbMaxWriters.SelectedIndex != -1) 
        xmlConf.find(xmlConf.listDetail, TagName.KeyWriterMaxItems)._Value = cbMaxWriters.Text;
    }

    private void cbTtitlePreferredLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
        xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleLanguage)._Value = cbTtitlePreferredLanguage.Text;
    }

    private void cbTtitleMaxTitles_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbTtitleMaxTitles.SelectedIndex != -1) 
        xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleMaxItems)._Value = cbTtitleMaxTitles.Text;
    }

    private void cbCertificationPreferredLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbCertificationPreferredLanguage.SelectedIndex != -1) 
        xmlConf.find(xmlConf.listDetail, TagName.KeyCertificationLanguage)._Value = cbCertificationPreferredLanguage.Text;
    }

    private void InitMappingTable()
    {
      Fields[0] = "OriginalTitle";
      Fields[1] = "TranslatedTitle";
      Fields[2] = "Picture";
      Fields[3] = "Description";
      Fields[4] = "Rating";
      Fields[5] = "Actors";
      Fields[6] = "Director";
      Fields[7] = "Producer";
      Fields[8] = "Year";
      Fields[9] = "Country";
      Fields[10] = "Category";
      Fields[11] = "URL Redirection Picture";
      Fields[12] = "ImageURL";
      Fields[13] = "Writer";
      Fields[14] = "Comment";
      Fields[15] = "Language";
      Fields[16] = "Tagline";
      Fields[17] = "Certification";
      Fields[18] = "IMDB_Id";
      Fields[19] = "IMDBrank";
      Fields[20] = "Studio";
      Fields[21] = "Edition";
      Fields[22] = "Fanart";
      Fields[23] = "Generic1";
      Fields[24] = "Generic2";
      Fields[25] = "Generic3";
      Fields[26] = "TranslatedTitle - All Names";
      Fields[27] = "TranslatedTitle - All Values";
      Fields[28] = "Certification - All Names";
      Fields[29] = "Certification - All Values";

      Column2.Items.Clear();
      Column2.Items.Add(""); // empty field to choose ....
      foreach (string field in Fields)
      {
        if (!string.IsNullOrEmpty(field) && !field.Contains("URL") && !field.Contains("All "))
        Column2.Items.Add(field);
      }
      for (int i = 0; i < 30; i++)
      {
        i = dataGridViewMapping.Rows.Add(); // add row for config
        dataGridViewMapping.Rows[i].Cells[0].Value = i;
        dataGridViewMapping.Rows[i].Cells[1].Value = Fields[i]; // adds field name
        dataGridViewMapping.Rows[i].Cells[2].Value = string.Empty;
        dataGridViewMapping.Rows[i].Cells[3].Value = false;
        dataGridViewMapping.Rows[i].Cells[4].Value = false;
        dataGridViewMapping.Rows[i].Cells[5].Value = false;
        dataGridViewMapping.Rows[i].Cells[6].Value = false;
        dataGridViewMapping.Rows[i].Cells[7].Value = false;
      }

    }

  }
}