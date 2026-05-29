namespace Project.Application.Abstractions.ExternalServices
{
    public interface INotificationService
    {
        Task SendToUserAsync(string userId, string eventName, object payload, CancellationToken ct = default);
        Task SendToGroupAsync(string group, string eventName, object payload, CancellationToken ct = default);
        Task SendToAllAsync(string eventName, object payload, CancellationToken ct = default);
        Task AddToGroupAsync(string connectionId, string group, CancellationToken ct = default);
        Task RemoveFromGroupAsync(string connectionId, string group, CancellationToken ct = default);
    }
}
