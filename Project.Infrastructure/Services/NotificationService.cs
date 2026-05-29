using Microsoft.AspNetCore.SignalR;
using Project.Application.Abstractions.ExternalServices;
using Project.Infrastructure.Hubs;

namespace Project.Infrastructure.Services
{
    public sealed class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public Task SendToUserAsync(string userId, string eventName, object payload, CancellationToken ct = default)
            => _hub.Clients.Group($"user-{userId}").SendAsync(eventName, payload, ct);

        public Task SendToGroupAsync(string group, string eventName, object payload, CancellationToken ct = default)
            => _hub.Clients.Group(group).SendAsync(eventName, payload, ct);

        public Task SendToAllAsync(string eventName, object payload, CancellationToken ct = default)
            => _hub.Clients.All.SendAsync(eventName, payload, ct);

        public Task AddToGroupAsync(string connectionId, string group, CancellationToken ct = default)
            => _hub.Groups.AddToGroupAsync(connectionId, group, ct);

        public Task RemoveFromGroupAsync(string connectionId, string group, CancellationToken ct = default)
            => _hub.Groups.RemoveFromGroupAsync(connectionId, group, ct);
    }
}
