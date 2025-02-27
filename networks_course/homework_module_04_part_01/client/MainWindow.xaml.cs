using client.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace client;


public partial class MainWindow : Window {
    public Frame MainFrame { get { return mainFrame; } }

    private bool isDraggingHeader = false;
    private Point mouseStartPoint;

    public MainWindow() {
        InitializeComponent();
        mainFrame.Navigate(new Login());
    }


    public void ChangeHeaderRectangleFill(Color color) {
        headerRectangle.Fill = new SolidColorBrush(color);
    }

    // Event Handlers
    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        isDraggingHeader = true;
        mouseStartPoint = e.GetPosition(this);
        Mouse.Capture(sender as IInputElement);
    }

    private void Header_MouseMove(object sender, MouseEventArgs e) {
        if (isDraggingHeader) {
            Point pos = e.GetPosition(this);
            double off_x = pos.X - mouseStartPoint.X;
            double off_y = pos.Y - mouseStartPoint.Y;
            Left += off_x;
            Top += off_y;
        }
    }
    private void Header_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        isDraggingHeader = false;
        Mouse.Capture(null);
    }

    private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e) {
        WindowState = WindowState.Minimized;
    }

    private void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
        Close();
    }
}