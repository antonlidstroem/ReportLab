using FastReport;
using FastReport.Utils;
using ReportLab.Console.Core;
using System.Drawing;

namespace ReportLab.Console.Providers.FastReport;

public class FastReportProvider : IReportProvider
{
    public string Name => "FastReport_Trial";

    public void Export(ReportModel model, string filePath)
    {
        using Report report = new Report();

        // Add data to the report
        report.RegisterData(model.Stats, "Stats");

        // Create a page
        ReportPage page = new ReportPage();
        report.Pages.Add(page);
        page.CreateUniqueName();

        // Title Band
        page.ReportTitle = new ReportTitleBand { Height = Units.Centimeters * 2 };
        TextObject titleText = new TextObject
        {
            Bounds = new RectangleF(0, 0, Units.Centimeters * 10, Units.Centimeters * 1),
            Text = model.Title,
            Font = new Font("Arial", 16, FontStyle.Bold)
        };
        page.ReportTitle.Objects.Add(titleText);

        // Data Band
        DataBand data = new DataBand
        {
            Height = Units.Centimeters * 0.5f,
            DataSource = report.GetDataSource("Stats")
        };
        page.Bands.Add(data);

        TextObject statText = new TextObject
        {
            Bounds = new RectangleF(0, 0, Units.Centimeters * 5, Units.Centimeters * 0.5f),
            Text = "[Stats.Label]: [Stats.Value]"
        };
        data.Objects.Add(statText);

        report.Prepare();

        using var export = new global::FastReport.Export.Pdf.PDFExport();
        report.Export(export, filePath);
    }
}