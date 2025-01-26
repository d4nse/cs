
using System.Windows;

namespace PART2
{
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

        }

        private void dancingProgBarsBtn_Click(object sender, RoutedEventArgs e)
        {
            DancingProgBars a = new();
            a.Show();
        }

        private void barRacingBtn_Click(object sender, RoutedEventArgs e)
        {
            BarRacing a = new();
            a.Show();
        }

        private void wordCounterBtn_Click(object sender, RoutedEventArgs e)
        {
            WordCounter a = new();
            a.Show();
        }
    }
}