namespace Avans_DevOps.AvansDevOps.Application.Reports.Exporters
{
    public interface IReportExporter
    {
        string Export(string reportContent, string sprintName, string outputDirectory);
    }
}