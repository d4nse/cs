using library;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace client.Pages;

public partial class Chats : Page {
    private MainWindow mainWindow;
    private Frame mainFrame;
    private ClientHandler Client;
    private UserProfile Profile;
    public ObservableCollection<ChatMessage> Messages { get; set; }
    private string SelectedChat;
    public Dictionary<string, ObservableCollection<ChatMessage>> RoomMessages { get; set; } = new();
    private ChatMessage LastMessageSent;
    private string WhateverThisIs = string.Empty;
    private ObservableCollection<string> SubscribedChats;

    public Chats(ClientHandler client, UserProfile profile) {
        InitializeComponent();
        mainWindow = (Application.Current.MainWindow as MainWindow)!;
        mainFrame = mainWindow.MainFrame;
        mainWindow.ChangeHeaderRectangleFill(Color.FromRgb(230, 230, 230));

        Client = client;
        Profile = profile;
        Messages = [];
        Messages.CollectionChanged += Messages_CollectionChanged;
        DataContext = this;
        SubscribedChats = new(Profile.SubscribedChats);

        ChatListBox.ItemsSource = SubscribedChats;
        LoadChat(SubscribedChats[0]);
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e) {
        await HandleChatsAsync();
    }

    private async Task HandleChatsAsync() {
        try {
            while (true) {
                var response = await Client.ListenForResponse();
                switch (response.Type) {
                case ResponseType.IncomingMessage: HandleIncomingMessage(response); break;
                case ResponseType.SentMessageReceived: HandleSentMessageReceived(response); break;
                case ResponseType.ChatCreated: HandleChatCreated(response); break;
                case ResponseType.ChatFound: HandleChatFound(response); break;
                default: HandleUnknownResponse(response); break;
                }
            }
        } catch (Exception ex) {
            MessageBox.Show($"Error occured: {ex.Message} : {ex.StackTrace}");
        }
    }

    private void HandleIncomingMessage(Response response) {
        if (response.Success) {
            var message = response.ContentAs<ChatMessage>();
            message.IsSentByUser = false;
            RoomMessages[SelectedChat].Add(message);
        } else {
            MessageBox.Show(response.Text);
        }
    }

    private void HandleSentMessageReceived(Response response) {
        LastMessageSent.IsSentByUser = true;
        LastMessageSent.Text = Regex.Replace(LastMessageSent.Text, response.Text, "[FILTERED]", RegexOptions.IgnoreCase);
        RoomMessages[SelectedChat].Add(LastMessageSent);
    }

    private void HandleUnknownResponse(Response response) {
        throw new NotImplementedException();
    }

    private void HandleChatCreated(Response response) {
        if (response.Success) {
            SubscribedChats.Add(WhateverThisIs);
            LoadChat(WhateverThisIs);
        } else {
            MessageBox.Show(response.Text);
        }
    }

    private void HandleChatFound(Response response) {
        if (response.Success) {
            SubscribedChats.Add(WhateverThisIs);
            LoadChat(WhateverThisIs);
        } else {
            MessageBox.Show(response.Text);
        }
    }

    private bool TryGetMessageFromInput(out ChatMessage message) {
        message = new ChatMessage() {
            From = Profile.Username,
            Text = ChatInputBox.Text,
            Timestamp = DateTime.Now,
            IsSentByUser = true // REMOVE THIS SOMEHOW LATER
        };
        ChatInputBox.Text = string.Empty;
        if (string.IsNullOrEmpty(message.Text)) {
            return false;
        }
        return true;
    }

    private void LoadChat(string chatTitle) {
        SelectedChat = chatTitle;
        ChatTitleBlock.Text = chatTitle;
        if (!RoomMessages.ContainsKey(chatTitle)) {
            RoomMessages.Add(chatTitle, new());
        }
        ChatMessageBox.ItemsSource = RoomMessages[chatTitle];
    }

    // Event Handlers
    private void Messages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            var messages = RoomMessages[SelectedChat];
            if (messages != null && messages.Count > 0) {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    ChatMessageBox.ScrollIntoView(messages[^1]);
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }

    private async void ChatInputBox_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            if (TryGetMessageFromInput(out ChatMessage message)) {
                var request = new SendMessageRequest() {
                    ChatTitle = SelectedChat,
                    Message = message,
                };
                LastMessageSent = message;
                await Client.WriteAsync(request.ToJson());
            }
        }
    }

    private async void CreateChatButton_Click(object sender, RoutedEventArgs e) {
        var dialog = new InputDialog(InputDialogType.CreateChat);
        if (dialog.ShowDialog() == true) {
            WhateverThisIs = dialog.ChatTitle;
            if (dialog.Whitelist != null)
                dialog.Whitelist.Add(Profile.Username);
            var request = new CreateChatRequest() {
                ChatTitle = dialog.ChatTitle,
                Whitelist = dialog.Whitelist,
            };
            await Client.WriteAsync(request.ToJson());
        }
    }

    private async void FindChatButton_Click(object sender, RoutedEventArgs e) {
        var dialog = new InputDialog(InputDialogType.FindChat);
        if (dialog.ShowDialog() == true) {
            WhateverThisIs = dialog.ChatTitle;
            var request = new FindChatRequest() {
                ChatTitle = dialog.ChatTitle,
            };
            await Client.WriteAsync(request.ToJson());
        }

    }

    private void ChatListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (e.AddedItems != null && e.AddedItems.Count > 0) {
            string chatTitle = e.AddedItems[0]?.ToString()!;
            LoadChat(chatTitle);
        }
    }
}

// UI Converters

public class BubbleBackgroundConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return (bool)value ? Brushes.LightBlue : Brushes.LightGray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

public class HorizontalAlignConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
