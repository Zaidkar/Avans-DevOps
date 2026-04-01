namespace Avans_DevOps.AvansDevOps.Application.Notifications.Contracts
{
    public interface INotificationSubscriber
    {
        void Update(INotificationEvent notificationEvent);
    }
}