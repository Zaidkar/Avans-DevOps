using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;

public class SmsNotificationStrategy(SmsSdk smsSdk) : INotificationStrategy
{
    private readonly SmsSdk _smsSdk = smsSdk;

    public void Execute(NotificationMessage message, List<SprintMember> recipients)
    {
        for (var index = 0; index < recipients.Count; index++)
        {
            _smsSdk.SendSms($"+310000000{index + 1}", message.Body);
        }
    }
}