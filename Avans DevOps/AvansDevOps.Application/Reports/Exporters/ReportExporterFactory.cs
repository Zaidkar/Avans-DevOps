namespace Avans_DevOps.AvansDevOps.Application.Reports.Exporters
{
    public static class ReportExporterFactory
    {
        public static IReportExporter Create(ReportFormat format)
        {
            // Design Pattern: Factory Method. De factory kiest het juiste exporter-object.
            return format switch
            {
                ReportFormat.Pdf => new PdfReportExporter(),
                ReportFormat.Png => new PngReportExporter(),
                _ => throw new NotSupportedException($"Format {format} wordt niet ondersteund.")
            };
        }
    }
}
