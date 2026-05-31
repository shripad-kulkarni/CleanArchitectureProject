namespace Project.Application.DTOs.Contact
{
    public record CreateContactMessageDto(
        string Name,
        string Email,
        string? Phone,
        string Subject,
        string Message);
}
