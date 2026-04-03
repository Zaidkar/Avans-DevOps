using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;
using Avans_DevOps.AvansDevOps.Application.Repositories;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public class DiscussionNotificationListener(
    ISprintRepository sprintRepository,
    INotificationStrategyFactory strategyFactory,
    IReadOnlyCollection<ChannelType> channels) : IEventListener
{
    private readonly ISprintRepository _sprintRepository = sprintRepository;
    private readonly INotificationStrategyFactory _strategyFactory = strategyFactory;
    private readonly IReadOnlyCollection<ChannelType> _channels = channels;

    public void Update(NotificationEventData data)
    {
        var recipients = _sprintRepository.GetMembers(data.SprintId);
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