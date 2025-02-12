using System.Diagnostics;
using System.Windows;

namespace LimitedSelfSpawner
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            int count_inst = Process.GetProcessesByName(AppDomain.CurrentDomain.FriendlyName).Length;
            if (count_inst > 3)
            {
                MessageBox.Show("Cant run more than 3 instances.", "Limit Reached", MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
                return;
            }
            base.OnStartup(e);
        }

    }

}
