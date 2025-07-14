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
            d.AJTB_TranscNo,
            d.AJTB_Operation,
            d.AJTB_DescName,
            d.AJTB_CreatedOn,
            d.AJTB_Debit,
            d.AJTB_Credit
        FROM acc_je_master m
        LEFT JOIN Acc_JETransactions_Details d ON m.Acc_JE_TransactionNo = d.AJTB_TranscNo
    ")).ToList();

            var results = new List<JEValidationResult>();

            foreach (var txn in transactions)
            {
                var matchedRules = new List<string>();
                var messageBuilder = new StringBuilder();

                // ✅ Rule 1: AS 2401 - Fraud/Irregularities
                if ((txn.AJTB_Debit ?? 0) > 1000000 || (txn.AJTB_Credit ?? 0) > 1000000)
                {
                    matchedRules.Add("AS 2401");
                    messageBuilder.AppendLine("Matched AS 2401: High-value debit/credit may indicate fraud or irregularity.");
                }

                // ✅ Rule 2: AS 1105 - Audit Evidence (Entry with Description and Audit tag)
                if (!string.IsNullOrWhiteSpace(txn.AJTB_DescName) && txn.AJTB_DescName.Contains("audit", StringComparison.OrdinalIgnoreCase))
                {
                    matchedRules.Add("AS 1105");
                    messageBuilder.AppendLine("Matched AS 1105: Entry tagged for audit work – considered as audit evidence.");
                }

                // ✅ Rule 3: AS 2101 - Planning (Based on creation date or description pattern)
                if (txn.AJTB_CreatedOn.HasValue && txn.AJTB_CreatedOn.Value.Day <= 5) // beginning of month
                {
                    matchedRules.Add("AS 2101");
                    messageBuilder.AppendLine("Matched AS 2101: Entry made early in the period – indicates planning activity.");
                }

                // ✅ Rule 4: AS 1000 - Responsibility (Based on operation naming pattern)
                if (!string.IsNullOrWhiteSpace(txn.AJTB_Operation) && txn.AJTB_Operation.Contains("assigned", StringComparison.OrdinalIgnoreCase))
                {
                    matchedRules.Add("AS 1000");
                    messageBuilder.AppendLine("Matched AS 1000: Operation indicates responsibility assignment.");
                }

                // ✅ Rule 5: AS 1010 - Backdated Entry (Before 2020)
                if (txn.AJTB_CreatedOn.HasValue && txn.AJTB_CreatedOn.Value.Year < 2020)
                {
                    matchedRules.Add("AS 1010");
                    messageBuilder.AppendLine("Matched AS 1010: Entry is dated before 2020 – possibly backdated.");
                }

                results.Add(new JEValidationResult
                {
                    TransactionNo = txn.AJTB_TranscNo ?? txn.Acc_JE_TransactionNo,
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











