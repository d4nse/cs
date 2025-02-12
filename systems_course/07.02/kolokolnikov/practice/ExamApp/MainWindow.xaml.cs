using Microsoft.Win32;
using System.Collections.Concurrent;
using System.IO;
using System.Windows;

namespace ExamApp
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource? cts;
        private ManualResetEvent pauseEvent = new ManualResetEvent(true);
        public static string outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ForbiddenWordsScanner");
        private Task? procTask;
        private List<string> forbiddenWords = new();
        private ConcurrentBag<string> allFiles = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            pauseEvent.Set();
            if (cts == null)
            {
                cts = new();
                var tok = cts.Token;
                procTask = ProcessFiles(tok);
                AddLog("Processing started");
            }
            else
            {
                AddLog("Processing resumed");
            }
            startBtn.IsEnabled = false;
            stopBtn.IsEnabled = true;
            pauseBtn.IsEnabled = true;
            loadWordsBtn.IsEnabled = false;
        }

        private async void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            pauseEvent.Set();
            cts?.Cancel();
            try
            {
                if (procTask != null)
                {
                    await procTask;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                AddLog($"Error: {ex.Message}");
            }
            finally
            {
                procTask?.Wait();
                cts?.Dispose();
                cts = null;
                AddLog("Processing stopped");
                startBtn.IsEnabled = true;
                stopBtn.IsEnabled = false;
                pauseBtn.IsEnabled = false;
                loadWordsBtn.IsEnabled = true;
            }
        }

        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {
            pauseEvent.Reset();
            AddLog("Processing paused");
            startBtn.IsEnabled = true;
            stopBtn.IsEnabled = true;
            pauseBtn.IsEnabled = false;
        }

        private async void loadWordsBtn_Click(object sender, RoutedEventArgs e)
        {
            wordsFileBox.Text = string.Empty;
            forbiddenWords.Clear();
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text Files|*.txt";
            if (dialog.ShowDialog() == true)
            {
                wordsFileBox.Text = dialog.FileName;
                forbiddenWords = (await File.ReadAllLinesAsync(wordsFileBox.Text)).ToList();
                AddLog($"Read {forbiddenWords.Count} forbidden words from {wordsFileBox.Text}");
                startBtn.IsEnabled = true;
            }
            else
            {
                AddLog($"Could not read file: {wordsFileBox.Text}");
                startBtn.IsEnabled = false;
            }
        }

        private void AddLog(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                logBox.Text += $"{DateTime.Now.ToString("HH:mm:ss")}: {msg}\n";
            });
        }

        private void SearchFiles(string path, CancellationToken tok)
        {
            try
            {
                AddLog($"Searching for files in {path}");
                Parallel.ForEach(Directory.GetDirectories(path), subDir =>
                {
                    pauseEvent.WaitOne();
                    if (tok.IsCancellationRequested) return;
                    SearchFiles(subDir, tok);
                });
                foreach (var file in Directory.GetFiles(path))
                {
                    pauseEvent.WaitOne();
                    if (tok.IsCancellationRequested) return;
                    allFiles.Add(file);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (Exception ex)
            {
                AddLog($"Error accessing {path}: {ex.Message}");
            }
        }

        private async Task ProcessFiles(CancellationToken tok)
        {
            List<Task> tasks = new();
            // Getting all files on system will be EXTREMELY slow, im not going to do that.

            pauseEvent.WaitOne();
            if (tok.IsCancellationRequested) return;
            tasks.Add(Task.Run(() => SearchFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), tok), tok));

            await Task.WhenAll(tasks);
            AddLog($"Total {allFiles.Count} files found");
            foreach (var file in allFiles)
            {
                pauseEvent.WaitOne();
                if (tok.IsCancellationRequested) break;
                await ProcessFile(tok, file);
            }
            AddLog($"Processing finished. Files saved to {outputDir}");
        }

        private async Task ProcessFile(CancellationToken tok, string file)
        {
            pauseEvent.WaitOne();
            if (tok.IsCancellationRequested) return;
            string content = await File.ReadAllTextAsync(file, tok);
            int count = 0;
            AddLog($"Searching for forbidden words in {file}");
            foreach (var word in forbiddenWords)
            {
                pauseEvent.WaitOne();
                if (tok.IsCancellationRequested) return;
                if (content.Contains(word))
                {
                    content = content.Replace(word, "[CENSORED]");
                    count++;
                }
            }
            if (count > 0)
            {
                string dest = Path.Combine(outputDir, Path.GetFileName(file));
                await File.WriteAllTextAsync(dest, content, tok);
                AddLog($"Found {count} forbidden words in {file}, written to {dest}");
            }
        }
    }
}