using System.Windows;

namespace client;

public enum InputDialogType {
    CreateChat,
    FindChat
}

public partial class InputDialog : Window {
    public string ChatTitle = string.Empty;
    public List<string>? Whitelist = null;

    public InputDialog(InputDialogType type) {
        InitializeComponent();
        switch (type) {
        case InputDialogType.CreateChat:
            WhitelistInput.IsEnabled = true;
            WhitelistInput.Visibility = Visibility.Visible;
            WhitelistLabel.IsEnabled = true;
            WhitelistLabel.Visibility = Visibility.Visible;
            break;
        case InputDialogType.FindChat:
            WhitelistInput.IsEnabled = false;
            WhitelistInput.Visibility = Visibility.Hidden;
            WhitelistLabel.IsEnabled = false;
            WhitelistLabel.Visibility = Visibility.Hidden;
            break;
        default:
            throw new Exception("Unknown dialog type");
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) {
        DialogResult = false;
        Close();
    }

    private void AcceptButton_Click(object sender, RoutedEventArgs e) {
        ChatTitle = ChatTitleInput.Text;
        if (WhitelistInput.Text != string.Empty)
            Whitelist = WhitelistInput.Text
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .ToList();
        DialogResult = true;
        Close();
    }
}
