using System.IO;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using ReportLab.Console.Core;
using ReportLab.Console.Providers.ScottPlot;

namespace ReportLab.Console.Providers;

public class ClosedXMLProvider : IReportProvider
{
    public string Name => "Excel";

    public void Export(ReportModel model, string filePath)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Skyddsrond");

        // 1. Header
        worksheet.Cell("A1").Value = model.Title;
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A2").Value = $"Inspektör: {model.Inspector} ({model.Date})";

        // 2. Tabell-data
        worksheet.Cell("A4").Value = "Område";
        worksheet.Cell("B4").Value = "Värde";

        for (int i = 0; i < model.Stats.Count; i++)
        {
            worksheet.Cell(i + 5, 1).Value = model.Stats[i].Label;
            worksheet.Cell(i + 5, 2).Value = model.Stats[i].Value;
        }

        // 3. FIX för "Picture too large": 
        // Vi skapar bilden och tvingar ClosedXML att hantera den som en PNG
        var chartImage = ChartGenerator.CreateBarChart(model.Stats);
        using var ms = new MemoryStream(chartImage);

        // Vi använder en säkrare metod för att lägga till bilden
        var picture = worksheet.AddPicture(ms, XLPictureFormat.Png, "Graf");

        // Placera bilden och skala den (0.7 = 70% storlek)
        picture.MoveTo(worksheet.Cell("D4")).Scale(0.7);

        workbook.SaveAs(filePath);
    }
}