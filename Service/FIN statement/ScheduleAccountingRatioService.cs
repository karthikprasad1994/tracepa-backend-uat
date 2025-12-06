using Dapper;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using TracePca.Data;
using TracePca.Dto.FIN_Statement;
using TracePca.Interface.FIN_Statement;
using static TracePca.Dto.FIN_Statement.ScheduleAccountingRatioDto;

namespace TracePca.Service.FIN_statement
{
    public class ScheduleAccountingRatioService : ScheduleAccountingRatioInterface
    {
        private readonly Trdmyus1Context _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScheduleAccountingRatioService(Trdmyus1Context dbcontext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ScheduleAccountingRatioDto.AccountingRatioResult> LoadAccRatioAsync(int yearId, int customerId, int branchId)
        {
            // Get DB name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session.");

            var connectionString = _configuration.GetConnectionString(dbName);

            var result = new ScheduleAccountingRatioDto.AccountingRatioResult();

            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                // define the 11 ratios metadata
                var ratioNames = new[]
                {
                    "Debt Equity Ratio",
                    "Debt Capital",
                    "Debt Service coverage ratio",
                    "Return on Equity Ratio",
                    "Inventory Turnover Ratio",
                    "Trade Receivables turnover ratio",
                    "Trade payables turnover ratio",
                    "Net capital turnover ratio",
                    "Net profit ratio",
                    "Return on Capital employed",
                    "Return on investment"
                };

                var numerators = new[]
                {
                    "Total Current Assets",
                    "Debt Capital",
                    "EBITDA-CAPEX",
                    "Profit for the year",
                    "COGS",
                    "Net Sales",
                    "Total Purchases (Fuel Cost + Other Expenses+Closing Inventory-Opening Inventory)",
                    "Sales",
                    "Net Profit",
                    "Earnings before interest and tax",
                    "Net Profit"
                };

                var denominators = new[]
                {
                    "Total Current Liabilities",
                    "Shareholder's Equity",
                    "Debt Service (Int+Principal)",
                    "Average Shareholder’s Equity",
                    "Average Inventory",
                    "Average trade receivables",
                    "Closing Trade Payables",
                    "Working capital (CA-CL)",
                    "Sales",
                    "Capital Employed",
                    "Investment"
                };

                // Helper for safe division
                decimal SafeDiv(decimal num, decimal den)
                {
                    if (den == 0) return 0m;
                    return num / den;
                }

                // --- Example 1: Debt Equity Ratio (CurrentAssets / CurrentLiabilities)
                {
                    // Adjust heading names to match your DB
                    int headingIdCA = await GetHeadingId(conn, tran, customerId, "2 Current Assets");
                    int headingIdCL = await GetHeadingId(conn, tran, customerId, "4 Current Liabilities");

                    var ca = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headingIdCA);
                    var cl = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headingIdCL);

                    decimal cur = SafeDiv(ca.Dc1, cl.Dc1);
                    decimal prev = SafeDiv(ca.DP1, cl.DP1);

                    result.Ratios.Add(new ScheduleAccountingRatioDto.RatioDto
                    {
                        Sr_No = 1,
                        RatioName = ratioNames[0],
                        Numerator = numerators[0],
                        Denominator = denominators[0],
                        CurrentReportingPeriod = Decimal.Round(cur, 4),
                        PreviousReportingPeriod = Decimal.Round(prev, 4),
                        Change = Decimal.Round(cur - prev, 4)
                    });
                }

                // --- Example 2: Debt Capital (Long-term+Short-term vs Shareholder funds) ---
                {
                    int subLongTermId = await GetSubHeadingId(conn, tran, customerId, "(a) Long-term borrowings");
                    int subShortTermId = await GetSubHeadingId(conn, tran, customerId, "(a) Short Term Borrowings");
                    int headingShareId = await GetHeadingId(conn, tran, customerId, "1 Shareholders Funds");

                    var longTerm = await GetSubHeadingAmt(conn, tran, yearId, customerId, 4, subLongTermId);
                    var shortTerm = await GetSubHeadingAmt(conn, tran, yearId, customerId, 4, subShortTermId);
                    var share = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headingShareId);

                    decimal totalBorrowCurrent = (longTerm.Dc1 + shortTerm.Dc1);
                    decimal totalBorrowPrev = (longTerm.DP1 + shortTerm.DP1);

