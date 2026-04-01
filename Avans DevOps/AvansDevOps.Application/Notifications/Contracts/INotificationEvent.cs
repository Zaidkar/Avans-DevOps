using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Contracts
{
    public interface INotificationEvent
    {
        NotificationMessage Message { get; }
        List<User> Recipients { get; }
    }
}