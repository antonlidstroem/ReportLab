namespace ReportLab.Console.Core;

public record DataPoint(double Value, string Label);

public class ReportModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Inspector { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;      
    public List<DataPoint> Stats { get; set; } = new();
}

public interface IReportProvider
{
    string Name { get; }
    void Export(ReportModel model, string filePath);
}