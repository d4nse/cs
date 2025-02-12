using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hWnd, string lpString);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("kernel32.dll")]
        public static extern bool Beep(uint dwFreq, uint dwDuration);

        [DllImport("user32.dll")]
        public static extern void MessageBeep(uint uType);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        private NamedPipeServerStream piper;


        public MainWindow()
        {
            InitializeComponent();
            Thread pipeThread = new Thread(StartPipeServer);
            pipeThread.Start();
        }

        private void StartPipeServer()
        {

            while (true)
            {
                piper = new NamedPipeServerStream("colorChangePipe", PipeDirection.In);
                piper.WaitForConnection();
                using (var reader = new StreamReader(piper))
                {
                    string msg = reader.ReadLine() ?? "";
                    if (msg == "ChangeColor")
                    {
                        RandomColor();
                    }
                }
                try
                {
                    piper.Disconnect();
                }
                catch (Exception) { }
            }
        }

        private void task1button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox(IntPtr.Zero, "Name: Vladimir 'D4NSE' Makarov", "Information", 0);
            MessageBox(IntPtr.Zero, "Age: 23", "Information", 0);
            MessageBox(IntPtr.Zero, "Occupation: Developer", "Information", 0);
            MessageBox(IntPtr.Zero, "Location: Unknown", "Information", 0);
        }

        private void task2button_Click(object sender, RoutedEventArgs e)
        {
            string title = GetCurrentWindowTitle();
            IntPtr hWnd = FindWindow(null, title);
            if (hWnd != IntPtr.Zero)
            {
                var dialog = new InputDialog("Enter new name for window: ");
                bool? result = dialog.ShowDialog();
                string newName = result == true ? dialog.InputText : string.Empty;
                if (!string.IsNullOrEmpty(newName))
                {
                    SetWindowText(hWnd, newName);
                    MessageBox(IntPtr.Zero, "Window renamed successfully!", "Success", 0);
                }
                else
                {
                    MessageBox(IntPtr.Zero, "Window was not renamed.", "Error", 0);
                }
            }
            else
            {
                MessageBox(IntPtr.Zero, $"{title} window not found.", "Error", 0);
            }
        }

        private async void task3button_Click(object sender, RoutedEventArgs e)
        {
            MessageBeep(0);
            await Task.Delay(500);

            // phrase 1
            await PlayNote(Notes.E4, 500, 0);
            await PlayNote(Notes.FSharp4, 500, 0);
            await PlayNote(Notes.G4, 500, 0);
            await PlayNote(Notes.A4, 500, 0);
            await PlayNote(Notes.B4, 750, 0);

            // phrase 2
            await PlayNote(Notes.E5, 500, 0);
            await PlayNote(Notes.D5, 500, 0);
            await PlayNote(Notes.B4, 500, 50);
            await PlayNote(Notes.E4, 750, 0);

            // phrase 3
            await PlayNote(Notes.B4, 500, 0);
            await PlayNote(Notes.A4, 500, 0);
            await PlayNote(Notes.G4, 500, 0);
            await PlayNote(Notes.FSharp4, 500, 0);

            // phrase 1
            await PlayNote(Notes.E4, 500, 0);
            await PlayNote(Notes.FSharp4, 500, 0);
            await PlayNote(Notes.G4, 500, 0);
            await PlayNote(Notes.A4, 500, 0);
            await PlayNote(Notes.B4, 750, 0);

            // phrase 4
            await PlayNote(Notes.B4, 500, 0);
            await PlayNote(Notes.A4, 500, 0);
            await PlayNote(Notes.G4, 500, 0);
            await PlayNote(Notes.FSharp4, 500, 0);
            await PlayNote(Notes.G4, 500, 0);
            await PlayNote(Notes.A4, 500, 0);
            await PlayNote(Notes.B4, 750, 0);
        }

        private void task4button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox(IntPtr.Zero, "To test task #4 you need to run both projects together and press the button in the StyleApp.", "Information", 0);
        }

        public void RandomColor()
        {
            // Ensure the operation is performed on the UI thread
            if (Dispatcher.CheckAccess())
            {
                // We're on the UI thread, update the background color
                Random random = new Random();
                byte r = (byte)random.Next(256);
                byte g = (byte)random.Next(256);
                byte b = (byte)random.Next(256);
                Color randomColor = Color.FromRgb(r, g, b);
                Background = new SolidColorBrush(randomColor);
            }
            else
            {
                // Dispatch the operation to the UI thread
                Dispatcher.Invoke(() => RandomColor());
            }
        }

        private string GetCurrentWindowTitle()
        {
            IntPtr hWnd = GetForegroundWindow();
            StringBuilder title = new StringBuilder(256);
            if (GetWindowText(hWnd, title, title.Capacity) > 0)
            {
                return title.ToString();
            }
            else
            {
                return "Error retrieving title";
            }
        }

        public async Task PlayNote(Notes note, uint len, int delay)
        {
            Beep(((uint)note), len);
            await Task.Delay(delay);
        }

        public enum Notes
        {
            C4 = 262,     // 261.63
            C5 = 523,     // 523.25
            CSharp4 = 277, // 277.18
            CSharp5 = 554, // 554.37
            D4 = 294,     // 293.66
            D5 = 587,     // 587.33
            DSharp4 = 311, // 311.13
            DSharp5 = 622, // 622.25
            E4 = 330,     // 329.63
            E5 = 659,     // 659.25
            F4 = 349,     // 349.23
            F5 = 698,     // 698.46
            FSharp4 = 370, // 369.99
            FSharp5 = 740, // 739.99
            G4 = 392,     // 392
            G5 = 784,     // 783.99
            GSharp4 = 415, // 415.3
            GSharp5 = 831, // 830.61
            A4 = 440,     // 440
            A5 = 880,     // 880
            ASharp4 = 466, // 466.16
            ASharp5 = 932, // 932.33
            B4 = 494,     // 493.88
            B5 = 988,     // 987.77
        }

    }
}