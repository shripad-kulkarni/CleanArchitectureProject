using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Project.Application.Abstractions.Identity;

namespace Project.Infrastructure.Hubs
{
    [Authorize]
    public sealed class NotificationHub : Hub
    {
        private readonly ICurrentUserService _currentUserService;

        public NotificationHub(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = _currentUserService.UserId;
            if (!string.IsNullOrEmpty(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = _currentUserService.UserId;
            if (!string.IsNullOrEmpty(userId))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");

            await base.OnDisconnectedAsync(exception);
        }

        public Task JoinGroup(string group)
            => Groups.AddToGroupAsync(Context.ConnectionId, group);

        public Task LeaveGroup(string group)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
    }
}
