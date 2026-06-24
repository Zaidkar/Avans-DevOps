using System;
using Avans_DevOps.AvansDevOps.Application.Notifications.Simple;
using Avans_DevOps.AvansDevOps.Application.Reports.Services;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;
using Moq;
using Xunit;

namespace AvansDevOps.Tests;

public class ReportGeneratorTests
{
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
        var eventManager = new EventManager();
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
            SprintGoalType.Review, eventManager);

        sprint.AddMember(developer, SprintRole.Developer);

        var doneItem = CreateDoneItem("Feature A", 3, developer);
        var todoItem = CreateTodoItem("Feature B", 5, developer);

        sprint.AddBacklogItem(doneItem);
        sprint.AddBacklogItem(todoItem);

        

        var generator = new ReportGenerator(
            sprint,
            new ReportMetricsCalculator());

        return (generator, sprint);
    }

    private static BacklogItem CreateDoneItem(string title, int storyPoints, User developer)
    {
        var eventManager = new EventManager();
        var item = new BacklogItem(Guid.NewGuid(), title, title, storyPoints, eventManager);
        item.AssignDeveloper(developer);
        item.MarkReadyForTesting();
        item.StartTesting();
        item.MarkTested();
        item.ApproveDone();
        return item;
    }

    private static BacklogItem CreateTodoItem(string title, int storyPoints, User developer)
    {
        var eventManager = new EventManager();
        var item = new BacklogItem(Guid.NewGuid(), title, title, storyPoints, eventManager);
        item.AssignDeveloper(developer);
        return item;
    }
}
