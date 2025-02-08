using System.IO;
using System.Windows;

namespace ExamApp
{

    public partial class App : Application
    {
        private const string mutexName = "ExamAppMutex";
        private Mutex? _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, mutexName, out bool createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Another instance is already running.");
                Current.Shutdown();
            }
            else if (e.Args.Contains("--headless"))
            {
                HandleConsoleMode(e.Args);
                Current.Shutdown();
            }
            else
                base.OnStartup(e);
        }

        private void HandleConsoleMode(string[] args)
        {
            try
            {
                var forbiddenWordsPath = args[Array.IndexOf(args, "--forbidden-words") + 1];
                var outputFolder = args[Array.IndexOf(args, "--output") + 1];
                var words = File.ReadAllLines(forbiddenWordsPath);
                ////var scanner = new ConsoleScanner();
                //scanner.RunScan(words, outputFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

}
