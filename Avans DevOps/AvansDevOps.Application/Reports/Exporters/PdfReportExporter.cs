namespace Avans_DevOps.AvansDevOps.Application.Reports.Exporters
{
    public class PdfReportExporter : TextFileReportExporterBase
    {
        protected override string Extension => "pdf";
    }
}