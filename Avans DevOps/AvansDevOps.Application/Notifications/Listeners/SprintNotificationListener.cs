using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Application.Notifications.Simple;

public class SprintNotificationListener(
    ISprintRepository sprintRepository,
    INotificationStrategyFactory strategyFactory,
    IReadOnlyCollection<ChannelType> channels,
    IReadOnlyCollection<SprintRole>? roles = null) : IEventListener
{
    private readonly ISprintRepository _sprintRepository = sprintRepository;
    private readonly INotificationStrategyFactory _strategyFactory = strategyFactory;
    private readonly IReadOnlyCollection<ChannelType> _channels = channels;
    private readonly IReadOnlyCollection<SprintRole>? _roles = roles;
    private readonly Dictionary<string, IReadOnlyCollection<SprintRole>> _eventTypeToRoles = new()
    {
        { NotificationEventNames.SprintFinished, new List<SprintRole> { SprintRole.Developer, SprintRole.Tester, SprintRole.ScrumMaster, SprintRole.ProductOwner } },
        { NotificationEventNames.ReleaseSuccess, new List<SprintRole> { SprintRole.ScrumMaster, SprintRole.ProductOwner } },
        { NotificationEventNames.ReleaseFailure, new List<SprintRole> { SprintRole.ScrumMaster } },
        { NotificationEventNames.ReleaseCancelled, new List<SprintRole> { SprintRole.ScrumMaster, SprintRole.ProductOwner } }
    };

    public void Update(NotificationEventData data)
    {
        List<SprintMember> recipients;
        if (_eventTypeToRoles.TryGetValue(data.EventType, out var mappedRoles))
        {
            recipients = GetMembersByRoles(data.SprintId, mappedRoles);
        }
        else if (_roles is not null && _roles.Count > 0)
        {
            recipients = GetMembersByRoles(data.SprintId, _roles);
        }
        else
        {
            recipients = _sprintRepository.GetMembers(data.SprintId);
        }

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

    private List<SprintMember> GetMembersByRoles(Guid sprintId, IReadOnlyCollection<SprintRole> roles)
    {
        var recipientsById = new Dictionary<Guid, SprintMember>();

        foreach (var role in roles)
        {
            foreach (var member in _sprintRepository.GetMembersByRole(sprintId, role))
            {
                recipientsById[member.User.Id] = member;
            }
        }

        return recipientsById.Values.ToList();
    }
}