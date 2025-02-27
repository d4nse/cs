using System.Windows;
using System.Windows.Controls;

namespace client.Pages;


public partial class ErrorPage : Page {
    private readonly MainWindow mainWindow;
    private readonly Frame mainFrame;
    private Page? returnPage;

    public ErrorPage(string title, string message) {
        InitializeComponent();
        mainWindow = (Application.Current.MainWindow as MainWindow)!;
        mainFrame = mainWindow.MainFrame;
        RetryButton.Visibility = Visibility.Hidden;
        RetryButton.IsEnabled = false;
        ErrorTitle.Text = title;
        ErrorMessage.Text = message;
    }

    public ErrorPage WithRetry(Page returnPage) {
        RetryButton.Visibility = Visibility.Visible;
        RetryButton.IsEnabled = true;
        this.returnPage = returnPage;
        return this;
    }

    private void RetryButton_Click(object sender, RoutedEventArgs e) {
        if (returnPage != null)
            mainFrame.Navigate(returnPage);
    }
}
