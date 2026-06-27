using Project.Domain.Entities;

namespace Project.Application.Specifications.Chat
{
    public sealed class ConversationSpecification : BaseSpecification<ChatMessage>
    {
        public ConversationSpecification(string userA, string userB, int page, int pageSize)
        {
            AddCriteria(m =>
                (m.SenderId == userA && m.ReceiverId == userB) ||
                (m.SenderId == userB && m.ReceiverId == userA));
            AddOrderByDescending(m => m.CreatedAt);
            EnablePaging((page - 1) * pageSize, pageSize);
        }
    }

    public sealed class UnreadMessagesSpecification : BaseSpecification<ChatMessage>
    {
        public UnreadMessagesSpecification(string receiverId, string senderId)
        {
            AddCriteria(m => m.ReceiverId == receiverId && m.SenderId == senderId && !m.IsRead);
        }
    }

    public sealed class UnreadCountSpecification : BaseSpecification<ChatMessage>
    {
        public UnreadCountSpecification(string receiverId)
        {
            AddCriteria(m => m.ReceiverId == receiverId && !m.IsRead);
        }
    }
}
