using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.jsreport;

public class JsReportBauhausProvider : JsReportBaseProvider
{
    public override string Name => "jsreport_Bauhaus";

    protected override string GetHtmlTemplate(ReportModel model, string labelsJson, string valuesJson)
    {
        var avg = model.Stats.Average(s => s.Value);
        return $@"
    <html>
        <head>
            <link href='https://fonts.googleapis.com/css2?family=Montserrat:wght@900&display=swap' rel='stylesheet'>
            <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
            <style>
                :root {{ --red: #e63946; --blue: #1d3557; --yellow: #ffb703; }}
                body {{ background: #f1faee; font-family: 'Montserrat'; margin: 0; overflow: hidden; color: var(--blue); }}
                
                /* Bakgrunds-numret som reagerar på score */
                .bg-number {{ position: absolute; font-size: 800px; line-height: 0.8; opacity: 0.1; z-index: -1; left: -100px; bottom: -100px; }}
                
                .header-block {{ background: var(--red); color: white; padding: 60px; clip-path: polygon(0 0, 100% 0, 85% 100%, 0% 100%); }}
                h1 {{ font-size: 110px; margin: 0; line-height: 0.8; text-transform: uppercase; letter-spacing: -8px; }}
                
                .content-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 0; height: 100%; }}
                .left-panel {{ border-right: 20px solid var(--blue); padding: 40px; }}
                .right-panel {{ background: var(--yellow); padding: 40px; border-bottom: 20px solid var(--red); }}
                
                .chart-container {{ background: white; border: 10px solid var(--blue); padding: 20px; box-shadow: 20px 20px 0 var(--red); }}
                .stat-row {{ display: flex; justify-content: space-between; border-bottom: 5px solid var(--blue); font-size: 24px; padding: 10px 0; }}
            </style>
        </head>
        <body>
            <div class='bg-number'>{avg:F0}</div>
            <div class='header-block'><h1>{model.Title.Split(' ')[0]}</h1></div>
            
            <div class='content-grid'>
                <div class='left-panel'>
                    <p style='font-size: 30px; font-weight: 900; text-transform: uppercase;'>Analysis // 2026</p>
                    <div class='chart-container'><canvas id='mainChart'></canvas></div>
                    <div style='margin-top: 50px;'>
                         <p>{model.Description}</p>
                    </div>
                </div>
                <div class='right-panel'>
                    <h2 style='font-size: 50px; text-transform: uppercase; margin: 0;'>Metrics</h2>
                    {string.Join("", model.Stats.Take(10).Select(s => $@"
                        <div class='stat-row'>
                            <span>{s.Label}</span>
                            <span>{s.Value:F1}</span>
                        </div>"))}
                </div>
            </div>

            <script>
                window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};
                new Chart(document.getElementById('mainChart'), {{
                    type: 'line',
                    data: {{ 
                        labels: [{labelsJson}], 
                        datasets: [{{ data: [{valuesJson}], borderColor: '#1d3557', borderWidth: 15, pointRadius: 0, fill: false, tension: 0 }}] 
                    }},
                    options: {{ animation: false, scales: {{ x:{{display:false}}, y:{{display:false}} }}, plugins:{{legend:{{display:false}}}} }}
                }});
            </script>
        </body>
    </html>";
    }

}