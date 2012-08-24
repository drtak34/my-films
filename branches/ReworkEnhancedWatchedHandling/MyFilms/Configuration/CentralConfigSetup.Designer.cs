namespace MyFilmsPlugin.Configuration
{
  partial class CentralConfigSetup
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CentralConfigSetup));
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.groupBoxCentralConfig = new System.Windows.Forms.GroupBox();
      this.btnSyncToServer = new System.Windows.Forms.Button();
      this.btnSyncFromServer = new System.Windows.Forms.Button();
      this.cbSyncFromServerOnStartup = new System.Windows.Forms.CheckBox();
      this.btnMyFilmsCentralConfigFile = new System.Windows.Forms.Button();
      this.MyFilmsCentralConfigDir = new System.Windows.Forms.TextBox();
      this.label23 = new System.Windows.Forms.Label();
      this.btnQuit = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.btnResetRemotePath = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBoxCentralConfig.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBoxCentralConfig
      // 
      this.groupBoxCentralConfig.Controls.Add(this.groupBox1);
      this.groupBoxCentralConfig.Controls.Add(this.btnResetRemotePath);
      this.groupBoxCentralConfig.Controls.Add(this.cbSyncFromServerOnStartup);
      this.groupBoxCentralConfig.Controls.Add(this.btnMyFilmsCentralConfigFile);
      this.groupBoxCentralConfig.Controls.Add(this.MyFilmsCentralConfigDir);
      this.groupBoxCentralConfig.Controls.Add(this.label23);
      this.groupBoxCentralConfig.Location = new System.Drawing.Point(12, 71);
      this.groupBoxCentralConfig.Name = "groupBoxCentralConfig";
      this.groupBoxCentralConfig.Size = new System.Drawing.Size(599, 147);
      this.groupBoxCentralConfig.TabIndex = 75;
      this.groupBoxCentralConfig.TabStop = false;
      this.groupBoxCentralConfig.Text = "Central Config Handling";
      this.toolTip1.SetToolTip(this.groupBoxCentralConfig, resources.GetString("groupBoxCentralConfig.ToolTip"));
      // 
      // btnSyncToServer
      // 
      this.btnSyncToServer.Location = new System.Drawing.Point(37, 23);
      this.btnSyncToServer.Name = "btnSyncToServer";
      this.btnSyncToServer.Size = new System.Drawing.Size(105, 23);
      this.btnSyncToServer.TabIndex = 3;
      this.btnSyncToServer.Text = "Sync to Server";
      this.btnSyncToServer.UseVisualStyleBackColor = true;
      this.btnSyncToServer.Click += new System.EventHandler(this.btnSyncToServer_Click);
      // 
      // btnSyncFromServer
      // 
      this.btnSyncFromServer.Location = new System.Drawing.Point(157, 23);
      this.btnSyncFromServer.Name = "btnSyncFromServer";
      this.btnSyncFromServer.Size = new System.Drawing.Size(105, 23);
      this.btnSyncFromServer.TabIndex = 4;
      this.btnSyncFromServer.Text = "Sync from Server";
      this.btnSyncFromServer.UseVisualStyleBackColor = true;
      this.btnSyncFromServer.Click += new System.EventHandler(this.btnSyncFromServer_Click);
      // 
      // cbSyncFromServerOnStartup
      // 
      this.cbSyncFromServerOnStartup.AutoSize = true;
      this.cbSyncFromServerOnStartup.Location = new System.Drawing.Point(27, 42);
      this.cbSyncFromServerOnStartup.Name = "cbSyncFromServerOnStartup";
      this.cbSyncFromServerOnStartup.Size = new System.Drawing.Size(244, 17);
      this.cbSyncFromServerOnStartup.TabIndex = 5;
      this.cbSyncFromServerOnStartup.Text = "Enable automatic Sync from Server on Startup";
      this.toolTip1.SetToolTip(this.cbSyncFromServerOnStartup, resources.GetString("cbSyncFromServerOnStartup.ToolTip"));
      this.cbSyncFromServerOnStartup.UseVisualStyleBackColor = true;
      this.cbSyncFromServerOnStartup.CheckedChanged += new System.EventHandler(this.cbSyncFromServerOnStartup_CheckedChanged);
      // 
      // btnMyFilmsCentralConfigFile
      // 
      this.btnMyFilmsCentralConfigFile.Location = new System.Drawing.Point(474, 111);
      this.btnMyFilmsCentralConfigFile.Name = "btnMyFilmsCentralConfigFile";
      this.btnMyFilmsCentralConfigFile.Size = new System.Drawing.Size(29, 20);
      this.btnMyFilmsCentralConfigFile.TabIndex = 2;
      this.btnMyFilmsCentralConfigFile.Text = "...";
      this.btnMyFilmsCentralConfigFile.UseVisualStyleBackColor = true;
      this.btnMyFilmsCentralConfigFile.Click += new System.EventHandler(this.btnMyFilmsCentralConfigFile_Click);
      // 
      // MyFilmsCentralConfigDir
      // 
      this.MyFilmsCentralConfigDir.Location = new System.Drawing.Point(27, 111);
      this.MyFilmsCentralConfigDir.Name = "MyFilmsCentralConfigDir";
      this.MyFilmsCentralConfigDir.Size = new System.Drawing.Size(432, 20);
      this.MyFilmsCentralConfigDir.TabIndex = 1;
      this.toolTip1.SetToolTip(this.MyFilmsCentralConfigDir, "Set the directory to store to / retrieve from the MyFilms.xml config file.\r\nIt sh" +
              "ould be a place that is accessible from all your workstations / HTPCs.");
      // 
      // label23
      // 
      this.label23.AutoSize = true;
      this.label23.Location = new System.Drawing.Point(31, 94);
      this.label23.Name = "label23";
      this.label23.Size = new System.Drawing.Size(165, 13);
      this.label23.TabIndex = 0;
      this.label23.Text = "Path to central MyFilms Config Dir";
      // 
      // btnQuit
      // 
      this.btnQuit.Location = new System.Drawing.Point(516, 236);
      this.btnQuit.Name = "btnQuit";
      this.btnQuit.Size = new System.Drawing.Size(75, 23);
      this.btnQuit.TabIndex = 7;
      this.btnQuit.Text = "Quit";
      this.btnQuit.UseVisualStyleBackColor = true;
      this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(33, 20);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(549, 39);
      this.label1.TabIndex = 76;
      this.label1.Text = resources.GetString("label1.Text");
      this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
      // 
      // btnResetRemotePath
      // 
      this.btnResetRemotePath.Location = new System.Drawing.Point(522, 111);
      this.btnResetRemotePath.Name = "btnResetRemotePath";
      this.btnResetRemotePath.Size = new System.Drawing.Size(57, 20);
      this.btnResetRemotePath.TabIndex = 7;
      this.btnResetRemotePath.Text = "Reset";
      this.toolTip1.SetToolTip(this.btnResetRemotePath, "Resets the remote config directory path");
      this.btnResetRemotePath.UseVisualStyleBackColor = true;
      this.btnResetRemotePath.Click += new System.EventHandler(this.btnResetRemotePath_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.btnSyncToServer);
      this.groupBox1.Controls.Add(this.btnSyncFromServer);
      this.groupBox1.Location = new System.Drawing.Point(296, 19);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(283, 64);
      this.groupBox1.TabIndex = 77;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Manual Sync from or to remote directory ...";
      // 
      // CentralConfigSetup
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(623, 271);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnQuit);
      this.Controls.Add(this.groupBoxCentralConfig);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "CentralConfigSetup";
      this.Text = "MyFilms Central Config Setup";
      this.Load += new System.EventHandler(this.CentralConfigSetup_Load);
      this.Leave += new System.EventHandler(this.CentralConfigSetup_Leave);
      this.groupBoxCentralConfig.ResumeLayout(false);
      this.groupBoxCentralConfig.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.GroupBox groupBoxCentralConfig;
    private System.Windows.Forms.Button btnQuit;
    private System.Windows.Forms.Button btnSyncToServer;
    private System.Windows.Forms.Button btnSyncFromServer;
    private System.Windows.Forms.CheckBox cbSyncFromServerOnStartup;
    private System.Windows.Forms.Button btnMyFilmsCentralConfigFile;
    private System.Windows.Forms.TextBox MyFilmsCentralConfigDir;
    private System.Windows.Forms.Label label23;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnResetRemotePath;
    private System.Windows.Forms.GroupBox groupBox1;
  }
}