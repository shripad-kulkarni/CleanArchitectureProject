using Project.Domain.Primitives;

namespace Project.Domain.Aggregates.ContactAggregate
{
    public sealed class ContactMessage : AggregateRoot
    {
        private ContactMessage() { }

        public string Name    { get; private set; } = string.Empty;
        public string Email   { get; private set; } = string.Empty;
        public string? Phone  { get; private set; }
        public string Subject { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public bool IsRead    { get; private set; }

        public static ContactMessage Create(
            string name,
            string email,
            string? phone,
            string subject,
            string message)
        {
            if (string.IsNullOrWhiteSpace(name))    throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(email))   throw new ArgumentException("Email is required.");
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Subject is required.");
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message is required.");

            return new ContactMessage
            {
                Name    = name.Trim(),
                Email   = email.Trim().ToLowerInvariant(),
                Phone   = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim(),
                Subject = subject.Trim(),
                Message = message.Trim(),
            };
        }

        public void MarkAsRead() => IsRead = true;
    }
}
