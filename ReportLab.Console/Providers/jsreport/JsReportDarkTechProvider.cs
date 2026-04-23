using jsreport.Local;
using jsreport.Binary;
using jsreport.Types;
using ReportLab.Console.Core;
using System.Globalization;

namespace ReportLab.Console.Providers.jsreport;

public class JsReportDarkTechProvider : IReportProvider
{
    public string Name => "jsreport_DarkTech";

    public void Export(ReportModel model, string filePath)
    {
        var rs = new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();
        string labelsJson = string.Join(",", model.Stats.Select(s => $"\"{s.Label}\""));
        string valuesJson = string.Join(",", model.Stats.Select(s => s.Value.ToString(CultureInfo.InvariantCulture)));

        string htmlTemplate = $@"
        <html>
            <head>
                <link href='https://fonts.googleapis.com/css2?family=Orbitron:wght@400;700&display=swap' rel='stylesheet'>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
                <style>
                    body {{ background: #0b0e14; color: #00d2ff; font-family: 'Orbitron', sans-serif; padding: 40px; }}
                    .card {{ background: rgba(0, 210, 255, 0.05); border: 1px solid #00d2ff; padding: 20px; border-radius: 15px; box-shadow: 0 0 15px rgba(0, 210, 255, 0.2); }}
                    h1 {{ text-transform: uppercase; letter-spacing: 5px; text-shadow: 0 0 10px #00d2ff; }}
                    .grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }}
                </style>
            </head>
            <body>
                <div class='card'>
                    <h1>System Analysis</h1>
                    <p>> {model.Title.ToUpper()} // STATUS: ACTIVE</p>
                </div>
                <div class='grid' style='margin-top:20px'>
                    <div class='card'><canvas id='radarChart'></canvas></div>
                    <div class='card' style='font-size: 12px;'>
                        <p style='color: #fff'>// ANALYTICAL LOG:</p>
                        {string.Join("<br>", model.Stats.Select(s => $"> {s.Label}: {s.Value}"))}
                    </div>
                </div>
                <script>
                    window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};
                    new Chart(document.getElementById('radarChart'), {{
                        type: 'radar',
                        data: {{
                            labels: [{labelsJson}],
                            datasets: [{{ label: 'Metrics', data: [{valuesJson}], borderColor: '#00d2ff', backgroundColor: 'rgba(0, 210, 255, 0.2)' }}]
                        }},
                        options: {{ animation: false, scales: {{ r: {{ grid: {{ color: '#1a2a3a' }}, angleLines: {{ color: '#1a2a3a' }}, ticks: {{ display: false }} }} }} }}
                    }});
                </script>
            </body>
        </html>";

        // Längst ner i Export-metoden, ersätt sparningen med detta:
        var report = rs.RenderAsync(new RenderRequest
        {
            Template = new Template { Content = htmlTemplate, Engine = Engine.None, Recipe = Recipe.ChromePdf }
        }).GetAwaiter().GetResult();

        string finalPath = filePath.EndsWith(".pdf") ? filePath : filePath + ".pdf";
        using (var fs = File.Create(finalPath))
        {
            report.Content.CopyTo(fs);
        }
    }
}