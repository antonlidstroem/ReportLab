using Microsoft.Playwright;
using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.PlayWright;

public class PlaywrightProvider : IReportProvider
{
    public string Name => "Playwright_Chromium";

    public void Export(ReportModel model, string filePath)
    {
        // We use a simple HTML string, similar to your jsreport logic
        string html = $@"
        <html>
            <body style='font-family: Arial; padding: 50px;'>
                <h1 style='color: #2c3e50;'>{model.Title}</h1>
                <p><strong>Inspektör:</strong> {model.Inspector}</p>
                <hr/>
                <ul>
                    {string.Join("", model.Stats.Select(s => $"<li>{s.Label}: {s.Value}</li>"))}
                </ul>
            </body>
        </html>";

        using var playwright = Microsoft.Playwright.Playwright.CreateAsync().GetAwaiter().GetResult();
        var browser = playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }).GetAwaiter().GetResult();
        var page = browser.NewPageAsync().GetAwaiter().GetResult();

        page.SetContentAsync(html).GetAwaiter().GetResult();

        page.PdfAsync(new PagePdfOptions
        {
            Path = filePath,
            Format = "A4",
            PrintBackground = true
        }).GetAwaiter().GetResult();
    }
}