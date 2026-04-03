using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;

public class NotificationStrategyContext(INotificationStrategy? strategy = null)
{
    private INotificationStrategy? _strategy = strategy;

    public void SetStrategy(INotificationStrategy strategy)
    {
        _strategy = strategy;
    }

    public void ExecuteStrategy(NotificationMessage message, List<SprintMember> recipients)
    {
        if (_strategy is null)
        {
            throw new InvalidOperationException("No notification strategy configured.");
        }

        _strategy.Execute(message, recipients);
    }
}