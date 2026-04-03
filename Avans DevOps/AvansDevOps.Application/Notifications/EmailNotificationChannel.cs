using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public class EmailNotificationChannel(IMailProvider mailProvider) : INotificationChannel
{
    private readonly IMailProvider _mailProvider = mailProvider;

    public void Send(NotificationMessage message, List<SprintMember> recipients)
    {
        foreach (var recipient in recipients)
        {
            _mailProvider.SendEmail(recipient.User.Email, message.Subject, message.Body);
        }
    }
}
