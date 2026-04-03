using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Application.Pipeline;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Application.Services;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Moq;

namespace AvansDevOps.Tests;

public class PipelineServiceTests
{
    private readonly Mock<ISprintRepository> _sprintRepository = new();
    private readonly Mock<IPipelineFactory> _pipelineFactory = new();
    private readonly Mock<IEventManager> _eventManager = new();

    private PipelineService CreateService() =>
        new(_sprintRepository.Object, _pipelineFactory.Object, _eventManager.Object);

    private static Sprint CreateFinishedReleaseSprint(Guid sprintId, string name, params SprintMember[] members)
    {
        var sprint = new Sprint(sprintId, name, new DateOnly(2026, 4, 1), new DateOnly(2026, 4, 14), SprintGoalType.Release);

        foreach (var member in members)
        {
            sprint.AddMember(member);
        }

        sprint.AssignPipeline(new PipelineDefinition(Guid.NewGuid(), "Release Pipeline"));
        sprint.Start();
        sprint.Finish();
        sprint.BeginRelease();
        return sprint;
    }

    private static Sprint CreateReleaseFailedSprint(Guid sprintId, string name, params SprintMember[] members)
    {
        var sprint = CreateFinishedReleaseSprint(sprintId, name, members);
        sprint.ReleaseFailed();
        return sprint;
    }

    private static User CreateUser(string name) =>
        new() { Id = Guid.NewGuid(), Name = name, Email = $"{name.Replace(" ", "").ToLowerInvariant()}@avans.dev" };

    private static SprintMember CreateMember(User user, SprintRole role) =>
        new(Guid.NewGuid(), user, role);

    [Fact]
    public void TC_24_FR_07_ReleaseSucceeded_NotifiesScrumMasterAndProductOwner()
    {
        var sprintId = Guid.NewGuid();
        var scrumMaster = CreateUser("Scrum Master");
        var productOwner = CreateUser("Product Owner");
        var sprint = CreateFinishedReleaseSprint(
            sprintId,
            "Release Sprint",
            CreateMember(scrumMaster, SprintRole.ScrumMaster),
            CreateMember(productOwner, SprintRole.ProductOwner));

        _sprintRepository.Setup(x => x.GetById(sprintId)).Returns(sprint);
        _sprintRepository.Setup(x => x.Update(sprintId, sprint)).Returns(true);
        var service = CreateService();

        var result = service.ReleaseSucceeded(sprintId);

        Assert.True(result);
        _eventManager.Verify(
            x => x.Notify(
                NotificationEventNames.ReleaseSuccess,
                It.Is<NotificationEventData>(data => data.SprintId == sprintId && data.Body.Contains(sprint.Name))),
            Times.Once);
    }

    [Fact]
    public void TC_25_FR_07_ReleaseFailed_NotifiesOnlyScrumMaster()
    {
        var sprintId = Guid.NewGuid();
        var scrumMaster = CreateUser("Scrum Master");
        var productOwner = CreateUser("Product Owner");
        var sprint = CreateFinishedReleaseSprint(
            sprintId,
            "Release Sprint",
            CreateMember(scrumMaster, SprintRole.ScrumMaster),
            CreateMember(productOwner, SprintRole.ProductOwner));

        _sprintRepository.Setup(x => x.GetById(sprintId)).Returns(sprint);
        _sprintRepository.Setup(x => x.Update(sprintId, sprint)).Returns(true);
        var service = CreateService();

        var result = service.ReleaseFailed(sprintId);

        Assert.True(result);
        _eventManager.Verify(
            x => x.Notify(
                NotificationEventNames.ReleaseFailure,
                It.Is<NotificationEventData>(data => data.SprintId == sprintId && data.Body.Contains(sprint.Name))),
            Times.Once);
    }

    [Fact]
    public void TC_26_FR_07_CancelRelease_NotifiesScrumMasterAndProductOwner()
    {
        var sprintId = Guid.NewGuid();
        var scrumMaster = CreateUser("Scrum Master");
        var productOwner = CreateUser("Product Owner");
        var sprint = CreateReleaseFailedSprint(
            sprintId,
            "Release Sprint",
            CreateMember(scrumMaster, SprintRole.ScrumMaster),
            CreateMember(productOwner, SprintRole.ProductOwner));

        _sprintRepository.Setup(x => x.GetById(sprintId)).Returns(sprint);
        _sprintRepository.Setup(x => x.Update(sprintId, sprint)).Returns(true);
        var service = CreateService();

        var result = service.CancelRelease(sprintId);

        Assert.True(result);
        _eventManager.Verify(
            x => x.Notify(
                NotificationEventNames.ReleaseCancelled,
                It.Is<NotificationEventData>(data => data.SprintId == sprintId && data.Body.Contains(sprint.Name))),
            Times.Once);
    }
}
