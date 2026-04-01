using Avans_DevOps.AvansDevOps.Application.Notifications.Events;
using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Application.Notifications.Publishers;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Handlers
{
    public class BacklogItemNotificationHandler(NotificationPublisher publisher) : IBacklogItemNotificationHandler
    {
        private readonly NotificationPublisher _publisher = publisher;

        public void NotifyReadyForTesting(string backlogItemTitle, List<User> recipients)
        {
            Publish("Backlog item ready for testing", $"Backlogitem {backlogItemTitle} is ready for testing", recipients);
        }

        public void NotifyBackToTodo(string backlogItemTitle, List<User> recipients)
        {
            Publish("Backlog item terug naar todo", $"Backlogitem {backlogItemTitle} is teruggezet naar todo", recipients);
        }

        public void NotifyTestedRejected(string backlogItemTitle, List<User> recipients)
        {
            Publish("Backlog item rejected after testing", $"Backlogitem {backlogItemTitle} is afgekeurd na testing", recipients);
        }

        private void Publish(string subject, string body, List<User> recipients)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [BacklogItemNotificationHandler] {subject}");

            var message = new NotificationMessage
            {
                Subject = subject,
                Body = body
            };

            _publisher.Publish(new BacklogItemEvent(message, recipients));
        }
    }
}