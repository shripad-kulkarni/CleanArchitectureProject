using Project.Application.Common.Result;
using Project.Application.DTOs.Chat;

namespace Project.Application.Abstractions.Services
{
    public interface IChatService
    {
        Task<Result<ChatMessageDto>> SendAsync(string senderId, SendMessageDto dto, CancellationToken ct = default);
        Task<Result<List<ChatMessageDto>>> GetConversationAsync(string userId, string otherUserId, int page = 1, int pageSize = 50, CancellationToken ct = default);
        Task<Result> MarkAsReadAsync(string receiverId, string senderId, CancellationToken ct = default);
        Task<Result<int>> GetUnreadCountAsync(string userId, CancellationToken ct = default);
        Task<Result<List<ChatUserDto>>> GetChatUsersAsync(string currentIdentityId, CancellationToken ct = default);
        Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, long fileSizeBytes, CancellationToken ct = default);
    }
}
