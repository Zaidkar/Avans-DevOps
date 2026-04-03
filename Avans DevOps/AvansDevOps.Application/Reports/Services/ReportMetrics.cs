using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Reports.Services
{
public class ReportMetricsCalculator
{
    public ReportData Calculate(Sprint sprint, List<BacklogItem> backlogItems)
    {
        var totalPoints = backlogItems.Sum(x => x.StoryPoints);

        var completedPoints = backlogItems
            .Where(x => x.CurrentState == "Done")
            .Sum(x => x.StoryPoints);

        var remainingPoints = Math.Max(0, totalPoints - completedPoints);

        var durationDays = Math.Max(1, sprint.EndDate.DayNumber - sprint.StartDate.DayNumber + 1);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var elapsedRaw = today.DayNumber - sprint.StartDate.DayNumber + 1;
        var elapsedDays = Math.Clamp(elapsedRaw, 1, durationDays);

        var plannedRate = totalPoints / (double)durationDays;
        var projectedRate = completedPoints / (double)elapsedDays;

        var burndownPoints = Enumerable
            .Range(0, durationDays + 1)
            .Select(day =>
            {
                var planned = Math.Max(0, totalPoints - (plannedRate * day));
                var projected = Math.Max(0, totalPoints - (projectedRate * day));
                return (Day: day, Planned: planned, Projected: projected);
            })
            .ToList();

        var effortByDeveloper = backlogItems
            .Where(item => item.AssignedDeveloper != null || item.LastDeveloper != null)
            .GroupBy(item => (item.AssignedDeveloper ?? item.LastDeveloper)!.Name)
            .Select(group =>
            (
                DeveloperName: group.Key,
                Total: group.Sum(x => x.StoryPoints),
                Completed: group.Where(x => x.CurrentState == "Done").Sum(x => x.StoryPoints)
            ))
            .OrderBy(x => x.DeveloperName)
            .ToList();

        return new ReportData
        {
            SprintName = sprint.Name,
            Members = sprint.Members,
            TotalPoints = totalPoints,
            CompletedPoints = completedPoints,
            RemainingPoints = remainingPoints,
            DurationDays = durationDays,
            ElapsedDays = elapsedDays,
            BurndownPoints = burndownPoints,
            EffortByDeveloper = effortByDeveloper
        };
    }
}
}