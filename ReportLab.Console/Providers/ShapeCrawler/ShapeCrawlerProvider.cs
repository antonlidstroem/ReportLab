using ShapeCrawler;
using ReportLab.Console.Core;
using ReportLab.Console.Providers.ScottPlot;
using System.IO;
using System.Linq;

namespace ReportLab.Console.Providers;

public class ShapeCrawlerProvider : IReportProvider
{
    public string Name => "PowerPoint";

    public void Export(ReportModel model, string filePath)
    {
        //// 1. Skapa en ny presentation
        //using var pres = new global::ShapeCrawler.Presentation();

        //// FIX 1: Prova 'Layouts' istället för 'SlideLayouts'
        //// Vi hämtar första layouten från första mastern
        //var master = pres.SlideMasters[0];
        //var layout = master.Layouts[0];

        //// 2. Lägg till en slide baserat på layouten
        //pres.Slides.Add(layout);
        //var slide = pres.Slides[0];

        //// 3. Hantera text (Samma logik som tidigare)
        //var titleBox = slide.Shapes.OfType<ITextBox>().FirstOrDefault();
        //if (titleBox != null && titleBox.Paragraphs.Count > 0)
        //{
        //    titleBox.Paragraphs[0].Portions[0].Text = model.Title;
        //}

        //// 4. Lägg till grafen
        //var chartImage = ChartGenerator.CreateBarChart(model.Stats);
        //using (var ms = new MemoryStream(chartImage))
        //{
        //    slide.Shapes.AddPicture(ms);
        //}

        //// 5. SPARNING
        //// Prova SaveAs först. Om det inte kompilerar, prova BinaryData + File.WriteAllBytes.
        //pres.Save(filePath);
    }
}