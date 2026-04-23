using System.Diagnostics;
using System.Text.Json;
using ReportLab.Console.Core;
using ReportLab.Console.Providers.Text;
using ReportLab.Console.Providers.QuestPDF;
using ReportLab.Console.Providers;

// 1. Ladda data
string jsonString = File.ReadAllText("mockdata.json");
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var myData = JsonSerializer.Deserialize<ReportModel>(jsonString, options);

// 2. Registrera tillgängliga providers
var providers = new List<IReportProvider>
{
    new QuestPdfProvider(),
    new TextReportProvider(),
    new ClosedXMLProvider(),
    new ShapeCrawlerProvider()
    // Här fyller du på med Excel/jsreport senare
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

    // 1. Skapa en dedikerad Export-mapp i projektet
    string projectRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..");
    string exportFolder = Path.Combine(projectRoot, "Exports");

    if (!Directory.Exists(exportFolder))
    {
        Directory.CreateDirectory(exportFolder);
    }

    // 2. Tvätta filnamnet (Ta bort : / \ * ? " < > | )
    string safeTitle = model.Title;
    foreach (char c in Path.GetInvalidFileNameChars())
    {
        safeTitle = safeTitle.Replace(c, '_');
    }

    // 3. Bygg det slutgiltiga filnamnet
    // 3. Bygg det slutgiltiga filnamnet
    string fileName = $"Export_{safeTitle}_{provider.Name}";

    // VIKTIGT: Matcha mot provider.Name (Excel/PowerPoint), inte klassnamnet
    string extension = provider.Name switch
    {
        "PlainText" => ".txt",
        "Excel" => ".xlsx",     // Matchar ClosedXMLProvider.Name
        "PowerPoint" => ".pptx", // Matchar ShapeCrawlerProvider.Name
        _ => ".pdf"
    };
    string fullPath = Path.Combine(exportFolder, fileName + extension);

    Console.Write($"Exporterar med {provider.Name}... ");

    try
    {
        // Vi skickar nu den fullständiga sökvägen inkl. ändelse
        provider.Export(model, fullPath);
        sw.Stop();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"KLAR ({sw.ElapsedMilliseconds} ms)");
        Console.ResetColor();
        Console.WriteLine($"   Sparad i: \\Exports\\{fileName}{extension}");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"FEL: {ex.Message}");
    }
    Console.ResetColor();
}