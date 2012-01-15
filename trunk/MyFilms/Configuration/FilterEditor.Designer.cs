namespace MyFilmsPlugin.Configuration
{
  partial class FilterEditor
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterEditor));
      this.groupBox_AntSelectedEnreg = new System.Windows.Forms.GroupBox();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.AntFreetextFilterItem = new System.Windows.Forms.Label();
      this.AntFilterFreeText = new System.Windows.Forms.TextBox();
      this.ExpressionPreview = new System.Windows.Forms.TextBox();
      this.AntFilterComb = new System.Windows.Forms.ComboBox();
      this.AntFilterSign2 = new System.Windows.Forms.ComboBox();
      this.AntFilterSign1 = new System.Windows.Forms.ComboBox();
      this.AntFilterItem2 = new System.Windows.Forms.ComboBox();
      this.AntFilterText2 = new System.Windows.Forms.TextBox();
      this.AntFilterItem1 = new System.Windows.Forms.ComboBox();
      this.AntFilterText1 = new System.Windows.Forms.TextBox();
      this.btnOK = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.lbl_Description = new System.Windows.Forms.Label();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.btnReset = new System.Windows.Forms.Button();
      this.btnPreview = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.groupBox_AntSelectedEnreg.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox_AntSelectedEnreg
      // 
      this.groupBox_AntSelectedEnreg.Controls.Add(this.label4);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.textBox1);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.label3);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.label2);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.label1);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFreetextFilterItem);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterFreeText);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.ExpressionPreview);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterComb);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterSign2);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterSign1);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterItem2);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterText2);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterItem1);
      this.groupBox_AntSelectedEnreg.Controls.Add(this.AntFilterText1);
      this.groupBox_AntSelectedEnreg.Location = new System.Drawing.Point(22, 47);
      this.groupBox_AntSelectedEnreg.Name = "groupBox_AntSelectedEnreg";
      this.groupBox_AntSelectedEnreg.Size = new System.Drawing.Size(599, 259);
      this.groupBox_AntSelectedEnreg.TabIndex = 28;
      this.groupBox_AntSelectedEnreg.TabStop = false;
      this.groupBox_AntSelectedEnreg.Text = "User defined Config Filters";
      this.toolTip1.SetToolTip(this.groupBox_AntSelectedEnreg, resources.GetString("groupBox_AntSelectedEnreg.ToolTip"));
      // 
      // textBox1
      // 
      this.textBox1.Enabled = false;
      this.textBox1.Location = new System.Drawing.Point(122, 100);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(40, 20);
      this.textBox1.TabIndex = 34;
      this.textBox1.Text = "AND";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(18, 76);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(91, 13);
      this.label3.TabIndex = 33;
      this.label3.Text = "Second Condition";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(18, 49);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(48, 13);
      this.label2.TabIndex = 32;
      this.label2.Text = "Operator";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(18, 22);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(73, 13);
      this.label1.TabIndex = 31;
      this.label1.Text = "First Condition";
      // 
      // AntFreetextFilterItem
      // 
      this.AntFreetextFilterItem.AutoSize = true;
      this.AntFreetextFilterItem.Location = new System.Drawing.Point(18, 129);
      this.AntFreetextFilterItem.Name = "AntFreetextFilterItem";
      this.AntFreetextFilterItem.Size = new System.Drawing.Size(70, 13);
      this.AntFreetextFilterItem.TabIndex = 30;
      this.AntFreetextFilterItem.Text = "Freetext Filter";
      this.toolTip1.SetToolTip(this.AntFreetextFilterItem, resources.GetString("AntFreetextFilterItem.ToolTip"));
      // 
      // AntFilterFreeText
      // 
      this.AntFilterFreeText.Location = new System.Drawing.Point(122, 126);
      this.AntFilterFreeText.Name = "AntFilterFreeText";
      this.AntFilterFreeText.Size = new System.Drawing.Size(459, 20);
      this.AntFilterFreeText.TabIndex = 29;
      // 
      // ExpressionPreview
      // 
      this.ExpressionPreview.BackColor = System.Drawing.SystemColors.ButtonFace;
      this.ExpressionPreview.Enabled = false;
      this.ExpressionPreview.Location = new System.Drawing.Point(122, 162);
      this.ExpressionPreview.Multiline = true;
      this.ExpressionPreview.Name = "ExpressionPreview";
      this.ExpressionPreview.ReadOnly = true;
      this.ExpressionPreview.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
      this.ExpressionPreview.Size = new System.Drawing.Size(459, 79);
      this.ExpressionPreview.TabIndex = 28;
      this.ExpressionPreview.TabStop = false;
      // 
      // AntFilterComb
      // 
      this.AntFilterComb.DisplayMember = "or";
      this.AntFilterComb.FormattingEnabled = true;
      this.AntFilterComb.ItemHeight = 13;
      this.AntFilterComb.Items.AddRange(new object[] {
            "or",
            "and"});
      this.AntFilterComb.Location = new System.Drawing.Point(122, 46);
      this.AntFilterComb.Name = "AntFilterComb";
      this.AntFilterComb.Size = new System.Drawing.Size(56, 21);
      this.AntFilterComb.TabIndex = 18;
      // 
      // AntFilterSign2
      // 
      this.AntFilterSign2.DisplayMember = "#";
      this.AntFilterSign2.FormattingEnabled = true;
      this.AntFilterSign2.Items.AddRange(new object[] {
            "=",
            "#",
            ">",
            "<",
            "like",
            "not like",
            "in",
            "not in",
            "like in"});
      this.AntFilterSign2.Location = new System.Drawing.Point(243, 73);
      this.AntFilterSign2.Name = "AntFilterSign2";
      this.AntFilterSign2.Size = new System.Drawing.Size(60, 21);
      this.AntFilterSign2.TabIndex = 23;
      // 
      // AntFilterSign1
      // 
      this.AntFilterSign1.DisplayMember = "#";
      this.AntFilterSign1.FormattingEnabled = true;
      this.AntFilterSign1.Items.AddRange(new object[] {
            "=",
            "#",
            ">",
            "<",
            "like",
            "not like",
            "in",
            "not in",
            "like in"});
      this.AntFilterSign1.Location = new System.Drawing.Point(243, 19);
      this.AntFilterSign1.Name = "AntFilterSign1";
      this.AntFilterSign1.Size = new System.Drawing.Size(60, 21);
      this.AntFilterSign1.TabIndex = 20;
      this.toolTip1.SetToolTip(this.AntFilterSign1, resources.GetString("AntFilterSign1.ToolTip"));
      // 
      // AntFilterItem2
      // 
      this.AntFilterItem2.FormattingEnabled = true;
      this.AntFilterItem2.Location = new System.Drawing.Point(122, 73);
      this.AntFilterItem2.Name = "AntFilterItem2";
      this.AntFilterItem2.Size = new System.Drawing.Size(115, 21);
      this.AntFilterItem2.Sorted = true;
      this.AntFilterItem2.TabIndex = 22;
      this.AntFilterItem2.SelectedIndexChanged += new System.EventHandler(this.AntFilterItem2_SelectedIndexChanged);
      // 
      // AntFilterText2
      // 
      this.AntFilterText2.Location = new System.Drawing.Point(309, 73);
      this.AntFilterText2.Name = "AntFilterText2";
      this.AntFilterText2.Size = new System.Drawing.Size(272, 20);
      this.AntFilterText2.TabIndex = 24;
      // 
      // AntFilterItem1
      // 
      this.AntFilterItem1.FormattingEnabled = true;
      this.AntFilterItem1.Location = new System.Drawing.Point(122, 19);
      this.AntFilterItem1.Name = "AntFilterItem1";
      this.AntFilterItem1.Size = new System.Drawing.Size(115, 21);
      this.AntFilterItem1.Sorted = true;
      this.AntFilterItem1.TabIndex = 19;
      this.AntFilterItem1.SelectedIndexChanged += new System.EventHandler(this.AntFilterItem1_SelectedIndexChanged);
      // 
      // AntFilterText1
      // 
      this.AntFilterText1.Location = new System.Drawing.Point(309, 19);
      this.AntFilterText1.Name = "AntFilterText1";
      this.AntFilterText1.Size = new System.Drawing.Size(272, 20);
      this.AntFilterText1.TabIndex = 21;
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(528, 315);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(75, 23);
      this.btnOK.TabIndex = 29;
      this.btnOK.Text = "Ok";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(421, 315);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 30;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // lbl_Description
      // 
      this.lbl_Description.AutoSize = true;
      this.lbl_Description.Location = new System.Drawing.Point(30, 9);
      this.lbl_Description.Name = "lbl_Description";
      this.lbl_Description.Size = new System.Drawing.Size(362, 26);
      this.lbl_Description.TabIndex = 31;
      this.lbl_Description.Text = "You can define 2 filter rules and combine it with a freetext definition.\r\nThe res" +
          "ult is shown in the text box and will be saved to your MyFilms config.";
      // 
      // btnReset
      // 
      this.btnReset.Location = new System.Drawing.Point(250, 315);
      this.btnReset.Name = "btnReset";
      this.btnReset.Size = new System.Drawing.Size(75, 23);
      this.btnReset.TabIndex = 32;
      this.btnReset.Text = "Reset";
      this.btnReset.UseVisualStyleBackColor = true;
      this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
      // 
      // btnPreview
      // 
      this.btnPreview.Location = new System.Drawing.Point(144, 315);
      this.btnPreview.Name = "btnPreview";
      this.btnPreview.Size = new System.Drawing.Size(75, 23);
      this.btnPreview.TabIndex = 33;
      this.btnPreview.Text = "Preview";
      this.btnPreview.UseVisualStyleBackColor = true;
      this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(18, 165);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(61, 26);
      this.label4.TabIndex = 35;
      this.label4.Text = "Expression \r\nPreview";
      // 
      // FilterEditor
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(639, 347);
      this.Controls.Add(this.btnPreview);
      this.Controls.Add(this.btnReset);
      this.Controls.Add(this.lbl_Description);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.groupBox_AntSelectedEnreg);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FilterEditor";
      this.ShowInTaskbar = false;
      this.Text = "Filter Editor";
      this.groupBox_AntSelectedEnreg.ResumeLayout(false);
      this.groupBox_AntSelectedEnreg.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox_AntSelectedEnreg;
    private System.Windows.Forms.Label AntFreetextFilterItem;
    private System.Windows.Forms.TextBox AntFilterFreeText;
    private System.Windows.Forms.TextBox ExpressionPreview;
    private System.Windows.Forms.ComboBox AntFilterComb;
    private System.Windows.Forms.ComboBox AntFilterSign2;
    private System.Windows.Forms.ComboBox AntFilterSign1;
    private System.Windows.Forms.ComboBox AntFilterItem2;
    private System.Windows.Forms.TextBox AntFilterText2;
    private System.Windows.Forms.ComboBox AntFilterItem1;
    private System.Windows.Forms.TextBox AntFilterText1;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Label lbl_Description;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.Button btnReset;
    private System.Windows.Forms.Button btnPreview;
    private System.Windows.Forms.Label label4;
  }
}