using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public class BacklogItemListener(
    ISprintRepository sprintRepository,
    INotificationChannelFactory channelFactory,
    IReadOnlyCollection<SprintRole> roles,
    IReadOnlyCollection<ChannelType> channels) : IEventListener
{
    private readonly ISprintRepository _sprintRepository = sprintRepository;
    private readonly INotificationChannelFactory _channelFactory = channelFactory;
    private readonly IReadOnlyCollection<SprintRole> _roles = roles;
    private readonly IReadOnlyCollection<ChannelType> _channels = channels;

    private readonly Dictionary<string, IReadOnlyCollection<SprintRole>> _eventTypeToRoles = new()
    {
        { NotificationEventNames.ReadyForTesting, new List<SprintRole> { SprintRole.Tester } },
        { NotificationEventNames.TestFailure, new List<SprintRole> { SprintRole.ScrumMaster } },
        
    };

    public void Update(NotificationEventData data)
    {
        var recipientsById = new List<SprintMember>();
        if (_eventTypeToRoles.TryGetValue(data.EventType, out var eventRoles))
        {
            foreach (var role in eventRoles)
            {
                var members = _sprintRepository.GetMembersByRole(data.SprintId, role);
                if (members != null)
                {
                    recipientsById.AddRange(members);
                }
            }
        }
        else if (_roles.Count > 0)
        {
            foreach (var role in _roles)
            {
                var members = _sprintRepository.GetMembersByRole(data.SprintId, role);
                if (members != null)
                {
                    recipientsById.AddRange(members);
                }
            }
        }
        else
        {
            var members = _sprintRepository.GetMembers(data.SprintId);
            if (members != null)
            {
                foreach (var member in members)
                {
                    recipientsById.Add(member);
                }
            }
        }

        var message = new NotificationMessage
        {
            Subject = data.Subject,
            Body = data.Body
        };

        foreach (var channelType in _channels)
        {
            _channelFactory.Create(channelType).Send(message, recipientsById);
        }
    }
}