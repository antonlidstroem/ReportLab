using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.jsreport;

public class JsReportVintageProvider : JsReportBaseProvider
{
    public override string Name => "jsreport_Vintage";

    protected override string GetHtmlTemplate(ReportModel model, string labelsJson, string valuesJson)
    {
        return $@"
    <html>
        <head>
            <link href='https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,700;1,400&family=Special+Elite&display=swap' rel='stylesheet'>
            <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
            <style>
                body {{ 
                    background-color: #e8e2d6;
                    background-image: radial-gradient(#d8d0c0 1px, transparent 0);
                    background-size: 20px 20px;
                    color: #3e3a36; font-family: 'Special Elite', serif; padding: 60px; 
                }}
                .paper-sheet {{ 
                    background: #fdfaf3; padding: 60px; 
                    box-shadow: 5px 5px 15px rgba(0,0,0,0.1); 
                    border: 1px solid #dcd3c1; position: relative;
                }}
                .paper-sheet::after {{ 
                    content: ''; position: absolute; top: 0; left: 0; right: 0; bottom: 0;
                    background: linear-gradient(135deg, transparent 95%, rgba(0,0,0,0.05) 100%);
                    pointer-events: none;
                }}
                .stamp {{ 
                    position: absolute; top: 50px; right: 50px; border: 4px double #8b0000; 
                    color: #8b0000; padding: 10px; transform: rotate(15deg); 
                    font-weight: bold; opacity: 0.7; font-family: sans-serif;
                }}
                h1 {{ font-family: 'Playfair Display'; font-size: 50px; border-bottom: 2px solid #3e3a36; }}
                .chart-area {{ margin: 40px 0; border: 1px solid #3e3a36; padding: 20px; background: white; }}
                table {{ width: 100%; border-collapse: collapse; margin-top: 30px; }}
                td, th {{ border: 1px solid #3e3a36; padding: 10px; text-align: left; }}
            </style>
        </head>
        <body>
            <div class='paper-sheet'>
                <div class='stamp'>OFFICIAL RECORD<br>{model.Date}</div>
                <p style='text-decoration: underline;'>Document No. 2026/A-99</p>
                <h1>Internal Dispatch: {model.Title}</h1>
                
                <p style='font-style: italic; margin: 20px 0;'>Attn: Executive Board<br>From: {model.Inspector}</p>
                
                <div style='column-count: 2; gap: 30px;'>
                    <p>{model.Description}</p>
                    <div class='chart-area'><canvas id='vintageChart'></canvas></div>
                </div>

                <table>
                    <thead><tr><th>CLASSIFICATION</th><th>RATING</th><th>OBSERVATIONS</th></tr></thead>
                    <tbody>
                        {string.Join("", model.Stats.Select(s => $@"
                            <tr>
                                <td>{s.Label.ToUpper()}</td>
                                <td>{s.Value:F1}</td>
                                <td>{(s.Value < 3 ? "Urgent attention required." : "Satisfactory condition.")}</td>
                            </tr>"))}
                    </tbody>
                </table>
            </div>
            <script>
                window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};
                new Chart(document.getElementById('vintageChart'), {{
                    type: 'bar',
                    data: {{ 
                        labels: [{labelsJson}].slice(0,6), 
                        datasets: [{{ data: [{valuesJson}].slice(0,6), backgroundColor: '#3e3a36' }}] 
                    }},
                    options: {{ animation: false, scales: {{ y: {{ beginAtZero: true, grid: {{ color: '#eee' }} }} }} }}
                }});
            </script>
        </body>
    </html>";
    }
}