using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;

public interface INotificationStrategy
{
    void Execute(NotificationMessage message, List<SprintMember> recipients);
}