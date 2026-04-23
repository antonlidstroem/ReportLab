using System.Globalization;
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using ReportLab.Console.Core;

public class JsReportBauhausProvider : IReportProvider
{
    public string Name => "jsreport_Bauhaus";

    public void Export(ReportModel model, string filePath)
    {
        var rs = new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();
        string labelsJson = string.Join(",", model.Stats.Select(s => $"\"{s.Label}\""));
        string valuesJson = string.Join(",", model.Stats.Select(s => s.Value.ToString(CultureInfo.InvariantCulture)));

        string htmlTemplate = $@"
        <html>
            <head>
                <link href='https://fonts.googleapis.com/css2?family=Montserrat:wght@900&display=swap' rel='stylesheet'>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
                <style>
                    body {{ background: #fff; color: #000; font-family: 'Montserrat', sans-serif; padding: 0; margin: 0; }}
                    .sidebar {{ position: absolute; left: 0; top: 0; bottom: 0; width: 60px; background: #ff3e00; }}
                    .top-bar {{ height: 100px; background: #ffbe00; margin-left: 60px; padding: 20px; display: flex; align-items: center; }}
                    .main {{ margin-left: 100px; padding: 40px; border-left: 10px solid #0055ff; min-height: 100vh; }}
                    h1 {{ font-size: 60px; line-height: 0.8; margin: 0; text-transform: uppercase; }}
                    .chart-box {{ border: 5px solid #000; padding: 20px; margin-top: 40px; background: #fff; }}
                </style>
            </head>
            <body>
                <div class='sidebar'></div>
                <div class='top-bar'><h1>{model.Title.Split(' ')[0]}</h1></div>
                <div class='main'>
                    <p style='font-weight: bold;'>ANALYSIS // 2026</p>
                    <div class='chart-box'><canvas id='bauhausChart'></canvas></div>
                    <div style='display: flex; gap: 20px; margin-top: 20px;'>
                        <div style='background: #0055ff; color: white; padding: 20px; flex: 1;'>SCORE: {model.Stats.Average(x => x.Value):F1}</div>
                        <div style='background: #000; color: white; padding: 20px; flex: 1;'>ID: {model.Date.Replace("-", "")}</div>
                    </div>
                </div>
                <script>
                    window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};
                    new Chart(document.getElementById('bauhausChart'), {{
                        type: 'line',
                        data: {{
                            labels: [{labelsJson}],
                            datasets: [{{ label: 'DATA', data: [{valuesJson}], borderColor: '#000', borderWidth: 10, fill: false, tension: 0 }}]
                        }},
                        options: {{ animation: false, scales: {{ x: {{ grid: {{ display: false }} }}, y: {{ grid: {{ color: '#000' }} }} }} }}
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