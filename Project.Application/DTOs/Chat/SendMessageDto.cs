namespace Project.Application.DTOs.Chat
{
    public sealed record SendMessageDto(
        string  ReceiverId,
        string  Content,
        string? FileUrl  = null,
        string? FileName = null);
}
