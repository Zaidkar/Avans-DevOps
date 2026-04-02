using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Strategies
{
    // Design Pattern: Strategy. Swaps notification channel behavior at runtime.
    public class NotificationContext(INotificationStrategy strategy)
    {
        private INotificationStrategy _strategy = strategy;

        public void SetStrategy(INotificationStrategy strategy)
        {
            _strategy = strategy;
        }

        public void Send(NotificationMessage message, List<User> recipients)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [NotificationContext] Send -> '{message.Subject}' for {recipients.Count} recipient(s)");
            _strategy.Send(message, recipients);
        }
    }
}