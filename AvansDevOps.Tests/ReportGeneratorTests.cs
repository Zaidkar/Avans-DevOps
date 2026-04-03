using Avans_DevOps.AvansDevOps.Application.Repositories;
using Avans_DevOps.AvansDevOps.Application.Reports.Services;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Moq;

namespace AvansDevOps.Tests;

public class ReportGeneratorTests
{
    [Fact]
    public void Generate_Throws_WhenSprintDoesNotExist()
    {
        var sprintRepository = new Mock<ISprintRepository>();
        var backlogRepository = new Mock<IBacklogItemRepository>();
        sprintRepository.Setup(x => x.GetById(It.IsAny<Guid>())).Returns((Sprint?)null);

        var generator = new ReportGenerator(
            sprintRepository.Object,
            backlogRepository.Object,
            new ReportMetricsCalculator());

        var action = () => generator.Generate(
            Guid.NewGuid(),
            "Avans",
            "Project X",
            "1.0",
            "Logo",
            "Sprintrapport",
            "Extra");

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Generate_IncludesExpectedSectionsAndMetadata()
    {
        var (generator, sprint) = CreateGeneratorWithSprintAndItems();

        var report = generator.Generate(
            sprint.Id,
            "Avans",
            "Project X",
            "2.1",
            "LOGO",
            "Sprintrapport",
            "Footer info");

        Assert.Contains("[Sprintrapport] Project X", report);
        Assert.Contains("Bedrijfsnaam: Avans", report);
        Assert.Contains("-- Teamsamenstelling --", report);
        Assert.Contains("-- Burndown projectie --", report);
        Assert.Contains("-- Effort per developer --", report);
        Assert.Contains("-- Footer --", report);
        Assert.Contains("Info: Footer info", report);
    }

    [Fact]
    public void Generate_ComputesBurndownAndEffortValues()
    {
        var (generator, sprint) = CreateGeneratorWithSprintAndItems();

        var report = generator.Generate(
            sprint.Id,
            "Avans",
            "Project X",
            "2.1",
            "LOGO",
            "Sprintrapport",
            "Footer info");

        Assert.Contains("Totaal punten: 8", report);
        Assert.Contains("Voltooid: 3", report);
        Assert.Contains("Resterend: 5", report);
        Assert.Contains("Dev One: toegewezen=8, voltooid=3", report);
        Assert.Contains("Burndown punten (dag -> gepland/projectie):", report);
    }

    private static (ReportGenerator Generator, Sprint Sprint) CreateGeneratorWithSprintAndItems()
    {
        var sprintRepository = new Mock<ISprintRepository>();
        var backlogRepository = new Mock<IBacklogItemRepository>();

        var developer = new User
        {
            Id = Guid.NewGuid(),
            Name = "Dev One",
            Email = "dev.one@avans.dev"
        };

        var sprint = new Sprint(
            Guid.NewGuid(),
            "Sprint 1",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            SprintGoalType.Review);

        sprint.AddMember(new SprintMember(Guid.NewGuid(), developer, SprintRole.Developer));

        var doneItem = CreateDoneItem("Feature A", 3, developer);
        var todoItem = CreateTodoItem("Feature B", 5, developer);

        sprint.AddBacklogItem(doneItem.Id);
        sprint.AddBacklogItem(todoItem.Id);

        sprintRepository.Setup(x => x.GetById(sprint.Id)).Returns(sprint);
        backlogRepository
            .Setup(x => x.GetAll())
            .Returns(new List<(Guid Id, BacklogItem Item)>
            {
                (doneItem.Id, doneItem),
                (todoItem.Id, todoItem)
            });

        var generator = new ReportGenerator(
            sprintRepository.Object,
            backlogRepository.Object,
            new ReportMetricsCalculator());

        return (generator, sprint);
    }

    private static BacklogItem CreateDoneItem(string title, int storyPoints, User developer)
    {
        var item = new BacklogItem(Guid.NewGuid(), title, title, storyPoints);
        item.AssignDeveloper(developer);
        item.MarkReadyForTesting();
        item.StartTesting();
        item.MarkTested();
        item.ApproveDone();
        return item;
    }

    private static BacklogItem CreateTodoItem(string title, int storyPoints, User developer)
    {
        var item = new BacklogItem(Guid.NewGuid(), title, title, storyPoints);
        item.AssignDeveloper(developer);
        return item;
    }
}
