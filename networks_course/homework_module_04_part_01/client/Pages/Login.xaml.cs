using library;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace client.Pages;

public partial class Login : Page {
    private readonly MainWindow mainWindow;
    private readonly Frame mainFrame;

    [GeneratedRegex(@"[^1-9]")]
    private static partial Regex ValidatePortRegex();
    [GeneratedRegex(@"^(?=.*[A-Z])(?=.*\d)")]
    private static partial Regex ValidatePasswordRegex();

    public Login() {
        InitializeComponent();
        mainWindow = (Application.Current.MainWindow as MainWindow)!;
        mainFrame = mainWindow.MainFrame;
        mainWindow.ChangeHeaderRectangleFill(Color.FromRgb(255, 255, 255));
        // add handlers
        UsernameInput.KeyDown += (sender, e) => FocusOnEnter(sender, e, PasswordInput);
        PasswordInput.KeyDown += (sender, e) => FocusOnEnter(sender, e, SignInButton);
        HostnameInput.KeyDown += (sender, e) => FocusOnEnter(sender, e, PortInput);
        PortInput.KeyDown += (sender, e) => FocusOnEnter(sender, e, SignInButton);
        PortInput.PreviewTextInput += (sender, e) => e.Handled = ValidatePortRegex().IsMatch(e.Text);
    }

    private void DisplayErrorHint(string hint, Control? errorSource = null) {
        HintMessage.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
        HintMessage.Text = hint;
        if (errorSource != null && errorSource.Focusable) errorSource.Focus();
    }

    private void HideErrorHint() {
        HintMessage.Background = Brushes.Transparent;
        HintMessage.Text = string.Empty;
    }

    private bool TryGetUsername(out string username) {
        username = UsernameInput.Text;
        if (username == string.Empty || username.Contains(' ') || username.Length > 127) {
            DisplayErrorHint("Username can not be empty, contain a space, or be longer than 127 characters.", UsernameInput);
            return false;
        }
        return true;
    }


    private bool TryGetHashword(out string hashword) {
        var password = PasswordInput.SecurePassword;
        PasswordInput.Clear();
        hashword = string.Empty;
        nint ptr = Marshal.SecureStringToGlobalAllocUnicode(password);
        string naked_password = Marshal.PtrToStringUni(ptr) ?? string.Empty;
        Marshal.ZeroFreeGlobalAllocUnicode(ptr);
        if (naked_password.Length < 8 || !ValidatePasswordRegex().IsMatch(naked_password)) {
            DisplayErrorHint("Password must be longer than 8 character, contain an upper case letter and a digit.");
            return false;
        }
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(naked_password));
        StringBuilder ss = new();
        foreach (var b in bytes) { ss.Append(b.ToString("x2")); }
        hashword = ss.ToString();
        return true;
    }

    private bool TryGetInstance(out IPEndPoint? instance) {
        instance = null;
        if (!int.TryParse(PortInput.Text, out int port) || (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)) {
            DisplayErrorHint("Port number out of range. Valid range is 0 to 65535.", HostnameInput);
            return false;
        }
        string hostname = HostnameInput.Text;
        IPAddress? ipAddress;
        try { ipAddress = (Dns.GetHostAddresses(hostname))[0]; } catch (Exception ex) {
            DisplayErrorHint($"Failed to resolve hostname: {ex.Message}", HostnameInput);
            return false;
        }
        instance = new IPEndPoint(ipAddress, port);
        return true;
    }

    // Event Handlers
    private async void SignInButton_Click(object sender, RoutedEventArgs e) {
        if (TryGetUsername(out var username) && TryGetHashword(out var hashword) && TryGetInstance(out var instance)) {
            if (username == null || hashword == null || instance == null) throw new Exception("SignIn Details are invalid, which is unexpected.");
            SignInButton.IsEnabled = false;
            HideErrorHint();
            try {
                var client = new ClientHandler();
                await client.ConnectAsync(instance);
                var request = new AuthenticateUserRequest(username, hashword);
                var response = await client.Request(request);
                if (response.Success) {
                    var profile = response.ContentAs<UserProfile>();
                    var page = new Chats(client, profile);
                    mainFrame.Navigate(page);
                } else {
                    throw new Exception(response.Text);
                }
            } catch (Exception ex) {
                var page = new ErrorPage("Error occured", ex.Message).WithRetry(this);
                mainFrame.Navigate(page);
            } finally {
                SignInButton.IsEnabled = true;
            }
        }
    }

    private static void FocusOnEnter(object? sender, KeyEventArgs e, Control focusTarget) {
        _ = sender;
        if (e.Key == Key.Enter || e.Key == Key.Tab) {
            e.Handled = true;
            if (focusTarget.Focusable) focusTarget.Focus();
        }
    }


}
