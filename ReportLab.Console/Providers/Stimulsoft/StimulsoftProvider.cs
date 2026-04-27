using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.Stimulsoft;

public class StimulsoftProvider : IReportProvider
{
    public string Name => "Stimulsoft_Trial";

    public void Export(ReportModel model, string filePath)
    {
        var report = new StiReport();
        report.RegData("Data", model.Stats);

        // Simple way: Load an empty report and add a header
        var page = report.Pages[0];
        var header = new StiHeaderBand { Height = 1.0 };
        page.Components.Add(header);

        var title = new StiText(new global::Stimulsoft.Base.Drawing.RectangleD(0, 0, 10, 1))
        {
            Text = model.Title,
            Font = new System.Drawing.Font("Arial", 20)
        };
        header.Components.Add(title);

        report.Render(false);
        report.ExportDocument(StiExportFormat.Pdf, filePath);
    }
}