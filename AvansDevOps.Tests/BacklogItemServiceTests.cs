using System.Runtime.CompilerServices;
using Avans_DevOps.AvansDevOps.Application.Notifications.Handlers;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Application.Services;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Moq;

namespace AvansDevOps.Tests;

public class BacklogItemServiceTests
{
    private readonly Mock<IBacklogItemRepository> _backlogRepo = new();
    private readonly Mock<ISprintRepository> _sprintRepo = new();
    private readonly Mock<IBacklogItemNotificationHandler> _notifications = new();
    private readonly Mock<IUserRepository> _userRepo = new();

    private BacklogItemService CreateService() =>
        new(_backlogRepo.Object, _sprintRepo.Object, _notifications.Object, _userRepo.Object);

    private static BacklogItem CreateItem(Guid id)
    {
        var item = new BacklogItem(id, "Item", "Desc", 3);
        item.AssignDeveloper(CreateUser("Dev"));
        return item;
    }

    private static Sprint CreateSprint(Guid id) =>
        new(id, "Sprint", new DateOnly(2026, 4, 1), new DateOnly(2026, 4, 14), SprintGoalType.Release);

    private static User CreateUser(string name) =>
        new() { Id = Guid.NewGuid(), Name = name, Email = $"{name.Replace(" ", "").ToLowerInvariant()}@test.com" };

    private static SprintMember CreateMember(string name, SprintRole role) =>
        new(Guid.NewGuid(), CreateUser(name), role);

    private static List<User> GetMembersByRole(Sprint sprint, SprintRole role) =>
        sprint.Members
            .Where(member => member.SprintRole == role)
            .Select(member => member.User)
            .ToList();

    // 🔹 1. Alle null backlogItem guards in één test
    [Fact]
    public void Methods_ReturnFalse_WhenBacklogItemNotFound()
    {
        var id = Guid.NewGuid();
        _backlogRepo.Setup(x => x.GetById(id)).Returns((BacklogItem?)null);

        var service = CreateService();

        Assert.False(service.StartWork(id));
        Assert.False(service.MarkReadyForTesting(id));
        Assert.False(service.StartTesting(id));
        Assert.False(service.MarkTested(id));
        Assert.False(service.ApproveDone(id));
        Assert.False(service.ReturnToReadyForTesting(id));
        Assert.False(service.ReturnToTodo(id));
    }

    // 🔹 2. Sprint-required methods
    [Fact]
    public void Methods_ReturnFalse_WhenSprintNotFound()
    {
        var id = Guid.NewGuid();
        var item = CreateItem(id);

        _backlogRepo.Setup(x => x.GetById(id)).Returns(item);
        _backlogRepo.Setup(x => x.GetSprintForBacklogItem(id)).Returns((Sprint?)null);

        var service = CreateService();

        Assert.False(service.MarkReadyForTesting(id));
        Assert.False(service.ReturnToReadyForTesting(id));
        Assert.False(service.ReturnToTodo(id));
    }

    // 🔹 3. Happy flow: testers notificatie
    [Fact]
    public void MarkReadyForTesting_NotifiesTesters()
    {
        var id = Guid.NewGuid();
        var sprintId = Guid.NewGuid();
        var item = CreateItem(id);
        var sprint = CreateSprint(sprintId);
        sprint.AddMember(CreateMember("Tessa van Dijk", SprintRole.Tester));
        var testers = GetMembersByRole(sprint, SprintRole.Tester);

        _backlogRepo.Setup(x => x.GetById(id)).Returns(item);
        _backlogRepo.Setup(x => x.GetSprintForBacklogItem(id)).Returns(sprint);
        _backlogRepo.Setup(x => x.Update(id, item)).Returns(true);
        _sprintRepo.Setup(x => x.GetMembersByRole(sprintId, SprintRole.Tester)).Returns(testers);

        var service = CreateService();

        var result = service.MarkReadyForTesting(id);

        Assert.True(result);
        _notifications.Verify(x => x.NotifyReadyForTesting(item.Title, testers), Times.Once);
    }

    // 🔹 4. ReturnToReadyForTesting notificatie
    [Fact]
    public void ReturnToReadyForTesting_NotifiesTesters()
    {
        var id = Guid.NewGuid();
        var sprintId = Guid.NewGuid();
        var item = CreateItem(id);
        var sprint = CreateSprint(sprintId);
        sprint.AddMember(CreateMember("Timo de Jong", SprintRole.Tester));
        var testers = GetMembersByRole(sprint, SprintRole.Tester);

        item.MarkReadyForTesting();
        item.StartTesting();
        item.MarkTested();

        _backlogRepo.Setup(x => x.GetById(id)).Returns(item);
        _backlogRepo.Setup(x => x.GetSprintForBacklogItem(id)).Returns(sprint);
        _backlogRepo.Setup(x => x.Update(id, item)).Returns(true);
        _sprintRepo.Setup(x => x.GetMembersByRole(sprintId, SprintRole.Tester)).Returns(testers);

        var service = CreateService();

        var result = service.ReturnToReadyForTesting(id);

        Assert.True(result);
        _notifications.Verify(x => x.NotifyReadyForTesting(item.Title, testers), Times.Once);
    }

    // 🔹 5. ReturnToTodo notificatie
    [Fact]
    public void ReturnToTodo_NotifiesScrumMaster()
    {
        var id = Guid.NewGuid();
        var sprintId = Guid.NewGuid();
        var item = CreateItem(id);
        var sprint = CreateSprint(sprintId);
        sprint.AddMember(CreateMember("Sanne Jansen", SprintRole.ScrumMaster));
        var scrumMasters = GetMembersByRole(sprint, SprintRole.ScrumMaster);

        item.MarkReadyForTesting();
        item.StartTesting();

        _backlogRepo.Setup(x => x.GetById(id)).Returns(item);
        _backlogRepo.Setup(x => x.GetSprintForBacklogItem(id)).Returns(sprint);
        _backlogRepo.Setup(x => x.Update(id, item)).Returns(true);
        _sprintRepo.Setup(x => x.GetMembersByRole(sprintId, SprintRole.ScrumMaster)).Returns(scrumMasters);

        var service = CreateService();

        var result = service.ReturnToTodo(id);

        Assert.True(result);
        _notifications.Verify(x => x.NotifyBackToTodo(item.Title, item.LastDeveloper, scrumMasters), Times.Once);
    }

    // 🔹 6. AssignDeveloper edge cases
    [Fact]
    public void AssignDeveloper_ReturnsFalse_WhenInvalid()
    {
        var backlogItemId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _backlogRepo.Setup(x => x.GetById(backlogItemId)).Returns((BacklogItem?)null);

        var service = CreateService();

        Assert.False(service.AssignDeveloper(backlogItemId, userId));
    }

    [Fact]
    public void AssignDeveloper_Updates_WhenValid()
    {
        var backlogItemId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var item = new BacklogItem(backlogItemId, "Item", "Desc", 2);
        var user = CreateUser("Dev");
        user.Id = userId;

        _backlogRepo.Setup(x => x.GetById(backlogItemId)).Returns(item);
        _userRepo.Setup(x => x.GetById(userId)).Returns(user);
        _backlogRepo.Setup(x => x.Update(backlogItemId, item)).Returns(true);

        var service = CreateService();

        var result = service.AssignDeveloper(backlogItemId, userId);

        Assert.True(result);
    }
}