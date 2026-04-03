using System.Text;
using Avans_DevOps.AvansDevOps.Domain.Entities;

namespace Avans_DevOps.AvansDevOps.Application.Reports.Services;

public class TextReportBuilder : IReportBuilder
{
    private readonly StringBuilder _content = new();

    public IReportBuilder AddHeader(string sprintLabel, string projectName, string logoLabel, string companyName, string version, string sprintName)
    {
        _content.AppendLine($"[{sprintLabel}] {projectName}");
        _content.AppendLine($"Logo: {logoLabel}");
        _content.AppendLine($"Bedrijfsnaam: {companyName}");
        _content.AppendLine($"Projectnaam: {projectName}");
        _content.AppendLine($"Versie: {version}");
        _content.AppendLine($"Sprint: {sprintName}");
        _content.AppendLine();

        return this;
    }

    public IReportBuilder AddTeamSection(IEnumerable<SprintMember> members)
    {
        _content.AppendLine("-- Teamsamenstelling --");
        foreach (var role in members.GroupBy(x => x.SprintRole.ToString()).OrderBy(x => x.Key))
        {
            _content.AppendLine($"{role.Key}: {role.Count()}");
        }

        _content.AppendLine("Leden:");
        foreach (var member in members.OrderBy(x => x.User.Name))
        {
            _content.AppendLine($"- {member.User.Name} ({member.SprintRole})");
        }

        return this;
    }

    public IReportBuilder AddBurndownSection(int totalPoints, int completedPoints, int remainingPoints, int durationDays, int elapsedDays, IEnumerable<(int Day, double Planned, double Projected)> burndownPoints)
    {
        _content.AppendLine();
        _content.AppendLine("-- Burndown projectie --");
        _content.AppendLine($"Totaal punten: {totalPoints}");
        _content.AppendLine($"Voltooid: {completedPoints}");
        _content.AppendLine($"Resterend: {remainingPoints}");
        _content.AppendLine($"Sprintdagen: {durationDays}");
        _content.AppendLine($"Verstreken dagen: {elapsedDays}");
        _content.AppendLine("Burndown punten (dag -> gepland/projectie):");

        foreach (var point in burndownPoints)
        {
            _content.AppendLine($"Dag {point.Day}: {point.Planned:0.##}/{point.Projected:0.##}");
        }

        return this;
    }

    public IReportBuilder AddEffortSection(IEnumerable<(string DeveloperName, int Total, int Completed)> effortByDeveloper)
    {
        _content.AppendLine();
        _content.AppendLine("-- Effort per developer --");
        foreach (var effort in effortByDeveloper)
        {
            _content.AppendLine($"{effort.DeveloperName}: toegewezen={effort.Total}, voltooid={effort.Completed}");
        }

        return this;
    }

    public IReportBuilder AddFooter(string sprintLabel, DateTime generatedAtUtc, string extraInfo)
    {
        _content.AppendLine();
        _content.AppendLine("-- Footer --");
        _content.AppendLine($"Sprintlabel: {sprintLabel}");
        _content.AppendLine($"Datum (UTC): {generatedAtUtc:O}");
        _content.AppendLine($"Info: {extraInfo}");

        return this;
    }

    public string Build()
    {
        return _content.ToString();
    }
}
