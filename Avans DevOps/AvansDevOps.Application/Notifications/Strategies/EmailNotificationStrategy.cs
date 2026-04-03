using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;

public class EmailNotificationStrategy(ExternalMailClient mailClient) : INotificationStrategy
{
    private readonly ExternalMailClient _mailClient = mailClient;

    public void Execute(NotificationMessage message, List<SprintMember> recipients)
    {
        foreach (var recipient in recipients)
        {
            _mailClient.SendMail(recipient.User.Email, message.Subject, message.Body);
        }
    }
}