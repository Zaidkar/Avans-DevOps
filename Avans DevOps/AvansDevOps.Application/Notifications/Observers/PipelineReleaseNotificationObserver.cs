using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Events;
using Avans_DevOps.AvansDevOps.Application.Notifications.Strategies;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Observers
{
    public class PipelineReleaseNotificationObserver(NotificationContext notificationContext) : INotificationSubscriber
    {
        private readonly NotificationContext _notificationContext = notificationContext;

        public void Update(INotificationEvent notificationEvent)
        {
            if (notificationEvent is not PipelineReleaseEvent pipelineReleaseEvent)
            {
                return;
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [PipelineReleaseNotificationObserver] Update");
            _notificationContext.Send(pipelineReleaseEvent.Message, pipelineReleaseEvent.Recipients);
        }
    }
}
