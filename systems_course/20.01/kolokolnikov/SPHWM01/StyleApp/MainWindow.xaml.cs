using System.IO;
using System.IO.Pipes;
using System.Windows;

namespace StyleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeStyle_Click(object sender, RoutedEventArgs e)
        {
            using (var pipeClient = new NamedPipeClientStream(".", "colorChangePipe", PipeDirection.Out))
            {
                try
                {
                    pipeClient.Connect(500);
                    using (var writer = new StreamWriter(pipeClient))
                    {
                        writer.AutoFlush = true;
                        writer.WriteLine("ChangeColor");
                    }
                }
                catch (TimeoutException)
                {
                    MessageBox.Show("MainApp is not running or not responding.");
                }
            }
        }

    }
}