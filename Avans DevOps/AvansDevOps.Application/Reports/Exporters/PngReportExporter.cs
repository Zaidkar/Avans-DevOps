namespace Avans_DevOps.AvansDevOps.Application.Reports.Exporters
{
    public class PngReportExporter : IReportExporter
    {
        public string Export(string reportContent, string sprintName, string outputDirectory)
        {
            return Write(reportContent, sprintName, outputDirectory, "png");
        }

        private static string Write(string reportContent, string sprintName, string outputDirectory, string extension)
        {
            Directory.CreateDirectory(outputDirectory);
            var fileName = $"{SanitizeFileName(sprintName)}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.{extension}";
            var outputPath = Path.Combine(outputDirectory, fileName);
            File.WriteAllText(outputPath, reportContent);
            return outputPath;
        }

        private static string SanitizeFileName(string value)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var safe = new string(value.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
            return string.IsNullOrWhiteSpace(safe) ? "sprint-report" : safe;
        }
    }
}