using System.Diagnostics;
using System.Windows;

namespace LimitedSelfSpawner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            int count_inst = Process.GetProcessesByName(AppDomain.CurrentDomain.FriendlyName).Length;
            instanceLabel.Content = $"Instance #{count_inst}";
        }

        private void spawnInstanceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var thisProc = Process.GetCurrentProcess();
                var thisExec = thisProc?.MainModule?.FileName;
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = thisExec,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using (Process proc = new())
                {
                    proc.StartInfo = info;
                    proc.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong: {ex.Message}");
            }
        }
    }
}