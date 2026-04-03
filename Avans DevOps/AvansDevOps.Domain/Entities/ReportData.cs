using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities
{
   public class ReportData
{
    public string SprintName { get; set; } = "";
    public IEnumerable<SprintMember> Members { get; set; } = [];

    public int TotalPoints { get; set; }
    public int CompletedPoints { get; set; }
    public int RemainingPoints { get; set; }

    public int DurationDays { get; set; }
    public int ElapsedDays { get; set; }

    public List<(int Day, double Planned, double Projected)> BurndownPoints { get; set; } = [];

    public List<(string DeveloperName, int Total, int Completed)> EffortByDeveloper { get; set; } = [];
}
}