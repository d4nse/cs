using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PART2
{
    public partial class BarRacing : Window
    {
        private List<ProgressBar> horses = new List<ProgressBar>();
        private CancellationTokenSource? cancTokSrc;
        private Random rng = new Random();
        private List<string> results = new List<string>();

        public BarRacing()
        {
            InitializeComponent();
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            stackPanel.Children.Clear();
            horses.Clear();
            results.Clear();
            cancTokSrc = new();
            for (int i = 0; i < 5; ++i)
            {
                var horse = new ProgressBar
                {
                    Width = 200,
                    Height = 30,
                    Maximum = 100,
                    Background = GetRandomColor(),
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(1)
                };
                stackPanel.Children.Add(horse);
                horses.Add(horse);
            }
            startBtn.IsEnabled = false;
            stopBtn.IsEnabled = true;
            StartRace(cancTokSrc.Token);
        }

        private async void StartRace(CancellationToken tok)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < horses.Count; i++)
            {
                int horseIndex = i;
                tasks.Add(Task.Run(() => RaceHorse(horses[horseIndex], horseIndex + 1, tok), tok));
            }
            await Task.WhenAll(tasks);
            StringBuilder b = new();
            b.Append("Results:\n");
            for (int i = 0; i < results.Count; ++i)
            {
                b.Append($"Finished {i + 1}th: {results[i]}\n");
            }
            MessageBox.Show(b.ToString());
            stopBtn.IsEnabled = false;
            startBtn.IsEnabled = true;
        }

        private void RaceHorse(ProgressBar horse, int horseNumber, CancellationToken tok)
        {
            double distanceCovered = 0;
            int speed;
            while (distanceCovered < 100 && !tok.IsCancellationRequested)
            {
                speed = rng.Next(1, 6);
                Thread.Sleep(100);
                distanceCovered += speed;
                Dispatcher.Invoke(() => horse.Value = Math.Min(distanceCovered, 100));
            }
            Dispatcher.Invoke(() => results.Add($"Horse {horseNumber}: {Math.Min(distanceCovered, 100)}%"));
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

