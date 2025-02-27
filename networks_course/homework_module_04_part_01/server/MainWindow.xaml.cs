using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace server;

public partial class MainWindow : Window
{
    private Server server;
    private Logger logger;

    public MainWindow()
    {
        InitializeComponent();
        logger = new(ref logbox);
        logger.Info("Logger service started");
        server = new(ref logger);
    }


    // HEADER BAR
    private bool isDragging = false;
    private Point mouseStartPoint;


    private void header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        isDragging = true;
        mouseStartPoint = e.GetPosition(this);
        Mouse.Capture(sender as IInputElement);
    }

    private void header_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (isDragging)
        {
            Point pos = e.GetPosition(this);
            double off_x = pos.X - mouseStartPoint.X;
            double off_y = pos.Y - mouseStartPoint.Y;
            Left += off_x;
            Top += off_y;
        }
    }
    private void header_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        isDragging = false;
        Mouse.Capture(null);
    }

    private void minimize_window_btn_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void close_window_btn_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void main_window_Loaded(object sender, RoutedEventArgs e)
    {
        await server.Start();
    }
}

public class Logger
{
    private TextBox stdout;

    public Logger(ref TextBox stdout)
    {
        this.stdout = stdout;
    }
    public void Write(string message)
    {
        stdout.AppendText(message);
        stdout.AppendText("\n");
    }
    public void Log(string level, string message)
    {
        var time = DateTime.Now.ToString("HH:mm:ss");
        Write($"[ {time} {level} ]: {message}");
    }
    public void Info(string message)
    {
        Log("Info", message);
    }
    public void Error(string message)
    {
        Log("Error", message);
    }
    public void Warn(string message)
    {
        Log("Warning", message);
    }
}