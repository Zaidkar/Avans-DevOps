using Avans_DevOps.AvansDevOps.Application.Repositories;

namespace Avans_DevOps.AvansDevOps.Application.Reports.Services
{
   public class ReportGenerator(
    ISprintRepository sprintRepository,
    IBacklogItemRepository backlogItemRepository,
    ReportMetricsCalculator calculator)
{
    private readonly ISprintRepository _sprintRepository = sprintRepository;
    private readonly IBacklogItemRepository _backlogItemRepository = backlogItemRepository;
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
        var sprint = _sprintRepository.GetById(sprintId)
            ?? throw new InvalidOperationException($"Sprint with id {sprintId} does not exist.");

        var backlogItems = _backlogItemRepository
            .GetAll()
            .Where(x => sprint.BacklogItemIds.Contains(x.Id))
            .Select(x => x.Item)
            .ToList();

        var data = _calculator.Calculate(sprint, backlogItems);

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