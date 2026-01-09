using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.RegularExpressions;
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
                    "Current ratio",
                    "Debt Equity ratio",
                    "Debt Service coverage ratio",
                    "Return on Equity Ratio",
                    "Inventory Turnover Ratio",
                    "Trade Receivables turnover ratio",
                    "Trade payables turnover ratio",
                    "Net working capital Ratio",
                    "Net profit/ sales",
                    "Return/capital employed",
                    "Return/ investment"
                };
                var Formula = new[]
             {
                    "Current Assets / Current Liabilities",
                    "Total Debt / Shareholders fund",
                    "Net  Income  / Total Debt",
                    "Net Income / Shareholders fund",
                    "Net Sales / Inventory ",
                    "Net Sales / Accounts Receivable",
                    "Net Purchases / Accounts Payable",
                    "Net Sales / Current assets- current liability ",
                    "Net Income / Net Sales",
                    "Net Income / Total Assets - Current Liability ",
                    "Net Income/ Shareholders fund"
                };

                //var numerators = new[]
                //{
                //    "Total Current Assets",
                //    "Debt Capital",
                //    "EBITDA-CAPEX",
                //    "Profit for the year",
                //    "COGS",
                //    "Net Sales",
                //    "Total Purchases (Fuel Cost + Other Expenses+Closing Inventory-Opening Inventory)",
                //    "Sales",
                //    "Net Profit",
                //    "Earnings before interest and tax",
                //    "Net Profit"
                //};

                //var denominators = new[]
                //{
                //    "Total Current Liabilities",
                //    "Shareholder's Equity",
                //    "Debt Service (Int+Principal)",
                //    "Average Shareholder’s Equity",
                //    "Average Inventory",
                //    "Average trade receivables",
                //    "Closing Trade Payables",
                //    "Working capital (CA-CL)",
                //    "Sales",
                //    "Capital Employed",
                //    "Investment"
                //};

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
                    int headingIdCL = await GetHeadingId(conn, tran, customerId, "Current Liabilities");

                    var ca = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headingIdCA);
                    var cl = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headingIdCL);

                    decimal cur = SafeDiv(ca.Dc1, cl.Dc1);
                    decimal prev = SafeDiv(ca.DP1, cl.DP1);

                    result.Ratios.Add(new ScheduleAccountingRatioDto.RatioDto
                    {
                        Sr_No = 1,
                        RatioName = ratioNames[0],
                        Formula= Formula[0],
                        Numerator = (Decimal.Round(ca.Dc1,2)).ToString(),
                        Denominator = (Decimal.Round(cl.Dc1,2)).ToString(),
                        CurrentReportingPeriod = Decimal.Round(cur, 4),
                        PreviousReportingPeriod = Decimal.Round(prev, 4),
                        Change = Math.Abs(Decimal.Round(cur - prev, 4))
                    });
                }

                // --- Example 2: Debt Capital (Long-term+Short-term vs Shareholder funds) ---
                {
                    //int subLongTermId = await GetSubHeadingId(conn, tran, customerId, "(a) Long-term borrowings");
                    //int subShortTermId = await GetSubHeadingId(conn, tran, customerId, "(a) Short Term Borrowings");

                    int headingNonCrLiablId = await GetHeadingId(conn, tran, customerId, "Non-current Liabilities");
                    int headingShareId = await GetHeadingId(conn, tran, customerId, "Shareholders  funds");

                    var longTerm = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headingNonCrLiablId);                
                    var share = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headingShareId);

                  

                    decimal cur = SafeDiv(longTerm.Dc1, share.Dc1);
                    decimal prev = SafeDiv(longTerm.DP1, share.DP1);

                    result.Ratios.Add(new ScheduleAccountingRatioDto.RatioDto
                    {
                        Sr_No = 2,
                        RatioName = ratioNames[1],
                        Formula = Formula[1],
                        Numerator = (Decimal.Round(longTerm.Dc1, 2)).ToString(),
                        Denominator = (Decimal.Round(share.Dc1, 2)).ToString(),
                        CurrentReportingPeriod = Decimal.Round(cur, 4),
                        PreviousReportingPeriod = Decimal.Round(prev, 4),
                        Change = Math.Abs(Decimal.Round(cur - prev, 4))
                    });
                }

                // --- Example 3: Debt Service coverage ratio (simplified) ---
                {
                    decimal plCur = await GetPandLFinalAmt(conn, tran, yearId, customerId, branchId);
                    decimal plPrev = await GetPandLFinalAmt(conn, tran, yearId - 1, customerId, branchId);
                    int headingNonCrLiablId = await GetHeadingId(conn, tran, customerId, "Non-current Liabilities");
                    var longTerm = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headingNonCrLiablId);

                    decimal cur = SafeDiv(plCur,longTerm.Dc1);
                    decimal prev = SafeDiv(plPrev ,longTerm.DP1);

                    result.Ratios.Add(new ScheduleAccountingRatioDto.RatioDto
                    {
                        Sr_No = 3,
                        RatioName = ratioNames[2],
                        Formula = Formula[2],
                        Numerator = (Decimal.Round(plCur, 2)).ToString(),
                        Denominator = (Decimal.Round(longTerm.Dc1, 2)).ToString(),
                        CurrentReportingPeriod = cur,
                        PreviousReportingPeriod = prev,
                        Change = Math.Abs(Decimal.Round(cur - prev, 4))
                    });
                }

                // --- Ratio 4: Return on Equity (sample) ---
                {
                    // Use P&L / Average shareholder funds
                    decimal pandlCur = await GetPandLFinalAmt(conn, tran, yearId, customerId, branchId);
                    decimal pandlPrev = await GetPandLFinalAmt(conn, tran, yearId - 1, customerId, branchId);

                    int headingShareId = await GetHeadingId(conn, tran, customerId, "Shareholders  funds");
                    var share = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headingShareId);

                    decimal cur = SafeDiv(pandlCur, share.Dc1);
                    decimal prev = SafeDiv(pandlPrev, share.DP1);

                    result.Ratios.Add(new ScheduleAccountingRatioDto.RatioDto
                    {
                        Sr_No = 4,
                        RatioName = ratioNames[3],
                        Formula = Formula[3],
                        Numerator = (Decimal.Round(pandlCur, 2)).ToString(),
                        Denominator = (Decimal.Round(share.Dc1, 2)).ToString(),
                        CurrentReportingPeriod = Decimal.Round(cur, 4),
                        PreviousReportingPeriod = Decimal.Round(prev, 4),
                        Change = Math.Abs(Decimal.Round(cur - prev, 4))
                    });
                }

                // --- Ratio 5: Inventory Turnover Ratio ---
                {
                    int subRevenueId = await GetSubHeadingId(conn, tran, customerId, "I Revenue from operations");
                    var dtRevenue = await GetSubHeadingAmt(conn, tran, yearId, customerId, 3, subRevenueId);
            
                    int subInventoriesId = await GetSubHeadingId(conn, tran, customerId, "(b) Inventories");                    
                    var dtCurInv = await GetSubHeadingAmt(conn, tran, yearId, customerId, 4, subInventoriesId);
                             
                    decimal cur = SafeDiv(dtRevenue.Dc1, dtCurInv.Dc1);
                    decimal prev = SafeDiv(dtRevenue.DP1, dtCurInv.DP1);
                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 5,
                        RatioName = ratioNames[4],
                        Formula = Formula[4],
                        Numerator = (Decimal.Round(dtRevenue.Dc1, 2)).ToString(),
                        Denominator = (Decimal.Round(dtRevenue.Dc1, 2)).ToString(),
                        CurrentReportingPeriod = Math.Round(cur, 4),
                        PreviousReportingPeriod = Math.Round(prev, 4),
                        Change = Math.Abs(Math.Round(cur - prev, 4))
                    });
                }

                // --- Ratio 6: Trade Receivables Turnover Ratio ---
                {
                    int subRevenueId = await GetSubHeadingId(conn, tran, customerId, "I Revenue from operations");
                    int subReceivableId = await GetSubHeadingId(conn, tran, customerId, "(c) Trade receivables");

                    var dtRevenue = await GetSubHeadingAmt(conn, tran, yearId, customerId, 3, subRevenueId);
                    var dtCurRec = await GetSubHeadingAmt(conn, tran, yearId, customerId, 4, subReceivableId);                           

                    decimal cur = SafeDiv(dtRevenue.Dc1, dtCurRec.Dc1);
                    decimal prev = SafeDiv(dtRevenue.DP1, dtCurRec.DP1);
                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 6,
                        RatioName = ratioNames[5] ,
                        Formula = Formula[5],
                        Numerator = (Decimal.Round(dtRevenue.Dc1, 2)).ToString(),
                        Denominator = (Decimal.Round(dtCurRec.Dc1, 2)).ToString(),
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
                        Formula = Formula[6],
                        Numerator = (Decimal.Round(dtCOGS.Dc1, 2)).ToString(),
                        Denominator = (Decimal.Round(dtPay.Dc1, 2)).ToString(),
                        CurrentReportingPeriod = Math.Abs(Math.Round(cur, 4)),
                        PreviousReportingPeriod = Math.Abs(Math.Round(prev, 4)),
                        Change = Math.Abs(Math.Round(cur - prev, 4))
                    });
                }

                // --- Ratio 8: Net Capital Turnover Ratio ---
                {
                    int headCLA = await GetHeadingId(conn, tran, customerId, "Current Liabilities");
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
                        Formula = Formula[7],
                        Numerator = (Decimal.Round(dtRevenue.Dc1, 2)).ToString(),
                        Denominator = (Decimal.Round(workingCapCur,2 )).ToString(),
                        CurrentReportingPeriod = Math.Abs(Math.Round(cur, 4)),
                        PreviousReportingPeriod = Math.Abs(Math.Round(prev, 4)),
                        Change = Math.Abs(Math.Round(cur - prev, 4))
                    });
                }


                // --- Ratio 9: Net Profit Ratio ---
                {
                    decimal plCur = await GetPandLFinalAmt(conn, tran, yearId, customerId, branchId);
                    decimal plPrev = await GetPandLFinalAmt(conn, tran, yearId - 1, customerId, branchId);

                    int subRevenueId = await GetSubHeadingId(conn, tran, customerId, "I Revenue from operations");
                    var dtRevenue = await GetSubHeadingAmt(conn, tran, yearId, customerId, 3, subRevenueId);

                    decimal cur = (SafeDiv(plCur, dtRevenue.Dc1)*100);
                    decimal prev = SafeDiv(plPrev, dtRevenue.DP1);

                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 9,
                        RatioName = ratioNames[8],
                        Formula = Formula[8],
                        Numerator = (Decimal.Round(plCur, 2)).ToString(),
                        Denominator = (Decimal.Round(dtRevenue.Dc1, 2)).ToString(),
                        CurrentReportingPeriod = (Math.Round(cur, 4)), //+ "%"
                        PreviousReportingPeriod = Math.Round(prev, 4),
                        Change = Math.Abs(Math.Round(cur - prev, 4))
                    });
                }

                // --- Ratio 10: Return on Capital Employed ---
                {
                    int headNCAssetId = await GetHeadingId(conn, tran, customerId, "1 Non-Current Assets");
                    int headCAssetId = await GetHeadingId(conn, tran, customerId, "2 Current Assets");
                    var dtNCAsset = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headNCAssetId);
                    var dtCAsset = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headCAssetId);
                    decimal AssetCur = dtNCAsset.Dc1 + dtCAsset.Dc1;
                    decimal AssetPrev = dtNCAsset.DP1 + dtCAsset.DP1;

                    int headShareId = await GetHeadingId(conn, tran, customerId, "Current Liabilities");
                    var dtShare = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headShareId);

                    decimal capCur = Math.Abs(dtShare.Dc1 - AssetCur);
                    decimal capPrev = Math.Abs(dtShare.DP1 - AssetPrev);


                    decimal plCur = await GetPandLFinalAmt(conn, tran, yearId, customerId, branchId);
                    decimal plPrev = await GetPandLFinalAmt(conn, tran, yearId - 1, customerId, branchId);                   

                    decimal cur = Math.Abs((SafeDiv(plCur, capCur) * 100));
                    decimal prev = Math.Abs((SafeDiv(plPrev, capPrev)* 100));

                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 10,
                        RatioName = ratioNames[9],
                        Formula = Formula[9],
                        Numerator = (Decimal.Round(plCur, 2)).ToString(),
                        Denominator = (Decimal.Round(capCur, 2)).ToString(),
                        CurrentReportingPeriod = Math.Abs(Math.Round(cur, 4)),
                        PreviousReportingPeriod = Math.Abs(Math.Round(prev, 4)),
                        Change = Math.Abs(Math.Round(cur - prev, 4))
                    });
                }

                // --- Ratio 11: Return on Investment ---
                {
                    decimal plCur = await GetPandLFinalAmt(conn, tran, yearId, customerId, branchId);
                    decimal plPrev = await GetPandLFinalAmt(conn, tran, yearId - 1, customerId, branchId);

                    int headingShareId = await GetHeadingId(conn, tran, customerId, "Shareholders  funds");

                    var share = await GetHeadingAmt(conn, tran, yearId, customerId, 4, headingShareId);

                    decimal cur = (SafeDiv(plCur, share.Dc1)*100);
                    decimal prev = (SafeDiv(plPrev, share.DP1)*100);            

                    result.Ratios.Add(new RatioDto
                    {
                        Sr_No = 11,
                        RatioName = ratioNames[10],
                        Formula = Formula[10],
                        Numerator = (Decimal.Round(plCur, 2)).ToString(),
                        Denominator = (Decimal.Round(share.Dc1, 2)).ToString(),
                        CurrentReportingPeriod = Math.Abs(Math.Round(cur, 4)),
                        PreviousReportingPeriod = Math.Abs(Math.Round(prev, 4)),
                        Change = Math.Abs(Math.Round(cur - prev, 4))
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
                WHERE ud.ATBUD_Schedule_Type = @SchedType AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_HeadingId = @HeadingId and ud.ATBUD_yearid=@YearId
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
                WHERE ud.ATBUD_Schedule_Type = @SchedType AND ud.ATBUD_CustId = @CustomerId AND ud.ATBUD_SubHeading = @SubHeadingId and ud.ATBUD_yearid=@YearId
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
                SELECT Abs(ISNULL(SUM(ATBU_Closing_TotalDebit_Amount - ATBU_Closing_TotalCredit_Amount), 0))
                FROM Acc_TrailBalance_Upload
                WHERE ATBU_Description = 'Net Income' AND ATBU_YearId = @YearId AND ATBU_CustId = @CustomerId AND ATBU_BranchId = @BranchId
            ";
            var value = await conn.ExecuteScalarAsync<decimal?>(sql, new { YearId = yearId, CustomerId = customerId, BranchId = branchId }, tran);
            return value ?? 0m;
        }

        #endregion

    }

}

