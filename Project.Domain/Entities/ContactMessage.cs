using Project.Domain.Primitives;

namespace Project.Domain.Entities
{
    public sealed class ContactMessage : Entity
    {
        private ContactMessage() { }

        public ContactMessage(string name, string email, string? phone, string subject, string message)
        {
            Name    = name.Trim();
            Email   = email.Trim().ToLowerInvariant();
            Phone   = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            Subject = subject.Trim();
            Message = message.Trim();
        }

        public string Name    { get; private set; } = string.Empty;
        public string Email   { get; private set; } = string.Empty;
        public string? Phone  { get; private set; }
        public string Subject { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public bool IsRead    { get; private set; }

        public void MarkAsRead() => IsRead = true;
    }
}
