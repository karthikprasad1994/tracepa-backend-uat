using Dapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using TracePca.Data;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.CashFlowDto;

namespace TracePca.Service.FIN_statement
{
    public class CashFlowService : CashFlowInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CashFlowService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
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





        public async Task<CashFlowCategory1Result> LoadCashFlowCategory1Async(
    int customerId, int yearId, int branchId, List<UserAdjustmentInput>? userAdjustments)
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

                // Mandatory rows labels (kept same order)
                string[] labels =
                {
            "A. Cash flow from operating activities",
            "Net Profit / (Loss) before extraordinary items and tax",
            "Adjustment for:",
            "Depreciation and amortisation",
            "Provision for impairment of fixed assets and intangibles",
            "Bad Debts",
            "Expense on employee stock option scheme",
            "Finance Costs",
            "Interest income",
            "Total Adjustment",
            "Operating profit / (loss) before working capital changes"
        };

                // safe conversion for nullable decimals
                decimal Safe(decimal? v) => v ?? 0m;

                // A. heading
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 1,
                    ParticularName = labels[0]
                });

                // 2. Net Profit = Income - Expenses (use GetHeadingAmt which returns HeadingAmount)
                {
                    int headIncomeId = await GetHeadingId(connection, tran, customerId, "Income");
                    int headExpenseId = await GetHeadingId(connection, tran, customerId, "Expenses");

                    // call helpers that accept transaction (matching the code you pasted previously)
                    var income = await GetHeadingAmt(connection, tran, yearId, customerId, 3, headIncomeId);
                    var expense = await GetHeadingAmt(connection, tran, yearId, customerId, 3, headExpenseId);

                    // your helper returns ScheduleAccountingRatioDto.HeadingAmount with Dc1/DP1
                    decimal cy = Safe(income.Dc1) - Safe(expense.Dc1);
                    decimal py = Safe(income.DP1) - Safe(expense.DP1);

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = 2,
                        ParticularName = labels[1],
                        CurrentYear = cy,
                        PreviousYear = py
                    });
                }

                // 3. Adjustment heading
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = 3,
                    ParticularName = labels[2]
                });

                decimal totalAdjustmentCY = 0m;
                decimal totalAdjustmentPY = 0m;
                int srNo = 4;

                // Helper local function to add adjustment rows and accumulate totals
                async Task AddAdjRow(int sr, string label, Func<Task<(decimal cy, decimal py)>> fetcher)
                {
                    var (cy, py) = await fetcher();
                    totalAdjustmentCY += cy;
                    totalAdjustmentPY += py;

                    result.Particular.Add(new CashFlowParticularDto
                    {
                        Sr_No = sr,
                        ParticularName = label,
                        CurrentYear = cy,
                        PreviousYear = py
                    });
                }

                // 4. Depreciation
                await AddAdjRow(4, labels[3], async () =>
                {
                    int id = await GetHeadingId(connection, tran, customerId, "Depreciation and amortisation");
                    var dt = await GetHeadingAmt(connection, tran, yearId, customerId, 3, id);
                    return (Safe(dt.Dc1), Safe(dt.DP1));
                });

                // 5. Provision for impairment
                await AddAdjRow(5, labels[4], async () =>
                {
                    int id = await GetHeadingId(connection, tran, customerId, "Provision for impairment of fixed assets and intangibles");
                    var dt = await GetHeadingAmt(connection, tran, yearId, customerId, 3, id);
                    return (Safe(dt.Dc1), Safe(dt.DP1));
                });

                // 6. Bad Debts
                await AddAdjRow(6, labels[5], async () =>
                {
                    int id = await GetHeadingId(connection, tran, customerId, "Bad Debts");
                    var dt = await GetHeadingAmt(connection, tran, yearId, customerId, 3, id);
                    return (Safe(dt.Dc1), Safe(dt.DP1));
                });

                // 7. ESOP expense
                await AddAdjRow(7, labels[6], async () =>
                {
                    int id = await GetHeadingId(connection, tran, customerId, "Expense on employee stock option scheme");
                    var dt = await GetHeadingAmt(connection, tran, yearId, customerId, 3, id);
                    return (Safe(dt.Dc1), Safe(dt.DP1));
                });

                // 8. Finance costs (example used subheading)
                await AddAdjRow(8, labels[7], async () =>
                {
                    int id = await GetSubHeadingId(connection, tran, customerId, "Finance costs");
                    var dt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 3, id);
                    return (Safe(dt.Dc1), Safe(dt.DP1));
                });

                // 9. Interest income (subheading)
                await AddAdjRow(9, labels[8], async () =>
                {
                    int id = await GetSubHeadingId(connection, tran, customerId, "Interest income");
                    var dt = await GetSubHeadingAmt(connection, tran, yearId, customerId, 3, id);
                    return (Safe(dt.Dc1), Safe(dt.DP1));
                });

                // 10. user added adjustments (if any)
                if (userAdjustments != null && userAdjustments.Any())
                {
                    foreach (var ua in userAdjustments)
                    {
                        totalAdjustmentCY += ua.CurrentYear;
                        totalAdjustmentPY += ua.PreviousYear;

                        result.Particular.Add(new CashFlowParticularDto
                        {
                            Sr_No = srNo++,
                            ParticularName = ua.Description,
                            CurrentYear = ua.CurrentYear,
                            PreviousYear = ua.PreviousYear
                        });
                    }
                }

                // Total Adjustment row
                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = srNo++,
                    ParticularName = labels[9],
                    CurrentYear = totalAdjustmentCY,
                    PreviousYear = totalAdjustmentPY
                });

                // Operating profit / (loss) before working capital changes = NetProfit + TotalAdjustment
                var netProfit = result.Particular.First(p => p.Sr_No == 2);

                result.Particular.Add(new CashFlowParticularDto
                {
                    Sr_No = srNo,
                    ParticularName = labels[10],
                    CurrentYear = netProfit.CurrentYear + totalAdjustmentCY,
                    PreviousYear = netProfit.PreviousYear + totalAdjustmentPY
                });

                tran.Commit();
                return result;
            }
            catch
            {
                try { tran.Rollback(); } catch { }
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
            const string sql = @"SELECT ISNULL(ASSH_ID,0) FROM ACC_SchedulesubHeading WHERE ASSH_Name = @SubHeadName AND ASSH_OrgType = @CustId";
            return await conn.ExecuteScalarAsync<int>(sql, new { SubHeadName = subHeadName, CustId = customerId }, tran);
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
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YearId = @YearId AND d.ATBU_CustId = @CustomerId
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YearId = @PrevYear AND e.ATBU_CustId = @CustomerId
                WHERE ud.ATBUD_Schedule_Type = @SchedType AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_HeadingId = @HeadingId
            ";

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
                LEFT JOIN Acc_TrailBalance_Upload d ON d.ATBU_Description = ud.ATBUD_Description AND d.ATBU_YearId = @YearId AND d.ATBU_CustId = @CustomerId
                LEFT JOIN Acc_TrailBalance_Upload e ON e.ATBU_Description = ud.ATBUD_Description AND e.ATBU_YearId = @PrevYear AND e.ATBU_CustId = @CustomerId
                WHERE ud.ATBUD_Schedule_Type = @SchedType AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_SubHeading = @SubHeadingId
            ";

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
    }
}



