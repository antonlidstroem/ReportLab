using ReportLab.Console.Core;


namespace ReportLab.Console.Providers.Text;

public class TextReportProvider : IReportProvider
{
    public string Name => "PlainText";

    public void Export(ReportModel model, string filePath)
    {
        var content = $"--- {model.Title} ---\n\n" +
                          $"{model.Description}\n\n" +
                      "DATA:\n" +
                      string.Join("\n", model.Stats.Select(s => $"{s.Label}: {s.Value}"));

        File.WriteAllText(filePath, content);
    }
}