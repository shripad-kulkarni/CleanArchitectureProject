using Project.Domain.Primitives;

namespace Project.Domain.Entities
{
    public sealed class ChatMessage : Entity
    {
        private ChatMessage() { }

        public ChatMessage(
            string senderId,
            string receiverId,
            string content,
            string? fileUrl = null,
            string? fileName = null)
        {
            SenderId   = senderId;
            ReceiverId = receiverId;
            Content    = content.Trim();
            FileUrl    = fileUrl;
            FileName   = fileName;
        }

        public string  SenderId   { get; private set; } = string.Empty;
        public string  ReceiverId { get; private set; } = string.Empty;
        public string  Content    { get; private set; } = string.Empty;
        public bool    IsRead     { get; private set; }
        public string? FileUrl    { get; private set; }
        public string? FileName   { get; private set; }

        public bool HasFile => FileUrl is not null;

        public void MarkAsRead() => IsRead = true;
    }
}
