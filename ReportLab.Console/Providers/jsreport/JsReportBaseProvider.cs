using System.Globalization;
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.jsreport;

public abstract class JsReportBaseProvider : IReportProvider
{
    public abstract string Name { get; }

    // Varje subklass implementerar bara denna metod för att returnera sin HTML
    protected abstract string GetHtmlTemplate(ReportModel model, string labelsJson, string valuesJson);


    public string GetFullHtml(ReportModel model)
    {
        string labelsJson = string.Join(",", model.Stats.Select(s => $"\"{s.Label}\""));
        string valuesJson = string.Join(",", model.Stats.Select(s => s.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)));

        return GetHtmlTemplate(model, labelsJson, valuesJson);
    }

    public virtual void Export(ReportModel model, string filePath)
    {
        // 1. Använd din nya metod för att få färdig HTML
        string htmlTemplate = GetFullHtml(model);

        // 2. Setup jsreport
        var rs = new LocalReporting()
            .UseBinary(JsReportBinary.GetBinary())
            .AsUtility()
            .Create();

        // 3. Rendera (använder htmlTemplate från steg 1)
        var report = rs.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = htmlTemplate,
                Engine = Engine.None,
                Recipe = Recipe.ChromePdf
            }
        }).GetAwaiter().GetResult();

        // 4. Spara filen
        string finalPath = filePath.EndsWith(".pdf") ? filePath : filePath + ".pdf";
        using var fs = File.Create(finalPath);
        report.Content.CopyTo(fs);
    }
}