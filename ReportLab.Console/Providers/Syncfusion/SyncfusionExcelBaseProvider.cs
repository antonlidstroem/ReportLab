using Syncfusion.XlsIO;
using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.Syncfusion;

public abstract class SyncfusionExcelBaseProvider : IReportProvider
{
    public abstract string Name { get; }

    protected abstract void FillWorkbook(IWorkbook workbook, ReportModel model);

    public void Export(ReportModel model, string filePath)
    {
        using var excelEngine = new ExcelEngine();
        var application = excelEngine.Excel;
        application.DefaultVersion = ExcelVersion.Xlsx;

        var workbook = application.Workbooks.Create(1);
        FillWorkbook(workbook, model);

        string finalPath = filePath.EndsWith(".xlsx") ? filePath : filePath + ".xlsx";
        using var fileStream = new FileStream(finalPath, FileMode.Create, FileAccess.Write);
        workbook.SaveAs(fileStream);
    }
}