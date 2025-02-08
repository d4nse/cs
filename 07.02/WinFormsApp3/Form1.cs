using System.Drawing.Text;
using System.Threading;

namespace WinFormsApp3
{
    public partial class Form1 : Form
    {
        private List<string> forbiddenWords;
        private string reportFile = "report.txt";
        private CancellationTokenSource cancellationTokenSource;
        private Task processingTask;

        public Form1()
        {
            InitializeComponent();
            forbiddenWords = new List<string>();
        }

        private void btnLoadForbiddenWords_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.int|All files(*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    forbiddenWords = File.ReadAllLines(openFileDialog.FileName).ToList();
                    MessageBox.Show("слова загружены");
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnPause.Enabled = true;
            btnStop.Enabled = true;

            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            processingTask = Task.Run(() => ProcessFiles(token), token);
        }

        private void ProcessFiles(CancellationToken token)
        {
            string[] drives = Directory.GetLogicalDrives();
            foreach (var drive in drives)
            {
                if (token.IsCancellationRequested) break;
                ProcessDirectory(drive, token);
            }

            MessageBox.Show("Обработка заверешна");
        }

        private void ProcessDirectory(string path, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            foreach (var filePath in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                if (token.IsCancellationRequested) break;
                ProcessFile(filePath);
            }
        }

        private void ProcessFile(string filePath)
        {
            string content = File.ReadAllText(filePath);
            var modifiedContent = content;
            int replacementCount = 0;

            foreach (var word in forbiddenWords)
            {
                if (modifiedContent.Contains(word))
                {
                    modifiedContent = modifiedContent.Replace(word, "*******");
                    replacementCount++;
                }
            }

            if (replacementCount > 0)
            {
                string originalFilePath = Path.Combine("C:\\ForbiddenFiles", Path.GetFileName(filePath));
                string modifiedFilePath = Path.Combine("C:\\ModifiedFiles", Path.GetFileName(filePath));

                File.Copy(filePath, originalFilePath);

                File.WriteAllText(modifiedFilePath, modifiedContent);

                LogFileDetails(filePath, replacementCount);
            }
        }

        private void LogFileDetails(string filePath, int replacementCount)
        {
            string reportLine = $"{filePath}, {new FileInfo(filePath).Length}, {replacementCount} replacemnts";
            File.AppendAllText(reportLine, reportLine + Environment.NewLine);
        }
        private void btnPause_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            btnPause.Enabled = false;
            btnStart.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            processingTask.Wait();
            btnStart.Enabled = true;
            btnPause.Enabled = false;
            btnStop.Enabled = false;
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            var topWords = forbiddenWords.GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new { Word = g.Key, Count = g.Count() })
                .ToList();
            string report = "топ 10 запрещенных слов:\n";
            foreach (var word in topWords)
            {
                report += $"{word.Word}: {word.Count}\n";
            }

            File.WriteAllText("report.txt", report);
            MessageBox.Show("отчет создан");
        }

    }
}
