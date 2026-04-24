using System.Diagnostics;
using System.Text.Json;
using ReportLab.Console.Core;
using ReportLab.Console.Providers;
using ReportLab.Console.Providers.jsreport;
using ReportLab.Console.Providers.QuestPDF;
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
    new JsReportBauhausProvider()  
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

    var input = Console.ReadKey(true).KeyChar.ToString().ToUpper();

    if (input == "Q") break;

    if (input == "A")
    {
        foreach (var p in providers) RunExport(p, myData!);
    }
    else if (int.TryParse(input, out int index) && index <= providers.Count)
    {
        RunExport(providers[index - 1], myData!);
    }

    Console.WriteLine("\nTryck på valfri tangent för att återvända till menyn...");
    Console.ReadKey();
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

