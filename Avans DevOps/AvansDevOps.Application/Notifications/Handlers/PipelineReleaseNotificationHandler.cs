using Avans_DevOps.AvansDevOps.Application.Notifications.Events;
using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Application.Notifications.Publishers;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Handlers
{
    public class PipelineReleaseNotificationHandler(NotificationPublisher publisher) : IPipelineReleaseNotificationHandler
    {
        private readonly NotificationPublisher _publisher = publisher;

        public void NotifyReleaseCancelled(string sprintName, List<User> recipients)
        {
            Publish("Pipeline release cancelled", $"Release of sprint {sprintName} has been cancelled.", recipients);
        }

        public void NotifyPipelineCompletedSuccessfully(string sprintName, List<User> recipients)
        {
            Publish("Pipeline activities successful", $"All pipeline activities for sprint {sprintName} were executed successfully.", recipients);
        }

        public void NotifyPipelineActivityFailed(string sprintName, List<User> recipients)
        {
            Publish("Pipeline activity failed", $"A pipeline activity failed during the release of sprint {sprintName}.", recipients);
        }

        private void Publish(string subject, string body, List<User> recipients)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [PipelineReleaseNotificationHandler] {subject}");

            var message = new NotificationMessage
            {
                Subject = subject,
                Body = body
            };

            _publisher.Publish(new PipelineReleaseEvent(message, recipients));
        }
    }
}
