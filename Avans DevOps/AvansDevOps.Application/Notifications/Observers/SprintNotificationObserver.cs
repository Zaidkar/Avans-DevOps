using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Events;
using Avans_DevOps.AvansDevOps.Application.Notifications.Strategies;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Observers
{
    public class SprintNotificationObserver(NotificationContext notificationContext) : INotificationSubscriber
    {
        private readonly NotificationContext _notificationContext = notificationContext;

        public void Update(INotificationEvent notificationEvent)
        {
            if (notificationEvent is not SprintEvent sprintEvent)
            {
                return;
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SprintNotificationObserver] Update");
            _notificationContext.Send(sprintEvent.Message, sprintEvent.Recipients);
        }
    }
}