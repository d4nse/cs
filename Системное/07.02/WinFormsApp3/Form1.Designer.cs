namespace WinFormsApp3
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnLoadForbiddenWords = new Button();
            btnStart = new Button();
            btnPause = new Button();
            btnStop = new Button();
            progressBar = new ProgressBar();
            txtLog = new TextBox();
            btnGenerateReport = new Button();
            SuspendLayout();
            // 
            // btnLoadForbiddenWords
            // 
            btnLoadForbiddenWords.Location = new Point(24, 49);
            btnLoadForbiddenWords.Name = "btnLoadForbiddenWords";
            btnLoadForbiddenWords.Size = new Size(150, 46);
            btnLoadForbiddenWords.TabIndex = 0;
            btnLoadForbiddenWords.Text = "загрузка";
            btnLoadForbiddenWords.UseVisualStyleBackColor = true;
            btnLoadForbiddenWords.Click += btnLoadForbiddenWords_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(214, 57);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(150, 46);
            btnStart.TabIndex = 1;
            btnStart.Text = "старт";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnPause
            // 
            btnPause.Enabled = false;
            btnPause.Location = new Point(418, 66);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(150, 46);
            btnPause.TabIndex = 2;
            btnPause.Text = "пауза";
            btnPause.UseVisualStyleBackColor = true;
            btnPause.Click += btnPause_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(614, 78);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(150, 46);
            btnStop.TabIndex = 3;
            btnStop.Text = "стоп";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(46, 174);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(200, 46);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 4;
            progressBar.Visible = false;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(304, 174);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(200, 78);
            txtLog.TabIndex = 5;
            // 
            // btnGenerateReport
            // 
            btnGenerateReport.Location = new Point(574, 228);
            btnGenerateReport.Name = "btnGenerateReport";
            btnGenerateReport.Size = new Size(150, 46);
            btnGenerateReport.TabIndex = 6;
            btnGenerateReport.Text = "отчет";
            btnGenerateReport.UseVisualStyleBackColor = true;
            btnGenerateReport.Click += btnGenerateReport_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnGenerateReport);
            Controls.Add(txtLog);
            Controls.Add(progressBar);
            Controls.Add(btnStop);
            Controls.Add(btnPause);
            Controls.Add(btnStart);
            Controls.Add(btnLoadForbiddenWords);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnLoadForbiddenWords;
        private Button btnStart;
        private Button btnPause;
        private Button btnStop;
        private ProgressBar progressBar;
        private TextBox txtLog;
        private Button btnGenerateReport;
    }
}
