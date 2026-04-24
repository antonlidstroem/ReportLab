using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.Syncfusion;

public abstract class SyncfusionPdfBaseProvider : IReportProvider
{
    public abstract string Name { get; }

    // Varje subklass implementerar denna för sin unika design
    protected abstract void DrawContent(PdfPage page, PdfGraphics graphics, ReportModel model);

    public void Export(ReportModel model, string filePath)
    {
        // 1. Skapa dokumentet
        using var document = new PdfDocument();

        // 2. Standardinställningar för sidan (A4)
        var page = document.Pages.Add();
        var graphics = page.Graphics;

        // 3. Anropa subklassens design
        DrawContent(page, graphics, model);

        // 4. Spara filen
        string finalPath = filePath.EndsWith(".pdf") ? filePath : filePath + ".pdf";
        using var fileStream = new FileStream(finalPath, FileMode.Create, FileAccess.Write);
        document.Save(fileStream);
    }

    // Hjälpmetod för att hämta standardtypsnitt som alla subklasser kan använda
    protected PdfFont GetFont(string family = "Helvetica", float size = 10, PdfFontStyle style = PdfFontStyle.Regular)
        => new PdfStandardFont(Enum.Parse<PdfFontFamily>(family), size, style);
}