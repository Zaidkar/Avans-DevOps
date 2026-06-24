using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public class DiscussionNotificationListener(
    Sprint sprint,
    INotificationStrategyFactory strategyFactory,
    IReadOnlyCollection<ChannelType> channels) : IEventListener
{
    private readonly INotificationStrategyFactory _strategyFactory = strategyFactory;
    private readonly IReadOnlyCollection<ChannelType> _channels = channels;

    public void Update(NotificationEventData data)
    {
        var recipients = new List<SprintMember>();
        recipients.AddRange(sprint.Members);
        var message = new NotificationMessage
        {
            Subject = data.Subject,
            Body = data.Body
        };

        var context = new NotificationStrategyContext();

        foreach (var channelType in _channels)
        {
            context.SetStrategy(_strategyFactory.Create(channelType));
            context.ExecuteStrategy(message, recipients);
        }
    }
}