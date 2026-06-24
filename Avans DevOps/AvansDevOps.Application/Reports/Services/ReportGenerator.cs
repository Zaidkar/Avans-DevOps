
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Reports.Services
{
   public class ReportGenerator(
   Sprint sprint,
    ReportMetricsCalculator calculator)
{
    private readonly ReportMetricsCalculator _calculator = calculator;

    public string Generate(
        Guid sprintId,
        string companyName,
        string projectName,
        string version,
        string logoLabel,
        string sprintLabel,
        string extraInfo)
    {
        var backlogItems = sprint.BacklogItems.ToList();

        var data = ReportMetricsCalculator.Calculate(sprint, backlogItems);

        var builder = new TextReportBuilder();

        return builder
            .AddHeader(sprintLabel, projectName, logoLabel, companyName, version, data.SprintName)
            .AddTeamSection(data.Members)
            .AddBurndownSection(
                data.TotalPoints,
                data.CompletedPoints,
                data.RemainingPoints,
                data.DurationDays,
                data.ElapsedDays,
                data.BurndownPoints)
            .AddEffortSection(data.EffortByDeveloper)
            .AddFooter(sprintLabel, DateTime.UtcNow, extraInfo)
            .Build();
    }
}
}