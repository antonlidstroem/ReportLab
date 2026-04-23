using jsreport.Local;
using jsreport.Binary;
using jsreport.Types;
using ReportLab.Console.Core;
using System.Globalization;

namespace ReportLab.Console.Providers.jsreport;

public class JsReportExecutiveProvider : IReportProvider
{
    public string Name => "jsreport_EXECUTIVE";

    public void Export(ReportModel model, string filePath)
    {
        var rs = new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();

        // Förbered data för JavaScript
        string labelsJson = string.Join(",", model.Stats.Select(s => $"\"{s.Label}\""));
        string valuesJson = string.Join(",", model.Stats.Select(s => s.Value.ToString(CultureInfo.InvariantCulture)));

        double average = model.Stats.Any() ? model.Stats.Average(x => x.Value) : 0;
        int criticalCount = model.Stats.Count(x => x.Value < 3);

        string htmlTemplate = $@"
        <html>
            <head>
                <link href='https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600;700&display=swap' rel='stylesheet'>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
                <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css'>
                
                <style>
                    :root {{
                        --primary: #1a237e;
                        --secondary: #536dfe;
                        --text-main: #2c3e50;
                        --text-light: #7f8c8d;
                        --danger: #d32f2f;
                        --success: #388e3c;
                        --bg: #f8f9fa;
                    }}

                    body {{ 
                        font-family: 'Inter', sans-serif; 
                        background-color: var(--bg); 
                        color: var(--text-main);
                        margin: 0; padding: 40px;
                    }}

                    .container {{ background: white; padding: 40px; border-radius: 4px; box-shadow: 0 0 20px rgba(0,0,0,0.05); }}

                    /* Header */
                    .header {{ display: flex; justify-content: space-between; border-bottom: 2px solid #eee; padding-bottom: 20px; margin-bottom: 30px; }}
                    .header h1 {{ margin: 0; font-size: 24px; color: var(--primary); text-transform: uppercase; letter-spacing: 1px; }}
                    
                    /* KPI Cards */
                    .kpi-row {{ display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; margin-bottom: 40px; }}
                    .kpi-card {{ background: white; border: 1px solid #e0e0e0; padding: 20px; border-radius: 8px; text-align: center; }}
                    .kpi-card .label {{ font-size: 11px; font-weight: 700; color: var(--text-light); text-transform: uppercase; margin-bottom: 5px; }}
                    .kpi-card .value {{ font-size: 24px; font-weight: 700; color: var(--primary); }}

                    /* Grid Layout för diagram */
                    .chart-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 30px; margin-bottom: 30px; }}
                    .chart-section {{ background: #fff; border: 1px solid #eee; padding: 20px; border-radius: 8px; }}
                    .chart-section h3 {{ font-size: 14px; margin-top: 0; color: var(--text-light); border-bottom: 1px solid #f5f5f5; padding-bottom: 10px; margin-bottom: 20px; }}

                    /* Tabell */
                    .analysis-table {{ width: 100%; border-collapse: collapse; margin-top: 20px; font-size: 13px; }}
                    .analysis-table th {{ background: #fcfcfc; text-align: left; padding: 12px; color: var(--text-light); font-weight: 600; border-bottom: 2px solid #eee; }}
                    .analysis-table td {{ padding: 12px; border-bottom: 1px solid #eee; }}
                    
                    .status-dot {{ display: inline-block; width: 8px; height: 8px; border-radius: 50%; margin-right: 8px; }}
                    .progress-bar {{ background: #eee; height: 6px; border-radius: 3px; width: 100px; }}
                    .progress-fill {{ height: 100%; border-radius: 3px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <div>
                            <h1>Operational Analysis Report</h1>
                            <div style='font-size: 12px; color: var(--text-light); margin-top: 5px;'>
                                TARGET: {model.Title.ToUpper()} | REFERENCE: ID-2026-X
                            </div>
                        </div>
                        <div style='text-align: right;'>
                            <div style='font-weight: bold;'>CONFIDENTIAL</div>
                            <div style='font-size: 11px;'>Generated: {model.Date}</div>
                        </div>
                    </div>

                    <div class='kpi-row'>
                        <div class='kpi-card'><div class='label'>Executive Score</div><div class='value'>{average:F1}</div></div>
                        <div class='kpi-card'><div class='label'>Critical Risks</div><div class='value' style='color: {(criticalCount > 0 ? "var(--danger)" : "var(--primary)")}'>{criticalCount}</div></div>
                        <div class='kpi-card'><div class='label'>Data Points</div><div class='value'>{model.Stats.Count}</div></div>
                        <div class='kpi-card'><div class='label'>Compliance</div><div class='value'>{(average / 5.0 * 100):F0}%</div></div>
                    </div>

                    <div class='chart-grid'>
                        <div class='chart-section'>
                            <h3>Performance Comparison (Radar View)</h3>
                            <canvas id='radarChart'></canvas>
                        </div>
                        <div class='chart-section'>
                            <h3>Risk Distribution (Doughnut)</h3>
                            <canvas id='doughnutChart'></canvas>
                        </div>
                        <div class='chart-section'>
                            <h3>Intensity Analysis (Polar)</h3>
                            <canvas id='polarChart'></canvas>
                        </div>
                        <div class='chart-section'>
                            <h3>Comparative Ranking (Bar)</h3>
                            <canvas id='barChart'></canvas>
                        </div>
                    </div>

                    <div class='chart-section' style='margin-top: 30px;'>
                        <h3>Detailed Strategic Breakdown</h3>
                        <table class='analysis-table'>
                            <thead>
                                <tr>
                                    <th>Indicator</th>
                                    <th>Trend</th>
                                    <th>Score</th>
                                    <th>Status</th>
                                    <th>Assessment</th>
                                </tr>
                            </thead>
                            <tbody>
                                {string.Join("", model.Stats.OrderByDescending(x => x.Value).Select(s => $@"
                                <tr>
                                    <td style='font-weight: 600;'>{s.Label}</td>
                                    <td>
                                        <div class='progress-bar'>
                                            <div class='progress-fill' style='width: {(s.Value / 5.0 * 100).ToString(CultureInfo.InvariantCulture)}%; background: {(s.Value < 3 ? "var(--danger)" : "var(--secondary)")}'></div>
                                        </div>
                                    </td>
                                    <td><strong>{s.Value:F1}</strong></td>
                                    <td>
                                        <span class='status-dot' style='background: {(s.Value < 3 ? "var(--danger)" : "var(--success)")}'></span>
                                        {(s.Value < 3 ? "Action Required" : "Compliant")}
                                    </td>
                                    <td style='color: var(--text-light)'>{(s.Value < 3 ? "High Priority" : "Monitor")}</td>
                                </tr>"))}
                            </tbody>
                        </table>
                    </div>
                </div>

                <script>
                    window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};
                    Chart.defaults.font.family = 'Inter';
                    Chart.defaults.font.size = 10;
                    Chart.defaults.color = '#7f8c8d';

                    const labels = [{labelsJson}];
                    const data = [{valuesJson}];

                    // RADAR
                    new Chart(document.getElementById('radarChart'), {{
                        type: 'radar',
                        data: {{
                            labels: labels,
                            datasets: [{{
                                label: 'Current Value',
                                data: data,
                                backgroundColor: 'rgba(83, 109, 254, 0.1)',
                                borderColor: '#536dfe',
                                borderWidth: 2,
                                pointRadius: 2
                            }}]
                        }},
                        options: {{ animation: false, scales: {{ r: {{ min: 0, max: 5, ticks: {{ display: false }} }} }} }}
                    }});

                    // DOUGHNUT
                    new Chart(document.getElementById('doughnutChart'), {{
                        type: 'doughnut',
                        data: {{
                            labels: ['Optimal (4-5)', 'Acceptable (3-4)', 'Critical (<3)'],
                            datasets: [{{
                                data: [
                                    data.filter(v => v >= 4).length,
                                    data.filter(v => v >= 3 && v < 4).length,
                                    data.filter(v => v < 3).length
                                ],
                                backgroundColor: ['#1a237e', '#536dfe', '#d32f2f']
                            }}]
                        }},
                        options: {{ animation: false, cutout: '70%' }}
                    }});

                    // POLAR
                    new Chart(document.getElementById('polarChart'), {{
                        type: 'polarArea',
                        data: {{
                            labels: labels.slice(0, 5),
                            datasets: [{{
                                data: data.slice(0, 5),
                                backgroundColor: ['#1a237e', '#283593', '#303f9f', '#3949ab', '#3f51b5']
                            }}]
                        }},
                        options: {{ animation: false, scales: {{ r: {{ ticks: {{ display: false }} }} }} }}
                    }});

                    // BAR (Horizontal)
                    new Chart(document.getElementById('barChart'), {{
                        type: 'bar',
                        data: {{
                            labels: labels,
                            datasets: [{{
                                label: 'Score',
                                data: data,
                                backgroundColor: data.map(v => v < 3 ? '#d32f2f' : '#1a237e'),
                                borderRadius: 4
                            }}]
                        }},
                        options: {{ indexAxis: 'y', animation: false, scales: {{ x: {{ min: 0, max: 5 }} }} }}
                    }});
                </script>
            </body>
        </html>";

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