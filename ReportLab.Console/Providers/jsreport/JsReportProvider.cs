using jsreport.Local;
using jsreport.Binary;
using jsreport.Types;
using ReportLab.Console.Core;
using System.Globalization;

namespace ReportLab.Console.Providers.jsreport;

public class JsReportProvider : IReportProvider
{
    public string Name => "jsreport";

    public void Export(ReportModel model, string filePath)
    {
        // 1. Initiera motorn på det enklaste sättet
        // AsUtility() gör att jsreport startar, renderar och stänger ner direkt.
        // Det är säkrast för konsolappar och slipper "daemon"-problem.
        var rs = new LocalReporting()
            .UseBinary(JsReportBinary.GetBinary())
            .AsUtility()
            .Create();

        // 2. Förbered data för JavaScript (Viktigt: använd . för decimaler!)
        string labelsJson = string.Join(",", model.Stats.Select(s => $"\"{s.Label}\""));
        string valuesJson = string.Join(",", model.Stats.Select(s => s.Value.ToString(CultureInfo.InvariantCulture)));

        // 3. HTML-mallen
        string htmlTemplate = $@"
        <html>
            <head>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
                <style>
                    body {{ font-family: sans-serif; margin: 30px; }}
                    h1 {{ color: #1a5276; border-bottom: 2px solid #eee; }}
                    .chart-box {{ width: 600px; height: 400px; margin-top: 20px; }}
                </style>
            </head>
            <body>
                <h1>{model.Title}</h1>
                <p><strong>Inspektör:</strong> {model.Inspector} | <strong>Datum:</strong> {model.Date}</p>
                <p>{model.Description}</p>

                <div class='chart-box'>
                    <canvas id='myChart'></canvas>
                </div>

                <script>
                    // Vi talar om för jsreport att vänta på att Chart.js ritar klart
                    window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};

                    const ctx = document.getElementById('myChart');
                    new Chart(ctx, {{
                        type: 'bar',
                        data: {{
                            labels: [{labelsJson}],
                            datasets: [{{
                                label: 'Poäng',
                                data: [{valuesJson}],
                                backgroundColor: 'rgba(54, 162, 235, 0.7)'
                            }}]
                        }},
                        options: {{ 
                            indexAxis: 'y',
                            animation: false // Måste vara false för PDF
                        }}
                    }});
                </script>
            </body>
        </html>";

        // 4. Rendera
        // Vi använder en tom Chrome-konfiguration för att undvika kompileringsfel
        var report = rs.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = htmlTemplate,
                Engine = Engine.None,
                Recipe = Recipe.ChromePdf
            }
        }).GetAwaiter().GetResult();

        // 5. Spara (Vi lägger till .pdf om det saknas)
        string finalPath = filePath.EndsWith(".pdf") ? filePath : filePath + ".pdf";
        using var fs = File.Create(finalPath);
        report.Content.CopyTo(fs);
    }
}