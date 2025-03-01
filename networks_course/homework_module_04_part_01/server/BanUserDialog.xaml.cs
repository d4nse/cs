using System.Windows;

namespace server;

public partial class BanUserDialog : Window {
    public string OffendersUsername { get; set; } = string.Empty;
    public DateTime? DateOfUnban { get; set; }

    public BanUserDialog() {
        InitializeComponent();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) {
        DialogResult = false;
        Close();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e) {
        var username = OffendersUsernameInput.Text;
        var date = DateOfUnbanPicker.SelectedDate;
        if (username == string.Empty) {
            MessageBox.Show("Please, enter the offender's username or cancel.");
            return;
        }
        if (date < DateTime.Now) {
            MessageBox.Show("Please enter a valid date, the date you've selected has passed and will never come.");
            return;
        }
        if (date == null) {
            MessageBox.Show("Keep in mind, not selecting the date will result in a permanent ban.");
        }
        OffendersUsername = username;
        DateOfUnban = date;
        DialogResult = true;
        Close();
    }
}
