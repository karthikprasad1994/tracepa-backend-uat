using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Playwright;
using TracePca.Dto.LedgerReview;
using TracePca.Interface.LedgerReview;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using PdfTextExtractor = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor;


namespace TracePca.Service.LedgerReview
{
    public class LedgerReviewService : LedgerReviewInterface
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;


        public LedgerReviewService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {

            _configuration = configuration;
            _httpClient = new HttpClient();

        }





      

   

    

   
        public async Task<List<PcaobRuleDto>> GetAllRulesAsync()
        {
            var result = new List<PcaobRuleDto>();
            var indexUrl = "https://pcaobus.org/oversight/standards/auditing-standards";
            var html = await _httpClient.GetStringAsync(indexUrl);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var pdfLinks = doc.DocumentNode
                .SelectNodes("//a[contains(@href, '.pdf')]")
                ?.Select(a => new
                {
                    Url = a.GetAttributeValue("href", "").Trim(),
                    Title = a.InnerText.Trim()
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Url))
                .ToList();

            if (pdfLinks == null || pdfLinks.Count == 0)
                return result;

            var tasks = pdfLinks.Select(async link =>
            {
                try
                {
                    var pdfUrl = link.Url.StartsWith("http") ? link.Url : $"https://pcaobus.org{link.Url}";
                    var title = link.Title;

                    var match = Regex.Match(title, @"AS\s*\d+");
                    var rule = match.Success ? match.Value : "Unknown";

                    var pdfBytes = await _httpClient.GetByteArrayAsync(pdfUrl);
                    var paragraphs = ExtractParagraphsFromPdf(pdfBytes);

                    return new PcaobRuleDto
                    {
                        Rule = rule,
                        Title = title,
                        Paragraphs = paragraphs
                    };
                }
                catch
                {
                    return null; // Skip broken PDFs
                }
            });

            var ruleDtos = await Task.WhenAll(tasks);
            result = ruleDtos.Where(r => r != null).ToList();

            return result;
        }

        private List<string> ExtractParagraphsFromPdf(byte[] pdfBytes)
        {
            var text = new StringBuilder();

            using var reader = new iText.Kernel.Pdf.PdfReader(new MemoryStream(pdfBytes));
            using var pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader);

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var page = pdfDoc.GetPage(i);
                var content = PdfTextExtractor.GetTextFromPage(page);
                text.AppendLine(content);
            }

            var allParagraphs = text.ToString()
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => p.Length > 40) // filter out short lines like headers
                .ToList();

            return allParagraphs;
        }
    }

}

    







