using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PART2
{
    public partial class DancingProgBars : Window
    {
        private List<ProgressBar> bars = new List<ProgressBar>();
        private CancellationTokenSource? cancTokSrc;
        private Random rng = new Random();

        public DancingProgBars()
        {
            InitializeComponent();
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(barsCountBox.Text, out int countBars) || countBars <= 0)
            {
                MessageBox.Show("Please enter a positive number.");
                return;
            }
            stackPanel.Children.Clear();
            bars.Clear();
            cancTokSrc = new();
            for (int i = 0; i < countBars; ++i)
            {
                var bar = new ProgressBar
                {
                    Width = 25,
                    Height = stackPanel.Height,
                    Maximum = 100,
                    Background = GetRandomColor(),
                    Foreground = GetRandomColor(),
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(1)
                };
                stackPanel.Children.Add(bar);
                bars.Add(bar);
            }
            startBtn.IsEnabled = false;
            stopBtn.IsEnabled = true;
            StartFillingBars(cancTokSrc.Token);
        }

        private async void StartFillingBars(CancellationToken tok)
        {
            var tasks = new List<Task>();
            foreach (var bar in bars)
            {
                tasks.Add(Task.Run(() => FillBar(bar, tok), tok));
            }
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {

            }

        }

        private void FillBar(ProgressBar bar, CancellationToken tok)
        {
            double value = 0;
            while (!tok.IsCancellationRequested)
            {
                while (value < 100 && !tok.IsCancellationRequested)
                {
                    Thread.Sleep(rng.Next(10, 100));
                    value += rng.Next(1, 10);
                    Dispatcher.Invoke(() => bar.Value = Math.Min(value, 100));
                }
                Dispatcher.Invoke(() => bar.Background = GetRandomColor());
                while (value > 0 && !tok.IsCancellationRequested)
                {
                    Thread.Sleep(rng.Next(10, 100));
                    value -= rng.Next(1, 10);
                    Dispatcher.Invoke(() => bar.Value = Math.Min(value, 100));
                }
                Dispatcher.Invoke(() => bar.Foreground = GetRandomColor());
            }
        }

        private SolidColorBrush GetRandomColor()
        {
            byte r = (byte)rng.Next(256);
            byte g = (byte)rng.Next(256);
            byte b = (byte)rng.Next(256);
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            cancTokSrc?.Cancel();
            startBtn.IsEnabled = true;
            stopBtn.IsEnabled = false;
        }
    }
}
