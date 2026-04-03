using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Application.Services;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Moq;

namespace AvansDevOps.Tests;

public class SprintServiceTests
{
    private readonly Mock<ISprintRepository> _sprintRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IEventManager> _eventManager = new();
    private readonly Mock<IPipelineService> _pipelineService = new();

    private SprintService CreateService() =>
        new(_sprintRepository.Object, _userRepository.Object, _eventManager.Object, _pipelineService.Object);

    [Fact]
    public void FinishSprint_FinishesSprintAndPublishesNotification()
    {
        var sprintId = Guid.NewGuid();
        var sprint = new Sprint(
            sprintId,
            "Sprint 4",
            new DateOnly(2026, 4, 1),
            new DateOnly(2026, 4, 14),
            SprintGoalType.Release);
        sprint.Start();

        _sprintRepository.Setup(x => x.GetById(sprintId)).Returns(sprint);
        _sprintRepository.Setup(x => x.Update(sprintId, sprint)).Returns(true);

        var service = CreateService();

        var result = service.FinishSprint(sprintId);

        Assert.True(result);
        _sprintRepository.Verify(x => x.Update(sprintId, sprint), Times.Once);
        _eventManager.Verify(
            x => x.Notify(
                NotificationEventNames.SprintFinished,
                It.Is<NotificationEventData>(data =>
                    data.EventType == NotificationEventNames.SprintFinished &&
                    data.SprintId == sprintId &&
                    data.Subject == "Sprint finished")),
            Times.Once);
    }
}
