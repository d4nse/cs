using System.Windows;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public string InputText { get; private set; }
        public string Message { get; set; }

        public InputDialog(string message)
        {
            InitializeComponent();
            InputText = "";
            Message = message;
            DataContext = this;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            InputText = InputTextBox.Text;
            DialogResult = true;
            Close();
        }
    }
}
