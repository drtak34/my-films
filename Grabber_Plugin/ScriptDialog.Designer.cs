namespace MyVideoGrabber
{
    partial class MultiGrabberForm
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
          this.button1 = new System.Windows.Forms.Button();
          this.button2 = new System.Windows.Forms.Button();
          this.button3 = new System.Windows.Forms.Button();
          this.button4 = new System.Windows.Forms.Button();
          this.button5 = new System.Windows.Forms.Button();
          this.button6 = new System.Windows.Forms.Button();
          this.listView1 = new System.Windows.Forms.ListView();
          this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
          this.label_Version = new System.Windows.Forms.Label();
          this.SuspendLayout();
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point(9, 37);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(58, 13);
          this.label1.TabIndex = 1;
          this.label1.Text = "Config files";
          // 
          // button1
          // 
          this.button1.Location = new System.Drawing.Point(469, 275);
          this.button1.Name = "button1";
          this.button1.Size = new System.Drawing.Size(75, 23);
          this.button1.TabIndex = 2;
          this.button1.Text = "OK";
          this.button1.UseVisualStyleBackColor = true;
          this.button1.Click += new System.EventHandler(this.button1_Click);
          // 
          // button2
          // 
          this.button2.Location = new System.Drawing.Point(388, 275);
          this.button2.Name = "button2";
          this.button2.Size = new System.Drawing.Size(75, 23);
          this.button2.TabIndex = 3;
          this.button2.Text = "Cancel";
          this.button2.UseVisualStyleBackColor = true;
          this.button2.Click += new System.EventHandler(this.button2_Click);
          // 
          // button3
          // 
          this.button3.Location = new System.Drawing.Point(550, 53);
          this.button3.Name = "button3";
          this.button3.Size = new System.Drawing.Size(45, 23);
          this.button3.TabIndex = 4;
          this.button3.Text = "Add";
          this.button3.UseVisualStyleBackColor = true;
          this.button3.Click += new System.EventHandler(this.button3_Click);
          // 
          // button4
          // 
          this.button4.Location = new System.Drawing.Point(12, 275);
          this.button4.Name = "button4";
          this.button4.Size = new System.Drawing.Size(51, 23);
          this.button4.TabIndex = 6;
          this.button4.Text = "Up";
          this.button4.UseVisualStyleBackColor = true;
          this.button4.Click += new System.EventHandler(this.button4_Click);
          // 
          // button5
          // 
          this.button5.Location = new System.Drawing.Point(69, 275);
          this.button5.Name = "button5";
          this.button5.Size = new System.Drawing.Size(52, 23);
          this.button5.TabIndex = 7;
          this.button5.Text = "Down";
          this.button5.UseVisualStyleBackColor = true;
          this.button5.Click += new System.EventHandler(this.button5_Click);
          // 
          // button6
          // 
          this.button6.Location = new System.Drawing.Point(148, 275);
          this.button6.Name = "button6";
          this.button6.Size = new System.Drawing.Size(59, 23);
          this.button6.TabIndex = 8;
          this.button6.Text = "Delete";
          this.button6.UseVisualStyleBackColor = true;
          this.button6.Click += new System.EventHandler(this.button6_Click);
          // 
          // listView1
          // 
          this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
          this.listView1.Location = new System.Drawing.Point(12, 53);
          this.listView1.MultiSelect = false;
          this.listView1.Name = "listView1";
          this.listView1.Size = new System.Drawing.Size(532, 216);
          this.listView1.TabIndex = 9;
          this.listView1.UseCompatibleStateImageBehavior = false;
          this.listView1.View = System.Windows.Forms.View.Details;
          // 
          // columnHeader1
          // 
          this.columnHeader1.Text = "";
          this.columnHeader1.Width = 520;
          // 
          // label_Version
          // 
          this.label_Version.AutoSize = true;
          this.label_Version.Location = new System.Drawing.Point(548, 9);
          this.label_Version.Name = "label_Version";
          this.label_Version.Size = new System.Drawing.Size(47, 13);
          this.label_Version.TabIndex = 10;
          this.label_Version.Text = "V0.0.0.0";
          this.label_Version.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
          // 
          // MultiGrabberForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(608, 310);
          this.Controls.Add(this.label_Version);
          this.Controls.Add(this.listView1);
          this.Controls.Add(this.button6);
          this.Controls.Add(this.button5);
          this.Controls.Add(this.button4);
          this.Controls.Add(this.button3);
          this.Controls.Add(this.button2);
          this.Controls.Add(this.button1);
          this.Controls.Add(this.label1);
          this.Name = "MultiGrabberForm";
          this.Text = "MultiGrabber - Configuration";
          this.Load += new System.EventHandler(this.MyVideoGrabberForm_Load);
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label_Version;
    }
}