using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Application.Repositories;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public class DiscussionNotificationListener(
    ISprintRepository sprintRepository,
    INotificationChannelFactory channelFactory,
    IReadOnlyCollection<ChannelType> channels) : IEventListener
{
    private readonly ISprintRepository _sprintRepository = sprintRepository;
    private readonly INotificationChannelFactory _channelFactory = channelFactory;
    private readonly IReadOnlyCollection<ChannelType> _channels = channels;

    public void Update(NotificationEventData data)
    {
        var recipients = _sprintRepository.GetMembers(data.SprintId);
        var message = new NotificationMessage
        {
            Subject = data.Subject,
            Body = data.Body
        };

        foreach (var channelType in _channels)
        {
            _channelFactory.Create(channelType).Send(message, recipients);
        }
    }
}