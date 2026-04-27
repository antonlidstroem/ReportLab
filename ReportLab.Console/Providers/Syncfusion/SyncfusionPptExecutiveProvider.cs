using Syncfusion.Presentation;
using Syncfusion.Drawing;
using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.Syncfusion;

public class SyncfusionPptExecutiveProvider : SyncfusionPptBaseProvider
{
    public override string Name => "Syncfusion_PPT_Executive_MAX";

    protected override void FillPresentation(IPresentation ppt, ReportModel model)
    {
        // 1. DEFINIERA VARUMÄRKESFÄRGER (Exakta RGB)
        var navyBlue = Color.FromArgb(26, 35, 126);
        var white = Color.White;
        var criticalRed = Color.FromArgb(211, 47, 47);

        // 2. SLIDE 1: PROFESSIONELL TITELSIDA
        ISlide titleSlide = ppt.Slides.Add(SlideLayoutType.Title);

        // Huvudtitel
        IShape titleShape = titleSlide.Shapes[0] as IShape;
        var titlePara = titleShape.TextBody.AddParagraph(model.Title.ToUpper());
        titlePara.Font.FontSize = 44;
        titlePara.Font.Bold = true;

        // Din fix: Använd .SystemColor för att injicera RGB-färgen
        titlePara.Font.Color.SystemColor = navyBlue;

        // Undertitel
        IShape subTitleShape = titleSlide.Shapes[1] as IShape;
        var subPara = subTitleShape.TextBody.AddParagraph($"STRATEGIC AUDIT // 2026\nINSPECTOR: {model.Inspector}");
        subPara.Font.FontSize = 18;

        // 3. SLIDE 2: ANALYTISK DATA
        ISlide dataSlide = ppt.Slides.Add(SlideLayoutType.Blank);

        // Header-band (Skapar en modern 'Corporate' look)
        IShape headerBand = dataSlide.Shapes.AddShape(AutoShapeType.Rectangle, 0, 0, 960, 60);
        headerBand.Fill.FillType = FillType.Solid;
        headerBand.Fill.SolidFill.Color.SystemColor = navyBlue;
        headerBand.LineFormat.Fill.FillType = FillType.None;

        var headerPara = headerBand.TextBody.AddParagraph("OPERATIONAL PERFORMANCE METRICS");
        headerPara.Font.Color.SystemColor = white;
        headerPara.Font.FontSize = 20;
        headerPara.Font.Bold = true;
        headerPara.HorizontalAlignment = HorizontalAlignmentType.Center;

        // 4. PROFESSIONELL TABELL
        // (X=50, Y=100, Bredd=860)
        ITable table = dataSlide.Shapes.AddTable(model.Stats.Count + 1, 2, 50, 100, 860, 400);

        // Header
        table[0, 0].TextBody.AddParagraph("INSPECTION AREA").Font.Bold = true;
        table[0, 1].TextBody.AddParagraph("PERFORMANCE SCORE").Font.Bold = true;

        for (int i = 0; i < model.Stats.Count; i++)
        {
            var stat = model.Stats[i];
            table[i + 1, 0].TextBody.AddParagraph(stat.Label);

            var scorePara = table[i + 1, 1].TextBody.AddParagraph(stat.Value.ToString("F1"));

            // Villkorlig formatering för kritiska värden
            if (stat.Value < 3.0)
            {
                scorePara.Font.Color.SystemColor = criticalRed;
                scorePara.Font.Bold = true;
            }
        }

        // SYNCFUSION POWER-MOVE: Applicera ett snyggt tema på tabellen
        table.BuiltInStyle = BuiltInTableStyle.MediumStyle2Accent1;

        // 5. SIDFOT (Branding)
        var footerText = dataSlide.Shapes.AddShape(AutoShapeType.Rectangle, 50, 500, 400, 30);
        footerText.Fill.FillType = FillType.None;
        footerText.LineFormat.Fill.FillType = FillType.None;
        var fPara = footerText.TextBody.AddParagraph($"Generated via Syncfusion Engine | {model.Date}");
        fPara.Font.FontSize = 9;
        fPara.Font.Color.SystemColor = Color.Gray;
    }
}