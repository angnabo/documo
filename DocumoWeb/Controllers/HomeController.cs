using System.Linq;
using System.Threading.Tasks;
using Documo.Renderer;
using Documo.TestData;
using DocumoWeb.Constants;
using DocumoWeb.Helpers;
using DocumoWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumoWeb.Controllers
{
    public class HomeController : Controller
    {
        
        public HomeController()
        {
            
        }
        
        public IActionResult Index()
        {
            var templateTypes = TemplateTypes.GetTemplateTypes()
                .ToDictionary(
                x => x.Id,
                x => x.Name);
            
            var model = new HomeModel
            {
                Html = "",
                TemplateTypes = templateTypes
            };
            
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Render(HomeModel model)
        {
            var testData = TestJsonObject.GetData();
            var file = await PdfRenderer.Render(model.Html, testData);
            return new FileContentResult(file, "application/pdf");
        }
        
        [HttpGet]
        public async Task<string> GetInvoiceTemplateHtmlCode(int id)
        {
            return await TemplateHelper.GetTemplateContents(id);
        }
        
        [HttpGet]
        public async Task<ViewResult> GetInvoiceTemplate(int id)
        {
            ViewBag.Html = await TemplateHelper.GetTemplateContents(id);
            return View("~/Views/_Template.cshtml");
        }
        
        [HttpPost]
        public async Task<ViewResult> RenderHtml(string html)
        {
            var sanitizedHtml = await HtmlRenderer.OpenDocument(html);
            var htmlTemplate = sanitizedHtml.DocumentElement.InnerHtml;
            ViewBag.Html = sanitizedHtml.DocumentElement.InnerHtml;
            return View("~/Views/_Template.cshtml");
        }
    }
}