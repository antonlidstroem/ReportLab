using Microsoft.AspNetCore.Mvc;
using BoldReports.Web.ReportDesigner;
using BoldReports.Web.ReportViewer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;

namespace ReportLab.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportDesignerController : ControllerBase, IReportDesignerController
    {
        private IMemoryCache _cache;
        private IWebHostEnvironment _hostingEnvironment;

        public ReportDesignerController(IMemoryCache memoryCache, IWebHostEnvironment hostingEnvironment)
        {
            _cache = memoryCache;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public object PostDesignerAction([FromBody] Dictionary<string, object> jsonResult)
        {
            return ReportDesignerHelper.ProcessDesigner(jsonResult, this, null, _cache);
        }

        [HttpPost]
        public object PostReportAction([FromBody] Dictionary<string, object> jsonResult)
        {
            return ReportHelper.ProcessReport(jsonResult, this, _cache);
        }

        [HttpGet]
        public object GetResource([FromQuery] ReportResource resource)
        {
            return ReportHelper.GetResource(resource, this, _cache);
        }

        [NonAction]
        public void OnInitReportOptions(ReportViewerOptions reportOption) { }

        [NonAction]
        public void OnReportLoaded(ReportViewerOptions reportOption) { }

        // FIX: Ändrad från object till void enligt felmeddelande CS0738
        [HttpPost]
        public void UploadReportAction()
        {
            ReportDesignerHelper.ProcessDesigner(null, this, Request.Form.Files[0], _cache);
        }

        [HttpGet]
        public object GetImage(string key, string imageId)
        {
            return ReportDesignerHelper.GetImage(key, imageId, this);
        }

        [HttpPost]
        public object PostFormDesignerAction()
        {
            return ReportDesignerHelper.ProcessDesigner(null, this, null, _cache);
        }

        [HttpPost]
        public object PostFormReportAction()
        {
            return ReportHelper.ProcessReport(null, this, _cache);
        }

        // FIX: Ändrad från void till bool enligt felmeddelande CS0738
        [NonAction]
        public bool SetData(string key, string itemId, ItemInfo itemData, out string errorMessage)
        {
            errorMessage = string.Empty;
            return true;
        }

        // FIX: Ändrad från object till ResourceInfo enligt felmeddelande CS0738
        [NonAction]
        public ResourceInfo GetData(string key, string itemId)
        {
            return null;
        }
    }
}