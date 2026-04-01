using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Strategies
{
    public class SmsNotificationStrategy(ISmsProvider smsProvider) : INotificationStrategy
    {
        private readonly ISmsProvider _smsProvider = smsProvider;

        public void Send(NotificationMessage message, List<User> recipients)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SmsNotificationStrategy] Send -> {recipients.Count} recipient(s)");

            for (var index = 0; index < recipients.Count; index++)
            {
                _smsProvider.SendSms($"+310000000{index + 1}", message.Body);
            }
        }
    }
}