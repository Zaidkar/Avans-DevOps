using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Events
{
    public class BacklogItemEvent(NotificationMessage message, List<User> recipients) : INotificationEvent
    {
        public NotificationMessage Message { get; } = message;
        public List<User> Recipients { get; } = recipients;
    }
}