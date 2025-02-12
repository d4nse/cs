namespace WinFormsApp2
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
            components = new System.ComponentModel.Container();
            listBoxProcesses = new ListBox();
            textBoxInterval = new TextBox();
            buttonStart = new Button();
            refreshTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // listBoxProcesses
            // 
            listBoxProcesses.FormattingEnabled = true;
            listBoxProcesses.Location = new Point(72, 95);
            listBoxProcesses.Name = "listBoxProcesses";
            listBoxProcesses.Size = new Size(240, 164);
            listBoxProcesses.TabIndex = 0;
            // 
            // textBoxInterval
            // 
            textBoxInterval.Location = new Point(376, 103);
            textBoxInterval.Name = "textBoxInterval";
            textBoxInterval.Size = new Size(200, 39);
            textBoxInterval.TabIndex = 1;
            // 
            // buttonStart
            // 
            buttonStart.Location = new Point(340, 179);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(150, 46);
            buttonStart.TabIndex = 2;
            buttonStart.Text = "button1";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += buttonStart_Click;
            // 
            // refreshTimer
            // 
            refreshTimer.Tick += this.RefreshTimer_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonStart);
            Controls.Add(textBoxInterval);
            Controls.Add(listBoxProcesses);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBoxProcesses;
        private TextBox textBoxInterval;
        private Button buttonStart;
        private System.Windows.Forms.Timer refreshTimer;
    }
}
