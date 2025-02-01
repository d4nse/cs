using System;
using System.Threading;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxNumberPriority.Items.AddRange(Enum.GetNames(typeof(ThreadPriority)));
            comboBoxLetterPriority.Items.AddRange(Enum.GetNames(typeof(ThreadPriority)));
            comboBoxSymbolPriority.Items.AddRange(Enum.GetNames(typeof(ThreadPriority)));

            comboBoxNumberPriority.SelectedIndex = 2;
            comboBoxLetterPriority.SelectedIndex = 2;
            comboBoxSymbolPriority.SelectedIndex = 2;
        }

        private void buttonStartNumbers_Click(object sender, EventArgs e)
        {
            Thread numberThread = new Thread(GenerateNumbers);
            numberThread.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), comboBoxNumberPriority.SelectedItem.ToString());
            numberThread.Start();
        }

        private void buttonStartLetters_Click(object sender, EventArgs e)
        {
            Thread letterThread = new Thread(GenerateLetters);
            letterThread.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), comboBoxLetterPriority.SelectedItem.ToString());
            letterThread.Start();
        }

        private void buttonStartSymbols_Click(object sender, EventArgs e)
        {
            Thread symbolThread = new Thread(GenerateSymbols);
            symbolThread.Priority = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), comboBoxSymbolPriority.SelectedItem.ToString());
            symbolThread.Start();
        }

        private void GenerateNumbers()
        {
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                int number = random.Next(0, 100);
                UpdateTextBox($"Number: {number}");
                Thread.Sleep(100);
            }
        }

        private void GenerateLetters()
        {
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                char letter = (char)random.Next('A', 'Z' + 1);
                UpdateTextBox($"Letter: {letter}");
                Thread.Sleep(100);
            }
        }

        private void GenerateSymbols()
        {
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                char symbol = (char)random.Next(33, 48);
                UpdateTextBox($"Symbol: {symbol}");
                Thread.Sleep(100);
            }
        }

        private void UpdateTextBox(string text)
        {
            if (textBoxOutput.InvokeRequired)
            {
                textBoxOutput.Invoke(new Action<string>(UpdateTextBox), text);
            }
            else
            {
                textBoxOutput.AppendText(text + Environment.NewLine);
            }
        }
    }
}