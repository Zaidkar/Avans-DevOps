using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Handlers
{
    public interface IBacklogItemNotificationHandler
    {
        void NotifyReadyForTesting(string backlogItemTitle, List<User> recipients);
        void NotifyBackToTodo(string backlogItemTitle, List<User> recipients);
        void NotifyTestedRejected(string backlogItemTitle, List<User> recipients);
    }
}