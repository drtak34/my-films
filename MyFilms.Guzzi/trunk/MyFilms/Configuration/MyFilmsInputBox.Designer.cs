namespace MyFilmsPlugin.MyFilms.Configuration
{
    partial class MyFilmsInputBox
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
          this.label1 = new System.Windows.Forms.Label();
          this.textBoxNewName = new System.Windows.Forms.TextBox();
          this.buttonOK = new System.Windows.Forms.Button();
          this.label2 = new System.Windows.Forms.Label();
          this.pictureBox1 = new System.Windows.Forms.PictureBox();
          ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
          this.SuspendLayout();
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point(22, 28);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(124, 13);
          this.label1.TabIndex = 0;
          this.label1.Text = "Please enter new name !";
          // 
          // textBoxNewName
          // 
          this.textBoxNewName.Location = new System.Drawing.Point(25, 84);
          this.textBoxNewName.Name = "textBoxNewName";
          this.textBoxNewName.Size = new System.Drawing.Size(402, 20);
          this.textBoxNewName.TabIndex = 1;
          this.textBoxNewName.TextChanged += new System.EventHandler(this.textBoxNewName_TextChanged);
          // 
          // buttonOK
          // 
          this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
          this.buttonOK.Location = new System.Drawing.Point(353, 125);
          this.buttonOK.Name = "buttonOK";
          this.buttonOK.Size = new System.Drawing.Size(74, 22);
          this.buttonOK.TabIndex = 2;
          this.buttonOK.Text = "OK";
          this.buttonOK.UseVisualStyleBackColor = true;
          this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
          // 
          // label2
          // 
          this.label2.AutoSize = true;
          this.label2.Location = new System.Drawing.Point(22, 51);
          this.label2.Name = "label2";
          this.label2.Size = new System.Drawing.Size(296, 13);
          this.label2.TabIndex = 3;
          this.label2.Text = "Using an existing name overwrites the corresponding config  !";
          // 
          // pictureBox1
          // 
          this.pictureBox1.Image = global::MyFilmsPlugin.Properties.Resources.film_reel_128x128;
          this.pictureBox1.Location = new System.Drawing.Point(353, 12);
          this.pictureBox1.Name = "pictureBox1";
          this.pictureBox1.Size = new System.Drawing.Size(85, 65);
          this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
          this.pictureBox1.TabIndex = 4;
          this.pictureBox1.TabStop = false;
          // 
          // MyFilmsInputBox
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(459, 163);
          this.Controls.Add(this.pictureBox1);
          this.Controls.Add(this.label2);
          this.Controls.Add(this.buttonOK);
          this.Controls.Add(this.textBoxNewName);
          this.Controls.Add(this.label1);
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
          this.Name = "MyFilmsInputBox";
          this.Text = "MyFilmsInputBox";
          ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxNewName;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}