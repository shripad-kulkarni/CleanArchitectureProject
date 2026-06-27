using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Identity;
using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.Services;
using Project.Application.Common.Errors;
using Project.Application.Common.Result;
using Project.Application.DTOs.Chat;
using Project.Application.Specifications.Chat;
using Project.Domain.Entities;
using Project.Domain.Constants;

namespace Project.Application.Services
{
    public sealed class ChatService : IChatService
    {
        private static readonly long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        private readonly IRepository<ChatMessage> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityUserLookupService _identityLookup;
        private readonly IFileStorageService _fileStorage;

        public ChatService(
            IRepository<ChatMessage> repository,
            IUnitOfWork unitOfWork,
            IIdentityUserLookupService identityLookup,
            IFileStorageService fileStorage)
        {
            _repository   = repository;
            _unitOfWork   = unitOfWork;
            _identityLookup = identityLookup;
            _fileStorage  = fileStorage;
        }

        public async Task<Result<ChatMessageDto>> SendAsync(string senderId, SendMessageDto dto, CancellationToken ct = default)
        {
            if (senderId == dto.ReceiverId)
                return Result<ChatMessageDto>.Failure(Error.Validation("Chat.SelfMessage", "Cannot send a message to yourself."));

            var message = ChatMessage.Create(senderId, dto.ReceiverId, dto.Content, dto.FileUrl, dto.FileName);
            await _repository.AddAsync(message, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<ChatMessageDto>.Success(ToDto(message));
        }

        public async Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, long fileSizeBytes, CancellationToken ct = default)
        {
            if (fileSizeBytes > MaxFileSizeBytes)
                return Result<string>.Failure(Error.Validation("Chat.FileTooLarge", "File must be 10 MB or smaller."));

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (!DocumentConstants.AllowedExtensions.Contains(ext))
                return Result<string>.Failure(Error.Validation("Chat.InvalidFileType",
                    $"Allowed types: {string.Join(", ", DocumentConstants.AllowedExtensions)}."));

            var path = await _fileStorage.UploadAsync(fileStream, fileName, "chat", ct);
            return Result<string>.Success(_fileStorage.GetFileUrl(path));
        }

        public async Task<Result<List<ChatMessageDto>>> GetConversationAsync(
            string userId, string otherUserId, int page = 1, int pageSize = 50, CancellationToken ct = default)
        {
            var spec = new ConversationSpecification(userId, otherUserId, page, pageSize);
            var messages = await _repository.ListAsync(spec, ct);

            // Return in chronological order (spec fetches descending for paging efficiency)
            messages.Reverse();

            return Result<List<ChatMessageDto>>.Success(messages.Select(ToDto).ToList());
        }

        public async Task<Result> MarkAsReadAsync(string receiverId, string senderId, CancellationToken ct = default)
        {
            var spec = new UnreadMessagesSpecification(receiverId, senderId);
            var unread = await _repository.ListAsync(spec, ct);

            foreach (var m in unread)
                m.MarkAsRead();

            if (unread.Count > 0)
                await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }

        public async Task<Result<int>> GetUnreadCountAsync(string userId, CancellationToken ct = default)
        {
            var spec = new UnreadCountSpecification(userId);
            var count = await _repository.CountAsync(spec, ct);
            return Result<int>.Success(count);
        }

        public async Task<Result<List<ChatUserDto>>> GetChatUsersAsync(string currentIdentityId, CancellationToken ct = default)
        {
            // Query Identity users directly — covers all login-capable accounts
            // regardless of whether they also have a domain User record.
            var users = await _identityLookup.GetAllChatUsersAsync(currentIdentityId, ct);
            return Result<List<ChatUserDto>>.Success(users);
        }

        private static ChatMessageDto ToDto(ChatMessage m) =>
            new(m.Id, m.SenderId, m.ReceiverId, m.Content, m.IsRead, m.CreatedAt, m.FileUrl, m.FileName);
    }
}
