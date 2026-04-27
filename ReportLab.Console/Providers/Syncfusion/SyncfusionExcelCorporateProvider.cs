using Syncfusion.XlsIO;
using ReportLab.Console.Core;
// Vi använder ett alias för att undvika namespace-krockar
using Color = Syncfusion.Drawing.Color;

namespace ReportLab.Console.Providers.Syncfusion;

public class SyncfusionExcelCorporateProvider : SyncfusionExcelBaseProvider
{
    public override string Name => "Syncfusion_Excel_Corporate";

    protected override void FillWorkbook(IWorkbook workbook, ReportModel model)
    {
        var sheet = workbook.Worksheets[0];
        sheet.Name = "Data Analysis";

        sheet["A1"].Text = model.Title;
        sheet["A1"].CellStyle.Font.Bold = true;
        sheet["A1"].CellStyle.Font.Size = 16;
        sheet["A1:B1"].Merge();

        sheet["A3"].Text = "Metric";
        sheet["B3"].Text = "Value";

        int row = 4;
        foreach (var stat in model.Stats)
        {
            sheet.Range[row, 1].Text = stat.Label;
            sheet.Range[row, 2].Number = stat.Value;
            row++;
        }

        // FIX: Operatorn heter 'Less', inte 'LessThan'
        var condition = sheet.Range[$"B4:B{row - 1}"].ConditionalFormats.AddCondition();
        condition.FormatType = ExcelCFType.CellValue;
        condition.Operator = ExcelComparisonOperator.Less;
        condition.FirstFormula = "3";

        // FIX: Använd global referens för att undvika CS0234
        condition.BackColorRGB = global::Syncfusion.Drawing.Color.FromArgb(255, 199, 206);
        condition.FontColorRGB = global::Syncfusion.Drawing.Color.FromArgb(156, 0, 6);

        var chart = sheet.Charts.Add();
        chart.DataRange = sheet.Range[$"A4:B{row - 1}"];
        chart.ChartType = ExcelChartType.Column_Clustered;
        chart.TopRow = 4;
        chart.LeftColumn = 4;
        chart.RightColumn = 12;
        chart.BottomRow = 20;
    }
}