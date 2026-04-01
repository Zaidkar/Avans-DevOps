using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Strategies
{
    public class EmailNotificationStrategy(IMailProvider mailProvider) : INotificationStrategy
    {
        private readonly IMailProvider _mailProvider = mailProvider;

        public void Send(NotificationMessage message, List<User> recipients)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [EmailNotificationStrategy] Send -> {recipients.Count} recipient(s)");

            foreach (var recipient in recipients)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [EmailNotificationStrategy] Sending email to {recipient.Email}...");
                _mailProvider.SendEmail(recipient.Email, message.Subject, message.Body);
            }
        }
    }
}