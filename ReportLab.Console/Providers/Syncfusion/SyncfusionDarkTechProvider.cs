using ReportLab.Console.Core;
using ReportLab.Console.Providers.Syncfusion;
using Syncfusion.Drawing; // <-- DENNA RAD LÖSER ALLA DINA POINTF/RECTANGLEF-FEL
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace ReportLab.Console.Providers.jsreport; // Se till att detta matchar din mappstruktur

public class SyncfusionDarkTechProvider : SyncfusionPdfBaseProvider
{
    public override string Name => "Syncfusion_DarkTech_MAX";

    protected override void DrawContent(PdfPage page, PdfGraphics graphics, ReportModel model)
    {
        var black = new PdfColor(5, 5, 10);
        var cyan = new PdfColor(0, 255, 255);
        var size = page.GetClientSize();

        // 1. BACKGROUND GRID
        graphics.DrawRectangle(new PdfSolidBrush(black), new RectangleF(0, 0, size.Width, size.Height));

        var gridPen = new PdfPen(new PdfColor(0, 255, 255, 30), 0.5f);
        for (int i = 0; i < size.Width; i += 30) graphics.DrawLine(gridPen, i, 0, i, size.Height);
        for (int i = 0; i < size.Height; i += 30) graphics.DrawLine(gridPen, 0, i, size.Width, i);

        // 2. HUD INTERFACE
        graphics.DrawRectangle(new PdfPen(cyan, 2f), new RectangleF(10, 10, size.Width - 20, size.Height - 20));

        // TITEL
        graphics.DrawString($">> SYSTEM_AUDIT: {model.Title}", GetFont("Courier", 18, PdfFontStyle.Bold),
            new PdfSolidBrush(cyan), new PointF(30, 30));

        // 3. CIRCULAR GAUGES
        float avg = (float)model.Stats.Average(s => s.Value);
        graphics.DrawEllipse(new PdfPen(cyan, 1f), new RectangleF(size.Width - 120, 30, 80, 80));

        var centerFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle };
        graphics.DrawString($"{avg:F1}", GetFont("Courier", 20, PdfFontStyle.Bold),
            new PdfSolidBrush(cyan), new RectangleF(size.Width - 120, 30, 80, 80), centerFormat);

        graphics.DrawString("GLOBAL_AVG", GetFont("Courier", 8), new PdfSolidBrush(cyan), new PointF(size.Width - 110, 115));

        // 4. TERMINAL LOG
        float yPos = 140;
        var logFont = GetFont("Courier", 9);
        var brush = new PdfSolidBrush(cyan);

        foreach (var stat in model.Stats)
        {
            float barWidth = (float)(stat.Value / 5.0 * 100);
            graphics.DrawRectangle(new PdfPen(cyan, 0.5f), new RectangleF(30, yPos + 2, 100, 8));
            graphics.DrawRectangle(new PdfSolidBrush(new PdfColor(0, 255, 255, 100)), new RectangleF(30, yPos + 2, barWidth, 8));

            graphics.DrawString($"{stat.Label.ToUpper()}", logFont, brush, new PointF(140, yPos));
            graphics.DrawString($"{stat.Value:F1}", logFont, brush, new PointF(size.Width - 80, yPos));

            yPos += 20;
            if (yPos > size.Height - 100) break;
        }

        // 5. FOOTER
        graphics.DrawLine(new PdfPen(cyan, 1f), 30, size.Height - 50, size.Width - 30, size.Height - 50);
        graphics.DrawString($">>> AUTH_BY_SYNCFUSION_CORE_ENGINE", logFont, brush, new PointF(30, size.Height - 45));
    }
}