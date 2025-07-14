using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using HtmlAgilityPack;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Data.SqlClient;
using Microsoft.Playwright;
using TracePca.Dto;
using TracePca.Dto.LedgerReview;
using TracePca.Interface.LedgerReview;
//using static Org.BouncyCastle.Math.EC.ECCurve;
using PdfTextExtractor = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor;


namespace TracePca.Service.LedgerReview
{
    public class LedgerReviewService : LedgerReviewInterface
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IDbConnection _db;

        private readonly Dictionary<string, string[]> _ruleKeywordMap = new()
    {
        { "AS 1000", new[] { "general responsibility", "audit engagement" } },
        { "AS 2101", new[] { "planning", "opening entry" } },
        { "AS 1105", new[] { "evidence", "confirmation", "supporting document" } },
        { "AS 2401", new[] { "fraud", "irregular", "suspicious" } }
    };

        public LedgerReviewService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {

            _configuration = configuration;
            _httpClient = new HttpClient();
            _db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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


        public async Task<List<JEValidationResult>> ValidateAndSendTransactionsAsync()
        {
            var transactions = (await _db.QueryAsync<AccJETransaction>(@"
        SELECT 
            m.Acc_JE_TransactionNo,
            d.AJTB_Operation,
            d.AJTB_TranscNo
        FROM acc_je_master m
        LEFT JOIN Acc_JETransactions_Details d ON m.Acc_JE_TransactionNo = d.AJTB_TranscNo
    ")).ToList();

            var results = new List<JEValidationResult>();

            foreach (var txn in transactions)
            {
                var matchedRules = new List<string>();
                var messageBuilder = new StringBuilder();

                var combinedText = $"{txn.AJTB_Operation}".ToLower();

                foreach (var rule in _ruleKeywordMap)
                {
                    foreach (var keyword in rule.Value)
                    {
                        if (!string.IsNullOrWhiteSpace(keyword) && combinedText.Contains(keyword.ToLower()))
                        {
                            matchedRules.Add(rule.Key);
                            messageBuilder.AppendLine($"Matched keyword '{keyword}' with rule {rule.Key}.");
                            break;
                        }
                    }
                }

                results.Add(new JEValidationResult
                {
                    TransactionNo = txn.Acc_JE_TransactionNo,
                    IsValid = matchedRules.Any(),
                    MatchedRules = matchedRules,
                    ValidationMessage = matchedRules.Any()
                        ? messageBuilder.ToString().Trim()
                        : "No matching PCAOB rule found for this transaction."
                });
            }

            return results;
        }
    }
}











