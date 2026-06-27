using Project.Application.DTOs.Contact;
using Project.Domain.Entities;

namespace Project.Application.Mapper
{
    public static class ContactMapper
    {
        public static ContactMessageDto ToDto(ContactMessage message)
        {
            return new ContactMessageDto(
                Id: message.Id,
                Name: message.Name,
                Email: message.Email,
                Phone: message.Phone,
                Subject: message.Subject,
                Message: message.Message,
                IsRead: message.IsRead,
                CreatedAt: message.CreatedAt);
        }
    }
}
