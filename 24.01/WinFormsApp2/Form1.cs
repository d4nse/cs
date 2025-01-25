using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            refreshTimer = new System.Windows.Forms.Timer(); // Инициализация
            refreshTimer.Tick += new EventHandler(RefreshTimer_Tick);
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void RefreshProcessList()
        {
            listBoxProcesses.Items.Clear();
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                listBoxProcesses.Items.Add($"{process.ProcessName} (ID: {process.Id})");
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxInterval.Text, out int interval) && interval > 0)
            {
                refreshTimer.Interval = interval * 1000;
                refreshTimer.Start();
                buttonStart.Enabled = false;
                textBoxInterval.Enabled = false;
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректный интервал обновления (в секундах).");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshProcessList();
        }
    }
}