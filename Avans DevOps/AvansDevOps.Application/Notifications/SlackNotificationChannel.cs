using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public class SlackNotificationChannel(ISlackProvider slackProvider) : INotificationChannel
{
    private readonly ISlackProvider _slackProvider = slackProvider;

    public void Send(NotificationMessage message, List<SprintMember> recipients)
    {
        for (var index = 0; index < recipients.Count; index++)
        {
            _slackProvider.SendSlack($"#recipient-{index + 1}", message.Body);
        }
    }
}
