using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Handlers
{
    public interface IDiscussionNotificationHandler
    {
        void NotifyDiscussionCreated(string discussionTitle, List<User> recipients);
        void NotifyDiscussionReply(string discussionTitle, List<User> recipients);
    }
}