using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace PART2
{
    public partial class WordCounter : Window
    {
        private string selectedDirectoryPath = string.Empty;

        public WordCounter()
        {
            InitializeComponent();
        }

        private void selectDirectoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog { Description = "Select a directory" };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedDirectoryPath = dialog.SelectedPath;
                selectedDirectoryLabel.Content = $"Selected directory: {selectedDirectoryPath}";
            }
        }

        private async void selectWordBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDirectoryPath)) return;
            outputBox.Clear();
            string word = selectWordBox.Text.Trim();
            if (string.IsNullOrEmpty(word)) return;

            outputBox.AppendText("Counting...\n");
            var results = await Task.Run(() => CountWordsInDirectory(selectedDirectoryPath, word));
            foreach (var result in results)
            {
                outputBox.AppendText(result + "\n");
            }
        }

        private List<string> CountWordsInDirectory(string directoryPath, string word)
        {
            List<string> results = new List<string>();
            try
            {
                var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    string fileContent = File.ReadAllText(file);
                    int count = CountWordsInFile(fileContent, word);
                    results.Add($"File Name: {Path.GetFileName(file)}\nPath: {file}\nCount occurrences: {count}");
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => outputBox.AppendText($"Error accessing directory: {ex.Message}\n"));
            }
            return results;
        }

        private int CountWordsInFile(string fileContent, string word)
        {
            var pattern = $@"\b{Regex.Escape(word)}\b";
            var matches = Regex.Matches(fileContent, pattern, RegexOptions.IgnoreCase);
            return matches.Count;
        }
    }
}
