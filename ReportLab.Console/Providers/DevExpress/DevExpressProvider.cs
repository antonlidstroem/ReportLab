// DevExpress Example

using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.DevExpress;

public class DevExpressProvider : IReportProvider
{
    public string Name => "DevExpress_Trial";
    public void Export(ReportModel model, string filePath)
    {
        // DevExpress uses 'XtraReport'
        // var report = new XtraReport();
        // ... build report ...
        // report.ExportToPdf(filePath);
    }
}