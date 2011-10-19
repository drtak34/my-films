namespace MyFilmsPlugin.Configuration
{
  partial class HyperLinkParamGenerator
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HyperLinkParamGenerator));
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.tbEditorStartParamsOutput = new System.Windows.Forms.TextBox();
      this.btnSaveEditorStartParams = new System.Windows.Forms.Button();
      this.label60 = new System.Windows.Forms.Label();
      this.tbEditorSearchExpression = new System.Windows.Forms.TextBox();
      this.label59 = new System.Windows.Forms.Label();
      this.cbEditorLayout = new System.Windows.Forms.ComboBox();
      this.label58 = new System.Windows.Forms.Label();
      this.label57 = new System.Windows.Forms.Label();
      this.label52 = new System.Windows.Forms.Label();
      this.btnLoadEditorValues = new System.Windows.Forms.Button();
      this.cbEditorViews = new System.Windows.Forms.ComboBox();
      this.cbEditorConfigs = new System.Windows.Forms.ComboBox();
      this.cbEditorViewValues = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.toolTipHyperlinkStartParams = new System.Windows.Forms.ToolTip(this.components);
      this.label2 = new System.Windows.Forms.Label();
      this.btnCancel = new System.Windows.Forms.Button();
      this.groupBox3.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.btnCancel);
      this.groupBox3.Controls.Add(this.label2);
      this.groupBox3.Controls.Add(this.label1);
      this.groupBox3.Controls.Add(this.tbEditorStartParamsOutput);
      this.groupBox3.Controls.Add(this.btnSaveEditorStartParams);
      this.groupBox3.Controls.Add(this.label60);
      this.groupBox3.Controls.Add(this.tbEditorSearchExpression);
      this.groupBox3.Controls.Add(this.label59);
      this.groupBox3.Controls.Add(this.cbEditorLayout);
      this.groupBox3.Controls.Add(this.label58);
      this.groupBox3.Controls.Add(this.label57);
      this.groupBox3.Controls.Add(this.label52);
      this.groupBox3.Controls.Add(this.btnLoadEditorValues);
      this.groupBox3.Controls.Add(this.cbEditorViews);
      this.groupBox3.Controls.Add(this.cbEditorConfigs);
      this.groupBox3.Controls.Add(this.cbEditorViewValues);
      this.groupBox3.Location = new System.Drawing.Point(12, 12);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(659, 186);
      this.groupBox3.TabIndex = 94;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Start Parameter Creator ...";
      this.toolTipHyperlinkStartParams.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
      // 
      // tbEditorStartParamsOutput
      // 
      this.tbEditorStartParamsOutput.Location = new System.Drawing.Point(15, 112);
      this.tbEditorStartParamsOutput.Name = "tbEditorStartParamsOutput";
      this.tbEditorStartParamsOutput.Size = new System.Drawing.Size(625, 20);
      this.tbEditorStartParamsOutput.TabIndex = 104;
      // 
      // btnSaveEditorStartParams
      // 
      this.btnSaveEditorStartParams.Location = new System.Drawing.Point(501, 147);
      this.btnSaveEditorStartParams.Name = "btnSaveEditorStartParams";
      this.btnSaveEditorStartParams.Size = new System.Drawing.Size(139, 23);
      this.btnSaveEditorStartParams.TabIndex = 103;
      this.btnSaveEditorStartParams.Text = "Create StartParameters";
      this.btnSaveEditorStartParams.UseVisualStyleBackColor = true;
      this.btnSaveEditorStartParams.Click += new System.EventHandler(this.btnSaveEditorStartParams_Click);
      // 
      // label60
      // 
      this.label60.AutoSize = true;
      this.label60.Location = new System.Drawing.Point(420, 16);
      this.label60.Name = "label60";
      this.label60.Size = new System.Drawing.Size(95, 13);
      this.label60.TabIndex = 102;
      this.label60.Text = "Search Expression";
      // 
      // tbEditorSearchExpression
      // 
      this.tbEditorSearchExpression.Location = new System.Drawing.Point(418, 32);
      this.tbEditorSearchExpression.Name = "tbEditorSearchExpression";
      this.tbEditorSearchExpression.Size = new System.Drawing.Size(100, 20);
      this.tbEditorSearchExpression.TabIndex = 101;
      // 
      // label59
      // 
      this.label59.AutoSize = true;
      this.label59.Location = new System.Drawing.Point(540, 16);
      this.label59.Name = "label59";
      this.label59.Size = new System.Drawing.Size(39, 13);
      this.label59.TabIndex = 100;
      this.label59.Text = "Layout";
      // 
      // cbEditorLayout
      // 
      this.cbEditorLayout.FormattingEnabled = true;
      this.cbEditorLayout.Items.AddRange(new object[] {
            "",
            "List View",
            "Big Thumbs",
            "Small Thumbs",
            "Filmstrip",
            "Coverflow"});
      this.cbEditorLayout.Location = new System.Drawing.Point(538, 32);
      this.cbEditorLayout.Name = "cbEditorLayout";
      this.cbEditorLayout.Size = new System.Drawing.Size(102, 21);
      this.cbEditorLayout.TabIndex = 99;
      this.cbEditorLayout.SelectedIndexChanged += new System.EventHandler(this.cbEditorLayout_SelectedIndexChanged);
      // 
      // label58
      // 
      this.label58.AutoSize = true;
      this.label58.Location = new System.Drawing.Point(151, 16);
      this.label58.Name = "label58";
      this.label58.Size = new System.Drawing.Size(82, 13);
      this.label58.TabIndex = 98;
      this.label58.Text = "Config selection";
      // 
      // label57
      // 
      this.label57.AutoSize = true;
      this.label57.Location = new System.Drawing.Point(286, 16);
      this.label57.Name = "label57";
      this.label57.Size = new System.Drawing.Size(77, 13);
      this.label57.TabIndex = 97;
      this.label57.Text = "View Selection";
      // 
      // label52
      // 
      this.label52.AutoSize = true;
      this.label52.Location = new System.Drawing.Point(286, 56);
      this.label52.Name = "label52";
      this.label52.Size = new System.Drawing.Size(104, 13);
      this.label52.TabIndex = 96;
      this.label52.Text = "Value to filter movies";
      // 
      // btnLoadEditorValues
      // 
      this.btnLoadEditorValues.Location = new System.Drawing.Point(15, 30);
      this.btnLoadEditorValues.Name = "btnLoadEditorValues";
      this.btnLoadEditorValues.Size = new System.Drawing.Size(121, 23);
      this.btnLoadEditorValues.TabIndex = 93;
      this.btnLoadEditorValues.Text = "Load MyFilms Data";
      this.btnLoadEditorValues.UseVisualStyleBackColor = true;
      this.btnLoadEditorValues.Click += new System.EventHandler(this.btnLoadEditorValues_Click);
      // 
      // cbEditorViews
      // 
      this.cbEditorViews.FormattingEnabled = true;
      this.cbEditorViews.Location = new System.Drawing.Point(284, 32);
      this.cbEditorViews.Name = "cbEditorViews";
      this.cbEditorViews.Size = new System.Drawing.Size(121, 21);
      this.cbEditorViews.TabIndex = 95;
      this.cbEditorViews.SelectedIndexChanged += new System.EventHandler(this.cbEditorViews_SelectedIndexChanged);
      // 
      // cbEditorConfigs
      // 
      this.cbEditorConfigs.FormattingEnabled = true;
      this.cbEditorConfigs.Location = new System.Drawing.Point(149, 32);
      this.cbEditorConfigs.Name = "cbEditorConfigs";
      this.cbEditorConfigs.Size = new System.Drawing.Size(121, 21);
      this.cbEditorConfigs.TabIndex = 94;
      this.cbEditorConfigs.SelectedIndexChanged += new System.EventHandler(this.cbEditorConfigs_SelectedIndexChanged);
      // 
      // cbEditorViewValues
      // 
      this.cbEditorViewValues.FormattingEnabled = true;
      this.cbEditorViewValues.Location = new System.Drawing.Point(284, 72);
      this.cbEditorViewValues.Name = "cbEditorViewValues";
      this.cbEditorViewValues.Size = new System.Drawing.Size(121, 21);
      this.cbEditorViewValues.TabIndex = 6;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(16, 96);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(132, 13);
      this.label1.TabIndex = 105;
      this.label1.Text = "Resulting Start Parameters";
      // 
      // toolTipHyperlinkStartParams
      // 
      this.toolTipHyperlinkStartParams.AutoPopDelay = 15000;
      this.toolTipHyperlinkStartParams.InitialDelay = 500;
      this.toolTipHyperlinkStartParams.ReshowDelay = 100;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(16, 144);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(288, 26);
      this.label2.TabIndex = 106;
      this.label2.Text = "Copy the Resulting Start Parameters and use it as hyperlink \r\non any button in yo" +
          "ur skin.";
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(406, 147);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 107;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // HyperLinkParamGenerator
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(683, 210);
      this.Controls.Add(this.groupBox3);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "HyperLinkParamGenerator";
      this.Text = "HyperLink Start Parameter Generator";
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.TextBox tbEditorStartParamsOutput;
    private System.Windows.Forms.Button btnSaveEditorStartParams;
    private System.Windows.Forms.Label label60;
    private System.Windows.Forms.TextBox tbEditorSearchExpression;
    private System.Windows.Forms.Label label59;
    private System.Windows.Forms.ComboBox cbEditorLayout;
    private System.Windows.Forms.Label label58;
    private System.Windows.Forms.Label label57;
    private System.Windows.Forms.Label label52;
    private System.Windows.Forms.Button btnLoadEditorValues;
    private System.Windows.Forms.ComboBox cbEditorViews;
    private System.Windows.Forms.ComboBox cbEditorConfigs;
    private System.Windows.Forms.ComboBox cbEditorViewValues;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ToolTip toolTipHyperlinkStartParams;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Label label2;
  }
}