                    // For shareholder funds we may add P&L — use GetPandLFinalAmt
                    decimal pandlCur = await GetPandLFinalAmt(conn, tran, yearId, customerId, branchId);
                    decimal pandlPrev = await GetPandLFinalAmt(conn, tran, yearId - 1, customerId, branchId);

                    decimal denomCur = share.Dc1 + pandlCur;
                    decimal denomPrev = share.DP1 + pandlPrev;

                    decimal cur = SafeDiv(totalBorrowCurrent, denomCur);
                    decimal prev = SafeDiv(totalBorrowPrev, denomPrev);

                    result.Ratios.Add(new ScheduleAccountingRatioDto.RatioDto
                    {
                        Sr_No = 2,
                        RatioName = ratioNames[1],
                        Numerator = numerators[1],
                        Denominator = denominators[1],
                        CurrentReportingPeriod = Decimal.Round(cur, 4),
                        PreviousReportingPeriod = Decimal.Round(prev, 4),
                        Change = Decimal.Round(cur - prev, 4)
                    });
                }

                // --- Example 3: Debt Service coverage ratio (simplified) ---
                {
                    // This is a sample; adapt to your exact formula & subheadings
                    // We'll try: (EBITDA - CAPEX) / (Interest + Principal)
                    // For denominator search we might use long term borrowings + finance costs
                    decimal ebitdaCur = 0m, ebitdaPrev = 0m;
                    // If you have functions to pull these exact numbers, call them here.
                    // For now we insert 0 to avoid crash and produce an entry (please adapt)
                    result.Ratios.Add(new ScheduleAccountingRatioDto.RatioDto
                    {
                        Sr_No = 3,
                        RatioName = ratioNames[2],
                        Numerator = numerators[2],
                        Denominator = denominators[2],
                        CurrentReportingPeriod = 0m,
                        PreviousReportingPeriod = 0m,
                        Change = 0m
                    });
                }

                // --- Ratio 4: Return on Equity (sample) ---
                {
                    // Use P&L / Average shareholder funds
                    decimal pandlCur = await GetPandLFinalAmt(conn, tran, yearId, customerId, branchId);
                    decimal pandlPrev = await GetPandLFinalAmt(conn, tran, yearId - 1, customerId, branchId);

                    int headingShareId = await GetHeadingId(conn, tran, customerId, "1 Shareholders Funds");
                    var curShare = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headingShareId);
                    var prevShare = await GetHeadingAmt(conn, tran, yearId - 1, customerId, 4, headingShareId);

                    // approximate average shareholder funds (current)
                    decimal avgShareCur = SafeDiv((curShare.Dc1 + pandlCur) + (prevShare.Dc1 + pandlPrev), 2);
                    decimal avgSharePrev = SafeDiv((prevShare.Dc1 + pandlPrev) + (prevShare.DP1 + pandlPrev), 2);

                    decimal cur = SafeDiv(pandlCur, avgShareCur);
                    decimal prev = SafeDiv(pandlPrev, avgSharePrev);

