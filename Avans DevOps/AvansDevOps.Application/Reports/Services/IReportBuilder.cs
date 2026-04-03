using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Reports.Services;

public interface IReportBuilder
{
    IReportBuilder AddHeader(string sprintLabel, string projectName, string logoLabel, string companyName, string version, string sprintName);
    IReportBuilder AddTeamSection(IEnumerable<SprintMember> members);
    IReportBuilder AddBurndownSection(int totalPoints, int completedPoints, int remainingPoints, int durationDays, int elapsedDays, IEnumerable<(int Day, double Planned, double Projected)> burndownPoints);
    IReportBuilder AddEffortSection(IEnumerable<(string DeveloperName, int Total, int Completed)> effortByDeveloper);
    IReportBuilder AddFooter(string sprintLabel, DateTime generatedAtUtc, string extraInfo);
    string Build();
}
