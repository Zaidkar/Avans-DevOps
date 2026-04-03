namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public interface IEventManager
{
    void Subscribe(string eventType, IEventListener listener);
    void Unsubscribe(string eventType, IEventListener listener);
    void Notify(string eventType, NotificationEventData data);
}
