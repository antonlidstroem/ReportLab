using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.Syncfusion
{
    public class SyncfusionWebDesignerProvider : IReportProvider
    {
        public string Name => "Syncfusion Web Designer (Interaktiv)";

        public void Export(ReportModel model, string filePath)
        {
            // 1. Här kan vi starta webbservern om den inte redan körs
            // (I ett enkelt scenario kör du den manuellt först)

            string url = "http://localhost:7079/report-designer";

            System.Console.WriteLine($"Öppnar Syncfusion Designer på {url}...");

            // 2. Starta standardwebbläsaren
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