                    result.Ratios.Add(new ScheduleAccountingRatioDto.RatioDto
                    {
                        Sr_No = 4,
                        RatioName = ratioNames[3],
                        Numerator = numerators[3],
                        Denominator = denominators[3],
                        CurrentReportingPeriod = Decimal.Round(cur, 4),
                        PreviousReportingPeriod = Decimal.Round(prev, 4),
                        Change = Decimal.Round(cur - prev, 4)
                    });
                }

                // --- Ratio 5: Inventory Turnover Ratio ---
                {
                    int subCostMaterialId = await GetSubHeadingId(conn, tran, customerId, "(a) Cost Of Materials Consumed");
                    int subInventoriesId = await GetSubHeadingId(conn, tran, customerId, "(b) Inventories");

                    var dtCostSales = await GetSubHeadingAmt(conn, tran, yearId, customerId, 3, subCostMaterialId);
                    var dtCurInv = await GetSubHeadingAmt(conn, tran, yearId, customerId, 4, subInventoriesId);
                    var dtPrevInv = await GetSubHeadingAmt(conn, tran, yearId - 1, customerId, 4, subInventoriesId);

                    decimal avgInvCur = (dtCurInv.Dc1 + dtPrevInv.Dc1) / 2;
                    decimal avgInvPrev = (dtCurInv.DP1 + dtPrevInv.DP1) / 2;

                    decimal cur = SafeDiv(dtCostSales.Dc1, avgInvCur);
                    decimal prev = SafeDiv(dtCostSales.DP1, avgInvPrev);

                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 5,
                        RatioName = ratioNames[4],
                        Numerator = numerators[4],
                        Denominator = denominators[4],
                        CurrentReportingPeriod = Math.Round(cur, 4),
                        PreviousReportingPeriod = Math.Round(prev, 4),
                        Change = Math.Round(cur - prev, 4)
                    });
                }

                // --- Ratio 6: Trade Receivables Turnover Ratio ---
                {
                    int subRevenueId = await GetSubHeadingId(conn, tran, customerId, "I Revenue from operations");
                    int subReceivableId = await GetSubHeadingId(conn, tran, customerId, "(c) Trade receivables");

                    var dtRevenue = await GetSubHeadingAmt(conn, tran, yearId, customerId, 3, subRevenueId);
                    var dtCurRec = await GetSubHeadingAmt(conn, tran, yearId, customerId, 4, subReceivableId);
                    var dtPrevRec = await GetSubHeadingAmt(conn, tran, yearId - 1, customerId, 4, subReceivableId);

                    decimal avgRecCur = (dtCurRec.Dc1 + dtPrevRec.Dc1) / 2;
                    decimal avgRecPrev = (dtCurRec.DP1 + dtPrevRec.DP1) / 2;

                    decimal cur = SafeDiv(dtRevenue.Dc1, avgRecCur);
                    decimal prev = SafeDiv(dtRevenue.DP1, avgRecPrev);

                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 6,
                        RatioName = ratioNames[5],
                        Numerator = numerators[5],
                        Denominator = denominators[5],
                        CurrentReportingPeriod = Math.Round(cur, 4),
                        PreviousReportingPeriod = Math.Round(prev, 4),
                        Change = Math.Round(cur - prev, 4)
                    });
                }

                // --- Ratio 7: Trade Payables Turnover Ratio ---
                {
                    int subCostOfGoodsId = await GetSubHeadingId(conn, tran, customerId, "(a) Cost Of Materials Consumed");
                    int subTradePayId = await GetSubHeadingId(conn, tran, customerId, "(b) Trade payables");

                    var dtCOGS = await GetSubHeadingAmt(conn, tran, yearId, customerId, 3, subCostOfGoodsId);
                    var dtPay = await GetSubHeadingAmt(conn, tran, yearId, customerId, 4, subTradePayId);

                    decimal cur = SafeDiv(dtCOGS.Dc1, dtPay.Dc1);
                    decimal prev = SafeDiv(dtCOGS.DP1, dtPay.DP1);

                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 7,
                        RatioName = ratioNames[6],
                        Numerator = numerators[6],
                        Denominator = denominators[6],
                        CurrentReportingPeriod = Math.Round(cur, 4),
                        PreviousReportingPeriod = Math.Round(prev, 4),
                        Change = Math.Round(cur - prev, 4)
                    });
                }

                // --- Ratio 8: Net Capital Turnover Ratio ---
                {
                    int headCLA = await GetHeadingId(conn, tran, customerId, "4 Current Liabilities");
                    int headCSA = await GetHeadingId(conn, tran, customerId, "2 Current Assets");

                    var dtCL = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headCLA);
                    var dtCA = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headCSA);

                    decimal workingCapCur = dtCA.Dc1 - dtCL.Dc1;
                    decimal workingCapPrev = dtCA.DP1 - dtCL.DP1;

                    int subRevenueId = await GetSubHeadingId(conn, tran, customerId, "I Revenue from operations");
                    var dtRevenue = await GetSubHeadingAmt(conn, tran, yearId, customerId, 3, subRevenueId);

                    decimal cur = SafeDiv(dtRevenue.Dc1, workingCapCur);
                    decimal prev = SafeDiv(dtRevenue.DP1, workingCapPrev);

                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 8,
                        RatioName = ratioNames[7],
                        Numerator = numerators[7],
                        Denominator = denominators[7],
                        CurrentReportingPeriod = Math.Round(cur, 4),
                        PreviousReportingPeriod = Math.Round(prev, 4),
                        Change = Math.Round(cur - prev, 4)
                    });
                }


                // --- Ratio 9: Net Profit Ratio ---
                {
                    decimal plCur = await GetPandLFinalAmt(conn, tran, yearId, customerId, branchId);
                    decimal plPrev = await GetPandLFinalAmt(conn, tran, yearId - 1, customerId, branchId);

                    int subRevenueId = await GetSubHeadingId(conn, tran, customerId, "I Revenue from operations");
                    var dtRevenue = await GetSubHeadingAmt(conn, tran, yearId, customerId, 3, subRevenueId);

                    decimal cur = SafeDiv(plCur, dtRevenue.Dc1);
                    decimal prev = SafeDiv(plPrev, dtRevenue.DP1);

                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 9,
                        RatioName = ratioNames[8],
                        Numerator = numerators[8],
                        Denominator = denominators[8],
                        CurrentReportingPeriod = Math.Round(cur, 4),
                        PreviousReportingPeriod = Math.Round(prev, 4),
                        Change = Math.Round(cur - prev, 4)
                    });
                }

                // --- Ratio 10: Return on Capital Employed ---
                {
                    int headIncomeId = await GetHeadingId(conn, tran, customerId, "Income");
                    int headExpenseId = await GetHeadingId(conn, tran, customerId, "IV Expenses");

                    var dtIncome = await GetHeadingAmt(conn, tran, yearId, customerId, 3, headIncomeId);
                    var dtExpense = await GetHeadingAmt(conn, tran, yearId, customerId, 3, headExpenseId);

                    decimal profitCur = dtIncome.Dc1 - dtExpense.Dc1;
                    decimal profitPrev = dtIncome.DP1 - dtExpense.DP1;

                    int headShareId = await GetHeadingId(conn, tran, customerId, "1 Shareholders Funds");
                    var dtShare = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headShareId);
                    var dtSharePrev = await GetHeadingAmt(conn, tran, yearId - 1, customerId, 4, headShareId);

                    decimal plCur = await GetPandLFinalAmt(conn, tran, yearId, customerId, branchId);
                    decimal plPrev = await GetPandLFinalAmt(conn, tran, yearId - 1, customerId, branchId);

                    decimal capCur = dtShare.Dc1 + plCur;
                    decimal capPrev = dtSharePrev.DP1 + plPrev;

                    decimal cur = SafeDiv(profitCur, capCur);
                    decimal prev = SafeDiv(profitPrev, capPrev);

                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 10,
                        RatioName = ratioNames[9],
                        Numerator = numerators[9],
                        Denominator = denominators[9],
                        CurrentReportingPeriod = Math.Round(cur, 4),
                        PreviousReportingPeriod = Math.Round(prev, 4),
                        Change = Math.Round(cur - prev, 4)
                    });
                }

                // --- Ratio 11: Return on Investment ---
                {
                    decimal plCur = await GetPandLFinalAmt(conn, tran, yearId, customerId, branchId);
                    decimal plPrev = await GetPandLFinalAmt(conn, tran, yearId - 1, customerId, branchId);

                    int subCurrentInv = await GetSubHeadingId(conn, tran, customerId, "(a) Current Investments");
                    int subNonCurrentInv = await GetSubHeadingId(conn, tran, customerId, "(b) Non-current investments");

                    var dtCurInv = await GetSubHeadingAmt(conn, tran, yearId, customerId, 4, subCurrentInv);
                    var dtNonInv = await GetSubHeadingAmt(conn, tran, yearId, customerId, 4, subNonCurrentInv);

                    decimal totalInvestCur = dtCurInv.Dc1 + dtNonInv.Dc1;
                    decimal totalInvestPrev = dtCurInv.DP1 + dtNonInv.DP1;

                    decimal cur = SafeDiv(plCur, totalInvestCur);
                    decimal prev = SafeDiv(plPrev, totalInvestPrev);

                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 11,
                        RatioName = ratioNames[10],
                        Numerator = numerators[10],
                        Denominator = denominators[10],
                        CurrentReportingPeriod = Math.Round(cur, 4),
                        PreviousReportingPeriod = Math.Round(prev, 4),
                        Change = Math.Round(cur - prev, 4)
                    });
                }


                tran.Commit();
                return result;
            }
            catch
            {
                try { tran.Rollback(); } catch { }
                throw;
            }
        }

        #region DB Helper Methods

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
            const string sql = @"
                SELECT ISNULL(SUM(ATBU_Closing_TotalDebit_Amount - ATBU_Closing_TotalCredit_Amount), 0)
                FROM Acc_TrailBalance_Upload
                WHERE ATBU_Description = 'Net Income' AND ATBU_YearId = @YearId AND ATBU_CustId = @CustomerId AND ATBU_BranchId = @BranchId
            ";
            var value = await conn.ExecuteScalarAsync<decimal?>(sql, new { YearId = yearId, CustomerId = customerId, BranchId = branchId }, tran);
            return value ?? 0m;
        }

        #endregion

    }

}

