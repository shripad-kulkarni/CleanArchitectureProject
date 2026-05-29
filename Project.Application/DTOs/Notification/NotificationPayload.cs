namespace Project.Application.DTOs.Notification
{
    public sealed record NotificationPayload(
        string Title,
        string Message,
        string Type = "info",   // info | success | warning | error
        object? Data = null
    );
}
