using System.Globalization;
using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.jsreport;

public class JsReportExecutiveProvider : JsReportBaseProvider
{
    public override string Name => "jsreport_EXECUTIVE";

    protected override string GetHtmlTemplate(ReportModel model, string labelsJson, string valuesJson)
    {
        double average = model.Stats.Any() ? model.Stats.Average(x => x.Value) : 0;
        int criticalCount = model.Stats.Count(x => x.Value < 3);

        return $@"
        <html>
            <head>
                <link href='https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600;700&display=swap' rel='stylesheet'>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
                <style>
                    :root {{
                        --primary: #1a237e; --secondary: #536dfe; --text-main: #2c3e50;
                        --text-light: #7f8c8d; --danger: #d32f2f; --success: #388e3c; --bg: #f8f9fa;
                    }}
                    body {{ font-family: 'Inter', sans-serif; background-color: var(--bg); color: var(--text-main); margin: 0; padding: 40px; }}
                    .container {{ background: white; padding: 40px; border-radius: 4px; box-shadow: 0 0 20px rgba(0,0,0,0.05); }}
                    .header {{ display: flex; justify-content: space-between; border-bottom: 2px solid #eee; padding-bottom: 20px; margin-bottom: 30px; }}
                    .kpi-row {{ display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; margin-bottom: 40px; }}
                    .kpi-card {{ background: white; border: 1px solid #e0e0e0; padding: 20px; border-radius: 8px; text-align: center; }}
                    .chart-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 30px; }}
                    .chart-section {{ background: #fff; border: 1px solid #eee; padding: 20px; border-radius: 8px; }}
                    .analysis-table {{ width: 100%; border-collapse: collapse; margin-top: 20px; font-size: 13px; }}
                    .analysis-table th {{ background: #fcfcfc; text-align: left; padding: 12px; border-bottom: 2px solid #eee; }}
                    .analysis-table td {{ padding: 12px; border-bottom: 1px solid #eee; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <div>
                            <h1 style='color: var(--primary);'>Operational Analysis</h1>
                            <div style='font-size: 12px;'>TARGET: {model.Title.ToUpper()}</div>
                        </div>
                    </div>

                    <div class='kpi-row'>
                        <div class='kpi-card'><div>Score</div><div style='font-size:24px; font-weight:700;'>{average:F1}</div></div>
                        <div class='kpi-card'><div>Risks</div><div style='color:var(--danger); font-size:24px;'>{criticalCount}</div></div>
                        <div class='kpi-card'><div>Data</div><div style='font-size:24px;'>{model.Stats.Count}</div></div>
                        <div class='kpi-card'><div>Compliance</div><div style='font-size:24px;'>{(average / 5.0 * 100):F0}%</div></div>
                    </div>

                    <div class='chart-grid'>
                        <div class='chart-section'><canvas id='radarChart'></canvas></div>
                        <div class='chart-section'><canvas id='doughnutChart'></canvas></div>
                    </div>

                    <table class='analysis-table'>
                        <thead><tr><th>Indicator</th><th>Score</th><th>Status</th></tr></thead>
                        <tbody>
                            {string.Join("", model.Stats.OrderByDescending(x => x.Value).Select(s => $@"
                            <tr>
                                <td>{s.Label}</td>
                                <td><strong>{s.Value:F1}</strong></td>
                                <td style='color: {(s.Value < 3 ? "var(--danger)" : "var(--success)")}'>
                                    {(s.Value < 3 ? "Action Required" : "Compliant")}
                                </td>
                            </tr>"))}
                        </tbody>
                    </table>
                </div>

                <script>
                    window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};
                    const labels = [{labelsJson}];
                    const data = [{valuesJson}];

                    new Chart(document.getElementById('radarChart'), {{
                        type: 'radar',
                        data: {{
                            labels: labels,
                            datasets: [{{ label: 'Current', data: data, borderColor: '#536dfe', backgroundColor: 'rgba(83, 109, 254, 0.1)' }}]
                        }},
                        options: {{ animation: false }}
                    }});

                    new Chart(document.getElementById('doughnutChart'), {{
                        type: 'doughnut',
                        data: {{
                            labels: ['Optimal', 'Critical'],
                            datasets: [{{ data: [data.filter(v => v >= 3).length, data.filter(v => v < 3).length], backgroundColor: ['#1a237e', '#d32f2f'] }}]
                        }},
                        options: {{ animation: false }}
                    }});
                </script>
            </body>
        </html>";
    }
}