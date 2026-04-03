using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public interface INotificationChannel
{
    void Send(NotificationMessage message, List<SprintMember> recipients);
}
