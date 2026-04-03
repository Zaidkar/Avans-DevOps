using Avans_DevOps.AvansDevOps.Application.Repositories.Fakes;
using Avans_DevOps.AvansDevOps.Domain.Entities;
using Avans_DevOps.AvansDevOps.Domain.Enum;

namespace Avans_DevOps.AvansDevOps.Application.Reports.Services;

public class ReportDemoRunner
{
    public void Run()
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [ReportDemoRunner] Run");

        var sprintRepository = new FakeSprintRepository();
        var backlogItemRepository = new FakeBacklogItemRepository(sprintRepository);
        var generator = new ReportGenerator(sprintRepository, backlogItemRepository, new ReportMetricsCalculator());

        var sprint = new Sprint(
            Guid.NewGuid(),
            "Sprint Report Demo",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            SprintGoalType.Review);

        var developer = new User
        {
            Id = Guid.NewGuid(),
            Name = "Developer Demo",
            Email = "developer.demo@avans.dev"
        };

        sprint.AddMember(new SprintMember(Guid.NewGuid(), developer, SprintRole.Developer));

        var doneItem = new BacklogItem(Guid.NewGuid(), "Login", "Implement login flow", 5);
        doneItem.AssignDeveloper(developer);
        doneItem.MarkReadyForTesting();
        doneItem.StartTesting();
        doneItem.MarkTested();
        doneItem.ApproveDone();

        var todoItem = new BacklogItem(Guid.NewGuid(), "Dashboard", "Add overview dashboard", 8);
        todoItem.AssignDeveloper(developer);

        sprint.AddBacklogItem(doneItem.Id);
        sprint.AddBacklogItem(todoItem.Id);

        sprintRepository.Create(sprint);
        backlogItemRepository.Create(doneItem);
        backlogItemRepository.Create(todoItem);

        var report = generator.Generate(
            sprint.Id,
            "Avans",
            "Avans DevOps",
            "1.0.0",
            "AVANS-LOGO",
            "Sprintrapport",
            "Demo-output voor handmatige controle");

        Console.WriteLine("[Demo] Generated sprint report:");
        Console.WriteLine(report);
    }
}
