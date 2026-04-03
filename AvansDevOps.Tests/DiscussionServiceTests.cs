using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Application.Services;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Moq;

namespace AvansDevOps.Tests;

public class DiscussionServiceTests
{
    private readonly Mock<IDiscussionRepository> _discussionRepository = new();
    private readonly Mock<IBacklogItemRepository> _backlogRepository = new();
    private readonly Mock<ISprintRepository> _sprintRepository = new();
    private readonly Mock<IEventManager> _eventManager = new();

    private DiscussionService CreateService() =>
        new(_discussionRepository.Object, _backlogRepository.Object, _eventManager.Object);

    private static User CreateUser(string name) =>
        new() { Id = Guid.NewGuid(), Name = name, Email = $"{name.Replace(" ", "").ToLowerInvariant()}@avans.dev" };

    private static Sprint CreateSprint(Guid sprintId) =>
        new(sprintId, "Sprint", new DateOnly(2026, 4, 1), new DateOnly(2026, 4, 14), SprintGoalType.Release);

    private static BacklogItem CreateBacklogItem(Guid id) =>
        new(id, "Item", "Desc", 3);

    [Fact]
    public void TC_22_FR_07_CreateDiscussion_NotifiesSprintTeamMembers()
    {
        var discussionId = Guid.NewGuid();
        var backlogItemId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();
        var thread = new DiscussionThread(Guid.NewGuid(), backlogItemId, "Refinement");
        var backlogItem = CreateBacklogItem(backlogItemId);
        var sprint = CreateSprint(sprintId);
        var teamMembers = new List<SprintMember>
        {
            new(Guid.NewGuid(), CreateUser("Product Owner"), SprintRole.ProductOwner),
            new(Guid.NewGuid(), CreateUser("Scrum Master"), SprintRole.ScrumMaster),
            new(Guid.NewGuid(), CreateUser("Tester One"), SprintRole.Tester)
        };

        _backlogRepository.Setup(x => x.GetById(backlogItemId)).Returns(backlogItem);
        _backlogRepository.Setup(x => x.GetSprintForBacklogItem(backlogItemId)).Returns(sprint);
        _discussionRepository.Setup(x => x.Create(thread)).Returns(discussionId);
        _sprintRepository.Setup(x => x.GetMembers(sprintId)).Returns(teamMembers);

        var service = CreateService();

        var result = service.Create(thread);

        Assert.Equal(discussionId, result);
        _eventManager.Verify(
            x => x.Notify(
                NotificationEventNames.DiscussionCreated,
                It.Is<NotificationEventData>(data => data.SprintId == sprintId && data.Body.Contains(thread.Subject))),
            Times.Once);
    }

    [Fact]
    public void TC_23_FR_07_Reply_NotifiesSprintTeamMembers()
    {
        var discussionId = Guid.NewGuid();
        var backlogItemId = Guid.NewGuid();
        var sprintId = Guid.NewGuid();
        var thread = new DiscussionThread(Guid.NewGuid(), backlogItemId, "Architecture");
        var replyAuthor = CreateUser("Developer One");
        var reply = new DiscussionPost(Guid.NewGuid(), replyAuthor, "Looks good", DateTime.UtcNow);
        var backlogItem = CreateBacklogItem(backlogItemId);
        var sprint = CreateSprint(sprintId);
        var teamMembers = new List<SprintMember>
        {
            new(Guid.NewGuid(), CreateUser("Developer One"), SprintRole.Developer),
            new(Guid.NewGuid(), CreateUser("Tester Two"), SprintRole.Tester)
        };

        _discussionRepository.Setup(x => x.GetById(discussionId)).Returns(thread);
        _discussionRepository.Setup(x => x.Update(discussionId, thread)).Returns(true);
        _backlogRepository.Setup(x => x.GetById(backlogItemId)).Returns(backlogItem);
        _backlogRepository.Setup(x => x.GetSprintForBacklogItem(backlogItemId)).Returns(sprint);
        _sprintRepository.Setup(x => x.GetMembers(sprintId)).Returns(teamMembers);

        var service = CreateService();

        var result = service.Reply(discussionId, reply);

        Assert.True(result);
        _eventManager.Verify(
            x => x.Notify(
                NotificationEventNames.DiscussionReply,
                It.Is<NotificationEventData>(data => data.SprintId == sprintId && data.Body.Contains(thread.Subject))),
            Times.Once);
    }
}
