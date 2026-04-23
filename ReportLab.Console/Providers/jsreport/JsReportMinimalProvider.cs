using jsreport.Local;
using jsreport.Binary;
using jsreport.Types;
using ReportLab.Console.Core;
using System.Globalization;
using System.Linq;
using System.IO;

namespace ReportLab.Console.Providers.jsreport;

public class JsReportMinimalProvider : IReportProvider
{
    public string Name => "jsreport_Minimal";

    public void Export(ReportModel model, string filePath)
    {
        var rs = new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();
        string labelsJson = string.Join(",", model.Stats.Select(s => $"\"{s.Label}\""));
        string valuesJson = string.Join(",", model.Stats.Select(s => s.Value.ToString(CultureInfo.InvariantCulture)));

        string htmlTemplate = $@"
        <html>
            <head>
                <link href='https://fonts.googleapis.com/css2?family=Inter:wght@300;400&display=swap' rel='stylesheet'>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
                <style>
                    body {{ font-family: 'Inter', sans-serif; padding: 60px; color: #111; background: #fff; }}
                    h1 {{ font-weight: 300; font-size: 42px; letter-spacing: -1px; margin-bottom: 10px; }}
                    .divider {{ width: 50px; height: 2px; background: #000; margin-bottom: 50px; }}
                    .chart-container {{ width: 100%; height: 300px; margin-bottom: 100px; }}
                </style>
            </head>
            <body>
                <h1>{model.Title}</h1>
                <div class='divider'></div>
                <p style='max-width: 500px;'>{model.Description}</p>
                <div class='chart-container'><canvas id='minimalChart'></canvas></div>
                <script>
                    window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};
                    new Chart(document.getElementById('minimalChart'), {{
                        type: 'line',
                        data: {{ labels: [{labelsJson}], datasets: [{{ data: [{valuesJson}], borderColor: '#000', fill: false }}] }},
                        options: {{ animation: false }}
                    }});
                </script>
            </body>
        </html>";

        // FIX: Ändrat från {{ }} till { }
        var report = rs.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = htmlTemplate,
                Engine = Engine.None,
                Recipe = Recipe.ChromePdf
            }
        }).GetAwaiter().GetResult();

        string finalPath = filePath.EndsWith(".pdf") ? filePath : filePath + ".pdf";
        using var fs = File.Create(finalPath);
        report.Content.CopyTo(fs);
    }
}