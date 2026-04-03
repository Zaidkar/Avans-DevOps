using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Application.Notifications.Models;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple.Strategies;
using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients;
using Moq;

namespace AvansDevOps.Tests;

public class NotificationConsoleTests
{
    private static User CreateUser(string name, string email) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            SlackChannel = $"#{name.Replace(" ", "-").ToLowerInvariant()}",
            PhoneNumber = "+31600000000"
        };

    private static SprintMember CreateMember(User user, SprintRole role) =>
        new(Guid.NewGuid(), user, role);

    private static string CaptureConsole(Action action)
    {
        var original = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            action();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    [Fact]
    public void TC_27_FR_07_FR_07_2_ReadyForTesting_EmailConsoleContainsBodyAndRecipient()
    {
        var sprintId = Guid.NewGuid();
        var tester = CreateUser("Tester One", "tester.one@avans.dev");
        var testerMember = CreateMember(tester, SprintRole.Tester);
        var sprintRepository = new Mock<ISprintRepository>();
        sprintRepository.Setup(x => x.GetMembersByRole(sprintId, SprintRole.Tester)).Returns(new List<SprintMember> { testerMember });

        var factory = new NotificationStrategyFactory(new ExternalMailClient(), new SlackSdk(), new SmsSdk());
        var eventManager = new EventManager();
        eventManager.Subscribe(
            NotificationEventNames.ReadyForTesting,
            new BacklogItemListener(sprintRepository.Object, factory, new[] { SprintRole.Tester }, new[] { ChannelType.Email }));

        var output = CaptureConsole(() =>
            eventManager.Notify(NotificationEventNames.ReadyForTesting, new NotificationEventData
            {
                SprintId = sprintId,
                Subject = "Backlog item ready for testing",
                Body = "Backlogitem 1 is ready for testing"
            }));

        Assert.Contains("Backlogitem 1 is ready for testing", output);
        Assert.Contains("to: tester.one@avans.dev", output);
        Assert.Contains("subject: Backlog item ready for testing", output);
    }

    [Fact]
    public void TC_28_FR_07_ReleaseSuccess_EmailConsoleContainsSprintAndRecipient()
    {
        var sprintId = Guid.NewGuid();
        var scrumMaster = CreateUser("Scrum Master", "scrummaster@avans.dev");
        var scrumMasterMember = CreateMember(scrumMaster, SprintRole.ScrumMaster);
        var sprintRepository = new Mock<ISprintRepository>();
        sprintRepository.Setup(x => x.GetMembersByRole(sprintId, SprintRole.ScrumMaster)).Returns(new List<SprintMember> { scrumMasterMember });

        var factory = new NotificationStrategyFactory(new ExternalMailClient(), new SlackSdk(), new SmsSdk());
        var eventManager = new EventManager();
        eventManager.Subscribe(
            NotificationEventNames.ReleaseFailure,
            new SprintNotificationListener(sprintRepository.Object, factory, new[] { ChannelType.Email }, new[] { SprintRole.ScrumMaster }));

        var output = CaptureConsole(() =>
            eventManager.Notify(NotificationEventNames.ReleaseFailure, new NotificationEventData
            {
                SprintId = sprintId,
                Subject = "Pipeline activity failed",
                Body = "A pipeline activity failed during the release of sprint Sprint 42."
            }));

        Assert.Contains("A pipeline activity failed during the release of sprint Sprint 42.", output);
        Assert.Contains("to: scrummaster@avans.dev", output);
        Assert.Contains("subject: Pipeline activity failed", output);
    }

    [Fact]
    public void TC_29_FR_07_2_DiscussionReply_UsesEmailAndSlackChannels()
    {
        var sprintId = Guid.NewGuid();
        var sprintRepository = new Mock<ISprintRepository>();
        var teamMember = CreateUser("Developer One", "developer.one@avans.dev");
        var teamSprintMember = CreateMember(teamMember, SprintRole.Developer);
        sprintRepository.Setup(x => x.GetMembers(sprintId)).Returns(new List<SprintMember> { teamSprintMember });

        var emailChannel = new Mock<INotificationStrategy>();
        var slackChannel = new Mock<INotificationStrategy>();
        var factory = new Mock<INotificationStrategyFactory>();
        factory.Setup(x => x.Create(ChannelType.Email)).Returns(emailChannel.Object);
        factory.Setup(x => x.Create(ChannelType.Slack)).Returns(slackChannel.Object);

        var listener = new DiscussionNotificationListener(
            sprintRepository.Object,
            factory.Object,
            new[] { ChannelType.Email, ChannelType.Slack });

        var output = CaptureConsole(() =>
            listener.Update(new NotificationEventData
            {
                SprintId = sprintId,
                Subject = "Discussion reply",
                Body = "Er is een nieuwe reactie geplaatst over Refinement"
            }));

        Assert.Equal(string.Empty, output);
        emailChannel.Verify(x => x.Execute(It.IsAny<NotificationMessage>(), It.Is<List<SprintMember>>(list => list.Count == 1)), Times.Once);
        slackChannel.Verify(x => x.Execute(It.IsAny<NotificationMessage>(), It.Is<List<SprintMember>>(list => list.Count == 1)), Times.Once);
    }
}
