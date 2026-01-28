using Dapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using Google.Apis.Drive.v3.Data;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using TracePca.Data;
using TracePca.Dto.Audit;
using TracePca.Dto.DigitalFilling;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static Dropbox.Api.Team.DesktopPlatform;
using static TracePca.Dto.FIN_Statement.CashFlowDto;

namespace TracePca.Service.FIN_statement
{
    public class CashFlowService : CashFlowInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        public CashFlowService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
            _connectionString = GetConnectionStringFromSession();
        }

        private string GetConnectionStringFromSession()
        {
            var dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connStr = _configuration.GetConnectionString(dbName);
            if (string.IsNullOrWhiteSpace(connStr))
                throw new Exception($"Connection string for '{dbName}' not found in configuration.");

            return connStr;
        }

        //DeleteCashFlowCategoryWise
        public async Task DeleteCashflowCategory1Async(int compId, int pkId, int custId, int Category)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 4: Check if the row is one of the first 7 fixed rows
            var fixedParticulars = new[]
            {
                "Finance Costs",
                "Expense on employee stock option scheme",
                "Bad Debts",
                "Provision for impairment of fixed assets and intangibles",
                "Depreciation and amortisation",
                "Adjustment for:",
                "Net Profit / (Loss) before extraordinary items and tax",
                "A.Cash flow from operating activities"
            };

            var rowParticular = await connection.QueryFirstOrDefaultAsync<string>(@"
            SELECT ACF_Description
            FROM Acc_Cashflow 
            WHERE ACF_Catagary = @Category
               AND ACF_pkid = @PkId
               AND ACF_Custid = @CustId 
               AND ACF_Compid = @CompId",
                new { PkId = pkId, CustId = custId, CompId = compId, Category = Category });

            if (rowParticular == null)
                throw new Exception("Row not found.");

            if (fixedParticulars.Contains(rowParticular))
                throw new Exception($"Deletion not allowed for '{rowParticular}'.");

            // ✅ Step 5: Delete the row
            var sql = @"
            DELETE FROM Acc_Cashflow
            WHERE ACF_Catagary = @Category
               AND ACF_pkid = @PkId
               AND ACF_Custid = @CustId
               AND ACF_Compid = @CompId";

            await connection.ExecuteAsync(sql, new { PkId = pkId, CustId = custId, CompId = compId, Category = Category });
        }

        //GetCashFlowID(SearchButton)
        public async Task<int> GetCashFlowParticularsIdAsync(int compId, string description, int custId, int branchId)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 4: Parameterized query to get the pkid
            var sql = @"
            SELECT ACF_pkid 
            FROM Acc_Cashflow 
            WHERE ACF_Description = @Description
               AND ACF_Custid = @CustId
               AND ACF_BranchId = @BranchId
               AND ACF_Compid = @CompId";

            var pkid = await connection.QueryFirstOrDefaultAsync<int?>(sql, new
            {
                Description = description,
                CustId = custId,
                BranchId = branchId,
                CompId = compId
            });

            if (!pkid.HasValue)
                throw new Exception("Cashflow row not found.");

            return pkid.Value;
        }

        //GetCashFlowForAllCategory
        public async Task<IEnumerable<CashFlowForAllCategoryDto>> GetCashFlowForAllCategoryAsync(int compId, int custId, int yearId, int branchId, int category)
        {
            // ✅ Step 1: Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Use SqlConnection
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ✅ Step 4: Parameterized query to get cashflow data by category
            var sql = @"
            SELECT 
               ACF_pkid,
               ACF_Custid,
               ACF_Description, 
               ACF_Current_Amount, 
               ACF_Prev_Amount
            FROM Acc_CashFlow
            WHERE ACF_Catagary = @Category
               AND ACF_Compid = @CompId
               AND ACF_Custid = @CustId
               AND ACF_yearid = @YearId
               AND ACF_Branchid = @BranchId";

            return await connection.QueryAsync<CashFlowForAllCategoryDto>(sql, new
            {
                Category = category,
                CompId = compId,
                CustId = custId,
                YearId = yearId,
                BranchId = branchId
            });
        }

        //SaveCashFlow(Category 1)
        //    public async Task<(bool HasCashflow, List<CashflowParticularDto> Partials)> GetMandatoryCashflowInMemoryAsync(
        //  int yearId = 0, int customerId = 0, int branchId = 0)
        //    {
        //        int custId = customerId != 0 ? customerId : TryGetIntFromSession("CustId");
        //        int yrId = yearId != 0 ? yearId : TryGetIntFromSession("YearId");
        //        int brId = branchId != 0 ? branchId : TryGetIntFromSession("BranchId");

        //        string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
        //        if (string.IsNullOrEmpty(dbName))
        //            throw new Exception("CustomerCode missing in session. Please re-login.");

        //        string connStr = _configuration.GetConnectionString(dbName);
        //        using var connection = new SqlConnection(connStr);
        //        await connection.OpenAsync();

        //        var mandatoryParticulars = new List<string>
        //{
        //    "A.Cash flow from operating activities",
        //    "Net Profit / (Loss) before extraordinary items and tax",
        //    "Adjustment for:",
        //    "Depreciation and amortisation",
        //    "Provision for impairment of fixed assets and intangibles",
        //    "Bad Debts",
        //    "Expense on employee stock option scheme",
        //    "Finance Costs",
        //    "Interest income",
        //    "Total Adjustment",
        //    "Operating profit / (loss) before working capital changes"
        //};

        //        var dtos = mandatoryParticulars.Select(p => new CashflowParticularDto
        //        {
        //            Description = p,
        //            IsHeading = IsHeading(p),
        //            Amount = 0m,
        //            SourceHint = string.Empty,
        //            Note = string.Empty
        //        }).ToList();

        //        var hasCashflow = await CustomerHasCashflowAsync(connection, custId, yrId, brId);
        //        if (!hasCashflow)
        //            return (false, new List<CashflowParticularDto>());

        //        for (int i = 0; i < dtos.Count; i++)
        //        {
        //            var dto = dtos[i];
        //            if (dto.IsHeading) continue;

        //            string descNorm = Normalize(dto.Description);
        //            decimal fetched = 0m;
        //            string source = null;

        //            try
        //            {
        //                if (descNorm.Contains("net profit") || descNorm.Contains("before extraordinary") || descNorm.Contains("net income"))
        //                {
        //                    fetched = await GetPandLFinalAmt(connection, yrId, custId, brId);
        //                    source = "P&L: Net Income";
        //                }
        //                else if (descNorm.Contains("depreciation") || descNorm.Contains("amortisation") || descNorm.Contains("amortization"))
        //                {
        //                    fetched = await GetAmountByDescription(connection, yrId, custId, brId, "Depreciation and amortisation");
        //                    source = "P&L: Depreciation";
        //                }
        //                else if (descNorm.Contains("finance") && descNorm.Contains("cost"))
        //                {
        //                    fetched = await GetAmountByDescription(connection, yrId, custId, brId, "Finance Costs");
        //                    source = "P&L: Finance Costs";
        //                }
        //                else if (descNorm.Contains("interest income") || descNorm.Contains("interestincome"))
        //                {
        //                    fetched = await GetAmountByDescription(connection, yrId, custId, brId, "Interest income");
        //                    source = "P&L: Interest Income";
        //                }
        //                else if (descNorm.Contains("provision for impairment") || descNorm.Contains("impairment"))
        //                {
        //                    fetched = await GetAmountFromNotesOrFixedAssets(connection, yrId, custId, brId, "Provision for impairment of fixed assets and intangibles");
        //                    source = "Notes/FA: Provision for impairment";
        //                }
        //                else if (descNorm.Contains("bad debts") || descNorm.Contains("sundry") || descNorm.Contains("written off"))
        //                {
        //                    fetched = await GetAmountFromNotesOrTBDetail(connection, yrId, custId, brId, "Sundry Balance written off");
        //                    source = "Notes: Bad Debts";
        //                }
        //                else if (descNorm.Contains("employee stock option") || descNorm.Contains("esop"))
        //                {
        //                    fetched = await GetAmountFromNotesOrTBDetail(connection, yrId, custId, brId, "ESOP");
        //                    source = "Notes: ESOP";
        //                }
        //                else if (descNorm.Contains("total adjustment"))
        //                {
        //                    decimal dep = await GetAmountByDescription(connection, yrId, custId, brId, "Depreciation and amortisation");
        //                    decimal fin = await GetAmountByDescription(connection, yrId, custId, brId, "Finance Costs");
        //                    decimal bad = await GetAmountFromNotesOrTBDetail(connection, yrId, custId, brId, "Sundry Balance written off");
        //                    decimal interest = await GetAmountByDescription(connection, yrId, custId, brId, "Interest income");
        //                    fetched = dep + fin + bad + interest;
        //                    source = "Computed: Total Adjustment (dep+fin+bad+interest)";
        //                }
        //                else if (descNorm.Contains("operating profit") || descNorm.Contains("operating profit / (loss) before"))
        //                {
        //                    decimal net = await GetPandLFinalAmt(connection, yrId, custId, brId);
        //                    decimal dep = await GetAmountByDescription(connection, yrId, custId, brId, "Depreciation and amortisation");
        //                    decimal fin = await GetAmountByDescription(connection, yrId, custId, brId, "Finance Costs");
        //                    decimal bad = await GetAmountFromNotesOrTBDetail(connection, yrId, custId, brId, "Sundry Balance written off");
        //                    decimal interest = await GetAmountByDescription(connection, yrId, custId, brId, "Interest income");
        //                    decimal totalAdjustment = dep + fin + bad + interest;
        //                    fetched = net + totalAdjustment;
        //                    source = "Computed: Operating profit before working capital changes";
        //                }
        //                else
        //                {
        //                    fetched = await GetAmountByDescription(connection, yrId, custId, brId, dto.Description);
        //                    source = "TB: Direct description match";
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                dto.Note = $"Error fetching: {ex.Message}";
        //                fetched = 0m;
        //                source = source ?? "Error";
        //            }

        //            dto.Amount = Math.Round(fetched, 2);
        //            dto.SourceHint = source ?? string.Empty;
        //            if (dto.Amount == 0 && string.IsNullOrEmpty(dto.Note))
        //                dto.Note = "No TB match / zero value";
        //        }

        //        return (true, dtos);
        //    }

        //    // -------------------- Helpers --------------------
        //    private bool IsHeading(string text)
        //    {
        //        if (string.IsNullOrWhiteSpace(text)) return false;
        //        var t = text.Trim().ToLowerInvariant();
        //        return t.StartsWith("a.") || t.StartsWith("adjustment") || t.StartsWith("total") || t.Contains("cash flow");
        //    }
        //    private int TryGetIntFromSession(string key)
        //    {
        //        try
        //        {
        //            var s = _httpContextAccessor.HttpContext?.Session.GetString(key);
        //            if (string.IsNullOrWhiteSpace(s)) return 0;
        //            return int.TryParse(s, out var v) ? v : 0;
        //        }
        //        catch { return 0; }
        //    }
        //    private string Normalize(string text)
        //    {
        //        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        //        var cleaned = new string(text.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
        //        return Regex.Replace(cleaned.Trim(), @"\s+", " ").ToLowerInvariant();
        //    }
        //    private async Task<bool> CustomerHasCashflowAsync(SqlConnection conn, int custId, int yearId, int branchId)
        //    {
        //        if (custId == 0 || yearId == 0) return false;

        //        const string sqlTb = @"
        //    SELECT TOP 1 1
        //    FROM Acc_TrailBalance_Upload
        //    WHERE ATBU_CustId = @CustId
        //      AND ATBU_YearId = @YearId";
        //        var tbExists = await conn.ExecuteScalarAsync<int?>(sqlTb, new { CustId = custId, YearId = yearId });
        //        if (tbExists.HasValue && tbExists.Value == 1) return true;

        //        const string sqlCf = @"
        //    SELECT TOP 1 1
        //    FROM Acc_CashFlow
        //    WHERE ACF_Custid = @CustId
        //      AND ISNULL(ACF_Yearid,0) = @YearId
        //      AND ISNULL(ACF_Catagary,0) = 1";
        //        var cfExists = await conn.ExecuteScalarAsync<int?>(sqlCf, new { CustId = custId, YearId = yearId });
        //        if (cfExists.HasValue && cfExists.Value == 1) return true;

        //        return false;
        //    }

        //    private async Task<decimal> GetAmountByDescription(SqlConnection conn, int yearId, int customerId, int branchId, string description)
        //    {
        //        if (string.IsNullOrWhiteSpace(description) || customerId == 0 || yearId == 0) return 0m;

        //        // 1) exact match scoped to branch
        //        const string sqlExact = @"
        //    SELECT ISNULL(SUM(ISNULL(ATBU_Closing_TotalDebit_Amount,0) - ISNULL(ATBU_Closing_TotalCredit_Amount,0)), 0) AS Amt
        //    FROM Acc_TrailBalance_Upload
        //    WHERE ATBU_Description = @Desc
        //      AND ATBU_YearId = @YearId
        //      AND ATBU_CustId = @CustomerId
        //      AND (@BranchId = 0 OR ISNULL(ATBU_BranchId,0) = @BranchId)";

        //        var exactVal = await conn.ExecuteScalarAsync<decimal?>(sqlExact, new
        //        {
        //            Desc = description,
        //            YearId = yearId,
        //            CustomerId = customerId,
        //            BranchId = branchId
        //        });

        //        if (exactVal.HasValue && exactVal.Value != 0m)
        //            return exactVal.Value;

        //        // 2) tokenized LIKE fallback (use longest token to reduce false positives)
        //        var normalized = Normalize(description);
        //        var tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(t => t.Length >= 3).ToArray();
        //        if (tokens.Length == 0) return 0m;
        //        var bestToken = tokens.OrderByDescending(t => t.Length).First();

        //        const string sqlLike = @"
        //    SELECT ISNULL(SUM(ISNULL(ATBU_Closing_TotalDebit_Amount,0) - ISNULL(ATBU_Closing_TotalCredit_Amount,0)), 0) AS Amt
        //    FROM Acc_TrailBalance_Upload
        //    WHERE ATBU_Description LIKE '%' + @Part + '%'
        //      AND ATBU_YearId = @YearId
        //      AND ATBU_CustId = @CustomerId
        //      AND (@BranchId = 0 OR ISNULL(ATBU_BranchId,0) = @BranchId)";

        //        var likeVal = await conn.ExecuteScalarAsync<decimal?>(sqlLike, new
        //        {
        //            Part = bestToken,
        //            YearId = yearId,
        //            CustomerId = customerId,
        //            BranchId = branchId
        //        });

        //        if (likeVal.HasValue && likeVal.Value != 0m)
        //            return likeVal.Value;

        //        // 3) grouped top match (broader fallback, branch optional)
        //        const string sqlGroupTop = @"
        //    SELECT TOP 1 ISNULL(SUM(ISNULL(ATBU_Closing_TotalDebit_Amount,0) - ISNULL(ATBU_Closing_TotalCredit_Amount,0)),0) AS Amt
        //    FROM Acc_TrailBalance_Upload
        //    WHERE ATBU_Description LIKE '%' + @Part + '%'
        //      AND ATBU_YearId = @YearId
        //      AND ATBU_CustId = @CustomerId
        //      AND (@BranchId = 0 OR ISNULL(ATBU_BranchId,0) = @BranchId)
        //    GROUP BY ATBU_Description
        //    ORDER BY ABS(ISNULL(SUM(ISNULL(ATBU_Closing_TotalDebit_Amount,0) - ISNULL(ATBU_Closing_TotalCredit_Amount,0)),0)) DESC";

        //        var groupVal = await conn.ExecuteScalarAsync<decimal?>(sqlGroupTop, new
        //        {
        //            Part = bestToken,
        //            YearId = yearId,
        //            CustomerId = customerId,
        //            BranchId = branchId
        //        });

        //        return groupVal ?? 0m;
        //    }

        //    private async Task<decimal> GetPandLFinalAmt(SqlConnection conn, int yearId, int customerId, int branchId)
        //    {
        //        if (customerId == 0 || yearId == 0) return 0m;

        //        const string sql = @"
        //    SELECT ISNULL(SUM(ISNULL(ATBU_Closing_TotalDebit_Amount,0) - ISNULL(ATBU_Closing_TotalCredit_Amount,0)),0)
        //    FROM Acc_TrailBalance_Upload
        //    WHERE ATBU_Description IN ('Net Income','Net Profit','Profit for the year')
        //      AND ATBU_YearId = @YearId
        //      AND ATBU_CustId = @CustomerId
        //      AND (@BranchId = 0 OR ISNULL(ATBU_BranchId,0) = @BranchId)";

        //        var val = await conn.ExecuteScalarAsync<decimal?>(sql, new
        //        {
        //            YearId = yearId,
        //            CustomerId = customerId,
        //            BranchId = branchId
        //        });

        //        return val ?? 0m;
        //    }

        //    private async Task<decimal> GetAmountFromNotesOrTBDetail(SqlConnection conn, int yearId, int customerId, int branchId, string partialDesc)
        //    {
        //        if (string.IsNullOrWhiteSpace(partialDesc) || customerId == 0 || yearId == 0) return 0m;

        //        const string sql = @"
        //    SELECT TOP 1
        //        ISNULL(SUM(ISNULL(ATBU_Closing_TotalDebit_Amount,0) - ISNULL(ATBU_Closing_TotalCredit_Amount,0)), 0) AS Amt
        //    FROM Acc_TrailBalance_Upload
        //    WHERE ATBU_Description LIKE '%' + @Part + '%'
        //      AND ATBU_YearId = @YearId
        //      AND ATBU_CustId = @CustomerId
        //      AND (@BranchId = 0 OR ISNULL(ATBU_BranchId,0) = @BranchId)
        //    GROUP BY ATBU_Description
        //    ORDER BY ABS(ISNULL(SUM(ISNULL(ATBU_Closing_TotalDebit_Amount,0) - ISNULL(ATBU_Closing_TotalCredit_Amount,0)),0)) DESC";

        //        var val = await conn.ExecuteScalarAsync<decimal?>(sql, new
        //        {
        //            Part = partialDesc,
        //            YearId = yearId,
        //            CustomerId = customerId,
        //            BranchId = branchId
        //        });

        //        return val ?? 0m;
        //    }

        //    private async Task<decimal> GetAmountFromNotesOrFixedAssets(SqlConnection conn, int yearId, int customerId, int branchId, string partialDesc)
        //    {
        //        if (string.IsNullOrWhiteSpace(partialDesc) || customerId == 0 || yearId == 0) return 0m;

        //        const string sqlExact = @"
        //    SELECT ISNULL(SUM(ISNULL(ATBU_Closing_TotalDebit_Amount,0) - ISNULL(ATBU_Closing_TotalCredit_Amount,0)), 0) AS Amt
        //    FROM Acc_TrailBalance_Upload
        //    WHERE ATBU_Description = @Desc
        //      AND ATBU_YearId = @YearId
        //      AND ATBU_CustId = @CustomerId
        //      AND (@BranchId = 0 OR ISNULL(ATBU_BranchId,0) = @BranchId)";

        //        var exactVal = await conn.ExecuteScalarAsync<decimal?>(sqlExact, new
        //        {
        //            Desc = partialDesc,
        //            YearId = yearId,
        //            CustomerId = customerId,
        //            BranchId = branchId
        //        });

        //        if (exactVal.HasValue && exactVal.Value != 0m)
        //            return exactVal.Value;

        //        const string sqlLike = @"
        //    SELECT TOP 1 
        //        ISNULL(SUM(ISNULL(ATBU_Closing_TotalDebit_Amount,0) - ISNULL(ATBU_Closing_TotalCredit_Amount,0)), 0) AS Amt
        //    FROM Acc_TrailBalance_Upload
        //    WHERE ATBU_Description LIKE '%' + @Part + '%'
        //      AND ATBU_YearId = @YearId
        //      AND ATBU_CustId = @CustomerId
        //      AND (@BranchId = 0 OR ISNULL(ATBU_BranchId,0) = @BranchId)
        //    GROUP BY ATBU_Description
        //    ORDER BY ABS(ISNULL(SUM(ISNULL(ATBU_Closing_TotalDebit_Amount,0) - ISNULL(ATBU_Closing_TotalCredit_Amount,0)),0)) DESC";

        //        var likeVal = await conn.ExecuteScalarAsync<decimal?>(sqlLike, new
        //        {
        //            Part = partialDesc,
        //            YearId = yearId,
        //            CustomerId = customerId,
        //            BranchId = branchId
        //        });

        //        return likeVal ?? 0m;
        //    }

        //    private async Task<int> GetHeadingId(SqlConnection conn, int customerId, string headName)
        //    {
        //        if (customerId == 0 || string.IsNullOrWhiteSpace(headName)) return 0;
        //        const string sql = @"
        //    SELECT ISNULL(ASH_ID,0) FROM ACC_ScheduleHeading WHERE ASH_Name = @HeadName AND ASH_OrgType = @CustId";
        //        return await conn.ExecuteScalarAsync<int?>(sql, new { HeadName = headName, CustId = customerId }) ?? 0;
        //    }
        //    private async Task<int> GetSubHeadingId(SqlConnection conn, int customerId, string subHeadName)
        //    {
        //        if (customerId == 0 || string.IsNullOrWhiteSpace(subHeadName)) return 0;
        //        const string sql = @"
        //    SELECT ISNULL(ASSH_ID,0) FROM ACC_SchedulesubHeading WHERE ASSH_Name = @SubHeadName AND ASSH_OrgType = @CustId";
        //        return await conn.ExecuteScalarAsync<int?>(sql, new { SubHeadName = subHeadName, CustId = customerId }) ?? 0;
        //    }
        //    private async Task<HeadingAmountDto> GetHeadingAmt1(SqlConnection conn, int yearId, int customerId, int schedType, int headingId)
        //    {
        //        if (headingId == 0) return new HeadingAmountDto();

        //        const string sql = @"
        //    SELECT 
        //        ISNULL(SUM(ISNULL(d.ATBU_Closing_TotalDebit_Amount,0) - ISNULL(d.ATBU_Closing_TotalCredit_Amount,0)),0) AS Dc1,
        //        ISNULL(SUM(ISNULL(e.ATBU_Closing_TotalDebit_Amount,0) - ISNULL(e.ATBU_Closing_TotalCredit_Amount,0)),0) AS DP1
        //    FROM Acc_TrailBalance_Upload_Details
        //    LEFT JOIN Acc_TrailBalance_Upload d 
        //        ON d.ATBU_Description = ATBUD_Description AND d.ATBU_YearId = @YearId AND d.ATBU_CustId = @CustomerId
        //    LEFT JOIN Acc_TrailBalance_Upload e 
        //        ON e.ATBU_Description = ATBUD_Description AND e.ATBU_YearId = @PrevYear AND e.ATBU_CustId = @CustomerId
        //    WHERE ATBUD_Schedule_Type = @SchedType
        //      AND ATBUD_CustId = @CustomerId
        //      AND ATBUD_HeadingId = @HeadingId
        //    GROUP BY ATBUD_HeadingId";
        //        var data = await conn.QueryFirstOrDefaultAsync<HeadingAmountDto>(sql, new
        //        {
        //            YearId = yearId,
        //            PrevYear = yearId - 1,
        //            CustomerId = customerId,
        //            SchedType = schedType,
        //            HeadingId = headingId
        //        });
        //        return data ?? new HeadingAmountDto();
        //    }

        //    // Subheading aggregation helper
        //    private async Task<HeadingAmountDto> GetSubHeadingAmt1(SqlConnection conn, int yearId, int customerId, int schedType, int subHeadingId)
        //    {
        //        if (subHeadingId == 0) return new HeadingAmountDto();

        //        const string sql = @"
        //    SELECT 
        //        ISNULL(SUM(ISNULL(d.ATBU_Closing_TotalDebit_Amount,0) - ISNULL(d.ATBU_Closing_TotalCredit_Amount,0)),0) AS Dc1,
        //        ISNULL(SUM(ISNULL(e.ATBU_Closing_TotalDebit_Amount,0) - ISNULL(e.ATBU_Closing_TotalCredit_Amount,0)),0) AS DP1
        //    FROM Acc_TrailBalance_Upload_Details
        //    LEFT JOIN Acc_TrailBalance_Upload d 
        //        ON d.ATBU_Description = ATBUD_Description AND d.ATBU_YearId = @YearId AND d.ATBU_CustId = @CustomerId
        //    LEFT JOIN Acc_TrailBalance_Upload e 
        //        ON e.ATBU_Description = ATBUD_Description AND e.ATBU_YearId = @PrevYear AND e.ATBU_CustId = @CustomerId
        //    WHERE ATBUD_Schedule_Type = @SchedType
        //      AND ATBUD_CustId = @CustomerId
        //      AND ATBUD_SubHeading = @SubHeadingId
        //    GROUP BY ATBUD_HeadingId";
        //        var data = await conn.QueryFirstOrDefaultAsync<HeadingAmountDto>(sql, new
        //        {
        //            YearId = yearId,
        //            PrevYear = yearId - 1,
        //            CustomerId = customerId,
        //            SchedType = schedType,
        //            SubHeadingId = subHeadingId
        //        });
        //        return data ?? new HeadingAmountDto();
        //    }

        public async Task<CashFlowCategory1Result> LoadCashFlowCategory1Async(int customerId, int yearId, int branchId, List<UserAdjustmentInput>? userAdjustments)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            string connectionString = _configuration.GetConnectionString(dbName)
                ?? throw new Exception($"Connection string for '{dbName}' not found.");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var tran = connection.BeginTransaction();
            try
            {
                Decimal dCashflorTotal = 0m;
                var result = new CashFlowCategory1Result
                {
                    Particular = new List<CashFlowParticularDto>()
                };

                decimal Safe(decimal? v) => v ?? 0m;

                // -------- FIXED LABEL ORDER ---------
                string[] labels =
                {
                    "A. Cash flow from operating activities",
                    "Net Profit / (Loss) before extraordinary items and tax",
                    "Adjustment for:",
                    "Depreciation and amortisation Expenses",
                    "Provision for impairment of fixed assets and intangibles",
                    "Bad Debts",
                    "Expense on employee stock option scheme",
                    "Finance Costs",
                    "Interest income",
                    "Operating profit / (loss) before working capital changes"
                };

                // Row 1: A. Cash flow from operating activities
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 1,
                    ParticularName = labels[0]
                });

                // Row 2: Net Profit / (Loss) before extraordinary items and tax
                int headIncomeId = await GetHeadingId(connection, tran, customerId, "Income");
                int headExpenseId = await GetHeadingId(connection, tran, customerId, "Expenses");

                var income = await GetHeadingAmt(connection, tran, yearId, customerId, 3, headIncomeId);
                var expense = await GetHeadingAmt(connection, tran, yearId, customerId, 3, headExpenseId);

                decimal netCY = Safe(income.Dc1) - Safe(expense.Dc1);
                decimal netPY = Safe(income.DP1) - Safe(expense.DP1);
                dCashflorTotal += netCY;

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 2,
                    ParticularName = labels[1],
                    CurrentYear = netCY,
                    PreviousYear = netPY
                });

                // Row 3: Adjustment for
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[2]
                });

                // totals and starting SR
                decimal totalAdjCY = 0m;
                decimal totalAdjPY = 0m;
                int sr = 4;

                // Row 4: Depreciation and amortisation Expenses (subheading)
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[3]);
                    // breakpoint here to inspect depSubHeadId
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 3, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalAdjCY += depCY;
                    totalAdjPY += depPY;
                    dCashflorTotal += depCY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[3],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }
                // Row 5: Provision for impairment of fixed assets and intangibles
                {
                    int provHeadId = await GetHeadingId(connection, tran, customerId, labels[4]);
                    // breakpoint here to inspect provHeadId
                    var provDt = await GetHeadingAmt(connection, tran, yearId, customerId, 3, provHeadId);
                    decimal provCY = Safe(provDt.Dc1);
                    decimal provPY = Safe(provDt.DP1);
                    totalAdjCY += provCY;
                    totalAdjPY += provPY;
                    dCashflorTotal += provCY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[4],
                        CurrentYear = provCY,
                        PreviousYear = provPY
                    });
                }
                // Row 6: Bad Debts (heading)
                {
                    int badHeadId = await GetHeadingId(connection, tran, customerId, labels[5]);
                    var badDt = await GetHeadingAmt(connection, tran, yearId, customerId, 3, badHeadId);
                    decimal badCY = Safe(badDt.Dc1);
                    decimal badPY = Safe(badDt.DP1);
                    totalAdjCY += badCY;
                    totalAdjPY += badPY;
                    dCashflorTotal += badCY;
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[5],
                        CurrentYear = badCY,
                        PreviousYear = badPY
                    });
                }
                // Row 7: Expense on employee stock option scheme (heading)
                {
                    int esopHeadId = await GetHeadingId(connection, tran, customerId, labels[6]);
                    var esopDt = await GetHeadingAmt(connection, tran, yearId, customerId, 3, esopHeadId);
                    decimal esopCY = Safe(esopDt.Dc1);
                    decimal esopPY = Safe(esopDt.DP1);
                    totalAdjCY += esopCY;
                    totalAdjPY += esopPY;
                    dCashflorTotal += esopCY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[6],
                        CurrentYear = esopCY,
                        PreviousYear = esopPY
                    });
                }
                // Row 8: Finance Costs (subheading)
                {
                    int finSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[7]);
                    var finDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 3, finSubHeadId);
                    decimal finCY = Safe(finDt.Dc1);
                    decimal finPY = Safe(finDt.DP1);
                    totalAdjCY += finCY;
                    totalAdjPY += finPY;
                    dCashflorTotal += finCY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[7],
                        CurrentYear = finCY,
                        PreviousYear = finPY
                    });
                }
                // Row 9: Interest income (subheading)
                {
                    int intSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[8]);
                    var intDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 3, intSubHeadId);
                    decimal intCY = Safe(intDt.Dc1);
                    decimal intPY = Safe(intDt.DP1);
                    totalAdjCY += intCY;
                    totalAdjPY += intPY;
                    dCashflorTotal += intCY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[8],
                        CurrentYear = intCY,
                        PreviousYear = intPY
                    });
                }



                //Dynamic Data
                var intList = await GetSubHeadingDynamicDataAmt(connection, tran, yearId, customerId, 3);
                foreach (var intDt in intList)
                {
                    decimal intCY = Safe(intDt.Dc1);
                    decimal intPY = Safe(intDt.DP1);

                    totalAdjCY += intCY;
                    totalAdjPY += intPY;
                    dCashflorTotal += intCY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = intDt.sSubHeading,
                        CurrentYear = intCY,
                        PreviousYear = intPY
                    });
                }
                 


                // USER-ADDED ADJUSTMENTS (AFTER SR 9)
                if (userAdjustments != null)
                {
                    foreach (var ua in userAdjustments)
                    {
                        totalAdjCY += ua.CurrentYear;
                        totalAdjPY += ua.PreviousYear;

                        result.Particular.Add(new CashFlowParticularDto
                        {
                            Sr_No = sr++,
                            ParticularName = ua.Description,
                            CurrentYear = dCashflorTotal,
                            PreviousYear = ua.PreviousYear
                        });
                    }
                }
                // Row (last): Operating Profit = NetProfit + Total Adjustments
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = sr,
                    ParticularName = labels[9], 
                    CurrentYear = netCY + totalAdjCY,
                    PreviousYear = netPY + totalAdjPY
                });

                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<CashFlowCategory1Result> LoadCashFlowCategory2Async(int customerId, int yearId, int branchId, List<UserAdjustmentInput>? userAdjustments)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            string connectionString = _configuration.GetConnectionString(dbName)
                ?? throw new Exception($"Connection string for '{dbName}' not found.");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var tran = connection.BeginTransaction();
            try
            {
                var result = new CashFlowCategory1Result
                {
                    Particular = new List<CashFlowParticularDto>()
                };

                decimal Safe(decimal? v) => v ?? 0m;

                // -------- FIXED LABEL ORDER ---------
                string[] labels =
                {
                    "Changes in working capital:",
                    "Adjustments for (increase) / decrease in operating assets:",
                    "Inventories",
                    "Trade receivables",
                    "Short-term loans and advances",
                    "Long-term loans and advances",
                    "Other current Assets",
                    "Adjustments for increase / (decrease) in operating liabilities:",
                    "Trade payables",
                    "Other current liabilities",
                    "Short-term provisions",
                    "",
                    "Operating profit / (loss) after working capital changes",
                    "give Add Description option ( + or - )",
                    "  Cash flow from extraordinary items/prior period items - Sale of Fixed Asset",
                    "  Cash generated from operations",
                    "give Add Description option ( + or - )",
                    "  Net income tax (paid) / refunds (net)",
                    "give Add Description option ( + or - )",
                    "Net cash flow from / (used in) operating activities (A)"
                };

                // Row 1: Changes in working capital:
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 1,
                    ParticularName = labels[0]
                });

                // Row 2: Adjustments for (increase) / decrease in operating assets:
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 1,
                    ParticularName = labels[1]
                });

                // Row 3: Inventories
                decimal totalCat2CY = 0m;
                decimal totalCat2PY = 0m;
                int sr = 4;

                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[2]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 4, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat2CY += depCY;
                    totalCat2PY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 8,
                        ParticularName = labels[2],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }


                // Row 4: Trade receivables
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[3]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 4, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat2CY += depCY;
                    totalCat2PY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 8,
                        ParticularName = labels[3],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }


                // Row 5: Short-term loans and advances
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[4]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 4, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat2CY += depCY;
                    totalCat2PY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 8,
                        ParticularName = labels[4],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }


                // Row 6: Long-term loans and advances
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[5]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 4, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat2CY += depCY;
                    totalCat2PY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 8,
                        ParticularName = labels[5],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }

                // Row 7: Other current Assets
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[6]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 4, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat2CY += depCY;
                    totalCat2PY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 8,
                        ParticularName = labels[6],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }

                // Row 8: Adjustments for increase / (decrease) in operating liabilities:
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 9,
                        ParticularName = labels[7],
                        CurrentYear = 0,
                        PreviousYear = 0
                    });
                }


                // Row 9: Trade payables
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[8]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 4, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat2CY += depCY;
                    totalCat2PY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 8,
                        ParticularName = labels[8],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }


                // Row 10: Other current liabilities
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[9]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 4, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat2CY += depCY;
                    totalCat2PY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 8,
                        ParticularName = labels[9],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }


                // Row 11: Short-term provisions
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[10]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 4, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat2CY += depCY;
                    totalCat2PY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 8,
                        ParticularName = labels[10],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }


                //Dynamic Data
                var intList = await GetSubHeadingDynamicDataAmt(connection, tran, yearId, customerId, 4);
                foreach (var intDt in intList)
                {
                    decimal intCY = Safe(intDt.Dc1);
                    decimal intPY = Safe(intDt.DP1);

                    totalCat2CY += intCY;
                    totalCat2PY += intPY;
                     

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = intDt.sSubHeading,
                        CurrentYear = intCY,
                        PreviousYear = intPY
                    });
                }


                // Row 12:  ""
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[11],
                        CurrentYear = totalCat2CY,
                        PreviousYear = totalCat2PY
                    });
                }



                // Row 12: Operating profit / (loss) after working capital changes
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[12],
                        CurrentYear = totalCat2CY,
                        PreviousYear = totalCat2PY
                    });
                }


                // Row 13: give Add Description option ( + or - )
                decimal totalCashGenCY = 0m;
                decimal totalCashGenPY = 0m;

                totalCashGenCY += totalCat2CY;
                totalCashGenPY += totalCat2PY;
                 
                {
                    decimal depCY = Safe(1);
                    decimal depPY = Safe(0);
                    totalCashGenCY += depCY;
                    totalCashGenPY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[13],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }


                // Row 14: Cash flow from extraordinary items/prior period items - Sale of Fixed Asset
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[14],
                        CurrentYear = 0,
                        PreviousYear = 0
                    });
                }

                // Row 15:  Cash generated from operations
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[15],
                        CurrentYear = totalCashGenCY,
                        PreviousYear = totalCashGenPY
                    });
                }


                // Row 16:  Net income tax(paid) / refunds(net)
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[16],
                        CurrentYear = 0,
                        PreviousYear = 0
                    });
                }

                // Row 17: give Add Description option ( + or - )
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[17],
                        CurrentYear = 0,
                        PreviousYear = 0
                    });
                }

                // Row 18:Net income tax (paid) / refunds (net)
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[18],
                        CurrentYear = 0,
                        PreviousYear = 0
                    });
                }

                // Row 19: Net cash flow from / (used in) operating activities (A)
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = labels[19],
                        CurrentYear = totalCashGenCY,
                        PreviousYear = totalCashGenPY
                    });
                } 
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<CashFlowCategory1Result> LoadCashFlowCategory3Async(int customerId, int yearId, int branchId, List<UserAdjustmentInput>? userAdjustments)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            string connectionString = _configuration.GetConnectionString(dbName)
                ?? throw new Exception($"Connection string for '{dbName}' not found.");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var tran = connection.BeginTransaction();
            try
            {
                var result = new CashFlowCategory1Result
                {
                    Particular = new List<CashFlowParticularDto>()
                };

                decimal Safe(decimal? v) => v ?? 0m;

                // -------- FIXED LABEL ORDER ---------
                string[] labels =
                {
                    "B. Cash flow from investing activities",
                    "Capital expenditure on fixed assets, including capital advances ",
                    "Capital Work In Progress - ",
                    "",
                    "Net cash flow from / (used in) investing activities (B)"
                };

                // Row 1: B. Cash flow from investing activities
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 1,
                    ParticularName = labels[0]
                });


                // Row 2: Capital expenditure on fixed assets, including capital advances
                decimal totalCat3CY = 0m;
                decimal totalCat3PY = 0m;
                int sr = 4;

                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[1]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 4, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat3CY += depCY;
                    totalCat3PY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 2,
                        ParticularName = labels[1],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }


                // Row 3: Capital Work In Progress -
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[2]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 4, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat3CY += depCY;
                    totalCat3PY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 3,
                        ParticularName = labels[2],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }

                //Dynamic Data
                var intList = await GetSubHeadingDynamicDataAmt(connection, tran, yearId, customerId, 4);
                foreach (var intDt in intList)
                {
                    decimal intCY = Safe(intDt.Dc1);
                    decimal intPY = Safe(intDt.DP1);

                    totalCat3CY += intCY;
                    totalCat3PY += intPY;


                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = intDt.sSubHeading,
                        CurrentYear = intCY,
                        PreviousYear = intPY
                    });
                }



                // Row 4: 
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 4,
                        ParticularName = labels[3],
                        CurrentYear = totalCat3CY,
                        PreviousYear = totalCat3PY
                    });
                }


                // Row 5: Net cash flow from / (used in) investing activities (B)
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 5,
                        ParticularName = labels[4],
                        CurrentYear = totalCat3CY,
                        PreviousYear = totalCat3PY
                    });
                }
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<CashFlowCategory1Result> LoadCashFlowCategory4Async(int customerId, int yearId, int branchId, List<UserAdjustmentInput>? userAdjustments)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            string connectionString = _configuration.GetConnectionString(dbName)
                ?? throw new Exception($"Connection string for '{dbName}' not found.");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var tran = connection.BeginTransaction();
            try
            {
                var result = new CashFlowCategory1Result
                {
                    Particular = new List<CashFlowParticularDto>()
                };

                decimal Safe(decimal? v) => v ?? 0m;

                // -------- FIXED LABEL ORDER ---------
                string[] labels =
                {
                    "C. Cash flow from financing activities",
                    "Proceeds from issue of equity shares",
                    "Share application money received / (refunded)",
                    "Proceeds from / (Repayment) of long-term borrowings",
                    "Proceeds of / (Repayment of) other short-term borrowings",
                    "Interest Received on deposits/Income tax refund",
                    "Insurance claims received  it refund ",
                    "Dividend Income",
                    "Finance cost",
                    "",
                    "Net cash flow from / (used in) financing activities (C)",
                };

                // Row 1: B. Cash flow from investing activities
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 1,
                    ParticularName = labels[0]
                });


                // Row 2: Proceeds from issue of equity shares
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 2,
                    ParticularName = labels[1]
                });

                // Row 3: Share application money received / (refunded)
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[2]
                });

                // Row 4: Proceeds from / (Repayment) of long-term borrowings
                decimal totalCat4CY = 0m;
                decimal totalCat4PY = 0m;
                int sr = 4;

                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[3]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 5, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat4CY += depCY;
                    totalCat4CY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 4,
                        ParticularName = labels[3],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }


                // Row 5: Proceeds of / (Repayment of) other short-term borrowings
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[4]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 5, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat4CY += depCY;
                    totalCat4CY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 4,
                        ParticularName = labels[4],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }


                // Row 6: Interest Received on deposits/Income tax refund
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[5]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 5, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat4CY += depCY;
                    totalCat4CY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 5,
                        ParticularName = labels[5],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }

                // Row 7: Insurance claims received  it refund 
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[6]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 5, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat4CY += depCY;
                    totalCat4CY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 6,
                        ParticularName = labels[6],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }

                // Row 7: Dividend Income
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[7]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 5, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat4CY += depCY;
                    totalCat4CY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 6,
                        ParticularName = labels[7],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }

                // Row 7: Finance cost
                {
                    int depSubHeadId = await GetSubHeadingId(connection, tran, customerId, labels[8]);
                    var depDt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 5, depSubHeadId);
                    decimal depCY = Safe(depDt.Dc1);
                    decimal depPY = Safe(depDt.DP1);
                    totalCat4CY += depCY;
                    totalCat4CY += depPY;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 6,
                        ParticularName = labels[8],
                        CurrentYear = depCY,
                        PreviousYear = depPY
                    });
                }


                //Dynamic Data
                var intList = await GetSubHeadingDynamicDataAmt(connection, tran, yearId, customerId, 5);
                foreach (var intDt in intList)
                {
                    decimal intCY = Safe(intDt.Dc1);
                    decimal intPY = Safe(intDt.DP1);

                    totalCat4CY += intCY;
                    totalCat4CY += intPY;


                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr++,
                        ParticularName = intDt.sSubHeading,
                        CurrentYear = intCY,
                        PreviousYear = intPY
                    });
                }

                // Row 7: ""
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 6,
                        ParticularName = labels[9],
                        CurrentYear = totalCat4CY,
                        PreviousYear = totalCat4CY
                    });
                }

                // Row 7: "Net cash flow from / (used in) financing activities (C)"
                {
                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 6,
                        ParticularName = labels[10],
                        CurrentYear = totalCat4CY,
                        PreviousYear = totalCat4CY
                    });
                }
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<CashFlowCategory1Result> LoadCashFlowCategory5Async(int customerId, int yearId, int branchId, List<UserAdjustmentInput>? userAdjustments)
        {
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            string connectionString = _configuration.GetConnectionString(dbName)
                ?? throw new Exception($"Connection string for '{dbName}' not found.");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var tran = connection.BeginTransaction();
            try
            {
                var result = new CashFlowCategory1Result
                {
                    Particular = new List<CashFlowParticularDto>()
                };

                decimal Safe(decimal? v) => v ?? 0m;

                // -------- FIXED LABEL ORDER ---------
                string[] labels =
                {
                    "Net increase / (decrease) in Cash and cash equivalents (A+B+C)",
                    "Cash and cash equivalents at the beginning of the year",
                    "Cash and cash equivalents at the end of the year",
                    "Reconciliation of Cash and cash equivalents with the Balance Sheet:",
                    "Cash and cash equivalents as per Balance Sheet ",
                    "Less: Bank balances not considered as Cash and cash equivalents as defined in AS 3 Cash Flow Statements (give details)",
                    "Net Cash and cash equivalents (as defined in AS 3 Cash Flow Statements) ",
                    "Add: Current investments considered as part of Cash and cash equivalents (as defined in AS 3 Cash Flow Statements) (Refer Note (ii) to Note 16 Current investments)",
                    "Cash and cash equivalents at the end of the year *",
                    "'* Comprises:",
                    "(a) Cash on hand",
                    "(b) Balances with banks - in current accounts",
                    "(c) Balances with banks - Fixed Deposits",
                    ""
                };

                decimal totalCat5CY = 0m;
                decimal totalCat5PY = 0m;

                // Row 1: B. Cash flow from investing activities
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 1,
                    ParticularName = labels[0]
                });


                // Row 2: Proceeds from issue of equity shares
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 2,
                    ParticularName = labels[1],
                    CurrentYear = 0,
                    PreviousYear = 0
                });

                // Row 3: Share application money received / (refunded)
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[2],
                    CurrentYear = 0,
                    PreviousYear = 0
                });

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[3],
                    CurrentYear = 0,
                    PreviousYear = 0
                });

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[4],
                    CurrentYear = 0,
                    PreviousYear = 0
                });

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[5],
                    CurrentYear = 0,
                    PreviousYear = 0
                });

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[6],
                    CurrentYear = 0,
                    PreviousYear = 0
                });

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[7],
                    CurrentYear = 0,
                    PreviousYear = 0
                });

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[8],
                    CurrentYear = 0,
                    PreviousYear = 0
                });

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[9],
                    CurrentYear = 0,
                    PreviousYear = 0
                });

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[10],
                    CurrentYear = 0,
                    PreviousYear = 0
                });

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[11],
                    CurrentYear = 0,
                    PreviousYear = 0
                });

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[12],
                    CurrentYear = 0,
                    PreviousYear = 0
                });




                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        private async Task<int> GetHeadingId(SqlConnection conn, SqlTransaction tran, int customerId, string headName)
        {
            const string sql = @"SELECT ISNULL(ASH_ID,0) FROM ACC_ScheduleHeading WHERE ASH_Name = @HeadName AND ASH_OrgType = @CustId";
            return await conn.ExecuteScalarAsync<int>(sql, new { HeadName = headName, CustId = customerId }, tran);
        }

        private async Task<int> GetSubHeadingId(SqlConnection conn, SqlTransaction tran, int customerId, string subHeadName)
        {
            const string sql = @"SELECT ISNULL(ASSH_ID,0) FROM ACC_SchedulesubHeading WHERE ASSH_Name like @SubHeadName AND ASSH_OrgType = @CustId";
            return await conn.ExecuteScalarAsync<int>(sql, new { SubHeadName = "%" + subHeadName + "%", CustId = customerId }, tran);
        }

        private async Task<ScheduleAccountingRatioDto.HeadingAmount> GetHeadingAmt(SqlConnection conn, SqlTransaction tran,
            int yearId, int customerId, int schedType, int headingId)
        {
            if (headingId == 0) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            // Ensure SQL names match your schema — adapted for safety
            var sql = @"
                SELECT 
                  ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0)) AS Dc1,
                  ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0)) AS DP1
                FROM Acc_TrailBalance_Upload_Details ud
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YearId = @YearId AND d.ATBU_CustId = @CustomerId and ud.ATBUD_YearId = @YearId
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YearId = @PrevYear AND e.ATBU_CustId = @CustomerId and ud.ATBUD_YearId = @PrevYear
                WHERE ud.ATBUD_Schedule_Type = @SchedType AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_HeadingId = @HeadingId ";

            var row = await conn.QueryFirstOrDefaultAsync(sql, new { YearId = yearId, PrevYear = yearId - 1, CustomerId = customerId, SchedType = schedType, HeadingId = headingId }, tran);

            if (row == null) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            decimal dc1 = row.Dc1 == null ? 0m : Convert.ToDecimal(row.Dc1);
            decimal dp1 = row.DP1 == null ? 0m : Convert.ToDecimal(row.DP1);

            return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = dc1, DP1 = dp1 };
        }

        private async Task<ScheduleAccountingRatioDto.HeadingAmount> GetSubHeadingAmt(SqlConnection conn, SqlTransaction tran,
            int yearId, int customerId, int schedType, int subHeadingId)
        {
            if (subHeadingId == 0) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            var sql = @"
                SELECT 
                  ABS(ISNULL(SUM(d.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(d.ATBU_Closing_TotalDebit_Amount),0)) AS Dc1,
                  ABS(ISNULL(SUM(e.ATBU_Closing_TotalCredit_Amount),0) - ISNULL(SUM(e.ATBU_Closing_TotalDebit_Amount),0)) AS DP1
                FROM Acc_TrailBalance_Upload_Details ud
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YearId = @YearId AND d.ATBU_CustId = @CustomerId and ud.ATBUD_YearId = @YearId
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YearId = @PrevYear AND e.ATBU_CustId = @CustomerId and ud.ATBUD_YearId = @PrevYear
                WHERE ud.ATBUD_Schedule_Type = @SchedType AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_SubHeading = @SubHeadingId";

            var row = await conn.QueryFirstOrDefaultAsync(sql, new { YearId = yearId, PrevYear = yearId - 1, CustomerId = customerId, SchedType = schedType, SubHeadingId = subHeadingId }, tran);

            if (row == null) return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = 0m, DP1 = 0m };

            decimal dc1 = row.Dc1 == null ? 0m : Convert.ToDecimal(row.Dc1);
            decimal dp1 = row.DP1 == null ? 0m : Convert.ToDecimal(row.DP1);

            return new ScheduleAccountingRatioDto.HeadingAmount { Dc1 = dc1, DP1 = dp1 };
        }
        private async Task<decimal> GetPandLFinalAmt(SqlConnection conn, SqlTransaction tran, int yearId, int customerId, int branchId)
        {
            const string sql = @" SELECT ISNULL(SUM(ATBU_Closing_TotalDebit_Amount - ATBU_Closing_TotalCredit_Amount), 0)
                FROM Acc_TrailBalance_Upload
                WHERE ATBU_Description = 'Net Income' AND ATBU_YearId = @YearId AND ATBU_CustId = @CustomerId AND ATBU_BranchId = @BranchId ";
            var value = await conn.ExecuteScalarAsync<decimal?>(sql, new { YearId = yearId, CustomerId = customerId, BranchId = branchId }, tran);
            return value ?? 0m;
        }

 
        public async Task<IEnumerable<CashflowClientDto>> LoadCashFlowClientAsync(int compId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = @"Select Cust_Id,Cust_Name from SAD_CUSTOMER_MASTER Where cust_Compid=@cust_Compid and CUST_DelFlg = 'A' order by Cust_Name";

                var result = await connection.QueryAsync<CashflowClientDto>(query, new
                {
                    cust_Compid = compId,
                });
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading customer user dropdown data.", ex);
            }
        }


        public async Task<IEnumerable<CashflowBranchDto>> LoadBranchDetailsAsync(int ClientID, int compId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = @"select Mas_Id as Branchid,Mas_Description as BranchName from SAD_CUST_LOCATION where Mas_CustID=@Mas_CustID and Mas_CompID=@Mas_CompID";

                var result = await connection.QueryAsync<CashflowBranchDto>(query, new
                {
                    Mas_CustID = ClientID,
                    Mas_CompID = compId,
                });
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading customer user dropdown data.", ex);
            }
        }


        public async Task<IEnumerable<CashflowFinacialYearDto>> LoadFinacialYearAsync(int compId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = @"Select YMS_YEARID, YMS_ID from YEAR_MASTER where YMS_FROMDATE < DATEADD(year,+1,GETDATE()) and YMS_CompId=@YMS_CompId order by YMS_ID desc";

                var result = await connection.QueryAsync<CashflowFinacialYearDto>(query, new
                {
                    YMS_CompId = compId,
                });
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading customer user dropdown data.", ex);
            }
        }


        public async Task<int> SaveCashFlowAsync(CashFlowAddDto model)
        {
            try
            {
                string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
                if (string.IsNullOrEmpty(dbName))
                    throw new Exception("CustomerCode is missing in session. Please log in again.");

                var connectionString = _configuration.GetConnectionString(dbName);

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var transaction = connection.BeginTransaction();

                try
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@ACF_pkid", 0);
                    parameters.Add("@ACF_Description", model.ACF_Description);
                    parameters.Add("@ACF_Custid", model.ACF_Custid);
                    parameters.Add("@ACF_Branchid", model.ACF_Branchid);
                    parameters.Add("@ACF_Current_Amount", 0);
                    parameters.Add("@ACF_Prev_Amount", 0);
                    parameters.Add("@ACF_Status", "C");
                    parameters.Add("@ACF_Crby", 0);
                    parameters.Add("@ACF_Updatedby", 0);
                    parameters.Add("@ACF_Compid", model.ACF_Compid);
                    parameters.Add("@ACF_Ipaddress", "");
                    parameters.Add("@ACF_Catagary", "");
                    parameters.Add("@ACF_Yearid", model.ACF_Yearid);

                    parameters.Add("@iUpdateOrSave", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@iOper", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "spAcc_Cashflow",
                        parameters,
                        transaction,
                        commandType: CommandType.StoredProcedure
                    );

                    await transaction.CommitAsync();

                    return parameters.Get<int>("@iOper");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<ScheduleAccountingRatioDto.SubHeadingDynamicAmount>>GetSubHeadingDynamicDataAmt(SqlConnection conn,SqlTransaction tran,int yearId,int customerId,int schedType)
        {
            var sql = @"SELECT ABS(ISNULL(ACF_Current_Amount, 0)) AS Dc1, ABS(ISNULL(ACF_Prev_Amount, 0)) AS DP1,
                        ACF_Description AS SubHeading FROM acc_cashflow WHERE ACF_Catagary = @SchedType
                        AND ACF_CustId = @CustomerId
                        AND ACF_yearid = @YearId";

            var rows = await conn.QueryAsync(sql,
                new { YearId = yearId, CustomerId = customerId, SchedType = schedType },
                tran);

            var result = new List<ScheduleAccountingRatioDto.SubHeadingDynamicAmount>();

            foreach (var row in rows)
            {
                result.Add(new ScheduleAccountingRatioDto.SubHeadingDynamicAmount
                {
                    Dc1 = row.Dc1 ?? 0m,
                    DP1 = row.DP1 ?? 0m,
                    sSubHeading = row.SubHeading ?? ""
                });
            }

            return result;
        }
    }
}



