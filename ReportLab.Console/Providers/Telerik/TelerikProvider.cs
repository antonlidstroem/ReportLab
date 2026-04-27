using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportLab.Console.Core;

namespace ReportLab.Console.Providers.Telerik
{
    public class TelerikProvider : IReportProvider
    {
        public string Name => "Telerik_Trial";
        public void Export(ReportModel model, string filePath)
        {
            // Telerik uses 'ReportProcessor'
            // var reportSource = new Telerik.Reporting.InstanceReportSource { ReportDocument = yourReport };
            // var result = processor.RenderReport("PDF", reportSource, null);
        }
    }
}
