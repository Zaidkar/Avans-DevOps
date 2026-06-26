namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
//Een toepassing van het observer pattern
public interface IEventListener
{
    void Update(NotificationEventData data);
}
