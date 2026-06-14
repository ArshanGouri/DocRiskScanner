using Microsoft.AspNetCore.Mvc;
using UglyToad.PdfPig;
using System.Text;
using DocRiskScanner.Services;

namespace DocRiskScanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly GroqService _groqService;

        public HomeController(GroqService groqService)
        {
            _groqService = groqService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
            {
                ViewBag.Error = "Please select a PDF file.";
                return View("Index");
            }

            string extractedText;
            using (var stream = new MemoryStream())
            {
                await pdfFile.CopyToAsync(stream);
                stream.Position = 0;

                using var document = PdfDocument.Open(stream);
                var textBuilder = new StringBuilder();
                foreach (var page in document.GetPages())
                {
                    textBuilder.AppendLine(page.Text);
                }
                extractedText = textBuilder.ToString();
            }

            ViewBag.FileName = pdfFile.FileName;

            try
            {
                var analysis = await _groqService.AnalyzeDocumentAsync(extractedText);
                ViewBag.Analysis = analysis;
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"AI analysis failed: {ex.Message}";
            }

            return View("Index");
        }
    }
}