using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Events;
using Avans_DevOps.AvansDevOps.Application.Notifications.Strategies;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Observers
{
    public class BacklogItemNotificationObserver(NotificationContext notificationContext) : INotificationSubscriber
    {
        private readonly NotificationContext _notificationContext = notificationContext;

        public void Update(INotificationEvent notificationEvent)
        {
            if (notificationEvent is not BacklogItemEvent backlogItemEvent)
            {
                return;
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [BacklogItemNotificationObserver] Update");
            _notificationContext.Send(backlogItemEvent.Message, backlogItemEvent.Recipients);
        }
    }
}