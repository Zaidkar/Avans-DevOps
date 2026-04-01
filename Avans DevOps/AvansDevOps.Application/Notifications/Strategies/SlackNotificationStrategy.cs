using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Strategies
{
    public class SlackNotificationStrategy(ISlackProvider slackProvider) : INotificationStrategy
    {
        private readonly ISlackProvider _slackProvider = slackProvider;

        public void Send(NotificationMessage message, List<User> recipients)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SlackNotificationStrategy] Send -> {recipients.Count} recipient(s)");

            for (var index = 0; index < recipients.Count; index++)
            {
                _slackProvider.SendSlack($"#recipient-{index + 1}", message.Body);
            }
        }
    }
}