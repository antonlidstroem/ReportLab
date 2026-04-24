using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.jsreport;

public class JsReportDesignerProvider : JsReportBaseProvider
{
    public override string Name => "jsreport_DESIGNER";


    public override void Export(ReportModel model, string filePath)
    {
        string html = GetFullHtml(model);

        // Spara som en temporär HTML-fil
        string tempPath = Path.Combine(Path.GetTempPath(), "ReportDesigner.html");
        File.WriteAllText(tempPath, html);

        // Öppna filen i standardwebbläsaren
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = tempPath,
            UseShellExecute = true
        };
        System.Diagnostics.Process.Start(psi);
    }

    protected override string GetHtmlTemplate(ReportModel model, string labelsJson, string valuesJson)
    {
        // Vi skickar med hela modellen som ett JSON-objekt till JavaScript
        string fullModelJson = System.Text.Json.JsonSerializer.Serialize(model);

        return $@"
        <html>
            <head>
                <link href='https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600&display=swap' rel='stylesheet'>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
                <style>
                    body {{ display: flex; height: 100vh; margin: 0; font-family: 'Inter', sans-serif; background: #f0f2f5; }}
                    
                    /* DESIGNER SIDEBAR */
                    .sidebar {{ width: 350px; background: #fff; border-right: 1px solid #ddd; padding: 25px; overflow-y: auto; box-shadow: 2px 0 10px rgba(0,0,0,0.05); }}
                    .sidebar h2 {{ font-size: 18px; margin-bottom: 20px; color: #1a237e; border-bottom: 2px solid #f0f2f5; padding-bottom: 10px; }}
                    .control-group {{ margin-bottom: 25px; }}
                    .control-group label {{ display: block; font-weight: 600; font-size: 12px; text-transform: uppercase; margin-bottom: 10px; color: #666; }}
                    
                    /* LIVE PREVIEW AREA */
                    .preview-area {{ flex: 1; padding: 40px; overflow-y: auto; display: flex; justify-content: center; }}
                    .report-paper {{ background: white; width: 800px; min-height: 1100px; padding: 60px; box-shadow: 0 10px 30px rgba(0,0,0,0.1); transition: all 0.3s ease; }}
                    
                    /* INTERACTIVE ELEMENTS */
                    .stat-item {{ display: flex; align-items: center; gap: 10px; padding: 5px 0; font-size: 13px; cursor: pointer; }}
                    .stat-item input {{ cursor: pointer; }}
                    .btn-export {{ background: #1a237e; color: white; border: none; padding: 12px 20px; width: 100%; border-radius: 6px; cursor: pointer; font-weight: 600; margin-top: 20px; }}
                    .btn-export:hover {{ background: #536dfe; }}
                </style>
            </head>
            <body>
                <div class='sidebar'>
                    <h2>Rapportdesigner</h2>
                    
                    <div class='control-group'>
                        <label>Välj diagramtyp</label>
                        <select id='chartType' onchange='updateReport()' style='width: 100%; padding: 8px;'>
                            <option value='bar'>Stapeldiagram (Modern)</option>
                            <option value='line'>Linjediagram (Minimal)</option>
                            <option value='radar'>Radardiagram (Executive)</option>
                            <option value='polarArea'>Polär Area (Vintage)</option>
                        </select>
                    </div>

                    <div class='control-group'>
                        <label>Visa fält i rapporten</label>
                        <div id='fieldsList'></div>
                    </div>

                    <button class='btn-export' onclick='window.print()'>Exportera till PDF</button>
                    <p style='font-size: 10px; color: #999; margin-top: 20px;'>* Använd Ctrl+P för att spara dina ändringar som en slutgiltig rapport.</p>
                </div>

                <div class='preview-area'>
                    <div class='report-paper' id='reportContent'>
                        <h1 id='previewTitle' style='margin:0; color:#1a237e;'></h1>
                        <p id='previewDesc' style='color:#666; margin-bottom: 40px;'></p>
                        
                        <div style='height: 400px; margin-bottom: 40px;'>
                            <canvas id='liveChart'></canvas>
                        </div>

                        <table id='dataTable' style='width:100%; border-collapse:collapse;'>
                            <thead style='border-bottom: 2px solid #eee;'>
                                <tr><th style='text-align:left; padding:10px;'>Område</th><th style='text-align:right; padding:10px;'>Värde</th></tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>

                <script>
                    const model = {fullModelJson};
                    let currentChart = null;

                    // Initiera designer
                    function init() {{
                        document.getElementById('previewTitle').innerText = model.Title;
                        document.getElementById('previewDesc').innerText = model.Description;
                        
                        const list = document.getElementById('fieldsList');
                        model.Stats.forEach((s, i) => {{
                            const div = document.createElement('div');
                            div.className = 'stat-item';
                            div.innerHTML = `<input type='checkbox' checked id='check-${{i}}' onchange='updateReport()'> <label for='check-${{i}}'>${{s.Label}}</label>`;
                            list.appendChild(div);
                        }});
                        
                        updateReport();
                    }}

                    function updateReport() {{
                        const selectedStats = model.Stats.filter((s, i) => document.getElementById(`check-${{i}}`).checked);
                        const type = document.getElementById('chartType').value;
                        
                        // Uppdatera Tabell
                        const tbody = document.querySelector('#dataTable tbody');
                        tbody.innerHTML = selectedStats.map(s => `
                            <tr style='border-bottom: 1px solid #eee;'>
                                <td style='padding:10px;'>${{s.Label}}</td>
                                <td style='padding:10px; text-align:right; font-weight:bold;'>${{s.Value}}</td>
                            </tr>
                        `).join('');

                        // Uppdatera Diagram
                        if (currentChart) currentChart.destroy();
                        const ctx = document.getElementById('liveChart').getContext('2d');
                        currentChart = new Chart(ctx, {{
                            type: type,
                            data: {{
                                labels: selectedStats.map(s => s.Label),
                                datasets: [{{
                                    label: 'Mätvärden',
                                    data: selectedStats.map(s => s.Value),
                                    backgroundColor: 'rgba(83, 109, 254, 0.6)',
                                    borderColor: '#1a237e',
                                    borderWidth: 2
                                }}]
                            }},
                            options: {{ responsive: true, maintainAspectRatio: false }}
                        }});
                    }}

                    init();
                </script>
            </body>
        </html>";
    }
}