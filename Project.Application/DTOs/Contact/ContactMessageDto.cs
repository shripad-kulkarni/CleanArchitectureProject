namespace Project.Application.DTOs.Contact
{
    public record ContactMessageDto(
        int Id,
        string Name,
        string Email,
        string? Phone,
        string Subject,
        string Message,
        bool IsRead,
        DateTime CreatedAt);
}
