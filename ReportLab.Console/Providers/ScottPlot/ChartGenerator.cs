using ScottPlot;
using ReportLab.Console.Core; // Din Core

namespace ReportLab.Console.Providers.ScottPlot;

public static class ChartGenerator
{
    // Här är fixen: Vi skriver ut hela sökvägen till din DataPoint
    public static byte[] CreateBarChart(List<ReportLab.Console.Core.DataPoint> data)
    {
        var plot = new global::ScottPlot.Plot();

        double[] values = data.Select(x => x.Value).ToArray();
        var bars = plot.Add.Bars(values);

        foreach (var bar in bars.Bars)
        {
            bar.FillColor = Colors.SteelBlue;
        }

        // Här fixar vi labels
        var ticks = data.Select((x, i) => new Tick(i, x.Label)).ToArray();
        plot.Axes.Bottom.TickGenerator = new global::ScottPlot.TickGenerators.NumericManual(ticks);

        plot.Axes.Frameless();
        plot.HideGrid();
        plot.Title("Månatlig Försäljning", 24);

        return plot.GetImageBytes(600, 400, global::ScottPlot.ImageFormat.Png);
    }
}