using System.ComponentModel;
using System.Windows;

namespace PART1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker primeGenWorker;
        private BackgroundWorker fibGenWorker;
        private ManualResetEvent primeGenPauseEvent;
        private ManualResetEvent fibGenPauseEvent;

        public MainWindow()
        {
            InitializeComponent();
            primeGenStopBtn.IsEnabled = false;
            primeGenPauseBtn.IsEnabled = false;
            primeGenResumeBtn.IsEnabled = false;
            fibGenStopBtn.IsEnabled = false;
            fibGenPauseBtn.IsEnabled = false;
            fibGenResumeBtn.IsEnabled = false;

            primeGenPauseEvent = new ManualResetEvent(true);
            fibGenPauseEvent = new ManualResetEvent(true);

            primeGenWorker = new BackgroundWorker();
            primeGenWorker.DoWork += PrimeGenWorker_DoWork;
            primeGenWorker.RunWorkerCompleted += PrimeGenWorker_RunWorkerCompleted;
            primeGenWorker.ProgressChanged += PrimeGenWorker_ProgressChanged;
            primeGenWorker.WorkerReportsProgress = true;
            primeGenWorker.WorkerSupportsCancellation = true;

            fibGenWorker = new BackgroundWorker();
            fibGenWorker.DoWork += FibGenWorker_DoWork;
            fibGenWorker.RunWorkerCompleted += FibGenWorker_RunWorkerCompleted;
            fibGenWorker.ProgressChanged += FibGenWorker_ProgressChanged;
            fibGenWorker.WorkerReportsProgress = true;
            fibGenWorker.WorkerSupportsCancellation = true;

        }

        private bool IsPrime(long value)
        {
            if (value <= 1) return false;
            if (value <= 3) return true;
            if (value % 2 == 0 || value % 3 == 0) return false;
            for (long i = 5; i * i <= value; i += 6)
            {
                if (value % i == 0 || value % (i + 2) == 0) return false;
            }
            return true;
        }

        // WORKER METHODS
        private void PrimeGenWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            (long lowerBound, long upperBound) = ((long, long))e.Argument!;
            long i = lowerBound;
            while (i <= upperBound)
            {
                if (primeGenWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                if (IsPrime(i))
                {
                    primeGenWorker.ReportProgress(0, i);
                }
                ++i;
                Thread.Sleep(10);
                primeGenPauseEvent.WaitOne();
            }
        }

        private void PrimeGenWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                primeGenOutputBox.AppendText("CANCELLED.\n");
                primeGenOutputBox.ScrollToEnd();
            }
            else if (e.Error != null)
            {
                primeGenOutputBox.AppendText($"{e.Error.Message.ToUpper()}.\n");
                primeGenOutputBox.ScrollToEnd();
            }
            else
            {
                primeGenOutputBox.AppendText("DONE.\n");
                primeGenOutputBox.ScrollToEnd();
            }
            primeGenStartBtn.IsEnabled = true;
            primeGenStopBtn.IsEnabled = false;
            primeGenPauseBtn.IsEnabled = false;
            primeGenResumeBtn.IsEnabled = false;
        }

        private void PrimeGenWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            string state = e.UserState?.ToString() ?? "ERROR";
            primeGenOutputBox.AppendText($"{state}\n");
            primeGenOutputBox.ScrollToEnd();
        }

        private void FibGenWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            (long lowerBound, long upperBound) = ((long, long))e.Argument!;
            long n = upperBound;
            if (n < 0)
            {
                fibGenOutputBox.AppendText("upper bound can't be < 0.");
                fibGenOutputBox.ScrollToEnd();
                return;
            }
            else if (n == 0 || n == 1)
            {
                fibGenWorker.ReportProgress(100, n);
                return;
            }
            double a = 0, b = 1, temp;
            while (b <= n)
            {
                if (fibGenWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                temp = a + b;
                a = b;
                b = temp;
                fibGenWorker.ReportProgress(0, b);
                Thread.Sleep(500);
                fibGenPauseEvent.WaitOne();
            }
        }

        private void FibGenWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                fibGenOutputBox.AppendText("CANCELLED.\n");
                fibGenOutputBox.ScrollToEnd();
            }
            else if (e.Error != null)
            {
                fibGenOutputBox.AppendText($"{e.Error.Message.ToUpper()}.\n");
                fibGenOutputBox.ScrollToEnd();
            }
            else
            {
                fibGenOutputBox.AppendText("DONE.\n");
                fibGenOutputBox.ScrollToEnd();
            }
            fibGenStartBtn.IsEnabled = true;
            fibGenStopBtn.IsEnabled = false;
            fibGenPauseBtn.IsEnabled = false;
            fibGenResumeBtn.IsEnabled = false;
        }

        private void FibGenWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            string state = e.UserState?.ToString() ?? "ERROR";
            fibGenOutputBox.AppendText($"{state}\n");
            fibGenOutputBox.ScrollToEnd();
        }

        // UI METHODS
        private void primeGenStartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!primeGenWorker.IsBusy)
            {
                primeGenOutputBox.Clear();
                long lowerBound;
                long upperBound;
                if (!long.TryParse(lowerBoundTextBox.Text, out lowerBound) || lowerBound < 2)
                    lowerBound = 2;
                if (!long.TryParse(upperBoundTextBox.Text, out upperBound))
                    upperBound = long.MaxValue;
                primeGenWorker.RunWorkerAsync((lowerBound, upperBound));
                primeGenStartBtn.IsEnabled = false;
                primeGenStopBtn.IsEnabled = true;
                primeGenPauseBtn.IsEnabled = true;
            }
        }

        private void primeGenStopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (primeGenWorker.IsBusy)
            {
                primeGenPauseEvent.Set();
                primeGenWorker.CancelAsync();
            }
        }

        private void primeGenPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            primeGenPauseEvent.Reset();
            primeGenOutputBox.AppendText("PAUSED.\n");
            primeGenOutputBox.ScrollToEnd();
            primeGenResumeBtn.IsEnabled = true;
            primeGenPauseBtn.IsEnabled = false;
        }

        private void primeGenResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            primeGenPauseEvent.Set();
            primeGenResumeBtn.IsEnabled = false;
            primeGenPauseBtn.IsEnabled = true;
        }

        private void fibGenStartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!fibGenWorker.IsBusy)
            {
                fibGenOutputBox.Clear();
                long lowerBound;
                long upperBound;
                if (!long.TryParse(lowerBoundTextBox.Text, out lowerBound) || lowerBound < 2)
                    lowerBound = 2;
                if (!long.TryParse(upperBoundTextBox.Text, out upperBound))
                    upperBound = long.MaxValue;
                fibGenWorker.RunWorkerAsync((lowerBound, upperBound));
                fibGenStartBtn.IsEnabled = false;
                fibGenStopBtn.IsEnabled = true;
                fibGenPauseBtn.IsEnabled = true;
            }
        }

        private void fibGenStopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (fibGenWorker.IsBusy)
            {
                fibGenPauseEvent.Set();
                fibGenWorker.CancelAsync();
            }
        }

        private void fibGenPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            fibGenPauseEvent.Reset();
            fibGenOutputBox.AppendText("PAUSED.\n");
            fibGenOutputBox.ScrollToEnd();
            fibGenResumeBtn.IsEnabled = true;
            fibGenPauseBtn.IsEnabled = false;
        }

        private void fibGenResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            fibGenPauseEvent.Set();
            fibGenResumeBtn.IsEnabled = false;
            fibGenPauseBtn.IsEnabled = true;
        }
    }
}