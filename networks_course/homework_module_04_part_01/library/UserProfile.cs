namespace library;

public struct UserProfile {
    public string Username { get; set; }
    public List<string> SubscribedChats { get; set; }
}

public struct Chatroom {
    public string Title { get; set; }
    public List<string> ChatUsernames { get; set; }
    public bool IsWhitelisted { get; set; }

}
