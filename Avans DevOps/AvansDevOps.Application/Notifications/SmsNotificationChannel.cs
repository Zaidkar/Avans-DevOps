using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public class SmsNotificationChannel(ISmsProvider smsProvider) : INotificationChannel
{
    private readonly ISmsProvider _smsProvider = smsProvider;

    public void Send(NotificationMessage message, List<SprintMember> recipients)
    {
        for (var index = 0; index < recipients.Count; index++)
        {
            _smsProvider.SendSms($"+310000000{index + 1}", message.Body);
        }
    }
}
