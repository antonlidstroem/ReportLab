using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.jsreport;

public class JsReportDarkTechProvider : JsReportBaseProvider
{
    public override string Name => "jsreport_DarkTech";

    protected override string GetHtmlTemplate(ReportModel model, string labelsJson, string valuesJson)
    {
        var statusColor = model.Stats.Average(s => s.Value) < 3 ? "#ff0055" : "#00f2ff";
        return $@"
    <html>
        <head>
            <link href='https://fonts.googleapis.com/css2?family=Orbitron:wght@400;900&family=JetBrains+Mono&display=swap' rel='stylesheet'>
            <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
            <style>
                body {{ background: #020406; color: {statusColor}; font-family: 'JetBrains Mono'; padding: 40px; margin: 0; border: 2px solid {statusColor}; }}
                .scanline {{ width: 100%; height: 100px; background: linear-gradient(to bottom, transparent, {statusColor}22, transparent); position: absolute; top: 0; left: 0; pointer-events: none; }}
                
                .header {{ display: flex; justify-content: space-between; align-items: flex-end; border-bottom: 4px double {statusColor}; padding-bottom: 20px; }}
                h1 {{ font-family: 'Orbitron'; font-size: 40px; margin: 0; text-shadow: 0 0 15px {statusColor}; }}
                
                .main-layout {{ display: grid; grid-template-columns: 1fr 350px; gap: 40px; margin-top: 30px; }}
                .panel {{ background: rgba(0, 242, 255, 0.03); border: 1px solid {statusColor}66; padding: 20px; position: relative; }}
                .panel::before {{ content: 'DATA_STREAM'; position: absolute; top: -10px; left: 10px; background: #020406; padding: 0 5px; font-size: 10px; }}
                
                .kpi-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }}
                .kpi-card {{ border: 1px solid {statusColor}; padding: 15px; text-align: center; }}
                .kpi-value {{ font-family: 'Orbitron'; font-size: 32px; display: block; }}
            </style>
        </head>
        <body>
            <div class='scanline'></div>
            <div class='header'>
                <div>
                    <h1>{model.Title}</h1>
                    <div>STAUTS: {(model.Stats.Average(s => s.Value) < 3 ? "CRITICAL_FAILURE" : "SYSTEM_STABLE")}</div>
                </div>
                <div style='text-align: right'>
                    <div>AGENT: {model.Inspector.ToUpper()}</div>
                    <div>REF: {model.Date}</div>
                </div>
            </div>

            <div class='main-layout'>
                <div class='panel'>
                    <canvas id='radarChart' height='400'></canvas>
                </div>
                <div class='panel'>
                    <div class='kpi-grid'>
                        <div class='kpi-card'><span class='kpi-value'>{model.Stats.Average(s => s.Value):F1}</span>SCORE</div>
                        <div class='kpi-card'><span class='kpi-value'>{model.Stats.Count(x => x.Value < 3)}</span>RISKS</div>
                    </div>
                    <div style='margin-top: 20px; font-size: 11px; line-height: 1.5;'>
                        {string.Join("", model.Stats.Select(s => $"<div>> {s.Label.Replace(" ", "_")}: {s.Value} <span style='float:right'>{(s.Value < 3 ? "[!!]" : "[OK]")}</span></div>"))}
                    </div>
                </div>
            </div>

            <script>
                window.JSREPORT_CHROME_PDF_OPTIONS = {{ waitForJS: true }};
                new Chart(document.getElementById('radarChart'), {{
                    type: 'radar',
                    data: {{
                        labels: [{labelsJson}],
                        datasets: [{{ 
                            data: [{valuesJson}], 
                            borderColor: '{statusColor}', 
                            backgroundColor: '{statusColor}33',
                            borderWidth: 2, pointRadius: 4, pointBackgroundColor: '{statusColor}'
                        }}]
                    }},
                    options: {{ 
                        animation: false,
                        scales: {{ r: {{ grid: {{ color: '{statusColor}22' }}, angleLines: {{ color: '{statusColor}22' }}, pointLabels: {{ color: '{statusColor}', font: {{ family: 'JetBrains Mono' }} }}, ticks: {{ display: false }} }} }}
                    }}
                }});
            </script>
        </body>
    </html>";
    }
}