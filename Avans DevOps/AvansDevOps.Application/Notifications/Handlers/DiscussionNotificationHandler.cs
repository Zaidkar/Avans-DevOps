using Avans_DevOps.AvansDevOps.Application.Notifications.Events;
using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Application.Notifications.Publishers;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Handlers
{
    public class DiscussionNotificationHandler(NotificationPublisher publisher) : IDiscussionNotificationHandler
    {
        private readonly NotificationPublisher _publisher = publisher;

        public void NotifyDiscussionCreated(string discussionTitle, List<User> recipients)
        {
            Publish("Discussion created", $"Er is een nieuwe discussie gestart over {discussionTitle}", recipients);
        }

        public void NotifyDiscussionReply(string discussionTitle, List<User> recipients)
        {
            Publish("Discussion reply", $"Er is een nieuwe reactie geplaatst over {discussionTitle}", recipients);
        }

        private void Publish(string subject, string body, List<User> recipients)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [DiscussionNotificationHandler] {subject}");

            var message = new NotificationMessage
            {
                Subject = subject,
                Body = body
            };

            _publisher.Publish(new DiscussionEvent(message, recipients));
        }
    }
}