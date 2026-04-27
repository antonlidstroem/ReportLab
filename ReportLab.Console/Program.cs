using System.Diagnostics;
using System.Text.Json;
using ReportLab.Console.Core;
using ReportLab.Console.Providers;
using ReportLab.Console.Providers.FastReport;
using ReportLab.Console.Providers.jsreport;
using ReportLab.Console.Providers.PlayWright;
using ReportLab.Console.Providers.QuestPDF;
using ReportLab.Console.Providers.Stimulsoft;
using ReportLab.Console.Providers.Syncfusion;
using ReportLab.Console.Providers.Text;

// 1. Ladda data
string jsonString = File.ReadAllText("mockdata.json");
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var myData = JsonSerializer.Deserialize<ReportModel>(jsonString, options);



// 2. Registrera tillgängliga providers
var providers = new List<IReportProvider>
{
    new JsReportDesignerProvider(),
    new SyncfusionCorporateProvider(),
    new SyncfusionDarkTechProvider(),
    new SyncfusionExcelCorporateProvider(),
    new SyncfusionPptExecutiveProvider(),  
    new QuestPdfProvider(),
    new TextReportProvider(),
    new ClosedXMLProvider(),
    new JsReportExecutiveProvider(),
    new JsReportMinimalProvider(),
    new JsReportDarkTechProvider(),
    new JsReportVintageProvider(), 
    new JsReportBauhausProvider(),
    new PlaywrightProvider(),
    new FastReportProvider(),
    new StimulsoftProvider(),
    new SyncfusionWebDesignerProvider()

};

while (true)
{
    Console.Clear();
    Console.WriteLine("--- REPORT GENERATOR 2026 ---");
    Console.WriteLine($"Laddad rapport: {myData?.Title}");
    Console.WriteLine("-----------------------------\n");
    Console.WriteLine("Välj export-provider:");

    for (int i = 0; i < providers.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {providers[i].Name}");
    }
    Console.WriteLine("A. Kör alla");
    Console.WriteLine("Q. Avsluta");

    Console.Write("\nVal: ");
    string input = Console.ReadLine()?.Trim().ToUpper() ?? "";

    if (input == "Q") break;

    if (input == "A")
    {
        foreach (var p in providers) RunExport(p, myData!);

        // Valfritt: Vänta 2 sekunder så man hinner se att alla blev klara
        Thread.Sleep(2000);
    }
    else if (int.TryParse(input, out int choice) && choice >= 1 && choice <= providers.Count)
    {
        RunExport(providers[choice - 1], myData!);

        // Valfritt: Vänta 1 sekund så man hinner se bekräftelsen
        Thread.Sleep(1000);
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Ogiltigt val.");
        Console.ResetColor();
        Thread.Sleep(1000);
    }

    // De gamla raderna för Console.ReadKey() är nu borttagna
    // Loopen går nu direkt upp till Console.Clear()
}

// Metod för att köra export och mäta tid
void RunExport(IReportProvider provider, ReportModel model)
{
    var sw = Stopwatch.StartNew();

    string projectRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..");

    if (provider is JsReportDesignerProvider)
    {
        Console.WriteLine($"Startar {provider.Name}...");
        provider.Export(model, ""); // FilePath behövs inte för designern
        return;
    }

    // --- LOGIK FÖR GRUPPERING ---
    string folderName = provider.Name switch
    {
        var n when n.StartsWith("jsreport") => "jsreport",
        var n when n.StartsWith("Syncfusion") => "Syncfusion",
        _ => provider.Name
    };
    string exportFolder = Path.Combine(projectRoot, "Exports", folderName);
    // -------------------------------

    if (!Directory.Exists(exportFolder))
    {
        Directory.CreateDirectory(exportFolder);
    }

    // Tvätta filnamnet
    string safeTitle = model.Title;
    foreach (char c in Path.GetInvalidFileNameChars())
    {
        safeTitle = safeTitle.Replace(c, '_');
    }

    // Bygg filnamnet - vi behåller provider.Name i filnamnet så vi ser skillnad på stilarna
    string fileName = $"Export_{safeTitle}_{provider.Name}";

    string extension = provider.Name switch
    {
        "PlainText" => ".txt",
        "Excel" => ".xlsx",
        var n when n.Contains("Excel") => ".xlsx",
        var n when n.Contains("PPT") => ".pptx",
        _ => ".pdf"
    };

    string fullPath = Path.Combine(exportFolder, fileName + extension);

    Console.Write($"Exporterar {provider.Name} till \\Exports\\{folderName}... ");

    try
    {
        provider.Export(model, fullPath);
        sw.Stop();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"KLAR ({sw.ElapsedMilliseconds} ms)");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nFEL: {ex.Message}");
        Console.ResetColor();
    }
}

