using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Drawing;
using ReportLab.Console.Core;
using ReportLab.Console.Providers.ScottPlot;

namespace ReportLab.Console.Providers.Syncfusion;

public class SyncfusionCorporateProvider : SyncfusionPdfBaseProvider
{
    public override string Name => "Syncfusion_Corporate_MAX";

    protected override void DrawContent(PdfPage page, PdfGraphics graphics, ReportModel model)
    {
        var primaryBlue = new PdfColor(21, 101, 192);
        var lightGrey = new PdfColor(245, 245, 245);
        var size = page.GetClientSize();

        // 1. ADVANCED HEADER (Med shapes och transparens)
        graphics.DrawRectangle(new PdfSolidBrush(primaryBlue), new RectangleF(0, 0, size.Width, 80));
        graphics.DrawRectangle(new PdfSolidBrush(new PdfColor(255, 255, 255, 40)), new RectangleF(size.Width - 150, 0, 150, 80));

        graphics.DrawString(model.Title.ToUpper(), GetFont("Helvetica", 22, PdfFontStyle.Bold), PdfBrushes.White, new PointF(25, 20));
        graphics.DrawString("CONFIDENTIAL // EXTERNAL AUDIT", GetFont("Helvetica", 9), PdfBrushes.White, new PointF(27, 50));

        // 2. SUMMARY BOX
        graphics.DrawRectangle(new PdfSolidBrush(lightGrey), new RectangleF(0, 100, size.Width, 40));
        graphics.DrawString($"ID: {model.Date.Replace("-", "")} | INSPECTOR: {model.Inspector.ToUpper()}",
            GetFont("Helvetica", 10, PdfFontStyle.Bold), new PdfSolidBrush(primaryBlue), new PointF(25, 112));

        // 3. DATA VISUALIZATION
        var chartBytes = ChartGenerator.CreateBarChart(model.Stats);
        using var ms = new MemoryStream(chartBytes);
        graphics.DrawImage(PdfImage.FromStream(ms), 0, 160, size.Width, 220);

        // 4. ENTERPRISE GRID (Med avancerad styling)
        PdfGrid grid = new PdfGrid();
        grid.Columns.Add(3);
        grid.Columns[1].Width = 80;
        grid.Columns[2].Width = 120;

        PdfGridRow header = grid.Headers.Add(1)[0];
        header.Cells[0].Value = "INSPECTION AREA";
        header.Cells[1].Value = "SCORE";
        header.Cells[2].Value = "COMPLIANCE";

        var hStyle = new PdfGridCellStyle
        {
            BackgroundBrush = new PdfSolidBrush(primaryBlue),
            TextBrush = PdfBrushes.White,
            Font = GetFont("Helvetica", 11, PdfFontStyle.Bold),
            StringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle }
        };
        for (int i = 0; i < 3; i++) header.Cells[i].Style = hStyle;

        foreach (var stat in model.Stats)
        {
            PdfGridRow row = grid.Rows.Add();
            row.Height = 25;
            row.Cells[0].Value = stat.Label;
            row.Cells[1].Value = stat.Value.ToString("F1");
            row.Cells[2].Value = stat.Value >= 3 ? "COMPLIANT" : "NON-COMPLIANT";

            // Villkorlig formatering (Rött för låga värden)
            if (stat.Value < 3)
            {
                row.Cells[1].Style.TextBrush = PdfBrushes.DarkRed;
                row.Cells[2].Style.BackgroundBrush = new PdfSolidBrush(new PdfColor(255, 235, 238));
                row.Cells[2].Style.TextBrush = PdfBrushes.DarkRed;
            }
            row.Cells[1].Style.StringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center };
        }

        // Rita grid och fånga slutpositionen för efterföljande text
        PdfLayoutResult result = grid.Draw(page, new PointF(0, 400));

        // 5. DYNAMISK FOOTER
        graphics.DrawString("Authorized by Syncfusion PDF Engine", GetFont("Helvetica", 8, PdfFontStyle.Italic),
            PdfBrushes.Gray, new PointF(25, size.Height - 20));
    }
}