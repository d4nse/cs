namespace MailSender
{
    partial class Form1
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toBox = new System.Windows.Forms.TextBox();
            this.themeBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fromBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.attachementsListBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bodyBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // toBox
            // 
            this.toBox.Location = new System.Drawing.Point(86, 27);
            this.toBox.Name = "toBox";
            this.toBox.Size = new System.Drawing.Size(400, 20);
            this.toBox.TabIndex = 0;
            // 
            // themeBox
            // 
            this.themeBox.Location = new System.Drawing.Point(86, 81);
            this.themeBox.Name = "themeBox";
            this.themeBox.Size = new System.Drawing.Size(400, 20);
            this.themeBox.TabIndex = 1;
            this.themeBox.Text = "SMTP client for lesson #3";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "To whom";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Theme";
            // 
            // fromBox
            // 
            this.fromBox.Location = new System.Drawing.Point(86, 131);
            this.fromBox.Name = "fromBox";
            this.fromBox.Size = new System.Drawing.Size(400, 20);
            this.fromBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "From whom";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(86, 177);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Attach a file";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // attachementsListBox
            // 
            this.attachementsListBox.FormattingEnabled = true;
            this.attachementsListBox.Location = new System.Drawing.Point(515, 29);
            this.attachementsListBox.Name = "attachementsListBox";
            this.attachementsListBox.Size = new System.Drawing.Size(293, 160);
            this.attachementsListBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(560, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(167, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "List of attached files";
            // 
            // bodyBox
            // 
            this.bodyBox.Location = new System.Drawing.Point(19, 231);
            this.bodyBox.Multiline = true;
            this.bodyBox.Name = "bodyBox";
            this.bodyBox.Size = new System.Drawing.Size(765, 327);
            this.bodyBox.TabIndex = 9;
            this.bodyBox.Text = "Free text here";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(709, 585);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "send";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(19, 585);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(291, 23);
            this.button3.TabIndex = 11;
            this.button3.Text = "New letter";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 620);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.bodyBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.attachementsListBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fromBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.themeBox);
            this.Controls.Add(this.toBox);
            this.Name = "Form1";
            this.Text = "MailSender";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox toBox;
        private System.Windows.Forms.TextBox themeBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fromBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox attachementsListBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox bodyBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

