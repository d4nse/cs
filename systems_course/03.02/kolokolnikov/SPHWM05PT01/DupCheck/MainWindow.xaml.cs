using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace DupCheck
{
    public partial class MainWindow : Window
    {
        List<string> chosenDirs = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void chooseDirsButton_Click(object sender, RoutedEventArgs e)
        {
            chosenDirs.Clear();
            chosenDirsBox.Text = string.Empty;
            var dialog = new OpenFolderDialog();
            dialog.Multiselect = true;
            dialog.ValidateNames = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if (dialog.ShowDialog() == true)
            {
                foreach (var dir in dialog.FolderNames)
                {
                    chosenDirsBox.Text += $"{dir}\n";
                    chosenDirs.Add(dir);
                }
                checkButton.IsEnabled = true;
            }
            else
            {
                checkButton.IsEnabled = false;
            }
        }

        private async void checkButton_Click(object sender, RoutedEventArgs e)
        {
            outputBox.Text = string.Empty;
            outputBox.Text += "Processing...\n";
            await Task.Run(() => FindDuplicates());
            outputBox.Text += "Processing complete.\n";
        }

        private void Print(string message)
        {
            Dispatcher.Invoke(() => outputBox.Text += message);
        }

        private void FindDuplicates()
        {
            if (chosenDirs.Count == 0)
            {
                Print("Directories not selected\n");
                return;
            }
            List<string> files = new();
            try
            {
                foreach (var dir in chosenDirs)
                {
                    var f = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                    foreach (var file in f)
                    {
                        files.Add(file);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Print($"No access to this directory.\n");
                return;
            }
            var fileGroups = new Dictionary<string, List<string>>();
            int curFile = 0;
            int totalFiles = chosenDirs.Count;
            Dispatcher.Invoke(() => progressBar.Value = 0);
            foreach (var file in files)
            {
                var hash = GetFileHash(file);
                if (!fileGroups.ContainsKey(hash))
                {
                    fileGroups[hash] = new List<string>();
                }
                fileGroups[hash].Add(file);
                Dispatcher.Invoke(() => progressBar.Value = (curFile / (double)totalFiles) * 100);
                curFile++;
            }
            bool dupsFound = false;
            foreach (var group in fileGroups.Values)
            {
                if (group.Count > 1)
                {
                    dupsFound = true;
                    string ogFile = group[0];
                    Print($"Found {group.Skip(1).Count()} duplicates of file `{ogFile}`: \n");
                    foreach (var dup in group.Skip(1))
                    {
                        Print($" - {dup}\n");
                    }
                }
            }
            if (!dupsFound)
            {
                Print("No duplicates found.\n");
            }
        }

        private string GetFileHash(string file)
        {
            using (var sha = SHA256.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    var hash = sha.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}