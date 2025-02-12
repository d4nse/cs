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
            buttonStartNumbers = new Button();
            buttonStartLetters = new Button();
            buttonStartSymbols = new Button();
            comboBoxNumberPriority = new ComboBox();
            comboBoxLetterPriority = new ComboBox();
            comboBoxSymbolPriority = new ComboBox();
            textBoxOutput = new TextBox();
            SuspendLayout();
            // 
            // buttonStartNumbers
            // 
            buttonStartNumbers.Location = new Point(95, 85);
            buttonStartNumbers.Name = "buttonStartNumbers";
            buttonStartNumbers.Size = new Size(150, 46);
            buttonStartNumbers.TabIndex = 0;
            buttonStartNumbers.Text = "button1";
            buttonStartNumbers.UseVisualStyleBackColor = true;
            buttonStartNumbers.Click += buttonStartNumbers_Click;
            // 
            // buttonStartLetters
            // 
            buttonStartLetters.Location = new Point(317, 85);
            buttonStartLetters.Name = "buttonStartLetters";
            buttonStartLetters.Size = new Size(150, 46);
            buttonStartLetters.TabIndex = 1;
            buttonStartLetters.Text = "button1";
            buttonStartLetters.UseVisualStyleBackColor = true;
            buttonStartLetters.Click += buttonStartLetters_Click;
            // 
            // buttonStartSymbols
            // 
            buttonStartSymbols.Location = new Point(591, 83);
            buttonStartSymbols.Name = "buttonStartSymbols";
            buttonStartSymbols.Size = new Size(150, 46);
            buttonStartSymbols.TabIndex = 2;
            buttonStartSymbols.Text = "button2";
            buttonStartSymbols.UseVisualStyleBackColor = true;
            buttonStartSymbols.Click += buttonStartSymbols_Click;
            // 
            // comboBoxNumberPriority
            // 
            comboBoxNumberPriority.FormattingEnabled = true;
            comboBoxNumberPriority.Location = new Point(38, 163);
            comboBoxNumberPriority.Name = "comboBoxNumberPriority";
            comboBoxNumberPriority.Size = new Size(242, 40);
            comboBoxNumberPriority.TabIndex = 3;
            // 
            // comboBoxLetterPriority
            // 
            comboBoxLetterPriority.FormattingEnabled = true;
            comboBoxLetterPriority.Location = new Point(260, 243);
            comboBoxLetterPriority.Name = "comboBoxLetterPriority";
            comboBoxLetterPriority.Size = new Size(242, 40);
            comboBoxLetterPriority.TabIndex = 4;
            // 
            // comboBoxSymbolPriority
            // 
            comboBoxSymbolPriority.FormattingEnabled = true;
            comboBoxSymbolPriority.Location = new Point(532, 335);
            comboBoxSymbolPriority.Name = "comboBoxSymbolPriority";
            comboBoxSymbolPriority.Size = new Size(242, 40);
            comboBoxSymbolPriority.TabIndex = 5;
            // 
            // textBoxOutput
            // 
            textBoxOutput.Location = new Point(86, 323);
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.ReadOnly = true;
            textBoxOutput.ScrollBars = ScrollBars.Vertical;
            textBoxOutput.Size = new Size(200, 78);
            textBoxOutput.TabIndex = 6;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBoxOutput);
            Controls.Add(comboBoxSymbolPriority);
            Controls.Add(comboBoxLetterPriority);
            Controls.Add(comboBoxNumberPriority);
            Controls.Add(buttonStartSymbols);
            Controls.Add(buttonStartLetters);
            Controls.Add(buttonStartNumbers);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonStartNumbers;
        private Button buttonStartLetters;
        private Button buttonStartSymbols;
        private ComboBox comboBoxNumberPriority;
        private ComboBox comboBoxLetterPriority;
        private ComboBox comboBoxSymbolPriority;
        private TextBox textBoxOutput;
    }
}
