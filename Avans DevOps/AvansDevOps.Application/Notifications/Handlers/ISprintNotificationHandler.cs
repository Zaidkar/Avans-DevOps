using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Handlers
{
    public interface ISprintNotificationHandler
    {
        void NotifySprintFinished(string sprintName, List<User> recipients);
        void NotifyReleaseSuccess(string sprintName, List<User> recipients);
        void NotifyReleaseFailure(string sprintName, List<User> recipients);
        void NotifyReleaseCancelled(string sprintName, List<User> recipients);
    }
}