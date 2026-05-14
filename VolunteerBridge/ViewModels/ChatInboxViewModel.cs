namespace VolunteerBridge.ViewModels
{
    public class ChatInboxRowViewModel
    {
        public int OtherUserId { get; set; }
        public string OtherUserName { get; set; } = string.Empty;
        public DateTime LastMessageAt { get; set; }
        public string LastMessagePreview { get; set; } = string.Empty;
        public int UnreadCount { get; set; }
    }

    public class ChatInboxViewModel
    {
        public List<ChatInboxRowViewModel> Threads { get; set; } = new();
        public int? ActiveUserId { get; set; }
        public string ActiveUserName { get; set; } = string.Empty;
    }
}
