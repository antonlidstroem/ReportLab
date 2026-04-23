using jsreport.Local;
using jsreport.Binary;
using jsreport.Types;
using ReportLab.Console.Core;
using System.Globalization;

namespace ReportLab.Console.Providers.jsreport;

public class JsReportProvider : IReportProvider
{
    public string Name => "jsreport_MAX";

    public void Export(ReportModel model, string filePath)
    {
        var rs = new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();

        // Förbered data för JS
        string labelsJson = string.Join(",", model.Stats.Select(s => $"\"{s.Label}\""));
        string valuesJson = string.Join(",", model.Stats.Select(s => s.Value.ToString(CultureInfo.InvariantCulture)));
        double average = model.Stats.Any() ? model.Stats.Average(x => x.Value) : 0;

        string htmlTemplate = $@"
        <html>
            <head>
                <link href='https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600;700&display=swap' rel='stylesheet'>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
                <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css'>
                
                <style>
                    :root {{
                        --bg: #0f172a;
                        --card-bg: rgba(30, 41, 59, 0.7);
                        --primary: #38bdf8;
                        --accent: #818cf8;
                        --success: #34d399;
                        --warning: #fbbf24;
                        --danger: #f87171;
                        --text: #f1f5f9;
                    }}

                    body {{ 
                        font-family: 'Poppins', sans-serif; 
                        background-color: var(--bg); 
                        color: var(--text);
                        margin: 0; padding: 40px;
                    }}

                    /* Bakgrundsdekoration för att visa CSS-kapacitet */
                    body::before {{
                        content: ''; position: fixed; top: 0; left: 0; width: 100%; height: 100%;
                        background: radial-gradient(circle at 0% 0%, #1e293b 0%, transparent 50%);
                        z-index: -1;
                    }}

                    .header {{ display: flex; justify-content: space-between; align-items: center; margin-bottom: 40px; }}
                    .header h1 {{ font-size: 2.5em; margin: 0; background: linear-gradient(to right, var(--primary), var(--accent)); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }}
                    
                    .kpi-grid {{ display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; margin-bottom: 30px; }}
                    .kpi-card {{ background: var(--card-bg); padding: 20px; border-radius: 15px; border: 1px solid rgba(255,255,255,0.1); text-align: center; }}
                    .kpi-card i {{ font-size: 1.5em; color: var(--primary); margin-bottom: 10px; }}
                    .kpi-value {{ font-size: 1.8em; font-weight: 700; }}

                    .main-grid {{ display: grid; grid-template-columns: 1.2fr 0.8fr; gap: 30px; }}
                    .chart-container {{ background: var(--card-bg); padding: 25px; border-radius: 20px; border: 1px solid rgba(255,255,255,0.1); }}
                    
                    .status-tag {{ padding: 40px; border-radius: 20px; text-align: center; font-weight: bold; font-size: 1.2em; }}
                    .low {{ background: rgba(248, 113, 113, 0.2); color: var(--danger); border: 1px solid var(--danger); }}
                    .high {{ background: rgba(52, 211, 153, 0.2); color: var(--success); border: 1px solid var(--success); }}

                    table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                    th {{ text-align: left; color: var(--primary); padding: 10px; border-bottom: 2px solid #334155; }}
                    td {{ padding: 10px; border-bottom: 1px solid #334155; font-size: 0.9em; }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <div>
                        <h1>REPORT INSIGHTS <span style='font-weight:300'>2026</span></h1>
                        <p style='color:#94a3b8'><i class='fa-solid fa-calendar'></i> {model.Date} | <i class='fa-solid fa-user-tie'></i> {model.Inspector}</p>
                    </div>
                    <div style='text-align:right'>
                        <i class='fa-solid fa-bolt' style='color:var(--primary); font-size: 2em;'></i>
                    </div>
                </div>

                <div class='kpi-grid'>
                    <div class='kpi-card'><i class='fa-solid fa-chart-line'></i><div style='font-size:0.8em'>GENOMSNITT</div><div class='kpi-value'>{average:F2}</div></div>
                    <div class='kpi-card'><i class='fa-solid fa-list-check'></i><div style='font-size:0.8em'>PARAMETRAR</div><div class='kpi-value'>{model.Stats.Count}</div></div>
                    <div class='kpi-card'><i class='fa-solid fa-triangle-exclamation'></i><div style='font-size:0.8em'>KRITISKA</div><div class='kpi-value' style='color:var(--danger)'>{model.Stats.Count(x => x.Value < 3)}</div></div>
                    <div class='kpi-card'><i class='fa-solid fa-award'></i><div style='font-size:0.8em'>TOPPSCORE</div><div class='kpi-value' style='color:var(--success)'>{model.Stats.Max(x => x.Value)}</div></div>
                </div>

                <div class='main-grid'>
                    <div class='chart-container'>
                        <h3 style='margin-top:0'><i class='fa-solid fa-microchip'></i> Data Analytics Matrix</h3>
                        <canvas id='radarChart'></canvas>
                    </div>
                    
                    <div style='display: flex; flex-direction: column; gap: 30px;'>
                        <div class='status-tag {(average < 3.5 ? "low" : "high")}'>
                            <i class='fa-solid {(average < 3.5 ? "fa-circle-xmark" : "fa-circle-check")}'></i>
                            {(average < 3.5 ? "STRATEGISK ÅTGÄRD KRÄVS" : "OPERATIV STATUS: OPTIMAL")}
                        </div>
                        
                        <div class='chart-container'>
                            <h3>Distribution</h3>
                            <canvas id='doughnutChart'></canvas>
                        </div>
                    </div>
                </div>

                <div class='chart-container' style='margin-top:30px'>
                    <h3><i class='fa-solid fa-ranking-star'></i> Detaljerad Ranking</h3>
                    <canvas id='barChart' style='height:300px'></canvas>
                </div>

                <script>
                    window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};
                    Chart.defaults.color = '#94a3b8';
                    Chart.defaults.font.family = 'Poppins';

                    const labels = [{labelsJson}];
                    const data = [{valuesJson}];

                    // Radar Chart
                    new Chart(document.getElementById('radarChart'), {{
                        type: 'radar',
                        data: {{
                            labels: labels,
                            datasets: [{{
                                label: 'Metrics',
                                data: data,
                                backgroundColor: 'rgba(56, 189, 248, 0.2)',
                                borderColor: '#38bdf8',
                                borderWidth: 3,
                                pointBackgroundColor: '#818cf8'
                            }}]
                        }},
                        options: {{ animation: false, scales: {{ r: {{ grid: {{ color: '#334155' }}, angleLines: {{ color: '#334155' }}, min: 0, max: 5 }} }} }}
                    }});

                    // Doughnut Chart
                    new Chart(document.getElementById('doughnutChart'), {{
                        type: 'doughnut',
                        data: {{
                            labels: ['High', 'Critical'],
                            datasets: [{{
                                data: [data.filter(v => v >= 3).length, data.filter(v => v < 3).length],
                                backgroundColor: ['#34d399', '#f87171'],
                                borderWidth: 0
                            }}]
                        }},
                        options: {{ animation: false, cutout: '70%' }}
                    }});

                    // Horizontal Bar Chart
                    new Chart(document.getElementById('barChart'), {{
                        type: 'bar',
                        data: {{
                            labels: labels,
                            datasets: [{{
                                label: 'Poäng',
                                data: data,
                                backgroundColor: data.map(v => v < 3 ? '#f87171' : '#38bdf8'),
                                borderRadius: 5
                            }}]
                        }},
                        options: {{ indexAxis: 'y', animation: false, scales: {{ x: {{ min:0, max:5, grid: {{ color: '#334155' }} }} }} }}
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