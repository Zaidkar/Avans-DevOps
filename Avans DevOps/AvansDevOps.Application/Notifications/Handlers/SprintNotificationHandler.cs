using Avans_DevOps.AvansDevOps.Application.Notifications.Events;
using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Application.Notifications.Publishers;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Handlers
{
    public class SprintNotificationHandler(NotificationPublisher publisher) : ISprintNotificationHandler
    {
        private readonly NotificationPublisher _publisher = publisher;

        public void NotifySprintFinished(string sprintName, List<User> recipients)
        {
            Publish("Sprint finished", $"Sprint {sprintName} is finished", recipients);
        }

        public void NotifyReleaseSuccess(string sprintName, List<User> recipients)
        {
            Publish("Release success", $"Sprint {sprintName} has been released successfully", recipients);
        }

        public void NotifyReleaseFailure(string sprintName, List<User> recipients)
        {
            Publish("Release failure", $"Release of sprint {sprintName} has failed", recipients);
        }

        public void NotifyReleaseCancelled(string sprintName, List<User> recipients)
        {
            Publish("Release cancelled", $"Release of sprint {sprintName} has been cancelled", recipients);
        }

        private void Publish(string subject, string body, List<User> recipients)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SprintNotificationHandler] {subject}");

            var message = new NotificationMessage
            {
                Subject = subject,
                Body = body
            };

            _publisher.Publish(new SprintEvent(message, recipients));
        }
    }
}