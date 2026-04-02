using Avans_DevOps.AvansDevOps.Application.Repositories;

namespace Avans_DevOps.AvansDevOps.Application.Reports.Services
{
    public class ReportGenerator(ISprintRepository sprintRepository, IBacklogItemRepository backlogItemRepository)
    {
        private readonly ISprintRepository _sprintRepository = sprintRepository;
        private readonly IBacklogItemRepository _backlogItemRepository = backlogItemRepository;

        public string Generate(Guid sprintId, string companyName, string projectName, string version, string logoLabel, string sprintLabel, string extraInfo)
        {
            var sprint = _sprintRepository.GetById(sprintId)
                ?? throw new InvalidOperationException($"Sprint with id {sprintId} does not exist.");

            var backlogItems = _backlogItemRepository
                .GetAll()
                .Where(x => sprint.BacklogItemIds.Contains(x.Id))
                .Select(x => x.Item)
                .ToList();

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

            var effortByDeveloper = backlogItems
                .Where(item => item.AssignedDeveloper != null || item.LastDeveloper != null)
                .GroupBy(item => (item.AssignedDeveloper ?? item.LastDeveloper)!.Name)
                .Select(group => new { DeveloperName = group.Key, Total = group.Sum(x => x.StoryPoints), Completed = group.Where(x => x.CurrentState == "Done").Sum(x => x.StoryPoints) })
                .OrderBy(x => x.DeveloperName)
                .ToList();

            var content = new System.Text.StringBuilder();
            content.AppendLine($"[{sprintLabel}] {projectName}");
            content.AppendLine($"Logo: {logoLabel}");
            content.AppendLine($"Bedrijfsnaam: {companyName}");
            content.AppendLine($"Projectnaam: {projectName}");
            content.AppendLine($"Versie: {version}");
            content.AppendLine($"Sprint: {sprint.Name}");
            content.AppendLine();

            content.AppendLine("-- Teamsamenstelling --");
            foreach (var role in sprint.Members.GroupBy(x => x.SprintRole.ToString()).OrderBy(x => x.Key))
            {
                content.AppendLine($"{role.Key}: {role.Count()}");
            }

            content.AppendLine("Leden:");
            foreach (var member in sprint.Members.OrderBy(x => x.User.Name))
            {
                content.AppendLine($"- {member.User.Name} ({member.SprintRole})");
            }

            content.AppendLine();
            content.AppendLine("-- Burndown projectie --");
            content.AppendLine($"Totaal punten: {totalPoints}");
            content.AppendLine($"Voltooid: {completedPoints}");
            content.AppendLine($"Resterend: {remainingPoints}");
            content.AppendLine($"Sprintdagen: {durationDays}");
            content.AppendLine($"Verstreken dagen: {elapsedDays}");
            content.AppendLine("Burndown punten (dag -> gepland/projectie):");
            for (var day = 0; day <= durationDays; day++)
            {
                var planned = Math.Max(0, totalPoints - (plannedRate * day));
                var projected = Math.Max(0, totalPoints - (projectedRate * day));
                content.AppendLine($"Dag {day}: {planned:0.##}/{projected:0.##}");
            }

            content.AppendLine();
            content.AppendLine("-- Effort per developer --");
            foreach (var effort in effortByDeveloper)
            {
                content.AppendLine($"{effort.DeveloperName}: toegewezen={effort.Total}, voltooid={effort.Completed}");
            }

            content.AppendLine();
            content.AppendLine("-- Footer --");
            content.AppendLine($"Sprintlabel: {sprintLabel}");
            content.AppendLine($"Datum (UTC): {DateTime.UtcNow:O}");
            content.AppendLine($"Info: {extraInfo}");

            return content.ToString();
        }
    }
}