using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Events;
using Avans_DevOps.AvansDevOps.Application.Notifications.Strategies;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Observers
{
    public class DiscussionNotificationObserver(NotificationContext notificationContext) : INotificationSubscriber
    {
        private readonly NotificationContext _notificationContext = notificationContext;

        public void Update(INotificationEvent notificationEvent)
        {
            if (notificationEvent is not DiscussionEvent discussionEvent)
            {
                return;
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [DiscussionNotificationObserver] Update");
            _notificationContext.Send(discussionEvent.Message, discussionEvent.Recipients);
        }
    }
}