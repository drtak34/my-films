using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyFilmsPlugin.Configuration
{
  using MyFilmsPlugin.DataBase;
  using MyFilmsPlugin.MyFilms.MyFilmsGUI;

  public partial class FilterEditor : Form
  {
    public FilterEditor()
    {
      InitializeComponent();
      AntMovieCatalog ds = new AntMovieCatalog();
      AntFilterItem1.Items.Add("(none)");
      AntFilterItem2.Items.Add("(none)");
      foreach (DataColumn dc in ds.Movie.Columns)
      {
        if ((dc.ColumnName != "Contents_Id" && dc.ColumnName != "Movie_Id" && dc.ColumnName != "IsOnline" &&
             dc.ColumnName != "IsOnlineTrailer" && dc.ColumnName != "LastPosition" && dc.ColumnName != "Picture" &&
             dc.ColumnName != "Fanart") &&
            (ExtendedFields ||
             (dc.ColumnName != "IMDB_Id" && dc.ColumnName != "TMDB_Id" && dc.ColumnName != "Watched" &&
              dc.ColumnName != "Certification" && dc.ColumnName != "Writer" && dc.ColumnName != "SourceTrailer" &&
              dc.ColumnName != "TagLine" && dc.ColumnName != "Tags" && dc.ColumnName != "RatingUser" &&
              dc.ColumnName != "Studio" && dc.ColumnName != "IMDB_Rank" && dc.ColumnName != "Edition" &&
              dc.ColumnName != "Aspectratio" && dc.ColumnName != "CategoryTrakt" && dc.ColumnName != "Favorite")))
        {
          if (dc.ColumnName != "DateAdded" && dc.ColumnName != "RecentlyAdded") // added "DatedAdded" to remove filter
          {
            AntFilterItem1.Items.Add(dc.ColumnName);
            AntFilterItem2.Items.Add(dc.ColumnName);
          }
        }
      }
    }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    private bool extendedFields = true;
    public bool ExtendedFields
    {
      get { return extendedFields; }
      set { extendedFields = value; }
    }

    public string ConfigString
    {
      get { return ExpressionPreview.Text; }
      set { ExpressionPreview.Text = value; }
    }

    private string StrDfltSelect;

    private string masterTitle;
    public string MasterTitle
    {
      get { return masterTitle; }
      set { masterTitle = value; }
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
      AntFilterItem1.ResetText();
      AntFilterItem2.ResetText();
      AntFilterText1.ResetText();
      AntFilterText2.ResetText();
      AntFilterFreeText.ResetText();
      AntFilterSign1.ResetText();
      AntFilterSign2.ResetText();
      AntFilterComb.ResetText();
      ExpressionPreview.ResetText();
    }

    private void AntFilterItem1_SelectedIndexChanged(object sender, EventArgs e)
    {

      if (AntFilterItem1.Text == "(none)")
        AntFilterText1.Clear();
    }

    private void AntFilterItem2_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (AntFilterItem2.Text == "(none)")
        AntFilterText2.Clear();
    }

    private void Selected_Enreg_TextChanged()
    {
      StrDfltSelect = "";
      string wAntFilterSign;
      if (AntFilterSign1.Text == "#")
        wAntFilterSign = "<>";
      else
        wAntFilterSign = AntFilterSign1.Text;
      if ((AntFilterItem1.Text.Length > 0) && !(AntFilterItem1.Text == "(none)"))
        if (AntFilterItem1.Text == "DateAdded")
          if ((AntFilterSign1.Text == "#") || (AntFilterSign1.Text == "not like"))
            StrDfltSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText1.Text) + "# or " + AntFilterItem1.Text + " is null) ";
          else if ((AntFilterSign1.Text == "in") || (AntFilterSign1.Text == "not in"))
            StrDfltSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " (" + DBitemList(AntFilterText1.Text, true) + ")) ";
          else if (AntFilterSign1.Text == "like in")
            StrDfltSelect = "(" + TransformedLikeIn(AntFilterItem1.Text, AntFilterText1.Text, true) + ") ";
          else
            StrDfltSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText1.Text) + "# ) ";
        else
          if ((AntFilterSign1.Text == "#") || (AntFilterSign1.Text == "not like"))
            StrDfltSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " '" + AntFilterText1.Text + "' or " + AntFilterItem1.Text + " is null) ";
          else if ((AntFilterSign1.Text == "in") || (AntFilterSign1.Text == "not in"))
            StrDfltSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " (" + DBitemList(AntFilterText1.Text, false) + ")) ";
          else if (AntFilterSign1.Text == "like in")
            StrDfltSelect = "(" + TransformedLikeIn(AntFilterItem1.Text, AntFilterText1.Text, false) + ") ";
          else
            StrDfltSelect = "(" + AntFilterItem1.Text + " " + wAntFilterSign + " '" + AntFilterText1.Text + "') ";
      if ((AntFilterComb.Text == "or") && (StrDfltSelect.Length > 0))
        StrDfltSelect = StrDfltSelect + " OR ";
      else
        if (StrDfltSelect.Length > 0)
          StrDfltSelect = StrDfltSelect + " AND ";

      if (AntFilterSign2.Text == "#") wAntFilterSign = "<>";
      else wAntFilterSign = AntFilterSign2.Text;

      if ((AntFilterItem2.Text.Length > 0) && !(AntFilterItem2.Text == "(none)"))
        if (AntFilterItem2.Text == "DateAdded")
          if ((AntFilterSign2.Text == "#") || (AntFilterSign2.Text == "not like"))
            StrDfltSelect = "(" + StrDfltSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText2.Text) + "# or " + AntFilterItem2.Text + " is null)) AND ";
          else if ((AntFilterSign2.Text == "in") || (AntFilterSign2.Text == "not in"))
            StrDfltSelect = "(" + AntFilterItem2.Text + " " + wAntFilterSign + " (" + DBitemList(AntFilterText2.Text, true) + ")) AND ";
          else if (AntFilterSign2.Text == "like in")
            StrDfltSelect = "(" + TransformedLikeIn(AntFilterItem2.Text, AntFilterText2.Text, true) + ") AND ";
          else
            StrDfltSelect = "(" + StrDfltSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " #" + Convert.ToDateTime(AntFilterText2.Text) + "# )) AND ";
        else
          if ((AntFilterSign2.Text == "#") || (AntFilterSign2.Text == "not like"))
            StrDfltSelect = "(" + StrDfltSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " '" + AntFilterText2.Text + "' or " + AntFilterItem2.Text + " is null)) AND ";
          else if ((AntFilterSign2.Text == "in") || (AntFilterSign2.Text == "not in"))
            StrDfltSelect = "(" + AntFilterItem2.Text + " " + wAntFilterSign + " (" + DBitemList(AntFilterText2.Text, false) + ")) AND ";
          else if (AntFilterSign2.Text == "like in")
            StrDfltSelect = "(" + TransformedLikeIn(AntFilterItem2.Text, AntFilterText2.Text, true) + ") AND ";
          else
            StrDfltSelect = "(" + StrDfltSelect + "(" + AntFilterItem2.Text + " " + wAntFilterSign + " '" + AntFilterText2.Text + "' )) AND ";
      if (!string.IsNullOrEmpty(AntFilterFreeText.Text))
        StrDfltSelect = StrDfltSelect + AntFilterFreeText.Text + " AND ";
      ExpressionPreview.Text = StrDfltSelect + masterTitle + " not like ''" + " AND";
      LogMyFilms.Debug("MyFilms (Build Selected Enreg) - Selected_Enreg: '" + ExpressionPreview.Text + "'");
      ConfigString = ExpressionPreview.Text;
    }

    private string DBitemList(string inputstring, bool isdate)
    {
      string returnValue = "";
      string[] split = inputstring.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string s in split)
      {
        if (returnValue.Length > 0) returnValue += ", ";
        if (isdate)
          returnValue += "#" + Convert.ToDateTime(s) + "#";
        else
          returnValue += "'" + s + "'";
      }
      return returnValue;
    }

    private string TransformedLikeIn(string dbfield, string inputstring, bool isdate)
    {
      string returnValue = "";
      string[] split = inputstring.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string s in split)
      {
        if (returnValue.Length > 0) returnValue += " OR ";
        if (isdate)
          returnValue += "(" + dbfield + " like #" + Convert.ToDateTime(s) + "#)";
        else
          returnValue += "(" + dbfield + " like '*" + s + "*')";
      }
      return returnValue;
    }

    private void btnPreview_Click(object sender, EventArgs e)
    {
      Selected_Enreg_TextChanged();
      CheckExpression();
    }

    private void CheckExpression()
    {
      if (AntFilterItem1.Text != "(none)" && AntFilterSign1.Text.Length == 0)
      {
        MessageBox.Show("Symbol for Filter comparison must be '=' or '#'", "Filter Editor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        AntFilterSign1.Focus();
        return;
      }
      if (AntFilterItem1.Text != "(none)" && AntFilterText1.Text.Length == 0)
      {
        MessageBox.Show("Length of Filter Text Item must be > 0", "Filter Editor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        AntFilterText1.Focus();
        return;
      }
      if (AntFilterItem2.Text.Length == 0)
        AntFilterItem2.Text = "(none)";
      if (AntFilterItem2.Text != "(none)" && AntFilterSign2.Text.Length == 0)
      {
        MessageBox.Show("Symbol for Filter comparison must be '=' or '#'", "Filter Editor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        AntFilterSign2.Focus();
        return;
      }
      if (AntFilterItem2.Text != "(none)" && AntFilterText2.Text.Length == 0)
      {
        MessageBox.Show("Length of Filter Text Item must be > 0", "Filter Editor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        AntFilterText2.Focus();
        return;
      }
      if (AntFilterItem1.Text != "(none)" && AntFilterItem2.Text != "(none)")
      {
        if (AntFilterComb.Text.Length == 0)
        {
          MessageBox.Show("Must be 'or' or 'and' for filter combination", "Filter Editor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          AntFilterComb.Focus();
        }
      }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      Selected_Enreg_TextChanged();
      CheckExpression();
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }


  }

}
