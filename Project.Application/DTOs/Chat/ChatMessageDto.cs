namespace Project.Application.DTOs.Chat
{
    public sealed record ChatMessageDto(
        int      Id,
        string   SenderId,
        string   ReceiverId,
        string   Content,
        bool     IsRead,
        DateTime SentAt,
        string?  FileUrl  = null,
        string?  FileName = null);
}
