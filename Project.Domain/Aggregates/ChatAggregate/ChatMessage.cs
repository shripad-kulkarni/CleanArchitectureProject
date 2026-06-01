using Project.Domain.Primitives;

namespace Project.Domain.Aggregates.ChatAggregate
{
    public sealed class ChatMessage : AggregateRoot
    {
        private ChatMessage() { }

        public string  SenderId   { get; private set; } = string.Empty;
        public string  ReceiverId { get; private set; } = string.Empty;
        public string  Content    { get; private set; } = string.Empty;
        public bool    IsRead     { get; private set; }
        public string? FileUrl    { get; private set; }
        public string? FileName   { get; private set; }

        public bool HasFile => FileUrl is not null;

        public static ChatMessage Create(
            string senderId,
            string receiverId,
            string content,
            string? fileUrl = null,
            string? fileName = null)
        {
            if (string.IsNullOrWhiteSpace(senderId))   throw new ArgumentException("SenderId is required.");
            if (string.IsNullOrWhiteSpace(receiverId)) throw new ArgumentException("ReceiverId is required.");

            // A message must have text, a file, or both
            var hasContent = !string.IsNullOrWhiteSpace(content);
            var hasFile    = !string.IsNullOrWhiteSpace(fileUrl);
            if (!hasContent && !hasFile)
                throw new ArgumentException("Message must have content or a file.");

            return new ChatMessage
            {
                SenderId   = senderId,
                ReceiverId = receiverId,
                Content    = content.Trim(),
                FileUrl    = fileUrl,
                FileName   = fileName
            };
        }

        public void MarkAsRead() => IsRead = true;
    }
}
