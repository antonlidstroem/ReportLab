using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportLab.Console.Providers.ScottPlot;
using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.QuestPDF;

public class QuestPdfProvider : IReportProvider
{
    public string Name => "QuestPDF";

    public void Export(ReportModel model, string filePath)
    {
        global::QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                // 1. HEADER
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(model.Title).FontSize(22).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"Datum: {model.Date} | Inspektör: {model.Inspector}").FontSize(9).Italic();
                    });
                });

                // 2. CONTENT
                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Spacing(15);

                    // Beskrivning
                    col.Item().Text("Sammanfattning av observationer").FontSize(14).SemiBold();
                    col.Item().Text(model.Description);

                    // DIAGRAMMET (Vi hämtar bilden från ScottPlot)
                    col.Item().PaddingVertical(10).AlignCenter().Image(ChartGenerator.CreateBarChart(model.Stats));

                    // TABELLEN
                    col.Item().PaddingTop(10).Text("Detaljerad mätdata").FontSize(14).SemiBold();

                    col.Item().Table(table =>
                    {
                        // Definiera kolumner: En bred för text, en smal för värde, en för status
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(3); // Kategori
                            c.RelativeColumn(1); // Värde
                            c.RelativeColumn(1); // Status
                        });

                        // Tabellhuvud (Header)
                        table.Header(h =>
                        {
                            h.Cell().Element(CellStyle).Text("Undersökt område").SemiBold();
                            h.Cell().Element(CellStyle).AlignCenter().Text("Poäng (1-5)").SemiBold();
                            h.Cell().Element(CellStyle).AlignCenter().Text("Status").SemiBold();

                            // Lokal hjälp-metod för styling av header-celler
                            static IContainer CellStyle(IContainer container) =>
                                container.DefaultTextStyle(x => x.FontSize(11))
                                         .PaddingVertical(5)
                                         .BorderBottom(1)
                                         .BorderColor(Colors.Black);
                        });

                        // Tabellrader (Data)
                        foreach (var stat in model.Stats)
                        {
                            table.Cell().Element(RowStyle).Text(stat.Label);
                            table.Cell().Element(RowStyle).AlignCenter().Text(stat.Value.ToString("0.0"));

                            // Dynamisk status-text baserat på värde
                            var statusText = stat.Value switch
                            {
                                < 3.0 => "ÅTGÄRD",
                                < 4.0 => "OK",
                                _ => "BRA"
                            };

                            var statusColor = stat.Value < 3.0 ? Colors.Red.Medium : Colors.Black;

                            table.Cell().Element(RowStyle).AlignCenter()
                                 .Text(statusText).FontColor(statusColor).SemiBold();

                            // Lokal hjälp-metod för styling av vanliga rader
                            static IContainer RowStyle(IContainer container) =>
                                container.BorderBottom(1)
                                         .BorderColor(Colors.Grey.Lighten3)
                                         .PaddingVertical(5);
                        }
                    });
                });

                // 3. FOOTER
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Sida ");
                    x.CurrentPageNumber();
                    x.Span(" av ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf(filePath);
    }
}