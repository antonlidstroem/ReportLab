using Syncfusion.Presentation;
using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.Syncfusion;

public abstract class SyncfusionPptBaseProvider : IReportProvider
{
    public abstract string Name { get; }

    protected abstract void FillPresentation(IPresentation ppt, ReportModel model);

    public void Export(ReportModel model, string filePath)
    {
        using var ppt = Presentation.Create();
        FillPresentation(ppt, model);

        string finalPath = filePath.EndsWith(".pptx") ? filePath : filePath + ".pptx";
        using var fileStream = new FileStream(finalPath, FileMode.Create, FileAccess.Write);
        ppt.Save(fileStream);
    }
}