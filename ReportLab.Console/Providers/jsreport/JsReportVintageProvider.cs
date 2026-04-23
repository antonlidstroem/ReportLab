using System.Globalization;
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using ReportLab.Console.Core;

public class JsReportVintageProvider : IReportProvider
{
    public string Name => "jsreport_Vintage";

    public void Export(ReportModel model, string filePath)
    {
        var rs = new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();
        string labelsJson = string.Join(",", model.Stats.Select(s => $"\"{s.Label}\""));
        string valuesJson = string.Join(",", model.Stats.Select(s => s.Value.ToString(CultureInfo.InvariantCulture)));

        string htmlTemplate = $@"
        <html>
            <head>
                <link href='https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,700;1,400&display=swap' rel='stylesheet'>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
                <style>
                    body {{ background: #f4ece1; color: #2c2c2c; font-family: 'Playfair Display', serif; padding: 50px; border: 2px solid #2c2c2c; margin: 10px; }}
                    h1 {{ font-size: 48px; text-align: center; border-bottom: 4px double #2c2c2c; margin-bottom: 10px; }}
                    .meta {{ text-align: center; font-style: italic; border-bottom: 1px solid #2c2c2c; padding-bottom: 10px; }}
                    .content {{ column-count: 2; column-gap: 40px; margin-top: 30px; }}
                </style>
            </head>
            <body>
                <h1>The Daily Insight</h1>
                <div class='meta'>Stockholm, {model.Date} — Dispatch by {model.Inspector}</div>
                <div class='content'>
                    <h2 style='margin-top: 0;'>{model.Title}</h2>
                    <p>{model.Description}</p>
                    <canvas id='vintageChart' style='background: white; padding: 10px;'></canvas>
                </div>
                <script>
                    window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};
                    new Chart(document.getElementById('vintageChart'), {{
                        type: 'bar',
                        data: {{
                            labels: [{labelsJson}],
                            datasets: [{{ label: 'Observed Data', data: [{valuesJson}], backgroundColor: '#2c2c2c' }}]
                        }},
                        options: {{ animation: false, indexAxis: 'y' }}
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