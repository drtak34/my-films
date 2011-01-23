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
    private string BodyLinkImg = string.Empty;

    private XmlConf xmlConf;

    private ArrayList listUrl = new ArrayList();

    private CookieContainer cookie = new CookieContainer();

    public GrabConfig()
    {

      System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
      InitializeComponent();

      if (CultureInfo.InstalledUICulture.ToString() == FrenchCulture.ToString())
        radioButtonFR.Checked = true;
      else
        radioButtonEN.Checked = true;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.Filter = "Fichiers xml (*.xml)|*.xml";
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

      switch (comboBox1.SelectedIndex)
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
        case 4:
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

      if (comboBox1.SelectedIndex > 0)
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
      xmlConf = new XmlConf(textConfig.Text);

      textName.Text = xmlConf.find(xmlConf.listGen, TagName.DBName)._Value;
      textURLPrefix.Text = xmlConf.find(xmlConf.listGen, TagName.URLPrefix)._Value;
      TextURL.Text = xmlConf.find(xmlConf.listSearch, TagName.URL)._Value;
      textRedir.Text = xmlConf.find(xmlConf.listSearch, TagName.URL)._Param1;
      textNextPage.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyNextPage)._Value;
      textStartPage.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStartPage)._Value;
      textStepPage.Text = xmlConf.find(xmlConf.listSearch, TagName.KeyStepPage)._Value;
      textPage.Text = textStartPage.Text;
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
      cbParamDetail.SelectedIndex = -1;
      comboBox1.SelectedIndex = -1;

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

    }



    private void button1_Click_1(object sender, EventArgs e)
    {
      SaveXml(textConfig.Text + ".tmp");
      Load_Preview(false);
    }

    private void Load_Preview(bool AlwaysAsk)
    {
      listPreview.Items.Clear();
      Grabber.Grabber_URLClass Grab = new Grabber_URLClass();
      Grabber_URLClass.IMDBUrl wurl;
      try
      {
        listUrl = Grab.ReturnURL(TextSearch.Text, textConfig.Text + ".tmp", Convert.ToInt16(textPage.Text), AlwaysAsk);
      }
      catch
      {
        DialogResult dlgResult = DialogResult.None;

        dlgResult = MessageBox.Show("Matching problem - check your definitions! (Make sure, year does not include non numerical characters)", "Error", MessageBoxButtons.OK);
        if (dlgResult == DialogResult.OK)
        {
        }
      }

      for (int i = 0; i < listUrl.Count; i++)
      {
        wurl = (Grabber_URLClass.IMDBUrl)listUrl[i];
        listPreview.Items.Add(wurl.Title);
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
        saveFileDialog1.Filter = "Fichiers xml (*.xml)|*.xml";
        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        {
          textConfig.Text = saveFileDialog1.FileName;
          LoadXml();
        }
      }
    }

    private void button3_Click(object sender, EventArgs e)
    {
      if (textConfig.Text.Length > 0)
        SaveXml(textConfig.Text);
    }

    private void TextKeyStart_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock == true)
        return;

      int iStart;
      int iEnd;
      switch (comboBox1.SelectedIndex)
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

      if (comboBox1.SelectedIndex > 0 && TextKeyStop.Text.Length > 0)
        textBody_NewSelection(TextKeyStart.Text, TextKeyStop.Text);

    }

    private void TextKeyStop_TextChanged(object sender, EventArgs e)
    {
      int iStart;
      int iEnd;
      switch (comboBox1.SelectedIndex)
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

      if (comboBox1.SelectedIndex > 0)
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

      if (TextURLDetail.Text.Length > 0)
      {

        textPreview.ResetText();

        BodyDetail = GrabUtil.GetPage(TextURLDetail.Text, null, out absoluteUri, new CookieContainer());

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


      }
      // Test if there is a redirection page for Covers and load page in BodyLinkImg
      string strStart = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Value;
      string strEnd = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkImg)._Value;
      string strParam1 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param1;
      string strParam2 = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param2;
      string strIndex = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgIndex)._Value;
      // ***** If Details/KeyStartLinkImg filled => there is a redirection Cover's Page. Get it before searching Cover
      if (strStart.Length > 0)
      {
        string strTemp = string.Empty;
        if (strParam1.Length > 0 && strParam2.Length > 0)
          strTemp = GrabUtil.FindWithAction(BodyDetail, strStart, strEnd, strParam1, strParam2).Trim();
        else
          strTemp = GrabUtil.Find(BodyDetail, strStart, strEnd).Trim();
        BodyLinkImg = GrabUtil.GetPage(strTemp, null, out absoluteUri, new CookieContainer());
      }
      else
        BodyLinkImg = BodyDetail;

    }

    private void textBodyDetail_NewSelection(string starttext, string endtext, int bodystart)
    {

      if (textBodyDetail.Text.Length > 0 && starttext.Length > 0 && endtext.Length > 0)
      {
        GLbBlockSelect = true;
        int iStart = -1;
        int iEnd = -1;
        try
        {
          iStart = textBodyDetail.Text.IndexOf(starttext, bodystart) + starttext.Length;
          iEnd = textBodyDetail.Find(endtext, iStart, RichTextBoxFinds.None);
        }
        catch
        {
          MessageBox.Show("Cannot find searchtext with given parameter, please change !", "Error");
        }

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
      lblComplement.Visible = false;
      textComplement.Visible = false;
      buttonPrevParam.Visible = true;
      //lblComplement.Text = "Complement";
      if (!textBodyDetail.Text.Equals(BodyDetail))
        textBodyDetail.Text = BodyDetail;


      switch (cbParamDetail.SelectedIndex)
      {

        case 0:
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartBody)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndBody)._Value;
          buttonPrevParam.Visible = false;
          break;
        case 1:
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartOTitle)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndOTitle)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyOTitleIndex)._Value;
          break;
        case 2:
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartTTitle)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndTTitle)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyTTitleIndex)._Value;
          break;
        case 3:
          if (!textBodyDetail.Text.Equals(BodyLinkImg))
            textBodyDetail.Text = BodyLinkImg;
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartImg)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndImg)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyImgIndex)._Value;
          break;
        case 4:
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
        case 5:
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
        case 6:
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndSyn)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeySynIndex)._Value;
          break;
        case 7:
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndRealise)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseIndex)._Value;
          break;
        case 8:
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndProduct)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyProductIndex)._Value;
          break;
        case 9:
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCredits)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndCredits)._Value;
          lblComplement.Visible = true;
          textComplement.Visible = true;
          lblComplement.Text = "RegExp";
          try { textComplement.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsRegExp)._Value; }
          catch { textComplement.Text = string.Empty; };
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsIndex)._Value;
          break;
        case 10:
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartCountry)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndCountry)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyCountryIndex)._Value;
          break;
        case 11:
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartGenre)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndGenre)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyGenreIndex)._Value;
          break;
        case 12:
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartYear)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndYear)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyYearIndex)._Value;
          break;
        case 13:
          textDReplace.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param1;
          textDReplaceWith.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param2;
          TextKeyStartD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Value;
          TextKeyStopD.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkImg)._Value;
          Index.Text = xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgIndex)._Value;
          break;
        default:
          textDReplace.Text = "";
          textDReplaceWith.Text = "";
          TextKeyStartD.Text = "";
          TextKeyStopD.Text = "";
          Index.Text = "";
          break;

      }

      if (cbParamDetail.SelectedIndex > 0)
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

      GLbBlock = false;
    }

    private void textKeyStartD_TextChanged(object sender, EventArgs e)
    {
      if (GLbBlock == true)
        return;

      int iStart;
      int iEnd;
      switch (cbParamDetail.SelectedIndex)
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
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Value = TextKeyStartD.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Value = TextKeyStartD.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Value = TextKeyStartD.Text;
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
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Value = TextKeyStartD.Text;
          break;
        default:
          TextKeyStartD.Text = "";
          break;

      }

      if (cbParamDetail.SelectedIndex > 0 && TextKeyStopD.Text.Length > 0)
        textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text));
    }

    private void TextKeyStopD_TextChanged(object sender, EventArgs e)
    {
      int iStart;
      int iEnd;
      switch (cbParamDetail.SelectedIndex)
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
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndSyn)._Value = TextKeyStopD.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndRealise)._Value = TextKeyStopD.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndProduct)._Value = TextKeyStopD.Text;
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
          xmlConf.find(xmlConf.listDetail, TagName.KeyEndLinkImg)._Value = TextKeyStopD.Text;
          break;
        default:
          TextKeyStopD.Text = "";
          break;

      }

      if (cbParamDetail.SelectedIndex > 0)
      {
        textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text));
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
        button4.Text = "Display Next Page";
      else
        if ((listPreview.SelectedIndex >= 0) && (listPreview.SelectedItem.ToString() == "---"))
          button4.Text = "Display Previous Page";
        else
          button4.Text = "Use with Detail Page";
      if (listPreview.SelectedIndex >= 0)
        button4.Enabled = true;
      else
        button4.Enabled = false;
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
          button4.Enabled = false;
        }
        else
        {
          if (listPreview.SelectedItem.ToString() == "---")
          {
            textPage.Text = Convert.ToString(Convert.ToInt16(textPage.Text) - Convert.ToInt16(textStepPage.Text));
            Grabber_URLClass.IMDBUrl wurl;
            wurl = (Grabber_URLClass.IMDBUrl)listUrl[listPreview.SelectedIndex];
            Load_Preview(false);
            button4.Enabled = false;
          }
          else
          {
            System.IO.File.Delete(textConfig.Text + ".tmp");
            Grabber_URLClass.IMDBUrl wurl;
            wurl = (Grabber_URLClass.IMDBUrl)listUrl[listPreview.SelectedIndex];
            TextURLDetail.Text = wurl.URL;
            EventArgs ea = new EventArgs();
            ButtonLoad_Click(ButtonLoad, ea);
            tabControl1.SelectTab(1);
          }
        }
      }
    }

    private void buttonPreview_Click(object sender, EventArgs e)
    {
      textPreview.Clear();
      SaveXml(textConfig.Text + ".tmp");
      Grabber.Grabber_URLClass Grab = new Grabber_URLClass();
      string[] Result = new string[20];

      try
      {
        Result = Grab.GetDetail(TextURLDetail.Text, Environment.GetEnvironmentVariable("TEMP"), textConfig.Text + ".tmp", true);
      }
      catch
      {
        DialogResult dlgResult = DialogResult.None;
        dlgResult = MessageBox.Show("Matching problem - check your definitions! (Make sure, year does not include non numerical characters)", "Error", MessageBoxButtons.OK);
        if (dlgResult == DialogResult.OK)
        {
        }
      }

      for (int i = 0; i < Result.Length; i++)
      {
        textPreview.SelectionFont = new Font("Arial", (float)9.75, FontStyle.Bold | FontStyle.Underline);

        switch (i)
        {
          case 0:
            textPreview.SelectedText += "Original Title" + Environment.NewLine;
            break;
          case 1:
            textPreview.SelectedText += "Translated Title" + Environment.NewLine;
            break;
          case 2:
            textPreview.SelectedText += "Cover" + Environment.NewLine;
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
            textPreview.SelectedText += "Synopsys" + Environment.NewLine;
            break;
          case 4:
            textPreview.SelectedText += "Note" + Environment.NewLine;
            break;
          case 5:
            textPreview.SelectedText += "Actors" + Environment.NewLine;
            break;
          case 6:
            textPreview.SelectedText += "Director" + Environment.NewLine;
            break;
          case 7:
            textPreview.SelectedText += "Producer" + Environment.NewLine;
            break;
          case 8:
            textPreview.SelectedText += "Year" + Environment.NewLine;
            break;
          case 9:
            textPreview.SelectedText += "Country" + Environment.NewLine;
            break;
          case 10:
            textPreview.SelectedText += "Genre" + Environment.NewLine;
            break;

        }
        if (i <= 10)
          textPreview.AppendText(Result[i] + Environment.NewLine);
        if (i == 2)
          textPreview.AppendText(Result[11] + Environment.NewLine);
      }

      System.IO.File.Delete(textConfig.Text + ".tmp");

    }

    private void textComplement_TextChanged(object sender, EventArgs e)
    {
      switch (cbParamDetail.SelectedIndex)
      {
        case 4:
        case 5:
          xmlConf.find(xmlConf.listDetail, TagName.BaseRating)._Value = textComplement.Text;
          break;
        case 9:
          xmlConf.find(xmlConf.listDetail, TagName.KeyCreditsRegExp)._Value = textComplement.Text;
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

      switch (cbParamDetail.SelectedIndex)
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
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param1 = textDReplace.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param1 = textDReplace.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param1 = textDReplace.Text;
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
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param1 = textDReplace.Text;
          break;
        default:
          break;

      }
      //if (cbParamDetail.SelectedIndex > 0 && textDReplaceWith.Text.Length > 0)
      //    textBodyDetail_NewSelection(TextKeyStartD.Text, TextKeyStopD.Text, ExtractBody(textBodyDetail.Text, Index.Text));

    }

    private void textDReplaceWith_TextChanged(object sender, EventArgs e)
    {
      switch (cbParamDetail.SelectedIndex)
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
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartSyn)._Param2 = textDReplaceWith.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartRealise)._Param2 = textDReplaceWith.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartProduct)._Param2 = textDReplaceWith.Text;
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
          xmlConf.find(xmlConf.listDetail, TagName.KeyStartLinkImg)._Param2 = textDReplaceWith.Text;
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

      switch (comboBox1.SelectedIndex)
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
      switch (comboBox1.SelectedIndex)
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
      switch (cbParamDetail.SelectedIndex)
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
          xmlConf.find(xmlConf.listDetail, TagName.KeySynIndex)._Value = Index.Text;
          break;
        case 7:
          xmlConf.find(xmlConf.listDetail, TagName.KeyRealiseIndex)._Value = Index.Text;
          break;
        case 8:
          xmlConf.find(xmlConf.listDetail, TagName.KeyProductIndex)._Value = Index.Text;
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
        case 13:
          xmlConf.find(xmlConf.listDetail, TagName.KeyLinkImgIndex)._Value = Index.Text;
          break;

        default:
          break;
      }
    }

    private void button5_Click(object sender, EventArgs e)
    {
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
      if (TextKeyStartD.Text.Length > 0 && TextKeyStopD.Text.Length > 0)
      {
        string find;
        if (textDReplace.Text.Length > 0)
          find = GrabUtil.FindWithAction(textBodyDetail.Text, TextKeyStartD.Text, TextKeyStopD.Text, textDReplace.Text, textDReplaceWith.Text, textComplement.Text);
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
    }

  }
}