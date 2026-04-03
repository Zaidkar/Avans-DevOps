namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public sealed class NotificationEventData
{
    public Guid SprintId { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
}
