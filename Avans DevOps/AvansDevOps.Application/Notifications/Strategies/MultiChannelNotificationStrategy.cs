using Avans_DevOps.AvansDevOps.Application.Notifications.Contracts;
using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Strategies
{
    public class MultiChannelNotificationStrategy(List<INotificationStrategy> strategies) : INotificationStrategy
    {
        private readonly List<INotificationStrategy> _strategies = strategies;

        public void Send(NotificationMessage message, List<User> recipients)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [MultiChannelNotificationStrategy] Send -> {_strategies.Count} channel(s)");

            foreach (var strategy in _strategies)
            {
                strategy.Send(message, recipients);
            }
        }
    }
}