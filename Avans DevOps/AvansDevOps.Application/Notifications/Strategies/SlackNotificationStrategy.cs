using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;

public class SlackNotificationStrategy(SlackSdk slackSdk) : INotificationStrategy
{
    private readonly SlackSdk _slackSdk = slackSdk;

    public void Execute(NotificationMessage message, List<SprintMember> recipients)
    {
        for (var index = 0; index < recipients.Count; index++)
        {
            _slackSdk.SendSlack($"#recipient-{index + 1}", message.Body);
        }
    }
}