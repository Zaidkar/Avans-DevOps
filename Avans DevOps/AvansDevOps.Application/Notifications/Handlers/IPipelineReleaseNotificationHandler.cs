using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Handlers
{
    public interface IPipelineReleaseNotificationHandler
    {
        void NotifyReleaseCancelled(string sprintName, List<User> recipients);
        void NotifyPipelineCompletedSuccessfully(string sprintName, List<User> recipients);
        void NotifyPipelineActivityFailed(string sprintName, List<User> recipients);
    }
}
