namespace library;

public struct ChatMessage
{
    public string From { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsSentByUser { get; set; }
}
