using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.jsreport;

public class JsReportMinimalProvider : JsReportBaseProvider
{
    public override string Name => "jsreport_Minimal";

    protected override string GetHtmlTemplate(ReportModel model, string labelsJson, string valuesJson)
    {
        return $@"
    <html>
        <head>
            <link href='https://fonts.googleapis.com/css2?family=Inter:wght@100;300;600&display=swap' rel='stylesheet'>
            <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
            <style>
                body {{ font-family: 'Inter', sans-serif; padding: 100px; color: #000; background: #fff; line-height: 1.6; }}
                .top-meta {{ display: flex; justify-content: space-between; font-size: 10px; text-transform: uppercase; letter-spacing: 3px; margin-bottom: 100px; }}
                h1 {{ font-weight: 100; font-size: 80px; letter-spacing: -4px; margin: 0 0 40px 0; }}
                .summary {{ font-size: 24px; font-weight: 300; max-width: 600px; margin-bottom: 80px; }}
                .chart-full {{ width: 100%; height: 400px; margin-bottom: 80px; }}
                .grid-stats {{ display: grid; grid-template-columns: repeat(3, 1fr); gap: 40px; border-top: 1px solid #eee; padding-top: 40px; }}
                .stat-item {{ font-size: 12px; }}
                .stat-value {{ font-size: 24px; font-weight: 600; display: block; }}
            </style>
        </head>
        <body>
            <div class='top-meta'>
                <span>{model.Date}</span>
                <span>Report / 001</span>
                <span>{model.Inspector}</span>
            </div>
            
            <h1>{model.Title}</h1>
            <div class='summary'>{model.Description}</div>
            
            <div class='chart-full'><canvas id='minimalChart'></canvas></div>

            <div class='grid-stats'>
                {string.Join("", model.Stats.Take(6).Select(s => $@"
                    <div class='stat-item'>
                        <span class='stat-value'>{s.Value:F1}</span>
                        {s.Label.ToUpper()}
                    </div>"))}
            </div>

            <script>
                window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};
                new Chart(document.getElementById('minimalChart'), {{
                    type: 'line',
                    data: {{ 
                        labels: [{labelsJson}], 
                        datasets: [{{ data: [{valuesJson}], borderColor: '#000', borderWidth: 1, pointRadius: 2, fill: true, backgroundColor: 'rgba(0,0,0,0.02)' }}] 
                    }},
                    options: {{ 
                        animation: false,
                        scales: {{ x: {{ display: false }}, y: {{ position: 'right', grid: {{ color: '#f5f5f5' }} }} }},
                        plugins: {{ legend: {{ display: false }} }}
                    }}
                }});
            </script>
        </body>
    </html>";
    }
}