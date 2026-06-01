using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Project.Application.Abstractions.Identity;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.Chat;

namespace Project.Infrastructure.Hubs
{
    [Authorize]
    public sealed class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ICurrentUserService _currentUserService;

        public ChatHub(IChatService chatService, ICurrentUserService currentUserService)
        {
            _chatService = chatService;
            _currentUserService = currentUserService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = _currentUserService.UserId;
            if (!string.IsNullOrEmpty(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{userId}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = _currentUserService.UserId;
            if (!string.IsNullOrEmpty(userId))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{userId}");

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(SendMessageDto dto)
        {
            var senderId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(senderId))
                throw new HubException("Not authenticated.");

            var result = await _chatService.SendAsync(senderId, dto);
            if (result.IsFailure)
                throw new HubException(result.Error.Message);

            var message = result.Value;

            // Deliver to both sender (all tabs) and receiver
            await Clients.Group($"chat-{senderId}").SendAsync("ReceiveMessage", message);

            if (dto.ReceiverId != senderId)
                await Clients.Group($"chat-{dto.ReceiverId}").SendAsync("ReceiveMessage", message);
        }

        public async Task MarkAsRead(string senderId)
        {
            var receiverId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(receiverId))
                throw new HubException("Not authenticated.");

            var result = await _chatService.MarkAsReadAsync(receiverId, senderId);
            if (result.IsFailure)
                throw new HubException(result.Error.Message);

            // Notify the original sender that their messages were read
            await Clients.Group($"chat-{senderId}").SendAsync("MessagesRead", new { by = receiverId });
        }
    }
}
